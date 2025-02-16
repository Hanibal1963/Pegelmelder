' ImageRenderer.vb
' (c) 2025 by Andreas Sauer
'

Imports SkiaSharp
Imports System.Globalization
Imports System.Text

Friend Class ImageRenderer

  Private ReadOnly dataset As New List(Of Tuple(Of Date, Integer))()
  Private ReadOnly uniqueDates As New SortedSet(Of Date)()
  Private bitmap As New SKBitmap
  Private drawRect As New SKRect
  Private canvas As SKCanvas
  Private base64Image As String

  Public Property Width As Integer = 800 'Breite der Bitmap
  Public Property Height As Integer = 500  'H�he der Bimap
  Public Property LineColor As SKColor = SKColors.Red 'Farbe der Datenlinien
  Public Property AxisColor As SKColor = SKColors.Black 'Farbe der Koordinatenlinien
  Public Property GridColor As SKColor = SKColors.LightGray  'Farbe der Gitternetzlinien
  Public Property PointColor As SKColor = SKColors.Blue 'Farbe der Datenpunkte
  Public Property TextColor As SKColor = SKColors.Black 'Farbe des Textes
  Public Property BackColor As SKColor = SKColors.White 'Hintegrundfarbe
  Public Property AltText As String = "Diagramm" 'Alternativtext des Bildes
  Public Property Caption As String = ""  'Beschriftungstext unterhalb des Diagramms
  Public Property Padding As Integer = 0 'Breite der R�nder um das Diagramm einschlie�lich des Beschriftungstextes
  Public Property TextSize As Single = 12  'Schriftgr��e

  Public Sub New(dataLines As String())
    Me.ParseDataLines(dataLines)
  End Sub

  Public Function GenerateImageTag() As String
    Me.CreateBitmap() ' Erstelle die Bitmap mit den angegebenen Abmessungen
    Me.CreateDrawRect() ' Erstelle die Zeichenfl�che unter Ber�cksichtigung des Paddings
    Me.DrawAxes() ' Zeicnnet die Achsen des Koordinatensystems
    Me.DrawYAxisLabels() ' Zeichnet die Beschriftung der y-Achse
    Me.DrawXAxisLabels() ' Zeichnet die Beschriftung der x-Achse
    Me.DrawDataLines() ' Zeichnet die Linien zwischen den Datenpunkten
    Me.DrawDataPoints() ' Zeichnet die Datenpunkte
    Me.DrawCaption() ' Zeichnet die Beschriftung unterhalb der x-Achse
    Me.EncodeImageToBase64() ' Bild als PNG in Base64 kodieren
    Return $"<img src='data:image/png;base64,{Me.base64Image}' alt='{Me.AltText}'/>"  ' Das img-Tag zur�ckgeben
  End Function

  Private Sub DrawDataLines()
    Using paint As New SKPaint()
      paint.Color = Me.LineColor
      paint.IsAntialias = True
      paint.StrokeWidth = 2

      ' Bestimme den maximalen und minimalen Wert im Datensatz
      Dim minValue As Integer = Me.dataset.Min(Function(t) t.Item2)
      Dim maxValue As Integer = Me.dataset.Max(Function(t) t.Item2)

      ' Zeichne die Linien zwischen den Datenpunkten
      For i As Integer = 0 To Me.dataset.Count - 2
        Dim x1 As Single = CSng(Me.drawRect.Left + (Me.drawRect.Width * (Me.dataset(i).Item1 - Me.uniqueDates.Min).TotalDays / (Me.uniqueDates.Max - Me.uniqueDates.Min).TotalDays))
        Dim y1 As Single = CSng(Me.drawRect.Bottom - ((Me.dataset(i).Item2 - minValue) / (maxValue - minValue) * Me.drawRect.Height))
        Dim x2 As Single = CSng(Me.drawRect.Left + (Me.drawRect.Width * (Me.dataset(i + 1).Item1 - Me.uniqueDates.Min).TotalDays / (Me.uniqueDates.Max - Me.uniqueDates.Min).TotalDays))
        Dim y2 As Single = CSng(Me.drawRect.Bottom - ((Me.dataset(i + 1).Item2 - minValue) / (maxValue - minValue) * Me.drawRect.Height))

        Me.canvas.DrawLine(x1, y1, x2, y2, paint)
      Next
    End Using
  End Sub

  Private Sub DrawDataPoints()
    Using paint As New SKPaint()
      paint.Color = Me.PointColor
      paint.IsAntialias = True
      paint.Style = SKPaintStyle.Fill

      ' Bestimme den maximalen und minimalen Wert im Datensatz
      Dim minValue As Integer = Me.dataset.Min(Function(t) t.Item2)
      Dim maxValue As Integer = Me.dataset.Max(Function(t) t.Item2)

      ' Zeichne jeden Datenpunkt
      For Each dataPoint As Tuple(Of Date, Integer) In Me.dataset
        Dim x As Single = CSng(Me.drawRect.Left + (Me.drawRect.Width * (dataPoint.Item1 - Me.uniqueDates.Min).TotalDays / (Me.uniqueDates.Max - Me.uniqueDates.Min).TotalDays))
        Dim y As Single = CSng(Me.drawRect.Bottom - ((dataPoint.Item2 - minValue) / (maxValue - minValue) * Me.drawRect.Height))

        Me.canvas.DrawCircle(x, y, 3, paint)
      Next
    End Using
  End Sub

  Private Function CalculateTextHeight() As Single
    Using paint As New SKPaint()
      paint.TextSize = Me.TextSize
      paint.IsAntialias = True

      ' Berechne die Textmetriken
      Dim metrics As SKFontMetrics = paint.FontMetrics
      ' Die H�he des Textes ist der Abstand von der h�chsten zur tiefsten Linie
      Dim textHeight As Single = metrics.Descent - metrics.Ascent
      Return textHeight
    End Using
  End Function

  Private Sub DrawCaption()
    If String.IsNullOrEmpty(Me.Caption) Then Return

    Using paint As New SKPaint()
      paint.Color = Me.TextColor
      paint.TextSize = Me.TextSize
      paint.IsAntialias = True

      Dim x As Single = Me.drawRect.Left
      Dim y As Single = Me.drawRect.Bottom + 10 + (2 * Me.TextSize)

      Me.canvas.DrawText(Me.Caption, x, y, paint)
    End Using
  End Sub

  Private Sub DrawXAxisLabels()
    Using paint As New SKPaint()
      paint.Color = Me.TextColor
      paint.TextSize = Me.TextSize
      paint.IsAntialias = True

      ' Bestimme die maximale Breite eines Labels
      Dim maxLabelWidth As Single = 0
      For Each datum As Tuple(Of Date, Integer) In Me.dataset
        Dim textWidth As Single = paint.MeasureText(datum.Item1.ToString("dd.MM."))
        If textWidth > maxLabelWidth Then
          maxLabelWidth = textWidth
        End If
      Next

      ' Berechne die maximale Anzahl der Labels, die auf die x-Achse passen
      Dim maxLabels As Integer = CInt(Math.Floor(Me.drawRect.Width / maxLabelWidth))

      ' Bestimme den Abstand zwischen den Labels
      Dim labelCount As Integer = Math.Min(Me.dataset.Count, maxLabels)
      Dim dateStep As Single = Me.drawRect.Width / (labelCount - 1)

      For i As Integer = 0 To labelCount - 1
        Dim index As Integer = CInt(Math.Floor(i * (Me.dataset.Count - 1) / (labelCount - 1)))
        Dim datum As Date = Me.dataset(index).Item1
        Dim x As Single = Me.drawRect.Left + (i * dateStep)
        Dim y As Single = Me.drawRect.Bottom + 5 + Me.TextSize

        ' Zeichne das Label
        Me.canvas.DrawText(datum.ToString("dd.MM."), x - (maxLabelWidth / 2), y, paint)
      Next
    End Using
  End Sub

  Private Sub DrawYAxisLabels()
    Using paint As New SKPaint()
      paint.Color = Me.TextColor
      paint.TextSize = Me.TextSize
      paint.IsAntialias = True

      ' Bestimme den maximalen und minimalen Wert im Datensatz
      Dim minValue As Integer = Me.dataset.Min(Function(t) t.Item2)
      Dim maxValue As Integer = Me.dataset.Max(Function(t) t.Item2)

      ' Bestimme die Anzahl der Labels und den Abstand zwischen ihnen
      Dim labelCount As Integer = 10
      Dim valueStep As Single = CSng((maxValue - minValue) / (labelCount - 1))

      For i As Integer = 0 To labelCount - 1
        Dim value As Single = minValue + (i * valueStep)
        Dim y As Single = Me.drawRect.Bottom - ((value - minValue) / (maxValue - minValue) * Me.drawRect.Height)

        ' Zeichne das Label
        Me.canvas.DrawText(value.ToString(), Me.drawRect.Left - 10 - Me.CalculateMaxYLabelWidth, y, paint)
      Next
    End Using
  End Sub

  Private Sub DrawAxes()
    Using paint As New SKPaint()
      paint.Color = Me.AxisColor
      paint.StrokeWidth = 2
      Me.canvas = New SKCanvas(Me.bitmap)
      ' Zeichne die x-Achse
      Me.canvas.DrawLine(Me.drawRect.Left, Me.drawRect.Bottom, Me.drawRect.Right, Me.drawRect.Bottom, paint)
      ' Zeichne die y-Achse
      Me.canvas.DrawLine(Me.drawRect.Left, Me.drawRect.Bottom, Me.drawRect.Left, Me.drawRect.Top, paint)
    End Using
  End Sub

  Private Sub CreateDrawRect()
    ' Ränder der Zeichenfläche
    Me.drawRect.Left = Me.Padding + Me.CalculateMaxYLabelWidth()
    Me.drawRect.Top = Me.Padding
    Me.drawRect.Right = Me.Width - Me.Padding
    ' unteren Rand der Zeichenfläche anpassen in Abh�ngigkeit ob eine Beschriftung vorhanden ist oder nicht
    Me.drawRect.Bottom = If(String.IsNullOrEmpty(Me.Caption),
      Me.Height - Me.Padding - Me.CalculateTextHeight(),
      Me.Height - Me.Padding - (2 * Me.CalculateTextHeight()))
  End Sub

  Private Function CalculateMaxYLabelWidth() As Single
    Dim maxWidth As Single = 0

    Using paint As New SKPaint()
      paint.TextSize = Me.TextSize
      paint.IsAntialias = True

      ' Bestimme den maximalen und minimalen Wert im Datensatz
      Dim minValue As Integer = Me.dataset.Min(Function(t) t.Item2)
      Dim maxValue As Integer = Me.dataset.Max(Function(t) t.Item2)

      ' Bestimme die Anzahl der Labels und den Abstand zwischen ihnen
      Dim labelCount As Integer = 10
      Dim valueStep As Single = CSng((maxValue - minValue) / (labelCount - 1))

      For i As Integer = 0 To labelCount - 1
        Dim value As Single = minValue + (i * valueStep)
        Dim text As String = value.ToString()

        ' Berechne die Breite des Textes
        Dim textWidth As Single = paint.MeasureText(text)
        If textWidth > maxWidth Then
          maxWidth = textWidth
        End If
      Next
    End Using

    Return maxWidth
  End Function

  Private Sub CreateBitmap()
    Me.bitmap = New SKBitmap(Me.Width, Me.Height)
    Me.bitmap.Erase(Me.BackColor)
  End Sub

  Private Sub EncodeImageToBase64()
    Me.base64Image = ""
    Using image As SKImage = SKImage.FromBitmap(Me.bitmap)
      Using data As SKData = image.Encode(SKEncodedImageFormat.Png, 100)
        Me.base64Image = Convert.ToBase64String(data.ToArray())
      End Using
    End Using
  End Sub

  Private Sub ParseDataLines(dataLines() As String)
    Me.dataset.Clear()
    Try
      For i As Integer = 0 To dataLines.Length - 1
        Dim parts = dataLines(i).Split(";"c)
        If parts.Length < 2 Then Continue For

        Dim datum As Date
        Dim wert As Integer
        If Date.TryParseExact(parts(0).Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, datum) AndAlso
                   Integer.TryParse(parts(1).Trim(), wert) Then

          Me.dataset.Add(Tuple.Create(datum, wert))
          Dim unused = Me.uniqueDates.Add(datum)
        End If
      Next
    Catch ex As Exception
      Console.WriteLine("Fehler beim Verarbeiten der Daten: " & ex.Message)
    End Try
  End Sub

End Class

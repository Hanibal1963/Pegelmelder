' ****************************************************************************************************************
' ImageRenderer.vb
' (c) 2025 by Andreas Sauer
' ****************************************************************************************************************
'

Imports SkiaSharp
Imports System.Globalization
Imports System.Text

Public Class ImageRenderer

  Private ReadOnly dataset As New List(Of Tuple(Of Date, Integer))()
  Private ReadOnly uniqueDates As New SortedSet(Of Date)()
  Private dateList As New List(Of Date)

  Private bitmap As SKBitmap
  Private canvas As SKCanvas

  Private axisPaint As SKPaint
  Private gridPaint As SKPaint
  Private linePaint As SKPaint
  Private pointPaint As SKPaint
  Private textPaint As SKPaint

  Private base64Image As String
  Private legendHeight As Integer
  Private maxLabelWidth As Single
  Private labelOffset As Integer
  Private adjustedPadding As Integer
  Private x0 As Integer
  Private y0 As Integer
  Private yMin As Integer
  Private yMax As Integer
  Private xMin As Integer
  Private xMax As Integer
  Private yRange As Integer
  Private xScale As Single
  Private yScale As Single

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
    Me.InitializePaints() ' Stifte f�r Achsen, Gitternetzlinien, Linien und Punkte 
    Me.CalculateCaptionHeight() ' Beschriftungsh�he berechnen
    Me.AdjustPadding()  ' Gesamtes Padding anpassen
    Me.CalculateYRange() ' Wertebereiche f�r Y-Achse
    Me.SortUniqueDates() ' Sortiere Datumswerte f�r die X-Achse
    Me.CalculateMaxLabelWidth() ' Maximale Breite der Y-Achsen-Beschriftung berechnen
    Me.CalculateScales() ' Skalierung berechnen
    Me.InitializeCanvas() ' Bitmap erstellen
    Me.SetAxisCoordinates() ' Ursprungspunkt berechnen
    Me.DrawAxes() ' Achsen zeichnen
    Me.DrawXAxisLabels() ' X-Achse beschriften
    Me.DrawYAxisLabels() ' Y-Achse beschriften
    Me.DrawDataLines() ' Datenlinien zeichnen
    Me.DrawDataPoints() ' Datenpunkte zeichnen
    Me.DrawCaptionText() ' Beschriftungstext zeichnen
    Me.EncodeImageToBase64() ' Bild als PNG in Base64 kodieren
    Return $"<img src='data:image/png;base64,{Me.base64Image}' alt='{Me.AltText}'/>"  ' Das img-Tag zur�ckgeben
  End Function

  Private Sub EncodeImageToBase64()
    Me.base64Image = ""
    Using image As SKImage = SKImage.FromBitmap(Me.bitmap)
      Using data As SKData = image.Encode(SKEncodedImageFormat.Png, 100)
        Me.base64Image = Convert.ToBase64String(data.ToArray())
      End Using
    End Using
  End Sub

  Private Sub DrawCaptionText()
    If Not String.IsNullOrEmpty(Me.Caption) Then
      Me.canvas.DrawText(Me.Caption, Me.x0, Me.Height + Me.legendHeight - 10, Me.textPaint)
    End If
  End Sub

  Private Sub DrawDataLines()
    For i As Integer = 0 To Me.dataset.Count - 2
      Dim x1 As Single = Me.x0 + (Me.dateList.IndexOf(Me.dataset(i).Item1) * Me.xScale)
      Dim y1 As Single = Me.y0 - ((Me.dataset(i).Item2 - Me.yMin) * Me.yScale)
      Dim x2 As Single = Me.x0 + (Me.dateList.IndexOf(Me.dataset(i + 1).Item1) * Me.xScale)
      Dim y2 As Single = Me.y0 - ((Me.dataset(i + 1).Item2 - Me.yMin) * Me.yScale)
      Me.canvas.DrawLine(x1, y1, x2, y2, Me.linePaint)
    Next
  End Sub

  Private Sub DrawDataPoints()
    For i As Integer = 0 To Me.dataset.Count - 2
      Dim px As Single = Me.x0 + (Me.dateList.IndexOf(Me.dataset(i).Item1) * Me.xScale)
      Dim py As Single = Me.y0 - ((Me.dataset(i).Item2 - Me.yMin) * Me.yScale)
      Me.canvas.DrawCircle(px, py, 4, Me.pointPaint)
    Next
  End Sub

  Private Sub DrawYAxisLabels()
    For y As Integer = Me.yMin To Me.yMax Step Math.Max(1, Me.yRange \ 10)
      Dim py As Single = Me.y0 - ((y - Me.yMin) * Me.yScale)
      Me.canvas.DrawLine(Me.x0, py, Me.Width - Me.adjustedPadding, py, Me.gridPaint)
      Me.canvas.DrawText(y.ToString(), Me.x0 - Me.labelOffset, py + 5, Me.textPaint)
    Next
  End Sub

  Private Sub DrawXAxisLabels()
    For i As Integer = 0 To Me.dateList.Count - 1 Step 5
      Dim px As Single = Me.x0 + (i * Me.xScale)
      Me.canvas.DrawLine(px, Me.y0, px, Me.adjustedPadding, Me.gridPaint)
      Me.canvas.DrawText(Me.dateList(i).ToString("dd.MM"), px - 15, Me.y0 + 20, Me.textPaint)
    Next
  End Sub

  Private Sub DrawAxes()
    Me.canvas.DrawLine(Me.x0, Me.adjustedPadding, Me.x0, Me.y0, Me.axisPaint) ' Y-Achse
    Me.canvas.DrawLine(Me.x0, Me.y0, Me.Width - Me.adjustedPadding, Me.y0, Me.axisPaint) ' X-Achse
  End Sub

  Private Sub SetAxisCoordinates()
    Me.x0 = Me.adjustedPadding + Me.labelOffset
    Me.y0 = Me.Height - Me.adjustedPadding
  End Sub

  Private Sub InitializePaints()
    Me.axisPaint = New SKPaint With {.Color = Me.AxisColor, .StrokeWidth = 2, .Style = SKPaintStyle.Stroke}
    Me.gridPaint = New SKPaint With {.Color = Me.GridColor, .StrokeWidth = 1, .Style = SKPaintStyle.Stroke}
    Me.linePaint = New SKPaint With {.Color = Me.LineColor, .StrokeWidth = 2, .Style = SKPaintStyle.Stroke}
    Me.pointPaint = New SKPaint With {.Color = Me.PointColor, .StrokeWidth = 5, .Style = SKPaintStyle.Fill}
    Me.textPaint = New SKPaint With {.Color = Me.TextColor, .TextSize = Me.TextSize}
  End Sub

  Private Sub InitializeCanvas()
    Me.bitmap = New SKBitmap(Me.Width, Me.Height + Me.legendHeight + Me.Padding)
    Me.canvas = New SKCanvas(Me.bitmap)
    Me.canvas.Clear(Me.BackColor)
  End Sub

  Private Sub CalculateScales()
    Me.xScale = CSng((Me.Width - (2 * Me.Padding) - Me.labelOffset) / (Me.xMax - Me.xMin))
    Me.yScale = CSng((Me.Height - (2 * Me.Padding) - Me.legendHeight) / (Me.yMax - Me.yMin))
  End Sub

  Private Sub CalculateMaxLabelWidth()
    Me.maxLabelWidth = Me.dataset.Max(Function(d) Me.textPaint.MeasureText(d.Item2.ToString()))
    Me.labelOffset = CInt(Math.Ceiling(Me.maxLabelWidth)) + 10
  End Sub

  Private Sub SortUniqueDates()
    Me.dateList = Me.uniqueDates.ToList()
    Me.xMin = 0
    Me.xMax = Me.dateList.Count - 1
  End Sub

  Private Sub CalculateYRange()
    Me.yMin = Me.dataset.Min(Function(d) d.Item2)
    Me.yMax = Me.dataset.Max(Function(d) d.Item2)
    Me.yRange = Me.yMax - Me.yMin
    If Me.yRange = 0 Then Me.yRange = 10
    Me.yMin -= CInt(Me.yRange * 0.1)
    Me.yMax += CInt(Me.yRange * 0.1)
  End Sub

  Private Sub AdjustPadding()
    Me.adjustedPadding = Me.Padding
  End Sub

  Private Sub CalculateCaptionHeight()
    Me.legendHeight = If(String.IsNullOrEmpty(Me.Caption), 0, CInt(Me.textPaint.TextSize + 10))
  End Sub

  Private Sub ParseDataLines(dataLines() As String)
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

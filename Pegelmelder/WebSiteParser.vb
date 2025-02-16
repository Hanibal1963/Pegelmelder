' WebSiteParser.vb
' (c) 2022 - 2025 by Andreas Sauer
'
' Diese Klasse stellt Methoden zum Parsen von Websitedaten zur Verfügung.
' Sie implementiert das IDisposable-Interface, um Ressourcen freizugeben.
'

Imports System.Net.Http

Friend Class WebSiteParser

  Implements IDisposable

  Private Const BLEILOCH As String = "Pegel Talsperre Bleiloch"
  Private Const HOHENWARTE As String = "Pegel Talsperre Hohenwarte"

  Private DisposedValue As Boolean
  Private ReadOnly Docsource As String

  Public Sub New(Weburl As String)
    Me.Docsource = GetWebSource(Weburl).Result 'Quelltext der Website runterladen
  End Sub

  ''' <summary>
  ''' Liest die Daten der Talsperre Hohenwarte aus dem Quelltext der Website
  ''' </summary>
  Public Function GetHohenwarteData() As List(Of String)
    Dim result As New List(Of String)
    Dim data As String
    For Each datarow As String In GetDataSource(Me.Docsource, HOHENWARTE)
      'Datenzeile in Werte splitten und Datum als Langdatum und Pegel in mm übernehmen
      data = CreateData(datarow)
      result.Add(data)
    Next
    Return result
  End Function

  ''' <summary>
  ''' Liest die Daten der Talsperre Bleiloch aus dem Quelltext der Website
  ''' </summary>
  ''' <returns></returns>
  Public Function GetBleilochData() As List(Of String)
    Dim result As New List(Of String)
    Dim data As String
    For Each datarow As String In GetDataSource(Me.Docsource, BLEILOCH)
      'Datenzeile in Werte splitten und Datum als Langdatum und Pegel in mm übernehmen
      data = CreateData(datarow)
      result.Add(data)
    Next
    Return result
  End Function

  ''' <summary>
  ''' Lädt den Quelltext einer Website herunter
  ''' </summary>
  Private Shared Async Function GetWebSource(URL As String) As Task(Of String)
    Dim client As New HttpClient
    Dim responsebody As String
    Using response As HttpResponseMessage = Await client.GetAsync(URL)
      Dim unused = response.EnsureSuccessStatusCode()
      responsebody = Await response.Content.ReadAsStringAsync
    End Using
    Return responsebody
  End Function

  ''' <summary>
  ''' Liest die Daten aus dem Quelltext der Website
  ''' </summary>
  Private Shared Function GetDataSource(Source As String, Location As String) As List(Of String)
    'Fehlerprüfung
    If String.IsNullOrEmpty(Source) Then
      Throw New ArgumentException(String.Format(My.Resources.NullOrEmtyMessage, NameOf(Source)))
    End If
    'Text vor Anfangstag und nach Endetag (incl. der Tags) des Datenbereiches entfenen
    Dim datasource As String = Source.Split("<div class=""" & Location & """>").Last.Split("</div>").First
    'Tabellentags entfernen
    datasource = datasource.Replace(datasource.Split("<tr>").First, "").Split("</table>").First
    'Endetags der Zeilen und Spalten entfernen
    datasource = datasource.Replace("</tr>", "").Replace("</td>", "")
    'Datenzeilen trennen
    Dim result As New List(Of String)
    For Each datarow As String In datasource.Split("<tr>")
      'Zeilenumbrüche, Tabstopps, Leerzeichen entfernen und Tags durch Semikolon und Punkt durch Komma ersetzen
      datarow = datarow.Replace(vbCrLf, "").Replace(vbTab, "").Replace(" ", "").Replace("<td>", ";").Replace(".", ",")
      'erstes ; entfernen
      datarow = Mid(datarow, 2)
      'nur zeilen mit Daten übernehmen
      If datarow <> "" Then
        result.Add(datarow)
      End If
    Next
    Return result
  End Function

  ''' <summary>
  ''' Erstellt eine Datenzeile im Format "Datum;Pegel" aus einer Zeile des Quelltextes
  ''' </summary>
  Private Shared Function CreateData(datarow As String) As String
    Dim datum As Date = CDate(datarow.Split(";").First)
    Dim pegel As Integer = CInt(CDec(datarow.Split(";").Last) * 1000)
    Dim day As String = datum.Day.ToString("00")
    Dim month As String = datum.Month.ToString("00")
    Dim year As String = datum.Year.ToString("0000")
    Dim result As String = $"{day}.{month}.{year};{pegel}"
    Return result
  End Function

  ' ' TODO: Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
  ' Protected Overrides Sub Finalize()
  '     ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
  '     Dispose(disposing:=False)
  '     MyBase.Finalize()
  ' End Sub

  ''' <summary>
  ''' Gibt die verwendeten Ressourcen frei
  ''' </summary>
  Public Sub Dispose() Implements IDisposable.Dispose
    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
    Me.Dispose(disposing:=True)
    GC.SuppressFinalize(Me)
  End Sub

  Protected Overridable Sub Dispose(disposing As Boolean)
    If Not Me.DisposedValue Then
      If disposing Then
        ' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
      End If
      ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
      ' TODO: Große Felder auf NULL setzen
      Me.DisposedValue = True
    End If

  End Sub

End Class

'
'
'****************************************************************************************************************
'Parser.vb
'(c) 2022 - 2024 by Andreas Sauer
'
' Diese Datei enth�lt Hilfsfunktionen zum Herunterladen und Verarbeiten von Websitedaten.
'
'****************************************************************************************************************
'

Imports System.Net.Http

Class HelperFunctions

  ''' <summary>
  ''' L�dt den Quelltext einer Website herunter
  ''' </summary>
  Friend Shared Async Function GetWebSource(URL As String) As Task(Of String)
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
  Friend Shared Function GetDataSource(Source As String, Location As String) As List(Of String)
    'Fehlerpr�fung
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
      'Zeilenumbr�che, Tabstopps, Leerzeichen entfernen und Tags durch Semikolon und Punkt durch Komma ersetzen
      datarow = datarow.Replace(vbCrLf, "").Replace(vbTab, "").Replace(" ", "").Replace("<td>", ";").Replace(".", ",")
      'erstes ; entfernen
      datarow = Mid(datarow, 2)
      'nur zeilen mit Daten �bernehmen
      If datarow <> "" Then
        result.Add(datarow)
      End If
    Next
    Return result
  End Function

  ''' <summary>
  ''' Erstellt eine Datenzeile im Format "Datum;Pegel" aus einer Zeile des Quelltextes
  ''' </summary>
  Friend Shared Function CreateData(datarow As String) As String
    Dim datum As Date = CDate(datarow.Split(";").First)
    Dim pegel As Integer = CInt(CDec(datarow.Split(";").Last) * 1000)
    Dim day As String = datum.Day.ToString("00")
    Dim month As String = datum.Month.ToString("00")
    Dim year As String = datum.Year.ToString("0000")
    Dim result As String = $"{day}.{month}.{year};{pegel}"
    Return result
  End Function

End Class

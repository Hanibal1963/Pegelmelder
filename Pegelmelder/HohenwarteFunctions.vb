' ****************************************************************************************************************
' HohenwarteFunctions.vb
' � 2025 by Andreas Sauer
' 
' Dieses Modul enth�lt Funktionen zur Berechnung und Verwaltung der Pegelst�nde f�r Hohenwarte.
' Es umfasst Methoden zur Berechnung der Differenzen, zum Hinzuf�gen neuer Pegelst�nde und zum Abrufen
' und Formatieren der Pegelstandsdaten aus einer CSV-Datei.
'
' ****************************************************************************************************************
'

Imports SchlumpfSoft.CsvFileManager
Imports SchlumpfSoft.WebSiteParser
Imports SchlumpfSoft.ChartRenderer

Module HohenwarteFunctions

  ''' <summary>
  ''' Berechnet die Differenzen der Pegelst�nde f�r Hohenwarte und aktualisiert die CSV-Datei.
  ''' </summary>
  ''' <param name="File">Der Pfad zur CSV-Datei, die die Hohenwarte-Pegeldaten enth�lt.</param>
  Friend Sub CalculateHohenwarteDifferences(File As String)

    Dim tempdata As List(Of String)
    Dim oldpegel As Integer
    Dim newpegel As Integer
    Dim differenz As Integer

    'Differenzen f�r Hohenwarte berechnen
    Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)

      'Datens�tze zwischenspeichern
      tempdata = CsvFile.Data

      'Alle Datens�tze durchlaufen (ausser Header)
      For index As Integer = 1 To tempdata.Count - 1

        If index = 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then

          'Differenz f�r 1. Datensatz auf 0 setzen falls noch nicht erfolgt
          CsvFile.ReplaceValue(index, tempdata.ElementAt(index) & ";+000000")

        ElseIf tempdata.ElementAt(index).Split(";").Length = 2 Then

          'Differenz f�r alle anderen Datens�tze berechnen falls noch nicht erfolgt
          oldpegel = CInt(tempdata.ElementAt(index - 1).Split(";").ElementAt(1))
          newpegel = CInt(tempdata.ElementAt(index).Split(";").ElementAt(1))
          differenz = newpegel - oldpegel

          'Vorzeichen f�r die Differenz setzen und Datens�tze ersetzen
          If differenz < 0 Then
            'Differenz ist < 0
            CsvFile.ReplaceValue(index, tempdata.ElementAt(index) & ";" & Format(differenz, "000000"))
          Else
            'Differenz ist >= 0 oder 
            CsvFile.ReplaceValue(index, tempdata.ElementAt(index) & ";+" & Format(differenz, "000000"))
          End If

        End If

      Next

    End Using

  End Sub

  ''' <summary>
  ''' F�gt einen neuen Datensatz in die Hohenwartepegeldaten ein
  ''' falls noch nicht vorhanden.
  ''' </summary>
  ''' <param name="File">Datendatei f�r Hohenwartedaten</param>
  Friend Function AddHohenwartePegel(File As String) As Boolean

    Dim result As Boolean = False
    Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)

      'alle Datens�tze der Webdaten durchsuchen
      Using WebData As New Parser(URL)

        Dim index As Integer
        For Each row As String In Parser.GetHohenwarteData

          '�berpr�fen ob Datum bereits existiert und Datensatz eintragen falls nicht
          index = CsvFile.FindRecord(row.Split(";").First)
          If index = -1 Then
            CsvFile.AddRecord(row)
            result = True
          End If

        Next

      End Using

    End Using

    Return result

  End Function

  ''' <summary>
  ''' Ruft die Hohenwarte-Daten aus der angegebenen Datei ab und formatiert sie gem�� der angegebenen Vorlage.
  ''' </summary>
  ''' <param name="File">Der Pfad zur Datei, die die Hohenwarte-Daten enth�lt.</param>
  ''' <param name="Records">Die Anzahl der Datens�tze, die abgerufen werden sollen.</param>
  ''' <param name="Linetemplate">Die Vorlage, die zum Formatieren der Datens�tze verwendet wird.</param>
  ''' <returns>Eine formatierte Zeichenfolge, die die abgerufenen Hohenwarte-Daten enth�lt.</returns>
  Friend Function GetHohenwarteData(File As String, Records As Integer, Linetemplate As String) As String

    Dim result As String = ""
    Dim record As String '= Linetemplate
    Dim data As New CsvFile(File, PEGELDATAHEADER)
    Dim length As Integer = CsvFile.Data.Count - 1
    Dim datum As String
    Dim pegel As String
    Dim diff As String

    'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
    If Records > length Then
      Records = length
    End If

    'Die letzten in Records gespeicherten Datens�tze durchlaufen
    For index As Integer = length + 1 - Records To length
      record = Linetemplate
      datum = CsvFile.Data.ElementAt(index).Split(";").ElementAt(0)
      pegel = CsvFile.Data.ElementAt(index).Split(";").ElementAt(1)
      diff = CsvFile.Data.ElementAt(index).Split(";").ElementAt(2)
      record = record.Replace("%DATUM%", datum)
      record = record.Replace("%PEGEL%", pegel)
      record = record.Replace("%DIFFERENZ%", diff)
      result &= record
    Next

    Return result

  End Function

  Friend Function GetHohenwarteImageCode(File As String, Records As Integer) As String

    Dim imagedata As New List(Of String)
    Dim data As New CsvFile(File, PEGELDATAHEADER)
    Dim length As Integer = CsvFile.Data.Count - 1
    Dim datum As String
    Dim pegel As String

    'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
    If Records > length Then
      Records = length
    End If

    'Die letzten in Records gespeicherten Datens�tze durchlaufen
    For index As Integer = length + 1 - Records To length
      datum = CsvFile.Data.ElementAt(index).Split(";").ElementAt(0)
      pegel = CsvFile.Data.ElementAt(index).Split(";").ElementAt(1)
      imagedata.Add($"{datum};{pegel}")
    Next

    Dim datalines As String() = imagedata.ToArray
    Dim renderer As New ImageRenderer(datalines) With
      {.Height = 200, .Width = 300, .Padding = 20, .Caption = $"Talsperre Hohenwarte", .TextSize = 10}
    Dim imagecode = renderer.GenerateImageTag

    Return imagecode

  End Function

End Module

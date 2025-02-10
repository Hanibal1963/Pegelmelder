' ****************************************************************************************************************
' HohenwarteFunctions.vb
' © 2025 by Andreas Sauer
' 
' Dieses Modul enthält Funktionen zur Berechnung und Verwaltung der Pegelstände für Hohenwarte.
' Es umfasst Methoden zur Berechnung der Differenzen, zum Hinzufügen neuer Pegelstände und zum Abrufen
' und Formatieren der Pegelstandsdaten aus einer CSV-Datei.
'
' ****************************************************************************************************************
'

Imports SchlumpfSoft.CsvFileManager
Imports SchlumpfSoft.WebSiteParser

Module HohenwarteFunctions

  ''' <summary>
  ''' Berechnet die Differenzen der Pegelstände für Hohenwarte und aktualisiert die CSV-Datei.
  ''' </summary>
  ''' <param name="File">Der Pfad zur CSV-Datei, die die Hohenwarte-Pegeldaten enthält.</param>
  Friend Sub CalculateHohenwarteDifferences(File As String)

    Dim tempdata As List(Of String)
    Dim oldpegel As Integer
    Dim newpegel As Integer
    Dim differenz As Integer

    'Differenzen für Hohenwarte berechnen
    Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)

      'Datensätze zwischenspeichern
      tempdata = CsvFile.Data

      'Alle Datensätze durchlaufen (ausser Header)
      For index As Integer = 1 To tempdata.Count - 1

        If index = 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then

          'Differenz für 1. Datensatz auf 0 setzen falls noch nicht erfolgt
          CsvFile.ReplaceValue(index, tempdata.ElementAt(index) & ";+000000")

        ElseIf tempdata.ElementAt(index).Split(";").Length = 2 Then

          'Differenz für alle anderen Datensätze berechnen falls noch nicht erfolgt
          oldpegel = CInt(tempdata.ElementAt(index - 1).Split(";").ElementAt(1))
          newpegel = CInt(tempdata.ElementAt(index).Split(";").ElementAt(1))
          differenz = newpegel - oldpegel

          'Vorzeichen für die Differenz setzen und Datensätze ersetzen
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
  ''' Fügt einen neuen Datensatz in die Hohenwartepegeldaten ein
  ''' falls noch nicht vorhanden.
  ''' </summary>
  ''' <param name="File">Datendatei für Hohenwartedaten</param>
  Friend Function AddHohenwartePegel(File As String) As Boolean

    Dim result As Boolean = False
    Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)

      'alle Datensätze der Webdaten durchsuchen
      Using WebData As New Parser(URL)

        Dim index As Integer
        For Each row As String In Parser.GetHohenwarteData

          'überprüfen ob Datum bereits existiert und Datensatz eintragen falls nicht
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
  ''' Ruft die Hohenwarte-Daten aus der angegebenen Datei ab und formatiert sie gemäß der angegebenen Vorlage.
  ''' </summary>
  ''' <param name="File">Der Pfad zur Datei, die die Hohenwarte-Daten enthält.</param>
  ''' <param name="Records">Die Anzahl der Datensätze, die abgerufen werden sollen.</param>
  ''' <param name="Linetemplate">Die Vorlage, die zum Formatieren der Datensätze verwendet wird.</param>
  ''' <returns>Eine formatierte Zeichenfolge, die die abgerufenen Hohenwarte-Daten enthält.</returns>
  Friend Function GetHohenwarteData(File As String, Records As Integer, Linetemplate As String) As String

    Dim result As String = ""
    Dim record As String '= Linetemplate
    Dim data As New CsvFile(File, PEGELDATAHEADER)
    Dim length As Integer = CsvFile.Data.Count - 1

    'maximale Anzahl der Datensätze anpassen wenn weniger Daten vorhanden als gewünscht
    If Records > length Then
      Records = length
    End If

    'Die letzten in Records gespeicherten Datensätze durchlaufen
    For index As Integer = length + 1 - Records To length
      record = Linetemplate
      record = record.Replace("%DATUM%", CsvFile.Data.ElementAt(index).Split(";").ElementAt(0))
      record = record.Replace("%PEGEL%", CsvFile.Data.ElementAt(index).Split(";").ElementAt(1))
      record = record.Replace("%DIFFERENZ%", CsvFile.Data.ElementAt(index).Split(";").ElementAt(2))
      result &= record
    Next

    Return result

  End Function

  Friend Function GetHohenwarteImageCode(File As String, Records As Integer) As String
    Dim result As String = "Hohenwartediagramm"
    Return result
  End Function

End Module

' ****************************************************************************************************************
' HelperFuctions.vb
' © 2025 by Andreas Sauer
'
' Dieses Modul stellt Hilfsfunktionen für das Verwalten von CSV-Dateien bereit.
' Es enthält Methoden zum Erstellen, Lesen und Schreiben von CSV-Dateien.
'
' Methoden:
' - CreateFile: Erstellt eine neue Datei mit Kopfzeilen und speichert sie.
' - ReadFile: Liest den Inhalt der Datei und erstellt eine Datenliste.
' - WriteFile: Schreibt den Inhalt der Datenliste in die Datei.
'
' ****************************************************************************************************************

''' <summary>
''' Modul, das Hilfsfunktionen für das Verwalten von CSV-Dateien bereitstellt.
''' </summary>
Module HelperFuctions

  ''' <summary>
  ''' Erstellt eine neue Datei mit Kopfzeilen und speichert sie.
  ''' </summary>
  Friend Sub CreateFile()
    'Kopfzeilen erstellen und Datei speichern
    DataList.Add(FileHeader)
    WriteFile()
  End Sub

  ''' <summary>
  ''' Liest den Inhalt der Datei und erstellt eine Datenliste.
  ''' </summary>
  Friend Sub ReadFile()
    'Datei lesen
    Contents = IO.File.ReadAllLines(FileName)
    'Datenliste erstellen
    DataList = Contents.ToList
  End Sub

  ''' <summary>
  ''' Schreibt den Inhalt der Datenliste in die Datei.
  ''' </summary>
  Friend Sub WriteFile()
    'Dateiinhalt generieren
    Contents = DataList.ToArray()
    'Datei schreiben
    IO.File.WriteAllLines(FileName, Contents)
  End Sub

End Module

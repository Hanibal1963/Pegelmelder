'****************************************************************************************************************
'CsvFile.vb
'(c) 2022 - 2024 by Andreas Sauer
'
' Diese Datei enthält die Definition der Klasse CsvFile, die zum Verwalten von CSV-Dateien verwendet wird.
' Die Klasse bietet Methoden zum Lesen, Schreiben, Suchen, Hinzufügen, Ersetzen und Löschen von Datensätzen
' in einer CSV-Datei. Außerdem implementiert sie das IDisposable-Interface zur ordnungsgemäßen Freigabe von Ressourcen.
'
' Enthaltene Methoden und Eigenschaften:
' - Data: Gibt die Datensätze als Liste zurück.
' - New(File As String, Header As String): Konstruktor zum Erstellen einer neuen Instanz der Klasse.
' - FindRecord(SearchData As String): Sucht einen Begriff in der Datenbank und gibt den Index zurück.
' - GetRecord(Index As Integer): Gibt den Datensatz mit dem angegebenen Index zurück.
' - AddRecord(DataRecord As String): Fügt einen neuen Datensatz an das Ende der Datenbank an.
' - ReplaceValue(Index As Integer, NewValue As String): Ersetzt einen Datensatz durch einen neuen Wert.
' - DeleteRecord(Index As Integer): Löscht einen Datensatz mit dem angegebenen Index.
' - Dispose(): Führt anwendungsspezifische Aufgaben zur Freigabe von Ressourcen durch.
' - Dispose(disposing As Boolean): Bereinigt die Ressourcen, die von der Klasse verwendet werden.
'
'****************************************************************************************************************
'

''' <summary>
''' Klasse zum verwalten von CSV-Dateien
''' </summary>
Public Class CsvFile

	Implements IDisposable

	''' <summary>
	''' Gibt die Datensätze als Liste zurück.
	''' </summary>
	''' <returns>Liste der Datensätze</returns>
	Public Shared ReadOnly Property Data As List(Of String)
		Get
			Return DataList
		End Get
	End Property

	''' <summary>
	''' Erstellt eine neue Instanz der Klasse.
	''' </summary>
	''' <param name="File">Name und Pfad der Datendatei.</param>
	''' <param name="Header">Headerdaten</param>
	Public Sub New(File As String, Header As String)
		FileName = File
		FileHeader = Header
		'Datei erstellen falls sie nicht existiert
		If Not IO.File.Exists(File) Then
			CreateFile()
		End If
		'Datei einlesen
		ReadFile()
	End Sub

	''' <summary>
	''' Sucht den Begriff <paramref name="SearchData"/> in der gesamten Datenbank.
	''' </summary>
	''' <param name="SearchData">Der in der Datenbank zu suchende Begriff.</param>
	''' <returns>Gibt den Index des gefundenen Datensatzes zurück.</returns>
	''' <remarks>Falls der Begriff <paramref name="SearchData"/> nicht gefunden wird gibt die Funktion -1 zurück.</remarks>
	Public Shared Function FindRecord(SearchData As String) As Integer
		Dim result As Integer = -1
		'Alle Datensätze nach den Suchdaten durchsuchen
		For i As Integer = 0 To DataList.Count - 1
			'Wenn gefunden -> Index merken
			If InStr(DataList.ElementAt(i), SearchData) <= 0 Then
				Continue For
			End If
			result = i
			Exit For
		Next
		'Index zurück oder -1 für nicht gefunden
		Return result
	End Function

	''' <summary>
	''' Gibt den Datensatz mit dem Index <paramref name="Index"/> zurück.
	''' </summary>
	''' <param name="Index">Index des Datensatzes</param>
	''' <returns>Der Datensatz</returns>
	Public Shared Function GetRecord(Index As Integer) As String
		Return DataList.ElementAt(Index)
	End Function

	''' <summary>
	''' Fügt einen neuen Datensatz <paramref name="DataRecord"/> an das Ende der Datenbank an.
	''' </summary>
	''' <param name="DataRecord">Datensatz der angefügt werden soll.</param>
	Public Shared Sub AddRecord(DataRecord As String)
		DataList.Add(DataRecord)
		WriteFile()
	End Sub

	''' <summary>
	''' Ersetzt den Datensatz mit dem Index <paramref name="Index"/> durch den neuen Datensatz <paramref name="NewValue"/>.
	''' </summary>
	''' <param name="Index">Index des zu ersetzenden Datensatzes.</param>
	''' <param name="NewValue">Der neue Datensatz.</param>
	Public Shared Sub ReplaceValue(Index As Integer, NewValue As String)
		DataList.Insert(Index + 1, NewValue)
		DataList.RemoveAt(Index)
		WriteFile()
	End Sub

	''' <summary>
	''' Löscht den Datensatz mit dem Index <paramref name="Index"/>.
	''' </summary>
	''' <param name="Index">Index des zu löschenden Datensatzes.</param>
	Public Shared Sub DeleteRecord(Index As Integer)
		'Element mit Index löschen
		DataList.RemoveAt(Index)
		'Datei schreiben
		WriteFile()
	End Sub

	' ' TODO: Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
	' Protected Overrides Sub Finalize()
	'     ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
	'     Dispose(disposing:=False)
	'     MyBase.Finalize()
	' End Sub

	''' <summary>
	''' Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, 
	''' der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
	''' </summary>
	Public Sub Dispose() Implements IDisposable.Dispose
		' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
		Me.Dispose(disposing:=True)
		GC.SuppressFinalize(Me)
	End Sub

	''' <summary>
	''' Bereinigt die Ressourcen, die von der Klasse verwendet werden.
	''' </summary>
	''' <param name="disposing">Gibt an, ob verwaltete Ressourcen freigegeben werden sollen.</param>
	Protected Overridable Sub Dispose(disposing As Boolean)
		If Not DisposedValue Then
			If disposing Then
				' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
				DataList = Nothing
				Contents = Nothing
			End If
			' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
			' TODO: Große Felder auf NULL setzen
			DisposedValue = True
		End If
	End Sub

End Class

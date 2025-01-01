'****************************************************************************************************************
'CsvFile.vb
'(c) 2022 - 2024 by Andreas Sauer
'****************************************************************************************************************
'

''' <summary>
''' Klasse zum verwalten von CSV-Dateien
''' </summary>
Public Class CsvFile

	Implements IDisposable

	Private DisposedValue As Boolean
	Private Contents As String()
	Private DataList As New List(Of String)
	Private ReadOnly File As String
	Private ReadOnly Header As String

	''' <summary>
	''' Gibt die Datensätze als Liste zurück.
	''' </summary>
	''' <returns></returns>
	Public ReadOnly Property Data As List(Of String)
		Get
			Return Me.DataList
		End Get
	End Property

	''' <summary>
	''' Erstellt eine neue Instanz der Klasse.
	''' </summary>
	''' <param name="File">
	''' Name und Pfad der Datendatei.
	''' </param>
	''' <param name="Header">
	''' Headerdaten
	''' </param>
	Public Sub New(File As String, Header As String)
		Me.File = File
		Me.Header = Header
		'Datei erstellen falls sie nicht existiert
		If Not IO.File.Exists(File) Then
			Me.CreateFile()
		End If
		'Datei einlesen
		Me.ReadFile()
	End Sub

	''' <summary>
	''' Sucht den Begriff <paramref name="SearchData"/> in der gesamten Datenbank.
	''' </summary>
	''' <param name="SearchData">
	''' Der in der Datenbank zu suchende Begriff.
	''' </param>
	''' <returns>
	''' Gibt den Index des gefundenen Datensatzes zurück.
	''' </returns>
	''' <remarks>
	''' Falls der Begriff <paramref name="SearchData"/> nicht gefunden wird gibt die Funktion -1 zurück.
	''' </remarks>
	Public Function FindRecord(SearchData As String) As Integer
		Dim result As Integer = -1
		'Alle Datensätze nach den Suchdaten durchsuchen
		For i As Integer = 0 To Me.DataList.Count - 1
			'Wenn gefunden -> Index merken
			If InStr(Me.DataList.ElementAt(i), SearchData) <= 0 Then
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
	''' <param name="Index">
	''' Index des Datensatzes
	''' </param>
	Public Function GetRecord(Index As Integer) As String
		Return Me.DataList.ElementAt(Index)
	End Function

	''' <summary>
	''' Fügt einen neuen Datensatz <paramref name="DataRecord"/> an das Ende der Datenbank an.
	''' </summary>
	''' <param name="DataRecord">
	''' Datensatz der angefügt werden soll.
	''' </param>
	Public Sub AddRecord(DataRecord As String)
		Me.DataList.Add(DataRecord)
		Me.WriteFile()
	End Sub

	''' <summary>
	''' Ersetzt den Datensatz mit dem Index <paramref name="Index"/> durch den neuen Datensatz <paramref name="NewValue"/>.
	''' </summary>
	''' <param name="Index">
	''' Index des zu ersetzenden Datensatzes.
	''' </param>
	''' <param name="NewValue">
	''' Der neue Datensatz.
	''' </param>
	Public Sub ReplaceValue(Index As Integer, NewValue As String)
		Me.DataList.Insert(Index + 1, NewValue)
		Me.DataList.RemoveAt(Index)
		Me.WriteFile()
	End Sub

	''' <summary>
	''' Löscht den Datensatz mit dem Index <paramref name="Index"/>.
	''' </summary>
	''' <param name="Index">
	''' Index des zu löschenden Datensatzes.
	''' </param>
	Public Sub DeleteRecord(Index As Integer)
		'Element mit Index löschen
		Me.DataList.RemoveAt(Index)
		'Datei schreiben
		Me.WriteFile()
	End Sub

	Private Sub CreateFile()
		'Kopfzeilen erstellen und Datei speichern
		Me.DataList.Add(Me.Header)
		Me.WriteFile()
	End Sub

	Private Sub ReadFile()
		'Datei lesen
		Me.Contents = IO.File.ReadAllLines(Me.File)
		'Datenliste erstellen
		Me.DataList = Me.Contents.ToList
	End Sub

	Private Sub WriteFile()
		'Dateiinhalt generieren
		Me.Contents = Me.DataList.ToArray()
		'Datei schreiben
		IO.File.WriteAllLines(Me.File, Me.Contents)
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

	Protected Overridable Sub Dispose(disposing As Boolean)
		If Not Me.DisposedValue Then
			If disposing Then
				' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
				Me.DataList = Nothing
				Me.Contents = Nothing
			End If
			' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
			' TODO: Große Felder auf NULL setzen
			Me.DisposedValue = True
		End If
	End Sub

End Class

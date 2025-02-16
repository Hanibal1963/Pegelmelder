' Copyright (c)2022-2025 by Andreas Sauer 

Namespace Pegelmelder

	''' <summary>
	''' Klasse zum verwalten von CSV-Dateien
	''' </summary>
	Friend Class CsvFile

		Implements IDisposable

		' Die Klasse bietet Methoden zum Lesen, Schreiben, Suchen, Hinzuf�gen, Ersetzen und L�schen von Datens�tzen
		' in einer CSV-Datei. Au�erdem implementiert sie das IDisposable-Interface zur ordnungsgem��en Freigabe von Ressourcen.
		'
		' Enthaltene Methoden und Eigenschaften:
		' - Data: Gibt die Datens�tze als Liste zur�ck.
		' - New(File As String, Header As String): Konstruktor zum Erstellen einer neuen Instanz der Klasse.
		' - FindRecord(SearchData As String): Sucht einen Begriff in der Datenbank und gibt den Index zur�ck.
		' - GetRecord(Index As Integer): Gibt den Datensatz mit dem angegebenen Index zur�ck.
		' - AddRecord(DataRecord As String): F�gt einen neuen Datensatz an das Ende der Datenbank an.
		' - ReplaceValue(Index As Integer, NewValue As String): Ersetzt einen Datensatz durch einen neuen Wert.
		' - DeleteRecord(Index As Integer): L�scht einen Datensatz mit dem angegebenen Index.
		' - Dispose(): F�hrt anwendungsspezifische Aufgaben zur Freigabe von Ressourcen durch.
		' - Dispose(disposing As Boolean): Bereinigt die Ressourcen, die von der Klasse verwendet werden.


		''' <summary>
		''' Gibt an, ob das Objekt verworfen wurde.
		''' </summary>
		Private DisposedValue As Boolean

		''' <summary>
		''' Ein Array von Zeichenfolgen, das den Inhalt der Datei speichert.
		''' </summary>
		Private Contents As String()

		''' <summary>
		''' Eine Liste von Zeichenfolgen, die die Daten speichert.
		''' </summary>
		Private DataList As New List(Of String)

		''' <summary>
		''' Der Name der Datei.
		''' </summary>
		Private ReadOnly FileName As String

		''' <summary>
		''' Der Header der Datei.
		''' </summary>
		Private ReadOnly FileHeader As String


		''' <summary>
		''' Gibt die Datens�tze als Liste zur�ck.
		''' </summary>
		''' <returns>Liste der Datens�tze</returns>
		Public ReadOnly Property Data As List(Of String)
			Get
				Return Me.DataList
			End Get
		End Property

		''' <summary>
		''' Erstellt eine neue Instanz der Klasse.
		''' </summary>
		''' <param name="File">Name und Pfad der Datendatei.</param>
		''' <param name="Header">Headerdaten</param>
		Public Sub New(File As String, Header As String)
			Me.FileName = File
			Me.FileHeader = Header
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
		''' <param name="SearchData">Der in der Datenbank zu suchende Begriff.</param>
		''' <returns>Gibt den Index des gefundenen Datensatzes zur�ck.</returns>
		''' <remarks>Falls der Begriff <paramref name="SearchData"/> nicht gefunden wird gibt die Funktion -1 zur�ck.</remarks>
		Public Function FindRecord(SearchData As String) As Integer
			Dim result As Integer = -1
			'Alle Datens�tze nach den Suchdaten durchsuchen
			For i As Integer = 0 To Me.DataList.Count - 1
				'Wenn gefunden -> Index merken
				If InStr(Me.DataList.ElementAt(i), SearchData) <= 0 Then
					Continue For
				End If
				result = i
				Exit For
			Next
			'Index zur�ck oder -1 f�r nicht gefunden
			Return result
		End Function

		''' <summary>
		''' Gibt den Datensatz mit dem Index <paramref name="Index"/> zur�ck.
		''' </summary>
		''' <param name="Index">Index des Datensatzes</param>
		''' <returns>Der Datensatz</returns>
		Public Function GetRecord(Index As Integer) As String
			Return Me.DataList.ElementAt(Index)
		End Function

		''' <summary>
		''' F�gt einen neuen Datensatz <paramref name="DataRecord"/> an das Ende der Datenbank an.
		''' </summary>
		''' <param name="DataRecord">Datensatz der angef�gt werden soll.</param>
		Public Sub AddRecord(DataRecord As String)
			Me.DataList.Add(DataRecord)
			Me.WriteFile()
		End Sub

		''' <summary>
		''' Ersetzt den Datensatz mit dem Index <paramref name="Index"/> durch den neuen Datensatz <paramref name="NewValue"/>.
		''' </summary>
		''' <param name="Index">Index des zu ersetzenden Datensatzes.</param>
		''' <param name="NewValue">Der neue Datensatz.</param>
		Public Sub ReplaceValue(Index As Integer, NewValue As String)
			Me.DataList.Insert(Index + 1, NewValue)
			Me.DataList.RemoveAt(Index)
			Me.WriteFile()
		End Sub

		''' <summary>
		''' L�scht den Datensatz mit dem Index <paramref name="Index"/>.
		''' </summary>
		''' <param name="Index">Index des zu l�schenden Datensatzes.</param>
		Public Sub DeleteRecord(Index As Integer)
			'Element mit Index l�schen
			Me.DataList.RemoveAt(Index)
			'Datei schreiben
			Me.WriteFile()
		End Sub

		''' <summary>
		''' Erstellt eine neue Datei mit Kopfzeilen und speichert sie.
		''' </summary>
		Private Sub CreateFile()
			'Kopfzeilen erstellen und Datei speichern
			Me.DataList.Add(Me.FileHeader)
			Me.WriteFile()
		End Sub

		''' <summary>
		''' Liest den Inhalt der Datei und erstellt eine Datenliste.
		''' </summary>
		Private Sub ReadFile()
			'Datei lesen
			Me.Contents = IO.File.ReadAllLines(Me.FileName)
			'Datenliste erstellen
			Me.DataList = Me.Contents.ToList
		End Sub

		''' <summary>
		''' Schreibt den Inhalt der Datenliste in die Datei.
		''' </summary>
		Private Sub WriteFile()
			'Dateiinhalt generieren
			Me.Contents = Me.DataList.ToArray()
			'Datei schreiben
			IO.File.WriteAllLines(Me.FileName, Me.Contents)
		End Sub

		' ' TODO: Finalizer nur �berschreiben, wenn "Dispose(disposing As Boolean)" Code f�r die Freigabe nicht verwalteter Ressourcen enth�lt
		' Protected Overrides Sub Finalize()
		'     ' �ndern Sie diesen Code nicht. F�gen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
		'     Dispose(disposing:=False)
		'     MyBase.Finalize()
		' End Sub

		''' <summary>
		''' F�hrt anwendungsspezifische Aufgaben durch, die mit der Freigabe, 
		''' der Zur�ckgabe oder dem Zur�cksetzen von nicht verwalteten Ressourcen zusammenh�ngen.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			' �ndern Sie diesen Code nicht. F�gen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
			Me.Dispose(disposing:=True)
			GC.SuppressFinalize(Me)
		End Sub

		''' <summary>
		''' Bereinigt die Ressourcen, die von der Klasse verwendet werden.
		''' </summary>
		''' <param name="disposing">Gibt an, ob verwaltete Ressourcen freigegeben werden sollen.</param>
		Protected Overridable Sub Dispose(disposing As Boolean)
			If Not Me.DisposedValue Then
				If disposing Then
					' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
					Me.DataList = Nothing
					Me.Contents = Nothing
				End If
				' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer �berschreiben
				' TODO: Gro�e Felder auf NULL setzen
				Me.DisposedValue = True
			End If
		End Sub

	End Class

End Namespace

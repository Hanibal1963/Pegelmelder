'****************************************************************************************************************
'Document.vb
'(c) 2022 - 2025 by Andreas Sauer
'
' Diese Klasse stellt Methoden und Eigenschaften zur Verfügung, um ein Dokument mit Platzhaltern zu erstellen und 
' diese Platzhalter durch tatsächliche Daten zu ersetzen. Die Platzhalter werden durch Vorlagen aus den Ressourcen 
' ersetzt, die in der Anwendung eingebettet sind.
'
'****************************************************************************************************************
'

Imports System.Text

Public Class Document

	''' <summary>
	''' Erzeugt ein neues Dokument mit Platzhaltern
	''' </summary>
	Public Sub New()
		Dim docbuilder As New StringBuilder(My.Resources.EmailTemplate)
		DocText = docbuilder.ToString
		Dim linebuilder As New StringBuilder(My.Resources.DatenZeilenTemplate)
		LineTemplate = linebuilder.ToString
	End Sub

	''' <summary>
	''' Gibt den erzeugten Dokumenttext zurück
	''' </summary>
	Public Shared ReadOnly Property DocumentText As String
		Get
			Return DocText
		End Get
	End Property

	''' <summary>
	''' Gibt das Datenzeilentemplate zurück
	''' </summary>
	''' <returns></returns>
	Public Shared ReadOnly Property DataLineTemplate As String
		Get
			Return LineTemplate
		End Get
	End Property

	''' <summary>
	''' Ersetzt den Platzhalter für die Anrede
	''' </summary>
	Public Shared Sub SetName(FirstName As String)
		DocText = DocText.Replace(FIRST_NAME, FirstName)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Datentabelle 
	''' </summary>
	Public Sub SetDataTable()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.DatenTableTemplate)
		DocText = DocText.Replace(DATA_TABLE, builder.ToString)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Leerzelle 
	''' </summary>
	Public Sub SetBlankCell()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.LeerzellenTemplate)
		DocText = DocText.Replace(BLANK_CELL, builder.ToString)
	End Sub

	''' <summary>
	''' Fügt die Datentabelle für Hohenwartedaten ein
	''' </summary>
	Public Sub SetHohenwarteData()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.HohenwarteDatenTabellenTemplate)
		DocText = DocText.Replace(HOHENWARTE_DATA, builder.ToString)
	End Sub

	''' <summary>
	''' Fügt die Diagrammgrafik für Hohenwarte ein
	''' </summary>
	Public Shared Sub InsertHohenwarteImage(imagecode As String)
		DocText = DocText.Replace(HOHENWARTE_IMAGE, imagecode)
	End Sub

	''' <summary>
	''' Fügt die Datentabelle für Bleilochdaten ein
	''' </summary>
	Public Sub SetBleilochData()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.BleilochDatenTabellenTemplate)
		DocText = DocText.Replace(BLEILOCH_DATA, builder.ToString)
	End Sub

	''' <summary>
	''' Fügt die Diagrammgrafik für Bleiloch ein
	''' </summary>
	Public Shared Sub InsertBleilochImage(imagecode As String)
		DocText = DocText.Replace(BLEILOCH_IMAGE, imagecode)
	End Sub

	''' <summary>
	''' Trägt die Daten in das Dokument ein
	''' </summary>
	Public Shared Sub FillData(Data As String)
		DocText = DocText.Replace(DATA_LINES, Data)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalte für den Applicationsname
	''' </summary>
	Public Shared Sub SetAppName(Name As String)
		DocText = DocText.Replace(APP_NAME, Name)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Versionsnummer der Application
	''' </summary>
	Public Shared Sub SetAppVersion(Version As String)
		DocText = DocText.Replace(APP_VERSION, Version)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für Copyrightifo in der Fusszeile
	''' </summary>
	Public Shared Sub SetAppCopy(AppCopyright As String)
		DocText = DocText.Replace(APP_COPY, AppCopyright)
	End Sub

	''' <summary>
	''' Entfernt den angegebenen Platzhalter
	''' </summary>
	Public Shared Sub RemovePlaceHolder(PlaceHolder As String)
		DocText = DocText.Replace(PlaceHolder, "")
	End Sub


End Class

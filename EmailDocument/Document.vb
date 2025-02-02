'****************************************************************************************************************
'EmailDocument.vb
'(c) 2022 - 2024 by Andreas Sauer
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

		DocText = DocText.Replace("%VORNAME%", FirstName)

	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Datentabelle 
	''' </summary>
	Public Sub SetDataTable()

		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.DatenTableTemplate)
		DocText = DocText.Replace("%DATENTABELLE%", builder.ToString)

	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Leerzelle 
	''' </summary>
	Public Sub SetBlankCell()

		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.LeerzellenTemplate)
		DocText = DocText.Replace("%LEERZELLE%", builder.ToString)

	End Sub

	''' <summary>
	''' Fügt die Datentabelle für Hohenwartedaten ein
	''' </summary>
	Public Sub SetHohenwarteData()

		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.HohenwarteDatenTabellenTemplate)
		DocText = DocText.Replace("%HOHENWARTEDATEN%", builder.ToString)

	End Sub

	''' <summary>
	''' Fügt die Datentabelle für Bleilochdaten ein
	''' </summary>
	Public Sub SetBleilochData()

		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.BleilochDatenTabellenTemplate)
		DocText = DocText.Replace("%BLEILOCHDATEN%", builder.ToString)

	End Sub

	''' <summary>
	''' Trägt die Daten in das Dokument ein
	''' </summary>
	Public Shared Sub FillData(Data As String)

		DocText = DocText.Replace("%DATENZEILEN%", Data)

	End Sub

	''' <summary>
	''' Ersetzt den Platzhalte für den Applicationsname
	''' </summary>
	Public Shared Sub SetAppName(Name As String)

		DocText = DocText.Replace("%APPLICATION%", Name)

	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Versionsnummer der Application
	''' </summary>
	Public Shared Sub SetAppVersion(Version As String)

		DocText = DocText.Replace("%VERSION%", Version)

	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für Copyrightifo in der Fusszeile
	''' </summary>
	Public Shared Sub SetAppCopy(AppCopyright As String)

		DocText = DocText.Replace("%COPYRIGHT%", AppCopyright)

	End Sub

	''' <summary>
	''' Entfernt den angegebenen Platzhalter
	''' </summary>
	Public Shared Sub RemovePlaceHolder(PlaceHolder As String)

		DocText = DocText.Replace(PlaceHolder, "")

	End Sub

	'''' <summary>
	'''' Setz den Verwaltungslink ein
	'''' </summary>
	'''' <param name="ManageLink">
	'''' </param>
	'Private Sub SetManageLink(ManageLink As String)
	'	Me._documenttext = Me._documenttext.Replace("%MANGELINK%", ManageLink)
	'End Sub

End Class

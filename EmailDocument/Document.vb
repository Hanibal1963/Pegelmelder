'****************************************************************************************************************
'EmailDocument.vb
'(c) 2022 - 2024 by Andreas Sauer
'****************************************************************************************************************
'

Imports System.Text

Public Class Document

	Private DocText As String
	Private ReadOnly LineTemplate As String

	''' <summary>
	''' Erzeugt ein neues Dokument mit Platzhaltern
	''' </summary>
	Public Sub New()
		Dim docbuilder As New StringBuilder(My.Resources.EmailTemplate)
    Me.DocText = docbuilder.ToString
    Dim linebuilder As New StringBuilder(My.Resources.DatenZeilenTemplate)
    Me.LineTemplate = linebuilder.ToString
  End Sub

	''' <summary>
	''' Gibt den erzeugten Dokumenttext zurück
	''' </summary>
	Public ReadOnly Property DocumentText As String
		Get
			Return Me.DocText
		End Get
	End Property

	''' <summary>
	''' Gibt das Datenzeilentemplate zurück
	''' </summary>
	''' <returns></returns>
	Public ReadOnly Property DataLineTemplate As String
		Get
			Return Me.LineTemplate
		End Get
	End Property

	''' <summary>
	''' Ersetzt den Platzhalter für die Anrede
	''' </summary>
	Public Sub SetName(FirstName As String)
		Me.DocText = Me.DocText.Replace("%VORNAME%", FirstName)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Datentabelle 
	''' </summary>
	Public Sub SetDataTable()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.DatenTableTemplate)
		Me.DocText = Me.DocText.Replace("%DATENTABELLE%", builder.ToString)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Leerzelle 
	''' </summary>
	Public Sub SetBlankCell()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.LeerzellenTemplate)
		Me.DocText = Me.DocText.Replace("%LEERZELLE%", builder.ToString)
	End Sub

	''' <summary>
	''' Fügt die Datentabelle für Hohenwartedaten ein
	''' </summary>
	Public Sub SetHohenwarteData()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.HohenwarteDatenTabellenTemplate)
		Me.DocText = Me.DocText.Replace("%HOHENWARTEDATEN%", builder.ToString)
	End Sub

	''' <summary>
	''' Fügt die Datentabelle für Bleilochdaten ein
	''' </summary>
	Public Sub SetBleilochData()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.BleilochDatenTabellenTemplate)
		Me.DocText = Me.DocText.Replace("%BLEILOCHDATEN%", builder.ToString)
	End Sub

	''' <summary>
	''' Trägt die Daten in das Dokument ein
	''' </summary>
	Public Sub FillData(Data As String)
		Me.DocText = Me.DocText.Replace("%DATENZEILEN%", Data)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalte füt fen Applicationsname
	''' </summary>
	Public Sub SetAppName(Name As String)
		Me.DocText = Me.DocText.Replace("%APPLICATION%", Name)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für die Versionsnummer der Application
	''' </summary>
	Public Sub SetAppVersion(Version As String)
		Me.DocText = Me.DocText.Replace("%VERSION%", Version)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter für Copyrightifo in der Fusszeile
	''' </summary>
	Public Sub SetAppCopy(AppCopyright As String)
		Me.DocText = Me.DocText.Replace("%COPYRIGHT%", AppCopyright)
	End Sub

	''' <summary>
	''' Entfernt den angegebenen Platzhalter
	''' </summary>
	Public Sub RemovePlaceHolder(PlaceHolder As String)
		Me.DocText = Me.DocText.Replace(PlaceHolder, "")
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

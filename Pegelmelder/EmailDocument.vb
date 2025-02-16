'****************************************************************************************************************
'Document.vb
'(c) 2022 - 2025 by Andreas Sauer
'
' Diese Klasse stellt Methoden und Eigenschaften zur Verf�gung, um ein Dokument mit Platzhaltern zu erstellen und 
' diese Platzhalter durch tats�chliche Daten zu ersetzen. Die Platzhalter werden durch Vorlagen aus den Ressourcen 
' ersetzt, die in der Anwendung eingebettet sind.
'
'****************************************************************************************************************
'

Imports System.Text

Friend Class EmailDocument

	Private Const FIRST_NAME As String = "%VORNAME%"
	Private Const DATA_TABLE As String = "%DATENTABELLE%"
	Private Const BLANK_CELL As String = "%LEERZELLE%"
	Private Const HOHENWARTE_DATA As String = "%HOHENWARTEDATEN%"
	Private Const HOHENWARTE_IMAGE As String = "%HOHENWARTEIMAGE%"
	Private Const BLEILOCH_DATA As String = "%BLEILOCHDATEN%"
	Private Const BLEILOCH_IMAGE As String = "%BLEILOCHIMAGE%"
	Private Const DATA_LINES As String = "%DATENZEILEN%"
	Private Const APP_NAME As String = "%APPLICATION%"
	Private Const APP_VERSION As String = "%VERSION%"
	Private Const APP_COPY As String = "%COPYRIGHT%"

	Private DocText As String

	''' <summary>
	''' Erzeugt ein neues Dokument mit Platzhaltern
	''' </summary>
	Public Sub New()
		Dim docbuilder As New StringBuilder(My.Resources.EmailTemplate)
		Me.DocText = docbuilder.ToString
		Dim linebuilder As New StringBuilder(My.Resources.DatenZeilenTemplate)
		Me.DataLineTemplate = linebuilder.ToString
	End Sub

	''' <summary>
	''' Gibt den erzeugten Dokumenttext zur�ck
	''' </summary>
	Public ReadOnly Property DocumentText As String
		Get
			Return Me.DocText
		End Get
	End Property

	''' <summary>
	''' Gibt das Datenzeilentemplate zur�ck
	''' </summary>
	''' <returns></returns>
	Public ReadOnly Property DataLineTemplate As String

	''' <summary>
	''' Ersetzt den Platzhalter f�r die Anrede
	''' </summary>
	Public Sub SetName(FirstName As String)
		Me.DocText = Me.DocText.Replace(FIRST_NAME, FirstName)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter f�r die Datentabelle 
	''' </summary>
	Public Sub SetDataTable()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.DatenTableTemplate)
		Me.DocText = Me.DocText.Replace(DATA_TABLE, builder.ToString)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter f�r die Leerzelle 
	''' </summary>
	Public Sub SetBlankCell()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.LeerzellenTemplate)
		Me.DocText = Me.DocText.Replace(BLANK_CELL, builder.ToString)
	End Sub

	''' <summary>
	''' F�gt die Datentabelle f�r Hohenwartedaten ein
	''' </summary>
	Public Sub SetHohenwarteData()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.HohenwarteDatenTabellenTemplate)
		Me.DocText = Me.DocText.Replace(HOHENWARTE_DATA, builder.ToString)
	End Sub

	''' <summary>
	''' F�gt die Diagrammgrafik f�r Hohenwarte ein
	''' </summary>
	Public Sub InsertHohenwarteImage(imagecode As String)
		Me.DocText = Me.DocText.Replace(HOHENWARTE_IMAGE, imagecode)
	End Sub

	''' <summary>
	''' F�gt die Datentabelle f�r Bleilochdaten ein
	''' </summary>
	Public Sub SetBleilochData()
		Dim builder As New StringBuilder
		Dim unused = builder.AppendLine(My.Resources.BleilochDatenTabellenTemplate)
		Me.DocText = Me.DocText.Replace(BLEILOCH_DATA, builder.ToString)
	End Sub

	''' <summary>
	''' F�gt die Diagrammgrafik f�r Bleiloch ein
	''' </summary>
	Public Sub InsertBleilochImage(imagecode As String)
		Me.DocText = Me.DocText.Replace(BLEILOCH_IMAGE, imagecode)
	End Sub

	''' <summary>
	''' Tr�gt die Daten in das Dokument ein
	''' </summary>
	Public Sub FillData(Data As String)
		Me.DocText = Me.DocText.Replace(DATA_LINES, Data)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalte f�r den Applicationsname
	''' </summary>
	Public Sub SetAppName(Name As String)
		Me.DocText = Me.DocText.Replace(APP_NAME, Name)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter f�r die Versionsnummer der Application
	''' </summary>
	Public Sub SetAppVersion(Version As String)
		Me.DocText = Me.DocText.Replace(APP_VERSION, Version)
	End Sub

	''' <summary>
	''' Ersetzt den Platzhalter f�r Copyrightifo in der Fusszeile
	''' </summary>
	Public Sub SetAppCopy(AppCopyright As String)
		Me.DocText = Me.DocText.Replace(APP_COPY, AppCopyright)
	End Sub

	''' <summary>
	''' Entfernt den angegebenen Platzhalter
	''' </summary>
	Public Sub RemovePlaceHolder(PlaceHolder As String)
		Me.DocText = Me.DocText.Replace(PlaceHolder, "")
	End Sub


End Class

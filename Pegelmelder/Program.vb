'****************************************************************************************************************
'Program.vb
'(c) 2022 - 2024 by Andreas Sauer
'****************************************************************************************************************
'

Imports System.IO
Imports System.Reflection
Imports System.Text
Imports SchlumpfSoft.MailClient
Imports SchlumpfSoft.CsvFileManager
Imports SchlumpfSoft.WebSiteParser
Imports SchlumpfSoft.EmailDocument

Module Program

#Region "Konstantendefinition"
	Const URL As String = "https://group.vattenfall.com/de/energie/wasserkraft/saalekaskade/" ' URL der Website
	Const BLEILOCHDATAFILE As String = "Bleilochpegeldaten.csv" ' Datendatei für Bleilochpegeldaten
	Const HOHENWARTEDATAFILE As String = "Hohenwartepegeldaten.csv" ' Datendatei für Hohenwartepegeldaten
	Const EMAILDATAFILE As String = "Emaildaten.csv" ' Datendatei für Emailempfängerdaten
	Const EMAILDATAHEADER As String = "Vorname;Name;Emailadresse;Modus;Datensätze" ' Dateiheader für Emailempfängerdatendatei
	Const PEGELDATAHEADER As String = "Datum;Pegelstand;Differenz" ' Dateiheader für Pegeldatendateien
	Const DATAPATH As String = ".local/pm" ' Pfad zu den Datendateien
	Const CONFIGPATH As String = ".config/pm" ' Pfad zur Konfigurationsdatei
	Const CONFIGFILE As String = "pm.conf" ' Name der Konfigurationsdatei
#End Region

#Region "Variablendefinition"
	Private NewHohenwarteData As Boolean ' neue Daten für Hohenwarthe
	Private NewBleilochData As Boolean   ' neue Daten für Bleiloch
	Private ConfigFilePath As String ' Pfad für Konfigurationsdateien
	Private DataFilePath As String ' Pfad für Datendateien
	Private Server As String ' Serveradresse
	Private User As String ' Benutzername
	Private Passw As String ' Passwort
	Private Port As String  ' Serverport
	Private Absender As String ' Emailabsender

	'Public _manageserver As String	' Managerserveradresse
	'Public _managerserverport As String ' Managerserverport

#End Region

	Sub Main()
		SetConfigPath()  'KofigurationsPfad initialisieren
		SetDataPath()  'Datenpfad initialisieren

#Region "Konfigurationsdatei"

		'Konfigurationsdatei überprüfen
		Dim conffile As String = ConfigFilePath & Path.DirectorySeparatorChar & CONFIGFILE
		Console.WriteLine("Prüfe ob Konfigurationsdatei existiert (" & conffile & ")")

		'neue Konfigurationsdatei erstellen wenn sie nicht existiert
		If Not File.Exists(conffile) Then
			Console.WriteLine("Die Konfigurationsdatei " & conffile & " wurde nicht gefunden.")
			Dim builder As New StringBuilder
			Dim unused = builder.AppendLine(My.Resources.ConfigFileTemplate)
			File.WriteAllText(conffile, builder.ToString)
			Console.WriteLine("Es wurde eine neue leere Datei erstellt.")
			Console.WriteLine("Bitte trage deine Daten in diese Datei ein.")
			Exit Sub
		End If

		'Konfiguration für laden
		LoadConfig(conffile)

		'Konfiguration für Server überprüfen
		If Server = "" Or User = "" Or Passw = "" Or Port = "" Or Absender = "" Then
			Console.WriteLine("Die Konfigurationsdatei " & conffile & " ist fehlerhaft.")
			Console.WriteLine("Bitte trage die korrekten Daten in diese Datei ein.")
			Exit Sub
		End If

#End Region

#Region "Emaildatenbank"

		'Prüfe ob Datenbank für Emailempfänger existiert
		Dim emldatafile As String = ConfigFilePath & Path.DirectorySeparatorChar & EMAILDATAFILE
		Console.WriteLine("Prüfe ob Datenbank für Emaildaten existiert (" & emldatafile & ")")

		'Datenbank erstellen falls sie nicht existiert
		If Not File.Exists(emldatafile) Then
			Console.WriteLine("Die Datenbank für Emaildaten (" & emldatafile & ") existiert noch nicht.")
			Dim builder As New StringBuilder
			Dim unused = builder.AppendLine(My.Resources.EmaildatenTemplate)
			File.WriteAllText(emldatafile, builder.ToString)
			Console.WriteLine("Es wurde eine neue leere Datenbank erstellt.")
			Console.WriteLine("In diese Datenbank müssen noch die Empfänger der Emails eingetragen werden.")
			Exit Sub
		End If

#End Region

		'Merker für neue Daten setzen
		NewHohenwarteData = False
		NewBleilochData = False

		'Neue Pegeldaten speichern
		NewHohenwarteData = AddHohenwartePegel(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE)
		NewBleilochData = AddBleilochPegel(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE)

		'Differenzwerte berechnen wenn neue Daten in Bleiloch vorhanden sind
		If NewBleilochData Then
			Console.WriteLine($"Program.Main:")
			Console.WriteLine($"Neue Daten in Bleiloch vorhanden. -> Differenz berechnen")
			CalculateBleilochDifferences(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE)
		End If

		'Differenzwerte berechnen wenn neue Daten in Hohenwarte vorhanden sind
		If NewHohenwarteData Then
			Console.WriteLine($"Program.Main:")
			Console.WriteLine($"Neue Daten in Hohenwarte vorhanden. -> Differenz berechnen")
			CalculateHohenwarteDifferences(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE)
		End If

		'Überprüfen ob neue Daten vorliegen
		If NewHohenwarteData = False And NewBleilochData = False Then
			'Beenden wenn keine neuen Daten vorhanden sind
			Exit Sub
		Else
			'Emails senden wenn neue Daten vorhanden sind
			SendMails()
		End If
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Private Sub SendMail(FirstName As String, Email As String, Modus As Integer, Records As Integer)
		Dim doc As New Document 'Emailvorlage erstellen
		doc.SetName(FirstName) 'Anrede einsetzen
		doc.SetDataTable() 'Datentabelle einfügen

		'Verwaltungslink einsetzen
		'document.SetManageLink(
		'	"http://" &
		'	ServerVariables._manageserver & ":" &
		'	ServerVariables._managerserverport &
		'	"/manage?vname=" & FirstName & "&email=" & Email)

		'Fusszeile komplettieren
		doc.SetAppName(Assembly.GetExecutingAssembly().GetName.Name)
		doc.SetAppVersion(Assembly.GetExecutingAssembly().GetName.Version.ToString)
		doc.SetAppCopy("(Copyright © 2024 by Andreas Sauer)")
		'Vorlage je nach Modus anpassen und Daten eintragen
		Dim linetemplate As String = doc.DataLineTemplate
		Dim bleilochdata As String = GetBleilochData(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE, Records, linetemplate)
		Dim hohenwartedata As String = GetHohenwarteData(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE, Records, linetemplate)
		Select Case Modus
			Case 0  'nur Hohenwartedaten eintragen
				doc.RemovePlaceHolder("%LEERZELLE%")
				doc.RemovePlaceHolder("%BLEILOCHDATEN%")
				doc.SetHohenwarteData()
				doc.FillData(hohenwartedata)
			Case 1 'nur Bleilochdaten eintragen
				doc.RemovePlaceHolder("%HOHENWARTEDATEN%")
				doc.RemovePlaceHolder("%LEERZELLE%")
				doc.SetBleilochData()
				doc.FillData(bleilochdata)
			Case 2 'beide Daten eintragen
				doc.SetHohenwarteData()
				doc.FillData(hohenwartedata)
				doc.SetBlankCell()
				doc.SetBleilochData()
				doc.FillData(bleilochdata)
			Case Else
				Exit Sub  'Fehler -> Ende
		End Select
		Dim mail As New Client(Server, CInt(Port), User, Passw)
		mail.Send(Absender, "pegelmelder", "Pegeldaten", Email, doc.DocumentText)
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Private Function LoadEmailData() As List(Of String)
		Dim datafilepath As String = ConfigFilePath & Path.DirectorySeparatorChar & EMAILDATAFILE
		Dim DataFile As New CsvFile(datafilepath, EMAILDATAHEADER)
		Return DataFile.Data
	End Function

  ''' <summary>
  ''' Fügt einen neuen Datensatz in die Bleilochpegeldaten ein
  ''' falls noch nicht vorhanden.
  ''' </summary>
  Private Function AddBleilochPegel(File As String) As Boolean
		Dim result As Boolean = False
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'alle Datensätze der Webdaten durchsuchen
			Dim WebData As New Parser(URL)
			Dim index As Integer
			For Each row As String In WebData.GetBleilochData
				'überprüfen ob Datum bereits existiert und Datensatz eintragen falls nicht
				index = PegelDataFile.FindRecord(row.Split(";").First)
				If index = -1 Then
					Console.WriteLine($"Program.AddBleilochPegel:")
					Console.WriteLine($"Es wurde ein neuer Datensatz gefunden:")
					Console.WriteLine($"{row}")
					PegelDataFile.AddRecord(row)
					result = True
				End If
			Next
		End Using
		Return result
	End Function

	''' <summary>
	''' Fügt einen neuen Datensatz in die Hohenwartepegeldaten ein
	''' falls noch nicht vorhanden.
	''' </summary>
	Private Function AddHohenwartePegel(File As String) As Boolean
		Dim result As Boolean = False
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'alle Datensätze der Webdaten durchsuchen
			Dim WebData As New Parser(URL)
			Dim index As Integer
			For Each row As String In WebData.GetHohenwarteData
				'überprüfen ob Datum bereits existiert und Datensatz eintragen falls nicht
				index = PegelDataFile.FindRecord(row.Split(";").First)
				If index = -1 Then
					Console.WriteLine($"Program.AddHohenwartePegel:")
					Console.WriteLine($"Es wurde ein neuer Datensatz gefunden:")
					Console.WriteLine($"{row}")
					PegelDataFile.AddRecord(row)
					result = True
				End If
			Next
		End Using
		Return result
	End Function

	''' <summary>
	''' 
	''' </summary>
	Private Sub CalculateHohenwarteDifferences(File As String)
		Dim tempdata As List(Of String)
		Dim oldpegel As Integer
		Dim newpegel As Integer
		Dim differenz As Integer
		'Differenzen für Hohenwarte berechnen
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'Datensätze zwischenspeichern
			tempdata = PegelDataFile.Data
			'Alle Datensätze durchlaufen (ausser Header)
			For index As Integer = 1 To tempdata.Count - 1
				If index = 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz für 1. Datensatz auf 0 setzen falls noch nicht erfolgt
					PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";+000000")
				ElseIf tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz für alle anderen Datensätze berechnen falls noch nicht erfolgt
					oldpegel = CInt(tempdata.ElementAt(index - 1).Split(";").ElementAt(1))
					newpegel = CInt(tempdata.ElementAt(index).Split(";").ElementAt(1))
					differenz = newpegel - oldpegel
					'Vorzeichen für die Differenz setzen und Datensätze ersetzen
					If differenz < 0 Then
						'Differenz ist < 0
						PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";" & Format(differenz, "000000"))
					Else
						'Differenz ist >= 0 oder 
						PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";+" & Format(differenz, "000000"))
					End If
				End If
			Next
		End Using
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Private Sub CalculateBleilochDifferences(File As String)
		Dim tempdata As List(Of String)
		Dim oldpegel As Integer
		Dim newpegel As Integer
		Dim differenz As Integer
		'Differenzen für Bleiloch berechnen
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'Datensätze zwischenspeichern
			tempdata = PegelDataFile.Data
			'Alle Datensätze durchlaufen (ausser Header)
			For index As Integer = 1 To tempdata.Count - 1
				If index = 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz für 1. Datensatz auf 0 setzen falls noch nicht erfolgt
					PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";000000")
				ElseIf tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz für alle anderen Datensätze berechnen falls noch nicht erfolgt
					oldpegel = CInt(tempdata.ElementAt(index - 1).Split(";").ElementAt(1))
					newpegel = CInt(tempdata.ElementAt(index).Split(";").ElementAt(1))
					differenz = newpegel - oldpegel
					'Vorzeichen für die Differenz setzen und Datensätze ersetzen
					If differenz < 0 Then
						'Differenz ist < 0
						PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";" & Format(differenz, "000000"))
					Else
						'Differenz ist >= 0 oder
						PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";+" & Format(differenz, "000000"))
					End If
				End If
			Next
		End Using
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Private Function GetBleilochData(File As String, Records As Integer, Linetemplate As String) As String
		Dim result As String = ""
		Dim record As String = Linetemplate
		Dim data As New CsvFile(File, PEGELDATAHEADER)
		Dim length As Integer = data.Data.Count - 1
		'maximale Anzahl der Datensätze anpassen wenn weniger Daten vorhanden als gewünscht
		If Records > length Then
			Records = length
		End If
		'Die letzten in Records gespeicherten Datensätze durchlaufen
		For index As Integer = length + 1 - Records To length
			record = record.Replace("%DATUM%", data.Data.ElementAt(index).Split(";").ElementAt(0))
			record = record.Replace("%PEGEL%", data.Data.ElementAt(index).Split(";").ElementAt(1))
			record = record.Replace("%DIFFERENZ%", data.Data.ElementAt(index).Split(";").ElementAt(2))
			result &= record
		Next
		Return result
	End Function

	''' <summary>
	''' 
	''' </summary>
	Private Function GetHohenwarteData(File As String, Records As Integer, Linetemplate As String) As String
		Dim result As String = ""
		Dim record As String = Linetemplate
		Dim data As New CsvFile(File, PEGELDATAHEADER)
		Dim length As Integer = data.Data.Count - 1
		'maximale Anzahl der Datensätze anpassen wenn weniger Daten vorhanden als gewünscht
		If Records > length Then
			Records = length
		End If
		'Die letzten in Records gespeicherten Datensätze durchlaufen
		For index As Integer = length + 1 - Records To length
			record = record.Replace("%DATUM%", data.Data.ElementAt(index).Split(";").ElementAt(0))
			record = record.Replace("%PEGEL%", data.Data.ElementAt(index).Split(";").ElementAt(1))
			record = record.Replace("%DIFFERENZ%", data.Data.ElementAt(index).Split(";").ElementAt(2))
			result &= record
		Next
		Return result
	End Function

	''' <summary>
	''' Emails an alle Empfänger senden
	''' </summary>
	Private Sub SendMails()
		Dim vname As String
		Dim name As String
		Dim email As String
		Dim modus As Integer
		Dim records As Integer
		'alle Emaildaten durchlaufen (Header ignorieren)
		For Each data As String In LoadEmailData().Skip(1)
			vname = data.Split(";").ElementAt(0)
			name = data.Split(";").ElementAt(1)
			email = data.Split(";").ElementAt(2)
			modus = CInt(data.Split(";").ElementAt(3))
			records = CInt(data.Split(";").ElementAt(4))
#If DEBUG Then
			Console.WriteLine($"Email an {vname} {name} ({email}) senden mit Modus {modus} und {records} Datensätzen.")
#End If
			SendMail(vname, email, modus, records)
		Next
	End Sub

	''' <summary>
	''' Initialisiert die Pfadvariable für Konfiguration und erstellt einen Ordner falls er nicht existiert
	''' </summary>
	Private Sub SetConfigPath()
		'Ordner zur Konfiguration festlegen
		ConfigFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & CONFIGPATH
#If DEBUG Then
		Console.WriteLine($"Pfad für Konfigurationsdateien: {ConfigFilePath}")
#End If
		'Ordner für Konfigurationsdateien erstellen falls er nicht existiert
		If Not Directory.Exists(ConfigFilePath) Then
#If DEBUG Then
			Console.WriteLine($"Ordner existiert nicht und wird erstellt.")
#End If
			Dim unused = Directory.CreateDirectory(ConfigFilePath)
		End If
	End Sub

	''' <summary>
	''' Initialisiert die Pfadvariabel für Daten und erstellt 
	''' einen Ordner falls er nicht existiert.
	''' </summary>
	Private Sub SetDataPath()
		'Ordner zur Datenspeicherung festlegen
		DataFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & DATAPATH
#If DEBUG Then
		Console.WriteLine($"Pfad für die Peglemelderdatendateien: {DataFilePath}")
#End If
		'Ordner für Datenspeicherung erstellen falls er nicht existiert
		If Not Directory.Exists(DataFilePath) Then
#If DEBUG Then
			Console.WriteLine($"Ordner existiert nicht und wird erstellt.")
#End If
			Dim unused = Directory.CreateDirectory(DataFilePath)
		End If
	End Sub

	''' <summary>
	''' Lädt die Konfigurationsdaten.
	''' </summary>
	Private Sub LoadConfig(file As String)
		For Each line As String In IO.File.ReadAllLines(file)
			Select Case True
				Case line.StartsWith("Server:") : Server = line.Split(":").Last
				Case line.StartsWith("User:") : User = line.Split(":").Last
				Case line.StartsWith("Passwort:") : Passw = line.Split(":").Last
				Case line.StartsWith("Port:") : Port = line.Split(":").Last
				Case line.StartsWith("Absender:") : Absender = line.Split(":").Last
					'Case line.StartsWith("ManagerServer:") : _manageserver = line.Split(":").Last
					'Case line.StartsWith("ManagerServerPort:") : _managerserverport = line.Split(":").Last
			End Select
		Next
	End Sub

End Module

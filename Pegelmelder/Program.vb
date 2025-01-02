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
	Const BLEILOCHDATAFILE As String = "Bleilochpegeldaten.csv" ' Datendatei f�r Bleilochpegeldaten
	Const HOHENWARTEDATAFILE As String = "Hohenwartepegeldaten.csv" ' Datendatei f�r Hohenwartepegeldaten
	Const EMAILDATAFILE As String = "Emaildaten.csv" ' Datendatei f�r Emailempf�ngerdaten
	Const EMAILDATAHEADER As String = "Vorname;Name;Emailadresse;Modus;Datens�tze" ' Dateiheader f�r Emailempf�ngerdatendatei
	Const PEGELDATAHEADER As String = "Datum;Pegelstand;Differenz" ' Dateiheader f�r Pegeldatendateien
	Const DATAPATH As String = ".local/pm" ' Pfad zu den Datendateien
	Const CONFIGPATH As String = ".config/pm" ' Pfad zur Konfigurationsdatei
	Const CONFIGFILE As String = "pm.conf" ' Name der Konfigurationsdatei
#End Region

#Region "Variablendefinition"
	Private NewHohenwarteData As Boolean ' neue Daten f�r Hohenwarthe
	Private NewBleilochData As Boolean   ' neue Daten f�r Bleiloch
	Private ConfigFilePath As String ' Pfad f�r Konfigurationsdateien
	Private DataFilePath As String ' Pfad f�r Datendateien
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

		'Konfigurationsdatei �berpr�fen
		Dim conffile As String = ConfigFilePath & Path.DirectorySeparatorChar & CONFIGFILE
		Console.WriteLine("Pr�fe ob Konfigurationsdatei existiert (" & conffile & ")")

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

		'Konfiguration f�r laden
		LoadConfig(conffile)

		'Konfiguration f�r Server �berpr�fen
		If Server = "" Or User = "" Or Passw = "" Or Port = "" Or Absender = "" Then
			Console.WriteLine("Die Konfigurationsdatei " & conffile & " ist fehlerhaft.")
			Console.WriteLine("Bitte trage die korrekten Daten in diese Datei ein.")
			Exit Sub
		End If

#End Region

#Region "Emaildatenbank"

		'Pr�fe ob Datenbank f�r Emailempf�nger existiert
		Dim emldatafile As String = ConfigFilePath & Path.DirectorySeparatorChar & EMAILDATAFILE
		Console.WriteLine("Pr�fe ob Datenbank f�r Emaildaten existiert (" & emldatafile & ")")

		'Datenbank erstellen falls sie nicht existiert
		If Not File.Exists(emldatafile) Then
			Console.WriteLine("Die Datenbank f�r Emaildaten (" & emldatafile & ") existiert noch nicht.")
			Dim builder As New StringBuilder
			Dim unused = builder.AppendLine(My.Resources.EmaildatenTemplate)
			File.WriteAllText(emldatafile, builder.ToString)
			Console.WriteLine("Es wurde eine neue leere Datenbank erstellt.")
			Console.WriteLine("In diese Datenbank m�ssen noch die Empf�nger der Emails eingetragen werden.")
			Exit Sub
		End If

#End Region

		'Merker f�r neue Daten setzen
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

		'�berpr�fen ob neue Daten vorliegen
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
		doc.SetDataTable() 'Datentabelle einf�gen

		'Verwaltungslink einsetzen
		'document.SetManageLink(
		'	"http://" &
		'	ServerVariables._manageserver & ":" &
		'	ServerVariables._managerserverport &
		'	"/manage?vname=" & FirstName & "&email=" & Email)

		'Fusszeile komplettieren
		doc.SetAppName(Assembly.GetExecutingAssembly().GetName.Name)
		doc.SetAppVersion(Assembly.GetExecutingAssembly().GetName.Version.ToString)
		doc.SetAppCopy("(Copyright � 2024 by Andreas Sauer)")
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
  ''' F�gt einen neuen Datensatz in die Bleilochpegeldaten ein
  ''' falls noch nicht vorhanden.
  ''' </summary>
  Private Function AddBleilochPegel(File As String) As Boolean
		Dim result As Boolean = False
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'alle Datens�tze der Webdaten durchsuchen
			Dim WebData As New Parser(URL)
			Dim index As Integer
			For Each row As String In WebData.GetBleilochData
				'�berpr�fen ob Datum bereits existiert und Datensatz eintragen falls nicht
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
	''' F�gt einen neuen Datensatz in die Hohenwartepegeldaten ein
	''' falls noch nicht vorhanden.
	''' </summary>
	Private Function AddHohenwartePegel(File As String) As Boolean
		Dim result As Boolean = False
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'alle Datens�tze der Webdaten durchsuchen
			Dim WebData As New Parser(URL)
			Dim index As Integer
			For Each row As String In WebData.GetHohenwarteData
				'�berpr�fen ob Datum bereits existiert und Datensatz eintragen falls nicht
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
		'Differenzen f�r Hohenwarte berechnen
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'Datens�tze zwischenspeichern
			tempdata = PegelDataFile.Data
			'Alle Datens�tze durchlaufen (ausser Header)
			For index As Integer = 1 To tempdata.Count - 1
				If index = 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz f�r 1. Datensatz auf 0 setzen falls noch nicht erfolgt
					PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";+000000")
				ElseIf tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz f�r alle anderen Datens�tze berechnen falls noch nicht erfolgt
					oldpegel = CInt(tempdata.ElementAt(index - 1).Split(";").ElementAt(1))
					newpegel = CInt(tempdata.ElementAt(index).Split(";").ElementAt(1))
					differenz = newpegel - oldpegel
					'Vorzeichen f�r die Differenz setzen und Datens�tze ersetzen
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
		'Differenzen f�r Bleiloch berechnen
		Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)
			'Datens�tze zwischenspeichern
			tempdata = PegelDataFile.Data
			'Alle Datens�tze durchlaufen (ausser Header)
			For index As Integer = 1 To tempdata.Count - 1
				If index = 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz f�r 1. Datensatz auf 0 setzen falls noch nicht erfolgt
					PegelDataFile.ReplaceValue(index, tempdata.ElementAt(index) & ";000000")
				ElseIf tempdata.ElementAt(index).Split(";").Length = 2 Then
					'Differenz f�r alle anderen Datens�tze berechnen falls noch nicht erfolgt
					oldpegel = CInt(tempdata.ElementAt(index - 1).Split(";").ElementAt(1))
					newpegel = CInt(tempdata.ElementAt(index).Split(";").ElementAt(1))
					differenz = newpegel - oldpegel
					'Vorzeichen f�r die Differenz setzen und Datens�tze ersetzen
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
		'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
		If Records > length Then
			Records = length
		End If
		'Die letzten in Records gespeicherten Datens�tze durchlaufen
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
		'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
		If Records > length Then
			Records = length
		End If
		'Die letzten in Records gespeicherten Datens�tze durchlaufen
		For index As Integer = length + 1 - Records To length
			record = record.Replace("%DATUM%", data.Data.ElementAt(index).Split(";").ElementAt(0))
			record = record.Replace("%PEGEL%", data.Data.ElementAt(index).Split(";").ElementAt(1))
			record = record.Replace("%DIFFERENZ%", data.Data.ElementAt(index).Split(";").ElementAt(2))
			result &= record
		Next
		Return result
	End Function

	''' <summary>
	''' Emails an alle Empf�nger senden
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
			Console.WriteLine($"Email an {vname} {name} ({email}) senden mit Modus {modus} und {records} Datens�tzen.")
#End If
			SendMail(vname, email, modus, records)
		Next
	End Sub

	''' <summary>
	''' Initialisiert die Pfadvariable f�r Konfiguration und erstellt einen Ordner falls er nicht existiert
	''' </summary>
	Private Sub SetConfigPath()
		'Ordner zur Konfiguration festlegen
		ConfigFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & CONFIGPATH
#If DEBUG Then
		Console.WriteLine($"Pfad f�r Konfigurationsdateien: {ConfigFilePath}")
#End If
		'Ordner f�r Konfigurationsdateien erstellen falls er nicht existiert
		If Not Directory.Exists(ConfigFilePath) Then
#If DEBUG Then
			Console.WriteLine($"Ordner existiert nicht und wird erstellt.")
#End If
			Dim unused = Directory.CreateDirectory(ConfigFilePath)
		End If
	End Sub

	''' <summary>
	''' Initialisiert die Pfadvariabel f�r Daten und erstellt 
	''' einen Ordner falls er nicht existiert.
	''' </summary>
	Private Sub SetDataPath()
		'Ordner zur Datenspeicherung festlegen
		DataFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & DATAPATH
#If DEBUG Then
		Console.WriteLine($"Pfad f�r die Peglemelderdatendateien: {DataFilePath}")
#End If
		'Ordner f�r Datenspeicherung erstellen falls er nicht existiert
		If Not Directory.Exists(DataFilePath) Then
#If DEBUG Then
			Console.WriteLine($"Ordner existiert nicht und wird erstellt.")
#End If
			Dim unused = Directory.CreateDirectory(DataFilePath)
		End If
	End Sub

	''' <summary>
	''' L�dt die Konfigurationsdaten.
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

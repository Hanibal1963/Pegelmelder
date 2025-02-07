'****************************************************************************************************************
'Program.vb
'(c) 2022 - 2025 by Andreas Sauer
'
' Dieses Programm überprüft und erstellt Konfigurations- und Datenbankdateien, 
' lädt Konfigurationsdaten, speichert neue Pegeldaten und sendet E-Mails, wenn neue Daten vorliegen.
'
'****************************************************************************************************************
'

Imports System.IO
Imports System.Text

Module Program

	Sub Main()

		'KofigurationsPfad initialisieren
		IntitFunctions.SetConfigPath()

		'Datenpfad initialisieren
		IntitFunctions.SetDataPath()

#Region "Konfigurationsdatei"

		'Konfigurationsdatei überprüfen
		Dim conffile As String = ConfigFilePath & Path.DirectorySeparatorChar & CONFIGFILE
		'Console.WriteLine($"Prüfe ob Konfigurationsdatei existiert (""{conffile}"")")
		Console.WriteLine(String.Format(My.Resources.CheckingConfigFile, conffile))

		'neue Konfigurationsdatei erstellen wenn sie nicht existiert
		If Not File.Exists(conffile) Then

			Console.WriteLine(String.Format(My.Resources.ConfigFileNotFound, conffile))
			Dim builder As New StringBuilder
			Dim unused = builder.AppendLine(My.Resources.ConfigFileTemplate)
			File.WriteAllText(conffile, builder.ToString)
			Console.WriteLine(My.Resources.NewConfigFileCreated)
			Console.WriteLine(My.Resources.InsertConfigFileData)
			Exit Sub

		End If
		IntitFunctions.LoadConfig(conffile)

		'Konfiguration für Server überprüfen
		If Server = "" Or User = "" Or Passw = "" Or Port = "" Or Absender = "" Then

			Console.WriteLine($"Die Konfigurationsdatei (""{conffile}"") ist fehlerhaft.")
			Console.WriteLine($"Bitte trage die korrekten Daten in diese Datei ein.")
			Exit Sub

		End If

#End Region

#Region "Emaildatenbank"

		'Prüfe ob Datenbank für Emailempfänger existiert
		Dim emldatafile As String = ConfigFilePath & Path.DirectorySeparatorChar & EMAILDATAFILE
		Console.WriteLine(String.Format(My.Resources.CheckingEmailDataFile, emldatafile))

		'Datenbank erstellen falls sie nicht existiert
		If Not File.Exists(emldatafile) Then

			Console.WriteLine(String.Format(My.Resources.EmailDataFileNotFound, emldatafile))
			Dim builder As New StringBuilder
			Dim unused = builder.AppendLine(My.Resources.EmaildatenTemplate)
			File.WriteAllText(emldatafile, builder.ToString)
			Console.WriteLine(My.Resources.NewEmailDataFileCreated)
			Console.WriteLine(My.Resources.InsertEmailData)
			Exit Sub

		End If

#End Region

		'Merker für neue Daten setzen
		NewHohenwarteData = False
		NewBleilochData = False

		'Neue Pegeldaten speichern
		NewHohenwarteData = HohenwarteFunctions.AddHohenwartePegel(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE)
		NewBleilochData = BleilochFunctions.AddBleilochPegel(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE)

		'Differenzwerte berechnen wenn neue Daten in Bleiloch vorhanden sind
		If NewBleilochData Then
			BleilochFunctions.CalculateBleilochDifferences(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE)
		End If

		'Differenzwerte berechnen wenn neue Daten in Hohenwarte vorhanden sind
		If NewHohenwarteData Then
			HohenwarteFunctions.CalculateHohenwarteDifferences(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE)
		End If

		'Überprüfen ob neue Daten vorliegen
		If NewHohenwarteData = False And NewBleilochData = False Then
			'Beenden wenn keine neuen Daten vorhanden sind
			Exit Sub
		Else
			MailFunctions.SendMails()
		End If

	End Sub

End Module

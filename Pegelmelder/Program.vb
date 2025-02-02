'****************************************************************************************************************
'Program.vb
'(c) 2022 - 2025 by Andreas Sauer
'****************************************************************************************************************
'

Imports System.IO
Imports System.Text

Module Program

	Sub Main()
    IntitFunctions.SetConfigPath()  'KofigurationsPfad initialisieren
    IntitFunctions.SetDataPath()  'Datenpfad initialisieren

#Region "Konfigurationsdatei"

		'Konfigurationsdatei überprüfen
		Dim conffile As String = ConfigFilePath & Path.DirectorySeparatorChar & CONFIGFILE
		Console.WriteLine($"Prüfe ob Konfigurationsdatei existiert (""{conffile}"")")

		'neue Konfigurationsdatei erstellen wenn sie nicht existiert
		If Not File.Exists(conffile) Then

			Console.WriteLine($"Die Konfigurationsdatei (""{conffile}"") wurde nicht gefunden.")
			Dim builder As New StringBuilder
			Dim unused = builder.AppendLine(My.Resources.ConfigFileTemplate)
			File.WriteAllText(conffile, builder.ToString)
			Console.WriteLine($"Es wurde eine neue leere Datei erstellt.")
			Console.WriteLine($"Bitte trage deine Daten in diese Datei ein.")
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
		Console.WriteLine($"Prüfe ob Datenbank für Emaildaten existiert (""{emldatafile}"")")

		'Datenbank erstellen falls sie nicht existiert
		If Not File.Exists(emldatafile) Then

			Console.WriteLine($"Die Datenbank für Emaildaten (""{emldatafile }"") existiert noch nicht.")
			Dim builder As New StringBuilder
			Dim unused = builder.AppendLine(My.Resources.EmaildatenTemplate)
			File.WriteAllText(emldatafile, builder.ToString)
			Console.WriteLine($"Es wurde eine neue leere Datenbank erstellt.")
			Console.WriteLine($"In diese Datenbank müssen noch die Empfänger der Emails eingetragen werden.")
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

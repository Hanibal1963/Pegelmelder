' ****************************************************************************************************************
' ConstantDefinitions.vb
' © 2025 by Andreas Sauer
' ****************************************************************************************************************
'

Module ConstantDefinitions

	' Datenquelle
	Friend Const URL As String = "https://group.vattenfall.com/de/energie/wasserkraft/saalekaskade/" ' URL der Website

	' Datenziele
	Friend Const DATAPATH As String = ".local/share/pm" ' Pfad zu den Datendateien für die Pegeldaten
	Friend Const BLEILOCHDATAFILE As String = "Bleilochpegeldaten.csv" ' Datendatei für Bleilochpegeldaten
	Friend Const HOHENWARTEDATAFILE As String = "Hohenwartepegeldaten.csv" ' Datendatei für Hohenwartepegeldaten

	' Dateiheader
	Friend Const EMAILDATAHEADER As String = "Vorname;Name;Emailadresse;Modus;Datensätze" ' Dateiheader für Emailempfängerdatendatei
	Friend Const PEGELDATAHEADER As String = "Datum;Pegelstand;Differenz" ' Dateiheader für Pegeldatendateien

	' Konfigurationsdaten
	Friend Const CONFIGPATH As String = ".config/pm" ' Pfad zu den Konfigurationsdateien
	Friend Const EMAILDATAFILE As String = "Emaildaten.csv" ' Name der Konfigurationsdatei für Emailempfängerdaten
	Friend Const CONFIGFILE As String = "pm.conf" ' Name der Konfigurationsdatei für den Emailserver

End Module

' ****************************************************************************************************************
' ConstantDefinitions.vb
' © 2025 by Andreas Sauer
' ****************************************************************************************************************
'

Module ConstantDefinitions

	Friend Const URL As String = "https://group.vattenfall.com/de/energie/wasserkraft/saalekaskade/" ' URL der Website
	Friend Const BLEILOCHDATAFILE As String = "Bleilochpegeldaten.csv" ' Datendatei für Bleilochpegeldaten
	Friend Const HOHENWARTEDATAFILE As String = "Hohenwartepegeldaten.csv" ' Datendatei für Hohenwartepegeldaten
	Friend Const EMAILDATAFILE As String = "Emaildaten.csv" ' Datendatei für Emailempfängerdaten
	Friend Const EMAILDATAHEADER As String = "Vorname;Name;Emailadresse;Modus;Datensätze" ' Dateiheader für Emailempfängerdatendatei
	Friend Const PEGELDATAHEADER As String = "Datum;Pegelstand;Differenz" ' Dateiheader für Pegeldatendateien
	Friend Const DATAPATH As String = ".local/share/pm" ' Pfad zu den Datendateien
	Friend Const CONFIGPATH As String = ".config/pm" ' Pfad zur Konfigurationsdatei
	Friend Const CONFIGFILE As String = "pm.conf" ' Name der Konfigurationsdatei

End Module

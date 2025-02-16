' Copyright (c)2022-2025 by Andreas Sauer 

Imports System.IO
Imports System.Reflection
Imports System.Text

Namespace Pegelmelder

  Module Program

    ' Dieses Programm �berpr�ft und erstellt Konfigurations- und Datenbankdateien, 
    ' l�dt Konfigurationsdaten, speichert neue Pegeldaten und sendet E-Mails, wenn neue Daten vorliegen.

#Region "Konstantendefinitionen"

    ' Datenquelle
    Private Const URL As String = "https://group.vattenfall.com/de/energie/wasserkraft/saalekaskade/" ' URL der Website

    ' Datenziele
    Private Const DATAPATH As String = ".local/share/pm" ' Pfad zu den Datendateien f�r die Pegeldaten
    Private Const BLEILOCHDATAFILE As String = "Bleilochpegeldaten.csv" ' Datendatei f�r Bleilochpegeldaten
    Private Const HOHENWARTEDATAFILE As String = "Hohenwartepegeldaten.csv" ' Datendatei f�r Hohenwartepegeldaten

    ' Dateiheader
    Private Const EMAILDATAHEADER As String = "Vorname;Name;Emailadresse;Modus;Datens�tze" ' Dateiheader f�r Emailempf�ngerdatendatei
    Private Const PEGELDATAHEADER As String = "Datum;Pegelstand;Differenz" ' Dateiheader f�r Pegeldatendateien

    ' Konfigurationsdaten
    Private Const CONFIGPATH As String = ".config/pm" ' Pfad zu den Konfigurationsdateien
    Private Const EMAILDATAFILE As String = "Emaildaten.csv" ' Name der Konfigurationsdatei f�r Emailempf�ngerdaten
    Private Const CONFIGFILE As String = "pm.conf" ' Name der Konfigurationsdatei f�r den Emailserver

#End Region

#Region "Variablendefinitionen"

    Private NewHohenwarteData As Boolean ' neue Daten f�r Hohenwarthe
    Private NewBleilochData As Boolean   ' neue Daten f�r Bleiloch
    Private ConfigFilePath As String ' Pfad f�r Konfigurationsdateien
    Private DataFilePath As String ' Pfad f�r Datendateien
    Private Server As String ' Serveradresse
    Private User As String ' Benutzername
    Private Passw As String ' Passwort
    Private Port As String  ' Serverport
    Private Absender As String ' Emailabsender

#End Region

    Sub Main()

      'KofigurationsPfad initialisieren
      SetConfigPath()

      'Datenpfad initialisieren
      SetDataPath()

#Region "Konfigurationsdatei"

      'Konfigurationsdatei �berpr�fen
      Dim conffile As String = ConfigFilePath & Path.DirectorySeparatorChar & CONFIGFILE
      Console.WriteLine(String.Format(My.Resources.CheckingConfigFile, conffile))

      'neue Konfigurationsdatei erstellen wenn sie nicht existiert
      If Not File.Exists(conffile) Then
        CreateConfigFile(conffile)
        Exit Sub
      End If

      LoadConfig(conffile)

      'Konfiguration f�r Server �berpr�fen
      Select Case True
        Case String.IsNullOrEmpty(Server)
          PrintMsg($"Die Serveradresse", conffile)
          Exit Sub

        Case String.IsNullOrEmpty(User)
          PrintMsg($"Der Benutzername", conffile)
          Exit Sub

        Case String.IsNullOrEmpty(Passw)
          PrintMsg($"Das Passwort", conffile)
          Exit Sub

        Case String.IsNullOrEmpty(Port)
          PrintMsg($"Der Serverport", conffile)
          Exit Sub

        Case String.IsNullOrEmpty(Absender)
          PrintMsg($"Die Absender Email", conffile)
          Exit Sub

        Case Else
          Exit Select

      End Select

#End Region

#Region "Emaildatenbank"

      'Pr�fe ob Datenbank f�r Emailempf�nger existiert
      Dim emldatafile As String = ConfigFilePath & Path.DirectorySeparatorChar & EMAILDATAFILE
      Console.WriteLine(String.Format(My.Resources.CheckingEmailDataFile, emldatafile))

      'Datenbank erstellen falls sie nicht existiert
      If Not File.Exists(emldatafile) Then
        CreateEmailDataFile(emldatafile)
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
        CalculateBleilochDifferences(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE)
      End If

      'Differenzwerte berechnen wenn neue Daten in Hohenwarte vorhanden sind
      If NewHohenwarteData Then
        CalculateHohenwarteDifferences(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE)
      End If

      '�berpr�fen ob neue Daten vorliegen
      If NewHohenwarteData = False AndAlso NewBleilochData = False Then
        'Beenden wenn keine neuen Daten vorhanden sind
        Exit Sub
      Else
        SendMails()
      End If

    End Sub

    Private Sub PrintMsg(Msg As String, File As String)
      Console.WriteLine(String.Format(My.Resources.ConfigFailMsg, File, Msg))

    End Sub

#Region "Hohenwartefunktionen"

    ''' <summary>
    ''' Berechnet die Differenzen der Pegelst�nde f�r Hohenwarte und aktualisiert die CSV-Datei.
    ''' </summary>
    ''' <param name="File">Der Pfad zur CSV-Datei, die die Hohenwarte-Pegeldaten enth�lt.</param>
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
          End If

          If index > 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then
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
    ''' F�gt einen neuen Datensatz in die Hohenwartepegeldaten ein
    ''' falls noch nicht vorhanden.
    ''' </summary>
    ''' <param name="File">Datendatei f�r Hohenwartedaten</param>
    Private Function AddHohenwartePegel(File As String) As Boolean

      Dim result As Boolean = False
      Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)

        'alle Datens�tze der Webdaten durchsuchen
        Using WebData As New WebSiteParser(URL)

          Dim index As Integer
          For Each row As String In WebData.GetHohenwarteData

            '�berpr�fen ob Datum bereits existiert und Datensatz eintragen falls nicht
            index = PegelDataFile.FindRecord(row.Split(";").First)
            If index = -1 Then
              PegelDataFile.AddRecord(row)
              result = True
            End If

          Next

        End Using

      End Using

      Return result

    End Function

    ''' <summary>
    ''' Ruft die Hohenwarte-Daten aus der angegebenen Datei ab und formatiert sie gem�� der angegebenen Vorlage.
    ''' </summary>
    ''' <param name="File">Der Pfad zur Datei, die die Hohenwarte-Daten enth�lt.</param>
    ''' <param name="Records">Die Anzahl der Datens�tze, die abgerufen werden sollen.</param>
    ''' <param name="Linetemplate">Die Vorlage, die zum Formatieren der Datens�tze verwendet wird.</param>
    ''' <returns>Eine formatierte Zeichenfolge, die die abgerufenen Hohenwarte-Daten enth�lt.</returns>
    Private Function GetHohenwarteData(File As String, Records As Integer, Linetemplate As String) As String

      Dim result As String = ""
      Dim record As String '= Linetemplate
      Dim data As New CsvFile(File, PEGELDATAHEADER)
      Dim length As Integer = data.Data.Count - 1
      Dim datum As String
      Dim pegel As String
      Dim diff As String

      'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
      If Records > length Then
        Records = length
      End If

      'Die letzten in Records gespeicherten Datens�tze durchlaufen
      For index As Integer = length + 1 - Records To length
        record = Linetemplate
        datum = data.Data.ElementAt(index).Split(";").ElementAt(0)
        pegel = data.Data.ElementAt(index).Split(";").ElementAt(1)
        diff = data.Data.ElementAt(index).Split(";").ElementAt(2)
        record = record.Replace("%DATUM%", datum)
        record = record.Replace("%PEGEL%", pegel)
        record = record.Replace("%DIFFERENZ%", diff)
        result &= record
      Next

      Return result

    End Function

    Private Function GetHohenwarteImageCode(File As String, Records As Integer) As String

      Dim imagedata As New List(Of String)
      Dim data As New CsvFile(File, PEGELDATAHEADER)
      Dim length As Integer = data.Data.Count - 1
      Dim datum As String
      Dim pegel As String

      'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
      If Records > length Then
        Records = length
      End If

      'Die letzten in Records gespeicherten Datens�tze durchlaufen
      For index As Integer = length + 1 - Records To length
        datum = data.Data.ElementAt(index).Split(";").ElementAt(0)
        pegel = data.Data.ElementAt(index).Split(";").ElementAt(1)
        imagedata.Add($"{datum};{pegel}")
      Next

      Dim datalines As String() = imagedata.ToArray
      Dim renderer As New ImageRenderer(datalines) With
      {.Height = 200, .Width = 300, .Padding = 20, .Caption = $"Talsperre Hohenwarte", .TextSize = 10}
      Dim imagecode = renderer.GenerateImageTag

      Return imagecode

    End Function

#End Region

#Region "Bleilochfunktionen"

    ''' <summary>
    ''' Berechnet die Differenzen der Pegelst�nde f�r Bleiloch und aktualisiert die CSV-Datei.
    ''' </summary>
    ''' <param name="File">Der Pfad zur CSV-Datei, die die Bleiloch-Pegeldaten enth�lt.</param>
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
          End If

          If index > 1 And tempdata.ElementAt(index).Split(";").Length = 2 Then
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
    ''' F�gt einen neuen Datensatz in die Bleilochpegeldaten ein
    ''' falls noch nicht vorhanden.
    ''' </summary>
    ''' <param name="File">Datendatei f�r Bleilochdaten</param>
    Private Function AddBleilochPegel(File As String) As Boolean

      Dim result As Boolean = False
      Using PegelDataFile As New CsvFile(File, PEGELDATAHEADER)

        'alle Datens�tze der Webdaten durchsuchen
        Using WebData As New WebSiteParser(URL)

          Dim index As Integer
          For Each row As String In WebData.GetBleilochData

            '�berpr�fen ob Datum bereits existiert und Datensatz eintragen falls nicht
            index = PegelDataFile.FindRecord(row.Split(";").First)
            If index = -1 Then
              PegelDataFile.AddRecord(row)
              result = True
            End If

          Next

        End Using

      End Using

      Return result

    End Function

    ''' <summary>
    ''' Ruft die Bleiloch-Daten aus der angegebenen Datei ab und formatiert sie gem�� der angegebenen Vorlage.
    ''' </summary>
    ''' <param name="File">Der Pfad zur Datei, die die Bleiloch-Daten enth�lt.</param>
    ''' <param name="Records">Die Anzahl der Datens�tze, die abgerufen werden sollen.</param>
    ''' <param name="Linetemplate">Die Vorlage, die zum Formatieren der Datens�tze verwendet wird.</param>
    ''' <returns>Eine formatierte Zeichenfolge, die die abgerufenen Bleiloch-Daten enth�lt.</returns>
    Private Function GetBleilochData(File As String, Records As Integer, Linetemplate As String) As String

      Dim result As String = ""
      Dim record As String '= Linetemplate
      Dim data As New CsvFile(File, PEGELDATAHEADER)
      Dim length As Integer = data.Data.Count - 1
      Dim datum As String
      Dim pegel As String
      Dim diff As String

      'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
      If Records > length Then
        Records = length
      End If

      'Die letzten in Records gespeicherten Datens�tze durchlaufen
      For index As Integer = length + 1 - Records To length
        record = Linetemplate
        datum = data.Data.ElementAt(index).Split(";").ElementAt(0)
        pegel = data.Data.ElementAt(index).Split(";").ElementAt(1)
        diff = data.Data.ElementAt(index).Split(";").ElementAt(2)
        record = record.Replace("%DATUM%", datum)
        record = record.Replace("%PEGEL%", pegel)
        record = record.Replace("%DIFFERENZ%", diff)
        result &= record
      Next

      Return result

    End Function

    Private Function GetBleilochImageCode(File As String, Records As Integer) As String

      Dim imagedata As New List(Of String)
      Dim data As New CsvFile(File, PEGELDATAHEADER)
      Dim length As Integer = data.Data.Count - 1
      Dim datum As String
      Dim pegel As String

      'maximale Anzahl der Datens�tze anpassen wenn weniger Daten vorhanden als gew�nscht
      If Records > length Then
        Records = length
      End If

      'Die letzten in Records gespeicherten Datens�tze durchlaufen
      For index As Integer = length + 1 - Records To length
        datum = data.Data.ElementAt(index).Split(";").ElementAt(0)
        pegel = data.Data.ElementAt(index).Split(";").ElementAt(1)
        imagedata.Add($"{datum};{pegel}")
      Next

      Dim datalines As String() = imagedata.ToArray
      Dim renderer As New ImageRenderer(datalines) With
      {.Height = 200, .Width = 300, .Padding = 20, .Caption = $"Talsperre Bleiloch", .TextSize = 10}
      Dim imagecode = renderer.GenerateImageTag

      Return imagecode

    End Function

#End Region

#Region "Emailfunktionen"

    ''' <summary>
    ''' Sendet eine E-Mail mit den angegebenen Daten.
    ''' </summary>
    ''' <param name="FirstName">Der Vorname des Empf�ngers.</param>
    ''' <param name="Email">Die E-Mail-Adresse des Empf�ngers.</param>
    ''' <param name="Modus">Der Modus, der bestimmt, welche Daten in die E-Mail eingef�gt werden.</param>
    ''' <param name="Records">Die Anzahl der Datens�tze, die in die E-Mail eingef�gt werden sollen.</param>
    Private Sub SendMail(FirstName As String, Email As String, Modus As Integer, Records As Integer)

      Dim doc As New EmailDocument 'Emailvorlage erstellen
      doc.SetName(FirstName) 'Anrede einsetzen
      doc.SetDataTable() 'Datentabelle einf�gen

      'Fusszeile komplettieren
      doc.SetAppName(Assembly.GetExecutingAssembly().GetName.Name)
      doc.SetAppVersion(Assembly.GetExecutingAssembly().GetName.Version.ToString)
      doc.SetAppCopy($"(Copyright � 2024 by Andreas Sauer)")

      'Vorlage je nach Modus anpassen und Daten eintragen
      Dim linetemplate As String = doc.DataLineTemplate
      Dim bleilochdata As String = GetBleilochData(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE, Records, linetemplate)
      Dim bleilochimagecode As String = GetBleilochImageCode(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE, Records)
      Dim hohenwartedata As String = GetHohenwarteData(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE, Records, linetemplate)
      Dim hohenwateimagecode As String = GetHohenwarteImageCode(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE, Records)

      Select Case Modus

        Case 0  'nur Hohenwartedaten eintragen
          'Leerzellen und Tabelle f�r Bleilochdaten entfernen 
          doc.RemovePlaceHolder($"%LEERZELLE%")
          doc.RemovePlaceHolder($"%BLEILOCHDATEN%")
          '
          doc.SetHohenwarteData()
          doc.FillData(hohenwartedata)
          doc.InsertHohenwarteImage(hohenwateimagecode)

        Case 1 'nur Bleilochdaten eintragen
          'Leerzellen und Tabelle f�r Hohenwartedaten entfernen 
          doc.RemovePlaceHolder($"%HOHENWARTEDATEN%")
          doc.RemovePlaceHolder($"%LEERZELLE%")
          '
          doc.SetBleilochData()
          doc.FillData(bleilochdata)
          doc.InsertBleilochImage(bleilochimagecode)

        Case 2 'beide Daten eintragen
          doc.SetHohenwarteData()
          doc.FillData(hohenwartedata)
          doc.InsertHohenwarteImage(hohenwateimagecode)

          doc.SetBlankCell()
          doc.SetBleilochData()
          doc.FillData(bleilochdata)
          doc.InsertBleilochImage(bleilochimagecode)

        Case Else
          'Fehler -> Ende
          Exit Sub

      End Select

      Dim mail As New EmailClient(Server, CInt(Port), User, Passw)
      mail.Send(Absender, "pegelmelder", "Pegeldaten", Email, doc.DocumentText)

    End Sub

    ''' <summary>
    ''' Emails an alle Empf�nger senden.
    ''' Diese Methode l�dt die E-Mail-Daten aus der CSV-Datei, 
    ''' durchl�uft alle Empf�nger und sendet die entsprechenden E-Mails.
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
        SendMail(vname, email, modus, records)
      Next

    End Sub

    ''' <summary>
    ''' L�dt die E-Mail-Daten aus der CSV-Datei.
    ''' </summary>
    ''' <returns>Eine Liste von E-Mail-Daten als Zeichenfolgen.</returns>
    Private Function LoadEmailData() As List(Of String)

      Dim datafilepath As String = ConfigFilePath & Path.DirectorySeparatorChar & EMAILDATAFILE
      Dim DataFile As New CsvFile(datafilepath, EMAILDATAHEADER)
      Return DataFile.Data

    End Function

    Private Sub CreateEmailDataFile(emldatafile As String)
      Console.WriteLine(String.Format(My.Resources.EmailDataFileNotFound, emldatafile))
      Dim builder As New StringBuilder
      Dim unused = builder.AppendLine(My.Resources.EmaildatenTemplate)
      File.WriteAllText(emldatafile, builder.ToString)
      Console.WriteLine(My.Resources.NewEmailDataFileCreated)
      Console.WriteLine(My.Resources.InsertEmailData)
    End Sub

#End Region

#Region "Konfiguratoionsfunktionen"

    ''' <summary>
    ''' Initialisiert die Pfadvariable f�r Konfiguration und erstellt einen Ordner falls er nicht existiert.
    ''' </summary>
    ''' <remarks>
    ''' Diese Methode legt den Pfad f�r die Konfigurationsdateien fest, basierend auf der Umgebungsvariable "HOME".
    ''' Wenn der Ordner f�r die Konfigurationsdateien nicht existiert, wird er erstellt.
    ''' </remarks>
    Private Sub SetConfigPath()

      'Ordner zur Konfiguration festlegen
      ConfigFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & CONFIGPATH

      'Ordner f�r Konfigurationsdateien erstellen falls er nicht existiert
      If Not Directory.Exists(ConfigFilePath) Then
        Dim unused = Directory.CreateDirectory(ConfigFilePath)
      End If

    End Sub

    ''' <summary>
    ''' Initialisiert die Pfadvariabel f�r Daten und erstellt 
    ''' einen Ordner falls er nicht existiert.
    ''' </summary>
    ''' <remarks>
    ''' Diese Methode legt den Pfad f�r die Datendateien fest, basierend auf der Umgebungsvariable "HOME".
    ''' Wenn der Ordner f�r die Datendateien nicht existiert, wird er erstellt.
    ''' </remarks>
    Private Sub SetDataPath()

      'Ordner zur Datenspeicherung festlegen
      DataFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & DATAPATH

      'Ordner f�r Datenspeicherung erstellen falls er nicht existiert
      If Not Directory.Exists(DataFilePath) Then
        Dim unused = Directory.CreateDirectory(DataFilePath)
      End If

    End Sub

    ''' <summary>
    ''' L�dt die Konfigurationsdaten aus der angegebenen Datei.
    ''' Diese Methode liest die Konfigurationsdatei zeilenweise ein und weist die Werte den entsprechenden Variablen zu.
    ''' </summary>
    ''' <param name="file">Der Pfad zur Konfigurationsdatei.</param>
    Private Sub LoadConfig(file As String)

      For Each line As String In IO.File.ReadAllLines(file)

        Select Case True
          Case line.StartsWith("Server:")
            Server = line.Split(":").Last

          Case line.StartsWith("User:")
            User = line.Split(":").Last

          Case line.StartsWith("Passwort:")
            Passw = line.Split(":").Last

          Case line.StartsWith("Port:")
            Port = line.Split(":").Last

          Case line.StartsWith("Absender:")
            Absender = line.Split(":").Last

          Case Else
            Exit Select

        End Select

      Next

    End Sub

    Private Sub CreateConfigFile(conffile As String)
      Console.WriteLine(String.Format(My.Resources.ConfigFileNotFound, conffile))
      Dim builder As New StringBuilder
      Dim unused = builder.AppendLine(My.Resources.ConfigFileTemplate)
      File.WriteAllText(conffile, builder.ToString)
      Console.WriteLine(My.Resources.NewConfigFileCreated)
      Console.WriteLine(My.Resources.InsertConfigFileData)
    End Sub

#End Region

  End Module

End Namespace

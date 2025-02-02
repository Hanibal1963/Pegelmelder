' ****************************************************************************************************************
' MailFunctions.vb
' © 2025 by Andreas Sauer
' ****************************************************************************************************************
'

Imports System.IO
Imports System.Reflection
Imports SchlumpfSoft.CsvFileManager
Imports SchlumpfSoft.EmailDocument
Imports SchlumpfSoft.MailClient

Module MailFunctions

  ''' <summary>
  ''' Sendet eine E-Mail mit den angegebenen Daten.
  ''' </summary>
  ''' <param name="FirstName">Der Vorname des Empfängers.</param>
  ''' <param name="Email">Die E-Mail-Adresse des Empfängers.</param>
  ''' <param name="Modus">Der Modus, der bestimmt, welche Daten in die E-Mail eingefügt werden.</param>
  ''' <param name="Records">Die Anzahl der Datensätze, die in die E-Mail eingefügt werden sollen.</param>
  Friend Sub SendMail(FirstName As String, Email As String, Modus As Integer, Records As Integer)

    Dim doc As New Document 'Emailvorlage erstellen
    Document.SetName(FirstName) 'Anrede einsetzen
    doc.SetDataTable() 'Datentabelle einfügen

    'Fusszeile komplettieren
    Document.SetAppName(Assembly.GetExecutingAssembly().GetName.Name)
    Document.SetAppVersion(Assembly.GetExecutingAssembly().GetName.Version.ToString)
    Document.SetAppCopy($"(Copyright © 2024 by Andreas Sauer)")

    'Vorlage je nach Modus anpassen und Daten eintragen
    Dim linetemplate As String = Document.DataLineTemplate
    Dim bleilochdata As String = GetBleilochData(DataFilePath & Path.DirectorySeparatorChar & BLEILOCHDATAFILE, Records, linetemplate)
    Dim hohenwartedata As String = GetHohenwarteData(DataFilePath & Path.DirectorySeparatorChar & HOHENWARTEDATAFILE, Records, linetemplate)

    Select Case Modus

      Case 0  'nur Hohenwartedaten eintragen
        Document.RemovePlaceHolder($"%LEERZELLE%")
        Document.RemovePlaceHolder($"%BLEILOCHDATEN%")
        doc.SetHohenwarteData()
        Document.FillData(hohenwartedata)

      Case 1 'nur Bleilochdaten eintragen
        Document.RemovePlaceHolder($"%HOHENWARTEDATEN%")
        Document.RemovePlaceHolder($"%LEERZELLE%")
        doc.SetBleilochData()
        Document.FillData(bleilochdata)

      Case 2 'beide Daten eintragen
        doc.SetHohenwarteData()
        Document.FillData(hohenwartedata)
        doc.SetBlankCell()
        doc.SetBleilochData()
        Document.FillData(bleilochdata)

      Case Else
        'Fehler -> Ende
        Exit Sub

    End Select

    Dim mail As New Client(Server, CInt(Port), User, Passw)
    mail.Send(Absender, "pegelmelder", "Pegeldaten", Email, Document.DocumentText)

  End Sub

  ''' <summary>
  ''' Emails an alle Empfänger senden.
  ''' Diese Methode lädt die E-Mail-Daten aus der CSV-Datei, 
  ''' durchläuft alle Empfänger und sendet die entsprechenden E-Mails.
  ''' </summary>
  Friend Sub SendMails()

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
  ''' Lädt die E-Mail-Daten aus der CSV-Datei.
  ''' </summary>
  ''' <returns>Eine Liste von E-Mail-Daten als Zeichenfolgen.</returns>
  Friend Function LoadEmailData() As List(Of String)

    Dim datafilepath As String = ConfigFilePath & Path.DirectorySeparatorChar & EMAILDATAFILE
    Dim DataFile As New CsvFile(datafilepath, EMAILDATAHEADER)
    Return CsvFile.Data

  End Function

End Module

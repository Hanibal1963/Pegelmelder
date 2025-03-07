' Copyright (c)2022-2025 by Andreas Sauer 

Imports System.Net
Imports System.Net.Mail

Namespace Pegelmelder

	''' <summary>
	''' Klasse zum versenden von Emails
	''' </summary>
	Public Class EmailClient

		' Diese Klasse stellt Methoden zur Verfügung, um E-Mails über einen SMTP-Server zu versenden. Sie ermöglicht die 
		' Konfiguration des SMTP-Servers und das Senden von E-Mails mit verschiedenen Parametern wie Absenderadresse, 
		' Empfängeradresse, Betreff und Inhalt.

		' Definiert einen SMTP-Client für den Versand von E-Mails
		Private ReadOnly smtpsrv As New SmtpClient

		''' <summary>
		''' Erstellt einen neuen EmailClient
		''' </summary>
		''' <param name="Host">Hostname oder IP des SMTP-Servers</param>
		''' <param name="Port">Anschlussnummer</param>
		''' <param name="User">Anmeldename des SMTP-Servers</param>
		''' <param name="Passwd">Anmeldepasswort des SMTP-Servers</param>
		''' <param name="SSl">Verschlüsselung</param>
		Public Sub New(Host As String, Port As Integer, User As String, Passwd As String, SSl As Boolean)
			With Me.smtpsrv
				.Host = Host
				.Port = Port
				.UseDefaultCredentials = False
				.EnableSsl = SSl
				.Credentials = New NetworkCredential With {.UserName = User, .Password = Passwd}
			End With
		End Sub

		''' <summary>
		''' Erstellt einen neuen EmailClient
		''' </summary>
		''' <param name="Host">Hostname oder IP des SMTP-Servers</param>
		''' <param name="Port">Anschlussnummer</param>
		''' <param name="User">Anmeldename des SMTP-Servers</param>
		''' <param name="Passwd">Anmeldepasswort des SMTP-Servers</param>
		''' <remarks>Der Client wird mit aktivierter SSL-Verschlüsselung erstellt.</remarks>
		Public Sub New(Host As String, Port As Integer, User As String, Passwd As String)
			Me.New(Host, Port, User, Passwd, True)
		End Sub

		''' <summary>
		''' Sendet die erstellte Email
		''' </summary>
		''' <param name="From">Absenderadresse der Email</param>
		''' <param name="Name">Anzeigename des Absenders</param>
		''' <param name="Subject">Betreff der Email</param>
		''' <param name="[To]">Empfängeradresse der Email</param>
		''' <param name="Msg">Inhalt der Email (Text oder Htmlquelltext)</param>
		''' <param name="Html">False für "Nur Text", True für HTML (Quelltext)</param>
		Public Sub Send(From As String, Name As String, Subject As String, [To] As String, Msg As String, Html As Boolean)
			Dim mail As New MailMessage
			With mail
				.From = New MailAddress(From, Name)
				.Subject = Subject
				.Body = Msg
				.IsBodyHtml = Html
				.To.Add([To])
			End With
			Try
				Me.smtpsrv.Send(mail)
			Catch ex As Exception
				Console.WriteLine(String.Format(My.Resources.SendMailErrorMsg, ex.ToString))
			End Try
		End Sub

		''' <summary>
		''' Sendet die erstellte Email
		''' </summary>
		''' <param name="From">Absenderadresse der Email</param>
		''' <param name="Name">Anzeigename des Absenders</param>
		''' <param name="Subject">Betreff der Email</param>
		''' <param name="[To]">Empfängeradresse der Email</param>
		''' <param name="Msg">Inhalt der Email (Text oder Htmlquelltext)</param>
		''' <remarks>Die Email wird im Html-Format gesendet.</remarks>
		Public Sub Send(From As String, Name As String, Subject As String, [To] As String, Msg As String)
			Me.Send(From, Name, Subject, [To], Msg, True)
		End Sub

	End Class

End Namespace

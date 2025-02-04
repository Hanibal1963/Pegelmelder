'
'****************************************************************************************************************
'Parser.vb
'(c) 2022 - 2024 by Andreas Sauer
'
' Diese Klasse stellt Methoden zum Parsen von Websitedaten zur Verfügung.
' Sie implementiert das IDisposable-Interface, um Ressourcen freizugeben.
'
'****************************************************************************************************************
'

Public Class Parser

  Implements IDisposable

  Public Sub New(Weburl As String)
    Docsource = HelperFunctions.GetWebSource(Weburl).Result 'Quelltext der Website runterladen
  End Sub

  ''' <summary>
  ''' Liest die Daten der Talsperre Hohenwarte aus dem Quelltext der Website
  ''' </summary>
  Public Shared Function GetHohenwarteData() As List(Of String)
    Dim result As New List(Of String)
    Dim data As String
    For Each datarow As String In HelperFunctions.GetDataSource(Docsource, HOHENWARTE)
      'Datenzeile in Werte splitten und Datum als Langdatum und Pegel in mm übernehmen
      data = HelperFunctions.CreateData(datarow)
      result.Add(data)
    Next
    Return result
  End Function

  ''' <summary>
  ''' Liest die Daten der Talsperre Bleiloch aus dem Quelltext der Website
  ''' </summary>
  ''' <returns></returns>
  Public Shared Function GetBleilochData() As List(Of String)
    Dim result As New List(Of String)
    Dim data As String
    For Each datarow As String In HelperFunctions.GetDataSource(Docsource, BLEILOCH)
      'Datenzeile in Werte splitten und Datum als Langdatum und Pegel in mm übernehmen
      data = HelperFunctions.CreateData(datarow)
      result.Add(data)
    Next
    Return result
  End Function

  ' ' TODO: Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
  ' Protected Overrides Sub Finalize()
  '     ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
  '     Dispose(disposing:=False)
  '     MyBase.Finalize()
  ' End Sub

  ''' <summary>
  ''' Gibt die verwendeten Ressourcen frei
  ''' </summary>
  Public Sub Dispose() Implements IDisposable.Dispose
    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
    Me.Dispose(disposing:=True)
    GC.SuppressFinalize(Me)
  End Sub

  Protected Overridable Sub Dispose(disposing As Boolean)
    If Not DisposedValue Then
      If disposing Then
        ' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
      End If
      ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
      ' TODO: Große Felder auf NULL setzen
      DisposedValue = True
    End If

  End Sub

End Class

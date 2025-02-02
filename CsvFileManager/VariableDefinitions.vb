' ****************************************************************************************************************
' VariableDefinitions.vb
' © 2025 by Andreas Sauer
'
' Dieses Modul definiert die globalen Variablen, die in der Anwendung verwendet werden.
' - DisposedValue: Gibt an, ob das Objekt verworfen wurde.
' - Contents: Ein Array von Zeichenfolgen, das den Inhalt der Datei speichert.
' - DataList: Eine Liste von Zeichenfolgen, die die Daten speichert.
' - FileName: Der Name der Datei.
' - FileHeader: Der Header der Datei.
'
' ****************************************************************************************************************

''' <summary>
''' Dieses Modul definiert die globalen Variablen, die in der Anwendung verwendet werden.
''' </summary>
Module VariableDefinitions

  ''' <summary>
  ''' Gibt an, ob das Objekt verworfen wurde.
  ''' </summary>
  Friend DisposedValue As Boolean

  ''' <summary>
  ''' Ein Array von Zeichenfolgen, das den Inhalt der Datei speichert.
  ''' </summary>
  Friend Contents As String()

  ''' <summary>
  ''' Eine Liste von Zeichenfolgen, die die Daten speichert.
  ''' </summary>
  Friend DataList As New List(Of String)

  ''' <summary>
  ''' Der Name der Datei.
  ''' </summary>
  Friend FileName As String

  ''' <summary>
  ''' Der Header der Datei.
  ''' </summary>
  Friend FileHeader As String

End Module

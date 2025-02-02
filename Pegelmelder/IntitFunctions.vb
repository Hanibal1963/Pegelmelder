' ****************************************************************************************************************
' InitFunctions.vb
' © 2025 by Andreas Sauer
' ****************************************************************************************************************
'

Imports System.IO

Module IntitFunctions

  ''' <summary>
  ''' Initialisiert die Pfadvariable für Konfiguration und erstellt einen Ordner falls er nicht existiert.
  ''' </summary>
  ''' <remarks>
  ''' Diese Methode legt den Pfad für die Konfigurationsdateien fest, basierend auf der Umgebungsvariable "HOME".
  ''' Wenn der Ordner für die Konfigurationsdateien nicht existiert, wird er erstellt.
  ''' </remarks>
  Friend Sub SetConfigPath()

    'Ordner zur Konfiguration festlegen
    ConfigFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & CONFIGPATH

    'Ordner für Konfigurationsdateien erstellen falls er nicht existiert
    If Not Directory.Exists(ConfigFilePath) Then
      Dim unused = Directory.CreateDirectory(ConfigFilePath)
    End If

  End Sub

  ''' <summary>
  ''' Initialisiert die Pfadvariabel für Daten und erstellt 
  ''' einen Ordner falls er nicht existiert.
  ''' </summary>
  ''' <remarks>
  ''' Diese Methode legt den Pfad für die Datendateien fest, basierend auf der Umgebungsvariable "HOME".
  ''' Wenn der Ordner für die Datendateien nicht existiert, wird er erstellt.
  ''' </remarks>
  Friend Sub SetDataPath()

    'Ordner zur Datenspeicherung festlegen
    DataFilePath = Environment.GetEnvironmentVariable("HOME") & Path.AltDirectorySeparatorChar & DATAPATH

    'Ordner für Datenspeicherung erstellen falls er nicht existiert
    If Not Directory.Exists(DataFilePath) Then
      Dim unused = Directory.CreateDirectory(DataFilePath)
    End If

  End Sub

  ''' <summary>
  ''' Lädt die Konfigurationsdaten aus der angegebenen Datei.
  ''' Diese Methode liest die Konfigurationsdatei zeilenweise ein und weist die Werte den entsprechenden Variablen zu.
  ''' </summary>
  ''' <param name="file">Der Pfad zur Konfigurationsdatei.</param>
  Friend Sub LoadConfig(file As String)

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

      End Select

    Next

  End Sub
End Module

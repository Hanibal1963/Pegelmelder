# Pegelmelder

## Einf�hrung
Pegelmelder ist ein VB.NET-Projekt, das f�r den **RaspberryPi** entwickelt wurde, um die Pegelst�nde der Talsperren Bleiloch und Hohenwarte zu �berwachen und die Daten in CSV-Dateien zu speichern. Zus�tzlich versendet das Programm E-Mails mit den aktuellen Pegeldaten an eine Liste von Empf�ngern.

## Features
- Herunterladen von Pegeldaten von einer Website
- Speichern der Pegeldaten in CSV-Dateien
- Berechnung der Differenzen der Pegelst�nde
- Versenden von E-Mails mit den aktuellen Pegeldaten
- Verwaltung der E-Mail-Empf�nger

## Klassen

### CsvFile
Die `CsvFile`-Klasse verwaltet CSV-Dateien. Sie kann Daten aus CSV-Dateien lesen, neue Datens�tze hinzuf�gen, vorhandene Datens�tze ersetzen und l�schen.

### Document
Die `Document`-Klasse erstellt E-Mail-Dokumente mit Platzhaltern, die durch aktuelle Daten ersetzt werden.

### Client
Die `Client`-Klasse sendet E-Mails �ber einen konfigurierten SMTP-Server.

### Parser
Die `Parser`-Klasse l�dt den HTML-Quelltext einer Website herunter und extrahiert die Pegeldaten der Talsperren Bleiloch und Hohenwarte.

## Hauptprogramm
Das Hauptprogramm (`Program.vb`) f�hrt die folgenden Schritte aus:
1. Initialisiert die Pfade f�r Konfigurations- und Datendateien.
2. �berpr�ft und l�dt die Konfigurationsdatei.
3. �berpr�ft das Vorhandensein der E-Mail-Datenbank und erstellt sie bei Bedarf.
4. L�dt und speichert neue Pegeldaten.
5. Berechnet die Differenzen der Pegelst�nde.
6. Sendet E-Mails mit den aktuellen Pegeldaten an die Empf�nger.

## Installation und Nutzung
1. Stelle sicher, dass die erforderlichen .NET-Abh�ngigkeiten installiert sind.
2. Klone das Repository: `git clone https://github.com/Hanibal1963/Pegelmelder.git`
3. �ffne das Projekt in Visual Studio.
4. Erstelle das Projekt und ver�ffentliche die Ausgabe auf deinem **RaspberryPi** (`/home/pi/.bin/pegelmelder`).
5. Konfiguriere die `home/pi/.config/pm/pm.conf`-Datei mit den SMTP-Serverdetails deines Email-Servers.
6. Konfiguriere die Datendatei `home/pi/.config/pm/Emaildaten.csv` mit den Daten der Emailempf�nger.
7. F�hre das Programm aus, um die Pegeldaten herunterzuladen und E-Mails zu versenden (`home/pi/.bin/pegelmelder/pm`).
8. Die Datendateien findest du im Verzeichnis `home/pi/.local/pm`.

## Konfigurationsdatei
Die `home/pi/.config/pm/pm.conf`-Datei enth�lt die folgenden Einstellungen:
- **Server:** Die Adresse des SMTP-Servers
- **User:** Der Benutzername f�r den SMTP-Server
- **Passwort:** Das Passwort f�r den SMTP-Server
- **Port:** Der Port f�r den SMTP-Server
- **Absender:** Die Absender-E-Mail-Adresse

## Datenbanken
Das Projekt verwendet mehrere CSV-Dateien zur Speicherung von Daten:
- `Bleilochpegeldaten.csv`: Pegeldaten der Talsperre Bleiloch
- `Hohenwartepegeldaten.csv`: Pegeldaten der Talsperre Hohenwarte
- `Emaildaten.csv`: Informationen �ber die E-Mail-Empf�nger

## Lizenz
Dieses Projekt steht unter der MIT-Lizenz. Weitere Informationen findest du in der `LICENSE`-Datei.

## Autor
Das Projekt wurde von Andreas Sauer entwickelt. (c) 2022 - 2024

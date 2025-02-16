## Pegelmelder

### Einführung

Pegelmelder ist ein VB.NET-Befehlszeilenprogramm, das für den **RaspberryPi** entwickelt wurde, um die Pegelstände der Talsperren Bleiloch und Hohenwarte zu überwachen und die Daten in CSV-Dateien zu speichern. 

Zusätzlich versendet das Programm E-Mails mit den aktuellen Pegeldaten an eine Liste von Empfängern.

### Features

- Herunterladen von Pegeldaten von einer Website
- Speichern der Pegeldaten in CSV-Dateien
- Berechnung der Differenzen der Pegelstände
- Versenden von E-Mails mit den aktuellen Pegeldaten
- Verwaltung der E-Mail-Empfänger


### Das Projekt ...

... umfasst folgende Dateien:

- Die `CsvFile`-Klasse verwaltet CSV-Dateien. Sie kann Daten aus CSV-Dateien lesen, neue Datensätze hinzufügen, vorhandene Datensätze ersetzen und löschen.
- Die `EmailDocument`-Klasse erstellt E-Mail-Dokumente mit Platzhaltern, die durch aktuelle Daten ersetzt werden.
- Die `EmailClient`-Klasse sendet E-Mails über einen konfigurierten SMTP-Server.
- Die `WebSiteParser`-Klasse lädt den HTML-Quelltext einer Website herunter und extrahiert die Pegeldaten der Talsperren Bleiloch und Hohenwarte.
- Die `ImageRenderer`-Klasse erstellt eine Diagramgrafik aus den übergebenen Daten.

Das Hauptprogramm (`Program.vb`) führt die folgenden Schritte aus:

1. Initialisiert die Pfade für Konfigurations- und Datendateien.
2. Überprüft und lädt die Konfigurationsdatei.
3. Überprüft das Vorhandensein der E-Mail-Datenbank und erstellt sie bei Bedarf.
4. Lädt und speichert neue Pegeldaten.
5. Berechnet die Differenzen der Pegelstände.
6. Erstellt ein E-Mail-Dokument aus den Daten und fügt am Ende der datentabelle jeweils eine Diagrammgrafik an.
7. Sendet E-Mails an die Empfänger.

### Installation und Nutzung

1. Stelle sicher, dass die erforderlichen .NET-Abhängigkeiten installiert sind.
2. Klone das Repository: `git clone https://github.com/Hanibal1963/Pegelmelder.git`
3. Öffne das Projekt in Visual Studio.
4. Erstelle das Projekt und veröffentliche die Ausgabe auf deinem **RaspberryPi** (`/home/pi/pegelmelder`).
5. Konfiguriere die `home/pi/.config/pm/pm.conf`-Datei mit den SMTP-Serverdetails deines Email-Servers.
6. Konfiguriere die Datendatei `home/pi/.config/pm/Emaildaten.csv` mit den Daten der Emailempfänger.
7. Führe das Programm aus, um die Pegeldaten herunterzuladen und E-Mails zu versenden (`home/pi/pegelmelder/pm`).
8. Die Datendateien findest du im Verzeichnis `home/pi/.local/share/pm`.

### Konfigurationsdatei

Die `home/pi/.config/pm/pm.conf`-Datei enthält die folgenden Einstellungen:

- **Server:** Die Adresse des SMTP-Servers
- **User:** Der Benutzername für den SMTP-Server
- **Passwort:** Das Passwort für den SMTP-Server
- **Port:** Der Port für den SMTP-Server
- **Absender:** Die Absender-E-Mail-Adresse

### Datenbanken

Das Projekt verwendet mehrere CSV-Dateien zur Speicherung von Daten:

- `Bleilochpegeldaten.csv`: Pegeldaten der Talsperre Bleiloch
- `Hohenwartepegeldaten.csv`: Pegeldaten der Talsperre Hohenwarte
- `Emaildaten.csv`: Informationen über die E-Mail-Empfänger

### geplante Erweiterungen

- Verwaltungstool für die Emaildaten
- Verwaltungstool für die Emailserverdaten
- ~~Einfügen einer Diagrammgrafik unterhalb der Datentabellen~~

### Lizenz

Dieses Projekt steht unter der MIT-Lizenz. Weitere Informationen findest du in der `LICENSE.txt`-Datei.

### Autor

Das Projekt wurde von Andreas Sauer entwickelt. (c) 2022 - 2025

Teile der projekte wurden mit Hilfe von [GitHub CoPilot](https://docs.github.com/de/copilot) erstellt.

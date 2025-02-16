﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Dieser Code wurde von einem Tool generiert.
'     Laufzeitversion:4.0.30319.42000
'
'     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
'     der Code erneut generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    '-Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    'Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    'mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    '''<summary>
    '''  Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("SchlumpfSoft.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        '''  Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;td id=&quot;Bleilochdaten&quot;&gt;
        '''  &lt;b&gt;Pegeldaten Bleiloch:&lt;/b&gt;
        '''  &lt;br /&gt;
        '''   &lt;table class=&quot;table&quot; id=&quot;Bleilochdatentabelle&quot;&gt;
        '''     &lt;tr&gt;
        '''       &lt;td class=&quot;celldate&quot; name=&quot;Datum&quot;&gt;Datum&lt;/td&gt;
        '''       &lt;td class=&quot;cellpegel&quot; name=&quot;Pegel&quot;&gt;Pegel (mm)&lt;/td&gt;
        '''       &lt;td class=&quot;celldifferenz&quot; name=&quot;Aenderung&quot;&gt;Änderung zum Vortag (mm)&lt;/td&gt;
        '''     &lt;/tr&gt;
        '''     %DATENZEILEN%
        '''   &lt;/table&gt;
        '''   &lt;br /&gt;
        '''   %BLEILOCHIMAGE%
        '''&lt;/td&gt;
        ''' ähnelt.
        '''</summary>
        Friend ReadOnly Property BleilochDatenTabellenTemplate() As String
            Get
                Return ResourceManager.GetString("BleilochDatenTabellenTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Prüfe ob Konfigurationsdatei existiert (&quot;{0}&quot;) ähnelt.
        '''</summary>
        Friend ReadOnly Property CheckingConfigFile() As String
            Get
                Return ResourceManager.GetString("CheckingConfigFile", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Prüfe ob Datenbank für Emaildaten existiert (&quot;{0}&quot;) ähnelt.
        '''</summary>
        Friend ReadOnly Property CheckingEmailDataFile() As String
            Get
                Return ResourceManager.GetString("CheckingEmailDataFile", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Die Konfigurationsdatei (&quot;{0}&quot;) ist fehlerhaft.
        '''Bitte trage die korrekten Daten in diese Datei ein. ähnelt.
        '''</summary>
        Friend ReadOnly Property ConfigFailMsg() As String
            Get
                Return ResourceManager.GetString("ConfigFailMsg", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Die Konfigurationsdatei (&quot;{0}&quot;) wurde nicht gefunden. ähnelt.
        '''</summary>
        Friend ReadOnly Property ConfigFileNotFound() As String
            Get
                Return ResourceManager.GetString("ConfigFileNotFound", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die #Email-Konfiguration
        '''Server:
        '''User:
        '''Passwort:
        '''Port:
        '''Absender:
        ''' ähnelt.
        '''</summary>
        Friend ReadOnly Property ConfigFileTemplate() As String
            Get
                Return ResourceManager.GetString("ConfigFileTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;table&gt;
        '''  &lt;tr id=&quot;Daten&quot;&gt;
        '''    %HOHENWARTEDATEN%
        '''    %LEERZELLE%
        '''    %BLEILOCHDATEN%
        '''  &lt;/tr&gt;
        '''&lt;/table&gt;
        ''' ähnelt.
        '''</summary>
        Friend ReadOnly Property DatenTableTemplate() As String
            Get
                Return ResourceManager.GetString("DatenTableTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;tr&gt;
        '''  &lt;td class=&quot;celldate&quot;&gt;%DATUM%&lt;/td&gt;
        '''  &lt;td class=&quot;cellpegel&quot;&gt;%PEGEL%&lt;/td&gt;
        '''  &lt;td class=&quot;celldifferenz&quot;&gt;%DIFFERENZ%&lt;/td&gt;
        '''&lt;/tr&gt;
        ''' ähnelt.
        '''</summary>
        Friend ReadOnly Property DatenZeilenTemplate() As String
            Get
                Return ResourceManager.GetString("DatenZeilenTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Die Datenbank für Emaildaten (&quot;{0}&quot;) existiert noch nicht. ähnelt.
        '''</summary>
        Friend ReadOnly Property EmailDataFileNotFound() As String
            Get
                Return ResourceManager.GetString("EmailDataFileNotFound", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Vorname;Name;Emailadresse;Modus;Datensätze ähnelt.
        '''</summary>
        Friend ReadOnly Property EmaildatenTemplate() As String
            Get
                Return ResourceManager.GetString("EmaildatenTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;!DOCTYPE html&gt;
        '''&lt;html lang=&quot;de&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        '''&lt;head&gt;
        '''    &lt;meta charset=&quot;utf-8&quot; /&gt;
        '''    &lt;style&gt;
        '''        .document
        '''        {
        '''            font-size: 14px
        '''        }
        '''        .table
        '''        {
        '''            border-collapse: collapse; 
        '''            border-spacing: 0px
        '''        }
        '''        .celldate
        '''        {
        '''            border: 1px solid #000000; 
        '''            width: 70px
        '''        }
        '''        .cellpegel 
        '''        {
        '''            border: 1px solid #000000; 
        '''            width: 100px
        '''  [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property EmailTemplate() As String
            Get
                Return ResourceManager.GetString("EmailTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;td id=&quot;Hohenwartedaten&quot;&gt;
        '''  &lt;b&gt;Pegeldaten Hohenwarte:&lt;/b&gt;
        '''  &lt;br /&gt;
        '''  &lt;table class=&quot;table&quot; id=&quot;Hohenwartedatentabelle&quot;&gt;
        '''    &lt;tr&gt;
        '''      &lt;td class=&quot;celldate&quot; name=&quot;Datum&quot;&gt;Datum&lt;/td&gt;
        '''      &lt;td class=&quot;cellpegel&quot; name=&quot;Pegel&quot;&gt;Pegel (mm)&lt;/td&gt;
        '''      &lt;td class=&quot;celldifferenz&quot; name=&quot;Aenderung&quot;&gt;Änderung zum Vortag (mm)&lt;/td&gt;
        '''    &lt;/tr&gt;
        '''    %DATENZEILEN%
        '''  &lt;/table&gt;
        '''  &lt;br /&gt;
        '''  %HOHENWARTEIMAGE%
        '''&lt;/td&gt;
        ''' ähnelt.
        '''</summary>
        Friend ReadOnly Property HohenwarteDatenTabellenTemplate() As String
            Get
                Return ResourceManager.GetString("HohenwarteDatenTabellenTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Bitte trage deine Daten in diese Datei ein. ähnelt.
        '''</summary>
        Friend ReadOnly Property InsertConfigFileData() As String
            Get
                Return ResourceManager.GetString("InsertConfigFileData", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die In diese Datenbank m�ssen noch die Empfänger der Emails eingetragen werden. ähnelt.
        '''</summary>
        Friend ReadOnly Property InsertEmailData() As String
            Get
                Return ResourceManager.GetString("InsertEmailData", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;td id=&quot;Leerzelle&quot; style=&quot;width: 50px&quot;&gt;&lt;/td&gt; ähnelt.
        '''</summary>
        Friend ReadOnly Property LeerzellenTemplate() As String
            Get
                Return ResourceManager.GetString("LeerzellenTemplate", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Es wurde eine neue leere Datei erstellt. ähnelt.
        '''</summary>
        Friend ReadOnly Property NewConfigFileCreated() As String
            Get
                Return ResourceManager.GetString("NewConfigFileCreated", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Es wurde eine neue leere Datenbank erstellt. ähnelt.
        '''</summary>
        Friend ReadOnly Property NewEmailDataFileCreated() As String
            Get
                Return ResourceManager.GetString("NewEmailDataFileCreated", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &quot;{0}&quot; kann nicht NULL oder leer sein. ähnelt.
        '''</summary>
        Friend ReadOnly Property NullOrEmtyMessage() As String
            Get
                Return ResourceManager.GetString("NullOrEmtyMessage", resourceCulture)
            End Get
        End Property
    End Module
End Namespace

<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>

<table border="1" bordercolor="#000000" width="100%" cellpadding="2">
	<tr>
		<td colspan="3" align="middle"><strong>Allgemeine Werkzeuge</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Entwurfsansicht" alt="Entwurfsansicht" src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td>Entwurfsansicht - Schaltet eWebEdit400 in die Entwurfsansicht.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="HTML-Ansicht" alt="HTML-Ansicht" src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td>HTML-Ansicht - Schaltet eWebEdit400 in die HTML-Ansicht.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Vorschau" alt="Vorschau" src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Vorschau - Schaltet eWebEdit400 in den Vorschaumodus.</td>
		<td>-</td>
	</tr>
	
	<tr>
		<td align="middle"><img title="Wandelt zu Großbuchstaben" alt="Wandelt zu Großbuchstaben" src="<%= this.SkinPath %>Buttons/ConvertToUpper.gif"></td>
		<td>Wandelt den ausgewählten Text zu Großbuchstaben. Elemente wie Bilder und Tabellen werden nicht verändert.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Wandelt zu Kleinbuchstaben" alt="Wandelt zu Kleinbuchstaben" src="<%= this.SkinPath %>Buttons/ConvertToLower.gif"></td>
		<td>Wandelt den ausgewählten Text zu Kleinbuchstaben. Elemente wie Bilder und Tabellen werden nicht verändert.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Imagemaps erstellen" alt="Imagemaps erstellen" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Das Werkzeug erlaubt dem Benutzer Imagemaps zu Erstellen (Bilder mit anklickbaren Bereiche).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Modify Images" alt="Modify Images" src="<%= this.SkinPath %>Buttons/EktronImageModificationTool.gif"></td>
		<td>Modify images with the Image Tool.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Erlaubt dem Benutzer Quellcode-Abschnitte zu Formatieren und Einzufügen" alt="Erlaubt dem Benutzer Quellcode-Abschnitte zu Formatieren und Einzufügen" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Erlaubt dem Benutzer Quellcode-Abschnitte zu Formatieren und Einzufügen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Schriftgröße" alt="Schriftgröße" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Auswahlliste zum Festlegen der Schriftgröße für die aktuelle Auswahl in Pixeln anstelle von relativen Größenangaben (wie mit der Auswahlliste FontSize).</td>
		<td>-</td>
	</tr>
	
	<tr>
		<td align="middle"><img title="Ansicht umschalten" alt="Ansicht umschalten" src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Ansicht umschalten - Schaltet eWebEdit400 in den Vollbildmodus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Rahmen Anzeigen/Ausblenden" alt="Rahmen Anzeigen/Ausblenden" src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td>Rahmen Anzeigen/Ausblenden - Tabellenrahmen im Arbeitsbereich anzeigen/ausblenden.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Zoom" alt="Zoom" src="<%= this.SkinPath %>Buttons/Zoom.gif"></td>
		<td>Zoom - Ändert die Stufe der Vergrößerung.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Modulmanager" alt="Modulmanager" src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td>Modulmanager - Aktiviert/deaktiviert Module aus der Auswahlliste der verfügbaren Module.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Andocken ein-/ausschalten" alt="Andocken ein-/ausschalten" src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Andocken ein-/ausschalten - Dockt alle schwebenden Symbolleisten an ihre ursprüngliche Position.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Wiederholen" alt="Wiederholen" src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Wiederholen - Den zuletzt ausgeführten Arbeitsschritt wiederholen. </td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Suchen und Ersetzen" alt="Suchen und Ersetzen" src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Suchen und Ersetzen - Findet und ersetzt Text im Editor.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td align="middle"><img title="Drucken" alt="Drucken" src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td>Drucken - Druckt den Inhalt des eWebEdit400 oder der gesamten Internetseite.</td>
		<td>Ctrl+P</td>
	</tr>
	<tr>
		<td align="middle"><img title="Rechtschreibung" alt="Rechtschreibung" src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td>Rechtschreibung - Startet den Assistenten für die Rechtschreibprüfung.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Ausschneiden" alt="Ausschneiden" src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Ausschneiden - Schneidet den markierten Bereich aus und kopiert ihn in die Zwischenablage.</td>
		<td>Ctrl+X</td>
	</tr>
	<tr>
		<td align="middle"><img title="Kopieren" alt="Kopieren" src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td>Kopieren - Kopiert den markierten Bereich in die Zwischenablage.</td>
		<td>Ctrl+C</td>
	</tr>
	<tr>
		<td align="middle"><img title="Einfügen" alt="Einfügen" src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Einfügen - Fügt den Inhalt der Zwischenablage in den Editor ein.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td align="middle"><img title="Text einfügen" alt="Text einfügen" src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Text einfügen - Fügt Text (ohnen Formatierung) in den eWebEdit400 ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Word-Text einfügen" alt="Word-Text einfügen" src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>Word-Text einfügen - Fügt Inhalte aus Word ein, und entfernt Word-spezifische Formatierungen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="HTML einfügen" alt="HTML einfügen" src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td>HTML einfügen - Fügt HTML Code ein und behält alle HTML-Tags.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Rückgängig" alt="Rückgängig" src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Rückgängig - Macht den letzten Arbeitsschritt rückgängig.</td>
		<td>Ctrl+Z</td>
	</tr>
	<tr>
		<td align="middle"><img title="Wiederholen" alt="Wiederholen" src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td>Wiederholen - Wiederholt den letzten Arbeitsschritt, der rückgängig gemacht wurde.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formatierung entfernen" alt="Formatierung entfernen" src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Formatierung entfernen - Entfernt alle Formatierungen (auch benutzerdefinierte) aus dem ausgewählten Text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Direkthilfe" alt="Direkthilfe" src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td>Direkthilfe - Zeigt diesen Hilfe-Dialog an.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Info" alt="Info" src="<%= this.SkinPath %>Buttons/AboutDialog.gif"></td>
		<td>Info - Zeigt Informationen zur aktuellen Version von eWebEdit400 an.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>Tabellen, Links, Sonderzeichen, Bilder und 
				Medien einfügen und verwalten</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bild einfügen" alt="Bild einfügen" src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td>Bild einfügen - Fügt ein Bild aus einem vordefinierten Verzeichnis ein.</td>
		<td>Ctrl+G</td>
	</tr>
	<tr>
		<td align="middle"><img title="Imagemap" alt="Imagemap" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Imagemap - Erlaubt, anklickbaren Bereiche innerhalb von Bildern zu definieren.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Image Tool" alt="Image Tool" src="<%= this.SkinPath %>Buttons/EktronImageModificationTool.gif"></td>
		<td>Image Tool - Modify images in your content.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Absolute Position" alt="Absolute Position" src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Absolute Position - Legt für das selektierte Objekt absolute Positionierung fest (frei positionierbar).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Tabelle einfügen" alt="Tabelle einfügen" src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Tabelle einfügen - Fügt eine Tabelle in den eWebEdit400 ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Tabellenrahmen umschalten" alt="Tabellenrahmen umschalten" src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>Tabellenrahmen umschalten - Schaltet die Anzeige aller Tabellenrahmen innerhalb des Editors ein/aus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Code Elemente einfügen" alt="Code Elemente einfügen" src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Code Elemente einfügen - Fügt vorgefertigte Code-Elemente ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formularelement einfügen" alt="Formularelement einfügen" src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td>Formularelement einfügen - Fügt ein Formularelement aus der Auswahlliste der verfügbaren Elementen ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Datum einfügen" alt="Datum einfügen" src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Datum einfügen - Fügt das aktuelle Datum ein. </td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Zeit einfügen" alt="Zeit einfügen" src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Zeit einfügen - Fügt die aktuelle Zeit ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Flash einfügen" alt="Flash einfügen" src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td>Flash einfügen - Fügt eine Flash-Animation ein und erlaubt deren Eigenschaften zu
			verändern.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Windows Media einfügen" alt="Windows Media einfügen" src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td>Windows Media einfügen - Fügt ein Windows Media Objekt (AVI, MPEG, WAV, etc.) 
			ein und erlaubt deren Eigenschaften zu ändern.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Dokument einfügen" alt="Dokument einfügen" src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Dokument einfügen - Fügt einen Hyperlink auf ein Dokument (PDF, DOC, etc.) in den Editor ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Hyperlink erstellen" alt="Hyperlink erstellen" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Hyperlink erstellen - Macht aus dem ausgewählten Text, Nummer oder Bild einen 
			Hyperlink.</td>
		<td>Ctrl+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Hyperlink entfernen" alt="Hyperlink entfernen" src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td>Hyperlink entfernen - Entfernt einen Hyperlink aus dem gewählten Text, Nummer oder Bild.</td>
		<td>Ctrl+Shift+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Sonderzeichen einfügen" alt="Sonderzeichen einfügen" src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>Sonderzeichen einfügen - Fügt Sonderzeichen ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Benutzerdefinierten Hyperlink hinzufügen" alt="Benutzerdefinierten Hyperlink hinzufügen" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Benutzerdefinierten Hyperlink hinzufügen - Fügt einen internen oder externen Link 
			aus einer vordefinierten Liste hinzu.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="HTML Vorlage auswählen" alt="HTML Vorlage auswählen" src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td>HTML Vorlage auswählen - Eine vordefinierte HTML-Vorlage auswählen und einfügen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>ERSTELLEN, FORMATIEREN UND BEARBEITEN von 
				ZEICHEN UND LINIEN</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Neuen Abschnitt einfügen" alt="Neuen Abschnitt einfügen" src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td>Neuen Abschnitt einfügen - Fügt einen neuen Abschnitt ein.</td>
		<td>
			Ctrl+M<br>
			Ctrl+Enter
		</td>
	</tr>
	<tr>
		<td align="middle"><img title="Abschnitt-Formatierung" alt="Abschnitt-Formatierung" src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Abschnitt-Formatierung - Legt die Formatierung für den ausgewählten Text fest.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Einzug verkleinern" alt="Einzug verkleinern" src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td>Einzug verkleinern - Verkleinert den Einzug für den Abschnitt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Einzug vergrößern" alt="Einzug vergrößern" src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td>Einzug vergrößern - Vergrößert den Einzug für den Abschnitt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Horizontale Linie" alt="Horizontale Linie" src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td>Horizontale Linie - Fügt eine horizontale Linie ein.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Links ausrichten" alt="Links ausrichten" src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td>Links ausrichten - Richtet den ausgewählten Text linksbündig aus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Mittig ausrichten" alt="Mittig ausrichten" src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td>Mittig ausrichten - Richtet den ausgewählten Text mittig aus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Rechts ausrichten" alt="Rechts ausrichten" src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Rechts ausrichten - Richtet den ausgewählten Text rechtsbündig aus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Numerierte Liste" alt="Numerierte Liste" src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td>Numerierte Liste - Erstellt eine numerierte Liste.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Aufzhlungszeichen" alt="Aufzhlungszeichen" src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td>Aufzhlungszeichen - Erstellt eine Aufzählliste.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>ERSTELLEN, FORMATIEREN UND BEARBEITEN von TEXT, 
				SCHRIFTART und LISTEN</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Fett" alt="Fett" src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td>Fett - Formatiert den markierten Text fett.</td>
		<td>Ctrl+B</td>
	</tr>
	<tr>
		<td align="middle"><img title="Kursiv" alt="Kursiv" src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td>Kursiv - Formatiert den markierten Text kursiv.</td>
		<td>Ctrl+I</td>
	</tr>
	<tr>
		<td align="middle"><img title="Unterstreichen" alt="Unterstreichen" src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td>Unterstreichen - Unterstreicht den markierten Text.</td>
		<td>Ctrl+U</td>
	</tr>
	<tr>
		<td align="middle"><img title="Hochgestellt" alt="Hochgestellt" src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td>Hochgestellt - Formatiert Text oder Nummern als hochgestellt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Tiefgestellt" alt="Tiefgestellt" src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td>Tiefgestellt - Formatiert Text oder Nummern als tiefergestellt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Schriftart" alt="Schriftart" src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td>Schriftart - Schriftart wählen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Schriftgröße" alt="Schriftgröße" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Schriftgröße - Schriftgröße ändern.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Textfarbe (Vordergrund)" alt="Textfarbe (Vordergrund)" src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td>Textfarbe (Vordergrund) - Ändert die Vordergrundfarbe des ausgewählten Textes.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Textfarbe (Hintergrund)" alt="Textfarbe (Hintergrund)" src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td>Textfarbe (Hintergrund) - Ändert die Hintergrundfarbe des ausgewählten Textes.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Benutzerdefinierte Formate" alt="Benutzerdefinierte Formate" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Benutzerdefinierte Formate - Fügt vordefinierte 
			Formatierungen zum ausgewählten Text hinzu.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>ANDERE TASTENKOMBINATIONEN</strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td>Markiert den gesamten Text, alle Bilder und Tabellen im Editor</td>
		<td>Ctrl+A</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Findet Text oder Zahlen im aktuellen Dokument.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Schließt das aktive Fenster.</td>
		<td>Ctrl+W</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Schließt die aktive Anwendung.</td>
		<td>Ctrl+F4</td>
	</tr>
</table>
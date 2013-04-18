<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>
<table cellspacing="0" cellpadding="2" border="1" bordercolor="#000000" style="font:normal 10px Arial">
	<tr>
		<td colspan="3" align="center"><strong>GENERELE KNAPPER</strong></td>
	</tr>
	<tr>
		<td align="center"><img title="Design knapp" alt="Design knapp" src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td> Design knapp -&nbsp;Setter Ektron eWebEdit400 i Design-modus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="HTML knapp" alt="HTML knapp" src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td> HTML&nbsp;knapp - Setter Ektron eWebEdit400 i HTML-modus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Forhåndsvisning" alt="Forhåndsvisning" src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Forhåndsvisning&nbsp;-&nbsp;Setter Ektron eWebEdit400 i forhåndsvinsings-modus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Skjermmodus" alt="Skjermmodus" src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Skjermmodus -&nbsp;Setter Ektron eWebEdit400 i Fullskjerm-modus.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Vis/Skjul ramme" alt="Vis/Skjul ramme" src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td> Vis/Skjul&nbsp;ramme - Viser eller skjuler rammer rundt tabeller.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Modul-håndtering" alt="Modul-håndtering" src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td> Modul-håndtering - Aktiverer /Deaktiverer mdouler fra nedtrekksmeny over 
			tilgjenglige moduler.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Docking" alt="Docking" src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Docking - Legger alle flytende verkøtylinjer tilbake til deres respektive område.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Gjenta siste kommando" alt="Gjenta siste kommando" src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Gjenta siste kommando - Snarvei for å gjenta siste handling.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Søk og erstatt" alt="Søk og erstatt" src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Søk og erstatt&nbsp;-&nbsp;Søker (og erstatter) tekst i innholdet.</td>
		<td>Ctrl+f</td>
	</tr>
	<tr>
		<td align="center"><img title="Skriv ut" alt="Skriv ut" src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td> Skriv ut - Skriver ut innholdet i&nbsp;Ektron eWebEdit400 eller hele websiden.</td>
		<td>Ctrl+p</td>
	</tr>
	<tr>
		<td align="center"><img title="Stavekontroll" alt="Stavekontroll" src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td> Stavekontroll - Starter stavekontrollen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Klipp ut" alt="Klipp ut" src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Klipp ut&nbsp;- Klipper ut valgt innhold&nbsp;til utklippstavla.</td>
		<td>Ctrl+x</td>
	</tr>
	<tr>
		<td align="center"><img title="Kopier" alt="Kopier" src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td> Kopier - Kopierer valgt innhold til utklippstavla.</td>
		<td>Ctrl+c</td>
	</tr>
	<tr>
		<td align="center"><img title="Lim inn" alt="Lim inn" src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Lim inn&nbsp; - Limer inn innholdet fra utkilippstavla i editoren.</td>
		<td>Ctrl+v</td>
	</tr>
	<tr>
		<td align="center"><img title="Lim inn ren tekst" alt="Lim inn ren tekst" src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Lim inn ren tekst - Limer inn innholdet som ren tekst (ingen formatering).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Lim inn fra Word" alt="Lim inn fra Word" src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>Lim inn fra Word - Lim inn tekst fra Word og fjern formattering som ikke passer 
			for web.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Linn inn HTML" alt="Linn inn HTML" src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td> Linn inn HTML&nbsp;- Limer inn innhold fra HTML og beholder 
			all formattering.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Angre" alt="Angre" src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Angre&nbsp;- Tar tilbake siste handling.</td>
		<td>Ctrl+z</td>
	</tr>
	<tr>
		<td align="center"><img title="Gjør på nytt" alt="Gjør på nytt" src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td> Gjør&nbsp;på nytt&nbsp;- Gjør siste handling på nytt.</td>
		<td>Ctrl+y</td>
	</tr>
	<tr>
		<td align="center"><img title="Format fjerner" alt="Format fjerner" src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Format&nbsp;fjerner - Fjerner egendefinert eller all formattering fra valgt innhold.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Hjelp" alt="Hjelp" src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td> Hjelp - Denne siden.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="center" height=18><strong>SETT INN OG ADMINISTRER LINKER, TABELLER, SPESIELLE 
				KARAKTERER, BILDER OG MEDIA</strong></td>
	</tr>
	<tr>
		<td align="center"><img title="Bildebehandling" alt="Bildebehandling" src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td> Bildebehandling&nbsp;- Sett inn bilder fra forhåndefinerte mapper.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Absolutt objekt posisjonering" alt="Absolutt objekt posisjonering" src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Absolutt objekt posisjonering -&nbsp;Setter absolutt posisjon på et objekt&nbsp;(fri 
			flytting).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn tabell" alt="Sett inn tabell" src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Sett inn tabell - Sett inn en tabell i&nbsp;Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Skru på/av rammer" alt="Skru på/av rammer" src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>Skru på/av rammer&nbsp;- Skru av på rammer på alle tabeller i Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn kodesnutt" alt="Sett inn kodesnutt" src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Sett inn kodesnutt - Sett inn forhåndsdefinerte kodesnutter.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn Skjema-elemnt" alt="Sett inn Skjema-elemnt" src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td> Sett inn Skjema-elemnt - Sett inn skjema-element 
			fra nedtrekksmeny.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn dato" alt="Sett inn dato" src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Sett inn dato&nbsp; - Sett inn gjeldende dato.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn tid" alt="Sett inn tid" src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Sett inn tid - Sett in gjeldende tid.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Flash-behandling" alt="Flash-behandling" src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td> Flash-behandling&nbsp; - Sett inn en Flash 
			animasjon.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Windows Media behandling" alt="Windows Media behandling" src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td> Windows Media&nbsp;behandling -&nbsp;Sett inn 
			et Windows media&nbsp;objekt (AVI, MPEG, WAV, etc.).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Dkcumentbehandling" alt="Dkcumentbehandling" src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Dkcumentbehandling - Sett inn&nbsp;link til et&nbsp;dokument&nbsp;på&nbsp;serveren (PDF, DOC, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Hyperlink" alt="Hyperlink" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td> Hyperlink&nbsp;- Lager link på valgt tekst eller bilde.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Fjern Hyperlink" alt="Fjern Hyperlink" src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td> Fjern&nbsp;Hyperlink&nbsp;- Fjerner link fra valgt tekst eller 
			bilde.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn spesialkarakter" alt="Sett inn spesialkarakter" src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>      Sett inn spesialkarakter -&nbsp;(€ ®, <font face="Arial">
				©, ±</font>, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn egendefinert link" alt="Sett inn egendefinert link" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Sett inn egendefinert&nbsp;link&nbsp;- Sett inn en ekstern eller ekstern link fra en 
			forhåndsdefinert liste.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Velg HTML mal" alt="Velg HTML mal" src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td> Velg&nbsp;HTML&nbsp;mal -&nbsp;Bruk en&nbsp;HTML&nbsp;mal fra en forhåndsdefinert 
			liste.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="center"><strong>OPPRETT, FORMATER OG REDIGER AVSNITT OG LINJER</strong></td>
	</tr>
	<tr>
		<td align="center"><img title="Sett inn nytt avsnitt" alt="Sett inn nytt avsnitt" src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td> Sett inn nytt avsnitt.</td>
		<td>Ctrl+m</td>
	</tr>
	<tr>
		<td align="center"><img title="Stil på avsnitt" alt="Stil på avsnitt" src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Stil på avsnitt -&nbsp;Bruk standard tekt stiler på valgt 
			tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Utrykk" alt="Utrykk" src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td> Utrykk - Rykker avsnitt til venstre.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Innrykk" alt="Innrykk" src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td> Innrykk &nbsp;- Rykker avsnitt til høyre.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Juster venstre" alt="Juster venstre" src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td> Juster venstre - Justerer tekst i valgt avsnitt til venstre.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sentrert" alt="Sentrert" src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td> Sentrert&nbsp; - Sentrerer teksten i valgt avsnitt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Juster høyre" alt="Juster høyre" src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Juster&nbsp;høyre&nbsp;- Justerer tekst i valgt avsnitt til høyre.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Blokk" alt="Blokk" src="<%= this.SkinPath %>Buttons/JustifyFull.gif"></td>
		<td>Blokk - Justerer tekst i valgt avsnitt som blokk.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Punktliste" alt="Punktliste" src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td> Punktliste&nbsp;- Oppretter en punktliste for valgt tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Nummerliste" alt="Nummerliste" src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td> Nummerliste - Opprettter en nummerert liste for valgt tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Sett in horisontal linje" alt="Sett in horisontal linje" src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td> Sett in horisontal linje.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="center"><strong>OPPRETT, FORMATER OG EDITER TEKST, FONTER OG LISTER</strong></td>
	</tr>
	<tr>
		<td align="center"><img title="Uthev" alt="Uthev" src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td> Uthev - Utherver valgt tekst.</td>
		<td>Ctrl+b</td>
	</tr>
	<tr>
		<td align="center"><img title="Kursiv" alt="Kursiv" src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td> Kursiv - Kursiv formaterring av valgt tekst.</td>
		<td>Ctrl+i</td>
	</tr>
	<tr>
		<td align="center"><img title="Understrek" alt="Understrek" src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td> Understrek&nbsp;- Understrek valgt tekst.</td>
		<td>Ctrl+u</td>
	</tr>
	<tr>
		<td align="center"><img title="Opphevet tekst" alt="Opphevet tekst" src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td> Opphevet tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Nedsenket tekst" alt="Nedsenket tekst" src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td> Nedsenket tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Velg skrifttype" alt="Velg skrifttype" src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td> Velg skrifttype.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Skriftstørrelse" alt="Skriftstørrelse" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Skriftstørrelse&nbsp;- Setter størrelsen på skriften.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Tekstfarge (forgrunn)" alt="Tekstfarge (forgrunn)" src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td> Tekstfarge (forgrunn)&nbsp;- Endre forgurnnsfarge på valgt 
			tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Tekstfarge (bakgrrunn)" alt="Tekstfarge (bakgrrunn)" src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td> Tekstfarge&nbsp;(bakgrrunn) - Endre bakrgunnsfarge på valgt 
			tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Egendefinerte stiler" alt="Egendefinerte stiler" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td> Egendefinerte stiler&nbsp;-&nbsp;Velg egne forhåndsdefinerte stiler på valgt 
			tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="center"><img title="Formatfjerner" alt="Formatfjerner" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td> Formatfjerner - Fjerner formattering 
			på valgt tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="center"><strong>ANDRE TASTATUR SNARVEIER</strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td> Velg all tekst, bilder og tabeller i editor.</td>
		<td>Ctrl+a</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Søk.</td>
		<td>Ctrl+f</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Lukk det aktive vinduet.</td>
		<td>Ctrl+w</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Lukk den aktive applikasjonen.</td>
		<td>Ctrl+F4</td>
	</tr>
</table>
<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>
<table cellspacing="0" cellpadding="2" border="1" bordercolor="#000000" style="font:normal 10px MS Sans Serif">
	<tr>
		<td colspan="3" align="middle"><strong>ALGEMEEN</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Ongedaan maken" alt="Ongedaan maken" src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Ongedaan maken - Maakt laatste wijziging ongedaan.</td>
		<td>Ctrl+z</td>
	</tr>
	<tr>
		<td align="middle"><img title="Opnieuw" alt="Opnieuw" src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td>Opnieuw - Doet de laatste wijziging, welke ongedaan is gemaakt, opnieuw.</td>
		<td>Ctrl+y</td>
	</tr>
	<tr>
		<td align="middle"><img title="Spellingscontrole" alt="Spellingscontrole" src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td>Spellingscontrole - Opend en voert uit de spellingscontrole.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Zoek en Vervang" alt="Zoek en Vervang" src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Zoek en Vervang - Zoek (en vervang) tekst in de content.</td>
		<td>Ctrl+f</td>
	</tr>
	<tr>
		<td align="middle"><img title="Knippen" alt="Knippen" src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Knippen - Knipt de geselecteerde content en plaats het op het klembord.</td>
		<td>Ctrl+x</td>
	</tr>
	<tr>
		<td align="middle"><img title="Kopieren button" alt="Kopieren button" src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td>Kopieren button - Kopieert de geselecteerde content naar het klembord.</td>
		<td>Ctrl+c</td>
	</tr>
	<tr>
		<td align="middle"><img title="Plakken" alt="Plakken" src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Plakken - Voegt de gekopieerde of geknipte content in.</td>
		<td>Ctrl+v</td>
	</tr>
	<tr>
		<td align="middle"><img title="Plakken vanuit MS Word" alt="Plakken vanuit MS Word" src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>Plakken vanuit MS Word - Voegt vanuit Word gekopieerde content in zonder web-onvriendelijke opmaak attributen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Plakken vanuit MS Word zonder lettertype en tekstgrootte" alt="Plakken vanuit MS Word zonder lettertype en tekstgrootte" src="<%= this.SkinPath %>Buttons/PasteFromWordNoFontsNoSizes.gif"></td>
		<td>Plakken vanuit MS Word zonder lettertype en tekstgrootte - Voegt vanuit Word gekopieerde content in zonder lettertype en tekst grote opmaak.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Plakken als platte tekst" alt="Plakken als platte tekst" src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Plakken als platte tekst - Voegt gekopieerde content zonder enige opmaak in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Plakken als HTML" alt="Plakken als HTML" src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td>Plakken als HTML - Voegt van andere websites gekopieerde content in met behoud van opmaak.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Code Invoegen" alt="Code Invoegen" src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Code Invoegen - Voegt voorgedefinieerde onderdelen in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Symbool Invoegen" alt="Symbool Invoegen" src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>Symbool Invoegen - Voegt een speciaal character in (&euro; &reg;, <font face="Arial">�, �</font>, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Tabel Invoegen" alt="Tabel Invoegen" src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Tabel Invoegen - Voegt een tabel in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Datum Invoegen" alt="Datum Invoegen" src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Datum Invoegen - Voegt huidige datum in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Tijd Invoegen" alt="Tijd Invoegen" src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Tijd Invoegen - Voegt huidige tijd in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Help" alt="Help" src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td>Help - Opende deze helppagina.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>TEKST OPMAAK</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Formatering Verwijderen" alt="Formatering Verwijderen" src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Formatering Verwijderen - Verwijder enkele of alle formattering van geselecteerde tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Stijl Toepassen" alt="Stijl Toepassen" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Stijl Toepassen - Past een voorgedefinieerde stijl toe op de geselecteerde tekst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Vet" alt="Vet" src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td>Vet - Zet geselecteerde tekst in Vet.</td>
		<td>Ctrl+b</td>
	</tr>
	<tr>
		<td align="middle"><img title="Schuin" alt="Schuin" src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td>Schuin - Zet geselecteerde tekst Schuin.</td>
		<td>Ctrl+i</td>
	</tr>
	<tr>
		<td align="middle"><img title="Onderstrepen" alt="Onderstrepen" src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td>Onderstrepen - Onderstreept geselecteerde tekst.</td>
		<td>Ctrl+u</td>
	</tr>
	<tr>
		<td align="middle"><img title="Links Uitlijnen" alt="Links Uitlijnen" src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td>Links Uitlijnen - Lijnt de geselecteerde paragraaf links uit.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Centreren" alt="Centreren" src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td>Centreren - Centreert de geselecteerde paragraaf.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Rechts Uitlijnen" alt="Rechts Uitlijnen" src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Rechts Uitlijnen - Lijnt de geselecteerde paragraaf rechts uit.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Verdelen" alt="Verdelen" src="<%= this.SkinPath %>Buttons/JustifyFull.gif"></td>
		<td>Verdelen - Verdeelt de geselecteerde paragraaf over de breedte van de pagina.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Verwijder Uitlijning" alt="Verwijder Uitlijning" src="<%= this.SkinPath %>Buttons/JustifyNone.gif"></td>
		<td>Verwijder Uitlijning - Verwijderd eventuele uitlijning van geselecteerde paragraaf.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Indent" alt="Indent" src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td>Indent - Indenteert de paragraaf naar rechts.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Outdent" alt="Outdent" src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td>Outdent - Indenteert de paragraaf naar links.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Superscript" alt="Superscript" src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td>Superscript - Maakt de geselecteerde tekst superscript.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Subscript" alt="Subscript" src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td>Subscript - Maakt de geselecteerde tekst subscript.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Genummerde Lijst" alt="Genummerde Lijst" src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td>Genummerde Lijst - Maakt van de huidige selectie een genummerde lijst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Gemarkeerde Lijst" alt="Gemarkeerde Lijst" src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td>Gemarkeerde Lijst - Maakt van de huidige selectie een gemarkeerde lijst.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Horizontale lijn" alt="Horizontale lijn" src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td>Horizontale lijn - Voegt een horizontale lijn in op de huidige cursor positie.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>INVOEGEN</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Link Toevoegen" alt="Link Toevoegen" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Link Toevoegen - Maakt van de geselecteerde tekst of foto een afbeelding.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Link Verwijderen" alt="Link Verwijderen" src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td>Link Verwijderen - Verwijderd link van geselecteerde tekst of afbeelding.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Afbeelding Invoegen" alt="Afbeelding Invoegen" src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td>Afbeelding Invoegen - Voegt een foto van een andere lokatie.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>WEBSITE SPECIFIEKE INVOEG KNOPPEN (indien aanwezig)</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Interne Link Invoegen" alt="Interne Link Invoegen" src="<%= this.SkinPath %>Buttons/InternalLink.gif"></td>
		<td>Interne Link Invoegen - Voeg een link toe naar een interne pagina.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Intern Document Invoegen" alt="Intern Document Invoegen" src="<%= this.SkinPath %>Buttons/InternalLink.gif"></td>
		<td>Intern Document Invoegen - Voeg een intern document in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Interne Afbeelding Invoegen" alt="Interne Afbeelding Invoegen" src="<%= this.SkinPath %>Buttons/InternalImage.gif"></td>
		<td>Interne Afbeelding Invoegen - Voeg een interne afbeelding in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Interne Thumbnail Invoegen" alt="Interne Thumbnail Invoegen" src="<%= this.SkinPath %>Buttons/InternalThumbnail.gif"></td>
		<td>Interne Thumbnail Invoegen - Voeg een thumbnail in van een interne afbeelding.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Interne Flash Annimatie Invoegen" alt="Interne Flash Annimatie Invoegen" src="<%= this.SkinPath %>Buttons/InternalFlash.gif"></td>
		<td>Interne Flash Annimatie Invoegen - Voeg een interne flash Annimatie in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Intern Geluid Invoegen" alt="Intern Geluid Invoegen" src="<%= this.SkinPath %>Buttons/InternalSound.gif"></td>
		<td>Intern Geluid Invoegen - Voeg een intern geluid in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Interne Video Invoegen" alt="Interne Video Invoegen" src="<%= this.SkinPath %>Buttons/InternalMovie.gif"></td>
		<td>Interne Video Invoegen - Voeg een interne video in.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>OPTIONELE KNOPPEN (indien aanwezig)</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Nieuwe Paragraaf Invoegen" alt="Nieuwe Paragraaf Invoegen" src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td>Nieuwe Paragraaf Invoegen - Voegt een nieuwe paragraaf in.</td>
		<td>Ctrl+m</td>
	</tr>
	<tr>
		<td align="middle"><img title="Ontwerp" alt="Ontwerp" src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td>Ontwerp - Zet de editor in Ontwerp (standaard) Mode.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="HTML" alt="HTML" src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td>HTML - Zet de editor in HTML Mode.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>OTHER KEYBOARD SHORTCUTS</strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td>Selects all text, images and tables in the editor.</td>
		<td>Ctrl+a</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Finds a string of text or numbers in the page.</td>
		<td>Ctrl+f</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Closes the active window.</td>
		<td>Ctrl+w</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Closes the active application.</td>
		<td>Ctrl+F4</td>
	</tr>
	<!--<tr>
		<td colspan="3" align="middle"><strong>GENERAL BUTTONS</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Preview button - Switches Ektron eWebEdit400 into Preview Mode.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Toggle Screen Mode - Switches Ektron eWebEdit400 into Full Screen Mode.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td>Show/Hide Border - Shows or hides borders around tables in the content area.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td>Module Manager - Activates /Deactivates modules from a drop-down list of 
			available modules.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Toggle Docking - Docks all floating toolbars to their respective docking areas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Repeat Last Command - A short-cut to repeat the last action performed.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td>Print button - Prints the contents of the Ektron eWebEdit400 or the whole web page.</td>
		<td>Ctrl+p</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/AboutDialog.gif"></td>
		<td>About Dialog - Shows the current version and credentials of Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Absolute Object Position button - Sets an absolute position of an object (free 
			move).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>Toggle Table Borders - Toggles borders of all tables within the editor.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td>Choose HTML Template - Applies and HTML template from a predefined list of 
			templates.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Paragraph Style Dropdown button - Applies standard text styles to selected 
			text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td>Flash Manager button - Inserts a Flash animation and lets you set its 
			properties.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td>Windows Media Manager button - Inserts a Windows media object (AVI, MPEG, WAV, 
			etc.) and lets you set its properties.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Document Manager - Inserts a link to a document on the server (PDF, DOC, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Insert Custom Link dropdown - Inserts an internal or external link from a 
			predefined list.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td>Insert Form Element - Inserts a form element from a drop-down list with 
			available elements.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td>Font Select button - Sets the font typeface.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Font Size button - Sets the font size.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td>Text Color (foreground) button - Changes the foreground color of the selected 
			text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="" alt="" src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td>Text Color (background) button - Changes the background color of the selected 
			text.</td>
		<td>-</td>
	</tr>
	-->
</table>
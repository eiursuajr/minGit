<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>
<table cellspacing="0" cellpadding="2" border="1" bordercolor="#000000" style="font:normal 10px Arial">
	<tr>
		<td colspan="3" align="middle"><strong>GENERELLA KNAPPAR</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Design" alt="Design" src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td>Design - Ställer om Ektron eWebEdit400 till Design läge.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="HTML" alt="HTML" src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td>HTML - Ställer om Ektron eWebEdit400 till HTML läge.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Förhandsgranska" alt="Förhandsgranska" src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Förhandsgranska - Ställer om Ektron eWebEdit400 till Förhandsgranska läge.</td>
		<td>-</td>
	</tr>
	
	<tr>
		<td align="middle"><img title="Versaler" alt="Versaler" src="<%= this.SkinPath %>Buttons/ConvertToUpper.gif"></td>
		<td>Konvertera markerad text till versaler, bibehåller icke text element som bilder och tabeller.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Gemener" alt="Gemener" src="<%= this.SkinPath %>Buttons/ConvertToLower.gif"></td>
		<td>Konvertera markerad text till gemener, bibehåller icke text element som bilder och tabeller.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Image maps" alt="Image maps" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Tillåt användare att skapa <i>image maps</i> genom att dra över bilden och skapa länkar med olika figurer.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formatera block av kod" alt="Formatera block av kod" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Tillåt användaren att infoga och formatera block av kod i redigeringsytan.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Teckensnittstorlek" alt="Teckensnittstorlek" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Tillåt att användaren för ett markerad text kan ange teckensnittstorlek i pixels, istället för fixerade storlekar mellan 1 till 7.</td>
		<td>-</td>
	</tr>
		
	<tr>
		<td align="middle"><img title="Byt skärmläge" alt="Byt skärmläge" src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Byt skärmläge - Ställer om Ektron eWebEdit400 i fullskärmsläge.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Visa/Dölj kantlinje" alt="Visa/Dölj kantlinje" src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td>Visa/Dölj kantlinje - Visar eller döljer kantlinjer runt tabeller i redigeringsytan.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Zoom" alt="Zoom" src="<%= this.SkinPath %>Buttons/Zoom.gif"></td>
		<td>Zoom - Ändrar storleken på text förstorning.</td>
		<td>-</td>
	</tr>

	<tr>
		<td align="middle"><img title="Modulhanteraren" alt="Modulhanteraren" src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td>Modulhanteraren - Aktiverar / Deaktiverar moduler från en vallista av möjliga moduler.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Byt dockning" alt="Byt dockning" src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Byt dockning - Dockar all flytande verktygsfält till deras respektive dockningsplatser.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Repetera senaste kommando" alt="Repetera senaste kommando" src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Repetera senaste kommando - En snabbväg för att repetera det senaste åtgärden.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Sök och Ersätt" alt="Sök och Ersätt" src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Sök och Ersätt - Söker (och ersätter) text i redigeringsytan.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td align="middle"><img title="Utskrift" alt="Utskrift" src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td>Utskrift - Skriver ut innehållet i redigeringsytan som en sida.</td>
		<td>Ctrl+P</td>
	</tr>
	<tr>
		<td align="middle"><img title="Rättstavning" alt="Rättstavning" src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td>Rättstavning - Startar rättstavningen.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Klipp ut" alt="Klipp ut" src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Klipp ut - Klipper ut markerad text och kopierar det till Urklipp.</td>
		<td>Ctrl+X</td>
	</tr>
	<tr>
		<td align="middle"><img title="Kopiera" alt="Kopiera" src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td>Kopiera - Kopierar markerad text till Urklipp.</td>
		<td>Ctrl+C</td>
	</tr>
	<tr>
		<td align="middle"><img title="Klistra in" alt="Klistra in" src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Klistra in - Klistrar in kopierad text från Urklipp till redigeringsytan.</td>
		<td>Ctrl+V</td>
	</tr>
	<tr>
		<td align="middle"><img title="Klistra in från Word" alt="Klistra in från Word" src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>Klistra in från Word - Klistrar in kopierad text från Word och tar bort webb ovänlig kod.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Klistra in från Word med rensning av kod" alt="Klistra in från Word med rensning av kod" src="<%= this.SkinPath %>Buttons/PasteFromWordNoFontsNoSizes.gif"></td>
		<td>Klistra in från Word med rensning av kod - Klistrar in kopierad text från Word och rensar all teckensnitt- och storleks- kod.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Klistra in ren text" alt="Klistra in ren text" src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Klistra in ren text - Klistrar in kopierad text (utan formateringar).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Klistra in HTML" alt="Klistra in HTML" src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td>Klistra in HTML - Klistrar in HTML kod till redigeringsytan med bibehållen HTML kod.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Ångra" alt="Ångra" src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Ångra - Ångra senaste inmatning.</td>
		<td>Ctrl+Z</td>
	</tr>
	<tr>
		<td align="middle"><img title="Upprepa" alt="Upprepa" src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td>Upprepa - Upprepar senaste ångrade inmatning.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td align="middle"><img title="Rensa formatering" alt="Rensa formatering" src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Rensa formatering - Tar bort egen eller annan formatering i den markerade texten.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Snabb Hjälp" alt="Snabb Hjälp" src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td>Snabb Hjälp - Startar Snabb hjälpen som du läser nu.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Om Dialogen" alt="Om Dialogen" src="<%= this.SkinPath %>Buttons/AboutDialog.gif"></td>
		<td>Om Dialogen - Visar version och vitsord för Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>INFOGA OCH HANTERAR LÄNKAR, TABELLER, SPECIALTECKEN, BILDER OCH MEDIA</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bildhanteraren" alt="Bildhanteraren" src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td>Bildhanteraren- Infogar en bild från en fördefinierad mapp(ar).</td>
		<td>Ctrl+G</td>
	</tr>
	<tr>
		<td align="middle"><img title="Image map" alt="Image map" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td><i>Image map</i> - Tillåter att användare kan skapa klickbara regioner inom en bild.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Positionering av objekt" alt="Positionering av objekt" src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Positionering av objekt - Sätter en absolut position för ett objekt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga tabell" alt="Infoga tabell" src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Infoga tabell - Infogar en tabell i Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Visa/Dölj Tabellkanter" alt="Visa/Dölj Tabellkanter" src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>Visa/Dölj Tabellkanter - Visar/Döljer tabellkanter för alla tabeller i redigeringsytan.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga kod snutt" alt="Infoga kod snutt" src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Infoga kod snutt - Infogar en fördefinierad kod snutt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga formulär element" alt="Infoga formulär element" src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td>Infoga formulär element - Infogar ett formulär element från en vallista med möjliga element.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga datum" alt="Infoga datum" src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Infoga datum - Infogar dagens datum.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga tid" alt="Infoga tid" src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Infoga tid - Infogar tid.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Flashhanteraren" alt="Flashhanteraren" src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td>Flashhanteraren - Infogar en Flash animering och låter dig sätta egenskaper för den.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Windows Media hanteraren" alt="Windows Media hanteraren" src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td>Windows Media hanteraren - Infogar en Windows media objekt (AVI, MPEG, WAV, 
			mm.) och låter dig sätta egenskaper för den.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Dokumenthanteraren" alt="Dokumenthanteraren" src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Dokumenthanteraren - Infogar en länk till ett dokument på servern (PDF, DOC, mm.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Länkhanteraren" alt="Länkhanteraren" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Länkhanteraren - Infogar en länk från en markerad text eller bild.</td>
		<td>Ctrl+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Ta bort länk" alt="Ta bort länk" src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td>Ta bort länk - Tar bort en länk för markerad text eller bild.</td>
		<td>Ctrl+Shift+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga specialtecken" alt="Infoga specialtecken" src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>Infoga specialtecken - Infogar specialtecken  (&euro; &reg;, <font face="Arial">
				, </font>, mm.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga egen länk" alt="Infoga egen länk" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Infoga egen länk - Infogar en länk från en fördefinierad lista.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Välj HTML mall" alt="Välj HTML mall" src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td>Välj HTML mall - Infogar en HTML mall från en fördefinierad lista.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>INFOGA, FORMATERA OCH ÄNDRA STYCKEN och LINJER</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga nytt stycke" alt="Infoga nytt stycke" src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td>Infoga nytt stycke - Infogar nytt stycke.</td>
		<td>Ctrl+M</td>
	</tr>
	<tr>
		<td align="middle"><img title="Format" alt="Format" src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Format - Applicerar standard text format för markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Minska indrag" alt="Minska indrag" src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td>Minska indrag - Minska indrag för stycket till vänster.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Indrag" alt="Indrag" src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td>Indrag - Indrag för stycket till höger.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Vänsterjustera" alt="Vänsterjustera" src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td>Vänsterjustera - Vänsterjustera valt stycke.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Centrera" alt="Centrera" src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td>Centrera - Centrerar valt stycke..</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Högerjustera" alt="Högerjustera" src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Högerjustera - Högerjustera valt stycke.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Justera" alt="Justera" src="<%= this.SkinPath %>Buttons/JustifyFull.gif"></td>
		<td>Justera - Justera valt stycke.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Punktlista" alt="Punktlista" src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td>Punktlista - Skapar en punktlista i markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Numrerad lista" alt="Numrerad lista" src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td>Numrerad lista - Skapar en numrerad lista i markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Infoga horisontell linje" alt="Infoga horisontell linje" src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td>Infoga horisontell linje - Infogar en horisontell linje.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>INFOGA, FORMATERA och ÄNDRA TEXT, TECKENSNITT och LISTOR</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Fet" alt="Fet" src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td>Fet - Applicerar fetstil på markerad text.</td>
		<td>Ctrl+B</td>
	</tr>
	<tr>
		<td align="middle"><img title="Kursiv" alt="Kursiv" src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td>Kursiv - Applicerar kursiv stil på markerad text.</td>
		<td>Ctrl+I</td>
	</tr>
	<tr>
		<td align="middle"><img title="Understruken" alt="Understruken" src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td>Understruken - Applicerar understruken stil på markerad text.</td>
		<td>Ctrl+U</td>
	</tr>
	<tr>
		<td align="middle"><img title="Genomstruken" alt="Genomstruken" src="<%= this.SkinPath %>Buttons/StrikeThrough.gif"></td>
		<td>Genomstruken - Applicerar genomstruken stil på markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Upphöjd" alt="Upphöjd" src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td>Upphöjd - Applicerar upphöjd stil på markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Nedsänkt" alt="Nedsänkt" src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td>Nedsänkt - Applicerar nedsänkt stil på markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Välj teckensnitt" alt="Välj teckensnitt" src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td>Välj teckensnitt - Sätter ett teckensnitt.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Teckenstorlek" alt="Teckenstorlek" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Teckenstorlek - Sätter en teckenstorlek.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Text färg (förgrund)" alt="Text färg (förgrund)" src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td>Text färg (förgrund) - Ändrar förgrundsfärgen för markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Text färg (bakgrund)" alt="Text färg (bakgrund)" src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td>Text färg (bakgrund) - Ändrar bakgrundsfärgen för markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Egen stil" alt="Egen stil" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Egen stil - Applicerar en egen fördefinierad CSS stil på markerad text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formatera kod block" alt="Formatera kod block" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Tillåter att användaren kan infoga och formatera kod block i texten.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Egna länkar" alt="Egna länkar" src="<%= this.SkinPath %>Buttons/CustomLinks.gif"></td>
		<td>Egna länkar - Infoga en egen fördefinierad länk.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>ANDRA SNABBTANGENTER</strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td>Markerar all text, bilder och tabeller i redigeringsytan.</td>
		<td>Ctrl+A</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Söker efter en text eller nummer i texten.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Stänger det aktiva fönstret.</td>
		<td>Ctrl+W</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Stänger den aktiva applikationen.</td>
		<td>Ctrl+F4</td>
	</tr>
</table>
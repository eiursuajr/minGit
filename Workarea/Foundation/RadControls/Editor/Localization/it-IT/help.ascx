<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>
<table cellspacing="0" cellpadding="2" border="1" bordercolor="#000000" style="font:normal 10px Arial">
	<tr>
		<td colspan="3" align="middle"><strong>PULSANTI GENERALI</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Vista di Progetazione" alt="Vista di Progetazione" src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td>Vista di Progetazione - Mostra il Ektron eWebEdit400 nella visualizzazione di progettazione.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Vista HTML" alt="Vista HTML" src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td>Vista HTML - Mostra il Ektron eWebEdit400 nella visione del codice HTML.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Vista Preview" alt="Vista Preview" src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Vista Preview - Mostra il Ektron eWebEdit400 nella visione di Preview.</td>
		<td>-</td>
	</tr>
	
	<tr>
		<td align="middle"><img title="Maiuscoli" alt="Maiuscoli" src="<%= this.SkinPath %>Buttons/ConvertToUpper.gif"></td>
		<td>Converte la selezione corrente del testo in caratteri maiuscoli, preservando gli elementi non testuali come immagini e tabelle.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Minuscoli" alt="Minuscoli" src="<%= this.SkinPath %>Buttons/ConvertToLower.gif"></td>
		<td>Converte la selezione corrente del testo in caratteri minuscoli, preservando gli elementi non testuali come immagini e tabelle.
	</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Mappe immagini" alt="Mappe immagini" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Permette agli utenti di creare mappe immagini disegnando sulle immagini e creando aree di collegamenti ipertestuali con forme differenti.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formattare blocchi di codice" alt="Formattare blocchi di codice" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Permette agli utenti di inserire e formattare blocchi di codice dentro il contenuto del Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Dimensione del font" alt="Dimensione del font" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Permette agli utenti di applicare alla selezione corrente di testo una dimensione del font in pixels, oltre che impostarla con con una dimesione fissa da 1 a 7 (come lo strumento di dimensione Font).</td>
		<td>-</td>
	</tr>
		
	<tr>
		<td align="middle"><img title="Vista Tutto schermo" alt="Vista Tutto schermo" src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Vista Tutto schermo - Cambia la visualizzazione del Ektron eWebEdit400 mettendola a tutto schermo.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Mostra/Nascondi Bordi" alt="Mostra/Nascondi Bordi" src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td>Mostra/Nascondi Bordi - Mostra o nasconde i bordi intorno alle tabelle nell' area di contenuto del Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Zoom" alt="Zoom" src="<%= this.SkinPath %>Buttons/Zoom.gif"></td>
		<td>Zoom - Cambia il livello di zoom del testo.</td>
		<td>-</td>
	</tr>

	<tr>
		<td align="middle"><img title="Manager Moduli" alt="Manager Moduli" src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td>Manager Moduli - Attiva / Disattiva i moduli da una lista drop-down list di moduli disponibili.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Blocca Toolbar" alt="Blocca Toolbar" src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Blocca Toolbar - Blocca tutte le toolbar alle loro rispettive zone di bloccaggio.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Ripeti l' ultimo comando" alt="Ripeti l' ultimo comando" src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Ripeti l' ultimo comando - Una scorciatoia per ripetere l' ultimo comando eseguito.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Trova e sostituisci" alt="Trova e sostituisci" src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Trova e sostituisci - Trova (e sostituisce) il testo nell' area del contenuto dell' editor.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Stampa" alt="Pulsante Stampa" src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td>Pulsante Stampa - Stampa il contenuto del Ektron eWebEdit400 o di tutta la pagina web.</td>
		<td>Ctrl+P</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante di controllo ortografia" alt="Pulsante di controllo ortografia" src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td>Pulsante di controllo ortografia - Lancia il controllo ortografia.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pusalnte Taglia" alt="Pusalnte Taglia" src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Pusalnte Taglia - Taglia la selezione del contenuto corrente e lo copia dentro gli appunti.</td>
		<td>Ctrl+X</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Copia" alt="Pulsante Copia" src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td>Pulsante Copia - Copia la selezione del contenuto corrente all' interno degli appunti.</td>
		<td>Ctrl+C</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pusalnte Incollan" alt="Pusalnte Incollan" src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Pusalnte Incollan - Incolla nell' editor il contenuto della parte copiata in precedenza negli appunti.</td>
		<td>Ctrl+V</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Incolla da Word" alt="Pulsante Incolla da Word" src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>Pulsante Incolla da Word - incolla il contenuto copiato da Word e rimuove i tag non conformi al web.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Incolla da Word pulendo i fonts e le dimensioni" alt="Pulsante Incolla da Word pulendo i fonts e le dimensioni" src="<%= this.SkinPath %>Buttons/PasteFromWordNoFontsNoSizes.gif"></td>
		<td>Pulsante Incolla da Word pulendo i fonts e le dimensioni - Pulisce tutti i tag specifici di Word e rimuove i nomi dei font e le loro dimensioni.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Incolla piano" alt="Pulsante Incolla piano" src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Pulsante Incolla piano - Incolla testo piano (senza formattazione) dentro l' editor.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante incolla come HTML" alt="Pulsante incolla come HTML" src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td>Pulsante incolla come HTML - Incolla codice HTML dentro l' area contenuto e mantiene tutti i tag HTML.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Annulla" alt="Pulsante Annulla" src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Pulsante Annulla - Annulla l' ultimo comando.</td>
		<td>Ctrl+Z</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Ripeti" alt="Pulsante Ripeti" src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td>Pulsante Ripeti - Riannulla/Ripete l' ultima azione che si possa annullare.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante di strpper Formato" alt="Pulsante di strpper Formato" src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Pulsante di strpper Formato - Rimuove dal testo selezioneto tutta la formatazzione comprendendo quella personalizzata.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Help Veloce" alt="Help Veloce" src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td>Help Veloce - Lancia l' Help veloce che state vedendo in questo momento.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Informazioni su r.s.d.editor" alt="Informazioni su r.s.d.editor" src="<%= this.SkinPath %>Buttons/AboutDialog.gif"></td>
		<td>Informazioni su r.s.d.editor - Mostra la versione corrente e le credenziali per Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>INSERISCE E GESTISCE LINKS, TABELLE, CARATTERI SPECIALI 
				, IMMAGINI E MEDIA</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Manager Immagini" alt="Pulsante Manager Immagini" src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td>Pulsante Manager Immagini - Inserisce un' immagine da una/alcune cartella/e immagini predefinita/e.</td>
		<td>Ctrl+G</td>
	</tr>
	<tr>
		<td align="middle"><img title="Mappa Immagine" alt="Mappa Immagine" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Mappa Immagine - Permette agli utenti di definire aree cliccabili dentro un' immagine.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante di posizionamento assoluto di un oggetto" alt="Pulsante di posizionamento assoluto di un oggetto" src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Pulsante di posizionamento assoluto di un oggetto - Imposta una posizione assoluta di un oggetto.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante di inserimento tabella" alt="Pulsante di inserimento tabella" src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Pulsante di inserimento tabella - Inserisce una tabella dentro il Ektron eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Imposta i Bordi Tabella" alt="Imposta i Bordi Tabella" src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>Imposta i Bordi Tabella - Imposta i bordi di tutte le tabelle dell' editor.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Inserisci Snippet" alt="Inserisci Snippet" src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Inserisci Snippet - Inserisce predefenite porzioni di codice.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Inserisce un Elemento Form" alt="Inserisce un Elemento Form" src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td>Inserisce un Elemento Form - Inserisce un elemento form da un menù a tendina con tutti gli elementi disponibili.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante inserisce Data" alt="Pulsante inserisce Data" src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Pulsante inserisce Data - Inserisce la data corrente.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante inserisci Ora" alt="Pulsante inserisci Ora" src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Pulsante inserisci Ora - Inserisce l' ora corrente.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Flash Manager" alt="Pulsante Flash Manager" src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td>Pulsante Flash Manager - Inserisce un' animazione Flash e ti permette di impostarne le proprietà.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Gestione Windows Media" alt="Pulsante Gestione Windows Media" src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td>Pulsante Gestione Windows Media - Inserisce un oggetto Windows Media (AVI, MPEG, WAV, 
			etc.) e permette di impostarne le sue proprietà.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Gestione Documenti" alt="Gestione Documenti" src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Gestione Documenti - Inserisce un link ad un documento sul server (PDF, DOC, ecc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Gestione collegamenti ipertestuali" alt="Pulsante Gestione collegamenti ipertestuali" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Pulsante Gestione collegamenti ipertestuali - Imposta il testo selezionato o l' imagine come un collegamento ipertestuale.</td>
		<td>Ctrl+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Rimuove Collegamento Ipertestuale" alt="Pulsante Rimuove Collegamento Ipertestuale" src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td>Pulsante Rimuove Collegamento Ipertestuale - Rimuove il collegamento ipertestuale del testo selezionato o dall' immagine.</td>
		<td>Ctrl+Shift+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Inserisce un carattere speciale da una lista di caratteri" alt="Inserisce un carattere speciale da una lista di caratteri" src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>Inserisce un carattere speciale da una lista di caratteri - Inserisce un carattere speciale (&euro; &reg;, <font face="Arial">
				©, ±</font>, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Menu a tendina di link personalizzati" alt="Menu a tendina di link personalizzati" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Menu a tendina di link personalizzati - Inserisce un link interno o esterno da una lista predefenita.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Scegli un Template HTML" alt="Scegli un Template HTML" src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td>Scegli un Template HTML - Applica un template HTML da una lista predefinita di templates.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>CREA, FORMATTA E MODIFICA PARAGRAFI e LINEE</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Pulsante Inserisci Nuovo Paragrafo" alt="Pulsante Inserisci Nuovo Paragrafo" src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td>Pulsante Inserisci Nuovo Paragrafo - Inserisce un nuovo paragrafo.</td>
		<td>Ctrl+M</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Menu Stili Paragrafo" alt="Bottone Menu Stili Paragrafo" src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Bottone Menu Stili Paragrafo - Applica lo stile di testo standard al testo selezionato.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Diminuisci Rientro" alt="Bottone Diminuisci Rientro" src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td>Bottone Diminuisci Rientro - Diminuisce il rientro del paragrafo verso sinistra.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Aumenta Rientro" alt="Bottone Aumenta Rientro" src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td>Bottone Aumenta Rientro - Aumenta il rientro del paragrafo verso destra.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Allineamento a sinistra" alt="Bottone Allineamento a sinistra" src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td>Bottone Allineamento a sinistra - Allinea il paragrafo selezionato a sinistra.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Allineamento centrato" alt="Bottone Allineamento centrato" src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td>Bottone Allineamento centrato - Allinea il paragrafo selezionato al centro.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Allineamento a destra" alt="Bottone Allineamento a destra" src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Bottone Allineamento a destra - Allinea il paragrafo selezionato a destra.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Allineamento giustificato" alt="Bottone Allineamento giustificato" src="<%= this.SkinPath %>Buttons/JustifyFull.gif"></td>
		<td>Bottone Allineamento giustificato - Giustifica il paragrafo selezionato.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Lista non ordinata" alt="Bottone Lista non ordinata" src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td>Bottone Lista non ordinata - Crea una lista non ordinata dalla selezione.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone lista ordinata" alt="Bottone lista ordinata" src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td>Bottone lista ordinata - Crea una lista numerata dalla selezione.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone Inserisci linea orizzontale (es. un righello orizzonatale)" alt="Bottone Inserisci linea orizzontale (es. un righello orizzonatale)" src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td>Bottone Inserisci linea orizzontale (es. un righello orizzonatale) - Inserisce una linea orizzontale nella posizione del cursore.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>CREA, FORMATTA E MODIFICA IL TESTO, I FONT e LE LISTE</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone grassetto" alt="Bottone grassetto" src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td>Bottone grassetto - Applica il formato grassetto al testo selezionato.</td>
		<td>Ctrl+B</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone corsivo" alt="Bottone corsivo" src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td>Bottone corsivo - Applica il formato corsivo al testo selezionato.</td>
		<td>Ctrl+I</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone sottolineato" alt="Bottone sottolineato" src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td>Bottone sottolineato - Applica il formato sottolineato al testo selezionato.</td>
		<td>Ctrl+U</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone barrato" alt="Bottone barrato" src="<%= this.SkinPath %>Buttons/StrikeThrough.gif"></td>
		<td>Bottone barrato - Applica il formato testo barrato al testo selezionato.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone apice" alt="Bottone apice" src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td>Bottone apice - Rende un testo in formato apice.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone pedice" alt="Bottone pedice" src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td>Bottone pedice - Rende un testo in formato pedice.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone selezione carattere" alt="Bottone selezione carattere" src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td>Bottone selezione carattere - Imposta il tipo di carattere.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone dimensione carattere" alt="Bottone dimensione carattere" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Bottone dimensione carattere - Imposta la dimensione del carattere.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone colore testo (di primo piano)" alt="Bottone colore testo (di primo piano)" src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td>Bottone colore testo (di primo piano) - Cambia il colore di primo piano del testo selezionato.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bottone colore di sfondo" alt="Bottone colore di sfondo" src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td>Bottone colore di sfondo - Cambia il colore di sfondo del testo selezionato</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Menu Stile personalizzato" alt="Menu Stile personalizzato" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Menu Stile personalizzato - Applica uno stile personalizzato o predefinito al testo selezionato.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Formattare un blocco di codice " alt="Formattare un blocco di codice" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Permette agli utenti di inserire e formattare un blocco di codice nel contenuto.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Menu Collegamenti personalizzati" alt="Menu Collegamenti personalizzati" src="<%= this.SkinPath %>Buttons/CustomLinks.gif"></td>
		<td>Menu Collegamenti personalizzati - Inserisce un collegamento personalizzato o predefinito.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>ALTRE SCORCIATOIE DA TASTIERA<strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td>Seleziona tutto il testo, le immagini e le tabelle nell'editor.</td>
		<td>Ctrl+A</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Cerca una stringa di testo o numeri nella pagina.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Chiude la finestra attiva.</td>
		<td>Ctrl+W</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Chiude l'applicazione attiva.</td>
		<td>Ctrl+F4</td>
	</tr>
</table>
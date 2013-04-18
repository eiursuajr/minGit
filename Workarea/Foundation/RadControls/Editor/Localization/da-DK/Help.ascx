<%@ Control Language="C#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.BaseDialogControl"%>

<table border="1" bordercolor="#000000" width="100%" cellpadding="2">
	<tr>
		<td colspan="3" align="middle">GENERAL BUTTONS</td>
	</tr>
	<tr>
		<td align="middle"><img title="Design" alt="Design" src="<%= this.SkinPath %>Img/ButtonDesign.gif"></td>
		<td>Design button - Switches eWebEdit400 into Design Mode.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="HTML" alt="HTML" src="<%= this.SkinPath %>Img/ButtonHtml.gif"></td>
		<td>HTML button - Switches eWebEdit400 into HTML Mode.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Preview" alt="Preview" src="<%= this.SkinPath %>Img/ButtonPreview.gif"></td>
		<td>Preview button - Switches eWebEdit400 into Preview Mode.</td>
		<td>-</td>
	</tr>
	
	<tr>
		<td align="middle"><img title="Convert to upper case" alt="Convert to upper case" src="<%= this.SkinPath %>Buttons/ConvertToUpper.gif"></td>
		<td>Convert the text of the current selection to upper case, preserving the non-text elements such as images and tables.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Convert to lower case" alt="Convert to lower case" src="<%= this.SkinPath %>Buttons/ConvertToLower.gif"></td>
		<td>Convert the text of the current selection to lower case, preserving the non-text elements such as images and tables.
	</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Allow users to create image maps" alt="Allow users to create image maps" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Allow users to create image maps through draging over the images and creating hyperlink areas of different shapes.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Modify an image in the content" alt="Modify an image in the content" src="<%= this.SkinPath %>Buttons/EktronImageModificationTool.gif"></td>
		<td>Modify an image in the content.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Format code blocks" alt="Format code blocks" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Allow users to insert and format code blocks into the content.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Font size" alt="Font size" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Allows the user to apply to the current selection font size measured in pixels, rather than a fixed-size 1 to 7 (as does the FontSize tool).</td>
		<td>-</td>
	</tr>
		
	<tr>
		<td align="middle"><img title="Toggle Screen Mode" alt="Toggle Screen Mode" src="<%= this.SkinPath %>Buttons/ToggleScreenMode.gif"></td>
		<td>Toggle Screen Mode - Switches eWebEdit400 into Full Screen Mode.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Show/Hide Border" alt="Show/Hide Border" src="<%= this.SkinPath %>Buttons/ToggleTableBorder.gif"></td>
		<td>Show/Hide Border - Shows or hides borders around tables in the content area.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Zoom" alt="Zoom" src="<%= this.SkinPath %>Buttons/Zoom.gif"></td>
		<td>Zoom - Changes the level of text magnification.</td>
		<td>-</td>
	</tr>

	<tr>
		<td align="middle"><img title="Module Manager" alt="Module Manager" src="<%= this.SkinPath %>Buttons/ModuleManager.gif"></td>
		<td>Module Manager - Activates /Deactivates modules from a drop-down list of 
			available modules.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Toggle Docking" alt="Toggle Docking" src="<%= this.SkinPath %>Buttons/ToggleDocking.gif"></td>
		<td>Toggle Docking - Docks all floating toolbars to their respective docking areas.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Repeat Last Command" alt="Repeat Last Command" src="<%= this.SkinPath %>Buttons/RepeatLastCommand.gif"></td>
		<td>Repeat Last Command - A short-cut to repeat the last action performed.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Find and Replace" alt="Find and Replace" src="<%= this.SkinPath %>Buttons/FindAndReplace.gif"></td>
		<td>Find and Replace - Find (and replaces) text in the editor's content area.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td align="middle"><img title="Print" alt="Print" src="<%= this.SkinPath %>Buttons/Print.gif"></td>
		<td>Print button - Prints the contents of the eWebEdit400 or the whole web page.</td>
		<td>Ctrl+P</td>
	</tr>
	<tr>
		<td align="middle"><img title="Spell" alt="Spell" src="<%= this.SkinPath %>Buttons/Spellcheck.gif"></td>
		<td>Spell button - Launches the spellchecker.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Cut" alt="Cut" src="<%= this.SkinPath %>Buttons/Cut.gif"></td>
		<td>Cut button - Cuts the selected content and copies it to the clipboard.</td>
		<td>Ctrl+X</td>
	</tr>
	<tr>
		<td align="middle"><img title="Copy" alt="Copy" src="<%= this.SkinPath %>Buttons/Copy.gif"></td>
		<td>Copy button - Copies the selected content to the clipboard.</td>
		<td>Ctrl+C</td>
	</tr>
	<tr>
		<td align="middle"><img title="Paste" alt="Paste" src="<%= this.SkinPath %>Buttons/Paste.gif"></td>
		<td>Paste button - Pastes the copied content from the clipboard into the editor.</td>
		<td>Ctrl+V</td>
	</tr>
	<tr>
		<td align="middle"><img title="Paste from Word" alt="Paste from Word" src="<%= this.SkinPath %>Buttons/PasteFromWord.gif"></td>
		<td>Paste from Word button - Pastes content copied from Word and removes the 
			web-unfriendly tags.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Paste from Word cleaning fonts and sizes" alt="Paste from Word cleaning fonts and sizes" src="<%= this.SkinPath %>Buttons/PasteFromWordNoFontsNoSizes.gif"></td>
		<td>Paste from Word cleaning fonts and sizes button - cleans all Word-specific tags  and removes font names and text sizes.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Paste Plain Text" alt="Paste Plain Text" src="<%= this.SkinPath %>Buttons/PastePlaintext.gif"></td>
		<td>Paste Plain Text button - Pastes plain text (no formatting) into the editor.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Paste as HTML" alt="Paste as HTML" src="<%= this.SkinPath %>Buttons/PasteAsHtml.gif"></td>
		<td>Paste as HTML button - Pastes HTML code in the content area and keeps all the 
			HTML tags.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Undo" alt="Undo" src="<%= this.SkinPath %>Buttons/Undo.gif"></td>
		<td>Undo button - Undoes the last action.</td>
		<td>Ctrl+Z</td>
	</tr>
	<tr>
		<td align="middle"><img title="Redo" alt="Redo" src="<%= this.SkinPath %>Buttons/Redo.gif"></td>
		<td>Redo button - Redoes/Repeats the last action, which has been undone.</td>
		<td>Ctrl+Y</td>
	</tr>
	<tr>
		<td align="middle"><img title="Format Stripper" alt="Format Stripper" src="<%= this.SkinPath %>Buttons/Sweeper.gif"></td>
		<td>Format Stripper button - Removes custom or all formatting from selected text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Quick Help" alt="Quick Help" src="<%= this.SkinPath %>Buttons/Help.gif"></td>
		<td>Quick Help - Launches the Quick Help you are currently viewing.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="About Dialog" alt="About Dialog" src="<%= this.SkinPath %>Buttons/AboutDialog.gif"></td>
		<td>About Dialog - Shows the current version and credentials of eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>INSERT AND MANAGE LINKS, TABLES, SPECIAL 
				CHARACTERS, IMAGES and MEDIA</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Image Manager" alt="Image Manager" src="<%= this.SkinPath %>Buttons/ImageManager.gif"></td>
		<td>Image Manager button - Inserts an image from a predefined image folder(s).</td>
		<td>Ctrl+G</td>
	</tr>
	<tr>
		<td align="middle"><img title="Image map" alt="Image map" src="<%= this.SkinPath %>Buttons/ImageMapDialog.gif"></td>
		<td>Image map - Allows users to define clickable areas within image.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Image Tool" alt="Image Tool" src="<%= this.SkinPath %>Buttons/EktronImageModificationTool.gif"></td>
		<td>Image Tool - Modify an image in the content.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Absolute Object Position" alt="Absolute Object Position" src="<%= this.SkinPath %>Buttons/AbsolutePosition.gif"></td>
		<td>Absolute Object Position button - Sets an absolute position of an object (free 
			move).</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert Table" alt="Insert Table" src="<%= this.SkinPath %>Buttons/InsertTable.gif"></td>
		<td>Insert Table button - Inserts a table in the eWebEdit400.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Toggle Table Borders" alt="Toggle Table Borders" src="<%= this.SkinPath %>Buttons/ToggleBorders.gif"></td>
		<td>Toggle Table Borders - Toggles borders of all tables within the editor.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert Snippet" alt="Insert Snippet" src="<%= this.SkinPath %>Buttons/InsertSnippet.gif"></td>
		<td>Insert Snippet - Inserts pre-defined code snippets.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert Form Element" alt="Insert Form Element" src="<%= this.SkinPath %>Buttons/InsertFormElement.gif"></td>
		<td>Insert Form Element - Inserts a form element from a drop-down list with 
			available elements.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert Date" alt="Insert Date" src="<%= this.SkinPath %>Buttons/InsertDate.gif"></td>
		<td>Insert Date button - Inserts current date.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert Time" alt="Insert Time" src="<%= this.SkinPath %>Buttons/InsertTime.gif"></td>
		<td>Insert Time button - Inserts current time.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Flash Manager" alt="Flash Manager" src="<%= this.SkinPath %>Buttons/FlashManager.gif"></td>
		<td>Flash Manager button - Inserts a Flash animation and lets you set its 
			properties.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Windows Media Manager" alt="Windows Media Manager" src="<%= this.SkinPath %>Buttons/MediaManager.gif"></td>
		<td>Windows Media Manager button - Inserts a Windows media object (AVI, MPEG, WAV, 
			etc.) and lets you set its properties.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Document Manager" alt="Document Manager" src="<%= this.SkinPath %>Buttons/DocumentManager.gif"></td>
		<td>Document Manager - Inserts a link to a document on the server (PDF, DOC, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Hyperlink Manager" alt="Hyperlink Manager" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Hyperlink Manager button - Makes the selected text or image a hyperlink.</td>
		<td>Ctrl+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Remove Hyperlink" alt="Remove Hyperlink" src="<%= this.SkinPath %>Buttons/Unlink.gif"></td>
		<td>Remove Hyperlink button - Removes the hyperlink from the selected text or 
			image.</td>
		<td>Ctrl+Shift+K</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert Special Character dropdown" alt="Insert Special Character dropdown" src="<%= this.SkinPath %>Buttons/Symbols.gif"></td>
		<td>Insert Special Character dropdown - Inserts a special character (&euro; &reg;, <font face="Arial">
				, </font>, etc.)</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert Custom Link dropdown" alt="Insert Custom Link dropdown" src="<%= this.SkinPath %>Buttons/LinkManager.gif"></td>
		<td>Insert Custom Link dropdown - Inserts an internal or external link from a 
			predefined list.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Choose HTML Template" alt="Choose HTML Template" src="<%= this.SkinPath %>Buttons/TemplateManager.gif"></td>
		<td>Choose HTML Template - Applies and HTML template from a predefined list of 
			templates.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>CREATE, FORMAT AND EDIT PARAGRAPHS and LINES</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert New Paragraph" alt="Insert New Paragraph" src="<%= this.SkinPath %>Buttons/InsertParagraph.gif"></td>
		<td>Insert New Paragraph button - Inserts new paragraph.</td>
		<td>Ctrl+M</td>
	</tr>
	<tr>
		<td align="middle"><img title="Paragraph Style Dropdown" alt="Paragraph Style Dropdown" src="<%= this.SkinPath %>Buttons/Paragraph.gif"></td>
		<td>Paragraph Style Dropdown button - Applies standard text styles to selected 
			text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Outdent" alt="Outdent" src="<%= this.SkinPath %>Buttons/Outdent.gif"></td>
		<td>Outdent button - Indents paragraphs to the left.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Indent" alt="Indent" src="<%= this.SkinPath %>Buttons/Indent.gif"></td>
		<td>Indent button - Indents paragraphs to the right.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Align Left" alt="Align Left" src="<%= this.SkinPath %>Buttons/JustifyLeft.gif"></td>
		<td>Align Left button - Aligns the selected paragraph to the left.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Center" alt="Center" src="<%= this.SkinPath %>Buttons/JustifyCenter.gif"></td>
		<td>Center button - Aligns the selected paragraph to the center.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Align Right" alt="Align Right" src="<%= this.SkinPath %>Buttons/JustifyRight.gif"></td>
		<td>Align Right button - Aligns the selected paragraph to the right.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Justify" alt="Justify" src="<%= this.SkinPath %>Buttons/JustifyFull.gif"></td>
		<td>Justify button - Justifies the selected paragraph.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Bulleted List" alt="Bulleted List" src="<%= this.SkinPath %>Buttons/InsertUnorderedList.gif"></td>
		<td>Bulleted List button - Creates a bulleted list from the selection.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Numbered List" alt="Numbered List" src="<%= this.SkinPath %>Buttons/InsertOrderedList.gif"></td>
		<td>Numbered List button - Creates a numbered list from the selection.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Insert horizontal line" alt="Insert horizontal line" src="<%= this.SkinPath %>Buttons/InsertHorizontalRule.gif"></td>
		<td>Insert horizontal line (e.g. horizontal rule) button - Inserts a horizontal 
			line at the cursor position.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>CREATE, FORMAT AND EDIT TEXT, FONT and LISTS</strong></td>
	</tr>
	<tr>
		<td align="middle"><img title="Bold" alt="Bold" src="<%= this.SkinPath %>Buttons/Bold.gif"></td>
		<td>Bold button - Applies bold formatting to selected text.</td>
		<td>Ctrl+B</td>
	</tr>
	<tr>
		<td align="middle"><img title="Italic" alt="Italic" src="<%= this.SkinPath %>Buttons/Italic.gif"></td>
		<td>Italic button - Applies italic formatting to selected text.</td>
		<td>Ctrl+I</td>
	</tr>
	<tr>
		<td align="middle"><img title="Underline" alt="Underline" src="<%= this.SkinPath %>Buttons/Underline.gif"></td>
		<td>Underline button - Applies underline formatting to selected text.</td>
		<td>Ctrl+U</td>
	</tr>
	<tr>
		<td align="middle"><img title="Strikethrough" alt="Strikethrough" src="<%= this.SkinPath %>Buttons/StrikeThrough.gif"></td>
		<td>Strikethrough button - Applies strikethrough formatting to selected text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Superscript" alt="Superscript" src="<%= this.SkinPath %>Buttons/Superscript.gif"></td>
		<td>Superscript button - Makes a text superscript.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Subscript" alt="Subscript" src="<%= this.SkinPath %>Buttons/Subscript.gif"></td>
		<td>Subscript button - Makes a text subscript.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Font Select" alt="Font Select" src="<%= this.SkinPath %>Buttons/FontName.gif"></td>
		<td>Font Select button - Sets the font typeface.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Font Size" alt="Font Size" src="<%= this.SkinPath %>Buttons/FontSize.gif"></td>
		<td>Font Size button - Sets the font size.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Text Color (foreground)" alt="Text Color (foreground)" src="<%= this.SkinPath %>Buttons/ForeColor.gif"></td>
		<td>Text Color (foreground) button - Changes the foreground color of the selected 
			text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Text Color (background)" alt="Text Color (background)" src="<%= this.SkinPath %>Buttons/BackColor.gif"></td>
		<td>Text Color (background) button - Changes the background color of the selected 
			text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Custom Styles dropdown" alt="Custom Styles dropdown" src="<%= this.SkinPath %>Buttons/Class.gif"></td>
		<td>Custom Styles dropdown - Applies custom, predefined styles to the selected 
			text.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Format code blocks" alt="Format code blocks" src="<%= this.SkinPath %>Buttons/FormatCodeBlock.gif"></td>
		<td>Allow users to insert and format code blocks into the content.</td>
		<td>-</td>
	</tr>
	<tr>
		<td align="middle"><img title="Custom Links dropdown" alt="Custom Links dropdown" src="<%= this.SkinPath %>Buttons/CustomLinks.gif"></td>
		<td>Custom Links dropdown - Inserts custom, predefined link.</td>
		<td>-</td>
	</tr>
	<tr>
		<td colspan="3" align="middle"><strong>OTHER KEYBOARD SHORTCUTS</strong></td>
	</tr>
	<tr>
		<td>-</td>
		<td>Selects all text, images and tables in the editor.</td>
		<td>Ctrl+A</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Finds a string of text or numbers in the page.</td>
		<td>Ctrl+F</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Closes the active window.</td>
		<td>Ctrl+W</td>
	</tr>
	<tr>
		<td>-</td>
		<td>Closes the active application.</td>
		<td>Ctrl+F4</td>
	</tr>
</table>
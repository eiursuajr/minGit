<%@ Control Language="c#" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<style>
	body, td, option {
		font-family: Verdana;
		font-size: 11px;
	}
</style>
<table width ="100%" class="MainTable">
	<tr>
		<td colspan="3" class="text"><script>localization.showText('PasteSourceCode');</script></td>
	</tr>
	<tr>
		<td colspan="3">
			<textarea id="SourceCode" style="width:100%;height:180;"></textarea>
		</td>
	</tr>

	<tr>
		<td colspan="3">
		<table cellpadding="0" cellspacing="0" border="0" width="100%">
			<tr>
				<td>
					<label title="Code Language" for="CodingLanguage"><script>localization.showText('CodeLanguage');</script></label>
				</td>
				<td>
					<select id="CodingLanguage" title="Select Coding Language from the DropDown Menu below">
						<option title="Select Markup - (x)HTML, XML, ASPX, ... as the Coding Type" value ="Xml">Markup - (x)HTML, XML, ASPX, ...</option>
						<option title="Select JavaScript as the Coding Type" value ="JScript">JavaScript</option>
						<option title="Select CSS as the Coding Type" value ="CSS">CSS</option>
						<option title="Select C# as the Coding Type" value ="CSharp">C#</option>		
						<option title="Select VB as the Coding Type" value ="Vb">VB</option>	
						<option title="Select Php as the Coding Type" value ="Php">Php</option>
						<option title="Select SQL as the Coding Type" value ="Sql">SQL</option>
						<option title="Select Delphi as the Coding Type" value ="Delphi">Delphi</option>
						<option title="Select Python as the Coding Type" value ="Python">Python</option>
						
					</select>
				</td>
				<td>
					<label title="Maximum Snippet Height" for="SnippetMaxHeight"><script>localization.showText('SnippetMaxHeight');</script></label>
				</td>
				<td>
					<input title="Enter Maximum Snippet Height Value" type="text" id="SnippetMaxHeight" value ="500" style="width:50px">
				</td>
				<td> px/%</td>

				<td><span id = "InfoLabel" style = "background-color:white"></span></td>

			</tr>
			<tr>
				<td>
					<label title="Show Line Numbers" for="ShowLineNumbers"><script>localization.showText('ShowLineNumbers');</script></label>
				</td>
				<td>
					<input title="Enable/Disable Visible Line Numbers" type = "checkbox" title="Show Line Numbers" name="ShowLineNumbers" id="ShowLineNumbers">
				</td>

				<td>
					<label title="Maximum Snippet Width" for="SnippetMaxWidth"><script>localization.showText('SnippetMaxWidth');</script></label>
				</td>
				<td>
					<input title="Enter Maximum Snippet Width Value" type="text" id="SnippetMaxWidth" value ="100%" style="width:50px">
				</td>
				<td> px/%</td>
				<td>
					<button class ="Button" title="Preview" onmousedown="document.getElementById('InfoLabel').innerHTML = localization.getText('Working');" onmouseup="PerformHighliting()" onclick="return false;"><script>localization.showText('Preview');</script></button>
				</td>
			</tr>
		</table>
		</td>
	</tr>

	<tr>
		<td colspan="3" class="text">
			
			<script>localization.showText('PreviewCode');</script>
			<style>
				.codesnippet td {
					font-family: Courier New;font-size: 11px;
				}
			</style>
			<div id = "FormattedCode" style = "overflow:auto;background-color:white;height:180px;border:solid 1px #7f9db9;width:690px;line-height: 100% !important;" class="codesnippet"></div>
		</td>
	</tr>
</table>

<div align="right">
<button class ="Button" title="Insert" onclick="insertCode()"><script>localization.showText('Insert');</script></button>
<button class ="Button" title="Cancel" onclick="Cancel()"><script>localization.showText('Cancel');</script></button>
</div>
<script>
	var arr = [];
	var isCodeHighlighted = false;
	// the actual highlite is done here
	function PerformHighliting() {
	
		// get the selected coding language from the dropdown
		var clDD = document.getElementById("CodingLanguage");
		var language = clDD.options[clDD.selectedIndex].value;
		
		// get not formatted source code
		var code = document.getElementById('SourceCode').value;
		
		// do we have to show line numbers?
		var showLineNumbers = document.getElementById('ShowLineNumbers').checked;
		
		// get the formatted code from the plain code 
		var formattedCode = SyntaxHighlighter.HighlightAll(code, language, showLineNumbers);

		// show the formatted code to user
		document.getElementById("FormattedCode").innerHTML   = formattedCode;

		document.getElementById("InfoLabel").innerHTML = "";
		isCodeHighlighted = true;
	}

</script>
	 <script language="javascript">
		function GetDialogArguments()
		{
			if (window.radWindow) 
			{
				return window.radWindow.Argument;
			}
			else
			{
				return null;
			}
		}

		var isRadWindow = true;
		var radWindow = GetEditorRadWindowManager().GetCurrentRadWindow(window);
		if (radWindow)
		{ 
			if (window.dialogArguments) 
			{ 
				radWindow.Window = window;
			} 
			radWindow.OnLoad(); 
		}
		
		// after we are done - call the custom callback and
		// pass the formatted code as parameter
		function insertCode()
		{
			// make sure the code is highlighted before to insert it
			if (!isCodeHighlighted) {
				PerformHighliting();
			}
		
			var codeHolder = document.getElementById("FormattedCode");
			var codeTable = codeHolder.getElementsByTagName('table')[0];
			
			// get the maxWidth/maxHeight of the code snippet
			var width = document.getElementById('SnippetMaxWidth').value;
			var heightOffset = 20;
			var height = codeTable.offsetHeight + heightOffset;
			
			var maxHeight = document.getElementById('SnippetMaxHeight').value;
			height = (height > maxHeight) ? maxHeight : height;
			
			// pass here the formatted code
			var returnValue = 
			{
				formattedCode : "<div style='overflow:auto;background-color:white;border:solid 1px #7f9db9;width:" + width + ";height:" + height + ";line-height: 100% !important;font-family: Courier New;font-size: 11px;'>" + document.getElementById("FormattedCode").innerHTML + "</div>"
			};

			// close the dialog and supply the formatted code to the callback function
			CloseDlg(returnValue);
		}
		
		function Cancel() {
			CloseDlg();
		}
		
	</script>

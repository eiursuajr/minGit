<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ColorPicker.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.ColorPicker" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script language="javascript">
	/************************************************
		*
		*	Default ColorPicker Pallette
		*
		************************************************/ 	 
	var defaultColorsArray = new Array(
		"",		 "#ffff00", "#00ff00", "#add8e6", "#008000", "#808080",
		"#ffd700", "#ffe4e1", "#00ffff", "#87ceeb", "#0000ff", "#a9a9a9",
		"#ffa500", "#ffc0cb", "#a52a2a", "#008080", "#000080", "#c0c0c0",
		"#ff0000", "#c71585", "#8b0000", "#4b0082", "#000000", "#ffffff"
	);

	/************************************************
		*
		*	General helpers
		*
		************************************************/	 	 
/*	function SelectColor(cp, color, hide)
	{
		if (cp)
		{
			cp.SelectColor(color, hide);
		}
	}
*/
</script>
<button id='MENU_BUTTON_<%=this.ClientID%>' 
		class="ColorPickerMainButton"
		onmouseover="this.className='ColorPickerMainButtonOver'" 
		onmouseout="this.className='ColorPickerMainButton'" 
		onclick="<%=this.ClientID%>.Toggle();return false;" 
		onfocus="this.blur();">			
	<div style="margin-left:4px;padding-bottom:3px;" id='MENU_BUTTON_SPAN_<%=this.ClientID%>' class="ColorPickerMenuSpan">&nbsp;</div>
</button>	
<div class="MenuElement" id="MENU_ELEMENT_<%=this.ClientID%>" style="visibility:hidden">
	<table id="COLOR_TABLE_<%=this.ClientID%>" width="110px" class="RadEColorPicker" cellspacing="1" cellpadding="0"></table>
</div>
<object id="dlgHelper" CLASSID="clsid:3050f819-98b5-11cf-bb82-00aa00bdce0b" width="0px" height="0px">
</object>
<script>
	var menu_<%=this.ClientID%> = new DropDownMenu(document.getElementById('MENU_BUTTON_<%=this.ClientID%>'), document.getElementById('MENU_ELEMENT_<%=this.ClientID%>'));
	var <%=this.ClientID%> = new ColorPicker('<%=this.ClientID%>', '<%=this.SkinPath%>', menu_<%=this.ClientID%>, defaultColorsArray);
</script>
<%@ Control CodeBehind="CssClassSelector.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="Ektron.Telerik.WebControls.EditorControls.CssClassSelector" %>

<% if (Page.Request.Browser.Browser == "xIE") { %>
<table	id="CssClassSelector_Table_<%=this.ClientID%>"
		cellspacing="0"
		cellpadding="0"
		border="0"
		class="RadEDropDown" 
		tabindex="0"
		onkeydown="OnTableKeyDown(e);" 
		onclick="<%=this.ClientID%>.TogglePopup(); return true;" 
		onmouseover="this.className='RadEDropDownOver'" 
		onmouseout="this.className='RadEDropDown'" 
		style="position:relative; display:inline;" 
		align="absmiddle" >
	<tr>
		<td nowrap
			id="CssClassSelector_Label_<%=this.ClientID%>" 
			style="padding-top:1px;padding-bottom:1px;padding-left:3px;padding-right:16px;">11</td>
		<td nowrap><img align="absmiddle" id="radEditorDropDownImg" src="<%=this.SkinPath%>Buttons/arrowDropdown.gif" /></td>
	</tr>
</table>
<% } else { %>
<style>
    SELECT.DropDown1
    {
		height:19px;
		width:100px;
		FONT-FAMILY: Tahoma;
		padding-left:2px;
		font-size:11px;
		margin:2px;
    }
    
	SELECT.DropDown1 OPTION
	{
		margin: 5px;
		width:70px;
		overflow:hidden;
		background-repeat:no-repeat;
		padding-left:20px;
	}
</style>

<select id="CssClassSelector_<%=this.ClientID%>" class="DropDown1">
</select>

<% } %>

<script language="javascript">	
<!--	
	var <%=this.ClientID%> = new CssClassSelector<%=(Page.Request.Browser.Browser == "xIE") ? "" : "NS" %> (
		'<%=this.ClientID%>'
		, eval('<%=this.RadCssClassArray%>')
		, '<%=this.CssFilter%>'
		, '<%=this.PopupWidth%>'
		, '<%=this.PopupHeight%>'
		, '<%=this.SkinPath%>');
	// ektron start
	function OnTableKeyDown(event)
	{
	    event = (event ? event : window.event);
	    switch (event.keyCode)
	    {
	        case 13:
	            <%=this.ClientID%>.TogglePopup(); 
	            window.setTimeout(function() {
	                if (<%=this.ClientID%>.PopupWnd)
	                {
	                    $ektron("td", <%=this.ClientID%>.PopupWnd).each( function(i, e)
                        {//debugger;
                            $ektron(this).attr("tabindex", 0); 
                        });
	                    $ektron("td:first", <%=this.ClientID%>.PopupWnd).focus();
	                }
	            }, 1);
	            return true;
	            break;
//	        case 40: //down arrow
//	            break;
	    }
	}
	// ektron end
//-->
</script>
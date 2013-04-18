<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="htmlchoicesfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.HtmlChoicesField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldValidateControl" Src="ucFieldValidation.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDataStyleControl" Src="ucFieldDataStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="radcb" Namespace="Telerik.WebControls" Assembly="RadComboBox.NET2" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Choices Field</title>
<style type="text/css">
    #ItemListDiv
    {
        overflow:auto;
        height:125px; /* needs to be shorter to prevent IE6 bug */
    }
    #ItemListDiv table th 
    {
        font-size: .87em; /* prevents horizontal scrolling */
    }
</style>
</head>
<body class="dialog">
<form id="Form1" runat="server">
    
    <div class="Ektron_DialogTabstrip_Container">
		<radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
					OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
            <Tabs>
                <radTS:Tab ID="General" Text="General" Value="General" />
                <radTS:Tab ID="Validation" Text="Validation" Value="Validation" />
                <radTS:Tab ID="DataStyle" Text="Data Style" Value="DataStyle" />
            </Tabs>
        </radTS:RadTabStrip>
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
	            <table width="100%">
				<tbody>
	            <ek:FieldNameControl ID="Name" runat="server" IndexedEnabled="false" />
	            <tr>
	                <td><label title="List" for="cboList" id="lblList" runat="server">List:</label></td>
	                <td colspan="2">
                        <radcb:RadComboBox id="cboList" runat="server"
                            Width="24.5em"
                            OnClientSelectedIndexChanged="OnClientSelectedIndexChanged"
                            AllowCustomText="True" />                    
	                </td>
	            </tr>
	            <tr>
	                <td colspan="3">
	                    <table style="width:100%" class="Ektron_TopSpaceSmall">
	                    <tbody>
                        <tr>
                            <td style="width:50%">
                                <fieldset>
                                <legend title="Allow Selection" id="lblAllowSelection" runat="server">Allow Selection</legend>
                                <div>
                                    <input title="Select <%=GetMessage("lbl only one") %>" type="radio" name="optAllow" id="optAllowOne" value="True" checked="checked" /><label title="Only one" for="optAllowOne" id="lblOnlyOne" runat="server">Only one</label><br />
                                    <input title="Select <%=GetMessage("lbl more than one") %>" type="radio" name="optAllow" id="optAllowMulti" value="False" /><label title="More than one" for="optAllowMulti" id="lblMoreThanOne" runat="server">More than one</label><br />
                                    <input title="Option to make a selection required" type="checkbox" name="chkSelRequired" id="chkSelRequired" /><label title="A selection is required" for="chkSelRequired" id="lblSelectionReqd" runat="server">A selection is required</label><br />
                                    <input title="Option to make the first item an invalid selection" type="checkbox" name="chkItem1Invalid" id="chkItem1Invalid" onclick="item1InvalidChanged(this.checked);" /><label title="First item is not a valid selection" for="chkItem1Invalid" id="lblFirstNotValid" runat="server">First item is not a valid selection</label><br />
                                </div>
                                </fieldset>
	                        </td>
	                        <td>&nbsp;</td>
	                        <td>
								<fieldset>
								<legend title="Appearance" id="lblAppearance" runat="server">Appearance</legend>
								<div>
									<input title="Select <%=GetMessage("lbl vert list") %>" type="radio" name="optDisplay" id="displayVerticalList" value="VerticalList" onclick="updateOptions();" checked="checked" /><label title="Vertical List" for="displayVerticalList" id="lblVertList" runat="server">Vertical List</label><br />
									<input title="Select <%=GetMessage("lbl horz list") %>" type="radio" name="optDisplay" id="displayHorizontalList" value="HorizontalList" onclick="updateOptions();" /><label title="Horizontal List" for="displayHorizontalList" id="lblHorzList" runat="server">Horizontal List</label><br />
									<input title="Select <%=GetMessage("lbl list box") %>" type="radio" name="optDisplay" id="displayListBox" value="ListBox" onclick="updateOptions();" /><label title="List Box" for="displayListBox" id="lblListBox" runat="server">List Box</label><br />
									<input title="Select <%=GetMessage("lbl drop list") %>" type="radio" name="optDisplay" id="displayDropList" value="DropList" onclick="updateOptions();" /><label title="Drop List" for="displayDropList" id="lblDropList" runat="server">Drop List</label><br />
								</div>
								</fieldset>
	                        </td>
                        </tr>
                        </tbody>
	                    </table>
	                </td>
	            </tr>
	            </tbody>
		        </table>
		        <div class="Ektron_TopSpaceVeryVerySmall" id="design_content">
                    <fieldset>
                    <legend id="lblItemList" runat="server" title="Item List">Item List</legend>
                    <div id="ItemListDiv">
                        <table id="select" summary="Select" border="0" ektdesignns_name="select" ektdesignns_caption="select" ektdesignns_nodetype="element" ektdesignns_role="root">
                        <thead>
						<tr>
							<th> </th> 
							<th id="lblSelected" title="Selected" runat="server">Selected</th> 
							<th id="lblDisplayText" title="Display Text" runat="server">Display Text</th> 
							<th id="lblValue" title="Value" runat="server">Value</th> 
							<th id="lblDisabled" title="Disabled" runat="server">Disabled</th>
                        </tr> 
                        </thead>
                        <tfoot class="design_prototype">
                        <tr onclick="design_row_setCurrent(event, this)">
                            <td colspan="4" unselectable="on">
                                <a href="#" onclick="design_row_setCurrent(event, $ektron(this).parent().parent().get(0));design_row_replace();return false;" menutype="button" class="design_dynlist_menu">
                                <img class="design_add_graphic" menutype="button" src="[skinpath]additem.gif" width="9" height="9" border="0" /> 
                                <asp:Literal id="lblOption2" runat="server" />
                                </a>
                            </td>
                        </tr>
                        <tr onclick="design_row_setCurrent(event, this)" id="option" ektdesignns_name="option" ektdesignns_caption="option" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
                            <td class="design_dynlist_first_normal" unselectable="on">
                                <a href="#" onclick="design_row_showContextMenu(event, this);return false;" menutype="button" class="design_dynlist_menu" onmouseover="design_row_onmouse(event, this)" onmouseout="design_row_onmouse(event, this)">
                                <img class="design_contextmenu_button" menutype="button" src="[skinpath]designmenu.gif" width="11" height="16" border="0" /> 
                                </a>
                            </td>
                            <td>  
                                <input xml:space="default" id="selected" runat="server" title="Select Item Option" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="selected" ektdesignns_nodetype="attribute" onclick="UpdateItemSelected(this);" /> 
                            </td>
                            <td>
                                <input value="" class="RadETextBox" id="Text" runat="server" title="Enter Text to Display for Item here" onblur="design_validate_re(/\S+/,this,'Cannot be blank');" maxlength="1500" ektdesignns_name="Text" ektdesignns_caption="Text" ektdesignns_nodetype="text" ektdesignns_validation="string-req" ektdesignns_datatype="string" ektdesignns_basetype="text" ektdesignns_schema="&lt;xs:minLength value='1'/&gt;" ektdesignns_validate="re:/\S+/" ektdesignns_invalidmsg="Cannot be blank" /> 
                            </td>
                            <td>
                                <input value="" class="RadETextBox" id="value" runat="server" title="Enter Item Value here" onblur="validateChoicesFieldItemValue(this);" maxlength="50" ektdesignns_name="value" ektdesignns_caption="value" ektdesignns_nodetype="attribute" ektdesignns_validation="choicesField" /> 
                            </td>
                            <td>
                                <input xml:space="default" id="disabled" runat="server" title="Disable/Enable Item" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="disabled" ektdesignns_nodetype="attribute" onclick="UpdateItemDisabled(this);" /> 
                            </td>
                        </tr>
                        </tfoot>
                        <tbody ektdesignns_list="true">
						<tr onclick="design_row_setCurrent(event, this)" ektdesignns_name="option" ektdesignns_caption="option" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
                            <td class="design_dynlist_first_normal" unselectable="on">
								<a href="#" onclick="design_row_showContextMenu(event, this);return false;" menutype="button" class="design_dynlist_menu" onmouseover="design_row_onmouse(event, this)" onmouseout="design_row_onmouse(event, this)">
								<img class="design_contextmenu_button" menutype="button" src="[skinpath]designmenu.gif" width="11" height="16" border="0" /> 
                                </a>
                            </td>
                            <td>
								<input xml:space="default" id="selected1" runat="server" title="Select Item Option" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="selected" ektdesignns_nodetype="attribute" onclick="UpdateItemSelected(this);" /> 
                            </td>
                            <td>
								<input value="" class="RadETextBox" id="Text1" runat="server" title="Enter Text to Display for Item here" onblur="design_validate_re(/\S+/,this,'Cannot be blank');" maxlength="1500" ektdesignns_name="Text" ektdesignns_caption="Text" ektdesignns_nodetype="text" ektdesignns_validation="string-req" ektdesignns_datatype="string" ektdesignns_basetype="text" ektdesignns_schema="&lt;xs:minLength value='1'/&gt;" ektdesignns_validate="re:/\S+/" ektdesignns_invalidmsg="Cannot be blank" /> 
                            </td>
                            <td>
								<input value="" class="RadETextBox" id="value1" runat="server" title="Enter Item Value here" onblur="validateChoicesFieldItemValue(this);" maxlength="50" ektdesignns_name="value" ektdesignns_caption="value" ektdesignns_nodetype="attribute" ektdesignns_validation="choicesField" /> 
                            </td>
                            <td>
								<input xml:space="default" id="disabled1" runat="server" title="Disable/Enable Item" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="disabled" ektdesignns_nodetype="attribute" onclick="UpdateItemDisabled(this);" /> 
                            </td>
                        </tr>
                        <tr onclick="design_row_setCurrent(event, this)">
                            <td colspan="5" unselectable="on">
                                <a href="#" onclick="design_row_setCurrent(event, $ektron(this).parent().parent().get(0));design_row_insertAbove();return false;" menutype="button" class="design_dynlist_menu">
								<img class="design_add_graphic" menutype="button" src="[skinpath]additem.gif" width="9" height="9" border="0" alt="Add Item" title="Add Item" /> 
                                <asp:Literal id="lblOption" runat="server" />
                                </a>
                            </td>
                        </tr>
						</tbody>
						</table>
					</div>
					</fieldset>
				</div>
	        </radTS:PageView>
            <radTS:PageView id="Pageview2" runat="server" >
                <table width="100%">
					<ek:FieldValidateControl ID="validateControl" runat="server" Enabled="true" />
                </table> 
            </radTS:PageView>
	        <radTS:PageView id="Pageview3" runat="server" >
                <table width="100%">
                    <ek:FieldDataStyleControl ID="FieldDataStyleControl1" runat="server" />
                </table> 
            </radTS:PageView>
        </radTS:RadMultiPage>
	</div>
	
    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>		
	
	<ek:FieldDialogButtons ID="FieldDialogButtons1" OnOK="return insertField();" runat="server" />
</form>
<script language="javascript" type="text/javascript">
<!--
    if ("undefined" == typeof cboList) cboList = <%= cboList.ClientID %>;
    if ("undefined" == typeof RadTabStrip1) RadTabStrip1 = <%= RadTabStrip1.ClientID %>;
    RadTabStrip1.ClientID = "<%= RadTabStrip1.ClientID %>";

	var ResourceText = 
	{
		sOptionsReqd: "<asp:literal id="sOptionsReqd" runat="server"/>"
	,	sFirstNotValid: "<asp:literal id="sFirstNotValid" runat="server"/>"
	,	sCannotBeBlank: "<asp:literal id="sCannotBeBlank" runat="server"/>"
	,   sFirstRowText: "<asp:literal id="sFirstRowText" runat="server"/>"
	};

    var path = window.location.href;
    var g_srcPath = path.substr(0, path.length - "dialogs/htmlchoicesfield.aspx".length);
    var g_skinPath = "<%=ResolveUrl(this.SkinControlsPath)%>ContentDesigner/";
    
    Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("div#RadTabStrip1 a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
//-->
</script>
</body>
</html>

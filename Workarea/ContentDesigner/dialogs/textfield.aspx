<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="textfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.TextField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldValidateControl" Src="ucFieldValidation.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldUseControl" Src="ucFieldUse.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldAllowControl" Src="ucFieldAllow.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldStyleControl" Src="ucFieldStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDataStyleControl" Src="ucFieldDataStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>
<%@ Register TagPrefix="ek" TagName="FieldAdvancedControl" Src="ucFieldAdvanced.ascx" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Text Field</title>
<style type="text/css">
    .SubOptions
    {
    	float: right;
    	width: 48%;
    }
</style>
</head>
<body class="dialog">
<form id="Form1" runat="server">
    
    <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
			OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
        <Tabs>
            <radTS:Tab ToolTip="General" ID="General" Text="General" Value="General" />
            <radTS:Tab ToolTip="Validation" ID="Validation" Text="Validation" Value="Validation" />
            <radTS:Tab ToolTip="Data Style" ID="DataStyle" Text="Data Style" Value="DataStyle" />
            <radTS:Tab ToolTip="Advanced" ID="Advanced" Text="Advanced" Value="Advanced" />
        </Tabs>
    </radTS:RadTabStrip> 
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
	            <table width="100%">
		            <ek:FieldNameControl ID="Name" runat="server" />
		            <tr>
		                <td><label title="Default value" for="txtDefVal" class="Ektron_StandardLabel" id="lblDefVal" runat="server">Default value:</label></td>
		                <td colspan="2">
		                    <div id="RichAreaDefaultContainer" style="display:none">
					            <textarea title="Enter Default value here (Multiple Lines)" id="richDefVal" name="richDefVal" rows="3" cols="24"></textarea>
		                    </div> 
		                    <div id="MultilineDefaultContainer" style="display:none">
					            <textarea title="Enter Default value here (Multiple Lines)" id="taDefVal" name="taDefVal" rows="3" cols="24"></textarea>
		                    </div> 
		                    <div id="SingleLineDefaultContainer" style="display:block">
					            <input title="Enter Default value here" type="text" id="txtDefVal" name="txtDefVal" value="" maxlength="2000" class="Ektron_StandardTextBox" />
		                    </div>
                        </td>
		            </tr>
		            <tr>
		                <td colspan="3" style="width: 100%">
	                        <ek:FieldStyleControl ID="FieldStyleControl" runat="server" />
		                </td>
		            </tr>
		            <tr>
		                <td colspan="3">
		                    <table style="width:100%" class="Ektron_TopSpaceVeryVerySmall">
	                            <tr>
	                                <td style="width:50%">
	                                    <ek:FieldUseControl ID="Use" runat="server" />
		                            </td>
		                            <td>&nbsp;&nbsp;</td>
	                                <td style="width:50%">
	                                    <ek:FieldAllowControl ID="Allow" runat="server" />
		                            </td>
	                            </tr>
		                    </table>
		                </td>
		            </tr>
		            <tr>
		                <td colspan="3" style="width: 100%">
		                    <fieldset class="Ektron_TopSpaceVeryVerySmall">
		                        <legend id="lblOptions" runat="server">Options</legend>
		                        <div class="Ektron_TopSpaceVeryVerySmall">
                                    <asp:CheckBox ID="chkKeepPTag" ToolTip="Retain single paragraph in Rich Formatting field" runat="server" CssClass="SubOptions" Enabled="false" />
                                    <asp:CheckBox ID="chkRichArea" ToolTip="Allow Rich Formtting Option" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" /> 
                                    <asp:CheckBox ID="chkMultiline" ToolTip="Allow Multiple Lines Option" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
                                    <asp:CheckBox ID="chkReadOnly" ToolTip="Cannot be Changed Option" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
                                    <asp:CheckBox ID="chkHidden" ToolTip="Invisible Option" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
                                    <asp:CheckBox ID="chkPassword" ToolTip="Password field Option"  runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
		                        </div> 
		                    </fieldset>
		                </td>
		            </tr>
	            </table>
	        </radTS:PageView>
	        <radTS:PageView id="Pageview2" runat="server">
                <table width="100%">
					<ek:FieldValidateControl ID="validateControl" runat="server" Enabled="true" />
                </table> 
	        </radTS:PageView>
	        <radTS:PageView id="Pageview3" runat="server">
                <table width="100%">
                    <ek:FieldDataStyleControl runat="server" />
                </table> 
	        </radTS:PageView>
	        <radTS:PageView id="Pageview4" runat="server">
                <table width="100%">
                    <ek:FieldAdvancedControl ID="fieldAdvanced" NodeTypeVisible="true" runat="server" />
                </table> 
            </radTS:PageView>
        </radTS:RadMultiPage>
    </div>
    
    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>	    
    
    <ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />
</form> 
<script language="javascript" type="text/javascript">
<!--
    if ("undefined" == typeof RadTabStrip1) RadTabStrip1 = <%= RadTabStrip1.ClientID %>;
    RadTabStrip1.ClientID = "<%= RadTabStrip1.ClientID %>";
    
	var ResourceText = 
	{
		sValue: "<asp:literal id="sValue" runat="server"/>"
	,	sInvalidDefVal: "<asp:literal id="sInvalidDefVal" runat="server"/>"
	,	sSize: "<asp:literal id="sSize" runat="server"/>"
    ,   sWidth: "<asp:literal id="sWidth" runat="server"/>"
    ,   sHeight: "<asp:literal id="sHeight" runat="server"/>"
	};

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

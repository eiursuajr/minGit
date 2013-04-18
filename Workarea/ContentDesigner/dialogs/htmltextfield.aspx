<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="htmltextfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.HtmlTextField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldValidateControl" Src="ucFieldValidation.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldStyleControl" Src="ucFieldStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDataStyleControl" Src="ucFieldDataStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title id="Title" runat="server">Text Field</title>
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
		            <ek:FieldNameControl ID="Name" runat="server" IndexedEnabled="false" />
		            <tr>
		                <td><label title="Default value" for="txtDefVal" class="Ektron_StandardLabel" id="lblDefVal" runat="server">Default value:</label></td>
		                <td colspan="2"> 
		                    <div id="RichAreaDefaultContainer" style="display:none">
					            <textarea title="Enter Default Value here" id="richDefVal" name="richDefVal" rows="3" cols="24"></textarea>
		                    </div> 
		                    <div id="MultilineDefaultContainer" style="display:none">
					            <textarea title="Enter Default Value here (Multiple Lines)" id="taDefVal" name="taDefVal" rows="3" cols="24"></textarea>
		                    </div> 
		                    <div id="SingleLineDefaultContainer" style="display:block">
					            <input title="Enter Default Value here" type="text" id="txtDefVal" name="txtDefVal" value="" maxlength="2000" class="Ektron_StandardTextBox" />
		                    </div>
                        </td>
		            </tr>
		            <tr>
		                <td colspan="3" style="width: 100%">
	                        <ek:FieldStyleControl ID="FieldStyleControl" runat="server" />
		                </td>
		            </tr>
		            <tr>
		                <td colspan="3" style="width: 100%">
		                    <fieldset class="Ektron_TopSpaceVeryVerySmall">
		                        <legend id="lblOptions" runat="server" title="Options">Options</legend>
		                        <div class="Ektron_TopSpaceVeryVerySmall">
                                    <asp:CheckBox ToolTip="Retain single paragraph in Rich Formatting field" ID="chkKeepPTag" runat="server" CssClass="SubOptions" Enabled="false" />
                                    <asp:CheckBox ToolTip="Allow Rich Formtting Option" ID="chkRichArea" Text="Allow rich formatting" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
                                    <asp:CheckBox ToolTip="Allow Multiple Lines Option" ID="chkMultiline" Text="Allow multiple lines" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
                                    <asp:CheckBox ToolTip="Cannot be Changed Option" ID="chkReadOnly" Text="Cannot be changed" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
                                    <asp:CheckBox ToolTip="Invisible Option" ID="chkHidden" Text="Invisible" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
                                    <asp:CheckBox ToolTip="Password field Option" ID="chkPassword" Text="Password field" runat="server" onclick="updateDisplay()" CssClass="OptionsCheckList" />
		                        </div>
		                    </fieldset>
		                </td>
		            </tr>
	            </table>
	        </radTS:PageView>
	        <radTS:PageView id="Pageview2" runat="server" >
                <table width="100%">
					<ek:FieldValidateControl ID="validateControl" runat="server" Enabled="true" />
                </table> 
            </radTS:PageView>
	        <radTS:PageView id="Pageview3" runat="server" >
                <table width="100%">
                    <ek:FieldDataStyleControl runat="server" />
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

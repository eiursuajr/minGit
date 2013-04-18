<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditTag.ascx.cs" Inherits="controls_Community_PersonalTags_EditTag" %>
<%@ Reference Page="../../../Community/PersonalTags.aspx" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronGrid">
        <tr>
            <td class="label"><asp:Literal ID="tagIdLabelLit" runat="server" />:</td>
            <td class="readOnlyValue"><asp:Literal ID="tagIdLit" runat="server" /></td>
        </tr>
        <tr>
            <td class="label"><asp:Literal ID="tagLangLabelLit" runat="server" />:</td>
            <td class="readOnlyValue"><asp:Literal ID="tagLangLit" runat="server" /></td>
        </tr>
        <tr>
            <td class="label"><asp:Literal ID="tagNameLabelLit" runat="server" />:</td>
            <td class="readOnlyValue"><asp:TextBox ToolTip="Name" Columns="40" ID="tagNameTxt" runat="server" /></td>
        </tr>    
        <tr style="display: none;">
            <td class="label" title="Description">Description:</td>
            <td class="readOnlyValue"><asp:TextBox ToolTip="Description" TextMode="MultiLine" Columns="40" Rows="6" ID="tagDescTxt" runat="server" /></td>
        </tr>                     
    </table>
</div>
	
<asp:HiddenField ID="tagLangIdHdn" runat="server" />
<asp:HiddenField ID="tagValid" Value="0" runat="server" />
<script type="text/javascript" language="javascript">
	function doSubmit(valId){
		var valHdnObj = document.getElementById(valId);
		if (valHdnObj) {
			// Indicate to server that this is a valid postback, 
			// so that it will send the data to the database:
			valHdnObj.value = "1";
		}
		document.forms[0].submit();
	}
</script>


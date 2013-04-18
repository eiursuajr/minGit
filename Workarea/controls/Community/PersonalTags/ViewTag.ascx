<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewTag.ascx.cs" Inherits="controls_Community_PersonalTags_ViewTag" %>
<%@ Reference Page="../../../Community/PersonalTags.aspx" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
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
            <td class="readOnlyValue"><asp:Literal ID="tagNameLit" runat="server" /></td>
        </tr>
    </table>

    <div class="ektronHeader"><asp:Literal ID="tagStatisticsLabel" runat="server" /></div>

	<div class="ektronBorder">
	    <div class="ektronPageGrid">
	        <asp:DataGrid ID="tagStatsGrid"
	            AutoGenerateColumns="false"
	            Width="100%"
                GridLines="None"
	            runat="server">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
        </div>
    </div>
</div>

<asp:HiddenField ID="tagIdHdn" runat="server" />
<asp:HiddenField ID="tagLangIdHdn" runat="server" />
<asp:HiddenField ID="tagValid" Value="0" runat="server" />
<script type="text/javascript" language="javascript">
	function doDeleteSubmit(valId, delMsg){
		var valHdnObj = document.getElementById(valId);
		if (valHdnObj) {
			if (confirm(delMsg)){
				// Indicate to server that this is a valid postback,
				// so that it will send the data to the database:
				valHdnObj.value = "1";
				document.forms[0].submit();
			}
		}
	}
</script>


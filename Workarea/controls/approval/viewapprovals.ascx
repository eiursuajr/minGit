<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewapprovals" CodeFile="viewapprovals.ascx.cs" %>

<script type="text/javascript">
var jsAction="<asp:literal id="jsAction" runat="server"/>";
var jsType="<asp:literal id="jsType" runat="server"/>"
var jsId="<asp:literal id="jsId" runat="server"/>"
	function LoadApproval(FormName){
		var num=document.forms[0].selLang.selectedIndex;
		document.forms[0].action="content.aspx?action="+jsAction+"&type="+jsType+"&id="+jsId+"&LangType="+document.forms[0].selLang.options[num].value;
		document.forms[0].submit();
		return false;
	}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <asp:Panel ID="pnlInherited" runat="server">
	    <asp:Label ID="lblInherited" runat="server" />
	    <div class="ektronTopSpaceSmall"></div>
    </asp:Panel>

    <table class="ektronGrid">
        <tr>
            <td class="label"><%=this.m_refMsg.GetMessage("lbl approval method")%></td>
            <td class="readOnlyValue"><asp:Label ID="lblMethod" runat="server" /></td>
        </tr>
    </table>
    <div style="margin-top:.5em;">
        <asp:DataGrid id="ViewApprovalsGrid"
            runat="server"
            Width="100%"
            AutoGenerateColumns="False"
            EnableViewState="False"
            CssClass="ektronGrid"
            GridLines="None">
            <HeaderStyle CssClass="title-header" />
        </asp:DataGrid>
    </div>    
</div>


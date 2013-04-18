<%@ Control Language="C#" AutoEventWireup="true" Inherits="editapprovalorder" CodeFile="editapprovalorder.ascx.cs" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">    
    <div id="td_eao_ordertitle" runat="server"></div>
    <table width="100%" class="ektronForm">
	    <tr>
		    <td width="10%" valign="top" id="td_eao_msg" runat="server"></td>
		    <td valign="middle" width="*">
		        <asp:ListBox ID="ApprovalList" 
		            Rows="20" 
		            Runat="server" 			         
			        Width="90%" 
			        CssClass="approvalList"
			    />
			    <span id="td_eao_link" runat="server" class="moveButtons"/>		
                <div class="ektronCaption" id="td_eao_title" runat="server"></div>
		    </td>
	    </tr>
    </table>
</div>
			
<input type="hidden" id="ApprovalOrder" name="ApprovalOrder" runat="server" />


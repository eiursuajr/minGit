<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Agents.aspx.cs" Inherits="Workarea_Notifications_Agents" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Notification Agent</title>
     <asp:literal id="StyleSheetJS" runat="server" />
     <script type="text/javascript" language="JavaScript">
     function SubmitForm(FormName) 
     {
		document.forms[FormName].submit();
        return false;
	 }
	 function resetCPostback()
     {
        document.forms["agent"].isCPostData.value = "";
     }
     function ConfirmDelete() 
	 {
			
			return (confirm('<asp:literal id="delAgentMsg" runat="server"/>'));
	 }
     </script>
</head>
<body>
    <form id="agent" runat="server">
    <div id="dhtmltooltip"></div>
			<div class="ektronPageHeader">
			    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
			    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		</div>
	<div id="AddAgent" runat="server">
	<table class="ektronForm">
                <tbody>
                    <tr>
                        <td class="label" title="Name">
                            <%=msgHelper.GetMessage("generic name")%>:</td>
                        <td class="value">
                            <asp:DropDownList ToolTip="Select Name from Drop Down Menu" ID="ddlagent" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label" title="Enabled">
                             <%=msgHelper.GetMessage("enabled")%>:</td>
                         <td class="value">
                            <asp:checkbox ToolTip="Enable/Disable Name Seleciton" ID="chkEnable" runat="server" /></td>
                    </tr>
                    </tbody>
      </table>
    </div>
    <div id="ViewAgents" runat ="server" >
    <div class="ektronPageContainer ektronPageGrid">
			        <asp:GridView id="ViewAgentGrid" 
			            runat="server" 
			            Width="100%" 
			            AutoGenerateColumns="False"
			            CssClass="ektronGrid"
			            GridLines="None">
                        <HeaderStyle CssClass="title-header" />			        
			        </asp:GridView>
			        <p class="pageLinks">
                        <asp:Label ToolTip="Page" runat="server" ID="PageLabel"><%=msgHelper.GetMessage("page lbl")%></asp:Label>
                        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                        <asp:Label ToolTip="of" runat="server" ID="OfLabel"> <%=msgHelper.GetMessage("lbl of")%></asp:Label>
                        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                        <input type="hidden" runat="server" name="hdnUnit" value="hidden" id="hdnUnit" />
                        <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
                    </p>
                    <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                        OnCommand="NavigationLink_Click" CommandName="First" />
                    <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage1" Text="[Previous Page]"
                        OnCommand="NavigationLink_Click" CommandName="Prev" />
                    <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                        OnCommand="NavigationLink_Click" CommandName="Next" />
                    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                        OnCommand="NavigationLink_Click" CommandName="Last" />
                    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
			    </div>                
			</div>
			<input type="hidden" runat="server" id="isCPostData" value="false" />
    
    </form>
</body>
</html>


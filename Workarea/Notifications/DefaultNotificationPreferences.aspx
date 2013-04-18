<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DefaultNotificationPreferences.aspx.cs"
    Inherits="Workarea_DefaultNotificationPreferences" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Default Notification Preferences</title>
    <asp:literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript" language="JavaScript">
     function SubmitForm(FormName) 
     {
		document.forms[FormName].submit();
        return false;
	 }
    </script>

    <style type="text/css">
	    .exception
	    {
	        background-color:#FBE3E4;
	        border: 1px solid #FBC2C4;
	        color: #D12F19;
	        display: block;
	        margin: 0.25em;
	        padding: 0;
	        background-image: url('../images/ui/icons/error.png');
	        background-repeat: no-repeat;
	        background-position: .25em .25em
	    }

	    .exception {padding: .25em 0 .25em 1.75em;}
	</style>
</head>
<body>
    <form id="notificationPreferences" runat="server">
        <div class="ektronPageHeader" id="ektronPageHeader" visible="false" runat="server">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <div id="agentDisabled" class="exception" runat="server">
                Turn on the agents before setting up the default preferences
            </div>
            <asp:GridView ID="DefaultPrefGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                CssClass="ektronGrid" GridLines="None">
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
            <asp:LinkButton ToolTip="NextPage" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" />
            <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" CommandName="Last" />
                
        </div>
    </form>
</body>
</html>



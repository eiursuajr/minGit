<%@ Page Language="C#" AutoEventWireup="true" CodeFile="byproduct.aspx.cs" Inherits="Commerce_byproduct" %>
<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../controls/paging/paging.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>By Product</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <asp:Literal ID="ltr_folder" runat="server" />
        <div class="ektronPageContainer ektronPageGrid">
            <asp:DataGrid 
                ID="FolderDataGrid" 
                runat="server"
                AutoGenerateColumns="False" 
                Width="100%" 
                AllowPaging="False" 
                AllowCustomPaging="True"
                PageSize="10" 
                PagerStyle-Visible="False" 
                EnableViewState="False"
                CssClass="ektronGrid"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
            <p class="pageLinks">
                <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            </p>
        </div>
        <ucEktron:Paging ID="ucPaging" Visible="false" runat="server" />
    </form>
</body>
</html>


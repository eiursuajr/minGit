<%@ Page Language="C#" AutoEventWireup="true" CodeFile="country.aspx.cs" Inherits="Commerce_locale_country" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Countries</title>
    <asp:literal id="ltr_js" runat="server" />
    <script type="text/javascript">
    function resetPostback()
    {
        document.forms[0].isPostData.value = "";
    }
    </script>
</head>
<body onclick="MenuUtil.hide()">
    <form id="form1" runat="server">
    <div class="ektronPageContainer">
        <asp:Panel cssclass="ektronPageGrid" ID="pnl_viewall" runat="Server">
            <asp:DataGrid ID="dg_viewall"
                AllowSorting="true"
                runat="server"
                AutoGenerateColumns="false"
                Width="100%"
		        CssClass="ektronGrid" 
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
                <Columns>
                    <asp:TemplateColumn>
                        <HeaderTemplate><%#Util_SortUrl("lbl numericisocode", "id")%></HeaderTemplate>
                        <ItemTemplate><a href='country.aspx?action=View&id=<%#(DataBinder.Eval(Container.DataItem, "id"))%>'><%#(DataBinder.Eval(Container.DataItem, "id"))%></a></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderTemplate><%#Util_SortUrl("generic name", "name")%></HeaderTemplate>
                        <ItemTemplate><a href='country.aspx?action=View&id=<%#(DataBinder.Eval(Container.DataItem, "id"))%>'><%#(DataBinder.Eval(Container.DataItem, "Name"))%></a></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderTemplate><%#Util_SortUrl("enabled", "enabled")%></HeaderTemplate>
                        <ItemTemplate><asp:checkbox runat="server" Enabled="false" ID="chk_enabled" Checked='<%#(DataBinder.Eval(Container.DataItem, "enabled"))%>' /></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderTemplate><%#Util_SortUrl("lbl longisocode", "longiso")%></HeaderTemplate>
                        <ItemTemplate><a href='country.aspx?action=View&id=<%#(DataBinder.Eval(Container.DataItem, "id"))%>'><%#(DataBinder.Eval(Container.DataItem, "LongIsoCode"))%></a></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn>
                        <HeaderTemplate><%#Util_SortUrl("lbl shortisocode", "shortiso")%></HeaderTemplate>
                        <ItemTemplate><a href='country.aspx?action=View&id=<%#(DataBinder.Eval(Container.DataItem, "id"))%>'><%#(DataBinder.Eval(Container.DataItem, "ShortIsoCode"))%></a></ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </asp:Panel>
        <asp:Panel ID="pnl_view" runat="server" Cssclass="ektronPageInfo" Visible="false">
            <table id="tblmain" class="ektronGrid" runat="server">
                <tr>
                    <td class="label"><asp:Literal ID="ltr_name" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_name" runat="server" Columns="50" MaxLength="25" /></td>
                </tr>
                <tr id="tr_id" runat="server">
                    <td class="label"><asp:Literal ID="ltr_id" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_id" runat="server" Columns="50" MaxLength="9" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Literal ID="ltr_enabled" runat="server" />:</td>
                    <td><asp:checkbox ID="chk_enabled" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Literal ID="ltr_long" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_long" runat="server" Columns="50" MaxLength="3" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Literal ID="ltr_short" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_short" runat="server" Columns="50" MaxLength="2" /></td>
                </tr>
                <tr>
                <td></td> 
                    <td>
                    <ektronUI:Message ID="uxMessage" DisplayMode="Information" Visible="false" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <p class="pageLinks">
            <asp:Label runat="server" ID="PageLabel">Page</asp:Label>
            <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
            <asp:Label runat="server" ID="OfLabel">of</asp:Label>
            <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
        </p>
        <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
            OnCommand="NavigationLink_Click" CommandName="First" />
        <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
            OnCommand="NavigationLink_Click" CommandName="Prev" />
        <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
            OnCommand="NavigationLink_Click" CommandName="Next" />
        <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
            OnCommand="NavigationLink_Click" CommandName="Last" />
        <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
        </div>
    </form>
</body>
</html>


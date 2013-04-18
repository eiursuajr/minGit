<%@ Page Language="C#" AutoEventWireup="true" CodeFile="region.aspx.cs" Inherits="Commerce_locale_region" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Regions</title>
    <asp:literal id="ltr_js" runat="server" />
    <script type="text/javascript" language="javascript">
    function resetPostback()
    {
        document.forms[0].isPostData.value = "";
    }
    </script>
    <!--[if lt IE 8]>
    <style type="text/css">
        input#btnSearch {float: none; display: block;}
    </style>
    <![endif]-->

</head>
<body onclick="MenuUtil.hide()">
    <form id="form1" runat="server">
    <div class="ektronPageContainer">
        <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
            <asp:DataGrid ID="dg_viewall"
                EnableViewState="False"
                AllowPaging="true"
                AllowCustomPaging="True"
                PageSize="10"
                PagerStyle-Visible="False"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="ektronGrid"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
        </asp:Panel>
        <asp:Panel ID="pnl_view" Cssclass="ektronPageInfo" runat="Server" Visible="false">
            <table id="tblmain" class="ektronGrid" runat="server">
                <tr>
                    <td class="label"><asp:Literal ID="ltr_name" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_name" runat="server" Columns="50" MaxLength="25" /></td>
                </tr>
                <tr id="tr_id" runat="server">
                    <td class="label"><asp:Literal ID="ltr_id" runat="server" />:</td>
                    <td><asp:Label ID="lbl_id" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Literal ID="ltr_code" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_code" runat="server" Columns="50" MaxLength="25" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Literal ID="ltr_country" runat="server" />:</td>
                    <td><asp:DropDownList ID="drp_country" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label">

                        <asp:Literal ID="ltr_enabled" runat="server" />
                    </td>
                    <td>
                        <asp:checkbox ID="chk_enabled" runat="server" />
                    </td>
                </tr>
                <tr runat="server" id="tr_addanother">
                    <td class="label">
                        <asp:Literal ID="ltr_addanother" runat="server" />
                    </td>
                    <td>
                        <asp:checkbox ID="chk_addanother" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <p class="pageLinks">
            <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
            <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
            <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
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


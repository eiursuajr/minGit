<%@ Page Language="C#" AutoEventWireup="true" CodeFile="itemselection.aspx.cs" Inherits="Commerce_itemselection" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Item Selection</title>
    <script type="text/javascript" language="javascript">
    function resetPostback()
    {
        document.forms[0].isPostData.value = "";
    }
    </script>
    <script type="text/javascript">
        $ektron(document).ready(function(){
           // $ektron(".ektronTitlebar").addClass("hide-title");
            //$ektron(".ektronPageHeader").addClass("no-top-padding");
            //$ektron(".ektronPageContainer").addClass("adjusted-33px-top");
        });
    </script>
</head>
<body style="margin:0 0 0 0;">
    <form id="form1" runat="server">
    <div class="ektronPageContainer ektronPageGrid">
        <asp:Panel cssclass="ektronPageGrid" ID="pnl_viewall" runat="Server">
            <div id="dhtmltooltip"></div>
            <asp:DataGrid ID="FolderDataGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                AllowPaging="False"
                AllowCustomPaging="True"
                PageSize="10"
                PagerStyle-Visible="False"
                EnableViewState="False"
                cssclass="ektronGrid"
                showheader="false">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
            <div class="paging" id="divPaging" runat="server">
                <ul class="direct">
                    <li><asp:ImageButton ToolTip="First Page" ID="ibFirstPage" runat="server" OnCommand="NavigationLink_Click" CommandName="First" /></li>
                    <li><asp:ImageButton ToolTip="Previous Page" ID="ibPreviousPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Prev" /></li>
                    <li><asp:ImageButton ToolTip="Next Page" ID="ibNextPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Next" /></li>
                    <li>
                        <asp:ImageButton ToolTip="Last Page" ID="ibLastPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Last" />
                        <asp:HiddenField ID="hdnTotalPages" runat="server" />
                    </li>
                </ul>
                <script type="text/javascript">
                    <!--                                //--><![CDATA[//><!--
                    function GoToPage(elem) {
                        var valid = true;
                        var adHocButton = $ektron(elem);
                        var selectedPage = adHocButton.prevAll("span.pageNumber").find("input.currentPage").attr("value");
                        var isValueNumeric = selectedPage.match(/^\d+$/) == null ? false : true;
                        if (isValueNumeric) {
                            var currentPage = parseInt(adHocButton.prevAll("input.currentPage").attr("value"), 10);
                            var totalPages = parseInt(adHocButton.prevAll("span.pageTotal").text(), 10);
                            if (selectedPage == 0 || selectedPage > totalPages) {
                                valid = false;
                                alert("Page number must be between 1 and " + totalPages + "!");
                                adHocButton.prevAll("span.pageNumber").find("input.currentPage").attr("value", currentPage);
                            }
                        } else {
                            valid = false;
                            alert("Page number must be numeric!");
                        }
                        return valid;
                    }
                    //--><!]]>
                </script>
                <p class="adHoc">
                    <span class="page"><asp:Literal ID="litPage" runat="server" /></span>
                    <span class="pageNumber"><asp:TextBox CssClass="currentPage" ID="txtCurrentPage" runat="server"></asp:TextBox></span>
                    <span class="pageOf"><asp:Literal ID="litOf" runat="server" /></span>
                    <input type="hidden" runat="server" name="hdnCurrentPage" class="currentPage" id="hdnCurrentPage" />
                    <span class="pageTotal"><asp:Literal ID="litTotalPages" runat="server" /></span>
                    <asp:ImageButton ToolTip="Go" ID="ibPageGo" CssClass="adHocPage" runat="server" OnCommand="AdHocPaging_Click" CommandName="AdHocPage" />
                </p>
            </div>    
        </asp:Panel>
        <asp:Panel CssClass="ektronPageGrid" ID="pnl_catalogs" runat="server" Visible="false">
            <asp:DataGrid ID="CatalogGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                AllowPaging="False"
                AllowCustomPaging="True"
                PageSize="10"
                PagerStyle-Visible="False"
                EnableViewState="False"
                cssclass="ektronGrid"
                showheader="false">
                <HeaderStyle CssClass="title-header" />
                <Columns>
                    <asp:TemplateColumn ItemStyle-Width="5%"><ItemTemplate><img style="margin-left: .25em;" alt="" src="../images/ui/icons/tree/folderGreen.png"/>&#160;&#160;</ItemTemplate></asp:TemplateColumn>
                    <asp:TemplateColumn ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top" >
                        <ItemTemplate><a href="itemselection.aspx?exclude=<%# excludeId %>&SelectedTab=<%# Request.QueryString["SelectedTab"] %>&action=browse<%=m_sPageAction%>&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "Name")%></a></ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </asp:Panel>
    </div>
    </form>
</body>
</html>


<%@ Control Language="C#" AutoEventWireup="true" Inherits="removefolderitem" CodeFile="removefolderitem.ascx.cs" %>

<script type="text/javascript" language="javascript">
function resetPostback()
{
    document.forms[0].deletecontentbycategory_isPostData.value = "";
}

var browseURL = '<asp:literal id="browseurljs" runat="Server"/>';
var pagebetween = '<asp:literal id="pagebetweenjs" runat="Server"/>';

function GoToDeletePage(pageid, pagetotal) 
{
    if (pageid <= pagetotal && pageid >= 1) {
        window.location.href = browseURL + pageid;
    } else { 
        alert(pagebetween); 
    }
}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:DataGrid ID="DeleteContentByGategoryGrid" 
        runat="server"
        CssClass="ektronGrid"
        AutoGenerateColumns="False"
        EnableViewState="False">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>

    <div class="paging" id="divPaging" runat="server" visible="false">
        <ul class="direct">
            <li><asp:ImageButton ID="ibFirstPage" runat="server" OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" /></li>
            <li><asp:ImageButton ID="ibPreviousPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" /></li>
            <li><asp:ImageButton ID="ibNextPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" /></li>
            <li>
                <asp:ImageButton ID="ibLastPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
                <asp:HiddenField ID="hdnTotalPages" runat="server" />
            </li>
        </ul>
        <p class="adHoc">
            <span class="page" title="Page"><asp:Literal ID="litPage" runat="server" /></span>
            <span class="pageNumber"><asp:TextBox CssClass="currentPage" ID="CurrentPage" runat="server"></asp:TextBox></span>
            <span class="pageOf" title="of"><asp:Literal ID="litOf" runat="server" /></span>
            <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
            <span class="pageTotal"><asp:Literal ID="TotalPages" runat="server" /></span>
            <asp:ImageButton ID="ibPageGo" CssClass="adHocPage" runat="server" CommandName="AdHocPage" />
        </p>
    </div>

    <input type="hidden" id="isPostData" value="true" name="isPostData" runat="server" />
    <input type="hidden" id="folder_id" name="folder_id" runat="server" />
    <input type="hidden" id="contentids" name="contentids" runat="server" />
    <input type="hidden" id="contentlanguages" name="contentlanguages" runat="server" />
    <input type="hidden" id="content_id" name="content_id" value="0" runat="server" />
    <input type="hidden" id="page_id" name="page_id" value="0" runat="server" />
</div>

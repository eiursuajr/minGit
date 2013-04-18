<%@ Control Language="C#" AutoEventWireup="true" CodeFile="paging.ascx.cs" Inherits="Ektron.Workarea.Controls.Paging" %>
<div class="paging clearfix">
    <input type="hidden" id="hdnSelectedPage" runat="server" class="selectedPage" />
    <input type="hidden" id="hdnCurrentPageIndex" runat="server" class="currentPageIndex" />
    <input type="hidden" id="hdnTotalPages" runat="server" class="totalPages" />
    <ul>
        <li><asp:ImageButton ID="ibFirstPage" runat="server" CssClass="FirstPage"
         OnClientClick="Ektron.Workarea.Paging.click(this);" /></li>
        <li><asp:ImageButton ID="ibPreviousPage" runat="server" CssClass="PreviousPage"
         OnClientClick="Ektron.Workarea.Paging.click(this);" /></li>
        <li><asp:ImageButton ID="ibNextPage" runat="server" CssClass="NextPage"
         OnClientClick="Ektron.Workarea.Paging.click(this);" /></li>
        <li><asp:ImageButton ID="ibLastPage" runat="server" CssClass="LastPage"
         OnClientClick="Ektron.Workarea.Paging.click(this);" /></li>
    </ul>
    <p>
        <span class="page"><asp:Literal ID="litPage" runat="server" /></span>
        <span class="pageNumber"><asp:TextBox CssClass="adHocPage" ID="txtPageNumber" MaxLength="5" runat="server"></asp:TextBox></span>
        <span class="pageOf"><asp:Literal ID="litOf" runat="server" /></span>
        <span class="pageTotal"><asp:Literal ID="litTotalPages" runat="server" /></span>
        <asp:LinkButton ID="lbPageGo" CssClass="AdHoc" runat="server" 
         OnClientClick="Ektron.Workarea.Paging.click(this);" />
    </p>   
</div>
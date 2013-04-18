<%@ Control Language="C#" AutoEventWireup="true" CodeFile="clientpaging.ascx.cs" Inherits="Ektron.Controls.ClientPaging" %>
<div class="paging clearfix">
    <input type="hidden" id="hdnSelectedPage" class="selectedPage" />
    <input type="hidden" id="hdnCurrentPageIndex" class="currentPageIndex" />
    <input type="hidden" id="hdnTotalPages" class="totalPages" />
    <p>
        <span class="page"><asp:Literal ID="litPage" runat="server" /></span>
        <span class="pageNumber">
     <%--   <asp:TextBox CssClass="adHocPage" ID="txtPageNumber" MaxLength="5" Enabled="false" runat="server"></asp:TextBox>--%>
           <input type="text" class="adHocPage" ID="txtPageNumber" disabled="disabled"></asp:TextBox>
        </span>
        <span class="pageOf"><asp:Literal ID="litOf" runat="server" /></span>
        <span class="pageTotal"><asp:Literal ID="litTotalPages" runat="server" /></span>
        
        <!-- Hidden until implemented -->
        <asp:ImageButton ID="ibPageGo" CssClass="AdHoc" runat="server" Visible="false"
         OnClientClick="Ektron.Controls.Paging.click(this);return false" />
    </p>
    <ul>
    
        <li>
            <a href="#" id="FirstPage" onclick="return false;">
                <asp:Image ID="ibFirstPage" runat="server" AlternateText="First Page" />
            </a>
        </li>
        <li>
            <a href="#" id="PreviousPage" onclick="return false;">
                <asp:Image ID="ibPreviousPage" runat="server" AlternateText="Previous Page" />
            </a>
        </li>
        <li>
            <a href="#" id="NextPage" onclick="return false;">
                <asp:Image ID="ibNextPage" runat="server" AlternateText="First Page" />
            </a>
         </li>
         <li>
            <a href="#" id="LastPage" onclick="return false;">
                <asp:Image ID="ibLastPage" runat="server" AlternateText="First Page" />
            </a>
         </li>
    </ul>   
</div>
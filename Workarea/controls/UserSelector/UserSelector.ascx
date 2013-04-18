<%@ Control Language="C#" AutoEventWireup="true" Debug="true" CodeFile="UserSelector.ascx.cs" Inherits="UserSelector" %>
<div id="<%# this.ClientID %>" class="ektron userSelector">
    <asp:HiddenField ID="ucSelectedUserIds" EnableViewState="true" runat="server" />
    <ul class="users">
        <%-- <li class="user" data-userId="#">Username</li> --%>
    </ul>
    <span class="usernameWrapper">
        <asp:TextBox ToolTip="User Name Text" ID="ucUserName" CssClass="username" runat="server"></asp:TextBox>
    </span>
    <div class="userDialog" title="Select a User">
        <ul class="users">
            <%-- <li class="user" data-userId="#">Username</li>  --%>
        </ul>
        <div class="paging">
            <span class="pageCountDisplay">
                Page <input type="text" class="currentPage" value="0"/></span> of <span class="pageCount">0</span>
            </span><span class="go icon" title="Go"></span> 
            <span class="firstPage icon" title="First page"></span><span class="previousPage icon" title="Previous page"></span><span class="nextPage icon" title="Next page"></span><span class="lastPage icon" title="Last page"></span>
        </div>
    </div>
</div>
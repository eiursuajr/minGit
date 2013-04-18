<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ActivityStream.ascx.cs"
    Inherits="widget_ActivityStream" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/Workarea/Personalization/Personalization.ascx" TagName="Personalization" TagPrefix="ucEktron" %>

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
   <%--<asp:UpdatePanel ID="control"  runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" RenderMode="Block" >
       <ContentTemplate>--%>
       <CMS:ActivityStream ID="cmsActivityFeed" EnablePaging="true"  runat="server" />
            <input id="hdnClientId" runat="server" class="clientId" name="ActivityFeed" type="hidden" />
            <input id="hdnCurrentPageNumber" name="hdnCurrentPageNumber" type="hidden" runat="server"/>
            <p class="ekActivityFeedPaging">
                <asp:Label runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            </p>
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" OnClientClick="" CommandName="First" />
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="PreviousPage1" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" OnClientClick="" CommandName="Prev" />
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" OnClientClick="" CommandName="Next" />
            <asp:LinkButton runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" OnClientClick="" CommandName="Last" />
            <asp:Button ID="btnFilter" runat="server" OnClick="btnFilter_Click" Text="Filter"
                Visible="true"></asp:Button>
        <div id="EkActivityFeedPrefenceSelection" visible="false" class="EkActivityFeedPrefenceSelection" runat="server">
            <br />
            <div id="selection">
                <fieldset id="SelectedList">
                    <legend><strong>Users and Groups</strong></legend>
                    <asp:Repeater ID="repFriends" runat="server" OnItemDataBound="repFriends_ItemDataBound"
                        OnItemCommand="repFriends_ItemCommand" Visible="false">
                        <HeaderTemplate>
                            <ul class="EkActivityFeedPreferenceList clearfix">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="PreferenceListItem">
                             <span class="name">
                                <%# (Container.DataItem as string) %>
                             </span>
                                <asp:LinkButton ID="btnDelete" CssClass="PreferenceListDel" CommandName="btnDelete"
                                    CommandArgument="<%# Container.ItemIndex %>" runat="server" ToolTip="Delete">
                                <span>Delete</span>
                                </asp:LinkButton>
                                <br />
                             </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblsearch" AssociatedControlID="query" runat="server">Search</asp:Label> 
                    <input type="text" name="q" id="query" class="textbox" runat="server" />
                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" />
                </fieldset>
            </div>
            <br />
            <asp:Button ID="CancelFilterBtn" runat="server" Text="Hide" OnClick="CancelFilterBtn_Click" />
        </div>
        <asp:Label ID="errorLb" runat="server" />
       <%--</ContentTemplate>
        </asp:UpdatePanel>--%>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <table style="width: auto;">
            <tr id="rowobjectid" runat="server">
                <td>
                    Object ID:
                </td>
                <td>
                    <asp:TextBox ID="objectid" runat="server"></asp:TextBox></td>
            </tr>
            <tr id="rowfeedtype" runat="server">
                <td>
                    Feed Type:</td>
                <td>
                    <asp:DropDownList ID="FeedTypeSelectDropDownList" runat="server">
                        <asp:ListItem>User</asp:ListItem>
                        <asp:ListItem>Community Group</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Max Results:</td>
                <td>
                    <asp:TextBox ID="txtmaxresults" runat="server" /></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="SaveButton" runat="server" OnClick="SaveButton_Click" Text="Save" />
                </td>
                <td>
                    <asp:Button ID="CancelButton" CssClass="LSCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                </td>
            </tr>
        </table>
    </asp:View>
</asp:MultiView>

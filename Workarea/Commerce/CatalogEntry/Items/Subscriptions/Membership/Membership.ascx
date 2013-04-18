<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Membership.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Subscriptions.Membership" %>
<%@ Reference Control="../../Items.ascx" %>
<input type="hidden" id="SubscriptionMembershipData" name="SubscriptionMembershipData" class="subscriptionMembershipData" value="" />
<input type="hidden" id="hdnSitePath" runat="server" class="subscriptionMembershipSitePath" value="false" />
<fieldset>
    <legend>Groups</legend>
    <table>
        <tbody>
            <tr class="cmsGroup group">
                <th>CMS Author Group</th>
                <td>
                    <span id="spanCmsGroupView" runat="server" class="groupName"></span>
                    <asp:PlaceHolder ID="phCmsAuthorGroupEdit" runat="server">
                        <input id="EktronSusbscriptionCmsGroupId" name="EktronSusbscriptionCmsGroupId" type="hidden" class="groupId" value="<%= GetCmsGroupId() %>" />
                        <input type="hidden" class="markedForDelete" id="EktronSusbscriptionCmsGroupMarkedForDelete" name="EktronSusbscriptionCmsGroupMarkedForDelete" value="false" />
                        <a href="#Edit" class="editCmsAuthorGroup edit" title="Edit" onclick="Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.edit(this);return false;">
                            <asp:Image ID="imgCmsAuthorGroupEdit" runat="server" AlternateText="Edit" ToolTip="Edit Cms Author Group" />
                        </a>
                        <a href="#MarkForDelete" class="markForDelete<%= GetMarkForDeleteHide("cms") %>" title="Mark For Delete" onclick="Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.markForDelete(this);return false;">
                            <asp:Image ID="imgCmsAuthorGroupMarkForDelete" runat="server" AlternateText="Mark For Delete" ToolTip="Mark For Delete" />
                        </a>
                        <a href="#Restore" class="restore" title="Restore" onclick="Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.restore(this);return false;">
                            <asp:Image ID="imgCmsAuthorGroupRestore" runat="server" AlternateText="Restore" ToolTip="Restore" />
                        </a>
                    </asp:PlaceHolder>
                </td>
            </tr>
            <tr class="membershipGroup group">
                <th>Member Group</th>
                <td>
                    <span id="spanMemberGroupView" runat="server" class="groupName"></span>
                    <asp:PlaceHolder ID="phMemberGroupEdit" runat="server">
                        <span class="required">*</span>
                        <input id="EktronSusbscriptionMembershipGroupId" name="EktronSusbscriptionMembershipGroupId" type="hidden" class="groupId" value="<%= GetMembershipGroupId() %>" />
                        <input type="hidden" class="markedForDelete" id="EktronSusbscriptionMembershipGroupMarkedForDelete" name="EktronSusbscriptionMembershipGroupMarkedForDelete" value="false" />
                        <a href="#Edit" class="editMembershipGroup edit" title="Edit" onclick="Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.edit(this);return false;">
                            <asp:Image ID="imgMemberGroupEdit" runat="server" AlternateText="Edit" ToolTip="Edit" />
                        </a>
                        <p class="requiredText">* required</p>
                    </asp:PlaceHolder>
                </td>
            </tr>
        </tbody>
    </table>
</fieldset>
<asp:PlaceHolder ID="phModal" runat="server" Visible="true">
    <div id="ItemsSubscriptionMembershipModal" class="itemsSubscriptionMembershipModal ektronWindow">
        <h3 class="itemsSubscriptionMembershipModalHeader">
		    <asp:Image ID="imgCloseModal" runat="server" AlternateText="Close" ToolTip="Close" />
            <span id="spanModalTitle" runat="server">Select Group</span>
        </h3>
        <iframe class="ItemsSubscriptionMembershipGroupSelector" frameborder="0" scrolling="auto"></iframe>
        <p class="cancelSubscriptionMembershipModal clearfix">
            <a href="#Close" class="button buttonRight redHover ektronModalClose" title="Close" onclick="return false;">
                <img alt="Close" title="Close" src="<% =GetImagePath() %>/markForDelete.gif" />
                <span>Close</span>
            </a>
        </p>
    </div>
</asp:PlaceHolder>
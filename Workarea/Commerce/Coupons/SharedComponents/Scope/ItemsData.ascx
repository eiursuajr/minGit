<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ItemsData.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Scope.ItemsData" %>
<div class="itemsData">
    <input type="hidden" id="hdnData" runat="server" name="CouponItemsData" class="data" value="" />
    <input type="hidden" id="hdnScope" runat="server" name="CouponItems" class="scope" value="" />
    <input type="hidden" id="hdnLanguageId" runat="server" name="CouponItems" class="languageId" value="" />
    <input type="hidden" id="hdnLocalizedJavascriptString" runat="server" name="CouponItems" class="couponItemsLocalizedStrings" value="" />
    <div class="itemsDataWrapper ektronPageInfo">
        <table class="items ektronGrid">
            <thead>
                <tr class="title-header">
                    <th><asp:literal ID="litId" runat="server" /></th>
                    <th><asp:literal ID="litName" runat="server" /></th>
                    <th><asp:literal ID="litPath" runat="server" /></th>
                    <th><asp:literal ID="litType" runat="server" /></th>
                    <asp:PlaceHolder ID="phMarkForDeleteHeader" runat="server" >
                        <th class="markForDelete center">
                            <img src="<%= GetItemsImagesPath() %>/delete.png" class="markForDelete" alt="<%= GetLocalizedStringMarkForDeleteAll() %>" title="<%= GetLocalizedStringMarkForDeleteAll() %>" onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.markForDeletePage();" />
                            <img src="<%= GetItemsImagesPath() %>/restore.png" class="restore" alt="<%= GetLocalizedStringRestoreAll() %>" title="<%= GetLocalizedStringRestoreAll() %>"  onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.restorePage();"/>
                        </th>
                    </asp:PlaceHolder>
                </tr>
            </thead>
            <tbody>
                <tr class="noItems skipStripe" id="trNoItems" runat="server">
                    <td id="tdNoItems" runat="server" class="center">
                        <asp:Literal ID="litNoItems" runat="server" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="phCloneRow" runat="server">
                    <tr class="cloneRow skipStripe">
                        <td class="id"></td>                                       
                        <td class="name"></td>
                        <td class="path"></td>
                        <td class="type"></td>
                        <td class="markForDelete data center">
                            <input type="hidden" class="id" name="CouponItems" value="" />
                            <input type="hidden" class="name" name="CouponItems" value="" />
                            <input type="hidden" class="path" name="CouponItems" value="" />
                            <input type="hidden" class="type" name="CouponItems" value="" />
                            <input type="hidden" class="typeCode" name="CouponItems" value="" />
                            <input type="hidden" class="subType" name="CouponItems" value="" />
                            <input type="hidden" class="markedForDelete" name="CouponItems" value="false" />
                            <img src="<%= GetItemsImagesPath() %>/markForDelete.gif" class="markForDelete" alt="<%= GetLocalizedStringMarkForDelete() %>" title="<%= GetLocalizedStringMarkForDelete() %>" onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.markForDelete(this);" />
                            <img src="<%= GetItemsImagesPath() %>/restore.gif" class="restore" alt="<%= GetLocalizedStringRestore() %>" title="<%= GetLocalizedStringRestore() %>"  onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.restore(this);"/>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:Repeater ID="rptPublishedItemsList" runat="server" OnItemDataBound="rptPublishedItemsList_ItemDataBound">
                    <ItemTemplate>
                        <tr class="item <%# GetStripeRow(Container.ItemIndex) %>">
                            <td class="id"><%# DataBinder.Eval(Container.DataItem, "Id") %></td>                                       
                            <td class="name"><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
                            <td class="path"><%# DataBinder.Eval(Container.DataItem, "Path") %></span></td>
                            <td class="type"><asp:Literal ID="litObjectType" runat="server" /></td>
                            <asp:PlaceHolder ID="phMarkForDeleteTableCell" runat="server">
                                <td class="markForDelete data center">
                                    <input type="hidden" class="id" name="CouponItems" value="<%# DataBinder.Eval(Container.DataItem, "Id") %>" />
                                    <input type="hidden" class="name" name="CouponItems" value="<%# DataBinder.Eval(Container.DataItem, "Name") %>" />
                                    <input type="hidden" class="path" name="CouponItems" value="<%# DataBinder.Eval(Container.DataItem, "Path") %>" />
                                    <input type="hidden" class="type" name="CouponItems" value="<%# GetType(DataBinder.Eval(Container.DataItem, "Type")) %>" />
                                    <input type="hidden" class="typeCode" name="CouponItems" value="<%# GetTypeCode(DataBinder.Eval(Container.DataItem, "Type")) %>" />
                                    <input type="hidden" class="subType" name="CouponItems" value="" />
                                    <input type="hidden" class="markedForDelete" name="CouponItems" value="false" />
                                    <img src="<%= GetItemsImagesPath() %>/markForDelete.gif" class="markForDelete" alt="<%= GetLocalizedStringMarkForDelete() %>" title="<%= GetLocalizedStringMarkForDelete() %>" onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.markForDelete(this);" />
                                    <img src="<%= GetItemsImagesPath() %>/restore.gif" class="restore" alt="<%= GetLocalizedStringRestore() %>" title="<%= GetLocalizedStringRestore() %>"  onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.Items.restore(this);"/>
                                </td>
                            </asp:PlaceHolder>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
        <asp:PlaceHolder ID="phActions" runat="server">
            <p class="actions clearfix">
                 <a href="#AddProductItem" class="button buttonRight greenHover buttonProduct" onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.select('product');return false;" title="<%= GetLocalizedStringProductButton() %>">
                    <%= GetLocalizedStringProductButton()%>
                </a>
                <a href="#AddTaxonomyItem" class="button buttonRight greenHover buttonTaxonomy" onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.select('category');return false;" title="<%= GetLocalizedStringCategoryButton() %>">
                    <%= GetLocalizedStringCategoryButton()%>
                </a>
                <a href="#AddCatalogItem" class="button buttonRight greenHover buttonCatalog" onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.select('catalog');return false;" title="<%= GetLocalizedStringCatalogButton() %>">
                    <%= GetLocalizedStringCatalogButton()%>
                </a>
            </p>
        </asp:PlaceHolder>
    </div>
    <asp:Placeholder ID="phModal" runat="server">
        <div id="EktronCouponItems" class="ektronWindow ektronModalWidth-25 ui-dialog ui-widget ui-widget-content ui-corner-all" id="AttributesModal">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix itemsModalHeader">
                <span class="ui-dialog-title"></span>
                <a href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose">
                    <span class="ui-icon ui-icon-closethick"><%= GetLocalizedStringCloseModal() %></span>
                </a>
            </div>
            <div class="ui-dialog-content ui-widget-content">
                <iframe class="ektronCouponItemsIframe" scrolling="auto" frameborder="0"></iframe>
            </div>
            <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                <p class="modalActions ektronModalButtonWrapper clearfix">
                    <a href="#Cancel" class="button buttonRight redHover buttonCancel ektronModalClose" title="<%= GetLocalizedStringCancel() %>" onclick="return false;">
                        <%= GetLocalizedStringCancel() %>
                    </a>
                    <a href="#Cancel" class="button buttonRight greenHover buttonOk" title="<%= GetLocalizedStringOk() %>" onclick="Ektron.Commerce.Coupons.Scope.ItemsData.Actions.ok();return false;">
                        <%= GetLocalizedStringOk() %>
                    </a>
                </p>
            </div>
        </div>
    </asp:Placeholder>
</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Items.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item" %>
<div class="EktronCommerceItems">
    <input type="hidden" id="ItemData" name="ItemData" class="itemData" value="" />
    <input type="hidden" id="ItemDataDeserializationMode" name="ItemDataDeserializationMode" class="ItemDataDeserializationMode" value="" />
    <input type="hidden" id="hdnItemEditable" runat="server" class="itemEditable" value="false" />
    <input type="hidden" id="hdnItemView" runat="server" class="itemView" value="" />
    <input type="hidden" id="hdnJsonMode" runat="server" class="jsonMode" value="" />
    <asp:MultiView ID="mvItemType" runat="server">
        <asp:View ID="vwDefault" runat="server">
            <h3>Items</h3>
            <div class="itemWrapper clearfix">
                <ul class="defaultViewItems">
                    <li id="liDefaultEmptyRow" runat="server" class="emptyItem">
                        <div>No items exist.</div>
                    </li>
                    <li id="liDefaultCloneItem" runat="server" class="cloneItem hide">
                        <div>
                            <span class="delete">
                                <a href="#MarkItemForDelete" title="Mark Item For Delete" onclick="Ektron.Commerce.CatalogEntry.Items.DefaultView.markForDelete(this);return false;">&#160;</a>
                                <input class="markedForDelete" type="hidden" value="" />
                            </span>
                            <span class="id">
                                <input class="id" type="hidden" value="" />
                            </span>
                            <span class="title">
                                <input class="title" type="hidden" value="" />
                            </span>
                        </div>
                    </li>
                    <asp:Repeater ID="rptDefault" runat="server">
                        <ItemTemplate>
                            <li class="clearfix defaultViewItem<%# GetDefaultItemEditClass() %>" title="<%# GetDefaultItemSortTitle() %>">
                                <div>
                                    <span class="delete" id="spanDefaultDelete" runat="server" visible="<%# GetDefaultDeleteLinkVisibility() %>">
                                        <a href="#MarkItemForDelete" title="Mark Item For Delete" onclick="Ektron.Commerce.CatalogEntry.Items.DefaultView.markForDelete(this);return false;">&#160;</a>
                                        <input class="markedForDelete" type="hidden" value="" />
                                    </span>
                                    <span class="id">
                                        <%# DataBinder.Eval(Container.DataItem, "Id") %>
                                        <input class="id" type="hidden" value="<%# DataBinder.Eval(Container.DataItem, "Id") %>" />
                                    </span>
                                    <span class="title">
                                        <%# DataBinder.Eval(Container.DataItem, "Title") %>
                                        <input class="title" type="hidden" value="<%# DataBinder.Eval(Container.DataItem, "Title") %>" />
                                    </span>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <p class="addDefaultNodeButtons clearfix" id="pDefaultAddItems" runat="Server" visible="false">
                    <a href="#AddItem" class="button buttonRight greenHover" title="Add Item" onclick="Ektron.Commerce.CatalogEntry.Items.DefaultView.Add.viewItems();return false;">
		                <asp:Image ID="imgDefaultAddItem" runat="server" AlternateText="Add Item" ToolTip="Add Item" />
		                Add Item
	                </a>
                </p>
            </div>
        </asp:View>
        <asp:View ID="vwKit" runat="server">
            <div class="kitView">
                <table>
                    <thead>
                        <tr>
                            <th>Kit Components</th>
                            <th>Details</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <asp:PlaceHolder ID="phKitListEmpty" runat="server">
                                <td class="list" colspan="2">
                                    <p class="emptyKitRow">
                                        <asp:Literal ID="litEmptyKitListLabel" runat="server" />
                                    </p>
                                </td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phKitList" runat="server">
                                <td class="list">
                                    <ul id="EktronCommerceItemsKitViewGroups" class="kitGroups EktronTreeview-famfamfam">
                                        <asp:Repeater ID="rptKitGroup" runat="server" OnItemDataBound="rptKitGroup_ItemDataBound">
                                            <ItemTemplate>
                                                <li class="kitGroup">
                                                    <input type="hidden" class="id" value="<%# DataBinder.Eval(Container.DataItem, "ID") %>" />
                                                    <input type="hidden" class="name" value="<%# DataBinder.Eval(Container.DataItem, "Name") %>" />
                                                    <input type="hidden" class="description" value="<%# DataBinder.Eval(Container.DataItem, "Description") %>" />
                                                    <input type="hidden" class="markedForDelete" value="false" />
                                                    <span class="kitGroupLabel" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(this, 'list', 'view');return false;"><%# DataBinder.Eval(Container.DataItem, "Name") %></span>
                                                    <ul class="kitItems">
                                                        <asp:Repeater ID="rptKitItem" runat="server">
                                                            <ItemTemplate>
                                                                <li class="kitItem">
                                                                    <input type="hidden" class="id" value="<%# DataBinder.Eval(Container.DataItem, "ID") %>" />
                                                                    <input type="hidden" class="name" value="<%# DataBinder.Eval(Container.DataItem, "Name") %>" />
                                                                    <input type="hidden" class="extraText" value="<%# DataBinder.Eval(Container.DataItem, "ExtraText") %>" />
                                                                    <input type="hidden" class="priceModifierPlusMinus" value="<%# GetPriceModifierPlusMinus((decimal)DataBinder.Eval(Container.DataItem, "PriceModification")) %>" />
                                                                    <input type="hidden" class="priceModifierDollars" value="<%# GetPriceModifierDollars((decimal)DataBinder.Eval(Container.DataItem, "PriceModification")) %>" />
                                                                    <input type="hidden" class="priceModifierCents" value="<%# GetPriceModifierCents((decimal)DataBinder.Eval(Container.DataItem, "PriceModification")) %>" />
                                                                    <input type="hidden" class="markedForDelete" value="false" />
                                                                    <span class="kitItemLabel" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(this, 'list', 'view');return false;"><%# DataBinder.Eval(Container.DataItem, "Name") %></span>
                                                                </li>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <li id="liKitGroupAddKitItem" runat="server" class="addKitItem" visible="<%# GetKitGroupAddKitGroupItem() %>">
                                                            <span onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Item.Add.showAddItemModal(this);return false;">Add Item</span>
                                                        </li>
                                                    </ul>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <li id="liKitGroupKitGroupClone" runat="server" class="kitGroupClone">
                                            <input type="hidden" class="id" value="0" />
                                            <input type="hidden" class="name" />
                                            <input type="hidden" class="description" />
                                            <input type="hidden" class="markedForDelete" value="false" />
                                            <span class="kitGroupLabel" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(this, 'list', 'view');return false;"></span>
                                            <ul class="kitItems" style="display:none;">
                                                <li class="kitItemClone">
                                                    <input type="hidden" class="id" value="0" />
                                                    <input type="hidden" class="name" />
                                                    <input type="hidden" class="extraText" />
                                                    <input type="hidden" class="priceModifierPlusMinus" />
                                                    <input type="hidden" class="priceModifierDollars" />
                                                    <input type="hidden" class="priceModifierCents" />
                                                    <input type="hidden" class="markedForDelete" value="false" />
                                                    <span class="kitItemLabel" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(this, 'list', 'view');return false;"></span>
                                                </li>
                                                <li class="addKitItem">
                                                    <span onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Item.Add.showAddItemModal(this);return false;">Add Item</span>
                                                </li>
                                            </ul>
                                        </li>
                                        <li id="liKitGroupAddKitGroup" runat="server" class="addKitGroup collapsable">
                                            <input type="hidden" class="id" />
                                            <input type="hidden" class="name" />
                                            <input type="hidden" class="description" />
                                            <input type="hidden" class="markedForDelete" value="false" />
                                            <span class="kitGroupLabel addKitGroup"><a href="#AddGroup" style="color:maroon" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Group.Add.showAddItemModal(this);return false;">Add Group</a></span>
                                        </li>
                                    </ul>   
                                </td>
                                <td class="detail">
                                    <div class="groupDetails">
                                        <table>
                                            <caption>Group Detail</caption>
                                            <tbody>
                                                <tr>
                                                    <th scope="col">Name</th>
                                                    <td>
                                                        <span class="groupNameView"></span>
                                                        <input type="text" title="Required" class="groupNameEdit nameField" />
                                                        <span class="required">*</span>
                                                    </td>
                                                </tr>
                                                <tr class="stripe">
                                                    <th scope="col">Description</th>
                                                    <td>
                                                        <span class="groupDescriptionView"></span>
                                                        <textarea cols="45" rows="5" class="groupDescriptionEdit"></textarea>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                        <p class="requiredFieldNote">* required</p>
                                    </div>
                                    <div class="itemDetails">
                                        <table>
                                            <caption>Item Detail</caption>
                                            <tbody>
                                                <tr>
                                                    <th scope="col">Name</th>
                                                    <td>
                                                        <span class="itemNameView"></span>
                                                        <input type="text" class="itemNameEdit nameField" />
                                                        <span class="required">*</span>
                                                    </td>
                                                </tr>
                                                <tr class="stripe">
                                                    <th scope="col">Extra Text</th>
                                                    <td>
                                                        <span class="itemExtraTextView"></span>
                                                        <textarea cols="45" rows="5" class="itemExtraTextEdit"></textarea>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th scope="col">Price Modifier</th>
                                                    <td>
                                                        <span class="currencySymbol">
                                                            <%= GetCurrencySymbol() %>
                                                        </span>
                                                        <span class="itemPriceModifierView"></span>
                                                        <select class="itemPriceModifierAddSubtractEdit">
                                                            <option class="add" value="+" selected="selected">+</option>
                                                            <option class="subtract" value="-">-</option>
                                                        </select>
                                                        <input type="text" class="itemPriceModifierDollarsEdit"/>
                                                        <span class="decimalPoint">.</span>
                                                        <input type="text" class="itemPriceModifierCentsEdit" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                        <p class="requiredFieldNote">* required</p>
                                    </div>
                                    <p id="pKitGroupEditButtons" runat="server" class="itemEditButtons editButtons">
                                        <a href="#Cancel" class="button buttonRight redHover cancel" title="Cancel" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.Buttons.cancel(this);return false;">
                                            <asp:Image ID="imgKitCancel" runat="server" AlternateText="Cancel" ToolTip="Cancel" />
                                            Cancel
                                        </a>
                                        <a href="#OK" class="button buttonRight greenHover ok" title="OK" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.Buttons.ok(this);return false;">
                                            <asp:Image ID="imgKitOK" runat="server" AlternateText="OK" ToolTip="OK" />
                                            OK
                                        </a>
                                        <a href="#MarkForDelete" class="button buttonRight greenHover markForDelete" title="Mark For Delete" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.Buttons.markForDelete(this);return false;">
                                            <asp:Image ID="imgKitMarkForDelete" runat="server" AlternateText="Mark For Delete" ToolTip="Mark For Delete" />
                                            Mark For Delete
                                        </a>
                                        <a href="#Restore" class="button buttonRight blueHover restore" title="Restore" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.Buttons.restore(this);return false;">
                                            <asp:Image ID="imgKitRestore" runat="server" AlternateText="Restore" ToolTip="Restore" />
                                            Restore
                                        </a>
                                        <a href="#Edit" class="button buttonRight greenHover edit" title="Edit" onclick="Ektron.Commerce.CatalogEntry.Items.KitView.Display.show(this, 'button', 'edit');return false;">
                                            <asp:Image ID="imgKitEdit" runat="server" AlternateText="Edit" ToolTip="Edit" />
                                            Edit
                                        </a>
                                    </p>
                                </td>
                            </asp:PlaceHolder>
                        </tr>
                    </tbody>
                </table>
                <input id="hdnBlankNameFieldErrorMessage" runat="server" type="hidden" class="blankNameFieldError" />
            </div>
        </asp:View>
        <asp:View ID="vwSubscription" runat="server">
            <div class="subscriptionView">
                <asp:PlaceHolder ID="phSubscription" runat="server" Visible="true" />
            </div>
        </asp:View>
    </asp:MultiView>
    <asp:PlaceHolder ID="phModal" runat="server" Visible="true">
        <div id="ItemsModal" class="itemsTabModal ektronWindow">
            <h3 class="itemsModalHeader">
			    <asp:Image ID="imgCloseModal" runat="server" AlternateText="Close" ToolTip="Close" />
                <span id="spanDefaultModalTitle" runat="server" visible="False"></span>
                <span id="spanKitModalGroupTitle" class="kitGroupModalGroupTitle" runat="server" visible="False"></span>
                <span id="spanKitModalItemTitle" class="kitGroupModalItemTitle" runat="server" visible="False"></span>
            </h3>
            <asp:MultiView ID="mvItemsModalContent" runat="server">
                <asp:View ID="vwDefaultModalContent" runat="server">
                    <iframe id="iframeAddItemsModal" runat="server" frameborder="0" width="350px" height="300px" scrolling="auto"></iframe>
                    <p class="cancelAddItemModal clearfix">
                        <a href="#Close" class="button buttonRight redHover ektronModalClose" title="Close" onclick="return false;">
                            <asp:Image ID="imgCloseAddItemModal" runat="server" AlternateText="Close" ToolTip="Close" />
                            <span>Close</span>
                        </a>
                    </p>
                </asp:View>
                <asp:View ID="vwKitModalContent" runat="server">
                    <div class="addKitNode">
                        <div class="addKitGroup">
                            <table>
                                <tbody>
                                    <tr>
                                        <th>Group Name:</th>
                                        <td>
                                            <input type="text" class="addGroupName nameField" />
                                            <span class="required">*</span>
                                        </td>
                                    </tr>
                                    <tr class="stripe">
                                        <th>Group Description:</th>
                                        <td><textarea cols="25" rows="5" class="addGroupDescription"></textarea></td>
                                    </tr>
                                </tbody>
                            </table>
                            <p class="requiredFieldNote">* required</p>
                        </div>
                        <div class="addKitItem">
                            <table>
                                <tbody>
                                    <tr>
                                        <th>Item Name:</th>
                                        <td>
                                            <input type="text" class="addItemName nameField" />
                                            <span class="required">*</span>
                                        </td>
                                    </tr>
                                    <tr class="stripe">
                                        <th>Extra Text:</th>
                                        <td><textarea cols="25" rows="5" class="addItemExtraText"></textarea></td>
                                    </tr>
                                    <tr>
                                        <th>Price Modifier</th>
                                        <td>
                                            <span class="currencySymbol">
                                                <%= GetCurrencySymbol() %>
                                            </span>
                                            <select class="addItemPriceModifierAddSubtractEdit">
                                                <option class="add" selected="selected" value="+">+</option>
                                                <option class="subtract" value="-">-</option>
                                            </select>
                                            <input type="text" class="addItemPriceAmountDollarsEdit" value="0" />
                                            <span class="decimalPoint">.</span>
                                            <input type="text" class="addItemPriceAmountCentsEdit" value="00" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <p class="requiredFieldNote">* required</p>
                        </div>
                        <p class="addKitNodeButtons editButtons">
                            <a href="#Cancel" class="button buttonRight redHover" title="Cancel" onclick="Ektron.Commerce.CatalogEntry.Items.Modal.KitView.Buttons.cancel();return false;">
                                <asp:Image ID="imgKitViewModalCancel" runat="server" AlternateText="Cancel" ToolTip="Cancel" />
                                Cancel
                            </a>
                            <span class="blockUiWrapper">
                                <a href="#OK" class="button buttonRight greenHover ok" title="OK" onclick="Ektron.Commerce.CatalogEntry.Items.Modal.KitView.Buttons.ok(this);return false;">
                                    <asp:Image ID="imgKitViewModalOk" runat="server" AlternateText="OK" ToolTip="OK" />
                                    OK
                                </a>
                            </span>
                        </p>
                    </div>    
                </asp:View>
            </asp:MultiView>
        </div>
    </asp:PlaceHolder>
</div>
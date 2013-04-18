<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MediaDefaults.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.MediaDefaults" %>

<div class="EktronMediaDefaults">
    <asp:PlaceHolder ID="phData" runat="server">
        <input type="hidden" id="MediaDefaultsData" name="MediaDefaultsData" class="mediaDefaultsData" value="" />
    </asp:PlaceHolder>
    <table class="ektronGrid">
        <caption><% =GetCaption() %></caption>
        <thead>
            <tr class="title-header">
                <th><% =GetNameLabel() %></th>
                <th class="narrowColumn center"><% =GetWidthLabel() %></th>
                <th class="narrowColumn center"><% =GetHeightLabel() %></th>
                <asp:PlaceHolder ID="phActionsHeader" runat="server">
                    <th class="narrowColumn center"><% =GetActionsLabel() %></th>
                </asp:PlaceHolder>
            </tr>
        </thead>
        <tbody>
            <asp:Placeholder ID="phEmptyRow" runat="server">
                <tr class="mediaDefaultsEmptyRow">
                    <td colspan="<% =GetEmptyRowColspan() %>" class="center">
                        <asp:Literal ID="litEmptyRowLabel" runat="server" />
                    </td>
                </tr>
            </asp:Placeholder>
            <asp:Placeholder ID="phCloneRow" runat="server">
                <tr class="mediaDefaultsCloneRow skipStripe">
                    <td class="name">
                        <span class="prefix">[filename]</span>
                        <span class="value">&#160;</span>
                        <input type="text" class="name" name="mediaDefaults" title="" />
                        <span class="extension">[.extension]</span>
                        <span class="example">Example: Chair<span class="value">&#160;</span>.gif</span>
                    </td>
                    <td class="width narrowColumn center">
                        <span class="number">&#160;</span>
                        <input type="text" class="width" name="mediaDefaults" title="" />
                        <span class="units">px</span>
                    </td>
                    <td class="height narrowColumn center">
                        <span class="number">&#160;</span>
                        <input type="text" class="height" name="mediaDefaults" title="" />
                        <span class="units">px</span>
                    </td>
                    <td class="data narrowColumn center">
                        <input type="hidden" name="mediaDefaults" class="markedForDelete" value="false" />
                        <input type="hidden" name="mediaDefaults" class="id" value="" />
                        <input type="hidden" name="mediaDefaults" class="name" value="" />
                        <input type="hidden" name="mediaDefaults" class="width" value="" />
                        <input type="hidden" name="mediaDefaults" class="height" value="" />
                        <p class="actions">
                            <a href="#Edit" class="edit" title="Edit" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.edit(this);return false;">
                                <img alt="Edit" title="Edit" src="<% =GetImagePath() %>/revise.gif" />                                
                            </a>
                            <a href="#OK" class="ok" title="OK" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.ok(this);return false;">
                                <img alt="OK" title="OK" src="<% =GetImagePath() %>/reviseOK.gif" />                                
                            </a>
                            <a href="#Cancel" class="cancel" title="Cancel" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.cancel(this);return false;">
                                <img alt="Cancel" title="Cancel" src="<% =GetImagePath() %>/reviseCancel.gif" />                                
                            </a>
                            <a href="#MarkForDelete" class="markForDelete" title="Mark For Delete" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.markedForDelete(this);return false;">
                                <img alt="Mark For Delete" title="Mark For Delete" src="<% =GetImagePath() %>/toggleDelete.gif" />                                
                            </a>
                            <a href="#Restore" class="restore" title="Restore" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.restore(this);return false;">
                                <img alt="Restore" title="Restore" src="<% =GetImagePath() %>/toggleDeleteUndo.gif" />                                
                            </a>
                        </p>
                    </td>
                </tr>
            </asp:Placeholder>
            <asp:Repeater ID="rptAttributesView" runat="server" OnItemDataBound="rptAttributesView_OnItemDataBound">
                <ItemTemplate>
                    <tr class="mediaDefault">
                        <td class="name wideColumn">
                            <span class="prefix">[filename]</span>
                            <span class="value"><%# GetTitle(DataBinder.Eval(Container.DataItem, "Title"))%></span>
                            <input type="text" class="name" name="mediaDefaults" title="<%# GetTitle(DataBinder.Eval(Container.DataItem, "Title"))%>" value="<%# GetTitle(DataBinder.Eval(Container.DataItem, "Title"))%>" />
                            <span class="extension">[.extension]</span>
                            <span class="example">Example: Chair<span class="value"><%# GetTitle(DataBinder.Eval(Container.DataItem, "Title"))%></span>.gif</span>
                        </td>
                        <td class="width narrowColumn center">
                            <span class="number"><%# DataBinder.Eval(Container.DataItem, "Width") %></span>
                            <input type="text" class="width" name="mediaDefaults" title="<%# DataBinder.Eval(Container.DataItem, "Width") %>" value="<%# DataBinder.Eval(Container.DataItem, "Width") %>" />
                            <span class="units">px</span>
                        </td>
                        <td class="height narrowColumn center">
                            <span class="number"><%# DataBinder.Eval(Container.DataItem, "Height") %></span>
                            <input type="text" class="height" name="mediaDefaults" title="<%# DataBinder.Eval(Container.DataItem, "Height") %>" value="<%# DataBinder.Eval(Container.DataItem, "Height") %>" />
                            <span class="units">px</span>
                        </td>
                        <asp:PlaceHolder ID="phActions" runat="server">
                            <td class="data center narrowColumn">
                                <input type="hidden" name="mediaDefaults" class="markedForDelete" value="false" />
                                <input type="hidden" name="mediaDefaults" class="id" value="<%# DataBinder.Eval(Container.DataItem, "Id") %>" />
                                <input type="hidden" name="mediaDefaults" class="name" value="<%# GetTitle(DataBinder.Eval(Container.DataItem, "Title"))%>" />
                                <input type="hidden" name="mediaDefaults" class="width" value="<%# DataBinder.Eval(Container.DataItem, "Width") %>" />
                                <input type="hidden" name="mediaDefaults" class="height" value="<%# DataBinder.Eval(Container.DataItem, "Height") %>" />
                                <p class="actions">
                                    <a href="#Edit" class="edit" title="Edit" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.edit(this);return false;">
                                        <img alt="Edit Attribute" title="Edit Attribute" src="<% =GetImagePath() %>/revise.gif" />                                
                                    </a>
                                    <a href="#OK" class="ok" title="OK" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.ok(this);return false;">
                                        <img alt="OK" title="OK" src="<% =GetImagePath() %>/reviseOK.gif" />                                
                                    </a>
                                    <a href="#Cancel" class="cancel" title="Cancel" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.cancel(this);return false;">
                                        <img alt="Cancel" title="" src="<% =GetImagePath() %>/reviseCancel.gif" />                                
                                    </a>
                                    <a href="#MarkForDelete" class="markForDelete" title="Mark For Delete" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.markedForDelete(this);return false;">
                                        <img alt="Mark For Delete" title="Mark For Delete" src="<% =GetImagePath() %>/toggleDelete.gif" />                                
                                    </a>
                                    <a href="#Restore" class="restore" title="Restore" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Buttons.restore(this);return false;">
                                        <img alt="Restore" title="Restore" src="<% =GetImagePath() %>/toggleDeleteUndo.gif" />                                
                                    </a>
                                </p>
                            </td>
                        </asp:PlaceHolder>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    <asp:PlaceHolder ID="phAddThumbnail" runat="server">
        <p class="addThumbnail clearfix right">
            <a href="#Add" class="button buttonRight greenHover buttonAdd" title="<% =GetAddThumbnailLabel() %>" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Modal.show();return false;">
                <% =GetAddThumbnailLabel() %>
            </a>
        </p>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phModal" runat="server">
        <input type="hidden" class="localizedStrings" name="mediaDefaults" value='<% =GetLocalizedJavascriptStrings() %>' />
        <div id="MediaDefaultsModal" class="ektronWindow ektronModalWidth-25 ui-dialog ui-widget ui-widget-content ui-corner-all" id="AttributesModal">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix itemsModalHeader">
                <span class="ui-dialog-title dateSelectorHeader"><%=_MessageHelper.GetMessage("lbl add thumbnail")%></span>
                <a title="close" href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose">
                    <span class="ui-icon ui-icon-closethick">close</span>
                </a>
            </div>
            <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                <table class="ektronGrid">
                    <tbody>
                        <tr>
                            <th class="label"><%=_MessageHelper.GetMessage("generic name")%></th>
                            <td class="name">
                                <input class="name" type="text" name="mediaDefaults" />
                                <span class="required">*</span>
                            </td>
                        </tr>
                        <tr>
                            <th class="label"><%=_MessageHelper.GetMessage("lbl width")%></th>
                            <td class="width">
                                <input class="width" type="text" name="mediaDefaults" />
                                <span class="units">px</span>
                                <span class="required">*</span>
                            </td>
                        </tr>
                        <tr>
                            <th class="label"><%=_MessageHelper.GetMessage("lbl height")%></th>
                            <td class="height">
                                <input class="height" type="text" name="mediaDefaults" />
                                <span class="units">px</span>
                                <span class="required">*</span>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <p class="required"><% =GetRequiredText() %></p>
            </div>
            <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                <p class="addDefaultNodeButtons ektronModalButtonWrapper clearfix">
                    <a href="#Cancel" class="button buttonRight redHover buttonRemove" title="Cancel" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Modal.hide();return false;">
                        Cancel
                    </a>
                    <a href="#Ok" class="button buttonRight greenHover buttonAdd" title="Ok" onclick="Ektron.Commerce.ProductTypes.MediaDefaults.Add.add(this);return false;">
                        OK
                    </a>
                </p>
            </div>
        </div>
    </asp:PlaceHolder>
</div>
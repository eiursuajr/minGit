<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Attributes.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.Attributes" %>

<div class="EktronAttributes">
    <asp:PlaceHolder ID="phData" runat="server">
        <input type="hidden" id="AttributeData" name="AttributeData" class="attributeData" value="" />
        <input type="hidden" class="todaysDate" id="hdnTodaysDate" runat="server" />
        <input type="hidden" class="dateFormat" id="hdnDateFormat" runat="server" />
        <input type="hidden" class="localizedStrings" name="attributes" value='<% =GetLocalizedJavascriptStrings() %>' />
    </asp:PlaceHolder>
    <table class="ektronGrid">
        <thead>
            <tr class="title-header">
                <th class="width-percent-40"><%=_MessageHelper.GetMessage("generic name")%></th>
                <th class="width-percent-10"><%=_MessageHelper.GetMessage("lbl calatog entry id")%></th>
                <th class="width-percent-10 center"><%=_MessageHelper.GetMessage("generic type")%></th>
                <th class="center" id="StatusHeader"><%=_MessageHelper.GetMessage("lbl product type status")%></th>
                <th class="" id="DefaultValueHeader"><%=_MessageHelper.GetMessage("lbl default value")%></th>
            </tr>
        </thead>
        <tbody>
            <asp:Placeholder ID="phEmptyRow" runat="server">
                <tr class="attributeEmptyRow skipStripe">
                    <td colspan="4" class="center">
                        <asp:Literal ID="litEmptyRowLabel" runat="server" />
                    </td>
                </tr>
            </asp:Placeholder>
            <asp:Placeholder ID="phCloneRow" runat="server">
                <tr class="attributeCloneRow skipStripe">
                    <th class="name width-percent-40">
                        <span>&#160;</span>
                    </th>
                    <td class="id width-percent-10">
                        <span>&#160;</span>
                    </td>
                    <td class="type width-percent-10 center">
                        <span>&#160;</span>
                    </td>
                    <td class="publishedStatus center">
                        <span>&#160;</span>
                    </td>
                    <td class="value center">
                        <input type="hidden" name="attributes" class="markedForDelete" value="false" />
                        <input type="hidden" name="attributes" class="id" value="" />
                        <input type="hidden" name="attributes" class="type" value="" />
                        <input type="hidden" name="attributes" class="name" value="" />
                        <input type="hidden" name="attributes" class="value" value="" />
                        <p class="actions">
                            <a href="#Edit" class="edit" title="Edit" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.edit(this);return false;">
                                <img alt="Edit Attribute" title="Edit Attribute" src="<% =GetImagePath() %>/revise.gif" />                                
                            </a>
                            <a href="#OK" class="ok" title="OK" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.ok(this);return false;">
                                <img alt="OK" title="OK" src="<% =GetImagePath() %>/reviseOK.gif" />                                
                            </a>
                            <a href="#Cancel" class="cancel" title="Cancel" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.cancel(this);return false;">
                                <img alt="Cancel" title="Cancel" src="<% =GetImagePath() %>/reviseCancel.gif" />                                
                            </a>
                            <a href="#MarkForDelete" class="markForDelete" title="Mark For Delete" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.markedForDelete(this);return false;">
                                <img alt="Mark For Delete" title="Mark For Delete" src="<% =GetImagePath() %>/toggleDelete.gif" />                                
                            </a>
                            <a href="#Restore" class="restore" title="Restore" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.restore(this);return false;">
                                <img alt="Restore" title="Restore" src="<% =GetImagePath() %>/toggleDeleteUndo.gif" />                                
                            </a>
                        </p>
                        <p class="data">
                            <span class="view">&#160;</span>
                            <select class="boolean" name="attributes">
                                <option value="true" selected="selected">Yes</option>
                                <option value="false">No</option>
                            </select>
                            <input type="text" class="text" name="attributes" title="" />
                            <input type="text" class="numeric" name="attributes" title="" />
                        </p>
                    </td>
                </tr>
            </asp:Placeholder>
            <asp:Repeater ID="rptAttributesView" runat="server" OnItemDataBound="rptAttributesView_OnItemDataBound">
                <ItemTemplate>
                    <tr class="attribute">
                        <th class="name width-percent-40">
                            <span class="<%# GetInactiveStatusClass(Container.DataItem) %>"><%# DataBinder.Eval(Container.DataItem, "Name") %></span>
                        </th>
                        <td class="id width-percent-10">
                            <span><%# DataBinder.Eval(Container.DataItem, "Id") %></span>
                        </td>
                        <td class="type width-percent-10 center">
                            <span class="<%# GetInactiveStatusClass(Container.DataItem) %>"><%# GetDataTypeFriendlyLabel(DataBinder.Eval(Container.DataItem, "DataType")) %></span>
                        </td>
                        <td class="publishedStatus center">
                            <span class="<%# GetInactiveStatusClass(Container.DataItem) %>"><%# GetPublishedStatusFriendlyLabel(Container.DataItem) %></span>
                        </td>
                        <td class="value center">
                            <asp:PlaceHolder ID="phDataAndActions" runat="server">
                                <input type="hidden" name="attributes" class="markedForDelete" value="<%# GetInactiveStatusMarkedForDelete(Container.DataItem) %>" />
                                <input type="hidden" name="attributes" class="id" value="<%# DataBinder.Eval(Container.DataItem, "Id") %>" />
                                <input type="hidden" name="attributes" class="type" value="<%# DataBinder.Eval(Container.DataItem, "DataType") %>" />
                                <input type="hidden" name="attributes" class="name" value="<%# DataBinder.Eval(Container.DataItem, "Name") %>" />
                                <input type="hidden" name="attributes" class="value" value="<%# DataBinder.Eval(Container.DataItem, "DefaultValue") %>" />
                                <p class="actions">
                                    <a href="#Edit" class="edit" title="Edit" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.edit(this);return false;" <%# GetButtonInactiveStatusClass(Container.DataItem, "edit") %>>
                                        <img alt="Edit Attribute" title="Edit Attribute" src="<% =GetImagePath() %>/revise.gif" />                                
                                    </a>
                                    <a href="#OK" class="ok" title="OK" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.ok(this);return false;">
                                        <img alt="OK" title="OK" src="<% =GetImagePath() %>/reviseOK.gif" />                                
                                    </a>
                                    <a href="#Cancel" class="cancel" title="Cancel" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.cancel(this);return false;">
                                        <img alt="Cancel" title="" src="<% =GetImagePath() %>/reviseCancel.gif" />                                
                                    </a>
                                    <a href="#MarkForDelete" class="markForDelete" title="Mark For Delete" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.markedForDelete(this);return false;" <%# GetButtonInactiveStatusClass(Container.DataItem, "markedForDelete") %>>
                                        <img alt="Mark For Delete" title="Mark For Delete" src="<% =GetImagePath() %>/toggleDelete.gif" />                                
                                    </a>
                                    <a href="#Restore" class="restore" title="Restore" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.restore(this);return false;" <%# GetButtonInactiveStatusClass(Container.DataItem, "restore") %>>
                                        <img alt="Restore" title="Restore" src="<% =GetImagePath() %>/toggleDeleteUndo.gif" />                                
                                    </a>
                                </p>
                            </asp:PlaceHolder>
                            <p class="data">
                                <span class="<%# GetInactiveStatusClass(Container.DataItem) %>view">
                                    <%# GetFriendlyDefaultValue(DataBinder.Eval(Container.DataItem, "DefaultValue"))%>
                                </span>
                                <asp:PlaceHolder ID="phBoolean" runat="server">
                                    <select class="boolean" name="attributes">
                                        <option value="true" <%# GetBooleanSelectedItem(Container.DataItem, true) %>>Yes</option>
                                        <option value="false" <%# GetBooleanSelectedItem(Container.DataItem, false) %>>No</option>
                                    </select>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phText" runat="server">
                                    <input type="text" class="text" name="attributes" title="<%# DataBinder.Eval(Container.DataItem, "DefaultValue") %>" value="<%# DataBinder.Eval(Container.DataItem, "DefaultValue") %>" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phNumeric" runat="server">
                                    <input type="text" class="numeric" name="attributes" title="<%# DataBinder.Eval(Container.DataItem, "DefaultValue") %>" value="<%# DataBinder.Eval(Container.DataItem, "DefaultValue") %>" />
                                </asp:PlaceHolder>
                            </p>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    <asp:PlaceHolder ID="phFooter" runat="server">
        <p class="addAttribute clearfix">
            <a href="#AddAttribute" class="button buttonRight greenHover buttonAdd" title="Add Attribute" onclick="Ektron.Commerce.ProductTypes.Attributes.Modal.show('newAttribute');return false;">
                <%=_MessageHelper.GetMessage("lbl add attribute")%>
            </a>
        </p>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phModal" runat="server">
        <div class="ektronWindow ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="AttributesModal">
            <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix itemsModalHeader">
                <span class="ui-dialog-title fieldHeader"><%=_MessageHelper.GetMessage("lbl add attribute")%></span>
                <span class="ui-dialog-title dateSelectorHeader"><%=_MessageHelper.GetMessage("lbl coupon select date")%></span>
                <a href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose">
                    <span class="ui-icon ui-icon-closethick"><%=_MessageHelper.GetMessage("close title")%></span>
                </a>
            </div>
            <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                <div id="EktronAttributesDatePicker"></div>
			    <table class="ektronGrid">
                    <thead>
                        <tr class="title-header header">
                            <th><%=_MessageHelper.GetMessage("generic name")%></th>
                            <th class="center"><%=_MessageHelper.GetMessage("generic type")%></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr class="fields">
                            <td class="name">
                                <input class="name" type="text" name="attributes" />
                                <span class="required">*</span>
                                <input id="EktronAttributesDatePickerData" type="hidden" class="selectedDate" name="attributes" />
                                <div id="EktronAttributesDatePicker"></div>
                                <p class="required" style="clear:both">* <%=_MessageHelper.GetMessage("lbl field cannot be blank")%></p>
                            </td>
                            <td class="type center">
                                <select name="attributes" class="type">
                                    <option value="text"><asp:Literal ID="litAddAttributeOptionText" runat="server" /></option>
                                    <option value="date"><asp:Literal ID="litAddAttributeOptionDate" runat="server" /></option>
                                    <option value="numeric"><asp:Literal ID="litAddAttributeOptionNumeric" runat="server" /></option>
                                    <option value="boolean"><asp:Literal ID="litAddAttributeOptionBoolean" runat="server" /></option>
                                </select>
                            </td>
                        </tr>
                    </tbody>
                </table>
		    </div>
	        <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
	            <ul class="ektronModalButtonWrapper ui-helper-clearfix">
                    <li><a href="#Cancel" class="button buttonRight redHover buttonClear" title="Cancel" onclick="Ektron.Commerce.ProductTypes.Attributes.Modal.hide();return false;"><%=_MessageHelper.GetMessage("btn cancel")%></a></li>	                
	                <li><a href="#Ok" class="button buttonRight greenHover buttonAdd ok" title="Ok" onclick="Ektron.Commerce.ProductTypes.Attributes.Add.add(this);return false;"><%=_MessageHelper.GetMessage("btn ok")%></a></li>
                    <li><a href="#SetDate" class="button buttonRight greenHover buttonAdd setDate" title="Ok" onclick="Ektron.Commerce.ProductTypes.Attributes.Buttons.setDate(this);return false;">OK</a></li>   
                </ul>
            </div>
        </div>
    </asp:PlaceHolder>
</div>
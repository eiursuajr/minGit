<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.List.List" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Coupon List</title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    </head>
    <body>
        <div class="ektron">
            <form id="formCouponList" runat="server">
                <div id="dhtmltooltip"></div>
                <div class="ektronPageHeader">
	                <table style="width: 100%; border-collapse: collapse;" class="baseClassToolbar">
		                <tbody>
		                    <tr>
			                    <td class="ektronTitlebar">
		                            <asp:Literal ID="litCouponHeader" runat="server" />
                                    <a id="aHelp" runat="server" class="help">
                                        <img alt="<% =_MessageHelper.GetMessage('generic help')%>" id="imgHelp" runat="server" />
                                    </a>
		                        </td>
		                    </tr>
	                    </tbody>
	                </table>
                </div>
                <asp:MultiView ID="mvPermissions" runat="server">
                    <asp:View ID="vwInavlidPersmissions" runat="server">
                        <div class="invalidPermissions">
                            <table class="ektronGrid">
                                <tbody>
                                    <tr class="center">
                                        <asp:Literal ID="litInvalidPermissions" runat="server" />
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </asp:View>
                    <asp:View ID="vwValidPermissions" runat="server">
                        <asp:ScriptManager ID="smCouponList" runat="server" />
                        <div class="couponList">
                            <input type="hidden" id="CouponListClientData" name="CouponListClientData" class="CouponListClientData" value="" />
                            <input type="hidden" id="CouponListLocalizedStrings" name="CouponList" class="CouponListLocalizedStrings" value="" runat="server" />
                            <div class="actions">
                                <asp:PlaceHolder ID="phActions" runat="server">
                                    <ul>
                                        <li class="addCoupon"><a class="primary addButton" href="../Add/Add.aspx" title="<% =GetLocalizedStringAddCoupon() %>"><%= GetLocalizedStringAddCoupon() %></a></li>
                                        <li class="save"><asp:LinkButton ID="lbSave" runat="server" OnClientClick="return Ektron.Commerce.Coupons.List.Actions.confirmSave();" OnCommand="Save_Click" /></li>                                        
                                        <li class="cancel">
                                            <a href="#Cancel" onclick="Ektron.Commerce.Coupons.List.Modal.Cancel.show();return false;" title="<%= GetLocalizedStringCancel() %>"><asp:Image ID="imgCancel" runat="server" />
                                            </a>
                                            </li>
                                    </ul>
                                </asp:PlaceHolder>
                            </div>
                            <asp:UpdatePanel ID="upCouponList" runat="server" EnableViewState="true" UpdateMode="Conditional" ChildrenAsTriggers="true" RenderMode="Block">
                                <ContentTemplate>
                                    <asp:PlaceHolder ID="phSearch" runat="server">
                                        <div class="clearfix">
                                            <p class="search">
                                                <label for="txtSearch"><asp:Literal ID="searchLabelText" runat="server" /></label>
                                                <asp:TextBox ID="txtSearch" runat="server" CssClass="search" AutoCompleteType="None" />
                                                <span><asp:Button ToolTip="<% =_MessageHelper.GetMessage('generic search')%>" ID="btnSearch" runat="server" OnCommand="Search_Click" CssClass="ektronWorkareaSearch" OnClientClick="Ektron.Commerce.Coupons.List.Actions.checkSearchState();" CommandName="Search" /></span>
                                                <span><asp:ImageButton ToolTip="<% =_MessageHelper.GetMessage('generic clear all msg')%>" ID="ibClearSearch" runat="server" OnCommand="Search_Click" CommandName="Clear" CssClass="clearSearch hide" /></span>
                                            </p>
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="coupons">
                                        <asp:HiddenField ID="hdnOrderByField" runat="server" Value="Id" />
                                        <asp:HiddenField ID="hdnOrderDirection" runat="server" Value="ascending" />
                                        <div class="couponListPublished">
                                            <input type="hidden" id="CouponListMarkedForDelete" name="CouponList" class="CouponListMarkedForDelete" value="" />
                                            <asp:MultiView ID="mvPublishedCoupons" runat="server">
                                                <asp:View ID="vwNoCoupons" runat="server">
                                                    <div class="ektronPageContainer">
                                                        <table class="ektronGrid">
                                                            <thead>
                                                                <tr class="title-header">
                                                                    <th class="center"><% =_MessageHelper.GetMessage("generic id")%></th>
                                                                    <th class="center"><% =_MessageHelper.GetMessage("enabled")%></th>
                                                                    <th class="center"><% =_MessageHelper.GetMessage("lbl code")%></th>
                                                                    <th class="center"><% =_MessageHelper.GetMessage("lbl currency")%></th>
                                                                    <th class="center"><% =_MessageHelper.GetMessage("lbl coupon desc")%></th>
                                                                    <th class="center"><% =_MessageHelper.GetMessage("lbl Count")%></th>
                                                                    <th class="center"><% =_MessageHelper.GetMessage("lbl start")%></th>
                                                                    <th class="center"><% =_MessageHelper.GetMessage("lbl end")%></th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr class="center">
                                                                    <td colspan="7" class="center">
                                                                        <asp:Literal ID="litNoCoupons" runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </asp:View>
                                                <asp:View ID="vwCoupons" runat="server">
                                                    <table class="coupons ektronGrid">
                                                        <thead>
                                                            <tr class="title-header" title="<%= GetLocalizedStringHeaderRowTitle() %>">
                                                                <th class="center"><asp:LinkButton ID="lbId" runat="server" OnCommand="Sorting_Click" CommandName="Id" CssClass="ascending" /></th>
                                                                <th class="center"><asp:LinkButton ID="lbEnabled" runat="server" OnCommand="Sorting_Click" CommandName="IsActive" /></th>
                                                                <th class="center"><asp:LinkButton ToolTip="Redeemable" ID="lbRedeemable" runat="server" OnCommand="Sorting_Click" CommandName="IsRedeemable"/></th>
                                                                <th class="center"><asp:LinkButton ID="lbCode" runat="server" OnCommand="Sorting_Click" CommandName="Code" /></th>
                                                                <th class="center"><asp:LinkButton ID="lbCurrency" runat="server" OnCommand="Sorting_Click" CommandName="CurrencyId" /></th>
                                                                <th class="center"><asp:LinkButton ID="lbDescription" runat="server" OnCommand="Sorting_Click" CommandName="Description" /></th>
                                                                <th class="center"><asp:LinkButton ToolTip="Count" ID="lbCount" runat="server" OnCommand="Sorting_Click" CommandName="Count" /></th>
                                                                <th class="center"><asp:LinkButton ID="lbStartDate" runat="server" OnCommand="Sorting_Click" CommandName="StartDate" /></th>
                                                                <th class="center"><asp:LinkButton ID="lbEndDate" runat="server" OnCommand="Sorting_Click" CommandName="ExpirationDate" /></th>
                                                                <asp:PlaceHolder ID="phMarkForDeleteHeader" runat="server" >
                                                                    <th class="markForDelete center">
                                                                        <img src="<%= GetImagesPath("list") %>/<%= GetDeleteImage() %>" class="markForDelete" alt="<%= GetLocalizedStringMarkForDeleteAll() %>" title="<%= GetLocalizedStringMarkForDeleteAll() %>" onclick="Ektron.Commerce.Coupons.List.Actions.markForDeletePage();" />
                                                                        <img src="<%= GetImagesPath("list") %>/<%= GetRestoreImage() %>" class="restore" alt="<%= GetLocalizedStringRestoreAll() %>" title="<%= GetLocalizedStringRestoreAll() %>"  onclick="Ektron.Commerce.Coupons.List.Actions.restorePage();"/>
                                                                    </th>
                                                                </asp:PlaceHolder>
                                                            </tr>
                                                        </thead>
                                                        <asp:PlaceHolder ID="phPaging" runat="server">
                                                            <tfoot>
                                                                <tr>
                                                                    <td class="paging" colspan="<%= GetFooterColspan() %>">
                                                                        <ul>
                                                                            <li><asp:ImageButton ToolTip="<% =_MessageHelper.GetMessage('lbl pagecontrol first page') %>" ID="ibFirstPage" runat="server" OnCommand="Paging_Click" CommandName="FirstPage" /></li>
                                                                            <li><asp:ImageButton ToolTip="<% =_MessageHelper.GetMessage('lbl pagecontrol previous page') %>" ID="ibPreviousPage" runat="server" OnCommand="Paging_Click" CommandName="PreviousPage" /></li>
                                                                            <li><asp:ImageButton ToolTip="<% =_MessageHelper.GetMessage('lbl pagecontrol next page') %>" ID="ibNextPage" runat="server" OnCommand="Paging_Click" CommandName="NextPage" /></li>
                                                                            <li>
                                                                                <asp:ImageButton ToolTip="<% =_MessageHelper.GetMessage('lbl pagecontrol last page') %>Last Page" ID="ibLastPage" runat="server" OnCommand="Paging_Click" CommandName="LastPage" />
                                                                                <asp:HiddenField ID="hdnTotalPages" runat="server" />
                                                                            </li>
                                                                        </ul>
                                                                        <p>
                                                                            <span class="page"><asp:Literal ID="litPage" runat="server" /></span>
                                                                            <span class="pageNumber"><asp:TextBox CssClass="currentPage" ID="txtPageNumber" runat="server"></asp:TextBox></span>
                                                                            <span class="pageOf"><asp:Literal ID="litOf" runat="server" /></span>
                                                                            <span class="pageTotal"><asp:Literal ID="litTotalPages" runat="server" /></span>
                                                                            <asp:ImageButton ID="ibPageGo" CssClass="adHocPage" runat="server" OnCommand="Paging_Click" CommandName="AdHocPage" />
                                                                        </p>
                                                                    </td>
                                                                </tr>
                                                            </tfoot>
                                                        </asp:PlaceHolder>
                                                        <tbody>
                                                            <asp:Repeater ID="rptCouponList" runat="server" OnItemDataBound="rptCouponList_DataBound">
                                                                <ItemTemplate>
                                                                    <tr<%# GetStripeRow(Container.ItemIndex) %> title="<%= GetLocalizedStringCouponRowTitle() %>">
                                                                        <td class="couponData center"><%# DataBinder.Eval(Container.DataItem, "Id") %></td>
                                                                        <td class="couponData center"><span class="<%# GetBooleanClass((bool)Eval("IsActive")) %>"><%# GetBooleanFriendlyName((bool)Eval("IsActive"))%></span></td>
                                                                        <td class="couponData center"><span class="<%# GetBooleanClass((bool)Eval("IsRedeemable")) %>"><%# GetBooleanFriendlyName((bool)Eval("IsRedeemable"))%></span></td>
                                                                        <td class="couponData center"><%# DataBinder.Eval(Container.DataItem, "Code") %></td>
                                                                        <td class="couponData center"><%# GetCurrencyFriendlyName(Container.DataItem) %></td>
                                                                        <td class="couponData center"><%# DataBinder.Eval(Container.DataItem, "Description") %></td>
                                                                        <td class="couponData center"><%# DataBinder.Eval(Container.DataItem, "UseCount") %></td>
                                                                        <td class="couponData center"><%# DataBinder.Eval(Container.DataItem, "StartDate") %></td>
                                                                        <td class="couponData center"><%# DataBinder.Eval(Container.DataItem, "ExpirationDate") %></td>
                                                                        <td class="couponActions center">
                                                                            <input type="hidden" class="id" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "Id") %>" />
                                                                            <asp:PlaceHolder ID="phMarkForDeleteTableCell" runat="server">
                                                                                    <input type="hidden" class="isEnabled" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "IsActive") %>" />
                                                                                    <input type="hidden" class="isEnabledFriendlyName" name="CouponList" value="<%# GetBooleanFriendlyName((bool)Eval("IsActive")) %>" />
                                                                                    <input type="hidden" class="code" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "Code") %>" />
                                                                                    <input type="hidden" class="currencyId" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "CurrencyId") %>" />
                                                                                    <input type="hidden" class="currencyName" name="CouponList" value="<%# GetCurrencyFriendlyName(Container.DataItem) %>" />
                                                                                    <input type="hidden" class="description" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "Description") %>" />
                                                                                    <input type="hidden" class="useCount" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "UseCount") %>" />
                                                                                    <input type="hidden" class="startDate" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "StartDate") %>" />
                                                                                    <input type="hidden" class="expirationDate" name="CouponList" value="<%# DataBinder.Eval(Container.DataItem, "ExpirationDate") %>" />
                                                                                    <input type="hidden" class="markedForDelete" name="CouponList" value="false" />
                                                                                    <img src="<%= GetImagesPath("list") %>/markForDelete.gif" class="markForDelete" alt="<%= GetLocalizedStringMarkForDelete() %>" title="<%= GetLocalizedStringMarkForDelete() %>" onclick="Ektron.Commerce.Coupons.List.Actions.markForDelete(this);" />
                                                                                    <img src="<%= GetImagesPath("list") %>/restore.gif" class="restore" alt="<%= GetLocalizedStringRestore() %>" title="<%= GetLocalizedStringRestore() %>"  onclick="Ektron.Commerce.Coupons.List.Actions.restore(this);"/>
                                                                            </asp:PlaceHolder>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tbody>
                                                    </table>
                                                </asp:View>
                                            </asp:MultiView>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:PlaceHolder ID="phModal" runat="server">
                                <div id="EktronCouponListModal" class="ektronModalWidth-50 ektronWindow ektronModalWidth-25 ui-dialog ui-widget ui-widget-content ui-corner-all">
                                    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix itemsModalHeader header">
                                        <span class="ui-dialog-title header">
                                            <span id="confirmCancelHeader" class="modalHeader"><asp:Literal ID="litConfirmCancelHeader" runat="server" /></span>
                                            <span id="confirmSaveHeader" class="modalHeader"><asp:Literal ID="litConfirmSaveHeader" runat="server" /></span>
                                        </span>
                                        <a class="ui-dialog-titlebar-close ui-corner-all ektronModalClose" href="#" onclick="Ektron.Commerce.Coupons.List.Modal.Save.cancel();return false;">
                                            <span class="ui-icon ui-icon-closethick">close</span>
                                        </a>
                                    </div>
                                    <div class="ui-dialog-content ui-widget-content ektronPageInfo body">
                                        <div id="EktronCouponConfirmCancel" class="modalBody">
                                            <p class="warning"><asp:Literal ID="litConfirmCancelMessage" runat="server" /></p>
                                        </div>
                                        <div id="EktronCouponConfirmSave" class="modalBody">
                                            <p class="warning"><asp:Literal ID="litConfirmSaveMessage" runat="server" /></p>
                                            <div class="couponsMarkedForDelete">
                                                <table class="ektronGrid couponsMarkedForDelete">
                                                    <thead>
                                                         <tr class="title-header">
                                                            <th class="center"><%= GetLocalizedStringIdHeader() %></th>
                                                            <th class="center"><%= GetLocalizedStringCodeHeader() %></th>
                                                            <th class="center"><%= GetLocalizedStringCurrencyHeader() %></th>
                                                            <th class="center"><%= GetLocalizedStringDescriptionHeader() %></th>
                                                            <th class="restore center"><img src="<%= GetImagesPath("list") %>/restore.png" alt="<%= GetLocalizedStringRestore() %>" title="<%= GetLocalizedStringRestore() %>" /></th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr class="cloneRow">
                                                            <td class="id center"></td>
                                                            <td class="code"></td>
                                                            <td class="currencyName"></td>
                                                            <td class="description"></td>
                                                            <td class="restore center">
                                                                <input type="hidden" class="id" name="CouponList" value="" />
                                                                <img src="<%= GetImagesPath("list") %>/restore.gif" alt="<%=GetLocalizedStringRestore() %>" title="<%= GetLocalizedStringRestore() %>" onclick="Ektron.Commerce.Coupons.List.Modal.Save.restore(this);" />
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="footer ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                                        <div id="confirmCancelFooter" class="modalFooter">
                                            <p class="modalActions clearfix">
                                                <a href="#No" class="button buttonRight redHover buttonCancel" title="<% =_MessageHelper.GetMessage("btn cancel")%>" onclick="Ektron.Commerce.Coupons.List.Modal.Cancel.no();return false;">
                                                    <asp:Literal ID="litCancelNo" runat="server" />
                                                </a>
                                                <a href="#Yes" class="button buttonRight greenHover buttonOk" title="<% =_MessageHelper.GetMessage("btn ok")%>" onclick="Ektron.Commerce.Coupons.List.Modal.Cancel.yes();return false;">
                                                    <asp:Literal ID="litCancelYes" runat="server" />
                                                </a>
                                            </p>
                                        </div>
                                        <div id="confirmSaveFooter" class="modalFooter">
                                            <p class="modalActions">
                                                <a title="<% =_MessageHelper.GetMessage("btn cancel")%>" href="#Cancel" class="button buttonRight redHover buttonCancel" onclick="Ektron.Commerce.Coupons.List.Modal.Save.cancel();return false;">
                                                    Cancel
                                                </a>
                                                <a title="<% =_MessageHelper.GetMessage("btn ok")%>" href="#OK" class="button buttonRight greenHover buttonOk" onclick="Ektron.Commerce.Coupons.List.Modal.Save.ok();return false;">
                                                    OK
                                                </a>
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </form>
        </div>
    </body>
</html>

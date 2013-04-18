<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Properties.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Properties.Properties" %>
<%@ OutputCache NoStore="true" Location="None" %>
<%@ Register TagPrefix="ucEktron" TagName="Type" Src="../SharedComponents/Type/Type.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Amount" Src="../SharedComponents/Type/Amount.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Percent" Src="../SharedComponents/Type/Percent.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Scope" Src="../SharedComponents/Scope/Scope.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Items" Src="../SharedComponents/Scope/Items.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Coupon Properties</title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
        <script type="text/javascript">
            function closeModal()
            {
                if ("function" == typeof parent.EktronUiDialogClose) 
                {
                    parent.EktronUiDialogClose("coupon");
                } 
                else 
                {
                    parent.ektb_remove();
                } 
            }
        </script>
    </head>
    <body>
        <div class="ektron">
            <form id="formCouponProperties" runat="server">
                <asp:MultiView ID="mvPermissions" runat="server">
                    <asp:View ID="vwInavlidPersmissions" runat="server">
                        <div class="invalidPermissions">
                            <p>
                                <span><asp:Literal ID="litInvalidPermissions" runat="server" /></span>
                            </p>
                        </div>
                    </asp:View>
                    <asp:View ID="vwValidPermissions" runat="server">
                        <asp:ScriptManager ID="smCouponPropeties" runat="server" />
                        <div class="couponProperties">
                            <div class="ektronTitlebar">
                                <asp:Literal ID="litCouponHeader" runat="server" />
                            </div>
                            <div class="actions limitedActions" id="divActions" runat="server">
                                <ul>
                                    <li class="save"><span id="spanSaveDisabled" class="saveDisabled" runat="server" /><asp:LinkButton ToolTip="Save" ID="lbSave" runat="server" OnClientClick="Ektron.Commerce.Coupons.Properties.Modal.show('save');return false;" OnCommand="Save_Click" /></li>
                                    <li class="delete"><span id="spanDeleteDisabled" class="deleteDisabled" runat="server" /><asp:LinkButton ToolTip="Delete" ID="lbDelete" runat="server" OnClientClick="Ektron.Commerce.Coupons.Properties.Modal.show('delete');return false;" OnCommand="Delete_Click" /></li>
                                    <li class="cancel"><asp:LinkButton ToolTip="Cancel" ID="lbCancel" runat="server" OnCommand="Cancel_Click" /></li>   
                                    <li class="exit"><asp:LinkButton ToolTip="Exit" ID="lbExit" runat="server" OnCommand="Exit_Click" /></li>   
                                    <li class="help"><a id="aHelp" runat="server" class="help"><img title="Help" alt="Help" id="imgHelp" runat="server" /></a></li> 
                                </ul>
                            </div>
                            <asp:UpdatePanel ID="upCouponProperties" runat="server" EnableViewState="true" UpdateMode="Conditional" ChildrenAsTriggers="true" RenderMode="Block">
                                <ContentTemplate>
                                    <div class="coupon">
                                        <asp:MultiView ID="MainView" runat="server" >
                                            <asp:View ID="InalidCouponView" runat="server" >
                                                <p class="invalidCoupon"><asp:Literal ID="litInvalidCoupon" runat="server" /></p>
                                            </asp:View>
                                            <asp:View ID="DeletedCouponView" runat="server" >
                                                <p class="deletedCoupon"><asp:Literal ID="litDeletedCoupon" runat="server" />
                                                <ul>
                                                    <li><asp:Hyperlink ID="aList" runat="server"></asp:Hyperlink></li>
                                                </ul>
                                                </p>
                                            </asp:View>
                                            <asp:View ID="NormalView" runat="server" >
                                                <div class="TabControls ektronPageTabbed ektronPageInfo">
                                                    <div class="ui-tabs ui-widget ui-widget-content ui-corner-all">
                                                        <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
                                                            <li id="liType" runat="server" class="ui-state-default ui-corner-top ui-tabs-selected ui-state-active"><asp:LinkButton ID="lbType" CssClass="InactiveTab" OnClientClick="return Ektron.Commerce.Coupons.Properties.Navigation.tabClick()" OnCommand="Type_Click" runat="server" /></li>
                                                            <li id="liDiscount" runat="server" class="ui-state-default ui-corner-top"><asp:LinkButton ID="lbDiscount" CssClass="InactiveTab" OnClientClick="return Ektron.Commerce.Coupons.Properties.Navigation.tabClick()" OnCommand="Discount_Click" runat="server" /></li>
                                                            <li id="liScope" runat="server" class="ui-state-default ui-corner-top"><asp:LinkButton ID="lbScope" CssClass="InactiveTab" OnClientClick="return Ektron.Commerce.Coupons.Properties.Navigation.tabClick()" OnCommand="Scope_Click" runat="server" /></li>
                                                            <li id="liItems" runat="server" class="ui-state-default ui-corner-top"><asp:LinkButton ID="lbItems" CssClass="InactiveTab" OnClientClick="return Ektron.Commerce.Coupons.Properties.Navigation.tabClick()" OnCommand="Items_Click" runat="server" /></li>
                                                        </ul>
                                                        <div class="TabAreas">
                                                            <asp:MultiView ID="Tabs" runat="server">
                                                                <asp:View ID="TypeTabView" runat="server">
                                                                    <ucEktron:Type ID="ucTypeControl" runat="server" />
                                                                </asp:View>
                                                                <asp:View ID="DiscountTabView" runat="server">
                                                                    <ucEktron:Amount ID="ucAmountControl" runat="server" />
                                                                    <ucEktron:Percent ID="ucPercentControl" Visible="false" runat="server" />
                                                                </asp:View>
                                                                <asp:View ID="ScopeTabView" runat="server">
                                                                    <ucEktron:Scope ID="ucScopeControl" runat="server" />
                                                                </asp:View>
                                                                <asp:View ID="ItemsTabView" runat="server">
                                                                    <ucEktron:Items ID="ucItemsControl" runat="server" />
                                                                </asp:View>
                                                            </asp:MultiView>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:View>
                                        </asp:MultiView>
                                        <input type="hidden" id="hfEditable" value="<%= this.IsEditable.ToString() %>" />
                                        <input type="hidden" id="hfHomePage" value="<%= HOME_PAGE_URL %>" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:PlaceHolder ID="phModal" runat="server">
                                <div id="EktronCouponsPropertiesModal" class="ektronWindow ektronModalWidth-25 ui-dialog ui-widget ui-widget-content ui-corner-all">
                                    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix propertiesModalHeader">
                                        <span class="ui-dialog-title header">
                                            <span id="confirmDeleteHeader"><asp:Literal ID="litConfirmDeleteHeader" runat="server" /></span>
                                            <span id="confirmSaveHeader"><asp:Literal ID="litConfirmSaveHeader" runat="server" /></span>
                                            <input type="hidden" class="modalType" name="properties" />
                                        </span>
                                        <a class="ui-dialog-titlebar-close ui-corner-all ektronModalClose" href="#">
                                            <span class="ui-icon ui-icon-closethick">Close Window</span>
                                        </a>
                                    </div>
                                    <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                                       <p id="confirmDeleteMessage" class="warning"><asp:Literal ID="litConfirmDeleteMessage" runat="server" /></p>
                                       <p id="confirmSaveMessage" class="warning"><asp:Literal ID="litConfirmSaveMessage" runat="server" /></p>
                                    </div>
                                    <div class="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                                        <p class="addDefaultNodeButtons ektronModalButtonWrapper clearfix">
                                            <a onclick="Ektron.Commerce.Coupons.Properties.Modal.hide();return false;" title="Cancel" class="button buttonRight redHover buttonRemove" href="#Cancel">
                                                <%=this.GetMessage("resolve cancel button")%>
                                            </a>
                                            <a onclick="Ektron.Commerce.Coupons.Properties.Modal.ok();return false;" title="Ok" class="button buttonRight greenHover buttonAdd" href="#OK">
                                                <%=this.GetMessage("btn ok")%>
                                            </a>    
                                        </p>
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

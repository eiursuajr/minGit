<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Add.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Add.Add" %>
<%@ Register TagPrefix="ucEktron" TagName="Finish" Src="Finish.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Scope" Src="../SharedComponents/Scope/Scope.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Items" Src="../SharedComponents/Scope/Items.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Type" Src="../SharedComponents/Type/Type.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Amount" Src="../SharedComponents/Type/Amount.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="Percent" Src="../SharedComponents/Type/Percent.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>Add Coupon</title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    </head>
    <body>
        <div class="ektron">
            <form id="formCouponAdd" runat="server">
                <div class="couponAdd" id="divCouponAdd" runat="server">
                    <asp:PlaceHolder ID="phModeView" runat="server">
                        <div class="invalidPermissions">
                            <p>
                                <span><asp:Literal ID="litInvalidPermissions" runat="server" /></span>
                            </p>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phModeEdit" runat="server">
                        <asp:ScriptManager ID="smCouponAdd" runat="server" />
                            <input type="hidden" id="CouponListLocalizedStrings" name="CouponList" class="CouponListLocalizedStrings" value="" runat="server" />
                            <div class="ektronTitlebar">
                                <asp:Literal ID="litAddCouponHeader" runat="server" />
                            </div>
                            <div class="ektronToolbar">
								<table>
									<tr>
										<td>
											<a class="primary backButton" href="../List/List.aspx" title="View All Coupons" id="aBack" runat="server">
												<asp:Image ToolTip="Back" ID="imgBack" runat="server" />
											</a>
										</td>
										<%=StyleHelper.ActionBarDivider %>
										<td>
											<a id="aHelp" runat="server" class="help">
												<img title="Help" id="imgHelp" runat="server" />
											</a>
										</td>
									</tr>
								</table>
                            </div>
                            <div class="couponAddWizard">
                            <asp:UpdatePanel ID="upCouponList" runat="server" EnableViewState="true" UpdateMode="Conditional" ChildrenAsTriggers="true" RenderMode="Block">
                                <ContentTemplate>
                                    <asp:Wizard 
                                        ID="wzCouponAdd" 
                                        runat="server" 
                                        OnFinishButtonClick="wzCouponAdd_OnFinishButtonClick" 
                                        DisplayCancelButton="true" 
                                        CancelDestinationPageUrl="../List/List.aspx" 
                                        BorderColor="#79b7e7" 
                                        BorderStyle="Solid" 
                                        BorderWidth="1"
                                        NavigationStyle-CssClass="navigation" 
                                        NavigationStyle-HorizontalAlign="Right"
                                        SideBarStyle-CssClass="sidebar" 
                                        CssClass="wizardTable">
                                        <WizardSteps>
                                            <asp:WizardStep ID="wsType" runat="server" StepType="Start">
                                                <ucEktron:Type ID="ucType" runat="server" />
                                            </asp:WizardStep>
                                            <asp:WizardStep ID="wsDiscount" runat="server" StepType="Step" OnLoad="wsDiscount_OnLoad">
                                                <asp:MultiView ID="mvDiscount" runat="server">
                                                    <asp:View ID="vwAmount" runat="server">
                                                        <ucEktron:Amount ID="ucAmount" runat="server" />
                                                    </asp:View>
                                                    <asp:View ID="vwPercent" runat="server">
                                                        <ucEktron:Percent ID="ucPercent" runat="server" />
                                                    </asp:View>
                                                </asp:MultiView>
                                            </asp:WizardStep>
                                            <asp:WizardStep ID="wsScope" runat="server" StepType="Step">
                                                <ucEktron:Scope ID="ucScope" runat="server" />
                                            </asp:WizardStep>
                                            <asp:WizardStep ID="wsItems" runat="server" StepType="Finish">
                                                <ucEktron:Items ID="ucItems" runat="server" />
                                            </asp:WizardStep>
                                            <asp:WizardStep ID="wsFinish" runat="server" StepType="Complete">
                                                <ucEktron:Finish ID="ucFinish" runat="server" />
                                            </asp:WizardStep>
                                        </WizardSteps>
                                        <SideBarTemplate>
                                            <asp:DataList runat="server" ID="SideBarList" OnItemDataBound="SideBarList_ItemDataBound">
			                                    <ItemTemplate>
				                                    <asp:LinkButton ID="SideBarButton" runat="server" />
			                                    </ItemTemplate>
		                                    </asp:DataList>
                                        </SideBarTemplate>
                                    </asp:Wizard>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </form>
        </div>
    </body>
</html>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Items.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Scope.Items" %>
<%@ Register TagPrefix="ucEktron" TagName="ItemsData" Src="ItemsData.ascx" %>
<div class="items ektronPageInfo">
    <asp:MultiView ID="mvScope" runat="server">
        <asp:View ID="vwEntireCart" runat="server">
            <table class="ektronGrid">
                <thead>
                    <tr class="title-header">
                        <th><asp:Literal ID="litEntireCartHeader" runat="server" /></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="center"><asp:Literal ID="litEntireCartDescription" runat="server" /></td>
                    </tr>
                </tbody>
            </table>
        </asp:View>
        <asp:View ID="vwApprovedItems" runat="server">
        <asp:ScriptManagerProxy ID="smpItems" runat="server" />
            <asp:UpdatePanel ID="upAllApprovedItems" runat="server" EnableViewState="true" UpdateMode="Conditional" ChildrenAsTriggers="true" RenderMode="Block">
                <ContentTemplate>
                    <table class="ektronGrid">
                        <thead>
                            <tr class="title-header">
                                <th>
                                    <asp:Literal ID="litItemsHeader" runat="server" />
                                    <span>: </span>
                                    <asp:Literal ID="litDescription" runat="server" />
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <div class="ektronPageContainer ektronPageTabbed">
                                        <div class="tabContainerWrapper">
                                            <div class="tabContainer ui-tabs ui-widget ui-widget-content ui-corner-all">
                                                <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all" style="border-right: medium none; width: 484px; visibility: visible;">
                                                    <li id="liInclude" runat="server">
                                                        <asp:LinkButton ID="lbInclude" runat="server" CssClass="selected" OnCommand="ItemsData_Click" CommandName="Include"></asp:LinkButton>
                                                    </li>
                                                    <li id="liExclude" runat="server" class="ui-state-default ui-corner-top">
                                                        <asp:LinkButton ID="lbExclude" runat="server" OnCommand="ItemsData_Click" CommandName="Exclude"></asp:LinkButton>
                                                    </li>
                                                </ul>
                                                <asp:MultiView ID="mvIncludeExclude" runat="server">
                                                    <asp:View ID="vwInclude" runat="server">
                                                        <ucEktron:ItemsData ID="ucInclude" runat="server" Scope="Include" />
                                                    </asp:View>
                                                    <asp:View ID="vwExclude" runat="server">
                                                        <ucEktron:ItemsData ID="ucExclude" runat="server" Scope="Exclude" />
                                                    </asp:View>
                                                </asp:MultiView>
                                            </div>
                                        </div>
                                    </div>
                                    <p style="padding:0em 0em 1em 1em;margin:0;"><asp:Literal ID="litDescriptionConflictMessage" runat="server" /></p>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:View>
    </asp:MultiView>
</div>
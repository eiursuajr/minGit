<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewcollection.ascx.cs"
    Inherits="Workarea_controls_collection_viewcollection" %>
<form name="netscapefix" method="post" action="#">
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="View Collection">
        <asp:Literal ID="litViewCollection" runat="server"></asp:Literal>
    </div>
    <div class="titlebar-error" id="divError" runat="server"></div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litEnableMult" runat="server"></asp:Literal>
                <td>
                    <asp:Literal ID="litViewLang" runat="server"></asp:Literal>
                </td>
                <asp:Literal ID="litCollItems" runat="server"></asp:Literal>
            </tr>
        </table>
    </div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <div class="heightFix">
        <!-------------------- Links for this Item ------------------------------------>
        <asp:Repeater ID="rptColl" runat="server">
            <HeaderTemplate>
                <table class="ektronGrid">
                    <tr class="title-header">
                        <td title="Title">
                            <%=MsgHelper.GetMessage("generic Title")%>
                        </td>
                        <td title="Language ID">
                            <%=MsgHelper.GetMessage("lbl Language ID") %>
                        </td>
                        <td title="ID">
                            <%=MsgHelper.GetMessage("generic ID")%>
                        </td>
                        <td title="URL Link">
                            <%=MsgHelper.GetMessage("generic URL Link")%>
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "HTML") %>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "ContentLanguage")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "ContentID")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "ContentLinks")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>

<script type="text/javascript">
		    <!--    //--><![CDATA[//><!--
    do_onload();
    //--><!]]>
</script>

<asp:Literal ID="litRefreshCollAccordion" runat="server" />
</form>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewallmenus.ascx.cs"
    Inherits="Workarea_controls_menu_viewallmenus" %>

<script type="text/javascript">
function LoadLanguageMenus(FormName){
    var num=document.forms[FormName].selLang.selectedIndex;
    document.forms[FormName].action = "collections.aspx?folderid=<%=folderId%>&action=ViewAllMenus&LangType=" + document.forms[FormName].selLang.options[num].value;
    document.forms[FormName].submit();
    return false;
}
</script>

<form name="frmViewMenus" action="collections.aspx" method="post">
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litTitle" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litViewLang" runat="server"></asp:Literal>
                <asp:Literal ID="litHelp" runat="server"></asp:Literal>
            </tr>
        </table>
    </div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <table class="ektronGrid" width="100%">
        <tr class="title-header">
            <td width="30%">
                <a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=navname&action=ViewAllMenus">
                    <asp:Literal ID="litGenericTitle" runat="server"></asp:Literal></a>
            </td>
            <td width="5%">
                <a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=collectionid&action=ViewAllMenus">
                    <asp:Literal ID="litGenericId" runat="server"></asp:Literal></a>
            </td>
            <td wrap="nowrap" title="Language ID">
                <asp:Literal ID="litLangId" runat="server"></asp:Literal>
            </td>
            <td>
                <a href="collections.aspx?folderid=<%=(folderId)%>&OrderBy=date&action=ViewAllMenus">
                    <asp:Literal ID="litDateMod" runat="server"></asp:Literal></a>
            </td>
        </tr>
        <asp:Repeater ID="rptMenus" runat="server">
            <ItemTemplate>
                <tr>
                    <% if(m_refApi.TreeModel == 1) {%>
                    <td>
                        <a href="menu.aspx?Action=viewcontent&treeviewid=-3&LangType=<%# DataBinder.Eval(Container.DataItem, "ContentLanguage")%>&folderid=<%# DataBinder.Eval(Container.DataItem, "folderId")%>&menuid=<%# DataBinder.Eval(Container.DataItem, "MenuID")%>">
                            <%# DataBinder.Eval(Container.DataItem, "MenuTitle")%></a>
                    </td>
                    <%} else {%>
                    <td>
                        <a href="collections.aspx?LangType=<%# DataBinder.Eval(Container.DataItem, "ContentLanguage")%>&folderid=<%# DataBinder.Eval(Container.DataItem, "folderId")%>&Action=ViewMenu&nid=<%# DataBinder.Eval(Container.DataItem, "MenuID")%>">
                            <%# DataBinder.Eval(Container.DataItem, "MenuTitle")%></a>
                    </td>
                    <%}%>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "MenuID")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "ContentLanguage")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "DisplayLastEditDate")%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>
</form>

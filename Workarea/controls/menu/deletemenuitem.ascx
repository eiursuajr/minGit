<%@ Control Language="C#" AutoEventWireup="true" CodeFile="deletemenuitem.ascx.cs"
    Inherits="Workarea_controls_menu_deletemenuitem" %>
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litTitle" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litHelp" runat="server"></asp:Literal>
            </tr>
        </table>
    </div>
</div>
<form name="selections" id="form1" runat="server" method="post">
<a title="Select All" href="#" onclick="SelectAll();return false;" id="aSelect" runat="server">
</a>&nbsp;&nbsp;<a id="aClear" runat="server" title="Clear All" href="#" onclick="ClearAll();return false;"></a>
<div class="ektronPageContainer ektronPageGrid">
    <table width="100%">
        <tr class="title-header">
            <td width="25%" title="Title">
                <asp:Literal ID="litGenericTitle" runat="server"></asp:Literal>
            </td>
            <td width="5%" title="ID">
                <asp:Literal ID="litID" runat="server"></asp:Literal>
            </td>
            <td title="URL Link">
                <asp:Literal ID="litURL" runat="server"></asp:Literal>
            </td>
        </tr>
        <asp:Repeater ID="rptItems" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <input type="checkbox" name="frm_check<%# DataBinder.Eval(Container.DataItem, "lLoop")%>"
                            onclick="document.forms.selections['frm_hidden<%# DataBinder.Eval(Container.DataItem, "lLoop")%>'].value=(this.checked ? '<%# DataBinder.Eval(Container.DataItem, "ID")%>.<%# DataBinder.Eval(Container.DataItem, "ItemType")%>.<%# DataBinder.Eval(Container.DataItem, "ItemID")%>' : 0);" />
                        <input size="<%# DataBinder.Eval(Container.DataItem, "ID")%>" type="hidden" name="frm_hidden<%# DataBinder.Eval(Container.DataItem, "lLoop")%>"
                            value="0" />
                        <input type="hidden" name="frm_languages<%# DataBinder.Eval(Container.DataItem, "lLoop")%>"
                            value="<%# DataBinder.Eval(Container.DataItem, "ContentLanguage")%>" />
                        <%# DataBinder.Eval(Container.DataItem, "ItemTitle")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "ItemID")%>
                    </td>
                    <td>
                        <%# DataBinder.Eval(Container.DataItem, "ItemLink")%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>

<script type="text/javascript" language="javascript">
		    <!--    //--><![CDATA[//><!--
    Collections = "<%=(cLinkArray)%>";
    Folders = "<%=(fLinkArray)%>";
    //--><!]]>
</script>

<input type="hidden" name="frm_content_ids" value="" />
<input type="hidden" name="frm_content_languages" value="" />
<input type="hidden" name="frm_folder_ids" value="" />
<input type="hidden" name="CollectionID" id="CollectionID" runat="server" />
</form>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="deletecollection.ascx.cs"
    Inherits="Workarea_controls_collection_deletecollection" %>
<div id="divError" runat="server" visible="false" class="ektronPageHeader">
    <div class="ektronTitlebar" title="Remove Items from Collection" id="DeleteCollectionError"
        runat="server">
    </div>
    <div class="titlebar-error" id="divTitle" runat="server">
    </div>
</div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="DeleteCollection" runat="server">
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
<form name="selections" method="post" action="collections.aspx?Action=doDeleteLinks&folderid=<%=folderId%>&nid=<%=nId %>&status=<%=Request.QueryString["status"] %>"
<div class="ektronPageContainer ektronPageGrid">
    <div class="heightFix">
        <table class="ektronGrid">
            <tr class="title-header">
                <td width="30%">
                    <input type="checkbox" title="Title" id="chkSelectAllRemove" onclick="selectClearAll(this);" />
                    <asp:Literal ID="litGenericTitle" runat="server"></asp:Literal>
                </td>
                <td width="5%" title="ID">
                    <asp:Literal ID="litGenericID" runat="server"></asp:Literal>
                </td>
                <td title="URL Link">
                    <asp:Literal ID="litGenericURL" runat="server"></asp:Literal>
                </td>
            </tr>
            <asp:Repeater ID="rptItems" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <input type="checkbox" name="frm_check<%# DataBinder.Eval(Container.DataItem, "lLoop")%>" onclick="document.forms.selections['frm_hidden<%# DataBinder.Eval(Container.DataItem, "lLoop")%>'].value=(this.checked ? <%# DataBinder.Eval(Container.DataItem, "ContentID")%> : 0);$ektron('#chkSelectAllRemove')[0].checked=false;" />
                            <input type="hidden" name="frm_languages<%# DataBinder.Eval(Container.DataItem, "lLoop")%>" value="<%# DataBinder.Eval(Container.DataItem, "ContentLanguage")%>" />
                            <input type="hidden" name="frm_hidden<%# DataBinder.Eval(Container.DataItem, "lLoop")%>" value="0" />
                            <%# DataBinder.Eval(Container.DataItem, "ContentIcon")%>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "ContentID")%>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "ContentLinks")%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <br />
    </div>
</div>
<script type="text/javascript" language="javascript">
		        <!--    //--><![CDATA[//><!--
    Collections = "<%=(cLinkArray)%>";
    Folders = "<%=(fLinkArray)%>";
    //--><!]]>
</script>

<input type="hidden" name="frm_content_ids" value="" />
<input type="hidden" name="frm_folder_ids" value="" />
<input type="hidden" name="frm_content_languages" value="" />
<input type="hidden" id="CollectionID" runat="server" name="CollectionID" />
</form>

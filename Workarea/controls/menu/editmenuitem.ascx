<%@ Control Language="C#" AutoEventWireup="true" CodeFile="editmenuitem.ascx.cs"
    Inherits="Workarea_controls_menu_editmenuitem" %>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="Edit Menu Item">
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
<form name="AddMenuItem" action="collections.aspx?action=doUpdateMenuItem&id=<%=gtLinks["ID"] %>&type=<%=gtLinks["ItemType"] %>&LangType=<%=gtLinks["ContentLanguage"] %>&iframe=<%=Request.QueryString["iframe"] %>" method="post">
<input type="hidden" name="CollectionID" value="<%=(MenuId)%>" id="Hidden12" />
<input type="hidden" name="FolderID" value="<%=(FolderId)%>" id="Hidden13" />
<input type="hidden" name="DefaultTitle" value="<%=gtLinks["DefaultTitle"].ToString()%>" />
<input type="hidden" name="frm_back" value="<%=(Request.QueryString["back"])%>" />
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label" title="Title">
                <asp:Literal ID="litGenericTitle" runat="server"></asp:Literal>
            </td>
            <td class="value">
                <input type="text" title="Enter Title here" id="Title" runat="server" name="Title" />
            </td>
        </tr>
        <tr>
            <td class="label" title="Image Link">
                 <asp:Literal ID="litImageLink" runat="server"></asp:Literal>:
            </td>
            <td class="value">
                <%=sitePath%>
                <input type="Text" title="Enter Image Link here" value="<%=gtLinks["ItemImage"].ToString()%>"
                    name="frm_menu_image" size="<%=(55 - sitePath.Length)%>" maxlength="75" onkeypress="return CheckKeyValue(event,'34');" />
                <a href="#" onclick="PopBrowseWin('images', '', 'document.forms.AddMenuItem.frm_menu_image');return false;">
                    <img alt="Select Image" title="Select Image" src="<%=(AppPath)%>images/UI/Icons/imageLink.png" /></a>
                <div class="ektronCaption">
                    <input name="frm_menu_image_override" id="Checkbox3" <% if (gtLinks["ImageOverride"].ToString().ToLower() == "true") {%>checked<%}%> type="checkbox" title="Use Image Instead of a Title" /><%=MsgHelper.GetMessage("alt Use image instead of a title")%></div>
            </td>
        </tr>
        <% if (!(("1" == gtLinks["ItemType"]) || ("2" == gtLinks["ItemType"]) || ("4" == gtLinks["ItemType"]))) {%>
        <tr>
            <td class="label" title="URL Link">
                <%=MsgHelper.GetMessage("generic URL Link")%>:
            </td>
            <td runat="server" id="tdItemLink" class="value">
                <input type="text" title="URL Link" name="Link" size="50" value="<%=gtLinks["ItemLink"]%>" />
            </td>
            <td runat="server" visible="false" id="tdURLLink" class="value">
                <input type="text" title="URL Link" name="Link" size="50" runat="server" id="urlLink" />
            </td>
        </tr>
        <% } %>
        <tr>
            <td class="label" title="Description">
                <%=MsgHelper.GetMessage("description label")%>
            </td>
            <td class="value">
                <textarea name="Description" title="Enter Description here" id="Description"><%=gtLinks["ItemDescription"]%></textarea>
            </td>
        </tr>
        <% if ("4" != gtLinks["ItemType"]) {%>
        <tr>
            <td class="label" title="Target">
                <%=MsgHelper.GetMessage("generic link target label")%>
            </td>
            <td class="value">
                <!-- 1 = _blank; 2 = _self; 3 = _parent; 4 = _top -->
                <input type="radio" name="Target" value="blank" <%if (gtLinks["ItemTarget"] == "popup") {%>checked<%}%> title="Popup" /><%=MsgHelper.GetMessage("Popup label")%>
                <input type="radio" name="Target" value="self" <%if (gtLinks["ItemTarget"] == "self") {%>checked<%}%> title="Self" /><%=MsgHelper.GetMessage("Self label")%>
                <input type="radio" name="Target" value="parent" <%if (gtLinks["ItemTarget"] == "parent") {%>checked<%}%> title="Parent" /><%=MsgHelper.GetMessage("Parent label")%>
                <input type="radio" name="Target" value="top" <%if (gtLinks["ItemTarget"] == "top") {%>checked<%}%> title="Top" /><%=MsgHelper.GetMessage("Top label")%>
            </td>
        </tr>
            <asp:Literal ID="litInfo" runat="server"></asp:Literal>
        <% } %>
    </table>
</div>
</form>

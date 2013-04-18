<%@ Control Language="C#" AutoEventWireup="true" CodeFile="editmenu.ascx.cs" Inherits="Workarea_controls_menu_editmenu" %>
<%@ Register Src="../../PageBuilder/foldertree.ascx" TagName="FolderTree" TagPrefix="CMS" %>
<form action="collections.aspx?Action=doEditMenu&nid=<%=(MenuId)%>&folderid=<%=(FolderId)%>&iframe=<%=Request.QueryString["iframe"]%>"
method="Post" name="menu">
<div id="dlgBrowse" class="ektronWindow ektronModalStandard">
    <div class="ektronModalHeader">
        <h3 title="Template Selection">
            <%=MsgHelper.GetMessage("lbl template selection")%>
            <a title="Close" href="#" onclick="return false;" class="ektronModalClose"></a>
        </h3>
    </div>
    <div class="ektronModalBody">
        <div class="folderTree">
            <CMS:FolderTree id="folderTree_editExistingMenu" runat="server" filter="[.]aspx$" />
        </div>
    </div>
</div>
<div id="FrameContainer" class="ektronBorder" style="position: absolute; top: 48px;
    left: 40px; width: 1px; height: 1px; display: none;">
    <iframe id="ChildPage" src="javascript:false;" name="ChildPage" frameborder="1" marginheight="0"
        marginwidth="0" width="100%" height="100%" scrolling="auto"></iframe>
</div>
<div allowtransparency="true" id="FolderPickerAreaOverlay" style="position: absolute;
    top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 10; background-color: transparent;">
    <iframe allowtransparency="true" src="javascript:false;" id="FolderPickerAreaOverlayChildPage"
        name="FolderPickerAreaOverlayChildPage" frameborder="0" marginheight="0" marginwidth="0"
        width="100%" height="100%" scrolling="no" style="background-color: transparent;
        background: transparent;"></iframe>
</div>
<!-- TOP: 48px; LEFT: 55px; -->
<div id="FolderPickerPageContainer" style="position: absolute; top: 0px; left: 0px;
    width: 1px; height: 1px; display: none; z-index: 20; background-color: transparent;
    border-style: none">
    <iframe id="FolderPickerPage" src="blank.htm" name="FolderPickerPage" frameborder="0"
        marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto">
    </iframe>
</div>
<input type="hidden" name="frm_folder_id" value="<%=(FolderId)%>" />
<input type="hidden" name="frm_back" value="<%=(Request.QueryString["back"])%>" />
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="ektron">
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
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label" title="Title">
                <%=MsgHelper.GetMessage("generic title label")%>
            </td>
            <td nowrap="nowrap">
                <input type="Text" title="Enter Title here" value="<%=(gtLinks["MenuTitle"])%>" name="frm_menu_title"
                    maxlength="255" onkeypress="return CheckKeyValue(event,'34');" />&nbsp;[<%=LanguageName%>]
            </td>
        </tr>
        <tr>
            <td class="label" title="Image Link">
                <%=MsgHelper.GetMessage("lbl Image Link")%>:
            </td>
            <td nowrap="nowrap">
                <%=sitePath%><input type="Text" title="Enter Image Link here" value="<%=(gtLinks["MenuImage"])%>"
                    name="frm_menu_image" size="<%=(55 - sitePath.Length)%>" maxlength="75" onkeypress="return CheckKeyValue(event,'34');" />
                <a href="#" onclick="PopBrowseWin('images', '', 'document.forms.menu.frm_menu_image');return false;">
                    <img src="<%=(AppPath)%>images/UI/Icons/imageLink.png" alt="Image Link" title="Image Link" /></a>
                <br />
                <input title="Use Image Instead of a Title Option" name="frm_menu_image_override" id="Checkbox2" <% if (gtLinks["ImageOverride"].ToString().ToLower() == "true") {%>checked<%}%> type="checkbox" /><%=MsgHelper.GetMessage("alt Use image instead of a title")%>
            </td>
        </tr>
        <tr>
            <td class="label" title="URL Link">
                <%=MsgHelper.GetMessage("generic URL Link")%>:
            </td>
            <td nowrap="nowrap">
                <span class="menuLinkSitePathPrefix" ><%=sitePath%></span>
                <input type="Text" title="Enter URL Link here" class="subMenuLinkText" value="<%=(gtLinks["MenuLink"])%>"
                    name="frm_menu_link" id="frm_menu_link" size="<%=(56 - sitePath.Length)%>" maxlength="255"
                    onkeyup="CheckMenuLinkProtocol(this);" onkeypress="return CheckKeyValue(event,'34');" />&nbsp; <a href="#" onclick="LoadSelectContentPage(<%=ContentLanguage %>);return true;">
                        <img alt="Select Page" title="Select Page" src="<%=(AppPath)%>images/UI/Icons/contentLink.png" /></a>
                <br />
                <%=MsgHelper.GetMessage("alt hyperlink this submenu to this link")%>
            </td>
        </tr>
        <tr>
            <td nowrap="nowrap" class="label" title="Template Link">
                <%=MsgHelper.GetMessage("lbl template link")%>:
            </td>
            <td nowrap="nowrap">
                <%=sitePath%><input type="Text" title="Enter Template Link here" value="<%=(gtLinks["MenuTemplate"])%>"
                    name="frm_menu_template" id="frm_menu_template" size="<%=(56 - sitePath.Length)%>"
                    maxlength="255" onkeypress="return CheckKeyValue(event,'34');" />
                <br />
                <%=MsgHelper.GetMessage("alt (Menu Template Link that contents under the current menu level may use.)")%>
            </td>
        </tr>
        <tr>
            <td class="label" title="Description">
                <%=MsgHelper.GetMessage("description label")%>
            </td>
            <td>
                <textarea title="Enter Description here" name="frm_menu_description" maxlength="255"
                    onkeypress="return CheckKeyValue(event,'34');"><%=gtLinks["MenuDescription"]%></textarea>
            </td>
        </tr>
    </table>
    <div class="ektronTopSpace">
    </div>
    <fieldset>
        <legend title="Folder Associations">
            <%=MsgHelper.GetMessage("lbl folder associations")%></legend>
        <div>
            <a title="Change" href="#" onclick="LoadFolderPicker();return (false);">
                <%=MsgHelper.GetMessage("btn change")%></a>
        </div>
        <br />
        <table width="100%" border="1" style="border-color: #d8e6ff;" id="EnhancedMetadataMultiContainer1">
        </table>
    </fieldset>
    <div class="ektronTopSpace">
    </div>
    <fieldset>
        <legend title="Template Associations">
            <%=MsgHelper.GetMessage("lbl template associations")%></legend>
        <table>
            <tbody>
                <tr>
                    <td width="50%">
                        <select id="template_list" style="width: 100%;" onchange="ta_editSelectList();" multiple
                            size="5" name="template_list">
                        </select>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td style="margin-left: 4px; margin-right: 4px;">
                        <a href="javascript:ta_moveItemUp()">
                            <img src="images/ui/icons/arrowHeadUp.png" title="Click to move item up" alt="Click to move item up" />
                        </a>
                        <br />
                        <a href="javascript:ta_moveItemDown()">
                            <img src="images/ui/icons/arrowHeadDown.png" title="Click to move item down" alt="Click to move item down" />
                        </a>
                        <br />
                        <br />
                        <input type="button" value="..." class="ektronModal browseButton" />
                    </td>
                    <td>
                        &nbsp;&nbsp;
                    </td>
                    <td width="50%">
                        <table class="ektronForm">
                            <tr>
                                <td class="label" title="Text">
                                    <%=MsgHelper.GetMessage("lbl Text")%>
                                </td>
                                <td class="value">
                                    <input id="template_text" name="template_text" size="40" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <input title="Add" id="Button1" onclick="ta_addItemToSelectList();" type="button"
                                        value="<%=(MsgHelper.GetMessage("generic add title"))%>" name="ta_btnAdd" />
                                    &nbsp;&nbsp;
                                    <input title="Change" id="Button2" onclick="ta_updateItemToSelectList();" type="button"
                                        value="<%=(MsgHelper.GetMessage("btn change"))%>" name="ta_btnChange" />
                                    &nbsp;&nbsp;
                                    <input title="Remove" id="Button3" onclick="ta_removeItemsFromSelectList();" type="button"
                                        value="<%=(MsgHelper.GetMessage("btn remove"))%>" name="ta_btnRemove" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </fieldset>
</div>
<input type="hidden" id="frm_set_to_template" name="frm_set_to_template" value="<%=(gtLinks["MenuTemplate"])%>" />
<input type="hidden" id="associated_folder_id_list" name="associated_folder_id_list"
    value="<%=(AssociatedFolderIdListString)%>" />
<input type="hidden" id="associated_folder_title_list" name="associated_folder_title_list"
    value="<%=(AssociatedFolderTitleListString)%>" />
<input type="hidden" id="associated_templates" name="associated_templates" value="<%=(AssociatedTemplatesString)%>" />

<script type="text/javascript" language="javascript">
    do_onload();
</script>

</form>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addmenu.ascx.cs" Inherits="Workarea_controls_menu_addmenu" %>
<%@ Register Src="../../PageBuilder/foldertree.ascx" TagName="FolderTree" TagPrefix="CMS" %>
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
<div id="FolderPickerPageContainer" style="position: absolute; top: 0px; left: 0px;
    width: 1px; height: 1px; display: none; z-index: 20; background-color: transparent;
    border-style: none">
    <iframe id="FolderPickerPage" src="blank.htm" name="FolderPickerPage" frameborder="0"
        marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto">
    </iframe>
</div>
<% 
    switch (action)
    {
        case "AddMenu":
%>
<form method="Post" name="menu" action="collections.aspx?Action=doAddMenu&nId=<%=nId %>&LangType<%=ContentLanguage %>&bPage=<%=Request.QueryString["bPage"] %>">
<%
    break;
            case "AddSubMenu":
%>
<form method="Post" onload="do_onload" name="menu" action="collections.aspx?Action=doAddSubMenu&LangType=<%=ContentLanguage %>&nId=<%=nId %>&iframe=<%=Request.QueryString["iframe"] %>&bPage=<%=Request.QueryString["bPage"] %><%=noWorkAreaString %>">
<%
    break;
            case "AddTransMenu":
%>
<form method="Post" onload="do_onload" name="menu" action="collections.aspx?Action=doAddTransMenu&LangType=<%=ContentLanguage %>&nId=<%=nId %>&bPage=<%=Request.QueryString["bPage"] %>">
<%
    break;
        }
%>
<div id="dlgBrowse" class="ektronWindow ektronModalStandard">
    <div class="ektronModalHeader">
        <h3 title="Template Selection">
            <%=MsgHelper.GetMessage("lbl template selection")%>
            <a title="Close" href="#" onclick="return false;" class="ektronModalClose"></a>
        </h3>
    </div>
    <div class="ektronModalBody">
        <div class="folderTree">
            <CMS:FolderTree ID="folderTree_editNewMenu" runat="server" filter="[.]aspx$" />
        </div>
    </div>
</div>
<input type="hidden" name="frm_folder_id" value="<%=folderId%>" />
<input type="hidden" name="frm_back" value="<%=Request.QueryString["back"]%>" />
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="Add Menu">
        <%=MsgHelper.GetMessage("Add Menu Title")%></div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
				<%=StyleHelper.ActionBarDivider%>
                <td>
                    <%=m_refStyle.GetHelpButton("AddMenu", "")%>
                </td>
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
            <td class="value">
                <input type="Text" title="Enter Title here" name="frm_menu_title" maxlength="255"
                    onkeypress="return CheckKeyValue(event,'34');" id="frm_menu_title" />
                [<%=LanguageName%>]
            </td>
        </tr>
        <tr>
            <td class="label" title="Image Link">
                <%=MsgHelper.GetMessage("lbl Image Link")%>:
            </td>
            <td class="value">
                <%=sitePath%>
                <input type="Text" name="frm_menu_image" size="<%=(55 - sitePath.Length)%>" maxlength="75"
                    onkeypress="return CheckKeyValue(event,'34');" />
                <a href="#" onclick="PopBrowseWin('images', '', 'document.forms.menu.frm_menu_image');return false;">
                    <img alt="Select Image" title="Select Image" src="<%=(AppPath)%>images/UI/Icons/imageLink.png" /></a>
                <div class="ektronCaption">
                    <input name="frm_menu_image_override" type="checkbox" title="Use Image Instead of a Title" /><%=MsgHelper.GetMessage("alt Use image instead of a title")%>
                </div>
            </td>
        </tr>
        <tr>
            <td class="label" title="URL Link">
                <%=MsgHelper.GetMessage("generic URL Link")%>:
            </td>
            <td class="value">
                <span class="menuLinkSitePathPrefix" ><%=sitePath%></span>
                <input type="Text" title="URL Link" name="frm_menu_link" id="frm_menu_link" class="subMenuLinkText" size="<%=(55 - sitePath.Length)%>"
                    maxlength="255" onkeyup="CheckMenuLinkProtocol(this);" onkeypress="return CheckKeyValue(event,'34');" />
                <a href="#" onclick="LoadSelectContentPage(<%=ContentLanguage %>);return true;">
                    <img alt="Select Page" title="Select Page" src="<%=(AppPath)%>images/UI/Icons/contentLink.png" /></a>
                <div class="ektronCaption" title="Hyperlink this menu item to this link">
                    <%=MsgHelper.GetMessage("alt hyperlink this submenu to this link")%></div>
            </td>
        </tr>
        <tr>
            <td class="label" title="Template Link">
                <%=MsgHelper.GetMessage("lbl template link")%>:
            </td>
            <td class="value">
                <%=sitePath%>
                <input title="Enter Template Link here" type="Text" name="frm_menu_template" size="<%=(55 - sitePath.Length)%>"
                    maxlength="255" onkeypress="return CheckKeyValue(event,'34');" />
                <div class="ektronCaption" title="Menu Template Link that contents under the current menu level may use">
                    <%=MsgHelper.GetMessage("alt (Menu Template Link that contents under the current menu level may use.)")%></div>
            </td>
        </tr>
        <tr>
            <td class="label" title="Description">
                <%=MsgHelper.GetMessage("description label")%>
            </td>
            <td class="value">
                <textarea title="Enter Description here" name="frm_menu_description" maxlength="255"
                    onkeypress="return CheckKeyValue(event,'34');" id="frm_menu_description"></textarea>
            </td>
        </tr>
        <input type="hidden" name="EnableReplication" value="<%=Request.QueryString["QD"]%>" />
    </table>
    <div class="ektronTopSpace">
    </div>
    <fieldset>
        <legend title="Folder Associations">
            <%=MsgHelper.GetMessage("lbl folder associations")%>:</legend>
        <div>
            <a title="Change" href="#" onclick="LoadFolderPicker();return (false);">
                <%=MsgHelper.GetMessage("btn change")%></a>
        </div>
        <br />
        <table width="100%" id="EnhancedMetadataMultiContainer1">
        </table>
    </fieldset>
    <div title="Template Associations" class="ektronHeader">
        <%=MsgHelper.GetMessage("lbl template associations")%>:</div>
    <table width="100%">
        <tr>
            <td style="width: 50%">
                <select id="template_list" style="width: 100%;" onchange="ta_editSelectList();" multiple="multiple"
                    size="5" name="template_list">
                </select>
            </td>
            <td>
                &nbsp;
            </td>
            <td style="margin-left: 4px; margin-right: 4px;">
                <a href="javascript:ta_moveItemUp()">
                    <img src="images/UI/Icons/arrowHeadUp.png" title="Click to move item up" alt="Click to move item up" />
                </a>
                <br />
                <a href="javascript:ta_moveItemDown()">
                    <img src="images/UI/Icons/arrowHeadDown.png" title="Click to move item down" alt="Click to move item down" />
                </a>
                <br />
                <br />
                <input title="Browse" type="button" value="..." class="ektronModal browseButton" />
            </td>
            <td>
                &nbsp;&nbsp;
            </td>
            <td style="width: 50%">
                <table class="ektronForm">
                    <tr>
                        <td class="label" title="Text">
                            <%=MsgHelper.GetMessage("lbl Text")%>
                        </td>
                        <td class="value">
                            <input title="Enter Text here" id="template_text" type="text" name="template_text" />
                        </td>
                    </tr>
                </table>
                <div class="ektronTopSpace">
                </div>
                <div style="padding-left: 50px">
                    <input title="Add" id="ta_btnAdd" name="ta_btnAdd" onclick="ta_addItemToSelectList();"
                        type="button" value="<%=(MsgHelper.GetMessage("generic add title"))%>" />
                    &nbsp;&nbsp;
                    <input title="Change" id="ta_btnChange" name="ta_btnChange" onclick="ta_updateItemToSelectList();"
                        type="button" value="<%=(MsgHelper.GetMessage("btn change"))%>" />
                    &nbsp;&nbsp;
                    <input title="Remove" id="ta_btnRemove" name="ta_btnRemove" onclick="ta_removeItemsFromSelectList();"
                        type="button" value="<%=(MsgHelper.GetMessage("btn remove"))%>" />
                </div>
            </td>
        </tr>
    </table>
    <input type="hidden" name="frm_menu_parentid" value="<%= Request.QueryString["parentid"] %>" />
    <input type="hidden" name="frm_menu_ancestorid" value="<%= Request.QueryString["ancestorid"] %>" />
    <input type="hidden" id="associated_folder_id_list" name="associated_folder_id_list"
        value="" />
    <input type="hidden" id="associated_folder_title_list" name="associated_folder_title_list"
        value="" />
    <input type="hidden" id="associated_templates" name="associated_templates" value="" />

    <script type="text/javascript" language="javascript">
		       <!--        //--><![CDATA[//><!--
        do_onload();
        //--><!]]>
    </script>
</div>
</form>
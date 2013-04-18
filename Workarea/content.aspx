<%@ Page Language="C#" AutoEventWireup="true" Inherits="content" ValidateRequest="false"
    CodeFile="content.aspx.cs" %>

<%@ Reference Control="controls/folder/viewfolder.ascx" %>
<%@ Reference Control="controls/folder/addfolder.ascx" %>
<%@ Reference Control="controls/folder/movefolderitem.ascx" %>
<%@ Reference Control="controls/folder/removefolderitem.ascx" %>
<%@ Reference Control="controls/folder/viewfolderattributes.ascx" %>
<%@ Reference Control="controls/folder/editfolderattributes.ascx" %>
<%@ Reference Control="controls/content/viewcontent.ascx" %>
<%@ Reference Control="controls/content/editcontentattributes.ascx" %>
<%@ Reference Control="controls/content/movecontent.ascx" %>
<%@ Reference Control="controls/content/localization_uc.ascx" %>
<%@ Reference Control="controls/permission/viewpermissions.ascx" %>
<%@ Reference Control="controls/permission/deletepermissions.ascx" %>
<%@ Reference Control="controls/permission/selectpermissions.ascx" %>
<%@ Reference Control="controls/permission/editpermissions.ascx" %>
<%@ Reference Control="controls/approval/viewapprovals.ascx" %>
<%@ Reference Control="controls/approval/editapprovalmethod.ascx" %>
<%@ Reference Control="controls/approval/addapproval.ascx" %>
<%@ Reference Control="controls/approval/editapprovalorder.ascx" %>
<%@ Reference Control="controls/approval/editpreapproval.ascx" %>
<%@ Reference Control="controls/approval/deleteapproval.ascx" %>
<%@ Reference Control="sync/SyncResources.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <title>Ektron Workarea Content</title>
    <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
                .moveButtons {float:left; display:block; width:16px; margin-left:.5em;}
                .approvalList {float: left;}
                div#td_eao_ordertitle {color:#1D5987;}
                img.imgUsers {vertical-align:middle; margin-right: .25em;}
                div.ektronCaption {clear: both; padding-top: .25em}
                div#EkTB_window {background:#eeeeee;}
                div#uploadingWrapper {background:#eeeeee; text-align:center; padding:1em 0em;}
                .permissionsUserSelectorLabel {font-weight:bold;color:black;}
                div#uploadingWrapper h1 {font-size: 1em;font-weight: bold;margin: .25em 0em;color: #555555;}
                .membershipGroup {display: block; padding-left:20px;background-image:url('images/ui/icons/usersMembership.png');background-repeat:no-repeat;background-position:left 2px;}
                .membershipUser {display: block; padding-left:20px;background-image:url('images/ui/icons/userMembership.png');background-repeat:no-repeat;background-position:left 2px;}
                .cmsGroup {display: block;padding-left:20px;background-image:url('images/ui/icons/users.png');background-repeat:no-repeat;background-position:left 2px;}
                .cmsUser {display: block;padding-left:20px;background-image:url('images/ui/icons/user.png');background-repeat:no-repeat;background-position:left 2px;}
                .narrowColumn {width:25%;}
                .minHeight { margin: .2em !important; }
                .blogCommentStatusPending
                {
                    display: block;
                    text-align: center;
                    margin: .25em;
                    padding: 0 20px;
                    background-position: right center;
                    background-repeat: no-repeat;
                    background-image: url('images/ui/icons/exclamation.png');
                }
                p.blogCommentStatusPending {background-position: left center;}

                div#dvStandard {overflow: visible !important;}
                #viewfolder_FolderDataGrid tbody tr.selected {border: 1px solid #dbb143; background: #f7eda1 url(csslib/ektronTheme/images/ui-bg_highlight-hard_75_f7eda1_1x100.png) 50% top repeat-x; color: #444444; }
                div#selectMultiLingual
                {
                    width: 390px;
                    height: 110px;
                    margin: -64px 0px 0px -275px;
                    background-color: #fff;
                    border: none;
                    padding: 0;
                    top: 50%;
                }
                div#moveContentModal
                {
                    width: 390px;
                    height: 110px;
                    margin: -64px 0px 0px -275px;
                    background-color: #fff;
                    border: none;
                    padding: 0;
                    top: 50%;
                }
                a.buttonSelected {background-image: url('images/UI/Icons/task.png'); background-position: .6em center;}
                div#progressbar
                {
                    width: 128px;
                    height: 128px;
                    background-color: #fff;
                    background-image: url("images/ui/loading_big.gif");
                    text-indent: -10000px;
                    border: none;
                    padding: 0;
                    top: 50%;
                    margin: -64px 0 0 -64px;
                }
               
            /*]]>*/-->
        </style>
    <!--[if lte IE 7]>
            <style type="text/css">
                .permissionsUserSelectorLabel {position:relative;top:-.75em;}
            </style>
        <![endif]-->
    <asp:placeholder id="sync_jsResourcesPlaceholder" runat="server"></asp:placeholder>
    <asp:Literal ID="ltr_commerceCSSJS" EnableViewState="false" runat="server" />
    <asp:Literal ID="ltrEktronReloadJs" EnableViewState="false" runat="server" />
    <asp:Literal ID="ltrStyleSheetJs" EnableViewState="false" runat="server" />
    <script type="text/javascript">
            <!--        //--><![CDATA[//><!--

        $ektron.addLoadEvent(function () {
            Ektron.Workarea.Sync.Relationships.Init();
        });

        Ektron.ready(function () {
            // MultiLanguage Modal Dialog.
            $ektron("#selectMultiLingual").modal(
                {
                    trigger: '',
                    modal: true,
                    toTop: true,
                    onShow: function (hash) {
                        hash.o.fadeIn();
                        hash.w.fadeIn();
                    },
                    onHide: function (hash) {
                        hash.w.fadeOut("fast");
                        hash.o.fadeOut("fast", function () {
                            if (hash.o) {
                                hash.o.remove();
                            }
                        });
                    }
                });

            // Move Content Confirmation Box Modal
            $ektron("#moveContentModal").modal(
                {
                    trigger: '',
                    modal: true,
                    toTop: true,
                    onShow: function (hash) {
                        hash.o.fadeIn();
                        hash.w.fadeIn();
                    },
                    onHide: function (hash) {
                        hash.w.fadeOut("fast");
                        hash.o.fadeOut("fast", function () {
                            if (hash.o) {
                                hash.o.remove();
                            }
                        });
                    }
                });
            // ProgressBar Modal Dialog.
            $ektron("#progressbar").modal(
                {
                    trigger: '',
                    modal: true,
                    toTop: true,
                    onShow: function (hash) {
                        hash.o.fadeIn();
                        hash.w.fadeIn();
                    },
                    onHide: function (hash) {
                        hash.w.fadeOut("fast");
                        hash.o.fadeOut("fast", function () {
                            if (hash.o) {
                                hash.o.remove();
                            }
                        });
                    }
                });
            if ($ektron("#preapproval_hdnInherited").length > 0) {
                if ($ektron("#preapproval_hdnInherited")[0].value == "false") {
                    $ektron("#selectusergroup").removeAttr("disabled");
                }
                else {
                    //$ektron("#trPreApprovalGroup")[0].style.display = "none";
                }
            }
            if ($ektron("#preapproval_hdnFolderId").length > 0) {
                if ($ektron("#preapproval_hdnFolderId")[0].value == "0") {
                    $ektron("#selectusergroup").removeAttr("disabled");
                    $ektron("#trPreApprovalGroup")[0].style.display = "block";
                }
            }
        });

        var folderjslanguage = "<asp:Literal EnableViewState='false' ID='folder_jslanguage' Runat='server'/>";
        if (undefined !== top.Ektron) {
            if (undefined !== top.Ektron.Workarea.FolderContext) {
                top.Ektron.Workarea.FolderContext.folderLanguage = folderjslanguage;
            }
        }

        var appPath = "<asp:Literal EnableViewState='false'  runat='server' id='jsAppPathMultiLang' />";
        var textPaste = "<asp:Literal EnableViewState='false'  runat='server' id='ltrPaste' />";
        var jsConfirmFolderDelete = "<asp:Literal EnableViewState='false' id='litConfirmFolderDelete' runat='Server'/>";

        var BreadcrumbNotitleMsg = "<asp:Literal EnableViewState='false'   runat='server' id='ltr_BreadcrumbNotitleMsg' />";
        var BreadcrumbAddBtnText = "<asp:Literal EnableViewState='false'  runat='server' id='BreadcrumbAddBtnText' />";
        var BreadcrumbSaveBtnText = "<asp:Literal EnableViewState='false'  runat='server' id='BreadcrumbSaveBtnText' />";
        var BreadcrumbRemoveTooltip = "<asp:Literal EnableViewState='false'  runat='server' id='BreadcrumbRemoveTooltip' />";

        var SiteAliasNoNameMsg = "<asp:Literal EnableViewState='false'  runat='server' id='SiteAliasNoNameMsg' />";
        var SiteAliasDelIconTooltip = "<asp:Literal EnableViewState='false'  runat='server' id='SiteAliasDelIconTooltip' />";
        var SiteAliasAddBtnText = "<asp:Literal EnableViewState='false'  runat='server' id='SiteAliasAddBtnText' />";

        // Copy/Move Action menu options related variables.
        var jsAlertCheckedOutSelected = "<asp:Literal EnableViewState='false'  runat='server' id='jsAlertCheckedOutSelected' />";
        var jsAlertCheckedOutSelectedCopy = "<asp:Literal EnableViewState='false'  runat='server' id='jsAlertCheckedOutSelectedCopy' />";
        var jsAlertSelectOneContent = "<asp:Literal EnableViewState='false'  runat='server' id='jsAlertSelectOneContent' />";
        var jsAlertSelectNotApprovedAll = "<asp:Literal EnableViewState='false'  runat='server' id='jsAlertSelectNotApprovedAll' />";
        var jsAlertSelectOneContentCopy = "<asp:Literal EnableViewState='false'  runat='server' id='jsAlertSelectOneContentCopy' />";
        $ektron.addLoadEvent(function () {
            var pageUrl = document.location.href;
            var i;
            if (pageUrl.indexOf("action=ViewPermissions") !== -1 || pageUrl.indexOf("action=viewpermissions") !== -1) {
                var userUrl = $ektron("#permission_PermissionsGenericGrid tr a");
                var assignedUrlIds = '';
                $ektron(userUrl).each(function (i) {
                    var thisId = this.href.split("PermID", this.href.indexOf("PermID"))[1].replace(/=/g, "");
                    thisId = thisId.split("&");
                    assignedUrlIds += thisId[0] + ',';
                });
                top.Ektron.Workarea.AddPermissionItems = assignedUrlIds;
            }
        });

        var jsEnableWorkareaNav = true;

        function CanNavigate() {
            return jsEnableWorkareaNav;
        }
        function CanShowNavTree() {
            return jsEnableWorkareaNav;
        }

        function showUploadingBox(bShow) {
            if (bShow)
                ektb_show("Uploading", "uploading.html?TB_iframe=true&height=30&width=100&modal=true", null);
            else
                ektb_remove();
        }
        function EktUploadCompleteCallback(retString, hasReqMeta, folderId, folderPath) {
            // @ this point there is just CD and we have waited the delay processing time (in seconds)to
            // make sure no other CU or CC states are coming so let's deal with what we have....
            if (hasReqMeta == true || hasReqMeta == "true") {
                var buffer = '';
                try {
                    buffer = new String(top.frames["ek_main"].location.href);
                    buffer = buffer.substring(0, buffer.lastIndexOf("/") + 1);
                }
                catch (ex) {
                    ;
                }

                var contArr = retString.split(",");
                var url = buffer + "DMSMetadata.aspx" + "?contentId=" + contArr[0] + "&folderId=" + folderId + "&idString=" + retString;

                if (top.frames["ek_main"] != null)
                    top.frames["ek_main"].location.href = url;
                return;
            }
            else {
                if (top.frames["ek_main"] != null) {
                    if (top.frames["ek_nav_bottom"] != null) {
                        try {
                            top.ReloadTrees("Forms,Content,Library");
                            top.TreeNavigation("ContentTree", folderPath);
                        }
                        catch (e) {
                            ; //alert(e.message);
                        }
                    }
                    top.frames["ek_main"].location.href = top.frames["ek_main"].location.href;
                }
            }
        }
        toggleSelectGroup = function (obj) {
            if (!confirm("<asp:Literal EnableViewState='false'  runat='server' id='jsConfirmChangeInheritance'/>")) {
                $ektron("#preapproval_chkInheritPreApprovalGroup")[0].checked = !obj;
                return false;
            }
            else {
                //$ektron("#trPreApprovalGroup")[0].style.display="block";
                $ektron("#selectusergroup").removeAttr("disabled");
            }
            var inheritParentConfiguration = $ektron("#preapproval_chkInheritPreApprovalGroup")[0].checked;
            if (inheritParentConfiguration) {
                $ektron("#selectusergroup")[0].disabled = "disabled";
            }
            else {
                $ektron("#selectusergroup").removeAttr("disabled");
            }
        }
        //--><!]]>
        var addFolderBreadCrumb_breakInheri = "<asp:Literal EnableViewState='false'  runat='server'  id='ltr_addFolderBreadCrumb_breakInheri' />";
    </script>
    <script type="text/javascript">
            <!--        //--><![CDATA[//><!--
        var UniqueID = "<asp:Literal EnableViewState='false'  id='UniqueLiteral'  runat='server'/>_";
        //--><!]]>
    </script>
    <asp:placeholder id="phShowTStatusMessage" runat="server" visible="False">
        <script type="text/javascript" id="EktronShowTStatusMessageJs">
                <!--            //--><![CDATA[//><!--
            alert("This content has been submitted, but waiting for completion of associated tasks. \n The publishing process will not proceed until the task has been completed.");
            //--><!]]>
        </script>
    </asp:placeholder>
    <asp:placeholder id="phShowAjaxTree" runat="server" visible="False">
        <script type="text/javascript" id="EktronShowAjaxTreeJs">
                 <!--            //--><![CDATA[//><!--
            if (typeof (top["ek_nav_bottom"]) != 'undefined') {
                if (typeof (top["ek_nav_bottom"]["NavIframeContainer"]) != 'undefined') {
                    if (typeof (top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]) != 'undefined') {
                        if (typeof (top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["ContentTree"]) != 'undefined') {
                            var treeobj = top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["ContentTree"];
                            if (treeobj.document.getElementById("selected_folder_id") != null) {
                                var SelectedTreeId = treeobj.document.getElementById("selected_folder_id").value;
                                var CurrentFolderId = "<asp:Literal EnableViewState='false'  ID='litShowAjaxTreeFolderId' runat='Server'/>";
                                if (CurrentFolderId == 0 && SelectedTreeId != 0) {
                                    var stylenode = treeobj.document.getElementById(SelectedTreeId);
                                    if (stylenode != null) {
                                        stylenode.style["background"] = "#ffffff";
                                        stylenode.style["color"] = "#000000";
                                        var stylenode = treeobj.document.getElementById(0/*CurrentFolderId*/);
                                        stylenode.style["background"] = "#3366CC";
                                        stylenode.style["color"] = "#ffffff";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            function reloadFolder(pid) {
                reloadTreeByName(pid, "ContentTree");
                reloadTreeByName(pid, "FormsTree");
                reloadTreeByName(pid, "LibraryTree");
            }
            function reloadTreeByName(pid, TreeName) {
                var obj = top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"][TreeName];
                if (obj != null) {
                    var node = obj.document.getElementById("T" + pid);
                    if (node != null) {
                        for (var i = 0; i < node.childNodes.length; i++) {
                            if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9) {
                                if (node.childNodes(i).nodeName == 'LI' || node.childNodes(i).nodeName == 'UL') {
                                    var parent = node.childNodes(i).parentElement;
                                    parent.removeChild(node.childNodes(i));
                                }
                            }
                            else {
                                if (node.childNodes[i].nodeName == 'LI' || node.childNodes[i].nodeName == 'UL') {
                                    var parent = node.childNodes[i].parentNode;
                                    parent.removeChild(node.childNodes[i]);
                                }
                            }
                        }
                        obj.TREES["T" + pid].children = [];
                        obj.TreeDisplayUtil.reloadParentTree(pid);
                        obj.onToggleClick(pid, obj.callback_function, pid);
                    }
                }
            }
            //--><!]]>
        </script>
    </asp:placeholder>
    <script type="text/javascript" id="EktronContentTemplateJs">
            <!--//--><![CDATA[//><!--
                var g_sHtmlAction;
                var standardObj = "";
                var advancedObj = "";
                var jsContentLanguage="<asp:Literal EnableViewState='false'  id='txtContentLanguage' runat='Server'/>";
                var jsDefaultContentLanguage="<asp:Literal EnableViewState='false'  id='txtDefaultContentLanguage' runat='Server'/>";
                var jsEnableMultilingual="<asp:Literal EnableViewState='false' id='txtEnableMultilingual' runat='Server'/>";
                var AlertCannotDisableEdit = "<asp:Literal EnableViewState='false' id='litAlertCannotDisableEdit' runat='Server'/>";
                var AlertCannotDisableReadonly = "<asp:Literal EnableViewState='false' id='litAlertCannotDisableReadonly' runat='Server'/>";
                var AlertCannotDisablePostReply = "<asp:Literal EnableViewState='false' id='litAlertCannotDisablePostReply' runat='Server'/>";
                var AlertCannotDisableLibraryReadonly = "<asp:Literal EnableViewState='false' id='litAlertCannotDisableLibraryReadonly' runat='Server'/>";

                // Start: Multiple Permissions variables
                var readonly = new Array();
                var edit = new Array();
                var add = new Array();
                var deleteperm = new Array();
                var restore = new Array();
                var addimages = new Array();
                var addfiles = new Array();
                var libraryonly = new Array();
                var addhyperlinks = new Array();
                var transverse_folder = new Array();
                var navigation = new Array();
                var addfolders = new Array();
                var editapproval = new Array();
                var editfolders = new Array();
                var deletefolders = new Array();
                var libreadonly = new Array();
                var overwritelib = new Array();
                var edit_preapproval = new Array();
                // End: Multiple Permissions variables

                function openComment(str)
                {
                    window.open("taskcomment.aspx?ref_type=T&ref_id=-9999&cid="+str+"&LangType="+jsContentLanguage,"cmt_win","width=650,height=350,resizable,scrollbars,status,titlebar");
                }

                function DeclineContent(id, fldid, page, LangType)
                {
                    var redUrl = 'content.aspx?action=declinecontent&id=' + id + '&fldid=' + fldid + '&page=' + page + '&LangType=' + LangType;
                    var declineComment = prompt('Enter a reason for declining this content (optional): ', '');
                    if (declineComment != null) {
                        redUrl += '&comment=' + escape(declineComment);
                        window.location = redUrl;
                    }
                }

                function SubmitForm(FormName, Validate) {
                    // Added support for folder custom field assignment; need
                    // to copy values into hidden field and validate selection:
                    if ((typeof(valAndSaveCFldAssignments)).toLowerCase() != 'undefined') {
                        if (!valAndSaveCFldAssignments()) {
                            return false;
                        }
                    }
                    if ((typeof(valAndSaveCSubAssignments)).toLowerCase() != 'undefined') {
                        if (!valAndSaveCSubAssignments()) {
                            return false;
                        }
                    }
                    if (Validate.length > 0) {
                        if (eval(Validate)) {
                            document.forms[0].submit();
                            return false;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        document.forms[0].submit();
                        return false;
                    }
                }

                function SubmitAddFolderForm(FormName, Validate) {
                    // AddFolder SubmitForm
                    var folderNameVal = "";
                    if (document.forms[0] != null && document.forms[0].foldername != null)
                        folderNameVal = document.forms[0].foldername.value;
                    var parentIdVal = $ektron(".ParentID").val()
                    $ektron.ajax({
                        type: "POST",
                        url: "controls/folder/" + "FolderAction.ashx",
                        data: "action=checkFolderExists&name=" + folderNameVal + "&parentid=" + parentIdVal,
                        async: false,
                        cache: false,
                        success: function(data) {
                            
                            var XML = new Ektron.Xml();
                            var xmlDoc = XML.loadXml(data);
                            var thisData = $ektron(xmlDoc);
                            if (thisData.find("error").length > 0) {
                                alert(thisData.find("error").text());
                                return false;
                            }
                            if ((typeof(valAndSaveCFldAssignments)).toLowerCase() != 'undefined') {
                                if (!valAndSaveCFldAssignments()) {
                                    return false;
                                }
                            }
                            if ((typeof(valAndSaveCSubAssignments)).toLowerCase() != 'undefined') {
                                if (!valAndSaveCSubAssignments()) {
                                    return false;
                                }
                            }
                            if (Validate.length > 0) {
                                if (eval(Validate)) {
                                    document.forms[0].submit();
                                    return false;
                                }
                                else {
                                    return false;
                                }
                            }
                            else {
                                document.forms[0].submit();
                                return false;
                            }
                        }
                    });
                }

                function PreviewXmlConfig() {
                    var SelectedXMLconfig = document.forms.frmContent.xmlconfig[document.forms.frmContent.xmlconfig.selectedIndex].value;
                    if (SelectedXMLconfig != 0) {
                        PopUpWindow('xml_config.aspx?LangType='+jsContentLanguage+'&action=ViewXmlConfiguration&id=' + SelectedXMLconfig + '&caller=content', 'Preview', 700, 540, 1, 0);
                    }
                }
                function PreviewXmlConfigByID(xml_id) {
                    if (xml_id != 0) {
                        PopUpWindow('xml_config.aspx?LangType='+jsContentLanguage+'&action=ViewXmlConfiguration&id=' + xml_id + '&caller=content', 'Preview', 700, 540, 1, 0);
                    }
                }

                var EditHandle = new Object;

                function SelectAllPerms() {
                    //standard permissions
                    if ($ektron("#dvStandard").css("display") == "block"){
                        $ektron("#dvStandard input[type='checkbox']").attr("checked", "checked");
                    }

                    //advanced permissions
                    if ($ektron("#dvAdvanced").css("display") == "block"){
                        $ektron("#dvAdvanced input[type='checkbox']").attr("checked", "checked");
                    }
                    return false;
                }

                function UnselectAllPerms() {
                    //standard permissions
                    if ($ektron("#dvStandard").css("display") == "block"){
                        $ektron("#dvStandard input[type='checkbox']").removeAttr("checked");
                    }

                    //advanced permissions
                    if ($ektron("#dvAdvanced").css("display") == "block"){
                        $ektron("#dvAdvanced input[type='checkbox']").removeAttr("checked");
                    }
                    return false;
                }

                function ConfirmDelete(Publish) {
                    if (Publish) {

                        var msg = "<asp:Literal EnableViewState='false' ID='litConfirmContentDeletePublish' runat='Server'/>";
                    }
                    else {
                        var msg = "<asp:Literal EnableViewState='false' ID='litConfirmContentDeleteSubmission' runat='Server'/>";
                    }
                    return confirm (msg);
                }

                function DeleteConfirmationDialog(href)
                {
                    var msg;
                    var confirmation;
                    msg = "<asp:Literal EnableViewState='false' id='litConfirmContentDeleteDialog' runat='Server'/>";

                    confirmation = confirm(msg);
                    confirmation = confirmation;

                    if (confirmation === true)
                    { window.location = href; }

                    return false;
                }

                function ConfirmFolderDelete(folder) {
                    if (folder != 0) {
                        return confirm (jsConfirmFolderDelete);
                    }
                        return confirm ("<asp:Literal EnableViewState='false' id='litConfirmFolderDeleteBelowRoot' runat='Server'/>");
                }

                function PopulateTemplate() {
                    if (document.forms.frmContent.templateoptions.options[document.forms.frmContent.templateoptions.selectedIndex].value != "ignore")
                    {
                        document.forms.frmContent.templatefilename.value = document.forms.frmContent.templateoptions.options[document.forms.frmContent.templateoptions.selectedIndex].value;
                    }
                }
                function CheckForillegalChar() {
                    var val = document.forms[0].foldername.value;
                    if ((val.indexOf("\\") >= 0) || (val.indexOf("/") >= 0) || (val.indexOf(":") >= 0)||(val.indexOf("*") >= 0) || (val.indexOf("?") >= 0)|| (val.indexOf("\"") >= 0) || (val.indexOf("<") >= 0)|| (val.indexOf(">") >= 0) || (val.indexOf("|") >= 0) || (val.indexOf("&") >= 0) || (val.indexOf("\'") >= 0))
                    {
                        alert("Folder name can't include ('\\', '/', ':', '*', '?', ' \" ', '<', '>', '|', '&', '\'').");
                        return false;
                    }
                    return true;
                }
                function CheckFolderParameters(type) {
                            document.forms.frmContent.foldername.value = Trim(document.forms.frmContent.foldername.value);
                            document.forms.frmContent.templatefilename.value = Trim(document.forms.frmContent.templatefilename.value);
                            document.forms.frmContent.stylesheet.value = Trim(document.forms.frmContent.stylesheet.value);
                            if ((document.forms.frmContent.foldername.value == ""))
                            {
                                alert("<asp:Literal EnableViewState='false' id='litAlertSupplyFoldername' runat='Server'/>");
                                //'Dt.11/28/05,UDAI Added For the defect#16872
                                attemptSetTab("dvProperties");
                                document.forms.frmContent.foldername.focus();
                                return false;
                            }else {
                                if (!CheckForillegalChar()) {
                                    return false;
                                }
                            }
                            if((document.forms.frmContent.foldername.value=="assets") || (document.forms.frmContent.foldername.value=="privateassets"))
                            {
                                alert(' Folder can not be named "Assets" or "PrivateAssets"');
                                return false;
                            }
                            saveSitemapPath();
                            saveSiteAliasList();
                            if (type == "edit") {
                                if( !TemplateConfigSave() )
                                {
                                    return false;
                                }

                                if( !ContentTypeConfigSave() )
                                {
                                    return false;
                                }

                                if( !TaxonomyConfigSave() )
                                {
                                    alert('You must select at least one taxonomy when you require one or more category');
                                    return false;
                                }
                            }

                            if(type == "add" ) {
                                if( !TemplateConfigSave() ) {
                                    return false;
                                }
                                if( !ContentTypeConfigSave() )
                                {
                                    return false;
                                }

                                if( !TaxonomyConfigSave() )
                                {
                                    alert('You must select at least one taxonomy when you require one or more category');
                                    return false;
                                }
                            }

                            var isDomainCheckBox = document.getElementById('IsDomainFolder');
                            if( isDomainCheckBox != null ) {
                                if( isDomainCheckBox.value == 'true'  || isDomainCheckBox.value == 'on') {
                                    var DomainProduction = document.getElementById('DomainProduction');
                                    var DomainStaging = document.getElementById('DomainStaging');
                                    if( (DomainProduction.value == '') || (DomainStaging == '')  ) {
                                        alert("<asp:Literal EnableViewState='false' id='litAlertRequiredDomain' runat='Server'/>");
                                        attemptSetTab("dvProperties");
                                        document.getElementById('DomainProduction').focus();
                                        return false;
                                    }
                                }
                            }
                            var regexp1 = /"/gi;
                            document.forms.frmContent.foldername.value = document.forms.frmContent.foldername.value.replace(regexp1, "'");
                            if (document.forms.frmContent.stylesheet.value.length > 0) {
                                document.forms.frmContent.stylesheet.value = document.forms.frmContent.stylesheet.value.replace(regexp1, "'");
                                var extension = document.forms.frmContent.stylesheet.value.split(".");
                                if (extension[extension.length - 1].toLowerCase() != "css") {
                                    alert("<asp:Literal EnableViewState='false' id='litAlertMissingAlternateStylesheet' runat='Server'/>");
                                    document.forms.frmContent.stylesheet.focus();
                                    return false;
                                }
                            }

                            return true;
                }

                function ConfirmDeletePermissions(base, type) {
                    var msg = ""
                    if (base == "group")
                    {
                        return confirm("<asp:Literal EnableViewState='false' id='litConfirmDeleteGroupPermissions' runat='Server'/>");
                    }
                    else
                    {
                        return confirm("<asp:Literal EnableViewState='false' id='litConfirmDeleteUserPermissions' runat='Server'/>");
                    }
                }
                function CheckPreApprovalSettings()
                {
                    if(!($ektron("#preapproval_chkInheritPreApprovalGroup")[0].checked) && ($ektron("#selectusergroup")[0].value==0)){
                        alert("<asp:Literal EnableViewState='false' id='litSelectPreapprovalGroup' runat='server'/>");
                        return false;
                    }
                    return true;
                }
                function CheckPermissionSettings(permission) {
                    CopyPermissions();
                    return pCheckPermissionSettings(permission);
                }

                function PreviewTemplate(sitepath,width,height)
                {
                    var templar = $ektron("#addTemplate")
                    if (templar.val() != 0) {
                        var selected = templar.find(":selected");
                        var strTemplatename = selected.text();
                        if (strTemplatename.indexOf(" (") != -1)
                        {
                            strTemplatename = strTemplatename.split(" (")[0];
                        }
                        var url = selected.attr("url");
                        if(typeof(url) == 'string' && url != ""){
                            window.open(url,'', 'resizable=1, scrollbars=1', 'toolbar,width=' + width + ',height=' + height);
                        }else{
                            window.open(sitepath + strTemplatename,'', 'resizable=1, scrollbars=1', 'toolbar,width=' + width + ',height=' + height);
                        }
                    } else {
                        alert("<asp:Literal EnableViewState='false' id='litSelectValidTemplate' runat='Server' />");
                    }
                }

                function PreviewSelectedXmlConfig(sitepath,width,height)
                {
                    var templar = document.getElementById("addContentType")
                    if (templar.value != -1) {
                        PopUpWindow('xml_config.aspx?LangType='+jsContentLanguage+'&action=ViewXmlConfiguration&id=' + templar.options[templar.selectedIndex].value + '&caller=content', 'Preview', 700, 540, 1, 0);
                    } else {
                        alert("<asp:Literal EnableViewState='false' runat='server' id='litInValidSmartFrom' />");
                    }
                }

                function PreviewSelectedProductType(sitepath, width, height) {
                    var templar = document.getElementById("addContentType")
                    if (templar.value != -1) {
                        PopUpWindow('commerce/producttypes.aspx?LangType=' + jsContentLanguage + '&action=viewproducttype&id=' + templar.options[templar.selectedIndex].value + '&caller=content', 'Preview', 700, 540, 1, 0);
                    } else {
                        alert("<asp:Literal EnableViewState='false' runat='server' id='litInValidSmartFrom1' />");
                    }
                }

                function PreviewSpecificTemplate(sitepath,width,height)
                {
                    window.open(sitepath,'','toolbar,width=' + width + ',height=' + height);
                }

                function SetMultiplePermissionsVariables()
                {

                    readonly = document.getElementById(UniqueID+"frm_readonly").value.replace("0", "").split(",");
                    readonly.splice(0,1);

                    edit = document.getElementById(UniqueID+"frm_edit").value.replace("0", "").split(",");
                    edit.splice(0,1);

                    add = document.getElementById(UniqueID+"frm_add").value.replace("0", "").split(",");
                    add.splice(0,1);

                    deleteperm = document.getElementById(UniqueID+"frm_delete").value.replace("0", "").split(",");
                    deleteperm.splice(0,1);

                    restore = document.getElementById(UniqueID+"frm_restore").value.replace("0", "").split(",");
                    restore.splice(0,1);

                    addimages = document.getElementById(UniqueID+"frm_addimages").value.replace("0", "").split(",");
                    addimages.splice(0,1);

                    addfiles = document.getElementById(UniqueID+"frm_addfiles").value.replace("0", "").split(",");
                    addfiles.splice(0,1);

                    libraryonly = document.getElementById(UniqueID+"frm_libreadonly").value.replace("0", "").split(",");
                    libraryonly.splice(0,1);

                    addhyperlinks = document.getElementById(UniqueID+"frm_addhyperlinks").value.replace("0", "").split(",");
                    addhyperlinks.splice(0,1);

                    transverse_folder = document.getElementById(UniqueID+"frm_transverse_folder").value.replace("0", "").split(",");
                    transverse_folder.splice(0,1);

                    navigation = document.getElementById(UniqueID+"frm_navigation").value.replace("0", "").split(",");
                    navigation.splice(0,1);

                    addfolders = document.getElementById(UniqueID+"frm_add_folders").value.replace("0", "").split(",");
                    addfolders.splice(0,1);

                    if( null != document.getElementById(UniqueID+"frm_edit_preapproval") )
                    {
                        editapproval = document.getElementById(UniqueID+"frm_edit_preapproval").value.replace("0", "").split(",");
                        editapproval.splice(0,1);
                    }

                    editfolders = document.getElementById(UniqueID+"frm_edit_folders").value.replace("0", "").split(",");
                    editfolders.splice(0,1);

                    deletefolders = document.getElementById(UniqueID+"frm_delete_folders").value.replace("0", "").split(",");
                    deletefolders.splice(0,1);

                    libreadonly = document.getElementById(UniqueID+"frm_libreadonly").value.replace("0", "").split(",");
                    libreadonly.splice(0,1);

                    overwritelib = document.getElementById(UniqueID+"frm_overwritelib").value.replace("0", "").split(",");
                    overwritelib.splice(0,1);

                    if( null != document.getElementById(UniqueID+"frm_edit_preapproval") )
                    {
                        editpreapproval = document.getElementById(UniqueID+"frm_edit_preapproval").value.replace("0", "").split(",");
                        editpreapproval.splice(0,1);
                    }
                }

                function pCheckPermissionSettings (permission) {
                    if(typeof(standardObj.frm_readonly.length) != "undefined")
                    {
                        var i = 0;
                        SetMultiplePermissionsVariables();

                        for(i = 0; i < standardObj.frm_readonly.length; i++ )
                        {
                            if (permission == "frm_readonly") {
                                if (readonly[i] == 0) {

                                    // disable related checkedboxes
                                    standardObj.frm_edit[i].checked = false;
                                    standardObj.frm_add[i].checked = false;
                                    standardObj.frm_delete[i].checked = false;
                                    standardObj.frm_restore[i].checked = false;
                                    standardObj.frm_navigation[i].checked = false;
                                    standardObj.frm_add_folders[i].checked = false;
                                    if (null != document.getElementById(UniqueID+"frm_edit_preapproval")) {
                                        standardObj.frm_edit_preapproval[i].checked = false;
                                    }
                                    standardObj.frm_edit_folders[i].checked = false;
                                    standardObj.frm_delete_folders[i].checked = false;
                                    CopyPermissions();

                                    if ((edit[i] != 0) || (add[i] != 0)
                                        || (deleteperm[i] != 0) || (restore[i] != 0)
                                        || (navigation[i] != 0) || (addfolders[i] != 0)
                                        || (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval") != null)? (editpreapproval[i] != 0):false)
                                        || (editfolders[i] != 0) || (deletefolders[i] != 0)) {
                                        alert(AlertCannotDisableReadonly);
                                        return false;
                                    }
                                }
                            }
                            else if (permission == "frm_libreadonly") {
                                if (libreadonly[i] == 0) {
                                    if ((addimages[i] != 0) || (addfiles[i] != 0)
                                        || (addhyperlinks[i] != 0) || (overwritelib[i] != 0))
                                    {
                                        if (document.getElementById(UniqueID+"frm_restore") != null && standardObj.frm_restore[i] != null)
                                        {
                                            standardObj.frm_addimages[i].checked = false;
                                            standardObj.frm_addfiles[i].checked = false;
                                            standardObj.frm_addhyperlinks[i].checked = false;
                                            standardObj.frm_overwritelib[i].checked = false;
                                            CopyPermissions();
                                            return true;

                                            //alert(AlertCannotDisableLibraryReadonly);
                                        }
                                        else
                                        {
                                            // "readonly" checkbox is "enable post reply" in forum/blog folders
                                            alert(AlertCannotDisablePostReply);
                                        }
                                        return false;
                                    }
                                }
                            }

                            else if (permission == "frm_edit") {
                                if (edit[i] != 0) {
                                    standardObj.frm_readonly[i].checked = true;
                                }
                                else {
                                    // disable related checkedboxes
                                    standardObj.frm_add[i].checked = false;
                                    CopyPermissions();

                                    if (add[i] != 0) {
                                        alert(AlertCannotDisableEdit);
                                        return false;
                                    }
                                }
                            }
                            else if ((permission == "frm_add") && (add[i] != 0)) {
                                standardObj.frm_readonly[i].checked = true;
                                if (standardObj.frm_edit[i].disabled != true) { // only reason we would not do this is if we have a board - SMK
                                    standardObj.frm_edit[i].checked = true;
                                }
                            }
                            else if ((permission == "frm_delete") && (deleteperm[i] != 0)) {
                                standardObj.frm_readonly[i].checked = true;
                            }
                            else if ((permission == "frm_restore") && (restore[i] != 0)) {
                                standardObj.frm_readonly[i].checked = true;
                            }

                            else if(permission=="frm_moderate")
                            {
                                if(standardObj.frm_addimages[i].checked==true)
                                {
                                    standardObj.frm_libreadonly[i].checked=true;
                                    standardObj.frm_delete[i].checked=true;
                                }
                            }
                            else if ((addimages[i] != 0) || (addfiles[i] != 0)
                                        || (addhyperlinks[i] != 0)) {
                                standardObj.frm_libreadonly[i].checked = true;
                            }
                            else if ((permission == "frm_addimages") && (addimages[i] != 0)) {
                                standardObj.frm_libreadonly[i].checked = true;
                            }
                            else if ((permission == "frm_addfiles") && (addfiles[i] != 0)) {
                                standardObj.frm_libreadonly[i].checked = true;
                            }
                            else if ((permission == "frm_addhyperlinks") && (addhyperlinks[i] != 0)) {
                                standardObj.frm_libreadonly[i].checked = true;
                            }
                            else if ((permission == "frm_overwritelib") && (overwritelib[i] != 0)) {
                                standardObj.frm_libreadonly[i].checked = true;
                            }
                            else if ((deleteperm[i] != 0) || (restore[i] != 0)) {
                                standardObj.frm_readonly[i].checked = true;
                            }
                            else if (((permission == "frm_navigation") && (navigation[i] != 0))
                                || ((permission == "frm_add_folders") && (addfolders[i] != 0))
                                || ((permission == "frm_edit_folders") && (editfolders[i] != 0))
                                || (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval")!= null)? (((permission == "frm_edit_preapproval") && (editpreapproval[i] != 0))):false)
                                || ((permission == "frm_delete_folders") && (deletefolders[i] != 0))) {
                                //document.getElementById(UniqueID+"frm_readonly").value = 1;
                                standardObj.frm_readonly[i].checked = true;
                            }
                            

                        }
                    }
                    else
                    {
                        if (permission == "frm_readonly") {
                            if (document.getElementById(UniqueID+"frm_readonly").value == 0) {

                                // disable related checkedboxes
                                standardObj.frm_edit.checked = false;
                                standardObj.frm_add.checked = false;
                                standardObj.frm_delete.checked = false;
                                if(standardObj.frm_restore != null)
                                {
                                     standardObj.frm_restore.checked = false;
                                }
                                if(standardObj.frm_navigation != null)
                                {
                                    standardObj.frm_navigation.checked = false;
                                }
                                standardObj.frm_add_folders.checked = false;
                                if (null != document.getElementById(UniqueID+"frm_edit_preapproval")) {
                                    standardObj.frm_edit_preapproval.checked = false;
                                }
                                standardObj.frm_edit_folders.checked = false;
                                standardObj.frm_delete_folders.checked = false;
                                CopyPermissions();

                                if ((document.getElementById(UniqueID+"frm_edit").value != 0) || (document.getElementById(UniqueID+"frm_add").value != 0)
                                        || (document.getElementById(UniqueID+"frm_delete").value != 0) || (document.getElementById(UniqueID+"frm_restore").value != 0)
                                        || (document.getElementById(UniqueID+"frm_navigation").value != 0) || (document.getElementById(UniqueID+"frm_add_folders").value != 0)
                                        || (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval")!= null)? (document.getElementById(UniqueID+"frm_edit_preapproval").value != 0):false)
                                        || (document.getElementById(UniqueID+"frm_edit_folders").value != 0) || (document.getElementById(UniqueID+"frm_delete_folders").value != 0)) {
                                    alert(AlertCannotDisableReadonly);
                                    return false;
                                }
                            }
                        }
                        else if (permission == "frm_libreadonly") {
                            if (document.getElementById(UniqueID+"frm_libreadonly").value == 0) {
                                if ((document.getElementById(UniqueID+"frm_addimages").value != 0) || (document.getElementById(UniqueID+"frm_addfiles").value != 0)
                                        || (document.getElementById(UniqueID+"frm_addhyperlinks").value != 0) || (document.getElementById(UniqueID+"frm_overwritelib").value != 0))
                                {
                                    if (document.getElementById(UniqueID+"frm_restore") != null && standardObj.frm_restore != null)
                                    {
                                        standardObj.frm_addimages.checked = false;
                                        standardObj.frm_addfiles.checked = false;
                                        standardObj.frm_addhyperlinks.checked = false;
                                        standardObj.frm_overwritelib.checked = false;
                                        CopyPermissions();
                                        return true;

                                        //alert(AlertCannotDisableLibraryReadonly);
                                    }
                                    else
                                    {
                                        // "readonly" checkbox is "enable post reply" in forum/blog folders
                                        alert(AlertCannotDisablePostReply);
                                    }
                                    return false;
                                }
                            }
                        }

                        else if (permission == "frm_edit") {
                            if (document.getElementById(UniqueID+"frm_edit").value != 0) {
                                standardObj.frm_readonly.checked = true;
                            }
                            else {
                                // disable related checkedboxes
                                standardObj.frm_add.checked = false;
                                CopyPermissions();

                                if (document.getElementById(UniqueID+"frm_add").value != 0) {
                                    alert(AlertCannotDisableEdit);
                                    return false;
                                }
                            }
                        }
                        else if ((permission == "frm_add") && (document.getElementById(UniqueID+"frm_add").value != 0)) {
                            standardObj.frm_readonly.checked = true;
                            if (standardObj.frm_edit.disabled != true) { // only reason we would not do this is if we have a board - SMK
                                standardObj.frm_edit.checked = true;
                            }
                        }
                        else if ((permission == "frm_delete") && (document.getElementById(UniqueID+"frm_delete").value != 0)) {
                            standardObj.frm_readonly.checked = true;
                        }
                        else if ((permission == "frm_restore") && (document.getElementById(UniqueID+"frm_restore").value != 0)) {
                            standardObj.frm_readonly.checked = true;
                        }
                        else if(permission=="frm_moderate")
                        {
                            if(standardObj.frm_addimages.checked==true)
                            {
                                standardObj.frm_libreadonly.checked=true;
                                standardObj.frm_delete.checked=true;
                            }
                        }
                        else if ((document.getElementById(UniqueID+"frm_addimages").value != 0) || (document.getElementById(UniqueID+"frm_addfiles").value != 0)
                                        || (document.getElementById(UniqueID+"frm_addhyperlinks").value != 0)) {
                            standardObj.frm_libreadonly.checked = true;
                        }
                        else if ((permission == "frm_addimages") && (document.getElementById(UniqueID+"frm_addimages").value != 0)) {
                            standardObj.frm_libreadonly.checked = true;
                        }
                        else if ((permission == "frm_addfiles") && (document.getElementById(UniqueID+"frm_addfiles").value != 0)) {
                            standardObj.frm_libreadonly.checked = true;
                        }
                        else if ((permission == "frm_addhyperlinks") && (document.getElementById(UniqueID+"frm_addhyperlinks").value != 0)) {
                            standardObj.frm_libreadonly.checked = true;
                        }
                        else if ((permission == "frm_overwritelib") && (document.getElementById(UniqueID+"frm_overwritelib").value != 0)) {
                            standardObj.frm_libreadonly.checked = true;
                        }
                        else if ((document.getElementById(UniqueID+"frm_delete").value != 0) || (document.getElementById(UniqueID+"frm_restore").value != 0)) {
                            standardObj.frm_readonly.checked = true;
                        }
                        else if (((permission == "frm_navigation") && (document.getElementById(UniqueID+"frm_navigation").value != 0))
                                || ((permission == "frm_add_folders") && (document.getElementById(UniqueID+"frm_add_folders").value != 0))
                                || ((permission == "frm_edit_folders") && (document.getElementById(UniqueID+"frm_edit_folders").value != 0))
                                || (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval")!= null)? (((permission == "frm_edit_preapproval") && (document.getElementById(UniqueID+"frm_edit_preapproval").value != 0))):false)
                                || ((permission == "frm_delete_folders") && (document.getElementById(UniqueID+"frm_delete_folders").value != 0))) {
                            document.getElementById(UniqueID+"frm_readonly").value = 1;
                        }
                        
                    }
                }

                function CheckForAnyPermissions() {
                    CopyPermissions();

                    var submitForm = false;
                    var i = 0;
                    if(typeof(standardObj.frm_readonly.length) != "undefined")
                    {
                        SetMultiplePermissionsVariables();

                        for(i = 0; i < standardObj.frm_readonly.length; i++ )
                        {
                            if ((readonly[i] != 0) || (edit[i] != 0) || (add[i] != 0) || (deleteperm[i] != 0) || (restore[i] != 0) || (addimages[i] != 0)
                                || (addfiles[i] != 0) || (addhyperlinks[i] != 0) || (libraryonly[i] != 0) || (transverse_folder[i] != 0))
                            {
                                submitForm = true;
                            }
                            else
                            {
                                submitForm = false;
                            }
                        }
                    }
                    else if ((document.getElementById(UniqueID+"frm_readonly").value != 0)
                            || (document.getElementById(UniqueID+"frm_edit").value != 0) || (document.getElementById(UniqueID+"frm_add").value != 0)
                            || (document.getElementById(UniqueID+"frm_delete").value != 0) || (document.getElementById(UniqueID+"frm_restore").value != 0)
                            || (document.getElementById(UniqueID+"frm_addimages").value != 0)
                            || (document.getElementById(UniqueID+"frm_addfiles").value != 0) || (document.getElementById(UniqueID+"frm_addhyperlinks").value != 0)
                            || (document.getElementById(UniqueID+"frm_libreadonly").value != 0) || (document.getElementById(UniqueID+"frm_transverse_folder").value != 0) )
                    {
                        return true;
                    }

                    if(submitForm == true)
                    {
                        return submitForm;
                    }

                    if (document.getElementById(UniqueID+"frm_membership").value == "true")
                    {
                        return true;
                    }
                    alert("<asp:Literal EnableViewState='false' id='litAlertSelectPermission' runat='Server'/>");
                    return false;
                }

                function CopyPermissions() {
                    CopyStdPermissions();
                    CopyAdvPermissions();
                }

                function CopyStdPermissions() {
                    GetFormObjects();

                    document.getElementById(UniqueID+"frm_readonly").value = "";
                    document.getElementById(UniqueID+"frm_edit").value = "";
                    document.getElementById(UniqueID+"frm_add").value = "";
                    document.getElementById(UniqueID+"frm_delete").value = "";
                    document.getElementById(UniqueID+"frm_restore").value = "";
                    document.getElementById(UniqueID+"frm_libreadonly").value = "";
                    document.getElementById(UniqueID+"frm_addimages").value = "";
                    document.getElementById(UniqueID+"frm_addfiles").value = "";
                    document.getElementById(UniqueID+"frm_addhyperlinks").value = "";
                    document.getElementById(UniqueID+"frm_overwritelib").value = "";

                    var i = 0;
                    if(typeof(standardObj.frm_readonly.length) != "undefined"){
                        for(i = 0; i < standardObj.frm_readonly.length; i++ )
                        {
                            document.getElementById(UniqueID+"frm_readonly").value += "," + (standardObj.frm_readonly[i].checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_edit").value += "," + (standardObj.frm_edit[i].checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_add").value += "," + (standardObj.frm_add[i].checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_delete").value += "," + (standardObj.frm_delete[i].checked ? 1 : 0);
                            if (document.getElementById(UniqueID+"frm_restore") != null && standardObj.frm_restore != null)
                            {
                                document.getElementById(UniqueID+"frm_restore").value += "," + (standardObj.frm_restore[i].checked ? 1 : 0);
                            }
                            document.getElementById(UniqueID+"frm_libreadonly").value += "," + (standardObj.frm_libreadonly[i].checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_addimages").value += "," + (standardObj.frm_addimages[i].checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_addfiles").value += "," + (standardObj.frm_addfiles[i].checked ? 1 : 0);
                            if (document.getElementById(UniqueID+"frm_addhyperlinks") != null && standardObj.frm_addhyperlinks != null)
                            {
                                document.getElementById(UniqueID+"frm_addhyperlinks").value += "," + (standardObj.frm_addhyperlinks[i].checked ? 1 : 0);
                                document.getElementById(UniqueID+"frm_overwritelib").value += "," + (standardObj.frm_overwritelib[i].checked ? 1 : 0);
                            }
                        }
                    }
                    else
                    {
                        document.getElementById(UniqueID+"frm_readonly").value = (standardObj.frm_readonly.checked ? 1 : 0);
                        document.getElementById(UniqueID+"frm_edit").value = (standardObj.frm_edit.checked ? 1 : 0);
                        document.getElementById(UniqueID+"frm_add").value = (standardObj.frm_add.checked ? 1 : 0);
                        document.getElementById(UniqueID+"frm_delete").value = (standardObj.frm_delete.checked ? 1 : 0);
                        if (document.getElementById(UniqueID+"frm_restore") != null && standardObj.frm_restore != null)
                        {
                            document.getElementById(UniqueID+"frm_restore").value = (standardObj.frm_restore.checked ? 1 : 0);
                        }
                        document.getElementById(UniqueID+"frm_libreadonly").value = (standardObj.frm_libreadonly.checked ? 1 : 0);
                        document.getElementById(UniqueID+"frm_addimages").value = (standardObj.frm_addimages.checked ? 1 : 0);
                        document.getElementById(UniqueID+"frm_addfiles").value = (standardObj.frm_addfiles.checked ? 1 : 0);
                        if (document.getElementById(UniqueID+"frm_addhyperlinks") != null && standardObj.frm_addhyperlinks != null)
                        {
                            document.getElementById(UniqueID+"frm_addhyperlinks").value = (standardObj.frm_addhyperlinks.checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_overwritelib").value = (standardObj.frm_overwritelib.checked ? 1 : 0);
                        }
                    }
                }

                function RestoreStdPermissions() {
                    GetFormObjects();
                    standardObj.frm_readonly.checked = ((document.getElementById(UniqueID+"frm_readonly").value != 0) ? true : false);
                    standardObj.frm_edit.checked = ((document.getElementById(UniqueID+"frm_edit").value != 0) ? true : false);
                    standardObj.frm_add.checked = ((document.getElementById(UniqueID+"frm_add").value != 0) ? true : false);
                    standardObj.frm_delete.checked = ((document.getElementById(UniqueID+"frm_delete").value != 0) ? true : false);
                    if (document.getElementById(UniqueID+"frm_restore") != null && standardObj.frm_restore != null)
                    {
                        standardObj.frm_restore.checked = ((document.getElementById(UniqueID+"frm_restore").value != 0) ? true : false);
                    }
                    standardObj.frm_libreadonly.checked = ((document.getElementById(UniqueID+"frm_libreadonly").value != 0) ? true : false);
                    standardObj.frm_addimages.checked = ((document.getElementById(UniqueID+"frm_addimages").value != 0) ? true : false);
                    standardObj.frm_addfiles.checked = ((document.getElementById(UniqueID+"frm_addfiles").value != 0) ? true : false);
                    if (document.getElementById(UniqueID+"frm_addhyperlinks") != null && standardObj.frm_addhyperlinks != null)
                    {
                        standardObj.frm_addhyperlinks.checked = ((document.getElementById(UniqueID+"frm_addhyperlinks").value != 0) ? true : false);
                        standardObj.frm_overwritelib.checked = ((document.getElementById(UniqueID+"frm_overwritelib").value != 0) ? true : false);
                    }
                }

                function CopyAdvPermissions() {
                    GetFormObjects();

                    document.getElementById(UniqueID+"frm_navigation").value = "";
                    document.getElementById(UniqueID+"frm_add_folders").value= "";
                    document.getElementById(UniqueID+"frm_edit_folders").value = "";
                    document.getElementById(UniqueID+"frm_delete_folders").value= "";
                    document.getElementById(UniqueID+"frm_transverse_folder").value = "";

                    if (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval")!= null))
                    {
                        document.getElementById(UniqueID+"frm_edit_preapproval").value = "";
                    }

                    var i = 0;

                    if(typeof(advancedObj.frm_readonly.length) != "undefined")
                    {
                        for(i = 0; i < advancedObj.frm_readonly.length; i++ )
                        {
                            if (document.getElementById(UniqueID+"frm_navigation") != null && advancedObj.frm_navigation != null)
                            {
                                document.getElementById(UniqueID+"frm_navigation").value += "," + (advancedObj.frm_navigation[i].checked ? 1 : 0);
                            }
                            document.getElementById(UniqueID+"frm_add_folders").value += "," + (advancedObj.frm_add_folders[i].checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_edit_folders").value += "," + (advancedObj.frm_edit_folders[i].checked ? 1 : 0);
                            document.getElementById(UniqueID+"frm_delete_folders").value += "," + (advancedObj.frm_delete_folders[i].checked ? 1 : 0);
                            if (document.getElementById(UniqueID+"frm_transverse_folder") != null && advancedObj.frm_transverse_folder != null)
                            {
                                document.getElementById(UniqueID+"frm_transverse_folder").value += "," + (advancedObj.frm_transverse_folder[i].checked ? 1 : 0);
                            }
                            if (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval")!= null))
                            {
                                document.getElementById(UniqueID+"frm_edit_preapproval").value += "," + (advancedObj.frm_edit_preapproval[i].checked ? 1 : 0);
                            }
                        }
                    }
                    else
                    {
                        if (document.getElementById(UniqueID+"frm_navigation") != null && advancedObj.frm_navigation != null)
                        {
                            document.getElementById(UniqueID+"frm_navigation").value = (advancedObj.frm_navigation.checked ? 1 : 0);
                        }
                        document.getElementById(UniqueID+"frm_add_folders").value = (advancedObj.frm_add_folders.checked ? 1 : 0);
                        document.getElementById(UniqueID+"frm_edit_folders").value = (advancedObj.frm_edit_folders.checked ? 1 : 0);
                        document.getElementById(UniqueID+"frm_delete_folders").value = (advancedObj.frm_delete_folders.checked ? 1 : 0);
                        if (document.getElementById(UniqueID+"frm_transverse_folder") != null && advancedObj.frm_transverse_folder != null)
                        {
                            document.getElementById(UniqueID+"frm_transverse_folder").value = (advancedObj.frm_transverse_folder.checked ? 1 : 0);
                        }
                        if (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval")!= null))
                        {
                            document.getElementById(UniqueID+"frm_edit_preapproval").value = (advancedObj.frm_edit_preapproval.checked ? 1 : 0);
                        }
                    }
                }


                function RestoreAdvPermissions() {
                    if (document.getElementById(UniqueID+"frm_navigation") != null && advancedObj.frm_navigation != null)
                    {
                        advancedObj.frm_navigation.checked = ((document.getElementById(UniqueID+"frm_navigation").value != 0) ? true : false);
                    }
                    advancedObj.frm_add_folders.checked = ((document.getElementById(UniqueID+"frm_add_folders").value != 0) ? true : false);
                    advancedObj.frm_edit_folders.checked = ((document.getElementById(UniqueID+"frm_edit_folders").value != 0) ? true : false);
                    advancedObj.frm_delete_folders.checked = ((document.getElementById(UniqueID+"frm_delete_folders").value != 0) ? true : false);
                    if (document.getElementById(UniqueID+"frm_transverse_folder") != null && advancedObj.frm_transverse_folder != null)
                    {
                        advancedObj.frm_transverse_folder.checked = ((document.getElementById(UniqueID+"frm_transverse_folder").value != 0) ? true : false);
                    }
                    if (("object"==typeof(document.getElementById(UniqueID+"frm_edit_preapproval"))) && (document.getElementById(UniqueID+"frm_edit_preapproval")!= null))
                        {advancedObj.frm_edit_preapproval.checked = ((document.getElementById(UniqueID+"frm_edit_preapproval").value != 0) ? true : false);}
                }


                function GetFormObjects() {
                    if (document.layers) {
                        standardObj = document.layers["permLayer"].document.layers["standard"].document.forms.frmContent;
                        advancedObj = document.layers["permLayer"].document.layers["advanced"].document.forms.frmContent;
                    }
                    else if (document.all) {
                        standardObj = document.forms.frmContent;
                        advancedObj = document.forms.frmContent;
                    }
                    else {
                        standardObj = document.forms["frmContent"];
                        advancedObj = document.forms["frmContent"];
                    }
                }

                function CheckEditPermissions() {
                    CopyPermissions();
                    if (CheckForAnyPermissions()) {
                        if (document.getElementById(UniqueID+"frm_membership").value == "true"){
                            return true;
                        }
                        if ((document.getElementById(UniqueID+"frm_readonly").value == false)
                            && (document.getElementById(UniqueID+"frm_origreadonly").value != false)) {
                            return confirm("<asp:Literal EnableViewState='false' id='litAlertReadContentPermissionRemovalEffectWarning' runat='Server'/>");
                        }
                        return true;
                    }
                    return false;
                }


                function CheckInheritance(input, id, type) {
                    var inheritPermissions = $ektron(input)[0].checked == true ? true : false;
                    if (!inheritPermissions) {
                        if (confirm("<asp:Literal EnableViewState='false' id='litConfirmDisableInheritance' runat='Server'/>")) {
                            self.location.href="content.aspx?LangType="+jsContentLanguage+"&action=DisableItemInheritance&id=" + id + "&type=" + type + "&membership=<asp:literal id="litDisableInheritenceIfMembershipTrue" runat="Server"/>";
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        if (confirm("<asp:Literal EnableViewState='false' id='litConfirmEnableInheritance' runat='Server'/>")) {
                            self.location.href="content.aspx?LangType="+jsContentLanguage+"&action=EnableItemInheritance&id=" + id + "&type=" + type + "&membership=<asp:literal id="litEnableInheritenceIfMembershipTrue" runat="Server"/>";
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                }

                function CheckPrivateContent(input, id, type) {
                    var isContentPrivate = $ektron(input)[0].checked == true ? true : false;
                    var msg = "";
                    var jscontentprivate="<asp:Literal EnableViewState='false' id='litConfirmMakeContentPrivate' runat='Server'/>";
                    var jscontentpublic="<asp:Literal EnableViewState='false' id='litConfirmMakeContentPublic' runat='Server'/>";
                    if ((type != "folder") || (id != 0)) {
                        var inheritPermissions = $ektron("span.inheritPermissions input")[0].checked;
                        if (inheritPermissions == true) {
                            alert("<asp:Literal EnableViewState='false' id='litAlertCannotAlterPContSetting' runat='Server'/>");
                            return false;
                        }
                    }
                    if (isContentPrivate) {
                        if (type == "folder") {
                            msg += "<asp:Literal EnableViewState='false' id='litConfirmMakeFolderPrivate' runat='Server'/>";
                        }
                        else {
                            msg += jscontentprivate;
                        }
                        if (confirm(msg)) {
                            self.location.href="content.aspx?LangType="+jsContentLanguage+"&action=EnableItemPrivateSetting&id=" + id + "&type=" + type + "&membership=<asp:literal id="litEnableItemPrivateSettingMembershipTrue" runat="Server"/>";
                        }
                    }
                    else {
                        if (type == "folder") {
                            msg += "<asp:Literal EnableViewState='false' id='litConfirmMakeFolderPublic' runat='Server'/>";
                        }
                        else {
                            msg += jscontentpublic;
                        }
                        if (confirm(msg)) {
                            self.location.href="content.aspx?LangType="+jsContentLanguage+"&action=DisableItemPrivateSetting&id=" + id + "&type=" + type + "&membership=<asp:literal id="litDisableItemPrivateSettingMembershipTrue" runat="Server"/>";
                        }
                    }
                    return false;
                }

                function CheckApprovalAddition(type) {
                    if (type == "group") {
                        return confirm("<asp:Literal EnableViewState='false' id='litConfirmAddApproverGroup' runat='Server'/>");
                    }
                    else {
                        return confirm("<asp:Literal EnableViewState='false' id='litConfirmAddApproverUser' runat='Server'/>");
                    }
                }

                function ConfirmDeleteApprovals(base, type) {
                    var msg = ""
                    if (base == "group")
                    {
                        return confirm("<asp:Literal EnableViewState='false' id='litConfirmDeleteApproverGroup' runat='Server'/>");
                    }
                    else
                    {
                        return confirm("<asp:Literal EnableViewState='false' id='litConfirmDeleteApproverUser' runat='Server'/>");
                    }
                }

                function ConfirmRestoreInheritance() {
                    var msg = "<asp:Literal EnableViewState='false' id='lit_restorelInheritance' runat='Server' />";
                    return confirm(msg);
                }

                function Move(sDir, objList, objOrder) {
                    if (objList.selectedIndex != null) {
                        nSelIndex = objList.selectedIndex;
                        if (nSelIndex > -1)
                        {
                            objList.selectedIndex = 0;
                            sSelValue = objList[nSelIndex].value;
                            sSelText = objList[nSelIndex].text;
                            if (sDir == "up" && nSelIndex > 0) {
                                sSwitchValue = objList[nSelIndex -1].value;
                                sSwitchText = objList[nSelIndex - 1].text;
                                objList[nSelIndex].value = sSwitchValue;
                                objList[nSelIndex].text = sSwitchText;
                                objList[nSelIndex - 1].value = sSelValue;
                                objList[nSelIndex - 1].text = sSelText;
                                objList[nSelIndex - 1].selected = true;
                            }
                            else if (sDir == "dn" && nSelIndex < (objList.length - 1)) {
                                sSwitchValue = objList[nSelIndex + 1].value;
                                sSwitchText = objList[nSelIndex +  1].text;
                                objList[nSelIndex].value = sSwitchValue;
                                objList[nSelIndex].text = sSwitchText;
                                objList[nSelIndex + 1].value = sSelValue;
                                objList[nSelIndex + 1].text = sSelText;
                                objList[nSelIndex + 1].selected = true;
                            }
                        }
                        else
                        {
                            alert("<asp:Literal EnableViewState='false' id='litAlertSelectUserOrGroup' runat='Server'/>");
                        }
                    }
                    objOrder.value = "";
                    for (i = 0; i < objList.length; i++) {
                        objOrder.value = objOrder.value + objList[i].value;
                        if (i < (objList.length - 1)) {
                            objOrder.value = objOrder.value + ",";
                        }
                    }
                }

                function CheckKeyValue(item, keys) {
                    var keyArray = keys.split(",");
                    for (var i = 0; i < keyArray.length; i++) {
                        if ((document.layers) || ((!document.all) && (document.getElementById))) {
                            if (item.which == keyArray[i]) {
                                return false;
                            }
                        }
                        else {
                            if (event.keyCode == keyArray[i]) {
                                return false;
                            }
                        }
                    }
                }

                function CheckForMeta(Completed) {
                    if (Completed) {
                        return true;
                    }
                    var obj = document.getElementById("dvHoldMessage");
                    if (obj != null) {
                        obj.style.display = "none";
                    }
                    alert("<asp:Literal EnableViewState='false' id='litAlertMetadataNotCompleted' runat='Server'/>");
                    return false;
                }

                function PopEditWindow(URLInfo, Name, a, b, c, d) {
                    EditHandle = PopUpWindow(URLInfo, Name, a, b, c, d);
                    top.SetEditor(EditHandle);
                }

                function SetMetaComplete(Complete, ID) {
                    top.SetMetaComplete(Complete, ID);
                }

                function Trim (string) {
                    if (string.length > 0) {
                        string = RemoveLeadingSpaces (string);
                    }
                    if (string.length > 0) {
                        string = RemoveTrailingSpaces(string);
                    }
                    return string;
                }

                function RemoveLeadingSpaces(string) {
                    while(string.substring(0, 1) == " ") {
                        string = string.substring(1, string.length);
                    }
                    return string;
                }

                function RemoveTrailingSpaces(string) {
                    while(string.substring((string.length - 1), string.length) == " ") {
                        string = string.substring(0, (string.length - 1));
                    }
                    return string;
                }

                function SwapPermDisplay() {
                    if (document.layers) {
                        var NavObj = document.layers["permLayer"].document.layers;
                        var NavObj1 = document.layers["messLayer"].document.layers;
                        if (NavObj["standard"].visibility == "show") {
                            if (typeof(document.layers["enablealldivns"]) == "object") {
                                CopyStdPermissions();
                            }
                            NavObj["standard"].visibility = "hidden";
                            NavObj1["advancedMess"].visibility = "hidden";
                            NavObj["advanced"].visibility = "show";
                            NavObj1["standardMess"].visibility = "show";
                            if (typeof(document.layers["enablealldivns"]) == "object") {
                                document.layers["enablealldivns"].visibility = "hidden";
                                document.layers["blankdivns"].visibility = "show";
                            }
                            if (typeof(document.layers["enablealldivns"]) == "object") {
                                RestoreAdvPermissions();
                            }
                        }
                        else {
                            if (typeof(document.layers["enablealldivns"]) == "object") {
                                CopyAdvPermissions();
                            }
                            NavObj["standard"].visibility = "show";
                            NavObj1["advancedMess"].visibility = "show";
                            NavObj["advanced"].visibility = "hidden";
                            NavObj1["standardMess"].visibility = "hidden";
                            if (typeof(document.layers["enablealldivns"]) == "object") {
                                document.layers["enablealldivns"].visibility = "show";
                                document.layers["blankdivns"].visibility = "hidden";
                            }
                            if (typeof(document.layers["enablealldivns"]) == "object") {
                                RestoreStdPermissions();
                            }
                        }
                    }
                    else if (document.all) {
                        var NavObj = document.all;
                        if (NavObj["standard"].style.display == "block") {
                            if (typeof(document.all["enablealldiv"]) == "object") {
                                CopyStdPermissions();
                            }
                            NavObj["standard"].style.display = "none";
                            NavObj["advancedMess"].style.display = "none";
                            NavObj["advanced"].style.display = "block";
                            NavObj["standardMess"].style.display = "block";
                            if (typeof(document.all["enablealldiv"]) == "object") {
                                NavObj["enablealldiv"].style.display = "none";
                                NavObj["blankdiv"].style.display = "block";
                            }
                            if (typeof(document.all["enablealldiv"]) == "object") {
                                RestoreAdvPermissions();
                            }
                        }
                        else {
                            if (typeof(document.all["enablealldiv"]) == "object") {
                                CopyAdvPermissions();
                            }
                            NavObj["standard"].style.display = "block";
                            NavObj["advancedMess"].style.display = "block";
                            NavObj["advanced"].style.display = "none";
                            NavObj["standardMess"].style.display = "none";
                            if (typeof(document.all["enablealldiv"]) == "object") {
                                NavObj["enablealldiv"].style.display = "block";
                                NavObj["blankdiv"].style.display = "none";
                            }
                            if (typeof(document.all["enablealldiv"]) == "object") {
                                RestoreStdPermissions();
                            }
                        }
                    }
                    else if (document.getElementById) {
                        if (document.getElementById("standard").style.display == "block") {
                            if (document.getElementById("enablealldiv") != null) {
                                CopyStdPermissions();
                            }
                            document.getElementById("standard").style.display = "none";
                            document.getElementById("advancedMess").style.display = "none";
                            document.getElementById("advanced").style.display = "block";
                            document.getElementById("standardMess").style.display = "block";
                            if (document.getElementById("enablealldiv") != null) {
                                document.getElementById("enablealldiv").style.display = "none";
                                document.getElementById("blankdiv").style.display = "block";
                            }
                            if (document.getElementById("enablealldiv") != null) {
                                RestoreAdvPermissions();
                            }
                        }
                        else {
                            if (document.getElementById("enablealldiv") != null) {
                                CopyAdvPermissions();
                            }
                            document.getElementById("standard").style.display = "block";
                            document.getElementById("advancedMess").style.display = "block";
                            document.getElementById("advanced").style.display = "none";
                            document.getElementById("standardMess").style.display = "none";
                            if (document.getElementById("enablealldiv") != null) {
                                document.getElementById("enablealldiv").style.display = "block";
                                document.getElementById("blankdiv").style.display = "none";
                            }
                            if (document.getElementById("enablealldiv") != null) {
                                RestoreStdPermissions();
                            }
                        }
                    }
                }

                function CheckXmlInheritance() {
                    if (document.forms.frmContent.frm_xmlinheritance.checked == true) {
                        document.forms.frmContent.xmlconfig.selectedIndex = 0;
                        return false;
                    }
                    return true;
                }

                function CheckConfigSelection() {
                    if (document.forms.frmContent.frm_xmlinheritance.checked == true) {
                        document.forms.frmContent.xmlconfig.selectedIndex = 0;
                        document.forms.frmContent.xmlconfig.disabled = true;
                    }
                    else {
                        document.forms.frmContent.xmlconfig.disabled = false;
                    }
                }

                function IsBrowserIE()
                {
                    // document.all is an IE only property
                    return (document.all ? true : false);
                }

                function ShowPane(tabID)
                {
                    $ektron(".tabContainer").tabs('select', tabID);
                }

                var arrInput = new Array(0);
                var arrInputValue = new Array(0);

                function noInputs(){
                    if (arrInput.length == 0){
                        document.getElementById('parah').innerHTML += '<p style="color:silver;padding:.25em;margin:0;">No Subjects Added</p>';
                    }
                }

                function addInput() {
                  arrInput.push(arrInput.length);
                  arrInputValue.push("");
                  var bSlow;
                  display();
                  bSlow = "Slowdown('category" + (arrInput.length - 1) + "');";
                  setTimeout(bSlow, 200);
                  Ektron.Workarea.Grids.show();
                }
                function Slowdown(fname){
                    document.getElementById(fname).focus();
                }

                  function addInputInit(value) {
                  //arrInput.push(createInput(arrInput.length));
                  arrInput.push(arrInput.length);
                  //arrInputValue.push(arrInputValue.length);
                  arrInputValue.push(value);
                  //document.getElementById('categorylength').value = arrInput.length;
                }

                function display() {
                    document.getElementById('parah').innerHTML="";
                    for (intI=0;intI<arrInput.length;intI++) {
                        document.getElementById('parah').innerHTML +=createInput(arrInput[intI], arrInputValue[intI]);
                    }
                    document.getElementById('categorylength').value = arrInput.length;
                }

                function saveValue(intId,strValue) {
                  arrInputValue[intId]=strValue;
                }

                function createInput(id,value) {
                    var removeLbText="<asp:Literal EnableViewState='false' runat='server' id='ltr_RemovaItemText' />";
                    var rcreateExp = /\'/gi;
                    var svalue = value.replace(rcreateExp, "&#39;");
                    var markup = "";
                    markup += "<input type='text' id='category" + id + "' name='category" + id + "' onchange='saveValue(" + id + ",this.value)' value='" + svalue + "' maxlength='75'> ";
                    markup += "<a href='javascript:removeInput(" + id + ")' class='button buttonInlineBlock redHover buttonRemove'>"+removeLbText+"</a>";
                    markup += "<div class='ektronTopSpace'></div>"
                    return markup;
                  //&nbsp;<a href='javascript:removeInput(" + id + ")'>Remove</a>
                }

                function deleteInput() {
                    //remove last
                    var cnfm = confirm("<asp:Literal EnableViewState='false' runat='server' id='ltr_RemovaLastItemText' />");
                    if (cnfm == true)
                    {
                        if (arrInput.length > 0) {
                            arrInput.pop();
                            arrInputValue.pop();
                        }
                        display();
                        Ektron.Workarea.Grids.show();
                    }
                }

                function removeInput(id) {
                    //remove individual
                    var cnfm = confirm("<asp:Literal EnableViewState='false' runat='server' id='ltrRemoveSubjectItemMsg' />");
                    if (cnfm == true)
                    {

                        if (arrInput.length > 0) {
                        //arrInput.splice(id,1);
                            arrInput.pop();
                            arrInputValue.splice(id,1);
                        }
                        display();
                        if (arrInput.length == 0){
                            noInputs();
                        }
                        Ektron.Workarea.Grids.show();
                    }
                }

                ///Blog Roll

                var arrRoll = new Array(0);
                var arrRollValue = new Array(0);
                var arrRollURL = new Array(0);
                var arrRollShort = new Array(0);
                var arrRollRel = new Array(0);

                function addRoll() {
                  arrRoll.push(arrRoll.length);
                  arrRollValue.push("");
                  arrRollURL.push("");
                  arrRollShort.push("");
                  arrRollRel.push("");
                  displayRoll();
                }

                  function addRollInit(linkname,url,shortdesc,rel) {
                  arrRoll.push(arrRoll.length);
                  arrRollValue.push(linkname);
                  arrRollURL.push(url);
                  arrRollShort.push(shortdesc);
                  arrRollRel.push(rel);
                }

                function displayRoll()
                {
                    var sItem = "";
                    document.getElementById('proll').innerHTML = '&#160;';

                    for (intI = 0; intI < arrRoll.length; intI++)
                    {
                        sItem = createRoll(arrRoll[intI], arrRollValue[intI], arrRollURL[intI], arrRollShort[intI], arrRollRel[intI]);
                        document.getElementById('proll').innerHTML += sItem; //createRoll(arrRoll[intI], arrRollValue[intI]);
                    }

                    Ektron.Workarea.Grids.show();

                    if (document.getElementById('rolllength') != null)
                    {
                        document.getElementById('rolllength').value = arrRoll.length;
                    }
                }

                function saveRoll(intId,strValue,type) {
                    if (type == "linkname") {
                        arrRollValue[intId]=strValue;
                    }else if (type == "url") {
                        arrRollURL[intId]=strValue;
                    }else if (type == "short") {
                        arrRollShort[intId]=strValue;
                    }else if (type == "rel") {
                        arrRollRel[intId]=strValue;
                    }
                }

                function createRoll(id,linkname,url,shortdesc,rel) {//,url,shortdesc,rel) {
                  var sRet = "";
                  var removeBlogLinkTtite   ="<asp:Literal EnableViewState='false' runat='server' id='ltrBlogRollLinkRemove' />";
                  var EditTitle             ="<asp:Literal EnableViewState='false' runat='server' id='ltrBlogRollEditTitle' />";
                  var lbl_BR_LinkName       ="<asp:Literal EnableViewState='false' runat='server' id='lbl_BR_LinkName' />";//lbl roll link name
                  var lbl_BR_URL            ="<asp:Literal EnableViewState='false' runat='server' id='lbl_BR_URL' />";
                  var lbl_BR_ShortDesc      ="<asp:Literal EnableViewState='false' runat='server' id='lbl_BR_ShortDesc' />";
                  var lbl_BR_Relationship   ="<asp:Literal EnableViewState='false' runat='server' id='lbl_BR_Relationship' />"

                  sRet = sRet + "<a href=\"#\" onclick=\"removeRoll(" + id + ")\" class=\"button buttonInlineBlock redHover buttonRemove\">"+removeBlogLinkTtite+"</a>"
                  sRet = sRet + "<div class=\"ektronTopSpace\"></div>"
                  sRet = sRet + "<table class='ektronGrid'>";
                  sRet = sRet + "<tr class=\"title-header\">"
                  sRet = sRet + "<td class=\"label\">"+lbl_BR_LinkName+":</td>"
                  sRet = sRet + "<td class=\"value\"><input name=\"editfolder_linkname" + id + "\" type=\"text\" value=\"" + linkname + "\" id=\"editfolder_linkname" + id + "\" onchange=\"saveRoll("+ id + ",this.value,'linkname')\" /></td>"
                  sRet = sRet + "</tr>";
                  if (url.length > 0) {
                  sRet = sRet + "<tr>"
                  sRet = sRet + "<td class=\"label\">"+lbl_BR_URL+":</td>"
                  sRet = sRet + "<td class=\"value\"><input name=\"editfolder_url" + id + "\" type=\"text\" value=\"" + url + "\" id=\"editfolder_url" + id + "\" onchange=\"saveRoll("+ id + ",this.value,'url')\" /></td>"
                  sRet = sRet + "</tr>";
                  } else {
                  sRet = sRet + "<tr>"
                  sRet = sRet + "<td class=\"label\">"+lbl_BR_URL+":</td>"
                  sRet = sRet + "<td class=\"value\"><input name=\"editfolder_url" + id + "\" type=\"text\" value=\"http://\" id=\"editfolder_url" + id + "\" onchange=\"saveRoll("+ id + ",this.value,'url')\" /></td>"
                  sRet = sRet + "</tr>";
                  }
                  sRet = sRet + "<tr>"
                  sRet = sRet + "<td class=\"label\">"+lbl_BR_ShortDesc+":</td>"
                  sRet = sRet + "<td class=\"value\"><input name=\"editfolder_short" + id + "\" type=\"text\" value=\"" + shortdesc + "\" id=\"editfolder_short" + id + "\" onchange=\"saveRoll("+ id + ",this.value,'short')\" /></td>"
                  sRet = sRet + "</tr>";
                  sRet = sRet + "<tr>"
                  sRet = sRet + "<td class=\"label\">"+lbl_BR_Relationship+":</td>"
                  sRet = sRet + "<td class=\"value\"><input name=\"editfolder_rel" + id + "\" type=\"text\" value=\"" + rel + "\" size=\"45\" id=\"editfolder_rel" + id + "\" onchange=\"saveRoll("+ id + ",this.value,'rel')\" />&nbsp;<a style=\"padding-top: .25em; padding-bottom: .25em;\" class=\"button buttonInline blueHover buttonEdit\" href=\"#\" onclick=\"window.open('blogs/xfnbuilder.aspx?field=editfolder_rel" + id + "&id=" + id + "','XFNBuilder','location=0,status=0,scrollbars=0,width=500,height=300');\">"+EditTitle+"</a></td>"
                  sRet = sRet + "</tr>";
                  sRet = sRet + "</table>";
                  sRet = sRet + "<div class=\"ektronTopSpace\"></div>"
                  return sRet;
                }

                function deleteRoll() {
                    //remove last
                    var cnfm = confirm("<asp:Literal EnableViewState='false' runat='server' id='lbl_roll_RemoveLastMsg' />");
                    if (cnfm == true)
                    {
                      if (arrRoll.length > 0) {
                         arrRoll.pop();
                         arrRollValue.pop();
                         arrRollURL.pop();
                         arrRollShort.pop();
                         arrRollRel.pop();
                      }
                      displayRoll();
                    }
                }

                function removeRoll(id) {
                    //remove last
                    var cnfm = confirm("<asp:Literal EnableViewState='false' runat='server' id='lbl_roll_RemoveThisMsg' />");
                    if (cnfm == true)
                    {
                      if (arrRoll.length > 0) {
                         arrRoll.pop();
                         arrRollValue.splice(id,1);
                         arrRollURL.splice(id,1);
                         arrRollShort.splice(id,1);
                         arrRollRel.splice(id,1);
                      }
                      displayRoll();
                    }
                }
                // BEGIN multitemplate/multixml functions
                function NormalizeDisplay(tableName, evenIndicator)
                {
                    // no longer necessary as of v8.0
                }

                function AddSelectEntry(element_id, xml_id, xml_name, url)
                {
                    //var selectList = document.getElementById(element_id);
                    //var newOption = document.createElement('option');
                    //newOption.value = xml_id;
                    //newOption.appendChild(document.createTextNode(xml_name));
                    //selectList.appendChild(newOption);
                    $ektron("#" + element_id).append("<option value=\"" + xml_id + "\">" + xml_name + "</option>");
                    if(typeof(url) == "string" && url != ""){
                        $ektron("#" + element_id + " option[value='"+xml_id+"']").attr("url", url);
                    }
                }

                // much thanks to http://www.easy-reader.net/archives/2005/09/02/death-to-bad-dom-implementations/
                function createElementWithName(tag, name, type) {
                    var element;
                    if ("input" == tag.toLowerCase() && "undefined" == typeof type)
                    {
                        type = "radio";
                    }
                    // First try the IE way; if this fails then use the standard way
                    if (document.all) {
                        element = $ektron("<" + tag + " type=\"" + type + "\" name=\"" + name + "\" />").get(0);
                    } else {
                        element = document.createElement(tag);
                        element.setAttribute('name', name);
                        element.setAttribute('type', type);
                    }
                    return element;
                }

                function RemoveSelectEntry(element_id, xml_id)
                {
                    var selectList = document.getElementById(element_id);
                    var i;
                    for( i = 0; i < selectList.childNodes.length; i++ ) {
                        if( selectList.childNodes[i].value == xml_id ) {
                            selectList.removeChild(selectList.childNodes[i]);
                            return;
                        }
                    }
                }

                function ToggleRequireSmartForms()
                {
                    var required = document.getElementById('requireSmartForms');
                    if( required.checked ) {
                        RemoveHTML();
                    }
                    else {
                        AddSelectEntry('addContentType', 0, 'Blank HTML');
                        setTimeout("var selectList = document.getElementById('addContentType');selectList.value = 0;ActivateContentType();", 10);
                        required.checked = false;
                    }
                }

                function RemoveHTML()
                {
                    var isEnabled = !document.getElementById('TypeBreak').checked;
                    if( !isEnabled ) {
                        return;
                    }
                    var theTable = document.getElementById('contentTypeTable');
                    var i;
                    for( i = 0; i < theTable.childNodes.length; i++ ) {
                        if( theTable.childNodes[i].id == 'row_' + '0' ) {
                            theTable.removeChild(theTable.childNodes[i]);
                        }
                    }
                    RemoveSelectEntry('addContentType', 0);
                    NormalizeDisplay('contentTypeTable', 'isEven');
                }

                    function OpenAddDialog()
                    {
                        window.open('template_config.aspx?view=add&folder_edit=1', '', 'resizable=1,scrollbars=1,width=700,height=500');
                    }

                function RemoveContentType(row_id, row_name)
                {
                    var isEnabled = !document.getElementById('TypeBreak').checked;
                    if( !isEnabled ) {
                        alert("<asp:Literal EnableViewState='false' runat='server' id='lbl_removeSmartFromWarn' />");
                        return;
                    }
                    var theTable = document.getElementById('contentTypeTable');
                    var i;
                    for( i = 0; i < theTable.childNodes.length; i++ ) {
                        if( theTable.childNodes[i].id == 'row_' + row_id ) {
                            if( confirm("<asp:Literal EnableViewState='false' runat='server' id='lbl_removeSmartFromAction' />") ) {
                                theTable.removeChild(theTable.childNodes[i]);
                                if( row_id != 0 ) {
                                    AddSelectEntry('addContentType', row_id, row_name);
                                }
                                else {
                                    var required = document.getElementById('requireSmartForms');
                                    required.checked = true;
                                }
                            }
                        }
                    }
                    NormalizeDisplay('contentTypeTable', 'isEven');
                    Ektron.Workarea.Grids.init();
                }

                function ActivateContentType(producttype)
                {
                    var selectList = document.getElementById('addContentType');
                    var xml_id = selectList.value;
                    var xml_name = selectList.options[selectList.selectedIndex].text;
                    // if zero, don't do anything - otherwise remove entry from select list
                    if( xml_id == -1 ) {
                        return;
                    }
                    else {
                        RemoveSelectEntry('addContentType', xml_id);
                    }

                    var isEven = document.getElementById('isEven');

                    var theTable = document.getElementById('contentTypeTable');
                    var newEntry = document.createElement('tr');

                    if (theTable.rows.length % 2 == 1)
                    {
                        $ektron(newEntry).attr("style", "background-color: #D0E5F5");
                    }
                    newEntry.id = 'row_' + xml_id;
                    newEntry.width = '100%';

                    var td_default = document.createElement('td');
                    td_default.className = 'center';
                    td_default.width = '10%';

                    var checkedByDefault = false;
                    checkedByDefault = (theTable.rows.length == 0);
                    var is_default = createElementWithName('input', 'sfdefault', 'radio');
                    is_default.id = 'sfdefault';
                    is_default.defaultChecked = checkedByDefault;
                    is_default.value = xml_id;

                    td_default.appendChild(is_default);

                    var td_left = document.createElement('td')
                    td_left.width = '70%';
                    td_left.appendChild(document.createTextNode(xml_name));

                    var input_left = document.createElement('input');
                    input_left.name = 'input_' + xml_id;
                    input_left.id = 'input_' + xml_id;
                    input_left.type = 'hidden';
                    input_left.value = xml_id;
                    td_left.appendChild(input_left);

                    var td_right = document.createElement('td');
                    td_right.className = 'center';
                    td_right.width = '10%';

                    var remove_link = document.createElement('a');
                    remove_link.href = 'javascript:RemoveContentType(' + xml_id + ',\'' + xml_name + '\')';
                    remove_link.appendChild(document.createTextNode("<asp:Literal EnableViewState='false' runat='server' id='ltr_SmartFormRemlbl' />"));
                    $ektron(remove_link).addClass("button redHover minHeight buttonRemove");

                    var td_preview = document.createElement('td');
                    td_preview.className = 'center';
                    td_preview.width = '10%';
                    if (xml_id != '0') {
                        var preview_link = document.createElement('a');
                        if (producttype != null && producttype == true)
                            preview_link.href = 'javascript:PreviewProductTypeByID(' + xml_id + ')';
                        else
                            preview_link.href = 'javascript:PreviewXmlConfigByID(' + xml_id + ')';
                        td_preview.appendChild(preview_link);
                        preview_link.appendChild(document.createTextNode("<asp:Literal EnableViewState='false' runat='server' id='ltr_SmartFormViewlbl' />"));
                        $ektron(preview_link).addClass("button greenHover minHeight buttonSearch");
                    }

                    td_right.appendChild(remove_link);
                    newEntry.appendChild(td_default);
                    newEntry.appendChild(td_left);
                    newEntry.appendChild(td_preview);
                    newEntry.appendChild(td_right);
                    theTable.appendChild(newEntry);
                    Ektron.Workarea.Grids.init();
                }

                function RemoveTemplate(row_id, row_name, url)
                {
                    var isEnabled = !document.getElementById('addTemplate').disabled;
                    if( !isEnabled ) {
                        alert("<asp:Literal EnableViewState='false' runat='server' id='lbl_removeTmpWarn' />");
                        return;
                    }

                    var theTable = document.getElementById('templateTable');
                    var i;
                    for( i = 0; i < theTable.childNodes.length; i++ ) {
                        if( theTable.childNodes[i].id == 'trow_' + row_id ) {
                            if( confirm("<asp:Literal EnableViewState='false' runat='server' id='lbl_removeTmpAction' />") ) {
                                theTable.removeChild(theTable.childNodes[i]);
                                if(typeof(url) == 'string' && url != ''){
                                    AddSelectEntry('addTemplate', row_id, row_name, url);
                                }else{
                                    AddSelectEntry('addTemplate', row_id, row_name);
                                }
                            }
                        }
                    }
                    NormalizeDisplay('templateTable', 'tisEven');
                    
                }

                function ActivateTemplate(sitePath)
                {
                    var selectList = document.getElementById('addTemplate');
                    var selectedtemplateurl = $ektron("#addTemplate :selected").attr("url");
                    if(typeof(selectedtemplateurl) != 'string') selectedtemplateurl = '';
                    var template_id = selectList.value;
                    var template_name = selectList.options[selectList.selectedIndex].text;
                    var jstemplate_name = template_name.replace(/\\/g, '\\\\');
                    // if zero, don't do anything - otherwise remove entry from select list
                    if( template_id == 0 || selectList.disabled == true) {
                        return;
                    }
                    else {
                        RemoveSelectEntry('addTemplate', template_id);
                    }

                    var theTable = document.getElementById('templateTable');

                    var enableThis = false;

                    if( theTable.innerHTML == '' ) {
                        enableThis = true;
                    }

                    var newEntry = document.createElement('tr');
                    newEntry.id = 'trow_' + template_id;
                    newEntry.width = '100%';

                    var td_left = document.createElement('td');
                    td_left.className = 'center';
                    td_left.width = '10%';

                    var is_default = createElementWithName('input', 'tdefault', 'radio');
                    is_default.id = 'tdefault';
                    is_default.value = template_name;

                    td_left.appendChild(is_default);

                    var td_middle = document.createElement('td')
                    td_middle.className = 'left';
                    td_middle.width = '70%';
                    td_middle.appendChild(document.createTextNode(template_name));

                    var input_middle = document.createElement('input');
                    input_middle.name = 'tinput_' + template_id;
                    input_middle.id = 'tinput_' + template_id;
                    input_middle.type = 'hidden';
                    input_middle.value = template_id;
                    td_middle.appendChild(input_middle);

                    var td_right = document.createElement('td');
                    td_right.className = 'center';
                    td_right.width = '10%';

                    var td_preview = document.createElement('td');
                    td_preview.className = 'center';
                    td_preview.width = '10%';

                    var remove_link = document.createElement('a');
                    remove_link.setAttribute('class', 'button redHover minHeight buttonRemove');
                    remove_link.className = 'button redHover minHeight buttonRemove';
                    
                    remove_link.href = 'javascript:RemoveTemplate(' + template_id + ',\'' + jstemplate_name + '\',\'' + selectedtemplateurl + '\')';

                    var preview_link = document.createElement('a');
                    preview_link.setAttribute('class','button greenHover minHeight buttonSearch');
                    preview_link.className = 'button greenHover minHeight buttonSearch';
                    
                    if (template_name.indexOf(' (') != -1)
                    {
                        template_name = template_name.split(' (')[0];
                    }
                    if(selectedtemplateurl != ''){
                        preview_link.href = 'javascript:PreviewSpecificTemplate(\'' + selectedtemplateurl + '\',800,600)';
                    }else{
                        preview_link.href = 'javascript:PreviewSpecificTemplate(\'' + sitePath + jstemplate_name.split(" (")[0] + '\',800,600)';
                    }
                    remove_link.appendChild(document.createTextNode("<asp:Literal EnableViewState='false' runat='server' id='ltr_TemplateRemLbl' />"));
                    preview_link.appendChild(document.createTextNode("<asp:Literal EnableViewState='false' runat='server' id='ltr_TemplateViewLbl' />"));
                    td_right.appendChild(remove_link);
                    td_preview.appendChild(preview_link);
                    newEntry.appendChild(td_left);
                    newEntry.appendChild(td_middle);
                    newEntry.appendChild(td_preview);
                    newEntry.appendChild(td_right);
                    theTable.appendChild(newEntry);
                    is_default.checked = enableThis;
                    Ektron.Workarea.Grids.init();
                }

                function fetchtaxonomyid(pid,arr){
                    for(var i=0;i<arr.length;i++){
                        if(arr[i]==pid){
                            return true;
                            break;
                        }
                    }
                    return false;
                }
               function ValidateCatSel(control){
                    if(document.getElementById("CategoryRequired").checked){
                         var element = document.forms[0].elements;
                         var len = element.length;
                         for (var i=0; i<len; i++) {
                            var item = element[i];
                            if(item.name=="taxlist" && item.type=='checkbox'){
                               if(item.checked) {
                                return true;break;
                               }
                            }
                        }
                        alert('At least one taxonomy must be selected when category selection is required.');
                        control.checked=true;
                        return false;
                    }
                }
                function ToggleTaxonomyInherit(control){
                    var element = document.forms[0].elements;
                    var len = element.length;
                    var catarr=taxonomytreearr;
                    if(control.checked){
                        for (var i=0; i<len; i++) {
                            if(element[i].name=="taxlist" && element[i].type=='checkbox')
                            element[i].checked=false;
                        }
                        catarr=taxonomyparenttreearr;
                        for (var i=0; i<len; i++) {
                            var item = element[i];
                            if(item.name=="taxlist" && item.type=='checkbox'){
                                if(fetchtaxonomyid(item.value,catarr)) {item.checked=true;}item.disabled=true;
                            }
                        }

                        CheckCatRequired(1);
                    }
                    else{
                        var ConfirmMsg="<asp:Literal EnableViewState='false' runat='server' id='ltrTaxConfirmBrkInherit' />";
                        if( confirm(ConfirmMsg) ) {
                            for (var i=0; i<len; i++) {
                                var item = element[i];
                                if(item.name=="taxlist" && item.type=='checkbox'){
                                item.checked=false;item.disabled=false;
                                }
                            }
                            for (var i=0; i<len; i++) {
                            if(element[i].name=="taxlist" && element[i].type=='checkbox')
                            element[i].checked=false;
                            }
                            for (var i=0; i<len; i++) {
                                var item = element[i];
                                if(item.name=="taxlist" && item.type=='checkbox'){
                                    if(fetchtaxonomyid(item.value,catarr)) {item.checked=true;}
                                }
                            }
                            CheckCatRequired(0)
                        }
                        else{
                            control.checked=true;CheckCatRequired(1);return false;
                        }
                    }
                }
                function CheckCatRequired(v){
                    var cr=document.getElementById("CategoryRequired");
                    if(cr){
                        if(v==1){ cr.disabled=true;
                        if(__jsparentcatrequired==1) cr.checked=true;
                        else cr.checked=false;
                        }
                        else {
                        cr.disabled=false;
                        if(__jscatrequired==1) cr.checked=true;
                        else cr.checked=false;
                        }
                    }
                }

                function ToggleProductTypesInherit(src, obj)
                {
                    if (ToggleMultiXmlTemplateInherit(src))
                        $ektron(".hiddenWhenInheriting").show();
                    else
                        $ektron(".hiddenWhenInheriting").hide();
                }

                function ToggleMultiXmlTemplateInherit(src)
                {
                    var result = false;
                    var contentTypes = document.getElementById('TypeBreak');
                    var templates = document.getElementById('TemplateTypeBreak');
                    var tdefault = document.getElementsByName('tdefault');
                    var sfdefault = document.getElementsByName('sfdefault');
                    var requireSmartForms = document.getElementById('requireSmartForms');
                    var i;

                    var isInheriting = false;

                    if( src == 'TypeBreak' ) {
                        if( contentTypes.checked ) {
                            isInheriting = true;
                        }
                        else {
                            isInheriting = false;
                        }
                    }
                    else {
                        if( templates.checked ) {
                            isInheriting = true;
                        }
                        else {
                            isInheriting = false;
                        }
                    }

                    var addTemplate = document.getElementById('addTemplate');
                    var addContentType = document.getElementById('addContentType');
                    var ConfimMsg="<asp:Literal EnableViewState='false' runat='server'  id='lit_template_inherit' />"
                    if( isInheriting == false ) {
                        if( confirm(ConfimMsg) ) {
                            result = true;
                            if( src == 'TypeBreak' ) {
                                contentTypes.checked = false;
                                addContentType.disabled = false;
                                requireSmartForms.disabled = false;
                                for (i = 0; i < sfdefault.length; i++ ) {
                                    sfdefault[i].disabled = false;
                                }
                            }
                            else {
                                templates.checked = false;
                                addTemplate.disabled = false;
                                for (i = 0; i < tdefault.length; i++ ) {
                                    tdefault[i].disabled = false;
                                }
                            }
                        }
                        else {
                            if( src == 'TypeBreak' ) {
                                contentTypes.checked = true;
                            }
                            else {
                                templates.checked = true;
                            }
                        }
                    }
                    else {
                        if( src == 'TypeBreak' ) {
                            addContentType.disabled = true;
                            for (i = 0; i < sfdefault.length; i++ ) {
                                sfdefault[i].disabled = true;
                            }
                            contentTypes.checked = true;
                            requireSmartForms.disabled = true;
                        }
                        else {
                            addTemplate.disabled = true;
                            for (i = 0; i < tdefault.length; i++ ) {
                                tdefault[i].disabled = true;
                            }
                            templates.checked = true;
                        }
                    }
                    return (result);
                }

                function FolderIsAProductCatalog(){
                    if ("undefined" != typeof isProductCatalog)
                        return isProductCatalog;
                    else
                        return false;
                }

                function ContentTypeConfigSave()
                {
                    var FolderID = "<asp:Literal EnableViewState='false' ID='litContentTypeFolderId' runat='Server'/>";
                    if( FolderID == 0 ) {
                        var contentTable = document.getElementById('contentTypeTable');
                        if( contentTable == null ) {
                            return true;
                        }

                        if( contentTable.innerHTML == '' ) {
                            if (FolderIsAProductCatalog())
                                alert('The root folder must specify at least one product type.');
                            else
                                alert('The root folder must specify at least one smart form.');
                            return false;
                        }
                    }
                    var contentTypes = document.getElementById('TypeBreak');

                    if( contentTypes == null ) {
                        return true;
                    }

                    if( !contentTypes.checked ) { // inheritence broken
                        var contentTable = document.getElementById('contentTypeTable');
                        if( contentTable.innerHTML == '' ) {
                                if (FolderIsAProductCatalog()) {
                                    alert('You must specify a default product type for this folder if you wish to break inheritance.');
                                    ShowPane('dvTypes');
                                } else
                                    alert('You must specify a default smart form for this folder if you wish to break inheritance.');
                                return false;
                        }
                    }

                    var defaultList = document.getElementsByName('sfdefault');
                    var isSelected = false;
                    var i;
                    for( i = 0; i < defaultList.length; i++ ) {
                        if( defaultList[i].checked ) {
                            isSelected = true;
                        }
                    }

                    var smartFormsRequired = document.getElementById('requireSmartForms').checked;
                    if( !isSelected && !smartFormsRequired ) {
                        for( i = 0; i < defaultList.length; i++ ) {
                            if( defaultList[i].value == 0 ) {
                                defaultList[i].checked = true;
                                isSelected = true;
                            }
                        }
                    }

                    if( !isSelected && !contentTypes.checked ) {
                            if (FolderIsAProductCatalog())
                                alert('You must select a default product type.');
                            else
                                alert('You must select a default smart form.');
                        return false;
                    }

                    return true;
                }

                function TemplateConfigSave()
                {
                    var FolderID = "<asp:Literal EnableViewState='false' ID='litTemplateConfigSaveFolderId' runat='Server'/>";

                    if( FolderID == 0 ) {
                        var templateTable = document.getElementById('templateTable');
                        if( templateTable.innerHTML == '' ) {
                            alert('The root folder must specify at least one template.');
                            return false;
                        }
                    }

                    var templateTypes = document.getElementById('TemplateTypeBreak');
                    if(!(templateTypes.type == "checkbox"))
                    { // user blog
                        var templateTable = document.getElementById('templateTable');
                        if( templateTable.innerHTML == '' ) {
                                alert('You must specify a default template for this journal.');
                                return false;
                        }
                    }
                    else if( !templateTypes.checked ) { // inheritence broken
                        var templateTable = document.getElementById('templateTable');
                        if( templateTable.innerHTML == '' ) {
                                alert('You must specify a default template for this folder if you wish to break inheritance.');
                                return false;
                        }
                    }

                    var defaultList = document.getElementsByName('tdefault');
                    var isSelected = false;
                    var i;
                    for( i = 0; i < defaultList.length; i++ ) {
                        if( defaultList[i].checked ) {
                            var templatefilename = document.getElementById('templatefilename');
                            templatefilename.value = defaultList[i].value;
                            isSelected = true;
                            
                            // need to POST the template ID, not just filename, as filename is no londer unique.
                            var selectedTemplateID = $ektron(defaultList[i]).parent().next().find("input").val();
                            $ektron('#defaultTemplateID').val(selectedTemplateID);
                        }

                    }

                    if( !isSelected && !templateTypes.checked ) {
                        alert('You must select a default template.');
                        return false;
                    }

                    return true;
                }
                    // END multitemplate/multixml functions

                    function TaxonomyConfigSave()
                    {
                         if (document.getElementById("CategoryRequired") !== null && document.getElementById("CategoryRequired").checked) {
                            var taxonomyItems = $ektron("input[name='taxlist']")
                            if (taxonomyItems !== null)
                            {
                                 for (var i=0; i<taxonomyItems.length; i++) {
                                   if(taxonomyItems[i].checked) {
                                      return true;
                                   }
                                }
                            }
                            return false;
                        }
                        return true;
                    }
            //--><!]]>
    </script>
    <script type="text/javascript">
            <!--        //--><![CDATA[//><!--
        function checkAll(bChecked) {
            for (var i = 0; i < document.forms.frmContent.elements.length; i++) {
                if (document.forms.frmContent.elements[i].type == "checkbox") {
                    document.forms.frmContent.elements[i].checked = bChecked;
                }
            }
        }

        function checkDeleteForm() {
            //document.forms.frmContent.action="content.aspx?LangType="+jsContentLanguage+"&action=submitMultiDelContAction";

            var alertNoSelect = '<asp:Literal id="ltr_DelAlert_NoSelect" runat="server" />';
            var alertDelete = '<asp:Literal id="ltr_DelAlert" runat="server" />';

            var bFound = false;
            var bRet;
            for (var i = 0; i < document.forms.frmContent.elements.length; i++) {
                if (document.forms.frmContent.elements[i].type == "checkbox") {
                    if (document.forms.frmContent.elements[i].checked) {
                        bFound = true;
                    }
                }
            }
            if (bFound == false) {
                alert(alertNoSelect);
                return false;
            }
            else {
                bRet = confirm(alertDelete);
                if (bRet) {
                    document.forms.frmContent.submit();
                }
            }
        }
        function checkAllFalse() {
            document.forms.frmContent.all.checked = false;
        }
        //--><!]]>
    </script>
    <script type="text/javascript">
            <!--        //--><![CDATA[//><!--
        // Client-side control of folder-level flagging options:

        function moveFlagsLeft() {
            var leftSel = document.getElementById("flagging_options_assigned");
            var rightSel = document.getElementById("flagging_options_available");
            moveSelectedFlags(rightSel, leftSel);
            // if this is the first item to be assigned, assume that it's the default:
            setDefaultFlag();
        }

        function moveFlagsRight() {
            var leftSel = document.getElementById("flagging_options_assigned");
            var rightSel = document.getElementById("flagging_options_available");
            moveSelectedFlags(leftSel, rightSel);
            updateHiddenFlags();
        }

        function moveSelectedFlags(src, dest) {
            if (src && dest) {
                for (var idx = 0; idx < src.length; idx++) {
                    if (src.options[idx].selected) {
                        ++dest.length;
                        dest.options[dest.length - 1].value = src.options[idx].value;
                        dest.options[dest.length - 1].text = removeDefaultFlagMsg(src.options[idx].text);
                    }
                }
                for (var idx = src.length - 1; idx >= 0; idx--) {
                    if (src.options[idx].selected) {
                        src.options[idx] = null;
                    }
                }
            }
        }

        function updateHiddenFlags() {
            document.getElementById("flagging_options_hdn").value = "";
            var assignedSel = document.getElementById("flagging_options_assigned");
            for (var idx = 0; idx < assignedSel.length; idx++) {
                addHiddenFlag(assignedSel[idx].value);
            }
        }

        function addHiddenFlag(flag) {
            var hdn = document.getElementById("flagging_options_hdn");
            if (hdn.value.length > 0) {
                hdn.value += "," + flag;
            }
            else {
                hdn.value = flag;
            }
        }

        function setDefaultFlag() {
            var foundDefault = false;
            var defaultIdx = -1;
            var leftSel = document.getElementById("flagging_options_assigned");
            var hdnDef = document.getElementById("flagging_options_default_hdn");
            hdnDef.value = "";
            // search for a selected flag in the assigned-flags listing, to determine which one
            // the user wants to be default (if multiple selected, assume first is desired one):
            for (var idx = 0; idx < leftSel.length; idx++) {
                leftSel.options[idx].text = removeDefaultFlagMsg(leftSel.options[idx].text);
                if (leftSel.options[idx].selected) {
                    if (foundDefault) {
                        leftSel.options[idx].selected = false;
                    }
                    else {
                        hdnDef.value = leftSel.options[idx].value;
                        leftSel.options[idx].text = addDefaultFlagMsg(leftSel.options[idx].text);
                        leftSel.options[idx].selected = false;
                        foundDefault = true;
                        defaultIdx = idx;
                    }
                }
            }
            // If no flag identified, and there's only one in the assigned list, then assume it's default.
            if ((!foundDefault) && (leftSel.length > 0)) {
                hdnDef.value = leftSel.options[0].value;
                leftSel.options[0].text = addDefaultFlagMsg(leftSel.options[0].text);
                foundDefault = true;
                defaultIdx = 0;
            }
            // Ensure that the default is at the top of the list (API doesn't support
            // saving & reading the default flag - so we'll use the first one as default):
            if (foundDefault && (defaultIdx > 0)) {
                // swap default flag with first flag (making the default first):
                var flagText = leftSel.options[0].text;
                var flagValue = leftSel.options[0].value;
                //
                leftSel.options[0].text = leftSel.options[defaultIdx].text;
                leftSel.options[0].value = leftSel.options[defaultIdx].value;
                //
                leftSel.options[defaultIdx].text = flagText;
                leftSel.options[defaultIdx].value = flagValue;
            }
            // Update the order of the flagging definition IDs in the hidden-field,
            // the default must be first to compensate for API not allowing read of
            // default flagging ID (so we'll make the first item the default one):
            updateHiddenFlags();
        }

        var DefaultFlagMsg = " (default)";
        function addDefaultFlagMsg(inText) {
            return (inText + DefaultFlagMsg);
        }
        function removeDefaultFlagMsg(inText) {
            var result = inText;
            var txtIdx = inText.indexOf(DefaultFlagMsg);
            if (txtIdx >= 0) {
                result = result.substring(0, txtIdx);
            }
            return (result);
        }

        function InheritFlagingChanged(name) {
            var cbx = document.getElementById("flagging_options_inherit_cbx");
            var __ddflags = document.getElementById(name);
            if (cbx.checked) {
                if (cbx.checked) {
                    __ddflags.disabled = "disabled";
                }
                else {
                    __ddflags.disabled = "";
                }
            } else {
                if (confirm("<asp:Literal EnableViewState='false' runat='server' id='litConfirmBreakInheritanceFlagging' />")) {
                    if (cbx.checked) {
                        __ddflags.disabled = "disabled";
                    }
                    else {
                        __ddflags.disabled = "";
                    }
                } else {
                    cbx.checked = !cbx.checked;
                    return false;
                }
            }
        }

        function InheritAliasedChanged(name) {
            var cbx = document.getElementById("chkInheritAliases");
            var __ddforcealias = document.getElementById(name);
            if (cbx.checked) {
                if (cbx.checked) {
                    __ddforcealias.disabled = "disabled";
                    __ddforcealias.checked = false;
                }
                else {
                    __ddforcealias.disabled = "";
                }
            }
            else {
                if (confirm("<asp:Literal EnableViewState='false' runat='server' id='litConfirmBreakInheritanceAliasing' />")) {
                    if (cbx.checked) {
                        __ddforcealias.disabled = "disabled";
                    }
                    else {
                        __ddforcealias.disabled = "";
                    }
                } else {
                    cbx.checked = !cbx.checked;
                    return false;
                }
            }
        }
        //------------------IscontentSearchable----------------
        function InheritIscontentSearchableChanged(name) {
            var ist = document.getElementById("chkInheritIscontentSearchable");
            var __ddIscontSearchable = document.getElementById(name);
            if (ist.checked) {
                if (ist.checked) {
                    __ddIscontSearchable.disabled = "disabled";
                    __ddIscontSearchable.checked = false;
                }
                else {
                    __ddIscontSearchable.disabled = "";
                }
            }
            else {
                if (confirm("<asp:Literal EnableViewState='false' runat='server' id='litConfirmBreakInheritIsContentSearchable' />")) {
                    if (ist.checked) {
                        __ddIscontSearchable.disabled = "disabled";
                    }
                    else {
                        __ddIscontSearchable.disabled = "";
                    }
                } else {
                    ist.checked = !ist.checked;
                    return false;
                }
            }
        }
        //----------------IscontentSearchableEnd-----------
        //-----------------DisplaySettings-----------------
        function InheritIsDisplaySettingsChanged(name) {
            var ist1 = document.getElementById("chkInheritIsDisplaySettings");
            var __ddIsDisplaySettings = document.getElementById(name);
            var __ddIsDisplaySettings0 = document.getElementById("chkIsDisplaySettingsSummary");            
            var __ddIsDisplaySettings1 = document.getElementById("chkIsDisplaySettingsMetaData");
            var __ddIsDisplaySettings2 = document.getElementById("chkIsDisplaySettingsAliasing");
            var __ddIsDisplaySettings3 = document.getElementById("chkIsDisplaySettingsSchedule");
            var __ddIsDisplaySettings4 = document.getElementById("chkIsDisplaySettingsComment");
            var __ddIsDisplaySettings5 = document.getElementById("chkIsDisplaySettingsTemplates");
            var __ddIsDisplaySettings6 = document.getElementById("chkIsDisplaySettingsTaxonomy");
            if (ist1.checked) {
                if (ist1.checked) {
                    __ddIsDisplaySettings.disabled = "disabled";
                    __ddIsDisplaySettings.checked = false;
                    __ddIsDisplaySettings0.disabled = "disabled";
                    __ddIsDisplaySettings0.checked = false;
                    __ddIsDisplaySettings1.disabled = "disabled";
                    __ddIsDisplaySettings1.checked = false;
                    __ddIsDisplaySettings2.disabled = "disabled";
                    __ddIsDisplaySettings2.checked = false;
                    __ddIsDisplaySettings3.disabled = "disabled";
                    __ddIsDisplaySettings3.checked = false;
                    __ddIsDisplaySettings4.disabled = "disabled";
                    __ddIsDisplaySettings4.checked = false;
                    __ddIsDisplaySettings5.disabled = "disabled";
                    __ddIsDisplaySettings5.checked = false;
                    __ddIsDisplaySettings6.disabled = "disabled";
                    __ddIsDisplaySettings6.checked = false;
                }
                else {
                    __ddIsDisplaySettings.disabled = "";
                    __ddIsDisplaySettings0.disabled = "";
                    __ddIsDisplaySettings1.disabled = "";
                    __ddIsDisplaySettings2.disabled = "";
                    __ddIsDisplaySettings3.disabled = "";
                    __ddIsDisplaySettings4.disabled = "";
                    __ddIsDisplaySettings5.disabled = "";
                    __ddIsDisplaySettings6.disabled = "";
                }
            }
            else {
                if (confirm("<asp:Literal EnableViewState='false' runat='server' id='litConfirmBreakInheritIsDisplaySettings' />")) {
                    if (ist1.checked) {
                        __ddIsDisplaySettings.disabled = "disabled";
                        __ddIsDisplaySettings0.disabled = "disabled";
                        __ddIsDisplaySettings1.disabled = "disabled";
                        __ddIsDisplaySettings2.disabled = "disabled";
                        __ddIsDisplaySettings3.disabled = "disabled";
                        __ddIsDisplaySettings4.disabled = "disabled";
                        __ddIsDisplaySettings5.disabled = "disabled";
                        __ddIsDisplaySettings6.disabled = "disabled";
                    }
                    else {
                        __ddIsDisplaySettings.disabled = "";
                        __ddIsDisplaySettings0.disabled = "";
                        __ddIsDisplaySettings1.disabled = "";
                        __ddIsDisplaySettings2.disabled = "";
                        __ddIsDisplaySettings3.disabled = "";
                        __ddIsDisplaySettings4.disabled = "";
                        __ddIsDisplaySettings5.disabled = "";
                        __ddIsDisplaySettings6.disabled = "";
                    }
                } else {
                    ist1.checked = !ist1.checked;
                    return false;
                }
            }
        }
        //---------DisplaySettingsEnd-------

        $ektron.addLoadEvent(function () {
            Ektron.ContextMenus.AppPath = "<asp:Literal EnableViewState='false' id='jsAppPath' runat='server' />";
            Ektron.ContentContextMenu.IsFolderAdmin = "<asp:Literal EnableViewState='false' id='jsIsFolderAdmin' runat='server' />";
            Ektron.ContentContextMenu.ConfirmDelete = "<asp:Literal EnableViewState='false' id='jsConfirmDelete' runat='server' />";

        });
        //--><!]]>
    </script>
</head>
<body onclick="MenuUtil.hide()">
    <!-- ProgressBar UI for Cut/Copy functionality -->
    <div id="progressbar" class="ektronWindow ui-progressbar ui-widget ui-widget-content ui-corner-all">
        <div class="ui-progressbar-value ui-widget-header-progressbar ui-corner-left" style="width: 100%;">
        </div>
    </div>
    <!-- End -->
    <!-- Content Context Menu -->
    <ul id="contentContextMenu" class="ektronContextMenu" oncontextmenu="return false;">
        <li class="cut"><a href="#cutContent" title="Cut">
            <asp:Literal ID="contentContextCutContent" runat="server" /></a> </li>
        <li class="copy"><a href="#copyContent" title="Copy">
            <asp:Literal ID="contentContextCopyContent" runat="server" /></a> </li>
        <li class="separator"></li>
        <li class="assignItems"><a href="#assignItems" title="Assign Items to Taxonomy">
            <asp:Literal ID="contentContextAssignContentToTaxonomy" runat="server" /></a>
        </li>
        <li class="assignItems"><a href="#assignItemsToCollection" title="Add items to Collection">
            <asp:Literal ID="contentContextAssignContentToCollection" runat="server" /></a>
        </li>
        <li class="assignItems"><a href="#assignItemsToMenu" title="Add Items to Menu">
            <asp:Literal ID="contentContextAssignContentToMenu" runat="server" /></a> </li>
        <li class="delete"><a href="#deleteContent" title="Delete Synchronization Profile">
            <asp:Literal ID="contentContextDeleteContent" runat="server" /></a> </li>
    </ul>
    <asp:Literal ID="ltrEmailAreaJs" EnableViewState="false" runat="server" />
    <!-- Modal Dialog: Confirm -->
    <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
        id="ConfirmDialog" style="display: none;">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix  ektronModalHeader">
            <h3 class="ui-dialog-title header">
                <span class="headerText"></span>
                <asp:HyperLink ToolTip="Close" ID="closeDialogLink" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose"
                    runat="server" />
            </h3>
        </div>
        <div class="ektronModalBody">
            <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                <p class="messages">
                </p>
            </div>
            <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                <li>
                    <asp:HyperLink ToolTip="Cancel" ID="btnConfirmCancel" runat="server" CssClass="redHover button cancelButton buttonRight" /></li>
                <li>
                    <asp:HyperLink ToolTip="Ok" ID="btnConfirmOk" runat="server" CssClass="greenHover button okButton buttonRight" /></li>
            </ul>
        </div>
    </div>
    <!-- Modal Dialog: Sync Status -->
    <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
        id="SyncStatusModal" style="display: none;">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
            <h3 class="ui-dialog-title header">
                <span class="headerText">
                    <asp:Literal ID="lblSyncStatus" runat="server" /></span>
                <asp:HyperLink ToolTip="Close" ID="closeDialogLink2" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose"
                    runat="server" />
            </h3>
        </div>
        <div class="ektronModalBody">
            <div class="ui-dialog-content ui-widget-content ektronPageInfo">
                <p class="messages">
                </p>
                <div class="syncStatusMessages">
                </div>
            </div>
            <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                <li>
                    <asp:HyperLink ToolTip="Close Sync" ID="btnCloseSyncStatus" runat="server" CssClass="redHover button buttonNoIcon buttonRight" /></li>
            </ul>
        </div>
    </div>
    <!-- Modal Dialog: Show Sync Configuratiosn Modal -->
    <div class="ektronWindow ektronSyncModal ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all"
        id="ShowSyncConfigModal" style="display: none;">
        <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
            <h3 class="ui-dialog-title header">
                <span class="headerText"></span>
                <asp:HyperLink ToolTip="Close" ID="closeDialogLink3" CssClass="ui-dialog-titlebar-close ui-corner-all ektronModalClose"
                    runat="server" />
            </h3>
        </div>
        <div class="ektronModalBody">
            <div class="ui-dialog-content ui-widget-content">
                <p class="messages">
                </p>
                <ul class="server" id="configurations">
                </ul>
                <select id="selectConfigs" size="7">
                </select>
            </div>
            <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
                <li>
                    <asp:HyperLink ToolTip="Close" ID="btnCloseConfigDialog" runat="server" CssClass="button buttonNoIcon buttonRight redHover" /></li>
                <li>
                    <asp:HyperLink ToolTip="Start Sync" ID="btnStartSync" runat="server" CssClass="greenHover button performSyncButton buttonRight"
                        onclick="Ektron.Workarea.Sync.Relationships.StartContentFolderSync(); return false;" /></li>
            </ul>
        </div>
    </div>
    <%-- Mutli Language Dialog Box asking user to copy in one or all languages the content is available.--%>
    <div class="ektronWindow ektronModalStandard" id="selectMultiLingual">
        <div class="ektronModalHeader">
            <h3>
                <span class="addPageTitle">
                    <asp:Literal runat="Server" ID="selMultLangOption" /></span><a class="ektronModalClose"
                        onclick="unBlockFrames();"></a>
            </h3>
        </div>
        <div style="padding-left: 10px;">
            <asp:Label runat="Server" ID="jsConfirmMultiLingual" />
        </div>
        <ul class="buttonWrapper ui-helper-clearfix floatLeft" style="width: 75%; padding-right: 15px;">
            <li class="floatLeft"><a class="button redHover buttonClear" title="Aborts the Task."
                style="display: block;" onclick="unBlockFrames();"><span>
                    <asp:Literal runat="server" ID="cancelButton" /></span> </a></li>
            <li class="floatLeft"><a class="button greenHover buttonSelected" title="Pastes Content in Only Language selected."
                style="display: block;" onclick="PasteSelectedContent(this, false);"><span>
                    <asp:Literal runat="server" ID="selectedLanguages" /></span> </a></li>
            <li class="floatLeft"><a class="button greenHover buttonCheckAll" title="Pastes Content in Available Languages the Content is in."
                style="display: block;" onclick="PasteSelectedContent(this, true);"><span>
                    <asp:Literal runat="server" ID="availableLanguages" /></span> </a></li>
        </ul>
    </div>
    <%-- End Modal Mark Up --%>
    <%-- Confirmation modal Dialog Box asking user wheather to move content or opt out. --%>
    <div class="ektronWindow ektronModalStandard" id="moveContentModal">
        <div class="ektronModalHeader">
            <h3>
                <span class="addPageTitle">
                    <asp:Literal runat="Server" ID="moveContentConfirmTitle" /></span><a class="ektronModalClose"
                        onclick="unBlockFrames();"></a>
            </h3>
        </div>
        <div style="padding-left: 10px;">
            <asp:Label runat="Server" ID="lblConfirmMoveContent" />
        </div>
        <ul class="buttonWrapper ui-helper-clearfix floatLeft" style="width: 75%; padding-right: 15px;">
            <li class="floatLeft"><a class="button redHover buttonClear" title="Aborts the Task."
                style="display: block;" onclick="unBlockFrames();"><span>
                    <asp:Literal runat="server" ID="moveCancelButton" /></span> </a></li>
            <li class="floatLeft"><a class="button greenHover buttonOk" title="Continue" style="display: block;"
                onclick="PasteSelectedContent(this, false);"><span>
                    <asp:Literal runat="server" ID="ltrOK" /></span> </a></li>
        </ul>
    </div>
    <%-- End Modal Mark Up --%>
    <form id="frmContent" method="post" runat="server">
    <input onkeypress="return CheckKeyValue(event,'34');" type="hidden" name="netscape" />
    <asp:PlaceHolder ID="DataHolder" runat="server"></asp:PlaceHolder>
    <input type="hidden" id="targetId" name="targetId" value="" />
    <input type="hidden" id="parentId" name="parentId" value="" />
    </form>
</body>
</html>

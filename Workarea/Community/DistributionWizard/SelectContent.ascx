<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectContent.ascx.cs" Inherits="Community_DistributeCommunityDocuments_SelectContent" %>
<%@ Register Src="SelectFolder.ascx" TagName="SelectFolder" TagPrefix="uc1" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<style type="text/css">
    .selectContentTableCell {vertical-align: top; padding: 0px; margin: 0px; border: 0px;}
    .selectContentListContainer {border: solid 1px #333333; width: 100%;}
    .selectContentFolderList {height: 321px; width: 100%; border-right: solid 1px #bebebe; padding: 0px; margin: 0px; overflow: scroll;}
    .selectContentFolderList ul {list-style-type: none; padding-left: 5px; margin-left: 5px;}
    .selectContentFolderList li {width: 100%;}
    .selectContentContentHeader {height: 30px; width: 100%; background: url('<%=SitePath %>Workarea/images/application/darkblue_gradiant-nm.gif'); vertical-align: middle;}
    .selectContentContentHeaderText {color: #ffffff; display:block; font-weight: bold; font-size: 13px; padding-left: 4px; padding-top: 7px;}
    .selectContentContentListHeader {border-bottom: solid 1px #bebebe; background-color: #ece9d8; font-size: 12px; color: #000000; padding-top: 3px; padding-bottom: 3px; padding-left: 7px;}
    .selectContentSelectedContentItem {background-color: #0000FF; color: #ffffff}
    #selectContentFolderListContainer {width: 40%;}
    #selectContentContentListContainer {width:60%;}
    #selectContentContentList {height: 300px; width: 100%; padding: 0px; margin: 0px; overflow: scroll;}
    #selectContentContentList ul {list-style-type: none; padding-left: 5px; margin-left: 0px; margin-top: 0px; height:99%;}
    #selectContentContentList ul li {white-space: nowrap; font-size: 12px; background-position: 0 center; background-repeat: no-repeat; padding: 0 0 0 20px; line-height: 1.5em;}
    ul li a {font-size: 12px; color: #000000; white-space:nowrap; text-decoration: none;}
    ul li.selectContentRelatedContentItem a {color: #999999 !important;}
    ul li a:hover {text-decoration: underline;}
    ul li a img {padding-right: 4px;}
    ul li img {padding-right: 4px;}

    /* content types */
    #selectContentContentList .file {background-image: url('../../images/ui/icons/filetypes/file.png')}
    /*  MS Office Files Types */
    .doc, .dot,.docx,.dotx,.docm,.dotm
        {background-image: url('../../images/ui/icons/filetypes/word.png') !important;}
    .xla, .xlc,.xlm,.xls,.xlt,.xlw,.xlax,.xlcx,.xlmx,.xlsx,.xltx,.xlwx,.xlam,.xlcm,.xlmm,.xlsm,.xltm,.xlwm
        {background-image: url('../../images/ui/icons/filetypes/excel.png') !important;}
    .ppt,.pot,.pps,.pptx,.potx,.ppsx,.pptm,.potm,.ppsm
        {background-image: url('../../images/ui/icons/filetypes/powerpoint.png') !important;}
    .mpp,.mppx
        {background-image: url('../../images/ui/icons/filetypes/project.png') !important;}
    .pub,.pubx
        {background-image: url('../../images/ui/icons/filetypes/publisher.png') !important;}
    .vsd,.vsdx
        {background-image: url('../../images/ui/icons/filetypes/visio.png') !important;}
    .mdb, .mdbx
        {background-image: url('../../images/ui/icons/filetypes/acess.png') !important;}
    /* Open Office File Types */
    .odt,.ott
        {background-image: url('../../images/application/ooo-writer-doc.gif') !important;}
    .oth
        {background-image: url('../../images/application/ooo-web-doc.gif') !important;}
    .odm
        {background-image: url('../../images/application/ooo-master-doc.gif') !important;}
    .odg,.otg
        {background-image: url('../../images/application/ooo-image-doc.gif') !important;}
    .odp, .otp":
        {background-image: url('../../images/application/ooo-impress-doc.gif') !important;}
    .ods,.ots
        {background-image: url('../../images/application/ooo-calc-doc.gif') !important;}
    .odc
        {background-image: url('../../images/application/ooo-chart-doc.gif') !important;}
    .odf
        {background-image: url('../../images/application/ooo-math-doc.gif') !important;}
    .odb
        {background-image: url('../../images/application/ooo-empty-doc.gif') !important;}
    .odi
        {background-image: url('../../images/application/ooo-draw-doc.gif') !important;}

    /* Adobe File Types */
    .pdf
        {background-image: url('../../images/ui/icons/filetypes/acrobat.png') !important;}
    .swf
        {background-image: url('../../images/ui/icons/filetypes/flash.png') !important;}
    /* Generic File Types */
    .txt, .asr
        {background-image: url('../../images/ui/icons/filetypes/text.png') !important;}
    .htm, .html, .htt, .mhtml
        {background-image: url('../../images/ui/icons/contentHtml.png') !important;}
    .bmp,.gif,.jpg,.jpeg,.pbm,.cmx,.ico,.cod,.ief,.jfif,.jpe,.pgm,.pnm,.ppm,.png,.ras,.rgb,.tif,.tiff,.xpm,.xbm,.xwd
        {background-image: url('../../images/ui/icons/filetypes/image.png') !important;}
    .avi,.wmv,.mov,.mpa,.mpe,.mpeg,.mpg,.mpv2,.wm,.wvx,.wmx,.wmz,.wmd,.wax,.wma
        {background-image: url('../../images/ui/icons/filetypes/video.png') !important;}
    .au,.snd,.mid,.mp3,.aif,.aifc,.aiff,.wav,.mp2,.asf,.mp4,.m3u,.asx,.lsf,.lsx,.rm
        {background-image: url('../../images/ui/icons/filetypes/audio.png') !important;}
    .qt
        {background-image: url('../../images/ui/icons/filetypes/quicktime.png') !important;}
     .rmi,.ra,.ram
        {background-image: url('../../images/ui/icons/filetypes/realplayer.png') !important;}
    /* Compressed Files */
    .gz,.zip
        {background-image: url('../../images/ui/icons/filetypes/zip.png') !important;}
    </style>

<input id="inputSelectedContentID" runat="server" type="hidden" />

<script type="text/javascript" language="javascript">
    var selectedFolderID = 0;
    var selectedContentID = 0;
    var selectedContentIDHiddenField = "<%= SelectedContentIDHiddenFieldName %>";
    var enableMultiSelect = "<%= EnableMultiSelect %>";

    // Highlights a specific content item. Only one piece of
    // content may be highlighted at a time -- any previously
    // highlighted items will no longer be so.
    function highlightContentItem(contentID)
    {
        var control = document.getElementById("selectContentContentList");
        var contentItemLinks = control.getElementsByTagName("a");
        for(i=0; i<contentItemLinks.length; i++)
        {
            if( contentItemLinks[i].id == "ContentItem_" + contentID )
            {
                contentItemLinks[i].className = "selectContentSelectedContentItem";
            }
            else
            {
                contentItemLinks[i].className = "";
            }
        }
    }

    // Callback to populate the folder name with the data
    // returned upon selection of a folder.
    function displayFolderNameCallback(folderName)
    {
        var control = document.getElementById("contentHeaderText");
        if(control != null)
        {
            control.innerHTML = folderName;
        }
    }

    // Callback to populate the content list with the data
    // returned upon selection of a folder.
    function displayContentCallback(content)
    {
        var control = document.getElementById("selectContentContentList");
        if(control != null)
        {
            control.innerHTML = content;
        }

        highlightContentItem(selectedContentID);
        checkSelectedContent();
    }

    // Due to access restrictions related to crossing domains with
    // XMLHttpRequests, dynamically generated javascript is referenced
    // and executed in its place.
    function ExecScript(url, scriptId)
    {
        if(!document.createElement) return null;

        element = document.createElement("script");

        if(!element) return null;

        element.src = url + "&cleanupid=" + scriptId;
        element.id = scriptId;
        element.type = "text/javascript";

        var head = document.getElementsByTagName("head")[0];

        if(!head) return null;

        head.appendChild(element);

        return element;
    }

    // Removes script element that was added dynamically
    // via ExecScript.
    function cleanUp(id)
    {
        var item = document.getElementById(id);

        if(!item) return null;

        var head = document.getElementsByTagName("head")[0];

        if(!head) return null;

        head.removeChild(item);
    }

    // Updates the content list to reflect that of the specified folder.
    // Content will be filtered according to the AssetExtension and
    // ContentTypeID properties of this control.
    function updateContent(folderID)
    {
        var control = document.getElementById("selectContentContentList");
        if(control != null)
        {
            control.innerHTML = "<div class=\"ContentLoadingMessage\">Loading...</div>";
        }

        var url = "<%=SitePath %>Workarea/Community/DistributionWizard/FolderData.aspx?Request=Content" +
            "&FolderID=" + folderID +
            "&AssetExtension=<%= AssetExtension %>" +
            "&CleanUpID=ekScriptUpdateContent" +
            "&ContentTypeID=<%= ContentTypeID %>" +
            "&ShowRelated=<%= ShowRelatedContent %>" +
            "&CheckEditPermissions=<%= CheckEditPermissions %>" +
            "&LanguageID=<%= LanguageID %>" +
            "&IsDistWiz=<%= IsDistributionWizard %>" +
            "&EnableMulti=<%= EnableMultiSelect %>";

        ExecScript(url,"ekScriptUpdateContent");
    }

    // Updates the folder name to reflect that of the specified folder.
    function updateFolderName(folderID)
    {
        var control = document.getElementById("selectContentContentList");
        if(control != null)
        {
            control.innerHTML = "<div class=\"ContentLoadingMessage\">Loading...</div>";
        }

        var url = "<%=SitePath %>Workarea/Community/DistributionWizard/FolderData.aspx?Request=Name" +
            "&FolderID=" + folderID +
            "&CleanUpID=ekScriptUpdateName";

        ExecScript(url, "ekScriptUpdateName");
    }

    // Marks the specified piece of content as selected. Content ID
    // is stored in a hidden field visible to the control upon
    // postback.
    function selectContent(contentID)
    {
        selectedContentID = contentID;

        var hiddenField = document.getElementById(selectedContentIDHiddenField);
        if( hiddenField != null )
        {
            hiddenField.value = selectedContentID;
            highlightContentItem(selectedContentID);
        }
    }

    // Marks the specified piece of content as selected. Content ID
    // is stored in a hidden field visible to the control upon
    // postback.
    function selectMultiContent(contentID, bCheckBox)
    {
        selectedContentID = contentID;
        var chkItem = document.getElementById("ChkContentItem_" + contentID);
        if (!bCheckBox && chkItem.checked)
        {
            chkItem.checked = false;
        }
        if (chkItem.checked)
        {
            var hiddenField = document.getElementById(selectedContentIDHiddenField);
            if( hiddenField != null )
            {
                hiddenField.value += ",ContentItem_" + selectedContentID + ",";
            }
        }
        else
        {
            var hiddenField = document.getElementById(selectedContentIDHiddenField);
            if( hiddenField != null )
            {
                hiddenField.value = hiddenField.value.replace(",ContentItem_" + selectedContentID + ",", "");
            }
        }

    }

    function checkSelectedContent()
    {
        if (!enableMultiSelect)
        {
            return false;
        }
        var hiddenField = document.getElementById(selectedContentIDHiddenField);
        var control = document.getElementById("selectContentContentList");
        var contentItemLinks = control.getElementsByTagName("a");

        for(i=0; i<contentItemLinks.length; i++)
        {
            if( hiddenField.value.indexOf("," + contentItemLinks[i].id + ",") != -1)
            {
                document.getElementById("Chk" + contentItemLinks[i].id).checked = true;
            }
        }
    }

    // Selects a folder and display its contents.
    function selectFolder(folderID)
    {
        selectedFolderID = folderID;

        updateContent(folderID);
        updateFolderName(folderID);
    }
</script>

<div class="selectContentListContainer">
    <div class="selectContentContentHeader"><div class="selectContentContentHeaderText">Selected Folder: <span id="contentHeaderText"></span></div></div>
    <table style="border-collapse: collapse; padding: 0px; margin: 0px; width:100%;">
        <tr>
            <td id="selectContentFolderListContainer" class="selectContentTableCell">
                <div class="selectContentFolderList">
                    <uc1:SelectFolder ID="cmsSelectFolder" runat="server" />
                </div>
            </td>
            <td id="selectContentContentListContainer" class="selectContentTableCell">
                <div class="selectContentContentListHeader">Title</div>
                <div id="selectContentContentList">
                </div>
            </td>
        </tr>
    </table>
</div>
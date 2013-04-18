<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="resourceselectorfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ResourceSelectorField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldMinMax" Src="ucFieldMinMax.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Resource Selector Field</title>
<style type="text/css">
img
{
    border-style: none;
    vertical-align: bottom; 
}
</style>
</head>
<body class="dialog">
<form id="Form1" runat="server">
    
    <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
			OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
        <Tabs>
            <radTS:Tab ID="General" Text="General" Value="General" />
            <radTS:Tab ID="ResourceType" Text="Content Type" Value="ResourceType" />
            <radTS:Tab ID="Configuration" Text="Configuration" Value="Configuration" />
            <radTS:Tab ID="Appearance" Text="Appearance" Value="Appearance" />
        </Tabs>
    </radTS:RadTabStrip>  
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
	            <table width="100%">
		            <ek:FieldNameControl ID="name" runat="server" IndexedEnabled="true" />
		            <tr>
		                <td><label title="Resource Type" for="resourceType" class="Ektron_StandardLabel" id="lblResType" runat="server">Resource Type:</label></td>
		                <td colspan="2">
		                    <select title="Select the Resource Type from the Drop Down Menu" id="resourceType" onchange="updateFilterByOption();">
		                        <option title="Content Resource" id="resContent" runat="server" value="content" selected="selected">Content Resource</option>
		                        <option title="Folder Resource" id="resFolder" runat="server" value="folder">Folder Resource</option>
		                        <option title="Taxonomy Resource" id="resTaxonomy" runat="server" value="taxonomy">Taxonomy Resource</option>
		                        <option title="Collection Resource" id="resCollection" runat="server" value="collection">Collection Resource</option>
		                    </select>
		                </td>
		            </tr>
		            <tr>
		                <td><label title="Default value" for="txtDefVal" class="Ektron_StandardLabel" id="lblDefVal" runat="server">Default value:</label></td>
		                <td colspan="2">
		                    <asp:HiddenField runat="server" ID="txtId" Value="" />
		                    <asp:HiddenField runat="server" ID="txtIdType" Value="" />
		                    <asp:HiddenField runat="server" ID="txtTitle" Value="" />
		                    <input title="Enter Default value here" type="text" name="txtDefVal" id="txtDefVal" readonly="readonly" class="Ektron_StandardTextBox Ektron_ReadOnlyBox" />&#160;
		                    <a title="Select Content" href="#" id="linkSelectContentPopup" class="design_fieldbutton" onclick="popupSelector(document.getElementById('txtIdType').value);" runat="server"><img class="design_fieldbutton" id="imgSelectContentPopup" title="Select Content" alt="Select Content" runat="server" /></a>
		                    &#160;&#160;
		                    <a title="Delete" href="#" id="linkDeleteDefault" onclick="clearSelectorValue('content');" runat="server"><img id="imgDeleteDefault" runat="server" title="Delete" alt="Delete" class="design_fieldbutton" /></a>
		                </td>
		            </tr>
		            <tr>
		                <td colspan="3">
                        <fieldset id="AllowContainer">
		                    <legend id="lblAllow" runat="server" title="Allow">Allow</legend>
		                    <div class="Ektron_TopSpaceVeryVerySmall">
                                <table>
									<ek:FieldMinMax runat="server" />
                                </table>
		                    </div>
	                    </fieldset>
		                </td>
		            </tr>
		        </table>
	        </radTS:PageView>
	        <radTS:PageView id="Pageview2" runat="server">
	            <fieldset>
					<legend id="lblFilterBy" runat="server" title="Let content author find this type of resource">Let content author find this type of resource:</legend>
                    <input title="HTML Content" id="chkContent" name="filterBy" type="checkbox" checked="checked" /><label title="HTML Content" id="lblContent" for="chkContent" runat="server">HTML Content</label><br />
                    <input title="<%=GetMessage("lbl smart form") %> Option" id="chkSmartForm" name="filterBy" type="checkbox" checked="checked" /><label id="lblSmartForm" title="Smart Form" for="chkSmartForm" runat="server">Smart Form</label><br />
                    <input title="<%=GetMessage("lbl html form") %> Option" id="chkForm" name="filterBy" type="checkbox" checked="checked" /><label id="lblHtmlForm" title="HTML Form" for="chkForm" runat="server">HTML Form</label><br />
                    <input title="<%=GetMessage("lbl microsoft asset") %> Option" id="chkMSAsset" name="filterBy" type="checkbox" checked="checked" /><label id="lblMSAsset" title="Microsoft Asset" for="chkMSAsset" runat="server">Microsoft Asset</label><br />
                    <input title="<%=GetMessage("lbl other asset") %> Option" id="chkOtherAsset" name="filterBy" type="checkbox" checked="checked" /><label id="lblOtherAsset" title="Other Asset" for="chkOtherAsset" runat="server">Other Asset</label><br />
                    <input title="<%=GetMessage("lbl video") %> Option" id="chkMedia" name="filterBy" type="checkbox" checked="checked" /><label id="lblVideo" title="Video" for="chkMedia" runat="server">Video</label><br />
                    <input title="<%=GetMessage("lbl commerce products") %> Option" id="chkProduct" name="filterBy" type="checkbox" checked="checked" /><label id="lblProduct" title="Products" for="chkProduct" runat="server">Products</label><br />
                    <div class="Ektron_TopSpaceSmall"></div>
                </fieldset>
	        </radTS:PageView>
            <radTS:PageView id="Pageview3" runat="server">
                <fieldset>
					<legend id="lblSearchBy" runat="server" title="Let content author select content by">Let content author select content by</legend>
                    <input title="Browsing Folders Option" id="chkFolder" name="chkFolder" type="checkbox" checked="checked" /><label id="lblFolder" title="Browsing folders" for="chkFolder" runat="server">Browsing folders</label><br />&#160;&#160;
                    &#160;&#160;&#160;&#160;<label for="txtDefFolder" class="Ektron_StandardLabel " id="lblDefFolder" title="Starting Folder" runat="server">Starting Folder:</label>
                    <asp:HiddenField runat="server" ID="txtFolderNavigation" Value="" />
		            <asp:HiddenField runat="server" ID="txtFolderId" Value="" />
		            <asp:HiddenField runat="server" ID="txtFolderTitle" Value="" />
                    <input type="text" name="txtDefFolder" id="txtDefFolder" readonly="readonly" class="Ektron_StandardTextBox Ektron_ReadOnlyBox" />
                    <a title="Select Folder" href="#" id="linkSelectFolderPopup" class="design_fieldbutton" onclick="popupSelector('startingfolder');" runat="server"><img class="design_fieldbutton" id="imgSelectFolderPopup" title="Select Folder" alt="Select Folder" runat="server" /></a>&#160;&#160;
                    <a title="Delete" href="#" id="linkDeleteDefaultFolder" onclick="clearSelectorValue('folder');" runat="server"><img id="imgDeleteDefaultFolder" runat="server" title="Delete" alt="Delete" class="design_fieldbutton" /></a>
                    <br />
                    <input title="<%=GetMessage("lbl browsing taxonomy categories") %> Option" id="chkTaxonomy" name="chkTaxonomy" type="checkbox" checked="checked" /><label id="lblTaxonomy" title="Browsing taxonomy categories" for="chkTaxonomy" runat="server">Browsing taxonomy categories</label><br />
                    <input title="<%=GetMessage("lbl search for key words") %> Option" id="chkWords" name="chkWords" type="checkbox" checked="checked" /><label id="lblWords" title="Searching for key words" for="chkWords" runat="server">Searching for key words</label><br /><br />
                </fieldset>
	        </radTS:PageView>
	        <radTS:PageView id="Pageview4" runat="server">
	            <fieldset>
					<legend id="lblSiteVisitorView" runat="server" title="Let site visitor view the resource as">Let site visitor view the resource as:</legend>
					<div id="appearanceContent" class="Ektron_DialogBodyContainer">
                    &#160;&#160;&#160;&#160;<input title="Select Content" id="chkContentBlock" name="appearanceContent" type="radio" value="content" checked="checked" /><label id="lblContentBlock" title="Content" for="chkContentBlock" runat="server">Content</label><br />
                    &#160;&#160;&#160;&#160;<input title="Select Content Title" id="chkContentTitle" name="appearanceContent" type="radio" value="title" /><label id="lblContentTitle" title="Content Title" for="chkContentTitle" runat="server">Content Title</label><br />
                    &#160;&#160;&#160;&#160;<input title="Title followed by the content" id="chkContentTitleHtml" name="appearanceContent" type="radio" value="titledcontent" /><label id="lblContentTitleHtml" title="Title followed by the content" for="chkContentTitleHtml" runat="server">Title followed by the content</label><br />
                    &#160;&#160;&#160;&#160;<input title="Quick Link" id="chkQuickLink" name="appearanceContent" type="radio" value="quicklink" /><label id="lblQuickLink" title="Quick Link" for="chkQuickLink" runat="server">Quick Link</label><br />
                    &#160;&#160;&#160;&#160;<input title="Quicklink with summary" id="chkQuickLinkSummary" name="appearanceContent" type="radio" value="teaser" /><label id="lblQuickLinkSummary" title="Quicklink with summary" for="chkQuickLinkSummary" runat="server">Quicklink with summary</label><br />
                    <div class="Ektron_TopSpaceSmall"></div>
                    </div>
                    <div id="appearanceFolder" class="Ektron_DialogBodyContainer">
                    &#160;&#160;&#160;&#160;<input title="Select List Quicklinks and summary of folder contents" id="chkFolderListSummary" name="appearanceFolder" type="radio" value="teaser" checked="checked" /><label id="lblFolderListSummary" title="List Quicklinks and summary of folder contents" for="chkFolderListSummary" runat="server">List Quicklinks and summary of folder contents</label><br />
                    &#160;&#160;&#160;&#160;<input title="Select <%=GetMessage("lbl folder quicklink") %>" id="chkFolderQuickLink" name="appearanceFolder" type="radio" value="quicklink" /><label id="lblFolderQuickLink" title="List Quicklinks of folder contents" for="chkFolderQuickLink" runat="server">List Quicklinks of folder contents</label><br />
                    &#160;&#160;&#160;&#160;<input title="Select <%=GetMessage("lbl sitemap path") %>" id="chkFolderBreadCrumb" name="appearanceFolder" type="radio" value="breadcrumb" /><label id="lblFolderBreadCrumb" title="Breadcrumb" for="chkFolderBreadCrumb" runat="server">Breadcrumb</label><br />
                    </div> 
                    <div id="appearanceTaxonomy" class="Ektron_DialogBodyContainer">
                    &#160;&#160;&#160;&#160;<input title="<%=GetMessage("lbl directory control") %>" id="chkTaxonomyDirectoryCtl" name="appearanceTaxonomy" type="radio" value="directoryctl" checked="checked" /><label id="lblTaxonomyDirectoryCtl" title="Directory control" for="chkTaxonomyDirectoryCtl" runat="server">Directory control</label><br />
                    </div> 
                    <div id="appearanceCollection" class="Ektron_DialogBodyContainer">
                    &#160;&#160;&#160;&#160;<input title="<%=GetMessage("lbl collection quicklink and teaser") %>" id="chkCollectionQuickLinkSummary" name="appearanceCollection" type="radio" value="teaser" checked="checked" /><label id="lblCollectionQuickLinkSummary" title="List Quicklinks and summary of collection items" for="chkCollectionQuickLinkSummary" runat="server">List Quicklinks and summary of collection items</label><br />
                    &#160;&#160;&#160;&#160;<input title="<%=GetMessage("lbl collection quicklink") %>" id="chkCollectionQuickLink" name="appearanceCollection" type="radio" value="quicklink" checked="checked" /><label id="lblCollectionQuickLink" for="chkCollectionQuickLink" title="List Quicklinks of collection items" runat="server">List Quicklinks of collection items</label><br />
                    </div> 
                    <div class="Ektron_DialogBodyContainer">
                    </div> 
                </fieldset>
	        </radTS:PageView> 
        </radTS:RadMultiPage>
	</div>

    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>		
	
	<ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />
</form> 
<script language="javascript" type="text/javascript">
<!--
    Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("div#RadTabStrip1 a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
    
    var ResourceText = 
	{
		sFieldHeading: "<asp:literal id="sFieldHeading" runat="server"/>"
	,	sInsertFieldHere: "<asp:literal id="sInsertFieldHere" runat="server"/>"
	,	sSelectContent: "<asp:literal id="sSelectContent" runat="server"/>"
	,	sCannotBeBlank: "<asp:literal id="sCannotBeBlank" runat="server"/>"
	,	sSelectOneOption: "<asp:literal id="sSelectOneOption" runat="server"/>"
	,	sSelectFolder: "<asp:literal id="sSelectFolder" runat="server"/>"
	,   sSelectTaxonomy: "<asp:literal id="sSelectTaxonomy" runat="server"/>"
	,   sSelectCollection: "<asp:literal id="sSelectCollection" runat="server"/>"
	,   sIncludeChild: "<asp:literal id="sIncludeChild" runat="server"/>"
	,   TreeRoot: "<asp:literal id="sTreeRoot" runat="server"/>"
	,   sChildren: "<asp:literal id="sChildren" runat="server"/>"
	,   sItems: "<asp:literal id="sItems" runat="server"/>"
	,   idType: {
            "folder": "<asp:literal id="sFolder" runat="server"/>" 
        ,   "content": "<asp:literal id="sContent" runat="server"/>"
        ,   "content:htmlcontent": "<asp:literal id="sHTMLContent" runat="server"/>" 
        ,   "content:htmlform": "<asp:literal id="sHtmlForm" runat="server"/>" 
	    ,   "content:smartform": "<asp:literal id="sSmartForm" runat="server"/>" 
        ,   "content:asset": "<asp:literal id="sAsset" runat="server"/>" 
        ,   "content:image": "<asp:literal id="sImage" runat="server"/>"
        ,   "content:multimedia": "<asp:literal id="sVideo" runat="server"/>" 
        ,   "content:product": "<asp:literal id="sProduct" runat="server"/>" 
        ,   "content:mso": "<asp:literal id="sMSOffice" runat="server"/>" 
        ,   "taxonomy": "<asp:literal id="sTaxonomy" runat="server"/>" 
        ,   "collection": "<asp:literal id="sCollection" runat="server"/>"
        }  
	};
    var m_objFormField = null;
    var m_oFieldElem = null;
    var m_oEditor = null;
    var m_idType = "";
    var m_idValue = "";
    var m_waPath = "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>";
    var m_langType; 
    
	function initField()
	{
	    m_objFormField = new EkFormFields();
	    m_oFieldElem = null;

        var oFieldElem = null;
	    var bIsRootLoc = false;
	    var sDefaultId = "";
	    var sDefaultPrefix = "";
	    var sContentTree = "";
        var args = GetDialogArguments();
	    if (args)
	    {
	        oFieldElem = args.selectedField;
	        bIsRootLoc = args.isRootLocation;
	        sDefaultPrefix = args.fieldPrefix;
	        sDefaultId = args.fieldId;
	        m_oEditor = args.EditorObj;
	        m_langType = m_oEditor.ekParameters.contentLanguage;
	    }

	    var date = null;
        if (m_objFormField.isDDFieldElement(oFieldElem) && "SPAN" == oFieldElem.tagName && $ektron(oFieldElem).hasClass("ektdesignns_resource"))
        {
            ekFieldNameControl.read(oFieldElem);
            
			var idValue = $ektron.toStr(oFieldElem.getAttribute("datavalue"));
			var idType = $ektron.toStr(oFieldElem.getAttribute("datavalue_idtype"));
			var sDisplayValue = $ektron.toStr(oFieldElem.getAttribute("datavalue_displayvalue"));
			var sTitle = "";
			if (sDisplayValue.length > 0)
			{
			    var pMarker = sDisplayValue.indexOf(" \xAB");
			    if (pMarker > 0)
			    {
			        sTitle = sDisplayValue.substr(0, pMarker);
			    }
			}
			
            document.getElementById("txtId").value = idValue;
            document.getElementById("txtIdType").value = idType;
            document.getElementById("txtTitle").value = sTitle;
            document.getElementById("txtDefVal").value = sDisplayValue; 
            document.getElementById("txtDefVal").title = sDisplayValue; 
          
            var idDefaultFolder = $ektron.toInt(oFieldElem.getAttribute("ektdesignns_defaultfolder"), 0);
			var sFolderNavigation = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_foldernavigation"), "descendant");
			var sFolderTitle = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_startfoldertitle"));
            var sFolderDisplay = m_objFormField.getResourceFieldDisplayValue(ResourceText, m_waPath, "folder", idDefaultFolder, (sFolderTitle || "Root"), m_langType, sFolderNavigation);
            document.getElementById("txtFolderId").value = idDefaultFolder;
            document.getElementById("txtFolderNavigation").value = sFolderNavigation;
            document.getElementById("txtDefFolder").value = sFolderDisplay;
            document.getElementById("txtDefFolder").title = sFolderDisplay;
            document.getElementById("txtFolderTitle").value = sFolderTitle;
            
            ekFieldMinMaxControl.read(oFieldElem);

            m_oFieldElem = oFieldElem;
            ekFieldNameControl.initSetting(true);
            
			var sSearchBy = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_searchby"), "folder taxonomy words");
			var searchByFolder = (/\bfolder\b/.test(sSearchBy));
			var searchByTaxonomy = (/\btaxonomy\b/.test(sSearchBy));
			var searchByWords = (/\bwords\b/.test(sSearchBy));
            document.getElementById("chkFolder").checked = searchByFolder;
            document.getElementById("chkTaxonomy").checked = searchByTaxonomy;
            document.getElementById("chkWords").checked = searchByWords;
 
            var sFilterBy = $ektron.toStr(oFieldElem.getAttribute("ektdesignns_filterby"), "content:htmlcontent");
			var filterByContent = (/\bcontent:htmlcontent\b/.test(sFilterBy));
			var filterBySmartForm = (/\bcontent:smartform\b/.test(sFilterBy));
			var filterByForm = (/\bcontent:htmlform\b/.test(sFilterBy));
			var filterByMSAsset = (/\bmso\b/.test(sFilterBy));
			var filterByOtherAsset = (/\basset\b/.test(sFilterBy));
			var filterByMedia = (/\bmultimedia\b/.test(sFilterBy));
			var filterByProduct = (/\bproduct\b/.test(sFilterBy));
			document.getElementById("chkContent").checked = filterByContent;
            document.getElementById("chkSmartForm").checked = filterBySmartForm;
            document.getElementById("chkForm").checked = filterByForm;
            document.getElementById("chkMSAsset").checked = filterByMSAsset;
            document.getElementById("chkOtherAsset").checked = filterByOtherAsset;
            document.getElementById("chkMedia").checked = filterByMedia;
            document.getElementById("chkProduct").checked = filterByProduct;
            
            switch (idType)
            {
                case "collection":
                    document.getElementById("resCollection").selected = true;
                    break;
                case "folder":
                    document.getElementById("resFolder").selected = true;
                    break;
                case "taxonomy":
                    document.getElementById("resTaxonomy").selected = true;
                    break;
                case "content":
                default:
                    document.getElementById("resContent").selected = true;
                    break;
            }
            updateFilterByOption();
            
            setSelectedResourceAppearance(oFieldElem.getAttribute("ektdesignns_appearance"));
	    }
	    else
		{
		    document.getElementById("txtIdType").value = "content";
		    ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
	        ekFieldNameControl.initSetting(false);
	        ekFieldMinMaxControl.setMin(0);
	        ekFieldMinMaxControl.setMax(1);
		}
	}
	
	function insertField()
	{
		if (false == validateDialog())
	    {
	        return false;
	    }
		var oFieldElem = m_oFieldElem;
		var objSpan = null;
	    if (null == oFieldElem)
	    {
		    oFieldElem = document.createElement("span");
		    oFieldElem.className = "ektdesignns_resource";
		    oFieldElem.setAttribute("contenteditable", "false");
		}

	    ekFieldNameControl.update(oFieldElem);
	    oFieldElem.setAttribute("ektdesignns_nodetype", "element");
	    var idValue = document.getElementById("txtId").value;
	    var idType = $ektron.toStr(document.getElementById("txtIdType").value, "content");
	    var sTitle = document.getElementById("txtTitle").value;
	    var idDisplayValue = m_objFormField.getResourceFieldDisplayValue(ResourceText, m_waPath, idType, idValue, sTitle, m_langType);
	    oFieldElem.setAttribute("datavalue_idtype", idType);
	    if (idValue)
	    {
	        oFieldElem.setAttribute("datavalue", $ektron.toInt(idValue, 0));
	        oFieldElem.setAttribute("datavalue_displayvalue", idDisplayValue);
	    }
	    else
	    {
	        oFieldElem.removeAttribute("datavalue");
	        oFieldElem.removeAttribute("datavalue_displayvalue");
	    }
	    var idDefaultFolder = $ektron.toInt(document.getElementById("txtFolderId").value, 0);
	    var sFolderNavigation = document.getElementById("txtFolderNavigation").value;
	    var sRoot = document.getElementById("txtFolderTitle").value;
	    if (idDefaultFolder)
	    {
	        oFieldElem.setAttribute("ektdesignns_defaultfolder", idDefaultFolder);
	    }
	    else
	    {
	        oFieldElem.removeAttribute("ektdesignns_defaultfolder");
	    }
	    if (sFolderNavigation.length > 0)
	    {
	        oFieldElem.setAttribute("ektdesignns_foldernavigation", sFolderNavigation);
	    }
	    else
	    {
	        oFieldElem.removeAttribute("ektdesignns_foldernavigation");
	    }
	    if (sRoot != ResourceText.TreeRoot)
	    {
	        oFieldElem.setAttribute("ektdesignns_startfoldertitle", sRoot);
	    }
	    
		var sSearchBy = "";
        if (!document.getElementById("chkFolder").disabled && document.getElementById("chkFolder").checked) sSearchBy += "folder ";
        if (!document.getElementById("chkTaxonomy").disabled && document.getElementById("chkTaxonomy").checked) sSearchBy += "taxonomy ";
        if (!document.getElementById("chkWords").disabled && document.getElementById("chkWords").checked) sSearchBy += "words ";
        if ("collection" == idType) sSearchBy = "collection";
        oFieldElem.setAttribute("ektdesignns_searchby", sSearchBy);
        
        oFieldElem.removeAttribute("ektdesignns_filterby");
        oFieldElem.setAttribute("ektdesignns_filterby", getFilterByString());
        
        var appearance = getSelectedResourceAppearance();
        if (appearance)
        {
			oFieldElem.setAttribute("ektdesignns_appearance", appearance);
		}
		else
		{
			oFieldElem.removeAttribute("ektdesignns_appearance");
		}
	    
	    ekFieldMinMaxControl.update(oFieldElem);
        oFieldElem.setAttribute("ektdesignns_validation", "content-req");
        var strAttOnBlur = "design_validate_re(/^[0-9]+$/,this,\"" + ResourceText.sCannotBeBlank + "\");";
        oFieldElem.setAttribute("onblur", Ektron.String.escapeJavaScriptAttributeValue(strAttOnBlur));
		objSpan = $ektron(oFieldElem).children("span").get(0); 
        if (!objSpan)
	    {
		    objSpan = oFieldElem.ownerDocument.createElement("span");
			objSpan.setAttribute("unselectable", "on");
			oFieldElem.appendChild(objSpan); 
			var objTextNode = oFieldElem.ownerDocument.createTextNode("\xA0");
			oFieldElem.appendChild(objTextNode);
			
			var strImgUrl = "<%=ResolveUrl(this.SkinControlsPath)%>ContentDesigner/btnexplorefolder.gif"; 

			var objFieldBtn = oFieldElem.ownerDocument.createElement("img");
			objFieldBtn.setAttribute("unselectable", "on");
			objFieldBtn.className = "design_fieldbutton";
			objFieldBtn.src = strImgUrl;
			switch (idType)
			{
			    case "collection":
			        objFieldBtn.alt = ResourceText.sSelectCollection; 
			        break;
			    case "taxonomy":
			        objFieldBtn.alt = ResourceText.sSelectTaxonomy; 
			        break;
			    case "folder":
			        objFieldBtn.alt = ResourceText.sSelectFolder; 
			        break;
			    case "content":
			    default:
			        objFieldBtn.alt = ResourceText.sSelectContent; 
			        break;
			}
			objFieldBtn.width = 16;
			objFieldBtn.height = 16;
			oFieldElem.appendChild(objFieldBtn);
		}
	    objSpan.innerHTML = idDisplayValue;

	    ekFieldNameControl.update(oFieldElem);
	   
		CloseDlg(oFieldElem);	
	}

    function getFilterByString()
    {
        var sFilterBy = "";
        if (document.getElementById("resContent").selected)
        {
            if (document.getElementById("chkContent").checked) sFilterBy += "content:htmlcontent ";
            if (document.getElementById("chkSmartForm").checked) sFilterBy += "content:smartform ";
            if (document.getElementById("chkForm").checked) sFilterBy += "content:htmlform ";
            if (document.getElementById("chkMSAsset").checked) sFilterBy += "content:mso ";
            if (document.getElementById("chkOtherAsset").checked) sFilterBy += "content:asset ";
            if (document.getElementById("chkMedia").checked) sFilterBy += "content:multimedia ";
            if (document.getElementById("chkProduct").checked) sFilterBy += "content:product ";
        }
        return sFilterBy;
    }
    
    function getSelectedResourceAppearance()
    {
        if (document.getElementById("resContent").selected)
        {
            if (document.getElementById("chkContentTitle").checked)
            {
                return "title";
            }
            else if (document.getElementById("chkContentTitleHtml").checked)
            {
                return "titledcontent";
            }
            else if (document.getElementById("chkQuickLink").checked)
            {
                return "quicklink";
            }
            else if (document.getElementById("chkQuickLinkSummary").checked)
            {
                return "teaser";
            }
            else
            {
                return ""; // default = content
            }
        }
        else if (document.getElementById("resFolder").selected)
        {
            if (document.getElementById("chkFolderBreadCrumb").checked)
            {
                return "breadcrumb";
            }
            else if (document.getElementById("chkFolderQuickLink").checked)
            {
                return "quicklink";
            }
            else 
            {
                return "teaser";
            }
        }
        else if (document.getElementById("resTaxonomy").selected)
        {
            return "directoryctl";
        }
        else if (document.getElementById("resCollection").selected)
        {
            if (document.getElementById("chkCollectionQuickLink").checked)
            {
                return "quicklink";
            }
            else 
            {
                return "teaser";
            }
        }
		return ""; // default
    }
    
    function setSelectedResourceAppearance(value)
    {
        if (document.getElementById("resContent").selected)
        {
			switch (value)
			{
				case "title":
					document.getElementById("chkContentTitle").checked = true;
					break;
				case "titledcontent":
					document.getElementById("chkContentTitleHtml").checked = true;
					break;
				case "quicklink":
					document.getElementById("chkQuickLink").checked = true;
					break;
				case "teaser":
					document.getElementById("chkQuickLinkSummary").checked = true;
					break;
				default:
					document.getElementById("chkContentBlock").checked = true;
					break;
			}
        }
        else if (document.getElementById("resFolder").selected)
        {
			switch (value)
			{
				case "breadcrumb":
					document.getElementById("chkFolderBreadCrumb").checked = true;
					break;
				case "quicklink":
					document.getElementById("chkFolderQuickLink").checked = true;
					break;
				case "teaser":
				default:
					document.getElementById("chkFolderListSummary").checked = true;
					break;
			}
		}
		else if (document.getElementById("resTaxonomy").selected)
        {
			document.getElementById("chkTaxonomyDirectoryCtl").checked = true;
		}
		else if (document.getElementById("resCollection").selected)
        {
			switch (value)
			{
				case "quicklink":
					document.getElementById("chkCollectionQuickLink").checked = true;
					break;
				case "teaser":
				default:
					document.getElementById("chkCollectionQuickLinkSummary").checked = true;
					break;
			}
		}
	}
    
    function updateFilterByOption()
    {
        var bDisableFilterByOption = true;
        var bDisableConfigurationOption = true;
        if (document.getElementById("resContent").selected)
        {
            bDisableFilterByOption = false;
            bDisableConfigurationOption = false;
            if (!/^content/.test(document.getElementById("txtIdType").value))
            {
				setDataValue("content", "", "");
            }
        }
        else
        {
            // default checked value although they are going to be disabled 
            document.getElementById("chkContent").checked = true;
            document.getElementById("chkSmartForm").checked = true;
            document.getElementById("chkForm").checked = true;
            document.getElementById("chkMSAsset").checked = true;
            document.getElementById("chkOtherAsset").checked = true;
            document.getElementById("chkMedia").checked = true;
        }
        if (document.getElementById("resFolder").selected)
        {
            bDisableConfigurationOption = false;
            if (!/^folder/.test(document.getElementById("txtIdType").value))
            {
				setDataValue("folder", "", "");
            }
            document.getElementById("chkFolder").disabled = false;
            document.getElementById("chkFolder").checked = true;
            document.getElementById("chkTaxonomy").disabled = true;
            document.getElementById("chkWords").disabled = true;
		}
		if (document.getElementById("resTaxonomy").selected)
        {
            if (!/^taxonomy/.test(document.getElementById("txtIdType").value))
            {
				setDataValue("taxonomy", "", "");
            }
            document.getElementById("chkFolder").disabled = true;
            document.getElementById("chkTaxonomy").disabled = false;
            document.getElementById("chkTaxonomy").checked = true;
            document.getElementById("chkWords").disabled = true;
		}
		if (document.getElementById("resCollection").selected)
        {
            if (!/^collection/.test(document.getElementById("txtIdType").value))
            {
				setDataValue("collection", "", "");
            }
            document.getElementById("chkFolder").disabled = true;
            document.getElementById("chkTaxonomy").disabled = true;
            document.getElementById("chkWords").disabled = true;
		}
		
        var tabStrip = <%= RadTabStrip1.ClientID %>;
        for (var i = 0; i < tabStrip.Tabs.length; i++)
        {
            if (i == 1) // Content Type tab
            {
                //Folder Resource and Taxonomy Resource Type do not have the Type Tab.
                if (bDisableFilterByOption)
                { 
                    tabStrip.Tabs[i].Disable();    
                }
                else
                {
                    tabStrip.Tabs[i].Enable();
                }
            }  
            else if (i == 2) // configuration tab
            {
                //Taxonomy Resource Type do not have the configuration Tab.
                if (bDisableConfigurationOption)
                { 
                    tabStrip.Tabs[i].Disable();    
                }
                else
                {
                    tabStrip.Tabs[i].Enable();
                }
            }          
        }
    }
    
    function popupSelector(fieldType)
	{
        if (m_oEditor)
        {
            var args = null;
            var sTitle = "";
            var retCallback = null;
            var width = 500;
            var height = 580;
            var sIdType = $ektron.toStr(document.getElementById("txtIdType").value, m_idType);
            var sResourceDisplay = $ektron.toStr(document.getElementById("txtDefVal").value, "");
            switch (fieldType)
            {
                case "collection":
                    args = {
	                    appPath: m_oEditor.ekParameters.srcPath
	                ,	langType: m_oEditor.ekParameters.contentLanguage
	                ,	selectedField: m_oFieldElem
			        ,	idType: sIdType
			        ,	idValue: $ektron.toStr(document.getElementById("txtId").value, m_idValue)
			        ,	searchByFolder: false
			        ,	searchByTaxonomy: false
			        ,	searchByWords: false
			        ,	filterBy: "collection"
                    ,   selectorType: fieldType
                    ,   folderNavigation: ""
                    ,   display: sResourceDisplay
	                };
	                sTitle = ResourceText.sSelectCollection;
	                retCallback = setResourceSelectorValue;
	                break;
                case "taxonomy":
                    args = {
	                    appPath: m_oEditor.ekParameters.srcPath
	                ,	langType: m_oEditor.ekParameters.contentLanguage
	                ,	selectedField: m_oFieldElem
			        ,	idType: sIdType
			        ,	idValue: $ektron.toStr(document.getElementById("txtId").value, m_idValue)
			        ,	searchByFolder: false
			        ,	searchByTaxonomy: true
			        ,	searchByWords: false
			        ,	filterBy: "taxonomy"
                    ,   selectorType: fieldType
                    ,   folderNavigation: ""
                    ,   display: sResourceDisplay
	                };
	                sTitle = ResourceText.sSelectTaxonomy;
	                retCallback = setResourceSelectorValue;
                    height = 450;
                    break;
                case "startingfolder":
                case "folder":
                    args = {
	                    appPath: m_oEditor.ekParameters.srcPath
	                ,	langType: m_oEditor.ekParameters.contentLanguage
	                ,	selectedField: m_oFieldElem
			        ,	idType: sIdType
			        ,	idValue: $ektron.toStr(document.getElementById("txtId").value, m_idValue)
			        ,	searchByFolder: true
			        ,	searchByTaxonomy: false
			        ,	searchByWords: false
//			        ,	filterBy: getFilterByString()
                    ,   selectorType: fieldType
                    ,   defaultFolder: ($ektron.toInt(document.getElementById("txtFolderId").value, 0))
			        ,   folderNavigation: $ektron.toStr(document.getElementById("txtFolderNavigation").value, "descendant")
			        ,   startFolderTitle: $ektron.toStr(document.getElementById("txtFolderTitle").value, ResourceText.TreeRoot)
			        ,   display: sResourceDisplay
	                };
	                sTitle = ResourceText.sSelectFolder;
	                retCallback = setResourceSelectorValue;
                    height = 450;
        	        break;
                case "content":
                default:
                    fieldType = "content";
                    args = {
	                    appPath: m_oEditor.ekParameters.srcPath
	                ,	langType: m_oEditor.ekParameters.contentLanguage
	                ,	selectedField: m_oFieldElem
			        ,	idType: sIdType
			        ,	idValue: $ektron.toStr(document.getElementById("txtId").value, m_idValue)
	                ,	searchByFolder: document.getElementById("chkFolder").checked
			        ,	searchByTaxonomy: document.getElementById("chkTaxonomy").checked
			        ,	searchByWords: document.getElementById("chkWords").checked
			        ,	filterBy: getFilterByString()
			        ,   selectorType: fieldType
			        ,   defaultFolder: ($ektron.toInt(document.getElementById("txtFolderId").value, 0))
			        ,   folderNavigation: $ektron.toStr(document.getElementById("txtFolderNavigation").value, "descendant")
			        ,   startFolderTitle: $ektron.toStr(document.getElementById("txtFolderTitle").value, ResourceText.TreeRoot)
			        ,   display: sResourceDisplay
	                };
	                sTitle = ResourceText.sSelectContent;
	                retCallback = setResourceSelectorValue;
                    break;
            }
        }
        m_oEditor.ShowDialog(
            m_oEditor.ekParameters.srcPath + "dialogs/resourceselectorpopup.aspx?LangType=" + m_oEditor.ekParameters.contentLanguage + "&SelectorType=" + fieldType + "&idType=" + sIdType
            , args
            , width
            , height
            , setResourceSelectorValue
            , null
            , sTitle);
        return false;
	}
	
	function setResourceSelectorValue(returnValue)
	{
	    if (returnValue)
	    {
			switch (returnValue.selectorType)
			{
			    case "startingfolder":
		            document.getElementById("txtFolderId").value = returnValue.folderId;
                    document.getElementById("txtFolderTitle").value = returnValue.folderTitle;
                    document.getElementById("txtFolderNavigation").value = returnValue.folderNavigation;
                    document.getElementById("txtDefFolder").value = returnValue.sDisplay;
                    document.getElementById("txtDefFolder").title = returnValue.sDisplay;
	                break;
	            case "folder":
	                setDataValue(returnValue.idType, returnValue.folderId, returnValue.sDisplay, returnValue.folderTitle);
			        break;
			    case "taxonomy":
			    case "collection":
			        setDataValue(returnValue.idType, returnValue.resourceId, returnValue.sDisplay, returnValue.resourceTitle);
			        document.getElementById("txtFolderNavigation").value = "";
			        break;
			    case "content":
			    default:
			        setDataValue(returnValue.idType, returnValue.idValue, returnValue.sDisplay, returnValue.contentTitle);
	                break;
	        }
	    }
	}
	
	function setDataValue(idType, idValue, sDisplay, sTitle)
	{
	    m_idType = idType;
        m_idValue = idValue;
        document.getElementById("txtIdType").value = idType;
        document.getElementById("txtId").value = idValue;
        document.getElementById("txtDefVal").value = sDisplay;
        document.getElementById("txtDefVal").title = sDisplay;
        document.getElementById("txtTitle").value = sTitle;
	}
	
	function clearSelectorValue(resourceType)
	{
	    switch (resourceType)
	    {
	        case "folder":
	            document.getElementById("txtFolderId").value = "";
	            document.getElementById("txtFolderTitle").value = "";
                document.getElementById("txtFolderNavigation").value = "";
                document.getElementById("txtDefFolder").value = "";
                document.getElementById("txtDefFolder").title = "";
	            break;
	        case "collection":
	        case "taxonomy":
	        case "content":
	        default:
                document.getElementById("txtId").value = "";
                document.getElementById("txtDefVal").value = "";
                document.getElementById("txtDefVal").title = "";
                document.getElementById("txtTitle").value = "";
                $ektron(".HiddenTBData").val("");
                break;
        }
	}
    
	
    function validateDialog()
	{
	    var bContinue = true;
	    var ret = [];
	    $ektron(document).trigger("onValidateDialog", [ret]);
	    if (ret && ret.length > 0)
	    {
            if ("Configuration" == ret[0].name)
            {
                var oTabCtl = <%= RadTabStrip1.ClientID %>;
                var currentTab = oTabCtl.SelectedTab;
                if (currentTab.Value != "Configuration")
                {
                    var tab = oTabCtl.FindTabById("<%= RadTabStrip1.ClientID %>_Configuration");
                    if (tab)
                    {
                        tab.SelectParents();
                    }
                }  
                bContinue = EkFormFields_PromptOnValidateAction(ret[0]); 
            } 
	    }
	    return bContinue;
	}
	
	function ClientTabSelectedHandler(sender, eventArgs)
	{      
	    var tab = eventArgs.Tab;  
	    var tabSelected = tab.Value.toLowerCase();
	    switch(tabSelected)
	    {
	        case "appearance":
	            $ektron("#appearanceContent").hide();
	            $ektron("#appearanceFolder").hide();
	            $ektron("#appearanceTaxonomy").hide();
	            $ektron("#appearanceCollection").hide();
				if (document.getElementById("resContent").selected)
				{
					$ektron("#appearanceContent").show();
				}
				else if (document.getElementById("resFolder").selected) 
				{
					$ektron("#appearanceFolder").show();
				}
				else if (document.getElementById("resTaxonomy").selected)
				{
					$ektron("#appearanceTaxonomy").show();
				}
				else if (document.getElementById("resCollection").selected)
				{
					$ektron("#appearanceCollection").show();
				}
	            break;
	        case "configuration":
	            document.getElementById("chkFolder").disabled = false;
	            document.getElementById("chkTaxonomy").disabled = false;
	            document.getElementById("chkWords").disabled = false;
	            if ("folder" == document.getElementById("txtIdType").value)
	            {
	                document.getElementById("chkTaxonomy").disabled = true;
	                document.getElementById("chkWords").disabled = true;
	            }
	            break;
	        case "resourcetype":
	            break;
	        case "general":
	            break;
	        default:
	            break;
	    }
	}
	
	Ektron.ready(function()
    {
        $ektron(document).bind("onValidateDialog", function(ev, oRet)
        {
            var errObj = null;
            if (!document.getElementById("resCollection").selected)
            {
			    var searchByFolder = document.getElementById("chkFolder").checked;
			    var searchByTaxonomy = document.getElementById("chkTaxonomy").checked;
			    var searchByWords = document.getElementById("chkWords").checked;
			    if (!(searchByFolder || searchByTaxonomy || searchByWords))
			    {
                    errObj = 
                    {
                        name:       "Configuration",
                        message:	ResourceText.sSelectOneOption, 
                        srcElement: document.getElementById("chkFolder")
                    };
                }
            }
            if (errObj)
            {
                oRet.push(errObj);
            }
        });
    });
//-->
</script>
</body>
</html>

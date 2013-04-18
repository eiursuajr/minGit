<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="resourceselectorpopup.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ResourceSelectorPopup" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>
<%@ Register TagPrefix="ek" TagName="FolderTree" Src="ucContentFolderTree.ascx" %>
<%@ Register TagPrefix="ek" TagName="TaxonomyTree" Src="ucTaxonomyTree.ascx" %>
<%@ Register TagPrefix="ek" TagName="CollectionTree" Src="ucCollectionTree.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title id="Title" runat="server">Select Content</title>
    <style type="text/css">
        div#CBResults 
        {
        	height: 10em !important;
        }
        div#Ektron_Dialog_Tabs_BodyContainer, .Ektron_Dialog_Tabs_BodyContainer, .Ektron_DialogTabBodyContainer
        {
        	margin: 0 !important;
        }
        .Invisible
        {
			display: none;
        }
        span.folder
        {
        	cursor: hand; 
        	cursor: pointer;
        }
        .CBfoldercontainer span.folderselected, div.treecontainer .selected
        {
	        background-color: #9cf !important;
        }
    </style>
</head>
<body class="dialog">
<form id="Form1" runat="server">
   <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
			OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
        <Tabs>
            <radTS:Tab ID="Folder" Text="Folder" Value="Folder" DisabledCssClass="Invisible" />
            <radTS:Tab ID="Taxonomy" Text="Taxonomy" Value="Taxonomy" DisabledCssClass="Invisible" />
            <radTS:Tab ID="Search" Text="Search" Value="Search" DisabledCssClass="Invisible" />
            <radTS:Tab ID="Collection" Text="Collection" Value="Collection" DisabledCssClass="Invisible" />
        </Tabs>
    </radTS:RadTabStrip>  
    </div>
    <div id="TreeViewDiv" class="Ektron_Dialog_Tabs_BodyContainer CBWidget">
        <radTS:RadMultiPage Height="260" id="RadMultiPage1" runat="server" SelectedIndex="0" AutoScrollBars="true">
            <radTS:PageView id="PageViewFolder" runat="server" > 
	            <ek:FolderTree ID="foldertree" runat="server" />
	        </radTS:PageView>
            <radTS:PageView id="PageViewTaxonomy" runat="server">
                <ek:TaxonomyTree ID="taxtree" runat="server" />
	        </radTS:PageView>
	        <radTS:PageView id="PageViewSearch" runat="server">
				<div class="BySearch">
					<label for="txtSearch" class="Ektron_StandardLabel" ID="lblSearchTerms" runat="server">Enter Search Terms:</label> <input id="txtSearch" type="text" class="searchtext" /> <a href="#" class="xsearchSubmit" ID="linkSearch" runat="server" onclick="return DoSearch(this);" onkeydown="return DoSearch(this);" title="Search">Search</a>
				</div>
	        </radTS:PageView>
	        <radTS:PageView id="PageViewCollection" runat="server" > 
	            <ek:CollectionTree ID="collectiontree" runat="server" />
	        </radTS:PageView>
        </radTS:RadMultiPage>
        <div id="GroupOptionsDiv" runat="server">
            <input id="chkAllItems" title="<%=GetMessage("lbl all folders") %>" name="chkItem" type="radio" /><label id="lblAllItems" title="All folders" for="chkAllItems" runat="server">All folders</label><br />
            <input id="chkChildItem" title="<%=GetMessage("lbl include child folder") %>" name="chkItem" type="radio" checked="checked" /><label id="lblChildItem" title="From this folder and its subfolder" for="chkChildItem" runat="server">From this folder and its subfolder</label><br />
            <input id="chkCurrentItem" title="<%=GetMessage("lbl this folder only") %>" name="chkItem" type="radio" /><label id="lblCurrentItem" title="From this folder only" for="chkCurrentItem" runat="server">From this folder only</label><br />
        </div>
	    <div id="ContentSelectionDiv" class="CBEdit" runat="server">
	        <div id="ResultsToggle"><a ID="linkResult" title="View Results" runat="server" href="#" onclick="return toggleResultsPane();">View Results</a></div>
            <div id="CBResults" style="display:none;"></div>
            <div id="CBPaging"></div>
        </div>
        <div id="ItemPathDiv" runat="server">
            <div class="Ektron_TopSpaceSmall"></div>
            <asp:Label id="lblCurrentPath" ToolTip="Current Path" runat="server" CssClass="label">Current Path:</asp:Label>
            <div id="taxPath"></div>
        </div>
        <asp:TextBox ID="tbData" ToolTip="Data" CssClass="HiddenTBData" runat="server" style="display:none;"></asp:TextBox>
        <asp:TextBox ID="tbFolderPath" ToolTip="Folder Path" CssClass="HiddenTBFolderPath" runat="server" style="display:none;"></asp:TextBox>
        <asp:TextBox ID="tbTaxonomyPath" ToolTip="Taxonomy Path" CssClass="HiddenTBTaxonomyPath" runat="server" style="display:none;"></asp:TextBox>
    	<input type="hidden" id="hdnAppPath" name="hdnAppPath" value="" />
        <input type="hidden" id="hdnLangType" name="hdnLangType" value="" />
        <input type="hidden" id="hdnFolderId" name="hdnFolderId" value="0"/>
    </div>
    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>		 
    <ek:FieldDialogButtons ID="FieldDialogButtons" OnOK="return insertField();" runat="server" />    
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
		sWarnNoSelection: "<asp:literal id="sWarnNoSelection" runat="server"/>"
	,	sWarnMultiSelection: "<asp:literal id="sWarnMultiSelection" runat="server"/>"
	,	sWarnNoResult: "<asp:literal id="sWarnNoResult" runat="server"/>"
	,   TreeRoot: "<asp:literal id="sTreeRoot" runat="server"/>"
	,   CollectionItems: "<asp:literal id="collectionItems" runat="server"/>"
	,   TaxonomyItems: "<asp:literal id="taxonomyItems" runat="server"/>"
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
    var webserviceURL = "";
    var m_filterBy = "content";
    var m_selectorType = "content";
    var m_idType = "content:htmlcontent";
    var m_defaultFolder = 0;
    var m_folderNavigation = "descendant";
    var m_objFormField = null;
    var m_waPath = "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>";
    var m_langType;
    var m_resourceTitle = "";
    
    function initField()
    {
        m_objFormField = new EkFormFields();
        var appPath;
        var idValue;
		var bSearchByFolder = true;
		var bSearchByTaxonomy = true;
		var bSearchByWords = true;
		var sRoot = ResourceText.TreeRoot;
		var searchTerm;

        var args = GetDialogArguments();
        if (args)
        {
            appPath = args.appPath;
            m_langType = args.langType;
			m_idType = args.idType;
			idValue = args.idValue + "";
            bSearchByFolder = args.searchByFolder;
            bSearchByTaxonomy = args.searchByTaxonomy;
            bSearchByWords = args.searchByWords;
            m_filterBy = (args.filterBy || "content:htmlcontent ");
            m_selectorType = args.selectorType;
            m_defaultFolder = args.defaultFolder;
            m_folderNavigation = (args.folderNavigation || m_folderNavigation);
            var startFolderTitle = args.startFolderTitle;
            var rootFolder = 0;
            if (startFolderTitle && startFolderTitle.length > 0 && m_folderNavigation != "ancestor" && m_selectorType != "startingfolder")
            {
                sRoot = args.startFolderTitle;
                rootFolder = m_defaultFolder;
            }
            searchTerm = args.searchTerm;
            m_resourceTitle = getResourceFieldResourceTitle(args.display);
        }
	    document.getElementById("hdnAppPath").value = appPath;
	    document.getElementById("hdnLangType").value = m_langType;
	    
		webserviceURL = (appPath ? appPath : m_waPath + "ContentDesigner/");
		webserviceURL += "dialogs/contentfoldertree.ashx";
		if (m_langType)
		{
			webserviceURL += "?LangType=" + m_langType;
		}
		
        var oTabCtl = <%= RadTabStrip1.ClientID %>;
        var tabFolder = oTabCtl.FindTabById("<%= RadTabStrip1.ClientID %>_Folder");
        var tabTaxonomy = oTabCtl.FindTabById("<%= RadTabStrip1.ClientID %>_Taxonomy");
        var tabWords = oTabCtl.FindTabById("<%= RadTabStrip1.ClientID %>_Search");
        var tabCollection = oTabCtl.FindTabById("<%= RadTabStrip1.ClientID %>_Collection");
		if (bSearchByFolder)
		{
			switch (m_selectorType)
			{
			    case "folder":
			    case "startingfolder":
			        //folder selector
			        if ("folder" == m_idType)
			        {
			            if (idValue != "undefined" && idValue.length > 0) 
			            {
			                m_defaultFolder = idValue;
			            }
			            var oCurrentItem = document.getElementById("chkCurrentItem");
			            if (oCurrentItem)
			            {
			                oCurrentItem.disabled = true;
			            }
			        }
		            if (document.getElementById("chkAllItems"))
		            {
		                if ("ancestor" == m_folderNavigation)
		                {
		                    document.getElementById("chkAllItems").checked = true;
		                }
		                else if ("descendant" == m_folderNavigation)
		                {
		                    document.getElementById("chkChildItem").checked = true;
		                }
		                else if (document.getElementById("chkCurrentItem"))
		                {
		                    document.getElementById("chkCurrentItem").checked = true;
		                }
		            }
		            if (m_defaultFolder)
		            {
		                updateFolderPath("folder", m_defaultFolder, m_filterBy);
		            }

			        $ektron("div.CBfoldercontainer span.folder").attr("data-ektron-folid", rootFolder).text(sRoot);
			        $ektron("div.CBfoldercontainer span[data-ektron-folid='" + m_defaultFolder + "']").addClass("folderselected");
                    ConfigContentFolderTreeView(m_filterBy, "folder", rootFolder);
                    openToSelectedContent();
                    break;
			    case "content":
			    default:
			        //content selector
			        $ektron("div.CBfoldercontainer span.folder").attr("data-ektron-folid", rootFolder).text(sRoot);
			        if (idValue)
			        {
				        document.getElementById("<%= tbData.ClientID %>").value = idValue;
				    }
				    if (searchTerm)
			        {
			            $ektron("input#txtSearch").attr("value", searchTerm);
			        }
				    if (m_defaultFolder >= 0)
				    {
			            if ("descendant" == m_folderNavigation || "ancestor" == m_folderNavigation)
			            {
			                // show child folder
			                $ektron.ajax(
                            {
                                type: "POST",
                                cache: false,
                                async: false,
                                url: webserviceURL,
                                data: {"request" : createRequestObj("getchildfolders", 0, "", "folder", rootFolder, m_filterBy) },
                                success: function(msg)
                                {
                                    var directory = eval("(" + msg + ")");
                                    var foldertree = ContentDirectoryToHtml(directory);
                                    if (foldertree.length > 0)
                                    {
                                        $ektron("ul.EktronFolderTree").html(foldertree);
                                        $ektron("ul.EktronFolderTree").treeview(
                                    {
                                        toggle : function(index, element)
                                        {
                                            var $element = $ektron(element);
                                            if($element.html() === "")
                                            {
                                                var folderid = $element.attr("data-ektron-folid");
                                                $ektron.ajax(
                                                {
                                                    type: "POST",
                                                    cache: false,
                                                    async: false,
                                                    url: webserviceURL,
                                                    data: {"request" : createRequestObj("getchildfolders", 0, "", "folder", folderid, m_filterBy) },
                                                    success: function(msg)
                                                    {
                                                        var directory = eval("(" + msg + ")");
                                                            var subfoldertree = ContentDirectoryToHtml(directory);
                                                            if (subfoldertree.length > 0)
                                                            {
                                                                var el = $ektron(subfoldertree);
                                                        $element.append(el);
                                                        $ektron("ul.EktronFolderTree").treeview({add: el});
                                                        configClickAction(m_filterBy, "content");
                                                        updateFolderPath("content", idValue, m_filterBy);
                                                    }
                                                        }
                                                });
                                            }
                                        }
                                    });
                                    }
                                    configClickAction(m_filterBy, "content");
                                    if (idValue)
                                    {
                                        updateFolderPath("content", idValue, m_filterBy);
                                    }
                                    else
                                    {
                                        updateFolderPath("folder", m_defaultFolder, m_filterBy);
                                    }
                                }
                            });
                            getResults("getfoldercontent", m_defaultFolder, 0, "folder", "", document.getElementById("<%= tbFolderPath.ClientID %>"), m_filterBy);
                        }
                        else
                        {
                            // just show content in the selected starting folder
                            document.getElementById("<%= tbFolderPath.ClientID %>").value = m_defaultFolder;
                            getResults("getfoldercontent", m_defaultFolder, 0, "folder", "", document.getElementById("<%= tbFolderPath.ClientID %>"), m_filterBy);
                        }
                        openToSelectedContent();
			        }
			        else
			        {
				        ConfigContentFolderTreeView(m_filterBy, m_selectorType, 0);
			        }
			        break;
			}
		}
		else
		{
            tabFolder.Disable();
		}
		if (bSearchByTaxonomy)
		{
            if (!bSearchByFolder) 
            {
                tabTaxonomy.SelectParents();
            }
            if ("taxonomy" == m_idType)
            {
                if (idValue.length > 0)
		        {
			        document.getElementById("<%= tbData.ClientID %>").value = idValue;
			        updateFolderPath("taxonomy", idValue, m_filterBy);
			    }
            }
            ConfigTaxonomyTreeView(m_filterBy);
		}
		else
		{
            tabTaxonomy.Disable();
		}
		if (bSearchByWords)
		{
            if (!bSearchByFolder && !bSearchByTaxonomy) tabWords.SelectParents();
			ConfigSearch();
		}
		else
		{
			tabWords.Disable();
		}
		if ("collection" == m_idType)
		{
		    $ektron("#ResultsToggle").html("<asp:literal id="sCollectionItems" runat="server"/>");
		    tabCollection.SelectParents();
		    configCollectionClickAction();
		    if (idValue.length > 0)
	        {
		        document.getElementById("<%= tbData.ClientID %>").value = idValue;
		        openToSelectedCollection(idValue);
		    }
		}
		else
		{
			tabCollection.Disable();
		}
    }	
    
    function insertField()
    {
        var retObj = null;
        var el = null;
        var sIdType = m_idType;
        var idValue;
        switch(m_selectorType)
        {
            case "taxonomy":
            case "collection":
                el = $ektron("div.treecontainer").find("span.selected");
                if (el.length === 0)
                {
                    alert(ResourceText.sWarnNoSelection);
                }
                else
                {
                    idValue = parseInt(el.attr("data-ektron-resid"), 10);
                    var sResTitle = $ektron.trim($ektron("div.treecontainer span.selected").text());
                    retObj = 
                    {
                        resourceId: idValue
                    ,   resourceTitle: sResTitle
                    ,   sDisplay: m_objFormField.getResourceFieldDisplayValue(ResourceText, m_waPath, sIdType, idValue, sResTitle, m_langType)
                    ,   idType: sIdType
                    };
                }
                break;
            case "folder":
            case "startingfolder":
                el = $ektron(".CBfoldercontainer").find("span.folderselected");
                if (el.length === 0)
                {
                    alert(ResourceText.sWarnNoSelection);
                }
                else
                {
                    var nav = getBrowseByString();
                    idValue = parseInt(el.attr("data-ektron-folid"), 10);
                    var sFolderTitle = $ektron.trim(($ektron("div.CBfoldercontainer span.folderselected").text() || ResourceText.TreeRoot));
                    retObj = 
                    {
                        folderId: idValue
                    ,	folderNavigation: nav
                    ,   folderTitle: sFolderTitle
                    ,   sDisplay: m_objFormField.getResourceFieldDisplayValue(ResourceText, m_waPath, "folder", idValue, sFolderTitle, m_langType, nav)
                    ,   idType: sIdType
                    };  
                }
                break;
            case "content":
            default:
                el = $ektron("#CBResults").find("div.selected");
                if (el.length === 0)
                {
                    alert(ResourceText.sWarnNoSelection);
                }
                else if (el.length > 1)
                {
                    alert(ResourceText.sWarnMultiSelection);
                }
                else
                {
                    var searchTerm = $ektron("input#txtSearch").attr("value");
                    $ektron(".HiddenTBData")[0].value = parseInt(el.children("span.contentid").text(), 10);
                    //make sure any cluetips get lost
                    $ektron("#cluetip, #cluetip-waitimage").remove();
                    sIdType = el.children("span.idtype").text().toLowerCase();
                    idValue = parseInt(el.children("span.contentid").text(), 10);
                    var sContentTitle = $ektron.trim(el.children("span.title").text());
                    retObj = 
                    {
                        idValue: idValue
                    ,	idType: sIdType
                    ,   contentTitle: sContentTitle
                    ,   sDisplay: m_objFormField.getResourceFieldDisplayValue(ResourceText, m_waPath, sIdType, idValue, sContentTitle, m_langType)
                    ,   searchTerm: searchTerm
                    };  
                }
                break;
        }
        if (retObj)
        {
            retObj.selectorType = m_selectorType;
            CloseDlg(retObj);	
        }
        return false;
    }
    
    function getResourceFieldResourceTitle(fullDisplay)
    {
        //For example, resource «Smart Form:564»
        if (!fullDisplay) return "";
        var lend = fullDisplay.indexOf("«");
        if (lend > -1)
        {
            return $ektron.trim(fullDisplay.substr(0, lend));
        }
        else
        {
            return fullDisplay;
        }
    }
    
    function getBrowseByString()
    {
        var sBrowseBy = "";
        if (m_selectorType != "folder")
        {
            if (document.getElementById("chkAllItems").checked) sBrowseBy = "ancestor";
            else if (document.getElementById("chkChildItem").checked) sBrowseBy = "descendant";
            else if (document.getElementById("chkCurrentItem").checked) sBrowseBy = "children";
        }
        return sBrowseBy;
    }
    
    function toggleResultsPane()
    {
        $ektron("#CBResults").slideToggle(750);
        return false;
    }
    
    function DoSearch(el)
    {
        var objectID = 0;
        var pageNum = 0;
        var action = "search";
        var objecttype = "search";
        var searchtext = $ektron("div.BySearch input.searchtext")[0].value;
        getResults(action, objectID, pageNum, objecttype, searchtext, el, m_filterBy, m_defaultFolder, m_folderNavigation);
        return false;
    }
    function ConfigSearch()
    {
        $ektron("div.BySearch input.searchtext").bind("keydown", function(evt){
            evtElement = $ektron(this)[0];
            if (evt.keyCode == 13){
                evt.preventDefault();
                evt.stopPropagation();
                evt.returnValue = false;
                evt.cancel = true;
                setTimeout(function(){
                    DoSearch(evtElement);
                }, 1);
                return false;
            }
        });
    }
    
    function getResults(action, objectID, pageNum, objecttype, search, el, filterBy, defaultFolder, folderNavigation)
    {
        var str = createRequestObj(action, pageNum, search, objecttype, objectID, filterBy, defaultFolder, folderNavigation);
        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: webserviceURL,
            data: {"request" : str },
            success: function(msg)
            {
               if (msg.length > 0)
               {
                    var contentitems = eval("(" + msg + ")");
                    var parentID;
                    if ( el != "")
                    {
                        parentID = $ektron(el).parents("div.CBWidget").attr("id");
                    }
                    var CBResults = $ektron("#"+ parentID);
                    CBResults = $ektron(CBResults).find("#CBResults");
                    CBResults.html(ContentToHtml(contentitems));

                    if (filterBy != "collection")
                    {
                        $ektron(CBResults).find("div.CBresult").click(function(){
                            onContentItemHandler(this);
                        });
                        $ektron(CBResults).find("div.CBresult").keydown(function(e){
                            if (32 == e.keyCode) // space bar
                            {
                                onContentItemHandler(this);
                            }
                        });
                        function onContentItemHandler(item)
                        {
                            var $item = $ektron(item);
                            parentID= $item.parents("div.CBWidget").attr("id");
                            $ektron(CBResults).find("div.CBresult").removeClass("selected");
                            $item.addClass("selected");           
                            setTimeout(function() {
                                // let the event done before scrolling into the item.
                                $item.get(0).scrollIntoView(false);
                            }, 1);
                        }
                    }
                    //CBResults.cluetip({cursor:'pointer', arrows:true, leftOffset:'25px'});
                    $ektron(CBResults).find("div.CBresult").cluetip({positionBy: "bottomTop", cursor:'pointer', arrows: true, leftOffset:"25px", topOffset: "20px", cluezIndex: 3500 });
                    $ektron("#"+ parentID).find("#CBPaging").html(contentitems.paginglinks);
                    //$ektron("#CBPaging").html(contentitems.paginglinks);

                    CBResults.slideDown(750);
                    if ("getfoldercontent" == action)
                    {
                        highlightSelectedContent();
                    }
                }
                else
                {
                    alert(ResourceText.sWarnNoResult);
                }
            }
        });
        return false;
	}
	
	function createRequestObj(action, pagenum, searchtext, objecttype, objectid, contenttype, defaultfolder, foldernavigation)
    {
        request = {
            "action": action,
            "filter": (contenttype || "content"),
            "page": pagenum,
            "searchText": searchtext,
            "objectType": objecttype,
            "objectID": objectid,
            "startFolder": (defaultfolder || 0)
        };
	    return Ektron.JSON.stringify(request);
	}
	
	function ContentToHtml(contentlist)
    {
        var html = "";
        if ("undefined" == typeof contentlist.contents || contentlist.contents === null || contentlist.contents.length === 0)// #57717
        {
            html = "<div class=\"CBNoresults\">" + ResourceText.sWarnNoResult + "</div>";
        }
        else
        {
            for (var i in contentlist.contents)
            {
                //#53210: this structure is reuse in function highlightSelectedContent() at ucContentFolderTree.ascx
                html += "<div tabindex=\"0\" ";
                html += "class=\"CBresult " + ((i%2 === 0) ? "even" : "odd") + "\" ";
                html += "rel=\"" + webserviceURL + "&detail=" + contentlist.contents[i].id + "\" ";
                html += "title=\"" + contentlist.contents[i].title + "\">";
                html += "<span class=\"icon\">" + contentlist.contents[i].icon + "</span>";
                html += "<span class=\"contentid\">" + contentlist.contents[i].id + "</span>";
                html += "<span class=\"title\">" + contentlist.contents[i].title + "</span>";
                html += "<span class=\"idtype\" style=\"display:none;\" >" + contentlist.contents[i].idtype + "</span>";
                html += "<br class=\"clearall\" />";
                html += "</div>";
            }
        }
        return html;
    }
    
    function updateFolderPath(resourceType, idValue, filterBy)
    {
        if ("taxonomy" == resourceType)
        {
            $ektron.ajax(
            {
                type: "POST",
                cache: false,
                async: false,
                url: webserviceURL,
                data: { request: req = createRequestObj("gettaxonomypath", 0, "", "taxonomy", idValue, filterBy, null, null, m_langType) },
                success: function(taxonomypath)
                {
		            document.getElementById("<%= tbTaxonomyPath.ClientID %>").value = taxonomypath;
                }
            });
        }
        else
        {
            $ektron.ajax(
            {
                type: "POST",
                cache: false,
                async: false,
                url: webserviceURL,
                data: { request: req = createRequestObj("getfolderpath", 0, "", resourceType, idValue, filterBy)},
                success: function(folderpath)
                {
		            document.getElementById("<%= tbFolderPath.ClientID %>").value = folderpath;
                }
            });    
        }
    }
    
    function ClientTabSelectedHandler(sender, eventArgs)
	{
	    $ektron("#CBResults").html("<div class=\"CBNoresults\">&#160;</div>");
	    $ektron("#CBPaging").html("&#160;");
	}
// -->
</script>
</body> 
</html> 

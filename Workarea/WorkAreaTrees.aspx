<%@ Page Language="C#" AutoEventWireup="true" Inherits="WorkAreaTrees" CodeFile="WorkAreaTrees.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WorkAreaTrees</title>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <script type="text/javascript">
		var ContentUrl = "";
		var FrameName = "<asp:Literal id='frameName' runat='server' />";
		var rootFolderName;
		var rootTaxonomyName;
		var rootCollectionName;
		var rootMenuName;
		var treesLoaded = 0;
		var g_selectedFolderList = "<asp:Literal id='selectedFolderList' runat='server' />";
		var g_selectedTaxonomyList = [<asp:Literal id='selectedTaxonomyList' runat='server' />];
		var g_selectedMenuList = [<asp:Literal id='selectedMenuList' runat='server' />];
        var g_selectedCollectionList = [<asp:Literal id='selectedCollectionList' runat='server' />];
		var __EkFolderId = "-1";
        var __TaxonomyOverrideId = 0;
        var AccordionIndex = "<asp:Literal id='szAccordionIndex' runat='server' />";         
		switch (FrameName)
		{
			case("Library"):
			{
				rootFolderName = "<asp:Literal id='genericLibraryTitle' runat='server' />";
				break;
			}
			default:
			{
				rootFolderName = "<asp:Literal id='genericContentTitle' runat='server' />";
				rootTaxonomyName = "<asp:Literal id='labelTaxonomies' runat='server' />";
				rootCollectionName = "<asp:Literal id='genericCollectionName' runat='server' />";
				rootMenuName = "<asp:Literal id='genericMenuTitle' runat='server' />";
			}
		}
    </script>
    <script type="text/javascript">
        //Debug.LEVEL = LogUtil.ALL;
        //LogUtil.logType = LogUtil.LOG_CONSOLE;
        Ektron.ready(function ()
     {
            $ektron("#accordion").accordion({ fillSpace: true });

            // initialize the trees
            Main.start();
            displayTree();
            showSelectedFolderTree();
            
            $ektron("#accordion").accordion('activate', parseInt(AccordionIndex));

            // initialize context menu AppPath property for the context menus
            Ektron.ContextMenus.AppPath = "<asp:Literal id='jsAppPath' runat='server' />";
            Ektron.ContextMenus.Trees.Folder.confirmFolderDelete = "<asp:Literal id='jsConfirmFolderDelete' runat='server' />";
            Ektron.ContextMenus.Trees.Collections.confirmCollectionDelete = "<asp:Literal id='jsConfirmCollectionDelete' runat='server' />";
            Ektron.ContextMenus.Trees.Menus.confirmMenuDelete = "<asp:Literal id='jsConfirmMenuDelete' runat='server' />";
            Ektron.ContextMenus.Trees.Folder.confirmInheritTargetFolder = "<asp:Literal id='jsConfirmBreakInheritance' runat='server' />";
            Ektron.ContextMenus.Trees.Folder.conformTaxonomyDelete = "<asp:Literal id='jsConfirmTaxonomyDelete' runat='server' />";

            // whenever the CMSAPIAjaxComplete event fires,
            // call the context menu Init again to bind to new elements
            $ektron(document).bind("CMSAPIAjaxComplete", function ()
            {
                Ektron.ContextMenus.Trees.Init();
            });

            $ektron("#blockTree").modal(
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
                    hash.o.fadeOut("fast", function ()
                    {
                        if (hash.o)
                        {
                            hash.o.remove();
                        }
                    });
                }
            });
        });

        $ektron.addLoadEvent(function()
        {
            $(window).resize(function(){
		        return adjustAccordionHeight();
            });
            adjustAccordionHeight();
        });
        
         
        function adjustAccordionHeight()
        {
            if ($ektron.browser.msie)
            {
                	var version = parseInt($ektron.browser.version, 10);
				if (version === 7) {
					// IE fires multiple resize events and messes up the height of things as we redraw.
					// Force it to place nice.
					$ektron("form#Form1").height($ektron("body").height()).css("zoom", "1");
					$ektron("#accordion").accordion("resize");
				}
            }
            else
            {
                $ektron("#accordion").accordion("resize");
            }
            return false;
        }

        function fillAccordian(index) 
        {
            if ("undefined" != typeof (toolkit))
             {
                if (index == "menus" && $ektron("#MenuTreeOutput")[0].innerHTML == "")
                    toolkit.getMenus(-99, loadMenus);
                else if (index == "collections" && $ektron("#CollectionTreeOutput")[0].innerHTML == "")
                    toolkit.getCollections(-99, loadCollections);
                else if (index == "taxonomies" && $ektron("#TaxonomyTreeOutput")[0].innerHTML == "")
                    toolkit.getTaxonomies(-99, loadTaxonomies);
            }
        }
        Ektron.ready(function () 
        {          
            if ("undefined" !== typeof (top.Ektron.Workarea.Height)) {
                top.Ektron.Workarea.Height.heightFix(function (height) {
                    $ektron("html, body, iframe").height(height);
                    $ektron("#accordion").accordion("resize");
                });
                top.Ektron.Workarea.Height.execute();
            }
            $ektron("#accordion").accordion("resize");

            if (AccordionIndex == "1") {
                toolkit.getTaxonomies(-99, loadTaxonomies);
                if (g_selectedTaxonomyList.length > 0) {
                    setTimeout(function () { TreeDisplayUtil.toggleTreeSet(g_selectedTaxonomyList, -1) }, 750);
                }
            }
            else if (AccordionIndex == "2") {
                toolkit.getCollections(-99, loadCollections);
                if (g_selectedCollectionList.length > 0) {
                    setTimeout(function () { TreeDisplayUtil.toggleTreeSet(g_selectedCollectionList, -2) }, 750);
                }
            }
            else if (AccordionIndex == "3") {
                toolkit.getMenus(-99, loadMenus);
                if (g_selectedMenuList.length > 0) {
                    setTimeout(function () { TreeDisplayUtil.toggleTreeSet(g_selectedMenuList, -3) }, 750);
                }
            }
        });
    </script>
    <style type="text/css">
        * {
	        margin: 0;
	        padding: 0;
        }

        html, body {
            background: #3f3f3f;
            width:100%;
	        height:100%;
	        overflow: hidden;
	        margin: 0;
	        padding: 0;
        }

        form#Form1 {display: block; position: absolute; top: -1px; left: 0; right: 0; bottom: 0; overflow:hidden; border-right: solid 1px #000000; height: 100%;}
        div#pleaseWait { width: 128px; height: 128px; margin: -64px 0 0 -64px; background-color: #fff; background-image: url("images/ui/loading_big.gif"); background-repeat: no-repeat; text-indent: -10000px; border: none; padding: 0; top: 50%; }
        div#blockTree { border: none; background: none;}
    </style>
</head>
<body>
    <!-- Folder Context Menu -->
    <ul id="folderContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="addFolder">
            <a title="Add Folder" href="#addFolder"><asp:Literal ID="folderContextAddFolder" runat="server" /></a>
        </li>
        <li class="addBlogFolder">
            <a title="Add Blog" href="#addBlogFolder"><asp:Literal ID="folderContextAddBlogFolder" runat="server" /></a>
        </li>
        <li class="addBoardFolder">
            <a title="Add Discussion Board" href="#addBoardFolder"><asp:Literal ID="folderContextAddDiscussionBoard" runat="server" /></a>
        </li>
        <li class="addCommunityFolder">
            <a title="Add Community Folder" href="#addCommunityFolder"><asp:Literal ID="folderContextAddCommunityFolder" runat="server" /></a>
        </li>
        <li class="addCalendarFolder">
            <a title="Add Calender" href="#addCalendarFolder"><asp:Literal ID="folderContextAddCalendarFolder" runat="server" /></a>
        </li>
        <li class="addEcommerceFolder">
            <a title="Add Catalog" href="#addEcommerceFolder"><asp:Literal ID="folderContextAddEcommerceFolder" runat="server" /></a>
        </li>
        <li class="addSiteFolder">
            <a title="Add Site Folder" href="#addSiteFolder"><asp:Literal ID="folderContextAddSiteFolder" runat="server" /></a>
        </li>
        <li class="pasteContent">
            <a title="Paste Content" href="#showConfirmModal"><asp:Literal ID="folderContextPasteContent" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="View Properties" href="#viewFolderProperties"><asp:Literal ID="folderContextViewProperties" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut" href="#cutFolder"><asp:Literal ID="folderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="folderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="folderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteFolder">
            <a title="Delete" href="#deleteFolder"><asp:Literal ID="folderContextDeleteFolder" runat="server" /></a>
        </li>
        <li class="deleteFolderContent">
            <a title="Delete Content from"href="#deleteFolderContent"><asp:Literal ID="folderContextDeleteFolderContent" runat="server" /></a>
        </li>
    </ul>
    <!-- End Folder Context Menu -->
    <!-- Site Folder Context Menu -->
    <ul id="siteFolderContextMenu" class="ektronContextMenu Menu"  oncontextmenu="return false;">
        <li class="addFolder">
            <a title="Add Folder" href="#addFolder"><asp:Literal ID="siteFolderContextAddFolder" runat="server" /></a>
        </li>
        <li class="addBlogFolder">
            <a title="Add Blog"href="#addBlogFolder"><asp:Literal ID="siteFolderContextAddBlogFolder" runat="server" /></a>
        </li>
        <li class="addBoardFolder">
            <a title="Add Discussion Board" href="#addBoardFolder"><asp:Literal ID="siteFolderContextAddDiscussionBoard" runat="server" /></a>
        </li>
        <li class="addCommunityFolder">
            <a title="Add Community Folder" href="#addCommunityFolder"><asp:Literal ID="siteFolderContextAddCommunityFolder" runat="server" /></a>
        </li>
        <li class="addCalendarFolder">
            <a title="Add Calender" href="#addCalendarFolder"><asp:Literal ID="siteFolderContextAddCalendarFolder" runat="server" /></a>
        </li>
        <li class="addEcommerceFolder">
            <a title="Add Catalog" href="#addEcommerceFolder"><asp:Literal ID="siteFolderContextAddEcommerceFolder" runat="server" /></a>
        </li>
        <li class="pasteContent">
            <a title="Paste Content" href="#showConfirmModal"><asp:Literal ID="siteFolderContextPasteContent" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="View Properties" href="#viewFolderProperties"><asp:Literal ID="siteFolderContextViewProperties" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut" href="#cutFolder"><asp:Literal ID="siteFolderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="siteFolderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="siteFolderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteSiteFolder">
            <a title="Delete" href="#deleteFolder"><asp:Literal ID="siteFolderContextDeleteFolder" runat="server" /></a>
        </li>
        <li class="deleteFolderContent">
            <a title="Delete" href="#deleteFolderContent"><asp:Literal ID="siteFolderContextDeleteFolderContent" runat="server" /></a>
        </li>
    </ul>
    <!-- End Site Folder Context Menu -->
    <!-- Blog Folder Context Menu -->
    <ul id="blogFolderContextMenu" class="ektronContextMenu Menu"  oncontextmenu="return false;">
        <li class="viewProperties">
            <a title="Properties" href="#viewFolderProperties"><asp:Literal ID="blogFolderContextViewProperties" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut" href="#cutFolder"><asp:Literal ID="blogFolderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="blogFolderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="blogFolderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteBlogFolder">
            <a title="Delete Blog" href="#deleteFolder"><asp:Literal ID="blogFolderContextDeleteBlog" runat="server" /></a>
        </li>
        <li class="deleteBlogContent">
            <a title="Delete Blog Content" href="#deleteFolderContent"><asp:Literal ID="blogFolderContextDeleteBlogPosts" runat="server" /></a>
        </li>
    </ul>
    <!-- End Blog Folder Context Menu -->
    <!-- Community Folder Context Menu -->
    <ul id="communityFolderContextMenu" class="ektronContextMenu Menu"  oncontextmenu="return false;">
        <li class="addBlogFolder">
            <a title="Add Blog Folder" href="#addBlogFolder"><asp:Literal ID="communityFolderContextAddBlog" runat="server" /></a>
        </li>
        <li class="addBoardFolder">
            <a title="Add Board Folder" href="#addBoardFolder"><asp:Literal ID="communityFolderContextAddBoard" runat="server" /></a>
        </li>
        <li class="addCommunityFolder">
            <a title="Add Community Folder" href="#addCommunityFolder"><asp:Literal ID="communityFolderContextAddCommunityFolder" runat="server" /></a>
        </li>
        <li class="addCalendarFolder">
            <a title="Add Calender Folder" href="#addCalendarFolder"><asp:Literal ID="communityFolderContextAddCalendarFolder" runat="server" /></a>
        </li>
        <li class="addEcommerceFolder">
            <a title="Add Discussion Board" href="#addEcommerceFolder"><asp:Literal ID="communityFolderContextAddEcommerceFolder" runat="server" /></a>
        </li>
        <li class="pasteContent">
            <a title="Paste" href="#showConfirmModal"><asp:Literal ID="communityFolderContextPasteContent" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="Properties" href="#viewFolderProperties"><asp:Literal ID="communityFolderContextViewProperties" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut" href="#cutFolder"><asp:Literal ID="communityFolderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="communityFolderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="communityFolderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteCommunityFolder">
            <a title="Delete Community Folder" href="#deleteFolder"><asp:Literal ID="communityFolderContextDeleteFolder" runat="server" /></a>
        </li>
        <li class="deleteFolderContent">
            <a title="Delete Folder Content" href="#deleteFolderContent"><asp:Literal ID="communityFolderContextDeleteFolderContent" runat="server" /></a>
        </li>
    </ul>
    <!-- End Community Folder Context Menu -->
    <!-- Disscussion Board Folder Context Menu -->
    <ul id="discussionBoardFolderContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="addDiscussionForum">
            <a title="Add Discussion Forum" href="#addDiscussionForum"><asp:Literal ID="boardFolderContextAddDiscussionForum" runat="server" /></a>
        </li>
        <li class="addSubject">
            <a title="Add Subject" href="#addSubject"><asp:Literal ID="boardFolderContextAddSubject" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="Properties" href="#viewBoardProperties"><asp:Literal ID="boardFolderContextViewProperties" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut" href="#cutFolder"><asp:Literal ID="boardFolderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="boardFolderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="boardFolderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteBoardFolder">
            <a title="Delete Board Folder" href="#deleteFolder"><asp:Literal ID="boardFolderContextDeleteBoard" runat="server" /></a>
        </li>
    </ul>
    <!-- End Disscussion Board Folder Context Menu -->
    <!-- Disscussion Forum Folder Context Menu -->
    <ul id="discussionForumFolderContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="add">
            <a title="Add" href="#addForumTopic"><asp:Literal ID="forumFolderContextAddTopic" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="Properties" href="#viewForumProperties"><asp:Literal ID="forumFolderContextViewProperties" runat="server" /></a>
        </li>
        <li class="viewPermissions">
            <a title="Permissions" href="#viewForumPermissions"><asp:Literal ID="forumFolderContextViewPermissions" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut" href="#cutFolder"><asp:Literal ID="forumFolderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="forumFolderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="forumFolderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteBoardFolder">
            <a title="Delete Board" href="#deleteForum"><asp:Literal ID="forumFolderContextDeleteForum" runat="server" /></a>
        </li>
    </ul>
    <!-- End Disscussion Forum Folder Context Menu -->
    <!-- Ecommerce Folder Context Menu -->
    <ul id="ecommerceFolderContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="addEcommerceFolder">
            <a title="Add Catalog" href="#addEcommerceFolder"><asp:Literal ID="ecommerceContentAddFolder" runat="server" /></a>
        </li>
        <li class="pasteContent">
            <a title="Paste" href="#showConfirmModal"><asp:Literal ID="ecommercePasteCatalogEntry" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="Properties" href="#viewFolderProperties"><asp:Literal ID="ecommerceContentViewProperties" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut"href="#cutFolder"><asp:Literal ID="ecommerceFolderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="ecommerceFolderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="ecommerceFolderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteEcommerceFolder">
            <a title="Delete Catalog" href="#deleteFolder"><asp:Literal ID="ecommerceContentDeleteFolder" runat="server" /></a>
        </li>
        <li class="deleteEcommerceContent">
            <a title="Delete Catalog Content" href="#deleteFolderContent"><asp:Literal ID="ecommerceContextDeleteContent" runat="server" /></a>
        </li>
    </ul>
    <!-- End Ecommerce Folder Context Menu -->
    <!-- Calendar Context Menu -->
    <ul id="calendarFolderContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="viewProperties">
            <a title="Properties" href="#viewFolderProperties"><asp:Literal ID="calendarViewProperties" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutFolder">
            <a title="Cut" href="#cutFolder"><asp:Literal ID="calendarFolderContextCutFolder" runat="server" /></a>
        </li>
        <li class="copyFolder">
            <a title="Copy" href="#copyFolder"><asp:Literal ID="calendarFolderContextCopyFolder" runat="server" /></a>
        </li>
        <li class="pasteFolder">
            <a title="Paste" href="#pasteFolder"><asp:Literal ID="calendarFolderContextPasteFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteCalendarFolder">
            <a title="Delete Calandar" href="#deleteFolder"><asp:Literal ID="calendarDeleteFolder" runat="server" /></a>
        </li>
    </ul>
    <!-- End Site Folder Context Menu -->
    <!-- Collections Context Menu -->
    <ul id="collectionsContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="add">
            <a title="Add" href="#addCollection"><asp:Literal ID="collectionContextAddCollection" runat="server" /></a>
        </li>
        <li class="addContent">
            <a title="Add Content" href="#addCollectionItems"><asp:Literal ID="collectionContextAdd" runat="server" /></a>
        </li>
        <li class="remove">
            <a title="Remove" href="#removeCollection"><asp:Literal ID="collectionContextRemove" runat="server" /></a>
        </li>
        <li class="reorder">
            <a title="Reorder" href="#reorderCollection"><asp:Literal ID="collectionContextReorder" runat="server" /></a>
        </li>
        <li class="pasteContent">
            <a title="Paste Content" href="#collectionAssignSelectedItems"><asp:Literal ID="collectionContextAssignSelectedItems" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="View Properties" href="#viewCollectionProperties"><asp:Literal ID="collectionContextView" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="delete">
            <a title="Delete" href="#deleteCollection"><asp:Literal ID="collectionContextDelete" runat="server" /></a>
        </li>
    </ul>
    <!-- End Collections Folder Context Menu -->
    <!-- Taxonomy Context Menu -->
    <ul id="taxonomyContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="add">
            <a title="Add" href="#addTaxonomy"><asp:Literal ID="taxonomyAdd" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="View Properties" href="#viewTaxonomyProperties"><asp:Literal ID="taxonomyContextView" runat="server" /></a>
        </li>
        <li class="assignItems">
            <a title="Assign Items" href="#taxonomyAddContent"><asp:Literal ID="taxonomyAddContent" runat="server" /></a>
        </li>
        <li class="pasteContent">
            <a title="Paste Content" href="#taxonomyAssignSelectedItems"><asp:Literal ID="taxonomyAssign" runat="server" /></a>
        </li>
        <li class="assignFolders">
            <a title="Assign Folders" href="#taxonomyAddFolders"><asp:Literal ID="taxonomyAddFolder" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="cutTaxonomy">
            <a title="Cut Taxonomy" href="#cutTaxonomy"><asp:Literal ID="taxonomyContextCut" runat="server" /></a>
        </li>
        <li class="copyTaxonomy">
            <a title="Copy Taxonomy" href="#copyTaxonomy"><asp:Literal ID="taxonomyContextCopy" runat="server" /></a>
        </li>
        <li class="pasteTaxonomy">
            <a title="Paste Taxonomy" href="#pasteTaxonomy"><asp:Literal ID="taxonomyContextPaste" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="deleteTaxonomy">
            <a title="Delete Taxonomy" href="#deleteTaxonomy"><asp:Literal ID="taxonomyContextDelete" runat="server" /></a>
        </li>
    </ul>
    <!-- End Taxonomy Folder Context Menu -->
    <!-- Menus Context Menu -->
    <ul id="menutreeContextMenu" class="ektronContextMenu Menu" oncontextmenu="return false;">
        <li class="add">
            <a title="Add" href="#addMenu"><asp:Literal ID="menuAdd" runat="server" /></a>
        </li>
        <li class="addContent">
            <a title="Add Content" href="#addMenuItems"><asp:Literal ID="menuContentAdd" runat="server" /></a>
        </li>
        <li class="remove">
            <a title="Remove" href="#removeMenuItems"><asp:Literal ID="menuRemoveItems" runat="server" /></a>
        </li>
        <li class="reorder">
            <a title="Reorder" href="#reorderMenu"><asp:Literal ID="menuContextReorder" runat="server" /></a>
        </li>
        <li class="pasteContent">
            <a title="Paste Content" href="#menuAssignSelectedItems"><asp:Literal ID="menuContextAssignSelectedItems" runat="server" /></a>
        </li>
        <li class="viewProperties">
            <a title="Add Properties" href="#viewMenuProperties"><asp:Literal ID="menuContextView" runat="server" /></a>
        </li>
        <li class="separator"></li>
        <li class="delete">
            <a title="Delete" href="#deleteMenu"><asp:Literal ID="menuContextDelete" runat="server" /></a>
        </li>
    </ul>
    <!-- End Menus Context Menu -->
    <form id="Form1" method="post" runat="server">
        <div id="blockTree" class="ektronWindow">
        </div>
        <div id="accordion">
            <h3 class="FolderTitle">
                <a title="<%=m_refMsgApi.GetMessage("Generic Folder Title")%>" href="#"><%=m_refMsgApi.GetMessage("generic content title")%></a>
            </h3>
            <div>
                <div id="TreeOutput" class="ektronTreeContainer">
                </div>
            </div>
            <asp:Placeholder ID="plContentTrees" Visible="false" runat="server">
                <asp:PlaceHolder ID="plTaxonomyTree" runat="server">
                    <h3 class="TaxonomyTitle">
                        <a title="<%=m_refMsgApi.GetMessage("taxonomytitle")%>" href="#" onclick="fillAccordian('taxonomies');" ><%=m_refMsgApi.GetMessage("lbl taxonomies")%></a>
                    </h3>
                    <div>
                        <div id="TaxonomyTreeOutput" class="ektronTreeContainer"></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plCollectionTree" runat="server" >
                    <h3 class="CollectionTitle">
                        <a title="<%=m_refMsgApi.GetMessage("collectiontitle")%>" href="#" onclick="fillAccordian('collections');" ><%=m_refMsgApi.GetMessage("generic collection title")%></a>
                    </h3>
                    <div>
                        <div id="CollectionTreeOutput" class="ektronTreeContainer"></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plMenuTree" runat="server" >
                    <h3 class="MenuTitle">
                        <a title="<%=m_refMsgApi.GetMessage("menutitle")%>" href="#"  onclick="fillAccordian('menus');" ><%=m_refMsgApi.GetMessage("generic menu title")%></a>
                    </h3>
                    <div>
                        <div id="MenuTreeOutput" class="ektronTreeContainer"></div>
                    </div>
                </asp:PlaceHolder>
            </asp:Placeholder>
        </div>
        <input type="hidden" id="folderName" name="folderName" />
        <input type="hidden" id="selected_folder_id" name="selected_folder_id" value="0" />
        <input type="hidden" id="contLanguage" name="contLanguage" runat="server" />
    </form>
</body>
</html>


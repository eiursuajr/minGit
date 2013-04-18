<%@ Page Language="C#" AutoEventWireup="true" Inherits="MenuTree" CodeFile="MenuTree.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MenuTree</title>
    <input id="StyleSheetJS" runat="server" />
</head>
<body onload="javascript:pageLoaded();">

    <script type="text/javascript" language="javascript">
//Debug.LEVEL = LogUtil.WARNING;
//LogUtil.logType = LogUtil.LOG_CONSOLE;
function pageLoaded()
{
	setTimeout("showSelectedFolderTree();", 100);
}
    </script>

    <form name="netscapefix" method="post" action="MenuTree.aspx" id="Form3">
        <div id="FrameContainer" class="ektronBorder" style="position: absolute; top: 48px;
            left: 20px; width: 1px; height: 1px; display: none; z-index: 1000;">
            <iframe id="ChildPage" name="ChildPage" frameborder="1" marginheight="0" marginwidth="0"
                width="100%" height="100%" scrolling="auto"></iframe>
        </div>
        <input type="hidden" name="CollectionID" id="CollectionID" value="" />
        <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
        <input type="hidden" name="ClickRootMenu" id="ClickRootMenu" value="false" />
        <input type="hidden" name="ClickType" id="ClickType" value="parent" />
        <input type="hidden" name="frm_content_ids" id="frm_content_ids" value="" />
        <div id="dhtmltooltip">
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div style="position: relative;">
            <span style="position: absolute" id='MenuTree'></span>
        </div>
    </form>

    <script type="text/javascript" language="javascript">

//////////
//
// The click handlers for the trees. These should be placed
// in an external file, I'm just throwing them in here for now,
// since we're only using the tree in once place.
//

var clickedElementPrevious = null;
var clickedIdPrevious = null;

function onContextMenuHandler( id, clickedElement )
{
	/* todo: create a 'highlight node' function
	if( clickedElementPrevious != null ) {
		clickedElementPrevious.style["background"] = "#ffffff";
		clickedElementPrevious.style["color"] = "#000000";
	}

	clickedElement.style["background"] = "#3366CC";
	clickedElement.style["color"] = "#ffffff";
	clickedElementPrevious = clickedElement;
	clickedIdPrevious = id;

	ContextMenuUtil.use( event, "treeMenu", clickedElement );
	*/

	return false;
}

function onDragEnterHandler( id, element )
{
	folderID = id;

	// todo: create a 'highlight node' function
	if( clickedElementPrevious != null ) {
		clickedElementPrevious.style["background"] = "#ffffff";
		clickedElementPrevious.style["color"] = "#000000";
	}

	element.style["background"] = "#3366CC";
	element.style["color"] = "#ffffff";
}

//setInterval( function() { alert( folderID ) } , 5000 ) ;
function onMouseOverHandler( id, element )
{
	element.style["background"] = "#ffffff";
	element.style["color"] = "#000000";
}

function onDragLeaveHandler( id, element )
{
	element.style["background"] = "#ffffff";
	element.style["color"] = "#000000";
}

function onFolderClick( id, clickedElement )
{
	// todo: create a 'highlight node' function
	if( clickedElementPrevious != null ) {
		//if( clickedIdPrevious == id ) {
		//	return;
		//}
		clickedElementPrevious.style["background"] = "#ffffff";
		clickedElementPrevious.style["color"] = "#000000";
	}

	clickedElement.style["background"] = "#3366CC";
	clickedElement.style["color"] = "#ffffff";
	clickedElementPrevious = clickedElement;
	clickedIdPrevious = id;

	var name = clickedElement.innerText;
	var folder = new Asset();
	folder.set( "name", name );
	folder.set( "id", id );

	//Explorer.setWorkingFolder( folder );
	//SearchManager.execute( "id=" + id );
}

function onToggleClick( id, callback, args )
{
	toolkit.getChildMenus( id, callback, args );
}

function makeElementEditable( element )
{
	element.contentEditable = true;
	element.focus();
	element.style.background = "#fff";
	element.style.color = "#000";
}

function LoadLanguage(inVal) {
		if(inVal=='0') { return false ; }
		top.notifyLanguageSwitch(inVal);
		// TODO: Use RegEx to replace the querystring
		document.location = '<%=Request.ServerVariables["PATH_INFO"] + "?" + Ektron.Cms.Common.EkFunctions.HtmlEncode(Request.ServerVariables["QUERY_STRING"].ToString().Replace("LangType", "L"))%>&LangType=' + inVal ;
	}

	function addBaseMenu(menuID, parentID, ancestID, foldID, langID) {
		document.location = 'collections.aspx?action=AddTransMenu&nId=' + menuID + '&backlang=<%=ContentLanguage%>&LangType=' + langID + '&folderid=' + foldID + '&ancestorid=' + ancestID + '&parentid=' + parentID   ;
	}

//////////
//
// Define the default images for the tree
//

var baseUrl = URLUtil.getAppRoot(document.location) + "images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder  = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.plusopenfolder   = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "folderExpanded.png";
TreeDisplayUtil.minusopenfolder  = baseUrl + "folderExpanded.png";
TreeDisplayUtil.folder = baseUrl + "folder.png";

var g_menu_id = "";
function displayMenu( menuRoot )
{
	document.body.style.cursor = "default";
	var menuName = null;
	try {
		menuName = menuRoot.title;
		g_menu_id = menuRoot.id;
	} catch( e ) {
		;
	}

	if( menuName != null ) {
		// Start with the root of the menu, via the AncestorMenuId:
		treeRoot = new Tree( menuName, <%=AncestorMenuId%>, null, menuRoot );
		TreeDisplayUtil.showSelf( treeRoot, document.getElementById( "MenuTree" ) );
		TreeDisplayUtil.toggleTree( treeRoot.node.id );
	} else {
		var element = document.getElementById( "MenuTree" );
		var debugInfo = "<b>Cannot connect to the service</b>";
		element.innerHTML = debugInfo;
	}
}

var toolkit = new EktronToolkit();
// Start with the root of the menu, via the AncestorMenuId:
toolkit.getMenu( <%=AncestorMenuId%>, displayMenu, 0 );

function reloadTreeRoot( id )
{
	// clear out existing tree
	TREES = {};
	toolkit.getMenu( id, displayMenu, 0 );
}

var g_selectedFolderList = "<%=m_selectedFolderList%>";
var g_timerForFolderTreeDisplay;
function showSelectedFolderTree()
{
    if (g_timerForFolderTreeDisplay)
	{
		window.clearTimeout(g_timerForFolderTreeDisplay);
	}

	g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
}

function showSelectedFolderTree_delayed()
{
	var bSuccessFlag = false;

	if (g_timerForFolderTreeDisplay)
	{
		window.clearTimeout(g_timerForFolderTreeDisplay);
	}

	if (g_selectedFolderList.length > 0)
	{
		var tree = TreeUtil.getTreeById(g_menu_id);
		if (tree)
		{
			var lastId = 0;
			var folderList = g_selectedFolderList.split(",");
			bSuccessFlag = TreeDisplayUtil.expandTreeSet( folderList );
		}

		if (!bSuccessFlag)
		{
			g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
			//alert('showSelectedFolderTree_delayed retrying...');
		}
	}
}

    </script>

    <script type="text/javascript" language="javascript">

// This legacy function appears to be unnecessary; commented out for now - remove later:
//function onMenuItemEdit(ItemId, MenuId, ItemType)
//{
//	var lastClickedOn = document.getElementById("LastClickedOn");
//	lastClickedOn.value= MenuId;
//	var clickType = document.getElementById("ClickType");
//	clickType.value= "parent";
//
//	LoadChildPage("submenuedititem", ItemId, MenuId, ItemType,"","");
//}

// This legacy function appears to be unnecessary; commented out for now - remove later:
//function onMenuItemDelete(ItemId, MenuId, ItemType, FolderID)
//{
//	var lItemType;
//	if (confirm("Are you sure you want to delete this item?"))
//	{
//		document.forms.netscapefix.frm_content_ids.value = ItemId + '.' + 0;
//		document.forms.netscapefix.CollectionID.value = MenuId;
//		document.forms.netscapefix.action = "collectionaction.aspx?iframe=true&action=doDeleteMenuItem&folderid=" +					FolderID + "&nid=" + MenuId;
//		document.forms.netscapefix.submit();
//	}
//	return false;
//}

function onSubMenuAddItem(MenuId, FolderId, ItemId, Type, AncestorID)
{
	if (MenuId == "")
	{
		alert("Please select a menu.");
		return false;
	}
	var lastClickedOn = document.getElementById("LastClickedOn");
	lastClickedOn.value= MenuId;

	var clickType = document.getElementById("ClickType");
	clickType.value= "self";

	LoadChildPage("submenuadditem", ItemId, MenuId, "", FolderId, AncestorID);
}

function onSubMenuEdit(MenuId, FolderId, ItemID, Type, AncestorID)
{
	var tAction = "submenuedit"
	var clickRootMenu = document.getElementById("ClickRootMenu");
	var clickType = document.getElementById("ClickType");
	if(Type != "Submenu" && Type != "Menu")
	{
		tAction = "submenuedititem";
		clickType.value= "self";
	}
	else
		clickType.value= "parent";

	if (Type == "Menu")
	{
	    clickRootMenu.value = "true";
	}

	var lastClickedOn = document.getElementById("LastClickedOn");
	lastClickedOn.value= MenuId;

	LoadChildPage(tAction, ItemID, MenuId, "",FolderId,AncestorID);
}

function onSubMenuDelete(MenuId, FolderID, ItemID, Type, ParentID)
{
	var frameObj = document.getElementById("ChildPage");
	if (confirm('<%=m_refMsg.GetMessage("alt are you sure you want to delete this item?")%>'))
	{
		if(Type != "Submenu" && Type != "Menu")
		{
			var lastClickedOn = document.getElementById("LastClickedOn");
			lastClickedOn.value= MenuId;

			var clickType = document.getElementById("ClickType");
			clickType.value= "self";

			frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=doDeleteMenuItem&nid=" + MenuId + "&pid=" + ParentID + "&frm_content_ids=" + ItemID + '.' + 0 + MenuId + "&CollectionID=" + MenuId ;
		}
		else if(Type == "Menu")
		{
			document.forms.netscapefix.frm_content_ids.value = MenuId + '.' + 4 + '.' + MenuId;
				document.forms.netscapefix.CollectionID.value = MenuId;
				document.forms.netscapefix.action = "collections.aspx?iframe=true&action=doDeleteMenu&folderid=" + FolderID + "&nid=" + MenuId ;
				document.forms.netscapefix.submit();
		}
		else
		{
			var lastClickedOn = document.getElementById("LastClickedOn");
			lastClickedOn.value= MenuId;

			var clickType = document.getElementById("ClickType");
			clickType.value= "parent";
			frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=doDeleteMenuItem&nid=" +								MenuId +  "&pid=" + ParentID + "&frm_content_ids=" + MenuId + '.' + 4 + '.' +										MenuId + "&CollectionID=" + MenuId ;
		}
	}
}

function onSubMenuOrderItem(MenuId, FolderId, ItemID, Type)
{
	var lastClickedOn = document.getElementById("LastClickedOn");
	lastClickedOn.value= MenuId;

	var clickType = document.getElementById("ClickType");
	clickType.value= "self";

	LoadChildPage("submenuorderitem", ItemID, MenuId, "",FolderId,"");

}

function LoadChildPage(tAction, ItemId, MenuId, ItemType, FolderId, AncestorId, QDContentOnly)
{
	var frameObj = document.getElementById("ChildPage");
	if (tAction == "submenuedit")
	{
		frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=EditMenu&nid=" + MenuId + "&folderid=" + FolderId + "&Ty=" + ItemType;
	}
	else if (tAction == "submenuedititem")
	{
			frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=EditMenuItem&nid=" + MenuId + "&folderid=" + FolderId + "&id=" + ItemId + '&Ty=' + ItemType;
	}
	else if (tAction == "submenuadditem")
	{
		    var url = "blankredirect.aspx?collections.aspx?iframe=true&action=AddMenuItem&nid=" + MenuId + "&folderid=" + FolderId + "&parentid=" + MenuId + "&ancestorid=" + AncestorId;
		    if ((QDContentOnly != undefined) && (QDContentOnly != null) && (QDContentOnly != '')) {
		        url = url + '&qdo=1';
		    }
			frameObj.src = url;
	}
	else if (tAction == "submenuorderitem")
	{
				frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=ReOrderMenuItems&nid=" + MenuId + "&folderid=" + FolderId;
	}



	var pageObj = document.getElementById("FrameContainer");
	pageObj.style.display = "";
	pageObj.style.width = "95%";
	pageObj.style.height = "95%";
}

function CancelIframe()
{
	var pageObj = document.getElementById("FrameContainer");
	pageObj.style.display = "none";
	pageObj.style.width = "1px";
	pageObj.style.height = "1px";

}
function CloseChildPage()
{
	var pageObj = document.getElementById("FrameContainer");
	pageObj.style.display = "none";
	pageObj.style.width = "1px";
	pageObj.style.height = "1px";

	var lastClickedOn = document.getElementById("LastClickedOn");

	var clickType = document.getElementById("ClickType");
	var clickRootMenu = document.getElementById("ClickRootMenu");
	if(clickType.value == "self") {
		TreeDisplayUtil.reloadTree(lastClickedOn.value);
	} else {
		TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
    }
    if (clickRootMenu.value == "true")
    {
		window.location.reload(true);
    }
}

    </script>

    <script language="javascript" type="text/javascript">
/*
	New code for the hover-over menu,
	for the workarea menu-maintenence UI:
*/

var g_delayedHideTimer = null;
var g_delayedHideTime = 1000;
var g_wamm_float_menu_treeid = -1;
var g_isIeInit = false;
var g_isIeFlag = false;

function IsBrowserIE()
{
	if (!g_isIeInit)
	{
		var ua = window.navigator.userAgent.toLowerCase();
		g_isIeFlag = (ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1));
		g_isIeInit = true;
	}
	return (g_isIeFlag);
}

function showWammFloatMenuForMenuNode(show, delay, event, treeId)
{
	var el = document.getElementById("wamm_float_menu_block_menunode");
	if (el)
	{
		if (g_delayedHideTimer)
		{
			clearTimeout(g_delayedHideTimer);
			g_delayedHideTimer = null;
		}

		if (show)
		{
			el.style.display = "none";
			showWammFloatMenuForMenuItemNode(false, false, null, -1);
			if (null != event)
			{
				el.style.left = (20 + getShiftedEventX(event)) + "px"
				if (IsBrowserIE())
				{
					el.style.top = (48 + getEventY(event)) + "px"
				}
				else
				{
					el.style.top = (getEventY(event)) + "px"
				}
				el.style.display = "";
				if ((typeof(treeId) !== "undefined") && (treeId != null))
				{
					g_wamm_float_menu_treeid = treeId;
				}
				else
				{
					g_wamm_float_menu_treeid = -1;
				}
			}
		}
		else
		{
			if (delay)
			{
				g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
			}
			else
			{
				el.style.display = "none";
			}
		}
	}
}

function getEventX(event)
{
	var xVal;
	if (IsBrowserIE())
	{
		xVal = event.x;
	}
	else
	{
		xVal = event.pageX;
	}

	return(xVal)
}

function getShiftedEventX(event)
{
	var srcLeft;
	var xVal;
	if (IsBrowserIE())
	{
		xVal = event.x;
	}
	else
	{
		xVal = event.pageX;
	}

	// attempt to shift div-tag to the right of the menu items:
	srcLeft = xVal;
	if (event.srcElement && event.srcElement.offsetLeft){
		srcLeft = event.srcElement.offsetLeft;
	}
	else if (event.target && event.target.offsetLeft){
		srcLeft = event.target.offsetLeft;
	}

	if (event.srcElement) {
		if (event.srcElement.offsetWidth) {
			xVal = srcLeft + event.srcElement.offsetWidth;
		}
		else if (event.srcElement.scrollWidth) {
			xVal = srcLeft + event.srcElement.scrollWidth;
		}
	}
	else if (event.target && event.target.offsetLeft){
		if (event.target.offsetWidth) {
			xVal = srcLeft + event.target.offsetWidth;
		}
		else if (event.target.scrollWidth) {
			xVal = srcLeft + event.target.scrollWidth;
		}
	}

	return(xVal)
}

function getEventY(event)
{
	var yVal;
	if (IsBrowserIE())
	{
		yVal = event.y;
	}
	else
	{
		yVal = event.pageY;
	}
	return(yVal)
}

function showWammFloatMenuForMenuItemNode(show, delay, event, treeId)
{
	var el = document.getElementById("wamm_float_menu_block_menuitemnode");
	if (el)
	{
		if (g_delayedHideTimer)
		{
			clearTimeout(g_delayedHideTimer);
			g_delayedHideTimer = null;
		}

		if (show)
		{
			el.style.display = "none";
			showWammFloatMenuForMenuNode(false, false, null, -1);
			if (null != event)
			{
				el.style.left = (20 + getShiftedEventX(event)) + "px"
				if (IsBrowserIE())
				{
					el.style.top = (48 + getEventY(event)) + "px"
				}
				else
				{
					el.style.top = (getEventY(event)) + "px"
				}
				el.style.display = "";
				if ((typeof(treeId) !== "undefined") && (treeId != null))
				{
					g_wamm_float_menu_treeid = treeId;
				}
				else
				{
					g_wamm_float_menu_treeid = -1;
				}
			}
		}
		else
		{
			if (delay)
			{
				g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuItemNode(false, false, null, -1)", g_delayedHideTime);
			}
			else
			{
				el.style.display = "none";
			}
		}
	}
}

 function wamm_float_menu_block_mouseover(obj)
 {
	if (g_delayedHideTimer)
	{
		clearTimeout(g_delayedHideTimer);
		g_delayedHideTimer = null;
	}
 }

 function wamm_float_menu_block_mouseout(obj)
 {
	if (null != obj){
		if ("wamm_float_menu_block_menunode" == obj.id)
		{
			g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
		}
		else
		{
			g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuItemNode(false, false, null, -1)", g_delayedHideTime);
		}
	}
 }

function routeAction(containerFlag, op)
{
	var tree = null;
	if ((typeof(g_wamm_float_menu_treeid) !== "undefined") && (g_wamm_float_menu_treeid != null))
	{
		tree = TreeUtil.getTreeById(g_wamm_float_menu_treeid);
	}

	if (tree && tree.node && tree.node.data)
	{
		var menuId = tree.node.data.menuID;
		var folderId = tree.node.data.folderID;
		var itemId = tree.node.data.itemID;
		var itemType = tree.node.data.itemType;
		var ancestorId = tree.node.data.ancestorID;
		var parentId = tree.node.pid;

		if (containerFlag)
		{
			// Hide the floating menu:
			showWammFloatMenuForMenuNode(false, false, null, -1);
		}
		else
		{
			//****************************************************
			//
			// ATTENTION - IMPORTANT NOTE
			//
			// For menu items (non menus/submenus) the server/ajax
			// returns the menuId and the itemId REVERSED!!!
			// We'll swap them here to compensate!
			var temp = menuId;
			menuId = itemId;
			itemId = temp;
			//****************************************************

			// Hide the floating menu:
			showWammFloatMenuForMenuItemNode(false, false, null, -1);
		}

		switch (op)
		{
			case "Add":
				onSubMenuAddItem(menuId, folderId, itemId, itemType, ancestorId);
				break;

			case "Delete":
				onSubMenuDelete(menuId, folderId, itemId, itemType, parentId);
				break;

			case "Reorder":
				onSubMenuOrderItem(menuId, folderId, itemId, itemType);
				break;

			case "Edit":
				onSubMenuEdit(menuId, folderId, itemId, itemType, ancestorId);
				break;

			default :
				break;
		}
	}
}
    </script>

    <div id="wamm_float_menu_block_menunode" class="Menu" style="position: absolute;
        left: 10px; top: 10px; display: none; z-index: 3200;" onmouseover="wamm_float_menu_block_mouseover(this)"
        onmouseout="wamm_float_menu_block_mouseout(this)">
        <ul>
            <li class="MenuItem add"><a href="#" title="Add" onclick="routeAction(false, 'Add');">
                <%=m_refMsg.GetMessage("generic add title")%>
            </a></li>
            <li class="MenuItem reorder"><a href="#" title="Reorder" onclick="routeAction(false, 'Reorder');">
                <%=m_refMsg.GetMessage("btn reorder")%>
            </a></li>
            <li class="MenuItem edit"><a href="#" title="Edit" onclick="routeAction(false, 'Edit');">
                <%=m_refMsg.GetMessage("generic edit title")%>
            </a></li>
            <li class="break" />
            <li class="MenuItem delete"><a href="#" title="Delete" onclick="routeAction(false, 'Delete');">
                <%=m_refMsg.GetMessage("generic delete title")%>
            </a></li>
        </ul>
    </div>
    <div id="wamm_float_menu_block_menuitemnode" class="Menu" style="position: absolute;
        left: 10px; top: 10px; display: none; z-index: 3200;" onmouseover="wamm_float_menu_block_mouseover(this)"
        onmouseout="wamm_float_menu_block_mouseout(this)">
        <ul>
            <li class="MenuItem edit"><a href="#" title="Edit" onclick="routeAction(false, 'Edit');">
                <%=m_refMsg.GetMessage("generic edit title")%>
            </a></li>
            <li class="break" />
            <li class="MenuItem delete"><a href="#" title="Delete" onclick="routeAction(false, 'Delete');">
                <%=m_refMsg.GetMessage("generic delete title")%>
            </a></li>
        </ul>
    </div>
</body>
</html>
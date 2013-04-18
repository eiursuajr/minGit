//////////
//
// override the default images for the tree
//

//TreeDisplayUtil.plusclosefolder  = "images/ui/icons/tree/folderCollapsed.png";
//TreeDisplayUtil.plusopenfolder   = "images/ui/icons/tree/plusopenfolder.png";
//TreeDisplayUtil.minusclosefolder = "images/ui/icons/tree/folderExpanded.png";
//TreeDisplayUtil.minusopenfolder  = "images/ui/icons/tree/minusOpenFolder.png";
//TreeDisplayUtil.folder = "images/ui/icons/tree/folder.png";

// 0 - normal folders
// 1 - blogs
// 2 - site
// 3 - discussion board
// 4 - discussion forum
// 5 - root
// 6 - community
// 7 - media
// 8 - calendar
// 9 - commerce
// 10 - taxonomy
// 11 - collection
// 12 - menu

var pcfarray = new Array(10);
pcfarray[0] = "images/ui/icons/tree/folderCollapsed.png";
pcfarray[1] = "images/ui/icons/tree/folderBlogCollapsed.png";
pcfarray[2] = "images/ui/icons/tree/folderSiteCollapsed.png";
pcfarray[3] = "images/ui/icons/tree/folderBoardCollapsed.png";
pcfarray[4] = "images/ui/icons/tree/folderBoardCollapsed.png";
pcfarray[5] = "images/ui/icons/tree/homeCollapsed.png";
pcfarray[6] = "images/ui/icons/tree/folderCommunityCollapsed.png";
pcfarray[7] = "images/ui/icons/tree/folderFilmCollapsed.png";
pcfarray[8] = "images/ui/icons/tree/folderCalendarCollapsed.png";
pcfarray[9] = "images/ui/icons/tree/folderGreenCollapsed.png";
pcfarray[10] = "images/ui/icons/tree/taxonomyCollapsed.png";
pcfarray[11] = "images/ui/icons/tree/collectionCollapsed.png";
pcfarray[12] = "images/ui/icons/tree/menuCollapsed.png";
TreeDisplayUtil.plusclosefolders = pcfarray;

var mcfarray = new Array(10);
mcfarray[0] = "images/ui/icons/tree/folderExpanded.png";
mcfarray[1] = "images/ui/icons/tree/folderBlogExpanded.png";
mcfarray[2] = "images/ui/icons/tree/folderSiteExpanded.png";
mcfarray[3] = "images/ui/icons/tree/folderBoardExpanded.png";
mcfarray[4] = "images/ui/icons/tree/folderBoardExpanded.png";
mcfarray[5] = "images/ui/icons/tree/homeExpanded.png";
mcfarray[6] = "images/ui/icons/tree/folderCommunityExpanded.png";
mcfarray[7] = "images/ui/icons/tree/folderFilmExpanded.png";
mcfarray[8] = "images/ui/icons/tree/folderCalendarExpanded.png";
mcfarray[9] = "images/ui/icons/tree/folderGreenExpanded.png";
mcfarray[10] = "images/ui/icons/tree/taxonomyExpanded.png";
mcfarray[11] = "images/ui/icons/tree/collectionExpanded.png";
mcfarray[12] = "images/ui/icons/tree/menuExpanded.png";
TreeDisplayUtil.minusclosefolders = mcfarray;

var farray = new Array(10);
farray[0] = "images/ui/icons/tree/folder.png";
farray[1] = "images/ui/icons/tree/folderBlog.png";
farray[2] = "images/ui/icons/tree/folderSite.png";
farray[3] = "images/ui/icons/tree/folderBoard.png";
farray[4] = "images/ui/icons/tree/folderBoard.png";
farray[5] = "images/ui/icons/tree/home.png";
farray[6] = "images/ui/icons/tree/folderCommunity.png";
farray[7] = "images/ui/icons/tree/folderFilm.png";
farray[8] = "images/ui/icons/tree/folderCalendar.png";
farray[9] = "images/ui/icons/tree/folderGreen.png";
farray[10] = "images/ui/icons/tree/taxonomy.png";
farray[11] = "images/ui/icons/tree/collection.png";
farray[12] = "images/ui/icons/tree/menu.png";
TreeDisplayUtil.folders = farray;

var clickedElementPrevious = null;
var clickedIdPrevious = null;
var callback_function = "";
var g_timerForFolderTreeDisplay;
var treesLoaded = 0;

function onContextMenuHandler(id, clickedElement)
{
    return false;
}

function onFolderNodeClick(id, clickedElement, treeViewId, langId)
{
    onFolderNodeClickEx(id, clickedElement, true, treeViewId, langId);
}

function onFolderNodeClickEx(id, clickedElement, openMainPage, treeViewId, langId)
{
    if (clickedElementPrevious != null) // Only null if running it outside of the frameset while debugging
    {
        clickedElementPrevious.className = "";
    }

    clickedElement.className += "ektronTreeSelectedItem";
    clickedElementPrevious = clickedElement;
    clickedIdPrevious = id;

    if (id < 0)
    {
        switch (treeViewId)
        {
            case -3:
                top["ek_main"].location.href = "collections.aspx?action=ViewMenuReport";
                break;
            case -2:
                top["ek_main"].location.href = "collections.aspx?action=ViewCollectionReport";
                break;
            case -1:
                top["ek_main"].location.href = "taxonomy.aspx";
                break;
            default:
                alert("There is no data associated with this node. Please select another node.");
        }
        return;
    }

    var folderName = clickedElement.innerText;
    var folderId = id;

    document.getElementById("folderName").value = folderName;
    document.getElementById("selected_folder_id").value = id;

    loadRightFrame(id, treeViewId, openMainPage, langId);
}

function loadRightFrame(id, treeViewId, openMainPage, langId)
{
    if (openMainPage)
    {
        if (top["ek_main"] != null)
        {
            var url = (FrameName == "Library") ?
                "library.aspx?action=ViewLibraryByCategory&id=" + id :
                getContentPageActionUrl(treeViewId) + id + "&treeViewId=" + treeViewId;
            if (typeof langId != 'undefined') {
                url = url + "&LangType=" + langId;
            }
            top["ek_main"].location.href = url;
        }
    }
}

function getContentPageActionUrl(treeViewId)
{
    switch (treeViewId)
    {
        case -3:
            return "menu.aspx?Action=viewcontent&menuid=";
            //return "menutree.aspx?Action=viewcontent&nid=";
        case -2:
            return "collections.aspx?Action=View&nid=";
        case -1:
            return "taxonomy.aspx?action=view&view=item&taxonomyid=";
        case 0:
        default:
            return "content.aspx?action=ViewContentByCategory&id=";
    }
}

function onToggleClick(id, callback, args)
{
    toolkit.getChildFolders(id, -1, callback, args);
    if (callback_function == '')
    {
        callback_function = callback;
    }
}

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
    var treeid = 0;

    if (g_timerForFolderTreeDisplay)
    {
        window.clearTimeout(g_timerForFolderTreeDisplay);
    }

    if (g_selectedFolderList.length > 0)
    {
        var tree = TreeUtil.getTreeById(0, treeid);
        if (tree)
        {
            var lastId = 0;
            var folderList = g_selectedFolderList.split(",");
            bSuccessFlag = TreeDisplayUtil.expandTreeSet(folderList, 0);
        }

        if (!bSuccessFlag)
        {
            g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
        }
        else
        {
            var idValStr = folderList[folderList.length - 1];
            var idVal = parseInt(idValStr, 10);
            var idLeafElement = TreeUtil.getLinkId(idVal, treeid);
            var obj = document.getElementById(idLeafElement);
            if (obj)
            {
                onFolderNodeClickEx(idVal, obj, false, treeid, -99);
            }
            else
            {
                g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
            }
        }
    }
}

function displayAccordionElements()
{
    if (FrameName == "Content")
    {
        // these elements have to be reloaded when language changes
        //toolkit.getTaxonomies(-99, loadTaxonomies);
        //toolkit.getCollections(-99, loadCollections);
        //toolkit.getMenus(-99, loadMenus);
    }
}
function reloadAccordionElements(langCode)
{
    if (FrameName == "Content")
    {
        // these elements have to be reloaded when language changes
        treesLoaded = treesLoaded - 3;  // because the 3 calls below increment this
        toolkit.getTaxonomies(langCode, loadTaxonomies);
        toolkit.getCollections(langCode, loadCollections);
        toolkit.getMenus(langCode, loadMenus);
    }
}
function refreshTaxonomyAccordion(langCode)
{
    if (FrameName == "Content")
    {
        treesLoaded = treesLoaded - 1;  // because the next call increments this
        toolkit.getTaxonomies(langCode, loadTaxonomies);
    }
}
function refreshCollectionAccordion(langCode)
{
    if (FrameName == "Content")
    {
        treesLoaded = treesLoaded - 1;  // because the next call increments this
        toolkit.getCollections(langCode, loadCollections);
    }
}
function refreshMenuAccordion(langCode)
{
    if (FrameName == "Content")
    {
        treesLoaded = treesLoaded - 1;  // because the next call increments this
        toolkit.getMenus(langCode, loadMenus);
    }
}
function refreshFoldersAccordion(langCode)
{
    if (FrameName == "Content")
    {
        treesLoaded = treesLoaded - 1;  // because the next call increments this
        toolkit.getRootFolder(loadFolders);
    }
}
function displayTree()
{
    toolkit.getRootFolder(loadFolders);
    displayAccordionElements()
}

function loadFolders(folderRoot)
{
    var containerId = "TreeOutput";
    if (rootFolderName == null)
    {
        showLoadingError(containerId);
        return;
    }

    var treeRoot = new Tree(rootFolderName, 0, null, folderRoot, 0);
    var divContainer = document.getElementById(containerId);
    TreeDisplayUtil.showSelf(treeRoot, divContainer);
    TreeDisplayUtil.toggleTree(treeRoot.node.id, 0);
    treesLoaded++;
    checkLoadingMessage();
}

function showLoadingError(containerId)
{
    var element = document.getElementById(containerId);
    element.innerHTML = "Cannot connect to the CMS server";
}

function loadTaxonomies(items)
{
    var containerId = "TaxonomyTreeOutput";
    if (rootTaxonomyName == null)
    {
        showLoadingError(containerId);
        return;
    }

    loadItems(items, containerId, rootTaxonomyName, 10, -1);
}

function loadCollections(items)
{
    var containerId = "CollectionTreeOutput";
    if (rootCollectionName == null)
    {
        showLoadingError(containerId);
        return;
    }

    loadItems(items, containerId, rootCollectionName, 11, -2);
}

function loadMenus(items)
{
    var containerId = "MenuTreeOutput";
    if (rootMenuName == null)
    {
        showLoadingError(containerId);
        return;
    }

    loadItems(items, containerId, rootMenuName, 12, -3);
}

function loadItems(items, containerId, rootName, imageIndex, treeViewId)
{
    var root = getRootAsset(rootName, imageIndex);
    var treeRoot = new Tree(rootName, treeViewId, null, root, treeViewId);
    var divContainer = document.getElementById(containerId);
    
    if (divContainer == null) {
        // taxonomy treeview is not always there
        return;
    }
    
    TreeDisplayUtil.showSelf(treeRoot, divContainer);
    TreeUtil.addChildren(items, treeViewId, treeViewId);
    treesLoaded++;
    checkLoadingMessage();
}
function reloadItems(containerId, treeViewId)
{
    TreeDisplayUtil.reloadTree(treeRoot, treeViewId);
}

function getRootAsset(name, type)
{
    var rootAsset = new Asset();
    rootAsset.set("name", name);
    rootAsset.set("type", type);
    rootAsset.set("hasChildren", "true");
    return rootAsset;
}

function checkLoadingMessage()
{
    if (treesLoaded == getTreeCount())
    {
        document.body.style.cursor = "default";
        if (typeof Explorer !== "undefined") {
            Explorer.onLoadExplorePanel();
        }
    }
}

function getTreeCount()
{
    switch (FrameName)
    {
        case "Library":
            return 1;
        case "Content":
            //return 1;
            return 4;
        default:
            return 1;
    }
}

function isBrowserFireFox()
{
    return (top.IsBrowserFF && top.IsBrowserFF());
}
// ***note at the asp:literal litPerReadOnlyLib, litLanguageId1, litLanguageId2, and litMainPage needs to be loaded before including this file
// they're loaded into the variables PerReadOnlyLib, PerContentTreeLang, PerLibraryTreeLang, PerMainPage

var EditorHandle = new Object;
var m_MakeNavTreeVisibleTimerId=0;
var m_SelectMainWindowTimerId=0;

// Note: If "m_bSnapNavFrame" is true, will open/close
// the navigation in one step rather than incrementally:
var m_bSnapNavFrame = true;

function SnapNavFrame(){
    return(m_bSnapNavFrame);
}

var m_DebugWindow=null;
function DebugMsg(Msg) {
    Msg = Msg + "<br>";
    if ((m_DebugWindow == null) || (m_DebugWindow.closed)) {
        m_DebugWindow = window.open('', 'myWin',
	        'toolbar=no, directories=no, location=no, status=yes, menubar=no, resizable=yes, scrollbars=yes, width=300, height=300');
    }
    m_DebugWindow.document.writeln(Msg);
}

function IsBrowserSafari() {
    //var posn;
    //posn = parseInt(navigator.appVersion.indexOf('Safari'));
    //return (0 <= posn);
    var ua = window.navigator.userAgent.toLowerCase();
    return (ua.indexOf('safari') > -1);
}

function IsBrowserIE()
{
    // obsolete method: return (document.all ? true : false);
    var ua = window.navigator.userAgent.toLowerCase();
    return ((ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1)));
}

function IsBrowserFF()
{
    var ua = window.navigator.userAgent.toLowerCase();
    return (ua.indexOf('firefox') > -1);
}

function IsBrowserFFVersion1_0()
{
    var ua = window.navigator.userAgent.toLowerCase();
    var index = ua.indexOf('firefox');
    if (index > -1)
    {
        ua = ua.substring(index);
        index = ua.indexOf('1.0');
    }
    return (index > -1);
}

function CanNavigate() {
    var EkMainObject = GetEkMainObject();
    if (EkMainObject) {
        // prevent infinite recursion: return if the object is this
        if (this == EkMainObject) {
	        return true;
        }
        if (typeof(EkMainObject.CanNavigate) == "function") {
	        return (EkMainObject.CanNavigate());
        }
    }
    return (true);
}

function CanShowNavTree() {
    var EkMainObject = GetEkMainObject();
    if (EkMainObject && (this != EkMainObject)) {
        if (typeof(EkMainObject.CanShowNavTree) == "function") {
	        return (EkMainObject.CanShowNavTree());
        }
    }
    return (true);
}

function SetEditor(Handle) {
    EditorHandle = Handle;
}

function SetMetaComplete(Complete, ID) {
    if (typeof(EditorHandle) != 'undefined') {
        if (EditorHandle.closed) {
	        return true;
        }
        if (typeof(EditorHandle.main) != 'undefined') {
	        if (typeof(EditorHandle.main.SetMetaComplete) != 'undefined') {
		        EditorHandle.main.SetMetaComplete(Complete, ID);
	        }
        }
    }
    return true;
}
var reloaded = 0;
var reloadTimeout = null;
function HasReloaded() {
    return reloaded;
}
function ResetReload() {
    if (reloadTimeout != null) {
        clearTimeout(reloadTimeout);
        reloadTimeout = null;
    }
    reloaded = 0;
}
function ReloadMe() {
    reloaded = 1;
    reloadTimeout = setTimeout("ResetReload()", 10000);
    top.ek_main.location.reload();
}

function StartShrink(e) {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.StartShrink(e);
    }
    else {
        // object is not ready, reschedule action for later:
        //TODO: BCB: Fix this so object is passed or works without it: PostFunctionCallback("StartShrink(" + e + ");");
    }
}

function StartEnlarge(e) {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.StartEnlarge(e);
    }
    else {
        // object is not ready, reschedule action for later:
        //TODO: BCB: Fix this so object is passed or works without it: PostFunctionCallback("StartEnlarge(" + e + ");");
    }
}

function BeginEnlarge() {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.BeginEnlarge();
    }
    else {
        // object is not ready, reschedule action for later:
        PostFunctionCallback("BeginEnlarge();");
    }
}

function ShrinkFrame() {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.ShrinkFrame();
    }
    else {
        // object is not ready, reschedule action for later:
        PostFunctionCallback("ShrinkFrame();");
    }
}

function ResizeFrame(Direction) {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.ResizeFrame(Direction);
    }
    else {
        // object is not ready, reschedule action for later:
        PostFunctionCallback("ResizeFrame('" + Direction + "');");
    }
}

var m_isMac = false;
var m_isMacInit = false;
function IsPlatformMac() {
    if (m_isMacInit) {
        return (m_isMac);
    } else {
        var posn;
        var sUsrAgent = new String(navigator.userAgent);
        sUsrAgent = sUsrAgent.toLowerCase();
        posn = parseInt(sUsrAgent.indexOf('mac'));
        m_isMac = (0 <= posn);
        m_isMacInit = true;
        return (m_isMac);
    }
}

function TogglePin(PinObj) {
    var NavToolbarObject = GetNavToolbarObject();

    if (NavToolbarObject) {
        NavToolbarObject.TogglePin(PinObj);
    }
    else {
        // object is not ready, reschedule action for later:
        //TODO: BCB: Fix this so object is passed or works without it: PostFunctionCallback("TogglePin(" + PinObj + ");");
    }
}

function MakeNavTreeVisible(TreeName) {
    // cancel any pending callbacks to this function:
    if (m_MakeNavTreeVisibleTimerId) {
        CancelFunctionCallback(m_MakeNavTreeVisibleTimerId);
        m_MakeNavTreeVisibleTimerId = 0;
    }

    // ensure objects are loaded & ready, if not
    // then schedule a callback to retry later:
    var NavTreeObject = GetNavTreeObject();
    var NavFoldersObject = GetNavFoldersObject();
    if (NavTreeObject && NavFoldersObject) {
        // objects are ready; make desired nav-tree visible:
        NavTreeObject.MakeNavTreeVisible(TreeName, NavFoldersObject);
    }
    else {
        // objects are not ready; reschedule to try again later:
        PostFunctionCallback("MakeNavTreeVisible('" + TreeName + "');");
    }
}

function ValidateObjectsFrames(testObject) {
    return ((typeof(testObject.frames) == "object")
		        && (testObject.frames.length > 0));
}

function ValidateObject(testObject, tryLoadedFunction, validateDocument) {
    var retVal = (typeof(testObject) == "object");

    // default to not testing the objects "Loaded" function:
    if (typeof(tryLoadedFunction) == "undefined") {
        tryLoadedFunction = false;
    }
    if (retVal && tryLoadedFunction) {
        retVal = ((typeof(testObject.IsLoaded) == "function")
	        && testObject.IsLoaded());
    }

    // default to not testing the objects "Document":
    if (typeof(validateDocument) == "undefined") {
        validateDocument = false;
    }

    if (retVal && validateDocument) {
        retval = (((typeof(testObject.document)) != "undefined")
	        && (testObject.document != null) );
    }
    return (retVal);
}

function EnableIcon(iconNumber, enableFlag) {
    var NavIconbarObject = GetNavIconbarObject();

    if (NavIconbarObject) {
        NavIconbarObject.EnableIcon(iconNumber, enableFlag);
        return true;
    }
    return false;
}

function SelectIcon(iconNumber, selectFlag) {
    var NavIconbarObject = GetNavIconbarObject();

    if (NavIconbarObject) {
        NavIconbarObject.SelectIcon(iconNumber, selectFlag);
        return true;
    }
    return false;
}

function switchMainPage() {
    var EkMainObject = GetEkMainObject();
    if (EkMainObject && (EkMainObject.location.href != '')) {
        var newmainpage = top.PerWorkareaPrefix + top.PerMainPage;
        if ((top.PerMainPage != '') && (EkMainObject.location.href != newmainpage)) {
            EkMainObject.location.href = newmainpage;
            top.PerMainPage = '';
        }
    } else {
        // not ready yet, so try again in a bit
        setTimeout(switchMainPage, 100);    // wait a bit to switch to avoid race condition w/ tab switching
    }
}

function SelectMainWindow(treeName) {
    var EkMainObject = GetEkMainObject();
    var EkDragDropObject = GetEkDragDropObject();

    // cancel any pending action (last takes precedence):
    if (m_SelectMainWindowTimerId) {
        CancelFunctionCallback(m_SelectMainWindowTimerId);
        m_SelectMainWindowTimerId = 0;
    }

    var PerReadOnly = true;

    var bottomFrameSet = document.getElementById("BottomFrameSet");
    bottomFrameSet.cols = "300, *";

    if (EkMainObject) {
        switch (treeName) {
            case ("UserMessages"): // User Messages Inbox page
	        {
		        EkMainObject.location.href = top.PerWorkareaPrefix + "CommunityMessaging.aspx?action=viewall";
		        HideDragDropWindow();
	            bottomFrameSet.cols = "0, *";
                // TODO: Ross - Figure out how to set the border to 0
		        break;
	        }
	        case ("SmartDesktopTree"): // Desktop page
	        {
		        EkMainObject.location.href = top.PerWorkareaPrefix + "dashboard.aspx";
		        HideDragDropWindow();
	            bottomFrameSet.cols = "0, *";
                // TODO: Ross - Figure out how to set the border to 0
		        break;
	        }
	        case ("ContentTree"): // Content page
	        {
		        if (CheckUserPerm()) {
		            // user permissions validated
		            // if jumping to a specific page, kick it off
		            if (top.PerMainPage != '') {
		                setTimeout(switchMainPage, 100);    // wait a bit to switch to avoid race condition w/ tab switching
		            } else {
			            EkMainObject.location.href = top.PerWorkareaPrefix + "content.aspx?action=ViewContentByCategory&id=0"; //&LangType=" + PerContentTreeLang;
			        }
			        // change the fram size EkDragDropObject
			        ShowDragDropWindow();
		        } else {
			        EkMainObject.location.href = top.PerWorkareaPrefix + "notify_user.aspx"; // this displays a simple message to the user.
			        HideDragDropWindow();
		        }
		        break;
	        }
	        case ("LibraryTree"): // Library page
	        {
		        if (PerReadOnlyLib) {
			        EkMainObject.location.href = top.PerWorkareaPrefix + "library.aspx?action=ViewLibraryByCategory&id=0";  //&LangType=" + PerLibraryTreeLang;
		        } else {
			        EkMainObject.location.href = top.PerWorkareaPrefix + "notify_user.aspx"; // this displays a simple message to the user.
		        }
		        HideDragDropWindow();
		        break;
	        }
	        case ("AdminTree"): // Settings page
	        {
	            // if jumping to a specific page, kick it off
	            if (top.PerMainPage != '') {
	                setTimeout(switchMainPage, 100);    // wait a bit to switch to avoid race condition w/ tab switching
	            } else {
		            EkMainObject.location.href = top.PerWorkareaPrefix + "notify_user.aspx?splash=admintree"; // this displays a simple message to the user.
		        }
		        HideDragDropWindow();
		        break;
	        }
	        case ("Help"): // Help page
	        {
	            EkMainObject.location.href = top.PerWorkareaPrefix + "help.aspx?action=helpdocuments"; //"help.aspx?action=ViewManuals";
	            HideDragDropWindow();
	            bottomFrameSet.cols = "0, *";
	            // TODO: Ross - Figure out how to set the border to 0
	            break;
	        }
	        case ("ReportTree"): // Report page
	        {
	            // if jumping to a specific page, kick it off
	            if (top.PerMainPage != '') {
	                setTimeout(switchMainPage, 100);    // wait a bit to switch to avoid race condition w/ tab switching
	            } else {
		            EkMainObject.location.href = top.PerWorkareaPrefix + "notify_user.aspx?splash=report"; // this displays a simple message to the user.
		        }
		        HideDragDropWindow();
		        break;
	        }
	        default:
	        {
		        EkMainObject.location.href = top.PerWorkareaPrefix + "notify_user.aspx"; // this displays a simple message to the user.
		        HideDragDropWindow();
            }
        }
    }
    else {
        m_SelectMainWindowTimerId = PostFunctionCallback("SelectMainWindow('" + treeName + "');");
    }
}

// Verify user has adequate permissions to view root folder:
function CheckUserPerm() {
    /*< %
    Dim ErrorStr
    Dim cTmpObj
    Dim ContObj = AppUI.EkContentRef
    cTmpObj = ContObj.GetFolderInfov2_0(0, ErrorStr)
    cTmpObj = nothing
    ContObj = nothing
    if (ErrorStr = "") then
    % >*/
        return (true);
    /*< % Else % >
        return (false);
    < %End If % >*/
}

function PostFunctionCallback(funcString, delayMs) {
    var timerId;
    // default to 100 millisecond delay:
    if (typeof(delayMs) == "undefined") {
        delayMs = 100;
    }
    timerId = setTimeout(funcString, delayMs);
    return (timerId);
}

function CancelFunctionCallback(timerId) {
    clearTimeout(timerId);
}

function ReloadTrees(TreeNames) {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.ReloadTrees(TreeNames);
    }
    else {
        PostFunctionCallback("ReloadTrees('" + TreeNames + "');");
    }
}

function TreeNavigation(treeName, path) {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.TreeNavigation(treeName, path);
    }
    else {
        PostFunctionCallback("TreeNavigation('" + treeName + "','" + path + "');");
    }
}

function SelectButton(ButtonNumber, TreeName) {
    var NavButtonObject = GetNavButtonObject();

    if (NavButtonObject) {
        NavButtonObject.SelectButton(ButtonNumber, TreeName);
        return true;
    }
    return false;
}

function NavTreeResizing() {
    var NavTreeObject = GetNavTreeObject();

    if (NavTreeObject) {
        NavTreeObject.NavTreeResizing();
    }
}

function GetNavTreeObject() {
    if (ValidateObjectsFrames(this)
        && ValidateObject(frames["ek_nav_bottom"], true)) {
        return (frames["ek_nav_bottom"]);
    }
    return (null)
}

function GetNavToolbarObject() {
    if (ValidateObjectsFrames(this)
        && ValidateObject(frames["nav_toolbar"], true)) {
        return (frames["nav_toolbar"]);
    }
    return (null)
}

function GetNavFoldersObject() {
    if (ValidateObjectsFrames(this)
        && ValidateObject(frames["ek_nav_bottom"], true)
        && ValidateObject(frames["ek_nav_bottom"]["NavIframeContainer"], false)
        && ValidateObject(frames["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"], true)) {
        return (frames["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]);
    }
    return (null)
}

function GetNavIconbarObject() {
    if ((ValidateObjectsFrames(this))
        && (ValidateObject(frames["ek_nav_bottom"], true))
        && (ValidateObject(frames["ek_nav_bottom"]["NavIframeContainer"], false))
        && (ValidateObject(frames["ek_nav_bottom"]["NavIframeContainer"]["nav_icon_area"], true, true))) {
        return (frames["ek_nav_bottom"]["NavIframeContainer"]["nav_icon_area"]);
    }
    return (null)
}

function GetNavButtonObject() {
    if ((ValidateObjectsFrames(this))
        && (ValidateObject(frames["ek_nav_bottom"], true))
        && (ValidateObject(frames["ek_nav_bottom"]["NavIframeContainer"], false))
        && (ValidateObject(frames["ek_nav_bottom"]["NavIframeContainer"]["nav_button_area"], true, true))) {
        return (frames["ek_nav_bottom"]["NavIframeContainer"]["nav_button_area"]);
    }
    return (null)
}

function GetEkMainObject() {
    if (ValidateObjectsFrames(this)
        && ValidateObject(frames["ek_main"], false)) {
        return (frames["ek_main"]);
    }
    return (null)
}
function GetEkDragDropObject() {
    if (ValidateObjectsFrames(this)
        && ValidateObject(frames["ek_dragdrop"], false)) {
        return (frames["ek_dragdrop"]);
    }
    return (null)
}
function HideDragDropWindow(){
    if (!ChangeDragDropWindow(true))
    {
        setTimeout("HideDragDropWindow()", 10);
    }
}
function ShowDragDropWindow(){
    return false;

    if (!ChangeDragDropWindow(false))
    {
        setTimeout("ShowDragDropWindow()", 10);
    }
}
function ChangeDragDropWindow(bHide)
{
    NavFrame = document.getElementById('BottomRightFrame');
    if (NavFrame != null)
    {
        if (bHide)
        {
	        NavFrame.rows = "*,1";
        }
        else
        {
	        if (CanShowNavTree)
	        {
		        NavFrame.rows = "*,87";
	        }
        }
        return true;
    }
    return false;
}

function DisplayUploadingBox(bShow) {
    try {
        var rightTop = window.frames["ek_main"];
        if(rightTop){
            if(typeof rightTop.showUploadingBox == 'function'){
                rightTop.showUploadingBox(bShow);
                var leftTree = window.frames["ek_nav_bottom"];
                if(leftTree){
                    if(typeof leftTree.blockWindowClick == 'function') {
                        leftTree.blockWindowClick(bShow);
	                    var leftDragDrop = window.frames["ek_dragdrop"];
                        if(leftDragDrop){
                            if(typeof leftDragDrop.blockWindowClick == 'function')
                                leftDragDrop.blockWindowClick(bShow);
                        }
                    }
                }
            }
        }
    }
    catch(e){
        alert(e.message);
    }
}

function notifyLanguageSwitch(langCode, folderId)
{
    // yes this is how deep the accordion is :-P
    if (typeof top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.reloadAccordionElements != 'undefined') {
        // if you're on the settings page, the folder tree isn't loaded, so we have to check if it is before calling it
        top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.reloadAccordionElements(langCode);
    }
}
function refreshMenuAccordion(langCode)
{
    top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.refreshMenuAccordion(langCode);
}
function refreshCollectionAccordion(langCode)
{
    top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.refreshCollectionAccordion(langCode);
}
function refreshTaxonomyAccordion(langCode) 
{
    if (typeof top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.refreshTaxonomyAccordion != 'undefined') 
    {
        top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.refreshTaxonomyAccordion(langCode);
    }
}
function refreshFoldersAccordion(langCode)
{
    top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.refreshFoldersAccordion(langCode);
}


var g_selectedWorkareaFolderList = '';
function openContentTreePath()
{
    if ("undefined" == typeof(top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.g_selectedFolderList)) {
        setTimeout(openContentTreePath, 100);    // wait a bit to try again later
    } else {
        top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.g_selectedFolderList = g_selectedWorkareaFolderList;
        top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.showSelectedFolderTree();
    }
}
function openSettingsTreePath()
{
    if ("undefined" == typeof(top.ek_nav_bottom.NavIframeContainer.nav_folder_area.AdminTree.OpenFolder)) {
        setTimeout(openSettingsTreePath, 100);    // wait a bit to try again later
    } else {
        top.ek_nav_bottom.NavIframeContainer.nav_folder_area.AdminTree.OpenFolder(g_selectedWorkareaFolderList, false);
    }
}

function showContentInWorkarea(contenturl, treevisible, treepath)
{
    var isSettingsTab = (("undefined" != typeof(treevisible)) && (treevisible == "Settings"));
    var isReportsTab = (("undefined" != typeof(treevisible)) && (treevisible == "Reports"));
    // switch main right pane to URL specified
    top.PerMainPage = unescape(contenturl);
    if (("undefined" != typeof(treevisible)) && !isSettingsTab && !isReportsTab) {
        //TODO: switch to "Content" or "Taxonomy" or "Menu" or "Collection" in accordion
    }
    // switch tree to appropriate path
    if ("undefined" != typeof(treepath)) {
        g_selectedWorkareaFolderList = treepath;
        if (isSettingsTab) {
            setTimeout(openSettingsTreePath, 100);    // jump tree if possible
        } else {
            setTimeout(openContentTreePath, 100);    // jump tree if possible
        }
    }
    if (isSettingsTab) {
        switchSettingsTab();
    } else if (isReportsTab) {
        switchReportsTab();
    } else {
        // default is switch to content tab in workarea
        switchContentTab();
    }
}


function switchContentTab() {
    if (typeof top.workareatop.ChangePage != 'undefined') {
        top.workareatop.ChangePage(top.workareatop.document.getElementById('ContentLink'), "ContentTree");
    } else {
        setTimeout(switchContentTab, 250);    // wait a bit to try again later
    }
}
function switchDesktopTab() {
    if (typeof top.workareatop.ChangePage != 'undefined') {
        top.workareatop.ChangePage(top.workareatop.document.getElementById('DesktopLink'), "SmartDesktopTree");
    } else {
        setTimeout(switchContentTab, 250);    // wait a bit to try again later
    }
}
function switchSettingsTab() {
    if (typeof top.workareatop.ChangePage != 'undefined') {
        top.workareatop.ChangePage(top.workareatop.document.getElementById('SettingsLink'), "AdminTree");
    } else {
        setTimeout(switchSettingsTab, 250);    // wait a bit to try again later
    }
}
function switchReportsTab() {
    if (typeof top.workareatop.ChangePage != 'undefined') {
        top.workareatop.ChangePage(top.workareatop.document.getElementById('ReportsLink'), "ReportTree");
    } else {
        setTimeout(switchSettingsTab, 250);    // wait a bit to try again later
    }
}

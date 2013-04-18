///////////////////////////////////////////////////////////
// ekFlexMenu Javascript file (ek_flexmenu.js):
// Client side support for the Ektron SmartMenu server control.


///////////////////////////////////////////////////////////
// CSS Style Class Name Enumertion:
function ekFlexMenu_classNames() { }
ekFlexMenu_classNames.button = "ekflexmenu_button";
ekFlexMenu_classNames.buttonHover = "ekflexmenu_button_hover";
ekFlexMenu_classNames.buttonSelected = "ekflexmenu_button_selected";
ekFlexMenu_classNames.buttonSelectedHover = "ekflexmenu_button_selected_hover";
ekFlexMenu_classNames.submenuItems = "ekflexmenu_submenu_items";
ekFlexMenu_classNames.submenuItemsHidden = "ekflexmenu_submenu_items_hidden";
ekFlexMenu_classNames.submenu = "ekflexmenu_submenu";
ekFlexMenu_classNames.submenuHover = "ekflexmenu_submenu_hover";
ekFlexMenu_classNames.submenuParent = "ekflexmenu_submenu_parent";
ekFlexMenu_classNames.submenuParentHover = "ekflexmenu_submenu_parent_hover";
ekFlexMenu_classNames.btnLink = "ekflexmenu_accessible_submenu_btnlink";
ekFlexMenu_classNames.link = "ekflexmenu_link";
ekFlexMenu_classNames.linkSelected = "ekflexmenu_link_selected";
ekFlexMenu_classNames.slaveBranchSelected = "ekflexmenu_slave_branch_sel";
ekFlexMenu_classNames.slaveContainer = "ekflexmenu_slavecontainer";
ekFlexMenu_classNames.activeMenu = "ActiveFlexMenu";

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
// Class ekFlexMenu:
var ekFlexMenu = function(menuObjectIdString) {

    /////////////////////////
    // public members:

    this.menuId = __ekFlexMenu_returnMenuId;
    // Returns the root menu id for this object.
    // Parameters: 
    //	None.

    this.hashCode = __ekFlexMenu_returnHashCode;
    // Returns the hash-code of the server control.
    // Parameters: 
    //	None.

    this.isSubmenuSelected = __ekFlexMenu_isSubmenuSelected;
    // Returns selected-status (and thereby the visibility of the 
    // associated submenu contents) of the identified submenu.
    // Parameters: 
    //	1 - The standard menu-submenu-id string (extra characters discarded).

    this.selectSubmenu = __ekFlexMenu_selectSubmenu;
    // Selects the designated submenu, setting the menu-button to a 'selected' 
    // state, and making any associated submenu content items visible.
    // Parameters: 
    //	1 - The standard menu-submenu-id string (extra characters discarded).

    this.unSelectSubmenu = __ekFlexMenu_unSelectSubmenu;
    // De-Selects the designated submenu, setting the menu-button to a non-selected
    // state, and making any associated submenu content items invisible.
    // Parameters: 
    //	1 - The standard menu-submenu-id string (extra characters discarded).

    this.hoverButton = __ekFlexMenu_hoverButton;
    // Sets the designated submenu-button to a hovered or non-hovered state.
    // Parameters: 
    //	1 - The standard menu-submenu-id string (extra characters discarded).
    //	2 - Hover flag (boolean; true to set state to hovered).

    this.selectMenuItem = __ekFlexMenu_ekFlexMenu_selectMenuItem;
    // Called when a menu-item (such as a link) is clicked, before
    // the page is submitted to the server.
    // Parameters: 
    //	1 - The element-object that is being selected.

    this.initializeWithServerVariables = __ekFlexMenu_initializeWithServerVariables;
    // Called by page-load initialization code, to initialize this object
    // with values passed from the server.
    // Parameters: 
    //	None.

    this.showRootMenu = __ekFlexMenu_showRootMenu;
    // Makes the contents of the root-menu visible, selects it's button if it exists.
    // Parameters: 
    //	None.


    /////////////////////////
    // private member functions:

    this.buildMenuSubmenuId = __ekFlexMenu_buildMenuSubmenuId;
    // Returns the standard menu-submenu-id string.
    // Parameters: 
    //	1 - The targetted submenu-id number (or string containing only numbers).

    this.getFolderButtonObject = __ekFlexMenu_getFolderButtonObject;
    // Returns the folder-button-object for the specified submenu.
    // Parameters: 
    //	1 - The targetted submenu-id (standard menu-submenu-id string).

    this.getSubmenuItemsObject = __ekFlexMenu_getSubmenuItemsObject;
    // Returns the folder-item-object for the specified submenu.
    // This may contain menu items such as links and nested submenus.
    // Parameters: 
    //	1 - The targetted submenu-id (standard menu-submenu-id string).

    this.getSubmenuObject = __ekFlexMenu_getSubmenuObject;
    // Returns the corresponding submenu object, 
    // for a given Submenu-Id (or Menu-Submenu-Id):
    // Parameters: 
    //	1 - The targetted submenu-id (standard menu-submenu-id string).

    this.getParentLevelSubmenuId = __ekFlexMenu_getParentLevelSubmenuId;
    // Returns the parent-levels menu-submenu-id for the given Submenu,
    // returns zero if the parent (or thismenu) is the root menu.
    // Parameters: 
    //	1 - The targetted submenu-id (standard menu-submenu-id string).

    this.getMenuLevel = __ekFlexMenu_getMenuLevel;
    // Returns the integer value of the menu level for
    // the menu identified by the given submenu-id string.
    // Parameters: 
    //	1 - The submenu-id (standard menu-submenu-id string).

    this.getEkFlexMenuContainerElement = __ekFlexMenu_getEkFlexMenuContainerElement;
    // Returns the outermost container element (DIV) that 
    // holds this entire ekFlexMenu object.
    // Parameters: 
    //	None.

    this.selectSubmenuHelper = __ekFlexMenu_selectSubmenuHelper;
    // Helper funtion for __ekFlexMenu_selectSubmenu, uses 
    // recursionSelects to ensure selected submenus are visible
    // even if they are buried with muliple nesting levels.
    // Parameters: 
    //	1 - The targetted submenu-id (standard menu-submenu-id string).

    this.collapseSiblingSubmenus = __ekFlexMenu_collapseSiblingSubmenus;
    // Hide sibling submenus of the designated submenu.
    // Parameters: 
    //	1 - The targetted submenu-id (standard menu-submenu-id string).

    this.collapseAllOpenSubmenus = __ekFlexMenu_collapseAllOpenSubmenus;
    // Closes all currently open submenus, to prevent overlap & visual clutter.
    // Parameters: 
    //	1 - Show root flag (boolean; true to make the root menu contents visible).

    this.collapseUnselectedStartLevelSubmenus = __ekFlexMenu_collapseUnselectedStartLevelSubmenus;
    // Used for slave menu; Closes all unselected submenus:
    // Parameters: 
    //	None.

    this.markParentSubmenu = __ekFlexMenu_markParentSubmenu;
    // Sets the parent folders' style to be a parent (optionally 
    // used in CSS to style parents & children differently):
    // Parameters: 
    //	1 - The submenu-id whose parent to mark (standard menu-submenu-id string).

    this.unMarkParentSubmenu = __ekFlexMenu_unMarkParentSubmenu;
    // Sets the parent folders' style to be a normal non-parent (optionally 
    // used in CSS to style parents & children differently):
    // Parameters: 
    //	1 - The submenu-id whose parent to unmark (standard menu-submenu-id string).

    this.hoverSubmenu = __ekFlexMenu_hoverSubmenu;
    // Sets the menus' container style to be hovered,
    // (optionally used in CSS to style contents & children differently):
    // Parameters: 
    //	1 - The submenu-id whose parent to hover (standard menu-submenu-id string).

    this.unHoverSubmenu = __ekFlexMenu_unHoverSubmenu;
    // Sets the menus' container style to be unhovered,
    // (optionally used in CSS to style contents & children differently):
    // Parameters: 
    //	1 - The submenu-id whose parent to unhover (standard menu-submenu-id string).


    this.getEkFlexMenuElementsByTagName = __ekFlexMenu_getEkFlexMenuElementsByTagName;
    // Returns an array of the ekflexmenu-elements with the specified tag-name.
    // Parameters: 
    //	1 - the element tag-name to search for.

    this.getElementsByClassName = __ekFlexMenu_getElementsByClassName;
    // Returns an array of the menu-elements, whose className 
    // attributes match the supplied name.
    // Parameters: 
    //	1 - the className to search for.

    this.getElementsByClassNameAndTagName = __ekFlexMenu_getElementsByClassNameAndTagName;
    // Returns an array of the menu-elements, whose className 
    // attributes match the supplied name.
    // Parameters: 
    //	1 - the className to search for.
    //  2 - the tag-name of the elements to include in the search.

    this.getEkFlexMenuElementsByName = __ekFlexMenu_getEkFlexMenuElementsByName;
    // Returns an array of the menu-elements, whose name attribute
    // match the supplied name.
    // Parameters: 
    //	1 - the name to search for.

    this.getDirectChildIds = __ekFlexMenu_getDirectChildIds;
    // Returns an array of all direct child-submenu-ids (length = 0 if none).
    // Parameters: 
    //	1 - The targetted submenu-id (standard menu-submenu-id string).

    this.mouseIn = __ekFlexMenu_mouseIn;
    // Called by external (non-object-instance) code, to prepare for 
    // delayed opening of identified submenu.
    // Parameters: 
    //	1 - the event object.
    //	2 - the element-object that triggered the event.

    this.mouseInHelper = __ekFlexMenu_mouseInHelper;
    // Shows/selects the appropriate submenu.
    // Parameters: 
    //	None.

    this.mouseOut = __ekFlexMenu_mouseOut;
    // Called by external (non-object-instance) code, to prepare for 
    // delayed opening of identified submenu.
    // Parameters: 
    //	1 - the event object.
    //	2 - the element-object that triggered the event.

    this.mouseOutHelper = __ekFlexMenu_mouseOutHelper;
    // Hides/unselects the appropriate submenu (possibly all but root).
    // Parameters: 
    //	None.

    this.disableAllEventHandlers = __ekFlexMenu_disableAllEventHandlers;
    // Disables all event handlers for elements of this menu object:
    // Parameters: 
    //	None.

    this.disableElementEventHandlers = __ekFlexMenu_disableElementEventHandlers;
    // Disables all event handlers for the given element:
    // Parameters: 
    //	1 - the element to disable events on.

    this.updateNoScriptLinks = __ekFlexMenu_updateNoScriptLinks;
    // When server (XSLT) builds HTML to create the menu,
    // it sets the hrefs of the menu-buttons to signal
    // the server that Javascript is disabled, and the page
    // needs to display a submenu. This way the menu will 
    // still funtion if Javascript is disabled/not supported
    // (required for 508 compliance), this also simplifies Ajax
    // related issues. This function modifies all these to eliminate 
    // page refreshes (not needed if Javascript is enabled).
    // Parameters: 
    //	None.

    this.initializeMouseOverPopUp = __ekFlexMenu_initializeMouseOverPopUp;
    // Determine if MouseOverPopUp is enabled, and 
    // perform related initialization if it is.
    // Parameters: 
    //	None.

    this.initializeMouseOverPopUpForAnchorsWithImageOnly = __ekFlexMenu_initializeMouseOverPopUpForAnchorsWithImageOnly;
    // corrects problem where MouseOverPopUp is enabled, and the submenu 
    // anchor tags do not contain any text - using an image instead. 
    // This can result in submenus that do not open consistently.
    // Parameters: 
    //	None.


    ///////////////////////////////////////////////////////
    // Master/Slave related functions:
    this.getSlaveControlObject = __ekFlexMenu_getSlaveControlObject;
    this.convertIdToSlaveControlId = __ekFlexMenu_convertIdToSlaveControlId;
    this.callSlave__showSubmenuBranch = __ekFlexMenu_callSlave__showSubmenuBranch;
    this.showSubmenuBranch = __ekFlexMenu_showSubmenuBranch;
    this.unSelectSubmenuList = __ekFlexMenu_unSelectSubmenuList;
    this.initializeSlaveMenu = __ekFlexMenu_initializeSlaveMenu;
    this.initializeMasterMenu = __ekFlexMenu_initializeMasterMenu;
    this.isTopLevelUI = __ekFlexMenu_isTopLevelUI;
    this.recordLastSlaveStartLevelMenu = __ekFlexMenu_recordLastSlaveStartLevelMenu;
    this.getLastSlaveStartLevelMenu = __ekFlexMenu_getLastSlaveStartLevelMenu;
    this.unHideSlaveMenu = __ekFlexMenu_unHideSlaveMenu;
    this.isSlaveMenuHiding = __ekFlexMenu_isSlaveMenuHiding;
    // Master/Slave related variables:
    this.topLevelUI = null;
    this.lastSelectedStartLevelSlaveMenuId = null;
    this.ekFlexMenu_defaultMenuIdString = false; // Slave menu default submenu-id.
    this.ekFlexMenu_slaveStartLevelIds = null; // slave menu start level ids.
    this.ekFlexMenu_slaveStartLevel = null; // slave menu start level.


    ///////////////////////////////////////////////////////
    // Member Ajax releated members:
    this.loadXMLDoc = __ekFlexMenu_ajax_loadXMLDoc;
    this.DecodeHTML = __ekFlexMenu_ajax_DecodeHTML;
    this.getPayload = __ekFlexMenu_ajax_getPayload;
    this.appendText = __ekFlexMenu_ajax_appendText;
    this.appendXml = __ekFlexMenu_ajax_appendXml;
    this.removeMenuFragmentContainer = __ekFlexMenu_ajax_removeMenuFragmentContainer;
    this.callService = __ekFlexMenu_ajax_callService;
    this.callAjaxForUserClick = __ekFlexMenu_ajax_callAjaxForUserClick;
    // Ajax related variables:
    //this.userAjaxXmlHttp1 = null;
    this.userAjaxParentId1 = ""


    /////////////////////////
    // private variables:

    this.private_menuIdString = __ekFlexMenu_parseMenuId(menuObjectIdString);
    // holds the root menu id.

    this.private_serverControlHash = __ekFlexMenu_static_parseServerControlHash(menuObjectIdString);
    // holds the server controls' hash-code.

    this.private_autoCollapseSubmenus = true;
    // Controls action on select-submenu; will 
    // collapse all other submenus - if this is true.

    this.private_startWithRootFolderCollapsed = false;
    // If set, will hide the root menu contents when all submenus 
    // are collapsed, otherwise will always leave root contents visible.

    this.private_startCollapsed = true;
    // If set, menu is initially rendered with all submenus closed.

    this.private_ajaxEnabled = true;
    // If set, will attempt to make Ajax calls to load submenus.

    this.ekFlexMenu_ajaxWSPath = "";
    // The Ajax-WebService base path.

    this.ekFlexMenu_displayXslt = "";
    // value to call web-service with for Ajax, to select server-side XSLT to process XML.

    this.ekFlexMenu_cacheInterval = "";
    // value to call web-service with for Ajax, to select period to hold server-side to cache data.

    this.private_masterControlIdHash = "";
    // If this is a slave control, then this variable holds the 
    // hash-code of the master source controls id.

    this.private_subscriberList = "";
    // If this a master control, then this comma delited list (string)
    // holds the hash-codes of each subscribing control.

    this.private_slaveControl = "";
    // If this a master control, then this variable
    // holds the hash-code of the slave control.

    this.private_isMasterControl = false;
    // True if this control is synchronized to another (slave) control.

    this.private_isSlaveControl = false;
    // True if this control is synchronized to another (master) control.

    this.private_preventHidingSlaveMenu = false;
    // If true, wiil show previously selected (or default) slave menu when 
    // conditions would normally produce an empty slave menu; e.g. selecting
    // a menu-item (link) from the master menu.

    this.private_lastSelectedMenuItemObj = null;
    // Holds previously selected menu-item-link, used to set old 
    // selection to a non-selected state when a new one is selected.

    this.private_selectionChanged = false;
    // Flag to indicate that user activity has changed state from
    // that which was rendered from the server.

    this.private_selectedMenuList = "";
    // Holds previously selected menu, used to set the old button
    // selection to a non-selected state when a new one is selected.

    this.private_swRevision = "0";
    // The software revision of the server control.

    this.enableMouseOverPopUp = false;
    // True to indicate that a submenu should open when moused over,
    // False inidcates that a click is required to oen submenus.

    // Mouse related variables; only used for pop-up menus (via mouse over):
    this.private_enableMouseOverSubmenuActivation = false;
    // If true, then mouseIn and mouseOut events will be used to 
    // open and close submenus (must be wired by server code).

    this.private_mouseEventTimer = null;
    // Used to hold the count-down timer object, to delay show/hide action.

    this.private_mouseEventEnteringElementId = null;
    // Holds the ID of the element-id that triggered the mouseIn event.

    this.private_mouseEventExitingElementId = null;
    // Holds the ID of the element-id that triggered the mouseOut event.

    this.private_enableRestoreMenuState = true;
    // If set to true, menu can restore original selections on mouse out (used with EnableMouseOverPopUp).
}
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Initialize Public Static Members:

ekFlexMenu.getMenuObj = __ekFlexMenu_static_getMenuObj;
// Returns the Menu-Object for a given ekflexmenu element ID,
// creates a new ekFlexMenu object if needed (stores these in
// an array as a property to the indow object - making it 
// available/shared with all SmartMenus (this was multple menus 
// can exist on a page, and have different objects that 
// are avaiable anywhere - given given an ekflexmenu element ID).
// Parameters: 
//	1 - The standard menu-submenu-id string (extra characters discarded).


ekFlexMenu.parseMenuSubmenuIdString = __ekFlexMenu_static_parseMenuSubmenuIdString;
// Returns the MenuSubmenuID string, for a given ekflexmenu element-ID
// (ex. given "e1234567_1_submenu_2_button" returns
// "e1234567_1_submenu_2" for root-menu 1, submenu 2):
// Parameters: 
//	1 - The standard menu-submenu-id string (extra characters discarded).

ekFlexMenu.parseServerControlHash = __ekFlexMenu_static_parseServerControlHash;
// Returns the server controls' hash-code of the supplied string (or
// whatever was supplied if not a valid menu-submenu id string):
// Parameters: 
//	1 - The standard menu-submenu-id string (extra characters discarded).

// Menu Folder-Button event handlers:
ekFlexMenu.menuBtnClickHdlr = __ekFlexMenu_static_menuButtonClickEventHandler;
// Handler for Menu-Button-Click events
// Parameters: 
//	1 - the event-object.

ekFlexMenu.menuBtnKeyHdlr = __ekFlexMenu_static_menuButtonKeyDownEventHandler;
// Handler for Menu-Button-Keydown events.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.menuBtnMouseOverHdlr = __ekFlexMenu_static_menuButtonMouseOverEventHandler;
// Handler for Menu-Button-MouseOver events.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.menuBtnMouseOutHdlr = __ekFlexMenu_static_menuButtonMouseOutEventHandler;
// Handler for Menu-Button-MouseOut events.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.menuBtnFocusHdlr = __ekFlexMenu_static_menuButtonFocusEventHandler;
// Handler for Menu-Button-Focus events.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.menuBtnBlurHdlr = __ekFlexMenu_static_menuButtonBlurEventHandler;
// Handler for Menu-Button-Blur events.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.menuBtnLinkFocusHdlr = __ekFlexMenu_static_menuButtonLinkFocusEventHandler;
// Handler for Menu-Button-Link-onFocus events.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.menuBtnLinkBlurHdlr = __ekFlexMenu_static_menuButtonLinkBlurEventHandler;
// Handler for Menu-Button-Link-onBlur events.
// Parameters: 
//	1 - the event-object.


// Menu Item-Link event handlers:
ekFlexMenu.itemLinkClickHdlr = __ekFlexMenu_static_menuItemLinkClickEventHandler;
// Handler for Menu-Item-Link-Click events.
// Parameters: 
//	1 - the event-object.

// Not Needed (key translated by browser, others handled by link pseudo classes):
//ekFlexMenu.itemLinkKeyHdlr = __ekFlexMenu_static_menuItemLinkKeyDownEventHandler;
//ekFlexMenu.itemLinkMouseOverHdlr = __ekFlexMenu_static_menuItemLinkMouseOverEventHandler;
//ekFlexMenu.itemLinkMouseOutHdlr = __ekFlexMenu_static_menuItemLinkMouseOutEventHandler;
//ekFlexMenu.itemLinkFocusHdlr = __ekFlexMenu_static_menuItemLinkFocusEventHandler;
//ekFlexMenu.itemLinkBlurHdlr = __ekFlexMenu_static_menuItemLinkBlurEventHandler;


// mouseIn and mouseOut event handlers:
ekFlexMenu.mouseIn = __ekFlexMenu_static_mouseIn;
// Prepare for delayed opening of the submenu related to the event-triggering element.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.mouseOut = __ekFlexMenu_static_mouseOut;
// Prepare for delayed closing of the submenu related to the event-triggering element.
// Parameters: 
//	1 - the event-object.

ekFlexMenu.mouseIn_empty = function(event) { return (true); } // bubble event...

///////////////////////////////////////////////////////////////////////////////
// Initialize Private Static Members:
ekFlexMenu.private_isValidMenuSubmenuIdString = __ekFlexMenu_static_isValidMenuSubmenuIdString
// Verifies that the supplied element-ID string is a valid 
// MenuSubmenuID string (ex. "e1234567_1_submenu_2_button...")
// Note: it may be more than this, but as long as the submitted string
// begins with a valid and usable standard menu-submenu-id string, then
// the results are positive (any extra appended characters are ignored).
// Parameters: 
//	1 - the id-string to test (may be an element-objects' Id).

ekFlexMenu.private_serverHelper_initialize = __ekFlexMenu_static_serverHelper_initialize;
// Calls initialization code, to configure and pre-open select menus.
// Attempts to obtain a ekFlexMenu object, and then calls its' 
// initializeWithServerVariables() method...
// Parameters: 
//	1 - The standard menu-submenu-id string (submenu-id and extra characters discarded).

ekFlexMenu.private_startupAllSmartMenus = __ekFlexMenu_static_serverHelper_startupAllSmartMenus;
// Ensures that all ekFlexMenu objects have been initialized.
// Parameters: 
//	None.

ekFlexMenu.private_shutdownAllSmartMenus = __ekFlexMenu_static_serverHelper_shutdownAllSmartMenus;
// Ensures that all ekFlexMenu objects have been un-initialized (allows clean-up, if needed).
// Parameters: 
//	None.

ekFlexMenu.private_getMenuId = __ekFlexMenu_static_getMenuId;
// Returns the base (root) Menu-ID number, for a given ekflexmenu element ID.
// Parameters: 
//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).

ekFlexMenu.private_getMenuIdString = __ekFlexMenu_static_getMenuIdString;
// Returns the base (root) Menu-ID String, for a given ekflexmenu element ID.
// Parameters: 
//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).

ekFlexMenu.private_getSubmenuId = __ekFlexMenu_static_getSubmenuId;
// Returns the Submenu-ID number, for a given ekflexmenu element ID.
// Parameters: 
//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).

ekFlexMenu.private_getSubmenuIdString = __ekFlexMenu_static_getSubmenuIdString;
// Returns the Submenu-ID String, for a given ekflexmenu element ID.
// Parameters: 
//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).

ekFlexMenu.private_getEvent = __ekFlexMenu_static_getEvent;
// Returns the event object.
// Parameters: 

ekFlexMenu.private_getEventElement = __ekFlexMenu_static_getEventElement;
// Returns the element object that triggered the event.
// Parameters: 
//	1 - the event (may be null if browser is IE).

ekFlexMenu.private_getIntNumber = __ekFlexMenu_static_getIntNumber;
// Returns the decimal equivelent of the given string value, 
// or zero (0) if supplied string value is not a number.
// Parameters: 
//	1 - the string to convert to a number.

ekFlexMenu.private_isValidSubmenuObj = __ekFlexMenu_static_isValidSubmenuObj;
// Verifies that element is a valid submenu object.
// Parameters: 
//	1 - the submenu object to test.
//	2 - the class-name to compare (may be a fragment, which 
//	    is useful if the class name can vary - such as 
//	    "ekflexmenu_button" and "ekflexmenu_button_selected").

ekFlexMenu.private_isValidSubmenuButton = __ekFlexMenu_static_isValidSubmenuButton;
// Verifies that element object is a valid submenu button.
// Parameters: 
//	1 - the button object to test.

ekFlexMenu.private_isValidSubmenuItems = __ekFlexMenu_static_isValidSubmenuItems;
// Verifies that element object is a valid submenu submenu_items.
// Parameters: 
//	1 - the submenu-items object to test.

ekFlexMenu.private_isValidSubmenu = __ekFlexMenu_static_isValidSubmenu;
// Verifies that element object is a valid submenu submenu.
// Parameters: 
//	1 - the submenu object to test.

ekFlexMenu.private_isValidSubmenuLink = __ekFlexMenu_static_isValidSubmenuLink;
// Verifies that element object is a valid submenu link.
// Parameters: 
//	1 - the submenu-link object to test.

ekFlexMenu.private_isValidEKMenu = __ekFlexMenu_static_isValidEKMenu;
// Verifies that element object is a valid main ekflexmenu object.
// Parameters: 
//	1 - the main-ekflexmenu-object to test.

ekFlexMenu.private_isDefined = __ekFlexMenu_static_isDefined;
// Verifies that the passed in object is not undefined.
// Parameters: 
//	1 - the object to test.

ekFlexMenu.isDefinedNotNull = __ekFlexMenu_static_isDefinedNotNull;
// Verifies that the passed in object is not undefined, and is not null.
// Parameters: 
//	1 - the main-ekflexmenu-object to test.

ekFlexMenu.hasClassName = __ekFlexMenu_static_hasClassName;
// Tests for the presence of a specified classname in the supplied object.
// Parameters: 
//	1 - the object to test.
//  2 - the classname to search for.

ekFlexMenu.addClassName = __ekFlexMenu_static_addClassName;
// Ensures that the given object has the specified classname.
// Parameters: 
//	1 - the object to update.
//  2 - the classname to add.

ekFlexMenu.removeClassName = __ekFlexMenu_static_removeClassName;
// Ensures that the given object does not have the specified classname.
// Parameters: 
//	1 - the object to update.
//  2 - the classname to remove.

ekFlexMenu.submenuIsTopLevel = __ekFlexMenu_static_submenuIsTopLevel;
// Returns true if the string points to the top level.
// Parameters: 
//	1 - A standard menu-submenu-id string (extra characters are discarded).

ekFlexMenu.clearSiblingButtons = __ekFlexMenu_clearSiblingButtons;
// Called when mouseover menu-links to unselect sibling-submenus.
// ...Only wired-up when enableMouseOverPopUp is true:
// Parameters: 
//	1 - the calling element.

// Ajax static functions:
ekFlexMenu.ajaxCallBack_stateChange = __ekFlexMenu_ajaxCallBack_stateChange;
ekFlexMenu.ajaxGetMenuObj = __ekFlexMenu_ajaxGetMenuObj
ekFlexMenu.ajaxCancelServerCall = __ekFlexMenu_static_ajaxCancelServerCall;


// Static variables:
ekFlexMenu.static_userAjaxXmlHttp1 = null;

// Constants:
ekFlexMenu.private_menuPrefix = "e"; // ekFlexMenu.private_menuPrefix = "ekfxmensel_";
ekFlexMenu.private_hashLength = 9; /* Eight character hexadecimal hash code, prefixed with "e" */
ekFlexMenu.private_namePrefix = "ekmengrp_";
//Update: no longer used: ekFlexMenu.private_submenuDelimiter = "_submenu_";
ekFlexMenu.private_buttonElementIdPostFix = "_button";
ekFlexMenu.private_submenuItemsElementIdPostFix = "_submenu_items";
ekFlexMenu.private_parentIdElementIdPostFix = "_parentid";
ekFlexMenu.private_ekflexmenuContainerElementIdPostFix = "_ekflexmenu"


//*********************************************************
// ekFlexMenu Static Member Definitions Begin:
//*********************************************************

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function
// Returns the Menu-Object for a given ekflexmenu element ID,
// creates a new ekFlexMenu object if needed (stores these in
// an array as a property to the window object - making it 
// available/shared with all SmartMenus (this way multiple menus 
// can exist on a page, and have different objects that 
// are available anywhere - given an ekflexmenu element ID).
function __ekFlexMenu_static_getMenuObj(elementId) {
    var menuObj = null;
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(elementId);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        //var menuIdString = ekFlexMenu.private_getMenuIdString(menuSubmenuId);
        var menuHashCode = ekFlexMenu.parseServerControlHash(menuSubmenuId);

        if (("undefined" == typeof window.ekFlexMenu_MenuObjArray)
			|| (null == window.ekFlexMenu_MenuObjArray)) {
            var MenuObjArray = new Array;
            menuObj = new ekFlexMenu(menuSubmenuId);
            MenuObjArray[menuHashCode] = menuObj;
            window.ekFlexMenu_MenuObjArray = MenuObjArray;
        }
        else if (null == window.ekFlexMenu_MenuObjArray[menuHashCode]) {
            window.ekFlexMenu_MenuObjArray[menuHashCode] = menuObj = new ekFlexMenu(menuSubmenuId);
        }
        else {
            menuObj = window.ekFlexMenu_MenuObjArray[menuHashCode];
        }
    }
    return (menuObj);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the MenuSubmenuID string, for a given ekflexmenu element-ID
// (ex. given "e1234567_1_submenu_2_button" returns
// "e1234567_1_submenu_2" for root-menu 1, submenu 2):
//
// Update:
// Now prefixed with server-control IDs' hexidecimal hash-code:
// (ex. given "c580fa7b_1_2_button" returns
// "c580fa7b_1_2" for root-menu 1, submenu 2):
function __ekFlexMenu_static_parseMenuSubmenuIdString(elementId) {
    var result = "";
    if (elementId
		&& ("undefined" != typeof elementId)
		&& ("undefined" != typeof elementId.length)
		&& (elementId.length > 0)
		&& ("undefined" != typeof elementId.indexOf)) {

        var frag = elementId.split("_");
        if (frag[0] && (ekFlexMenu.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
            result = frag[0] + "_" + frag[1] + "_" + frag[2];
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the server controls' hash-code of the supplied string (or
// whatever was supplied if not a valid menu-submenu id string):
function __ekFlexMenu_static_parseServerControlHash(id) {
    var result = "";
    if (id && id.split) {
        var frag = id.split("_");
        if (frag[0] && (ekFlexMenu.private_hashLength == frag[0].length)) {
            result = frag[0];
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the event object:
function __ekFlexMenu_static_getEvent(e) {
    if (e) return (e);
    else return (window.event);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the element object that triggered the event:
function __ekFlexMenu_static_getEventElement(e) {
    if (e) return ((e.srcElement) ? e.srcElement : e.target);
    else return (null);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-Click events:
function __ekFlexMenu_static_menuButtonClickEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    var result = true;
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && el.parentNode && el.parentNode.nodeName && ("A" == el.parentNode.nodeName)) {
            el = el.parentNode;
        }
        if (el && ("undefined" != el.id)) {
            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                var prevState = menuObj.isSubmenuSelected(el.id);
                if (prevState) {
                    menuObj.unSelectSubmenu(el.id);
                    result = false; // event consumed.
                }
                else {
                    menuObj.selectSubmenu(el.id);
                    result = false; // event consumed.

                    // Attempt to call ajax if enabled, refresh the page 
                    // the with desired submenu open, if the call fails:
                    if (menuObj.private_ajaxEnabled) {
                        if (menuObj.callAjaxForUserClick(el.id)) {
                            // Prevent the "noscript" link from refreshing the page:
                            ev.returnValue = false;
                            //ev.cancelBubble = true;
                            result = false; // event consumed.
                        }
                        else {
                            result = true; // event not consumed; fire href link.
                        }
                    }
                }
            }
        }
        // Test to see if browser should follow link:
        if (ekFlexMenu.isDefinedNotNull(el)) {
            if (ekFlexMenu.isDefinedNotNull(el.href)) {
                if (ekFlexMenu.isDefinedNotNull(el.href.indexOf)) {
                    if (0 > el.href.indexOf("ekfxmen_noscript=1")) {
                        // This is a valid user link: 
                        //   signal event not consumed, and fire href link.
                        result = true;
                    }
                }
            }
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-Keydown events:
// Note: Typically called by a submenu-folder-button when 
//   a key is pressed, and 508-Compliance is disabled. 
function __ekFlexMenu_static_menuButtonKeyDownEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {

            var key = ev.keyCode;
            if (key && ((key == 32))) { // select submenu on space-bar press...

                // Prevent screen from scrolling, due to internal 
                // link-click (anchor-tag, with href="#"):
                if (ev.preventDefault && ev.stopPropagation) {
                    ev.preventDefault();
                    ev.stopPropagation();
                }
                else {
                    ev.returnValue = false;
                }

                // Now toggle the state of the menu:
                ekFlexMenu.menuBtnClickHdlr(ev);

                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-MouseOver events:
function __ekFlexMenu_static_menuButtonMouseOverEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                menuObj.hoverButton(el.id, true);
                menuObj.hoverSubmenu(el.id);
                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-MouseOut events:
function __ekFlexMenu_static_menuButtonMouseOutEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                menuObj.hoverButton(el.id, false);
                menuObj.unHoverSubmenu(el.id);
                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-Focus events:
function __ekFlexMenu_static_menuButtonFocusEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                menuObj.hoverButton(el.id, true);
                menuObj.hoverSubmenu(el.id);
                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-Blur events:
function __ekFlexMenu_static_menuButtonBlurEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                menuObj.hoverButton(el.id, false);
                menuObj.unHoverSubmenu(el.id);
                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-Link-onFocus events:
function __ekFlexMenu_static_menuButtonLinkFocusEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                menuObj.hoverButton(el.id, true);
                menuObj.hoverSubmenu(el.id);
                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Button-Link-onBlur events:
function __ekFlexMenu_static_menuButtonLinkBlurEventHandler(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                menuObj.hoverButton(el.id, false);
                menuObj.unHoverSubmenu(el.id);
                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
function __ekFlexMenu_static_getValidParentId(el) {
    var topContainer = "_ekflexmenu";
    var len = topContainer.length;

    while (el
		&& (el.parentNode)) {

        if (el.id && (el.id.length)) {
            if (ekFlexMenu.private_isValidMenuSubmenuIdString(el.id)) {
                return (el.id);
            }
        }
        el = el.parentNode;
    }

    return ("");
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// mouseIn event handler; prepares for delayed opening of 
// the submenu related to the event-triggering element.
// Parameters: 
//	1 - the event-object.
function __ekFlexMenu_static_mouseIn(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var elmtId = el.id;
            if (0 == elmtId.length) {
                elmtId = __ekFlexMenu_static_getValidParentId(el);
            }
            var menuObj = ekFlexMenu.getMenuObj(elmtId);
            if (menuObj) {
                menuObj.mouseIn(e, el);
                return (true); // event not-consumed (allow bubbling).
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// mouseOut event handler; prepares for delayed closing of 
// the submenu related to the event-triggering element.
// Parameters: 
//	1 - the event-object.
function __ekFlexMenu_static_mouseOut(e) {
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {
            var elmtId = el.id;
            if (0 == elmtId.length) {
                elmtId = __ekFlexMenu_static_getValidParentId(el);
            }
            var menuObj = ekFlexMenu.getMenuObj(elmtId);
            if (menuObj) {
                menuObj.mouseOut(e, el);
                return (true); // event not-consumed (allow bubbling).
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handler for Menu-Item-Link-Click events:
function __ekFlexMenu_static_menuItemLinkClickEventHandler(e) {
    var linkIsAButton = false;
    var ev = ekFlexMenu.private_getEvent(e);
    if (ev) {
        var el = ekFlexMenu.private_getEventElement(ev);
        if (el && ("undefined" != el.id)) {

            if (ekFlexMenu.isDefinedNotNull(el)
				&& ekFlexMenu.isDefinedNotNull(el.tagName)
				&& ("IMG" == el.tagName)) {
                // The element is an image, attempt to pass
                // -off the event to the wrapping element: 
                if (ekFlexMenu.isDefinedNotNull(el.parentNode)
					&& ekFlexMenu.private_isValidSubmenuButton(el.parentNode)) {
                    el = el.parentNode;
                    if (ekFlexMenu.isDefinedNotNull(el.click)) {
                        el.click(ev);
                        return (false);
                    }
                    linkIsAButton = true;
                }
                else {
                    return (true);
                }
            }
            else if (ekFlexMenu.private_isValidSubmenuButton(el)) {
                linkIsAButton = true;
            }

            var menuObj = ekFlexMenu.getMenuObj(el.id);
            if (menuObj) {
                // may need to toggle menu state if the link is a menu button:				
                if (linkIsAButton) {
                    var prevState = menuObj.isSubmenuSelected(el.id);
                    if (prevState) {
                        menuObj.unSelectSubmenu(el.id);
                    }
                    else {
                        menuObj.selectSubmenu(el.id);
                    }
                }
                menuObj.selectMenuItem(el);
                return (false); // event consumed.
            }
        }
    }
    return (true);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the decimal equivelent of the given string value, 
// or zero (0) if supplied string value is not a number:
function __ekFlexMenu_static_getIntNumber(val) {
    var result = 0;
    var tempResult = parseInt(val, 10);
    if (NaN != tempResult) {
        result = tempResult;
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the Menu-ID number, for a given ekflexmenu element ID:
function __ekFlexMenu_static_getMenuId(elementId) {
    var result = 0;
    var idString = ekFlexMenu.private_getMenuIdString(elementId);
    if (idString.length) {
        result = ekFlexMenu.private_getIntNumber(idString);
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the Menu-ID String, for a given ekflexmenu element ID:
function __ekFlexMenu_static_getMenuIdString(elementId) {
    var result = "";
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(elementId);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var frag = elementId.split("_");
        if (frag[0] && (ekFlexMenu.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
            result = frag[1];
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the Submenu-ID number, for a given ekflexmenu element ID:
function __ekFlexMenu_static_getSubmenuId(elementId) {
    var result = 0;
    var idString = ekFlexMenu.private_getSubmenuIdString(elementId);
    if (idString.length) {
        result = ekFlexMenu.private_getIntNumber(idString);
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns the Submenu-ID String, for a given ekflexmenu element ID:
function __ekFlexMenu_static_getSubmenuIdString(elementId) {
    var result = "";
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(elementId)) {
        var frag = elementId.split("_");
        if (frag[0] && (ekFlexMenu.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
            result = frag[2];
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Verifies that the supplied element-ID string is a valid 
// MenuSubmenuID string (ex. "e1234567_1_submenu_2_button")
function __ekFlexMenu_static_isValidMenuSubmenuIdString(elementId) {
    var result = false;
    if (elementId
		&& ("undefined" != typeof elementId)
		&& ("undefined" != typeof elementId.length)
		&& (elementId.length > 0)
		&& ("undefined" != typeof elementId.indexOf)) {
        var frag = elementId.split("_");
        if (frag[0] && (ekFlexMenu.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
            result = true;
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Verifies that element is a valid submenu object:
function __ekFlexMenu_static_isValidSubmenuObj(obj, classNameFrag) {
    var result = false;
    if (obj
		&& ("undefined" != typeof obj.id)
		&& ("undefined" != typeof obj.className)
		&& ("undefined" != typeof obj.className.indexOf)
		&& (0 <= obj.className.indexOf(classNameFrag))) {
        result = true;
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Verifies that element object is a valid submenu button:
function __ekFlexMenu_static_isValidSubmenuButton(obj) {
    return (ekFlexMenu.private_isValidSubmenuObj(obj, ekFlexMenu_classNames.button));
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Verifies that element object is a valid submenu submenu_items:
function __ekFlexMenu_static_isValidSubmenuItems(obj) {
    return (ekFlexMenu.private_isValidSubmenuObj(obj, "submenu_items"));
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Verifies that element object is a valid submenu submenu:
function __ekFlexMenu_static_isValidSubmenu(obj) {
    return (ekFlexMenu.private_isValidSubmenuObj(obj, "submenu"));
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Verifies that element object is a valid submenu link:
function __ekFlexMenu_static_isValidSubmenuLink(obj) {
    return (ekFlexMenu.private_isValidSubmenuObj(obj, "link"));
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Verifies that element object is a valid main ekflexmenu object:
function __ekFlexMenu_static_isValidEKMenu(obj) {
    return (ekFlexMenu.private_isValidSubmenuObj(obj, "ekflexmenu"));
}

///////////////////////////////////////////////////////////
// Verifies that the passed in object is not undefined.
// Parameters: 
//	1 - the main-ekflexmenu-object to test.
function __ekFlexMenu_static_isDefined(obj) {
    return ("undefined" != typeof obj);
}

///////////////////////////////////////////////////////////
// Verifies that the passed in object is not 
// undefined, and is not null.
// Parameters: 
//	1 - the main-ekflexmenu-object to test.
function __ekFlexMenu_static_isDefinedNotNull(obj) {
    return (ekFlexMenu.private_isDefined(obj) && (null != obj));
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Calls initialization code, to configure and pre-open select menus:
function __ekFlexMenu_static_serverHelper_initialize(id) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(id);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        //var menuHashCode = ekFlexMenu.parseServerControlHash(menuSubmenuId);
        var menuObj = ekFlexMenu.getMenuObj(menuSubmenuId);
        if (menuObj) {
            menuObj.initializeWithServerVariables();
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Ensures that all ekFlexMenu objects have been initialized:
function __ekFlexMenu_static_serverHelper_startupAllSmartMenus() {
    if (("undefined" != typeof window.ekFlexMenu_ekflexmenuArray)
		&& (null != window.ekFlexMenu_ekflexmenuArray)
		&& ("undefined" != typeof window.ekFlexMenu_ekflexmenuArray.length)
		&& (null != window.ekFlexMenu_ekflexmenuArray.length)) {

        for (var idx = 0; idx < window.ekFlexMenu_ekflexmenuArray.length; idx++) {
            var startMenu = window.ekFlexMenu_ekflexmenuArray[idx];
            if (startMenu.length)
                ekFlexMenu.private_serverHelper_initialize(startMenu);
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Ensures that all ekFlexMenu objects have been initialized:
function __ekFlexMenu_static_serverHelper_shutdownAllSmartMenus() {
    ekFlexMenu.ajaxCancelServerCall();
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Terminates any outstanding Ajax calls:
function __ekFlexMenu_static_ajaxCancelServerCall() {
    if (ekFlexMenu.static_userAjaxXmlHttp1 != null) {
        ekFlexMenu.static_userAjaxXmlHttp1.abort();
        ekFlexMenu.static_userAjaxXmlHttp1 = null;
    }
}



///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Tests for the presence of a specified classname in the supplied object.
function __ekFlexMenu_static_hasClassName(obj, className) {
    var idx, names;
    if (obj && ("undefined" != typeof obj.className)
		&& ("undefined" != typeof obj.className.split)) {
        names = obj.className.split(" ");
        for (idx = 0; idx < names.length; idx++) {
            if (names[idx] == className)
                return true;
        }
    }
    return false;
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Ensures that the given object has the specified classname.
function __ekFlexMenu_static_addClassName(obj, className) {
    if (ekFlexMenu.hasClassName(obj, className))
        return;

    if (obj && ("undefined" != typeof obj.className)
		&& ("undefined" != typeof obj.className.length)) {
        if (0 == obj.className.length) {
            obj.className = className;
        }
        else {
            obj.className += " " + className;
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Ensures that the given object does not have the specified classname.
function __ekFlexMenu_static_removeClassName(obj, className) {
    var idx, matchId, names, result;
    if (obj && ("undefined" != typeof obj.className)
		&& ("undefined" != typeof obj.className.split)) {
        names = obj.className.split(" ");
        obj.className = "";
        for (idx = 0; idx < names.length; idx++) {
            if (names[idx] != className) {
                if (idx > 0)
                    obj.className += " " + names[idx];
                else
                    obj.className += names[idx];
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Returns true if the string points to the top level.
function __ekFlexMenu_static_submenuIsTopLevel(elementId) {
    var result = false;
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(elementId)) {
        var sub = ekFlexMenu.private_getSubmenuIdString(elementId);
        var ancestorId = ekFlexMenu.private_getMenuIdString(elementId);
        result = ((0 == sub) || (ancestorId == sub));
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Handle Ajax callback:
function __ekFlexMenu_ajaxCallBack_stateChange() {
    if (ekFlexMenu.static_userAjaxXmlHttp1.readyState == 4) {
        if (ekFlexMenu.isDefinedNotNull(ekFlexMenu.static_userAjaxXmlHttp1.status)) {
            if (200 <= ekFlexMenu.static_userAjaxXmlHttp1.status < 300) {
                var menuObj = ekFlexMenu.ajaxGetMenuObj(ekFlexMenu.static_userAjaxXmlHttp1.responseText);
                if (menuObj) {
                    var resType = ekFlexMenu.static_userAjaxXmlHttp1.getResponseHeader('Content-Type');
                    if (0 > resType.indexOf("text/xml")) {
                        menuObj.appendText(menuObj.userAjaxParentId1);
                    }
                    else {
                        menuObj.appendXml(menuObj.userAjaxParentId1);
                    }
                }
            }
            else {
                // TODO: flag failure, and use links' href paramter as fallback...
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// Ajax helper function:
function __ekFlexMenu_ajaxGetMenuObj(reqStr) {
    var result = null;
    var idx = reqStr.indexOf("ekflexmenu");
    if (idx >= 0) {
        var tempStr = reqStr.substr(idx);
        var matchStr = " id=";
        idx = tempStr.indexOf(matchStr);
        if (idx >= 0) {
            tempStr = tempStr.substr(idx + matchStr.length);
            idx = tempStr.indexOf(ekFlexMenu.private_menuPrefix);
            if (idx >= 0) {
                tempStr = tempStr.substr(idx);
                tempStr = ekFlexMenu.parseMenuSubmenuIdString(tempStr);
                result = ekFlexMenu.getMenuObj(tempStr);
            }
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Static Member Helper Function.
// clearSiblingButtons:
// Called when mouseover menu-links to unselect sibling-submenus.
// ...Only wired-up when enableMouseOverPopUp is true:
function __ekFlexMenu_clearSiblingButtons(obj) {
    // find sibling-submenus, set them to unselected and hide their children:
    if (obj && obj.parentNode && obj.parentNode.parentNode && obj.parentNode.parentNode.id) {
        $ektron("#" + obj.parentNode.parentNode.id + " > li.ekflexmenu_submenu").find(" > ul").removeClass("ekflexmenu_submenu_items").addClass("ekflexmenu_submenu_items_hidden").end().find(" > a.ekflexmenu_button_selected").removeClass("ekflexmenu_button_selected").addClass("ekflexmenu_button");
    }
    return (true);
}


//*********************************************************
// ekFlexMenu Instance Member Definitions Begin:
//*********************************************************

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the root-menu id string.
function __ekFlexMenu_returnMenuId() {
    return (this.private_menuIdString);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the hash-code of the server control.
function __ekFlexMenu_returnHashCode() {
    return (this.private_serverControlHash);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the root-menu id string of the supplied string (or
// whatever was supplied if not a valid menu-submenu id string):
function __ekFlexMenu_parseMenuId(id) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(id);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        return (ekFlexMenu.private_getMenuIdString(menuSubmenuId));
    }
    else {
        return (id);
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns boolean, indicating if identified button is 
// currently selected (and therefore, then the associated
// visibility state of the identified submenu items):
function __ekFlexMenu_isSubmenuSelected(idString) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var btnObj = this.getFolderButtonObject(menuSubmenuId);
        if (ekFlexMenu.private_isValidSubmenuButton(btnObj)) {
            return (ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonSelected)
				|| ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonSelectedHover));
        }
    }
    return (false);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Selects the identified menu; if there is a folder-button, 
// then the class is updated to selected state. Then shows 
// the associated submenu items:
function __ekFlexMenu_selectSubmenu(idString, optional_fromMouseIn) {
    var defaultFromMouseIn = false;
    if (ekFlexMenu.isDefinedNotNull(optional_fromMouseIn)) {
        defaultFromMouseIn = optional_fromMouseIn;
    }

    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        if (this.private_autoCollapseSubmenus) {
            if (this.private_isSlaveControl && defaultFromMouseIn) {
                this.collapseUnselectedStartLevelSubmenus();
            }
            else {
                this.collapseAllOpenSubmenus(false);
            }
        }
        this.private_selectedMenuList = menuSubmenuId;
        this.selectSubmenuHelper(menuSubmenuId);

        if (!defaultFromMouseIn) {
            this.callSlave__showSubmenuBranch(idString);
        }

        this.private_selectionChanged = true;
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_getSlaveControlObject() {
    var result = null;
    if (this.private_isMasterControl
		&& ekFlexMenu.isDefinedNotNull(this.private_slaveControl)
		&& ekFlexMenu.isDefinedNotNull(this.private_slaveControl.length)
		&& (this.private_slaveControl.length > 0)) {
        var slaveId = this.private_slaveControl + "_" + this.menuId() + "_0";
        var slaveObj = ekFlexMenu.getMenuObj(slaveId);
        if (slaveObj) {
            result = slaveObj;
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_callSlave__showSubmenuBranch(idString) {
    var slaveObj = this.getSlaveControlObject();
    var btnContainer;
    if (slaveObj) {
        if (slaveObj.showSubmenuBranch(this.convertIdToSlaveControlId(slaveObj, idString))) {
            // slave menu succesfully activated, mark top button properly,
            // after ensuring all other top-level-buttons are un-marked:
            for (var ui in this.topLevelUI) {
                //btnContainer = document.getElementById(ui);
                btnContainer = document.getElementById(ui);
                if (btnContainer) {
                    if (ekFlexMenu.hasClassName(btnContainer, ekFlexMenu_classNames.slaveBranchSelected)) {
                        ekFlexMenu.removeClassName(btnContainer, ekFlexMenu_classNames.slaveBranchSelected);
                    }
                }
            }

            var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
            var parentId = this.getParentLevelSubmenuId(menuSubmenuId);
            while ((parentId != menuSubmenuId) && (0 != ekFlexMenu.private_getSubmenuId(parentId))) {
                if (this.isTopLevelUI(parentId)) {
                    btnContainer = document.getElementById(parentId);
                    if (btnContainer) {
                        if (!ekFlexMenu.hasClassName(btnContainer, ekFlexMenu_classNames.slaveBranchSelected)) {
                            ekFlexMenu.addClassName(btnContainer, ekFlexMenu_classNames.slaveBranchSelected);
                        }
                    }
                    break;
                }
                parentId = this.getParentLevelSubmenuId(parentId);
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_initializeSlaveMenu() {
    var isVisible = false;
    var firstObj = null;
    var menuSubmenuId;
    var inAr = this.getEkFlexMenuElementsByTagName("INPUT");
    for (var idx = 0; idx < inAr.length; idx++) {
        if (inAr[idx].value.indexOf(this.private_masterControlIdHash) == 0) {
            var localId = inAr[idx].id;
            var obj;
            if (localId.length >= ekFlexMenu.private_hashLength) {
                localId = this.buildMenuSubmenuId(ekFlexMenu.private_getSubmenuIdString(localId)) + ekFlexMenu.private_submenuItemsElementIdPostFix;
                obj = document.getElementById(localId);
                if (obj) {
                    if (null == firstObj) {
                        firstObj = obj;
                    }

                    if (ekFlexMenu.hasClassName(obj, ekFlexMenu_classNames.submenuItems)) {
                        isVisible = true;
                    }

                    if (null == this.topLevelUI) {
                        this.topLevelUI = new Array;
                    }
                    menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(localId);
                    if (!this.topLevelUI[menuSubmenuId]) {
                        this.topLevelUI[menuSubmenuId] = true;
                    }
                }
            }
        }
    }

    if ((!isVisible) && firstObj) {
        ekFlexMenu.removeClassName(firstObj, ekFlexMenu_classNames.submenuItemsHidden);
        ekFlexMenu.addClassName(firstObj, ekFlexMenu_classNames.submenuItems);
        //this.selectSubmenu(firstSubmenu);
    }

    if (this.ekFlexMenu_defaultMenuIdString) {
        this.ekFlexMenu_slaveStartLevel = this.getMenuLevel(this.ekFlexMenu_defaultMenuIdString);
    }
    else if (this.ekFlexMenu_slaveStartLevelIds && this.ekFlexMenu_slaveStartLevelIds.length > 0) {
        this.ekFlexMenu_slaveStartLevel = this.getMenuLevel(this.ekFlexMenu_slaveStartLevelIds[0]);
    }
    else if (this.topLevelUI) {
        this.ekFlexMenu_slaveStartLevel = this.getMenuLevel(this.topLevelUI[0]);
    }

    if (this.private_preventHidingSlaveMenu && this.isSlaveMenuHiding()) {
        var menuId = this.getLastSlaveStartLevelMenu();
        this.selectSubmenuHelper(menuId);
        this.recordLastSlaveStartLevelMenu(menuId);
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_initializeMasterMenu() {
    var menuSubmenuId = this.buildMenuSubmenuId(0) + ekFlexMenu.private_submenuItemsElementIdPostFix;
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var node = document.getElementById(menuSubmenuId);
        if (node) {
            var nodes = node.childNodes;
            for (var idx = 0; idx < nodes.length; idx++) {
                if (null == this.topLevelUI) {
                    this.topLevelUI = new Array;
                }
                menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(nodes[idx].id);
                if (!this.topLevelUI[menuSubmenuId]) {
                    this.topLevelUI[menuSubmenuId] = true;
                }
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Called from master to select slave submenu.
function __ekFlexMenu_showSubmenuBranch(idString) {
    var result = false;
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var id = menuSubmenuId;
        var obj = document.getElementById(id);
        if (obj) {
            //if (this.private_autoCollapseSubmenus) {
            //	this.collapseAllOpenSubmenus(false);
            //}

            for (var ui in this.topLevelUI) {
                this.unSelectSubmenu(ui, true);
            }

            var lastId = this.getLastSlaveStartLevelMenu();
            if (lastId && lastId.length > 0) {
                this.unSelectSubmenu(lastId, true);
            }
            else if (this.private_isSlaveControl && this.ekFlexMenu_defaultMenuIdString) {
                this.unSelectSubmenu(this.ekFlexMenu_defaultMenuIdString);
            }
            else if (this.private_isSlaveControl) {
                // unknown current submenu; hide all at start level:
                this.collapseAllOpenSubmenus();
            }

            // if enabled, prevent a situation from causing the slave to be empty / hidden:
            if (this.private_preventHidingSlaveMenu
			    && this.getMenuLevel(menuSubmenuId) >= 0
			    && this.getMenuLevel(menuSubmenuId) < this.ekFlexMenu_slaveStartLevel) {
                menuSubmenuId = this.getLastSlaveStartLevelMenu();
            }

            this.selectSubmenuHelper(menuSubmenuId);
            this.recordLastSlaveStartLevelMenu(menuSubmenuId);
            result = true;
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_unHideSlaveMenu() {
    return; // DO NOT DO USE THIS METHOD!!!
    var targId = this.getLastSlaveStartLevelMenu();
    if (targId && (targId.length > 0)) {
        this.selectSubmenuHelper(targId);
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_recordLastSlaveStartLevelMenu(id) {
    if (id && this.ekFlexMenu_slaveStartLevelIds[id]) {
        this.lastSelectedStartLevelSlaveMenuId = id;
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_getLastSlaveStartLevelMenu() {
    if (this.lastSelectedStartLevelSlaveMenuId && (this.lastSelectedStartLevelSlaveMenuId.length > 0)) {
        return (this.lastSelectedStartLevelSlaveMenuId);
    }
    else if (this.ekFlexMenu_defaultMenuIdString && this.ekFlexMenu_defaultMenuIdString.length > 0) {
        return (this.ekFlexMenu_defaultMenuIdString);
    }
    else {
        return ("");
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns true if slave menus contents are all hidden.
function __ekFlexMenu_isSlaveMenuHiding() {
    var id = this.getEkFlexMenuContainerElement().id;
    var startLevel = this.ekFlexMenu_slaveStartLevel.toString();
    return ($ektron("#" + id + " .ekflexmenu_menu_level_" + startLevel + " .ekflexmenu_submenu_items").length == 0);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_unSelectSubmenuList(menuList) {
    var listAr = menuList.split(",");
    var idx;
    for (idx = 0; idx < listAr.length; idx++) {
        this.unSelectSubmenu(listAr[idx]);
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_convertIdToSlaveControlId(slaveObj, idString) {
    var result = idString;
    if (slaveObj && idString && idString.length && (idString.length >= ekFlexMenu.private_hashLength)) {
        result = slaveObj.hashCode() + idString.substr(ekFlexMenu.private_hashLength);
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_isTopLevelUI(idString) {
    return (this.topLevelUI && this.topLevelUI[ekFlexMenu.parseMenuSubmenuIdString(idString)]);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Helper funtion for __ekFlexMenu_selectSubmenu, uses 
// recursionSelects to ensure selected submenus are visible
// even if they are buried with muliple nesting levels:
function __ekFlexMenu_selectSubmenuHelper(idString) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        if (this.private_selectedMenuList.length) {
            this.private_selectedMenuList += "," + menuSubmenuId;
        }
        else {
            this.private_selectedMenuList = menuSubmenuId;
        }

        var btnObj = this.getFolderButtonObject(menuSubmenuId);
        if (ekFlexMenu.private_isValidSubmenuButton(btnObj)) {
            var wasHovering = (ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonHover)
				|| ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonSelectedHover));
            if (wasHovering) {
                ekFlexMenu.removeClassName(btnObj, ekFlexMenu_classNames.buttonHover);
                ekFlexMenu.addClassName(btnObj, ekFlexMenu_classNames.buttonSelectedHover);
            }
            else {
                ekFlexMenu.removeClassName(btnObj, ekFlexMenu_classNames.button);
                ekFlexMenu.addClassName(btnObj, ekFlexMenu_classNames.buttonSelected);
            }
        }

        var itmObj = this.getSubmenuItemsObject(menuSubmenuId);
        if (ekFlexMenu.private_isValidSubmenuItems(itmObj)) {
            ekFlexMenu.removeClassName(itmObj, ekFlexMenu_classNames.submenuItemsHidden);
            ekFlexMenu.addClassName(itmObj, ekFlexMenu_classNames.submenuItems);
        }

        // Ensure parent folders are visible as well, in case
        // we got here from something else than a user click:
        if (!(this.private_isSlaveControl && this.isTopLevelUI(menuSubmenuId))) {
            var parentId = this.getParentLevelSubmenuId(menuSubmenuId);
            if (parentId != menuSubmenuId) {
                this.selectSubmenuHelper(parentId); // recursively call this function until all parents are open.
            }

            this.markParentSubmenu(menuSubmenuId);
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Unselects the identified menu; if there is a folder-button, 
// then the class is updated to an unselected state. Then 
// hides the associated submenu items:
function __ekFlexMenu_unSelectSubmenu(idString, topLevelUIOverride) {
    if (idString && idString.length) {
        var overrideTopLevelUI = false;
        if (ekFlexMenu.isDefinedNotNull(topLevelUIOverride)) {
            overrideTopLevelUI = topLevelUIOverride;
        }
        var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
        if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
            if ((ekFlexMenu.private_getSubmenuId(menuSubmenuId) == 0)
				|| ((ekFlexMenu.private_getSubmenuId(menuSubmenuId) > 0)
				&& (overrideTopLevelUI || !this.private_isSlaveControl || !this.isTopLevelUI(menuSubmenuId)))) {
                var btnObj = this.getFolderButtonObject(menuSubmenuId);
                if (ekFlexMenu.private_isValidSubmenuButton(btnObj)) {
                    var wasHovering = (ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonHover)
						|| ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonSelectedHover));
                    if (wasHovering) {
                        ekFlexMenu.removeClassName(btnObj, ekFlexMenu_classNames.buttonSelectedHover);
                        ekFlexMenu.addClassName(btnObj, ekFlexMenu_classNames.buttonHover);
                    }
                    else {
                        ekFlexMenu.removeClassName(btnObj, ekFlexMenu_classNames.buttonSelected);
                        ekFlexMenu.addClassName(btnObj, ekFlexMenu_classNames.button);
                    }
                }

                var itmObj = this.getSubmenuItemsObject(menuSubmenuId);
                // Don't hide slave containers:
                if (!(this.private_isSlaveControl
					&& (ekFlexMenu.submenuIsTopLevel(menuSubmenuId)
						|| ekFlexMenu.hasClassName(itmObj, ekFlexMenu_classNames.slaveContainer)))) {
                    if (ekFlexMenu.private_isValidSubmenuItems(itmObj)) {
                        ekFlexMenu.removeClassName(itmObj, ekFlexMenu_classNames.submenuItems);
                        ekFlexMenu.addClassName(itmObj, ekFlexMenu_classNames.submenuItemsHidden);
                    }
                }

                this.unMarkParentSubmenu(menuSubmenuId);
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Closes all currently open submenus, to prevent overlap & visual clutter:
function __ekFlexMenu_collapseAllOpenSubmenus(showRootFlag) {
    if (this.private_selectionChanged) {
        this.unSelectSubmenuList(this.private_selectedMenuList);
        this.private_selectedMenuList = ""
    }
    else {
        var openMenusArray = this.getElementsByClassNameAndTagName(ekFlexMenu_classNames.submenuItems, "UL");

        // hide all visible submenus:
        for (var idx = 0; idx < openMenusArray.length; idx++) {
            this.unSelectSubmenu(openMenusArray[idx].id);
        }

        // TODO: FIX: ensure all buttons are disabled (should be done 
        // by previous step, but this fails for master/slave menus):
        var activeButtons = this.getElementsByClassNameAndTagName(ekFlexMenu_classNames.buttonSelected, "SPAN");
        for (idx = 0; idx < activeButtons.length; idx++) {
            this.unSelectSubmenu(activeButtons[idx].id);
        }
    }

    // Now that all menus have been hdden, determine 
    // if the the root-menu should be made visible:
    if ("undefined" != typeof showRootFlag) {
        // parameter was passed, use it to control/override defalt behaviour:
        if (showRootFlag)
            this.showRootMenu();
    }
    else {
        // use default behaviour:
        if (!this.private_startWithRootFolderCollapsed)
            this.showRootMenu();
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Used for slave menu; Closes all unselected submenus:
function __ekFlexMenu_collapseUnselectedStartLevelSubmenus() {
    var lastMenuId = this.getLastSlaveStartLevelMenu();
    var openMenusArray = this.getElementsByClassNameAndTagName(ekFlexMenu_classNames.submenuItems, "UL");

    // hide all visible submenus:
    for (var idx = 0; idx < openMenusArray.length; idx++) {
        if (lastMenuId != ekFlexMenu.parseMenuSubmenuIdString(openMenusArray[idx].id)) {
            this.unSelectSubmenu(openMenusArray[idx].id);
        }
    }

    // TODO: FIX: ensure all buttons are disabled (should be done 
    // by previous step, but this fails for master/slave menus):
    var activeButtons = this.getElementsByClassNameAndTagName(ekFlexMenu_classNames.buttonSelected, "SPAN");
    for (idx = 0; idx < activeButtons.length; idx++) {
        this.unSelectSubmenu(activeButtons[idx].id);
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Hide sibling submenus of the designated submenu:
function __ekFlexMenu_collapseSiblingSubmenus(idString) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var parentLevelId = this.getParentLevelSubmenuId(menuSubmenuId);
        if (ekFlexMenu.private_isValidMenuSubmenuIdString(parentLevelId)
			&& (parentLevelId != menuSubmenuId)) {
            var idArray = this.getDirectChildIds(parentLevelId)
            for (var idx = 0; idx < idArray.length; idx++) {
                if (idArray[idx] != menuSubmenuId) {
                    this.unSelectSubmenu(idArray[idx]);
                }
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Sets the parent folders' style to be a parent (optionally 
// used in CSS to style parents differently):
function __ekFlexMenu_markParentSubmenu(idString) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var parentLevelId = this.getParentLevelSubmenuId(menuSubmenuId);
        if (ekFlexMenu.private_isValidMenuSubmenuIdString(parentLevelId)
			&& (parentLevelId != menuSubmenuId)) {
            var parentObj = this.getSubmenuObject(parentLevelId);
            if ((ekFlexMenu.isDefinedNotNull(parentObj))
				&& (ekFlexMenu.private_isDefined(parentObj.className))) {
                if (ekFlexMenu.hasClassName(parentObj, ekFlexMenu_classNames.submenu)) {
                    ekFlexMenu.removeClassName(parentObj, ekFlexMenu_classNames.submenu);
                    ekFlexMenu.addClassName(parentObj, ekFlexMenu_classNames.submenuParent);
                }
                else if (ekFlexMenu.hasClassName(parentObj, ekFlexMenu_classNames.submenuHover)) {
                    ekFlexMenu.removeClassName(parentObj, ekFlexMenu_classNames.submenuHover);
                    ekFlexMenu.addClassName(parentObj, ekFlexMenu_classNames.submenuParentHover);
                }
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Sets the parent folders' style to be a normal non-parent 
// (optionally used in CSS to style parents & children differently):
function __ekFlexMenu_unMarkParentSubmenu(idString) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var parentLevelId = this.getParentLevelSubmenuId(menuSubmenuId);
        if (ekFlexMenu.private_isValidMenuSubmenuIdString(parentLevelId)
			&& (parentLevelId != menuSubmenuId)) {
            var parentObj = this.getSubmenuObject(parentLevelId);
            if ((ekFlexMenu.isDefinedNotNull(parentObj))
				&& (ekFlexMenu.private_isDefined(parentObj.className))) {
                if (ekFlexMenu.hasClassName(parentObj, ekFlexMenu_classNames.submenuParent)) {
                    ekFlexMenu.removeClassName(parentObj, ekFlexMenu_classNames.submenuParent);
                    ekFlexMenu.addClassName(parentObj, ekFlexMenu_classNames.submenu);
                }
                else if (ekFlexMenu.hasClassName(parentObj, ekFlexMenu_classNames.submenuParentHover)) {
                    ekFlexMenu.removeClassName(parentObj, ekFlexMenu_classNames.submenuParentHover);
                    ekFlexMenu.addClassName(parentObj, ekFlexMenu_classNames.submenuHover);
                }
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Sets the menus' container style to be hovered,
// (optionally used in CSS to style contents & children differently):
function __ekFlexMenu_hoverSubmenu(idString) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var menuObj = this.getSubmenuObject(menuSubmenuId);
        if ((ekFlexMenu.isDefinedNotNull(menuObj))
			&& (ekFlexMenu.private_isDefined(menuObj.className))) {
            if (ekFlexMenu.hasClassName(menuObj, ekFlexMenu_classNames.submenu)) {
                ekFlexMenu.removeClassName(menuObj, ekFlexMenu_classNames.submenu);
                ekFlexMenu.addClassName(menuObj, ekFlexMenu_classNames.submenuHover);
            }
            else if (ekFlexMenu.hasClassName(menuObj, ekFlexMenu_classNames.submenuParent)) {
                ekFlexMenu.removeClassName(menuObj, ekFlexMenu_classNames.submenuParent);
                ekFlexMenu.addClassName(menuObj, ekFlexMenu_classNames.submenuParentHover);
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Sets the menus' container style to be hovered,
// (optionally used in CSS to style contents & children differently):
function __ekFlexMenu_unHoverSubmenu(idString) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var menuObj = this.getSubmenuObject(menuSubmenuId);
        if ((ekFlexMenu.isDefinedNotNull(menuObj))
			&& (ekFlexMenu.private_isDefined(menuObj.className))) {
            if (ekFlexMenu.hasClassName(menuObj, ekFlexMenu_classNames.submenuHover)) {
                ekFlexMenu.removeClassName(menuObj, ekFlexMenu_classNames.submenuHover);
                ekFlexMenu.addClassName(menuObj, ekFlexMenu_classNames.submenu);
            }
            else if (ekFlexMenu.hasClassName(menuObj, ekFlexMenu_classNames.submenuParentHover)) {
                ekFlexMenu.removeClassName(menuObj, ekFlexMenu_classNames.submenuParentHover);
                ekFlexMenu.addClassName(menuObj, ekFlexMenu_classNames.submenuParent);
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns an array of all direct child-submenu-ids (length = 0 if none):
function __ekFlexMenu_getDirectChildIds(idString) {
    var result = new Array;
    var elementName = ekFlexMenu.private_namePrefix + "submenu_items";
    var cmpId, elementArray;
    var parentMenuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(parentMenuSubmenuId)) {
        elementArray = this.getEkFlexMenuElementsByName(elementName);
        if (("undefined" != typeof elementArray)
			&& (null != elementArray)
			&& ("undefined" != typeof elementArray.length)
			&& (null != elementArray.length)) {
            for (var idx = 0; idx < elementArray.length; idx++) {
                cmpId = ekFlexMenu.parseMenuSubmenuIdString(elementArray[idx].id);
                if (ekFlexMenu.private_isValidMenuSubmenuIdString(cmpId)) {
                    if ((parentMenuSubmenuId == this.getParentLevelSubmenuId(cmpId)
							&& (parentMenuSubmenuId != cmpId))) {  //ekFlexMenu.private_getSubmenuIdString
                        result[result.length] = cmpId;
                    }
                }
            }
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// returns an array of the menu-elements whose name 
// attribute matches the supplied name:
function __ekFlexMenu_getEkFlexMenuElementsByName(elementName) {
    var result = new Array;
    var divArray = this.getEkFlexMenuElementsByTagName("div");
    for (var idx = 0; idx < divArray.length; idx++) {
        if (elementName == divArray[idx].name) {
            result[result.length] = divArray[idx];
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// returns an array of the ekflexmenu-elements with the 
// specified tag-name: 
function __ekFlexMenu_getEkFlexMenuElementsByTagName(tagName) {
    var result = new Array;
    var ekflexmenuContainer = this.getEkFlexMenuContainerElement();
    if (ekflexmenuContainer && ("undefined" != typeof ekflexmenuContainer.getElementsByTagName)) {
        var divArray = ekflexmenuContainer.getElementsByTagName(tagName);
        if (("undefined" != typeof divArray) && (null != divArray)) {
            result = divArray;
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// returns an array of the menu-elements whose className 
// attribute matches the supplied name:
function __ekFlexMenu_getElementsByClassName(className) {
    var result = new Array;
    var divArray = this.getEkFlexMenuElementsByTagName("*");
    for (var idx = 0; idx < divArray.length; idx++) {
        if (("undefined" != divArray[idx].className)
			&& (ekFlexMenu.hasClassName(divArray[idx], className))) {
            result[result.length] = divArray[idx];
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// returns an array of the menu-elements whose className 
// attribute matches the supplied name:
function __ekFlexMenu_getElementsByClassNameAndTagName(className, tagName) {
    var result = new Array;
    var divArray = this.getEkFlexMenuElementsByTagName(tagName);
    for (var idx = 0; idx < divArray.length; idx++) {
        if (("undefined" != divArray[idx].className)
			&& (ekFlexMenu.hasClassName(divArray[idx], className))) {
            result[result.length] = divArray[idx];
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Unselects the identified menu; if there is a folder-button, 
// then the class is updated to an unselected state. Then 
// hides the associated submenu items:
function __ekFlexMenu_hoverButton(idString, hoverFlag) {
    var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(idString);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
        var btnObj = this.getFolderButtonObject(menuSubmenuId);
        if (ekFlexMenu.private_isValidSubmenuButton(btnObj)) {
            var wasHovering = (ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonHover)
				|| ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonSelectedHover));
            if (hoverFlag == wasHovering) {
                return;
            }
            var isSelected = (ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonSelected)
				|| ekFlexMenu.hasClassName(btnObj, ekFlexMenu_classNames.buttonSelectedHover));
            if (hoverFlag) {
                ekFlexMenu.removeClassName(btnObj, ((isSelected) ? ekFlexMenu_classNames.buttonSelected : ekFlexMenu_classNames.button));
                ekFlexMenu.addClassName(btnObj, ((isSelected) ? ekFlexMenu_classNames.buttonSelectedHover : ekFlexMenu_classNames.buttonHover));
            }
            else {
                ekFlexMenu.removeClassName(btnObj, ((isSelected) ? ekFlexMenu_classNames.buttonSelectedHover : ekFlexMenu_classNames.buttonHover));
                ekFlexMenu.addClassName(btnObj, ((isSelected) ? ekFlexMenu_classNames.buttonSelected : ekFlexMenu_classNames.button));
            }
        }
    }
}

///////////////////////////////////////////////////////////
// Annonymous Helper Function.
// Called by __ekFlexMenu_mouseIn to prepare for the
// delayed opening of identified submenu.
// Parameters: 
//	1 - the ID of the element that triggered the event.
function __ekFlexMenu_mouseInHelperCaller(id) {
    if (id) {
        var menuObj = ekFlexMenu.getMenuObj(id);
        if (menuObj) {
            menuObj.mouseInHelper();
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Called by external (non-object-instance) code, to prepare for 
// delayed opening of identified submenu.
// Parameters: 
//	1 - the event object.
//	2 - the element-object that triggered the event.
function __ekFlexMenu_mouseIn(e, el) {
    if (this.private_mouseEventTimer) {
        window.clearTimeout(this.private_mouseEventTimer);
        this.private_mouseEventTimer = null;
    }
    this.private_mouseEventEnteringElementId = el.id;
    this.private_mouseEventTimer = window.setTimeout(function() { __ekFlexMenu_mouseInHelperCaller(el.id) }, 50);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Shows/selects the appropriate submenu.
function __ekFlexMenu_mouseInHelper() {
    if (this.private_mouseEventEnteringElementId) {
        var menuSubmenuId = ekFlexMenu.parseMenuSubmenuIdString(this.private_mouseEventEnteringElementId);
        if (ekFlexMenu.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
            if (this.private_isMasterControl) {
                // Dont select bottom level menus for master-control 
                // via mouse-over; force user to click to select these:
                itemsObj = this.getSubmenuItemsObject(menuSubmenuId);
                if (!ekFlexMenu.isDefinedNotNull(itemsObj)) {
                    return;
                }
            }
            this.selectSubmenu(menuSubmenuId, true);
            $ektron("#" + this.getEkFlexMenuContainerElement().id + ":has(." + ekFlexMenu_classNames.buttonSelectedHover + ")").addClass(ekFlexMenu_classNames.activeMenu)
        }
    }
}

///////////////////////////////////////////////////////////
// Annonymous Helper Function.
// Called by __ekFlexMenu_mouseOut to prepare for the
// delayed opening of identified submenu.
// Parameters: 
//	1 - the ID of the element that triggered the event.
function __ekFlexMenu_mouseOutHelperCaller(id) {
    if (id) {
        var menuObj = ekFlexMenu.getMenuObj(id);
        if (menuObj) {
            menuObj.mouseOutHelper();
            if ("function" == typeof menuObj.restoreMenuState) {
                menuObj.restoreMenuState();
            }
        }
    }
}


///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Called by external (non-object-instance) code, to prepare for 
// delayed opening of identified submenu.
// Parameters: 
//	1 - the event object.
//	2 - the element-object that triggered the event.
function __ekFlexMenu_mouseOut(e, el) {
    if (this.private_mouseEventTimer) {
        window.clearTimeout(this.private_mouseEventTimer);
        this.private_mouseEventTimer = null;
    }
    var id = __ekFlexMenu_static_getValidParentId(el);
    this.private_mouseEventExitingElementId = id;
    this.private_mouseEventTimer = window.setTimeout(function() { __ekFlexMenu_mouseOutHelperCaller(id) }, 500);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Hides/unselects the appropriate submenu (possibly all but root).
// Parameters: 
//	None.
function __ekFlexMenu_mouseOutHelper() {
    //if (this.private_mouseEventEnteringElementId) {
    //	this.unSelectSubmenu(this.private_mouseEventEnteringElementId);
    //}
    if (this.private_autoCollapseSubmenus) {
        // don't leave all submenus hidden for slave-menus, otherwise nothing to click:
        if (this.private_isSlaveControl) {
            this.collapseUnselectedStartLevelSubmenus();
        }
        else {
            this.collapseAllOpenSubmenus();
        }
    }
    else if (this.private_mouseEventEnteringElementId) {
        this.unSelectSubmenu(this.private_mouseEventEnteringElementId);
    }

    // Partial Fix for defect 34459
    if ($ektron.browser.msie) {
        window.status = '';
    }

    $ektron("#" + this.getEkFlexMenuContainerElement().id + ":not(:has(." + ekFlexMenu_classNames.buttonSelectedHover + "))").removeClass(ekFlexMenu_classNames.activeMenu)
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the Menu-Submenu-Id string, for a given Submenu-Id:
function __ekFlexMenu_buildMenuSubmenuId(submenuId) {
    return (this.hashCode() + "_" + this.menuId() + "_" + submenuId);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the corresponding submenu-folder-button object, 
// for a given Submenu-Id (or Menu-Submenu-Id):
function __ekFlexMenu_getFolderButtonObject(submenuId) {
    var id = ekFlexMenu.parseMenuSubmenuIdString(submenuId);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(id)) {
        id = id + ekFlexMenu.private_buttonElementIdPostFix;
    }
    else {
        id = this.buildMenuSubmenuId(submenuId) + ekFlexMenu.private_buttonElementIdPostFix;
    }
    return (document.getElementById(id));
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the corresponding submenu-Items object, 
// for a given Submenu-Id (or Menu-Submenu-Id):
function __ekFlexMenu_getSubmenuItemsObject(submenuId) {
    var id = ekFlexMenu.parseMenuSubmenuIdString(submenuId);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(id)) {
        id = id + ekFlexMenu.private_submenuItemsElementIdPostFix;
    }
    else {
        id = this.buildMenuSubmenuId(submenuId) + ekFlexMenu.private_submenuItemsElementIdPostFix;
    }
    return (document.getElementById(id));
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the corresponding submenu object, 
// for a given Submenu-Id (or Menu-Submenu-Id):
function __ekFlexMenu_getSubmenuObject(submenuId) {
    var id = ekFlexMenu.parseMenuSubmenuIdString(submenuId);
    var result = null;
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(id)) {
        result = document.getElementById(id);
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the parent-submenu-id for the given Submenu-Id 
// (or the given Menu-Submenu-Id), returns zero if top (root):
function __ekFlexMenu_getParentLevelSubmenuId(submenuId) {
    var result = this.buildMenuSubmenuId("0"); // default to root.
    var id = ekFlexMenu.parseMenuSubmenuIdString(submenuId);
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(id)) {
        id = id + ekFlexMenu.private_parentIdElementIdPostFix;
    }
    else {
        id = this.buildMenuSubmenuId(submenuId) + ekFlexMenu.private_parentIdElementIdPostFix;
    }
    var hiddenObj = document.getElementById(id);
    if (hiddenObj
		&& ("undefined" != typeof hiddenObj.value)
		&& ("undefined" != typeof hiddenObj.value.length)
		&& (hiddenObj.value.length > 0)) {
        result = hiddenObj.value;
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the integer value of the menu level for
// the menu identified by the given submenu-id string.
function __ekFlexMenu_getMenuLevel(submenuId) {
    var result = 0;
    if (ekFlexMenu.private_isValidMenuSubmenuIdString(submenuId)) {
        var obj = this.getSubmenuObject(submenuId);
        if (obj && obj.className && obj.className.length && (obj.className.length > 0)) {
            var levelPrefix = "ekflexmenu_menu_level_";
            var idx = obj.className.indexOf(levelPrefix);
            if (idx >= 0) {
                var val = obj.className.substr(idx + levelPrefix.length)
                if (val.length) {
                    result = parseInt(val, 10);
                }
            }
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Returns the outermost container element (DIV) that
// holds this entire ekFlexMenu object:
function __ekFlexMenu_getEkFlexMenuContainerElement() {
    var containerId = this.hashCode() + "_"
		+ this.menuId()
		+ "_"
		+ "0"
		+ ekFlexMenu.private_ekflexmenuContainerElementIdPostFix;
    var containerObj = document.getElementById(containerId);
    if (containerObj
		&& ekFlexMenu.private_isValidEKMenu(containerObj)) {
        return (containerObj);
    }
    else {
        return (null);
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Called when a menu-item (such as a link) is clicked, 
// modifies the items href parameter to pass the selected
// item info over the querystring:
function __ekFlexMenu_ekFlexMenu_selectMenuItem(el) {
    var elm = el;
    var isWrapper = false;

    if (ekFlexMenu.isDefinedNotNull(elm)
		&& ekFlexMenu.isDefinedNotNull(elm.parentNode)
		&& ekFlexMenu.isDefinedNotNull(elm.parentNode.tagName)
		&& ("A" == elm.parentNode.tagName)) {
        // event is from a button, that's wrapped 
        // with an anchor. Use the anchor element:
        elm = elm.parentNode;
        isWrapper = true;
    }

    if (elm && ("undefined" != typeof elm.id) && ("undefined" != typeof elm.href)) {
        //// Update: to correct a problem with FireFox (where events like mouse-out
        //// could call the handlers between the current page unloading and the next
        //// page loading) we must ensure that the event handlers are not called
        //// while we're navigating/submitting the page:
        //// TODO: Test for IE, skip if true (only needed for non-IE browsers, particuarly FireFox).
        // Further Update:
        //   Now testing validity of calling event handlers from menu HTML, so this 
        //   time consuming call to "disableAllEventHandlers" is not needed:
        //this.disableAllEventHandlers();

        if (elm.href.indexOf("?") < 0) {
            elm.href += "?";
        }
        else {
            elm.href += "&";
        }

        var modId = elm.id;
        var matchVal = "ekfxmensel_";
        if (modId.length > matchVal.length) {
            var idx = modId.indexOf(matchVal);
            if (idx >= 0) {
                modId = modId.substr(idx + matchVal.length);
            }
        }
        elm.href += matchVal.substr(0, matchVal.length - 1) + "=" + modId;

        if ((this.private_lastSelectedMenuItemObj != null) && (this.private_lastSelectedMenuItemObj != elm)) {
            ekFlexMenu.removeClassName(this.private_lastSelectedMenuItemObj, ekFlexMenu_classNames.linkSelected);
            ekFlexMenu.addClassName(this.private_lastSelectedMenuItemObj, ekFlexMenu_classNames.link);
        }
        this.private_lastSelectedMenuItemObj = elm;

        if (!isWrapper) {
            ekFlexMenu.removeClassName(elm.className, ekFlexMenu_classNames.link);
            ekFlexMenu.addClassName(elm.className, ekFlexMenu_classNames.linkSelected);
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Disables all event handlers for elements of this menu object:
function __ekFlexMenu_disableAllEventHandlers() {
    var elArray = this.getEkFlexMenuElementsByTagName("*");
    for (var idx = 0; idx < elArray.length; idx++) {
        this.disableElementEventHandlers(elArray[idx]);
    }
    var el = this.getEkFlexMenuContainerElement();
    if (el) {
        this.disableElementEventHandlers(el);
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Disables all event handlers for elements of this menu object:
function __ekFlexMenu_disableElementEventHandlers(el) {
    if (el) {
        if (ekFlexMenu.isDefinedNotNull(el.onmouseout))
            el.onmouseout = null;

        if (ekFlexMenu.isDefinedNotNull(el.onmouseover))
            el.onmouseover = null;

        if (ekFlexMenu.isDefinedNotNull(el.onfocus))
            el.onfocus = null;

        if (ekFlexMenu.isDefinedNotNull(el.onblur))
            el.onblur = null;

        if (ekFlexMenu.isDefinedNotNull(el.onclick))
            el.onclick = null;

        if (ekFlexMenu.isDefinedNotNull(el.ondblclick))
            el.ondblclick = null;

        if (ekFlexMenu.isDefinedNotNull(el.onkeydown))
            el.onkeydown = null;

        if (ekFlexMenu.isDefinedNotNull(el.onkeypress))
            el.onkeypress = null;

        if (ekFlexMenu.isDefinedNotNull(el.onkeyup))
            el.onkeyup = null;
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// This function modifies all these to eliminate page 
// refreshes (not needed if Javascript is enabled).
// Parameters: 
//	None.
function __ekFlexMenu_updateNoScriptLinks() {
    return;
    // UPDATE:
    //	This function no longer runs, instead the "noscript" links 
    //  are removed when the user clicks a menu button. This was
    // done becuase IE had a problem with background image flicker
    // whenever a page was loaded (images were refreshed after
    // the page was rendered - causing a moment with the background
    // being displayed on some elements...
    //
    //	var links = this.getEkFlexMenuElementsByTagName("A");
    //	for (var idx=0; idx < links.length; idx++) {
    //		if (("undefined" != typeof links[idx].href) && (0 <= links[idx].href.indexOf("ekfxmen_noscript=1"))) {
    //			links[idx].href = "#NoScroll";
    //		}
    //	}
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Called by page-load initialization code, to determine if MouseOverPopUp 
// is enabled, and perform related initialization if it is:
function __ekFlexMenu_initializeMouseOverPopUp() {
    var containerId = "#" + this.buildMenuSubmenuId(0) + "_ekflexmenu";
    this.enableMouseOverPopUp = (null != $ektron(containerId).attr("onmouseover"));
    if (this.enableMouseOverPopUp && ekFlexMenu_IsValid(ekFlexMenu)) {
        if ($ektron.browser.msie) {
            // Fix for defect 34459 (problem: IE doesn't show href in window status when mouseover-popup is enalbed)
            $ektron(containerId + " a.ekflexmenu_link").mouseover(function() { if (ekFlexMenu_IsValid(ekFlexMenu.clearSiblingButtons)) { if (this.href) window.status = this.href; return (ekFlexMenu.clearSiblingButtons(this)); } return (true); });

            $ektron(containerId + " a.ekflexmenu_button, "
                + containerId + " a.ekflexmenu_button_hover, "
                + containerId + " a.ekflexmenu_button_selected, "
                + containerId + " a.ekflexmenu_button_selected_hover").mouseover(function() { if ((this.href)) window.status = this.href; });
        } else {
            $ektron(containerId + " a.ekflexmenu_link").mouseover(function() { if (ekFlexMenu_IsValid(ekFlexMenu.clearSiblingButtons)) { return (ekFlexMenu.clearSiblingButtons(this)); } return (true); });
        }

        if (this.private_enableRestoreMenuState) {
            // mark all selected nodes, so we can restore original selections on mouse out:
            $ektron(containerId + " .ekflexmenu_submenu_items").attr("data-ektron-flexmenu-preselected-submenu", "true");
            $ektron(containerId + " .ekflexmenu_button_selected").attr("data-ektron-flexmenu-preselected-button", "true");
            $ektron(containerId + " .ekflexmenu_link_selected").attr("data-ektron-flexmenu-preselected-link", "true");
            // add instance method to restore original state:
            this.restoreMenuState = function() {
                $ektron("[data-ektron-flexmenu-preselected-submenu=true]").removeClass("ekflexmenu_submenu_items_hidden").addClass("ekflexmenu_submenu_items");
                $ektron("[data-ektron-flexmenu-preselected-button=true]").removeClass("ekflexmenu_button").addClass("ekflexmenu_button_selected");
                $ektron("[data-ektron-flexmenu-preselected-link=true]").removeClass("ekflexmenu_link").addClass("ekflexmenu_link_selected");
            }
        }
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Note this is only an issue when EnableMouseOverPopUp="true" and the 
// anchor tag has no text, only an image.
// (Corrects defect 60697) When mousing over anchor tags that have no text,
// only an image, the browser can miss the mouse-over of the containing 
// anchor tag as the image takes up most of it's space, and because the 
// image markup is created in the menu back-end it does not include
// the same mouse-over event handler information as the containing anchor
// tag - resulting in the sub menu not being selected.
function __ekFlexMenu_initializeMouseOverPopUpForAnchorsWithImageOnly() {
    var containerId = "#" + this.buildMenuSubmenuId(0) + "_ekflexmenu";
    this.enableMouseOverPopUp = (null != $ektron(containerId).attr("onmouseover"));
    if (this.enableMouseOverPopUp && ekFlexMenu_IsValid(ekFlexMenu)) {
        $ektron(containerId + " .ekflexmenu_button IMG").bind("mouseover", function (event) {
            var currentImage = $ektron(event.target);
            var parentAnchor = currentImage.closest("a");

            if (parentAnchor) {
                parentAnchor.mouseover();

                if (!parentAnchor.hasClass(ekFlexMenu_classNames.buttonSelected)
                    && !parentAnchor.hasClass(ekFlexMenu_classNames.buttonSelectedHover)) {
                    parentAnchor.trigger("click");
                }
            }
        });

        $ektron(containerId + " .ekflexmenu_button IMG").bind("mouseout", function (event) {
            var currentImage = $ektron(event.target);
            var parentAnchor = currentImage.closest("a");

            if (parentAnchor) {
                parentAnchor.mouseout();
            }
        });
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
// Called by page-load initialization code, to initialize this object
// with values passed from the server.
// Parameters: 
//	None.
function __ekFlexMenu_initializeWithServerVariables() {
    var baseId = this.hashCode();
    if (baseId && baseId.length) {
        // Obtain the server control property, autoCollapseBranches:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_autoCollapseBranches))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_autoCollapseBranches[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_autoCollapseBranches[baseId].length))
			&& (0 < window.ekFlexMenu_autoCollapseBranches[baseId].length)) {

            this.private_autoCollapseSubmenus = ("true" == window.ekFlexMenu_autoCollapseBranches[baseId]);
        }

        // Obtain the server control property, swRev:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_swRev))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_swRev[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_swRev[baseId].length))
			&& (0 < window.ekFlexMenu_swRev[baseId].length)) {

            this.private_swRevision = window.ekFlexMenu_swRev[baseId];
        }

        // Obtain the server control property, startCollapsed:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startCollapsed))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startCollapsed[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startCollapsed[baseId].length))
			&& (0 < window.ekFlexMenu_startCollapsed[baseId].length)) {

            this.private_startCollapsed = ("true" == window.ekFlexMenu_startCollapsed[baseId]);
        }

        // Obtain the server control property, startWithRootFolderCollapsed:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startWithRootFolderCollapsed))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startWithRootFolderCollapsed[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startWithRootFolderCollapsed[baseId].length))
			&& (0 < window.ekFlexMenu_startWithRootFolderCollapsed[baseId].length)) {

            this.private_startWithRootFolderCollapsed = ("true" == window.ekFlexMenu_startWithRootFolderCollapsed[baseId]);
        }

        // Obtain the hash-code of the server control property, MasterControlId:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_masterControlIdHash))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_masterControlIdHash[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_masterControlIdHash[baseId].length))
			&& (0 < window.ekFlexMenu_masterControlIdHash[baseId].length)) {

            this.private_masterControlIdHash = window.ekFlexMenu_masterControlIdHash[baseId];
        }

        // Obtain the server control property, ajaxEnabled:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_ajaxEnabled))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_ajaxEnabled[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_ajaxEnabled[baseId].length))
			&& (0 < window.ekFlexMenu_ajaxEnabled[baseId].length)) {

            this.private_ajaxEnabled = ("true" == window.ekFlexMenu_ajaxEnabled[baseId]);
        }

        // Obtain the Ajax-WebService base path:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_ajaxWSPath))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_ajaxWSPath[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_ajaxWSPath[baseId].length))
			&& (0 < window.ekFlexMenu_ajaxWSPath[baseId].length)) {
            //			
            this.ekFlexMenu_ajaxWSPath = window.ekFlexMenu_ajaxWSPath[baseId];
            if (this.ekFlexMenu_ajaxWSPath.length && this.ekFlexMenu_ajaxWSPath.lastIndexOf("/") != (this.ekFlexMenu_ajaxWSPath.length - 1)) {
                this.ekFlexMenu_ajaxWSPath += "/";
            }
        }

        // Set displayXslt:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_displayXslt))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_displayXslt[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_displayXslt[baseId].length))
			&& (0 < window.ekFlexMenu_displayXslt[baseId].length)) {
            //			
            this.ekFlexMenu_displayXslt = window.ekFlexMenu_displayXslt[baseId];
        }

        // Set cacheInterval:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_cacheInterval))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_cacheInterval[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_cacheInterval[baseId].length))
			&& (0 < window.ekFlexMenu_cacheInterval[baseId].length)) {
            //			
            this.ekFlexMenu_cacheInterval = window.ekFlexMenu_cacheInterval[baseId];
        }

        // Obtain the slave/subscriber list:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_subscriberList))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_subscriberList[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_subscriberList[baseId].length))
			&& (0 < window.ekFlexMenu_subscriberList[baseId].length)) {

            this.private_subscriberList = window.ekFlexMenu_subscriberList[baseId];

            if (this.private_subscriberList.length > 0) {
                var subList = this.private_subscriberList.split(",");
                if (subList && subList[0]) {
                    this.private_slaveControl = subList[0];
                    this.private_isMasterControl = true;
                }
            }
        }

        // Default slave-menu:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_defaultMenuIdString))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_defaultMenuIdString[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_defaultMenuIdString[baseId].length))
			&& (0 < window.ekFlexMenu_defaultMenuIdString[baseId].length)) {
            //			
            this.ekFlexMenu_defaultMenuIdString = window.ekFlexMenu_defaultMenuIdString[baseId];
        }

        // Slave menu start-level ids:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_slaveStartLevelIds))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_slaveStartLevelIds[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_slaveStartLevelIds[baseId].length))
			&& (0 < window.ekFlexMenu_slaveStartLevelIds[baseId].length)) {
            //			
            var startLevelIds = window.ekFlexMenu_slaveStartLevelIds[baseId].split(",");
            if (null == this.ekFlexMenu_slaveStartLevelIds) {
                this.ekFlexMenu_slaveStartLevelIds = new Array;
            }
            for (var idx = 0; idx < startLevelIds.length; idx++) {
                if (!this.ekFlexMenu_slaveStartLevelIds[startLevelIds[idx]]) {
                    this.ekFlexMenu_slaveStartLevelIds[startLevelIds[idx]] = true;
                }
            }
        }

        // The server may have passed a submenu id, indicating which one to open initially:
        if ((ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startupSubmenuBranchId))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startupSubmenuBranchId[baseId]))
			&& (ekFlexMenu.isDefinedNotNull(window.ekFlexMenu_startupSubmenuBranchId[baseId].length))
			&& (0 < window.ekFlexMenu_startupSubmenuBranchId[baseId].length)) {

            var id = ekFlexMenu.parseMenuSubmenuIdString(window.ekFlexMenu_startupSubmenuBranchId[baseId]);
            if (ekFlexMenu.private_isValidMenuSubmenuIdString(id)) {
                this.selectSubmenu(id);
            }
        }

        if (this.private_masterControlIdHash.length) {
            this.private_isSlaveControl = true;
            this.initializeSlaveMenu();
        }

        if (this.private_isMasterControl) {
            this.initializeMasterMenu();
        }

        this.updateNoScriptLinks();
        this.initializeMouseOverPopUp();
        this.initializeMouseOverPopUpForAnchorsWithImageOnly();
    }
}

///////////////////////////////////////////////////////////
// Makes the contents of the root-menu visible, selects it's button if it exists.
// Parameters: 
//	None.
function __ekFlexMenu_showRootMenu() {
    var rootMenuId = this.buildMenuSubmenuId(0);
    this.selectSubmenu(rootMenuId);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_loadXMLDoc(url) {
    var result = false;

    ekFlexMenu.ajaxCancelServerCall();
    if (window.XMLHttpRequest) {
        ekFlexMenu.static_userAjaxXmlHttp1 = new XMLHttpRequest()
    }
    else if (window.ActiveXObject) {
        ekFlexMenu.static_userAjaxXmlHttp1 = new ActiveXObject("Msxml2.XMLHTTP");
        if (null == ekFlexMenu.static_userAjaxXmlHttp1) {
            ekFlexMenu.static_userAjaxXmlHttp1 = new ActiveXObject("Microsoft.XMLHTTP");
        }
    }

    if (ekFlexMenu.static_userAjaxXmlHttp1 != null) {
        ekFlexMenu.static_userAjaxXmlHttp1.onreadystatechange = ekFlexMenu.ajaxCallBack_stateChange;
        ekFlexMenu.static_userAjaxXmlHttp1.open("POST", url, true);
        ekFlexMenu.static_userAjaxXmlHttp1.send('');
        result = true;
    }

    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_DecodeHTML(str) {
    var result = "";
    if (str) {
        result = new String(str);
        result = result.replace(/\&amp;/gi, "&");
        result = result.replace(/\&lt;/gi, "<");
        result = result.replace(/\&gt;/gi, ">");
        result = result.replace(/\&quot;/gi, "\"");
        result = result.replace(/\&#39;/gi, "'");
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_getPayload(response) {
    var result = null;
    if (response) {
        if (response.xml) {
            result = response.xml;
        }
        else if (response.childNodes && ("undefined" != typeof XMLSerializer)) {
            if (XMLSerializer) {
                result = new XMLSerializer().serializeToString(response);
            }
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_appendText(targId) {
    var targ = document.getElementById(targId);
    if (targ && ("undefined" != typeof targ.innerHTML)) {
        targ.innerHTML = this.DecodeHTML(this.getPayload(ekFlexMenu.static_userAjaxXmlHttp1.responseXML));
    }
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_appendXml(targId) {
    if (!ekFlexMenu.isDefinedNotNull(ekFlexMenu.static_userAjaxXmlHttp1)
		|| !ekFlexMenu.isDefinedNotNull(ekFlexMenu.static_userAjaxXmlHttp1.responseXML)) {
        return;
    }

    var targ = document.getElementById(targId);
    if (targ && ("undefined" != typeof targ.innerHTML)) {
        if (ekFlexMenu.isDefinedNotNull(ekFlexMenu.static_userAjaxXmlHttp1.responseXML.text)) {
            //targ.innerHTML = ekFlexMenu.static_userAjaxXmlHttp1.responseXML.text;
            var menuFrag = this.removeMenuFragmentContainer(ekFlexMenu.static_userAjaxXmlHttp1.responseXML.text);
            if (menuFrag.length > 0) {
                targ.innerHTML = targ.innerHTML + menuFrag;
            }
        }
        else {
            // (FireFox, etc.)
            targ.innerHTML = "";
            if (ekFlexMenu.isDefinedNotNull(ekFlexMenu.static_userAjaxXmlHttp1.responseXML.firstChild)
				&& ekFlexMenu.isDefinedNotNull(ekFlexMenu.static_userAjaxXmlHttp1.responseXML.firstChild.textContent)) {
                targ.innerHTML = ekFlexMenu.static_userAjaxXmlHttp1.responseXML.firstChild.textContent;
            } else if ("undefined" != typeof XMLSerializer) {
                result = new XMLSerializer().serializeToString(ekFlexMenu.static_userAjaxXmlHttp1.responseXML);
                targ.innerHTML = this.DecodeHTML(result);
            }
            else if ("undefined" != typeof document.importNode) {
                var src = document.importNode(ekFlexMenu.static_userAjaxXmlHttp1.responseXML.documentElement, true);
                var tempEl = document.createElement("span");
                tempEl.appendChild(src);
                targ.innerHTML = this.DecodeHTML(tempEl.innerHTML);
                tempEl = null;
            }
        }
    }
    this.userAjaxParentId1 = "";
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_removeMenuFragmentContainer(menuStr) {
    var result = "";
    var idx = menuStr.indexOf("<ul");
    if (idx >= 0) {
        result = menuStr.substr(idx);
        idx = result.lastIndexOf("</li>");
        if (idx > 0) {
            result = result.substr(0, idx - 1);
        }
    }
    return (result);
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_callService(ctrlHash, menuId, submenuId, parentId, menuDepth) {
    this.userAjaxParentId1 = "";
    if (parentId.length > 0) {
        this.userAjaxParentId1 = parentId;
        return (this.loadXMLDoc(this.ekFlexMenu_ajaxWSPath + "WebServiceAPI/Navigation/FlexMenu.asmx/GetMenuFragment?ctrl_hash=" + ctrlHash + "&menu_depth=" + menuDepth + "&menu_id=" + menuId + "&submenu_id=" + submenuId + "&menu_xslt=" + this.ekFlexMenu_displayXslt + "&cache_interval=" + this.ekFlexMenu_cacheInterval));
    }
    return false
}

///////////////////////////////////////////////////////////
// ekFlexMenu Instance Member Helper Function.
function __ekFlexMenu_ajax_callAjaxForUserClick(btnId) {
    var result = false;

    // check if container's children already loaded:
    var containerId = ekFlexMenu.parseMenuSubmenuIdString(btnId);
    var si = this.getSubmenuItemsObject(containerId);
    if (si) {
        result = true;
    }
    else {
        // false, initiate ajax-call:
        var ctrlHash = this.hashCode();
        var menuId = ekFlexMenu.private_getMenuIdString(btnId);
        var submenuId = ekFlexMenu.private_getSubmenuIdString(btnId);
        var parentId = this.getFolderButtonObject(btnId).parentNode.id;
        //var menuDepth = 1 + this.getMenuLevel(btnId);
        result = this.callService(ctrlHash, menuId, submenuId, parentId, 1);
    }

    return (result);
}

///////////////////////////////////////////////////////////
ekFlexMenu_loadEventConfigured = false; // global variable for ekFlexMenu_addLoadEvent(), to indicate if code has initialized.
///////////////////////////////////////////////////////////
// This funtion is caled by the in-line-code following
// this functions' definition, to ensure that the 
// windows' on-load event is hooked with the ekFlexMenu
// initialization code.
function ekFlexMenu_addLoadEvent() {
    if (ekFlexMenu_loadEventConfigured)
        return;

    ekFlexMenu_loadEventConfigured = true;
    var oldOnload = window.onload;
    window.onload = function() {
        if ("function" == typeof oldOnload)
            oldOnload();

        if ("undefined" == typeof $ektron) {
            setTimeout(window.onload, 100);
        } else {
            ekFlexMenu.private_startupAllSmartMenus();
        }
    }
}
ekFlexMenu_addLoadEvent(); // Call the preceeding function to hook the ekFlexMenu initialization code.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
ekFlexMenu_unloadEventConfigured = false; // global variable for ekFlexMenu_addUnLoadEvent(), to indicate if code has initialized.
///////////////////////////////////////////////////////////
// This funtion is caled by the in-line-code following
// this functions' definition, to ensure that the 
// windows' on-unload event is hooked with the ekFlexMenu
// cleanup code. 
function ekFlexMenu_addUnLoadEvent() {
    if (ekFlexMenu_unloadEventConfigured)
        return;

    ekFlexMenu_unloadEventConfigured = true;
    var oldOnunload = window.onunload;
    window.onunload = function() {
        if ("function" == typeof oldOnunload)
            oldOnunload();

        //setTimeout(ekFlexMenu.private_startupAllSmartMenus, 100);
        ekFlexMenu.private_shutdownAllSmartMenus();
    }
}
ekFlexMenu_addUnLoadEvent(); // Call the preceeding function to hook the ekFlexMenu initialization code.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//	Menu element names (prefixed by "ekmengrp_"):
//  Note: these are only rendered if the SmartMenu server controls' renderElementNames  is true (default=false, see Navigation.vb).
//
//		accessible_menu_startheading - H2: Wraps the skip-menu with a navigation-heading (only rendered when 508-Compliance is enabled).
//		accessible_menu_startlink - A: Wraps the skip-menu text with a navigation-link (only rendered when 508-Compliance is enabled).
//		btnlink - A: Wraps each menu button title with a navigation-anchor (only rendered when 508-Compliance is enabled).
//		button - SPAN: Holds the title, and acts as a button (or folder) for the associated submenu.
//		ekflexmenu - DIV: Wraps the entire menu (the outer-most non-user container element).
//		link - A: A Link for individual menu items (quicklinks, external links, etc.).
//		menu_end - DIV: Wraps the menu-end page-anchor (only rendered when 508-Compliance is enabled).
//		menu_start - DIV: Wraps the menu-start link (only rendered when 508-Compliance is enabled).
//		submenu - DIV: Holds submenu items, such as a submenu title and links.
//		submenu_items - DIV: Container for menu lists.
//		submenu_navheading - H3: Wraps each menu button title with a navigation-heading (only rendered when 508-Compliance is enabled).
//		unorderedlist - UL: A container for menu list items (useful for non-graphical browsers).
//		unorderedlist_item - LI: Container for menu items (typically either links or sub-menus).

///////////////////////////////////////////////////////////////////////////////

function EkTbWebMenuPopUpWindow(url, hWind, nWidth, nHeight, nScroll, nResize) {
    url = url.replace(/&amp;amp;/g, "&").replace(/&amp;/g, "&");
    if (nWidth > screen.width) {
        nWidth = screen.width;
    }
    if (nHeight > screen.height) {
        nHeight = screen.height;
    }
    if (url.toLowerCase().indexOf('workarea.aspx') !== -1) {
        cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + screen.availWidth + ',height=' + screen.availHeight;
        popupwin = window.open(url, hWind, cToolBar);
        popupwin.moveTo(0, 0);
        popupwin.resizeTo(screen.availWidth, screen.availHeight);
    }
    else {
        cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
        popupwin = window.open(url, hWind, cToolBar);
    }
    return popupwin;
}

///////////////////////////////////
// 
function ekFlexMenu_LogMsg(msg) {
    var dt = new Date();
    msg = Date() + " - " + msg;
    if (window.console && window.console.log) {
        window.console.log(msg);
    }
    else if (window.Debug && window.Debug.writeln) {
        window.Debug.writeln(msg);
    }
    else {
        ekFlexMenu_DebugMsg(msg);
    }
}

var g_ekFlexMenu_DebugWindow = null;
function ekFlexMenu_DebugMsg(Msg) {
    Msg = '>>>' + Msg + ' <br> ';
    if ((g_ekFlexMenu_DebugWindow == null) || (g_ekFlexMenu_DebugWindow.closed)) {
        g_ekFlexMenu_DebugWindow = window.open('Debug Notes', 'myWin', 'toolbar=no, directories=no, location=no, status=yes, menubar=no, resizable=yes, scrollbars=yes, width=500, height=300');
    }
    g_ekFlexMenu_DebugWindow.document.writeln(Msg);
    g_ekFlexMenu_DebugWindow.scrollTo(0, 10000000);
}

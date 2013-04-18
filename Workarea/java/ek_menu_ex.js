///////////////////////////////////////////////////////////
// ekMenu Javascript file (ek_menu_ex.js):
// Client side support for the Ektron SmartMenu server control.


///////////////////////////////////////////////////////////
// CSS Style Class Name Enumertion:
function ekMenuEx_classNames() {}
ekMenuEx_classNames.button = "ekmenu_button";
ekMenuEx_classNames.buttonHover = "ekmenu_button_hover";
ekMenuEx_classNames.buttonSelected = "ekmenu_button_selected";
ekMenuEx_classNames.buttonSelectedHover = "ekmenu_button_selected_hover";
ekMenuEx_classNames.submenuItems = "ekmenu_submenu_items";
ekMenuEx_classNames.submenuItemsHidden = "ekmenu_submenu_items_hidden";
ekMenuEx_classNames.submenu = "ekmenu_submenu";
ekMenuEx_classNames.submenuHover = "ekmenu_submenu_hover";
ekMenuEx_classNames.submenuParent = "ekmenu_submenu_parent";
ekMenuEx_classNames.submenuParentHover = "ekmenu_submenu_parent_hover";
ekMenuEx_classNames.btnLink = "ekmenu_accessible_submenu_btnlink";
ekMenuEx_classNames.link = "ekmenu_link";
ekMenuEx_classNames.linkSelected = "ekmenu_link_selected";
ekMenuEx_classNames.slaveBranchSelected = "ekmenu_slave_branch_sel";

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
// Class ekMenuEx:
var ekMenuEx = function (menuObjectIdString) {

	/////////////////////////
	// public members:

		this.menuId = __ekMenuEx_returnMenuId;
			// Returns the root menu id for this object.
			// Parameters: 
			//	None.
			
		this.hashCode = __ekMenuEx_returnHashCode;
			// Returns the hash-code of the server control.
			// Parameters: 
			//	None.

		this.isSubmenuSelected = __ekMenuEx_isSubmenuSelected;
			// Returns selected-status (and thereby the visibility of the 
			// associated submenu contents) of the identified submenu.
			// Parameters: 
			//	1 - The standard menu-submenu-id string (extra characters discarded).
			
		this.selectSubmenu = __ekMenuEx_selectSubmenu;
			// Selects the designated submenu, setting the menu-button to a 'selected' 
			// state, and making any associated submenu content items visible.
			// Parameters: 
			//	1 - The standard menu-submenu-id string (extra characters discarded).

		this.unSelectSubmenu = __ekMenuEx_unSelectSubmenu;
			// De-Selects the designated submenu, setting the menu-button to a non-selected
			// state, and making any associated submenu content items invisible.
			// Parameters: 
			//	1 - The standard menu-submenu-id string (extra characters discarded).
		
		this.hoverButton = __ekMenuEx_hoverButton;
			// Sets the designated submenu-button to a hovered or non-hovered state.
			// Parameters: 
			//	1 - The standard menu-submenu-id string (extra characters discarded).
			//	2 - Hover flag (boolean; true to set state to hovered).

		this.selectMenuItem = __ekMenuEx_ekMenu_selectMenuItem;
			// Called when a menu-item (such as a link) is clicked, before
			// the page is submitted to the server.
			// Parameters: 
			//	1 - The element-object that is being selected.
		
		this.initializeWithServerVariables = __ekMenuEx_initializeWithServerVariables;
			// Called by page-load initialization code, to initialize this object
			// with values passed from the server.
			// Parameters: 
			//	None.
		
		this.showRootMenu = __ekMenuEx_showRootMenu;
			// Makes the contents of the root-menu visible, selects it's button if it exists.
			// Parameters: 
			//	None.


	/////////////////////////
	// private member functions:
	
		this.buildMenuSubmenuId = __ekMenuEx_buildMenuSubmenuId;
			// Returns the standard menu-submenu-id string.
			// Parameters: 
			//	1 - The targetted submenu-id number (or string containing only numbers).
			
		this.getFolderButtonObject = __ekMenuEx_getFolderButtonObject;
			// Returns the folder-button-object for the specified submenu.
			// Parameters: 
			//	1 - The targetted submenu-id (standard menu-submenu-id string).
			
		this.getSubmenuItemsObject = __ekMenuEx_getSubmenuItemsObject;
			// Returns the folder-item-object for the specified submenu.
			// This may contain menu items such as links and nested submenus.
			// Parameters: 
			//	1 - The targetted submenu-id (standard menu-submenu-id string).

		this.getSubmenuObject = __ekMenuEx_getSubmenuObject;
			// Returns the corresponding submenu object, 
			// for a given Submenu-Id (or Menu-Submenu-Id):
			// Parameters: 
			//	1 - The targetted submenu-id (standard menu-submenu-id string).
			
		this.getParentLevelSubmenuId = __ekMenuEx_getParentLevelSubmenuId;
			// Returns the parent-levels menu-submenu-id for the given Submenu,
			// returns zero if the parent (or thismenu) is the root menu.
			// Parameters: 
			//	1 - The targetted submenu-id (standard menu-submenu-id string).
			
		this.getEkMenuContainerElement = __ekMenuEx_getEkMenuContainerElement;
			// Returns the outermost container element (DIV) that 
			// holds this entire ekMenu object.
			// Parameters: 
			//	None.
		
		this.selectSubmenuHelper = __ekMenuEx_selectSubmenuHelper;
			// Helper funtion for __ekMenuEx_selectSubmenu, uses 
			// recursionSelects to ensure selected submenus are visible
			// even if they are buried with muliple nesting levels.
			// Parameters: 
			//	1 - The targetted submenu-id (standard menu-submenu-id string).
		
		this.collapseSiblingSubmenus = __ekMenuEx_collapseSiblingSubmenus;
			// Hide sibling submenus of the designated submenu.
			// Parameters: 
			//	1 - The targetted submenu-id (standard menu-submenu-id string).
		
		this.collapseAllOpenSubmenus = __ekMenuEx_collapseAllOpenSubmenus;
			// Closes all currently open submenus, to prevent overlap & visual clutter.
			// Parameters: 
			//	1 - Show root flag (boolean; true to make the root menu contents visible).

		this.markParentSubmenu = __ekMenuEx_markParentSubmenu;
			// Sets the parent folders' style to be a parent (optionally 
			// used in CSS to style parents & children differently):
			// Parameters: 
			//	1 - The submenu-id whose parent to mark (standard menu-submenu-id string).

		this.unMarkParentSubmenu = __ekMenuEx_unMarkParentSubmenu;
			// Sets the parent folders' style to be a normal non-parent (optionally 
			// used in CSS to style parents & children differently):
			// Parameters: 
			//	1 - The submenu-id whose parent to unmark (standard menu-submenu-id string).
			
		this.hoverSubmenu = __ekMenuEx_hoverSubmenu;
			// Sets the menus' container style to be hovered,
			// (optionally used in CSS to style contents & children differently):
			// Parameters: 
			//	1 - The submenu-id whose parent to hover (standard menu-submenu-id string).
			
		this.unHoverSubmenu = __ekMenuEx_unHoverSubmenu;
			// Sets the menus' container style to be unhovered,
			// (optionally used in CSS to style contents & children differently):
			// Parameters: 
			//	1 - The submenu-id whose parent to unhover (standard menu-submenu-id string).
			
			
		this.getEkMenuElementsByTagName = __ekMenuEx_getEkMenuElementsByTagName;
			// Returns an array of the ekmenu-elements with the specified tag-name.
			// Parameters: 
			//	1 - the element tag-name to search for.
			
		this.getElementsByClassName = __ekMenuEx_getElementsByClassName;
			// Returns an array of the menu-elements, whose className 
			// attributes match the supplied name.
			// Parameters: 
			//	1 - the className to search for.
			
		this.getElementsByClassNameAndTagName = __ekMenuEx_getElementsByClassNameAndTagName;
			// Returns an array of the menu-elements, whose className 
			// attributes match the supplied name.
			// Parameters: 
			//	1 - the className to search for.
			//  2 - the tag-name of the elements to include in the search.
			
		this.getEkMenuElementsByName =  __ekMenuEx_getEkMenuElementsByName;
			// Returns an array of the menu-elements, whose name attribute
			// match the supplied name.
			// Parameters: 
			//	1 - the name to search for.

		this.getDirectChildIds = __ekMenuEx_getDirectChildIds;
			// Returns an array of all direct child-submenu-ids (length = 0 if none).
			// Parameters: 
			//	1 - The targetted submenu-id (standard menu-submenu-id string).
			
		this.mouseIn = __ekMenuEx_mouseIn;
			// Called by external (non-object-instance) code, to prepare for 
			// delayed opening of identified submenu.
			// Parameters: 
			//	1 - the event object.
			//	2 - the element-object that triggered the event.
		
		this.mouseInHelper = __ekMenuEx_mouseInHelper;
			// Shows/selects the appropriate submenu.
			// Parameters: 
			//	None.

		this.mouseOut = __ekMenuEx_mouseOut;
			// Called by external (non-object-instance) code, to prepare for 
			// delayed opening of identified submenu.
			// Parameters: 
			//	1 - the event object.
			//	2 - the element-object that triggered the event.

		this.mouseOutHelper = __ekMenuEx_mouseOutHelper;
			// Hides/unselects the appropriate submenu (possibly all but root).
			// Parameters: 
			//	None.

		this.disableAllEventHandlers =  __ekMenuEx_disableAllEventHandlers;
			// Disables all event handlers for elements of this menu object:
			// Parameters: 
			//	None.

		this.disableElementEventHandlers = __ekMenuEx_disableElementEventHandlers;
			// Disables all event handlers for the given element:
			// Parameters: 
			//	1 - the element to disable events on.

		
		///////////////////////////////////////////////////////
		// Master/Slave related functions:
		this.getSlaveControlObject = __ekMenuEx_getSlaveControlObject;
		this.convertIdToSlaveControlId = __ekMenuEx_convertIdToSlaveControlId;
		this.callSlave__showSubmenuBranch = __ekMenuEx_callSlave__showSubmenuBranch;
		this.showSubmenuBranch = __ekMenuEx_showSubmenuBranch;
		this.unSelectSubmenuList = __ekMenuEx_unSelectSubmenuList;
		this.initializeSlaveMenu = __ekMenuEx_initializeSlaveMenu;
		this.initializeMasterMenu = __ekMenuEx_initializeMasterMenu;
		this.isTopLevelUI = __ekMenuEx_isTopLevelUI;
		// Master/Slave related variables:
		this.topLevelUI = null;


	/////////////////////////
	// private variables:
	
		this.private_menuIdString = __ekMenuEx_parseMenuId(menuObjectIdString);
			// holds the root menu id.

		this.private_serverControlHash = __ekMenuEx_static_parseServerControlHash(menuObjectIdString);
			// holds the server controls' hash-code.

		this.private_autoCollapseSubmenus = true;
			// Controls action on select-submenu; will 
			// collapse all other submenus - if this is true.

		this.private_startWithRootFolderCollapsed = false;
			// If set, will hide the root menu contents when all submenus 
			// are collapsed, otherwise will always leave root contents visible.

		this.private_startCollapsed = true;
			// If set, menu is initially rendered with all submenus closed.
			
		this.private_masterControlIdHash = "";
			// If this is a slave control, then this variable holds the 
			// hash-code of the master sercer controls id.
			
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
			// The software revision of the server control (default to 6.0).
			
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
}
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Initialize Public Static Members:

	ekMenuEx.getMenuObj = __ekMenuEx_static_getMenuObj;
		// Returns the Menu-ObjectID for a given ekmenu element ID,
		// creates a new ekMenuEx object if needed (stores these in
		// an array as a property to the indow object - making it 
		// available/shared with all SmartMenus (this was multple menus 
		// can exist on a page, and have different objects that 
		// are avaiable anywhere - given given an ekmenu element ID).
		// Parameters: 
		//	1 - The standard menu-submenu-id string (extra characters discarded).


	ekMenuEx.parseMenuSubmenuIdString = __ekMenuEx_static_parseMenuSubmenuIdString;
		// Returns the MenuSubmenuID string, for a given ekmenu element-ID
		// (ex. given "ekmensel_1_submenu_2_button" returns
		// "ekmensel_1_submenu_2" for root-menu 1, submenu 2):
		// Parameters: 
		//	1 - The standard menu-submenu-id string (extra characters discarded).

	ekMenuEx.parseServerControlHash = __ekMenuEx_static_parseServerControlHash;
		// Returns the server controls' hash-code of the supplied string (or
		// whatever was supplied if not a valid menu-submenu id string):
		// Parameters: 
		//	1 - The standard menu-submenu-id string (extra characters discarded).

	// Menu Folder-Button event handlers:
		ekMenuEx.menuBtnClickHdlr = __ekMenuEx_static_menuButtonClickEventHandler;
			// Handler for Menu-Button-Click events
			// Parameters: 
			//	1 - the event-object.
			
		ekMenuEx.menuBtnKeyHdlr = __ekMenuEx_static_menuButtonKeyDownEventHandler;
			// Handler for Menu-Button-Keydown events.
			// Parameters: 
			//	1 - the event-object.

		ekMenuEx.menuBtnMouseOverHdlr = __ekMenuEx_static_menuButtonMouseOverEventHandler;
			// Handler for Menu-Button-MouseOver events.
			// Parameters: 
			//	1 - the event-object.

		ekMenuEx.menuBtnMouseOutHdlr = __ekMenuEx_static_menuButtonMouseOutEventHandler;
			// Handler for Menu-Button-MouseOut events.
			// Parameters: 
			//	1 - the event-object.

		ekMenuEx.menuBtnFocusHdlr = __ekMenuEx_static_menuButtonFocusEventHandler;
			// Handler for Menu-Button-Focus events.
			// Parameters: 
			//	1 - the event-object.

		ekMenuEx.menuBtnBlurHdlr = __ekMenuEx_static_menuButtonBlurEventHandler;
			// Handler for Menu-Button-Blur events.
			// Parameters: 
			//	1 - the event-object.

		ekMenuEx.menuBtnLinkFocusHdlr = __ekMenuEx_static_menuButtonLinkFocusEventHandler;
			// Handler for Menu-Button-Link-onFocus events.
			// Parameters: 
			//	1 - the event-object.

		ekMenuEx.menuBtnLinkBlurHdlr = __ekMenuEx_static_menuButtonLinkBlurEventHandler;
			// Handler for Menu-Button-Link-onBlur events.
			// Parameters: 
			//	1 - the event-object.


	// Menu Item-Link event handlers:
		ekMenuEx.itemLinkClickHdlr = __ekMenuEx_static_menuItemLinkClickEventHandler;
			// Handler for Menu-Item-Link-Click events.
			// Parameters: 
			//	1 - the event-object.
		
		// Not Needed (key translated by browser, others handled by link pseudo classes):
			//ekMenuEx.itemLinkKeyHdlr = __ekMenuEx_static_menuItemLinkKeyDownEventHandler;
			//ekMenuEx.itemLinkMouseOverHdlr = __ekMenuEx_static_menuItemLinkMouseOverEventHandler;
			//ekMenuEx.itemLinkMouseOutHdlr = __ekMenuEx_static_menuItemLinkMouseOutEventHandler;
			//ekMenuEx.itemLinkFocusHdlr = __ekMenuEx_static_menuItemLinkFocusEventHandler;
			//ekMenuEx.itemLinkBlurHdlr = __ekMenuEx_static_menuItemLinkBlurEventHandler;


	// mouseIn and mouseOut event handlers:
		ekMenuEx.mouseIn = __ekMenuEx_static_mouseIn;
			// Prepare for delayed opening of the submenu related to the event-triggering element.
			// Parameters: 
			//	1 - the event-object.
		
		ekMenuEx.mouseOut = __ekMenuEx_static_mouseOut;
			// Prepare for delayed closing of the submenu related to the event-triggering element.
			// Parameters: 
			//	1 - the event-object.

		ekMenuEx.mouseIn_empty = function (event) {return (true);} // bubble event...

///////////////////////////////////////////////////////////////////////////////
// Initialize Private Static Members:
	ekMenuEx.private_isValidMenuSubmenuIdString = __ekMenuEx_static_isValidMenuSubmenuIdString
		// Verifies that the supplied element-ID string is a valid 
		// MenuSubmenuID string (ex. "ekmensel_1_submenu_2_button...")
		// Note: it may be more than this, but as long as the submitted string
		// begins with a valid and usable standard menu-submenu-id string, then
		// the results are positive (any extra appended characters are ignored).
		// Parameters: 
		//	1 - the id-string to test (may be an element-objects' Id).

	ekMenuEx.private_serverHelper_initialize = __ekMenuEx_static_serverHelper_initialize;
		// Calls initialization code, to configure and pre-open select menus.
		// Attempts to obtain a ekMenuEx object, and then calls its' 
		// initializeWithServerVariables() method...
		// Parameters: 
		//	1 - The standard menu-submenu-id string (submenu-id and extra characters discarded).
	
	ekMenuEx.private_startupAllSmartMenus = __ekMenuEx_static_serverHelper_startupAllSmartMenus;
		// Ensures that all ekMenu objects have been initialized.
		// Parameters: 
		//	None.
		
	ekMenuEx.private_shutdownAllSmartMenus = __ekMenuEx_static_serverHelper_shutdownAllSmartMenus;
		// Ensures that all ekMenu objects have been un-initialized (allows clean-up, if needed).
		// Parameters: 
		//	None.
		
	ekMenuEx.private_getMenuId = __ekMenuEx_static_getMenuId;
		// Returns the base (root) Menu-ID number, for a given ekmenu element ID.
		// Parameters: 
		//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).
	
	ekMenuEx.private_getMenuIdString = __ekMenuEx_static_getMenuIdString;
		// Returns the base (root) Menu-ID String, for a given ekmenu element ID.
		// Parameters: 
		//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).
		
	ekMenuEx.private_getSubmenuId = __ekMenuEx_static_getSubmenuId;
		// Returns the Submenu-ID number, for a given ekmenu element ID.
		// Parameters: 
		//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).
		
	ekMenuEx.private_getSubmenuIdString = __ekMenuEx_static_getSubmenuIdString;
		// Returns the Submenu-ID String, for a given ekmenu element ID.
		// Parameters: 
		//	1 - the elements' full Id (shuold contain the standard menu-submenu-id string).
		
	ekMenuEx.private_getEvent = __ekMenuEx_static_getEvent;
		// Returns the event object.
		// Parameters: 
		
	ekMenuEx.private_getEventElement = __ekMenuEx_static_getEventElement;
		// Returns the element object that triggered the event.
		// Parameters: 
		//	1 - the event (may be null if browser is IE).
		
	ekMenuEx.private_getIntNumber = __ekMenuEx_static_getIntNumber;
		// Returns the decimal equivelent of the given string value, 
		// or zero (0) if supplied string value is not a number.
		// Parameters: 
		//	1 - the string to convert to a number.

	ekMenuEx.private_isValidSubmenuObj = __ekMenuEx_static_isValidSubmenuObj;
		// Verifies that element is a valid submenu object.
		// Parameters: 
		//	1 - the submenu object to test.
		//	2 - the class-name to compare (may be a fragment, which 
		//	    is useful if the class name can vary - such as 
		//	    "ekmenu_button" and "ekmenu_button_selected").
		
	ekMenuEx.private_isValidSubmenuButton = __ekMenuEx_static_isValidSubmenuButton;
		// Verifies that element object is a valid submenu button.
		// Parameters: 
		//	1 - the button object to test.
		
	ekMenuEx.private_isValidSubmenuItems = __ekMenuEx_static_isValidSubmenuItems;
		// Verifies that element object is a valid submenu submenu_items.
		// Parameters: 
		//	1 - the submenu-items object to test.
		
	ekMenuEx.private_isValidSubmenu = __ekMenuEx_static_isValidSubmenu;
		// Verifies that element object is a valid submenu submenu.
		// Parameters: 
		//	1 - the submenu object to test.
		
	ekMenuEx.private_isValidSubmenuLink = __ekMenuEx_static_isValidSubmenuLink;
		// Verifies that element object is a valid submenu link.
		// Parameters: 
		//	1 - the submenu-link object to test.
		
	ekMenuEx.private_isValidEKMenu = __ekMenuEx_static_isValidEKMenu;
		// Verifies that element object is a valid main ekmenu object.
		// Parameters: 
		//	1 - the main-ekmenu-object to test.

	ekMenuEx.private_isDefined = __ekMenuEx_static_isDefined;
		// Verifies that the passed in object is not undefined.
		// Parameters: 
		//	1 - the object to test.

	ekMenuEx.isDefinedNotNull = __ekMenuEx_static_isDefinedNotNull;
		// Verifies that the passed in object is not undefined, and is not null.
		// Parameters: 
		//	1 - the main-ekmenu-object to test.

	ekMenuEx.hasClassName = __ekMenuEx_static_hasClassName;
		// Tests for the presence of a specified classname in the supplied object.
		// Parameters: 
		//	1 - the object to test.
		//  2 - the classname to search for.
		
	ekMenuEx.addClassName = __ekMenuEx_static_addClassName;
		// Ensures that the given object has the specified classname.
		// Parameters: 
		//	1 - the object to update.
		//  2 - the classname to add.
		
	ekMenuEx.removeClassName = __ekMenuEx_static_removeClassName;
		// Ensures that the given object does not have the specified classname.
		// Parameters: 
		//	1 - the object to update.
		//  2 - the classname to remove.
		
	// Constants:
		ekMenuEx.private_menuPrefix = "ekmensel_";
		ekMenuEx.private_namePrefix = "ekmengrp_";
		//Update: no longer used: ekMenuEx.private_submenuDelimiter = "_submenu_";
		ekMenuEx.private_buttonElementIdPostFix = "_button";
		ekMenuEx.private_submenuItemsElementIdPostFix = "_submenu_items";
		ekMenuEx.private_parentIdElementIdPostFix = "_parentid";
		ekMenuEx.private_ekmenuContainerElementIdPostFix = "_ekmenu"
		ekMenuEx.private_hashLength = 8;


//*********************************************************
// ekMenuEx Static Member Definitions Begin:
//*********************************************************

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function
// Returns the Menu-ObjectID for a given ekmenu element ID,
// creates a new ekMenuEx object if needed (stores these in
// an array as a property to the window object - making it 
// available/shared with all SmartMenus (this way multiple menus 
// can exist on a page, and have different objects that 
// are available anywhere - given an ekmenu element ID).
function __ekMenuEx_static_getMenuObj(elementId) {
	var menuObj = null;
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(elementId);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		//var menuIdString = ekMenuEx.private_getMenuIdString(menuSubmenuId);
		var menuHashCode = ekMenuEx.parseServerControlHash(menuSubmenuId);
		
		if (("undefined" == typeof window.ekMenuEx_MenuObjArray)
			|| (null == window.ekMenuEx_MenuObjArray)) {
			var MenuObjArray = new Array;
			menuObj = new ekMenuEx(menuSubmenuId);
			MenuObjArray[menuHashCode] = menuObj;
			window.ekMenuEx_MenuObjArray = MenuObjArray;
		} 
		else if (null == window.ekMenuEx_MenuObjArray[menuHashCode]) {
			window.ekMenuEx_MenuObjArray[menuHashCode] = menuObj = new ekMenuEx(menuSubmenuId);
		}
		else {
			menuObj = window.ekMenuEx_MenuObjArray[menuHashCode];
		}
	}
	return (menuObj);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the MenuSubmenuID string, for a given ekmenu element-ID
// (ex. given "ekmensel_1_submenu_2_button" returns
// "ekmensel_1_submenu_2" for root-menu 1, submenu 2):
//
// Update:
// Now prefixed with server-control IDs' hexidecimal hash-code:
// (ex. given "c580fa7b_1_2_button" returns
// "c580fa7b_1_2" for root-menu 1, submenu 2):
function __ekMenuEx_static_parseMenuSubmenuIdString(elementId) {
	var result = "";
	if (elementId 
		&& ("undefined" != typeof elementId)
		&& ("undefined" != typeof elementId.length)
		&& (elementId.length > 0)
		&& ("undefined" != typeof elementId.indexOf)) {

		var frag = elementId.split("_");
		if (frag[0] && (ekMenuEx.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
			result = frag[0] + "_" + frag[1] + "_" + frag[2];
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the server controls' hash-code of the supplied string (or
// whatever was supplied if not a valid menu-submenu id string):
function __ekMenuEx_static_parseServerControlHash(id) {
	var result = "";
	if (id && id.split) {
		var frag = id.split("_");
		if (frag[0] && (ekMenuEx.private_hashLength == frag[0].length)) {
			result = frag[0];
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the event object:
function __ekMenuEx_static_getEvent(e) {
	if (e) return (e);
	else return (window.event);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the element object that triggered the event:
function __ekMenuEx_static_getEventElement(e) {
	if (e) return ((e.srcElement) ? e.srcElement : e.target);
	else return (null);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-Click events:
function __ekMenuEx_static_menuButtonClickEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
			var menuObj = ekMenuEx.getMenuObj(el.id);
			if (menuObj) {
				var prevState = menuObj.isSubmenuSelected(el.id);
				if (prevState) {
					menuObj.unSelectSubmenu(el.id);
				}
				else {
					menuObj.selectSubmenu(el.id);
				}
				
				return (false); // event consumed.
			}
		}
	}
	return (true);	
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-Keydown events:
// Note: Typically called by a submenu-folder-button when 
//   a key is pressed, and 508-Compliance is disabled. 
function __ekMenuEx_static_menuButtonKeyDownEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
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
				ekMenuEx.menuBtnClickHdlr(ev);
				
				return (false); // event consumed.
			}
		}
	}
	return (true);	
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-MouseOver events:
function __ekMenuEx_static_menuButtonMouseOverEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
			var menuObj = ekMenuEx.getMenuObj(el.id);
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
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-MouseOut events:
function __ekMenuEx_static_menuButtonMouseOutEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
			var menuObj = ekMenuEx.getMenuObj(el.id);
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
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-Focus events:
function __ekMenuEx_static_menuButtonFocusEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
			var menuObj = ekMenuEx.getMenuObj(el.id);
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
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-Blur events:
function __ekMenuEx_static_menuButtonBlurEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
			var menuObj = ekMenuEx.getMenuObj(el.id);
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
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-Link-onFocus events:
function __ekMenuEx_static_menuButtonLinkFocusEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
			var menuObj = ekMenuEx.getMenuObj(el.id);
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
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Button-Link-onBlur events:
function __ekMenuEx_static_menuButtonLinkBlurEventHandler(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
			var menuObj = ekMenuEx.getMenuObj(el.id);
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
function __ekMenuEx_static_getValidParentId(el) {
	var topContainer = "_ekmenu";
	var len = topContainer.length;
	
	while(el 
		&& (el.parentNode)) {
		
		if (el.id && (el.id.length)) {
			if (ekMenuEx.private_isValidMenuSubmenuIdString(el.id)) {
				return (el.id);
			}
		}
		el = el.parentNode;
	}

	return ("");
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// mouseIn event handler; prepares for delayed opening of 
// the submenu related to the event-triggering element.
// Parameters: 
//	1 - the event-object.
function __ekMenuEx_static_mouseIn(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
		    var elmtId = el.id;
			if (0 == elmtId.length) {
				elmtId = __ekMenuEx_static_getValidParentId(el);
			}
			var menuObj = ekMenuEx.getMenuObj(elmtId);
			if (menuObj) {
				menuObj.mouseIn(e, el);
				return (true); // event not-consumed (allow bubbling).
			}
		}
	}
	return (true);	
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// mouseOut event handler; prepares for delayed closing of 
// the submenu related to the event-triggering element.
// Parameters: 
//	1 - the event-object.
function __ekMenuEx_static_mouseOut(e) {
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {
		    var elmtId = el.id;
			if (0 == elmtId.length) {
				elmtId = __ekMenuEx_static_getValidParentId(el);
			}
			var menuObj = ekMenuEx.getMenuObj(elmtId);
			if (menuObj) {
				menuObj.mouseOut(e, el);
				return (true); // event not-consumed (allow bubbling).
			}
		}
	}
	return (true);	
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Handler for Menu-Item-Link-Click events:
function __ekMenuEx_static_menuItemLinkClickEventHandler(e) {
	var linkIsAButton = false;
	var ev = ekMenuEx.private_getEvent(e);
	if (ev) {
		var el = ekMenuEx.private_getEventElement(ev);
		if (el && ("undefined" != el.id)) {

			if (ekMenuEx.isDefinedNotNull(el)
				&& ekMenuEx.isDefinedNotNull(el.tagName)
				&& ("IMG" == el.tagName)) 
			{
				// The element is an image, attempt to pass
				// -off the event to the wrapping element: 
				if (ekMenuEx.isDefinedNotNull(el.parentNode) 
					&& ekMenuEx.private_isValidSubmenuButton(el.parentNode))
				{
					el = el.parentNode;
					if (ekMenuEx.isDefinedNotNull(el.click)) {
						el.click(ev);
						return (false);
					}
					linkIsAButton = true;
				}
				else {
					return (true); 
				}
			}
			else if (ekMenuEx.private_isValidSubmenuButton(el)) {
				linkIsAButton = true;
			}

			var menuObj = ekMenuEx.getMenuObj(el.id);
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
				return (menuObj.selectMenuItem(el));
			}
		}
	}
	return (true);	
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the decimal equivelent of the given string value, 
// or zero (0) if supplied string value is not a number:
function __ekMenuEx_static_getIntNumber(val) {
	var result = 0;
	var tempResult = parseInt(val, 10);
	if (NaN != tempResult) {
		result = tempResult;
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the Menu-ID number, for a given ekmenu element ID:
function __ekMenuEx_static_getMenuId(elementId) {
	var result = 0;
	var idString = ekMenuEx.private_getMenuIdString(elementId);
	if (idString.length) {
		result = ekMenuEx.private_getIntNumber(idString);
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the Menu-ID String, for a given ekmenu element ID:
function __ekMenuEx_static_getMenuIdString(elementId) {
	var result = "";
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(elementId);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var frag = elementId.split("_");
		if (frag[0] && (ekMenuEx.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
			result = frag[1];
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the Submenu-ID number, for a given ekmenu element ID:
function __ekMenuEx_static_getSubmenuId(elementId) {
	var result = 0;
	var idString = ekMenuEx.private_getSubmenuIdString(elementId);
	if (idString.length) {
		result = ekMenuEx.private_getIntNumber(idString);
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Returns the Submenu-ID String, for a given ekmenu element ID:
function __ekMenuEx_static_getSubmenuIdString(elementId) {
	var result = "";
	if (ekMenuEx.private_isValidMenuSubmenuIdString(elementId)) {
		var frag = elementId.split("_");
		if (frag[0] && (ekMenuEx.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
			result = frag[2];
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Verifies that the supplied element-ID string is a valid 
// MenuSubmenuID string (ex. "ekmensel_1_submenu_2_button")
function __ekMenuEx_static_isValidMenuSubmenuIdString(elementId) {
	var result = false;
	if (elementId 
		&& ("undefined" != typeof elementId)
		&& ("undefined" != typeof elementId.length)
		&& (elementId.length > 0)
		&& ("undefined" != typeof elementId.indexOf)) {
		var frag = elementId.split("_");
		if (frag[0] && (ekMenuEx.private_hashLength == frag[0].length) && frag[1] && frag[2]) {
					result = true;
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Verifies that element is a valid submenu object:
function __ekMenuEx_static_isValidSubmenuObj(obj, classNameFrag) {
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
// ekMenuEx Static Member Helper Function.
// Verifies that element object is a valid submenu button:
function __ekMenuEx_static_isValidSubmenuButton(obj) {
	return (ekMenuEx.private_isValidSubmenuObj(obj, ekMenuEx_classNames.button));
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Verifies that element object is a valid submenu submenu_items:
function __ekMenuEx_static_isValidSubmenuItems(obj) {
	return (ekMenuEx.private_isValidSubmenuObj(obj, "submenu_items"));
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Verifies that element object is a valid submenu submenu:
function __ekMenuEx_static_isValidSubmenu(obj) {
	return (ekMenuEx.private_isValidSubmenuObj(obj, "submenu"));
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Verifies that element object is a valid submenu link:
function __ekMenuEx_static_isValidSubmenuLink(obj) {
	return (ekMenuEx.private_isValidSubmenuObj(obj, "link"));
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Verifies that element object is a valid main ekmenu object:
function __ekMenuEx_static_isValidEKMenu(obj) {
	return (ekMenuEx.private_isValidSubmenuObj(obj, "ekmenu"));
}

///////////////////////////////////////////////////////////
// Verifies that the passed in object is not undefined.
// Parameters: 
//	1 - the main-ekmenu-object to test.
function __ekMenuEx_static_isDefined(obj) {
	return ("undefined" != typeof obj);
}

///////////////////////////////////////////////////////////
// Verifies that the passed in object is not 
// undefined, and is not null.
// Parameters: 
//	1 - the main-ekmenu-object to test.
function __ekMenuEx_static_isDefinedNotNull(obj) {
	return (ekMenuEx.private_isDefined(obj) && (null != obj));
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Calls initialization code, to configure and pre-open select menus:
function __ekMenuEx_static_serverHelper_initialize(id) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(id);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		//var menuHashCode = ekMenuEx.parseServerControlHash(menuSubmenuId);
		var menuObj = ekMenuEx.getMenuObj(menuSubmenuId);
		if (menuObj) {
			menuObj.initializeWithServerVariables();
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Ensures that all ekMenu objects have been initialized:
function __ekMenuEx_static_serverHelper_startupAllSmartMenus() {
	if (("undefined" != typeof window.ekMenuEx_ekmenuArray)
		&& (null != window.ekMenuEx_ekmenuArray)
		&& ("undefined" != typeof window.ekMenuEx_ekmenuArray.length)
		&& (null != window.ekMenuEx_ekmenuArray.length)) {
		
		for (var idx = 0; idx < window.ekMenuEx_ekmenuArray.length; idx++) {
			var startMenu = window.ekMenuEx_ekmenuArray[idx];
			if (startMenu.length) 
				ekMenuEx.private_serverHelper_initialize(startMenu);
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Ensures that all ekMenu objects have been initialized:
function __ekMenuEx_static_serverHelper_shutdownAllSmartMenus() {
	// ----------------------------------------------------------
	// Note: This function should remain hooked even if there
	// is nothing to deallocate/cleanup, as it corrects an
	// issue where some browsers (FireFox) attempt to cache the
	// page and reload it when the user clicks the back button
	// WITHOUT FIRING THE ONLOAD EVENT!!! This means that the 
	// Javascript initialization code doesn't run and the menu is
	// left in whatever state it was in when the page was left.
	// (See defect #23045 ...)
	// But hooking either onbeforeunload or onunload causes the 
	// browser to fire the onload event when the back button is 
	// clicked, as it appears to note that the page unitialized...
	// ----------------------------------------------------------
}

///////////////////////////////////////////////////////////
// ekMenuEx Static Member Helper Function.
// Tests for the presence of a specified classname in the supplied object.
function __ekMenuEx_static_hasClassName(obj, className) {
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
// ekMenuEx Static Member Helper Function.
// Ensures that the given object has the specified classname.
function __ekMenuEx_static_addClassName(obj, className) {
	if (ekMenuEx.hasClassName(obj, className))
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
// ekMenuEx Static Member Helper Function.
// Ensures that the given object does not have the specified classname.
function __ekMenuEx_static_removeClassName(obj, className) {
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


//*********************************************************
// ekMenuEx Instance Member Definitions Begin:
//*********************************************************

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the root-menu id string.
function __ekMenuEx_returnMenuId() {
	return (this.private_menuIdString);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the hash-code of the server control.
function __ekMenuEx_returnHashCode() {
	return (this.private_serverControlHash);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the root-menu id string of the supplied string (or
// whatever was supplied if not a valid menu-submenu id string):
function __ekMenuEx_parseMenuId(id) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(id);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		return (ekMenuEx.private_getMenuIdString(menuSubmenuId));
	}
	else {
		return (id);
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns boolean, indicating if identified button is 
// currently selected (and therefore, then the associated
// visibility state of the identified submenu items):
function __ekMenuEx_isSubmenuSelected(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var btnObj = this.getFolderButtonObject(menuSubmenuId);
		if (ekMenuEx.private_isValidSubmenuButton(btnObj)) {
			return (ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonSelected)
				|| ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonSelectedHover));
		}
	}
	return (false);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Selects the identified menu; if there is a folder-button, 
// then the class is updated to selected state. Then shows 
// the associated submenu items:
function __ekMenuEx_selectSubmenu(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		if (this.private_autoCollapseSubmenus) {
			this.collapseAllOpenSubmenus(false);
		}
		this.private_selectedMenuList = menuSubmenuId;
		this.selectSubmenuHelper(menuSubmenuId);
	
		this.callSlave__showSubmenuBranch(idString);
		this.private_selectionChanged = true;
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_getSlaveControlObject() {
	var result = null;
	if (this.private_isMasterControl
		&& ekMenuEx.isDefinedNotNull(this.private_slaveControl) 
		&& ekMenuEx.isDefinedNotNull(this.private_slaveControl.length)
		&& (this.private_slaveControl.length > 0)) {
		var slaveId = this.private_slaveControl + "_" + this.menuId() + "_0";
		var slaveObj = ekMenuEx.getMenuObj(slaveId);
		if (slaveObj) {
			result = slaveObj;
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_callSlave__showSubmenuBranch(idString) {
	var slaveObj = this.getSlaveControlObject();
	var btnContainer;
	if (slaveObj) {
		if (slaveObj.showSubmenuBranch(this.convertIdToSlaveControlId(slaveObj, idString))) {
			// slave menu succesfully activated, mark top button properly,
			// after ensuring all other top-level-buttons are un-marked:
			for (var ui in this.topLevelUI) {
				btnContainer = document.getElementById(ui);
				if (btnContainer) {
					if (ekMenuEx.hasClassName(btnContainer, ekMenuEx_classNames.slaveBranchSelected)) {
						ekMenuEx.removeClassName(btnContainer, ekMenuEx_classNames.slaveBranchSelected);
					}
				}
			}

			var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
			var parentId = this.getParentLevelSubmenuId(menuSubmenuId);
			while ((parentId != menuSubmenuId) && (0 != parentId)) {
				if (this.isTopLevelUI(parentId)) {
					btnContainer = document.getElementById(parentId);
					if (btnContainer) {
						if (!ekMenuEx.hasClassName(btnContainer, ekMenuEx_classNames.slaveBranchSelected)) {
							ekMenuEx.addClassName(btnContainer, ekMenuEx_classNames.slaveBranchSelected);
						}
					}
					break;
				}
				parentId = this.getParentLevelSubmenuId(menuSubmenuId);
			}
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_initializeSlaveMenu() {
	var isVisible = false;
	if (ekMenuEx.private_getIntNumber(this.private_swRevision) > 0) {
		var firstObj = null;
		var menuSubmenuId;
		var inAr = this.getEkMenuElementsByTagName("INPUT");
		for (var idx=0; idx < inAr.length; idx++) {
			if (inAr[idx].value.indexOf(this.private_masterControlIdHash) == 0) {
				var localId = inAr[idx].id;
				var obj;
				if (localId.length >= ekMenuEx.private_hashLength) {
					localId = this.buildMenuSubmenuId(ekMenuEx.private_getSubmenuIdString(localId)) + ekMenuEx.private_submenuItemsElementIdPostFix;
					obj = document.getElementById(localId);
					if (obj) {
						if (null == firstObj) {
							firstObj = obj;
						}

						if (ekMenuEx.hasClassName(obj, ekMenuEx_classNames.submenuItems)) {
							isVisible = true;
						}
						
						if (null == this.topLevelUI) {
							this.topLevelUI = new Array;
						}
						menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(localId);
						if (!this.topLevelUI[menuSubmenuId]) {
							this.topLevelUI[menuSubmenuId] = true;
						}
					}
				}
			}
		}
	}
	if ((!isVisible) && firstObj) {
		ekMenuEx.removeClassName(firstObj, ekMenuEx_classNames.submenuItemsHidden);
		ekMenuEx.addClassName(firstObj, ekMenuEx_classNames.submenuItems);
		//this.selectSubmenu(firstSubmenu);
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_initializeMasterMenu() {
	var menuSubmenuId = this.buildMenuSubmenuId(0) + ekMenuEx.private_submenuItemsElementIdPostFix;
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var node = document.getElementById(menuSubmenuId);
		if (node) {
			var nodes = node.childNodes;
			for (var idx = 0; idx < nodes.length; idx++) {
				if (null == this.topLevelUI) {
					this.topLevelUI = new Array;
				}
				menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(nodes[idx].id);
				if (!this.topLevelUI[menuSubmenuId]) {
					this.topLevelUI[menuSubmenuId] = true;
				}
			}
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_showSubmenuBranch(idString) {
	var result = false;
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var id = menuSubmenuId;
		var obj = document.getElementById(id);
		if (obj) {
			//if (this.private_autoCollapseSubmenus) {
			//	this.collapseAllOpenSubmenus(false);
			//}

			for (var ui in this.topLevelUI) {
				this.unSelectSubmenu(ui, true);
			}
			
			this.selectSubmenuHelper(menuSubmenuId);
			result = true;
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_unSelectSubmenuList(menuList) {
	var listAr = menuList.split(",");
	var idx;
	for (idx=0; idx < listAr.length; idx++) {
		this.unSelectSubmenu(listAr[idx]);
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_convertIdToSlaveControlId(slaveObj, idString) {
	var result = idString;
	if (slaveObj && idString && idString.length && (idString.length >= ekMenuEx.private_hashLength)) {
		result = slaveObj.hashCode() + idString.substr(ekMenuEx.private_hashLength);
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
function __ekMenuEx_isTopLevelUI(idString) {
	return (this.topLevelUI && this.topLevelUI[ekMenuEx.parseMenuSubmenuIdString(idString)]);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Helper funtion for __ekMenuEx_selectSubmenu, uses 
// recursionSelects to ensure selected submenus are visible
// even if they are buried with muliple nesting levels:
function __ekMenuEx_selectSubmenuHelper(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		if (this.private_selectedMenuList.length) {
			this.private_selectedMenuList += "," + menuSubmenuId;
		}
		else {
			this.private_selectedMenuList = menuSubmenuId;
		}
		
		var btnObj = this.getFolderButtonObject(menuSubmenuId);
		if (ekMenuEx.private_isValidSubmenuButton(btnObj)) {
			var wasHovering = (ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonHover)
				|| ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonSelectedHover));
			if (wasHovering) {
				ekMenuEx.removeClassName(btnObj, ekMenuEx_classNames.buttonHover);
				ekMenuEx.addClassName(btnObj, ekMenuEx_classNames.buttonSelectedHover);
			}
			else {
				ekMenuEx.removeClassName(btnObj, ekMenuEx_classNames.button);
				ekMenuEx.addClassName(btnObj, ekMenuEx_classNames.buttonSelected);
			}
		}
		
		var itmObj = this.getSubmenuItemsObject(menuSubmenuId);
		if (ekMenuEx.private_isValidSubmenuItems(itmObj)) {
			ekMenuEx.removeClassName(itmObj, ekMenuEx_classNames.submenuItemsHidden);
			ekMenuEx.addClassName(itmObj, ekMenuEx_classNames.submenuItems);
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
// ekMenuEx Instance Member Helper Function.
// Unselects the identified menu; if there is a folder-button, 
// then the class is updated to an unselected state. Then 
// hides the associated submenu items:
function __ekMenuEx_unSelectSubmenu(idString, topLevelUIOverride) {
	if (idString && idString.length) {
		var overrideTopLevelUI = false;
		if (ekMenuEx.isDefinedNotNull(topLevelUIOverride)) {
			overrideTopLevelUI = topLevelUIOverride;
		}
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		if ((ekMenuEx.private_getSubmenuId(menuSubmenuId) > 0) 
				&& (overrideTopLevelUI || !this.private_isSlaveControl || !this.isTopLevelUI(menuSubmenuId))) {
		var btnObj = this.getFolderButtonObject(menuSubmenuId);
		if (ekMenuEx.private_isValidSubmenuButton(btnObj)) {
			var wasHovering = (ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonHover)
				|| ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonSelectedHover));
			if (wasHovering) {
				ekMenuEx.removeClassName(btnObj, ekMenuEx_classNames.buttonSelectedHover);
				ekMenuEx.addClassName(btnObj, ekMenuEx_classNames.buttonHover);
			}
			else {
				ekMenuEx.removeClassName(btnObj, ekMenuEx_classNames.buttonSelected);
				ekMenuEx.addClassName(btnObj, ekMenuEx_classNames.button);
			}
		}
		
		var itmObj = this.getSubmenuItemsObject(menuSubmenuId);
		if (ekMenuEx.private_isValidSubmenuItems(itmObj)) {
			ekMenuEx.removeClassName(itmObj, ekMenuEx_classNames.submenuItems);
			ekMenuEx.addClassName(itmObj, ekMenuEx_classNames.submenuItemsHidden);
		}

		this.unMarkParentSubmenu(menuSubmenuId);
	}
}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Closes all currently open submenus, to prevent overlap & visual clutter:
function __ekMenuEx_collapseAllOpenSubmenus(showRootFlag) {
	if (this.private_selectionChanged) {
		this.unSelectSubmenuList(this.private_selectedMenuList);
		this.private_selectedMenuList = ""
	}
	else {
		var openMenusArray;
		if (ekMenuEx.private_getIntNumber(this.private_swRevision) > 0) {
			openMenusArray = this.getElementsByClassNameAndTagName(ekMenuEx_classNames.submenuItems, "UL");
		}
		else {
			openMenusArray = this.getElementsByClassName(ekMenuEx_classNames.submenuItems);
		}

		// hide all visible submenus:
	for (var idx=0; idx < openMenusArray.length; idx++) {
		this.unSelectSubmenu(openMenusArray[idx].id);
	}
	
		// TODO: FIX: ensure all buttons are disabled (should be done 
		// by previous step, but this fails for master/slave menus):
		var activeButtons = this.getElementsByClassNameAndTagName(ekMenuEx_classNames.buttonSelected, "SPAN");
		for (idx=0; idx < activeButtons.length; idx++) {
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
// ekMenuEx Instance Member Helper Function.
// Hide sibling submenus of the designated submenu:
function __ekMenuEx_collapseSiblingSubmenus(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var parentLevelId = this.getParentLevelSubmenuId(menuSubmenuId);
		if (ekMenuEx.private_isValidMenuSubmenuIdString(parentLevelId)
			&& (parentLevelId != menuSubmenuId)) {
			var idArray = this.getDirectChildIds(parentLevelId)
			for (var idx=0; idx < idArray.length; idx++) {
				if (idArray[idx] != menuSubmenuId) {
					this.unSelectSubmenu(idArray[idx]);
				}
			}
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Sets the parent folders' style to be a parent (optionally 
// used in CSS to style parents differently):
function __ekMenuEx_markParentSubmenu(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var parentLevelId = this.getParentLevelSubmenuId(menuSubmenuId);
		if (ekMenuEx.private_isValidMenuSubmenuIdString(parentLevelId)
			&& (parentLevelId != menuSubmenuId)) 
		{
			var parentObj = this.getSubmenuObject(parentLevelId);
			if ((ekMenuEx.isDefinedNotNull(parentObj)) 
				&& (ekMenuEx.private_isDefined(parentObj.className)))
			{
				if (ekMenuEx.hasClassName(parentObj, ekMenuEx_classNames.submenu)) {
					ekMenuEx.removeClassName(parentObj, ekMenuEx_classNames.submenu);
					ekMenuEx.addClassName(parentObj, ekMenuEx_classNames.submenuParent);
				}
				else if (ekMenuEx.hasClassName(parentObj, ekMenuEx_classNames.submenuHover)) {
					ekMenuEx.removeClassName(parentObj, ekMenuEx_classNames.submenuHover);
					ekMenuEx.addClassName(parentObj, ekMenuEx_classNames.submenuParentHover);
				}
			}
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Sets the parent folders' style to be a normal non-parent 
// (optionally used in CSS to style parents & children differently):
function __ekMenuEx_unMarkParentSubmenu(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var parentLevelId = this.getParentLevelSubmenuId(menuSubmenuId);
		if (ekMenuEx.private_isValidMenuSubmenuIdString(parentLevelId)
			&& (parentLevelId != menuSubmenuId)) 
		{
			var parentObj = this.getSubmenuObject(parentLevelId);
			if ((ekMenuEx.isDefinedNotNull(parentObj)) 
				&& (ekMenuEx.private_isDefined(parentObj.className))) 
			{
				if (ekMenuEx.hasClassName(parentObj, ekMenuEx_classNames.submenuParent)) {
					ekMenuEx.removeClassName(parentObj, ekMenuEx_classNames.submenuParent);
					ekMenuEx.addClassName(parentObj, ekMenuEx_classNames.submenu);
				}
				else if (ekMenuEx.hasClassName(parentObj, ekMenuEx_classNames.submenuParentHover)) {
					ekMenuEx.removeClassName(parentObj, ekMenuEx_classNames.submenuParentHover);
					ekMenuEx.addClassName(parentObj, ekMenuEx_classNames.submenuHover);
				}
			}
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Sets the menus' container style to be hovered,
// (optionally used in CSS to style contents & children differently):
function __ekMenuEx_hoverSubmenu(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var menuObj = this.getSubmenuObject(menuSubmenuId);
		if ((ekMenuEx.isDefinedNotNull(menuObj)) 
			&& (ekMenuEx.private_isDefined(menuObj.className)))
		{
			if (ekMenuEx.hasClassName(menuObj, ekMenuEx_classNames.submenu)) {
				ekMenuEx.removeClassName(menuObj, ekMenuEx_classNames.submenu);
				ekMenuEx.addClassName(menuObj, ekMenuEx_classNames.submenuHover);
			}
			else if (ekMenuEx.hasClassName(menuObj, ekMenuEx_classNames.submenuParent)) {
				ekMenuEx.removeClassName(menuObj, ekMenuEx_classNames.submenuParent);
				ekMenuEx.addClassName(menuObj, ekMenuEx_classNames.submenuParentHover);
			}
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Sets the menus' container style to be hovered,
// (optionally used in CSS to style contents & children differently):
function __ekMenuEx_unHoverSubmenu(idString) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var menuObj = this.getSubmenuObject(menuSubmenuId);
		if ((ekMenuEx.isDefinedNotNull(menuObj)) 
			&& (ekMenuEx.private_isDefined(menuObj.className))) 
		{
			if (ekMenuEx.hasClassName(menuObj, ekMenuEx_classNames.submenuHover)) {
				ekMenuEx.removeClassName(menuObj, ekMenuEx_classNames.submenuHover);
				ekMenuEx.addClassName(menuObj, ekMenuEx_classNames.submenu);
			}
			else if (ekMenuEx.hasClassName(menuObj, ekMenuEx_classNames.submenuParentHover)) {
				ekMenuEx.removeClassName(menuObj, ekMenuEx_classNames.submenuParentHover);
				ekMenuEx.addClassName(menuObj, ekMenuEx_classNames.submenuParent);
			}
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns an array of all direct child-submenu-ids (length = 0 if none):
function __ekMenuEx_getDirectChildIds(idString) {
	var result = new Array;
	var elementName = ekMenuEx.private_namePrefix + "submenu_items";
	var cmpId, elementArray;
	var parentMenuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(parentMenuSubmenuId)) {
		elementArray = this.getEkMenuElementsByName(elementName);
		if (("undefined" != typeof elementArray)
			&& (null != elementArray)
			&& ("undefined" != typeof elementArray.length)
			&& (null != elementArray.length))
			{
				for (var idx=0; idx < elementArray.length; idx++) {
					cmpId = ekMenuEx.parseMenuSubmenuIdString(elementArray[idx].id);
					if (ekMenuEx.private_isValidMenuSubmenuIdString(cmpId)) {
						if ((parentMenuSubmenuId == this.getParentLevelSubmenuId(cmpId)
							&& (parentMenuSubmenuId != cmpId))) {  //ekMenuEx.private_getSubmenuIdString
							result[result.length] = cmpId;
						}
					}
				}
			}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// returns an array of the menu-elements whose name 
// attribute matches the supplied name:
function __ekMenuEx_getEkMenuElementsByName(elementName) {
	var result = new Array;
	var divArray = this.getEkMenuElementsByTagName("div");
	for (var idx=0; idx < divArray.length; idx++) {
		if (elementName == divArray[idx].name) {
			result[result.length] = divArray[idx];
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// returns an array of the ekmenu-elements with the 
// specified tag-name: 
function __ekMenuEx_getEkMenuElementsByTagName(tagName) {
	var result = new Array;
	var ekmenuContainer = this.getEkMenuContainerElement();
	if (ekmenuContainer && ("undefined" != typeof ekmenuContainer.getElementsByTagName)) {
		var divArray = ekmenuContainer.getElementsByTagName(tagName);
		if (("undefined" != typeof divArray) && (null != divArray)) {
			result = divArray;
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// returns an array of the menu-elements whose className 
// attribute matches the supplied name:
function __ekMenuEx_getElementsByClassName(className) {
	var result = new Array;
	var divArray = this.getEkMenuElementsByTagName("*");
	for (var idx=0; idx < divArray.length; idx++) {
		if (("undefined" != divArray[idx].className)
			&& (ekMenuEx.hasClassName(divArray[idx], className))) {
			result[result.length] = divArray[idx];
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// returns an array of the menu-elements whose className 
// attribute matches the supplied name:
function __ekMenuEx_getElementsByClassNameAndTagName(className, tagName) {
	var result = new Array;
	var divArray = this.getEkMenuElementsByTagName(tagName);
	for (var idx=0; idx < divArray.length; idx++) {
		if (("undefined" != divArray[idx].className)
			&& (ekMenuEx.hasClassName(divArray[idx], className))) {
			result[result.length] = divArray[idx];
		}
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Unselects the identified menu; if there is a folder-button, 
// then the class is updated to an unselected state. Then 
// hides the associated submenu items:
function __ekMenuEx_hoverButton(idString, hoverFlag) {
	var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(idString);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
		var btnObj = this.getFolderButtonObject(menuSubmenuId);
		if (ekMenuEx.private_isValidSubmenuButton(btnObj)) {
			var wasHovering = (ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonHover)
				|| ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonSelectedHover));
			if (hoverFlag == wasHovering) {
				return;
			}
			var isSelected = (ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonSelected) 
				|| ekMenuEx.hasClassName(btnObj, ekMenuEx_classNames.buttonSelectedHover));
			if (hoverFlag) {
				ekMenuEx.removeClassName(btnObj, ((isSelected) ? ekMenuEx_classNames.buttonSelected : ekMenuEx_classNames.button));
				ekMenuEx.addClassName(btnObj, ((isSelected) ? ekMenuEx_classNames.buttonSelectedHover : ekMenuEx_classNames.buttonHover));
			}
			else {
				ekMenuEx.removeClassName(btnObj, ((isSelected) ? ekMenuEx_classNames.buttonSelectedHover : ekMenuEx_classNames.buttonHover));
				ekMenuEx.addClassName(btnObj, ((isSelected) ? ekMenuEx_classNames.buttonSelected : ekMenuEx_classNames.button));
			}
		}
	}
}

///////////////////////////////////////////////////////////
// Annonymous Helper Function.
// Called by __ekMenuEx_mouseIn to prepare for the
// delayed opening of identified submenu.
// Parameters: 
//	1 - the ID of the element that triggered the event.
function __ekMenuEx_mouseInHelperCaller(id) {
	if (id) {
		var menuObj = ekMenuEx.getMenuObj(id);
		if (menuObj) {
			menuObj.mouseInHelper();
		}
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Called by external (non-object-instance) code, to prepare for 
// delayed opening of identified submenu.
// Parameters: 
//	1 - the event object.
//	2 - the element-object that triggered the event.
function __ekMenuEx_mouseIn(e, el) {
	if (this.private_mouseEventTimer) {
		window.clearTimeout(this.private_mouseEventTimer);
		this.private_mouseEventTimer = null;
	}
	this.private_mouseEventEnteringElementId = el.id;
	this.private_mouseEventTimer = window.setTimeout(function () {__ekMenuEx_mouseInHelperCaller(el.id)}, 50);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Shows/selects the appropriate submenu.
function __ekMenuEx_mouseInHelper() {
	if (this.private_mouseEventEnteringElementId) {
		var menuSubmenuId = ekMenuEx.parseMenuSubmenuIdString(this.private_mouseEventEnteringElementId);
		if (ekMenuEx.private_isValidMenuSubmenuIdString(menuSubmenuId)) {
			if (this.private_isMasterControl) {
				// Dont select bottom level menus for master-control 
				// via mouse-over; force user to click to select these:
				itemsObj = this.getSubmenuItemsObject(menuSubmenuId);
				if (!ekMenuEx.isDefinedNotNull(itemsObj)) {
					return;
				}
			}
			this.selectSubmenu(menuSubmenuId);
		}
	}
}

///////////////////////////////////////////////////////////
// Annonymous Helper Function.
// Called by __ekMenuEx_mouseOut to prepare for the
// delayed opening of identified submenu.
// Parameters: 
//	1 - the ID of the element that triggered the event.
function __ekMenuEx_mouseOutHelperCaller(id) {
	if (id) {
		var menuObj = ekMenuEx.getMenuObj(id);
		if (menuObj) {
			menuObj.mouseOutHelper();
		}
	}
}


///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Called by external (non-object-instance) code, to prepare for 
// delayed opening of identified submenu.
// Parameters: 
//	1 - the event object.
//	2 - the element-object that triggered the event.
function __ekMenuEx_mouseOut(e, el) {
	if (this.private_mouseEventTimer) {
		window.clearTimeout(this.private_mouseEventTimer);
		this.private_mouseEventTimer = null;
	}
	this.private_mouseEventExitingElementId = el.id;
	this.private_mouseEventTimer = window.setTimeout(function () {__ekMenuEx_mouseOutHelperCaller(el.id)}, 500);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Hides/unselects the appropriate submenu (possibly all but root).
// Parameters: 
//	None.
function __ekMenuEx_mouseOutHelper() {
	//if (this.private_mouseEventEnteringElementId) {
	//	this.unSelectSubmenu(this.private_mouseEventEnteringElementId);
	//}
	if (this.private_autoCollapseSubmenus) {
		this.collapseAllOpenSubmenus();
	}
	else if (this.private_mouseEventEnteringElementId) {
		this.unSelectSubmenu(this.private_mouseEventEnteringElementId);
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the Menu-Submenu-Id string, for a given Submenu-Id:
function __ekMenuEx_buildMenuSubmenuId(submenuId) {
	return (this.hashCode() + "_" + this.menuId() + "_" + submenuId);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the corresponding submenu-folder-button object, 
// for a given Submenu-Id (or Menu-Submenu-Id):
function __ekMenuEx_getFolderButtonObject(submenuId) {
	var id = ekMenuEx.parseMenuSubmenuIdString(submenuId);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(id)) {
		id = id + ekMenuEx.private_buttonElementIdPostFix;
	}
	else {
		id = this.buildMenuSubmenuId(submenuId) + ekMenuEx.private_buttonElementIdPostFix;
	}
	return (document.getElementById(id));
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the corresponding submenu-Items object, 
// for a given Submenu-Id (or Menu-Submenu-Id):
function __ekMenuEx_getSubmenuItemsObject(submenuId) {
	var id = ekMenuEx.parseMenuSubmenuIdString(submenuId);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(id)) {
		id = id + ekMenuEx.private_submenuItemsElementIdPostFix;
	}
	else {
		id = this.buildMenuSubmenuId(submenuId) + ekMenuEx.private_submenuItemsElementIdPostFix;
	}
	return (document.getElementById(id));
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the corresponding submenu object, 
// for a given Submenu-Id (or Menu-Submenu-Id):
function __ekMenuEx_getSubmenuObject(submenuId) {
	var id = ekMenuEx.parseMenuSubmenuIdString(submenuId);
	var result = null;
	if (ekMenuEx.private_isValidMenuSubmenuIdString(id)) {
		result = document.getElementById(id);
	}
	return (result);
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Returns the parent-submenu-id for the given Submenu-Id 
// (or the given Menu-Submenu-Id), returns zero if top (root):
function __ekMenuEx_getParentLevelSubmenuId(submenuId) {
	var result = this.buildMenuSubmenuId("0"); // default to root.
	var id = ekMenuEx.parseMenuSubmenuIdString(submenuId);
	if (ekMenuEx.private_isValidMenuSubmenuIdString(id)) {
		id = id + ekMenuEx.private_parentIdElementIdPostFix;
	}
	else {
		id = this.buildMenuSubmenuId(submenuId) + ekMenuEx.private_parentIdElementIdPostFix;
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
// ekMenuEx Instance Member Helper Function.
// Returns the outermost container element (DIV) that
// holds this entire ekMenu object:
function __ekMenuEx_getEkMenuContainerElement() {
	var containerId = this.hashCode() + "_"
		+ this.menuId() 
		+ "_"
		+ "0" 
		+ ekMenuEx.private_ekmenuContainerElementIdPostFix;
	var containerObj = document.getElementById(containerId);
	if (containerObj
		&& ekMenuEx.private_isValidEKMenu(containerObj)) {
		return (containerObj);
	}
	else {
		return (null);
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Called when a menu-item (such as a link) is clicked, 
// modifies the items href parameter to pass the selected
// item info over the querystring:
function __ekMenuEx_ekMenu_selectMenuItem(el) {
	var elm = el;
	var isWrapper = false;
	
	if (ekMenuEx.isDefinedNotNull(elm)
		&& ekMenuEx.isDefinedNotNull(elm.parentNode)
		&& ekMenuEx.isDefinedNotNull(elm.parentNode.tagName)
		&& ("A" == elm.parentNode.tagName)) 
	{
		// event is from a button, that's wrapped 
		// with an anchor. Use the anchor element:
		elm = elm.parentNode; 
		isWrapper = true;
	}
		
	if (elm && ("undefined" != typeof elm.id) && ("undefined" != typeof elm.href)) {
		// Update: to correct a problem with FireFox (where events like mouse-out
		// could call the handlers between the current page unloading and the next
		// page loading) we must ensure that the event handlers are not called
		// while we're navigating/submitting the page:
		// TODO: Test for IE, skip if true (only needed for non-IE browsers, particuarly FireFox).
		if (elm.href.indexOf("javascript://") < 0)
		{
		    this.disableAllEventHandlers();
		}
		
// ****************************************************************************
// Prevent appending ekmensel parameter to querystring; workaround for possible
// issue of mis-matching Control ID causing failure to identify item...
// ****************************************************************************		
//		if (elm.href.indexOf("?") < 0) {
//			elm.href += "?";
//		}
//		else {
//			elm.href += "&";
//		}
//	
//		var modId = elm.id;
//		var matchVal = "ekmensel_";
//		if (modId.length > matchVal.length) {
//			var idx = modId.indexOf(matchVal);
//			if (idx >= 0) {
//				modId = modId.substr(idx + matchVal.length);
//			}
//		}
//		elm.href += matchVal.substr(0, matchVal.length - 1) + "=" + modId;
// ****************************************************************************		

		if ((this.private_lastSelectedMenuItemObj != null) && (this.private_lastSelectedMenuItemObj != elm)) {
			ekMenuEx.removeClassName(this.private_lastSelectedMenuItemObj, ekMenuEx_classNames.linkSelected);
			ekMenuEx.addClassName(this.private_lastSelectedMenuItemObj, ekMenuEx_classNames.link);
		}
		this.private_lastSelectedMenuItemObj = elm;

		if (!isWrapper) {
			ekMenuEx.removeClassName(elm.className, ekMenuEx_classNames.link);
			ekMenuEx.addClassName(elm.className, ekMenuEx_classNames.linkSelected);
		}

		// now navigate to selected link
		if (elm.target && elm.target.toLowerCase() == "_self") {
            document.location.href = elm.href;
            return false; // onclick handled, ignore href attribute.
		}

		return true; // browser needs to use links' href attribute.
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Disables all event handlers for elements of this menu object:
function __ekMenuEx_disableAllEventHandlers() {
	var elArray = this.getEkMenuElementsByTagName("*");
	for (var idx=0; idx < elArray.length; idx++) {
		this.disableElementEventHandlers(elArray[idx]);
	}
	var el = this.getEkMenuContainerElement();
	if (el)
	{
		this.disableElementEventHandlers(el);
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Disables all event handlers for elements of this menu object:
function __ekMenuEx_disableElementEventHandlers(el) {
	if (el) {
		if (ekMenuEx.isDefinedNotNull(el.onmouseout))
			el.onmouseout = null;

		if (ekMenuEx.isDefinedNotNull(el.onmouseover))
			el.onmouseover = null;

		if (ekMenuEx.isDefinedNotNull(el.onfocus))
			el.onfocus = null;

		if (ekMenuEx.isDefinedNotNull(el.onblur))
			el.onblur = null;

		if (ekMenuEx.isDefinedNotNull(el.onclick))
			el.onclick = null;

		if (ekMenuEx.isDefinedNotNull(el.ondblclick))
			el.ondblclick = null;

		if (ekMenuEx.isDefinedNotNull(el.onkeydown))
			el.onkeydown = null;

		if (ekMenuEx.isDefinedNotNull(el.onkeypress))
			el.onkeypress = null;

		if (ekMenuEx.isDefinedNotNull(el.onkeyup))
			el.onkeyup = null;
	}
}

///////////////////////////////////////////////////////////
// ekMenuEx Instance Member Helper Function.
// Called by page-load initialization code, to initialize this object
// with values passed from the server.
// Parameters: 
//	None.
function __ekMenuEx_initializeWithServerVariables() {
	var baseId = this.hashCode();
	if (baseId && baseId.length) {
		// Obtain the server control property, autoCollapseBranches:
		if ((ekMenuEx.isDefinedNotNull(window.ekMenuEx_autoCollapseBranches))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_autoCollapseBranches[baseId]))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_autoCollapseBranches[baseId].length))
			&& (0 < window.ekMenuEx_autoCollapseBranches[baseId].length)) {
			
			this.private_autoCollapseSubmenus = ("true" == window.ekMenuEx_autoCollapseBranches[baseId]);
		}

		// Obtain the server control property, swRev:
		if ((ekMenuEx.isDefinedNotNull(window.ekMenuEx_swRev))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_swRev[baseId]))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_swRev[baseId].length))
			&& (0 < window.ekMenuEx_swRev[baseId].length)) {
			
			this.private_swRevision = window.ekMenuEx_swRev[baseId];
		}

		// Obtain the server control property, startCollapsed:
		if ((ekMenuEx.isDefinedNotNull(window.ekMenuEx_startCollapsed))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_startCollapsed[baseId]))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_startCollapsed[baseId].length))
			&& (0 < window.ekMenuEx_startCollapsed[baseId].length)) {
			
			this.private_startCollapsed = ("true" == window.ekMenuEx_startCollapsed[baseId]);
		}

		// Obtain the server control property, startWithRootFolderCollapsed:
		if ((ekMenuEx.isDefinedNotNull(window.ekMenuEx_startWithRootFolderCollapsed))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_startWithRootFolderCollapsed[baseId]))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_startWithRootFolderCollapsed[baseId].length))
			&& (0 < window.ekMenuEx_startWithRootFolderCollapsed[baseId].length)) {
			
			this.private_startWithRootFolderCollapsed = ("true" == window.ekMenuEx_startWithRootFolderCollapsed[baseId]);
		}

		// Obtain the hash-code of the server control property, MasterControlId:
		if ((ekMenuEx.isDefinedNotNull(window.ekMenuEx_masterControlIdHash))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_masterControlIdHash[baseId]))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_masterControlIdHash[baseId].length))
			&& (0 < window.ekMenuEx_masterControlIdHash[baseId].length)) {
			
			this.private_masterControlIdHash = window.ekMenuEx_masterControlIdHash[baseId];
		}

		// Obtain the slave/subscriber list:
		if ((ekMenuEx.isDefinedNotNull(window.ekMenuEx_subscriberList))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_subscriberList[baseId]))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_subscriberList[baseId].length))
			&& (0 < window.ekMenuEx_subscriberList[baseId].length)) {
			
			this.private_subscriberList = window.ekMenuEx_subscriberList[baseId];
			
			if (this.private_subscriberList.length > 0) {
				var subList = this.private_subscriberList.split(",");
				if (subList && subList[0]) {
					this.private_slaveControl = subList[0];
					this.private_isMasterControl = true;
				}
			}
		}

		// The server may have passed a submenu id, indicating which one to open initially:
		if ((ekMenuEx.isDefinedNotNull(window.ekMenuEx_startupSubmenuBranchId))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_startupSubmenuBranchId[baseId]))
			&& (ekMenuEx.isDefinedNotNull(window.ekMenuEx_startupSubmenuBranchId[baseId].length))
			&& (0 < window.ekMenuEx_startupSubmenuBranchId[baseId].length)) {
			
			var id = ekMenuEx.parseMenuSubmenuIdString(window.ekMenuEx_startupSubmenuBranchId[baseId]);
			if (ekMenuEx.private_isValidMenuSubmenuIdString(id)) {
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
	}
}

///////////////////////////////////////////////////////////
// Makes the contents of the root-menu visible, selects it's button if it exists.
// Parameters: 
//	None.
function __ekMenuEx_showRootMenu() {
	var rootMenuId = this.buildMenuSubmenuId(0);
	this.selectSubmenu(rootMenuId);
}

///////////////////////////////////////////////////////////
ekMenuEx_loadEventConfigured = false; // global variable for ekMenuEx_addLoadEvent(), to indicate if code has initialized.
///////////////////////////////////////////////////////////
// This funtion is caled by the in-line-code following
// this functions' definition, to ensure that the 
// windows' on-load event is hooked with the ekMenuEx
// initialization code. 
function ekMenuEx_addLoadEvent() 
{
	if (ekMenuEx_loadEventConfigured)
		return;
		
	ekMenuEx_loadEventConfigured = true;
    var oldOnload = window.onload;
    window.onload = function() {
        if ("function" == typeof oldOnload) 
            oldOnload();

        //setTimeout(ekMenuEx.private_startupAllSmartMenus, 100);
        ekMenuEx.private_startupAllSmartMenus();
	}
}
ekMenuEx_addLoadEvent(); // Call the preceeding function to hook the ekMenuEx initialization code.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
ekMenuEx_unloadEventConfigured = false; // global variable for ekMenuEx_addUnLoadEvent(), to indicate if code has initialized.
///////////////////////////////////////////////////////////
// This funtion is caled by the in-line-code following
// this functions' definition, to ensure that the 
// windows' on-unload event is hooked with the ekMenuEx
// cleanup code. 
function ekMenuEx_addUnLoadEvent() 
{
	if (ekMenuEx_unloadEventConfigured)
		return;
		
	ekMenuEx_unloadEventConfigured = true;
    var oldOnunload = window.onunload;
    window.onunload = function() {
        if ("function" == typeof oldOnunload) 
            oldOnunload();

        //setTimeout(ekMenuEx.private_startupAllSmartMenus, 100);
        ekMenuEx.private_shutdownAllSmartMenus();
	}
}
ekMenuEx_addUnLoadEvent(); // Call the preceeding function to hook the ekMenuEx initialization code.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//	Menu element names (prefixed by "ekmengrp_"):
//  Note: these are only rendered if the SmartMenu server controls' renderElementNames  is true (default=false, see Navigation.vb).
//
//		accessible_menu_startheading - H2: Wraps the skip-menu with a navigation-heading (only rendered when 508-Compliance is enabled).
//		accessible_menu_startlink - A: Wraps the skip-menu text with a navigation-link (only rendered when 508-Compliance is enabled).
//		btnlink - A: Wraps each menu button title with a navigation-anchor (only rendered when 508-Compliance is enabled).
//		button - SPAN: Holds the title, and acts as a button (or folder) for the associated submenu.
//		ekmenu - DIV: Wraps the entire menu (the outer-most non-user container element).
//		link - A: A Link for individual menu items (quicklinks, external links, etc.).
//		menu_end - DIV: Wraps the menu-end page-anchor (only rendered when 508-Compliance is enabled).
//		menu_start - DIV: Wraps the menu-start link (only rendered when 508-Compliance is enabled).
//		submenu - DIV: Holds submenu items, such as a submenu title and links.
//		submenu_items - DIV: Container for menu lists.
//		submenu_navheading - H3: Wraps each menu button title with a navigation-heading (only rendered when 508-Compliance is enabled).
//		unorderedlist - UL: A container for menu list items (useful for non-graphical browsers).
//		unorderedlist_item - LI: Container for menu items (typically either links or sub-menus).

///////////////////////////////////////////////////////////////////////////////

var g_DebugWindow=null;
function DebugMsg(Msg) {
    Msg = '>>>' + Msg + ' <br> ';
    if ((g_DebugWindow == null) || (g_DebugWindow.closed)) {
        g_DebugWindow = window.open('Debug Notes', 'myWin', 'toolbar=no, directories=no, location=no, status=yes, menubar=no, resizable=yes, scrollbars=yes, width=500, height=300');
    }
    g_DebugWindow.document.writeln(Msg);
    g_DebugWindow.scrollTo(0,10000000);
}

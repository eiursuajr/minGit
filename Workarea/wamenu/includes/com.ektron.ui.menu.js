//////////
//
// name: Menu
// desc: An object for creating and describing context menus
//

function Menu( name )
{
	//////////
	//
	// public members
	//

	this.addBreak = __Menu_addBreak;
	this.addMenu  = __Menu_addMenu;
	this.addItem  = __Menu_addItem;
	this.display  = __Menu_display;
	this.render   = __Menu_render;
	this.show     = __Menu_show;
	this.hide     = __Menu_hide;
	this.getChildMenus = __Menu_getChildMenus;
	this.hideChildMenus  = __Menu_hideChildMenus;
	this.hasChildMenu  = __Menu_hasChildMenu;
	this.hideDescendentMenus = __Menu_hideDescendentMenus;
	this.parentButtonId;
	
	//////////
	//
	// private members
	//
	
	this.name = name;
	this.data = new Array();
	this.rendered = false;
	this.visible  = false;

	// platform/browser info:
	this.isMac = 0 <= ((navigator.userAgent).toLowerCase()).indexOf('mac');
	this.isSafari = 0 <= ((navigator.userAgent).toLowerCase()).indexOf('safari');
}

//////////
//
// name: hasChildMenu
// desc: Returns whether or not a menu has a submenu
//

function __Menu_hasChildMenu()
{
	var children = this.getChildMenus();
	return children.length == 0 ? false : true;
}

//////////
//
// name: hideChildMenus
// desc: Hides all the currently added children
//

function __Menu_hideChildMenus()
{
	var children = this.getChildMenus();
	for( var i = 0; i < children.length; i++ ) {
		var child = children[i];
		child.hide();
	}
}

//////////
//
// name: hideDescendentMenus
// desc: Recursively closes all submenus
//

function __Menu_hideDescendentMenus( menu )
{
	if( menu == null ) {
		menu = this;
	}
	if( menu.hasChildMenu() ) {
		var children = menu.getChildMenus();
		for( var i = 0; i < children.length; i++ ) {
			var child = children[i];
			menu.hideDescendentMenus( child );
		}
		menu.hideChildMenus();
	}
}

//////////
//
// name: getChildMenus
// desc: Gets all the added child menus
//

function __Menu_getChildMenus()
{
	var children = new Array();
	for( var i = 0; i < this.data.length; i++ ) {
		var menuItem = this.data[i];
		if( menuItem.type == "menu" ) {
			// menuItem.handler is the submenu (Menu object)
			children[children.length] = menuItem.handler;
		}
	}
	
	return children;
}

//////////
//
// name: addBreak
// desc: Adds a break to the Menu
//

function __Menu_addBreak()
{

    var previousElementOutput = this.data.length - 1;
    var currentMenuItemCount = this.data.length;
    this.data[this.data.length] = new MenuItem( null, null, "break" );

    //This checks to make sure that the previous menu item isn't a break also
    if(previousElementOutput > -1 && this.data[previousElementOutput]['type'] == 'break'){
        this.data.pop();
    }

}

//////////
//
// name: addMenu
// desc: Adds a sub menu to the menu.
//

function __Menu_addMenu( name, submenu )
{
	this.data[this.data.length] = new MenuItem( name, submenu, "menu" );
}

//////////
//
// name: addItem
// desc: Adds a MenuItem to the Menu. Hashvars is a map of name
//       value pairs that is scoped to the handler. Since the handler runs in its
//       own scope, it doesn't have access to data outside of it (other
//       than global data). This is a way to give an item access to data.
//

function __Menu_addItem( name, handler, hashvars )
{
	this.data[this.data.length] = new MenuItem( name, handler, "text", hashvars );
}

//////////
//
// name: display
// desc: Displays the Menu; if it is not rendered,
//		 we'll render it. If it is not visible, we'll show it.
//

function __Menu_display( evt, args, isSubMenu )
{
	// hide all open menus, if there are any

//alert(evt + '-' + name + args);

	this.render( args );
	this.show( evt, isSubMenu );
}

//////////
//
// name: show
// desc: Makes the context menu visible
//

function __Menu_show( evt, isSubMenu )
{
	var e = window.event ? window.event : evt;
	var element = document.getElementById( this.name );
	if( element ) {
	    var maxWidth  = document.body.clientWidth;
        var maxHeight = document.body.clientHeight;
        var ctxWidth  = 0;//145; // todo: read this from stylesheet
        var ctxHeight = 0;//145; // todo: read this from stylesheet
	var x;
	var y;
	if( !isSubMenu ) {
	    // figure out the event target
	    var targ;
	    if (e.target) targ = e.target;
	    else if (e.srcElement) targ = e.srcElement;
	    if (targ.nodeType == 3) // defeat Safari bug
		    targ = targ.parentNode;


	    // determine the X & Y coords for the target
	    var target = $ektron(targ);
	    var targetPosition = target.offset();
	    var targetHeight = target.outerHeight();
	    var targX = parseInt(targetPosition.left, 10);
	    var targY = parseInt(targetPosition.top, 10);
	    
	    // position the menu
	    x = targX;
	    y = targY + targetHeight;
	    
	}
	else {
	    // figure out the event target
	    var targ;
	    if (e.target) targ = e.target;
	    else if (e.srcElement) targ = e.srcElement;
	    if (targ.nodeType == 3) // defeat Safari bug
		    targ = targ.parentNode;


		// determine the X & Y coords for the target
	    var target = $ektron(targ);
	    var targetPosition = target.offset();
	    var targetWidth = target.outerWidth();
	    var targX = parseInt(targetPosition.left, 10);
	    var targY = parseInt(targetPosition.top, 10);
	    
	    x = targX + targetWidth;
	    y = targY;


	}

        var openX = 0;
        var openY = 0;

        openX = x;
        openY = y-7;

        try {
	        var childCount = $(e.srcElement).children("span").size();

            if(childCount > 0){
                this.menuButtonOwner = e.srcElement;
            }else{

                //this.menuButtonOwner = e.srcElement.par;
            }

        }
        catch( ex ) {
            alert(ex);
        }
        

		// wait time for submenu to display in ms
        var _waitTime = isSubMenu ? 500 : 0;	
        $ektron(element).css({"top": openY + "px", "left" : openX + "px"});
		this.visible = true;
	
		MenuUtil.subMenuDisplayPid = setInterval ( // future note: maybe change setInterval/clearInterval to setTimeout/clearTimeout...
			function() {
				element.style["display"] = "";
			},
			_waitTime
		);
	}
}

//////////
//
// name: hide
// desc: Makes the context menu hidden
//

function __Menu_hide()
{
	var element = document.getElementById( this.name );
	if( element != null ) {
		element.style["display"] = "none";
		element.style["visibility"] = "hidden";
		this.visible = false;
	}
}

//////////
//
// name: render
// desc: Inserts the context menu into the document object. The 'args' parameter
//		 is passed along when the context menu is used, for example:
//		 MenuUtil.use( "myMenu", "myArgumentValue" ); 
//		 

function __Menu_render( args )
{
	// Since we're passing arguments to the Menu's onclick handler
	// we need to create a new context menu each time. So, if we've already
	// rendered a context menu, we'll remove it and re-render it using new
	// args data
	var menu = document.getElementById( this.name );
	if( menu ) {
		document.body.removeChild( menu );
	}

	menu = document.createElement( "div" );
	menu.style.position = "absolute";
	menu.id = this.name;	
	menu.className = "Menu";
	
	for( var i = 0; i < this.data.length; i++ ) {
		var item = this.data[i];
		var itemElement = document.createElement( "div" );
		var itemIcon = document.createElement( "div" );
		switch( item.type ) {
			case "break":
				itemElement.className = "MenuItemBreak";
			break;
			case "menu":
				// handler in this case is our Menu object
				var submenu = item.handler;
				MenuUtil.add( submenu );
				itemElement.name = submenu.name;
				itemElement.className = "MenuItemSubMenu";
				//var arrow = "&nbsp;&nbsp;<span style='font-family:webdings;font-size:12px;'>&#52;</span>";
				//var arrow = "&nbsp;&nbsp;&nbsp;&nbsp;<img src=\"images/application/menu/arrow.gif\" />";
				var arrow = "";
				itemElement.innerHTML = item.name + arrow;
				itemElement.args = args;
				itemElement.onclick = onClickMenu;
				itemElement.onmouseover = onMouseoverSubMenu;
				itemElement.onmouseout = onMouseoutSubMenu;
			break;
			default:
			    itemIcon.className = "MenuIcon";
			    itemIcon.innerHTML = "<img src=\"images/application/btn_add-nm.gif\" width=\"22\" height=\"22\" align=\"left\" />";
			    itemIcon.id   = "icon" + i;
				itemElement.className = "MenuItem";
				itemElement.innerHTML = item.name;
				itemElement.name = this.name;
				itemElement.id   = "" + i;
				itemElement.args = args;
				itemElement.onclick = onClickDefault;
				itemElement.onmouseover = onMouseoverDefault;
				itemElement.onmouseout = onMouseoutDefault;
			break;
		}
		menu.onmouseover = onMouseoverNone;
		//menu.appendChild( itemIcon );
		menu.appendChild( itemElement );
	}

	document.body.appendChild( menu );
}

function onClickMenu()
{
	event.cancelBubble = true;
}
 
function onMouseoverMenu()
{
	this.className = "MenuItemOver";
	MenuUtil.use( event, this.name, this.args, true );
}

function onMouseoverSubMenu(evt)
{
    var e = window.event ? window.event : evt;
    try {
	    this.className = "MenuItemSubMenuOver";
	    MenuUtil.use( e, this.name, this.args, true );
	}
	catch( ex ) {
	    alert(ex);
	}
}

function onMouseoutMenu()
{
	this.className = "MenuItemOut";
	// if a subMenu display is pending, clear it
	if( MenuUtil.subMenuDisplayPid != null ) {
		clearInterval( MenuUtil.subMenuDisplayPid );
	}
}

function onMouseoutSubMenu()
{
	this.className = "MenuItemSubMenuOut";
	// if a subMenu display is pending, clear it
	if( MenuUtil.subMenuDisplayPid != null ) {
		clearInterval( MenuUtil.subMenuDisplayPid );
	}
}

function onClickDefault()
{
	var menu = MenuUtil.get( this.name );
	if( menu ) {
		var item = menu.data[this.id]
		item.handler(this.args, item.vars);
	}
}

function onMouseoverDefault()
{
	this.className = "MenuItemOver"; 
}

function onMouseoutDefault()
{
	this.className = "MenuItemOut"; 
}


function onMouseoverNone()
{
	var name = this.id;
	var menu = MenuUtil.get( name );
	menu.hideDescendentMenus();
}


//////////
//
// name: MenuItem
// desc: Simple class describing an entry in the context menu
//

function MenuItem( name, handler, type, hashvars )
{
	this.name = name;
	this.handler = handler;
	this.type = type ? type : "text";
	this.vars = hashvars;
}

//////////
//
// name: MenuUtil
// desc: Static utility class for manipulating the Menu
//
//

var MenuUtil =
{
	//////////
	//
	// name: enableDefaultMenu
	// desc: Enables or disables the default context menu for a given element.
	//
	enableDefaultMenu: function (evt, enabled) {
		disabled = enabled ? true : false;
		evt.cancelBubble = enabled;
		return enabled;
	},

	//////////
	//
	// name: hideMenuByName
	// desc: hides context menus with given name
	//
	hideMenuByName: function (name) {
		var menu = MenuUtil.data[name];

		if (menu) {
			menu.hide();
		}
	},

	//////////
	//
	// name: hide
	// desc: hides all registered context menus
	//
	hide: function () {
		if (typeof MenuUtil != 'undefined') {
			for (var name in MenuUtil.data) {

				try {
					if (MenuUtil.data[name].parentButtonId != null) {

						var parentButton = document.getElementById(MenuUtil.data[name].parentButtonId);

						if (parentButton != 'undefined') {
							var index = parentButton.className.indexOf(" active-button-menu");
							if (index != -1) {
								parentButton.className = parentButton.className.substring(0, index);
							}
						}
					}
				}
				catch (ex) {
					alert(ex);
				}

				MenuUtil.data[name].hide();
			}
		}
	},

	//////////
	//
	// name: add
	// desc: A method for registering a Menu with the Menuutil.
	//		 If an element wants to associate a Menu with it, it need
	//		 only create the Menu, add it here, then reference it using
	//		 onMenu='MenuUtil.use("name")', e.g.:
	//
	//		 // create the context menu
	//		 var menu = new Menu( "myMenu" );
	//		 menu.addItem( "Hello", function() { alert( "hello" ) } );
	//
	//		 // register it with the util
	//		 MenuUtil.add( menu );
	//
	//       // use it wherever you want by name "hello"
	//		 This is a <span onMenu="MenuUtil.use('myMenu')">Hello World</span> example
	//
	add: function (menu) {
		MenuUtil.data[menu.name] = menu;
	},

	//////////
	//
	// name: get
	// desc: Gets context menu by name from global registry
	//

	get: function (name) {
		return MenuUtil.data[name];
	},

	//////////
	//
	// name: use
	// desc: A method for binding a context menu with a document element. Example usage:
	//       // use it wherever you want by name "hello"
	//		 This is a <span onMenu="MenuUtil.use('hello')">Hello World</span> example
	//

	use: function (evt, name, args, isSubMenu) {
		// Should we hide other open context menus? In the
		// case of opening submenus, we don't want to hide
		// previous conetxt menus.
		if (!isSubMenu) {
			MenuUtil.hide();
		}

		var menu = MenuUtil.data[name];
		if (menu) {

			if (args != null) {
				menu.parentButtonId = args;

				try {
					if (MenuUtil.data[name].parentButtonId != null) {

						var parentButton = document.getElementById(MenuUtil.data[name].parentButtonId);

						if (parentButton != 'undefined') {
							if (parentButton.className.indexOf("active-button-menu") == -1) {
								parentButton.className += " active-button-menu";
							}
						}
					}
				}
				catch (ex) {
					alert(ex);
				}
			}

			menu.display(evt, args, isSubMenu);
		}

		evt.cancelBubble = true;
		return false;
	},

	//////////
	//
	// name: copy
	// desc: Gets a copy of the Menu named 'name'
	//

	copy: function (name) {
		var menu = MenuUtil.data[name];
		var copy = new Menu();
		if (menu) {
			for (var i = 0; i < menu.data.length; i++) {
				var item = menu.data[i];
				copy.addItem(item.name, item.handler);
			}
		}
		return copy;
	},

	data: new Array(),
	subMenuDisplayPid: null
};

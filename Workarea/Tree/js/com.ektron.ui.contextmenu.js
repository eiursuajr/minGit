//////////
//
// name: ContextMenu
// desc: An object for creating and describing context menus
//

function ContextMenu( name )
{
	//////////
	//
	// public members
	//

	this.addBreak = __ContextMenu_addBreak;
	this.addMenu  = __ContextMenu_addMenu;
	this.addItem  = __ContextMenu_addItem;
	this.display  = __ContextMenu_display;
	this.render   = __ContextMenu_render;
	this.show     = __ContextMenu_show;
	this.hide     = __ContextMenu_hide;
	this.getChildMenus = __ContextMenu_getChildMenus;
	this.hideChildMenus  = __ContextMenu_hideChildMenus;
	this.hasChildMenu  = __ContextMenu_hasChildMenu;
	this.hideDescendentMenus = __ContextMenu_hideDescendentMenus;
	
	//////////
	//
	// private members
	//
	
	this.name = name;
	this.data = new Array();
	this.rendered = false;
	this.visible  = false;
}

//////////
//
// name: hasChildMenu
// desc: Returns whether or not a menu has a submenu
//

function __ContextMenu_hasChildMenu()
{
	var children = this.getChildMenus();
	return children.length == 0 ? false : true;
}

//////////
//
// name: hideChildMenus
// desc: Hides all the currently added children
//

function __ContextMenu_hideChildMenus()
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

function __ContextMenu_hideDescendentMenus( menu )
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

function __ContextMenu_getChildMenus()
{
	var children = new Array();
	for( var i = 0; i < this.data.length; i++ ) {
		var menuItem = this.data[i];
		if( menuItem.type == "menu" ) {
			// menuItem.handler is the submenu (contextMenu object)
			children[children.length] = menuItem.handler;
		}
	}
	
	return children;
}

//////////
//
// name: addBreak
// desc: Adds a break to the contextMenu
//

function __ContextMenu_addBreak()
{
	this.data[this.data.length] = new ContextMenuItem( null, null, "break" );
}

//////////
//
// name: addMenu
// desc: Adds a sub menu to the menu.
//

function __ContextMenu_addMenu( name, submenu )
{
	this.data[this.data.length] = new ContextMenuItem( name, submenu, "menu" );
}

//////////
//
// name: addItem
// desc: Adds a contextMenuItem to the contextMenu. Hashvars is a map of name
//       value pairs that is scoped to the handler. Since the handler runs in its
//       own scope, it doesn't have access to data outside of it (other
//       than global data). This is a way to give an item access to data.
//

function __ContextMenu_addItem( name, handler, hashvars )
{
	this.data[this.data.length] = new ContextMenuItem( name, handler, "text", hashvars );
}

//////////
//
// name: display
// desc: Displays the contextMenu; if it is not rendered,
//		 we'll render it. If it is not visible, we'll show it.
//

function __ContextMenu_display( args, isSubMenu )
{
	// hide all open menus, if there are any
	this.render( args );
	this.show( event, isSubMenu );
}

//////////
//
// name: show
// desc: Makes the context menu visible
//

function __ContextMenu_show( event, isSubMenu )
{
	var element = document.getElementById( this.name );
	if( element ) {
	    var maxWidth  = document.body.clientWidth;
        var maxHeight = document.body.clientHeight;
        var ctxWidth  = 145; // todo: read this from stylesheet
        var ctxHeight = 145; // todo: read this from stylesheet
        var x = event.clientX;
        var y = event.clientY;
        var openX = 0;
        var openY = 0;

		// if its a submenu, bump it over a little
		x += isSubMenu ? 40 : 0;

		// open to the left of, or the right of, the cursor?
		if( ( x + ctxWidth - 10 ) > maxWidth ) {
			openX = x - ctxWidth - 5;
		} else {
			openX = x;
		}

		// open to the left of, or the right of, the cursor?
		if( ( y + ctxHeight - 10 ) > maxHeight ) {
			openY = y - ctxHeight + 55;
		} else {
			openY = y;
		}

		// wait time for submenu to display in ms
		var _waitTime = isSubMenu ? 500 : 0;
		element.style["display"] = "none";
		element.style["left"]  = openX;
		element.style["top"] = openY;
		this.visible = true;
		
		ContextMenuUtil.subMenuDisplayPid = setInterval (
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

function __ContextMenu_hide()
{
	var element = document.getElementById( this.name );
	if( element ) {
		element.style["display"] = "none";
		this.visible = false;
	}
}

//////////
//
// name: render
// desc: Inserts the context menu into the document object. The 'args' parameter
//		 is passed along when the context menu is used, for example:
//		 ContextMenuUtil.use( "myMenu", "myArgumentValue" ); 
//		 

function __ContextMenu_render( args )
{
	// Since we're passing arguments to the contextMenu's onclick handler
	// we need to create a new context menu each time. So, if we've already
	// rendered a context menu, we'll remove it and re-render it using new
	// args data
	var menu = document.getElementById( this.name );
	if( menu ) {
		document.body.removeChild( menu );
	}

	menu = document.createElement( "div" );
	menu.id = this.name;
	menu.className = "contextMenu";
	
	for( var i = 0; i < this.data.length; i++ ) {
		var item = this.data[i];
		var itemElement = document.createElement( "div" );
		switch( item.type ) {
			case "break":
				itemElement.className = "contextMenuItemBreak";
			break;
			case "menu":
				// handler in this case is our ContextMenu object
				var submenu = item.handler;
				ContextMenuUtil.add( submenu );
				itemElement.name = submenu.name;
				itemElement.className = "contextMenuItemSubMenu";
				var arrow = "&nbsp;&nbsp;<span style='font-family:webdings;font-size:12px;'>&#52;</span>";
				itemElement.innerHTML = item.name + arrow;
				itemElement.onclick =
					function()
					{
						event.cancelBubble = true;
					} 
				itemElement.onmouseover =
					function()
					{
						this.className = "contextMenuItemOver";
						ContextMenuUtil.use( event, this.name, args, true );
					};
				itemElement.onmouseout =
					function()
					{
						this.className = "contextMenuItemOut";
						// if a subcontextmenu display is pending, clear it
						if( ContextMenuUtil.subMenuDisplayPid != null ) {
							clearInterval( ContextMenuUtil.subMenuDisplayPid );
						}
					};
			break;
			default:
				itemElement.className = "contextMenuItem";
				itemElement.innerHTML = item.name;
				itemElement.name = this.name;
				itemElement.id   = "" + i;
				itemElement.onclick =
					function()
					{
						var menu = ContextMenuUtil.get( this.name );
						if( menu ) {
							var item = menu.data[this.id]
							item.handler(args, item.vars);
						}
					}
				itemElement.onmouseover = function(){this.className = "contextMenuItemOver"; };
				itemElement.onmouseout = function(){this.className = "contextMenuItemOut"; };
			break;
		}
		menu.onmouseover =
			function()
			{
				var name = this.id;
				var menu = ContextMenuUtil.get( name );
				menu.hideDescendentMenus();
			}
		menu.appendChild( itemElement );
	}

	document.body.appendChild( menu );
}

//////////
//
// name: ContextMenuItem
// desc: Simple class describing an entry in the context menu
//

function ContextMenuItem( name, handler, type, hashvars )
{
	this.name = name;
	this.handler = handler;
	this.type = type ? type : "text";
	this.vars = hashvars;
}

//////////
//
// name: ContextMenuUtil
// desc: Static utility class for manipulating the contextmenu
//
//

var ContextMenuUtil =
{
	//////////
	//
	// name: enableDefaultMenu
	// desc: Enables or disables the default context menu for a given element.
	//
	enableDefaultMenu: function( event, enabled )
	{
		disabled = enabled ? true : false;
		event.cancelBubble = enabled;
		return enabled;
	},

	//////////
	//
	// name: hideMenuByName
	// desc: hides context menus with given name
	//
	hideMenuByName: function( name )
	{
		var menu = ContextMenuUtil.data[name];

		if( menu ) {
			menu.hide();
		}
	},
	
	//////////
	//
	// name: hide
	// desc: hides all registered context menus
	//
	hide: function()
	{
		for( var name in ContextMenuUtil.data ) {
			ContextMenuUtil.data[name].hide();
		}
	},

	//////////
	//
	// name: add
	// desc: A method for registering a contextmenu with the contextmenuutil.
	//		 If an element wants to associate a contextmenu with it, it need
	//		 only create the contextmenu, add it here, then reference it using
	//		 oncontextmenu='ContextMenuUtil.use("name")', e.g.:
	//
	//		 // create the context menu
	//		 var menu = new ContextMenu( "myMenu" );
	//		 menu.addItem( "Hello", function() { alert( "hello" ) } );
	//
	//		 // register it with the util
	//		 ContextMenuUtil.add( menu );
	//
	//       // use it wherever you want by name "hello"
	//		 This is a <span oncontextmenu="ContextMenuUtil.use('myMenu')">Hello World</span> example
	//
	add: function( menu )
	{
		ContextMenuUtil.data[menu.name] = menu;
	},
	
	//////////
	//
	// name: get
	// desc: Gets context menu by name from global registry
	//
	
	get: function( name )
	{
		return ContextMenuUtil.data[name];
	},
	
	//////////
	//
	// name: use
	// desc: A method for binding a context menu with a document element. Example usage:
	//       // use it wherever you want by name "hello"
	//		 This is a <span oncontextmenu="ContextMenuUtil.use('hello')">Hello World</span> example
	//
	
	use: function( event, name, args, isSubMenu )
	{	
		// Should we hide other open context menus? In the
		// case of opening submenus, we don't want to hide
		// previous conetxt menus.
		if( ! isSubMenu ) {
			ContextMenuUtil.hide();
		}
		
		var menu = ContextMenuUtil.data[name];
		if( menu ) {
			menu.display( args, isSubMenu );
		}
		event.cancelBubble = true;
		return false;
	},

	//////////
	//
	// name: copy
	// desc: Gets a copy of the contextmenu named 'name'
	//
	
	copy: function( name )
	{
		var menu = ContextMenuUtil.data[name];
		var copy = new ContextMenu();
		if( menu ) {
			for( var i = 0; i < menu.data.length; i++ ) {
				var item = menu.data[i];
				copy.addItem( item.name, item.handler );
			}
		}
		return copy;
	},

	data: new Array(),
	subMenuDisplayPid: null
}

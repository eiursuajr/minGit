//////////
//
// name: Tree
// desc: General purpose tree data structure
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function Tree( text, id, pid, data)
{
	this.node       = new Node( text, id, pid, data );
	this.children   = new Array();

	this.appendNode = function( node ) {
		this.children[this.children.length] = node;
	}

	// keep around reference
	TREES[TreeUtil.getTreeId(id)] = this;
}

//////////
//
// name: Node
// desc: Node element of the tree
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function Node( n, id, pid, data )
{
	this.name      = n;
	this.action    = onToggleClick;
	this.iconOp    = null;
	this.iconCl	   = null;
	this.id		   = id;
	this.pid	   = pid;
	this.data	   = data;
	this.status    = "closed";
	this.renderedSelf	  = false;
	this.renderedChildren = false;

	this.setAction    = function( actn ) { this.action = actn; }
	this.setOpenIcon  = function( icon ) { this.iconOp = icon; }
	this.setCloseIcon = function( icon ) { this.iconCl = icon; }
}

var TREES = new Array();

//////////
//
// name: TreeDisplayUtil
// desc: A static utility class for displaying a Tree object
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

var TreeDisplayUtil =
{
//////////
	//
	// name: getAncestry
	// desc: Given an id, this function returns the ancestry for
	//       the node, starting the node itself, tracing back
	//       to the root node. It returns an array of tree nodes.
	//

	getAncestry: function( id, ancestry )
	{
		ancestry = ancestry != null ? ancestry : [];
		var tree = TreeUtil.getTreeById( id );

		if( tree != null ) {
			ancestry[ancestry.length] = tree;
			var id = tree.node.id;
			var pid = TreeDisplayUtil.getParentId( id );
			if( pid != -1 ) {
				return TreeDisplayUtil.getAncestry( pid, ancestry );
			} else {
				return ancestry;
			}
		}

		return null;
	},

	//////////
	//
	// name: getRootNode
	// desc: Given an id, this function returns its root. If the passed
	//       id represents a tree not seen, the function returns null.
	//       If it represents the root node, the function returns its id.
	//       Otherwise, it recurses until it finds the root.
	//

	getRootNodeId: function( id )
	{
		var ancestry = TreeDisplayUtil.getAncestry( id );
		var rootNodeId = null;

		if( ancestry != null ) {
			if( ancestry.length > 0 ) {
				var root = ancestry[ancestry.length - 1];
				rootNodeId = root.node.id;
			}
		}

		return rootNodeId;
	},

	//////////
	//
	// name: getParentId
	// desc: Given an id, this function returns its parent id. If the
	//       passed id represents a tree not seen, the function returns
	//       null. If the id represents the root node (who doesn't have
	//       a parent) the function returns -1. Otherwise, it returns
	//       the parent id.
	//

	getParentId: function( id )
	{
		var tree = TreeUtil.getTreeById( id );
		var pid = null;

		if( tree != null ) {
			pid = -1;
			if( tree.node.pid != null ) {
				pid = tree.node.pid;
			}
		}

		return pid;
	},

	//////////
	//
	// name: getElementById
	// desc: Given an id, returns the document element for that item
	//

	getElementById: function( id )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getElementById(" + id + ")",
				"starting..." );

		var element = document.getElementById(
						TreeUtil.getTreeId( id ) );

		return element;
	},

	//////////
	//
	// name: reloadTree
	// desc: Given an id, this function forces a reload of a subtree
	//
	reloadTree: function( id )
	{
		var tree = TreeUtil.getTreeById( id );

		if( tree != null ) {
			if( tree.node.renderedChildren != true ) {
				TreeDisplayUtil.expandTree( tree );
			} else {
				tree.node.renderedChildren = false;
				var containerId = TreeUtil.getChildId( id );
				var containerElement = document.getElementById( containerId );
				containerElement.parentNode.removeChild( containerElement );

				//alert(TreeUtil.getTreeId(tree.node.id));
				TREES[TreeUtil.getTreeId(tree.node.id)].children = [];
				TreeDisplayUtil.expandTree( tree );
			}
		}
	},

	//////////
	//
	// name: removeTree
	// desc: Given an id, this function removes a subtree from the display
	//

	removeTree: function( id )
	{
		var tree = TreeUtil.getTreeById( id );

		if( tree != null ) {
			// Set the renderedChildren flag to false
			tree.node.renderedChildren = false;

			// Remove the children from the DOM
			var containerId = TreeUtil.getChildId( id );
			var containerElement = document.getElementById( containerId );
			containerElement.parentNode.removeChild( containerElement );

			// This is where tree data is cached. Remove the child data from it.
			TREES[TreeUtil.getTreeId(tree.node.id)].children = [];
		}
	},

	//////////
	//
	// name: reloadParentTree
	// desc: Given an id, this function forces the load of a parent subtree
	//

	reloadParentTree: function( id )
	{
		var tree = TreeUtil.getTreeById( id );

		if( tree != null ) {
			var pid = tree.node.pid;
			TreeDisplayUtil.reloadTree(pid);
		}
	},

	getChildElementById: function( id )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getChildElementById(" + id + ")",
				"starting..." );

		var element = document.getElementById(
						TreeUtil.getChildId( id ) );

		return element;
	},

	//////////
	//
	// name: toggleTree
	// desc: If a node is open, close it. Otherwise, open it.
	//
	toggleTree: function( pid )
	{
		var tree = TreeUtil.getTreeById( pid );

		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"toggleTree(" + pid + ")",
				"tree.node.status = " + tree.node.status );

		if( tree.node.status == "open" ) {
			TreeDisplayUtil.collapseTree( tree );
		} else {
			TreeDisplayUtil.expandTree( tree );
		}
	},
		//////////
	//
	// name: expandTreeSet
	// desc: expandTreeSet accepts a set of ids and asynchronously & recursively
	//       expands them. The ids must be formatted in the following way:
	//       ids[0]   -> root
	//       its[1]   -> first-level
	//        ...
	//       ids[n-3] -> grandparent
	//       ids[n-2] -> parent
	//       ids[n-1] -> self
	//
	//       e.g.:
	//
	//       TreeDisplayUtil.expandTreeSet( [0, 23, 45, 47, 76] );
	//       TreeDisplayUtil.expandTreeSet( new Array( "0, 23, 45, 47, 76" ) );
	//

	expandTreeSet: function( ids )
	{
		return TreeDisplayUtil.__expandTreeSet( [ids] );
	},
		//////////
	//
	// name: __expandTreeSet
	// desc: __expandTreeSet is a private method that does the work for
	//		 expandTreeSet. This function handles the asynchronous calls
	//       and the oncomplete event handling. Upon oncomplete, it recursively
	//       calls itself.
	//

	__expandTreeSet: function( vargs )
	{
		var retVal = false;
		var ids = [];
		if( vargs.length > 0 ) {
			ids = vargs[0];
		}

		var currentIndex = 0;
		if( vargs.length > 1 ) {
			currentIndex = vargs[1];
			currentIndex++;
		}

		onComplete	 = TreeDisplayUtil.expandTreeSet;
		if( ids.length > 0 ) {
			var id = ids[currentIndex];
			if( id != null ) {
				var vargs = [ids, currentIndex, onComplete];
				var tree = TreeUtil.getTreeById( id );
				if( tree != null && tree.node.renderedChildren == true ) {
					retVal = TreeDisplayUtil.showChildren( tree );
					if (retVal)
					{
						retVal = TreeDisplayUtil.__expandTreeSet( vargs );
					}
				} else {
					retVal = TreeDisplayUtil.expandTreeById( id, TreeDisplayUtil.__expandTreeSet, vargs );
				}
			}
		}
		return (retVal);
	},
	//////////
	//
	// name: toggleTree
	// desc: If a node hasn't been opened before, fetch data and display it
	//

	expandTree: function( tree )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTree(" + tree.node.id + ")",
				"starting..." );

		// the "tree.node.id == 0" is a workaround for a bug on in the ContentService
		// that has the root element returning hasChildren = false.
		if( ( tree.node.data.hasChildren == "true" ) || ( tree.node.id == 0 ) ) {
			if( ! tree.node.renderedChildren ) {
				tree.node.renderedChildren = true;
				TreeUtil.addChildren( tree.node.id );
			} else {
				TreeDisplayUtil.showChildren( tree );
			}
		}
	},

	expandTreeEx: function( tree, onComplete, vargs )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeEx(" + tree.node.id + ")",
				"starting..." );

		// the "tree.node.id == 0" is a workaround for a bug on in the ContentService
		// that has the root element returning hasChildren = false.
		if( ( tree.node.data.hasChildren == "true" ) || ( tree.node.id == 0 ) ) {
			if( ! tree.node.renderedChildren ) {
				tree.node.renderedChildren = true;
				TreeUtil.addChildrenEx( tree.node.id, onComplete, vargs );
			} else {
				TreeDisplayUtil.showChildren( tree );
			}
		}
	},

	expandTreeById: function( id )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeById(" + id + ")",
				"starting..." );

		var tree = TreeUtil.getTreeById( id );
		TreeDisplayUtil.expandTree( tree );
	},

	//////////
	//
	// name: expandTreeById
	// desc: Wrapper around expandTree, which expects a tree object
	//

	expandTreeById: function( id, onComplete, vargs )
	{
		var retVal = false;
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeById(" + id + ")",
				"starting..." );

		var tree = TreeUtil.getTreeById( id );
		if (tree)
		{
			TreeDisplayUtil.expandTreeEx( tree, onComplete, vargs );
			retVal = true;
		}
		else
		{
			retVal = false;
		}
		return (retVal);
	},

	//////////
	//
	// name: collapseTree
	// desc: hides a subtree
	//
	collapseTree: function( tree )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"collapseTree(" + tree.node.id + ")",
				"starting..." );

		if( tree.node.data.hasChildren ) {
			TreeDisplayUtil.hideChildren( tree );
		}
	},

	hideChildren: function( tree )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"hideChildren(" + tree.node.id + ")",
				"starting..." );

		var element = TreeDisplayUtil.getChildElementById( tree.node.id );
		element.style["display"] = "none";
		tree.node.status = "closed";

		var iconElement  = document.getElementById( "I" + tree.node.id );
		if( iconElement ) {
		    if (TreeDisplayUtil.plusclosefolders == undefined) {
			iconElement.src = TreeDisplayUtil.plusclosefolder;
			} else {
			    iconElement.src = TreeDisplayUtil.plusclosefolders[tree.node.data.type];
			}
		}
	},

	showChildren: function( tree )
	{
		var retVal = false;
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showChildren(" + tree.node.id + ")",
				"starting..." );

		var childElement = TreeDisplayUtil.getChildElementById( tree.node.id );
		var iconElement  = document.getElementById( "I" + tree.node.id );

		if( !childElement ) {
			retVal = false;
			LogUtil.addMessage( LogUtil.CRITICAL,
					"TreeDisplayUtil",
					"showChildren(" + tree.node.id + ")",
					"Couldn't find container for child element" );
		} else {
			childElement.style["display"] = "";

			if( iconElement ) {
		        if (TreeDisplayUtil.minusclosefolders == undefined) {
			        iconElement.src = TreeDisplayUtil.minusclosefolder;
			    } else {
			        iconElement.src = TreeDisplayUtil.minusclosefolders[tree.node.data.type];
			    }
			}

			tree.node.status = "open";
			retVal = true;
		}
		return (retVal);
	},

	showSelf: function( tree, container )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showSelf(" + tree.node.id + ")",
				"starting..." );

		if( ! tree.node.rendered ) {
			TreeDisplayUtil.renderSelf( tree, container );
		}

		var element = TreeDisplayUtil.getElementById( tree.node.id );
		element.style["display"] = "";
		tree.node.status = "closed";
	},

	//////////
	//
	// name: renderChildren
	// desc: Given a tree, iterate through children and build up display
	//

	renderChildren: function( tree )
	{
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderChildren(" + tree.node.id + ")",
				"starting..." );

		var childContainerId = TreeUtil.getChildId( tree.node.id );
		var ul = document.createElement( "ul" );
		ul.setAttribute( "className", "ekSubTree" );
		ul.setAttribute( "id", childContainerId );
		//Remove-this-line? ul.style.listStyleType = "none"; // can't rely on .css for this because it is not initially applied

		var numberChildren = tree.children.length;
		for( var i = 0; i < numberChildren; i++ )
		{
			var child = tree.children[i];

			var li = document.createElement( "li" );
			li.setAttribute( "className", "ekTreeItem" );
			li.setAttribute( "id", TreeUtil.getTreeId(child.node.id) );
			li.innerHTML = TreeDisplayUtil.createNodeHTML( child );
			ul.appendChild( li );
			child.node.rendered = true;
		}
		tree.node.renderedChildren = true;

		var parentElement = document.getElementById( TreeUtil.getTreeId(tree.node.id) );
		if( parentElement != null ) {
			parentElement.appendChild( ul );
		}
	},

	displayContents: function( id, clickedElement )
	{
		return onFolderClick( id, clickedElement );
	},

	//////////
	//
	// name: renderSelf
	// desc: Given a tree, display it.
	//

	renderSelf: function( tree, container )
	{
		var src = "";
		if( tree.node.data.hasChildren == "true" ) {
		    if (TreeDisplayUtil.plusclosefolders == undefined) {
			    src = TreeDisplayUtil.plusclosefolder;
			} else {
			    src = TreeDisplayUtil.plusclosefolders[tree.node.data.type];
			}
		} else {
	        if (TreeDisplayUtil.folders == undefined) {
		        src = TreeDisplayUtil.folder;
		    } else {
		        src = TreeDisplayUtil.folders[tree.node.data.type];
		    }
		}
		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderSelf(" + tree.node.id + ")",
				"starting..." );

		var ul = document.createElement( "ul" );
		ul.setAttribute( "className", "ektree" );
		//Remove-this-line? ul.style.listStyleType = "none"; // can't rely on .css for this because it is not initially applied

		var li = document.createElement( "li" );
		li.setAttribute( "className", "ekTreeRootItem" );
		li.setAttribute( "id", TreeUtil.getTreeId(tree.node.id) );

		var type = tree.node.data.itemType;
		var strClass="";
		var strOnMouseOver="";
		if ((type == "content") || (type == "ExternalLink") || (type == "Library"))
		{
			src = URLUtil.getAppRoot(document.location) + "images/UI/Icons/menuItem.png";
			strClass="";
			strOnMouseOver=" onmouseover='showWammFloatMenuForMenuItemNode(true, false, event, " + tree.node.id + ");' ";
			strOnMouseOut=" onmouseout='showWammFloatMenuForMenuItemNode(false, true, event, " + tree.node.id + ");' ";
		}
		else
		{
			strClass="class='hasChildIcon'";
			strOnMouseOver=" onmouseover='showWammFloatMenuForMenuNode(true, false, event, " + tree.node.id + ");' ";
			strOnMouseOut=" onmouseout='showWammFloatMenuForMenuNode(false, true, event, " + tree.node.id + ");' ";
		}

		var buffer = "<img treeid='" + tree.node.id + "' onclick='TreeDisplayUtil.toggleTree(" + tree.node.id + ")' " + strClass + " " + strOnMouseOut + " id='I" + tree.node.id + "' src='" + src + "'>";
		buffer += "<span treeid='" + tree.node.id + "' " + strOnMouseOver + strOnMouseOut + " onclick='TreeDisplayUtil.expandTreeById(" + tree.node.id + ")'>";

		// If this object is not a menu or submenu, then it must be handled differently:
        if ((tree.node.data && tree.node.data.itemType && ('Menu' == tree.node.data.itemType)) || (tree.node.data && tree.node.data.itemType && ('Submenu' == tree.node.data.itemType))) {
			buffer += "<a href='#' onclick='TreeDisplayUtil.displayContents(" + tree.node.id + ", this);return false;' id='" + tree.node.id + "'>" + tree.node.name + "</a>";
		}
		else {
			if ("function" == typeof routeAction)
				buffer += "<a href='#' onclick='routeAction(false, \"Edit\");return false;' id='" + tree.node.id + "'>" + tree.node.name + "</a>";
			else
				buffer += tree.node.name;
        }
        buffer += "</span>";

		li.innerHTML = buffer
		ul.appendChild( li );

		if( !container ) {
			container = document.getElementById( "TreeOutput" );
		}
		if (ul.outerHTML == 'undefined') {
		    container.innerHTML = ul.outerHTML;
		} else {
		    container.innerHTML = '<UL class=ektree>' + ul.innerHTML + '</UL>';
		}

		tree.node.rendered = true;
	},

	createNodeHTML: function( tree )
	{
		var src = "";
		if( tree.node.data.hasChildren == "true" ) {
		    if (TreeDisplayUtil.plusclosefolders == undefined) {
			    src = TreeDisplayUtil.plusclosefolder;
			} else {
			    src = TreeDisplayUtil.plusclosefolders[tree.node.data.type];
			}
		} else {
	        if (TreeDisplayUtil.folders == undefined) {
		        src = TreeDisplayUtil.folder;
		    } else {
		        src = TreeDisplayUtil.folders[tree.node.data.type];
		    }
		}

		var type = tree.node.data.itemType;
		var strClass="";
		var strOnMouseOver="";
		if ((type == "content") || (type == "ExternalLink") || (type == "Library"))
		{
		    src = URLUtil.getAppRoot(document.location) + "images/UI/Icons/menuItem.png";
			strClass="";
			strOnMouseOver=" onmouseover='showWammFloatMenuForMenuItemNode(true, false, event, " + tree.node.id + ");' ";
			strOnMouseOut=" onmouseout='showWammFloatMenuForMenuItemNode(false, true, event, " + tree.node.id + ");' ";
		}
		else
		{
			strClass="class='hasChildIcon'";
			strOnMouseOver=" onmouseover='showWammFloatMenuForMenuNode(true, false, event, " + tree.node.id + ");' ";
			strOnMouseOut=" onmouseout='showWammFloatMenuForMenuNode(false, true, event, " + tree.node.id + ");' ";
		}

		var buffer = "<img treeid='" + tree.node.id + "' onclick='TreeDisplayUtil.toggleTree(" + tree.node.id + ")' " + strClass + " " + strOnMouseOut + " id='I" + tree.node.id + "' src='" + src + "'>";
		buffer += "<span treeid='" + tree.node.id + "' " + strOnMouseOver + strOnMouseOut + " onclick='TreeDisplayUtil.expandTreeById(" + tree.node.id + ")'>";

		// If this object is not a menu or submenu, then it must be handled differently:
        if ((tree.node.data && tree.node.data.itemType && ('Menu' == tree.node.data.itemType)) || (tree.node.data && tree.node.data.itemType && ('Submenu' == tree.node.data.itemType))) {
			buffer += "<a href='#' onclick='TreeDisplayUtil.displayContents(" + tree.node.id + ", this);return false;' id='" + tree.node.id + "'>" + tree.node.name + "</a>";
		}
		else {
			if ("function" == typeof routeAction)
				buffer += "<a href='#' onclick='routeAction(false, \"Edit\");return false;' id='" + tree.node.id + "'>" + tree.node.name + "</a>";
			else
				buffer += tree.node.name;
        }
        buffer += "</span>";

		return buffer;
	}
}

//////////
//
// name: TreeUtil
// desc: A static utility class for manipulating a Tree object
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

var TreeUtil =
{
	appendChildren: function( children, vargs )
	{
		// This arguments are passed above in addChildren via the callback
		var parentId	   = vargs[0];
		var onComplete	   = vargs[1];
		var onCompleteArgs = vargs[2];

		var element = document.getElementById( "load" + parentId );
		if( element ) {
			var parent = element.parentNode;
			parent.removeChild( element );
		}


		var tree = TreeUtil.getTreeById( parentId );

		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeUtil",
				"appendChildren( children, " + parentId + ", onCompleteCallback )",
				"starting..." );

		if( children != null ) {
			// for each child, make a new node, and append it to tree
			for( var i = 0; i < children.assets.length; i++ ) {
				var child = children.assets[i];
				tree.appendNode( new Tree( child.name, child.id, parentId, child, 0 ) );
			}
		}
		TreeDisplayUtil.renderChildren( tree );
		TreeDisplayUtil.showChildren( tree );
		if( onComplete != null ) {
			onComplete( onCompleteArgs );
		} else {
			LogUtil.addMessage( LogUtil.WARNING,
				"TreeUtil",
				"appendChildren( children, " + parentId + ", onCompleteCallback )",
				"failed to call onComplete callback (probably not specified, since not required)" );
		}
	},

	//////////
	//
	// name: addChildren
	// desc: This method is overloaded in the following way:
	//			addChildren( pid );
	//			addChildren( children, pid );
	//
	//		addChildren( pid ): When given just a parent id, this function
	//		calls the data gathering function defined for the node
	//		to obtain its child data. Once the data gathering function
	//		has completed, it calls the call back function for
	//		inserting its results into the tree. In our case, we call
	//		addChildren( children, pid ).
	//
	//		addChildren( children, pid ): Given a set of child nodes and a
	//		parent id, this function iterates through the set of child
	//		nodes and inserts them as children of the parentNode (node
	//		with id = pid).
	//

	addChildren: function()
	{
		switch( arguments.length )
		{
			case 1:
				// we get one argument, the parent id
				var pid = arguments[0];
				var tree = this.getTreeById( pid );

				LogUtil.addMessage( LogUtil.DEBUG,
						"TreeUtil",
						"addChildren(" + pid + ")",
						"starting..." );

				// Each tree node defines the method in which it
				// obtains its child data. Get its action here.
				var getChildContents = tree.node.action;

				// display loading message
				var loadMessage = document.createElement( "div" );
				loadMessage.id = "load" + pid;
				loadMessage.className = "loadingMessage";
				loadMessage.innerHTML = "loading...";

				var element = TreeDisplayUtil.getElementById( pid );
				element.appendChild( loadMessage );

				// Call our data gathering function. It's async,
				// so we'll need to pass a callback function
				// for rendering the resulting data set. We also
				// pass along the parent id (pid) as an argument
				// for the callback method.
				getChildContents( tree.node.id, TreeUtil.addChildren, pid );

				break;

			case 2:
			case 3:
				// we get two or three arguments (this is callback)
				var children = arguments[0];
				var parentId = arguments[1];
				// third argument is tree ID...added by Ross

				// remove the "loading..." message
				var element = document.getElementById( "load" + parentId );
				if( element ) {
					var parent = element.parentNode;
					parent.removeChild( element );
				}

				var tree = TreeUtil.getTreeById( parentId );

				LogUtil.addMessage( LogUtil.DEBUG,
						"TreeUtil",
						"addChildren( children, " + parentId + " )",
						"starting..." );

				if( children != null ) {
					// for each child, make a new node, and append it to tree
					for( var i = 0; i < children.assets.length; i++ ) {
						var child = children.assets[i];
						tree.appendNode( new Tree( child.name, child.id, parentId, child, 0 ) );
					}
				}

				TreeDisplayUtil.renderChildren( tree );
				TreeDisplayUtil.showChildren( tree );

				break;
		}
	},

	addChildrenEx: function( pid, onComplete, vargs )
	{
		var tree = this.getTreeById( pid );

		LogUtil.addMessage( LogUtil.DEBUG,
				"TreeUtil",
				"addChildrenEx(" + pid + ")",
				"starting..." );

		// Each tree node defines the method in which it
		// obtains its child data. Get its action here.
		var getChildContents = tree.node.action;

		// display loading message
		var loadMessage = document.createElement( "div" );
		loadMessage.id = "load" + pid;
		loadMessage.className = "loadingMessage";
		loadMessage.innerHTML = "loading...";


		var element = TreeDisplayUtil.getElementById( pid );
		element.appendChild( loadMessage );

		// Call our data gathering function. It's async,
		// so we'll need to pass a callback function
		// for rendering the resulting data set. We also
		// pass along the parent id (pid) as an argument
		// for the callback method.

		/* getChildContents == tree.node.action == toolkit.getChildFolders( id, callback, args ); */
		getChildContents( tree.node.id, TreeUtil.appendChildren, [pid, onComplete, vargs] );
	},

	getTreeId: function( id )
	{
		return "T" + id;
	},

	getChildId: function( id )
	{
		return "C" + id;
	},

	getTreeById: function( id )
	{
		return TREES[this.getTreeId(id)]
	}
}

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
	// todo: create a 'highlight node' function
	/*
	if( clickedElementPrevious != null ) {
		clickedElementPrevious.style["background"] = "#ffffff";
		clickedElementPrevious.style["color"] = "#000000";
	}

	clickedElement.style["background"] = "#3366CC";
	clickedElement.style["color"] = "#ffffff";
	clickedElementPrevious = clickedElement;
	clickedIdPrevious = id;

	ContextMenuUtil.use( event, "treeMenu", clickedElement );

	return false;
	*/
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

	Explorer.setWorkingFolder( folder );
	SearchManager.execute( "id=" + id );
}

function onToggleClick( id, callback, args )
{
	toolkit.getChildFolders( id, -1, callback, args );
}

function makeElementEditable( element )
{
	element.contentEditable = true;
	element.focus();
	element.style.background = "#fff";
	element.style.color = "#000";
}

//////////
//
// The following defines what the context menu will look
// like when fired from the tree
//

var treeMenu = new ContextMenu( "treeMenu" );
//treeMenu.addItem( "Rename", function( node ){ makeElementEditable( node ) } );
//treeMenu.addItem( "Delete", function(){alert("not implemented")} );
//treeMenu.addBreak();
treeMenu.addItem( "Properties", function( node ) { Explorer.showFolderPropertiesWindow( node.id ); } );
ContextMenuUtil.add( treeMenu );

//////////
//
// Define the default images for the tree
//

var baseUrl = URLUtil.getAppRoot(document.location) +  "images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder  = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.plusopenfolder   = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "folderExpanded.png";
TreeDisplayUtil.minusopenfolder  = baseUrl + "folderExpanded.png";
TreeDisplayUtil.folder = baseUrl + "folder.png";

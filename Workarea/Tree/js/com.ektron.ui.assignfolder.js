function Tree( text, id, pid, data)
{
	this.node       = new Node( text, id, pid, data );
	this.children   = new Array();

	this.appendNode = function( node ) {
		this.children[this.children.length] = node;
	}

	TREES[TreeUtil.getTreeId(id)] = this;
}

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

var TreeDisplayUtil =
{
    getAncestry: function(id, ancestry)
    {

        ancestry = ancestry != null ? ancestry : [];
        var tree = TreeUtil.getTreeById(id);

        if (tree != null)
        {
            ancestry[ancestry.length] = tree;
            var id = tree.node.id;
            var pid = TreeDisplayUtil.getParentId(id);
            if (pid != -1)
            {
                return TreeDisplayUtil.getAncestry(pid, ancestry);
            } else
            {
                return ancestry;
            }
        }

        return null;
    },

    getRootNodeId: function(id)
    {

        var ancestry = TreeDisplayUtil.getAncestry(id);
        var rootNodeId = null;

        if (ancestry != null)
        {
            if (ancestry.length > 0)
            {
                var root = ancestry[ancestry.length - 1];
                rootNodeId = root.node.id;
            }
        }

        return rootNodeId;
    },


    getParentId: function(id)
    {

        var tree = TreeUtil.getTreeById(id);
        var pid = null;

        if (tree != null)
        {
            pid = -1;
            if (tree.node.pid != null)
            {
                pid = tree.node.pid;
            }
        }

        return pid;
    },


    getElementById: function(id)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getElementById(" + id + ")",
				"starting...");

        var element = document.getElementById(
						TreeUtil.getTreeId(id));

        return element;
    },

    reloadTree: function(id)
    {

        var tree = TreeUtil.getTreeById(id);

        if (tree != null)
        {
            if (tree.node.renderedChildren == true)
            {
                TreeDisplayUtil.removeTree(id);
            }
            TreeDisplayUtil.expandTree(tree);
        }
    },

    removeTree: function(id)
    {

        var tree = TreeUtil.getTreeById(id);

        if (tree != null)
        {
            // Set the renderedChildren flag to false
            tree.node.renderedChildren = false;

            // Remove the children from the DOM
            var containerId = TreeUtil.getChildId(id);
            var containerElement = document.getElementById(containerId);
            containerElement.parentNode.removeChild(containerElement);

            // This is where tree data is cached. Remove the child data from it.
            TREES[TreeUtil.getTreeId(tree.node.id)].children = [];
        }
    },

    reloadParentTree: function(id)
    {

        var tree = TreeUtil.getTreeById(id);

        if (tree != null)
        {
            var pid = tree.node.pid;
            TreeDisplayUtil.reloadTree(pid);
        }
    },

    getChildElementById: function(id)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getChildElementById(" + id + ")",
				"starting...");

        var element = document.getElementById(
						TreeUtil.getChildId(id));

        return element;
    },


    toggleTree: function(pid)
    {

        var tree = TreeUtil.getTreeById(pid);

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"toggleTree(" + pid + ")",
				"tree.node.status = " + tree.node.status);

        if (tree.node.status == "open")
        {
            TreeDisplayUtil.collapseTree(tree);
        } else
        {
            TreeDisplayUtil.expandTree(tree);
        }
    },

    expandTreeSet: function(ids)
    {

        return TreeDisplayUtil.__expandTreeSet([ids]);
    },

    __expandTreeSet: function(vargs)
    {

        var retVal = false;
        var ids = [];
        if (vargs.length > 0)
        {
            ids = vargs[0];
        }

        var currentIndex = 0;
        if (vargs.length > 1)
        {
            currentIndex = vargs[1];
            currentIndex++;
        }

        onComplete = TreeDisplayUtil.expandTreeSet;
        if (ids.length > currentIndex)
        {
            var id = ids[currentIndex];
            if (id != null)
            {
                var vargs = [ids, currentIndex, onComplete];
                var tree = TreeUtil.getTreeById(id);
                if (tree != null && tree.node.renderedChildren == true)
                {
                    retVal = TreeDisplayUtil.showChildren(tree);
                    if (retVal)
                    {

                        retVal = TreeDisplayUtil.__expandTreeSet(vargs);
                    }
                } else
                {
                    retVal = TreeDisplayUtil.expandTreeById(id, TreeDisplayUtil.__expandTreeSet, vargs);
                }
            }
        }
        else
        {

            // reached end of id array, signal success:
            retVal = true;
        }

        return (retVal);
    },

    expandTree: function(tree)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTree(" + tree.node.id + ")",
				"starting...");

        // the "tree.node.id == 0" is a workaround for a bug on in the ContentService
        // that has the root element returning hasChildren = false.
        if ((tree.node.data.hasChildren == "true") || (tree.node.id == 0))
        {
            if (!tree.node.renderedChildren)
            {
                tree.node.renderedChildren = true;
                TreeUtil.addChildren(tree.node.id);
            } else
            {
                TreeDisplayUtil.showChildren(tree);
            }
        }
    },

    expandTreeEx: function(tree, onComplete, vargs)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeEx(" + tree.node.id + ")",
				"starting...");

        // the "tree.node.id == 0" is a workaround for a bug on in the ContentService
        // that has the root element returning hasChildren = false.
        if ((tree.node.data.hasChildren == "true") || (tree.node.id == 0))
        {
            if (!tree.node.renderedChildren)
            {
                tree.node.renderedChildren = true;
                TreeUtil.addChildrenEx(tree.node.id, onComplete, vargs);
            } else
            {
                TreeDisplayUtil.showChildren(tree);
            }
        }
    },

    expandTreeById: function(id)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeById(" + id + ")",
				"starting...");

        var tree = TreeUtil.getTreeById(id);
        TreeDisplayUtil.expandTree(tree);
    },


    expandTreeById: function(id, onComplete, vargs)
    {

        var retVal = false;
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeById(" + id + ")",
				"starting...");

        var tree = TreeUtil.getTreeById(id);
        if (tree)
        {

            TreeDisplayUtil.expandTreeEx(tree, onComplete, vargs);
            retVal = true;
        }
        else
        {

            retVal = false;
        }
        return (retVal);
    },


    collapseTree: function(tree)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"collapseTree(" + tree.node.id + ")",
				"starting...");

        if (tree.node.data.hasChildren)
        {
            TreeDisplayUtil.hideChildren(tree);
        }
    },

    hideChildren: function(tree)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"hideChildren(" + tree.node.id + ")",
				"starting...");

        var element = TreeDisplayUtil.getChildElementById(tree.node.id);
        element.style["display"] = "none";
        tree.node.status = "closed";

        var iconElement = document.getElementById("I" + tree.node.id);
        if (iconElement)
        {
            if (TreeDisplayUtil.plusclosefolders == undefined)
            {
                iconElement.src = TreeDisplayUtil.plusclosefolder;
            } else
            {
                iconElement.src = TreeDisplayUtil.plusclosefolders[tree.node.data.type];
            }
        }
    },

    showChildren: function(tree)
    {

        var retVal = false;
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showChildren(" + tree.node.id + ")",
				"starting...");

        var childElement = TreeDisplayUtil.getChildElementById(tree.node.id);
        var iconElement = document.getElementById("I" + tree.node.id);

        if (!childElement)
        {
            retVal = false;
            LogUtil.addMessage(LogUtil.CRITICAL,
					"TreeDisplayUtil",
					"showChildren(" + tree.node.id + ")",
					"Couldn't find container for child element");
        } else
        {
            childElement.style["display"] = "";

            if (iconElement)
            {
                if (TreeDisplayUtil.minusclosefolders == undefined)
                {
                    iconElement.src = TreeDisplayUtil.minusclosefolder;
                } else
                {
                    iconElement.src = TreeDisplayUtil.minusclosefolders[tree.node.data.type];
                }
            }

            tree.node.status = "open";
            if (true == IsAlreadySelected(tree.node.id.toString()))
            {
                TreeDisplayUtil.UpdateSubTree(tree.node.data.id);
            }
            retVal = true;
        }
        return (retVal);
    },

    UpdateSubTree: function(parentTreeId)
    {
        // #55469: timing issue. the subTree are invisible/hidden and therefore this following might not work if subTree are not yet expanded.
        var isParentChecked = $ektron("img#I" + parentTreeId).siblings("input:checkbox#itemlist").attr("checked");
        if (true == isParentChecked)
        {
            if (this.isFolderRecursive(parentTreeId))
            {
                var me = this;
                $ektron("ul#C" + parentTreeId + " input:checkbox").each(function()
                {
                    if (me.isFolderEnabled(this.value))
                    {
                        this.checked = isParentChecked;
                        this.disabled = isParentChecked;
                    }
                });
            }
        }
    },

    showSelf: function(tree, container)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showSelf(" + tree.node.id + ")",
				"starting...");

        if (!tree.node.rendered)
        {
            TreeDisplayUtil.renderSelf(tree, container);
        }

        var element = TreeDisplayUtil.getElementById(tree.node.id);
        element.style["display"] = "";
        tree.node.status = "closed";
    },


    renderChildren: function(tree)
    {

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderChildren(" + tree.node.id + ")",
				"starting...");

        var childContainerId = TreeUtil.getChildId(tree.node.id);
        var ul = document.createElement("ul");
        ul.setAttribute("className", "ekSubTree");
        ul.setAttribute("id", childContainerId);

        var numberChildren = tree.children.length;
        for (var i = 0; i < numberChildren; i++)
        {

            var child = tree.children[i];

            var li = document.createElement("li");
            li.setAttribute("className", "ekTreeItem");
            li.setAttribute("id", TreeUtil.getTreeId(child.node.id));

            li.innerHTML = TreeDisplayUtil.createNodeHTML(child, tree.node.id);
            ul.appendChild(li);
            child.node.rendered = true;
        }
        tree.node.renderedChildren = true;

        var parentElement = document.getElementById(TreeUtil.getTreeId(tree.node.id));
        if (parentElement != null)
        {

            parentElement.appendChild(ul);
        }
    },

    displayContents: function(id, clickedElement)
    {

        return onFolderClick(id, clickedElement);
    },


    renderSelf: function(tree, container)
    {

        var src = "";
        if (tree.node.data.hasChildren == "true")
        {
            if (TreeDisplayUtil.plusclosefolders == undefined)
            {
                src = TreeDisplayUtil.plusclosefolder;
            } else
            {
                src = TreeDisplayUtil.plusclosefolders[tree.node.data.type];
            }
        } else
        {
            if (TreeDisplayUtil.folders == undefined)
            {
                src = TreeDisplayUtil.folder;
            } else
            {
                src = TreeDisplayUtil.folders[tree.node.data.type];
            }
        }
        if ("undefined" == typeof src) return;
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderSelf(" + tree.node.id + ")",
				"starting...");

        var ul = document.createElement("ul");
        ul.setAttribute("className", "ektree");
        var li = document.createElement("li");
        li.setAttribute("className", "ekTreeRootItem");
        li.setAttribute("id", TreeUtil.getTreeId(tree.node.id));
        var buffer = "<img onclick='TreeDisplayUtil.toggleTree(" + tree.node.id + ")' class='hasChildIcon' id='I" + tree.node.id + "' src='" + src + "'>";

        var checkedoption = '';
        if (IsAlreadySelected(tree.node.id.toString())) { checkedoption = ' checked '; }
        buffer += "<input type='checkbox' onclick='UpdateSelectedValue(this);' name='itemlist' id='itemlist' value='" + tree.node.id + "'" + checkedoption + "/><span onclick='TreeDisplayUtil.expandTreeById(" + tree.node.id + ")'>" + tree.node.name + "</span>";
        li.innerHTML = buffer
        ul.appendChild(li);

        if (!container)
        {
            container = document.getElementById("TreeOutput");
        }
        if (ul.outerHTML == 'undefined')
        {
            container.innerHTML = ul.outerHTML;
        } else
        {
            container.innerHTML = '<UL class=ektree>' + ul.innerHTML + '</UL>';
        }

        tree.node.rendered = true;
    },

    isFolderRecursive: function(folderId)
    {
        if ("undefined" == typeof g_newFolderDescendants)
        {
            return true; // default
        }

        for (var i = 0; i < g_newFolderDescendants.length; i++)
        {
            if (g_newFolderDescendants[i] == folderId)
            {
                return true;
            }
        }

        for (var i = 0; i < g_prevFolderDescendants.length; i++)
        {
            if (g_prevFolderDescendants[i] == folderId)
            {
                return true;
            }
        }

        return false;
    },

    isFolderEnabled: function(folderId)
    {
        if ("undefined" == typeof g_prevFolderDescendants)
        {
            return true; // default
        }

        for (var i = 0; i < g_prevFolderDescendants.length; i++)
        {
            if (g_prevFolderDescendants[i] == folderId)
            {
                return false;
            }
        }

        for (var i = 0; i < g_prevFolderChildren.length; i++)
        {
            if (g_prevFolderChildren[i] == folderId)
            {
                return false;
            }
        }

        return true;
    },

    createNodeHTML: function(tree, treeParentId)
    {
        var src = "";
        if (tree.node.data.hasChildren == "true")
        {
            if (TreeDisplayUtil.plusclosefolders == undefined)
            {
                src = TreeDisplayUtil.plusclosefolder;
            }
            else
            {
                src = TreeDisplayUtil.plusclosefolders[tree.node.data.type];
            }
        }
        else
        {
            if (TreeDisplayUtil.folders == undefined)
            {
                src = TreeDisplayUtil.folder;
            }
            else
            {
                src = TreeDisplayUtil.folders[tree.node.data.type];
            }
        }

        var checkedoption = "";
        var checkBoxAttributeList = "";
        var buffer = "";
        var isNodeSelected = IsAlreadySelected(tree.node.id.toString());
        if (isNodeSelected)
        {
            checkedoption = " checked=\"checked\"";
        }

        if (!this.isFolderEnabled(tree.node.id.toString()))
        {
            checkedoption += " disabled=\"disabled\"";
        }

        buffer = "<img onclick='TreeDisplayUtil.toggleTree(" + tree.node.id + ")' class='hasChildIcon' id='I" + tree.node.id + "' src='" + src + "' />";
        checkBoxAttributeList = "onclick='UpdateSelectedValue(this);' name='itemlist' id='itemlist'";
        buffer += "<input type='checkbox' " + checkBoxAttributeList + " value='" + tree.node.id + "'" + checkedoption + " /><span onclick='TreeDisplayUtil.expandTreeById(" + tree.node.id + ")'>" + tree.node.name + "</span>";
        return buffer;
    }
}

var TreeUtil =
{
	appendChildren: function( children, vargs )
	{
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

	addChildren: function()
	{
		switch( arguments.length )
		{
			case 1:
				// we get one argument, the parent id (this is the add request)
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
                if (element != null) 
                {
				element.appendChild( loadMessage );
                }


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
        if (element != null) 
        {
		element.appendChild( loadMessage );
        }
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

var clickedElementPrevious = null;
var clickedIdPrevious = null;

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


var treeMenu = new ContextMenu( "treeMenu" );
treeMenu.addItem( "Properties", function( node ) { Explorer.showFolderPropertiesWindow( node.id ); } );
ContextMenuUtil.add( treeMenu );

var baseUrl ="images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder  = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.plusopenfolder   = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "folderExpanded.png";
TreeDisplayUtil.minusopenfolder  = baseUrl + "folderExpanded.png";
TreeDisplayUtil.folder = baseUrl + "folder.png";
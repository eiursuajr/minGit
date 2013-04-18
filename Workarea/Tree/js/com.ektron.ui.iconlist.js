/////////
//
// name: IconList
// desc: Given a list of data items, IconList will
//		 render the results as a list of icons and
//		 associated meta data.
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function IconList( itemList )
{
	/////////
	//
	// public members
	//
	
	this.display	= __IconList_display;
	
	/////////
	//
	// private members
	//

	this.updateStatusBar    = __IconList_updateStatusBar;
	this.setResultContainer = __IconList_setResultContainer;
	this.setStatusContainer = __IconList_setStatusContainer;
	this.itemList		 = null;
	this.resultContainer = null;
	this.statusContainer = null;

	// constructor
	{
		this.itemList = itemList;
	}

	//////////
	//
	// private members
	//
}

function __IconList_setResultContainer( element )
{
	this.resultContainer = element;
}

function __IconList_setStatusContainer( element )
{
	this.statusContainer = element;
}

function __IconList_updateStatusBar()
{
	var status   = this.statusContainer;
	var itemList = this.itemList;
	var text = "";
	
	if( itemList.length > 0 ) {
		var plural = itemList.length > 1 ? "s" : "";
		text = "Displaying " + itemList.length + " item" + plural;
	} else {
		text = "";
	}

	setTimeout( function() {
		status.innerText = text;
	}, 50 );
}

function __IconList_display()
{
	var buffer = "";
	IconListUtil.loadPreviewCount = 0;
	IconListUtil.clearList();

	var itemList = this.itemList;
	var url    = ExplorerConfigUtil.getSiteRoot();
	var expurl = ExplorerConfigUtil.getParam( "exp_http_root" );
	var qlink  = ExplorerConfigUtil.getServerRoot();

	this.updateStatusBar();
	if( itemList ) {
		if( itemList.length == 0 ) {
			buffer = "<div style='padding:2px;margin-left:2px;'>There are no items to display.</div>";
		}
	}

	var output = this.resultContainer;
	for( var i = 0; i < itemList.length; i++ ) {
		var asset = itemList[i];
		IconListUtil.addItem( asset );
		buffer += '<div class="searchResultItem' + ( i % 2) + '" oncontextmenu="return ContextMenuUtil.use(event, \'iconListItem\', ' + i + ' )">';

		var type = IconListUtil.getAssetType( asset );

		// if it's content and we have a quicklink, show preview
		var showPreview = false;
		if( type == "content" ) {
			if( asset["quickLink"]  ) {
				showPreview = true;
			}
		}

		// if it's a document, determine which icon to show based on mimetype
		var iconname  = IconListUtil.getIconName( asset );
		var img = '<img class="itemIcon" src="' + url + expurl + '/images/icons/' + iconname + '.gif">';

		asset["__previewurl"] = qlink + asset["quickLink"];

		buffer += '<div class="itemPreviewBox">';
		if( showPreview ) {
			buffer += '<iframe name="preview' + i + '" id="preview' + i + '" onreadystatechange="IconListUtil.onReadyStateChange(this, ' + i + ')"';
			buffer += ' style="display:none;" class="itemPreview" scrolling=no marginwidth=0 marginheight=0';
			buffer += ' frameborder=0>';
			buffer += '</iframe>';
		}
		buffer += '<span id=i' + i + ' class="itemIconBox">';
		buffer += img;
		buffer += '</span>';
		buffer += '</div>'

		var firstName = asset["lastEditorFirstName"];
		var lastName  = asset["lastEditorLastName"];
		var dateModified = asset["dateModified"];
		
		if( !dateModified ) {
			dateModified = asset["lastModified"];
		}
		if(  !firstName ) {
			firstName = asset["editorFirstName"];
		}
		if( !lastName ) {
			lastName = asset["editorLastName"];
		}

		var onclickhandler = 'onclick="window.open(\'' + asset["__previewurl"] + '\')"';

		if( type == "document" ) {
			onclickhandler = 'onclick="window.open(\'../showcontent.aspx?id=' + asset["id"] + '\')"';
		}

		var link = '<span class="itemTitleLink" {%onclick%}><b>' + asset["title"] + '</b></span>';
		link = link.replace( "{%onclick%}", onclickhandler );
		
		buffer += '<ul>';
		buffer += '<li class="itemTitle">' + link + '</li>';
		
		var summary = IconListUtil.getSummary( asset );
		if( summary != "" ) {
			buffer += '<li style="color:666666;">"' + summary + ' ... "</li>';
		}

		buffer += '<li style="color:008000;">Last edited by ' + firstName + ' ' + lastName + " on " + dateModified  + '</li>';
		buffer += '</ul>';
		buffer += '</div>';
	}
	
	// load html into display container
	output.innerHTML = buffer;
	
	// Get the preview size from the registry
	IconListUtil.setPreviewSize();

	// kick off loading of previews
	IconListUtil.loadPreviews();
}

//////////
//
// name: IconListUtil
// desc: Static util class for manipulating and managing an IconList
//

var IconListUtil =
{
	//////////
	//
	// name: addItem
	// desc: adds data item to the list
	//

	addItem: function( item )
	{
		var data = IconListUtil.data;
		data[data.length] = item;
	},

	//////////
	//
	// name: onReadyStateChange
	// desc: This is the onReadyStateChange handler for the item previews
	//
	
	onReadyStateChange: function( element, index )
	{
		if( element.readyState == "complete" ) {
			if( element.src != "" ) {
				IconListUtil.showPreview( element, index );
			}
		}
	},
	
	//////////
	//
	// name: showPreview
	// desc: Notice that this is showPreview singular. This function will
	//	     show an iframe and hide its icon.
	//
	
	showPreview: function( element, index )
	{
		element.style.display = '';
		var icon = document.getElementById( "i" + index );
		if( icon ) {
			icon.style.display = 'none';
			var body = window.frames[element.name].document.body;
			if( body ) {
				body.oncontextmenu = function() { return false; };
				body.onclick = function() { return false; };
				body.onselectstart = function() { return false; };
				body.style.cursor = "default"
				body.oncontextmenu = function() { 
					// window.frames[element.name].parent.IconListUtil.menu( index );
					return false;
				}
			}
		}
	},

	//////////
	//
	// name: loadPreviews
	// desc: Iterates through all of the elements and synchronously loads their previews
	//

	loadPreviews: function()
	{
		var i = IconListUtil.loadPreviewCount++;
		if( i < IconListUtil.data.length ) {
			// if Ektron Explorer is closed after it is opened through
			// Windows Explorer while previews are loading, a JavaScript
			// error occurs since mainWindow becomes null. We'll try
			// catch the error and drop it.
			try {
				var iframe = mainWindow.document.getElementById( "preview" + i );
				if( iframe ) {
					var asset = IconListUtil.data[i];
					var url = asset["__previewurl"];
					if( url ) {
						iframe.src = url;
					}
				}

				setTimeout( IconListUtil.loadPreviews, 1000 * ( i + 2 ) );
			} catch( e ) {
				; //
			}
		}
	},

	//////////
	//
	// name: clearList
	// desc: zeros out the data list
	//

	clearList: function()
	{
		IconListUtil.data = new Array();
	},

	//////////
	//
	// name: setPreviewSize
	// desc: Sets the size of the content item preview window
	//

	setPreviewSize: function()
	{
		var size = ExplorerConfigUtil.getPersistentParam( "previewSize" );	
		
		if( size ) {
			IconListUtil.previewSize = size;
		}
	},

	//////////
	//
	// name: getIconName
	// desc: Given an asset, it returns the appropriate icon name
	//	
	
	getIconName: function( asset )
	{
		var type = IconListUtil.getAssetType( asset );
		var iconname  = "ektron"; // default icon
		
		if( type == "document" ) {
			var assetInfo = asset["contentText"];
			var mimetype  = assetInfo.replace( /.*<MimeName>([^<]*)<\/MimeName>.*/gim, "$1" );

			if( mimetype != "null" ) {
				iconname = mimetype;
			}
		}
		return iconname;
	},
	
	//////////
	//
	// name: getAssetType
	// desc: Given an asset, it returns its type, either "document" or "content"
	//	
	
	getAssetType: function( asset )
	{
		// if asset["type"] is ane integer, its a document. otherwise, it's content
		return ( new String( parseInt( asset["type"], 10 ) ) == "NaN" ) ? "content" : "document";
	},

	//////////
	//
	// name: getSummary
	// desc: Given an asset, it returns a summary
	//	
	
	getSummary: function( asset )
	{
		var type = IconListUtil.getAssetType( asset );
		var keys = [ "teaser", "contentText" ]
		var summary = "";

		if( type == "content" ) {
			for( var i = 0; i < keys.length; i++ ) {
				var _summary = StringUtil.removeTags( asset[keys[i]] );
				if( ( _summary != null ) && ( _summary != "" ) && ( _summary != "null" ) ) {
					summary = _summary;
					if( summary.charAt( 0 ) == "e" ) {
						summary = summary.substring( 2 );
					}
					break;
				}
			}
		}
		
		summary = StringUtil.removeTags( summary );
		summary = StringUtil.trim( summary, 200 );
		
		return summary;
	},
	
	data: new Array(),
	loadPreviewCount: 0,
	previewSize : "S"
}

//////////
//
// The context menu used for the iconlist
//

var iconListSubMenu = new ContextMenu( "iconListPreviewSize" );
iconListSubMenu.addItem( "Small", function(i) { __setPreviewSize( i, "S" ) } );
iconListSubMenu.addItem( "Medium", function(i) { __setPreviewSize( i, "M" ) } );
iconListSubMenu.addItem( "Large", function(i) { __setPreviewSize( i, "L" ) } );
iconListSubMenu.addBreak();
iconListSubMenu.addItem( "All Small", function(i) { 
	ExplorerConfigUtil.setPersistentParam( "previewSize", "S" );
	__setAllPreviewSize( "S" )
	});
iconListSubMenu.addItem( "All Medium", function(i) {
	ExplorerConfigUtil.setPersistentParam( "previewSize", "M" );
	__setAllPreviewSize( "M" )
	} );
iconListSubMenu.addItem( "All Large", function(i) {
	ExplorerConfigUtil.setPersistentParam( "previewSize", "L" );
	__setAllPreviewSize( "L" )
	} );

var iconListMenu = new ContextMenu( "iconListItem" );
iconListMenu.addMenu( "View", iconListSubMenu );
iconListMenu.addBreak();
iconListMenu.addItem( "Properties", function(){} );
//iconListMenu.addItem( "About", function(){} );

ContextMenuUtil.add( iconListMenu );

//////////
//
// context menu item click handlers
// 

function __setPreviewSize( id, size )
{
	var element = document.getElementById( "preview" + id );
	if( element ) {
		element.className = "itemPreview" + size;
	}
}

function __setAllPreviewSize( size )
{
	var nodeSet    = null;
	var tryCount   = 0;
	var classNames = [ "itemPreviewS", "itemPreviewM", "itemPreviewL", "itemPreview" ];

	// since each icon in the list can display differently, we'll 
	// look for all sized icons, and set them to the desired size
	for( var i = 0; i < classNames.length; i++ ) {
		nodeSet = DOMUtil.getElementByClassName( classNames[i] );
		if( nodeSet != null ) {
			if( nodeSet.length != 0 ) {
				for( var j = 0; j < nodeSet.length; j++ ) {
					var element = nodeSet[j];
					if( element ) {
						element.className = "itemPreview" + size;
					}
				}
			}
		}
	}
}

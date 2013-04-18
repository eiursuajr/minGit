/////////
//
// name: IconList
// desc: Given a list of data items, IconList will
//		 render the results as a list of icons and
//		 associated meta data.
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

// Note: This file has been modified (and renamed)
// to work within the Ektron CMS Workarea (BCB)...

/*
var iconList = new IconList( data );
iconList.display();
*/

//////////
//
// name: forcePreview
// desc: 
//
function forcePreview( index )
{
	var el = document.getElementById( "preview" + index );
	if (el){
		if (el.style.display != ''){
			el.style.display = '';
		
			// hide the icon:
			var icon = document.getElementById( "i" + index );
			if( icon ) {
				icon.style.display = 'none';
			}

			// now load the page:
			var asset = IconListUtil.data[index];
			var url = asset["__previewurl"];
			if( url ) {
				el.src = url;
			}
		}
	}
}


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
	
	this.setResultContainer = __IconList_setResultContainer;
	this.itemList		 = null;
	this.resultContainer = null;
	
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

function __IconList_display()
{
	var buffer = "";
	IconListUtil.loadPreviewCount = 0;
	IconListUtil.clearList();
	
	var itemList = this.itemList;
	var url    = "";
	var qlink  = "";
	
	var output = this.resultContainer;
	for( var i = 0; i < itemList.length; i++ ) {
		var asset = itemList[i];
		IconListUtil.addItem( asset );
		buffer += '<div class="GSearchResultItem' + ( i % 2) + '" >';
		
		var img;

		var type = ( new String( parseInt( asset["type"], 10 ) ) == "NaN" ) ? "content" : "document";
		var showPreview = false;
		var showImage = false;
		
		img = '<span id=i' + i + ' class="itemIconBox" ' +
			' onmouseover="forcePreview(' + i + ')" >' +
			'<img class="itemIcon" src="' + url + '/workarea/images/application/icon-ektron.gif" >' + 
			'</span>';
		
		if( type == "content" ) {
			if( asset["imageLink"]  ) {
				showPreview = true;
			}
		} else {
			// type is document:
			showImage = true;
			if (asset["thumbnailUrl"]){
				img = '<span id=i' + i + ' class="itemIconBox">' +
					'<img class="itemIcon" src="' + asset["thumbnailUrl"] + '">' + 
					'</span>';
			}
		}
		
		if( showPreview ) {
			asset["__previewurl"] = qlink + asset["imageLink"];
			buffer += '<div class="itemPreviewBox">';
			buffer += '<a href="' + qlink + asset["quickLink"] + '">';
			buffer += '<iframe name="preview' + i + '" id="preview' + i + '" onreadystatechange="IconListUtil.onReadyStateChange(this, ' + i + ')"';
			buffer += ' style="display:none;" class="itemPreview" scrolling=no marginwidth=0 marginheight=0';
			buffer += ' quickLink="' + qlink + asset["quickLink"] + '"';
			buffer += ' frameborder=0>';
			buffer += '</iframe>' + img + '</a>' + '</div>';
		} else {
			buffer += '<a href="' + qlink + asset["quickLink"] + '">';
			buffer += img + '</a>';
		}

		var firstName = asset["lastEditorFirstName"];
		var lastName  = asset["lastEditorLastName"];
		var dateModified = asset["dateModified"];
		var summary = asset["teaser"];
		
		
		if( !summary ) {
			summary = asset["content"];
		}
		
		if( !dateModified ) {
			dateModified = asset["lastModified"];
		}
		if(  !firstName ) {
			firstName = asset["editorFirstName"];
		}
		if( !lastName ) {
			lastName = asset["editorLastName"];
		}
		
		buffer += '<ul class="GSearch">';
		buffer += '<li class="itemTitle"><a href="' + qlink + asset["quickLink"] + '"><b>' + asset["title"] + '</b></a></li>';
		
		if( summary ) {
			summary = StringUtil.removeTags( summary );
			summary = StringUtil.trim( summary, 128 );
			buffer += '<li style="color:666666;">"' + summary + ' ... "</li>';
		}

		buffer += '<li style="color:008000;">';
		if ((firstName + lastName).length) {
			buffer += 'Last edited by ' + firstName + ' ' + lastName;
			if (dateModified.length) {
				buffer += ' on ' + dateModified;
			}
		} else if (dateModified.length) {
			buffer += 'Last edited on ' + dateModified;
		}
		buffer += '</li>';
		buffer += '</ul>';
		buffer += '</div>';
	}
	
	// load html into display container
	output.innerHTML = buffer;

	// kick off loading of previews
	IconListUtil.loadPreviews();
}

//////////
//
// name: IconListUtil
// desc: Static util class for manipulating and managing an IconList
//

/*

IconListUtil.addItem( itemObject );

*/

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
				IconListUtil.clearAllHandlers(body); //IconListUtil.clearHandlers(body);
			}
		}
	},

	//////////
	// 
	// name: clearAllHandlers
	// desc: Resets the event handlers for the 
	// supplied element and all contained elements.
	// 

	clearAllHandlers: function( element, index  )
	{
		var idx;
		if( element ) {
			for (idx=0; idx<element.all.length; ++idx) {
				IconListUtil.clearHandlers(element.all(idx), index);
			}
		}
	},
	
	//////////
	// 
	// name: clearHandlers
	// desc: Attempts to reset the event handlers for the supplied element.
	// 

	clearHandlers: function( element, index )
	{
		if( element ) {
			if (null != element.oncontextmenu){
				element.oncontextmenu = ''; //function() { return false; };
			}
			if (null != element.onclick ){
				element.onclick = ''; //function() { return false; };
			}
			if (null != element.ondblclick ){
				element.onclick = ''; //function() { return false; };
			}
			if (null != element.onselectstart){
				element.onselectstart = ''; //function() { return false; };
			}
			if ((null != element.cursor) && ("" != element.cursor)){
				element.cursor = "default"
			}
			if (null != element.onmousedown){
				element.onmousedown = ''; //function() { return false; };
			}
			if (null != element.onmouseup){
				element.onmouseup = ''; //function() { return false; };
			}
			if (null != element.onmousemove){
				element.onmousemove = ''; //function() { return false; };
			}
			if (null != element.onmouseover){
				element.onmouseover = ''; //function() { return false; };
			}
			if (null != element.onmouseout){
				element.onmouseout = ''; //function() { return false; };
			}
			if ("select" == element.tagName.toLowerCase()){
				element.disabled = true;
			}
			if ((null != element.alt) && ("" != element.alt)){
				element.alt = ""
			}
			if ((null != element.title) && ("" != element.title)){
				element.title = ""
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
				//var iframe = mainWindow.document.getElementById( "preview" + i );
				var iframe = document.getElementById( "preview" + i );
				if( iframe ) {
					var asset = IconListUtil.data[i];
					var url = asset["__previewurl"];
					if( url ) {
						iframe.src = url;
					}
				}

				setTimeout( IconListUtil.loadPreviews, 500 * ( i + 2 ) );
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
	
	data: new Array(),
	loadPreviewCount: 0,
	previewSize : "S"
}

//////////
//
// name: PropertiesWindow
// desc: A properties window for Folder information. Displays
//       folder meta data.
//

function PropertiesWindow( type, data )
{
	//////////
	//
	// public methods
	//
	
	this.show = __PropertiesWindow_show;
	
	//////////
	//
	// private members
	//
	
	this.width   = 325;
	this.height  = 365;
	this.console = null;
	this.type = type;
	this.data = data;
}

function __PropertiesWindow_show()
{
	if( this.console == null ) {
		var rand = new String( Math.random() ).replace( ".", "" );
		var settings = "resizable=yes,height=" + this.height + ",width=" + this.width + ",toolbar=no,scrollbars=auto,menubar=no"
		this.console = window.open( "resources/tabdialog.html", "PropertiesWindow" + rand, settings );
	}

	var args = { "console": this.console, "type": "ektron", "title": "Ektron Explorer Properties" };
	var callback = PropertiesWindowUtil.display;
	var tryCount = 0;
	var data = this.data;

	try {
		this.console.document.body.onload = function() {
			PropertiesWindowUtil.display( data, args );
		}
	} catch( e ) {
		LogUtil.addMessage(
			LogUtil.CRITICAL,
			"PropertiesWindow",
			"show",
			"failed displaying window: " + e.message + "."
		);
	}
}


//////////
//
// name: FolderPropertiesWindow
// desc: A properties window for Folder information. Displays
//       folder meta data.
//

function FolderPropertiesWindow( folderId )
{
	//////////
	//
	// public methods
	//
	
	this.show = __FolderPropertiesWindow_show;
	
	//////////
	//
	// private members
	//
	
	this.width   = 300;
	this.height  = 250;
	this.console = null;
	this.folderId = folderId;
}

function __FolderPropertiesWindow_show()
{
	if( this.console == null ) {
		var rand = new String( Math.random() ).replace( ".", "" );
		var settings = "resizable=yes,height=" + this.height + ",width=" + this.width + ",toolbar=no,scrollbars=auto,menubar=no"
		this.console = window.open( "resources/tabdialog.html", "folderPropertiesWindow" + rand, settings );
	}

	// getFolder takes a folderId, and we give it our callback and callback args
	var args = { "console": this.console, "type": "folder", "title": "Folder Properties" };
	var callback = PropertiesWindowUtil.display;
	var fid = this.folderId;
	var tryCount = 0;

	try {
		this.console.document.body.onload = function() {
			toolkit.getFolder( fid, callback, args );
		}
	} catch( e ) {
		LogUtil.addMessage(
			LogUtil.CRITICAL,
			"FolderPropertiesWindow",
			"show",
			"failed to attach event handler to dialog on load event: " + e.message + "."
		);
	}
}

//////////
//
// name: PropertiesWindowUtil
// desc: Static class for taking an asset and a console, and rendering the content
//       to the console. The display function below is the callback method used
//		 by the toolkit's getFolder method above.
//
//

var PropertiesWindowUtil =
{
	display: function( asset, args )
	{
		var console = args["console"];
		var type  = args["type"];
		var title = args["title"];
		
		try {
			console.setTitle( title );
			console.setType( type );
			console.setData( asset );
			console.render();
		} catch( e ) {
			; //
		}
	}
}



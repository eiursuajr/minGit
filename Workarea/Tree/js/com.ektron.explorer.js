//////////
//
// name: Explorer
// desc: This serves as the entry point into the application.
// auth: William Cava <willam.cava@ektron.com>
// date: April 2005
//

var Explorer =
{
	initTextPid: null,
	
	//////////
	//
	// name: initialize
	// desc: This method is called when the Explorer panel
	//       loads and before 'start' is called. It performs
	//		 setup and initialization routines.
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	initialize: function()
	{
		Explorer.showLoadingMessage();
		Explorer.defineDefaultContextMenu();
	},

	//////////
	//
	// name: clearLoadingMessage
	// desc: Clears the loading message
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	clearLoadingMessage: function()
	{
		clearInterval(Explorer.initTextPid);
		var element = document.getElementById( "loadingMessage" );
		document.body.removeChild( element );
	},

	//////////
	//
	// name: showLoadingMessage
	// desc: Displays animated loading message
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	showLoadingMessage: function()
	{
		var element = document.createElement( "span" );
		element.setAttribute( "id", "loadingMessage" );
		element.innerText = "Loading ";

		var progressBar = document.createElement( "span" );
		progressBar.setAttribute( "id", "loadingBar" );
		progressBar.innerHTML = "&nbsp;";
		
		element.appendChild( progressBar );
		document.body.appendChild( element );
	
		var width = 0;
		Explorer.initTextPid = setInterval(
			function() {
				width = parseInt( progressBar.style["width"], 10 );
				width = new String(width) == "NaN" ? 10 : width;
				width = width > 70 ? "10px" : "" + ( width += 1 ) + "px";
				progressBar.style["width"] = width;
			},
			75
		); // end setInterval
		
	},
	
	//////////
	//
	// name: openFolderBrowser
	// desc: Launches a folder browser, returns the folderId of selected folder
	//
	
	openFolderBrowser: function( selectElement )
	{
		var settings = "dialogWidth:325px;dialogHeight:400px;resizable:no;status:no;";
		var folder = window.showModalDialog( "resources/folderbrowser.html", self, settings );
		
		if( folder != null ) {
			Explorer.setWorkingFolder( folder );
		}			
	},

	//////////
	//
	// name: setWorkingFolder
	// desc: Given a folder object (folder.id, folder.name), this function sets
	//	     the current working folder by modifying the select list to include
	//		 the folder.
	//
		
	setWorkingFolder: function( folder )
	{
		var selectElement = document.forms["search"]["folderid"];
		selectElement.style.display = 'none';

		if( folder != null ) {	
			FormUtil.removeOptionByValue( selectElement, "{browse}" );
			var optionElement = FormUtil.getOptionByValue( selectElement, folder.id );
			if( optionElement == null ) {
				FormUtil.addOptionToList( selectElement, folder.name, folder.id );
			}
			FormUtil.addOptionToList( selectElement, "Browse...", "{browse}" );
			FormUtil.selectOptionByValue( selectElement, folder.id )
		}
		selectElement.style.display = '';
	},
	
	//////////
	//
	// name: start
	// desc: Kicks things off
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//
	
	start: function()
	{
		Explorer.showTabs();
	},
	
	onLoadExplorePanel: function()
	{
		Explorer.clearLoadingMessage();
	},
	
	//////////
	//
	// name: showTabs
	// desc: Draws the tabs on the screen
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	showTabs: function()
	{
		var tabPanel = new TabPanel( "explorerPanel", document.body.clientWidth - 8, document.body.clientHeight - 10, 70, 25 );
		tabPanel.addTab( "Explore" );
		tabPanel.addTab( "Search" );
		tabPanel.setPanel( "Explore", Explorer.getExplorePanel() );
		tabPanel.setPanel( "Search", Explorer.getSearchPanel() );
		tabPanel.display();
		
		// todo: this should be moved, perhaps into the tab code. maybe
		// have a flag for fill/autoresize
		document.body.onresize = function() {
			var elements = DOMUtil.getElementByClassName( "tabPanel" );
			for( var i = 0; i < elements.length; i++ ) {
				var element = elements[i];
				element.style.width = document.body.clientWidth - 8;
				element.style.height = document.body.clientHeight - 35;
			}

			var foo = document.getElementById( "explorerPanelGroup" );
			foo.style.width = document.body.clientWidth - 8;
			foo.style.height = document.body.clientHeight - 10;
		}
	},
	
	//////////
	//
	// name: getExplorePanel
	// desc: Returns the HTML for the Explore tab panel
	// date: May 2005
	// auth: William Cava <william.cava@ektron.com>
	//
	
	getExplorePanel: function()
	{
		return ExplorePanel.getExplorePanel();
	},
	
	//////////
	//
	// name: getSearchPanel
	// desc: Returns the HTML for the Search tab panel
	// date: May 2005
	// auth: William Cava <william.cava@ektron.com>
	//
	
	getSearchPanel: function()
	{
		return SearchPanel.getSearchPanel();
	},

	//////////
	//
	// name: loadURL
	// desc: Given a URL, this function loads URL in main window
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	loadURL: function( url )
	{
		setTimeout(
			function() {
				try {
					mainWindow.document.location.href = url;
				} catch( e ) {
					LogUtil.addMessage(
						LogUtil.CRITICAL,
						"Explorer",
						"loadURL",
						"Error trying to load '" + url + "' " +
						"with message: " + e.message );
				}
			}, 50
		);
	},
	
	//////////
	//
	// name: loadMainURL
	// desc: Loads main.html into the main window
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	loadMainURL: function()
	{
		var mainURL = ExplorerConfigUtil.getSiteRoot();
		mainURL += ExplorerConfigUtil.getParam( "exp_http_root" );
		mainURL += "/main.html";

		var currentURL = new String( Explorer.getURL() ).toLowerCase();
		var sMainURL = new String( mainURL ).toLowerCase();

		if( sMainURL != currentURL ) {
			Explorer.loadURL(mainURL);
		}
	},

	//////////
	//
	// name: getURL
	// desc: Gets the main window's current URL
	//
	
	getURL: function()
	{
		var url = null;

		try {
			url = window.external.EktGetUrl();
		} catch( e ) {
			LogUtil.addMessage(
				LogUtil.WARNING,
				"Explorer",
				"getURL",
				"Failed to get current URL " +
				"with message: " + e.message );
		}
		
		return url;
	},

	//////////
	//
	// name: getViews
	// desc: Reads the view definition file from the server
	//
	getViews: function()
	{
		// this will be fetched off of the server
		if( true ) //if( this.views == null )
		{
			//this.views = this.viewManager.getViews();
		}

		return this.views;
	},
	
	//////////
	//
	// name: defineDefaultContextMenu
	// desc: defines the default context menu for the application
	//
	defineDefaultContextMenu: function()
	{
		var list = []; //window.external.EktGetConfigList()
		var arrlist = new String( list ).split(",");
		var currentName = "My Configuration"; //window.external.EktGetConfigName();
		var configMenu = null;
		
		if( arrlist.length > 1 ) {
			var configMenu = new ContextMenu( "subMenu2" );
			for( var i = 0; i < arrlist.length; i++ ) {
				var name = arrlist[i];
				var handler = function( args, vars ) {
					//window.external.EktSetandUseConfig( vars["name"] );
				}
				var hashvars = {"name": name};
				if( name == currentName ) {
					name = "<b>" + name + "</b>";
				}
				configMenu.addItem( name, handler, hashvars );
			}
		}
	
		var s = ( 1 == 1 ? "s" : "" );
		var menu = new ContextMenu( "default" );
		menu.addItem("Manage Configuration" + s, Explorer.openConfigManager );
		if( configMenu ) {
			menu.addMenu("Use Configuration", configMenu );
		}
		menu.addBreak();
		menu.addItem("Refresh", Explorer.refresh );
		menu.addBreak();
		menu.addItem("Properties", Explorer.showPropertiesWindow );
		Explorer.ctxMenu = menu;
		ContextMenuUtil.add( Explorer.ctxMenu );
	},
	
	//////////
	//
	// name: openWindow
	// desc: Opens a new window, populating the new window with the
	//       data passed to the function
	//
	openWindow: function( data, settings )
	{
		var defaultSettings = "resizable=yes,height=450,width=550,toolbar=no,scrollbars=auto,menubar=no";
		settings = settings ? settings : defaultSettings;
		var rand = new String( Math.random() ).replace( ".", "" );

		var newWindow = window.open( "", "openWindow" + rand, settings );
		newWindow.document.write( unescape(data) );
	},
	
	//////////
	//
	// name: refresh
	// desc: Loads the current configuration from the registry and 
	//		 initiates a reload of the Ektron Explorer window.
	//
	refresh: function()
	{
		try {
			window.external.EktUseCurrentConfig()
			window.location.href = window.location.pathname;
		} catch( e ) {
			LogUtil.addMessage( 
				LogUtil.WARNING,
				"Explorer",
				"refresh",
				"Failed calling EktUseCurrentConfig: " +
				e.message );
		}
	},

	//////////
	//
	// name: openWorkArea
	// desc: Opens the work area in an external window
	//
	openWorkArea: function()
	{
		throw "Not implemented";
	},

	//////////
	//
	// name: showFolderPropertiesWindow
	// desc: Opens a window showing the current configuration
	//
	showFolderPropertiesWindow: function( folderId )
	{
		var propertiesWindow = new FolderPropertiesWindow( folderId )
		propertiesWindow.show();
	},
	
	//////////
	//
	// name: showPropertiesWindow
	// desc: Opens a window showing the current configuration
	//
	showPropertiesWindow: function()
	{
		var type = "Ektron Explorer Properties";
		var data = {};
		
		data["version"]  = Explorer.getCurrentVersion();
		data["config"]   = Explorer.getConfigName();
		data["location"] = Explorer.getLocation();
		data["username"] = Explorer.getUserName();
		
		var cookies = StringUtil.parseNameValuePairs( document.cookie );
		for( var k in cookies ) {
			if( cookies[k] != "" ) {
				data[k] = cookies[k];
			}
		}

		var propertiesWindow = new PropertiesWindow( type, data );
		propertiesWindow.show();
	},

	//////////
	//
	// name: openConfigManager
	// desc: Opens the configuration manager utility
	//
	openConfigManager: function()
	{
		try {
			window.external.EktLaunchConfig();
		} catch( e ) {
			LogUtil.addMessage(
				LogUtil.CRITICAL,
				"Explorer",
				"openConfigurationManager",
				"failed calling EktLaunchConfig: " +
				e.message
			);
		}
	},
	
	//////////
	//
	// name: getConfigName
	// desc: Returns the current config name in use
	//
			
	getConfigName: function()
	{
		var name = "unknown";
		try {
			name = window.external.EktGetConfigName();
		} catch( e ) {
			; // noop
		}
		
		return name;
	},	
	
	//////////
	//
	// name: getCurrentVersion
	// desc: Returns the current version of ektbartb
	//
		
	getCurrentVersion: function()
	{
		var version = "unknown";
		try {
			version = window.external.EktGetCurrentVersion();
		} catch( e ) {
			; // noop
		}
		
		return version;
	},	
	
	//////////
	//
	// name: getLocation
	// desc: Returns the CMS site root
	//
	
	getLocation: function()
	{
		var location = "unknown";
		try {
			location = window.external.EktGetLocation();
		} catch( e ) {
			; // noop
		}
		
		return location;
	},	
	
	//////////
	//
	// name: getUserName
	// desc: Returns the name of the user from the current configuration
	//
	getUserName: function()
	{
		var username = "admin";
		try {
			username = window.external.EktGetName();
		} catch( e ) {
			; // noop
		}
		
		return username;
	},

	//////////
	//
	// name: getPassword
	// desc: Returns the password of the user from the current configuration
	//
	getPassword: function()
	{
		var password = "admin";
		try {
			password = window.external.EktGetPassword();
		} catch( e ) {
			; // noop
		}

		return password;
	},
	
	//////////
	//
	// name: getDefaultContextMenu
	// desc: returns the default context menu object
	//
	getDefaultContextMenu: function()
	{
		return Explorer.defaultContextMenu;
	},
	
	//////////
	//
	// private members
	//

	views: null,
	//viewManager: new ViewManager(),
	explorerTabs: null,
	ctxMenu: null
}



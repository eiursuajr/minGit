//////////
//
// name: SearchRequest
// desc: Encapsulates information sent to the SearchService API, and
//		 provides functions for generating XML request packet.
//
//

function SearchRequest()
{
	//////////
	//
	// public members
	//
	
	this.getRequestXML	= __SearchRequest_getRequestXML;
	this.setParam		= __SearchRequest_setParam;
	this.getParam		= __SearchRequest_getParam;
	this.toString		= __SearchRequest_toString;

	//////////
	//
	// private members
	// 
	
	this.version = "0.1";
	this.params     = new Array();
	this.maxnumber	= null;
	this.folderpath	= null;
	this.recursive	= null;
	this.searchtext	= null;
	this.partialwordmatch = null;
}

function __SearchRequest_setParam( n, v )
{
	this.params[n] = v;
}

function __SearchRequest_getParam( n )
{
	return this.params[n];
}

function __SearchRequest_toString()
{
	return this.getRequestXML();
}

function __SearchRequest_getRequestXML()
{
	var buffer = "";
	buffer += "<searchRequest version='0.1'>";
	buffer += "<config>";
	
	for( var k in this.params ) {
		if( this.params[k] != null ) {
			buffer += "<param name='" + k + "' value='" + this.params[k] + "'/>";
		}
	}
	
	buffer += "</config>";
	buffer += "</searchRequest>";
	
	return buffer;	
}

//////////
//
// name: SearchManager
// desc: Manages search functions
//

var SearchManager =
{
	//////////
	//
	// name: execute
	// desc: execute receives a string of name value pairs that define
	//		 the search query, converts the query into a SearchRequest
	//		 object, executes a search using the toolkit.
	//

	execute: function( query )
	{
		var mainURL = ExplorerConfigUtil.getSiteRoot();
		mainURL += ExplorerConfigUtil.getParam( "exp_http_root" );
		mainURL += "/main.html";

		var currentURL = new String( Explorer.getURL() ).toLowerCase();
		var sMainURL = new String( mainURL ).toLowerCase();

		if( sMainURL != currentURL ) {
			var shouldBlock = true;
			Explorer.loadURL(mainURL, shouldBlock);
			setTimeout( "SearchManager.execute( '" + query + "' )", 250 );
			return;
		}

		var hash = StringUtil.parseNameValuePairs( query );
		var id = hash["id"];
		var qtext = hash["qtext"];
		var qtitle = hash["qtitle"];
		var folderid = hash["folderid"] ? hash["folderid"] : 0;

		var searchResult = null;

		if( id != null ) {
			searchRequest = new SearchRequest();
			searchRequest.setParam( "searchtext", "" );
			searchRequest.setParam( "folderid", id );
			searchRequest.setParam( "recursive", "false" );
			searchRequest.setParam( "searchtype", "ALL" );
			searchRequest.setParam( "partialwordmatch", "true" );
		}

		if( ( qtext != null ) || ( qtitle != null ) ) {

			var types = { "a": "ALL", "n": "ANY", "e": "EXACT" }
			var subFolders  = hash["sf"];
			var searchType  = hash["searchtype"];
			var partialWord = hash["pw"];

			searchRequest = new SearchRequest();
			searchRequest.setParam( "searchtext", qtext );
			searchRequest.setParam( "searchtitle", qtitle );
			searchRequest.setParam( "recursive", subFolders != "false" ? "true" : "false" );
			searchRequest.setParam( "folderid", folderid );
			searchRequest.setParam( "searchtype", types[searchType] );
			searchRequest.setParam( "partialwordmatch", (partialWord == "on" || partialWord == "true") ? "true" : "false" );
		}
		
		if( searchRequest != null ) {
			try {
				var resultContainer = mainWindow.document.getElementById( "iconListOutput" );
				var statusContainer = mainWindow.document.getElementById( "iconListStatus" );
				resultContainer.innerHTML = "<div style='padding:2px;margin-left:2px;color:#c0c0c0;'>Loading ...</div>";
				statusContainer.innerHTML = "";
				document.body.style.cursor = "wait";
				mainWindow.document.body.style.cursor = "wait";
				toolkit.executeSearch( searchRequest, SearchManager.displayResults );
			} catch( e ) {

				LogUtil.addMessage(
					LogUtil.WARNING,
					"SearchManager",
					"execute",
					"exception thrown: " +
					e.description +
					" - will attempt retry in 1000 ms" );

				setTimeout( function() {
					SearchManager.execute( query );
					}, 1000 );

			}
		}
	},
	
	//////////
	//
	// name: displayResults
	// desc: Displays result set in main window
	//
	
	displayResults: function( searchResult )
	{
		if( searchResult ) {
			var resultContainer = mainWindow.document.getElementById( "iconListOutput" );
			var statusContainer = mainWindow.document.getElementById( "iconListStatus" );

			if( resultContainer ) {
				if( statusContainer ) {
					var iconList = new IconList( searchResult.assets );
					iconList.setResultContainer( resultContainer );
					iconList.setStatusContainer( statusContainer );
					iconList.display();
				}
			}
		}
		document.body.style.cursor = "default";
		mainWindow.document.body.style.cursor = "default";
	}
}








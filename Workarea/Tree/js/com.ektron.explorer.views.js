//////////
//
// name: View
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function View( title, body, url )
{
	this.title = title;
	this.body  = body;
	this.url   = url;
}

//////////
//
// name: ViewManager
// desc: Manages loading views
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function ViewManager()
{
	//////////
	//
	// public members:
	//

	this.load         = __ViewManager_load;
	this.loadView     = __ViewManager_loadView;
	this.loadDocument = __ViewManager_loadRemoteDocument;
	this.getViews	  = __ViewManager_getViews;
	
	//////////
	//
	// private members
	//
	
	this.http = new HttpProxy();
	this.data = new Array();
	
	//////////
	//
	// constructor
	//
	{
		this.load();
	}
}

//////////
//
// name: getViews
// desc: Returns an array of views.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005

function __ViewManager_getViews()
{
	return this.data;
}

//////////
//
// name: load
// desc: Retrieves the view definition file from the server, and
//		 loads each view into a view object. This way, once a CMS
//		 server is updated, each client is automatically updated.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005

function __ViewManager_load()
{
	var base = ExplorerConfigUtil.getSiteRoot();
	var view = ExplorerConfigUtil.getParam("exp_http_root");

	var index      = this.loadDocument( base + view + "/views/view.xml" );
	var viewNodes  = index.selectNodes( "/explorer/views/view" );

	for( var i = 0; i < viewNodes.length; i++ ) {
		var viewNode = viewNodes[i];
		var title =  viewNode.getAttribute( "name" );
		var url   =  viewNode.getAttribute( "url" );
		var body = this.loadView( baseURL + url );

		var view = new View( title, body, url );
		view.url   = url;
		view.body  = body;
		view.title = title;

		this.data[this.data.length] = view;
	}
}

function __ViewManager_loadView( url )
{
	var view = this.loadDocument( url );
	return view.xml;
}

//////////
//
// this really shouldn't be here-- clean it up
//

function __ViewManager_loadRemoteDocument( url )
{
	this.http.open( url, false, "GET" );
	this.http.send();

	/*  check for http success */
	var status = this.http.getStatus();

	if( status != 200 ) {
		var err  = "HTTP error: expected 200, received " + status;
		LogUtil.addMessage( LogUtil.CRITICAL,
				"CMSAPI",
				"execute()" +
				url,
				err );
		// todo: need to define the types of exceptions that can be returned
		// throw Exception.HTTPError(err);
		// raise EngineException.AlreadyLoggedIn(self, "already logged in", None)
	}

	var document = this.http.getDocument();

	// verify that we have a document
	if( !document ) {
		// null document
		LogUtil.addMessage( LogUtil.CRITICAL,
				"CMSAPI",
				"execute()" +
				url,
				"document is null" );
	}

	// verify that we have well formed xml
	if( document.parseError != 0 ) {
		// some type of parse error, handle accordingly
		LogUtil.addMessage( LogUtil.CRITICAL,
				"CMSAPI",
				"execute()" +
				url,
				"XML parse error: " +
				StringUtil.HTMLEncode(
					document.parseError.srcText )
				);
	}

	return document;
}

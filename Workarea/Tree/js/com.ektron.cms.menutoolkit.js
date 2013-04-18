//////////
//
// name: EktronToolkit
// desc: A client side toolkit for interacting with the CMS WebServices
// auth: William Cava <william.cava@ektron.com>
// date: April 2005, May 2005
// 

function EktronToolkit()
{
	//////////
	//
	// public members
	//

	this.executeSearch	= __EktronToolkit_executeSearch;
	this.getChildContent	= __EktronToolkit_getChildContent;
	this.getChildFolders	= __EktronToolkit_getChildFolders;
	this.getContent		= __EktronToolkit_getContent;
	this.getRootFolder	= __EktronToolkit_getRootFolder;
	this.getFolder      = __EktronToolkit_getFolder;
	this.getMenu        = __EktronToolkit_getMenu;
	this.getChildMenus  = __EktronToolkit_getChildMenus;
	this.api = new CMSAPI();
}

function __EktronToolkit_getMenu( id, callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getMenu()",
		"Starting." );

	// function pointer to object creator
	var factory  = AssetFactory.create;
	var postdata = { "id": id };
	var apicall  = this.api.lookup( "get_menu" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getMenu()",
		"Returning." );
}

function __EktronToolkit_getChildMenus( id, callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getChildMenus()",
		"Starting." );

	// function pointer to object creator
	var factory  = AssetListFactory.create;
	var postdata = { "id": id };
	var apicall  = this.api.lookup( "get_child_menus" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getChildMenus()",
		"Returning." );
}

function __EktronToolkit_getRootFolder( callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getRootFolder()",
		"Starting." );

	// function pointer to object creator
	var factory  = AssetFactory.create;
	var postdata = { "id": 0 };
	var apicall  = this.api.lookup( "get_folder" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getRootFolder()",
		"Returning." );
}

function __EktronToolkit_getFolder( id, callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getFolder()",
		"Starting." );

	// function pointer to object creator
	var factory  = AssetFactory.create;
	var postdata = { "id": id };
	var apicall  = this.api.lookup( "get_folder" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getFolder()",
		"Returning." );
}

function __EktronToolkit_getChildFolders( id, callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getChildFolders(" + id + ")",
		"Starting." );

	var factory  = AssetListFactory.create;
	var postdata = { "folderId": id };
	var apicall  = this.api.lookup( "get_child_folders" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getChildFolders(" + id + ")",
		"Returning." );
}

function __EktronToolkit_getContent( id, callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getContent(" + id + ")",
		"Starting." );

	var factory  = AssetFactory.create;
	var postdata = { "id": id };
	var apicall  = this.api.lookup( "get_content" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );
	var params   = apicall[1];

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getContent(" + id + ")",
		"Returning." );
}

function __EktronToolkit_getChildContent( id, callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getChildContent(" + id + ")",
		"Starting." );

	var factory  = AssetListFactory.create;
	var postdata = { "folderId": id };
	var apicall  = this.api.lookup( "get_child_content" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );
	var params   = apicall[1];

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getChildContent(" + id + ")",
		"Returning." );
}

function __EktronToolkit_executeSearch( query, callback, vargs )
{
	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"executeSearch(" + StringUtil.HTMLEncode(query) + ")",
		"Starting." );

	var factory  = AssetListFactory.create;
	var postdata = { "query": query };
	var apicall  = this.api.lookup( "search" );
	var document = this.api.execute( apicall, postdata, factory, callback, vargs );
	var params   = apicall[1];

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"executeSearch(" + StringUtil.HTMLEncode(query) + ")",
		"Returning." );
}

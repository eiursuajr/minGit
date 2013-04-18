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
    this.getRootFolder          = __EktronToolkit_getRootFolder;
	this.getChildFolders	    = __EktronToolkit_getChildFolders;	
	this.getFolder              = __EktronToolkit_getFolder;
	this.getTaxonomy            = __EktronToolkit_getTaxonomy;
	this.getTaxonomies          = __EktronToolkit_getTaxonomies;
	this.getAllSubCategory      = __EktronToolkit_getAllSubCategory;
	this.getCollections         = __EktronToolkit_getCollections;
	this.getMenus               = __EktronToolkit_getMenus;
	this.getSubMenus            = __EktronToolkit_getSubMenus;	
	this.api = new CMSAPI();
}

function __EktronToolkit_getRootFolder(callback, vargs) 
{
	// function pointer to object creator
	var factory  = AssetFactory.create;
	var postdata = { "id": 0 };
	var apicall = this.api.lookup("get_folder");
	var document = this.api.execute(apicall, postdata, factory, callback, vargs, 0);	

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getRootFolder()",
		"Returning." );
}

function __EktronToolkit_getFolder( id, callback, vargs )
{
	// function pointer to object creator
	var factory  = AssetFactory.create;
	var postdata = { "id": id };
	var apicall = this.api.lookup("get_folder");
	var document = this.api.execute(apicall, postdata, factory, callback, vargs, 0);

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getRootFolder()",
		"Returning." );
}

function __EktronToolkit_getChildFolders(id, langid, callback, vargs) 
{
    // function pointer to object creator
	var factory  = AssetListFactory.create;
	var postdata = { "folderId": id , "langid" : langid };
	var apicall = this.api.lookup("get_child_folders");
	var document = this.api.execute(apicall, postdata, factory, callback, vargs, 0);

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getChildFolders(" + id + ")",
		"Returning." );
}

function __EktronToolkit_getAllSubCategory(id, langid, callback, vargs) 
{
    // function pointer to object creator
	var factory  = AssetListFactory.create;
	var postdata = { "taxonomyid": id , "folderid" : __EkFolderId , "langid" : langid , "taxonomyoverrideid" : __TaxonomyOverrideId};
	var apicall  = this.api.lookup( "get_child_category" );
	var document = this.api.execute(apicall, postdata, factory, callback, vargs, -1);	

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getAllSubCategory(" + id + ")",
		"Returning." );
	__EkFolderId=-1;//Only once required.
}

function __EktronToolkit_getTaxonomy(id, langid, callback, vargs) 
{
    // function pointer to object creator
    var factory  = AssetFactory.create;
	var postdata = { "taxonomyid": id , "langid" : langid };
	var apicall  = this.api.lookup( "get_taxonomy" );
	var document = this.api.execute(apicall, postdata, factory, callback, vargs, -1);	

	LogUtil.addMessage( LogUtil.DEBUG,
		"EktronToolkit",
		"getTaxonomy()",
		"Returning." );
}

function __EktronToolkit_getTaxonomies(langid, callback, vargs) 
{
    // function pointer to object creator
    var factory = AssetListFactory.create;
    var postdata = { "taxonomyid": 0 , "langid" : langid };
    var apicall = this.api.lookup("get_taxonomies");
    var document = this.api.execute(apicall, postdata, factory, callback, vargs, -1);
    
    LogUtil.addMessage(LogUtil.DEBUG,
		"EktronToolkit",
		"getTaxonomies()",
		"Returning.");
}

function __EktronToolkit_getCollections(langid, callback, vargs)
{
    // function pointer to object creator
    var factory = AssetListFactory.create;
    var postdata = { "collectionid": 0 , "langid" : langid };
    var apicall = this.api.lookup("get_collections");
    var document = this.api.execute(apicall, postdata, factory, callback, vargs, -2);

    LogUtil.addMessage(LogUtil.DEBUG,
		"EktronToolkit",
		"getCollections()",
		"Returning.");
}

function __EktronToolkit_getMenus(langid, callback, vargs)
{
    // function pointer to object creator
    var factory = AssetListFactory.create;
    var postdata = { "menuid": 0 , "langid" : langid };
    var apicall = this.api.lookup("get_menus");
    var document = this.api.execute(apicall, postdata, factory, callback, vargs, -3);

    LogUtil.addMessage(LogUtil.DEBUG,
		"EktronToolkit",
		"getMenus()",
		"Returning.");
}

function __EktronToolkit_getSubMenus(id, langid, callback, vargs)
{
    // function pointer to object creator
    var factory = AssetListFactory.create;
    var postdata = { "menuid": id , "langid" : langid };
    var apicall = this.api.lookup("get_submenus");
    var document = this.api.execute(apicall, postdata, factory, callback, vargs, -3);

    LogUtil.addMessage(LogUtil.DEBUG,
		"EktronToolkit",
		"getSubMenus()",
		"Returning.");
}
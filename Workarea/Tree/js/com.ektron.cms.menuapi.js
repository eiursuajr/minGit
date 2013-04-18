//////////
//
// name: CMSAPI
// desc: The CMS Service exposes two interfaces, a SOAP interface and
//		 an XML interface. The primary purpose of this class is to
//		 encapsulate the mapping from function name to resource and
//		 response structure. If the CMS Service should make cosmetic
//		 changes to its response XML structure, this is the only file
//		 that should need updating.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

var cms_api_meth	= ExplorerConfigUtil.getParam( "cms_svc_method" );
var cms_api_host	= URLUtil.getHostName( document.location );
var cms_api_async   = false;

function CMSAPI()
{
	//////////
	//
	// public members
	//

	this.execute	 = __CMSAPI_execute;
	this.lookup		 = __CMSAPI_lookup;
	this.setUserName = __CMSAPI_setUserName;
	this.setPassword = __CMSAPI_setPassword;
	this.initialize  = __CMSAPI_initialize;
	this.setHostname = __CMSAPI_setHostname;
	this.setDomain = __CMSAPI_setDomain;

	//////////
	//
	// private methods
	//

	this._buildRequestURL  = __CMSAPI_buildRequestURL;

	//////////
	//
	// private properties
	//

	this.map;
	this.username;
	this.password;
	this.domain;
	this.userid;
	this.sitepath;
	this.preview;
	this.sitelang;

	//////////
	//
	// initialize 
	//

	AsyncHttpUtil.setPoolSize( 5 );

	this.map =
	{
		"get_menu" :
					[   "/" + URLUtil.ek_appPath + "MenuTreeData.aspx",
						{
							"id": "/AxMenuData/TreeID/text()",
							"menuID": "/AxMenuData/ID/text()",
							"title": "/AxMenuData/Title/text()",
							"hasChildren": "/AxMenuData/HasChildren/text()",
							"itemType": "/AxMenuData/Type/text()",
							"template": "/AxMenuData/Template/text()",
							"link": "/AxMenuData/Link/text()",
							"parentID": "/AxMenuData/ParentID/text()",
							"ancestorID": "/AxMenuData/AncestorID/text()",
							"folderID": "/AxMenuData/FolderID/text()",
							"language": "/AxMenuData/ContentLanguage/text()",
							"itemID": "/AxMenuData/ID/text()"
						}
					],
		"get_child_menus" :
					[   "/" + URLUtil.ek_appPath + "MenuTreeData.aspx",
						{
							"children": "/AxMenuData/Item",
							"childmap":
							{
								"id": "./TreeID/text()",
								"menuID": "./ID/text()",
								"name": "./ItemTitle/text()",
								"image": "./ItemImage/text()",
								"hasChildren": "./HasChildren/text()",
								"itemType": "./ItemType/text()",
								"link": "./ItemLink/text()",
								"ancestorID": "./AncestorID/text()",
								"folderID": "./FolderID/text()",
								"language": "./ContentLanguage/text()",
								"itemID": "./ItemID/text()"
							}
						}
					],
		"get_content" :
					[	"/ContentService.asmx/GetContent",
						{	
							"id": "/ContentData/Id/text()",
							"title": "/ContentData/Title/text()",
							"languageid": "/ContentData/LanguageId/text()",
							"editorFirstName": "/ContentData/EditorFirstName/text()",
							"editorLastName": "/ContentData/EditorLastName/text()",
							"lastModified": "/ContentData/DisplayLastEditDate/text()",
							"dateCreated": "/ContentData/DisplayDateCreated/text()",
							"username": "/ContentData/UserName/text()",
							"isPublished": "/ContentData/IsPublished/text()",
							"type": "/ContentData/Type/text()",
							"content": "/ContentData/Html/text()"
						}
					],

		"get_child_content" :
					[	"/ContentService.asmx/GetChildContent",
						{	
							"children": "/ArrayOfContentData/ContentData",
							"childmap":
							{
								"id": "./Id/text()",
								"title": "./Title/text()",
								"languageid": "./LanguageId/text()",
								"editorFirstName": "./EditorFirstName/text()",
								"editorLastName": "./EditorLastName/text()",
								"lastModified": "./DisplayLastEditDate/text()",
								"dateCreated": "./DisplayDateCreated/text()",
								"username": "./UserName/text()",
								"isPublished": "./IsPublished/text()",
								"type": "./Type/text()",
								"content": "./Html/text()",
								"teaser": "./Teaser/text()"
							}
						}
					],

		"get_folder" :
					[	"/ContentService.asmx/GetFolder",
						{	
							"id": "/FolderData/Id/text()",
							"description": "/FolderData/Description/text()",
							"hasChildren": "/FolderData/HasChildren/text()",
							"name": "/FolderData/Name/text()",
							"nameWithPath": "/FolderData/NameWithPath/text()",
							"isContentEditable": "/FolderData/Permissions/CanEdit/text()",
							"canTraverse": "/FolderData/Permissions/CanTraverseFolders/text()",
							"canRead": "/FolderData/Permissions/IsReadOnly/text()",
							"canReadLib": "/FolderData/Permissions/IsReadOnlyLib/text()",
							"canEdit": "/FolderData/Permissions/CanPublish/text()"
						}
					],

		"get_child_folders" :
					[	"/ContentService.asmx/GetChildFolders",
						{	
							"children": "/ArrayOfFolderData/FolderData",
							"childmap":
							{
								"id": "./Id/text()",
								"description": "./Description/text()",
								"hasChildren": "./HasChildren/text()",
								"name": "./Name/text()",
								"nameWithPath": "./NameWithPath/text()",
								"isContentEditable": "./Permissions/CanEdit/text()",
								"canTraverse": "./Permissions/CanTraverseFolders/text()",
								"canRead": "./Permissions/IsReadOnly/text()",
								"canReadLib": "./Permissions/IsReadOnlyLib/text()",
								"canEdit": "./Permissions/CanPublish/text()"
							}
						}
					],

		"search" :
					[	"/SearchService.asmx/ExecuteSearch",
						{
							"children": "/ArrayOfSearchContentItem/SearchContentItem",
							"childmap":
							{
								"id": "./ID/text()",
								"language": "./LanguageID/text()",
								"title": "./Title/text()",
								"contentText": "./ContentText/text()",
								"dateCreated": "./DisplayDateCreated/text()",
								"lastEditorFirstName": "./LastEditorFname/text()",
								"lastEditorLastName": "./LastEditorLname/text()",
								"dateModified": "./DisplayDateModified/text()",
								"templateFileName": "./TemplateFileName/text()",
								"quickLink": "./QuickLink/text()",
								"teaser": "./Teaser/text()",
								"folderId": "./FolderID/text()",
								"folderPath": "./FolderPath/text()",
								"type": "./ContentType/text()",
								"canEdit": "./Permissions/CanEdit/text()"
							}
						}
					]
	}
	
	// constructor
	{
		this.initialize();
	}
}

//////////
//
// name: lookup
// desc: Given a name, lookup returns an APICall object
//		 describing the parameters needed for interfacing
//		 with the CMS Service.
// auth: William Cava
// date: April 2005
//

function __CMSAPI_lookup( functionName )
{
	return this.map[functionName];
}

//////////
//
// name: execute
// desc: Given an apicall and postdata, executes a call to the
//		 CMS service opening a connection to the resource 
//		 specified in the apicall object. If the request is
//		 succesful, the response is deserialized into an object
//		 and returned to the caller.
// auth: William Cava
// date: April 2005
//

function __CMSAPI_execute( apicall, postdata, factory, callback, vargs )
{
	var url = this._buildRequestURL( apicall[0] );

	var httpRequest = new HttpRequest();
	httpRequest.setType( HttpRequest.POST );
	httpRequest.setUrl( url );
	httpRequest.setHeaders( {
			"Content-Type": "application/x-www-form-urlencoded"
			} );
	httpRequest.setData( postdata );

	AsyncHttpUtil.request( httpRequest, function() {
			var msg = "";

			var httpRequest, httpProxy, params, document;
			try { httpRequest = arguments[0]; } catch( e ) { msg += "get httpRequest failed"; }
			try { httpProxy   = arguments[1]; } catch( e ) { msg += "get httpProxy failed"; }
			try { params = apicall[1]; } catch( e ) { msg += "get params failed"; }
			try { document = httpProxy.getDocument(); } catch( e ) { msg += "getDocument() failed"; }
			try { msg += "callback=<pre>" + callback + "</pre> " } catch( e ) {}
			try { msg += "factory=<pre>" + factory + "</pre> " } catch( e ) {}

			try {
				callback( factory( params, document ), vargs );
			} catch( e ) {
				LogUtil.addMessage(
					LogUtil.CRITICAL,
					"CMSAPI",
					"execute",
					"callback failed: " + msg + " -- message: " + e.message );
			}
		}
	); // end of AsyncHttpUtil.request
}

function __CMSAPI_buildRequestURL( serviceURL )
{
	var url  = URLUtil.getSiteRoot( document.location ) + serviceURL;
	return url;
}

function __CMSAPI_setUserName( username )
{
	this.username = username;
}

function __CMSAPI_setPassword( password )
{
	this.password = password;
}

function __CMSAPI_setHostname( hostname )
{
	this.hostname = hostname;
}

function __CMSAPI_setDomain( hostname )
{
	this.domain = domain;
}

function __CMSAPI_initialize()
{
	try {
		/*
		this.username = Explorer.getUserName();
		this.password = Explorer.getPassword();
		this.domain = Explorer.getDomain();
		this.hostname = Explorer.getHostName();

		var ecm = CookieUtil.getCookie( "ecm" );
		this.userid   = ecm["user_id"] ? ecm["user_id"] : "";
		this.sitepath = ecm["site_id"] ? ecm["site_id"] : "";
		this.preview  = ecm["site_preview"] ? ecm["site_preview"] : "";
		this.sitelang = ecm["SiteLanguage"] ? ecm["SiteLanguage"] : "";
		*/
	} catch( e ) {
		LogUtil.addMessage(
			LogUtil.WARNING,
			"CMSAPI",
			"initialize",
			"Failed to initialize: " + 
			e.description );
	}
}





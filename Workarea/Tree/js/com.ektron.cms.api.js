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
// modified auth: Udaiappa Ramachandran  <udai.ramachandran@ektron.cm>
// date: July 2005
var cms_api_meth = "POST"; //ExplorerConfigUtil.getParam( "cms_svc_method" );
var cms_api_async = false;
var cms_api_appPath = "";
try
{
    if ((typeof ____ek_appPath2 !== "undefined") && (____ek_appPath2 != null))
        cms_api_appPath = ____ek_appPath2;
} catch (e) { }

function CMSAPI()
{
    //////////
    //
    // public members
    //

    this.execute = __CMSAPI_execute;
    this.lookup = __CMSAPI_lookup;
    this.initialize = __CMSAPI_initialize;

    //////////
    //
    // private methods
    //

    this._buildRequestURL = __CMSAPI_buildRequestURL;

    //////////
    //
    // private properties
    //

    this.map;

    //////////
    //
    // initialize
    //

    AsyncHttpUtil.setPoolSize(5);

    this.map =
	{
	    "get_folder":
					[cms_api_appPath + "WorkAreaTrees.aspx?method=get_folder",
						{
						    "id": "/FolderData/Id/text()",
						    "description": "/FolderData/Description/text()",
						    "hasChildren": "/FolderData/HasChildren/text()",
						    "name": "/FolderData/Name/text()",
						    "nameWithPath": "/FolderData/NameWithPath/text()",
						    "grantTraverse": "/FolderData/Permissions/CanTraverseFolders/text()",
						    "grantRead": "/FolderData/Permissions/IsReadOnly/text()",
						    "grantReadLib": "/FolderData/Permissions/IsReadOnlyLib/text()",
						    "type": "/FolderData/FolderType/text()",
						    "parentid": "/FolderData/ParentId/text()"
						}
					],

	    "get_child_folders":
					[cms_api_appPath + "WorkAreaTrees.aspx?method=get_child_folders",
						{
						    "children": "/ArrayOfFolderData/FolderData",
						    "childmap":
							{
							    "id": "./Id/text()",
							    "description": "./Description/text()",
							    "hasChildren": "./HasChildren/text()",
							    "name": "./Name/text()",
							    "nameWithPath": "./NameWithPath/text()",
							    "grantTraverse": "./Permissions/CanTraverseFolders/text()",
							    "grantRead": "./Permissions/IsReadOnly/text()",
							    "grantReadLib": "./Permissions/IsReadOnlyLib/text()",
							    "type": "./FolderType/text()",
							    "parentid": "/FolderData/ParentId/text()"
							}
						}
					],
	    "get_taxonomy":
					[
					cms_api_appPath + "WorkAreaTrees.aspx?method=get_taxonomy",
						{
						    "id": "/TaxonomyData/Id/text()",
						    "parentid": "/TaxonomyData/TaxonomyParentId/text()",
						    "description": "/TaxonomyData/TaxonomyDescription/text()",
						    "title": "/TaxonomyData/TaxonomyName/text()",
						    "name": "/TaxonomyData/TaxonomyName/text()",
						    "nameWithPath": "/TaxonomyData/TaxonomyPath/text()",
						    "count": "/TaxonomyData/TaxonomyItemCount/text()",
						    "hasChildren": "/TaxonomyData/TaxonomyHasChildren/text()",
						    "visible": "/TaxonomyData/Visible/text()"
						}
					],
	  "get_child_category":
					[cms_api_appPath + "WorkAreaTrees.aspx?method=get_child_category",
						{
						    "children": "/ArrayOfTaxonomyBaseData/TaxonomyBaseData",
						    "childmap":
							{
							    "id": "./TaxonomyId/text()",
							    "langid": "./TaxonomyLanguage/text()",
							    "parentid": "./TaxonomyParentId/text()",
							    "description": "./TaxonomyDescription/text()",
							    "title": "./TaxonomyName/text()",
							    "name": "./TaxonomyName/text()",
							    "nameWithPath": "./TaxonomyPath/text()",
							    "count": "./TaxonomyItemCount/text()",
							    "hasChildren": "./TaxonomyHasChildren/text()",
							    "visible": "./Visible/text()"
							}
						}
					],
	    "get_taxonomies":
					[
					    cms_api_appPath + "WorkAreaTrees.aspx?method=get_taxonomies",
						{
						    "children": "/ArrayOfTaxonomyBaseData/TaxonomyBaseData",
						    "childmap":
							{
							    "id": "/TaxonomyBaseData/Id/text()",
							    "langid": "/TaxonomyBaseData/TaxonomyLanguage/text()",
							    "parentid": "/TaxonomyBaseData/TaxonomyParentId/text()",
							    "description": "/TaxonomyBaseData/TaxonomyDescription/text()",
							    "title": "/TaxonomyBaseData/TaxonomyName/text()",
							    "name": "/TaxonomyBaseData/TaxonomyName/text()",
							    "nameWithPath": "/TaxonomyBaseData/TaxonomyPath/text()",
							    "count": "/TaxonomyBaseData/TaxonomyItemCount/text()",
							    "hasChildren": "/TaxonomyBaseData/TaxonomyHasChildren/text()",
							    "visible": "/TaxonomyBaseData/Visible/text()"
							}
						}
					],
	    "get_collections":
					[
					    cms_api_appPath + "WorkAreaTrees.aspx?method=get_collections",
						{
						    "children": "/ArrayOfCollectionListData/CollectionListData",
						    "childmap":
							{
							    // NOTE: Collections are not hierarchical, so there is no parent/child info
							    "id": "/CollectionListData/Id/text()",
							    "description": "/CollectionListData/Description/text()",
							    "title": "/CollectionListData/Title/text()",
							    "name": "/CollectionListData/Title/text()",
							    "nameWithPath": "/CollectionListData/Title/text()",
							    "folderid": "/CollectionListData/FolderId/text()",
							    "status": "/CollectionListData/Status/text()",
							    "approvalRequired": "/CollectionListData/ApprovalRequired/text()"
							}
						}
					],
	    "get_menus":
					[
					    cms_api_appPath + "WorkAreaTrees.aspx?method=get_menus",
						{
						    "children": "/ArrayOfAxMenuData/AxMenuData",
						    "childmap":
							{
							    "id": "/AxMenuData/ID/text()",
							    "langid": "/AxMenuData/ContentLanguage/text()",
							    "folderid": "/AxMenuData/FolderID/text()",
							    "description": "/AxMenuData/Description/text()",
							    "title": "/AxMenuData/Title/text()",
							    "name": "/AxMenuData/Title/text()",
							    "nameWithPath": "/AxMenuData/Title/text()",
							    "hasChildren": "/AxMenuData/HasChildren/text()",
							    "parentid": "/AxMenuData/ParentID/text()",
							    "ancestorid": "/AxMenuData/AncestorID/text()",
							    "itemCount": "/AxMenuData/ItemCount/text()"
							}
						}
					],
		"get_submenus":
					[
					    cms_api_appPath + "WorkAreaTrees.aspx?method=get_submenus",
						{
						    "children": "/ArrayOfAxMenuData/AxMenuData",
						    "childmap":
							{
							    "id": "/AxMenuData/ID/text()",
							    "langid": "/AxMenuData/ContentLanguage/text()",
							    "folderid": "/AxMenuData/FolderID/text()",
							    "title": "/AxMenuData/Title/text()",
							    "name": "/AxMenuData/Title/text()",
							    "nameWithPath": "/AxMenuData/Title/text()",
							    "hasChildren": "/AxMenuData/HasChildren/text()",
							    "parentid": "/AxMenuData/ParentID/text()",
							    "ancestorid": "/AxMenuData/AncestorID/text()",
							    "itemCount": "/AxMenuData/ItemCount/text()"
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

function __CMSAPI_lookup(functionName)
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

function __CMSAPI_execute(apicall, postdata, factory, callback, vargs, treeViewId)
{
    var url = this._buildRequestURL(apicall[0]);

    var httpRequest = new HttpRequest();
    httpRequest.setType(HttpRequest.POST);
    httpRequest.setUrl(url);
    httpRequest.setHeaders({ "Content-Type": "application/x-www-form-urlencoded" });
    httpRequest.setData(postdata);

    AsyncHttpUtil.request(httpRequest, function()
    {
        var msg = "";

        var httpRequest, httpProxy, params, doc;
        try { httpRequest = arguments[0]; } catch (e) { msg += "get httpRequest failed"; }
        try { httpProxy = arguments[1]; } catch (e) { msg += "get httpProxy failed"; }
        try { params = apicall[1]; } catch (e) { msg += "get params failed"; }
        try { doc = httpProxy.getDocument(); } catch (e) { msg += "getDocument() failed"; }
        try { msg += "callback=<pre>" + callback + "</pre> " } catch (e) { }
        try { msg += "factory=<pre>" + factory + "</pre> " } catch (e) { }

        try
        {
            // factory will be of type Asset or AssetList, depending on whether we are obtaining a single node, or a list of nodes
            var assetOrAssetList = factory(params, doc)
            callback(assetOrAssetList, vargs, treeViewId);
            if (typeof $ektron !== "undefined")
            {
                $ektron(document).trigger("CMSAPIAjaxComplete");
            }
        }
        catch (e)
        {
            //alert("Error");
            LogUtil.addMessage(
					LogUtil.CRITICAL,
					"CMSAPI",
					"execute",
					"callback failed: " + msg + " -- message: " + e.message);
        }
    }
	);   // end of AsyncHttpUtil.request
}

function __CMSAPI_buildRequestURL(serviceURL)
{
    return (serviceURL);
}

function __CMSAPI_initialize()
{
}
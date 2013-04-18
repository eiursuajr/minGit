//////////
//
// name: Asset
// desc: Definition of an asset object
// date: May 2005
// auth: William Cava <william.cava@ektron.com>
//

function Asset()
{
	//////////
	//
	// public methods
	//
	
	this.set		= __Asset_set;
	this.get		= __Asset_get;
	
	//////////
	//
	// private member variables;
	//
}

//////////
//
// name: AssetList
// desc: Definition of an assetlist object
// date: May 2005
// auth: William Cava <william.cava@ektron.com>
//

function AssetList()
{
	//////////
	//
	// public methods
	//
	
	this.set		= __Asset_set;
	this.get		= __Asset_get;
	this.add		= __AssetList_add;
	
	//////////
	//
	// private member variables;
	//
	
	this.assets		= new Array();
}

function __Asset_set( n, v )
{
	this[n] = v;
}

function __Asset_get( n )
{
	return this[n];
}

function __AssetList_add( asset )
{
	this.assets[this.assets.length] = asset;
}

//////////
//
// name: Asset, AssetList Factories
// desc: Static class factories given params and a document, returns
//       a new object of specfied type. 'Factory' is sort of a
//		 stretch here, since in addition to object creation,
//		 we're additionally dynamically populating the object
//		 with data available in the parameters.
//

var AssetFactory =
{
	//////////
	//
	// public methods
	//
	"create" : function( params, document ) {
		var parser  = new Parser(document);
		var asset = new Asset();
		for( var property in params ) {
			var xpath = params[property]
			asset.set( property, parser.getText( xpath ) );
		}
	
		return asset;
	}
}

var AssetListFactory =
{
	//////////
	//
	// public methods
	//
	"create" : function( params, document ) {
		var parser  = new Parser(document);
		var assetList = new AssetList();
		var childNodes = parser.getNodeList( params["children"] );
		for( var i = 0; i < childNodes.length; i++ ) {
			var childNode = childNodes[i];
			var asset = AssetFactory.create( params["childmap"], childNode );
			assetList.add( asset );
		}

		return assetList;
	}
}

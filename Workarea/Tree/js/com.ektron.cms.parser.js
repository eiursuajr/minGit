////////////
// name: Parser
// desc: Contains generic functions for parsing XML. This makes
//		 it easier to change the parsing functionality if needed
//		 without changing the users of this class.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function Parser( document )
{
	//////////
	//
	// public members
	//

	this.setDocument	 = __Parser_setDocument;
	this.getText		 = __Parser_getText;
	this.getBoolean		 = __Parser_getBoolean;
	this.getTextArray	 = __Parser_getTextArray;
	this.getNodeList	 = __Parser_getNodeList;
	this.checkConditions = __Parser_checkConditions;

	//////////
	//
	// private members
	//

	this.document = null;
	
	//////////
	//
	// constructor
	//
	{
		this.setDocument( document );
	}
}

//////////
//
// name: getText
// desc: Returns text for a given XPath expression
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __Parser_getText( xpath, context )
{
	if( arguments.length > 1 ) {
		this.document = context;
	}
	
	var value = null;

	// start ie
	//node = this.document.selectSingleNode( xpath );
	
	//if( node ) {
	//	value = node.text;
	//	alert(xpath + ' = ' + value);
	//}
	// end ie

	var aXpath = xpath.split("/");
	var xval;

	xval = (aXpath[(aXpath.length - 2)]);	
    	//alert(xval);
	if (this.document.getElementsByTagName(xval)[0].firstChild != null) {
		value = (this.document.getElementsByTagName(xval)[0].firstChild.data);
	}
	
	return value;
}

//////////
//
// name: getBoolean
// desc: Evaluates the query and returns a boolean for the result. If
//		 the query is not boolean, this function will return whether
//		 any nodes matched (rather than throw exception)
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __Parser_getBoolean( xpath, context )
{
	if( arguments.length > 1 ) {
		this.document = context;
	}

	var value = false;
	
	var nodeSet = this.document.getElementsByTagName( xpath );

	if( nodeSet ) {
		if( nodeSet.length ) {
			value = true;
		}
	}

	return value;
}
	    
 
//////////
//
// name: checkConditions
// desc: asserts that all conditions return true
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//
function __Parser_checkConditions( conditions, context )
{
	var condition = true;
	
	for( var c in conditions ) {
		if( this.getBoolean( c ) == false ) {
			condition = false;
			break;
		}
	}
	
	return condition;
}	  	    

//////////
//
// name: getTextArray
// desc: this function expects a dictionary of name:query and returns a
//		 dictionary of name:result
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//
function __Parser_getTextArray( querydict, context )
{
	throw "not implemented";
}	    

	    
//////////
//
// name: getNodeList
// desc: get the nodelist result of the query, which can be passed as the
//		 "context" parameter to other queries
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//
function __Parser_getNodeList( xpath, context )
{
	if( arguments.length > 1 ) {
		this.document = context;
	}
	
	var value = null;
	
	var aXpath = xpath.split("/");
	var xval;

	xval = (aXpath[(aXpath.length - 1)]);	
    	//alert(xval);
    	
	if (this.document.getElementsByTagName(xval) != null) {
	  //alert('a ' + this.document.getElementsByTagName(xval));
		var nodeList = (this.document.getElementsByTagName(xval));
	}
	
	//var nodeList = this.document.selectNodes( xpath );
	//alert('nl ' + nodeList.length + nodeList[0].id);
	return nodeList;
}	  

function __Parser_setDocument( document )
{
	this.document = document;
}

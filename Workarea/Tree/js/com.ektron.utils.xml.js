//////////
//
// name: XMLUtil
// desc: Static class XMLUtils provides a collection
//		 of useful functions for manipulating XML
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
// 

var XMLUtil =
{
	//////////
	//
	// name: deserialize
	// desc: A recursive method that turns an xml dom into one or more JavaScript
	//		 objects. This should evolve to take element data types into account.
	// auth: William Cava <william.cava@ektron.com>
	// date: April 2005
	//

	deserialize: function( node )
	{
		var object = new Array();

		if( node ) {
			object[node.tagName] = new Array();

			// get attributes
			if( node.attributes ) {
				var atts = new Array();
				var attributes = node.attributes;
				for( var i = 0; i < attributes.length; i++ ) {
					var attribute = attributes[i];
					var name  = attribute.name;
					var value = attribute.nodeValue;
					atts[name] = value;
				}
				object[node.tagName]["_attributes"] = atts;
			}

			// get children
			if( node.hasChildNodes() ) {
				object[node.tagName]["_children"] = new Array();
				for( var i = 0; i < node.childNodes.length; i++ ) {
					var child = node.childNodes[i];
					arrObjects = object[node.tagName]["_children"];
					arrObjects[arrObjects.length] = this.deserialize(child);
				}
			}

			// get text
			if( node.text ) {
				object[node.tagName]["_value"] = node.text;
			}
		}

		return object;
	},
	
	//////////
	//
	// name: createDocument
	// desc: Creates a new XML document
	//

	createDocument: function( s, isAsync )
	{
		if ( arguments.length == 1 ) {
			isAsync = false;
		}

		var objDoc = new ActiveXObject("Msxml2.DOMDocument.3.0");
		objDoc.loadXML(s);
		objDoc.async = isAsync;
		
		return objDoc;
	}	
}

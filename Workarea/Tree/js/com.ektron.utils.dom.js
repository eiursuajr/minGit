//////////
//
// name: DOMUtil
// desc: Static class DOMUtil provides a collection
//		 of useful functions for manipulating the
//		 Document Object Model.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
// 

var DOMUtil =
{
	//////////
	// name: getChildElement
	// desc: Recursively determines whether or not an element
	//		 has a child element that meets the specified
	//		 search criteria; if so, returns element.
	//
	getChildElement: function( element, name, value )
	{	
		if( element ) {
			if( element[name] != value ) {
				var children = element.childNodes;
				for( var i = 0; i < children.length; i++ ) {

					try {
						LogUtil.writeMessage( "tagName: " + children[i].tagName );
						LogUtil.writeMessage( children[i].getAttribute("id" ) );
					} catch( e ) {
						LogUtil.addMessage( LogUtil.CRITICAL,
									"DOMUtil",
									"getChildElement",
									e.description );
					}
		
					this.getChildElement( children[i], name, value );
				}
			} else {
				return element;
			}
		}
	},

	//////////
	//
	// name: getElementByClassName
	// desc: Given a className, this function returns an array of
	//       elements that have the specified className
	//
	elements: null,
	getElementByClassName: function( name )
	{
		var node = document.body;
		DOMUtil.elements = new Array();
		DOMUtil.__getElementByClassName( name, node );
		
		return DOMUtil.elements;		
	},
	
	__getElementByClassName: function( name, node )
	{
		if( node.className == name ) {
			var elements = DOMUtil.elements;
			elements[elements.length] = node;
		}

		if( node.hasChildNodes() ) {
			for( var i = 0; i < node.childNodes.length; i++ ) {
				var child = node.childNodes[i];
				DOMUtil.__getElementByClassName( name, child );
			}
		}
	},

	//////////
	// name: hasChildElement
	// desc: Determines whether or not an element
	//		 has a child element that meets the specified
	//		 search criteria
	//
	hasChildElement: function( element, name, value )
	{
		return this.getChildElement( element,
						name,
						value ) ? true : false;
	},

	/* private methods */

	//////////
	// name: __checkDependencies
	// desc: make sure we have 
	//
	__checkDependencies: function()
	{
		; // no dependencies
	}
}

DOMUtil.__checkDependencies();

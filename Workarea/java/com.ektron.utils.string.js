//////////
//
// name: StringUtils
// desc: Static class StringUtils provides a collection
//		 of useful functions for manipulating the
//		 strings.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
// 

var StringUtil =
{
	//////////
	//
	// name: serializeAsXML
	// desc: A recursive method that turns an object into a
	//		 xml string. Simply calling Object.toString()
	//		 isn't an option since that returns the type as string,
	//		 e.g., [Object object]. Added maxDepthRecursion to
	//		 prevent stack overflow when objects have circular
	//		 dependecies.
	//

	maxDepthRecursion: 10,
	curDepthRecursion: 0,
	serializeAsXML: function( object )
	{
		var buf = "";
		buf += "<object>";
		for( k in object ) {
			switch( typeof object[k] ) {
				case 'object':
				case '[object Object]':
					buf += "<property name='" + k + "'>";
					this.curDepthRecursion++;
					if( this.curDepthRecursion < this.maxDepthRecursion ) {
						buf += this.serializeAsXML( object[k] );
					}
					buf += "</property>";
				break;
				case 'number':
				case 'string':
				case 'function':
				case 'unknown':
				case 'undefined':				
				default:
					buf += "<property name='" + k +
						   "' value= '" + object[k] +
						   "'/>";
				break;
			}
		}
		buf += "</object>";
		return buf;
	},
	
	//////////
	//
	// name: parseNameValuePairs
	// desc: Given a name value pair formated as name1=value1&name2=value2,
	//		 this function will return an array of name value pairs
	//
	
	parseNameValuePairs: function( data )
	{
		var hash = new Array();
		var params = new String( data ).split("&");

		for( var i = 0; i < params.length; i++ ) {
			var nvps  = new String( params[i] );
			var nvp   = nvps.split( "=" );
			var name  = "";
			var value = new String("");
			if( nvp ) {
				switch( nvp.length ) {
					case 1 :
						name  = unescape(nvp[0]);
						value = null;
						break;
					case 2 :
						name  = unescape(nvp[0]);
						value = unescape(nvp[1]);
						if( !value ) {
							value = new String("");
						}
					break;
				}
			}

			hash[name] = value;
		}
		
		return hash;
	},
	
	//////////
	//
	// name: HTMLEncode
	// desc: Returns an HTML encoded version of the string
	//

	HTMLEncode: function(s)
	{
		s = new String(s);
		s = s.replace( /&/g, "&amp;" );
		s = s.replace( /</g, "&lt;" );
		s = s.replace( />/g, "&gt;" );
		s = s.replace( /\"/g, "&quot;" );
		
		return s;
	},

	
	//////////
	//
	// name: removeTags
	// desc: Removes all tags (e.g., <tag>) from the input string
	//

	removeTags: function(s)
	{
		return new String( s ).replace( /<[^>]*>/g, "" );
	},

	//////////
	//
	// name: trim
	// desc: Cuts the input string off at a specified length n,
	//		 taking into acount word breaks.
	//
	
	trim: function(s, n)
	{
		var arr = new String(s).split( " " );
		var buf = "";

		for( var i = 0; i < arr.length; i++ ) {
			var word = arr[i];
			if( ( buf.length + word.length ) < n) {
				buf += word + " ";
			}
		}
		
		return buf;
	},

	//////////
	//
	// name: contains
	// desc: Returns true if s2 is contained within s1, false otherwise.
	//

	contains: function(s1, s2)
	{
		return new String(s1).indexOf(s2) != -1 ? true : false;
	}	
}

//////////
//
// name: CookieUtil
// desc: A generic cookie utility for getting/setting simple and complex
//		 cookies. Since ASP supports the notion of a dictionary hash 
//		 within a named cookie, we'll also provide the same support here.
//

var CookieUtil =
{
	//////////
	//
	// name: getCookie
	// desc: A routine for getting cookies. Also provides the ability
	//       to set a default value that will be returned in the event that
	//       the cookie is not defined. In the event that a cookie is a 
	//       dictionary cookie, it returns a hash map of name value pairs.
	// 
	// usage:
	//
	//       /* simple cookie example */
	//       var guid = CookieUtil.getCookie( "ekguid" );
	//
	//       /* example of using a default value; defaults to admin */
	//       var name = CookieUtil.getCookie( "username", "admin" );
	//
	//       /* example of getting a dictionary style cookie; here its the ecm's user_id value */
	//       var value = CookieUtil.getCookie( "ecm" );
	//       var user_id = value["user_id"];
	//
		
	getCookie: function( name, defaultValue )
	{
		CookieUtil.parseCookies();
		
		var value = CookieUtil.map[name];
		if( value == null ) {
			value = defaultValue;
		}
		
		return value;
	},
	
	//////////
	//
	// name: setCookie
	// desc: A routine for setting cookies. Only name and value are
	//       required values; all others do not need to be specified
	//
	
	setCookie: function( name, value, expires, path, domain, secure )
	{
	    document.cookie = name + "=" + escape( value ) +
        ( ( expires ) ? "; expires=" + expires.toGMTString() : "" ) +
        ( ( path ) ? "; path=" + path : "" ) +
        ( ( domain ) ? "; domain=" + domain : "" ) +
        ( ( secure ) ? "; secure" : "" );
	},

	//////////
	//
	// name: deleteCookie
	// desc: A routine for deleting a cookie by its name. Path and domain
	//       are optional values, however if they're specified, they must
	//       be the same value used to create the cookie in the first place.
	//
	
	deleteCookie: function( name, path, domain )
	{
		if( CookieUtil.getCookie( name ) != null ) {
			document.cookie = name + "=" + 
				( ( path ) ? "; path=" + path : "" ) +
				( ( domain ) ? "; domain=" + domain : "" ) +
				"; expires=Thu, 01-Jan-70 00:00:01 GMT";
		}
	},
	
	//////////
	//
	// private methods/member variables
	//

	//////////
	//
	// name: parseCookies
	// desc: A private routine used to turn the document.cookie value into
	//       hash of name value pairs. In the event that a cookie is a 
	//       dictionary cookie, it will also be parsed and turned into a
	//       hash map of name value pairs. E.g.:
	//
	//       /* example of getting the ecm's user_id value */
	//       var value = CookieUtil.getCookie( "ecm" );
	//       var user_id = value["user_id"];
	//
	
	parseCookies: function()
	{
		var cookie = new String( document.cookie );
		var cookies = cookie.split( ";" );
		for( var i = 0; i < cookies.length; i++ ) {
			var nameValuePair = cookies[i];
			var nameValuePairArray = new String( nameValuePair ).split( "=" );
			var name  = null;
			var value = null;
			// if there are more than two equal signs, we have a complex cookie
			// that is storing name value pairs in its payload. ASP, for example,
			// allows for "dictionary" style cookies.
			switch( nameValuePairArray.length )
			{
				case 0:case 1:
					; // noop
				break;
				case 2:
					// typical cookie
					name  = nameValuePairArray[0];
					value = nameValuePairArray[1];
				break;
				default:
					// dictionary cookie
					name = nameValuePairArray[0];
					// nameValuePair looks like cookieName=n1=v1&n2=v2&n3=v3 etc
					// so we strip out the first name= part (e.g. cookieName=) and
					// parse the remaining namevalue pairs into a hash map
					var values = nameValuePair.replace( name + "=", "" )
					value = StringUtil.parseNameValuePairs( values );
				break;
			}
			// trim leading and trailing white space
			if (name != null)
			{
			    name = name.replace( /^ +/g, "" );
			    name = name.replace( / +$/g, "" );
			    CookieUtil.map[name] = value;
			}
		}
		CookieUtil.isCookieParsed = true;
	},
	
	//////////
	//
	// private member variables
	//
	
	cookies: document.cookie,
	isCookieParsed: false,
	map: {}
}

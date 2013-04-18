//////////
//
// name: QueryStringUtil
// desc: A utility class for manipulating and accessing
//		 parameters from the querystring
// auth: William Cava <willam.cava@ektron.com>
// date: April 2005
// 
//
var QueryStringUtil =
{
	//////////
	//
	// name: getParam
	// desc: given a parameter name, this function will
	//		 return its value. If the value is undefined
	//		 and a defaultValue is provided, defaultValue
	//		 is returned.
	//		 
	//
	getParam: function( name, defaultValue )
	{
		if( !this.queryParams ) {
			this.__setQueryParams();
		}

		var value = this.queryParams[name];

		if( !value ) {
			value = null;
			if( defaultValue ) {
				value = defaultValue;
			}
		}
		
		return value;
	},

	//////////
	//
	// name: getQueryString
	// desc: This function takes no parameters and returns
	//		 the raw querystring.
	//
	getQueryString: function()
	{
		if( !this.queryString ) {
			this.__setQueryString();
		}

		return this.queryString;
	},

	//////////
	//
	// private member, method variables
	//

	queryString: null,
	queryParams: null,
	
	//////////
	//
	// name: __setQueryString
	// desc: grabs the querystring from the URL, normalizes it,
	//		 and stores it locally.
	//
	__setQueryString: function()
	{
		this.queryString = new String(location.search);
		if( this.queryString.indexOf("?") == 0 ) {
			var length = this.queryString.length;
			// first char is "?", remove it.
			this.queryString = this.queryString.substr(1, length);
		}
	},

	//////////
	//
	// name: __forceQueryString
	// desc: This method will force a querystring to a
	//		 particular value.
	//
	__forceQueryString: function(s)
	{
		this.queryString = s;
	},
	
	//////////
	//
	// name: __setQueryParams
	// desc: grabs the querystring, breaks the params into name value
	//		 pairs, and stores them locally as a hash
	//
	__setQueryParams: function()
	{
		this.queryParams = new Array();
		var params = this.getQueryString().split("&");

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

			this.queryParams[name] = value;
		}
	}
}

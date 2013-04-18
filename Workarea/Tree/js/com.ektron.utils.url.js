//////////
//
// name: URLUtil
// desc: A helper class for manipulating URLs

var URLUtil = 
{
	ek_appPath : "WorkArea/", // must include trailing '/'
	ek_appPath2:"",
	getHostName : function( url )
	{
		var newUrl = new String(url);
		newUrl = newUrl.replace( "https://", "" );
		newUrl = newUrl.replace( "http://", "" );

		// strip everything after the first forward slash
		var host = newUrl.replace( /^([^\/]+).*$/, "$1" );
		
		return host;
	},

	getBaseURL : function( url )
	{
		var _protocol = "http://";
		var newUrl = new String( url );
		
		if( StringUtil.contains( newUrl, "https://" ) ) {
			_protocol = "https://";
		}
		
		newUrl = _protocol + URLUtil.getHostName( url );

		return newUrl;
	},

	getSiteRoot: function( url )
	{
		url = new String( url );
		var indexOfWorkarea = url.toLowerCase().indexOf( this.ek_appPath.toLowerCase() );
		if( indexOfWorkarea != -1 ) {
			url = url.substring(0,indexOfWorkarea); 
		}

		return url;
	},

	getAppRoot: function( url )
	{
		url = new String( url );
		var indexOfWorkarea = url.toLowerCase().indexOf( this.ek_appPath.toLowerCase() );
		if( indexOfWorkarea != -1 ) {
			url = url.substring(0,indexOfWorkarea + this.ek_appPath.length);
		}

		return url;
	}

	/*
	getServerRoot : function()
	{
		var protocol = "http";
		var url = String( this.getSiteRoot() );

		if( url.indexOf( "https://" ) == 0 ) {
			protocol = "https";
		}

		return protocol + "://" + this.getHostName();
	},
	
	getSiteRoot : function()
	{
		return ExplorerConfig["cms_site_root"];
	}
	*/	
}
if (typeof ____ek_appPath2 != 'undefined') {
    URLUtil.ek_appPath2=____ek_appPath2;
}
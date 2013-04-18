//////////
//
// name: ExplorerConfigUtil
// desc: Global config data
//

var ExplorerConfig = new Array();
ExplorerConfig["exp_build_ver"]		 = "0";
ExplorerConfig["cms_svc_method"]	 = "POST";

//////////
//
// name: ExplorerConfigUtil
// desc: An object for retrieving and setting config data
//

var ExplorerConfigUtil =
{
	"getPersistentParam": function( name )
	{
		var value = null;
		try {
			value = window.external.EktGetRegValue( name );
		} catch( e ) {
			LogUtil.addMessage( 
				LogUtil.CRITICAL,
				"ExplorerConfigUtil",
				"getPersistentParam",
				"Failed getting param '" + name + "': " +
				e.description );
		}
		
		return value;
	},

	"setPersistentParam": function( name, value )
	{
		try {
			var status = window.external.EktSetRegValue( name, value );
		} catch( e ) {
			LogUtil.addMessage( 
				LogUtil.CRITICAL,
				"ExplorerConfigUtil",
				"setPersistentParam",
				"laksjflaksjdf" );
//				"Failed setting param '" + name + "', " +
//				"value '" + value + "': " +
//				e.description );
		}
	},

	"getHostName" : function()
	{
		var url = new String(ExplorerConfig["cms_site_root"]);
		url = url.replace( "https://", "" );
		url = url.replace( "http://", "" );

		// strip everything after the first forward slash
		var host = url.replace( /^([^\/]+).*$/, "$1" );
		
		return host;
	},

	"getServerRoot" : function()
	{
		var protocol = "http";
		var url = String( this.getSiteRoot() );

		if( url.indexOf( "https://" ) == 0 ) {
			protocol = "https";
		}

		return protocol + "://" + this.getHostName();
	},
	
	"getSiteRoot": function()
	{
		return ExplorerConfig["cms_site_root"];
	},
	
	"getParam": function( name )
	{
		return ExplorerConfig[name];
	},
	
	"setParam": function( name, value )
	{
		if( name == "cms_site_root" ) {
			var value  = new String( value ).toLowerCase();
			var substr = new String( ExplorerConfig["exp_http_root"] ).toLowerCase()
			var index = value.indexOf( substr );
			if( index != -1 ) {
				value = value.substring( 0, index );
			}
		}

		return ExplorerConfig[name] = value;
	}
}

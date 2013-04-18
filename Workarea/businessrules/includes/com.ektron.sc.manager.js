function EnvironmentManager()
{
	// public functions
	this.getParam		= __EnvironmentManager_getParam;
	this.addEnvironment	= __EnvironmentManager_addEnvironment;

	// private members
	this.data = {};
		
	// initialize
	{
		this.addEnvironment( "cookie", CookieUtil );
		this.addEnvironment( "querystring", QueryStringUtil );
		// this.addEnvironment( "form", CookieUtil );
	}
}

function __EnvironmentManager_getParam( scope, name )
{
	return this.data[scope].getParam( name );
}

function __EnvironmentManager_addEnvironment( name, environment )
{
	this.data[name] = environment;
}

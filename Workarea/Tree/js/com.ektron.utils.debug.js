//////////
//
// name: Debug
// desc: The beginnings of a class used for
//		 storing debug state information. Accessed
//		 globally, statically.
var Debug =
{
	// debugging off by default
	LEVEL : "-1",

	setLevel : function( level )
	{
		this.LEVEL = level;
	}
}

/*
TODO: We'll allow overloading this from cookies
var __debug_tmp_level = Request.Cookies.item("debug");
if( typeof(__debug_tmp_level) != undefined ) {
	Debug.LEVEL = __debug_tmp_level;
}

// We'll also allow overloading this from cookies (takes precendence)
var __debug_tmp_level = Request.QueryString.item("debug");
if( typeof(__debug_tmp_level) != undefined ) {
	Debug.LEVEL = __debug_tmp_level;
}
*/
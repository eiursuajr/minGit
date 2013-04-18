//////////
//
// name: LogUtil
// desc: This static class acts as a utility for logging application
//		 messages. Is configurable so it can write to an external
//		 console window or log file.
// auth: william.cava@ektron.com
// date: March 16, 2005
//
var LogUtil = 
{
	logConsole: false,
	logWindow: window,
	logType: null,

	//////////
	//
	// name: addMessage
	// desc: Adds message to the log
	//
	setLogType: function( type )
	{
		LogUtil.logType = type;
	},

	//////////
	//
	// name: addMessage
	// desc: Adds message to the log
	//
	addMessage: function( level, packageName, functionName, message )
	{
		// overload for one parameter
		if( arguments.length == 1 ) {
			message = level;
			packageName = "";
			functionName = "";
			level = LogUtil.DEBUG;
		}

		// Show only messages with equal or greater severity
		if( level <= Debug.LEVEL ) {
			switch( LogUtil.logType ) {
				case LogUtil.LOG_FILE:
					LogUtil.writeToFile( level, packageName, functionName, message );
				break;
				
				case LogUtil.LOG_CONSOLE:
					LogUtil.writeToConsole( level, packageName, functionName, message );
				break;
			}
		}
	},
	
	writeToFile: function( level, packageName, functionName, message )
	{
		// todo, move
		var v = "?.?.?.?";
		try {
			var v2 = window.external.EktGetCurrentVersion();
			v = v2 == "0" ? v : v2;
		} catch( e ) {
			;
		}
	
		try { 
			var entry = "<entry><version>Ektron Explorer-" + v + "</version>" +
						"<stamp>" + new Date().getTime() + "</stamp>" +
						"<level>" + LogUtil.DISPLAY_TEXT[level] + "</level>" +
						"<package>" + packageName + 
						"." + functionName + "</package>" +
						"<message><![CDATA[" + message + "]]></message></entry>";

			window.external.EktWriteLog( entry );
		} catch( e ) {
			; // can't log this error
		}
	},
	
	writeToConsole: function( level, packageName, functionName, message )
	{
		if( !LogUtil.logConsole ) {
			LogUtil.openConsole();
		}

		try { 
			LogUtil.logConsole.add_message(
							LogUtil.DISPLAY_TEXT[level],
							packageName,
							functionName,
							message );
		} catch( e ) {
			; // can't log this error
		}
	},

	//////////
	//
	// name: write message
	// desc: writes a message to the log
	//
	writeMessage: function( message )
	{
		this.addMessage( message );
	},

	//////////
	//
	// name: setWindow
	// desc: This sets the window
	//
	setWindow: function( w )
	{
		LogUtil.logWindow = w;
	},
	
	//////////
	//
	// name: openConsole
	// desc: opens the logging console
	//
	openConsole: function()
	{
		// TODO: This needs to be resolved!!
		this.logConsole = LogUtil.logWindow.open(
					"resources/logutil.html",
					"logConsole",
					"height=525,width=675,scrollbars=yes,resizable=yes");
	}
}

//////////
//
// The log util can log to a window console or a file
//

LogUtil.LOG_FILE    = 0;
LogUtil.LOG_CONSOLE = 1;

//////////
//
// Set the default log
//

LogUtil.logType = LogUtil.LOG_FILE;

/*
CRITICAL	The health of the system or the Engine is in jeopardy;
			for example, an operation has failed because there is not
			enough memory. 
SERIOUS		An operation did not succeed. 
ERROR		The user has caused an error. The error messages are
			provided to help the user correct the problem. 
WARNING		An error has occurred that the system might or might not
			be able to work around. 
DEFAULT		An error has occurred that the system has already worked
			around. 
DEBUG		Information that helps the user debug a problem. 
ALL			Verbose output. 
*/

LogUtil.CRITICAL = 0;
LogUtil.SERIOUS	 = 1;
LogUtil.ERROR	 = 2;
LogUtil.WARNING	 = 3;
LogUtil.DEFAULT	 = 4;
LogUtil.DEBUG	 = 5;
LogUtil.ALL		 = 6;

LogUtil.DISPLAY_COLOR = new Array();
LogUtil.DISPLAY_COLOR[LogUtil.CRITICAL]	= "red";
LogUtil.DISPLAY_COLOR[LogUtil.SERIOUS]	= "red";
LogUtil.DISPLAY_COLOR[LogUtil.ERROR]	= "red";
LogUtil.DISPLAY_COLOR[LogUtil.WARNING]	= "orange";
LogUtil.DISPLAY_COLOR[LogUtil.DEFAULT]	= "yellow";
LogUtil.DISPLAY_COLOR[LogUtil.DEBUG]	= "green";
LogUtil.DISPLAY_COLOR[LogUtil.ALL]		= "blue";

LogUtil.DISPLAY_TEXT = new Array();
LogUtil.DISPLAY_TEXT[LogUtil.CRITICAL]	= "CRITICAL";
LogUtil.DISPLAY_TEXT[LogUtil.SERIOUS]	= "SERIOUS";
LogUtil.DISPLAY_TEXT[LogUtil.ERROR]		= "ERROR";
LogUtil.DISPLAY_TEXT[LogUtil.WARNING]	= "WARNING";
LogUtil.DISPLAY_TEXT[LogUtil.DEFAULT]	= "DEFAULT";
LogUtil.DISPLAY_TEXT[LogUtil.DEBUG]		= "DEBUG";
LogUtil.DISPLAY_TEXT[LogUtil.ALL]		= "ALL";
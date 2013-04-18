//////////
//
// our global vars
//
var mainWindow = null;
var toolkit    = null;

//////////
//
// name: Main
// desc: Kicks things off
//
//

var Main =
{
	//////////
	//
	// name: start
	// desc: The entrypoint into the Explorer
	//
	
	start: function()
	{
		try {
			// Initialize everyone
			Main.initialize();
			
			// Obtain handle on main window
			Main.getMainWindow();

		} catch( e ) {
			Main.logMessage( "start", e.message );
		}
	},

	//////////
	//
	// name: intialize
	// desc: Initialize globals
	//
	
	initialize: function()
	{
		try {
			toolkit = new EktronToolkit();
	        if (typeof Explorer !== "undefined") {
			    Explorer.initialize();
			}
		}
		catch( e ) {
			Main.logMessage( "initialize", e.message )
		}
	},

	//////////
	//
	// name: getMainWindow
	// desc: We try to get a handle on the main explorer window.
	//		 in the event that the mainWindow is null, we'll wait
	//		 250 ms and retry, up to 4 times (1 second). mainWindow
	//		 should be set by onMainWindowLoad by ektbartb when it
	//		 raises that event, but in the case that the main window
	//		 loads before the explorer window, there wont be anyone
	//		 around to catch that event. In that case, this function
	//		 will be called to obtain a handle on the event.
	//
	
	getMainWindow: function()
	{
		try{
	        if (typeof window.external.EktGetWindow !== "undefined") {
			    Main.onMainWindowLoad( window.external.EktGetWindow() );
			}
			if( mainWindow == null ) {
				if( Main.tryCounter++ < 4 ) {
					setTimeout( Main.getMainWindow, 250 );
				}
			}
		} catch( e ) {
			Main.logMessage( "getMainWindow", e.message );
		}
	},
	
	//////////
	//
	// name: onMainWindowLoad
	// desc: event handler for the onMainWindowLoad
	//       event. This event is fired by the ektbartb
	//		 and also called from Main.getMainWindow
	//

	onMainWindowLoad: function( handler )
	{
		if( mainWindow == null ) {
			mainWindow = handler;
			if( Main.explorerStarted == false ) { 
				Main.explorerStarted = true;
				Explorer.start();
			}
		}
		//Main.setDebugMode();
	},	

	//////////
	//
	// name: logMessage
	// desc: When an exception is thrown here, we'll try to log
	//		 the exeption, but in the event that the LogUtil is
	//		 throwing the exception, we'll want to fail quietly.
	//		 This is a simple function that tries to add a message
	//	     to the log, but fails quietly.
	//

	logMessage: function( location, message )
	{
		try {
			LogUtil.addMessage(
				LogUtil.CRITICAL,
				"Main",
				location,
				message );
		} catch( e ) {
			; // fail quietly
		}
	},

	//////////
	//
	// name: setDebugMode
	// desc: Provides an interface for setting the debug level
	//		 of the application. This is currently not implented.
	//		 The implementation could check the value of a cookie
	//		 or parameter from the querystring, then set the
	//		 LogUtil.LEVEL = Debug.ALL; etc.
	//
	
	setDebugMode: function()
	{
		// throw Exception( "not implemented" );
	},

	tryCounter: 0,
	browserWindow: null,
	explorerStarted: false
}

//////////
//
// name: JSTest
// desc: The beginings of a very simple unit
//		 testing framework for server side JScript
// auth: william.cava@ektron.com
// date: March 17, 2005
//

// dependencies
LogUtil;

function JSTest( name, assertion, param1, param2, description )
{
	this.name = name;
	this.assertion = assertion;
	this.param1 = param1;
	this.param2 = param2;
	this.description = description;
	
}

//////////
//
// name: JSUnitTest
// desc: The beginings of a very simple unit
//		 testing framework for server side JScript
// auth: william.cava@ektron.com
// date: March 17, 2005
//
function JSUnitTest()
{
	this.assertEquals		= __JSUnitTest_assertEquals;
}

function __JSUnitTest_assertEquals( param1, param2 )
{
	return param1 == param2;
}

//////////
//
// name: JSTestSuite
// desc: The beginings of a very simple unit
//		 testing framework for server side JScript
// auth: william.cava@ektron.com
// date: March 17, 2005
//
function JSTestSuite()
{
	// public methods
	this.addTest	= __JSTestSuite_addTest;
	this.run		= __JSTestSuite_run;

	// private properties / methods
	this.tests		= new Array();
	this.initialize = __JSTestSuite_initialize
	
	// constructor
	{
		this.initialize();
	}
}

function __JSTestSuite_initialize()
{
	; // noop
}

function __JSTestSuite_addTest( name, unitTestMethod, param1, param2, description )
{
	this.tests[this.tests.length++] = new JSTest(
								name,
								unitTestMethod,
								param1,
								param2,
								description );
}

function __JSTestSuite_run()
{
	var statusColor = "green";
	for( var i = 0; i < this.tests.length; i++ ) {
		var test = this.tests[i];
		var msg  = "Testing: <b>" + test.name + "</b> <ul><li>";
		try {
			// assertion error or not
			if( ! test.assertion( test.param1, test.param2 ) ) {
				msg += "Assertion error ";
				statusColor = "orange";
			} else {
				msg += "Assertion passed ";
				statusColor = "green";
			}
			msg += "</li>";
			msg += "<li>Expected <b style='background:#ffffcc;padding:1px;border:solid 1px #cccccc;'>" +
					test.param2 + "</b>";
			msg += " and received <b style='background:#ffffcc;padding:1px;border:solid 1px #cccccc;'>" +
					test.param1 + "</b></li>";

		// exception error
		} catch( e ) {
			msg += "Exception! Reason: " + e.message;
			statusColor = "red";
		}

		msg += "</li>";
	    msg += ( this.description ? "<li>" + this.description + "</li>" : "" );
	    msg += "</ul>"; 

		// simple formatting. Red box means exception, green means
		// success, and orange means exception.
		msg = "<div style='padding:5pt;margin:5px;border: " +
				statusColor + " solid 1px;'>" +
				"<ul><span style='width:20px;height:20px;background:" +
				statusColor + ";'>&nbsp;</span>&nbsp;&nbsp;" + msg +
				"</ul></div><style>body{font-family:tahoma;font-size:" +
				"10pt}</style>";

		JSUnitTestLogger.writeMessage( "\n" + msg );
	}
}


var JSUnitTestLogger =
{
	logConsole: false,

	//////////
	//
	// name: addMessage
	// desc: Adds message to the log
	//
	writeMessage: function( msg )
	{
		if( !this.logConsole ) {
			this.openConsole();
			this.logConsole.document.write( "<html><body>" );
		}

		this.logConsole.document.write( msg );
	},

	//////////
	//
	// name: openConsole
	// desc: opens the logging console
	//
	openConsole: function()
	{
		this.logConsole = window.open("",
			"unitTestWin",
			"height=400,width=600,scrollbars=yes,resizable=yes");
	}
}
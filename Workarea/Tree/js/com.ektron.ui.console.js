//////////
//
// name: Console
// desc: A Console UI
// auth: william.cava@ektron.com
// date: March 2005
//
function Console( name )
{
	this.writeMessage = __Console_writeMessage;
	this.write        = __Console_write;
	this.open         = __Console_open;
	this.close		  = __Console_close;


	//////////
	// 
	// private members:
	//

	//this.logConsole = false;
	//this.consoleName = name ? name : "console";
}

//////////
//
// name: write
// desc: Given a message, this function will write the
//       unformatted string to the console. If no console is available,
//       a new console will be opened.
//

function __Console_write( data )
{
	if( !this.logConsole ) {
		this.openConsole();
	}
	this.logConsole.document.write( data );
}

//////////
//
// name: writeMessage
// desc: Given a message, this function will write the
//       message to the console. This message provides minimal
//		 formatting. If no console is available, a new console
//		 will be opened.
//

function __Console_writeMessage( data )
{
	if( !this.logConsole ) {
		this.openConsole();
	}
	this.write( "<div style='border:dashed red 1px;padding: 5px;'>" + data + "</div>" );
}

//////////
//
// name: openConsole
// desc: Opens a new console
//

function __Console_open()
{
	this.logConsole = window.open("",
		this.consoleName,
		"height=400,width=600,scrollbars=yes,resizable=yes");
}

//////////
//
// name: closeConsole
// desc: Closes the opened console
//

function __Console_close()
{
	if( this.logConsole ) {
		this.logConsole.close();
	}
}
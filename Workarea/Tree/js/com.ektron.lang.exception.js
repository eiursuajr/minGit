//////////
//
// name: Exception
// desc: Exception class can be used to either wrap an
//		 existing javascript exception, or as its own
//		 exception definition.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function Exception( e )
{
	//////////
	//
	// public members
	//
	
	this.getDescription = __Exception_getDescription;
	this.toString		= __Exception_toString;

	//////////
	//
	// private members
	//
	
	this.exception = null;
	this.setException	= __Exception_setException;
	
	//////////
	//
	// constructor
	//
	{
		this.setException( e );
	}
}

//////////
//
// name: getDescription
// desc: JavaScript's exception object has human readable
//		 exception information tucked away in either a
//		 property called "message" or "description". This
//		 function tries to gather the information from
//		 both ... and returns whatever it could find.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//
function __Exception_getDescription()
{
	var description = "";
	
	if( this.exception ) {
		try
		{
			description += this.exception.message + " ";
		}
		catch( e )
		{
			; // noop
		}
		try
		{
			description += this.exception.description + " ";
		}
		catch( e )
		{
			; // noop
		}
	}
	
	return description;
}

//////////
//
// name: setDescription
// desc: Setter for a javascript exception object
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __Exception_setException( e )
{
	this.exception = e;
}

//////////
//
// name: toString
// desc: If we have a JavaScript exception object,
//		 this method collects all available information
//		 from the object and returns it as a string.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __Exception_toString()
{
	var exception = "";
	
	for( var k in this.exception ) {
		var value = exception[k];
		if( value ) {
			exception += k + "=" + value + ", ";
		}
	}

	return exception;
}


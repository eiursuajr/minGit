//////////
//
// name: HttpProxy
// desc: Adapter pattern, used to normalize the interfaces
//		 provided by XML/HTTP objects and to localize
//		 error handling and logging.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function HttpProxy()
{
	//////////
	//
	// public members
	//

	this.fetch		 = __HttpProxy_fetch;
	this.send		 = __HttpProxy_send;
	this.open		 = __HttpProxy_open;
	this.initialize	 = __HttpProxy_initialize;
	this.getDocument = __HttpProxy_getDocument;
	this.getRootNode = __HttpProxy_getRootNode;
	this.getStatus	 = __HttpProxy_getStatus;
	this.setHeaders  = __HttpProxy_setHeaders
	this.getReadyState		  = __HttpProxy_getReadyState;
	this.setReadyStateHandler = __HttpProxy_setReadyStateHandler;

	//////////
	//
	// private members
	//

	this.xmlhttp = null;
	this.readyStateHandler = null;

	//////////
	//
	// constructor
	//
	{
		this.initialize();
	}	
}

function __HttpProxy_getReadyState()
{
	return this.xmlhttp.readyState;
}

function __HttpProxy_setReadyStateHandler( f )
{
	this.xmlhttp.onreadystatechange = f;
}

//////////
//
// name: fetch
// desc: given a URL and a a/synch flag, fetch opens
//		 a connection to the server, sends the request,
//		 and if it is a synchronous request, returns
//		 the response.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//
function __HttpProxy_fetch( url, isAsync )
{
	var responseText = null;
	
	this.open( url, isAsync, "GET" );
	this.send();
	
	if( ! isAsync ) {
		if( this.xmlhttp.responseText ) {
			responseText = this.xmlhttp.responseText;
		}
	}

	return responseText;
}

//////////
//
// name: send
// desc: issues an HTTP request
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __HttpProxy_send( data )
{
	if( data == null ) {
		data = "";
	}
	LogUtil.addMessage( LogUtil.DEBUG,
			"HttpProxy",
			"send",
			"send(" + data + ")");

	try
	{
		this.xmlhttp.send( data );
	}
	catch( e )
	{
		LogUtil.addMessage( LogUtil.CRITICAL,
				"HttpProxy",
				"send",
				"Failed to send(" + data + ")" +
				new Exception(e) );
	}
}

//////////
//
// name: open
// desc: opens an HTTP connection
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __HttpProxy_open( url, isAsync, requestType )
{
	// we overload open
	if( arguments.length == 2 ) {
		requestType = "GET";
	}

	if( this.xmlhttp ) {
		LogUtil.addMessage( LogUtil.DEBUG,
				"HttpProxy",
				"open",
				"Opening " + 
				( isAsync ? "asynchronous" : "synchronous" ) +
				" HTTP connection: " + requestType + " " +
					url );
		try
		{
			this.xmlhttp.open( requestType, url, isAsync );
		}
		catch( e )
		{
			LogUtil.addMessage( LogUtil.CRITICAL,
					"HttpProxy",
					"open",
					"Failed to open( " + url + ", " + isAsync + ", "
					+ requestType + "): " + e.description );
		}
	} else {
		LogUtil.addMessage( LogUtil.CRITICAL,
				"HttpProxy",
				"open",
				"Cannot open: we do not have an XMLHTTP object" );
	}
}

//////////
//
// name: setHeaders
// desc: Sets request headers, given a hash of headers
// auth: William Cava
// date: May 2005
//

function __HttpProxy_setHeaders( headers )
{
	for( var header in headers ) {
		this.xmlhttp.setRequestHeader( header, headers[header] );
	}
}

//////////
//
// name: initialize
// desc: Attempts to create an XML/HTTP object in order
//		 of preference.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __HttpProxy_initialize()
{
	/*@cc_on @*/
	/*@if (@_jscript_version >= 5)
	// JScript gives us Conditional compilation, we can cope with old IE versions.
	// and security blocked creation of the objects.
	try
	{
		this.xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
	}
	catch (e1)
	{
		LogUtil.addMessage( LogUtil.CRITICAL,
				"HttpProxy",
				"initialize",
				"Failed creating MSXML2.XMLHTTP, trying Microsoft.XMLHTTP" );
		try
		{
			this.xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch (e2)
		{
			LogUtil.addMessage( LogUtil.CRITICAL,
					"HttpProxy",
					"initialize",
					"Failed creating Microsoft.XMLHTTP, trying XMLHttpRequest()" );

			this.xmlhttp = false;
		}
	}
	@end @*/
	if (!this.xmlhttp && typeof XMLHttpRequest!='undefined') {
		this.xmlhttp = new XMLHttpRequest();
	}

	if( !this.xmlhttp ) {
		LogUtil.addMessage( LogUtil.CRITICAL,
				"HttpProxy",
				"initialize",
				"Failed to initialize xmlhttp!" );
	}
}

//////////
//
// name: getRootNode
// desc: If the previous request was for a valid
//		 XML document, getRootNode returns the root
//		 node for the XML document.
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __HttpProxy_getRootNode()
{
	var rootNode = null;
	if( this.xmlhttp ) {
		try { 
			rootNode = this.xmlhttp.documentElement;
		} catch( e ) {
			LogUtil.addMessage( LogUtil.WARNING,
					"HttpProxy",
					"getRoot",
					"Attempted to getRoot, documentElement was null" );
		}
	}
	
	return rootNode;		
}

//////////
//
// name: getDocument
// desc: Returns the document object
// auth: William Cava <william.cava@ektron.com>
// date: April 2005
//

function __HttpProxy_getDocument()
{
	var document = null;
	var warning  = null;
	
	if( this.xmlhttp ) {
		try {
			var document = this.xmlhttp.responseXML;
		} catch( e ) {
			warning = "this.xmlhttp.responseXML not an object: " +
					  e.description;
		}
	} else {
		warning = "this.xmlhttp is null";
	}
	
	if( warning ) {
		LogUtil.addMessage( LogUtil.WARNING,
				"HttpProxy",
				"getDocument",
				"Attempted to getDocument, " +
				warning);
	}

	return document;
}

function __HttpProxy_getStatus()
{
    var status = null;
    var warning  = null;
   if(this.xmlhttp)
   {
            try
                {
                    status = this.xmlhttp.status
                }catch(e)
                    {
                        warning ="this.xmlhttp.status not an object: " + e.description;
                    }
   }else
                {
                     warning = "this.xmlhttp is null";
                }
   if( warning ) {
		LogUtil.addMessage( LogUtil.WARNING,
				"HttpProxy",
				"getStatus",
				"Attempted to getStatus, " +
				warning);
				}
	return status ;
}

//////////
//
// The following readyState values are based on the properties of
// the IServerXMLHTTPRequest interface, definitions on MSDN:
// http://tinyurl.com/9gh7n
//

//////////
// 
// The object has been created but has not been initialized
// because the open method has not been called.
//
HttpProxy.UNINITIALIZED = 0

//////////
//
// The object has been created but the send method has not been called. 
//
HttpProxy.LOADING = 1;

//////////
//
// The send method has been called and the status and headers are
// available, but the response is not yet available.
//
HttpProxy.LOADED = 2;

//////////
//
// Some data has been received. You can call responseBody and
// responseText to get the current partial results.
//
HttpProxy.INTERACTIVE = 3;

//////////
//
// All the data has been received, and the complete data is available
// in responseBody and responseText. 
//
HttpProxy.COMPLETED = 4;


//////////
//
// name: HttpRequest
// desc: Request information such as URL, post/get, etc.
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function HttpRequest()
{
	this.setHeaders	= __HttpRequest_setHeaders;
	this.setUrl		= __HttpRequest_setUrl;
	this.setType	= __HttpRequest_setType;
	this.setData	= __HttpRequest_setData;
	this.getSerializedData		= __HttpRequest_getSerializedData;
	this.getSerializedHeaders	= __HttpRequest_getSerializedHeaders;

	this.headers = null;
	this.url	 = null;
	this.type	 = null;
	this.data	 = null;
}

function __HttpRequest_getSerializedHeaders()
{
	var headers = "";
	for( var header in this.headers ) {
		headers += header + "=" + this.headers[header] + "&";
	}
	
	return headers;
}

function __HttpRequest_getSerializedData()
{
	var data = "";
	for( var item in this.data ) {
		data += item + "=" + this.data[item] + "&";
	}
	
	return data;
}

function __HttpRequest_setHeaders( headerMap )
{
	this.headers = headerMap;
}

function __HttpRequest_setUrl( url )
{
	this.url = url;
}

function __HttpRequest_setType( type )
{
	switch( type )
	{
		case "GET":
			this.type = "GET"
		break;
		case "POST":
			this.type = "POST"
		break;
	}
}

function __HttpRequest_setData( data )
{
	this.data = data;
}

HttpRequest.GET  = "GET";
HttpRequest.POST = "POST";

//////////
//
// name: HttpPool
// desc: A class for creating and managing a pool of http objects.
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function HttpPool( size )
{
	this.objects = new Array();
	this.objectType = "HttpProxy()";
	this.size = ( size ) ? size : 10;

	this.init = _httpPool_init;
	this.claimObject = _httpPool_claimObject;
	this.releaseObject = _httpPool_releaseObject;
	
	this.init( size );
}

//////////
//
// name: init
// desc: Creates a pool of HttpProxy objects of size n
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function _httpPool_init( size )
{

	var iSize = ( ! size ) ? this.size:size;
	for( var i = 0; i < iSize; i++ ) {
		var obj = eval( "new " + this.objectType );
	
		this.objects[this.objects.length] = obj;
	}
}

//////////
//
// name: claimObject
// desc: Claims an HttpProxy object and removes it from the pool
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function _httpPool_claimObject()
{
	var msg = "";
	var object = null;

	if( this.objects.length == 0 ) {
		msg = "Pool is empty";
	} else {
		object = this.objects.pop();
		msg += "Claimed 1 object; " + this.objects.length +
			   " objects left in pool.";
	}

	LogUtil.addMessage( LogUtil.DEBUG,
			"HttpPool",
			"claimObject",
			msg );	

	return object;
}

//////////
//
// name: releaseObject
// desc: Releases an HttpProxy object and returns it to the pool
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function _httpPool_releaseObject( object )
{
	var level, message;
	
	if( this.objects.length < this.size ) {
		this.objects[this.objects.length] = object;
		level   = LogUtil.DEBUG;
		message = "Released 1 object; " + this.objects.length + " objects now in pool.";
	} else {
		level   = LogUtil.WARNING;
		message = "Tried to release 1 object; max pool sized reached!";
	}

	LogUtil.addMessage( level,
			"HttpPool",
			"releaseObject",
			message );
}

//////////
//
// name: AsyncHttpUtil
// desc: A static utility class used for managing asynchronous http requests
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

var AsyncHttpUtil =
{
	// we'll randomize retries to reduce contention
	maxTimeUntilRetry: 400,
	minTimeUntilRetry: 600,
	maxNumRetry: 3,
	httpPool: null,

	"setPoolSize": function( poolSize )
	{
		AsyncHttpUtil.httpPool = new HttpPool(poolSize);
	},

	//////////
	//
	// name: request
	// desc: Given an httpRequest and readyStateHandler, request issues
	//		 an asynchronous request to the URL defined in httpRequest.
	//		 It tries to obtain an httpProxy object from its httpPool,
	//		 if it fails, it waits for timeTillRetry milliseconds, and
	//		 tries again, until maxNumRetry is reached.
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	"request": function( httpRequest, readyStateHandler, waitCount )
	{
		var httpProxy = AsyncHttpUtil.httpPool.claimObject();
		
		// do we have a proxy object?
		if( ! httpProxy ) {

			// handle overloaded request method
			if( arguments.length == 2 ) {
				waitCount  = 1;
			} else {
				waitCount += 1;
			}

			var serializedData    = httpRequest.getSerializedData();
			var serializedHeaders = httpRequest.getSerializedHeaders();
			var url  = httpRequest.url;
			var type = httpRequest.type;

			var max = AsyncHttpUtil.maxTimeUntilRetry;
			var min = AsyncHttpUtil.minTimeUntilRetry;
			var waitTime = Math.floor( ( max - min + 1 ) * Math.random() + min );

			// wait a bit then try again, until we've hit the waitCount
			if( waitCount <= AsyncHttpUtil.maxNumRetry ) {
				var params = "(url=" + url + "), " +
							"(type=" + type + "), " +
							"(data=" + serializedData + "), " +
							"(headers=" + serializedHeaders + ") " +
							"(readyStateHandler=" + readyStateHandler + ")";

				LogUtil.addMessage( LogUtil.WARNING,
						"AsyncHttpUtil",
						"request",
						"HttpPool is empty, no resources free. Wait #" + waitCount +
						" of " + AsyncHttpUtil.maxNumRetry + ". Waiting " +
						waitTime + " ms to try again. Calling waitRequest " +
						" with params: " + params);

				window.setTimeout("AsyncHttpUtil.waitRequest( '" + url + "'," +
								"'" + type + "', '" + serializedData + "', '" +
								serializedHeaders + "', " + readyStateHandler +
								", " + waitCount + " )", waitTime);
			} else {
				LogUtil.addMessage( LogUtil.CRITICAL,
						"AsyncHttpUtil",
						"request",
						"Reached max waitCount. Giving up." );
			}

		} else {
			httpProxy.open( httpRequest.url, true, httpRequest.type );
			if( httpRequest.headers != null ) {
				httpProxy.setHeaders( httpRequest.headers );
			}
			httpProxy.setReadyStateHandler(
				function()
				{
					if( httpProxy.getReadyState() == HttpProxy.COMPLETED ) {
						AsyncHttpUtil.handleResponse( httpRequest, httpProxy, readyStateHandler );
					}
				}
			);
			httpProxy.send(httpRequest.getSerializedData());
		}
	},
	
	//////////
	//
	// name: waitRequest
	// desc: Since setTimeout can not accept objects as parameters,
	//		 AsyncHttpUtil.request()'s setTimeout function calls
	//		 AsyncHttpUtil.waitRequest() instead, which accepts a
	//		 number of variants. AsyncHttpUtil.waitRequest() takes the
	//		 variants, assembles the appropriate objects to interface
	//		 with AsyncHttpUtil.request(), and calls it.
	//
	//		 TODO: the NVP deserialization should happen in StringUtils
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	"waitRequest": function( url, type, data, header, readyStateHandler, waitCount )
	{
		var httpRequest = new HttpRequest();
		httpRequest.setUrl( url );
		httpRequest.setType( type );
		
		var mapData = new Array();
		var arrData = new String( data ).split("&");
		for( var i = 0; i < arrData.length; i++ ) {
			var nvp   = arrData[i].split( "=" );
			var name  = nvp[0];
			var value = nvp[1];
			if( nvp.length > 1 ) {
				mapData[name] = value;
			}
		}

		var mapHeaders = new Array();
		var arrHeader = new String( header ).split("&");
		for( var j = 0; j < arrHeader.length; j++ ) {
			var nvp   = arrHeader[j].split( "=" );
			var name  = nvp[0];
			var value = nvp[1];
			if( nvp.length > 1 ) {
				mapHeaders[name] = value;
			}
		}
		
		httpRequest.setData( mapData );
		httpRequest.setHeaders( mapHeaders );

		AsyncHttpUtil.request( httpRequest, readyStateHandler, waitCount );
	},

	//////////
	//
	// name: handleResponse
	// desc: This method acts as the generic onreadystatechange event handler.
	//		 It is passed all the information describing its request context.
	//		 It is also passed the user defined readyStateHandler.
	// auth: William Cava <william.cava@ektron.com>
	// date: May 2005
	//

	"handleResponse": function( httpRequest, httpProxy, readyStateHandler )
	{
		var document = httpProxy.getDocument();
		var status = httpProxy.getStatus();

		if( status != 200 ) {
			var responseText = "";
			try {
				responseText = httpProxy.xmlhttp.responseText;
			} catch( e ) {
				; // noop
			}
			
			var err  = "HTTP error: expected 200, received " + status;
			err += " type=" + httpRequest.type;
			err += " url=" + httpRequest.url;
			err += " data=" + httpRequest.getSerializedData();
			err += " headers=" + httpRequest.getSerializedHeaders();
			err += " responseText=" + responseText;

			LogUtil.addMessage(
					LogUtil.CRITICAL,
					"AsyncHttpUtil",
					"handleResponse",
					err );

			// TODO: Need to define the types of exceptions that can be returned
			// throw Exception.HTTPError(e);
		} else {

			// 200 success trace log
			LogUtil.addMessage(
					LogUtil.DEBUG,
					"AsyncHttpUtil",
					"handleResponse",
					"HTTP 200: data=" + 
					httpProxy.xmlhttp.responseText );
		}

		// do we have a document?
		if( document != null) {

			if( document.parseError != 0 ) {
				// some type of parse error
				try {
				var xmlerr = "";
				if (typeof document.parseError != "undefined") {
				  xmlerr = document.parseError.srcText;
				}
				LogUtil.addMessage( LogUtil.CRITICAL,
						"CMSAPI",
						"execute()" +
						httpRequest.url + ", " + document,
						"XML parse error: " +
						StringUtil.HTMLEncode(xmlerr)
						);
				} catch( e ) {
				}
			}

			try {
				readyStateHandler( httpRequest, httpProxy );
			} catch( e ) {
				LogUtil.addMessage( LogUtil.WARNING,
					"AsyncHttpUtil",
					"readyStateHandler",
					"Error dispatching to user defined readyStateHandler: " + 
					e.message );
			}
		}

		// we're done, release the resource
		AsyncHttpUtil.httpPool.releaseObject( httpProxy );

		return null;
	}
}

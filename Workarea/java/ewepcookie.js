// DO NOT CHANGE THIS CODE
// Copyright 2002-2005, Ektron, Inc.

/*
	Include eweputil.js (for trim).
	Include ewepfind.js to use evalDocument, evalWindow.
	
	Class: ewepCookie - class that provides easy access to document cookies
	(uses ewepfind.js functions)
	
	Properties:
		expiresInSeconds - cookie will expire given number of seconds after being set
		evalDocument
		evalWindow
		
	Methods:
		setCookie([name,] value [, expires] [, path] [, domain] [, secure])
		getCookie([name])
		removeCookie([name])
						
	Events:
		none
*/

// Special thanks to: Bill Dortch, hIdaho Design <BDORTCH@NETW.COM> who released similar functions to the public domain.
// Reference: http://www.cookiecentral.com/code/js_cookie8.htm

var g_eWebEditProCookie_Count = 0;
function eWebEditProCookie(name, evalDocument, evalWindow)
{
	// optional name, evalDocument, evalWindow
	if (name)
	{
		this.name = name;
	}
	else
	{
		this.name = "cookie" + g_eWebEditProCookie_Count++;
	}
	this.evalDocument = evalDocument;
	this.evalWindow = evalWindow;
	
	if ("function" == typeof findDocument)
	{
		this.findDocument = findDocument;
	}
	else
	{
		this.findDocument = function() { return document; };
	}
	
	this.expiresInSeconds = 3 * 365 * 24 * 60 * 60; // expire in 3 years
	
	this.setCookie = eWebEditProCookie_setCookie;
	this.getCookie = eWebEditProCookie_getCookie;
	this.removeCookie = eWebEditProCookie_removeCookie;
}

function eWebEditProCookie_setCookie(args)
{
	var expDateDefault = new Date();
	expDateDefault.setTime(expDateDefault.getTime() + this.expiresInSeconds * 1000); 

	var argv = this.setCookie.arguments;
	var argc = this.setCookie.arguments.length;
	var name = "";
	var value = "";
	if (argc == 1)
	{
		name = this.name;
		value = argv[0];
	}
	else if (argc >= 2)
	{
		name = argv[0];
		value = argv[1];
	}
	var expires = (argc > 2) ? argv[2] : expDateDefault;
	var path = (argc > 3) ? argv[3] : null;
	var domain = (argc > 4) ? argv[4] : null;
	var secure = (argc > 5) ? argv[5] : false;
	
	if ("object" == typeof expires)
	{
		expires = expires.toGMTString();
	}

	var objDoc = this.findDocument(this.evalDocument, this.evalWindow);
	objDoc.cookie = name + "=" + escape(value) +
	        (expires ? "; expires=" + expires : "") +
	        (path ? "; path=" + path : "") +
	        (domain ? "; domain=" + domain : "") +
	        (secure ? "; secure" : "");
}

function eWebEditProCookie_getCookie(name)
{
	// optional name
	if (!name)
	{
		name = this.name;
	}
	var objDoc = this.findDocument(this.evalDocument, this.evalWindow);
	var strCookie = objDoc.cookie + "";
	var aryPairs = strCookie.split(";");
	var pair = [];
	for (var i = 0; i < aryPairs.length; i++) 
	{
		pair = aryPairs[i].split("=");
		if (pair.length == 2)
		{
			var key; 
			key = pair[0] + "";
			key = eWebEditProUtil_trim(key);
			if (key == name)
			{
				// found
				return unescape(pair[1]);
			}
		}
	}
	return; // "undefined"
}

function eWebEditProCookie_removeCookie(name)
{
	// optional name
	if (!name)
	{
		name = this.name;
	}
	var expDate = new Date();
	expDate.setTime(expDate.getTime() - 1);  // set expiration in the past to remove it
	
	var objDoc = this.findDocument(this.evalDocument, this.evalWindow);
	objDoc.cookie = name + "=; expires=" + expDate.toGMTString();
}


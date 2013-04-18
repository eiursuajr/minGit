/* Used in EkRadEditor...DialogLoaderBase.cs), but copied into ekxbrowser.js on 2009-06-17 */

// DO NOT CHANGE THIS CODE
// Copyright 2000-2007, Ektron, Inc.

// static, not exposed as method in this class, use queryArgs[]
function EkUtil_parseQuery()
{
	var objQuery = new Object();
	var strQuery = location.search.substring(1);
	// escape() encodes space as "%20".
	// If space is encoded as "+", then use the following line
	// in your customized function.
	// strQuery = strQuery.replace(/\+/g, " ");
	var aryQuery = strQuery.split("&");
	var pair = [];
	for (var i = 0; i < aryQuery.length; i++)
	{
		pair = aryQuery[i].split("=");
		if (2 == pair.length)
		{
			objQuery[unescape(pair[0])] = unescape(pair[1]);
		}
	}
	return objQuery;
}

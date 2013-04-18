// Copyright © 2001 by Apple Computer, Inc., All Rights Reserved.
//
// You may incorporate this Apple sample code into your own code
// without restriction. This Apple sample code has been provided "AS IS"
// and the responsibility for its operation is yours. You may redistribute
// this code, but you are not permitted to redistribute it as
// "Apple sample code" after having made changes.

// ugly workaround for missing support for selectorText in Netscape6/Mozilla
// call onLoad() or before you need to do anything you would have otherwise used
// selectorText for.
var ugly_selectorText_workaround_flag = true;
var allStyleRules;
// code developed using the following workaround (CVS v1.15) as an example.
// http://lxr.mozilla.org/seamonkey/source/extensions/xmlterm/ui/content/XMLTermCommands.js
function ugly_selectorText_workaround() {
    return; // v8.0
	if((navigator.userAgent.indexOf("Gecko") == -1) ||
	   (ugly_selectorText_workaround_flag)) {
		return; // we've already been here or shouldn't be here
	}
	var styleElements = document.getElementsByTagName("style");

	for(var i = 0; i < styleElements.length; i++) {
		var styleText = styleElements[i].firstChild.data;
		// this should be using match(/\b[\w-.]+(?=\s*\{)/g but ?= causes an
		// error in IE5, so we include the open brace and then strip it
		allStyleRules = styleText.match(/\b[\w-.]+(\s*\{)/g);
	}

	for(var i = 0; i < allStyleRules.length; i++) {
		// probably insufficient for people who like random gobs of
		// whitespace in their styles
		allStyleRules[i] = allStyleRules[i].substr(0, (allStyleRules[i].length - 2));
	}
	ugly_selectorText_workaround_flag = true;
}


// setStyleById: given an element id, style property and
// value, apply the style.
// args:
//  i - element id
//  p - property
//  v - value
//
function setStyleById(i, p, v) {
	var n = document.getElementById(i);
	n.style[p] = v;
}

// getStyleById: given an element ID and style property
// return the current setting for that property, or null.
// args:
//  i - element id
//  p - property
function getStyleById(i, p) {
	var n = document.getElementById(i);
	var s = eval("n.style." + p);

	// try inline
	if((s != "") && (s != null)) {
		return s;
	}

	// try currentStyle
	if(n.currentStyle) {
		var s = eval("n.currentStyle." + p);
		if((s != "") && (s != null)) {
			return s;
		}
	}

	// try styleSheets
	var sheets = document.styleSheets;
	if(sheets.length > 0) {
		// loop over each sheet
		for(var x = 0; x < sheets.length; x++) {
			// grab stylesheet rules
			var rules = new Object;
			if (document.all) {
				rules = sheets[x].rules;
			}
			else {
				rules = sheets[x].cssRules;
			}
			if(rules.length > 0) {
				// check each rule
				for(var y = 0; y < rules.length; y++) {
					var z = rules[y].style;
					// selectorText broken in NS 6/Mozilla: see
					// http://bugzilla.mozilla.org/show_bug.cgi?id=51944
					//ugly_selectorText_workaround();
					if(allStyleRules) {
						if(allStyleRules[y] == i) {
							return z[p];
						}
					} else {
						// use the native selectorText and style stuff
						if(((z[p] != "") && (z[p] != null)) ||
						   (rules[y].selectorText == i)) {
							return z[p];
						}
					}
				}
			}
		}
	}
	return null;
}

// setStyleByClass: given an element type and a class selector,
// style property and value, apply the style.
// args:
//  t - type of tag to check for (e.g., SPAN)
//  c - class name
//  p - CSS property
//  v - value

function setStyleByCssClass(t, c, p, v){
	var sheets = document.styleSheets;
	if(sheets.length > 0) {
		// loop over each sheet
		for(var x = 0; x < sheets.length; x++) {
			// grab stylesheet rules
			if (!document.all) {
				var rules = sheets[x].cssRules;
			}
			else {
				var rules = sheets[x].rules;
			}
			if(rules.length > 0) {
				// check each rule
				for(var y = 0; y < rules.length; y++) {
					var z = rules[y].style;
					// selectorText broken in NS 6/Mozilla: see
					// http://bugzilla.mozilla.org/show_bug.cgi?id=51944
					//ugly_selectorText_workaround();
					if(allStyleRules) {
						if((allStyleRules[y] == c) ||
						   (allStyleRules[y] == (t + "." + c))) {
							z[p] = v;
						}
					} else {
						// use the native selectorText and style stuff
						// update: Safari returns extra characters, filter them out:
						var cleanedRules = rules[y].selectorText;
						var rulesLoc = cleanedRules.indexOf('[');
						if (0 < rulesLoc) {
							cleanedRules = cleanedRules.substr(0,rulesLoc)
						}
						if(((z[p] != "") && (z[p] != null)) &&
						   ((cleanedRules == c) ||
						    (cleanedRules == ("." + c)) ||
						    (cleanedRules == (t.toUpperCase() + "." + c)) ||
						    (cleanedRules == (t.toLowerCase() + "." + c))
						    )) {
							z[p] = v;
						}
					}
				}
			}
		}
	}
}
// setStyleByClass: given an element type and a class selector,
// style property and value, apply the style.
// args:
//  t - type of tag to check for (e.g., SPAN)
//  c - class name
//  p - CSS property
//  v - value
var ie = (document.all) ? true : false;

function setStyleByClass(t,c,p,v){
	var elements;
	if(t == '*') {
		// '*' not supported by IE/Win 5.5 and below
		elements = (ie) ? document.all : document.getElementsByTagName('*');
	} else {
		elements = document.getElementsByTagName(t);
	}
	for(var i = 0; i < elements.length; i++){
		var node = elements.item(i);
		for(var j = 0; j < node.attributes.length; j++) {
			if(node.attributes.item(j).nodeName == 'class') {
				if(node.attributes.item(j).nodeValue == c) {
					eval('node.style.' + p + " = '" +v + "'");
				}
			}
		}
	}
}

// getStyleByClass: given an element type, a class selector and a property,
// return the value of the property for that element type.
// args:
//  t - element type
//  c - class identifier
//  p - CSS property
function getStyleByClass(t, c, p) {
	// first loop over elements, because if they've been modified they
	// will contain style data more recent than that in the stylesheet
	var elements;
	if(t == '*') {
		// '*' not supported by IE/Win 5.5 and below
		elements = (ie) ? document.all : document.getElementsByTagName('*');
	} else {
		elements = document.getElementsByTagName(t);
	}
	for(var i = 0; i < elements.length; i++){
		var node = elements.item(i);
		if (node.attributes != null) {
			for(var j = 0; j < node.attributes.length; j++) {
				if(node.attributes.item(j).nodeName == 'class') {
					if(node.attributes.item(j).nodeValue == c) {
						var theStyle = eval('node.style.' + p);
						if((theStyle != "") && (theStyle != null)) {
							return theStyle;
						}
					}
				}
			}
		}
	}
	// if we got here it's because we didn't find anything
	// try styleSheets
	var sheets = document.styleSheets;
	if(sheets.length > 0) {
		// loop over each sheet
		for(var x = 0; x < sheets.length; x++) {
			// grab stylesheet rules
			if (!document.all) {
				var rules = sheets[x].cssRules;
			}
			else {
				var rules = sheets[x].rules;
			}
			if(rules.length > 0) {
				// check each rule
				for(var y = 0; y < rules.length; y++) {
					var z = rules[y].style;
					// selectorText broken in NS 6/Mozilla: see
					// http://bugzilla.mozilla.org/show_bug.cgi?id=51944
					//ugly_selectorText_workaround();
					if(allStyleRules) {
						if((allStyleRules[y] == c) ||
						   (allStyleRules[y] == (t + "." + c))) {
							return z[p];
						}
					} else {
						// use the native selectorText and style stuff
						// update: Safari returns extra characters, filter them out:
						var cleanedRules = rules[y].selectorText;
						var rulesLoc = cleanedRules.indexOf('[');
						if (0 < rulesLoc) {
							cleanedRules = cleanedRules.substr(0,rulesLoc)
						}
						if(((z[p] != "") && (z[p] != null)) &&
						   ((cleanedRules == c) ||
						    (cleanedRules == ("." + c)) ||
						    (cleanedRules == (t.toUpperCase() + "." + c)) ||
						    (cleanedRules == (t.toLowerCase() + "." + c))
						    )) {
							return z[p];
						}
					}
				}
			}
		}
	}
	return null;
}

// getStyleByCssClass: given an element type, a class selector and a property,
// return the value of the property for that element type.
// args:
//  t - element type
//  c - class identifier
//  p - CSS property
function getStyleByCssClass(t, c, p) {
    // try styleSheets
	var sheets = document.styleSheets;
	if(sheets.length > 0) {
		// loop over each sheet
		for(var x = 0; x < sheets.length; x++) {
			// grab stylesheet rules
			var rules = "";
			try
			{
			    if (!document.all) {
				    rules = sheets[x].cssRules;
			    }
			    else {
				    rules = sheets[x].rules;
			    }
			}
			catch(ex)
			{
			    // do nothing
			}
			if(rules.length > 0) {
				// check each rule
				for(var y = 0; y < rules.length; y++) {
					var z = rules[y].style;
					// selectorText broken in NS 6/Mozilla: see
					// http://bugzilla.mozilla.org/show_bug.cgi?id=51944
					//ugly_selectorText_workaround();
					if(allStyleRules) {
						if((allStyleRules[y] == c) ||
						   (allStyleRules[y] == (t + "." + c))) {
						    return z[p];
						}
					} else {
						// use the native selectorText and style stuff
						// update: Safari returns extra characters, filter them out:
						var cleanedRules = rules[y].selectorText;
						if (cleanedRules != null)
						{
						    var rulesLoc = cleanedRules.indexOf('[');
						    if (0 < rulesLoc) {
							    cleanedRules = cleanedRules.substr(0,rulesLoc)
						    }
						    if(((z[p] != "") && (z[p] != null)) &&
						       ((cleanedRules == c) ||
						        (cleanedRules == ("." + c)) ||
						        (cleanedRules == (t.toUpperCase() + "." + c)) ||
						        (cleanedRules == (t.toLowerCase() + "." + c))
						        )) {
						        return z[p];
						    }
						}
					}
				}
			}
		}
	}
	return null;
}

// updateBackgroundImageStyleByCssClass: given an element type, a class selector and a property,
// return the value of the property for that element type.
// args:
//  t - element type
//  c - class identifier
//  p - CSS property
//	PathPrefix - New prefix to the image filename to complete the path
//	StripPath - Possible path that may already exist in the image path. Netscape likes to prefix the
//				path from the web root before giving it to us.
function updateBackgroundImageStyleByCssClass(t, c, p, PathPrefix, StripPath) {
	// try styleSheets
	var sheets = document.styleSheets;
	if(sheets.length > 0) {
		// loop over each sheet
		for(var x = 0; x < sheets.length; x++) {
			// grab stylesheet rules
			if (!document.all) {
				var rules = sheets[x].cssRules;
			}
			else {
				var rules = sheets[x].rules;
			}
			if(rules.length > 0) {
				// check each rule
				for(var y = 0; y < rules.length; y++) {
					var z = rules[y].style;
					// selectorText broken in NS 6/Mozilla: see
					// http://bugzilla.mozilla.org/show_bug.cgi?id=51944
					//ugly_selectorText_workaround();
					if(allStyleRules) {
						if((allStyleRules[y] == c) ||
						   (allStyleRules[y] == (t + "." + c))) {
							return z[p];
						}
					} else {
						// use the native selectorText and style stuff
						// update: Safari returns extra characters, filter them out:
						var cleanedRules = rules[y].selectorText;
						if (cleanedRules != null)
						{
						    var rulesLoc = cleanedRules.indexOf('[');
						    if (0 < rulesLoc) {
							    cleanedRules = cleanedRules.substr(0,rulesLoc)
						    }
						    if(((z[p] != "") && (z[p] != null)) &&
						       ((cleanedRules == c) ||
						        (cleanedRules == ("." + c)) ||
						        (cleanedRules == (t.toUpperCase() + "." + c)) ||
						        (cleanedRules == (t.toLowerCase() + "." + c))
						        )) {
								    var bgUrl = z[p].toLowerCase();
								    bgUrl = bgUrl.replace("url(", "");
								    if (StripPath != null) {
									    if (StripPath.length > 0) {
										    position = bgUrl.indexOf(StripPath);
										    if (position > -1) {
											    bgUrl = bgUrl.substring((position + StripPath.length), 1000);
										    } else {
											    // Note: ***sometimes*** we get a double slash in the
											    // strip-path that doesn't exist in bgUrl; filter those:
											    var tmpStripPath = CleanDblSlash(StripPath);
											    position = bgUrl.indexOf(tmpStripPath);
											    if (position > -1) {
												    bgUrl = bgUrl.substring((position + tmpStripPath.length), 1000);
											    }
										    }
									    }
								    }

							    // only use prefix if not already included (sometimes already exits):
							    if ((bgUrl.toLowerCase()).indexOf(PathPrefix.toLowerCase()) < 0) {
								    z[p] = "url(" + PathPrefix + bgUrl;
							    } else {
								    z[p] = "url(" + bgUrl;
							    }

							    return true;
						    }
						}
					}
				}
			}
		}
	} else {
		return false;
	}
}

// setStyleByTag: given an element type, style property and
// value, and whether the property should override inline styles or
// just global stylesheet preferences, apply the style.
// args:
//  e - element type or id
//  p - property
//  v - value
//  g - boolean 0: modify global only; 1: modify all elements in document
function setStyleByTag(e, p, v, g) {
	if(g) {
		var elements = document.getElementsByTagName(e);
		for(var i = 0; i < elements.length; i++) {
			elements.item(i).style[p] = v;
		}
	} else {
		var sheets = document.styleSheets;
		if(sheets.length > 0) {
			for(var i = 0; i < sheets.length; i++) {
				if (!document.all) {
					var rules = sheets[i].cssRules;
				}
				else {
					var rules = sheets[i].rules;
				}
				if(rules.length > 0) {
					for(var j = 0; j < rules.length; j++) {
						var s = rules[j].style;
						// selectorText broken in NS 6/Mozilla: see
						// http://bugzilla.mozilla.org/show_bug.cgi?id=51944
						//ugly_selectorText_workaround();
						if(allStyleRules) {
							if(allStyleRules[j] == e) {
								s[p] = v;
							}
						} else {
							// use the native selectorText and style stuff
							if(((s[p] != "") && (s[p] != null)) &&
							   (rules[j].selectorText == e)) {
								s[p] = v;
							}
						}

					}
				}
			}
		}
	}
}

// getStyleByTag: given an element type and style property, return
// the property's value
// args:
//  e - element type
//  p - property
function getStyleByTag(e, p) {
	var sheets = document.styleSheets;
	if(sheets.length > 0) {
		for(var i = 0; i < sheets.length; i++) {
			if (!document.all) {
				var rules = sheets[i].cssRules;
			}
			else {
				var rules = sheets[i].rules;
			}
			if(rules.length > 0) {
				for(var j = 0; j < rules.length; j++) {
					var s = rules[j].style;
					// selectorText broken in NS 6/Mozilla: see
					// http://bugzilla.mozilla.org/show_bug.cgi?id=51944
					//ugly_selectorText_workaround();
					if(allStyleRules) {
						if(allStyleRules[j] == e) {
							return s[p];
						}
					} else {
						// use the native selectorText and style stuff
						if(((s[p] != "") && (s[p] != null)) &&
						   (rules[j].selectorText == e)) {
							return s[p];
						}
					}

				}
			}
		}
	}

	// if we don't find any style sheets, return the value for the first
	// element of this type we encounter without a CLASS or STYLE attribute
	var elements = document.getElementsByTagName(e);
	var sawClassOrStyleAttribute = false;
	for(var i = 0; i < elements.length; i++) {
		var node = elements.item(i);
		for(var j = 0; j < node.attributes.length; j++) {
			if((node.attributes.item(j).nodeName == 'class') ||
			   (node.attributes.item(j).nodeName == 'style')){
			   sawClassOrStyleAttribute = true;
			}
		}
		if(! sawClassOrStyleAttribute) {
			return elements.item(i).style[p];
		}
	}
}

function MakeClassPathRelative(TagType, ClassName, Property, PathPrefix, StripPath, loops) {
    return;
	var giveUpCnt = 50;
	// initialize the loop counter:
	if ((typeof(loops)).toLowerCase() == 'undefined') {
		var loops = 0;
	}

	var loopCnt = parseInt(loops, 10);
	if (loopCnt >= giveUpCnt){
		return;
	}

	try {

	    if (null != getStyleByCssClass(TagType, ClassName, Property)) {
		TagType = TagType.toLowerCase();
		StripPath = StripPath.toLowerCase();
		updateBackgroundImageStyleByCssClass(TagType, ClassName, Property, PathPrefix, StripPath);
	    } else {
		var delayFuncName = "MakeClassPathRelative('" + TagType + "', '" + ClassName + "', '" + Property + "', '" + PathPrefix + "', '" + StripPath + "', '" + (loopCnt + 1)+ "')";
		setTimeout(delayFuncName, 100);
	    }
	  }
	catch(ex){
		   //Do Nothing
	 }
}

function CanAccessStyleSheets() {
	var sheets = document.styleSheets;
	return (sheets.length > 0);
}

function IsBrowserSafari() {
	var posn;
	posn = parseInt(navigator.appVersion.indexOf('Safari'));
	return (0 <= posn);
}

var m_DebugWindow=null;
function DebugMsg(Msg) {
    Msg = '>>>' + Msg + ' <br> ';
    if ((m_DebugWindow == null) || (m_DebugWindow.closed)) {
        m_DebugWindow = window.open('', 'myWin', 'toolbar=no, directories=no, location=no, status=yes, menubar=no, resizable=yes, scrollbars=yes, width=500, height=300');
    }
    m_DebugWindow.document.writeln(Msg);
}

function CleanDblSlash(strText) {
	var idx, retStr;
	retStr = '';
	for (idx=0; idx < strText.length; idx++) {
		if (idx>0){
			if ((strText[idx] != '/') || (strText[idx-1] != '/')) {
				retStr += strText[idx];
			}
		} else {
			retStr += strText[idx];
		}
	}
	return (retStr);
}

function hideMultiMenu () {
	if (document.all) {
		document.all.xmladdMenu.style.visibility = 'hidden';
		return false;
	} else if (document.layers) {
		document.xmladdMenu.visibility = 'hide';
		return false;
	} else if (!document.all && document.getElementById) {
		document.getElementById('xmladdMenu').style.visibility = 'hidden';
		return false;
	}
	return true;
}

function showMultiMenu (evt) {
    // can't generalize this because only one param for onclick event w/ some browsers like Firefox
	var myevt = evt ? evt : window.event;
	if (document.all) {
		document.all.xmladdMenu.style.pixelLeft = myevt.clientX;
		document.all.xmladdMenu.style.pixelTop = myevt.clientY;
		document.all.xmladdMenu.style.visibility = 'visible';
		return false;
	} else if (document.layers) {
		if (myevt.which == 3) {
			document.xmladdMenu.left = myevt.x;
			document.xmladdMenu.top = myevt.y;
			document.xmladdMenu.onmouseout = hideMultiMenu;
			document.xmladdMenu.visibility = 'show';
			return false;
		}
	} else if (!document.all && document.getElementById) {
		if (myevt != undefined) {
			document.getElementById('xmladdMenu').style.left = myevt.pageX+'px';
			document.getElementById('xmladdMenu').style.top = myevt.pageY+'px';
			document.getElementById('xmladdMenu').style.visibility = 'visible';
			return false;
		}
	}
	return true;
}

function ekxbrowserCheck(domClassName)
{
	if (null == domClassName) return false;
	if ("undefined" == typeof domClassName.prototype) return false;
	if (null == domClassName.prototype) return false;
	if (typeof domClassName.prototype.__defineGetter__ != "function") return false;
	return true;
}

//source code extracted from http://www.codeproject.com/jscript/crossbrowserjavascript.asp?df=100&forumid=245519&exp=0&select=1712237

// Safari 3, HTMLElement is a function w/o prototype
if (typeof HTMLElement != "undefined" && ekxbrowserCheck(HTMLElement)) 
{
    
    // This definition needs to stay.  "parentElement" would still exist in the content.
    // see DesignToEntryXSLT.xslt, it converts the $ektron(document).parent().get(0)
    HTMLElement.prototype.__defineGetter__("parentElement",function()
    {
        if (this.parentNode == this.ownerDocument)
        {
            return null;
        }
        return this.parentNode;
    });
    
}

if (typeof Element != "undefined" && ekxbrowserCheck(Element))
{
    Element.prototype.__defineGetter__("text",function()
    {
        return this.textContent;
    });
}

// Safari does not have Event.prototype
if (typeof Event != "undefined" && ekxbrowserCheck(Event)) 
{
    Event.prototype.__defineGetter__("srcElement",function()
    {
        return this.target;
    });
    Event.prototype.__defineGetter__("offsetX",function()
    {
		var offset = 0;
		var objElem = this.target;
		while (objElem != null && objElem.tagName != "BODY")
		{
			offset += objElem.offsetLeft;
			objElem = objElem.offsetParent;
		}
        return this.clientX - offset;
    });
    Event.prototype.__defineGetter__("offsetY",function()
    {
		var offset = 0;
		var objElem = this.target;
		while (objElem != null && objElem.tagName != "BODY")
		{
			offset += objElem.offsetTop;
			objElem = objElem.offsetParent;
		}
        return this.clientY - offset;
    });
    // reference: http://www.reloco.com.ar/mozilla/compat.html
    Event.prototype.__defineSetter__("cancelBubble",function(value)
    {
        if (true == value) this.stopPropagation();
    });
    Event.prototype.__defineSetter__("returnValue",function(value)
    {
        if (false == value) this.preventDefault();
    });
}

function ekCanHaveChildren(oElem)
{
    if (oElem && /*Node.ELEMENT_NODE*/1 == oElem.nodeType)
    {
        switch(oElem.tagName.toLowerCase())
        {
            case "area":
            case "base":
            case "basefont":
            case "col":
            case "frame":
            case "hr":
            case "img":
            case "br":
            case "input":
            case "isindex":
            case "link":
            case "meta":
            case "param":
                return false;
        }
        return true;
    }
    else
    {
        return false;
    }
}

function ekHasChildren(oElem)
{
	return (ekCanHaveChildren(oElem) && $ektron(oElem).children().length > 0);
}

function ekCreateRange(sel)
{
    var rng = null;
    try
    {
        rng = sel.createRange();
    }
    catch(ex)
    {
        try
        {
            sel.clear();
            rng = sel.createRange();
        }
        catch (ex) {}
    }
    return rng;
}

function getSelectionElement(doc)
{
	var targetElement = null;
	if (doc.selection && ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9)) // IE except IE9
    {
        var sel = doc.selection;
		var rng = ekCreateRange(sel);
	    if (rng)
	    {
	        // #60630: commonParentElement() and parentElement() might not be defined in IE9. 
            if ("Control" == sel.type && typeof rng.commonParentElement != "undefined") // .type is undefined in Opera
	        {
			    targetElement = rng.commonParentElement();
	        }
	        else if (typeof rng.parentElement != "undefined")
	        {
			    targetElement = rng.parentElement();
			}
		}
    }
	else if (doc.defaultView && doc.defaultView.getSelection) // Mozilla and IE9: IE9 support both window.getSelection and doc.selection.
	{
		var sel = doc.defaultView.getSelection();
		if (sel.rangeCount > 0) {
			var rng = sel.getRangeAt(0);
			targetElement = rng.commonAncestorContainer;
			try 
			{
				// May throw "Permission denied to access property 'nodeType' from a non-chrome context
				// Seen in FF 3.5 when cursor is moved INTO input text field using arrow key.
				if (targetElement.nodeType != 1 /*Node.ELEMENT_NODE*/)
				{
					targetElement = targetElement.parentNode;
				}
			}
			catch (ex)
			{
				return null;
			}
		    var eSelCtlElem = $ektron(".design_selected_field", targetElement.ownerDocument);
		    if (0 == eSelCtlElem.length)	
		    {																
			    return targetElement;
			}
			else
			{
			    return eSelCtlElem.get(0);
			}
		}
	}
	else // Safari, et others
	{
	}
	return targetElement;
}

function ekIsMac() 
{
    var userAgent = navigator.userAgent.toLowerCase();
    if (userAgent.indexOf('mac') != -1) 
    {
        return true;
    }
    return false;
}

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

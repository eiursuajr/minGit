var m_EkTbTimeout_AjaxToolBar = null;
var m_EkTbAutomaticOutsideBorder_AjaxToolBar = true;
var m_EkTbOutsideBorder_AjaxToolBar = true;
var m_EkTbMenuOffDelay_AjaxToolBar = 500;
var m_EkTbMenuBorderWidth_AjaxToolBar = 3;

var m_EkTbStopAtRoot = 0;
var m_EkTbStopAtCss = 1;
var m_EkTbStopAtOffset = 2;
var m_EkTbLastObj;
var m_bEkTbReady;

if (m_bEkTbReady !== true) {
    m_bEkTbReady = false;
}

function GetPreviewURL() {

    var url = self.location.href;
    var arString = url.split('?');
    if (arString.length > 1) { url = url + "&cmsMode=Preview" }
    else { url = url + "?cmsMode=Preview" }
    return url;
}
function addEkTbLoadEvent() {
    var oldonload = window.onload;
    window.onload = function () {
        if (typeof oldonload == 'function') {
            oldonload();
        }
        setTimeout("m_bEkTbReady = true;", 500);
    }
}

if (typeof $ektron !== 'undefined') {
    Ektron.ready(function () {
        m_bEkTbReady = true;
    });
}

addEkTbLoadEvent();

function EkTbWebMenuPopUpWindow(url, hWind, nWidth, nHeight, nScroll, nResize) {
    var cToolBar, popupwin;
    url = url.replace(/&amp;amp;/g, "&").replace(/&amp;/g, "&");
    if (!m_bEkTbReady) return false;
    if (nWidth > screen.width) {
        nWidth = screen.width;
    }
    if (nHeight > screen.height) {
        nHeight = screen.height;
    }
    if (nWidth == screen.width && nHeight == screen.height) {
        cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + screen.width + ',height=' + screen.height;
        popupwin = window.open(url, hWind, cToolBar);
        popupwin.moveTo(0, 0);
        popupwin.resizeTo(screen.availWidth, screen.availHeight);
    } else {
        cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
        popupwin = window.open(url, hWind, cToolBar);
    }
    return popupwin;
}

function EkTbFade(opacity, speed, change, holdTime, startDelay, fadeType, name) {
    if (!m_bEkTbReady) return false;
    var MyObj = document.getElementById(name);
    if (startDelay == 0) {
        if (!document.all) {
            MyObj.style.MozOpacity = (opacity / 100);
        }
        else {
            MyObj.filters.alpha.opacity = (opacity);
        }
        if (((opacity >= 0) && (change < 0)) || ((opacity < 99) && (change > 0))) {
            opacity += change;
            if (opacity > 99) {
                opacity = 99;
            }
            setTimeout("EkTbFade(" + opacity + "," + speed + "," + change + "," + holdTime + "," + startDelay + ",'" + fadeType + "','" + name + "')", speed);
        }
        else {
            change = (0 - change);
            opacity += change;
            if (fadeType.toLowerCase() == "cycle") {
                setTimeout("EkTbFade(" + opacity + "," + speed + "," + change + "," + holdTime + "," + startDelay + ",'" + fadeType + "','" + name + "')", holdTime);
            }
        }
    }
    else {
        var tmp = startDelay;
        startDelay = 0;
        setTimeout("EkTbFade(" + opacity + "," + speed + "," + change + "," + holdTime + "," + startDelay + ",'" + fadeType + "','" + name + "')", tmp);
    }
}

var m_isMac = false;
var m_isMacInit = false;
function IsPlatformMac() {
    if (m_isMacInit) {
        return (m_isMac);
    } else {
        var posn;
        var sUsrAgent = new String(navigator.userAgent);
        sUsrAgent = sUsrAgent.toLowerCase();
        posn = parseInt(sUsrAgent.indexOf('mac'));
        m_isMac = (0 <= posn);
        m_isMacInit = true;
        return (m_isMac);
    }
}

var m_isSafari = false;
var m_isSafariInit = false;
function IsBrowserSafari() {
    if (m_isSafariInit) {
        return (m_isSafari);
    } else {
        var posn;
        var sUsrAgent = new String(navigator.userAgent);
        sUsrAgent = sUsrAgent.toLowerCase();
        posn = parseInt(sUsrAgent.indexOf('safari'));
        m_isSafari = (0 <= posn);
        m_isSafariInit = true;
        return (m_isSafari);
    }
}

function EkTbRollOver(e, MyObj) {
    if (!m_bEkTbReady) return false;
    var top = 0;
    var tmpTop = 0;
    var left = 0;
    var tmpLeft = 0;
    var width = 0;
    var height = 0;
    var toolbarNumArray = (MyObj.id).split("_");
    var automaticBorder = true;
    var outsideBorder = true;
    var menuBorderWidth = 3;
    var localTimeout = null;
    var ekTbOuterElement = null;
    // ensure previous object is deactivated (fixes Safari ghosting problem):
    if (m_EkTbLastObj && (m_EkTbLastObj != MyObj)) {
        EkTbOffNow(m_EkTbLastObj.id)
    }
    m_EkTbLastObj = MyObj;
    eval("localTimeout = m_EkTbTimeout_" + toolbarNumArray[1] + ";");
    if (localTimeout != null) {
        localTimeout = clearTimeout(localTimeout);
        eval("m_EkTbTimeout_" + toolbarNumArray[1] + " = localTimeout;");
    }

    // Mac-Safari returns invalid offsetHeight values for DIV elements,
    // which are used for the new EkWebToolbar (to bracket the content),
    // workaround - when needed, use the outer table cell instead of the DIV:
    if (IsPlatformMac()) {
        ekTbOuterElement = xBrowserReturnObjById("EkTbOuterElmt_" + toolbarNumArray[1]);
        if (ekTbOuterElement != null) {
            height = ekTbOuterElement.offsetHeight;
        } else {
            height = MyObj.offsetHeight;
        }
    } else {
        height = MyObj.offsetHeight;
    }

    tmpLeft = EkTbFindParentPositionLeft(MyObj, (m_EkTbStopAtRoot), true);
    width = MyObj.offsetWidth;
    tmpTop = EkTbFindParentPositionTop(MyObj, (m_EkTbStopAtRoot), true);
    eval("automaticBorder = m_EkTbAutomaticOutsideBorder_" + toolbarNumArray[1] + ";");
    eval("outsideBorder = m_EkTbOutsideBorder_" + toolbarNumArray[1] + ";");
    eval("menuBorderWidth = m_EkTbMenuBorderWidth_" + toolbarNumArray[1] + ";");
    if (outsideBorder == true) {
        if (((tmpLeft - menuBorderWidth) < 0) && (automaticBorder)) {
            left = 0;
        }
        else {
            left = tmpLeft - menuBorderWidth;
        }
        if (((tmpTop - menuBorderWidth) < 0) && (automaticBorder)) {
            top = 0;
        }
        else {
            top = tmpTop - menuBorderWidth;
        }
        if (((((left + width) == document.body.clientWidth) && ((tmpLeft - menuBorderWidth) < 0))
			|| (((left + width + menuBorderWidth) == document.body.clientWidth) && ((tmpLeft - menuBorderWidth) >= 0)))
			&& (automaticBorder)
			) {
            width = (document.body.clientWidth - left);
        }
        else {
            width += (menuBorderWidth * 2);
        }
        if (((((top + height) == document.body.clientHeight) && ((tmpTop - menuBorderWidth) < 0))
			|| (((top + height + menuBorderWidth) == document.body.clientHeight) && ((tmpTop - menuBorderWidth) >= 0)))
			&& (automaticBorder)
			) {
            height = (document.body.clientHeight - top);
        }
        else {
            height += (menuBorderWidth * 2);
        }
    }
    else {
        left = tmpLeft;
        top = tmpTop;
    }

    // Mac needs positioning to be adjusted:
    if (IsPlatformMac()) {
        if (IsBrowserSafari) {
            left += 4;
            top += 6;
        } else {
            left += 4;
            top += 14;
        }
    }

    if (toolbarNumArray[1] != "AjaxToolBar" && document.getElementById("EkTbLeftBar_" + toolbarNumArray[1]).parentNode != document.body) {
        var BodyObj = document.body;
        var toolObj1 = document.getElementById("EkTbLeftBar_" + toolbarNumArray[1]);
        toolObj1.parentNode.removeChild(toolObj1);
        var tmpObj1 = BodyObj.appendChild(toolObj1);
        var toolObj2 = document.getElementById("EkTbRightBar_" + toolbarNumArray[1]);
        toolObj2.parentNode.removeChild(toolObj2);
        var tmpObj2 = BodyObj.appendChild(toolObj2);
        var toolObj3 = document.getElementById("EkTbTopBar_" + toolbarNumArray[1]);
        toolObj3.parentNode.removeChild(toolObj3);
        var tmpObj3 = BodyObj.appendChild(toolObj3);
        var toolObj4 = document.getElementById("EkTbBottomBar_" + toolbarNumArray[1]);
        toolObj4.parentNode.removeChild(toolObj4);
        var tmpObj4 = BodyObj.appendChild(toolObj4);
        var toolObj5 = document.getElementById("EkTbToolbar_" + toolbarNumArray[1]);
        toolObj5.parentNode.removeChild(toolObj5);
        var tmpObj5 = BodyObj.appendChild(toolObj5);
    }
    document.getElementById("EkTbLeftBar_" + toolbarNumArray[1]).style.top = top + "px";
    document.getElementById("EkTbLeftBar_" + toolbarNumArray[1]).style.left = left + "px";
    document.getElementById("EkTbLeftBar_" + toolbarNumArray[1]).style.width = menuBorderWidth + "px";
    document.getElementById("EkTbLeftBar_" + toolbarNumArray[1]).style.height = height + "px";

    document.getElementById("EkTbRightBar_" + toolbarNumArray[1]).style.top = top + "px";
    document.getElementById("EkTbRightBar_" + toolbarNumArray[1]).style.left = ((left + width) - menuBorderWidth) + "px";
    document.getElementById("EkTbRightBar_" + toolbarNumArray[1]).style.width = menuBorderWidth + "px";
    document.getElementById("EkTbRightBar_" + toolbarNumArray[1]).style.height = height + "px"; ;

    document.getElementById("EkTbTopBar_" + toolbarNumArray[1]).style.top = top + "px";
    document.getElementById("EkTbTopBar_" + toolbarNumArray[1]).style.left = left + "px";
    document.getElementById("EkTbTopBar_" + toolbarNumArray[1]).style.width = width + "px"; ;
    document.getElementById("EkTbTopBar_" + toolbarNumArray[1]).style.height = menuBorderWidth + "px";

    document.getElementById("EkTbBottomBar_" + toolbarNumArray[1]).style.top = ((top + height) - menuBorderWidth) + "px";
    document.getElementById("EkTbBottomBar_" + toolbarNumArray[1]).style.left = left + "px";
    document.getElementById("EkTbBottomBar_" + toolbarNumArray[1]).style.width = width + "px"; ;
    document.getElementById("EkTbBottomBar_" + toolbarNumArray[1]).style.height = menuBorderWidth + "px";

    document.getElementById("EkTbLeftBar_" + toolbarNumArray[1]).style.display = "";
    document.getElementById("EkTbRightBar_" + toolbarNumArray[1]).style.display = "";
    document.getElementById("EkTbTopBar_" + toolbarNumArray[1]).style.display = "";
    document.getElementById("EkTbBottomBar_" + toolbarNumArray[1]).style.display = "";
}

function EkTbRollOut(e, MyObj) {
    if (!m_bEkTbReady) return false;
    var toElement = "";
    var srcElement = "";

    if (document.all) {
        toElement = e.toElement;
        srcElement = e.srcElement;
    }
    else {
        toElement = e.relatedTarget;
        srcElement = e.target;
    }
    if ((srcElement.id != MyObj.id)
		&& (!EkTbIsChild(srcElement, MyObj))) {
        //alert("No: " + srcElement.id);
        return;
    }
    if ((toElement == null) || (!((EkTbIsChild(toElement, MyObj)) || (MyObj.id == toElement.id)))) {
        var tmpArray = MyObj.id.split("_");
        eval("m_EkTbTimeout_" + tmpArray[1] + " = setTimeout(\"EkTbOff('" + MyObj.id + "')\", m_EkTbMenuOffDelay_" + tmpArray[1] + ")");
    }
}

function EkTbOff(MyObjId) {
    if (!m_bEkTbReady) return false;
    try {
        var tmpArray = MyObjId.split("_");
        if (eval("m_EkTbTimeout_" + tmpArray[1] + " != null")) {
            eval("m_EkTbTimeout_" + tmpArray[1] + " = null");
            document.getElementById("EkTbLeftBar_" + tmpArray[1]).style.display = "none";
            document.getElementById("EkTbRightBar_" + tmpArray[1]).style.display = "none";
            document.getElementById("EkTbTopBar_" + tmpArray[1]).style.display = "none";
            document.getElementById("EkTbBottomBar_" + tmpArray[1]).style.display = "none";
            document.getElementById("EkTbToolbar_" + tmpArray[1]).style.display = "none";
        }
    }
    catch (e) { }
}

function EkTbOffNow(MyObjId) {
    if (!m_bEkTbReady) return false;
    try {
        var tmpArray = MyObjId.split("_");
        if (eval("m_EkTbTimeout_" + tmpArray[1] + " != null")) {
            eval("m_EkTbTimeout_" + tmpArray[1] + " = null");
        }
        document.getElementById("EkTbLeftBar_" + tmpArray[1]).style.display = "none";
        document.getElementById("EkTbRightBar_" + tmpArray[1]).style.display = "none";
        document.getElementById("EkTbTopBar_" + tmpArray[1]).style.display = "none";
        document.getElementById("EkTbBottomBar_" + tmpArray[1]).style.display = "none";
        document.getElementById("EkTbToolbar_" + tmpArray[1]).style.display = "none";
    } catch (e) { }
}

function EkTbCancelOff(MyObj) {
    if (!m_bEkTbReady) return false;
    var tmpArray = MyObj.id.split("_");
    var tmpTimeout = eval("m_EkTbTimeout_" + tmpArray[1]);
    if (tmpTimeout != null) {
        clearTimeout(tmpTimeout);
        eval("m_EkTbTimeout_" + tmpArray[1] + " = null;");
    }
}

function EkTbFindParentPositionLeft(Obj, StopAt, includePositioningContainers) {
    if (!m_bEkTbReady) return false;
    var curLeft = 0;

    if (Obj.offsetParent) {
        while (Obj && (null != Obj.offsetLeft)) {
            if ((StopAt != m_EkTbStopAtRoot)
				&& (EkTbIsStopTag(Obj, StopAt))) {
                break;
            }

            if (includePositioningContainers
				|| (Obj.style
				&& Obj.style.position
				&& Obj.style.position
				&& ('relative' != Obj.style.position.toLowerCase())
				&& ('absolute' != Obj.style.position.toLowerCase()))) {
                curLeft += Obj.offsetLeft;
            }
            Obj = Obj.offsetParent;
        }
    }
    else if (Obj.x) {
        curLeft += Obj.x;
    }
    return (curLeft);
}

function EkTbFindParentPositionTop(Obj, StopAt, includePositioningContainers) {
    if (!m_bEkTbReady) return false;
    var curTop = 0;

    if (Obj.offsetParent) {
        while (Obj && (null != Obj.offsetTop)) {
            if ((StopAt != m_EkTbStopAtRoot)
				&& (EkTbIsStopTag(Obj, StopAt))) {
                break;
            }

            if (includePositioningContainers
				|| (Obj.style
				&& Obj.style.position
				&& Obj.style.position
				&& ('relative' != Obj.style.position.toLowerCase())
				&& ('absolute' != Obj.style.position.toLowerCase()))) {
                curTop += Obj.offsetTop;
            }
            Obj = Obj.offsetParent;
        }
    }
    else if (Obj.x) {
        curleft += Obj.x;
    }
    return (curTop);
}

function EkTbGetScrollTop() {
    if (EkTbIsInQuirksMode()) {
        return (document.body.scrollTop);
    }
    else {
        return (document.documentElement.scrollTop);
    }
}

function EkTbGetScrollLeft() {
    if (EkTbIsInQuirksMode()) {
        return (document.body.scrollLeft);
    }
    else {
        return (document.documentElement.scrollLeft);
    }
}

function EkTbIsInQuirksMode() {
    // document.compatMode "BackCompat" : No DocType
    // document.compatMode "CSS1Compat" : DocType specified
    // Also
    // document.documentElement.clientHeight == 0 : No DocType
    // document.documentElement.clientHeight != 0 : DocType specified
    return (0 == document.documentElement.clientHeight);
}

// Not currently used
function EkTbFindTopDifference(ChildObj, ParentObj) {
    if (!m_bEkTbReady) return false;
    var curTop = 0;

    if (ChildObj != ParentObj) {
        curTop += ChildObj.offsetTop;
        if (ChildObj.offsetParent) {
            while ((ChildObj.offsetParent) && (ChildObj != ParentObj)) {
                curTop += ChildObj.offsetTop;
                ChildObj = ChildObj.offsetParent;
            }
        }
    }
    return (curTop);
}

// Not currently used
function EkTbFindLeftDifference(ChildObj, ParentObj) {
    if (!m_bEkTbReady) return false;
    var curLeft = 0;

    if (ChildObj != ParentObj) {
        curLeft += ChildObj.offsetLeft;
        if (ChildObj.offsetParent) {
            while ((ChildObj.offsetParent) && (ChildObj != ParentObj)) {
                curLeft += ChildObj.offsetLeft;
                ChildObj = ChildObj.offsetParent;
            }
        }
    }
    return (curLeft);
}

function EkTbIsChild(obj, ParentObj) {
    if (!m_bEkTbReady) return false;
    var retVal = false;
    var tmpArray = "";
    if (obj) {
        if ((typeof obj.id == 'string') && (((obj.id).indexOf("EkTbToolbar_") > -1)
			|| ((obj.id).indexOf("EkTbLeftBar_") > -1)
			|| ((obj.id).indexOf("EkTbRightBar_") > -1)
			|| ((obj.id).indexOf("EkTbTopBar_") > -1)
			|| ((obj.id).indexOf("EkTbBottomBar_") > -1))) {
            retVal = true;
        }
        else {
            if (obj.parentNode) {
                while (obj.parentNode) {
                    obj = obj.parentNode;
                    if ((ParentObj.id).indexOf("EkTb") > -1) {
                        tmpArray = (ParentObj.id).split("_");
                    }
                    else {
                        tmpArray = new Array;
                        tmpArray[0] = "";
                        tmpArray[1] = "";
                    }
                    if ((obj == ParentObj)
						|| (obj == document.getElementById("EkTbToolbar_" + tmpArray[1]))
						|| (obj == document.getElementById("EkTbLeftBar_" + tmpArray[1]))
						|| (obj == document.getElementById("EkTbRightBar_" + tmpArray[1]))
						|| (obj == document.getElementById("EkTbTopBar_" + tmpArray[1]))
						|| (obj == document.getElementById("EkTbBottomBar_" + tmpArray[1]))) {
                        retVal = true;
                        break;
                    }
                }
            }
        }
    }
    return (retVal);
}

function EkTbIsStopTag(Obj, StopAt) {
    if (!m_bEkTbReady) return false;
    var retVal = false;

    if ((((Obj.style.position).toLowerCase() == "relative") && (StopAt & m_EkTbStopAtCss))
		 || (((Obj.style.position).toLowerCase() == "absolute") && (StopAt & m_EkTbStopAtOffset))) {
        if (!document.all) {
            if ((Obj.tagName).toLowerCase() != "table") {
                retVal = true;
            }
        }
        else {
            retVal = true;
        }
    }
    return (retVal);
}

function EkTbMacShowToolbar(e, MyObj) {
    if (!m_bEkTbReady) return false;
    var retVal = true;

    if (navigator.userAgent.indexOf("Mac") > -1) {
        if (e.shiftKey) {
            retVal = EkTbShowToolbar(e, MyObj);
        }
    }
    return (retVal);
}

function EkTbShowToolbar(e, MyObj) {
    if (!m_bEkTbReady) return false;
    var retVal = true;
    var toElement = "";
    var srcElement = "";
    var locationX = 0;
    var locationY = 0;
    if (document.all) {
        toElement = e.toElement;
        srcElement = e.srcElement;
        if (e.y == event.clientY) {
            // operate normally
            locationY = (e.y + (EkTbFindParentPositionTop(srcElement, m_EkTbStopAtRoot, false) - EkTbFindParentPositionTop(srcElement, m_EkTbStopAtCss, false) + EkTbGetScrollTop()));
            locationX = (e.x + (EkTbFindParentPositionLeft(srcElement, m_EkTbStopAtRoot, false) - EkTbFindParentPositionLeft(srcElement, m_EkTbStopAtCss, false) + EkTbGetScrollLeft()));
        }
        else {
            //special case for a div in a div (blogs)
            //alert('Y= ' + e.y + " - " + event.clientY + " - " + event.screenY + " - " + ((EkTbFindParentPositionTop(srcElement, m_EkTbStopAtRoot) - EkTbFindParentPositionTop(srcElement, m_EkTbStopAtCss) + EkTbGetScrollTop()) ));
            locationY = (event.clientY + EkTbFindParentPositionTop(srcElement, m_EkTbStopAtRoot) - EkTbFindParentPositionTop(srcElement, m_EkTbStopAtCss) + EkTbGetScrollTop());
            if (event.screenX != e.clientX) {
                locationX = (event.clientX + (e.x + (EkTbFindParentPositionLeft(srcElement, m_EkTbStopAtRoot, false) - EkTbFindParentPositionLeft(srcElement, m_EkTbStopAtCss, false) + EkTbGetScrollLeft()))) - e.x;
            }
            else {
                locationX = (event.screenX); // e.x + (EkTbFindParentPositionLeft(srcElement, m_EkTbStopAtRoot, false)  - EkTbFindParentPositionLeft(srcElement, m_EkTbStopAtCss, false) + EkTbGetScrollLeft()) );
            }
        }
    }
    else {
        toElement = e.relatedTarget;
        srcElement = e.target;
        locationY = e.pageY;
        locationX = e.pageX;
    }
    if (!e.ctrlKey) {
        var tmpArray = MyObj.id.split("_");
        var rightEdge = (EkTbFindParentPositionLeft(document.getElementById("EkTbBase_" + tmpArray[1]), (m_EkTbStopAtRoot), true) + document.getElementById("EkTbBase_" + tmpArray[1]).offsetWidth);
        if (locationX > rightEdge) {
            locationX = rightEdge;
        }
        document.getElementById("EkTbToolbar_" + tmpArray[1]).style.top = ((locationY)) + "px";
        document.getElementById("EkTbToolbar_" + tmpArray[1]).style.left = ((locationX)) + "px";
        document.getElementById("EkTbToolbar_" + tmpArray[1]).style.display = "";
        retVal = false;
    }
    return (retVal);
}

function EkTbStopBubble(e, MyObj) {
    e.cancelBubble = true;
}


function xBrowserReturnObjById(id) {
    if (document.getElementById) {
        var returnVar = document.getElementById(id);
    }
    else if (document.all) {
        var returnVar = document.all[id];
    }
    else if (document.layers) {
        var returnVar = document.layers[id];
    }
    return returnVar;
}

/*********************************************************/
/************** Begin Ektron Editor's Menu ***************/
/*********************************************************/
if (typeof $ektron !== 'undefined') {
    Ektron.EditorsMenu = {
        bindEvents: function () {

            //show menu when mouseover marker
            $ektron(document).listen("mouseover", "a.EktronEditorsMenuMarker", function (e) {
                Ektron.EditorsMenu.show(this, e, true);
            });
            $ektron(document).listen("keypress", "a.EktronEditorsMenuMarker", function (e) {
                if (e.keyCode === 13)  //show only if pressed key is 'enter'
                    Ektron.EditorsMenu.show(this, e, false);
            });

            //hide menu on timeout when mouseleave menu
            $ektron(document).listen("mouseout", "a", function (e) {
                var menu = $ektron(this).parent().parent();
                if (menu.hasClass("EktronEditorsMenu")) {
                    var timeoutId = setTimeout(function () {
                        Ektron.EditorsMenu.hide(menu);
                    }, Ektron.EditorsMenu.timeoutDuration);
                    menu.attr("timeoutId", timeoutId);
                }
            });

            //clear timeout when mouseenter menu item
            $ektron(document).listen("mouseover", "a", function (e) {
                var menu = $ektron(this).parent().parent();
                if (menu.hasClass("EktronEditorsMenu")) {
                    clearTimeout(menu.attr("timeoutId"));
                    menu.removeAttr("timeoutId");
                }
            });

            //hide menu on timeout when menu item link loses focus
            $ektron(document).listen("blur", "a", function (e) {
                var menu = $ektron(this).parent().parent();
                if (menu.hasClass("EktronEditorsMenu")) {
                    var timeoutId = setTimeout(function () {
                        Ektron.EditorsMenu.hide(menu);
                    }, Ektron.EditorsMenu.timeoutDuration);
                    menu.attr("timeoutId", timeoutId);
                }
            });

            //clear timeout when menu item link gains focus
            $ektron(document).listen("focus", "a", function (e) {
                var menu = $ektron(this).parent().parent();
                if (menu.hasClass("EktronEditorsMenu")) {
                    clearTimeout(menu.attr("timeoutId"));
                    menu.removeAttr("timeoutId");
                }
            });
        },

        show: function (marker, e, autoHide) {
            //get menu ul (sibling of img marker
            var menuId = $ektron(marker).attr("data-ektron-editorsmenu-id");
            var menu = $ektron("#" + menuId);

            //skip if menu is visibile
            if (menu.css("display") === "none" && menu.hasClass("cloned") === false) {



                //don't layer if menu preceeds an iframe, object, or embed
                var layerMenu = true;
                if (menu.parent().find('iframe').length > 0)
                    layerMenu = false;
                if (menu.parent().find('object').length > 0)
                    layerMenu = false;
                if (menu.parent().find('embed').length > 0)
                    layerMenu = false;

                if (layerMenu === false) {
                    //push following content down
                    menu.attr("class", "EktronEditorsMenu EktronEditorsMenuNotLayered");
                    menu.css("display", "block");
                } else {

                    //clone menu & position at marker position
                    menu.addClass("cloned");
                    menu = menu.clone(true);
                    menu.prependTo("body");
                    menu.attr("cloneid", menu.attr("id"));
                    menu.removeAttr("id");
                    menu.addClass("EktronEditorsMenu cloned");


                    var zIndex = 99998;

                    menu.siblings("ul.EktronEditorsMenu").each(function () {
                        menu.siblings("ul.EktronEditorsMenu").length;
                        if (zIndex <= parseInt($ektron(this).css("z-index"))) {
                            zIndex = parseInt($ektron(this).css("z-index")) + 1;
                        }
                    });
                    menu.css("z-index", String(zIndex));

                    Ektron.EditorsMenu.setPosition(menu);
                    menu.slideDown(function () {
                        if (autoHide === false) {
                            //trigger: keyboard - don't autohide & set focus on first menu item link
                            menu.find("li:first a:first").focus();
                        }
                    });
                }

                //set an attribute that contains the timeout id
                menu.attr("timeoutId", timeoutId);

                if (autoHide === true) {
                    //trigger: mouseover - hide menu if no menu item mouseover
                    var timeoutId = setTimeout(function () {
                        Ektron.EditorsMenu.hide(menu);
                    }, Ektron.EditorsMenu.timeoutDuration);
                    menu.attr("timeoutId", timeoutId);
                }
            }
        },

        hide: function (menu) {
            menu.slideUp("fast", function () {
                if (menu.hasClass("cloned") === true) {
                    //menu is cloned and layered on top of content
                    var clonedMenu = $ektron("#" + menu.attr("cloneid"));
                    clonedMenu.removeClass("cloned");
                    clonedMenu.removeAttr("timeoutId");
                    menu.remove();
                } else {
                    //menu is not cloned and layered; simply hide and remove not layered class
                    menu.removeClass("EktronEditorsMenuNotLayered");
                }
            });
        },

        setPosition: function (menu) {

            //get marker
            var marker = $ektron("a[data-ektron-editorsmenu-id='" + menu.attr("cloneid") + "']");
            var markerOffset = marker.offset();

            //get marker dimensions
            var markerTop = markerOffset.top;
            var markerRight = markerOffset.left + marker.width();
            var markerBottom = markerOffset.top + marker.height();
            var markerLeft = markerOffset.left;

            //set menu position defaults
            var menuTop = markerTop;
            var menuLeft = markerLeft;

            //determine menu height
            var tempMenu = menu.clone().prependTo("body");
            tempMenu.css("left", "-1000px");
            tempMenu.css("display", "block");

            //get menu height & width
            var menuHeight = tempMenu.height();
            var menuWidth = tempMenu.width();

            //remove temp menu
            tempMenu.remove();

            //get height & widths for body
            var body = $ektron("body");
            var bodyHeight = body.height();
            var bodyWidth = body.width();

            //determine menu location - above or below
            if ((menuHeight > bodyHeight) || (menuHeight < (bodyHeight - markerBottom))) {
                //place the menu below the marker

                //menuHeight > bodyHeight = the height of the menu is greater than the height of the body
                //menuHeight < (bodyHeight - markerBottom) = the height of the menu is less than the distance between
                //the bottom of the menu marker, and the bottom of the body.  This means the menu can fit below the marker
                //without causing scroll bars

                menuTop = markerBottom;
            } else {
                if ((menuHeight > markerTop) && (menuHeight > (bodyHeight - markerBottom))) {
                    //place the menu below the marker

                    //the height of the menu cannot be placed neither above, nor below the marker without causing scroll bars

                    menuTop = markerBottom;
                } else {
                    //place the menu above the marker

                    //the height of the menu is less than the height of the body AND the height of the menu is less than
                    //the top of the marker - this means the menu can fit above the marker

                    menuTop = markerTop - menuHeight;
                }
            }

            //determine menu location - right or left
            if ((menuWidth > bodyWidth) || (menuWidth < (bodyWidth - markerRight))) {
                //place the menu to the right of the marker

                //menuWidth > bodyWidth = the width of the menu is greater than the width of the body
                //menuWidth > markerLeft = the width of the menu is greater than the distance between
                //the right of the menu marker, and the right of the body.  This means the menu can fit to the right of the marker

                menuLeft = markerRight; //right
            } else {
                if ((menuWidth < bodyWidth) && (menuWidth > markerLeft)) {
                    //place the menu to the right of the marker

                    //the width of the menu cannot be placed neither to the right, nor the left of the marker without causing scroll bars

                    menuLeft = markerRight;
                } else {
                    //place the menu to the left of the marker

                    //the width of the menu is less than the width of the body AND the width of the menu is less than
                    //the distance between the left of the body and the left of the marker - this means the menu can fit
                    //to the left of the marker

                    menuLeft = markerLeft;
                }
            }

            //set menu location
            menu.css("top", menuTop);
            menu.css("left", menuLeft);

            if ($ektron.browser.msie) {
                if ($ektron.browser.version < 7) {
                    menu.find("li").css("position", "relative");
                }
                menu.find("a").css("word-wrap", "normal");  //ensures menu item text does not wrapdefault.ap
            }
        },

        timeoutDuration: 1500
    }

    /**
    * jQuery.Listen
    * Copyright (c) 2007-2008 Ariel Flesler - aflesler(at)gmail(dot)com | http://flesler.blogspot.com
    * Dual licensed under MIT and GPL.
    * Date: 3/7/2008
    *
    * @projectDescription Light and fast event handling, using event delegation.
    * Homepage: http://flesler.blogspot.com/2007/10/jquerylisten.html
    * Requires jQuery 1.2.3 or higher. Tested on FF 2|IE 6/7|Safari 3|Opera 9, Windows XP.
    *
    * @author Ariel Flesler
    * @version 1.0.3
    *
    * @id jQuery.listen
    * @param {String} name Name of the event to listen (f.e: click, mouseover, etc).
    * @param {DOM Element} listener optional: The DOM element to listen from, the document element by default.
    * @param {String|Boolean} selector A simple selector in one of this formats: "#id", "tagname", ".class", or "tagname.class".
    * @param {Function} handler The event handler to register.
    *
    * Notes:
    *	-The selectors support is low in order to mantain scalability. You can use comma-separated selectors.
    *	  I consider these 4 options, the most useful and I believe they are enough for many cases.
    *	-This plugin can't handle non-bubbling events. It handles focus & blur thanks to the focusin/focusout approach.
    */
    ;  (function ($) {

        $.fn.indexer = function (name) {//allow public access to the indexers
            return this[0] && indexer(this[0], name) || null;
        };
        $.indexer = function (name) {
            return indexer(document, name);
        };

        var $event = $.event,
		    $special = $event.special,
		    $listen = $.listen = function (name, listener, selector, handler) {
		        if (typeof listener != 'object') { //document is the default listener
		            handler = selector;
		            selector = listener;
		            listener = document;
		        }
		        each(name.split(/\s+/), function (ev) {
		            ev = $listen.fixes[ev] || ev; //try to use a fixed event.
		            var idxer = indexer(listener, ev) || indexer(listener, ev, new Indexer(ev, listener));

		            idxer.append(selector, handler); // register the handler.
		            idxer.start();
		        });
		    },
		    indexer = function (elem, name, val) {
		        return $.data(elem, name + '.indexer', val);
		    };

        $.extend($listen, {
            regex: /^((?:\w*?|\*))(?:([#.])([\w-]+))?$/, //matches "#id", "tag", ".class" or "tag.class", also "tag#id" but the tag is ignored.
            fixes: { //registry of replacement for non-bubbling events, you can add more ( please fix change for IE :) )
                focus: 'focusin',
                blur: 'focusout'
            },
            cache: function (on) {
                this.caching = on;
            }
        });

        //taken and adapted from http://dev.jquery.com/browser/trunk/plugins/validate/lib/jquery.delegate.js?rev=4374
        $.each($listen.fixes, function (original, fix) {
            $special[fix] = {
                setup: function () {
                    if ($.browser.msie) return false;
                    this.addEventListener(original, $special[fix].handler, true);
                },
                teardown: function () {
                    if ($.browser.msie) return false;
                    this.removeEventListener(original, $special[fix].handler, true);
                },
                handler: function (e) {
                    arguments[0] = e = $event.fix(e);
                    e.type = fix;
                    return $event.handle.apply(this, arguments);
                }
            };
        });

        $.fn.listen = function (name, selector, handler) {//listen using the prototype
            return this.each(function () {
                $listen(name, this, selector, handler);
            });
        };

        function Indexer(name, listener) {
            $.extend(this, {
                ids: {},
                tags: {},
                listener: listener,
                event: name
            });
            this.id = Indexer.instances.push(this); //for cleaning up later
        };
        Indexer.instances = [];

        Indexer.prototype = {
            constructor: Indexer,
            handle: function (e) {
                var sp = e.stopPropagation; //intercept any call to stopPropagation
                e.stopPropagation = function () {
                    e.stopped = true;
                    sp.apply(this, arguments);
                };
                indexer(this, e.type).parse(e);
                e.stopPropagation = sp; //revert
                sp = e.data = null; //cleanup
            },
            on: false,
            bubbles: false,
            start: function () {//start listening (bind)
                if (!this.on) { //avoid duplicates
                    $event.add(this.listener, this.event, this.handle);
                    this.on = true;
                }
            },
            stop: function () {//stop listening (unbind)
                if (this.on) {
                    $event.remove(this.listener, this.event, this.handle);
                    this.on = false;
                }
            },
            cache: function (node, handlers) {
                return $.data(node, 'listenCache_' + this.id, handlers);
            },
            parse: function (e) {
                var node = e.data || e.target,
				    args = arguments, handlers;

                if (!$listen.caching || !(handlers = this.cache(node))) {//try to retrieve cached handlers
                    handlers = [];

                    if (node.id && this.ids[node.id])//if this node has an id and there are handlers registered to it..
                        push(handlers, this.ids[node.id]);

                    each([node.nodeName, '*'], function (tag) {//look for handlers registered by name.class.
                        var klasses = this.tags[tag];
                        if (klasses)
                            each((node.className + ' *').split(' '), function (klass) {
                                if (klass && klasses[klass])
                                    push(handlers, klasses[klass]); //append the handlers to the list.
                            });
                    }, this);

                    if ($listen.caching)
                        this.cache(node, handlers);
                }

                if (handlers[0]) {
                    each(handlers, function (handler) {
                        if (handler.apply(node, args) === false) {
                            e.preventDefault();
                            e.stopPropagation();
                        }
                    });
                }

                if (!e.stopped && (node = node.parentNode) && (node.nodeName == 'A' || this.bubbles && node != this.listener)) {//go up ?
                    e.data = node; //I rather not alter e.target, it might be used.
                    this.parse(e);
                }
                handlers = args = node = null; //cleanup
            },
            append: function (selector, handler) {
                each(selector.split(/\s*,\s*/), function (selector) {//support comma separated selectors
                    var match = $listen.regex.exec(selector);
                    if (!match)
                        throw '$.listen > "' + selector + '" is not a supported selector.';
                    var 
					    id = match[2] == '#' && match[3],
					    tag = match[1].toUpperCase() || '*',
					    klass = match[3] || '*';
                    if (id)//we have an id, register the handler to it.
                        (this.ids[id] || (this.ids[id] = [])).push(handler);
                    else if (tag) {//we have an name and/or class
                        tag = this.tags[tag] = this.tags[tag] || {};
                        (tag[klass] || (tag[klass] = [])).push(handler);
                    }
                }, this);
            }
        };

        function each(arr, fn, scope) {
            for (var i = 0, l = arr.length; i < l; i++)
                fn.call(scope, arr[i], i);
        };
        function push(arr, elems) {
            arr.push.apply(arr, elems);
            return arr;
        };

        $(window).unload(function () {// cleanup
            if (typeof Indexer == 'function')
                each(Indexer.instances, function (idxer) {
                    idxer.stop();
                    $.removeData(idxer.listener, idxer.event + '.indexer');
                    idxer.ids = idxer.names = idxer.listener = null;
                });
        });

    })($ektron);

    //Initialize Ektron Editors Menu Object
    Ektron.ready(function () {
        Ektron.EditorsMenu.bindEvents();
    });
}
/*********************************************************/
/**************** End Ektron Editor's Menu ***************/
/*********************************************************/
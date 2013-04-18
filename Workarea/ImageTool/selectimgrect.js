var g_strResizeEkImgTool = "";
var g_strSelectRectIDEkImgTool = "selectRect";
var g_resizerOffsetEkImgTool = 2;
var g_resizerWidthEkImgTool = 4;
var g_arySelectLocEkImgTool = new Array(0, 0, 75, 75);  // left, top, width, height (see below)
var gc_iLeftEkImgTool = 0;
var gc_iTopEkImgTool = 1;
var gc_iWidthEkImgTool = 2;
var gc_iHeightEkImgTool = 3;

var gc_strWholeMoveIDEkImgTool = "zzazzxxz"; // Have we click inside, and should use the Move method.
var g_cursorOffsetXEkImgTool = 0;  // from left: the offset from when they click in the middle of the rect.
var g_cursorOffsetYEkImgTool = 0;  // from top:  These are used with the Move method.
var g_lastCursorXEkImgTool = 0;  // the last mouse position reported
var g_lastCursorYEkImgTool = 0;  // save so that we don't keep drawing for no reason

// Image Dimensions
// Since the script can't see the dimensions directly,
// they must be set by calling SetImageDimensions().
var g_iImageWidthEkImgTool = -1;
var g_iImageHeightEkImgTool = -1;

// Modes
var g_mSelectRectModeEkImgTool = 0;
var gc_SelModeCropEkImgTool = 0;
var gc_SelModeTextEkImgTool = 1;

var gc_colorActiveResizeEkImgTool = "red";
var gc_colorInactiveResizeEkImgTool = "black";

var gc_strTextRectFieldNameEkImgTool = "ekImgToolTextRect";

// Limits
var g_selectMaxXEkImgTool = 0;  // No selection, whatsoever, can't go over these values.
var g_selectMaxYEkImgTool = 0;
var g_selectOffsetXEkImgTool = 3;  // The offset for any dimensional calculations.
var g_selectOffsetYEkImgTool = 3;  // these are currently not used, but will be soon.

var g_currentImageIDEkImgTool = "";  // the ID of the image that we are currently drawn on
var g_currentSelectorIDEkImgTool = "";

// Naming modifiers
var gc_strSelRectSuffixEkImgTool = "_SelRect";
var gc_strRszFuncSuffixEkImgTool = "_EkImgToolResizeMoveTo";  // Appended to ID to make function

// Resize Handles
var g_aryRszHandleIDEkImgTool =    new Array("upperleftrsz", "lowerrightrsz", "upperrightrsz", "lowerleftrsz", "midupperrsz", "midrightrsz", "midbottomrsz", "midleftrsz");
var g_aryRszHandleCurEkImgTool =   new Array("se-resize",    "se-resize",     "ne-resize",     "ne-resize",    "n-resize",    "e-resize",    "n-resize",     "e-resize"  );
var g_aryRszHandleRPosXEkImgTool = new Array(0,              1,               1,               0,              0.5,           1,             0.5,            0);
var g_aryRszHandleRPosYEkImgTool = new Array(0,              1,               0,               1,              0,             0.5,           1,              0.5);


var g_strRszTextMsgIDEkImgTool = "textmsgrsz";
var g_avgCharWidthEkImgTool = 12;

var g_strSelectRectStyleInfoEkImgTool = 
            "position:absolute; " +  
            "left:20px; top:20px; " +  
            "width:75px; height:75px; " +  
            "visibility:hidden; " + 
            "MARGIN-TOP:3px; MARGIN-BOTTOM:3px; MARGIN-LEFT:3px; MARGIN-RIGHT:3px; " + 
            "border-top:black solid 1px; " +  
            "border-bottom:black solid 1px; " + 
            "border-left:black solid 1px; " +  
            "border-right:black solid 1px; " + 
            "background-color:transparent;" +
            "";
var g_strTextRectStyleInfoEkImgTool = 
            "position:absolute; " +  
            "left:20px; top:20px; " +  
            "width:1px; height:1px; " +  
            "visibility:hidden; " + 
            "MARGIN-TOP:0px; MARGIN-BOTTOM:0px; MARGIN-LEFT:0px; MARGIN-RIGHT:0px; " + 
            "border-top:black hidden 0px; " +  
            "border-bottom:black hidden 0px; " + 
            "border-left:black hidden 0px; " +  
            "border-right:black hidden 0px; " + 
            "background-color:transparent;" +
            "";
var g_strHandleStyleInfoEkImgTool = 
            "position:absolute; " +  
            "left:-3px; top:-3px; " +  
            //"width:7px; height:7px; " +  // This is causing to not load in IE7; set in code.
            "visibility:hidden; " + 
            "border-top:silver solid 1px; " +  
            "border-bottom:silver solid 1px; " + 
            "border-left:silver solid 1px; " +  
            "border-right:silver solid 1px; " + 
            "font-size:6pt; " + 
            "background-color:black;" +
            "";
var g_strImageWrapperSelStyleInfoEkImgTool =
            "visibility:visible; " +
            "background-color:transparent; " +
            "";
var g_strTextStyleInfoEkImgTool = 
            "position:absolute; " +  
            "left:0px; top:0px; " +  
            "width:100%; height:100%; " + 
            "visibility:hidden; " + 
            "border-top:silver solid 1px; " +  
            "border-bottom:silver solid 1px; " + 
            "border-left:silver solid 1px; " +  
            "border-right:silver solid 1px; " + 
            "font-size:8pt; " + 
            "";
var g_strFloatingTextStyleInfoEkImgTool = 
            "font-family:Verdana; " + 
            "font-size:12pt; " + 
            "font-weight:normal; " + 
            "text-shadow:white 4px 4px 2px; " + 
            "POSITION: absolute; " + 
            "border-top:black hidden 0px; " +  
            "border-bottom:black hidden 0px; " + 
            "border-left:black hidden 0px; " +  
            "border-right:black hidden 0px; " + 
            "PADDING-RIGHT: 0px; " + 
            "PADDING-LEFT: 0px; " + 
            "PADDING-BOTTOM: 0px; " + 
            "PADDING-TOP: 0px; " + 
            "Z-INDEX: 1; " + 
            "FLOAT: none; " + 
            "MARGIN: 0px; " + 
            "POSITION: absolute; " + 
            "TOP: 10%; " + 
            "background-color:transparent; " + 
            "visible:true; " +
            "width:auto; " +
            "";


////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// Public Setup Methods
////////////////////////////////////////////////////////////////////////////////

function SetImageDimensionsEkImgTool(width, height)
{
    g_iImageWidthEkImgTool = width;
    g_iImageHeightEkImgTool = height;
    
    //alert("Setting Dimensions (" + width + ", " + height + ")");
}

function SetResizeRectOnDocument()
{
    var idx = 0;
    g_mSelectRectModeEkImgTool = gc_SelModeCropEkImgTool;

    document.write("<div style=\"" + g_strSelectRectStyleInfoEkImgTool + " cursor:move; " + "\" id=\"" + g_strSelectRectIDEkImgTool + 
            "\" onmouseup=\"EkImgToolEndResize()\" >");
//            "\" onmouseup=\"EkImgToolEndResize()\" ondblclick=\"EkImgToolExecSelection()\">");
    // Inset 
    document.write("<div style=\"" + g_strTextStyleInfoEkImgTool + " cursor:move; " + "\" id=\"" + g_strRszTextMsgIDEkImgTool + 
            "\" onmouseup=\"EkImgToolEndResize()\" ></div>");
//            "\" onmouseup=\"EkImgToolEndResize()\" ondblclick=\"EkImgToolExecSelection()\"></div>");
    
    // These are the handles
    for(idx = 0; idx < g_aryRszHandleIDEkImgTool.length; idx++)
    {
        EkImgToolMakeSelectionResizeHandle(g_aryRszHandleIDEkImgTool[idx], g_aryRszHandleCurEkImgTool[idx]);
    }
        
    document.write("</div>");
}

function disableEnterKey(e)
{
     var key;     
     if(window.event)
          key = window.event.keyCode; //IE
     else
          key = e.which; //firefox     

     return (key != 13);
}

function SetTextRectOnDocument()
{
    var idx = 0;
    g_mSelectRectModeEkImgTool = gc_SelModeTextEkImgTool;

    document.write("<div style=\"" + g_strTextRectStyleInfoEkImgTool + " cursor:move; " + "\" id=\"" + g_strSelectRectIDEkImgTool + 
            "\" onmouseup=\"EkImgToolEndResize()\" >");
//            "\" onmouseup=\"EkImgToolEndResize()\" ondblclick=\"EkImgToolExecSelection()\">");

    document.write("<input type=\"Text\" maxlength=\"300\" value=\" \" style=\"" + g_strRszTextMsgIDEkImgTool +  
            "\" id=\"" + g_strRszTextMsgIDEkImgTool + 
            "\" name=\"" + g_strRszTextMsgIDEkImgTool + 
            "\" onmouseup=\"EkImgToolEndResize()\" onkeypress=\"return EkImgToolIncreaseTextAreaSize(event)\" ></div>");
//            "\" onmouseup=\"EkImgToolEndResize()\" onkeypress=\"return EkImgToolIncreaseTextAreaSize(event)\" ondblclick=\"EkImgToolExecSelection()\"></div>");
    
    // These are no handles
        
    document.write("</div>");

    g_arySelectLocEkImgTool[gc_iWidthEkImgTool] = 10;
    g_arySelectLocEkImgTool[gc_iHeightEkImgTool] = 10;
}

function TextRectFieldName()
{
    return g_strRszTextMsgIDEkImgTool;
}

// The strWrappingTagID is the ID of the tag (div/span) 
// that is wrapping the image.  This will be used as the
// actual listener for mouse movements.
function ShowAreaSelectRect(strFromID, strWrappingTagID)
{
    g_currentImageIDEkImgTool = strFromID;
    g_currentSelectorIDEkImgTool = strFromID;  
    if("undefined" != typeof strWrappingTagID)
    {
        if(strWrappingTagID.length > 0)
        {
            g_currentSelectorIDEkImgTool = strWrappingTagID;
        }
    }
    
    EkImgToolSetSelectionListeners(g_currentSelectorIDEkImgTool, true);
    
    // Fix the height of the surrounding DIV or SPAN tag.
    EkImgToolMatchFrameToImage(g_currentImageIDEkImgTool, g_currentSelectorIDEkImgTool);

    g_strResizeEkImgTool = "";
    
    EkImgToolPositionAreaSelectRect(20, 20, 75, 75);
    EkImgToolShowSelectRectangle(true);
}


////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// Public General Use Methods
////////////////////////////////////////////////////////////////////////////////
            
// Sets the text shown in the rectangle.
// This can be HTML.
function SetSelectionRectText(strMsg, fontname, fontsizept, bBold, bItal)
{
    var elem = document.getElementById(g_strRszTextMsgIDEkImgTool);
    if(elem)
    {
        g_avgCharWidthEkImgTool = fontsizept;
            
        if(g_mSelectRectModeEkImgTool == gc_SelModeCropEkImgTool)
        {
            var strLook = "";
            
            strLook += "font-family:" + fontname + "; ";
            strLook += "font-size:" + fontsizept + "pt; ";
            if(bBold == true)
            {
                strLook += "font-weight:bold; font-style:normal; ";
            }
            else if(bItal == true)
            {
                strLook += "font-weight:normal; font-style:italic; ";
            }
            else
            {
                strLook += "font-weight:normal; font-style:normal; ";
            }
            
            elem.innerHTML = "<span style=\"" + strLook + "\">" + strMsg + "</span>";
        }
        else if(g_mSelectRectModeEkImgTool == gc_SelModeTextEkImgTool)
        {
            if(fontname.length > 0)
                elem.style.fontFamily = fontname;
            if(fontsizept.length > 0)
                elem.style.fontSize = fontsizept + "pt";
            if(bBold)
                elem.style.fontWeight = "bold";
            if(bItal)
            {
                elem.style.fontWeight = "normal";
                elem.style.fontStyle = "italic";
            }
            elem.style.backgroundColor = "transparent";
            //elem.style.textShadow = "white 2px 2px 2px";
                
            elem.value = strMsg;
            EkImgToolIncreaseTextAreaSize(null);
        }
        
        elem.focus();
    }
}

function EkImgToolIncreaseTextAreaSize(e)
{
    var result = true;
    if (e != null) {
        var result = disableEnterKey(e);
    }
    var elem = document.getElementById(g_strRszTextMsgIDEkImgTool);
    var iLen = elem.value.length; //if(0 == iLen) iLen = 1;
    var iWidth = (iLen * g_avgCharWidthEkImgTool) + 14;
    if (result != false) {
        elem.style.width = iWidth + "px";
    }
    return (result);
}

function EkImgToolPositionAreaSelectRect(left, top, width, height)
{
    if((g_iImageWidthEkImgTool > 0) && (g_iImageHeightEkImgTool > 0))
    {
        if( ((left + width) > g_iImageWidthEkImgTool) || ((top + height) > g_iImageHeightEkImgTool))
        {
            left = (g_iImageWidthEkImgTool / 20) + 1;
            width = left * 4;
            top = (g_iImageHeightEkImgTool / 20) + 1;
            height = top * 4;
            
            //alert("(" + left + ", " + top + ")  [" + width + ", " + height + "]");
            
            EkImgToolMoveSelectRect(left, top);
        }
        else
        {
            EkImgToolMoveSelectRect(left - g_selectOffsetXEkImgTool, top - g_selectOffsetYEkImgTool);
        }
    }
    else
    {
        EkImgToolMoveSelectRect(left - g_selectOffsetXEkImgTool, top - g_selectOffsetYEkImgTool);
    }

    EkImgToolResizeSelectRect(width, height);
    
    if(g_mSelectRectModeEkImgTool != gc_SelModeTextEkImgTool) EkImgToolDrawResizers();
}

function GetSelectionDimension(dimension)
{
    switch(dimension.toLowerCase())
    {
        case "0":
        case "left":
        case "x":
            return g_arySelectLocEkImgTool[gc_iLeftEkImgTool] + g_selectOffsetXEkImgTool;
        break;
        
        case "1":
        case "top":
        case "y":
            return g_arySelectLocEkImgTool[gc_iTopEkImgTool] + g_selectOffsetYEkImgTool;
        break;
        
        case "2":
        case "width":
        case "w":
            return g_arySelectLocEkImgTool[gc_iWidthEkImgTool];
        break;
        
        case "3":
        case "height":
        case "h":
            return g_arySelectLocEkImgTool[gc_iHeightEkImgTool];
        break;        
    }
}


////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// Private Setup Methods
////////////////////////////////////////////////////////////////////////////////

function EkImgToolSetSelectionListeners(strID, bAssign)
{
    var elem = document.getElementById(strID);
    if(elem)
    {
        if(bAssign)
        {
            if(window.addEventListener)
            { // Mozilla, Netscape, Firefox
                elem.addEventListener('mouseup', EkImgToolEndResize, false); 
                elem.addEventListener('mousemove', EkImgToolResizeMove, false); 
//                elem.addEventListener('dblclick', EkImgToolExecSelection, false);
            }
            else if(window.attachEvent)
            { // IE
                elem.attachEvent('onmouseup', EkImgToolEndResize); 
                elem.attachEvent('onmousemove', EkImgToolResizeMove); 
//                elem.attachEvent('ondblclick', EkImgToolExecSelection);
            }
            else
            { // classic
                elem.onmousemove = EkImgToolResizeMove; 
                elem.onmouseup = EkImgToolEndResize; 
//                elem.ondblclick = EkImgToolExecSelection;
            }
        }
        else
        {
            if(window.removeEventListener)
            { // Mozilla, Netscape, Firefox
                elem.removeEventListener('mouseup', EkImgToolEndResize, false); 
                elem.removeEventListener('mousemove', EkImgToolResizeMove, false); 
//                elem.removeEventListener('dblclick', EkImgToolExecSelection, false);
            }
            else if(window.detachEvent)
            { // IE
                elem.detachEvent('onmouseup', EkImgToolEndResize); 
                elem.detachEvent('onmousemove', EkImgToolResizeMove); 
//                elem.detachEvent('ondblclick', EkImgToolExecSelection);
            }
            else
            { // Classic
                elem.onmousemove = null; 
                elem.onmouseup = null; 
//                elem.ondblclick = null;
            }
        }
    }

    EkImgToolSetSelectionRectangleEvents(true);

}

function EkImgToolSetSelectionRectangleEvents(bAssign)
{
    var elem;
    var aryElements = new Array(g_strSelectRectIDEkImgTool, g_strRszTextMsgIDEkImgTool);
    
    for(var idx = 0; idx < aryElements.length; idx++)
    {
        elem = document.getElementById(aryElements[idx]);
        if(elem)
        {
            if(bAssign)
            {
                if(window.addEventListener)
                { // Mozilla, Netscape, Firefox
                    elem.addEventListener('mousedown', EkImgToolStartSelectRectMove, false);
                    elem.addEventListener('mousemove', EkImgToolResizeMove, false);
                    elem.addEventListener('mouseup', EkImgToolEndResize, false); 
//                    elem.addEventListener('dblclick', EkImgToolExecSelection, false);
                }
                else if(window.attachEvent)
                { // IE
                    elem.attachEvent('onmousedown', EkImgToolStartSelectRectMove);
                    elem.attachEvent('onmousemove', EkImgToolResizeMove);
                    elem.attachEvent('onmouseup', EkImgToolEndResize); 
//                    elem.attachEvent('ondblclick', EkImgToolExecSelection);
                }
                else
                { // Classic
                    elem.onmousedown = EkImgToolStartSelectRectMove;
                    elem.onmousemove = EkImgToolResizeMove;
                    elem.onmouseup = EkImgToolEndResize;
//                    elem.ondblclick = EkImgToolExecSelection;
                }
            }
            else  // ! bAssign
            {
                if(window.removeEventListener)
                { // Mozilla, Netscape, Firefox
                    elem.removeEventListener('mousedown', EkImgToolStartSelectRectMove, false);
                    elem.removeEventListener('mousemove', EkImgToolResizeMove, false);
                    elem.removeEventListener('mouseup', EkImgToolEndResize, false); 
//                    elem.removeEventListener('dblclick', EkImgToolExecSelection, false);
                }
                else if(window.detachEvent)
                { // IE
                    elem.detachEvent('onmousedown', EkImgToolStartSelectRectMove);
                    elem.detachEvent('onmousemove', EkImgToolResizeMove);
                    elem.detachEvent('onmouseup', EkImgToolEndResize); 
//                    elem.detachEvent('ondblclick', EkImgToolExecSelection);
                }
                else
                { // Classic
                    elem.onmousedown = null;
                    elem.onmousemove = null;
                    elem.onmouseup = null;
//                    elem.ondblclick = null;
                }
            }
        }
    }

}


////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// Private Moving Methods
// These methods DO NOT take into account the offsets of the source of the
// request.  They act on the coordinates and measures as given.
////////////////////////////////////////////////////////////////////////////////

// This moves the rectangle origin, the size is not modified.
//  --
// |  |  (the sides)
//  --
function EkImgToolMoveSelectRect(left, top)
{
    var selectElem = document.getElementById(g_strSelectRectIDEkImgTool);
    if(top > 0)
    {
        selectElem.style.top =  top + "px";
        g_arySelectLocEkImgTool[gc_iTopEkImgTool] = top;
    }
    if(left > 0)
    {
        selectElem.style.left = left + "px";
        g_arySelectLocEkImgTool[gc_iLeftEkImgTool] = left;
    }
    selectElem.style.visibility = "visible";
}

// This moves the upper left, thus making the rectangle smaller/larger.
// +--
// |
function EkImgToolOriginSelectRect(left, top)
{
    var selectElem = document.getElementById(g_strSelectRectIDEkImgTool);
    var origTop = g_arySelectLocEkImgTool[gc_iTopEkImgTool];
    var origLeft = g_arySelectLocEkImgTool[gc_iLeftEkImgTool];
    
    if(top < (g_arySelectLocEkImgTool[gc_iTopEkImgTool] + g_arySelectLocEkImgTool[gc_iHeightEkImgTool]))
    {
        selectElem.style.top =  top + "px";
        g_arySelectLocEkImgTool[gc_iTopEkImgTool] = top;
    }
    if(left < (g_arySelectLocEkImgTool[gc_iLeftEkImgTool] + g_arySelectLocEkImgTool[gc_iWidthEkImgTool]))
    {
        selectElem.style.left = left + "px";
        g_arySelectLocEkImgTool[gc_iLeftEkImgTool] = left;
    }
    
    EkImgToolResizeSelectRect(g_arySelectLocEkImgTool[gc_iWidthEkImgTool] - (left - origLeft), g_arySelectLocEkImgTool[gc_iHeightEkImgTool] - (top - origTop));
}

// this modifies the size of the rectangle without moving the origin.
//   |
// --+   
function EkImgToolResizeSelectRect(width, height)
{
    var selectElem = document.getElementById(g_strSelectRectIDEkImgTool);
    if(width > 0)
    {
        selectElem.style.width =  width + "px";
        g_arySelectLocEkImgTool[gc_iWidthEkImgTool] = width;
    }
    if(height > 0)
    {
        selectElem.style.height = height + "px";
        g_arySelectLocEkImgTool[gc_iHeightEkImgTool] = height;
    }
}

// This moves the Y origin, the height, and the width.
// --+
//   |
function EkImgToolUpperRightSelectRect(width, top)
{
    var selectElem = document.getElementById(g_strSelectRectIDEkImgTool);
    var origTop = g_arySelectLocEkImgTool[gc_iTopEkImgTool];

    // Adjust the Y (top) origin
    if(top < (g_arySelectLocEkImgTool[gc_iTopEkImgTool] + g_arySelectLocEkImgTool[gc_iHeightEkImgTool]) )
    {
        selectElem.style.top =  top + "px";
        g_arySelectLocEkImgTool[gc_iTopEkImgTool] = top;
    }
    
    // Adjust the width (left stays put)
    if(width > 0)
    {
        selectElem.style.width = width + "px";
        g_arySelectLocEkImgTool[gc_iWidthEkImgTool] = width;
    }
    
    EkImgToolResizeSelectRect(g_arySelectLocEkImgTool[gc_iWidthEkImgTool], g_arySelectLocEkImgTool[gc_iHeightEkImgTool] - (top - origTop) );
}

// Modifies the height, X location, and width.
// |
// +--
function EkImgToolLowerLeftSelectRect(left, height)
{
    var selectElem = document.getElementById(g_strSelectRectIDEkImgTool);
    var origLeft = g_arySelectLocEkImgTool[gc_iLeftEkImgTool];

    // Adjust the X (left) origin
    if(left < (g_arySelectLocEkImgTool[gc_iLeftEkImgTool] + g_arySelectLocEkImgTool[gc_iWidthEkImgTool]))
    {
        selectElem.style.left = left + "px";
        g_arySelectLocEkImgTool[gc_iLeftEkImgTool] = left;
    }
    
    // Adjust the height (top stays put)
    if(height > 0)
    {
        selectElem.style.height = height + "px";
        g_arySelectLocEkImgTool[gc_iHeightEkImgTool] = height;
    }
 
    EkImgToolResizeSelectRect(g_arySelectLocEkImgTool[gc_iWidthEkImgTool] - (left - origLeft), g_arySelectLocEkImgTool[gc_iHeightEkImgTool]);    
}

function EkImgToolMatchFrameToImage(strImage, strFrame)
{
    if(strImage != strFrame)
    {
        var elemImageArea = document.getElementById(strImage);
        var w = elemImageArea.style.width;
        var h = elemImageArea.style.height;
        elemImageArea = document.getElementById(strFrame);
        elemImageArea.style.width = w;
        elemImageArea.style.height = h;
        g_selectMaxXEkImgTool = w + elemImageArea.style.left;
        g_selectMaxYEkImgTool = h + elemImageArea.style.top;
    }
}


////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// Private Mouse Action Methods
// These movement methods take care of the adjustements needed due to 
// the source of the request, such as the mouse position offset.
////////////////////////////////////////////////////////////////////////////////

function upperleftrsz_EkImgToolResizeMoveTo(X, Y)
{
    // +--
    // |
    EkImgToolOriginSelectRect(X - g_resizerOffsetEkImgTool - (g_selectOffsetXEkImgTool), Y - g_resizerOffsetEkImgTool - (g_selectOffsetYEkImgTool));
    EkImgToolDrawResizers();
}
function upperrightrsz_EkImgToolResizeMoveTo(X, Y)
{
    // --+
    //   |
    // We need to double the offsets, since two affect our location.
    EkImgToolUpperRightSelectRect(X - g_arySelectLocEkImgTool[gc_iLeftEkImgTool] - (g_selectOffsetXEkImgTool * 2), Y - (g_selectOffsetYEkImgTool * 2));
    EkImgToolDrawResizers();
}
function lowerrightrsz_EkImgToolResizeMoveTo(X, Y)
{
    //   |
    // --+   
    EkImgToolResizeSelectRect(X - g_arySelectLocEkImgTool[gc_iLeftEkImgTool] - (g_selectOffsetXEkImgTool), Y - g_arySelectLocEkImgTool[gc_iTopEkImgTool] - (g_selectOffsetYEkImgTool));
    EkImgToolDrawResizers();
}
function lowerleftrsz_EkImgToolResizeMoveTo(X, Y)
{
    // |
    // +--
    // We need to double the offsets, since two affect our location.
    EkImgToolLowerLeftSelectRect(X - (g_selectOffsetXEkImgTool * 2), Y - g_arySelectLocEkImgTool[gc_iTopEkImgTool] - (g_selectOffsetYEkImgTool * 2));
    EkImgToolDrawResizers();
}
function midupperrsz_EkImgToolResizeMoveTo(X, Y)
{
    //  --+--
    // |     |
    EkImgToolOriginSelectRect(g_arySelectLocEkImgTool[gc_iLeftEkImgTool], Y - g_resizerOffsetEkImgTool - (g_selectOffsetYEkImgTool));
    EkImgToolDrawResizers();
}
function midrightrsz_EkImgToolResizeMoveTo(X, Y)
{
    // --
    //   |
    //   +
    //   |
    // --
    EkImgToolResizeSelectRect(X - g_arySelectLocEkImgTool[gc_iLeftEkImgTool] - (g_selectOffsetXEkImgTool), g_arySelectLocEkImgTool[gc_iHeightEkImgTool]);
    EkImgToolDrawResizers();
}
function midbottomrsz_EkImgToolResizeMoveTo(X, Y)
{
    // |     |
    //  --+--
    EkImgToolResizeSelectRect(g_arySelectLocEkImgTool[gc_iWidthEkImgTool], Y - g_arySelectLocEkImgTool[gc_iTopEkImgTool] - (g_selectOffsetYEkImgTool));
    EkImgToolDrawResizers();
}
function midleftrsz_EkImgToolResizeMoveTo(X, Y)
{
    //  --
    // |
    // +
    // |
    //  --
    EkImgToolOriginSelectRect(X - g_resizerOffsetEkImgTool - (g_selectOffsetXEkImgTool), g_arySelectLocEkImgTool[gc_iTopEkImgTool]);
    EkImgToolDrawResizers();
}


////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// Private Display Methods
////////////////////////////////////////////////////////////////////////////////

function EkImgToolMakeSelectionResizeHandle(strID, strCursorType)
{
    document.write("<div style=\"" + 
        g_strHandleStyleInfoEkImgTool + " cursor:" + strCursorType + "; " + 
        "\" id=\"" + strID + "\"  onmousedown=\"EkImgToolStartResize('" + strID + "')\"  onmouseup=\"EkImgToolEndResize()\"></div>");
}

function EkImgToolShowSelectRectangle(bShow)
{
    var visVal = "visible";
    if("" != g_strResizeEkImgTool)
        EkImgToolShowResizeHandles(false);
    else
        EkImgToolShowResizeHandles(bShow);
      
    if(false == bShow)
        visVal = "hidden";
    else
        visVal = "visible";
    var elem = document.getElementById(g_strSelectRectIDEkImgTool);
    elem.style.visibility = visVal;
}

function EkImgToolShowResizeHandles(bShow)
{
    var visVal = "visible";
    var elem;
    var idx;
    if(false == bShow)
    {
        visVal = "hidden";
    }
    
    for(idx = 0; idx < g_aryRszHandleIDEkImgTool.length; idx++)
    {
        elem = document.getElementById(g_aryRszHandleIDEkImgTool[idx]); if(elem) elem.style.visibility = visVal;
    }
    
    // The text message area must be shown after everything else.
    elem = document.getElementById(g_strRszTextMsgIDEkImgTool);
    if(elem) elem.style.visibility = visVal;   
  
}

function EkImgToolDrawResizers()
{
    if(g_mSelectRectModeEkImgTool != gc_SelModeTextEkImgTool) 
    {
        // Only take the time to draw if we are resizing.
        var elemSelect = document.getElementById(g_strSelectRectIDEkImgTool);
        
        var iWidth = g_arySelectLocEkImgTool[gc_iWidthEkImgTool];   
        var iHeight = g_arySelectLocEkImgTool[gc_iHeightEkImgTool];  
        var iNewLeft = iWidth - g_resizerWidthEkImgTool + 1;
        var iNewTop = iHeight - g_resizerWidthEkImgTool + 1;
        
        var resizerWidth = 7; 
        var resizerHeight = 7; 
        var resizerOffset = g_resizerOffsetEkImgTool;
        
        var elem;
        var idx;
        
        for(idx = 0; idx < g_aryRszHandleIDEkImgTool.length; idx++)
        {
            elem = document.getElementById(g_aryRszHandleIDEkImgTool[idx]); if(elem) { elem.style.width = g_resizerWidthEkImgTool + "px"; elem.style.height = g_resizerWidthEkImgTool + "px"; if(g_aryRszHandleRPosXEkImgTool[idx] != 0) elem.style.left = (iNewLeft * g_aryRszHandleRPosXEkImgTool[idx]) + "px"; if(g_aryRszHandleRPosYEkImgTool[idx] != 0)  elem.style.top = (iNewTop * g_aryRszHandleRPosYEkImgTool[idx]) + "px";}
        } 
    }
}

function EkImgToolImageDebugMessage(strFunc, strMsg)
{
    // The client defines a onSelectionRectangleMove function.
    // If defined it is called.
  	if ("function" == typeof onImageEditActivity)
    {
        onImageEditActivity(strFunc, strMsg);
    }
}

function EkImgToolShowInformationOnElement(strElemID)
{
    var strData = "";
    
    if("" != strElemID)
    {
        var elem = document.getElementById(strElemID);
        if(elem)
        {
            strData = "Info for element " + strElemID + "\n" + "\n";
            strData += "Dimensions:  (" + elem.style.left + ", " + elem.style.top + ")  [" + elem.style.width + ", " + elem.style.height + "]" + "\n";
            
            alert(strData);
        }
    }
    else
    {
        strData = "Info for the document object:\n" + "\n";
        strData += "Margins:  " + document.body.leftMargin + ", " + document.body.topMargin + ", " + document.body.rightMargin + ", " + document.body.bottomMargin + "\n";
            
        alert(strData);
    }
}


////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// Event Listener Methods
// These methods are what are registered to receive browser events.
////////////////////////////////////////////////////////////////////////////////

function EkImgToolStartSelectRectMove(e)
{
    var ev = (e)?e:((event)?event:(window.event)?window.event:null); // the event comes from many sources
    if(ev)
    {
        g_cursorOffsetXEkImgTool = ev.clientX - g_arySelectLocEkImgTool[gc_iLeftEkImgTool];
        g_cursorOffsetYEkImgTool = ev.clientY - g_arySelectLocEkImgTool[gc_iTopEkImgTool];
    }
    else
    {
        // Have the cursor do the origin
        g_cursorOffsetXEkImgTool = 1;
        g_cursorOffsetYEkImgTool = 1;
    }
    
    // set cursor to move cursor
    document.getElementById(g_strRszTextMsgIDEkImgTool).style.cursor="move";
    
    EkImgToolStartResize(gc_strWholeMoveIDEkImgTool);
}

function EkImgToolStartResize(strResizeId)
{
    if(("" == g_strResizeEkImgTool) && ("" != strResizeId))
    {
        g_strResizeEkImgTool = strResizeId;
        if(strResizeId != gc_strWholeMoveIDEkImgTool)
        {
            var selectElem = document.getElementById(g_strResizeEkImgTool);
            selectElem.style.backgroundColor = gc_colorActiveResizeEkImgTool;
        }
    }
}

function EkImgToolEndResize()
{
    if("" != g_strResizeEkImgTool)
    {
        if(g_strResizeEkImgTool != gc_strWholeMoveIDEkImgTool)
        {
            var selectElem = document.getElementById(g_strResizeEkImgTool);
            if(selectElem) selectElem.style.backgroundColor = gc_colorInactiveResizeEkImgTool;
        }
        g_strResizeEkImgTool = "";
        
        document.getElementById(g_strRszTextMsgIDEkImgTool).style.cursor="auto";
    }   
}

function EkImgToolExecSelection()
{
    //alert("About to submit");
    //document.forms[0].submit();
    
    // The page will need to control how the submission happens
    if ("function" == typeof onSelectionDoubleClick)
    {
        onSelectionDoubleClick(g_arySelectLocEkImgTool[gc_iLeftEkImgTool], g_arySelectLocEkImgTool[gc_iTopEkImgTool],
                g_arySelectLocEkImgTool[gc_iWidthEkImgTool], g_arySelectLocEkImgTool[gc_iHeightEkImgTool]);
    }    
}

function EkImgToolResizeMove(e)
{
    if("" != g_strResizeEkImgTool)
    {
        var ev = (e)?e:((event)?event:(window.event)?window.event:null); // the event comes from many sources
        if(ev)
        {
            // the mouse move can't be cancelled
            //if(ev.cancelable) ev.preventDefault();
            //if(ev.bubbles) ev.cancelBubble = true; // the mouse move can't be cancelled
            //ev.stopPropagation();   // Netscape???
            
            if(g_strResizeEkImgTool != gc_strWholeMoveIDEkImgTool)
            {
                // Only attempt if we have actually created the method.
                // This helps in developing the methods.
                if(eval("('function' == typeof " + g_strResizeEkImgTool + gc_strRszFuncSuffixEkImgTool + ")?true:false;"))
                {
                    eval(g_strResizeEkImgTool + gc_strRszFuncSuffixEkImgTool + "(" + ev.clientX + "," + ev.clientY + ");");
                }
            }
            else
            {
            	var newposX = ev.clientX - g_cursorOffsetXEkImgTool;
            	var newposY = ev.clientY - g_cursorOffsetYEkImgTool;
            	
            	g_selectMaxXEkImgTool = document.getElementById('ImageDisplay').width;
            	g_selectMaxYEkImgTool = document.getElementById('ImageDisplay').height;
            	
		if ((g_selectMaxXEkImgTool > 0) && (newposX > g_selectMaxXEkImgTool)) {
		  newposX = g_selectMaxXEkImgTool;
		}
		if ((g_selectMaxYEkImgTool > 0) && (newposY > g_selectMaxYEkImgTool)) {
		  newposY = g_selectMaxYEkImgTool;
		}
                EkImgToolMoveSelectRect(newposX, newposY);
            }
            
            // Someone else may want to know about this
  	        if ("function" == typeof onSelectionRectangleMove)
            {
                onSelectionRectangleMove(g_arySelectLocEkImgTool[gc_iLeftEkImgTool] + g_selectOffsetXEkImgTool, g_arySelectLocEkImgTool[gc_iTopEkImgTool] + g_selectOffsetYEkImgTool,
                        g_arySelectLocEkImgTool[gc_iWidthEkImgTool], g_arySelectLocEkImgTool[gc_iHeightEkImgTool]);
            }
        }
    }
}


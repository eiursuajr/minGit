<%@ Page Language="C#" AutoEventWireup="true" CodeFile="suggestedresults.aspx.cs"
    Inherits="Workarea_search_suggestedresults" ValidateRequest="false" %>

<%@ Reference Control="../controls/search/viewsuggestedresults.ascx" %>
<%@ Reference Control="../controls/search/addsuggestedresult.ascx" %>
<%@ Reference Control="../controls/search/viewsuggestedresult.ascx" %>
<%@ Reference Control="../controls/search/deletesuggestedresults.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Suggested Results</title>
    <asp:Literal ID="jsStyleSheet" runat="server" />
    <style type="text/css">
        .positionRelative
        {
            position: relative;
            margin: 0px;
            padding: 0px;
        }
        .hideElement
        {
            position: absolute;
            left: -4000px;
        }
        .showElement
        {
            position: absolute;
            left: 0px;
            z-index: 1000;
        }
        .showFrameContainer
        {
            position: absolute;
            top: 5%;
            left: 2%;
            z-index: 1500;
            width: 98%;
            height: 95%;
            background: #fff;
        }
        div#suggestedResultsItems
        {
            width: 98.5%;
            height: 25em;
            overflow: auto;
            margin-right: 1em;
            border: solid 1px #7f9db9;
        }
        div#suggestedResultsItems ul.selectedSuggestedResults
        {
            margin: 0em;
            padding: 0em;
            list-style-type: none;
            height: 100%;
        }
        div#suggestedResultsItems ul li.suggestedResult
        {
            display: block;
        }
        div#suggestedResultsItems ul li.suggestedResult a, div#suggestedResultsItems ul li.suggestedResult div.anchor
        {
            display: block;
            padding: .5em;
        }
        div#suggestedResultsItems ul li.fillToTop
        {
            display: block;
            height: 99.5%;
            background: orange;
        }
        div#suggestedResultsItems ul li.fillToTop a
        {
            display: block;
            height: 100%;
            background: #fff;
        }
        div#suggestedResultsItems ul li.fillToTop a:hover, div#suggestedResultsItems ul li.suggestedResult a.menuAction:hover
        {
            background: #D0E5F5;
            text-decoration: none;
        }
        div#suggestedResultsItems ul li span.suggestedResultLink
        {
            display: block;
            font-size: 1.25em;
            font-weight: bold;
            color: #235478;
            text-decoration: none;
        }
        div#suggestedResultsItems ul li span.suggestedResultLink:hover
        {
            text-decoration: underline;
            cursor: pointer;
        }
        div#suggestedResultsItems ul li span.suggestedResultID
        {
            display: none;
        }
        div#suggestedResultsItems ul li span.suggestedResultContentID
        {
            display: none;
        }
        div#suggestedResultsItems ul li div.suggestedResultSummary
        {
            margin: 0em;
            padding: 0em;
        }
        div#suggestedResultsItems ul li div.suggestedResultSummary p
        {
            margin: .25em 0em .25em 0em;
        }
        /* InContext Menu */div.inContextMenu
        {
            position: absolute;
            left: -1000px;
            top: -1000px;
        }
        /*  Hidden DIVs */div#add_edit_SuggestedResult
        {
            position: absolute;
            top: 0px;
            height: 100%;
            width: 100%;
            background: #fff;
        }
        input.suggestedResultLink
        {
            width: 20em;
            font-size: 1em;
        }
        input.suggestedResultTitle
        {
            width: 31em;
        }
        input#buttonBrowseContent
        {
            width: 13em;
        }
        .caption
        {
            display: block;
            font-size: .9em;
            color: #222;
        }
        div#hiddenFormFields
        {
            position: absolute;
            left: -4000px;
            top: -4000px;
        }
        div#htmlEditorWrapper
        {
            position: relative;
            left: 1px;
        }
        div#optionsText
        {
            float: right;
            text-align: right;
            padding: 0em 1.25em;
            font-size: .92em;
        }
        div#resultSizeWarning
        {
            background: url(  '../images/ui/icons/error.png' ) no-repeat .5em .5em;
            background-color: #FBE3E4;
            border: 1px solid #FBC2C4;
            color: #D12F19;
            margin: .5em 0;
        }
        div#resultSizeWarning p
        {
            margin: 0em;
            padding: .5em .5em .5em 2.25em;
        }
        /* Modal Overrides and Content */div.ektronCMSContent
        {
            width: 50em;
            margin-left: -25em;
        }
        div.ektronCMSContent div.ektronModalBody
        {
            padding: 0;
        }
        #ChildPage
        {
            height: 30em;
        }
        .fixHeight
        {
            line-height: 1.8em;
        }
        .ektronHeader
        {
            margin-top: .5em;
        }
    </style>

    <script language="JavaScript" src="../java/jfunct.js" type="text/javascript">
    </script>

    <script language="JavaScript" src="../java/toolbar_roll.js" type="text/javascript">
    </script>

    <script type="text/javascript">
	<!--
    // define global JS variables
    var suggestedResultID = '<asp:literal id="jsSuggestedResultID" runat="server"/>';
    var action = '<asp:literal id="jsPageAction" runat="server"/>';
    var arrSuggestedResults = new Array;
    var addEditAction = "add";
    var editArgs = 0;
    var suggestedResultRecommendedMaxSize = 10;

    function LoadLanguage(num)
    {
	    document.forms[0].action="suggestedresults.aspx?id="+suggestedResultID+"&action="+action+"&LangType="+num;
	    document.forms[0].submit();
	    return false;
    }

    function VerifyDeleteSuggestedResultSet()
    {
        var binReturn;
        binReturn = confirm('<asp:literal id="jsDelResultSet" runat="server"/>');
        return binReturn;
    }

    function VerifyDeleteSuggestedResult()
    {
        var binReturn;
        if (action == "addsuggestedresult") {
            binReturn = confirm('<asp:literal id="jsDelResult" runat="server"/>');
        }
        else {
            binReturn = confirm('<asp:literal id="jsDelResultEdit" runat="server"/>');
        }
        return binReturn;
    }

    String.prototype.unescapeHTML = function()
    {
        var htmlNode = makeNewElement("div");
        htmlNode.innerHTML = this;
        if(htmlNode.innerText)
        {
            return htmlNode.innerText; // IE
        }
        if (htmlNode.textContent)
        {
            return htmlNode.textContent; // FF
        }
        return "";
    }

    // define the JavaScript suggestedResult object for use in our various functions
    function suggestedResultObject(srID, title, link, summary, contentID, searchOrder)    {
        // define the properties
        this.id = srID
        this.title = title;
        this.summary = summary;
        this.link = link;
        this.contentID = contentID;
        this.searchOrder = searchOrder;
    }

    // helper function to remove child nodes
    function removeChildNodes(object)
    {
        if (object.hasChildNodes()) {
            while (object.childNodes.length >= 1)
            {
                object.removeChild(object.firstChild);
            }
        }
    }

    // helper function to attach events to an object
    function attachEvent(object, strNewEvent, strNewEventHandler)
    {
        try
        {
            if(object.addEventListener)
            {
                object.addEventListener(strNewEvent, eval("this." + strNewEventHandler), false);
            }
            else if (object.attachEvent)
            {
                object.attachEvent("on" + strNewEvent, eval(strNewEventHandler));
            }
            else
            {
                eval("object.on" + strNewEvent) = eval(strNewEventHandler);
            }
        }
        catch(ex){}
    }

    // helper function to get the arguments to pass through the InContext Menus (for Search Order)
    function getArgs(objectID) {
        var args = -1;
        // find the underscore
        var underScoreIndex = objectID.lastIndexOf("_");
        // get everything after the underscore
        args = objectID.substring(underScoreIndex + 1);
        if (args === "") {
            return -1;
        }
        return parseInt(args, 10);
    }

    // helper function to find the DIV element that contaisn the arguments we need
    function findArgs(obj) {
        theArgs = -1
        do {
            if (obj.args > -1) {
                theArgs = parseInt(obj.args, 10);
            }
            else {
                // go up one node and try again
                obj = obj.parentNode;
            }
        }
        while(theArgs == -1);
        return theArgs;
    }

    // helper function to Hide InContext Menus
    function HideMenus() {
        $ektron("div.inContextMenu").css("left", "-1000px");
    }

    // create a Screen object to capture the maxWidth and maxHeight of the screen
    function Screen() {
        this.maxWidth = null;
        this.maxHeight = null;
        this.setMaxWidth = function() {
            if (self.innerWidth) {
              this.maxWidth = self.innerWidth;
            }
            else if (document.documentElement && document.documentElement.clientWidth) {
              this.maxWidth = document.documentElement.clientWidth;
            }
            else if (document.body) {
              this.maxWidth = document.body.clientWidth;
            }
        }
        this.setMaxHeight = function() {
            if (self.innerHeight) {
              this.maxHeight = self.innerHeight;
            }
            else if (document.documentElement && document.documentElement.clientHeight) {
              this.maxHeight = document.documentElement.clientHeight;
            }
            else if (document.body) {
              this.maxHeight = document.body.clientHeight;
            }
        }
        this.setMaxWidth();
        this.setMaxHeight();
    }

    // helper function to Show InContext Menus
    function ShowMenu(evt, name, args) {
        HideMenus();
        var element = document.getElementById(name);
        if( element ) {
	        var offsetItemsY = 1;
	        var offsetBreaksY = 1;
	        var thisScreen = new Screen();
            var maxWidth  = thisScreen.maxWidth;
            var maxHeight = thisScreen.maxHeight;
            var ctxWidth  = 145;
            var ctxHeight = 145;
            var x = evt.clientX;
            var y = evt.clientY;
            var openX = 0;
            var openY = 0;

	        // open to the left of, or the right of, the cursor?
	        ctxHeight = offsetItemsY * 15;
	        if( ( y + ctxHeight ) > maxHeight ) {
		        openY = y - ctxHeight
	        }
	        else {
		        openY = y;
	        }

	        if( ( x + ctxWidth - 0 ) > maxWidth ) {
		        if( ( x - ctxWidth - 10 ) < 0 ) {
			        openX = x - parseInt( ctxWidth / 2 );
		        } else {
			        openX = x - ctxWidth - 5;
		        }
	        }
	        else {
		        openX = x;
	        }

	        element.style.left = openX + "px"
	        element.style.top = openY + "px"
	        element.args = args;
        }
        evt.cancelBubble = true;
    }

    // helper function for the Suggested Results to display the InContext Menus
    function SuggestedResultClickHandler(e) {
        // get the target element
        if (e.target) targetNode = e.target;
        else if (e.srcElement) targetNode = e.srcElement;
        var targetId = targetNode.id
        // make sure the user clicked on an item we can identify, if not go up until we get what we need
        if (targetId.lastIndexOf("suggestedResult_") == -1) {
            var suggestedResultTargetId = -1
            do {
                targetNode = targetNode.parentNode;
                targetId = targetNode.id;
                if (targetId.lastIndexOf("suggestedResult_") > -1) {
                    suggestedResultTargetId = targetId;
                }
            }
            while(suggestedResultTargetId == -1)
        }
        // get the arguments
        var args = getArgs(targetId);

        // figure out which menu to show
        if (arrSuggestedResults.length == 0) {
	        // no suggested results, display Add only menu
	        display_menu = "zeroSuggestedResultsMenu";
	    }
	    else if (arrSuggestedResults.length == 1) {
	        // only one suggested result, display menu sans Move Up and Move Down options
	        display_menu = "singleSuggestedResultMenu";
	    }
	    else if (args == 1) {
	        // user is hovering over the first item of the array, display menu sans Move Up option
	        display_menu = "firstSuggestedResultMenu";
	    }
	    else if (args == arrSuggestedResults.length){
	        // user is hovering over the last item of the array, display menu sans Move Down option
	        display_menu = "lastSuggestedResultMenu"
	    }
	    else {
	        // user is hovering over something in the middle, display default menu
	        display_menu = "defaultSuggestedResultMenu"
	    }

        // show the menu
        ShowMenu( e, display_menu, args );
        return false;
    }

    // helper function to catch the InContext Menu click events
    function MenuClickEventHandler( e, element ) {
        var elemParent = element.parentNode;
        var parentClass = elemParent.className
        var spaceIndex = parentClass.lastIndexOf(" ");
        var command = parentClass.substring(spaceIndex+1);
        // go up to the menu DIV and get the aruments we passed to it
        var args = findArgs(element);
        switch(command) {
            case "add":
                addSR();
                break;
            case "edit":
                addEditAction = "edit";
                editArgs = args;
                editSR(args);
                break;
            case "moveUp":
                moveUpSR(args);
                break;
            case "moveDown":
                moveDownSR(args);
                break;
            case "delete":
                deleteSR(args);
                break;
            default:
                alert("Oops!  There are no commands associated with this menu option.");
        }
    }

    function addSR()
    {
        var add_edit_Div = document.getElementById("add_edit_SuggestedResult");
        var divTermChoices = document.getElementById("termChoice");
        //var selectTermType = document.getElementById("termType")
        //var selectSyn = document.getElementById("synonymSetTerm");
        var inputSRID = document.getElementById("sr_ID");
        var inputLinkObj = document.getElementById("sr_link");
        var inputTitleObj = document.getElementById("sr_title");
        var inputContentLanguageObj = document.getElementById("sr_contentLanguage");
        var inputContentID =  document.getElementById("sr_contentID");
        // This is a hard-coded of the ClientID for the content designer in the user control.
        var inputSummaryObj = Ektron.ContentDesigner.instances["HtmlEditor1"];
        //set the value for transferAddEdit function
        addEditAction = "add"
        // reset the form fields to empty strings
        inputSRID.value = "";
        inputLinkObj.value = "";
        inputTitleObj.value = "";
        inputContentLanguageObj.value = "";
        inputContentID.value =  "";
        inputSummaryObj.setContent("document", "");
        divTermChoices = "hideElement";
        //selectTermType.className = "hideElement";
        //selectSyn.className = "hideElement"
        add_edit_Div.className = "showElement";
    }

    function editSR(searchOrder) {
        var add_edit_Div = document.getElementById("add_edit_SuggestedResult");
        var inputLinkObj = document.getElementById("sr_link");
        var inputTitleObj = document.getElementById("sr_title");
        var inputContentID =  document.getElementById("sr_contentID");
        //This is a hard-coded of the ClientID for the content designer in the user control.
        var inputSummaryObj = Ektron.ContentDesigner.instances["HtmlEditor1"];
        var inputSummaryWrapperObj = document.getElementById("htmlEditorWrapper");
        var selectedSR = arrSuggestedResults[searchOrder-1];
        var divTermChoices = document.getElementById("termChoice");
        //var selectTermType = document.getElementById("termType");
        //var selectSyn = document.getElementById("synonymSetTerm");
        // assign the selected Suggested Results parameters to the inputs
        inputLinkObj.value = selectedSR.link;
        inputTitleObj.value = selectedSR.title;
        inputContentID.value = selectedSR.contentID;
        // make a temp DIV so we can clean up the HTML of the summary
        var tempSummary = makeNewElement("div");
        tempSummary.innerHTML = selectedSR.summary
        tempSummary.innerHTML = getInnerText(tempSummary);
        try {
            inputSummaryObj.setContent("document", tempSummary.innerHTML);
        }
        catch(e) {
            alert("error:\n" + e);
        }
        tempSummary = null;
        // hide the select drop downs and reveal the add_edit layer
        divTermChoices = "hideElement";
        //selectTermType.className = "hideElement";
        inputSummaryWrapperObj.className = "showElement";
        //selectSyn.className = "hideElement"
        add_edit_Div.className = "showElement";
    }

    function deleteSR(searchOrder)
    {
        var verify = VerifyDeleteSuggestedResult();
        if (verify) {
            var tempArr = new Array;
            for (SRs = 0; SRs < arrSuggestedResults.length; SRs++) {
                // build a temp array to store the array elements we want to keep.
                if (SRs != searchOrder-1) {
                    tempArr.push(arrSuggestedResults[SRs])
                    // update the searchOrder property to match the new index value
                    tempArr[(tempArr.length-1)].searchOrder = tempArr.length
                }
            }
            // overwrite the previous array with the temp one
            arrSuggestedResults = tempArr;
            // and finally, redraw the UL accordingly
            drawSR_UL(arrSuggestedResults, document.getElementById("selectedSuggestedResults"));
            drawSR_formFields(arrSuggestedResults, document.getElementById("hiddenFormFields"));
        }
    }

    function moveUpSR(searchOrder)
    {
        var arrIndex = searchOrder-1;
        if (arrIndex != 0) {
            var srAbove = arrSuggestedResults[arrIndex-1];
            var thisSR = arrSuggestedResults[arrIndex];
            // update the searchOrder properties of each suggestedResult
            srAbove.searchOrder = searchOrder;
            thisSR.searchOrder = searchOrder-1;
            // now swapp them
            arrSuggestedResults[arrIndex] = srAbove;
            arrSuggestedResults[arrIndex-1] = thisSR;
            // and finally, redraw the UL accordingly
            drawSR_UL(arrSuggestedResults, document.getElementById("selectedSuggestedResults"));
            drawSR_formFields(arrSuggestedResults, document.getElementById("hiddenFormFields"));
        }
    }

    function moveDownSR(searchOrder)
    {
        var arrIndex = searchOrder-1;
        if (searchOrder != arrSuggestedResults.length) {
            var srBelow = arrSuggestedResults[arrIndex+1];
            var thisSR = arrSuggestedResults[arrIndex];
            // update the searchOrder properties of each suggestedResult
            srBelow.searchOrder = searchOrder;
            thisSR.searchOrder = searchOrder+1;
            // now swapp them
            arrSuggestedResults[arrIndex] = srBelow;
            arrSuggestedResults[arrIndex+1] = thisSR;
            // and finally, redraw the UL accordingly
            drawSR_UL(arrSuggestedResults, document.getElementById("selectedSuggestedResults"));
            drawSR_formFields(arrSuggestedResults, document.getElementById("hiddenFormFields"));
        }
    }

    // helper function for making DOM elements via cross-browser compatible methods
    function makeNewElement (elementTag)
    {
        try {
            newElement = document.createElement("<" + elementTag + "/>");
        }
        catch(e) {
            newElement = document.createElement(elementTag);
        }
        return newElement;
    }

    // helper function to get the Inner Text of a node via cross browser compatible methods
    function getInnerText(object)
    {
        if (object.innerText) {
            return object.innerText;
            }
        else {
            return object.textContent;
        }
    }

    function drawSR_formFields(arraySuggestedResultObjects, targetDIV)
    {
        var targetDIV = targetDIV;  // fix for IE6 bug
        // clear out the current childNodes of the target DIV
        removeChildNodes(targetDIV);
        // check the length of the array, and only add things to the DIV if needed
        if (arraySuggestedResultObjects.length > 0){
            for(SR = 0; SR < arraySuggestedResultObjects.length; SR++) {
                //create a text input for the ID
                var inputSR_ID = makeNewElement("input");
                inputSR_ID.setAttribute("id", "suggestedResult_ID_" + SR);
                inputSR_ID.setAttribute("name", "suggestedResult_ID_" + SR);
                inputSR_ID.setAttribute("value", arraySuggestedResultObjects[SR].id);
                inputSR_ID.setAttribute("type", "text");
                targetDIV.appendChild(inputSR_ID);

                //create a text input for the Title
                var inputTitle = makeNewElement("input");
                inputTitle.setAttribute("id", "suggestedResult_Title_" + SR);
                inputTitle.setAttribute("name", "suggestedResult_Title_" + SR);
                inputTitle.setAttribute("value", arraySuggestedResultObjects[SR].title);
                inputTitle.setAttribute("type", "text");
                targetDIV.appendChild(inputTitle);

                //create a text input for the Link
                var inputLink = makeNewElement("input");
                inputLink.setAttribute("id", "suggestedResult_Link_" + SR);
                inputLink.setAttribute("name", "suggestedResult_Link_" + SR);
                inputLink.setAttribute("value", arraySuggestedResultObjects[SR].link);
                inputLink.setAttribute("type", "text");
                targetDIV.appendChild(inputLink);

                //create a textarea input for the summary
                var inputLink = makeNewElement("textarea");
                inputLink.setAttribute("id", "suggestedResult_Summary_" + SR);
                inputLink.setAttribute("name", "suggestedResult_Summary_" + SR);
                targetDIV.appendChild(inputLink);
                if (inputLink.innerHTML) {
                    inputLink.innerHTML = arraySuggestedResultObjects[SR].summary;
                }
                else {
                    inputLink.value = arraySuggestedResultObjects[SR].summary;
                }

                //create a text input for the ContentID
                var inputContentID = makeNewElement("input");
                inputContentID.setAttribute("id", "suggestedResult_ContentID_" + SR);
                inputContentID.setAttribute("name", "suggestedResult_ContentID_" + SR);
                inputContentID.setAttribute("value", arraySuggestedResultObjects[SR].contentID);
                inputContentID.setAttribute("type", "text");
                targetDIV.appendChild(inputContentID);

                //create a text input for the Search Order
                var inputSearchOrder = makeNewElement("input");
                inputSearchOrder.setAttribute("id", "suggestedResult_SearchOrder_" + SR);
                inputSearchOrder.setAttribute("name", "suggestedResult_SearchOrder_" + SR);
                inputSearchOrder.setAttribute("value", arraySuggestedResultObjects[SR].searchOrder);
                inputSearchOrder.setAttribute("type", "text");
                targetDIV.appendChild(inputSearchOrder);
            }
            //create a text input for the Number of Suggested Results
            var inputNumSuggestedResults = makeNewElement("input");
            inputNumSuggestedResults.setAttribute("id", "numSuggestedResults");
            inputNumSuggestedResults.setAttribute("name", "numSuggestedResults");
            inputNumSuggestedResults.setAttribute("value", arraySuggestedResultObjects.length);
            inputNumSuggestedResults.setAttribute("type", "text");
            targetDIV.appendChild(inputNumSuggestedResults);
        }
    }

    function findAnchor(e)
    {
        // get the target element
        if (e.target) targetNode = e.target;
        else if (e.srcElement) targetNode = e.srcElement;
        while (targetNode.className != "menuAction") {
            targetNode = targetNode.parentNode;
        }
        return targetNode;
    }

    function changeBackgroundOver(e)
    {
        // get the target element
        targetNode = findAnchor(e);
        targetNode.style.background = "\#eeeeee"
    }

    function changeBackgroundOut(e)
    {
        // get the target element
        targetNode = findAnchor(e);
        targetNode.style.background = "\#ffffff"
    }

    function drawSR_UL(arraySuggestedResultObjects, targetUL)
    {
        var resultSizeWarning = document.getElementById("resultSizeWarning");
        var targetUL = targetUL; // fix for IE6 bug
        // clear out the current childNodes of the target UL
        removeChildNodes(targetUL);
        if (arraySuggestedResultObjects.length > 0) {
            var totalHeight = 0;
            for(SR = 0; SR < arraySuggestedResultObjects.length; SR++) {
                // create a new LI node and append it to the targetUL
                var tempLI = makeNewElement("li");
                tempLI.setAttribute("id", "suggestedResult_li_" + arraySuggestedResultObjects[SR].searchOrder);
                tempLI.className = "suggestedResult";

                // create a new anchor tag and append it to the LI you just made
                var tempAnchor = makeNewElement("a");
                // add the event handlers
                attachEvent(tempAnchor, "click", "SuggestedResultClickHandler");
                attachEvent(tempAnchor, "contextmenu", "SuggestedResultClickHandler");
                attachEvent(tempAnchor, "contextmenu", "SuggestedResultClickHandler");
                // fix background rollovers for IE6
                attachEvent(tempAnchor, "mouseover", "changeBackgroundOver");
                attachEvent(tempAnchor, "mouseout", "changeBackgroundOut");
                tempAnchor.className = "menuAction";
                tempAnchor.setAttribute("id", "suggestedResult_" + arraySuggestedResultObjects[SR].searchOrder);
                // create a new pseudo link for the UI display
                var tempPseudoLink = makeNewElement("span");
                tempPseudoLink.className = "suggestedResultLink";
                tempPseudoLink.setAttribute("id", "suggestedResult_link_" + arraySuggestedResultObjects[SR].searchOrder);
                tempPseudoLink.setAttribute("title", arraySuggestedResultObjects[SR].link);
                var titleTXT =  document.createTextNode(arraySuggestedResultObjects[SR].title);
                tempPseudoLink.appendChild(titleTXT);

                // create a new summary paragraph
                var tempSummary = makeNewElement("div");
                tempSummary.setAttribute("id", "suggestedResult_summary_" + arraySuggestedResultObjects[SR].searchOrder);
                tempSummary.className = "suggestedResultSummary";
                tempSummary.innerHTML = arraySuggestedResultObjects[SR].summary
                tempSummary.innerHTML = getInnerText(tempSummary);

                // create a new contentID span
                var tempContentID = makeNewElement("span");
                tempContentID.setAttribute("id", "suggestedResult_contentID_" + arraySuggestedResultObjects[SR].searchOrder)
                var tempContentID_TXT = "";
                if (arraySuggestedResultObjects[SR].contentID != "") {
                    tempContentID_TXT = document.createTextNode(arraySuggestedResultObjects[SR].contentID);
                }
                else {
                    tempContentID_TXT = document.createTextNode(tempContentID_TXT);
                }
                tempContentID.appendChild(tempContentID_TXT);
                tempContentID.className = "suggestedResultContentID"

                // create a new suggested result ID span
                var tempSR_ID = makeNewElement("span");
                tempSR_ID.setAttribute("id", "suggestedResult_srID_" + arraySuggestedResultObjects[SR].searchOrder)
                var tempSR_ID_TXT = document.createTextNode(arraySuggestedResultObjects[SR].srID);
                tempSR_ID.appendChild(tempSR_ID_TXT);
                tempSR_ID.className = "suggestedResultID"

                // append everything properly
                tempAnchor.appendChild(tempPseudoLink);
                tempAnchor.appendChild(tempSummary);
                tempAnchor.appendChild(tempContentID);
                tempAnchor.appendChild(tempSR_ID);
                tempLI.appendChild(tempAnchor);
                targetUL.appendChild(tempLI);

                // add this LI's height to the TotalHeight
                totalHeight += tempLI.offsetHeight;
            }
            var lastLI = targetUL.lastChild;
            var lastLIAnchor = lastLI.firstChild;
            if (targetUL.parentNode.offsetHeight > totalHeight) {
                lastLIAnchor.style.height = (targetUL.offsetHeight - (totalHeight - lastLI.offsetHeight)) + "px";
            }
        }
        else
        {
            //just output one BIG LI to fill the UL
            var tempLI = makeNewElement("li" );
            tempLI.setAttribute("id", "suggestedResult_li");
            tempLI.className = "fillToTop";

            var tempAnchor = makeNewElement("a" );
            // add the mouseover event handlers
            attachEvent(tempAnchor, "click", "SuggestedResultClickHandler");
            attachEvent(tempAnchor, "contextmenu", "SuggestedResultClickHandler");
            // fix background rollovers for IE6
            attachEvent(tempAnchor, "mouseover", "changeBackgroundOver");
            attachEvent(tempAnchor, "mouseout", "changeBackgroundOut");
            tempAnchor.className = "menuAction";
            tempAnchor.setAttribute("id", "suggestedResult_0");
            var tempAnchorTXT = document.createTextNode("\u00a0");
            tempAnchor.appendChild(tempAnchorTXT);
            tempLI.appendChild(tempAnchor);
            targetUL.appendChild(tempLI);
        }
        if (arraySuggestedResultObjects.length > suggestedResultRecommendedMaxSize)
        {
            resultSizeWarning.style.display = "block";
            resultSizeWarning.style.visibility = "visible";
        }
        else
        {
            resultSizeWarning.style.display = "none";
            resultSizeWarning.style.visibility = "hidden";
        }
     }

    function cancelAddEdit()
    {
        var add_edit_Div = document.getElementById("add_edit_SuggestedResult");
        var divTermChoices = document.getElementById("termChoice");
        //var selectTermType = document.getElementById("termType");
        divTermChoices = "showElement";
        //selectTermType.className = "showElement";
	    add_edit_Div.className = "hideElement";
    }

    function CloseChildPage()
    {
        var inputSummaryWrapperObj = document.getElementById("htmlEditorWrapper");
	    inputSummaryWrapperObj.className = "showElement";
	    $ektron(".ektronCMSContent").modalHide();
    }

    function LoadChildPage(LanguageID)
    {
	    var frameObj = document.getElementById("ChildPage");
	    frameObj.src = "../blankredirect.aspx?SelectCreateContent.aspx?FolderID=0&LangType=" + LanguageID + "&browser=1&ty=suggestedResults"
	    var inputSummaryWrapperObj = document.getElementById("htmlEditorWrapper");
	    var divTermChoices = document.getElementById("termChoice");
        divTermChoices = "hideElement";

	    inputSummaryWrapperObj.className = "hideElement";
	    $ektron(".ektronCMSContent").modalShow();
    }

    function ReturnChildValue(content_id,content_title,Content_QLink,objTeaser,folderId,languageID) {
        // let's get some page objects to manipulate
        var inputLinkObj = document.getElementById("sr_link");
        var inputTitleObj = document.getElementById("sr_title");
        var inputContentLanguageObj = document.getElementById("sr_contentLanguage");
        var inputContentID =  document.getElementById("sr_contentID");
        //This is a hard-coded of the ClientID for the content designer in the user control.
        //I tried using FindControl("addsuggestedresult").FindControl("HtmlEditor1").ClientID and I get an
        //error in .Net.
        var inputSummaryObj = Ektron.ContentDesigner.instances["HtmlEditor1"];
        var inputSummaryWrapperObj = document.getElementById("htmlEditorWrapper");
        // hide the iframe page
        $ektron(".ektronCMSContent").modalHide();
        //show the eWebEditPro Editor
        inputSummaryWrapperObj.className = "showElement";
        // pass the proper values to the form fields
        inputLinkObj.value = Content_QLink;
        inputTitleObj.value = content_title;
        inputContentLanguageObj.value = languageID;
        inputContentID.value= content_id;
        // add the teaser to the eWebEditPro instance
        try
        {
            inputSummaryObj.setContent("document", objTeaser.value);
        }
        catch(e)
        {
            alert("error:\n" + e);
        }
    }

    // helper function to better manage onload calls
    function addLoadEvent(func)
    {
        var oldonload = window.onload;
        if (typeof window.onload != 'function') {
            window.onload = func;
        } else {
            window.onload = function() {
            if (oldonload) {
                oldonload();
            }
            func();
        }
      }
    }

    // function to add and/or edit a suggested result for the array
    function transferAddEdit()
    {
        var inputSRID = document.getElementById("sr_ID");
        var inputLinkObj = document.getElementById("sr_link");
        var inputTitleObj = document.getElementById("sr_title");
        var inputContentLanguageObj = document.getElementById("sr_contentLanguage");
        var inputContentID =  document.getElementById("sr_contentID");
        //This is a hard-coded of the ClientID for the content designer in the user control.
        //I tried using FindControl("addsuggestedresult").FindControl("HtmlEditor1").ClientID and I get an
        //error in .Net.
        var inputSummaryObj = Ektron.ContentDesigner.instances["HtmlEditor1"];
        var inputSummary = "";
        // call the eWebEditPro save method and grab the editor's contents
        try {
            // get a lock on the eWebEditPro hidden element
            inputSummary = inputSummaryObj.getContent();
            var regExStripLineBreaks = new RegExp("\\n", "g" );
            var inputSummary = inputSummary.replace(regExStripLineBreaks, "");
        }
        catch(e) {
            alert("error:\n" + e);
        }
        // define variables for error checking
        var errMsg = ""
        var inputSummaryStripped = ""
        if (inputSummary.length > 0) {
            inputSummaryStripped = (inputSummary.unescapeHTML()).replace( /<[^<|>]+?>/gi,'' );
        }
        // check for errors
        if (inputLinkObj.value == "") {
            errMsg = '<asp:literal id="jsLinkReq" runat="server"/>\n'
        }
        if (inputTitleObj.value == "") {
            errMsg += '<asp:literal id="jsTitleReq" runat="server"/>\n'
        }
        if (inputSummary.length == 0) {
            errMsg += '<asp:literal id="jsSummaryReq" runat="server"/>\n'
        }
        else if (inputSummaryStripped.length > 320){
            errMsg += '<asp:literal id="jsSizeExceeded" runat="server"/>\n'
        }
        if (errMsg.length > 0) {
            alert(errMsg);
        }
        else {
            // create a temporary suggestedResultObject to store the values
            var newSuggestedResult = new suggestedResultObject("", "", "", "", "", "");
            newSuggestedResult.srID = $ektron.trim(inputSRID.value);
            newSuggestedResult.title = $ektron.trim(inputTitleObj.value);
            newSuggestedResult.link = $ektron.trim(inputLinkObj.value);
            newSuggestedResult.summary = String(inputSummary);
            newSuggestedResult.contentID = $ektron.trim(inputContentID.value);
            // add/update the array with the temporary suggestedResult
            if (addEditAction === "add") {
                arrSuggestedResults.push(newSuggestedResult);
                arrSuggestedResults[arrSuggestedResults.length-1].searchOrder = (arrSuggestedResults.length);
            }
            else {
                newSuggestedResult.searchOrder = editArgs;
                arrSuggestedResults[editArgs-1] = newSuggestedResult;
            }
            // redraw the UL to reflect the changes.
            drawSR_UL(arrSuggestedResults, document.getElementById("selectedSuggestedResults"));
            //redraw the additional form fields
            drawSR_formFields(arrSuggestedResults, document.getElementById("hiddenFormFields"));
            // hide the add_edit div layer
            cancelAddEdit();
            // reset the form fields to empty strings
            inputSRID.value = "";
            inputLinkObj.value = "";
            inputTitleObj.value = "";
            inputContentLanguageObj.value = "";
            inputContentID.value =  "";
            inputSummary = "";
        }
    }

    function checkAddSuggestedResults(formToProcess)
    {
        //var termType = document.getElementById("termType");
        //var termTypeValue = termType[termType.selectedIndex].value;
        var term = document.getElementById("addsuggestedresult_txtSynonyms").value;
        //var synSet = document.getElementById("synonymSetTerm")
        //var synSetValue = synSet[synSet.selectedIndex].value;
        var regExpComma = new RegExp("[,]");
        var regExpParenthesis = new RegExp("[()]")
        var binErr = false;
        var strErrMsg = "";

        // check what kind of term the user wants and validate accordingly
        //if (termTypeValue == "0") {
            //user is trying to add SR associated with a single term
            if (term.length == 0) {
                binErr = true;
                strErrMsg += '<asp:literal id="jsTermReq" runat="server"/>\n'

            }
            else {
                if (regExpComma.exec(term) != null ) {
                    binErr = true;
                    strErrMsg += '<asp:literal id="jsTermNoCommas" runat="server"/>\n'
                }
                if (regExpParenthesis.exec(term) != null) {
                    binErr = true;
                    strErrMsg += '<asp:literal id="jsTermsNoParenthesis" runat="server"/>\n'
                }
            }
//        }
//        else {
//            if (synSetValue == "-1") {
//                binErr = true;
//                strErrMsg += '<asp:literal id="jsSynonymSetReq" runat="server"/>\n'
//            }
//        }
        //make sure we've got at least one Suggested Result in the array
        if (arrSuggestedResults.length == 0) {
            binErr = true;
            strErrMsg += '<asp:literal id="jsSugResultReq" runat="server"/>\n'
        }
        // if all is well, submit the form, otherwise show any errors
        if (binErr == false) {
            document.forms[0].addsuggestedresult$submitMode.value = "0";
            formToProcess.submit();
        }
        else {
            alert(strErrMsg);
        }
    }
    
    //-->
    </script>

</head>
<body onclick="HideMenus()">
    <form id="Form1" method="post" runat="server">
    <asp:PlaceHolder ID="DataHolder" runat="server"></asp:PlaceHolder>
    </form>

    <script type="text/javascript">
    <!--
    // if we're in add mode, we need to run a few functions on load
    if (action=="addsuggestedresult" || action=="editsuggestedresult")
    {
        Ektron.ready(function(){
            //var selectTermType = $ektron("#termType")[0];

            drawSR_UL(arrSuggestedResults, $ektron("#selectedSuggestedResults")[0]);

            drawSR_formFields(arrSuggestedResults, $ektron("#hiddenFormFields")[0]);

            // init any modals
            $ektron(".ektronCMSContent").modal({
                modal: true,
                toTop: true,
                onShow: function(hash) {
                    hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");;
                    hash.o.fadeTo("fast", 0.5, function() {
				        hash.w.fadeIn("fast");
			        });
                },
                onHide: function(hash) {
                    hash.w.fadeOut("fast");
			        hash.o.fadeOut("fast", function(){
				        if (hash.o)
				        {
					        hash.o.remove();
			            }
			        });
                }
            });
        });
    }
    //-->
    </script>

</body>
</html>

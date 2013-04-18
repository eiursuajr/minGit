<!--
var g_formName = '';

function CancelWindow() {
	self.close();
}

function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
	var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
	var popupwin = window.open(url, hWind, cToolBar);
	return popupwin;
}

function openWindow(url){
	newWindow=window.open(url,'newWin','tollbar=yes,location=yes,scrollbars=yes,width=300,hight=200')
}

function openWindow2(url){
	newWindow=window.open(url,'newWin','tollbar=yes,menubar=yes,location=yes,scrollbars=yes,directories=yes')
}

function closeWindow()
	{
	if(newWindow && !newWindow.closed)
		{
			newWindow.close()
		}
	}

function PopUpWindowFull (url, hWind, nWidth, nHeight, nResize)
{
	var cToolBar = "toolbar=yes,location=yes,directories=yes,resizable=1,status=" + nResize + ",menubar=yes,scrollbars=yes,width=" + nWidth + ",height=" + nHeight
	var popupwin = window.open(url, hWind, cToolBar);
	return popupwin;
}
function QuickLinkSelect(baseFolderID, formName,  titleFormElem, useQLinkCheck, SetBrowserState) {
	// There are specific requirements for this function. See "QuickLinkSelect.aspx" for those.
	//
	return(QuickLinkSelectv48(baseFolderID, formName,  titleFormElem, useQLinkCheck, SetBrowserState, 0)) ;
}

function QuickLinkSelectv48(baseFolderID, formName,  titleFormElem, useQLinkCheck, SetBrowserState, forceTemplate) {
	// There are specific requirements for this function. See "QuickLinkSelect.aspx" for those.
	//
	// This function was added to give the programmer the ability to Force Template Use Only on the
	// returned Quicklinks. This was because we may not want to internally use the externally aliased links.
	//var sendQString = '?folderid=' + baseFolderID + '&formName=' + formName + '&titleFormElem=' +
	//			titleFormElem + '&useQLinkCheck=' +  useQLinkCheck + '&SetBrowserState=' + SetBrowserState
	//			+ '&forcetemplate=' + forceTemplate + '&disAllowAddContent=1' ;
	//var cToolBar = 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=450,height=300';
	//window.open('QuickLinkSelect.aspx' + sendQString, 'QlinkSelectWin', cToolBar) ;
	//return true ;
	return(QuickLinkSelectBase(baseFolderID, formName,  titleFormElem, useQLinkCheck, SetBrowserState, forceTemplate,0));
}

function QuickLinkSelectBase(baseFolderID, formName,  titleFormElem, useQLinkCheck, SetBrowserState, forceTemplate,AddContent) {
	// This is the base quicklink insertion, which allows you to select adding a contentblock
	var sendQString = '?folderid=' + baseFolderID + '&formName=' + formName + '&titleFormElem=' +
				titleFormElem + '&useQLinkCheck=' +  useQLinkCheck + '&SetBrowserState=' + SetBrowserState
				+ '&forcetemplate=' + forceTemplate + '&disAllowAddContent=' + AddContent;

	var cToolBar = 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=450,height=300';
	window.open('QuickLinkSelect.aspx' + sendQString, 'QlinkSelectWin', cToolBar) ;

	return true ;
}

//-->

	var g_emailChecked = false;

	function PopUpWindow_Email (url, hWind, nWidth, nHeight, nScroll, nResize) {
		var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
		var popupwin = window.open(url, hWind, cToolBar);
		return popupwin;
	}

	function IsBrowserIE_Email() {
	    if (window.location.href.indexOf('override_ie=true') > 0)
	        return false;

	      if (null == parent.document.getElementById("EmailFrameContainer"))
	        return false;

		// document.all is an IE only property
		return (document.all ? true : false);
	}

	function ToggleEmailCheckboxes() {
		var idx, prefix, name;

		g_emailChecked = !g_emailChecked;
		for(idx = 0; idx < document.forms[0].elements.length; idx++ ) {
			if (document.forms[0].elements[idx].type == "checkbox") {
				name = document.forms[0].elements[idx].name;
				if (name.indexOf("emailcheckbox_") != -1) {
					document.forms[0].elements[idx].checked = g_emailChecked;
				}
			}
		}
	}
	
	 
    //This function retrieves the values of the variables in the HTTP query string and it takes key as paramerter.
    //returns null value if the key doesn't exist.
    function getQueryStringValue(key){
    try{
          key = key.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
          var regex = new RegExp("[\\?&]"+key+"=([^&#]*)");
          var arMatchedCollection = regex.exec(window.location.href);
          if(arMatchedCollection == null)
            return null;
          else
            return arMatchedCollection[1];
       }
     catch(err){
            return null;
        }  
    }
	
	// Open email window/layer ontop of current window:
	function LoadEmailChildPage(userGrpId) {
		var pageObj, frameObj

		if (IsBrowserIE_Email()) {
			// Configure the email window to be visible:
			frameObj = document.getElementById("EmailChildPage");
			if ((typeof(frameObj) == "object") && (frameObj != null)) {
				frameObj.src = "blankredirect.aspx?email.aspx?" + userGrpId;
				pageObj = document.getElementById("EmailFrameContainer");
				pageObj.style.display = "";
				pageObj.style.width = "85%";
				pageObj.style.height = "90%";

				// Ensure that the transparent layer completely covers the parent window:
				pageObj = document.getElementById("EmailActiveOverlay");
				pageObj.style.display = "";
				pageObj.style.width = "100%";
				pageObj.style.height = "100%";
			}
		}
		else {
			// Using Netscape; cant use transparencies & eWebEditPro preperly
			// - so launch in a seperate pop-up window:
			PopUpWindow_Email("blankredirect.aspx?email.aspx?" + userGrpId,"CMSEmail",490,500,1,1);
		}
	}

	// Open email window/layer ontop of current window (extended version,
	// iterates through the controls to determine which usuer/group to add):
    var jsEmailNoUserMsg="email error: no users selected to receive email";
	function LoadEmailChildPageEx() {
		var idx, name, prefix, userGrpId, pageObj, qtyElements, frameObj, haveTargets=false;

		// build user-group ID string, based on current check-box status:
		userGrpId = "";
		prefix = "emailcheckbox_";
		qtyElements = document.forms[0].elements.length
		for(idx = 0; idx < qtyElements; idx++ ) {
			if (document.forms[0].elements[idx].type == "checkbox"){
				name = document.forms[0].elements[idx].name;
				if ((name.length > prefix.length)
					&& (0 == name.indexOf(prefix))
					&& (document.forms[0].elements[idx].checked == true)) {
					userGrpId = userGrpId + name.substring(prefix.length) + ",";
					haveTargets = true;
				}
			}
		}
		if (haveTargets) {
			// Build either a user array or a group array:
			if (typeof(document.forms[0].groupMarker) == "undefined")
				userGrpId = "userarray=" + escape(userGrpId.substring(0, userGrpId.length - 1));
			else
				userGrpId = "grouparray=" + escape(userGrpId.substring(0, userGrpId.length - 1));
			if (IsBrowserIE_Email()) {
				frameObj = document.getElementById("EmailChildPage");
				if ((typeof(frameObj) == "object") && (frameObj != null)) {
					frameObj.src = "blankredirect.aspx?email.aspx?" + userGrpId;
					pageObj = document.getElementById("EmailFrameContainer");
					pageObj.style.display = "";
					pageObj.style.width = "85%";
					pageObj.style.height = "90%";

					pageObj = document.getElementById("EmailActiveOverlay");
					pageObj.style.display = "";
					pageObj.style.width = "100%";
					pageObj.style.height = "100%";
				}
			}
			else {
				PopUpWindow_Email("blankredirect.aspx?email.aspx?" + userGrpId,"CMSEmail",490,600,1,1);
			}
		}
		else {
			alert(jsEmailNoUserMsg);  //JAVASCRIPT MESSAGE SHOUD READ FROM MESSAGE OBJECT'TODO:UDAI 04/27
		}
	}


	// Close email window/layer:
	function CloseEmailChildPage() {
		var pageObj;
		if (document.getElementById("EmailFrameContainer") != null) {
			pageObj = document.getElementById("EmailFrameContainer");
		}
		else {
			if (IsBrowserIE_Email()){
				if (parent.document.getElementById("EmailFrameContainer") != null) {
					pageObj = parent.document.getElementById("EmailFrameContainer");
				}
			}
			else{
				top.close();
			}
		}
		// Configure the email window to be invisible:
		if (pageObj && ("undefined" != typeof pageObj))
		{
			pageObj.style.display = "none";
			pageObj.style.width = "1px";
			pageObj.style.height = "1px";
		}

		// Ensure that the transparent layer does not cover any of the parent window:
		if (document.getElementById("EmailActiveOverlay") != null) {
			pageObj = document.getElementById("EmailActiveOverlay");
		}
		else {
			if (IsBrowserIE_Email()){
				if (parent.document.getElementById("EmailActiveOverlay") != null) {
					pageObj = parent.document.getElementById("EmailActiveOverlay");
				}
			}
			else
			{
				top.close();
			}
		}
		if (pageObj && ("undefined" != typeof pageObj))
		{
		pageObj.style.display = "none";
		pageObj.style.width = "1px";
		pageObj.style.height = "1px";
		}
	}


function Trim (string) {
	if (string.length > 0) {
		string = RemoveLeadingSpaces (string);
	}
	if (string.length > 0) {
		string = RemoveTrailingSpaces(string);
	}
	return string;
}

function RemoveLeadingSpaces(string) {
	while(string.substring(0, 1) == " ") {
		string = string.substring(1, string.length);
	}
	return string;
}

function RemoveTrailingSpaces(string) {
	while(string.substring((string.length - 1), string.length) == " ") {
		string = string.substring(0, (string.length - 1));
	}
	return string ;
}


function EnableElement(id, bEnable)
{
	var objElem = document.getElementById(id);
	if (objElem)
	{
		// Mozilla FireFox 1.0 does not support .disabled for <label>
		if (typeof objElem.disabled != "undefined")
		{
			objElem.disabled = !bEnable;
		}
		// if a label, follow the 'for' attribute as well
		if (typeof objElem.htmlFor != "undefined")
		{
			// don't use recursion to avoid possibility of infinite recursion
			objElem = document.getElementById(objElem.htmlFor);
			if (objElem && typeof objElem.disabled != "undefined")
			{
				objElem.disabled = !bEnable;
			}
		}
	}
}

function ShowElement(id, bShow)
{
	var objElem = document.getElementById(id);
	if (objElem)
	{
		objElem.style.display = (bShow ? "" : "none");
		ShowSelectElements(objElem, bShow);
	}
}

function ShowSelectElements(objElem, bShow)
// due to a bug in IE, descendent select lists may still be displayed
// so target each individually
{
	if (!objElem) return;
	if ("undefined" == typeof objElem.children) return;
	if ("SELECT" == objElem.tagName)
	{
		objElem.style.display = (bShow ? "" : "none");
	}
	else
	{
		for (var i = 0; i < objElem.children.length; i++)
		{
			ShowSelectElements(objElem.children[i], bShow);
		}
	}
}

function GetElementValue(oElem)
{
	if (!oElem) return;
	if (typeof oElem.value != "undefined")
	{
		if ("INPUT" == oElem.tagName && ("checkbox" == oElem.type || "radio" == oElem.type))
		{
			var strValue = oElem.value + "";
			if (strValue.length > 0 && strValue != "true")
			{
				if (oElem.checked)
				{
					return strValue;
				}
				else
				{
					return "";
				}
			}
			else // boolean
			{
				if (oElem.checked)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		else
		{
			return oElem.value;
		}
	}
	else if (typeof oElem.innerHTML != "undefined")
	{
		return oElem.innerHTML;
	}
	else if (typeof oElem.innerText != "undefined")
	{
		return oElem.innerText;
	}
	else
	{
		return; // no value
	}
}

function SetElementValue(oElem, value)
{
	if (!oElem) return;
	if (typeof oElem.value != "undefined")
	{
		if ("INPUT" == oElem.tagName && ("checkbox" == oElem.type))
		{
			// boolean
			if ("true" == value || true == value)
			{
				oElem.checked = true;
			}
			else if ("false" == value || false == value)
			{
				oElem.checked = false;
			}
			else
			{
				oElem.value = value;
			}
		}
		else
		{
			oElem.value = value;
		}
	}
	else if (typeof oElem.innerHTML != "undefined")
	{
		oElem.innerHTML = value;
	}
	else if (typeof oElem.innerText != "undefined")
	{
		oElem.innerText = value;
	}
}

function numberValidate(field)
{
	if ( !( /^(\-?([0-9]*\.)?[0-9]+)$/.test(field.value) ) && field.value != "")
	{
		alert("You must enter a valid number:\nOnly decimal digits and a period allowed\n(Examples: 123 or 123.456)");
		field.value = "";
		attemptFocus(field);
		return false;
	}
	return true;
}

function textareaValidate(field)
{
	var strText = GetElementValue(field) + "";
	var len = strText.length;
	if(len > 2000)
	{
		alert("Over limit of characters in text box.");
		attemptFocus(field);
		return false;
	}
	return true;
}
function ValidateMeta(formName)
// CAUTION: do not name this function 'V a l i d a t e M e t a d a t a',
// suspect the word 'M e t a d a t a' causes ASP to omit the JavaScript
{
	var reqfields;
	var validationfields;
	var validpair;
	var objform = document.forms[formName];
	g_formName = formName;

	if(objform.req_fields)
	{
		reqfields = objform.req_fields.value.split(",");
		for(var i = 0; i < reqfields.length; i++)
		{
			if(objform.elements[reqfields[i]])
			{
				if(objform.elements[reqfields[i]].value == "")
				{
					// alert('<%= (CmsMsg("js: alert cannot submit meta incomplete"))%>');
					alert('You did not fill out all required metadata fields.') ;
                    $ektron("a#divMetaData").click();
					attemptSetTab('dvMetadata');
					attemptFocus(objform.elements[reqfields[i]]);
					if ($ektron("#editContentTabs li").length > 0)
				        $ektron("#editContentTabs li").removeClass("ui-tabs-selected ui-state-active");
				    if ($ektron("#liMetadata").length > 0)    
                        $ektron("#liMetadata").addClass("ui-tabs-selected ui-state-active");
					return false;
				}
			}
		}
	}

	if(objform.needed_validation)
	{
		validationfields = objform.needed_validation.value.split(":");
		for(var j = 0; j < validationfields.length; j++)
		{
			validpair = validationfields[j].split(",");
			if(objform.elements[validpair[0]])
			{
				if(validpair[1] == "number")
				{
					if(!numberValidate(objform.elements[validpair[0]]))
					{
						return false;
					}
				}
				else if(validpair[1] == "text")
				{
					if(!textareaValidate(objform.elements[validpair[0]]))
					{
						return false;
					}
				}
				else if(validpair[1] == "date")
				{
					/*
						Date Validation no longer required.
					var originalDate;
					var arrayForNumber = validpair[0].split("_");
					if(arrayForNumber.length > 0)
					{
						if(arrayForNumber[0] = "frm_text")
						{
							originalDate = "frm_text_sel" + arrayForNumber[arrayForNumber.length-1];
							objform.elements[originalDate].value = Trim(objform.elements[originalDate].value);
						}
					}
					else
					{
						originalDate = validpair[0];
					}
					if(!CheckDate(objform.elements[originalDate].value))
					{
						return false;
					}
					*/
				}
			}
		}
	}
	return true;
}

function attemptSetTab(targetTab){
	try
	{
		if ( (typeof(ShowPane)).toLowerCase() == 'function'){
			ShowPane(targetTab);
		}
	}
	catch (e)
	{
		// eat error; unable to select tab, for any number of reasons...
	}
}

function attemptFocus(objElem)
{
	try
	{
		objElem.focus();
	}
	catch (e)
	{
		// unable to set focus for any number of reasons
	}
}

function LoadFolderChildPage(PageAction, ContentLanguage) {
		var pageObj, frameObj
		if("siteupdateactivity" == PageAction)
		{
			PageAction = PageAction + "&noblogfolders=1";
		}
		if (IsBrowserIE_Email()) {
			// Configure the email window to be visible:
			frameObj = document.getElementById("EmailChildPage");
			if ((typeof(frameObj) == "object") && (frameObj != null)) {
				//frameObj.src = "blankredirect.aspx?SelectCreateContent.aspx?action=" + PageAction + "&FolderID=0&LangType=" + ContentLanguage + "&browser=0&for_tasks=0&from_page=report";
				frameObj.src = "blankredirect.aspx?SelectFolder.aspx?action=" + PageAction + "&FolderID=0&LangType=" + ContentLanguage + "&browser=0&from_page=report";
				pageObj = document.getElementById("EmailFrameContainer");
				pageObj.style.display = "";
				pageObj.style.width = "85%";
				pageObj.style.height = "90%";

				// Ensure that the transparent layer completely covers the parent window:
				pageObj = document.getElementById("EmailActiveOverlay");
				pageObj.style.display = "";
				pageObj.style.width = "100%";
				pageObj.style.height = "100%";
			}
		}
		else {
			// Using Netscape; cant use transparencies & eWebEditPro preperly
			// - so launch in a seperate pop-up window:
			//PopUpWindow_Email("blankredirect.aspx?SelectCreateContent.aspx?action=" + PageAction + "&FolderID=0&LangType=" + ContentLanguage + "&browser=1&for_tasks=0&from_page=report","CMSEmail",490,500,1,1);
			PopUpWindow_Email("blankredirect.aspx?SelectFolder.aspx?action=" + PageAction + "&FolderID=0&LangType=" + ContentLanguage + "&browser=1&for_tasks=0&from_page=report","CMSEmail",490,500,1,1);
	}
}

function LoadUserListChildPage(PageAction)
{
		var idx, name, prefix, userGrpId, pageObj, qtyElements, hid1, hid2, frameObj, haveTargets=false, rptStatus;
			if (PageAction == undefined) {
				rptStatus = "";
			}
			else {
				rptStatus = PageAction;
			}
			// If user is trying to email a chart, show alert that it cannot be done
			if (document.getElementById("chart") != null) {
				alert ('Charts cannot be emailed');
				return false;
			}

			if (document.getElementById("ReportDataGrid") != null && document.getElementById("ReportDataGrid").innerHTML != "") {
				// Get report body for Content reports from reports.aspx
				rptObj =  document.getElementById("ReportDataGrid");
				hid1 = document.getElementById("rptHtml");
				hid1.value = rptObj.innerHTML; //FF does not support outerHTML
			}
			else {
				// get the report grid and title for the approval from viewApprovalFolder_ViewApproval control
				if (document.getElementById("viewApprovalList_ViewGrid") != null) {
					// Get report body for Approval reports from approval.aspx
					rptObj =  document.getElementById("viewApprovalList_ViewGrid");
					hid1 = document.getElementById("rptHtml");
					hid1.value = rptObj.innerHTML; //FF does not support outerHTML
				}
			}
			//Get the report title from the report form
			txtVal = document.forms[0].title;
			if (txtVal.indexOf("Content Reports:") > -1) {
				txtVal = txtVal.substring(txtVal.indexOf("Content Reports:")+16)
				txtVal = txtVal.substring(0,txtVal.indexOf("</SPAN>"));
			}
			else {
				if (txtVal.indexOf("View All") > -1) {
					txtVal = txtVal.substring(txtVal.indexOf("View All"))
					txtVal = txtVal.substring(0,txtVal.indexOf("</SPAN>"));
				}
			}
			hid2 = document.getElementById("rptTitle");
			txtVal = txtVal.replace(/^\s*|\s*$/g,"");
			hid2.value = (hid2.value != "" ? hid2.value : txtVal);
			var urlparam = "";
			if( document.location.href.toLowerCase().indexOf('action=siteupdateactivity') > -1)
			{
			    if ("string" == typeof document.getElementById("excludeUserGroups").value)
			    {
				    var sGroup = document.getElementById("excludeUserGroups").value + "";
				    if (sGroup != "")
				    {
					    urlparam = urlparam + "&group_ids=" + sGroup;
				    }
			    }
			    if ("string" == typeof document.getElementById("excludeUserIds").value)
			    {
				    var sUser = document.getElementById("excludeUserIds").value + "";
				    if (sUser != "")
				    {
					    urlparam = urlparam + "&user_ids=" + sUser;
				    }
			    }
			}
			// Build either a user array or a group array:
			if (IsBrowserIE_Email()) {
				frameObj = document.getElementById("EmailChildPage");
				if ((typeof(frameObj) == "object") && (frameObj != null)) {
					frameObj.src = "blankredirect.aspx?SelectUserGroup.aspx?action=Report&rptStatus=" + rptStatus + urlparam;
					pageObj = document.getElementById("EmailFrameContainer");
					pageObj.style.display = "";
					pageObj.style.width = "85%";
					pageObj.style.height = "90%";

					pageObj = document.getElementById("EmailActiveOverlay");
					pageObj.style.display = "";
					pageObj.style.width = "100%";
					pageObj.style.height = "100%";
				}
			}
			else {
				PopUpWindow_Email("blankredirect.aspx?SelectUserGroup.aspx?action=Report&rptStatus=" + rptStatus + urlparam,"CMSEmail",490,500,1,1);
			}
}

var ekt_winprint = null;
var ekt_rpthtml = null;
var ekt_rpttitle = null;
function delayedPrintReport() {
    // change to use a blank aspx page instead of a html page to load the js and css at code behind.
    if (ekt_winprint.location.href.indexOf("printreport.aspx") < -1) {
        setTimeout("delayedPrintReport()", 200);
    } else if ("undefined" == typeof ekt_winprint.document || "unknown" == typeof ekt_winprint.document) {
        setTimeout("delayedPrintReport()", 200);
    } else {
        var printContent = "<table cellspacing=\"0\" align=\"Left\" rules=\"all\" bordercolor=\"White\" border=\"1\" id=\"ReportDataGrid\" style=\"border-color:White;border-style:None;width:100%;border-collapse:collapse;\">";
        printContent += ekt_rpthtml;
        printContent += "</table>";
        while(ekt_winprint.document.body == null)
        {
         setTimeout("delayedPrintReport()", 200);
         return;
        }
        ekt_winprint.document.body.innerHTML = printContent;
        ekt_winprint.document.title = ekt_rpttitle;
        ekt_winprint.focus();
        ekt_winprint.print();
    }
}
var noDataPrintMsg='There is no data to print';
function PrintReport () {
		var bDoPrint = false;
		var rptHtml = "";
		if (document.getElementById("BadLinkReportGrid") != null) {
			bDoPrint = true;
			rptHtml = document.getElementById("BadLinkReportGrid").innerHTML;
		}
		else if (document.getElementById("UpdateActivityTbl") != null)
		{
			bDoPrint = true;
			// getting innerHTML reads back weird MSIE markup
			//rptHtml = document.getElementById("UpdateActivityTbl").innerHTML;
			rptHtml = "<tr><td>" + unescape(document.getElementById("siteRptHtml").value) + "</td></tr>";
			// remove ektronGrid clas so it displays
			rptHtml = rptHtml.replace(/class\=\"ektronGrid\"/gi, '');
		}
		else if (document.getElementById("ReportDataGrid") != null)
		{
			bDoPrint = true;
			rptHtml = document.getElementById("ReportDataGrid").innerHTML;
		}
		
        if (bDoPrint)  {
            var strTemp = "";
            var startPt = -1;
            var endPt = -1;
            var reportTitle = "";
            if(rptHtml == ""){
               alert ('There is no data to print');
               return false;
                }
            while (rptHtml.toLowerCase().indexOf ('</a>') > -1) {
                startPt = rptHtml.toLowerCase().indexOf('<a');
                endPt = rptHtml.substring(startPt).indexOf(">");
                rptHtml = rptHtml.replace(rptHtml.substring(startPt,startPt + endPt+1),'');
                rptHtml = rptHtml.replace("</a>",'');
                rptHtml = rptHtml.replace("</A>",'');
            }

            while (rptHtml.toLowerCase().indexOf('<input') > -1) {
                startPt = rptHtml.toLowerCase().indexOf('<input');
                endPt = rptHtml.substring(startPt).indexOf(">");
                rptHtml = rptHtml.replace(rptHtml.substring(startPt,startPt + endPt+1),'');
            }

            while (rptHtml.toLowerCase().indexOf('<span') > -1) {
                startPt = rptHtml.toLowerCase().indexOf('<span');
                endPt = rptHtml.substring(startPt).indexOf(">");
                rptHtml = rptHtml.replace(rptHtml.substring(startPt,startPt + endPt+1),'');
            }
			rptHtml = rptHtml.replace(/\<\/span\>/gi, '');

            if (document.forms[0].rptTitle != null) {
                reportTitle = document.forms[0].rptTitle.value;
            }
            if (reportTitle == "") {
                reportTitle = document.title;
            }

			if (reportTitle.indexOf("Content Reports:") > -1) {
			    reportTitle = reportTitle.substring(reportTitle.lastIndexOf("class=WorkareaTitlebar"));
				reportTitle = reportTitle.substring(reportTitle.indexOf("Content Reports:")+16)
				reportTitle = reportTitle.substring(0,reportTitle.indexOf("</SPAN>"));
			}
			else {
				if (reportTitle.indexOf("View All") > -1) {
				    reportTitle = reportTitle.substring(reportTitle.lastIndexOf("class=WorkareaTitlebar"));
					reportTitle = reportTitle.substring(reportTitle.indexOf("View All"))
					reportTitle = reportTitle.substring(0,reportTitle.indexOf("</SPAN>"));
				}
			}
                // change to use a blank aspx page instead of a html page to load the js and css at code behind.
                ekt_winprint = window.open('printreport.aspx', '', 'location=0,scrollbars=1,resizable=1');
                ekt_rpthtml = rptHtml;
                ekt_rpttitle = reportTitle;
                delayedPrintReport();
            }
            else {
                if (document.getElementById("viewApprovalList_hdnNeedingApproval").value > 0) {
                    var rptHtml = document.getElementById("viewApprovalList_dgItemsNeedingApproval").innerHTML;
                    var strTemp = "";
                    var startPt = -1;
                    var endPt = -1;
                    var reportTitle = "";

                    while (rptHtml.toLowerCase().indexOf ('</a>') > -1) {
                        startPt = rptHtml.toLowerCase().indexOf('<a');
                        endPt = rptHtml.substring(startPt).indexOf(">");
                        rptHtml = rptHtml.replace(rptHtml.substring(startPt,startPt + endPt+1),'');
                        rptHtml = rptHtml.replace("</a>",'');
                        rptHtml = rptHtml.replace("</A>",'');
                    }

                    while (rptHtml.toLowerCase().indexOf('<input') > -1) {
                                startPt = rptHtml.toLowerCase().indexOf('<input');
                                endPt = rptHtml.substring(startPt).indexOf(">");
                                rptHtml = rptHtml.replace(rptHtml.substring(startPt,startPt + endPt+1),'');
                    }

                    while (rptHtml.toLowerCase().indexOf('<span') > -1) {
                        startPt = rptHtml.toLowerCase().indexOf('<span');
                        endPt = rptHtml.substring(startPt).indexOf(">");
                        rptHtml = rptHtml.replace(rptHtml.substring(startPt,startPt + endPt+1),'');
                    }
    			    rptHtml = rptHtml.replace(/\<\/span\>/gi, '');

                    reportTitle = document.forms[0].title;

		    		if (reportTitle.indexOf("View All") > -1) {
				        reportTitle = reportTitle.substring(reportTitle.lastIndexOf("class=WorkareaTitlebar"));
					    reportTitle = reportTitle.substring(reportTitle.indexOf("View All"))
					    reportTitle = reportTitle.substring(0,reportTitle.indexOf("</SPAN>"));
				    }

                    // change to use a blank aspx page instead of a html page to load the js and css at code behind.
                    ekt_winprint = window.open('printreport.aspx', '', 'location=0,scrollbars=1,resizable=1');
                    ekt_rpthtml = rptHtml;
                    ekt_rpttitle = reportTitle;
                    delayedPrintReport();
                }
                else {
                    alert (noDataPrintMsg);
                }
            }
}


//-->

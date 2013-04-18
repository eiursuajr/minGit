function loadUrlWithDate(display,month,day,year, dtFormat, carryQString_obsolete) {
	// This function accepts "month", "day", "year" format. 
	//
	// It then replaces the date formatting based on what Date format is put in to the function.
	// The format is the standard Miscrosoft Date Description format.
	//
	// MM - Month Digit
	// dd - Day Digit
	// yyyy - Year (4 digits)
	// Add the rest...
	//
	
	month = parseInt(month) ;
	year = parseInt(year) ;
	if(month==0)  { month = 12 ; year -= 1 ; }
	if(month==13) { month = 1 ; year += 1 ; }

	var formattedDt = dtFormat ;
	var reg = /(MM)|(M)/ ;
	formattedDt = formattedDt.replace(reg,month) ;

	reg = /yyyy/ ;
	formattedDt = formattedDt.replace(reg,year) ;

	reg = /(dd)|(d)/ ;
	formattedDt = formattedDt.replace(reg,day) ;
	
	var extendedQString = getCalQueryString();
	var newLoc = document.location.protocol + '//' + document.location.host + document.location.pathname + '?display=' + display + '&sDate='  + formattedDt + extendedQString;
	
	document.location.href = newLoc ;
	return(false) ;
}

function loadUrlWithSingleDate(display,fullDate) {
	// This function takes a "full date" which is the "Short Date" format as outlined in your regional settings.
	var extendedQString = getCalQueryString();
	var newLoc = document.location.protocol + '//' + document.location.host + document.location.pathname + '?display=' + display + '&sDate=' + fullDate + '&eDate=' + fullDate + extendedQString;
	document.location.href = newLoc ;
	return(false) ;
}

function getCalQueryString()
{
	var extendedQString = '';
	var bFoundDisplay = false;
	var argstr = location.search.substring(1, location.search.length);
	var args = argstr.split('&');
	for (var i = 0; i < args.length; i++) 
	{
		var name = args[i].substring(0,args[i].indexOf('=')).toLowerCase();
		if ('display' == name)
		{
			bFoundDisplay = true;
		}
		else if ((name != 'sdate') && (name != 'edate')) 
		{
			extendedQString += '&' + args[i];
		}
	}
	if (!bFoundDisplay)
	{
		extendedQString += '&displaymod=editworkarea';
	}
	return extendedQString
}

function eventDisplay(eventid, fullDate) {
	// This function accepts an event to be highlighted and the date upon which the event
	// happens. Again, uses the Short Date format as outlined in your regional settings.
	// Note: The date and Event ID must match correctly.
	// 

	var argstr = location.search.substring(1, location.search.length) ;
	var args = argstr.split('&') ;
	var extendedQString = '' ;

	for (var i = 0; i < args.length; i++) {
		if((args[i].substring(0,args[i].indexOf('=')).toLowerCase()!='evhighlight')&&(args[i].substring(0,args[i].indexOf('=')).toLowerCase()!='display')&&(args[i].substring(0,args[i].indexOf('=')).toLowerCase()!='sdate')&&(args[i].substring(0,args[i].indexOf('=')).toLowerCase()!='edate')) {
			extendedQString += '&' + args[i] ;
		}
	}

	var newLoc = document.location.protocol + '//' + document.location.host + document.location.pathname + '?display=event&sDate=' + fullDate + '&eDate=' + fullDate + '&evHighlight=' + eventid + extendedQString;

	document.location.href = newLoc ;
	// alert(newLoc) ;
	return(false) ;
}

function showEventTypeSel(s) {
	// where s is a "select" element of type "object"
	//
	// This function accepts a select element. It then loops through all selected
	// event type IDs and shows those eveny type dics (see showEventType for more info)
	//

	// First hide All Events
	hideAllEvents() ;

	// Now loop through the select options	
	for(var x=0;x<s.length;x++) {
		// if the option is selected call showEventType
		if(s[x].selected) {
			showEventType(s[x].value) ;
		}
	}

}

function hideAllEvents() {
	// This function hides all Events. See "showEventType" for more information on naming conventions.
	
	var rq = /^evtype/
	var divArr = document.getElementsByTagName("div")

	if(divArr) {
		for(var x=0;x<divArr.length;x++) {
			if(rq.test(divArr[x].id)) {
				divArr[x].style.display = 'none' ;
			}
		}
	}
}

function showEventType(etid) {
	// This function accepts an event type id and finds all events that use this id.
	// Note: To find the events, the events must be wrappered in a div tag that begins
	//		with the name "evtype", and must have appended to that name each event type id 
	//		to which it is associated. 
	//		For example:
	//			name = evtype_1_2 means that the event has event type ids 1 and 2
	//			name = evtype_3_64 means that the event has event type ids 3 and 64
	//			name = evtype_2 means that the event has event type id 2
	//
	// ** Note: This function does not hide all of the other event types. It only shows them.
	//			See "hideAllEventTypes()" for hiding the events.
	
	var divArr = document.getElementsByTagName("div")

	// Matches all event types
	var rq = /^evtype/
		
	// Passing etid=0 shows all event types, so if it's 0 then set rp to rq
	if(etid!=0) {
		eval('var rp = /^evtype(.)*_' + etid + '(.)*/')
	} else {
		var rp = rq ;
	}
	
	if(divArr) {
		for(var x=0;x<divArr.length;x++) {
			if(rq.test(divArr[x].id)) {
				// Only check those divs whose ids begin with 'evtype' (RegEx rq)
				if(rp.test(divArr[x].id)) {
					divArr[x].style.display = 'block' ;
				}
			}
		}
	}
}

function ecmPopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
	// Function for Ektron Administration Area Pop Up
	var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
	var popupwin = window.open(url, hWind, cToolBar);
	return popupwin;
}

function showEventDetail(id) {
	eval('document.getElementById(\'ev'+id+'\').style.visibility="visible";') ;
}
function hideEventDetail(id) {
	eval('document.getElementById(\'ev'+id+'\').style.visibility="hidden";') ;
}


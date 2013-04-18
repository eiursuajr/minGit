var curSelectTd ;
var selectTdHold ;
var canGetElemById = true;
if ("undefined" == typeof document.getElementById) //nn6 ie5
{
	canGetElemById = false;
}

function initTargetDate() {
	if(document.selectorForm.timeSeperator) {
		var tSep = document.selectorForm.timeSeperator.value ;
	}
	if(document.selectorForm.serverdateseperator) {
		var dSep = document.selectorForm.serverdateseperator.value ;
	}
	var tDt = targetdate ;

	if(tDt.indexOf(' ',0) > 0) {
		// We have a time, so remove it.
		tDt = tDt.substring(0,tDt.indexOf(' ',0)) ;
	}
	
	if (dSep == ".")
	{
		dSep = "\\" + dSep;
	}
	
	var regEx = new RegExp(dSep, "gi");
	tDt = tDt.replace(regEx,'_') ;

	if (canGetElemById) 
	{
		curSelectTd = eval('document.getElementById(\'td_' + tDt + '\')') ;
		if(curSelectTd) {
			selectTdHold = curSelectTd.className ;
			curSelectTd.className = 'mup_selectedDate' ;
			if ("function" == typeof curSelectTd.onclick)
			{
				curSelectTd.onclick();
			}
		}
	}

	document.selectorForm.newDateTime.value = targetdate ;
	if (canGetElemById) 
	{
		var curSelectDiv = eval('document.getElementById(\'div_' + tDt + '\')') ;
	
		if(curSelectDiv) {
			document.selectorForm.newDisplayDateTime.value = curSelectDiv.innerHTML ;
		}
	}
	// now set the time, if we need to. Use the TimeSeperator as the litmus
	if(document.selectorForm.timeSeperator) {
		tDt = targetdate ;
		if(tDt.indexOf(tSep,0)>0) {

			if(tDt.indexOf(document.selectorForm.serverdateseperator.value)>0) {
				tDt = tDt.substring(tDt.indexOf(' ',0), tDt.length) ;
			}

			var nextIndex = tDt.indexOf(tSep,0) ;
			var hr = parseInt(tDt.substring(0,nextIndex), 10) ;
			var curIndex = nextIndex + 1;
			nextIndex = tDt.indexOf(tSep,curIndex) ;

			if(nextIndex<0) {
				nextIndex = tDt.indexOf(' ',curIndex+1) ;
				if(nextIndex<0) {
					nextIndex = tDt.length ;
				}
			}
			var min = parseInt(tDt.substring(curIndex,nextIndex), 10) ;
			var merid = tDt.substring(tDt.lastIndexOf(' ')+1,tDt.length) ;

			if(merid.indexOf(tSep)<0) {
				// We have a Meridian on the server...
				if(document.selectorForm.amdesignator.value!='') {
					// No need to convert to 24Hr. set hour, minute, meridian
					document.selectorForm.hrSelect.value = hr ;
					if(merid.replace(/ /gi,'')==document.selectorForm.serveramdesignator.value.replace(/ /gi,'')) {
						document.selectorForm.meridSelect.value = "a"
						document.selectorForm.newDisplayDateTime.value += ' ' + hr + tSep + min + ' ' + document.selectorForm.amdesignator.value ;
					} else {
						document.selectorForm.meridSelect.value = "p"
						document.selectorForm.newDisplayDateTime.value += ' ' + hr + tSep + min + ' ' + document.selectorForm.pmdesignator.value ;
					}
				} else {
					// Convert 12hr. to 24Hr. and set Hour and Minute
					if(merid.replace(/ /gi,'')==document.selectorForm.serveramdesignator.value.replace(/ /gi,'')) {
						// in the anti-meridian, set the hour
						document.selectorForm.hrSelect.value = hr ;
						document.selectorForm.newDisplayDateTime.value += ' ' + hr + tSep + min ;
					} else {
						// in the post-meridian, set the hour, unless it's 12
						if(hr!=12) {
							document.selectorForm.hrSelect.value = hr + 12 ;
							document.selectorForm.newDisplayDateTime.value += ' ' + (hr + 12) + tSep + min ;
						} else {
							document.selectorForm.hrSelect.value = 12 ;
							document.selectorForm.newDisplayDateTime.value += ' 12' + tSep + min ;
						}
					}
				}
				document.selectorForm.miSelect.value = min ;
			} else {
				// We do not have a Meridian on the Server... 			
				// Check if have a meridian on the display
				var mer ;
				if(document.selectorForm.meridSelect) {
					// Meridian on Display, convert 24 Hr. to 12 Hr. and set
					if(hr>11) {
						if(hr==12) {
							document.selectorForm.meridSelect.value='a' ;
							mer = document.selectorForm.serveramdesignator.value ;
						} else {
							hr -= 12 ;
							document.selectorForm.meridSelect.value='p' ;
							mer = document.selectorForm.serverpmdesignator.value ;
						}
					} else {
						document.selectorForm.meridSelect.value='a' ;
						mer = document.selectorForm.serveramdesignator.value ;
						if(hr==0) {
							hr = 12 ;
						}
					}

					document.selectorForm.hrSelect.value = hr ;
					document.selectorForm.miSelect.value = min ;
					min = min.toString() ;
					hr = hr.toString() ;
					if(min.length<2) {
						min = '0' + min ;
					}
					if(hr.length<2) {
						hr = '0' + hr ;
					}
					document.selectorForm.newDisplayDateTime.value += ' ' + hr + tSep + min + ' ' + mer ;
				} else {
					// No meridian on display, no problem, set hours and minutes
					document.selectorForm.hrSelect.value = hr ;
					document.selectorForm.miSelect.value = min ;
					document.selectorForm.newDisplayDateTime.value += ' ' + hr + tSep + min ;
				}
			}
			if (document.selectorForm.hrSelect && "function" == typeof document.selectorForm.hrSelect.onchange)
			{
				document.selectorForm.hrSelect.onchange();
			}
		}
	}

}

function setSelectedDate(inServerDate) {
	if(document.selectorForm.timeSeperator) {
		var tSep = document.selectorForm.timeSeperator.value ;
	}
	if(document.selectorForm.serverdateseperator) {
		var dSep = document.selectorForm.serverdateseperator.value ;
	}
	var tDt = inServerDate ;

	if(tDt.indexOf(' ',0) > 0) {
		// We have a time, so remove it.
		tDt = tDt.substring(0,tDt.indexOf(' ',0)) ;
	}
	if (dSep == ".")
	{
		dSep = "\\" + dSep;
	}
	var regEx = new RegExp(dSep, "gi");
	tDt = tDt.replace(regEx,'_') ;

	
	if(selectTdHold) {
		curSelectTd.className = selectTdHold ;
	}
	
	if (canGetElemById) 
	{
		curSelectTd = eval('document.getElementById(\'td_' + tDt + '\')') ;
		if (curSelectTd != null)
		{
			selectTdHold = curSelectTd.className ;
			curSelectTd.className = 'mup_selectedDate' ;
		}
	}
}

function updateParentDate(serverDate, displayDate, n_dow, n_dom, n_monum, n_yrnum) {

	if((n_dow)&&(n_dom)&&(n_monum)&&(n_yrnum)) {
		document.selectorForm.new_dow.value = n_dow ;
		document.selectorForm.new_dom.value = n_dom ;
		document.selectorForm.new_monum.value = n_monum ;
		document.selectorForm.new_yrnum.value = n_yrnum ;
	}

	document.selectorForm.newDateTime.value = serverDate ;
	document.selectorForm.newDisplayDateTime.value = displayDate ;
	setSelectedDate(serverDate) ;
}

function updateParentDateWithTime(serverDate, displayDate, n_dow, n_dom, n_monum, n_yrnum) {
	var f = document.selectorForm ;
	var fE = f.newDateTime ;

	if((n_dow)&&(n_dom)&&(n_monum)&&(n_yrnum)) {
		document.selectorForm.new_dow.value = n_dow ;
		document.selectorForm.new_dom.value = n_dom ;
		document.selectorForm.new_monum.value = n_monum ;
		document.selectorForm.new_yrnum.value = n_yrnum ;
	}

	if(serverDate!=0) {
		fE.value = serverDate ;
	} else {
		//var x = fE.value.indexOf(' ',0) ;
		serverDate = fE.value.substring(0,fE.value.indexOf(' ',0)) ;
		fE.value = serverDate ;
	}
	// Now add the time in
	if(f.hrSelect) {
		var hrSelect = f.hrSelect.value ;
		var miSelect = f.miSelect.value ;

		if(f.meridSelect) {
			if(f.meridSelect.value=='p') {
				if(hrSelect!='12') {
					hrSelect = parseInt(hrSelect, 10) ;
					hrSelect += 12 ;
					hrSelect = hrSelect.toString();
				}
			}

			if((f.meridSelect.value=='a')&&(hrSelect=='12')) {
				hrSelect = '0' ;
			}
		}

		if(hrSelect.length<2) {
			hrSelect = '0' + hrSelect;
		}
		if(miSelect.length<2) {
			miSelect = '0' + miSelect;
		}

		fE.value += ' ' + hrSelect + f.timeSeperator.value + miSelect ;
		if(document.selectorForm.serverdateseperator) {
			var dSep = document.selectorForm.serverdateseperator.value ;
		}
		if (dSep == ".")
		{
			dSep = "\\" + dSep;
		}
		var regEx = new RegExp(dSep, "gi");
		var tDate = serverDate.replace(regEx,'_');
		if (eval('document.selectorForm.z' + tDate))
		{
			eval('f.newDisplayDateTime.value = document.selectorForm.z' + tDate + '.value ;') ;
		}

		hrSelect = f.hrSelect.value ;
		if(hrSelect.length<2) {
			hrSelect = '0' + hrSelect;
		}
		
		f.newDisplayDateTime.value += ' ' + hrSelect + f.localTimeSeperator.value + miSelect ;

		if(f.meridSelect) {
			f.newDisplayDateTime.value += ' ' + f.meridSelect[f.meridSelect.selectedIndex].innerHTML ;
		}
	}
	setSelectedDate(fE.value) ;
}
function getHour()
{
	var f = document.selectorForm ;
	var fE = f.newDateTime ;
	var hrSelect = f.hrSelect.value ;
	if(f.meridSelect) {
		if(f.meridSelect.value=='p') {
			if(hrSelect!='12') {
				hrSelect = parseInt(hrSelect, 10) ;
				hrSelect += 12 ;
				hrSelect = hrSelect.toString();
			}
		}

		if((f.meridSelect.value=='a')&&(hrSelect=='12')) {
			hrSelect = '0' ;
		}
	}

	if(hrSelect.length<2) {
		hrSelect = '0' + hrSelect;
	}
	return hrSelect;
		
}
function getMin()
{
	var f = document.selectorForm ;
	var miSelect = f.miSelect.value ;
	if(miSelect.length<2) {
		miSelect = '0' + miSelect;
	}
	return miSelect;
}
function updateTime() {
		var f = document.selectorForm ;
		var fE = f.newDateTime ;
		var hrSelect = f.hrSelect.value ;
		var miSelect = f.miSelect.value ;

		if(f.meridSelect) {
			if(f.meridSelect.value=='p') {
				if(hrSelect!='12') {
					hrSelect = parseInt(hrSelect, 10) ;
					hrSelect += 12 ;
					hrSelect = hrSelect.toString();
				}
			}

			if((f.meridSelect.value=='a')&&(hrSelect=='12')) {
				hrSelect = '0' ;
			}
		}

		if(hrSelect.length<2) {
			hrSelect = '0' + hrSelect;
		}
		if(miSelect.length<2) {
			miSelect = '0' + miSelect;
		}

		fE.value = hrSelect + f.timeSeperator.value + miSelect ;

		// eval('f.newDisplayDateTime.value = document.selectorForm.z' + serverDate.replace(/\//gi,'_') + '.value ;') ;

		hrSelect = f.hrSelect.value ;
		if(hrSelect.length<2) {
			hrSelect = '0' + hrSelect;
		}
		
		f.newDisplayDateTime.value = ' ' + hrSelect + f.localTimeSeperator.value + miSelect ;

		if(f.meridSelect) {
			f.newDisplayDateTime.value += ' ' + f.meridSelect[f.meridSelect.selectedIndex].innerHTML ;
		}
}

function EkDTValidate(spanTag, objWindow)
{
    if ("undefined" == typeof objWindow) 
    {
		objWindow = window.opener;
	}
    if (objWindow && !objWindow.closed)
    {
		// IE may limit access to objects in 'opener' window, so use unconventional means
	    if (typeof objWindow.Ektron != "undefined" && typeof objWindow.Ektron.SmartForm != "undefined" && typeof objWindow.Ektron.SmartForm.validateElement != "undefined") 
	    {
		    objWindow.Ektron.SmartForm.validateElement(spanTag);	
	    }
	    else if ("function" == typeof objWindow.design_prevalidateElement) 
	    {
		    objWindow.design_prevalidateElement(spanTag, null);	
	    }
	}
}

function doneClick() 
{
	var objWindow = window.opener;
	if (objWindow && !objWindow.closed)
	{
	    if (formname == "")
	    {
	        formname = "0";
	    }
		var oTargetForm = objWindow.document.forms[formname];
		if (!oTargetForm) return;
		var oSelectorForm = document.selectorForm;
		if (!oSelectorForm) return;
		var sDateISO = formatDateISO8601(oSelectorForm.new_yrnum.value, oSelectorForm.new_monum.value, oSelectorForm.new_dom.value); 
		
		var hiddenElem = oTargetForm.elements[formelement];
		hiddenElem.value = oSelectorForm.newDateTime.value;
		
		var hiddenCMSElem = oTargetForm.elements[formelement + '_iso'];
		if (hiddenCMSElem) 
		{
			hiddenCMSElem.value = sDateISO;
		}
		
		if (canGetElemById) 
		{
			var spanTag = objWindow.document.getElementById(spanid);
			// ISO 8601 format: CCYY-MM-DD
			if ('0000-0-0'==sDateISO)
			{ 
				var strLocation=window.location.href;			
				if (strLocation.indexOf('type=time')<0) 
				{
					alert("No date has been selected on the displayed calendar. \n  Please select a date.");
					return false;
				}				
			}
			spanTag.datavalue = sDateISO;
			spanTag.setAttribute("datavalue", sDateISO);
			spanTag.value = sDateISO;
			spanTag.setAttribute("value", sDateISO);
			spanTag.innerHTML = oSelectorForm.newDisplayDateTime.value;	//'Tuesday, October 09, 2009 09:40 PM';//
			
			EkDTValidate(spanTag, objWindow);
		}
		
		// Extended Mode
		var dowElem = oTargetForm.elements[formelement + '_dow'];
		var domElem = oTargetForm.elements[formelement + '_dom'];
		var monumElem = oTargetForm.elements[formelement + '_monum'];
		var yrnumElem = oTargetForm.elements[formelement + '_yrnum'];
		var hourElem = oTargetForm.elements[formelement + '_hr'];
		var minElem = oTargetForm.elements[formelement + '_mi'];

		if ((dowElem) && (domElem) && (monumElem) && (yrnumElem)) 
		{
			dowElem.value = oSelectorForm.new_dow.value;
			domElem.value = oSelectorForm.new_dom.value;
			monumElem.value = oSelectorForm.new_monum.value;
			yrnumElem.value = oSelectorForm.new_yrnum.value;
		}
		if ((hourElem) && (minElem))
		{
			hourElem.value = getHour();
			minElem.value = getMin();
		}
		if ("function" == typeof objWindow.dateUpdatedEvent || "object" == typeof objWindow.dateUpdatedEvent) 
		{
			// If a developer wants a specific action to occur after a date is 
			// updated from the selector she will have to create a function on 
			// the parent page with this name (dateUpdatedEvent). The parameter
			// is for the name of the element being updated.
			objWindow.dateUpdatedEvent();
		}
	}
	window.close();
}

function cancelClick() {
	window.close() ;
}

function formatDateISO8601(year, month, day)
{
	var y = year + "";
	var m = month + "";
	var d = day + "";
	while (y.length < 4) y = "0" + y;
	if (m.length < 2) m = "0" + m;
	if (d.length < 2) d = "0" + d;
	return y + "-" + m + "-" + d;
}

function openDTselector(dType, targetDate, spanId, formName, formElement, cmsAppPath) {
	var winProps = 'toolbar=0, status=0, resizable=1' ;
	if(dType=='time') {
		winProps += ', height=150, width=250' ;
	} else {
		winProps += ', height=350, width=300' ;
	}
	if (targetDate == null)
	{
		targetDate = "";
	}
	window.open(cmsAppPath + 'calendarAdmin/datetimeselector.aspx?type=' + dType + '&targetdate=' + targetDate 
			+ '&spanid=' + spanId + '&formname=' + formName + '&formelement=' + formElement + '&sdate=' + targetDate,'dtSelWin',
			winProps);
}

function clearDTvalue(targetDateElem, spanId, noDateMessage) {
	targetDateElem.value = "";
	if (canGetElemById) 
	{
		var hiddenCMSElem = document.getElementById(targetDateElem.id + '_iso');
		if (hiddenCMSElem) 
		{
			hiddenCMSElem.value = "";
		}

		var spanTag = document.getElementById(spanId);
		spanTag.datavalue = "";
		spanTag.setAttribute("datavalue", "");
		spanTag.value = "";
		spanTag.setAttribute("value", "");
		spanTag.innerHTML = noDateMessage;
		EkDTValidate(spanTag, window);
	}
}

function EkDTCompareDates(objLowDateField, objHighDateField)
{

	if ((objLowDateField == null) || (objHighDateField == null))
	{							
		return true;
	}
	if ((objLowDateField.value == null) || (objLowDateField.value == "")
			|| (objHighDateField.value == null) || (objHighDateField.value == ""))
	{
		return true;
	}
	
	var name = objLowDateField.name;
	var _minDay = eval("document.forms[0]." + name + "_dom.value;");
	var _minMonth = eval("document.forms[0]." + name + "_monum.value;");
	if (_minMonth)
	{
		_minMonth = parseInt(_minMonth, 10) - 1;
	}
	var _minYear = eval("document.forms[0]." + name + "_yrnum.value;");
	var _minHr = eval("document.forms[0]." + name + "_hr;");
	var _minMi = eval("document.forms[0]." + name + "_mi;");
	
	var name = objHighDateField.name;
	var _maxDay = eval("document.forms[0]." + name + "_dom.value;");
	var _maxMonth = eval("document.forms[0]." + name + "_monum.value;");
	if (_maxMonth)
	{
		_maxMonth = parseInt(_maxMonth, 10) - 1;
	}
	var _maxYear = eval("document.forms[0]." + name + "_yrnum.value;");
	var _maxHr = eval("document.forms[0]." + name + "_hr;");
	var _maxMi = eval("document.forms[0]." + name + "_mi;");
	if (_minHr == null && _minHr != "undefined") _minHr = "";
	if (_maxHr == null && _maxHr != "undefined") _maxHr = "";
	
	if (_minHr != "")
	{
		var myLowerDate=new Date(_minYear, _minMonth, _minDay, _minHr.value, _minMi.value);
	}
	else
	{
		//var myLowerDate=new Date(_minYear, _minMonth, _minDay);
		var myLowerDate=new Date(_minYear, _minMonth, _minDay,0,0);
	}
	
	if (_maxHr != "")
	{
		var myHighDate=new Date(_maxYear, _maxMonth, _maxDay, _maxHr.value, _maxMi.value);	
	}
	else
	{
		//var myHighDate=new Date(_maxYear, _maxMonth, _maxDay);
		var myHighDate=new Date(_maxYear, _maxMonth, _maxDay,23,59);
	}
	
	if (myLowerDate >= myHighDate)
	{
		return false;
	}
	return true;
							
}
function ConvertEkDateToDate(objDateField)
{

	if (objDateField == null)
	{							
		return null;
	}
	if ((objDateField.value == null) || (objDateField.value == ""))
	{
		return null;
	}
	
	var name = objDateField.name;
	var _minDay;
	if (document.getElementById(name + "_dom") != null) 
	    _minDay = document.getElementById(name + "_dom").value;
	var _minMonth;
	if (document.getElementById(name + "_monum") != null)
	    _minMonth= document.getElementById(name + "_monum").value;
	if (_minMonth)
	{
		_minMonth = parseInt(_minMonth, 10) - 1;
	}
	var _minYear;
	if (document.getElementById(name + "_yrnum") != null)
	    _minYear = document.getElementById(name + "_yrnum").value;;
	var _minHr = document.getElementById(name + "_hr");
	var _minMi = document.getElementById(name + "_mi");
	if (_minHr == null && _minHr != "undefined") _minHr = "";
	
	var myDate = null;
	if (_minHr != "")
	{
		myDate=new Date(_minYear, _minMonth, _minDay, _minHr.value, _minMi.value);
	}
	else
	{
		myDate=new Date(_minYear, _minMonth, _minDay,0,0);
	}
    return myDate;
}
//usage: getFormElement(this).elementid.value
function getFormElement(objStartElem)
{
    var form = objStartElem;
    while ( form.tagName.toLowerCase() != "body" )
    {
        if ( form.tagName.toLowerCase() == "form" )
        {
            break;
        }
        form = form.parentNode;        
    }
    //make sure it is the form element
    if ( form.tagName.toLowerCase() != "form" )
    {
        form = null;
    }    
    return form;
}

function obtainFormID(thisform)
{
    if (thisform.attributes == null || thisform.attributes['id'] == null || thisform.attributes['id'].nodeValue == null)
    {
        return thisform.id;
    }
    else
    {
        return thisform.attributes['id'].nodeValue;
    }
}
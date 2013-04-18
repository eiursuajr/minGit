
var g_AdvancedGroupSelected = '' ;
var g_AdvancedOptionsEnabled = '' ;
var g_enableCmsConstraints = '' ;
var g_enableAssetConstraints = '' ;
var g_enableFormReInitializing = '' ;
	
function OpenCalendarForElement(id, id_data) {
	var ctrlObj;
	ctrlObj=document.getElementById(id);
	if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase())) {
		ctrlObj.value = Trim(ctrlObj.value);
		//CallCalendarForElement(ctrlObj.value, '<%=AppPath%>calendar.aspx', id, id_data, ctrlObj.form.name);
	}
}

function CallCalendarForElement(date, pagename, ctrl, hctrl, formname){	
	var regexp1 = / /gi;
	var template;
	var paremeter;
	var Url;
	var items = pagename.split('?');
	if (items.length == 2 ) {
		template = items[0].toLowerCase();
		paremeter = items[1].toLowerCase();
	}
	else{
		template = pagename.toLowerCase();
		paremeter= '';
	}
	if (date.replace(regexp1, '') == '')
	{
		Url = template + '?form=' + formname + '&ctrl=' + ctrl + '&hctrl=' + hctrl;
		if (paremeter.length){
			Url = Url + '&' + paremeter;
		} 
		PopUpWindow (Url, 'Calendar', 325, 280, 0, 1);
	}
	else {
		//if (CheckDate(date)) {
			date = RemoveLeadingSpaces(date);
			var date_times = date.split(' ');
			if (date_times.length < 2) {
				var times = '';
				var hours = '';
			}
			else {
				var times = date_times[1].split(':');
				var hours = times[0].replace(regexp1, '');
			}
			if (times.length < 2) {
				var minutes = '';
			}
			else {
				var minutes = times[1].replace(regexp1, '');
			}
			if (times.length < 3) {
				var seconds = '';
			}
			else {
				var seconds = times[2].replace(regexp1, '');
			}
			if (date_times.length < 2) {
				var ampm = '';
			}
			else {
				var ampm = date_times[2].replace(regexp1, '');
			}
			Url = template + '?form=' + formname + '&ctrl=' + ctrl + '&hctrl=' + hctrl + '&startdate=' + date_times[0] + '&showdate=1';
										+ '&hours=' + hours + '&minutes=' + minutes + '&seconds=' + seconds + '&tt=' + ampm;
			if (paremeter.length){
				Url = Url + '&' + paremeter;
			}
											
			PopUpWindow (Url,'Calendar', 325, 280, 0, 1);
		//}
	}
}	

function UpdateTextCounter(id, maxlimit) {
	var objText = document.getElementById(id);
	var objCount = document.getElementById(id + "_len");
	if (objText && objCount)
	{
		var strText = GetElementValue(objText) + "";
		var len = strText.length;
		if (len > maxlimit) {
			SetElementValue(objText, strText.substring(0, maxlimit));
			objCount.style.color = "red";
			objCount.style.fontWeight = "bold";
		}
		else
		{
			objCount.style.color = "";
			objCount.style.fontWeight = "";
		}
		SetElementValue(objCount, len);
	}
}
		
function SetDefault(id) {
	var objElem = document.getElementById(id);
	if (!objElem) return;
	var objDefault = document.getElementById(id + "default");
	if (!objDefault) return;
	// if (confirm("<%= CmsMsg("js: confirm restore default text") %>"))
	if(confirm("Are you sure that you want to restore the default text?"))
	{
		objElem.value = objDefault.value;
		if ("function" == typeof objElem.onchange)
		{
			try
			{
				objElem.onchange();
			}
			catch (e)
			{
				// just in case something goes wrong
			}
		}
	}
}

function booleanSelected(selfield, textfield, falsevalue, truevalue) {
	textfield.value = (selfield.checked ? truevalue : falsevalue);
}

function outputSelected(selfield,textfield,seperator) {
	var retValue;
	var sel = getSelected(selfield);
    var strSel = "";
   	for (var item in sel) {       
		strSel += sel[item].value + seperator + " ";
    }
    strSel = strSel.substring(0, strSel.length-2);
    var ch = strSel.substring(0, 1);
    if (ch == seperator) {
		strSel = strSel.substring(1, strSel.length);
    }
    if(document.forms[0])
    {
		document.forms[0].elements[textfield].value = strSel;
	}
}
				
function getSelected(opt) {
   var selected = new Array();
   var index = 0;
   for (var intLoop = 0; intLoop < opt.length; intLoop++) {
      if ((opt[intLoop].selected) ||
          (opt[intLoop].checked)) {
         index = selected.length;
         selected[index] = new Object;
         selected[index].value = opt[intLoop].value;
         selected[index].index = intLoop;
      }
   }
   return selected;
}

/******************************************************************************************************/

	function convertUnicodeToCharRef(strContent) {		
		return strContent;
		//var strConverted = "";
		//var lenContent = strContent.length;
		//for (var i = 0; i < lenContent; i++) {
		//	var chrCode = strContent.charCodeAt(i);
		//	if (chrCode < 150) { strConverted += strContent.charAt(i); }
		//	else { strConverted += "&#" + chrCode + ";"; }
		//}
		//return strConverted;
	}
	function processContent(){
	    if(document.forms[0].isPostData.value !=""){
		var strTempText;
		var strSearchText;
		if (g_AdvancedGroupSelected) {
			strSearchText = window.document.ecmSearchAllAssets.ecmKeywords.value;
		} else {
			strSearchText = window.document.ecmSearchAllAssets.ecmBasicKeywords.value;
		}
		strTempText = convertUnicodeToCharRef(strSearchText);
		window.document.ecmSearchAllAssets.ecmsearchtext.value=strTempText;
		return false;}else{return true;}
	}
	function showAdvancedGroupPanel(flag){
		g_AdvancedGroupSelected = flag;
		if (flag) {
			ShowElement("basic_group_panel", false);
			ShowElement("advanced_group_panel", true);
			SetElementValue('ecmsearchmode', 'advanced');
		} else {
			ShowElement("advanced_group_panel", false);
			ShowElement("basic_group_panel", true);
			SetElementValue('ecmsearchmode', 'basic');
		}
	}
	
	function showAdvancedOptionsPanel(flag){
		g_AdvancedOptionsEnabled = flag;
		if (flag) {
			ShowElement("advanced_options_panel_link", false);
			ShowElement("advanced_options_panel", true);
		} else {
			ShowElement("advanced_options_panel", false);
			ShowElement("advanced_options_panel_link", true);
		}
	}
	function CheckboxSelectorClicked(ctrlObj) {
		var secondObj;
		switch (ctrlObj.name) {
			/*
			case 'ecmCheckboxDocTypeSelectorIncludeContent' :
				secondObj = document.getElementById("ecmCheckboxDocTypeSelectorIncludeForms");
				ShowElement("cms_group", (ctrlObj.checked && g_enableCmsConstraints) || (secondObj.checked && g_enableCmsConstraints));
				break;
			case 'ecmCheckboxDocTypeSelectorIncludeForms' :
				secondObj = document.getElementById("ecmCheckboxDocTypeSelectorIncludeContent");
				ShowElement("cms_group", (ctrlObj.checked && g_enableCmsConstraints) || (secondObj.checked && g_enableCmsConstraints));
				break;
			*/
			case 'ecmCheckboxDocTypeSelectorIncludeAssets' :
				ShowElement("dms_group", (ctrlObj.checked && g_enableAssetConstraints));
				break;
		}
	}
	function toggleMultSelView(ctrlName, btnObj, minSize, maxSize) {
		var ctrlObj;
		ctrlObj = document.getElementById(ctrlName);
		if ((null != ctrlObj) && (typeof(ctrlObj) == 'object')) {
		   	if(ctrlObj.size == minSize) {
	   			ctrlObj.size = maxSize;
				btnObj.value=' ^ ';
		   	} else {
		   		ctrlObj.size = minSize;
				btnObj.value=' v ';
		   	}
		}
		return false;
	}
	function DisableElement(elmName, flag){ // TODO
		var ctrlObj;
		ctrlObj=document.getElementById(elmName);
		if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase()) && ('object' == typeof(ctrlObj.attributes)) && ('object' == typeof(ctrlObj.attributes("disabled")))) {
			ctrlObj.attributes("disabled").nodeValue = flag;
		}
	}
	function ShowElement(elmName, flag){ // TODO
		var ctrlObj;
		ctrlObj=document.getElementById(elmName);
		if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase())) {
			if (flag) {
				ctrlObj.style.display = "";
			} else {
				ctrlObj.style.display = "none";
			}
			ShowSelectElements(ctrlObj, flag);	
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

	function SetElementVisible(elmName, flag){
		var ctrlObj;
		ctrlObj=document.getElementById(elmName);
		if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase())) {
			if (flag) {
				ctrlObj.style.visibility = "visible";
			} else {
				ctrlObj.style.visibility = "hidden";
			}
		}
	}
	function SetElementClass(elmName, className){
		var ctrlObj;
		ctrlObj=document.getElementById(elmName);
		if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase())) {
			ctrlObj.className = className;
		}
	}
	function HandleTextLogicChange(ctrlObj, textElemId){
		var targCtrlObj;
		if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase())) {
			SetElementVisible(textElemId, ('' != (ctrlObj.value).toLowerCase()));
		}
	}
	function HandleDateLogicChange(ctrlObj, minTxtElemId, minImgElemId, maxTxtElemId, maxImgElemId){
		var targCtrlObj;
		if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase())) {
			SetElementVisible(minTxtElemId, ('' != (ctrlObj.value).toLowerCase()));
			SetElementVisible(minImgElemId, ('' != (ctrlObj.value).toLowerCase()));
			SetElementVisible(maxTxtElemId, ('lessthanorgreaterthanequals' == (ctrlObj.value).toLowerCase()));
			SetElementVisible(maxImgElemId, ('lessthanorgreaterthanequals' == (ctrlObj.value).toLowerCase()));
		}
	}
	function HandleRangeLogicChange(ctrlObj, minTxtElemId, maxTxtElemId){
		var targCtrlObj;
		if ((null != ctrlObj) && ('object' == (typeof(ctrlObj)).toLowerCase())) {
			SetElementVisible(minTxtElemId, ('' != (ctrlObj.value).toLowerCase()));
			SetElementVisible(maxTxtElemId, ('lessthanorgreaterthanequals' == (ctrlObj.value).toLowerCase()));
		}
	}

	function Trim (string) { // TODO
		if (string.length > 0) {
			string = RemoveLeadingSpaces (string);
		}
		if (string.length > 0) {
			string = RemoveTrailingSpaces(string);
		}
		return string;
	}		
	function RemoveLeadingSpaces(string) {
		while(string.substring(0, 1) == ' ') {
			string = string.substring(1, string.length);
		}
		return string;
	}
	function RemoveTrailingSpaces(string) {
		while(string.substring((string.length - 1), string.length) == ' ') {
			string = string.substring(0, (string.length - 1));
		}
		return string;
	}
	function ReInitializeForm(){
		var oCrl, aryElems, oElem;
		var aryTagNames = ['input', 'select', 'textarea'];
		if (!g_enableFormReInitializing) {
			return;
		}
		// tickle the on-change-event for all input controls:
		oCrl=document.getElementById('ecmSearchAllAssets');
		if ((null != oCrl) && ('object' == (typeof (oCrl)).toLowerCase()) && ("undefined" != typeof(oCrl.readyState)) && ('complete' == (oCrl.readyState).toLowerCase())) {
			for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++){
				aryElems = oCrl.getElementsByTagName(aryTagNames[iTagName]);
				for (var i = 0; i < aryElems.length; i++){
					try {
						oElem = aryElems[i];
						if ('checkbox' == oElem.type) {
							oElem.onclick();
						} else {
							oElem.onchange();
						}
					}
					catch(e) {
						// eat the error silently...
					}
				}
			}
		} else {
		setTimeout('ReInitializeForm()', 10);
		}
	}

//CALLBACK SUPPORT ADDED BY UDAI ON 01/17/06
var __ecmDisplayResult="__EkDisplayResultControl";
	
function ICallBackHandler(){
}
ICallBackHandler.ValidateKey=function(item){
    if (item.keyCode==13){
        __LoadSearchResult(ICallBackHandler.getArguements(),'__ecmcurrentpage=1');
        return false;
    }
}
ICallBackHandler.DisplaySearchError=function(message, context) {
    alert('An unhandled exception has occurred:\n' + message);
}
ICallBackHandler.DisplaySearchResult=function(result, context) {
    document.getElementById(__ecmDisplayResult).innerHTML=result;
}
ICallBackHandler.getArguements=function(){
    document.getElementById(__ecmDisplayResult).innerHTML="loading...";
    return(ICallBackHandler.serializeForm());
}
ICallBackHandler.serializeForm = function() {
    for(var form=0;form<document.forms.length;form++){
        var element = document.forms[form].elements;
        var len = element.length;
        var query_string = "";
        this.AddSearchField = 
        function(name,value) { 
            if (query_string.length>0) { 
            query_string += "&";
            }
            query_string += encodeURIComponent(name) + "=" + encodeURIComponent(value);
        };
        for (var i=0; i<len; i++) {
            var item = element[i];
            try{
                if (!item.disabled && item.name.indexOf('ecm')!=-1) {
                    switch(item.type) {
                        case 'text': case 'password': case 'hidden': case 'textarea': 
                            this.AddSearchField(item.name,item.value);
                            break;
                        case 'select-one':
                            if (item.selectedIndex>=0) {
                                this.AddSearchField(item.name,item.options[item.selectedIndex].value);
                            }
                            break;
                        case 'select-multiple':
                            for (var j=0; j<item.options.length; j++) {
                                if (item.options[j].selected) {
                                    this.AddSearchField(item.name,item.options[j].value);
                                }
                            }
                            break;
                        case 'checkbox': case 'radio':
                            if (item.checked) {
                                this.AddSearchField(item.name,item.value);
                            }
                            break;
                    }
                }
            }
            catch(e)
            {
            }
        }
        if(query_string.length>0){
            break;
        }
    }
    return query_string;
};


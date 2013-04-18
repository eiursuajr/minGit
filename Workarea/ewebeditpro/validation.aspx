<%@ Page Language="C#" AutoEventWireup="true" Inherits="validation" ValidateRequest="false"
    CodeFile="validation.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head runat="server">
    <title>validation</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />

    <script language="JavaScript1.2" type="text/javascript" src="../java/jfunct.js">
    </script>

    <script language="JavaScript1.2" type="text/javascript" src="eweputil.js">
    </script>

    <asp:Literal ID="CloseWindow" runat="server" />

    <script language="javascript" type="text/javascript">
		function trim(s) {
		  while (s.substring(0,1) == ' ') {
    		s = s.substring(1,s.length);
  		  }
    	  while (s.substring(s.length-1,s.length) == ' ') {
    		s = s.substring(0,s.length-1);
  		  }
  		  return s;
		}
		function CheckKeyValue(item, keys) {
			var keyArray = keys.split(",");
			for (var i = 0; i < keyArray.length; i++) {
				if ((document.layers) || ((!document.all) && (document.getElementById))) {
					if (item.which == keyArray[i]) {
						return false;
					}
				}
				else {
					if (event.keyCode == keyArray[i]) {
						return false;
					}
				}
			}
		}
		function doSubmit(){
				if(!eWebEditProUtil.isOpenerAvailable()){
					alert('Unable to save changes. The editor page has been closed.');
					self.close();
				}
				var myindex, idx

				 myindex = document.forms[0].selType.selectedIndex;
				if (myindex != 0 &&  trim(document.forms[0].txtErrorMsg.value) == '' ) {
					alert('Enter the Error Message')
				}
				else {

					if (document.forms[0].txtErrorMsg.value.match("'") == "'") {
						alert("Invalid Char(\" ' \") in the error message ");
						return false;
					}
					if ( (myindex == 9) || (myindex == 2) ) {
						if ( trim(document.forms[0].txtMin.value) != '') {
							if (!(isNumeric(document.forms[0].txtMin.value))) {
								alert('The min value must be a number');
								return;
							}
						}
						if ( trim(document.forms[0].txtMax.value) != '') {
							if (!(isNumeric(document.forms[0].txtMax.value))) {
								alert('The Max value must be a number');
								return;
							}
						}
					}

					if (myindex == 4) {
						if ( trim(document.forms[0].txtMin.value) != '') {
							if (!(isDate(document.forms[0].txtMin.value))) {
								alert('The min value must be a date');
								return;
							}
						}
						if ( trim(document.forms[0].txtMax.value) != '') {
							if (!(isDate(document.forms[0].txtMax.value))) {
								alert('The Max value must be a date');
								return;
							}
						}
					}

					idx=document.forms[0].selType.selectedIndex;
					document.forms[0].validation_type.value=document.forms[0].selType.options[idx].value;;
					//document.forms[0].action ="validation.aspx?flag=True&action=<%=Action%>&editorName=<%=editorName%>";
					document.forms[0].submit();
				}
			}


			function  isNumeric( strValue ) {
			/******************************************************************************
			DESCRIPTION: Validates that a string contains only valid numbers.

			PARAMETERS:
			strValue - String to be tested for validity

			RETURNS:
			True if valid, otherwise false.
			******************************************************************************/
			var objRegExp  =  /(^-?\d\d*\.\d*$)|(^-?\d\d*$)|(^-?\.\d\d*$)/;

			//check for numeric characters
			return objRegExp.test(strValue);
			}


			function isDate( strValue ) {
			/************************************************
			DESCRIPTION: Validates that a string contains only
				valid dates with 2 digit month, 2 digit day,
				4 digit year. Date separator can be ., -, or /.
				Uses combination of regular expressions and
				string parsing to validate date.
				Ex. mm/dd/yyyy or mm-dd-yyyy or mm.dd.yyyy

			PARAMETERS:
			strValue - String to be tested for validity

			RETURNS:
			True if valid, otherwise false.

			REMARKS:
			Avoids some of the limitations of the Date.parse()
			method such as the date separator character.
			*************************************************/
			var objRegExp = /^\d{1,2}(\/)\d{1,2}\1\d{4}$/

			//check to see if in correct format
			if(!objRegExp.test(strValue))
				return false; //doesn't match pattern, bad date
			else{
				var strSeparator = strValue.substring(2,3) //find date separator
				var arrayDate = strValue.split(strSeparator); //split date into month, day, year
				//create a lookup for months not equal to Feb.
				var arrayLookup = { '01' : 31,'03' : 31, '04' : 30,'05' : 31,'06' : 30,'07' : 31,
									'08' : 31,'09' : 30,'10' : 31,'11' : 30,'12' : 31}
				var intDay = parseInt(arrayDate[1]);

				//check if month value and day value agree
				if(arrayLookup[arrayDate[0]] != null) {
				if(intDay <= arrayLookup[arrayDate[0]] && intDay != 0)
					return true; //found in lookup table, good date
				}

				//check for February
				var intYear = parseInt(arrayDate[2]);
				var intMonth = parseInt(arrayDate[0]);
				if( ((intYear % 4 == 0 && intDay <= 29) || (intYear % 4 != 0 && intDay <=28)) && intDay !=0)
				return true; //Feb. had valid number of days
			}
			return false; //any other values, bad date
			}


			/***********************************************
			* Contractible Headers script- © Dynamic Drive (www.dynamicdrive.com)
			* This notice must stay intact for legal use. Last updated Oct 21st, 2003.
			* Visit http://www.dynamicdrive.com/ for full source code
			***********************************************/

			var enablepersist="off" //Enable saving state of content structure using session cookies? (on/off)
			var collapseprevious="no" //Collapse previously open content when opening present? (yes/no)

			if (document.getElementById){
			document.write('<style type="text/css">')
			document.write('.switchcontent{display:none;}')
			document.write('</style>')
			}

			function getElementbyClass(classname){
			ccollect=new Array()
			var inc=0
			var alltags=document.all? document.all : document.getElementsByTagName("*")
			for (i=0; i<alltags.length; i++){
			if (alltags[i].className==classname)
			ccollect[inc++]=alltags[i]
			}
			}

			function contractcontent(omit){
			var inc=0
			while (ccollect[inc]){
			if (ccollect[inc].id!=omit)
			ccollect[inc].style.display="none"
			inc++
			}
			}


			function expandcontent(cid){
			if (typeof ccollect!="undefined"){
			if (collapseprevious=="yes")
			contractcontent(cid)
			document.getElementById(cid).style.display=(document.getElementById(cid).style.display!="block")? "block" : "none"
			}
			}

			function expand_it(cid){
			if (typeof ccollect!="undefined"){
			document.getElementById(cid).style.display="block"
			}
			}

			function collapse_it(cid){
			if (typeof ccollect!="undefined"){
			document.getElementById(cid).style.display="none"
			}
			}

			function revivecontent(){
				contractcontent("omitnothing")
				selectedItem=getselectedItem()
				selectedComponents=selectedItem.split("|")
				for (i=0; i<selectedComponents.length-1; i++)
				document.getElementById(selectedComponents[i]).style.display="block"
			}

			function get_cookie(Name) {
			var search = Name + "="
			var returnvalue = "";
			if (document.cookie.length > 0) {
			offset= document.cookie.indexOf(search)
			if (offset != -1) {
			offset += search.length
			end = document.cookie.indexOf(";", offset);
			if (end == -1) end = document.cookie.length;
			returnvalue=unescape(document.cookie.substring(offset, end))
			}
			}
			return returnvalue;
			}

			function getselectedItem(){
			if (get_cookie(window.location.pathname) != ""){
			selectedItem=get_cookie(window.location.pathname)
			return selectedItem
			}
			else
			return ""
			}

			function saveswitchstate(){
			var inc=0, selectedItem=""
			while (ccollect[inc]){
			if (ccollect[inc].style.display=="block")
			selectedItem+=ccollect[inc].id+"|"
			inc++
			}

			document.cookie=window.location.pathname+"="+selectedItem
			}

			function do_onload(){
				getElementbyClass("switchcontent")
				if (enablepersist=="on" && typeof ccollect!="undefined")
				revivecontent()
			}


			if (window.addEventListener)
			window.addEventListener("load", do_onload, false)
			else if (window.attachEvent)
			window.attachEvent("onload", do_onload)
			else if (document.getElementById)
			window.onload=do_onload

			if (enablepersist=="on" && document.getElementById)
			window.onunload=saveswitchstate


			function show_range() {
 				var myindex
				myindex = document.forms[0].selType.selectedIndex;
				if ((myindex == 9) || (myindex == 2) || (myindex == 4)) {
					expand_it('sc3');
					if (myindex == 4) {
						expand_it('sc2');
					}
				} else {
					collapse_it('sc3');
					collapse_it('sc2');
				}
			}
			function show_range2(Min, Max) {
 				var myindex
 				var bMin
 				var bMax
 				var arMin = Min.split(',');
 				var arMax = Max.split(',');
 				bMin = false;
 				bMax = false;
				myindex = document.forms[0].selType[document.forms[0].selType.selectedIndex].value;
				for (var i = 0; i < arMin.length; i++) {
					//alert(myindex + ' == ' + trim(arMin[i]));
					if (myindex == trim(arMin[i])){
						bMin = true;
						break;
					}
				}
				for (var i = 0; i < arMax.length; i++) {
					if (myindex == trim(arMax[i])){
						bMax = true;
						break;
					}
				}
				if (bMin || bMax) {
					expand_it('sc3');
				} else {
					collapse_it('sc3');
					collapse_it('sc2');
				}
			}

    </script>

</head>
<body onload="setTimeout('myBodyLoad()', 50);">
    <form name="frmValidation" id="frmValidation" action="validation.aspx" method="post"
    runat="server">
    <input type="hidden" name="field_text" value="<%=Server.HtmlEncode(FieldText)%>" />
    <input type="hidden" name="validation_type" value="<%=ValidationType%>" />
    <table>
        <tr>
            <td class="label" width="95">
                Validation Type:
            </td>
            <td>
                <asp:Literal ID="vType" runat="server" />

                <script language="javascript" type="text/javascript">
						function myBodyLoad() {
							show_range2('<%=arMin%>','<%=arMax%>');
						}
                </script>

            </td>
        </tr>
        <tr>
            <td class="label" title="Error Message" width="95">
                Error Message:
            </td>
            <td>
                <input type="text" title="Error Message" name="txtErrorMsg" onkeypress="javascript:return CheckKeyValue(event,'34');"
                    size="40" value="<%=ValidationMsg%>" />
            </td>
        </tr>
    </table>
    <div class="switchcontent" id="sc3">
        <table>
            <tr>
                <td class="label" title="Minimum Value" width="95">
                    Min Value:
                </td>
                <td>
                    <input type="text" title="Enter a Minimum Value" name="txtMin" size="10" value="<%=MinVal%>" /><div
                        class="switchcontent" id="sc2">
                        Ex. mm/dd/yyyy</div>
                </td>
            </tr>
            <tr>
                <td class="label" title="Maximum Value" width="95">
                    Max Value:
                </td>
                <td>
                    <input type="text" title="Enter a Maximum Value" name="txtMax" size="10" value="<%=MaxVal%>" />
                </td>
            </tr>
        </table>
    </div>
    <table align="center">
        <tr>
            <td>
                <input title="Save" type="button" value="Save" name="btnSave" onclick="javascript:doSubmit();" /><input
                    title="Cancel" type="button" value="Cancel" name="btnCancel" onclick="javascript:self.close();" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>

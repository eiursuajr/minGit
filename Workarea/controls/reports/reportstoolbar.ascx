<%@ Control Language="C#" AutoEventWireup="true" Inherits="reportstoolbar" CodeFile="reportstoolbar.ascx.cs" %>
<div id="dhtmltooltip"></div>
<script type="text/JavaScript">
		function UpdateView(){
			var oForm = document.forms[0];
			//var idx = oForm.selLang.selectedIndex;
			//var lang = oForm.selLang.options[idx].value;
			//debugger;
			var LangType = "";
			var ContType = "";
			var folderId = 0;
			var strAction = "";
			var strReportName = "";
			var strTempOrg = "";
			var objSelLang = document.getElementById('selLang');
			var objSelSupertype = document.getElementById('selAssetSupertype');
			
			strAction = "";
			strReportName = "<%=Request.QueryString["action"].ToString()%>";

			if (strReportName.toLowerCase() == "siteupdateactivity") {
				var selObj = document.forms[0].selDisplay;
				if (document.getElementById("rptFolder").value == "") {
					document.getElementById("rptFolder").value = document.getElementById("hselFolder").innerHTML;
				}
				document.getElementById("ContType").value = ContType;
				document.getElementById("LangType").value = LangType;
			}
			else {
				if (objSelLang != null)
				{
					LangType = objSelLang.value;
							
					if (strAction.indexOf("language") > -1) {
						strTemp =  strAction.substring(strAction.indexOf("language"),strAction.length);
						if ((strTemp.indexOf("&") > -1) && (strTemp.indexOf("&") < strTemp.length)) {
							strTemp = strTemp.substring(0,strTemp.indexOf("&"));
						}
						strTempOrg = strTemp;
						strTemp = strTemp.replace(strTemp.substring(strTemp.indexOf("=")+1),LangType)
						strAction = strAction.replace (strTempOrg,strTemp);
					}
					else	{
						strAction = strAction + "&language=" + LangType;
					}
				}
				
				if (objSelSupertype != null)	{
					ContType = objSelSupertype.value;
									
					if (strAction.indexOf("ContType") > -1)		{
						strTemp =  strAction.substring(strAction.indexOf("ContType"),strAction.length);
						if ((strTemp.indexOf("&") > -1) && (strTemp.indexOf("&") < strTemp.length)) {
							strTemp = strTemp.substring(0,strTemp.indexOf("&"));
						}
						strTempOrg = strTemp;
						strTemp = strTemp.replace(strTemp.substring(strTemp.indexOf("=")+1),ContType)
						strAction = strAction.replace (strTempOrg,strTemp);
					}
					else	{
						strAction = strAction + "&ContType=" + ContType;
					}
				}
		
				strAction = "reports.aspx?" + strAction;
				oForm.action = strAction;
				//window.location.replace(strAction);
				oForm.submit();
				return false;
			}
		}
		
		function ShowGraph () {
			//debugger;
			var selObj = document.forms[0].selDisplay;
			var oForm = document.forms[0];
			if (document.getElementById("rptFolder").value == "") {
				document.getElementById("rptFolder").value = document.getElementById("hselFolder").innerHTML;
			}
			strAction = "reports.aspx?action=siteupdateactivity&btnSubmit=1&showChart=1&filtertype=path&filterid=" + document.forms[0].fId.value + "&rptFolder=" + document.getElementById("rptFolder").value + "&subfldInclude=" + document.forms[0].subfolder.value + "&start_date='" + document.getElementById("start_date").value + "'&end_date='" + document.getElementById("end_date").value +  "'&report_display=" + selObj[selObj.selectedIndex].value + "&ContType=" + document.getElementById("selAssetSupertype").value + "&LangType=" + document.getElementById("selLang").value;
			//window.location.replace(strAction);
			oForm.action = strAction;
			oForm.submit();
		}
		
		function CheckApproveSelect()
		{
		    var _countChecked = 0;
            var err = 1;
            var bret = true;
            var oForm = document.forms[0];
            
            var ins = document.getElementsByTagName('input');
            for (i = 0; i < ins.length; i++) 
            {
                if (ins[i].getAttribute('type') == 'radio' && ins[i].name.indexOf("cr_") > -1 && ins[i].checked == true) { err = 0; }
            }
            	            
            if (err == 1) { bret = false; alert('Please select as least one review to update.'); }
            if (bret == true) {oForm.submit();}
		}
		
		function CheckApproveReset(toflag)
		{
		    var _countChecked = 0;
            var err = 1;
            var bret = true;
            
            var ins = document.getElementsByTagName('input');
            for (i = 0; i < ins.length; i++) 
            {
                if (toflag == true && ins[i].getAttribute('type') == 'radio' && ins[i].name.indexOf("cr_") > -1 && ins[i].value.indexOf("cr_app_") > -1) { ins[i].checked = true; }
                
                if (toflag == false && ins[i].getAttribute('type') == 'radio' && ins[i].name.indexOf("cr_") > -1 && ins[i].value.indexOf("cr_dec_") > -1) { ins[i].checked = true; }
            }
		}
		
</script>

<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>

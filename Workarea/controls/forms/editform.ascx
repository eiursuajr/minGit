<%@ Control Language="C#" AutoEventWireup="true" Inherits="editform" CodeFile="editform.ascx.cs" %>
<style type="text/css">
    #propertiesTabs
    {
	    border: 0px;
    }
</style>

<script type="text/javascript">
<!--
Ektron.ready( function()
{
   var tabsContainers = $ektron(".tabContainer");
   tabsContainers.tabs();
    // hook clicks to the properties and mail properties tab and add custom handling
    // here because we don't want the regular tab handling because that would really mess
    // up the form wizard which uses parts of this form :-P
    $ektron('#tabFormPropsLink').bind("click", function(){
	    $ektron("#dvProperties").show();
	    $ektron("#sc1").hide();
	    
	    $ektron('#tabFormProps').addClass('ui-tabs-selected ui-state-active');
	    $ektron('#tabMailProps').removeClass('ui-tabs-selected ui-state-active');
	    $ektron('#tabMailProps').addClass('ui-state-default');
	});
    $ektron('#tabMailPropsLink').bind("click", function(){
	    $ektron("#dvProperties").hide();
	    $ektron("#sc1").show();
	    
	    $ektron('#tabFormProps').removeClass('ui-tabs-selected ui-state-active');
	    $ektron('#tabMailProps').addClass('ui-tabs-selected ui-state-active');
	    $ektron('#tabFormProps').addClass('ui-state-default');
	});
});

    function VerifyAddForm()
	{
		if (!validateTitle()) return false;

		var objElem = null;
		objElem = document.getElementById(UniqueID+"frm_form_description");
		fixValue(objElem);

		if (!validateNumSubmission()) return false;
		/*
		var bFormData = false;
		if (document.getElementById(UniqueID+"frm_form_mail").checked) {
			bFormData = true;
		}
		if (!bFormData) {
			if (document.getElementById(UniqueID+"frm_form_db").checked) {
				bFormData = true;
			}
		}
		if (!bFormData) {
			DisplayHoldMsg(false);
			alert("Specify form data!");
			return false;
		}
		*/
		if (!validateEmail()) return false;
		objElem = document.getElementById(UniqueID+"frm_form_mailcc");
		fixValue(objElem);

		objElem = document.getElementById(UniqueID+"frm_form_mailsubject");
		fixValue(objElem);

		objElem = document.getElementById(UniqueID+"frm_form_mailpreamble");
		fixValue(objElem);

		document.getElementById(UniqueID+"frm_multi_form_id").value="<%=Request.QueryString["form_id"]%>";

		return true;
	}

	function validateTitle()
	{
		var objElem = document.getElementById(UniqueID+"frm_form_title");
		fixValue(objElem);
		if (objElem != null)
		{
			if ("" == objElem.value){
				DisplayHoldMsg(false);
				if (typeof oProgressSteps != "undefined")
				{
					oProgressSteps.select("title");
				}
				alert("<asp:Literal id='ltr_formTitle' runat='server'/>");
				objElem.focus();
				return false;
			}
		}
		return true;
	}

	function validateNumSubmission()
	{
		var objElemChecks = document.getElementById(UniqueID+"frm_form_limit_submission");
		var objElemNum = document.getElementById(UniqueID+"frm_form_number_of_submission");
		if (objElemChecks != null)
		{
			if (true == objElemChecks.checked && "" == objElemNum.value)
			{
				DisplayHoldMsg(false);
				alert("<asp:Literal runat='server' id='ltr_msgSubmission'/>");
				objElemNum.focus();
				return false;
			}
		}
		return true;
	}

	function validateEmail()
	{
	    var objElem = document.getElementById(UniqueID+"frm_form_mailto");
		fixValue(objElem);
		if (!validateEmailFormat()) return false;
		if (objElem != null)
		{
			if (document.getElementById(UniqueID+"frm_form_mail").checked) {
				if ("" == objElem.value)
				{
					DisplayHoldMsg(false);
					if (typeof oProgressSteps != "undefined")
					{
						oProgressSteps.select("email");
					}
					alert("<asp:Literal runat='server' id='ltr_emailReq'/>");
					$ektron("a#tabMailPropsLink").click();
					objElem.focus();
					return false;
				}
			}
		}
		return true;
	}
	function validateEmailFormat()
	{
		// RFC 2822 Standard Email Formats
		var reEmailAddrsSemiDelim = /^[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*(\s*\;\s*[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*)*$/;
		var reEmailAddr = /^[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*$/;
		var reSubstitutionField = new RegExp("^\xAB[^\xBB]+\xBB$"); // \xAB = «(<<), \xBB = »(>>) which may be corrupted if file is not saved as UTF-8, so use ASCII 
		var email_to = document.getElementById(UniqueID+"frm_form_mailto");
		email_to.value = $ektron.trim(email_to.value);
		var email_from = document.getElementById(UniqueID+"frm_form_mailfrom");
		email_from.value = $ektron.trim(email_from.value);
		var email_cc = document.getElementById(UniqueID+"frm_form_mailcc");
		email_cc.value = $ektron.trim(email_cc.value);
		if (email_to.value != "" && !reSubstitutionField.test(email_to.value) && !reEmailAddrsSemiDelim.test(email_to.value)) 
		{
			alert("<asp:Literal runat='server' id='ltr_valemailaddr'/>");
			email_to.focus();
			return false;
		}
		if (email_cc.value != "" && !reSubstitutionField.test(email_cc.value) && !reEmailAddrsSemiDelim.test(email_cc.value)) 
		{
			alert("<%= m_refMsg.GetMessage("enter valid email address or leave blank")%>");
			email_cc.focus();
			return false;
		}
		if (email_from.value != "" && !reSubstitutionField.test(email_from.value) && !reEmailAddr.test(email_from.value)) 
		{
			alert("<%= m_refMsg.GetMessage("enter valid email address or leave blank")%>");
			email_from.focus();
			return false;
		}
		return true;
	}
	function expandmail(name, cid)
	{
		var objElem = document.forms["myform"].elements[UniqueID+name];
		if (objElem != null)
		{
			if (objElem.checked == true)
			{
				document.getElementById(cid).style.display = "";
			}
			else
			{
				document.getElementById(cid).style.display = "none";
			}
		}
	}

	function fixValue(objElem)
	{
		if (!objElem) return;
		objElem.value = Trim(objElem.value).replace(/\"/g, "'");
	}

	function useFieldValue(objSelectElem)
	{
		if (!objSelectElem) return;
		if (objSelectElem.tagName != "SELECT") return;
		var id = objSelectElem.id;
		id = id.substr(0, id.length - 4); // Remove "_sel" from end of name
		var objElem = document.getElementById(id);
		if (!objElem) return;
		var idx = objSelectElem.selectedIndex;
		if (idx <= 0 || "" == objSelectElem.value)
		{
			objElem.value = "";
			objElem.readOnly = false;
		}
		else
		{
			// Can't use descriptive name (objSelectElem.options[idx].text) b/c the
			// field name is needed and there's no other place to store it.
			var strFieldName = objSelectElem.value;
		    // 171 = left (double) angle quote (guillemet)
			// 187 = right (double) angle quote (guillemet)
			objElem.value = String.fromCharCode(171) + strFieldName + String.fromCharCode(187);
			objElem.readOnly = true;
		}
	}

	function SetSubmissions (name, cid) {
	    var objElem = document.forms["myform"].elements[UniqueID+name];
		    if (objElem != null)
		    {
			    var bChecked = (objElem.checked == true);
			    document.getElementById(UniqueID+cid).readOnly = !bChecked;
				document.getElementById(UniqueID+cid).disabled = !bChecked;
		    }
	}

	function onDatabase(objElem)
	{
		var bChecked = (objElem.checked);
		var objAutofillElem = document.getElementById(UniqueID + "frm_form_af");
		if (objAutofillElem)
		{
			objAutofillElem.readOnly = !bChecked;
			objAutofillElem.disabled = !bChecked;
		}
	}

// -->
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader editFormHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>

<div id="generalProperties" class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper" id="propertiesTabContainer">
            <div class="tabContainer" >
                <ul>
                    <li id="tabFormProps">
                        <a title="<%=m_refMsg.GetMessage("form properties text")%>" href="#dvProperties" id="tabFormPropsLink">
                            <%=m_refMsg.GetMessage("form properties text")%>
                        </a>
                    </li>
                    <li id="tabMailProps">
                        <a title="<%=m_refMsg.GetMessage("lbl mail properties")%>" href="#emailProperties" id="tabMailPropsLink">
                            <%=m_refMsg.GetMessage("lbl mail properties")%>
                        </a>
                    </li>
                </ul>

                    <div id="emailProperties">
	                <hr/>
	                <p onclick="expandcontent('sc1')">
	                    <span class="moreinfo">
	                        <%=m_refMsg.GetMessage("lbl mail properties")%>
	                    </span>
	                </p>
        </div>
        <div id="dvProperties" style="">
	    <table class="ektronGrid ektronBorder" width="100%">
	            <asp:Placeholder ID="EditFormPanel1" Runat="server" Visible="False">
	        <tr>
	                <td class="label" title="<%=m_refMsg.GetMessage("id label")%>">
                        <%=m_refMsg.GetMessage("id label")%>
                    </td>
                    <td>
                        <asp:Literal id="lblFormId" Runat="server" />
                        <input id="frm_form_id" type="hidden" name="frm_form_id" runat="server"/>
                    </td>
            </tr>
                </asp:Placeholder>
               
		    <tr id="titleProperty">
			    <td class="label" title="<%=m_refMsg.GetMessage("generic title label")%>"><%=m_refMsg.GetMessage("generic title label")%></td>
			    <td>
				    <input title="Title" name="frm_form_title" id="frm_form_title" type="text" maxlength="75" size="40" runat="server" />
				    <asp:Literal ID="lblLangName" Runat="server" />
			    </td>
		    </tr>
		    <tr id="descriptionProperty">
			    <td class="label"><%=m_refMsg.GetMessage("lbl discussionforumtitle")%>:</td>
			    <td><input name="frm_form_description" id="frm_form_description" type="text" maxlength="255" size="40" runat="server" /></td>
		    </tr>
		    <tr id="dataProperties">
			    <td class="label"><%=m_refMsg.GetMessage("lbl form data")%>:</td>
			    <td>
			        <span id="mailProperty">
			            <input name="frm_form_mail" title="Mail" id="frm_form_mail" type="checkbox" value="mail" onclick="expandmail('frm_form_mail', 'sc1')" runat="server" />&#160;<label title="<%=m_refMsg.GetMessage("lbl mail")%>" for="<%=frm_form_mail.ClientID%>"><%=m_refMsg.GetMessage("lbl mail")%></label>
			            <br />
				    </span>
				    <span id="dbProperty">
				        <input name="frm_form_db" title="Database" id="frm_form_db" type="checkbox" value="db" onclick="onDatabase(this)" runat="server" />&#160;<label title="<%=m_refMsg.GetMessage("lbl database")%>" for="<%=frm_form_db.ClientID%>"><%=m_refMsg.GetMessage("lbl database")%></label>
				        <br />
				    </span>
				    <span id="afProperty">
				        <input name="frm_form_af" title="Autofill form values" id="frm_form_af" type="checkbox" value="af" runat="server" />&#160;<label title="<%=m_refMsg.GetMessage("lbl autofill form values")%>" for="<%=frm_form_af.ClientID%>"><%=m_refMsg.GetMessage("lbl autofill form values")%></label>
				        <br />
				    </span>
				    <span id="submissionProperty">
				        <input name="frm_form_limit_submission" title="Limit submissions" id="frm_form_limit_submission" type="checkbox" onclick="SetSubmissions('frm_form_limit_submission', 'frm_form_number_of_submission')" runat="server" />&#160;<label title="<%=m_refMsg.GetMessage("lbl limit submissions")%>" for="<%=frm_form_limit_submission.ClientID%>"><%=m_refMsg.GetMessage("lbl limit submissions")%></label>
				        <br />
				    </span>
			        <span id="txtSubmissionProperty">
			            <input name="frm_form_number_of_submission" title="Number Of Submissions" id="frm_form_number_of_submission" type="text" size="3" style="width: auto;" runat="server" />&#160;&#160;<label title="<%=m_refMsg.GetMessage("lbl number of submissions")%>" for="<%=frm_form_number_of_submission.ClientID%>"><%=m_refMsg.GetMessage("lbl number of submissions")%></label>
			        </span>
			    </td>
		    </tr>
		    <tr id="taskProperties">
			    <td class="label" title="<%=m_refMsg.GetMessage("lbl assign task to")%>"><%=m_refMsg.GetMessage("lbl assign task to")%>:</td>
			    <td>
                    <input type="hidden" name="content_id" value="" id="content_id" runat="server"/>
                    <input type="hidden" name="current_language" value="" id="current_language" runat="server"/>
                    <input type="hidden" name="assigned_to_user_id" value="" id="assigned_to_user_id" runat="server"/>
                    <input type="hidden" name="assigned_to_usergroup_id" value="" id="assigned_to_usergroup_id" runat="server"/>
                    <input type="hidden" name="current_user_id" value="" id="current_user_id" runat="server"/>
                    <input type="hidden" name="assigned_by_user_id" value="" id="assigned_by_user_id" runat="server"/>
                    <asp:Literal ID="AssignTaskTo" Runat="server"/>
			    </td>
		    </tr>
	    </table>
	    </div>
            </div>
    </div>
    
</div>
<input type="hidden" name="frm_action" id="frm_action" value="AddForm" runat="server"/>
<input type="hidden" name="frm_folder_id" id="frm_folder_id" runat="server"/> <input type="hidden" name="frm_content_id" id="frm_content_id" runat="server"/>
<input type="hidden" name="frm_multi_form_id" id="frm_multi_form_id" runat="server"/>
<input type="hidden" name="frm_multi_form_language" id="frm_multi_form_language" runat="server"/>
<input type="hidden" name="frm_copy_lang_from" id="frm_copy_lang_from" runat="server"/>
<input type="hidden" name="frm_form_type" id="frm_form_type" runat="server"/>


<div class="switchcontent" id="sc1" style="position: absolute; top: 100px; left:10px;">
	<table class="ektronGrid">
		<col valign="top" /><col valign="top" />
		<tr>
			<td class="label" title="<%=m_refMsg.GetMessage("generic to label")%>"><%=m_refMsg.GetMessage("generic to label")%></td> <!--- -HC- --->
			<td>
			    <input name="frm_form_mailto" id="frm_form_mailto" type="text" maxlength="75" size="40"	runat="server" />
			    <asp:Literal ID="litMailTo"  Runat="server" />
			</td>
		</tr>
		<tr>
			<td class="label" title="<%=m_refMsg.GetMessage("generic from label")%>"><%=m_refMsg.GetMessage("generic from label")%></td> <!--- -HC- --->
			<td>
			    <input name="frm_form_mailfrom" id="frm_form_mailfrom" type="text" maxlength="75" size="40"	runat="server" />
				<asp:Literal ID="litMailFrom"  Runat="server" />
			</td>
		</tr>
		<tr>
			<td class="label" title="<%=m_refMsg.GetMessage("generic cc label")%>"><%=m_refMsg.GetMessage("generic cc label")%></td> <!--- -HC- --->
			<td>
			    <input name="frm_form_mailcc" id="frm_form_mailcc" type="text" maxlength="75" size="40"	runat="server" />
				<asp:Literal ID="litMailCC"  Runat="server" />
			</td>
		</tr>
		<tr>
			<td class="label" title="<%=m_refMsg.GetMessage("generic subject label")%>"><%=m_refMsg.GetMessage("generic subject label")%></td> <!--- -HC- --->
			<td>
			    <input name="frm_form_mailsubject" id="frm_form_mailsubject" type="text" maxlength="75"	size="40" runat="server" />
				<asp:Literal ID="litMailSubject" Runat="server" />
			</td>
		</tr>
		<tr>
			<td class="label" title="<%=m_refMsg.GetMessage("lbl preamble")%>"><%=m_refMsg.GetMessage("lbl preamble")%>:</td><!--- -HC- --->
			<td>
			    <input name="frm_form_mailpreamble" id="frm_form_mailpreamble" type="text" maxlength="255" size="40" runat="server" />
			    <asp:Literal ID="litMailMessageBody" Runat="server" />
			</td>
		</tr>
		<tr>
			<td class="label" title="<%=m_refMsg.GetMessage("alt send data in xml format")%>"><%=m_refMsg.GetMessage("alt send data in xml format")%>:</td>
			<td>
			    <input type="checkbox" title="Send XML Packet" name="frm_send_xml_packet" id="frm_send_xml_packet" runat="server" />
			</td>
		</tr>
	</table>
</div>

<input type="hidden" name="frm_initial_form" id="frm_initial_form" runat="server" />
<input type="hidden" name="frm_initial_response" id="frm_initial_response" runat="server" />

<script type="text/javascript">
<!--
	function onSelectInitialForm(objSelectedForm)
	{
		document.getElementById(UniqueID+"frm_form_title").value = objSelectedForm.title.substr(0,75);
		document.getElementById(UniqueID+"frm_form_description").value = objSelectedForm.description.substr(0,255);
		if (typeof objSelectedForm.frm_form_number_of_submission != "undefined")
		{
			document.getElementById(UniqueID+"frm_form_number_of_submission").value = objSelectedForm.frm_form_number_of_submission;
		}
		if (typeof objSelectedForm.type != "undefined")
		{
			document.getElementById(UniqueID+"frm_form_type").value = objSelectedForm.type.substr(0,10);
		}
		else
		{
			document.getElementById(UniqueID+"frm_form_type").value = "form";
		}
		if (typeof objSelectedForm.submit != "undefined")
		{
			document.getElementById(UniqueID+"frm_form_limit_submission").checked = true;
			SetSubmissions('frm_form_limit_submission', 'frm_form_number_of_submission');
			document.getElementById(UniqueID+"frm_form_number_of_submission").value = objSelectedForm.submit.limit;
			if (typeof objSelectedForm.submit.autofill != "undefined")
			{
				document.getElementById(UniqueID+"frm_form_af").checked = objSelectedForm.submit.autofill;
			}
		}
		else
		{
			document.getElementById(UniqueID+"frm_form_limit_submission").checked = false;
			SetSubmissions('frm_form_limit_submission', 'frm_form_number_of_submission');
		}
		if ("object" == typeof objSelectedForm.mail && objSelectedForm.mail != null)
		{
			document.getElementById(UniqueID+"frm_form_mail").checked = true;
			document.getElementById(UniqueID+"frm_form_mailto").value = objSelectedForm.mail.to.substr(0,255);
			document.getElementById(UniqueID+"frm_form_mailfrom").value = objSelectedForm.mail.from.substr(0,75);
			document.getElementById(UniqueID+"frm_form_mailcc").value = objSelectedForm.mail.cc.substr(0,255);
			document.getElementById(UniqueID+"frm_form_mailsubject").value = objSelectedForm.mail.subject.substr(0,128);
			document.getElementById(UniqueID+"frm_form_mailpreamble").value = objSelectedForm.mail.messageBody.substr(0,128);
        }
        else
        {
			document.getElementById(UniqueID+"frm_form_mail").checked = false;
			document.getElementById(UniqueID+"frm_form_mailto").value = "";
			document.getElementById(UniqueID+"frm_form_mailfrom").value = "";
			document.getElementById(UniqueID+"frm_form_mailcc").value = "";
			document.getElementById(UniqueID+"frm_form_mailsubject").value = "";
			document.getElementById(UniqueID+"frm_form_mailpreamble").value = "";
        }
		document.getElementById(UniqueID+"frm_initial_form").value = objSelectedForm.designSrc;
		document.getElementById(UniqueID+"frm_initial_response").value = objSelectedForm.responseSrc;
		definedProgressSteps(objSelectedForm);
	}

	function definedProgressSteps(objSelectedForm)
	{
		if (typeof objSelectedForm != "undefined")
		{
			if ("undefined" == typeof objSelectedForm.submit)
			{
				//all forms
				oProgressSteps.define([
{ id:"select",	title:"<%= m_refMsg.GetMessage("lbl new form")%>",			description:"<%= m_refMsg.GetMessage("msg sel form below and begin with blank")%>"}
, { id:"title",	title:"<%= m_refMsg.GetMessage("lbl form title")%>",			description:"<%= m_refMsg.GetMessage("alt msg form title")%>" }
//, { id:"db",	title:"Data Storage",		description:"Do you wish to store the submitted data in the database? You can change your answer later in the form properties." }
//, { id:"email",	title:"Email Notification",	description:"Do you wish to send an email when form data is submitted? You can change your answer later in the form properties." }
, { id:"task",	title:"<%= m_refMsg.GetMessage("lbl assign task")%>",		description:"<%= m_refMsg.GetMessage("alt msg assign task")%>" }
, { id:"cont",	title:"<%= m_refMsg.GetMessage("alt continue to design")%>",	description:"" }
//, { id:"",		title:"Meta Data",			description:"" }
//, { id:"",		title:"Schedule",			description:"" }
, { id:"",		title:"<%= m_refMsg.GetMessage("lbl setup complete")%>",		description:"" }
]);
			}
			else
			{
				//poll and survey
				oProgressSteps.define([
{ id:"select",	title:"<%= m_refMsg.GetMessage("lbl new form")%>",			description:"<%= m_refMsg.GetMessage("msg sel form below and begin with blank")%>" }
, { id:"title",	title:"<%= m_refMsg.GetMessage("lbl form title")%>",			description:"<%= m_refMsg.GetMessage("alt msg form title")%>" }
, { id:"cont",	title:"<%= m_refMsg.GetMessage("msg define poll")%>",	description:"" }
, { id:"",		title:"<%= m_refMsg.GetMessage("lbl setup complete")%>",		description:"" }
]);
			}
		}
	}

	function ensureCleanPage()
	{
		document.getElementById("titleProperty").style.display = "none";
		document.getElementById("descriptionProperty").style.display = "none";
		document.getElementById("dataProperties").style.display = "none";
		document.getElementById("taskProperties").style.display = "none";
	}
// -->
</script>
<div class="ektronPageContainer ektronPageInfo" id="selectInitialWrapper" style="z-index:-1">
    <div id="selectInitial">
	    <asp:Literal id="SelectInitialForm" runat="server" />
    </div>
</div>
<script type="text/javascript">
<!--
SetSubmissions ("frm_form_limit_submission", "frm_form_number_of_submission");
if ("object" == typeof oProgressSteps && oProgressSteps != null)
{
	var strProgressSteps_PanelTop = "55px";
	var oSelInit = document.getElementById("selectInitial");
	var oGenProps = document.getElementById("generalProperties");
	var oSC1 = document.getElementById("sc1");
	document.getElementById("emailProperties").style.display = "none";
	oSelInit.style.position = "absolute";
	oSelInit.style.top = strProgressSteps_PanelTop;
	oGenProps.style.position = "absolute";
	oGenProps.style.top = strProgressSteps_PanelTop;
	oSC1.style.position = "relative";
	oSC1.style.top = strProgressSteps_PanelTop;

	// Initialize to a clean display
    //oSelInit.style.display = "none";
    oGenProps.style.display = "none";
    oSC1.style.display = "none";

	oProgressSteps.onselect = function(stepNumber)
	{
		var oSelInit = document.getElementById("selectInitial");
		var oGenProps = document.getElementById("generalProperties");
		var oSC1 = document.getElementById("sc1");
		oSelInit.style.display = "none";
		oGenProps.style.display = "none";
		oSC1.style.display = "none";
		switch (this.getStep(stepNumber).id)
		{
		case "select":
			oSelInit.style.display = "";
			setTimeout("ensureCleanPage()", 700);
			break;
		case "title":
            $ektron(oGenProps).show();
            $ektron(".tabContainerWrapper").show();
            $ektron("div.tabContainer").css("border:none");
            $ektron(".tabContainerWrapper ul, div.ektronTabBackground").hide();
            $ektron("#titleProperty, #descriptionProperty").show().parent().css("opacity", "100%"); 
            $ektron("#taskProperties, #dataProperties").hide();
			break;
		case "db":
			if (!validateTitle()) return false;
			oGenProps.style.display = "";
			document.getElementById("titleProperty").style.display = "none";
			document.getElementById("descriptionProperty").style.display = "none";
			document.getElementById("dataProperties").style.display = "";
			document.getElementById("taskProperties").style.display = "none";
			document.getElementById("mailProperty").style.display = "none";
			document.getElementById("dbProperty").style.display = "inline";
			document.getElementById("afProperty").style.display = "inline";
			break;
		case "email":
			if (!validateTitle()) return false;
			oGenProps.style.display = "";
			document.getElementById("titleProperty").style.display = "none";
			document.getElementById("descriptionProperty").style.display = "none";
			document.getElementById("dataProperties").style.display = "";
			document.getElementById("taskProperties").style.display = "none";
			document.getElementById("mailProperty").style.display = "inline";
			document.getElementById("dbProperty").style.display = "none";
			document.getElementById("afProperty").style.display = "none";
			oSC1.style.display = "";
			break;
		case "task":
			if (!validateTitle()) return false;
			if (!validateEmail()) return false;
			oGenProps.style.display = "";
			document.getElementById("titleProperty").style.display = "none";
			document.getElementById("descriptionProperty").style.display = "none";
			document.getElementById("dataProperties").style.display = "none";
			document.getElementById("taskProperties").style.display = "";
			break;
		case "cont":
			if (!validateTitle()) return false;
			if (!validateEmail()) return false;
			var objAElem = document.getElementById("image_link_100");
			if (objAElem)
			{
				if ("function" == typeof objAElem.onclick)
				{
					oProgressSteps.disable(); // prevent multiple clicks when submitting form
					objAElem.onclick();
				}
				else //if ("string" == typeof objAElem.onclick) //for mac browsers
				{
					var sFn = objAElem.getAttribute("onclick");
					if (sFn)
					{
						oProgressSteps.disable(); // prevent multiple clicks when submitting form
						objAElem.fnonclick = new Function(sFn);
						objAElem.fnonclick();
					}
				}
			}
			return false;
			break;
		default:
			break;
		}
	}
	oProgressSteps.oncancel = function()
	{
	    var fromEE = location.href.indexOf("FromEE=1");
	    if(fromEE > 0)
	    {
	        window.close();
	        return;
	    }
        location.href = Ektron.Workarea.Sync.Resources.sitePath + "content.aspx?action=ViewContentByCategory&id=" + vFolderId + "&treeViewId=0";
//		var objAElem = document.getElementById("image_link_101");
//		if (objAElem)
//		{
//			location.href = objAElem.href;
//		}
	}
	definedProgressSteps();
}
// -->
</script>

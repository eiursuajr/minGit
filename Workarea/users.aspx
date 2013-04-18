<%@ Page  Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="users" MaintainScrollPositionOnPostback="True" CodeFile="users.aspx.cs" %>
<%@ Reference Control="controls/user/viewusers.ascx" %>
<%@ Reference Control="controls/user/viewgroups.ascx" %>
<%@ Reference Control="controls/user/adduser.ascx" %>
<%@ Reference Control="controls/user/editgroups.ascx" %>
<%@ Reference Control="controls/user/edituser.ascx" %>
<%@ Reference Control="controls/user/viewcustomproperties.ascx" %>
<%@ Reference Control="controls/user/addcustomproperty.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>Users</title>
        <asp:literal id="jsStyleSheet" runat="server"/>
        <script type="text/javascript">
	        <!--
			    var UniqueID="<asp:literal id="jsUniqueId" runat="server"/>";
			    var ContentLanguage = "<asp:literal id="jsContentLanguage" runat="server"/>";
			    var SitePath = "<asp:literal id="jsSitePath" runat="server"/>";
			    var ImageExtensions = "<asp:literal id="jsImageExtensions" runat="server"/>";
			    var jsUsernameReq = "<asp:literal id="jsUsernameReq" runat="server"/>";
			    var jsFirstnameReq = "<asp:literal id="jsFirstnameReq" runat="server"/>";
			    var jsLastnameReq = "<asp:literal id="jsLastnameReq" runat="server"/>";
			    var jsEmailBlank = "<asp:literal id="jsEmailBlank" runat="server"/>";
			    var jsEmailAddress = "<asp:literal id="jsEmailAddress" runat="server"/>";
			    var jsEmailInvalid = "<asp:literal id="jsEmailInvalid" runat="server"/>";
			    var jsValidEmail = "<asp:literal id="jsValidEmail" runat="server"/>";
			    var jsMakeSelection = "<asp:literal id="jsMakeSelection" runat="server"/>";
			    var jsInvalidPageWidth = "<asp:literal id="jsInvalidPageWidth" runat="server"/>";
			    var jsInvalidPageHeight = "<asp:literal id="jsInvalidPageHeight" runat="server"/>";
                jsEmailNoUserMsg = "<asp:literal id="jsEmailNoUserMsg" runat="server"/>";

			    var g_emailChecked = false;
			    var g_already_in_pwd_event = false;
			    var jsADIntegration=<asp:literal id="jsADIntegration" runat="server"/>;
			    function LoadChildPage() {
				    if (IsBrowserIE_Email()) 
				    {
					    var frameObj = document.getElementById("ChildPage");
					    frameObj.src = "blankredirect.aspx?SelectCreateContent.aspx?FolderID=0&rmadd=false&LangType=" + ContentLanguage + "&browser=0"
    					
					    var pageObj = document.getElementById("FrameContainer");
					    pageObj.style.display = "";
					    pageObj.style.width = "80%";
					    pageObj.style.height = "80%";
    					
				    }
				    else
				    {
					    // Using Netscape; cant use transparencies & eWebEditPro preperly 
					    // - so launch in a seperate pop-up window:
					    PopUpWindow("SelectCreateContent.aspx?FolderID=0&rmadd=false&LangType=" + ContentLanguage + "&browser=1","SelectContent", 490,500,1,1);
    					
				    }
    				
			    }
    			
			    function ReturnChildValue(contentid,contenttitle,QLink, FolderID, LanguageID) {
				    // take value, store it, write to display
    				
				    CloseChildPage();
				    document.getElementById("templatefilename").value = QLink.replace(SitePath,'');
				    document.getElementById("folderId").value = FolderID;
			    }
    			
				    function CloseChildPage()
			    {
				    if (IsBrowserIE_Email()) 
				    {
					    var pageObj = document.getElementById("FrameContainer");
					    pageObj.style.display = "none";
					    pageObj.style.width = "1px";
					    pageObj.style.height = "1px";
				    }
			    }
    			
			    function IsChildWaiting() {
				    var pageObj = document.getElementById("FrameContainer");
				    if (pageObj == null) {
					    return (false);
				    }
				    if (pageObj.style.display == "") {
					    return (true);
				    }
				    else {
					    return (false);
				    }
			    }
    			
			    function GoBackToCaller(){
			    window.location.href = document.referrer;
			    }
    			
			    function VerifyLDAPForm (){
			    var regexp1 = /"/gi;
			    document.getElementById(UniqueID+"LDAP_username").value = Trim(document.getElementById(UniqueID+"LDAP_username").value.replace(regexp1, "'"));
			    document.getElementById(UniqueID+"LDAP_firstname").value = Trim(document.getElementById(UniqueID+"LDAP_firstname").value.replace(regexp1, "'"));
			    document.getElementById(UniqueID+"LDAP_lastname").value = Trim(document.getElementById(UniqueID+"LDAP_lastname").value.replace(regexp1, "'"));
			    document.getElementById(UniqueID+"LDAP_email_addr1").value = Trim(document.getElementById(UniqueID+"LDAP_email_addr1").value.replace(regexp1, "'"));
			    //document.getElementById(UniqueID+"LDAP_organization").value = Trim(document.getElementById(UniqueID+"LDAP_organization").value.replace(regexp1, "'"));
			    //document.getElementById(UniqueID+"LDAP_orgunit").value = Trim(document.getElementById(UniqueID+"LDAP_orgunit").value.replace(regexp1, "'"));
			    document.getElementById(UniqueID+"LDAP_ldapdomain").value = Trim(document.getElementById(UniqueID+"LDAP_ldapdomain").value.replace(regexp1, "'"));
			    if (document.getElementById(UniqueID+"LDAP_username").value == "")
			    {
				    alert (jsUsernameReq);
				    document.getElementById(UniqueID+"LDAP_username").focus();
				    return false;
			    }
			    if (document.getElementById(UniqueID+"LDAP_ldapdomain").value == "")
			    {
				    alert("For LDAP authentication, you are required to have a path.");
				    document.getElementById(UniqueID+"LDAP_orgunit").focus();
				    return false;
			    } // else{
    			
			    if (document.getElementById(UniqueID+"LDAP_firstname").value == "")
			    {
				    alert (jsFirstnameReq);
				    document.getElementById(UniqueID+"LDAP_firstname").focus();
				    return false;
			    }
    			
			    if (document.getElementById(UniqueID+"LDAP_lastname").value == "")
			    {
				    alert (jsLastnameReq);
				    document.getElementById(UniqueID+"LDAP_lastname").focus();
				    return false;
			    }		
    			
			    if (document.getElementById(UniqueID+"LDAP_disable_msg").checked == false) {
				    if (document.getElementById(UniqueID+"LDAP_email_addr1").value == "") {
				    if(jsADIntegration){
					    alert(jsEmailBlank);
				    }else{		
					    alert(jsEmailAddress);
					    document.getElementById(UniqueID+"LDAP_email_addr1").focus();
				    }
					    return false;
				    }
				    var atLocation = document.getElementById(UniqueID+"LDAP_email_addr1").value.search("@")
				    if ((atLocation == -1) || (atLocation == 0) || (atLocation == (document.getElementById(UniqueID+"LDAP_email_addr1").value.length - 1))) {
				     if (jsADIntegration){
					    alert(jsEmailInvalid);			
				    }else 					{
					    alert(jsValidEmail);				
					    document.getElementById(UniqueID+"LDAP_email_addr1").focus();	}		
									    return false;
				    }
			    }
			    var width = "";
			    var height = "";
			    if (typeof document.forms[0].txtWidth == "object") {
				    width = Trim(document.getElementById(UniqueID+"txtWidth").value);
					    if((isNaN(width)) || (width < 400) || (width > 2400)) {
						    alert (jsInvalidPageWidth);
						    return false;
					    }
			    }
			    if (typeof document.forms[0].txtHeight == "object") {
				    height = Trim(document.getElementById(UniqueID+"txtHeight").value);
				    if((isNaN(height)) || (height < 300) || (height > 1800)) {
					    alert (jsInvalidPageHeight);
					    return false;
				    }
			    }
			    return true;
			    }
			    function VerifyForm () {
                var specialCaseOverride = (document.getElementById(UniqueID+"specialCaseOverride") != null &&
                    document.getElementById(UniqueID+"specialCaseOverride").value == "1");
			    var regexp1 = /"/gi;
			    document.getElementById(UniqueID+"username").value = Trim(document.getElementById(UniqueID+"username").value.replace(regexp1, "'"));
			    document.getElementById(UniqueID+"firstname").value = Trim(document.getElementById(UniqueID+"firstname").value.replace(regexp1, "'"));
			    document.getElementById(UniqueID+"lastname").value = Trim(document.getElementById(UniqueID+"lastname").value.replace(regexp1, "'"));
			    document.getElementById(UniqueID+"email_addr1").value = Trim(document.getElementById(UniqueID+"email_addr1").value.replace(regexp1, "'"));
			    var orgunit_text=document.getElementById("orgunit_text");
    			
			    if (typeof(eWebEditPro) != "undefined" && eWebEditPro != null)
			    {
			        eWebEditPro.instances[0].editorGetMethod = 'getBodyHTML';
			        eWebEditPro.instances[0].save(eWebEditPro.instances[0].linkedElement, null, new Function());
			    }  
    			
			    if (("object"==typeof(orgunit_text)) && (orgunit_text != null)) 
			    {
				    document.getElementById("orgunit_text").value = Trim(document.getElementById("orgunit_text").value.replace(regexp1, "'"));
				    document.getElementById("organization_text").value = Trim(document.getElementById("organization_text").value.replace(regexp1, "'"));
			    }
			    if (document.getElementById(UniqueID+"username").value == "")
			    {
				    alert (jsUsernameReq);
                    $ektron("ul li a[title=General]")[0].click();
				    document.getElementById(UniqueID+"username").focus();
				    return false;
			    }
				if((document.getElementById(UniqueID+"username").value.indexOf("<") >= 0)|| (document.getElementById(UniqueID+"username").value.indexOf(">") >= 0))
			    {
			      alert("Username cannot include '<', '>'");
			      return false;
		        }
                if (!specialCaseOverride)
                {
			        if (document.getElementById(UniqueID+"firstname").value == "")
			        {
				        alert (jsFirstnameReq);
				        document.getElementById(UniqueID+"firstname").focus();
				        return false;
			        }
			        if((document.getElementById(UniqueID+"firstname").value.indexOf("<") >= 0)|| (document.getElementById(UniqueID+"firstname").value.indexOf(">") >= 0))
			        {
			          alert("Firstname cannot include '<', '>'");
			          return false;
		            }
			        if (document.getElementById(UniqueID+"lastname").value == "")
			        {
				        alert (jsLastnameReq);
				        document.getElementById(UniqueID+"lastname").focus();
				        return false;
			        }	
			        if((document.getElementById(UniqueID+"lastname").value.indexOf("<") >= 0)|| (document.getElementById(UniqueID+"lastname").value.indexOf(">") >= 0))
			        {
			          alert("Lastname cannot include '<', '>'");
			          return false;
		            }	
                }
			    if ("undefined" != typeof document.getElementById(UniqueID+"displayname"))
			    {
			        document.getElementById(UniqueID+"displayname").value = Trim(document.getElementById(UniqueID+"displayname").value.replace(regexp1, "'"));
			        if (0 == document.getElementById(UniqueID+"displayname").value.length)
			        {
				        alert ("<asp:literal id="jsErrDisplayName" runat="server"/>");
				        document.getElementById(UniqueID+"displayname").focus();
				        return false;
				    }
				    if((document.getElementById(UniqueID+"displayname").value.indexOf("<") >= 0)|| (document.getElementById(UniqueID+"displayname").value.indexOf(">") >= 0))
			        {
			                alert("Displayname cannot include '<', '>'");
			                return false;
		            }	
			    }
			    var bpwd = false;
			    if (document.getElementById(UniqueID+"pwd").value == document.getElementById(UniqueID+"confirmpwd").value)
			    {
    	
				    if (typeof document.forms[0].hpwd == "object")
				    {
					    if (document.getElementById(UniqueID+"pwd").value == document.getElementById(UniqueID+"hpwd").value)
					    {
						    bpwd = true;
					    }
				    }
			    }
    			
			    if (("object"==typeof(orgunit_text)) && (orgunit_text != null)) 
			    {
				    document.getElementById("orgunit_text").value = Trim(document.getElementById("orgunit_text").value.replace(regexp1, "'"));
				    document.getElementById("ldapdomain_text").value = Trim(document.getElementById("ldapdomain_text").value.replace(regexp1, "'"));
				    document.getElementById("organization_text").value = Trim(document.getElementById("organization_text").value.replace(regexp1, "'"));
    				
				    if (document.getElementById("orgunit_text").value == "")
				    {
					    alert("For LDAP authentication, you are required to have an Organizational Unit with either a Domain or Organization.");
					    document.getElementById("orgunit_text").focus();
					    return false;
				    }else{
					    if (document.getElementById("organization_text").value == "" && document.getElementById("ldapdomain_text").value == "") {
						    alert ("Organization or Domain cannot be empty for given Organizational Unit. At least one is required.");
						    document.getElementById("organization_text").focus();
						    return false;
					    }
				    }
			    }
			    if (!bpwd)
			    {
			        var validationString = "<asp:literal id='passwordValidationString' runat='server' />";
			        var validationErrorString = "<asp:literal id='passwordValidationErrorString' runat='server' />";
        			
			        if (document.getElementById(UniqueID+"pwd").value == "")
			        {
				        alert ("<asp:literal id="jsPasswordReq" runat="server"/>");
				        $ektron('.tabContainer').tabs('select', 'General');
				        document.getElementById(UniqueID+"pwd").focus();
				        return false;
			        }
        			
			        if (document.getElementById(UniqueID+"pwd").value != document.getElementById(UniqueID+"confirmpwd").value)
			        {
				        alert ("<asp:literal id="jsPasswordInvalid" runat="server"/>");
				        $ektron('.tabContainer').tabs('select', 'General');
				        document.getElementById(UniqueID+"pwd").focus();
				        return false;
			        }
        			
			        var value = document.getElementById(UniqueID+"pwd").value;
        			
			        if (validationString.charAt(0) == '/')
			            validationString = validationString.substring(1);
			        if (validationString.length > 0 && validationString.charAt(validationString.length - 1) == '/')
			            validationString = validationString.substring(0, validationString.length - 1);
			        var re = new RegExp(validationString)
			        if (re.test(value)) {
                        alert(validationErrorString);
                        return false;
			        }
			    }

                var emailpattern=/^[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*$/;
                var emailVal = document.getElementById(UniqueID+"email_addr1").value;
			    if ((document.getElementById(UniqueID+"disable_msg")!=null) && (document.getElementById(UniqueID+"disable_msg").checked == false)) {
				    if (document.getElementById(UniqueID+"email_addr1").value == "") {
				    if(jsADIntegration){
					    alert(jsEmailBlank);
				    }else{		
					    alert(jsEmailAddress);
					    $ektron('.tabContainer').tabs('select', 'General');
					    document.getElementById(UniqueID+"email_addr1").focus();
				    }
					    return false;
				    }
				    if (!emailpattern.test(document.getElementById(UniqueID+"email_addr1").value)) {
				        if (jsADIntegration){
					        alert(jsEmailInvalid);			
				        }else{
					    alert(jsValidEmail);
					    $ektron("a[href|=#General]").click();
					    document.getElementById(UniqueID+"email_addr1").focus();	}		
						return false;
				    }
				    else if (emailVal.substring(emailVal.indexOf("@")).indexOf(".") < 0) {
				        alert(jsValidEmail);
				        $ektron("a[href|=#General]").click();
				        document.getElementById(UniqueID+"email_addr1").focus();
				        return false;
				    }
			    }else{
			        if (document.getElementById(UniqueID+"email_addr1").value != "") {
				        if (!emailpattern.test(document.getElementById(UniqueID+"email_addr1").value)) {
				            if (jsADIntegration){
					            alert(jsEmailInvalid);			
				            }else 					{
				                $ektron('.tabContainer').tabs('select', 'General');
					            alert("<asp:literal id="jsValidEmailOrBlank" runat="server"/>");				
					            document.getElementById(UniqueID+"email_addr1").focus();	
					        }		
					        return false;
				        }
			        }
			    }
			    var width = "";
			    var height = "";
    			
			    if($ektron('#chkFullScreen').length > 0) {			
			        if(!$ektron('#chkFullScreen')[0].checked) {
			            if (typeof document.forms[0].txtWidth == "object") {
				            width = Trim(document.getElementById(UniqueID+"txtWidth").value);
					            if((isNaN(width)) || (width < 400) || (width > 2400)) {
						            alert (jsInvalidPageWidth);
						            return false;
					            }
			            }
			            if (typeof document.forms[0].txtHeight == "object") {
				            height = Trim(document.getElementById(UniqueID+"txtHeight").value);
				            if((isNaN(height)) || (height < 300) || (height > 1800)) {
					            alert (jsInvalidPageHeight);
					            return false;
				            }
			            }
			        }
			    }
    			
			    if( typeof document.forms[0].avatar == "object") {
			        avatar = Trim(document.getElementById('avatar').value);

			        if( avatar.length > 0) {
			            if( !IsValidImage(avatar) ) {
			                alert("An image with one of the following extensions must be supplied for an avatar: " + ImageExtensions);
			                return false;
			            }
			        }
			    }
			    return true;
		    }
		    
		    function ConfirmDeleteUser() {
			    return confirm("<asp:literal id="jsDeleteUser" runat="server"/>");
		    }
    		
		    function ConfirmAddUserToGroup () {
			    return confirm("<asp:literal id="jsAddUser" runat="server"/>");
		    }
    		
		    function ConfirmActivateUser() {
	            return confirm("Are you sure you want to activate this user?");
            }
            
		    function ConfirmDeleteUserFromGroup () {
			    return confirm("<asp:literal id="jsDelUserFromGroup" runat="server"/>");
		    }
    		
		    function VerifyGroupName() {
			    var regexp1 = /"/gi;
			    document.getElementById(UniqueID+"UserGroupName").value = Trim(document.getElementById(UniqueID+"UserGroupName").value.replace(regexp1, "'"));
			    if (document.getElementById(UniqueID+"UserGroupName").value == "")
			    {
				    alert("<asp:literal id="jsUserGroupNameReq" runat="server"/>");
				    document.getElementById(UniqueID+"UserGroupName").focus();
				    return false;
			    }
			    return true;
		    }

		    function VerifyDeleteGroup() {
			    var bRet;
			    var deleteString = "<asp:literal id="jsDelUserGroup" runat="server"/>";
    			
			    if(typeof window.IsGroupPartOfSubscriptionProduct == 'function' && IsGroupPartOfSubscriptionProduct())
			        deleteString = "<asp:literal id="jsDelUserGroupSubscription" runat="server"/>";
    			
			    bRet = confirm(deleteString);
    			
			    if (bRet) {
				    DisplayHoldMsg(true);
			    }
			    return bRet;
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
		    
		    function Select_pwd(item) {
			    if (!g_already_in_pwd_event) {
				    g_already_in_pwd_event = true;
				    item.select();
				    //document.forms[0].pwd.onbeforeeditfocus = old;
				    g_already_in_pwd_event = false;
			    }
			    return false;
		    }
    		
		    function UnSelect_pwd(item) {
			    if (!g_already_in_pwd_event) {
				    g_already_in_pwd_event = true;
					    // this will remove the "select" state from the text:
					    var old = item.value;
					    item.value = old;
				    g_already_in_pwd_event = false;
			    }
		    }

		    function SubmitForm(FormName, Validate) {
			    var go = true;
			    if (typeof Ek_EUMFmValidateReqFields == "function")
			    {
				    go = Ek_EUMFmValidateReqFields(document.forms[0]);
				    if (go)
				    {
					    go = Ek_EUMFmValidate(document.forms[0]);
				    }
			    }
			    if (!go) {return false; }
			    if (Validate.length > 0) {
				    if (eval(Validate)) {
					    document.forms[0].submit();
					    return false;
				    }
				    else {
					    return false;
				    }
			    }
			    else {
				    document.forms[0].submit();
				    return false;
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
			    return string;
		    }

		    function CheckRadio(myType){
			    var found = 0;
			    for(i = 0; i < document.forms[0].elements.length; i++ ) {
				    if(document.forms[0].elements[i].type == "radio") {
					    if (document.forms[0].elements[i].checked){
						    found = 1;
						    break;
					    }
				    }
			    }
			    if (found == 0){
				    if (myType == 0){
					    alert(jsMakeSelection);
				    }
				    else {
					    alert(jsMakeSelection);
				    }
				    return false;
			    }
			    else{
				    return true;
			    }
		    }
    		
		    function CheckReturn(myType){
			    if (CheckRadio(myType)){
				    if (document.forms[0].rp.value == "3"){
					    if (!top.opener.closed) {
						    top.opener.document.forms[0].elements[document.forms[0].e1.value].value = document.forms[0].adusername.value;
						    top.opener.document.forms[0].elements[document.forms[0].e2.value].value = document.forms[0].addomain.value;
					    }	
					    top.close();
					    return false;						
				    }
			    }
		    }
    		
		    function SetUp(tempStr){
			    var tempArray = tempStr.split("_@_");
			    document.forms[0].adusername.value = tempArray[0];	
			    document.forms[0].addomain.value = tempArray[1];			
		    }
    		
    		
		    function CheckAll(){
			    var i;
			    var bChecked = document.forms[0].checkall.checked
			    i = 0;
			    for(i = 0; i < document.forms[0].elements.length; i++ ) {
				    if(document.forms[0].elements[i].type == "checkbox") {
					    document.forms[0].elements[i].checked = bChecked;
				    }
			    }							
		    }			

		    function SelectEmail(target) {
			    document.forms[0].elements[target].checked = !document.forms[0].elements[target].checked;
		    }
    		
		    function IsValidImage(filename) {
			    var ExtensionList = ImageExtensions;

			    if (ExtensionList.length > 0) {
				    var ExtensionArray = ExtensionList.split(",");
				    var FileExtension = filename.split(".");
				    for (var i = 0; i < ExtensionArray.length; i++) {
					    if (FileExtension[FileExtension.length - 1].toLowerCase() == Trim(ExtensionArray[i].toLowerCase())) {
						    return true;
					    }
				    }
				    return false;
			    }
		    }
	    //-->
        </script>
        <style type="text/css">
            #dvUserGroups p {padding:0em 1em 1em 1em;margin:0;font-weight:bold;color:#1d5987;}
            #dvUserGroups ul.userGroups {list-style:none;margin:0 0 0 1em;padding:0;border:1px solid #d5e7f5;}
            #dvUserGroups ul.userGroups li {display:block;padding:.25em;border-bottom:none;}
            #dvUserGroups ul.userGroups li.stripe {background-color:#e7f0f7;}
        </style>
    </head>
    <body>
        <asp:Literal ID="MakeEmailArea" runat="server" />
        <asp:Literal ID="CloseScriptJS" runat="server" />
        <form id="Form1" method="post" runat="server">
            <asp:PlaceHolder ID="DataHolder" runat="server"></asp:PlaceHolder>
        </form>
    </body>
</html>


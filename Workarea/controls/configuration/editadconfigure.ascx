<%@ Control Language="C#" AutoEventWireup="true" Inherits="editadconfigure" CodeFile="editadconfigure.ascx.cs" %>

<script language="javascript" type="text/javascript">
<!--
var UniqueID="<asp:literal id='jsUniqueID' runat='server'/>";
function IsBrowserIE() {
	// document.all is an IE only property
	return (document.all ? true : false);
}
function VerifyForm() {
	var MyUrl = "<%=(Request.ServerVariables["http_host"]).ToLower()%>";
		
	if (MyUrl.indexOf("demo.ektron.com") != -1) {
		alert("<%= (m_refMsg.GetMessage("js: alert demo.ektron.com detected")) %>");
		return false;
	}
	if (document.getElementById(UniqueID+"EnableADAuth").checked) {
		for( i = 0; i < document.forms[0].elements.length; i++ ) {
			if( document.forms[0].elements[i].type == "text" ) {
				var elementName = new String(document.forms[0].elements[i].name);
				if( elementName.indexOf("userpropvalue") != -1 ){
					if(document.forms[0].elements[i].value == "" ){
						alert("<%= (m_refMsg.GetMessage("js: alert user prop cannot be empty")) %>");
						document.forms[0].elements[i].focus();
						return false;
					}
				}
			}
		}
	}
	if (document.getElementById(UniqueID+"EnableLDAP").checked) {
		if(document.getElementById(UniqueID+"ServerText").value == ""){
			alert("<%= (m_refMsg.GetMessage("js: alert LDAP server empty")) %>");
			document.getElementById(UniqueID+"ServerText").focus();
			return false;
		}
		if(document.getElementById(UniqueID+"PortText").value == ""){
			alert("<%= "Port cannot be empty." %>");
			document.getElementById(UniqueID+"PortText").focus();
			return false;
		}
		if(document.getElementById(UniqueID+"OrgText").value == "" && document.getElementById(UniqueID+"LDAPDomainText").value == ""){
			alert("<%= "You must specify either an Organization or a Domain." %>");
			document.getElementById(UniqueID+"OrgText").focus();
			return false;
		}
//		if(document.getElementById(UniqueID+"OrgUnitText").value == ""){
//			alert("<%= (m_refMsg.GetMessage("js: alert org empty")) %>");
//			TR_OU.style.display = (true==IsBrowserIE()? "block":"table-row");
//			hideshowlabel.innerHTML="close";
//			return false;
//		}
	}
	if (document.getElementById(UniqueID+"EnableADInt").checked) {
		if ( (document.getElementById(UniqueID+"admingroupname").value == "") || (document.getElementById(UniqueID+"admingroupdomain").value == "") ){
			alert("<%= (m_refMsg.GetMessage("js: alert AD admin group required")) %>");
			return false;
		}
        if (!UsingADAdvanced()) {
		    if ((document.getElementById("domainname").options[document.getElementById("domainname").selectedIndex].value.toLowerCase() != document.getElementById(UniqueID+"admingroupdomain").value.toLowerCase()) && (document.getElementById("domainname").options[document.getElementById("domainname").selectedIndex].value != "") ) {
			    alert("<%= (m_refMsg.GetMessage("js: alert AD admin group match domain")) %>");
			    return false;			
		    }			
		    var bDomainFound = false;
		    for( i = 0; i < document.getElementById("domainname").options.length; i++ ){
			    if (document.getElementById(UniqueID+"admingroupdomain").value.toLowerCase() == document.getElementById("domainname").options[i].value.toLowerCase()){
				    bDomainFound = true;
				    break;
			    }
		    }
		    if (!bDomainFound){
			    alert("<%= (m_refMsg.GetMessage("js: alert AD admin group available domain")) %>");
			    return false;
		    }			
        }									
	} else if (!UsingADAdvanced() && (document.getElementById("domainname").options.length == 1) && (document.getElementById(UniqueID+"EnableADAuth").checked)) {	
	    // if domains == 1, thenwe don't have any domains (problem). Error out...
	    alert("<%= (m_refMsg.GetMessage("js: no ad domains")) %>");
	    return false;
	}
	return true;
}

function CheckLDAP(type, populateAttribute)
{
    var LDAPtxt = type;
    if (document.getElementById(UniqueID+"EnableLDAP").checked) {    
        if (Trim(LDAPtxt.length < 1))
        {
            LDAPtxt = document.getElementById(UniqueID+"drp_LDAPtype").options[document.getElementById(UniqueID+"drp_LDAPtype").selectedIndex].value;
        }
        switch (LDAPtxt)
        {
            case "AD" :
            document.getElementById(UniqueID+"OrgText").disabled = true;
            document.getElementById(UniqueID+"LDAPDomainText").disabled = false;
            if (populateAttribute) { document.getElementById(UniqueID+"txtLDAPAttribute").value = 'sAMAccountName'; }
            document.getElementById(UniqueID+"LDAP_SSL").disabled = false;
            break;
            case "NO" :
            document.getElementById(UniqueID+"OrgText").disabled = false;
            document.getElementById(UniqueID+"LDAPDomainText").disabled = true;
            if (populateAttribute) { document.getElementById(UniqueID+"txtLDAPAttribute").value = 'uid'; }
            document.getElementById(UniqueID+"LDAP_SSL").disabled = false;
            break;
            case "SU" :
            document.getElementById(UniqueID+"OrgText").disabled = false;
            document.getElementById(UniqueID+"LDAPDomainText").disabled = false;
            if (populateAttribute) { document.getElementById(UniqueID+"txtLDAPAttribute").value = 'uid'; }
            document.getElementById(UniqueID+"LDAP_SSL").disabled = false;
            break;
            case "OT" :
            document.getElementById(UniqueID+"OrgText").disabled = false;
            document.getElementById(UniqueID+"LDAPDomainText").disabled = false;
            if (populateAttribute) { document.getElementById(UniqueID+"txtLDAPAttribute").value = 'cn'; }
            document.getElementById(UniqueID+"LDAP_SSL").disabled = false;
            break;
        }
    }
}

function hideshowou()
{
    var container = document.getElementById("TR_OU");    
    var hideshowlabel = document.getElementById("hideshowlabel")
	if(container.style.display == "none")
	{
		fillou();
		container.style.display =(true==IsBrowserIE()? "block":"table-row");
		hideshowlabel.innerHTML="<%= m_refMsg.GetMessage("close title") %>";	
	}
	else
	{
		container.style.display = "none";
		hideshowlabel.innerHTML="expand";	
	}
}
function fillou()
{
	var ouxml = document.getElementById(UniqueID+"OrgUnitText").value;
	var arUnits = ouxml.split("</>");
	var newElem=document.createElement("OPTION");	
	var objSele=document.getElementById("selou");	
	objSele.length=0;
	for(var i = 0; i < arUnits.length; i++)
	{
		//document.getElementById("selou").add(opt);
		newElem=document.createElement("OPTION");
		newElem.value=arUnits[i];
		newElem.text=arUnits[i];
		
		if ((typeof(objSele.options.add)).toLowerCase() != 'undefined') {
			objSele.options.add(newElem);
		} else {
			objSele.options[objSele.options.length] = newElem;
		}
	}
	
	// remove any blank options
	for(var j = 0; j < document.getElementById("selou").options.length; j++)
	{
		if(document.getElementById("selou").options[j].value=="")
		{
			document.getElementById("selou").options[j] = null;
		}
	}	
}
function addou()
{
	var newElem=document.createElement("OPTION");	
	var objSele=document.getElementById("selou");	
	if(document.getElementById("addoutext").value != "")
	{
		newElem=document.createElement("OPTION");
		newElem.value=document.getElementById("addoutext").value;
		newElem.text=document.getElementById("addoutext").value;
		if ((typeof(objSele.options.add)).toLowerCase() != 'undefined') {
			objSele.options.add(newElem);
		} else {
			objSele.options[objSele.options.length] = newElem;
		}		
	}
	sethiddenou();

}
function editou()
{
    for(var i = 0; i < document.getElementById("selou").options.length; i++)
	{
		if(document.getElementById("selou").options[i].selected == true)
		{
		    var ouval = document.getElementById("selou").options[i].value;
		    document.getElementById("addoutext").value = ouval;    
			document.getElementById("selou").remove(i);
			break;
	    }
	}
	sethiddenou();	
}
function deleteou()
{
	for(var i = 0; i < document.getElementById("selou").options.length; i++)
	{
		if(document.getElementById("selou").options[i].selected == true)
			document.getElementById("selou").remove(i);
	}
	sethiddenou();	
}
function sethiddenou()
{
	document.getElementById(UniqueID+"OrgUnitText").value = "";
	for(var i = 0; i < document.getElementById("selou").options.length; i++)
	{
		document.getElementById(UniqueID+"OrgUnitText").value += document.getElementById("selou").options[i].value;
		document.getElementById(UniqueID+"OrgUnitText").value += "</>";
	}
}
function SubmitForm(FormName, Validate) {
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

function DoSearch(){
    if (document.getElementById(UniqueID+"EnableADAuth").checked){
        if (document.getElementById(UniqueID+"EnableADInt").checked == true) {
            PopUpWindow('users.aspx?action=MapCMSUserGroupToAD&groupid=1&f=0&e1=' + UniqueID + 'admingroupname&e2=' + UniqueID + 'admingroupdomain&rp=3','Summary',690,380,1,1)
        } else {
            alert("<%= (m_refMsg.GetMessage("js: alert no ad intr")) %>");
        }
    }
}

function UsingADAdvanced()
{
    return <%= (m_bADAdvanced).ToString().ToLower() %>;
}

function EnableAuthentication(){
	if (document.getElementById(UniqueID+"EnableADAuth").checked){
		document.getElementById(UniqueID+"EnableADInt").disabled = false;
		document.getElementById(UniqueID+"drpAutoAddType").disabled = false;
		// update for LDAP, can't select anything unless it is AD
		if (!UsingADAdvanced()) {
		    document.getElementById("domainname").disabled = false;
		}
		document.getElementById(UniqueID+"admingroupdomain").disabled = false;
		document.getElementById(UniqueID+"admingroupname").disabled = false;
		document.getElementById("userpropvalue1").disabled = false;
		document.getElementById("userpropvalue2").disabled = false;
		document.getElementById("userpropvalue3").disabled = false;
		//document.getElementById(UniqueID+"OrgUnitText").disabled = true;	
		document.getElementById(UniqueID+"OrgText").disabled = true;
		document.getElementById(UniqueID+"ServerText").disabled = true;
		document.getElementById(UniqueID+"drp_LDAPtype").disabled = true;
		document.getElementById(UniqueID+"txtLDAPAttribute").disabled = true;
		document.getElementById(UniqueID+"LDAP_SSL").disabled = true;
		document.getElementById(UniqueID+"PortText").disabled = true;
		document.getElementById(UniqueID+"LDAPDomainText").disabled = true;			
	}
	else if (document.getElementById(UniqueID+"EnableLDAP").checked){
		// update for LDAP, can't select anything unless it is AD
		//document.getElementById(UniqueID+"OrgUnitText").disabled = false;	
		document.getElementById(UniqueID+"OrgText").disabled = false;
		document.getElementById(UniqueID+"ServerText").disabled = false;
		document.getElementById(UniqueID+"drp_LDAPtype").disabled = false;
		document.getElementById(UniqueID+"txtLDAPAttribute").disabled = false;
		document.getElementById(UniqueID+"LDAP_SSL").disabled = false;
		document.getElementById(UniqueID+"PortText").disabled = false;	
		document.getElementById(UniqueID+"LDAPDomainText").disabled = false;	
		document.getElementById(UniqueID+"EnableADInt").checked = false;
		document.getElementById(UniqueID+"EnableADInt").disabled = true;
		document.getElementById(UniqueID+"EnableAutoUser").checked = false;
		document.getElementById(UniqueID+"EnableAutoUserToGroup").checked = false;				
		document.getElementById(UniqueID+"EnableAutoUser").disabled = true;
		document.getElementById(UniqueID+"EnableAutoUserToGroup").disabled = true;	
		document.getElementById(UniqueID+"drpAutoAddType").disabled = true;
		if (!UsingADAdvanced()) {
            document.getElementById("domainname").disabled = true;
        }
		document.getElementById(UniqueID+"admingroupdomain").disabled = true;
		document.getElementById(UniqueID+"admingroupname").disabled = true;	
		document.getElementById("userpropvalue1").disabled = true;
		document.getElementById("userpropvalue2").disabled = true;
		document.getElementById("userpropvalue3").disabled = true;
		CheckLDAP('', true);
	}
	else{
		document.getElementById(UniqueID+"EnableADInt").checked = false;
		document.getElementById(UniqueID+"EnableADInt").disabled = true;
		document.getElementById(UniqueID+"EnableAutoUser").checked = false;
		document.getElementById(UniqueID+"EnableAutoUserToGroup").checked = false;				
		document.getElementById(UniqueID+"EnableAutoUser").disabled = true;
		document.getElementById(UniqueID+"EnableAutoUserToGroup").disabled = true;
		document.getElementById(UniqueID+"drpAutoAddType").disabled = true;
		// update for LDAP, can't select anything unless it is AD
        if (!UsingADAdvanced()) {
		    document.getElementById("domainname").disabled = true;
        }
		document.getElementById(UniqueID+"admingroupdomain").disabled = true;
		document.getElementById(UniqueID+"admingroupname").disabled = true;
		document.getElementById("userpropvalue1").disabled = true;
		document.getElementById("userpropvalue2").disabled = true;
		document.getElementById("userpropvalue3").disabled = true;
		//document.getElementById(UniqueID+"OrgUnitText").disabled = true;	
		document.getElementById(UniqueID+"OrgText").disabled = true;
		document.getElementById(UniqueID+"ServerText").disabled = true;
		document.getElementById(UniqueID+"drp_LDAPtype").disabled = true;
		document.getElementById(UniqueID+"txtLDAPAttribute").disabled = true;
		document.getElementById(UniqueID+"LDAP_SSL").disabled = true;
		document.getElementById(UniqueID+"PortText").disabled = true;	
		document.getElementById(UniqueID+"LDAPDomainText").disabled = true;			
	}
}

function EnableIntegration(){
	if (!document.getElementById(UniqueID+"EnableADAuth").checked){	
		document.getElementById(UniqueID+"EnableADInt").checked = false;			
	}
	else{
		if (document.getElementById(UniqueID+"EnableADInt").checked){
			document.getElementById(UniqueID+"EnableAutoUser").disabled = false;
			document.getElementById(UniqueID+"EnableAutoUserToGroup").disabled = false;
		}
		else{
			document.getElementById(UniqueID+"EnableAutoUser").checked = false;
			document.getElementById(UniqueID+"EnableAutoUserToGroup").checked = false;				
			document.getElementById(UniqueID+"EnableAutoUser").disabled = true;
			document.getElementById(UniqueID+"EnableAutoUserToGroup").disabled = true;				
		}				
	}
}

function DisableAD(checkBox){
	if (!document.getElementById(UniqueID+"EnableADInt").checked){		
		if (checkBox == "EnableAutoUser"){
			if (document.getElementById(UniqueID+"EnableAutoUser").checked){
				document.getElementById(UniqueID+"EnableAutoUser").checked = false;
			}
			else{
				document.getElementById(UniqueID+"EnableAutoUser").checked = true;
			}		
		}
		else {
			if (document.getElementById(UniqueID+"EnableAutoUserToGroup").checked){
				document.getElementById(UniqueID+"EnableAutoUserToGroup").checked = false;
			}
			else{
				document.getElementById(UniqueID+"EnableAutoUserToGroup").checked = true;
			}										
		}
	}
}
//-->
</script>
<style type="text/css">
    .spacer5em {
        margin-bottom: .5em
    }
</style>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <!-- Version / Build / Service Pack -->
    <span id="versionNumber" runat="server"></span>
    <span id="buildNumber" runat="server"></span>
    <span id="servicePack" runat="server"></span>

    <div class="ektronTopSpace"></div>
    <div class="ui-widget" id="sync" runat="server">
        <div class="ui-state-highlight ui-corner-all" style="padding: 0 0.7em; margin-top: 20px;"> 
            <span class="ui-icon ui-icon-info" style="float: left; margin-right: 0.3em;"></span>
                <asp:Literal ID="ltr_status" runat="server"></asp:Literal>  
        </div>
        <div class="spacer5em"></div>
    </div>
    
    <!-- Active Directory Installed -->
    <fieldset>
        <legend><span id="installed" runat="server"></span></legend>    
	    <input title="Disable Active Directory and LDAP Authentication" type="radio" id="DisableAD" name="ADGroupSelect" value="disable_adauth" onclick="EnableAuthentication();" runat="server" /><%=m_refMsg.GetMessage("disable active directory")%>
	    <br />
	    <input title="Enable LDAP Authentication " type="radio" id="EnableLDAP" name="ADGroupSelect" value="enable_LDAP" onclick="EnableAuthentication();" runat="server" /><%=m_refMsg.GetMessage("enable LDAP")%>

        <div style="padding-left:30px;">
            <table class="ektronForm">
                <tr>
                    <td class="label" title="Type"><%=m_refMsg.GetMessage("generic type")%>:</td>
                    <td class="value"><asp:DropDownList ID="drp_LDAPtype" runat="server" /></td>
                </tr>
	            <tr>	            
		            <td class="label" title="LDAP Server"><%=m_refMsg.GetMessage("LDAP server label")%>:</td>
		            <td class="value"><input type="text" id="ServerText" name="ServerText" maxlength="255" runat="server"/></td>
	            </tr>
	            <tr>	            
		            <td class="label" title="Port"><%=m_refMsg.GetMessage("lbl port")%>:</td>
		            <td class="value"><input type="text" id="PortText" name="PortText" maxlength="255" runat="server"/></td>
	            </tr>
	            <tr>	            
		            <td class="label" title="Organization"><%=m_refMsg.GetMessage("org label")%>:</td>
		            <td class="value"><input type="text" id="OrgText" name="OrgText" maxlength="255" runat="server"/></td>
	            </tr>
	            <tr>	            
		            <td class="label" title="Domain"><%=m_refMsg.GetMessage("domain title")%>:</td>
		            <td class="value"><input type="text" id="LDAPDomainText" name="LDAPDomainText" maxlength="255" runat="server"/></td>
	            </tr>
	            <tr>	            
		            <td class="label" title="Attribute"><%=m_refMsg.GetMessage("lbl ldap attribute")%>:</td>
		            <td class="value"><asp:textbox id="txtLDAPAttribute" maxlength="20" runat="server" /></td>
	            </tr>
	            <tr>	            
		            <td class="label" title="Use SSL"><%=m_refMsg.GetMessage("lbl use ssl")%>:</td>
		            <td class="value"><input type="checkbox" id="LDAP_SSL" name="LDAP_SSL" runat="server"/></td>
	            </tr>
		        <tr>
			        <td class="label" title="Path"><%=m_refMsg.GetMessage("generic path")%>:</td>
			        <td>
				        <a title="Expand" id="hideshowlabel" name="hideshowlabel" href="#" onclick="hideshowou();return false;"><%=m_refMsg.GetMessage("lbl expand")%></a>
			        </td>
		        </tr>
		        <tr id="TR_OU" style="DISPLAY:none">
		            <td></td>
			        <td>
				        <table>
                            <tr>
                                <td></td>
                                <td>
                                    <%=m_refMsg.GetMessage("msg path separated")%>
                                    <br />
							        &nbsp;&nbsp;ou=subsub,ou=sub,ou=main 
							        <br />
							        &nbsp;&nbsp;ou=Amherst,ou=New Hampshire,o=US
						        </td>
                            </tr>
					        <tr>
						        <td width="50"><a title="Add" id="addlabel" href="#" onclick="addou();return false;"><%=m_refMsg.GetMessage("generic add title")%></a></td>
						        <td><input type="text" id="addoutext" name="addoutext" maxlength="255" size="50"/></td>
					        </tr>									
					        <tr>
						        <td>
                                    <a title="Edit" id="A1" href="#" onclick="editou();return false;"><%=m_refMsg.GetMessage("generic edit title")%></a>
                                    <br />
                                    <a title="Delete" id="deletelabel" href="#" onclick="deleteou();return false;"><%=m_refMsg.GetMessage("generic delete title")%></a></td>
						        <td>
						            <select multiple id="selou" name="selou" style="WIDTH: 335px;HEIGHT:100px"></select>
						        </td>
					        </tr>
				        </table>
			        </td>
		        </tr>
	        </table>
        </div>	        
	       
        <input title="Enable Active Directory Authentication " type="radio" id="EnableADAuth" name="ADGroupSelect" value="enable_adauth" onclick="EnableAuthentication();" runat="server"/>
	        <%= m_refMsg.GetMessage("enable active directory authentication") %>

        <div style="padding-left:30px;">
	        <input title="Enable Active Directory Authentication" type="checkbox" id="EnableADInt" name="EnableADInt" value="enable_ad" onclick="EnableIntegration();" runat="server"/>
		        <%=m_refMsg.GetMessage("enable active directory")%>
	        <br />
	        <div style="padding-left:20px;">
	            <input title="Enable automatic addition of user from AD" type="checkbox" id="EnableAutoUser" name="EnableAutoUser" value="enable_autouser" onclick="DisableAD('EnableAutoUser');" runat="server"/>
		            <%=m_refMsg.GetMessage("enable auto add user")%>
	            <br />
	            <input title="Enable automatic addition of user to groups" type="checkbox" id="EnableAutoUserToGroup" name="EnableAutoUserToGroup" value="enable_usergroup" onclick="DisableAD('EnableAutoUserToGroup');" runat="server"/>
		            <%=m_refMsg.GetMessage("enable auto user to group")%>
		    </div>
        </div>	       
        
        <input type="hidden" id="OrgUnitText" name="OrgUnitText" runat="server" maxlength="255" />
    </fieldset>

    <!-- AutoAddType -->
    <div class="ektronTopSpace"></div>
    <fieldset>
	    <legend id="lgd_autoAddType" class="ektronHeader" runat="server"></legend>   
        <table class="ektronForm">
            <tr>
                <td class="label"><span id="autoAddTypeProperty" runat="server"></span>:</td>
	            <td class="value"><asp:DropDownList ID="drpAutoAddType" runat="server" Enabled="false"></asp:DropDownList></td>
            </tr>
        </table>
    </fieldset>
    
    <!-- User Property Association -->
    <div class="ektronTopSpace"></div>
    <fieldset>
	    <legend id="userProperty" class="ektronHeader" runat="server"></legend>   
        <table class="ektronForm">
            <tr>
                <td style="padding-right:6px;"><span id="cmsProperty" runat="server"></span></td>
                <td><span id="activeDirectoryProperty" runat="server"></span></td>
            </tr>
            <asp:Literal ID="mapping_list" Runat="server" />
        </table>
    </fieldset>

    <!-- CMS Admistrator Group Association -->
    <div class="ektronTopSpace"></div>
    <fieldset>
	    <legend id="adminGroupMap" class="ektronHeader" runat="server"></legend>
	    <input type="text" id="admingroupname" name="admingroupname" maxlength="255" class="ektronTextMedium" runat="server"/>@<input type="text" id="admingroupdomain" name="admingroupdomain" maxlength="255" class="ektronTextMedium" runat="server"/> <span id="searchLink" runat="server"></span>
		<div class="ektronCaption">(<span id="adGroupName" runat="server"></span>@<span id="adDomain" runat="server"></span>)</div>
    </fieldset>

    <!-- Domain -->
    <div class="ektronTopSpace"></div>
	<table class="ektronForm">
		<tr>
			<td class="label" id="domain" runat="server"></td>
	        <td class="value" id="domainDropdown" runat="server"></td>
	    </tr>
	</table>
	
	<asp:Literal runat="server" ID="LDAPjs" />
	<input type="hidden" name="userpropcount" id="userpropcount" runat="server" />
</div>


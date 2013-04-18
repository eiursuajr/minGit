<%@ Control Language="C#" AutoEventWireup="true" Inherits="editconfiguration" CodeFile="editconfiguration.ascx.cs" %>

<script type="text/javascript">
    <!--//--><![CDATA[//><!--

		var UniqueID='<asp:literal id="jsUniqueID" runat="server"/>_';
		var jsContentLanguage='<asp:literal id="jsContentLanguage" runat="server"/>';
		function IsBrowserIE_Email() {
			// document.all is an IE only property
			return (document.all ? true : false);
		}
		function LoadChildPage() {

			if (IsBrowserIE_Email())
			{
				var frameObj = document.getElementById("ChildPage");
				frameObj.src = "blankredirect.aspx?SelectCreateContent.aspx?FolderID=0&rmadd=false&LangType="+jsContentLanguage+"&browser=0"

				var pageObj = document.getElementById("FrameContainer");
				pageObj.style.display = "";
				pageObj.style.width = "80%";
				pageObj.style.height = "80%";

			}
			else
			{
				// Using Netscape; cant use transparencies & eWebEditPro preperly
				// - so launch in a seperate pop-up window:
				PopUpWindow("SelectCreateContent.aspx?FolderID=0&rmadd=false&LangType="+jsContentLanguage+"&browser=1","SelectContent", 490,500,1,1);

			}
		}

		function ReturnChildValue(contentid,contenttitle,QLink, FolderID,LanguageID) {
			// take value, store it, write to display

			CloseChildPage();
			document.getElementById(UniqueID+"templatefilename").value = QLink.replace('<%=SITEPATH%>', '');
			document.getElementById(UniqueID+"folderId").value = FolderID;

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
		function AreYouSure() {
			var lMsg;
			if (document.forms.config.filesystemsupport.checked == true) {
				lMsg = '<%=(m_refMsg.GetMessage("library folder turn on confirmation"))%>';
			}
			else {
				lMsg = '<%=(m_refMsg.GetMessage("library folder turn off confirmation"))%>';
			}
			return confirm(lMsg);
		}

		function VerifyForm() {
			var MyUrl = '<%=(Request.ServerVariables["http_host"]).ToLower()%>';
			if (MyUrl.indexOf("demo.ektron.com") != -1) {
				alert('<%= m_refMsg.GetMessage("js: alert demo.ektron.com detected") %>');
				return false;
			}
			if (document.forms.config.EnableMessaging.checked == true || document.forms.config[UniqueID + "chkVerifyUserOnAdd"].checked == true) {
				document.forms.config.SystemEmaillAddr.value = Trim(document.forms.config.SystemEmaillAddr.value);
				if (document.forms.config.SystemEmaillAddr.value == "") {
					alert('<%= m_refMsg.GetMessage("js: alert enter email address") %>');
					$ektron("a[href='#dvGeneral']").click();
					document.forms.config.SystemEmaillAddr.focus();
					return false;
				}
				var atLocation = document.forms.config.SystemEmaillAddr.value.search("@");
				if ((atLocation == -1) || (atLocation == 0) || (atLocation == (document.forms.config.SystemEmaillAddr.value.length - 1))) {
					alert('<%= m_refMsg.GetMessage("js: alert enter valid email") %>');
					document.forms.config.SystemEmaillAddr.focus();
					return false;
				}
			}
			if (!VerifyLicense()) {
				return false;
			}
			var reg1 = /\,/gi;
			var reg2 = /\./gi;
			//document.forms.config.content_size.value = Trim(document.forms.config.content_size.value);
			//var contentSize = document.forms.config.content_size.value.replace(reg1, "");
			//contentSize = contentSize.replace(reg2, "");
			//debugger;  document.forms.config.summary_size.value = Trim(document.forms.config.summary_size.value);
			//var summarySize = document.forms.config.summary_size.value.replace(reg1, "");
			//summarySize = summarySize.replace(reg2, "");
			//if ((contentSize == 0) || (contentSize > 1000000)) {
			//	alert("<%=(m_refMsg.GetMessage("js: max content size error (sql)"))%>");
			//	document.forms.config.content_size.focus();
			//	return false;
			//}
			//else if ((contentSize == 0) || (contentSize > 1000000)) {
			//	alert("<%=(m_refMsg.GetMessage("js: max content size error (access)"))%>");
			//	document.forms.config.content_size.focus();
			//	return false;
			//}
//			if ((summarySize == 0) || (summarySize > 65000)) {
//				alert('<%=(m_refMsg.GetMessage("js: max summary size error"))%>');
//				document.forms.config.summary_size.focus();
//				return false;
//			}
			var regexp1 = /"/gi;
			document.getElementById("editconfiguration_username").value = Trim(document.getElementById("editconfiguration_username").value.replace(regexp1, "'"));
			if (document.getElementById("editconfiguration_username").value == "")
			{
				alert ('<% =(m_refMsg.GetMessage("js: alert username required")) %>');
				document.getElementById("editconfiguration_username").focus();
				return false;
			}
			if (document.getElementById("pwd").value == "")
			{
				alert ('<% =(m_refMsg.GetMessage("js: alert password required")) %>');
				document.getElementById("pwd").focus();
				return false;
			}
			if (document.getElementById("pwd").value.search('"') != -1) {
				alert('<%=m_refMsg.GetMessage("js: alert dquote invalid")%>');
				document.getElementById("pwd").focus();
				return false;
			}
			if (document.getElementById("pwd").value != document.getElementById("confirmpwd").value)
			{
				alert ('<% =(m_refMsg.GetMessage("js: alert user cannot confirm password")) %>');
				document.getElementById("pwd").focus();
				return false;
			}
			//document.forms.config.content_size.value = contentSize;
			//document.forms.config.summary_size.value = summarySize;

//			var width = "";
//			var height = "";
//			if (typeof document.getElementById("editconfiguration_txtWidth") == "object") {
//				width = Trim(document.getElementById("editconfiguration_txtWidth").value);
//					if((width < 400) || (width > 2400)) {
//						alert ("Work pages' width must be in the range from 400 to 2400.");
//						return false;
//					}
//			}
//			if (typeof document.getElementById("editconfiguration_txtHeight") == "object") {
//				height = Trim(document.getElementById("editconfiguration_txtHeight").value);
//				if((height < 300) || (height > 1800)) {
//					alert ("Work pages' height must be in the range from 300 to 1800.");
//					return false;
//				}
//			}
			return true;
		}

		function VerifyLicense() {
			regexp2 = /\*/gi;
			document.forms.config.license1.value = Trim(document.forms.config.license1.value);
			document.forms.config.license.value = Trim(document.forms.config.license.value);
			if (document.forms.config.license.value != "")
			{
				if (document.forms.config.license1.value != "") {
					newkeys = document.forms.config.license.value.split(",");
					oldkeys = document.forms.config.license1.value.split(",");
					for (var iLoop = 0; iLoop < oldkeys.length; iLoop++) {
						keyParts = oldkeys[iLoop].split("?");
						if (keyParts.length != 2) {
							alert('<%= m_refMsg.GetMessage("js: alert invalid license found") %>' + " " + oldkeys[iLoop]) + ".";
							return false;
						}
						for (var xLoop = 0; xLoop < newkeys.length; xLoop++) {
							newkeyArray = newkeys[xLoop].split("?");
							if (newkeyArray.length != 2) {
								alert('<%= m_refMsg.GetMessage("js: alert invalid license entered") %>');
								return false;
							}
							newkeyArray[1] = newkeyArray[1].replace(regexp2, "");
							keyParts[1] = keyParts[1].replace(regexp2, "");
							if (newkeyArray[1] == keyParts[1]) {
								newkeyArray[0] = newkeyArray[0].replace(regexp2, "");
								keyParts[0] = keyParts[0].replace(regexp2, "");
								if (newkeyArray[0] != keyParts[0]) {
									alert('<%= m_refMsg.GetMessage("js: alert license key modified") %>');
									return false;
								}
							}
						}
					}
					return true;
				}
				newkeys = document.forms.config.license.value.split(",");
				for (var xLoop = 0; xLoop < newkeys.length; xLoop++) {
					keyParts = newkeys[xLoop].split("?");
					if (keyParts.length != 2) {
						alert('<%= m_refMsg.GetMessage("js: alert invalid license entered") %>');
						return false;
					}
				}
			}
			return true;
		}
		function SubmitForm(FormName, Validate) {
			if (Validate.length > 0) {
				if (eval(Validate)) {
					document.forms[FormName].submit();
					return false;
				}
				else {
					return false;
				}
			}
			else {
				document.forms[FormName].submit();
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
//		function checkWordStlyes(){
//			if(document.getElementById("editconfiguration_styles_0").checked) {
//				//make sure the WORD stlyes peserve is unchecked.
//				if(document.getElementById("editconfiguration_word_styles").checked) {
//					document.getElementById("editconfiguration_word_styles").checked = false;
//					}
//				}
//			else {
//				alert("<%= (m_refMsg.GetMessage("js: alert remove styles recommended")) %>");
//			}
//		}
//
//		function WordStlyes(){
//			//word styles can't be enabled if the remove styles is enabled
//			if(document.getElementById("editconfiguration_word_styles").checked) {
//				//check the state of the Remove styles
//				if(document.getElementById("editconfiguration_styles_0").checked) {
//					alert("<%= (m_refMsg.GetMessage("js: alert cannot enable word styles")) %>");
//					document.getElementById("editconfiguration_word_styles").checked = false;
//				}
//			}
//		}

	//--><!]]>
</script>

<script type="text/javascript">
    <!--//--><![CDATA[//><!--
        Ektron.ready(function() {
            var tabsContainers = $ektron(".tabContainer");
            tabsContainers.tabs();
        });
    //--><!]]>
</script>

<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <asp:PlaceHolder ID="phTabs" runat="server">
                <ul>
                    <li><a title="<%=m_refMsg.GetMessage("general label")%>" href="#dvGeneral">
                        <%=m_refMsg.GetMessage("general label")%>
                    </a></li>
                    <li><a title="<%=m_refMsg.GetMessage("editor options label")%>" href="#dvEditor">
                        <%=m_refMsg.GetMessage("editor options label")%>
                    </a></li>
                    <li><a title="<%=m_refMsg.GetMessage("workarea options label")%>" href="#dvWorkarea">
                        <%=m_refMsg.GetMessage("workarea options label")%>
                    </a></li>
                </ul>
            </asp:PlaceHolder>
            <div id="dvGeneral">
                <span id="td_version" runat="server"></span><span id="td_buildnumber" runat="server">
                </span><span id="td_ServicePack" runat="server"></span>
                <div class="ektronTopSpace">
                </div>
                <table class="ektronGrid">
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("lbl license key")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("lbl license key")%>:
                        </td>
                        <td id="td_licensekey" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("lbl Module Licenses")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("lbl Module Licenses")%>:
                        </td>
                        <td id="td_modulelicense" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("setup default language prompt")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("setup default language prompt")%>
                        </td>
                        <td id="td_languagelist" runat="server">
                        </td>
                    </tr>
                    <tr id="trSettings" runat="server" visible="false">
		                <td class="label shortLabel"><%=m_refMsg.GetMessage("settings max content label")%>:</td>
		                <td id="td_maxcontent" runat="server"></td>
	                </tr>
                    <tr id="trSummary" runat="server" visible="false" >
                        <td class="label shortLabel">
                            <%=m_refMsg.GetMessage("settings max summary label")%>:
                        </td>
                        <td id="td_maxsummary" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("system email address label")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("system email address label")%>
                        </td>
                        <td id="td_email" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("lbl server type")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("lbl server type")%>
                        </td>
                        <td id="td_server_type" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("lbl Asynchronous Processor Location")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("lbl Asynchronous Processor Location")%>:
                        </td>
                        <td id="td_asynch_location" runat="server">
                        </td>
                    </tr>
                    <asp:Literal ID="PubPdf" runat="server" />
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("library filesystem folder prompt")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("library filesystem folder prompt")%>:
                        </td>
                        <td class="value" id="td_libfolder" runat="server">
                        </td>
                    </tr>
                </table>
                <div class="ektronTopSpace">
                </div>
                <div title="<%=m_refMsg.GetMessage("built in user label")%>" class="ektronHeader">
                    <%=m_refMsg.GetMessage("built in user label")%></div>
                <table class="ektronGrid">
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("username label")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("username label")%>
                        </td>
                        <td>
                            <input title="Username" type="text" id="username" name="username" maxlength="50"
                                onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("password label")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("password label")%>
                        </td>
                        <td id="TD_Pwd" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("confirm pwd label")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("confirm pwd label")%>
                        </td>
                        <td id="TD_Pwd2" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("account locked")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("account locked")%>:
                        </td>
                        <td id="TD_AccLocked">
                            <asp:Literal ID="accountLocked" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="dvEditor">
                <table class="ektronGrid">
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("styles label")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("styles label")%>
                        </td>
                        <td class="readOnlyValue">
                            <input title="Preserve MS-Word Styles " type="checkbox" id="word_styles" name="word_styles"
                                value="font" runat="server" /><%=m_refMsg.GetMessage("preserve word styles")%>
                            <br />
                            <input title="Preserve MS-Word Classes " type="checkbox" id="word_classes" name="word_classes"
                                value="font" runat="server" /><%=m_refMsg.GetMessage("preserve word classes")%>
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("fonts label")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("fonts label")%>
                        </td>
                        <td class="readOnlyValue">
                            <input title="Enable Font Buttons" type="checkbox" id="font_style" name="font_style"
                                value="font" runat="server" /><%=m_refMsg.GetMessage("enable font buttons")%>
                        </td>
                    </tr>
                    <%--
	                <tr>
		                <td><asp:RadioButtonList id="styles" RepeatDirection="Vertical" Runat="server" /></td>
		                <td>&nbsp;</td>
		                <td valign="top"><input type="checkbox" id="font_style" name="font_style" value="font" runat="server"><%=m_refMsg.GetMessage("enable font buttons")%></td>
	                </tr>
	                <tr>
		                <td>&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" id="word_styles" name="word_styles" value="font" onClick="javascript:WordStlyes();"
				                runat="server"><%=m_refMsg.GetMessage("preserve word styles")%></td>
		                <td>&nbsp;</td>
		                <td>&nbsp;</td>
	                </tr>
            --%>
                    <tr>
                        <td class="label shortLabel" title="<%=m_refMsg.GetMessage("accessibility label")%>">
                            <%=m_refMsg.GetMessage("accessibility label")%>
                        </td>
                        <td class="readOnlyValue">
                            <%-- <input type="radio" id="access_none" name="access" value="0" runat="server"/><label title="<%=m_refMsg.GetMessage("access none label")%>" for="access_none"><%=m_refMsg.GetMessage("access none label")%></label>
		                    <br />
		                    <input type="radio" id="access_loose" name="access" value="1" runat="server" /><label title="<%=m_refMsg.GetMessage("access loose label")%>" for="access_loose"><%=m_refMsg.GetMessage("access loose label")%></label>
		                    <br />
		                    <input type="radio" id="access_strict" name="access" value="2" runat="server" /><label title="<%=m_refMsg.GetMessage("access strict label")%>" for="access_strict"><%=m_refMsg.GetMessage("access strict label")%></label>--%>
                            <asp:RadioButtonList ID="access" runat="server">
                                <asp:ListItem id="access_none" runat="server" Value="0" />
                                <asp:ListItem id="access_loose" runat="server" Value="1" />
                                <asp:ListItem id="access_strict" runat="server" Value="2" />
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="dvWorkarea">
                <%--<div class="ektronHeader"><%=m_refMsg.GetMessage("lbl Work Page size")%>:</div>
                <table class="ektronGrid">
                    <tr>
                        <td class="label shortLabel"><%=m_refMsg.GetMessage("lbl Width")%>:</td>
                        <td><input type="text" id="txtWidth" name="txtWidth" class="ektronTextXXXSmall" runat="server" /> px</td>
                    </tr>
                    <tr>
                        <td class="label shortLabel"><%=m_refMsg.GetMessage("lbl height")%>:</td>
                        <td><input type="text" id="txtHeight" name="txtHeight" class="ektronTextXXXSmall" runat="server" /> px</td>
                    </tr>
                </table>

                <div class="ektronTopSpace"></div>--%>
                <div title="<%=m_refMsg.GetMessage("lbl landing page after login")%>" class="ektronHeader">
                    <%=m_refMsg.GetMessage("lbl landing page after login")%>:</div>
                <table class="ektronGrid">
                    <tr>
                        <td colspan="2" class="indent">
                            <%=SITEPATH%>
                            <input title="Landing Page After Login" type="text" id="templatefilename" name="templatefilename"
                                runat="server" />
                            <a title="<%= m_refMsg.GetMessage("generic select") %>" class="button buttonInline greenHover buttonCheckAll"
                                href="#" onclick="LoadChildPage();return true;">
                                <%=m_refMsg.GetMessage("generic select")%></a>
                            <br />
                            <input title="Set Smart Desktop As The Start Location In The Workarea" type="checkbox"
                                id="chkSmartDesktop" name="chkSmartDesktop" runat="server" /><%=m_refMsg.GetMessage("alt set smart desktop as the start location in the workarea")%>
                        </td>
                    </tr>
                    <%--<tr>
                        <td title="<%= m_refMsg.GetMessage("lbl display button text in the title bar") %>"
                            class="label shortLabel">
                            <%=m_refMsg.GetMessage("lbl display button text in the title bar")%>
                        </td>
                        <td class="value">
                            <input title="Display Button Text in the Title Bar" type="checkbox" id="disptitletext"
                                name="disptitletext" runat="server" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("force preferences msg")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("force preferences msg")%>
                        </td>
                        <td class="value">
                            <input title="Force preferences to all users" type="checkbox" id="forcePrefs" name="forcePrefs"
                                runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("verify user on add lbl")%>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("verify user on add lbl")%>
                        </td>
                        <td class="value">
                            <input title="Enable Verify Email" type="checkbox" id="chkVerifyUserOnAdd" name="chkVerifyUserOnAdd"
                                runat="server" />
                            <div class="ektronCaption">
                                <%=m_refMsg.GetMessage("verify user on add desc")%></div>
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("lbl Enable PreApproval Group") %>" class="label shortLabel">
                            <%=m_refMsg.GetMessage("lbl Enable PreApproval Group")%>
                        </td>
                        <td class="value">
                            <input title="Enable Preapproval Group" type="checkbox" id="chkEnablePreApproval"
                                name="chkEnablePreApproval" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<input type="hidden" name="folderId" id="folderId" runat="server" />
<input type="hidden" id="userid" name="userid" runat="server" />
<input type="hidden" name="access_def" id="access_def" runat="server" />

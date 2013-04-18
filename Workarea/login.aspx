<%@ Page Language="C#" AutoEventWireup="true" Inherits="login" CodeFile="login.aspx.cs"
    EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <meta http-equiv="Pragma" content="no-cache" />

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
            function GetLoginInfo()
            {
                var username = document.getElementById("username").value;
                var password = document.getElementById("password").value;

                return [username, password];
            }

            function SetLoginInfo()
            {
                try {
                    var loginInfo = GetLoginInfo();
                    Ektron.PrivateData.SetLoginInfo(loginInfo[0], loginInfo[1]);
                } catch(e) { }
                return true;
            }

            function ValidatePassword()
            {
                var password1 = document.getElementById('newpassword1').value;
                var password2 = document.getElementById('newpassword2').value;
                if (password1 != password2){
                    alert("<asp:literal id='passwordMismatchErrorString' runat='server' />");
                    return false;
                }
                else
                {
                    var validationString = "<asp:literal id='passwordValidationString' runat='server' />";
                    if (validationString && validationString.length > 0)
                    {
                        var errorMsgs = ValidateRegExMsgArray(password1, validationString);
                        if (errorMsgs.length > 0)
                        {
                            alert(errorMsgs[0]);
                            return false;
                        }
                    }
                }
                return true;
            }

            function ValidateRegExMsgArray(password, regexAndErrorMessages)
            {
                var errors = [];
                var regex, errorMessage;
                var parts = [];
                var raw = trimStart(regexAndErrorMessages, "[");
                raw = trimEnd(raw, "]");
                raw = raw.split("],[");

                for (var idx = 0; idx < raw.length; idx++)
                {
                    parts = raw[idx].split("/,");
                    regex = trimStart(parts[0], "/");

                    errorMessage = this.trimStart(trimStart(parts[1], ' '), '\\"');
                    errorMessage = this.trimEnd(errorMessage, '\\"');

                    var re = new RegExp(regex);
                    if (!re.test(password))
                        errors[errors.length] = errorMessage;
                }
                return errors;
            }

            function trimStart(text, toRemove){
                if (text.indexOf(toRemove) == 0 && text.length >= toRemove.length)
                    return text.substr(toRemove.length);
                return text;
            }

            function trimEnd(text, toRemove){
                if (text.length > toRemove.length && toRemove == text.substr(text.length - toRemove.length))
                    return text.substr(0, text.length - toRemove.length);
                return text;
            }
        //--><!]]>
    </script>

    <script type="text/javascript" src="java/jfunct.js">
    </script>

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
	        function setFocus() {
		        self.focus();
		        if((typeof(document.LoginRequestForm)).toLowerCase() != "undefined") {
		            var loginRequestForm = $ektron("#LoginRequestForm");
		            var loginRequestFormUsername = loginRequestForm.find("#username")
		            if (loginRequestForm.length > 0 && loginRequestFormUsername.length > 0)
		            {
		                loginRequestFormUsername.focus();
		            }
		        }
	        }

	        // Clears the authentication for DMS control
	        function clearAuth()
	        {
	            try{
	                document.execCommand("ClearAuthenticationCache");
	            }catch(err){
	                // do nothing
	            }
	        }
	        
	        $ektron(document).ready(function()
	        {
	            // add hover effects for the inputButtons
                $ektron(".inputButton").hover(
                    function()
                    {
                        $ektron(this).addClass("ui-state-hover");
                    },
                    function()
                    {
                        $ektron(this).removeClass("ui-state-hover");
                    }
                );
	        });
        //--><!]]>
    </script>

    <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
                html, body {height: 100%;}
                body {background: #E47E0F;}
                table {margin: .25em auto;}
                table td {padding: .2em;}
                .padTop {padding-top: 1em;}
                .padBottom {padding-bottom: .5em;}
                form {display: block; overflow: auto; height: 100%; position: relative;}
                #loginWrapper {padding: 0 .25em .25em; margin: .5em .25em; overflow: visible;}

                .ui-widget #LoginBtn .ui-icon,
                .ui-widget #changePasswordBtn .ui-icon,
                .ui-widget #btn_skip .ui-icon,
                .ui-widget #LogoutBtn .ui-icon
                {
                    left:.2em;margin:-8px 5px 0 0;position:absolute;top:50%;
                }

                .ui-widget #loginCancel .ui-icon,
                .ui-widget #changePasswordCancel .ui-icon,
                .ui-widget #logoutCancel .ui-icon
                {
                    left:.2em;margin:-8px 5px 0 0;position:absolute;top:50%;
                }
            /*]]>*/-->
        </style>
    <!--[if IE 6]>
        <style type="text/css">
            /* float clearing for IE6 */
            * html .ui-helper-clearfix{
              height: 1%;
              overflow: visible;
            }

            .ui-widget #LoginBtn .ui-icon,
            .ui-widget #loginCancel .ui-icon,
            .ui-widget #changePasswordCancel .ui-icon,
            .ui-widget #changePasswordBtn .ui-icon,
            .ui-widget #LogoutBtn .ui-icon,
            .ui-widget #logoutCancel .ui-icon,
            .ui-widget #btn_skip .ui-icon
            {
                top: 37.5%;
            }

        </style>
        <![endif]-->
</head>
<body onload="setFocus();">
    <form id="LoginRequestForm" method="post" runat="server">
        <div id="loginWrapper">
            <asp:Panel ID="LoginErrorPanel" runat="server" Visible="False" CssClass="ui-widget">
                <div class="ui-state-error ui-corner-all ui-helper-clearfix" style="padding: .25em .5em;
                    margin: .25em 0">
                    <span class="ui-icon ui-icon-alert" style="float: left; margin-right: 0.3em; margin-top: .2em">
                    </span>
                    <asp:Literal ID="ErrorText" runat="server" />
                </div>
            </asp:Panel>
            <asp:Panel ID="LoginRequestPanel" runat="server" Visible="False" CssClass="ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"
                Style="padding: 0.2em 0.2em 0">
                <div>
                    <div class="ui-widget-header ui-helper-clearfix ui-corner-all" style="padding: 0.25em">
                        <asp:Literal ID="loginlbl" runat="server" /></div>
                </div>
                <table class="fields">
                    <tfoot>
                        <tr>
                            <td colspan="2" class="right padTop padBottom">
                                <input type="submit" value="Login" onclick="SetLoginInfo()" style="position: absolute;
                                    top: -10000px;" />
                                <asp:LinkButton ID="LoginBtn" OnClientClick="SetLoginInfo()" CssClass="ui-state-default ui-corner-all inputButton"
                                    runat="server">
                                    <span class="ui-icon ui-icon-unlocked"></span>
                                    <asp:Literal ID="loginLoginText" runat="server" />
                                </asp:LinkButton>
                                <asp:LinkButton OnClientClick="self.close(); if ('function' == typeof parent.ektb_remove) {parent.ektb_remove();} return false;" ID="loginCancel" CssClass="ui-state-default ui-corner-all inputButton"
                                    runat="server">
                                    <span class="ui-icon ui-icon-circle-close"></span>
                                    <asp:Literal ID="loginCancelText" runat="server" />
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </tfoot>
                    <tbody>
                        <tr>
                            <td class="label padTop">
                                <label title="User" id="usernamelbl" for="username" runat="server" />
                            </td>
                            <td class="padTop">
                                <input title="Enter Username here" class="ektronTextXSmall" type="text" name="username"
                                    id="username" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <label title="Password" id="passwordlbl" for="password" runat="server" />
                            </td>
                            <td>
                                <input title="Enter Password here" class="ektronTextXSmall" type="password" name="pwd"
                                    id="password" runat="server" autocomplete="off" /></td>
                        </tr>
                        <tr id="TR_domain" runat="server" class="stripe">
                            <td title="Domain" id="domainlbl" class="label" runat="server">
                            </td>
                            <td>
                                <asp:DropDownList ToolTip="Select a Domain Name from the Drop Down Menu" ID="domainname" runat="server" /></td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="PasswordExpiredPanel" runat="server" Visible="False" CssClass="ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"
                Style="padding: 0.2em 0.2em 0">
                <asp:HiddenField ID="hdn_action" runat="server" />
                <div>
                    <div class="ui-widget-header ui-helper-clearfix ui-corner-all" style="padding: 0.25em">
                        <asp:Literal ID="passwordExpiredLabel" runat="server" /></div>
                </div>
                <div class="ektronCaption" style="margin: .25em">
                    <asp:Literal ID="passwordResetlbl" runat="server" /></div>
                <table class="fields">
                    <tr>
                        <td class="label">
                            <label title="New Password" id="newpassword1label" for="newpassword1" runat="server" />
                        </td>
                        <td>
                            <input class="ektronTextXSmall" title="Enter Password here" type="password" name="newpassword1"
                                id="newpassword1" /></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <label title="Confirm New Password" id="newpassword2label" for="newpassword2" runat="server" />
                        </td>
                        <td>
                            <input class="ektronTextXSmall" title="Enter Password here to Confirm" type="password"
                                name="newpassword2" id="newpassword2" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="right padTop padBottom">
                            <asp:LinkButton ID="changePasswordBtn" OnClientClick="if (ValidatePassword()) {document.getElementById('hdn_action').value = 'reset'; document.forms[0].submit();} return false;"
                                CssClass="ui-state-default ui-corner-all inputButton" runat="server">
                                <span class="ui-icon ui-icon-locked"></span>
                                <asp:Literal ID="changePasswordText" runat="server" />
                            </asp:LinkButton>
                            <asp:LinkButton OnClientClick="self.close(); return false;" ID="changePasswordCancel"
                                CssClass="ui-state-default ui-corner-all inputButton" Visible="false" runat="server">
                                <span class="ui-icon ui-icon-circle-close"></span>
                                <asp:Literal ID="changePasswordCancelText" runat="server" />
                            </asp:LinkButton>
                            <asp:LinkButton OnClientClick="document.getElementById('hdn_action').value = 'skip'; document.forms[0].submit(); return false;"
                                ID="btn_skip" CssClass="ui-state-default ui-corner-all inputButton" Visible="false"
                                runat="server">
                                <span class="ui-icon ui-icon-triangle-1-e"></span>
                                <asp:Literal ID="skipText" runat="server" />
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="LogoutPanel" runat="server" Visible="False" CssClass="ui-widget ui-widget-content ui-helper-clearfix ui-corner-all"
                Style="padding: 0.2em 0.2em 0">
                <div>
                    <div class="ui-widget-header ui-helper-clearfix ui-corner-all" style="padding: 0.25em">
                        <asp:Literal ID="logoutmsg" runat="server" /></div>
                    <div style="padding: 1em; text-align: center;">
                        <asp:LinkButton ToolTip="Logout" CssClass="ui-state-default ui-corner-all inputButton"
                            PostBackUrl="login.aspx?action=logout&i=19069" ID="LogoutBtn" runat="server"
                            OnClick="LogoutBtn_Click">
                            <span class="ui-icon ui-icon-locked"></span>
                            <asp:Literal ID="litLogoutButtonText" runat="server" />
                        </asp:LinkButton>
                        <asp:LinkButton OnClientClick="self.close(); return false;" ID="logoutCancel" CssClass="ui-state-default ui-corner-all inputButton"
                            runat="server">
                            <span class="ui-icon ui-icon-circle-close"></span>
                            <asp:Literal ID="logoutCancelText" runat="server" />
                        </asp:LinkButton>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="LoginSuceededPanel" runat="server" Visible="False">

                <script type="text/javascript">
                        <!--//--><![CDATA[//><!--
					        var test = "<%=HttpContext.Current.Session["RedirectLnk"]%>"
					        var frmRedirectLnk = "<%=HttpContext.Current.Session["fromLnkPg"]%>"
					        if (test.length == 0) {
						        //PopUpWindow("close.aspx?logout=true","Admin400",1,1,0,0);
						        window.open("close.aspx?logout=true","Admin400","toolbar=0,location=0,scrollbars=0,width=1,height=1");
						        self.close;
					        }
					        var szTmp = typeof(top.opener);
					        szTmp = szTmp.toLowerCase();
					        if (szTmp != "undefined") {
						        if (top.opener != null && !top.opener.closed && top.opener.location)
						        {
						        <%
						        if (m_template.Length > 0) {
						        %>
							        top.opener.location.href="<%=(m_refUserApi.SitePath + m_template)%>";
						        <%
						        } else {
						        %>
						            var UseSSL = "<%=m_refUserApi.UseSsl %>";
						            var PleaseRefresh = "<%=m_PleaseLoginMsg %>";
						            if( UseSSL == "False" ) {
						                //20854 - don't refresh the opener instead redirect. The refresh will post the data back on the server.
						                top.opener.location.href = (top.opener.location.href).replace(top.opener.location.hash,"");
						            }
						            else {
						                try {
						                    top.opener.location.href = (top.opener.location.href).replace(top.opener.location.hash,"");
						                    //top.opener.location.reload();
						                }
						                catch( exp ) {
						                    alert(PleaseRefresh);
						                }
						            }
						        <%
						        }
						        %>
						        }
					        }
					        if ((test.length == 0) || (frmRedirectLnk == "0")) {
							        self.close();
					        }
                        //--><!]]>
                </script>

            </asp:Panel>
        </div>
        <asp:Literal ID="ltr_olduser" runat="server"></asp:Literal>
        <asp:Literal ID="autologin" runat="server"></asp:Literal>
        <asp:Literal ID="WorkareaCloserJS" runat="server"></asp:Literal>
    </form>
</body>
</html>

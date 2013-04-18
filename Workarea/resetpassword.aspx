<%@ Page Language="C#" AutoEventWireup="false" CodeFile="resetpassword.aspx.cs" Inherits="resetpassword" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Reset Password</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table width="400px">			
			<tr><td nowrap="true"><h3 title="Reset Password">Reset Password:</h3></td></tr>
			<tr><td nowrap="true">
				<cms:Membership id="resetPassword" EnableCaptcha="true" RegisterButtonText="Reset" ResetButtonText="Clear" runat="server" DisplayMode="ResetPassword" UserSuccessMessage="Your password has been emailed to you."></cms:Membership>
			</td></tr>
			</table>
    </div>
    </form>
</body>
</html>
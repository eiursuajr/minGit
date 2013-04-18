<%@ Page validaterequest="False" Language="C#" AutoEventWireup="true" Inherits="activateuser" CodeFile="activateuser.aspx.cs" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Activate Account</title>
	</head>
	<body>
		<form id="frmActivate" name="frmActivate" method="post" runat="server">
			<table class="ektronGrid" width="400px">			
			<tr><td nowrap="true"><h3 title="Activate Account">Activate Account:</h3></td></tr>
			<tr><td nowrap="true">
				<cms:Membership id="AtivateUser" EnableCaptcha="true" RegisterButtonText="Activate" ResetButtonText="Clear" runat="server" DisplayMode="AccountActivate" UserSuccessMessage="Your account is now activated."></cms:Membership>
			</td></tr>
			</table>
		</form>
	</body>
</html>

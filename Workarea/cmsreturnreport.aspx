<%@ Page Language="C#" AutoEventWireup="false" %>

<%@ Import namespace="Ektron.Cms.Site" %>
<%@ Import namespace="Ektron.Cms" %>
<%@ Import namespace="Ektron.Cms.UI.CommonUI" %>

<%
	ApplicationAPI AppUI = new ApplicationAPI();
	EkSite objSite = new EkSite();
	objSite=AppUI.EkSiteRef;
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Status CMS</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
	</head>
	<body>
		<%
            Response.Write(objSite.CMSGetStatus());
		%>
	</body>
</html>


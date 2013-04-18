<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SiteLanguage.aspx.cs" Inherits="SiteLanguage" %>

<script language="JavaScript">
function pageReload() {
	<% if (AppUI.RedirectorOn) { %>
	var redirExtStr = '<%= AppUI.RedirectorManExt %>' ;
	var redirExt = redirExtStr.split(',') ;
	var parLoc = '' ;
	var isAliased = false ;
	// Verify that we have the parent window is an acceptable extension.
	for(var x=0;x<redirExt.length;x++) {
		parLoc = top.opener.location.toString() ;
		if(parLoc.indexOf(redirExt[x],1)>0) {
			isAliased = true ;
		}
	}
	// Send this location to the Redir Page, which will translate it to the correct page.
	if(isAliased==true) {
		top.opener.location = 'SiteLangRedir.aspx?parLoc=' + parLoc ;
		self.close() ;
	} else {
		top.opener.location.reload() ;
		self.close() ;
	}
	<% }else{ %>
		top.opener.location.reload();
		self.close() ;
	<%}%>
}
</script>

<html>
<head runat="server">
    <meta name="GENERATOR" content="Microsoft Visual Studio 6.0">
    <title></title>
</head>
<body onload="pageReload();">
    <p>
        &nbsp;</p>
</body>
</html>

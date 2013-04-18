<%@ Page Language="C#" AutoEventWireup="true" Inherits="explorerappsettings" CodeFile="explorerappsettings.aspx.cs" %>
<%if ( Request.QueryString["config"] == "1" ) {%>
</appPath><%=AppPath%></appPath>	
<%} else {%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head runat="server">
    <title> Explorer Application Settings </title> 
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <asp:literal id="StyleSheetJS" runat="server"/>
	<style>body{ font-family: ms sans serif; font-size: 10px; color: 969696}</style>
  </head>
  <body style="border:none;margin:0;"
      oncontextmenu="return false;"
      marginwidth="0"
	  marginheight="0"
	  leftmargin="0"
	  topmargin="0"
	  scroll="no">
	  
		<table width="100%" height="100%" ID="Table1">
		<tr>
		<td valign="middle" align="center">
		<div style="margin-top:5px;margin-bottom:5px;font-size:8px;width:100px;text-align:left;">
		Connecting
		<span id="progress">.</span>
		</div>
		<script language="javascript">
		
		var progress = document.getElementById( "progress" );

		setInterval( function() {
			var text = new String( progress.innerText );
			if( progress.innerText.length > 4 ) {
				progress.innerText = "";
			} else {
				progress.innerText += ".";
			}
		}, 500 );
		
		var appPath = "<%=AppPath%>";
		appPath = appPath.toLowerCase();
		appPath = "/" + appPath;
		// remove leading and trailing slashes
		appPath = appPath.substring(appPath.length-1, appPath.length) == "/" ? appPath.substring(0, appPath.length-1) : appPath;
		appPath = appPath.substring(0, 1) == "/" ? appPath.substring(1, appPath.length) : appPath;
		window.external.EktSetAppPath(appPath, "none");
		window.external.EktUseCurrentConfig();
		</script>
		</td>
		</tr>
		</table>
  </body>
</html>
<%}%>


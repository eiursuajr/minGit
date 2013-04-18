<%@ Page Language="C#" AutoEventWireup="true" Inherits="PreviewForm" CodeFile="PreviewForm.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Preview Form</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
		<link rel="stylesheet" type="text/css" href="<%=api.AppPath%>csslib/ektron.workarea.css"/>
		<link rel="stylesheet" type="text/css" href="<%=api.AppPath%>csslib/designformentry.css"/>
		<script language="JavaScript1.2" type="text/javascript" src="<%=api.AppPath%>java/platforminfo.js">
</script>
		<script language="JavaScript1.2" type="text/javascript" src="<%=api.AppPath%>java/designformentry.js">
</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
	
<script type="text/javascript">
<!--//--><![CDATA[//><!--
	function removeEditable(containingElement)
	{
		var oContainer = null;
		if ("object" == typeof containingElement && containingElement != null)
		{
			oContainer = containingElement;
		}
		else if ("string" == typeof containingElement && containingElement.length > 0)
		{
			if (typeof document.getElementById != "undefined")
			{
				oContainer = document.getElementById(containingElement);
			}
		}
		if (!oContainer) return;
		if ("undefined" == typeof oContainer.getElementsByTagName) return;

		var aryTagNames = ["label", "legend"];
		var aryElems;
		for (var iTagName = 0; iTagName < aryTagNames.length; iTagName++)
		{
			aryElems = oContainer.getElementsByTagName(aryTagNames[iTagName]);
			for (var i = 0; i < aryElems.length; i++)
			{
				aryElems[i].contentEditable = false;
			}
		}
	}
	setTimeout("removeEditable(document.body)", 100);
//--><!]]>
</script>
		</form>
	</body>
</html>


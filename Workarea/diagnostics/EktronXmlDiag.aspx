<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EktronXmlDiag.aspx.cs" Inherits="Workarea_diagnostics_EktronXmlDiag" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />  
    <title>Ektron.Xml Diagnostic Messages</title>

	<style type="text/css">
	h1
	{
		font: bold large Verdana;
	}
	p
	{
		font: normal medium Verdana;
	}
	.status
	{
		font-weight: bolder;
	}
	#cookie
	{
		font-family: "Courier New";
	}
	</style>
	<script language="JavaScript" type="text/javascript">
	<!--
	    function enableEktronXmlDiagMessages(enable) {
	        $ektron.cookie("Ektron.diagException", enable, { expires: (enable ? 3 : -1), path: "/" });
	        $ektron.cookie("Ektron.Xml.onexception", enable, { expires: (enable ? 3 : -1), path: "/" });
	        $ektron.cookie("Xml_xslTransform.onexception", enable, { expires: (enable ? 3 : -1), path: "/" });
	        updateStatus();
	    }

	    function enableEktronXmlXslTransformDiagMessages(enable) {
	        $ektron.cookie("Xml_xslTransform.onexception", enable, { expires: (enable ? 3 : -1), path: "/" });
	        updateStatus();
	    }

	    function updateStatus() {
	        var status = $ektron.cookie("Xml_xslTransform.onexception");
	        var statusMessage = Ektron.String.format("Diagnostic error message alerts for xslTransform exceptions are <span class=\"status\">{0}</span>.", status ? "enabled" : "disabled");
	        $ektron("#XslTransformStatus").html(statusMessage);
	        status = $ektron.cookie("Ektron.Xml.onexception");
	        statusMessage = Ektron.String.format("Diagnostic error message alerts for ALL Ektron.Xml exceptions are <span class=\"status\">{0}</span>.", status ? "enabled" : "disabled");
	        $ektron("#EktronXmlStatus").html(statusMessage);
	        $ektron("#cookie").html(document.cookie);
	    }
	// -->
	</script>
</head>
<body onload="updateStatus()">
    <h1>Ektron.Xml Diagnostic Messages</h1>
    
    <p>
		<span id="EktronXmlStatus"></span> 
		<button onclick="enableEktronXmlDiagMessages(true)">Enable</button>
		<button onclick="enableEktronXmlDiagMessages(false)">Disable</button>
    </p>
    
    <p>
		<span id="XslTransformStatus"></span> 
		<button onclick="enableEktronXmlXslTransformDiagMessages(true)">Enable</button>
		<button onclick="enableEktronXmlXslTransformDiagMessages(false)">Disable</button>
    </p>
    
    <p>document.cookie: <span id="cookie">&#160;</span></p>
</body>
</html>

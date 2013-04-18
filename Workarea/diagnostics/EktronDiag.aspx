<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EktronDiag.aspx.cs" Inherits="Workarea_diagnostics_EktronDiag" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />  
    <title>Ektron JavaScript Diagnostic Messages</title>

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
	    function enableEktronDiagMessages(enable) {
	        $ektron.cookie("Ektron.diagException", enable, { expires: (enable ? 3 : -1), path: "/" });
	        updateStatus();
	    }

	    function updateStatus() {
	        var status = $ektron.cookie("Ektron.diagException");
	        var statusMessage = Ektron.String.format("Diagnostic error messages for Ektron JavaScript are <span class=\"status\">{0}</span>.", status ? "enabled" : "disabled");
	        $ektron("#EktronAlertStatus").html(statusMessage);
	        $ektron("#cookie").html(document.cookie);
	    }
	// -->
	</script>
</head>
<body onload="updateStatus()">
    <h1>Ektron JavaScript Diagnostic Messages</h1>
   
    <p id="EktronAlertStatus">&#160;</p>
    
    <p>Diagnostic alert messages for Ektron JavaScript exceptions: 
    <button onclick="enableEktronDiagMessages(true)">Enable</button>
    <button onclick="enableEktronDiagMessages(false)">Disable</button>
    </p>
    
    <p>document.cookie: <span id="cookie">&#160;</span></p>
</body>

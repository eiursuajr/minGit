<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomDialog.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.CustomDialog" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title id="Title" runat="server">Custom Dialog</title>
</head>
<body>
<form id="form1" runat="server">
    <div class="Ektron_Dialog_Tabs_BodyContainer">
		<p title="Prompt">Prompt: <br />
		<asp:TextBox ToolTip="Enter Prompt here" runat="server"  type="text" name="txtContent" id="txtContent" width="200" /></p>
    </div>  
    <ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" /> 
</form>
<script language="javascript" type="text/javascript">
<!--
	function initField()
	{
	    // To retrieve arguments from the dialog.
	    var args = GetDialogArguments();
	    if (args)
	    {
	        document.getElementById("txtContent").value = args.content;
	    }
	}
	
	function insertField()
	{
	    var strContent = document.getElementById("txtContent").value;
	    var returnValue = 
	    {
			content : strContent
	    };
		CloseDlg(returnValue);	
	}
	
	Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("#txtContent").focus();
        BindOnRadWindowKeyDown();
    });	
//-->
</script>
</body>
</html>
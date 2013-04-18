<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WindowsMediaVideo.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.WindowsMediaVideo" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title title="Windows Media Video" id="Title" runat="server">Windows Media Video</title>
</head>
<body class="dialog">
    <form id="form1" runat="server">
    <div title="<%=GetMessage("lbl wmv prompt") %>" class="Ektron_Dialog_Tabs_BodyContainer">
		<p><asp:Literal ID="lblPrompt" runat="server" /><br />
		<asp:TextBox ToolTip="Enter the URL of a Windows Media Video (WMV) file" runat="server" type="text" name="txtURL" id="txtURL" width="200" TabIndex="1" /></p>
    </div>  
    <ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />  
    </form>
<script language="javascript" type="text/javascript">
<!--
	function initField()
	{
	    
        var args = GetDialogArguments();
	    if (args)
	    {
			// copy arguments here
	    }
	}
	
	function insertField()
	{
	    var strUrl = document.getElementById("txtURL").value;
	    var objWmv = 
	    {
			url: strUrl
	    };
		CloseDlg(objWmv);	
	}

    Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("#txtURL").focus();
        BindOnRadWindowKeyDown();
    });	
//-->
</script>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WindowsMediaVideo.aspx.cs" Inherits="Workarea_ewebeditpro_WindowsMediaVideo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Windows Media Video</title>
</head>
<body class="dialog">
    <form id="form1" runat="server">
    <div class="Ektron_Dialog_Tabs_BodyContainer">
		<p title="Enter the URL of a Windows Media Video (WMV) file below">Enter the URL of a Windows Media Video (WMV) file: <br />
		<asp:TextBox ToolTip="Enter the URL of a Windows Media Video (WMV) file here" runat="server" id="txtURL" width="200" /></p>
    </div>  
    <div class="Ektron_Dialogs_ButtonContainer Ektron_Dialogs_Buttons">
        <input type="button" title="Ok" name="btnOK" id="btnOK" class="Ektron_StandardButton" value="OK" onclick="insertField();" />
        &nbsp;
        <input type="button" title="Cancel" name="btnCancel" id="btnCancel" class="Ektron_StandardButton" onclick="CloseDlg(); return false;" value="Cancel" />
    </div>
    </form>
<script language="javascript" type="text/javascript">
<!--
	function CloseDlg()
	{
	    this.close();
	}
	
	function insertField()
	{
		var strUrl = document.getElementById("txtURL").value;
	    var sEditorName = "<asp:literal id="sEditorName" runat="server"/>";
	    var insert_wmv = '<embed name="MediaPlayer" type="application/x-mplayer2"';
	    insert_wmv += 'width="200" height="160" src="' + $ektron.htmlEncode(strUrl) + '"';
	    insert_wmv += ' title="' + $ektron.htmlEncode("Windows Media Video") + '"';
	    insert_wmv += ' autostart="0"></embed> '; 
        var objInstance = null;
        if (typeof this.parent.opener.eWebEditPro != "undefined" && this.parent.opener.eWebEditPro)
		{
		    objInstance = this.parent.opener.eWebEditPro.instances[sEditorName];
		}
		if (objInstance && objInstance.isEditor())
		{
		    objInstance.editor.pasteHTML(insert_wmv);
		} 
		CloseDlg();
	}
//-->
</script>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="signature.aspx.cs" Inherits="Workarea_signature"  %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Signature</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<script type="text/javascript">
 
	<!--//--><![CDATA[//><!--
	var ResourceText = 
	{
		sSignatureTooLong : "<asp:Literal id="sSignatureTooLong" runat="server"/>"
	};
	//--><!]]>
	</script>
	<script type="text/javascript">
 
	<!--//--><![CDATA[//><!--
	function WriteBack()
	{
		var hSig = self.parent.document.getElementById("content_html"); 
		if (hSig != null) 
		{ 
			var sSignature = ""; 
			var editor = Ektron.ContentDesigner.instances["cdSignature"];
			if (editor != null) 
			{ 
				sSignature = editor.getContent();
				if (sSignature.length > 500)
				{ 
					alert(ResourceText.sSignatureTooLong);
					return false;
				} 
			} 
			hSig.value = encodeURIComponent(sSignature); 
			var dSig = self.parent.document.getElementById("ek_dvsignature2"); 
			$ektron(dSig).html(sSignature);
		} 
		CloseWindow(); 
	} 

	function CloseWindow() 
	{ 
		setTimeout("parent.ektb_remove()", 500);
	} 

	function GetSignature() 
	{ 
		var editor = Ektron.ContentDesigner.instances["cdSignature"];
		if (editor != null) 
		{ 
			var dSig = self.parent.document.getElementById("ek_dvsignature2"); 
			var sSignature = Ektron.Xml.serializeXhtml(dSig.childNodes); 
			if (sSignature.length > 0)
			{
		        editor.setContent("document", sSignature); 
		    }
		} 
		else 
		{ 
			setTimeout("GetSignature();", 1000); 
		} 
	} 

	Ektron.ready(function()
	{
		setTimeout("GetSignature();", 2000); 
	});
	//--><!]]>
	</script> 
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
		<div style="margin-top: 4px; margin-bottom: 4px;">
			<input title="Save" type="button" id="cmdOk" name="cmdOk" onclick="WriteBack(); return false;" runat="server" />
			<input title="Cancel" type="button" id="close" onclick="CloseWindow();" runat="server" />
		</div>
		<ektron:ContentDesigner ID="cdSignature" Width="100%" runat="server"/>
    </form>
</body>
</html>


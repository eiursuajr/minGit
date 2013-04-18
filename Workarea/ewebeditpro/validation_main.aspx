<%@ Page Language="C#" AutoEventWireup="true" Inherits="validation_main" CodeFile="validation_main.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head runat="server">
    <title>validation_main</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
</head>
<body onload="JavaScript:self.focus()">
    <form id="frmValidation" name="frmValidation" method="post" runat="server">
    <input type="hidden" name="field_text" value="" />
    </form>

    <script language="JavaScript1.2" type="text/javascript" src="eweputil.js">
    </script>

    <script language="JavaScript1.2" type="text/javascript">
		document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
		var strTextData='';
		var objInstance = eWebEditProUtil.getOpenerInstance();
		strTextData = objInstance.editor.getSelectedHTML();
		// Remove CR LF
		strTextData = strTextData.replace(/\n/g, "");
		strTextData = strTextData.replace(/\r/g, "");
		var validation_id="0";
		var action="new";
		var editorName='<%=Request["editor_name"]%>';
		var content_id='<%=Request["id"]%>';
		var LangType='<%=Request["LangType"]%>';
		
		if (!((strTextData.indexOf('<input')==0 && (strTextData.indexOf('type=')==-1 || strTextData.indexOf('type="input')>-1 || strTextData.indexOf('type="password')>-1)) || strTextData.indexOf('<textarea')==0 || strTextData.indexOf('<select')==0) && 
			!((strTextData.indexOf('<INPUT')==0 && (strTextData.indexOf('type=')==-1 || strTextData.indexOf('type=input')>-1 || strTextData.indexOf('type=password')>-1)) || strTextData.indexOf('<TEXTAREA')==0 || strTextData.indexOf('<SELECT')==0)) {
			alert('Can not apply validation to the type of field selected.');
			self.close();
		}else{
			document.frmValidation.field_text.value=strTextData;
			var iPos = strTextData.indexOf("ekv");
			if ( iPos > 0 ){
				validation_id=FindEkvID(strTextData);
				action="Update";
			}else{
				validation_id.value="0";
				action="New";
			}
			document.frmValidation.action="validation.aspx?validation_id="+validation_id+"&action="+action+"&content_id="+content_id+"&editorName="+editorName+"&LangType="+LangType;
			document.frmValidation.submit();
		}
		function FindEkvID(ekvText) {
			var strLength = ekvText.length;
			var cTemp='';
			var Id='';
			var cQuote=0;
			var i = ekvText.indexOf("ekv");
			var new_ekvText = ekvText.substring(i,strLength)
			for(var i=0;i < new_ekvText.length; i++) {
				cTemp =  new_ekvText.charAt(i);
				if (cTemp=="\""){
					cQuote+=1;
				}
				if ((cQuote==1) && (cTemp!="\"")){
					Id+=cTemp
				}
			}
			return(Id);
		}
		
    </script>

</body>
</html>

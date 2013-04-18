<%@ Page Language="C#" AutoEventWireup="true" %>

<html>
<head runat="server">
    <title>Comment</title>
</head>
<body onload="JavaScript:self.focus()">
    <form action="changecomment.aspx?LangType=<%=Request["LangType"]%>&editorName=<%=Request.QueryString["editor_name"]%>&ref_id=<%=Request.QueryString["id"]%>"
    method="post" name="frmcomment">
    <input type="hidden" name="commentkey_text" value="" />
    <input type="hidden" name="commentkey_id" value="0" />
    <input type="hidden" name="comment_type" value="" />
    <input type="hidden" name="ref_id" value="2" />
    <input type="hidden" name="ref_type" value="<%=Request["ref_type"]%>" />
    <input type="hidden" name="LangType" value="<%=Request["LangType"]%>" />
    </form>
</body>

<script language="JavaScript1.2" type="text/javascript" src="eweputil.js">
</script>

<script language="JavaScript1.2" type="text/javascript">
		function isMyTag(objInstance, objXmlTag){
			var strTagName = "mycomment";
			return (objXmlTag != null && objXmlTag.IsValid() && strTagName == objXmlTag.getPropertyString("TagName") && objInstance.editor.IsTagApplied(strTagName));
		}
		var strCommentText = "";
		var objInstance = eWebEditProUtil.getOpenerInstance();
		if (objInstance && objInstance.isEditor()){
			var objXmlDoc = objInstance.editor.XMLProcessor();
			if (objXmlDoc != null){
				var objXmlTag = objXmlDoc.ActiveTag();
				if (isMyTag(objInstance, objXmlTag)){
					strCommentText = objXmlDoc.DecodeAttributeValue(objXmlTag.GetTagAttribute("comment"));
				}
			}
		}
		if (strCommentText > 0) {
			document.forms[0].elements["commentkey_id"].value = strCommentText;
		}else{
			document.forms[0].elements["commentkey_text"].value = strCommentText;
		}
		if(strCommentText ==""){
			document.forms[0].elements["comment_type"].value = "NEW";
		}
		document.frmcomment.submit();
</script>

</html>

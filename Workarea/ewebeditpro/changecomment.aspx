<%@ Page Language="C#" AutoEventWireup="true" validateRequest="false" CodeFile="changecomment.aspx.cs" Inherits="Workarea_ewebeditpro_changecomment" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>

<html>
	<head runat="server">
		<meta http-equiv="pragma" content="no-cache">
		<title runat="server"></title>
		<script language="JavaScript1.2" type="text/javascript" src="eweputil.js"></script>
		<script language="JavaScript" type="text/javascript" src="../java/toolbar_roll.js"></script>
		<script language="JavaScript" type="text/javascript" src="../java/empjsfunc.js"></script>
		<script language="JavaScript" type="text/javascript" src="../java/dateonlyjsfunc.js"></script>
		<script language="JavaScript" type="text/javascript">
		<!--
		var editorName = "<asp:Literal id="lEditorName" runat="server"/>"
		function submit_form(op)
		{
			var oForm = document.forms[0];
			var tid = oForm.ref_id.value;
			var ty = oForm.ty.value;
			document.getElementById("editorName").value = editorName;
			if ("insert" == op || "update" == op)
			{
				if ("update" == op)
				{
					oForm.action='changecomment.aspx?editorName=' + editorName + '&action=Update';
				}
				else
				{
					oForm.action = 'changecomment.aspx?editorName=' + editorName + '&action=Add&comment_id=0';
				}
				if (eWebEditProUtil.isOpenerAvailable())
				{
				    oForm.submit();
				}
				else
				{
				    alert('<asp:Literal id="EditorPageClosed" runat="server"/>');
				}
            }
			return false;
		}
		function DoSort(key)
		{
			document.forms[0].action="changecomment.aspx?editorName=" + editorName + "&orderby=" + replace_string(key," ","%20");
			document.forms[0].submit();
		}
		function replace_string(string,text,by)
		{
			var strLength = string.length, txtLength = text.length;
			if ((strLength == 0) || (txtLength == 0)) return string;
			var i = string.indexOf(text);
			if ((!i) && (text != string.substring(0,txtLength))) return string;
			if (i == -1) return string;
			var newstr = string.substring(0,i) + by;
			if (i+txtLength < strLength)
				newstr += replace_string(string.substring(i+txtLength,strLength),text,by);
			return newstr;
		}
		//-->
		</script>
	</head>
	<body bgcolor="#ffffff" bottommargin="0" leftmargin="5" rightmargin="0" topmargin="0">
		<form action="changecomment.aspx" name="contentcomment" method="Post" id="contentcomment" runat="server">
			<input type="hidden" name="OrderBy" value="<%= (HttpUtility.HtmlEncode(Request.QueryString["OrderBy"])) %>">
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tr><td>&nbsp;</td></tr>
				<%if( Action != "Edit" && Action != "Update") { %>
				<tr>
					<td>
						<table border="1" width="100%" cellspacing="0">
							<tr class=cal-header2>
								<%if(OrderBy == "") {%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=m_refMsg.GetMessage("lbl date/time")%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=m_refMsg.GetMessage("lbl added by")%></a></td><td width="65%"><%=m_refMsg.GetMessage("comments label")%></td>
								<%} else if( OrderBy == "date_created asc") {%>
									<td width="15%"><a href="javascript: DoSort('date_created desc');" title="Sort by Date/Time"><%=m_refMsg.GetMessage("lbl date/time")%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=m_refMsg.GetMessage("lbl added by")%></a></td><td width="65%"><%=m_refMsg.GetMessage("comments label")%></td>
								<%} else if(OrderBy == "date_created desc"){%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=m_refMsg.GetMessage("lbl date/time")%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=m_refMsg.GetMessage("lbl added by")%></a></td><td width="65%"><%=m_refMsg.GetMessage("comments label")%></td>
								<%} else if( OrderBy == "last_name asc"){%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=m_refMsg.GetMessage("lbl date/time")%></a></td><td width="20%"><a href="javascript: DoSort('last_name desc');" title="Sort by Last Name"><%=m_refMsg.GetMessage("lbl added by")%></a></td><td width="65%"><%=m_refMsg.GetMessage("comments label")%></td>
								<%} else if(OrderBy == "last_name desc"){%>
									<td width="15%"><a href="javascript: DoSort('date_created asc');" title="Sort by Date/Time"><%=m_refMsg.GetMessage("lbl date/time")%></a></td><td width="20%"><a href="javascript: DoSort('last_name asc');" title="Sort by Last Name"><%=m_refMsg.GetMessage("lbl added by")%></a></td><td width="65%"><%=m_refMsg.GetMessage("comments label")%></td>
								<%}%>
							</tr>
							<asp:Literal ID="CommentListHtml" runat="server"></asp:Literal>

						</table>
					</td>
				</tr>
				<%}%>
				<tr><td>&nbsp;</td></tr>
				<% 	if (Action.ToString().Length == 0 || "Edit" == Action)
        {%>
				<tr>
					<td class="input-box-text" title="Enter what would you like to say here"><%=m_refMsg.GetMessage("comments label")%>:</td>
				</tr>
				<tr>
					<td>
						<ektron:ContentDesigner ID="CommentEditor" Width="450px" Height="200px"
							Toolbars="Minimal" AllowFonts="true" AllowScripts="true" ShowHtmlMode="false" runat="server" />
					</td>
				</tr>
				<tr><td>&nbsp;</td></tr>
				<tr>
					<td>
					<%if (Action == "Edit")
                     {%>
						<input title="Click here to Update the folder" type=button name="btn_submit" value="<%= m_refMsg.GetMessage("alt update button text")%>" onclick="return submit_form('update');">
					<%}
                        else
                     {%>
						<input title="Insert Comment" type=button name="btn_submit" value="<%= m_refMsg.GetMessage("btn insert")%>" onclick="return submit_form('insert');">
					<%}%>
					&nbsp;&nbsp;<input title="Close" type=button name="btn_cancel" value="<%= m_refMsg.GetMessage("close title")%>" onclick="top.close()"></td>
				</tr>
				<% }
                     else
                 { %>
				<tr>
					<td><input title="Cancel" type=button name="btn_cancel" value="Close" onclick="top.close()">
					<input type="hidden" name="commenttext" value="<%=Server.HtmlEncode(CommentText)%>" /></td>
				</tr>
				<%  } %>
			</table>
			<input type="hidden" name="netscape" Onkeypress="javascript:return CheckKeyValue(event,'34');" />
			<input type="hidden" name="comment_id" value="<%=CommentId%>" />
			<input type="hidden" name="commentkey_id" value="<%=CommentKeyId%>" />
			<input type="hidden" name="ref_id" value="<%=RefId%>" />
			<input type="hidden" name="LangType" value="<%=ContentLanguage%>" />
			<input type="hidden" name="ref_type" value="<%=RefType%>" />
			<input type="hidden" name="comment_type" value="<%=CommentType%>" />
			<input type="hidden" name="user_id" value="" />
			<input type="hidden" name="editorName" id="editorName" value="" />
			<input type="hidden" name="orderyby" value="<%=OrderBy%>" />
			<input type="hidden" name="ty" value="<%=(HttpUtility.HtmlEncode(Request.QueryString["ty"]))%>" />
		</form>
	</body>
</html>
<script Language="JavaScript1.2" type="text/javascript">
<!--
var CommentText =
{
	Comment:		"Comment"
,	OK: 			"OK"
,	Cancel:			"Cancel"

,	ErrPageClosed:	"Unable to save changes. The editor page has been closed."
}
function isMyTag(objInstance, objXmlTag)
{
	var strTagName = "mycomment";
	return (objXmlTag != null && objXmlTag.IsValid() && strTagName == objXmlTag.getPropertyString("TagName") && objInstance.editor.IsTagApplied(strTagName));
}


function insertElement()
{
	if (!eWebEditProUtil.isOpenerAvailable())
	{
		alert(CommentText["ErrPageClosed"]);
	}
	else
	{
		var objInstance = eWebEditProUtil.getOpenerInstance();
		if (objInstance)
		{
		    var objXmlDoc = objInstance.editor.XMLProcessor();
		    var objXmlTag = null;
		    if (objXmlDoc != null)
		    {
			    objXmlTag = objXmlDoc.ActiveTag();
		    }
		    if (objXmlDoc != null && isMyTag(objInstance, objXmlTag))
		    {
			    objXmlTag.SetTagAttribute("comment", objXmlDoc.EncodeAttributeValue(<%=CommentKeyId%>));
			    objXmlDoc.ApplyTag(objXmlTag);
		    }
		    else
		    {
			    var strHTML = '<mycomment comment="' + objXmlDoc.EncodeAttributeValue(eWebEditProUtil.HTMLEncode(<%=CommentKeyId%>)) + '" />';
			    objInstance.editor.pasteHTML(strHTML);
		    }
		}
		try
		{
			top.close();
		}
		catch (e)
		{
			// ignore
		}
	}
}
//-->
</script>
<%if ((Action == "Add" && Flag == true && CommentType == "NEW") || (ResetCommentTag == true)){%>
        <script language="JavaScript" type="text/javascript">
        <!--
        insertElement();
        document.forms[0].comment_type.value="";
        //-->
        </script>
<% } else if("Add" == Action) { %>
        <script language="JavaScript" type="text/javascript">
        <!--
        try
        {
	        top.close();
        }
        catch (e)
        {
	        // ignore
        }
        //-->
        </script>
<%}%>

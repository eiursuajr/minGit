<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="TaskComment"
    CodeFile="TaskComment.aspx.cs" %>

<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <meta http-equiv="pragma" content="no-cache" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="csslib/ektron.workarea.css" />
    <script type="text/javascript" src="ewebeditpro/eweputil.js"></script>
    <script type="text/javascript" src="java/toolbar_roll.js"></script>
    <script type="text/javascript" src="java/empjsfunc.js"></script>
    <script type="text/javascript" src="java/dateonlyjsfunc.js"></script>
    <script type="text/javascript">
		<!--
			var PubOption;
			function submit_form(op) {
				op = op.toLowerCase();
				var tid=document.forms[0].ref_id.value;
				var ty = document.forms[0].ty.value;
				var blnHaveData = false;
				var editor = Ektron.ContentDesigner.instances["commenttext"];
				var strTempText = editor.getContent();   
				if (strTempText.length > 0)
				{
					blnHaveData = true;
				}

				if (op=="insert")
				{
					if(blnHaveData)
					{
						document.forms.taskcomment.action="taskcomment.aspx?action=Add&comment_id=0";
						if(window.top.opener && window.top.opener.closed)
						{
							alert('Unable to save changes.  The task page has been closed.');
						}
						else
						{
							__doPostBack("btnSubmit",""); 
						}
					}
					else
					{
						alert('Comments not specified!');
					}
					return false;
				}
				else if(op=="update")
				{
					if(blnHaveData)
					{
						document.forms.taskcomment.action="taskcomment.aspx?action=Update";
						if(window.top.opener && window.top.opener.closed)
						{
							alert('Unable to save changes.  The task page has been closed.');
						}
						else
						{
							__doPostBack("btnSubmit","");
						}
					}
					else
					{
						alert('Comments not specified!');
					}
					return false;
				
				}
				return false;
			}
			function DoSort(key)
			{
				document.taskcomment.action="taskcomment.aspx?orderby="+key;
				document.taskcomment.submit();
			}
		//-->
    </script>
    <style type="text/css">
        a.buttonCommentAdd
        {
            background-image: url(Images/ui/icons/commentAdd.png);
            background-position: .6em center;
        }
    </style>
    <script type="text/javascript" src="java/stylehelper.js"></script>
    <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
</head>
<body>
    <form action="taskcomment.aspx" id="taskcomment" method="post" runat="server">
    <div class="ektronPageContainer ektronPageInfo">
    <asp:PlaceHolder ID="ltr_sig" runat="server" />
        <div class="ektronTopSpace">
        </div>
        <a title="Submit" href="#" class="button buttonInline greenHover buttonCommentAdd"
            name="btnSubmit" id="btnSubmit" type="button" onclick="javascript:return submit_form($ektron('#ltrSubmit')[0].innerHTML);">
            <asp:Label ID="ltrSubmit" runat="server" />
        </a>&nbsp;&nbsp; <a type="button" title="Cancel" id="btnCancel" class="button buttonInline redHover buttonClear"
            value="Close" onclick="javascript:top.close()">
            <asp:Literal runat="server" ID="ltrCancel" />
        </a>
    </div>
    <input type="hidden" name="netscape" id="netscape" onkeypress="javascript:return CheckKeyValue(event,'34');" />
    <input type="hidden" name="comment_id" id="comment_id" value="<%=CommentId%>" />
    <input type="hidden" name="commentkey_id" id="commentkey_id" value="<%=CommentKeyId%>" />
    <input type="hidden" name="ref_id" id="ref_id" value="<%=RefId%>" />
    <input type="hidden" name="ref_type" id="ref_type" value="<%=RefType%>" />
    <input type="hidden" name="ty" id="ty" value="<%=ActionType%>" />
    <input type="hidden" name="OrderBy" value="<%=OrderBy%>" />
    </form>
</body>
</html>

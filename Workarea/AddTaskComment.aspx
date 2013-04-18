<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddTaskComment.aspx.cs" Inherits="AddTaskComment" %>

<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <asp:PlaceHolder ID="phJScript" runat="server">
        <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
        <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
        <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <meta http-equiv="Pragma" content="no-cache" />
        <script type="text/javascript" src="ewebeditpro/eweputil.js"></script>
        <script type="text/javascript" src="java/toolbar_roll.js"></script>
        <script type="text/javascript" src="java/empjsfunc.js"></script>
        <script type="text/javascript" src="java/dateonlyjsfunc.js"></script>
        <title><%=AppName + " Comments"%></title>
        <script type="text/javascript">
	<!--//--><![CDATA[//><!--
	    function IsBrowserIE() 
	    {
		    // document.all is an IE only property
		    return (document.all ? true : false);
	    }
		function submit_form(op) 
		{
			var blnHaveData = false;
			var editor = Ektron.ContentDesigner.instances["commenttext"];
			var strContent = editor.getContent();
//		    document.getElementById("commenttext").value = editor.getContent();   
//		    if (document.getElementById("commenttext").value.length > 0)
		    if (strContent.length > 0)
		    {
				blnHaveData = true;
			}
			if (op=="insert")
			{
					if(blnHaveData)
					{
						document.forms[0].action="AddTaskComment.aspx?action=Add&comment_id=0&id="+document.getElementById("cid").value+"&actionName="+document.getElementById("actionName").value+"&fldid="+document.getElementById("fldid").value+"&page="+document.getElementById("page").value+"&LangType=<%=Request.QueryString["LangType"]%>";
						if(window.top.opener && window.top.opener.closed)
						{
							alert('Unable to save changes.  The task page has been closed.');
						}
						else 
						{
							document.forms[0].submit();
							CloseWindow();
						}
					}
					else 
					{
						alert('Comments not specified!');
					}
					return false;
			}
		}
		function DoSort(key)
		{
			document.addtaskcomment.action="AddTaskComment.aspx?orderby="+key;
			document.addtaskcomment.submit();
		}
		function CloseWindow()
		{
			if (IsBrowserIE()) 
			{
				if (typeof parent.ReturnChildValue != "undefined")
				{
				parent.ReturnChildValue("action=" + document.getElementById("actionName").value + "&id=" + document.getElementById("cid").value + "&fldid=" + document.getElementById("fldid").value + "&page=" + document.getElementById("page").value );
			}
			}
			else 
			{
				if (typeof top.opener.ReturnChildValue != "undefined")
				{
				top.opener.ReturnChildValue("action=" + document.getElementById("actionName").value + "&id=" + document.getElementById("cid").value + "&fldid=" + document.getElementById("fldid").value + "&page=" + document.getElementById("page").value );
				close();
			}
		}
		}
	//--><!]]>
        </script>

    </asp:PlaceHolder>
    <asp:Literal ID="ltrScript" runat="server"></asp:Literal>
</head>
<body bgcolor="#ffffff" bottommargin="0" leftmargin="5" rightmargin="0" topmargin="0">
    <form id="addtaskcomment" name="addtaskcomment" runat="server">
    <input type="hidden" name="OrderBy" value="<%= (Request.QueryString["OrderBy"]) %>"
        id="OrderBy" />
    <div>
        <div>
            <label for="commenttext" class="input-box-text" title="Enter Task Comments in the text area below">
                <asp:Literal ID="ltrComments" runat="server"></asp:Literal></label>
        </div>
        <div>
            <asp:PlaceHolder ID="ltr_sig" runat="server" />
        </div>
        <div>
            <input title="Insert" type="button" name="btn_submit" id="" value="Insert" onclick="return submit_form('insert');" />
            &nbsp;&nbsp;<input title="Close" type="button" name="btn_cancel" id="" value="Close"
                onclick="CloseWindow()" />
        </div>
    </div>
    <input type="hidden" name="netscape" onkeypress="javascript:return CheckKeyValue(event,'34');"
        id="netscape" />
    <input type="hidden" name="comment_id" value="<%=Request.QueryString["Comment_Id"]%>"
        id="comment_id" />
    <input type="hidden" name="commentkey_id" value="<%=Request.QueryString["commentkey_id"]%>"
        id="commentkey_id" />
    <input type="hidden" name="ref_type" value="T" id="ref_type" />
    <input type="hidden" name="user_id" value="" id="user_id" />
    <input type="hidden" name="cid" value="<%=Request.QueryString["id"]%>" id="cid" />
    <input type="hidden" name="ty" value="<%=(Request.QueryString["ty"])%>" id="ty" />
    <input runat="server" type="hidden" id="actionName" name="actionName" value="" />
    <input type="hidden" name="fldid" value="<%=(Request.QueryString["fldid"])%>" id="fldid" />
    <input type="hidden" name="page" value="<%=(Request.QueryString["page"])%>" id="page" />
    </form>
    <asp:Literal ID="ClosePanel" runat="server" />
</body>
</html>

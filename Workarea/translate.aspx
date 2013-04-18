<%@ Import Namespace="Ektron.Cms.Site" %>
<%@ Import Namespace="Ektron.Cms.Content" %>
<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="Ektron.Cms.UI.CommonUI" %>
<%@ Import Namespace="Ektron.Cms.Common" %>

<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="translate"
    CodeFile="translate.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<script language="cs" runat="server">
    ApplicationAPI AppUI = new ApplicationAPI();
    const int ALL_CONTENT_LANGUAGES = -1;
    const int CONTENT_LANGUAGES_UNDEFINED = 0;
    object currentUserID;
    int EnableMultilingual;
    object msgs;
    object gtMsgObj;
    object gtObj;
    object gtMess;
    object mylang;
    string sitePath;
    string AppName;
    string AppeWebPath;
    string AppPath;
    string AppImgPath;
    EkMessageHelper MsgHelper;
    int ContentLanguage;

    public bool IsCMS300()
    {
        bool returnValue;
        returnValue = true;
        return returnValue;
    }
</script>

<html>
<head runat="server">
    <title id="pageTitle" runat="server"></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">

    <script type="text/javascript" language="javascript" src="ContentDesigner/RadWindow.js">
    </script>

    <script language="javascript" type="text/javascript">
<!--
InitializeRadWindow();
function pasteContent() 
{
    var isContentDesigner = false;
    try
    {
        var args = GetDialogArguments();
        if(args)
        {
            if(args.content)
            {
                isContentDesigner = true;
            }
        }
    }
    catch(e)
    {
        isContentDesigner = false;
    }
	if(isContentDesigner)
	{
	    var retValue = document.getElementById("content").value;
	    CloseDlg(retValue);
	}
	else if (top.opener && top.opener.eWebEditPro)
	{
		var objInstance = top.opener.eWebEditPro.instances["<%= htmleditor %>"];
		if (objInstance)
		{
			var translatedContent = document.getElementById("displaycontent").innerHTML;
			objInstance.load(translatedContent);
			top.close();
		}
		else
		{
			alert("Could not find editor '<%= htmleditor%>'.");
		}
	}
	else
	{
		alert("This page must be opened by a web page that contains the editor.");
	}
}
// -->
    </script>

    <style type="text/css">
        body
        {
            background: #fff;
        }
    </style>
</head>
<body>
    <p align="center">
        <input title="Paste" name="btn" type="button" value="Paste Content" onclick="pasteContent()" /></p>
    <hr />
    <div id="displaycontent" title="<%= htmcontent %>">
        <%=htmcontent%>
    </div>
    <textarea title="Enter Content here" id="content" style="display: none; width: 100%;
        height: 300px;" runat="server"></textarea>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="librarylist" CodeFile="librarylist.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link rel='stylesheet' type='text/css' href='Tree/css/com.ektron.ui.contextmenu.css' />
    <link rel='stylesheet' type='text/css' href='Tree/css/com.ektron.ui.tree.css' />
    <!-- TODO: Ross - Remove this when it is registered by the base class -->
    <script type="text/javascript" src="/workarea/java/stylehelper.js"></script>
    <asp:Literal ID="ltrSytleScript" runat="server"></asp:Literal>
    <link rel="stylesheet" type="text/css" href="csslib/ektron.workarea.css" />
</head>
<body <% if (m_bAjaxTree) { %> onclick="ContextMenuUtil.hide()" onload="Main.start();displayTree()"
    <% } %>>
    <% if (m_bAjaxTree)
       { %>

    <script type="text/javascript" src="Tree/js/com.ektron.explorer.init.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.explorer.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.explorer.config.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.explorer.windows.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.cms.types.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.cms.parser.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.cms.toolkit.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.cms.api.js">
    </script>

    <script type="text/javascript" src='Tree/js/com.ektron.ui.contextmenu.js'>
    </script>

    <script type="text/javascript" src='Tree/js/com.ektron.ui.iconlist.js'>
    </script>

    <script type="text/javascript" src='Tree/js/com.ektron.ui.explore.js'>
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.ui.tree.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.net.http.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.lang.exception.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.utils.form.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.utils.log.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.utils.dom.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.utils.debug.js">
    </script>

    <script type="text/javascript" src="Tree/js/com.ektron.utils.string.js">
    </script>
    <script type="text/javascript">
        <!--        //--><![CDATA[//><!--

        //Debug.LEVEL = LogUtil.ALL;
        //LogUtil.logType = LogUtil.LOG_CONSOLE;

        var ContentUrl = 'libraryinsert.aspx?action=ViewLibraryByCategory&scope=<%=Request.QueryString["scope"]%><%=QueryType %><%=RetField%><%=disableLinkManage %><%=enableQDOparam%>&id='
        var FrameName = "Library";
        var rootFolderName = '<%=MsgHelper.GetMessage("generic Library title") %>';
        //--><!]]>
    </script>

    <%}%>
    <% if (m_bAjaxTree)
       { %>
    <div id="TreeOutput" style="width: 100%; height: 100%; overflow: auto;">
    </div>
    <input type="hidden" id="folderName" name="folderName" />
    <input type="hidden" id="selected_folder_id" name="selected_folder_id" value="0" />

    <script language="javascript" src="java/ektron.workareatrees.js" type="text/javascript">
    </script>

    <script type="text/javascript">
		    <!--        //--><![CDATA[//><!--
        // This function overrides the one of the same name in ektron.workareatrees.js
        function loadRightFrame(id, treeViewId, openMainPage) {
            top["libraryinsert"].location.href = ContentUrl + id;
        }
        //--><!]]>
    </script>

    <% }
       else
       { %>

    <script type="text/javascript" src="java/ekfoldercontrol.js">
    </script>

    <script type="text/javascript">
	<!--//--><![CDATA[//><!--
	function ClearFolderInfo() {

		<% if (actionType == "library") { %>
			top.libraryinsert.ClearFolderInfo();
		<% } %>
    }

	ekFolderCreateTextLinks = 1;
	ekFolderFontSize = 2;
	ekFolderMaxDescriptionLength=0;
	ekFolderImagePath = "images/application/folders/";

	<%  
	    SiteObj = AppUI.EkSiteRef;
        cPerms = SiteObj.GetPermissions(0, 0, "folder");
        if (cPerms.ContainsKey("ReadOnlyLib"))
        {
            if ((scope == "all"))
            {
                Response.Write(("var urlInfoArray = new Array(\"frame\", \"javascript:ClearFolderInfo();\", \"medialist\", \"frame\", \"library" +
                    "insert.aspx?action=ViewLibraryByCategory&id=" + (0 + ("&scope="
                                + (scope
                                + (RetField + "\", \"libraryinsert\");"))))));
            }
            else
            {
                Response.Write(("var urlInfoArray = new Array(\"frame\", \"javascript:ClearFolderInfo();\", \"medialist\", \"frame\", \"library" +
                    "insert.aspx?action=ViewLibraryByCategory&id=" + (0 + ("&scope="
                                + (scope
                                + (RetField + "&type=images\", \"libraryinsert\");"))))));
            }
            Response.Write(("TopTreeLevel = CreateFolderInstance(\""
                            + (MsgHelper.GetMessage("generic Library title") + "\", urlInfoArray);")));
        }
        else
        {
            Response.Write(("TopTreeLevel = CreateFolderInstance(\""
                            + (MsgHelper.GetMessage("generic Library title") + "\", \"\");")));
        }
        cDbObj = AppUI.EkContentRef;
        cAllFolders = cDbObj.GetFolderTreeForUserID(0);
        OutputLibraryFolders(0, 0); 
     %>
	//--><!]]>
    </script>

    <script type="text/javascript">
	    <!--        //--><![CDATA[//><!--
        InitializeFolderControl();
        //--><!]]>
    </script>
     <% } %>
</body>
</html>

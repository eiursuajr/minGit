<%@ Page Language="C#" AutoEventWireup="true" Inherits="workareanavigationtrees" CodeFile="workareanavigationtrees.aspx.cs" %>
<%@ Register TagPrefix="NavigationTree" TagName="GoogleSubtree" Src="controls/NavigationTrees/GoogleAnalyticsReportSubtree.ascx" %>
<%@ Register TagPrefix="NavigationTree" TagName="WebTrendsSubtree" Src="controls/NavigationTrees/WebTrendsReportSubtree.ascx" %>
<%@ Register TagPrefix="NavigationTree" TagName="SiteCatalystSubtree" Src="controls/NavigationTrees/SiteCatalystReportSubtree.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>workareanavigationtrees</title>
        <meta http-equiv="Pragma" content="no-cache"/>
        <script type="text/javascript">
	    <!--
		    var bIsValid = false;
		    var bIsValidTested = false;

		    function IsValid() {
			    if (bIsValidTested)
				    return bIsValid;
				    bIsValid = ((typeof(top) == "object")
				    && (typeof(top.StartShrink) == "function")
				    && (typeof(top.StartEnlarge) == "function"))

			    bIsValidTested = true;
			    return bIsValid;
		    }

		    function StartShrinkNavArea(e) {
			    if (IsValid())
				    top.StartShrink(e);
		    }

		    function StartEnlargeNavArea(e) {
			    if (IsValid())
				    top.StartEnlarge(e);
		    }
		//-->
        </script>

<%--
Not used. Initially implemented client-side during development, but later moved to code-behind. doug.domeny 2009-08-04
		<script type="text/javascript">
        <!--
        function insertTree(elem, parent)
        {
			switch (elem.tagName)
			{
				case "UL":
					$ektron(elem).children("li").each(function()
					{
						insertTree(this, parent);
					});
					break;
				case "LI":
					var caption = elem.childNodes[0].nodeValue;
					var li = $ektron(elem);
					var subtree = li.children("ul");
					if (subtree.length > 0)
					{
						parent = InsertFolder(parent, CreateFolderInstance(caption, []));
						subtree.each(function()
						{
							insertTree(this, parent);
						});
					}
					else
					{
						InsertFile(parent, CreateLink(caption, ["frame", "TBDURL", "ek_main"]));
					}
					break;
				default:
					break;
			}
        }
        // -->
        </script>
--%>

        <script type="text/javascript">
        <!--
			    ekFolderCreateTextLinks = 1;
			    ekFolderFontSize = 2;
			    ekFolderMaxDescriptionLength=0;
			    ekFolderImagePath = "images/ui/icons/tree/";

			    // wrap the output into a DIV we can style a bit
			    Ektron.ready(function()
		        {
		            $ektron("body").wrapInner("<div class='ektronTreeContainer ektronTreeComposite' style='padding: 0;'><ul></ul></div>");
		        });
		// -->
        </script>

 		<asp:Literal id="treeJsOutput" runat="server" />

	</head>
    <body onmouseover="StartEnlargeNavArea(event);" onmouseout="StartShrinkNavArea(event);" style="padding-left: 10px;">
    <NavigationTree:GoogleSubtree id="GoogleAnalyticsContainer" runat="server" />
    <NavigationTree:SiteCatalystSubtree id="SiteCatalystContainer" runat="server" />
    <NavigationTree:WebTrendsSubtree id="WebTrendsContainer" runat="server" />
    </body>
</html>


<%@ Page Language="C#" AutoEventWireup="true" Inherits="xml_config" CodeFile="xml_config.aspx.cs" %>
<%@ Reference Control="controls/xmlconfig/viewxml_config.ascx" %>
<%@ Reference Control="controls/xmlconfig/editxml_config.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>meta_data</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <asp:Literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript">
    <!--/*--><![CDATA[/*><!--*/
    function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
		var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
		var popupwin = window.open(url, hWind, cToolBar);
		return popupwin;
	}
	function ShowTransString(Text) 
	{
	    var ObjId = "WorkareaTitlebar";
	    var ObjShow = document.getElementById('_' + ObjId);
	    var ObjHide = document.getElementById(ObjId);
	    if ((typeof ObjShow != "undefined") && (ObjShow != null)) 
	    {
		    ObjShow.innerHTML = Text;
		    ObjShow.style.display = "inline";
		    if ((typeof ObjHide != "undefined") && (ObjHide != null)) 
		    {
			    ObjHide.style.display = "none";
		    }
	    }
    }
    
	Ektron.ready(function()
	{
	    var headerStrongTags = $ektron("strong.headerRow");
	    headerStrongTags.each(function(i)
	    {
	        $ektron(this)
	            .parent("td").addClass("ui-widget-header ui-state-active")
	    });
	});
    /*]]>*/-->
    </script>

    <style type="text/css">
        strong.headerRow
        {
            color: #fff;
            text-align: left;
            display: block;
        }
        fieldset {margin-top: .5em;}
    </style>
</head>
<body>
    <form id="xmlconfiguration" method="post" runat="server">
        <asp:PlaceHolder ID="DataHolder" runat="server" />
    </form>
</body>
</html>

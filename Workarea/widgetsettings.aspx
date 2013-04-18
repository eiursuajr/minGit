<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="widgetsettings.aspx.cs" Inherits="Workarea_widgetsettings" %>

<%@ Register Src="controls/widgetSettings/WidgetSpace.ascx" TagName="WidgetSpace" TagPrefix="uc1" %>
<%@ Register Src="controls/widgetSettings/WidgetSync.ascx" TagName="WidgetSync" TagPrefix="uc2" %>
<%@ Register Src="controls/widgetSettings/WidgetEdit.ascx" TagName="WidgetEdit" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Widget Settings</title>
    <asp:Literal id="m_strStyleSheetJS" runat="server" />
        <script type="text/javascript" src="java/jfunct.js"></script>
    <script type="text/javascript" src="java/toolbar_roll.js"></script>
    <script type="text/javascript">
        $ektron(document).ready(function(){
            if($ektron(".ektronTitlebar #WorkareaTitlebar").text()=="Edit Widget"){
                //$ektron(".ektronTitlebar #WorkareaTitlebar").parent().addClass("hide-title");
               // $ektron(".ektronPageHeader").addClass("no-top-padding");
            }

        });
    </script>
    <style type="text/css">
        body { font-size: 12px; }
        td.main { padding:10px 10px 10px 10px; }
        table.dataTable { width: 100%; text-align:left; border: solid 1px black; }
        table.dataTable th { padding-right: 10px; border-bottom:solid thin black; }
        table.dataTable td { padding-right: 10px; }
        img { border: 0px; }
        ul.widgetlist { list-style: none; }
        ul.widgetlist li { padding: 3px; }
        ul.widgetlist span { vertical-align: top; }
        ul.widgetlist img { border: 0px; }
        table.widgetSettings { width:95%; }
        span.displayParsedDate { display:block; width: 180px; }	
        span.error { color: red; }
        a.widgetedit {display: inline-block; width: 16px; margin: 0 .25em 0 0; padding: 0; text-indent: -10000px; background-repeat: no-repeat; background-position: center center; background-image: url("images/UI/Icons/contentEdit.png"); overflow: hidden;}
        .padLeft { margin-left: .25em; display:inline-block; }
    </style>
    <!--[if IE]>
    <style type="text/css">
        /* without the following line, the contents of the TDs are not visible in IE */
        a.widgetedit {float: left;}
    </style>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <asp:PlaceHolder runat="server" ID="placeHolder" />
    </form>
</body>
</html>

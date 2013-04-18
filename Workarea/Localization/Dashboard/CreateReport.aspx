<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateReport.aspx.cs" Inherits="Localization_CreateReport" %>
<%@ Register Src="Widgets/CreateReport.ascx" TagName="KPI" TagPrefix="Ektron" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title id="titleTag" runat="server"></title>
        
        
        <link href="css/NewWidgets.css" rel="stylesheet" type="text/css" />
        <script type="text/javascript" language="javascript">
            Ektron.ready(function() {
                $ektron("div.EktronPersonalization div.widget > div.content").show();
            });
        </script>
        <style type="text/css" >
            div.EktronPersonalization {min-width:880px;}
        </style>
        <asp:literal id="StyleSheetJS" runat="server" />
    </head>
    <body>
        <form id="form1" runat="server">
 
       	<div class="ektronPageContainer ektronPageInfo">

            <div class="EktronPersonalization">
                <div class="widget">
                    <div class="header">
                    </div>
                    <div class="content" >
                        <Ektron:KPI ID="kpi1" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        </form>
    </body>
</html>

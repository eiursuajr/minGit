<%@ Page Language="C#" AutoEventWireup="true" CodeFile="selectTaxonomy.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.selectTaxonomy" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Catalog Entry</title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style type="text/css">
            body {background-color:white;}
            table#Table6 {display:none;}
        </style>
    </head>
    <body class="UiMain">
        <form id="frmMain" runat="server">
            <div class="taxonomySelect">
                <asp:Literal runat="server" ID="EditTaxonomyHtml"></asp:Literal>
                <div id="TreeOutput"></div>
                <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
                <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
                <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
                <input type="hidden" id="hdnData" name="data" class="data" value="" runat="server" />
                <input type="hidden" id="hdnUniqueId" name="uniqueId" class="uniqueId" value="" runat="server" />
            </div>
        </form>
    </body>
</html>
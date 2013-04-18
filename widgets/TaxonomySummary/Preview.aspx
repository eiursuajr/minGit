<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Preview.aspx.cs" Inherits="widgets_TaxonomySummary_Preview" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Theme Preview Page</title>
</head>
<body style="margin-top:0; margin-bottom:0; padding-top:0; padding-bottom:0; background-color:#fff; ">
    <form id="form1" runat="server">
        <div class="theme<%=SelectedThemes%>">
            <h3 class="taxlistheader">Header text: IT News</h3>
            <ul class="itemlist">
                <li><a class="title" href="javascript:void(0)" title="Microsoft ups the ante against pirates!">
                    Title: Microsoft ups the ante against...</a><p>
                        Content: With its operating system Windows 7 and other popular software...</p>
                </li>
                <li><a class="title" href="javascript:void(0)" title="Microsoft ups the ante against pirates!">
                    Title: Microsoft ups the ante against...</a><p>
                        Content: With its operating system Windows 7 and other popular software...</p>
                </li>
                <li><a class="title" href="javascript:void(0)" title="Microsoft ups the ante against pirates!">
                    Title: Microsoft ups the ante against...</a><p>
                        Content: With its operating system Windows 7 and other popular software...</p>
                </li>
            </ul>
        </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wizardTest.aspx.cs" Inherits="Workarea_PageBuilder_wizardTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        body
        {
            font-family: Verdana;
            font-size: 75%;
        }
        li
        {
            font-size: 1;
            margin: .25em 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ul>
            <li><a title="Add Page (add)" href="#AddPage" onclick="return Ektron.PageBuilder.Wizards.showAddPage({mode: 'add', folderId: 14, language: 1033})">
                Add New Page (mode: add)</a></li>
            <li><a title="Add Page (save as)" href="#AddPage" onclick="return Ektron.PageBuilder.Wizards.showAddPage({mode: 'saveAs', folderId: 14, language: 1033})">
                Add New Page (mode: saveAs)</a></li>
        </ul>
    </div>
    </form>
</body>
</html>

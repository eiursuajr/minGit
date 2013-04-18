<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyPhotoGallery.aspx.cs" Inherits="MyWorkspace_MyPhotoGallery" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>My Photo Gallery</title>
    <asp:Literal id="StyleSheet" runat="server"/>
</head>
<body>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>

        <cms:PhotoGallery runat="server" 
            TaxonomyCols="1"
            TaxonomyItemCols="3" 
            MaxResults="12"
            ID="MyPhotoGallery" />
    </form>
</body>
</html>


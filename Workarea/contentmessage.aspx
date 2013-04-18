<%@ Page Language="C#" AutoEventWireup="true" CodeFile="contentmessage.aspx.cs" Inherits="Workarea_controls_content_contentmessage" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <asp:literal runat="server" id="ltStyle" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronPageHeader">
            <div title="View Messages for the Content" class="ektronTitlebar" id="divTitleBar"
                runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <CMS:MessageBoard ID="MessageBoard1" runat="server" EnablePaging="True" MarkupLanguage="community/messageboardworkarea.ekml"
                ObjectParameter="id" />
        </div>
    </form>
</body>
</html>

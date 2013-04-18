<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyMessageBoard.aspx.cs" Inherits="MyWorkspace_MyMessageBoard" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>My Message Board</title>
</head>
<body>
    <form id="form1" runat="server">
        <cms:MessageBoard ID="mb1" runat="Server" ObjectType="User" MarkupLanguage="../community/messageboardworkarea.ekml"  EnablePaging="true"/>
    </form>
</body>
</html>


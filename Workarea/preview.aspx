<%@ Page Language="C#" AutoEventWireup="true" Inherits="preview" CodeFile="preview.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>preview</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
    <!--//--><![CDATA[//><!--
    function ReLoad(){
        try{top.opener.location.href = (top.opener.location.href).replace(top.opener.location.hash,"");}catch(e){}
        //top.opener.location.reload();
        self.close();
    }
    //--><!]]>
    </script>

</head>
<body onload="javascript:ReLoad();">
</body>
</html>


<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs"  Inherits="Workarea_PrivateData_Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript" language="javascript" src="js/Ektron.Crypto.js"></script>
    <script type="text/javascript" language="javascript" src="js/Ektron.Cache.js"></script>
    <script type="text/javascript" language="javascript" src="js/Ektron.PrivateData.aspx"></script>
    
    <script type="text/javascript" language="javascript">
    <!--
        function GetLoginInfo()
        {
            var inputs = $ektron(".login-div input");
            
            var username = inputs.get(0).value;
            var password = inputs.get(1).value;
            
            return [username, password];
        }
        
        function DoLogin()
        {
            var loginInfo = GetLoginInfo();
            Ektron.PrivateData.SetLoginInfo(loginInfo[0], loginInfo[1]);
        }
    -->
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="login-div">
        <asp:Login ID="login" runat="server" DisplayRememberMe="False" OnAuthenticate="login_Authenticate">
        </asp:Login>
    </div>
    </form>
</body>
</html>

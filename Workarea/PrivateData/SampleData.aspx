<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SampleData.aspx.cs" Inherits="Security_SampleData"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Sample Data Page</title>
    
    <script type="text/javascript" language="javascript" src="js/Ektron.PrivateData.aspx"></script>
    <script type="text/javascript" language="javascript" src="js/Ektron.Crypto.js"></script>
    <script type="text/javascript" language="javascript" src="js/Ektron.Cache.js"></script>
    
    <script type="text/javascript">
        var encData = "";
        
        Ektron.ready(function(){
            Ektron.PrivateData.Debug = false;
            //Ektron.PrivateData.SetLoginInfo("builtin", "builtin");
            //alert(Ektron.PrivateData.SecurityKey);  
            Ektron.PrivateData.GetSecurityKey();
            
            encData = Ektron.PrivateData.Encrypt("This is a test of the encryption1234567890-");
            $ektron("#callbackdiv").html(encData)
			Ektron.PrivateData.Log(encData);
//            Ektron.PrivateData.Set("TestData", encData, function(data){
//                $ektron("#callbackdiv").html(data);
//            });
//            Ektron.PrivateData.Get("TestData", function(data){
//                Ektron.PrivateData.Log(Ektron.Crypto.Base64.decode(data));
//            });

            Ektron.PrivateData.LoadEncryptedData();
        });
        
        function callback(data){
            $ektron("#callbackdiv").html(data);
        }
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Vestibulum sit amet magna. Nulla mi. Praesent eget urna. Pellentesque varius tristique velit. Morbi venenatis placerat massa. Morbi sed felis in justo facilisis ornare. Maecenas lacus dui, consequat eu, malesuada sit amet, pellentesque a, elit. Quisque vitae turpis. In sed justo pellentesque velit volutpat molestie. Sed luctus mi scelerisque nisl. Duis tempus. Morbi a nisl. Sed vestibulum feugiat turpis. Duis pede. Integer risus libero, aliquet a, condimentum et, volutpat tempus, diam. </div>
    
        <div id="enc1" class="EktronEncryptedData">key:TestData</div>

        <div id="EncEmpty" class="EktronEncryptedData">key:badKey</div>
        
        <div id="callbackdiv"></div>
    </div>
    </form>
</body>
</html>

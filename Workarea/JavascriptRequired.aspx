<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JavascriptRequired.aspx.cs" Inherits="java_JavascriptRequired" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Jacascript Required</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <style type="text/css">
        <!--/*--><![CDATA[/*><!--*/
        div {margin: 3.2em; text-align: center;}
        h1{margin-bottom: 3.2em;}
        /*]]>*/-->
    </style>
    <script type="text/javascript" >
        <!--//--><![CDATA[//><!--
        function PageLoad(){
            //debugger;
            var elm = document.getElementById("infoContainer");
            if (elm){
                elm.innerText = "Reffered by: " + document.referrer;
            }
        }
        //--><!]]>
    </script>
</head>
<body onload="PageLoad()">
    <form id="form1" runat="server">
    <div>
        <h1 title="Javascript Required">Javascript Required</h1>
        <p title="Javascript is required for the page the you selected">
            Javascript is required for the page the you selected.
        </p>
        <p title="Please enable Javascript in your browser, or use a browser that supports it">
            Please enable Javascript in your browser, or use a browser that supports it.
        </p>
        <p id="infoContainer">...</p>
    </div>
    </form>
</body>
</html>

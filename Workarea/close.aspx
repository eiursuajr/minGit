<%@ Page Language="C#" AutoEventWireup="true" CodeFile="close.aspx.cs" Inherits="Workarea_close" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script type="text/javascript">
            <!--//--><![CDATA[//><!--
            function IsBrowserIE() 
            {
                // document.all is an IE only property
                return (document.all ? true : false);
            }
            function WaitOnLoadAction() {
	            setTimeout('loadPage();', 100);
            }
            function loadPage(){
	            if(window.top.opener)
	            {
		            var buffer = '';
            		
		            try {
		                buffer = new String( top.opener.location );
		            }
		            catch( ex ) {
            		
		            }
		            if (buffer.indexOf("#") != -1)
		            {
			            var sUrl = top.opener.location.pathname;
                        
			            sUrl = sUrl + '<%= (Request.QueryString["__taxonomyid"] != null ? "__taxonomyid=" + Request.QueryString["__taxonomyid"] : "") %>';
            		
		    	            //alert(window.location.pathname);
            		            top.opener.location.href = sUrl;
		            }
	                else if(buffer.indexOf("/main.html") < 1)
                    {  
                        try
                        {
			                <% if ((!string.IsNullOrEmpty(Request.QueryString["reload"]))) { %>
                                top.opener.location.reload(true);
                            <% } else if (!string.IsNullOrEmpty(Request.QueryString["toggle"])) { %>
                                window.top.opener.location.href = (window.top.opener.location.href);
			                <% } else { %>
                                top.opener.location(top.opener.location.href);
			                <% } %>
                        }
                        catch(e)
                        {
                        }
                    }
		            top.close();
	            }
	            else
	            { 
		            if (("object"==typeof top.opener) && top.opener!=null)
		            {	
			            <% if ((string.IsNullOrEmpty(Request.QueryString["logout"]))) { %>
				            var buffer = new String( top.opener.location );
				            if(1 > buffer.indexOf("main.html"))
					            top.opener.location.reload(true);
			            <% } %>
		            }
		            top.close();
	            }
            }
            //--><!]]>
        </script>
</head>
<body onload="loadPage();">
</body>
</html>

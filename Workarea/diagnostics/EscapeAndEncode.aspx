<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EscapeAndEncode.aspx.cs" Inherits="EscapeAndEncode" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Escape and Encode</title>
    <style type="text/css">
    thead th, thead td
    {
		font-weight: bold;
    }
    th
    {
		font-weight: normal;
		text-align: left;
		font-family: arial;
		font-size: small;
    }
    td
    {
		font-family: Courier New;
		font-size: small;
    }
    tr.js
    {
		color: #cc0000;
    }
    tr.net
    {
		background-color: #ccffcc;
    }
    tr.AntiXss
    {
		background-color: #ccffce;
    }
    </style>
    <script runat="server">
		string SampleText = "T: <+%>/\\?=&#\"'ñό世«»";
    </script>
    <asp:Placeholder runat="server">
    <script type="text/javascript">
    <!--
    var sSampleText = "<%= Ektron.Cms.API.JS.Escape(SampleText) %>";
    
    var sEkUrlEncode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Ektron.Cms.Common.EkFunctions.UrlEncode,SampleText))%>";
    var sUrlEncode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Server.UrlEncode,SampleText))%>";
    var sUrlEncodeUnicode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(System.Web.HttpUtility.UrlEncodeUnicode,SampleText))%>";
    var sUrlPathEncode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Server.UrlPathEncode,SampleText))%>";
    var sHtmlAttributeEncode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(System.Web.HttpUtility.HtmlAttributeEncode,SampleText))%>";
    var sEkHtmlEncode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Ektron.Cms.Common.EkFunctions.HtmlEncode,SampleText))%>";
    var sHtmlEncode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Server.HtmlEncode,SampleText))%>";
    var sJSEscapeAndEncode = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Ektron.Cms.API.JS.EscapeAndEncode,SampleText))%>";
    var sJSEscape = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Ektron.Cms.API.JS.Escape,SampleText))%>";
    var sEscapeRegExp = "<%=Ektron.Cms.API.JS.Escape(applyEscape(Ektron.Cms.API.JS.EscapeRegExp,SampleText))%>";
    var sRegexEscape = "<%=Ektron.Cms.API.JS.Escape(applyEscape(System.Text.RegularExpressions.Regex.Escape,SampleText))%>";
    
    var sAntiXssUrlEncode ="<%=Ektron.Cms.API.JS.Escape(applyEscape(Microsoft.Security.Application.AntiXss.UrlEncode,SampleText))%>";
    var sAntiXssHtmlEncode ="<%=Ektron.Cms.API.JS.Escape(applyEscape(Microsoft.Security.Application.AntiXss.HtmlEncode,SampleText))%>";
    var sAntiXssHtmlAttributeEncode ="<%=Ektron.Cms.API.JS.Escape(applyEscape(Microsoft.Security.Application.AntiXss.HtmlAttributeEncode,SampleText))%>";
    var sAntiXssXmlEncode ="<%=Ektron.Cms.API.JS.Escape(applyEscape(Microsoft.Security.Application.AntiXss.XmlEncode,SampleText))%>";
    var sAntiXssXmlAttributeEncode ="<%=Ektron.Cms.API.JS.Escape(applyEscape(Microsoft.Security.Application.AntiXss.XmlAttributeEncode,SampleText))%>";
    var sAntiXssJavaScriptEncode ="<%=Ektron.Cms.API.JS.Escape(applyEscape(Microsoft.Security.Application.AntiXss.JavaScriptEncode,SampleText))%>";


    Ektron.ready(function()
    {
		displayText("SampleText1", applyEscape(function(s){return s}, sSampleText));
		displayText("SampleText2", applyEscape(function(s){return s}, sSampleText));
		displayText("SampleText3", applyEscape(function(s){return s}, sSampleText));
		displayText("SampleText4", applyEscape(function(s){return s}, sSampleText));
		
		displayText("encodeURIComponent", applyEscape(encodeURIComponent, sSampleText));
		displayText("encodeURI", applyEscape(encodeURI, sSampleText));
		displayText("escape", applyEscape(escape, sSampleText));
		
		displayText("EkUrlEncode", sEkUrlEncode);
		displayText("UrlEncode", sUrlEncode);
		displayText("UrlEncodeUnicode", sUrlEncodeUnicode);
		displayText("UrlPathEncode", sUrlPathEncode);
		
		displayText("htmlEncode", applyEscape($ektron.htmlEncode, sSampleText));
		displayText("htmlEncodeText", applyEscape($ektron.htmlEncodeText, sSampleText));

		displayText("HtmlAttributeEncode", sHtmlAttributeEncode);
		displayText("EkHtmlEncode", sEkHtmlEncode);
		displayText("HtmlEncode", sHtmlEncode);
		
		displayText("StringEscape", applyEscape(Ektron.String.escape, sSampleText));
		displayText("JSEscapeAndEncode", sJSEscapeAndEncode);
		displayText("JSEscape", sJSEscape);
		
		displayText("RegExpEscape", applyEscape(Ektron.RegExp.escape, sSampleText));
		
		displayText("EscapeRegExp", sEscapeRegExp);
		displayText("RegexEscape", sRegexEscape);
		
		displayText("AXssUrlEncode",sAntiXssUrlEncode);
		displayText("AXssHtmlEncode",sAntiXssHtmlEncode);
		displayText("AXssHtmlAttributeEncode",sAntiXssHtmlAttributeEncode);
		displayText("AXssXmlEncode",sAntiXssXmlEncode);
		displayText("AXssXmlAttributeEncode",sAntiXssXmlAttributeEncode);
		displayText("AXssJavaScriptEncode",sAntiXssJavaScriptEncode);
    });
    
    function applyEscape(f, s)
    {
		var r = new Ektron.String();
		for (var i = 0; i < s.length; i++)
		{
			var c = s.charAt(i);
			var t = f(c);
			var S = new Ektron.String(t);
			r.append(S.padLeft(c.charCodeAt(0) > 0x7f ? 10 : 7, "\xa0"));
			//r.append(t);
		}
		return r.toString();
    }
    
    function displayText(id, t)
    {
		$ektron("#" + id).html($ektron.htmlEncodeText(t));
    }
    
    // -->
    </script>
    </asp:Placeholder>
</head>
<body>
    <form id="form1" runat="server">
    <table>
    <thead>
    <tr><th>&#160;</th><th>Sample text</th><td id="SampleText1">&#160;</td></tr>
    </thead>
    <tbody>
    <tr class="js"><th>JS</th><th>encodeURIComponent</th><td id="encodeURIComponent">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>EkFunctions.UrlEncode</th><td id="EkUrlEncode">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>Server.UrlEncode</th><td id="UrlEncode">&#160;</td></tr>
    <tr class="AntiXss"><th>AntiXss</th><th>AntiXss.UrlEncode</th><td id="AXssUrlEncode">&#160;</td></tr>
    <tr class="js"><th>JS</th><th>encodeURI</th><td id="encodeURI">&#160;</td></tr>
    <tr class="js"><th>JS</th><th>escape [deprecated]</th><td id="escape">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>HttpUtility.UrlEncodeUnicode</th><td id="UrlEncodeUnicode">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>Server.UrlPathEncode</th><td id="UrlPathEncode">&#160;</td></tr>
    </tbody>
    </table>
    <br />
    <table>
    <thead>
    <tr><th>&#160;</th><th>Sample text</th><td id="SampleText2">&#160;</td></tr>
    </thead>
    <tbody>
    <tr class="js"><th>JS</th><th>$ektron.htmlEncode</th><td id="htmlEncode">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>EkFunctions.HtmlEncode</th><td id="EkHtmlEncode">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>Server.HtmlEncode</th><td id="HtmlEncode">&#160;</td></tr>
    <tr class="js"><th>JS</th><th>$ektron.htmlEncodeText</th><td id="htmlEncodeText">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>HttpUtility.HtmlAttributeEncode</th><td id="HtmlAttributeEncode">&#160;</td></tr>
	<tr class="AntiXss"><th>AntiXss</th><th>AntiXss.HtmlEncode</th><td id="AXssHtmlEncode">&#160;</td></tr>
	<tr class="AntiXss"><th>AntiXss</th><th>AntiXss.XmlEncode</th><td id="AXssXmlEncode">&#160;</td></tr>
	<tr class="AntiXss"><th>AntiXss</th><th>AntiXss.HtmlAttributeEncode</th><td id="AXssHtmlAttributeEncode">&#160;</td></tr>
	<tr class="AntiXss"><th>AntiXss</th><th>AntiXss.XmlAttributeEncode</th><td id="AXssXmlAttributeEncode">&#160;</td></tr>
    </tbody>
    </table>
	<br />
    <table>
    <thead>
    <tr><th>&#160;</th><th>Sample text</th><td id="SampleText4">&#160;</td></tr>
    </thead>
    <tbody>
	<tr class="AntiXss"><th>AntiXss</th><th>AntiXss.JavaScriptEncode</th><td id="AXssJavaScriptEncode">&#160;</td></tr>
    </tbody>
    </table>
	<br />
    <table>
    <thead>
    <tr><th>&#160;</th><th>Sample text</th><td id="SampleText3">&#160;</td></tr>
    </thead>
    <tbody>
    <tr class="net"><th>.NET</th><th>Ektron...JS.EscapeAndEncode</th><td id="JSEscapeAndEncode">&#160;</td></tr>
    <tr class="js"><th>JS</th><th>Ektron.String.escape</th><td id="StringEscape">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>Ektron...JS.Escape</th><td id="JSEscape">&#160;</td></tr>
    </tbody>
    </table>
	<br />
    <table>
    <thead>
    <tr><th>&#160;</th><th>Sample text</th><td id="SampleText5">&#160;</td></tr>
    </thead>
    <tbody>
    <tr class="js"><th>JS</th><th>Ektron.RegExp.escape</th><td id="RegExpEscape">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>Ektron...JS.EscapeRegExp</th><td id="EscapeRegExp">&#160;</td></tr>
    <tr class="net"><th>.NET</th><th>RegularExpressions.Regex.Escape</th><td id="RegexEscape">&#160;</td></tr>
    </tbody>
    </table>

    </form>
</body>
</html>

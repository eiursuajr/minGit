<%@ Page Language="C#" AutoEventWireup="true"
 Inherits="compares" CodeFile="compare.aspx.cs" AspCompat="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>CMS Compare</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <script language="javascript" src="ewebdiff/ewebdiff.js">
</script>
	<script language="VBScript" src="ewebdiff/ewebdiff.vbs">
</script>
    <link rel="STYLESHEET" href="csslib/ektron.workarea.css" type="text/css">
	<link href="csslib/tabui.css" type="text/css" rel="STYLESHEET"/>
	<link href="xslt/xmlverbatim.css" type="text/css" rel="STYLESHEET"/>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
        <asp:HiddenField ID="csnewcontent" RunAt="server" />
        <asp:HiddenField ID="csbasecontent" RunAt="server" />
		<asp:HiddenField ID="isxml" RunAt="server" Value="0" />
		<asp:Panel ID="ShowDiffPanel" Runat="server" Visible=False>
			<script type="text/javascript">
			    <!--//--><![CDATA[//><!--
				var sHtml = "";
				sHtml = Ekt_CreateEWebDiff();
				if(sHtml.length != 0){
					document.write('<div id="eWebDiffLoadMsg" name="eWebDiffLoadMsg" style="color: blue; text-align:center;">Loading Please wait...</div>');
					this.document.write('<div align="center" id="eWebDiffHolder" name="eWebDiffHolder">');
					document.write(sHtml);//Create eWebDiff Control
					this.document.write('</div>');
					if(this.document.eWebDiffCtl != "undefined") {		
						if (ShowDiff) {	
								if (typeof this.document.eWebDiffCtl != "undefined"){
								this.document.eWebDiffCtl.Licensekey = '<asp:literal id="LicKey" runat="Server"/>';
								if (document.forms[0].isxml.value == "1") {
								    //for some reason, XML comparison mode doesn't work w/ cshtmldiff.ocx, so we do a pseudo comparison using HTML mode
								    this.document.eWebDiffCtl.AnalysisMode = 1;
								    this.document.eWebDiffCtl.docompare(unescape("<html><body>" + document.forms[0].csbasecontent.value + "</body></html>"), 
								                                    unescape("<html><body>" + document.forms[0].csnewcontent.value + "</body></html>"));
								} else {
								    this.document.eWebDiffCtl.AnalysisMode = 2;
								    this.document.eWebDiffCtl.docompare(document.forms[0].csbasecontent.value, 
								                                    document.forms[0].csnewcontent.value);
								}
								eWebDiffLoadMsg.style.cssText = "display: none;";
								eWebDiffHolder.style.cssText = "";				
							}
						}
					}
					else{
						eWebDiffLoadMsg.innerHtml = "Ektron eWebDiff is not installed.";							
					}		
				}
				else{
					this.document.write("<div style='border-style:solid; border:1; border-color:red; color:red; text-align:center'>Ektron eWebDiffF is only supported in IE 5.5 or above.</div>");
				}
				//--><!]]>
			</script>			
		</asp:Panel>
		<asp:Panel ID="HideDiffPanel" Runat="server" Visible=False>
			<P>
				<br><br>
				<h1 align="center" title="Content is identical">Content is identical</h1>
			</P>
			<p align="center">
				<input title="Close" type="button" value="Close" onClick="javascript:window.close();" ID="buttonClose" NAME="buttonClose">
			</p>
		</asp:Panel>
		<asp:Panel ID="ShowSvrDiffPanel" runat="server" Visible="false">
		    <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
		        function setBGColor(id, color) {
		            var el = document.getElementById(id); 
                    if(el.currentStyle) { 
                     // for msie 
                        el.style.backgroundColor = color; 
                    } else { 
                        el.style.setProperty("background-color", color, null); 
                    } 
		        }
		        function showSpan(id, flag) {
		            var el = document.getElementById(id); 
		            if (flag) {
		                el.style.display = 'block';
		            } else {
		                el.style.display = 'none';
		            }
		        }
		        function switchTab(tabname) {
		            setBGColor('lnkTabNew', "#B0C4DE");
		            setBGColor('lnkTabOrig', "#B0C4DE");
		            setBGColor('lnkTabDiff', "#B0C4DE");
		            setBGColor('lnkTab' + tabname, "white");
		            showSpan('contentTabOrig', false);
		            showSpan('contentTabNew', false);
		            showSpan('contentTabDiff', false);
		            showSpan('contentTab' + tabname, true);
		        }
		        //--><!]]>
		    </script>
		
            <ul id="menu">
              <li id="nav-1"><a title="Difference" ID="lnkTabDiff"
                href="javascript:switchTab('Diff');" style="background:white;font-family:Verdana;font-weight:bold;text-decoration:none;">
                <asp:Literal ID="lblTabDiff" runat="server" Text="Difference" /></a></li>
              <li id="nav-2"><a title="Origional" ID="lnkTabOrig"
                href="javascript:switchTab('Orig');" style="font-family:Verdana;font-weight:bold;text-decoration:none;">
                <asp:Literal ID="lblTabOrig" runat="server" Text="Original" /></a></li>
              <li id="nav-3"><a title="New" ID="lnkTabNew"
                href="javascript:switchTab('New');" style="font-family:Verdana;font-weight:bold;text-decoration:none;">
                <asp:Literal ID="lblTabNew" runat="server" Text="New" /></a></li>
            </ul>
            <div id="contents">
            <span id="contentTabOrig" style="display:none">
                <asp:Literal ID="DiffOrigContent" runat="server"/>
            </span>
            <span id="contentTabNew" style="display:none">
                <asp:Literal ID="DiffNewContent" runat="server"/>
            </span>
                        
            <span id="contentTabDiff">
                <asp:Literal ID="DiffResults" runat="server"/>
	            <br />
	            <br />
	            <font style="font-family:Verdana">
	            <font style="font-weight:bold;color:Green"><asp:Literal ID="lblLegend" runat="Server" Text="Legend:"/></font>
	            <br />
	            - <span class="HDAdded"><asp:Literal ID="lblLegendAdded" runat="Server" Text="Added"/></span>
	            <br />
	            - <span class="HDDeleted"><asp:Literal ID="lblLegendDeleted" runat="Server" Text="Deleted"/></span>
	            </font>
            </span>
            </div>
		</asp:Panel>
    </form>
  </body>
</html>

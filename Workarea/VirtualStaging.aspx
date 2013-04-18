<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VirtualStaging.aspx.cs" ValidateRequest="false" Inherits="Workarea_VirtualStaging" %>
<%@ Reference Control="controls/virtualstaging/viewvirtualstaging.ascx" %>
<%@ Reference Control="controls/virtualstaging/editvirtualstaging.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title><asp:Literal id="ltr_title" runat="server" /></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	<asp:literal id="StyleSheetJS" runat="server"></asp:literal>
  </head>
  <body>

    <form id="virtualstaging" name="virtualstaging" method="post" runat="server">
	    <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" title="virtual staging page title -HC" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronForm">		
			    <tr>
				    <td>
					    <asp:PlaceHolder ID="DataHolder" Runat="server"></asp:PlaceHolder>
				    </td>
			    </tr>
		    </table>
		</div>
    </form>
  </body>
  <%if (m_blnRefreshFrame == true)
    { %>
          <script type="text/javascript" language="javascript">
	        <!--
	        var frmNavBottom;
	        frmNavBottom=window.parent.frames["ek_nav_bottom"];	
	        if (("object"==typeof(frmNavBottom)) && (frmNavBottom!= null))
	        {
		        frmNavBottom.ReloadTrees('smartdesktop');
		        frmNavBottom.ReloadTrees('admin');
	        }
	        //-->
	        </script>
  <%}%>
</html>

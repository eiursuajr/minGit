<%@ Page Language="C#" AutoEventWireup="true" Inherits="configure" CodeFile="configure.aspx.cs" ValidateRequest="false" %>
<%@ Register src="controls/configuration/viewconfiguration.ascx" tagname="viewconfiguration" tagprefix="uc1" %>
<%@ Register src="controls/configuration/editconfiguration.ascx" tagname="editconfiguration" tagprefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title><asp:Literal runat="server" id="litTitle" /></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<asp:literal id="StyleSheetJS" runat="server"/>
  </head>
  <body>
    <form id="config" name="config" method="post" runat="server">
		<div id="FrameContainer" style="z-index:100; DISPLAY: none; LEFT: 55px; WIDTH: 1px; POSITION: absolute; TOP: 48px; HEIGHT: 1px">
			<iframe id="ChildPage" name="ChildPage" 
			    frameborder="yes" marginheight="0" marginwidth="0" 
			    width="100%" height="100%" scrolling="auto">
			</iframe>
		</div>
		<div id="dhtmltooltip"></div>
		<div class="ektronPageHeader">
		    <div title="Application Setup" class="ektronTitlebar" id="divTitleBar" runat="server"></div>
		    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
		</div>
		<asp:MultiView ID="ViewSet" ActiveViewIndex="0" runat="server">
		    <asp:View ID="View" runat="server">
		        <uc1:viewconfiguration ID="viewconfiguration1" runat="server" />
		    </asp:View>
		    <asp:View ID="Edit" runat="server">
		        <uc2:editconfiguration ID="editconfiguration1" runat="server" />
		    </asp:View>
		</asp:MultiView>
		<%--<asp:PlaceHolder ID="DataHolder" Runat="server" />--%>
    </form>
  </body>
  <%	if (m_blnRefreshFrame == true)
        {%>
  <script type="text/javascript">
	<!--//--><![CDATA[//><!--
	var frmNavBottom;
	frmNavBottom=window.parent.frames["ek_nav_bottom"];	
	if (("object"==typeof(frmNavBottom)) && (frmNavBottom!= null))
	{
		frmNavBottom.ReloadTrees('smartdesktop');
		frmNavBottom.ReloadTrees('admin');
	}
	//--><!]]>
	</script>
  <%}%>
</html>


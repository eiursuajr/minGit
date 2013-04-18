<%@ Page Language="C#" AutoEventWireup="true" Inherits="historyarea" CodeFile="historyarea.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>	<%=AppName + " " + m_refMsg.GetMessage("history page html title")%></title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<meta http-equiv="pragma" content="no-cache"/>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <script type="text/javascript">
     <!--//--><![CDATA[//><!-
    function HideListWindow(){
		if (!ChangeListWindow(true))
		{
			setTimeout("HideListWindow()", 10);
		}
	}
	function ShowListWindow(){
		if (!ChangeListWindow(false))
		{
			setTimeout("ShowListWindow()", 10);
		}
	}
	function ChangeListWindow(bHide)
	{
		NavFrame = document.getElementById('BottomFrameSet');
		if (NavFrame != null)
		{	
			if (bHide)
			{
				NavFrame.cols = "*";
			}
			else
			{						
				NavFrame.cols = "*";				
			}
			return true;					
		}
		return false;
	}
	//--><!]]>
	</script>
  </head>
	<frameset  rows="*" border="0" frameborder="yes" framespacing="0">
		<%--<frameset cols="0,0" framespacing="0" frameborder="no" border="0">			
			<frame name="workareatop" src="workareatop.aspx?title=workarea_history_top.gif" scrolling="no" frameborder="no" marginwidth="0" marginheight="0"/>
		</frameset>--%>
		<frameset id="BottomFrameSet" cols="*" border="5" frameborder="yes" framespacing="0" bordercolor="#74aee7">
			<%--<frame id="list_frame" name="list_frame" src="historylist.aspx" scrolling="auto" marginwidth="0" marginheight="0" style="BORDER-RIGHT: #74aee7 solid" frameborder="yes" runat="server"/>--%>
			<frame id="history_frame" name="history_frame" src="history.aspx" marginwidth="0" marginheight="0" frameborder="no" scrolling="auto" runat="server"/>
 		</frameset>
	</frameset>
	<body/>
</html>

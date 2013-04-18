<%@ Page Language="C#" AutoEventWireup="true" Inherits="mediamanager" CodeFile="mediamanager.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server"> 
		<title> 
			<asp:literal id="litTitle" runat="server" />
		</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>	
		<script type="text/javascript" language="javascript" src="ContentDesigner/RadWindow.js" >
</script>
		<script type="text/javascript">
		<!--//--><![CDATA[//><!--
		// #57894, #48600 & #45571: do not InitializeRadWindow() if mediamanager is called by ecommerce media tab or by the metadata tab
		// these mediamanager are not using the RadWindow object. they are using window.open().
		if (typeof InitializeRadWindow != "undefined" && -1 == parent.location.href.indexOf("&productmode=true") &&  -1 == parent.location.href.indexOf("&retfield=")) {	  
		    InitializeRadWindow();
		}
		var m_Stat = false;
		var m_SelectedLibraryItem = new LibraryItem();
		function LibraryItem()
		{
			this.ID = 0;
			this.FolderID = 0;
			this.Type = 0;
			this.Title = "";
			this.FileName = "";
		}
		function SetSelectedLibraryItem(id, folderID, type, title, filename)
		{
			//DEBUG:	alert(id + ", " + folderID + ", " + type + ", " + title + ", " + filename);
			var item = new LibraryItem();
			item.ID = id;
			item.FolderID = folderID;
			item.Type = type;
			item.Title = title;
			item.FileName = filename;
			
			//Now save the item
			m_SelectedLibraryItem = item;
			
		}
		function GetSelectedLibraryItem()
		{
			return m_SelectedLibraryItem
		}
		function SetLoadStatus(val)
		{
			if (val != "")
			{
				m_Stat = true;
			}
			else
			{
				m_Stat = false;
			}
		}
		function GetLoadStatus()
		{
			return m_Stat;
		}
		
		function CloseRadDlg(filename, caption, type)
		{
            args = { sFilename: filename, sCaption: caption , sType: type};
            if (typeof window.radWindow != "undefined")
            {
                CloseDlg(args);
            }
		}
		setTimeout("self.focus()",2000);
        //--><!]]>
		</script>	
	</head>
	<asp:literal id="litFrameset" runat="server" />
</html>


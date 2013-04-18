<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MetaSelectContainer.aspx.cs" Inherits="MetaSelectContainer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	<script type="text/javascript">
	<!--//--><![CDATA[//><!--
	
	function isMetaSelectContainer()
	{
		return (true);
	}
	
	function getMainWindow()
	{
		var mainWindow = null;

		if (top && top.opener && !top.opener.closed
			&& ("function" == typeof top.opener.ek_ma_CloseMetaChildPage))
		{
			// we're inside a popup window.
			mainWindow = top.opener;
		}
		else
		{
			// we're in an iframe.
			if (window.parent
				&& ("function" == typeof window.parent.ek_ma_CloseMetaChildPage))
			{
				mainWindow = window.parent;
			}
		}
		return mainWindow;
	}
	
	function getSelectorWindow()
	{
		var selectorWindow = null;
		if (window.frames && window.frames[1]
			&& ("function" == typeof window.frames[1].metaselect_addMetaSelectRow))
		{
			selectorWindow = window.frames[1];
		}
		return (selectorWindow);
	}

	// support for SelectFolder.aspx:
	function ReturnChildValue(folderid, folderpath, metadataFormTagId)
	{
		var delimiter = ";"; // default.
		var mainWindow = getMainWindow();
		if (mainWindow
			&& ("function" == typeof mainWindow.ek_ma_getDelimiter))
		{
			delimiter = mainWindow.ek_ma_getDelimiter(metadataFormTagId)
		}
		var selectorWindow = getSelectorWindow();
		if (selectorWindow)
		{
			selectorWindow.metaselect_addMetaSelectRow(folderid, folderpath, metadataFormTagId, delimiter)
		}
		else if (mainWindow)
		{
			mainWindow.ReturnChildValue(folderid, folderpath, metadataFormTagId)
		}
	}
	
	function CloseChildPage()
	{
		var selectorWindow = getSelectorWindow();
		if (selectorWindow)
		{
			return; // don't close, as window in frames[0] is no longer in control...s
		}
		
		var mainWindow = getMainWindow();
		if (mainWindow 
			&& ("function" == typeof mainWindow.ek_ma_CloseMetaChildPage))
		{
			mainWindow.ek_ma_CloseMetaChildPage()
		}
	}
	
	// support for MetaSelect.aspx"
	function addMetaSelectRow(selectedId, title, metadataFormTagId)
	{
		var delimiter = ";"; // default.
		var mainWindow = getMainWindow();
		if (mainWindow
			&& ("function" == typeof mainWindow.ek_ma_getDelimiter))
		{
			delimiter = mainWindow.ek_ma_getDelimiter(metadataFormTagId)
		}
		var selectorWindow = getSelectorWindow();
		if (selectorWindow)
		{
			selectorWindow.metaselect_addMetaSelectRow(selectedId, title, metadataFormTagId, delimiter)
		}
		else if (mainWindow && mainWindow.ek_ma_ReturnMediaUploaderValue)
		{
			mainWindow.ek_ma_ReturnMediaUploaderValue(selectedId, title, metadataFormTagId)
		}
	}
	
	//--><!]]>
	</script>

</head>
<asp:Literal ID="frameset_lit" runat="server" />
</html>

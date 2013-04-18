<%@ Page Language="C#" AutoEventWireup="true" Inherits="CommunityDragDrop" CodeFile="CommunityDragDrop.aspx.cs" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Reference Control="../controls/folder/viewfolder.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>DragDropCtl</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript">
		////////////////////// Properties //////////////////////
		// Properties are set from ViewFolder.ascx
		////////////////////////////////////////////////////////
		var folderID = 0;
		var contLang = -1;
		var publishHtml = 0;
		var contentID = 0;
		var folderPath = "content/";
		
		var mode_set = <%=mode_set.ToString().ToLower()%>;
		var mode = <%=mode%>;
		var mode_id = <%=mode_id%>;
		
		function FolderPath(path)
		{
		    if (path != null)
			{
				folderPath = path;
			}	
		}
		
		function FolderID(id)
		{
			if (id != null)
			{
				folderID = id;
			}			
		}
		
		function SetFolderPath(path)
		{
		    var divDAV = document.getElementById("divDAV_dropuploader");
		    var iframeDAV = document.getElementById("iframeDAV_dropuploader");
		    if(divDAV)
		    {
		        iframeDAV_dropuploader.location.href = "about:blank";
		        divDAV.navigateFrame("http://localhost/webdav/hi/", "iframeDAV_dropuploader");
		    }
		}
		
		function ContentID(id)
		{
		    if( id != null )
		    {
		        contentID = id;
		    }
		}
		function ContentLanguage(id)
		{
			if (id != null)
			{
				contLang = id;
			}
		}
		function PublishHtml(id)
		{
			if (id != null)
			{
				publishHtml = id;
			}
		}
		////////////////////// /Properties //////////////////////
		function Startup() {
			if (typeof FolderID != "function")
			{
				//setTimeout("Startup()", 10);
			}
		}
		
		function ConditionalSet()
		{	
			if( mode_set ) {
			    if( mode == "0" ) {
			        FolderID(mode_id);
			    }
			    else if( mode == "1" ) {
			        ContentID(mode_id);
			    }
			}
		}
		
		function SetDMSExt(ext, title) {
            if ((typeof EktAsset == 'object') && (EktAsset.instances[0].isReady()))
            {    
                var objectInstance = EktAsset.instances[0]
                if(objectInstance)
                {
                    objectInstance = objectInstance.editor;
                    if(objectInstance)
                    {
                        objectInstance.FileTypes = "'*." + ext + "'";
                        objectInstance.SetDragDropText(title);
                    }
                }      
            } 
            else {
                //setTimeout('SetDMSExt("' + ext + '", "' + title + '")',100);
            }
        }
		ConditionalSet();
		</script>
	</head>
	<body topmargin="0" rightmargin="0" leftmargin="0" bottommargin="0">
		<form id="dropupload" method="post" runat="server">
			<table width="100%">
				<tr>
					<td>
                        <cms:explorerdragdrop id="dropuploader" runat="server"
                            width="100%"></cms:explorerdragdrop></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>


<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.MediaManager" AutoEventWireUp="false" CodeBehind="MediaManager.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="MediaPreviewer" Src="../Controls/MediaPreviewer.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileBrowser" Src="../Controls/FileBrowser.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileUploader" Src="../Controls/FileUploader.ascx" %>

<asp:literal ID="messageHolder" Runat="server" />
<asp:panel id="actionControlsHolder" Runat="server">

<div id="MainContainer"
    <div class="Ektron_DialogTabstrip_Container">
	    <telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainContainer">
		    <telerik:tab text="<script>localization.showText('Tab1HeaderText');</script>" selected="True" elementid="MediaViewer"/>
		    <telerik:tab text="<script>localization.showText('Tab2HeaderText');</script>" elementid="MediaUploader" onclientclick="ConfigureUploadPanel();" enabled="false"/>
	    </telerik:tabcontrol>
    </div>

    <div class="Ektron_Dialog_Tabs_BodyContainer">
	    <div class="ErrorMessage" id="divErrorMessage" runat="server" visible="false"></div>
	    <div id="MediaViewer">
	        <div class="Ektron_TopLabel"><script>localization.showText('Directory');</script></div>
            <input type="text" class="Ektron_WideTextBox" id="FolderPathBox">	    
		    <table>
			    <tr>
				    <td valign="top">
					    <telerik:filebrowser id="fileBrowser" runat="server" />
				    </td>
				    <td><span class="Ektron_LeftSpaceSmall"></span></td>
				    <td valign="top">
					    <telerik:mediapreviewer	id="previewer" runat="server" />
				    </td>
			    </tr>
		    </table>
	    </div>
	    <div id="MediaUploader" style="OVERFLOW:hidden;HEIGHT:300px">
		    <telerik:FileUploader id="fileUploader" runat="server" />
	    </div>
	    
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>	

    <div class="Ektron_Dialogs_ButtonContainer">
        <button class="Ektron_StandardButton" title="Insert" onclick="return OkClicked()" type="button">
            <script>localization.showText('Insert');</script>
	    </button>
	    <span class="Ektron_LeftSpaceSmall"></span>
	    <button class="Ektron_StandardButton" title="Close" onclick="CloseDlg()" type="button">
	        <script>localization.showText('Close');</script>
	    </button>
    </div>	
</div>

<asp:literal id="javascriptInitialize" Runat="server" />

<script language="javascript">
/*----------------Common functions------------------------*/
function ConfigureUploadPanel()
{
	if (messageHolderRowID)
	{
		if (isErrorVisible)
		{
			isErrorVisible = false;
		}
		else
		{
			var tr = document.getElementById(messageHolderRowID);
			if (tr && tr.style.display != "none") tr.style.display = "none";
		}
	}
	if (fileBrowser.CurrentItem)
	{
		document.getElementById('CurrentDirectoryBox').value = fileBrowser.CurrentItem.GetPath();
	}
}

function ShowPath(path)
{
	document.getElementById("FolderPathBox").value = path;
}

/* OK button clicked */
function OkClicked()
{
	if (fileBrowser.SelectedItem.Type == "D")
	{
		alert(localization["NoMediaSelectedToInsert"]);
		return;
	}

	if ((trim(document.getElementById("mediaWidth").value) == "") ||
		(trim(document.getElementById("mediaHeight").value) == ""))
	{
		alert(localization.AlertWidthHeight);
	}
	else
	{
		var mediaObject = previewer.GetHtml();
		CloseDlg(mediaObject);
	}
}

function SwitchPreviewMode()
{
	previewer.SwitchPreviewMode(fileBrowser.SelectedItem, FileFullName);
}

fileBrowser.OnFolderChange = function(browserItem)
{
	previewer.Preview(browserItem, FileFullName);
	ShowPath(browserItem.GetPath());
	TabHolder.SetTabEnabled(1, browserItem.Permissions & fileBrowser.UploadPermission);
};

fileBrowser.OnClientClick = function(browserItem)
{
	previewer.Preview(browserItem, FileFullName);
	ShowPath(browserItem.GetPath());
};

var FileFullName = null;

function OnLoad()
{
	dialogArgs = GetDialogArguments();
	FileFullName = dialogArgs.MediaPath;
	if (FileFullName)
	{
		ShowPath(fileBrowser.SelectedItem.GetPath());
		previewer.Preview(fileBrowser.SelectedItem, FileFullName);
	}

	var keys = GetProperties().keys;
	for (var i=0; i<keys.length; i++)
	{
		addOption(document.getElementById("property"), keys[i], keys[i]);
	}
	TabHolder.SetTabEnabled(1, fileBrowser.CurrentItem.Permissions & fileBrowser.UploadPermission);
	TabHolder.SelectCurrentTab();
	var itemToShow = fileBrowser.SelectedItem != null?fileBrowser.SelectedItem:fileBrowser.CurrentItem;
	ShowPath(itemToShow.GetPath());
}

AttachEvent(window, "load", OnLoad);
</script>
</asp:panel>
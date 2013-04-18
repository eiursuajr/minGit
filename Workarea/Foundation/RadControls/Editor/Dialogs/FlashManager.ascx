<%@ Control Language="C#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.FlashManager" AutoEventWireUp="false" CodeBehind="FlashManager.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FlashPreviewer" Src="../Controls/FlashPreviewer.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileBrowser" Src="../Controls/FileBrowser.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileUploader" Src="../Controls/FileUploader.ascx" %>

<asp:literal ID="messageHolder" Runat="server" />
<asp:panel id="actionControlsHolder" Runat="server">

<div id="MainContainer">
    <div class="Ektron_DialogTabstrip_Container">
	    <telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainTable">
		    <telerik:tab text="<script>localization.showText('Tab1HeaderText');</script>" selected="True"  elementid="FlashViewer"/>
		    <telerik:tab text="<script>localization.showText('Tab2HeaderText');</script>" onclientclick="ConfigureUploadPanel()" elementid="FlashUploader" enabled="false"/>
	    </telerik:tabcontrol>
    </div>

    <div class="Ektron_Dialog_Tabs_BodyContainer">
	    <div class="ErrorMessage" id="divErrorMessage" runat="server" visible="false"></div>
	    <div id="FlashViewer">
            <div class="Ektron_TopLabel" title="Directory"><script>localization.showText('Directory');</script></div>
            <input title="Enter Directory Text here" type="text" class="Ektron_WideTextBox" id="FolderPathBox">
		    <table>
			    <tr>
				    <td valign="top" title="File Browser">
					    <telerik:filebrowser id="fileBrowser" runat="server" />
				    </td>
				    <td title="Previewer"><span class="Ektron_LeftSpaceSmall"></span></td>
				    <td valign="top">
					    <telerik:flashpreviewer id="previewer" runat="server" />
				    </td>
			    </tr>
		    </table>
	    </div>
	    <div id="FlashUploader" style="OVERFLOW:hidden;HEIGHT:300px">
		    <telerik:FileUploader id="fileUploader" runat="server"/>
	    </div>
	    
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>
    		
    <div class="Ektron_Dialogs_ButtonContainer">			
	    <button title="Insert" class="Ektron_StandardButton" onclick="return OkClicked()" type="button">
		    <script>localization.showText('Insert');</script>
	    </button>
	    <span class="Ektron_LeftSpaceSmall"></span>
	    <button title="Close" class="Ektron_StandardButton" onclick="CloseDlg('');" type="button">
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
		alert(localization["NoFlashSelectedToInsert"]);
		return;
	}

	if ((trim(document.getElementById("flashWidth").value) == "") ||
		(trim(document.getElementById("flashHeight").value) == ""))
	{
		alert(localization.AlertWidthHeight);
	}
	else if ((document.getElementById("classYes").checked) && (trim(document.getElementById("classID").value) == ""))
	{
		alert(localization.AlertClassID);
	}
	else
	{
		if (dialogArgs && dialogArgs.StripAbsoluteImagesPaths == false)
		{
			previewer.StripAbsolutePath = false;
		}
		var flashObject = previewer.GetHtml();
		CloseDlg(flashObject);
	}
}

function SwitchPreviewMode()
{
	previewer.SwitchPreviewMode(fileBrowser.SelectedItem, FileFullName, dialogArgs);
}

fileBrowser.OnFolderChange = function(browserItem)
{
	previewer.Preview(browserItem, FileFullName, dialogArgs.Flash);
	ShowPath(browserItem.GetPath());
	TabHolder.SetTabEnabled(1, browserItem.Permissions & fileBrowser.UploadPermission);
};

fileBrowser.OnClientClick = function(browserItem)
{
	previewer.Preview(browserItem, FileFullName, dialogArgs);
	ShowPath(browserItem.GetPath());
};

var FileFullName = null;
var dialogArgs = null;

function OnLoad()
{
	dialogArgs = GetDialogArguments();
	FileFullName = dialogArgs.FlashPath;
	if (FileFullName)
	{
		ShowPath(fileBrowser.SelectedItem.GetPath());
		previewer.Preview(fileBrowser.SelectedItem, FileFullName, dialogArgs);
	}
	TabHolder.SetTabEnabled(1, fileBrowser.CurrentItem.Permissions & fileBrowser.UploadPermission);
	TabHolder.SelectCurrentTab();
	var itemToShow = fileBrowser.SelectedItem != null?fileBrowser.SelectedItem:fileBrowser.CurrentItem;
	ShowPath(itemToShow.GetPath());
}

AttachEvent(window, "load", OnLoad);
</script>
</asp:panel>
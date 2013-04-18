<%@ Control Language="C#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.ImageManager" AutoEventWireUp="false" CodeBehind="ImageManager.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="ImagePreviewer" Src="../Controls/ImagePreviewer.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileBrowser" Src="../Controls/FileBrowser.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileUploader" Src="../Controls/FileUploader.ascx" %>
<%@ Register TagPrefix="telerik" TagName="ThumbLinkOptionSetter" Src="../Controls/ThumbLinkOptionSetter.ascx"%>

<asp:literal ID="messageHolder" Runat="server" />
<asp:panel id="actionControlsHolder" Runat="server">

<div id="MainContainer">
    <div class="Ektron_DialogTabstrip_Container">
	    <telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainContainer">
		    <telerik:tab text="<script>localization.showText('Tab1HeaderText');</script>" selected="True" onclientclick="" elementid="ImageViewer"/>
		    <telerik:tab text="<script>localization.showText('Tab2HeaderText');</script>" onclientclick="ConfigureUploadPanel();" elementid="ImageUploader" enabled="false"/>
	    </telerik:tabcontrol>
    </div>

    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <div class="ErrorMessage" id="divErrorMessage" runat="server" visible="false"></div>
        <div id="ImageViewer">
			<div title="Image URL" class="Ektron_TopLabel"><script>localization.showText('ImageURL');</script></div>
            <input title="Enter Image URL" type="text" id="FolderPathBox" class="Ektron_WideTextBox">
	        <telerik:ThumbLinkOptionSetter id="mainThumbLinkOptionSetter" runat="server" />   
	        <div class="Ektron_TopSpaceVerySmall"></div>     
	        <table>
		        <tr>
			        <td valign="top" title="Fle Browser">
				        <telerik:filebrowser id="fileBrowser" runat="server" />
			        </td>
			        <td><span class="Ektron_LeftSpaceSmall"></span></td>
			        <td valign="top" title="Previewer">
				        <telerik:imagepreviewer id="previewer" runat="server" />
			        </td>
		        </tr>
	        </table>
        </div>
        <div title="Image Uploader" id="ImageUploader" style="OVERFLOW:hidden;">
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
        <button title="Close" class="Ektron_StandardButton" onclick="CloseDlg()" type="button">
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
		alert(localization["NoImageSelectedToInsert"]);
		return;
	}

	var image = {
		imagePath: "",
		linkImagePath: "",
		imageAltText: previewer.GetAltText()
	};

//	image.imagePath = document.getElementById("FolderPathBox").value;
	image.imagePath = fileBrowser.SelectedItem.GetUrl();
	var options = mainThumbLinkOptionSetter.GetOptions();
	var originalImage = fileBrowser.SelectedItem.GetOriginalImage(thumbAppendix);
	if (options.LinkToImage && originalImage)
	{
		image.linkImagePath = originalImage.GetUrl();
		image.targetToNew = options.TargetToNew;
	}

	retValue = image;
	//Ektron Editor starts
    var bCallCloseDlg = true;
    var paramList = EkUtil_parseQuery();
    if(typeof paramList.AccessChecks != "undefined")
    {
        var AccessChecks = paramList.AccessChecks.toLowerCase();
        if ("" == image.imageAltText && AccessChecks != "none") // Accessibility Required field(s) is blank.
        {
            switch (AccessChecks)
            {
                case "warn":
                    bCallCloseDlg = true;
                    if (confirm("The content is not accessibility compliance. Would you like to correct it?"))//TODO:localization["AccessChecksWarn"]
                    {
                        bCallCloseDlg = false;
                    }
                    break;
                case "enforce":
                    alert("Please fill in the accessibility compliance field(s)"); //TODO:localization["AccessChecksEnforce"]
                    bCallCloseDlg = false;
                    break;
                case "none":
                default:
		            bCallCloseDlg = true;
		            break;
		    }
        }    
    }  
    if (true == bCallCloseDlg)
    {
	CloseDlg(retValue);
    }
    //Ektron Editor ends
}

fileBrowser.OnFolderChange = function(browserItem)
{
	/* Show the file path in the text box*/
	ShowPath(browserItem.GetPath());
	/*Previewer - Clear it*/
	previewer.Clear();
	/*Tab - see if the new folder allows uploads and enable/disable the upload tab */
	TabHolder.SetTabEnabled(1, browserItem.Permissions & fileBrowser.UploadPermission);
};

fileBrowser.OnClientClick = function(browserItem)
{
	var imagePath = browserItem.GetUrl();
	if (browserItem.Type == "F")
	{
		previewer.LoadObjectFromPath(imagePath);
		if (browserItem.Attributes && browserItem.Attributes[0])
		{
			previewer.SetAltText(browserItem.Attributes[0]);
		}
	}
	else previewer.LoadObjectFromPath(null);
	ShowPath(browserItem.GetPath());

	var isThumbnail = false;
	if (imagePath != "/")
	{
		fileExists = browserItem.IsThumbnail(thumbAppendix);
		if (fileExists)
		{
			isThumbnail = true;
		}
	}
	mainThumbLinkOptionSetter.SetVisibility(document.all && isThumbnail);
};

function OnLoad()
{
	if (hasThumbnailCreationErrorOccurred)
	{
		previewer.ShowThumbnailCreator();
	}
	TabHolder.SetTabEnabled(1, fileBrowser.CurrentItem.Permissions & fileBrowser.UploadPermission);
	TabHolder.SelectCurrentTab();
	var itemToShow = fileBrowser.SelectedItem != null?fileBrowser.SelectedItem:fileBrowser.CurrentItem;
	ShowPath(itemToShow.GetPath());
	previewer.LoadObjectFromPath(itemToShow.Type == "F"?itemToShow.GetUrl():null);
}

AttachEvent(window, "load", OnLoad);
</script>
</asp:panel>
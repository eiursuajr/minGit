<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.TemplateManager" AutoEventWireUp="false" CodeBehind="TemplateManager.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="TemplatePreviewer" Src="../Controls/TemplatePreviewer.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileBrowser" Src="../Controls/FileBrowser.ascx" %>
<%@ Register TagPrefix="telerik" TagName="FileUploader" Src="../Controls/FileUploader.ascx" %>
<asp:literal ID="messageHolder" Runat="server"></asp:literal>
<asp:panel id="actionControlsHolder" Runat="server">
<table id="MainTable" width="550px" class="MainTable" border="0" cellpadding="0" cellspacing="0">
	<tr>
		<th align="left" valign="bottom">
			<telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainTable">
				<telerik:tab image="Dialogs/TabIcons/TemplateTab1.gif" text="<script>localization.showText('Tab1HeaderText');</script>" selected="True" onclientclick="" elementid="TemplateViewer"/>
				<telerik:tab image="Dialogs/TabIcons/TemplateTab2.gif" text="<script>localization.showText('Tab2HeaderText');</script>" onclientclick="ConfigureUploadPanel();" elementid="TemplateUploader" enabled="false"/>
			</telerik:tabcontrol>
		</th>
	</tr>
	<tr>
		<td class="MainTableContentCell" valign="top">
			<div class="ErrorMessage" id="divErrorMessage" runat="server" visible="false"></div>
			<div id="TemplateViewer" style="OVERFLOW:hidden;HEIGHT:300px">
				<table cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td colspan="3">
							<table cellpadding="0" cellspacing="0">
								<tr>
									<td nowrap class="Label" title="Template URL">&nbsp;<script>localization.showText('TemplateURL');</script></td>
									<td width="100%"><input title="Enter Template URL here" type="text" style="WIDTH:100%" class="RadETextBox" id="FolderPathBox"></td>
								</tr>
							</table>
						</td>
						<td rowspan="2" width="40" valign="top">
							<button class="Button" title="Ok" onclick="OkClicked();" type="button">
								<script>localization.showText('Insert');</script>
							</button><button class="Button" title="Close" onclick="CancelClicked();" type="button">
								<script>localization.showText('Close');</script>
							</button>
						</td>
					</tr>
					<tr>
						<td valign="top" title="File Browser">
							<telerik:filebrowser id="fileBrowser" runat="server"></telerik:filebrowser>
						</td>
						<td class="VerticalSeparator" nowrap></td>
						<td valign="top" title="Previewer">
							<telerik:templatepreviewer id="previewer" runat="server"></telerik:templatepreviewer>
						</td>
					</tr>
				</table>
			</div>
			<div id="TemplateUploader" style="display:none;" title="File Uploader">
				<telerik:fileUploader id="fileUploader" runat="server"/>
			</div>
		</td>
	</tr>
</table>
<asp:literal id="javascriptInitialize" Runat="server"></asp:literal>
<script language="javascript">
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

	var theTemplateManager = new TemplateManager(fileBrowser, previewer, TabHolder);
	
	function OkClicked()
	{
		if (!theTemplateManager.IsTemplateChosen())
		{
			alert(localization["NoTemplateSelectedToInsert"]);
			return;
		}
		CloseDlg(theTemplateManager.GetReturnResult());
	}
	function CancelClicked()
	{
		CloseDlg();
	}

	function InitControl()
	{
		theTemplateManager.Initialize();
	}

	AttachEvent(window, "load", InitControl);

</script>
</asp:panel>
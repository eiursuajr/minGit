<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImagePreviewer.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.ImagePreviewer" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="ThumbnailCreator" Src="./ThumbnailCreator.ascx" %>
<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td>
			<table align="left" border="0" cellpadding="0" cellspacing="0" class="ImageButtonHolder">
				<tr>
					<td><a href="#" id="bestFitButtonLink" onclick="<%=this.ID%>.HideThumbnailCreator();CallFitImage();">
						 <button title="Best Fit" class="ImageButton"><telerik:editorschemeimage id="bestFitIcon" align="absMiddle" relsrc="Dialogs/bestFitIcon.gif" runat="server" border="0"></telerik:editorschemeimage></button>
						</a><script>localization.setAttribute("bestFitButtonLink", "title", "BestFit");</script>
					</td>
					<td><a href="#" id="actualSizeButtonLink" onclick="<%=this.ID%>.HideThumbnailCreator();ShowActualImageSize();">
						<button title="Actual Size" class="ImageButton"><telerik:editorschemeimage id="actualSizeIcon" align="absMiddle" relsrc="Dialogs/actualSizeIcon.gif" runat="server" border="0"></telerik:editorschemeimage></button>
						</a><script>localization.setAttribute("actualSizeButtonLink", "title", "ActualSize");</script>
					</td>
					<td><a href="#" id="zoomInButtonLink" onclick="<%=this.ID%>.HideThumbnailCreator();ScaleImage(80,true);">
						<button title="Zoom in" class="ImageButton"><telerik:editorschemeimage id="zoomInIcon" align="absMiddle" relsrc="Dialogs/zoomInIcon.gif" runat="server" border="0"></telerik:editorschemeimage></button>
						</a><script>localization.setAttribute("zoomInButtonLink", "title", "ZoomIn");</script>
					</td>
					<td><a id="zoomOutButtonLink" href="#" onclick="<%=this.ID%>.HideThumbnailCreator();ScaleImage(40);">
						<button title="Zoom Out" class="ImageButton"><telerik:editorschemeimage id="zoomOutIcon" align="absMiddle" relsrc="Dialogs/zoomOutIcon.gif" runat="server" border="0"></telerik:editorschemeimage></button>
						</a><script>localization.setAttribute("zoomOutButtonLink", "title", "ZoomOut");</script>
					</td>
					<td><a id="<%=this.ID%>_thumbnailCreatorLink" href="#" onclick="<%=this.ID%>.SwitchThumbnailCreator();">
						<button title="Create Thumbnail Icon" class="ImageButton"><telerik:editorschemeimage id="thumbnailCreatorIcon" align="absMiddle" relsrc="Dialogs/ThumbnailCreatorIcon.gif" runat="server" border="0"></telerik:editorschemeimage></button>
						</a><script>localization.setAttribute("<%=this.ID%>_thumbnailCreatorLink", "title", "CreateThumbnailIconTitle");</script>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td>
			<div id="<%=this.ID%>_previewAreaHolder" class="PreviewAreaHolder">
				<table height="100%" style="width:100%;" align="center" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td colspan="2" valign="center" id="PreviewArea"></td>
					</tr>
					<tr id="<%=this.ID%>_altTextRow">
						<td nowrap class="label">
							<script type="text/javascript">localization.showText("ImageAltText");</script>
						</td>
						<td>
							<input type="text" class="RadETextbox" id="<%=this.ID%>_altText"/>
							<telerik:editorschemeimage id="accessibilityIcon" runat="server" relsrc="Dialogs/Accessibility.gif" border="0"/>
						</td>
					</tr>
				</table>
			</div>
			<div id="<%=this.ID%>_thumbnailCreatorHolder" style="display:none;">
				<telerik:ThumbnailCreator
					id="theThumbnailCreator"
					runat="server"
				/>
			</div>
		</td>
	</tr>
</table>
<script language="javascript">
	var theThumbnailCreator = null;
	var canCreateThumbnail = <%=this.AllowThumbGeneration.ToString().ToLower()%>;
	if (canCreateThumbnail)
	{
		var theThumbnailCreator = <%=this.theThumbnailCreator.ClientID%>;
	}
	var <%=this.ID%> = new ImagePreviewer('<%=this.ID%>', theThumbnailCreator, '<%=this.ThumbSuffix%>', canCreateThumbnail);
</script>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Flash.ascx.cs" Inherits="Widgets_Flash" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/widgets/ContentBlock/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<%@ Register Src="~/widgets/ContentBlock/taxonomytree.ascx" TagPrefix="UC" TagName="TaxonomyTree" %>

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:Label ID="errorLb" runat="server" />
        <div id="container">
            <asp:Literal ID="ltrFlash" runat="server"></asp:Literal>
            <CMS:ContentBlock ID="contentBlock" runat="server" Visible="false" />
        </div>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=uniqueId%>" class="FlashWidget">
            <div class="CBEdit">
                <asp:Label ID="editError" runat="server" />
                <div class="CBTabInterface">
                    <ul class="CBTabWrapper clearfix">
                        <li class="CBTab selected"><a href="#ByFolder"><span>Select File</span></a></li>
                        <li class="CBTab"><a href="#SelThumb"><span>Select Thumbnail</span></a></li>
                        <li class="CBTab"><a href="#Properties"><span>Properties</span></a></li>
                        <li class="CBTab"><a href="#Upload"><span>Upload</span></a></li>
                    </ul>
                    <div class="ByFolder CBTabPanel Panels">
                        <UC:FolderTree ID="foldertree" runat="server" />
                    </div>
                    <div class="ByFolder Panels">
                        <div class="CBResults">Select a folder to browse videos</div>
                        <div class="CBPaging"></div>
                    </div>
                    <div class="SelThumb CBTabPanel Panels" style="display:none;">
                        <UC:FolderTree ID="foldertree1" runat="server" />
                        <div class="hideThumb" style="display:none;">
                            Please select a video to play first.
                        </div>
                    </div>
                    <div class="SelThumb Panels" style="display:none;">
                        <div class="CBResults">Select a folder to browse thumbnails</div>
                        <div class="CBPaging"></div>
                    </div>
                    <div class="Properties CBTabPanel Panels" style="display:none;">
                        <table>
                            <tr>
                                <td>File Source:</td>
                                <td><span class="filesource" runat="server" id="txtSource"></span></td>
                            </tr>
                            <tr>
                                <td>File Height:</td>
                                <td><asp:TextBox ID="txtHeight" CssClass="height" runat="server" Width="40px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>File Width:</td>
                                <td><asp:TextBox ID="txtWidth" CssClass="width" runat="server" Width="40px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>Autostart:</td>
                                <td><asp:CheckBox ID="chkAutostart" CssClass="autostart" runat="server" Width="40px"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td>Video Thumbnail:</td>
                                <td>
                                    <span class="thumbnail" runat="server" id="thumbnailImg">None</span>
                                    <a href="#thumbChange" class="thumbChange" id="thumbChange" runat="server" onclick="return false;" style="display:none;">change</a>
                                    <a href="#thumbRemove" class="thumbRemove" id="thumbRemove" runat="server" onclick="return false;" style="display:none;">remove</a>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="Upload CBTabPanel Panels" style="display:none;">
                        <span style="float:left;">Current Path: <span class="curPath">Please select a folder</span></span>
                        <span style="float:right;" class="uploadType" data-filter="Flash (*.swf;*.flv)|*.swf;*.flv">Uploading Video</span>
                        <div style="clear:both;"></div>
                        <div style="height:160px;">
		                    <object data="data:application/x-silverlight," type="application/x-silverlight-2" width="100%" height="100%" id="<%=uniqueId %>_uploader">
			                    <param name="source" value="<%=sitePath %>/widgets/flash/FileUpload.xap"/>
                			    <param name="onerror" value="onSilverlightError" />
			                    <param name="background" value="white" />
			                    <param name="minRuntimeVersion" value="2.0.31005.0" />
			                    <param name="autoUpgrade" value="true" />
			                    <param name="initParams" value="UploadPage=<%=sitePath %>/widgets/flash/FlashHandler.ashx,Filter=Flash (*.swf;*.flv)|*.swf;*.flv,JavascriptGetQueryParamsFunction=Ektron.PFWidgets.Flash.getQueryString,JavascriptGetFilterFunction=Ektron.PFWidgets.Flash.getUploadFilter,JavascriptIndividualUploadFinishFunction=Ektron.PFWidgets.Flash.UploadReturn" />
			                    <a href="http://go.microsoft.com/fwlink/?LinkID=124807" style="text-decoration: none;">
     			                    You must have silverlight to use the uploader
			                    </a>
		                    </object>
		                    <iframe style='visibility:hidden;height:0;width:0;border:0px'></iframe>
                        </div>
                    </div>
                </div>
                <input type="hidden" ID="hdnVideoFolderPath" class="HiddenVideoFolderPath" name="HiddenVideoFolderPath" value="" runat="server" />
                <input type="hidden" id="hdnThumbFolderPath" class="hdnThumbFolderPath" name="hdnThumbFolderPath" value="" runat="server" />
                <input type="hidden" id="hdnFolderId" class="hdnFolderId" name="hdnFolderId" value="-1" runat="server" />
                <input type="hidden" id="hdnContentId" class="hdnContentId" name="hdnContentId" value="0" runat="server" />
                <input type="hidden" id="hdnThumbFile" class="hdnThumbFile" name="hdnThumbFile" value="-1" runat="server" />
                <input type="hidden" id="hdnThumbID" class="hdnThumbID" name="hdnThumbID" value="0" runat="server" />

                <div class="CBEditControls">
                    <button class="FlAdd" id="<%=uniqueId%>addButton" type="button">Upload</button>
                    <asp:Button ID="SaveButton" CssClass="CBSave" runat="server" Text="Save" OnClick="SaveButton_Click" />
                    <asp:Button ID="CancelButton" CssClass="CBCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                </div>
            </div>
            <input type="hidden" id="hdnAppPath" name = "hdnAppPath" value="<%=appPath%>" />
            <input type="hidden" id="hdnLangType" name ="hdnLangType" value="<%=langType%>" />
        </div>
    </asp:View>
</asp:MultiView>
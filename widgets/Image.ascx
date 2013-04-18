<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Image.ascx.cs" Inherits="Widgets_Image" %>
<%@ Register Src="~/widgets/image/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
    <asp:PlaceHolder ID="phContent" runat="server">
        <asp:Label ID="errorLb" runat="server" />
        <div id="container">
            <asp:Literal ID="ltrImage" runat="server"></asp:Literal>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">
            <div id="divHelpText" runat="server" style="font: normal 12px/15px arial; width: 100%; height: 100%;">
                Click on the 'Edit' icon (<img alt="edit icon" title="edit icon" src="<%=appPath %>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png" width="12" height="12" border="0" />) in the top-right corner of this widget
                to select an image you wish to display.
            </div>
        </asp:PlaceHolder>
    </asp:View>
    <asp:View ID="Edit" runat="server">    
        <div id="<%=uniqueId%>" class="ImageWidget">
            <div class="CBEdit">
                <asp:Label ID="editError" runat="server" />
                <div class="CBTabInterface">
                    <ul class="CBTabWrapper clearfix">
                        <li class="CBTab selected"><a href="#ByFolder"><span>Select File</span></a></li>
                        <li class="CBTab"><a href="#Properties"><span>Properties</span></a></li>
                        <li class="CBTab"><a href="#Upload"><span>Upload</span></a></li>
                    </ul>
                    <div class="ByFolder CBTabPanel Panels">
                        <UC:FolderTree ID="foldertree" runat="server" />
                    </div>
                    <div class="ByFolder Panels">
                        <div class="CBResults">
                           <p>To select an image already in a CMS400 folder, select the folder then choose the image from the list that will appear here.</p>
                           <p>To upload a new image from your computer, select the folder to which you want to upload it. Then, click the Upload tab and choose the image.</p>
                        </div>
                        <div class="CBPaging">
                        </div>
                    </div>
                    <div class="Properties CBTabPanel Panels" style="display: none;">
                        <table>
                            <tr>
                                <td>
                                    <span style="float: left;">Current Path: </span>
                                </td>
                                <td>
                                    <span id="uxCurSelectedPath" runat ="server"  class="curPath">Please select a folder</span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    File Source:
                                </td>
                                <td>
                                    <span class="filesource" runat="server" id="txtSource"></span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Tool Tip:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtToolTip" class="toolTip" runat="server" Style="width: 95%;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Image Height (px):
                                </td>
                                <td>
                                    <asp:TextBox ID="txtHeight" CssClass="height" runat="server" MaxLength="3" onkeypress="return AllowOnlyNumeric(event);"
                                        oncopy="return MouseClickEvent();" onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();"
                                        Style="width: 95%;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Image Width (px):
                                </td>
                                <td>
                                    <asp:TextBox ID="txtWidth" CssClass="width" runat="server" MaxLength="3" onkeypress="return AllowOnlyNumeric(event);"
                                        oncopy="return MouseClickEvent();" onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();"
                                        Style="width: 95%;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Border:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBorder" CssClass="imgborder" runat="server" Text="0" MaxLength="3"
                                        onkeypress="return AllowOnlyNumeric(event);" oncopy="return MouseClickEvent();"
                                        onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();" Style="width: 95%;"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="Upload CBTabPanel Panels" style="display: none;">
                        <span style="float: left;">Current Path: <span class="curPath">Please select a folder</span></span>
                        <span style="float: right;" class="uploadType" data-filter="Image files (*.gif;*.jpg;*.jpeg;*.png;*.bmp)|*.gif;*.jpg;*.jpeg;*.png;*.bmp">
                            Uploading Images</span>
                        <div style="clear: both;">
                        </div>
                        <div style="height: 160px;">
                            <object data="data:application/x-silverlight," type="application/x-silverlight-2"
                                width="100%" height="100%" id="<%=uniqueId %>_uploader">
                                <param name="source" value="<%=sitePath %>/widgets/image/FileUpload.xap" />
                                <param name="onerror" value="onSilverlightError" />
                                <param name="background" value="white" />
                                <param name="minRuntimeVersion" value="2.0.31005.0" />
                                <param name="autoUpgrade" value="true" />
                                <param name="initParams" value="UploadPage=<%=sitePath %>/widgets/image/ImageHandler.ashx,
                                Filter=Image files (*.gif;*.jpg;*.jpeg;*.png;*.bmp)|*.gif;*.jpg;*.jpeg;*.png;*.bmp,
                                JavascriptGetQueryParamsFunction=Ektron.PFWidgets.Image.getQueryString,
                                JavascriptGetFilterFunction=Ektron.PFWidgets.Image.getUploadFilter,
                                JavascriptIndividualUploadFinishFunction=Ektron.PFWidgets.Image.UploadReturn" />
                                <a href="http://go.microsoft.com/fwlink/?LinkID=124807" style="text-decoration: none;">
                                    You must have silverlight to use the uploader </a>
                            </object>
                            <iframe style='visibility: hidden; height: 0; width: 0; border: 0px'></iframe>
                        </div>
                    </div>
                </div>
                <input type="hidden" id="hdnFolderPath" class="hiddenFolderPath" name="hiddenFolderPath"
                    value="" runat="server" />
                <input type="hidden" id="hdnFolderId" class="hdnFolderId" name="hdnFolderId" value="-1"
                    runat="server" />
                <input type="hidden" id="hdnContentId" class="hdnContentId" name="hdnContentId" value="0"
                    runat="server" />
                <div class="CBEditControls">
                    <asp:Button ID="SaveButton" CssClass="CBSave" runat="server" Text="Save" OnClick="SaveButton_Click" OnClientClick="return Ektron.PFWidgets.Image.Save(this);" />
                    <asp:Button ID="CancelButton" CssClass="CBCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                </div>
            </div>
        </div>
    </asp:View>
</asp:MultiView>
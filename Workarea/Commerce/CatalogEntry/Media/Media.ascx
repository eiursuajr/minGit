<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Media.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Medias.Media" %>
<div class="EktronMediaTabWrapper">
    <input type="hidden" id="MediaTabData" name="MediaTabData" class="mediaTabData" value="" />
    <div class="mediaGroupHeader">
        <h3>Images</h3>
        <asp:PlaceHolder ID="phAddImageEdit" runat="server">
            <p class="addImage">
                <a href="#AddNewImage" class="button buttonRight greenHover addNewImage" onclick="Ektron.Commerce.MediaTab.Images.addNewImageShowForm();" >
                    <img class="addNewImage" title="<% =GetMessage("lbl add new image") %>" alt="<% =GetMessage("lbl add new image") %>" src="<% =GetMediaImageImagePath() %>/addImage.gif" />
                    <asp:Label runat="server" ID="lbl_AddNewImage" Visible="true" />
                </a>
                <a href="#AddImage" class="button buttonRight greenHover addImage" onclick="Ektron.Commerce.MediaTab.Images.addImageShowForm(<%= GetProductTypeId() %>);" >
                    <img class="addImage" title="<% =GetMessage("btn add image") %>" alt="<% =GetMessage("btn add image") %>" src="<% =GetMediaImageImagePath() %>/Image.gif" />
                    <asp:Label runat="server" ID="lbl_AddImage" Visible="true" />
                </a>
            </p>
        </asp:PlaceHolder>
    </div>
    <div class="mediaGroup">
        <ul class="mediaList images">
            <asp:PlaceHolder ID="phCloneItem" runat="server">
                <li class="mediaItemClone clearfix">
                    <input type="hidden" class="imageId" value="" />
                    <table class="mediaImage">
                        <tfoot>
                            <tr>
                                <td colspan="3">
                                    <p class="actions clearfix">
                                        <a href="#DefaultImageSelector" class="button buttonLeft greenHover gutterRight defaultImageSelector" title="Set as Default Selector" onclick="Ektron.Commerce.MediaTab.Images.setDefault(this);return false;">
                                            <input type="hidden" class="default" value="false" />
                                            <img class="setAsDefault" alt="<% =GetMessage("alt set default") %>" title="<% =GetMessage("alt set default") %>" src="<% =GetMediaImageImagePath() %>/Image.gif" />
                                            <span><% =GetSetDefaultLabel()%></span>
                                        </a>
                                        <a href="#ViewThumbnails" class="button buttonLeft blueHover viewThumbnails" title="View Thumbnails" onclick="Ektron.Commerce.MediaTab.Images.Thumbnails.toggle(this);return false;" id="aThumbnails" runat="server">
                                            <img class="viewThumbnails" alt="<% =GetMessage("lbl view thumbnails") %>" title="<% =GetMessage("lbl view thumbnails") %>" src="<% =GetMediaImageImagePath() %>/thumbnails.gif" />
                                            <span>View Thumbnails</span>
                                        </a>
                                        <a href="#Delete" class="button buttonRight redHover deleteImageSelector" title="" onclick="Ektron.Commerce.MediaTab.Images.markForDelete(this);return false;">
                                            <input type="hidden" class="markedForDelete" value="false" />
                                            <img class="delete" alt="<% =GetMessage("generic delete title") %>" title="<% =GetMessage("generic delete title") %>" src="<% =GetMediaImageImagePath() %>/toggleDelete.gif" />
                                            <img class="restore" alt="<% =GetMessage("generic restore title") %>" title="<% =GetMessage("generic restore title") %>" src="<% =GetMediaImageImagePath() %>/toggleDeleteUndo.gif" />
                                            <span>Mark For Delete</span>
                                        </a>
                                    </p>
                                </td>
                            </tr>
                        </tfoot>
                        <tbody>
                            <tr>
                                <th rowspan="7" class="mediaPreview showFullsizeModal">
                                    <span class="viewFullsize">view actual size</span>
                                    <img src="" title="" alt="" />
                                </th>
                                <th class="label">Title:</th>
                                <td class="title">
                                    <div class="edit">
                                        <input type="hidden" class="title" value="true" />
                                        <img class="revise" title="<% =GetMessage("generic edit title") %>" alt="<% =GetMessage("generic edit title") %>" src="<% =GetMediaImageImagePath() %>/revise.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.showForm($ektron(this).parent());" />
                                        <img class="reviseOK" title="<% =GetMessage("btn ok") %>" alt="<% =GetMessage("btn ok") %>" src="<% =GetMediaImageImagePath() %>/reviseOK.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.ok($ektron(this).parent());" />
                                        <img class="reviseCancel" title="<% =GetMessage("resolve cancel button") %>" alt="<% =GetMessage("resolve cancel button") %>" src="<% =GetMediaImageImagePath() %>/reviseCancel.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.cancel($ektron(this).parent());" />
                                        <span title="<% =GetMessage("generic edit title") %>">&#160;</span>
                                    </div>
                                </td>
                            </tr>
                            <tr class="stripe">
                                <th class="label">Alt Text:</th>
                                <td class="altText">
                                    <div class="edit">
                                        <input type="hidden" class="altText" value="true" />
                                        <img class="revise" title="<% =GetMessage("generic edit title") %>" alt="<% =GetMessage("generic edit title") %>" src="<% =GetMediaImageImagePath() %>/revise.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.showForm($ektron(this).parent());" />
                                        <img class="reviseOK" title="<% =GetMessage("btn ok") %>" alt="<% =GetMessage("btn ok") %>" src="<% =GetMediaImageImagePath() %>/reviseOK.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.ok($ektron(this).parent());" />
                                        <img class="reviseCancel" title="<% =GetMessage("generic cancel") %>" alt="<% =GetMessage("generic cancel") %>" src="<% =GetMediaImageImagePath() %>/reviseCancel.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.cancel($ektron(this).parent());" />
                                        <span title="<% =GetMessage("generic edit title") %>">&#160;</span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th class="label path">Path:</th>
                                <td class="path">
                                    <input type="hidden" class="path" value="true" />
                                    <div>&#160;</div>
                                </td>
                            </tr>
                            <tr class="stripe">
                                <th class="label">Width:</th>
                                <td class="width">
                                    <input type="hidden" class="path" value="true" />
                                    <span>&#160;</span>
                                </td>
                            </tr>
                            <tr>
                                <th class="label">Height:</th>
                                <td class="height">
                                    <input type="hidden" class="height" value="true" />
                                    <span>&#160;</span>
                                </td>
                            </tr>
                            <tr class="stripe">
                                <th class="label">Type:</th>
                                <td class="type">
                                    <span>&#160;</span>
                                </td>
                            </tr>
                            <tr>
                                <th class="label">
                                    <span style="white-space:nowrap;">Gallery Display:</span>
                                </th>
                                <td class="gallery">
                                    <select class="gallery" onchange="Ektron.Commerce.MediaTab.Images.setGalleryDisplay();return false;">
                                        <option value="true" selected="selected">Yes</option>
                                        <option value="false">No</option>
                                    </select>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="thumbnails">
                        <table class="thumbnailWrapper" summary="This table contains thumbnail images and provides the ability to select the default thumbnail image.">
                            <tbody>
                                <tr>
                                    <td>
                                        <div class="thumbnailImageWrapper">
                                            <table class="thumbnailWrapper">
                                                <tbody>
                                                    <tr class="thumbnailRowWrapper">
                                                        <td>
                                                            <p class="dimensions">
                                                                <span class="height"></span>&#160;x&#160;<span class="width"></span>
                                                            </p>
                                                            <p class="thumbnailPreview">
                                                                <input type="hidden" class="thumbnailHeight" name="mediaTab" value="0" />
                                                                <input type="hidden" class="thumbnailWidth" name="mediaTab" value="0" />
                                                                <input type="hidden" class="thumbnailPath" name="mediaTab" value="" />
                                                                <input type="hidden" class="thumbnailImageName" name="mediaTab" value="" />
                                                                <input type="hidden" class="thumbnailTitle" name="mediaTab" value="" />
                                                                <img alt="" class="thumbnail" src="" />
                                                            </p>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phNoImages" runat="server">
                <li class="mediaImageNone stripe">
                    <p class="mediaNoImage">
                        <asp:Literal ID="litNoImages" runat="server" />
                    </p>
                </li>
            </asp:PlaceHolder>
            <asp:Repeater ID="rptImage" OnItemDataBound="rptImage_ItemDataBound" runat="server">
                <ItemTemplate>
                    <li class="mediaItem clearfix">
                        <asp:PlaceHolder ID="phImageDataEdit" runat="server">
                            <input type="hidden" class="imageId" value="<%# DataBinder.Eval(Container.DataItem, "Id") %>" />
                        </asp:PlaceHolder>
                        <table class="mediaImage">
                            <tfoot>
                                <tr>
                                    <td colspan="3">
                                        <p class="actions clearfix">
                                            <asp:PlaceHolder ID="phDefaultButtonEdit" runat="server">
                                                <a href="#DefaultImageSelector" class="button buttonLeft greenHover gutterRight defaultImageSelector<%# GetIsDefaultImageClass((DataBinder.Eval(Container.DataItem, "FilePath"))) %>" title="Set as Default Selector" onclick="Ektron.Commerce.MediaTab.Images.setDefault(this);return false;">
                                                    <input type="hidden" class="default" value="<%# GetIsDefaultImageField((DataBinder.Eval(Container.DataItem, "FilePath"))) %>" />
                                                    <img class="setAsDefault" alt="<% =GetMessage("alt set default") %>" title="<% =GetMessage("alt set default") %>" src="<% =GetMediaImageImagePath() %>/Image.gif" />
                                                    <span><%# GetIsDefaultImageLabel((DataBinder.Eval(Container.DataItem, "FilePath"))) %></span>
                                                </a>
                                            </asp:PlaceHolder>
                                            <a href="#ViewThumbnails" class="button buttonLeft blueHover viewThumbnails" title="View Thumbnails" onclick="Ektron.Commerce.MediaTab.Images.Thumbnails.toggle(this);return false;" id="aThumbnails" runat="server">
                                                <img class="viewThumbnails" alt="<% =GetMessage("lbl view thumbnails") %>" title="<% =GetMessage("lbl view thumbnails") %>" src="<% =GetMediaImageImagePath() %>/thumbnails.gif" />
                                                <span>View Thumbnails</span>
                                            </a>
                                            <asp:PlaceHolder ID="phDeleteButtonEdit" runat="server">
                                                <a href="#Delete" class="button buttonRight redHover deleteImageSelector" title="<% =GetMessage("generic delete title") %>" onclick="Ektron.Commerce.MediaTab.Images.markForDelete(this);return false;">
                                                    <input type="hidden" class="markedForDelete" value="false" />
                                                    <img class="delete" alt="<% =GetMessage("generic delete title") %>" title="<% =GetMessage("generic delete title") %>" src="<% =GetMediaImageImagePath() %>/toggleDelete.gif" />
                                                    <img class="restore" alt="<% =GetMessage("generic restore title") %>" title="<% =GetMessage("generic restore title") %>" src="<% =GetMediaImageImagePath() %>/toggleDeleteUndo.gif" />
                                                    <span>Mark For Delete</span>
                                                </a>
                                            </asp:PlaceHolder>
                                        </p>
                                    </td>
                                </tr>
                            </tfoot>
                            <tbody>
                                <tr>
                                    <th rowspan="7" class="mediaPreview showFullsizeModal">
                                        <span class="viewFullsize">view actual size</span>
                                        <img src="<%# FixCacheImage(DataBinder.Eval(Container.DataItem, "FilePath")) %>" title="<%# DataBinder.Eval(Container.DataItem, "Alt") %>" alt="<%# DataBinder.Eval(Container.DataItem, "Alt") %>" data-ektron-commerce-media-image-width="<%# DataBinder.Eval(Container.DataItem, "Width") %>" data-ektron-commerce-media-image-height="<%# DataBinder.Eval(Container.DataItem, "Height") %>" />
                                    </th>
                                    <th class="label">Title:</th>
                                    <td class="title">
                                        <asp:PlaceHolder ID="phTitleEdit" runat="server">
                                            <div class="edit">
                                                <input type="hidden" class="title" value="<%# DataBinder.Eval(Container.DataItem, "FileName") %>" />
                                                <img class="revise" title="<% =GetMessage("generic edit title") %>" alt="<% =GetMessage("generic edit title") %>" src="<% =GetMediaImageImagePath() %>/revise.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.showForm($ektron(this).parent());" />
                                                <img class="reviseOK" title="<% =GetMessage("btn ok") %>" alt="<% =GetMessage("btn ok") %>" src="<% =GetMediaImageImagePath() %>/reviseOK.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.ok($ektron(this).parent());" />
                                                <img class="reviseCancel" title="<% =GetMessage("generic cancel") %>" alt="<% =GetMessage("generic cancel") %>" src="<% =GetMediaImageImagePath() %>/reviseCancel.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.cancel($ektron(this).parent());" />
                                                <span title="<% =GetMessage("generic edit title") %>"><%# DataBinder.Eval(Container.DataItem, "FileName") %></span>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phTitleView" runat="server">
                                            <span title="<% =GetMessage("generic edit title") %>"><%# DataBinder.Eval(Container.DataItem, "FileName") %></span>
                                        </asp:PlaceHolder>
                                    </td>
                                </tr>
                                <tr class="stripe">
                                    <th class="label">Alt Text:</th>
                                    <td class="altText">
                                        <asp:PlaceHolder ID="phAltTextEdit" runat="server">
                                            <div class="edit">
                                                <input type="hidden" class="altText" value="<%# DataBinder.Eval(Container.DataItem, "Alt") %>" />
                                                <img class="revise" title="<% =GetMessage("generic edit title") %>" alt="<% =GetMessage("generic edit title") %>" src="<% =GetMediaImageImagePath() %>/revise.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.showForm($ektron(this).parent());" />
                                                <img class="reviseOK" title="<% =GetMessage("btn ok") %>" alt="<% =GetMessage("btn ok") %>" src="<% =GetMediaImageImagePath() %>/reviseOK.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.ok($ektron(this).parent());" />
                                                <img class="reviseCancel" title="<% =GetMessage("generic cancel") %>" alt="<% =GetMessage("generic cancel") %>" src="<% =GetMediaImageImagePath() %>/reviseCancel.gif" onclick="Ektron.Commerce.MediaTab.Images.Edit.cancel($ektron(this).parent());" />
                                                <span title="title=<% =GetMessage("generic edit title") %>""><%# DataBinder.Eval(Container.DataItem, "Alt") %>&#160;</span>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phAltTextView" runat="server">
                                            <span title="<% =GetMessage("generic edit title") %>"><%# DataBinder.Eval(Container.DataItem, "Alt") %>&#160;</span>
                                        </asp:PlaceHolder>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="label path">Path:</th>
                                    <td class="path">
                                        <asp:PlaceHolder ID="phPathEdit" runat="server">
                                            <input type="hidden" class="path" value="<%# DataBinder.Eval(Container.DataItem, "FilePath") %>" />
                                        </asp:PlaceHolder>
                                        <div><%# DataBinder.Eval(Container.DataItem, "FilePath") %></div>
                                    </td>
                                </tr>
                                <tr class="stripe">
                                    <th class="label">Width:</th>
                                    <td class="width">
                                        <input type="hidden" class="width" value="" />
                                        <span>&#160;</span>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="label">Height:</th>
                                    <td class="height">
                                        <input type="hidden" class="height" value="" />
                                        <span>&#160;</span>
                                    </td>
                                </tr>
                                <tr class="stripe">
                                    <th class="label">Type:</th>
                                    <td class="type">
                                        <span>&#160;</span>
                                    </td>
                                </tr>
                                <tr>
                                    <th class="label">
                                        <span style="white-space:nowrap;">Gallery Display:</span>
                                    </th>
                                    <td class="gallery">
                                        <asp:PlaceHolder ID="phGalleryEdit" runat="server">
                                            <select class="gallery" onchange="Ektron.Commerce.MediaTab.Images.getImageData();">
                                                <option value="true" <%# GetSelectedDisplay(true, (DataBinder.Eval(Container.DataItem, "IncludedInGallery"))) %>>Yes</option>
                                                <option value="false" <%# GetSelectedDisplay(false, (DataBinder.Eval(Container.DataItem, "IncludedInGallery"))) %>>No</option>
                                            </select>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phGalleryView" runat="server">
                                            <span><%# GetGalleryLabel((DataBinder.Eval(Container.DataItem, "IncludedInGallery"))) %></span>
                                        </asp:PlaceHolder>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <asp:PlaceHolder ID="phThumbnails" runat="server" Visible="true">
                            <div class="thumbnails">
                                <table class="thumbnailWrapper" summary="This table contains thumbnail images and provides the ability to select the default thumbnail image.">
                                    <tbody>
                                        <tr>
                                            <td>
                                                <div class="thumbnailImageWrapper">
                                                    <table class="thumbnailWrapper">
                                                        <tbody>
                                                            <tr class="thumbnailRowWrapper">
                                                                <asp:Repeater ID="rptThumbnail" runat="server">
                                                                    <ItemTemplate>
                                                                        <td>
                                                                            <p class="dimensions">
                                                                                <span class="height"></span>&#160;x&#160;<span class="width"></span>
                                                                            </p>
                                                                            <p class="thumbnailPreview">
                                                                                <input type="hidden" class="thumbnailHeight" name="mediaTab" value="<%# GetThumbnailImageHeight(DataBinder.Eval(Container.DataItem, "Height")) %>" />
                                                                                <input type="hidden" class="thumbnailWidth" name="mediaTab" value="<%# GetThumbnailImageWidth(DataBinder.Eval(Container.DataItem, "Width")) %>" />
                                                                                <input type="hidden" class="thumbnailPath" name="mediaTab" value="<%# GetThumbnailImagePath(DataBinder.Eval(Container.DataItem, "FilePath")) %>" />
                                                                                <input type="hidden" class="thumbnailImageName" name="mediaTab" value="<%# DataBinder.Eval(Container.DataItem, "FileName") %>" />
                                                                                <input type="hidden" class="thumbnailTitle" name="mediaTab" value="<%# GetThumbnailImageTitleData(Container.DataItem) %>" />
                                                                                <img alt="<%# GetThumbnailImageAltText(Container.DataItem) %>" title="<%# GetThumbnailImageTitle(Container.DataItem) %>" class="thumbnail" src="<%# GetThumbnailPath(FixCacheImage(DataBinder.Eval(Container.DataItem, "FilePath")) )%>" />
                                                                            </p>
                                                                        </td>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </asp:PlaceHolder>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="modal">
            <div id="ektronMediaModal" class="ektronWindow">
                <h4 class="ektronMediaModalHeader clearfix">
                    <span>Actual Size</span>
                    <img class="ektronModalClose" alt="<% =GetMessage("lbl close window") %>" title="<% =GetMessage("lbl close window") %>" src="<% =GetMediaImageImagePath() %>/closeButton.gif" />
                </h4>
                <table class="fullsizeMedia" summary="Selected image and dimensions">
                    <tbody>
                        <tr>
                            <th title="Image height" class="height">
                                <span>&#160;</span>
                            </th>
                            <td>
                                <div class="fullsizeMediaWrapper">&#160;</div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>&#160;</span>
                            </td>
                            <th title="Image width" class="width">
                                <span>&#160;</span>
                            </th>
                        </tr>
                    </tbody>
                </table>
            </div>
            <asp:PlaceHolder ID="phModal" runat="server" Visible="false">
                <div id="ektronMediaAddNewImageModal" class="ektronWindow">
                    <h4 class="ektronMediaModalHeader clearfix">
                        <span>Add New Image</span>
                        <img class="ektronModalClose" alt="<% =GetMessage("lbl close window") %>" title="<% =GetMessage("lbl close window") %>" src="<% =GetMediaImageImagePath() %>/closeButton.gif" />
                    </h4>
                    <input type="hidden" class="iframeAddImageSrc" id="inputAddImageIframeSrc" runat="server" />
                    <iframe id="iframeAddImage" runat="server" frameborder="0" scrolling="auto"></iframe>
                    <p class="cancelAddImageModal clearfix">
                        <a href="#Cancel" class="button buttonRight redHover ektronModalClose" title="<% =GetMessage("generic cancel") %>" onclick="return false;">
                            <img class="cancel" alt="<% =GetMessage("generic cancel") %>" title="<% =GetMessage("generic cancel") %>" src="<% =GetMediaImageImagePath() %>/delete.gif" />
                            <span>Cancel</span>
                        </a>
                    </p>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</div>

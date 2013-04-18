<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RotatingBanner.ascx.cs"
    Inherits="widgets_RotatingBanner" %>
<%@ Register Src="~/widgets/RotatingBanner/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<input type="hidden" value='<%=ImageHeight %>' id="hdnImageHeight" />
<input type="hidden" value='<%=ImageWidth %>' id="hdnImageWidth" />
<input type="hidden" value='<%=Convert.ToString(MaxResult) %>' id="hdnMaxResult" />
<input type="hidden" value='<%=Convert.ToString(Duration) %>' id="hdnDuration" />
<input type="hidden" value='<%=Convert.ToString(CollectionID) %>' id="hdnCollID" />
<input type="hidden" value='<%=Convert.ToString(Title) %>' id="hdnShowTitle" />
<input type="hidden" value='<%=Convert.ToString(Teaser) %>' id="hdnShowTeaser" />
<input type="hidden" value='<%=Convert.ToString(ShowMediaControl) %>' id="hdnShowMediaControl" />
<input type="hidden" value='<%=RandCallID %>' id="hdnRandCallID" />
<input type="hidden" value='<%=sitePath %>' id="hdnSitePath" />

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:PlaceHolder ID="phContent" runat="server">
            <div id="ticker">
                <div id="ticker_content">
                    <div style="float: left;">
                        <div id="ticker_content_one">
                        </div>
                        <div id="ticker_content_two">
                        </div>
                    </div>
                    <br style="clear: both;" />
                    <div id="ticker_nav" style="float: left;">
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">         
            <div id="divHelpText" runat="server" style="font:normal 12px/15px arial; width:100%; height:100%;">
               Click on the 'Edit' icon (<img alt="edit icon" title="edit icon" src="<%=appPath %>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png" width="12" height="12" border="0" />) in the top-right corner of this widget to select the collection item you wish to display.
            </div>
        </asp:PlaceHolder>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=uniqueId%>" class="ImageWidget">
            <div class="CBEdit">
                <asp:Label ID="editError" runat="server" />
                <div class="CBTabInterface">
                    <ul class="CBTabWrapper clearfix">
                        <li class="CBTab selected"><a href="#Properties"><span>Properties</span></a></li>
                        <%if (isAdmin)
                          { %>
                        <li class="CBTab"><a href="#Upload"><span>Upload</span></a></li>
                        <%} %>
                    </ul>
                    <div class="Properties CBTabPanel Panels" style="display: none;">
                        <table>
                            <tr>
                                <td>
                                    Available Collections:
                                </td>
                                <td>
                                    <asp:DropDownList ID="uxCollections" runat="server" onchange="fnChange(this);">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Image Width (in px.):
                                </td>
                                <td>
                                    <asp:TextBox ID="uxImageWidth" onkeypress="return AllowOnlyNumeric(event);" oncopy="return MouseClickEvent();"
                                        onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Image Height (in px.):
                                </td>
                                <td>
                                    <asp:TextBox ID="uxImageHeight" onkeypress="return AllowOnlyNumeric(event);" oncopy="return MouseClickEvent();"
                                        onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Show Title:
                                </td>
                                <td>
                                    <asp:CheckBox ID="uxShowTitle" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Show Teaser:
                                </td>
                                <td>
                                    <asp:CheckBox ID="uxShowTeaser" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Show Media Control:
                                </td>
                                <td>
                                    <asp:CheckBox ID="uxShowMediaControl" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Max Result:
                                </td>
                                <td>
                                    <asp:TextBox runat="server" onkeypress="return AllowOnlyNumeric(event);" oncopy="return MouseClickEvent();"
                                        onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();" ID="uxMaxResult"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Rotate duration (in Sec.):
                                </td>
                                <td>
                                    <asp:TextBox runat="server" onkeypress="return AllowOnlyNumeric(event);" oncopy="return MouseClickEvent();"
                                        onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();" ID="uxRotatingDuration"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="Upload CBTabPanel Panels" style="display: none;">
                        <div class="ByFolder CBTabPanelFolder">
                            <UC:FolderTree ID="foldertree" runat="server" />
                        </div>
                        <span style="float: left;">Current Path: <span class="curPath">Please select a folder</span></span>
                        &nbsp;&nbsp;<span id="CollDetails">Current Collection:
                            <%=CollectionName+" (Id:" +Convert.ToString(CollectionID) +")"%>
                        </span><span style="float: right;" class="uploadType" data-filter="Image files (*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png">
                            Upload Images</span>
                        <div style="clear: both;">
                        </div>
                        <div style="height: 160px;">
                            <object data="data:application/x-silverlight," type="application/x-silverlight-2"
                                width="100%" height="100%" id="<%=uniqueId %>_uploader">
                                <param name="source" value="<%=sitePath %>widgets/RotatingBanner/FileUpload.xap" />
                                <param name="onerror" value="onSilverlightError" />
                                <param name="background" value="white" />
                                <param name="minRuntimeVersion" value="2.0.31005.0" />
                                <param name="autoUpgrade" value="true" />
                                <param name="initParams" value="UploadPage=<%=sitePath %>widgets/RotatingBanner/ImageHandler.ashx,
                                Filter=Image files (*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png,
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
                    <asp:TextBox ID="tbData" runat="server" Style="display: none;">
                    </asp:TextBox>
                    <asp:Button ID="uxSave" CssClass="CBSave" runat="server" Text="Save" OnClick="uxSave_Click" />
                    <asp:Button ID="uxCancel" CssClass="CBCancel" runat="server" Text="Cancel" OnClick="uxCancel_Click" />
                </div>
            </div>
        </div>
    </asp:View>
</asp:MultiView>
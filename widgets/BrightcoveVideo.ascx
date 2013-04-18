<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BrightcoveVideo.ascx.cs"
    Inherits="Widgets_BrightcoveVideo" %>
<script language="javascript" type="text/javascript" src="http://admin.brightcove.com/js/BrightcoveExperiences_all.js"></script>
<div id="<%= ClientID %>" class="ektronWidgetBrightcove">
    <asp:MultiView ID="BCViewSet" runat="server">
        <asp:View ID="Initial" runat="server">
            <asp:PlaceHolder runat="server" ID="phInitialInstructions" Visible="false">
                <h4>Click the Edit icon to configure your Brightcove settings</h4>
            </asp:PlaceHolder>
        </asp:View>
        <asp:View ID="Settings" runat="server">
            <div class="settingsWrapper">
                <div class="referralwrapper">
                    <a href="http://roia.biz/im/n/.Gl1vq1BAAGWpEMAAAsiQgAAsUBmMQA-A/" target="_blank">
                    </a>
                </div>
                <div class="settingsFormWrapper">
                    <div class="settingsFromInner">
                        <div class="settingsIntro">
                            <h3>
                                Have an existing Brightcove account?</h3>
                            <p class="subHeader">
                                Link your account and access your media library.</p>
                            <p class="StepCounter">
                                Settings</p>
                        </div>
                        <table>
                            <tr>
                                <td class="label">
                                    Publisher ID:
                                </td>
                                <td class="data">
                                    <asp:TextBox ID="tbPublisherID" runat="server"></asp:TextBox>
                                </td>
                                <td class="help">
                                    <img alt="publisher ID help" src="<%= SitePath %>widgets/brightcovevideo/images/help.png"
                                        href="#pubHelp" rel="#pubHelp" />
                                        <div id="pubHelp" >
                                        <div class="helpTitle">
                                            Where can I find my Publisher ID?</div>
                                        <div class="helpBody">
                                            Please log in to the Brightcove Studio to obtain your Publisher ID. A valid Publisher
                                            ID is required.</div>
                                        <div>
                                            <a href="http://www.brightcove.com/" target="_blank">Go to the Brightcove Studio</a></div>
                                </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Read Token:
                                </td>
                                <td class="data">
                                    <asp:TextBox ID="tbReadToken" runat="server" CssClass="readtoken"></asp:TextBox>
                                </td>
                                <td class="help">
                                    <img alt="Read Token Help" src="<%= SitePath %>widgets/brightcovevideo/images/help.png"
                                        href="#readHelp" rel="#readHelp" />
                                    <div id="readHelp" >
                                        <div class="helpTitle">
                                            Where can I find my Read Token?</div>
                                        <div class="helpBody">
                                            Please log in to the Brightcove Studio to obtain your Read Token. A valid Read Token
                                            is required.</div>
                                        <div>
                                            <a href="http://www.brightcove.com/" target="_blank">Go to the Brightcove Studio</a></div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Write Token:
                                </td>
                                <td class="data">
                                    <asp:TextBox ID="tbWriteToken" runat="server"></asp:TextBox>
                                </td>
                                <td class="help">
                                    <img alt="Write Token help" src="<%= SitePath %>widgets/brightcovevideo/images/help.png"
                                        href="#writeHelp" rel="#writeHelp" />
                                    <div id="writeHelp" >
                                        <div class="helpTitle">
                                            Where can I find my Write Token?</div>
                                        <div class="helpBody">
                                            Please log in to the Brightcove Studio to obtain your Write Token. This value is
                                            optional.</div>
                                        <div>
                                            <a href="http://www.brightcove.com/" target="_blank">Go to the Brightcove Studio</a></div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">
                                    Player IDs:
                                </td>
                                <td class="data">
                                    <asp:TextBox ID="tbPlayerIds" runat="server"></asp:TextBox>
                                </td>
                                <td class="help">
                                    <img alt="Player ID Help" src="<%= SitePath %>widgets/brightcovevideo/images/help.png"
                                        href="#playerHelp" rel="#playerHelp" />
                                    <div id="playerHelp">
                                        <div class="helpTitle">
                                            Where can I find my PlayerID?</div>
                                        <div class="helpBody">
                                            Please log in to the Brightcove Studio to obtain your Player ID. This value is required.
                                            Add multiple separated by commas.</div>
                                        <div>
                                            <a href="http://www.brightcove.com/" target="_blank">Go to the Brightcove Studio</a></div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <div class="warningWrapper">
                            <asp:RequiredFieldValidator ID="rfvPublisherID" runat="server" ControlToValidate="tbPublisherID"
                                ValidationGroup="settings" Display="Static" ForeColor="#DB341D" CssClass="tokenwarning"><span class="warningTitle">Publisher ID </span>is required.</asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfvReadToken" runat="server" ControlToValidate="tbReadToken"
                                ValidationGroup="settings" Display="Static" ForeColor="#DB341D" CssClass="tokenwarning"><span class="warningTitle">Read Token </span>is required.</asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfvPlayer" runat="server" ControlToValidate="tbPlayerIds"
                                ValidationGroup="settings" Display="Static" ForeColor="#DB341D" CssClass="tokenwarning"><span class="warningTitle">Player ID </span>is required.</asp:RequiredFieldValidator>
                            <span class="tokenwarning"><span class="tokenError"><span class="warningTitle">Read
                                Token </span><span>is invalid</span></span></span>
                        </div>
                        <div class="actionRow">
                            <asp:LinkButton ID="lbSaveSettings" runat="server" Text="Save" CssClass="bcButton saveSettingsBtn"
                                OnClick="btnSaveSettingsClk" ValidationGroup="settings"></asp:LinkButton>
                            <asp:LinkButton ID="lbCancelSettings" runat="server" Text="Cancel" CssClass="bcButton"
                                OnClick="btnCancelSettingsClk"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </asp:View>
        <asp:View ID="NoVideo" runat="server">
            <asp:PlaceHolder runat="server" ID="phNoVideoInstructions" Visible="false">
                <h4>Click the Edit icon to select a video</h4>
            </asp:PlaceHolder>
        </asp:View>
        <asp:View ID="View" runat="server">
            <!-- Start of Brightcove Player -->
            <div style="display: none">
            </div>
            <!--
                By use of this code snippet, I agree to the Brightcove Publisher T and C 
                found at http://corp.brightcove.com/legal/terms_publisher.cfm. 
            -->
            <object id="<%= ClientID %>myExperience" class="BrightcoveExperience">
                <param name="id" value="BCViewPlayer" />
                <param name="bgcolor" value="#FFFFFF" />
                <param name="width" value="<%=Width %>" />
                <param name="height" value="<%=Height %>" />
                <param name="playerID" value="<%=PlayerID %>" />
                <param name="publisherID" value="<%=PublisherID %>" />
                <param name="@videoPlayer" value="<%=VideoID%>" />
                <param name="isVid" value="true" />
                <param name="isUI" value="true" />
                <param name="@playlistTabs" value="<%=PlaylistID %>" />
                <param name="wmode" value="transparent" />
            </object>
            <!-- End of Brightcove Player -->
            <div id="<%= ClientID %>reloadVideo">
            </div>
        </asp:View>
        <asp:View ID="EmbedView" runat="server">
            <asp:Literal ID="ltrEmbedCode" runat="server"></asp:Literal>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div class="editWrapper">
                <div class="quickPublish">
                    <div class="editHeader clearfix">
                        <h3>
                            Select a video from your Brightcove account</h3>
                        <div class="options">
                            <span class="embedLink">Enter Embed Code</span> <span></span><span id="uploadLink"
                                runat="server" visible="false" class="uploadLink">
                                <img src="<%= SitePath %>widgets/brightcovevideo/images/icoUpload.png" />Upload
                                a video <span class="sep">|</span> </span>
                        </div>
                    </div>
                    <div class="borderfull">
                        <div class="borderbottom clearfix">
                            <div class="playlistSearchInput">
                            </div>
                            <div class="videoSearchInput borderleft">
                                <asp:DropDownList ID="ddlSearchOptions" runat="server" CssClass="searchOptions" EnableViewState="false">
                                    <asp:ListItem Text="Name & Description" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Tags" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="Reference ID" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Custom Fields" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                                <span class="searchBorder">
                                    <asp:TextBox ID="tbSearchInput" runat="server" CssClass="searchInput"></asp:TextBox>
                                    <img alt="search execute" src="<%= SitePath %>widgets/brightcovevideo/images/icoSearch.gif"
                                        class="executeSearch" />
                                </span>
                                <asp:DropDownList ID="ddlSortOptions" runat="server" CssClass="searchSort" EnableViewState="false">
                                    <asp:ListItem Text="Sort By..." Value="DISPLAY_NAME" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Name" Value="DISPLAY_NAME"></asp:ListItem>
                                    <asp:ListItem Text="Publish Date" Value="PUBLISH_DATE"></asp:ListItem>
                                    <asp:ListItem Text="Modified Date" Value="MODIFIED_DATE"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddlSortOrder" runat="server" CssClass="sortOrder" EnableViewState="false">
                                    <asp:ListItem Text="Desc" Value="DESC" Selected="True" />
                                    <asp:ListItem Text="Asc" Value="ASC" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="clearfix">
                            <div class="playlistList">
                                <ul id="videoCategory">
                                    <%--PlaceHolder videoCategoryListtmpl--%>
                                </ul>
                            </div>
                            <div class="videoList borderleft borderright borderbottom">
                                <div class="listheader">
                                    <span class="bcName">Video Name</span> <span class="bcDate">Last Updated</span>
                                </div>
                                <div class="videoListList">
                                    <ul id="videolisttarget">
                                        <%--PlaceHolder videoListtmpl--%>
                                    </ul>
                                    <div id="loader">
                                    </div>
                                </div>
                                <div class="previewVideoPlayer">
                                    <%--PlaceHolder previewtmpl--%>
                                </div>
                            </div>
                            <div class="videoDetail">
                                <div class="videoInfo" id="videoInfo">
                                    <%--PlaceHolder videoDetailtmpl--%>
                                </div>
                                <div class="playerSelection">
                                    <span>Player</span>
                                    <asp:DropDownList ID="ddlPlayers" runat="server">
                                    </asp:DropDownList>
                                    <span class="help">
                                        <img alt="Player ID Help" src="<%= SitePath %>widgets/brightcovevideo/images/help.png"
                                            href="#playerHelp" rel="#playerHelp" />
                                        <div id="playerHelp">
                                            <div class="helpTitle">
                                                What's this?</div>
                                            <div class="helpBody">
                                                In Brightcove Studio, you can configure custom video players. Add additional players
                                                in the Ektron global widget settings.</div>
                                            <div>
                                                <a href="http://www.brightcove.com/" target="_blank">Go to the Brightcove Studio</a></div>
                                        </div>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="DimensionsErrorDisplay">
                        <asp:RequiredFieldValidator ID="widthValidator" runat="server" ControlToValidate="tbWidth" ValidationGroup="Dimensions" Display="Dynamic" ForeColor="#DB341D" CssClass="tokenwarning"><span class="warningTitle">Video Width </span>is required.<br /></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="heightValidator" runat="server" ControlToValidate="tbHeight" ValidationGroup="Dimensions" Display="Dynamic" ForeColor="#DB341D" CssClass="tokenwarning"><span class="warningTitle">Video Height </span>is required.<br /></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator id="widthFormatValidator" runat="server" ControlToValidate="tbWidth" ValidationGroup="Dimensions" Display="Dynamic" ValidationExpression="^\d+$" CssClass="tokenwarning">Please enter a valid <span class="warningTitle">Video Width</span>.<br /></asp:RegularExpressionValidator>
                        <asp:RegularExpressionValidator id="heightFormatValidator" runat="server" ControlToValidate="tbHeight" ValidationGroup="Dimensions" Display="Dynamic" ValidationExpression="^\d+$" CssClass="tokenwarning">Please enter a valid <span class="warningTitle">Video Height</span>.<br /></asp:RegularExpressionValidator>
                    </div>
                    <div class="ekBCfooter clearfix">
                        <div class="ekBCleft">
                            <span class="ekBCexternalLink">Inactive videos are not shown. To see all your videos visit
                                the <a target="_blank" href="http://www.brightcove.com">Brightcove Studio</a>
                            </span>
                        </div>
                        <div class="ekBCleft dimensions">
                            <span>Width:</span><asp:TextBox ID="tbWidth" runat="server"></asp:TextBox>
                            <span>Height:</span><asp:TextBox ID="tbHeight" runat="server"></asp:TextBox>
                        </div>
                        <div class="ekBCright">
                            <input type="hidden" id="hdnVideoId" runat="server" class="hdnVideoId" />
                            <input type="hidden" id="hdnPlaylistId" runat="server" class="hdnPlaylistId" />
                            <input type="hidden" id="hdnPlayerId" runat="server" class="hdnPlayerId" />
                            <asp:LinkButton ID="LinkButton1" runat="server" Text="Save" ValidationGroup="Dimensions" CssClass="bcButton bcSaveVideo" OnClick="btnSaveClk"></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton2" runat="server" Text="Cancel" CssClass="bcButton"
                                OnClick="btnCancelClk"></asp:LinkButton>
                        </div>
                    </div>
                    <script id="videoListtmpl" type="x-jquery-tmpl">
                    <li title="{{= name}}" >
                        <input type="hidden" value="{{= id}}" class="hdnVideoID" />
                        <span class="thumb"><img src="{{if thumbnailURL != null }}{{= thumbnailURL}}{{else}}<%= SitePath %>widgets/brightcovevideo/images/noImageThumb.png{{/if}}" /></span>
                        <span class="bcName" >{{= name}}</span>
                        <span class="bcDate">{{= lastModifiedDateString}}</span>
                    </li>
                    </script>
                    <script id="videoDetailtmpl" type="x-jquery-tmpl">
                    <img alt="preview info thumbnail" src="{{if thumbnailURL != null }}{{= thumbnailURL}}{{else}}<%= SitePath %>widgets/brightcovevideo/images/noImageThumb.png{{/if}}" />
                    <div class="clearfix">
                        <span class="videoAction videoCancelLink" >cancel</span>
                        <span class="videoAction videoEditLink" >edit</span>
                        <span class="videoAction videoSaveLink" >save</span>
                        <span class="videoAction videoDeleteLink" >delete</span>
                    </div>
                    <div class="properties" >
                        <div class="clearfix" ><span class="property" >Name</span><span class="propertyValue editableProperty" title="{{= name}}">{{= name}}</span><input id="nameproperty" name="nameproperty" class="propertyValue hiddenProperty" value="{{= name}}" /></div>
                        <div class="clearfix" ><span class="property" >Status</span><span class="propertyValue">Active</span></div>
                        <div class="clearfix" ><span class="property" >Duration</span><span class="propertyValue" title="{{= duration}}">{{= duration}}</span></div>
                        <div class="clearfix" ><span class="property" >ID</span><span class="propertyValue" title="{{= id}}">{{= id}}</span></div>
                        <div class="clearfix" ><span class="property" >Ref ID</span><span class="propertyValue editableProperty" title="{{= referenceId}}">{{= referenceId}}</span><input id="referenceproperty" name="referenceproperty" class="propertyValue hiddenProperty" value="{{= referenceId}}" /></div>
                        <div class="clearfix" ><span class="property" >Tags</span><span class="propertyValue editableProperty" title="{{= tags}}">{{= tags}}</span><input id="tagsproperty" name="tagsproperty" class="propertyValue hiddenProperty" value="{{= tags}}" /></div>
                    </div>
                    </script>
                    <script id="videoCategoryListtmpl" type="x-jquery-tmpl">
                    <li>{{= name}} <span class="videoCount">({{= total_count}})</span><input type="hidden" value="{{= id}}" class="hdnplaylistID" /></li>
                    </script>
                    <script id="previewtmpl" type="x-jquery-tmpl">
                        <iframe src="{{= previewlink}}"></iframe>
                    </script>
                </div>
                <div class="uploadPane">
                    <div class="editHeader clearfix">
                        <h3>
                            Upload a video to your Brightcove Account</h3>
                        <div class="options">
                            <span class="backLink">
                                <img src="<%= SitePath %>widgets/brightcovevideo/images/icoUpload.png" />
                                Back to quick publish</span>
                        </div>
                    </div>
                    <div class="grayBorder">
                        <div class="uploadWrapper">
                            <div class="clearfix">
                                <span class="ekBCleft">Video Name:</span>
                                <asp:TextBox ID="tbVideoName" runat="server" CssClass="videoName right"></asp:TextBox>
                            </div>
                            <div class="clearfix">
                                <span class="ekBCleft">Video Description:</span>
                                <asp:TextBox ID="tbDescription" runat="server" CssClass="videoDescription right"></asp:TextBox>
                            </div>
                            <div class="clearfix">
                                <span class="ekBCleft">Reference ID:</span>
                                <asp:TextBox ID="tbRefID" runat="server" CssClass="videoRef right" MaxLength="150"></asp:TextBox>
                            </div>
                            <div class="clearfix">
                                <span class="ekBCleft">Tags (comma separated):</span>
                                <asp:TextBox ID="tbTags" runat="server" CssClass="videoTags right"></asp:TextBox>
                            </div>
                            <div class="clearfix">
                                <span class="ekBCleft">File:</span>
                                <%--<asp:FileUpload ID="fuFile" runat="server" CssClass="videoFile right" />--%>
                                    <span class="uploadInput">
                                        <input id="fuFile"   type="file" name="fuFile" autocomplete="off" class="videoFile right" />
                                    </span>
                            </div>
                            <div class="ekBCright clearfix actionRow">
                                <a href="#" class="bcButton uploadVideoBtn">Upload</a>
                                <%--<asp:LinkButton ID="btnUpload" runat="server" Text="Save" CssClass="bcButton uploadVideoBtn" OnClick="btnUploadClk"></asp:LinkButton>--%>
                                <asp:LinkButton ID="LinkButton4" OnClientClick="return false;" runat="server" Text="Cancel" CssClass="bcButton backLink"
                                    ></asp:LinkButton>
                            </div>
                            <div class="error">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="embedPane">
                    <div class="editHeader clearfix">
                        <h3>
                            Embed a Video Player</h3>
                        <div class="options">
                            <span class="backLink">
                                <img src="<%= SitePath %>widgets/brightcovevideo/images/icoUpload.png" />
                                Back to quick publish</span>
                        </div>
                    </div>
                    <div class="grayBorder">
                        <asp:TextBox ID="tbEmbed" runat="server" TextMode="MultiLine" CssClass="embedInput"
                            EnableViewState="false"></asp:TextBox>
                        <div class="ekBCright clearfix actionRow">
                            <asp:LinkButton ID="lbSaveEmbed" runat="server" Text="Save" CssClass="bcButton embedSubmit"
                                OnClick="btnSaveEmbedClk"></asp:LinkButton>
                            <asp:LinkButton ID="lbCancelEmbed" runat="server" Text="Cancel" CssClass="bcButton backLink"
                                OnClientClick="return false;"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="bcLoading">
            </div>
        </asp:View>
    </asp:MultiView>
</div>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="YouTubeVideo.ascx.cs" Inherits="widgets_YouTubeVideo" %>
<div id="<%= ClientID %>" class="ektronWidgetYouTube">
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
        <asp:PlaceHolder ID="phContent" runat="server">
        <div id="uxRenderVideo" runat="server">
            <object width="100%" height="412" >        
                    <param value="<%= MovieSourceURL %>" name="movie" />
                    <param name="pluginspage" value="http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash" />
                <param name="bgcolor" value="#FFFFFF" />
                <param name="allowScriptAccess" value="always" />
                <param name="allowFullScreen" value="true" />
                <param name="swliveconnect" value="true" />
                <param name="wmode" value="transparent" />
                <param name="quality" value="high" />
                    <embed src="<%= MovieSourceURL %>" bgcolor="#FFFFFF" name="flashObj" width="100%" height="412"
                    seamlesstabbing="false" type="application/x-shockwave-flash" allowfullscreen="true"
                    swliveconnect="true" wmode="transparent" pluginspage="http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash"/></object>
                    </div>
                    </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">
            <div id="divHelpText" runat="server" style="font: normal 12px/15px arial; width: 100%; height: 100%;">
                Click on the 'Edit' icon (<img alt="edit icon" title="edit icon" src="<%=appPath %>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png" width="12" height="12" border="0" />) in the top-right corner of this widget
                to select a video you wish to display.
            </div>
        </asp:PlaceHolder>
        </asp:View>          
        <asp:View ID="Edit" runat="server">
            <div class="youtube" id="YouTubeVideo1">
                <ul class="ektronWidgetYTTabs clearfix">
                    <li><a href="#" onclick="Ektron.Widget.YouTubeVideo.SwitchPane(this, 'VideoListTab'); return false;"
                        id="VideoListTab" class="ektronWidgetYTTab selectedTab">Video List</a></li>
                    <li><a href="#" onclick="Ektron.Widget.YouTubeVideo.SwitchPane(this, 'SearchLink'); return false;"
                        id="SearchLink" class="ektronWidgetYTTab">Search</a></li>
                </ul>
                <div class="pane VideoListTab">
                    <div class="YTOptions YTViewOptions">
                        Sort by:
                        <select id="<%= ClientID %>sort_by" onchange="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].FirstVideos();">
                <option value="recently_featured" selected="selected">Recently featured</option>
                            <option value="top_rated">Top rated</option>
                            <option value="top_favorites">Top favorites</option>
                            <option value="most_viewed">Most viewed</option>
                            <option value="most_popular">Most popular</option>
                            <option value="most_recent">Most recent</option>
                        </select>
                        <select style="display:none;" id="<%= ClientID %>sort_order" onchange="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].FirstVideos();">
                            <option value="DESC">Descending</option>
                            <option value="ASC">Ascending</option>
                        </select>
                    </div>
                    <ul class="video-list ektronWidgetYTVideos">
                    </ul>
                    <ul class="ektronWidgetYTButtonWrapper">
                        <li><a id="<%= ClientID %>First" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].FirstVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonFirst" title="First" style="display: none;"><span>First</span></a></li>
                        <li><a id="<%= ClientID %>Previous" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].PreviousVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonPrevious" title="Previous" style="display: none;"><span>Prev</span></a></li>
                        <li><span class="video-result">No Results</span></li>                        
                        <li><a id="<%= ClientID %>Next" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].NextVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonNext" title="Next" style="display: none;"><span>Next</span></a></li>
                        <li><a id="<%= ClientID %>Last" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].LastVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonLast" title="Last" style="display: none;"><span>Last</span></a></li>
                    </ul>
                </div>
                <div class="pane SearchLink" style="display: none;">
                    <div class="search-box YTOptions YTSearchOptions">
                        Search by:
                        <select id="<%= ClientID %>searchtype" onchange="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].SearchFirstVideos();">
                            <option value="TEXT">Text</option>
                            <option value="TAG">Tag</option>
                        </select>
                        <input type="text" id="<%= ClientID %>SearchText" onkeypress="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].KeyPressHandler(this, event, '<%= ClientID %>');" />
                        <a id="<%= ClientID %>Search" title="Search" class="ektronWidgetYTGoButton" onclick="Ektron.Widget.YouTubeVideo.ResetPages();Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].SearchVideos();">
                            Go</a>
                    </div>
                    <ul class="video-search ektronWidgetYTVideos">
                    </ul>
                    <ul class="ektronWidgetYTButtonWrapper ektronWidgetYTSearchButtons">
                        <li><a id="<%= ClientID %>FirstSearch" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].SearchFirstVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonFirst" title="First" style="display: none;"><span>First</span></a></li>
                        <li><a id="<%= ClientID %>PreviousSearch" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].SearchPreviousVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonPrevious" title="Previous" style="display: none;"><span>Prev</span></a></li>
                        <li><span class="video-search-result">No Results</span></li>
                        <li><a id="<%= ClientID %>NextSearch" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].SearchNextVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonNext" title="Next" style="display: none;"><span>Next</span></a></li>
                        <li><a id="<%= ClientID %>LastSearch" onclick="Ektron.Widget.YouTubeVideo.widgets['<%= ClientID %>'].SearchLastVideos();" class="ektronWidgetYTButton ektronWidgetYTButtonLast" title="Last" style="display: none;"><span>Last</span></a></li>
                    </ul>
                </div>
            </div>
            <asp:TextBox ID="tbData" runat="server" Style="display: none;
                width: 95%"> </asp:TextBox>
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click"  Style="display: none;"/> &nbsp;&nbsp;
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
        </asp:View>
    </asp:MultiView>
</div>

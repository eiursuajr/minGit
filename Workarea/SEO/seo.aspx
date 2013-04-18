<%@ Page Language="c#" AutoEventWireup="false" CodeFile="seo.aspx.cs" Inherits="Ektron.SEO.seo" %>
<%@ Register TagPrefix="analytics" TagName="Detail" Src="../Analytics/reporting/Detail.ascx" %>
<%@ Register TagPrefix="ektron" TagName="DateRangePicker" Src="../controls/generic/date/DateRangePicker.ascx" %>
<%@ Register TagPrefix="analytics" TagName="ProviderSelector" Src="../analytics/controls/ProviderSelector.ascx" %> 
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>SEO - W3C</title>
        <style type="text/css">
            body {font-family:"Arial","sans-serif"; font-size: 75%;}
            
            #UrlBox
            {
				margin-left: 1em;
            }
            
            th 
            {
				text-align: left;
            }
            
            .dataTable
            {
                width: 100%;
            }
            
            .iconTd
            {
                width: 5%;
                vertical-align:top;
            }
            .verticalTh
            {
                width:15%;
                font-weight: bold;
                vertical-align: top;
                text-align: right;
                padding-right: 1em;
            }

            iframe
            {
                border: solid 1px #888;
                width: 100%;
                height: 95%;
                margin: .5em 0;
            }

            div#seoModal
            {
                width: 90%;
                height: 90%;
                left: 50%;
                margin-left: -46%;
                margin-top: 0%;
                top: 4%;
                overflow: auto;
            }

            .modalHeader {display: block; text-align: right;}
            a.ektronModalClose, a.ektronModalClose:active, a.ektronModalClose:visited, a.ektronModalClose:focus
            {
                display: inline-block;
                background: #e6e6e6;
                border: solid 1px #d3d3d3;
                color: #555;
                outline-style: none;
                font-weight: bold;
                padding: .5em;
                text-decoration: none;
            }
            a.ektronModalClose:hover
            {
                background: #dadada;
                color: #212121;
                border-color: #999;
            }

            h2
	        {
	            margin-top:12.0pt;
	            margin-right:0in;
	            margin-bottom:3.0pt;
	            margin-left:0in;
	            page-break-after:avoid;
	            font-size:14.0pt;
	            font-family:"Arial","sans-serif";
	            font-style:italic;
            }

	        .pbox
	        {
	            background-color:#E6F7DD;
	            border:#50A029 1px solid;
	            padding: 5px;
		    }
		    .errMsg
		    {
                background-color:#FBE3E4;
                border: 1px solid #FBC2C4;
                color: #D12F19;
                display: block;
                margin: 0.25em;
                padding: 5px;

            }
            /*
            #TrafficDateRange
            {
				margin-bottom: 1em;
				font-weight: bold;
            }
            */
            .DateRangeAndProviderContainer
            {
				display: block;
				margin-bottom: 1em;
            }
			.AnalyticsDetailLineChart
			{
				margin-top: 2px;
				margin-bottom: 1em;
			}
			.UpdateProgressContainer { position: absolute; top: 14em; left: 12em; border: solid 1px #D7E0E7; margin: 1em; padding: 2em; background-color: #E7F0F7; z-index:1; }
        </style>
        <script type="text/javascript">
        <!--//--><![CDATA[//><!--
          Ektron.ready(function()
          {
            $ektron("#tabs").tabs( { cookie: { expires: 3 } } );

            $ektron('#seoModal').modal({
                trigger: '.viewIframeTrigger',
                modal: true,
                toTop: true,
                onShow: function(h)
                {
                    var trigger = $ektron(h.t);
                    var newSrc = trigger.attr("data-ektron-location");
                    var theIframe = $ektron("#fraModal");
                    theIframe.attr("src", newSrc);
                    //$ektron('#seoModal').css("margin-top", -1 * Math.round($ektron('#seoModal').outerHeight()/2));
                    h.w.fadeIn();
                    return false;
                },
                onHide: function(h)
                {
                    var theIframe = $ektron("#fraModal");
                    theIframe.attr("src", "start.htm");
                    h.w.hide();
                    if(h.o)
                    {
                        h.o.remove();
                    }
                    return false;
                }
            });

            <asp:literal id="ltrShowTrafficTab" runat="server" visible="false">
                $ektron('.ui-state-active').removeClass('ui-tabs-selected ui-state-active');
                $ektron('.ui-tabs-panel').addClass('ui-tabs-hide'); 
                $ektron('#liTrafficTab').addClass('ui-tabs-selected ui-state-active');
                $ektron('#tabTraffic').show();
                $ektron('#tabTraffic').removeClass('ui-tabs-hide');           
            </asp:literal>
          });
        //--><!]]>
        </script>
        <script type="text/javascript" language="javascript">
        Ektron.ready(function() {
            // hook ASP.NET Ajax begin and end events, to show busy signal when update panel makes a callback:
            if ("undefined" != typeof Sys.WebForms.PageRequestManager.getInstance) {
                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginCallbackHandler);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndCallbackHandler);
            }
        });
        
        function BeginCallbackHandler() {
            $ektron(".UpdateProgressContainer").show();
        }

        function EndCallbackHandler() {
            $ektron(".UpdateProgressContainer").hide();
        }
        function OpenSegments() {
            var provider = $ektron('#ProviderSelect_ProviderSelectorList').val();
            window.open('../analytics/reporting/segments.aspx?provider=' + provider, null, 'height=300,width=450,status=no,toolbar=no,menubar=no,location=no');
            return false;
        }
        function UpdateSegments()
        {
            setTimeout("__doPostBack('','')", 0);
        }
    </script>
    <script type="text/javascript" language="javascript">
        Ektron.ready(function() {
            $ektron(".tabbedButtons a").each(function (){
                $ektron(this).click(function (){ SEO__SetScrollable(this); });
            });
            
            SEO__SetScrollable($ektron("a[href=#tabTraffic]")[0]);
        });
        
        function SEO__SetScrollable(e){
            if (null == e)
                return;

            var url = $ektron(e).attr("href");
            
            if (url == null || url.length == 0)
                return;
            
            var container = $ektron(url).eq(0);
            SEO__SetScrollable_helper(container);
            
            $ektron(window).resize(function() {SEO__SetScrollable_helper(container);});
        }
        
        function SEO__SetScrollable_helper(container){
            if (null == container)
                return;

            var origStyle = (null != container.attr("style")) ? container.attr("style") : "";
            origStyle = SEO__SetScrollable_removeStyleString(origStyle, "overflow-y: hidden !important;");
            origStyle = SEO__SetScrollable_removeStyleString(origStyle, "overflow-y: auto !important;");
            container.attr("style", origStyle + "overflow-y: hidden !important;");
            container.css("height", "auto");
            
            container.height($ektron(window).height() - 
                (container.offset().top 
                    + (container.innerHeight() 
                        - container.height())));

            origStyle = (null != container.attr("style")) ? container.attr("style") : "";
            origStyle = SEO__SetScrollable_removeStyleString(origStyle, "overflow-y: hidden !important;");
            container.attr("style", origStyle + "overflow-y: auto !important;");
        }
        
        function SEO__SetScrollable_removeStyleString(text, search){
            var modText = text.replace(new RegExp("! important", "gi"), "!important");
            return SEO__SetScrollable_padSemicolon(modText.replace(new RegExp(search, "gi"), ""));
        }
        
        function SEO__SetScrollable_padSemicolon(text){
            return (text + ((text.length && ";" != text.substr(text.length - 1, 1)) ? ";" : ""));
        }
        
    </script>
    </head>
    <body>
        <div class="UpdateProgressContainer" style="display: none;">
		    <h2><asp:Literal ID="litLoadingMessage" runat="server" /></h2>
		</div>
        <form id="form1" runat="server">
        	<asp:ScriptManager runat="server" EnablePartialRendering="true" ID="ScriptManager1" />

             <div class="ektronWindow" id="seoModal">
                <div class="modalHeader">
                    <a title="Close" href="#" class="ektronModalClose"><span id="lblClose" runat="server">Close</span></a>
                </div>
                <iframe id="fraModal" src="start.htm" frameborder="0"></iframe>
            </div>
            
            <div id="tabs" class="flora">
                <p id="UrlBox">
					<asp:Label ToolTip="URL" ID="lblUrl" runat="server" Text="URL" />:
                    <asp:TextBox ToolTip="Enter a URL here" ID="URLTextbox" runat="server" Width="60%"></asp:TextBox>
                    <asp:Button ID="ChangeURLButton" runat="server" Text="Change" />
                </p>
                <p id="errMsg" runat="server" class="errMsg" visible="false" enableviewstate="false">
					<asp:Literal ID="ltr_error" runat="server" EnableViewState="false" />
                </p>

                <ul class="tabbedButtons">
                    <li><a title="SEO tab" href="#tabSEO"><span id="lblSEO" runat="server">SEO</span></a></li>
                    <li><a title="Google tab" href="#tabGoogle"><span id="lblGoogle" runat="server">Google</span></a></li>
                    <li><a title="W3C tab" href="#tabW3C"><span id="lblW3c" runat="server">W3C</span></a></li>
                    <li><a title="Alexa tab" href="#tabAlexa"><span id="lblAlexa" runat="server">Alexa</span></a></li>
                    <li><a title="Images tab" href="#tabImages"><span id="lblImages" runat="server">Images</span></a></li>
                    <li><a title="Text tab" href="#tabText"><span id="lblText" runat="server">Text</span></a></li>
                    <li><a title="Meta tab" href="#tabMeta"><span id="lblMeta" runat="server">Meta</span></a></li>
					<li id="liTrafficTab" runat="server"><a title="Traffic tab" href="#tabTraffic"><span id="lblTraffic" runat="server">Traffic</span></a></li>
                </ul>
                
                <div id="tabSEO" runat="server">
                    <p class="pbox" id="lblDescSEO" runat="server">Metadata provides search engines with information about your web pages, thereby increasing the optimization of your web pages.  If you do not specify metadata, search engines will determine on their own what your page is about, which can greatly reduce your search rankings.  By providing relevant metadata across all of your metadata tags, search engines will reward you and rank your pages better.</p>
                    <table border="0" cellpadding="5" cellspacing="5" class="dataTable">
                    <tbody>
                    <tr>
                        <td class="iconTd">
                            <asp:Image ToolTip="Title Not Valid" ID="titleOkImage" runat="server" ImageUrl="notvalid.png" />
                        </td>
                        <td class="verticalTh" title="Title">
                            <asp:Literal ID="lblTitle" runat="server">Title</asp:Literal>:</td>
                        <td>
                            <asp:Literal ID="titleLiteral" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="iconTd">
                            <asp:Image ToolTip="Description Not Valid" ID="descriptionOkImage" runat="server" ImageUrl="notvalid.png" />
                        </td>
                        <td class="verticalTh" title="Description">
                            <asp:Literal ID="lblDesc" runat="server">Description</asp:Literal>:</td>
                        <td>
                            <asp:Literal ID="descriptionLiteral" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="iconTd">
                            <asp:Image ToolTip="Keywords Not Valid" ID="keywordsOkImage" runat="server" ImageUrl="notvalid.png" />
                        </td>
                        <td class="verticalTh" title="Keywords">
                            <asp:Literal ID="lblKeywords" runat="server">Keywords</asp:Literal>:</td>
                        <td>
                            <asp:Literal ID="keywordLiteral" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="iconTd">
                            <asp:Image ToolTip="Language Not Valid" ID="languageOkImage" runat="server" ImageUrl="notvalid.png" />
                        </td>
                        <td class="verticalTh" title="Language">
                            <asp:Literal ID="LblLang" runat="server">Language</asp:Literal>:</td>
                        <td>
                            <asp:Literal ID="languageLiteral" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="iconTd">
                            <asp:Image ToolTip="Character Set Not Valid" ID="charSetOkImage" runat="server" ImageUrl="notvalid.png" />
                        </td>
                        <td class="verticalTh" title="Character Set">
                            <asp:Literal ID="lblCharSet" runat="server">Character Set</asp:Literal>:
                        </td>
                        <td>
                            <asp:Literal ID="charSetLiteral" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="iconTd">
                            <asp:Image ToolTip="First H1 tag Not Valid" ID="FirstH1OkImage" runat="server" ImageUrl="notvalid.png" />
                        </td>
                        <td class="verticalTh" title="First H1 tag">
                            <asp:Literal ID="lblH1Tag" runat="server">First H1 tag</asp:Literal>:</td>
                        <td>
                            <asp:Literal ID="H1Literal" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="iconTd">&#160;</td>
                        <td class="verticalTh">&#160;</td>
                        <td>&#160;</td>
                    </tr>
                    </tbody>
                    </table>
                </div>

                <div id="tabGoogle">
                    <p class="pbox" id="lblDescGoogle" runat="server">To make sure that your web pages are fully optimized, you can check out what information search engines, like Google, have about your web page. Use this report to find out the details of your site.</p>
                    <table class="dataTable">
						<tbody>
							<tr>
								<td><asp:HyperLink ID="hGoogleLink" runat="server" EnableViewState="false" /></td>
								<td title="Pages that link to this page"><asp:Literal ID="lblLinkThisPage" runat="server">Pages that link to this page</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hGoogleSite" runat="server" EnableViewState="false" /></td>
								<td title="Indexed pages in your site"><asp:Literal ID="lblIndexedPages" runat="server">Indexed pages in your site</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hGoogleCache" runat="server" EnableViewState="false" /></td>
								<td title="The current cached version of this page"><asp:Literal ID="lblCachedVersion" runat="server">The current cached version of this page</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hGoogleInfo" runat="server" EnableViewState="false" /></td>
								<td title="Information Google has about this page"><asp:Literal ID="lblAboutThisPage" runat="server" Text="Information Google has about this page" /></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hGoogleRelated" runat="server" EnableViewState="false" /></td>
								<td title="Pages that are similar to this page"><asp:Literal ID="lblSimilarToPage" runat="server">Pages that are similar to this page</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hGoogleRobots" runat="server" EnableViewState="false" /></td>
								<td title="robots.txt">robots.txt&#160;&#160;</td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hGoogleMobileImages" runat="server" EnableViewState="false" /></td>
								<td title="What page looks like in mobile device"><asp:Literal ID="lblMobileImgLooks" runat="server">What page looks like in mobile device</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hGoogleMobileNoImages" runat="server" EnableViewState="false" /></td>
								<td title="What page looks like in mobile device"><asp:Literal ID="lblMobileNoImgLooks" runat="server">What page looks like in mobile device</asp:Literal></td>
							</tr>
                        </tbody>
                    </table>
                </div>

                <div id="tabW3C">
                    <p class="pbox" id="lblDescW3c" runat="server">A web page that has been validated by the W3C can increase your search engine rankings.  Use these reports to validate your page markup across all browsers(including mobile devices), the CSS and check for broken links.</p>
                    <table class="dataTable">
						<tbody>
							<tr>
								<td><asp:HyperLink ID="hW3CValidation" runat="server" EnableViewState="false" /></td>
								<td title="Checks the markup validity of Web documents in HTML"><asp:Literal ID="lblCheckMarkup" runat="server">Checks the markup validity of Web documents in HTML</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hW3CLinkCheck" runat="server" EnableViewState="false" /></td>
								<td title="Checks for broken links"><asp:Literal ID="lblCheckLinks" runat="server">Checks for broken links</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hW3CCSS" runat="server" EnableViewState="false" /></td>
								<td title="Checks Cascading Style Sheets (CSS)"><asp:Literal ID="lblCheckCss" runat="server">Checks Cascading Style Sheets (CSS)</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hW3CMobile" runat="server" EnableViewState="false" /></td>
								<td title="Checks for validity for mobile phones"><asp:Label ID="lblCheckMobilePhones" runat="server">Checks for validity for mobile phones</asp:Label></td>
							</tr>
						</tbody>
					</table>
                </div>

                <div id="tabAlexa">
                    <p class="pbox" id="lblDescAlexa" runat="server">Alexa is a leader in providing insight on the overall ranking of your web site on the Internet.  You can use the reports to identify your current ranking on the internet, comparing your site to others as well as discovering sites related to yours.</p>
                    <table class="dataTable">
						<tbody>
							<tr>
								<td><asp:HyperLink ID="hAlexaSiteInfo" runat="server" EnableViewState="false" /></td>
								<td title="Information about your site from Alexa"><asp:Literal ID="lblOverviewAlexa" runat="server">Information about your site from Alexa</asp:Literal></td>
							</tr>
							<tr>
 								<td><asp:HyperLink ID="hAlexaTrafficStats" runat="server" EnableViewState="false" /></td>
								<td title="Traffic details"><asp:Literal ID="lblTrafficDetails" runat="server">Traffic details</asp:Literal></td>
							</tr>
							<tr>
 								<td><asp:HyperLink ID="hAlexaRelatedLinks" runat="server" EnableViewState="false" /></td>
								<td title="Related Sites"><asp:Literal ID="lblRelatedSites" runat="server">Related Sites</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hAlexaKeywords" runat="server" EnableViewState="false" /></td>
								<td title="Link to Sites"><asp:Literal ID="lblAlexaKeywords" runat="server">Link to Sites</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hAlexaClickstream" runat="server" EnableViewState="false" /></td>
								<td title="Link to Sites"><asp:Literal ID="lblAlexaClickstream" runat="server">Link to Sites</asp:Literal></td>
							</tr>
							<tr>
								<td><asp:HyperLink ID="hAlexaLinksIn" runat="server" EnableViewState="false" /></td>
								<td title="Link to Sites"><asp:Literal ID="lblLinkToSites" runat="server">Link to Sites</asp:Literal></td>
							</tr>
                        </tbody>
                    </table>

                </div>

                <div id="tabImages">
                    <p class="pbox" id="lblDescImg" runat="server">To ensure search engine optimization, and 508 compliance, all of the images that are used on your web page should contain alt tags. Since search engines cannot determine the contents of an image, alt tags describe the image to help with your web page relevancy.  In addition, alt tags must be applied to all images in order for your web page to be 508 compliant.</p>
                    <table class="dataTable">
                        <tr>
                            <td title="Status"><strong><asp:Literal ID="lblStatus" runat="server">Status</asp:Literal></strong></td>
                            <td title="Alt tag"><strong><asp:Literal ID="lblAltTag" runat="server">Alt tag</asp:Literal></strong></td>
                            <td title="Image"><strong><asp:Literal ID="lblImg" runat="server">Image</asp:Literal></strong></td>
                        </tr>
                        <asp:Literal ID="ImageLiteral" runat="server"></asp:Literal>
                    </table>
                </div>

                <div id="tabText" class="InnerRegion">
                    <p class="pbox" id="lblDescText" runat="server">This report displays your web page in plain text, which is very similar to the way that search engines see your page.  Web pages that are content rich will yield higher search engine rankings than pages that are mainly images or multimedia based.  Use this report to identify the text of the page and the placement of keywords within it.</p>
                     <p><asp:Literal ID="BodyTextLiteral" runat="server"></asp:Literal></p>
                </div>

                <div id="tabMeta">
                    <p class="pbox" id="lblDescMeta" runat="server">This report displays the metadata for this web page.</p>
                    <p><asp:Literal ID="MetaDataLiteral" runat="server"></asp:Literal></p>
                </div>
                			
				<div id="tabTraffic" runat="server">
					<div class="DateRangeAndProviderContainer">
					<analytics:ProviderSelector ID="ProviderSelect" runat="server" AutoPostBack="true" /><ektron:DateRangePicker id="DateRangePicker1" runat="server" />
					<asp:ImageButton ID="SegmentPopupBtn" runat="server" OnClientClick="return OpenSegments();" ImageUrl="../images/UI/Icons/bricks.png" BorderStyle="None" AlternateText="Provider Segments Filter" ToolTip="Provider Segments Filter" />
                    </div>
					<asp:UpdatePanel ID="pnlAnalyticsReport" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
						<Triggers>
							<asp:AsyncPostBackTrigger ControlID="DateRangePicker1" EventName="SelectionChanged" />
							<asp:AsyncPostBackTrigger ControlID="ProviderSelect" EventName="OnProviderChanged" />
						</Triggers>
						<ContentTemplate>
						    <p id="errAnalyticsMsg" runat="server" class="errMsg" visible="false" enableviewstate="false">
					            <asp:Literal ID="ltr_analyticsError" runat="server" EnableViewState="false" />
                            </p>
							<analytics:Detail ID="AnalyticsReport" runat="server" />
						</ContentTemplate>
					</asp:UpdatePanel>
					
               </div>
            </div>
        </form>
    </body>
</html>
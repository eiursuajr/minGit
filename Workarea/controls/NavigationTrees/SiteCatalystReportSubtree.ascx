<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SiteCatalystReportSubtree.ascx.cs" Inherits="SiteCatalystReportSubtree" %>
<ul id="SiteAnalytics" visible="false" runat="server">
	<li runat="server" id="Li1">sam site metrics
		<ul runat="server" id="Ul1">
			<li runat="server" id="Li2" title="sam sitecatalyst trend page views" href="Analytics/reporting/Trends.aspx?report=pageviews">sam sitecatalyst trend page views</li>
			<li runat="server" id="Li3" title="sam sitecatalyst trend visits" href="Analytics/reporting/Trends.aspx?report=visits">sam sitecatalyst trend visits</li>
			<li runat="server" id="Li4" title="sam visitors">sam visitors
				<ul runat="server" id="Ul2">
					<%--<li runat="server" id="Li5" href="Report.HourlyUnique">Hourly Unique Visitors</li>--%>
					<li runat="server" id="Li6" title="sam sitecatalyst trend daily unique visitors" href="Analytics/reporting/Trends.aspx?report=dailyVisitors">sam sitecatalyst trend daily unique visitors</li>
					<%--<li runat="server" id="Li7" href="Report.WeeklyUnique">Weekly Unique Visitors</li>
					<li runat="server" id="Li8" href="Report.MonthlyUnique">Monthly Unique Visitors</li>
					<li runat="server" id="Li9" href="Report.QuarterlyUnique">Quarterly Unique Visitors</li>
					<li runat="server" id="Li10" href="Report.YearlyUnique">Yearly Unique Visitors</li>--%>
				</ul>
			</li>
			<li runat="server" id="Li11" title="sam time spent per visit" href="Analytics/reporting/Reports.aspx?report=TimeVisitOnSite">sam time spent per visit</li>
			<%--<li runat="server" id="Li12">Purchases
				<ul runat="server" id="Ul3">
					<li runat="server" id="Li13" href="Report.PurchaseSummary&rp=e|1;ec_pri|0">Purchase Conversion Funnel</li>
					<li runat="server" id="Li14" href="1;ec_reset|1">Revenue</li>
					<li runat="server" id="Li15" href="2;ec_reset|1">Orders</li>
					<li runat="server" id="Li16" href="3;ec_reset|1">Units</li>
				</ul>
			</li>
			<li runat="server" id="Li17">Shopping Cart
				<ul runat="server" id="Ul4">
					<li runat="server" id="Li18" href="Report.CartSummary&rp=e|1;ec_pri|0">Cart Conversion Funnel</li>
					<li runat="server" id="Li19" href="4;ec_reset|1">Carts</li>
					<li runat="server" id="Li20" href="5;ec_reset|1">Cart Views</li>
					<li runat="server" id="Li21" href="8;ec_reset|1">Cart Additions</li>
					<li runat="server" id="Li22" href="9;ec_reset|1">Cart Removals</li>
					<li runat="server" id="Li23" href="7;ec_reset|1">Checkouts</li>
				</ul>
			</li>--%>
			<%--<li runat="server" id="Li24">Custom Events
				<ul runat="server" id="Ul5">
					<li runat="server" id="Li25" href="Report.CustomSummary&rp=e|1;ec_pri|0">Custom Events Funnel</li>
					<li runat="server" id="Li26">Custom Events 1-20
						<ul runat="server" id="Ul6">
							<li runat="server" id="Li27" href="101;ec_reset|1">Searches</li>
							<li runat="server" id="Li28" href="102;ec_reset|1">Null Searches</li>
							<li runat="server" id="Li29" href="103;ec_reset|1">Product Views Event</li>
							<li runat="server" id="Li30" href="104;ec_reset|1">Units Added</li>
							<li runat="server" id="Li31" href="105;ec_reset|1">Units Removed</li>
							<li runat="server" id="Li32" href="106;ec_reset|1">Revenue Added</li>
							<li runat="server" id="Li33" href="107;ec_reset|1">Revenue Removed</li>
							<li runat="server" id="Li34" href="108;ec_reset|1">Cost of Goods</li>
							<li runat="server" id="Li35" href="109;ec_reset|1">Page Views Event</li>
							<li runat="server" id="Li36" href="110;ec_reset|1">Email Signups</li>
							<li runat="server" id="Li37" href="111;ec_reset|1">Newsletter Signups</li>
							<li runat="server" id="Li38" href="112;ec_reset|1">Registrations</li>
							<li runat="server" id="Li39" href="113;ec_reset|1">Shipping Information</li>
							<li runat="server" id="Li40" href="114;ec_reset|1">Billing Information</li>
							<li runat="server" id="Li41" href="115;ec_reset|1">Order Confirmation</li>
							<li runat="server" id="Li42" href="116;ec_reset|1">Product Review Read</li>
							<li runat="server" id="Li43" href="117;ec_reset|1">Product Review Write</li>
							<li runat="server" id="Li44" href="118;ec_reset|1">Wish list</li>
							<li runat="server" id="Li45" href="119;ec_reset|1">Login</li>
							<li runat="server" id="Li46" href="120;ec_reset|1">Member Checkout</li>
						</ul>
					</li>
					<li runat="server" id="Li66">Custom Events 21-40
						<ul runat="server" id="Ul7">
							<li runat="server" id="Li47" href="121;ec_reset|1">Forward to Friend</li>
							<li runat="server" id="Li48" href="122;ec_reset|1">Product Zoom</li>
							<li runat="server" id="Li49" href="123;ec_reset|1">Store Locator</li>
							<li runat="server" id="Li50" href="124;ec_reset|1">Return Units</li>
							<li runat="server" id="Li51" href="125;ec_reset|1">Return Revenue</li>
							<li runat="server" id="Li52" href="126;ec_reset|1">Offline Orders</li>
							<li runat="server" id="Li53" href="127;ec_reset|1">Offline Revenue</li>
							<li runat="server" id="Li54" href="129;ec_reset|1">Total Click Thrus</li>
							<li runat="server" id="Li55" href="130;ec_reset|1">Click Past</li>
							<li runat="server" id="Li56" href="131;ec_reset|1">CVP Orders</li>
							<li runat="server" id="Li57" href="132;ec_reset|1">CVP Revenue</li>
							<li runat="server" id="Li58" href="133;ec_reset|1">Add to Registry</li>
							<li runat="server" id="Li59" href="134;ec_reset|1">Virtual Catalog</li>
							<li runat="server" id="Li60" href="135;ec_reset|1">Emails Sent</li>
							<li runat="server" id="Li61" href="136;ec_reset|1">Emails Opened</li>
							<li runat="server" id="Li62" href="137;ec_reset|1">Emails Delivered</li>
							<li runat="server" id="Li63" href="138;ec_reset|1">Emails Unsubscribed</li>
							<li runat="server" id="Li64" href="139;ec_reset|1">Emails Bounced</li>
							<li runat="server" id="Li65" href="140;ec_reset|1">Shipping Cost</li>
						</ul>
					</li>
					<li runat="server" id="Li67">Custom Events 41-60
						<ul runat="server" id="Ul8">
							<li runat="server" id="Li68" href="141;ec_reset|1">Form Abandonment</li>
							<li runat="server" id="Li69" href="142;ec_reset|1">Form Success</li>
							<li runat="server" id="Li70" href="143;ec_reset|1">Form Error</li>
							<li runat="server" id="Li71" href="144;ec_reset|1">DFA View Thrus</li>
							<li runat="server" id="Li72" href="145;ec_reset|1">DFA Impressions</li>
							<li runat="server" id="Li73" href="146;ec_reset|1">DFA Clicks</li>
							<li runat="server" id="Li74" href="147;ec_reset|1">Out of Stock</li>
							<li runat="server" id="Li75" href="148;ec_reset|1">Twitter Comments</li>
							<li runat="server" id="Li76" href="149;ec_reset|1">Campaign Cost</li>
							<li runat="server" id="Li77" href="150;ec_reset|1">CVP Assist</li>
							<li runat="server" id="Li78" href="201;ec_reset|1">KW CVP Orders</li>
							<li runat="server" id="Li79" href="202;ec_reset|1">KW CVP Revenue</li>
							<li runat="server" id="Li80" href="203;ec_reset|1">KW CVP Assist</li>
						</ul>
					</li>
					<li runat="server" id="Li81">Custom Events 61-80
						<ul runat="server" id="Ul9">
							<li runat="server" id="Li82" href="227;ec_reset|1">Impressions</li>
							<li runat="server" id="Li83" href="228;ec_reset|1">Pos Variable</li>
							<li runat="server" id="Li84" href="229;ec_reset|1">Keyword Cost</li>
							<li runat="server" id="Li85" href="230;ec_reset|1">Clicks</li>
						</ul>
					</li>
				</ul>
			</li>--%>
		</ul>
	</li>
	<li runat="server" id="Li86">sam site content
		<ul runat="server" id="Ul10">
			<li runat="server" id="Li87" title="sam pages" href="Analytics/reporting/Reports.aspx?report=pages">sam pages</li>
			<li runat="server" id="Li88" title="sam site sections" href="Analytics/reporting/Reports.aspx?report=siteSection">sam site sections</li>
			<li runat="server" id="Li89" title="sam servers" href="Analytics/reporting/Reports.aspx?report=server">sam servers</li>
			<li runat="server" id="Li90" title="sam links">sam links
				<ul runat="server" id="Ul11">
					<li runat="server" id="Li91" title="sam exit links" href="Analytics/reporting/Reports.aspx?report=linkExit">sam exit links</li>
					<li runat="server" id="Li92" title="sam file downloads" href="Analytics/reporting/Reports.aspx?report=linkDownload">sam file downloads</li>
					<li runat="server" id="Li93" title="sam custom links" href="Analytics/reporting/Reports.aspx?report=linkCustom">sam custom links</li>
					<%--<li runat="server" id="Li94" href="Report.ClickMap&rp=ec_reset|1">ClickMap</li>--%>
				</ul>
			</li>
			<li runat="server" id="Li95" title="sam pages not found" href="Analytics/reporting/Reports.aspx?report=pagesNotFound">sam pages not found</li>
		</ul>
	</li>
	<%--<li runat="server" id="Li96">sam video
		<ul runat="server" id="Ul12">
			<li runat="server" id="Li97" href="Analytics/reporting/Reports.aspx?report=videosViews">sam video views</li>
			<li runat="server" id="Li98" href="Analytics/reporting/Reports.aspx?report=videosVisits">sam video visits</li>
			<li runat="server" id="Li99" href="Report.DailyUniqueMediaVisitors">Daily Unique Video Visitors</li>
			<li runat="server" id="Li100">Videos Reports
				<ul runat="server" id="Ul13">
					<li runat="server" id="Li101" href="rp=division[0]|75:1;">Video Classification without representation</li>
					<li runat="server" id="Li102" href="rp=division[0]|75:2;">htnshtn</li>
					<li runat="server" id="Li103" href="Report.MediaConsumption">Videos</li>
				</ul>
			</li>
			<li runat="server" id="Li104" href="Report.NextMediaFlow">Next Video Flow</li>
			<li runat="server" id="Li105" href="Report.PreviousMediaFlow">Previous Video Flow</li>
			<li runat="server" id="Li106">Video Segments Viewed Reports
				<ul runat="server" id="Ul14">
					<li runat="server" id="Li107" href="rp=select_division[0]|75:1;media_reset|1">Video Classification without representation</li>
					<li runat="server" id="Li108" href="rp=select_division[0]|75:2;media_reset|1">htnshtn</li>
					<li runat="server" id="Li109" href="rp=media_reset|1">Video Segments Viewed</li>
				</ul>
			</li>
			<li runat="server" id="Li110">Time Spent on Video Reports
				<ul runat="server" id="Ul15">
					<li runat="server" id="Li111" href="select_division[0]|75:1;media_reset|1">Video Classification without representation</li>
					<li runat="server" id="Li112" href="select_division[0]|75:2;media_reset|1">htnshtn</li>
					<li runat="server" id="Li113" href="media_reset|1">Time Spent on Video</li>
				</ul>
			</li>
			<li runat="server" id="Li114" href="Analytics/reporting/Reports.aspx?report=videoPlayers">sam video players</li>
			<li runat="server" id="Li115" href="Report.MediaPlayersMedia">Videos by Player</li>
			<li runat="server" id="Li116" href="Report.MediaPlayersMediaSegments">Video Details by Player</li>
		</ul>
	</li>--%>
	<li runat="server" id="Li117">sam mobile
		<ul runat="server" id="Ul16">
			<li runat="server" id="Li118" href="Analytics/reporting/Reports.aspx?report=mobileDeviceName" title="sam devices">sam devices</li>
			<li runat="server" id="Li119" href="Analytics/reporting/Reports.aspx?report=mobileManufacturer" title="sam manufacturer">sam manufacturer</li>
			<li runat="server" id="Li120" href="Analytics/reporting/Reports.aspx?report=mobileScreenSize" title="sam screen size">sam screen size</li>
			<li runat="server" id="Li121" href="Analytics/reporting/Reports.aspx?report=mobileScreenHeight" title="sam screen height">sam screen height</li>
			<li runat="server" id="Li122" href="Analytics/reporting/Reports.aspx?report=mobileScreenWidth" title="sam screen width">sam screen width</li>
			<li runat="server" id="Li123" href="Analytics/reporting/Reports.aspx?report=mobileCookieSupport" title="sam cookie support">sam cookie support</li>
			<li runat="server" id="Li124" href="Analytics/reporting/Reports.aspx?report=mobileImageSupport" title="sam image support">sam image support</li>
			<li runat="server" id="Li125" href="Analytics/reporting/Reports.aspx?report=mobileColorDepth" title="sam color depth">sam color depth</li>
			<li runat="server" id="Li126" href="Analytics/reporting/Reports.aspx?report=mobileAudioSupport" title="sam audio support">sam audio support</li>
			<li runat="server" id="Li127" href="Analytics/reporting/Reports.aspx?report=mobileVideoSupport" title="sam video support">sam video support</li>
			<li runat="server" id="Li128" href="Analytics/reporting/Reports.aspx?report=mobileDRM" title="sam drm">sam drm</li>
			<li runat="server" id="Li129" href="Analytics/reporting/Reports.aspx?report=mobileNetProtocols" title="sam net protocols">sam net protocols</li>
			<li runat="server" id="Li130" href="Analytics/reporting/Reports.aspx?report=mobileOS" title="sam operating system">sam operating system</li>
			<li runat="server" id="Li131" href="Analytics/reporting/Reports.aspx?report=mobileJavaVM" title="sam java version">sam java version</li>
			<li runat="server" id="Li132" href="Analytics/reporting/Reports.aspx?report=mobileMaxBookmarkUrlLength" title="sam bookmark url length">sam bookmark url length</li>
			<li runat="server" id="Li133" href="Analytics/reporting/Reports.aspx?report=mobileMaxMailUrlLength" title="sam mail url length">sam mail url length</li>
			<li runat="server" id="Li134" href="Analytics/reporting/Reports.aspx?report=mobileMaxBroswerUrlLength" title="sam browser url length">sam browser url length</li>
			<li runat="server" id="Li135" href="Analytics/reporting/Reports.aspx?report=mobileDeviceNumberTransmit" title="sam device number transmit (on/off)">sam device number transmit (on/off)</li>
			<li runat="server" id="Li136" href="Analytics/reporting/Reports.aspx?report=mobilePushToTalk" title="sam ptt">sam ptt</li>
			<li runat="server" id="Li137" href="Analytics/reporting/Reports.aspx?report=mobileMailDecoration" title="sam decoration mail support">sam decoration mail support</li>
			<li runat="server" id="Li138" href="Analytics/reporting/Reports.aspx?report=mobileInformationServices" title="sam information services">sam information services</li>
		</ul>
	</li>
	<li runat="server" id="Li139" title="sam paths">sam paths
		<ul runat="server" id="Ul17">
			<%--<li runat="server" id="Li140" href="Report.NextPageFlow">Next Page Flow</li>
			<li runat="server" id="Li141" href="Report.NextPage">Next Page</li>
			<li runat="server" id="Li142" href="Report.PreviousPageFlow">Previous Page Flow</li>
			<li runat="server" id="Li143" href="Report.PreviousPage">Previous Page</li>
			<li runat="server" id="Li144" href="Report.FalloutLanding">Fallout</li>
			<li runat="server" id="Li145" href="Report.FullPaths">Full Paths</li>
			<li runat="server" id="Li146" href="Report.PathAnalysisLanding">PathFinder</li>
			<li runat="server" id="Li147" href="Report.VisitDepth">Path Length</li>--%>
			<li runat="server" id="Li148" title="sam page analysis">sam page analysis
				<ul runat="server" id="Ul18">
					<%--<li runat="server" id="Li149" href="Report.PageSummary">Page Summary</li>--%>
					<li runat="server" id="Li150" href="Analytics/reporting/Reports.aspx?report=reloads" title="sam reloads">sam reloads</li>
					<li runat="server" id="Li151" href="Analytics/reporting/Reports.aspx?report=averagePageDepth" title="sam page depth">sam page depth</li>
					<li runat="server" id="Li152" href="Analytics/reporting/Reports.aspx?report=timeVisitOnPage" title="sam time spent on page">sam time spent on page</li>
					<%--<li runat="server" id="Li153" href="Report.ClicksToPage">Clicks to Page</li>--%>
				</ul>
			</li>
			<li runat="server" id="Li154">sam entries exits
				<ul runat="server" id="Ul19">
					<li runat="server" id="Li155" href="Analytics/reporting/Reports.aspx?report=entryPage" title="sam entry pages">sam entry pages</li>
					<li runat="server" id="Li156" href="Analytics/reporting/Reports.aspx?report=entryPageOriginal" title="sam original entry pages">sam original entry pages</li>
					<%--<li runat="server" id="Li157" href="Analytics/reporting/Reports.aspx?report=pathLenth">Single Page Visits</li>--%>
					<%--<li runat="server" id="Li158" href="Report.ExitPage">Exit Pages</li>--%>
				</ul>
			</li>
			<%--<li runat="server" id="Li159" href="Report.LongestPath">Longest Paths</li>--%>
		</ul>
	</li>
	<li runat="server" id="Li160">sam traffic sources
		<ul runat="server" id="Ul20">
			<li runat="server" id="Li161" href="Analytics/reporting/Reports.aspx?report=searchEngineKeyword" title="sam search keywords - all">sam search keywords - all</li>
			<li runat="server" id="Li162" href="Analytics/reporting/Reports.aspx?report=searchEnginePaidKeyword" title="sam search keywords - paid">sam search keywords - paid</li>
			<li runat="server" id="Li163" href="Analytics/reporting/Reports.aspx?report=searchEngineNaturalKeyword" title="sam search keywords - natural">sam search keywords - natural</li>
			<li runat="server" id="Li164" href="Analytics/reporting/Reports.aspx?report=sitecatalystSearchEngine" title="sam search engines - all">sam search engines - all</li>
			<li runat="server" id="Li165" href="Analytics/reporting/Reports.aspx?report=searchEnginePaid" title="sam search engines - paid">sam search engines - paid</li>
			<li runat="server" id="Li166" href="Analytics/reporting/Reports.aspx?report=searchEngineNatural" title="sam search engines - natural">sam search engines - natural</li>
			<li runat="server" id="Li167" href="Analytics/reporting/Reports.aspx?report=searchEngineNaturalPageRank" title="sam all search page ranking">sam all search page ranking</li>
			<li runat="server" id="Li168" href="Analytics/reporting/Reports.aspx?report=referringDomain" title="sam referring domains">sam referring domains</li>
			<li runat="server" id="Li169" href="Analytics/reporting/Reports.aspx?report=referringDomainOriginal" title="sam original referring domains">sam original referring domains</li>
			<%--<li runat="server" id="Li170" href="Analytics/reporting/Reports.aspx?report=referrer">sam referrers</li>
			<li runat="server" id="Li171" href="Analytics/reporting/Reports.aspx?report=referrerType">sam referrer type</li>--%>
		</ul>
	</li>
	<%--<li runat="server" id="Li172">Campaigns
		<ul runat="server" id="Ul21">
			<li runat="server" id="Li173" href="Report.CampaignSummary&rp=e|1;ec_pri|20">Campaign Conversion Funnel</li>
			<li runat="server" id="Li174">Tracking Code
				<ul runat="server" id="Ul22">
					<li runat="server" id="Li175" href="1;ec_reset|1">Creative Elements</li>
					<li runat="server" id="Li176" href="2;ec_reset|1">Campaigns</li>
					<li runat="server" id="Li177" href="3;ec_reset|1">Site Name</li>
					<li runat="server" id="Li178" href="4;ec_reset|1">Ad Name</li>
					<li runat="server" id="Li179" href="5;ec_reset|1">Page Name</li>
					<li runat="server" id="Li180" href="8;ec_reset|1">Message Name</li>
					<li runat="server" id="Li181" href="21;ec_reset|1">Name</li>
					<li runat="server" id="Li182" href="Report.GetConversions&rp=ec_pri|20;ec_reset|1">Tracking Code</li>
				</ul>
			</li>
		</ul>
	</li>--%><%--up to here--%>
	<li runat="server" id="Li183" title="sam products">sam products
		<ul runat="server" id="Ul23">
			<%--<li runat="server" id="Li184" href="Report.ProductSummary&rp=e|1;ec_pri|18">Products Conversion Funnel</li>--%>
			<li runat="server" id="Li185" href="Analytics/reporting/Reports.aspx?report=products" title="sam products">sam products</li>
			<%--<li runat="server" id="Li186" href="Report.GetConversions&rp=ec_pri|18;ec_sub1|18;ec_reset|1;view|1">Cross Sell</li>
			<li runat="server" id="Li187" href="Report.GetConversions&rp=ec_pri|19;ec_reset|1">Categories</li>--%>
		</ul>
	</li>
	<li runat="server" id="Li188">sam visitor retention
		<ul runat="server" id="Ul24">
			<li runat="server" id="Li189" href="Analytics/reporting/Reports.aspx?report=returnFrequency" title="sam return frequency">sam return frequency</li>
			<%--<li runat="server" id="Li190" href="Report.ReturnVisits">Return Visits</li>
			<li runat="server" id="Li191" href="Report.DailyReturnVisits">Daily Return Visits</li>--%>
			<li runat="server" id="Li192" href="Analytics/reporting/Reports.aspx?report=visitNumber" title="sam visit number">sam visit number</li>
			<%--<li runat="server" id="Li193">Sales Cycle
				<ul runat="server" id="Ul25">
					<li runat="server" id="Li194" href="Report.GetConversions&rp=ec_pri|29;ec_reset|1">Customer Loyalty</li>
					<li runat="server" id="Li195" href="Report.GetConversions&rp=ec_pri|30;ec_reset|1">Days Before First Purchase</li>
					<li runat="server" id="Li196" href="Report.GetConversions&rp=ec_pri|31;ec_reset|1">Days Since Last Purchase</li>
					<li runat="server" id="Li197" href="Report.DailyUniqueCustomers&rp=ec_pri|32">Daily Unique Customers</li>
					<li runat="server" id="Li198" href="Report.WeeklyUniqueCustomers">Weekly Unique Customers</li>
					<li runat="server" id="Li199" href="Report.MonthlyUniqueCustomers">Monthly Unique Customers</li>
					<li runat="server" id="Li200" href="Report.QuarterlyUniqueCustomers">Quarterly Unique Customers</li>
					<li runat="server" id="Li201" href="Report.YearlyUniqueCustomers">Yearly Unique Customers</li>
				</ul>
			</li>--%>
		</ul>
	</li>
	<li runat="server" id="Li202" title="sam visitor profile">sam visitor profile
		<ul runat="server" id="Ul26">
			<li runat="server" id="Li203" title="sam geosegmentation">sam geosegmentation
				<ul runat="server" id="Ul27">
					<li runat="server" id="Li204" href="Analytics/reporting/Reports.aspx?report=geoCountries" title="sam countries">sam countries</li>
					<li runat="server" id="Li205" href="Analytics/reporting/Reports.aspx?report=geoRegions" title="sam regions">sam regions</li>
					<li runat="server" id="Li206" href="Analytics/reporting/Reports.aspx?report=geoCities" title="sam cities">sam cities</li>
					<%--<li runat="server" id="Li207" href="Report.GeoStates&rp=view|7;geo_country|1173709922">U.S. States</li>--%>
					<li runat="server" id="Li208" href="Analytics/reporting/Reports.aspx?report=geoDMA" title="sam u.s. dma">sam u.s. dma</li>
				</ul>
			</li>
			<li runat="server" id="Li209" href="Analytics/reporting/Reports.aspx?report=homePage" title="sam visitor home page">sam visitor home page</li>
			<li runat="server" id="Li210" href="Analytics/reporting/Reports.aspx?report=sitecatalystLanguage" title="sam languages">sam languages</li>
			<li runat="server" id="Li211" href="Analytics/reporting/Reports.aspx?report=timeZone" title="sam time zones">sam time zones</li>
			<li runat="server" id="Li212" href="Analytics/reporting/Reports.aspx?report=domain" title="sam domains">sam domains</li>
			<li runat="server" id="Li213" href="Analytics/reporting/Reports.aspx?report=topLevelDomain" title="sam top level domains">sam top level domains</li>
			<li runat="server" id="Li214" title="sam technology">sam technology
				<ul runat="server" id="Ul28">
					<li runat="server" id="Li215" href="Analytics/reporting/Reports.aspx?report=sitecatalystBrowsers" title="sam browsers">sam browsers</li>
					<li runat="server" id="Li216" href="Analytics/reporting/Reports.aspx?report=browserType" title="sam browser types">sam browser types</li>
					<li runat="server" id="Li217" href="Analytics/reporting/Reports.aspx?report=browserWidth" title="sam browser width">sam browser width</li>
					<li runat="server" id="Li218" href="Analytics/reporting/Reports.aspx?report=browserHeight" title="sam browser height">sam browser height</li>
					<li runat="server" id="Li219" href="Analytics/reporting/Reports.aspx?report=sitecatalystOperatingSystem" title="sam operating systems">sam operating systems</li>
					<li runat="server" id="Li220" href="Analytics/reporting/Reports.aspx?report=monitorColorDepth" title="sam monitor color depths">sam monitor color depths</li>
					<li runat="server" id="Li221" href="Analytics/reporting/Reports.aspx?report=monitorResolution" title="sam monitor resolutions">sam monitor resolutions</li>
					<%--<li runat="server" id="Li222" href="Report.Plugins">Netscape Plug-Ins</li>--%>
					<li runat="server" id="Li223" href="Analytics/reporting/Reports.aspx?report=javaEnabled" title="sam java">sam java</li>
					<li runat="server" id="Li224" href="Analytics/reporting/Reports.aspx?report=javaScriptEnabled" title="sam javascript">sam javascript</li>
					<li runat="server" id="Li225" href="Analytics/reporting/Reports.aspx?report=javaScriptVersion" title="sam javascript version">sam javascript version</li>
					<li runat="server" id="Li226" href="Analytics/reporting/Reports.aspx?report=cookiesEnabled" title="sam cookies">sam cookies</li>
					<li runat="server" id="Li227" href="Analytics/reporting/Reports.aspx?report=connectionTypes" title="sam connection types">sam connection types</li>
				</ul>
			</li>
			<%--<li runat="server" id="Li228">Visitor Details
				<ul runat="server" id="Ul29">
					<li runat="server" id="Li229" href="Report.KeyVisitors">Key Visitors</li>
					<li runat="server" id="Li230" href="Report.KeyVisitorPages">Pages Viewed by Key Visitors</li>
					<li runat="server" id="Li231" href="Report.RecentVisitors">Last 100 Visitors</li>
					<li runat="server" id="Li232" href="Report.VisitorDetail">Visitor Snapshot</li>
				</ul>
			</li>--%>
			<li runat="server" id="Li233" href="Analytics/reporting/Reports.aspx?report=visitorUSState" title="sam visitor state">sam visitor state</li>
			<li runat="server" id="Li234" href="Analytics/reporting/Reports.aspx?report=visitorZipCode" title="sam visitor zip/postal code">sam visitor zip/postal code</li>
		</ul>
	</li>
</ul>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleAnalyticsReportSubtree.ascx.cs" Inherits="GoogleAnalyticsReportSubtree" %>
<ul id="SiteAnalytics" visible="false" runat="server">
	<li id="Li1" runat="server" title="sam overview" href="Analytics/reporting/Overview.aspx">sam overview</li>
	<li id="Li2" runat="server" title="sam visitors">sam visitors
	  <ul id="Ul1" runat="server">
		<%--<li runat="server" href="Analytics/reporting/Reports.aspx?report=">sam overview</li>--%>
		<%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam benchmarking</li>--%>
		<li id="Li3" runat="server" title="sam locations" href="Analytics/reporting/Reports.aspx?report=locations">sam locations</li>
		<li id="Li4" runat="server" title="sam new vs returning" href="Analytics/reporting/Reports.aspx?report=newvsreturning">sam new vs returning</li>
		<li id="Li5" runat="server" title="sam languages" href="Analytics/reporting/Reports.aspx?report=languages">sam languages</li>
		<li id="Li6" runat="server" title="sam visitor trending">sam visitor trending
		  <ul id="Ul2" runat="server">
			<li id="Li7" runat="server" title="sam visits" href="Analytics/reporting/Trends.aspx?report=visits">sam visits</li>
			<li id="Li8" runat="server" title="sam absolute unique visitors" href="Analytics/reporting/Trends.aspx?report=absoluteuniquevisitors">sam absolute unique visitors</li>
			<li id="Li9" runat="server" title="sam pageviews" href="Analytics/reporting/Trends.aspx?report=pageviews">sam pageviews</li>
			<li id="Li10" runat="server" title="sam average pageviews" href="Analytics/reporting/Trends.aspx?report=averagepageviews">sam average pageviews</li>
			<li id="Li11" runat="server" title="sam time on site" href="Analytics/reporting/Trends.aspx?report=timeonsite">sam time on site</li>
			<li id="Li12" runat="server" title="sam bounce rate" href="Analytics/reporting/Trends.aspx?report=bouncerate">sam bounce rate</li>
		  </ul>
		</li>
		<%--
		<li runat="server">sam visitor loyalty
		  <ul runat="server">
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam loyalty</li>
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam recency</li>
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam length of visit</li>
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam depth of visit</li>
		  </ul>
		</li>
		--%>
		<li id="Li13" runat="server" title="sam browser capabilities">sam browser capabilities
		  <ul id="Ul3" runat="server">
			<li id="Li14" runat="server" title="sam browsers" href="Analytics/reporting/Reports.aspx?report=browsers">sam browsers</li>
			<li id="Li15" runat="server" title="sam operating systems" href="Analytics/reporting/Reports.aspx?report=os">sam operating systems</li>
			<li id="Li16" runat="server" title="sam browsers and os" href="Analytics/reporting/Reports.aspx?report=platforms">sam browsers and os</li>
			<li id="Li17" runat="server" title="sam screen colors" href="Analytics/reporting/Reports.aspx?report=colors">sam screen colors</li>
			<li id="Li18" runat="server" title="sam screen resolutions" href="Analytics/reporting/Reports.aspx?report=resolutions">sam screen resolutions</li>
			<li id="Li19" runat="server" title="sam flash versions" href="Analytics/reporting/Reports.aspx?report=flash">sam flash versions</li>
			<li id="Li20" runat="server" title="sam java support" href="Analytics/reporting/Reports.aspx?report=java">sam java support</li>
		  </ul>
		</li>
		<li id="Li21" runat="server" title="sam network properties">sam network properties
		  <ul id="Ul4" runat="server">
			<li id="Li22" runat="server" title="sam network location" href="Analytics/reporting/Reports.aspx?report=networklocations">sam network location</li>
			<li id="Li23" runat="server" title="sam hostnames" href="Analytics/reporting/Reports.aspx?report=hostnames">sam hostnames</li>
			<li id="Li24" runat="server" title="sam connection speeds" href="Analytics/reporting/Reports.aspx?report=connectionspeeds">sam connection speeds</li>
		  </ul>
		</li>
		<li id="Li25" runat="server" title="sam user defined" href="Analytics/reporting/Reports.aspx?report=userdefined">sam user defined</li>
	  </ul>
	</li>
	<li id="Li26" runat="server" title="sam traffic sources">sam traffic sources
	  <ul id="Ul5" runat="server">
		<%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam overview</li>--%>
		<li id="Li27" runat="server" title="sam direct traffic" href="Analytics/reporting/Reports.aspx?report=direct">sam direct traffic</li>
		<li id="Li28" runat="server" title="sam referring sites" href="Analytics/reporting/Reports.aspx?report=referring">sam referring sites</li>
		<li id="Li29" runat="server" title="sam search engines" href="Analytics/reporting/Reports.aspx?report=searchengines">sam search engines</li>
		<li id="Li30" runat="server" title="sam all traffic sources" href="Analytics/reporting/Reports.aspx?report=trafficsources">sam all traffic sources</li>
		<%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam adwords
		  <ul runat="server">
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam adwords campaigns</li>
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam keyword positions</li>
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam tv campaigns</li>
		  </ul>
		</li>--%>
		<li id="Li31" runat="server" title="sam keywords" href="Analytics/reporting/Reports.aspx?report=keywords">sam keywords</li>
		<li id="Li32" runat="server" title="sam campaigns" href="Analytics/reporting/Reports.aspx?report=campaigns">sam campaigns</li>
		<li id="Li33" runat="server" title="sam ad versions" href="Analytics/reporting/Reports.aspx?report=adversions">sam ad versions</li>
	  </ul>
	</li>
	<li id="Li34" runat="server" title="sam content">sam content
	  <ul id="Ul6" runat="server">
		<%--<li runat="server" href="Analytics/reporting/Reports.aspx?report=">sam overview</li>--%>
		<li id="Li35" runat="server" title="sam top content" href="Analytics/reporting/Reports.aspx?report=topcontent">sam top content</li>
		<li id="Li36" runat="server" title="sam content by title" href="Analytics/reporting/Reports.aspx?report=contentbytitle">sam content by title</li>
		<li id="Li37" runat="server" title="sam top landing pages" href="Analytics/reporting/Reports.aspx?report=toplanding">sam top landing pages</li>
		<li id="Li38" runat="server" title="sam top exit pages" href="Analytics/reporting/Reports.aspx?report=topexit">sam top exit pages</li>
	    <%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam site overlay</li>--%>
		<%--
		<li runat="server" href="Analytics/reporting/tbd.aspx">sam site search</li>
		<li runat="server" href="Analytics/reporting/tbd.aspx">sam event tracking</li>
		--%>
	  </ul>
	</li>
</ul>

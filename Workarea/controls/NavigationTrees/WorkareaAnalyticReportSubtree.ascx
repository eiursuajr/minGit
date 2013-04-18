<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WorkareaAnalyticReportSubtree.ascx.cs" Inherits="WorkareaAnalyticReportSubtree" %>
<ul id="SiteAnalytics" visible="false" runat="server">
	<li id="Li1" runat="server" title="sam overview" href="Analytics/reporting/Overview.aspx">sam overview</li>
	<li id="Li2" runat="server" title="sam visitors">sam visitors
	  <ul id="Ul1" runat="server">
		<%--<li runat="server" href="Analytics/reporting/Reports.aspx?report=">sam overview</li>--%>
		<%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam benchmarking</li>--%>
		<li id="Li3" runat="server" href="Analytics/reporting/Reports.aspx?report=locations" title="sam locations">sam locations</li>
		<li id="Li4" runat="server" href="Analytics/reporting/Reports.aspx?report=newvsreturning" title="sam new vs returning">sam new vs returning</li>
		<li id="Li5" runat="server" href="Analytics/reporting/Reports.aspx?report=languages" title="sam languages">sam languages</li>
		<li id="Li6" runat="server" title="sam visitor trending">sam visitor trending
		  <ul id="Ul2" runat="server">
			<li id="Li7" runat="server" href="Analytics/reporting/Trends.aspx?report=visits" title="sam visits">sam visits</li>
			<li id="Li8" runat="server" href="Analytics/reporting/Trends.aspx?report=absoluteuniquevisitors" title="sam absolute unique visitors">sam absolute unique visitors</li>
			<li id="Li9" runat="server" href="Analytics/reporting/Trends.aspx?report=pageviews" title="sam pageviews">sam pageviews</li>
			<li id="Li10" runat="server" href="Analytics/reporting/Trends.aspx?report=averagepageviews" title="sam average pageviews">sam average pageviews</li>
			<li id="Li11" runat="server" href="Analytics/reporting/Trends.aspx?report=timeonsite" title="sam time on site">sam time on site</li>
			<li id="Li12" runat="server" href="Analytics/reporting/Trends.aspx?report=bouncerate" title="sam bounce rate">sam bounce rate</li>
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
			<li id="Li14" runat="server" href="Analytics/reporting/Reports.aspx?report=browsers" title="sam browsers">sam browsers</li>
			<li id="Li15" runat="server" href="Analytics/reporting/Reports.aspx?report=os" title="sam operating systems">sam operating systems</li>
			<li id="Li16" runat="server" href="Analytics/reporting/Reports.aspx?report=platforms" title="sam browsers and os">sam browsers and os</li>
			<li id="Li17" runat="server" href="Analytics/reporting/Reports.aspx?report=colors" title="sam screen colors">sam screen colors</li>
			<li id="Li18" runat="server" href="Analytics/reporting/Reports.aspx?report=resolutions" title="sam screen resolutions">sam screen resolutions</li>
			<li id="Li19" runat="server" href="Analytics/reporting/Reports.aspx?report=flash" title="sam flash versions">sam flash versions</li>
			<li id="Li20" runat="server" href="Analytics/reporting/Reports.aspx?report=java" title="sam java support">sam java support</li>
		  </ul>
		</li>
		<li id="Li21" runat="server" title="sam network properties">sam network properties
		  <ul id="Ul4" runat="server">
			<li id="Li22" runat="server" href="Analytics/reporting/Reports.aspx?report=networklocations" title="sam network location">sam network location</li>
			<li id="Li23" runat="server" href="Analytics/reporting/Reports.aspx?report=hostnames" title="sam hostnames">sam hostnames</li>
			<li id="Li24" runat="server" href="Analytics/reporting/Reports.aspx?report=connectionspeeds" title="sam connection speeds">sam connection speeds</li>
		  </ul>
		</li>
		<li id="Li25" runat="server" href="Analytics/reporting/Reports.aspx?report=userdefined" title="sam user defined">sam user defined</li>
	  </ul>
	</li>
	<li id="Li26" runat="server" title="sam traffic sources">sam traffic sources
	  <ul id="Ul5" runat="server">
		<%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam overview</li>--%>
		<li id="Li27" runat="server" href="Analytics/reporting/Reports.aspx?report=direct" title="sam direct traffic">sam direct traffic</li>
		<li id="Li28" runat="server" href="Analytics/reporting/Reports.aspx?report=referring" title="sam referring sites">sam referring sites</li>
		<li id="Li29" runat="server" href="Analytics/reporting/Reports.aspx?report=searchengines" title="sam search engines">sam search engines</li>
		<li id="Li30" runat="server" href="Analytics/reporting/Reports.aspx?report=trafficsources" title="sam all traffic sources<">sam all traffic sources</li>
		<%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam adwords
		  <ul runat="server">
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam adwords campaigns</li>
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam keyword positions</li>
			<li runat="server" href="Analytics/reporting/tbd.aspx">sam tv campaigns</li>
		  </ul>
		</li>--%>
		<li id="Li31" runat="server" href="Analytics/reporting/Reports.aspx?report=keywords" title="sam keywords">sam keywords</li>
		<li id="Li32" runat="server" href="Analytics/reporting/Reports.aspx?report=campaigns" title="sam campaigns">sam campaigns</li>
		<li id="Li33" runat="server" href="Analytics/reporting/Reports.aspx?report=adversions" title="sam ad versions">sam ad versions</li>
	  </ul>
	</li>
	<li id="Li34" runat="server" title="sam content">sam content
	  <ul id="Ul6" runat="server">
		<%--<li runat="server" href="Analytics/reporting/Reports.aspx?report=">sam overview</li>--%>
		<li id="Li35" runat="server" href="Analytics/reporting/Reports.aspx?report=topcontent" title="sam top content">sam top content</li>
		<li id="Li36" runat="server" href="Analytics/reporting/Reports.aspx?report=contentbytitle" title="sam content by title">sam content by title</li>
		<li id="Li37" runat="server" href="Analytics/reporting/Reports.aspx?report=toplanding" title="sam top landing pages">sam top landing pages</li>
		<li id="Li38" runat="server" href="Analytics/reporting/Reports.aspx?report=topexit" title="sam top exit pages">sam top exit pages</li>
	    <%--<li runat="server" href="Analytics/reporting/tbd.aspx">sam site overlay</li>--%>
		<%--
		<li runat="server" href="Analytics/reporting/tbd.aspx">sam site search</li>
		<li runat="server" href="Analytics/reporting/tbd.aspx">sam event tracking</li>
		--%>
	  </ul>
	</li>
</ul>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Summary.ascx.cs" Inherits="Analytics_reporting_Summary" %>
<style type="text/css">
    .summarytable td
    {
        padding: 5px;
    }
    .graphcolumn
    {
        vertical-align: top;
        width: 76px;
    }
    .statcolumn
    {
        font-weight: bold;
        vertical-align: top;
        text-align: right;
    }
</style>

<div class="AnalyticSummaryReport">
    <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
        <div class="AnalyticsErrorMessage"><asp:Literal ID="litErrorMessage" runat="server" /></div>
    </asp:Panel>
                
    <table border="0" cellpadding="5" cellspacing="5" class="summarytable">
	    <tbody>
			<tr id="rowVisits" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgVisits" runat="server" />
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnVisits" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litVisits" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblVisits" runat="server" /> <span class="perDay"><asp:Literal ID="litVisitsPerDay" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowVisits2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnVisits2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litVisits2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblVisits2" runat="server" /> <span class="perDay"><asp:Literal ID="litVisitsPerDay2" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowVisits3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnVisits3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litVisits3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblVisits3" runat="server" /> <span class="perDay"><asp:Literal ID="litVisitsPerDay3" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowVisits4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnVisits4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litVisits4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblVisits4" runat="server" /> <span class="perDay"><asp:Literal ID="litVisitsPerDay4" runat="server"/></span>
			    </td>
		    </tr>
			<tr id="rowPagesPerVisit" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgPagesPerVisit" runat="server" />
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnPPV" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litPagesPerVisit" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPagesPerVisit" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPagesPerVisit2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPPV2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litPagesPerVisit2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPagesPerVisit2" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPagesPerVisit3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPPV3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litPagesPerVisit3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPagesPerVisit3" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPagesPerVisit4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPPV4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litPagesPerVisit4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPagesPerVisit4" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPageviews" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_pageviews" runat="server" />
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnPV" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_pageviews" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPageviews" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_pageviewsperday" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowPageviews2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPV2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_pageviews2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPageviews2" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_pageviewsperday2" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowPageviews3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPV3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_pageviews3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPageviews3" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_pageviewsperday3" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowPageviews4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPV4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_pageviews4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPageviews4" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_pageviewsperday4" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowUniqueViews" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_uniqueviews" runat="server" />
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnUV" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_uniqueviews" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblUniqueViews" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_uniqueviewsperday" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowUniqueViews2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnUV2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_uniqueviews2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblUniqueViews2" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_uniqueviewsperday2" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowUniqueViews3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnUV3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_uniqueviews3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblUniqueViews3" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_uniqueviewsperday3" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowUniqueViews4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnUV4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_uniqueviews4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblUniqueViews4" runat="server" /> <span class="perDay"><asp:Literal ID="ltr_uniqueviewsperday4" runat="server"/></span>
			    </td>
		    </tr>
		    <tr id="rowTimeOnSite" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgTimeOnSite" runat="server"/>
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnTOS" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litTimeOnSite" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnSite" runat="server" />
			    </td>
		    </tr>
		     <tr id="rowTimeOnSite2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnTOS2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litTimeOnSite2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnSite2" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowTimeOnSite3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnTOS3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litTimeOnSite3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnSite3" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowTimeOnSite4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnTOS4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="litTimeOnSite4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnSite4" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowTimeOnPage" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_timeonpage" runat="server"/>
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnTOP" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_timeonpage" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnPage" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowTimeOnPage2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnTOP2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_timeonpage2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnPage2" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowTimeOnPage3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnTOP3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_timeonpage3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnPage3" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowTimeOnPage4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnTOP4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_timeonpage4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblTimeOnPage4" runat="server" />
			    </td>
		    </tr>
		    <%--<tr id="rowPercentNewVisits" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="imgPercentNewVisits" runat="server"/>
			    </td>
			    <td class="statcolumn">
				    <asp:Literal ID="litPercentNewVisits" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPercentNewVisits" runat="server" />
			    </td>
		    </tr>--%>
		    <tr id="rowBounceRate" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_bouncerate" runat="server"/>
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnBR" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_bouncerate" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblBounceRate" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowBounceRate2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnBR2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_bouncerate2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblBounceRate2" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowBounceRate3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnBR3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_bouncerate3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblBounceRate3" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowBounceRate4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnBR4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_bouncerate4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblBounceRate4" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPercentExit" runat="server" visible="false">
			    <td class="graphcolumn">
				    <asp:Image ID="img_percentexit" runat="server"/>
			    </td>
			    <td class="segmentcolumn" id="segmentcolumnPE" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_percentexit" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPercentExit" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPercentExit2" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPE2" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_percentexit2" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPercentExit2" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPercentExit3" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPE3" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_percentexit3" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPercentExit3" runat="server" />
			    </td>
		    </tr>
		    <tr id="rowPercentExit4" runat="server" visible="false">
			    <td>&#160;</td>
			    <td class="segmentcolumn" id="segmentcolumnPE4" runat="server" visible="false"></td>
			    <td class="statcolumn">
				    <asp:Literal ID="ltr_percentexit4" runat="server" />
			    </td>
			    <td>
				    <asp:Label ID="lblPercentExit4" runat="server" />
			    </td>
		    </tr>
        </tbody>
    </table>
</div>

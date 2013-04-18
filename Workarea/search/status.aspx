<%@ Page Language="C#" AutoEventWireup="true" CodeFile="status.aspx.cs" Inherits="Workarea_search_status" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search Status</title>
    <asp:Literal ID="styleHelper" runat="server"></asp:Literal>

    <style type="text/css">
        div.sectionHeader { font-weight: bold; color: #1d5987; }
        table.ektronGrid { margin-bottom: 15px; }
        td.header.label {width: 150px; text-align: left;}
        td.value { width: 200px; }
        .description { color: #888888; text-align: left; font-weight: normal;}
    </style>

    <script type="text/javascript">

        function crawlIncremental() {
            var result = confirm('<asp:Literal ID="ltrincremental" runat="server"></asp:Literal>');
            if (result) {
                alert('<asp:Literal ID="ltrincrementalRequest" runat="server"></asp:Literal>');
                document.location = "status.aspx?action=incremental";
            }
        }

        function crawlFull() {
            var result = confirm('<asp:Literal ID="ltrFullC" runat="server"></asp:Literal>');
            if (result) {
                alert('<asp:Literal ID="ltrFullRequest" runat="server"></asp:Literal>');
                document.location = "status.aspx?action=full";
            }
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="htmToolBar" runat="server">
                <table>
                    <tr>
                        <td id="tdIncrementalCrawlButton" runat="server"></td>
                        <td id="tdFullCrawlButton" runat="server"></td>
                        <td id="tdRefreshButton" runat="server"></td>
						<%=StyleHelper.ActionBarDivider %>
						<td id="tdSearchStatusHelpButton" runat="server"></td>
                    </tr>
                </table>
            </div>
        </div>
        <div>
            <div class="ektronPageInfo">
                <div class="sectionHeader">
                    <div class="title"><%=this.Sites.EkMsgRef.GetMessage("lbl Status Information")%></div>
                    <div class="description">
                    <%=this.Sites.EkMsgRef.GetMessage("lbl status desc")%>
                    </div>
                </div>
                <table class="ektronGrid">
                    <tr class="title-header">
                        <th></th>
                        <th><%=this.Sites.EkMsgRef.GetMessage("lbl value")%></th>
                        <th><%=this.Sites.EkMsgRef.GetMessage("lbl description")%></th>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Search Server")%></td>
                        <td class="value"><asp:Literal ID="ucSearchServer" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl search server host")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Content Source Name")%></td>
                        <td class="value"><asp:Literal ID="ucContentSourceName" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl index associated site")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Query Credentials")%></td>
                        <td class="value"><asp:Literal ID="ucUsername" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl Windows user authorized")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Incremental Crawl Request Pending")%></td>
                        <td class="value"><asp:Literal ID="ucIsCrawlSchedule" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl incremental crawl submit")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Current Action")%></td>
                        <td class="value"><asp:Literal ID="ucCurrentAction" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl indexing request")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Next Action")%></td>
                        <td class="value"><asp:Literal ID="ucPendingAction" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl indexing completion")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Crawl Start Time")%></td>
                        <td class="value"><asp:Literal ID="ucCrawlStartTime" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl start recent crawl")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Crawl End Time")%></td>
                        <td class="value"><asp:Literal ID="ucCrawlEndTime" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl end recent crawl")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Crawl Duration")%></td>
                        <td class="value"><asp:Literal ID="ucCrawlDuration" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl duration recent crawl")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Incremental Crawl Interval")%></td>
                        <td class="value"><asp:Literal ID="ucCrawlInterval" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl Indicates frequency")%></td>
                    </tr>
                </table>

                <div class="sectionHeader">
                    <div class="title"><%=this.Sites.EkMsgRef.GetMessage("lbl Crawl Filters")%></div>
                    <div class="description">
                    <%=this.Sites.EkMsgRef.GetMessage("lbl following filters")%>
                    </div>
                </div>

                <table class="ektronGrid">
                    <tr class="title-header">
                        <th></th>
                        <th><%=this.Sites.EkMsgRef.GetMessage("btn filter")%></th>
                        <th><%=this.Sites.EkMsgRef.GetMessage("generic description")%></th>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl html content")%></td>
                        <td class="value"><asp:Literal ID="ucIncludeHtmlContent" runat="server"></asp:Literal></td>    
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl cms contents")%></td>                            
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Document Content")%></td>
                        <td class="value"><asp:Literal ID="ucIncludeDocumentContent" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl dms contents")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Forum Content")%></td>
                        <td class="value"><asp:Literal ID="ucIncludeForumContent" runat="server"></asp:Literal></td>
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl forum contents")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Product Content")%></td>                                    
                        <td class="value"><asp:Literal ID="ucIncludeProductContent" runat="server"></asp:Literal></td>    
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl product contents")%></td>
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Community Members")%></td>                                    
                        <td class="value"><asp:Literal ID="ucIncludeCommunityMembers" runat="server"></asp:Literal></td>   
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl Community etc")%></td>                                 
                    </tr>
                    <tr>
                        <td class="header label"><%=this.Sites.EkMsgRef.GetMessage("lbl Community Content")%></td>                                    
                        <td class="value"><asp:Literal ID="ucIncludeCommunityContent" runat="server"></asp:Literal></td>                                    
                        <td class="description"><%=this.Sites.EkMsgRef.GetMessage("lbl Community Content etc")%></td>
                    </tr>
                </table>

            </div>
        </div>
    
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports" validateRequest="false" CodeFile="reports.aspx.cs" %>
<%@ Reference Control="controls/reports/reportstoolbar.ascx" %>
<%@ Register TagPrefix="uxEktron" TagName="Paging" Src="controls/paging/paging.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
	    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	    <title>Reports</title>
		<asp:placeHolder id="phHead" runat="server">
		    <asp:literal id="litStyleSheetJs" runat="server" />
		    <script type="text/javascript">
	            <!--//--><![CDATA[//><!--
		            var DisableButtons = false;
		            
		            function DisplayHoldMsg_Local(flag) {
			            // call the standard function:
			            DisableButtons = flag;
			            DisplayHoldMsg(flag);
		            }
		            function WarnAllCheckin() {
			            if (DisableButtons)
			            {
				            return false;
			            }
			            return confirm("<%=_MessageHelper.GetMessage("js: checkin all displayed warning")%>");
		            }
		            function CloseChildPage() {

			            if (document.getElementById("EmailFrameContainer") != null) {
				            var pageObj = document.getElementById("EmailFrameContainer");
			            }
			            else {
				            if (parent.document.getElementById("EmailFrameContainer") != null) {
					            var pageObj = parent.document.getElementById("EmailFrameContainer");
				            }
			            }
			            // Configure the email window to be invisible:
			            pageObj.style.display = "none";
			            pageObj.style.width = "1px";
			            pageObj.style.height = "1px";

			            // Ensure that the transparent layer does not cover any of the parent window:
			            pageObj = document.getElementById("EmailActiveOverlay");

			            if (document.getElementById("EmailActiveOverlay") != null) {
				            var pageObj = document.getElementById("EmailActiveOverlay");
			            }
			            else {
				            if (parent.document.getElementById("EmailActiveOverlay") != null) {
					            var pageObj = parent.document.getElementById("EmailActiveOverlay");
				            }
			            }
			            pageObj.style.display = "none";
			            pageObj.style.width = "1px";
			            pageObj.style.height = "1px";
		            }
		            function WarnAllSubmit() {
			            if (DisableButtons)
			            {
				            return false;
			            }
			            return confirm("<%=_MessageHelper.GetMessage("js: submit all displayed warning")%>");
		            }
		            function SelectAll() {
			            var lLoop = 0;
			            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
				            if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
						            && (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1)) {
					            document.forms.selections[lLoop].checked = true;
				            }
			            }
			            var cArray = Collections.split(",");
			            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
				            if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden")
								            && (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)) {
					            var cIndex = document.forms.selections[lLoop].name.toLowerCase().replace("frm_hidden", "");
					            document.forms.selections[lLoop].value = cArray[cIndex];
				            }
			            }
		            }
		            function ClearAll() {
			            var lLoop = 0;
			            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
				            if ((document.forms.selections[lLoop].type.toLowerCase() == "checkbox")
						            && (document.forms.selections[lLoop].name.toLowerCase().search("frm_check") != -1)) {
					            document.forms.selections[lLoop].checked = false;
				            }
			            }
			            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
				            if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden")
								            && (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)) {
					            document.forms.selections[lLoop].value = 0;
				            }
			            }
		            }
		            function GetIDs() {
			            var lLoop = 0;
			            document.forms.selections.frm_content_ids.value = "";
			            document.forms.selections.frm_language_ids.value = "";
			            document.forms.selections.frm_folder_ids.value = "";
			            var fArray = Folders.split(",");
			            var lArray = Languages.split(",");
			            for (lLoop = 0; lLoop < document.forms.selections.length; lLoop++) {
				            if ((document.forms.selections[lLoop].type.toLowerCase() == "hidden")
						            && (document.forms.selections[lLoop].name.toLowerCase().search("frm_hidden") != -1)
						            && (document.forms.selections[lLoop].value != 0)) {
					            document.forms.selections.frm_content_ids.value = document.forms.selections.frm_content_ids.value + "," + document.forms.selections[lLoop].value;
					            var cIndex = document.forms.selections[lLoop].name.toLowerCase().replace("frm_hidden", "");
					            document.forms.selections.frm_folder_ids.value = document.forms.selections.frm_folder_ids.value + "," + fArray[cIndex];
					            document.forms.selections.frm_language_ids.value = document.forms.selections.frm_language_ids.value + "," + lArray[cIndex];
				            }
			            }
			            document.forms.selections.frm_content_ids.value = document.forms.selections.frm_content_ids.value.substring(1, document.forms.selections.frm_content_ids.value.length);
			            document.forms.selections.frm_folder_ids.value = document.forms.selections.frm_folder_ids.value.substring(1, document.forms.selections.frm_folder_ids.value.length);
			            document.forms.selections.frm_language_ids.value = document.forms.selections.frm_language_ids.value.substring(1, document.forms.selections.frm_language_ids.value.length);
			            if (document.forms.selections.frm_content_ids.value.length == 0) {
				            alert("<%=_MessageHelper.GetMessage("js:no items selected")%>");
				            return false;
			            }
			            <asp:literal id="lblAction" runat="server"/>
			            return false;
		            }
		            function ReturnChildValue (foldername) {
			            CloseChildPage();
			            document.getElementById("folderidspan").innerHTML = "<div id=\"div3\" style=\"display: none;position: block;\"></div><div id=\"folderidspan\" style=\"display: block;position: block;\">(" + foldername + ")&nbsp;" + "&nbsp;&nbsp;<a href=\"#\" onclick=\"LoadFolderChildPage();return true;\">Change</a>" + "&nbsp;&nbsp;</div>";
		            }
		            function export_result()
		            {
			            //document.selections.result_type.value="export";
			            var startDate = document.getElementById("start_date").value;
			            var endDate = document.getElementById("end_date").value;
			            var inclFolder = document.getElementById("fId").value;
			            var rootFolder = document.getElementById("rootFolder").value;
			            var subFld = document.getElementById("subfldInclude").value;
			            var exclUser = document.getElementById("excludeUserIds").value;
			            var exclGroup = document.getElementById("excludeUserGroups").value;
			            //var selObj = document.selections.selDisplay;
			            //var sDisplay = selObj[selObj.selectedIndex].value;
                        var sDisplay = $ektron("#selDisplay")[0].selectedIndex

			            var LangType = document.getElementById("LangType").value;
			            var iContType = document.getElementById("ContType").value;

			            location.href = "reports.aspx?filterid=" + inclFolder + "&startdate='" + startDate + "'&enddate='" + endDate + "'&subfldinclude=" + subFld + "&action=siteupdateactivity&reporttype=export&btnSubmit=1&filtertype=path&ex_users=" + exclUser + "&ex_groups=" + exclGroup + "&report_display=" + sDisplay + "&ContType=" + iContType + "&LangType=" + LangType + "&rootfolder=" + rootFolder;
		            }
        		    
        		    function LoadLanguage(FormName)
        		    {
                        var num=document.forms[FormName].selLang.selectedIndex;
                        document.forms[FormName].action = "reports.aspx?action=" + '<asp:Literal ID="litPageAction" runat="server" />' + "&LangType=" + document.forms[0].selLang.options[num].value + "&orderby=" + '<asp:Literal ID="litOrderBy" runat="server" />' + "&filtertype=" + '<asp:Literal ID="litFilterType" runat="server" />' + "&filterid=" + '<asp:Literal ID="litFilterId" runat="server" />' + "&interval=" + '<asp:Literal ID="litInterval" runat="server" />';
                        document.forms[FormName].submit();
                        return false;
                    }
        		   
		            Collections = "<asp:literal id="litCollectionList" runat="server"/>";
		            Folders = "<asp:literal id="litFolderList" runat="server"/>";
		            Languages = "<asp:literal id="litLanguageList" runat="server"/>";

		            function showDetails(sId)
		            {
			            var obj = document.getElementById("PageActivity_" + sId);
			            if (obj && ("undefined" != typeof obj))
			            {
				            if ("none" == obj.style.display)
				            {
					            obj.style.display = "";
				            }
				            else
				            {
					            obj.style.display = "none";
				            }
			            }
		            }
		            function hideDetails(folderCount)
		            {
			            if (("undefined" != typeof document.body.readyState)
				            && ("complete" == document.body.readyState))
			            {
				            for (var i = 0; i < folderCount; i++)
				            {
					            var obj = document.getElementById("PageActivity_" + i);
					            if (obj && ("undefined" != typeof obj))
					            {
						            obj.style.display = "none";
					            }
				            }
			            }
			            else
			            {
				            setTimeout("hideDetails(" + folderCount + ")", 100);
			            }
		            }
		            function pageLoaded()
		            {
			            var i = 1;
			            var obj = document.getElementById("PageActivity_" + i);
			            if ("ev" == g_displayMode)
			            {
				            while (obj && ("undefined" != typeof obj))
				            {
					            obj.style.display = "none";
					            i++;
					            obj = document.getElementById("PageActivity_" + i);
				            }
			            }

			            obj = document.getElementById("UpdateActivityTbl_div");
			            if (obj && ("undefined" != typeof obj))
			            {
				            obj.style.display = "block";
			            }
		            }

		            var g_displayMode = "";
		            function setDisplayMode(displayMode)
		            {
			            g_displayMode = displayMode;
		            }
		            checkAll = function(obj)
		            {
		                var checked = obj.checked;
		                if(checked)
		                {
		                    SelectAll();
		                    return false;
		                }
		                else
		                {
		                    ClearAll();
		                    return false
		                }
		            }
			    //--><!]]>
		    </script>
		</asp:placeHolder>
        <style type="text/css">
            .ektronPageInfo { padding:0; }
            a.buttonGetResult {
            	/*background-image: url(Images/ui/icons/chartBar.png); */
            	/*background-position:.15em center;*/
            }
            #dvHoldMessage{display: none; position: absolute; opacity: .5; filter:Alpha(Opacity=50); -ms-filter:"progid:DXImageTransform.Microsoft.Alpha(Opacity=50)";
             z-index: 9999; border: none; padding: 0; margin: 0; top: 0; left: 0; right: 0; bottom: 0; background-color: #fff;}
            #dvHoldMessage h3{position: absolute; z-index: 10000; width: 128px; height: 128px; margin: -64px 0 0 -64px; background-color: #fff; background-image: url("images/ui/loading_big.gif"); backgground-repeat: no-repeat; text-indent: -10000px; border: none; padding: 0; top: 50%; left: 50%;}
            .smallCell {width:20px;}
            .mediumCell {width:50px;}
        </style>
	</head>
	<body onload="pageLoaded()">
		<asp:literal id="EmailArea" runat="server" />
		<form id="selections" action="reports.aspx" method="post" runat="server">
			<asp:literal id="PostBackPage" Runat="server" />
			<input id="rptHtml" type="hidden" name="rptHtml"/>
			<asp:Literal id="SiteActivityHtml" runat="server" />
			<input id="rptTitle" type="hidden" name="rptTitle" runat="server"/>
			<input id="rptFolder" type="hidden" name="rptFolder"/>
			<div id="dvHoldMessage">
				<h3><%=_MessageHelper.GetMessage("one moment msg")%></h3>
			</div>
			<asp:placeholder id="ToolBarHolder" Runat="server" />
			<div class="ektronPageContainer ektronPageInfo">
		    <asp:MultiView ID="mvReports" runat="server">
		        <asp:View ID="vwReportLegacy" runat="server">
			    <table class="ektronGrid" width="100%">
				    <tr id="top" Visible="False" runat="server">
					    <td colspan="2">
						    <input id="txtInterval" type="hidden" name="txtInterval" runat="server" visible="false"/>
						    <select id="selInterval" onchange="selChange(this)" name="selInterval" runat="server" visible="false">
						    </select>
        				    <asp:literal id="lblDays" runat="server" Visible="False" />
				        </td>
				    </tr>
				    <tr id="tr_startDate" visible="False" runat="server">
					    <td class="label" title="Start Date"><asp:literal id="lblStartDate" Runat="server" /></td>
					    <td class="value"><asp:literal id="dtStart" Runat="server" /></td>
				    </tr>
				    <tr id="tr_endDate" visible="False" runat="server">
					    <td class="label" title="End Date"><asp:literal id="lblEndDate" Runat="server" /></td>
					    <td class="value"><asp:literal id="dtEnd" Runat="server" /></td>
				    </tr>
				</table>
				<table class="ektronGrid" width="100%">
				    <tr id="editSchedule" visible="False" runat="server">
					    <td title="Edit Schedule"><asp:literal id="EditScheduleHtml" Runat="server" /></td>
				    </tr>
				    <tr runat="server" id="tr_SiteUpdateActivity" visible="false">
					    <td><asp:literal id="SiteUpdateActivity" Runat="server" Visible="False" /></td>
				    </tr>
				    <tr id="tr_phUpdateActivity" Runat="server" visible="false">
                        <td>
                            <div id="UpdateActivityTbl_div" style="display: none">
                                <asp:PlaceHolder ID="phUpdateActivity" runat="server" Visible="True" />
                            </div>
                        </td>
				    </tr>
			    </table>
	            <div id="ReportDataGrid">
	                <asp:DataGrid id="dgReport"
	                    runat="server"
	                    Width="100%"
	                    AutoGenerateColumns="False"
	                    CssClass="ektronGrid ektronTopSpaceSmall"
			            GridLines="None">
                        <HeaderStyle CssClass="title-header" />
		            </asp:DataGrid>
		        </div>
		         <uxEktron:Paging ID="uxPaging" runat="server" Visible="false" />
			    <asp:image id="chart" Runat="server" Visible="False" ImageUrl="chart.aspx" />
				<asp:label id="lblTbl" Runat="server" Visible="False" />
			    <asp:literal id="SearchPhraseReportLit" Runat="server" />
			
			<input type="hidden" name="frm_content_ids"/> <input type="hidden" name="frm_language_ids"/>
			<input type="hidden" name="frm_folder_ids"/>
			<asp:HiddenField ID="hdn_timespan" runat="server" />
		        </asp:View>
		        <asp:View ID="vwReportV8" runat="server">
		           <%-- <asp:PlaceHolder ID="phReportControl" Runat="server"/>--%>
		        </asp:View>
		    </asp:MultiView>
		    </div>
		</form>
	</body>
</html>


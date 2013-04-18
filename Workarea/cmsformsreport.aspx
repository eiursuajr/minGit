<%@ Page Language="C#" AutoEventWireup="true" Inherits="cmsformsreport" CodeFile="cmsformsreport.aspx.cs" %>
<%@ Register TagPrefix="uxEktron" TagName="Paging" Src="controls/paging/paging.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="lblTitle" runat="server" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <asp:Literal ID="StyleSheetJS" runat="server" />
    <link rel="stylesheet" type="text/css" href="csslib/ektron.fixedPositionToolbar.css" />
    <script src="java/empjsfunc.js" type="text/javascript"></script>
    <script src="java/toolbar_roll.js" type="text/javascript"></script>
    <script src="java/internCalendarDisplayFuncs.js" type="text/javascript"></script>
    <script type="text/javascript">
		    <!--//--><![CDATA[//><!--
		        var invalidFormatMsg = '<asp:Literal ID="ltrInvalidErrorMsg" runat="server"></asp:Literal>';
	        <asp:Literal ID="ltrFormDataids" runat="server"></asp:Literal>
	        
	        var strFolderID = '<asp:Literal ID="ltrStrFolderID" runat="server"></asp:Literal>';
	        var FormId = '<asp:Literal ID="ltrFormID" runat="server"></asp:Literal>';
	        var ContentLanguage = '<asp:Literal ID="ltrContentLanguage" runat="server"></asp:Literal>';
	        var DefaultFormTitle = '<asp:Literal ID="ltrDefaultFormTitle" runat="server"></asp:Literal>';
		    var deleteFormDataID;
		    //ecmMonths = "AppUI.GetenglishMonthsAbbrev"; // Both IE and NS	
		    function submit_form(op){	
			    /*
			    Don't know why start date is required.	
			    if ((document.getElementById("start_date").value == undefined) || (document.getElementById("start_date").value == "")) {
				    alert ("You must enter the start date ");
				    return false;
			    }
			    */    			
			    document.forms[0].flag.value="true";
			    if (typeof document.forms[0].selform != "undefined") {
				    var idx=document.forms[0].selform.selectedIndex;
				    document.forms[0].form_title.value =document.forms[0].selform.options[idx].text;
				    document.forms[0].form_id.value=document.forms[0].selform.options[idx].value;			
			    }
			    else
			    {
				    //document.forms[0].form_title.value = document.forms[0].selformTitle.value;
				    //document.forms[0].form_id.value = document.forms[0].selformId.value;
			    }
			    document.forms[0].data_type.value=document.forms[0].seltype.value;
			    document.forms[0].display_type.value=document.forms[0].seldisplay.value;
			    var selhid = 0;
			    var selhtitle = "";
			    if (typeof document.getElementById("selhid") != null && document.getElementById("selhid").value != "none")
			    {
				    document.forms[0].selhid.value = document.forms[0].selhistory.value;	
				    selhid = document.forms[0].selhid.value + "";	
				    if (typeof document.getElementById("hid_" + selhid) != null)
				    {
					    selhtitle = document.getElementById("hid_" + selhid).value;	
				    }
			    }
			    if (op=="show"){
				    if ((document.forms[0].start_date.value != "") && (document.forms[0].end_date.value != "")) {
					    if (!EkDTCompareDates(document.forms[0].start_date, document.forms[0].end_date))
					    {
						    var msg = "You cannot have the start date later than the end date.";
						    alert('<asp:Literal ID="ltrAlertStartDate" runat="server"></asp:Literal>');
						    return false;				
					    }
				    }
				    document.forms[0].result_type.value="show";
				    if (0 == selhid)
				    {
				        if (strFolderID != "")
				        {
				            document.forms[0].action = "cmsformsreport.aspx?id=" + FormId + "&LangType=" + ContentLanguage + "&FormTitle=" + DefaultFormTitle + "&folder_id=" + strFolderID;
				        }
				        else
				        {
				            document.forms[0].action = "cmsformsreport.aspx?id=" + FormId + "&LangType=" + ContentLanguage + "&FormTitle=" + DefaultFormTitle;
				        }
				    }				    
				    else
				    {
					    if ("" == selhtitle)
					    {
					        selhtitle = DefaultFormTitle;
					    }
					    if (strFolderID != "")
				        {
				            document.forms[0].action = "cmsformsreport.aspx?id=" + FormId + "&LangType=" + ContentLanguage + "&FormTitle=" + selhtitle + "&hid=" + selhid + "&folder_id=" + strFolderID;
                        }
				        else
				        {
				            document.forms[0].action = "cmsformsreport.aspx?id=" + FormId + "&LangType=" + ContentLanguage + "&FormTitle=" + selhtitle + "&hid=" + selhid;
				        }
				    }
				    document.forms[0].submit();
			    }else{
				    document.forms[0].result_type.value="export";
				    window.open("cmsformsreport.aspx?flag=true&result_type=export&data_type=" + document.forms[0].data_type.value + "&LangType=" + ContentLanguage + "", "rptwin1", "width=650,height=350,resizable,scrollbars,status,titlebar");
			    }
    			
		    }
//		    function export_result(){
//			    document.forms[0].result_type.value="export";
//			    location.href = "exportformdata.aspx?flag=true&result_type=export&display_type=" +document.forms[0].display_type.value + "&data_type=" +document.forms[0].data_type.value+"&form_id="+document.forms[0].form_id.value+"&form_title="+document.forms[0].form_title.value+"&fieldname="+document.forms[0].fieldname.value+"&qlang="+document.forms[0].qlang.value;
//			    //window.open("exportformdata.aspx?flag=true&result_type=export&display_type=" +document.forms[0].display_type.value + "&data_type=" +document.forms[0].data_type.value+"&form_id="+document.forms[0].form_id.value+"&form_title="+document.forms[0].form_title.value,"rptwin","width=650,height=350,resizable,scrollbars,status,titlebar");
//			    // I know of no way to close the popup window (except to just call .close after some time).
//			    // Calling .close when the Save As dialog is displayed sounds a beep.
//			    // This is a common problem, as found when searching the web.
//			    //    "You don't need an additional window. Just link to the page directly. 
//			    //     The browser will see you're downloading a file and leave the current page in tact. ...
//			    //     It shows the download dialog as a separate dialog, without affecting the page that has the link."
//			    //     Imar Spaanjaars
//			    //     Everyone is unique, except for me.
//			    //		[http://p2p.wrox.com/topic.asp?TOPIC_ID=31831 as seen on 2005-12-05]
//			    // -doug.domeny 2005-12-05
//		    }
    		
		    function CheckIt(Obj) {
			    return false;
		    }
		    function SelectAll(Obj){
			    if (Obj.checked){
				    for (var i = 0; i < arFormDataId.length; i++) {
					    var objTmp = eval("document.forms[0].ektChk"+ arFormDataId[i]);
					    objTmp.checked = true;
				    }
			    }else{
				    for (var i = 0; i < arFormDataId.length; i++) {
					    var objTmp = eval("document.forms[0].ektChk"+ arFormDataId[i]);
					    objTmp.checked = false;
				    }
			    }
		    }
		    function ConfirmDelete() 
		    {
			    if ("8" == document.forms[0].seldisplay.options[document.forms[0].seldisplay.selectedIndex].value)
			    {
				    // cannot delete individual entry from "Submitted Data as XML" report type.
				    // the seldisplay.options value is "8"
				    return false;
			    }
			    var DeleteFormDataID = "";
			    for (var i = 0; i < arFormDataId.length; i++) 
			    {	
				    var obj = eval("document.forms[0].ektChk"+ arFormDataId[i]);
				    if ("undefined" == typeof obj)
				    {
				        alert('<asp:Literal ID="ltrReportDel" runat="server"></asp:Literal>');
					    return false;
				    }
				    else
				    {
					    if (typeof obj.checked && obj.checked) 
					    {
						    if (DeleteFormDataID == "") 
						    {
							    DeleteFormDataID = arFormDataId[i];
						    }
						    else{
							    DeleteFormDataID = DeleteFormDataID + "," + arFormDataId[i];
						    }
					    }
				    }
			    }
			    if (DeleteFormDataID == "") 
			    {
			        alert('<asp:Literal ID="ltrDelFormData" runat="server"></asp:Literal>');
			    } 
			    else 
			    {
			        if (confirm('<asp:Literal ID="ltrDelSelFormData" runat="server"></asp:Literal>'))
				    {								
					    document.forms[0].delete_data_id.value = DeleteFormDataID;
					    document.forms[0].action = "cmsformsreport.aspx?action=delete&id=" + FormId + " &LangType=" + ContentLanguage;
					    document.forms[0].submit();
				    }
			    }
			    return false;
		    }

            function onBeforeExport()
            {
                if (parseInt(document.forms[0].totalPages.value, 10) > 1)
                {
                    alert('<asp:Literal ID="ltrWaitMsg" runat="server"></asp:Literal>');
                }
                document.forms[0].result_type.value="export";
                return true;
            }
		    //--><!]]>     
    </script>
    <style type="text/css">
        a.buttonGetResult
        {
            background-image: url(Images/ui/icons/chartBar.png);
            background-position: .6em center;
        }
        .warningError
        {
            padding: .5em 1em .5em 2.25em;
            margin: .5em 0;
            background-repeat: no-repeat;
            background-image: url(Images/ui/icons/error.png);
            background-position: .6em center;
        }
        table.ektronReport td, th
        {
            border: 1px solid #e7f0f7 !important;
        }
        table.ektronReport
        {
            border-color: #e7f0f7 !important;
        }
        .lblsFormTitle
        {
            text-align: center !important;
            font-size: larger;
        }
    </style>
</head>
<body>
    <form id="frmReport" name="frmReport" method="post" runat="server">
    <div class="ektronPageHeader">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
    </div>
    <div class="ektronPageContainer ektronPageInfo">
        <input type="hidden" name="flag" value="false" />
        <input type="hidden" name="form_id" value="<%=FormId%>" />
        <input type="hidden" name="data_type" value="<%=DataType%>" />
        <input type="hidden" name="form_title" value="<%=(DefaultFormTitle)%>" />
        <asp:HiddenField ID="result_type" runat="server" />
        <input type="hidden" name="display_type" value="<%=DisplayType%>" />
        <input type="hidden" name="delete_data_id" value="" />
        <input type="hidden" name="fieldname" value="<%=sPollFieldId%>" />
        <input type="hidden" name="qlang" value="<%=QueryLang%>" />
        <asp:Literal runat="server" ID="resultsMessage" />
        <table class="ektronGrid">
            <tr>
                <td class="label" title="Start Date">
                    <asp:Literal ID="lblStartDate" runat="server" />
                </td>
                <td>
                    <asp:Literal ID="dtStart" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label" title="End Date">
                    <asp:Literal ID="lblEndDate" runat="server" />
                </td>
                <td>
                    <asp:Literal ID="dtEnd" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label" title="Report Display">
                    <%=m_refMsg.GetMessage("lbl report display")%>:
                </td>
                <td colspan="3">
                    <asp:Literal ID="SelectFormReport" runat="server" />
                    <%--<select id="seldisplay" name="seldisplay">
							    <option value="vertical" selected=" <% if (DisplayType = "vertical"){ %>selected<% } %>">Vertical</option>
							    <option value="horizontal" selected="<% if (DisplayType = "horizontal") {%> selected<% } %>">Horizontal</option>
						    </select>--%>
                </td>
            </tr>
            <asp:Literal ID="SelectHistoryReport" runat="server" />
        </table>
        <input type="hidden" id="seltype" name="seltype" value="All" />
        <div class="ektronTopSpace">
            <ul class="buttonWrapperLeft ui-helper-clearfix">
                <li><a title="Get Report" class="button buttonLeft greenHover" type="button"
                    id="btnShow" name="btnShow" value="<%= m_refMsg.GetMessage("btn get result")%>"
                    onclick="submit_form('show');">
                    <asp:Literal runat="server" ID="litGetResult" /></a> </li>
            </ul>
        </div>
        <div class="ektronTopSpace" style="clear: both;">
            <asp:Literal ID="FormResult" runat="server" />
            <uxEktron:Paging ID="uxPaging" runat="server" />
        </div>
        <div class="ektronTopSpace">
            <asp:LinkButton class="button buttonLeft greenHover buttonExport" ID="BtnExport" 
                runat="server" Text="Export Report" OnClick="BtnExport_Click" OnClientClick="return onBeforeExport()" />
            <asp:Literal ID="ExportResult" runat="server" />
            <asp:HiddenField ID="totalPages" runat="server" />
        </div>
    </div>
    </form>
</body>
</html>

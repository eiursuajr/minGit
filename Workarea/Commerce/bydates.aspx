<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bydates.aspx.cs" Inherits="Commerce_bydates" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reporting By Dates</title>

    <script language="JavaScript" type="text/javascript">
    Ektron.ready( function() {
        $ektron('#go_live_span').addClass('minWidthSpan');
        $ektron('#end_date_span').addClass('minWidthSpan');    
    });
    function captureValue()
    {
        var startDate = new Date();
        var endDate = new Date();
        var hdnStartDate = document.getElementById('hdnStartDate');
        var hdnEndDate = document.getElementById('hdnEndDate');
        
        startDate = document.getElementById('go_live_span');
        endDate = document.getElementById('end_date_span');
        
        if ((startDate.innerHTML == undefined) || (endDate.innerHTML == undefined) || (startDate.innerHTML ==  '[None]') || (endDate.innerHTML ==  '[None]') || (startDate.innerHTML == '') || (endDate.innerHTML == '') || (endDate.value < startDate.value))
        {   
            alert('<asp:Literal runat="server" id="ltr_errStartEndDate" />');
            return false;
        }
        else
        {
            hdnStartDate.value = startDate.value;
            hdnEndDate.value = endDate.value;
                     
            window.parent.location.href = "fulfillment.aspx?action=bydates&startdate="+hdnStartDate.value+"&enddate="+hdnEndDate.value;
            return false;
        } 
    }    

    function Close() 
    {
        parent.ektb_remove();
    }
    </script>
    <style type="text/css">
        .minWidthSpan { width: 260px !important;}
        .btnFilter { display:inline-block !important; margin: 1em 1em 0 0 !important; }
    </style>
</head>
<body>
    <form id="frmMain" runat="server">
        <div>            
            <table class="ektronGrid">                
                <tr>
                    <td title="Start Date">
                        <asp:Literal Visible="true" ID="ltr_startdate" runat="server" />:</td>
                    <td title="Select a Start Date and Time from the Calendar">
                        <asp:Literal Visible="true" ID="ltr_startdatesel" runat="Server"/>
                    </td>
                </tr>
                <tr>
                    <td title="End Date">
                        <asp:Literal Visible="true" ID="ltr_enddate" runat="server" />:</td>
                    <td title="Select an End Date and Time from the Calendar">
                        <asp:Literal Visible="true" ID="ltr_enddatesel" runat="Server"/>
                    </td>
                </tr>                
            </table>
            <div class="right">
                 <a class="button buttonFilter blueHover btnFilter" onclick="captureValue();" value="Filter" id="Button1" name="btnFilter" title="Filter">Filter</a>
            </div>
            <input type="hidden" id="hdnStartDate" runat="server" name="hdnStartDate" />
            <input type="hidden" id="hdnEndDate" runat="server" name="hdnEndDate" />           
        </div>
    </form>
</body>
</html>


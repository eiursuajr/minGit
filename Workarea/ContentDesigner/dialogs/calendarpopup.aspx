<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="calendarpopup.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.CalendarPopup" AutoEventWireup="false" %>
<%@ Register TagPrefix="radCln" Namespace="Telerik.WebControls" Assembly="RadCalendar.NET2" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title id="Title" runat="server">Select Date</title>
    <script language="javascript" type="text/javascript">
    <!--
        var hdnSelectedDate_ClientID = "<asp:literal id="hdnSelectedDate_ClientID" runat="server" />";
        function Calendar_OnDateClick(calendarInstance, args)
        {
            var tDate = args.RenderDay.Date;
            var dSelected = new Date(tDate[0], tDate[1] -1, tDate[2]);
            document.getElementById(hdnSelectedDate_ClientID).value = Ektron.Xml.serializeDate(dSelected);
        }
	// -->
    </script>
</head>
<body class="dialog">
<form id="Form1" runat="server">
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <div>
            <radCln:RadCalendar id="calDate" runat="server" 
                AutoPostBack="false"
                TitleFormat="MMMM yyyy"
                RangeMinDate="0001-01-01"
                EnableMultiSelect="false">
                <ClientEvents OnDateClick="Calendar_OnDateClick" />
            </radCln:RadCalendar>
                 
            <asp:CheckBox ToolTip="Remove Date" ID="chkRemoveDate" runat="server" Text="Remove date" />
        </div>
        <asp:HiddenField ID="hdnOrigDate" runat="server" />
        <asp:HiddenField ID="hdnSelectedDate" runat="server" />
    </div> 
    <ek:FieldDialogButtons ID="FieldDialogButtons" OnOK="return insertField();" runat="server" />
</form> 
<script language="javascript" type="text/javascript">
<!--
    Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
    
        var m_oFieldElem = null;
        var dNowXml = Ektron.Xml.serializeDate(new Date());
        
        function initField()
	    {
	        m_oFieldElem = null;
	        var oFieldElem = null;
	        var datevalue = null;
	        var dateTriplets = new Array;
	        var args = GetDialogArguments();
	        if (args)
	        {
	            oFieldElem = args.selectedField;
	            datevalue = oFieldElem.getAttribute("datavalue");
                datevalue = (null == datevalue ? oFieldElem.getAttribute("value") : datevalue);
	            datevalue = (null == datevalue ? dNowXml : datevalue);
	            datevalue = ("" == datevalue ? dNowXml : datevalue);
	            m_oFieldElem = oFieldElem;
	        }
	        else
		    {
		        datevalue = dNowXml;
		    }
		    if (datevalue != null && datevalue != "")
		    {
		        document.getElementById("<%=hdnOrigDate.ClientID%>").value = datevalue;
		        dateTriplets = [datevalue.substr(0,4), parseInt(datevalue.substr(5,2), 10), parseInt(datevalue.substr(8), 10)];
		        var calendar = null;
		        if (typeof <%= calDate.ClientID %> != "undefined")
		        {
		            calendar = <%= calDate.ClientID %>; 
		            calendar.SelectDate(dateTriplets, true); 
		        }
		    }
	    }
        function insertField()
        {
            var oFieldElem = m_oFieldElem;
            var oInputElem = $ektron("input", oFieldElem).get(0);
            if (true == document.getElementById("<%=chkRemoveDate.ClientID %>").checked)
            {
                // the date field
                oInputElem.value = "";
                oInputElem.setAttribute("value", "");
                oInputElem.removeAttribute("value");
                // need to remove the attribute instead of setting it to "" b/c
                // it cannot be set as value="", instead it will be set as value="value"
                oFieldElem.removeAttribute("value");
                oFieldElem.removeAttribute("datavalue");
            }
            else
            {
                var dSelectedXML = (document.getElementById("<%=hdnSelectedDate.ClientID%>").value || dNowXml);
                var dSelected = Ektron.Xml.parseDate(dSelectedXML);
                if (dSelected != null)
                {
					oFieldElem.setAttribute("value", dSelectedXML);
					oFieldElem.setAttribute("datavalue", dSelectedXML);
					// Need to explicitly set the 'value' attribute to the value of the 'value' property.
					// Need to set the 'value' property so Mozilla updates the display.
					oInputElem.value = dSelected.toLocaleDateString();
					oInputElem.setAttribute("value", oInputElem.value);
                }
            }
            // need to fire "onblur" to validate the content?
            CloseDlg(oFieldElem);	
        }
    //-->
</script>
</body> 
</html> 

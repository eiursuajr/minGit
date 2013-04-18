<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CreateReport.ascx.cs"
    Inherits="Workarea_Widgets_CreateReport" %>
<%@ Register Assembly="Ektron.Cms.Framework.UI.Controls.EktronUI" Namespace="Ektron.Cms.Framework.UI.Controls.EktronUI"
    TagPrefix="ektronUI" %>
<%@ Register TagPrefix="Loc" TagName="ReportGrid" Src="../../../controls/dashboard/ReportGrid.ascx" %>
<%@ Register TagPrefix="Ek" TagName="DatePicker" Src="../../../controls/generic/date/DatePicker.ascx" %>
<%@ Register TagPrefix="Ek" TagName="DateRangePicker" Src="../../../controls/generic/date/DateRangePicker.ascx" %>

<asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
    <asp:View ID="View" runat="server">
<script language="javascript" type="text/javascript">
    function <%= ReportGrid1.ClientID %>_getSearchTerms(id) {
        var tb = document.getElementById(id);
        if (tb != null && tb.value != 'Enter a Title')
            return tb.value;
        return '';
    }
    function <%= ReportGrid1.ClientID %>_DoSearch(value) {
        $ektron('#<%= txtContentTitle.ClientID %>').val(value);
        <%= ReportGrid1.ClientID %>_SetFilter('title', value, false);
        <%= ReportGrid1.ClientID %>_Filter(document.getElementById('<%= ReportGrid1.ClientID %>_newfilters').value);
        return false;
    }
    function <%= ReportGrid1.ClientID %>_blurSearchBox(box, blur) {
        if (box == null)
            return;
        if (!blur && box.value == 'Enter a Title')
            box.value = '';
        else if (blur && box.value == '')
            box.value = 'Enter a Title';
    }
    function <%= ReportGrid1.ClientID %>_setSearchBoxes(value) {
        if (value == null || value == '')
            value = 'Enter a Title';
        document.getElementById('<%= ReportGrid1.ClientID %>_searchtext1').value = value;
    }
    function <%= ReportGrid1.ClientID %>_SearchFilterCallback(filter, value) {
        if (filter == 'title')
            <%= ReportGrid1.ClientID %>_setSearchBoxes(value);
    }
    function SetFilters() {
        var e_createdstart = $ektron('#<%= this.ClientID %>_datecreated .StartCreated input');
        var e_createdend = $ektron('#<%= this.ClientID %>_datecreated .EndCreated input');
        var e_modifiedstart = $ektron('#<%= this.ClientID %>_datemodified .StartModified input');
        var e_modifiedend = $ektron('#<%= this.ClientID %>_datemodified .EndModified input');
        var e_locale = $ektron('#<%= ddlLocale.ClientID %>');
        var e_localenotin = $ektron('#<%= chkLocaleNotIn.ClientID %>:checked');
        var e_status = $ektron('#<%= ddlStatus.ClientID %>');
        var e_contentid = $ektron('#<%= txtContentID.ClientID %>');
        var e_author = $ektron('#<%= ddlAuthor.ClientID %>');
        var e_title = $ektron('#<%= txtContentTitle.ClientID %>');
        var e_folder = $ektron('#<%= ddlFolderID.ClientID %>');
      
        // Set internal filter values
        <%= ReportGrid1.ClientID %>_SetFilter('datecreatedstart', e_createdstart.val(), false);
        <%= ReportGrid1.ClientID %>_SetFilter('datecreatedend', e_createdend.val(), false);
        <%= ReportGrid1.ClientID %>_SetFilter('locale', e_locale.val(), false);
        <%= ReportGrid1.ClientID %>_SetFilter('localenotin', (e_localenotin.val() == null ? 'f' : 't'), false);
        <%= ReportGrid1.ClientID %>_SetFilter('locstatus', e_status.val() || 'Undefined', false);
        <%= ReportGrid1.ClientID %>_SetFilter('lastmodifiedstart', e_modifiedstart.val(), false);
        <%= ReportGrid1.ClientID %>_SetFilter('lastmodifiedend', e_modifiedend.val(), false);
        <%= ReportGrid1.ClientID %>_SetFilter('contentid', CleanString(e_contentid.val(), 'Enter an ID'), false);
        <%= ReportGrid1.ClientID %>_SetFilter('authorid', e_author.val(), false);
        <%= ReportGrid1.ClientID %>_SetFilter('title', CleanString(e_title.val(), 'Enter a Title'), false);
        <%= ReportGrid1.ClientID %>_SetFilter('folderid', e_folder.val(), false);
    }
    function GetFilterString() {
        return document.getElementById('<%= ReportGrid1.ClientID %>_newfilters').value;
    }
    function DoFilter() {
        SetFilters();

        <%= ReportGrid1.ClientID %>_Filter(GetFilterString());
        return false;
    }
    function DoPrint() {
        SetFilters();
        
        window.open('<%= Page.ResolveUrl("../../controls/dashboard/report.aspx") %>?out=print&<%= ReportGrid1.IncludeFolderPath ? "fp=1&" : string.Empty %>f=' + escape(GetFilterString()),
            '_blank');
        return false;
    }
    function DoExcel() {
        SetFilters();
        
        window.open('<%= Page.ResolveUrl("../../controls/dashboard/report.aspx") %>?out=xls&f=' + escape(GetFilterString()),
            '_blank');
        return false;
    }
    function CleanString(str, defaultval) {
        if (str == defaultval)
            return '';
        return str || '';
    }
    var <%= ReportGrid1.ClientID %>_busyCount = 0;
    function <%= ReportGrid1.ClientID %>_ShowBusy(busyFlag) {
        if (busyFlag)
            <%= ReportGrid1.ClientID %>_busyCount++;
        else
            <%= ReportGrid1.ClientID %>_busyCount--;
            
        if (<%= ReportGrid1.ClientID %>_busyCount < 0)
            <%= ReportGrid1.ClientID %>_busyCount = 0;
        if (<%= ReportGrid1.ClientID %>_busyCount == 1) {
            var busy = '<span style="color:red;">Please wait...</span>';
            document.getElementById('<%= ReportGrid1.ClientID %>_busy1').innerHTML = busy;
        } else if (<%= ReportGrid1.ClientID %>_busyCount == 0) {
            document.getElementById('<%= ReportGrid1.ClientID %>_busy1').innerHTML = '';
        }
    }
    function <%= ReportGrid1.ClientID %>_UpdateSelections(cid, selected) {
        var sel1 = document.getElementById('<%= ReportGrid1.ClientID %>_selected1');
        var selval = '';
        if (cid != null) {
            var val = document.getElementById('<%= ReportGrid1.ClientID %>_selected').value;
            var count = val == '' ? 0 : val.split(',').length;
            selval = (count == 0 ? 'No' : count) + ' item' + (count == 1 ? '' : 's') + ' selected';
        }
        sel1.innerHTML = selval;
    }

    function ClearDate(parentId) {
        var el = $ektron('#' + parentId + '_DatePicker_uxDatePicker');
        el.find('input').datepicker("setDate", null);
        el.find('input').val("");
        el.find('label').fadeTo(100, 1);
        var dps = el.parent().parent().parent().find(".hasDatepicker");
        dps.datepicker("option", "minDate", null).datepicker("option", "maxDate", null)
    }

    function SelectDate(parentId) {
        $ektron('#' + parentId + '_DatePicker_uxDatePicker input').datepicker("show");
    }

    Ektron.ready(function() {
        var startcreatedClientId = '#<%=StartCreated.ClientID %>_DatePicker_uxDatePicker input';
        var endcreatedClientId = '#<%=EndCreated.ClientID %>_DatePicker_uxDatePicker input';
        var startmodifiedClientId = '#<%=StartModified.ClientID %>_DatePicker_uxDatePicker input';
        var endmodifiedClientId = '#<%=EndModified.ClientID %>_DatePicker_uxDatePicker input';

        $ektron(startcreatedClientId).change(function(){
            var d = $ektron(startcreatedClientId).datepicker("getDate");
            $ektron(endcreatedClientId).datepicker("option", "minDate", d);
        });
        $ektron(endcreatedClientId).change(function(){
            var d = $ektron(endcreatedClientId).datepicker("getDate");
            $ektron(startcreatedClientId).datepicker("option", "maxDate", d);
        });
        $ektron(startmodifiedClientId).change(function(){
            var d = $ektron(startmodifiedClientId).datepicker("getDate");
            $ektron(endmodifiedClientId).datepicker("option", "minDate", d);
        });
        $ektron(endmodifiedClientId).change(function(){
            var d = $ektron(endmodifiedClientId).datepicker("getDate");
            $ektron(startmodifiedClientId).datepicker("option", "maxDate", d);
        });
    });
</script>
<style type="text/css">
.DatePicker_Label { font-size: 0.8em; }
.DatePicker_input { font-size: 0.9em; }
.FolderPath { font-size: 0.8em; }
input.hasDatepicker { width:155px; }
.ui-datepicker-trigger { cursor:pointer; }
</style>
<%--three column row begins--%>
<div class="threeColumn">
    <%--column 1 begins--%>
    <div class="column divider">
        <div class="paddingLeft">
             <div class="topMargin">
                <asp:Label ID="Label16" AssociatedControlID="ddlLocale" runat="server"><%=this.GetMessage("lbl Lang/Dialect")%>: </asp:Label>
                <%--dropdown begins--%>
                 <asp:DropDownList ID="ddlLocale" runat="server">
                </asp:DropDownList> <asp:CheckBox runat="server" ID="chkLocaleNotIn" Text="Does not include" />
                <%--dropdown ends--%>
            </div>
            <div class="topMargin">
                <asp:Label ID="Label17" AssociatedControlID="ddlStatus" runat="server"><%=this.GetMessage("lbl Trans Status")%>:</asp:Label>
                <%--dropdown begins--%>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Value="">(Any)</asp:ListItem>
                </asp:DropDownList><%--dropdown ends--%>
            </div>
        </div>
    </div>
    <%--column 1 ends--%>
    <%--column 2 begins--%>
    <div class="column divider">
        <div class="paddingLeft">         
            <div class="topMargin">
               <asp:Label ID="Label15" runat="server"><%=this.GetMessage("content dc label")%></asp:Label>
                <%--the following div is a temp div that needs to be removed--%>
                <div id="<%= this.ClientID %>_datecreated">
                    <div>
                        <div class="dateLabel"><%=this.GetMessage("generic start date label")%></div>
                        <span class="dateClass ektronTextSmall">
                            <ektronUI:DatePicker ID="StartCreated" runat="server" CssClass="StartCreated" 
                                ButtonImage="../../images/ui/icons/calendar.png" ShowOn="Both" ButtonImageOnly="true" >
                            </ektronUI:DatePicker>
                            <img onclick="ClearDate('<%=StartCreated.ClientID %>');" 
                                style="cursor:pointer;" alt="Delete the Date" title="Delete the Date" src="../../images/ui/icons/calendarDelete.png" />
                        </span>
                    </div>
                    <div>
					    <div class="dateLabel"><%=this.GetMessage("generic end date label")%></div>
                        <span class="dateClass ektronTextSmall">
                            <ektronUI:DatePicker ID="EndCreated" runat="server" CssClass="EndCreated" ButtonImage="../../images/ui/icons/calendar.png" ShowOn="Both" ButtonImageOnly="true">
                            </ektronUI:DatePicker>
                            <img onclick="ClearDate('<%=EndCreated.ClientID %>');" 
                                style="cursor:pointer;" alt="Delete the Date" title="Delete the Date" src="../../images/ui/icons/calendarDelete.png" />
                        </span>
                    </div>
                </div>
            </div>
            <div class="topMargin">
                <asp:Label ID="Label19" runat="server"><%=this.GetMessage("lbl Last Modified")%>:</asp:Label>
                <%--the following div is a temp div that needs to be removed--%>
                <div id="<%= this.ClientID %>_datemodified">
                    <div>
                        <div class="dateLabel"><%=this.GetMessage("generic start date label")%></div>
                        <span class="dateClass ektronTextSmall">
                            <ektronUI:DatePicker ID="StartModified" runat="server" CssClass="StartModified" ButtonImage="../../images/ui/icons/calendar.png" ShowOn="Both" ButtonImageOnly="true">
                            </ektronUI:DatePicker>
                            <img onclick="ClearDate('<%=StartModified.ClientID %>');" 
                                style="cursor:pointer;" alt="Delete the Date" title="Delete the Date" src="../../images/ui/icons/calendarDelete.png" />
                        </span>
                    </div>
                    <div>
					    <div class="dateLabel"><%=this.GetMessage("generic end date label")%></div>
                        <span class="dateClass ektronTextSmall">
                            <ektronUI:DatePicker ID="EndModified" runat="server" CssClass="EndModified" ButtonImage="../../images/ui/icons/calendar.png" ShowOn="Both" ButtonImageOnly="true">
                            </ektronUI:DatePicker>
                            <img onclick="ClearDate('<%=EndModified.ClientID %>');" 
                                style="cursor:pointer;" alt="Delete the Date" title="Delete the Date" src="../../images/ui/icons/calendarDelete.png" />
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%--column 2 ends--%>
    <%--column 3 begins--%>
    <div class="column">
        <div class="paddingLeft">
            <div class="topMargin">
                <asp:Label ID="Label20" AssociatedControlID="txtContentID" runat="server"><%=this.GetMessage("content id label")%></asp:Label>
                <asp:TextBox CssClass="smallTextBox" ID="txtContentID" runat="server" Text="Enter an ID" onClick="this.select();"></asp:TextBox>
            </div>
            <div class="topMargin">
                <asp:Label ID="Label1" AssociatedControlID="ddlFolderID" runat="server"><%=this.GetMessage("lbl folder")%>:</asp:Label>
                <asp:DropDownList ID="ddlFolderID" runat="server">
                </asp:DropDownList>
            </div>
            <div class="topMargin">
                <asp:Label ID="Label21" AssociatedControlID="ddlAuthor" runat="server"><%=this.GetMessage("generic author")%>:</asp:Label>
                <%--dropdown begins--%>
                <asp:DropDownList ID="ddlAuthor" runat="server">
                </asp:DropDownList><%--dropdown ends--%>
            </div>
            <div class="topMargin">
                <asp:Label ID="Label22" AssociatedControlID="txtContentTitle" runat="server"><%=this.GetMessage("generic title")%>:</asp:Label>
                <asp:TextBox CssClass="smallTextBox" ID="txtContentTitle" runat="server" Text="Enter a Title" onClick="this.select();"></asp:TextBox>
            </div>
        </div>
    </div>
    <%--column 3 ends--%>
</div>
<%--three column row ends--%>
<div style="clear:both;"></div>
<div style="text-align: center; width: 100%; display: block;" aclass="center"><center>
    <asp:Button runat="server" ID="btnFilter" CssClass="goBtn" Text="Filter Results" OnClientClick="return DoFilter();" />
    <asp:Button runat="server" ID="btnPrint" CssClass="goBtn" Text="Print" OnClientClick="return DoPrint();" />
    <asp:Button runat="server" ID="btnCreate" CssClass="goBtn" Text="Create Report" OnClientClick="return DoExcel();" />
    </center>
</div>
<span id="<%= ReportGrid1.ClientID %>_busy1"></span>
<div style="clear:both;"></div>


<%--two column row begins--%>
<div class="twoColumn" style="display:none;">
    <%--search section begins--%>
    <div class="column divider">
        <div class="paddingLeft">
            <input type="text" class="textbox" id="<%= ReportGrid1.ClientID %>_searchtext1" value="Enter a Title" onblur="<%= ReportGrid1.ClientID %>_blurSearchBox(this, true);" onfocus="<%= ReportGrid1.ClientID %>_blurSearchBox(this, false);" />
            <input type="button" class="goBtn" value="Search" onclick="return <%= ReportGrid1.ClientID %>_DoSearch( <%= ReportGrid1.ClientID %>_getSearchTerms('<%= ReportGrid1.ClientID %>_searchtext1'));" />
            <span id="<%= ReportGrid1.ClientID %>_selected1"></span>
        </div>
    </div>
    <%--search section begins--%>
    <%--print and create report buttons begin--%>
    <div class="column">
        <div class="paddingLeft">
            <asp:Button CssClass="goBtn" ID="Button1" runat="server" Text="Print" />
            <asp:Button CssClass="goBtn" ID="Button2" runat="server" Text="Create Report" /></div>
    </div>
    <%--print and create report buttons end--%>
</div>
<%--two column row ends--%>

<Loc:ReportGrid runat="server" ID="ReportGrid1" ClientSetFilterCallback="SearchFilterCallback" ClientShowBusyDuringCallback="ShowBusy" ClientSelectionChanged="UpdateSelections" UseClientIDPrefix="true" IncludeFolderPath="true" />

    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>_edit">
            <table>
                <tr>
                    <td><%=this.GetMessage("generic title")%></td>
                    <td><asp:TextBox ID="txtTitle" runat="server">Dashboard</asp:TextBox></td>
                </tr>
            </table>
            <br />
              <!-- End To Do ..............................  -->
             <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        </div>
    </asp:View>
    
</asp:MultiView>

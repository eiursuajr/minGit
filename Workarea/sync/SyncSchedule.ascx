<%@ Control Language="C#" EnableViewState="true"  AutoEventWireup="true" CodeFile="SyncSchedule.ascx.cs" Inherits="SyncSchedule" %>
<table>
    <tr>
        <td class="scheduleOptions">
            <asp:RadioButtonList ToolTip="Select Schedule" ID="rdoSchedule" runat="server">
                <asp:ListItem Value="None"></asp:ListItem>
                <asp:ListItem Value="OneTime"></asp:ListItem>
                <asp:ListItem Value="Hourly"></asp:ListItem>
                <asp:ListItem Value="Daily"></asp:ListItem>
                <asp:ListItem Value="Weekly"></asp:ListItem>
                <asp:ListItem Value="Monthly"></asp:ListItem>
            </asp:RadioButtonList>
        </td>
        <td>
            <fieldset id="scheduleFieldset" class="displayNone">
                <div id="divOneTimeSchedule" class="displayNone">
                    <asp:Label CssClass="optionHeader" ID="lblOneTimeSchedule" runat="server"></asp:Label>
                    <div class="description">
                        <asp:Label ID="lblOneTimeDescription" runat="server"></asp:Label>
                    </div>
                    <asp:Literal ID="ltrOneTimeCalendar" runat="server"></asp:Literal>
                </div>
                <div id="divHourlySchedule" class="displayNone">
                    <asp:Label CssClass="optionHeader" ID="lblHourlySchedule" runat="server"></asp:Label>
                    <div class="description">
                        <asp:Label ID="lblHourlyDescription" runat="server"></asp:Label>
                    </div>
                    <asp:DropDownList ToolTip="Select the minute of the hour on which the synchronization activity should recur from the drop down menu" ID="ddlHourlyMinute" runat="server"></asp:DropDownList>
                </div>
                <div id="divDailySchedule" class="displayNone">
                    <asp:Label CssClass="optionHeader" ID="lblDailySchedule" runat="server"></asp:Label>
                    <div class="description">
                        <asp:Label ID="lblDailyDescription" runat="server"></asp:Label>
                    </div>
                    <asp:DropDownList ToolTip="Select the Hour of day at which the synchronization activity should recur from the drop down menu" ID="ddlDailyHour" runat="server"></asp:DropDownList>&nbsp;:
                    <asp:DropDownList ToolTip="Select the Minute of day at which the synchronization activity should recur from the drop down menu" ID="ddlDailyMinute" runat="server"></asp:DropDownList>&nbsp;
                    <asp:DropDownList ToolTip="Select AM/PM at which the synchronization activity should recur from the drop down menu" ID="ddlDailyAMPM" runat="server"></asp:DropDownList>
                </div>
                <div id="divWeeklySchedule" class="displayNone">
                    <div>
                        <asp:Label CssClass="optionHeader" ID="lblWeeklySchedule" runat="server"></asp:Label>
                        <div class="description">
                            <asp:Label ID="lblWeeklyDayDescription" runat="server"></asp:Label>
                        </div>
                        <asp:DropDownList ToolTip="Select the day of the week on which synchronization activity should recur from the drop down menu" id="ddlWeeklyDay" runat="server"></asp:DropDownList>
                        </div>
                    <div>
                        <asp:Label CssClass="optionHeader" ID="lblWeeklySchedule2" runat="server"></asp:Label>
                        <div class="description">
                            <asp:Label ID="lblWeeklyTimeDescription" runat="server"></asp:Label>
                        </div>
                        <asp:DropDownList ToolTip="Select the Hour of day at which the synchronization activity should recur from the drop down menu" ID="ddlWeeklyHour" runat="server"></asp:DropDownList>&nbsp;:
                        <asp:DropDownList ToolTip="Select the Minute of day at which the synchronization activity should recur from the drop down menu" ID="ddlWeeklyMinute" runat="server"></asp:DropDownList>&nbsp;
                        <asp:DropDownList ToolTip="Select AM/PM at which the synchronization activity should recur from the drop down menu" ID="ddlWeeklyAMPM" runat="server"></asp:DropDownList>
                    </div>
                </div>
                <div id="divMonthlySchedule" class="displayNone">
                    <div>
                        <asp:Label CssClass="optionHeader" ID="lblMonthlySchedule" runat="server"></asp:Label>
                        <div class="description">
                            <asp:Label ID="lblMonthlyDayDescription" runat="server"></asp:Label>
                        </div>
                        <asp:DropDownList ToolTip="Select the day of the month on which the synchronization activity should recur from the drop down menu" id="ddlMonthlyDay" runat="server"></asp:DropDownList>
                    </div>
                    <div>
                        <asp:Label CssClass="optionHeader" ID="lblMonthlySchedule2" runat="server"></asp:Label>
                        <div class="description">
                            <asp:Label ID="lblMonthlyTimeDescription" runat="server"></asp:Label>
                        </div>
                        <asp:DropDownList ToolTip="Select the Hour of day at which the synchronization activity should recur from the drop down menu" ID="ddlMonthlyHour" runat="server"></asp:DropDownList>&nbsp;:
                        <asp:DropDownList ToolTip="Select the Minute of day at which the synchronization activity should recur from the drop down menu" ID="ddlMonthlyMinute" runat="server"></asp:DropDownList>&nbsp;
                        <asp:DropDownList ToolTip="Select AM/PM at which the synchronization activity should recur from the drop down menu" ID="ddlMonthlyAMPM" runat="server"></asp:DropDownList>
                    </div>
                </div>
            </fieldset>
        </td>
    </tr>
</table>
<input id="syncScheduleElementPrefix" type="hidden" runat="server" />
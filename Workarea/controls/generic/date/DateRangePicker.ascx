<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateRangePicker.ascx.cs" Inherits="Ektron.Cms.Common.DateRangePicker" %>
<%@ Register TagPrefix="ektron" TagName="DatePicker" Src="DatePicker.ascx" %>
<span id="DateRangePickerContainer" class="DateRangePickerContainer" runat="server">
	<ektron:DatePicker id="StartDatePicker" CssClass="DateRangePicker_StartDate" runat="server" />
	<ektron:DatePicker id="EndDatePicker" CssClass="DateRangePicker_EndDate" runat="server" />
	<asp:ImageButton ToolTip="Refresh" ID="btnRefresh" CssClass="RefreshButton" OnClick="btnRefresh_OnClick" runat="server" />
	<asp:CompareValidator ID="CompareDatesValidator" ControlToValidate="StartDatePicker" Operator="LessThanEqual" ControlToCompare="EndDatePicker" Type="Date" CssClass="DateRangePickerValidator" Display="Dynamic" EnableClientScript="false" runat="server" />
</span>

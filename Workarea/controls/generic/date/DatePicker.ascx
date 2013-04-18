<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DatePicker.ascx.cs" Inherits="Ektron.Cms.Common.DatePicker" %>
<span id="DatePickerContainer" class="DatePickerContainer" runat="server">
	<asp:Label ToolTip="Date" ID="lblDatePicker" CssClass="DatePicker_Label" runat="server" EnableViewState="false" />
	<asp:TextBox ToolTip="Date" CssClass="DatePicker_input" ID="tbDate" runat="server" />
</span>

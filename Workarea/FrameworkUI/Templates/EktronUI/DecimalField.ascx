<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DecimalField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.DecimalField" %>
<span id="uxTextField" runat="server" class="ektron-ui-control ektron-ui-input ektron-ui-decimalField">
	<asp:TextBox ID="aspInput" runat="server" autocomplete="off" />
</span>
<ektronUI:JavaScriptBlock ID="uxJavaScriptBlock" ClientIDMode="AutoID" ExecutionMode="OnEktronReady" runat="server">
	<ScriptTemplate>
		$ektron.global.preferCulture("<%= currentCulture %>");
		$ektron("#<%=  aspInput.ClientID %>").spinner(
		{
			incremental: <%= this.ControlContainer.Incremental.ToString().ToLower() %>,
			numberFormat: "n<%= this.ControlContainer.DecimalPlaces %>",
			max: <%= Convert.ToDecimal(this.ControlContainer.MaxValue) %>,
			min: <%= Convert.ToDecimal(this.ControlContainer.MinValue) %>,
			page: <%= this.ControlContainer.PageCount %>,
			step: <%= this.ControlContainer.Step %>
		})
		.attr("data-ektron-initialvalue", "<%= this.ControlContainer.Value.ToString()%>")
		.bind("blur", function()
		{
			var field = $ektron(this);
			if (field.val() == "")
			{
				field.val(field.attr("data-ektron-initialvalue"));
			}
		});
	</ScriptTemplate>
</ektronUI:JavaScriptBlock>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IntegerField.ascx.cs" Inherits="Ektron.Cms.Framework.UI.Controls.EktronUI.Templates.IntegerField" %>
<span id="uxTextField" runat="server" class="ektron-ui-control ektron-ui-input ektron-ui-integerField">
	<asp:TextBox ID="aspInput" runat="server" />
</span>
<ektronUI:JavaScriptBlock ID="uxJavaScriptBlock" ClientIDMode="AutoID" ExecutionMode="OnEktronReady" runat="server">
	<ScriptTemplate>
		$ektron.global.preferCulture("<%= currentCulture %>");
		$ektron("#<%=  aspInput.ClientID %>").spinner(
		{
			incremental: <%= this.ControlContainer.Incremental.ToString().ToLower() %>,
			numberFormat: "n0",
			max: <%= this.ControlContainer.MaxValue %>,
			min: <%= this.ControlContainer.MinValue %>,
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

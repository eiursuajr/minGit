<%@ Control CodeBehind="DialogLoaderJavascript.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="Ektron.Telerik.WebControls.Dialogs.DialogLoaderJavascript" %>
<input type="hidden" runat="server" id="hiddenDialogParams"/>
<input type="hidden" runat="server" id="hiddenCommonDialogParams"/>
<asp:literal id="literalScripts" runat="server" />
<asp:placeholder id="placeholderDialog" runat="server"></asp:placeholder>
<asp:literal runat="server" id="literalSelfReloadScript">
	<script type="text/javascript">
		if (window.attachEvent)
		{
			window.attachEvent("onload", SubmitParameters);
		}
		else if (window.addEventListener)
		{
			window.addEventListener("load", SubmitParameters, false);
		}
		
		function SubmitParameters()
		{
			var dialogArguments = GetDialogArguments();
			if (dialogArguments)
			{
				var internalParameters = dialogArguments.InternalParameters;
				if (internalParameters)
				{
					hiddenDialogParamsClientElement.value = internalParameters;
				}

				var commonDialogParameters = dialogArguments.CommonInternalParameters;
				if (commonDialogParameters)
				{
					hiddenCommonDialogParamsClientElement.value = commonDialogParameters;
				}

				document.forms[0].submit();
			}
		}
		
	</script>
</asp:literal>
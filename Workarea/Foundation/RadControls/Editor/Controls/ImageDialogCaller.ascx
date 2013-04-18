<%@ Control Language="c#" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" CodeBehind="ImageDialogCaller.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.ImageDialogCaller" %>

<nobr><input tabindex="<%=this.TabIndex%>" type="text" class="Ektron_StandardTextBox" id="<%=this.ClientID%>_resultTextBox">&nbsp;
<button id='<%=this.ClientID%>_dialogOpenerButton' onclick=" return <%=this.ClientID%>.CallImageDialog();" onfocus="this.blur();">
&nbsp;...&nbsp;
</button></nobr>
<asp:literal id="ltScriptHolder" runat="server" />
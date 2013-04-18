<%@ Control Language="c#" AutoEventWireup="false" Codebehind="StyleBuilderCaller.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.StyleBuilderCaller" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<span class="Label">
<script>localization.showText('BuildStyle');</script>
<input type="hidden" id="<%=this.ClientID%>_styleTextHolder"/>
<input type="button" title="Style Builder" id="<%=this.ClientID%>_showStyleBuilder" style="margin-bottom:1px;height:20px;border:1px solid #999999;" onclick="javascript:<%=this.ClientID%>.ShowDialog();" onfocus="this.blur();" value=" ... "/>
</span>
<script>
	var <%=this.ClientID%> = new StyleBuilderCaller('<%=this.ClientID%>');
</script>
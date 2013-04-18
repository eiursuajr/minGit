<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TargetedContent.ascx.cs" Inherits="Ektron.Widgets.TargetedContentWidget" %>
<%@ Register Src="~/widgets/controls/ColumnDisplay.ascx" TagName="ColumnDisplay" TagPrefix="UX" %>
<%@ Register Src="~/widgets/TargetedContent/RuleEditor/RuleEditor.ascx" TagName="RuleEditor" TagPrefix="UX" %>
<%@ Register Src="~/widgets/TargetedContent/Controls/TargetContentList.ascx" TagName="TargetContentList" TagPrefix="UX" %>

<div id="wrapper" runat="server" class="targetedContent">
	<asp:MultiView ID="TargetedContentViewSet" runat="server">
		<asp:View ID="View" runat="server">
			<UX:ColumnDisplay ID="ActiveColumn" runat="server" />
		</asp:View>
		<asp:View ID="PageEditing" runat="server">
			<asp:HiddenField ID="hdnSelectedZone" runat="server" Value="0" OnValueChanged="SelectedZoneChanged" />
			<asp:HiddenField ID="hdnZoneOrder" runat="server" Value="" OnValueChanged="ZoneOrderChanged" />
			
			<div style="width: 850px;">
			    <a title="select global configuration" style="float:left; text-decoration:underline; color:Blue;" class="globalConfigSelect" id="aSelectGlobalConfig" runat="server" ><%=m_refMsg.GetMessage("lbl Select Global Configuration")%></a>
			</div>
			
			<UX:ColumnDisplay ID="ConditionalZones" runat="server" />
			
			<div class="ui-widget">
				<asp:LinkButton ID="btnAddConditionalZone" style="float:left;" CssClass="targetedContent-addZone ektron ek-button ek-button-icon-left ui-corner-all ui-state-default" Text="Add" EnableViewState="false" runat="server" OnClick="btnAddConditionalZone_Click" />
				<asp:LinkButton ID="btnEditConditionalZone" style="float:left;" CssClass="targetedContent-editZone ektron ek-button ui-corner-all ui-state-default" Text="Edit" EnableViewState="false" runat="server" OnClick="btnEditConditionalZone_Click" />
				<asp:Button ID="btnChangeZoneOrder" CssClass="targetedContent-changeZoneOrder" Text="hidden" EnableViewState="false" runat="server" />
			</div>
			

            <UX:TargetContentList ID="ucTargetContentList" runat="server" OnTargetContentSelected="ucTargetContentList_TargetContentSelected" />
			
		</asp:View>
		<asp:View ID="PageEditingGlobalConfig" runat="server">
		    <div class="ektronTargetContentGlobalView">
		    <asp:Label ID="lblContentConfiguration" runat="server" class="ektronGlobalContentTitle" AssociatedControlID="ucSpanGlobalConfigTitle">Targeted Content:</asp:Label>
			    <span id="ucSpanGlobalConfigTitle" class="ektrontargetConfigTitle" runat="server"></span>
			    <asp:LinkButton CssClass="remColumn" ID="btnDeleteConfigurationColumn" OnClick="btnDeleteGlobalTargetContent_Click" runat="server">
                    <img alt="" id="imgRemove" runat="server" class="PBclosebutton PB-UI-icon" src="#" runat="server" />
                </asp:LinkButton>
			</div>
		</asp:View>
		<asp:View ID="Edit" runat="server">
			<div class="targetedContent-rule ui-widget">
			    <input type="hidden" runat="server" ID="hdnSavedSetId" class="SavedSetId" />
			
				<asp:Label ID="lblConditionName" AssociatedControlID="tbRulesetName" runat="server" EnableViewState="false" />
				<asp:TextBox ID="tbRulesetName" runat="server" />
				<div class="targetedContent-rule-editor">
					<UX:RuleEditor ID="ruleEditor" runat="server" />
				</div>
			</div>
			<div class="WidgetEditControls ui-widget">
				<asp:Button ID="CancelButton" CssClass="WidgetCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
				<asp:Button ID="SaveButton" CssClass="WidgetSave" runat="server" Text="Save" OnClick="SaveButton_Click" />
			</div>
		</asp:View>
		<asp:View ID="NonPageBuilderView" runat="server">
		    <div class="invalidPage">
                <span><%=m_refMsg.GetMessage("alert targetwidget")%></span>
			</div>
		</asp:View>
	</asp:MultiView>

</div>
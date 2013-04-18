<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TablePropertiesControl.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.TablePropertiesControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="ColorPicker" Src="./ColorPicker.ascx"%>
<%@ Register TagPrefix="telerik" TagName="AlignmentSelector" Src="./AlignmentSelector.ascx"%>
<%@ Register TagPrefix="telerik" TagName="TableBorderControl" Src="./TableBorderControl.ascx"%>
<%@ Register TagPrefix="telerik" TagName="ImageDialogCaller" Src="./ImageDialogCaller.ascx"%>
<%@ Register TagPrefix="telerik" TagName="CssClassSelector" Src="./CssClassSelector.ascx"%>
<%@ Register TagPrefix="telerik" TagName="SpinBox" Src="./SpinBox.ascx"%>
<%@ Register TagPrefix="telerik" TagName="StyleBuilderCaller" Src="./StyleBuilderCaller.ascx"%>
<table border="0" cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td valign="top">
			
						<fieldset>
							<legend>
								<script>localization.showText('Dimensions');</script>
							</legend>
							<table width="100%">
								<tr>
									<td class="Label">
										<script>localization.showText('Width');</script>
									</td>
									<td>
										<input tabindex="1" class="Text" id="<%=this.ClientID%>_tableWidth" style="WIDTH: 50px" type="text" maxlength="6">
									</td>
									<td class="Label" nowrap>px, %</td>
								</tr>
								<tr>
									<td class="Label">
										<script>localization.showText('Height');</script>
									</td>
									<td>
										<input tabindex="2" class="Text" id="<%=this.ClientID%>_tableHeight" style="WIDTH: 50px" type="text" maxlength="6">
									</td>
									<td class="Label" nowrap>px, %</td>
								</tr>
							</table>
						</fieldset>
					
						<fieldset>
							<legend>
								<script>localization.showText('Layout');</script>
							</legend>
							<table height="68" cellspacing="1" cellpadding="3" width="100%">
								<tr>
									<td class="Label" valign="center" width="50%">
										<table>
											<tr>

												<td class="Label" width="50%" nowrap><script>localization.showText('Background');</script>:</td>
												<td><telerik:colorpicker id="BgColorPicker" runat="server"></telerik:colorpicker></td>
											</tr>
											<tr>
												<td class="Label" width="50%" nowrap><script>localization.showText('Alignment');</script></td>
												<td><telerik:alignmentselector id="TableAlignmentSelector" runat="server" mode="TABLE"></telerik:alignmentselector></td>
											</tr>
											<tr>
												<td class="Label" width="50%" nowrap>
													<script>localization.showText('CellSpacing');</script>
												</td>
												<td>
													<telerik:SpinBox tabindex="3" id="cellSpacingSpinBox" runat="server"/>
												</td>
											</tr>
											<tr>
												<td class="Label" width="50%" nowrap>
													<script>localization.showText('CellPadding');</script>
												</td>
												<td>
													<telerik:SpinBox tabindex="4" id="cellPaddingSpinBox" runat="server"/>
												</td>
											</tr>
											<tr>
												<td class="Label" width="50%" nowrap><script>localization.showText('Id');</script></td>
												<td><input tabindex="5" class="Text" type="text" id="<%=this.ClientID%>_idHolder"/></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</fieldset>					
		</td>
		<td valign="top" width="100%">
			<fieldset><legend>
				<script>localization.showText('Border');</script>
				</legend>
				<telerik:tablebordercontrol id="theTableBorderControl" runat="server"></telerik:tablebordercontrol>
			</fieldset>
		</td>
	</tr>

	<tr>
		<td colspan="3">
			<fieldset>
				<legend>
					<script>localization.showText('BackgroundImage');</script>
				</legend>

				<telerik:imagedialogcaller tabindex="6" id="bgImageDialogCaller" runat="server" />
			</fieldset>
		</td>
	</tr>

	<tr>
		<td colspan="3">
			<fieldset><legend>
					<script>localization.showText('TableCss');</script>
				</legend>
				<table width="100%">
					<tr>
						<td class="Label" nowrap>
							<script>localization.showText('CssClass');</script>
						</td>
						<td width="100%">
							<telerik:CssClassSelector id="theCssClassSelector"
								cssfilter="ALL, TABLE"
								width="150px"
								popupwidth="200px"
								popupheight="160px"
								runat="server">
							</telerik:CssClassSelector>
						</td>
						<td class="Label" style="display:none;">
							<telerik:StyleBuilderCaller								
								id="theStyleBuilderCaller"
								runat="server"
								/>
						</td>
					</tr>
				</table>
			</fieldset>
		</td>
	</tr>
</table>
<script language="javascript">
	<%=this.ClientID%> = new TablePropertiesControl('<%=this.ClientID%>', '<%=this.SkinPath%>', <%=BgColorPicker.ClientID%>, <%=TableAlignmentSelector.ClientID%>, <%=theTableBorderControl.ClientID%>, <%=bgImageDialogCaller.ClientID%>, <%=theCssClassSelector.ClientID%>, <%=this.cellSpacingSpinBox.ClientID%>, <%=this.cellPaddingSpinBox.ClientID%>, <%=this.theStyleBuilderCaller.ClientID%>);
</script>
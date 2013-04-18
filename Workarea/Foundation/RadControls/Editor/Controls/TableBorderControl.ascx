<%@ Control ClassName="TableBorderControl" Language="c#" AutoEventWireup="false" Codebehind="TableBorderControl.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.TableBorderControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="ColorPicker" Src="./ColorPicker.ascx" %>
<%@ Register TagPrefix="telerik" TagName="SpinBox" Src="./SpinBox.ascx" %>
<TABLE border="0" class="TableBorderControlMain" cellpadding="1" cellspacing="2">
	<tr>
		<td class="HButtonHolder" width="1" colspan=2>
			
			<table border="0" cellspacing=0 cellpadding=0>
				<tr>						
					<td>
						<table border="0" cellspacing=0 cellpadding=0 class=ImageButtonHolder>
							<tr>
								<td>
									<a id="TableAllsides" title="All sides" onclick="<%=this.ClientID%>.SetFrame('border')" href="#" >
										<button class="ImageButton" type="button">
											<img src="<%=this.SkinPath%>Dialogs/FrmBorderIcon.gif" align="absMiddle" border="0" >
										</button>
									</a>
									<script> localization.setAttribute("TableAllsides", "title", "Allsides");</script>
								</td>						
								<td>
									<A id="TableRules" title="All rows and columns" onclick="<%=this.ClientID%>.SetRules('all')" href="#" >
										<button class="ImageButton" type="button">
											<img src="<%=this.SkinPath%>Dialogs/RuleAllIcon.gif" align="absMiddle" border="0" >
										</button>
									</A>
									<script> localization.setAttribute("TableRules", "title", "Rules");</script>
								</TD>
								<td>
									<a id="TableNosides" title="No borders" onclick="<%=this.ClientID%>.SetFrame('void')" href="#" >
										<button class="ImageButton" type="button">
											<img src="<%=this.SkinPath%>Dialogs/FrmVoidIcon.gif" align="absMiddle" border="0" >
										</button>
									</a>
									<script> localization.setAttribute("TableNosides", "title", "Nosides");</script>
								</td>
								<td>
									<a id="TableNorules" title="No interior borders" onclick="<%=this.ClientID%>.SetRules('none')" href="#" >
										<button class="ImageButton" type="button">
											<img src="<%=this.SkinPath%>Dialogs/RuleNoneIcon.gif" align="absMiddle" border="0" >
										</button>
									</a>
									<script> localization.setAttribute("TableNorules", "title", "Norules");</script>
								</td>
							</tr>
						</table>
					</td>
					<td width="100%">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
					<td>
						<telerik:colorpicker id="BorderColorPicker" 
							runat="server"></telerik:colorpicker>
					</td>
					<td>
						<telerik:SpinBox id="theSpinBox" runat="server"/>
					</td>
				</tr>
			</table>
			
		</td>			
	</tr>
	
	<TR>
		<TD  class="VButtonHolder" valign="middle" align="center">
			<TABLE border="0" cellspacing=0 cellpadding=0 class=ImageButtonHolder>
				<TR>
					<TD>
						<A id="TableLefthand" title="Left side" onclick="<%=this.ClientID%>.SetFrame('lhs')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/FrmLeftIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableLefthand", "title", "Lefthand");</script>
					</TD>
				</TR>					
				<TR>
					<TD>
						<A id="TableBetweencolumns" title="Between columns" onclick="<%=this.ClientID%>.SetRules('cols')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/RuleColsIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableBetweencolumns", "title", "Betweencolumns");</script>
					</TD>
				</TR>
				<TR>
					<TD>
						<A id="TableRighthand" title="Right side" onclick="<%=this.ClientID%>.SetFrame('rhs')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/FrmRightIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableRighthand", "title", "Righthand");</script>
					</TD>
				</TR>
				<TR>
					<TD>
						<A id="TableRightleft" title="Left and right sides" onclick="<%=this.ClientID%>.SetFrame('vsides')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/FrmVsidesIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableRightleft", "title", "Rightleft");</script>
					</TD>
				</TR>
			</TABLE>
		</TD>
		<TD  class="Preview" align="center" valign="middle" width="150px" height="150px">				
			<div style="overflow: auto; width: 120px; height: 120px;">
				<table id="<%=this.ClientID%>_PREVIEW" class="TableBorderControlPreview" cellspacing="0" cellspacing="0" width="118px" height="118px">
					<tr>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
					</tr>
					<tr>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
					</tr>
					<tr>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
					</tr>
					<tr>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
						<td width="25%">&nbsp;</td>
					</tr>
				</table>
			</div>
		</TD>
	</TR>
	<TR>
		<TD>&nbsp;</TD>
		<TD class="HButtonHolder" valign="middle" align="center">			
			<TABLE border="0" cellspacing=0 cellpadding=0 class=ImageButtonHolder>
				<TR>
					<TD>
						<A id="TableTopbottom" title="Top and bottom side" onclick="<%=this.ClientID%>.SetFrame('hsides')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/FrmHsidesIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableTopbottom", "title", "Topbottom");</script>
					</TD>						
					<TD>
						<A id="TableTopside" title="Top side" onclick="<%=this.ClientID%>.SetFrame('above')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/FrmAboveIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableTopside", "title", "Topside");</script>
					</TD>
					<TD>
						<A id="TableBetweenrows" title="Between rows" onclick="<%=this.ClientID%>.SetRules('rows')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/RuleRowsIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableBetweenrows", "title", "Betweenrows");</script>
					</TD>
					<TD>
						<A id="TableBottomside" title="Bottom side" onclick="<%=this.ClientID%>.SetFrame('below')" href="#" >
							<button class="ImageButton" type="button">
								<img src="<%=this.SkinPath%>Dialogs/FrmBelowIcon.gif" align="absMiddle" border="0" >
							</button>
						</A>
						<script> localization.setAttribute("TableBottomside", "title", "Bottomside");</script>
					</TD>			
				</TR>
			</TABLE>			
		</TD>
	</TR>
</TABLE>

<script language="javascript">
	var <%=this.ClientID%> = new TableBorderControl('<%=this.ClientID%>', <%=this.BorderColorPicker.ClientID%>, <%=theSpinBox.ClientID%>);
</script>
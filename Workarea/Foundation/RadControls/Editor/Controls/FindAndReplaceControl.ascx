<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FindAndReplaceControl.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.FindAndReplaceControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table height="100%" cellpadding="6" cellspacing="0" border="0">
	<tr>
		<td valign="top">
			<table cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td width="90" style="PADDING-BOTTOM:5px">
						<label for="<%=this.ClientID%>_searchText">
							<script>localization.showText('SearchText')</script>
						</label>
					</td>
					<td style="PADDING-RIGHT:10px;PADDING-BOTTOM:5px">
						<input type="text" id="<%=this.ClientID%>_searchText" style="WIDTH:210px" class="Text" tabindex=1>
					</td>
					<td style="PADDING-BOTTOM:5px">
						<button class="Button" title="Find Next" type="button" onclick="javascript:<%=this.ClientID%>.FindNext();" tabindex=9>
							<script>localization.showText('FindNext')</script>
						</button>
					</td>
				</tr>
				<tr id="<%=this.ClientID%>_replaceDialogRow" style="DISPLAY:none">
					<td width="90">
						<label for="<%=this.ClientID%>_replacementText">
							<script>localization.showText('ReplacementText')</script>
						</label>
					</td>
					<td>
						<input type="text" id="<%=this.ClientID%>_replacementText" name="replacementText" style="WIDTH:210px" class="Text" tabindex=2>
					</td>
					<td>
						<button class="Button" title="Replace" type="button" onclick="javascript:<%=this.ClientID%>.RunReplace();" tabindex=10>
							<script>localization.showText('Replace')</script>
						</button>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<table cellpadding="0" cellspacing="0" border="0">
							<tr>
								<td style="PADDING-RIGHT:4px" class="Label">
									<fieldset style="PADDING-LEFT:5px;WIDTH:133px">
										<legend>
											<script>localization.showText('SearchIn')</script>
										</legend>
										<input type="radio" title="Search Entire Text" name="searchTypeRadio" id="<%=this.ClientID%>_searchTypeRadioWholeText" onclick="javascript:<%=this.ClientID%>.SetSearchType(this.value);" value="Whole" checked tabindex=3><label for="searchTypeRadioWholeText"><script>localization.showText('SearchEntireText')</script></label><br>
										<input type="radio" title="Search Selection" name="searchTypeRadio" id="<%=this.ClientID%>_searchTypeRadioSelectionOnly" onclick="javascript:<%=this.ClientID%>.SetSearchType(this.value);" value="Selection" tabindex=4><label for="searchTypeRadioSelectionOnly"><script>localization.showText('SearchSelection')</script></label>
									</fieldset>
								</td>
								<td class="Label">
									<fieldset style="PADDING-LEFT:5px;WIDTH:133px">
										<legend>
											<script>localization.showText('SearchDirection')</script>
										</legend>
										<input type="radio" title="Search Down" name="searchDirectionRadio" id="searchDirectionRadioUp" onclick="javascript:<%=this.ClientID%>.SetSearchDirection(this.value);" value="Up" tabindex=5><label for="searchDirectionRadioUp"><script>localization.showText('SearchUp')</script></label><br>
										<input type="radio" title="Search Up" name="searchDirectionRadio" id="searchDirectionRadioDown" onclick="javascript:<%=this.ClientID%>.SetSearchDirection(this.value);" checked value="Down" tabindex=6><label for="searchDirectionRadioDown"><script>localization.showText('SearchDown')</script></label>
									</fieldset>
								</td>
							</tr>
							<tr>
								<td colspan="2">&nbsp;</td>
							</tr>
							<tr>
								<td colspan="2" class="Label">
									<p>
										<input type="checkbox" title="Match Case" style="MARGIN-LEFT:-4px" name="searchModifierMatchCaseCheckBox" id="searchModifierMatchCaseCheckBox" onclick="javascript:<%=this.ClientID%>.SetCaseMatch(this.checked);" tabindex=7><label for="searchModifierMatchCaseCheckBox"><script>localization.showText('MatchCase')</script></label><br>
										<input type="checkbox" title="Whole Words Only" style="MARGIN-LEFT:-4px" name="searchModifierMatchWholeWordsCheckBox" id="searchModifierMatchWholeWordsCheckBox" onclick="javascript:<%=this.ClientID%>.SetWholeWordMatch(this.checked);" tabindex=8><label for="searchModifierMatchWholeWordsCheckBox"><script>localization.showText('WholeWordsOnly')</script></label>
									</p>
								</td>
							</tr>
						</table>
					</td>
					<td valign="top" style="PADDING-TOP:0px" height="100%">
						<table cellpadding="0" cellspacing="0" border="0" height="100%">
							<tr id="<%=this.ClientID%>_replaceAllDialogRow" style="DISPLAY:none">
								<td style="PADDING-BOTTOM:7px;PADDING-TOP:0px">
									<button class="Button" title="Replace All" type="button" onclick="javascript:<%=this.ClientID%>.ReplaceAll();" tabindex=11>
										<script>localization.showText('ReplaceAll')</script>
									</button>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<table border="0" cellpadding="0" cellspacing="0">
										<tr>
											<td style="pading-bottom:4px" tabindex=12>
												<button class="Button" title="Ok" id="submitButton" onclick="javascript:<%=this.ClientID%>.CloseWindow();" type="button" >
													<script>localization.showText('OK')</script>
												</button>
											</td>
										</tr>
										<tr>
											<td style="PADDING-BOTTOM:6px;PADDING-TOP:4px">
												<button class="Button" title="Cancel" onclick="javascript:<%=this.ClientID%>.CancelChanges();" type="button" tabindex=13>
													<script>localization.showText('Cancel')</script>
												</button>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="FlashPreviewer.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.FlashPreviewer" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.Dialogs" Assembly="Ektron.RadEditor" %>
<table cellpadding="0" cellspacing="0">
	<tr>
		<td height="23" class="label">
			<input type="checkbox" title="Flash Preview" onclick="javascript:SwitchPreviewMode();">
			<script>localization.showText("SwitchToPreviewMode");</script>
		</td>
	</tr>
	<tr>
		<td>
			<div class="PreviewAreaHolder">
				<div id="PropertiesPane">
					<table cellpadding="0" cellspacing="0" align="left">
						<tr>
							<td colspan="2" valign="top">
								<span id="loader" style="display:none"></span>
							</td>
						</tr>
						<tr>
							<td nowrap class="label" id="SpecifyClassIDCell">
								<script>
									localization.showText('SpecifyClassID');
									localization.setAttribute("SpecifyClassIDCell", "title", "NoteClassID");
								</script>
							</td>
							<td>
								<input type="checkbox" title="Enable" defaultvalue="false" id="classYes" onclick="enableClass(this.checked)" />
							</td>
						</tr>
						<tr id="classIDRow1" style="display:none">
							<td class="label" nowrap>
								<script>localization.showText('ClassID');</script>
							</td>
							<td>
								<input type="text" defaultvalue="" id="classID" class="RadETextBox" disabled />
							</td>
						</tr>
						<tr id="classIDRow2" style="display:none">
							<td class="label" title="Flash Version">
								<script>localization.showText('FlashVersion');</script>
							</td>
							<td>
								<select title="Select Flash Version from Drop Down Menu" defaultvalue="5" class="DropDown" id="version" disabled>
									<option title="Flash Version 5" value="5">5.0</option>
									<option title="Flash Version 6" value="6">6.0</option>
									<option title="Flash Version 8" value="8">8.0</option>
									<option title="Flash Version 9" value="9">9.0</option>
								</select>
							</td>
						</tr>

						<tr>
							<td  class="label" title="Width">
								<script>localization.showText('Width');</script>
							</td>
							<td>
								<input type="text" title="Enter the Width of Flash Video" id="flashWidth" style="width:110px" defaultvalue="150" class="RadETextBox" />
							</td>
						</tr>
						<tr>
							<td  class="label" title="Height">
								<script>localization.showText('Height');</script>
							</td>
							<td>
								<input type="text" title="Enter the Height of Flash Video" id="flashHeight" style="width:110px" defaultvalue="150" class="RadETextBox" />
							</td>
						</tr>
						<tr>
							<td  class="label" title="Quality">
								<script>localization.showText('Quality');</script>
							</td>
							<td>							
								<select title="Select Video Quality" id="quality" class="DropDown" defaultvalue="high">
								   <script>
									document.write(									
										'<option value="high">' + localization.getText('High') + '</option>' + 
										'<option value="medium">' + localization.getText('Medium') + '</option>' + 
										'<option value="low">' +  localization.getText('Low') + '</option>'
									);	
									</script>									
								</select>
							</td>
						</tr>
						<tr>
							<td class="label">
								<script>localization.showText('Play');</script>
							</td>
							<td >
								<input title="Play" type="checkbox" checked="true" defaultvalue="true" id="playYes" />
							</td>
						</tr>
						<tr>
							<td  class="label">
								<script>localization.showText('Loop');</script>
							</td>
							<td >
								<input title="Loop" type="checkbox" defaultvalue="false" id="loopYes" />
							</td>
						</tr>
						<tr>
							<td  class="label">
								<script>localization.showText('FlashMenu');</script>
							</td>
							<td >
								<input title="Menu" type="checkbox" defaultvalue="false" id="menuYes" />
							</td>
						</tr>
						<tr>
							<td  class="label">
								<script>localization.showText('Transparent');</script>
							</td>
							<td >
								<input title="Transparent" type="checkbox" defaultvalue="false" id="transparentYes" />
							</td>
						</tr>
						<tr>
							<td  class="label">
								<script>localization.showText('HTMLAlign');</script>
							</td>
							<td>
								<select id="htmlAlign" defaultvalue="baseline" class="DropDown">
								<script>
									document.write(
									'<option value="baseline">' +
										localization.getText('Baseline') + 
									'</option>' + 
									'<option value="bottom">' +
										localization.getText('Bottom') + 
									'</option>' + 
									'<option value="left">' +
										localization.getText('Left') + 
									'</option>' + 
									'<option value="middle">' +
										localization.getText('Middle') + 
									'</option>' + 
									'<option value="right">' +
										localization.getText('Right') + 
									'</option>' + 
									'<option value="texttop">' +
										localization.getText('TextTop') + 
									'</option>' + 
									'<option value="top">' +
										localization.getText('Top') + 
									'</option>'
									);
									
								</script>									
								</select>
							</td>
						</tr>
						<tr>
							<td  class="label">
								<script>localization.showText('FlashAlign');</script>
							</td>
							<td>
								<select defaultvalue="LT" class="DropDown" id="flashAlign">
								<script>
									document.write(
									
									'<option value="LT">' +
										localization.getText('LeftTop') + 
									'</option>' + 
									'<option value="LC">' +
										localization.getText('LeftCenter') + 
									'</option>' + 
									'<option value="LB">' +
										localization.getText('LeftBottom') + 
									'</option>' + 
									'<option value="RT">' +
										localization.getText('RightTop') + 
									'</option>' + 
									'<option value="RC">' +
										localization.getText('RightCenter') + 
									'</option>' + 
									'<option value="RB">' +
										localization.getText('RightBottom') + 
									'</option>' + 
									'<option value="CT">' +
										localization.getText('CenterTop') + 
									'</option>' + 
									'<option value="CC">' +
										localization.getText('CenterCenter') + 
									'</option>' + 
									'<option value="CB">' +
										localization.getText('CenterBottom') + 
									'</option>'																										
									);																		
								</script>	
								
								</select>
							</td>
						</tr>
						<tr>
							<td  class="label" title="Background Color">
								<script>localization.showText('BackgroundColor');</script>
							</td>
							<td>
								<select title="Select Background Color from Drop Down Menu" defaultvalue="" class="DropDown" id="backgroundColor" onchange="changeColor(this, '_______', 9)">
								<script>
									document.write(
									
									'<option value="">' + 
										localization.getText('NoColor') + 
									'</option>' + 
									'<option value="#000000" style="background-color:#000000">' + 
										localization.getText('Black') + 
									'</option>' + 
									'<option value="#0000FF" style="background-color:#0000FF">' + 
										localization.getText('Blue') + 
									'</option>' + 
									'<option value="#008000" style="background-color:#008000">' + 
										localization.getText('Green') + 
									'</option>' + 
									'<option value="#FFA500" style="background-color:#FFA500">' + 
										localization.getText('Orange') + 
									'</option>' + 
									'<option value="#FF0000" style="background-color:#FF0000">' + 
										localization.getText('Red') + 
									'</option>' + 
									'<option value="#FFFFFF" style="background-color:#FFFFFF">' + 
										localization.getText('White') + 
									'</option>' + 
									'<option value="#FFFF00" style="background-color:#FFFF00">' + 
										localization.getText('Yellow') + 
									'</option>' + 
									'<option value="_______">' + 
										localization.getText('Custom') + 
									'</option>'									
									);								
									</script>																	
								</select>
								<object id="dlgHelper" CLASSID="clsid:3050f819-98b5-11cf-bb82-00aa00bdce0b" width="0px" height="0px" VIEWASTEXT></object>
							</td>
						</tr>
					</table>
				</div>
				<div id="PreviewPane">
					<table border="0" cellpadding="0" cellspacing="0" align="center" height="100%">
						<tr>
							<td id="PreviewObjectHolder"></td>
						</tr>
					</table>
				</div>
				<div id="EmptyPane">
					<table cellpadding="0" cellspacing="0" align="center" height="100%">
						<tr>
							<td></td>
						</tr>
					</table>
				</div>
			</div>
		</td>
	</tr>
</table>
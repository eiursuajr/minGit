<%@ Control Language="c#" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="ImageDialogCaller" Src="../Controls/ImageDialogCaller.ascx"%>
<script>
	AttachEvent(window, "load", InitializeImageMap);
</script>
<TABLE class="MainTable" id="holderTable" cellSpacing="0" cellPadding="0" width="100%" height="100%">
	<tr>
		<TD class="MainTableContentCell" width="100%" height="100%">
			<TABLE height="100%">
				<TR>
					<TD height="100%" valign = "top">
						<iframe src="javascript:''" id="ImageMapFrame" width="410" height="380">
						</iframe>
						<textarea id = "ImageMapHTML" style = "display:none">
							<style>

								/*    RECTANGLE      */	
								.rect_shape_selected 
								{ 
									background-color:#33FFFF; 
									-moz-opacity:0.4; 
									font-size:1px; 
									cursor:move;
								}

								.rect_shape_not_selected 
								{ 
									background-color:#33ccff; 
									-moz-opacity:0.4; 
									font-size:1px; 
									cursor:move;
								}

								.rect_shape_resizer 
								{ 
									width:5;  
									height:5;
									background-color:red; 
									font-size:1px; 
									position:absolute;
									cursor:SE-resize;
								}
								
								.rect_shape_border
								{
									border : 1px solid black;
								}
								
								/*    CIRCLE      */
								.circ_shape_selected 
								{ 
									-moz-opacity:0.4;
									cursor:move;
								}

								.circ_shape_not_selected 
								{ 
									-moz-opacity:0.4;
									cursor:move;
								}

								.circ_shape_resizer 
								{ 
									width:5; 
									height:5;
									background-color:red; 
									font-size:1px; 
									position:absolute;
									cursor:W-resize;
								}
								
								IMG 
								{
									border: 1px solid black;
								}
								/* see below the circle bckg color define in JS -> CIRCLESHAPE_BCKG_COLOR_SELECTED */
							</style>
							<!--[if IE]>
							<STYLE>
								.rect_shape_selected {
									filter:progid:DXImageTransform.Microsoft.Alpha(opacity=40);
								}

								.rect_shape_not_selected {
									filter:progid:DXImageTransform.Microsoft.Alpha(opacity=40);
								}
								
								.circ_shape_selected 
								{ 
									filter:progid:DXImageTransform.Microsoft.Alpha(opacity=40); 
								}

								.circ_shape_not_selected 
								{ 
									filter:progid:DXImageTransform.Microsoft.Alpha(opacity=40);
								}
								
								
								v\:* { 
									behavior: url(#default#VML);

								}
								
							</STYLE>
							<![endif]-->
							<!-- 

							Insert this DIV in order FF not to drag the image and to stop the events
							The table inside the DIV is because IE to handle the mousedown event
								-->
							<div id = dummy style ="position:absolute;" unselectable="on">
								<table  cellpadding=0 cellspacing=0 width = 100% height = 100%>
									<tr>
										<td unselectable="on" style ="font-size:1px"></td>
									</tr>
								</table>
							</div>
		
						</textarea>
						<script>
							var CIRCLESHAPE_BCKG_COLOR_NOT_SELECTED = '#33CCFF';
							var CIRCLESHAPE_BCKG_COLOR_SELECTED		= '#33FFFF';
							InitImageMapFrame();
						</script>
					</TD>
					<TD valign="top" width="400">

						<table height = "380" cellpadding="0" cellspacing="0">
							<tr>
								<td height ="100%" valign = top>
						<fieldset id ="image_props" >
							<legend>
								<script>localization.showText('ImageFieldSet');</script>
							</legend>
								<table cellpadding="2" cellspacing="2">
										<tr>
											<td align="right" width="50"><nobr><label><script>localization.showText('ImageSrc');</script></label></nobr></td>
											<td>
												<telerik:imagedialogcaller
													id="changeSourceImageDialogCaller"
													runat="server"/>
												<script>
													var ImageDialogCaller = <%=changeSourceImageDialogCaller.ClientID%>;
												</script>
											</td>
										</tr>
									</table>
						</fieldset>
						<table cellpadding=0 cellspacing=2 id ="area_controls" style = "display:none">
							<tr>
								<td><button class="Button" title="New Area" onclick="var t = InsertNewMapAreaDlg()" type="button">
										<script>localization.showText('NewArea');</script>
									</button>
								</td>
								<td><input id="shape_type" title="Select Shape Type - Rectangle Option" type="radio" name="shape_type" checked></td>
								<td><span class="Text"  title="Shape Type - Rectangle Name Entry" onclick = "forms[0]['shape_type'][0].click()"><script>localization.showText('RectangleShapeName');</script></span></td>
								<td><input id="shape_type" title="Select Shape Type - Circular Option" type="radio" name="shape_type" ></td>
								<td><span class="Text"  title="Shape Type - Circular Name Entry" onclick = "forms[0]['shape_type'][1].click()"><script>localization.showText('CircleShapeName');</script></span></td>
								<td><input style = "display:none" title="Select Shape Type - Polygon Option" id="shape_type" type="radio" name="shape_type"  disabled></td>
								<td><span class="Text"  title="Shape Type -  Polygon Name Entry" style = "display:none" onclick = "forms[0]['shape_type'][2].click()"><script>localization.showText('PolygonShapeName');</script></span></td>
							</tr>
							<tr>
								<td colspan ="7" style = "padding-left:6" class="Text">
							<script>localization.showText('HowToAdd');</script>
						</span></td>
							</tr></table>
						<script>
							// set the values for the shape types
							var Form = document.forms[0];
							var ShapeTypeGroup = Form["shape_type"];
							ShapeTypeGroup[0].value = AREA_SHAPE_CONSTANTS.RECTANGLE_TYPE;
							ShapeTypeGroup[1].value = AREA_SHAPE_CONSTANTS.CIRCLE_TYPE;
							ShapeTypeGroup[2].value = AREA_SHAPE_CONSTANTS.POLYGON_TYPE;
							
							if (!IsCircleShapeSupported()) {// IE and FF1.5+
								ShapeTypeGroup[1].disabled = true;
							}
							
							ShapeTypeGroup[2].disabled = true;// dotn support polygon for now
						</script>

						<fieldset style = "display:none" id ="map_props" >
							<legend>
								<script>localization.showText('AreaFieldSet');</script>
							</legend>
							<DIV id="area_props">
								<DIV id="rect_props">
									<TABLE cellpadding="2" cellspacing="2">
										<TR>
											<TD align="right" width="50"><label for="rect_x" title="Left"><SCRIPT>localization.showText('Left');</SCRIPT></label></TD>
											<TD><INPUT id="rect_x" onblur="UpdateAreaPropsDlg()" title="Enter Left Value" type="text" name="rect_x" class="Text" style="WIDTH:50px" disabled></TD>
										</TR>
										<TR>
											<TD align="right"><label for="rect_y" title="Top"><SCRIPT>localization.showText('Top');</SCRIPT></label></TD>
											<TD><INPUT id="rect_y" onblur="UpdateAreaPropsDlg()" title="Enter Top Value" type="text" name="rect_y" class="Text" style="WIDTH:50px" disabled></TD>
										</TR>
										<TR>
											<TD align="right"><label for="rect_width" title="Width"><SCRIPT>localization.showText('Width');</SCRIPT></label></TD>
											<TD><INPUT id="rect_width" onblur="UpdateAreaPropsDlg()" title="Enter Width Value" type="text" name="rect_width" class="Text" style="WIDTH:50px" disabled></TD>
										</TR>
										<TR>
											<TD align="right"><label for="rect_height" title="Height"><SCRIPT>localization.showText('Height');</SCRIPT></label></TD>
											<TD><INPUT id="rect_height" onblur="UpdateAreaPropsDlg()" title="Enter Height Value" type="text" name="rect_height" class="Text" style="WIDTH:50px"
													disabled></TD>
										</TR>
									</TABLE>
								</DIV>
								<TABLE cellpadding="2" cellspacing="2">
									<TR>
										<TD width="50" align="right"><label for="area_link" title="Url"><SCRIPT>localization.showText('Url');</SCRIPT></label></TD>
										<TD><INPUT id="area_link" onblur="UpdateAreaPropsDlg()" type="text" name="area_link" class="Text" disabled></TD>
									</TR>
									<TR>
										<TD align="right"><label for="area_target" title="Target"><SCRIPT>localization.showText('Target');</SCRIPT></label></TD>
										<TD><input type="text" id="area_target" onblur="UpdateAreaPropsDlg()"  style="WIDTH:50px" class="Text" disabled>&nbsp;
											<select title="Select Option from Drop Down Menu" id="area_target_selector" class="DropDown" onchange="forms[0].area_target.value = this.value;UpdateAreaPropsDlg();"
												disabled>
												<option value="" selected>
													<script>localization.showText('Target')</script>
												</option>
												<option title="blank" value="_blank"><script>localization.showText('_blank')</script></option>
												<option title="parent" value="_parent"><script>localization.showText('_parent')</script></option>
												<option title="self" value="_self"><script>localization.showText('_self')</script></option>
												<option title="top" value="_top"><script>localization.showText('_top')</script></option>
												<option title="search" value="_search"><script>localization.showText('_search')</script></option>
												<option title="media" value="_media"><script>localization.showText('_media')</script></option>
											</select>
										</TD>
									</TR>
									<TR>
										<TD align="right"><label title="Comment" for="area_comment"><SCRIPT>localization.showText('Comment');</SCRIPT></label></TD>
										<TD><INPUT id="area_comment" title="Enter Comment Here" onblur="UpdateAreaPropsDlg()" type="text" name="area_comment" class="Text" disabled></TD>
									</TR>
								</TABLE>
								<BUTTON class="Button" title="Update" onclick="var t = UpdateAreaPropsDlg()" type="button" id="area_update_button" disabled><nobr><SCRIPT>localization.showText('Update');</SCRIPT></nobr></BUTTON><button class="Button" title="Remove" onclick="var t = RemoveAreaDlg()" type="button" id="area_remove_button" disabled><nobr><script>localization.showText('Remove');</script></nobr></button><button class="Button" title="Remove All" onclick="var t = RemoveAreaDlg(true)" type="button" id="area_remove_all_button"><nobr><script>localization.showText('RemoveAll');</script></nobr></button>
							</DIV>
						</fieldset>
								</td>
							</tr>
							<tr>
								<td align = right><button class="Button" title="Ok" onclick="ReturnNewImageMap()" type="button">
										<script>localization.showText('OK');</script>
									</button><button title="Close" class="Button" onclick="CloseDlg();" type="button">
										<script>localization.showText('Cancel');</script>
									</button>

								</td>
							</tr>
						</table>
					</TD>
				</TR>
			</TABLE>
		</TD>
	</tr>
</TABLE>

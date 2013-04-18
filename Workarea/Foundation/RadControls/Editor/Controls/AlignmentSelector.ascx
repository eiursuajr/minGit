<%@ Control Language="c#" AutoEventWireup="false" Codebehind="AlignmentSelector.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.AlignmentSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
	<button id='MENU_BUTTON_<%=this.ClientID%>' class="AlignmentMainButton" onmouseover="this.className='AlignmentMainButtonOver'" onmouseout="this.className='AlignmentMainButton'" onclick="<%=this.ClientID%>.Toggle();return false;" onfocus="this.blur();">
		<img style="margin-left:-2px;padding-bottom:3px;" id='MENU_BUTTON_IMG_<%=this.ClientID%>' align="absMiddle" src="<%=this.SkinPath%>Img/AlignMiddleLeft.gif" border="0">
	</button>	
	
	<div class="MenuElement" id="MENU_ELEMENT_<%=this.ClientID%>" style="visibility: hidden">
		<table id="MENU_ELEMENT_TABLE_<%=this.ClientID%>" class="AlignmentControlTable" cellpadding=0 cellspacing=0>
			<tr>
				<td>
					<a href="#">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(0);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/x.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(1);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/x.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(2);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/x.gif" border="0">
						</button>
					</a>
				</td>
			</tr>
			<tr>
				<td id="TopLeft">
					<a href="#" title="Align to the Top Left"> 
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(3);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignTopLeft.gif" border="0">
						</button>
					</a>
				</td>				
				<td>
					<a href="#" title="Align to the Top Center">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(4);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignTopCenter.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#" title="Align  to the Top Right">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(5);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignTopRight.gif" border="0">
						</button>
					</a>
				</td>
			</tr>
			<tr>
				<td>
					<a href="#" title="Align to the Middle Left">
						<button class="ImageButton"  onclick="<%=this.ClientID%>.SelectAlignmentByIndex(6);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignMiddleLeft.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#" title="Align to the Middle Center">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(7);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignMiddleCenter.gif" border="0">
						</button>
					</a>
				</td>
				<td><a href="#" title="Align  to the Middle Right"> <button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(8);" 
					 type=button><img align="absMiddle" src="<%=this.SkinPath%>Img/AlignMiddleRight.gif" border="0"></button>
					</a>
				</td>
			</tr>
			<tr>
				<td>
					<a href="#" title="Align to the Bottom Left">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(9);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignBottomLeft.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#" title="Align to the Bottom Cener">	
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(10);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignBottomCenter.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#">	
						<button title="Align  to the Bottom Right" class="ImageButton"  onclick="<%=this.ClientID%>.SelectAlignmentByIndex(11);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/AlignBottomRight.gif" border="0">
						</button>
					</a>
				</td>
			</tr>
			<tr>
				<td>
					<a href="#">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(12);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/x.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(13);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/x.gif" border="0">
						</button>
					</a>
				</td>
				<td>
					<a href="#">
						<button class="ImageButton" onclick="<%=this.ClientID%>.SelectAlignmentByIndex(14);" type=button>
							<img align="absMiddle" src="<%=this.SkinPath%>Img/x.gif" border="0">
						</button>
					</a>
				</td>
			</tr>
		</table>
	</div>
<script>
	var menu_<%=this.ClientID%> = new DropDownMenu(document.getElementById('MENU_BUTTON_<%=this.ClientID%>'), document.getElementById('MENU_ELEMENT_<%=this.ClientID%>'));
	var <%=this.ClientID%> = new AlignmentSelector('<%=this.ClientID%>', '<%=this.SkinPath%>', '<%=this.Mode%>', menu_<%=this.ClientID%>);
</script>
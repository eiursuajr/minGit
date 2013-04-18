<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="TargetContentEdit.aspx.cs" Inherits="Workarea_TargetedContent_TargetContentEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="../PageBuilder/PageControls/PageHost.ascx" TagPrefix="PB" TagName="PageHost" %>
<%@ Register Src="../PageBuilder/PageControls/DropZone.ascx" TagPrefix="PB" TagName="DropZone" %>
<%@ Register Assembly="Ektron.Cms.Widget" Namespace="Ektron.Cms.PageBuilder" TagPrefix="PB" %>
<%@ Register Src="../../widgets/TargetedContent.ascx" TagPrefix="TC" TagName="TargetedContent" %>

<%@ Register Src="../PageBuilder/PageControls/WidgetHost.ascx" TagPrefix="W" TagName="WidgetHost" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<asp:Literal id="ltrlStyleSheetJS" runat="server" />
	<style type="text/css">
		div.settings {margin : 10px 5px}
		label#lblName {font-weight:bold;}
		div.topmenu{display:none;}
		div.ektronPageHeader { margin-top:5px;}
		div.ektronPageContainer { margin-top:5px;}
		div.widgetlist a.scrollLeft {top:0px;}
		div#targetContentZoneWrapper{width: 350px;margin-bottom:250px;}
		
		/*Hide default columns and headers */
		div#targetContentZone_dzcontainer {border-width:0px;}
		div.dropzone div.PBColumn ul.columnwidgetlist { background-image:none;border-width:0px;}
		
		/*Hide default column controls*/
		div.PBDZhasHeader > .PBDZHeader {background-color: White;}
		div.PBDZhasHeader .PBDZHeader  input.PBAddColumn {display:none;}
		div#targetContentZone_dzcontainer div.PBColumn ul.columnwidgetlist li.header a.resizeColumn {display:none;}
		div#targetContentZone_dzcontainer div.PBColumn ul.columnwidgetlist li.header a.remColumn {display:none;} 
	   
		/* Target Content Header buttons*/
		div#targetContentZone_dzcontainer div.PBColumn ul.columnwidgetlist li.PBItem div.widget div.header div.buttons > a.edit {display:none;}
		div#targetContentZone_dzcontainer div.PBColumn ul.columnwidgetlist li.PBItem div.widget div.header div.buttons > a.delete {display:none;}
		div.content a.globalConfigSelect {display:none;}
		
		/* re-display sub columns */
		div#targetContentZone_dzcontainer ul.columnwidgetlist li.PBItem div.content ul.columnwidgetlist { background-image:url("../PageBuilder/PageControls/Themes/TrueBlue/images/header-background.png");border-width:1px;}

		/* re-display segment remove button */
		div#targetContentZone_dzcontainer div.PBColumn ul.columnwidgetlist li.PBItem li.header a.remColumn {display:inline;}
		
		/* re-display edit buttons inside targetcontent drop zone*/
		div#targetContentZone_dzcontainer div.PBColumn ul.columnwidgetlist li.PBItem div.widget div.content div.header div.buttons > a.edit {display:inline;}
		div#targetContentZone_dzcontainer div.PBColumn ul.columnwidgetlist li.PBItem div.widget div.content div.header div.buttons > a.delete {display:inline;}
		.WidgetSave, .WidgetCancel{ height: auto !important;}
	</style>

<script type="text/javascript">
	Ektron.ready(function() {
		init();
		//make sure script is re-executed on updatepanel postback
		if (typeof Sys != "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager && Sys.WebForms.PageRequestManager.getInstance() != null) {
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(init);
		}
	});

	function init() {
	    //destry drop areas
	    setTimeout(function () {
	        if ($ektron(".dropzone > div.PBColumn > ul.columnwidgetlist").is(".ui-sortable")) {
	            $ektron(".dropzone > div.PBColumn > ul.columnwidgetlist").sortable("destroy");
	        }
	    }, 1000);

		//recreate drop areas ONLY within targeted content widget
		//NOTE: copied from Workarea\PageBuilder\PageControls\JS\WidgetTray.js
		var connectWithEls = 'li.PBItem div.PBColumn ul.columnwidgetlist';
		
		CheckKeyValue = function (item, keys) {
			var keyArray = keys.split(",");
			for (var i = 0; i < keyArray.length; i++) {
				if ((document.layers) || ((!document.all) && (document.getElementById))) {
					if (item.which == keyArray[i]) {
						return false;
					}
				}
				else {
					if (event.keyCode == keyArray[i]) {
						return false;
					}
				}
			}
		}
		
		replaceSpecialCharacter = function()
		{
			$ektron("#txtName")[0].value =$ektron("#txtName")[0].value.replace(/</g, "").replace(/>/g, "");
		}
		if ($ektron(connectWithEls).is("ui-sortable")) {
			$ektron(connectWithEls).sortable("destroy");
		}
		$ektron(connectWithEls).sortable({
			connectWith: [connectWithEls],
			items: 'li.PBItem:not(.header)',
			zIndex: 99999,
			scroll: true,
			cursor: "move",
			handle: 'div.header',
			tolerance: 'intersect',
			distance: 3,
			cursorAt: {
				top: 5,
				left: 5
			},
			placeholder: 'PBHighlight widget50px',
			helper: function(e, el) {
				var helper = null;
				Ektron.PageBuilder.WidgetHost.toggleDropZones();
				Ektron.PageBuilder.WidgetHost.destroyResizables();

				var type = $ektron(el).find("[widget-type-id]");
				if (type.length !== 0) {
					var typeid = type.attr("widget-type-id");
					var wid = $ektron("ul.widgetList li#" + typeid + "-Widget");
					if (wid.length !== 0) {
						helper = wid.clone();
						helper.width("80px");
						helper.height("80px");
						helper.addClass("ektronPBWidgetTokenDrag");
						if (helper.find("span").length !== 0 && type.find("span").length !== 0) {
							helper.find("span").html(type.find("span").html());
							if ($ektron.trim(helper.find("span").html().replace(/&nbsp;/g, "")) === "") {
								helper.find("span").html(wid.find("span").html());
							}
						}
					}
				}

				if (helper === null) {
					helper = $ektron("<li class=\"widgetToken ektronPBWidgetTokenDrag\" title=\"" + Ektron.ResourceText.PageBuilder.WidgetTray.widget + "\"><span>" + Ektron.ResourceText.PageBuilder.WidgetTray.widget + "</span></li>");
				}

				helper.prependTo("body");
				return helper;
			},
			start: function(Event, ui) {
				Ektron.PageBuilder._srcContainer = ui.item.parent();
			},
			stop: function(Event, ui) {
				Ektron.PageBuilder.WidgetHost.dropHandler(ui);
				Ektron.PageBuilder.WidgetHost.createResizables();
				Ektron.PageBuilder.WidgetHost.toggleDropZones();
				
			}
		});

		if (!Ektron.PageBuilder.WidgetTray.menuOpen) {
			Ektron.PageBuilder.WidgetTray.menuToggle();
		}

		var tacked = ($ektron.cookie && $ektron.cookie("PageBuilderMenuTacked") == "true");
		if (!tacked) {
			$ektron.cookie("PageBuilderMenuTacked", "true");
		}
		Ektron.PageBuilder.WidgetTray.menuTackInit();
		
	}
	verifyTitle = function(e)
	{
		if($ektron("#txtName")[0].value == "")
		{
			alert("<asp:Literal runat='server' id='ltrTitleEmpty' />");
			if(navigator.appName == "Netscape")
			{
				e.preventDefault();
			}
			else
			{
				event.returnValue=false; 
				event.cancel =true;
			}
			return false;
		}
}

function ShowTransString(Text) {
    var ObjId = "ektronTitlebar";
    var ObjShow = document.getElementById('_' + ObjId);
    var ObjHide = document.getElementById(ObjId);
    if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
        ObjShow.innerHTML = Text;
        ObjShow.style.display = "inline";
        if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
            ObjHide.style.display = "none";
        }
    }
}

function HideTransString() {
    var ObjId = "ektronTitlebar";
    var ObjShow = document.getElementById(ObjId);
    var ObjHide = document.getElementById('_' + ObjId);
    if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
        ObjShow.style.display = "inline";
        if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
            ObjHide.style.display = "none";
        }
    }
}
</script>
</head>
<body>

	<form id="form1" runat="server">
		<PB:PageHost ID="PageHost1" runat="server" />
		
		<div class="ektronPageHeader">
			<div class="ektronTitlebar" id="txtTitleBar" runat="server">
				<span id="WorkareaTitlebar" style="display: inline;" runat="server"></span>
				<span id="_WorkareaTitlebar" style="display: none;"></span>
			</div>
			<div class="ektronToolbar" id="htmToolBar" runat="server">
				<table>
					<tbody>
						<tr>
							<td title="" class="button" id="image_cell_101" runat="server">
								<asp:LinkButton CssClass="primary cancelButton" ID="image_link_101" OnClick="ucCancelButton_click" style="cursor:default;" onmouseout="HideTransString();RollOut(this);" runat="server">
									<asp:Literal ID="CancelLabel" runat="server" />
								</asp:LinkButton>
							</td>
							<td title="" class="button" id="image_cell_100" runat="server">
								<asp:LinkButton CssClass="primary saveButton" ID="image_link_100" OnClientClick="replaceSpecialCharacter();" OnClick="ucSaveButton_click" style="cursor:default;" onmouseout="HideTransString();RollOut(this);" runat="server">
									<asp:Literal ID="SaveLabel" runat="server" />
								</asp:LinkButton>
							</td>
							<td>
								<asp:Literal ID="uxHelpbutton" runat="server" />
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
		<div class="ektronPageContainer">
			<div class="settings">
				<asp:Label ID="lblName" runat="server" AssociatedControlID="txtName"></asp:Label>
				<asp:TextBox ID="txtName" runat="server"></asp:TextBox>
			</div>
			
			<div id="targetContentZoneWrapper" class="clearfix">
				<PB:DropZone ID="targetContentZone" AllowAddColumn="true" AllowColumnResize="true" runat="server" />
			</div>
		</div>
		
	</form>
</body>
</html>

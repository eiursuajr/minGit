
if ("undefined" == typeof Ektron.Widget) Ektron.Widget = {};
if ("undefined" == typeof Ektron.Widget.TargetedContent) Ektron.Widget.TargetedContent =
{
    instances: [],
    init: function (id, zoneCount) {
        var objInstance = Ektron.Widget.TargetedContent.instances[id];
        if (!objInstance) {
            objInstance = new Ektron.Widget.TargetedContent.Instance();
            Ektron.Widget.TargetedContent.instances[id] = objInstance;
        }

        objInstance.$wrapper = $ektron("#" + id);
        objInstance.$selectedZone = objInstance.$wrapper.find("> input[id*='SelectedZone']");
        objInstance.$columnsContainer = objInstance.$wrapper.find("> div.columns-container");
        objInstance.$conditionalZones = objInstance.$columnsContainer.find("> div.PBColumn");
        objInstance.$conditionalZoneLists = objInstance.$conditionalZones.find("> ul.columnwidgetlist");

        objInstance.zoneCount = zoneCount;
        objInstance.selectedZone = $ektron.toInt(objInstance.$selectedZone.val());

        objInstance.hideAllZones();

        if (objInstance.$columnsContainer.is("ui-sortable")) {
            objInstance.$columnsContainer.sortable("destroy");
        }
        objInstance.$columnsContainer.sortable(
	        {
	            items: "div.PBColumn",
	            handle: "> ul.columnwidgetlist > li.header",
	            containment: "parent",
	            forceHelperSize: true,
	            axis: "y",
	            cursor: "move",
	            delay: 200,
	            cancel: ".selectedZone",
	            stop: function (e, ui) {
	                var index = $ektron(ui.item).closest("div.PBColumn").index();
	                objInstance.$selectedZone.val(index);

	                var strZoneOrder = objInstance.$columnsContainer.sortable("serialize", { key: "zone", attribute: "id", expression: /(\d+)_zone/ });
	                // format is "zone=nn&zone=nn&zone=nn" where nn is the 0-based index
	                strZoneOrder = (new Ektron.String(strZoneOrder)).trim().replace("zone=", "").replace("&", ",").toString();
	                objInstance.$wrapper.find("> input[id*='ZoneOrder']").val(strZoneOrder);
	                objInstance.$wrapper.find(".targetedContent-changeZoneOrder").click();
	                // click of changeZoneOrder performs a postback
	            }
	        });

        objInstance.$conditionalZoneLists.find("> li.header").click(function () {
            var index = $ektron(this).closest("div.PBColumn").index();
            objInstance.showConditionalZone(index);
        });

        objInstance.showConditionalZone(objInstance.selectedZone);

        $ektron("div.targetContentListModal").dialog({ autoOpen: false, resizable: true });
        $ektron("div.targetContentListModal").parent().appendTo($("form:first"));

        return objInstance;
    },

    Instance: function () {
        this.zoneCount = 0;
        this.selectedZone = 0;
        this.showConditionalZone = Ektron.Widget.TargetedContent.p_Instance_showConditionalZone;
        this.hideAllZones = Ektron.Widget.TargetedContent.p_Instance_hideAllZones;
    },

    p_Instance_showConditionalZone: function (index) {
        var $conditionalZoneList = $ektron(this.$conditionalZoneLists[this.selectedZone]);
        $conditionalZoneList.find("> li.header").removeClass("selectedZone");
        $conditionalZoneList.find("> li.PBItem").hide();
        $conditionalZoneList.css("min-height", 0);

        this.selectedZone = index;
        this.$selectedZone.val(index);

        $conditionalZoneList = $ektron(this.$conditionalZoneLists[this.selectedZone]);
        $conditionalZoneList.css("min-height", '200px');
        $conditionalZoneList.find("> li.PBItem").show();
        $conditionalZoneList.find("> li.header").addClass("selectedZone");
    },

    p_Instance_hideAllZones: function () {
        this.$conditionalZoneLists.find("> li.header").removeClass("selectedZone");
        this.$conditionalZoneLists.find("> li.PBItem").hide();
        this.$conditionalZoneLists.css("min-height", 0);
    }
};

if ("undefined" == typeof Ektron.Widget.TargetedContentList) Ektron.Widget.TargetedContentList =
{
    init: function() {
        $ektron(document).bind("Ektron.Controls.ClientPaging.PageEvent", null, function(ev, data) {
            var url = $ektron("input.hdnTargetContentListUrl").val() + " div.targetContentListModal table.ektronGrid tbody";
            $ektron("div.targetContentListModal table.ektronGrid").load(url, data);
        });
    },
    showDialog: function(selectSourceControl) {
		$('.delete').click(function() 
		{
			$ektron("div.targetContentListModal:first").dialog('close');
		});
        var dialog = $ektron("div.targetContentListModal");
        dialog.parent().appendTo($("form:first"));

        $ektron("input.hdnTargetContentControlSource").val(selectSourceControl);
        $ektron("div.targetContentListModal:first").dialog('open');

    },
    select: function(id) {
        $ektron("input.hdnTargetContentSelectId").val(id);
        return false;
    }
};

Ektron.ready(function() {
    Ektron.Widget.TargetedContentList.init();
});

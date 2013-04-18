//Define Ektron.PageBuilder object only if it's not already defined
if (Ektron.PageBuilder === undefined) {
	Ektron.PageBuilder = {};
}

Ektron.ready(function() {
    if (Ektron.PBSettings !== undefined && Ektron.PBSettings.dontClose) {
        var img = $ektron("div.EktronPersonalizationWrapper img.menuToggleImg")[0];
        img.src = img.src.replace("menuhandled_pullouthover", "menuhandled_putback_hover");
        $ektron("div.EktronPersonalizationWrapper .MenuToggle").addClass("MenuOpened");
    } else {
        Ektron.PageBuilder.WidgetTray.menuClose();
    }

    Ektron.PageBuilder.WidgetTray.initScroller();

    $ektron("li.widgetToken").hover(
        function() { $ektron(this).addClass("hover"); },
        function() { $ektron(this).removeClass("hover"); }
    );

    $ektron("div.topmenuitem").hover(
        function() {
            //over

            //get marker
            var marker = $ektron(this);
            var markerOffset = marker.offset();
            var menu = $ektron(this).children("ul");

            //get marker dimensions
            var markerTop = markerOffset.top;
            var markerRight = markerOffset.left + marker.width();
            var markerBottom = markerOffset.top + marker.height();
            var markerLeft = markerOffset.left;

            //set menu position defaults
            var menuTop = markerBottom;
            var menuLeft = markerLeft;

            // set menu location
            //set menu location
            menu.css("top", menuTop);
            menu.css("left", menuLeft);

            // reveal the menu
            menu.fadeIn(100);

            // add class "hover"
            marker.addClass("menuHover");
        },
        function() {
            //out
            $ektron(this).removeClass("menuHover").children("ul").fadeOut(100);
        }
    );

    // remove disabled=disabled from menu links so IE doesn't mess with how it looks
    $ektron("div.topmenuitem").find("ul li a").removeAttr("disabled");

    if ($ektron("div.EktronPersonalizationWrapper div#widgetlist").children().length === 0) {
        $ektron("div.pullchain").hide();
    }

    $ektron(".topmenuinputbox").keyup(function(evt) {
        if ($ektron("div.pullchain:visible").length > 0) {
            Ektron.PageBuilder.WidgetTray.trayOpen();
            var str = this.value;
            if (typeof str == "undefined") {
                str = "";
            }
            str = str.replace("'", "");
            str = str.toLowerCase();
            var widgets = $ektron("li.widgetToken span");
            $ektron("ul.ektronPersonalizationWidgetList").animate({ left: "0px" }, 150);

            widgets.each(function(item) {
                var widget = $ektron(widgets[item]);
                var title = widget.html();

                if (title.toLowerCase().search(str) != -1) {
                    widget.parent().show(350);
                }
                else {
                    widget.parent().hide(350);
                }
            });
        }
    });

    Ektron.PageBuilder.WidgetHost.refreshSortables();
    Ektron.PageBuilder.WidgetHost.initColHeaders();
    Ektron.PageBuilder.WidgetHost.initMasterZones();
    Ektron.PageBuilder.WidgetTray.menuTackInit();
});

//Ektron PageBuilder Object
Ektron.PageBuilder.WidgetTray = {
    trayVisible: false,
    isResizing: false,
    menuOpen: true,
    toggleTray: function () {
        if (this.trayVisible) {
            this.trayClose();
        } else {
            this.trayOpen();
        }
    },
    closeAll: function () {
        if (this.trayVisible) {
            $ektron("div.EktronPersonalizationWrapper div#widgetlist > a").fadeOut(250, function () {
                $ektron("div.EktronPersonalizationWrapper div#widgetlist").slideUp(250, function () {
                    Ektron.PageBuilder.WidgetTray.menuClose();
                });
            });
            $ektron("div#toggleTray").removeClass("opened");
            this.trayVisible = false;
        }
        else if (this.menuOpen) {
            Ektron.PageBuilder.WidgetTray.menuClose();
        }
    },
    trayClose: function () {
        if (this.trayVisible === true) {
            $ektron("div.EktronPersonalizationWrapper div#widgetlist > a").fadeOut(250, function () {
                $ektron("div.EktronPersonalizationWrapper div#widgetlist").slideUp(250);
            });
            $ektron("div#toggleTray").removeClass("opened");
            this.trayVisible = false;
        }
    },
    trayOpen: function () {
        if (this.trayVisible === false) {
            var scrollLinks = $ektron("div.EktronPersonalizationWrapper div#widgetlist > a");
            scrollLinks.hide();  // hide the scroll links if they aren't already hidden
            $ektron("div.EktronPersonalizationWrapper div#widgetlist").slideDown(250, function () {
                scrollLinks.fadeIn(250);
            });
            $ektron("div#toggleTray").addClass("opened");
            this.trayVisible = true;
        }
    },
    initScroller: function () {
        var el = $ektron("div#widgetlist ul li");
        var onewidth = 0;
        if (el !== null) {
            var outerwidth = parseInt(el.css("margin-left"), 10) + parseInt(el.css("padding-left"), 10) + 2;
            onewidth = parseInt(el.css("width"), 10) + outerwidth * 2;
        }
        var barwidth = onewidth * $ektron(".widgetToken").length;
        $ektron(".ektronPersonalizationWidgetList").width(barwidth + "px");
    },
    scrollLeft: function () {
        var onePage = $ektron("div#widgetlist").width();
        var list = $ektron("ul.ektronPersonalizationWidgetList");
        var width = list.width();
        var newpos = (list.offset().left) - (onePage * .7);
        //don't go past end
        if (width < onePage - newpos) {
            newpos = onePage - width;
        }
        $ektron("ul.ektronPersonalizationWidgetList").animate({ left: newpos + "px" }, 150);
    },
    scrollRight: function () {
        var onePage = $ektron("div#widgetlist").width();
        var list = $ektron("ul.ektronPersonalizationWidgetList");
        var width = list.width();
        var newpos = (list.offset().left) + (onePage * .7);
        //don't go past end
        if (newpos > 0) {
            newpos = 0;
        }
        $ektron("ul.ektronPersonalizationWidgetList").animate({ left: newpos + "px" }, 150);
    },
    menuToggle: function () {
        var pers = $ektron("div.EktronPersonalizationWrapper");
        var body = $ektron("body");
        if (this.menuOpen) {
            if (this.trayVisible) {
                this.closeAll();
            }
            else {
                this.menuClose(true);
            }
        } else {
            pers.animate({ left: "0px" }, 250, 'swing');
            var img = $ektron("div.EktronPersonalizationWrapper img.menuToggleImg")[0];
            img.src = img.src.replace("menuhandled_pullouthover", "menuhandled_putback_hover");
            $ektron("div.EktronPersonalizationWrapper .MenuToggle").addClass("MenuOpened");
            Ektron.PageBuilder.WidgetTray.menuOpen = true;
        }
    },
    menuTackInit: function () {
        var tacked = ($ektron.cookie && $ektron.cookie("PageBuilderMenuTacked") == "true");
        if (tacked) {
            var img = $ektron("div.EktronPersonalizationWrapper img.menuTackImg");
            img.addClass("tacked");
            img[0].src = img[0].src.replace("thumbtack_out", "thumbtack_in");
            var menutoggle = $ektron("div.EktronPersonalizationWrapper .MenuToggle");
            menutoggle.addClass("MenuOpened");
            img = $ektron("div.EktronPersonalizationWrapper img.menuToggleImg")[0];
            img.src = img.src.replace("menuhandled_pullouthover", "menuhandled_putback_hover");
            Ektron.PageBuilder.WidgetTray.menuOpen = true;
        }
    },
    menuTack: function () {
        var img = $ektron("div.EktronPersonalizationWrapper img.menuTackImg");
        img.toggleClass("tacked");
        if (img.hasClass("tacked")) {
            img[0].src = img[0].src.replace("thumbtack_out", "thumbtack_in");
            if ($ektron.cookie) { $ektron.cookie("PageBuilderMenuTacked", "true", { path: '/' }); }
        } else {
            img[0].src = img[0].src.replace("thumbtack_in", "thumbtack_out");
            if ($ektron.cookie) { $ektron.cookie("PageBuilderMenuTacked", null, { path: '/' }); }
        }

    },
    menuClose: function (force) {
        //force close the menu if the user clicks on the close directly, without clicking on the tack first
        force == ("undefined" == typeof (force)) ? false : force;
        if (force) {
            Ektron.PageBuilder.WidgetTray.menuTack();
        }

        var tacked = ($ektron.cookie && $ektron.cookie("PageBuilderMenuTacked") == "true");

        if ((this.menuOpen && !tacked)) {
            var pers = $ektron("div.EktronPersonalizationWrapper");
            var body = $ektron("body");
            pers.animate({ left: (0 - body.width()) }, 500, 'swing');
            var img = $ektron("div.EktronPersonalizationWrapper img.menuToggleImg")[0];
            img.src = img.src.replace("menuhandled_putback_hover", "menuhandled_pullouthover");
            $ektron("div.EktronPersonalizationWrapper .MenuToggle").removeClass("MenuOpened");
            this.menuOpen = false;
        }
    }
};

Ektron.PageBuilder.WidgetHost = {
    dropHandler: function (ui) {
        var action;
        var srcDropZoneID;
        var srcColumnID;
        var srcSortOrder;
        var newDropZoneID;
        var newColumnID;
        var widgettype;
        var newSortOrder = $ektron(ui.item.parent().children()).index(ui.item) - 1;

        //first revert from the dropzone style back to standard list item
        var rev = $ektron("li.DropArea");
        rev.removeClass("DropArea");
        rev.children().css("visibility", "");
        rev.children("span.remove").remove();

        if ((ui.item !== null) && ui.item.hasClass("widgetToken")) {
            action = "add";
            srcDropZoneID = "";
            srcColumnID = 0;
            srcSortOrder = 0;
            srcColumnGuid = "00000000-0000-0000-0000-000000000000";

            srcIsNested = false;
            srcNestedSortOrder = 0;

            newDropZoneID = ui.item.parents("[dropzoneid]").attr("dropzoneid");
            newColumnID = ui.item.parents("[columnid]").attr("columnid");
            newColumnGuid = ui.item.parents("[columnguid]").attr("columnguid");

            //rebuild widgetlist
            var tmp = ui.item.clone();
            var widgetlist = $ektron("ul.ektronPersonalizationWidgetList").children();
            var myid = parseInt(tmp[0].id, 10);
            if (widgetlist.length === 0) {
                $ektron("ul.ektronPersonalizationWidgetList").append(tmp);
            }
            else if (parseInt(widgetlist[widgetlist.length - 1].id, 10) < myid) {
                //insert at end
                tmp.insertAfter("li#" + widgetlist[widgetlist.length - 1].id);
            }
            else {
                //find place to insert
                for (var i = 0; i < widgetlist.length; i++) {
                    if (parseInt(widgetlist[i].id, 10) > myid) {
                        tmp.insertBefore("li#" + widgetlist[i].id);
                        break;
                    }
                }
            }
            widgettype = parseInt(ui.item[0].id, 10);
        } else if (ui.item.parents("[dropzoneid]").length > 0) {
            action = "move";
            srcDropZoneID = Ektron.PageBuilder._srcContainer.parents("[dropzoneid]").attr("dropzoneid");
            srcColumnID = Ektron.PageBuilder._srcContainer.attr("columnid");
            srcColumnGuid = Ektron.PageBuilder._srcContainer.attr("columnguid");

            //srcIsNested = ui.element.parent().hasClass("nested");            
            //if(srcIsNested)
            //{
            //    srcNestedSortOrder = ui.item.attr("sort");
            //    srcSortOrder = ui.item.attr("sort");//$ektron(ui.element.parents(".PBItem")).attr("sort");
            //}
            //else
            //{
            srcSortOrder = ui.item.attr("sort");
            //}

            newDropZoneID = ui.item.parents("[dropzoneid]").attr("dropzoneid");
            newColumnID = ui.item.parents("[columnid]").attr("columnid");
            newColumnGuid = ui.item.parents("[columnguid]").attr("columnguid");
            widgettype = $ektron(ui.item[0]).find("div[widget-type-id]").attr("widget-type-id");
            widgettype = parseInt(widgettype, 10);
        } else {
            return;
        }

        command = {
            "Action": action,
            "OldWidgetLocation": {
                "isNested": false,
                "nestedSortOrder": 0,
                "widgetTypeID": widgettype,
                "dropZoneID": srcDropZoneID,
                "ColumnID": srcColumnID,
                "columnGuid": srcColumnGuid,
                "OrderID": srcSortOrder
            },
            "NewWidgetLocation": {
                "isNested": false,
                "nestedSortOrder": 0,
                "widgetTypeID": widgettype,
                "dropZoneID": newDropZoneID,
                "ColumnID": newColumnID,
                "columnGuid": newColumnGuid,
                "OrderID": newSortOrder
            }
        };

        //update personalization
        if (typeof ui.item.parents("[dropzoneid]")[0] != "undefined") {
            if (ui.item !== null && ui.item.parents("[dropzoneid]") !== null && ui.item.parents("[dropzoneid]")[0].id !== null) {
                var newpostloc = ui.item.parents("[dropzoneid]")[0].id.replace(/_/g, "$");
                __doPostBack(newpostloc, Ektron.JSON.stringify(command));
            }
        }
    },
    RemoveColumn: function (el) {
        command = {
            "Action": "RemoveColumn",
            "OldWidgetLocation": {
                "widgetTypeID": "0",
                "dropZoneID": "",
                "ColumnID": "0",
                "OrderID": "0"
            },
            "NewWidgetLocation": {
                "widgetTypeID": "0",
                "dropZoneID": $ektron(el).parents("[dropzoneid]").attr("dropzoneid"),
                "ColumnID": $ektron(el).parents("[dropzoneid]").children("[columnid]").attr("columnid"),
                "OrderID": "0"
            }
        };

        var postloc = $ektron(el).parents("[dropzoneid]")[0].id.replace(/_/g, "$");
        __doPostBack(postloc, Ektron.JSON.stringify(command));
    },
    initMasterZones: function () {
        if (Ektron.PBMasterSettings.isMasterLayout) { //if it's a master page, then find all dropzones not marked master and lock them.
            if ($ektron.blockUI) {
                $ektron("div.dropzone").unblock();
                var layoutzones = $ektron("div.dropzone > div.PBColumn").parents("div.dropzone:not(.isMasterZone)");
                layoutzones.block({
                    message: '<a href="#" title="Unlock - Set as Master Zone" onclick="Ektron.PageBuilder.WidgetHost.setMasterDropZone(this);return false;"><img style="border:none;" alt="Unlock - Set as Master Zone" src="' + Ektron.PBMasterSettings.pathToLock + '"></a>',
                    css: { border: 'none', cursor: 'auto' },
                    overlayCSS: { cursor: 'auto' },
                    baseZ: 2990
                });
            }
        }
    },
    BlockUI: function () {
        //close any expanded widgets hanging around
        $ektron(".ektronWindow").modalHide();

        //close open menus
        $ektron("div.setSize").slideUp(100, function () { $ektron(this).remove(); });
        $ektron("div.setMaster").slideUp(100, function () { $ektron(this).remove(); });

        if ($ektron.blockUI) {
            if ($ektron("div.PBBlockUI").length < 1) {
                $ektron("body").prepend("<div class='PBBlockUI'></div>");
            }
            if (Ektron.PBMasterSettings.isMasterLayout) {
                $ektron("div.dropzone").unblock();
            }
            $ektron.blockUI({ message: $ektron("div.PBBlockUI"), css: { width: 'auto', left: '50%', top: '50%', border: 'none' }, overlayCSS: { opacity: '0.95' }, fadeIn: '250', fadeOut: '250', baseZ: 2990 });
        }
    },
    unBlockUI: function () {
        Ektron.PageBuilder.WidgetHost.initColHeaders();
        Ektron.PageBuilder.WidgetHost.initMasterZones();
        if ($ektron.blockUI) {
            $ektron.unblockUI();
        }

        var toOpen = $ektron(".OpeninModal"); //open any widgets that should autoopen in modals
        toOpen.removeClass("OpeninModal");
        if (toOpen.length > 0) {
            Ektron.PageBuilder.WidgetHost.openAsModal(toOpen[0]);
        }

        if (Ektron.PageBuilder.WidgetTray) {
            Ektron.PageBuilder.WidgetTray.trayClose();
        }
        //refresh sortables if any
        Ektron.PageBuilder.WidgetHost.refreshSortables();
    },
    initColHeaders: function () {
        $ektron("div.PBColumn ul.columnwidgetlist li.header > span").addClass("position");
    },
    setMasterDropZone: function (el) {
        var pageBody = $ektron("body");
        var dropzone = $ektron(el).parents("div.dropzone");
        var PBColumn = dropzone.find("div.PBColumn");
        var dropzoneId = PBColumn.attr("dropzoneid");
        var elId = PBColumn.attr("id");
        var isMasterZone = dropzone.hasClass("isMasterZone");
        var msg;

        if (!isMasterZone) {
            msg = "Warning!\r\n Unlocking this will overwrite any column or widget data in the current dropzone that is based on this master layout. Click OK to unlock this dropzone.";
            if (confirm(msg)) {
                Ektron.PageBuilder.WidgetHost.SetZoneType(el, true);
            }
        } else {
            msg = "Warning!\r\n Locking this dropzone deletes columns and widgets on templates based on this master layout. Click OK to lock this dropzone.";
            if (confirm(msg)) {
                Ektron.PageBuilder.WidgetHost.SetZoneType(el, false);
            }
        }
        return false;
    },
    SetZoneType: function (el, isMasterZone) {
        var dropzone = $ektron(el).parents("div.dropzone");
        var PBColumn = dropzone.find("div.PBColumn");
        var dzoneid = PBColumn.attr("dropzoneid");
        var postbackid = PBColumn.attr("id");

        command = {
            "Action": "SaveDZType",
            "DropzoneInfo":
            {
                "dropZoneID": dzoneid,
                "isMaster": isMasterZone
            }
        };
        var postloc = postbackid.replace(/_/g, "$");
        __doPostBack(postloc, Ektron.JSON.stringify(command));
    },
    toggleDropZones: function () {
        var el = $ektron("div.PBColumn ul.columnwidgetlist");
        el.toggleClass("highlight");
    },
    refreshSortables: function () {
        if ($ektron("div.PBColumn").length > 0) {
            $ektron("input.PBAddColumn").hover(
                function () { this.src = this.src.replace("addcolumn_off.png", "addcolumn_on.png"); },
                function () { this.src = this.src.replace("addcolumn_on.png", "addcolumn_off.png"); }
            );
            $ektron("img.PBclosebutton").hover(
                function () { this.src = this.src.replace("icon_close.png", "icon_close_hover.png"); },
                function () { this.src = this.src.replace("icon_close_hover.png", "icon_close.png"); }
            );
            $ektron("img.PBeditbutton").hover(
                function () { this.src = this.src.replace("edit_off.png", "edit_on.png"); },
                function () { this.src = this.src.replace("edit_on.png", "edit_off.png"); }
            );
            $ektron("img.PBMasterbutton").hover(
                function () { this.src = this.src.replace("lock_off.png", "lock_on.png"); },
                function () { this.src = this.src.replace("lock_on.png", "lock_off.png"); }
            );
            $ektron("img.PBexpandbutton").hover(
                function () { this.src = this.src.replace("icon_expand.png", "icon_expand_over.png"); },
                function () { this.src = this.src.replace("icon_expand_over.png", "icon_expand.png"); }
            );
            $ektron("img.PBhelpbutton").hover(
                function () { this.src = this.src.replace("icon_help.png", "icon_help_over.png"); },
                function () { this.src = this.src.replace("icon_help_over.png", "icon_help.png"); }
            );
            $ektron("img.PBAddColumn").hover(
                function () { this.src = this.src.replace("addcolumnbutton.png", "addcolumnbuttonon.png"); },
                function () { this.src = this.src.replace("addcolumnbuttonon.png", "addcolumnbutton.png"); }
            );


            $ektron('div.PBColumn ul.columnwidgetlist').each(function (i, el) {
                $ektron(el).children().each(function (i, inel) {
                    $ektron(inel).attr("sort", i - 1); //subtract 1 because header is an li
                }
                );
            });

            //to support multivariate sections that have nested widgets, add nonsortable class to descendant column widget lists (if necessary)
            //first, match ul.columnwidgetlist that are descendants of PBColumn and a div that is multi-classed "PBViewing" AND "PBNonsortable"
            //second, add class PBNonsortable to the matching ul.columnwidgetlist elements
            //this restricts the "connectWithEls" var to NOT include such non-sortables
            $ektron(".PBColumn .PBViewing.PBNonsortable ul.columnwidgetlist").addClass("PBNonsortable");
            var connectWithEls = 'div.PBColumn:not(.PBNonsortable) ul.columnwidgetlist:not(.PBNonsortable)';
            if (Ektron.PBMasterSettings.isMasterLayout) {
                connectWithEls = 'div.isMasterZone div.PBColumn:not(.PBNonsortable) ul.columnwidgetlist';
            }


            $ektron(".PBNonsortable li.PBItem:not(.header)").addClass("PBNonsortable");

            if ($ektron("li.PBItem").is("ui-sortable"))
            {
                $ektron("li.PBItem").sortable("destroy");
            }
            $ektron(connectWithEls).sortable({
                connectWith: [connectWithEls],
                items: 'li.PBItem:not(.header, .PBNonsortable)',
                zIndex: 99999,
                scroll: true,
                cursor: "move",
                handle: 'div.header:not(.PBNonsortable li.PBItem div.header)',
                tolerance: 'intersect',
                distance: 3,
                cursorAt: {
                    top: 5,
                    left: 5
                },
                placeholder: 'PBHighlight widget50px',
                //forcePlaceholderSize: true,
                //type:"semi-dynamic",
                /*change: function(e, el) {
                if (!el.item.hasClass("DropArea")) {
                el.item.addClass("DropArea");
                el.item.css("visibility", "");
                el.item.children().css("visibility", "hidden");
                el.item.append("<span class='remove'>" + Ektron.ResourceText.PageBuilder.WidgetTray.dropControlHere + "</span>");
                }
                },*/
                helper: function (e, el) {
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
                start: function (Event, ui) {
                    Ektron.PageBuilder._srcContainer = ui.item.parent();
                },
                stop: function (Event, ui) {
                    Ektron.PageBuilder.WidgetHost.dropHandler(ui);
                    Ektron.PageBuilder.WidgetHost.createResizables();
                    Ektron.PageBuilder.WidgetHost.toggleDropZones();
                }
            });

            if ($ektron("ul.ektronPersonalizationWidgetList").is("ui-sortable")) {
                $ektron("ul.ektronPersonalizationWidgetList").sortable("destroy");
            }
            $ektron("ul.ektronPersonalizationWidgetList").sortable({
                items: ".widgetToken",
                connectWith: [connectWithEls],
                opacity: ".8",
                revert: true,
                zIndex: 99999,
                scroll: true,
                cursor: "move",
                distance: 3,
                tolerance: 'intersect',
                placeholder: 'PBHighlight widget50px',
                //forcePlaceholderSize: true,
                //type:"semi-dynamic",
                helper: function (e, el) {
                    Ektron.PageBuilder.WidgetTray.closeAll();
                    Ektron.PageBuilder.WidgetHost.toggleDropZones();
                    Ektron.PageBuilder.WidgetHost.destroyResizables();
                    var helper = $ektron(el).clone();
                    helper.addClass("ektronPBWidgetTokenDrag");
                    helper.prependTo("body");
                    return helper;
                },
                start: function (Event, ui) {
                    Ektron.PageBuilder._srcContainer = ui.item.parent();
                },
                stop: function (Event, ui) {
                    if ($ektron(ui.item).parents("ul.widgetList").length === 0) {
                        Ektron.PageBuilder.WidgetHost.dropHandler(ui);
                        Ektron.PageBuilder.WidgetHost.createResizables();
                    }
                    Ektron.PageBuilder.WidgetHost.toggleDropZones();
                }
            });

            Ektron.PageBuilder.WidgetHost.destroyResizables();
            Ektron.PageBuilder.WidgetHost.createResizables();
        }
    },
    destroyResizables: function () {
        if ($ektron("div.dropzone div.PBColumn[resizable='true'] > ul.columnwidgetlist").parent("div.PBColumn[resizable=true]").is("ui-resizable")) {
            $ektron("div.dropzone div.PBColumn[resizable='true'] > ul.columnwidgetlist").parent("div.PBColumn[resizable=true]").resizable("destroy");
        }
    },
    createResizables: function () {

        var othercolumns = $ektron("div.dropzone div.PBColumn[resizable='true'] > ul.columnwidgetlist[columnid]").parent("div.PBColumn[resizable='true']");
        if (othercolumns.length > 0) {
            if (othercolumns.is("ui-resizable")) {
                othercolumns.resizable("destroy");
            }
            othercolumns.resizable(
                {
                    handles: "e",
                    minWidth: 50,
                    autoHide: true,
                    stop: function (e, ui) {
                        Ektron.PageBuilder.WidgetHost.resizeHandler(ui);
                    }
                }
            );
        }
    },

    resizeHandler: function (el) {
        command = {
            "Action": "ResizeColumn",
            "NewWidgetLocation": {
                "dropZoneID": $ektron(el.element[0]).attr("dropzoneid"),
                "ColumnID": $ektron(el.element[0]).children("ul.columnwidgetlist").attr("columnid"),
                "Width": el.size.width,
                "Unit": "px"
            }
        };

        var postloc = el.element[0].id.replace(/_/g, "$");
        __doPostBack(postloc, Ektron.JSON.stringify(command));
    },
    resizeColumn: function (el) {
        var pageBody = $ektron("body");
        var PBColumn = $ektron(el).parents("div.PBColumn");
        var dropzoneId = PBColumn.attr("dropzoneid");
        var columnId = PBColumn.attr("id");
        if (pageBody.children("div.setSize[columnid='" + columnId + "']").length === 0) {
            var string = "<div class='setSize' dropzoneid='" + dropzoneId + "' columnid='" + columnId + "'>";
            string += Ektron.ResourceText.PageBuilder.WidgetTray.newWidth + ": <input class='newwidth' type='text' onkeypress='var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which; return /^([.\\d\\t\\x08])$\/.test(String.fromCharCode(k) );' \/>";
            string += "    <select class='setUnit'>";
            string += "      <option value='px'>" + Ektron.ResourceText.PageBuilder.WidgetTray.pixels + "</option>";
            string += "      <option value='em'>" + Ektron.ResourceText.PageBuilder.WidgetTray.em + "</option>";
            string += "      <option value='%'>" + Ektron.ResourceText.PageBuilder.WidgetTray.percent + "</option>";
            string += "    </select>&nbsp;";
            string += "    <a href='#' onclick='Ektron.PageBuilder.WidgetHost.ManualResize(this, true); return false;'>" + $ektron.trim(Ektron.ResourceText.PageBuilder.WidgetTray.save) + "</a> ";
            string += "<a href='#' onclick='Ektron.PageBuilder.WidgetHost.ManualResize(this, false); return false;'>" + $ektron.trim(Ektron.ResourceText.PageBuilder.WidgetTray.cancel) + "</a>";
            string += "</div>";
            pageBody.prepend(string);


            var setSize = pageBody.find("div.setSize[columnid='" + columnId + "']");
            var header = PBColumn.find("li.header");
            var columnPosition = PBColumn.offset();
            var headerLeft = parseInt(columnPosition.left, 10);
            var headerRight = headerLeft + parseInt(PBColumn.outerWidth(), 10);
            var headerTop = parseInt(columnPosition.top, 10);
            var headerBottom = headerTop + parseInt(header.height(), 10);

            setSize.css("left", headerLeft + "px").css("top", headerTop + parseInt(header.height(), 10) + "px");

            setSize.slideDown(500);
            // line up on the left (default behavior) unless...
            if (headerRight - parseInt(setSize.outerWidth(), 10) >= 0) {
                // line up on the right
                setSize.css("left", headerLeft + (PBColumn.outerWidth() - parseInt(setSize.outerWidth(), 10)) + "px");
            }
        }
    },

    ManualResize: function (el, savebool) {
        var parent = $ektron(el).parents("div.setSize");
        var newwidth = parent.children("input.newwidth")[0].value;
        var newunit = parent.children("select")[0].value;
        var dzoneid = parent.attr("dropzoneid");
        var dzone = $ektron("#" + parent.attr("columnid"));
        var columnid = dzone.children("ul.columnwidgetlist").attr("columnid");

        if (savebool) {
            command = {
                "Action": "ResizeColumn",
                "NewWidgetLocation":
                {
                    "dropZoneID": dzoneid,
                    "ColumnID": columnid,
                    "Width": newwidth,
                    "Unit": newunit
                }
            };
            var postloc = dzone[0].id.replace(/_/g, "$");
            __doPostBack(postloc, Ektron.JSON.stringify(command));
        }
        $ektron(el).parents("div.setSize").slideUp(500, function () { $ektron(this).remove(); });
    },
    openAsModal: function (el) {
        var modalcontent = $ektron(el).parents("div.header").parent().children("div.content");
        var open = false;
        var originalstyles = {
            "margin-left": modalcontent.css("margin-left"),
            "margin-top": modalcontent.css("margin-top"),
            "z-index": modalcontent.css("z-index"),
            "padding-top": modalcontent.css("padding-top"),
            "padding-bottom": modalcontent.css("padding-bottom"),
            "padding-left": modalcontent.css("padding-left"),
            "padding-right": modalcontent.css("padding-right")
        };
        var originallocation = modalcontent.parent();
        modalcontent.prependTo("form");
        modalcontent.modal({
            modal: true,
            onShow: function (hash) {
				hash.w.children().wrapAll("<div class=\"ektronModalBody widgetModalDraggable\" style=\"max-height:465px; height:465px; height:auto !important; overflow:auto;\"></div>");
                hash.w.children(".ektronModalBody").before("<div class=\"ektronModalHeader\"><h3><span>Editing Widget</span></h3></div>");
                hash.w.addClass("ektronWindow").addClass("ektronModalStandard widgetModalDraggable");
                hash.w.css("width", "45em");
                hash.w.css("margin-left", -1 * Math.round(hash.w.outerWidth() / 2)); //recenter
                var margin = Math.round(hash.w.outerHeight() / 2);
                if (margin > 200) { margin = 200; }
                hash.w.css("margin-top", -1 * margin);
                hash.w.css("padding", "0 0 0 0");
                hash.w.css("z-index", "9998");


                var showit = function () {
                    var recenterme = function () {
                        if (hash.w.parent()[0].tagName.toLowerCase() == "form" && open) {
                            var newleftmargin = -1 * Math.round(hash.w.outerWidth() / 2);
                            var newtopmargin = Math.round(hash.w.outerHeight() / 2);
                            if (newtopmargin > 200) { newtopmargin = 200; }
                            if (hash.w.css("margin-left") != newleftmargin || hash.w.css("margin-top") != newtopmargin) {
                                hash.w.animate({
                                    marginLeft: newleftmargin,
                                    marginTop: -newtopmargin
                                }, 300);
                                setTimeout(recenterme, 350);
                            } else {
                                setTimeout(recenterme, 30);
                            }
                        }
                    };
                    hash.w.fadeIn("slow");
                    setTimeout(recenterme, 30);
                };
                setTimeout(showit, 200);
                open = true;
            },
            onHide: function (hash) {
                open = false;
                hash.w.fadeOut("slow", function () {
                    hash.w.appendTo(originallocation);
                    hash.w.removeClass("ektronWindow").removeClass("ektronModalStandard");
                    hash.w.find(".ektronModalHeader").remove();
                    hash.w.find(".ektronModalBody").replaceWith(function () {
                        $ektron(this).contents();
                    });
                    setTimeout(function () {
                        hash.w.removeAttr("style");
                        hash.w.css("margin-left", originalstyles["margin-left"]);
                        hash.w.css("margin-top", originalstyles["margin-top"]);
                        hash.w.css("padding-top", originalstyles["padding-top"]);
                        hash.w.css("padding-bottom", originalstyles["padding-bottom"]);
                        hash.w.css("padding-left", originalstyles["padding-left"]);
                        hash.w.css("padding-right", originalstyles["padding-right"]);
                        hash.w.show();
                        if (hash.o) { hash.o.remove(); }
                    }, 1);
                });
            }
        });
        modalcontent.draggable({ cancel: ".ektronModalBody" });
		modalcontent.modalShow();
    },
    Help: function (url) {
        var helpmodal = $ektron("#widgethelp");
        if (helpmodal.length === 0) {
            helpmodal = "<div id=\"widgethelp\" class=\"ektronWindow ektronModalStandard\">";
            helpmodal += "<div class=\"ektronModalHeader\"><h3><span>Widget Help</span><a href=\"#\" class=\"ektronModalClose\">Close</a></h3></div>";
            helpmodal += "<div class=\"ektronModalBody\"><iframe style=\"z-index:-1; width:100%; border:0px;\" id=\"WidgetHelpIframe\" src=\"" + url + "\"></iframe></div></div>";
            $ektron("body").append(helpmodal);
            helpmodal = $ektron("#widgethelp");
        }

        helpmodal.modal({
            modal: true,
            target: '#WidgetHelpIframe',
            onHide: function (hash) {
                hash.w.fadeOut('2000', function () {
                    hash.o.remove();
                    helpmodal.remove();
                });
            },
            onShow: function (hash) {
                var $trigger = $ektron(hash.t);
                var $modalWindow = $ektron(hash.w);
                var $iframe = $modalWindow.find('iframe');
                $iframe.html('').attr('src', url);
                //grow height
                $iframe.parent().css({ height: '350px' });
                $iframe.css({ height: '100%' });
                //recenter
                $modalWindow.css("margin-top", -1 * Math.round($modalWindow.outerHeight() / 2));
                $modalWindow.css("margin-left", -1 * Math.round($modalWindow.outerWidth() / 2));
                hash.w.fadeIn('2000');
            },
            toTop: true
        }).modalShow();
    }
};

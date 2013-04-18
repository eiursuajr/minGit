//Define Ektron.Personalization object only if it's not already defined
if (Ektron.Personalization === undefined) {
    Ektron.Personalization = {};
}

//Ektron Personalization Object
Ektron.Personalization = {
    //properties
    resizeTimeoutId: -1,

    //methods
    bindEvents: function () {
        //resize table on window.resize
        $ektron(window).bind("resize", function () {
            Ektron.Personalization.Columns.resize();
        });

        //show remove tab button
        $ektron("div.EktronPersonalization div.tabWrapper ul.tabs li.tab").hover(
                  function () {
                      $ektron(this).addClass("hover");
                      $ektron(this).children("a.remove").fadeIn("normal");
                  },
                  function () {
                      $ektron(this).children("a.remove").fadeOut("fast", function () {
                          $ektron(this).parent().removeClass("hover");
                      });
                  }
            );

        //highlight widget tokens on hover
        $ektron("div.EktronPersonalization ul.widgetList li.widgetToken").hover(
                  function () {
                      $ektron(this).addClass("hover");
                  },
                  function () {
                      $ektron(this).removeClass("hover");
                  }
            );

        //show red remove column image on hover over remove link
        $ektron("div.EktronPersonalization td.column a.removeColumn").hover(
                  function () {
                      $ektron(this).children("img.normal").css("display", "none");
                      $ektron(this).children("img.hover").css("display", "block");
                      $ektron(this).addClass("hover");
                  },
                  function () {
                      $ektron(this).children("img.normal").css("display", "block");
                      $ektron(this).children("img.hover").css("display", "none");
                      $ektron(this).removeClass("hover");
                  }
            );

        //show menu when mouseover marker
        $ektron("div.EktronPersonalization ul.tabs li.tabOptions a.tabOptions").bind("click", function (e) {
            Ektron.Personalization.OptionsMenu.show(false);
        });
        $ektron("div.EktronPersonalization ul.tabs li.tabOptions a.tabOptions").bind("keypress", function (e) {
            if (e.keyCode === 13)  //show only if pressed key is 'enter'
                Ektron.Personalziation.OptionsMenu.show(false);
        });

        //hide menu on timeout when mouseout menu
        $ektron("div.EktronPersonalization ul.tabOptions, div.EktronPersonalization li.tabOptions").bind("mouseleave", function (e) {
            window.clearTimeout(Ektron.Personalization.OptionsMenu.timeoutId);
            Ektron.Personalization.OptionsMenu.timeoutId = setTimeout(function () {
                Ektron.Personalization.OptionsMenu.hide();
            }, Ektron.Personalization.OptionsMenu.timeoutDuration);
        });

        //clear timeout when mouseover menu item
        $ektron("div.EktronPersonalization ul.tabOptions, div.EktronPersonalization ul.tabOptions *, div.EktronPersonalization li.tabOptions, div.EktronPersonalization li.tabOptions *").bind("mouseenter", function (e) {
            window.clearTimeout(Ektron.Personalization.OptionsMenu.timeoutId);
        });
    },
    BlockUi: {
        init: function () {
            Ektron.Personalization.BlockUi.unblock();
        },
        unblock: function () {
            setTimeout(function () {
                $ektron("div.EktronPersonalizationWrapper").unblock();
            }, 500);
        },
        block: function () {
            $ektron.blockUI.defaults.css = {};
            $ektron("div.EktronPersonalizationWrapper").block();
        }
    },
    Columns: {
        init: function () {
            var numCols = $ektron("div.EktronPersonalization table.ektronWidgetPage td.column").length; //3
            var numWidgets = $ektron("div.EktronPersonalization table.ektronWidgetPage div.widget").length;
            var width = $ektron("div.EktronPersonalization p.widgetTrayHandle").width();
            var tableWidth = parseInt($ektron("div.EktronPersonalization").width(), 10); //750

            //set width on each column
            var calculatedWidth = (tableWidth / numCols); //233.333
            var actualWidth = Math.floor(calculatedWidth); //233
            var error = calculatedWidth - actualWidth; //.333
            var compensationFrequency = -1;
            if (error > 0) {
                compensationFrequency = Math.ceil(1 / error); //3
            }

            //set widths
            $ektron("div.EktronPersonalization table.ektronWidgetPage").width(tableWidth);


            var applyLegacyIeFix = false;
            //fix for ie7 when widgets contain object/embed - columns may not have correct widths
            if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 8) {
                applyLegacyIeFix = true;
            }

            if (applyLegacyIeFix) {
                $ektron("div.widgetWrapper").css("position", "relative").css("width", +tableWidth + 1 + "px").css("padding-bottom", "25px").css("overflow-x", "auto").css("overflow-y", "hidden");
            }

            //loop through each column
            $ektron("div.EktronPersonalization table.ektronWidgetPage td.column").each(function (i) {
                var newWidth = actualWidth;
                if (compensationFrequency > 0 && (i + 1) % compensationFrequency == 0) {
                    newWidth++;
                }

                if (applyLegacyIeFix) {
                    if ($ektron(this).find("div.widget div.content embed, div.widget div.content object").length == 0) {
                        var overflowWidth = String(Math.floor((newWidth - 23)));
                        $ektron(this).find("div.widget").css("width", String(overflowWidth) + "px");
                        $ektron(this).find("div.widget div.header").css("width", String(overflowWidth) + "px");
                        $ektron(this).find("div.widget div.content").css("width", String(overflowWidth) + "px");
                    }
                } else {
                    var overflowWidth = String(Math.floor((newWidth - 23)));
                    $ektron(this).find("div.widget").css("width", String(overflowWidth) + "px");
                    $ektron(this).find("div.widget div.header").css("width", String(overflowWidth) + "px");
                    $ektron(this).find("div.widget div.content").css("width", String(overflowWidth) + "px");

                }
                $ektron(this).css("width", String(newWidth - 2) + "px");
            });
            $ektron("div.widgetWrapper").css({
                "opacity": 100,
                "filter": "alpha(opacity=100)",
                "-ms-filter": "progid:DXImageTransform.Microsoft.Alpha(Opacity=100)"
            });
        },
        remove: function (obj) {
            var modalWindow = $ektron("#ektronPersonalizationTabModal");
            modalWindow.find("th.tabHeader span.label").text(removeColumnTitle);
            modalWindow.find("div.addTab").css("display", "none");
            modalWindow.find("div.removeTab").css("display", "none");
            modalWindow.find("div.removeColumn").css("display", "block");
            modalWindow.find("div.resetWidgets").css("display", "none");
            modalWindow.find("div.editDefaultWidgets").css("display", "none");
            $ektron('#ektronPersonalizationTabModal').modalShow();

            var selectedTab = $ektron(obj);
            modalWindow.find("div.removeColumn a.confirmRemove").attr("href", selectedTab.attr("href"));
        },
        removeConfirmation: function (obj) {
            var modal = $ektron('#ektronPersonalizationTabModal');
            modal.modalHide();
            Ektron.Personalization.update(obj);
            eval($ektron(obj).attr("href"));
        },
        resize: function () {
            window.clearTimeout(Ektron.Personalization.resizeTimeoutId);
            Ektron.Personalization.resizeTimeoutId = window.setTimeout('Ektron.Personalization.Columns.init();', 10);
        },
        stripe: function (num) {
            if (num % 2 == 0) {
                return true;
            } else {
                return false;
            }
        }
    },
    Editable: false,
    escapeAndEncode: function (string) {
        return string
                  .replace(/&/g, "&amp;")
                  .replace(/</g, "&lt;")
                  .replace(/>/g, "&gt;")
                  .replace(/'/g, "\'")
                  .replace(/\"/g, "\"")
    },
    getEmSize: function () {
        var emSize;
        if ($ektron.browser.msie) {
            var em = $ektron('<div/>')
                .css({ position: 'absolute', top: 0, left: 0, height: '1.0em', visibility: 'hidden', lineHeight: '1.0em' })
                .text('M').appendTo("body");
            emSize = em[0].style.pixelHeight + "px";
            em.remove();
        } else {
            emSize = $ektron("body").css("font-size");
        }
        emSize = emSize.replace(/px/g, "");  //remove the "px" from the returned value
        return +emSize;
    },
    init: function () {
        //init WidgetTray
        Ektron.Personalization.WidgetTray.init();

        //init Columns
        Ektron.Personalization.Columns.init();

        //init Widget drag-and-drop/sorting
        Ektron.Personalization.Widgets.init();

        //init modal
        Ektron.Personalization.Modal.init();

        //bind events
        Ektron.Personalization.bindEvents();

        //init block ui
        Ektron.Personalization.BlockUi.init();

        //init workarea if possible
        if ("undefined" != typeof Ektron.Workarea) {
            if ("undefined" != typeof Ektron.Workarea.Grids) {
                Ektron.Workarea.Grids.init();
            }
            if ("undefined" != typeof Ektron.Workarea.Tabs) {
                Ektron.Workarea.Tabs.init();
            }
        }
    },
    Modal: {
        init: function () {
            var modal = $ektron('#ektronPersonalizationTabModal');
            modal.drag('.tabHeader');
            modal.modal({
                trigger: '',
                toTop: true,
                modal: true,
                overlay: 0,
                onShow: function (hash) {
                    var originalWidth = hash.w.width();
                    hash.w.find("h4").css("width", originalWidth + "px");
                    var width = "-" + String(originalWidth / 2) + "px";
                    hash.w.css("margin-left", width);
                    hash.o.fadeTo("fast", 0.5, function () {
                        hash.w.fadeIn("fast");
                    });
                },
                onHide: function (hash) {
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function () {
                        if (hash.o)
                            hash.o.remove();
                    });
                }
            });

            //clear tab text field
            modal.find("input:text").attr("value", "");
        }
    },
    OptionsMenu: {
        hide: function () {
            $ektron("div.EktronPersonalization ul.tabOptions").slideUp(250);
        },
        timeoutDuration: 1250,
        timeoutId: 0,
        show: function (autoHide) {
            var menu = $ektron("div.EktronPersonalization ul.tabOptions");
            menu.slideDown(250, function () {
                if (autoHide === false) {
                    //trigger: keyboard - don't autohide & set focus on first menu item link
                    menu.find("li:first a:first").focus();
                }
            });

            if (autoHide === true) {
                //trigger: mouseover - hide menu if no menu item mouseover
                window.clearTimeout(Ektron.Personalization.OptionsMenu.timeoutId);
                Ektron.Personalization.OptionsMenu.timeoutId = setTimeout(function () {
                    Ektron.Personalization.OptionsMenu.hide();
                }, Ektron.Personalization.OptionsMenu.timeoutDuration);
            }

        }
    },
    SetEditable: function (val) {
        Ektron.Personalization.Editable = val;
    },
    Tabs: {
        activeIndex: -1,
        add: function () {
            var modalWindow = $ektron("#ektronPersonalizationTabModal");
            modalWindow.find("th.tabHeader span.label").text(addTabPopupTitle);
            modalWindow.find("div.addTab").css("display", "block");
            modalWindow.find("div.removeTab").css("display", "none");
            modalWindow.find("div.removeColumn").css("display", "none");
            modalWindow.find("div.resetWidgets").css("display", "none");
            modalWindow.find("div.editDefaultWidgets").css("display", "none");
            $ektron('#ektronPersonalizationTabModal').modalShow();
        },
        select: function (index) {
            activeIndex = index;
            var elem = $ektron("div.EktronPersonalizationWrapper input.activePage");
            elem.attr("value", index);
        },
        vaidateLabel: function (obj) {
            var tabLabel = $ektron("#" + obj.id).parents(".ektronWindow").find("tbody input");

            //ensure "tab label" field is not empty
            if (tabLabel.val() === undefined || tabLabel.val().length === 0) {
                //field is empty
                tabLabel.nextAll("span.requiredReminder").fadeOut("fast", function () {
                    tabLabel.nextAll("span.requiredReminder").fadeIn("fast");
                });
            } else {
                //field is not empty - escape and encode prior to submitting
                var modal = $ektron('#ektronPersonalizationTabModal');
                modal.modalHide();
                tabLabel.attr("value", Ektron.Personalization.escapeAndEncode(tabLabel.val()));
                Ektron.Personalization.update("#" + obj.id);
                eval($ektron(obj).attr("href"));

            }
        },
        remove: function (obj) {
            var modalWindow = $ektron("#ektronPersonalizationTabModal");

            var tabs = $ektron(obj).parents("ul.tabs").children("li.tab:not([class*='tabOptions'])").length;

            modalWindow.find("th.tabHeader span.label").text(removeTabTitle);
            modalWindow.find("div.addTab").css("display", "none");
            modalWindow.find("div.removeColumn").css("display", "none");
            modalWindow.find("div.removeTab").css("display", tabs == 1 ? "none" : "block");
            modalWindow.find("div.resetWidgets").css("display", tabs == 1 ? "block" : "none");
            modalWindow.find("div.editDefaultWidgets").css("display", "none");
            $ektron('#ektronPersonalizationTabModal').modalShow();

            if (tabs > 1) {
                var selectedTab = $ektron(obj);
                modalWindow.find("div.removeTab span.label").text(selectedTab.next().text());
                modalWindow.find("div.removeTab a.confirmRemove").attr("href", selectedTab.attr("href"));
            }
        },
        removeConfirmation: function (obj) {
            $ektron('#ektronPersonalizationTabModal').modalHide();
            Ektron.Personalization.update(obj);
            eval($ektron(obj).attr("href"));

        }
    },
    update: function (obj) {
        Ektron.Personalization.BlockUi.block();
        return true;
    },
    Widgets: {
        init: function () {
            if (Ektron.Personalization.Editable) {
                if ($ektron("ul.ektronPersonalizationWidgetList").is("ui-sortable")) {
                    $ektron("ul.ektronPersonalizationWidgetList").sortable("destroy");
                }
                $ektron("ul.ektronPersonalizationWidgetList").sortable({
                    appendTo: "body",
                    items: ".widgetToken",
                    connectWith: ["table.ektronWidgetPage td.column"],
                    opacity: .8,
                    revert: false,
                    zIndex: 99999,
                    scroll: true,
                    cursor: "move",
                    dropOnEmpty: true,
                    containment: $ektron("body"),
                    forcePlaceholderSize: true,
                    placeholder: "placeholder",
                    helper: "clone",
                    start: function (e, ui) {
                        Ektron.Personalization.Widgets.sortElement = ui.item;
                    },
                    stop: function (e, ui) {
                        if ($(ui.item).parents("ul.widgetList").length == 0) {
                            Ektron.Personalization.Widgets.sortHandler(e, ui);
                        }
                    }
                });
                if ($ektron("table.ektronWidgetPage td.column").is("ui-sortable")) {
                    $ektron("table.ektronWidgetPage td.column").sortable("destroy");
                }
                $ektron("table.ektronWidgetPage td.column").sortable({
                    items: "div.widget",
                    connectWith: ["table.ektronWidgetPage td.column"],
                    zIndex: 99999,
                    scroll: true,
                    sensitivity: 1,
                    handle: ".header",
                    containment: $ektron("body"),
                    cursor: "move",
                    cursorAt: { top: 5, left: 5 },
                    dropOnEmpty: true,
                    helper: function (e, el) {
                        //var helper = null;
                        //var title = $ektron(el).find("input.widgetTitle").val();
                        //var widgetTokenImagePath = $ektron(el).find("input.widgetTokenImagePath").val();
                        var widgetTypeID = $ektron(el).find("input.widgetTokenTypeId").val();
                        var widgetToken = $ektron("div.widgetTray ul.widgetList li.widgetToken input[value='" + widgetTypeID + "']").parent();
                        var helper = null;
                        if (widgetToken.length == 0) {
                            widgetToken = $ektron("div.widgetTray ul.widgetList li.widgetTokenFirst");
                            helper = widgetToken.clone();
                            var widgetTypeImg = $ektron(el).find("input.widgetTokenImagePath").val();
                            var widgetTypeName = $ektron(el).find("input.widgetTitle").val();
                            helper.attr("title", widgetTypeName);
                            helper.find("img").attr("alt", widgetTypeName);
                            helper.find("img").attr("title", widgetTypeName);
                            helper.find("img").attr("src", widgetTypeImg);
                            helper.find("span").text(widgetTypeName);
                            helper.find("input").val(widgetTypeID);
                        } else {
                            helper = widgetToken.clone();
                        }
                        helper
                            .css("height", widgetToken.css("height"))
                            .css("width", widgetToken.css("width"))
                            .css("min-height", "76px")
                            .css("list-style", "none");
                        //helper.find("img").attr("src", widgetTokenImagePath);
                        //helper.find("span").html(title);
                        helper.prependTo("body");
                        return helper;
                    },
                    forcePlaceholderSize: false,
                    placeholder: "placeholder",
                    start: function (e, ui) {
                        Ektron.Personalization.Widgets.sortElement = ui.item;
                        var sortElement = $ektron(Ektron.Personalization.Widgets.sortElement);
                        var column = sortElement.parents(".column");
                        var startColumnIndex = column.parents(".widgetList").find(".column").index(column.get(0));
                        startWidgetIndex = column.find(".widget").index(sortElement.get(0));
                        sortElement.attr("data-ektron-startColumnIndex", startColumnIndex);
                        sortElement.attr("data-ektron-startWidgetIndex", startWidgetIndex);
                    },
                    stop: function (e, ui) {
                        Ektron.Personalization.Widgets.sortHandler(e, ui);
                        if ($ektron.browser.msie) {
                            $ektron("div.widget").css("zoom", "-1").css("zoom", "1");
                        }
                    }
                });

                //if a widget is in edit mode, scroll to it
                $ektron("div.EktronPersonalization div.widget a.edit").each(function (i) {
                    if ($ektron(this).attr("data-ektron-editMode") === "true") {
                        Ektron.Personalization.Widgets.scrollToMe(this);
                    }
                });
            }
        },
        editDefaults: function (obj) {
            var modalWindow = $ektron("#ektronPersonalizationTabModal");
            modalWindow.find("th.tabHeader span.label").text(editDefaultTitle);
            modalWindow.find("div.addTab").css("display", "none");
            modalWindow.find("div.removeTab").css("display", "none");
            modalWindow.find("div.removeColumn").css("display", "none");
            modalWindow.find("div.resetWidgets").css("display", "none");
            modalWindow.find("div.editDefaultWidgets").css("display", "block");
            $ektron('#ektronPersonalizationTabModal').modalShow();
        },
        reset: function (obj) {
            var modalWindow = $ektron("#ektronPersonalizationTabModal");
            modalWindow.find("th.tabHeader span.label").text(resetWidgetTitle);
            modalWindow.find("div.addTab").css("display", "none");
            modalWindow.find("div.removeTab").css("display", "none");
            modalWindow.find("div.removeColumn").css("display", "none");
            modalWindow.find("div.resetWidgets").css("display", "block");
            modalWindow.find("div.editDefaultWidgets").css("display", "none");
            $ektron('#ektronPersonalizationTabModal').modalShow();
        },
        resetConfirmation: function (obj) {
            $ektron('#ektronPersonalizationTabModal').modalHide();
            Ektron.Personalization.update(obj);
            eval($ektron(obj).attr("href"));
        },
        scrollToMe: function (obj) {
            var widget = $ektron(obj).parents("div.widget");
            $ektron.scrollTo(widget, 800, { onAfter: function () {
                widget.effect("highlight", { color: "#9a9a9a" }, 500);
            }
            });
        },
        sortElement: {},
        sortHandler: function (e, ui) {
            var action;
            var command;
            var controller;
            var endColumnIndex;
            var endWidgetIndex;
            var sortAction;
            var startColumnIndex;
            var startWidgetIndex;
            var widgetTypeIndex;

            //determine sort action
            if ($ektron(Ektron.Personalization.Widgets.sortElement).parents("li.widgetToken").length > 0
                    || $ektron(Ektron.Personalization.Widgets.sortElement).hasClass("widgetToken")) {
                sortAction = "add"; //widget is new - comes from widget tray
            } else {
                sortAction = "move"; //widget already exists - comes from another column
            }

            switch (sortAction) {
                case ("add"):
                    controller = "personalization";
                    action = "add_widget";
                    var widget = $ektron(Ektron.Personalization.Widgets.sortElement);
                    if (!widget.hasClass("widgetToken"))
                        widget = $ektron(Ektron.Personalization.Widgets.sortElement).parents(".widgetToken");

                    var column = widget.parents(".column");
                    endColumnIndex = column.parents(".widgetList").find(".column").index(column.get(0));
                    endWidgetIndex = column.find("li.widgetToken").prevAll("div.widget").length;

                    widgetTypeIndex = parseInt(widget.find("input").attr("value"), 10);

                    command = {
                        Controller: controller,
                        Action: action,
                        Arguments: [
                                      widgetTypeIndex,
                                      endColumnIndex,
                                      endWidgetIndex
                                ]
                    }
                    $ektron(document).trigger("EktronPersonalizationSortEnd", ["add", Ektron.Personalization.Widgets.sortElement]);
                    break;
                case ("move"):
                    controller = "widget_list_container";
                    action = "move_widget";
                    startColumnIndex = parseInt($ektron(Ektron.Personalization.Widgets.sortElement).attr("data-ektron-startColumnIndex"));
                    startWidgetIndex = parseInt($ektron(Ektron.Personalization.Widgets.sortElement).attr("data-ektron-startWidgetIndex"));

                    var column = $ektron(Ektron.Personalization.Widgets.sortElement).parents(".column");
                    endColumnIndex = column.parents(".widgetList").find(".column").index(column.get(0));
                    var widget = $ektron(Ektron.Personalization.Widgets.sortElement);
                    endWidgetIndex = column.find(".widget").index(widget.get(0));

                    command = {
                        Controller: controller,
                        Action: action,
                        Arguments: [
                                      startColumnIndex,
                                      startWidgetIndex,
                                      endColumnIndex,
                                      endWidgetIndex
                                ]
                    }
                    // this event is raised to solve the problem of moving the widgets in the dashboard and making it work with icallbackeventhandler
                    //Parameters passed are the event and the widget which is moved.
                    $ektron(document).trigger("EktronPersonalizationSortEnd", ["move", Ektron.Personalization.Widgets.sortElement]);
                    break;
                default:
                    command === null;
                    break;
            }
            if (command !== null) {
                //update personalization
                Ektron.Personalization.BlockUi.block();
                setTimeout(function () {
                    __doPostBack($ektron(Ektron.Personalization.Widgets.sortElement).parents("div.EktronPersonalization").parent().attr("id"), Ektron.JSON.stringify(command));
                }, 100);
            } else {
                alert("Command Not Recognized");
            }
        }
    },
    WidgetTray: {
        slideDistance: 50,
        hide: function (menu) {
            var columnsRemove = $ektron("div.EktronPersonalization div.columnRemove");

            if (columnsRemove.length > 0) {
                //hide "remove column" widgets
                columnsRemove.slideUp("fast", function () {
                    var slideOutWidth = menu.width() + (4 * Ektron.Personalization.getEmSize());
                    menu.find("ul.widgetList").animate({ left: slideOutWidth });
                    var arrow = $ektron("a.widgetTrayToggle span");
                    arrow.fadeOut("fast", function () {
                        arrow.removeClass("directionUp");
                        arrow.fadeIn("fast");
                    });
                    menu.find("p.scrollWrapper a").fadeOut("fast", function () {
                        menu.slideUp("fast");
                    });
                });
            } else {
                var slideOutWidth = menu.width() + (4 * Ektron.Personalization.getEmSize());
                menu.find("ul.widgetList").animate({ left: slideOutWidth });
                var arrow = $ektron("a.widgetTrayToggle span");
                arrow.fadeOut("fast", function () {
                    arrow.removeClass("directionUp");
                    arrow.fadeIn("fast");
                });
                menu.find("p.scrollWrapper a").fadeOut("fast", function () {
                    menu.slideUp("fast");
                });
            }
        },
        init: function () {
            //set width of widget bar
            var menuWidth;
            var menu;
            var widgetItems;
            var oneEm;
            var buffer;
            var widgetListWidth = 0;

            //get EM size and set buffer
            oneEm = Ektron.Personalization.getEmSize() + 2;
            buffer = oneEm;

            //get widget tokens
            $ektron("div.EktronPersonalization li.widgetToken:first").addClass("widgetTokenFirst");
            $ektron("div.EktronPersonalization li.widgetToken:last").addClass("widgetTokenLast");
            widgetItems = $ektron("div.EktronPersonalization li.widgetToken");
            if (widgetItems.length === 0) {
                menuWidth = 52 + buffer;
            } else {
                widgetItemWidth = parseInt(widgetItems.eq(0).css("width").replace(/px/g, "")) + oneEm;
                menuWidth = ((widgetItems.length + 1) * widgetItemWidth + (2 * oneEm));
            }
            menu = $ektron("div.EktronPersonalization div.widgetTray ul.widgetList");
            menu.css("width", menuWidth);
            menu.css("left", menuWidth + "px");
        },
        next: function () {
            var widgetList = $ektron(".widgetList");
            var widgetListWidth = parseInt(widgetList.width());
            var widgetListWrapperWidth = parseInt(widgetList.parent().width());
            var left = parseInt(widgetList.css("left").replace(/px/g, ""));  //remove the "px"
            var widgetListRight = widgetListWidth + left;

            var newLeft = left + "px";
            left = left - (widgetListRight > widgetListWrapperWidth + Ektron.Personalization.WidgetTray.slideDistance ? Ektron.Personalization.WidgetTray.slideDistance : (widgetListWidth - widgetListWrapperWidth) + left);
            var newLeft = left + "px";
            widgetList.animate({ left: newLeft });

            widgetListRight = widgetListWidth + left;
            Ektron.Personalization.WidgetTray.nextPreviousToggle(left, widgetListRight, widgetListWrapperWidth);
        },
        nextPreviousToggle: function (left, widgetListRight, widgetListWrapperWidth) {
            var scrollLeft = $ektron("div.EktronPersonalization div.widgetTray p.scrollWrapper a.scrollLeft");
            var scrollRight = $ektron("div.EktronPersonalization div.widgetTray p.scrollWrapper a.scrollRight");

            if (left === 0) {
                scrollLeft.addClass("hide"); //left OFF
                scrollRight.removeClass("hide"); //right ON
            } else if ((left < 0) && (widgetListRight >= widgetListWrapperWidth)) {
                scrollLeft.removeClass("hide"); //left ON
                scrollRight.removeClass("hide"); //right ON
            } else if ((left < 0) && (widgetListRight < widgetListWrapperWidth)) {
                scrollLeft.removeClass("hide"); //left ON
                scrollRight.addClass("hide"); //right OFF
            }

        },
        previous: function () {
            var widgetList = $ektron(".widgetList");
            var widgetListWidth = parseInt(widgetList.width());
            var widgetListWrapperWidth = parseInt(widgetList.parent().width());
            var left = parseInt(widgetList.css("left").replace(/px/g, ""));  //remove the "px"
            var widgetListRight = widgetListWidth + left;

            left = left + (left <= -Ektron.Personalization.WidgetTray.slideDistance ? Ektron.Personalization.WidgetTray.slideDistance : -left);
            var newLeft = left + "px";
            $ektron(".widgetList").animate({ left: newLeft });

            Ektron.Personalization.WidgetTray.nextPreviousToggle(left, widgetListRight, widgetListWrapperWidth);
        },
        show: function (menu) {
            //intialize widget tray
            Ektron.Personalization.WidgetTray.init();

            //set width on overflow wrapper for ie6
            if (($ektron.browser.msie) && (parseInt($ektron.browser.version) < 7)) {
                var width = $ektron("div.EktronPersonalization .widgetTrayHandle").width();
                width = width + Ektron.Personalization.getEmSize();
                $ektron("div.EktronPersonalization div.widgetTray div.overflowWrapper").css("width", width);
            }

            //point arrow down
            var arrow = $ektron("a.widgetTrayToggle span");
            arrow.fadeOut("fast", function () {
                arrow.addClass("directionUp");
                arrow.fadeIn("fast");
            });

            //open widget tray
            var menu = $ektron("div.EktronPersonalization div.widgetTray");
            menu.slideDown("fast", function () {
                menu.find("p.scrollWrapper a").fadeIn("fast");
            });

            //slide widgets in from left
            menu.find("ul.widgetList").animate({ left: "0" }, function () {
                //show "remove column" widgets
                $ektron("div.EktronPersonalization div.columnRemove").slideDown("fast");
            });

            Ektron.Personalization.WidgetTray.slideDistance = parseInt($ektron(".widgetList").parent().width());
        },
        toggle: function () {
            var menu = $ektron("div.EktronPersonalization div.widgetTray");
            if (menu.css("display") === "block") {
                Ektron.Personalization.WidgetTray.hide(menu);
            } else {
                Ektron.Personalization.WidgetTray.show(menu);
            }
        }
    }
}
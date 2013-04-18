if ("undefined" == typeof Ektron) { Ektron = {}; }
if ("undefined" == typeof Ektron.Analytics) { Ektron.Analytics = {}; }
if ("undefined" == typeof Ektron.Analytics.Overlay) {
    Ektron.Analytics.Overlay = {
        bindEvents: function () {
            $ektron('.displayOverlay').click(function () {
                Ektron.Ektron.Analytics.Overlay.display();
            });
            $ektron('#page-overlay').click(function () {
                Ektron.Analytics.Overlay.remove();
            });
            $ektron(window).resize(Ektron.Analytics.Overlay.handleWindowResizing);
        },
        range: -30,
        cookieKey: 'ektron_analytics_overlay',
        display: function () {
            if (window.EktronOverlayLoaded != null && window.EktronOverlayLoaded == true) {
                return;
            }
            window.EktronOverlayLoaded = true;
            var overlay = '<div id="EktronAnalyticsOverlay" class="EktronAnalyticsOverlay">' +
                                '<span class="EktronOverlayUi">' +
                                    '<span class="EktronOverlayUiDisplayingMode">Displaying: </span>' +
                                    '<select class="EktronAnalyticsOverlayViewSelect" onchange="Ektron.Analytics.Overlay.setView(this.options[this.selectedIndex].value);">' +
                                        '<option value="clicks">Clicks</option>' +
            /* '<option value="clickmap">Click Map</option>' + */
            /* '<option value="clickpoint">Click Point</option>' + */
                                    '</select> ' +
                                    '<span class="EktronOverlayDatePicker">' +
                                        '<label for="from">From</label>' +
                                        '<input class="startDate" type="text" id="from" name="from"/>' +
                                        '<label for="to">To</label>' +
                                        '<input class="endDate" type="text" id="to" name="to"/>' +
                                        '<a href="#" title="Refresh" onclick="Ektron.Analytics.Overlay.refresh(); return false;" ><img alt="refresh" src="' + Ektron.Analytics.Overlay.appImgPath() + '../UI/Icons/refresh.png" /></a>' +
                                    '</span>' +
                                '</span>' +
                                '<a href="#" class="EktronAnalyticsOverlayRemove" onclick="Ektron.Analytics.Overlay.remove();return false;">Remove Overlay</a> ' +
                            '</div>';
            $ektron("body").prepend(overlay);
            Ektron.Analytics.Overlay.setView('clicks');
        },
        Clicks: {
            show: function () {
                $ektron.fn.cluetip.defaults.width = 150;
                $ektron.fn.cluetip.defaults.cluezIndex = 999999;
                $ektron.fn.cluetip.defaults.clickThrough = true;

                Ektron.Analytics.Overlay.setCookieKeyValue('mode', 'clicks');

                var href = Ektron.Analytics.Overlay.appPath() + 'analytics/overlay/ektronOverlay.ashx?type=clicks' + Ektron.Analytics.Overlay.getDateRange();
                $ektron.getJSON(href, function (data) {
                    if (data != null) {
                        if (data.errorFlag) {
                            alert(data.errorMessage);
                        } else {
                            Ektron.Analytics.Overlay.datePicker(data.dateRangeData);
                            for (var i = 0; i < data.items.length; i++) {
                                // identify and tag all direct matches:
                                $ektron('a[href$="' + data.items[i].url + '"]:not(.EktronAnalyticsOverlayRemove,.EktronEditorsMenu a)').addClass('EktronAnalyticsOverlayClickInfo').attr({ title: 'Clicks: ' + data.items[i].clicks + '<br />' + data.items[i].percent + '%' }).attr("data-ektron-overlay-percent", data.items[i].percent);

                                // attempt to identify links using current location "context" to complete the link path:
                                if (data.linkContext.length > 1) {
                                    $ektron('a:not([href^="/"],[href^="#"],[href^="javascript:"])').each(function () {
                                        var obj = $ektron(this);
                                        obj.attr("data-ektron-overlay-adjusted-href", data.linkContext + obj.attr("href"));
                                    });
                                    $ektron('a[data-ektron-overlay-adjusted-href$="' + data.items[i].url + '"]:not(.EktronAnalyticsOverlayRemove,.EktronEditorsMenu a)').addClass('EktronAnalyticsOverlayClickInfo').attr({ title: 'Clicks: ' + data.items[i].clicks + '<br />' + data.items[i].percent + '%' }).attr("data-ektron-overlay-percent", data.items[i].percent);
                                }
                            }
                        }
                        // set appropriate links with no data to show zero clicks:
                        $ektron('a:not(.EktronAnalyticsOverlayClickInfo,.EktronAnalyticsOverlay a,.EktronOverlayDatePicker a,.EktronAnalyticsOverlayRemove,.EktronEditorsMenuMarker,.EktronEditorsMenu a)').not($ektron('a[onclick^="ecmPopUpWindow"]')).addClass('EktronAnalyticsOverlayClickInfo').attr({ title: 'Clicks: 0 <br /> 0.00%' }).attr("data-ektron-overlay-percent", "0.00");

                        // format the data (in the title) for the clue-tip code:
                        $ektron('a.EktronAnalyticsOverlayClickInfo').cluetip({ splitTitle: '|' });

                        // add click-bar, identifying percentages:
                        $ektron('a.EktronAnalyticsOverlayClickInfo').each(function () {
                            var obj = $ektron(this);
                            var percentDataObj = obj.attr("data-ektron-overlay-percent");
                            var percent = 0;
                            try { percent = ((percentDataObj != null) ? parseFloat(percentDataObj.toString()) : 0); }
                            catch (ex) { percent = 0; }
                            var barObj = $ektron(this).find('.ektronOverlayInnerBar');
                            if (barObj != null && barObj.length > 0) {
                                barObj.css('width', percent / 2.0 + 'px');
                            } else {
                                var bars = "<div class='ektronOverlayOuterBar' style='width:50px' ><div class='ektronOverlayInnerBar' style='width:" + percent / 2.0 + "px;'></div></div>";
                                obj.append(bars);
                            }
                        });
                    }
                    $ektron('.EktronEditorsMenu').hide();
                    $ektron('#page-overlay').remove();
                    $ektron('#overlay-loading').remove();
                });
            },
            hide: function () {
                $ektron('.ektronOverlayOuterBar').remove();
                $ektron('a').removeClass('EktronAnalyticsOverlayClickInfo');
                $ektron('a').cluetip('destroy');
                $ektron(document).trigger('hideCluetip');
            }
        },
        refresh: function () {
            var startDate = $ektron(".EktronOverlayDatePicker input.startDate").val();
            if (startDate != null && startDate.length > 0) {
                Ektron.Analytics.Overlay.setCookieKeyValue('startdate', startDate);
            }

            var endDate = $ektron(".EktronOverlayDatePicker input.endDate").val();
            if (endDate != null && endDate.length > 0) {
                Ektron.Analytics.Overlay.setCookieKeyValue('enddate', endDate);
            }
            Ektron.Analytics.Overlay.setView('clicks');
        },
        datePicker: function (dateRangeData) {
            if (dateRangeData != null) {
                if (dateRangeData.fromText != null && dateRangeData.fromText.length > 0) {
                    $ektron(".EktronOverlayDatePicker label[for='from']").text(dateRangeData.fromText)
                }

                if (dateRangeData.toText != null && dateRangeData.toText.length > 0) {
                    $ektron(".EktronOverlayDatePicker label[for='to']").text(dateRangeData.toText)
                }

                if (dateRangeData.startDate != null && dateRangeData.startDate.length > 0) {
                    $ektron(".EktronOverlayDatePicker input.startDate").val(dateRangeData.startDate)
                }

                if (dateRangeData.endDate != null && dateRangeData.endDate.length > 0) {
                    $ektron(".EktronOverlayDatePicker input.endDate").val(dateRangeData.endDate)
                }
            }
            $ektron(".EktronOverlayDatePicker input.startDate").val()

            var dates = $ektron('#from, #to').datepicker({
                changeMonth: true,
                numberOfMonths: 1,
                beforeShow: function (input, inst) {
                    $ektron('body > .ui-datepicker').each(function () {
                        var obj = $ektron(this);
                        obj.attr("data-ektron-overlay-original-zindex", obj.css("z-index"));
                        obj.css("z-index", "9999999");
                    });
                },
                onSelect: function (selectedDate) {
                    var option = this.id == "from" ? "minDate" : "maxDate";
                    var instance = $(this).data("datepicker");
                    var date = $ektron.datepicker.parseDate(selectedDate, instance.settings.dateFormat || $.datepicker._defaults.dateFormat);
                    dates.not(this).datepicker("option", option, date);
                },
                onClose: function (dateText, inst) {
                    $ektron('body > .ui-datepicker').each(function () {
                        var obj = $ektron(this);
                        obj.css("z-index", obj.attr("data-ektron-overlay-original-zindex"));
                        obj.removeAttr("data-ektron-overlay-original-zindex");
                    });
                }
            });
        },
        getDateRange: function () {
            return "&startdate=" + $ektron(".EktronOverlayDatePicker input.startDate").val() + "&enddate=" + $ektron(".EktronOverlayDatePicker input.endDate").val();
        },
        Abandon: {
            show: function () {
                $ektron.get(Ektron.Analytics.Overlay.appPath() + 'analytics/overlay/clickmap.aspx?type=abandonpath', { l: escape(document.location.pathname) },
					function (html) {
					    $ektron(html).appendTo('body');

					    var pageHeight = $ektron(document).height();
					    var newHeight = 0;
					    var blockHeight = Math.round(pageHeight / 10);
					    var abandonRate = 0;

					    // Set the Top Offset for the 10% Report Div to 0, but the height to our blockHeight
					    Ektron.Analytics.Overlay.setOffset("10", newHeight, abandonRate);

					    // Set the Top Offset for the 20% Report Div
					    newHeight = blockHeight;
					    Ektron.Analytics.Overlay.setOffset("20", newHeight, abandonRate);

					    // Set the Top Offset for the 30% Report Div
					    newHeight = blockHeight * 2;
					    Ektron.Analytics.Overlay.setOffset("30", newHeight, abandonRate);

					    // Set the Top Offset for the 40% Report Div
					    newHeight = blockHeight * 3;
					    Ektron.Analytics.Overlay.setOffset("40", newHeight, abandonRate);

					    // Set the Top Offset for the 50% Report Div
					    newHeight = blockHeight * 4;
					    Ektron.Analytics.Overlay.setOffset("50", newHeight, abandonRate);

					    // Set the Top Offset for the 60% Report Div
					    newHeight = blockHeight * 5;
					    Ektron.Analytics.Overlay.setOffset("60", newHeight, abandonRate);

					    // Set the Top Offset for the 70% Report Div
					    newHeight = blockHeight * 6;
					    Ektron.Analytics.Overlay.setOffset("70", newHeight, abandonRate);

					    // Set the Top Offset for the 80% Report Div
					    newHeight = blockHeight * 7;
					    Ektron.Analytics.Overlay.setOffset("80", newHeight, abandonRate);

					    // Set the Top Offset for the 90% Report Div
					    newHeight = blockHeight * 8;
					    Ektron.Analytics.Overlay.setOffset("90", newHeight, abandonRate);

					    // Set the Top Offset for the 100% Report Div
					    newHeight = blockHeight * 9;
					    Ektron.Analytics.Overlay.setOffset("100", newHeight, abandonRate);

					    $ektron('#page-overlay').remove();
					    $ektron('#overlay-loading').remove();
					}
				);
            },
            hide: function () {
            }
        },
        ClickPoint: {
            show: function () {
                Ektron.Analytics.Overlay.setCookieKeyValue('mode', 'clickpoint');
                Ektron.Analytics.Overlay.resizingCallback = Ektron.Analytics.Overlay.ClickPoint.resize;

                var href = Ektron.Analytics.Overlay.appPath() + 'analytics/overlay/ektronOverlay.ashx?type=clickmap' + Ektron.Analytics.Overlay.getDateRange();
                $ektron.getJSON(href, function (data) {
                    if (data != null) {
                        if (data.errorFlag) {
                            alert(data.errorMessage);
                        } else {
                            Ektron.Analytics.Overlay.datePicker(data.dateRangeData);
                            var width = $ektron(document).width();
                            var height = $ektron(document).height();
                            for (var i = 0; i < data.items.length; i++) {
                                var x = width * (data.items[i].scaledX / 100) - 4;
                                var y = height * (data.items[i].scaledY / 100) - 4;
                                var clickObj = "<div class='ektronOverlayClickPointPointer' style='position: absolute; top: " + y + "px; left: " + x + "px; opacity: 0.8; filter: alpha(opacity=80); background-image: url(" + Ektron.Analytics.Overlay.appImgPath() + "bookmarks/remove.gif); background-repeat: no-repeat; z-index:999998 !important; padding: 8px;' ></div>";
                                $ektron("body").append(clickObj);
                            }
                        }
                    }
                    $ektron('.EktronEditorsMenu').hide();
                    $ektron('#page-overlay').remove();
                    $ektron('#overlay-loading').remove();
                });
            },
            resize: function () {
                if (Ektron.Analytics.Overlay.resizingTimer != null) {
                    window.clearTimeout(Ektron.Analytics.Overlay.resizingTimer);
                }
                $ektron('.ektronOverlayClickPointPointer').remove();
                Ektron.Analytics.Overlay.resizingTimer = window.setTimeout(Ektron.Analytics.Overlay.ClickPoint.show, 500);
            },
            delayedResize: function () {
            },
            hide: function () {
                $ektron('.ektronOverlayClickPointPointer').remove();
            }
        },
        ClickMap: {
            show: function () {
                Ektron.Analytics.Overlay.setCookieKeyValue('mode', 'clickmap');
                Ektron.Analytics.Overlay.resizingCallback = Ektron.Analytics.Overlay.ClickMap.resize;

                var href = Ektron.Analytics.Overlay.appPath() + 'analytics/overlay/ektronOverlay.ashx?type=clickmap' + Ektron.Analytics.Overlay.getDateRange();
                $ektron.getJSON(href, function (data) {
                    if (data != null) {
                        if (data.errorFlag) {
                            alert(data.errorMessage);
                        } else {
                            Ektron.Analytics.Overlay.datePicker(data.dateRangeData);
                            var width = $ektron(document).width();
                            var height = $ektron(document).height();
                            var grid = "<div class='clickmapGrid' style='position: absolute; top: 0px; left: 0px; opacity: 0.5; filter: alpha(opacity=50); width: " + width.toString() + "px; height: " + height.toString() + "px; margin: 0; padding: 0; z-index:999998 !important;'>";
                            grid += "<table style='width: 100%; height: 100%;' cellspacing='1' cellpadding='1'>";
                            for (var y = 0; y <= 50; y++) {
                                grid += "<tr class='clickmapRow'>";
                                for (var x = 0; x <= 50; x++) {
                                    grid += "<td class='clickmapCell_" + x.toString() + "_" + y.toString() + "' style='background-color: #000;' >";
                                    grid += "</td>";
                                }
                                grid += "</tr>";
                            }
                            grid += "</table>";
                            grid += "</div>";

                            $ektron("body").append(grid);

                            // now transfer data to cells, accumulating counts
                            var maxCnt = 1;
                            for (var i = 0; i < data.items.length; i++) {
                                var dataX = Math.round(0.5 + data.items[i].scaledX / 2);
                                var dataY = Math.round(0.5 + data.items[i].scaledY / 2);
                                var cellObj = $ektron(".clickmapCell_" + dataX.toString() + "_" + dataY.toString());
                                if (cellObj != null) {
                                    var cnt = 0;
                                    var cellData = cellObj.attr("data-ektron-overlay-cellcount");
                                    if (cellData != null) {
                                        try { cnt = ((cellData != null) ? parseInt(cellData.toString(), 10) : 0); }
                                        catch (ex) { cnt = 0; }
                                    }
                                    cnt += data.items[i].clicks;
                                    cellObj.attr("data-ektron-overlay-cellcount", cnt.toString());

                                    if (cnt > maxCnt) { maxCnt = cnt; }
                                }
                            }

                            // coolor array:
                            var cellColors = new Array();
                            cellColors[0] = "#000";
                            cellColors[1] = "#005";
                            cellColors[2] = "#00a";
                            cellColors[3] = "#00f";
                            cellColors[4] = "#050";
                            cellColors[5] = "#0a0";
                            cellColors[6] = "#0f0";
                            cellColors[7] = "#500";
                            cellColors[8] = "#a00";
                            cellColors[9] = "#f00";

                            var colorScale = 9 / maxCnt;

                            // now utilize counts to set colors:
                            $ektron(".clickmapGrid td[data-ektron-overlay-cellcount]").each(function () {
                                var cellData = $ektron(this).attr("data-ektron-overlay-cellcount");
                                if (cellData != null) {
                                    var cnt = 0;
                                    try { cnt = ((cellData != null) ? parseInt(cellData.toString(), 10) : 0); }
                                    catch (ex) { cnt = 0; }
                                    var scaledCount = Math.round(cnt * colorScale);
                                    if (scaledCount > 9) {
                                        scaledCount = 9;
                                    }
                                    $ektron(this).css("background-color", cellColors[scaledCount]);
                                }
                            });
                        }
                    }
                    $ektron('.EktronEditorsMenu').hide();
                    $ektron('#page-overlay').remove();
                    $ektron('#overlay-loading').remove();
                });
            },
            resize: function () {
                $ektron(".clickmapGrid").css("height", $ektron(document).height().toString() + "px");
                $ektron(".clickmapGrid").css("width", $ektron(document).width().toString() + "px");
            },
            hide: function () {
                $ektron(".clickmapGrid").remove();
            }
        },
        HeatMap: {
            show: function () {
                Ektron.Analytics.Overlay.setCookieKeyValue('mode', 'heatmap');
                $ektron('.ektronOverlayOuterBar').remove();

                var href = Ektron.Analytics.Overlay.appPath() + 'analytics/overlay/ektronOverlay.ashx?type=clickmap' + Ektron.Analytics.Overlay.getDateRange();
                $ektron.getJSON(href, function (data) {
                    if (data != null) {
                        if (data.errorFlag) {
                            alert(data.errorMessage);
                        } else {
                            Ektron.Analytics.Overlay.datePicker(data.dateRangeData);
                        }
                    }
                    $ektron('.EktronEditorsMenu').hide();
                    $ektron('#page-overlay').remove();
                    $ektron('#overlay-loading').remove();
                });
            },
            hide: function () {
            }
        },
        init: function () {
            // if needed, allow server-side registered Javascript to load before running initialization code:
            if ('undefined' == typeof Ektron || 'undefined' == typeof Ektron.Site || 'undefined' == typeof Ektron.Site.SiteData) {
                window.setTimeout(Ektron.Analytics.Overlay.init, 100);
                return;
            }

            Ektron.Analytics.Overlay.bindEvents();
        },
        addCssLink: function (href, id) {
            if ($ektron("#" + id).length == 0 && $ektron("[href='" + href + "']").length == 0) {
                var link = '<link href="' + href + '" type="text/css" rel="stylesheet" id="' + id + '">'
                $ektron("body").prepend(link);
            }
        },
        addJavascriptLink: function (src, id) {
            if ($ektron("#" + id).length == 0 && $ektron("[src='" + src + "']").length == 0) {
                var link = '<script src="' + src + '" id="' + id + '"></script>';
                $ektron("body").prepend(link);
            }
        },
        setOffset: function (selector, height, abandonRate) {
            $ektron(".PercentBlock" + selector).addClass("AbandonSection").css("height", height).css("top", 0);
            $ektron(".PercentBlock" + selector).addClass("AbandonSection").css("height", height - 2 + "px");
            if ($ektron("#Rate" + selector).text().length > 1) {
                abandonRate = parseInt($ektron("#Rate" + selector).text().substring(0, $ektron("#Rate" + selector).text().length - 1), 10);
                $ektron(".PercentBlock" + selector).addClass("Highlight" + abandonRate);
            }
        },
        setView: function (overlayType) {
            Ektron.Analytics.Overlay.hide();

            $ektron('#page-overlay').remove();
            $ektron('#overlay-container').remove();


            $ektron('<div id="page-overlay"></div>').appendTo('body');
            $ektron('<div id="overlay-loading"></div>').appendTo('body');

            switch (overlayType) {
                case 'clicks':
                    Ektron.Analytics.Overlay.Clicks.show();
                    break;
                case 'browsersize':
                    break;
                case 'abandonpath':
                    Ektron.Analytics.Overlay.Abandon.show();
                    break;
                case 'clickpoint':
                    Ektron.Analytics.Overlay.ClickPoint.show();
                    break;
                case 'clickmap':
                    Ektron.Analytics.Overlay.ClickMap.show();
                    break;
                case 'heatmap':
                    Ektron.Analytics.Overlay.HeatMap.show();
                    break;
            }
        },
        hide: function () {
            Ektron.Analytics.Overlay.Abandon.hide();
            Ektron.Analytics.Overlay.Clicks.hide();
            Ektron.Analytics.Overlay.ClickPoint.hide();
            Ektron.Analytics.Overlay.ClickMap.hide();
            Ektron.Analytics.Overlay.HeatMap.hide();
        },
        remove: function () {
            Ektron.Analytics.Overlay.hide();
            $ektron('.ektronOverlayOuterBar').remove();
            $ektron('#EktronAnalyticsOverlay').remove();
            $ektron('#page-overlay').remove();
            $ektron('#overlay-container').remove();
            Ektron.Analytics.Overlay.removeOverlayCookie();
            document.location.href = document.location.href;
        },
        appPath: function () {
            if ('undefined' != typeof Ektron.Site && 'undefined' != typeof Ektron.Site.SiteData && 'undefined' != typeof Ektron.Site.SiteData.Cms && 'undefined' != typeof Ektron.Site.SiteData.Cms.ApplicationPath) {
                return Ektron.Site.SiteData.Cms.ApplicationPath;
            }

            return "";
        },
        appImgPath: function () {
            if ("" != Ektron.Analytics.Overlay.appPath()) {
                return Ektron.Analytics.Overlay.appPath() + "images/application/";
            }

            return "";
        },
        useSiteThemeRollerTheme: function () {
            if ('undefined' != typeof Ektron.Site && 'undefined' != typeof Ektron.Site.SiteData && 'undefined' != typeof Ektron.Site.SiteData.Ux && 'undefined' != typeof Ektron.Site.SiteData.Ux.LoadThemeRollerTheme) {
                return (Ektron.Site.SiteData.Ux.LoadThemeRollerTheme === 'true');
            }

            return false;
        },
        siteThemeRollerPath: function () {
            if ('undefined' != typeof Ektron.Site && 'undefined' != typeof Ektron.Site.SiteData && 'undefined' != typeof Ektron.Site.SiteData.Ux && 'undefined' != typeof Ektron.Site.SiteData.Ux.ThemeRollerPath) {
                return Ektron.Site.SiteData.Ux.ThemeRollerPath;
            }

            return "";
        },
        setOverlayCookie: function (value) {
            $ektron.cookie(Ektron.Analytics.Overlay.cookieKey, value, { expires: 365, "encoding": "none", "path": "/" });
        },
        removeOverlayCookie: function () {
            $ektron.cookie(Ektron.Analytics.Overlay.cookieKey, null, { expires: -1, "encoding": "none", "path": "/" });
        },
        getCookieKeyValue: function (key) {
            var cookieVal = $ektron.cookie(Ektron.Analytics.Overlay.cookieKey);
            if (cookieVal != null) {
                var cookieArray = cookieVal.split('&');
                for (var x = 0; x < cookieArray.length; x++) {
                    var cookieKey = cookieArray[x].split('=')[0];
                    if (cookieKey == key) {
                        return cookieArray[x].split('=')[1];
                    }
                }
            }

            return '';
        },
        setCookieKeyValue: function (key, value) {
            var flag = false;
            var matchIndex = -1;
            var cookieVal = $ektron.cookie(Ektron.Analytics.Overlay.cookieKey);
            if (cookieVal != null) {
                var cookieArray = cookieVal.split('&');
                for (var index = 0; index < cookieArray.length; index++) {
                    var cookieKey = cookieArray[index].split('=')[0];
                    if (cookieKey == key) {
                        cookieArray[index] = key + "=" + value;
                        flag = true;
                        matchIndex = index;
                        break;
                    }
                }
                if (matchIndex > -1) {
                    var newVal = '';
                    var delim = '';
                    for (var index = 0; index < cookieArray.length; index++) {
                        if (index != matchIndex) {
                            newVal += delim + cookieArray[index];
                            delim = '&';
                        }
                    }
                    Ektron.Analytics.Overlay.setOverlayCookie(newVal + delim + key + '=' + value);
                } else {
                    Ektron.Analytics.Overlay.setOverlayCookie(cookieVal + '&' + key + '=' + value);
                }
            } else {
                Ektron.Analytics.Overlay.setOverlayCookie(key + '=' + value);
            }
        },
        handleWindowResizing: function () {
            if (Ektron.Analytics.Overlay.resizingCallback != null) {
                Ektron.Analytics.Overlay.resizingCallback();
            }
        },
        resizingCallback: null,
        resizingTimer: null
    };
}

function Ektron_Analytics_Overlay_Initializer() {
    if ('undefined' == typeof Ektron || 'undefined' == typeof Ektron.Analytics || 'undefined' == typeof Ektron.Analytics.Overlay || 'undefined' == typeof Ektron.Analytics.Overlay.init) {
        window.setTimeout(Ektron_Analytics_Overlay_Initializer, 100);
        return;
    }
    Ektron.Analytics.Overlay.init();
}

function Ektron_Analytics_Overlay_HookEktronReady() {
    if ('undefined' == typeof Ektron || 'undefined' == typeof Ektron.ready) {
        window.setTimeout(Ektron_Analytics_Overlay_HookEktronReady, 100);
        return;
    }
    Ektron.ready(Ektron_Analytics_Overlay_Initializer);
}

Ektron_Analytics_Overlay_HookEktronReady();

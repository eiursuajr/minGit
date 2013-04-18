if (typeof (Ektron) == "undefined") Ektron = {};
if (typeof (Ektron.Widget) == "undefined") Ektron.Widget = {};
if (typeof (Ektron.Widget.Multivariate) == "undefined") Ektron.Widget.Multivariate = {};
if (typeof (Ektron.Widget.Multivariate.columns) == "undefined") Ektron.Widget.Multivariate.columns = {};

if (typeof (Ektron.Widget.MultivariateSection) == "undefined")
{
    Ektron.Widget.MultivariateSection =
    {
        data: {},
        Init: function (id, columnCount, selectedIndex, columns) {
            var sliders = $ektron("#" + id + " .slider");
            var itemNumberText = "/" + columnCount;

            var val;

            if (typeof (Ektron.Widget.MultivariateSection.data[id]) == "undefined") {
                Ektron.Widget.MultivariateSection.data[id] = { index: 0 };
            }

            if (selectedIndex > -1) {
                Ektron.Widget.MultivariateSection.data[id].index = selectedIndex;
            }
            if ($ektron.cookie && selectedIndex < 1) {
                var sliderval = $ektron.cookie('mvt_sliderval_' + id);
                if (sliderval && sliderval > 0) {
                    var columnid = sliderval - 1;
                    var displayedcolumns = $ektron("#" + id + " .columns-container").children().length;
                    if (columnCount > columnid && displayedcolumns > columnid) {
                        Ektron.Widget.MultivariateSection.data[id].index = columnid;
                    }
                }
            }

            if (Ektron.Widget.MultivariateSection.data[id].index >= columnCount) Ektron.Widget.MultivariateSection.data[id].index = columnCount - 1;
            val = Ektron.Widget.MultivariateSection.data[id].index + 1; //(selectedIndex > -1) ? selectedIndex+1 : 1;

            var last = $ektron($ektron("#" + id + " .columns-container").children()[val - 1]);
            var slide = function (i) {
                $ektron("#" + id + " .item-number").html((i + 1) + "/" + columnCount);
                last.css("display", "none");
                last = $ektron($ektron("#" + id + " .columns-container").children()[i]);
                last.css("display", "block");

                Ektron.Widget.MultivariateSection.data[id].index = i;
                sliders.slider('option', 'value', i + 1);
                if ($ektron.cookie) $ektron.cookie('mvt_sliderval_' + id, i + 1);
            }

            if (sliders.is(".ui-slider")) {
                sliders.slider('destroy');
            }
            sliders.slider({
                value: val,
                min: 1,
                max: columnCount,
                steps: columnCount - 1,
                slide: function (evt, ui) {
                    slide(ui.value - 1);
                }
            });

            var columnsVisible = 0;
            var $columns = $ektron("#" + id + " .columns-container").children();
            $columns.each(function (i) {
                var $element = $ektron($columns[i]);
                if ($element.attr("display") != "none") {
                    columnsVisible++;
                }
                $element.hide();
            });


            $ektron(columns).each(function (i) {

                Ektron.Widget.Multivariate.columns[columns[i]] = {
                    guid: columns[i],
                    index: i,
                    display: (function (j) {
                        return function () {
                            $ektron(sliders).slider('option', 'value', j + 1);
                            slide(j);
                        };
                    })(i)
                };
            });

            $ektron("#" + id + " .item-number").html("Variant " + val + "/" + columnCount);
            last.css("display", "block");

            $ektron("#" + id + " .add-variant").unbind('click');
            $ektron("#" + id + " .add-variant").click(function (evt, ui) {
                $ektron("#" + id + " .add-variant-btn").click();
            });
            //$ektron("#" + id).mouseover(function(evt){evt.stopPropagation();});
            if (columnsVisible != 1) {
                slide(1);
            }
            slide(val - 1);
        }
    }
}

if (typeof Sys != "undefined") {
//    Sys.Application.notifyScriptLoaded();
}
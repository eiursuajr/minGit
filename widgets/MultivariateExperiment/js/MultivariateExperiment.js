if (typeof (Ektron) == "undefined") Ektron = {};
if (typeof (Ektron.Widget) == "undefined") Ektron.Widget = {};
if (typeof (Ektron.Widget.Multivariate) == "undefined") Ektron.Widget.Multivariate = {};
if (typeof (Ektron.Widget.Multivariate.combinations) == "undefined") Ektron.Widget.Multivariate.combinations = {};
var isShowReport=false;
if (typeof (Ektron.Widget.MultivariateExperiment) == "undefined")
{
    Ektron.Widget.MultivariateExperiment =
    {
        widgets : {},
        Init : function(id)
        {
            var widget = $ektron("#" + id);
            
            $ektron("#" + id + " .promote-button").confirm({standalone:true,message:"Are you sure you want to promote this combination? This will remove all variations outside of this combination from the page."});

            var _id = id;
            
            var DisplayReport = function(visible)
            {
                widget.find(".show-report").css("display", visible ? "none" : "block");
                widget.find(".hide-report").css("display", visible ? "block" : "none");
                widget.find(".multivariate-experiment-content").css("display", visible ? "block" : "none");
            };
            
            widget.find(".show-report").click(function(evt)
            {
                DisplayReport(true);
                evt.stopPropagation();
                evt.preventDefault();
            });
            
            widget.find(".hide-report").click(function(evt)
            {
                DisplayReport(false);
                evt.stopPropagation();
                evt.preventDefault();
            });
            
            var dlg = $ektron("[data-ektron-pageTreeMarker=" + _id + "]");
            if (null == dlg || 0 == dlg.length){
                dlg = widget.find(".browse-dialog").dialog(
                {
                    autoOpen : false,
                    width: 600,
                    height: 400,
                    buttons : 
                    {
                        "Cancel" : function() {$ektron(this).dialog("close");}
                    }
                });
                dlg.attr("data-ektron-pageTreeMarker", _id);
                widget.find(".browse-dialog").hide();
            }
            
            Ektron.Widget.MultivariateExperiment.widgets[id] = 
            {
                SelectTarget : function(id, title)
                {
                    widget.find(".target-id").val(id);
                    $ektron("div.ui-dialog:has(span." + _id + ")").hide();
                }
            };
            
            widget.find(".target-page-browse").click(function(evt){dlg.dialog("open");});

            DisplayReport(isShowReport);
        },
        AddCombination : function(combination)
        {
            Ektron.Widget.Multivariate.combinations[combination.guid] = {
                guid: combination.guid,
                display: function()
                {
                    $ektron(combination.variantGuids).each(function(i)
                    {
                        Ektron.Widget.Multivariate.columns[combination.variantGuids[i]].display();
                    });
                }
            };
        },
        Preview : function(guid)
        {
            Ektron.Widget.Multivariate.combinations[guid].display();
            return false;
        }
    };
}

if (typeof Sys != "undefined") {
//    Sys.Application.notifyScriptLoaded();
}
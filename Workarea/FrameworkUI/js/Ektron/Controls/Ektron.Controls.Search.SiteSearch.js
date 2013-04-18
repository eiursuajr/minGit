if ("undefined" == typeof (Ektron)) { Ektron = {}; }
if ("undefined" == typeof (Ektron.Controls)) { Ektron.Controls = {}; }
if ("undefined" == typeof (Ektron.Controls.Search)) { Ektron.Controls.Search = {}; }
if ("undefined" == typeof (Ektron.Controls.Search.SiteSearch)) 
{
    Ektron.Controls.Search.SiteSearch = {
        //methods
        init: function (settings) {
            // set default parameters
            s = {
                clientId: null,
                zIndex: 1000
            };

            // extend defaults with the options passed in
            settings = $ektron.extend(s, settings);

            thisSeachControl = $ektron("#" + settings.clientId);
            var advanced = thisSeachControl.find(".advancedSearch");
            var basic = thisSeachControl.find(".basicSearch");
            var toggleLink = thisSeachControl.find("a.toggleAdvancedSearch");
            var toggleIcon = thisSeachControl.find("a.toggleAdvancedSearchIcon");

            //ensure return key causes submit on basic input
            $ektron(basic.find("input:text")).bindReturnKey(function (event, ui) {
                $ektron(ui).closest(".basicSearch").find(".ektron-ui-button .ui-button").click();
            });

            //ensure return key causes submit on basic input
            $ektron(advanced.find("input:text")).bindReturnKey(function (event, ui) {
                $ektron(ui).closest(".advancedSearch").find(".ektron-ui-button .ui-button").click();
            });

            // wrap the advanced search markup to comply with our best practices regarding CSS namespacing
            advanced = advanced.wrap('<div class="ektron-ui-control ektron-ui-search ektron-ui-search-siteSearch ektron-ui-hidden" />').removeClass("ektron-ui-hidden").parent();

            // position advanced search UI
            advanced.toTop().css({ "position": "absolute" });
            advanced.position({ my: "right top", at: "right bottom", of: basic, collision: "none, none" });

            // reposition when window resizes
            $ektron(window).resize(function () {
                advanced.position({ my: "right top", at: "right bottom", of: basic, collision: "none, none" });
            });

            // Set zIndex property, default value is 1000
            if (settings.zIndex !== undefined) {
                advanced.css("zIndex", settings.zIndex);
            }

            // bind advanced search toggle
            toggleLink.bind("click", {
                advancedPanel: advanced,
                basicPanel: basic,
                link: toggleLink,
                icon: toggleIcon
            }, Ektron.Controls.Search.SiteSearch.toggleAdvancedSearch);

            // clear the "JavascriptRequired" link
            toggleLink.attr("href", "");
            toggleIcon.attr("href", "");
        },
        toggleAdvancedSearch: function (event) {
            // get a handle on the toggle link 
            var currentState = event.data.link.is(".hideAdvancedSearch");

            if (currentState === true) {
                //  hide the advanced search UI
                event.data.link.removeClass("hideAdvancedSearch");
                event.data.advancedPanel.addClass("ektron-ui-hidden");
                event.data.basicPanel.find("a.toggleAdvancedSearch span.ui-icon").addClass("ui-icon-triangle-1-s").removeClass("ui-icon-triangle-1-n");
                event.data.basicPanel.find(".ui-state-disabled").removeClass("ui-state-disabled");
                event.data.basicPanel.find(".ui-autocomplete-input, .ektron-ui-button button").removeAttr("disabled");
            }
            else {
                // show the advanced search UI
                event.data.link.addClass("hideAdvancedSearch");
                event.data.advancedPanel.removeClass("ektron-ui-hidden");
                event.data.basicPanel.find("a.toggleAdvancedSearch span.ui-icon").addClass("ui-icon-triangle-1-n").removeClass("ui-icon-triangle-1-s");
                event.data.basicPanel.find(".ui-autocomplete-input, .ektron-ui-button button").addClass("ui-state-disabled");
                event.data.basicPanel.find(".ui-autocomplete-input, .ektron-ui-button button").attr("disabled", "disabled");
            }
            return false;
        }
    };
}
if ("undefined" == typeof (Ektron)) { Ektron = {}; }
if ("undefined" == typeof (Ektron.Controls)) { Ektron.Controls = {}; }
if ("undefined" == typeof (Ektron.Controls.Search)) { Ektron.Controls.Search = {}; }
if ("undefined" == typeof (Ektron.Controls.Search.UserSearch)) 
{
    Ektron.Controls.Search.UserSearch = {
        //methods
        init: function (settings) {
            // set default parameters
            s = {
                clientId: null
            };

            // extend defaults with the options passed in
            settings = $ektron.extend(s, settings);

            var thisUserSearchControl = $ektron("#" + settings.clientId);
            var directory = thisUserSearchControl.find(".directorySearch");
            var basic = thisUserSearchControl.find(".basicSearch");
            var toggleLink = thisUserSearchControl.find("a.toggleDirectorySearch");
            var toggleIcon = thisUserSearchControl.find("a.toggleDirectorySearchIcon");

            // clear the "JavascriptRequired" link
            toggleLink.attr("href", "");
            toggleIcon.attr("href", "");

            // bind directory search toggle
            toggleLink.bind("click", {
                directoryPanel: directory,
                basicPanel: basic,
                link: toggleLink,
                icon: toggleIcon
            }, Ektron.Controls.Search.UserSearch.toggleDirectorySearch);
        },
        toggleDirectorySearch: function (event) {
            // get a handle on the toggle link 
            var currentState = event.data.link.is(".hideDirectorySearch");
            if (currentState === true) {
                //  hide the directory search UI
                event.data.link.removeClass("hideDirectorySearch");
                event.data.directoryPanel.slideUp("slow");
                event.data.basicPanel.find("a.toggleDirectorySearch span.ui-icon").addClass("ui-icon-triangle-1-s").removeClass("ui-icon-triangle-1-n");
                event.data.basicPanel.find(".ui-button").button("option", "disabled", false);
                event.data.basicPanel.find(".ektron-ui-input input").removeAttr("disabled").removeClass("ui-state-disabled");
            }
            else {
                // show the directory search UI
                event.data.link.addClass("hideDirectorySearch");
                event.data.directoryPanel.slideDown("slow");
                event.data.basicPanel.find("a.toggleDirectorySearch span.ui-icon").addClass("ui-icon-triangle-1-n").removeClass("ui-icon-triangle-1-s");
                event.data.basicPanel.find(".ui-button").button("option", "disabled", true);
                event.data.basicPanel.find(".ektron-ui-input input").attr("disabled", "disabled").addClass("ui-state-disabled");
            }
            return false;
        }
    };
}
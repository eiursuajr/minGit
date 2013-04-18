﻿if ("undefined" == typeof (Ektron)) { Ektron = {}; }
if ("undefined" == typeof (Ektron.Controls)) { Ektron.Controls = {}; }
if ("undefined" == typeof (Ektron.Controls.EktronUI)) { Ektron.Controls.EktronUI = {}; }
if ("undefined" == typeof (Ektron.Controls.EktronUI.ButtonSet)) {
    Ektron.Controls.EktronUI.ButtonSet = {
        init: function (ui) {
            var buttonSet = $ektron(ui);
            var lastLabel = buttonSet.find("label:last");
            if (!lastLabel.hasClass("ui-corner-right")) {
                lastLabel.addClass("ui-corner-right");
            }
        }
    }; 
}
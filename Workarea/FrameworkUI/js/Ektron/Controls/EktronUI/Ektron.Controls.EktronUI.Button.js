if ("undefined" == typeof (Ektron)) { Ektron = {}; }
if ("undefined" == typeof (Ektron.Controls)) { Ektron.Controls = {}; }
if ("undefined" == typeof (Ektron.Controls.EktronUI)) { Ektron.Controls.EktronUI = {}; }
if ("undefined" == typeof (Ektron.Controls.EktronUI.Button)) {
    Ektron.Controls.EktronUI.Button = {
        RadioButton: {
            click: function (ui) {
                var label = $ektron(ui).next();

                //get value proxy control for clicked radio button and store correct value
                var valueProxy = label.find(":hidden");

                //clear values for all proxy value controls in button set
                var parent = label.parent();
                parent.find(":hidden").attr("value", "false");

                //set selected value
                valueProxy.attr("value", "true");
            }
        }
    };
}
if ("undefined" == typeof (Ektron)) { Ektron = {}; }
if ("undefined" == typeof (Ektron.Controls)) { Ektron.Controls = {}; }
if ("undefined" == typeof (Ektron.Controls.Search)) {Ektron.Controls.Search = {}; }
if ("undefined" == typeof (Ektron.Controls.Search.XmlSearch)) 
{
    Ektron.Controls.Search.XmlSearch = {
        //methods
        init: function (settings) {
            // set default parameters
            s = {
                clientId: null
            };

            // extend defaults with the options passed in
            settings = $ektron.extend(s, settings);

            var thisUserSearchControl = $ektron("#" + settings.clientId);

            //ensure return key causes submit on basic input
            $ektron(thisUserSearchControl.find("input:text:not(:hidden)")).bindReturnKey(function (event, ui) {
                $ektron(ui).closest(".ektron-ui-search-xml").find(".ektron-ui-button .ui-button").click();
            });
        }
    }; 
}
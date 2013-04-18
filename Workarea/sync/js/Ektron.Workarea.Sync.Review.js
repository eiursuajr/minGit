// Define Ektron object only if it's not already defined
if (typeof (Ektron) == "undefined") {
    Ektron = {};
}

// Define Ektron.Workarea object only if it's not already defined
if (typeof (Ektron.Workarea) == "undefined") {
    Ektron.Workarea = {};
}

// Define Ektron.Workarea.Sync object only if it's not already defined
if (typeof (Ektron.Workarea.Sync) == "undefined") {
    Ektron.Workarea.Sync = {};
}

// Define Ektron.Workarea.Sync.Review object only if it's not already defined
if (typeof (Ektron.Workarea.Sync.Review) == "undefined") {
    Ektron.Workarea.Sync.Review =
    {
        Init: function() {
            $ektron(".checkUncheckAll").click(function() {
                Ektron.Workarea.Sync.Review.ToggleCheckBoxes(this);
            });

            var foldersTable = $ektron("#folders .conflictsList tbody");
            var usersTable = $ektron("#users .conflictsList tbody");
            var metadataTable = $ektron("#metadata .conflictsList tbody");
            var emailsTable = $ektron("#email .conflictsList tbody");

            if (foldersTable.children().length == 0) {
                $ektron("#foldersTab, #folders").hide();
            }
            else {
                $ektron("#foldersTab, #folders").show();
            }

            if (metadataTable.children().length == 0) {
                $ektron("#metadataTab, #metadata").hide();
            }
            else {
                $ektron("#metadataTab, #metadata").show();
            }

            if (emailsTable.children().length == 0) {
                $ektron("#emailTab, #email").hide();
            }
            else {
                $ektron("#emailTab, #email").show();
            }

            if (usersTable.children().length === 0) {
                $ektron("#usersTab, #users").hide();
            }
            else {
                $ektron("#usersTab, #users").show();
            }

            $ektron("#tabWrapper > ul").tabs({ fx: { opacity: 'toggle'} });
            $ektron("#tabWrapper > ul").css("width", "100%");

            if (foldersTable.children().length > 0 ||
                usersTable.children().length > 0 ||
                metadataTable.children().length > 0 ||
                emailsTable.children().length > 0) {
                
                var firstVisibleTab = $ektron("#tabWrapper > ul li").not($ektron(":hidden")).filter(":first").attr("id");
                var selectTab = firstVisibleTab.substring(0, firstVisibleTab.lastIndexOf("Tab"));
                $ektron("#tabWrapper").tabs("select", "#" + selectTab);
            }
        },

        ToggleCheckBoxes: function(parentCheckBox) {
            var parent = $ektron(parentCheckBox).parent().parent().parent().parent();
            if (parent != null) {
                parent.find("input.conflictCheckBox").attr(
                    "checked",
                    $ektron(parentCheckBox).is(":checked"));
            }
        }
    }
}

//define Ektron object only if it's not already defined
if (typeof (Ektron) == "undefined") {
    Ektron = {};
}

//define Ektron.Workarea object only if it's not already defined
if (typeof (Ektron.Workarea) == "undefined") {
    Ektron.Workarea = {};
}

//define Ektron.Workarea object only if it's not already defined
if (typeof (Ektron.Workarea.Sync) == "undefined") {
    Ektron.Workarea.Sync = {};
}

//define Ektron.Workarea object only if it's not already defined
if (typeof (Ektron.Workarea.Sync.Profile) == "undefined") {
    Ektron.Workarea.Sync.Profile =
    {
        mode: null,

        Init: function () {

            Ektron.Workarea.Sync.Profile.mode = Ektron.Workarea.Sync.Profile.GetDisplayMode();
            $ektron("input[name='rdoConflictResolution']").change(function () { Ektron.Workarea.Sync.Profile.EnableConflictPolicyDescription(); });
            $ektron("#chkBinaries").change(function () { Ektron.Workarea.Sync.Profile.HandleBinariesChecked(); });
            $ektron("#chkTemplates").change(function () { Ektron.Workarea.Sync.Profile.HandleTemplatesChecked(); });
            $ektron("input[name='rdoFilters']").change(function () { Ektron.Workarea.Sync.Profile.EnableFilterBoxes(); });
            $ektron("#chkDatabase").change(function () { Ektron.Workarea.Sync.Profile.HandleDatabaseChecked(); });
            $ektron(".dbscopeitems input").change(function () { Ektron.Workarea.Sync.Profile.HandleScopeChecked(); });


            Ektron.Workarea.Sync.Profile.EnableFilterBoxes();
            Ektron.Workarea.Sync.Profile.EnableConflictPolicyDescription();
            Ektron.Workarea.Sync.Profile.MakeScopeHover();
            $ektron("#ulItemsToSynchronize input").change(function () { Ektron.Workarea.Sync.Profile.EnableFilterInput(); });
            Ektron.Workarea.Sync.Profile.EnableFilterInput();
        },

        GetDisplayMode: function () {
            return $ektron("#hdnDisplayMode").attr("value");
        },

        EnableFilterInput: function () {
            if (Ektron.Workarea.Sync.Profile.mode != "View") {
                var enableFilters = true;
                var databaseChecked = false;

                var itemsToSynchronize = $ektron("#ulItemsToSynchronize input:checked");
                var workareaChkBox = $ektron('#chkWorkarea').is(':checked');
                var templateChkBox = $ektron('#chkTemplates').is(':checked');
                var binariesChkBox = $ektron('#chkBinaries').is(':checked');

                if (workareaChkBox == false && templateChkBox == false && binariesChkBox == false) {
                    enableFilters = false;
                }
                else {
                    itemsToSynchronize.each(
                    function (index, element) {
                        if (element.id == "chkDatabase") {
                            enableFilters = false;
                            databaseChecked = true;
                        }
                    });
                }

                if (enableFilters) {
                    $ektron("input[name='rdoFilters']").removeAttr("disabled");
                }
                else {
                    $ektron("input[name='rdoFilters']").attr("disabled", "disabled");
                    $ektron("input[name='rdoFilters']").removeAttr("checked");
                    $ektron("#rdoFilters_0").attr("checked", "checked");
                    Ektron.Workarea.Sync.Profile.EnableFilterBoxes();
                }
            }
        },

        EnableFilterBoxes: function () {
            if (Ektron.Workarea.Sync.Profile.mode != "View") {
                var filterSelection = $ektron("input[name='rdoFilters']:checked").attr("value");
                switch (filterSelection) {
                    case "None":
                        $ektron("#txtFileFilters").attr("disabled", "disabled");
                        $ektron("#txtDirectoryFilters").attr("disabled", "disabled");

                        $ektron("#txtFileFilters").attr("value", "");
                        $ektron("#txtDirectoryFilters").attr("value", "");
                        break;
                    case "Include":
                        $ektron("#txtFileFilters").removeAttr("disabled");
                        $ektron("#txtDirectoryFilters").attr("disabled", "disabled");

                        $ektron("#txtDirectoryFilters").attr("value", "");
                        break;
                    case "Exclude":
                        $ektron("#txtFileFilters").removeAttr("disabled");
                        $ektron("#txtDirectoryFilters").removeAttr("disabled");
                        break;
                }
            }
        },

        EnableConflictPolicyDescription: function () {
            var conflictPolicy = $ektron("input[name='rdoConflictResolution']:checked").attr("value");
            if (conflictPolicy == "SourceWins") {
                $ektron("#lblSourceWinsDesc").show();
                $ektron("#lblDestinationWinsDesc").hide();
            }
            else {
                $ektron("#lblSourceWinsDesc").hide();
                $ektron("#lblDestinationWinsDesc").show();
            }
        },

        Save: function () {
            // TODO: Input validation.
            document.forms[0].submit();
        },

        ConfirmDelete: function () {
            return confirm(Ektron.Workarea.Sync.Resources.deleteProfileDialogMessage);
        },

        HandleBinariesChecked: function () {
            if ($ektron("#chkBinaries").is(":checked")) {
                $ektron("#chkTemplates").attr("checked", "checked");
            }
        },

        HandleTemplatesChecked: function () {
            if (!$ektron("#chkTemplates").is(":checked")) {
                $ektron("#chkBinaries").removeAttr("checked");
            }
        },

        HandleDatabaseChecked: function () {
            if (!$ektron("#chkDatabase").is(":checked")) {
                $ektron(".dbscopeitems input").removeAttr("checked");
            }
        },

        HandleScopeChecked: function () {
            if (!$ektron(".dbscopeitems input").is(":checked")) {
                $ektron("#chkDatabase").removeAttr("checked");
            } else {
                $ektron("#chkDatabase").attr("checked", "checked");
            }
        },

        MakeScopeHover: function () {
            $ektron('.dbscopeitems span label').attr('for', '').hover(function () {
                $ektron(this).addClass('scopehover');
            },
            function () {
                $ektron(this).removeClass('scopehover');
            }
            );

            $ektron('.dbscopeitems span[title]').each(function () {
                $(this).find('label').attr('title', $(this).attr('title'));
            });

            $ektron('.dbscopeitems span label').cluetip({ activation: 'click', sticky: true, mouseOutClose: false, positionBy: 'bottomTop', cursor: 'pointer', arrows: true, leftOffset: "35px", topOffset: "20px", cluezIndex: 9999, splitTitle: '|', showTitle: false, cluetipClass: 'jtip', closePosition: 'title' });
            $ektron('.dbscopeitems span[title]').unbind('click');
        }
    };
}

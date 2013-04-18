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

if (typeof (Ektron.Workarea.Sync.Schedule) == "undefined") {
    Ektron.Workarea.Sync.Schedule =
    {
        controlPrefix: null,

        Init: function() {
            Ektron.Workarea.Sync.Schedule.controlPrefix = Ektron.Workarea.Sync.Schedule.GetControlPrefix();
            Ektron.Workarea.Sync.Schedule.ShowScheduleInput();

            // Bind events for schedule interval selection.
            $ektron("input[name='" + Ektron.Workarea.Sync.Schedule.GetInputGroupName("rdoSchedule") + "']").click(function() { Ektron.Workarea.Sync.Schedule.ShowScheduleInput(); });

            // Bind events for monthly schedule selection.
            $ektron("input[name='" + Ektron.Workarea.Sync.Schedule.GetInputGroupName("rdoSchedule") + "']").click(function() { Ektron.Workarea.Sync.Schedule.WarnOnMonthlySelection(); });
            $ektron("#" + Ektron.Workarea.Sync.Schedule.GetInputId("ddlMonthlyDay")).change(function() { Ektron.Workarea.Sync.Schedule.WarnOnMonthlySelection(); });
        },

        GetControlPrefix: function() {
            return "ssScheduleOptions";
        },

        GetInputGroupName: function(identifier) {
            return Ektron.Workarea.Sync.Schedule.controlPrefix + "$" + identifier;
        },

        GetInputId: function(identifier) {
            return Ektron.Workarea.Sync.Schedule.controlPrefix + "_" + identifier;
        },

        ShowScheduleInput: function() {
            var selectedElement = $ektron("input[name='" + Ektron.Workarea.Sync.Schedule.GetInputGroupName("rdoSchedule") + "']:checked");
            if (selectedElement.length > 0) {
                var interval = selectedElement.attr("value");
                if (typeof (interval) != "undefined") {
                    switch (interval) {
                        case "None":
                            $ektron("#scheduleFieldset").addClass("displayNone");
                            $ektron("#divOneTimeSchedule").addClass("displayNone");
                            $ektron("#divHourlySchedule").addClass("displayNone");
                            $ektron("#divDailySchedule").addClass("displayNone");
                            $ektron("#divWeeklySchedule").addClass("displayNone");
                            $ektron("#divMonthlySchedule").addClass("displayNone");
                            break;
                        case "OneTime":
                            $ektron("#scheduleFieldset").removeClass("displayNone");
                            $ektron("#divOneTimeSchedule").removeClass("displayNone");
                            $ektron("#divHourlySchedule").addClass("displayNone");
                            $ektron("#divDailySchedule").addClass("displayNone");
                            $ektron("#divWeeklySchedule").addClass("displayNone");
                            $ektron("#divMonthlySchedule").addClass("displayNone");
                            break;
                        case "Hourly":
                            $ektron("#scheduleFieldset").removeClass("displayNone");
                            $ektron("#divOneTimeSchedule").addClass("displayNone");
                            $ektron("#divHourlySchedule").removeClass("displayNone");
                            $ektron("#divDailySchedule").addClass("displayNone");
                            $ektron("#divWeeklySchedule").addClass("displayNone");
                            $ektron("#divMonthlySchedule").addClass("displayNone");
                            break;
                        case "Daily":
                            $ektron("#scheduleFieldset").removeClass("displayNone");
                            $ektron("#divOneTimeSchedule").addClass("displayNone");
                            $ektron("#divHourlySchedule").addClass("displayNone");
                            $ektron("#divDailySchedule").removeClass("displayNone");
                            $ektron("#divWeeklySchedule").addClass("displayNone");
                            $ektron("#divMonthlySchedule").addClass("displayNone");
                            break;
                        case "Weekly":
                            $ektron("#scheduleFieldset").removeClass("displayNone");
                            $ektron("#divOneTimeSchedule").addClass("displayNone");
                            $ektron("#divHourlySchedule").addClass("displayNone");
                            $ektron("#divDailySchedule").addClass("displayNone");
                            $ektron("#divWeeklySchedule").removeClass("displayNone");
                            $ektron("#divMonthlySchedule").addClass("displayNone");
                            break;
                        case "Monthly":
                            $ektron("#scheduleFieldset").removeClass("displayNone");
                            $ektron("#divOneTimeSchedule").addClass("displayNone");
                            $ektron("#divHourlySchedule").addClass("displayNone");
                            $ektron("#divDailySchedule").addClass("displayNone");
                            $ektron("#divWeeklySchedule").addClass("displayNone");
                            $ektron("#divMonthlySchedule").removeClass("displayNone");
                            break;
                    }
                }
            }
        },

        WarnOnMonthlySelection: function() {
            var selectedElement = $ektron("input[name='" + Ektron.Workarea.Sync.Schedule.GetInputGroupName("rdoSchedule") + "']:checked");
            if (selectedElement.length > 0 && selectedElement.attr("value") == "Monthly") {
                var selectedDayElement = $ektron("#" + Ektron.Workarea.Sync.Schedule.GetInputId("ddlMonthlyDay"));
                if (selectedDayElement.length > 0) {
                    var selectedDay = parseInt(selectedDayElement.attr("value"));
                    if (selectedDay > 28) {
                        alert(Ektron.Workarea.Sync.Resources.monthlyScheduleWarning);
                    }
                }
            }
        }
    };
}

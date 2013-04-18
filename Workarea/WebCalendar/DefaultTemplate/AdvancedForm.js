//Define Ektron.PageBuilder object only if it's not already defined
if (Ektron.WebCalendar === undefined) {
    Ektron.WebCalendar = {};
}

//Ektron PageBuilder Object
Ektron.WebCalendar.AdvancedForm = {
    appPath: "",
    init: function (containerID, appPath, isinworkarea) {
        if (isinworkarea) {
            $ektron("form").addClass("WebCalendarEdit");
            window.jsEnableWorkareaNav = false;
        }
        Ektron.WebCalendar.AdvancedForm.appPath = appPath;
        $ektron(".rsAdvOptionsScroll").tabs({
            show: function (event, ui) {
                Ektron.WebCalendar.AdvancedForm.Validation.HideError();
                return true;
            }
        });
        Ektron.WebCalendar.AdvancedForm.hiddenValues.Init();
        Ektron.WebCalendar.AdvancedForm.DatePicking.DateInit(containerID);
        Ektron.WebCalendar.AdvancedForm.Recurrence.RecurrenceInit();
        Ektron.WebCalendar.AdvancedForm.Alias.AliasDropDownInit();
        Ektron.WebCalendar.AdvancedForm.Taxonomy.Init();
        Ektron.WebCalendar.AdvancedForm.Validation.Init();

        var alldaycheck = $ektron("#" + containerID + " .allday input");
        if (alldaycheck.length > 0 && alldaycheck[0].checked) {
            var starttime = $ektron("#" + containerID + " .starttime");
            var enddate = $ektron("#" + containerID + " .enddate");
            var startdate = $ektron("#" + containerID + " .startdate");
            starttime.hide();
            startdate.parent().find(".ui-timepicker-trigger").parent().hide();
            $ektron("#" + containerID + " .lblEndDate").hide();
            enddate.hide()
            enddate.parent().find(".ui-datepicker-trigger").hide();
            $ektron("#" + containerID + " .endtime").hide();
            enddate.parent().find(".ui-timepicker-trigger").parent().hide();
        }
    },
    destroy: function (reshow) {
        try {
            if (window != null) {
                window.jsEnableWorkareaNav = true;  // enable navigation tabs in workarea
                if (typeof window.RadEditorGlobalArray != 'undefined') {
                    var length = window.RadEditorGlobalArray.length;
                    if (length > 0) {
                        for (var i = 0; i < length; i++) {
                            if (document.getElementById(window.RadEditorGlobalArray[i].Id) == null) {
                                window.RadEditorGlobalArray.splice(i, 1);
                                length--;
                                i--;
                            }
                        }
                    }
                }
            }
        } catch (ex) { }

        if (typeof reshow == 'boolean' && reshow) {
            window.jsEnableWorkareaNav = true;
        }
        $ektron("form").removeClass("WebCalendarEdit");
        return true;
    },

    hiddenValues: {
        Init: function () {
            Ektron.WebCalendar.AdvancedForm.hiddenValues.UserCulture = $ektron(".rsAdvOptions .initialization .initUserCulture").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.TimeDisplayFormat = $ektron(".rsAdvOptions .initialization .initTimeDisplayFormat").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrTitleRequired = $ektron(".rsAdvOptions .initialization .initErrTitleRequired").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrStartRequired = $ektron(".rsAdvOptions .initialization .initErrStartRequired").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrEndRequired = $ektron(".rsAdvOptions .initialization .initErrEndRequired").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrMetaDataRequired = $ektron(".rsAdvOptions .initialization .initErrMetaDataRequired").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrTaxonomyRequired = $ektron(".rsAdvOptions .initialization .initErrTaxonomyRequired").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrAliasRequired = $ektron(".rsAdvOptions .initialization .initErrAliasRequired").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrStartBeforeEnd = $ektron(".rsAdvOptions .initialization .initErrStartBeforeEnd").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.Time8AM = $ektron(".rsAdvOptions .initialization .initTime8AM").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.Time9AM = $ektron(".rsAdvOptions .initialization .initTime9AM").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.TimeDayStart = $ektron(".rsAdvOptions .initialization .initTimeDayStart").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.CalendarButton = $ektron(".rsAdvOptions .initialization .initCalendarButton").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.TimePickButton = $ektron(".rsAdvOptions .initialization .initTimePickButton").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.CalendarButtonAlt = $ektron(".rsAdvOptions .initialization .initCalendarButtonAlt").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.TimePickButtonAlt = $ektron(".rsAdvOptions .initialization .initTimePickButtonAlt").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrorIcon = $ektron(".rsAdvOptions .initialization .initErrorIcon").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.LocationMaxLength = $ektron(".rsAdvOptions .initialization .initLocationMaxLength").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.TitleMaxLength = $ektron(".rsAdvOptions .initialization .initTitleMaxLength").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.InvalidCharTitle = $ektron(".rsAdvOptions .initialization .initInvalidCharTitle").val();
            Ektron.WebCalendar.AdvancedForm.hiddenValues.InvalidCharLocation = $ektron(".rsAdvOptions .initialization .initInvalidCharLocation").val();
        },
        UserCulture: "",
        TimeDisplayFormat: "",
        ErrTitleRequired: "",
        ErrStartRequired: "",
        ErrEndRequired: "",
        ErrMetaDataRequired: "",
        ErrTaxonomyRequired: "",
        ErrAliasRequired: "",
        ErrStartBeforeEnd: "",
        Time8AM: "",
        Time9AM: "",
        TimeDayStart: "",
        CalendarButton: "",
        TimePickButton: "",
        ErrorIcon: "",
        LocationMaxLength: "",
        TitleMaxLength: "",
        InvalidCharTitle: "",
        InvalidCharLocation: ""
    },
    Validation: {
        Init: function () {
            var numerics = $ektron("div.rsAdvOptions .ui-numeric-input");
            //block chars for numerics
            numerics.bind("keypress", function (event) {
                var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which;

                var val = this.value;
                if (val == "") val = 0;
                val = parseInt(val, 10);
                if (k == 38) {//increment
                    this.value = val + 1;
                    return true;
                }
                if (k == 40) {//decrement
                    if (val > 0) {
                        this.value = val - 1;
                        return true;
                    } else {
                        $ektron(this).addClass("ui-numeric-input-error");
                        window.setTimeout("Ektron.WebCalendar.AdvancedForm.Validation.ClearTextBoxError()", 750);
                        //show Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrorIcon in this textbox
                        return false;
                    }
                }

                if ((this.value.length > 2 || isNaN(parseInt(String.fromCharCode(k), 10))) &&
                            !(k == 8 || k == 37 || k == 39 || k == 16)) {
                    $ektron(this).addClass("ui-numeric-input-error");
                    window.setTimeout("Ektron.WebCalendar.AdvancedForm.Validation.ClearTextBoxError()", 750);
                    //show Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrorIcon in this textbox
                    return false;
                }
            }).bind("change", function (event) {
                $this = $ektron(this);
                var newval = "";
                var curval = $this.val();
                //strip non-numerics
                var numregex = /[^0-9]/g
                newval = curval.replace(numregex, "");
                $this.val(newval);
            }).bind("paste", function (event) {
                var el = this;
                setTimeout(function () {
                    $this = $ektron(el);
                    var newval = "";
                    var curval = $this.val();
                    //strip non-numerics
                    var numregex = /[^0-9]/g
                    newval = curval.replace(numregex, "");
                    $this.val(newval);
                }, 30);
            });

            $ektron(".rsAdvOptions .titleTextBox").bind("keypress", function (event) {
                var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which;
                var cha = String.fromCharCode(k);
                if (cha == "*" || cha == "/" || cha == "|" || cha == "\"" || cha == ">" || cha == "<" || cha == "\\") {
                    return false;
                }
            });
        },
        ClearTextBoxError: function () {
            $ektron(".ui-numeric-input-error").removeClass("ui-numeric-input-error");
        },
        DisplayError: function (error) {
            var show = $ektron(".rsAdvancedSubmitArea .rsErrors");
            var msgcontainer = show.find("strong");
            msgcontainer.text(error);
            show.show();
            return false;
        },
        HideError: function () {
            var show = $ektron(".rsAdvancedSubmitArea .rsErrors");
            show.hide();
        },
        Validate: function () {
            $ektron(".rsAdvancedSubmitArea .rsErrors").hide();

            //title (not blank)
            //start date, end date (don't allow typing for dates, implement timepicker for time, make sure end time is after start time)
            //metadata, taxonomy (required fields)
            var allday = $ektron(".rsAdvOptions .rsAllDayWrapper .allday input");
            var title = $ektron(".rsAdvOptions .titleTextBox");
            var location = $ektron(".rsAdvOptions .locationTextBox");
            var startdate = $ektron(".rsAdvOptions .startdate");
            var starttime = $ektron(".rsAdvOptions .starttime");
            var enddate = $ektron(".rsAdvOptions .enddate");
            var endtime = $ektron(".rsAdvOptions .endtime");
            if (title.length == 0 || startdate.length == 0 || starttime.length == 0 || enddate.length == 0 || endtime.length == 0) {
                alert("Could not find form element.");
                return false;
            }

            //check title here
            if (title.val() == "") {
                return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrTitleRequired);
            }
            if (title.val().length > 200) {
                return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.TitleMaxLength);
            }

            //check location here
            if (location.val().length > 300) {
                return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.LocationMaxLength);
            }

            //check for invalid chars here
            var invalidchars = ["*", "/", "|", "\"", ">", "<", "\\"];
            for (var i = 0; i < invalidchars.length; i++) {
                if (title.val().indexOf(invalidchars[i]) > -1)
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.InvalidCharTitle + " (" + invalidchars[i] + ")");
                if (location.val().indexOf(invalidchars[i]) > -1)
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.InvalidCharLocation + " (" + invalidchars[i] + ")");
            }

            //check that start and end date are parseable here
            try {
                startdate = $ektron.global.parseDate(startdate.val(), null, Ektron.WebCalendar.AdvancedForm.hiddenValues.UserCulture);
            } catch (ex) {
                return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrStartRequired);
            }

            if (!allday[0].checked) {
                try {
                    enddate = $ektron.global.parseDate(enddate.val(), null, Ektron.WebCalendar.AdvancedForm.hiddenValues.UserCulture);
                } catch (ex) {
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrEndRequired);
                }
            }

            //start datetime
            var re = new RegExp("^(([0-9]+?):([0-5][0-9])( )?((A|P|a|p)(M|m))?)$");
            starttime = starttime.val();
            if (!starttime.match(re)) {
                return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrStartRequired);
            } else {
                var timecomponents = starttime.split(":");
                timecomponents[0] = parseInt(timecomponents[0], 10);
                timecomponents[1] = parseInt(timecomponents[1], 10);
                if (timecomponents[0] == "NaN" || timecomponents[1] == "NaN") {
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrStartRequired);
                } else {
                    if (starttime.toUpperCase().indexOf("PM") > -1) timecomponents[0] += 12;
                    if (starttime.toUpperCase().indexOf("PM") > -1 && timecomponents[0] == 24) timecomponents[0] = 12;
                    if (starttime.toUpperCase().indexOf("AM") > -1) timecomponents[0] = timecomponents[0] % 12;
                    if (timecomponents[0] < 0 || timecomponents[0] > 23 || timecomponents[1] < 0 || timecomponents[1] > 59) {
                        return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrStartRequired);
                    } else {
                        startdate.setHours(timecomponents[0], timecomponents[1]);
                    }
                }
            }

            if (!allday[0].checked) {
                //end datetime
                endtime = endtime.val();
                if (!endtime.match(re)) {
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrEndRequired);
                } else {
                    var timecomponents = endtime.split(":");
                    timecomponents[0] = parseInt(timecomponents[0], 10);
                    timecomponents[1] = parseInt(timecomponents[1], 10);
                    if (timecomponents[0] == "NaN" || timecomponents[1] == "NaN") {
                        return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrEndRequired);
                    } else {
                        if (endtime.toUpperCase().indexOf("PM") > -1) timecomponents[0] += 12;
                        if (endtime.toUpperCase().indexOf("PM") > -1 && timecomponents[0] == 24) timecomponents[0] = 12;
                        if (endtime.toUpperCase().indexOf("AM") > -1) timecomponents[0] = timecomponents[0] % 12;
                        if (timecomponents[0] < 0 || timecomponents[0] > 23 || timecomponents[1] < 0 || timecomponents[1] > 59) {
                            return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrEndRequired);
                        } else {
                            enddate.setHours(timecomponents[0], timecomponents[1]);
                        }
                    }
                }

                //check that startdate is less than enddate
                if (startdate >= enddate) {
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrStartBeforeEnd);
                }
            }

            //check if taxonomy is required. if so, make sure it's selected
            if ($ektron("#Taxonomy .TaxRequiredBool").text() == "true") {
                var selectednodes = $ektron("#Taxonomy .hdnSelectedNodes input").val();
                if (selectednodes == "") {
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrTaxonomyRequired);
                }
            }

            //check if alias is required. if so, make sure alias name is entered
            if ($ektron("#Alias .AliasRequiredBool").text() == "true") {
                var manualaliasname = $ektron("#Alias input.AliasName").val();
                if (manualaliasname == "") {
                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrAliasRequired);
                }
            }

            //metadata
            var required = $ektron("#Metadata input[name='req_fields']");
            if (required.length > 0) {
                required = required.val();
                if (required.length > 0) {
                    required = required.split(",");
                    for (var i = 0; i < required.length; i++) {
                        if (required[i] != "") {
                            var value = $ektron("#Metadata #" + required[i] + ", #Metadata input[name='" + required[i] + "']").val();
                            if (value == null || value == "") {
                                return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError(Ektron.WebCalendar.AdvancedForm.hiddenValues.ErrMetaDataRequired);
                            }
                        }
                    }
                }
            }

            //recurrence
            var recurring = $ektron("#Recurrence .recurCheck input");
            if (recurring.length > 0 && recurring[0].checked) {
                var recurenddate = $ektron("#Recurrence .rsAdvRecurrenceRangePanel span.rsAdvRadio input:checked");
                if (recurenddate[0].value == "RepeatGivenOccurrences") {
                    var txtbox = recurenddate.parent().parent().find("input:text");
                    if (txtbox.val() == "") {
                        return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError("Specify number of occurences to end after");
                    }
                }
                if (recurenddate[0].value == "RepeatUntilGivenDate") {
                    var datebox = recurenddate.parent().parent().find("input.datetime");
                    var timebox = recurenddate.parent().parent().find("input.timepick");
                    if (datebox.val() == "") {
                        return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError("Specify end date for recurrence");
                    }
                    if (timebox.val() == "") {
                        return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError("Specify end time for recurrence");
                    }
                    else {
                        var re = new RegExp("^(([0-9]+?):([0-5][0-9])( )?((A|P|a|p)(M|m))?)$");
                        timebox = timebox.val();
                        if (!timebox.match(re)) {
                            return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError("Please enter a valid end time");
                        } else {
                            var timecomponents = timebox.split(":");
                            timecomponents[0] = parseInt(timecomponents[0], 10);
                            timecomponents[1] = parseInt(timecomponents[1], 10);
                            if (timecomponents[0] == "NaN" || timecomponents[1] == "NaN") {
                                return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError("Please enter a valid end time");
                            } else {
                                if (timebox.toUpperCase().indexOf("PM") > -1) timecomponents[0] += 12;
                                if (timebox.toUpperCase().indexOf("PM") > -1 && timecomponents[0] == 24) timecomponents[0] = 12;
                                if (timebox.toUpperCase().indexOf("AM") > -1) timecomponents[0] = timecomponents[0] % 12;
                                if (timecomponents[0] < 0 || timecomponents[0] > 23 || timecomponents[1] < 0 || timecomponents[1] > 59) {
                                    return Ektron.WebCalendar.AdvancedForm.Validation.DisplayError("Please enter a valid end time");
                                }
                            }
                        }
                    }
                }
            }

            window.jsEnableWorkareaNav = true;
            $ektron("form").removeClass("WebCalendarEdit");
            return true;
        }
    },
    Alias: {
        AliasDropDownInit: function () {
            $ektron(".AliasDropDown").change(function (e) {
                var aliasextension = $ektron(".AliasExtension");
                aliasextension.val($ektron(".AliasDropDown").val());
            });
        }
    },
    Taxonomy: {
        Init: function () {
            $ektron(".AdvCalendarSelect .CalendarSelect").change(function () {
                var selected = $ektron(".AdvCalendarSelect .CalendarSelect").val();
                Ektron.WebCalendar.AdvancedForm.Taxonomy.reInit(selected);
            });
        },
        reInit: function (folderID) {
            var container = $ektron("div#Taxonomy");
            var selectednodesInput = container.find(".hdnSelectedNodes input");
            var noTaxonomyLabel = container.find(".NoTaxonomiesLabel");
            var taxRequired = $ektron("div#Taxonomy .TaxRequiredBool");
            var metadata = $ektron("div#Metadata>*:not(div.EnhancedMetadataBasic)");
            var taxonomydata = $ektron("div#Taxonomy .hdnSelectedNodes");
            var hdnInputid = selectednodesInput[0].id;

            taxRequired.text("false");
            taxonomydata.html("");
            noTaxonomyLabel.text("");
            container.find(".treecontainer").remove();
            $ektron.ajax({
                type: "POST",
                cache: false,
                async: false,
                url: Ektron.WebCalendar.AdvancedForm.appPath + "WebCalendar/DefaultTemplate/AdvancedTemplate.ashx",
                data: { "folderid": folderID },
                success: function (msg) {
                    var retval = eval("(" + msg + ")");
                    container.prepend(retval.Taxonomy);
                    noTaxonomyLabel.text(retval.NoTaxonomyString);
                    taxRequired.text(retval.TaxonomyRequired);
                    taxonomydata.html('<input type=\'text\' id=\'' + hdnInputid + '\' value=\'' + retval.PreSelectedTaxonomy + '\' />');
                    if (retval.Taxonomy != "" && Ektron.TaxonomyTree !== undefined) {
                        Ektron.TaxonomyTree.init(Ektron.WebCalendar.AdvancedForm.appPath, hdnInputid);
                    }
                    metadata.remove();
                    $ektron("div#Metadata").prepend(retval.Metadata);
                }
            });
        }
    },
    Recurrence: {
        RecurrenceInit: function () {
            $ektron(".recurCheck input").change(function (e) {
                Ektron.WebCalendar.AdvancedForm.Recurrence.RecurCheckChange(this);
            }).click(function (e) {
                Ektron.WebCalendar.AdvancedForm.Recurrence.RecurCheckChange(this);
            });

            var freqGroupName = $ektron(".rsRecurrenceOptionList input[type='radio']").attr("name");
            $ektron("input[name='" + freqGroupName + "']").change(function () {
                Ektron.WebCalendar.AdvancedForm.Recurrence.RecurTypeChange(freqGroupName);
            }).click(function () {
                Ektron.WebCalendar.AdvancedForm.Recurrence.RecurTypeChange(freqGroupName);
            });
        },
        RecurCheckChange: function (el) {
            if (el.checked) {
                $ektron(".rsAdvRecurrencePanel").show();
                if ($ektron(".rsRecurrenceOptionList input[type='radio']:checked").length == 0) {
                    var first = $ektron(".rsRecurrenceOptionList input[type='radio']:first");
                    first[0].checked = true;
                    Ektron.WebCalendar.AdvancedForm.Recurrence.RecurTypeChange(first.attr("name"));
                }
            } else {
                $ektron(".rsAdvRecurrencePanel").hide();
            }
        },
        RecurTypeChange: function (freqGroupName) {
            var selected = $ektron("input[name='" + freqGroupName + "']:checked").val();
            $ektron(".rsAdvDaily").hide();
            $ektron(".rsAdvWeekly").hide();
            $ektron(".rsAdvMonthly").hide();
            $ektron(".rsAdvYearly").hide();
            if (selected == "RepeatFrequencyDaily") {
                $ektron(".rsAdvDaily").show();
            } else if (selected == "RepeatFrequencyWeekly") {
                $ektron(".rsAdvWeekly").show();
            } else if (selected == "RepeatFrequencyMonthly") {
                $ektron(".rsAdvMonthly").show();
            } else if (selected == "RepeatFrequencyYearly") {
                $ektron(".rsAdvYearly").show();
            }
        }
    },
    DatePicking: {
        DateInit: function (containerID) {
            var allDay = $ektron("#" + containerID + " .allday input");
            if (allDay.length == 0) {
                setTimeout('Ektron.WebCalendar.AdvancedForm.DatePicking.DateInit("' + containerID + '")', 50);
                return;
            }
            var dates = $ektron(".rsTimePick input.datetime");
            var times = $ektron(".rsTimePick input.timepick");

            $ektron(".rsTimePick input.startdatesecondary").val($ektron(".rsTimePick input.startdate").val());

            dates.focus(function () {
                $ektron(this).blur();
            }).datepicker({
                culture: Ektron.WebCalendar.AdvancedForm.hiddenValues.UserCulture,
                showOn: 'button',
                buttonText: Ektron.WebCalendar.AdvancedForm.hiddenValues.CalendarButtonAlt,
                buttonImage: Ektron.WebCalendar.AdvancedForm.hiddenValues.CalendarButton,
                buttonImageOnly: true,
                showButtonPanel: false,
                beforeShow: function (input) {
                    $this = $ektron(this);
                    if ($this.hasClass("enddate") || $this.hasClass("rangeenddate")) {
                        //set it to have mindate of startdate
                        $startdate = $ektron(".rsTimePick input.startdate").datepicker('getDate');
                        $this.datepicker('option', 'minDate', $startdate);
                    }
                }
            }).change(function () {
                $this = $ektron(this);
                //update other field with new value
                if ($this.hasClass("startdate")) {
                    $ektron(".rsTimePick input.startdatesecondary").val($this.val());
                }
                if ($this.hasClass("startdatesecondary")) {
                    if ($ektron(".rsTimePick input.startdate").val() == $ektron(".rsTimePick input.enddate").val()) {
                        $ektron(".rsTimePick input.enddate").val($this.val());
                    }
                    $ektron(".rsTimePick input.startdate").val($this.val());
                }
            });

            var timechange = function (input) {
                var $this = null;
                if (typeof (input.jquery) != 'undefined') {
                    $this = input;
                } else {
                    $this = $ektron(this);
                }
                if ($this.hasClass("starttime")) {
                    $end = $this.parent().parent().find("input.endtime");
                    if (!$end.hasClass("selected")) {
                        //set end to one hour after start
                        var time = $this.val().split(":");
                        var endtime = time.slice(0); //create a copy, not a reference
                        endtime[0]++;
                        if (time[0] == 11) {
                            if (time[1].indexOf("AM") > -1) {
                                endtime[1] = time[1].replace("AM", "PM");
                            } else {
                                endtime[1] = time[1].replace("PM", "AM");
                            }
                        }
                        if (time[0] == 11 && time[1].indexOf("PM") > -1) {
                            //increment end day
                            var enddate = $ektron(".rsTimePick input.enddate");
                            var d = $ektron.global.parseDate(enddate.val(), null, Ektron.WebCalendar.AdvancedForm.hiddenValues.UserCulture);
                            d.setDate(d.getDate() + 1);
                            enddate.val($ektron.global.format(d, null, Ektron.WebCalendar.AdvancedForm.hiddenValues.UserCulture));
                        } else {
                            //set to same day as start
                            $ektron(".rsTimePick input.enddate").val($ektron(".rsTimePick input.startdate").val());
                        }
                        if (endtime[0] > 12) {
                            endtime[0] -= 12;
                        }
                        $end.val(endtime[0] + ":" + endtime[1]);
                    }
                } else {
                    if (!$this.hasClass("selected")) {
                        $this.addClass("selected");
                    }
                }
            };

            var timepickerpopup = '<img alt="' + Ektron.WebCalendar.AdvancedForm.hiddenValues.TimePickButtonAlt + '" title="' + Ektron.WebCalendar.AdvancedForm.hiddenValues.TimePickButtonAlt + '" class="ui-timepicker-trigger" src="' + Ektron.WebCalendar.AdvancedForm.hiddenValues.TimePickButton + '"> </img>';
            times.ptTimeSelect({
                onFocusDisplay: false,
                popupImage: timepickerpopup,
                onClose: timechange
            });
            times.bind("keyup", timechange);
            times.bind("keypress", function (event) {
                var k = event.keyCode ? event.keyCode : event.charCode ? event.charCode : event.which;

                if (k == 37 || k == 39) return true; //left, right
                if (k < 48 && k != 32) return true; //special chars excluding space

                if (k >= 48 && k <= 57) return true; //0-9
                if (k == 32 || k == 58 || k == 65 || k == 77 || k == 80) return true; //' ', :, A, M, P
                if (k == 97 || k == 109 || k == 112) return true;
                return false;
            });

            allDay.click(function () {
                Ektron.WebCalendar.AdvancedForm.DatePicking.AllDayCheckChange(this, containerID);
            });
        },
        AllDayCheckChange: function (el, containerID) {
            var lblstartdate = $ektron("#" + containerID + " .lblStartDate");
            var startdate = $ektron("#" + containerID + " .startdate");
            var startdatetrigger = startdate.parent().find(".ui-datepicker-trigger");
            var starttime = $ektron("#" + containerID + " .starttime");
            var starttimetrigger = startdate.parent().find(".ui-timepicker-trigger").parent();

            var lblenddate = $ektron("#" + containerID + " .lblEndDate");
            var enddate = $ektron("#" + containerID + " .enddate");
            var enddatetrigger = enddate.parent().find(".ui-datepicker-trigger");
            var endtime = $ektron("#" + containerID + " .endtime");
            var endtimetrigger = enddate.parent().find(".ui-timepicker-trigger").parent();

            if (el.checked) {
                starttime.hide();
                starttimetrigger.hide();
                lblenddate.hide();
                enddate.hide();
                enddatetrigger.hide();
                endtime.hide();
                endtimetrigger.hide();

                starttime.attr("rel", starttime.val());
                starttime.val(Ektron.WebCalendar.AdvancedForm.hiddenValues.TimeDayStart);
                endtime.attr("rel", endtime.val());
                endtime.val(Ektron.WebCalendar.AdvancedForm.hiddenValues.TimeDayStart);
                var d = enddate.datepicker('getDate');
                d.setDate(d.getDate() + 1);
                enddate.attr("rel", enddate.val());
                $ektron(".rsTimePick input.enddate").val($ektron.global.format(d, null, Ektron.WebCalendar.AdvancedForm.hiddenValues.UserCulture));
            } else {
                if (starttime.attr("rel")) {
                    starttime.val(starttime.attr("rel"));
                } else {
                    starttime.val(Ektron.WebCalendar.AdvancedForm.hiddenValues.Time8AM);
                }
                if (enddate.attr("rel")) {
                    enddate.val(enddate.attr("rel"));
                } else {
                    enddate.val(startdate.val());
                }
                if (endtime.attr("rel")) {
                    endtime.val(endtime.attr("rel"));
                } else {
                    endtime.val(Ektron.WebCalendar.AdvancedForm.hiddenValues.Time9AM);
                }
                starttime.show();
                starttimetrigger.show();
                lblenddate.show();
                enddate.show();
                enddatetrigger.show();
                endtime.show();
                endtimetrigger.show();
            }
        }
    }
};

try {
    if (typeof Sys != 'undefined' && typeof (Sys.WebForms) != 'undefined' && typeof (Sys.WebForms.PageRequestManager) != 'undefined') {
        Ektron.WebCalendar.PageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
        if (Ektron.WebCalendar.PageRequestManager != null) {
            Ektron.WebCalendar.PageRequestManager.add_endRequest(Ektron.WebCalendar.AdvancedForm.destroy);
        }
    }
} catch (ex) { }

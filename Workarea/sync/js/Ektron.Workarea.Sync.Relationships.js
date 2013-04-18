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

// Define Ektron.Workarea.Sync.Relationships object only if it's not already defined
if (typeof (Ektron.Workarea.Sync.Relationships) == "undefined") {
    Ektron.Workarea.Sync.Relationships =
    {
        pollingInterval: null,
        pollingAttempts: 0,

        // Initializes the sync relationships page elements.
        Init: function () {

            $ektron("#SyncStatusModal").modal({
                trigger: "",
                modal: true,
                onHide: function (hash) {
                    Ektron.Workarea.Sync.Relationships.EndStatusPolling();
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function () {
                        if (hash.o) {
                            hash.o.remove();
                        }

                        if (Ektron.Workarea.Sync.Resources.isPostBack.toLowerCase() == "true") {
                            location.replace(Ektron.Workarea.Sync.Resources.rawUrl);
                        }
                        else {
                            location.reload(true);
                        }
                    });
                }
            });

            $ektron("#ResolveSyncCollisionsModal").modal({
                modal: true,
                trigger: ".launchResolveSyncCollisionsButton",
                overlay: 0,
                onShow: function (hash) {

                    Ektron.Workarea.Sync.Relationships.InitResolveSyncCollisionsDialog();

                    hash.o.fadeTo("fast", 0.5, function () {
                        hash.w.fadeIn("fast");
                    });
                },
                onHide: function (hash) {
                    hash.w.fadeOut("fast", function () {

                    });
                    hash.o.fadeOut("fast", function () {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    });

                    // Reload the page so that the toolbar
                    // is appropriately updated.

                    location.reload();
                }
            });

            $ektron("#ShowSyncConfigModal").modal({
                modal: true,
                trigger: "",
                overlay: 0,
                onShow: function (hash) {

                    Ektron.Workarea.Sync.Relationships.InitSyncConfigDialog();

                    hash.o.fadeTo("fast", 0.5, function () {
                        hash.w.fadeIn("fast");
                    });
                },
                onHide: function (hash) {
                    hash.w.fadeOut("fast", function () {

                    });
                    hash.o.fadeOut("fast", function () {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    });

                    // Reload the page so that the toolbar
                    // is appropriately updated.

                    //location.reload();
                }
            });

            $ektron("#CreateSyncRelationshipModal #syncDialogs_btnConnect").click(function () {
                Ektron.Workarea.Sync.Relationships.NextCreateRelationshipStep2Of3();
            });

            $ektron("#CreateSyncRelationshipModal #syncDialogs_btnNextStep2").click(function () {
                Ektron.Workarea.Sync.Relationships.NextCreateRelationshipStep3Of3();
            });

            $ektron("#CreateSyncRelationshipModal #syncDialogs_btnBackStep2").click(function () {
                Ektron.Workarea.Sync.Relationships.ShowCreateRelationshipStep1Of3(false);
            });

            $ektron("#CreateSyncRelationshipModal #syncDialogs_btnBackStep3").click(function () {
                Ektron.Workarea.Sync.Relationships.ShowCreateRelationshipStep2Of3(false);
            });

            $ektron("#CreateSyncRelationshipModal .cancelButton").click(function () {
                Ektron.Workarea.Sync.Relationships.CancelCreateSyncRelationship(false);
            });

            $ektron("#CreateSyncRelationshipModal #txtServerName").keyup(function () {
                Ektron.Workarea.Sync.Relationships.HandleServerNameChange();
            });

            $ektron("#CreateSyncRelationshipModal .syncDirectionButton").click(function () {
                Ektron.Workarea.Sync.Relationships.SwapSyncDirection();
            });

            $ektron("#CreateSyncRelationshipModal .createButton").click(function () {
                Ektron.Workarea.Sync.Relationships.CreateRelationship();
            });

            $ektron("#CreateSyncRelationshipModal #syncDialogs_selectLocalMultiSite").change(function () {
                Ektron.Workarea.Sync.Relationships.HandleLocalMultiSiteSelection();
            });

            $ektron("#CreateSyncRelationshipModal").modal({
                modal: true,
                trigger: ".launchCreateModalButton",
                onShow: function (hash) {
                    hash.o.fadeTo("fast", 0.5, function () {
                        hash.w.fadeIn("fast");
                    });
                    Ektron.Workarea.Sync.Relationships.ShowCreateRelationshipStep1Of3(true);
                },
                onHide: function (hash) {
                    hash.w.fadeOut("fast", function () {

                    });
                    hash.o.fadeOut("fast", function () {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    });

                    $ektron("#CreateSyncRelationshipModal #step1of3 .messages").hide();
                    $ektron("#CreateSyncRelationshipModal #step3of3 .messages").hide();
                }
            });

            $ektron(".ui-widget-header .ektronModalClose").hover(
                    function () {
                        $ektron(this).addClass("ui-state-hover");
                    },
                    function () {
                        $ektron(this).removeClass("ui-state-hover");
                    }
                );

            $ektron("#ResolveSyncCollisionsModal").modal({ modal: true, trigger: "" });
            $ektron("#ConfirmDialog").modal({ modal: true, trigger: "" });
            $ektron("#ShowSyncConfigModal").modal({ trigger: "", modal: true });

            $ektron("#CreateCloudRelationshipModal").modal({
                modal: true,
                trigger: ".launchCreateCloudModalButton",
                onShow: function (hash) {
                    hash.o.fadeTo("fast", 0.5, function () {
                        hash.w.fadeIn("fast");
                    });
                    Ektron.Workarea.Sync.Relationships.ShowCreateCloudRelationshipStep1(true);
                },
                onHide: function (hash) {
                    hash.w.fadeOut("fast", function () {

                    });
                    hash.o.fadeOut("fast", function () {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    });
                    $ektron("#CreateCloudRelationshipModal #cloudStep1 .messages").hide();
                }
            });

            $ektron("#CreateCloudRelationshipModal .btnCloudConnect").click(function () {
                Ektron.Workarea.Sync.Relationships.CreateCloudRelationship();
            });

            $ektron("#CreateCloudRelationshipModal .btnCloudCancel").click(function () {
                Ektron.Workarea.Sync.Relationships.CancelCreateCloudRelationship(false);
            });

        },


        // Prompts the user to confirm the deletion of the selected profile and submits
        // their request if they choose to proceed.
        DeleteProfile: function (button) {
            var deleteButton = $ektron(button);
            var profileId = deleteButton.attr("rel");
            if (typeof (profileId) != "undefined") {
                Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                        Ektron.Workarea.Sync.Resources.deleteProfileDialogTitle,
                        Ektron.Workarea.Sync.Resources.deleteProfileDialogMessage,
                        null,
                        function () { Ektron.Workarea.Sync.Relationships.SendDeleteProfileRequest(profileId); },
                        function () { return false; });
            }
        },

        // Sends a request to the server to delete the specified profile.
        SendDeleteProfileRequest: function (profileId) {
            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=DeleteProfile&id=" + profileId,
                dataType: "json",
                async: false,
                success: Ektron.Workarea.Sync.Relationships.HandleDeleteProfileResponse
            });
        },

        // Handles a response from a delete profile request.
        HandleDeleteProfileResponse: function (data) {
            window.location.reload(true);
        },

        // Prompts the user to confirm the deletion of the selected relationship and submits
        // their request if they choose to proceed.
        DeleteRelationship: function (button) {
            var deleteButton = $ektron(button);
            var id = deleteButton.attr("rel");
            if (typeof (id) != "undefined") {
                Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                        Ektron.Workarea.Sync.Resources.deleteRelationshipDialogTitle,
                        Ektron.Workarea.Sync.Resources.deleteRelationshipDialogMessage,
                        null,
                        function () { Ektron.Workarea.Sync.Relationships.SendDeleteRelationshipRequest(id); },
                        function () { return false; });
            }
        },

        // Sends a request to the server to delete the specified relationship.
        SendDeleteRelationshipRequest: function (relationshipId) {
            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=DeleteRelationship&id=" + relationshipId,
                dataType: "json",
                async: false,
                success: Ektron.Workarea.Sync.Relationships.HandleDeleteRelationshipResponse
            });
        },

        // Handles a response from a delete relationship request.
        HandleDeleteRelationshipResponse: function (data) {
            window.location.reload(true);
        },

        // Toggles that status (pause/resume) of selected profile.
        ToggleScheduleStatus: function (button) {
            var statusButton = $ektron(button);
            var id = statusButton.attr("rel");

            if (statusButton.hasClass("resume")) {
                Ektron.Workarea.Sync.Relationships.SendResumeScheduleRequest(id);
            }
            else {
                Ektron.Workarea.Sync.Relationships.SendPauseScheduleRequest(id);
            }
        },

        // Sets a request to the server to pause the specified profile.
        SendPauseScheduleRequest: function (profileId) {
            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=Pause&id=" + profileId,
                dataType: "json",
                async: false,
                success: Ektron.Workarea.Sync.Relationships.HandlePauseScheduleResponse
            });
        },

        // Handles a response from a pause schedule request.
        HandlePauseScheduleResponse: function (data) {
            if (data != null && data.Success) {
                var statusButton = $ektron(".syncButtons a[rel='" + data.Id + "'].pause");
                if (statusButton.length == 1) {
                    statusButton.removeClass("pause");
                    statusButton.addClass("resume");

                    var nextRunTimeBlock = statusButton.parent().parent().parent().find("p.nextRunTime");
                    nextRunTimeBlock.text(Ektron.Workarea.Sync.Resources.nextSyncTimeLabel);
                    if (data.NextRunTime == Ektron.Workarea.Sync.Resources.dateTimeMaxValue) {
                        nextRunTimeBlock.text(nextRunTimeBlock.text() + Ektron.Workarea.Sync.Resources.nextSyncTimeNoneLabel);
                    }
                    else {
                        nextRunTimeBlock.text(nextRunTimeBlock.text() + data.NextRunTime);
                    }
                }
            }
        },

        // Sets a request to the server to resume the specified profile.
        SendResumeScheduleRequest: function (profileId) {
            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=Resume&id=" + profileId,
                dataType: "json",
                async: false,
                success: Ektron.Workarea.Sync.Relationships.HandleResumeScheduleResponse
            });
        },

        // Handles a response from a resume schedule request.
        HandleResumeScheduleResponse: function (data) {
            if (data != null && data.Success) {
                var statusButton = $ektron(".syncButtons a[rel='" + data.Id + "'].resume");
                if (statusButton.length == 1) {
                    statusButton.removeClass("resume");
                    statusButton.addClass("pause");

                    var nextRunTimeBlock = statusButton.parent().parent().parent().find("p.nextRunTime");
                    nextRunTimeBlock.text(Ektron.Workarea.Sync.Resources.nextSyncTimeLabel);

                    if (data.NextRunTime == Ektron.Workarea.Sync.Resources.dateTimeMaxValue) {
                        nextRunTimeBlock.text(nextRunTimeBlock.text() + Ektron.Workarea.Sync.Resources.nextSyncTimeNoneLabel);
                    }
                    else {
                        nextRunTimeBlock.text(nextRunTimeBlock.text() + data.NextRunTime);
                    }
                }
            }
        },

        ShowSyncStatus: function (button) {
            var statusButton = $ektron(button);
            var profileId = statusButton.attr("rel");

            Ektron.Workarea.Sync.Relationships.DisplayStatusDialog(profileId);
        },

        DisplayStatusDialog: function (profileId) {
            Ektron.Workarea.Sync.Relationships.pollingAttempts = 0;

            var statusDialog = $ektron("#SyncStatusModal");
            var profileIdPlaceHolder = statusDialog.find(".statusHeaderProfileId");
            profileIdPlaceHolder.text(profileId);
            var closeButton = $ektron("#SyncStatusModal .button");

            // Bind the status dialog's close button.

            closeButton.unbind("click");
            closeButton.click(function () { statusDialog.modalHide(); });

            // Display the status dialog.

            statusDialog.modalShow();

            // Start polling for status updates. Polling will end when the
            // dialog is closed or when a status entry is received indicating
            // that the activity is complete.

            Ektron.Workarea.Sync.Relationships.StartStatusPolling(profileId);

            Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                    $ektron("#SyncStatusModal p.messages"),
                    Ektron.Workarea.Sync.Resources.retrievingStatusMessage,
                    "inProgress");
        },

        StartStatusPolling: function (profileId) {
            Ektron.Workarea.Sync.Relationships.EndStatusPolling();
            Ektron.Workarea.Sync.Relationships.pollingInterval = setInterval(
                    function () { Ektron.Workarea.Sync.Relationships.SendGetStatusRequest(profileId); },
                    5000);
        },

        EndStatusPolling: function () {
            if (Ektron.Workarea.Sync.Relationships.pollingInterval != null) {
                clearInterval(Ektron.Workarea.Sync.Relationships.pollingInterval);
                Ektron.Workarea.Sync.Relationships.pollingInterval = null;
            }
        },

        SendGetStatusRequest: function (profileId) {
            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncStatusHandlerPath + "?action=Status&id=" + profileId,
                dataType: "json",
                async: true,
                success: Ektron.Workarea.Sync.Relationships.HandleGetStatusResponse,
                error: Ektron.Workarea.Sync.Relationships.HandleGetStatusError
            });
        },

        HandleGetStatusResponse: function (data) {
            if (data != null) {

                var messageBlock = $ektron("#SyncStatusModal .syncStatusMessages");

                if (messageBlock.length > 0) {
                    messageBlock.empty();

                    // Populate the status dialog's message block with the
                    // data received in the response.

                    if (data.Entries != null && data.Entries.length > 0) {

                        // We've received a status response with entries in
                        // it so reset the polling counter.

                        Ektron.Workarea.Sync.Relationships.pollingAttempts = 0;

                        Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                            $ektron("#SyncStatusModal p.messages"),
                            Ektron.Workarea.Sync.Resources.syncInProgressMessage,
                            "inProgress");

                        var isLoadBalanceMessage = false;
                        for (var i = 0; i < data.Entries.length; i++) {

                            // If the status entry does not represent a "SyncEnded"
                            // message, build an entry element to be displayed. The
                            // "SyncEnded" data is displayed seperately.

                            if (data.Entries[i].Code != "SyncEnded") {
                                var entryWrapper = $ektron("<div>");
                                var spanTime = $ektron("<span>");
                                var spanMessage = $ektron("<span>");

                                spanTime.text(data.Entries[i].DateCreated);
                                spanTime.addClass("statusTime");
                                entryWrapper.append(spanTime);

                                if (data.Entries[i].Code == "SyncCheckingLoadBalanceEnd") {
                                    isLoadBalanceMessage = false;
                                }

                                if (!isLoadBalanceMessage) {
                                    spanMessage.text(data.Entries[i].Message);
                                    entryWrapper.append(spanMessage);

                                    if (data.Entries[i].Details != null && data.Entries[i].Details != "") {
                                        var detailsMessage = $ektron("<div>");
                                        detailsMessage.text(data.Entries[i].Details);

                                        // If the entry's code indicates "SyncFailed", display
                                        // its details with the "syncError" class. Otherwise,
                                        // use the standard style.

                                        if (data.Entries[i].Code == "SyncFailed") {
                                            detailsMessage.addClass("syncError");
                                        }
                                        else {
                                            detailsMessage.addClass("stepProgress");
                                        }

                                        entryWrapper.append(detailsMessage);
                                    }

                                    if (data.Entries[i].Statistics != null && data.Entries[i].Statistics != "") {
                                        var statisticsMessageWrapper = $ektron("<div>");
                                        statisticsMessageWrapper.addClass("syncStatistics");

                                        var statisticsMesageHeader = $ektron("<h5>");
                                        statisticsMesageHeader.text("Synchronization Statistics");

                                        var statisticsMessage = $ektron("<p>");
                                        statisticsMessage.html(data.Entries[i].Statistics);

                                        statisticsMessageWrapper.append(statisticsMesageHeader);
                                        statisticsMessageWrapper.append(statisticsMessage);

                                        entryWrapper.append(statisticsMessageWrapper);
                                    }

                                    if (data.Entries[i].Code == "SyncPerformingLoadBalance") {
                                        isLoadBalanceMessage = true;
                                    }
                                }
                                else {
                                    var message = $ektron("<span>");
                                    message.text(data.Entries[i].Message);
                                    message.addClass("stepProgress");

                                    entryWrapper.append(message);
                                }

                                messageBlock.append(entryWrapper);
                            }
                        }
                    }
                    else {
                        Ektron.Workarea.Sync.Relationships.CheckPollingAttempts();
                    }

                    // If activity is complete, stop any current polling.

                    if (data.IsComplete) {
                        Ektron.Workarea.Sync.Relationships.EndStatusPolling();

                        var lastEntryMessage = $ektron("<div>");

                        if (data.Success) {

                            lastEntryMessage.text(Ektron.Workarea.Sync.Resources.syncCompleteMessage);
                            messageBlock.append(lastEntryMessage);

                            Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                                $ektron("#SyncStatusModal p.messages"),
                                Ektron.Workarea.Sync.Resources.syncCompleteMessage,
                                "success");

                            lastEntryMessage.addClass("syncSuccess");
                        }
                        else {

                            lastEntryMessage.text(Ektron.Workarea.Sync.Resources.syncCanceledMessage);
                            messageBlock.append(lastEntryMessage);

                            Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                                $ektron("#SyncStatusModal p.messages"),
                                Ektron.Workarea.Sync.Resources.syncErrorMessage,
                                "error");

                            lastEntryMessage.addClass("syncError");
                        }
                    }
                }

                messageBlock.append("<div class=\"scrollTo\"></div>");
                messageBlock.scrollTo($ektron("#SyncStatusModal .scrollTo"));
            }
        },

        HandleGetStatusError: function (data) {
            Ektron.Workarea.Sync.Relationships.CheckPollingAttempts();
        },

        CheckPollingAttempts: function () {
            if (Ektron.Workarea.Sync.Relationships.pollingAttempts < 2) {
                Ektron.Workarea.Sync.Relationships.pollingAttempts++;
            }
            else {
                Ektron.Workarea.Sync.Relationships.EndStatusPolling();
                Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                    $ektron("#SyncStatusModal p.messages"),
                    Ektron.Workarea.Sync.Resources.noSyncStatusAvailable,
                    "success");

                $ektron("#SyncStatusModal div.syncStatusMessages").hide();
            }
        },

        IsSyncInProgress: function () {
            var response = null;

            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=IsSyncInProgress",
                dataType: "json",
                async: false,
                success: function (data) {
                    response = data;
                }
            });

            return response;
        },

        InitialSynchronize: function (button) {
            Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                Ektron.Workarea.Sync.Resources.syncProfileDialogTitle,
                Ektron.Workarea.Sync.Resources.initialSyncDialogMessage,
                Ektron.Workarea.Sync.Resources.initialSyncDialogCaption,
                function () {
                    Ektron.Workarea.Sync.Relationships.Synchronize(button, false);
                },
                function () { return false; });
        },

        Synchronize: function (button, confirm) {
            var statusButton = $ektron(button);
            var profileId = statusButton.attr("rel");

            if (confirm == null || confirm) {
                Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                    Ektron.Workarea.Sync.Resources.syncProfileDialogTitle,
                    Ektron.Workarea.Sync.Resources.syncProfileDialogMessage,
                    null,
                    function () {
                        Ektron.Workarea.Sync.Relationships.LaunchSync(button, profileId);
                    },
                    function () { return false; });
            }
            else {
                Ektron.Workarea.Sync.Relationships.LaunchSync(button, profileId);
            }
        },

        LaunchSync: function (button, profileId) {
            var response = Ektron.Workarea.Sync.Relationships.IsSyncInProgress();
            if (response != null && response.Success && !response.IsSyncInProgress) {
                Ektron.Workarea.Sync.Relationships.SendSynchronizeRequest(profileId);
                Ektron.Workarea.Sync.Relationships.ShowSyncStatus(button);
            }
            else {
                Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                    Ektron.Workarea.Sync.Relationships.syncInProgressDialogTitle,
                    response.Messages[0],
                    null,
                    function () {
                        Ektron.Workarea.Sync.Relationships.DisplayStatusDialog(response.ProfileId);
                    });
            }
        },

        SendSynchronizeRequest: function (profileId) {
            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=Sync&id=" + profileId,
                dataType: "json",
                async: true,
                success: Ektron.Workarea.Sync.Relationships.HandleSynchronizeResponse
            });
        },

        HandleSynchronizeResponse: function (data) {

        },

        ShowCreateRelationshipStep1Of3: function (doModalShow) {
            if (!doModalShow) {
                $ektron("#CreateSyncRelationshipModal").fadeOut(200);
            }

            Ektron.Workarea.Sync.Relationships.LoadCertificates();

            $ektron("#step1of3").show();
            $ektron("#step2of3").hide();
            $ektron("#step3of3").hide();

            if (doModalShow) {
                $ektron("#CreateSyncRelationshipModal").modalShow();
            }
            else {
                $ektron("#CreateSyncRelationshipModal").fadeIn("fast");
            }
        },

        NextCreateRelationshipStep2Of3: function () {
            var serverNameInput = $ektron("#CreateSyncRelationshipModal #step1of3 #txtServerName");
            var certificateInput = $ektron("#CreateSyncRelationshipModal #step1of3 #selectCertificate");

            if (typeof (serverNameInput.attr("value")) != "undefined" && serverNameInput.attr("value").length > 0) {
                if (typeof (certificateInput.val()) != "undefined" && certificateInput.val() != "Please select" && certificateInput.val() != "") {

                    var connectButton = $ektron("#CreateSyncRelationshipModal #step1of3 .connectButton");
                    connectButton.removeClass("connectButton");
                    connectButton.addClass("connectingButton");

                    // Get remote sites
                    $ektron.ajax({
                        url: Ektron.Workarea.Sync.Resources.syncHandlerPath + '?action=Sites&local=false&server=' + serverNameInput.attr("value") + "&certificate=" + certificateInput.val(),
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            if (data.Success && data.Sites.length > 0) {
                                Ektron.Workarea.Sync.Relationships.ShowCreateRelationshipStep2Of3(data);
                            }
                            else {
                                var message = "<p>";
                                message += Ektron.Workarea.Sync.Resources.noCMSSitesFoundMessage;
                                message += " ";
                                message += Ektron.Workarea.Sync.Resources.serviceErrorMessage;
                                message += "</p>";
                                message += "<ul>";
                                message += "<li>";
                                message += Ektron.Workarea.Sync.Resources.serviceErrorCauseAMessage;
                                message += "</li>";
                                message += "<li>";
                                message += Ektron.Workarea.Sync.Resources.serviceErrorCauseBMessage;
                                message += "</li>";
                                message += "<li>";
                                message += Ektron.Workarea.Sync.Resources.serviceErrorCauseCMessage;
                                message += "</li>";
                                message += "<li>";
                                message += Ektron.Workarea.Sync.Resources.serviceErrorCauseDMessage;
                                message += "</li>";
                                message += "<li>";
                                message += Ektron.Workarea.Sync.Resources.serviceErrorCauseEMessage;
                                message += "</li>";
                                message += "</ul>";

                                Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                                    $ektron("#CreateSyncRelationshipModal #step1of3 div.messages"),
                                    message,
                                    "error");
                            }
                        },
                        error: function () { alert("Request failed."); }
                    });

                    connectButton.removeClass("connectingButton");
                    connectButton.addClass("connectButton");
                }
                else {
                    Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                        $ektron("#CreateSyncRelationshipModal #step1of3 div.messages"),
                        Ektron.Workarea.Sync.Resources.enterCertificateMessage,
                        "error");
                }
            }
            else {
                Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                    $ektron("#CreateSyncRelationshipModal #step1of3 div.messages"),
                    Ektron.Workarea.Sync.Resources.enterServerNameMessage,
                    "error");
            }
        },

        CancelCreateSyncRelationship: function (data) {
            $ektron('#CreateSyncRelationshipModal').modalHide();
        },

        ShowCreateRelationshipStep2Of3: function (data) {
            $ektron("#CreateSyncRelationshipModal").fadeOut(200);
            $ektron("#CreateSyncRelationshipModal #step1of3 div.messages").hide();

            if (data != null) {

                var remoteServerName = $ektron("#CreateSyncRelationshipModal #step1of3 #txtServerName").attr("value");
                var portNumber = "8732";

                $ektron("#CreateSyncRelationshipModal #step2of3 #RemoteServer").text(remoteServerName);
                $ektron("#CreateSyncRelationshipModal #step2of3 #PortNumber").text(portNumber);

                if (data.Sites != null) {
                    var remoteSitesList = $ektron("#CreateSyncRelationshipModal #step2of3 #remoteSites");
                    remoteSitesList.empty();

                    for (var i = 0; i < data.Sites.length; i++) {

                        // Create the parent item to encapsulate data for
                        // an individual CMS site.

                        var siteItem = $ektron("<li>");
                        var siteItemHeader = $ektron("<h5>");
                        var siteItemInfo = $ektron("<ul>");
                        var serverNameItem = $ektron("<li>");
                        var serverNameLabel = $ektron("<span>");
                        var serverName = $ektron("<span>");
                        var securityItem = $ektron("<li>");
                        var securityLabel = $ektron("<span>");
                        var security = $ektron("<span>");

                        siteItem.addClass("RemoteMultiSitePath");

                        // Event Bindings

                        siteItem.mouseover(
                                function () {
                                    $ektron(this).addClass("hover");
                                }
                            );

                        siteItem.mouseout(
                                function () {
                                    $ektron(this).removeClass("hover");
                                }
                            );

                        siteItem.click(
                                function () {
                                    $ektron("#CreateSyncRelationshipModal #step2of3 #remoteSites li").removeClass("selected");
                                    $ektron(this).addClass("selected");
                                }
                            );

                        // CMS Site Header

                        siteItemHeader.text(data.Sites[i].DatabaseName);

                        // Site Details

                        siteItemInfo.addClass("cmsInfo");
                        siteItemInfo.addClass("clearFix");

                        // Server Name Information

                        serverNameLabel.addClass("serverInfoLabel");
                        serverNameLabel.text("Server Name:");
                        serverName.text(data.Sites[i].DatabaseServerName);
                        serverName.addClass("serverNameField");

                        // Build the CMS site list:
                        //  <ul>
                        //      <li>
                        //          <h5></h5>
                        //          <ul>
                        //              <li>Server Name: [Name]</li>
                        //          </ul>
                        //      </li>
                        //      ...
                        //  </ul>

                        serverNameItem.append(serverNameLabel);
                        serverNameItem.append(serverName);
                        siteItemInfo.append(serverNameItem);
                        siteItemInfo.append(securityItem);
                        siteItem.append(siteItemHeader);
                        siteItem.append(siteItemInfo);
                        remoteSitesList.append(siteItem);
                    }

                    // Select the first site option in the list.
                    $ektron("#CreateSyncRelationshipModal #step2of3 #remoteSites li:first").addClass("selected");
                }
            }

            $ektron("#step1of3").hide();
            $ektron("#step2of3").show();
            $ektron("#step3of3").hide();
            $ektron("#CreateSyncRelationshipModal").fadeIn("fast");
        },

        NextCreateRelationshipStep3Of3: function () {
            var serverNameInput = $ektron("#CreateSyncRelationshipModal #step1of3 #txtServerName");
            var certificateInput = $ektron("#CreateSyncRelationshipModal #step1of3 #selectCertificate option:selected");
            var localServerName = $ektron("#CreateSyncRelationshipModal #step3of3 #syncDialogs_localServer");

            if (typeof (serverNameInput.attr("value")) != "undefined" && serverNameInput.attr("value").length > 0) {
                if (typeof (certificateInput.attr("value")) != "undefined" && certificateInput.attr("value").length > 0) {

                    var localSiteData = null;
                    var remoteSiteData = null;

                    // Get local sites.

                    $ektron.ajax({
                        url: Ektron.Workarea.Sync.Resources.syncHandlerPath + '?action=Sites&local=true&server=' + localServerName.attr("value") + "&certificate=" + localServerName.attr("value"),
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            if (data.Success) {
                                localSiteData = data;
                            }
                        }
                    });

                    // Get remote sites.

                    $ektron.ajax({
                        url: Ektron.Workarea.Sync.Resources.syncHandlerPath + '?action=Sites&local=false&server=' + serverNameInput.attr("value") + "&certificate=" + certificateInput.attr("value"),
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            if (data.Success) {
                                remoteSiteData = data;
                            }
                        }
                    });

                    // If site data was retrieved successfully for both the
                    // local and remote sites, display the next step in the dialog.

                    if (localSiteData != null && remoteSiteData != null) {
                        Ektron.Workarea.Sync.Relationships.ShowCreateRelationshipStep3Of3(
                            localSiteData,
                            remoteSiteData);
                    }
                }
            }
        },

        ShowCreateRelationshipStep3Of3: function (localSiteData, remoteSiteData) {
            $ektron("#CreateSyncRelationshipModal").fadeOut(200);

            // Retrieve the selected local and remote database names.
            var localDatabaseName = $ektron("#directionLocalServer input.localDatabaseName").attr("value");
            var localDatabaseServer = $ektron("#directionLocalServer input.localDatabaseServer").attr("value");
            var remoteDatabaseName = $ektron("#step2of3 #remoteSites .selected h5").text();
            var remoteDatabaseServer = $ektron("#step2of3 #remoteSites .selected .serverNameField").text();

            // If there are no multi-site folders, hide the multisite selection options.
            var localMultiSiteListOptions = $ektron("#syncDialogs_selectLocalMultiSite option");
            if (localMultiSiteListOptions.length < 2) {
                $ektron("ul#directionLocalServer div.selectLocalMultiSite").hide();
            }
            else {
                $ektron("ul#directionLocalServer div.selectLocalMultiSite").show();
            }

            // For each site listed in the specified data structure,
            // find the ones matching our select local and remote
            // databases and display their details.
            for (var i = 0; i < localSiteData.Sites.length; i++) {

                if (localSiteData.Sites[i].DatabaseName.toLowerCase() == localDatabaseName.toLowerCase() &&
                            localSiteData.Sites[i].DatabaseServerName.toLowerCase() == localDatabaseServer.toLowerCase()) {

                    // Load the site paths into the drop down list.
                    var ddlLocalSitePaths = $ektron("#directionLocalServer #selectLocalMultiSitePath");
                    ddlLocalSitePaths.empty();
                    for (var j = 0; j < localSiteData.Sites[i].SitePaths.length; j++) {
                        var sitePathOption = $ektron("<option>");
                        sitePathOption.attr("value", localSiteData.Sites[i].SitePaths[j]);
                        sitePathOption.text(localSiteData.Sites[i].SitePaths[j]);

                        ddlLocalSitePaths.append(sitePathOption);
                    }

                    // If there is only one site path, disable the input option.
                    if (ddlLocalSitePaths.children().length == 1) {
                        ddlLocalSitePaths.attr("disabled", "disabled");
                    }
                    else {
                        ddlLocalSitePaths.removeAttr("disabled");
                    }
                }
            }

            for (var i = 0; i < remoteSiteData.Sites.length; i++) {

                if (remoteSiteData.Sites[i].DatabaseName.toLowerCase() == remoteDatabaseName.toLowerCase() &&
                            remoteSiteData.Sites[i].DatabaseServerName.toLowerCase() == remoteDatabaseServer.toLowerCase()) {

                    // Retrieve and populate the DOM elements intended to wrap the
                    // various site details.
                    var databaseNameHeader = $ektron("#directionRemoteServer  h5 #remoteDatabaseName");
                    var lblServerName = $ektron("#directionRemoteServer #lblRemoteSiteServerName");
                    var lblIntegratedSecurity = $ektron("#directionRemoteServer #lblRemoteSiteIntegratedSecurity");

                    databaseNameHeader.text(remoteSiteData.Sites[i].DatabaseName);
                    lblServerName.text(remoteSiteData.Sites[i].DatabaseServerName);
                    lblIntegratedSecurity.text(remoteSiteData.Sites[i].IntegratedSecurity);

                    // Load the site paths into the drop down list.
                    var ddlRemoteSitePaths = $ektron("#directionRemoteServer #selectRemoteMultiSite");
                    ddlRemoteSitePaths.empty();
                    for (var j = 0; j < remoteSiteData.Sites[i].SitePaths.length; j++) {
                        var sitePathOption = $ektron("<option>");
                        sitePathOption.attr("value", remoteSiteData.Sites[i].SitePaths[j]);
                        sitePathOption.text(remoteSiteData.Sites[i].SitePaths[j]);

                        ddlRemoteSitePaths.append(sitePathOption);
                    }

                    // If there is only one site path, hide the input options.
                    if (ddlRemoteSitePaths.children().length < 2) {
                        $ektron("ul#directionRemoteServer div.selectRemoteMultiSite").hide();
                    }
                    else {
                        $ektron("ul#directionRemoteServer div.selectRemoteMultiSite").show();
                    }
                }
            }

            Ektron.Workarea.Sync.Relationships.HandleLocalMultiSiteSelection();

            $ektron("#step1of3").hide();
            $ektron("#step2of3").hide();
            $ektron("#step3of3").show();
            $ektron("#CreateSyncRelationshipModal").fadeIn("fast");
        },

        CreateRelationship: function () {
            var localDatabaseName = $ektron("#directionLocalServer input.localDatabaseName").attr("value");
            var localServerName = $ektron("#directionLocalServer input.localDatabaseServer").attr("value");
            var localMultiSite = $ektron("#step3of3 #directionLocalServer .multiSiteDropDown").attr("value");
            var localSitePath = $ektron("#step3of3 #selectLocalMultiSitePath").attr("value");
            var remoteDatabaseName = $ektron("#step3of3 #remoteDatabaseName").text();
            var remoteServerName = $ektron("#CreateSyncRelationshipModal #step1of3 #txtServerName").attr("value");
            var remoteDatabaseServer = $ektron("#step2of3 #remoteSites .selected .serverNameField").text();
            var remoteSitePath = $ektron("#step3of3 #selectRemoteMultiSite").attr("value");
            var certificate = $ektron("#step1of3 #selectCertificate option:selected").attr("value");
            var direction = "upload";
            if ($ektron("#replacingDatabaseServer #directionLocalServer").length > 0) {
                direction = "download";
            }

            $ektron.ajax({
                async: true,
                dataType: "json",
                type: "POST",
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=CreateRelationship" +
                    "&localDatabase=" + localDatabaseName +
                    "&localServer=" + localServerName +
                    "&localMultiSite=" + localMultiSite +
                    "&localSitePath=" + localSitePath +
                    "&remoteDatabase=" + remoteDatabaseName +
                    "&remoteDatabaseServer=" + remoteDatabaseServer +
                    "&remoteServer=" + remoteServerName +
                    "&remoteSitePath=" + remoteSitePath +
                    "&certificate=" + certificate +
                    "&direction=" + direction,
                cache: false,
                success: function (data) { Ektron.Workarea.Sync.Relationships.CompleteCreateRelationship(data); }
            });
        },

        CompleteCreateRelationship: function (data) {
            if (data.Success) {
                if (data.Resurrected) {
                    $ektron("#CreateSyncRelationshipModal").modalHide();

                    Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                        Ektron.Workarea.Sync.Resources.relationshipReactivatedDialogTitle,
                        Ektron.Workarea.Sync.Resources.relationshipReactivatedDialogMessage.replace("{0}", data.RelationshipId),
                        null,
                        function () { document.location.reload(true); },
                        null,
                        false);
                }
                else {
                    document.location.reload(true);
                }
            }
            else {
                if (data.Messages.length > 0) {
                    var messageContainer = $ektron("#CreateSyncRelationshipModal #step3of3 .messages");
                    messageContainer.empty();
                    messageContainer.html("<span class='error'>" + data.Messages[0] + "</span>").fadeIn("slow");
                }
                else {
                    $ektron("#CreateSyncRelationshipModal #step3of3 .messages").hide();
                }
            }
        },

        LoadCertificates: function () {
            var certificatesList = $ektron("#CreateSyncRelationshipModal #selectCertificate");
            certificatesList.empty();
            certificatesList.append($ektron("<option>").text("Please wait..."));

            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=Certificates",
                dataType: "json",
                async: true,
                error: function () { alert('error'); },
                success: Ektron.Workarea.Sync.Relationships.HandleCertificatesRequest
            });
        },

        HandleCertificatesRequest: function (data) {
            var certificatesList = $ektron("#CreateSyncRelationshipModal #selectCertificate");
            certificatesList.empty();

            if (data != null &&
                    data.Success &&
                    data.Certificates != null &&
                    data.Certificates.length > 0) {

                $ektron("#CreateSyncRelationshipModal #step1of3 div.messages").hide();

                certificatesList.append($ektron("<option>").text("Please select"));

                for (var i = 0; i < data.Certificates.length; i++) {
                    var certificate = $ektron("<option>");
                    certificate.attr("value", data.Certificates[i]);
                    certificate.text(data.Certificates[i]);
                    certificatesList.append(certificate);
                }
            }
            else {
                // Display connection error

                var message = Ektron.Workarea.Sync.Resources.noCertificatesFoundMessage;

                Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                    $ektron("#CreateSyncRelationshipModal #step1of3 div.messages"),
                    message,
                    "error");

                var noCertificatesOption = $ektron("<option>").text("No certificates found");
                noCertificatesOption.attr("value", "");
                certificatesList.append(noCertificatesOption);
            }
        },

        HandleServerNameChange: function () {
            var serverNameInput = $ektron("#CreateSyncRelationshipModal #step1of3 #txtServerName");
            var matchingOptions = $ektron("#CreateSyncRelationshipModal #step1of3 #selectCertificate option");

            var matchingElement = null;
            for (var i = 0; i < matchingOptions.length && matchingElement == null; i++) {
                var optionElement = $ektron(matchingOptions[i]);
                if (optionElement.attr("value").toLowerCase() == serverNameInput.attr("value").toLowerCase()) {
                    matchingElement = optionElement;
                }
            }

            if (matchingElement != null) {
                for (var i = 0; i < matchingOptions.length; i++) {
                    $ektron(matchingOptions[i]).removeAttr("selected");
                }

                matchingElement.attr("selected", "selected");
            }
        },

        SwapSyncDirection: function () {
            var remoteServer = $ektron("#directionRemoteServer");
            var remoteServerParent = remoteServer.parent();
            var localServer = $ektron("#directionLocalServer");
            var localServerParent = localServer.parent();

            remoteServer.remove();
            localServer.remove();

            localServerParent.append(remoteServer);
            remoteServerParent.append(localServer);

            $ektron("#CreateSyncRelationshipModal #syncDialogs_selectLocalMultiSite").unbind("change");
            $ektron("#CreateSyncRelationshipModal #syncDialogs_selectLocalMultiSite").change(function () {
                Ektron.Workarea.Sync.Relationships.HandleLocalMultiSiteSelection();
            });
        },


        ShowCreateCloudRelationshipStep1: function (doModalShow) {
            if (!doModalShow) {
                $ektron("#CreateCloudRelationshipModal").fadeOut(200);
            }

            window.setTimeout(
                function () {
                    $ektron("#cloudStep1").show();

                    if (doModalShow) {
                        $ektron("#CreateCloudRelationshipModal").modalShow();
                    }
                    else {
                        $ektron("#CreateCloudRelationshipModal").fadeIn("fast");
                    }
                },
                200);
        },

        CreateCloudRelationship: function () {
            var localDatabaseName = $ektron(".createCloud input.localDatabaseNameCloud").attr("value");
            var localServerName = $ektron(".createCloud input.localDatabaseServerCloud").attr("value");
            var localSitePath = "";
            var sqlConnectionString = $ektron(".createCloud input#txtSQLServer").attr("value");
            var localIPAddress = $ektron(".createCloud input.serverIPAddress").attr("value");
            var blobStorage = $ektron(".createCloud input#txtBlobStorage").attr("value");
            var accountName = $ektron(".createCloud input#txtAccountName").attr("value");
            var containerName = $ektron(".createCloud input#txtContainerName").attr("value");
            var accountKey = $ektron(".createCloud input#txtAccountKey").attr("value");
            var cloudDomain = $ektron(".createCloud input#txtCloudDomain").attr("value");
            var direction = "upload";

            $ektron.ajax({
                async: true,
                dataType: "json",
                type: "POST",
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=CreateCloudRelationship" +
                    "&localDatabase=" + localDatabaseName +
                    "&localServer=" + localServerName +
                    "&sqlConnectionString=" + sqlConnectionString +
                    "&localIPAddress=" + localIPAddress +
                    "&blobStorage=" + blobStorage +
                    "&accountName=" + accountName +
                    "&containerName=" + containerName +
                    "&accountKey=" + accountKey +
                    "&cloudDomain=" + cloudDomain +
                    "&direction=" + direction,
                cache: false,
                success: function (data) { Ektron.Workarea.Sync.Relationships.CompleteCreateCloudRelationship(data); }
            });
        },

        CompleteCreateCloudRelationship: function (data) {
            if (data.Success) {
                if (data.Resurrected) {
                    $ektron("#CreateCloudRelationshipModal").modalHide();

                    Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                        Ektron.Workarea.Sync.Resources.relationshipReactivatedDialogTitle,
                        Ektron.Workarea.Sync.Resources.relationshipReactivatedDialogMessage.replace("{0}", data.RelationshipId),
                        null,
                        function () { document.location.reload(true); },
                        null,
                        false);
                }
                else {
                    document.location.reload(true);
                }
            }
            else {
                if (data.Messages.length > 0) {
                    var messageContainer = $ektron("#CreateCloudRelationshipModal #cloudStep1 .messages");
                    messageContainer.empty();
                    messageContainer.html("<span class='error'>" + data.Messages[0] + "</span>").fadeIn("slow");
                }
                else {
                    $ektron("#CreateCloudRelationshipModal #cloudStep1 .messages").hide();
                }
            }
        },

        CancelCreateCloudRelationship: function (data) {
            $ektron('#CreateCloudRelationshipModal').modalHide();
        },

        UpdateDialogMessage: function (messageBlock, message, cssClass) {
            messageBlock.empty();
            messageBlock.show();

            var headerMessage = $ektron("<span>");
            headerMessage.addClass(cssClass);
            headerMessage.html(message);

            messageBlock.append(headerMessage).fadeIn("fast");
        },

        // Displays a confirmation dialog with the specified title and
        // message. If specified, the 'OK' and 'Cancel' callbacks will
        // be executed when their respective button is pressed.
        ShowConfirmDialog: function (title, message, caption, okCallback, cancelCallback, showCancelButton) {
            var confirmDialog = $ektron("#ConfirmDialog");
            var okButton = $ektron("#ConfirmDialog .okButton");
            var cancelButton = $ektron("#ConfirmDialog .cancelButton");
            var messageBlock = $ektron("#ConfirmDialog p.messages");
            var headerBlock = $ektron("#ConfirmDialog span.headerText");
            var captionBlock = $ektron("#ConfirmDialog p.messagesCaption");

            headerBlock.html(title);
            messageBlock.html(message);

            if (caption != null && caption.length > 0) {
                captionBlock.html(caption);
                captionBlock.show();
            }
            else {
                captionBlock.hide();
            }

            okButton.unbind();
            okButton.click(function () {
                confirmDialog.modalHide();
                if (typeof (okCallback) == "function") {
                    okCallback();
                }
            });

            cancelButton.unbind();
            cancelButton.click(function () {
                confirmDialog.modalHide();
                if (typeof (cancelCallback) == "function") {
                    cancelCallback();
                }
            });

            if (showCancelButton == null ||
                showCancelButton == true ||
                cancelCallback != null) {
                cancelButton.show();
            }
            else {
                cancelButton.hide();
            }

            messageBlock.show();
            confirmDialog.css("margin-top", -1 * Math.round(confirmDialog.outerHeight() / 2)).css("top", "50%");
            confirmDialog.modalShow();
        },

        // Initializes the 'Resolve Sync Collisions' modal dialog, enabling its buttons
        // registering click handlers, and clearing message content. This function
        // can also be used to "reset" the dialog after it has been invoked.
        InitResolveSyncCollisionsDialog: function () {

            var resolveButton = $ektron("#syncDialogs_btnNextResolveSyncCollisions");
            var cancelButton = $ektron("#syncDialogs_btnCancelResolveSyncCollisions");
            var messagePanel = $ektron("#resolveSyncCollisionsMessages");

            // Clear bindings on the 'Cancel' button and register
            // our click handler.
            cancelButton.unbind("click");
            cancelButton.bind("click", function (e) {
                $ektron('#ResolveSyncCollisionsModal').modalHide();
            });

            // Clear bindings on the 'Resolve' button and register
            // our click handler.
            resolveButton.unbind("click");
            resolveButton.bind("click", function (e) {
                Ektron.Workarea.Sync.Relationships.ResolveSyncCollisions();
            });

            // Reset the classes and content of the 'Resolve' button.
            resolveButton.removeClass("resolveSyncCollisionsInProgressButton");
            resolveButton.addClass("greenHover");
            resolveButton.addClass("resolveSyncCollisionsButton");
            resolveButton.text(Ektron.Workarea.Sync.Resources.resolveResolveButton);

            // Reset the content of the 'Cancel' button.
            cancelButton.text(Ektron.Workarea.Sync.Resources.resolveCancelButton);

            // Clear any lingering messages.
            messagePanel.hide();
            messagePanel.empty();
            messagePanel.removeClass("error");
            messagePanel.removeClass("success");

            // Show the buttons.
            cancelButton.show();
            resolveButton.show();
        },

        // Submits a request for conflict resolution to the sync handler,
        // updating the UI according to its status.
        ResolveSyncCollisions: function () {

            var resolveButton = $ektron("#syncDialogs_btnNextResolveSyncCollisions");
            var cancelButton = $ektron("#syncDialogs_btnCancelResolveSyncCollisions");

            // Disable the 'Resolve' and display the progress indicator.
            resolveButton.unbind("click");
            resolveButton.removeClass("resolveSyncCollisionsButton");
            resolveButton.removeClass("greenHover");
            resolveButton.addClass("resolveSyncCollisionsInProgressButton");

            // Hide the 'Cancel' button.
            cancelButton.hide();

            // Post our request to the server.
            $ektron.ajax({
                async: true,
                type: "POST",
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=ResolveConflicts",
                dataType: "json",
                cache: false,
                error: function (data) {
                    var messagePanel = $ektron("#resolveSyncCollisionsMessages");
                    var message = $ektron("<span>");

                    message.html(Ektron.Workarea.Sync.Resources.resolveCommunicationError);

                    messagePanel.empty();
                    messagePanel.addClass("error");
                    messagePanel.prepend(message);
                    messagePanel.show();

                    Ektron.Workarea.Sync.Relationships.ResolveSyncCollisionsCompleted();
                },
                success: function (data) {
                    var message = $ektron("<span>");
                    var messagePanel = $ektron("#resolveSyncCollisionsMessages");
                    messagePanel.empty();
                    messagePanel.prepend(message);

                    // If the request to resolve sync collisions returns successfully,
                    // display the appropriate message. Otherwise, display the corresponding
                    // message for each error code in the response.

                    if (data.Success) {
                        message.html(Ektron.Workarea.Sync.Resources.resolveSuccess);
                        messagePanel.addClass("success");
                    }
                    else {
                        var messageText = "";

                        if (data.Messages != null && data.Messages.length > 0) {
                            messageText = data.Messages[0];
                        }

                        messagePanel.addClass("error");
                        message.html(messageText);
                    }

                    messagePanel.show();

                    Ektron.Workarea.Sync.Relationships.ResolveSyncCollisionsCompleted();
                }
            });
        },

        // Sets the 'Resolve Sync Collisions' modal dialog to a
        // completed state.
        ResolveSyncCollisionsCompleted: function () {
            // Hide the 'Resolve' button and display the
            // 'Close' button (just the cancel button with new text).
            var resolveButton = $ektron("#syncDialogs_btnNextResolveSyncCollisions");
            var cancelButton = $ektron("#syncDialogs_btnCancelResolveSyncCollisions");

            resolveButton.hide();
            cancelButton.text(Ektron.Workarea.Sync.Resources.resolveCloseButton);
            cancelButton.show();
        },

        StartContentFolderSync: function () {
            var profileId = $ektron("#ShowSyncConfigModal #configurations li.selected").attr("data-ektron-id");
            if (profileId != null) {

                $ektron("#ShowSyncConfigModal").modalHide();

                var contentId = $ektron("#ShowSyncConfigModal").attr("data-ektron-contentid");
                var languageId = $ektron("#ShowSyncConfigModal").attr("data-ektron-languageid");
                var assetId = $ektron("#ShowSyncConfigModal").attr("data-ektron-assetid");
                var assetVersion = $ektron("#ShowSyncConfigModal").attr("data-ektron-assetversion");
                var folderId = $ektron("#ShowSyncConfigModal").attr("data-ektron-folderid");
                var isMultisite = $ektron("#ShowSyncConfigModal").attr("data-ektron-multisite");
                var findMultisite = $ektron("#ShowSyncConfigModal").attr("data-ektron-findmultisite");
                var isContentSync = $ektron("#ShowSyncConfigModal").attr("data-ektron-contentsync");
                var profileId = $ektron("#ShowSyncConfigModal ul#configurations li.selected").attr("data-ektron-id");

                $ektron.ajax({
                    async: true,
                    type: "POST",
                    dataType: "json",
                    url: Ektron.Workarea.Sync.Resources.syncHandlerPath,
                    cache: false,
                    data: "action=ContentFolderSync&id=" + profileId + "&folderId=" + folderId + "&contentId=" + contentId + "&isContentSync=" + isContentSync + "&assetid=" + assetId + "&assetversion=" + assetVersion + "&languageid=" + languageId,
                    success: Ektron.Workarea.Sync.Relationships.HandleContentFolderSyncResponse
                });

                Ektron.Workarea.Sync.Relationships.DisplayStatusDialog(profileId);
            }
            else {
                alert(Ektron.Workarea.Sync.Resources.selectProfileMessage);
            }
        },

        HandleContentFolderSyncResponse: function (data) {
            if (data != null) {
                if (!data.Success && data.Messages != null && data.Messages.length > 0) {
                    Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                        $ektron("#SyncStatusModal p.messages"),
                        data.Messages[0],
                        "error");
                }
            }
        },

        InitSyncConfigDialog: function () {
            $ektron("#ShowSyncConfigModal .headerText").text(Ektron.Workarea.Sync.Resources.selectProfileMessage);
        },

        ShowSyncConfigurations: function (contentLanguage, contentId, contentAssetId, contentAssetVersion, folderID, isMultisite, findMultisiteProfiles) {

            var contentSync = false;
            var syncId = 0;

            contentSync = contentId != null;

            if (findMultisiteProfiles !== true) {
                findMultisiteProfiles = false;
            }

            if (isMultisite !== true) {
                isMultisite = false;
            }

            $ektron("#ShowSyncConfigModal").attr("data-ektron-contentid", contentId);
            $ektron("#ShowSyncConfigModal").attr("data-ektron-languageid", contentLanguage);
            $ektron("#ShowSyncConfigModal").attr("data-ektron-assetid", contentAssetId);
            $ektron("#ShowSyncConfigModal").attr("data-ektron-assetversion", contentAssetVersion);
            $ektron("#ShowSyncConfigModal").attr("data-ektron-folderid", folderID);
            $ektron("#ShowSyncConfigModal").attr("data-ektron-multisite", isMultisite);
            $ektron("#ShowSyncConfigModal").attr("data-ektron-findmultisite", findMultisiteProfiles);
            $ektron("#ShowSyncConfigModal").attr("data-ektron-contentsync", contentSync);

            $ektron.ajax({
                async: true,
                type: "POST",
                dataType: "json",
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath,
                cache: false,
                data: "action=GetProfiles&folderId=" + folderID + "&contentId=" + contentId + "&folderType=" + isMultisite + "&isContentSync=" + contentSync + "&languageid=" + contentLanguage + "&includemultisiteprofiles=" + findMultisiteProfiles,
                success: Ektron.Workarea.Sync.Relationships.HandleGetProfilesResponse
            });
        },

        HandleLocalMultiSiteSelection: function () {
            var selectLocalMultiSitePath = $ektron("#selectLocalMultiSitePath");
            if ($ektron("#syncDialogs_selectLocalMultiSite").val() === '-1') {
                selectLocalMultiSitePath[0].selectedIndex = 0;
                selectLocalMultiSitePath.attr("disabled", "disabled");
            }
            else {
                selectLocalMultiSitePath.removeAttr("disabled");
            }
        },

        HandleGetProfilesResponse: function (data) {
            if (data != null) {
                var configurationsUL = $ektron("ul#configurations");
                var selectConfigs = $ektron("select#selectConfigs");

                $ektron("#ShowSyncConfigModal p.messages").hide();
                configurationsUL.empty();

                $ektron("#ShowSyncConfigModal #btnCloseConfigDialog").unbind("click");
                $ektron("#ShowSyncConfigModal #btnCloseConfigDialog").click(function () { $ektron("#ShowSyncConfigModal").modalHide(); });

                if (data.Relationships == null || data.Relationships.length == 0) {

                    // Hide the configuration list and display
                    // the appropriate buttons.

                    configurationsUL.hide();
                    $ektron("#ShowSyncConfigModal #btnStartSync").hide();

                    // Build the message to describe the issue to the
                    // user and attach it to the dialog.

                    var message = "<p>";
                    message += Ektron.Workarea.Sync.Resources.noSyncConfigMessage;
                    message += "</p>";
                    message += Ektron.Workarea.Sync.Resources.selectConfigEmptyMessage;

                    Ektron.Workarea.Sync.Relationships.UpdateDialogMessage(
                        $ektron("#ShowSyncConfigModal p.messages"),
                        message,
                        "error");
                }
                else {

                    // Show the configuration list and display
                    // the appropriate buttons.

                    configurationsUL.show();
                    $ektron("#ShowSyncConfigModal #btnStartSync").show();

                    // Render each profile option in the configuration list.

                    for (var i = 0; i < data.Relationships.length; i++) {
                        var listItem = $ektron("<li>");
                        listItem.addClass("relationship");

                        switch (data.Relationships[i].Direction) {
                            case "Download":
                                listItem.addClass("download");
                                break;
                            case "Upload":
                                listItem.addClass("upload");
                                break;
                            default:
                                listItem.addClass("bidirectional");
                                break;
                        }

                        var itemName = $ektron("<h5>");
                        itemName.html(data.Relationships[i].DatabaseName);

                        var ulServerInfo = $ektron("<ul>");
                        ulServerInfo.addClass("cmsInfo");
                        ulServerInfo.addClass("clearFix");

                        if (data.Relationships[i].ServerName != null &&
                        data.Relationships[i].ServerName.length > 0) {
                            var liServerName = $ektron("<li>");
                            var label = $ektron("<strong>");
                            var span = $ektron("<span>");

                            label.html("Server Name: ");
                            span.html(data.Relationships[i].ServerName);

                            liServerName.append(label);
                            liServerName.append(span);
                            ulServerInfo.append(liServerName);
                        }

                        if (data.Relationships[i].LocalSitePath != null &&
                        data.Relationships[i].LocalSitePath.length > 0) {
                            var liLocalSitePath = $ektron("<li>");
                            var label = $ektron("<strong>");
                            var span = $ektron("<span>");

                            label.html("Local Site Path: ");
                            span.html(data.Relationships[i].LocalSitePath);

                            liLocalSitePath.append(label);
                            liLocalSitePath.append(span);
                            ulServerInfo.append(liLocalSitePath);
                        }

                        if (data.Relationships[i].RemoteSitePath != null &&
                        data.Relationships[i].RemoteSitePath.length > 0) {
                            var liRemoteSitePath = $ektron("<li>");
                            var label = $ektron("<strong>");
                            var span = $ektron("<span>");

                            label.html("Remote Site Path: ");
                            span.html(data.Relationships[i].RemoteSitePath);

                            liRemoteSitePath.append(label);
                            liRemoteSitePath.append(span);
                            ulServerInfo.append(liRemoteSitePath);
                        }

                        if (data.Relationships[i].LocalMultiSite != null &&
                        data.Relationships[i].LocalMultiSite.length > 0) {
                            var liLocalMultiSite = $ektron("<li>");
                            var label = $ektron("<strong>");
                            var span = $ektron("<span>");

                            label.html("Local Multisite: ");
                            span.html(data.Relationships[i].LocalMultiSite);

                            liLocalMultiSite.append(label);
                            liLocalMultiSite.append(span);
                            ulServerInfo.append(liLocalMultiSite);
                        }

                        listItem.append(itemName);
                        listItem.append(ulServerInfo);
                        configurationsUL.append(listItem);

                        for (var j = 0; j < data.Relationships[i].Profiles.length; j++) {
                            var listItem = $ektron("<li>");
                            listItem.addClass("syncProfilesWrapper");
                            listItem.attr("data-ektron-id", data.Relationships[i].Profiles[j].ProfileId);

                            listItem.mouseover(
                            function () {
                                $ektron(this).addClass("hover");
                            }
                        );

                            listItem.mouseout(
                            function () {
                                $ektron(this).removeClass("hover");
                            }
                        );

                            listItem.click(
                            function () {
                                $ektron(this).parent().children().removeClass("selected");
                                $ektron(this).addClass("selected");
                            }
                        );

                            var divItem = $ektron("<div>");
                            divItem.addClass("profile");
                            var profileTitleStr = data.Relationships[i].Profiles[j].ProfileName;
                            if (data.Relationships[i].Profiles[j].Items.length > 0) {
                                profileTitleStr = profileTitleStr + " (";
                                for (var ii = 0; ii < data.Relationships[i].Profiles[j].Items.length; ii++) {
                                    profileTitleStr = profileTitleStr + data.Relationships[i].Profiles[j].Items[ii];
                                    if (ii < data.Relationships[i].Profiles[j].Items.length - 1) profileTitleStr = profileTitleStr + ",";
                                }
                                profileTitleStr = profileTitleStr + ") ";
                            }

                            divItem.html(profileTitleStr);

                            switch (data.Relationships[i].Profiles[j].Direction) {
                                case "Download":
                                    divItem.addClass("download");
                                    break;
                                case "Upload":
                                    divItem.addClass("upload");
                                    break;
                                default:
                                    divItem.addClass("bidirectional");
                                    break;
                            }

                            listItem.append(divItem);
                            configurationsUL.append(listItem);
                        }
                    }
                }

                $ektron("#ShowSyncConfigModal").modalShow();
            }
        }
    }
}

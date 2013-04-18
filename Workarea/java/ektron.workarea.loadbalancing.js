//define Ektron object only if it's not already defined
if (typeof (Ektron) == "undefined") {
    Ektron = {};
}

//define Ektron.Workarea object only if it's not already defined
if (typeof (Ektron.Workarea) == "undefined") {
    Ektron.Workarea = {};
}

if (typeof (Ektron.Workarea.LoadBalancing) == "undefined") {
    Ektron.Workarea.LoadBalancing =
    {
        // Handle for current polling interval timer.
        timerHandle: null,

        // Flag indicating whether any empty status should
        // terminate polling.
        stopPollingOnEmptyStatus: false,

        // Initialize the load balancing UI, preparing the status
        // dialog, binding button events, etc.
        initializeUI: function() {

            // Initialize modal status dialog.
            $ektron("#loadBalanceStatusWindow").modal({
                modal: true,
                onShow: function(hash) {
                    var messageContainer = $ektron("#loadBalanceStatusWindow .messages");

                    if ($ektron("iframe") !== null) {
                        $ektron("iframe").css("visibility", "hidden");
                    }

                    $ektron("#loadBalanceStatusWindow").css("margin-top", -1 * Math.round($ektron("#loadBalanceStatusWindow").outerHeight() / 2)).css("top", "50%");

                    Ektron.Workarea.LoadBalancing.setModalHeader(
                        Ektron.Workarea.LoadBalancing.Resources.retrievingStatus,
                        "inProgress");

                    hash.o.fadeTo("fast", 0.5, function() {
                        hash.w.fadeIn("fast");
                    });

                    Ektron.Workarea.LoadBalancing.startStatusPolling();
                },
                onHide: function(hash) {
                    var statusMessages = $ektron("#loadBalanceStatusWindow .loadBalanceStatusMessages");
                    statusMessages.empty();
                    if ($ektron("iframe") !== null) {
                        $ektron("iframe").css("visibility", "visible");
                    }
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function() {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    });

                    Ektron.Workarea.LoadBalancing.endStatusPolling();

                    Ektron.Workarea.LoadBalancing.stopPollingOnEmptyStatus = false;

                    window.location.reload(true);
                }
            });

            $ektron(".forceLoadBalancedSyncButton").bind("click", function(e) {
                // Do not initiate the synchronization if the button
                // is currently disabled.
                if ($ektron(".forceLoadBalancedSyncButton").hasClass("greenHover")) {

                    // Disable the start sync button.
                    $ektron(".forceLoadBalancedSyncButton").removeClass("greenHover");
                    $ektron(".forceLoadBalancedSyncButton").text(Ektron.Workarea.LoadBalancing.Resources.buttonInProgress);

                    // Begin the synchronization activities.
                    Ektron.Workarea.LoadBalancing.forceLoadBalancedSync();

                    // Show the status dialog.
                    $ektron("#loadBalanceStatusWindow").modalShow();
                }
            });

            // Bind click handler for status dialog close button.
            $ektron("#btnCloseLoadBalanceStatus").bind("click", function(e) {
                $ektron("#loadBalanceStatusWindow").modalHide();
            });
        },

        // Initiates load balanced sync activities. If a message is
        // provided, it will be displayed to the user as a
        // confirmation prompt (OK/Cancel). If no message is
        // specified, load balancing will be initiated immediately.
        forceLoadBalancedSync: function(message) {
            var result = true;
            if (message != null) {
                result = confirm(message);
            }

            if (result) {

                Ektron.Workarea.LoadBalancing.startStatusPolling();

                $ektron.ajax({
                    url: 'sync/LoadBalancingHandler.ashx?action=loadbalance',
                    async: true,
                    cache: false,
                    dataType: 'json',
                    error: function() {

                        // Restore button state.
                        $ektron(".forceLoadBalancedSyncButton").addClass("greenHover");
                        $ektron(".forceLoadBalancedSyncButton").text(Ektron.Workarea.LoadBalancing.Resources.buttonStart);
                    },
                    success: function(data) {

                        if (!data.Success) {
                            // Load balance failed.
                            Ektron.Workarea.LoadBalancing.setModalHeader(data.Message, "error");
                        }

                        // Retrieve the final status for the activity.
                        Ektron.Workarea.LoadBalancing.updateStatus();

                        // Restore button state.
                        $ektron(".forceLoadBalancedSyncButton").addClass("greenHover");
                        $ektron(".forceLoadBalancedSyncButton").text(Ektron.Workarea.LoadBalancing.Resources.buttonStart);
                    }
                });
            }
        },

        // Starts status polling if it is not already in progress.
        startStatusPolling: function() {
            if (Ektron.Workarea.LoadBalancing.timerHandle == null) {
                Ektron.Workarea.LoadBalancing.timerHandle = window.setInterval(
                    Ektron.Workarea.LoadBalancing.updateStatus,
                    5000);
            }
        },

        // Ends status polling if it is currently in progress.
        endStatusPolling: function() {
            if (Ektron.Workarea.LoadBalancing.timerHandle !== null) {
                window.clearInterval(Ektron.Workarea.LoadBalancing.timerHandle);
                Ektron.Workarea.LoadBalancing.timerHandle = null;
            }
        },

        // Retrieves the current load balance sync status from the server
        // and updates the status window accordingly.
        updateStatus: function() {
            $ektron.ajax({
                url: 'sync/LoadBalancingStatusHandler.ashx?action=status',
                async: true,
                cache: false,
                dataType: 'json',
                success: function(data) {
                    Ektron.Workarea.LoadBalancing.displayStatus(data);
                }
            });
        },

        // Sets the header text for the status dialog.
        setModalHeader: function(message, className) {
            if (!$ektron("#loadBalanceStatusWindow .messages span").hasClass("success") &&
                !$ektron("#loadBalanceStatusWindow .messages span").hasClass("error")) {

                var messageBlock = $ektron("<span>").html(message);
                messageBlock.addClass(className);

                $ektron("#loadBalanceStatusWindow .messages").empty();
                $ektron("#loadBalanceStatusWindow .messages").append(messageBlock);
                $ektron("#loadBalanceStatusWindow .messages").show();
            }
        },

        // Displays the specified load balance sync status
        // in the status dialog, terminating polling if the
        // message is complete.
        displayStatus: function(data) {

            if (data.IsComplete) {

                Ektron.Workarea.LoadBalancing.endStatusPolling();

                if (data.IsAuthorized) {
                    if (data.Success) {
                        // Load balance sync completed successfully.
                        Ektron.Workarea.LoadBalancing.setModalHeader(
                            Ektron.Workarea.LoadBalancing.Resources.success,
                            "success");
                    }
                    else {
                        if (data.Entries.length > 0) {
                            // Load balance failed.
                            Ektron.Workarea.LoadBalancing.setModalHeader(data.Message, "error");
                        }
                    }
                }
                else {
                    // User was not authorized to make load balance call.
                    Ektron.Workarea.LoadBalancing.setModalHeader(
                        Ektron.Workarea.LoadBalancing.Resources.notAuthorized,
                        "error");
                }
            }
            else {
                // Indicate that the load balance sync is in progress.
                Ektron.Workarea.LoadBalancing.setModalHeader(
                        Ektron.Workarea.LoadBalancing.Resources.inProgress,
                        "inProgress");
            }

            var statusMessages = $ektron("#loadBalanceStatusWindow .syncStatusMessages");

            // Clear the existing messages from the status pane.
            statusMessages.empty();
            
            // Populate the status pane with the new message list.
            if (data.Entries.length > 0) {
                var statusTable = $ektron("<table class=\"statusTable\">");
                for (var i = 0; i < data.Entries.length; i++) {
                    var statusRow = $ektron("<tr>");
                    var statusTimeCell = $ektron("<td>");
                    var statusMessageCell = $ektron("<td>");

                    statusTimeCell.html(data.Entries[i].Date);
                    statusTimeCell.addClass("time");

                    statusMessageCell.html(data.Entries[i].Message);
                    statusMessageCell.addClass("message");

                    statusRow.append(statusTimeCell);
                    statusRow.append(statusMessageCell);
                    statusTable.append(statusRow);
                }

                statusMessages.append(statusTable);
            }
            else {
                // In some cases (e.g. manual display of status dialog) an
                // empty status message indicates that polling should end.
                // In those cases, display the no status message.
                if (Ektron.Workarea.LoadBalancing.stopPollingOnEmptyStatus) {
                    Ektron.Workarea.LoadBalancing.endStatusPolling();
                    Ektron.Workarea.LoadBalancing.setModalHeader(
                        Ektron.Workarea.LoadBalancing.Resources.noStatus,
                        "error");
                }
            }

            // Append the scroll to element, keeping the most recent
            // messages in view.
            statusMessages.append("<div class=\"scrollTo\"");
            statusMessages.scrollTo(".scrollTo");

        },

        // Manually display the status dialog.
        manualStatus: function() {

            // When displaying manual status, an empty status message
            // indicates that no LB activities are current being performed.
            // Therefore stop polling on empty status.
            Ektron.Workarea.LoadBalancing.stopPollingOnEmptyStatus = true;

            // Display status dialog.
            $ektron('#loadBalanceStatusWindow').modalShow();
        }
    }
}

//define Ektron.Workarea.LoadBalancing.Resources object only if it's not already defined
if (typeof (Ektron.Workarea.LoadBalancing.Resources) == "undefined") {
    Ektron.Workarea.LoadBalancing.Resources = {};
}
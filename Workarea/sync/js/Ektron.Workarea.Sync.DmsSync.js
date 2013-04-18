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

// Define Ektron.Workarea.Sync.DmsSync object only if it's not already defined
if (typeof (Ektron.Workarea.Sync.DmsSync) == "undefined") {
    Ektron.Workarea.Sync.DmsSync =
    {
        Init: function() {
            window.focus();

            var desiredWidth;
            var desiredHeight;
            var screenWidth = screen.availWidth ? screen.availWidth : screen.width;
            var screenHeight = screen.availHeight ? screen.availHeight : screen.height;

            var divSyncConfig = $ektron("#ShowSyncConfigModal");
            if (divSyncConfig.length > 0) {
                desiredWidth = divSyncConfig.outerWidth() + 30;
                desiredHeight = divSyncConfig.outerHeight() + 120;
            }
            else {
                desiredWidth = 300;
                desiredHeight = 200;
            }

            window.resizeTo(desiredWidth, desiredHeight);
            window.moveTo(
                Math.round((screenWidth - desiredWidth) / 2),
                Math.round((screenHeight - desiredHeight) / 2));
        },

        LayoutDialogs: function() {
            var divSyncConfig = $ektron("#ShowSyncConfigModal");
            if (divSyncConfig.length > 0) {
                divSyncConfig.css("margin-top", -(Math.round(divSyncConfig.outerHeight() / 4)));

                setTimeout(function() {
                    $ektron(".ektronModalClose").unbind("click");
                    $ektron("#syncDialogs_btnCloseSyncStatus").unbind("click");
                    $ektron("#btnCloseConfigDialog").unbind("click");

                    $ektron("#btnStartSync").click(function() {
                        var divSyncStatus = $ektron("#SyncStatusModal");
                        divSyncStatus.css("margin-top", -(Math.round(divSyncStatus.outerHeight() / 4)));

                        $ektron("#syncDialogs_btnCloseSyncStatus").click(function() {
                            self.close();
                        });
                    });

                    $ektron(".ektronModalClose").click(function() {
                        self.close();
                    });

                    $ektron("#btnCloseConfigDialog").click(function() {
                        self.close();
                    });
                },
                200);
            }
        }
    }
}

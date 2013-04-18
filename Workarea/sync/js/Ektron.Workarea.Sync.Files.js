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
if (typeof (Ektron.Workarea.Sync.Files) == "undefined") {
    Ektron.Workarea.Sync.Files =
    {
        Submit: function(profileId) {
            var tree = $find("rtvFiles");
            var checkedNodes = tree.get_checkedNodes();
            if (checkedNodes != null && checkedNodes.length > 0) {
                var checkedFilesParam = "";

                for (var i = 0; i < checkedNodes.length; i++) {
                    checkedFilesParam += encodeURIComponent(checkedNodes[i].get_value()) + "|";
                }

                var response = Ektron.Workarea.Sync.Relationships.IsSyncInProgress();
                if (response != null && response.Success && !response.IsSyncInProgress) {
                    Ektron.Workarea.Sync.Files.SendSynchronizeRequest(profileId, checkedFilesParam);
                    Ektron.Workarea.Sync.Relationships.DisplayStatusDialog(profileId);
                }
                else {
                    Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                        Ektron.Workarea.Sync.Relationships.syncInProgressDialogTitle,
                        response.Messages[0],
                        null,
                        function() {
                            Ektron.Workarea.Sync.Relationships.DisplayStatusDialog(response.ProfileId);
                        });
                }
            }
            else {
                alert("Please select the files you would like to synchronize.");
            }
        },

        SendSynchronizeRequest: function(profileId, checkedFilesParam) {
            $ektron.ajax({
                url: Ektron.Workarea.Sync.Resources.syncHandlerPath + "?action=SyncFiles&files=" + checkedFilesParam + "&id=" + profileId,
                dataType: "json",
                async: true,
                success: Ektron.Workarea.Sync.Files.HandleSyncFilesResponse
            });
        },

        HandleSyncFilesResponse: function(data) {

        },

        DisplaySyncInProgressDialog: function(message, id) {
            Ektron.Workarea.Sync.Relationships.ShowConfirmDialog(
                Ektron.Workarea.Sync.Relationships.syncInProgressDialogTitle,
                message,
                null,
                function() {
                    Ektron.Workarea.Sync.Relationships.DisplayStatusDialog(id);
                });
        }
    }
}

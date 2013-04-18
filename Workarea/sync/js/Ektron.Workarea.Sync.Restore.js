if ("undefined" === typeof Ektron.Workarea) {
    Ektron.Workarea = { }
}

if ("undefined" === typeof Ektron.Workarea.Sync) {
    Ektron.Workarea.Sync = { }
}

if ("undefined" === typeof Ektron.Workarea.Sync.Restore) {
    Ektron.Workarea.Sync.Restore =
    {
        BindNodeChanged: function() {
            $ektron("#tvFileHierarchy input[type='checkbox']").click(function() {
                Ektron.Workarea.Sync.Restore.HandleNodeChanged(this);
            });
        },

        HandleNodeChanged: function(checkBoxElement) {
            var selectedCheckBox = $ektron(checkBoxElement);
            var selectedCheckBoxId = selectedCheckBox.attr("id");
            var divChildNodes = $ektron("#" + selectedCheckBoxId.substring(0, selectedCheckBoxId.length - 8) + "Nodes");

            divChildNodes.find("input[type='checkbox']").each(function(index, checkbox) {
                if (selectedCheckBox.is(":checked")) {
                    $ektron(checkbox).attr("checked", "checked");
                }
                else {
                    $ektron(checkbox).removeAttr("checked");
                }
            });
        },

        Submit: function() {
            alert("This feature is not yet implemented.");
            //document.forms[0].submit();
        }
    }
}    

Ektron.ready(function() {
    Ektron.Workarea.Sync.Restore.BindNodeChanged();
});
    

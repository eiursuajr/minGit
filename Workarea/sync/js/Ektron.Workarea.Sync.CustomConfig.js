if (typeof (Ektron) == "undefined") {
    Ektron = {};
}
if (typeof (Ektron.Workarea) == "undefined") {
    Ektron.Workarea = {};
}
if (typeof (Ektron.Workarea.Sync) == "undefined") {
    Ektron.Workarea.Sync = {};
}
if (typeof (Ektron.Workarea.Sync.CustomConfig) == "undefined") {
    Ektron.Workarea.Sync.CustomConfig =
    {
        Init: function () {
            
        },

        Save: function () {
            var selectEntities = $ektron('select.selectedentitieslist option');
            var strSelectedEntities = '';
            selectEntities.each(function () {
                strSelectedEntities = strSelectedEntities + $ektron(this).text() + ',';
            });
            $ektron('#hdnSelectedEntities').val(strSelectedEntities);
            $ektron('form').submit();
        },

        IncludeItem: function () {
            var selectEntity = $ektron('select.availlableentitieslist option:selected');
            $ektron('select.selectedentitieslist').append($ektron(selectEntity).clone());
            selectEntity.remove();
        },

        IncludeAllItems: function () {
            var selectEntities = $ektron('select.availlableentitieslist option');
            $ektron('select.selectedentitieslist').append($ektron(selectEntities).clone());
            selectEntities.remove();
        },

        ExcludeItem: function () {
            var selectEntity = $ektron('select.selectedentitieslist option:selected');
            $ektron('select.availlableentitieslist').append($ektron(selectEntity).clone());
            selectEntity.remove();
        },

        ExcludeAllItems: function () {
            var selectEntities = $ektron('select.selectedentitieslist option');
            $ektron('select.availlableentitieslist').append($ektron(selectEntities).clone());
            selectEntities.remove();
        },

        MoveUp: function () {
            var selectEntity = $ektron('select.selectedentitieslist option:selected');
            var prevOption = selectEntity.prev('option');
            if (prevOption.text() != "") {
                selectEntity.remove();
                $(prevOption).before($ektron(selectEntity));
            }
        },

        MoveDown: function () {
            var selectEntity = $ektron('select.selectedentitieslist option:selected');
            var prevOption = selectEntity.next('option');
            if (prevOption.text() != "") {
                selectEntity.remove();
                $(prevOption).after($ektron(selectEntity));
            }
        }
    };
}
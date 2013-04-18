if (typeof (Ektron) == "undefined") {
    Ektron = {};
}
if (typeof (Ektron.Workarea) == "undefined") {
    Ektron.Workarea = {};
}
if (typeof (Ektron.Workarea.Sync) == "undefined") {
    Ektron.Workarea.Sync = {};
}
if (typeof (Ektron.Workarea.Sync.Preview) == "undefined") {
    Ektron.Workarea.Sync.Preview =
    {
        Init: function () {
            $ektron('.uitabs').tabs({
                cookie: {
                    expires: 1
                }
            });
        }
    };
}

Ektron.ready(function () {
    Ektron.Workarea.Sync.Preview.Init();
});
Ektron.ready(function () {

    if (typeof (Ektron.Widget) == "undefined") {
        Ektron.Widget = {};
    }

    if (typeof (Ektron.Widget.BrightcoveVideoView) == "undefined") {
        Ektron.Widget.BrightcoveVideo = {};
    }

    Ektron.Widget.BrightcoveVideoView = {
        ShowVideo: function (sourceID, targetID) {
            //brightcove.removeExperience("BCViewPlayer");
            var player = $ektron(sourceID)[0];
            var target = $ektron(targetID)[0];
            brightcove.createExperience(player, target, true);
        }
    }
});
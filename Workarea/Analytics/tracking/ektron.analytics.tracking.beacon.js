if ("undefined" === typeof Ektron) { Ektron = {}; }
if ("undefined" === typeof Ektron.Analytics) { Ektron.Analytics = {}; }
if ("undefined" === typeof Ektron.Analytics.Tracking) { Ektron.Analytics.Tracking = {}; }
if ("undefined" === typeof Ektron.Analytics.Tracking.Beacon) {
    Ektron.Analytics.Tracking.Beacon = {
        init: function() {
            if ('undefined' == typeof Ektron || 'undefined' == typeof Ektron.Site || 'undefined' == typeof Ektron.Site.SiteData) {
                window.setTimeout(Ektron.Analytics.Tracking.Beacon.init, 100);
                return;
            }

            // recover persisted data from cookie
            Ektron.Analytics.Tracking.Beacon.recoverData();

            // bind all click events to the logger:
            $ektron(document).mousedown(Ektron.Analytics.Tracking.Beacon.logClick);
        },

        logClick: function(e) {
            var data = {};
            data.x = e.pageX;
            data.y = e.pageY;
            data.width = $ektron(document).width();
            data.height = $ektron(document).height();
            data.locationHref = window.location.href;
            data.date = new Date();
			data.xml = '';
            Ektron.Analytics.Tracking.Beacon.storeEvent(data);
        },

        storeEvent: function(data) {
            var elapsedSeconds = Ektron.Analytics.Tracking.Beacon.totalElapsedSeconds();

            // store the event data
            Ektron.Analytics.Tracking.Beacon.eventData.push(data);

            // if the quantity stored, or time since last store are too great, then upload the data
            if (Ektron.Analytics.Tracking.Beacon.eventData.length >= Ektron.Analytics.Tracking.Beacon.BUFFER_LIMIT
                || elapsedSeconds >= Ektron.Analytics.Tracking.Beacon.BUFFER_TIME_LIMIT) {
                Ektron.Analytics.Tracking.Beacon.uploadData();
            }

            // persist data to cookie
            Ektron.Analytics.Tracking.Beacon.saveData();
        },

        uploadData: function() {
            var trasmitPackage = {};
            trasmitPackage.dataType = "ClickEventData";
            trasmitPackage.count = Ektron.Analytics.Tracking.Beacon.eventData.length;
            trasmitPackage.payload = Ektron.Analytics.Tracking.Beacon.eventData;
            var stringData = Ektron.JSON.stringify(trasmitPackage);
            var href = Ektron.Analytics.Tracking.Beacon.appPath() + 'analytics/tracking/ektronAnalyticsTracking.ashx?type=storeclickdata';
            $ektron.post(href, "beacondata=" + escape(stringData), function(hostData) { }, "json");

            // clear the buffer
            Ektron.Analytics.Tracking.Beacon.eventData.length = 0;
            Ektron.Analytics.Tracking.Beacon.removeTrackingCookie();
        },

        elapsedSeconds: function() {
            if (Ektron.Analytics.Tracking.Beacon.eventData.length > 0) {
                var now = new Date();
                var last = Ektron.Analytics.Tracking.Beacon.eventData[Ektron.Analytics.Tracking.Beacon.eventData.length - 1].date;
                return (now - last) / 1000;
            }
            return 0;
        },

        totalElapsedSeconds: function() {
            if (Ektron.Analytics.Tracking.Beacon.eventData.length > 0) {
                var now = new Date();
                var first = Ektron.Analytics.Tracking.Beacon.eventData[0].date;
                return (now - first) / 1000;
            }
            return 0;
        },

        saveData: function() {
            var stringData = Ektron.JSON.stringify(Ektron.Analytics.Tracking.Beacon.eventData);
            stringData = escape(stringData);
            Ektron.Analytics.Tracking.Beacon.setTrackingCookie(stringData);
        },

        recoverData: function() {
            var stringData = $ektron.cookie(Ektron.Analytics.Tracking.Beacon.cookieKey);
            if ("undefined" != typeof stringData && null != stringData && "undefined" != typeof stringData.length && stringData.length > 0) {
                try {
                    Ektron.Analytics.Tracking.Beacon.eventData = Ektron.JSON.parse(unescape(stringData));
                    return;
                }
                catch (e) { }
            }

            Ektron.Analytics.Tracking.Beacon.eventData = new Array();
        },

        appPath: function() {
        if ("undefined" != typeof Ektron.Site && "undefined" != typeof Ektron.Site.SiteData && "undefined" != typeof Ektron.Site.SiteData.Cms && "undefined" != typeof Ektron.Site.SiteData.Cms.ApplicationPath) {
                return Ektron.Site.SiteData.Cms.ApplicationPath;
            }
            return "";
        },

        setTrackingCookie: function(value) {
            $ektron.cookie(Ektron.Analytics.Tracking.Beacon.cookieKey, value, { expires: 365, "encoding": "none", "path": "/" });
        },

        removeTrackingCookie: function() {
            $ektron.cookie(Ektron.Analytics.Tracking.Beacon.cookieKey, null, { expires: -1, "encoding": "none", "path": "/" });
        },

        BUFFER_LIMIT: 5,
        BUFFER_TIME_LIMIT: 600 /* 600 seconds = 10 minutes */,
        cookieKey: 'ektron_analytics_tracking_beacon',
        eventData: []
    }; 
}

Ektron.ready(function() {
    Ektron.Analytics.Tracking.Beacon.init(); 
});
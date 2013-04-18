if ("undefined" == typeof (Ektron)) {Ektron = {} }
if ("undefined" == typeof (Ektron.Symantec)) {
    Ektron.Symantec =
    {
        init: function () {
            if ("localhost" == location.hostname && typeof SymRealOnLoad != "undefined") {
                if (document != null && document.cookie && -1 == document.cookie.indexOf("Ektron.SymantecWarning=")) {
                    var msg = "The Symantec ad blocking in your system may prevent some web pages from functioning properly. Please consult your IT administrator if you need to turn it off. ";
                    msg += "\nFor more information, please read:\n\nhttp://dev.ektron.com/kb_article.aspx?id=24520";
                    alert(msg);
                    var date = new Date();
                    var periodInDays = 1;
                    date.setTime(date.getTime() + (periodInDays * 24 * 60 * 60 * 1000));
                    var expires = "expires=" + date.toUTCString(); // use expires attribute, max-age is not supported by IE
                    document.cookie = "Ektron.SymantecWarning=true; " + expires + "; path=/";
                }
            }
        }
    }; 
}

Ektron.ready(function (event, eventName) {
    Ektron.Symantec.init();
});
//define $ektron
var $ektron = window.$ektron = window.jQuery;
window.jQuery.noConflict(true);
if ("undefined" == typeof (window.$)) { window.$ = window.$ektron; }
if ("undefined" == typeof (window.jQuery)) { window.jQuery = window.$ektron; }

//define Ektron
if ("undefined" == typeof Ektron) { var Ektron = window.Ektron = {}; }

//integrate jquery and ms-ajax
Ektron.ready = function (fn) {
    if (!jQuery.isReady) {
        $ektron(document).ready(fn);
    }
    else {
        $ektron(document).one("EktronReady", function () {
            try {
                fn.apply(this, arguments);
            }
            catch (ex) {
                if ("undefined" !== typeof (Ektron.OnException)) {
                    Ektron.OnException.diagException(ex, [fn]);
                }
            }
        });
    }
};
// for PageRequestManager
Ektron.ready.endRequestHandler = function (sender, args) {
    var objError = args.get_error();
    if (objError) {
        args.set_errorHandled(true);
        if ("undefined" !== typeof (Ektron.OnException)) {
            Ektron.OnException.diagException(new Error("Error during Ajax request:\n" + objError.message), arguments);
        }
    }
    else {
        $ektron(".ektron-registration-order").append("<li>MS-AJAX complete</li>")
        Ektron.ClientManager.MSAjax.ready = true;
        Ektron.ClientManager.ready();
    }
};
// for Page.ClientScript.GetCallbackEventReference, which is JavaScript function WebForm_DoCallback
Ektron.ready.ClientScriptCallbackEvent =
	{
	    eventCallback: function (result, context) {
	        // note: $ektron.ajaxCallback triggers "EktronReady", ["callback"]
	        $ektron(document).trigger("EktronReady", ["callbackEvent", context]);
	    }
	, errorCallback: function (message, context) {
	    if ("undefined" !== typeof (Ektron.OnException)) {
	        Ektron.OnException.diagException(new Error("Error during Ajax DoCallback request:\n" + message), arguments);
	    }
	}
	};
// If RadAjaxControl is already defined at this time, DON'T override its _endRequest handler,
// just add to the MS Ajax endRequest, which will fire after Telerik's.
if (typeof Sys != "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager && Sys.WebForms.PageRequestManager.getInstance() != null) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.ready.endRequestHandler);
}
else {
    $ektron(document).ready(function () {
        if (typeof Sys != "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager && Sys.WebForms.PageRequestManager.getInstance() != null) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.ready.endRequestHandler);
        }
    });
}

if ("undefined" == typeof (Ektron.ClientManager)) {
    Ektron.ClientManager = {
        add: function (settings) {
            switch (settings.mode) {
                case "postback":
                    $ektron("#" + settings.id).attr("data-ektron-clientmanager-loaded", "false");
                    $ektron.ajax({
                        url: settings.path,
                        dataType: 'script',
                        async: false,
                        success: function () {
                            $ektron("#" + settings.id).attr("data-ektron-clientmanager-loaded", "true");
                            Ektron.ClientManager.updateHiddenField(settings.id);
                        }
                    });
                    setTimeout(function () {
                        Ektron.ClientManager.ready();
                    }, 10);
                    break;
                case "callback":
                    Ektron.ClientManager.updateHiddenField(settings.id);
                    break;
                case "css-ajax":
                    Ektron.ClientManager.updateHiddenField(settings.id);
                    break;
            }
        },
        MSAjax: {
            ready: false
        },
        ready: function () {
            //get all script tags that have been registered and loaded
            var externalScripts = $ektron("script[data-ektron-clientmanager-loaded='true']").each(function (i) {
                $ektron(this).remove();
            });
            //raise Ektron.ready only after all scripts have been loaded
            externalScripts = $ektron("script[data-ektron-clientmanager-loaded='false']");
            if (externalScripts.length == 0 && Ektron.ClientManager.MSAjax.ready) {
                $ektron(document).trigger("EktronReady");
            }
        },
        updateHiddenField: function (id) {
            var hiddenField = $ektron("#EktronClientManager");
            if (hiddenField.length > 0) {
                var ids = hiddenField.attr("value") + "," + id;
                hiddenField.attr("value", ids);
            }
        }
    };
}
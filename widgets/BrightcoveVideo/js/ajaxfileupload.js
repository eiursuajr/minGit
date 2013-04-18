
$ektron.extend({


    createUploadIframe: function (id, uri) {
        //create frame
        var frameId = 'jUploadFrame' + id;

        if (window.ActiveXObject) {
            var io = document.createElement('<iframe id="' + frameId + '" name="' + frameId + '" />');
            if (typeof uri == 'boolean') {
                io.src = 'javascript:false';
            }
            else if (typeof uri == 'string') {
                io.src = uri;
            }
        }
        else {
            var io = document.createElement('iframe');
            io.id = frameId;
            io.name = frameId;
        }
        io.style.position = 'absolute';
        io.style.top = '-1000px';
        io.style.left = '-1000px';

        document.body.appendChild(io);

        return io
    },
    createUploadForm: function (id, fileElementId) {
        //create form	
        var formId = 'jUploadForm' + id;
        var fileId = 'jUploadFile' + id;
        var form = $ektron('<form  action="" method="POST" name="' + formId + '" id="' + formId + '" enctype="multipart/form-data"></form>');

        // we can't clone a file input, so we need to replicate the HTML for it and append a duplicate of that HTML
        var fileInputWrapper = $ektron('.' + fileElementId);
        var newFileInput = fileInputWrapper.html();
        fileInputWrapper.attr('id', fileId);
        fileInputWrapper.before('<span class="uploadInput">' + newFileInput + '</span>');

        fileInputWrapper.appendTo(form);

        //set attributes
        form.css('position', 'absolute');
        form.css('top', '-1200px');
        form.css('left', '-1200px');
        form.appendTo('body');
        return form;
    },

    ajaxFileUpload: function (s) {
        // TODO introduce global settings, allowing the client to modify them for all requests, not only timeout		
        s = $ektron.extend({}, $ektron.ajaxSettings, s);
        var id = new Date().getTime()
        var form = $ektron.createUploadForm(id, s.fileElementId);
        var io = $ektron.createUploadIframe(id, s.secureuri);
        var frameId = 'jUploadFrame' + id;
        var formId = 'jUploadForm' + id;
        // Watch for a new set of requests
        if (s.global && !$ektron.active++) {
            $ektron.event.trigger("ajaxStart");
        }
        var requestDone = false;
        // Create the request object
        var xml = {}
        if (s.global)
            $ektron.event.trigger("ajaxSend", [xml, s]);
        // Wait for a response to come back
        var uploadCallback = function (isTimeout) {
            var io = document.getElementById(frameId);
            try {
                if (io.contentWindow) {
                    xml.responseText = io.contentWindow.document.body ? io.contentWindow.document.body.innerHTML : null;
                    xml.responseXML = io.contentWindow.document.XMLDocument ? io.contentWindow.document.XMLDocument : io.contentWindow.document;

                } else if (io.contentDocument) {
                    xml.responseText = io.contentDocument.document.body ? io.contentDocument.document.body.innerHTML : null;
                    xml.responseXML = io.contentDocument.document.XMLDocument ? io.contentDocument.document.XMLDocument : io.contentDocument.document;
                }
            } catch (e) {
                //$ektron.handleError(s, xml, null, e);
            }
            if (xml || isTimeout == "timeout") {
                requestDone = true;
                var status;
                try {
                    // Clean Up our response text -- Some of these are browser specific
                    var responseText = xml.responseText;
                    responseText = responseText.replace("<pre style=\"word-wrap: break-word; white-space: pre-wrap;\">", ""); // Chrome Browser Only
                    responseText = responseText.replace("</pre>", ""); // Chrome Browser Only
                    responseText = responseText.replace("<PRE>", "");
                    responseText = responseText.replace("</PRE>", "");
                    xml.responseText = responseText;

                    status = isTimeout != "timeout" ? "success" : "error";
                    // Make sure that the request was successful or notmodified
                    if (status != "error") {
                        // process the data (runs the xml through httpData regardless of callback)
                        var data = $ektron.uploadHttpData(xml, s.dataType);
                        // If a local callback was specified, fire it and pass it the data
                        if (s.success)
                            s.success(data, status);

                        // Fire the global callback
                        if (s.global)
                            $ektron.event.trigger("ajaxSuccess", [xml, s]);
                    }
                    else {
                        //$ektron.handleError(s, xml, status);
                    }
                } catch (e) {
                    status = "error";
                    //$ektron.handleError(s, xml, status, e);
                }

                // The request was completed
                if (s.global)
                    $ektron.event.trigger("ajaxComplete", [xml, s]);

                // Handle the global AJAX counter
                if (s.global && ! --$ektron.active)
                    $ektron.event.trigger("ajaxStop");

                // Process result
                if (s.complete)
                    s.complete(xml, status);

                $ektron(io).unbind()

                setTimeout(function () {
                    try {
                        $ektron(io).remove();
                        $ektron(form).remove();
                    } catch (e) {
                        //$ektron.handleError(s, xml, null, e);
                    }

                }, 100)

                xml = null

            }
        }
        // Timeout checker
        if (s.timeout > 0) {
            setTimeout(function () {
                // Check to see if the request is still happening
                if (!requestDone) uploadCallback("timeout");
            }, s.timeout);
        }
        try {
            // var io = $ektron('#' + frameId);
            var form = $ektron('#' + formId);
            $ektron(form).attr('action', s.url);
            $ektron(form).attr('method', 'POST');
            $ektron(form).attr('target', frameId);

            if (form.encoding) {
                form.encoding = 'multipart/form-data';
            }
            else {
                form.enctype = 'multipart/form-data';
            }

            var requestElement = $ektron('<input id="request" name="request" value="" ></input>');
            $ektron(requestElement).val(s.data.request);
            $ektron(requestElement).appendTo(form);

            $ektron(form).submit();

        } catch (e) {
            $ektron.handleError(s, xml, null, e);
        }
        if (window.attachEvent) {
            document.getElementById(frameId).attachEvent('onload', uploadCallback);
        }
        else {
            document.getElementById(frameId).addEventListener('load', uploadCallback, false);
        }
        return { abort: function () { } };

    },

    uploadHttpData: function (r, type) {
        var data = !type;
        data = type == "xml" || data ? r.responseXML : r.responseText;
        // If the type is "script", eval it in global context
        if (type == "script")
            $ektron.globalEval(data);
        // Get the JavaScript object, if JSON is used.
        if (type == "json") {
            data = data.replace("<pre>", "").replace("</pre>", "");
            eval("data = " + data);
        }
        // evaluate scripts within html
        if (type == "html")
            $ektron("<div>").html(data).evalScripts();
        if (type == "string") {
            data = r.responseText;
        }
        return data;
    }
})


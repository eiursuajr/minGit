// File: confirm.js

(function($)
{
    $.fn.confirm = function(options)
    {
        var settings = 
        {
            message : "Are you sure you want to do this?",
            title : "Are you sure?",
            event : "click",
            standalone : false,
            onOkay : function() {return true;},
            onCancel : function() {}
        };
        
        $.extend(settings, options);
        
        this.each(function()
        {
            var obj = this;
            var $obj = $(this);
            var evtHandler;
            
            // Stores the jQuery and inline events for the selected object
            var BackupEvent = function(evt)
            {
                var storedEvents = $.data(obj, "events");
                
                if(typeof obj._realEvent == "undefined")
                    obj._realEvent = {};
                
                obj._realEvent[evt] = new Array();
                
                if(obj["on"+evt])
                {
                    // push a copy of the static onclick event handler
                    obj._realEvent[evt].push(
                        (function(f)
                        {
                            // decorate the handler to return true
                            // if it originally returned nothing
                            // (so that it functions the same as 
                            // before)
                            return function()
                            {
                                var result = f();
                                if(typeof(result) == "undefined")
                                {
                                    return true;
                                }
                                return result;
                            };
                        }(obj["on"+evt])));
                }
                
                if((typeof storedEvents == "undefined") || 
                   (typeof storedEvents[evt] == "undefined")) return;                
                
                for (var i in storedEvents[evt])
                {
                    obj._realEvent[evt].push(storedEvents[evt][i]);
                }
            }
            
            // Fires the stored events for an object (without firing the current events !)
            var FireOriginalEvent = function(evt)
            {
                $obj.unbind(evt, evtHandler);
                $.each(obj._realEvent[evt], function()
                {
                    $obj.bind(evt, this);
                });
                
                $obj.each(function()
                {
                    if(evt == "click" && this.tagName == "A")
                    {
                        if($(this).triggerHandler(evt) || obj._realEvent[evt].length == 0)
                            window.location = $(this).attr("href");
                    }
                    else
                    {
                        $(this).trigger(evt);
                    }
                });
            }
            
            // Restores the stored events for an object
            var RestoreOriginalEvent = function(evt)
            {
                $obj.unbind(evt, evtHandler);
                $.each(obj._realEvent[evt], function()
                {
                    $obj.bind(evt, this);
                });
            }
            
            if(typeof(options) == "string")
            {
                options = {message : options};
                
                if(options == "destroy")
                {
                    RestoreOriginalEvent(settings.event);
                }
            }
            
            // Create dialog
            var dlg = $("<div></div>");
            
            dlg.attr("title", settings.title);
            dlg.html(settings.message);
            
            if(settings.standalone == false)
            {
                // Create the dialog class
                dlg.dialog(
                    {
                        buttons:
                        {
                            "Yes":function(){$(this).dialog("close");settings.onOkay();},
                            "No":function(){$(this).dialog("close");settings.onCancel();}
                        }
                    });
                return;
            }
            
            // Decorate the event handlers passed in
            var onOkay = function()
            {
                // Close the dialog first
                dlg.dialog("close");
                if(settings.onOkay())
                {
                    // Fire the real event first
                    FireOriginalEvent(settings.event);
                }
                
                // And rebind original event
                $obj.one(settings.event, evtHandler);
            };
            
            var onCancel = function()
            {
                // Close the dialog first
                dlg.dialog("close");
                // Call the passed in event handler
                settings.onCancel();
                // And rebind the original event
                $obj.one(settings.event, evtHandler);
            };
            
            // Create the dialog class
            dlg.dialog(
                    {
                        autoOpen: false,
                        buttons:
                        {
                            "Yes":onOkay,
                            "No":onCancel
                        }
                    });
            
            // Create the onclick event handler
            var evtHandler = function(evt)
            {
                // Let's stop it from messing anything else
                evt.stopPropagation();
                evt.preventDefault();
                
                // And show the dialog
                dlg.dialog("open");
                return false;
            };
            
            // Store the old click events
            BackupEvent(settings.event);
            
            // Get rid of any old click events
            $obj.unbind(settings.event);
            
            // Don't forget inline clicks !
            obj.onclick = null;
            
            // And register the click event handler
            $obj.one(settings.event, evtHandler);
        });
    };
})($ektron);
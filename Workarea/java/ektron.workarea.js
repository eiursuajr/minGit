//define Ektron object only if it's not already defined
if (typeof(Ektron) == "undefined") {
	Ektron = {};
}

//define Ektron.Workarea object only if it's not already defined
if (typeof(Ektron.Workarea) == "undefined") {
	Ektron.Workarea = {};
}

/*
Ektron.Workarea.Overlay Class Details
Dependencies:	ektron.js			    // required by the page loading ektron.workarea.js
				ektron.workarea.css 	// required by any page rendering the overlay effect

Methods:
                .unblock()  		    // unblocks any frames/iframes blocked by a
                                           previous use of block().
                .block(settings)	    // blocks the UI of one or more frames/iframes.
                                           Takes an optional settings parameter.

The optional settings object used in the block() method is a JS object that contains overrides
for the default values used for blocking.  The options provided by this object are as follows:

    messageText:    The text displayed above the overlay layer.
                    The default is the English text "Please wait...",
                    and is typically hidden via the CSS.
    messageLayer:   the markup that wraps the messageText.
                    The default is "<h1><span></span></h1>".
    overlayClass:   the CSS class used to style the overlay.
                    The default is "ui-widget-overlay", which is the class
                    used to create overlay effects in the jQuery Themeroller CSS.
    messageClass:   the CSS class used to style the DIV wrapping the messageLayer
                    and messageText.  The default is "ektronOverlayPleaseWait",
                    and it appears as a large rotating gif currently.
    baseZ:          the z-index value used by the first of the three layers
                    of the overlay effect.  The default is 3000.
    centerX:        a Boolean value indicating whether or not the messageClass wrapper
                    should be centered horizontally or not.  The default value is true.
    centerY:        a Boolean value indicating whether or not the messageClass wrapper
                    should be centered vertically or not.  The default value is true.
    onBlock:        a callback function that, when provided, will be executed
                    after the overlay block method is called.  The default value is null.
    onUnblock:      a callback function that, when provided, will be executed after the
                    overlay unblock method is called.  The default value is null.
    target:         a JS data object specifying one or more frames/iframes by id (or name
                    if no id attribute is found), and whether or not the messageClass
                    layer should be visible.  The default is to block the current window
                    where the block method is called.
                    
                    For example:
                    
                    Ektron.Workarea.Overlay.block(
                    {
                        target: 
                        {
                            "ek_main" : true,  // this will block the frame with id "ek_main"
                            "ek_nav_bottom" : false
                        }
                    });
*/
if (typeof(Ektron.Workarea.Overlay) == "undefined")
{
    Ektron.Workarea.Overlay =
    {
        /* properties */
        defaults: {
            messageLayer: "<h1><span></span></h1>",
            messageText: "Please wait...",
            overlayClass: "ui-widget-overlay",
            messageClass: "ektronOverlayPleaseWait",
            baseZ: 3000,
            centerX: true,
            centerY: true,
            onBlock: null,
            onUnblock: null,
            target: null
        },

        /* methods */
        // blocks one or more windows/frames as desired
        block: function(settings)
        {
            var s = $ektron.extend(Ektron.Workarea.Overlay.defaults, settings);
            if (s.target !== null && typeof(s.target) == "object")
            {
                // loop through target object properties and initialize
                for(property in s.target)
                {
                    var docs = Ektron.Workarea.Overlay.findFrame(property, top);
                    for(var i in docs)
                    {
                        var target = $ektron(docs[i].document);
                        var showWait = s.target[property];
                        Ektron.Workarea.Overlay.createLayers(target, showWait, s);
                    }
                }
            }
            else
            {
                // no targets specified, use window.self
                Ektron.Workarea.Overlay.createLayers($ektron(document), true, s);
            }


            if (s.onBlock)
            {
                $ektron(document).one("ektronOverlayShowComplete", function()
                {
                    s.onBlock();
                });
            }

            // bind the onUnblock callback if present
            if (s.onUnblock)
            {
                $ektron(document).one("ektronOverlayHideComplete", function()
                {
                    s.onUnblock();
                });
            }

            $ektron(document).one("ektronOverlayUnblock", function()
            {
                if (s.target !== null && typeof(s.target) == "object")
                {
                    for(property in s.target)
                    {
                        var docs = Ektron.Workarea.Overlay.findFrame(property, top);
                        for(var i in docs)
                        {
                            var target = $ektron(docs[i].document);
                            var showWait = s.target[property];
                            Ektron.Workarea.Overlay.hide(target, s);
                        }
                    }
                }
                else
                {
                    // no targets specified, use the current window's document object
                    Ektron.Workarea.Overlay.hide($ektron(document), s);
                }
            });

            $ektron(document).trigger("ektronOverlayShowComplete", s);
        },

        // createLayers
        createLayers: function(_el, wait, s)
        {
            var body = _el.find("body");
            var i = body.find(".ektronOverlayIframe");
            var o = body.find("." + s.overlayClass);
            var m = body.find("." + s.messageClass);
            var z = s.baseZ;

            if (i.length === 0)
            {
                var layer1 = ($ektron.browser.msie) ? '<iframe class="ektronOverlay ektronOverlayIframe" style="z-index: ' + (s.baseZ) + ';" src="about:blank"></iframe>' : '<div class="ektronOverlay ektronOverlayIframe" style="display: none"></div>';
                body.append(layer1);
            }
            if (o.length === 0)
            {
                var layer2 = '<div class="ektronOverlay ' + s.overlayClass + '" style="z-index: ' + (s.baseZ++) + ';"></div>';
                body.append(layer2);
            }
            if (m.length === 0 && wait)
            {
                var messageText = $ektron(document.createTextNode(s.messageText));
                var layer3 = '<div class="ektronOverlay ' + s.messageClass + '" style="z-index: ' + (s.baseZ++) + ';"></div>';
                body.append(layer3);
                layer3 = $ektron(s.messageClass);
                layer3.prepend(messageText);
                messageText.wrap(s.messageLayer);
                messageLayer = layer3.children();

                // now that thigns are visible we can center them if desired
                if (s.centerY)
                {
                    messageLayer.css({
                        "position": "fixed",
                        "margin-top": (-1 * Math.round(messageLayer.outerHeight()/2)),
                        "top": "50%"
                    });
                }
                if (s.centerX)
                {
                    messageLayer.css({
                        "position": "fixed",
                        "margin-left": (-1 * Math.round(messageLayer.outerWidth()/2)),
                        "left": "50%"
                    });
                }
            }

            // show the overlay layers
            Ektron.Workarea.Overlay.show(body, s);
        },

        show: function (body, s)
        {
            body.find("div.ektronOverlay:not('.ektronOverlayIframe')").show();
            $ektron(document).trigger("ektronOverlayShowComplete");
        },

        hide: function(body, s)
        {
            body.find("div.ektronOverlay:not('.ektronOverlayIframe')").hide();
            $ektron(document).trigger("ektronOverlayHideComplete");
        },

        findFrame: function(frameName, windowObj)
        {
            var result = [];
            var currentFrame = null;
            if (windowObj.frames)
            {
                for(var i=0;i < windowObj.frames.length; i++)
                {
                    // true frames don't let us get the id via the frames array, so...
                    currentFrame = (windowObj.document.getElementsByTagName("frame").length > 0 ) ? windowObj.document.getElementsByTagName("frame")[i] : windowObj.document.getElementsByTagName("iframe")[i];
                    if (currentFrame.id == frameName || currentFrame.name == frameName)
                    {
                        result.push(currentFrame.contentWindow);
                    }
                    else
                    {
                        if (currentFrame.contentWindow.frames.length > 0)
                        {
                            var nextWindows = Ektron.Workarea.Overlay.findFrame(frameName, currentFrame.contentWindow);
                            for(var x=0;x < nextWindows.length; x++)
                            {
                                result.push(nextWindows[x]);
                            }
                        }
                    }
                }
            }
            return result;
        },

        // removes the block layers
        unblock: function()
        {
            $ektron(document).trigger("ektronOverlayUnblock");
        }
    }
}

//define Ektron.Workarea.Grids object only if it's not already defined
if (typeof(Ektron.Workarea.Grids) == "undefined")
{
    Ektron.Workarea.Grids =
    {
        init: function() {
            if (typeof($ektron) != "undefined")
            {
                Ektron.ready(function()
                {
                    Ektron.Workarea.Grids.show();
                });
            }
        },
        show: function() {
            //stripe rows
            var ektronGrid = $ektron("table.ektronForm, table.ektronGrid");
            if (ektronGrid.length > 0)
            {
                ektronGrid.each(function()
                {
                   $ektron(this).find(">tbody>tr:not([class*='title-header'][class*='skipStripe']):odd").addClass("stripe");
                });
                /* 
                need to explicitly hide in case CSS display:none has not been applied yet, in which case,
                fadeIn will do nothing and later when the CSS is applied, the display:none will take precedence.
                */
                ektronGrid.hide(); 
                ektronGrid.fadeIn("fast");
            }
        }
    }
}

//define Ektron.Workarea.Tabs object only if it's not already defined
if (typeof(Ektron.Workarea.Tabs) == "undefined")
{
	Ektron.Workarea.Tabs =
	{
	    init: function()
	    {
	        //initialize tabs when ready
	        if ("undefined" != typeof $ektron)
	        {
	            Ektron.ready(function()
	            {
	                Ektron.Workarea.Tabs.show();
	            });
            }
	    },
	    setWidth: function()
	    {
	        var tabContainer = $ektron(".tabContainer");

            var elem = tabContainer.find(".ui-tabs-panel");

            var width = 0;
            var wrapperWidth = 0;
            var buffer = $ektron.browser.msie ? 2 : 1; //different rounding for IE and FF
            var items = $ektron(".tabContainer .ui-tabs-nav li");
            items.each(function(i)
            {
                width = width + $ektron(this).outerWidth(true) + buffer;
            });
			if (!($ektron(".ektronTabBackground").size() > 0)) {
            	tabContainer.children("ul.ui-tabs-nav").after('<div class="ektronTabBackground"></div>');
            	tabContainer.children("div.ektronTabBackground").after('<div class="clearfix" style="height:1px;"></div>');
            	tabContainer.children("div.ektronTabBackground").show();
			}
            tabContainer.children("ul.ui-tabs-nav")
                .css("border-right", "none")
                .css("width", width)
                .css("display", "none")
                .css("visibility", "visible")
                .show();

            var originalWidth = width;
            function processOutermostTables(elem)
            {
                $ektron(elem).children().each(function()
                {
                    if ("TABLE" === this.tagName)
                    {
                        var eTable = $ektron(this);
                        var eParents = eTable.parents(".ui-tabs-panel");
                        var tabsPadding = eParents.outerWidth(true) - eParents.width();
                        var tableWidth = eTable.outerWidth(true);
                        var wrapperWidth = tableWidth + tabsPadding;
                        if (wrapperWidth > width)
                        {
                            width = wrapperWidth;
                        }
                    }
                    else
                    {
                        processOutermostTables(this);
                    }
                });
            }
            elem.each(function()
            {
                processOutermostTables(this);
            });

            if (width == originalWidth)
            {
                var tabsPanel = $ektron(".ui-tabs-panel");
                width = width + (tabsPanel.outerWidth(true) - tabsPanel.width());
            }

            //apply width to containing div to ensure min-width of content
            var minWidth;
            //tab wrapper
            minWidth = "min-width:" + width + "px;width: auto !important;width:" + width + "px;";
            $ektron(".tabContainer").parents("div.tabContainerWrapper").attr("style", minWidth);

            //tab background
            minWidth = "display:block;"
            $ektron(".tabContainer").children("div.ektronTabBackground").attr("style", minWidth);
	    },
        show: function()
        {
            //re-initialize tabs
            var tabsContainers = $ektron(".tabContainer");
            if (tabsContainers.length > 0) {
                tabsContainers.tabs();
                Ektron.Workarea.Tabs.setWidth();
            }
        }
	}
}

//initializers
Ektron.Workarea.Grids.init();
Ektron.Workarea.Tabs.init();


$ektron(document).ready(function() {
	
	//tooltip plugin
    (function($){  
     $.fn.tooltip = function(options) {  
      	
		var defaults = {  
		   offset: [0,0],  
		   classNameBase: "tooltip",
		   srcAttrib : "title"
		};  
			
		var options = $.extend(defaults, options);
		var arrow_class = options.classNameBase + "-arrow";
		
        return this.each(function() {  
			var button = $(this);
			
			if (!(button.attr(options.srcAttrib).length > 0)) { return; }
			
			//keep browser tooltip from appearing on links
			if (options.srcAttrib == "title") {
				button.attr({"tt" : button.attr("title")});
				button.removeAttr("title");
				options.srcAttrib = "tt";
			}
			
			button.hover(function() {
				$("." + options.classNameBase + ", ."+arrow_class).remove();
			
				var tooltip = document.createElement("div");
				$("body").prepend(tooltip);
				tooltip = $(tooltip);
				tooltip.html(button.attr(options.srcAttrib)).addClass(options.classNameBase);
				
				var xpos = button.offset().left - ((tooltip.outerWidth() - button.outerWidth()) / 2) + options.offset[0];
				var ypos = button.offset().top - tooltip.outerHeight() + options.offset[1];
				var xlimit = $(window).width() - tooltip.outerWidth();
				var ylimit = button.offset().top - tooltip.outerHeight();
				
				if (xpos < 0) {
					xpos = 0;
				} else {
					if (xpos > xlimit) {
						xpos = xlimit;
					}
				}
				if (ypos < 0) {
					ypos = 0;
				} else {
					if (ypos > ylimit) {
						ypos = ylimit;
					}
				}
				tooltip.css({
					"left" : Math.floor(xpos) + "px",
					"top" : Math.floor(ypos) + "px"
				});

				var arrow = document.createElement("div");
				$("body").prepend(arrow);
				arrow = $(arrow);
				arrow.addClass(arrow_class);
				
				var xpos_arrow = button.offset().left + ((button.outerWidth() - arrow.outerWidth()) / 2) + options.offset[0];
				var ypos_arrow = ypos + tooltip.outerHeight();
				
				arrow.css({
					"left": Math.floor(xpos_arrow) + "px",
					"top": Math.floor(ypos_arrow) + "px"
				});
				
			}, function() {
				$("." + options.classNameBase + ", ." + arrow_class).remove();
			});
        });  
     };  
    })(jQuery);
	
	//underline rollover plugin
	(function($){  
     $.fn.underlineRollover = function(options) {  
	 
	 	var defaults = {  
		   "maxWidth" : 24,
		   "className" : "action-bar-rollover-indicator",
		   "containerSelector" : ".ektronToolbar, .couponList ul"
		};  
			
		var options = $.extend(defaults, options);
		
        return this.each(function() {
			var button = $(this);
			button.hover(function() {
				var container = button.closest(options.containerSelector);
				container = container.first();
				if (container.size() == 0) { return; }
				button.rollover = document.createElement("div");
				container.append(button.rollover);
				button.rollover = $(button.rollover);
				button.rollover.addClass(options.className);
				var lineWidth = button.outerWidth();
				if (lineWidth > options.maxWidth) {
					lineWidth = options.maxWidth;
				}
				var xpos = button.position().left;
				var leftmargin = Number(button.css("margin-left").replace("px",""));
				if (leftmargin > 0) {
					xpos += leftmargin;
				}
				button.rollover.css({
					"width":+lineWidth + "px",
					"left":+xpos + "px"
				});
			}, function() {
				if (button.rollover) {
					button.rollover.remove();
				}
			});
		});  
 	 };
    })(jQuery);

	//give applicable icons' ancestor td a title attribute so the tooltip plugin works correctly with them
	$ektron(".ektronToolbar [title] img[title], .ektronToolbar [title] a[title], .ektronPageHeader [title] img[title], .ektronPageHeader [title] a[title]").each(function() {
		var button = $ektron(this);
		var ancestor = button.parents().closest("td");
		if (ancestor.size() == 0) {
			ancestor = button.parents().closest("a");
		}
		if (ancestor.size() > 0) {
			ancestor.attr({"title" : button.attr("title")});
			ancestor.find("[title]").removeAttr("title");
		}
	});
	
	//tooltip and rollover underline init
	var menuItems = $ektron(".ektronToolbar :not(span,select)[title]:not([type=text]), .couponList [title], .baseClassToolbar .ektronTitlebar [title], #aHelp[title]").each(function() {
		if ($ektron(this).hasClass("primary") || (!isBackButton($ektron(this)) && $ektron(this).children().hasClass("primary"))) {
			return;
		}
		if ($ektron(this).closest(".ektronFormWizardToolbar").size() > 0 ) {
			if ($ektron(this).attr("id") !== "DeskTopHelp") {
				return;
			}
		}
		var tt_offset = [1, -4];
		if ($ektron(this).attr("id") == "DeskTopHelp") {
			tt_offset = [1, -8];
		}

        if (!($ektron(this).closest("#pnlwizard").size() > 0) || ($ektron(this).attr("id") == "DeskTopHelp")) {
            $ektron(this).tooltip({
                "offset": tt_offset,
                "classNameBase": "action-bar-tooltip"
            });
        }
		if (!($ektron(this).closest(".helpassets, #pnlwizard").size() > 0) && !isBackButton($ektron(this))) {
			$ektron(this).underlineRollover();
		}
	});
	
	function isBackButton(obj) {
		return (obj.hasClass("backButton") || obj.hasClass("cancelButton") ||
		obj.children().hasClass("backButton") || obj.children().hasClass("cancelButton"));
	}
	
	//when title bar and action bar are not wrapped in div.ektronPageHeader, in settings->roles->built-in
	$ektron(".ektronTitlebar").each(function() {
		var titleBar = $ektron(this);
		if (titleBar.parents().closest(".ektronPageHeader").size() == 0 && !(titleBar.parents().closest("#MetaSelectContainer, .ektronPageGrid.attachment-table").size() > 0)) {
			var ektronPageHeader = document.createElement("div");
			titleBar.after(ektronPageHeader);
			ektronPageHeader = $ektron(ektronPageHeader);
			ektronPageHeader.addClass("ektronPageHeader");
			ektronPageHeader.append(titleBar);
			ektronPageHeader.append($ektron(".ektronToolbar"));
		}
	});
	
	//remove empty title bars
	$ektron(".ektronPageHeader .ektronTitlebar").each(function() {
		if ($ektron(this).html().replace(/&nbsp;/g, '').length == 0) {
			$ektron(this).unwrap();
		}
	});
    $ektron('#aHelp, #select_folder_save_btn_container td, #select_folder_save_btn_container a, #select_folder_save_btn_container img,#select_folder_save_btn_container .contextualHelpButton').hover(function(){
        var buttonCheck = $ektron(this);
        var customHeight = "33px";
        if(buttonCheck.attr("id") == "aHelp"){
            customHeight = "27px";
        }
        $(".action-bar-tooltip,.action-bar-tooltip-arrow").css({
            position: 'absolute',
            top: customHeight
        });

    },function(){

    });

    $ektron('.ektronPageHeader a').hover(function(){
        MenuUtil.hide();
    },function(){

    });
	
	/*in workarea, settings > config > personalizations > targeted content > add*/
	$ektron("form[action*='TargetContentEdit.Aspx']").addClass("workarea-targeted-content");
 
});
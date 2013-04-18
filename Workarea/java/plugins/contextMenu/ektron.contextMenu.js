// Based on the jQuery Context Menu Plugin by Cory S.N. LaViska, and modified by Keith Pepin
//
// Version 1.1
//
// Cory S.N. LaViska
// A Beautiful Site (http://abeautifulsite.net/)
//
// Visit http://abeautifulsite.net/notebook/80 for usage and more information
//
// Terms of Use
//
// This software is licensed under a Creative Commons License and is copyrighted
// (C)2008 by Cory S.N. LaViska.
//
// For details, visit http://creativecommons.org/licenses/by/3.0/us/
//
// The settings object has the following properties:
//      menu:  The selector identifying the context menu.
//      hideMenuDelay: the time in milliseconds before the menu will fadeout.
//      inSpeed: The speed used when fading in the menu.  Default is 150.
//      outSpeed: The speed used when fading out the menu.  Default is 75.
//      onContextMenu(settings): The callback that is executed prior to displaying the menu.
//                      This callback will automatically be fed a settings object containing
//                      the following properties:
//                      settings = {
//                          event: the actual event triggered (typically click),
//                          menuTrigger: a jQuery object reference for the original item
//                                       right clicked to display the menu,
//                          position: {
//                              x: (x - offset.left),
//                              y: (y - offset.top),
//                              docX: x,
//                              docY: y
//                          }
//                      }

//      onItemClick(settings): The callback executed when a menu item is clicked upon.
//                      This callback will automatically be fed a settigns object containing
//                      the following properties:
//                      settings = {
//                          action: the href of the clicked menuItem,
//                          event: the actual event triggered (typically click),
//                          menuTrigger:  a jQuery object reference for the original item
//                                        right clicked to display the menu,
//                          menuItemClicked: a jQuery object reference for the menu item clicked by the user
//                          position: {
//                              x: (x - offset.left),
//                              y: (y - offset.top),
//                              docX: x,
//                              docY: y
//                          }
//                      }


(function()
{
    $.extend($.fn,
	{
	    contextMenu: function(settings)
	    {
	        // Defaults
	        var o = $.extend(
	        {
	            inSpeed: 150,
	            outSpeed: 75
	        }, settings);
	        // if o.menu isn't a string return false
	        if (!('string' === typeof (o.menu)))
	        {
	            return false;
	        }
	        // 0 needs to be -1 for expected results (no fade)
	        if (o.inSpeed === 0)
	        {
	            o.inSpeed = -1;
	        }
	        if (o.outSpeed === 0)
	        {
	            o.outSpeed = -1;
	        }
	        // Loop each context menu
	        $(this).each(function()
	        {
	            var el = $(this);
	            // Add contextMenu class
	            var contextMenu = $(o.menu);
	            if (contextMenu.length !== 1)
	            {
	                // either selector is bad, or we've got more than one in play
	                if (contextMenu.length === 0)
	                {
	                    return false;
	                }
	                contextMenu = $(contextMenu[0]);
	            }
	            // Add contextMenu class
	            contextMenu.addClass('ektronContextMenu');
	            // Simulate a true right click
	            el.mousedown(function (e) 
                {
	                $ektron('ul.ektronContextMenu').unbind('contextmenu').bind('contextmenu', false);
	                var evt = e;
	                $(this).mouseup(function(e)
	                {
	                    var srcElement = $(this);
	                    var offset = srcElement.offset();
	                    srcElement.unbind('mouseup');
	                    if (evt.button == 2)
	                    {
	                        // Hide context menus that may be showing
                            $(".ektronContextMenu").hide();

	                        if (el.hasClass('disabled'))
	                        {
	                            return false;
	                        }

	                        // Detect mouse position
	                        var d = {}, x, y;
	                        if (self.innerHeight)
	                        {
	                            // IE
	                            d.pageYOffset = self.pageYOffset;
	                            d.pageXOffset = self.pageXOffset;
	                            d.innerHeight = self.innerHeight;
	                            d.innerWidth = self.innerWidth;
	                        }
	                        else if (document.documentElement && document.documentElement.clientHeight)
	                        {
	                            // IE6 in strict mode
	                            d.pageYOffset = document.documentElement.scrollTop;
	                            d.pageXOffset = document.documentElement.scrollLeft;
	                            d.innerHeight = document.documentElement.clientHeight;
	                            d.innerWidth = document.documentElement.clientWidth;
	                        }
	                        else if (document.body)
	                        {
	                            // all others
	                            d.pageYOffset = document.body.scrollTop;
	                            d.pageXOffset = document.body.scrollLeft;
	                            d.innerHeight = document.body.clientHeight;
	                            d.innerWidth = document.body.clientWidth;
	                        }
	                        x = (e.pageX ? e.pageX : (e.clientX + d.scrollLeft));
	                        y = (e.pageY ? e.pageY : (e.clientY + d.scrollTop));

                            //adjust menu position
                            var availableHeight = $ektron("body").outerHeight();
                            var availableWidth = $ektron("body").outerWidth();
                            var menuWidth = contextMenu.outerWidth();
                            var menuHeight = contextMenu.outerHeight();
                            if (x + menuWidth > availableWidth)
                            {
                                x -= menuWidth;
                            }
                            if (y + menuHeight > availableHeight)
                            {
                                y -= menuHeight;
                            }
                            if (x < 0) x = 0;
                            if (y < 0) y = 0;

	                        // Show the menu
	                        $(document).unbind('click');
	                        contextMenu.css({ top: y, left: x }).fadeIn(o.inSpeed);

	                        // perform onContextMenu callback if present
	                        if ('function' === typeof (o.onContextMenu))
	                        {
	                            /*
	                            call the onContextMenu and pass it the information regarding
	                            the triggering element
	                            */
	                            var returnValue = o.onContextMenu(
						        {
						            event: e,
						            menuTrigger: el,
						            position: {
						                x: (x - offset.left),
						                y: (y - offset.top),
						                docX: x,
						                docY: y
						            }
						        });
	                            if (returnValue === false)
	                            {
	                                return false;
	                            }
	                        }

	                        // Hover events
	                        contextMenu.find('a').mouseover(function()
	                        {
	                            contextMenu.find('li.hover').removeClass('hover');
	                            $(this).parent().addClass('hover');
	                        }).mouseout(function()
	                        {
	                            contextMenu.find('li.hover').removeClass('hover');
	                        });

	                        // Keyboard
	                        $(document).keyup(function(e)
	                        {
	                            key = e.charCode || e.keyCode || 0;
	                            switch (key)
	                            {
	                                case 38: // up
	                                    if (contextMenu.find('li.hover').length === 0)
	                                    {
	                                        contextMenu.find('li:last').addClass('hover');
	                                    }
	                                    else
	                                    {
	                                        contextMenu.find('li.hover').removeClass('hover').prevAll('li:not(.disabled)').eq(0).addClass('hover');
	                                        if (contextMenu.find('li.hover').length === 0)
	                                        {
	                                            contextMenu.find('li:last').addClass('hover');
	                                        }
	                                    }
	                                    break;
	                                case 40: // down
	                                    if (contextMenu.find('li.hover').length === 0)
	                                    {
	                                        contextMenu.find('li:first').addClass('hover');
	                                    }
	                                    else
	                                    {
	                                        contextMenu.find('li.hover').removeClass('hover').nextAll('li:not(.disabled)').eq(0).addClass('hover');
	                                        if (contextMenu.find('li.hover').length === 0)
	                                        {
	                                            contextMenu.find('li:first').addClass('hover');
	                                        }
	                                    }
	                                    break;
	                                case 13: // enter
	                                    contextMenu.find('li.hover a').trigger('click');
	                                    break;
	                                case 27: // esc
	                                    $(document).trigger('click');
	                                    break;
	                            }
	                        });

	                        // When items are selected
	                        contextMenu.find('a').unbind('click');
	                        contextMenu.find('li a').click(function()
	                        {
	                            var thisLink = $(this);
	                            if (thisLink.parent().is(":not(.disabled)"))
	                            {
	                                $(document).unbind('click').unbind('keypress');
	                                $(".ektronContextMenu").hide();
	                                // onItemClick Callback
	                                if ('function' == typeof (o.onItemClick))
	                                {
	                                    var actionString = thisLink.attr('href') || "";
	                                    if (actionString.length > 0)
	                                    {
	                                        var aryMatch = actionString.match(/#([^\?]+)/);
	                                        if (aryMatch.length > 0)
	                                        {
	                                            actionString = aryMatch[1];
	                                        }
	                                        else
	                                        {
	                                            actionString = "";
	                                        }
	                                    }
	                                    var returnValue = o.onItemClick(
								        {
								            action: actionString,
								            event: e,
								            menuTrigger: srcElement,
								            menuItemClicked: thisLink,
								            position: {
								                x: x - offset.left,
								                y: y - offset.top,
								                docX: x,
								                docY: y
								            }
								        });
	                                    if (returnValue === false)
	                                    {
	                                        return false;
	                                    }
	                                }
	                            }
	                            return false;
	                        });

	                        // Hide bindings
	                        // hide if the user clicks somewhere else in the document
	                        setTimeout(function()
	                        {
	                            // Delay for Mozilla
	                            $(document).click(function()
	                            {
	                                $(document).unbind('click').unbind('keypress');
	                                contextMenu.fadeOut(o.outSpeed);
	                                return false;
	                            });
	                        }, 0);
	                        // have we set a hideMenuDelay?  If so, enable it.
	                        if ('number' == typeof (o.hideMenuDelay))
	                        {
	                            //var thisMenu = contextMenu;
	                            // hide on mouseleave if they don't mouseneter within the specified delay.
	                            contextMenu.mouseleave(function()
	                            {
	                                var mouseOutTimer = setTimeout(function()
	                                {
	                                    $(document).unbind('click').unbind('keypress');
	                                    contextMenu.fadeOut(o.outSpeed);
	                                    $(document).trigger("contextMenuHide");
	                                }, o.hideMenuDelay);
	                                contextMenu.mouseenter(function()
	                                {
	                                    clearTimeout(mouseOutTimer);
	                                });
	                            });

	                            // hide the menu if the user doesn't mouseenter within the delay
	                            var initialHide = setTimeout(function()
	                            {
	                                $(document).unbind('click').unbind('keypress').trigger("contextMenuHide");
	                                contextMenu.fadeOut(o.outSpeed);
	                                $(document).trigger("contextMenuHide");
	                            }, o.hideMenuDelay);
	                            contextMenu.mouseenter(function()
	                            {
	                                clearTimeout(initialHide);
	                            });
	                        }
	                    }
	                });
	            });

	            // Disable text selection
	            if ($.browser.mozilla)
	            {
	                contextMenu.css({ 'MozUserSelect': 'none' });
	            }
	            else if ($.browser.msie)
	            {
	                contextMenu.bind('selectstart.disableTextSelect', function ()
	                {
	                    return false;
	                });
	            }
	            else
	            {
	                contextMenu.bind('mousedown.disableTextSelect', function ()
	                {
	                    return false;
	                });
	            }
	            // Disable browser context menu (requires both selectors to work in IE/Safari + FF/Chrome)
	            el.bind('contextmenu', function(e)
	            {
	                return false;
	            });
            });
	        return $(this);
	    },

	    // Disable context menu items on the fly
	    disableContextMenuItems: function(o)
	    {
	        var el = $(this);
	        if (o === undefined)
	        {
	            // Disable all
	            el.find('li').addClass('disabled');
	        }
	        else
	        {
	            var d = o.split(',');
	            for (var i = 0; i < d.length; i++)
	            {
	                el.each(function ()
	                {
	                    $(this).find('A[href="' + d[i] + '"]').parent().addClass('disabled');
	                });
	            }
	        }
	        return el;
	    },

	    // Enable context menu items on the fly
	    enableContextMenuItems: function(o)
	    {
	        var el = $(this);
	        if (o === undefined)
	        {
	            // Enable all
	            el.find('li.disabled').removeClass('disabled');
	        }
            else
	        {
	            var d = o.split(',');
	            for (var i = 0; i < d.length; i++)
	            {
    	            el.each(function()
	                {
	                    $(this).find('a[href="' + d[i] + '"]').parent().removeClass('disabled');
	                });
	            }
	        }
	        return el;
	    },

	    // Disable context menu(s)
	    disableContextMenu: function () 
        {
	        var el = $(this);
	        el.find("li").addClass('disabled');
	        return el;
	    },

	    // Enable context menu(s)
	    enableContextMenu: function()
	    {
	        var el = $(this);
	        el.find("li").removeClass('disabled');
	        return el;
	    },

	    // Destroy context menu(s)
	    destroyContextMenu: function()
	    {
	        var el = $(this);
	        // Disable action
	        el.unbind('mousedown mouseup');
	        return el;
	    }
	});

    // prevent contextmenu event of the browser
    $ektron('.ektronContextMenu').bind('contextmenu', function(e)
    {
        return false;
    });
})($ektron);

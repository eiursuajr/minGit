Ektron.ready(function()
{
    if ("undefined" === typeof Ektron.ContextMenus)
    {
        // context menu code
        Ektron.ContextMenus = {};
        Ektron.ContextMenus.AppPath = '';  // this variable value must be provided prior to calling the Init method.

        Ektron.ContextMenus.AdjustMenuPosition = function(menu, isContentMenu)
        {
            var menuPosition = menu.offset();
            var menuWidth = menu.outerWidth();
            var menuHeight = menu.outerHeight();
            var availableHeight = $ektron("body").outerHeight();
            var availableWidth = $ektron("body").outerWidth();
            
            if(isContentMenu)
            {
                if (menuPosition.left + menuWidth > availableWidth)
                {
                    menuPosition.left -= menuWidth;
                }
            }
            else
            {
                if (menuPosition.top + menuHeight > availableHeight)
                {
                    menuPosition.top -= menuHeight;
                }
            }
            if(!(menuPosition.left <= 0 || menuPosition.top <= 0))
            {
                menu.css({top: menuPosition.top, left: menuPosition.left});
            }
        };

        Ektron.ContextMenus.GetPermissions = function(settings)
        {
            var s = {
                context: null,
                dataParams: "",
                onSuccess: null,
                onError: null
            };
            $ektron.extend(s, settings);

            // set up a default permissions object
            var permissionsObj = new Ektron.Permissions();

            // check the permissions
            $ektron.ajax(
            {
                type: "Get",
                url: Ektron.ContextMenus.AppPath + "/controls/permission/permissionsCheckHandler.ashx",
                data: s.dataParams,
                dataType: "json",
                success: function(data)
                {
                    if (s.onSuccess !== null)
                    {
                        s.onSuccess(data, s.context);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    if (s.onError !== null)
                    {
                        s.onError(XMLHttpRequest, textStatus, errorThrown, s.context);
                    }
                }
            });
            return permissionsObj;
        };

        Ektron.ContextMenus.HighlightAndTextSwap = function(el, context)
        {
            //remove old highlights
            $ektron(".contextMenuHighlight").removeClass("contextMenuHighlight");
            // highlight this item
            el.addClass("contextMenuHighlight");
            context.Menu.find("span.triggerName").each(function(i)
            {
                $(this).html(context.Text);
            });
        };

        Ektron.ContextMenus.RedirectRightPane = function(url)
        {
            var rightFrame = $ektron(top.document).find("#BottomFrameSet").find("#BottomRightFrame").find("#ek_main");
            rightFrame[0].src = Ektron.ContextMenus.AppPath + url;
        };

        Ektron.ContextMenus.ShowSeparator = function(menu)
        {
            var separators = menu.find(".separator");
            separators.each(function(i)
            {
                var currentSeparator = $ektron(this);
                var nextEnabledCommands = (currentSeparator.nextAll("li:not('.disabled')").length > 0) ? true : false;
                var previousVisibleSeparators = (currentSeparator.prevAll("li.separator:visible").length > 0) ? true : false;
                var isFirstSeparator = Boolean(i==0);

                if (nextEnabledCommands && (previousVisibleSeparators || isFirstSeparator) )
                {
                    currentSeparator.show();
                }
            });
        };
    }
});
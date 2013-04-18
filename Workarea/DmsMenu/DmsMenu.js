if("undefined" === typeof Ektron)
{
    var Ektron = window.Ektron = {};
}

if ("undefined" === typeof Ektron.DMSMenu)
{
    Ektron.DMSMenu =
    {
        /* Properties
        ----------------------------------*/
        contentInfo: {
            id: "",
            parentId: "",
            languageId: "",
            guid: "",
            status: "",
            contentType: "",
            communityDocumentsMenu: "",
            dmsSubtype: ""
        },
        dynamicContentBox: true,
        ektronControlId: "",
        menuAppPath: "",
        mouseTimeOut: 0,
        menuInitialized: false,
        poundSymbolFilename: "",
        andSymbolFilename:"Cannot view or edit files with &,+ and % characters in the file name or folder path.",
        browseris:null,
        /* Objects
        ----------------------------------*/
        ItemWrapper:
        {
            /* Item Wrapper Methods */
            BindToggle: function(itemWrappers)
            {
                // bind dsmItemWrapper toggle events
                itemWrappers.live("click", function(e)
                {
                    // make sure the click was a left click
                    var button;
                    if (e.which == null)
                    {
                        // browser doesn't support event.which (IE)
                        button = (e.button < 2) ? "left" : ((e.button == 4) ? "middle" : "right");

                    }
                    else
                    {
                        /* All other browsers */
                        button = (e.which < 2) ? "left" : ((e.which == 2) ? "middle" : "right");

                    }

                    if (button == "left")
                    {
                        var isActive = $ektron(this).hasClass("dmsItemWrapperMenuActive");
                        if (isActive)
                        {
                            // deactivate this Item Wrapper and Menu
                            $ektron(this).removeClass("dmsItemWrapperMenuActive");
                            // hide the menu
                            Ektron.DMSMenu.HideMenu();
                        }
                        else
                        {
                            // deactivate the previously active item wrapper
                            $ektron(".dmsItemWrapperMenuActive").removeClass("dmsItemWrapperMenuActive");
                            var thisWrapper = $ektron(this);
                            // add active class to this item wrapper
                            thisWrapper.addClass("dmsItemWrapperMenuActive");
                            // get the DMS Menu for this item
                            Ektron.DMSMenu.FetchMenu(thisWrapper);
                        }
                    }
                });
            },

            Init: function(itemWrappers)
            {
                Ektron.DMSMenu.ItemWrapper.BindToggle(itemWrappers);
                browseris=new Ektron.DMSMenu.Browseris();
            }
        },

        MenuWrapper:
        {
            BindHover: function(menu)
            {
                // bind menu hover effects
                menu.hover(
                // mouseover
                    function()
                    {
                        // kill the hide timeout caused by the mouseout on the ItemWrapper
                        clearTimeout(Ektron.DMSMenu.mouseTimeOut);

                        var itemWrapper = $ektron(".dmsItemWrapperMenuActive");
                        var dmsWrapper = itemWrapper.parent();

                        // make the ItemWrapper appear in "hover" mode
                        dmsWrapper.addClass("dmsWrapperHover");
                    },
                // mouseout
                    function()
                    {
                        var itemWrapper = $ektron(".dmsItemWrapperMenuActive");
                        var dmsWrapper = itemWrapper.parent();

                        // remove the dmsWrapper "hover" state
                        dmsWrapper.removeClass("dmsWrapperHover");

                        // hide the Menu
                        Ektron.DMSMenu.HideMenu();

                    }
                );
            },

            FixListMenuWidth: function(menu)
            {
                var menuListItems = menu.find("li[class!=sectionBreak]");
                var sectionBreaks = menu.find("li.sectionBreak");
                var menuListItemLinks = menu.find("a");
                var menuWidth = 0;
                var version = parseInt($ektron.browser.version, 10);
                menuListItems.each(function(i)
                {
                    var thisListItem = $ektron(this);

                    var selector = "a";
                    if (version < 7)
                    {
                        selector = "span";
                    }

                    var listItemWidth = parseInt(thisListItem.find(selector).outerWidth(), 10);
                    if (menuWidth < listItemWidth)
                    {
                        menuWidth = listItemWidth;

                    }
                });
                menuListItems.css("width", menuWidth + "px");
                if (version < 7)
                {
                    sectionBreaks.css("width", (menuWidth + 8) + "px");
                    menuListItemLinks.css("width", "100%");
                }
                menu.css("width", menuWidth + "px");
            },

            Init: function(menu)
            {
                Ektron.DMSMenu.MenuWrapper.BindHover(menu);
                /*
                the following if condition fixes a hover bug for the menu items in IE7,
                and a width problem in IE6
                */
                if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 8)
                {
                    Ektron.DMSMenu.MenuWrapper.FixListMenuWidth(menu);
                }
            }
        },

        Wrapper:
        {
            /* Wrapper Methods */
            BindHover: function(wrappers)
            {
                // bind dmsWrapper hover effects
                wrappers.live("mouseover", function()
                {
                    // kill the hide timeout caused by the mouseout on anything else
                    var thisWrapper = $ektron(this);
                    thisWrapper.addClass("dmsWrapperHover");
                    var itemWrapper = thisWrapper.find(".dmsItemWrapper");
                    var isActive = itemWrapper.hasClass("dmsItemWrapperMenuActive");
                    if (isActive)
                    {
                        // kill any hide timeouts
                        clearTimeout(Ektron.DMSMenu.mouseTimeOut);
                        // show the menu
                        Ektron.DMSMenu.ShowMenu();
                    }
                });
                wrappers.live("mouseout", function()
                {
                    var thisWrapper = $ektron(this);
                    thisWrapper.removeClass("dmsWrapperHover");
                    var itemWrapper = thisWrapper.find(".dmsItemWrapper");
                    var isActive = itemWrapper.hasClass("dmsItemWrapperMenuActive");
                    if (isActive)
                    {
                        clearTimeout(Ektron.DMSMenu.mouseTimeOut);
                        Ektron.DMSMenu.mouseTimeOut = setTimeout(function()
                        {
                            Ektron.DMSMenu.HideMenu();
                        }, 10);
                    }
                });
            },

            Init: function(wrappers)
            {
                Ektron.DMSMenu.Wrapper.BindHover(wrappers);
            }
        },

        WrapperLink:
        {
            BindHover: function(links)
            {
                // bind dmsWrapperLink hover effects
                links.live("mouseover", function()
                {
                    // kill the hide timeout caused by the mouseout on anything else
                    clearTimeout(Ektron.DMSMenu.mouseTimeOut);
                });
                links.live("mouseout", function()
                {
                    clearTimeout(Ektron.DMSMenu.mouseTimeOut);
                    Ektron.DMSMenu.mouseTimeOut = setTimeout(function()
                    {
                        Ektron.DMSMenu.HideMenu();
                    }, 10);
                });
            },

            Init: function(links)
            {
                Ektron.DMSMenu.WrapperLink.BindHover(links);
            }
        },

        /* Methods
        ----------------------------------*/
        CalculateMenuPosition: function()
        {
            var menuWrapper = $ektron(".dmsMenuWrapper");
            if (menuWrapper.length > 0)
            {
                var itemWrapper = $ektron(".dmsItemWrapperMenuActive");
                var dmsWrapper = itemWrapper.parent();
                var menu = $ektron(".dmsMenuWrapper");
                var availableHeight = $ektron("body").outerHeight();
                // determine correct placement of the menu
                //var menuPosition = $ektron.positionedOffset(dmsWrapper);
                menuPosition = dmsWrapper.offset();
                var menuWrapperWidth = menuWrapper.outerWidth();
                var itemWrapperWidth = itemWrapper.outerWidth();
                /*
                the next line is a fix for FF when the scrollbar appears and then disappears
                (as a result of AJAX content brought into the page for example).  It forces
                FF to recalculate the width of the itemWrappers correctly when this happens.
                */
                var bs = itemWrapper.css("width");
                if(menuPosition != null)
                    {
                            if (menuWrapperWidth > itemWrapperWidth)
                            {
                                // menu is wider than the wrapper, position it slightly right of the wrapper left edge
                                menuPosition.left += 20;
                            }
                            else
                            {
                                // menu is smaller than the wrapper, position it right
                                menuPosition.left += (itemWrapperWidth - menuWrapperWidth);
                            }

                            if (menuPosition.top + (dmsWrapper.outerHeight() - 1) + menu.outerHeight() > availableHeight)
                            {
                                // menu would appear below the window's available space,
                                // display it above the wrapper
                                menuPosition.top = menuPosition.top - menu.outerHeight() + 1;
                            }
                            else
                            {
                                // display the menu below the wrapper
                                menuPosition.top += (dmsWrapper.outerHeight() - 1);
                            }
                            menuWrapper.css({
                                "left": menuPosition.left + "px",
                                "top": menuPosition.top + "px"
                            });
                  }
            }
        },

        ConfirmDelete: function(str)
        {
            var confirmation = confirm(str);
            if (confirmation === true)
            {
                return true;
            }
            else
            {
                return false;
            }
        },

        DestroyMenu: function(leaveActive)
        {
            // remove the menu
            $ektron("ul.dmsMenuWrapper").remove();
        },

        /* This method opens Office documents in IE only */
        EditMSOfficeFile: function(checkOutUrl, fileName)
        {
            if ($ektron.browser.msie)
            {
                try
                {
                    /* Use Jquery to execute content-state action via ajax */
                    $ektron.get(checkOutUrl,
                        function(data, status)
                        {
                            if (status === "success")
                            {
                                /* attempt to change content state */
                                try
                                {
                                    var obj = null;
                                    /* if ajax call was successful, open office with document */
                                    if (fileName.indexOf("&") != -1 || fileName.indexOf("+") != -1 )
                                     {
                                            alert(Ektron.DMSMenu.andSymbolFilename);
                                            return false;
                                      }
                                    try
                                    {
                                       
                                        obj = new ActiveXObject('SharePoint.OpenDocuments.2');
                                        obj.EditDocument2(window, fileName, '');
                                    }
                                    catch (e2)
                                    {
                                        try
                                        {
                                            obj = new ActiveXObject('SharePoint.OpenDocuments.3');
                                            obj.EditDocument3(window, fileName, '');
                                        }
                                        catch (e3)
                                        {
                                            obj = new ActiveXObject('SharePoint.OpenDocuments.1');
                                            obj.EditDocument(window, fileName, '');
                                        }
                                    }

                                    //31312 - edit in office does not work first time after install
                                    Ektron.DMSMenu.RefreshPage();
                                    /* prevent href from firing - ajax attempt was ok */
                                }
                                catch (e)
                                {
                                    /* active X instantiation failed, attempt to peform the same action via href (non-ajax) */
                                    window.location = checkOutUrl + "&executeActiveX=true";
                                }
                            }
                            else
                            {
                                /* ajax request failed, attempt to peform the same action via href (non-ajax) */
                                window.location = checkOutUrl + "&executeActiveX=true";
                            }
                        });
                    return false;
                }
                catch (e)
                {
                    //alert(e.description);
                    return true;
                }
            }
        },

        FetchMenu: function(itemWrapper)
        {
            itemWrapper.addClass("dmsItemWrapperLoading");

            // destroy any previous menus
            Ektron.DMSMenu.DestroyMenu();

            /*
            if the appropriate data attributes are present assign them to the necessary variables,
            otherwise assign empty string value.
            */
            var inputContentInfo = itemWrapper.find("input[id^=dmsContentInfo]");
            var contentInfo = Ektron.DMSMenu.contentInfo;
            if (inputContentInfo.length > 0)
            {
                contentInfo = $ektron.extend(Ektron.DMSMenu.contentInfo, Ektron.JSON.parse(inputContentInfo.val()));
            }

            var contentId = contentInfo.id;
            var languageId = contentInfo.languageId;
            var menuGUID = contentInfo.guid;
            var menuType = contentInfo.communityDocumentsMenu;
            var menuSubType = contentInfo.dmsSubtype;
            var communityGroupId = contentInfo.communityGroupid;
            var pageurl = window.location.href; // pass this on so that we can handle the back button properly
            var taxonomyOverrideId = 0;

            var dmsWrapper = itemWrapper.parent();
            var dmsMenuClientId = dmsWrapper.find("input[type='hidden'][class='dmsItemClientId']");
            var ieMenu = "false";

            if (dmsMenuClientId.length > 0)
            {
                // this is a control using the DMS menu, and we need that control's ClientId for AJAX responses
                dmsMenuClientId = dmsMenuClientId.val().replace(/\$/g, "_");
            }
            else
            {
                dmsMenuClientId = Ektron.DMSMenu.ektronControlId;
            }

            /* If the browser is IE, then we can create a menu specific to IE (opens Office assets directly in Office */
            /* See the methods "EditMSOfficeFile()" and ViewMSOfficeFile() below */
            if ($ektron.browser.msie)
            {
                // Only set "ieMenu" to true if office is installed
                /*
                ShowMultipleUpload() tests to see if Office is installed
                and exists in Workarea/java/determineoffice.js
                */
                if (typeof ShowMultipleUpload != 'undefined' && ShowMultipleUpload())
                {
                    ieMenu = "true";
                }
            }
            if ($ektron.browser.mozilla) {
                var ffplugin = Ektron.DMSMenu.CreateFirefoxOnWindowsPlugin();
                if(ffplugin !=null)
                    ieMenu = "true";
            }

            if (menuType !== '')
            {
                if ($ektron("#taxonomyselectedtree").length > 0)
                {
                    taxonomyOverrideId = $ektron("#taxonomyselectedtree").attr("value");
                }
            }

            // Use JQuery to fetch the menu via Ajax
            $ektron.get(Ektron.DMSMenu.menuAppPath + "DmsMenu/DmsMenu.aspx",
                {
                    contentId: contentId,
                    createIeSpecificMenu: ieMenu,
                    communityDocuments: menuType,
                    dynamicContentBox: Ektron.DMSMenu.dynamicContentBox,
                    dmsEktControlID: dmsMenuClientId,
                    dmsLanguageId: languageId,
                    taxonomyOverrideId: taxonomyOverrideId,
                    dmsMenuGuid: menuGUID,
                    dmsMenuSubtype: menuSubType,
                    fromPage: pageurl,
                    communityGroupId: communityGroupId
                },
                function(data)
                {
                    try
                    {
                        var notLoggedInCheck = data.indexOf("-1|");
                        if (notLoggedInCheck != -1)
                        {
                            // user's not logged-in, alert user to log in.
                            alert(String(data).replace("-1|", ""));
                        }
                        else
                        {
                            // user's logged-in - process data
                            // insert hte data returned into the page
                            $ektron("body").append(data);
                            var dmsMenuWrapper = $ektron(".dmsMenuWrapper");
                            // ensure we can get/set height/width
                            dmsMenuWrapper.css("visibility", "hidden");
                            dmsMenuWrapper.css("display", "block");
                            // apply "inherited" styling so we can get accurate measurements
                            dmsMenuWrapper.css({
                                "font-family": (itemWrapper.css("font-family")) ? itemWrapper.css("font-family") : "inherit",
                                "font-weight": (itemWrapper.css("font-weight")) ? itemWrapper.css("font-weight") : "inherit",
                                "font-style": (itemWrapper.css("font-style")) ? itemWrapper.css("font-style") : "inherit",
                                "font-variant": (itemWrapper.css("font-variant")) ? itemWrapper.css("font-variant") : "inherit"
                            });
                            Ektron.DMSMenu.MenuWrapper.Init(dmsMenuWrapper);
                            // make sure the mouse is still hovering over the wrapper before showing the menu
                            if (itemWrapper.parent().hasClass("dmsWrapperHover"))
                            {
                                Ektron.DMSMenu.ShowMenu();
                            }
                        }
                    }
                    finally
                    {
                        itemWrapper.removeClass("dmsItemWrapperLoading");
                    }
                }
            );

        },

        ForceCheckIn: function(href, message)
        {
            var confirmation = confirm(message);
            if (confirmation)
            {
                Ektron.DMSMenu.MenuAction(href);
            }
        },

        HideMenu: function()
        {
            $ektron("ul.dmsMenuWrapper").css("visibility", "hidden");
        },

        Init: function()
        {
            /*
            This method initializes the elements of the page that are require DMS Menus.
            */
            if (!Ektron.DMSMenu.menuInitialized)
            {
                // find the dmsWrappers in the page
                var dmsWrappers = $ektron(".dmsWrapper");
                var dmsItemWrappers = $ektron(".dmsItemWrapper");
                var dmsWrapperLinks = $ektron(".dmsViewItemAnchor");
                Ektron.DMSMenu.Wrapper.Init(dmsWrappers);
                Ektron.DMSMenu.WrapperLink.Init(dmsWrapperLinks);
                Ektron.DMSMenu.ItemWrapper.Init(dmsItemWrappers);

                // bind resize events
                $ektron(window).bind("resize", function()
                {
                    setTimeout(function()
                    {
                        Ektron.DMSMenu.CalculateMenuPosition();
                    }, 1);
                });

                // trigger EktronDMSMenuReady event
                $ektron(document).trigger("EktronDMSMenuReady");
                Ektron.DMSMenu.menuInitialized = true;
            }
        },

        /*
        The following method executes a number of menu actions - mainly for actions that require content state change.
        Specifically, this fucntion is called by the following menu items:
        Approve, CheckIn, Decline, Delete, Publish, Submit
        */
        MenuAction: function(href)
        {
            try
            {
                /* Use Jquery to execute content-state action via ajax */
                $ektron.get(href,
                    function(data, status)
                    {
                        if (status === "success")
                        {
                            if (data.substring(0, "message:".length) == "message:")
                            {
                                alert(data.substring("message:".length));
                            } else
                            {
                                /* content state change succeeded, refresh page to show change in state */
                                Ektron.DMSMenu.RefreshPage();
                            }
                        }
                        else
                        {
                            alert("failed!");
                            /* ajax request failed, attempt to peform the same action via href (non-ajax) */
                            window.location = href;
                        }
                    }
                );
                return false;
            }
            catch (e)
            {
                //the ajax request failed, return true so that the browser attempts to perform the action via href.
                return true;
            }
        },

        ModifyImage: function(href, idThumb)
        {
            // declare required variables
            var imgtag = $ektron("#" + idThumb);
            var imgContainer = imgtag.parents("div.image");
            var oldimg = imgtag.attr("src");

            // if there are no matching elements, alert and return false
            if (imgtag.length == 0)
            {
                alert('error: missing ID for thumbnail image in ekml template');
                return false; // no thumbnail so ignore command
            }

            // indicate that we've started working by showing the loading graphic
            imgContainer.css({
                "background-image": "url(" + Ektron.DMSMenu.menuAppPath + "images/application/DMSMenu/dmsMenuAjaxLoading.gif)",
                "background-position": "center center",
                "background-repeat": "no-repeat"
            });
            imgtag.css({ "opacity": 0, "filter": "alpha(opacity=0)" });

            try
            {
                /* Use Jquery to execute content-state action via ajax */
                $ektron.get(href,
                    function(data, status)
                    {
                        if (status === "success")
                        {
                            // anything special to do?
                        }
                        else
                        {
                            /* ajax request failed, attempt to peform the same action via href (non-ajax) */
                            window.location = href;
                        }

                        var re = new RegExp("([?|&])r=.*?(&|$)", "i");  // allows us to replace key "r" values in the QueryString
                        var bigimgtag = $ektron("#" + idThumb.replace(/GalleryThumb/, "PhotoGallery"));
                        var bigImgHref = bigimgtag.attr("href");

                        // check the QueryString of the URL for the "r" key
                        if (Ektron.QueryString["r"])
                        {
                            if (oldimg.match(re))
                            {
                                oldimg = oldimg.replace(re, '$1' + "r" + "=" + Math.random() + '$2');
                            }
                            else
                            {
                                oldimg = oldimg + "&r=" + Math.random();
                            }
                            if (oldimg.match(re))
                            {
                                bigImgHref = bigImgHref.replace(re, '$1' + "r" + "=" + Math.random() + '$2');
                            }
                            else
                            {
                                bigImgHref = bigImgHref + "&r=" + Math.random();
                            }
                        }
                        else
                        {
                            oldimg += "?r=" + Math.random();
                            bigImgHref += "?r=" + Math.random();
                        }
                        /* refresh thumbnail */
                        imgtag.attr("src", oldimg).css({
                            "opacity": 1,
                            "filter": "alpha(opacity=100)"
                        });
                        // move loading image background-image out of sight
                        imgContainer.css({
                            "background-position": "center -10000px"
                        });
                        // switch over view image as well
                        bigimgtag.attr("href", bigImgHref); // replace preloaded image
                    });
                return false;
            }
            catch (e)
            {
                imgContainer.css({
                    "background-position": "center -10000px"
                });
                imgtag.css({
                    "opacity": 1,
                    "filter": "alpha(opacity=100)"
                });
                //the ajax request failed, return true so that the browser attempts to perform the action via href.
                return true;
            }
        },

        RefreshPage: function()
        {
            /*
            This function is called by menu items that need to perform server-side logic,
            which the content page needs to refesh to see.
            For example, "Check-in" executes an ajax request to check-in the content item,
            then needs to refresh the page to show the state of the content has changed from "O" to "I".
            */
            setTimeout(function()
            {
                self.location.href = self.location;
            }, 10);
        },

        /*
        This method does the same thing as MenuAction, but it opens a success/failure
        window to show indicate to the user if the request was succesfully sent or not
        */
        RequestCheckIn: function(href)
        {
            try
            {
                /* Use Jquery to execute content-state action via ajax */
                $ektron.get(href,
                    function(data, status)
                    {
                        //show response - success/failure
                        alert(data);
                    }
                );
                return false;
            }
            catch (e)
            {
                //the ajax request failed, return true so that the browser attempts to perform the action via href.
                return true;
            }
        },

        ShowMenu: function()
        {
            // makes sure the menu is positioned correctly
            Ektron.DMSMenu.CalculateMenuPosition();
            $ektron("ul.dmsMenuWrapper").css("visibility", "visible");
        },

        Sync: function(settings)
        {
            /*  possible settings parameters:
            {
            contentLanguage: indicates the content language for this content
            contentId: the id of the cotnent item
            contentAssetId: the content asset id number (if applicable)
            contentAssetVersion: the content asset version number (if applicable)
            folderId: the parent folder id  number
            dmsSyncPath: the full path to the dmsSync.aspx necessary when the modals are not present
            }
            */
            var s = settings;
            // check for the presence of the necessary SyncConfigModal
            if ($ektron("#ShowSyncConfigModal").length > 0)
            {
                // the modal is present, so we can do the sync in this window
                Ektron.Workarea.Sync.Relationships.ShowSyncConfigurations(s.contentLanguage, s.contentId, s.contentAssetId, s.contentAssetVersion, s.folderId, s.isMultisite);
            }
            else
            {
                // popup a new window to perform the sync
                dmsSyncWindow = window.open(s.dmsSyncPath + '?contentLanguage=' + s.contentLanguage + '&contentId=' + s.contentId + '&contentAssetId=' + s.contentAssetId + '&contentAssetVersion=' + s.contentAssetVersion + '&folderId=' + s.folderId + '&isMultiSite=' + s.isMultisite, 'dmsSync', 'resizable=no,scrollbars=no,toolbar=no,status=no,menubar=no,location=no,height=1,width=1');
                dmsSyncWindow.focus();
            }
            //return false to prevent click through
            return false;
        },

        ViewMSOfficeFile: function(fileName)
        {
            if ($ektron.browser.msie)
            {
                if (fileName.indexOf("&") != -1 || fileName.indexOf("+") != -1 )
                {
                    alert(Ektron.DMSMenu.andSymbolFilename);
                    return false;
                }
                var obj = new ActiveXObject('SharePoint.OpenDocuments.2');
                obj.ViewDocument2(window, fileName, '');
                return false;
            }
        },

        Browseris: function () {
            var a = navigator.userAgent.toLowerCase();
            this.osver = 1;
            if (a) {
                var g = a.substring(a.indexOf("windows ") + 11);
                this.osver = parseFloat(g)
            }
            this.major = parseInt(navigator.appVersion);
            this.nav = a.indexOf("mozilla") != -1 && (a.indexOf("spoofer") == -1 && a.indexOf("compatible") == -1);
            this.nav6 = this.nav && this.major == 5;
            this.nav6up = this.nav && this.major >= 5;
            this.nav7up = false;
            if (this.nav6up) {
                var b = a.indexOf("netscape/");
                if (b >= 0) this.nav7up = parseInt(a.substring(b + 9)) >= 7
            }
            this.ie = a.indexOf("msie") != -1;
            this.aol = this.ie && a.indexOf(" aol ") != -1;
            if (this.ie) {
                var e = a.substring(a.indexOf("msie ") + 5);
                this.iever = parseInt(e);
                this.verIEFull = parseFloat(e)
            } else this.iever = 0;
            this.ie4up = this.ie && this.major >= 4;
            this.ie5up = this.ie && this.iever >= 5;
            this.ie55up = this.ie && this.verIEFull >= 5.5;
            this.ie6up = this.ie && this.iever >= 6;
            this.ie7down = this.ie && this.iever <= 7;
            this.ie7up = this.ie && this.iever >= 7;
            this.ie8standard = this.ie && document.documentMode && document.documentMode == 8;
            this.winnt = a.indexOf("winnt") != -1 || a.indexOf("windows nt") != -1;
            this.win32 = this.major >= 4 && navigator.platform == "Win32" || a.indexOf("win32") != -1 || a.indexOf("32bit") != -1;
            this.win64bit = a.indexOf("win64") != -1;
            this.win = this.winnt || this.win32 || this.win64bit;
            this.mac = a.indexOf("mac") != -1;
            this.w3c = this.nav6up;
            this.safari = a.indexOf("webkit") != -1;
            this.safari125up = false;
            this.safari3up = false;
            if (this.safari && this.major >= 5) {
                var b = a.indexOf("webkit/");
                if (b >= 0) this.safari125up = parseInt(a.substring(b + 7)) >= 125;
                var f = a.indexOf("version/");
                if (f >= 0) this.safari3up = parseInt(a.substring(f + 8)) >= 3
            }
            this.firefox = this.nav && a.indexOf("firefox") != -1;
            this.firefox3up = false;
            this.firefox36up = false;
            if (this.firefox && this.major >= 5) {
                var d = a.indexOf("firefox/");
                if (d >= 0) {
                    var c = a.substring(d + 8);
                    this.firefox3up = parseInt(c) >= 3;
                    this.firefox36up = parseFloat(c) >= 3.6
                }
            }
        },


        IsSupportedFirefoxOnWin: function () {
            return (browseris.winnt || browseris.win32 || browseris.win64bit) && browseris.firefox3up
        },
        IsFirefoxOnWindowsPluginInstalled: function () {

            return navigator.mimeTypes && navigator.mimeTypes["application/x-sharepoint"] && navigator.mimeTypes["application/x-sharepoint"].enabledPlugin
        },
        CreateFirefoxOnWindowsPlugin: function () {

            var b = null;
            if (Ektron.DMSMenu.IsSupportedFirefoxOnWin()) try {
                b = document.getElementById("winFirefoxPlugin");
                if (!b && Ektron.DMSMenu.IsFirefoxOnWindowsPluginInstalled()) {
                    var a = document.createElement("object");
                    a.id = "winFirefoxPlugin";
                    a.type = "application/x-sharepoint";
                    a.width = 0;
                    a.height = 0;
                    a.style.setProperty("visibility", "hidden", "");
                    document.body.appendChild(a);
                    b = document.getElementById("winFirefoxPlugin")
                }
            } catch (c) {
                b = null
            }
            return b
        },


        editDocumentWithProgIDNoUI: function (strDocument, varProgID, varEditor, bCheckout, strhttpRoot, strCheckouttolocal) {
            var objEditor;
            var fRet;
            var fUseLocalCopy = false;
            varEditor = varEditor.replace(/(?:\.\d+)$/, '');
            if (strDocument.charAt(0) == "/" || strDocument.substr(0, 3).toLowerCase() == "%2f")
                strDocument = document.location.protocol + "//" + document.location.host + strDocument;
            //var strextension = SzExtension(unescapeProperly(strDocument));

            try {
                objEditor = Ektron.DMSMenu.CreateFirefoxOnWindowsPlugin(); //StsOpenEnsureEx2(varEditor + ".3");
                if (objEditor != null) {
                    if (!objEditor.EditDocument3(window, strDocument, fUseLocalCopy, varProgID))
                        return 1;
                    return;
                }
            }
            catch (e) {
            }
            return 1;
        }

        //end of class
    };
}

Ektron.ready(function()
{
    if (typeof(Sys) !== "undefined")
    {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.DMSMenu.Init);
    }
    Ektron.DMSMenu.Init();
});
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////           This file depends on the following files:                                               /////
/////           1. /Ektron.js                                                                           /////
/////           2. /Ektron.Workarea.Js                                                                  /////
/////           3. /plugins/contextMenu/ektron.contextMenu.css                                          /////
/////           4. /plugins/contextMenu/ektron.contextMenu.js                                           /////
/////           5. Workarea/csslib/ektron.workarea.css                                                  /////
/////           6. /ektron.workarea.contextmenus.js                                                     /////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////

Ektron.ready(function()
{
    if ("undefined" === typeof Ektron.ContentContextMenu)
    {
        Ektron.ContentContextMenu = {};
        Ektron.ContentContextMenu.IsFolderAdmin = '';
        Ektron.ContentContextMenu.ConfirmDelete = '';
        Ektron.ContentContextMenu.lastSelected = null;
        Ektron.ContentContextMenu.Content =
        {
            // Properties
            contentInfo: {
                id: "",
                parentId: "",
                languageId: "",
                guid: "",
                communityDocumentsMenu: "",
                contentType: "",
                status: "",
                dmsSubtype: ""
            },
            // Properties
            cpyitem: {
                i: "",
                l: "",
                p: "",
                t:""
            },
            selectedRows: ".ektronPageContainer > table.ektronGrid tr.selected",
            dmsContentInfo: ".ektronPageContainer > table.ektronGrid tr.selected td p > input",
            
            //Check for Approved Status Item within the selected Items
            CheckUnApprovedItem : function(obj)
            {
                var unApprovedItemCount = 0;
                $ektron.each(obj, function(i)
                {
                    if(obj[i].innerHTML !== "A")
                    {
                        unApprovedItemCount = unApprovedItemCount + 1;
                    }
                });
                return unApprovedItemCount;
            },
            
            // Sets the ClipBoard for Copy option
            SetClipBoardCopyAction : function(action, context, itemWrapper)
            {
                Ektron.ContentContextMenu.SetClipBoard(action, context, itemWrapper);
                top.Ektron.Workarea.TaxonomyClipBoard.action = action;

                $ektron.each(itemWrapper, function(i)
                {
                    Ektron.ContentContextMenu.Content.contentInfo = null;
                    x = $ektron(itemWrapper[i]).attr("value");
                    Ektron.ContentContextMenu.Content.contentInfo = (x) ? $ektron.extend(Ektron.ContentContextMenu.Content.contentInfo, Ektron.JSON.parse(x)) : Ektron.ContentContextMenu.Content.contentInfo;
                    var z;
                    Ektron.ContentContextMenu.Content.cpyitem.i = Ektron.ContentContextMenu.Content.contentInfo.id;
                    Ektron.ContentContextMenu.Content.cpyitem.l = Ektron.ContentContextMenu.Content.contentInfo.languageId;
                    Ektron.ContentContextMenu.Content.cpyitem.p = Ektron.ContentContextMenu.Content.contentInfo.parentId;
                    Ektron.ContentContextMenu.Content.cpyitem.t = Ektron.ContentContextMenu.Content.contentInfo.contentType;
                    z = $ektron.extend(z, Ektron.ContentContextMenu.Content.cpyitem);
                    top.Ektron.Workarea.ClipBoard.items.push(z);
                    top.Ektron.Workarea.TaxonomyClipBoard.items.push(z);
                });
            },
            // Sets only the content with Approved States.
            SetApprovedClipBoardCopyAction : function(action, context, itemWrapper)
            {
                top.Ektron.Workarea.TaxonomyClipBoard.action = action;
                top.Ektron.Workarea.ClipBoard.action = action;
                
                Ektron.ContentContextMenu.Content.contentInfo = null;
                x = itemWrapper;
                Ektron.ContentContextMenu.Content.contentInfo = (x) ? $ektron.extend(Ektron.ContentContextMenu.Content.contentInfo, Ektron.JSON.parse(x)) : Ektron.ContentContextMenu.Content.contentInfo;
                var z;
                Ektron.ContentContextMenu.Content.cpyitem.i = Ektron.ContentContextMenu.Content.contentInfo.id;
                Ektron.ContentContextMenu.Content.cpyitem.l = Ektron.ContentContextMenu.Content.contentInfo.languageId;
                Ektron.ContentContextMenu.Content.cpyitem.p = Ektron.ContentContextMenu.Content.contentInfo.parentId;
                 Ektron.ContentContextMenu.Content.cpyitem.t = Ektron.ContentContextMenu.Content.contentInfo.contentType;
                z = $ektron.extend(z, Ektron.ContentContextMenu.Content.cpyitem);
                top.Ektron.Workarea.TaxonomyClipBoard.items.push(z);
                top.Ektron.Workarea.ClipBoard.items.push(z);
            },
            
            // Classes
            EnableCommands: function(permissions, menu)
            {
                menu.disableContextMenu();
                menu.find("a").attr("data-ektron-contentinfo", permissions.id);
                folderPermissions = permissions.Folder;
                roleMemberships = permissions.RoleMemberships;
                enableList = [];
                
                var canMoveCopyDelete = Ektron.ContentContextMenu.GetDeleteContentContextProperties();
                if (Ektron.ContentContextMenu.IsFolderAdmin === "False" || Ektron.ContentContextMenu.IsFolderAdmin === false)
                {
                    Ektron.ContentContextMenu.IsFolderAdmin = false;
                }
                else
                {
                    Ektron.ContentContextMenu.IsFolderAdmin = true;
                }
                if ((permissions.Folder.IsAdmin || permissions.IsARoleMemberForFolder_FolderUserAdmin || roleMemberships.MoveOrCopy) && (folderPermissions.CanAdd || folderPermissions.CanEdit))
                {
                    enableList.push("#copyContent");
                    enableList.push("#cutContent");
                }
                
                if(folderPermissions.CanDelete && (canMoveCopyDelete.status != "O" && canMoveCopyDelete.status != "S"))
                {
                    //enable Delete Content action item
                    enableList.push("#deleteContent");
                }
                
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContentContextMenu.InitializeContentMenu(Ektron.ContentContextMenu.Content.selectedRows, "#contentContextMenu");
            }
        };

        Ektron.ContentContextMenu.GetDeleteContentContextProperties = function()
        {
            var contextData = {
                id: "",
                parentId: "",
                languageId: "",
                guid: "",
                communityDocumentsMenu: "",
                contentType: "",
                status: "",
                dmsSubtype: ""
            };
            var returnValue = true;
            var selectedFirstRow;
            var itemWrapper = $ektron(Ektron.ContentContextMenu.Content.dmsContentInfo);
            
            for(i = 0; i < itemWrapper.length; i++ )
            {   
                contextData = null;
                selectedFirstRow = null;
                selectedFirstRow = $ektron(itemWrapper[i]).attr("value");
                contextData = (selectedFirstRow) ? $ektron.extend(itemWrapper, Ektron.JSON.parse(selectedFirstRow)) : itemWrapper;
                
                if( contextData.status == "O" || contextData.status == "S" )
                {   
                    returnValue = false;
                    break;                    
                }
            }
            return contextData;
        };
        
        Ektron.ContentContextMenu.GetContentContextProperties = function(el, menu)
        {
            var contextData = {
                id: "",
                parentId: "",
                languageId: "",
                guid: "",
                communityDocumentsMenu: "",
                contentType: "",
                status: "",
                dmsSubtype: ""
            };
            var selectedFirstRow;
            var itemWrapper = $ektron(Ektron.ContentContextMenu.Content.dmsContentInfo);

            selectedFirstRow = $ektron(itemWrapper[0]).attr("value");
            contextData = (selectedFirstRow) ? $ektron.extend(itemWrapper, Ektron.JSON.parse(selectedFirstRow)) : itemWrapper;
            contextData.Menu = $ektron(menu);
            return contextData;
        };

        Ektron.ContentContextMenu.Overlay = function()
        {
            Ektron.Workarea.Overlay.block(
            {
                baseZ : 10000,
                target: {
                    "ek_main" : true,
                    "nav_toolbar" : false,
                    "ContentTree" : false
                }
            });
        };

        Ektron.ContentContextMenu.Unblock = function()
        {
            Ektron.Workarea.Overlay.unblock();
        };

        Ektron.ContentContextMenu.HighlightAndTextSwap = function(el, context)
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

        Ektron.ContentContextMenu.Init = function ()
        {
            // initialize
            Ektron.ContentContextMenu.Content.Init();

            // remove the highlight class when contextMenuHide event fires
            $ektron(document).bind("contextMenuHide", function()
            {
                $ektron(document).find(".contextMenuHighlight").removeClass("contextMenuHighlight");
            });
        };

        Ektron.ContentContextMenu.InitializeContentMenu = function(menuTarget, menuId)
        {
            var targetLinks = $ektron(menuTarget + "");
            targetLinks.contextMenu(
            {
                menu: menuId,
                hideMenuDelay: 1000,
                inSpeed: 400,
                outSpeed: 100,
                onContextMenu: function(s)
                {
                    var thisMenu = $ektron(this.menu);
                    thisMenu.disableContextMenu();
                    var context = Ektron.ContentContextMenu.GetContentContextProperties(s, this.menu);
                    Ektron.ContentContextMenu.HighlightAndTextSwap(s.menuTrigger, context);
                    var permissionsObj = Ektron.ContextMenus.GetPermissions({
                        context: context,
                        dataParams: "checkType=Content&action=getPermissions&id=" + context.parentId,
                        onSuccess: function(data)
                        {
                            permissionsObj = $ektron.extend(permissionsObj, data);
                            Ektron.ContentContextMenu.Content.EnableCommands(permissionsObj, context.Menu);
                            Ektron.ContextMenus.AdjustMenuPosition(context.Menu, true);
                        },
                        onError: function (XMLHttpRequest, textStatus, errorThrown, myContext)
                        {
                            myContext.Menu.disableContextMenu();
                        }
                    });
                },
                onItemClick: function(s)
                {
                    // If no language is selected, throw an alert and return false.
                    if( folderjslanguage == -1 )
                    {
                        alert('A language must be selected!');
                        return false;
                    }
                    Ektron.ContentContextMenu.PerformAction(s.action, s.menuTrigger, s.menuItemClicked);
                }
            });
        };

        Ektron.ContentContextMenu.PerformAction = function(action, context, trigger)
        {
            var x;
            var itemWrapper = $ektron(Ektron.ContentContextMenu.Content.dmsContentInfo);
            var selectedItems = $ektron($ektron('#viewfolder_FolderDataGrid tr.selected a[href*=""#Status""]'));
            var unApprovedItems = Ektron.ContentContextMenu.Content.CheckUnApprovedItem(selectedItems);
            
            top.Ektron.Workarea.ClipBoard.items = [];
            top.Ektron.Workarea.TaxonomyClipBoard.items = [];
            var i = 0;
            
            switch(action)
            {
                case "cutContent":
                    setClipBoard();
                    break;
                case "copyContent":
                    setCopyClipBoard();
                    break;
                case "deleteContent":
                    if(confirm(Ektron.ContentContextMenu.ConfirmDelete))
                    {
                        Ektron.ContentContextMenu.Overlay();
                        Ektron.ContentContextMenu.SetClipBoard(action, context, itemWrapper);
                        var clipBoardContext = Ektron.JSON.stringify(top.Ektron.Workarea.ClipBoard.items, null, null);
                        var href = "controls/content/cutCopyAssignHandler.ashx";
                        var deleteContentContext = {
                            clipBoardContext : clipBoardContext,
                            action: action,
                            targetid : top.Ektron.Workarea.ClipBoard.items[0].p
                        };
                        $ektron.ajax({
                            type: "Post",
                            url:href,
                            data: deleteContentContext,
                            dataType: "string",
                            success: function(url)
                            {
                                Ektron.ContentContextMenu.Unblock();
                                var redirectUrl = "/content.aspx?action=ViewContentByCategory&id=" + top.Ektron.Workarea.ClipBoard.items[0].p + "&treeViewId=0";
                                Ektron.ContextMenus.RedirectRightPane(redirectUrl);
                                top.Ektron.Workarea.ClipBoard.items = [];
                            }
                        });
                    }
                    break;
            }
        };

        Ektron.ContentContextMenu.SetClipBoard = function(action, context, itemWrapper)
        {
            top.Ektron.Workarea.TaxonomyClipBoard.items = [];
            var y;
            top.Ektron.Workarea.ClipBoard.action = action;
            $ektron.each(itemWrapper, function(i)
            {
                Ektron.ContentContextMenu.Content.contentInfo = null;
                y = $ektron(itemWrapper[i]).attr("value");
                Ektron.ContentContextMenu.Content.contentInfo = (y) ? $ektron.extend(Ektron.ContentContextMenu.Content.contentInfo, Ektron.JSON.parse(y)) : Ektron.ContentContextMenu.Content.contentInfo;
                var z;
                Ektron.ContentContextMenu.Content.cpyitem.i = Ektron.ContentContextMenu.Content.contentInfo.id;
                Ektron.ContentContextMenu.Content.cpyitem.l = Ektron.ContentContextMenu.Content.contentInfo.languageId;
                Ektron.ContentContextMenu.Content.cpyitem.p = Ektron.ContentContextMenu.Content.contentInfo.parentId;
                Ektron.ContentContextMenu.Content.cpyitem.t = Ektron.ContentContextMenu.Content.contentInfo.contentType;
                z = $ektron.extend(z, Ektron.ContentContextMenu.Content.cpyitem);
                top.Ektron.Workarea.ClipBoard.items.push(z);
                top.Ektron.Workarea.TaxonomyClipBoard.items.push(z);
                
            });
        };
        
        Ektron.ContentContextMenu.ToggleClass = function(evt)
        {
            if($ektron(evt.target.parentNode).find("div.dmsWrapper").length > 0)
            {
                var parentNode = $ektron(evt.target.parentNode);
                if (parentNode.is(".selected"))
                {
                    parentNode.unbind("contextmenu")
                        .destroyContextMenu();
                    parentNode.removeClass("selected");
                }
                else
                {
                    parentNode.addClass("selected");
                }
            }
        };
        Ektron.ContentContextMenu.SetClassSelected = function(e)
        {
            $ektron("#viewfolder_FolderDataGrid").find("tr.selected")
                .removeClass("selected")
                .unbind("contextmenu")
                .destroyContextMenu();
            $ektron("#viewfolder_FolderDataGrid").find("tr:not('.title-header')")
                .bind("mouseenter", function(e)
                {
                    var currentElement = $ektron(this);
                    if (currentElement.hasClass("selected"))
                    {
                        Ektron.ContentContextMenu.lastSelected.removeClass("selected");
                    }                    
                    $ektron(this).addClass("selected");
                    Ektron.ContentContextMenu.lastSelected = $ektron(this);
                });
        };
    }
});
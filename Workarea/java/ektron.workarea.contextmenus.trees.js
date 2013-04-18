/////////////////////////////////////////////////////////////////
/////   This file has the following dependancies:           /////
/////       1. ektron.js                                    /////
/////       2. Ektron.ContextMenus.js  [ plugin ]           /////
/////       3. ektron.workarea.contextmenus.js              /////
/////       4. ektron.workarea.css                          /////
/////       5. Ektron.ContextMenus.css  [ plugin CSS]       /////
/////////////////////////////////////////////////////////////////

Ektron.ready(function()
{
    if ("undefined" === typeof Ektron.ContextMenus.Trees)
    {
        // create the Ektron.ContextMenus.Trees namespace
        Ektron.ContextMenus.Trees = {};
        Ektron.ContextMenus.Trees.redirectUrl = "";
        
        Ektron.ContextMenus.Trees.blockframes = function()
        {
            Ektron.Workarea.Overlay.block(
            {
                target: 
                {
                    "ek_main" : true,
                    "ek_nav_bottom" : false
                }
            });
        };
        //Blocks all the frames irrespective which one is highlighted.
        Ektron.ContextMenus.Trees.blockallframes = function()
        {
            Ektron.Workarea.Overlay.block(
            {
                target:
                {
                    "ek_nav_bottom" : true
                }
            });
        };
        
        Ektron.ContextMenus.Trees.BlogFolder =
        {
            EnableCommands: function(permissions, menu)
            {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                roleMemberships = permissions.RoleMemberships;
                enableList = [];
                if(folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals)
                {
                    // enable View Properties
                    enableList.push("#viewFolderProperties");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin) && (permissions.Id !== 0))
                {
                    // enable cut copy folder options
                    enableList.push("#cutFolder");
                    enableList.push("#copyFolder");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.IsAdmin) && (top.Ektron.Workarea.FolderClipBoard.folderid != "") 
                    && (top.Ektron.Workarea.FolderClipBoard.foldertype === "1" && top.Ektron.Workarea.FolderClipBoard.foldertype === "2"))
                {
                    // enable paste folder options
                    enableList.push("#pasteFolder");
                }
                if (folderPermissions.CanDeleteFolders)
                {
                    enableList.push("#deleteFolder");
                }
                if(folderPermissions.IsAdmin || permissions.IsARoleMemberForFolder_FolderUserAdmin)
                {
                    enableList.push("#deleteFolderContent");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div.ektronTreeContainer ul.Content li a[data-ektron-datatype='1']", "#blogFolderContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.Calendar =
        {
            confirmFolderDelete: "",
            EnableCommands: function(permissions, menu) {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                enableList = [];
                if ((folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals)) {
                    // enable View Properties
                    enableList.push("#viewFolderProperties");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin) && (permissions.Id !== 0))
                {
                    // enable cut copy folder options
                    enableList.push("#cutFolder");
                    enableList.push("#copyFolder");
                }
                if (permissions.Id !== 0 && folderPermissions.CanDeleteFolders) {
                    // can't delete the root folder no matter what
                    enableList.push("#deleteFolder");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function() {
                Ektron.ContextMenus.Trees.InitializeMenu("div#TreeOutput ul.Content li a[data-ektron-datatype='8']", "#calendarFolderContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.Collections =
        {
            confirmCollectionDelete: "",
            EnableCommands: function(permissions, menu, folderid, id)
            {
                id = parseInt(id, 10);
                menu.find("a").attr("data-ektron-folderid", folderid);
                folderPermissions = permissions.Folder;
                rolePermissions = permissions.RoleMemberships;
                enableList = [];
                if (rolePermissions.AminCollectionMenu || rolePermissions.AdminCollection || folderPermissions.IsCollections)
                {
                    if (id < 0)
                    {
                        // root node
                        enableList.push("#addCollection");
                    }
                    else
                    {
                        enableList.push("#addCollectionItems");
                        enableList.push("#removeCollection");
                        enableList.push("#reorderCollection");
                        enableList.push("#viewCollectionProperties");
                        enableList.push("#deleteCollection");
                    }
                }
                
                if(top.Ektron.Workarea.TaxonomyClipBoard.items.length > 0 && id > -1)
                {
                    // enable paste Content
                    enableList.push("#collectionAssignSelectedItems");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div#CollectionTreeOutput ul.ektree li a", "#collectionsContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.CommunityFolder =
        {
            EnableCommands: function(permissions, menu)
            {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                rolesPermissions = permissions.RoleMemberships; 
                enableList = [];
                if(folderPermissions.CanAddFolders)
                {
                    enableList.push("#addBlogFolder");
                    enableList.push("#addBoardFolder");
                    enableList.push("#addCommunityFolder");
                    enableList.push("#addCalendarFolder");
                    if (permissions.LicensedFeatures.eCommerce) {
                        enableList.push("#addEcommerceFolder");
                    }
                }
                if((top.Ektron.Workarea.ClipBoard.items.length > 0) && (top.Ektron.Workarea.ClipBoard.items[0].t != 3333 ))
                {
                    // enable paste Content
                    enableList.push("#showConfirmModal");
                }
                if(folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals)
                {
                    // enable View Properties
                    enableList.push("#viewFolderProperties");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin) && (permissions.Id !== 0))
                {
                    // enable cut copy folder options
                    enableList.push("#cutFolder");
                    enableList.push("#copyFolder");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.IsAdmin) && (top.Ektron.Workarea.FolderClipBoard.folderid != "") 
                    && (top.Ektron.Workarea.FolderClipBoard.foldertype === "6"))
                {
                    // enable paste folder options
                    enableList.push("#pasteFolder");
                }
                if(folderPermissions.CanDelete)
                {
                    // enable delete options
                    enableList.push("#deleteFolderContent");
                }
                if (folderPermissions.CanDeleteFolders)
                {
                    enableList.push("#deleteFolder");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div.ektronTreeContainer ul.Content li a[data-ektron-datatype='6']", "#communityFolderContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.DiscussionBoardFolder =
        {
            EnableCommands: function(permissions, menu)
            {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                enableList = [];
                if(folderPermissions.CanAddFolders)
                {
                    enableList.push("#addDiscussionForum");
                    enableList.push("#addSubject");
                }
                if(folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals)
                {
                    // enable View Properties
                    enableList.push("#viewBoardProperties");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin) && (permissions.Id !== 0))
                {
                    // enable cut copy folder options
                    enableList.push("#cutFolder");
                    enableList.push("#copyFolder");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.IsAdmin) && (top.Ektron.Workarea.FolderClipBoard.folderid != "") 
                    && (top.Ektron.Workarea.FolderClipBoard.foldertype != "3" && top.Ektron.Workarea.FolderClipBoard.foldertype === "4"))
                {
                    // enable paste folder options
                    enableList.push("#pasteFolder");
                }
                if(folderPermissions.CanDeleteFolders)
                {
                    // enable delete options
                    enableList.push("#deleteFolder");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div.ektronTreeContainer ul.Content li a[data-ektron-datatype='3']", "#discussionBoardFolderContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.DiscussionForumFolder =
        {
            EnableCommands: function(permissions, menu)
            {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                enableList = [];                
                if(folderPermissions.CanAdd)
                {
                    enableList.push("#addForumTopic");
                }
                if(permissions.IsARoleMemberForFolder_FolderUserAdmin || folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals)
                {
                    // enable View Properties
                    enableList.push("#viewForumProperties");
                }
                if(permissions.IsARoleMemberForFolder_FolderUserAdmin || folderPermissions.IsAdmin)
                {
                    // enable View Properties
                    enableList.push("#viewForumPermissions");
                }
                if(folderPermissions.CanDeleteFolders || folderPermissions.IsAdmin)
                {
                    // enable delete options
                    enableList.push("#deleteForum");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div.ektronTreeContainer ul.Content li a[data-ektron-datatype='4']", "#discussionForumFolderContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.EcommerceFolder =
        {
            EnableCommands: function(permissions, menu)
            {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                rolesPermissions = permissions.RoleMemberships;
                licenses = permissions.LicensedFeatures;
                enableList = [];
                
                if((top.Ektron.Workarea.ClipBoard.items.length > 0) && (top.Ektron.Workarea.ClipBoard.items[0].t == 3333) )
                {
                    // enable paste Content
                    enableList.push("#showConfirmModal");
                }
                if ((folderPermissions.CanAddFolders || rolesPermissions.CommerceAdmin)&& licenses.eCommerce)
                {
                    enableList.push("#addEcommerceFolder");
                }
                if(folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals)
                {
                    // enable View Properties
                    enableList.push("#viewFolderProperties");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin) && (permissions.Id !== 0))
                {
                    // enable cut copy folder options
                    enableList.push("#cutFolder");
                    enableList.push("#copyFolder");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.IsAdmin) && (top.Ektron.Workarea.FolderClipBoard.folderid != "") 
                    && (top.Ektron.Workarea.FolderClipBoard.foldertype === "9"))
                {
                    // enable paste folder options
                    enableList.push("#pasteFolder");
                }
                if ((folderPermissions.CanDeleteFolders || rolesPermissions.CommerceAdmin) && licenses.eCommerce)
                {
                    // can't delete the root folder no matter what
                    enableList.push("#deleteFolder");
                }
                if ((folderPermissions.CanDelete || folderPermissions.IsAdmin) && licenses.eCommerce)
                {
                    // enable delete options
                    enableList.push("#deleteFolderContent");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div.ektronTreeContainer ul.Content li a[data-ektron-datatype='9']", "#ecommerceFolderContextMenu");
            }
        };
        
        Ektron.ContextMenus.Trees.EnableCommands = function(permissions, context)
        {
            context.Menu.disableContextMenu();
            switch (context.DataType)
            {
                case "0":
                    // folder
                    Ektron.ContextMenus.Trees.Folder.EnableCommands(permissions, context.Menu);
                    break;
                case "1":
                    // blog
                    Ektron.ContextMenus.Trees.BlogFolder.EnableCommands(permissions, context.Menu);
                    break;
                case "2":
                    // site|domain
                    Ektron.ContextMenus.Trees.SiteFolder.EnableCommands(permissions, context.Menu);
                    break;
                case "3":
                    // discussion board
                    Ektron.ContextMenus.Trees.DiscussionBoardFolder.EnableCommands(permissions, context.Menu);
                    break;
                case "4":
                    // discussion forum
                    Ektron.ContextMenus.Trees.DiscussionForumFolder.EnableCommands(permissions, context.Menu);
                    break;
                case "5":
                    // root
                    break;
                case "6":
                    // community folder
                    Ektron.ContextMenus.Trees.CommunityFolder.EnableCommands(permissions, context.Menu);
                    break;
                case "7":
                    // media folder
                    break;
                case "8":
                    // calendar folder
                    Ektron.ContextMenus.Trees.Calendar.EnableCommands(permissions, context.Menu);
                    break;
                case "9":
                    // ecommerce|catalog folder
                    Ektron.ContextMenus.Trees.EcommerceFolder.EnableCommands(permissions, context.Menu);
                    break;
                case "10":
                    // taxonomy
                    Ektron.ContextMenus.Trees.Taxonomy.EnableCommands(permissions, context.Menu, context.Id);
                    break;
                case "11":
                    // collection
                    Ektron.ContextMenus.Trees.Collections.EnableCommands(permissions, context.Menu, context.FolderId, context.Id);
                    break;
                case "12":
                    // menu
                    Ektron.ContextMenus.Trees.Menus.EnableCommands(permissions, context.Menu, context.FolderId, context.Id, context.ItemCount);
                    break;
            }
            Ektron.ContextMenus.AdjustMenuPosition(context.Menu, false);
        };
        
        Ektron.ContextMenus.Trees.Folder =
        {
            confirmFolderDelete: "",
            confirmInheritTargetFolder: "",
            EnableCommands: function(permissions, menu)
            {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                roleMemberships = permissions.RoleMemberships;
                enableList = [];
                if(folderPermissions.CanAddFolders)
                {
                    enableList.push("#addFolder");
                    enableList.push("#addBlogFolder");
                    enableList.push("#addBoardFolder");
                    enableList.push("#addCommunityFolder");
                    enableList.push("#addCalendarFolder");
                    if (permissions.LicensedFeatures.eCommerce) {
                        enableList.push("#addEcommerceFolder");
                    }
                    if(permissions.Id === 0 && permissions.LicensedFeatures.MultiSite)
                    {
                        enableList.push("#addSiteFolder");
                    }
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin || folderPermissions.CanAdd || folderPermissions.CanEdit) && (top.Ektron.Workarea.ClipBoard.items.length > 0 && top.Ektron.Workarea.ClipBoard.items[0].t != 3333))
                {
                    // enable paste Content
                    enableList.push("#showConfirmModal");
                }
                if((folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals))
                {
                    // enable View Properties
                    enableList.push("#viewFolderProperties");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin) && (permissions.Id !== 0))
                {
                    // enable cut copy folder options
                    enableList.push("#cutFolder");
                    enableList.push("#copyFolder");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.IsAdmin) && (top.Ektron.Workarea.FolderClipBoard.folderid != "") 
                    && (top.Ektron.Workarea.FolderClipBoard.foldertype !== "2") && top.Ektron.Workarea.FolderClipBoard.foldertype !== "4")
                {
                    // enable paste folder options
                    enableList.push("#pasteFolder");
                }
                if(folderPermissions.CanDelete || folderPermissions.IsAdmin)
                {
                    // enable delete options
                    enableList.push("#deleteFolderContent");
                }
                if (permissions.Id !== 0 && folderPermissions.CanDeleteFolders)
                {
                    // can't delete the root folder no matter what
                    enableList.push("#deleteFolder");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div#TreeOutput ul.Content li a[data-ektron-datatype='0'], div.ektronTreeContainer ul.Content li#T0_tv0 > span a", "#folderContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.GetContextProperties = function(el, menu)
        {
            var contextData = {};
            contextData.TriggerId = el.attr("id");
            contextData.Text = el.text();
            contextData.LangType = el.attr("data-ektron-languagetype");
            contextData.DataType = el.attr("data-ektron-datatype");
            if ("undefined" == typeof(contextData.DataType))
            {
                contextData.DataType = "0";
                el.attr("data-ektron-datatype", "0");
            }
            if ("undefined" == typeof(contextData.LangType))
            {
                contextData.LangType = "";
                el.attr("data-ektron-languagetype", "");
            }
            contextData.Id = (contextData.TriggerId).substring(1, (contextData.TriggerId).indexOf("_"));
            contextData.Menu = $ektron(menu);
            var folderid = el.attr("data-ektron-folderid");
            if(folderid)
            {
                contextData.FolderId = folderid;
            }
            var parentId = el.attr("data-ektron-parentid");
            if(parentId)
            {
                contextData.ParentId = parentId;
            }
            var itemCount = el.attr("data-ektron-itemcount");
            if(itemCount)
            {
                contextData.ItemCount = itemCount;
            }
            var ancestorId = el.attr("data-ektron-ancestorid");
            if(ancestorId)
            {
                contextData.AncestorId = ancestorId;
            }
            return contextData;
        };
        
        Ektron.ContextMenus.Trees.GetFolderPath = function(folderId)
        {
            var href = "controls/content/cutCopyAssignHandler.ashx";
            var folderPathContext = {
                targetid : folderId,
                action : "getfolderpath"
            };
            $ektron.ajax({
                type: "Post",
                url: href,
                data: folderPathContext,                
                success: function(data)
                {
                    top.TreeNavigation("LibraryTree", data);
                    top.TreeNavigation("ContentTree", data);
                }
            });
        };
        
        Ektron.ContextMenus.Trees.Init = function ()
        {
            // initialize
            Ektron.ContextMenus.Trees.Folder.Init();
            Ektron.ContextMenus.Trees.SiteFolder.Init();
            Ektron.ContextMenus.Trees.BlogFolder.Init();
            Ektron.ContextMenus.Trees.CommunityFolder.Init();
            Ektron.ContextMenus.Trees.DiscussionBoardFolder.Init();
            Ektron.ContextMenus.Trees.DiscussionForumFolder.Init();
            Ektron.ContextMenus.Trees.EcommerceFolder.Init();
            Ektron.ContextMenus.Trees.Calendar.Init();
            Ektron.ContextMenus.Trees.Taxonomy.Init();
            Ektron.ContextMenus.Trees.Collections.Init();
            Ektron.ContextMenus.Trees.Menus.Init();

            // remove the highlight class when contextMenuHide event fires
            $ektron(document).bind("contextMenuHide", function()
            {
                $ektron(document).find(".contextMenuHighlight").removeClass("contextMenuHighlight");
            });
        };

        Ektron.ContextMenus.Trees.InitializeMenu = function(menuTarget, menuId)
        {
            var targetLinks = $ektron(menuTarget + "");
            targetLinks.destroyContextMenu().contextMenu(
            {
                menu: menuId,
                hideMenuDelay: 2500,
                inSpeed: 500,
                outSpeed: 200,
                onContextMenu: function(s)
                {
                    var theContext = Ektron.ContextMenus.Trees.GetContextProperties(s.menuTrigger, this.menu);
                    Ektron.ContextMenus.HighlightAndTextSwap(s.menuTrigger, theContext);
                    var theDataParams = "checkType=" + theContext.DataType;
                    if (theContext.FolderId)
                    {
                        theDataParams += "&action=getPermissions&id=" + theContext.FolderId;
                    }
                    else
                    {
                        theDataParams += "&action=getPermissions&id=" + theContext.Id;
                    }
                    var permissionsObj = Ektron.ContextMenus.GetPermissions({
                        context: theContext,
                        dataParams: theDataParams,
                        onSuccess: function(data)
                        {
                            var permissions = $ektron.extend(permissionsObj, data);
                            Ektron.ContextMenus.Trees.EnableCommands(permissions, theContext);
                        },
                        onError: function(XMLHttpRequest, textStatus, errorThrown, myContext)
                        {
                            myContext.menu.disableContextMenu();
                        }
                    });
                },
                onItemClick: function(s)
                {
                    Ektron.ContextMenus.Trees.PerformAction(s.action, s.menuTrigger, s.menuItemClicked);
                }
            });
        };

        Ektron.ContextMenus.Trees.Menus =
        {
            confirmMenuDelete: "",
            EnableCommands: function(permissions, menu, folderid, id, itemCount)
            {
                id = parseInt(id, 10);
                menu.find("a").attr("data-ektron-folderid", folderid);
                rolePermissions = permissions.RoleMemberships;
                enableList = [];
                folderPermissions = permissions.Folder;
                roleMemberships = permissions.RoleMemberships;
                if (rolePermissions.AminCollectionMenu || rolePermissions.AdminMenu || folderPermissions.IsCollections)
                {
                    if (id < 0)
                    {
                        enableList.push("#addMenu");
                    }
                    if (id > 0)
                    {
                        enableList.push("#addMenuItems");
                        if (itemCount > 0)
                        {
                            enableList.push("#removeMenuItems");
                        }
                        if (itemCount > 1)
                        {
                            enableList.push("#reorderMenu");
                        }
                        enableList.push("#viewMenuProperties");
                        enableList.push("#deleteMenu");
                    }
                }
                
                if(top.Ektron.Workarea.TaxonomyClipBoard.items.length > 0 && id > -1)
                {
                    // enable paste Content
                    enableList.push("#menuAssignSelectedItems");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div#MenuTreeOutput ul.ektree li span a", "#menutreeContextMenu");
            }
        };

        // Had to take it out from the Perform Action as a confirmation modal dialog has
        // to be attended before the paste can move forward.
        Ektron.ContextMenus.PasteContent = function(multiLang)
        {
            var id = top.frames["ek_main"].$ektron("#targetId")[0].value;
            var parentId = top.frames["ek_main"].$ektron("#parentId")[0].value;
            var clipBoardAction = top.Ektron.Workarea.ClipBoard.action;
            var clipBoardContext = Ektron.JSON.stringify(top.Ektron.Workarea.ClipBoard.items, null, null);
            var href = "controls/content/cutCopyAssignHandler.ashx";
            var params = {
                clipBoardContext : clipBoardContext,
                action : clipBoardAction,
                multiLang : multiLang,
                targetId : id
            };
            $ektron.ajax({
                type: "Post",
                url: href,
                data: params,                
                success: function(data)
                {
                    if(data.toString().indexOf("Error: ") !== 0)
                    {
                        Ektron.ContextMenus.Trees.redirectUrl = "/content.aspx?action=ViewContentByCategory&id=" + id + "&treeViewId=0";
                    }
                    else
                    {
                        Ektron.ContextMenus.redirectToErrror(data);
                    }
                    Ektron.ContextMenus.RedirectRightPane(Ektron.ContextMenus.Trees.redirectUrl);
                    if(clipBoardAction === "cutContent")
                    {
                        top.Ektron.Workarea.ClipBoard.items = [];
                        top.Ektron.Workarea.TaxonomyClipBoard.items = [];
                        top.Ektron.Workarea.ClipBoard.action = clipBoardAction;
                    }
                    $ektron(top.document).find('#BottomFrameSet').find('#ek_nav_bottom')[0].contentWindow.NavIframeContainer.frames['nav_folder_area'].frames['ContentTree'].$ektron('.ektronModalOverlay').modalHide();
                    $ektron(top.document).find('#BottomFrameSet').find('#ek_nav_bottom')[0].contentWindow.NavIframeContainer.frames['nav_folder_area'].frames['ContentTree'].$ektron('#blockTree').modalHide();
                    Ektron.ContextMenus.Trees.GetFolderPath(id);
                }
            });
        };
        
        Ektron.ContextMenus.Trees.PerformAction = function(action, context, trigger)
        {
            var triggerId = context.attr("id");
            var triggerName = trigger.find("span.triggerName");
            var id = triggerId.substring(1, triggerId.indexOf("_"));
            var folderid = " ";
            var folderIdAttr = trigger.attr("data-ektron-folderid");
            var status = context.attr("data-ektron-status");
            
            var langType = ("undefined" == typeof(trigger.attr("data-ektron-languagetype"))) ? (("undefined" == typeof(context.attr("data-ektron-languagetype"))) ? "" : context.attr("data-ektron-languagetype")) : trigger.attr("data-ektron-languagetype");
            if(langType == "")
            {
                langType = $ektron("#contLanguage")[0].value;
            }
            var parentId = context.attr("data-ektron-parentid");
            var ancestorId = context.attr("data-ektron-ancestorid");
            var approvalRequired = (context.attr("data-ektron-approvalrequired") == "true") ? true : false;
            if(folderIdAttr)
            {
                folderid = trigger.attr("data-ektron-folderid");
            }
            var url = new Ektron.String();
            var redirect = false;

            var clipBoardAction = top.Ektron.Workarea.ClipBoard.action;
            var clipBoardContext = Ektron.JSON.stringify(top.Ektron.Workarea.ClipBoard.items, null, null);
            
            var taxonomyClipBoardAction = top.Ektron.Workarea.TaxonomyClipBoard.action;
            var taxonomyClipBoardContext = Ektron.JSON.stringify(top.Ektron.Workarea.TaxonomyClipBoard.items, null, null);
            
            var folderClipBoardAction = top.Ektron.Workarea.FolderClipBoard.action;
            var folderSourceId = top.Ektron.Workarea.FolderClipBoard.folderid;
            
            var taxonomyTreeClipBoardAction = top.Ektron.Workarea.TaxonomyTreeClipBoard.action;
            var taxonomyTreeClipBoardSourceId = top.Ektron.Workarea.TaxonomyTreeClipBoard.taxonomyid;
            var multiLang = false;
            var href = "controls/content/cutCopyAssignHandler.ashx";
            
            switch (action)
            {
                case "addBlogFolder":
                   command = "content.aspx?action=AddSubFolder&type=blog&id={0}";
                   redirect = true;
                   break;
                case "addBlogPost":
                    command = "edit.aspx?close=false&ContType=1&type=add&createtask=1&id={0}&folderid={0}&back_file=content.aspx&back_action=ViewContentByCategory&back_id={0}&AllowHTML=1";
                   redirect = true;
                    break;
                case "addBoardFolder":
                    command = "content.aspx?action=AddSubFolder&type=discussionboard&id={0}";
                    redirect = true;
                    break;
                case "addCollection":
                   command = "collections.aspx?action=Add&folderid=0&rf=1&back=collections.aspx%3faction%3dViewCollectionReport";
                   redirect = true;
                   break;
                case "addCollectionItems":
                   command = "collections.aspx?action=AddLink&nid={0}&folderid={2}&rf=1";
                   redirect = true;
                   break;
                case "addCommunityFolder":
                    command = "content.aspx?LangType=" + langType + "&action=AddSubFolder&type=communityfolder&id={0}";
                    redirect = true;
                    break;
                case "addCalendarFolder":
                    command = "content.aspx?action=AddSubFolder&type=calendar&id={0}";
                    redirect = true;
                    break;
                case "addDiscussionForum":
                    command = "content.aspx?action=AddSubFolder&type=discussionforum&id={0}";
                    redirect = true;
                    break;
                case "addEcommerceFolder":
                    command = "content.aspx?LangType=" + langType + "&action=AddSubFolder&type=catalog&id={0}";
                    redirect = true;
                    break;
                case "addFolder":
                   command = "content.aspx?LangType=" + langType + "&type=folder&action=AddSubFolder&id={0}";
                   redirect = true;
                   break;
                case "addMenu":
                    if (id < 0)
                    {
                        command = "collections.aspx?action=AddMenu&folderid=0&bPage=ViewMenuReport&rf=1&back=collections.aspx%3faction%3dViewMenuReport";
                    }
                    else
                    {
                        command = "collections.aspx?action=AddSubMenu&folderid=0&nId={0}&parentid={0}&ancestorid=" + ancestorId + "&bPage=ViewMenuReport&rf=1&back=collections.aspx%3faction%3dViewMenuReport";
                    }
                    redirect = true;
                    break;
                case "addMenuItems":
                   command = "collections.aspx?action=AddMenuItem&nid={0}&folderid=0&LangType=" + langType + "&back=menu.aspx%3fAction%3dviewcontent%26menuid%3d{0}%26treeViewId%3d-3&parentid={0}&ancestorid=" + ancestorId + "&rf=1";
                   redirect = true;
                   break;
                case "addSiteFolder":
                    command = "content.aspx?LangType=1033&type=site&action=AddSubFolder&id={0}";
                    redirect = true;
                    break;
                case "addSubject":
                    command = "threadeddisc/addeditboard.aspx?action=addcat&id={0}";
                    redirect = true;
                    break;
                case "addTaxonomy":
                    if (id < 0)
                    {
                        command = "taxonomy.aspx?action=add";
                    }
                    else
                    {
                        command = "taxonomy.aspx?action=add&parentid={0}&LangType=" + langType;
                    }
                    redirect = true;
                    break;
                case "addForumTopic":
                    command = "threadeddisc/addedittopic.aspx?action=add&LangType=" + langType + "&id={0}&rf=1";
                    redirect = true;
                    break;
                case "deleteFolder":
                    command = "content.aspx?action=DoDeleteFolder&id={0}&ParentID={1}";
                    redirect = top.confirm(Ektron.ContextMenus.Trees.Folder.confirmFolderDelete);
                    break;
                case "deleteCollection":
                    command = "collections.aspx?action=doDelete&nId={0}&folderid={2}&rf=1&bpage=reports";
                    redirect = top.confirm(Ektron.ContextMenus.Trees.Collections.confirmCollectionDelete);
                    break;
                case "deleteFolderContent":
                    command = "content.aspx?action=DeleteContentByCategory&id={0}";
                    redirect = true;
                    break;
                case "deleteForum":
                    command = "content.aspx?action=DoDeleteFolder&id={0}&ParentID={1}";
                    redirect = top.confirm(Ektron.ContextMenus.Trees.Folder.confirmFolderDelete);
                    break;
                case "deleteMenu":
                    command = "collections.aspx?action=doDeleteMenu&reloadtrees=menu&nid={0}&LangType=" + langType + "&back=menu.aspx%3Faction%3Ddeleted%26title%3D" + triggerName + "&rf=1";
                    redirect = top.confirm(Ektron.ContextMenus.Trees.Menus.confirmMenuDelete);
                    break;
                case "removeCollection":
                    command = "collections.aspx?action=DeleteLink&nid={0}&folderid={2}&rf=1";
                    redirect = true;
                    break;
                case "removeMenuItems":
                    command = "menu.aspx?action=removeitems&menuid={0}&parentid=" + parentId + "&rf=1";
                    redirect = true;
                    break;
                case "reorderCollection":
                    var commandAddendum = "";
                    if (approvalRequired)
                    {
                        if (status == "A")
                        {
                            commandAddendum = "&folderid=0&checkout=true";
                        }
                        else
                        {
                            commandAddendum = "&status=o";
                        }
                    }
                    command = "collections.aspx?action=ReOrderLinks&nid={0}&rf=1" + commandAddendum;
                    redirect = true;
                    break;
                case "reorderMenu":
                    command = "collections.aspx?action=ReOrderMenuItems&nid={0}&folderid=0&LangType=" + langType + "&back=menu.aspx%3fAction%3dviewcontent%26menuid%3d{0}%26treeViewId%3d-3&rf=1";
                    redirect = true;
                    break;
                case "taxonomyAddContent":
                    command = "taxonomy.aspx?action=additem&taxonomyid={0}&LangType=" + langType;
                    redirect = true;
                    break;
                case "taxonomyAddFolders":
                    command = "taxonomy.aspx?action=addfolder&taxonomyid={0}&LangType=" + langType;
                    redirect = true;
                    break;
                case "viewCollectionProperties":
                    command = "collections.aspx?action=ViewAttributes&nid={0}&folderid={2}";
                    redirect = true;
                    break;
                case "viewBoardProperties":
                    command = "threadeddisc/addeditboard.aspx?LangType=" + langType + "&action=View&id={0}";
                    redirect = true;
                    break;
                case "viewForumProperties":
                    command = "threadeddisc/addeditforum.aspx?action=View&id={0}";
                    redirect = true;
                    break;
                case "viewForumPermissions":
                    command = "content.aspx?action=ViewPermissions&type=folder&rf=1&id={0}";
                    redirect = true;
                    break;
                case "viewFolderProperties":
                    command = "content.aspx?action=ViewFolder&id={0}";
                    redirect = true;
                    break;
                case "viewMenuProperties":
                    command = "menu.aspx?action=viewmenu&menuid={0}&parentid=0&LangType=" + langType;
                    redirect = true;
                    break;
                case "viewTaxonomyProperties":
                    command = "taxonomy.aspx?action=view&taxonomyid={0}&LangType=" + langType;
                    redirect = true;
                    break;
                // Action for the MultiLingual paste.
                case "showConfirmModal":
                    // Check if the source content language is different the workarea language,
                    // if so, throw an alert to user that moving content can not happen and select the source content language.
                    if(("" + top.Ektron.Workarea.FolderContext.folderLanguage + "" !== "" + top.Ektron.Workarea.ClipBoard.items[0].l + "") && (clipBoardAction.toLowerCase() == "cutcontent"))
                    {
                        alert("The current language does not match the language of the cut content. Before you paste, you must switch to the cut content language.");
                        return false;
                    }
                    // Checks if the right frame is not content.aspx.
                    // if not, then refresh the right frame with content.aspx with target folder's content.
                    if(top.frames["ek_main"].location.href.indexOf("content.aspx?") === -1)
                    {
                        top.Ektron.Workarea.isnoncontentframe = true;
                        top.Ektron.Workarea.backurl = top.frames["ek_main"].location.href;
                        // wait until the right pane is refreshed and then continue with setting the target folder id, 
                        // target parent id, as well as blocking the right and left frame using modal dialog.
                        var time = setTimeout('Ektron.ContextMenus.blockWindow();',1500);
                        var url = "content.aspx?action=ViewContentByCategory&id=" + id + "&treeviewid=0";
                        Ektron.ContextMenus.RedirectRightPane(url);
                    }
                    // if it is content.aspx page, then perform with the normal routine.
                    else
                    {
                        $ektron(top.document).find('#BottomFrameSet').find('#ek_nav_bottom')[0].contentWindow.NavIframeContainer.frames['nav_folder_area'].frames['ContentTree'].$ektron('#blockTree').modalShow(); 
                        top.frames['ek_main'].$ektron('#targetId')[0].value = id; 
                        top.frames['ek_main'].$ektron('#parentId')[0].value = parentId;
                        Ektron.ContextMenus.blockWindow.cutmovewindow();
                    }
                    break;
                case "taxonomyAssignSelectedItems":
                    Ektron.ContextMenus.Trees.blockframes();
                    var taxonomyAssignSelectedItems = {
                        clipBoardContext : taxonomyClipBoardContext,
                        action : "assignitems",
                        targetid : id
                    };
                    $ektron.ajax({
                        type: "Post",                        
                        url: href,
                        data: taxonomyAssignSelectedItems,                        
                        success: function(data)
                        {
                            if(data.toString().indexOf("Error: ") !== 0)
                            {
                                Ektron.ContextMenus.Trees.redirectUrl = "taxonomy.aspx?action=view&view=item&taxonomyid=" + id + "&treeViewId=-1&LangType=" + langType;
                                top.refreshTaxonomyAccordion(langType);
                            }
                            else
                            {
                                Ektron.ContextMenus.redirectToErrror(data);
                            }
                            Ektron.Workarea.Overlay.unblock();
                            Ektron.ContextMenus.RedirectRightPane(Ektron.ContextMenus.Trees.redirectUrl);
                        }
                    });
                    
                    break;
                case "collectionAssignSelectedItems":
                
                    Ektron.ContextMenus.Trees.blockframes();
                    
                    var collectionAssignSelectedItems = {
                        clipBoardContext : taxonomyClipBoardContext,
                        action : "assignItemsToCollection",
                        targetid : id
                    };
                    
                    $ektron.ajax({
                        type: "Post",                        
                        url: href,
                        data: collectionAssignSelectedItems,                        
                        success: function(data)
                        {
                            if(data.toString().indexOf("Error: ") !== 0)
                            {
                                Ektron.ContextMenus.Trees.redirectUrl = "collections.aspx?Action=View&nid=" + id + "&treeViewId=-2";
                            }
                            else
                            {
                                Ektron.ContextMenus.redirectToErrror(data);
                            }
                            Ektron.Workarea.Overlay.unblock();
                            Ektron.ContextMenus.RedirectRightPane(Ektron.ContextMenus.Trees.redirectUrl);
                        }
                    });
                    break;
                case "menuAssignSelectedItems":
                
                    Ektron.ContextMenus.Trees.blockframes();
                    
                    
                    var menuAssignSelectedItems = {
                        clipBoardContext : taxonomyClipBoardContext,
                        action : "assignItemsToMenu",
                        targetid : id
                    };
                    $ektron.ajax({
                        type: "Post",
                        url: href,
                        data: menuAssignSelectedItems,                        
                        success: function(data)
                        {
                            if(data.toString().indexOf("Error: ") !== 0)
                            {
                                Ektron.ContextMenus.Trees.redirectUrl = "menu.aspx?Action=viewcontent&menuid=" + id + "&treeViewId=-3&LangType=" + langType;
                            }
                            else
                            {
                                Ektron.ContextMenus.redirectToErrror(data);
                            }
                            Ektron.Workarea.Overlay.unblock();
                            Ektron.ContextMenus.RedirectRightPane(Ektron.ContextMenus.Trees.redirectUrl);
                        }
                    });
                    break;
                case "cutFolder":
                case "copyFolder":
                    Ektron.ContextMenus.Trees.SetFolderClipBoard(action, context);
                    break;
                case "pasteFolder":
                    if(top.Ektron.Workarea.FolderClipBoard.foldertype == "9" && folderClipBoardAction == "copyFolder")
                    {
                        alert("Warning! The catalog folder will be copied but its content will not be copied. ");
                    }
                    if(!confirm(Ektron.ContextMenus.Trees.Folder.confirmInheritTargetFolder))
                    {
                        return false;
                    }
                    
                    Ektron.ContextMenus.Trees.blockframes();
                    
                    var folderClipBoard = {
                        action: folderClipBoardAction,
                        sourceid : folderSourceId,
                        targetid : id
                    };
                    $ektron.ajax({
                        type: "Post",
                        url: href,
                        data: folderClipBoard,                        
                        success: function(data)
                        {
                            if(data.toString().indexOf("Error: ") !== 0)
                            {
                                Ektron.ContextMenus.Trees.redirectUrl = "content.aspx?action=ViewContentByCategory&id=" + id + "&treeViewId=0";
                                Ektron.ContextMenus.Trees.GetFolderPath(id);
                            }
                            else
                            {
                                Ektron.ContextMenus.redirectToErrror(data);
                            }
                            Ektron.Workarea.Overlay.unblock();
                            Ektron.ContextMenus.RedirectRightPane(Ektron.ContextMenus.Trees.redirectUrl);
                        }
                    });
                    break;
                case "cutTaxonomy":
                case "copyTaxonomy":
                    Ektron.ContextMenus.Trees.SetTaxonomyTreeClipBoard(action, context);
                    break;
                case "deleteTaxonomy":
                    if(!confirm(Ektron.ContextMenus.Trees.Folder.conformTaxonomyDelete))
                    {
                        return false;
                    }
                    
                    taxonomyTreeClipBoardSourceId = context.attr("data-ektron-folderid");
                    taxonomyTreeClipBoardAction = action;
                    Ektron.ContextMenus.Trees.blockframes();
                    
                    var delTaxClipBoard = {
                        action : taxonomyTreeClipBoardAction,
                        targetid : 0,
                        sourceid : taxonomyTreeClipBoardSourceId,
                        LangType : langType
                    };
                    $ektron.ajax({
                        type: "Post",
                        url: href,
                        data: delTaxClipBoard,                        
                        success: function(data)
                        {
                            if(data.toString().indexOf("Error: ") !== 0)
                            {
                                Ektron.ContextMenus.Trees.redirectUrl = "taxonomy.aspx?rf=1&reloadtrees=Tax";
                                top.refreshTaxonomyAccordion(langType);
                            }
                            else
                            {
                                Ektron.ContextMenus.redirectToErrror(data);
                            }
                            Ektron.Workarea.Overlay.unblock();
                            Ektron.ContextMenus.RedirectRightPane(Ektron.ContextMenus.Trees.redirectUrl);
                            top.Ektron.Workarea.TaxonomyTreeClipBoard.action = "";
                            top.Ektron.Workarea.TaxonomyTreeClipBoard.taxonomyid = "";
                        }
                    });
                    break;
                    
                case "pasteTaxonomy":
                    
                    Ektron.ContextMenus.Trees.blockframes();
                    
                    var pasteTaxContext = {
                        sourceid : taxonomyTreeClipBoardSourceId,
                        action : taxonomyTreeClipBoardAction,
                        targetid : id,
                        LangType : langType
                    };
                    $ektron.ajax({
                        type: "Post",
                        url: href,
                        data: pasteTaxContext,                        
                        success: function(data)
                        {
                            if(data.toString().indexOf("Error: ") !== 0)
                            {
                                Ektron.ContextMenus.Trees.redirectUrl = "taxonomy.aspx?action=view&view=item&reloadtrees=Tax&taxonomyid=" + data + "&treeViewId=-1&LangType=" + langType;
                                top.refreshTaxonomyAccordion(langType);
                            }
                            else
                            {
                                Ektron.ContextMenus.redirectToErrror(data);
                            }
                            Ektron.Workarea.Overlay.unblock();
                            Ektron.ContextMenus.RedirectRightPane(Ektron.ContextMenus.Trees.redirectUrl);
                            top.Ektron.Workarea.TaxonomyTreeClipBoard.action = "";
                            top.Ektron.Workarea.TaxonomyTreeClipBoard.taxonomyid = "";
                        }
                    });
                    break;
                default:
                    // unrecognized command
                    top.alert("unrecognized menu command");
                    break;
            }
            if (redirect)
            {
                url = Ektron.String.format(command, id, parentId, folderid);
                Ektron.ContextMenus.RedirectRightPane(url);
            }
        };
        
        // If the right frame is not content.aspx, after redirecting it to content.aspx page,
        // set the target folderid, target folder's parent id as well as block the right and the left frame.
        
        Ektron.ContextMenus.blockWindow = function(){
            var id = top.Ektron.Workarea.FolderContext.folderId;
            var parentId = top.Ektron.Workarea.FolderContext.folderParentId;
            $ektron(top.document).find('#BottomFrameSet').find('#ek_nav_bottom')[0].contentWindow.NavIframeContainer.frames['nav_folder_area'].frames['ContentTree'].$ektron('#blockTree').modalShow(); 
            top.frames['ek_main'].$ektron('#targetId')[0].value = id; 
            top.frames['ek_main'].$ektron('#parentId')[0].value = parentId;
            
            // Call is made to block the frames depending on the action cut or move.
            
            Ektron.ContextMenus.blockWindow.cutmovewindow();
        };
        
        // Blocks the frames depending on the action cut or move.
        
        Ektron.ContextMenus.blockWindow.cutmovewindow = function(){
        
            // Confirmation Modal dialog, which gives the user if he/she wants to move the content.
            
            if ( top.Ektron.Workarea.ClipBoard.action.toLowerCase() == "cutcontent")
            {
                top.frames["ek_main"].$ektron("#moveContentModal").modalShow();
            }
            
            // Show Confirmation Dialog asking user to copy in single selected language 
            // or all the languages in which the content is generated.
            
            else if ( top.Ektron.Workarea.ClipBoard.action.toLowerCase() == "copycontent")
            {
                
                top.frames["ek_main"].$ektron("#selectMultiLingual").modalShow();
            }
        };
        Ektron.ContextMenus.redirectToErrror = function(data)
        {
            data = data.replace("Error: ", '');
            Ektron.ContextMenus.Trees.redirectUrl = "/reterror.aspx?info=" + data;
        };
        
        Ektron.ContextMenus.Trees.SetFolderClipBoard = function(action, context)
        {
            top.Ektron.Workarea.FolderClipBoard.action = action;
            top.Ektron.Workarea.FolderClipBoard.folderid = context.attr("data-ektron-folderid");
            top.Ektron.Workarea.FolderClipBoard.foldertype = context.attr("data-ektron-datatype");
        };
        
        Ektron.ContextMenus.Trees.SetTaxonomyTreeClipBoard = function(action, context)
        {
            top.Ektron.Workarea.TaxonomyTreeClipBoard.action = action;
            top.Ektron.Workarea.TaxonomyTreeClipBoard.taxonomyid = context.attr("data-ektron-folderid");
        };
        
        Ektron.ContextMenus.Trees.SiteFolder =
        {
            EnableCommands: function(permissions, menu)
            {
                menu.find("a").attr("data-ektron-parentid", permissions.ParentId);
                folderPermissions = permissions.Folder;
                rolesPermissions = permissions.RoleMemberships;
                enableList = [];
                if(folderPermissions.CanAddFolders)
                {
                    enableList.push("#addFolder");
                    enableList.push("#addBlogFolder");
                    enableList.push("#addBoardFolder");
                    enableList.push("#addCommunityFolder");
                    enableList.push("#addCalendarFolder");
                    if (permissions.LicensedFeatures.eCommerce)
                    {
                        enableList.push("#addEcommerceFolder");
                    }
                }
                if((top.Ektron.Workarea.ClipBoard.items.length > 0)  && ( rolesPermissions.MoveOrCopy !== false )  && ( top.Ektron.Workarea.ClipBoard.items[0].t != 3333))
                {
                    // enable paste Content
                    enableList.push("#showConfirmModal");
                }
                if(folderPermissions.CanEditFolders || folderPermissions.CanEditApprovals)
                {
                    // enable View Properties
                    enableList.push("#viewFolderProperties");
                }
                if(folderPermissions.CanDelete)
                {
                    // enable delete options
                    enableList.push("#deleteFolderContent");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin) && (permissions.Id !== 0))
                {
                    // enable cut copy folder options
                    enableList.push("#cutFolder");
                    enableList.push("#copyFolder");
                }
                if (top.Ektron.Workarea.ClipBoard.items.length > 0 && top.Ektron.Workarea.ClipBoard.items[0].t != 3333) 
                {
                    enableList.push("#showConfirmModal");
                }
                if((folderPermissions.CanAddFolders || folderPermissions.IsAdmin) && (top.Ektron.Workarea.FolderClipBoard.folderid != "") 
                    && (top.Ektron.Workarea.FolderClipBoard.foldertype !== "2"))
                {
                    // enable paste folder options
                    enableList.push("#pasteFolder");
                }
                if (folderPermissions.CanDeleteFolders)
                {
                    enableList.push("#deleteFolder");
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div.ektronTreeContainer ul.Content li a[data-ektron-datatype='2']", "#siteFolderContextMenu");
            }
        };

        Ektron.ContextMenus.Trees.Taxonomy =
        {
            conformTaxonomyDelete: "",
            EnableCommands: function(permissions, menu, id)
            {
                id = parseInt(id, 10);
                enableList = [];
                if(permissions.TaxonomyAdministrator)
                {
                    enableList.push("#addTaxonomy");
                    if (id > 0)
                    {
                        enableList.push("#viewTaxonomyProperties");
                        enableList.push("#taxonomyAddContent");
                        enableList.push("#taxonomyAddFolders");
                        enableList.push("#cutTaxonomy");
                        enableList.push("#copyTaxonomy");
                        enableList.push("#deleteTaxonomy");
                    }
                    if(top.Ektron.Workarea.TaxonomyClipBoard.items.length > 0 && id != -1)
                    {
                        enableList.push("#taxonomyAssignSelectedItems");
                    }                   
                    if(top.Ektron.Workarea.TaxonomyTreeClipBoard.taxonomyid != "")
                    {
                        enableList.push("#pasteTaxonomy");
                    }
                }
                menu.enableContextMenuItems(enableList.toString());
                Ektron.ContextMenus.ShowSeparator(menu);
            },

            Init: function()
            {
                Ektron.ContextMenus.Trees.InitializeMenu("div#TaxonomyTreeOutput ul.ektree li a", "#taxonomyContextMenu");
            }
        };
    }
});
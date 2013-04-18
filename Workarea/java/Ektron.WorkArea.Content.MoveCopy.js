////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////
////////// This JavaScript is created to have confirmation as well as option  //////////
////////// modal dialog and related function in one central file, which is    //////////
////////// loaded when content.aspx page is loaded.                           //////////
////////// This is to avoid unnecessary bolcking frames inconsistensly.       //////////
////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////

Ektron.ready(function () {

    // Copy/Move context object, which is set for copy/move with target Folder Id, 
    //current folderType, parent Folder and currently select language.
    var folderContext;

    // Check if the folderContext is defined, if so then continue with operations.
    // This check is to see content.aspx is opened in a separate window independent (not part of workarea.aspx frame)
    // from workarea.aspx
    if(undefined !== top.Ektron && undefined !== top.Ektron.Workarea.FolderContext)
    {
        folderContext = top.Ektron.Workarea.FolderContext;

        // Using jQuery AJAX to load the permissions for the current user.
        GetPermissions = function (settings) {
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
            $ektron.ajax({
                type: "Get",
                url: appPath + "/controls/permission/permissionsCheckHandler.ashx",
                data: s.dataParams,
                dataType: "json",
                success: function (data) {
                    if (s.onSuccess !== null) {
                        s.onSuccess(data, s.context);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (s.onError !== null)
                    {
                        s.onError(XMLHttpRequest, textStatus, errorThrown, s.context);
                    }
                }
            });
            return permissionsObj;
        };

        // Displays the Confirmation Modal dislog for multi language copy/move.
        // Also, sets the folderId and parent FolderId for the copy/move action.

        showConfirm = function () {
            // Check if the source content language is different the workarea language,
            // if so, throw an alert to user that moving content can not happen and select the source content language.
            if(("" + folderjslanguage + "" !== "" + top.Ektron.Workarea.ClipBoard.items[0].l + "") && (top.Ektron.Workarea.ClipBoard.action.toLowerCase() == "cutcontent"))
            {
                alert("The current language does not match the language of the cut content. Before you paste, you must switch to the cut content language.");
                return false;
            }
            $ektron(top.document).find('#BottomFrameSet').find('#ek_nav_bottom')[0].contentWindow.NavIframeContainer.frames['nav_folder_area'].frames['ContentTree'].$ektron('#blockTree').modalShow();
            top.frames["ek_main"].$ektron("#targetId")[0].value = folderContext.folderId;
            top.frames["ek_main"].$ektron("#parentId")[0].value = folderContext.parentId;
            // Show confirmation Box for MultiLingual Move Content.
            // Essentially you can Move content in only language.
            // Moving content in a language, moves in all the languages.

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

        // Enable Paste Option in the Action Menu for the following folder depending upon the user permissions and clipboard settings.
        // Root Folder
        // Regular Folder
        // Site Domain Folder
        // Community Folder
        // Ecommerce Folder

        EnablePasteOption = function (permissions, folderContext) {
            var folderPermissions = permissions.Folder;
            if (location.href.indexOf("action=ViewContentByCategory") !== -1 || location.href.indexOf("action=viewcontentbycategory") !== -1) {
                switch (folderContext.folderType) {
                    // Root Folder  
                    case "5":
                        if ((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin || folderPermissions.CanAdd || folderPermissions.CanEdit) && (top.Ektron.Workarea.ClipBoard.items.length > 0 && top.Ektron.Workarea.ClipBoard.items[0].t != 3333)) {
                            actionmenu.addItem("&nbsp;<img src='images/UI/Icons/paste.png' />&nbsp;&nbsp;Paste", function () { showConfirm(); });
                        }
                        break;

                    // Content Folder  
                    case "0":
                        if ((folderPermissions.CanAddFolders || folderPermissions.CanEditFolders || folderPermissions.IsAdmin || folderPermissions.CanAdd || folderPermissions.CanEdit) && (top.Ektron.Workarea.ClipBoard.items.length > 0 && top.Ektron.Workarea.ClipBoard.items[0].t != 3333)) {
                            actionmenu.addItem("&nbsp;<img src='images/UI/Icons/paste.png' />&nbsp;&nbsp;Paste", function () { showConfirm(); });
                            return true;
                        }
                        break;

                    // Site Domain Folder  
                    case "2":
                        if (top.Ektron.Workarea.ClipBoard.items.length > 0 && top.Ektron.Workarea.ClipBoard.items[0].t != 3333) {
                            // enable paste Content
                            actionmenu.addItem("&nbsp;<img src='images/UI/Icons/paste.png' />&nbsp;&nbsp;Paste", function () { showConfirm(); });
                        }
                        break;

                    // Community Folder  
                    case "6":
                        if (top.Ektron.Workarea.ClipBoard.items.length > 0 && top.Ektron.Workarea.ClipBoard.items[0].t != 3333) {
                            actionmenu.addItem("&nbsp;<img src='images/UI/Icons/paste.png' />&nbsp;&nbsp;Paste", function () { showConfirm(); });
                        }
                        break;

                    // Ecommerce Folder  
                    case "9":
                        if (top.Ektron.Workarea.ClipBoard.items.length > 0 && top.Ektron.Workarea.ClipBoard.items[0].t == 3333) {
                            actionmenu.addItem("&nbsp;<img src='images/UI/Icons/paste.png' />&nbsp;&nbsp;Paste", function () { showConfirm(); });
                        }
                        break;
                }
            }
        };
        // Checks if the paste option is already there in the Action menu.
        hasPasteOption = function () {
            var result = true;
            for (var i = 0; i < actionmenu.data.length; i++) {
                if (actionmenu.data[i].name !== null) {
                    if (actionmenu.data[i].name.indexOf("paste.png") !== -1) {
                        result = false;
                    }
                }
            }
            return result;
        };

        var permissionarams = "checkType=" + folderContext.folderType + "&action=getPermissions&id=" + folderContext.folderId;
        var permissions;

        var permissionsObj = GetPermissions({
            context: folderContext,
            dataParams: permissionarams,
            onSuccess: function (data) {
                permissions = $ektron.extend(permissionsObj, data);
                EnablePasteOption(permissions, folderContext);
            },
            onError: function (XMLHttpRequest, textStatus, errorThrown, myContext) {
                //myContext.menu.disableContextMenu();
            }
        });
        // Checks to see if there are any items selected for copy/move
        // not in Approved state, and if so, returns false else,
        // returns true.
        checkValidation = function (obj) {
            var checkedOutCount = 0;
            $ektron.each(obj, function (i) {
                if (obj[i].innerHTML != "A") {
                    checkedOutCount = checkedOutCount + 1;
                }
            });
            return checkedOutCount;
        };

        // Sets the buffer for Moving Content if and only if the user elects 
        // to go ahead with moving content with some contents not in approved states 
        // and opts out of not publishing those contents.
        setClipBoard = function () {
            top.Ektron.Workarea.ClipBoard.items = [];
            top.Ektron.Workarea.TaxonomyClipBoard.items = [];

            var selectedContentItems = $ektron('#viewfolder_FolderDataGrid tr.selected').length;
            var contentItemWrapper = $ektron('.ektronPageContainer > table.ektronGrid tr.selected td p > input');
            var selectedItemStatus = $ektron('#viewfolder_FolderDataGrid tr.selected a[href*="#Status"]');
            var unApprovedItems = checkValidation(selectedItemStatus);

            if (selectedContentItems === 0) {
                alert(jsAlertSelectOneContent);
                return false;
            }
            else {
                // If the number items selected equals to the unapproved items, then clear the clipboard.
                if (selectedItemStatus.length === unApprovedItems) {
                    alert(jsAlertSelectNotApprovedAll);
                    return false;
                }
                else if (unApprovedItems > 0) {
                    if (confirm(jsAlertCheckedOutSelected)) {
                        // If user wants to go ahead with the action, set the clip board with the items selected with only Approved status.
                        for (i = 0; i < selectedItemStatus.length; i++) {
                            if (selectedItemStatus[i].innerHTML === "A") {
                                Ektron.ContentContextMenu.Content.SetApprovedClipBoardCopyAction('cutContent', null, contentItemWrapper[i].value);
                            }
                        }
                        // Enable Paste Option only if it not available in the Action menu.
                        if(hasPasteOption())
                        {
                            EnablePasteOption(permissions, folderContext);
                        }
                    }
                    else {
                        return false;
                    }
                }
                else {
                    // If every item selected is in the approved state, set the clipboard with the items selected.
                    Ektron.ContentContextMenu.SetClipBoard('cutContent', null, contentItemWrapper);
                    // Enable Paste Option only if it not available in the Action menu.
                    if(hasPasteOption())
                    {
                        EnablePasteOption(permissions, folderContext);
                    }
                }
            }
        };

        // Sets the buffer for copying Content if and only if the user elects 
        // to go ahead with copying content with some contents not in approved states 
        // and opts out of not publishing those contents.
        setCopyClipBoard = function () {            

            top.Ektron.Workarea.ClipBoard.items = [];
            top.Ektron.Workarea.TaxonomyClipBoard.items = [];

            var selectedContentItems = $ektron('#viewfolder_FolderDataGrid tr.selected').length;
            var contentItemWrapper = $ektron('.ektronPageContainer > table.ektronGrid tr.selected td p > input');
            var selectedItemStatus = $ektron('#viewfolder_FolderDataGrid tr.selected a[href*="#Status"]');
            var unApprovedItems = checkValidation(selectedItemStatus);
            if (selectedContentItems === 0) {
                alert(jsAlertSelectOneContentCopy);
                return false;
            }
            else {
                // If the number items selected equals to the unapproved items, then clear the clipboard.
                if (selectedItemStatus.length === unApprovedItems) {
                    alert(jsAlertSelectNotApprovedAll);
                    return false;
                }
                else if (unApprovedItems > 0) {
                    if (confirm(jsAlertCheckedOutSelectedCopy)) {
                        // If user wants to go ahead with the action, set the clip board with the items selected with only Approved status.
                        for (i = 0; i < selectedItemStatus.length; i++) {
                            if (selectedItemStatus[i].innerHTML === "A") {
                                Ektron.ContentContextMenu.Content.SetApprovedClipBoardCopyAction('copyContent', null, contentItemWrapper[i].value);
                            }
                        }
                        // Enable Paste Option only if it not available in the Action menu.
                        if(hasPasteOption())
                        {
                            EnablePasteOption(permissions, folderContext);
                        }
                    }
                    else {
                        return false;
                    }
                }
                else {
                    // If every item selected is in the approved state, set the clipboard with the items selected.
                    Ektron.ContentContextMenu.SetClipBoard('copyContent', null, contentItemWrapper);
                    // Enable Paste Option only if it not available in the Action menu.
                   if(hasPasteOption())
                    {
                        EnablePasteOption(permissions, folderContext);
                    }
                }
            }
        };

        unBlockFrames = function () {
            top.ek_nav_bottom.NavIframeContainer.nav_folder_area.ContentTree.$ektron("#blockTree").modalHide();
            $ektron('#selectMultiLingual').modalHide();
            $ektron('#moveContentModal').modalHide();
            if (top.Ektron.Workarea.isnoncontentframe === true) {
                top.frames["ek_main"].location.href = top.Ektron.Workarea.backurl;
                top.Ektron.Workarea.isnoncontentframe = false;
            }
        };

        PasteSelectedContent = function (obj, option) {
            $ektron('a.buttonClear').attr("onclick", "return false;");
            $ektron('a.buttonCheckAll').attr("onclick", "return false;");
            $ektron('a.buttonSelected').attr("onclick", "return false;");
            Ektron.ContextMenus.PasteContent(option);
            $ektron("#selectMultiLingual").modalHide();
            $ektron("#moveContentModal").modalHide();
            $ektron("#progressbar").modalShow();

        };
    }
});
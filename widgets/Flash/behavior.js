function onSilverlightError(sender, args) {
    var appSource = ""; var errorType = args.ErrorType; var iErrorCode = args.ErrorCode;
    if (sender != null && sender != 0) appSource = sender.getHost().Source;
    var errMsg = "Unhandled Error in Silverlight 2 Application " + appSource + "\nCode: " + iErrorCode + "    \nCategory: " + errorType + "       \nMessage: " + args.ErrorMessage + "     \n";
    if (errorType == "ParserError"){
        errMsg += "File: " + args.xamlFile + "     \nLine: " + args.lineNumber + "     \nPosition: " + args.charPosition + "     \n"; }
    else if (errorType == "RuntimeError"){
        if (args.lineNumber != 0)
            errMsg += "Line: " + args.lineNumber + "     \nPosition: " + args.charPosition + "     \n";
        errMsg += "MethodName: " + args.methodName + "     \n"; }
    throw new Error(errMsg);
}


if (Ektron.PFWidgets === undefined) {
	Ektron.PFWidgets = {};
}

Ektron.PFWidgets.Flash = {
    parentID: "",
    webserviceURL: "/pagebuilder/widgets/contentblock/FlashHandler.ashx",
    flashFilter: "Flash (*.swf;*.flv)|*.swf;*.flv",
    imageFilter: "Thumbnail (*.jpg;*.gif)|*.jpg;*.gif",
    setupAll: function(widgetId) {
        var localparent = $ektron("#" + widgetId);
        Ektron.PFWidgets.Flash.refreshTabs();
        Ektron.PFWidgets.Flash.Folder.ConfigFolderTree(localparent);
        Ektron.PFWidgets.Flash.Folder.ConfigThumbTree(localparent);
        var uploadbutton = localparent.find(".FlAdd");
        var uploadthumbnail = localparent.find(".thumbChange");
        var deletethumbnail = localparent.find(".thumbRemove");
        uploadbutton.attr("disabled", "disabled");
        //if video is not selected hide thumbnail contents and show something else

        var title = localparent.find(".filesource").html();
        if (title.match(/\(flv\)$/)) {
            localparent.find(".autostart").parent().parent().show();
            localparent.find(".thumbnail").parent().parent().show();
        } else {
            localparent.find(".autostart").parent().parent().hide();
            localparent.find(".thumbnail").parent().parent().hide();
        }


        var hdncontentid = localparent.find(".hdnContentId");
        if (hdncontentid.val() == "" || hdncontentid.val() == 0) {
            localparent.find(".hideThumb").show();
            localparent.find("div.SelThumb .CBfoldercontainer, div.SelThumb div.CBResults, div.SelThumb div.CBPaging").hide();
        }

        uploadbutton.click(function() {
            var seltab = localparent.find("ul.CBTabWrapper li.selected a").attr("href");
            if (seltab.indexOf("#ByFolder") > -1) {
                //clear properties pane
                Ektron.PFWidgets.Flash.updatePropsPane(this, "");
                localparent.find(".hdnContentId").val("");
                //reset uploading text
                var action = localparent.find(".uploadType");
                action.html("Uploading Video");
                //set file filter
                action.attr("data-filter", Ektron.PFWidgets.Flash.flashFilter);
                //set querystring
            } else if (seltab.indexOf("#SelThumb") > -1) {
                localparent.find(".hdnThumbFile").val("");
                var action = localparent.find(".uploadType");
                action.html("Uploading Thumbnail");
                action.attr("data-filter", Ektron.PFWidgets.Flash.imageFilter);
            }
            localparent.find("li.CBTab a[href*=Upload]").click();
        });
        uploadthumbnail.click(function() {
            var contentid = localparent.find(".hdnContentId").val();
            if (contentid != "" && contentid > 0) {
                //switch to select thumbnail tab
                localparent.find("li.CBTab a[href*='SelThumb']").click();
            }
            else {
                alert("You must select a file before you can modify the image associated with it.");
            }
        });
        deletethumbnail.click(function() {
            localparent.find(".hdnThumbFile").val("");
            localparent.find(".thumbnail").html("");
            localparent.find(".thumbChange").show();
            localparent.find(".thumbRemove").hide();
        });
    },
    getQueryString: function(uploaderID) {
        var localparent = $ektron("#" + uploaderID).parents("div.FlashWidget");
        var action = localparent.find(".uploadType");
        if (localparent.find(".hdnFolderId").val() > -1) {
            if (action.attr("data-filter") == Ektron.PFWidgets.Flash.flashFilter) {
                return Ektron.PFWidgets.Flash.getSelectedFolder(localparent);
            } else {
                return Ektron.PFWidgets.Flash.getSelectedVideo(localparent);
            }
        }
        alert('You must select a folder first.');
        return false;
    },
    getSelectedVideo: function(localparent) {
        var contid = localparent.find(".hdnContentId").val();
        var selected = localparent.find("div.SelThumb div.CBfoldercontainer span.selected");
        if (selected.length == 0) {
            selected = localparent.find("div.ByFolder div.CBfoldercontainer span.selected");
        }
        var folid = selected.attr("data-ektron-folid");
        if (folid == typeof (undefined)) {
            alert('Please select a folder to upload to before trying to upload.');
            return false;
        }

        return "thumbnailForContentID=" + contid + "&libraryFolder=" + folid;
    },
    getSelectedFolder: function(localparent) {
        var selected = localparent.find("div.ByFolder div.CBfoldercontainer span.selected");
        if (selected.length == 0) {
            alert('Please select a folder to upload to before trying to upload.');
            return false;
        }
        if (selected.length > 1) {
            alert('How did you select more than one folder? I\'m confused.');
            return false;
        }
        var folid = selected.attr("data-ektron-folid");
        if (folid == typeof (undefined)) {
            alert('Please select a folder to upload to before trying to upload.');
            return false;
        }

        return "folderid=" + folid;
    },
    getUploadFilter: function(uploaderID) {
        var localparent = $ektron("#" + uploaderID).parents("div.FlashWidget");
        var selected = localparent.find("span.uploadType");
        var filter = Ektron.PFWidgets.Flash.flashFilter;
        if (selected.length != 0) {
            filter = selected.attr("data-filter");
        }

        return filter;
    },
    UploadReturn: function(uploaderID, retval) {
        if (retval.indexOf("|") > -1) {
            Ektron.PFWidgets.Flash.updatePropsPane("#" + uploaderID, retval);
        } else {
            var pos = retval.indexOf("\"message\":");
            if (pos > -1) {
                var message = retval.substr(pos + 11);
                message = message.substr(0, message.indexOf("\""));
                alert("Error: " + message);
            } else {
                alert(retval);
            }
        }
    },
    refreshTabs: function() {
        Ektron.PFWidgets.Flash.Tabs.init();
        $ektron("#CBtabInterface").fadeIn(250);
    },
    toggleResultsPane: function() {
        $ektron(".ByFolder .CBResults").slideToggle(750);
        return false;
    },
    updatePropsPane: function(el, response) {
        response = response.split("|");
        var filetype = "not flv";
        if (response.length >= 6) {
            var localparent = $ektron(el).parents("div.FlashWidget");
            var folderid = parseInt(response[1], 10);
            var contentid = parseInt(response[2], 10);
            var title = response[3];
            var width = response[4];
            var height = parseInt(response[5], 10);
            var thumbnail = response[6];
            var thumbid = response[7];

            if (title.match(/\(flv\)$/)) {
                filetype = "flv";
            }
            //update fields, switch to properties pane
            localparent.find(".filesource").html(title);
            localparent.find(".height").val(height);
            localparent.find(".width").val(width);
            localparent.find(".hdnContentId").val(contentid);
            localparent.find(".hdnFolderId").val(folderid);
            localparent.find(".hdnThumbID").val(thumbid);
            localparent.find("li.CBTab a[href*='#Properties']").click();
            var thumbimg = localparent.find(".thumbnail");
            var thumbchg = localparent.find(".thumbChange");
            var thumbrem = localparent.find(".thumbRemove");
            var thumb = localparent.find(".hdnThumbFile");
            if (thumbnail == "None") {
                thumb.val("");
                thumbimg.html(thumbnail);
                thumbchg.show();
                thumbrem.hide();

            } else {
                thumb.val(thumbnail);
                thumbimg.html("<img alt=\"thumbnail\" style=\"width:250px; height:auto;\" src=\"" + thumbnail + "\"/>")
                thumbchg.show();
                thumbrem.show();
            }

            localparent.find(".hideThumb").hide();
            localparent.find("div.SelThumb .CBfoldercontainer, div.SelThumb div.CBResults, div.SelThumb div.CBPaging").show();
        } else {
            var localparent = $ektron(el).parents("div.FlashWidget");
            localparent.find(".filesource").html("");
            localparent.find(".height").val("");
            localparent.find(".width").val("");
            localparent.find(".thumbnail").html("None");
            localparent.find(".hdnThumbFile").val("");
            localparent.find(".thumbChange").hide();
            localparent.find(".thumbRemove").hide();
        }
        
        if (filetype == "flv") {
            localparent.find(".autostart").parent().parent().show();
            localparent.find(".thumbnail").parent().parent().show();
        } else {
            localparent.find(".autostart").parent().parent().hide();
            localparent.find(".thumbnail").parent().parent().hide();
        }
    },
    UploadImage: function(el, contentid) {
        var localparent = $ektron(el).parents("div.FlashWidget");
        var selected = localparent.find("span.uploadType");
        selected.attr("data-filter", Ektron.PFWidgets.Flash.imageFilter);
        selected.html("Uploading Thumbnail");

        var curpath = localparent.find("span.curPath");
        var folder = localparent.find("span.selected");
        var path = folder.text();
        var parents = folder.parents("ul[data-ektron-folid]");
        for (var i = 0; i < parents.length; i++) {
            path = $ektron(parents[i]).siblings("span.folder").text() + " > " + path;
        }

        var file = localparent.find(".filesource").text();
        curpath.text(path + " > " + file);

        localparent.find("li.CBTab a[href*='#Upload']").click();
        //change query param on sl
    },
    ShowAddButton: function(el, show) {
        var thisel = $ektron(el);
        var localparent = thisel.parents("div.FlashWidget");
        var button = localparent.find(".FlAdd");
        if (show) {
            var folderid = thisel.attr("data-ektron-folid");
            if (folderid == null || folderid == "") {
                return;
            }
            //send folderid to silverlight uploader and show path on upload tab
            var curpath = localparent.find("span.curPath");
            var path = thisel.text();
            var parents = thisel.parents("ul[data-ektron-folid]");
            for (var i = 0; i < parents.length; i++) {
                path = $ektron(parents[i]).siblings("span.folder").text() + " > " + path;
            }
            curpath.text(path + " (folder id: " + folderid + ")");
            //$ektron("#abcdef")[0].Content.abcdef.setQueryParams("This is the querystring I will upload with the file: folderid = " + folderid);
            button.removeAttr("disabled");
        } else {
            button.attr("disabled", "disabled");
        }
    },
    getResults: function(action, objectID, pageNum, objecttype, search, el, args) {
        var filter = args["filter"];
        var resultdiv = $ektron(args["resultDiv"]);
        var foldertreeroot = $ektron(args["folderTreeRoot"]);
        var str = Ektron.PFWidgets.Flash.createRequestObj(action, pageNum, search, objecttype, objectID);

        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.Flash.webserviceURL,
            data: { "request": str, "filter": filter },
            success: function(msg) {
                var localparent;
                var contentitems = eval("(" + msg + ")");
                if (el != "") {
                    localparent = $ektron(el).parents("div.FlashWidget");
                    Ektron.PFWidgets.Flash.parentID = $ektron(localparent).attr("id");
                }
                resultdiv.html(Ektron.PFWidgets.Flash.ContentToHtml(contentitems));

                $ektron(resultdiv).find("div.CBresult").click(function() {
                    args.onItemSelected(this);
                });
                $ektron(resultdiv).find("div.CBresult").cluetip({ positionBy: "bottomTop", cursor: 'pointer', arrows: true, leftOffset: "25px", topOffset: "20px", cluezIndex: 3500 });
                var paging = $ektron(resultdiv).parent().find(".CBPaging");
                paging.html(contentitems.paginglinks);
                paging.find("a").click(function() {
                    var page = $ektron(this).attr("pageid");
                    Ektron.PFWidgets.Flash.getResults(action, objectID, page, objecttype, search, el, args);
                });
            }
        });
        return false;
    },
    createRequestObj: function(action, pagenum, searchtext, objecttype, objectid) {
        request = {
            "action": action,
            "page": pagenum,
            "searchText": searchtext,
            "objectType": objecttype,
            "objectID": objectid
        };
        return Ektron.JSON.stringify(request);
    },
    ContentToHtml: function(contentlist) {
        var html = "";
        if (contentlist.contents === null || contentlist.contents.length === 0) {
            html = "<div class=\"CBNoresults\">No Results</div>";
        } else {
            for (var i in contentlist.contents) {
                html += "<div ";
                html += "class=\"CBresult " + ((i % 2 === 0) ? "even" : "odd") + "\" ";
                html += "rel=\"" + Ektron.PFWidgets.Flash.webserviceURL + "?detail=" + contentlist.contents[i].id + "\" ";
                html += "title=\"" + contentlist.contents[i].title + "\">";
                html += "<span class=\"title\">" + contentlist.contents[i].title + "</span>";
                html += "<span class=\"contentid\">" + contentlist.contents[i].id + "</span>";
                html += "<br class=\"clearall\" />";
                html += "</div>";
            }
        }
        return html;
    },
    Save: function() {
        //make sure any cluetips get lost
        $ektron("#cluetip, #cluetip-waitimage").remove();
        var el = $ektron("#" + Ektron.PFWidgets.Flash.parentID).find(".hdnContentId");
        if (el.length === 0 || el.length > 1) {
            alert("Error");
        } else if (el.val) {
            var RegExp = /^(\d*)$/;
            var result = x.match(RegExp);
            if (result != 0 && result != null) {
                return true;
            } else {
                alert("Please select a flash movie to play.")
            }
        }
        return false;
    },

    Tabs:
    {
        init: function() {
            $ektron(".CBTabInterface ul li.CBTab a").bind("click", function(e) {
                var parentLi = $ektron(this).parent();
                var tabsContainer = parentLi.parents(".CBTabInterface");
                var poundChar = $ektron(this).attr("href").indexOf("#") + 1;
                var targetPanelSelector = $ektron(this).attr("href").substring(poundChar);
                var targetPanel = tabsContainer.find("." + targetPanelSelector);
                parentLi.addClass("selected").siblings().removeClass("selected");
                tabsContainer.find(".Panels").hide();
                targetPanel.show();
                $ektron(this).parents(".CBEdit").find(".FlAdd").attr("disabled", "disabled");
                return false;
            }
            );
        }
    }
};

Ektron.PFWidgets.Flash.Folder = {
    DirectoryToHtml: function(directory) {
        var html = "";
        for (var i in directory.subdirectories) {
            html += "<li class=\"closed";
            if (i == directory.subdirectories.length - 1) {
                html += " last";
            }
            html += "\"><span class=\"folder\" data-ektron-canadd=\"" + directory.subdirectories[i].canadd + "\"data-ektron-folid=\"" + directory.subdirectories[i].id + "\">" +
                directory.subdirectories[i].name + "</span>";
            if (directory.subdirectories[i].haschildren) {
                html += "<ul data-ektron-canadd=\"" + directory.subdirectories[i].canadd + "\" data-ektron-folid=\"" + directory.subdirectories[i].id + "\"></ul>";
            }
            html += "</li>";
        }

        return html;
    },
    ConfigThumbTree: function(localparent) {
        Ektron.PFWidgets.Flash.Folder.ConfigGenericFolderTree({
            localParent: localparent,
            filter: "jpg,gif",
            resultDiv: "div.SelThumb div.CBResults",
            folderTreeRoot: "div.SelThumb ul.EktronFolderTree",
            openToItemID: ".hdnThumbID",
            openToFolderID: ".hdnThumbFolderPath",
            onItemSelected: function(el) {
                var el = $ektron(el);
                var CBResults = el.parent();
                var contentid = el.find(".contentid").html();
                var filename = el.find(".title").html();
                var localparent = el.parents("div.FlashWidget");
                $ektron(CBResults).find("div.CBresult").removeClass("selected");
                el.addClass("selected");
                localparent.find(".hdnThumbID").val(contentid);
                //update properties tab and switch to it
                $ektron.ajax({
                    type: "POST",
                    cache: false,
                    async: false,
                    url: Ektron.PFWidgets.Flash.webserviceURL,
                    data: { "selectedThumb": contentid },
                    success: function(msg) { //msg contains file url
                        localparent.find(".hdnThumbFile").val(msg);
                        localparent.find(".thumbnail").html("<img alt=\"thumbnail\" style=\"width:250px; height:auto;\" src=\"" + msg + "\"/>")
                        localparent.find(".thumbChange").show();
                        localparent.find(".thumbRemove").show();
                        localparent.find("li.CBTab a[href*='#Properties']").click();
                    }
                });
            }
        });
    },
    ConfigFolderTree: function(localparent) {
        Ektron.PFWidgets.Flash.Folder.ConfigGenericFolderTree({
            localParent: localparent,
            filter: "swf,flv",
            resultDiv: "div.ByFolder div.CBResults",
            folderTreeRoot: "div.ByFolder ul.EktronFolderTree",
            setHiddenFolderID: ".hdnFolderId",
            openToItemID: ".hdnContentId",
            openToFolderID: ".HiddenVideoFolderPath",
            onItemSelected: function(el) {
                var el = $ektron(el);
                var CBResults = el.parent();
                var contentid = $ektron(el).find(".contentid").html();
                var filename = $ektron(el).find(".title").html();
                var localparent = el.parents("div.FlashWidget");
                $ektron(CBResults).find("div.CBresult").removeClass("selected");
                el.addClass("selected");
                localparent.find(".hdnFolderId").val(contentid);
                //update properties tab and switch to it
                var autostartcheckbox = localparent.find(".autostart input");
                if (filename != null && filename.match(".flv")) {
                    autostartcheckbox.removeAttr("disabled");
                } else {
                    autostartcheckbox.attr("disabled", "disabled").removeAttr("checked");
                }

                localparent.find(".hideThumb").hide();
                localparent.find("div.SelThumb .CBfoldercontainer, div.SelThumb div.CBResults, div.SelThumb div.CBPaging").show();

                $ektron.ajax({
                    type: "POST",
                    cache: false,
                    async: false,
                    url: Ektron.PFWidgets.Flash.webserviceURL,
                    data: { "selectedContent": contentid },
                    success: function(msg) {
                        Ektron.PFWidgets.Flash.updatePropsPane(el, msg);
                    }
                });
            }
        });
    },
    ConfigGenericFolderTree: function(args) {
        var localparent = args["localParent"];
        var filter = args["filter"];
        var resultdiv = localparent.find(args["resultDiv"]);
        var foldertreeroot = localparent.find(args["folderTreeRoot"]);
        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.Flash.webserviceURL,
            data: { "request": Ektron.PFWidgets.Flash.createRequestObj("getchildfolders", 0, "", "folder", 0), "filter": filter },
            success: function(msg) {
                var directory = eval("(" + msg + ")");
                foldertreeroot.html(Ektron.PFWidgets.Flash.Folder.DirectoryToHtml(directory));
                foldertreeroot.treeview(
                {
                    toggle: function(index, element) {
                        var $element = $ektron(element);

                        if ($element.html() === "") {
                            $ektron.ajax(
                            {
                                type: "POST",
                                cache: false,
                                async: false,
                                url: Ektron.PFWidgets.Flash.webserviceURL,
                                data: { "request": Ektron.PFWidgets.Flash.createRequestObj("getchildfolders", 0, "", "folder", $element.attr("data-ektron-folid")), "filter": filter },
                                success: function(msg) {
                                    var directory = eval("(" + msg + ")");
                                    var el = $(Ektron.PFWidgets.Flash.Folder.DirectoryToHtml(directory));
                                    $element.append(el);
                                    foldertreeroot.treeview({ add: el });
                                    Ektron.PFWidgets.Flash.Folder.configClickAction(args);
                                }
                            });
                        }
                    }
                });
                Ektron.PFWidgets.Flash.Folder.configClickAction(args);
                Ektron.PFWidgets.Flash.Folder.openToSelectedContent(args);
            }
        });
    },
    openToSelectedContent: function(args) {
        if ('openToFolderID' in args) {
            var localparent = args["localParent"];
            var filter = args["filter"];
            var resultdiv = localparent.find(args["resultDiv"]);
            var foldertreeroot = localparent.find(args["folderTreeRoot"]);
            var folderid = localparent.find(args["openToFolderID"]).val(); //".CBEdit .HiddenVideoFolderPath"

            if (folderid != "") {
                var fid = folderid.split(',');
                if (fid.length > 0) {
                    //now use fid to open all the folders
                    var clicktarget = null;
                    for (var i = fid.length-1; i >= 0; i--) {
                        if (i !== 0) {
                            clicktarget = foldertreeroot.find("span.folder[data-ektron-folid='" + fid[i] + "']").parent().children("div.expandable-hitarea");
                        }
                        else {
                            clicktarget = foldertreeroot.find("span.folder[data-ektron-folid='" + fid[i] + "']");
                        }

                        if (clicktarget.length > 0) {
                            clicktarget.click();
                        }
                    }
                    //now scroll to the folder
                    foldertreeroot.parent().scrollTo(foldertreeroot.parent().find("span.folder[data-ektron-folid='" + fid[0] + "']"));
                }
            }

            if ('openToItemID' in args) {
                var contentid = localparent.find(args["openToItemID"]);
                if (contentid.length === 0) {
                    return true;
                }
                cid = contentid.val();

                //now select the content and scroll to it
                var citem = foldertreeroot.parents(".CBTabInterface").find(".CBResults .CBresult span.contentid:contains('" + cid + "')").parent();
                if (citem.length > 0 && cid > 0 ) {
                    $ektron(citem).addClass("selected");
                    citem.parent().scrollTo(citem);
                }
            }
        }
    },
    configClickAction: function(args) {
        var localparent = args["localParent"];
        var filter = args["filter"];
        var resultdiv = localparent.find(args["resultDiv"]);
        var foldertreeroot = localparent.find(args["folderTreeRoot"]);

        var parent = foldertreeroot.parent();
        parent.find("span[data-ektron-folid]").unbind("click").click(function() {
            localparent = $ektron(this).parents("div.FlashWidget");
            parent.find(".selected").removeClass("selected");
            $ektron(this).addClass("selected");
            var objectID = $ektron(this).attr("data-ektron-folid");
            var CanAdd =  $ektron(this).attr("data-ektron-canadd");
            if ('setHiddenFolderID' in args) {
                localparent.find(args['setHiddenFolderID']).val(objectID);
            }
            var pageNum = 0;
            var action = "getfoldercontent";
            var objecttype = "folder";
            Ektron.PFWidgets.Flash.parentID = localparent.attr("id");
            Ektron.PFWidgets.Flash.getResults(action, objectID, pageNum, objecttype, "", this, args);
            if(CanAdd == "false")
            {
             
                Ektron.PFWidgets.Flash.ShowAddButton(this, false);
            }
            else
            {
                Ektron.PFWidgets.Flash.ShowAddButton(this, true);
            }
        });
    }
};
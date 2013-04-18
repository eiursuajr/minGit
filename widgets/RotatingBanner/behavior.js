function onSilverlightError(sender, args) {
    var appSource = ""; var errorType = args.ErrorType; var iErrorCode = args.ErrorCode;
    if (sender != null && sender != 0) appSource = sender.getHost().Source;
    var errMsg = "Unhandled Error in Silverlight 2 Application " + appSource + "\nCode: " + iErrorCode + "    \nCategory: " + errorType + "       \nMessage: " + args.ErrorMessage + "     \n";
    if (errorType == "ParserError") {
        errMsg += "File: " + args.xamlFile + "     \nLine: " + args.lineNumber + "     \nPosition: " + args.charPosition + "     \n";
    }
    else if (errorType == "RuntimeError") {
        if (args.lineNumber != 0)
            errMsg += "Line: " + args.lineNumber + "     \nPosition: " + args.charPosition + "     \n";
        errMsg += "MethodName: " + args.methodName + "     \n";
    }
    throw new Error(errMsg);
}


if (Ektron.PFWidgets === undefined) {
    Ektron.PFWidgets = {};
}

Ektron.PFWidgets.Image =
{
    parentID: "",
    webserviceURL: "",

    imageFilter: "Thumbnail (*.jpg;*jpeg;*png;*.gif)|*.jpg;*.jpeg;*png;*.gif",
    setupAll: function(widgetId) {
        var localparent = $ektron("#" + widgetId);
        Ektron.PFWidgets.Image.refreshTabs();
        Ektron.PFWidgets.Image.Folder.ConfigFolderTree(localparent);
    },
    getQueryString: function(uploaderID) {
        var localparent = $ektron("#" + uploaderID).parents("div.ImageWidget");
        var action = localparent.find(".uploadType");
        if (localparent.find(".hdnFolderId").val() > -1) {
            return Ektron.PFWidgets.Image.getSelectedImage(localparent);

        }
        alert('You must select a folder first.');
        return false;
    },

    Save: function(el) {
        Ektron.PFWidgets.Image.parentID = $ektron(el).parents("div.ImageWidget").attr("id");
        var textbox = $ektron("#" + Ektron.PFWidgets.Image.parentID).find(".folderid");
        if ($ektron(textbox).val() == 0) {
            alert('Please select a collection.');
            return false;
        }
        else
            return true;
    },


    getSelectedImage: function(localparent) {
        var contid = localparent.find(".hdnContentId").val();
        selected = localparent.find("div.ByFolder div.CBfoldercontainer span.selected");
        var folid = selected.attr("data-ektron-folid");
        if (folid == typeof (undefined)) {
            alert('Please select a folder to upload before trying to upload.');
            return false;
        }

        return "thumbnailForContentID=" + contid + "&libraryFolder=" + folid + "&collectionID=" + $("#hdnCollID").val();
    },

    getUploadFilter: function(uploaderID) {
        var localparent = $ektron("#" + uploaderID).parents("div.ImageWidget");
        var selected = localparent.find("span.uploadType");
        var filter = Ektron.PFWidgets.Image.ImageFilter;
        if (selected.length != 0) {
            filter = selected.attr("data-filter");
        }

        return filter;
    },
    UploadReturn: function(uploaderID, retval) {


        if (retval.indexOf("|") > -1) {

            Ektron.PFWidgets.Image.updatePropsPane("#" + uploaderID, retval);
        }
        else {
            var pos = retval.indexOf("\"message\":");
            if (pos > -1) {
                var message = retval.substr(pos + 11);
                message = message.substr(0, message.indexOf("\""));
                alert("Error: " + message);
            }
            else {
                alert(retval);
            }
        }
    },
    refreshTabs: function() {
        Ektron.PFWidgets.Image.Tabs.init();
        $ektron("#CBtabInterface").fadeIn(250);
    },

    updatePropsPane: function(el, response) {

        response = response.split("|");

        if (response.length >= 6) {
            var localparent = $ektron(el).parents("div.ImageWidget");
            var folderid = parseInt(response[1], 10);
            var contentid = parseInt(response[2], 10);
            var title = response[3];
            var width = response[4];
            var height = parseInt(response[5], 10);
            var thumbnail = response[6];
            var thumbid = response[7];
            var imgBorder = response[6];

            //update fields, switch to properties pane
            localparent.find(".filesource").html(title);
            localparent.find(".height").val(height);
            localparent.find(".width").val(width);

            localparent.find(".imgborder").val(imgBorder);
            var toolTip = title.substring(0, title.lastIndexOf('.'))
            localparent.find(".toolTip").val(toolTip);
            localparent.find(".hdnContentId").val(contentid);
            localparent.find(".hdnFolderId").val(folderid);
            localparent.find(".hdnThumbID").val(thumbid);
            localparent.find("li.CBTab a[href*='#Properties']").click();

        }
        else {
            var localparent = $ektron(el).parents("div.ImageWidget");
            localparent.find(".filesource").html("");
            localparent.find(".height").val("");
            localparent.find(".width").val("");

        }

    },

    UploadImage: function(el, contentid) {
        var localparent = $ektron(el).parents("div.ImageWidget");
        var selected = localparent.find("span.uploadType");
        selected.attr("data-filter", Ektron.PFWidgets.Image.imageFilter);
        selected.html("Uploading Image");

        var curpath = localparent.find("span.curPath");
        var folder = localparent.find("span.selected");
        var path = folder.text();
        var parents = folder.parents("ul[data-ektron-folid]");
        for (var i = 0; i < parents.length; i++) {
            path = $ektron(parents[i]).siblings("span.folder").text() + " > " + path;
        }

        var file = localparent.find(".filesource").text();
        curpath.text(path + " > " + file);

        //change query param on sl
    },

    ShowAddButton: function(el, show) {

        var thisel = $ektron(el);
        var localparent = thisel.parents("div.ImageWidget");
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

        }

    },


    getResults: function(action, objectID, pageNum, objecttype, search, el, args) {
        var filter = args["filter"];
        var resultdiv = $ektron(args["resultDiv"]);
        var foldertreeroot = $ektron(args["folderTreeRoot"]);
        var str = Ektron.PFWidgets.Image.createRequestObj(action, pageNum, search, objecttype, objectID);

        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.Image.webserviceURL,
            data: { "request": str, "filter": filter },
            success: function(msg) {
                var localparent;
                var contentitems = eval("(" + msg + ")");
                if (el != "") {
                    localparent = $ektron(el).parents("div.ImageWidget");
                    Ektron.PFWidgets.Image.parentID = $ektron(localparent).attr("id");
                }
                resultdiv.html(Ektron.PFWidgets.Image.ContentToHtml(contentitems));

                $ektron(resultdiv).find("div.CBresult").click(function() {
                    args.onItemSelected(this);
                });
                $ektron(resultdiv).find("div.CBresult").cluetip({ positionBy: "bottomTop", cursor: 'pointer', arrows: true, leftOffset: "25px", topOffset: "20px", cluezIndex: 3500 });
                var paging = $ektron(resultdiv).parent().find(".CBPaging");
                paging.html(contentitems.paginglinks);
                paging.find("a").click(function() {
                    var page = $ektron(this).attr("pageid");
                    Ektron.PFWidgets.Image.getResults(action, objectID, page, objecttype, search, el, args);

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

                html += "rel=\"" + Ektron.PFWidgets.Image.webserviceURL + "?detail=" + contentlist.contents[i].id + "\" ";
                html += "title=\"" + contentlist.contents[i].title + "\">";
                html += "<span class=\"title\">" + contentlist.contents[i].title + "</span>";
                html += "<span class=\"contentid\">" + contentlist.contents[i].id + "</span>";
                html += "<br class=\"clearall\" />";
                html += "</div>";
            }
        }
        return html;
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

                return false;
            }
            );
        }
    }
};

Ektron.PFWidgets.Image.Folder = {
    DirectoryToHtml: function(directory) {
        var html = "";
        for (var i in directory.subdirectories) {
            html += "<li class=\"closed";
            if (i == directory.subdirectories.length - 1) {
                html += " last";
            }
            html += "\"><span class=\"folder\" data-ektron-folid=\"" + directory.subdirectories[i].id + "\">" +
                directory.subdirectories[i].name + "</span>";
            if (directory.subdirectories[i].haschildren) {
                html += "<ul data-ektron-folid=\"" + directory.subdirectories[i].id + "\"></ul>";
            }
            html += "</li>";
        }

        return html;
    },

    ConfigFolderTree: function(localparent) {

        Ektron.PFWidgets.Image.Folder.ConfigGenericFolderTree({
            localParent: localparent,
            filter: "jpg,gif,jpeg,png",
            resultDiv: "div.ByFolder div.CBResults",
            folderTreeRoot: "div.ByFolder ul.EktronFolderTree",
            setHiddenFolderID: ".hdnFolderId",
            openToItemID: ".hdnContentId",
            openToFolderID: ".hiddenFolderPath",
            onItemSelected: function(el) {
                var el = $ektron(el);
                var CBResults = el.parent();
                var contentid = $ektron(el).find(".contentid").html();
                var filename = $ektron(el).find(".title").html();
                var localparent = el.parents("div.ImageWidget");
                $ektron(CBResults).find("div.CBresult").removeClass("selected");
                el.addClass("selected");
                localparent.find(".hdnFolderId").val(contentid);

                //update properties tab and switch to it


                $ektron.ajax({
                    type: "POST",
                    cache: false,
                    async: false,
                    url: Ektron.PFWidgets.Image.webserviceURL,
                    data: { "selectedContent": contentid },
                    success: function(msg) {
                        Ektron.PFWidgets.Image.updatePropsPane(el, msg);
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
            url: Ektron.PFWidgets.Image.webserviceURL,
            data: { "request": Ektron.PFWidgets.Image.createRequestObj("getchildfolders", 0, "", "folder", 0), "filter": filter },
            success: function(msg) {
                var directory = eval("(" + msg + ")");
                foldertreeroot.html(Ektron.PFWidgets.Image.Folder.DirectoryToHtml(directory));
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
                                url: Ektron.PFWidgets.Image.webserviceURL,
                                data: { "request": Ektron.PFWidgets.Image.createRequestObj("getchildfolders", 0, "", "folder", $element.attr("data-ektron-folid")), "filter": filter },
                                success: function(msg) {
                                    var directory = eval("(" + msg + ")");
                                    var el = $(Ektron.PFWidgets.Image.Folder.DirectoryToHtml(directory));
                                    $element.append(el);
                                    foldertreeroot.treeview({ add: el });
                                    Ektron.PFWidgets.Image.Folder.configClickAction(args);
                                }
                            });
                        }
                    }
                });
                Ektron.PFWidgets.Image.Folder.configClickAction(args);
                Ektron.PFWidgets.Image.Folder.openToSelectedContent(args);
            }
        });
    },
    openToSelectedContent: function(args) {
        if ('openToFolderID' in args) {
            var localparent = args["localParent"];
            var filter = args["filter"];
            var resultdiv = localparent.find(args["resultDiv"]);
            var foldertreeroot = localparent.find(args["folderTreeRoot"]);
            var folderid = localparent.find(args["openToFolderID"]).val(); //".CBEdit .hiddenFolderPath"

            if (folderid != "") {
                var fid = folderid.split(',');
                if (fid.length > 0) {
                    //now use fid to open all the folders
                    var clicktarget = null;
                    for (var i = fid.length - 1; i >= 0; i--) {
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
                if (citem.length > 0 && cid > 0) {
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
        parent.find("span[data-ektron-folid]").unbind("click").click
            (
                function() {
                    localparent = $ektron(this).parents("div.ImageWidget");
                    parent.find(".selected").removeClass("selected");
                    $ektron(this).addClass("selected");
                    var objectID = $ektron(this).attr("data-ektron-folid");
                    if ('setHiddenFolderID' in args) {
                        localparent.find(args['setHiddenFolderID']).val(objectID);
                    }
                    var pageNum = 0;
                    var action = "getfoldercontent";
                    var objecttype = "folder";
                    Ektron.PFWidgets.Image.parentID = localparent.attr("id");
                    Ektron.PFWidgets.Image.getResults(action, objectID, pageNum, objecttype, "", this, args);
                   
                    Ektron.PFWidgets.Image.ShowAddButton(this, true);
                }
            );
    }
};

function LoadPropertiesTab(el)
 {
   
    var localparent = $ektron("#" + el);
    localparent.find("li.CBTab a[href*='#Properties']").click();
}
Ektron.ready(function () {

    if (typeof (Ektron.Widget) == "undefined") {
        Ektron.Widget = {};
    }


    if (typeof (Ektron.Widget.BrightcoveVideo) == "undefined") {
        Ektron.Widget.BrightcoveVideo = {};

    }
    Ektron.Widget.BrightcoveVideo =
        {
            // PROPERTIES
            lastPage: 0,
            maxCount: 0,
            pageSize: 5,
            pageNumber: 0,
            webserviceURL: "widgets/BrightcoveVideo/handler/BCHandler.ashx",
            sortBy: "publish_date", // options include CREATION_DATE, MODIFIED_DATE, PLAYS_TOTAL, PLAYS_TRAILING_WEEK. 
            sortOrder: "DESC",  // options include ascending (ASC) or descending (DESC).
            widgets: [],
            sitepath: "",
            enableWrite: false,

            // CLASS OBJECTS
            CreateRequestObject: function (widgetid, action, id, name, description, refid, tags, fileurl, searchterm, searchtype, searchSort, sortOrder, readToken, pagenumber) {
                request = {
                    "widgetid": widgetid,
                    "action": action,
                    "id": id,
                    "title": name,
                    "description": description,
                    "refid": refid,
                    "tags": tags,
                    "fileurl": fileurl,
                    "searchterm": searchterm,
                    "searchtype": searchtype,
                    "searchSort": searchSort,
                    "sortOrder": sortOrder,
                    "readToken": readToken,
                    "pagenumber": pagenumber
                };

                return Ektron.JSON.stringify(request);
            },

            ProcessRequest: function (widgetid, action, id, name, description, refid, tags, fileurl, searchterm, searchtype, searchSort, sortOrder, readToken, pagenumber) {
                var str = Ektron.Widget.BrightcoveVideo.CreateRequestObject(widgetid, action, id, name, description, refid, tags, fileurl, searchterm, searchtype, searchSort, sortOrder, readToken, pagenumber);

                var returnData;
                if (fileurl.length > 0) {
                    try {
                        $ektron.ajaxFileUpload({
                            url: Ektron.Widget.BrightcoveVideo.sitePath + Ektron.Widget.BrightcoveVideo.webserviceURL,
                            secureuri: false,
                            fileElementId: 'uploadInput',
                            dataType: 'json',
                            data: { "request": str },
                            success: function (data, status) {
                                $ektron('.error').show();
                                if (data.error != undefined && data.error == true) {
                                    $ektron('.error').html(data.message);
                                }
                                else {
                                    //$ektron('.error').html('Video uploaded ID: ' + data.result + ". Please note that there may be a delay in the video displaying while it is processed.");
                                    $ektron('.error').html('This video uploaded successfully! <br />The new video ID is: ' + data.result + '<br /><br />Please note that there may be a delay in the video displaying while it is being processed.');
                                    $ektron('.error').addClass('uploadSuccess');

                                    // Clear our Upload fields
                                    $ektron('.videoName').val('');
                                    $ektron('.videoDescription').val('');
                                    $ektron('.videoRef').val('');
                                    $ektron('.videoTags').val('');
                                }
                                Ektron.Widget.BrightcoveVideo.EndLoading();
                            },
                            error: function (data, status, e) {
                                var responseText = data.responseText;

                                // Clean Up our response text -- Some of these are browser specific
                                responseText = responseText.replace("<pre style=\"word-wrap: break-word; white-space: pre-wrap;\">", ""); // Chrome Browser Only
                                responseText = responseText.replace("</pre>", ""); // Chrome Browser Only
                                responseText = responseText.replace("<PRE>", "");
                                responseText = responseText.replace("</PRE>", "");

                                // Create the Error Object
                                var errorResponse = {};
                                var errorResponse = Ektron.JSON.parse(responseText);

                                $ektron('.error').show();
                                if (errorResponse.message != undefined) {
                                    $ektron('.error').html(errorResponse.message);
                                }
                                if (errorResponse.result != undefined) {
                                    //$ektron('.uploadSuccess').html('Video uploaded ID: ' + errorResponse.result + ". Please note that there may be a delay in the video displaying while it is processed.");
                                    $ektron('.error').html('This video uploaded successfully! <br />The new video ID is: ' + errorResponse.result + '<br /><br />Please note that there may be a delay in the video displaying while it is being processed.');
                                    $ektron('.error').addClass('uploadSuccess');

                                    // Clear our Upload fields
                                    $ektron('.videoName').val('');
                                    $ektron('.videoDescription').val('');
                                    $ektron('.videoRef').val('');
                                    $ektron('.videoTags').val('');
                                }
                                Ektron.Widget.BrightcoveVideo.EndLoading();
                            }
                        });
                    }
                    catch (e) {
                        alert('error e: ' + e);
                    }
                } else {
                    $ektron.ajax({
                        type: "POST",
                        cache: false,
                        async: false,
                        url: Ektron.Widget.BrightcoveVideo.sitePath + Ektron.Widget.BrightcoveVideo.webserviceURL,
                        data: { "request": str },
                        dataType: "json",
                        success: function (msg) {
                            returnData = msg;
                        }
                    });
                }
                return returnData;
            },

            StartLoading: function () {
                $ektron('.bcLoading').show();
            },

            EndLoading: function () {
                $ektron('.bcLoading').hide();
            },

            BrightcoveVideoWidget: function (id, widgetid) {
                var obj = this;
                obj.id = id;
                obj.videoID;
                obj.playerID;
                obj.videoList = [];
                obj.playlists = [];
                obj.widgetid = widgetid;

                if (id.length > 1) {

                    obj.ValidateSettings = function () {
                        var readToken = $ektron(".readtoken").val();
                        if (readToken == "") return false;
                        var responseData = Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "validate", "", "", "", "", "", "", "", "", "", "", readToken, 0);
                        if (responseData.success == true) {
                            return true;
                        } else {
                            $ektron('.tokenError').show();
                            return false;
                        }
                    };

                    obj.SearchVideos = function (pagenumber) {
                        var tbData = $ektron("#" + id + "_tbSearchInput");
                        var text = tbData.val();
                        var searchtype = $ektron("#" + id + "_ddlSearchOptions").val();
                        var searchSort = $ektron(".searchSort").val();
                        var sortOrder = $ektron(".sortOrder").val();
                        var responseData = Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "searchglobal", id, "", "", "", "", "", text, searchtype, searchSort, sortOrder, "", pagenumber);

                        $ektron('.JSONResponse').val(Ektron.JSON.stringify(responseData));

                        // Ensure none of our Playlists are selected
                        //  Brightcove currently doesn't support searching within a playlist
                        //  We don't want the UI to appear as though the search is specific to any playlist
                        $ektron('.hdnPlaylistId').val('');
                        $ektron('.playlistList li').removeClass('selectedItem');

                        obj.BindInfiniteScroll(responseData.total_count, obj.SearchVideos);
                        return responseData;
                    };

                    obj.ClearVideoCategories = function () {
                        $ektron('#videoCategory').html("");
                    };

                    obj.ClearVideoList = function () {
                        $ektron('#videolisttarget').html("");
                    };

                    obj.ClearVideoDetail = function () {
                        $ektron('#videoInfo').html("");
                    };

                    obj.ClearSelectedVideo = function () {
                        $ektron('.hdnVideoId').val("");
                    };

                    obj.ResetVideoForm = function () {
                        $ektron('.uploadWrapper input').val('');
                    };

                    obj.renderVideoList = function (responseData) {
                        if (responseData.videos) {
                            responseData.items = responseData.videos;
                        }
                        if (responseData.items.length > 0) {

                            responseData = obj.sortPlaylist(responseData);

                            for (var i = 0; i < responseData.items.length; i++) {
                                // convert milliseconds from epoch to date
                                var lastUpdated = new Date(parseInt(responseData.items[i].lastModifiedDate));
                                lastUpdated = lastUpdated.toDateString() + ", " + lastUpdated.toLocaleTimeString();
                                responseData.items[i].lastModifiedDateString = lastUpdated;

                                //Copy data to array
                                obj.videoList[responseData.items[i].id] = responseData.items[i];
                            }

                            $('#videoListtmpl').tmpl(responseData.items).appendTo('#videolisttarget');

                            $ektron('.videoListList li:odd').addClass('oddVideoRow');
                            $ektron('.videoListList li').click(function () {
                                $ektron('.videoListList li').removeClass('selectedItem');
                                $ektron(this).addClass('selectedItem');
                                obj.VideoClicked(this, id);
                            });
                        } else {
                            //$ektron('#videolisttarget').html('<li class="noresults">No Results</li>');
                        }
                    };

                    obj.sortPlaylist = function (responseData) {
                        var sortSelection = $ektron(".searchSort").val();
                        var searchSort = 'name';
                        var sortOrder = $ektron(".sortOrder").val();

                        // Map the values from the dropzone to the JSON keys we have
                        //  TODO: Figure out why these keys aren't the same names
                        if (sortSelection.toUpperCase() == 'MODIFIED_DATE') {
                            searchSort = 'lastModifiedDate';
                        } else if (sortSelection.toUpperCase() == 'PUBLISH_DATE') {
                            // We don't have Publish Keys - Use Modified Date for now
                            searchSort = 'lastModifiedDate';
                        } else if (sortSelection.toUpperCase() == 'PLAYS_TRAILING_WEEK') {
                            // We don't have Recently Played Keys - Just return for now
                            return responseData;
                        } else if (sortSelection.toUpperCase() == 'PLAYS_TOTAL') {
                            // We don't have Plays Total Keys - Just return for now
                            return responseData;
                        } else if (sortSelection.toUpperCase() == 'REFERENCE_ID') {
                            searchSort = 'referenceId';
                        }

                        // Sort our list appropriately
                        responseData.items.sort(videoSort(searchSort, sortOrder, function (a) { return a.toLowerCase() }));

                        return responseData;
                    };

                    obj.renderVideo = function (videoID) {

                        var milliseconds = obj.videoList[videoID].length;
                        var seconds = Math.floor((milliseconds / 1000) % 60);
                        if (seconds < 10) {
                            seconds = "0" + seconds;
                        }
                        var minutes = Math.floor((milliseconds / (1000 * 60)) % 60);
                        obj.videoList[videoID].duration = minutes + ":" + seconds;
                        $('#videoDetailtmpl').tmpl(obj.videoList[videoID]).appendTo('#videoInfo');

                        if (Ektron.Widget.BrightcoveVideo.enableWrite) {
                            $ektron('#videoInfo .videoDeleteLink').click(function () {
                                obj.DeleteVideo(videoID);
                            });
                            $ektron('#videoInfo .videoEditLink').click(function () {
                                obj.EditVideo(videoID);
                            });
                            $ektron('#videoInfo .videoSaveLink').click(function () {
                                obj.SaveVideo(videoID);
                            });
                            $ektron('#videoInfo .videoCancelLink').click(function () {
                                obj.CancelEdit();
                            });
                        } else {
                            $ektron('#videoInfo .videoDeleteLink').hide();
                            $ektron('#videoInfo .videoEditLink').hide();
                            $ektron('#videoInfo .videoSaveLink').hide();
                        }
                        $ektron('#videoInfo img').click(function () {
                            obj.PreviewVideo(videoID);
                        });
                    };

                    obj.VideoClicked = function (target, id) {
                        obj.ClearVideoDetail();
                        obj.ClearSelectedVideo();
                        var videoID = $ektron(target).find('.hdnVideoID').val();
                        $ektron('.hdnPlaylistId').val('');
                        $ektron('.hdnVideoId').val(videoID);
                        obj.renderVideo(videoID);
                    };

                    obj.PreviewVideo = function (videoID) {
                        Ektron.Widget.BrightcoveVideo.StartLoading();
                        $ektron('.bcLoading').unbind('click');
                        $ektron('.bcLoading').click(function () {
                            brightcove.removeExperience("previewPlayer");
                            $ektron('.previewVideoPlayer').hide();
                            Ektron.Widget.BrightcoveVideo.EndLoading();
                        });
                        $ektron('.previewVideoPlayer').html('');
                        var playerID = $ektron(".playerSelection option:selected").val();
                        var params = {};
                        params.bgcolor = "#90C9E0";
                        params.width = "480";
                        params.height = "272";
                        params.playerID = playerID;
                        params.videoId = videoID;
                        params.isVid = "true";
                        params.autoStart = "true";
                        params.isUI = "true";
                        var player = brightcove.createElement("object");
                        player.id = "previewPlayer";
                        var parameter;
                        for (var i in params) {
                            parameter = brightcove.createElement("param");
                            parameter.name = i;
                            parameter.value = params[i];
                            player.appendChild(parameter);
                        }
                        //debugger;
                        brightcove.createExperience(player, $ektron('.previewVideoPlayer')[0], true);

                        $ektron('.previewVideoPlayer').show();
                    };

                    obj.GetAllVideos = function () {
                        var responseData = obj.GetAllVideoPage(0);
                        var dataItem = { id: "allvideos", name: "All Videos", videos: responseData.items, total_count: responseData.total_count };
                        obj.playlists["allvideos"] = dataItem;
                        if (dataItem.videos.length > 0) {
                            $('#videoCategoryListtmpl').tmpl(dataItem).appendTo('#videoCategory');
                        }
                    };

                    obj.GetAllVideoPage = function (pageid) {
                        return Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "getallvideos", id, "", "", "", "", "", "", "", "", "", "", pageid);
                    };

                    obj.GetAllPlaylists = function () {
                        var responseData = Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "getallplaylists", id, "", "", "", "", "", "", "", "", "", "", 0);
                        for (var i = 0; i < responseData.items.length; i++) {
                            obj.playlists[responseData.items[i].id] = responseData.items[i];
                            responseData.items[i].total_count = responseData.items[i].videoIds.length;
                        }
                        $('#videoCategoryListtmpl').tmpl(responseData.items).appendTo('#videoCategory');
                        $ektron('.playlistList li').click(function () {
                            $ektron('.playlistList li').removeClass('selectedItem');
                            $ektron(this).addClass('selectedItem');
                            obj.PlaylistClicked(this, id);
                        });
                    };

                    obj.PlaylistClicked = function (target, id) {
                        obj.ClearVideoList();
                        obj.ClearVideoDetail();
                        obj.MaxPages = 0;

                        // Clear our Search Text.  
                        //  There's currently now way to search within a playlist.  So, remove the search term to improve usability.
                        $ektron('.searchInput').val('');

                        var playlistID = $ektron(target).find('.hdnplaylistID').val();
                        if (playlistID == "allvideos") {
                            obj.BindInfiniteScroll(obj.playlists[playlistID].total_count, obj.GetAllVideoPage);
                        }
                        $ektron('.hdnPlaylistId').val(playlistID);
                        $ektron('.hdnVideoId').val('');
                        obj.renderVideoList(obj.playlists[playlistID]);
                    };

                    obj.AddVideo = function () {
                        $ektron('.error').html('');
                        Ektron.Widget.BrightcoveVideo.StartLoading();
                        var title = $ektron(".uploadPane .videoName").val();
                        var description = $ektron(".uploadPane .videoDescription").val();
                        var refID = $ektron(".uploadPane .videoRef").val();
                        var tags = $ektron(".uploadPane .videoTags").val();
                        var fileurl = $ektron(".uploadPane .videoFile").val();

                        // Ensure we have a clean location for this video
                        //var fileStart = fileurl.lastIndexOf("\\") + 1;
                        //fileurl = fileurl.substring(fileStart);

                        if (title != "" && description != "" && fileurl != "") {
                            $ektron('.error').show();
                            $ektron('.error').html('Your video is being processed.');
                            Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "videocreate", "", title, description, refID, tags, fileurl, "", "", "", "", "", 0);
                        } else {
                            $ektron('.error').show();
                            $ektron('.error').html('Title, Description and a File are required.');
                        }

                    };


                    obj.DeleteVideo = function (id) {
                        var currentPlaylistID = $ektron('.playlistList li.selectedItem').find('.hdnplaylistID').val();

                        Ektron.Widget.BrightcoveVideo.StartLoading();
                        obj.ClearVideoDetail();
                        obj.ClearVideoCategories();
                        obj.ClearVideoList();
                        Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "videodelete", id, "", "", "", "", "", "", "", "", "", "", 0);
                        obj.GetAllVideos();
                        obj.GetAllPlaylists();
                        Ektron.Widget.BrightcoveVideo.EndLoading();

                        // If we were in a specific playlist, reload the items in that playlist
                        obj.renderVideoList(obj.playlists[currentPlaylistID]);
                        $ektron('.playlistList li').find('.hdnplaylistID[value=' + currentPlaylistID + ']').parent().addClass('selectedItem');
                        $ektron('.videoInfo').text('This video has been deleted successfully.  It may take a few minutes for this deletion to be reflected in the widget.');
                    };

                    obj.EditVideo = function (videoID) {
                        $ektron('.videoEditLink').hide();
                        $ektron('.videoDeleteLink').hide();
                        $ektron('.videoSaveLink').show();
                        $ektron('.videoCancelLink').show();
                        $ektron('.editableProperty').hide();
                        $ektron('.hiddenProperty').show();
                    };

                    obj.SaveVideo = function (videoID) {
                        Ektron.Widget.BrightcoveVideo.StartLoading();
                        obj.videoList[videoID].name = $ektron('#nameproperty').val();
                        obj.videoList[videoID].referenceId = $ektron('#referenceproperty').val();
                        obj.videoList[videoID].tags = $ektron('#tagsproperty').val();
                        var responseData = Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "videoupdate", videoID, obj.videoList[videoID].name, obj.videoList[videoID].shortDescription, obj.videoList[videoID].referenceId, obj.videoList[videoID].tags, "", "", "", "", "", "", 0);
                        $ektron('.videoSaveLink').hide();
                        $ektron('.videoEditLink').show();
                        $ektron('.videoDeleteLink').show();
                        $ektron('.videoCancelLink').hide();
                        $ektron('.hiddenProperty').hide();
                        $ektron('.editableProperty').show();
                        Ektron.Widget.BrightcoveVideo.EndLoading();
                    };

                    obj.CancelEdit = function () {
                        Ektron.Widget.BrightcoveVideo.StartLoading();
                        $ektron('.videoSaveLink').hide();
                        $ektron('.videoEditLink').show();
                        $ektron('.videoDeleteLink').show();
                        $ektron('.videoCancelLink').hide();
                        $ektron('.hiddenProperty').hide();
                        $ektron('.editableProperty').show();
                        Ektron.Widget.BrightcoveVideo.EndLoading();
                    }

                    obj.GetVideo = function (videoID) {
                        Ektron.Widget.BrightcoveVideo.StartLoading();
                        obj.ClearVideoDetail();
                        var responseData = Ektron.Widget.BrightcoveVideo.ProcessRequest(obj.widgetid, "videoread", videoID, "", "", "", "", "", "", "", "", "", "", 0);
                        if (responseData != null) {
                            obj.videoList[videoID] = responseData;
                            obj.renderVideo(videoID);
                        }
                        Ektron.Widget.BrightcoveVideo.EndLoading();
                    };

                    obj.KeyPressHandler = function (elem, event, id) {
                        if (event.keyCode == 13) {
                            if (event.preventDefault) event.preventDefault();
                            if (event.stopPropagation) event.stopPropagation();
                            event.returnValue = false;
                            event.cancel = true;
                            setTimeout('Ektron.Widget.BrightcoveVideo.widgets["' + id + '"].SearchVideos(0)', 1);
                            return false;
                        }
                    };

                    obj.ClearEmbedCode = function () {
                        $ektron('.embedPane .embedInput').val('');
                    };

                    obj.ShowQuickPublishPane = function () {
                        $ektron('.uploadPane').hide();
                        $ektron('.embedPane').hide();
                        $ektron('.quickPublish').show();
                    };

                    obj.ShowUploadPane = function () {
                        $ektron('.quickPublish').hide();
                        $ektron('.embedPane').hide();
                        $ektron('.uploadPane').show();
                    };

                    obj.ShowEmbedPane = function () {
                        $ektron('.uploadPane').hide();
                        $ektron('.quickPublish').hide();
                        $ektron('.embedPane').show();
                    };

                    obj.BindInfiniteScroll = function (resultCount, dataSourceFunction) {
                        $ektron('.videoListList').unbind("scroll resize");
                        obj.MaxPages = Math.ceil(resultCount / 15);
                        var scrollContainer = $ektron('.videoListList');
                        var videoList = $ektron('#videolisttarget');
                        videoList.data("currentPage", 1);
                        $ektron('.videoListList').bind("scroll resize", function () {
                            Ektron.Widget.BrightcoveVideo.widgets[id].checkListItemContents(scrollContainer, videoList, dataSourceFunction);
                        });
                    };

                    obj.checkListItemContents = function (container, list, dataSourceFunction) {
                        if (obj.isMoreListItemsNeeded(container, list)) {
                            obj.getMoreListItems(list, dataSourceFunction);
                        }
                    };

                    obj.isMoreListItemsNeeded = function (container, list) {
                        var scrollBuffer = 90;
                        if ((container[0].clientHeight + container[0].scrollTop + scrollBuffer) >= container[0].scrollHeight) {
                            return (true);
                        } else {
                            return (false);
                        }
                    };

                    obj.getMoreListItems = function (list, dataSourceFunction) {
                        var nextOffset = (list.data("currentPage") || 1);
                        if (nextOffset < obj.MaxPages) {
                            var loader = $("#loader");
                            loader.text("Loading New Items");
                            obj.renderVideoList(dataSourceFunction(nextOffset));
                            list.data("currentPage", (nextOffset + 1));
                            loader.text("End");
                        }
                    };
                }
            },

            // METHODS

            AddWidget: function (id, isEdit, widgetid, sitePath, enableWrite) {

                // resize if modal
                setTimeout('$ektron(".ektronWindow").css("width", "800px");', 500);

                Ektron.Widget.BrightcoveVideo.sitePath = sitePath;
                Ektron.Widget.BrightcoveVideo.enableWrite = enableWrite;

                // Load new widget
                Ektron.Widget.BrightcoveVideo.StartLoading();
                var widg = new Ektron.Widget.BrightcoveVideo.BrightcoveVideoWidget(id, widgetid);
                Ektron.Widget.BrightcoveVideo.widgets[id] = widg;

                $ektron('.help img').cluetip({
                    cluetipClass: 'jtip',
                    local: true,
                    cursor: 'pointer',
                    width: '300px',
                    sticky: true,
                    showTitle: false,
                    activation: 'hover',
                    positionBy: 'mouse',
                    dropShadow: false,
                    mouseOutClose: true,
                    closeText: '<img src="' + Ektron.Widget.BrightcoveVideo.sitePath + 'widgets/BrightcoveVideo/images/close.png" alt="close" />'
                });

                // 
                $ektron('.saveSettingsBtn').click(function () {
                    var clientValidate = Page_ClientValidate('settings');

                    if (clientValidate == true) {
                        return Ektron.Widget.BrightcoveVideo.widgets[id].ValidateSettings();
                    } else {
                        return false;
                    }
                });

                if (isEdit == true) {

                    // Attach search handler
                    $ektron("#" + id + " .executeSearch").click(function () {
                        Ektron.Widget.BrightcoveVideo.StartLoading();
                        Ektron.Widget.BrightcoveVideo.widgets[id].ClearVideoList();
                        Ektron.Widget.BrightcoveVideo.widgets[id].renderVideoList(Ektron.Widget.BrightcoveVideo.widgets[id].SearchVideos(0));
                        Ektron.Widget.BrightcoveVideo.EndLoading();
                    });
                    //Bind enter
                    $ektron('.searchInput').keydown(function (event) {
                        if (event.keyCode == "13") {
                            event.preventDefault();
                            Ektron.Widget.BrightcoveVideo.StartLoading();
                            Ektron.Widget.BrightcoveVideo.widgets[id].ClearVideoList();
                            Ektron.Widget.BrightcoveVideo.widgets[id].renderVideoList(Ektron.Widget.BrightcoveVideo.widgets[id].SearchVideos(0));
                            Ektron.Widget.BrightcoveVideo.EndLoading();
                        }
                    });
                    // Bind Sort Dropdown
                    $ektron('.searchSort').bind('change', function () {
                        var playlistID = $ektron('.hdnPlaylistId').val();

                        // If we have a playlistID, the sort should be applied to that playlist only
                        if (playlistID.length > 0 && playlistID != "allvideos") {
                            // Re-Render our playlist - with the correct sorting
                            Ektron.Widget.BrightcoveVideo.LoadSortedPlaylist(id, playlistID);
                        } else {
                            // Rerun the search now that we have a new sort value
                            Ektron.Widget.BrightcoveVideo.StartLoading();
                            Ektron.Widget.BrightcoveVideo.widgets[id].ClearVideoList();
                            Ektron.Widget.BrightcoveVideo.widgets[id].renderVideoList(Ektron.Widget.BrightcoveVideo.widgets[id].SearchVideos(0));
                            Ektron.Widget.BrightcoveVideo.EndLoading();
                            // Make sure our 'All Videos' Playlist is still selected
                            $ektron('.playlistList li:first-child').addClass('selectedItem');
                        }
                    });
                    // Bind Sort Order Dropdown
                    $ektron('.sortOrder').bind('change', function () {
                        var playlistID = $ektron('.hdnPlaylistId').val();

                        // If we have a playlistID, the sort should be applied to that playlist only
                        if (playlistID.length > 0 && playlistID != "allvideos") {
                            // Re-Render our playlist - with the correct sorting
                            Ektron.Widget.BrightcoveVideo.LoadSortedPlaylist(id, playlistID);
                        } else {
                            // Rerun the search now that we have a new sort order
                            Ektron.Widget.BrightcoveVideo.StartLoading();
                            Ektron.Widget.BrightcoveVideo.widgets[id].ClearVideoList();
                            Ektron.Widget.BrightcoveVideo.widgets[id].renderVideoList(Ektron.Widget.BrightcoveVideo.widgets[id].SearchVideos(0));
                            Ektron.Widget.BrightcoveVideo.EndLoading();
                            // Make sure our 'All Videos' Playlist is still selected
                            $ektron('.playlistList li:first-child').addClass('selectedItem');
                        }
                    });
                    //Check video input
                    $ektron('.bcSaveVideo').click(function (event) {
                        var videoId = $ektron('.hdnVideoId').val();
                        var videoPListId = $ektron('.hdnPlaylistId').val();
                        if (videoId == "" && videoPListId == "") {
                            alert("Please select a video or playlist before saving.");
                            return false;
                        }
                        if (!Page_ClientValidate('Dimensions')) {
                            return false;
                        }
                    });
                    //Set embedsubmit
                    $ektron('.embedSubmit').click(function () {
                        var embedText = $ektron(".embedPane .embedInput").val();
                        if (embedText == "") {
                            alert("please enter embed link");
                            return false;
                        }
                    });
                    //Set up menu
                    $ektron('.backLink').click(function () {
                        $ektron(".uploadPane .videoName").val("");
                        $ektron(".uploadPane .videoDescription").val("");
                        $ektron(".uploadPane .videoRef").val("");
                        $ektron(".uploadPane .videoTags").val("");
                        $ektron('.error').html("");
                        Ektron.Widget.BrightcoveVideo.widgets[id].ShowQuickPublishPane();
                    });
                    $ektron('.uploadLink').click(function () {
                        $ektron('.error').hide();
                        Ektron.Widget.BrightcoveVideo.widgets[id].ShowUploadPane();
                    });
                    $ektron('.embedLink').click(function () {
                        Ektron.Widget.BrightcoveVideo.widgets[id].ShowEmbedPane();
                    });

                    $ektron('.uploadVideoBtn').click(function () {
                        var title = $ektron(".uploadPane .videoName").val();
                        if (title == "") {
                            alert("Video Name is required");
                            return false;
                        }
                        var description = $ektron(".uploadPane .videoDescription").val();
                        if (description == "") {
                            alert("Video Description is required");
                            return false;
                        }
                        var tags = $ektron(".uploadPane .videoTags").val();
                        if (tags == "") {
                            alert("Tags are required");
                            return false;
                        }
                        var fileurl = $ektron(".uploadPane .videoFile").val();
                        if (fileurl == "") {
                            alert("File url is required");
                            return false;
                        }

                        Ektron.Widget.BrightcoveVideo.widgets[id].AddVideo();
                        return false;
                    });

                    $ektron('.bcButton').click(function () {
                        Ektron.Widget.BrightcoveVideo.EscapeHTML('.embedPane .embedInput');
                    });

                    // Get global list
                    Ektron.Widget.BrightcoveVideo.widgets[id].GetAllVideos();

                    // Get all playlists
                    Ektron.Widget.BrightcoveVideo.widgets[id].GetAllPlaylists();

                    // Load the 'All Videos' list to start with
                    Ektron.Widget.BrightcoveVideo.LoadSortedPlaylist(id, "allvideos");

                    // Ensure this list scrolls to show all videos
                    Ektron.Widget.BrightcoveVideo.widgets[id].BindInfiniteScroll(Ektron.Widget.BrightcoveVideo.widgets[id].playlists["allvideos"].total_count, Ektron.Widget.BrightcoveVideo.widgets[id].GetAllVideoPage);

                    // Highlight 'All Videos' in our list of playlists
                    $ektron('.playlistList li:first-child').addClass('selectedItem');

                    // Load selected video
                    var selectedVideo = $ektron('.hdnVideoId').val();
                    if (selectedVideo) {
                        Ektron.Widget.BrightcoveVideo.widgets[id].GetVideo(selectedVideo);
                    }
                }
                // Hide Loading
                Ektron.Widget.BrightcoveVideo.EndLoading();
            },

            LoadSortedPlaylist: function (id, playlistID) {
                Ektron.Widget.BrightcoveVideo.widgets[id].ClearVideoList();
                Ektron.Widget.BrightcoveVideo.widgets[id].ClearVideoDetail();
                Ektron.Widget.BrightcoveVideo.widgets[id].MaxPages = 0;

                Ektron.Widget.BrightcoveVideo.widgets[id].renderVideoList(Ektron.Widget.BrightcoveVideo.widgets[id].playlists[playlistID]);
            },

            GetWidget: function (id) {
                return Ektron.Widget.BrightcoveVideo.widgets[id];
            },

            EscapeHTML: function (target) {
                var tbData = $ektron(target);
                var result;
                if (tbData.length) {
                    result = tbData.val();
                    // less-thans (<)
                    result = result.replace(/\</g, '&lt;');
                    // greater-thans (>)
                    result = result.replace(/\>/g, '&gt;');
                    tbData.val(result);
                }
                return true;
            }
        };

    var videoSort = function (field, sortOrder, primer) {

        var reverseSort = false;

        sortOrder.toLowerCase() == 'desc' ? reverseSort = -1 : reverseSort = 1;
        return function (a, b) {

            // Ensure null values don't break our sort
            if (a[field] == null) {
                a[field] = '';
            }
            if (b[field] == null) {
                b[field] = '';
            }

            a = a[field];
            b = b[field];

            if (typeof (primer) != 'undefined') {
                a = primer(a);
                b = primer(b);
            }

            if (a < b) return reverseSort * -1;
            if (a > b) return reverseSort * 1;

            return 0;

        }
    }

});
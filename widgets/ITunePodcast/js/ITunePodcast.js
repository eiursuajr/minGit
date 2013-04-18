
Ektron.ready(function() {

    if (typeof (Ektron.Widget) == "undefined") {
        Ektron.Widget = {};
    }

    if (typeof (Ektron.Widget.ITunePodcast) == "undefined") {
        Ektron.Widget.ITunePodcast =
        {
            // PROPERTIES
            lastPage: 0,
            maxCount: 0,
            pageSize: 5,
            pageNumber: 0,         
            widgets: [],
            currentCallWidgetID: "",
            hdnPodcast: "",
            hdnPaneID: "",
            hdnSearchtextID: "",
            hdnSeatchTypeID: "",
            hdnSortByID: "",
            uxbtnRemoveID: "",

            // CLASS OBJECTS
            ITunePodcastWidget: function(id, outputId, submitButtonId, hdnPodcastListID, hdnPaneid, hdnSearchtextid, hdnsearchTypeid, hdnSearchordid, RemoveID) {
                var obj = this;
                obj.id = id;
                obj.submitBtn = $ektron("#" + submitButtonId);
                obj.output = $ektron("#" + outputId);
                Ektron.Widget.ITunePodcast.hdnPodcast = hdnPodcastListID;
                Ektron.Widget.ITunePodcast.hdnPaneID = hdnPaneid;
                Ektron.Widget.ITunePodcast.hdnSearchtextID = hdnSearchtextid;
                Ektron.Widget.ITunePodcast.hdnSeatchTypeID = hdnsearchTypeid;
                Ektron.Widget.ITunePodcast.hdnSortByID = hdnSearchordid;
                Ektron.Widget.ITunePodcast.uxbtnRemoveID = RemoveID;
                var token = "aa";
                if (token.length > 1) {
                    obj.ImageClicked = function(id) {
                        obj.output.attr("value", id);
                        obj.submitBtn.click();
                    };

                    obj.FindImages = function() {

                        var startIndex = Ektron.Widget.ITunePodcast.pageNumber + 1;
                        var queryUrl = obj.GetDataUrl("");

                        queryUrl += "&term=a&media=podcast&entity=podcast&limit=1000";

                        Ektron.Widget.ITunePodcast.currentCallWidgetID = obj.id;
                        obj.AppendScriptTag(queryUrl, 'searchYImage' + obj.id, 'ITunePodcastCallBackDisplayImages');

                    };

                    obj.PreviousImages = function() {
                        Ektron.Widget.ITunePodcast.pageNumber += -1;
                        obj.FindImages();
                    };

                    obj.NextImages = function() {
                        Ektron.Widget.ITunePodcast.pageNumber += 1;
                        obj.FindImages();
                    };

                    obj.FirstImages = function() {
                        Ektron.Widget.ITunePodcast.ResetPages();
                        Ektron.Widget.ITunePodcast.pageNumber = 0;
                        obj.FindImages();
                    };

                    obj.LastImages = function() {
                        Ektron.Widget.ITunePodcast.pageNumber = Ektron.Widget.ITunePodcast.lastPage;
                        obj.FindImages();
                    };

                    obj.SearchPreviousImages = function() {
                        Ektron.Widget.ITunePodcast.pageNumber += -1;
                        obj.SearchImages();
                    };

                    obj.SearchNextImages = function() {
                        Ektron.Widget.ITunePodcast.pageNumber += 1;
                        obj.SearchImages();
                    };

                    obj.SearchFirstImages = function() {
                        Ektron.Widget.ITunePodcast.ResetPages();
                        Ektron.Widget.ITunePodcast.pageNumber = 0;
                        obj.SearchImages();
                    };

                    obj.SearchLastImages = function() {
                        Ektron.Widget.ITunePodcast.pageNumber = Ektron.Widget.ITunePodcast.lastPage;
                        obj.SearchImages();
                    };

                    obj.SearchImages = function() {
                        var tbData = $ektron("#" + id + "SearchText");
                        var searchtext = tbData.val();
                        
                        
                        // for saving search text in hidden field
                        if (document.getElementById(Ektron.Widget.ITunePodcast.hdnSearchtextID) != null) {
                            if (searchtext != document.getElementById(Ektron.Widget.ITunePodcast.hdnSearchtextID).value) {
                                var hiddenColl = document.getElementById(Ektron.Widget.ITunePodcast.hdnPodcast);
                                if (hiddenColl != null)
                                    hiddenColl.value = '';
                            }
                            document.getElementById(Ektron.Widget.ITunePodcast.hdnSearchtextID).value = searchtext;
                        }

                        if (searchtext.length <= 0) {
                            return;
                        }

                        var startIndex = Ektron.Widget.ITunePodcast.pageNumber + 1;
                        var queryUrl = obj.GetDataUrl("");
                        queryUrl += "&term=" + searchtext + "&media=podcast&entity=podcast&limit=1000";

                        Ektron.Widget.ITunePodcast.currentCallWidgetID = obj.id;
                        obj.AppendScriptTag(queryUrl, 'searchYImage' + obj.id, 'ITunePodcastCallBackDisplaySearchImages');
                    };

                    obj.MakeImageThumbnail = function(thumbnailURL) {
                        var thumbnail = $ektron("<img></img>");
                        thumbnail.attr("src", thumbnailURL);
                        thumbnail.attr("class", "thumbnail");
                        return thumbnail;
                    };

                    obj.MakeImageLink = function(id, name, image) {
                        var ImageLink = $ektron("<a></a>");
                        ImageLink.attr("href", name);
                        ImageLink.attr("target", "new");
                        ImageLink.html(image);
                        return ImageLink;
                    };

                    obj.MakeImageShortDescription = function(shortDescription) {
                        var description = $ektron("<span></span><br />");
                        description.attr("class", "short-description");
                        description.html(shortDescription);
                        return description;
                    };
                    
                    obj.MakeImageDetails = function(title, collectionName, artistName) {
                        var description = $ektron("<span></span>");
                        description.attr("class", "short-description");
                        description.html("<label style='display:block; padding:0 0 6px 0;'>" + title + "</label><strong>Collection Name:</strong> " + collectionName+ "<br /><strong>Artist Name:</strong> " + artistName);
                        return description;
                    };

                    obj.DisplayImages = function(PodcastCollection) {

                        var list = $ektron("#" + obj.id + " ul.Image-list");
                        list.html("");
                        var alt = false;

                        Ektron.Widget.ITunePodcast.maxCount = PodcastCollection.resultCount;

                        if (Ektron.Widget.ITunePodcast.maxCount <= 0) {
                            Ektron.Widget.ITunePodcast.Pagingbuttons(obj.id, 0, 0);
                            return;
                        }
                        
                        var totalItemCounter=-1;
                        var itemIndex = -1;
                        $.each(PodcastCollection.results, function(podcastIdx, podcast) {
                            totalItemCounter = totalItemCounter + 1;
                            var startItemIndex=(Ektron.Widget.ITunePodcast.pageNumber * Ektron.Widget.ITunePodcast.pageSize);
                            if(totalItemCounter >= startItemIndex && totalItemCounter < (startItemIndex+Ektron.Widget.ITunePodcast.pageSize))
                            {
                                itemIndex = itemIndex + 1;
                                // Build the thumbnail url
                                var thumbnail = obj.MakeImageThumbnail(podcast.artworkUrl60);
                                // Build the photo url
                                var PodcastLink = obj.MakeImageLink('#', podcast.trackViewUrl, thumbnail.get(0));
                                var description = obj.MakeImageDetails(podcast.trackName,podcast.collectionName,podcast.artistName);
                                var PodcastID = podcast.trackId;



                                var listItem = $ektron("<li></li>");
                                if (itemIndex == 0) {
                                    listItem.attr("class", "alt1 ImageFirst clearfix");
                                }
                                else {
                                    listItem.attr("class", (alt = !alt) ? "alt1 clearfix" : "alt2 clearfix");
                                }
                                
                                listItem.append("<input id='chkSelImg" + podcast.trackId + "' class='ImageCheckList' type='checkbox' " + ITunePodcastGetStatus(podcast.trackId, podcast.trackName.replace("'",""), podcast.collectionName.replace("'",""), podcast.artistName.replace("'",""), podcast.trackViewUrl, podcast.artworkUrl60, podcast.artworkUrl100, this, Ektron.Widget.ITunePodcast.hdnPodcast) + " onclick=\"ITunePodcastAddtoCollection('" + podcast.trackId  + "','" + podcast.trackName.replace("'","")  + "','" + podcast.collectionName.replace("'","")  + "','" + podcast.artistName.replace("'","") + "','" + podcast.trackViewUrl + "','" + podcast.artworkUrl60 + "','" + podcast.artworkUrl100 + "',this,'" + Ektron.Widget.ITunePodcast.hdnPodcast + "')\" />");

                                listItem.append("<div class='ImageThumbOuter'></div>");
                                listItem.find(".ImageThumbOuter").append("<div class='ImageThumbInner'></div>");
                                listItem.find(".ImageThumbInner").append(PodcastLink.get(0));
                                listItem.append(description.get(0));
                                list.append(listItem);
                            }
                        });

                        Ektron.Widget.ITunePodcast.Pagingbuttons(obj.id, Ektron.Widget.ITunePodcast.maxCount, itemIndex);
                    };

                    obj.DisplaySearchImages = function(PodcastCollection) {
                        var list = $ektron("#" + obj.id + " ul.Image-search");
                        list.html("");
                        var alt = false;
                        Ektron.Widget.ITunePodcast.maxCount = PodcastCollection.resultCount;

                        if (Ektron.Widget.ITunePodcast.maxCount <= 0) {
                            Ektron.Widget.ITunePodcast.PagingbuttonsSearch(obj.id, 0, 0);
                            return;
                        }
                        
                        var totalItemCounter=-1;
                        var itemIndex = -1;
                        $.each(PodcastCollection.results, function(podcastIdx, podcast) {
                            totalItemCounter = totalItemCounter + 1;
                            var startItemIndex=(Ektron.Widget.ITunePodcast.pageNumber * Ektron.Widget.ITunePodcast.pageSize);
                            if(totalItemCounter >= startItemIndex && totalItemCounter < (startItemIndex+Ektron.Widget.ITunePodcast.pageSize))
                            {
                                itemIndex = itemIndex + 1;
                                // Build the thumbnail url
                                var thumbnail = obj.MakeImageThumbnail(podcast.artworkUrl60);
                                // Build the photo url
                                var PodcastLink = obj.MakeImageLink('#', podcast.trackViewUrl, thumbnail.get(0));
                                var description = obj.MakeImageDetails(podcast.trackName,podcast.collectionName,podcast.artistName);
                                var PodcastID = podcast.trackId;



                                var listItem = $ektron("<li></li>");
                                if (itemIndex == 0) {
                                    listItem.attr("class", "alt1 ImageFirst clearfix");
                                }
                                else {
                                    listItem.attr("class", (alt = !alt) ? "alt1 clearfix" : "alt2 clearfix");
                                }
                                
                                //listItem.append("<input id='chkSelImg" + photo.id + "' class='ImageCheckList' type='checkbox' " + ITunePodcastGetStatus(photo.id, photo.owner, photo.secret, photo.server, photo.farm, photo.title.replace("'", ""), photo.ispublic, photo.isfriend, photo.isfamily, this, Ektron.Widget.Flickr.hdnPodcast) + " onclick=\"ITunePodcastAddtoCollection('" + photo.id + "','" + photo.owner + "','" + photo.secret + "','" + photo.server + "','" + photo.farm + "','" + photo.title.replace("'", "") + "','" + photo.ispublic + "','" + photo.isfriend + "','" + photo.isfamily + "',this,'" + Ektron.Widget.Flickr.hdnPhoto + "');\" />");
                                listItem.append("<input id='chkSelImg" + podcast.trackId + "' class='ImageCheckList' type='checkbox' " + ITunePodcastGetStatus(podcast.trackId, podcast.trackName.replace("'",""), podcast.collectionName.replace("'",""), podcast.artistName.replace("'",""), podcast.trackViewUrl, podcast.artworkUrl60, podcast.artworkUrl100, this, Ektron.Widget.ITunePodcast.hdnPodcast) + " onclick=\"ITunePodcastAddtoCollection('" + podcast.trackId  + "','" + podcast.trackName.replace("'","")  + "','" + podcast.collectionName.replace("'","")  + "','" + podcast.artistName.replace("'","") + "','" + podcast.trackViewUrl + "','" + podcast.artworkUrl60 + "','" + podcast.artworkUrl100 + "',this,'" + Ektron.Widget.ITunePodcast.hdnPodcast + "')\" />");

                                listItem.append("<div class='ImageThumbOuter'></div>");
                                listItem.find(".ImageThumbOuter").append("<div class='ImageThumbInner'></div>");
                                listItem.find(".ImageThumbInner").append(PodcastLink.get(0));
                                listItem.append(description.get(0));
                                list.append(listItem);
                            }
                        });


                        Ektron.Widget.ITunePodcast.PagingbuttonsSearch(obj.id, Ektron.Widget.ITunePodcast.maxCount, itemIndex);
                    };

                    obj.PlayerItem = function(title, id) {
                        var item = $ektron("<li></li>");
                        item.html(title);
                        item.click(function() {
                            obj.PlayerClicked(title, id);
                        });
                        return item.get(0);
                    };

                    obj.KeyPressHandler = function(elem, event, id) {
                        if (event.keyCode == 13) {
                            if (event.preventDefault) event.preventDefault();
                            if (event.stopPropagation) event.stopPropagation();
                            event.returnValue = false;
                            event.cancel = true;
                            Ektron.Widget.ITunePodcast.ResetPages();
                            setTimeout('Ektron.Widget.ITunePodcast.widgets["' + id + '"].SearchImages()', 1);
                            return false;

                        }
                    };

                    obj.GetDataUrl = function(ReqType) {
                        var returnUrl = "";
                        returnUrl = 'http://ax.phobos.apple.com.edgesuite.net/WebObjects/MZStoreServices.woa/wa/wsSearch?1';

                        return returnUrl;
                    };

                    obj.AppendScriptTag = function(scriptSrc, scriptId, scriptCallback) {
                        // Remove any old existance of a script tag by the same name
                        var oldScriptTag = document.getElementById(scriptId);
                        if (oldScriptTag) {
                            oldScriptTag.parentNode.removeChild(oldScriptTag);
                        }
                        // Create new script tag
                        var script = document.createElement('script');
                        script.setAttribute('src', scriptSrc + '&callback=' + scriptCallback);
                        script.setAttribute('id', scriptId);
                        script.setAttribute('type', 'text/javascript');

                        // Append the script tag to the head to retrieve a JSON feed of Images
                        // NOTE: This requires that a head tag already exists in the DOM at the
                        // time this function is executed.
                        document.getElementsByTagName('head')[0].appendChild(script);
                    };

                    obj.LoadState = function() {
                        //load state

                    };

                }
                else {
                    var message = "You need to add your PublisherID, token, PlayerID provided by Brightcove.  Add these to the Workarea - Settings-Personalizations-ITunePodcast widget, the variables are PlayerID, publisherID, and token";
                    $ektron(".ektronWidgetBrightcove").html("");
                    $ektron(".ektronWidgetBrightcove").append(message);
                    return;
                }
            },

            // METHODS
            AddWidget: function(id, outputId, submitButtonId, hdnPodcastListID, hdnPaneid, hdnSearchtextid, hdnsearchTypeid, hdnSearchordid, RemoveID) {

                var widg = new Ektron.Widget.ITunePodcast.ITunePodcastWidget(id, outputId, submitButtonId, hdnPodcastListID, hdnPaneid, hdnSearchtextid, hdnsearchTypeid, hdnSearchordid, RemoveID);
                Ektron.Widget.ITunePodcast.widgets[id] = widg;
 
                // Create Image player list
                $ektron("#" + id + " .player-heading").hover(
                    function(evt) {
                        var playerHeading = $ektron(this).find("ul");
                        playerHeading.width($ektron(this).width());
                        playerHeading.show();
                    },
                    function() {
                        playerHeading.hide();
                    }
                );
                Ektron.Widget.ITunePodcast.widgets[id].FindImages();
            },

            GetWidget: function(id) {

                return Ektron.Widget.ITunePodcast.widgets[id];
            },

            Pagingbuttons: function(id, maxcount, items) {
                var numpages = 0;
                var theresults = "Results";
                var pagestart = 0;
                var pageend = parseInt(items);

                if (maxcount > 0) {
                    numpages = parseInt((maxcount - 1) / Ektron.Widget.ITunePodcast.pageSize);
                }
                if (maxcount > Ektron.Widget.ITunePodcast.pageSize) {

                    $ektron("#" + id + "First").css('display', '');
                    $ektron("#" + id + "Previous").css('display', '');
                    $ektron("#" + id + "Next").css('display', '');
                    $ektron("#" + id + "Last").css('display', '');

                }
                else {
                    $ektron("#" + id + "First").css('display', 'none');
                    $ektron("#" + id + "Previous").css('display', 'none');
                    $ektron("#" + id + "Next").css('display', 'none');
                    $ektron("#" + id + "Last").css('display', 'none');
                }
                Ektron.Widget.ITunePodcast.lastPage = numpages;
                if (Ektron.Widget.ITunePodcast.pageNumber == 0) {
                    $ektron("#" + id + "First").attr("disabled", true).addClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "Previous").attr("disabled", true).addClass("ektronWidgetFKPreviousDisabled");
                }
                else {
                    $ektron("#" + id + "First").attr("disabled", false).removeClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "Previous").attr("disabled", false).removeClass("ektronWidgetFKPreviousDisabled");
                }

                if (Ektron.Widget.ITunePodcast.pageNumber < numpages) {
                    $ektron("#" + id + "Next").attr("disabled", false).removeClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "Last").attr("disabled", false).removeClass("ektronWidgetFKLastDisabled");
                }
                else {
                    $ektron("#" + id + "Next").attr("disabled", true).addClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "Last").attr("disabled", true).addClass("ektronWidgetFKLastDisabled"); ;
                }
                if (maxcount > 0) {
                    pagestart = (Ektron.Widget.ITunePodcast.pageNumber * Ektron.Widget.ITunePodcast.pageSize) + 1;
                    pageend = pageend + pagestart;
                    theresults = "Results " + pagestart + " - " + pageend + " of " + maxcount;
                } else {
                    theresults = "No Results";
                }
                $ektron(".Image-result").html("");
                $ektron(".Image-result").append(theresults);

            },

            PagingbuttonsSearch: function(id, maxcount, items) {
                var numpages = 0;
                var theresults = "Results";
                var pagestart = 0;
                var pageend = parseInt(items);
                if (maxcount > 0) {
                    numpages = parseInt((maxcount - 1) / Ektron.Widget.ITunePodcast.pageSize);
                }

                if (maxcount > Ektron.Widget.ITunePodcast.pageSize)  // first Page check
                {
                    $ektron("#" + id + "FirstSearch").css('display', '');
                    $ektron("#" + id + "PreviousSearch").css('display', '');
                    $ektron("#" + id + "NextSearch").css('display', '');
                    $ektron("#" + id + "LastSearch").css('display', '');

                }
                else {
                    $ektron("#" + id + "FirstSearch").css('display', 'none');
                    $ektron("#" + id + "PreviousSearch").css('display', 'none');
                    $ektron("#" + id + "NextSearch").css('display', 'none');
                    $ektron("#" + id + "LastSearch").css('display', 'none');

                }
                Ektron.Widget.ITunePodcast.lastPage = numpages;
                if (Ektron.Widget.ITunePodcast.pageNumber == 0) {
                    $ektron("#" + id + "FirstSearch").attr("disabled", true).addClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "PreviousSearch").attr("disabled", true).addClass("ektronWidgetFKPreviousDisabled");
                }
                else {
                    $ektron("#" + id + "FirstSearch").attr("disabled", false).removeClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "PreviousSearch").attr("disabled", false).removeClass("ektronWidgetFKPreviousDisabled");
                }

                if (Ektron.Widget.ITunePodcast.pageNumber < numpages) {
                    $ektron("#" + id + "NextSearch").attr("disabled", false).removeClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "LastSearch").attr("disabled", false).removeClass("ektronWidgetFKLastDisabled");
                }
                else {
                    $ektron("#" + id + "NextSearch").attr("disabled", true).addClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "LastSearch").attr("disabled", true).addClass("ektronWidgetFKLastDisabled");
                }

                if (maxcount > 0) {
                    pagestart = (Ektron.Widget.ITunePodcast.pageNumber * Ektron.Widget.ITunePodcast.pageSize) + 1;
                    pageend = pageend + pagestart;
                    theresults = "Results " + pagestart + " - " + pageend + " of " + maxcount;
                } else {
                    theresults = "No Results";
                }

                $ektron(".Image-search-result").html("");
                $ektron(".Image-search-result").append(theresults);
            },

            SwitchPane: function(el, panename) {

                var parent = $ektron(el).parents(".ITunePodcast");
                var tablist = parent.find(".ektronWidgetFKTabs li a");
                var panes = parent.children(".pane");

                for (var i = 0; i < tablist.length; i++) {
                    $ektron(tablist[i]).removeClass("selectedTab");
                    if (tablist[i].id == panename) 
                    {
                        $ektron(tablist[i]).addClass("selectedTab");
                    }
                }

                for (var i = 0; i < panes.length; i++) {
                    $ektron(panes[i]).hide();
                    if ($ektron(panes[i]).hasClass(panename)) $ektron(panes[i]).show();
                }

                Ektron.Widget.ITunePodcast.ResetPages();

                if (document.getElementById(Ektron.Widget.ITunePodcast.uxbtnRemoveID) != null) {
                    if (panename == 'Collection') {
                        document.getElementById(Ektron.Widget.ITunePodcast.uxbtnRemoveID).style.display = 'block';
                    }
                    else {
                        document.getElementById(Ektron.Widget.ITunePodcast.uxbtnRemoveID).style.display = 'none';
                    }
                }
                var hiddenColl = document.getElementById(Ektron.Widget.ITunePodcast.hdnPodcast);
                
                
                //remove temp selected items
                if (hiddenColl != null)
                    hiddenColl.value = '';
                    
                //uncheck temp selected items    
                var inpItems = parent[0].getElementsByTagName('input');                
                for(i=0; i <inpItems.length; i++){
                    var oInp = inpItems[i];
                    
                    if(oInp.id.indexOf('chkSelImg') != -1 || oInp.id.indexOf('chkSelSearchImg') != -1){
                        oInp.checked = false;
                    }
                }

            },
            ResetPages: function() {
                Ektron.Widget.ITunePodcast.pageNumber = 0;
                Ektron.Widget.ITunePodcast.maxCount = 0;
                Ektron.Widget.ITunePodcast.lastPage = 0;
            }

        };
    }
});


function ITunePodcastCallBackDisplayImages(data) {
    var objW = Ektron.Widget.ITunePodcast.GetWidget(Ektron.Widget.ITunePodcast.currentCallWidgetID);
    if (objW) {
        objW.DisplayImages(data);
    }
}

function ITunePodcastCallBackDisplaySearchImages(data) {
    var objW = Ektron.Widget.ITunePodcast.GetWidget(Ektron.Widget.ITunePodcast.currentCallWidgetID);
    if (objW) {
        objW.DisplaySearchImages(data);
    }
}

function ITunePodcastAddtoCollection(trackId, trackName, collectionName, artistName, trackViewUrl, artworkUrl60, artworkUrl100, IdCheckbox, hdnID) {
    var hiddenColl = document.getElementById(hdnID);
    var podcastdetails = trackId + '|' + trackName + '|' + collectionName + '|' + artistName + '|' + trackViewUrl + '|' + artworkUrl60 + '|' + artworkUrl100;
    
    if (IdCheckbox.checked) {
        if (hiddenColl.value.indexOf(podcastdetails) < 0) {
            hiddenColl.value = hiddenColl.value + '~' + podcastdetails;
        }
    }
    else {
        hiddenColl.value = hiddenColl.value.toString().replace('~' + podcastdetails, '');
    }

}
function ITunePodcastGetStatus(trackId, trackName, collectionName, artistName, trackViewUrl, artworkUrl60, artworkUrl100, IdCheckbox, hdnID) {
    var hiddenColl = document.getElementById(hdnID);
    var podcastdetails = trackId + '|' + trackName + '|' + collectionName + '|' + artistName + '|' + trackViewUrl + '|' + artworkUrl60 + '|' + artworkUrl100;
    
    if (hiddenColl.value.indexOf(podcastdetails) <= 0)
        return '';
    else
        return 'checked = checked';

}

function AllowOnlyNumeric(e) {
    var key;
    // Get the ASCII value of the key that the user entered
    if (navigator.appName.lastIndexOf("Microsoft Internet Explorer") > -1)
        key = e.keyCode;
    else
        key = e.which;

    if ((key == 0 || key == 8 || key == 9))
        return true;
    // Verify if the key entered was a numeric character (0-9) or a decimal (.)
    if ((key > 47 && key < 58))
    // If it was, then allow the entry to continue
        return true;
    else { // If it was not, then dispose the key and continue with entry
        e.returnValue = null;
        return false;
    }
}

function HideRemove() {
    document.getElementById('helptext').style.display = 'none';
}
function ShowRemove() {
    document.getElementById('helptext').style.display = 'block';
}

function listStyle() {
    var list = null;

    list = document.getElementById("ulSelected");
    if (list != null) {
        DragDrop.makeListContainer(list, 'g1');
        //list.onDragOver = function() { this.style["background"] = "#cdcfc2"; };
        list.onDragOut = function() { this.style["background"] = "none"; };
    }


};

function getSort(ListID) {
    order = document.getElementById(ListID);
    if (order != null)
        order.value = DragDrop.serData('g1', null);
}

function MouseClickEvent() {
    // Disable cut ,copy and paste in propeties tab textboxes
    return false;
}


function InitDragDrop() {
    list = document.getElementById("ulSelected");
    if (list != null && DragDrop != null) {
        DragDrop.firstContainer = DragDrop.lastContainer = list;
        list.previousContainer = null;
        list.nextContainer = null;

    }
}

function ValidateItunePodcastCollection(hdnCollectionId, ListID)
{
    if( document.getElementById(hdnCollectionId).value == '0')
    {
        alert("There is no podcast in the collection...");
        return false;
    }
    getSort(ListID);
    return true;
}

Ektron.ready(function() {

    if (typeof (Ektron.Widget) == "undefined") {
        Ektron.Widget = {};
    }

    if (typeof (Ektron.Widget.Flickr) == "undefined") {
        Ektron.Widget.Flickr =
        {
            // PROPERTIES
            lastPage: 0,
            maxCount: 0,
            pageSize: 5,
            pageNumber: 0,
            sortBy: "published", // options include CREATION_DATE, MODIFIED_DATE, PLAYS_TOTAL, PLAYS_TRAILING_WEEK. 
            sortOrder: "DESC",  // options include ascending (ASC) or descending (DESC).            
            widgets: [],
            currentCallWidgetID: "",
            hdnPhoto: "",
            App_key: "",
            hdnPaneID: "",
            hdnSearchtextID: "",
            hdnSeatchTypeID: "",
            hdnSortByID: "",
            uxbtnRemoveID: "",

            // CLASS OBJECTS
            FlickrWidget: function(id, outputId, submitButtonId, AppKey, hdnPhotoListID, hdnPaneid, hdnSearchtextid, hdnsearchTypeid, hdnSearchordid, RemoveID) {
                var obj = this;
                obj.id = id;
                obj.submitBtn = $ektron("#" + submitButtonId);
                obj.output = $ektron("#" + outputId);
                Ektron.Widget.Flickr.App_key = AppKey;
                Ektron.Widget.Flickr.hdnPhoto = hdnPhotoListID;
                Ektron.Widget.Flickr.hdnPaneID = hdnPaneid;
                Ektron.Widget.Flickr.hdnSearchtextID = hdnSearchtextid;
                Ektron.Widget.Flickr.hdnSeatchTypeID = hdnsearchTypeid;
                Ektron.Widget.Flickr.hdnSortByID = hdnSearchordid;
                Ektron.Widget.Flickr.uxbtnRemoveID = RemoveID;
                var token = "aa";
                if (token.length > 1) {
                    obj.ImageClicked = function(id) {
                        obj.output.attr("value", id);
                        obj.submitBtn.click();
                    };

                    obj.FindImages = function() {

                        //                        var hiddenColl = document.getElementById(Ektron.Widget.Flickr.hdnPhoto);
                        //                        if (hiddenColl != null)
                        //                            hiddenColl.value = '';
                        var oSort_by = '';
                        Ektron.Widget.Flickr.sortBy = 'flickr.photos.getRecent';
                        if ($ektron("#" + id + "sort_by")[0] != null) {
                            oSort_by = $ektron("#" + id + "sort_by")[0];
                            Ektron.Widget.Flickr.sortBy = oSort_by.options[oSort_by.selectedIndex].value;
                        }
                        if (document.getElementById(Ektron.Widget.Flickr.hdnSortByID) != null) {
                            if (Ektron.Widget.Flickr.sortBy != document.getElementById(Ektron.Widget.Flickr.hdnSortByID).value) {
                                var hiddenColl = document.getElementById(Ektron.Widget.Flickr.hdnPhoto);
                                if (hiddenColl != null)
                                    hiddenColl.value = '';
                            }
                            document.getElementById(Ektron.Widget.Flickr.hdnSortByID).value = Ektron.Widget.Flickr.sortBy;

                        }


                        var startIndex = Ektron.Widget.Flickr.pageNumber + 1;
                        var queryUrl = obj.GetDataUrl("");

                        queryUrl += "&per_page=" + Ektron.Widget.Flickr.pageSize + "&page=" + startIndex;
                        queryUrl += "&sort=date-posted-desc";
                        queryUrl += "&method=" + Ektron.Widget.Flickr.sortBy;

                        Ektron.Widget.Flickr.currentCallWidgetID = obj.id;
                        obj.AppendScriptTag(queryUrl, 'searchYImage' + obj.id, 'callBackDisplayImages');

                    };

                    obj.PreviousImages = function() {
                        Ektron.Widget.Flickr.pageNumber += -1;
                        obj.FindImages();
                    };

                    obj.NextImages = function() {
                        Ektron.Widget.Flickr.pageNumber += 1;
                        obj.FindImages();
                    };

                    obj.FirstImages = function() {
                        Ektron.Widget.Flickr.ResetPages();
                        Ektron.Widget.Flickr.pageNumber = 0;
                        obj.FindImages();
                    };

                    obj.LastImages = function() {
                        Ektron.Widget.Flickr.pageNumber = Ektron.Widget.Flickr.lastPage;
                        obj.FindImages();
                    };

                    obj.SearchPreviousImages = function() {
                        Ektron.Widget.Flickr.pageNumber += -1;
                        obj.SearchImages();
                    };

                    obj.SearchNextImages = function() {
                        Ektron.Widget.Flickr.pageNumber += 1;
                        obj.SearchImages();
                    };

                    obj.SearchFirstImages = function() {
                        Ektron.Widget.Flickr.ResetPages();
                        Ektron.Widget.Flickr.pageNumber = 0;
                        obj.SearchImages();
                    };

                    obj.SearchLastImages = function() {
                        Ektron.Widget.Flickr.pageNumber = Ektron.Widget.Flickr.lastPage;
                        obj.SearchImages();
                    };

                    obj.SearchImages = function() {

                        var tbData = $ektron("#" + id + "SearchText");
                        var searchtext = tbData.val();

                        // for saving search text in hidden field
                        if (document.getElementById(Ektron.Widget.Flickr.hdnSearchtextID) != null) {
                            if (searchtext != document.getElementById(Ektron.Widget.Flickr.hdnSearchtextID).value) {
                                var hiddenColl = document.getElementById(Ektron.Widget.Flickr.hdnPhoto);
                                if (hiddenColl != null)
                                    hiddenColl.value = '';
                            }
                            document.getElementById(Ektron.Widget.Flickr.hdnSearchtextID).value = searchtext;
                        }

                        if (searchtext.length <= 0) {
                            return;
                        }
                        var searchtype = $ektron("#" + id + "searchtype").val();
                        // for saving search type in hidden field
                        if (document.getElementById(Ektron.Widget.Flickr.hdnSeatchTypeID) != null) {
                            if (searchtype != document.getElementById(Ektron.Widget.Flickr.hdnSeatchTypeID).value) {
                                var hiddenColl = document.getElementById(Ektron.Widget.Flickr.hdnPhoto);
                                if (hiddenColl != null)
                                    hiddenColl.value = '';
                            }
                            document.getElementById(Ektron.Widget.Flickr.hdnSeatchTypeID).value = searchtype;
                        }

                        var startIndex = Ektron.Widget.Flickr.pageNumber + 1;
                        var queryUrl = obj.GetDataUrl("");
                        queryUrl += "&per_page=" + Ektron.Widget.Flickr.pageSize + "&page=" + startIndex;


                        if (searchtype == "TAG") {
                            queryUrl += '&tags=' + searchtext;
                        } else {
                            queryUrl += "&text=" + searchtext;
                        }
                        queryUrl += "&method=flickr.photos.search";


                        Ektron.Widget.Flickr.currentCallWidgetID = obj.id;
                        obj.AppendScriptTag(queryUrl, 'searchYImage' + obj.id, 'callBackDisplaySearchImages');
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
                        var description = $ektron("<span></span>");
                        description.attr("class", "short-description");
                        description.html(shortDescription);
                        return description;
                    };

                    obj.DisplayImages = function(ImageCollection) {

                        var list = $ektron("#" + obj.id + " ul.Image-list");
                        list.html("");
                        var alt = false;

                        Ektron.Widget.Flickr.maxCount = ImageCollection.photos.total;

                        if (Ektron.Widget.Flickr.maxCount <= 0) {
                            Ektron.Widget.Flickr.Pagingbuttons(obj.id, 0, 0);
                            return;
                        }

                        var itemIndex = -1;
                        $.each(ImageCollection.photos.photo, function(photoIdx, photo) {
                            itemIndex = itemIndex + 1;
                            // Build the thumbnail url
                            var thumbnail = obj.MakeImageThumbnail(["http://farm", photo.farm, ".static.flickr.com/", photo.server, "/", photo.id, "_", photo.secret, "_t.jpg"].join(""));
                            // Build the photo url
                            var ImageLink = obj.MakeImageLink('#', ["http://www.flickr.com/photos/", photo.owner, "/", photo.id].join(""), thumbnail.get(0));
                            var description = obj.MakeImageShortDescription(photo.title.replace("'", ""));
                            var ImageID = photo.id;



                            var listItem = $ektron("<li></li>");
                            if (itemIndex == 0) {
                                listItem.attr("class", "alt1 ImageFirst clearfix");
                            }
                            else {
                                listItem.attr("class", (alt = !alt) ? "alt1 clearfix" : "alt2 clearfix");
                            }
                            listItem.append("<input id='chkSelImg" + photo.id + "' class='ImageCheckList' type='checkbox' " + GetStatus(photo.id, photo.owner, photo.secret, photo.server, photo.farm, photo.title.replace("'", ""), photo.ispublic, photo.isfriend, photo.isfamily, this, Ektron.Widget.Flickr.hdnPhoto) + " onclick=\"AddtoCollection('" + photo.id + "','" + photo.owner + "','" + photo.secret + "','" + photo.server + "','" + photo.farm + "','" + photo.title.replace("'", "") + "','" + photo.ispublic + "','" + photo.isfriend + "','" + photo.isfamily + "',this,'" + Ektron.Widget.Flickr.hdnPhoto + "');\" />");

                            listItem.append("<div class='ImageThumbOuter'></div>");
                            listItem.find(".ImageThumbOuter").append("<div class='ImageThumbInner'></div>");
                            listItem.find(".ImageThumbInner").append(ImageLink.get(0));
                            listItem.append(description.get(0));
                            list.append(listItem);
                        });

                        Ektron.Widget.Flickr.Pagingbuttons(obj.id, Ektron.Widget.Flickr.maxCount, itemIndex);
                    };

                    obj.DisplaySearchImages = function(ImageCollection) {
                        var list = $ektron("#" + obj.id + " ul.Image-search");
                        list.html("");
                        var alt = false;
                        Ektron.Widget.Flickr.maxCount = ImageCollection.photos.total;

                        if (Ektron.Widget.Flickr.maxCount <= 0) {
                            Ektron.Widget.Flickr.PagingbuttonsSearch(obj.id, 0, 0);
                            return;
                        }

                        var itemIndex = -1;
                        $.each(ImageCollection.photos.photo, function(photoIdx, photo) {
                            itemIndex = itemIndex + 1;
                            // Build the thumbnail url
                            var thumbnail = obj.MakeImageThumbnail(["http://farm", photo.farm, ".static.flickr.com/", photo.server, "/", photo.id, "_", photo.secret, "_t.jpg"].join(""));
                            // Build the photo url
                            var ImageLink = obj.MakeImageLink('#', ["http://www.flickr.com/photos/", photo.owner, "/", photo.id].join(""), thumbnail.get(0));
                            var description = obj.MakeImageShortDescription(photo.title.replace("'", ""));
                            var ImageID = photo.id;



                            var listItem = $ektron("<li></li>");
                            if (itemIndex == 0) {
                                listItem.attr("class", "alt1 ImageFirst clearfix");
                            }
                            else {
                                listItem.attr("class", (alt = !alt) ? "alt1 clearfix" : "alt2 clearfix");
                            }
                            listItem.append("<input id='chkSelSearchImg" + photo.id + "' class='ImageCheckList' type='checkbox' " + GetStatus(photo.id, photo.owner, photo.secret, photo.server, photo.farm, photo.title.replace("'", ""), photo.ispublic, photo.isfriend, photo.isfamily, this, Ektron.Widget.Flickr.hdnPhoto) + " onclick=\"AddtoCollection('" + photo.id + "','" + photo.owner + "','" + photo.secret + "','" + photo.server + "','" + photo.farm + "','" + photo.title.replace("'", "") + "','" + photo.ispublic + "','" + photo.isfriend + "','" + photo.isfamily + "',this,'" + Ektron.Widget.Flickr.hdnPhoto + "');\" />");

                            listItem.append("<div class='ImageThumbOuter'></div>");
                            listItem.find(".ImageThumbOuter").append("<div class='ImageThumbInner'></div>");
                            listItem.find(".ImageThumbInner").append(ImageLink.get(0));
                            listItem.append(description.get(0));
                            list.append(listItem);
                        });


                        Ektron.Widget.Flickr.PagingbuttonsSearch(obj.id, Ektron.Widget.Flickr.maxCount, itemIndex);
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
                            Ektron.Widget.Flickr.ResetPages();
                            setTimeout('Ektron.Widget.Flickr.widgets["' + id + '"].SearchImages()', 1);
                            return false;

                        }
                    };

                    obj.GetDataUrl = function(ReqType) {
                        var returnUrl = "";
                        var api_key = "";
                        //                        if(document.getElementById(App_key)!=null)
                        //                        {
                        //                        api_key=document.getElementById(App_key).value;
                        //                        }

                        api_key = Ektron.Widget.Flickr.App_key;

                        returnUrl = 'http://api.flickr.com/services/rest?api_key=' + api_key;

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
                        script.setAttribute('src', scriptSrc + '&format=json&jsoncallback=' + scriptCallback);
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
                    var message = "You need to add your PublisherID, token, PlayerID provided by Brightcove.  Add these to the Workarea - Settings-Personalizations-Flickr widget, the variables are PlayerID, publisherID, and token";
                    $ektron(".ektronWidgetBrightcove").html("");
                    $ektron(".ektronWidgetBrightcove").append(message);
                    return;
                }
            },

            // METHODS
            AddWidget: function(id, outputId, submitButtonId, AppKey, hdnPhotoListID, hdnPaneid, hdnSearchtextid, hdnsearchTypeid, hdnSearchordid, RemoveID) {

                var widg = new Ektron.Widget.Flickr.FlickrWidget(id, outputId, submitButtonId, AppKey, hdnPhotoListID, hdnPaneid, hdnSearchtextid, hdnsearchTypeid, hdnSearchordid, RemoveID);
                Ektron.Widget.Flickr.widgets[id] = widg;
 
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

                Ektron.Widget.Flickr.widgets[id].FindImages();
                //loadwidgetstate();
            },

            GetWidget: function(id) {

                return Ektron.Widget.Flickr.widgets[id];
            },

            Pagingbuttons: function(id, maxcount, items) {
                var numpages = 0;
                var theresults = "Results";
                var pagestart = 0;
                var pageend = parseInt(items);

                if (maxcount > 0) {
                    numpages = parseInt((maxcount - 1) / Ektron.Widget.Flickr.pageSize);
                }
                if (maxcount > Ektron.Widget.Flickr.pageSize) {

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
                Ektron.Widget.Flickr.lastPage = numpages;
                if (Ektron.Widget.Flickr.pageNumber == 0) {
                    $ektron("#" + id + "First").attr("disabled", true).addClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "Previous").attr("disabled", true).addClass("ektronWidgetFKPreviousDisabled");
                }
                else {
                    $ektron("#" + id + "First").attr("disabled", false).removeClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "Previous").attr("disabled", false).removeClass("ektronWidgetFKPreviousDisabled");
                }

                if (Ektron.Widget.Flickr.pageNumber < numpages) {
                    $ektron("#" + id + "Next").attr("disabled", false).removeClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "Last").attr("disabled", false).removeClass("ektronWidgetFKLastDisabled");
                }
                else {
                    $ektron("#" + id + "Next").attr("disabled", true).addClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "Last").attr("disabled", true).addClass("ektronWidgetFKLastDisabled"); ;
                }
                if (maxcount > 0) {
                    pagestart = (Ektron.Widget.Flickr.pageNumber * Ektron.Widget.Flickr.pageSize) + 1;
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
                    numpages = parseInt((maxcount - 1) / Ektron.Widget.Flickr.pageSize);
                }

                if (maxcount > Ektron.Widget.Flickr.pageSize)  // first Page check
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
                Ektron.Widget.Flickr.lastPage = numpages;
                if (Ektron.Widget.Flickr.pageNumber == 0) {
                    $ektron("#" + id + "FirstSearch").attr("disabled", true).addClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "PreviousSearch").attr("disabled", true).addClass("ektronWidgetFKPreviousDisabled");
                }
                else {
                    $ektron("#" + id + "FirstSearch").attr("disabled", false).removeClass("ektronWidgetFKFirstDisabled");
                    $ektron("#" + id + "PreviousSearch").attr("disabled", false).removeClass("ektronWidgetFKPreviousDisabled");
                }

                if (Ektron.Widget.Flickr.pageNumber < numpages) {
                    $ektron("#" + id + "NextSearch").attr("disabled", false).removeClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "LastSearch").attr("disabled", false).removeClass("ektronWidgetFKLastDisabled");
                }
                else {
                    $ektron("#" + id + "NextSearch").attr("disabled", true).addClass("ektronWidgetFKNextDisabled");
                    $ektron("#" + id + "LastSearch").attr("disabled", true).addClass("ektronWidgetFKLastDisabled");
                }

                if (maxcount > 0) {
                    pagestart = (Ektron.Widget.Flickr.pageNumber * Ektron.Widget.Flickr.pageSize) + 1;
                    pageend = pageend + pagestart;
                    theresults = "Results " + pagestart + " - " + pageend + " of " + maxcount;
                } else {
                    theresults = "No Results";
                }

                $ektron(".Image-search-result").html("");
                $ektron(".Image-search-result").append(theresults);
            },

            SwitchPane: function(el, panename) {

                var parent = $ektron(el).parents(".Flickr");
                var tablist = parent.find(".ektronWidgetFKTabs li a");
                var panes = parent.children(".pane");

                for (var i = 0; i < tablist.length; i++) {
                    $ektron(tablist[i]).removeClass("selectedTab");
                    if (tablist[i].id == panename) $ektron(tablist[i]).addClass("selectedTab");
                }

                for (var i = 0; i < panes.length; i++) {
                    $ektron(panes[i]).hide();
                    if ($ektron(panes[i]).hasClass(panename)) $ektron(panes[i]).show();
                }

                Ektron.Widget.Flickr.ResetPages();

                if (document.getElementById(Ektron.Widget.Flickr.uxbtnRemoveID) != null) {
                    if (panename == 'Collection') {
                        document.getElementById(Ektron.Widget.Flickr.uxbtnRemoveID).style.display = 'block';
                    }
                    else {
                        document.getElementById(Ektron.Widget.Flickr.uxbtnRemoveID).style.display = 'none';
                    }
                }
                var hiddenColl = document.getElementById(Ektron.Widget.Flickr.hdnPhoto);
                
                
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
                Ektron.Widget.Flickr.pageNumber = 0;
                Ektron.Widget.Flickr.maxCount = 0;
                Ektron.Widget.Flickr.lastPage = 0;
            }

        };
    }
});


function callBackDisplayImages(data) {
    var objW = Ektron.Widget.Flickr.GetWidget(Ektron.Widget.Flickr.currentCallWidgetID);
    if (objW) {
        objW.DisplayImages(data);
    }
}

function callBackDisplaySearchImages(data) {
    var objW = Ektron.Widget.Flickr.GetWidget(Ektron.Widget.Flickr.currentCallWidgetID);
    if (objW) {
        objW.DisplaySearchImages(data);
    }
}

function AddtoCollection(Id, Owner, Secret, Server, Farm, Title, IsPublic, IsFriend, IsFamily, IdCheckbox, hdnID) {

    var hiddenColl = document.getElementById(hdnID);
    var photodetails = Id + '|' + Owner + '|' + Secret + '|' + Server + '|' + Farm + '|' + Title + '|' + IsPublic + '|' + IsFriend + '|' + IsFamily;

    if (IdCheckbox.checked) {
        if (hiddenColl.value.indexOf(photodetails) < 0) {
            hiddenColl.value = hiddenColl.value + '~' + photodetails;
        }
    }
    else {
        hiddenColl.value = hiddenColl.value.toString().replace('~' + photodetails, '');
    }

}
function GetStatus(Id, Owner, Secret, Server, Farm, Title, IsPublic, IsFriend, IsFamily, IdCheckbox, hdnID) {

    var hiddenColl = document.getElementById(hdnID);
    var photodetails = Id + '|' + Owner + '|' + Secret + '|' + Server + '|' + Farm + '|' + Title + '|' + IsPublic + '|' + IsFriend + '|' + IsFamily;


    if (hiddenColl.value.indexOf(photodetails) <= 0)
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


function ValidateFlickrCollection(hdnCollectionId) {
    if (document.getElementById(hdnCollectionId).value == '0') {
        alert("There is no image in the collection.");
        return false;
    }

    return true;
}
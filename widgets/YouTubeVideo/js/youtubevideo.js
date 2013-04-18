Ektron.ready(function()
{
    if (typeof(Ektron.Widget) == "undefined")
    {
        Ektron.Widget = {};
    }

    if(typeof(Ektron.Widget.YouTubeVideo) == "undefined")
    {
        Ektron.Widget.YouTubeVideo =
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

            // CLASS OBJECTS
            YouTubeVideoWidget: function (id, outputId, submitButtonId)
            {
                var obj = this;
                obj.id = id;               
                obj.submitBtn = $ektron("#"+submitButtonId);
                obj.output = $ektron("#"+outputId);
                var token ="aa";
                if (token.length > 1) 
                {
                    obj.VideoClicked = function(id)
                    {
                        obj.output.attr("value", id);
                        obj.submitBtn.click();
                    };

                    obj.FindVideos = function()
                    {
                   
                      var oSort_by = $ektron("#" + id + "sort_by")[0];
                      Ektron.Widget.YouTubeVideo.sortBy = oSort_by.options[oSort_by.selectedIndex].value;
                       
                      var startIndex = (Ektron.Widget.YouTubeVideo.pageNumber  * Ektron.Widget.YouTubeVideo.pageSize) + 1; 
                      var queryUrl = obj.GetDataUrl(Ektron.Widget.YouTubeVideo.sortBy);                      
                      queryUrl += "?max-results=" + Ektron.Widget.YouTubeVideo.pageSize + "&start-index=" + startIndex; 
                      if(Ektron.Widget.YouTubeVideo.sortBy.indexOf('recent') == -1){    
                        queryUrl += "&time=today";
                      }
                        
                      Ektron.Widget.YouTubeVideo.currentCallWidgetID = obj.id;
                      obj.AppendScriptTag(queryUrl,'searchYVideo' + obj.id ,'callBackDisplayVideos');                      
                      
                    };

                    obj.PreviousVideos = function()
                    {
                        Ektron.Widget.YouTubeVideo.pageNumber += - 1;
                        obj.FindVideos();
                    };

                    obj.NextVideos = function()
                    {
                        Ektron.Widget.YouTubeVideo.pageNumber += 1;
                        obj.FindVideos();
                    };

                    obj.FirstVideos = function()
                    {                        
                        Ektron.Widget.YouTubeVideo.ResetPages();
                        Ektron.Widget.YouTubeVideo.pageNumber = 0;
                        obj.FindVideos();
                    };

                    obj.LastVideos = function()
                    {
                        Ektron.Widget.YouTubeVideo.pageNumber = Ektron.Widget.YouTubeVideo.lastPage;
                        obj.FindVideos();
                    };

                    obj.SearchPreviousVideos = function()
                    {
                        Ektron.Widget.YouTubeVideo.pageNumber += -1;
                        obj.SearchVideos();
                    };

                    obj.SearchNextVideos = function()
                    {
                        Ektron.Widget.YouTubeVideo.pageNumber += 1;
                        obj.SearchVideos();
                    };

                    obj.SearchFirstVideos = function()
                    {
                        Ektron.Widget.YouTubeVideo.ResetPages();
                        Ektron.Widget.YouTubeVideo.pageNumber = 0;
                        obj.SearchVideos();
                    };

                    obj.SearchLastVideos = function()
                    {
                        Ektron.Widget.YouTubeVideo.pageNumber = Ektron.Widget.YouTubeVideo.lastPage;
                        obj.SearchVideos();
                    };

                    obj.SearchVideos = function() 
                    {                    
                        
                       var tbData = $ektron("#" + id + "SearchText");
                       var searchtext = tbData.val();
                       if(searchtext.length <= 0){
                            return;
                       }
                       var searchtype = $ektron("#" + id + "searchtype").val();
                        
                       var startIndex = (Ektron.Widget.YouTubeVideo.pageNumber  * Ektron.Widget.YouTubeVideo.pageSize) + 1; 
                       var queryUrl = obj.GetDataUrl("");                                            
                       queryUrl += "?max-results=" + Ektron.Widget.YouTubeVideo.pageSize + "&start-index=" + startIndex; 
                       
                       
                        if (searchtype == "TAG") {
                            queryUrl += '&category=' + searchtext;
                        }else{
                            queryUrl += "&vq=" + searchtext; 
                        }   
                       
                       Ektron.Widget.YouTubeVideo.currentCallWidgetID = obj.id;
                       obj.AppendScriptTag(queryUrl,'searchYVideo' + obj.id ,'callBackDisplaySearchVideos');
                    };
                    
                    obj.MakeVideoThumbnail = function(thumbnailURL)
                    {
                        var thumbnail = $ektron("<img></img>");
                        thumbnail.attr("src", thumbnailURL);
                        thumbnail.attr("class", "thumbnail");
                        return thumbnail;
                    };

                    obj.MakeVideoLink = function(id, name)
                    {
                        var videoLink = $ektron("<a></a>");
                        videoLink.attr("href", "");
                        videoLink.html(name);
                        return videoLink;
                    };

                    obj.MakeVideoShortDescription = function(shortDescription)
                    {
                        var description = $ektron("<span></span>");
                        description.attr("class", "short-description");
                        description.html(shortDescription);
                        return description;
                    };

                    obj.DisplayVideos = function(videoCollection)
                    {                        
                        var list = $ektron("#"+obj.id+" ul.video-list");
                        list.html("");
                        var alt = false;
                        //Ektron.Widget.YouTubeVideo.maxCount = videoCollection.total_count;                        
                        Ektron.Widget.YouTubeVideo.maxCount = videoCollection.feed.openSearch$totalResults.$t;
                        
                        if(Ektron.Widget.YouTubeVideo.maxCount <= 0){
                          Ektron.Widget.YouTubeVideo.Pagingbuttons(obj.id, 0, 0);  
                          return;
                        }
                        
                         var itemIndex =-1;   
                         // Loops through entries in the feed and calls appendVideoData for each
                        //for (var i in videoCollection.items) 
                        for (var i = 0, entry; entry = videoCollection.feed.entry[i]; i++)
                        {                            
                           itemIndex = itemIndex + 1;
                           var videoID = entry.id.$t.substring(entry.id.$t.lastIndexOf('/')+1); 
                            var listItem = $ektron("<li></li>");
                            if (i === 0) {
                                listItem.attr("class", "alt1 videoFirst clearfix");
                            }
                            else {
                                listItem.attr("class", (alt = !alt) ? "alt1 clearfix" : "alt2 clearfix");
                            }                            
                            var thumbnail = obj.MakeVideoThumbnail(entry.media$group.media$thumbnail[0].url);
                            var videoLink = obj.MakeVideoLink('#',entry.media$group.media$title.$t);
                            var description = obj.MakeVideoShortDescription(entry.media$group.media$description.$t);
                            listItem.append("<div class='videoThumbOuter'></div>");
                            listItem.find(".videoThumbOuter").append("<div class='videoThumbInner'></div>");
                            listItem.find(".videoThumbInner").append(thumbnail.get(0));
                            //listItem.append("</div></div>");
                            listItem.append(videoLink.get(0));
                            listItem.append(description.get(0));

                            listItem.hover(function() 
                                { 
                                    $ektron(this).addClass('hover'); 
                                },
                                function() 
                                {
                                    $ektron(this).removeClass('hover'); 
                                }
                            );
                            listItem.click(
                                (function(id) 
                                    {
                                        return function() 
                                        {
                                            obj.VideoClicked(id);
                                            return false;
                                        };
                                    }
                                //)(videoCollection.items[i].id)
                                )(videoID)
                                
                            );
                            list.append(listItem);
                        }
                        //itemIndex = itemIndex + 1;
                        Ektron.Widget.YouTubeVideo.Pagingbuttons(obj.id, Ektron.Widget.YouTubeVideo.maxCount, itemIndex);
                    };

                    obj.DisplaySearchVideos = function(videoCollection) 
                    {
                        var list = $ektron("#" + obj.id + " ul.video-search");
                        list.html("");
                        var alt = false;
                        Ektron.Widget.YouTubeVideo.maxCount = videoCollection.feed.openSearch$totalResults.$t;
                        
                        if(Ektron.Widget.YouTubeVideo.maxCount <= 0){
                          Ektron.Widget.YouTubeVideo.PagingbuttonsSearch(obj.id, 0, 0);  
                          return;
                        }
                        
                        var itemIndex =-1;
                        
                       for (var i = 0, entry; entry = videoCollection.feed.entry[i]; i++) {
                            //if ( entry.yt$noembed) {
                              //  continue; 
                            //}else{
                                itemIndex = itemIndex + 1;
                            //}
                       
                            var videoID = entry.id.$t.substring(entry.id.$t.lastIndexOf('/')+1); 
                       
                            var listItem = $ektron("<li></li>");
                            if (itemIndex == 0) {
                                listItem.attr("class", "alt1 videoFirst clearfix");
                            }
                            else {
                                listItem.attr("class", (alt = !alt) ? "alt1 clearfix" : "alt2 clearfix");
                            }
                            var thumbnail = obj.MakeVideoThumbnail(entry.media$group.media$thumbnail[0].url);
                            var videoLink = obj.MakeVideoLink('#',entry.media$group.media$title.$t);
                            var description = obj.MakeVideoShortDescription(entry.media$group.media$description.$t);
                            listItem.append("<div class='videoThumbOuter'></div>");
                            listItem.find(".videoThumbOuter").append("<div class='videoThumbInner'></div>");
                            listItem.find(".videoThumbInner").append(thumbnail.get(0));
                            listItem.append(videoLink.get(0));
                            listItem.append(description.get(0));

                            listItem.hover(function() { $ektron(this).addClass('hover'); },
                                           function() { $ektron(this).removeClass('hover'); });
                            listItem.click((function(id) {
                                return function() {
                                    obj.VideoClicked(id);
                                    return false;
                                };
                            })(videoID));
                            list.append(listItem);
                        }
                        //itemIndex = itemIndex + 1;
                        Ektron.Widget.YouTubeVideo.PagingbuttonsSearch(obj.id, Ektron.Widget.YouTubeVideo.maxCount, itemIndex);
                    };

                    obj.PlayerItem = function(title, id)
                    {
                        var item = $ektron("<li></li>");
                        item.html(title);
                        item.click(function ()
                        {
                            obj.PlayerClicked(title, id);
                        });
                        return item.get(0);
                    };
                    
                    obj.KeyPressHandler = function(elem, event, id) 
                    {                        
                        if (event.keyCode == 13) {
                            if(event.preventDefault) event.preventDefault();
                            if(event.stopPropagation) event.stopPropagation();
                            event.returnValue = false;
                            event.cancel = true;
                            Ektron.Widget.YouTubeVideo.ResetPages();
                            setTimeout('Ektron.Widget.YouTubeVideo.widgets["' + id + '"].SearchVideos()', 1);
                            return false;
                            
                        }
                    };
                    
                    obj.GetDataUrl=function(ReqType){
                        var returnUrl = "";
                        if(ReqType != '' &&  ReqType.length > 0){
                            //top rated
                            returnUrl = 'http://gdata.youtube.com/feeds/standardfeeds/' + ReqType;                        
                        }else{
                            //all videos
                            returnUrl = 'http://gdata.youtube.com/feeds/videos';
                        }
                         
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
                          script.setAttribute('src', 
                              scriptSrc + '&alt=json-in-script&callback=' + scriptCallback);
                          script.setAttribute('id', scriptId);
                          script.setAttribute('type', 'text/javascript');
                          // Append the script tag to the head to retrieve a JSON feed of videos
                          // NOTE: This requires that a head tag already exists in the DOM at the
                          // time this function is executed.
                          document.getElementsByTagName('head')[0].appendChild(script);
                    };
                   
                } 
                else 
                {
                    var message = "You need to add your PublisherID, token, PlayerID provided by Brightcove.  Add these to the Workarea - Settings-Personalizations-YouTubeVideo widget, the variables are PlayerID, publisherID, and token";
                    $ektron(".ektronWidgetBrightcove").html("");
                    $ektron(".ektronWidgetBrightcove").append(message);
                    return;
               }
            },
            
            // METHODS
            AddWidget: function(id, outputId, submitButtonId)
            {
                var widg = new Ektron.Widget.YouTubeVideo.YouTubeVideoWidget(id, outputId, submitButtonId);
                Ektron.Widget.YouTubeVideo.widgets[id] = widg;

                // Create video player list
                $ektron("#" + id + " .player-heading").hover(
                    function(evt) 
                    {
                        var playerHeading = $ektron(this).find("ul");
                        playerHeading.width($ektron(this).width());
                        playerHeading.show();
                    }, 
                    function() 
                    {
                        playerHeading.hide();
                    }
                );
                
                Ektron.Widget.YouTubeVideo.widgets[id].FindVideos();
            },

            GetWidget : function(id)
            {
            
                return Ektron.Widget.YouTubeVideo.widgets[id];
            },
            
            Pagingbuttons: function (id, maxcount, items)
            {
                var numpages = 0;
                var theresults = "Results";
                var pagestart = 0;
                var pageend = parseInt(items);
                if (maxcount > 0)
                {
                    numpages = parseInt((maxcount-1)/Ektron.Widget.YouTubeVideo.pageSize);
                }
                if (maxcount > Ektron.Widget.YouTubeVideo.pageSize) {

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
                Ektron.Widget.YouTubeVideo.lastPage = numpages;
                if (Ektron.Widget.YouTubeVideo.pageNumber == 0)
                {
                    $ektron("#" + id + "First").attr("disabled", true).addClass("ektronWidgetYTFirstDisabled");
                    $ektron("#" + id + "Previous").attr("disabled", true).addClass("ektronWidgetYTPreviousDisabled");
                }
                else
                {
                    $ektron("#" + id + "First").attr("disabled", false).removeClass("ektronWidgetYTFirstDisabled");
                $ektron("#" + id + "Previous").attr("disabled", false).removeClass("ektronWidgetYTPreviousDisabled");
                }

                if (Ektron.Widget.YouTubeVideo.pageNumber < numpages)
                {
                    $ektron("#" + id + "Next").attr("disabled", false).removeClass("ektronWidgetYTNextDisabled");
                    $ektron("#" + id + "Last").attr("disabled", false).removeClass("ektronWidgetYTLastDisabled");
                }
                else
                {
                $ektron("#" + id + "Next").attr("disabled", true).addClass("ektronWidgetYTNextDisabled");
                $ektron("#" + id + "Last").attr("disabled", true).addClass("ektronWidgetYTLastDisabled"); ;
                }
                if (maxcount > 0) {
                    pagestart = (Ektron.Widget.YouTubeVideo.pageNumber * Ektron.Widget.YouTubeVideo.pageSize) + 1;
                    pageend = pageend+pagestart;
                    theresults = "Results " + pagestart + " - " + pageend +  " of " + maxcount;
                } else {
                    theresults = "No Results";
                }
                $ektron(".video-result").html("");
                $ektron(".video-result").append(theresults);
            },
            
            PagingbuttonsSearch: function(id, maxcount, items)
            {
                var numpages = 0;
                var theresults = "Results";
                var pagestart = 0;
                var pageend = parseInt(items);
                if (maxcount > 0)
                {
                    numpages = parseInt((maxcount-1)/Ektron.Widget.YouTubeVideo.pageSize);
                }
 
                if (maxcount > Ektron.Widget.YouTubeVideo.pageSize)  // first Page check
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
                Ektron.Widget.YouTubeVideo.lastPage = numpages;
                if (Ektron.Widget.YouTubeVideo.pageNumber == 0)
                {
                    $ektron("#" + id + "FirstSearch").attr("disabled", true).addClass("ektronWidgetYTFirstDisabled");
                    $ektron("#" + id + "PreviousSearch").attr("disabled", true).addClass("ektronWidgetYTPreviousDisabled");
                }
                else
                {
                    $ektron("#" + id + "FirstSearch").attr("disabled", false).removeClass("ektronWidgetYTFirstDisabled");
                    $ektron("#" + id + "PreviousSearch").attr("disabled", false).removeClass("ektronWidgetYTPreviousDisabled");
                }

                if (Ektron.Widget.YouTubeVideo.pageNumber < numpages)
                {
                    $ektron("#" + id + "NextSearch").attr("disabled", false).removeClass("ektronWidgetYTNextDisabled");
                    $ektron("#" + id + "LastSearch").attr("disabled", false).removeClass("ektronWidgetYTLastDisabled");
                }
                else
                {
                    $ektron("#" + id + "NextSearch").attr("disabled", true).addClass("ektronWidgetYTNextDisabled");
                    $ektron("#" + id + "LastSearch").attr("disabled", true).addClass("ektronWidgetYTLastDisabled");
                }

                if (maxcount > 0) {
                    pagestart = (Ektron.Widget.YouTubeVideo.pageNumber * Ektron.Widget.YouTubeVideo.pageSize) + 1;
                    pageend = pageend+pagestart;
                    theresults = "Results " + pagestart + " - " + pageend +  " of " + maxcount;
                } else {
                    theresults = "No Results";
                }

                $ektron(".video-search-result").html("");
                $ektron(".video-search-result").append(theresults);
            },

            SwitchPane: function(el, panename) {
                var parent = $ektron(el).parents(".youtube");
                var tablist = parent.find(".ektronWidgetYTTabs li a");
                var panes = parent.children(".pane");

                for(var i=0; i<tablist.length; i++){
                    $ektron(tablist[i]).removeClass("selectedTab");
                    if(tablist[i].id == panename) $ektron(tablist[i]).addClass("selectedTab");
                }

                for(var i=0; i<panes.length; i++){
                    $ektron(panes[i]).hide();
                    if($ektron(panes[i]).hasClass(panename)) $ektron(panes[i]).show();
                }

                Ektron.Widget.YouTubeVideo.ResetPages();
            },
            ResetPages: function() {
                Ektron.Widget.YouTubeVideo.pageNumber = 0;
                Ektron.Widget.YouTubeVideo.maxCount = 0;
                Ektron.Widget.YouTubeVideo.lastPage = 0;
            } 
        };
    }
});


 function callBackDisplayVideos(data){        
      var objW=  Ektron.Widget.YouTubeVideo.GetWidget(Ektron.Widget.YouTubeVideo.currentCallWidgetID);
      if(objW){
        objW.DisplayVideos(data);
      }
 }
 
function callBackDisplaySearchVideos(data){ 
    var objW=  Ektron.Widget.YouTubeVideo.GetWidget(Ektron.Widget.YouTubeVideo.currentCallWidgetID);
      if(objW){
        objW.DisplaySearchVideos(data);
      }       
}
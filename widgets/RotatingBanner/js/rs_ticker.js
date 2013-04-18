/*
rs_ticker.js
	
July 2009 - Javascript for the News Ticker - Indigo Creative
	
Requires jQuery (jquery.com)
	
HTML Structure
	
<div id="ticker">
<div id="ticker_content">
<div id="ticker_nav"></div>
<div id="ticker_content_one">
<img />
<h3><a>Title</a></h3>
<p>Description</p>
</div>
<div id="ticker_content_two">
</div>
</div>
</div>

*/

/* ================================================================= */
/* jQuery document-is-ready function. Place all calls here */

//jQuery(document).ready(function() {
//    rs_start();
var path = "widgets/RotatingBanner/";
var img_path = path + "images/";
/* --------------------------------------- */
/* 	
Changes should not need to occur below
this point
*/
/* --------------------------------------- */
/* Internal vars for ticker management */
var ticker_timer;
var ticker_title;
var ticker_link;
var ticker_image;
var ticker_description;
var ticker_page;
var ticker_page_count;
var ticker_frame;
var ticker_output;
var ticker_navigation;
var ticker_is_playing;
/* --------------------------------------- */
var photo_path;
var img_width;
var img_height;
var max_result;
var collection_id;
var show_title;
var show_teaser;
var ticker_delay;
var ticker_fade_time;
var isFirstTime;


function rs_start() {
    // *************************************************************
    if ($("#ticker_content").length) {

        /* *************************************** */
        /* --------------------------------------- */
        /* General user settings for integration */
        //"images/"; // relative path to control images (include trailing '/')
        photo_path = ""; // "images/";//"photos/"; // relative path to news photos (include trailing '/')
        ticker_title = new Array();
        ticker_link = new Array();
        ticker_image = new Array();
        ticker_description = new Array();
        clearTimeout(ticker_timer);
        img_width = $("#hdnImageWidth").val();
        img_height = $("#hdnImageHeight").val();
        max_result = $("#hdnMaxResult").val();
        collection_id = $("#hdnCollID").val();
        show_title = $("#hdnShowTitle").val();
        show_teaser = $("#hdnShowTeaser").val();
        ticker_delay = $("#hdnDuration").val() * 1000; // miliseconds between frames
        // ticker_fade_time = parseInt(ticker_delay) / 2; // miliseconds fade out / in effect
        //if (ticker_fade_time < 500)
        ticker_fade_time = 500;
        ticker_page = 1;
        isFirstTime = false;
        ticker_frame = 1;
        ticker_is_playing = true;
        /* --------------------------------------- */
        /* *************************************** */
         path = $("#hdnSitePath").val() + "widgets/RotatingBanner/";
         img_path = path + "images/";

        // Don't let the fade time be more than half the per-page time
        if ((ticker_fade_time * 2) > ticker_delay)
            ticker_fade_time = Math.floor(ticker_delay / 2);

        // Read the XML feed file and get our data.
        // If successful, begin the timer

        var xmlurl = ''; //'GetFeaturesXML.aspx?type=type1&q=';
        var now = new Date().getTime();
        var querystring = Math.floor(now / (1000 * 60 * 15)); // forces a fresh update of the XML file after 15 minutes
        //        xmlurl = 'widgets/RotatingBanner/RotatingBanner.ashx?CollID=' + collection_id + '&q=' + querystring + '&max_result=' + max_result; // + querystring;
        xmlurl = path + 'RotatingBanner.ashx?CollID=' + collection_id + '&q=' + querystring + '&max_result=' + max_result; // + querystring;        
        //for remove cache on edit time       
        xmlurl += "&rnd=" + $("#hdnRandCallID").val();        
        $.get(xmlurl, {}, function(xml) {
            var c = 1;

            $(xml).find('highlight').each(function() {

                ticker_title[c] = $(this).find('title').text();
                ticker_image[c] = $(this).find('image').text();
                ticker_link[c] = $(this).find('link').text();
                ticker_description[c] = $(this).find('description').text();
                c++;


            });
            if ($(xml).find('highlight').length > 0) {
                show_title = $("#hdnShowTitle").val();
                show_teaser = $("#hdnShowTeaser").val();
                show_MediaControl = $("#hdnShowMediaControl").val();
                
                if(show_MediaControl == 'True'){
                    $("#ticker_content").css({ "height": parseInt(parseInt(img_height) + 40).toString() + "px" });
                }else{
                    $("#ticker_content").css({ "height": parseInt(parseInt(img_height) + 10).toString() + "px" });
                }

                if (show_teaser == 'True' || show_title == 'True') {
                    $("#ticker_content").css({ "width": parseInt(parseInt(img_width) + 250).toString() + "px" });
                    $("#ticker_nav").css({ "width": parseInt(parseInt(img_width) + 230).toString() + "px" });
                }
                else {
                    $("#ticker_content").css({ "width": img_width + "px" });
                    $("#ticker_nav").css({ "width": img_width + "px" });
                }

                // save the count of articles
                ticker_page_count = c;

                // We know the number of frames now, inject the navigation bar
                //customize
                if(show_MediaControl == 'True'){
                    rs_ticker_nav();
                    $("#ticker_nav").css({ "display": "" });                        
                }else{
                    $("#ticker_nav").css({ "display": "none" });                                        
                }

                // Call our loop function and start the timer
                rs_ticker("start");
            }
            else {

                $("#ticker").css({ "display": "none" });
                $("#ticker_nav").css({ "background-color": "#ffffff", "height": "0px", "width": "0px" });
                $("#ticker_content_one").css({ "background-color": "#ffffff", "height": "0px", "border": "#ffffff" });
                $("#ticker_content_two").css({ "background-color": "#ffffff", "height": "0px", "border": "#ffffff" });
            }
        });

    }

}


// *************************************************************
// Generate the navigational elements and detect clicks
function rs_ticker_nav() {
   
    // Back button
    ticker_navigation = "<a href=\"#\" id=\"_back\" class=\"button\"><img src=\"" + img_path + "back.gif" + "\" width=\"14\" height=\"15\" alt=\"Back\" /></a>";

    // Play / Pause button
    ticker_navigation += "<a href=\"#\" id=\"_play\" class=\"button\"><img src=\"" + img_path + (ticker_is_playing ? "pause.gif" : "play.gif") + "\" width=\"10\" height=\"15\" alt=\"Play / Pause\" /></a>";

    // Next button
    ticker_navigation += "<a href=\"#\" id=\"_next\" class=\"button\"><img src=\"" + img_path + "next.gif" + "\" width=\"14\" height=\"15\" alt=\"Next\" /></a>";

    // Per page button
    //for (var i = 1; i < ticker_page_count; i++) {
    ticker_navigation += "<label href=\"#\" id=\"pageNumber\" class=\"page\">" + 1 + " of " + parseInt(parseInt(ticker_page_count) - 1).toString() + "</label>";
    //}


    // Add our navigation bar to the DOM
    $("#ticker_nav").html(ticker_navigation);

    // -------------------------------------------------------------
    // Navigation buttons ------------------------------------------
    // -------------------------------------------------------------

    // Adjust spacing on our control buttons
    $("#ticker_nav a:has(img)").css({ "margin": "0", "padding": "0" });
    $("#ticker_nav a#_back").css("margin-right", "2px");
    $("#ticker_nav a#_next").css("margin", "0 2px");

    //    // Jump to page..
    //    $("#ticker_nav a.page").click(function(e) {
    //        e.preventDefault();
    //        rs_ticker("stop");
    //        $("#ticker_nav a").removeClass("here");
    //        ticker_page = $(this).attr("id");
    //        updateFrame();
    //    });

    // PLAY / PAUSE
    $("#ticker_nav a#_play").click(function(e) {
        e.preventDefault(); // stop normal link click
        (ticker_is_playing ? rs_ticker("stop") : rs_ticker("start"));
    });

    // NEXT
    $("#ticker_nav a#_next").click(function(e) {
        e.preventDefault(); // stop normal link click
        rs_ticker("stop");
        ticker_page++;
        ticker_page = (ticker_page >= ticker_page_count ? 1 : ticker_page);
        updateFrame();
    });

    // BACK
    $("#ticker_nav a#_back").click(function(e) {
        e.preventDefault(); // stop normal link click
        rs_ticker("stop");
        ticker_page--;
        ticker_page = (ticker_page < 1 ? ticker_page_count - 1 : ticker_page);
        updateFrame();
    });

}
// *************************************************************
// Generate the HTML for the next frame and fade them
function rs_ticker(mode) {

    // If we're stopping...
    if (mode == "stop") {
        clearTimeout(ticker_timer);
        ticker_is_playing = false;
        $("#ticker_nav a#_play img").attr("src", img_path + "play.gif");
        //updateNav(); /* Commented by WS  because two button was getting highlighted after pause*/
        return;
    }

    $("#ticker_nav 	#_play img").attr("src", img_path + "pause.gif");

    ticker_is_playing = true;

    // Form the HTML for the next frame
    if (isFirstTime)
        updateFrame();
    else
        getFrame();

    // Update the 'current page' value
    ticker_page++;
    ticker_page = (ticker_page >= ticker_page_count ? 1 : ticker_page);

    isFirstTime = true;
    // Call the timer again
    //customize
    if( ticker_page_count > 2){
        ticker_timer = setTimeout(rs_ticker, ticker_delay);
    }

}
// *************************************************************
// User has chosen to move to another frame. Don't let this happen
// until the animation between frames has completed if that's happening
function updateFrame() {

    var i = 0;
    //($("#ticker_content #ticker_content_one:animated").size() != 0) && 
    while (i < ticker_fade_time)
        i++;

    getFrame();
}
// *************************************************************
// Form the HTML that will be output
function getFrame() {

    show_title = $("#hdnShowTitle").val();
    show_teaser = $("#hdnShowTeaser").val();

    // Image
    ticker_output = "<img src=\"" + photo_path + ticker_image[ticker_page] + "\" width=\"" + img_width + "\" height=\"" + img_height + "\" alt=\"" + ticker_title[ticker_page] + "\" />";

    // Title + Link
    if (show_title == 'True')
        ticker_output += "<h3><a href=\"" + ticker_link[ticker_page] + "\" title=\"" + ticker_title[ticker_page] + "\">" + ticker_title[ticker_page] + "</a></h3><br/>";

    // Description
    if (show_teaser == 'True') {
        ticker_output += "<div style=\"overflow:auto; float:left; height:" + parseInt(parseInt(img_height) - 50).toString() + "px;" + " width:230px;\" ><p style=\"padding:0 0 0 0; margin:0 0 0 0;\" title=\"" + ticker_description[ticker_page] + "\">" + ticker_description[ticker_page] + "</p></div>";


    }

    // Remove page highlight before we move to the next page
    //$("#ticker_nav a").removeClass("here");

    // Fade between current page and the next page
    if (ticker_frame == 1) {
        $("#ticker_content #ticker_content_two").fadeOut(ticker_fade_time, function() { $(this).empty(); });
        $("#ticker_content #ticker_content_one").html(ticker_output).fadeIn(ticker_fade_time, updateNav());
        ticker_frame = 2;
    } else {
        $("#ticker_content #ticker_content_one").fadeOut(ticker_fade_time, function() { $(this).empty(); });
        $("#ticker_content #ticker_content_two").html(ticker_output).fadeIn(ticker_fade_time, updateNav());
        ticker_frame = 1;
    }
}
// *************************************************************
// Places page highlight on the current page
function updateNav() {
    // Highlight the page we're on
    //$("#ticker_nav a#" + ticker_page).addClass("here");
    $("#pageNumber").text(ticker_page + " of " + parseInt(parseInt(ticker_page_count) - 1).toString());

}
// *************************************************************
//});
/* ================================================================= */
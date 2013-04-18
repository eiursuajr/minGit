<%@ Control Language="C#" AutoEventWireup="true" CodeFile="googletrackingcode.ascx.cs" Inherits="Analytics_Template_GoogleTrackingCode" EnableTheming="false" EnableViewState="false" %>
<!-- Start Google Code -->
<script type="text/javascript">
var _gaq = _gaq || [];
_gaq.push(['_setAccount', '<asp:literal id="GoogleUserAccount" runat="server"/>']);
_gaq.push(['_trackPageview']);
<asp:literal runat="server" id="variables"/>
(function() {
var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();
/* Start Track Event binding Code */
//// extracted from http://runtingsproper.blogspot.com/2009/12/how-to-automatically-track-events-with.html
//// un-comment the below if you would like google analytics to track your pdf download links on the website.
//$(document).ready(function () { 
//    TrackEventsForClicks();
//});
//  
//function TrackEventsForClicks() 
//{ 
//    TrackEventByFileExtension(".pdf"); 
//    // add your file extension here
//}   
//  
//function TrackEventByFileExtension(FileExtension) 
//{ 
//    $("a[href$='" + FileExtension + "']").click(function() { 
//        var fileName = ExtractFileNameFromUrl($(this).attr("href")); 
//        _gaq.push(['_trackEvent', 'click', fileName]);
//    }); 
//} 
//  
//function ExtractFileNameFromUrl(Url) 
//{ 
//    // Note this code assumes 
//    //   - that the url doesnt contain a query string 
//    //   - that a real url has been passed in 
//    //   - that the url has a filename and isnt a folder 
//    var SplitUrl = Url.split('/'); 
//    var FileName = SplitUrl[SplitUrl.length-1]; 
//    return FileName; 
//}
/* End Track Event Binding Code */
</script>
<!-- End Google Code -->
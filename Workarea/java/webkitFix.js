if (typeof Sys !== "undefined" && Sys.Browser) 
{
    if ("undefined" === typeof Sys.Browser.WebKit)
    {
        Sys.Browser.WebKit = {};//Safari 3 is considered WebKit
    }
    if (navigator.userAgent.indexOf( 'WebKit/' ) > -1)
    {
        if (Sys.Browser.agent !== Sys.Browser.WebKit)
        {
            Sys.Browser.agent = Sys.Browser.WebKit;
            Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
            Sys.Browser.name = 'WebKit';
        }
    }
}
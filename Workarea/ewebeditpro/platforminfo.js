// DO NOT CHANGE THIS CODE
// Copyright 2000-2002, Ektron, Inc.

/*
	Class: PlatformInfo
	
	Properties:
		isWindows
		isMac
		isSun
		isUnix
		isNetscape
		isNetscape60 - NS6.0 and NS6.01 have many special cases and so gets their own property
		isIE
		isOpera
		isSafari
		browserVersion (float) e.g., 5.01
		
	Methods:
		none
									
	Events:
		none
*/
function PlatformInfo() 
{
	// reference: http://www.mozilla.org/docs/web-developer/sniffer/browser_type.html
    var ua = window.navigator.userAgent.toLowerCase();

	this.isWindows = (ua.indexOf("win") > -1);
	this.isWinXPSP2 = (ua.indexOf("SV1") > -1);
	this.isWinVista = (ua.indexOf("windows nt 6.") > -1);
	this.isMac = (ua.indexOf("mac") > -1);
	this.isSun = (ua.indexOf("sunos") > -1);
	this.isUnix = (	this.isSun || (ua.indexOf("x11") > -1) || (ua.indexOf("irix") > -1) || 
					(ua.indexOf("hp-ux") > -1) || (ua.indexOf("sco") > -1) || (ua.indexOf("unix_sv") > -1) || 
					(ua.indexOf("unix_system_v") > -1)  || (ua.indexOf("ncr") > -1)  || (ua.indexOf("reliantunix") > -1) || 
                 	(ua.indexOf("dec") > -1) || (ua.indexOf("osf1") > -1) || (ua.indexOf("dec_alpha") > -1) || 
					(ua.indexOf("alphaserver") > -1) || (ua.indexOf("ultrix") > -1) || (ua.indexOf("alphastation") > -1) || 
					(ua.indexOf("sinix") > -1) || (ua.indexOf("aix")  > -1) || (ua.indexOf("inux") > -1) || 
					(ua.indexOf("bsd") > -1) || (ua.indexOf("freebsd") > -1));
	
	// Opera may present itself as IE or Netscape.
	var pOpera = ua.indexOf("opera");
	this.isOpera = (pOpera > -1);
	this.isSafari = ((ua.indexOf("safari") != -1) && (!this.isOpera));
	this.isNetscape = ((window.navigator.appName == "Netscape") && !this.isOpera);
    //this.isNetscape = ((navigator.appName.indexOf("Netscape") != -1) && (!this.isOpera));
    this.isFirefox = (((ua.indexOf("firefox/1.") != -1) || (ua.indexOf("firefox/2.") != -1)) && (!this.isOpera));
	this.isNetscape60 = false; // may be set true below
	var pIE = ua.indexOf("msie ");
	this.isIE = ((pIE > -1) && !this.isOpera);

    	if (this.isFirefox)
    	{
        	this.isNetscape = true;  
        	this.isOpera = false;  // just to be sure
    		this.isNetscape60 = false;  // just to be sure
    	}
	if (this.isOpera)
	{
		this.browserVersion = parseFloat(ua.substring(pOpera + 6));
	}
	else if (this.isIE) 
	{ 
		this.browserVersion = parseFloat(ua.substring(pIE + 5));
	}
	else if (this.isNetscape)
	{
		var pNetscape = ua.indexOf("netscape/");
		if (pNetscape > -1)
		{
			this.browserVersion = parseFloat(ua.substring(pNetscape + 9));
		}
		else
		{
			var pNetscape6 = ua.indexOf("netscape6");
			if (pNetscape6 > -1)
			{
				this.browserVersion = parseFloat(ua.substring(pNetscape6 + 10));
				this.isNetscape60 = (this.browserVersion >= 6.0 && this.browserVersion < 6.1);
			}
			else
			{
				this.browserVersion = parseFloat(window.navigator.appVersion);
				if (this.browserVersion >= 5.0)
				{
					// Browser is mimicing Netscape. Ex, Mozilla 5.0 is Netscape 6.x.
					var pMozilla = ua.indexOf("rv:");
					if (pMozilla > -1)
					{
						if (ua.indexOf("rv:0.9.4") > -1)
						{
							this.browserVersion = 6.2; // mimic Netscape 6.2.
						}
						else if (ua.indexOf("rv:1.7.5")> -1)
						{
						 	this.browserVersion = 8.1; // Netscape 8.1
						}
						else
						{
							var nRVversion = parseFloat(ua.substring(pMozilla + 3));
							if (nRVversion >= 1.0 && nRVversion < 1.4)
							{
								this.browserVersion = 7.0; // mimic Netscape 7.0.
							}
							else if (nRVversion >= 1.4 && nRVversion < 1.5) //currently not support Mozilla 1.5
							{
								this.browserVersion = 7.1; // mimic Netscape 7.1.
							}
							else if (nRVversion >= 1.7 && nRVversion < 1.8) 
							{
								this.browserVersion = 7.2; // mimic Netscape 7.2., FireFox 0.9, FireFox 1.5 and Mozilla 1.7
							}
							else if (nRVversion >=1.8 && nRVversion < 1.9)
							{
								this.browserVersion = 7.23; // mimic FireFox 1.5, it does not correspond to any Netscape version yet.
							}
							else if (nRVversion >=1.9)
							{
								this.browserVersion = 7.96; // mimic FireFox 3.0, it does not correspond to any Netscape version yet.
							}
							else 
							{
								this.isNetscape = false;
							}
						}
					}
					else
					{
						this.isNetscape = false;
					}
				}
			}
		}
	}
	else
	{
		this.browserVersion = parseFloat(window.navigator.appVersion);
	}
}


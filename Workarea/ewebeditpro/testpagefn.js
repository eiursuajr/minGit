function clientEnv(obj, sName, sOCXName)
{
	if (typeof obj == "object")
	{
		obj.refreshStatus();
		var eskerNeeded = (obj.instanceTypes["activex"].isSupported && obj.isNetscape);
		
		var strOsName = "unknown";
		var strOsReq = "(Windows 95 or later required)";
		if (obj.isWindows)
		{
			strOsName = "Microsoft Windows";
			strOsReq = "(Windows 95 or later required)";
		}
		else if (obj.isMac)
		{
			strOsName = "Mac OS";
			strOsReq = "";
		}
		else if (obj.isSun)
		{
			strOsName = "Sun Solaris";
			strOsReq = "";
		}
		else if (obj.isUnix)
		{
			strOsName = "Unix";
			strOsReq = "";
		}
		document.writeln("Operating System: " + strOsName + " " + strOsReq + "<br />");
		if (navigator.systemLanguage)
		{
			document.writeln("System language: " + navigator.systemLanguage + "<br />");
		}
		if (navigator.userLanguage)
		{
			document.writeln("User language: " + navigator.userLanguage + "<br />");
		}
		
		var strBrowserName = "unknown";
		var strBrowserReq = "(IE or Netscape required)";
		if (obj.isIE)
		{
			strBrowserName = "Internet Explorer";
			strBrowserReq = "(IE 5.5 or later required)";
		}
		else if (obj.isNetscape)
		{
			strBrowserName = "Netscape";
			strBrowserReq = "(4.7 or later required, IE 5.5 or later must also be installed)";
		}
		else if (obj.isOpera)
		{
			strBrowserName = "Opera";
			strBrowserReq = "";
		}
		document.writeln("Browser: " + strBrowserName + ", version " + obj.browserVersion + " " + strBrowserReq + "<br />");
		var strLanguageCode = "";
		if (navigator.language) // for Netscape
		{
	    	strLanguageCode = navigator.language;
		}
	 	if (navigator.userLanguage) // for IE
		{
	    	strLanguageCode = navigator.userLanguage;
		}
		document.writeln("Language: " + strLanguageCode + "<br />");
		
		if (!obj.isSupported)
		{
			document.writeln("<i>This OS or browser version is not yet supported.</i><br />");
		}
	
		document.write(sName + " Instance Types: <br />");
		for (var i = 0; i < obj.instanceTypes.length; i++)
		{
			document.write("&nbsp;&nbsp;&nbsp;&nbsp;" + obj.instanceTypes[i].type + " supported? " + obj.instanceTypes[i].isSupported + "<br />");
		}
		document.write("&nbsp;&nbsp;&nbsp;&nbsp;Currently Selected Type: " + obj.selectedType + "<br />");
		document.write(sName + " supported? " + obj.isSupported + "<br />");
		document.write("Automatic installation supported? " + obj.isAutoInstallSupported + "<br />");
		document.write(sName + " installed? ");
		if (obj.isAutoInstallSupported && !obj.isInstalled)
		{
			document.write("false, automatic installation failed");
		}
		else if (eskerNeeded && obj.isInstalled) 
		{
			document.write("probably, because the Ektron plug-in for " + sName + " is installed");
		}
		else
		{
			document.write(obj.isInstalled);
		}
		document.write("<br />");
		
		var strVersion = "unknown";
		var strActBarVersion = "unknown";
		var strUniToolVersion = "unknown";
		var strUniTool2Version = "unknown";
		var strUniToolVBVersion = "unknown";
		var strWIFXVersion = "unknown";
		var strMSCalVersion = "unknown";
		
		if(typeof ActiveXVersionInstalled != "undefined")
		{
			strActBarVersion = ActiveXVersionInstalled("ActiveBar3Library.ActiveBar3");
			strUniToolVersion = ActiveXVersionInstalled("UniToolbox.UniCombo.10");
			strUniTool2Version = ActiveXVersionInstalled("UniToolbox2.UniTab");
			strUniToolVBVersion = ActiveXVersionInstalled("UniToolboxVB.UniButton");
			strWIFXVersion = ActiveXVersionInstalled("WebImageFX.ImageEditor"); 
			strMSCalVersion = ActiveXVersionInstalled("MSCAL.Calendar");
		}
		
		if (obj.versionInstalled)
		{
			strVersion = obj.versionInstalled;
		}
		else if (!obj.isInstalled)
		{
			strVersion = "not installed";
		}
		
		document.write("Esker Netscape plug-in required? " + eskerNeeded);
		if (eskerNeeded)
		{
			document.write(" Installed? " + obj.isInstalled);
			document.writeln("<ul>");
			//document.writeln("<li>Firefox X is NOT SUPPORTED at this time (<a href=\"http://dev.ektron.com/kb_article.aspx?id=7076\">latest information</a>)</li>");
			document.writeln("<li>Firefox 3 requires Esker 7.96</li>");
			document.writeln("<li>Firefox 2 requires Esker 7.93</li>");
			document.writeln("<li>Firefox 1.5 requires Esker 7.93</li>");
			document.writeln("<li>NS 8.1 requires Esker 7.92</li>");
			document.writeln("<li>NS 7.2 requires Esker 7.9</li>");
			document.writeln("<li>NS 7.1 requires Esker 7.2</li>");
			document.writeln("<li>NS 7.0 requires Esker 7</li>");
			document.writeln("<li>NS 6.2 requires Esker 6.6</li>");
			document.writeln("<li>NS 6.1 requires Esker 6.5</li>");
			document.writeln("<li>NS 6.0 requires Esker 6.4</li>");
			document.writeln("<li>NS 4.x requires Esker 4.5</li>");
			document.writeln("</ul>");
		}
		document.write("<br />");	

		document.write("Version of " + sOCXName + " actually installed: <span class=\"value\">" + strVersion + "</span><br />");
		
		if(!obj.isNetscape)
		{
			document.write("Version of ActiveBar installed: <span class=\"value\">" + strActBarVersion + "</span><br />");
			document.write("Version of UniToolbox installed: <span class=\"value\">" + strUniToolVersion + "</span><br />");
			document.write("Version of UniToolbox2 installed: <span class=\"value\">" + strUniTool2Version + "</span><br />");
			document.write("Version of UniToolboxVB installed: <span class=\"value\">" + strUniToolVBVersion + "</span><br />");
			document.write("Version of WebImageFX installed: <span class=\"value\">" + strWIFXVersion + "</span><br />");
			document.write("Version of MS Calendar installed: <span class=\"value\">" + strMSCalVersion + "</span><br />");
		}
		
		document.write("<br />");	
			
		if (obj.isNetscape)
		{
			/* Display info on Esker plugin */
			/*source: Netscape's about:plugins */
			navigator.plugins.refresh(false);
			
			var numPlugins = navigator.plugins.length;
			
			if (numPlugins == 0)
			{
				document.writeln("<b><font size=\"+2\">No plug-ins are installed.</font></b><br />");
			}
			
			for (i = 0; i < numPlugins; i++)
			{
		        var plugin = navigator.plugins[i];
				
				if (plugin.name.indexOf("Esker", 0) != -1 || plugin.name.indexOf("Ektron", 0) != -1)
				{
			        document.write("<p><font size=\"+1\"><b>");
			        document.write(plugin.name);
			        document.writeln("</b></font></p>");
			
			        document.writeln("<dl><dd>File name:");
			        document.write(plugin.filename);
			        document.write("<dd><br />");
			        document.write(plugin.description);
			        document.writeln("</dl><p>");
			
			        document.writeln("<table width=\"100%\" border=\"2\" cellpadding=\"5\">");
			        document.writeln("<tr><th width=\"20%\"><font size=\"-1\">Mime Type</font></th>");
			        document.writeln("<th width=\"50%\"><font size=\"-1\">Description</font></th>");
			        document.writeln("<th width=\"20%\"><font size=\"-1\">Suffixes</font></th>");
			        document.writeln("<th><font size=\"-1\">Enabled</th></tr>");
					
			        var numTypes = plugin.length;
			        for (j = 0; j < numTypes; j++)
			        {
		                var mimetype = plugin[j];
		                if (mimetype)
		                {
		                       var enabled = "No";
		                       var enabledPlugin = mimetype.enabledPlugin;
		                       if (enabledPlugin && (enabledPlugin.name == plugin.name))
							{
		                        enabled = "Yes";
							}
		                       document.writeln("<tr align=\"center\">");
		                       document.writeln("<td>" + mimetype.type + "</td>");
		                       document.writeln("<td>" + mimetype.description + "</td>");
		                       document.writeln("<td>" + mimetype.suffixes + "</td>");
		                       document.writeln("<td>" + enabled + "</td>");
		                       document.writeln("</tr>");
		                }
			        }
			        document.write("</table>");
				}
			}
		}
	}
}
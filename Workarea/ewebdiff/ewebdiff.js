var cDIFF_PATH = "ewebdiff/";
var cDIFF_VERSION = "2,0,0,2";
var cDIFF_PROGID = "eWebDiff.eWebDiffLibCtl";
var cDIFF_CLASSID = "00DC8428-524C-43B3-834A-2E133F819B72";
var cDIFF_INSCLSID = "F5C958D0-8D50-4DFF-8473-B021357DA491";
var cDIFF_NAME = "eWebDiffCtl";
var cDIFF_NAMELP = "eWebDiffCtlInstall";
var g_cFormName = "myform";
var g_cFieldName = "MyeWebDiff";
var IsInstalled;
var ShowDiff = true;
var Version;

function Ekt_CreateEWebDiff()
{
	var sHtml;
	var pIE;
	var browserVersion;
	var ua = window.navigator.userAgent.toLowerCase();
	sHtml = "";	
	if(navigator.appName.indexOf("Netscape") == -1) 
	{
		pIE = ua.indexOf("msie ");
		browserVersion = parseFloat(ua.substring(pIE + 5));
		if (pIE > -1 && browserVersion >= 5.0)
		{	
			Version = replaceSubstring(eWebDiffInstalled(), ",", "");			
			var VerToInstall = replaceSubstring(cDIFF_VERSION, ",", "");			
			
			if (Version >= VerToInstall) {
				IsInstalled = true;
			}
			else {
				IsInstalled = false;
			}
			if (IsInstalled)
			{	
				ShowDiff = true;
				sHtml += '<OBJECT CLASSID="clsid:5220cb21-c88d-11cf-b347-00aa00a28331" ID="ewebdifflicense" VIEWASTEXT><PARAM NAME="LPKPath" VALUE="ewebdiff/ewebdiff.lpk"></OBJECT>'				
				sHtml += '<OBJECT ID="' + cDIFF_NAME + '" CLASSID="CLSID:' + cDIFF_CLASSID + '"';
				sHtml += ' CODEBASE="' + cDIFF_PATH + 'ewebdiff.CAB#version=' + cDIFF_VERSION + '" VIEWASTEXT>';			
				sHtml += '</OBJECT>';
			
			}							
			else
			{	
				ShowDiff = false;		
				sHtml += '<OBJECT ID="' + cDIFF_NAME + '" CLASSID="CLSID:' + cDIFF_INSCLSID + '"';
				sHtml += ' CODEBASE="' + cDIFF_PATH + 'ewebdiff.CAB#version=' + cDIFF_VERSION + '" VIEWASTEXT>';	
				sHtml += '</OBJECT>';	
				sHtml += '<script language="JavaScript1.2" type="text/javascript" for="' + cDIFF_NAME + '" ';
				sHtml += 'event="onInstalled()">';
				sHtml += 'oneWebDiffInstalled();';
				sHtml += '</scr' + 'ipt>';
			}
		}
	}	
	return sHtml;
}

function oneWebDiffInstalled()
{
	eWebDiffLoadMsg.style.cssText = "display: none;";
}
function replaceSubstring(inputString, fromString, toString) {
   // Goes through the inputString and replaces every occurrence of fromString with toString
   var temp = inputString;
   if (fromString == "") {
      return inputString;
   }
   if (toString.indexOf(fromString) == -1) { // If the string being replaced is not a part of the replacement string (normal situation)
      while (temp.indexOf(fromString) != -1) {
         var toTheLeft = temp.substring(0, temp.indexOf(fromString));
         var toTheRight = temp.substring(temp.indexOf(fromString)+fromString.length, temp.length);
         temp = toTheLeft + toString + toTheRight;
      }
   } else { // String being replaced is part of replacement string (like "+" being replaced with "++") - prevent an infinite loop
      var midStrings = new Array("~", "`", "_", "^", "#");
      var midStringLen = 1;
      var midString = "";
      // Find a string that doesn't exist in the inputString to be used
      // as an "inbetween" string
      while (midString == "") {
         for (var i=0; i < midStrings.length; i++) {
            var tempMidString = "";
            for (var j=0; j < midStringLen; j++) { tempMidString += midStrings[i]; }
            if (fromString.indexOf(tempMidString) == -1) {
               midString = tempMidString;
               i = midStrings.length + 1;
            }
         }
      } // Keep on going until we build an "inbetween" string that doesn't exist
      // Now go through and do two replaces - first, replace the "fromString" with the "inbetween" string
      while (temp.indexOf(fromString) != -1) {
         var toTheLeft = temp.substring(0, temp.indexOf(fromString));
         var toTheRight = temp.substring(temp.indexOf(fromString)+fromString.length, temp.length);
         temp = toTheLeft + midString + toTheRight;
      }
      // Next, replace the "inbetween" string with the "toString"
      while (temp.indexOf(midString) != -1) {
         var toTheLeft = temp.substring(0, temp.indexOf(midString));
         var toTheRight = temp.substring(temp.indexOf(midString)+midString.length, temp.length);
         temp = toTheLeft + toString + toTheRight;
      }
   } // Ends the check to see if the string being replaced is part of the replacement string or not
   return temp; // Send the updated string back to the user
} // Ends the "replaceSubstring" function

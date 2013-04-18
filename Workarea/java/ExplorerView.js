var g_debug_level = 0; // 0 = no debugging: 1 = just errors: 2 = warnings and errors: 3 = all messages

/*
 * ExplorerView.js
 *
 * Defines the core Javascript funtions and helper methods for the ExplorerView
 * server control.
 */
var g_ektLogWin = null;
function creatLoggingWindow()
{
    try
    {
        //debugger;
        if(g_ektLogWin != null)
        {
            g_ektLogWin.close();
            g_ektLogWin = null;
        }

        var url = g_baseUrl;
        if(url != "")
        {
            if(url.indexOf("workarea/") > 0)
                url.replace("workarea/", "/");

            if(url.indexOf("workarea") > 0)
                url.replace("workarea", "");

            if(url.lastIndexOf("/") != url.length)
                url = url + "/";

            url = url + "workarea/debugPage.html";

            if(top.opener)
            {
                g_ektLogWin = top.opener.open(url, "ektdebugging", "width=500,height=450,scrollbars=yes,resizable=yes");
            }
            else
            {
                g_ektLogWin = top.window.open(url, "ektdebugging", "width=500,height=450,scrollbars=yes,resizable=yes");
            }
        }
        return g_ektLogWin;
    }
    catch(e)
    {
        ; // no need to error, it's a debug window
    }
    return null;
}

function logEvent(level, message)
{
    try
    {
        if(g_debug_level == 0)
            return;

        if(g_debug_level >= level)
        {
            if(g_ektLogWin == null)
                creatLoggingWindow();
        }

        if(g_ektLogWin)
        {
            var doc = g_ektLogWin.document;
            if(doc)
            {
                var div = doc.getElementById("ektdebugoutput");
                if(div)
                {
                    div.innerHTML = div.innerHTML + "<br>" + message;
                }
            }
        }
    }
    catch(e)
    {
        ; // no need to error, it's a debug window
    }
    return;
}

/**
 ** HELPER FUNCTIONS
 **/
var ExplorerQueryString=new function(){
    this.Param=function(key){
        var self=ExplorerQueryString;
        var value = null;
        for (var i=0;i<self.Param.keys.length;i++)
        {
	        if (self.Param.keys[i]==key){
		        value = self.Param.values[i];
		        break;
	        }
        }
        return value;
    };
    this.Param.keys = null;
    this.Param.values = null;
    this.Parse=function(args){
        var self=ExplorerQueryString;
        self.Param.keys = new Array();
        self.Param.values = new Array();
        var query = args;
        var pairs = query.split("&");

        for (var i=0;i<pairs.length;i++)
        {
	        var pos = pairs[i].indexOf('=');
	        if (pos >= 0)
	        {
		        var argname = unescape(pairs[i].substring(0,pos));
		        var value = unescape(pairs[i].substring(pos+1));
		        self.Param.keys[self.Param.keys.length] = argname;
		        self.Param.values[self.Param.values.length] = value;
	        }
        }
    };
};

function GetIframeDoc(iframeID)
{
    try
    {
        var iframeObj = document.getElementById(iframeID);
        var iframeDoc = (iframeObj.contentWindow ||
                         iframeObj.contentDocument);
        if(iframeDoc.document) iframeDoc = iframeDoc.document;
        return iframeDoc;
    }
    catch(e)
    {
        alert("Error: Unable to find iframe.\n\n[" + e.name + "]\n" + e.message);
        return null;
    }
}

function NavigateIframe(iframeID, iframeURL)
{
    try
    {
        var iframeObj = document.getElementById(iframeID);
        iframeObj.location.href = iframeURL;
        return true;
    }
    catch(e)
    {
        return false;
    }
}

function SetCookie(c_name, value)
{
    document.cookie=c_name+ "=" +escape(value);
}

function GetCookie(c_name)
{
    if (document.cookie.length>0)
    {
        c_start=document.cookie.indexOf(c_name + "=");
        if (c_start!=-1)
        {
            c_start=c_start + c_name.length+1 ;
            c_end=document.cookie.indexOf(";",c_start);
            if (c_end==-1) c_end=document.cookie.length;
            return unescape(document.cookie.substring(c_start,c_end));
        }
    }
    return "";
}

function GetEcmValue(name, cookieEcm)
{
    if (cookieEcm.length > 0)
    {
        c_start = cookieEcm.indexOf(name+"=");
        if (c_start!=-1)
        {
            c_start=c_start + name.length+1 ;
            c_end=cookieEcm.indexOf("&",c_start);
            if (c_end == -1)
                c_end=cookieEcm.length;
            return unescape(cookieEcm.substring(c_start,c_end));
        }
    }
    return "";
}

/**
 ** METADATA
 **/
var g_ajaxPage = "assetmanagement/dmspoll.ashx";
//var g_viewPath = "";
var g_baseUrl = "";
var g_metaPage = "workarea/DMSMetadata.aspx";
var g_showingTBWindow = false;
var g_showingLoadMessage = true;
var req = null;
if (window.ActiveXObject) {
 req = new ActiveXObject("Microsoft.XMLHTTP");
}
function AjaxRequest(url, callback)
{
	try
	{
		if(req)
		{
		    url = url + "&" + new Date().getTime();
			req.open("POST", url, false);
			req.setRequestHeader("Pragma", "no-cache");
			req.onreadystatechange = function()
			{
			    if(req.readyState == 4 && req.status == 200)
			    { 
			       if (g_showingLoadMessage)
			       {
			        var waitLoad = document.getElementById("ek_DmsLoadWait")
                     if (waitLoad != null)
                        {
                            waitLoad.style.position = 'absolute';
                            waitLoad.style.left = '-10000px';
                        }
                        
			         var ifrmeWrapper =  document.getElementById("container_ExplorerDragDrop");
                     if(ifrmeWrapper != null)
                         {   
                            ifrmeWrapper.style.border = "solid 1px #999;";
                         }	           	
                    
			         g_showingLoadMessage = false;
			       }
			        
			        if(callback)
			            callback();
			    }
			}
			req.send("W=1");
			return;
		}
	}
	catch(e)
	{
		HandleError("AjaxRequest", e);
	}
	return "";
}

function GetContentIDString_Callback()
{

    var responseText = req.responseText;
    responseText = responseText.replace("<EktResponse>", "");
    responseText = responseText.replace("</EktResponse>", "");

    if(responseText != "")
    {
        if(HasRequiredMetaData())
        {
            var idString = responseText;
            ClearContentIDsNoRefresh();
            DisplayMetaDataPage(idString);
        }
        else
        {
            ClearContentIDs();
        }
    }
}

function HasRequiredMetaData()
{
    try
    {
        if(g_metaRequired == "Yes")
            return true;
        else
            return false;
    }
    catch(e)
    {
        return false;
    }
    return false;
}


function DisplayMetaDataPage(contentIds)
{ 
    var contArr = contentIds.split(",");
    var url = g_baseUrl + g_metaPage + "?contentId=" + contArr[0] + "&folderId=" + g_folderId + "&idString=" + contentIds +"&LangType=" + g_langType;

    if(top.frames["ek_main"] != null)
        top.frames["ek_main"].location.href = url;
    else
    {
        if (parent != null && typeof parent.ektb_remove == 'function')
        {
            var taxonomyId = 0;
            if (typeof g_TaxId != 'undefined' && g_TaxId > 0) {
                url = url + "&taxonomyId=" + g_TaxId;
            }
            url = url + "&close=true&EkTB_iframe=true&height=550&width=650&modal=true&refreshCaller=true";
            parent.ektb_show("Add asset data", url);
        }
    }

    return;
}

////////////////////////////
// GetContentIDs
//
// Return Values:
//      0 - requestText is empty, nothing at all is on the server
//      1 - requestText has all of the uploaded contentIds, can finish the upload processing
//      2 - there is text in the requestText but it is either in an intermediate uploading state or we are still waiting to determine if it is finished
//
///////////////////////////

function GetContentIDs( )
{
    logEvent(3, "GetContentIDs:enter");

    AjaxRequest(g_baseUrl + g_ajaxPage + "?action=get_content_ids",null);
    var responseText = req.responseText;
    responseText = responseText.replace("<EktResponse>", "");
    responseText = responseText.replace("</EktResponse>", "");
    logEvent(3, "GetContentIDs:responseText=" + responseText);
    if(responseText != "") // we have a reply with data in it from the ajax call
    {
   
        var ifrmeWrapper =  document.getElementById("container_ExplorerDragDrop");
        if(ifrmeWrapper != null)
         {   
             ifrmeWrapper.style.position =  'relative';
             ifrmeWrapper.style.top = "200px";
             ifrmeWrapper.style.border = "none";
         }
        var waitImage = document.getElementById("ek_DmsWaitImage");
        if (waitImage != null)
        {
            waitImage.style.position =  'relative';
            waitImage.style.left = '200px';
            waitImage.style.top = '10px';
        }
        

        logEvent(3, "myPoll.GetResponseLength()=" + myPoll.GetResponseLength() + "responseText.length=" + responseText.length);

        if(((responseText.indexOf("#CU#") > 0 || responseText.indexOf("#CC#") > 0) && responseText.lastIndexOf("#CU#") > responseText.lastIndexOf("#CD#"))
                                                                    || (myPoll.GetResponseLength() > 0 && responseText.length > myPoll.GetResponseLength())) // in an intermediate state
        {
            logEvent(3, "GetContentIDs:foudn a CU");
            if(!g_showingTBWindow)
            {
                if(top.DisplayUploadingBox)
                    top.DisplayUploadingBox(true);
                g_showingTBWindow = true;
            }
            myPoll.ResetProcessingDelay(); // we hit a CU or CC state, in this case reset the delay for the CD state (since we need to wait 3 seconds after out first CD)
            myPoll.ResetCounter();
            myPoll.SetResponseLength(responseText.length);
            logEvent(3, "GetContentIDs:returning 2");
            return 2;
        }

        // if there is no CU or CC state, then do one more loop to make sure there isn't a CU or CC state coming....
        if(responseText.indexOf("#CD#") > 0)
        {
            logEvent(3, "GetContentIDs:got a CD state");
            if(myPoll.ShouldDelay())
            {
                if(!g_showingTBWindow)
                {
                    if(top.DisplayUploadingBox)
                        top.DisplayUploadingBox(true);
                    g_showingTBWindow = true;
                }

                logEvent(3, "GetContentIDs:DecrementProcessingDelay");
                myPoll.DecrementProcessingDelay();
                myPoll.ResetCounter();
                myPoll.SetResponseLength(responseText.length);
                logEvent(3, "GetContentIDs:return 2");
                return 2;
            }
        }

        myPoll.SetResponseLength(0);

        if(g_showingTBWindow)
        {
            if(top.DisplayUploadingBox)
                top.DisplayUploadingBox(false);
            g_showingTBWindow = false;
        }

        if(typeof parent.EktUploadCompleteCallback == 'function')
        {
            logEvent(3, "GetContentIDs:Callback is defined");
            var retString = ExtractIDArray(responseText);
            parent.EktUploadCompleteCallback(retString, HasRequiredMetaData(), g_folderId, g_cmsFolderPath);
            return 1;
        }


        // @ this point there is just CD and we have waited the delay processing time (in seconds)to
        // make sure no other CU or CC states are coming so let's deal with what we have....
        if(HasRequiredMetaData())
        {
            logEvent(3, "GetContentIDs:has metadata or taxonomy");
            var idString = "";
            if(responseText.indexOf("#CD#") > 0)
            {    
                // if there are "CD"'s attached to the state, parse them out so that the metadata page can use the clean ids.
                logEvent(3, "GetContentIDs:responseText=" + responseText);
                idArr = responseText.split(";");
                for(var i = 0; i < idArr.length; i++)
                {
                    logEvent(3, "GetContentIDs:idArr[" + i + "]=" + idArr[i]);
                    if(idArr[i].length > 0 && idArr[i] != "")
                    {
                        if(idArr[i].substring(0, idArr[i].indexOf("#")).length > 0 && idArr[i].substring(0, idArr[i].indexOf("#")) != "")
                        {
                            idString = idString + idArr[i].substring(0, idArr[i].indexOf("#")) + ",";
                        }
                        logEvent(3, "GetContentIDs:(loop " + i + ")idString=" + idString);
                    }
                }
            }
            else
                idString = responseText;

            logEvent(3, "GetContentIDs:idString=" + idString);

            ClearContentIDsNoRefresh(); // we want to clear the ids on the server so we can recieve new ones but not refresh the page since we are going to show the metadata page
            DisplayMetaDataPage(idString); // show the metadata page, passing it the id's of the content to attach the metadata to.
            myPoll.Pause();
        }
        else
        {
           // no required metadata and it is done with the upload, so just clear the ids and refresh the page
           myPoll.Pause();
           ClearContentIDs();
        }

        // we had to have waited for the processing delay to make sure there were no files
        // still uploading before we start to process the page, so reset it for next time
        //myPoll.ResetProcessingDelay();
        return 1;
    }
    else // no text returned from the ajax call...
    {
        myPoll.ResetProcessingDelay(); // probably redundant to reset this here but better to be safe than sorry
        return 0;
    }

}
function ExtractIDArray(responseTextObject)
{
    var idString = "";
    if(responseTextObject.indexOf("#CD#") > 0)
    {
        // if there are "CD"'s attached to the state, parse them out so that the metadata page can use the clean ids.

        logEvent(3, "GetContentIDs:responseText=" + responseTextObject);
        idArr = responseTextObject.split(";");

        for(var i = 0; i < idArr.length; i++)
        {
            logEvent(3, "GetContentIDs:idArr[" + i + "]=" + idArr[i]);
            if(idArr[i].length > 0 && idArr[i] != "")
            {
                if(idArr[i].substring(0, idArr[i].indexOf("#")).length > 0 && idArr[i].substring(0, idArr[i].indexOf("#")) != "")
                {
                idString = idString + idArr[i].substring(0, idArr[i].indexOf("#")) + ",";
        }
                logEvent(3, "GetContentIDs:(loop " + i + ")idString=" + idString);
            }
        }
    }
    else
        idString = responseTextObject;

    return idString;
}

function ContentCB(evt)
{
    if(parent.EktUploadCompleteCallback)
    {
        if(typeof parent.EktUploadCompleteCallback == 'function')
        {
            var retString = ExtractIDArray(evt.target.getAttribute("rel"));
            if(parent.EktUploadCompleteCallback)
            {
                if(typeof parent.EktUploadCompleteCallback == 'function')
                {
                    parent.EktUploadCompleteCallback(retString, evt.target.getAttribute("reqmeta"), evt.target.getAttribute("folderid"), evt.target.getAttribute("folderpath"));
                }
                return 1;
            }
        }
    }
}

function docallback(view)
{
	try
	{
		ExplorerView_SetCallBack("iframeDAV_dragdrop");
	}
	catch(e)
	{
		setTimeout("docallback('" + view + "')",100);
	}
}

function attachListenerSocket(view)
{
	try
	{
		document.getElementsByTagName("iframeDAV_dragdrop_ExplorerView_CallBack")[0].addEventListener("ek_ContentDataCallBack", ContentCB, false, true);
    }
	catch(e)
	{
		setTimeout("attachListenerSocket('" + view + "')", 100);
	}
}

function ClearContentIDs()
{
    logEvent(3, "ClearContentIDs:enter");
    AjaxRequest(g_baseUrl + g_ajaxPage + "?action=clear_ids", null);
    ExplorerView_ReloadWorkarea();
    logEvent(3, "ClearContentIDs:exit");
    return;
}
function ClearContentIDsNoRefresh()
{
    logEvent(3, "ClearContentIDsNoRefresh:enter");
    myPoll.Pause();
    AjaxRequest(g_baseUrl + g_ajaxPage + "?action=clear_ids", null);
    logEvent(3, "ClearContentIDsNoRefresh:exit");
    return;
}
function HandleError(func, err)
{
    alert(func + "\nerror: " + err.type + "\nmessage: " + err.message);
    return;
}
function ContentPoll()
{
    this.pollCounter = 0;
    this.contentIDs = "";
    this.bPolling = false;
    this.timeoutHandle = null;
    this.processingDelay = 4; // each loop is 1 second
    this.responseLength = 0;

    this.SetUrl = function()
    {
        myPoll.SetBaseUrl();
        return;
    };

    this.SetBaseUrl = function()
    {
        logEvent(3, "SetBaseUrl:enter");
        var index = g_viewPath.indexOf("ekdavroot");
        var url = g_viewPath.substring(0, index);
        var hostname = document.location.hostname;

        var fSlash = url.indexOf("//");
        var sSlash = url.indexOf("/", url.indexOf("//") + 2);

        var replaceTerm = url.substring(fSlash + 2, sSlash);

        var port = "";
        if(replaceTerm.indexOf(":") > 0)
        {
            port = replaceTerm.substring(replaceTerm.indexOf(":"), replaceTerm.length);
        }

        logEvent(3, "SetBaseUrl:port=" + port);

        hostname = hostname + port;

        url = url.replace(replaceTerm, hostname);
        logEvent(3, "SetBaseUrl:url=" + url);
        g_baseUrl = url;
        logEvent(3, "SetBaseUrl:exit, g_baseUrl=" + g_baseUrl);
        return;
    };

    this.Start = function()
    {
        logEvent(3, "Start");
        if(this.IsPolling())
            return false;

        myPoll = this;
        if(myPoll.timeoutHandle)
        {
            logEvent(3, "Start:clearTimeout");
            clearTimeout(myPoll.timeoutHandle);
        }
        logEvent(3, "Start:ClearContentIDsNoRefresh");
        ClearContentIDsNoRefresh();
        this.contentIDs = "";
        this.ResetCounter();
        this.bPolling = true;
        this.DoPoll();
        return true;
    };

//    this.Pause = function()
//    {
//        alert(this.isPolling);
//        alert(this.timeoutHandle != null);
//        if(this.isPolling && this.timeoutHandle != null)
//            clearTimeout(this.timeoutHandle);
//    };

    this.Continue = function()
    {
        this.DoPoll();
        return;
    };

    this.IsPolling = function()
    {
        return this.bPolling;
    };


    this.DoPoll = function()
    {
        try
	    {
	        logEvent(3, "DoPoll:Enter");
		    if(myPoll.pollCounter == 0 || myPoll.IsPolling() == false)
		    {
		        logEvent(3, "DoPoll:myPoll.pollCounter=" + myPoll.pollCounter + ":myPoll.IsPolling()=" + myPoll.IsPolling());
		        //alert(myPoll.pollCounter);
		        myPoll.bPolling = false;
		        if(myPoll.timeoutHandle)
                {
                    clearTimeout(myPoll.timeoutHandle);
                }

                if(g_showingTBWindow)
                {
                    logEvent(3, "DoPoll:g_showingTBWindow is not null");
                    if(top.DisplayUploadingBox)
                        top.DisplayUploadingBox(false);
                    g_showingTBWindow = false;
                    ClearContentIDs(); // if we got here than the server hasn't moved in a while, we may as well refresh the page since thee may be new info.
                }

                if(myPoll.pollCounter == 0)
                    ClearContentIDs();

			    return true;
		    }
		    var state = GetContentIDs();
            logEvent(3, "DoPoll:returned state=" + state);
		    if(state == 0)
		    {
		        -- myPoll.pollCounter;
	            logEvent(3, "DoPoll:myPoll.pollCounter=" + myPoll.pollCounter);
		    }
		    else if(state == 1)
		    {
		        return true;
		        alert('almost done');
		    }

            logEvent(3, "DoPoll:reset Timeout");
		    myPoll.timeoutHandle = setTimeout('myPoll.DoPoll()', 3000); // lets make this 3 seconds instead of 1 for our polling loop
            return true;
	    }
	    catch(e)
	    {
		    HandleError("DoPoll", e);
		    return false;
	    }
    };

    this.Pause = function()
    {
        if(myPoll.timeoutHandle)
        {
            clearTimeout(myPoll.timeoutHandle);
            return true;
        }

        return true;
    };

    this.ResetCounter = function()
    {
	    myPoll.pollCounter = 60; // 3 minutes, 60 loops x 3 seconds per loop
        return true;
    };

    this.ResetProcessingDelay = function()
    {
	    myPoll.processingDelay = 4;
        return true;
    };

    this.DecrementProcessingDelay = function()
    {
        if(myPoll.processingDelay > 0)
        {
            myPoll.processingDelay--;
            logEvent(3, "GetContentIDs:DecrementProcessingDelay = " + myPoll.processingDelay);
            return true;
        }
        else
            return false;
    };


    this.SetResponseLength = function(length)
    {
        myPoll.responseLength = length;
        logEvent(3, "SetResponseLength:length =" + length);
    };

    this.GetResponseLength = function()
    {
        logEvent(3, "GetResponseLength:length =" + myPoll.responseLength);
        return myPoll.responseLength;
    };

    this.ShouldDelay = function()
    {
        if(myPoll.processingDelay > 0)
            return true;
        else
            return false;
    };


    return true;
};

var myPoll = new ContentPoll();
function ExplorerView_StartPoll()
{
    myPoll.SetUrl();
	myPoll.Start();
    return true;
};
/**
 ** AJAX FUNCTIONS
 **/

var ExplorerView_iOldDocCount = -1;
function GetDocumentCount(folderID)
{
    return 1;//document.getElementById("poller").value;
}

function CreateXmlHttpObject()
{
    var xmlhttp;
    // This if condition for Firefox and Opera Browsers
    if (!xmlhttp && typeof XMLHttpRequest != 'undefined')
    {
    try
    {
        xmlhttp = new XMLHttpRequest();
    }
    catch (e)
    {
        alert("Your browser is not supporting XMLHTTPRequest");
        xmlhttp = false;
    }
    }
    else
    {
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    return xmlhttp;
}

// The notorious AJAX object
var request = CreateXmlHttpObject();

function GetFolderContentCount(folderID)
{
    request.open("GET", "dragdropfuncs.aspx?fid=" + folderID, true);
    request.onreadystatechange = GetFolderContentCount_Callback;
    request.send(null);
    return true;
}

function GetFolderContentCount_Callback()
{
    retval = -1;
    if (request.readyState == 4)
    {
        if (request.status == 200)
        {
            var html = request.responseText;
            try
            {
                pos1 = html.indexOf("<contentCount>");
                pos2 = html.indexOf("</contentCount>");
                retval = html.substring(pos1 + "<contentCount>".length, pos2);
            }
            catch (e)
            {
                alert(e.message);
            }
        }
    }
    return retval;
}


/**
 ** EXPLORER VIEW FUNCTIONS
 **/
function ExplorerView_Disable(viewID, viewPath)
{
//    var authDiv = document.getElementById("auth_" + viewID);
//    var unauthDiv = document.getElementById("unauth_" + viewID);
//
//    unauthDiv.style.visibility = "visible";
//    unauthDiv.style.display    = "block";
//
//    authDiv.style.visibility = "hidden";
//    authDiv.style.display    = "none";
}

function ExplorerView_Enable(viewID, viewPath)
{
    //g_viewPath = viewPath;
    ExplorerView_Show(viewID, viewPath);
}

function ExplorerView_Show(viewID, viewPath, folderID)
{
    logEvent(3, "ExplorerView_Show:viewID=" + viewID + ":viewPath=" + viewPath + ":folderID=" + folderID);
    //g_viewPath = viewPath;
    var authDiv = document.getElementById("auth_" + viewID);
    //var unauthDiv = document.getElementById("unauth_" + viewID);

    authDiv.style.visibility = "visible";
    authDiv.style.display    = "block";

    //unauthDiv.style.visibility = "hidden";
    //unauthDiv.style.display    = "none";

    var iframeDav = document.getElementById("iframeDAV_" + viewID);

    if(iframeDav && iframeDav.document && iframeDav.document.body)
    {

        var iframeBody = iframeDav.document.body;
        iframeBody.ondragover = function() {window.event.returnValue = false;}
        iframeBody.ondragenter = function() { ExplorerView_StartPoll();window.event.returnValue = false;}
        iframeBody.ondrop = function() {window.event.returnValue = false;}
    }
    var isOk=false;
    try
    {
        var ret = "";
        ret = document.getElementById("divDAV_" + viewID).navigateFrame(viewPath, "iframeDAV_" + viewID);
        logEvent(3, "ExplorerView_Show:First ret=" + ret);
        if (ret=="OK")
			isOk=true;
        else
        {
            var davRoot = viewPath.indexOf("ekdavroot");
            var slashIndex = viewPath.indexOf("/", davRoot);
            var rootFolder = viewPath;
            if(slashIndex > 0)
                rootFolder = viewPath.substring(0, slashIndex);
            var idPath = rootFolder + "/_ektid_" + folderID + "/";
            var ret = "";
            ret = document.getElementById("divDAV_" + viewID).navigateFrame(idPath, "iframeDAV_" + viewID);
            if (ret=="OK")
			    isOk=true;
			logEvent(3, "ExplorerView_Show:Second ret=" + ret);
        }
		/*else
		{
		    var davRoot = viewPath.indexOf("ekdavroot");
	        var slashIndex = viewPath.indexOf("/", davRoot);
	        var rootFolder = viewPath;
	        if(slashIndex > 0)
	            rootFolder = viewPath.substring(0, slashIndex);
            ret = document.getElementById("divDAV_" + viewID).navigateFrame(rootFolder, "iframeDAV_" + viewID);
            ret = document.getElementById("divDAV_" + viewID).navigateFrame(viewPath, "iframeDAV_" + viewID);
            if (ret=="OK")
				isOk=true;
            else
                alert(ret);
		}*/
    }
    catch (e) {}
    if (!isOk &&
			(0==viewPath.search("http://[a-zA-Z0-9\-\.]+(:80)?/") || 0==viewPath.search("https://[a-zA-Z0-9\-\.]+(:80)?/")))
		{
			var sUrl=viewPath
				.replace(/http:\/\/([a-zA-Z0-9\-\.]+)(:80)?[\/]/, "//$1/")
				.replace(/https:\/\/([a-zA-Z0-9\-\.]+)(:80)?[\/]/, "//$1/")
				.replace(/[\/]/g, "\\");
			var targetFrame=document.frames.item("iframeDAV_" + viewID);
			if (targetFrame !=null)
			{
				try
				{
				        var davRoot = sUrl.indexOf("ekdavroot");
				        var slashIndex = sUrl.indexOf("\\", davRoot);
				        var rootFolder = sUrl;
				        if(slashIndex > 0)
				            rootFolder = sUrl.substring(0, slashIndex);
						targetFrame.onload=null;
						targetFrame.document.location.href=rootFolder;
						targetFrame.document.location.href=sUrl;
						isOk=true;
			            logEvent(3, "ExplorerView_Show:Vista Way");
				}
				catch (e) { }
			}
		}
		if (!isOk)
		{
			alert("Error Occured");
		}

}

//function ExplorerView_Login(username, password, viewID, viewPath)
//{
////    var request = new ActiveXObject("Microsoft.XMLHTTP");
////    var inner_viewID = viewID;
////    var inner_viewPath = viewPath;
////    request.open("PROPFIND", viewPath, true, username, password);
////    request.setRequestHeader("Depth", "1");
////    request.onreadystatechange = function() {
////        if (request.readyState==4)
////            ExplorerView_Show(inner_viewID, inner_viewPath);
////    };
////    request.send(null);
////    request.open("GET", viewPath, true, username, password);
////    request.onreadystatechange = function() {
////        if (request.readyState==4)
//            ExplorerView_Show(viewID, viewPath);
////    };
////    request.send(null);
//}

//function ExplorerView_AutoLogin(viewID, viewPath)
//{
//    var cookieEcm = GetCookie("ecm");
//    var username = GetEcmValue("username", cookieEcm) + "_ekdav";
//    var password = GetEcmValue("unique_id", cookieEcm);
//    ExplorerView_Login(username, password, viewID, viewPath);
//}

/**
 ** EXPLORER VIEW EVENTS
 **/
function ExplorerView_ReloadWorkarea()
{
     logEvent(3, "ExplorerView_ReloadWorkarea");
     if(top.frames["ek_main"] != null)
     {
        logEvent(3, "ExplorerView_ReloadWorkarea:InWorkArea");
        var buffer = '';
	    try {
	        buffer = new String( top.frames["ek_main"].location.href );
		    logEvent(3, "ExplorerView_ReloadWorkarea:buffer=" + buffer);
	    }
	    catch( ex ) {
		    logEvent(3, "ExplorerView_ReloadWorkarea:buffer error");
	    }
        if (buffer.indexOf("#") != -1)
	    {
            logEvent(3, "ExplorerView_ReloadWorkarea:Found a # sign");
		    var sUrl = top.frames["ek_main"].location.pathname;
		    logEvent(3, "ExplorerView_ReloadWorkarea:sUrl=" + sUrl);
		    var taxonomyId = "";
		    if(document.getElementById('taxonomyselectedtree') != null)
			    taxonomyId = document.getElementById('taxonomyselectedtree').value;
			else{
			    ExplorerQueryString.Parse(document.location.search);
                taxonomyId=ExplorerQueryString.Param("TaxonomyId");
            }
		    if(taxonomyId != "")
		    {
		        logEvent(3, "ExplorerView_ReloadWorkarea:taxonomyId=" + taxonomyId);
		        var tempBuffer = new String( top.frames["ek_main"].location.pathname );
		        logEvent(3, "ExplorerView_ReloadWorkarea:tempBuffer=" + tempBuffer);
		        if (tempBuffer.indexOf("__taxonomyid=") > -1)
		        {
		            var startindex = tempBuffer.indexOf("__taxonomyid=");
		            var endindex = tempBuffer.indexOf("&", startindex);

		            if (endindex == -1)
		            {
		                endindex = tempBuffer.length;
		                startindex--;
		            }
		            else
		                endindex++;

		            var replaceTerm = tempBuffer.substring(startindex, endindex);
		            tempBuffer = tempBuffer.replace(replaceTerm, "");
		        }

		        if (tempBuffer.indexOf("?") > -1)
			        sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			    else
			        sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;

		        logEvent(3, "ExplorerView_ReloadWorkarea:Final sUrl=" + sUrl);
			}
			var num = GetExisitingReloadImageNum(sUrl);
    	    sUrl = CleanExisitingReloadImageNum(sUrl);
            logEvent(3, "ExplorerView_ReloadWorkarea:Set Workarea Url=" + top.frames["ek_main"].location.href + ExplorerView_AddReloadImageFlag(top.frames["ek_main"].location.href));
            top.frames["ek_main"].location.href = sUrl + ExplorerView_AddReloadImageFlag(sUrl, num);

	    }
	    else
	    {
	        logEvent(3, "ExplorerView_ReloadWorkarea:Set Workarea Url=" + top.frames["ek_main"].location.href + ExplorerView_AddReloadImageFlag(top.frames["ek_main"].location.href));
	        var num = GetExisitingReloadImageNum(top.frames["ek_main"].location.href);
	    	var newUrl = CleanExisitingReloadImageNum(top.frames["ek_main"].location.href);
            top.frames["ek_main"].location.href = newUrl + ExplorerView_AddReloadImageFlag(newUrl, num);
        }
        if(top.frames["ek_nav_bottom"] != null)
        {
            try
            {
			    var obj=top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["ContentTree"];
                if(obj!=null){
                    logEvent(3, "ExplorerView_ReloadWorkarea:ek_nav_bottom:not null");
			        var node = obj.document.getElementById( "T" + g_folderId );
                    logEvent(3, "ExplorerView_ReloadWorkarea:ek_nav_bottom: " + node);
			        if(node!=null){
				        for (var i=0;i<node.childNodes.length;i++){
						    if(node.childNodes(i).nodeName=='LI' || node.childNodes(i).nodeName=='UL'){
							    var parent = node.childNodes(i).parentElement;
							    parent.removeChild( node.childNodes(i));
						    }
				        }
				        obj.TREES["T" + g_folderId].children = [];
				        obj.TreeDisplayUtil.reloadParentTree(g_folderId);
				        obj.onToggleClick(g_folderId,obj.callback_function,g_folderId);
			        }
			    }
			}
			catch(e)
			{
			    // not a major error, just log it...
                logEvent(3, "ExplorerView_ReloadWorkarea:ek_nav_bottom refresh error e=" + e.message);
			}
		}
     }
     else if(top.frames["mainFrame"] != null)
     {
        logEvent(3, "ExplorerView_ReloadWorkarea:InDeveloperSamples");
        var buffer = '';
	    try {
	        buffer = new String( top.frames["mainFrame"].location.href );
		    logEvent(3, "ExplorerView_ReloadWorkarea:buffer=" + buffer);
	    }
	    catch( ex ) {
		    logEvent(3, "ExplorerView_ReloadWorkarea:buffer error");
	    }
        if (buffer.indexOf("#") != -1)
	    {
            logEvent(3, "ExplorerView_ReloadWorkarea:Found a # sign");
		    var sUrl = top.frames["mainFrame"].location.pathname;
		    logEvent(3, "ExplorerView_ReloadWorkarea:sUrl=" + sUrl);
		    var taxonomyId = "";
		    if(document.getElementById('taxonomyselectedtree') != null)
			    taxonomyId = document.getElementById('taxonomyselectedtree').value;
			else{
			    ExplorerQueryString.Parse(document.location.search);
                taxonomyId=ExplorerQueryString.Param("TaxonomyId");
            }
		    if(taxonomyId != "")
		    {
		        logEvent(3, "ExplorerView_ReloadWorkarea:taxonomyId=" + taxonomyId);
		        var tempBuffer = new String( top.frames["mainFrame"].location.pathname );
		        logEvent(3, "ExplorerView_ReloadWorkarea:tempBuffer=" + tempBuffer);
		        if (tempBuffer.indexOf("__taxonomyid=") > -1)
		        {
		            var startindex = tempBuffer.indexOf("__taxonomyid=");
		            var endindex = tempBuffer.indexOf("&", startindex);

		            if (endindex == -1)
		            {
		                endindex = tempBuffer.length;
		                startindex--;
		            }
		            else
		                endindex++;

		            var replaceTerm = tempBuffer.substring(startindex, endindex);
		            tempBuffer = tempBuffer.replace(replaceTerm, "");
		        }

		        if (tempBuffer.indexOf("?") > -1)
			        sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			    else
			        sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;

		        logEvent(3, "ExplorerView_ReloadWorkarea:Final sUrl=" + sUrl);
			}

    	    var num = GetExisitingReloadImageNum(sUrl);
    	    sUrl = CleanExisitingReloadImageNum(sUrl);
            logEvent(3, "ExplorerView_ReloadWorkarea:Set Workarea Url=" + top.frames["mainFrame"].location.href + ExplorerView_AddReloadImageFlag(top.frames["mainFrame"].location.href));
            top.frames["mainFrame"].location.href = sUrl + ExplorerView_AddReloadImageFlag(sUrl, num);
	    }
	    else
	    {
	        logEvent(3, "ExplorerView_ReloadWorkarea:Set Workarea Url=" + top.frames["mainFrame"].location.href + ExplorerView_AddReloadImageFlag(top.frames["mainFrame"].location.href));
	        var num = GetExisitingReloadImageNum(top.frames["mainFrame"].location.href);
	    	var newUrl = CleanExisitingReloadImageNum(top.frames["mainFrame"].location.href);
            top.frames["mainFrame"].location.href = newUrl + ExplorerView_AddReloadImageFlag(newUrl, num);
        }
     }
     else
     {
		logEvent(3, "ExplorerView_ReloadWorkarea:Not in workarea");
     	var buffer = '';
     	var objDoc = null;
	    try {
	        if (top != null && top.opener != null)
	        {
	            buffer = new String( top.opener.location );
	            objDoc = top.opener;
	        }
	        else if(self != null && self.parent != null)
	        {
	            buffer = new String( self.parent.location );
	            objDoc = self.parent;
	        }
	        
		    logEvent(3, "ExplorerView_ReloadWorkarea:buffer=" + buffer);
	    }
	    catch( ex ) {
		    logEvent(3, "ExplorerView_ReloadWorkarea:buffer error");
	    }
	    var taxonomyId = "";
	    var sUrl ;
	    if (buffer.indexOf("#") != -1)
	    {
	        logEvent(3, "ExplorerView_ReloadWorkarea:Found a #");
	        sUrl = objDoc.location.pathname;
	        if(objDoc.location.search != "")
	          sUrl = sUrl + objDoc.location.search;
	        logEvent(3, "ExplorerView_ReloadWorkarea:sUrl=" + sUrl);
		    if(objDoc.document.getElementById('taxonomyselectedtree') != null)
			    taxonomyId = objDoc.document.getElementById('taxonomyselectedtree').value;

		    if(taxonomyId != "")
		    {
		        logEvent(3, "ExplorerView_ReloadWorkarea:taxonomyId=" + taxonomyId);
		        var tempBuffer = new String( objDoc.parent.location.pathname );
		        if(objDoc.parent.location.search != "")
		           tempBuffer = tempBuffer + objDoc.parent.location.search ; 
		        if (tempBuffer.indexOf("__taxonomyid=") > -1)
		        {

	                logEvent(3, "ExplorerView_ReloadWorkarea:Found Tax Id");
		            var startindex = tempBuffer.indexOf("__taxonomyid=");
		            var endindex = tempBuffer.indexOf("&", startindex);

		            if (endindex == -1)
		            {
		                endindex = tempBuffer.length;
		                startindex--;
		            }
		            else
		                endindex++;

		            var replaceTerm = tempBuffer.substring(startindex, endindex);
		            tempBuffer = tempBuffer.replace(replaceTerm, "");
		        }

		        if (tempBuffer.indexOf("?") > -1)
			        sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			    else
			        sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;

	            logEvent(3, "ExplorerView_ReloadWorkarea:sUrl=" + sUrl);
			}
			else
			{
			  if(sUrl.indexOf("__taxonomyid=") > -1)
			  {
			        logEvent(3, "ExplorerView_ReloadWorkarea:No TaxID Remove Existing");
			        var startindex = sUrl.indexOf("__taxonomyid=");
		            var endindex = sUrl.indexOf("&", startindex);

		            if (endindex == -1)
		            {
		                endindex = sUrl.length;
		                startindex--;
		            }
		            else
		                endindex++;

		            var replaceTerm = sUrl.substring(startindex, endindex);
		            sUrl = sUrl.replace(replaceTerm, "");
			  }
			}

	    	if (self.parent != null) {
	            logEvent(3, "ExplorerView_ReloadWorkarea:Parent=" + sUrl);
	            var num = GetExisitingReloadImageNum(sUrl);
	    	    sUrl = CleanExisitingReloadImageNum(sUrl);
                objDoc.location.href = sUrl + ExplorerView_AddReloadImageFlag(sUrl, num);
            } else {
	           logEvent(3, "ExplorerView_ReloadWorkarea:Top=" + sUrl);
	           var num = GetExisitingReloadImageNum(sUrl);
	    	   sUrl = CleanExisitingReloadImageNum(sUrl);
               objDoc.location.href = sUrl + ExplorerView_AddReloadImageFlag(sUrl, num);
            }
	    }
	    else
	    {
	        logEvent(3, "ExplorerView_ReloadWorkarea:No #");
	        var sUrl1="";
	        if(taxonomyId==""){
    	        ExplorerQueryString.Parse(document.location.search);
                taxonomyId=ExplorerQueryString.Param("TaxonomyId");
            }

		    logEvent(3, "ExplorerView_ReloadWorkarea:sUrl1=" + sUrl1);

            if (taxonomyId != "")
            {
                logEvent(3, "ExplorerView_ReloadWorkarea:taxonomyId=" + taxonomyId);
		        sUrl1 = sUrl1 + "?__taxonomyid=" + taxonomyId;
		    }

	    	if (parent != null) {
	    	    // 31403
	    	    if (sUrl1 != "")
	    	    {
		            logEvent(3, "ExplorerView_ReloadWorkarea:sUrl1 not 0");
	    		    sUrl1 = parent.location.href;
	    		    var tempBuffer = new String( parent.location.href );
                    if (tempBuffer.indexOf("__taxonomyid=") > -1)
                    {
		                logEvent(3, "ExplorerView_ReloadWorkarea:__taxonomyid exists");
                        var startindex = tempBuffer.indexOf("__taxonomyid=");
                        var endindex = tempBuffer.indexOf("&", startindex);

                        if (endindex == -1)
                        {
                            endindex = tempBuffer.length;
                            startindex--;
		                }
                        else
		                    endindex++;

                        var replaceTerm = tempBuffer.substring(startindex, endindex);
                        tempBuffer = tempBuffer.replace(replaceTerm, "");
                        logEvent(3, "ExplorerView_ReloadWorkarea:tempBuffer=" + tempBuffer);
                    }

                    if (tempBuffer.indexOf("?") > -1)
                    {
                        var poundsign = tempBuffer.indexOf("#");
                        if (poundsign > -1)
                        {
			                if(tempBuffer.substring(0,poundsign).lastIndexOf("&") == tempBuffer.substring(0,poundsign).length)
			                {
                           		sUrl1 = tempBuffer.substring(0,poundsign) + "__taxonomyid=" +taxonomyId;
			                }
			                else
			                {
                            	sUrl1 = tempBuffer.substring(0,poundsign) + "&__taxonomyid=" +taxonomyId;
			                }
			                var num = GetExisitingReloadImageNum(sUrl1);
	    	                sUrl1 = CleanExisitingReloadImageNum(sUrl1);
                            sUrl1 = sUrl1 + ExplorerView_AddReloadImageFlag(sUrl1, num) + tempBuffer.substring(poundsign);
                            logEvent(3, "ExplorerView_ReloadWorkarea:(w/ pound)sUrl1=" + sUrl1);
                        }
                        else
                        {
			                if(tempBuffer.lastIndexOf("&") == tempBuffer.length)
			                {
                                logEvent(3, "ExplorerView_ReloadWorkarea:(w/out pound)sUrl1=" + sUrl1);
                                sUrl1 = tempBuffer + "__taxonomyid=" +taxonomyId;
			                }
			                else
			                {
                            	sUrl1 = tempBuffer + "&__taxonomyid=" +taxonomyId;
			                }
			                var num = GetExisitingReloadImageNum(sUrl1);
	    	                sUrl1 = CleanExisitingReloadImageNum(sUrl1);
	                        sUrl1 = sUrl1 + ExplorerView_AddReloadImageFlag(sUrl1, num);
                            logEvent(3, "ExplorerView_ReloadWorkarea:(w/out pound)sUrl1=" + sUrl1);
	                    }
	                }
	                else
	                {
	                    sUrl1 = tempBuffer + "?__taxonomyid=" + taxonomyId;
	                    var num = GetExisitingReloadImageNum(sUrl1);
	    	            sUrl1 = CleanExisitingReloadImageNum(sUrl1);
	                    sUrl1 = sUrl1 + ExplorerView_AddReloadImageFlag(sUrl1, num);
                        logEvent(3, "ExplorerView_ReloadWorkarea:(w/out ?)sUrl1=" + sUrl1);
	                }
	                var querystring = sUrl1.substring(sUrl1.indexOf("?"), sUrl1.length);
	                logEvent(3, "ExplorerView_ReloadWorkarea:Parent=" + parent.location.pathname + querystring);
	                parent.location.href = parent.location.pathname + querystring;
		        }
		        else
		        {
	                logEvent(3, "ExplorerView_ReloadWorkarea:Parent=" + parent.location.pathname + parent.location.search + ExplorerView_AddReloadImageFlag(parent.location.pathname + parent.location.search));
			        var num = GetExisitingReloadImageNum(parent.location.pathname + parent.location.search);
	    	        var newUrl = CleanExisitingReloadImageNum(parent.location.pathname + parent.location.search);
			        parent.location.href = newUrl + ExplorerView_AddReloadImageFlag(parent.location.pathname + parent.location.search, num);
			    }
		    }
		    else
		    {
                var tempBuffer = new String( top.location.href );
			    var poundsign = tempBuffer.indexOf("#");
			    var newUrl = tempBuffer;
                if (poundsign > -1)
                {
                    newUrl = tempBuffer.substring(0,poundsign);
                    var num = GetExisitingReloadImageNum(newUrl);
	    	        newUrl = CleanExisitingReloadImageNum(newUrl);
                    newUrl = newUrl + ExplorerView_AddReloadImageFlag(newUrl, num);
                    newUrl=newUrl.replace(/__taxonomyid=[0-9]+/ig,'__taxonomyid='+taxonomyId);
                    logEvent(3, "ExplorerView_ReloadWorkarea:(w/ pound)newUrl=" + newUrl);
                }
			    else
			    {
                    var num = GetExisitingReloadImageNum(newUrl);
	    	        newUrl = CleanExisitingReloadImageNum(newUrl);
			        newUrl = newUrl + ExplorerView_AddReloadImageFlag(newUrl, num);
			    }
			    logEvent(3, "ExplorerView_ReloadWorkarea:(w/out ?)newUrl=" + newUrl);
                top.location.href = newUrl;
            }
		}
	}
}
function GetExisitingReloadImageNum(url)
{
    if(url.indexOf("ekimgreload=2") > 0)
    	return 2;
    else
        return 1;
}

function CleanExisitingReloadImageNum(url)
{
    var newUrl = new String( url );
    // if it is just one parameter in a string of them, remove it
    newUrl = newUrl.replace("&ekimgreload=1", "");
    newUrl = newUrl.replace("&ekimgreload=2", "");

    // if it is the first parameter in a string with others, make sure to switch the next param's '&' with a '?'
    newUrl = newUrl.replace("?ekimgreload=1&", "?");
    newUrl = newUrl.replace("?ekimgreload=2&", "?");

    // if it is the only param, just remove it
    newUrl = newUrl.replace("?ekimgreload=1", "");
    newUrl = newUrl.replace("?ekimgreload=2", "");
    return newUrl;
}

function ExplorerView_AddReloadImageFlag(path, num){
    logEvent(3, "AddReloadImageFlag:path=" + path);

    if(num != 1)
        num = 1;
    else
        num = 2;

    var result = "";
    var delim = (path.indexOf("?") >= 0) ? "&" : "?";
    var parmName = "ekimgreload";
    if (path.indexOf(parmName) < 0){
        result += delim + parmName + "=" + num;
    }
    return (result);
}

/**
 ** FIREFOX FUNCTIONS
 **/
function ExplorerView_SetDropUrl(url, viewID)
{
	if("createEvent" in document)
	{
		var element = document.createElement(viewID + "_ExplorerView_Data");
		element.setAttribute("id", "ekdragdrop");
		var location = "";
		if(top.frames["ek_main"] != null)
		{
		    location = top.frames["ek_main"].location.href;
		}
		else if(top.frames["mainFrame"] != null)
        {
            var buffer = '';
	        try {
	            buffer = new String( top.frames["mainFrame"].location.href );
	        }
	        catch( ex ) {
	        }
            if (buffer.indexOf("#") != -1)
	        {
		        var sUrl = top.frames["mainFrame"].location.href;
		        var taxonomyId = "";
		        if(document.getElementById('taxonomyselectedtree') != null)
			        taxonomyId = document.getElementById('taxonomyselectedtree').value;
			    else{
			        ExplorerQueryString.Parse(document.location.search);
                    taxonomyId=ExplorerQueryString.Param("TaxonomyId");
                }
		        if(taxonomyId != "")
		        {
		            var tempBuffer = new String( top.frames["mainFrame"].location.href );
		            if (tempBuffer.indexOf("__taxonomyid=") > -1)
		            {
		                var startindex = tempBuffer.indexOf("__taxonomyid=");
		                var endindex = tempBuffer.indexOf("&", startindex);

		                if (endindex == -1)
		                {
		                    endindex = tempBuffer.length;
		                    startindex--;
		                }
		                else
		                    endindex++;

		                var replaceTerm = tempBuffer.substring(startindex, endindex);
		                tempBuffer = tempBuffer.replace(replaceTerm, "");
		            }

		            if (tempBuffer.indexOf("?") > -1)
			            sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			        else
			            sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;

			    }
    	       var num = GetExisitingReloadImageNum(sUrl);
    	       sUrl = CleanExisitingReloadImageNum(sUrl);
               location = sUrl + ExplorerView_AddReloadImageFlag(sUrl);
	        }
	        else
	        {
	            location = top.frames["mainFrame"].location.href;
	        }
	    }
		else
		{
		    var buffer = '';
	        try {
	            buffer = new String( top.opener.location );
	        }
	        catch( ex ) {
	        }
	        if (buffer.indexOf("#") != -1)
	        {
		        var sUrl = parent.location.pathname;
		        var taxonomyId = "";
		        if(document.getElementById('taxonomyselectedtree') != null)
			        taxonomyId = document.getElementById('taxonomyselectedtree').value;
		        if(taxonomyId != "")
		        {
		            var tempBuffer = new String( parent.location.pathname );
		            if (tempBuffer.indexOf("__taxonomyid=") > -1)
		            {
		                var startindex = tempBuffer.indexOf("__taxonomyid=");
		                var endindex = tempBuffer.indexOf("&", startindex);

		                if (endindex == -1)
		                {
		                    endindex = tempBuffer.length;
		                    startindex--;
		                }
		                else
		                    endindex++;

		                var replaceTerm = tempBuffer.substring(startindex, endindex);
		                tempBuffer = tempBuffer.replace(replaceTerm, "");
		            }

		            if (tempBuffer.indexOf("?") > -1)
			            sUrl = tempBuffer + "&__taxonomyid=" +taxonomyId;
			        else
			            sUrl = tempBuffer + "?__taxonomyid=" + taxonomyId;
			    }
	    	    location = sUrl;
	        }
	        else
	        {
	    	    if (parent != null) {
		            location = parent.location.pathname;
		        } else {
                    location = top.location.href;
		        }

		        if(location.indexOf("http") < 0)
		            location = top.location.href;
		    }
		}

        element.setAttribute("url", url + "#;#" + g_folderId + "#;#" + g_metaRequired + "#;#" + location + "#;#" + g_fileTypes + "#;#" + g_cmsFolderPath + "#;#" + g_langType + "#;#" + g_TaxId);
		document.documentElement.appendChild(element);

		var evt = document.createEvent("Events");
		evt.initEvent("ek_onDragDropInit", true, false);
		element.dispatchEvent(evt);

		return true;
	}
	return false;
}

function ExplorerView_Ping(url, viewID)
{
    var element = document.createElement(viewID + "_ExplorerView_Ping");
	element.addEventListener("ek_pongExtension", function(){ExplorerView_PongListener(url,viewID);},false, true);
    document.documentElement.appendChild(element);

    var evt = document.createEvent("Events");
	evt.initEvent("ek_pingExtension", true, false);
	element.dispatchEvent(evt);
	 var waitLoad = document.getElementById("ek_DmsLoadWait")
       if (waitLoad != null)
         {
              waitLoad.style.position = 'absolute';
              waitLoad.style.left = '-10000px';
         }
}

function ExplorerView_SetCallBack(viewID)
{
    var element = document.createElement(viewID + "_ExplorerView_CallBack");
	element.addEventListener("ek_doCallBackExtension", function(){ExplorerView_CallBackCheckListener(viewID);},false, true);
    document.documentElement.appendChild(element);

    // we want to set the callback to run if the callback function exists on the page
    if(typeof parent.EktUploadCompleteCallback == 'function')
    {
        var evt = document.createEvent("Events");
        evt.initEvent("ek_CallBackHandler", true, false);
        element.dispatchEvent(evt);
    }
}

var EkExplorerViewSetUp =
{
    url : "",
    ViewID : "",
    PluginExists : false
};

function ExplorerView_PongListener(url, viewID)
{
    document.getElementById("divDavStatus").innerHTML = "";
    ExplorerView_SetDropUrl(url, viewID);
    EkExplorerViewSetUp.url = url;
	EkExplorerViewSetUp.ViewID = viewID;
	EkExplorerViewSetUp.PluginExists = true;

}

function ExplorerView_CallBackCheckListener(viewID)
{
	EkExplorerViewSetUp.CallBackCapable = true;
}

if (navigator.userAgent.indexOf("Firefox")!=-1)
{
    window.addEventListener("ek_pongExtension", ExplorerView_PongListener, false, true);
    window.addEventListener("ek_doCallBackExtension", ExplorerView_CallBackCheckListener, false, true);
	docallback("ifCodeUpload");
    attachListenerSocket("ifCodeUpload");
}
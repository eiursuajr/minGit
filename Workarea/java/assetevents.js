// Copyright 2005, Ektron, Inc.

// associated with edit.asp


function AssetHandler(name)
{
	// Properties
	this.name = name;
	this.action = "";
	this.formName = 0;
	this.actionPage = "";
	this.languageCode = 1033;
	this.folderId = 0;
	this.pubHtml = 0;
	this.method = "add";
	this.contId =  -1;
	this.Edit = 0; //Set this to raise save event insted of publish when item dragged and dropped on the control.
	
	// Methods
	this.SetAction = AssetHandler_SetAction;
	this.StoreAssetInfo = AssetHandler_StoreAssetInfo;
	this.SetPostInfo = AssetHandler_SetPostInfo;
	this.SetFormPostInfo = AssetHandler_SetFormPostInfo;
	this.SetFormDataInfo = AssetHandler_SetFormDataInfo;
    this.SetPostEventSave = AssetHandler_SetPostEventSave; //Set only if you want to trigger save instead of publish when using dargdrop control.
	// Events
	this.OnAssetUserRequestEvent = AssetHandler_OnAssetUserRequestEvent;
	this.OnAssetEvent = AssetHandler_OnAssetEvent;
		
	this.trace = AssetHandler_trace;
	this.traceEnabled = false;
	if (this.traceEnabled)
	{
		document.writeln("<textarea name='AssetHandlerLog' cols='80' rows='12'>AssetHandler log " + new Date() + "</textarea>");
	}
	//cannot call trace until the formName is defined.
	//this.trace("AssetHandler log " + new Date());
}

// Methods

// EktAsset methods
// .createNewAsset
// .checkOut
// .undoCheckOut
// .checkIn
// .publish
// .saveToAssetServer
// .restore
// .SetPostInfo
// .SetFormPostInfo
function AssetHandler_SetPostEventSave(bool_save)
{
    //Set to true when in edit screen
    if (bool_save){
        this.Edit = 1;        
    } else {
        this.Edit = 0;        
    }
}

function AssetHandler_SetPostInfo(actpage, lang, folder, pubHtml,method,contId,batchsize)
{
	// this should be called after the control is ready
	//debugger
	this.actionPage = actpage;
	this.languageCode = lang;
	this.folderId = folder;
	this.pubHtml = pubHtml;
	this.method = method;
	this.contId = contId;
	this.batchsize = batchsize || 4;	//#DMSfiles to batch process (defaults if unspecified)

	// set the control's parameters to enable posting when ektexplorer sends configure event
	//have to use the 0 instance instead of the this.name instance because this.name refers to ekMain instead of astViewer!
	var objInstance = EktAsset.instances[0];
	if (typeof objInstance != "object" || (null == objInstance))
	{
		alert("Asset control instance not found. Name=" + this.name);
		return ("cancel" == action); 
	}
	//alert(typeof objInstance.editor);
	var objControlInstance = objInstance.editor;	// get a handle to the real ektexplorer obj
	if ((typeof objControlInstance != "object" && typeof objControlInstance != "function") || (null == objControlInstance))
	{
		return;
	}
	//alert("Asset control instance found. Name=");
	objControlInstance.SetPostParameters(1,this.batchsize,this.actionPage);
	objControlInstance.SetNamedData("ContentLanguage",this.languageCode);
	objControlInstance.SetNamedData("FolderID", this.folderId);
	objControlInstance.SetNamedData("PubAsHtml", this.pubHtml);
	objControlInstance.SetNamedData("Method", this.method);
	objControlInstance.SetNamedData("ContentId", this.contId);
	var cookVal = CookieUtil.getCookie("ecm","");
	var cookRet = cookVal["user_id"]+ "&site_id=" + cookVal["site_id"];
	objControlInstance.SetNamedData("CookieVal",cookRet);
	if (document.forms[0].content_media_html != null && document.forms[0].content_media_html != undefined && document.forms[0].content_media_html.value != '')
	{
		objControlInstance.SetNamedData("content_html",document.forms[0].content_media_html.value);
		objControlInstance.SetNamedData("contenttype","104");
	}	
	if(document.forms[0].chkLockedContentLink != null && document.forms[0].chkLockedContentLink != undefined && document.forms[0].chkLockedContentLink.value != '')
	{
	    if(document.forms[0].chkLockedContentLink.value == "on")
	       objControlInstance.SetNamedData("LockedContentLink",1);
	    else
	        objControlInstance.SetNamedData("LockedContentLink",0);
	}
	else
	    objControlInstance.SetNamedData("LockedContentLink",0);
	
	//Chandra removed comments and added this
	//uploading file any way deletes session
	//if (navigator.appName != 'Microsoft Internet Explorer')
	//{
	   var dc = document.cookie;
	   if (dc != "") 
	   {
		//var cookies = dc.split("; ");
	     objControlInstance.SetPostInfo("Cookie", dc);
	   }
	//}
}

function AssetHandler_SetFormDataInfo(data)
{
    // set form fields from content edit page so we can do the http post properly
	var objInstance = EktAsset.instances[0];
	if (typeof objInstance != "object" || (null == objInstance))
	{
		alert("Asset control instance not found. Name=" + this.name);
		return ("cancel" == action); 
	}
	
	 if ((typeof objInstance.editor == "undefined") || (objInstance.editor == null))
	{  	
		return;
	}	
	objInstance = objInstance.editor;	// get a handle to the real ektexplorer obj
		
	// set checkin/publish
	objInstance.SetNamedData("taxonomyselectedtree",data);
	
	
}


function AssetHandler_SetFormPostInfo(actiontype)
{
    // set form fields from content edit page so we can do the http post properly
	var objInstance = EktAsset.instances[0];
	if (typeof objInstance != "object" || (null == objInstance))
	{
		alert("Asset control instance not found. Name=" + this.name);
		return ("cancel" == action); 
	}
	
	 if ((typeof objInstance.editor == "undefined") || (objInstance.editor == null))
	{  	
		return;
	}	
	objInstance = objInstance.editor;	// get a handle to the real ektexplorer obj
		
	// set checkin/publish
	objInstance.SetNamedData("editaction",actiontype);
	// blank values cause MSIE to blow up, the don't send values if they're blank :-P

	if (document.forms[0].content_title != null && document.forms[0].content_title.value != '')
	{
		objInstance.SetNamedData("content_title",document.forms[0].content_title.value);
	}
	if (document.forms[0].content_image != null && document.forms[0].content_image != undefined && document.forms[0].content_image != '')
	{
		objInstance.SetNamedData("content_image",document.forms[0].content_image.value);
	}
	
	if (document.forms[0].content_type.value != '')
	{
		objInstance.SetNamedData("contenttype",document.forms[0].content_type.value);
	}
		
	if (document.forms[0].content_teaser != null && document.forms[0].content_teaser.value != '')
		objInstance.SetNamedData("ContentTeaser",document.forms[0].content_teaser.value);
	//The following lines added by Udai for the defect# 14895
	if (document.forms[0].content_comment != null && document.forms[0].content_comment.value != '')
		objInstance.SetNamedData("content_comment",document.forms[0].content_comment.value);
		
	if (document.forms[0].frm_manalias != null && document.forms[0].frm_manalias.value != '')
	{
	    if(document.forms[0].ast_frm_manaliasExt.value != "")
	    {
		    objInstance.SetNamedData("ast_man_alias",document.forms[0].frm_manalias.value + document.forms[0].ast_frm_manaliasExt.value);
		}
		else if(document.forms[0].frm_ManAliasExt != null && document.forms[0].frm_ManAliasExt.value != "")
		{
		    objInstance.SetNamedData("ast_man_alias",document.forms[0].frm_manalias.value + document.forms[0].frm_ManAliasExt.value);
		}
		    
	}
	if (document.forms[0].content_html != null && document.forms[0].content_html.value != '')
		objInstance.SetNamedData("content_html",document.forms[0].content_html.value);
		
	if (document.forms[0].AddQlink != null && document.forms[0].AddQlink.checked)
	{
		objInstance.SetNamedData("AddQlink",document.forms[0].AddQlink.value);		
	}
	else
	{
		objInstance.SetNamedData("AddQlink","");				
	}
	if (document.forms[0].IsSearchable != null && document.forms[0].IsSearchable.checked)
	{
		objInstance.SetNamedData("IsSearchable",document.forms[0].IsSearchable.value);	
	}
	else
	{
		objInstance.SetNamedData("IsSearchable","");		
	}
	if (document.forms[0].taxonomyselectedtree.value != '')
		objInstance.SetNamedData("taxonomyselectedtree",document.forms[0].taxonomyselectedtree.value);
	if (document.forms[0].go_live.value != '')
		objInstance.SetNamedData("go_live",document.forms[0].go_live.value);
	if (document.forms[0].end_date.value != '')
		objInstance.SetNamedData("end_date",document.forms[0].end_date.value);
		
	var radioGroup=document.forms[0].end_date_action_radio;
	for (var i=0;i<radioGroup.length;i++)
	{
		if (radioGroup[i].checked)
		{
			objInstance.SetNamedData("end_date_action_radio",radioGroup[i].value);
		}	
	}		
		
	// send metadata
	if(typeof document.forms[0].frm_validcounter != "undefined"){
		var metacount = document.forms[0].frm_validcounter.value; 
    }else{
        var metacount =0;
    }
	objInstance.SetNamedData("metacount", metacount);
	for (var i = 1;  i <= metacount;  i++) {
		var metatype = eval("document.forms[0].frm_meta_type_id_" + i + ".value");
		var metaval = eval("document.forms[0].frm_text_" + i + ".value");
		objInstance.SetNamedData("metatype"+i,metatype);
		if (metaval != '')
			objInstance.SetNamedData("metaval"+i,metaval);
	}
	
	//send subscriptions
	if (document.forms[0].suppress_notification != undefined) {
		// no notifications!
		objInstance.SetNamedData("suppressnotification","true");
	}
	else {
		// get the stuff
		if (document.forms[0].notify_option[0].checked == true) {
			objInstance.SetNamedData("notify_option",document.forms[0].notify_option[0].value);
		}
		else if (document.forms[0].notify_option[1].checked == true) {
			objInstance.SetNamedData("notify_option",document.forms[0].notify_option[1].value);
		}
		else if (document.forms[0].notify_option[2].checked == true) {
			objInstance.SetNamedData("notify_option",document.forms[0].notify_option[2].value);
		}
		
		if (document.forms[0].suspend_notification_button.checked == true)
		objInstance.SetNamedData("suspendnotification","true");
		
		if (document.forms[0].send_notification_button.checked == true)
		objInstance.SetNamedData("sendnotification","true");
		
		if (document.forms[0].notify_subject.value != '')
		objInstance.SetNamedData("NotifySubject",document.forms[0].notify_subject.value);
		
		//if (document.forms[0].notify_url.value != '')
		//objInstance.SetNamedData("NotifyURL",document.forms[0].notify_url.value);
	
		if (document.forms[0].notify_emailfrom.value != '')
		objInstance.SetNamedData("notifyemailfrom",document.forms[0].notify_emailfrom.value);
		
		//if (document.forms[0].notify_weblocation.value != '')
		//objInstance.SetNamedData("notifyweblocation",document.forms[0].notify_weblocation.value);
	
			if (document.forms[0].notify_optoutid.value != '')
			objInstance.SetNamedData("notifyoptoutid",document.forms[0].notify_optoutid.value);
				
			if (document.forms[0].use_message_button.checked == true) {
				objInstance.SetNamedData("notifymessageid",document.forms[0].notify_messageid.value);
			}
			else {
				objInstance.SetNamedData("notifymessageid",0);
			}
			
			if (document.forms[0].use_summary_button.checked == true) {
				objInstance.SetNamedData("notifysummaryid",1);
			}
			else {
				objInstance.SetNamedData("notifysummaryid",0);
			}
			if (validateObject(document.forms[0].use_contentlink_button)) {
				if (document.forms[0].use_contentlink_button.checked == true) {
					objInstance.SetNamedData("notifycontentlink",1);
				}
				else {
					objInstance.SetNamedData("notifycontentlink",0);
				}
			} else {
				objInstance.SetNamedData("notifycontentlink",0);
			}
			if (document.forms[0].use_content_button.checked == true) {
				if (document.forms[0].frm_content_id.value != '') {
					objInstance.SetNamedData("notifycontentid",document.forms[0].frm_content_id.value);
					objInstance.SetNamedData("content_id",document.forms[0].frm_content_id.value);
				}
				else {
					objInstance.SetNamedData("notifycontentid",0);
					objInstance.SetNamedData("content_id",0);
				}
			}
			else {
				objInstance.SetNamedData("notifycontentid",0);
			}
			
			if (document.forms[0].titlename.value != '')
			objInstance.SetNamedData("titlename",document.forms[0].titlename.value);
			
			if (document.forms[0].notify_unsubscribeid.value != '')
			objInstance.SetNamedData("notifyunsubscribeid",document.forms[0].notify_unsubscribeid.value);
			
			var idx, tableObj, qtyElements, retStr;
            tableObj = tableObj = document.getElementById('therows');
            tableObj = tableObj.getElementsByTagName('input');
            retStr = '';
            if ((validateObject(tableObj))){
				qtyElements = tableObj.length;
                for(idx = 0; idx < qtyElements; idx++ ) {
					if ((tableObj[idx].type == 'checkbox') && tableObj[idx].checked){
           			    retStr = retStr + tableObj[idx].name + ' ';
					}
				}
			}
			if (retStr != '')
			objInstance.SetNamedData("contentsubassignments",retStr);
	}
}

function AssetHandler_SetAction(action, sFilename)
// returns true to continue, false to cancel
{
   	var bRet = true;
	action = action + ""; // ensure it is a string
	action = action.toLowerCase();
	if (typeof EktAsset != "object")
	{
		// For debugging...
		//StoreAssetInfo("pseudofile.doc", "pseudoid");
		/* 
		var data = "";
		data += "<AssetInfo>";
		data += "<AssetFilename>pseudo,plain,text,file.txt</AssetFilename>";
		data += "<AssetID>pseudoid</AssetID>";
		data += "<MimeType>text/plain</MimeType>";
		data += "<FileExtension>txt</FileExtension>";
		data += "<ErrorNumber>0</ErrorNumber>";
		data += "<ErrorMessage></ErrorMessage>";
		data += "</AssetInfo>";
		data += "<AssetInfo>";
		data += "<AssetFilename>pseudofile.jpg</AssetFilename>";
		data += "<AssetID>pseudoid</AssetID>";
		data += "<MimeType>image/jpeg</MimeType>";
		data += "<FileExtension>jpg</FileExtension>";
		data += "<ErrorNumber>0</ErrorNumber>";
		data += "<ErrorMessage></ErrorMessage>";
		data += "</AssetInfo>";
		data += "<AssetInfo>";
		data += "<AssetFilename>pseudofile.doc</AssetFilename>";
		data += "<AssetID>pseudoid</AssetID>";
		data += "<MimeType>application/msword</MimeType>";
		data += "<FileExtension>doc</FileExtension>";
		data += "<ErrorNumber>0</ErrorNumber>";
		data += "<ErrorMessage></ErrorMessage>";
		data += "</AssetInfo>";
		data += "<AssetInfo>";
		data += "<AssetFilename>pseudofile.zip</AssetFilename>";
		data += "<AssetID>pseudoid</AssetID>";
		data += "<MimeType>application/zip</MimeType>";
		data += "<FileExtension>zip</FileExtension>";
		data += "<ErrorNumber>0</ErrorNumber>";
		data += "<ErrorMessage></ErrorMessage>";
		data += "</AssetInfo>";
		StoreAssetInfo("", "", data);
		return true; // continue, don't abort action
		*/
	
		if (action != "cancel")
		{
			alert("The asset control was not created. Unable to perform action: '" + action + "'.");
		}
		return ("cancel" == action); 
	}
	var objInstance = EktAsset.instances[this.name];
	if (typeof objInstance != "object" || (null == objInstance))
	{
		alert("Asset control instance not found. Name=" + this.name);
		return ("cancel" == action); 
	}
	this.trace("SetAction('" + action + "') {enter}");
	 //alert(document.cookie);
	   
	switch (action)
	{
	case "new":
	    //var dc = document.cookie;
		objInstance.createNewAsset();
		//alert(dc);
		//document.cookie = dc;
		break;
	case "checkout":
		objInstance.checkOut();
		break;
	case "cancel":
		if (action != this.action)
		{
			objInstance.undoCheckOut();
			this.action = action; // if user clicks again, then continue cancel
			bRet = false;
		}
		else
		{
			this.action = "";
		}
		break;
	case "checkin":
		if (action != this.action)
		{
		//    var dc = document.cookie;
			objInstance.checkIn(sFilename);
	//		alert(dc);
	//		document.cookie = dc;
			bRet = false;
		}
		else
		{
			this.action = "";
		}
		break;
	case "checkintype":
		if ("checkin" != this.action)
		{
	//	    var dc = document.cookie;
			objInstance.checkInAsType(sFilename,"Html");
	//		alert(dc);
	//		document.cookie = dc;
			bRet = false;
		}
		else
		{
			this.action = "";
		}
		break;
	case "publish":
		if (action != this.action)
		{
		    //var dc = document.cookie;
			objInstance.publish(sFilename);
			//alert(dc);
			//document.cookie = dc;
			bRet = false;
		}
		else
		{
			this.action = "";
		}
		break;
	case "publishtype":
		if ("publish" != this.action)
		{
		    //var dc = document.cookie;
			objInstance.publishAsType(sFilename,"Html");
			//alert(dc);
			//document.cookie = dc;
			bRet = false;
		}
		else
		{
			this.action = "";
		}
		break;
	case "save":
		if (action != this.action)
		{
		    //var dc = document.cookie;
			objInstance.saveToAssetServer(sFilename);
			//alert(dc);
			//document.cookie = dc;
			bRet = false;
		}
		else
		{
			this.action = "";
		}
		break;
	case "savetype":
		if ("save" != this.action)
		{
		    //var dc = document.cookie;
			objInstance.saveAsType(sFilename);
			//alert(dc);
			//document.cookie = dc;
			bRet = false;
		}
		else
		{
			this.action = "";
		}
		break;
	case "workoffline":
		if (action != this.action)
		{
			if ("function" == typeof objInstance.workOffline)
			{
				objInstance.workOffline();
			}
			else
			{
				alert("Work Offline is not supported for this type of asset.");
			}
			bRet = false;
		}
		else
		{
			this.action = "";
		}	
		break;
	case "savelocalcopy":
		if (action != this.action)
		{
		    if ("function" == typeof objInstance.saveLocalCopy)
			{
				objInstance.saveLocalCopy();
			}
			else
			{
				alert("Save Local Copy is not supported for this type of asset.");
			}
			bRet = false;
		}
		else
		{
			this.action = "";
			bRet = false;
		}	
		break;
	default:
		this.action = action;
		break;
	}
	this.trace("SetAction('" + action + "') {exit}");
	return bRet;
}


// Events

function AssetHandler_OnAssetUserRequestEvent()
{
	// Event handlers for global EktAsset 
	// EktAsset.event.srcName
	// EktAsset.event.userRequest

	// Events
	// onuserrequest
	
	var sInstanceName = EktAsset.event.srcName;
	var sEventName = EktAsset.event.eventName;
	var userRequest = EktAsset.event.userRequest;
	var bTakeAction = false;
	var bEventComplete = false;
	var sActionRequested = "";
	
	this.trace(sInstanceName + "." + sEventName + "('" + userRequest + "')");
	
	switch (userRequest.toLowerCase())
	{	        	    
	case "cancelnew":
		sActionRequested = "cancel";
		bEventComplete = true; // Don't call .undoCheckout method.
		bTakeAction = true;
		break;
	case "undocheckout":
		sActionRequested = "cancel";
		bTakeAction = true;
		break;
	case "checkin":
		sActionRequested = "checkin";
		bTakeAction = true;
		break;
	case "publish":
	    if (this.Edit == 1) {
		    sActionRequested = "save";
	    } else {
	        sActionRequested = "publish";
	    }
		bTakeAction = true;	
		break;
	case "savetoassetserver":
		sActionRequested = "save";
		bTakeAction = true;
		break;
	case "workoffline":
		sActionRequested = "workoffline";
		bTakeAction = true;
		break;
	case "savelocalcopy":
		sActionRequested = "savelocalcopy";
		bTakeAction = true;
		break;
	default:
		alert("Unknown user request '" + userRequest + "' from instance " + sInstanceName);
		break;
	}

	if (bTakeAction)
	{
	    if (bEventComplete)
		{
			this.action = sActionRequested;
		}
		else
		{
			this.action = "";
		}
		// this is a workaround to change the focus of the page to a text field.  So, the activeX would not
		// unload itself before everything is saved.
		// window.document.frmMain.content_title.focus();
		var oForm = window.document.forms[this.formName];
		if (oForm && typeof oForm.content_title != "undefined")
		{
			// before attemptin to set the focus, ensure that the 
			// control is visisble (i.e. not in full-screen mode):
			EnsureNotFullScreenMode();
			oForm.content_title.focus();
		}
		SetAction(sActionRequested);
	}
	
	return;
}

// if full-screen-mode is active and code attempts to set 
// the focus to an off-screen control, error occurs...
function EnsureNotFullScreenMode()
{
	if ("function" == (typeof(SetFullScreenView)).toLowerCase()){
		SetFullScreenView(false);
	}
}

function AssetHandler_OnAssetEvent()
{
	// Event handlers for global EktAsset 
	// EktAsset.event.srcName
	// EktAsset.event.item
	// EktAsset.event.file (or .data)
	// EktAsset.event.assetid (or .data)
	// EktAsset.event.data (or .file and .assetid)
	// EktAsset.event.errno
	// EktAsset.event.errormessage

	// Events
	// oncheckout
	// onundocheckout
	// oncheckin 
	// onpublish
	// onsavetoassetserver
	// onrestore
	//debugger
	var sInstanceName = EktAsset.event.srcName;
	var sEventName = EktAsset.event.eventName;
	var item = EktAsset.event.item;
	var file = EktAsset.event.file;
	var assetid = EktAsset.event.assetid;
	var data = EktAsset.event.data;
	var errno = EktAsset.event.errno;
	var errormessage = EktAsset.event.errormessage;
	var sEventType = sEventName.toLowerCase();

	if ("on" == sEventType.substring(0, 2))
	{
		// Remove the "on" prefix from the event name.
		// This mimics the 'type' property in the standard
		// event object.
		sEventType = sEventType.substring(2);
	}

	if (isNaN(errno))
	{
		errno = 0;
	}
	else
	{
		errno = parseInt(errno, 10);
	}
	// Can't trust the ActiveX control to provide consistent information, so check for everything.
	var bEventComplete = ((100 == item) && !errno /*&& !errormessage*/); // 100% percent complete?
	var bTakeAction = false;
	var bStoreAssetInfo = false;

	if ("undefined" == typeof(data))
	{
		this.trace(sInstanceName + "." + sEventName + "(" + item + ",'" + file + "','" + assetid + "'," + errno + ",'" + errormessage + "')");
	}
	else
	{
		this.trace(sInstanceName + "." + sEventName + "(" + item + "," + errno + ",'" + errormessage + "')");
		this.trace("data:");
		this.trace(data);
	}
	switch (sEventType)
	{
	case "checkout":
		this.action = "checkout";
		break;
	case "undocheckout":
		this.action = "cancel";
		bTakeAction = true;
		break;
	case "checkin":
		this.action = "checkin";
		bStoreAssetInfo = true;
		bTakeAction = true;
		break;
	case "publish":
	    if(this.Edit == 1) {
	        this.action = "save"
	    } 
	    else 
	    {
	        this.action = "publish";
		}
		bStoreAssetInfo = true;
		bTakeAction = true;
		break;
	case "savetoassetserver":
		this.action = "save";
		bStoreAssetInfo = true;
		bTakeAction = true;
		break;
	case "restore":
		this.action = "restore";
		break;
	case "workoffline":
		this.action = "workoffline";
		bTakeAction = true;
		break;
	case "savelocalcopy":
		this.action = "savelocalcopy";
		bTakeAction = true;
		break;
	default:
		alert("Unknown event " + sEventName + " from instance " + sInstanceName);
		break;
	}
//alert("bTakeAction=" + bTakeAction + "; bEventComplete=" + bEventComplete + "; sEventType=" + sEventType + "; data=" + data + "; errno=" + errno + "; errmsg=" + errormessage);
	if (bTakeAction)
	{
		if (bEventComplete)
		{
			if (bStoreAssetInfo)
			{
				errormessage = this.StoreAssetInfo(file, assetid, data);
			}
			if (!errormessage)
			{
				SetAction(this.action);
			}
			else
			{
				bEventComplete = false;
			}
		}
	}
	
	// Can't trust the ActiveX control to provide consistent information, so check for everything.
	if (item < 0 || errno || errormessage) // -1 = error
	{
		this.action = "error";
		PromptErrorMessage(errno, errormessage, "");
	}

	return bEventComplete;
}

// more

function AssetHandler_trace(msg)
{
	if (this.traceEnabled)
	{
		document.forms[this.formName].elements.AssetHandlerLog.value += msg + "\n";
	}
}

var g_AssetHandler = new AssetHandler(0);

function OnAssetEventHandler()
{
	g_AssetHandler.OnAssetEvent();
}

function OnAssetUserRequestEventHandler()
{
	g_AssetHandler.OnAssetUserRequestEvent();
}

function AttachEktAssetEventHandlers()
{
	if (typeof EktAsset == "object")
	{
		EktAsset.oncheckout = OnAssetEventHandler;
		EktAsset.onundocheckout = OnAssetEventHandler;
		EktAsset.oncheckin = OnAssetEventHandler;
		EktAsset.onpublish = OnAssetEventHandler;
		EktAsset.onsavetoassetserver = OnAssetEventHandler;
		EktAsset.onrestore = OnAssetEventHandler;
		EktAsset.onworkoffline = OnAssetEventHandler;
		EktAsset.onsavelocalcopy = OnAssetEventHandler;
		EktAsset.onuserrequest = OnAssetUserRequestEventHandler;
	}
//	else
//	{
//		alert("Unable to create the asset control. Please check connection to the asset management server.");
//	}
}
// Allow EktAsset code to load first
setTimeout('AttachEktAssetEventHandlers()',1);

function AssetHandler_StoreAssetInfo(filename, assetid, data)
{
	var strRetErrMsg = "";
	var nErrCount = 0;
	var c_AssetInfoKeys = ["AssetFilename", "AssetID", "MimeType", "FileExtension"];
	if ("undefined" == typeof data)
	{
 		if (!filename)
 		{
			strRetErrMsg = "Missing file name.";
 		}
 		else if (!assetid)
 		{
			strRetErrMsg = "Missing ID.\nFile: " + filename;
		}
		document.forms[this.formName].asset_assetfilename.value = filename;
		document.forms[this.formName].asset_assetid.value = assetid;
		document.forms[this.formName].asset_mimetype.value = "";
		document.forms[this.formName].asset_fileextension.value = "";
	}
	else
	{
	    //debugger
		data = data + ""; // ensure it is a string
		var aryAssetInfo = ParseAssetInfoData(data);
		for (var k = 0; k < c_AssetInfoKeys.length; k++)
		{
			var key = c_AssetInfoKeys[k];
			var strValue = "";
			for (var i = 0; i < aryAssetInfo.length; i++)
			{
				var info = aryAssetInfo[i];
				// Can't trust the ActiveX control to provide consistent information, so check for everything.
				var errno = info.ErrorNumber;
				var errormessage = info.ErrorMessage;
				if (isNaN(errno))
				{
					errno = 0;
				}
				else
				{
					errno = parseInt(errno, 10);
				}
				if (errno == 0)
				{
				    var index =errormessage.indexOf("=",0);
				    if(index > 0)
				    {
				        if(document.forms[this.formName].content_id != undefined)
				            document.forms[this.formName].content_id.value = errormessage.substring(index+1,errormessage.length);
				    }
				        
				    errormessage = "";
				}
 				if (!errno && !errormessage)
				{
 					if (!info.AssetFilename)
 					{
						errormessage = "Missing file name. File number: " + (i + 1);
 					}
// 					else if (!info.AssetID)
// 					{
//						errormessage = "Missing ID.\nFile: " + info.AssetFilename;
//					}
				}
 				if (!errno && !errormessage)
				{
					if (i > 0)
					{
						strValue += ","; // delimiter
					}
					strValue += info[key].replace(/,/g, "%2C");
				}
				else if (0 == k)
				{
					nErrCount++;
					PromptErrorMessage(errno, errormessage, info.AssetFilename);
				}
			}
			document.forms[this.formName].elements["asset_" + key.toLowerCase()].value = strValue;
		}
		if ((nErrCount > 0) && (nErrCount == aryAssetInfo.length))
		{
			strRetErrMsg = "Errors were reported for every asset.";
		}
	}
	return strRetErrMsg; 
}

// CAUTION: This is NOT a generalized XML parser
function ParseAssetInfoData(data)
// returns array of AssetInfo
{
	// Ported from AssetMgtData.bas ParseAssetMgtResult()
	// XML Data Format
	//   <AssetInfo>
	//       <AssetFilename>...</AssetFilename>
	//       <AssetID>...</AssetID>
	//       <MimeType>...</MimeType>
	//       <FileExtension>...</FileExtension>
	//       <ErrorNumber>...</ErrorNumber>
	//       <ErrorMessage>...</ErrorMessage>
	//       <ContentID>...</ContentID>
	//   </AssetInfo>
	//   :
	//
    var cRet = new Array();
    var ItemTags = ["AssetFilename", "AssetID", "MimeType", "FileExtension", "ErrorNumber", "ErrorMessage","ContentID"];
    var pGroupStart = -1;
    do
    {
        pGroupStart = data.indexOf("<AssetInfo>");
        if (pGroupStart >= 0) 
        {
            data = data.substr(pGroupStart); // start at the beginning
            var pNextStart = 0;
            var cGroup = new Array();
            for (var iItem = 0; iItem < ItemTags.length; iItem++)
            {
                var strKeyName = ItemTags[iItem];
                var pItemStart = data.indexOf("<" + strKeyName + ">");
                var strItemValue = "";
                if (pItemStart >= 0)
                {
                    pItemStart += strKeyName.length + 2;
                    pItemEnd = data.indexOf("</" + strKeyName + ">");
                    if (pItemEnd >= 0)
                    {
                        strItemValue = data.substr(pItemStart, pItemEnd - pItemStart);
                        if (pItemEnd > pNextStart)
                        {
							pNextStart = pItemEnd;
                        }
                    }
                }
                cGroup[strKeyName] = strItemValue;
            }
            data = data.substr(pNextStart); // chop off what was just parsed so indexOf will continue
            cRet[cRet.length] = cGroup;
        }
    }
    while (pGroupStart >= 0);
    return cRet;
}

function PromptErrorMessage(errno, errormessage, sOtherMsg)
{
	var sErrMsg = "";
	var sDisplayMsg = "";
	if (0 == errormessage.length) 
	{
		sErrMsg = "No error message.";
		sDisplayMsg = "Error " + errno + " " + sErrMsg;
		if (errno == 0) {
			return;	//ignore error codes of zero
		}
	}
	else
	{
		sDisplayMsg = errormessage;
	}
	if (sOtherMsg.length != 0)
	{
		sDisplayMsg += "\nFile: " + sOtherMsg;
	}
	alert(sDisplayMsg);
}

function IsInWorkOffline(AssetId){
	var objInstance = EktAsset.instances[0];
	if (!((typeof objInstance != "object") || (null == objInstance))){
		return (objInstance.isInWorkOffline(AssetId));
	}else{
		return false;
	}
}

function CancelWorkOffline(AssetId){
	var objInstance = EktAsset.instances[0];
	if (!((typeof objInstance != "object") || (null == objInstance))){
		return (objInstance.cancelWorkOffline(AssetId));
	}else{
		return false;
	}
}
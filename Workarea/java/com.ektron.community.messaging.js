// Class CommunityMsgClass:

function CommunityMsgClass (idTxt) {

    this.GetId = function (){
		return (this._id);
    },
    
    this.SetId = function (id){
		this._id = id;
    },
    
    this.Initialize = function(){
        this.UserSelectCtl_Initialize();
    },

	this.MsgShowMessageTargetUI = function(hdnId, flag){
        this.InitSelectedUsers(flag);
	},
	
	this.MsgSaveMessageTargetUI = function(){
		var divObj = document.getElementById(this.GetId() + '_EktMsgTargetsBody' + this.GetId());
		var hdbObj = document.getElementById('ektouserid' + this.GetId());
		var toObj = document.getElementById('ekpmsgto' + this.GetId());
		var idx;
		var userId;
		if (toObj){toObj.value='';}
		if (divObj){
			if (hdbObj){
				hdbObj.value = '';
				
				var users = this.UserSelectCtl_GetSelectUsers();
				if (("undefined" != users) && (null != users)){
					hdbObj.value = users;
								if (toObj){
						toObj.value='';
						var userArray = users.split(',');
						for (idx = 0; idx < userArray.length; idx++){
							if(toObj && toObj.value.length > 0){
										toObj.value += ', ';
									}
							var userName = this.UserSelectCtl_GetUserName(userArray[idx]);
							if (userName)
								toObj.value += userName;
						}
					}
				}
				
			}
		}
		this.MsgCloseMessageTargetUI();
	},

	this.MsgCloseMessageTargetUI = function(){
		var divObj = document.getElementById('MessageTargetUI' + this.GetId());
		if (divObj){
			divObj.style.display = 'none';
		}
		ektb_remove();
	},

	this.MsgCancelMessageTargetUI = function(){
		//Recover Original State:
		this.searchHasRun = false;
		var obj = CommunitySearchClass.GetObject(this.usersel_comsearch_ClientID);
		if (obj){
		    obj.SelectTab("basic");
		}
		this.MsgCloseMessageTargetUI();
	},

	this.MsgTarg_PrevPage = function(){
		var pageHdnObj = document.getElementById("RecipientsPage" + this.GetId());
		if (pageHdnObj && pageHdnObj.value && (pageHdnObj.value.length > 0)){
			var page = parseInt(pageHdnObj.value);
			if (!isNaN(page)){
				page -= 1;
				pageHdnObj.value = page.toString();
				this.MsgTarg_DoSearch('','');
			}
		}
	},

	this.MsgTarg_NextPage = function(){
		var pageHdnObj = document.getElementById("RecipientsPage" + this.GetId());
		if (pageHdnObj && pageHdnObj.value && (pageHdnObj.value.length > 0)){
			var page = parseInt(pageHdnObj.value);
			if (!isNaN(page)){
				page += 1;
				pageHdnObj.value = page.toString();
				this.MsgTarg_DoSearch('','');
			}
		}
	},

    this.InitSelectedUsers = function(flag){
		var userIds = null;
		var hdbObj = document.getElementById('ektouserid' + this.GetId());
		if (hdbObj){
			this.UserSelectCtl_SetSelectUsers(hdbObj.value);
			userIds = hdbObj.value.split(',');
		}

		var toObj = document.getElementById('ekpmsgto' + this.GetId());
		var userNames = null;
		if (toObj){
			userNames = toObj.value.split(", ");
		}
		if ((userIds.length > 0) && userNames){
			for (var idx=0; idx < userIds.length; idx++){
			    this.UserSelectCtl_SetUserName(userIds[idx], userNames[idx]);
			}
		}
		
		// IE6 has a bug, where even though the checkboxes are updated to be set to a checked state, they appear unchecked (only with elements added via Ajax?!?). Workaround, refresh when browser is IE6:
		var isIE6 = (window.navigator.appVersion.indexOf("MSIE 6.")>=0);
		if (isIE6 || (flag && !this.searchHasRun)){
            // kick-off a search for all users:
            CommunitySearchClass.DoSearch(this.usersel_comsearch_ClientID, '');
            this.searchHasRun = true;
        }
    }

	this.SendMessage = function(alertNoRecipientMsg){
		var hdbObj = document.getElementById('ektouserid' + this.GetId());
		var hdbGroupObj = document.getElementById('ektogroupid' + this.GetId());
		if (hdbObj && (hdbObj.value.length > 0)){
			hdbObj = document.getElementById('EkMsg_hdnRecipientsValidated' + this.GetId());
			if (hdbObj){
				hdbObj.value = "1";
				//don't submit here, use submit button instead, as this causes problem in FireFox (the Javascript Editor posts data that causes an ASP.NET validation error): document.form1.submit();
				return (true);
			}
		}
		else if (hdbGroupObj && (hdbGroupObj.value.length > 0)){			
    		hdbObj = document.getElementById('EkMsg_hdnRecipientsValidated' + this.GetId());
			if (hdbObj){
				hdbObj.value = "1";
				//don't submit here, use submit button instead, as this causes problem in FireFox (the Javascript Editor posts data that causes an ASP.NET validation error): document.form1.submit();
				return (true);
			}
		}
		else {
			alert(alertNoRecipientMsg);
			return (false);
		}
	},

    this.SetUserSelectId = function (userSelId){
        this.userSelId = userSelId;
    },
    
    this.SetAjaxCall = function (ptr){
        this._ajaxCall = ptr;
    },
    
    this.GetAjaxCall = function (){
        return (this._ajaxCall);
    },
    

    /////////////////////////////////////////////////
    // methods for user selection - community search:

    this.UserSelectCtl_Initialize = function (){
		if ("undefined" != typeof CommunitySearchClass){
            var objId = this.GetId();
            var fPtr = function (){CommunityMsgClass.UserSelectCtl_Initialize_Complete(objId);};
            CommunitySearchClass.CallbackWhenReady(this.usersel_comsearch_ClientID, fPtr);
        }
        else{
            setTimeout('CommunityMsgClass.GetObject("' + this.GetId() + '").UserSelectCtl_Initialize()', 50);
        }
    },

    this.UserSelectCtl_Initialize_Complete = function (){
        var objId = this.GetId();
        var fPtr = function (containerObj){ CommunityMsgClass.UserSelectCtl_NotifySearchResultsChanged(objId, containerObj); };
        CommunitySearchClass.Hook_NotifySearchResultsChanged(this.usersel_comsearch_ClientID, fPtr);

        // kick-off a search for all users:
        //CommunitySearchClass.DoSearch(this.usersel_comsearch_ClientID, '');
    },

    this.UserSelectCtl_NotifySearchResultsChanged = function (containerObj){
        if (containerObj && containerObj.innerHTML && containerObj.innerHTML.length > 0){
            var userList = null;
            if (("undefined" != typeof window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID])){
                userList = window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID];
                var els = containerObj.getElementsByTagName('input');
                var chkAllObj = null;
                var allChecked = true;
                var checkedCnt = 0;
                for (var idx = 0; idx < els.length ; idx++){
                    if ("checkbox" == els[idx].type){

                        if (els[idx].name.length > 0){
                            if (("undefined" != userList[els[idx].name]) && userList[els[idx].name]){
                                els[idx].checked = true;
                                ++checkedCnt;
                            }
                            else{
                                allChecked = false;
                            }
                        }
                        else{
                            chkAllObj = els[idx];
                        }
                    }
                }
                if (chkAllObj && allChecked && (checkedCnt > 0)){
                    chkAllObj.checked = true;
                }
            }
        }
    },
    
    this.UserSelectCtl_CheckAll = function (chkboxObj, ctrlId){
        var containerObj = document.getElementById('CommunitySearch_ResultsContainer_Table_' + ctrlId);
        if (containerObj && containerObj.innerHTML && containerObj.innerHTML.length > 0){
            var els = containerObj.getElementsByTagName('input');
            for (var idx = 0; idx < els.length ; idx++){
                if (("checkbox" == els[idx].type) && (els[idx].name.length > 0)){
                    els[idx].checked = (chkboxObj.checked && !this.usersel_comsearch_SingleSelection);
                    this.UserSelectCtl_CheckboxClicked(els[idx], ctrlId, els[idx].name, els[idx].alt)
                }
            }
        }
    },
    
    this.UserSelectCtl_CheckboxClicked = function (chkboxObj, ctrlId, userId, displayName){
        var userList = null;
        var userNameList = null;
        
        if (chkboxObj.checked && this.usersel_comsearch_SingleSelection){
            this.UserSelectCtl_CheckAll(chkboxObj, ctrlId);
            chkboxObj.checked = true;
        }

        if (("undefined" != typeof window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID])){
            userList = window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID];
        }
        else{
            userList = new Array;
            window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID] = userList;
        }
        userList[userId] = chkboxObj.checked;

        // save usernames for each id:
        if (("undefined" != typeof window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID])){
            userNameList = window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID];
        }
        else{
            userNameList = new Array;
            window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID] = userNameList;
        }
        userNameList[userId] = displayName;
    },
    
    this.UserSelectCtl_GetSelectUsers = function (){
        var userList = null;
        var result = '';
        if (("undefined" != typeof window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID])){
            userList = window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID];
        }
        else{
            userList = new Array;
            window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID] = userList;
        }
        
        for (var userItem in userList){
            if (userList[userItem]){
                if (result.length)
                    result += ",";
                result += userItem.toString();
            }
        }
         //In safari the for is going through an extra loop so removing the ',' if appended to the end. 
       if(result.lastIndexOf(",")==result.length-1)
        {
            result = result.substring(0,result.length-1);
        }
        return (result);
    },

    this.UserSelectCtl_GetUserName = function (userId){
        var userNameList = null;
        var result = '';
        if (("undefined" != typeof window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID])){
            userNameList = window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID];
        }
        else{
            userNameList = new Array;
            window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID] = userNameList;
        }
        if (("undefined" != typeof userNameList[userId]) && (null != userNameList[userId]))
            result = userNameList[userId];
        return (result);
    },

    this.UserSelectCtl_SetSelectUsers = function (idList){
        var idArray = idList.split(',');
        var userList = new Array;
        window["UserSelectCtl_UserList" + this.usersel_comsearch_ClientID] = userList;
        
        for (var idx = 0; idx < idArray.length; idx++){
            userList[idArray[idx]] = true;
        }
    },

    this.UserSelectCtl_SetUserName = function (userId, displayName){
        var userNameList = null;
        var result = '';
        if (("undefined" != typeof window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID])){
            userNameList = window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID];
        }
        else{
            userNameList = new Array;
            window["UserSelectCtl_UserNameList" + this.usersel_comsearch_ClientID] = userNameList;
        }
        if ("undefined" != typeof userNameList){
            userNameList[userId] = displayName;
        }
    },

	
	///////////////////////////
	// initialize properties:
	this._id = '';
	this.SetId(idTxt);
	this.MsgUsersSelArray = new Object();
	this.SearchInit = false;
	this.userSelId = idTxt + '_ComSearch';

    this.usersel_comsearch_ClientID = this.GetId() + '_ComSearch';
    this.usersel_comsearch_SingleSelection = false;
    
    this.searchHasRun = false;
}

CommunityMsgClass.GetObject = function (id){
	var fullId = "CommunityMsgObj_" + id;
	if (("undefined" != typeof window[fullId])
		&& (null != window[fullId])){
		return (window[fullId]);
	}
	else {
	    var cmObj = new CommunityMsgClass(id);
	    window[fullId] = cmObj;
	    window[fullId + '_ComSearch'] = cmObj; // save a Sreference for the search control.
	    cmObj.Initialize();
	    return (cmObj);
	}
}

CommunityMsgClass.addLoadEvent = function(func) {
    var oldonload = window.onload;
    if (typeof window.onload != 'function') {
        window.onload = func;
    } else {
        window.onload = function() {
            if (oldonload) {
                oldonload();
            }
            func();
        }
    }
}

CommunityMsgClass.UserSelectCtl_Initialize_Complete = function (id){
    var obj = CommunityMsgClass.GetObject(id);
    if (obj)
        obj.UserSelectCtl_Initialize_Complete();
}

CommunityMsgClass.UserSelectCtl_NotifySearchResultsChanged = function (id, containerObj){
    var obj = CommunityMsgClass.GetObject(id);
    if (obj)
        obj.UserSelectCtl_NotifySearchResultsChanged(containerObj);
}

function StripHtml(stext){
        var re = /<\S[^>]*>/g;
        stext = stext.replace(re,"");
        return (stext); 
}

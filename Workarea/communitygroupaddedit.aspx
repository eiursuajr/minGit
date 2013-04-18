<%@ Page Language="C#" AutoEventWireup="true" CodeFile="communitygroupaddedit.aspx.cs" Inherits="communitygroupaddedit" ValidateRequest="false" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register TagPrefix="Community" TagName="UserSelectControl" Src="controls/Community/Components/UserSelectControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" runat="server">
<head runat="server">
    <title>Add Edit Community Group</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <asp:literal id="ltr_css" runat="server" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        groupAdmin = function(title, id)
        {
            this.title = title;
            this.id = id;
        }
        
        // Multiple Admin Users UI Starts
        // This function generates the UI for selected admin Users.
        GenerateAdminUserUI= function(title, id, groupId) {
            var span_elem;
            var att;
            var NodesCollection;
            var newEntry;
            var groupAdmin = new Array();
            
            var firfox = document.createElement("div");
            
            // Generates UI when multiple admins are selected to add the Group.
            if(groupId == "0")
            {
                NodesCollection = document.getElementById("addAdminUsers"); 
            }
            // Generates UI when multiple admins are selected to edit the Group.
            else
            {
                NodesCollection = document.getElementById("editAdminUsers");  
            }
            newEntry = document.createElement("div");
            newEntry.id = "user_" + title.toLowerCase();
            newEntry.className = "AdminUser";

            span_elem = document.createElement("span");
            span_elem.id = "user_links_" + title.toLowerCase();
            span_elem.innerHTML = "<img onclick=\"deleteUserFromList('" + title + "','" + id + "');\" alt=\"Delete path\" src=\"images/UI/Icons/delete.png\" />&nbsp;&nbsp;&nbsp;&nbsp;";
            newEntry.appendChild(span_elem);

            span_elem = document.createElement("span");
            span_elem.id = "user_title_" + title.toLowerCase();
            span_elem.innerHTML = title;
            newEntry.appendChild(span_elem);

            firfox.appendChild(newEntry);
            NodesCollection.appendChild(firfox);
        }

        renderGroupAdmins = function (groupId) {
            for (var i = 0; i < arGroupAdmins.length; i++)
            {
                GenerateAdminUserUI(arGroupAdmins[i].title, arGroupAdmins[i].id, groupId);
                $ektron("input#hdnAdminUsers")[0].value = $ektron("input#hdnAdminUsers")[0].value + arGroupAdmins[i].id + ",";
            }
        }
        // Adds selected Admin users to the list of users under the browser button.
        AddAdminUsers = function () {
            var checkedAdminUsers = $ektron("table#CommunitySearch_ResultsContainer_Table_Invite_UsrSel_usersel_comsearch input:checkbox");
            var counterCheckedUsers = 0;
            var userExists = false;
            if (checkedAdminUsers.length > 0) {
                $ektron.each(checkedAdminUsers, function () {
                    if (this.checked) {
                        if ($ektron(this).attr('alt')) {
                            if (AdminUserNotExists(this)) {
                                $ektron("input#hdnAdminUsers")[0].value = $ektron("input#hdnAdminUsers")[0].value + "," + $ektron(this).attr("name");
                                GenerateAdminUserUI($ektron(this).attr('alt'), $ektron(this).attr('name'), 0);
                                counterCheckedUsers = counterCheckedUsers + 1;
                            }
                            else {
                                userExists = true;
                            }
                        }
                    }
                });
                if (counterCheckedUsers > 0) {
                    $ektron("#MessageTargetUI__Page").modalHide();
                }
                else {
                    if (userExists == true)
                        alert("You selected a user(s) who is already a group admin.")
                    else
                        alert("Please select at least one user.")
                }
            }
        }

        // Checks whether the user already exists in the list.
        AdminUserNotExists = function(obj) {
            var thisUserName = $ektron(obj).attr('alt');
            if ($ektron("span#user_title_" + thisUserName.toLowerCase()).length == 0) {
                return true;
            }
        }
        deleteUserFromList = function (title, id) {
            var obj = document.getElementById("user_title_" + title.toLowerCase()).parentNode;

            obj.parentNode.removeChild(obj);
            var array = new Array();
            var arrayNewAdmins = new Array();
            var userCounter = 0;
            array = $ektron("#hdnAdminUsers")[0].value.split(",");
            for (userCounter = 0; userCounter < array.length; userCounter++) {
                if (array[userCounter] != id) {
                    //$ektron("#hdnAdminUsers")[0].value = $ektron("#hdnAdminUsers")[0].value.replace(id + ",", "");
                    //$ektron("#hdnAdminUsers")[0].value = $ektron("#hdnAdminUsers")[0].value.replace("," + id, "");
                    arrayNewAdmins.push(array[userCounter]);
                }
            }
            $ektron("#hdnAdminUsers")[0].value = arrayNewAdmins.join(",");
        }
        // Multiple Admin Users UI ends.
        function GetCommunityMsgObject(id){
	        var fullId = "CommunityMsgObj_" + id;
	        if (("undefined" != typeof window[fullId])
		        && (null != window[fullId])){
		        return (window[fullId]);
	        }
	        else {
	            var obj = new CommunityMsgClass(id);
	            var fullId = "CommunityMsgObj_" + id;
	            window[fullId] = obj;
		        return (obj);
	        }
        }

        function CommunityMsgClass (idTxt) {
            this.getId = function () {
                return (this.id);
            },

	        this.SetUserSelectId = function (userSelId) {
	            this.userSelId = userSelId;
	        },

	        this.MsgShowMessageTargetUI = function (hdnId) {
	            $ektron('#MessageTargetUI' + this.getId()).modalShow();
	        },

	        this.MsgSaveMessageTargetUI = function () {
	            var hdbObj = document.getElementById('ektouserid' + this.getId());
	            var toObj = document.getElementById('ekpmsgto' + this.getId());
	            var idx;
	            if (hdbObj) {
	                hdbObj.value = '';
	                var users = UserSelectCtl_GetSelectUsers(this.userSelId);
	                if (("undefined" != users) && (null != users)) {
	                    hdbObj.value = users;
	                    if (toObj) {
	                        toObj.value = '';
	                        var userArray = users.split(',');
	                        for (idx = 0; idx < userArray.length; idx++) {
	                            if (toObj && toObj.value.length > 0) {
	                                toObj.value += ', ';
	                            }
	                            var userName = UserSelectCtl_GetUserName(this.userSelId, userArray[idx]);
	                            if (userName)
	                                toObj.value += userName;
	                        }
	                    }
	                }
	            }
	            this.MsgCloseMessageTargetUI();
	        },


	        this.MsgCloseMessageTargetUI = function () {
	            $ektron('#MessageTargetUI' + this.getId()).modalHide();
	        },

	        this.MsgCancelMessageTargetUI = function () {
	            this.MsgCloseMessageTargetUI();
	        },

            ///////////////////////////
            // initialize properties:
	        this.id = idTxt;
	        this.name = '';
	        this.MsgUsersSelArray = new Object();
	        this.userSelId = '';
        }
	    //--><!]]>
    </script>
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        var inPublishProcess = false;

        function ShowAddGroupTagArea(){
            $ektron("#newTagName").val("");
            $ektron("#newTagNameDiv").modalShow();
	    }

	    var customPTagCnt = 0;
	    function SaveNewGroupTag(){
		    var objTagName = document.getElementById("newTagName");
		    var objTagLanguage = document.getElementById("TagLanguage");
		    var objLanguageFlag = document.getElementById("flag_" + objTagLanguage.value);
		    var divObj = document.getElementById("newTagNameScrollingDiv");

    		if(!CheckForillegalChar(objTagName.value)){
		        return;
		    }

		    if (objTagName && (objTagName.value.length > 0) && divObj){
			    ++customPTagCnt;
			    divObj.innerHTML += "<input type='checkbox' checked='checked' onclick='ToggleCustomPTagsCbx(this, \"" + objTagName.value + "\");' id='userCustomPTagsCbx_" + customPTagCnt + "' name='userCustomPTagsCbx_" + customPTagCnt + "' />&#160;"

			    if(objLanguageFlag != null){
			        divObj.innerHTML += "<img src='" + objLanguageFlag.value + "' border=\"0\" />"
			    }

			    divObj.innerHTML +="&#160;" + objTagName.value + "<br />"

			    AddHdnTagNames(objTagName.value + '~' + objTagLanguage.value);
			    
                $ektron('#newTagNameScrollingDiv input[data-ektron-checkbox-flag="false"]').removeAttr('checked');
                $ektron('#newTagNameScrollingDiv input[data-ektron-checkbox-flag="true"]').attr('checked', 'checked');
		    }

		    // now close window:
		    CancelSaveNewGroupTag();
	    }

	    function CheckForillegalChar(tag) {
            if (Trim(tag) == '')
            {
               alert('<asp:Literal ID="error_TagsCantBeBlank" Text="Please enter a name for the Tag." runat="server"/>');
               return false;
            } else {

                //alphanumeric plus _ -
                var tagRegEx = /[!"#$%&'()*+,./:;<=>?@[\\\]^`{|}~ ]+/;
                if(tagRegEx.test(tag)==true) {
                    alert('<asp:Literal ID="error_InvalidChars" Text="Tag Text can only include alphanumeric characters." runat="server"/>');
                    return false;
                }

            }
            return true;
        }

	    function CancelSaveNewGroupTag(){
            $ektron("#newTagNameDiv").modalHide();
	    }

        function CancelBroswseCommunityUserModal(){
            $ektron("#MessageTargetUI__Page").modalHide();
        }
        
	    function AddHdnTagNames(newTagName){
		    objHdn = document.getElementById("newTagNameHdn");
		    if (objHdn){
			    var vals = objHdn.value.split(";");
			    var matchFound = false;
			    for (var idx = 0; idx < vals.length; idx++){
				    if (vals[idx] == newTagName){
					    matchFound = true;
					    break;
				    }
			    }
			    if (!matchFound){
				    if (objHdn.value.length > 0){
					    objHdn.value += ";";
				    }
				    objHdn.value += newTagName;
			    }
		    }
	    }

	    function RemoveHdnTagNames(oldTagName){
		    objHdn = document.getElementById("newTagNameHdn");
		    if (objHdn && (objHdn.value.length > 0)){
			    var vals = objHdn.value.split(";");
			    objHdn.value = "";
			    for (var idx = 0; idx < vals.length; idx++){
				    if (vals[idx] != oldTagName){
					    if (objHdn.value.length > 0){
						    objHdn.value += ";";
					    }
					    objHdn.value += vals[idx];
				    }
			    }
		    }
	    }

	    function ToggleCustomPTagsCbx(btnObj, tagName){
		    if (btnObj.checked){
			    AddHdnTagNames(tagName);
			    btnObj.checked = true;
			    $ektron(btnObj).attr("data-ektron-checkbox-flag","true");
		    }
		    else{
			    RemoveHdnTagNames(tagName);
			    btnObj.checked = false; // otherwise re-checks when adding new custom tag.
			    $ektron(btnObj).attr("data-ektron-checkbox-flag","false");
		    }
	    }

	    function toggleVisibility(itm)
        {
            switch(itm)
            {
                case "upload":
                    $ektron("#avatar_upload_panel").modalShow();
                    break;
                case "close":
                    $ektron("#avatar_upload_panel").modalHide();
                    break;
          }
        }

        function updateavatar()
        {
            var tfile = document.getElementById('GroupAvatar_TB');
            var ofile = document.getElementById('fileupload1');
            if (tfile.value.indexOf('[file]') > -1)
            {
                ofile.outerHTML  = document.getElementById('fileupload1').outerHTML;
                tfile.value = tfile.value.replace('[file]', '');
            }
        }
        //--><!]]>
    </script>
    <script type="text/javascript">
    
        if (typeof(Ektron.Workarea.GroupEdit) == "undefined")
        {
            Ektron.Workarea.GroupEdit =
            {
                init: function() {

                    $ektron(".TestEmailSettingsButton").button();
                    $ektron(".chkEnableEmail").bind("click", function() {
                        $("table.EktronGroupEmailSetting").toggle(this.checked);
                    });

                    $ektron(".TestEmailSettingsButton").bind("click", function() {
                        $("div#ektronTestEmailResults").html("Verifying account...");
                        $("div#ektronTestEmailResults").load("notifications/TestEmailConnection.ashx", {
                            'account': $ektron('input.txtEmailAccount').val(),
                            'password': $ektron('input.txtEmailPassword').val(),
                            'server': $ektron('input.txtEmailReplyServer').val(),
                            'serverPort': $ektron('input.txtEmailReplyServerPort').val(),
                            'useSsl': $ektron('input#chkUseSsl')[0].checked
                        });
                        return false;
                    });
                }
            }
        }

        Ektron.ready(function () {
            if ($ektron('input.txtEmailAccount').length > 0) {
                $ektron('input.txtEmailAccount').attr("autocomplete", "off");
            }
            if ($ektron('input.txtEmailPassword').length > 0)
                $ektron('input.txtEmailPassword').attr("autocomplete", "off");

            Ektron.Workarea.GroupEdit.init();
            var tabsContainers = $ektron(".tabContainer");
            tabsContainers.tabs();

            //Tag, Upload, Browse Modal
            $ektron("#newTagNameDiv, #avatar_upload_panel").modal({
                trigger: '',
                modal: true,
                toTop: true,
                onShow: function (hash) {
                    hash.w.css("margin-top", 0); //hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                    hash.o.fadeIn();
                    hash.w.fadeIn();
                    setTimeout(CommunityGroupAddEdit__SetScrollable, 500);
                    $(window).resize(CommunityGroupAddEdit__SetScrollable);
                },
                onHide: function (hash) {
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function () {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    });
                }
            });
            $ektron("#MessageTargetUI__Page").modal({
                trigger: '',
                modal: true,
                toTop: false,
                onShow: function (hash) {
                    hash.w.css("margin-top", 0); //hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                    hash.o.fadeIn();
                    hash.w.fadeIn();
                    setTimeout(CommunityGroupAddEdit__SetScrollable, 500);
                    $(window).resize(CommunityGroupAddEdit__SetScrollable);
                },
                onHide: function (hash) {
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function () {
                        if (hash.o) {
                            hash.o.remove();
                        }
                    });
                }
            });

            // reveal the content of the page
            $ektron("#form1").css({
                "position": "static",
                "left": "auto"
            });
        });

    function CommunityGroupAddEdit__SetScrollable(){
        var container = $ektron(".EktMsgTargets").eq(0);

        container.height($ektron(window).height() -
            (container.offset().top
                + (container.innerHeight()
                    - container.height())));

        $ektron(".analyticsReport .ektronToolbar").eq(0).width(container.outerWidth());
        container.eq(0).css("overflow-y", "auto");
        container.eq(0).css("overflow-x", "auto");
        container.find("* *").eq(0).css("overflow", "visible");
    }
    
   
    </script>
     <link   type='text/css' rel='stylesheet' href='Tree/css/com.ektron.ui.tree.css' />
    <style type="text/css">
			#T0{ float:none; position:relative; }
			ul.ektree{ float:none; position:relative; background-color: #ffffff; border: 1px solid #000000; margin: 10px 10px 10px 10px; padding: 10px 10px 10px 10px; }
            #TreeOutput{ position:relative; width:100%; }
			#d_dvCategory table{ width:100%; }
			div#newTagNameDiv { height: 175px !important; width:375px !important; margin: 17em 0 0 -15em !important; border: solid 1px #aaaaaa; z-index: 10; background-color: white; left: 50%; position: fixed; margin-left: -20em;}
			div#avatar_upload_panel { height: 125px; width:400px;top:7%;left:35%; margin: 10em 0 0 -15em; border: solid 1px #aaaaaa; z-index: 10; background-color: white; }
			div#MessageTargetUI__Page { margin: 10em 0 0 1.0em;top:3px;left:2px; border: solid 1px #aaaaaa; z-index: 10; background-color: white; }
		    /* Styling for the Browse Members/Friends elements */
		    .EktMsgTargetCtl{ font-family: Verdana,Geneva,Arial,Helvetica,sans-serif; font-size: 12px; position: relative; top: 2px; left: 0px; background-color: white; z-index:12; }
		    .EktMsgTargets{ position: relative; top: 20px; left: 16px; border: solid 1px #dddddd; padding: 10px; }
		    .EktMsgTargets div.CommunitySearch_ResultsContainer { width: 440px; }
		    #newTagName { width: 275px !important; }
		    #newTagNameScrollingDiv { height: 80px; overflow: auto; border: z-index: 1; }
            a.btnUpload { padding-top: .2em !important; padding-bottom: .2em !important;line-height: 16pt !important; display:inline-block; text-decoration: none !important; }
            .nameWidth { color:#1D5987; font-weight: bold; text-align: right; white-space: nowrap; width:10%; }
            a.buttonBrowseUSer {background-position: .6em center;}
            #FrameContainer { position: relative; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 1000; }
            body div.ektronWindow {position: relative !important; top: 0; left: 0; margin-left: 0; margin-top: 0;}
            div#MessageTargetUI__Page.dv_MessageTargetUI {width: auto; margin-top: -42px !important; margin-left: -21px !important;}
            .dv_MessageTargetUI .ektronModalBody {padding: 2px;}
            .CommunitySearch_BasicTextboxContainer input {width: 200px;}
            
            form#form1 {position: absolute; left: -10000px}
            
            /* Email Enabled */
            .ektronForm table.EktronGroupEmailSetting{border:1px solid silver;border-collapse:collapse;margin-top:10px;}
            .ektronForm table.EktronGroupEmailSetting th{border:1px solid silver;font-weight:bold;text-align:left;}
            .ektronForm table.EktronGroupEmailSetting th span.note{font-weight:normal;}
            .ektronForm table.EktronGroupEmailSetting td{border:1px solid silver;text-align:left;}
            .ektronForm table.EktronGroupEmailSetting td div#ektronTestEmailResults{float:left;margin:5px 0 0 10px;}
            .ektronForm table.EktronGroupEmailSetting input.TestEmailSettingsButton{width:120px;text-align:center;margin-top:5px;float:left;}
            .ektronForm table.EktronGroupEmailSetting input.TestEmailSettingsButton:hover{cursor:pointer;}
    </style>
    <script type="text/javascript">
        $ektron(document).ready(function () {
            $ektron(".ektronPageHeader").css({
                paddingTop: '0px'
            });
            $ektron("body .ektronPageContainer").attr("style", "top: 68px !important");
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" name="content_id" value="0" id="content_id" runat="server" />
        <input type="hidden" runat="server" id="submitasstagingview" name="submitasstagingview" value="" />
        <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
        <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
        <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value=""
            runat="server" />
        <asp:Literal ID="ltr_js" runat="Server" />
        <div id="MessageTargetUI__Page" class="dv_MessageTargetUI ektronWindow ektronModalStandard">
            <div class="ektronModalBody">
                <div class="EkTB_dialog">
                    <div class="EktMsgTargetCtl">
                        <div>
                            <div id="browseCommunityUsers" class="EktMsgTargets">
                                <!-- <asp_Literal ID="ltr_recipientselect" run_at="Server" /> -->
                                <Community:UserSelectControl id="Invite_UsrSel" FriendsOnly="false" runat="server" />
                                <asp:button ToolTip="Done" id="cgae_userselect_done_btn" runat="server" />
                                <asp:button ToolTip="Cancel" id="cgae_userselect_cancel_btn" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="ektronPageContainer ektronPageTabbed">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li>
                            <a title="Properties" href="#dvProperties">
                              <asp:Label ID="lblProperties" runat="server" />
                            </a>
                        </li>
                        <li>
                            <a title="Tags" href="#dvTags">
                                <asp:Label ID="lblTags" runat="server" />
                            </a>
                        </li>
                        <asp:PlaceHolder ID="phCategoryTab" runat="server">
                            <li>
                                <a title="Category" href="#dvCategory">
                                <asp:Label ID="lblCategory" runat="server" />
                                </a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phAliasTab" runat="server">
                        <li>
                         <a title="Links" href="#dvAlias">
                            Links
                         </a>
                        </li>
                        </asp:PlaceHolder>
                   </ul>

                <div id="dvProperties">
                    <span id="errmsg" runat="server" />
                    <table class="ektronForm">
                        <tr>
                            <td class="label" title="Group Name"><asp:Literal ID="ltr_groupname" runat="server" />:</td>
                            <td class="value"><asp:TextBox ToolTip="Enter Group Name here" ID="GroupName_TB" CssClass="ektronTextMedium" runat="server" /></td>
                        </tr>
                        <tr runat="server" id="tr_ID">
                            <td class="label" title="ID"><asp:Literal ID="ltr_groupid" runat="server" />:</td>
                            <td class="value"><asp:Label ToolTip="ID" ID="lbl_id" runat="server" /></td>
                        </tr>
                        <tr id="tr_admin" runat="server">
                            <td class="label" title="Administrator"><asp:Literal ID="ltr_admin" runat="server" />:</td>
                            <td class="value">
                                <%--<input type="text" title="Enter Administrator Name" id="ekpmsgto__Page" name="ekpmsgto__Page" disabled="disabled" runat="server" class="ektronTextMedium" />--%>
                                <input type="hidden" id="ektouserid__Page" name="ektouserid__Page" value="" runat="server" />
                                <asp:Literal ID="BrowseUsers" runat="server"  />
                                <div id="addAdminUsers">
		                        </div>
		                        <div id="editAdminUsers">
		                        </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Membership"><asp:Literal ID="ltr_groupjoin" runat="server" />:</td>
                            <td class="value">
                                <asp:RadioButton ToolTip="Open" ID="PublicJoinYes_RB" runat="server" GroupName="PublicJoin" Text="Yes" />&nbsp;&nbsp;
                                <asp:RadioButton ToolTip="Restricted" ID="PublicJoinNo_RB" runat="server" GroupName="PublicJoin" Text="No" />&nbsp;&nbsp;
                                <asp:RadioButton ToolTip="Hidden" ID="PublicJoinHidden_RB" runat="server" GroupName="PublicJoin" Text="No" />

                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Features" style="white-space: nowrap;">
                                <asp:Literal ID="ltr_groupfeatures" runat="server"></asp:Literal>
                            </td>
                            <td>
                                <asp:CheckBox ToolTip="Create Group Calender" ID="FeaturesCalendar_CB" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:CheckBox ToolTip="Create Group Forum" ID="FeaturesForum_CB" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:CheckBox ToolTip="Create Group Todo List" ID="FeaturesTodo_CB" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Image"><asp:Literal ID="ltr_groupavatar" runat="server" />:</td>
                            <td class="value">
                                <asp:Literal ID="ltr_avatarpath" runat="server" /><asp:TextBox ToolTip="Enter Image Text here" ID="GroupAvatar_TB" CssClass="ektronTextMedium" runat="server" />
                                <a class="button buttonInlineBlock greenHover buttonUpload btnUpload" href="javascript:toggleVisibility('upload');" name="upload">
                                    <asp:Literal ID="ltr_upload" runat="server" />
                                </a>
                                <div id="avatar_upload_panel" class="ektronWindow ektronModalStandard">
                                    <div class="ektronModalHeader">
                                        <h3>
                                            <span class="headerText" title="Upload"><asp:Literal ID="lblUpload" runat="server" /></span>
                                            <asp:HyperLink ToolTip="Close" ID="closeDialogLink3" CssClass="ektronModalClose" runat="server" />
                                        </h3>
                                    </div>
                                    <div class="ektronModalBody">
                                        <asp:Label ToolTip="Status" ID="lbStatus" runat="server" />
                                        <div style="float:right !important">
                                            <input type="file" title="Enter a Fle to Upload" id="fileupload1" runat="server" />
                                        </div>
                                        <br />
                                        <div class="ektronSpace">
                                            <div class="ektronSpace"/>
                                            <ul class="buttonWrapper ui-helper-clearfix">
                                                <li><a title="Close" href="#" class="button redHover buttonClear" onclick="toggleVisibility('close'); return false;">
                                                    <asp:Literal ID="ltr_close" runat="server" />
                                                </a></li>
                                                <li><a href="#" class="button greenHover buttonOk" onclick="CheckUpload(document.getElementById('fileupload1').value); return false;">
                                                    <asp:Literal ID="ltr_ok" runat="server" />
                                                </a></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Group Location"><asp:Literal ID="ltr_grouplocation" runat="server" />:</td>
                            <td class="value"><asp:TextBox ToolTip="Enter Group Location here" ID="Location_TB" CssClass="ektronTextMedium" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Group Short Description"><asp:Literal ID="ltr_groupsdesc" runat="server" />:</td>
                            <td class="value"><asp:TextBox ToolTip="Enter Group Short Description here" ID="ShortDescription_TB"  CssClass="ektronTextMedium" runat="server" MaxLength="100" /></td>
                        </tr>
                        <tr>
                            <td valign="top"class="label" title="Group Description"><asp:Literal ID="ltr_groupdesc" runat="server" />:</td>
                            <td class="value"><asp:TextBox ToolTip="Enter Group Location here" ID="Description_TB" runat="server" Rows="6" TextMode="MultiLine" MaxLength="500" /></td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <div runat="server" id="tr_EnableDistribute">
                                    <span class="label" title="Enable Distribution to Site"><asp:CheckBox ToolTip="Option to Enable Distribution to Site" ID="EnableDistributeToSite_CB" runat="server" /><asp:Literal ID="ltr_enabledistribute" runat="server" /></span>
                                </div>

                                <div runat="server" id="tr_AllowMembersToManageFolders">
                                    <span class="label" colspan="2" title="Allow member to manage folders"><asp:CheckBox ToolTip="Option to Allow member to manage folders" ID="AllowMembersToManageFolders_CB" runat="server" /><asp:Literal ID="ltr_AllowMembersToManageFolders" runat="server" /></span>
                                </div>

                                <div runat="server" id="tr_MessageBoardModeration">
                                    <span class="label" colspan="2" title="Message Board Moderation"><asp:CheckBox ToolTip="Option for Message Board Moderation" ID="chkMsgBoardModeration" runat="server" /><asp:Literal ID="ltr_MsgBoardModeration" runat="server" /></span>
                                </div>
                                <div runat="server" id="tr_EnableDocumentNotifications">
                                    <span class="label" colspan="2" title="Attach Documents in Email Notifications"><asp:CheckBox ToolTip="Option to Attach Documents in Email Notifications" ID="chkEnableDocumentNotifications" runat="server" /><asp:Literal ID="ltrlEnableDocumentNotifications" runat="server" text="Attach Documents in Email Notifications" /></span>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td><asp:Literal ID="ltr_Emaildesc" runat="server" Text="Group Email" />:</td>
                            <td>
                                <span class="label" colspan="2" title="Enable Group Emails"><asp:CheckBox ToolTip="Option to Enable Group Emails" ID="chkEnableEmail" CssClass="chkEnableEmail" runat="server" /><asp:Literal ID="Literal1" Text="Enable Group Emails" runat="server" /></span>
                                <table id="ucEktronGroupEmailSetting" runat="server" class="EktronGroupEmailSetting">
                                    <thead>
                                        <tr>
                                            <th colspan="2" style="border:1px solid silver;font-weight:bold;">
                                                <asp:Label ToolTip="Email Details" ID="lblEmailReplyHeader" runat="server" Text="Email Details (Note: e-mail server settings are set in Community Management Notifications settings screen)" />
                                                <span class="note"><asp:Label ID="lblDetailsNote" runat="server" Text="" /></span>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr class="EktronGroupEmailSetting">
                                            <th>
                                                <asp:Label ToolTip="Email Address" ID="lblEmailAddress" runat="server" Text="Email Address" />
                                            </th>
                                            <td>
                                                <asp:TextBox ToolTip="Enter Email Address here" ID="txtEmailAddressValue" class="txtEmailAddressValue" runat="server" />
                                            </td>
                                        </tr>
                                        <tr class="EktronGroupEmailSetting">
                                            <th>
                                                <asp:Label ToolTip="Email Account Name" ID="lblEmailAccount" runat="server" Text="Email Account Name" />
                                                 <span class="note"><asp:Label ToolTip="this is the email account name used to retrieve the email" ID="Label1" runat="server" Text=" - this is the email account name used to retrieve the email." /></span>
                                            </th>
                                            <td>
                                                <asp:TextBox ToolTip="Enter Email Account Name here" ID="txtEmailAccount" class="txtEmailAccount" runat="server" />
                                            </td>
                                        </tr>
                                        <tr class="EktronGroupEmailSetting">
                                            <th>
                                                <asp:Label ToolTip="Email Account Password" ID="lblEmailPassword" runat="server" Text="Email Account Password" />
                                            </th>
                                            <td>
                                                <asp:TextBox ToolTip="Enter Email Account Password here" ID="txtEmailPassword" class="txtEmailPassword" runat="server" TextMode="Password" />
                                            </td>
                                        </tr>
                                         <tr class="EktronGroupEmailSetting">
                                            <th>
                                                <asp:Label ID="lblEmailServer" runat="server" Text="Email Server" />
                                            </th>
                                            <td>
                                                <asp:Label ID="lblEmailServerValue" runat="server" Text="" />
                                            </td>
                                        </tr>
                                        <tr class="EktronGroupEmailSetting">
                                            <th>
                                                <asp:Label ID="lblEmailServerPort" runat="server" Text="Email Server Port" />
                                            </th>
                                            <td>
                                                <asp:Label ID="lblEmailServerPortValue" runat="server" Text="" />
                                            </td>
                                        </tr>
                                        <tr class="EktronGroupEmailSetting">
                                            <th>
                                                <asp:Label ID="lblUseSsl" runat="server" Text="Use SSL" />
                                            </th>
                                            <td>
                                                <asp:CheckBox ID="CheckBox1" class="chkUseSsl" runat="server" Enabled="false"/>
                                            </td>
                                        </tr>
                                        <tr class="EktronGroupEmailSetting" id="ucTestRow" runat="server">
                                            <td colspan="2"> 
  
                                                <input ID="btnTestEmailSettings" title="Test Email Settings" class="TestEmailSettingsButton" value="Test Connection" runat="server" />
                                                <div id="ektronTestEmailResults"></div>
                                                <input type="hidden" id="txtEmailReplyServer" class="txtEmailReplyServer" runat="server" />
                                                <input type="hidden" id="txtEmailReplyServerPort" class="txtEmailReplyServerPort" runat="server" />
                                                <asp:CheckBox ID="chkUseSsl" class="chkUseSsl" runat="server" Enabled="false" Style="display:none;" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="dvTags">
                    <div id="TD_GroupTags" runat="server"></div>
                </div>
                <asp:PlaceHolder ID="phCategoryFrame" runat="server">
                    <div id="dvCategory">
                        <asp:Literal runat="server" ID="EditTaxonomyHtml" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phAliasFrame" runat="server">
                        <div id="dvAlias">
                            <p style="width: auto; height: auto; overflow: auto;" class="groupAliasList" ><%=groupAliasList%></p>
                        </div>
                    </asp:PlaceHolder> 
                    <div id="dvWaitImage">
                    </div>
                </div>
            </div>
        </div>
        <asp:Literal ID="UpdateFieldJS" runat="server" />
        <input type="hidden" name="hdnAdminUsers" id="hdnAdminUsers" value="" runat="server"/>
        <%--<div id="FrameContainer_">
            <iframe id="ChildPage" src="javascript:false;" frameborder="1" marginheight="0" marginwidth="0" width="100%"
                height="100%" scrolling="auto" style="background-color: white;">
            </iframe>
        </div>--%>

    </form>

    <script type="text/javascript">
        <asp:literal id="js_taxon" runat="server" />
    // var taxonomytreearr="".split(",");
    // var taxonomytreedisablearr="".split(",");
    function fetchtaxonomyid(pid){
        for(var i=0;i<taxonomytreearr.length;i++){
            if(taxonomytreearr[i]==pid){
                return true;
                break;
            }
        }
        return false;
    }
     function fetchdisabletaxonomyid(pid){
        for(var i=0;i<taxonomytreedisablearr.length;i++){
            if(taxonomytreedisablearr[i]==pid){
                return true;
                break;
            }
        }
        return false;
    }
    function updatetreearr(pid,op){
        if(op=="remove"){
            for(var i=0;i<taxonomytreearr.length;i++){
                if(taxonomytreearr[i]==pid){
                    taxonomytreearr.splice(i,1);break;
                }
            }
        }
        else{
            taxonomytreearr.splice(0,0,pid);
        }

        document.getElementById("taxonomyselectedtree").value="";
        for(var i=0;i<taxonomytreearr.length;i++){
            if(document.getElementById("taxonomyselectedtree").value==""){
                document.getElementById("taxonomyselectedtree").value=taxonomytreearr[i];
            }else{
                document.getElementById("taxonomyselectedtree").value=document.getElementById("taxonomyselectedtree").value+","+taxonomytreearr[i];
            }
        }
    }
    function selecttaxonomy(control){
        var pid=control.value;
        if(control.checked)
        {
            updatetreearr(pid,"add");
        }
        else
        {
            updatetreearr(pid,"remove");
        }
        var currval=eval(document.getElementById("chkTree_T"+pid).value);
        var node = document.getElementById( "T" + pid );
        var newvalue=!currval;
        document.getElementById("chkTree_T"+pid).value=eval(newvalue);
        if(control.checked)
          {
            Traverse(node,true);
          }
        else
          {
            Traverse(node,false);
            var hasSibling = false;
            if (taxonomytreearr != "")
              { for(var i = 0 ;i<taxonomytreearr.length;i++)
                    {
                      if(taxonomytreearr[i] != "")
                        {
                          var newnode = document.getElementById( "T" + taxonomytreearr[i]);
                            if(newnode != null && newnode.parentNode == node.parentNode)
                               {Traverse(node,true);hasSibling=true;break;}
                        }
                    }
              }
            if(hasSibling == false)
            {
             checkParent(node);
            }
          }
    }

    function checkParent(node)
    { if(node!= null)
        {
              var subnode = node.parentNode;
              if(subnode!=null && subnode.id!="T0" &&  subnode.id!="")
              {
                        for(var j=0;j<subnode.childNodes.length;j++)
                          {var pid=subnode.childNodes[j].id;
                           if(document.getElementById("chkTree_"+pid).value == true || document.getElementById("chkTree_"+pid).value == "true")
                              {Traverse(subnode.childNodes[j],true);return;}
                          }
               checkParent(subnode.parentNode);
              }
        }
    }
    function Traverse(node,newvalue){
        if(node!=null){
            subnode=node.parentNode;
            if(subnode!=null && subnode.id!="T0" &&  subnode.id!=""){
                for(var j=0;j<subnode.childNodes.length;j++){
                    var n=subnode.childNodes[j]
                    if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                        var pid=subnode.id;
                        updatetreearr(pid.replace("T",""),"remove");
                        document.getElementById("chkTree_"+pid).value=eval(newvalue);
                        if (navigator.userAgent.indexOf("Firefox") > -1){
                            n.checked = eval(newvalue);
                            n.disabled = eval(newvalue);
                        }
                        else{
                            n.setAttribute("checked",eval(newvalue));
                            n.setAttribute("disabled",eval(newvalue));
                        }

                    }
                }
                if(HasChildren(subnode) && subnode.getAttribute("checked")){
                       subnode.setAttribute("checked",true);
                        subnode.setAttribute("disabled",true);
                }
                Traverse(subnode,newvalue);
            }
        }
    }
    function HasChildren(subnode)
    {
        if(subnode!=null){
            for(var j=0;j<subnode.childNodes.length;j++)
            {
                for(var j=0;j<subnode.childNodes.length;j++){
                    var n=subnode.childNodes[j]
                    if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                        var pid=subnode.id;
                        var v=document.getElementById("chkTree_"+pid).value;
                        if(v==true || v=="true"){
                        return true;break;
                        }
                    }
                }
            }
        }
        return false;
    }

    function SaveCategory()
    {
        var selected_nodes = document.getElementById('taxonomyselectedtree');
        var target = parent.document.getElementById('ekcategoryselection');
        if( target != null ) {
            target.value = selected_nodes.value;
        }
        top.CloseCategorySelect(false);
    }

    function Wait(bool)
    {
        if (bool)
        {
            ShowPane('dvWaitImage');
            document.getElementById("dvWaitImage").innerHTML = '<img src="images/application/loading_big.gif" alt="Please wait..." />';
            document.getElementById("dialog_publish").style.visibility = 'hidden';
            document.getElementById("dialog_cancel").style.visibility = 'hidden';

        } else {
            document.getElementById("dvWaitImage").innerHTML = '';
            document.getElementById("dialog_publish").style.visibility = 'visible';
            document.getElementById("dialog_cancel").style.visibility = 'visible';
            ShowPane('dvContent');
        }
        inPublishProcess = bool;
    }

    </script>
    <%if (TaxonomyRoleExists == true)
      {%>
    <script language="javascript" type="text/javascript">
    var taxonomytreemenu=true;
    var g_delayedHideTimer = null;
    var g_delayedHideTime = 1000;
    var g_wamm_float_menu_treeid = -1;
    var g_isIeInit = false;
    var g_isIeFlag = false;

    function IsBrowserIE(){
	    if (!g_isIeInit)
	    {
		    var ua = window.navigator.userAgent.toLowerCase();
		    g_isIeFlag = (ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1));
		    g_isIeInit = true;
	    }
	    return (g_isIeFlag);
    }

    function markMenuObject(markFlag, id) {
	    if (id && (id > 0)) {
		    var obj = document.getElementById(id);
		    if (obj && obj.className) {
			    if (markFlag) {
				    if (obj.className.indexOf("linkStyle_selected") < 0) {
					    obj.className += " linkStyle_selected";
				    }
			    }
			    else {
				    if (obj.className.indexOf("linkStyle_selected") >= 0) {
					    obj.className = "linkStyle";
				    }
			    }
		    }
	    }
    }

    function showWammFloatMenuForMenuNode(show, delay, event, treeId){
	    var el = document.getElementById("wamm_float_menu_block_menunode");
	    var visible = "";
	    if (el)
	    {
		    if (g_delayedHideTimer)
		    {
			    clearTimeout(g_delayedHideTimer);
			    g_delayedHideTimer = null;
		    }
		    var tree = null;
	        if (treeId > 0)
	        {
		        tree = TreeUtil.getTreeById(treeId);
	        }
	        if (tree && tree.node && tree.node.data)
	        {
		        visible = tree.node.data.visible;
		    }
		    if (show)
		    {
		        el.style.display = "none";
			    if (visible!="false")
					markMenuObject(false, g_wamm_float_menu_treeid);
			    if (null != event)
			    {
			        var hoverElement = $ektron("#" + treeId);
			        var offset = hoverElement.offset();
                    var hoverElementHeight = parseInt(hoverElement.height(), 10);
                    var hoverElementWidth = parseInt(hoverElement.width(), 10)
                    
                    var fixedPositionToolbarFix = 0;
                    if ($ektron("form#LibraryItem").length > 0) {
						fixedPositionToolbarFix = 44;
                    }
                    
                    el.style.top = (parseInt(offset.top, 10) + hoverElementHeight - 5 - fixedPositionToolbarFix) + "px";
	                el.style.left = (parseInt(offset.left, 10) + hoverElementWidth - 5) + "px";
				      
				    el.style.display = "";
				    if (treeId && (treeId > 0))
				    {
					    g_wamm_float_menu_treeid = treeId;
					    if (visible!="false")
							markMenuObject(true, treeId);
				    }
				    else
				    {
					    g_wamm_float_menu_treeid = -1;
				    }
			    }
		    }
		    else
		    {
			    if (delay)
			    {
				    g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
			    }
			    else
			    {
				    el.style.display = "none";
				    if (visible!="false")
						markMenuObject(false, g_wamm_float_menu_treeid);
			    }
		    }
	    }
    }

    function getEventX(event){
	    var xVal;
	    if (IsBrowserIE())
	    {
		    xVal = event.x;
	    }
	    else
	    {
		    xVal = event.pageX;
	    }
	    return(xVal)
    }

    function getShiftedEventX(event)
    {
	    var srcLeft;
	    var xVal;
	    if (IsBrowserIE())
	    {
		    xVal = event.x;
	    }
	    else
	    {
		    xVal = event.pageX;
	    }

	    // attempt to shift div-tag to the right of the menu items:
	    srcLeft = xVal;
	    if (event.srcElement && event.srcElement.offsetLeft){
		    srcLeft = event.srcElement.offsetLeft;
	    }
	    else if (event.target && event.target.offsetLeft){
		    srcLeft = event.target.offsetLeft;
	    }

	    if (event.srcElement) {
		    if (event.srcElement.offsetWidth) {
			    xVal = srcLeft + event.srcElement.offsetWidth;
		    }
		    else if (event.srcElement.scrollWidth) {
			    xVal = srcLeft + event.srcElement.scrollWidth;
		    }
	    }
	    else if (event.target && event.target.offsetLeft){
		    if (event.target.offsetWidth) {
			    xVal = srcLeft + event.target.offsetWidth;
		    }
		    else if (event.target.scrollWidth) {
			    xVal = srcLeft + event.target.scrollWidth;
		    }
	    }

	    return(xVal)
    }


    function getEventY(event){
	    var yVal;
	    if (IsBrowserIE())
	    {
		    yVal = event.y;
	    }
	    else
	    {
		    yVal = event.pageY;
	    }
	    return(yVal)
    }

     function wamm_float_menu_block_mouseover(obj) {
	    if (g_delayedHideTimer){
		    clearTimeout(g_delayedHideTimer);
		    g_delayedHideTimer = null;
	    }
     }

     function wamm_float_menu_block_mouseout(obj) {
	    if (null != obj){
		    g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
	    }
     }

    function routeAction(containerFlag, op){
	    var tree = null;
	    if (g_wamm_float_menu_treeid > 0){
		    tree = TreeUtil.getTreeById(g_wamm_float_menu_treeid);
	    }

	    if (tree && tree.node && tree.node.data){
		    var TaxonomyId = tree.node.data.id;
		    var ParentId = tree.node.pid;
		    if(ParentId==null || ParentId=='undefined'){
		        ParentId=0;
		    }

		    showWammFloatMenuForMenuNode(false, false, null, -1);
		    LoadChildPage(op,TaxonomyId,ParentId);
	    }
    }
    function LoadChildPage(Action, TaxonomyId, ParentId) {
	    var frameObj = document.getElementById("ChildPage");
	    var lastClickedOn = document.getElementById("LastClickedOn");
	    lastClickedOn.value= TaxonomyId;
	    document.getElementById("LastClickedParent").value=ParentId;
	    if(parseInt(ParentId)==0){document.getElementById("ClickRootCategory").value="true";}
	    else{document.getElementById("ClickRootCategory").value="false";}
	    switch (Action){
		    case "add":
		        if (TaxonomyId == "") {
		            alert("Please select a taxonomy.");
		            return false;
	            }
			    frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=add&parentid="+TaxonomyId ;
			    break;
    	    default :
			    break;
	    }
	    if(Action!="delete"){
	        DisplayIframe();
	    }
    }
    function DisplayIframe(){
        var pageObj = document.getElementById("FrameContainer");
        pageObj.style.display = "";
        if(navigator.userAgent.indexOf("MSIE 6.0") > -1){
            pageObj.style.width = "100%";
			pageObj.style.height = "500px";
	    }
	    else{
	        pageObj.style.width = "95%";
	        pageObj.style.height = "95%";
	    }
    }
    function CancelIframe(){
	    var pageObj = document.getElementById("FrameContainer");
	    pageObj.style.display = "none";
	    pageObj.style.width = "1px";
	    pageObj.style.height = "1px";
    }
    function CloseChildPage(){
        CancelIframe();
	    var ClickRootCategory = document.getElementById("ClickRootCategory");
	    var lastClickedOn = document.getElementById("LastClickedOn");
	    var clickType = document.getElementById("ClickType");
        if (ClickRootCategory.value == "true")
            __EkFolderId="<%=m_intTaxFolderId%>";
        else{
            __EkFolderId=-1;
            TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
        }
        var node = document.getElementById("T" + lastClickedOn.value );
        if(node!=null){
            for (var i=0;i<node.childNodes.length;i++){
		        if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9)
		        {
			        if(node.childNodes(i).nodeName == 'LI' || node.childNodes(i).nodeName == 'UL'){
				        var parent = node.childNodes(i).parentElement;
				        parent.removeChild( node.childNodes(i));
			        }
		        }
		        else
		        {
			        if(node.childNodes[i].nodeName == 'LI' || node.childNodes[i].nodeName == 'UL'){
				        var parent = node.childNodes[i].parentNode;
				        parent.removeChild( node.childNodes[i]);
			        }
		        }
            }
            TREES["T" + lastClickedOn.value].children = [];
            TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
            onToggleClick(lastClickedOn.value,TreeUtil.addChildren,lastClickedOn.value);
        }
    }
    </script>
	<% if(Page.Request.Url.AbsoluteUri.IndexOf("membership_add_content.aspx")== -1 && Page.Request.Url.ToString().IndexOf("forum=1") == -1){ %>
    <div id="wamm_float_menu_block_menunode" class="Menu" style="position:absolute; left:10px; top:10px;
        display:none; z-index:3200;" onmouseover="wamm_float_menu_block_mouseover(this)"
        onmouseout="wamm_float_menu_block_mouseout(this)">
        <input type="hidden" name="LastClickedParent" id="LastClickedParent" value="" />
        <input type="hidden" name="ClickRootCategory" id="ClickRootCategory" value="false" />
        <ul>
            <li class="MenuItem add">
                <a href="#" onclick="routeAction(true, 'add');"><%=(m_refMsg.GetMessage("generic add title"))%></a>
            </li>
        </ul>
    </div>
    <% } %>
     <% } %>
    <%else{%>
    <script type="text/javascript" >
        var taxonomytreemenu=false;
    </script>
    <% } %>   
<script type="text/javascript">
	var taxonomytreemode="editor";var ____ek_appPath2="";
</script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.url.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.init.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.config.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.explorer.windows.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.types.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.parser.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.toolkit.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.cms.api.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.contextmenu.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.iconlist.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.tabs.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.explore.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.ui.taxonomytree.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.net.http.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.lang.exception.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.form.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.log.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.dom.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.debug.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.string.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.cookie.js"></script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.querystring.js"></script>
<script type="text/javascript">
var clickedElementPrevious = null;
var clickedIdPrevious = null;

function onDragEnterHandler( id, element ){
  folderID = id;
  if( clickedElementPrevious != null ) {
    clickedElementPrevious.style["background"] = "#ffffff";
    clickedElementPrevious.style["color"] = "#000000";
  }
  element.style["background"] = "#3366CC";
  element.style["color"] = "#ffffff";
}

function onMouseOverHandler( id, element ){
  element.style["background"] = "#ffffff";
  element.style["color"] = "#000000";
}

function onDragLeaveHandler( id, element ) {
  element.style["background"] = "#ffffff";
  element.style["color"] = "#000000";
}

function onFolderClick( id, clickedElement ){
  var tree = null;
  var visible = "";
  if (id > 0)
  {
    tree = TreeUtil.getTreeById(id);
  }
  if (tree)
  {
    if(tree.node){
      if(tree.node.data){
        visible = tree.node.data.visible;
      }
    }
  }
  if( clickedElementPrevious != null ) {
  var previousTree = null;
  var previousVisible = "";
  if (clickedElementPrevious.id > 0)
    previousTree = TreeUtil.getTreeById(clickedElementPrevious.id);
  if (previousTree){
    if(previousTree.node){
      if( previousTree.node.data){
        previousVisible = previousTree.node.data.visible;}
      }
    }
    if(previousVisible != "false"){
      clickedElementPrevious.style["background"] = "#ffffff";
      clickedElementPrevious.style["color"] = "#000000";
    }
    else{
      clickedElementPrevious.style["background"] = "#808080";
      clickedElementPrevious.style["color"] = "#000000";
    }
  }
  if(visible != "false"){
    clickedElement.style["background"] = "#3366CC";
    clickedElement.style["color"] = "#ffffff";
  }
  else{
    clickedElement.style["background"] = "#808080";
    clickedElement.style["color"] = "#ffffff";
  }
  clickedElementPrevious = clickedElement;
  clickedIdPrevious = id;

  var name = clickedElement.innerText;
  var folder = new Asset();
  folder.set( "name", name );
  folder.set( "id", id );
  folder.set("folderid",__EkFolderId);
  __EkFolderId=-1;
}

function onToggleClick( id, callback, args ){
  toolkit.getAllSubCategory( id, -99, callback, args );
}

function makeElementEditable( element ) {
  element.contentEditable = true;
  element.focus();
  element.style.background = "#fff";
  element.style.color = "#000";
}

var baseUrl = URLUtil.getAppRoot(document.location) + "images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder  = baseUrl + "taxonomyCollapsed.png";
TreeDisplayUtil.plusopenfolder   = baseUrl + "taxonomyCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "taxonomyExpanded.png";
TreeDisplayUtil.minusopenfolder  = baseUrl + "taxonomyExpanded.png";
TreeDisplayUtil.folder = baseUrl + "taxonomy.png";

var g_menu_id = "";
function displayCategory( categoryRoot ) {
  document.body.style.cursor = "default";
  var taxonomyTitle = null;
  try {
    taxonomyTitle = categoryRoot.title;
    g_menu_id = categoryRoot.id;
  } catch( e ) {
  ;
  }

  if( taxonomyTitle != null ) {
    treeRoot = new Tree( taxonomyTitle, __TaxonomyOverrideId, null, categoryRoot, 0 );
    TreeDisplayUtil.showSelf( treeRoot, document.getElementById( "TreeOutput" ) );
    TreeDisplayUtil.toggleTree( treeRoot.node.id );
  } else {
    var element = document.getElementById( "TreeOutput" );
    var debugInfo = "<b>Cannot connect to the service</b>";
    element.innerHTML = debugInfo;
  }
}

var toolkit = new EktronToolkit();
toolkit.getTaxonomy( __TaxonomyOverrideId, -99, displayCategory, __TaxonomyOverrideId );

function reloadTreeRoot( id ){
  TREES = {};
  toolkit.getTaxonomy( id, -99, displayCategory, __TaxonomyOverrideId );
}

var g_selectedFolderList = "0";
var g_timerForFolderTreeDisplay;
function showSelectedFolderTree(){
  if (g_timerForFolderTreeDisplay){
    window.clearTimeout(g_timerForFolderTreeDisplay);
  }
  g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
}

function showSelectedFolderTree_delayed() {
  var bSuccessFlag = false;
  if (g_timerForFolderTreeDisplay){
    window.clearTimeout(g_timerForFolderTreeDisplay);
  }

  if (g_selectedFolderList.length > 0){
    var tree = TreeUtil.getTreeById(g_menu_id);
    if (tree){
      var lastId = 0;
      var folderList = g_selectedFolderList.split(",");
      bSuccessFlag = TreeDisplayUtil.expandTreeSet( folderList );
    }

    if (!bSuccessFlag){
      g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
    }
  }
}
</script>
<asp:Literal ID="litInitialize" runat="server"  />
</body>
</html>
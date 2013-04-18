<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserSelectControl.ascx.cs" Inherits="controls_Community_Components_UserSelectControl" %>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
    <style type="text/css">
        .UserSelectTargetCtl {padding: 10px; overflow: auto;} 
        .UserSelectTargetCtl .CommunitySearchCtl {width: 100%;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_GroupFilterFieldset{display: none;}
        /*
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_BasicSearchTab{display: none;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_AdvancedSearchTab{display: none;}
        */
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_LocationSearchTab{display: none;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_UserFilterFieldset{border: none;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_UserFilterFieldset legend{display: none;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_AdvancedContainer_Selected {padding: 0px;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_AdvancedContainer_Selected .CommunitySearch_SearchButtonContainer{margin-left: 5px;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_UserFilterContainer .CommunitySearch_FilterModeContainer select{width: 125px;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_UserFilterContainer .CommunitySearch_FilterTextboxContainer input{width: 125px;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_ResultTableHeadAvatar{width: 20%;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_ResultTableHeadMember{}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_ResultTableHeadSelect{width: 12%;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_ResultTable .CommunitySearch_Result_InfoBlockContainer{left: -95px;}
        .UserSelectTargetCtl .CommunitySearchCtl .FriendsOnlyButton{margin-left: 25px;margin-right: 0px;}
        .UserSelectTargetCtl .CommunitySearchCtl .FriendsOnlyButtonLabel{background-color: #FFFFFF;display: inline;margin-left: 0px;}
        .UserSelectTargetCtl .CommunitySearchCtl .CommunitySearch_SearchButtonContainer {margin-top: 0px;}
    </style>
<div class="UserSelectCtl">
    <div class="UserSelectTargetCtl" >
        <cms:CommunitySearch ID="usersel_comsearch" runat="server" EnableMap="false" StartingTab="basic" EnableGroupResults="false" />
    </div>
</div>

<asp:Literal ID="usersel_comsearch_jsinit" runat="server" />
<script type="text/javascript" language="javascript">
//<!-- 
    function UserSelectCtl_Initialize(){
        if ("undefined" != typeof CommunitySearchClass){
            CommunitySearchClass.CallbackWhenReady(usersel_comsearch_ClientID, UserSelectCtl_Initialize_Complete);
        }
        else{
            setTimeout('UserSelectCtl_Initialize()', 50);
        }
    }

    function UserSelectCtl_Initialize_Complete(){
        CommunitySearchClass.Hook_NotifySearchResultsChanged(usersel_comsearch_ClientID, UserSelectCtl_NotifySearchResultsChanged);
        // kick-off a search for all users:
        CommunitySearchClass.DoSearch(usersel_comsearch_ClientID, '');
    }

    function UserSelectCtl_NotifySearchResultsChanged(containerObj){
        if (containerObj && containerObj.innerHTML && containerObj.innerHTML.length > 0){
            var userList = null;
            if (("undefined" != typeof window["UserSelectCtl_UserList" + usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserList" + usersel_comsearch_ClientID])){
                userList = window["UserSelectCtl_UserList" + usersel_comsearch_ClientID];
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
    }

    function UserSelectCtl_CheckAll(chkboxObj, ctrlId) {
        var containerObj = document.getElementById('CommunitySearch_ResultsContainer_Table_' + ctrlId);
        if (containerObj && containerObj.innerHTML && containerObj.innerHTML.length > 0) {
            var els = containerObj.getElementsByTagName('input');
            var checkAll = els[0].checked;
            
            var userList = new Array;
            var userNameList = new Array;
            window["UserSelectCtl_UserList" + usersel_comsearch_ClientID] = userList;
            window["UserSelectCtl_UserNameList" + usersel_comsearch_ClientID] = userNameList;

            for (var idx = 0; idx < els.length; idx++) {
                if (("checkbox" == els[idx].type) && (els[idx].name.length > 0)) {

                    if (checkAll == false) {
                        els[idx].checked = false;
                    }
                    else {
                        userList[els[idx].name] = true;
                        userNameList[els[idx].name] = els[idx].alt;
                        els[idx].checked = true;
                    }
                }
            }
        }
    }

    function UserSelectCtl_CheckboxClicked(chkboxObj, ctrlId, userId, displayName) {
        var userList = null;
        var userNameList = null;
        if (chkboxObj.checked && usersel_comsearch_SingleSelection){
            window["UserSelectCtl_UserList" + usersel_comsearch_ClientID] = null;
        }
        if (chkboxObj.checked && usersel_comsearch_SingleSelection){
            //UserSelectCtl_CheckAll(chkboxObj, ctrlId);
            chkboxObj.checked = true;
        }

        if (("undefined" != typeof window["UserSelectCtl_UserList" + usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserList" + usersel_comsearch_ClientID])){
            userList = window["UserSelectCtl_UserList" + usersel_comsearch_ClientID];
        }
        else{
            userList = new Array;
            window["UserSelectCtl_UserList" + usersel_comsearch_ClientID] = userList;
        }
        userList[userId] = chkboxObj.checked;

        // save usernames for each id:
        if (("undefined" != typeof window["UserSelectCtl_UserNameList" + usersel_comsearch_ClientID]) && (null != window["UserSelectCtl_UserNameList" + usersel_comsearch_ClientID])){
            userNameList = window["UserSelectCtl_UserNameList" + usersel_comsearch_ClientID];
        }
        else{
            userNameList = new Array;
            window["UserSelectCtl_UserNameList" + usersel_comsearch_ClientID] = userNameList;
        }
        userNameList[userId] = displayName;
    }
    
    function UserSelectCtl_GetSelectUsers(id){
        var userList = null;
        var result = '';
        if (("undefined" != typeof window["UserSelectCtl_UserList" + id]) && (null != window["UserSelectCtl_UserList" + id])){
            userList = window["UserSelectCtl_UserList" + id];
        }
        else{
            userList = new Array;
            window["UserSelectCtl_UserList" + id] = userList;
        }
        
        for (var userItem in userList){
            if (userList[userItem]){
                if (result.length)
                    result += ",";
                result += userItem.toString();
            }
        }
        return (result);
    }

    function UserSelectCtl_GetUserName(ctlId, userId){
        var userNameList = null;
        var result = '';
        if (("undefined" != typeof window["UserSelectCtl_UserNameList" + ctlId]) && (null != window["UserSelectCtl_UserNameList" + ctlId])){
            userNameList = window["UserSelectCtl_UserNameList" + ctlId];
        }
        else{
            userNameList = new Array;
            window["UserSelectCtl_UserNameList" + ctlId] = userNameList;
        }
        if (("undefined" != typeof userNameList[userId]) && (null != userNameList[userId]))
            result = userNameList[userId];
        return (result);
    }
    //Initialize:
    UserSelectCtl_Initialize();
//-->
</script>


<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditMessage.ascx.cs" Inherits="controls_Community_Messaging_EditMessage" Debug="true" %>
<%@ Register TagPrefix="Community" TagName="UserSelectControl" Src="../Components/UserSelectControl.ascx" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../../../controls/Editor/ContentDesignerWithValidator.ascx" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <div class="messageSpacer"></div>
    <div class="messageUILoading"></div>
    <div class="newMessage">
        <table class="ektronForm">
            <tr>
                <td id="MsgToLabel" class="label" runat="server"></td>
                <td class="value">
                    <asp:Literal ID="ltrMsgTo" runat="server" />
                    <asp:Literal ID="ltrBrowseFriends" runat="server" />
                    <br />

                    <div id="MessageTargetUI<%=(this.ClientID) %>" class="dv_MessageTargetUI" style="display: none;">
                        <div class="EkTB_dialog">
                            <div class="EktMsgTargetCtl" >
                                <div class="EktMsgTargets">
                                    <!-- asp_Literal ID="ltr_recipientselect" run_at="Server"></asp_Literal -->
                                    <Community:UserSelectControl id="Invite_UsrSel" runat="server" />
                                    <asp:button id="cgae_userselect_done_btn" runat="server" />
                                    <asp:button id="cgae_userselect_cancel_btn" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
	          </tr>
	          <tr>
		          <td id="MsgSubjectLabel" class="label" runat="server"></td>
		          <td class="value"><asp:Literal ID="ltrMsgSubject" runat="server" /></td>
	          </tr>
	          <tr>
		          <td colspan="2">
			          <asp:Literal ID="ltrMsgView" runat="server" />
			          <ektron:ContentDesigner ID="cdContent_teaser" runat="server" AllowScripts="true" Height="500" Width="100%" Visible="false"
                          Toolbars="Minimal" ShowHtmlMode="false" />
		          </td>
	          </tr>
        </table>
    </div>
</div>

<asp:Literal ID="litHdnToUserIds" runat="server" />
<input type="hidden" id="hdnRecipientsValidated<%=(this.ClientID) %>" name="hdnRecipientsValidated<%=(this.ClientID) %>" value="" />

<script type="text/javascript" language="javascript">
<!--

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
	    this.getId = function(){
			return (this.id);
		},

	    this.SetUserSelectId = function (userSelId){
	        this.userSelId = userSelId;
	    },

	    this.MsgShowMessageTargetUI = function(hdnId){
			var divObj = document.getElementById('MessageTargetUI' + this.getId());
		    if (divObj && divObj.style.display == ''){
			    divObj.style.display = 'none';
		    }
		    else if (divObj && divObj.style.display == 'none'){
					divObj.style.display = '';
			}
		},

	    this.MsgSaveMessageTargetUI = function(){
			var hdbObj = document.getElementById('ektouserid' + this.getId());
			var toObj = document.getElementById('ekpmsgto' + this.getId());
			var idx;
				if (hdbObj){
					hdbObj.value = '';
			    var users = UserSelectCtl_GetSelectUsers(this.userSelId);
			    if (("undefined" != users) && (null != users)){
			        hdbObj.value = users;
								if (toObj){
			            toObj.value='';
			            var userArray = users.split(',');
			            for (idx = 0; idx < userArray.length; idx++){
				            if(toObj && toObj.value.length > 0){
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


	    this.MsgCloseMessageTargetUI = function(){
			var divObj = document.getElementById('MessageTargetUI' + this.getId());
			if (divObj){
				divObj.style.display = 'none';
			}
		    ektb_remove();
		},

	    this.MsgCancelMessageTargetUI = function(){
			this.MsgCloseMessageTargetUI();
		},

		this.SendMessage = function(){
			var hdbObj = document.getElementById('ektouserid' + this.getId());
			if (hdbObj && (hdbObj.value.length > 0)){
				hdbObj = document.getElementById('hdnRecipientsValidated' + this.getId());
				if (hdbObj){
					hdbObj.value = "1";
					document.forms[0].submit();
				}
			}
			else {
				alert('<%=m_refMsg.GetMessage("js: no recipients")%>');
			}
		},

	///////////////////////////
	// initialize properties:
	this.id = idTxt;
	    this.name = '';
	this.MsgUsersSelArray = new Object();
	    this.userSelId = '';
}
    Ektron.ready(function()
        {
            $ektron(".messageUILoading, .messageSpacer").hide();
            $ektron(".newMessage").show();
        }
    );
-->
</script>
<asp:Literal ID="ltrMsgJSObjectId" runat="server" ></asp:Literal>



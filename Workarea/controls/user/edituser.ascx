<%@ Control Language="C#" AutoEventWireup="true" Inherits="edituser" CodeFile="edituser.ascx.cs" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../../controls/Editor/ContentDesignerWithValidator.ascx" %>

<script type="text/javascript">
    var jsIsAdmin=<asp:literal id="jsIsAdmin" runat="server"/>;
    function RestoreDefault() {
	    var msg = "Are you sure you would like to restore user preferences to the system default?";
	    var jsPreferenceFolderId="<asp:literal id="jsPreferenceFolderId" runat="server"/>";
	    var jsPreferenceWidth="<asp:literal id="jsPreferenceWidth" runat="server"/>";
	    var jsPreferenceHeight="<asp:literal id="jsPreferenceHeight" runat="server"/>";
	    var jsPreferenceTemplate="<asp:literal id="jsPreferenceTemplate" runat="server"/>";
	    var jsPreferenceDispTitleTxt="<asp:literal id="jsPreferenceDispTitleTxt" runat="server"/>";        

	    if (!confirm(msg)) {
		    return false;
	    }
	    if(jsPreferenceFolderId=="")
		    document.forms[0].chkSmartDesktop.checked = true;
	    else
		    document.forms[0].chkSmartDesktop.checked = false;

	    document.forms[0].txtWidth.value = jsPreferenceWidth;
	    document.forms[0].txtHeight.value = jsPreferenceHeight;
	    if (9999 == jsPreferenceWidth && 9999 == jsPreferenceHeight)
	    {
	        $ektron("#chkFullScreen").attr("checked","on");
            $ektron("td input#txtWidth").parent().parent().hide();
            $ektron("td input#txtHeight").parent().parent().hide();
	    }
	    if(jsPreferenceDispTitleTxt=="1")
		    document.forms[0].chkDispTitleText.checked = true;
	    else
		    document.forms[0].chkDispTitleText.checked = false;

	    document.forms[0].templatefilename.value=jsPreferenceTemplate;
	    return false;
    }

	function ShowAddPersonalTagArea(){
        $ektron("#newTagNameDiv").modalShow();
	}

	this.customPTagCnt = 0;
	function SaveNewPersonalTag(){
		// add new tag:
		//<input " + IIf(htTagsAssignedToUser.ContainsKey(td.Id), "checked=""checked"" ", "") + " type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;" + td.Text + "<br />
		var objTagName = document.getElementById("newTagName");
		var objTagLanguage = document.getElementById("TagLanguage");
		var objLanguageFlag = document.getElementById("flag_" + objTagLanguage.value);

		var divObj = document.getElementById("newAddedTagNamesDiv");

		if(!CheckForillegalChar(objTagName.value)){
		    return;
		}

		if (objTagName && (objTagName.value.length > 0) && divObj){
			++this.customPTagCnt;
			divObj.innerHTML += "<input type='checkbox' checked='checked' onclick='ToggleCustomPTagsCbx(this, \"" + objTagName.value + "\");' id='userCustomPTagsCbx_" + this.customPTagCnt + "' name='userCustomPTagsCbx_" + this.customPTagCnt + "' />&#160;"

			if(objLanguageFlag != null){
			    divObj.innerHTML += "<img src='" + objLanguageFlag.value + "' border=\"0\" />"
			}

			divObj.innerHTML +="&#160;" + objTagName.value + "<br />"

			AddHdnTagNames(objTagName.value + '~' + objTagLanguage.value);
		}

		// now close window:
		CancelSaveNewPersonalTag();
	}

	function CancelSaveNewPersonalTag(){
	    $ektron("input#newTagName")[0].value = "";
        $ektron("#newTagNameDiv").modalHide();
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
		}
		else{
			RemoveHdnTagNames(tagName);
			btnObj.checked = false; // otherwise re-checks when adding new custom tag.
		}
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
    function ShowGrid(mode) 
    {
        
         var obj = document.getElementById('dvCollGrid');
         var obj1 = document.getElementById('dvGroupGrid');
         var obj2 = document.getElementById('dvPrivacyGrid');
         if(mode=='Coll')
         {
            if (obj)
            {
              obj.style.display = 'block';
              obj1.style.display ='none'; 
              obj2.style.display = 'none';
              document.getElementById('dvColleaguetab').className='SelectedTab';
              document.getElementById('dvGrouptab').className='UnSelectedTab';
              document.getElementById('dvPrivacytab').className='UnSelectedTab';
            }
          }
         else if(mode=='Privacy')
         {
          obj.style.display = 'none';
          obj1.style.display = 'none';
          obj2.style.display = 'block';
          document.getElementById('dvColleaguetab').className='UnSelectedTab';
          document.getElementById('dvGrouptab').className='UnSelectedTab';
          document.getElementById('dvPrivacytab').className='SelectedTab';
         
         }
         else
         {
          obj.style.display = 'none';
          obj1.style.display = 'block';
          obj2.style.display = 'none';
          document.getElementById('dvColleaguetab').className='UnSelectedTab';
          document.getElementById('dvGrouptab').className='SelectedTab';
          document.getElementById('dvPrivacytab').className='UnSelectedTab';
         
         }
     }
       

    function AvatorDialogClose()
    {
         var jsAvatorUploadDlgId = "<asp:literal id="jsUxDialogSelectorTxt" runat="server"/>";
         $ektron(jsAvatorUploadDlgId).dialog('close'); 
         return false;
    }

    function AvatarDialogInit()
    {
        setTimeout( function() 
        {
            $ektron(document).find(".uxAvatarUploadIframe").attr("src", "upload.aspx?action=edituser&addedit=true&returntarget=avatar&modal=true");//alert($ektron(jsAvatorUploadDlgId).length);
        }, 0);
    }

    Ektron.ready( function() {
        //Tag Modal
        $ektron("#newTagNameDiv").modal({
            trigger: '',
            modal: true,
            toTop: true,
            onShow: function(hash){
                hash.o.fadeIn();
                hash.w.fadeIn();
            },
            onHide: function(hash){
                hash.w.fadeOut("fast");
                hash.o.fadeOut("fast", function(){
                    if (hash.o){
                        hash.o.remove();
                    }
                });
            }
        });
    });
</script>

<style type="text/css">
    .selectContent { background-image: url('Images/ui/icons/check.png'); background-repeat: no-repeat; background-position:.5em center; padding-bottom: .2em !important; padding-top: .2em !important; line-height: 16px !important;}
    .minWidth { width: auto !important; }
    div#newTagNameDiv { height: 100px; width:350px; margin: 10em 0 0 -15em; border: solid 1px #aaaaaa; background-color: white; }
    .tagLabel { color: #1D5987; font-weight: bold; text-align: right; white-space: nowrap; width: 10%; }
    .wrapText { white-space: normal !important; width: 20%; }
</style>
<div id="FrameContainer" style="position: absolute; top: 48px; left: 55px; width: 1px;
    z-index: 9999; height: 1px; display: none;">
    <iframe id="ChildPage" frameborder="yes" marginheight="0" marginwidth="0" width="100%"
        height="100%" scrolling="auto"></iframe>
</div>
<div id="dhtmltooltip">
</div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server">
    </div>
    <div class="ektronToolbar" id="htmToolBar" runat="server">
    </div>
</div>
<div class="ektronPageContainer">
    <div class="ektronPageInfo">
        <div class="tabContainerWrapper">
            <div class="tabContainer">
                <ul>
                    <li><a title="General" href="#dvGeneral"><%=m_refMsg.GetMessage("general label")%></a></li>
                    <asp:PlaceHolder ID="phWorkareaTab" runat="server">
                    <li><a title="Workpage" href="#dvWorkpage"><%=m_refMsg.GetMessage("workarea options label")%></a></li>
                    </asp:PlaceHolder>
                    <li><a title="Custom" href="#dvCustom"><%=m_refMsg.GetMessage("lbl custom")%></a></li>
                    <li id="activitiesTab" runat="server"><a title="Activities" href="#dvActivities"><%=m_refMsg.GetMessage("lbl activities")%></a></li>
                    <li id="aliasTab" runat="server"><a title="Alias" href="#dvAlias"><%=m_refMsg.GetMessage("lbl profile links")%></a></li>
                 </ul>
                <div id="dvGeneral">
                    <table class="ektronGrid">
                        <tr>
                            <td class="label" title="Username"><span style="color:red;">*</span><%=m_refMsg.GetMessage("username label")%></td>
                            <td><asp:Literal ID="username" runat="server" /></td>
                        </tr>
                        <tr id="TR_domain" runat="server">
                            <td class="label" title="Domain"><% =(m_refMsg.GetMessage("domain title")) %></td>
                            <td id="TD_path" runat="server"></td>
                        </tr>
                        <tr id="TR_organization" runat="server">
                            <td class="label" title="Organization"><%=m_refMsg.GetMessage("org label")%></td>
                            <td><asp:Literal ID="org" runat="server" /></td>
                        </tr>
                        <tr id="TR_orgunit" runat="server">
                            <td class="label" title="Organizational Unit"><%=m_refMsg.GetMessage("org unit label")%></td>
                            <td><asp:Literal ID="orgunit" runat="server" /></td>
                        </tr>
                        <tr id="TR_ldapdomain" runat="server">
                            <td class="label" title="Path"><%=m_refMsg.GetMessage("generic path")%>:</td>
                            <td><asp:Literal ID="ldapdomain" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Id"><%=m_refMsg.GetMessage("generic id")%>:</td>
                            <td><asp:Literal ID="ltr_uid" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="First Name"><span style="color:red;">*</span><%=m_refMsg.GetMessage("first name label")%></td>
                            <td><asp:Literal ID="firstname" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Last Name"><span style="color:red;">*</span><%=m_refMsg.GetMessage("last name label")%></td>
                            <td><asp:Literal ID="lastname" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Display Name"><span style="color:red;">*</span><%=m_refMsg.GetMessage("display name label")%>:</td>
                            <td><asp:Literal ID="displayname" runat="server" /></td>
                        </tr>
                        <tr>
                            <asp:Literal ID="hppwd" runat="server" />
                        </tr>
                        <tr>
                            <asp:Literal ID="confirmpwd" runat="server" />
                        </tr>
                        <%//If m_intGroupType = 0 Then%>
                        <tr>
                            <td class="label" title="User Language"><%=m_refMsg.GetMessage("user language label")%></td>
                            <td><asp:Literal ID="language" runat="server" /></td>
                        </tr>
                        <%//End If %>
                        <tr>
                            <asp:Literal ID="email" runat="server" />
                        </tr>
                        <%if ((this.m_refContentApi.RequestInformationRef.LoginAttempts == -1) && ((!string.IsNullOrEmpty(security_data.ToString())) && security_data.IsAdmin) || this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminUsers)) {%>
                        <tr>
                            <td class="label" title="The account is locked. Please contact your administrator."><%=m_refMsg.GetMessage("account locked")%></td>
                            <td><asp:Literal ID="accountLocked" runat="server" /></td>
                        </tr>
                        <%}%>
                        <tr>
                            <td class="label" title="Content and Forum Editor"><%=m_refMsg.GetMessage("lbl editor")%>:</td>
                            <td><asp:DropDownList ToolTip="Select Content and Forum Editor from Drop Down List" ID="drp_editor" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Avatar"><%=m_refMsg.GetMessage("lbl avatar")%>:</td>
                            <td><asp:Literal ID="ltr_avatar" runat="Server" /> 
                             <ektronUI:Dialog ID="uxDialog" CssClass="EktronAvatarUploadUI" Width="400" Height="300" Modal="true" Title="Avatar Upload" runat="server">
                                <ContentTemplate>
                                    <iframe class="uxAvatarUploadIframe xUploadUIControls" frameborder="0" border="0"  ID="uxAvatarUploadIframe" scrolling="no" runat="server" height="100%" width="100%"></iframe>	
                                </ContentTemplate>
                            </ektronUI:Dialog>
                            <asp:Literal ID="ltr_upload" runat="server"/>
							</td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Literal ID="TD_msg" runat="server" />
                                <div class="ektronTopSpace">
                                </div>
                                <%if (m_intGroupType == 0) {%>
                                <asp:Literal ID="enablemsg" runat="server" />
                                <%}%>
                            </td>
                            <td><asp:Literal ID="ltr_checkBox" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Address">
                                <%=m_refMsg.GetMessage("lbl map address")%>
                                :</td>
                            <td><asp:Literal ID="ltrmapaddress" runat="Server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Latitude">
                                <%=m_refMsg.GetMessage("lbl map latitude")%>
                                :</td>
                            <td><asp:Literal ID="ltrmaplatitude" runat="Server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Longitude">
                                <%=m_refMsg.GetMessage("lbl map longitude")%>
                                :</td>
                            <td><asp:Literal ID="ltrmaplongitude" runat="Server" /></td>
                        </tr>
                        <tr>
                            <td class="label" id="TD_signature" runat="server" title="Forum Signature"><%=m_refMsg.GetMessage("lbl signature")%>:</td>
                            <td><asp:PlaceHolder ID="ltr_sig" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Tags"><%=m_refMsg.GetMessage("lbl personal tags")%></td>
                            <td id="TD_personalTags" runat="server"></td>
                        </tr>
                    </table>
                </div>
                <asp:PlaceHolder ID="phWorkareaContent" runat="server">
                <div id="dvWorkpage">
                    <table class="ektronGrid">
                        <%if (m_intGroupType == 0) {%>
                        <asp:Literal ID="lockedmsg" runat="server" />
                        <tr>
                            <td class="label">
                                <%= m_refMsg.GetMessage("lbl fullscreen")%>
                                :</td>
                            <td>
                                <input title="Full Screen" type="checkbox" id="chkFullScreen" name="chkFullScreen" onclick="javascript:enableWidthHeight(this);" /></td>
                        </tr>
                        <tr>
                            <asp:Literal ID="width" runat="server" />
                        </tr>
                        <tr>
                            <asp:Literal ID="height" runat="server" />
                        </tr>
                        <%--<tr>
                            <asp:Literal ID="disptext" runat="server" />
                        </tr>--%>
                        <tr>
                            <td class="label" title="Landing Page After Login">
                                <%=m_refMsg.GetMessage("lbl landing page after login")%>
                                <asp:Literal ID="forcemsg" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <asp:Literal ID="folder" runat="server" />
                        </tr>
                        <asp:Literal ID="preference" runat="server" />
                        <%}%>
                    </table>
                </div>
                </asp:PlaceHolder>
                <div id="dvCustom">
                    <table class="ektronGrid ektronForm">
                        <asp:Literal ID="litUCPUI" runat="server" />
                    </table>
                    <input type="hidden" id="hdnHeight" name="hdnHeight" value="9999" />
                    <input type="hidden" id="hdnWidth" name="hdnWidth" value="9999" />
                </div>
                <div id="dvActivities" class="EkMembershipActivityTab">
                     <table id="EkMembershipActivityTable" cellspacing="0" runat="server">
                        <tr>
                        <td class="subTabsWrapper">
                                <ul class="activities_tab_subTabs">
                                <li id="dvColleaguetab" class="SelectedTab"><a title="Friends" href="#" onclick="ShowGrid('Coll');return false;">
                                    <%=m_refMsg.GetMessage("lbl friends")%></a></li>
                                <li id="dvGrouptab" class="UnSelectedTab"><a title="Groups" href="#" onclick="ShowGrid('Group');return false;">
                                     <%=m_refMsg.GetMessage("lbl groups")%></a></li>
                                <li id="dvPrivacytab" class="UnSelectedTab"><a title="Privacy" href="#" onclick="ShowGrid('Privacy');return false;">
                                     <%=m_refMsg.GetMessage("lbl privacy")%></a></li>
                                </ul>
                          </td>
                             <td>
                                <div class="dvCollGrid" id="dvCollGrid">
                                <span class="EkActivityNotifyText" title="Notify me about these colleague activities"> <%=m_refMsg.GetMessage("lbl notify colleagues activities")%></span>
                                    <asp:GridView ID="CollGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                                        CssClass="ektronGrid" GridLines="None">
                                        <HeaderStyle CssClass="title-header" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td>
                                <div class="dvGroupGrid" id="dvGroupGrid" style="display: none;">
                                <span class="EkActivityNotifyText" title="Notify me about these Community Group activities"><%=m_refMsg.GetMessage("lbl notify groups activities")%></span>
                                    <asp:GridView ID="GroupGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                                        CssClass="ektronGrid" GridLines="None">
                                        <HeaderStyle CssClass="title-header" />
                                    </asp:GridView>
                                </div>
                            </td>
                            <td>
                            <div class="dvPrivacyGrid" id="dvPrivacyGrid" style="display:none;">
                            <span class="EkActivityNotifyText" title="Notify others about my activities"> <%=m_refMsg.GetMessage("lbl notify my activities")%></span>
                                 <asp:GridView ID="PrivacyGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                                      CssClass="ektronGrid" GridLines="None">
                                      <HeaderStyle CssClass="title-header" />
                                </asp:GridView>  
                            </div>
                            </td>
                        </tr>
                    </table>
                </div>
              <div id="dvAlias">
               <table class="ektronGrid" id="tblAliasList" runat="server">
                   <tr>
                       <td >  
                          <p style="width: auto; height: auto; overflow: auto;" class="groupAliasList" ><%=groupAliasList%></p>
                       </td>
                   </tr>
               </table>
            </div>
            </div>
        </div>
    </div>
</div>
<input type="hidden" name="netscape" onkeypress="return CheckKeyValue(event,'34');"
    id="Hidden43" />
<input type="hidden" id="IsAdmin" name="IsAdmin" value="" />

<script type="text/javascript">
<!--
    Ektron.ready( function() {
        if(($ektron("#txtWidth").attr("value") == "9999") && ($ektron("#txtHeight").attr("value") == "9999")) {
            $ektron("#chkFullScreen").attr("checked","on");
            $ektron("td input#txtWidth").parent().parent().hide();
            $ektron("td input#txtHeight").parent().parent().hide();
        }

        document.forms[0].IsAdmin.value = jsIsAdmin;
        document.forms[0].username.onkeypress = document.forms[0].netscape.onkeypress
        document.forms[0].firstname.onkeypress = document.forms[0].netscape.onkeypress
        if (jsADIntegration) {
            document.forms[0].language.focus()
        } else {
            if (jsIsAdmin) {
                document.forms[0].username.focus();
            } else if (document.forms[0].pwd.hidden == false) {
                document.forms[0].pwd.focus();
            }
        }
    });
    function enableWidthHeight(obj){

        if(obj.checked){
            $ektron("td input#txtWidth").parent().parent().hide();
            $ektron("td input#txtHeight").parent().parent().hide();

            $ektron("#txtWidth").attr("disabled", "disabled");
            $ektron("#txtHeight").attr("disabled", "disabled");
        } else {
            $ektron("td input#txtWidth").parent().parent().show();
            $ektron("td input#txtHeight").parent().parent().show();
            $ektron("#txtWidth")
                .attr("value", "900");
            $ektron("#txtHeight")
                .attr("value", "580");
            document.getElementById("txtWidth").removeAttribute("disabled");
            document.getElementById("txtHeight").removeAttribute("disabled");
        }
    }
//-->
</script>



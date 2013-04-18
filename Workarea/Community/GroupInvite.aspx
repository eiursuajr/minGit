<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupInvite.aspx.cs" Inherits="Community_GroupInvite" %>
<%@ Register TagPrefix="Community" TagName="UserSelectControl" Src="../controls/Community/Components/UserSelectControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Group Invite</title>
    <script src="../java/thickbox.js" type="text/javascript">
</script>
    <link rel="stylesheet" href="../csslib/box.css" />
    <script type="text/javascript">
        function GroupInvite_TabClick(e){
            var el;
            if (e.srcElement){
                el = e.srcElement;
            }
            else{
                el = e.target;
            }
            if ("GroupInviteFriendsTab" == el.id){
                el.className = "Selected";
                var obj = document.getElementById("GroupInviteEmailTab");
                if (obj){
                    obj.className = "NotSelected";
                }

                obj = document.getElementById("GroupInviteFriendsContainer");
                if (obj){
                    obj.className = "GroupInviteContainer_Selected";
                }
                obj = document.getElementById("GroupInviteEmailContainer");
                if (obj){
                    obj.className = "GroupInviteContainer_NotSelected";
                }
            }
            else{
                el.className = "Selected";
                var obj = document.getElementById("GroupInviteFriendsTab");
                if (obj){
                    obj.className = "NotSelected";
                }
                obj = document.getElementById("GroupInviteFriendsContainer");
                if (obj){
                    obj.className = "GroupInviteContainer_NotSelected";
                }
                obj = document.getElementById("GroupInviteEmailContainer");
                if (obj){
                    obj.className = "GroupInviteContainer_Selected";
                }
            }
        }
        
        function GroupInvite_ValidateInvitiations(usrSelId){
            var result = true;
            var emails = "";
            var emailsValidated = false;
            var users = UserSelectCtl_GetSelectUsers(usrSelId);
            
            // check for supplied emails:
            var emailObj = document.getElementById("GroupInviteEmails");
            if (emailObj){
                emails = GroupInvite_CleanEmails(emailObj.value);
                if (emails.length > 0){
                    emailsValidated = GroupInvite_ValidateEmail(emails);
                    if (!emailsValidated){
                        result = false;
                        setTimeout("GroupInvite_ShowError('Invalid Email Address')", 100);
                    }
                }
            }
            
            if (result){
                if ((users.length > 0) || emailsValidated){
                    var hdnObj = document.getElementById("GroupInvite_UserIds");
                    if (hdnObj){
                        hdnObj.value = users;
                    }
                    hdnObj = document.getElementById("GroupInvite_Emails");
                    if (hdnObj){
                        hdnObj.value = emails;
                    }
                }
                else{
                    setTimeout("GroupInvite_ShowError('No Colleagues Selected')", 100);
                    result = false;
                }
            }
            
            return (result);
        }
        
        function GroupInvite_ShowError(msg){
            alert(msg);
        }

		function GroupInvite_CleanEmails(emails){
			var result = emails;
			var delim = ';';
			var idx = 0;
			var emailAdd;
			result = GroupInvite_ReplaceAll(result, '\n', delim); // NewLine
			result = GroupInvite_ReplaceAll(result, '\'', delim);
			result = GroupInvite_ReplaceAll(result, String.fromCharCode(13, 10), delim); // <CR>
			result = GroupInvite_ReplaceAll(result, String.fromCharCode(13), delim); // <CR>
			result = GroupInvite_ReplaceAll(result, String.fromCharCode(10), delim); // <LF>
			result = GroupInvite_ReplaceAll(result, String.fromCharCode(9), delim); // <Tab>
			result = GroupInvite_ReplaceAll(result, '|', delim);
			result = GroupInvite_ReplaceAll(result, '"', delim);
			result = GroupInvite_ReplaceAll(result, ',', delim);
			result = GroupInvite_ReplaceAll(result, ' ', delim);
			
			// Remove repeating-delimiters:
			result = GroupInvite_ReplaceAll(result, delim + delim, delim);
			
			// Remove delimiter at the very beginning, if it exists:
			if ((result.length > 0) && (delim == result.substr(0, 1))){
				result = result.substr(1);
			}

			// Remove any trailing delimiter:
			if (result.length && (delim == result.substr(result.length - 1))){
				result = result.substr(0, result.length - 1);
			}
			
			// Remove duplicates:
			var emailArray = result.split(';');
			result = '';
			for (var idxOuter = 0; idxOuter < emailArray.length; idxOuter++){
				var dupeFound = false;
				for (var idxInner = idxOuter + 1; idxInner < emailArray.length; idxInner++){
					if (emailArray[idxOuter] == emailArray[idxInner]){
						dupeFound = true;
						break;
					}
				}
				if (!dupeFound){
					if (result.length){
						result += delim;
					}
					result += emailArray[idxOuter];
				}
			}
			
			return (result);
		}

		function GroupInvite_ReplaceAll(text, origVal, newVal){
			var result = text;
			var flag = (origVal.length > 0);
			while (flag){
				result = result.replace(origVal, newVal);
				flag = (result.indexOf(origVal) >= 0);
			}
			return (result);
		}
		
		function GroupInvite_ValidateEmail(emails){
			var result = false;
			var idx = 0;
			var emailAdd;
			var emailArray = emails.split(';');
			for (idx=0; idx < emailArray.length; idx++){
				emailAdd = emailArray[idx].replace(' ', '').replace(';', '');
				if (emailAdd.length > 0){
					if (!GroupInvite_IsEmail(emailAdd)){
						result = false;
						break;
					}
					else {
						result = true;
					}
				}
			}
			return (result);
		}
		
		function GroupInvite_IsEmail(strData) {
			var	iCtr,jCtr,sLength,atPos,cs,cTemp;

			varErrMsg='Invalid email address detected!';
			if (GroupInvite_IsEmpty(strData))
				if (isEmail.arguments.length == 1)
					return false;
				else
					return (isEmail.arguments[1] == true);

				   
			if (GroupInvite_IsWhitespace(strData))
				return false;
				    
			iCtr = 1;
			sLength = strData.length;

			iCtr = strData.indexOf('@');
			atPos = iCtr;
				    
			if ( iCtr < 0 )
				return false;
			else 
				iCtr+=2;
			
			iCtr = strData.lastIndexOf('.');
			if ( iCtr < 0 )
				return false;
			else 
				iCtr++;			
					    
			if (iCtr > sLength)
				return false;
					
			cTemp	= '';
			cs		= '';
			for(jCtr = atPos+1; jCtr < strData.length; jCtr++) {
				cTemp =  strData.charAt(jCtr);
				if( (cTemp != '.') && (cTemp != '-') )
					cs += cTemp;
			}
					
			iCtr = strData.lastIndexOf( ' ' );
			if ( iCtr > 0 )
				return false;
			else 
				iCtr++;	
			for(i=0;i<strData.length;i++)
			{
				cTemp =  strData.charAt(i);
				if((cTemp=='?')||(cTemp=='(')||(cTemp==')')||(cTemp=='=')||(cTemp=='+')||(cTemp=='~')||(cTemp=='`')||(cTemp=='!')||(cTemp=='#')||(cTemp=='$')||(cTemp=='%')||(cTemp=='^')||(cTemp=='&')||(cTemp=='*'))
				{
					return false;
				}
			}
			return true;
		}
		
		function GroupInvite_IsEmpty(strData) {
			return ((strData == null) || (strData.length == 0));
		}

		var GroupInvite_Whitespace = ' \t\n\r';
		
		function GroupInvite_IsWhitespace(strData) {
			var iCtr,cTemp;
			if (GroupInvite_IsEmpty(strData))
				return true;
			for (iCtr = 0; iCtr < strData.length; iCtr++) {   
				var cTemp = strData.charAt(iCtr);
				if (GroupInvite_Whitespace.indexOf(cTemp) == -1)
					return false;
			}
			return true;
		}

        function GroupInvite_ShowEmailHelp(flag){
            var obj = document.getElementById("GroupInviteEmailHelpContainer");
            if (obj){
                obj.style.display = ((flag) ? "block" : "none");
            }
        }
        
    </script>
    <style type="text/css">
        .GroupInviteCtl{
            margin: 0.63em;
            border: solid 1px #3B5998;
            background-color: #F7F7F7;
            width: 27.91em;
        }
        
        .GroupInviteCtl h2{
            color: #FFFFFF;
            background-color: #3B5998;
            border-bottom: solid 1px #3B5998;
            margin-top: 0px;
            padding: 0.315em;
            padding-left: 0.63em;
        }
        
        .GroupInviteCtl p{
            margin-left: 0.63em;
        }        
        .GroupInviteCtl label{
            display: block;
            margin-left: 0.63em;
        }        
        .GroupInviteCtl textarea{
            margin-left: 0.63em;
            border: solid 1px #3B5998;
            width: 21.8em;
        }        
        .GroupInviteCtl .GroupInviteOptionalBlock textarea{
            /*width: 32.8em;*/
            width: 94.62%;
        }
        .GroupInviteCtl ul{
            margin-bottom: 0px;
            margin-left: 0.63em;
            margin-bottom: 1px;
            padding: 0px;
        }
        .GroupInviteCtl .GroupInviteTabs li{
            display: inline;
            cursor:pointer;
            margin-right: 0.189em;
            padding: 0.126em;
            padding-left: 1.26em;
            padding-right: 1.26em;
            border-left: solid 1px #3B5998;
            border-top: solid 1px #3B5998;
            border-right: solid 1px #3B5998;
            color: #3B5998;
            background-color: #D8DFEA;
            font-weight: bolder;
        }
        .GroupInviteCtl .GroupInviteTabs li.Selected{
            color: #9595C0;
            background-color: #fff;
        }
        .GroupInviteCtl .GroupInviteContainer_Selected{
            display: block;
            margin-top: 0px;
            margin-bottom: 0px;
            margin-left: 0.63em;
            margin-right: 0.63em;
            border: solid 1px #3B5998;
            background-color: #fff;
            /*padding-bottom: 0.63em;*/
            padding-bottom: 0em;
        }
        .GroupInviteCtl .GroupInviteContainer_NotSelected{
            display: none;
        }
        .GroupInviteCtl .GroupInviteEmailHelpContainer{
            position: absolute;
            top: 1.50em;
            right: 1.26em;
            border: solid 1px #3B5998;
            padding: 0.63em;
            background-color: white;
        }
        .GroupInviteContainer_Selected textarea{
            margin-bottom: 0.63em;
        }
        .GroupInviteOptionalBlock{
            margin-top: 1.26em;
            margin-bottom: 0.63em;
        }
        .GroupInviteResult{
            margin: 1.26em;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="GroupInviteCtl" style="">
            <h2 title="Invite to Group">Invite to Group</h2>
            <asp:Panel ID="GroupInviteStartupUiPanel" runat="server" >
                <p title="Send invitations to users and other people!">Send invitations to users and other people!</p>
                <ul class="GroupInviteTabs" >
                    <li id="GroupInviteFriendsTab" onclick="GroupInvite_TabClick(event)" class="Selected"  title="Users Tab">Users</li><li id="GroupInviteEmailTab" onclick="GroupInvite_TabClick(event)" class="NotSelected" title="Email Tab">Email</li></ul>
                <div id="GroupInviteFriendsContainer" class="GroupInviteContainer_Selected">
                    <asp:Panel ID="Invite_UsrSel_panel" runat="server" >
                        <Community:UserSelectControl id="Invite_UsrSel" FriendsOnly="false" runat="server" />
                    </asp:Panel>
                </div>
                <div id="GroupInviteEmailContainer" class="GroupInviteContainer_NotSelected">
                    <div style="position: relative; top: 0px; left: 0px;">
                        <div id="GroupInviteEmailHelpContainer" class="GroupInviteEmailHelpContainer" style="display:none;">
                            seperate addresses using commas etc
                        </div>
                    </div>
                    <table>
                        <tr>
                            <td><label for="GroupInviteEmails" title="Recipient email addresses">Recipient email addresses</label></td>
                            <td><a title="seperate addresses using commas etc" href="#" onmouseover="GroupInvite_ShowEmailHelp(true)" onmouseout="GroupInvite_ShowEmailHelp(false)" onclick="return false;" ondblclick="return false;" ><%=GetMessage("generic help")%></a></td>
                        </tr>
                        <tr>
                            <td colspan="2"><textarea title="Enter Recipient Email Addresses here" style="overflow: auto;" id="GroupInviteEmails" rows="15"></textarea></td>
                        </tr>
                    </table>
                </div>
                <div class="GroupInviteOptionalBlock">
                    <label for="GroupInviteOptionalText" title="Optional Message">Optional Message:</label>
                    <textarea title="Enter Optional Message here" style="overflow: auto;" id="GroupInviteOptionalText" name="GroupInviteOptionalText" rows="5"><%=GetMessage("default group invite optional text")%></textarea>
                </div>
                <div style="text-align:right; padding: 0.63em;">
                    <asp:Button ToolTip="Send Invitations" ID="SendInviteBtn" runat="server" Text="Send Invitations"  OnClick="SendInviteBtn_Click"/>
                   <%-- <asp:Button ToolTip="Cancel" ID="CanceInviteBtn" runat="server" OnClick="ektb_remove();" Text="Cancel" />--%>
                   <input type="button" runat="server" onclick="self.parent.ektb_remove();" value="Cancel" />
                </div>
                <input type="hidden" id="GroupInvite_UserIds" name="GroupInvite_UserIds" value="" />
                <input type="hidden" id="GroupInvite_Emails" name="GroupInvite_Emails" value="" />
            </asp:Panel>
            <asp:Panel ID="GroupInviteResultUiPanel" Visible="false" runat="server" >
                <div class="GroupInviteResult">
                    <asp:Literal ID="GroupInviteResults" runat="server" />
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>


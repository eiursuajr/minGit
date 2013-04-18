<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InviteFriends.aspx.cs" Inherits="MyWorkspace_InviteFriends" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Invite Friends</title>
    <style type="text/css" >
        .EktInviteCtl{
	        font-family: Verdana,Geneva,Arial,Helvetica,sans-serif;
	        font-size: 12px;
	        /*border: solid 1px rgb(192, 192, 192);*/
	        background-color: white;
	        width: 400px;
	        position: relative;
	        top: 0px;
	        left: 0px;
        }
        .EktInviteCtl_HeaderBar{
	        background-color: #EEEEEE;
	        font-weight: bold;
	        font-size: 22px; 
	        padding: 5px 10px 5px 10px;
        }
        .EktInviteCtl_body{
	        padding: 5px 10px 5px 10px;
        }
        .EktInviteCtl_infoContainer{
	        font-size: 15px;
	        font-weight: bold;
        }
        .EktInviteCtl_addressContainer{
	        margin-top: 10px;
	        margin-left: 10px;
        }
        .EktInviteCtl_addressTitleContainer{
	        display: inline;
        }
        .EktInviteCtl_addressHelpLink
        {
	        position: relative;
	        top: 0px;
	        left: 50px;
	        color: Blue;
	        text-decoration: undeline;
	        cursor: pointer;
        }
        .EktInviteCtl_addressHelpContainer
        {
	        position: absolute;
	        top: -35px;
	        left: 95px;
	        border: solid 1px #EEEEEE;
	        background-color: #FFFFE7;
	        width: 50%;
	        padding: 2px;
        }
        .EktInviteCtl_messageContainer{
	        margin-top: 10px;
	        margin-left: 10px;
        }
        .EktInviteCtl_messageTitleContainer{
	        display: block;
        }
        .EktInviteCtl_sendButtonContainer{
	        margin-top: 10px;
	        text-align: center;
        }
        .EktInviteCtl textarea {font-family:Verdana,Geneva,Arial,Helvetica,sans-serif; font-size: .92em; padding: .25em;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="EktInviteCtl">
                <asp:Panel ID="InvitePanel" runat="server">
                    <div class="EktInviteCtl_body">
                        <div class="EktInviteCtl_addressContainer">
                            <span title="Enter Recipient email addresses in the Textbox below" class="EktInviteCtl_addressTitleContainer">Recipient email addresses: </span>
                            <span class="EktInviteCtl_addressHelpLink"><a onmouseout="InviteShowHelp(false);"
                                onmouseover="InviteShowHelp(true);" href="#">Help</a> </span>
                            <div id="inv1_inviteEmailAddressHelp" style="display: none;" class="EktInviteCtl_addressHelpContainer">
                                Seperate addresses using either commas, spaces, semi-colons, or new-lines.
                            </div>
                            <textarea title="Enter Recipient email addresses here" rows="5" cols="30" name="inv1_inviteEmailAddress" id="inv1_inviteEmailAddress"
                                onfocus="javascript:InitInviteTextArea(this);"></textarea>
                        </div>
                        <div class="EktInviteCtl_messageContainer">
                            <span title="Enter an Optional message in the Textbox below" class="EktInviteCtl_messageTitleContainer">Optional message:</span>
                            <textarea title="Enter an Optional message here" rows="5" cols="30" name="inv1_inviteMessage" id="inv1_inviteMessage">Hello, I am sending you this email because I would like you to join me at this website.</textarea>
                        </div>
                        <div class="EktInviteCtl_sendButtonContainer">
                            <input title="Send Invitations" type="button" value="Send Invitations" name="inv1_inviteSendBtn" id="inv1_inviteSendBtn"
                                onclick="javascript:return(SendInviteMsg(this));" />
                        </div>
                    </div>
                    <input type="hidden" name="inv1_inviteHdn" id="inv1_inviteHdn" value="" />
                </asp:Panel>
                <asp:Panel ID="InivitedPanel" runat="server">
                    <div class="EktInviteCtl_body">
                        <div class="EktInviteCtl_infoContainer">
                            <asp:Literal ID="infoMsgLit" runat="server" ></asp:Literal>
                        </div>
                        <ul>
                            <asp:Literal ID="invitedLit" runat="server" ></asp:Literal>
                        </ul>
                    </div>
                </asp:Panel>
                
                <asp:Literal ID="lit_faultyEmailAddrMsg" runat="server" ></asp:Literal>
                <asp:Literal ID="lit_longEmailAddrMsg" runat="server" ></asp:Literal>
                
                <script type="text/javascript" language="javascript">
				<!--
				function SendInviteMsg(btnObj){
					var uniqueId = btnObj.id.substr(0, btnObj.id.indexOf('_'));
					var emailObj = document.getElementById(uniqueId + '_inviteEmailAddress');
					if (emailObj.inviteInitialized && (emailObj.value.length > 0)){
						var email = InviteCleanEmails(emailObj.value);
						if (email.length < 100){
						    if (InviteValidateEmail(email)) {
							    var hdnObj = document.getElementById(uniqueId + '_inviteHdn');
							    hdnObj.value = '1'; // this validates action for server...
							    var act = document.forms[0].action;
							    if (act){
								    if (act.indexOf('?') > 0){
									    act += '&invid=' + uniqueId;
								    }
								    else{
									    act += '?invid=' + uniqueId;
								    }
								    emailObj.value = email;
								    document.forms[0].action = act;
								    document.forms[0].submit();
							    }
							    return (true);
						    }
						    else{ 
							    alert(faultyEmailAddrMsg);
							    return (false);
						    }
						}
					    else{ 
						    alert(longEmailAddrMsg);
						    return (false);
					    }
					}
					else {
						alert('Error: you need to enter the recipients email address!');
						return (false);
					}
				}
				
				function InitInviteTextArea(taObj){
					if (!taObj.inviteInitialized){
						taObj.inviteInitialized = true;
						taObj.value = '';
					}
				}

				function InviteCleanEmails(emails){
					var result = emails;
					var delim = ';';
					var idx = 0;
					var emailAdd;
					result = InviteReplaceAll(result, '\n', delim); // NewLine
					result = InviteReplaceAll(result, '\'', delim);
					result = InviteReplaceAll(result, String.fromCharCode(13, 10), delim); // <CR>
					result = InviteReplaceAll(result, String.fromCharCode(13), delim); // <CR>
					result = InviteReplaceAll(result, String.fromCharCode(10), delim); // <LF>
					result = InviteReplaceAll(result, String.fromCharCode(9), delim); // <Tab>
					result = InviteReplaceAll(result, '|', delim);
					result = InviteReplaceAll(result, '"', delim);
					result = InviteReplaceAll(result, ',', delim);
					result = InviteReplaceAll(result, ' ', delim);
					
					// Remove repeating-delimiters:
					result = InviteReplaceAll(result, delim + delim, delim);
					
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

				function InviteReplaceAll(text, origVal, newVal){
					var result = text;
					var flag = (origVal.length > 0);
					while (flag){
						result = result.replace(origVal, newVal);
						flag = (result.indexOf(origVal) >= 0);
					}
					return (result);
				}
				
				function InviteValidateEmail(emails){
					var result = false;
					var idx = 0;
					var emailAdd;
					var emailArray = emails.split(';');
					for (idx=0; idx < emailArray.length; idx++){
						emailAdd = emailArray[idx].replace(' ', '').replace(';', '');
						if (emailAdd.length > 0){
							if (!InviteIsEmail(emailAdd)){
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
				
				function InviteIsEmail(strData) {
					var	iCtr,jCtr,sLength,atPos,cs,cTemp;

					varErrMsg='Invalid email address detected!';
					if (InviteIsEmpty(strData))
						if (isEmail.arguments.length == 1)
							return false;
						else
							return (isEmail.arguments[1] == true);

						   
					if (InviteIsWhitespace(strData))
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
				
				function InviteIsEmpty(strData) {
					return ((strData == null) || (strData.length == 0));
				}

				var InviteWhitespace = ' \t\n\r';
				
				function InviteIsWhitespace(strData) {
					var iCtr,cTemp;
					if (InviteIsEmpty(strData))
						return true;
					for (iCtr = 0; iCtr < strData.length; iCtr++) {   
						var cTemp = strData.charAt(iCtr);
						if (InviteWhitespace.indexOf(cTemp) == -1)
							return false;
					}
					return true;
				}
				
				function InviteClearEmailArea(){
					var taObj = document.getElementById('inv1_inviteEmailAddress');
					if (taObj){
						taObj.value = '';
					}
				}

				function InviteShowHelp(showFlag){
					var obj = document.getElementById('inv1_inviteEmailAddressHelp');
					if (obj){
						obj.style.display = (showFlag) ? '' : 'none';
					}
				}
				
				// Clear right after page render:
				setTimeout('InviteClearEmailArea();', 100);

				//-->
				</script>

            </div>
            <!--
            DisplayXslt="xslt/invite.xsl" 
            DisplayXslt="Basic Invite" 
        -->
        </div>
    </form>
</body>
</html>


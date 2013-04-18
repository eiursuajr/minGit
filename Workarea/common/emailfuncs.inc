<%@ Import Namespace="Ektron.Cms" %>
<%@ Import Namespace="Ektron.Cms.Common" %>
<script language="vb" runat="server">
Dim messagesEmail, gtMessEmail, gtMsgObjEmail, ErrorStringEmail as Object
Dim senderEmailAddressInitialized, senderEmailAddressStr as Object
Dim MsgHelper_emailfuncs as EkMessageHelper
</script>
<%	
' Notes:
' 
' This file was created to support Ad-Hoc Email with the CMS300 Product.
'
' To avoid namespace collisions, all functions have then word "Email" either
' contained in their proper name, or appended onto the end of their names.
'
MsgHelper_emailfuncs=(New CommonApi).EkMsgRef

messagesEmail = "generic Username, generic Cancel, generic to label, generic from label, " _
	& "generic subject label, send email button text, email error: No users selected to receive email, " _
	& "alt send email to"
'gtMsgObjEmail = AppUI.EkMessageRef()
'gtMessEmail = gtMsgObjEmail.GetMsgsByTitleTwo(messagesEmail, CurrentUserId,ErrorStringEmail)
senderEmailAddressInitialized = False
senderEmailAddressStr = ""
%>

<script language="javascript">
	var g_emailChecked = false;

	function PopUpWindow_Email (url, hWind, nWidth, nHeight, nScroll, nResize) {
		var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
		var popupwin = window.open(url, hWind, cToolBar);
		return popupwin;
	}

	function IsBrowserIE_Email() {
		// document.all is an IE only property
		return (document.all ? true : false);
	}

	function ToggleEmailCheckboxes() {
		var idx, prefix, name;
		
		g_emailChecked = !g_emailChecked;
		for(idx = 0; idx < document.forms.selections.elements.length; idx++ ) {
			if (document.forms.selections.elements[idx].type == "checkbox") {
				name = document.forms.selections.elements[idx].name;
				if (name.indexOf("emailcheckbox_") != -1) {
					document.forms.selections.elements[idx].checked = g_emailChecked;
				}
			}
		}							
	}			

	// Open email window/layer ontop of current window:
	function LoadEmailChildPage(userGrpId) {
		var pageObj, frameObj
		
		if (IsBrowserIE_Email()) {
			// Configure the email window to be visible:
			frameObj = document.getElementById("EmailChildPage");
			if ((typeof(frameObj) == "object") && (frameObj != null)) {
				frameObj.src = "blankredirect.aspx?email.aspx?" + userGrpId;
				pageObj = document.getElementById("EmailFrameContainer");
				pageObj.style.display = "";
				pageObj.style.width = "85%";
				pageObj.style.height = "90%";

				// Ensure that the transparent layer completely covers the parent window:
				pageObj = document.getElementById("EmailActiveOverlay");
				pageObj.style.display = "";
				pageObj.style.width = "100%";
				pageObj.style.height = "100%";
			}
		}
		else {
			// Using Netscape; cant use transparencies & eWebEditPro preperly 
			// - so launch in a seperate pop-up window:
			PopUpWindow_Email("blankredirect.aspx?email.aspx?" + userGrpId,"CMSEmail",490,500,1,1);
			CloseEmailChildPage();
		}
	}

	// Open email window/layer ontop of current window (extended version, 
	// iterates through the controls to determine which usuer/group to add):
	function LoadEmailChildPageEx() {
		var idx, name, prefix, userGrpId, pageObj, qtyElements, frameObj, haveTargets=false;

		// build user-group ID string, based on current check-box status:
		userGrpId = "";
		prefix = "emailcheckbox_";
		qtyElements = document.forms.selections.elements.length
		for(idx = 0; idx < qtyElements; idx++ ) {
			if (document.forms.selections.elements[idx].type == "checkbox"){
				name = document.forms.selections.elements[idx].name;
				if ((name.length > prefix.length)
					&& (0 == name.indexOf(prefix))
					&& (document.forms.selections.elements[idx].checked == true)) {
					userGrpId = userGrpId + name.substring(prefix.length) + ",";
					haveTargets = true;
				}
			}
		}			
		if (haveTargets) {
			// Build either a user array or a group array:
			if (typeof(document.forms.selections.groupMarker) == "undefined")
				userGrpId = "userarray=" + escape(userGrpId.substring(0, userGrpId.length - 1));
			else
				userGrpId = "grouparray=" + escape(userGrpId.substring(0, userGrpId.length - 1));
			if (IsBrowserIE_Email()) {
				frameObj = document.getElementById("EmailChildPage");
				if ((typeof(frameObj) == "object") && (frameObj != null)) {
					frameObj.src = "blankredirect.aspx?email.aspx?" + userGrpId;
					pageObj = document.getElementById("EmailFrameContainer");
					pageObj.style.display = "";
					pageObj.style.width = "85%";
					pageObj.style.height = "90%";

					pageObj = document.getElementById("EmailActiveOverlay");
					pageObj.style.display = "";
					pageObj.style.width = "100%";
					pageObj.style.height = "100%";
				}
			}
			else {
				PopUpWindow_Email("blankredirect.aspx?email.aspx?" + userGrpId,"CMSEmail",490,500,1,1);
				CloseEmailChildPage();
			}
		}
		else {
			alert("<%=(MsgHelper_emailfuncs.GetMessage("email error: No users selected to receive email"))%>");
		}
	}
	
	
	// Close email window/layer:
	function CloseEmailChildPage() {
		var pageObj = document.getElementById("EmailFrameContainer");

		// Configure the email window to be invisible:
		pageObj.style.display = "none";
		pageObj.style.width = "1px";
		pageObj.style.height = "1px";

		// Ensure that the transparent layer does not cover any of the parent window:
		pageObj = document.getElementById("EmailActiveOverlay");
		pageObj.style.display = "none";
		pageObj.style.width = "1px";
		pageObj.style.height = "1px";
	}
</script>

<script language="vb" runat="server">

' ---------------------------------------------------------------------------
' Create an area for the email window to open (draws ontop of a transparent layer, that covers the 
' parent window - to ensure that the user doesn't accidentally click on the parents' controls):
Sub MakeEmailArea () 
	Response.Write("<div allowtransparency=""true"" id=""EmailActiveOverlay"" style=""POSITION: absolute; TOP: 0px; LEFT: 0px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none; Z-INDEX: 1; background-color: transparent; "">")
	Response.Write("<iframe src=""blank.htm"" allowtransparency=""true"" id=""EmailOverlayChildPage"" name=""EmailOverlayChildPage"" frameborder=""no"" marginheight=""0"" marginwidth=""0"" width=""100%"" height=""100%"" scrolling=""no"" style=""background-color: transparent; background: transparent; FILTER: chroma(color=#FFFFFF)"">")
	Response.Write("</iframe>")
	Response.Write("</div>")
	Response.Write("<div id=""EmailFrameContainer"" style=""POSITION: absolute; TOP: 48px; LEFT: 55px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none; Z-INDEX: 2; Background-color: white; Border-Style: Outset"">")
	Response.Write("<iframe src=""blank.htm"" id=""EmailChildPage"" name=""EmailChildPage"" frameborder=""yes"" marginheight=""0"" marginwidth=""0"" width=""100%"" height=""100%"" scrolling=""auto"">")
	Response.Write("</iframe>")
	Response.Write("</div>")

End Sub

' ---------------------------------------------------------------------------
Function DoesKeyExist_Email(ByRef collectionObject as Object, ByRef keyName as Object) as Object
	dim dummy as Object
	On Error Resume Next	' Used to determine condition, only affects this procedure 
							' (reverts back to previous method when out of scope).
	Err.Clear
	dummy = collectionObject.Item(keyName)
	If (Err.number = 0) Then
		DoesKeyExist_Email = True
	Else
		DoesKeyExist_Email = False
	End If
End Function

' ---------------------------------------------------------------------------
' Safe Collection Reading (returns empty string if key-item doesn't exist):
Function SafeColRead_Email(ByRef collectionObject as Object, ByRef keyName as Object) as Object
	If (DoesKeyExist_Email(collectionObject, keyName)) Then
		SafeColRead_Email = collectionObject(keyName)
	Else
		SafeColRead_Email = ""
	End If
End Function

' ---------------------------------------------------------------------------
' Creates the notes (subject) section of the hyperlink and query string:
Function MakeNotes_Email(ByRef notesName as Object, ByRef notesText as Object) as Object
	MakeNotes_Email = "&notes=" & HttpContext.Current.Server.URLEncode(notesName & ": " & Replace(notesText,"'","&apos;")) 
End Function

' ---------------------------------------------------------------------------
' Creates the notes (subject) section of the hyperlink and query string, using supplied collection:
Function MakeNotesFmCol_Email(ByRef notesName as Object, ByRef collectionObject as Object, ByRef keyName as Object) as Object
	if (DoesKeyExist_Email(collectionObject, keyName)) then
		MakeNotesFmCol_Email = MakeNotes_Email(notesName, collectionObject(keyName))
	else
		MakeNotesFmCol_Email = ""
	end if
End Function

' ---------------------------------------------------------------------------
' Creates the content-identification section of the hyperlink and query 
' string, attempt to determine content language from collection object):
Function MakeContentId_Email(ByRef collectionObject as Object, ByRef keyName as Object) as Object
	dim retStr as String
	if (DoesKeyExist_Email(collectionObject, keyName)) then
		retStr = "&contentid=" & collectionObject(keyName)
		if (DoesKeyExist_Email(collectionObject, "ContentLanguage")) then
			retStr = retStr & "&emailclangid=" & collectionObject("ContentLanguage")
		end if
		MakeContentId_Email = retStr
	else
		MakeContentId_Email = ""
	end if
End Function

' ---------------------------------------------------------------------------
' Builds the email graphic image for the hyperlink, if logged in 
' user has a valid email address, otherwise no graphic is added:
Function MakeEmailGraphic() as Object
	' Option: Add code to test for users email address in db, if it doesn't 
	' exist then do not create graphic...
	' Downside: this will cause a performance hit that may be unacceptable 
	' at sites with many users/items to test/display.
	if (IsLoggedInUsersEmailValid()) then
		MakeEmailGraphic = "<img src=""" & AppPath & "images/ui/icons/email.png"" border=""0"" align=""absbottom"">"
	else
		MakeEmailGraphic = ""
	end if
End Function

' ---------------------------------------------------------------------------
' Builds a hyperlink to launch the email window, user-email-target is task target:
Function MakeUserTaskEmailLink(ByRef taskObj as Object) as Object
	if (IsLoggedInUsersEmailValid()) then
		MakeUserTaskEmailLink = "<a href=""#""" _
			& "onclick=""LoadEmailChildPage('userid=" _
				& taskObj.AssignedToUserID _
				& MakeNotes_Email("Task", taskObj.TaskTitle) _
				& "')""" _
				& "title='" _
				& MsgHelper_emailfuncs.GetMessage("alt send email to") _
				& " """ _
				& Replace(taskObj.AssignedToUser, "'", "`") _
				& """" _
				& "'>" _
				& taskObj.AssignedToUser _
				& "&nbsp;" _
				& MakeEmailGraphic() _
				& "</a>"
	else
		MakeUserTaskEmailLink = taskObj.AssignedToUser
	end if
End Function 

' ---------------------------------------------------------------------------
' Builds a hyperlink to launch the email window, group-email-target is task target:
Function MakeUserGroupTaskEmailLink(ByRef taskObj as Object) as Object
	if (IsLoggedInUsersEmailValid()) then
		MakeUserGroupTaskEmailLink = "<a href=""#""" _
			& "onclick=""LoadEmailChildPage('groupid=" _
				& taskObj.AssignToUserGroupID _
				& MakeNotes_Email("Task", taskObj.TaskTitle) _
				& "')""" _
				& "title='" _
				& MsgHelper_emailfuncs.GetMessage("alt send email to") _
				& " """ _
				& Replace(taskObj.AssignedToUserGroup, "'", "`") _
				& """" _
				& "'>" _
				& taskObj.AssignedToUserGroup _
				& "&nbsp;" _
				& MakeEmailGraphic() _
				& "</a>"
	else
		MakeUserGroupTaskEmailLink = taskObj.AssignedToUserGroup
	end if
End Function 

' ---------------------------------------------------------------------------
' Builds a hyperlink to launch the email window, for task with email-target to task author:
Function MakeByUserTaskEmailLink(ByRef taskObj as Object) as Object
	if (IsLoggedInUsersEmailValid()) then
		MakeByUserTaskEmailLink = "<a href=""#""" _
			& "onclick=""LoadEmailChildPage('userid=" _
				& taskObj.AssignedByUserID _
				& MakeNotes_Email("Task", taskObj.TaskTitle) _
				& "')""" _
				& "title='" _
				& MsgHelper_emailfuncs.GetMessage("alt send email to") _
				& " """ _
				& Replace(taskObj.AssignedByUser, "'", "`") _
				& """" _
				& "'>" _
				& taskObj.AssignedByUser _
				& "&nbsp;" _
				& MakeEmailGraphic() _
				& "</a>"
	else
		MakeByUserTaskEmailLink = taskObj.AssignedByUser
	end if
End Function 

' ---------------------------------------------------------------------------
' Builds a hyperlink to launch the email window, for content item, 
' with email-target to the specified collection userKeyName:
Function MakeUserContentEmailLinkKey(ByRef colObj as Object, ByRef userKeyName as Object) as Object
	if (DoesKeyExist_Email(colObj, "UserID")) then
		if (IsLoggedInUsersEmailValid()) then
			MakeUserContentEmailLinkKey = "<a href=""#""" _
				& "onclick=""LoadEmailChildPage('userid=" _
					& colObj("UserID") _
					& MakeNotesFmCol_Email("Content", colObj, "ContentTitle") _
					& MakeContentId_Email(colObj, "ContentID") _
					& "')""" _
					& "title='" _
					& MsgHelper_emailfuncs.GetMessage("alt send email to") _
					& " """ _
					& Replace(SafeColRead_Email(colObj, userKeyName), "'", "`") _ 
					& """" _
					& "'>" _
					& SafeColRead_Email(colObj, userKeyName) _
					& "&nbsp;" _
					& MakeEmailGraphic() _
					& "</a>"
		else
			MakeUserContentEmailLinkKey = SafeColRead_Email(colObj, userKeyName)
		end if
	else
		MakeUserContentEmailLinkKey = ""
	end if
End Function 

' ---------------------------------------------------------------------------
' Builds a hyperlink to launch the email window, for content item, 
' with email-target to the editors' name (can display name in forward or
' reverse order, can hide author name from display - where it will only be shown
' for the mouse-hover alt-text/tool-tips):
Function MakeUserEditorContentEmailLink(ByRef colObj as Object, ByVal reverseName as Object, ByVal hideText as Object) as Object
	dim nameStr, shownText as Object
	if (DoesKeyExist_Email(colObj, "UserID")) then
		if (reverseName) then
			nameStr = Replace(SafeColRead_Email(colObj, "EditorLname") & ", " & SafeColRead_Email(colObj, "EditorFname"), "'", "`")
		else
			nameStr = Replace(SafeColRead_Email(colObj, "EditorFname") & " " & SafeColRead_Email(colObj, "EditorLname"), "'", "`")
		end if
		if (hideText) then
			shownText = ""
		else
			shownText = nameStr & "&nbsp;"
		end if
		
		if (IsLoggedInUsersEmailValid()) then
			MakeUserEditorContentEmailLink = "<a href=""#""" _
				& "onclick=""LoadEmailChildPage('userid=" _
					& colObj("UserID") _
					& MakeNotesFmCol_Email("Content", colObj, "ContentTitle") _
					& MakeContentId_Email(colObj, "ContentID") _
					& "')""" _
					& "title='" _
					& MsgHelper_emailfuncs.GetMessage("alt send email to") _
					& " """ _
					& nameStr _ 
					& """" _
					& "'>" _
					& shownText _
					& MakeEmailGraphic() _
					& "</a>"
		else
			MakeUserEditorContentEmailLink = shownText
		end if
	else
		MakeUserEditorContentEmailLink = ""
	end if
End Function 

' ---------------------------------------------------------------------------
' Get senders email address; Only hits database once. ErrorString returned 
' empty-string, unless error occurs:
Function GetSendersEmailAddress(ByRef ErrorString as String) as Object
	Dim userObj_Email, cUserInfo_Email as Object

	ErrorString = ""
	if (senderEmailAddressInitialized) then
		GetSendersEmailAddress = senderEmailAddressStr
	else
		userObj_Email = AppUI.EkUserRef()
		cUserInfo_Email = userObj_Email.GetUserEmailInfoByID(currentUserID)
		If (ErrorString = "") Then
			If (cUserInfo_Email.count) Then
				senderEmailAddressStr = SafeColRead_Email(cUserInfo_Email, "EmailAddr1")
				senderEmailAddressInitialized = True
				GetSendersEmailAddress = senderEmailAddressStr
			End If
		End IF
		cUserInfo_Email = nothing
		userObj_Email = nothing
	end if
	Return Nothing
End Function

' ---------------------------------------------------------------------------
' Returns true only if the logged in user has a valid (non-empty string) 
' email address, otherwise false:
Function IsLoggedInUsersEmailValid() as Object
	Dim addressStr, ErrorString as Object
	ErrorString = ""
	IsLoggedInUsersEmailValid = False
	addressStr = GetSendersEmailAddress(ErrorString)
	if ((Len(ErrorString) = 0) And (Len(addressStr) > 0)) then
		IsLoggedInUsersEmailValid = True
	end if
End Function

' ---------------------------------------------------------------------------

' Cleanup:
'gtMsgObjEmail = nothing
</script>

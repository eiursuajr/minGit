using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;

public class EmailHelper
{

    private EkMessageHelper gtMessEmail;
    private bool  senderEmailAddressInitialized=false ;
    private object senderEmailAddressStr;
    private string AppImgPath;
    private string AppPath;

    public EmailHelper()
    {
        CommonApi objCommonApi = new CommonApi();
        gtMessEmail = objCommonApi.EkMsgRef;
        AppImgPath = objCommonApi.AppImgPath;
        AppPath = objCommonApi.AppPath;
    }
    // ---------------------------------------------------------------------------
    // Create an area for the email window to open (draws ontop of a transparent layer, that covers the
    // parent window - to ensure that the user doesn't accidentally click on the parents' controls):
    public string MakeEmailArea()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (Utilities.IsBrowserIE())
        {
            result.Append("<div allowtransparency=\"true\" id=\"EmailActiveOverlay\" style=\"position: absolute; top: 0px; left: 0px; width: 1px; height: 1px; display: none; z-index: 1; background-color: transparent; \">");
            result.Append("<iframe allowtransparency=\"true\" id=\"EmailOverlayChildPage\" name=\"EmailOverlayChildPage\" frameborder=\"no\" marginheight=\"0\" marginwidth=\"0\" width=\"100%\" height=\"100%\" scrolling=\"no\" style=\"background-color: transparent; background: transparent; FILTER: chroma(color=#FFFFFF)\">");
            result.Append("</iframe>");
            result.Append("</div>");
        }
        result.Append("<div id=\"EmailFrameContainer\" style=\"position: absolute; top: 48px; left: 55px; width: 1px; height: 1px; display: none; z-index: 6; Background-color: white; Border-Style: Outset\">");
        result.Append("<iframe id=\"EmailChildPage\" name=\"EmailChildPage\" frameborder=\"yes\" marginheight=\"0\" marginwidth=\"0\" width=\"100%\" height=\"100%\" scrolling=\"auto\">");
        result.Append("</iframe>");
        result.Append("</div>");
        return (result.ToString());
    }

    // ---------------------------------------------------------------------------
    public bool DoesKeyExist_Email(Collection collectionObject, string keyName)
    {
        bool returnValue;
        object dummy;
        //On Error Resume Next VBConversions Warning: On Error Resume Next not supported in C# // Used to determine condition, only affects this procedure
        // (reverts back to previous method when out of scope).
        Information.Err().Clear();
        dummy = collectionObject[keyName];
        if (Information.Err().Number == 0)
        {
            returnValue = true;
        }
        else
        {
            returnValue = false;
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Safe Collection Reading (returns empty string if key-item doesn't exist):
    public string SafeColRead_Email(ref Collection collectionObject,  string keyName)
    {
        string returnValue;
        if (DoesKeyExist_Email(collectionObject, keyName))
        {
            returnValue = (string)collectionObject[keyName];
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Creates the notes (subject) section of the hyperlink and query string:
    public string MakeNotes_Email(string notesName, string notesText)
    {
        string returnValue;
        returnValue = "&notes=" + EkFunctions.UrlEncode(notesName + ": " + notesText.Replace("\'", "&apos;"));
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Creates the notes (subject) section of the hyperlink and query string, using supplied collection:
    public string MakeNotesFmCol_Email( string notesName, ref Collection collectionObject,  string keyName)
    {
        string returnValue;
        if (DoesKeyExist_Email(collectionObject, keyName))
        {
            returnValue = MakeNotes_Email(notesName, (string)collectionObject[keyName]);
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Creates the content-identification section of the hyperlink and query
    // string, attempt to determine content language from collection object):
    public string MakeContentId_Email(ref Collection collectionObject,  string keyName)
    {
        string returnValue;
        string retStr;
        if (DoesKeyExist_Email(collectionObject, keyName))
        {
            retStr = (string)("&contentid=" + collectionObject[keyName]);
            if (DoesKeyExist_Email(collectionObject, "ContentLanguage"))
            {
                retStr = retStr + "&emailclangid=" + collectionObject["ContentLanguage"];
            }
            returnValue = retStr;
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Builds the email graphic image for the hyperlink, if logged in
    // user has a valid email address, otherwise no graphic is added:
    public string MakeEmailGraphic()
    {
        string returnValue;
        // Option: Add code to test for users email address in db, if it doesn't
        // exist then do not create graphic...
        // Downside: this will cause a performance hit that may be unacceptable
        // at sites with many users/items to test/display.
        if (IsLoggedInUsersEmailValid())
        {
            returnValue = "<img src=\"" + AppPath + "images/ui/icons/email.png\" border=\"0\" align=\"absbottom\">";
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Builds a hyperlink to launch the email window, user-email-target is task target:
    public string MakeUserTaskEmailLink(EkTask taskObj, bool blnShowFullName)
    {
        string returnValue;
        UserAPI objUserAPI = new UserAPI();
        UserData objUserData = new UserData();
        string strUserFullName;

        objUserData = objUserAPI.GetActiveUserById(taskObj.AssignedToUserID, false);
        strUserFullName = objUserData.FirstName + " " + objUserData.LastName;

        if (blnShowFullName == false)
        {
            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + taskObj.AssignedToUserID + MakeNotes_Email("Task", (string)taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + strUserFullName.Replace("\'", "`") + "\"" + "\'>" + taskObj.AssignedToUser + "&nbsp;" + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = taskObj.AssignedToUser;
            }
        }
        else
        {
            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + taskObj.AssignedToUserID + MakeNotes_Email("Task", (string)taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + strUserFullName.Replace("\'", "`") + "\"" + "\'>" + strUserFullName + " (" + taskObj.AssignedToUser + ")" + "&nbsp;" + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = strUserFullName + " (" + taskObj.AssignedToUser + ")";
            }
        }
        return returnValue;
    }

    public string MakeUserTaskEmailLink2(Ektron.Cms.Content.EkTask taskObj, bool blnShowFullName)
    {
        string returnValue;
        UserAPI objUserAPI = new UserAPI();
        UserData objUserData = new UserData();
        string strUserFullName;

        objUserData = objUserAPI.GetActiveUserById(taskObj.AssignedToUserID, false);
        strUserFullName = objUserData.FirstName + " " + objUserData.LastName;

        if (blnShowFullName == false)
        {
            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + taskObj.AssignedToUserID + MakeNotes_Email("Task", taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + strUserFullName.Replace("\'", "`") + "\"" + "\'>" + taskObj.AssignedToUser + "&nbsp;" + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = taskObj.AssignedToUser;
            }
        }
        else
        {
            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + taskObj.AssignedToUserID + MakeNotes_Email("Task", taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + strUserFullName.Replace("\'", "`") + "\"" + "\'>" + strUserFullName + " (" + taskObj.AssignedToUser + ")" + "&nbsp;" + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = strUserFullName + " (" + taskObj.AssignedToUser + ")";
            }
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Builds a hyperlink to launch the email window, group-email-target is task target:
    public string MakeUserGroupTaskEmailLink(EkTask taskObj)
    {
        string returnValue;
        if (IsLoggedInUsersEmailValid())
        {
            returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'groupid=" + taskObj.AssignToUserGroupID + MakeNotes_Email("Task", (string)taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + Strings.Replace((string)taskObj.AssignedToUserGroup, "\'", "`", 1, -1, 0) + "\"" + "\'>" + taskObj.AssignedToUserGroup + "&nbsp;" + MakeEmailGraphic() + "</a>";
        }
        else
        {
            returnValue = taskObj.AssignedToUserGroup;
        }
        return returnValue;
    }

    public string MakeUserGroupTaskEmailLink2(EkTask taskObj)
    {
        string returnValue;
        if (IsLoggedInUsersEmailValid())
        {
            returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'groupid=" + taskObj.AssignToUserGroupID + MakeNotes_Email("Task", taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + taskObj.AssignedToUserGroup.Replace("\'", "`") + "\"" + "\'>" + taskObj.AssignedToUserGroup + "&nbsp;" + MakeEmailGraphic() + "</a>";
        }
        else
        {
            returnValue = taskObj.AssignedToUserGroup;
        }
        return returnValue;
    }


    // ---------------------------------------------------------------------------
    // Builds a hyperlink to launch the email window, for task with email-target to task author:
    public string MakeByUserTaskEmailLink(EkTask taskObj, bool blnShowFullName)
    {
        string returnValue;
        UserAPI objUserAPI = new UserAPI();
        UserData objUserData = new UserData();
        string strUserFullName;

        objUserData = objUserAPI.GetActiveUserById(Convert.ToInt64 (taskObj.AssignedByUserID), false);
        strUserFullName = objUserData.FirstName + " " + objUserData.LastName;

        if (blnShowFullName == false)
        {
            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + taskObj.AssignedByUserID + MakeNotes_Email("Task", (string)taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + strUserFullName.Replace("\'", "`") + "\"" + "\'>" + taskObj.AssignedByUser + "&nbsp;" + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = taskObj.AssignedByUser;
            }
        }
        else
        {
            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + taskObj.AssignedByUserID + MakeNotes_Email("Task", (string)taskObj.TaskTitle) + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + strUserFullName.Replace("\'", "`") + "\"" + "\'>" + strUserFullName + " (" + taskObj.AssignedByUser + ")" + "&nbsp;" + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = strUserFullName + " (" + taskObj.AssignedByUser + ")";
            }
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Builds a hyperlink to launch the email window, for content item,
    // with email-target to the specified collection userKeyName:
    public string MakeUserContentEmailLinkKey(ref Collection colObj, ref string userKeyName)
    {
        string returnValue;
        if (DoesKeyExist_Email(colObj, "UserID"))
        {
            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + colObj["UserID"] + MakeNotesFmCol_Email("Content", ref colObj, "ContentTitle") + MakeContentId_Email(ref colObj, "ContentID") + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + SafeColRead_Email(ref colObj,  userKeyName).Replace("\'", "`") + "\"" + "\'>" + SafeColRead_Email(ref colObj,  userKeyName) + "&nbsp;" + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = SafeColRead_Email(ref colObj,  userKeyName);
            }
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Builds a hyperlink to launch the email window, for content item,
    // with email-target to the editors' name (can display name in forward or
    // reverse order, can hide author name from display - where it will only be shown
    // for the mouse-hover alt-text/tool-tips):
    public string MakeUserEditorContentEmailLink(ref Collection colObj, bool reverseName, bool hideText)
    {
        string returnValue;
        string nameStr;
        string shownText;
        if (DoesKeyExist_Email(colObj, "UserID"))
        {
            if (reverseName)
            {
                nameStr = SafeColRead_Email(ref colObj, "EditorLname") + ", " + SafeColRead_Email(ref colObj, "EditorFname").Replace("\'", "`");
            }
            else
            {
                nameStr = SafeColRead_Email(ref colObj, "EditorFname") + " " + SafeColRead_Email(ref colObj, "EditorLname").Replace("\'", "`");
            }
            if (hideText)
            {
                shownText = "";
            }
            else
            {
                shownText = nameStr + "&nbsp;";
            }

            if (IsLoggedInUsersEmailValid())
            {
                returnValue = "<a href=\"#\"" + "onclick=\"LoadEmailChildPage(\'userid=" + colObj["UserID"] + MakeNotesFmCol_Email("Content", ref colObj, "ContentTitle") + MakeContentId_Email(ref colObj, "ContentID") + "\')\"" + " title=\'" + gtMessEmail.GetMessage("alt send email to") + " \"" + nameStr + "\"" + "\'>" + shownText + MakeEmailGraphic() + "</a>";
            }
            else
            {
                returnValue = shownText;
            }
        }
        else
        {
            returnValue = "";
        }
        return returnValue;
    }

    // ---------------------------------------------------------------------------
    // Get senders email address; Only hits database once. ErrorString returned
    // empty-string, unless error occurs:
    public string GetSendersEmailAddress()
    {
        string Ret = "";
        Ektron.Cms.User.EkUser userObj_Email;
        Collection cUserInfo_Email;
        if (senderEmailAddressInitialized)
        {
            Ret = senderEmailAddressStr.ToString();
        }
        else
        {
            SiteAPI m_refSiteapi = new SiteAPI();
            userObj_Email = m_refSiteapi.EkUserRef;
            cUserInfo_Email = (Collection)userObj_Email.GetUserEmailInfoByID(m_refSiteapi.UserId);
            if (cUserInfo_Email.Count > 0)
            {
                senderEmailAddressStr = SafeColRead_Email(ref cUserInfo_Email, "EmailAddr1");
                senderEmailAddressInitialized = true;
                Ret = senderEmailAddressStr.ToString();
            }
        }
        cUserInfo_Email = null;
        userObj_Email = null;
        return Ret;
    }
    // --------------------------------------------------------------------------
    // Returns true only if the logged in user has a valid (non-empty string)
    // email address, otherwise false:
    public bool IsLoggedInUsersEmailValid()
    {
        bool returnValue;
        string addressStr;
        returnValue = false;
        addressStr = GetSendersEmailAddress();
        if (addressStr.Length > 0)
        {
            returnValue = true;
        }
        return returnValue;
    }

    #region  JavaScript Functions

    public string EmailJS()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        result.Append("<script language=\"javascript\">" + "\r\n");
        result.Append("	var g_emailChecked = false;" + "\r\n");

        result.Append("	function PopUpWindow_Email (url, hWind, nWidth, nHeight, nScroll, nResize) {" + "\r\n");
        result.Append("		var cToolBar = \'toolbar=0,location=0,directories=0,status=\' + nResize + \',menubar=0,scrollbars=\' + nScroll + \',resizable=\' + nResize + \',width=\' + nWidth + \',height=\' + nHeight;" + "\r\n");
        result.Append("		var popupwin = window.open(url, hWind, cToolBar);" + "\r\n");
        result.Append("		return popupwin;" + "\r\n");
        result.Append("	}" + "\r\n");

        result.Append("	function IsBrowserIE_Email() {" + "\r\n");
        result.Append("		if (window.location.href.indexOf(\'override_ie=true\') > 0)" + "\r\n");
        result.Append("			return false;" + "\r\n");
        result.Append("" + "\r\n");
        result.Append("		if (null == document.getElementById(\"EmailFrameContainer\"))" + "\r\n");
        result.Append("			return false;" + "\r\n");
        result.Append("" + "\r\n");
        result.Append("		// document.all is an IE only property" + "\r\n");
        result.Append("		return (document.all ? true : false);" + "\r\n");
        result.Append("	}" + "\r\n");

        result.Append(" function getIEVersion()" + "\r\n");
        result.Append(" {" + "\r\n");
        result.Append("   // Returns IE version or -1" + "\r\n");
        result.Append("   var rv = -1; // Return value assumes failure." + "\r\n");
        result.Append("   if (navigator.appName == \'Microsoft Internet Explorer\')" + "\r\n");
        result.Append("   {" + "\r\n");
        result.Append("     var ua = navigator.userAgent;" + "\r\n");
        result.Append("     var re  = new RegExp(\"MSIE ([0-9]{1,}[\\.0-9]{0,})\");" + "\r\n");
        result.Append("     if (re.exec(ua) != null)" + "\r\n");
        result.Append("       rv = parseFloat( RegExp.$1 );" + "\r\n");
        result.Append("   }" + "\r\n");
        result.Append("   return rv;" + "\r\n");
        result.Append(" }" + "\r\n");

        result.Append("	function ToggleEmailCheckboxes() {" + "\r\n");
        result.Append("		var idx, prefix, name;" + "\r\n");

        result.Append("		g_emailChecked = !g_emailChecked;" + "\r\n");
        result.Append("for(idx = 0; idx < document.forms[0].elements.length; idx++ ) {" + "\r\n");
        result.Append("if (document.forms[0].elements[idx].type == \"checkbox\") {" + "\r\n");
        result.Append("name = document.forms[0].elements[idx].name;" + "\r\n");
        result.Append("				if (name.indexOf(\"emailcheckbox_\") != -1) {" + "\r\n");
        result.Append("					document.forms[0].elements[idx].checked = g_emailChecked;" + "\r\n");
        result.Append("				}" + "\r\n");
        result.Append("			}" + "\r\n");
        result.Append("		}				" + "\r\n");
        result.Append("	}			" + "\r\n");

        result.Append("	// Open email window/layer ontop of current window:" + "\r\n");
        result.Append("	function LoadEmailChildPage(userGrpId) {" + "\r\n");
        result.Append("        var pageObj, frameObj;" + "\r\n");

        result.Append("		if (IsBrowserIE_Email() && getIEVersion() != 6) {" + "\r\n");
        result.Append("			// Configure the email window to be visible:" + "\r\n");
        result.Append("			frameObj = document.getElementById(\"EmailChildPage\");" + "\r\n");
        result.Append("			if ((typeof(frameObj) == \"object\") && (frameObj != null)) {" + "\r\n");
        result.Append("				frameObj.src = \"blankredirect.aspx?email.aspx?\" + userGrpId;" + "\r\n");
        result.Append("				pageObj = document.getElementById(\"EmailFrameContainer\");" + "\r\n");
        result.Append("				pageObj.style.display = \"\";" + "\r\n");
        result.Append("				pageObj.style.width = \"85%\";" + "\r\n");
        result.Append("				pageObj.style.height = \"90%\";" + "\r\n");

        result.Append("				// Ensure that the transparent layer completely covers the parent window:" + "\r\n");
        result.Append("				pageObj = document.getElementById(\"EmailActiveOverlay\");" + "\r\n");
        result.Append("				pageObj.style.display = \"\";" + "\r\n");
        result.Append("				pageObj.style.width = \"100%\";" + "\r\n");
        result.Append("				pageObj.style.height = \"100%\";" + "\r\n");
        result.Append("			}" + "\r\n");
        result.Append("		}" + "\r\n");
        result.Append("		else {" + "\r\n");
        result.Append("			// Using Netscape; cant use transparencies & eWebEditPro preperly " + "\r\n");
        result.Append("			// - so launch in a seperate pop-up window:" + "\r\n");
        result.Append("			PopUpWindow_Email(\"blankredirect.aspx?email.aspx?\" + userGrpId,\"CMSEmail\",490,500,1,1);" + "\r\n");
        result.Append("			CloseEmailChildPage();" + "\r\n");
        result.Append("		}" + "\r\n");
        result.Append("	}" + "\r\n");

        result.Append("	// Open email window/layer ontop of current window (extended version, " + "\r\n");
        result.Append("	// iterates through the controls to determine which usuer/group to add):" + "\r\n");
        result.Append("	function LoadEmailChildPageEx() {" + "\r\n");
        result.Append("		var idx, name, prefix, userGrpId, pageObj, qtyElements, frameObj, haveTargets=false;" + "\r\n");

        result.Append("		// build user-group ID string, based on current check-box status:" + "\r\n");
        result.Append("		userGrpId = \"\";" + "\r\n");
        result.Append("		prefix = \"emailcheckbox_\";" + "\r\n");
        result.Append("        qtyElements = document.forms[0].elements.length" + "\r\n");
        result.Append("		for(idx = 0; idx < qtyElements; idx++ ) {" + "\r\n");
        result.Append("			if (document.forms[0].elements[idx].type == \"checkbox\"){" + "\r\n");
        result.Append("				name = document.forms[0].elements[idx].name;" + "\r\n");
        result.Append("				if ((name.length > prefix.length)" + "\r\n");
        result.Append("					&& (document.forms[0].elements[idx].checked == true)) {" + "\r\n");
        result.Append("					userGrpId = userGrpId + name.substring(prefix.length) + \",\";" + "\r\n");
        result.Append("					haveTargets = true;" + "\r\n");
        result.Append("				}" + "\r\n");
        result.Append("			}" + "\r\n");
        result.Append("		}			" + "\r\n");
        result.Append("		if (haveTargets) {" + "\r\n");
        result.Append("			// Build either a user array or a group array:" + "\r\n");
        result.Append("			if (typeof(document.forms[0].groupMarker) == \"undefined\")" + "\r\n");
        result.Append("				userGrpId = \"userarray=\" + escape(userGrpId.substring(0, userGrpId.length - 1));" + "\r\n");
        result.Append("        else" + "\r\n");
        result.Append("				userGrpId = \"grouparray=\" + escape(userGrpId.substring(0, userGrpId.length - 1));" + "\r\n");
        result.Append("			if (IsBrowserIE_Email()) {" + "\r\n");
        result.Append("				frameObj = document.getElementById(\"EmailChildPage\");" + "\r\n");
        result.Append("				if ((typeof(frameObj) == \"object\") && (frameObj != null)) {" + "\r\n");
        result.Append("					frameObj.src = \"blankredirect.aspx?email.aspx?\" + userGrpId;" + "\r\n");
        result.Append("					pageObj = document.getElementById(\"EmailFrameContainer\");" + "\r\n");
        result.Append("					pageObj.style.display = \"\";" + "\r\n");
        result.Append("					pageObj.style.width = \"85%\";" + "\r\n");
        result.Append("					pageObj.style.height = \"90%\";" + "\r\n");

        result.Append("					pageObj = document.getElementById(\"EmailActiveOverlay\");" + "\r\n");
        result.Append("					pageObj.style.display = \"\";" + "\r\n");
        result.Append("					pageObj.style.width = \"100%\";" + "\r\n");
        result.Append("					pageObj.style.height = \"100%\";" + "\r\n");
        result.Append("				}" + "\r\n");
        result.Append("			}" + "\r\n");
        result.Append("			else {" + "\r\n");
        result.Append("				PopUpWindow_Email(\"blankredirect.aspx?email.aspx?\" + userGrpId,\"CMSEmail\",490,500,1,1);" + "\r\n");
        result.Append("				CloseEmailChildPage();" + "\r\n");
        result.Append("			}" + "\r\n");
        result.Append("		}" + "\r\n");
        result.Append("		else {" + "\r\n");
        result.Append("			alert(\"" + gtMessEmail.GetMessage("email error: No users selected to receive email") + "\");" + "\r\n");
        result.Append("		}" + "\r\n");
        result.Append("	}" + "\r\n");

        result.Append("	// Close email window/layer:" + "\r\n");
        result.Append("	function CloseEmailChildPage() {" + "\r\n");
        result.Append("		if (IsBrowserIE_Email()) {" + "\r\n");
        result.Append("			var pageObj = document.getElementById(\"EmailFrameContainer\");" + "\r\n");

        result.Append("			// Configure the email window to be invisible:" + "\r\n");
        result.Append("			pageObj.style.display = \"none\";" + "\r\n");
        result.Append("			pageObj.style.width = \"1px\";" + "\r\n");
        result.Append("			pageObj.style.height = \"1px\";" + "\r\n");

        result.Append("			// Ensure that the transparent layer does not cover any of the parent window:" + "\r\n");
        result.Append("			pageObj = document.getElementById(\"EmailActiveOverlay\");" + "\r\n");
        result.Append("			pageObj.style.display = \"none\";" + "\r\n");
        result.Append("			pageObj.style.width = \"1px\";" + "\r\n");
        result.Append("			pageObj.style.height = \"1px\";" + "\r\n");
        result.Append("		}" + "\r\n");
        result.Append("	}" + "\r\n");
        result.Append("</script>" + "\r\n");
        return (result.ToString());

    }
    #endregion
}
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

public partial class email : System.Web.UI.Page
{
    #region Private Members
    protected string globalWarningMsg = "";
    protected long UserId;
    protected long GroupId;
    protected string UserArray;
    protected string GroupArray;
    protected object MsgNotes;
    protected long ContentId;
    protected string emailclangid;
    protected object target;
    protected object source;
    protected object subject;
    protected object sMessage;
    protected EkMessageHelper m_refMsg;
    protected string strTargUserEmail;
    protected string strUserFName;
    protected string strUserLName;
    protected object iMaxContLength;
    protected object localeFileString;
    protected string strUserName;
    protected EmailHelper m_refMail = new EmailHelper();
    protected string AppeWebPath = "";
    protected Ektron.Cms.User.EkUser m_refUser;
    protected Ektron.Cms.Content.EkContent m_refContent = new Ektron.Cms.Content.EkContent();
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected string BrowserCode = "en";
    protected LanguageData language_data;
    protected int ContentLanguage = -1;
    private bool fromModal = false;
    private bool commerceAdmin = false;
    private string cancelJavascript = "closeEmailChildPage();";


    #endregion


    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        //string var1 = "";
        string var2 = "";

        if (!string.IsNullOrEmpty(Request.QueryString["fromModal"]))
        {

            fromModal = Convert.ToBoolean(Request.QueryString["fromModal"]);
            commerceAdmin = m_refSiteApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin);
            cancelJavascript = "if ('function' == typeof parent.EktronUiDialogClose) { parent.EktronUiDialogClose('email'); } else { parent.ektb_remove(); }"; 
           
        }
       
        RegisterResources();
        m_refMsg = m_refSiteApi.EkMsgRef;
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (m_refSiteApi.RequestInformationRef.IsMembershipUser > 0)
        {
            Response.Redirect((string)("reterror.aspx?info=" + m_refSiteApi.EkMsgRef.GetMessage("msg login cms user")), true);
            return;
        }
        ContentLanguage = m_refSiteApi.ContentLanguage;
        if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
        {
            ContentLanguage = m_refSiteApi.DefaultContentLanguage;
        }
        language_data = m_refSiteApi.GetLanguageById(ContentLanguage);
        BrowserCode = language_data.BrowserCode;
        strTargUserEmail = "";
        strUserFName = "";
        strUserLName = "";
        globalWarningMsg = "";
        iMaxContLength = 65000;
        localeFileString = "0000";
        m_refUser = m_refSiteApi.EkUserRef;
        m_refContent = m_refSiteApi.EkContentRef;
        UserId = Convert.ToInt64(Request.QueryString["userid"]);
        GroupId = Convert.ToInt64(Request.QueryString["groupid"]);

        UserArray = Request.QueryString["userarray"];
        GroupArray = Request.QueryString["grouparray"];
        MsgNotes = Request.QueryString["notes"];
        ContentId = Convert.ToInt64(Request.QueryString["contentid"]);
        emailclangid = Request.QueryString["emailclangid"];
        target = Request.Form["target"];
        source = Request.Form["source"];
        subject = Request.Form["subject"];
        sMessage = this.message.Content;
        AppeWebPath = m_refSiteApi.ApplicationPath + m_refSiteApi.AppeWebPath;
        var2 = m_refContent.GetEditorVariablev2_0(0, "email");
        EmailData.Text += m_refMail.EmailJS();
        DisplayControl();
    }

    private void DisplayControl()
    {
        Collection cUserInfo = new Collection();
        object cGroups;
        object cGroup;
        string tempString;
        if (Page.IsPostBack)
        {
            // Post-Back, user clicked the "send" button:
            try
            {
                RegExpValidator.Validate();
                if (RegExpValidator.IsValid)
                {
                    if (SendEmail()) // Email Sent, hide/close window:
                    {

                        EmailData.Text += "<script type=\"text/javascript\">" + cancelJavascript + "</script>";

                    }
                }
            }
            catch (Exception ex)
            {
                TD_msg.Text = "<div class=\"ui-state-error ektronTopSpaceSmall\"><span class=\"ui-icon warning\"></span>" + m_refMsg.GetMessage("Error: SendEmail-function failed") + "\r\n" + ex.Message + "</div>";
            }
        }
        else // ---------------------------------------------------------------------
        {
            if (GetIntSafe(UserId) > 0)
            {
                //
                // Userid was passed, get info for email target:
                //
                cUserInfo = (Collection)m_refUser.GetUserEmailInfoByID(UserId);
                if (cUserInfo.Count > 0)
                {
                    string st1 = "UserName";
                    string st2 = "FirstName";
                    string st3 = "LastName";
                    string st4 = "EmailAddr1";
                    strUserName = m_refMail.SafeColRead_Email(ref cUserInfo, st1);
                    strUserFName = m_refMail.SafeColRead_Email(ref cUserInfo, st2);
                    strUserLName = m_refMail.SafeColRead_Email(ref cUserInfo, st3);
                    strTargUserEmail = m_refMail.SafeColRead_Email(ref cUserInfo, st4);
                }
                ShowControls();
            }
            else if (GetIntSafe(GroupId) > 0)
            {
                //
                // Groupid was passed, get all users in the group:
                // Build strTargUserEmail (a comma-delimited string of all user addresses from group):
                //
                cGroups = m_refUser.GetTaskUsersByGroupv2_0(GroupId, "");
                foreach (object tempLoopVar_cGroup in (IEnumerable)cGroups)
                {
                    cGroup = tempLoopVar_cGroup;
                    Collection cGroup1 = (Collection)cGroup;
                    string st5 = "EmailAddr1";
                    tempString = m_refMail.SafeColRead_Email(ref cGroup1, st5);
                    if (Strings.Len(tempString) > 0)
                    {
                        strTargUserEmail = strTargUserEmail + tempString + ", ";
                    }
                }
                if (Strings.Len(strTargUserEmail) > 2)
                {
                    strTargUserEmail = Strings.Left(strTargUserEmail.ToString(), System.Convert.ToInt32(Strings.Len(strTargUserEmail) - 2));
                }
                ShowControls();

            }
            else if ((!string.IsNullOrEmpty(UserArray)) && (Information.IsNumeric(UserArray)))
            {
                //
                // Build strTargUserEmail (a comma-delimited string of all user addresses, from user ids passed in array):
                //
                string[] UserA = Strings.Split(UserArray.ToString(), ",", -1, 0);
                UserArray = UserA[UserA.Length - 1];
                foreach (long tempLoopVar_UserId in UserArray)
                {
                    for (int i = 0; i < UserA.Length; i++)
                    {
                        UserId = Convert.ToInt64(UserA[i]);

                        // Get info for email target:
                        cUserInfo = (Collection)m_refUser.GetUserEmailInfoByID(UserId);

                        if (cUserInfo.Count > 0)
                        {
                            string EmailAddr1 = "EmailAddr1";
                            tempString = m_refMail.SafeColRead_Email(ref cUserInfo, EmailAddr1);
                            if (Strings.Len(tempString) > 0)
                            {
                                strTargUserEmail = strTargUserEmail + tempString + ", ";
                            }
                        }
                    }
                }
                if (Strings.Len(strTargUserEmail) > 2)
                {
                    strTargUserEmail = Strings.Left(strTargUserEmail.ToString(), System.Convert.ToInt32(Strings.Len(strTargUserEmail) - 2));
                }
                ShowControls();

                cUserInfo = null;
            }
            else if (!string.IsNullOrEmpty(GroupArray) && Information.IsNumeric(GroupArray))
            {
                //
                // Build strTargUserEmail (a comma-delimited string of all user addresses, from group ids passed in array):
                //
                string[] groupA = Strings.Split(GroupArray.ToString(), ",", -1, 0);
                GroupArray = groupA[groupA.Length - 1];
                for(int i = 0; i < groupA.Length; i++)
                {
                    GroupId = Convert.ToInt64(groupA[i]);
                    cGroups = m_refUser.GetTaskUsersByGroupv2_0(GroupId, "");
                    foreach (object tempLoopVar_cGroup in (Collection)cGroups)
                    {
                        string EmailAddr1 = "EmailAddr1";
                        cGroup = tempLoopVar_cGroup;
                        Collection cGroup1 = (Collection)cGroup;
                        tempString = m_refMail.SafeColRead_Email(ref cGroup1, EmailAddr1);
                        if (Strings.Len(tempString) > 0)
                        {
                            strTargUserEmail = strTargUserEmail + tempString + ", ";
                        }
                    }

                }
                if (Strings.Len(strTargUserEmail) > 2)
                {
                    strTargUserEmail = Strings.Left(strTargUserEmail.ToString(), System.Convert.ToInt32(Strings.Len(strTargUserEmail) - 2));
                }
                ShowControls();

            }
            else
            {
                // Error: No information obained from querystring, nothing passed from source:
                ShowError(m_refMsg.GetMessage("Error: Email missing source address") + ", " + m_refMsg.GetMessage("Error: Email missing target address"));
            }
        }
    }
    private bool SendEmail()
    {
        Ektron.Cms.Common.EkMailService objMailServ;
        try
        {
            if (!string.IsNullOrEmpty(target.ToString()))
            {
                objMailServ = m_refSiteApi.EkMailRef;

                objMailServ.MailTo = target.ToString();
                objMailServ.MailFrom = source.ToString();
                objMailServ.MailSubject = subject.ToString();
                objMailServ.MailBodyText = sMessage.ToString();
                objMailServ.SendMail();
                objMailServ = null;
            }
            else
            {
                throw (new Exception(m_refMsg.GetMessage("Error: Email missing target address")));
            }
            return (true);
        }
        catch (Exception ex)
        {
            throw (new Exception(ex.Message));
        }
        finally
        {
            objMailServ = null;
        }
        // Exiting the function restores default error handling (crash script).
        // Alternately could do explicitly: On Error Goto 0
    }

    // ---------------------------------------------------------------------------
    // Render the email UI:
    public void ShowControls()
    {
        string msgPrefix;
        string tempStr;
        string strSrcUserEmail;
        msgPrefix = null;
        TD_msg.Text = "";
        // Attempt to get senders email address (error if empty string).
        // Note: Should not be able to get here if source address is missing,
        // as email icons/links will not be shown, but tested here anyway:
        strSrcUserEmail = m_refMail.GetSendersEmailAddress();
        if ((strSrcUserEmail.ToString() == "") && !commerceAdmin)
        {
            throw (new Exception(m_refMsg.GetMessage("Error: Email missing source address")));
        }


        // Warn if destination email address is empty string, if nothing else to show:
        if ((strTargUserEmail == "") && (globalWarningMsg == ""))
        {
            globalWarningMsg = "*** " + m_refMsg.GetMessage("Warning: destination address missing") + " ***";
        }

        // Prepare to insert username if available:
        if ((!string.IsNullOrEmpty(strUserFName.ToString())) || (!string.IsNullOrEmpty(strUserLName.ToString())))
        {
            msgPrefix = strUserFName;
            if ((!string.IsNullOrEmpty(msgPrefix.ToString())) && (!string.IsNullOrEmpty(strUserLName.ToString())))
            {
                msgPrefix = msgPrefix + " ";
            }
            msgPrefix = "<span>" + msgPrefix + strUserLName + ": " + "</span>";
        }

        // Add related content url if available (requires content-id to be supplied):
        tempStr = GetContentUrl();
        if (!string.IsNullOrEmpty(tempStr.ToString()))
        {
            msgPrefix = msgPrefix + "<br/>" + tempStr + "<br/>";
        }
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        // Render the UI page:
        if (globalWarningMsg.Length > 0)
        {
            result.Append("<div class=\"ui-state-error ektronTopSpaceSmall\"><span class=\"ui-icon warning\"></span>" + globalWarningMsg + "</div>");
        }
        result.Append("<table class=\"ektronGrid ektronBorder maxWidth ektronTopSpace\">");
        result.Append("<tr><td>");
        result.Append("<label class=\"label right\"> " + m_refMsg.GetMessage("generic to label") + "</label></td><td>&nbsp;<input type=\"text\" value=\"" + RemoveRedunancies(strTargUserEmail) + "\" name=\"target\" id=\"target\" size=\"49\" " + (fromModal ? "readonly style=\"background:#f4f4f4;\" " : "") + " /><br/>");
        result.Append("</td></tr>");
        result.Append("<tr><td>");
        if (fromModal && commerceAdmin)
        {

            result.Append("<label class=\"label right\"> " + m_refMsg.GetMessage("generic from label") + "</label></td><td>&nbsp;");
            result.Append("<input type=text value=\"" + strSrcUserEmail + "\" name=\"source\" id=\"source\" size=\"49\" maxlength=\"49\"/>");

        }
        else
        {

            result.Append("<label class=\"label right\"> " + m_refMsg.GetMessage("generic from label") + "</label></td><td>&nbsp;<label>" + strSrcUserEmail + "</label><br/>");
            result.Append("<input type=\"hidden\" value=\"" + strSrcUserEmail + "\" name=\"source\" id=\"source\" />");

        }
        result.Append("<input type=\"hidden\" value=\"true\" name=\"postback\" id=\"postback\" />");
        result.Append("</td></tr>");
        result.Append("<tr><td>");
        result.Append("<label class=\"label right\"> " + m_refMsg.GetMessage("generic subject label") + "</label></td><td>&nbsp;<input type=\"text\" value=\"" + MsgNotes + "\" name=\"subject\" id=\"subject\" size=\"49\" maxlength=\"44\" /><br/>");
        result.Append("</td></tr></table>");
        EmailData.Text += result.ToString();

        this.message.Content = "<p>" + msgPrefix + "</p>";
        this.message.AllowFonts = true;
        this.message.Visible = true;
        RegExpValidator.ErrorMessage = m_refMsg.GetMessage("content size exceeded");
        RegExpValidator.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(System.Convert.ToInt32(iMaxContLength));

        System.Text.StringBuilder result2 = new System.Text.StringBuilder();
        dvButtons.Visible = true;
        this.btnSendMail.Text = m_refMsg.GetMessage("send email button text");
        ltrCancel.Text = "    <li><a class=\"button buttonInlineBlock redHover buttonClear\" type=\"button\" onclick=\"" + cancelJavascript + "\" value=\"" + m_refMsg.GetMessage("generic Cancel") + "\" name=\"cancel\" id=\"cancel\" >" + m_refMsg.GetMessage("generic Cancel") + "</a></li>";
        EmailData2.Text += result2.ToString();
    }
    // ---------------------------------------------------------------------------
    // Show the error message and a Cancel/Exit button:
    public void ShowError(object ErrorMsg)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<div class=\"ui-state-error ektronTopSpaceSmall\">");
        result.Append("<span class=\"ui-icon warning\"></span>");
        result.Append(ErrorMsg);
        result.Append("<ul class=\"buttonWrapper buttonWrapperLeft ektronTopSpace ui-helper-clearfix\"><li><input type=\"button\" onclick=\"" + cancelJavascript + "\" value=\"" + m_refMsg.GetMessage("generic Cancel") + "\" name=\"cancel\" id=\"cancel\"></li></ul>");
        result.Append("</div>");
        TD_msg.Text = result.ToString();
    }
    // ---------------------------------------------------------------------------
    public string GetContentUrl()
    {
        string returnValue;
        ShowContentBlock cCont;
        string quickLink;
        string tempStr;
        string urlStr;
        string ErrString;
        object QorAmp;
        long folderid;
        ErrString = null;
        returnValue = "";
        if (GetIntSafe(ContentId) > 0)
        {
            cCont = m_refContent.ShowContentv2_0(ContentId, false, false);
            folderid = Convert.ToInt64(cCont.FolderID);
            if (folderid >= 0)
            {
                if (folderid >= 0)
                {
                    quickLink = m_refContent.GetContentQlink(ContentId, folderid);
                    if (ErrString == null)
                    {
                        if (!string.IsNullOrEmpty(quickLink))
                        {
                            tempStr = Request.ServerVariables["HTTPS"];
                            if ((Strings.LCase(tempStr.ToString())).IndexOf("off") + 1 != -1)
                            {
                                urlStr = "http://";
                            }
                            else
                            {
                                urlStr = "https://";
                            }

                            urlStr = urlStr + Request.ServerVariables["HTTP_HOST"];

                            tempStr = System.Convert.ToInt32(Request.ServerVariables["SERVER_PORT"]).ToString().Trim();
                            if (System.Convert.ToInt32(tempStr) != 80)
                            {
                                urlStr = urlStr + ":" + tempStr;
                            }

                            if (Strings.Left(quickLink.ToString(), 1) != "/")
                            {
                                quickLink = "/" + quickLink;
                            }

                            if (quickLink.ToString().IndexOf("?") + 1 > 0)
                            {
                                QorAmp = "&";
                            }
                            else
                            {
                                QorAmp = "?";
                            }

                            if (!string.IsNullOrEmpty(emailclangid))
                            {
                                urlStr = urlStr + quickLink + QorAmp + "LangType=" + emailclangid;
                            }
                            else
                            {
                                urlStr = urlStr + quickLink + QorAmp + "LangType=" + m_refSiteApi.ContentLanguage;
                            }

                            returnValue = "<span>URL:</span> <a href=\"" + EkFunctions.HtmlEncode(urlStr.ToString()) + "\">" + Server.HtmlEncode(urlStr.ToString()) + "</a>";
                        } // (quickLink <> "")
                    } // (ErrString = "")
                } // (folderId >= 0)
            } // (folderId <> "")

        }

        return returnValue;
    }
    // ---------------------------------------------------------------------------
    // Remove redundant names from the supplied text-string (assumes semicolon
    // delimiter). Scans supplied string for a match, if none found then includes
    // in returned string.
    public object RemoveRedunancies(object textStr)
    {
        object returnValue;
        object retVal;
        string[] arrayStr;
        int index;
        int index2;
        int limit;

        retVal = "";
        if (Strings.Len(textStr) > 0)
        {
            arrayStr = Strings.Split(textStr.ToString(), ",", -1, 0);
            limit = arrayStr.Length - 1;
            // Remove whitespace from around items:
            for (index = 0; index <= limit; index++)
            {
                arrayStr[index] = Strings.Trim((string)(arrayStr[index]));
            }
            // Scan for duplicates:
            for (index = 0; index <= limit; index++)
            {
                for (index2 = (index + 1); index2 <= limit; index2++)
                {
                    if (arrayStr[index] == arrayStr[index2])
                    {
                        break;
                    }
                }
                // Append item is match was not found:
                if (index2 > limit)
                {
                    retVal = retVal + arrayStr[index] + ", ";
                }
            }
            if (Strings.Len(retVal) > 2)
            {
                retVal = Strings.Left(retVal.ToString(), System.Convert.ToInt32(Strings.Len(retVal) - 2));
            }
        }
        returnValue = retVal;
        return returnValue;
    }
    //----------------------------------------------------------------------------
    // GetIntSafe:
    // Returns either the integer conversion, or zero (if empty string supplied).
    public long GetIntSafe(long strVal)
    {
        long returnValue;
        long retVal = 0;
        if (!(Information.IsDBNull(strVal)))
        {
            if (!string.IsNullOrEmpty(Convert.ToString(strVal)))
            {
                retVal = System.Convert.ToInt64(strVal);
            }
        }
        returnValue = retVal;
        return returnValue;
    }
    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
    }
}
	


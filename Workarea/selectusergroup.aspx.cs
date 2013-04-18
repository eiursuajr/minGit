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

public partial class selectusergroup : System.Web.UI.Page
{
    protected long GroupId;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected string strTargUserEmail = "";
    protected object strUserArray;
    protected object strSrcUserEmail;
    protected object strList;
    protected object strAttachReports;
    protected object msgPrefix;
    protected object target;
    protected string[] UserArray = new string[] { };
    protected string[] GroupArray = new string[] { };
    protected string UserList;
    protected string GroupList;
    protected object localeFileString;
    protected object iMaxContLength;
    protected string AppeWebPath;
    protected long m_intId = 0;
    protected long m_intFolderId = -1;
    protected int m_taskType = 0;
    protected UserAPI m_refUserApi = new UserAPI();
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected EmailHelper m_refMail = new EmailHelper();
    protected string m_strPageAction = "";
    protected string m_sRptStatus = "";
    protected EkMailService objMailServ;
    protected string UserIdList = "";
    protected string GroupIdList = "";

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (m_refUserApi.UserId == 0 || m_refUserApi.RequestInformationRef.IsMembershipUser > 0)
            {
                Response.Redirect("login.aspx?fromLnkPg=1", false);
                return;
            }
            if (!String.IsNullOrEmpty(Request.QueryString["id"]))
            {
                m_intId = Convert.ToInt64(Request.QueryString["id"]);

            }
            if (!String.IsNullOrEmpty(Request.QueryString["folder_id"]))
            {
                m_intFolderId = Convert.ToInt64(Request.QueryString["folder_id"]);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["TaskType"]))
            {
                m_taskType = Convert.ToInt32(Request.QueryString["TaskType"]);

            }
            m_refMsg = m_refUserApi.EkMsgRef;
            StyleSheetJS.Text = m_refStyle.GetClientScript();
            AppImgPath = m_refUserApi.AppImgPath;
            AppPath = m_refUserApi.AppPath;
            AppeWebPath = m_refSiteApi.AppeWebPath;

            RegisterResources();
            SetJsServerVariables();

            localeFileString = "0000";
            iMaxContLength = 65000;
            if (!(Request.QueryString["action"] == null))
            {
                if (Request.QueryString["action"] != "")
                {
                    m_strPageAction = EkFunctions.UrlEncode(Request.QueryString["action"]);
                }
                if (!(Request.Form["frm_email_list"] == null))
                {
                    strTargUserEmail = (string)(Request.Form["frm_email_list"].ToString());
                    if (strTargUserEmail.IndexOf(":") > 0)
                    {
                        GroupArray = strTargUserEmail.Substring(0, System.Convert.ToInt32(strTargUserEmail.IndexOf(":") - 1)).Split(',');
                    }
                    if ((strTargUserEmail.Length - strTargUserEmail.IndexOf(":") - 1) != 0)
                    {
                        UserArray = strTargUserEmail.Substring(System.Convert.ToInt32(strTargUserEmail.IndexOf(":") + 1), System.Convert.ToInt32(strTargUserEmail.LastIndexOf(",") - strTargUserEmail.IndexOf(":") - 1)).Split(',');
                    }
                }
                if (!String.IsNullOrEmpty(Request.QueryString["rptStatus"]))
                {
                    m_sRptStatus = Request.QueryString["rptStatus"];
                }
                if (!(Request.QueryString["user_ids"] == null))
                {
                    if (Request.QueryString["user_ids"] != "\'\'")
                    {
                        UserIdList = Strings.Replace(Request.QueryString["user_ids"], "\'", "", 1, -1, 0);
                        UserIdList = "," + UserIdList + ",";
                    }
                }
                if (!(Request.QueryString["group_ids"] == null))
                {
                    if (Request.QueryString["group_ids"] != "\'\'")
                    {
                        GroupIdList = Strings.Replace(Request.QueryString["group_ids"], "\'", "", 1, -1, 0);
                        GroupIdList = "," + GroupIdList + ",";
                    }
                }
            }
            if (m_strPageAction != "InputSearch")
            {
                Populate_UserGroupGrid();
                if ((m_strPageAction != "") && (m_strPageAction != "searchuser"))
                {
                    RemoveRedundancies(GroupArray);
                    RemoveRedundancies(UserArray);
                    SetUserEmailsList();
                    DisplayControl();
                }
            }
            else
            {
                DisplaySearchParameters();
            }

        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }
    public void SetUserEmailsList()
    {
        UserData cUserInfo = new UserData();
        UserData[] cGroups;

        try
        {
            strTargUserEmail = "";
            if (Page.IsPostBack)
            {
                if ((UserArray.Length == 0) && (GroupArray.Length == 0))
                {
                    // Error: No information obained from querystring, nothing passed from source:
                    Utilities.ShowError((string)(m_refMsg.GetMessage("Error: Email missing source address") + ", " + m_refMsg.GetMessage("Error: Email missing target address")));
                }

                if (UserArray.Length > 0)
                {
                    for (int i = 0; i <= UserArray.GetUpperBound(0); i++)
                    {
                        //For Each UserId In UserArray
                        // With the new ehnacement to allow user to enter email if the user is disabled notification makes
                        // to check if this is exisitng user Id or provided Email address
                        if (Information.IsNumeric(UserArray[i]))
                        {
                            cUserInfo = m_refUserApi.GetUserEmailInfoByID(Convert.ToInt64(UserArray[i]));

                            if (!(cUserInfo == null))
                            {
                                if (Strings.Len(cUserInfo.Email.ToString()) > 0)
                                {
                                    if (strTargUserEmail != "")
                                    {
                                        strTargUserEmail = strTargUserEmail + ", " + cUserInfo.Email.ToString();
                                    }
                                    else
                                    {
                                        strTargUserEmail = (string)(cUserInfo.Email.ToString());
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If it is a provided email address just append it to the email list
                            strTargUserEmail = strTargUserEmail + UserArray[i] + ", ";
                        }
                    }
                }
                if (GroupArray.Length > 0)
                {
                    bool hasNonEmptyGroup = false;
                    //GroupArray = Strings.Split(GroupArray.ToString(), ",", -1, 0);
                    for (int i = 0; i <= GroupArray.GetUpperBound(0); i++)
                    {
                        if (Information.IsNumeric(GroupArray[i]))
                        {
                            GroupId = Convert.ToInt64(GroupArray[i]);
                            cGroups = m_refUserApi.GetUsersByGroup(GroupId, "");
                            //If (cGroups Is Nothing) Then
                            // If the group is empty check if there are any further groups selected.
                            // If there are more groups, proceed with
                            //Throw New Exception(m_refMsg.GetMessage("error: no user in the group"))
                            if (!(cGroups == null))
                            {
                                hasNonEmptyGroup = true;
                                foreach (UserData cGroup in cGroups)
                                {
                                    if (Strings.Len(cGroup.Email.ToString()) > 0)
                                    {
                                        if (strTargUserEmail != "")
                                        {
                                            strTargUserEmail = strTargUserEmail + ", " + cGroup.Email.ToString();
                                        }
                                        else
                                        {
                                            strTargUserEmail = (string)(cGroup.Email.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!(hasNonEmptyGroup))
                    {
                        throw (new Exception(m_refMsg.GetMessage("no user in group")));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
        finally
        {
            cUserInfo = null;
        }
    }
    public void DisplaySearchParameters()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        SelectUserGroupToolBar();
        result.Append("<table class=\"ektronForm\">");
        result.Append("<tr>");
        result.Append("<td class=\"label\">");
        result.Append("" + m_refMsg.GetMessage("first name label") + "</td>");
        result.Append("<td class=\"value\">");
        result.Append("<input type=\"text\" value=\"\" name=\"First Name\" id=\"fname\">");
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">");
        result.Append("" + m_refMsg.GetMessage("last name label") + "</td>");
        result.Append("<td class=\"value\">");
        result.Append("<input type=\"text\" value=\"\" name=\"Last Name\" id=\"lname\">");
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">");
        result.Append("" + m_refMsg.GetMessage("username label") + "</td>");
        result.Append("<td class=\"value\">");
        result.Append("<input type=\"text\" value=\"\" name=\"Login Name\" id=\"loginname\">");
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">");
        result.Append("" + m_refMsg.GetMessage("lbl community group name") + ":" + "</td>");
        result.Append("<td class=\"value\">");
        result.Append("<input type=\"text\" value=\"\" name=\"Group Name\" id=\"grpname\">");
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("</table>");
        result.Append("<div class=\"ektronTopSpace\">");
        result.Append("<input type=\"button\" onclick=\"submit_form(\'send\')\" value=\"" + m_refMsg.GetMessage("btn search") + " " + m_refMsg.GetMessage("lbl name") + "\"send\" id=\"send\">");
        EmailData.Text = result.ToString();
    }
    public string RemoveRedundancies(string textStr)
    {
        return RemoveRedundancies(Strings.Split(textStr.ToString(), ",", -1, 0));
    }
    public string RemoveRedundancies(string[] textStr)
    {
        string returnValue = "";
        string retVal = "";
        int index;
        int index2;
        int limit;
        if ((textStr.Length - 1) > -1)
        {
            limit = textStr.Length - 1;
            // Remove whitespace from around items:
            for (index = 0; index <= limit; index++)
            {
                textStr[index] = Strings.Trim(textStr[index]);
            }
            // Scan for duplicates:
            for (index = 0; index <= limit; index++)
            {
                for (index2 = (index + 1); index2 <= limit; index2++)
                {
                    if (textStr[index] == textStr[index2])
                    {
                        break;
                    }
                }
                // Append item is match was not found:
                if (index2 > limit)
                {
                    retVal = retVal + textStr[index] + ", ";
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
    private void Populate_UserGroupGrid()
    {
        string sUserId = "";
        string sGroupId = "";
        string sUserList = "";
        string sGroupList = "";
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();

        // set up collections to look up which users/groups were currently selected
        SortedList selUserIDs = new SortedList();
        SortedList selGroupIDs = new SortedList();
        if (UserArray.Length > 0)
        {
            foreach (string tempLoopVar_UserId in (IEnumerable)UserArray)
            {
                string UserId = tempLoopVar_UserId;
                selUserIDs.Add(UserId, UserId);
            }
        }
        if (GroupArray.Length > 0)
        {
            foreach (string tempLoopVar_GroupId in (IEnumerable)GroupArray)
            {
               string GroupId = tempLoopVar_GroupId;
                selGroupIDs.Add(GroupId, GroupId);
            }
        }

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "EMAILAREA";
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderText = (string)("<input type=\"checkbox\" onclick=\"javascript:ToggleCheckboxes();\" title=\"" + m_refMsg.GetMessage("alt send email to all") + "\" id=\"emailAll\">&nbsp;" + m_refMsg.GetMessage("generic all"));
        colBound.ItemStyle.Wrap = false;
        UserGroupGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("user or group name title");
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        UserGroupGrid.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        dt.Columns.Add(new DataColumn("EMAILAREA", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));

        if ((m_intId > 0) && m_taskType != (int)Ektron.Cms.Common.EkEnumeration.TaskType.FormSubmissionTaskCategory)
        {
            dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "<br>";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "<a href=\"#\" onclick=\"javascript:selectUser(\'0\', \'0\', \'All Authors\');\" title=\"Select All Authors\">All Authors</a>";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "<br>";
            dt.Rows.Add(dr);
        }

        SortedList user_group_list;
        int i = 0;
        string strUsrNameType = "";
        string strUsrNameValue = "";

        if (m_strPageAction == "searchuser")
        {
            if (Request.QueryString["fname"] != "")
            {
                strUsrNameValue = (string)(Request.QueryString["fname"] + ",");
                strUsrNameType = "first_name,";
            }
            if (Request.QueryString["lname"] != "")
            {
                strUsrNameValue += (string)(Request.QueryString["lname"] + ",");
                strUsrNameType += "last_name,";
            }
            if (Request.QueryString["loginname"] != "")
            {
                strUsrNameValue += (string)(Request.QueryString["loginname"] + ",");
                strUsrNameType += "user_name,";
            }
            if (Request.QueryString["groupname"] != "")
            {
                strUsrNameValue += Request.QueryString["groupname"];
                strUsrNameType += "usergroup_name";
            }
            user_group_list = m_refUserApi.GetUserDataByUserName(strUsrNameValue, strUsrNameType);
        }
        else
        {
            if (m_intFolderId > -1 && m_intId == 0)
            {
                //create user_group_list for new form task by folder id when content id is not available.
                user_group_list = m_refUserApi.GetAllUsersAndUserGroupsByFolderId(m_refUserApi.UserId, m_intFolderId);
            }
            else
            {
                user_group_list = m_refUserApi.GetUsersAndUserGroupsForTask(m_refUserApi.UserId, m_intId);
            }
        }

        if (user_group_list != null)
        {
            for (i = 0; i < user_group_list.Count; i++)
            {
                dr = dt.NewRow();
                dr[0] = "";

                UserData userItem = null;
                UserGroupData groupItem = null;
                if (user_group_list.GetKey(i).ToString().StartsWith("usergroup"))
                    groupItem = user_group_list.GetByIndex(i) as UserGroupData; 
                else if (user_group_list.GetKey(i).ToString().StartsWith("user"))
                    userItem = user_group_list.GetByIndex(i) as UserData;

                if (groupItem != null && groupItem.Id > 0)
                {
                    if (m_strPageAction == "")
                    {
                        dr[1] = "<a href=\"#\" onclick=\"javascript:selectUser(\'2\', \'" + groupItem.Id + "\', \'" + groupItem.GroupDisplayName.Replace("&#39;", "\\&#39;") + "\');\" title=\"Select User or Group Name - Group " + groupItem.GroupDisplayName + "\">";
                    }
                    else
                    {
                        if (m_strPageAction.ToLower() != "report")
                        {
                            dr[0] = "<input type=\"checkbox\" name=\"emailGrpcheckbox_" + groupItem.Id + "\" ID=\"Checkbox1\">";
                        }
                        else
                        {
                            long itmpcaller = m_refUserApi.RequestInformationRef.CallerId;
                            try
                            {
                                object cGroups;
                                // We are making standard CMS User as internal admin to avoid permission error
                                m_refUserApi.RequestInformationRef.UserId = EkConstants.InternalAdmin;
                                m_refUserApi.RequestInformationRef.CallerId = EkConstants.InternalAdmin;

                                cGroups = m_refUserApi.GetUsersByGroup(groupItem.Id, "");
                                dr[0] = "<input type=\"checkbox\" name=\"emailGrpcheckbox_" + groupItem.Id + "\" ID=\"Checkbox1\"";
                                if ((GroupIdList.IndexOf(("," + groupItem.Id + ",").ToString()) + 1 > 0) || (selGroupIDs.ContainsKey(groupItem.Id)))
                                {
                                    dr[0] = dr[0] + " checked";
                                }
                                dr[0] = dr[0] + ">";
                            }
                            catch (Exception)
                            {
                                dr[0] = "<input type=\"checkbox\" disabled name=\"emailGrpcheckbox_" + groupItem.Id + "\" ID=\"Checkbox1\">";
                            }
                            finally
                            {
                                m_refUserApi.RequestInformationRef.UserId = itmpcaller;
                                m_refUserApi.RequestInformationRef.CallerId = itmpcaller;
                            }
                        }
                    }
                    dr[1] += "<img src=\"" + AppPath + "images/ui/icons/users.png\" align=\"absbottom\" alt=\"Select User or Group Name - Group " + groupItem.GroupDisplayName  + "\" title=\"Select User or Group Name - Group " + groupItem.GroupDisplayName + "\">" + groupItem.GroupDisplayName + "</a>";
                    if (sGroupList.Length > 0)
                    {
                        sGroupList = sGroupList + ",";
                        sGroupId = sGroupId + ",";
                    }
                    sGroupList = sGroupList + "\'" + groupItem.Id + "\'";
                    sGroupId = sGroupId + groupItem.Id;
                }
                else if (userItem != null && userItem.Id > 0)
                {
                    // If this is a user

                    // Prepare display name from FirstName + LastName
                    string displayName = String.Empty;
                    if (!String.IsNullOrEmpty(userItem.FirstName) && !String.IsNullOrEmpty(userItem.LastName))
                    {
                        displayName = String.Format("{0} {1} ({2})", userItem.FirstName, userItem.LastName, userItem.DisplayUserName);
                    }
                    else
                    {
                        displayName = userItem.DisplayUserName;
                    }

                    if (m_strPageAction == "")
                    {
                        dr[1] = "<a href=\"#\" onclick=\"javascript:selectUser(\'1\', \'" + userItem.Id + "\', \'" + displayName.Replace("\'", "\\&#39;") + "\');\" title=\"Select User or Group Name - User " + userItem.DisplayUserName + "\">";
                    }
                    else
                    {
                        dr[0] = "<input type=\"checkbox\" name=\"emailUsercheckbox_" + userItem.Id + "\" ID=\"Checkbox1\"";
                        if ((UserIdList.IndexOf(("," + userItem.Id + ",").ToString()) + 1 > 0) || (selUserIDs.ContainsKey(userItem.Id.ToString())))
                        {
                            dr[0] = dr[0] + " checked";
                        }
                        dr[0] = dr[0] + ">";
                    }
                    dr[1] += "<img src=\"" + AppPath + "images/ui/icons/user.png\" align=\"absbottom\" alt=\"Select User or Group Name - User " + userItem.DisplayUserName + "\" title=\"Select User or Group Name - User " + userItem.DisplayUserName + "\">" + displayName + "</a>";
                    if ((userItem.Email == "") && (m_strPageAction != "") && m_sRptStatus != "siteupdateactivity_siteRpt")
                    {
                        dr[1] += "<font color=\"red\"><i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + m_refMsg.GetMessage("alt email address not available") + "</i></font><input type=\"text\"  name=\"usrAddr" + userItem.Id + "\" id=\"usraddr" + userItem.Id + "\" Onkeypress=\"javascript:return CheckKeyValue(event,\'13\');\" value=\"\"  maxlength=\"100\">";
                    }
                    if (sUserList.Length > 0)
                    {
                        sUserList = sUserList + ",";
                        sUserId = sUserId + ",";
                    }
                    sUserList = sUserList + "\'" + userItem.DisplayUserName + "\'";
                    sUserId = sUserId + userItem.Id;
                }
                dt.Rows.Add(dr);
            }
        }

        DataView dv = new DataView(dt);
        UserGroupGrid.DataSource = dv;
        UserGroupGrid.DataBind();
        if (m_strPageAction == "")
        {
            UserGroupGrid.Columns[0].Visible = false;
        }
        SelectUserGroupToolBar();

        if ("siteupdateactivity_siteRpt" == m_sRptStatus)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append("var JSUserIdArray = new Array(" + sUserId + ");" + "\r\n");
            result.Append("var JSUserListArray = new Array(" + sUserList + ");" + "\r\n");
            result.Append("var JSGroupIdArray = new Array(" + sGroupId + ");" + "\r\n");
            result.Append("var JSGroupListArray = new Array(" + sGroupList + ");" + "\r\n");
            JSUserListArray.Text = result.ToString();
        }
    }
    private void DisplayControl()
    {
        if (Page.IsPostBack)
        {
            // Post-Back, user clicked the "send" button:
            try
            {
                if (SendEmail())
                {
                    // Email Sent, hide/close window:
                    if (EmailData.Text == "")
                    {
                        EmailData.Text = "<script language=\"javascript\">javascript:CloseEmailChildPage();</script>";
                    }
                }
            }
            catch (Exception ex)
            {
                TD_msg.InnerHtml = m_refMsg.GetMessage("Error: SendEmail-function failed") + "\r\n" + ex.Message;
            }
        }
        else
        {
            // display "from email addr" prompt if from address not configured
            if ((m_refMail.GetSendersEmailAddress() == "") && !("siteupdateactivity_siteRpt" == m_sRptStatus))
            {
                EmailData.Text = "<table><tr><td><input type=\"text\" id=\"frmEmailAddr\"></td><td><font color=\"red\">" + m_refMsg.GetMessage("msg enter email") + "</font></td></tr></table>";
            }
        }
    }
    private object RemoveItemsFromReport(string strBody, string findPattern, string startPt, string endPt)
    {
        object returnValue;
        int lInitPos;
        int lFinalPos;
        string strTemp;
        string strInt;
        string strFin;

        strTemp = strBody;
        while (strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase) > -1) //need to ignore case, FF and IE return different cases for Tag name.
        {
            strInt = strBody.Substring(0, strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase));
            if (findPattern != startPt)
            {
                lInitPos = strInt.LastIndexOf(startPt, System.StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                lInitPos = strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase);
            }
            if (lInitPos == -1)
            {
                break;
            }
            if (endPt == "")
            {
                //Just to remove the tables in the beginning of the task report
                lFinalPos = lInitPos;
                lInitPos = strBody.IndexOf(startPt, System.StringComparison.OrdinalIgnoreCase);
                if (lFinalPos != lInitPos)
                {
                    strBody = strBody.Replace(strBody.Substring(lInitPos, lFinalPos - lInitPos), "");
                    //One time execution exit the loop once the upper tables are removed
                }
                break;
            }
            else
            {
                strFin = strBody.Substring(strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase), strBody.Length - strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase) - 1);
                lFinalPos = strFin.IndexOf(endPt, System.StringComparison.OrdinalIgnoreCase);
                strBody = strBody.Replace(strBody.Substring(lInitPos, System.Convert.ToInt32((strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase) - lInitPos) + lFinalPos + endPt.Length)), "");
                strTemp = strBody;
            }
        }
        returnValue = strBody;
        return returnValue;
    }
    private string GetTaskWorkAreaTitleBar(string strBody)
    {
        string returnValue;
        string findPattern = "WorkareaTitlebar"; // it might be id or class attribute, it might come with quotes too.
        int lInitPos;
        string strFin = "";

        if (strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase) > -1) //need to ignore case, FF and IE return different cases for Tag name.
        {
            lInitPos = strBody.IndexOf(">", strBody.IndexOf(findPattern, System.StringComparison.OrdinalIgnoreCase));
            if (lInitPos > -1)
            {
                strFin = strBody.Substring(lInitPos + 1, System.Convert.ToInt32(strBody.IndexOf("</SPAN>", System.StringComparison.OrdinalIgnoreCase) - lInitPos - 1));
            }
        }
        returnValue = HttpUtility.HtmlDecode(strFin);
        return returnValue;
    }
    private bool SendEmail()
    {
        string strMailBody = "";
        string strRptTitle = "";
        bool bIsTaskRpt = false;
        bool bIsRptTitleFound = false;
        int llpos = 0;
        string strtemp = "";
        int lid = 0;
        int llanguage = 0;
        string strHttpType = HttpContext.Current.Request.Url.Scheme + "://";

        try
        {
            if (strTargUserEmail.ToString() != "")
            {
                objMailServ = m_refSiteApi.EkMailRef;

                objMailServ.MailTo = strTargUserEmail.ToString();
                objMailServ.MailFrom = m_refMail.GetSendersEmailAddress();

                if (objMailServ.MailFrom == "")
                {
                    if (!(Request.Form["frmEmailAddr"] == null))
                    {
                        objMailServ.MailFrom = Request.Form["frmEmailAddr"];
                    }
                    else
                    {
                        if (Request.QueryString["sndrEmail"] == null)
                        {
                            EmailData.Text = "<table><tr><td><input type=\"text\" id=\"frmEmailAddr\"></td><td><font color=\"red\">" + m_refMsg.GetMessage("msg enter email") + "</font></td></tr></table>";
                            return (true);
                        }
                        else
                        {
                            objMailServ.MailFrom = Request.QueryString["sndrEmail"];
                        }
                    }
                }

                if (!(Request.Form["attRptHtml"] == null))
                {
                    strMailBody = (string)(Request.Form["attRptHtml"].ToString());
                }

                if (!(Request.Form["setRptTitle"] == null))
                {
                    if (!(Request.QueryString["rptstatus"] == null) && "siteupdateactivity" == Request.QueryString["rptstatus"])
                    {
                        bIsTaskRpt = false;
                        objMailServ.MailSubject = "Site Activity Report";
                        bIsRptTitleFound = true;
                    }
                    else if ((Request.Form["setRptTitle"].IndexOf("Content") == -1) && (Request.Form["setRptTitle"].IndexOf("Refresh") == -1) && (Request.Form["setRptTitle"].IndexOf("Site") == -1) && (Request.Form["setRptTitle"].IndexOf("Asynchronous") == -1) && (Request.Form["setRptTitle"].IndexOf("Preapproval") == -1))
                    {
                        // It is the Task report. Get title from the MailBody
                        strRptTitle = GetTaskWorkAreaTitleBar(strMailBody);
                        bIsTaskRpt = true;
                        objMailServ.MailSubject = strRptTitle;
                        bIsRptTitleFound = true;
                    }
                    else if (!(Request.Form["setRptTitle"].IndexOf("Preapproval") == -1))
                    {
                        strRptTitle = "Content Reports: Preapproval Groups Report";
                        bIsTaskRpt = true;
                        objMailServ.MailSubject = strRptTitle;
                        bIsRptTitleFound = true;
                    }
                    else
                    {
                        // It is the content report. Should have a report title from LoadUserListChildPage.
                        if (Request.Form["setRptTitle"] != "")
                        {
                            objMailServ.MailSubject = Request.Form["setRptTitle"];
                            bIsRptTitleFound = true;
                        }
                    }
                }

                if ((!(bIsRptTitleFound)) || (objMailServ.MailSubject.Trim() == ""))
                {
                    //In case no title found, add a default one to avoid send mail failure
                    if (m_sRptStatus == "viewsearchphrasereport")
                    {
                        objMailServ.MailSubject = "Search Phrase Report";
                    }
                    else
                    {
                        objMailServ.MailSubject = "Report";
                    }
                }

                string msgNoData = m_refMsg.GetMessage("msg no data report");
                if (strMailBody != "")
                {
                    if (bIsTaskRpt)
                    {
                        // remove checkbox and images from the report
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "checkbox", "<INPUT", ">"));
                        // Remove images from task reports & set task report title exclude the reports containing word content, site, refresh

                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "<IMG", "<IMG", ">"));
                        // remove extra email button and other stuff from task report
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "class=title-header", "<TABLE", ""));
                        // remove the links on the user names, inline javascript functions, hidden variables
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "href=\"#\"", "<A", ">"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "AddShownTaskID(", "<SCRIPT", "SCRIPT>"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "text/javascript", "<SCRIPT", "SCRIPT>"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "type=hidden", "<INPUT", ">"));
                        // remove links on the titles that do sorting
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "sort\">", "<A", ">"));
                        //remove invisible fields
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "style=\"DISPLAY: none\">", "<TR", "/TR>"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "ektronTitlebar", "<DIV", "/DIV>"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "setTaskStateForSelTasks(", "<TABLE", "/TABLE>"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "RefreshTasksWithTaskType(", "<TABLE", "/TABLE>"));

                        // replace the occurrences of any aspx file included in the link to be the real path
                        strMailBody = strMailBody.Replace("tasks.aspx?", strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "tasks.aspx?");
                        strMailBody = strMailBody.Replace("content.aspx?", strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "content.aspx?");
                        strMailBody = strMailBody.Replace("tasks.aspx?", strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "tasksaction.aspx?");
                        strMailBody = strMailBody.Replace("users.aspx?", strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "users.aspx?");
                        if (m_sRptStatus == "viewsearchphrasereport" && strMailBody.IndexOf("<TR", System.StringComparison.OrdinalIgnoreCase) == strMailBody.LastIndexOf("<TR", System.StringComparison.OrdinalIgnoreCase))
                        {
                            strMailBody = msgNoData;
                        }
                    }
                    else
                    {
                        //This is the content report
                        // remove the links for the Last Editor and Path.
                        strMailBody = strMailBody.Replace("src=\"" + m_refUserApi.AppImgPath, "src=" + strHttpType + m_refUserApi.RequestInformationRef.HostUrl + m_refUserApi.AppImgPath);
                        var ImgUIPath = m_refUserApi.ApplicationPath + "images/ui/icons/";
                        strMailBody = strMailBody.Replace("src=\"" + ImgUIPath, "src=" + strHttpType + m_refUserApi.RequestInformationRef.HostUrl + ImgUIPath);
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "checkbox", "<INPUT", ">"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "href=\"reports.aspx?", "<A", ">"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "LoadEmailChildPage(", "<A ", ">"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "selectAllOrNoneWrapper", "<DIV ", "DIV>"));

                        //remove inline script functions and hidden fields
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "SCRIPT>", "<SCRIPT", "SCRIPT>"));
                        strMailBody = (string)(RemoveItemsFromReport(strMailBody, "type=hidden", "<INPUT", ">"));
                        strMailBody = strMailBody.Replace("reports.aspx?", strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "workarea.aspx?page=reports.aspx&");

                        // Replace the occurrences of content.aspx & cmsform.aspx with the full server Url.
                        //strMailBody = strMailBody.Replace("reports.aspx?", "http://" & m_refUserApi.RequestInformationRef.HostUrl.ToString() & m_refUserApi.RequestInformationRef.ApplicationPath.ToString() & "workarea.aspx?page=reports.aspx&")
                        //strMailBody = strMailBody.Replace("content.aspx?", "http://" & m_refUserApi.RequestInformationRef.HostUrl.ToString() & m_refUserApi.RequestInformationRef.ApplicationPath.ToString() & "workarea.aspx?page=content.aspx&")
                        //strMailBody = strMailBody.Replace("cmsform.aspx?", "http://" & m_refUserApi.RequestInformationRef.HostUrl.ToString() & m_refUserApi.RequestInformationRef.ApplicationPath.ToString() & "workarea.aspx?page=cmsform.aspx&")

                        strMailBody = strMailBody.Replace("reports.aspx?", strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "reports.aspx?");

                        llpos = strMailBody.IndexOf("content.aspx?");
                        while (llpos > -1)
                        {
                            strtemp = strMailBody.Substring(llpos, System.Convert.ToInt32(strMailBody.Substring(llpos, strMailBody.Length - llpos).IndexOf(">")));
                            llpos = strtemp.IndexOf("id");
                            lid = int.Parse(strtemp.Substring(llpos + 3, System.Convert.ToInt32(strtemp.Substring(llpos, strtemp.Length - llpos).IndexOf("&") - 3)));
                            strMailBody = strMailBody.Replace(strtemp, strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "linkview.aspx?id=" + lid + "&viewStatus=" + m_sRptStatus + "&fromEmail=1" + "\"");
                            llpos = strMailBody.IndexOf("content.aspx?");
                        }

                        llpos = strMailBody.IndexOf("cmsform.aspx?");
                        while (llpos > -1)
                        {
                            strtemp = strMailBody.Substring(llpos, System.Convert.ToInt32(strMailBody.Substring(llpos, strMailBody.Length - llpos).IndexOf(">")));
                            llpos = strtemp.IndexOf("id");
                            lid = int.Parse(strtemp.Substring(llpos + 3, System.Convert.ToInt32(strtemp.Substring(llpos, strtemp.Length - llpos).IndexOf("&") - 3)));
                            strMailBody = strMailBody.Replace(strtemp, strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "linkview.aspx?ekfrm=" + lid + "&viewStatus=" + m_sRptStatus + "&fromEmail=1" + "\"");
                            llpos = strMailBody.IndexOf("cmsform.aspx?");
                        }

                        if (Request.Form["setRptTitle"].IndexOf("Approval") > -1)
                        {
                            strMailBody = (string)(RemoveItemsFromReport(strMailBody, "fldid", "<A", ">"));
                            llpos = strMailBody.IndexOf("approval.aspx?");
                            string rptContnet = "";
                            while (llpos > -1)
                            {
                                strtemp = strMailBody.Substring(llpos, System.Convert.ToInt32(strMailBody.Substring(llpos, strMailBody.Length - llpos).IndexOf(">")));
                                llpos = strtemp.IndexOf("id");
                                if (strMailBody.IndexOf("rptType") > -1)
                                {
                                    lid = int.Parse(strtemp.Substring(llpos + 3, System.Convert.ToInt32(strtemp.Substring(llpos, strtemp.Length - llpos).IndexOf("&") - 3)));
                                    llpos = strtemp.ToLower().IndexOf("langtype");
                                    if (llpos != -1)
                                    {
                                        llanguage = int.Parse(strtemp.Substring(llpos + 9, System.Convert.ToInt32(strtemp.Substring(llpos, strtemp.Length - llpos).IndexOf("&") - 9)));
                                    }
                                    llpos = strtemp.IndexOf("rptType");

                                    rptContnet = strtemp.Substring(llpos + 8, strtemp.Length - llpos - 9);
                                    if (rptContnet == "2")
                                    {
                                        strMailBody = strMailBody.Replace(strtemp, strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "linkview.aspx?ekfrm=" + lid + "&LangType=" + llanguage + "&viewStatus=viewApproval" + "&fromEmail=1" + "\"");
                                    }
                                    else
                                    {
                                        strMailBody = strMailBody.Replace(strtemp, strHttpType + m_refUserApi.RequestInformationRef.HostUrl.ToString() + m_refUserApi.RequestInformationRef.ApplicationPath.ToString() + "linkview.aspx?Id=" + lid + "&LangType=" + llanguage + "&viewStatus=viewApproval" + "&fromEmail=1" + "\"");
                                    }
                                }

                                llpos = strMailBody.IndexOf("approval.aspx?");
                            }
                        }
                    }
                }
                else
                {
                    strMailBody = msgNoData;
                }

                objMailServ.MailBodyText = strMailBody; //Request.Form("attRptHtml").ToString

                if (objMailServ.MailFrom != "")
                {
                    if (!(Request.QueryString["sndrEmail"] == null))
                    {
                        // We got the email address from the Email Data text box.
                        // Set the EmailData to "" to close the IFrame
                        EmailData.Text = "";
                    }
                    objMailServ.SendMail();
                    objMailServ = null;
                    // Mail Sent Successfully
                    EmailData.Text = "";
                }
                else
                {
                    throw (new Exception(m_refMsg.GetMessage("Error: Email missing source address")));
                }

            }
            else
            {
                throw (new Exception(m_refMsg.GetMessage("error: email missing target address")));
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
        //' exiting the function restores default error handling (crash script).
        //' alternately could do explicitly: on error goto 0
    }
    private void SelectUserGroupToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (m_strPageAction.ToLower().IndexOf("search") > -1)
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("Search user email report"));
        }
        else if (m_strPageAction.ToLower().IndexOf("report") > -1)
        {
            if ("siteupdateactivity_siteRpt" == m_sRptStatus)
            {
                divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl select user or group"));
            }
            else
            {
                divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt Select user email report"));
            }
        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("alt Select User or Group For Task"));
        }
        result.Append("<table><tr>");

		bool addHelpDivider = false;

		if (m_strPageAction.ToLower() != "report")
		{
			// Do not add back button for first window that shows all users i.e. with PageAction = "Report"
			// If the page action is search history.back
			if (m_strPageAction.ToLower().IndexOf("search") > -1)
			{
				if (m_strPageAction.ToLower().IndexOf("inputsearch") > -1)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/back.png", "selectusergroup.aspx?action=Report", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				}
				else
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/back.png", Request.ServerVariables["HTTP_REFERER"], m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
				}
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/back.png", "javascript:top.close();", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			addHelpDivider = true;
		}

        if (m_strPageAction != "")
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/cancel.png", "#", m_refMsg.GetMessage("alt Exit without selecting user"), m_refMsg.GetMessage("close title"), "Onclick=\"javascript:CloseEmailChildPage();\"", StyleHelper.SaveButtonCssClass, true));
            //result.Append("<input type=""button"" onclick=""submit_form()"" value=""" & m_refMsg.GetMessage("send email button text") & """ name=""send"" id=""send"">")
            //result.Append("<input type=""button"" onclick=""CloseEmailChildPage()"" value=""" & m_refMsg.GetMessage("generic Cancel") & """ name=""cancel"" id=""cancel"">")
            if ("siteupdateactivity_siteRpt" == m_sRptStatus)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/save.png", "#", m_refMsg.GetMessage("lbl click here to save the event"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SaveSelUserList();\"", StyleHelper.SaveButtonCssClass, true));
            }
            else
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/email.png", "#", m_refMsg.GetMessage("Email Report button text"), m_refMsg.GetMessage("btn email"), "Onclick=\"javascript:submit_form();\"", StyleHelper.EmailButtonCssClass, true));
            }
            
            if (m_strPageAction.ToLower().IndexOf("search") == -1 && m_sRptStatus != "siteupdateactivity_siteRpt")
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/icons/magnifier.png", "selectusergroup.aspx?action=InputSearch", m_refMsg.GetMessage("alt Search for user"), m_refMsg.GetMessage("btn search"), "", StyleHelper.SearchButtonCssClass));
            }

			addHelpDivider = true;
        }
        
		if (addHelpDivider)
		{
			result.Append(StyleHelper.ActionBarDivider);
		}

        result.Append("<td>");

        if (m_strPageAction.ToLower().IndexOf("report") > -1)
        {
            result.Append(m_refStyle.GetHelpButton("Selectemail", ""));
        }
        else if (m_strPageAction.ToLower().IndexOf("search") > -1)
        {
            result.Append(m_refStyle.GetHelpButton("Searchemail", ""));
        }

        result.Append("</td></tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
		Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "java/stylehelper.js", "EktronSyleHelperJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "ewebeditpro/eweputil.js", "EktronEWepUtilJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refSiteApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
    private void SetJsServerVariables()
    {
        ltr_rptStatus.Text = Request.QueryString["rptStatus"];
        ltr_emailError.Text = m_refMsg.GetMessage("email error: No users selected to receive email");
        ltr_emailFromError.Text = m_refMsg.GetMessage("js: alert invalid email address");
    }
}
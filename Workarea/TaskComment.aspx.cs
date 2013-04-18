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
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Content;
using Ektron.Cms.Common;

public partial class TaskComment : System.Web.UI.Page
{
    protected Ektron.ContentDesignerWithValidator ctlEditor;
    protected bool g_isIEFlagInitialized = false;
    protected bool g_isIEFlag;
    protected SiteAPI m_siteRef = new SiteAPI();
    protected string RefType;
    protected string OrderBy;
    protected string ActionType;
    protected string Action;
    protected long CommentKeyId;
    protected long CommentId;
    protected long RefId;
    protected string AppPath;
    protected void Page_Init(object sender, System.EventArgs e)
    {
        ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("Controls/Editor/ContentDesignerWithValidator.ascx");
        ltr_sig.Controls.Add(ctlEditor);
        ctlEditor.ID = "commenttext";
        ctlEditor.AllowScripts = false;
        ctlEditor.Height = new Unit(300, UnitType.Pixel);
        ctlEditor.Width = new Unit(100, UnitType.Percentage);
        ctlEditor.Stylesheet = m_siteRef.AppPath + "csslib/ewebeditprostyles.css";
        ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
        ctlEditor.AllowFonts = true;
        ctlEditor.ShowHtmlMode = false;
    }
    protected void Page_Load(System.Object sender, System.EventArgs e)
    {
        ApplicationAPI AppUI = new ApplicationAPI();
        long CurrentUserID;
        EkContent cObj1 =  new EkContent(AppUI.RequestInformationRef);
        Collection cComments;
        object retVal;
        string CommentText;
        string ErrorString = "";
        int iMaxContLength;
        string AppName;
        string AppeWebPath;
        int ContentLanguage;
        int EnableMultilingual;
        string platform;
        object IsMac;
        
        string AppImgPath = "";
        EkMessageHelper MsgHelper;

        MsgHelper = (new CommonApi()).EkMsgRef;

        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        ltrScript.Text = (new StyleHelper()).GetClientScript();
        if (m_siteRef.RequestInformationRef.IsMembershipUser == 1)
        {
            Response.Redirect("reterror.aspx?info=" + MsgHelper.GetMessage("msg login cms user"), false);
            return;
        }
        ContentLanguage = -1;
        if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (!string.IsNullOrEmpty(AppUI.GetCookieValue("LastValidLanguageID").ToString()))
            {
                ContentLanguage = Convert.ToInt32(AppUI.GetCookieValue("LastValidLanguageID"));
            }
        }
        AppUI.ContentLanguage = ContentLanguage;
        m_siteRef.RequestInformationRef.ContentLanguage = ContentLanguage;
        EnableMultilingual = AppUI.EnableMultilingual;
        cObj1 = AppUI.EkContentRef;
        CurrentUserID = AppUI.UserId;
        AppName = AppUI.AppName;
        AppeWebPath = AppUI.AppeWebPath;
        AppImgPath = AppUI.AppImgPath;
        AppPath = AppUI.AppPath;
        RefType = EkFunctions.HtmlEncode(Request["ref_type"]);
        iMaxContLength = 65000;
        if ("" == Request["commentkey_id"])
        {
            CommentKeyId = 0;
        }
        else
        {
            CommentKeyId = Convert.ToInt64(Request["commentkey_id"]);
        }
        Action = EkFunctions.HtmlEncode(Request.QueryString["action"]);
        ActionType = EkFunctions.HtmlEncode(Request.QueryString["ty"]);
        if ("" == Request["Comment_Id"])
        {
            CommentId = 0;
        }
        else
        {
            CommentId = Convert.ToInt64(Request["Comment_Id"]);
        }
        RefId = System.Convert.ToInt64(Request["ref_id"]);
        OrderBy = EkFunctions.HtmlEncode(Request["orderby"]);
        platform = Request.ServerVariables["HTTP_USER_AGENT"];
        if (platform.ToString().IndexOf("Windows") + 1 > 0)
        {
            IsMac = 0;
        }
        else
        {
            IsMac = 1;
        }
        this.Title = AppName + " Comments";
        ltrCancel.Text = MsgHelper.GetMessage("generic cancel");
        if (Action == null || "Add" == Action || "" == Action)
        {
            ltrSubmit.Text = MsgHelper.GetMessage("btn insert");
        }
        else if ("Edit" == Action)
        {
            ltrSubmit.Text = MsgHelper.GetMessage("btn update");
        }

        this.ctlEditor.ErrorMessage = MsgHelper.GetMessage("content size exceeded");
        this.ctlEditor.ValidationExpression = Utilities.BuildRegexToCheckMaxLength(System.Convert.ToInt32(iMaxContLength));

        CommentText = "";
        if (IsPostBack)
        {
            this.ctlEditor.Validate();
            if (this.ctlEditor.IsValid)
            {
                if (Action == null || "Add" == Action)
                {
                    CommentText = this.ctlEditor.Content;
                    retVal = cObj1.AddComment(CommentKeyId, CommentId, RefId, RefType, CurrentUserID, CommentText);
                    if (ErrorString != "")
                    {
                        Response.Redirect("../reterror.aspx?info=" + ErrorString);
                    }
                }
                else if ("Update" == Action)
                {
                    CommentText = this.ctlEditor.Content;
                    retVal = cObj1.UpdateComment(CommentId, CommentText);
                    if (ErrorString != "")
                    {
                        Response.Redirect("../reterror.aspx?info=" + ErrorString);
                    }
                }
                Response.Write("<script type=\"text/javascript\">" + "\r\n");
                Response.Write("<!--" + "\r\n");
                Response.Write("if (opener != null)" + "\r\n");
                Response.Write("{" + "\r\n");
                Response.Write("window.top.opener.location.href = window.top.opener.location.href;" + "\r\n");
                Response.Write("}" + "\r\n");
                Response.Write("self.close();" + "\r\n");
                Response.Write("//-->" + "\r\n");
                Response.Write("</script>");
            }
        }
        else
        {
            if ("Edit" == Action)
            {
                cComments = cObj1.GetAllComments(CommentKeyId, CommentId, RefId, RefType, CurrentUserID, "");
                if (ErrorString != "")
                {
                    Response.Redirect("../reterror.aspx?info=" + ErrorString);
                }
                for (int i = 1; i <= cComments.Count; i++)
                {
                    Collection coll = (Collection)cComments[i];
                    CommentText = coll["COMMENTS_TEXT"].ToString();
                }
                
                this.ctlEditor.Content = CommentText.ToString();
            }
        }
    }
    public bool IsBrowserIE()
    {
        bool returnValue;
        if (!(g_isIEFlagInitialized))
        {
            string str;
            str = Request.ServerVariables["HTTP_USER_AGENT"];
            g_isIEFlag = System.Convert.ToBoolean(str.IndexOf("MSIE") + 1 > 0);
            g_isIEFlagInitialized = true;
        }
        returnValue = g_isIEFlag;
        return returnValue;
    }
}

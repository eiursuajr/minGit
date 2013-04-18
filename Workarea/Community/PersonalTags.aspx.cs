using System;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;

public partial class Community_PersonalTags : workareabase
{
    protected string m_action = "";
    protected string m_mode;
    protected string m_appImgPath;
    protected int m_ContentLanguage;
    protected long m_userId;
    protected long m_id;
    protected UserAPI m_refUserApi;
    protected CommonApi m_refCommonAPI;

    public Community_PersonalTags()
    {
        m_refStyle = new StyleHelper();
        m_mode = "";
        m_userId = 0;
        m_id = 0;
        m_refUserApi = new UserAPI();
        m_refCommonAPI = new CommonApi();
        m_refContentApi = new ContentAPI();
        m_refMsg = RefCommonAPI.EkMsgRef;

        Utilities.SetLanguage(m_refCommonAPI);
    }

    public StyleHelper RefStyle
    {
        get
        {
            return (m_refStyle);
        }
        set
        {
            m_refStyle = value;
        }
    }
    public EkMessageHelper RefMsg
    {
        get
        {
            return (m_refMsg);
        }
        set
        {
            m_refMsg = value;
        }
    }
    public UserAPI RefUserApi
    {
        get
        {
            return (m_refUserApi);
        }
        set
        {
            m_refUserApi = value;
        }
    }
    public CommonApi RefCommonAPI
    {
        get
        {
            return (m_refCommonAPI);
        }
        set
        {
            m_refCommonAPI = value;
        }
    }
    public ContentAPI RefContentApi
    {
        get
        {
            return (m_refContentApi);
        }
        set
        {
            m_refContentApi = value;
        }
    }

    public string Action
    {
        get
        {
            return (m_action);
        }
        set
        {
            m_action = value;
        }
    }

    public string Mode
    {
        get
        {
            return (m_mode);
        }
        set
        {
            m_mode = value;
        }
    }

    public new int ContentLanguage
    {
        get
        {
            return (m_refCommonAPI.ContentLanguage);
        }
        set
        {
            m_refCommonAPI.ContentLanguage = value;
        }
    }

    public long UserId
    {
        get
        {
            return (m_userId);
        }
        set
        {
            m_userId = value;
        }
    }

    public long TagId
    {
        get
        {
            return (m_id);
        }
        set
        {
            m_id = value;
        }
    }

    public new string AppImgPath
    {
        get
        {
            return (m_appImgPath);
        }
        set
        {
            m_appImgPath = value;
        }
    }

    protected bool CheckAccess()
    {
        if (this.m_refContentApi.IsLoggedIn)
        {
            if (this.m_iID > 0 && this.m_sPageAction == "delete")
            {
                Ektron.Cms.Common.EkEnumeration.GroupMemberStatus mMemberStatus;
                mMemberStatus = this.m_refCommunityGroupApi.GetGroupMemberStatus(this.m_iID, this.m_refContentApi.UserId);
                return (this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommunityGroupAdmin) || mMemberStatus == Ektron.Cms.Common.EkEnumeration.GroupMemberStatus.Leader);
            }
            else // if logged in, can see this
            {
                return true;
            }
        }

        return false;
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        System.Web.UI.UserControl ctl = null;
        try
        {
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            if (!CheckAccess())
            {
                throw (new Exception(this.GetMessage("err communityaddedit no access")));
            }

            RegisterResources();
            ptagsJSContainerLit.Text = RefStyle.GetClientScript();
            UserId = RefCommonAPI.RequestInformationRef.UserId;
            AppImgPath = RefUserApi.AppImgPath;
            ContentLanguage = RefCommonAPI.RequestInformationRef.ContentLanguage;
            if (!(Request.QueryString["action"] == null))
            {
                Action = (string)(Request.QueryString["action"].ToLower());
            }
            if (!(Request.QueryString["mode"] == null))
            {
                Mode = (string)(Request.QueryString["mode"].ToLower());
            }
            if (!(Request.QueryString["id"] == null))
            {
                TagId = Convert.ToInt64(Request.QueryString["id"].ToLower());
            }

            switch (m_action)
            {
                case "addtag":
                    ctl = (System.Web.UI.UserControl)LoadControl("../Controls/Community/PersonalTags/EditTag.ascx");
                    break;
                case "edittag":
                    ctl = (System.Web.UI.UserControl)LoadControl("../Controls/Community/PersonalTags/EditTag.ascx");
                    break;
                //CType(ctl, controls_Community_PersonalTags_EditTag).TagId =
                case "viewall":
                    ctl = (System.Web.UI.UserControl)LoadControl("../Controls/Community/PersonalTags/ViewAllTags.ascx");
                    break;
                case "viewtag":
                    ctl = (System.Web.UI.UserControl)LoadControl("../Controls/Community/PersonalTags/ViewTag.ascx");
                    break;
                case "viewdefaulttags":
                    ctl = (System.Web.UI.UserControl)LoadControl("../Controls/Community/PersonalTags/TagDefaults.ascx");
                    break;
                default:
                    ctl = (System.Web.UI.UserControl)LoadControl("../Controls/Community/PersonalTags/ViewAllTags.ascx");
                    break;
            }
            if (ctl != null)
            {
                FindControl("PTagsCtlHolder").Controls.Add(ctl);
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message + ex.StackTrace);
        }
    }
    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronCommunityCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);

    }
}


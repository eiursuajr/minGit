using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.API.Content;

partial class Workarea_LocaleTaxonomy : System.Web.UI.Page
{

    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected string m_strPageAction = "";
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected CommonApi m_refCommon = new CommonApi();
    protected int EnableMultilingual = 0;
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected long m_intTaxonomyId = 0;
    protected int m_intTaxonomyLanguage = 1033;
    protected int TaxonomyLanguage = -1;
    protected bool AddDeleteIcon = false;
    protected long CurrentUserId = 0;
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected string m_strSearchText = "";
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();

    protected ContentAPI m_refContentApi = new ContentAPI();


    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {            
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
            CurrentUserId = m_refCommon.RequestInformationRef.UserId;
            m_refContent = m_refCommon.EkContentRef;
            m_refMsg = m_refCommon.EkMsgRef;
            RegisterResources();            
            Utilities.ValidateUserLogin();
            if ((m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || (!m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminXliff))) && (!m_refContentApi.IsAdmin()))
            {
                Response.Redirect("../reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login xliff administrator"), false);
                return;
            }
            else
            {
                AppImgPath = m_refCommon.AppImgPath;  
                AppPath = m_refCommon.AppPath;
                litAppPath.Text = AppPath;
                EnableMultilingual = m_refCommon.EnableMultilingual;
                displaystylesheet.Text = m_refStyle.GetClientScript();
                if ((Request.QueryString["action"] != null && !string.IsNullOrEmpty(Request.QueryString["action"])))
                {
                    m_strPageAction = Request.QueryString["action"].ToLower();
                }
                Utilities.SetLanguage(m_refCommon);

                TaxonomyLanguage = m_refCommon.ContentLanguage;
                if ((Page.IsPostBack))
                {
                    if (((Request.Form[submittedaction.UniqueID]) == "deleteSelected"))
                    {
                        TaxonomyRequest taxonomy_request_data = new TaxonomyRequest();
                        taxonomy_request_data.TaxonomyIdList = Request.Form["selected_taxonomy"];
                        taxonomy_request_data.TaxonomyLanguage = TaxonomyLanguage;
                        m_refContent.DeleteTaxonomy(taxonomy_request_data);
                        Response.Redirect("LocaleTaxonomy.aspx?rf=1", false);
                    }
                }
                switch (m_strPageAction)
                {
                    case "add":
                        addLocaleTaxonomy m_at = default(addLocaleTaxonomy); 
                        m_at = (addLocaleTaxonomy)LoadControl("../controls/localetaxonomy/addLocaleTaxonomy.ascx");
                        m_at.ID = "taxonomy";
                        DataHolder.Controls.Add(m_at);
                        break;
                
                    case "edit":
                    case "reorder":
                        editLocaleTaxonomy m_et = default(editLocaleTaxonomy);
                        m_et = (editLocaleTaxonomy)LoadControl("../controls/localetaxonomy/editLocaleTaxonomy.ascx");
                        m_et.ID = "taxonomy";
                        DataHolder.Controls.Add(m_et);
                        break;
                    case "view":
                        viewLocaletaxonomy m_vt = default(viewLocaletaxonomy);
                        m_vt = (viewLocaletaxonomy)LoadControl("../controls/localetaxonomy/viewLocaletaxonomy.ascx");
                        m_vt.ID = "taxonomy";
                        DataHolder.Controls.Add(m_vt);
                        break;                        
                    case "viewcontent":
                    case "removeitems":
                        Control m_vi = default(Control);

                        m_vi = (Control)LoadControl("../controls/localetaxonomy/viewLocaleTaxonomyItems.ascx");
                        m_vi.ID = "taxonomy";
                        DataHolder.Controls.Add(m_vi);
                        break;                        
                    case "viewattributes":
                        Control m_va = default(Control);
                        m_va = (Control)LoadControl("../controls/localetaxonomy/viewLocaleTaxonomyAttributes.ascx");
                        m_va.ID = "taxonomy";
                        DataHolder.Controls.Add(m_va);
                        break;
                    case "additem":
                    case "addfolder":
                        if ((m_strPageAction == "addfolder"))
                        {
                            //body.Attributes.Add("onload", "Main.start();displayTreeFolderSelect();showSelectedFolderTree();setupClassNames();")
                            body.Attributes.Add("onload", "Main.start();displayTreeFolderSelect();showSelectedFolderTree();setupClassNames();");
                        }

                        assignLocaleTaxonomy m_asnt = default(assignLocaleTaxonomy);
                        m_asnt = (assignLocaleTaxonomy)LoadControl("../controls/localetaxonomy/assignLocaleTaxonomy.ascx");
                        m_asnt.ID = "taxonomy";
                        DataHolder.Controls.Add(m_asnt);

                        break;
                    default:
                        div_taxonomylist.Visible = true;
                        if ((IsPostBack == false || (IsPostBack == true & !string.IsNullOrEmpty(Request.Form[isSearchPostData.UniqueID]))))
                        {
                            //ViewAllTaxonomy();
                            ViewAllToolBar();
                        }
                        break;
                }

				//if ((Request.QueryString["rf"] == "1"))
				//{
				//    litRefreshAccordion.Text = "<script language=\"javascript\">" + "\n" + "top.refreshTaxonomyAccordion(" + TaxonomyLanguage + ");" + "\n" + "</script>" + "\n";
				//}

            }
            SetJsServerVariables();            
        }
        catch (System.Threading.ThreadAbortException)
        {
            //Do nothing
        }
        catch (Exception ex)
        {
            Response.Redirect("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message + ".") + "&LangType=" + TaxonomyLanguage, false);
        }
    }

    private void ViewAllToolBar()
    {
        bool addBreak = false;
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view all locale taxonomy page title"));
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\n");

		bool primaryCssApplied = false;

        if ((this.TaxonomyLanguage > 0))
        {
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/add.png", "localeTaxonomy.aspx?action=add", m_refMsg.GetMessage("alt add button text (locale taxonomy)"), m_refMsg.GetMessage("btn add locale taxonomy"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;
            addBreak = true;
        }

        if ((AddDeleteIcon && this.TaxonomyLanguage > 0))
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text (selectedtaxonomy)"), m_refMsg.GetMessage("btn delete"), "onclick=\"return Delete();\"", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;
            addBreak = true;
        }
        if (addBreak)
            result.Append("<td>&nbsp;|&nbsp;</td>");
        result.Append("<td class=\"label\">");
        result.Append(m_refMsg.GetMessage("lbl View") + ":");
        result.Append("</td>");
        result.Append("<td>");
        result.Append(m_refStyle.ShowAllActiveLanguage(true, "", "javascript:defaultLoadLanguage(this.value);", TaxonomyLanguage.ToString()));
        result.Append("</td>");
        result.Append("<td>&nbsp;</td>");
        result.Append("<td>");
       // result.Append("<label for=\"txtSearch\">" + m_refMsg.GetMessage("generic search") + "</label>");
       // result.Append("<input type=text class=\"ektronTextMedium\" id=txtSearch name=txtSearch value=\"" + m_strSearchText + "\" onkeydown=\"CheckForReturn(event)\">");
        result.Append("</td>");
        result.Append("<td>");
       // result.Append("<input type=button value=" + m_refMsg.GetMessage("btn search") + " id=\"btnSearch\" name=\"btnSearch\"  class=\"ektronWorkareaSearch\" onclick=\"searchtaxonomy();\">");
        result.Append("</td>");
        result.Append("<td>" + m_refStyle.GetHelpButton("TranslationPackages", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }

    private void RegisterResources()
    {
        // register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/jfunct.js", "EktronJFunctJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
        Ektron.Cms.API.JS.RegisterJS(this, m_refCommon.ApplicationPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronSearchBoxInputLabelInitJS");
        // register CSS
        Ektron.Cms.API.Css.RegisterCss(this, m_refCommon.ApplicationPath + "csslib/ektron.fixedPositionToolbar.css", "FixedPosToolbarCss");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        //Ektron.Cms.API.Css.RegisterCss(Me, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.All)
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
    private void SetJsServerVariables()
    {
        ltr_selTaxMsg.Text = m_refMsg.GetMessage("select taxonomy msg");
        ltr_delCnfrmMsg.Text = m_refMsg.GetMessage("delete taxonomy msg");
        ltr_msgSearch.Text = m_refMsg.GetMessage("alert msg search");
        ltr_lang.Text = TaxonomyLanguage.ToString();
        ltr_contLangCnfrmMsg.Text = m_refMsg.GetMessage("alert msg add taxonomy lang");
    }
}
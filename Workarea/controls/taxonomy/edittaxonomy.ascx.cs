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

public partial class edittaxonomy : System.Web.UI.UserControl
{
    protected ContentAPI m_refApi = new ContentAPI();
    protected StyleHelper m_refstyle = new StyleHelper();
    protected string AppImgPath = "";
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "";
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected int TaxonomyLanguage = -1;
    protected long TaxonomyId = 0;
    protected long TaxonomyParentId = 0;
    protected LanguageData language_data;
    protected TaxonomyData taxonomy_data = new TaxonomyData();
    protected TaxonomyRequest taxonomy_request;
    protected bool ShowSaveIcon = true;
    protected string m_strReorderAction = "category";
    protected TaxonomyBaseData[] taxonomy_arr = null;
    protected int TotalItems = 0;
    protected string m_strCurrentBreadcrumb = "";
    protected bool m_bSynchronized = true;
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    protected List<CustomPropertyData> _customPropertyDataList;
    protected Ektron.Cms.Framework.Core.CustomProperty.CustomProperty _customProperty = new Ektron.Cms.Framework.Core.CustomProperty.CustomProperty();
    protected CustomPropertyData _customPropertyData = new Ektron.Cms.Common.CustomPropertyData();
    protected CustomPropertyObjectData _customPropertyObjectData = new Ektron.Cms.Common.CustomPropertyObjectData();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refApi.EkMsgRef;
        AppImgPath = m_refApi.AppImgPath;
        m_strPageAction = Request.QueryString["action"];
        object refAPI = m_refApi as object;
        Utilities.SetLanguage(m_refApi);
        TaxonomyLanguage = m_refApi.ContentLanguage;
        if (TaxonomyLanguage == -1)
        {
            TaxonomyLanguage = m_refApi.DefaultContentLanguage;
        }
        if (Request.QueryString["taxonomyid"] != null)
        {
            TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
            hdnTaxonomyId.Value = TaxonomyId.ToString();
        }
        if (Request.QueryString["parentid"] != null)
        {
            TaxonomyParentId = Convert.ToInt64(Request.QueryString["parentid"]);
        }
        m_refContent = m_refApi.EkContentRef;
        if (Request.QueryString["reorder"] != null)
        {
            m_strReorderAction = Convert.ToString(Request.QueryString["reorder"]);
        }
        chkApplyDisplayAllLanguages.Text = m_refMsg.GetMessage("lbl apply display taxonomy languages");
        chkConfigContent.Text = chkConfigContent.ToolTip = m_refMsg.GetMessage("content text");
        chkConfigUser.Text = chkConfigUser.ToolTip = m_refMsg.GetMessage("generic user");
        chkConfigGroup.Text = chkConfigGroup.ToolTip = m_refMsg.GetMessage("lbl group");
        if (Page.IsPostBack)
        {
            if (m_strPageAction == "edit")
            {
                taxonomy_data.TaxonomyLanguage = TaxonomyLanguage;
                taxonomy_data.TaxonomyParentId = TaxonomyParentId;
                taxonomy_data.TaxonomyId = TaxonomyId;
                taxonomy_data.TaxonomyDescription = Request.Form[taxonomydescription.UniqueID];
                taxonomy_data.TaxonomyName = Request.Form[taxonomytitle.UniqueID];
                taxonomy_data.TaxonomyImage = Request.Form[taxonomy_image.UniqueID];
                taxonomy_data.CategoryUrl = Request.Form[categoryLink.UniqueID];
                if (tr_enableDisable.Visible == true)
                {
                    if (!string.IsNullOrEmpty(Request.Form[chkEnableDisable.UniqueID]))
                    {
                        taxonomy_data.Visible = true;
                    }
                    else
                    {
                        taxonomy_data.Visible = false;
                    }
                }
                else
                {
                    taxonomy_data.Visible = Convert.ToBoolean(Request.Form[visibility.UniqueID]);
                }
                if (Request.Form[inherittemplate.UniqueID] != null)
                {
                    taxonomy_data.TemplateInherited = true;
                }
                if (Request.Form[taxonomytemplate.UniqueID] != null)
                {
                    taxonomy_data.TemplateId = Convert.ToInt64(Request.Form[taxonomytemplate.UniqueID]);
                }
                else
                {
                    taxonomy_data.TemplateId = 0;
                }

                try
                {
                    m_refContent.UpdateTaxonomy(taxonomy_data);
                }
                catch (Exception ex)
                {
                    Utilities.ShowError(ex.Message);
                }
                if ((!(Request.Form[chkApplyDisplayAllLanguages.UniqueID] == null)) && (Request.Form[chkApplyDisplayAllLanguages.UniqueID].ToString().ToLower() == "on"))
                {
                    m_refContent.UpdateTaxonomyVisible(TaxonomyId, -1, taxonomy_data.Visible);
                }
                m_refContent.UpdateTaxonomySynchronization(TaxonomyId, GetCheckBoxValue(chkTaxSynch));
                //else
                //{
                //    m_refContent.UpdateTaxonomyVisible(TaxonomyId, TaxonomyLanguage, taxonomy_data.Visible);
                //}
                if (TaxonomyParentId == 0)
                {
                    string strConfig = string.Empty;
                    if (!string.IsNullOrEmpty(Request.Form[chkConfigContent.UniqueID]))
                    {
                        strConfig = "0";
                    }
                    if (!string.IsNullOrEmpty(Request.Form[chkConfigUser.UniqueID]))
                    {
                        if (string.IsNullOrEmpty(strConfig))
                        {
                            strConfig = "1";
                        }
                        else
                        {
                            strConfig = strConfig + ",1";
                        }
                    }
                    if (!string.IsNullOrEmpty(Request.Form[chkConfigGroup.UniqueID]))
                    {
                        if (string.IsNullOrEmpty(strConfig))
                        {
                            strConfig = "2";
                        }
                        else
                        {
                            strConfig = strConfig + ",2";
                        }
                    }
                    if (!(string.IsNullOrEmpty(strConfig)))
                    {
                        m_refContent.UpdateTaxonomyConfig(TaxonomyId, strConfig);
                    }
                }
                UpdateCustomProperties();

                if (Request.QueryString["iframe"] == "true")
                {
                    Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
                }
                else
                {
                    if ((Request.QueryString["backaction"] != null) && Convert.ToString(Request.QueryString["backaction"]).ToLower() == "viewtree")
                    {
                        Response.Redirect((string)("taxonomy.aspx?action=viewtree&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage), true);
                    }
                    else
                    {
                        Response.Redirect("taxonomy.aspx?action=view&view=item&taxonomyid=" + TaxonomyId + "&rf=1", true);
                    }
                }
            }
            else
            {
                if (Request.Form[LinkOrder.UniqueID] != "")
                {
                    taxonomy_request = new TaxonomyRequest();
                    taxonomy_request.TaxonomyId = TaxonomyId;
                    taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
                    taxonomy_request.TaxonomyIdList = Request.Form[LinkOrder.UniqueID];
                    if (!string.IsNullOrEmpty(Request.Form[chkOrderAllLang.UniqueID]))
                    {
                        if (m_strReorderAction == "category")
                        {
                            m_refContent.ReOrderAllLanguageCategories(taxonomy_request);
                        }
                        else if (m_strReorderAction == "users")
                        {
                            
                                taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User;
                            m_refContent.ReOrderTaxonomyItems(taxonomy_request);
                        }
                    }
                    else
                    {
                        if (m_strReorderAction == "category")
                        {
                            m_refContent.ReOrderCategories(taxonomy_request);
                        }
                        else
                        {
                            if (m_strReorderAction == "users")
                            {
                                taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User;
                            }
                            m_refContent.ReOrderTaxonomyItems(taxonomy_request);
                        }
                    }
                }
                if (Request.QueryString["iframe"] == "true")
                {
                    Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
                }
                else
                {
                    if(m_strReorderAction == "category")
                        Response.Redirect("taxonomy.aspx?action=view&taxonomyid=" + TaxonomyId + "&rf=1&reloadtrees=Tax", true);
                    else
                        Response.Redirect("taxonomy.aspx?action=view&taxonomyid=" + TaxonomyId + "&rf=1", true);
                }
            }
        }
        else
        {
            ltr_sitepath.Text = m_refApi.SitePath;
            taxonomy_request = new TaxonomyRequest();
            taxonomy_request.TaxonomyId = TaxonomyId;
            taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;

            if (m_strPageAction == "edit")
            {
                taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);

                taxonomydescription.Text = taxonomy_data.TaxonomyDescription;
                taxonomydescription.ToolTip = taxonomydescription.Text;
                taxonomytitle.Text = taxonomy_data.TaxonomyName;
                taxonomytitle.ToolTip = taxonomytitle.Text;
                taxonomy_image.Text = taxonomy_data.TaxonomyImage;
                taxonomy_image.ToolTip = taxonomy_image.Text;
                taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage;
                categoryLink.Text = taxonomy_data.CategoryUrl;
                visibility.Value = taxonomy_data.Visible.ToString();
                if (Request.QueryString["taxonomyid"] != null)
                {
                    TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
                }

                if (TaxonomyParentId > 0)
                {
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyParentId, TaxonomyLanguage);
                    if (TaxonomyId > 0)
                        chkTaxSynch.Checked = m_refContent.IsSynchronizedTaxonomy(TaxonomyId, TaxonomyLanguage);
                }
                else if (TaxonomyId > 0)
                {
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyId, TaxonomyLanguage);
                    chkTaxSynch.Checked = m_bSynchronized;
                }
                if (taxonomy_data.Visible == true)
                {
                    chkEnableDisable.Checked = true;
                }
                else
                {
                    chkEnableDisable.Checked = false;
                }
                if (taxonomy_data.TaxonomyImage != "")
                {
                    taxonomy_image_thumb.ImageUrl = (taxonomy_data.TaxonomyImage.IndexOf("/") == 0) ? taxonomy_data.TaxonomyImage : m_refApi.SitePath + taxonomy_data.TaxonomyImage;
                }
                else
                {
                    taxonomy_image_thumb.ImageUrl = m_refApi.AppImgPath + "spacer.gif";
                }
                language_data = (new SiteAPI()).GetLanguageById(TaxonomyLanguage);
                if (taxonomy_data.TaxonomyParentId == 0)
                {
                    inherittemplate.Visible = false;
                    lblInherited.Text = "No";
                    lblInherited.ToolTip = lblInherited.Text;
                    inherittemplate.Checked = taxonomy_data.TemplateInherited;
                }
                else
                {
                    inherittemplate.Visible = true;
                    lblInherited.Text = "";
                    inherittemplate.Checked = taxonomy_data.TemplateInherited;
                    if (taxonomy_data.TemplateInherited)
                    {
                        taxonomytemplate.Enabled = false;
                    }
                }
                TemplateData[] templates = null;
                templates = m_refApi.GetAllTemplates("TemplateFileName");
                taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem("- " + m_refMsg.GetMessage("generic select template") + " -", "0"));
                if ((templates != null) && templates.Length > 0)
                {
                    for (int i = 0; i <= templates.Length - 1; i++)
                    {
                        taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem(templates[i].FileName, templates[i].Id.ToString()));
                        if (taxonomy_data.TemplateId == templates[i].Id)
                        {
                            taxonomytemplate.SelectedIndex = i + 1;
                        }
                    }
                }
                if ((language_data != null) && (m_refApi.EnableMultilingual == 1))
                {
                    lblLanguage.Text = "[" + language_data.Name + "]";
                    lblLanguage.ToolTip = lblLanguage.Text;
                }
                m_strCurrentBreadcrumb = (string)(taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\\", " > "));
                if (m_strCurrentBreadcrumb == "")
                {
                    m_strCurrentBreadcrumb = "Root";
                }
                inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)");
                if (TaxonomyParentId == 0)
                {
                    tr_config.Visible = true;
                    List<int> config_list = m_refApi.EkContentRef.GetAllConfigIdListByTaxonomy(TaxonomyId, TaxonomyLanguage);
                    for (int i = 0; i <= config_list.Count - 1; i++)
                    {
                        if (config_list[i] == 0)
                        {
                            chkConfigContent.Checked = true;
                        }
                        else if (config_list[i] == 1)
                        {
                            chkConfigUser.Checked = true;
                        }
                        else if (config_list[i] == 2)
                        {
                            chkConfigGroup.Checked = true;
                        }
                    }
                }
                else
                {
                    tr_config.Visible = false;
                }

                LoadCustomPropertyList();

            }
            else
            {
                if (m_strReorderAction == "category")
                {
                    taxonomy_request.PageSize = 99999999; // pagesize of 0 used to mean "all"
                    taxonomy_arr = m_refContent.ReadAllSubCategories(taxonomy_request);
                    if (taxonomy_arr != null)
                    {
                        TotalItems = taxonomy_arr.Length;
                    }
                    else
                    {
                        TotalItems = 0;
                    }
                    if (TotalItems > 1)
                    {
                        td_msg.Text = m_refMsg.GetMessage("generic first msg");
                        OrderList.DataSource = taxonomy_arr;
                        OrderList.DataTextField = "TaxonomyName";
                        OrderList.DataValueField = "TaxonomyId";
                        OrderList.DataBind();
                        OrderList.SelectionMode = ListSelectionMode.Multiple;
                        OrderList.CssClass = "width: 100%; border-style: none; border-color: White; font-family: Verdana;font-size: x-small;";
                        if (TotalItems > 20)
                        {
                            OrderList.Rows = 20;
                        }
                        else
                        {
                            OrderList.Rows = TotalItems;
                        }
                        OrderList.Width = 300;
                        if (TotalItems > 0)
                        {
                            loadscript.Text = "document.forms[0].taxonomy_OrderList[0].selected = true;";
                        }
                        for (int i = 0; i <= taxonomy_arr.Length - 1; i++)
                        {
                            if (LinkOrder.Value == "")
                            {
                                LinkOrder.Value = Convert.ToString(taxonomy_arr[i].TaxonomyId);
                            }
                            else
                            {
                                LinkOrder.Value = Convert.ToString(taxonomy_arr[i].TaxonomyId) + ",";
                            }
                        }
                    }
                    else
                    {
                        LoadNoItem();
                    }
                }
                else if (m_strReorderAction == "users")
                {
                    DirectoryUserRequest uReq = new DirectoryUserRequest();
                    DirectoryAdvancedUserData uDirectory = new DirectoryAdvancedUserData();
                    uReq.GetItems = true;
                    uReq.DirectoryId = TaxonomyId;
                    uReq.DirectoryLanguage = TaxonomyLanguage;
                    uReq.PageSize = 99999;
                    uReq.CurrentPage = 1;
                    uDirectory = this.m_refContent.LoadDirectory(ref uReq);
                    TotalItems = uDirectory.DirectoryItems.Count();
                    if (TotalItems > 1)
                    {
                        td_msg.Text = m_refMsg.GetMessage("generic first msg");
                        OrderList.DataSource = uDirectory.DirectoryItems;
                        OrderList.DataTextField = "Username";
                        OrderList.DataValueField = "Id";
                        OrderList.DataBind();
                        OrderList.SelectionMode = ListSelectionMode.Multiple;
                        OrderList.CssClass = "width: 100%; border-style: none; border-color: White; font-family: Verdana;font-size: x-small;";
                        if (TotalItems > 20)
                        {
                            OrderList.Rows = 20;
                        }
                        else
                        {
                            OrderList.Rows = TotalItems;
                        }
                        OrderList.Width = 300;
                        if (TotalItems > 0)
                        {
                            loadscript.Text = "document.forms[0].taxonomy_OrderList[0].selected = true;";
                        }
                        for (int i = 0; i <= TotalItems - 1; i++)
                        {
                            if (LinkOrder.Value == "")
                            {
                                LinkOrder.Value = Convert.ToString(uDirectory.DirectoryItems[i].Id);
                            }
                            else
                            {
                                LinkOrder.Value = Convert.ToString(uDirectory.DirectoryItems[i].Id) + ",";
                            }
                        }
                    }
                    else
                    {
                        LoadNoItem();
                    }
                }
                else
                {
                    AllLangForm.Visible = false; // the all languages checkbox is only valid for categories
                    taxonomy_request.PageSize = 99999999;
                    taxonomy_request.IncludeItems = true;
                    taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);
                    tr_ordering.Visible = true; //Not showing for items
                    if (taxonomy_data.TaxonomyItems != null)
                    {
                        TotalItems = taxonomy_data.TaxonomyItems.Length;
                        if (TotalItems > 1)
                        {
                            td_msg.Text = m_refMsg.GetMessage("generic first msg");
                            OrderList.DataSource = taxonomy_data.TaxonomyItems;
                            OrderList.DataTextField = "TaxonomyItemTitle";
                            OrderList.DataValueField = "TaxonomyItemId";
                            OrderList.DataBind();
                            OrderList.SelectionMode = ListSelectionMode.Multiple;
                            OrderList.CssClass = "width: 100%; border-style: none; border-color: White; font-family: Verdana;font-size: x-small;";
                            if (TotalItems > 20)
                            {
                                OrderList.Rows = 20;
                            }
                            else
                            {
                                OrderList.Rows = TotalItems;
                            }
                            OrderList.Width = 300;
                            if (TotalItems > 0)
                            {
                                loadscript.Text = "document.forms[0].taxonomy_OrderList[0].selected = true;";
                            }
                            foreach (TaxonomyItemData taxonomy_item in taxonomy_data.TaxonomyItems)
                            {
                                if (LinkOrder.Value == "")
                                {
                                    LinkOrder.Value = Convert.ToString(taxonomy_item.TaxonomyItemId);
                                }
                                else
                                {
                                    LinkOrder.Value = Convert.ToString(taxonomy_item.TaxonomyItemId) + ",";
                                }
                            }
                        }
                        else
                        {
                            LoadNoItem();
                        }
                    }
                }
            }
            TaxonomyToolBar();
        }
    }
    private void LoadNoItem()
    {
        ShowSaveIcon = false;
        td_moveicon.Visible = false;
        td_msg.Text = m_refMsg.GetMessage("msg no items available to perform reorder");
        OrderList.Visible = false;
        tr_ordering.Visible = false;
    }
    private void TaxonomyToolBar()
    {
        string _taxName = "";

        if (taxonomy_data != null && taxonomy_data.TaxonomyName != null && taxonomy_data.TaxonomyName != "")
        {
            _taxName = taxonomy_data.TaxonomyName;
        }
        else if (TaxonomyId > 0) //will be called only on reorder items screen. No other way to get the taxonomy name
        {
            taxonomy_request = new TaxonomyRequest();
            taxonomy_request.TaxonomyId = TaxonomyId;
            taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
            TaxonomyData _taxData = m_refContent.ReadTaxonomy(ref taxonomy_request);
            _taxName = _taxData.TaxonomyName;
        }

        if (m_strPageAction == "reorder")
        {
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar((string)(m_refMsg.GetMessage("reorder taxonomy page title") + "&nbsp;&nbsp;\"" + _taxName + "\"&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "\' />"));
        }
        else
        {
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar((string)(m_refMsg.GetMessage("edit taxonomy page title") + "&nbsp;&nbsp;\"" + _taxName + "\"&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "\' />"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();

        result.Append("<table><tr>" + "\r\n");

		if (Request.QueryString["iframe"] == "true")
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onClick=\"javascript:parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true));
		}
		else
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("taxonomy.aspx?action=view&taxonomyid=" + TaxonomyId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (m_strPageAction == "edit")
        {
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=\"javascript:if(SetPropertyIds()){Validate(true);}\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
            if (ShowSaveIcon)
            {
				result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=\"javascript:if(SetPropertyIds()){Validate(false);}\"", StyleHelper.SaveButtonCssClass, true));
            }
        }

        if (m_strPageAction != "edit")
        {
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>" + ReorderDropDown() + "</td>");
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>" + m_refstyle.GetHelpButton("ReOrderTaxonomyOrCategoryItem", "") + "</td>");
        }
        else
        {
			result.Append(StyleHelper.ActionBarDivider);
            result.Append("<td>" + m_refstyle.GetHelpButton("EditTaxonomyOrCategory", "") + "</td>");
        }

        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private string ReorderDropDown()
    {
        StringBuilder result = new StringBuilder();
        result.Append(m_refMsg.GetMessage("type label") + "<select id=\"selreorderitem\" name=\"selreorderitem\" onchange=\"javascript:LoadReorderType(this.value);\">" + "<option value=\"category\" " + FindSelected("category") + ">Category</option>" + "<option value=\"item\"  " + FindSelected("item") + ">Items</option>" + "<option value=\"users\" " + FindSelected("users") + ">Users</option></select>");
        return result.ToString();
    }
    private string FindSelected(string chk)
    {
        string val = "";
        if (m_strReorderAction == chk)
        {
            val = " selected ";
        }
        return val;
    }

    private void LoadCustomPropertyList()
    {

        int i = 0;
        int j = 1;

        PagingInfo pageInfo = new PagingInfo();
        pageInfo.CurrentPage = 1;
        pageInfo.RecordsPerPage = 99999;

        _customPropertyDataList = _customProperty.GetList(EkEnumeration.CustomPropertyObjectType.TaxonomyNode, m_refApi.ContentLanguage, pageInfo);
        availableCustomProp.ToolTip = m_refMsg.GetMessage("li taxonomy select default text");
        availableCustomProp.Items.Insert(0, new ListItem(m_refMsg.GetMessage("li taxonomy select default text"), "-1"));
        for (i = 0; i <= _customPropertyDataList.Count - 1; i++)
        {
            if (_customPropertyDataList[i].IsEnabled)
            {
                availableCustomProp.Items.Insert(j, _customPropertyDataList[i].PropertyName);
                availableCustomProp.Items[j].Value = Convert.ToString(_customPropertyDataList[i].PropertyId);
                j++;
            }
        }

        availableCustomProp.DataBind();

    }

    private void UpdateCustomProperties()
    {
        int i = 0;
        Ektron.Cms.Framework.Core.CustomProperty.CustomProperty cp = new Ektron.Cms.Framework.Core.CustomProperty.CustomProperty();
        Ektron.Cms.Framework.Core.CustomProperty.CustomPropertyObject cpo = new Ektron.Cms.Framework.Core.CustomProperty.CustomPropertyObject();
        System.Collections.Generic.List<Ektron.Cms.Common.CustomPropertyObjectData> listData;
        string[] selectedIds = null;
        string[] selectedValues = null;

        PagingInfo pageInfo = new PagingInfo();
        pageInfo.CurrentPage = 1;
        pageInfo.RecordsPerPage = 99999;

        listData = cpo.GetList(TaxonomyId, m_refApi.ContentLanguage, EkEnumeration.CustomPropertyObjectType.TaxonomyNode, pageInfo);
        if (Request.Form[hdnSelectedIDS.UniqueID] != "")
        {
            selectedIds = Request.Form[hdnSelectedIDS.UniqueID].Remove(System.Convert.ToInt32(Request.Form[hdnSelectedIDS.UniqueID].Length - 1), 1).Split(";".ToCharArray());
        }
        if (Request.Form[hdnSelectValue.UniqueID] != "")
        {
            selectedValues = Request.Form[hdnSelectValue.UniqueID].Remove(System.Convert.ToInt32(Request.Form[hdnSelectValue.UniqueID].Length - 1), 1).Split(";".ToCharArray());
        }
        _customPropertyDataList = _customProperty.GetList(Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType.TaxonomyNode, m_refApi.ContentLanguage, pageInfo);
        for (i = 0; i <= listData.Count - 1; i++)
        {
            cpo.Delete(TaxonomyId, m_refApi.ContentLanguage, EkEnumeration.CustomPropertyObjectType.TaxonomyNode, listData[i].PropertyId);
        }
        if ((selectedIds != null) || (selectedValues != null))
        {
            for (i = 0; i <= selectedIds.Length - 1; i++)
            {
                //var customPropertyData = cp.GetItem(selectedIds[i], m_refApi.ContentLanguage);
                //CustomPropertyObjectData data = new CustomPropertyObjectData(TaxonomyId, m_refApi.ContentLanguage, selectedIds[i], EkEnumeration.CustomPropertyObjectType.TaxonomyNode);
                Ektron.Cms.Common.CustomPropertyData customPropertyData = cp.GetItem(Convert.ToInt64(selectedIds[i].ToString()), m_refApi.ContentLanguage);
                CustomPropertyObjectData data = new CustomPropertyObjectData(TaxonomyId, m_refApi.ContentLanguage, Convert.ToInt64(selectedIds[i].ToString()), EkEnumeration.CustomPropertyObjectType.TaxonomyNode);
                if ((customPropertyData != null) && (data != null))
                {
                    string inputValue = HttpUtility.UrlDecode((string)(selectedValues[i].ToString()));
                    switch (customPropertyData.PropertyDataType)
                    {
                        case EkEnumeration.CustomPropertyItemDataType.Boolean:
                            bool booleanValue;
                            if (bool.TryParse(inputValue, out booleanValue))
                            {
                                data.AddItem(booleanValue);
                            }
                            break;

                        case EkEnumeration.CustomPropertyItemDataType.DateTime:
                            DateTime dateTimeValue;
                            if (DateTime.TryParse(inputValue, out dateTimeValue))
                            {
                                data.AddItem(dateTimeValue);
                            }
                            break;
                        default:
                            data.AddItem(inputValue);
                            break;
                    }
                    cpo.Add(data);
                }
            }
        }
    }

    private bool GetCheckBoxValue(Control ctrl)
    {
        bool isChecked = false;
        if (Request.Form[ctrl.UniqueID] != null &&
            !string.IsNullOrEmpty(Request.Form[ctrl.UniqueID]))
        {
            isChecked = true;
        }
        return isChecked;
    }
}
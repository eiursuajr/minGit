//-----------------------------------------------------------------------
// <copyright file="editLocaleTaxonomy.ascx.cs" company="Ektron" author="Rama Ila">
// Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
// This user control will Edit the Provided Translation Package.
//-----------------------------------------------------------------------
using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.Common;


partial class editLocaleTaxonomy : System.Web.UI.UserControl
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
    protected string _ViewItem = "item";
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
    protected List<Ektron.Cms.Common.CustomPropertyData> _customPropertyDataList;
    protected Ektron.Cms.Framework.Core.CustomProperty.CustomProperty _customProperty = new Ektron.Cms.Framework.Core.CustomProperty.CustomProperty();
    protected Ektron.Cms.Common.CustomPropertyData _customPropertyData = new Ektron.Cms.Common.CustomPropertyData();
    protected Ektron.Cms.Common.CustomPropertyObjectData _customPropertyObjectData = new Ektron.Cms.Common.CustomPropertyObjectData();
    protected Ektron.Cms.Core.CustomProperty.CustomPropertyObjectBL _coreCustomProperty = new Ektron.Cms.Core.CustomProperty.CustomPropertyObjectBL();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refApi.EkMsgRef;
        AppImgPath = m_refApi.AppImgPath;
        m_strPageAction = Request.QueryString["action"];
        object refAPI = m_refApi as object;
        Utilities.SetLanguage(m_refApi);
        TaxonomyLanguage = m_refApi.ContentLanguage;
        if ((TaxonomyLanguage == -1))
        {
            TaxonomyLanguage = m_refApi.DefaultContentLanguage;
        }
        if ((Request.QueryString["taxonomyid"] != null))
        {
            TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
            hdnTaxonomyId.Value = TaxonomyId.ToString();
        }
        if ((Request.QueryString["parentid"] != null))
        {
            TaxonomyParentId = Convert.ToInt64(Request.QueryString["parentid"]);
        }
        m_refContent = m_refApi.EkContentRef;
        if ((Request.QueryString["reorder"] != null))
        {
            m_strReorderAction = Convert.ToString(Request.QueryString["reorder"]);
        }
        if ((Request.QueryString["view"] != null))
        {
            _ViewItem = Request.QueryString["view"];
        }
        if ((Page.IsPostBack))
        {
            if ((m_strPageAction == "edit"))
            {
                taxonomy_data.TaxonomyLanguage = TaxonomyLanguage;
                taxonomy_data.TaxonomyParentId = TaxonomyParentId;
                taxonomy_data.TaxonomyId = TaxonomyId;
                taxonomy_data.TaxonomyDescription = Request.Form[taxonomydescription.UniqueID];
                taxonomy_data.TaxonomyName = Request.Form[taxonomytitle.UniqueID];
                // taxonomy_data.TaxonomyImage = Request.Form[taxonomy_image.UniqueID];
                //  taxonomy_data.CategoryUrl = Request.Form[categoryLink.UniqueID];
                //if (tr_enableDisable.Visible == true)
                //{
                //    if (!string.IsNullOrEmpty(Request.Form[chkEnableDisable.UniqueID]))
                //    {
                //        taxonomy_data.Visible = true;
                //    }
                //    else
                //    {
                //        taxonomy_data.Visible = false;
                //    }
                //}
                //else
                //{
                //    taxonomy_data.Visible = Convert.ToBoolean(Request.Form[visibility.UniqueID]);
                //}
                //if ((Request.Form[inherittemplate.UniqueID] != null))
                //{
                //    taxonomy_data.TemplateInherited = true;
                //}
                //if ((Request.Form[taxonomytemplate.UniqueID] != null))
                //{
                //    taxonomy_data.TemplateId = Convert.ToInt64(Request.Form[taxonomytemplate.UniqueID]);
                //}
                //else
                //{
                //    taxonomy_data.TemplateId = 0;
                //}

                try
                {
                    m_refContent.UpdateTaxonomy(taxonomy_data);
                }
                catch (Exception ex)
                {
                    Utilities.ShowError(ex.Message);
                }
                //if (((Request.Form[chkApplyDisplayAllLanguages.UniqueID] != null)) && (Request.Form[chkApplyDisplayAllLanguages.UniqueID].ToString().ToLower() == "on"))
                //{
                //    m_refContent.UpdateTaxonomyVisible(TaxonomyId, -1, taxonomy_data.Visible);
                //}
                //else
                //{
                //    m_refContent.UpdateTaxonomyVisible(TaxonomyId, TaxonomyLanguage, taxonomy_data.Visible);
                //}
                if ((TaxonomyParentId == 0))
                {
                    string strConfig = string.Empty;
                    //if ((!string.IsNullOrEmpty(Request.Form[chkConfigContent.UniqueID])))
                    //{
                    //    strConfig = "0";
                    //}
                    //if ((!string.IsNullOrEmpty(Request.Form[chkConfigUser.UniqueID])))
                    //{
                    //    if ((string.IsNullOrEmpty(strConfig)))
                    //    {
                    //        strConfig = "1";
                    //    }
                    //    else
                    //    {
                    //        strConfig = strConfig + ",1";
                    //    }
                    //}
                    //if ((!string.IsNullOrEmpty(Request.Form[chkConfigGroup.UniqueID])))
                    //{
                    //    if ((string.IsNullOrEmpty(strConfig)))
                    //    {
                    //        strConfig = "2";
                    //    }
                    //    else
                    //    {
                    //        strConfig = strConfig + ",2";
                    //    }
                    //}
                    //if ((!(string.IsNullOrEmpty(strConfig))))
                    //{
                    //    m_refContent.UpdateTaxonomyConfig(TaxonomyId, strConfig);
                    //}
                }
                UpdateCustomProperties();

                if ((Request.QueryString["iframe"] == "true"))
                {
                    Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
                }
                else
                {
                    if ((Request.QueryString["backaction"] != null && Convert.ToString(Request.QueryString["backaction"]).ToLower() == "viewtree"))
                    {
                        Response.Redirect("LocaleTaxonomy.aspx?action=viewtree&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage, true);
                    }
                    else if (Request.QueryString["view"] != null)
                    {
                        Response.Redirect("LocaleTaxonomy.aspx?action=view&view=" +Convert.ToString(Request.QueryString["view"]) + "&taxonomyid=" + TaxonomyId + "&rf=1", true);
                    }
                    else
                    {
                        Response.Redirect("LocaleTaxonomy.aspx?action=view&view=item&taxonomyid=" + TaxonomyId + "&rf=1", true);
                    }
                }
            }
            else
            {
                if ((!string.IsNullOrEmpty(Request.Form[LinkOrder.UniqueID])))
                {
                    taxonomy_request = new TaxonomyRequest();
                    taxonomy_request.TaxonomyId = TaxonomyId;
                    taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
                    taxonomy_request.TaxonomyIdList = Request.Form[LinkOrder.UniqueID];
                    if (!string.IsNullOrEmpty(Request.Form[chkOrderAllLang.UniqueID]))
                    {
                        if ((m_strReorderAction == "category"))
                        {
                            m_refContent.ReOrderAllLanguageCategories(taxonomy_request);
                        }
                    }
                    else
                    {
                        if ((m_strReorderAction == "category"))
                        {
                            m_refContent.ReOrderCategories(taxonomy_request);
                        }
                        else
                        {
                            m_refContent.ReOrderTaxonomyItems(taxonomy_request);
                        }
                    }
                }
                if ((Request.QueryString["iframe"] == "true"))
                {
                    Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
                }
                else
                {
                    Response.Redirect("LocaleTaxonomy.aspx?action=view&type="+ Request.QueryString["type"]+"&taxonomyid=" + TaxonomyId + "&rf=1", true);
                }
            }
        }
        else
        {
            //ltr_sitepath.Text = m_refApi.SitePath;
            taxonomy_request = new TaxonomyRequest();
            taxonomy_request.TaxonomyId = TaxonomyId;
            taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;

            if ((m_strPageAction == "edit"))
            {
                taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);

                taxonomydescription.Text = taxonomy_data.TaxonomyDescription;
                taxonomytitle.Text = taxonomy_data.TaxonomyName;
                //  taxonomy_image.Text = taxonomy_data.TaxonomyImage;
                // taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage;
                //   categoryLink.Text = taxonomy_data.CategoryUrl;
                visibility.Value = taxonomy_data.Visible.ToString();
                if ((Request.QueryString["taxonomyid"] != null))
                {
                    TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
                }

                if (TaxonomyParentId > 0)
                {
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyParentId, TaxonomyLanguage);
                }
                else if (TaxonomyId > 0)
                {
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyId, TaxonomyLanguage);
                }

                // ' why in the world would you disable the visible flag if it's not synchronized???
                //If Not m_bSynchronized Then
                //tr_enableDisable.Visible = False
                //End If

                //if (taxonomy_data.Visible == true)
                //{
                //    chkEnableDisable.Checked = true;
                //}
                //else
                //{
                //    chkEnableDisable.Checked = false;
                //}
                //if (!string.IsNullOrEmpty(taxonomy_data.TaxonomyImage))
                //{
                //    taxonomy_image_thumb.ImageUrl = (taxonomy_data.TaxonomyImage.IndexOf("/") == 0 ? taxonomy_data.TaxonomyImage : m_refApi.SitePath + taxonomy_data.TaxonomyImage);
                //}
                //else
                //{
                //    taxonomy_image_thumb.ImageUrl = m_refApi.AppImgPath + "spacer.gif";
                //}
                //language_data = (new SiteAPI()).GetLanguageById(TaxonomyLanguage);
                ////if ((taxonomy_data.TaxonomyParentId == 0))
                //{
                //    inherittemplate.Visible = false;
                //    lblInherited.Text = "No";
                //    inherittemplate.Checked = taxonomy_data.TemplateInherited;
                //}
                //else
                //{
                //    inherittemplate.Visible = true;
                //    lblInherited.Text = "";
                //    inherittemplate.Checked = taxonomy_data.TemplateInherited;
                //    if ((taxonomy_data.TemplateInherited))
                //    {
                //        taxonomytemplate.Enabled = false;
                //    }
                //}
                //TemplateData[] templates = null;
                //templates = m_refApi.GetAllTemplates("TemplateFileName");
                //taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem("-select template-", "0"));
                //if ((templates != null && templates.Length > 0))
                //{
                //    for (int i = 0; i <= templates.Length - 1; i++)
                //    {
                //        taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem(templates[i].FileName, templates[i].Id.ToString()));
                //        if ((taxonomy_data.TemplateId == templates[i].Id))
                //        {
                //            taxonomytemplate.SelectedIndex = i + 1;
                //        }
                //    }
                //}
                if (((language_data != null) && (m_refApi.EnableMultilingual == 1)))
                {
                    lblLanguage.Text = "[" + language_data.Name + "]";
                }
                m_strCurrentBreadcrumb = taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\\", " > ");
                if ((string.IsNullOrEmpty(m_strCurrentBreadcrumb)))
                {
                    m_strCurrentBreadcrumb = "Root";
                }
                //admin inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)");
                if ((TaxonomyParentId == 0))
                {
                    //tr_config.Visible = true;
                    //List<Int32> config_list = m_refApi.EkContentRef.GetAllConfigIdListByTaxonomy(TaxonomyId, TaxonomyLanguage);
                    //for (int i = 0; i <= config_list.Count - 1; i++)
                    //{
                    //    if ((config_list[i] == 0))
                    //    {
                    //        chkConfigContent.Checked = true;
                    //    }
                    //    else if ((config_list[i] == 1))
                    //    {
                    //        chkConfigUser.Checked = true;
                    //    }
                    //    else if ((config_list[i] == 2))
                    //    {
                    //        chkConfigGroup.Checked = true;
                    //    }
                    //}
                }
                else
                {
                    //  tr_config.Visible = false;
                }
                LoadCustomPropertyList();
            }
            else
            {
                if ((m_strReorderAction == "category"))
                {
                    taxonomy_request.PageSize = 99999999;
                    // pagesize of 0 used to mean "all"
                    taxonomy_arr = m_refContent.ReadAllSubCategories(taxonomy_request);
                    if ((taxonomy_arr != null))
                    {
                        TotalItems = taxonomy_arr.Length;
                    }
                    else
                    {
                        TotalItems = 0;
                    }
                    if ((TotalItems > 1))
                    {
                        td_msg.Text = m_refMsg.GetMessage("generic first msg");
                        OrderList.DataSource = taxonomy_arr;
                        OrderList.DataTextField = "TaxonomyName";
                        OrderList.DataValueField = "TaxonomyId";
                        OrderList.DataBind();
                        OrderList.SelectionMode = ListSelectionMode.Multiple;
                        OrderList.CssClass = "width: 100%; border-style: none; border-color: White; font-family: Verdana;font-size: x-small;";
                        if ((TotalItems > 20))
                        {
                            OrderList.Rows = 20;
                        }
                        else
                        {
                            OrderList.Rows = TotalItems;
                        }
                        OrderList.Width = 300;
                        if ((TotalItems > 0))
                        {
                            loadscript.Text = "document.forms[0].taxonomy_OrderList[0].selected = true;";
                        }
                        for (int i = 0; i <= taxonomy_arr.Length - 1; i++)
                        {
                            if ((string.IsNullOrEmpty(LinkOrder.Value)))
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
                else
                {
                    AllLangForm.Visible = false;
                    // the all languages checkbox is only valid for categories
                    taxonomy_request.PageSize = 99999999;
                    taxonomy_request.IncludeItems = true;
                    taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);
                    tr_ordering.Visible = true;
                    //Not showing for items
                    if ((taxonomy_data.TaxonomyItems != null))
                    {
                        TotalItems = taxonomy_data.TaxonomyItems.Length;
                        if ((TotalItems > 1))
                        {
                            td_msg.Text = m_refMsg.GetMessage("generic first msg");
                            OrderList.DataSource = taxonomy_data.TaxonomyItems;
                            OrderList.DataTextField = "TaxonomyItemTitle";
                            OrderList.DataValueField = "TaxonomyItemId";
                            OrderList.DataBind();
                            OrderList.SelectionMode = ListSelectionMode.Multiple;
                            OrderList.CssClass = "width: 100%; border-style: none; border-color: White; font-family: Verdana;font-size: x-small;";
                            if ((TotalItems > 20))
                            {
                                OrderList.Rows = 20;
                            }
                            else
                            {
                                OrderList.Rows = TotalItems;
                            }
                            OrderList.Width = 300;
                            if ((TotalItems > 0))
                            {
                                loadscript.Text = "document.forms[0].taxonomy_OrderList[0].selected = true;";
                            }
                            foreach (TaxonomyItemData taxonomy_item in taxonomy_data.TaxonomyItems)
                            {
                                if ((string.IsNullOrEmpty(LinkOrder.Value)))
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
        if ((taxonomy_data != null) && (taxonomy_data.TaxonomyName != null) && !string.IsNullOrEmpty(taxonomy_data.TaxonomyName))
        {
            _taxName = taxonomy_data.TaxonomyName;
            //will be called only on reorder items screen. No other way to get the taxonomy name
        }
        else if (TaxonomyId > 0)
        {
            taxonomy_request = new TaxonomyRequest();
            taxonomy_request.TaxonomyId = TaxonomyId;
            taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
            TaxonomyData _taxData = m_refContent.ReadTaxonomy(ref taxonomy_request);
            _taxName = _taxData.TaxonomyName;
        }
        if (m_strPageAction == "reorder")
        {
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("reorder taxonomy page title") + "&nbsp;&nbsp;\"" + _taxName + "\"&nbsp;&nbsp;<img style='vertical-align:middle;' src='" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "' />");
        }
        else
        {
            divTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("btn locale edit") + "&nbsp;&nbsp;\"" + _taxName + "\"&nbsp;&nbsp;<img style='vertical-align:middle;' src='" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "' />");
        }
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.AppendLine("<table><tr>");

		if ((Request.QueryString["iframe"] == "true"))
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onClick=\"parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true));
		}
		else
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "LocaleTaxonomy.aspx?action=view&view=" + _ViewItem + "&taxonomyid=" + TaxonomyId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if ((m_strPageAction == "edit"))
        {
			   result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (locale taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=\"if(SetPropertyIds()){Validate(true);}\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
            if ((ShowSaveIcon))
            {
				result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (taxonomy)"), m_refMsg.GetMessage("btn update"), "onclick=\"if(SetPropertyIds()){Validate(false);}\"", StyleHelper.SaveButtonCssClass, true));
            }
        }
        result.Append(StyleHelper.ActionBarDivider);
        if ((m_strPageAction != "edit"))
        {
            result.Append("<td>" + ReorderDropDown() + "</td>");
            result.Append("<td>" + m_refstyle.GetHelpButton("ReOrderLocaleTaxonomyItem", "") + "</td>");
        }
        else
        {
            result.Append("<td>" + m_refstyle.GetHelpButton("EditLocaleTaxonomy", "") + "</td>");
        }
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private string ReorderDropDown()
    {
        StringBuilder result = new StringBuilder();
        result.Append(m_refMsg.GetMessage("type label") + "<select id=\"selreorderitem\" name=\"selreorderitem\" onchange=\"javascript:LoadReorderType(this.value);\">" + "<option value=\"category\" " + FindSelected("category") + ">Category</option>" + "<option value=\"item\"  " + FindSelected("item") + ">Items</option></select>");
        return result.ToString();
    }
    private string FindSelected(string chk)
    {
        string val = "";
        if ((m_strReorderAction == chk))
        {
            val = " selected ";
        }
        return val;
    }

    private void LoadCustomPropertyList()
    {
        //int i = 0;
        //int j = 1;

        //PagingInfo pageInfo = new PagingInfo();
        //pageInfo.CurrentPage = 1;
        //pageInfo.RecordsPerPage = 99999;

        //_customPropertyDataList = _customProperty.GetList(EkEnumeration.CustomPropertyObjectType.TaxonomyNode, m_refApi.ContentLanguage, pageInfo);
        //for (i = 0; i <= _customPropertyDataList.Count - 1; i++)
        //{
        //    if (_customPropertyDataList[i].IsEnabled)
        //    {
        //        //availableCustomProp.Items.Insert(j, _customPropertyDataList[i].PropertyName);
        //        //availableCustomProp.Items[j].Value = _customPropertyDataList[i].PropertyId.ToString();
        //        j = j + 1;
        //    }
        //}

        //availableCustomProp.DataBind();

    }

    private void UpdateCustomProperties()
    {
       
        int i = 0;
        Ektron.Cms.Framework.Core.CustomProperty.CustomProperty cp = new Ektron.Cms.Framework.Core.CustomProperty.CustomProperty();
        Ektron.Cms.Framework.Core.CustomProperty.CustomPropertyObject cpo = new Ektron.Cms.Framework.Core.CustomProperty.CustomPropertyObject();
        System.Collections.Generic.List<Ektron.Cms.Common.CustomPropertyObjectData> listData = default(System.Collections.Generic.List<Ektron.Cms.Common.CustomPropertyObjectData>);
        string[] selectedIds = null;
        string[] selectedValues = null;

        PagingInfo pageInfo = new PagingInfo();
        pageInfo.CurrentPage = 1;
        pageInfo.RecordsPerPage = 99999;

        listData = cpo.GetList(TaxonomyId, m_refApi.ContentLanguage, EkEnumeration.CustomPropertyObjectType.TaxonomyNode, pageInfo);
        if (!string.IsNullOrEmpty(Request.Form[hdnSelectedIDS.UniqueID]))
        {
            selectedIds = Request.Form[hdnSelectedIDS.UniqueID].Remove(Request.Form[hdnSelectedIDS.UniqueID].Length - 1, 1).Split(';');
        }
        if (!string.IsNullOrEmpty(Request.Form[hdnSelectValue.UniqueID]))
        {
            selectedValues = Request.Form[hdnSelectValue.UniqueID].Remove(Request.Form[hdnSelectValue.UniqueID].Length - 1, 1).Split(';');
        }
        _customPropertyDataList = _customProperty.GetList(Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType.TaxonomyNode, m_refApi.ContentLanguage, pageInfo);

        for (i = 0; i <= listData.Count - 1; i++)
        {
            cpo.Delete(TaxonomyId, m_refApi.ContentLanguage, EkEnumeration.CustomPropertyObjectType.TaxonomyNode, listData[i].PropertyId);
        }

        if (selectedIds != null | selectedValues != null)
        {
            for (i = 0; i <= selectedIds.Length - 1; i++)
            {
                Ektron.Cms.Common.CustomPropertyData customPropertyData = cp.GetItem(Convert.ToInt64(selectedIds[i].ToString()), m_refApi.ContentLanguage);
                CustomPropertyObjectData data = new CustomPropertyObjectData(TaxonomyId, m_refApi.ContentLanguage, Convert.ToInt64(selectedIds[i].ToString()), EkEnumeration.CustomPropertyObjectType.TaxonomyNode);

                if ((((customPropertyData != null)) && ((data != null))))
                {
                    string inputValue = System.Web.HttpUtility.UrlDecode(selectedValues[i].ToString());

                    switch (customPropertyData.PropertyDataType)
                    {
                        case EkEnumeration.CustomPropertyItemDataType.Boolean:
                            bool booleanValue = false;
                            if ((bool.TryParse(inputValue, out booleanValue)))
                            {
                                data.AddItem(booleanValue);
                            }

                            break;
                        case EkEnumeration.CustomPropertyItemDataType.DateTime:
                            DateTime dateTimeValue = default(DateTime);
                            if ((DateTime.TryParse(inputValue, out dateTimeValue)))
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
}

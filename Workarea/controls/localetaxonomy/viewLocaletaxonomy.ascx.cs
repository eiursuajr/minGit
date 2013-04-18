//-----------------------------------------------------------------------
// <copyright file="viewLocaletaxonomy.ascx.cs" company="Ektron" author="Rama Ila">
// Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
// Dispaly the translation package with Items.
//-----------------------------------------------------------------------
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.Core.CustomProperty;
using Ektron.Cms.Content;
using Ektron.Cms.Localization;

partial class viewLocaletaxonomy : System.Web.UI.UserControl
{
    protected CommonApi _Common = new CommonApi();
    protected StyleHelper _StyleHelper = new StyleHelper();
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected EkMessageHelper _MessageHelper = null;
    protected string _PageAction = "";
    protected Ektron.Cms.Content.EkContent _Content;
    protected long TaxonomyId = 0;
    protected int TaxonomyLanguage = -1;
    protected LanguageData language_data;
    protected TaxonomyRequest taxonomy_request;
    protected TaxonomyData taxonomy_data;
    protected long TaxonomyParentId = 0;
    protected string _ViewItem = "item";
    protected bool AddDeleteIcon = false;
    protected long TaxonomyItemCount = 0;
    protected long TaxonomyCategoryCount = 0;
    protected string _TaxonomyName = "";
    protected Ektron.Cms.LocalizationAPI _LocalizationApi = new LocalizationAPI();
    protected Ektron.Cms.API.Folder folderApi = new Ektron.Cms.API.Folder();
    protected string mainTranslationPackageLink = "";
    protected int m_intCurrentPage = 1;
    protected int m_intTotalPages = 1;
    protected int m_intMetadataCurrentPage = 1;
    protected int m_intMetadataTotalPages = 1;
    protected string m_strDelConfirm = "";
    protected string m_strDelItemsConfirm = "";
    protected string m_strSelDelWarning = "";
    protected string m_strCurrentBreadcrumb = "";
    protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
    string parentTaxonomyPath = string.Empty;

    public viewLocaletaxonomy()
    {
        _MessageHelper = _Common.EkMsgRef;
    }
    protected void Page_Load(object sender, System.EventArgs e)
    {
        _MessageHelper = _Common.EkMsgRef;
        AppImgPath = _Common.AppImgPath;
        AppPath = _Common.AppPath;
        this.MenuAppPath.Text = AppPath;
        _PageAction = Request.QueryString["action"];
        object refCommon = _Common as object;
        mainTranslationPackageLink = "<a href =\"LocaleTaxonomy.aspx\">Translation Packages</a>";
        Utilities.SetLanguage(_Common);
        RegisterResources();
        _Content = _Common.EkContentRef;
       // TaxonomyLanguage = _Common.DefaultContentLanguage;
        TaxonomyLanguage = _Common.ContentLanguage;
        TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
        if ((Request.QueryString["view"] != null))
        {
            _ViewItem = Request.QueryString["view"];
        }
        taxonomy_request = new TaxonomyRequest();
        taxonomy_request.TaxonomyId = TaxonomyId;
        taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
        taxonomy_request.PageSize = 99999999;
        // pagesize of 0 used to mean "all"
        TaxonomyBaseData[] taxcats = null;
        taxcats = _Content.ReadAllSubCategories(taxonomy_request);

        if ((taxcats != null))
        {
            TaxonomyCategoryCount = taxcats.Length;
        }
        if ((Page.IsPostBack && !string.IsNullOrEmpty(Request.Form[isPostData.UniqueID])))
        {
            if ((Request.Form["submittedaction"] == "delete"))
            {
                _Content.DeleteTaxonomy(taxonomy_request);
                //Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
                Response.Redirect("LocaleTaxonomy.aspx?rf=1", false);
            }
            else if ((Request.Form["submittedaction"] == "deleteitem"))
            {
                taxonomy_request.TaxonomyIdList = Request.Form["selected_items"];
                switch (_ViewItem.ToLower())
                {
                    case "cgroup":
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group;
                        break;
                    case "user":
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User;
                        break;
                    case "locale":
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Locale;
                        break;
                    case "folder":
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.FolderDescendants;
                        _Content.RemoveTaxonomyItem(taxonomy_request);
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.FolderChildren;
                        break;
                    default:
                        taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Content;
                        break;
                }

                _Content.RemoveTaxonomyItem(taxonomy_request);
                if ((Request.Params["ccp"] == null))
                {
                    Response.Redirect("LocaleTaxonomy.aspx?" + Request.ServerVariables["query_string"] + "&ccp=true", true);
                }
                else
                {
                    Response.Redirect("LocaleTaxonomy.aspx?" + Request.ServerVariables["query_string"], true);
                }
            }
        }
        else if ((IsPostBack == false))
        {
            DisplayPage();
        }

        AssignTextStrings();

        isPostData.Value = "true";
        hdnSourceId.Value = TaxonomyId.ToString();
    }

    private void DisplayPage()
    {
        List<int> langList = new List<int>();
        List<int> parenttaxonomyLanguageList = new List<int>();
        switch (_ViewItem.ToLower())
        {
            case "user":
                DirectoryUserRequest uReq = new DirectoryUserRequest();
                DirectoryAdvancedUserData uDirectory = new DirectoryAdvancedUserData();
                uReq.GetItems = true;
                uReq.DirectoryId = TaxonomyId;
                uReq.DirectoryLanguage = TaxonomyLanguage;
                uReq.PageSize = _Common.RequestInformationRef.PagingSize;
                uReq.CurrentPage = m_intCurrentPage;
                uDirectory = this._Content.LoadDirectory(ref uReq);
                if ((uDirectory != null))
                {
                    TaxonomyParentId = uDirectory.DirectoryParentId;
                    lbltaxonomyid.Text = uDirectory.DirectoryId.ToString();
                    taxonomytitle.Text = uDirectory.DirectoryName;
                    _TaxonomyName = uDirectory.DirectoryName;
                    taxonomydescription.Text = uDirectory.DirectoryDescription;
                    ////taxonomy_image_thumb.ImageUrl = _Common.AppImgPath + "spacer.gif";
                    string[] taxonomyPathArray = new string[] { };
                    string[] taxonomyIdPathArray = new string[] { };
                    string taxonomyBreadCrumbWithLinks = string.Empty;
                    if (uDirectory.DirectoryPath != string.Empty)
                    {
                        taxonomyPathArray = uDirectory.DirectoryPath.Split('\\');
                    }
                    if (uDirectory.DirectoryIDPath != string.Empty)
                    {
                        taxonomyIdPathArray = uDirectory.DirectoryIDPath.Split('/');
                    }
                    //form the taxonomy Bread Crumb Links.
                    for (int k = 1; k < taxonomyPathArray.Length; k++)
                    {
                         int taxonomyIdForBreadCrumb =0;
                       int.TryParse(taxonomyIdPathArray[k],out taxonomyIdForBreadCrumb);
                       if (taxonomyIdForBreadCrumb == TaxonomyId)
                        {
                            if (taxonomyBreadCrumbWithLinks == string.Empty)
                            {
                                taxonomyBreadCrumbWithLinks = taxonomyPathArray[k];
                            }
                            else
                            {
                                taxonomyBreadCrumbWithLinks += " > " + taxonomyPathArray[k];
                            }
                        }
                        else
                        {
                            if (taxonomyBreadCrumbWithLinks == string.Empty)
                            {
                                taxonomyBreadCrumbWithLinks = "<a id=" + taxonomyIdPathArray[k] + " href=\"localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + taxonomyIdPathArray[k] + "&treeViewId=-1&LangType=" + TaxonomyLanguage + "\" class=\"linkStyle\">" + taxonomyPathArray[k] + " </a>";
                            }
                            else
                            {
                                taxonomyBreadCrumbWithLinks += " > <a id=" + taxonomyIdPathArray[k] + " href=\"localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + taxonomyIdPathArray[k] + "&treeViewId=-1&LangType=" + TaxonomyLanguage + "\" class=\"linkStyle\">" + taxonomyPathArray[k] + " </a>";
                            }
                        }

                    }
                    if (taxonomyBreadCrumbWithLinks != string.Empty)
                    {
                        m_strCurrentBreadcrumb = mainTranslationPackageLink + " > " + taxonomyBreadCrumbWithLinks;
                    }
                    else
                    {
                        m_strCurrentBreadcrumb = mainTranslationPackageLink;
                    }
                    if (uDirectory.DirectoryParentId == 0)
                    {
                        parentTaxonomyPath = uDirectory.DirectoryPath.Replace("\\" + uDirectory.DirectoryName, "\\");
                    }
                    else
                    {
                        parentTaxonomyPath = uDirectory.DirectoryPath.Replace("\\" + uDirectory.DirectoryName, "");
                    }
                    hdn_parentTaxonomyPath.Value = parentTaxonomyPath;
                    //if ((string.IsNullOrEmpty(uDirectory.TemplateName)))
                    //{
                    //    lblTemplate.Text = "[None]";
                    //}
                    //else
                    //{
                    //    lblTemplate.Text = uDirectory.TemplateName;
                    //}
                    //if ((uDirectory.InheritTemplate))
                    //{
                    //    lblTemplateInherit.Text = "Yes";
                    //}
                    //else
                    //{
                    //    lblTemplateInherit.Text = "No";
                    //}

                    m_intTotalPages = uReq.TotalPages;
                }
                PopulateUserGridData(uDirectory);
                TaxonomyToolBar();
                //ltrItemCount.Text = uDirectory.DirectoryItems.Length.ToString();
                break;
            case "cgroup":
                DirectoryAdvancedGroupData dagdRet = new DirectoryAdvancedGroupData();
                DirectoryGroupRequest cReq = new DirectoryGroupRequest();
                cReq.CurrentPage = m_intCurrentPage;
                cReq.PageSize = _Common.RequestInformationRef.PagingSize;
                cReq.DirectoryId = TaxonomyId;
                cReq.DirectoryLanguage = TaxonomyLanguage;
                cReq.GetItems = true;
                cReq.SortDirection = "";

                dagdRet = this._Common.CommunityGroupRef.LoadDirectory(ref cReq);
                if ((dagdRet != null))
                {
                    TaxonomyParentId = dagdRet.DirectoryParentId;
                    lbltaxonomyid.Text = dagdRet.DirectoryId.ToString();
                    taxonomytitle.Text = dagdRet.DirectoryName;
                    _TaxonomyName = dagdRet.DirectoryName;
                    taxonomydescription.Text = dagdRet.DirectoryDescription;
                    //taxonomy_image_thumb.ImageUrl = _Common.AppImgPath + "spacer.gif";
                    //form the BreadCrumb Link.
                    string[] taxonomyPathArray = new string[] { };
                    string[] taxonomyIdPathArray = new string[] { };
                    string taxonomyBreadCrumbWithLinks = string.Empty;
                    if (dagdRet.DirectoryPath != string.Empty)
                    {
                        taxonomyPathArray = dagdRet.DirectoryPath.Split('\\');
                    }
                    if (dagdRet.DirectoryIDPath != string.Empty)
                    {
                        taxonomyIdPathArray = dagdRet.DirectoryIDPath.Split('/');
                    }
                    //form the taxonomy Bread Crumb Links.
                    for (int k = 1; k < taxonomyPathArray.Length; k++)
                    {
                        int taxonomyIdForBreadCrumb =0;
                       int.TryParse(taxonomyIdPathArray[k],out taxonomyIdForBreadCrumb);
                       if (taxonomyIdForBreadCrumb == TaxonomyId)
                        {
                            if (taxonomyBreadCrumbWithLinks == string.Empty)
                            {
                                taxonomyBreadCrumbWithLinks = taxonomyPathArray[k];
                            }
                            else
                            {
                                taxonomyBreadCrumbWithLinks += " > " + taxonomyPathArray[k];
                            }
                        }
                        else
                        {
                            if (taxonomyBreadCrumbWithLinks == string.Empty)
                            {
                                taxonomyBreadCrumbWithLinks = "<a id=" + taxonomyIdPathArray[k] + " href=\"localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + taxonomyIdPathArray[k] + "&treeViewId=-1&LangType=" + TaxonomyLanguage + "\" class=\"linkStyle\">" + taxonomyPathArray[k] + " </a>";
                            }
                            else
                            {
                                taxonomyBreadCrumbWithLinks += " > <a id=" + taxonomyIdPathArray[k] + " href=\"localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + taxonomyIdPathArray[k] + "&treeViewId=-1&LangType=" + TaxonomyLanguage + "\" class=\"linkStyle\">" + taxonomyPathArray[k] + " </a>";
                            }
                        }
                    }
                    // <a id="136" href="localeTaxonomy.aspx?action=view&amp;view=locale&amp;taxonomyid=136&amp;treeViewId=-1&amp;LangType=1033" class="linkStyle">chanduila </a>
                    //   m_strCurrentBreadcrumb = mainTranslationPackageLink + taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\\", " > ");
                    if (taxonomyBreadCrumbWithLinks != string.Empty)
                    {
                        m_strCurrentBreadcrumb = mainTranslationPackageLink + " > " + taxonomyBreadCrumbWithLinks;
                    }
                    else
                    {
                        m_strCurrentBreadcrumb = mainTranslationPackageLink;
                    }

                    if ((string.IsNullOrEmpty(m_strCurrentBreadcrumb)))
                    {
                        m_strCurrentBreadcrumb = mainTranslationPackageLink;
                    }
                    if (dagdRet.DirectoryParentId == 0)
                    {
                        parentTaxonomyPath = dagdRet.DirectoryPath.Replace("\\" + dagdRet.DirectoryName, "\\");
                    }
                    else
                    {
                        parentTaxonomyPath = dagdRet.DirectoryPath.Replace("\\" + dagdRet.DirectoryName, "");
                    }

                  
                    hdn_parentTaxonomyPath.Value = parentTaxonomyPath;
                    //if ((string.IsNullOrEmpty(dagdRet.TemplateName)))
                    //{
                    //    lblTemplate.Text = "[None]";
                    //}
                    //else
                    //{
                    //    lblTemplate.Text = dagdRet.TemplateName;
                    //}
                    //if ((dagdRet.InheritTemplate))
                    //{
                    //    lblTemplateInherit.Text = "Yes";
                    //}
                    //else
                    //{
                    //    lblTemplateInherit.Text = "No";
                    //}
                    m_intTotalPages = cReq.TotalPages;
                }
                PopulateCommunityGroupGridData(dagdRet);
                TaxonomyToolBar();
                //ltrItemCount.Text = cReq.RecordsAffected.ToString();
                break;
            case "locale":
                taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Locale;
                taxonomy_request.TaxonomyType = EkEnumeration.TaxonomyType.Locale;
                taxonomy_request.IncludeItems = true;
                taxonomy_request.PageSize = _Common.RequestInformationRef.PagingSize;
                taxonomy_request.CurrentPage = m_intCurrentPage;
                taxonomy_data = _Content.ReadTaxonomy(ref taxonomy_request);

                if ((taxonomy_data != null))
                {
                    TaxonomyParentId = taxonomy_data.TaxonomyParentId;
                    lbltaxonomyid.Text = taxonomy_data.TaxonomyId.ToString();
                    taxonomytitle.Text = taxonomy_data.TaxonomyName;
                    _TaxonomyName = taxonomy_data.TaxonomyName;
                    if (string.IsNullOrEmpty(taxonomy_data.TaxonomyDescription))
                    {
                        taxonomydescription.Text = "[None]";
                    }
                    else
                    {
                        taxonomydescription.Text = Server.HtmlEncode(taxonomy_data.TaxonomyDescription);
                    }
                    //if (string.IsNullOrEmpty(taxonomy_data.TaxonomyImage))
                    //{
                    //    taxonomy_image.Text = "[None]";
                    //}
                    //else
                    //{
                    //    taxonomy_image.Text = taxonomy_data.TaxonomyImage;
                    //}
                    //taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage;
                    //if (string.IsNullOrEmpty(taxonomy_data.CategoryUrl))
                    //{
                    //    catLink.Text = "[None]";
                    //}
                    //else
                    //{
                    //    catLink.Text = taxonomy_data.CategoryUrl;
                    //}

                    //if (taxonomy_data.Visible == true)
                    //{
                    //    ltrStatus.Text = "Enabled";
                    //}
                    //else
                    //{
                    //    ltrStatus.Text = "Disabled";
                    //}
                    //if (!string.IsNullOrEmpty(taxonomy_data.TaxonomyImage.Trim()))
                    //{
                    //    taxonomy_image_thumb.ImageUrl = (taxonomy_data.TaxonomyImage.IndexOf("/") == 0 ? taxonomy_data.TaxonomyImage : _Common.SitePath + taxonomy_data.TaxonomyImage);
                    //}
                    //else
                    //{
                    //    taxonomy_image_thumb.ImageUrl = _Common.AppImgPath + "spacer.gif";
                    //}
                    //form the BreadCrumb Link.
                    FormBreadCrumb(taxonomy_data);
                    //if ((string.IsNullOrEmpty(taxonomy_data.TemplateName)))
                    //{
                    //    lblTemplate.Text = "[None]";
                    //}
                    //else
                    //{
                    //    lblTemplate.Text = taxonomy_data.TemplateName;
                    //}
                    //if ((taxonomy_data.TemplateInherited))
                    //{
                    //    lblTemplateInherit.Text = "Yes";
                    //}
                    //else
                    //{
                    //    lblTemplateInherit.Text = "No";
                    //}
                    m_intTotalPages = taxonomy_request.TotalPages;
                }
                Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy localeTaxonomyApi = new Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy(_Common.RequestInformationRef);
                //API = New Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy(_Common.RequestInformationRef)
                langList = localeTaxonomyApi.GetLocaleIdList(TaxonomyId, _Common.ContentLanguage, false);
                parenttaxonomyLanguageList = localeTaxonomyApi.GetLocaleIdList(TaxonomyParentId, _Common.ContentLanguage, true);
                PopulateLocaleContentGridData(langList, parenttaxonomyLanguageList);
                TaxonomyToolBar();
                //ltrItemCount.Text = langList.Count.ToString();
                break;
            default:
                // Content
                taxonomy_request.IncludeItems = true;
                taxonomy_request.PageSize = _Common.RequestInformationRef.PagingSize;
                taxonomy_request.CurrentPage = m_intCurrentPage;
                taxonomy_data = _Content.ReadTaxonomy(ref taxonomy_request);
                if ((taxonomy_data != null))
                {
                    TaxonomyParentId = taxonomy_data.TaxonomyParentId;
                    lbltaxonomyid.Text = taxonomy_data.TaxonomyId.ToString();
                    taxonomytitle.Text = taxonomy_data.TaxonomyName;
                    _TaxonomyName = taxonomy_data.TaxonomyName;
                    if (string.IsNullOrEmpty(taxonomy_data.TaxonomyDescription))
                    {
                        taxonomydescription.Text = "[None]";
                    }
                    else
                    {
                        taxonomydescription.Text = Server.HtmlEncode(taxonomy_data.TaxonomyDescription);
                    }
                    //if (string.IsNullOrEmpty(taxonomy_data.TaxonomyImage))
                    //{
                    //    taxonomy_image.Text = "[None]";
                    //}
                    //else
                    //{
                    //    taxonomy_image.Text = taxonomy_data.TaxonomyImage;
                    //}
                    //taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage;
                    //if (string.IsNullOrEmpty(taxonomy_data.CategoryUrl))
                    //{
                    //    catLink.Text = "[None]";
                    //}
                    //else
                    //{
                    //    catLink.Text = taxonomy_data.CategoryUrl;
                    //}

                    //if (taxonomy_data.Visible == true)
                    //{
                    //    ltrStatus.Text = "Enabled";
                    //}
                    //else
                    //{
                    //    ltrStatus.Text = "Disabled";
                    //}
                    //if (!string.IsNullOrEmpty(taxonomy_data.TaxonomyImage.Trim()))
                    //{
                    //    taxonomy_image_thumb.ImageUrl = (taxonomy_data.TaxonomyImage.IndexOf("/") == 0 ? taxonomy_data.TaxonomyImage : _Common.SitePath + taxonomy_data.TaxonomyImage);
                    //}
                    //else
                    //{
                    //    taxonomy_image_thumb.ImageUrl = _Common.AppImgPath + "spacer.gif";
                    //}
                    //form the BreadCrumb Link.
                    FormBreadCrumb(taxonomy_data);
                    //if ((string.IsNullOrEmpty(taxonomy_data.TemplateName)))
                    //{
                    //    lblTemplate.Text = "[None]";
                    //}
                    //else
                    //{
                    //    lblTemplate.Text = taxonomy_data.TemplateName;
                    //}
                    //if ((taxonomy_data.TemplateInherited))
                    //{
                    //    lblTemplateInherit.Text = "Yes";
                    //}
                    //else
                    //{
                    //    lblTemplateInherit.Text = "No";
                    //}
                    m_intTotalPages = taxonomy_request.TotalPages;
                    //ltrItemCount.Text = taxonomy_request.RecordsAffected.ToString();
                }
                PopulateContentGridData();
                TaxonomyToolBar();
                //ltrItemCount.Text = taxonomy_request.RecordsAffected.ToString();
                break;
        }

        DisplayTaxonomyMetadata();
        tr_catcount.Visible = false;
        //if (_ViewItem.ToLower() == "locale")
        //{
        //    ltrItemCount.Text = langList.Count.ToString();
        //}
        //else
        //{
        //    ltrItemCount.Text = taxonomy_request.RecordsAffected.ToString();
        //}
    }

    private void FormBreadCrumb(TaxonomyData taxonomy_data)
    {
        //form the BreadCrumb Link.
        string[] taxonomyPathArray = new string[] { };
        string[] taxonomyIdPathArray = new string[] { };
        string taxonomyBreadCrumbWithLinks = string.Empty;
      
        if (taxonomy_data.TaxonomyPath != string.Empty)
        {
            taxonomyPathArray = taxonomy_data.TaxonomyPath.Split('\\');
        }
        if (taxonomy_data.TaxonomyIdPath != string.Empty)
        {
            taxonomyIdPathArray = taxonomy_data.TaxonomyIdPath.Split('/');
        }
        //form the taxonomy Bread Crumb Links.
        for (int k = 1; k < taxonomyPathArray.Length; k++)
        {
            int taxonomyIdForBreadCrumb = 0;
            int.TryParse(taxonomyIdPathArray[k], out taxonomyIdForBreadCrumb);
            if (taxonomyIdForBreadCrumb == TaxonomyId)
            {
                if (taxonomyBreadCrumbWithLinks == string.Empty)
                {
                    taxonomyBreadCrumbWithLinks = taxonomyPathArray[k];
                }
                else
                {
                    taxonomyBreadCrumbWithLinks += " > " + taxonomyPathArray[k];
                }
            }
            else
            {
                if (taxonomyBreadCrumbWithLinks == string.Empty)
                {
                    taxonomyBreadCrumbWithLinks = "<a id=" + taxonomyIdPathArray[k] + " href=\"localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + taxonomyIdPathArray[k] + "&treeViewId=-1&LangType=" + TaxonomyLanguage + "\" class=\"linkStyle\">" + taxonomyPathArray[k] + " </a>";
                }
                else
                {
                    taxonomyBreadCrumbWithLinks += " > <a id=" + taxonomyIdPathArray[k] + " href=\"localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + taxonomyIdPathArray[k] + "&treeViewId=-1&LangType=" + TaxonomyLanguage + "\" class=\"linkStyle\">" + taxonomyPathArray[k] + " </a>";
                }
            }
        }
        //   <a id="136" href="localeTaxonomy.aspx?action=view&amp;view=locale&amp;taxonomyid=136&amp;treeViewId=-1&amp;LangType=1033" class="linkStyle">chanduila </a>
        //   m_strCurrentBreadcrumb = mainTranslationPackageLink + taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\\", " > ");
        if (taxonomyBreadCrumbWithLinks != string.Empty)
        {
            m_strCurrentBreadcrumb = mainTranslationPackageLink + " > " + taxonomyBreadCrumbWithLinks;
        }
        else
        {
            m_strCurrentBreadcrumb = mainTranslationPackageLink;
        }
        if ((string.IsNullOrEmpty(m_strCurrentBreadcrumb)))
        {
            m_strCurrentBreadcrumb = mainTranslationPackageLink;
        }
        if (taxonomy_data.TaxonomyParentId == 0)
        {
            parentTaxonomyPath = taxonomy_data.TaxonomyPath.Replace("\\" + taxonomy_data.TaxonomyName, "\\");
        }
        else
        {
            parentTaxonomyPath = taxonomy_data.TaxonomyPath.Replace("\\" + taxonomy_data.TaxonomyName, "");
        }
       hdn_parentTaxonomyPath.Value = parentTaxonomyPath;

    }

    private void PopulateLocaleContentGridData(List<int> langList,List<int> parenttaxonomyLanguageList)
    {
        VisiblePageControls(false);
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=\"checkbox\" name=\"checkall\" onclick=\"checkAll('selected_items',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("TITLE", _MessageHelper.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(50), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        //  TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("LANGUAGE", _MessageHelper.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        //TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("URL", _MessageHelper.GetMessage("generic url link"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(30), false, false));
        //TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ARCHIVED", _MessageHelper.GetMessage("lbl archived"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        DataTable dt = new DataTable();
        DataRow dr = default(DataRow);
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        //dt.Columns.Add(new DataColumn("URL", typeof(string)));
        //dt.Columns.Add(new DataColumn("ARCHIVED", typeof(string)));
        Ektron.Cms.Framework.Localization.LocaleManager locApi = new Ektron.Cms.Framework.Localization.LocaleManager();
        List<LocaleData> locales = locApi.GetEnabledLocales(langList);
        List<LocaleData> parentEnabledlocales = locApi.GetEnabledLocales(parenttaxonomyLanguageList);
        //get the list of all the languages assigned to the top level locale taxonomy.
        List<int> localeIdsOfPackage = new List<int>();

        if (locales != null && locales.Count > 0)
        {
            AddDeleteIcon = true;
            for (int i = 0; i <= (locales.Count - 1); i++)
            {
                localeIdsOfPackage.Add(locales[i].Id);
                if (locales[i].Id == TaxonomyLanguage)
                {
                    dr = dt.NewRow();
                    dr["CHECK"] = "<input type=\"checkbox\" name=\"disabled_items\" disabled id=\"disabled_items\" value=\"" + locales[i].Id + "\" onclick=\"checkAll('selected_items',true);\">";
                    dr["TITLE"] = "<img src='" + locales[i].FlagUrl + "' />&nbsp;&nbsp;" + locales[i].CombinedName;
                    dr["ID"] = locales[i].Id;
                    dt.Rows.Add(dr);
                }
                else
                {
                    dr = dt.NewRow();
                    dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + locales[i].Id + "\" onclick=\"checkAll('selected_items',true);\">";
                    dr["TITLE"] = "<img src='" + locales[i].FlagUrl + "' />&nbsp;&nbsp;" + locales[i].CombinedName;
                    dr["ID"] = locales[i].Id;
                    dt.Rows.Add(dr);
                }
            }
            DataView dv = new DataView(dt);
            TaxonomyItemList.DataSource = dv;
            TaxonomyItemList.DataBind();
        }
        else
        {
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            TaxonomyItemList.GridLines = GridLines.None;
        }
        if (parentEnabledlocales != null && parentEnabledlocales.Count > 0)
        {
            for (int i = 0; i <= (parentEnabledlocales.Count - 1); i++)
            {
                if (!localeIdsOfPackage.Contains(parentEnabledlocales[i].Id))
                {
                    dr = dt.NewRow();
                dr["CHECK"] = "<input type=\"checkbox\" name=\"disabled_items\" disabled id=\"disabled_items\" value=\"" + parentEnabledlocales[i].Id + "\" onclick=\"checkAll('selected_items',true);\">";
                dr["TITLE"] = "<img src='" + parentEnabledlocales[i].FlagUrl + "' />&nbsp;&nbsp;" + parentEnabledlocales[i].CombinedName;
                dr["ID"] = parentEnabledlocales[i].Id;
                dt.Rows.Add(dr);
                }
                
            }
            DataView dv = new DataView(dt);
            TaxonomyItemList.DataSource = dv;
            TaxonomyItemList.DataBind();
        }
        else
        {
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            TaxonomyItemList.GridLines = GridLines.None;
        }
    }
    private string ConfigName(int id)
    {
        switch (id)
        {
            case 0:
                return "Content";
            case 1:
                return "User";
            case 2:
                return "Group";
            default:
                return "Content";
        }
    }
    private void PopulateCommunityGroupGridData(DirectoryAdvancedGroupData cgDirectory)
    {
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=\"checkbox\" name=\"checkall\" onclick=\"checkAll('selected_items',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("COMMUNITYGROUP", _MessageHelper.GetMessage("lbl community group"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, true));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("INFORMATION", "&#160;", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));

        TaxonomyItemList.Columns[2].ItemStyle.VerticalAlign = VerticalAlign.Top;
        TaxonomyItemList.Columns[3].ItemStyle.VerticalAlign = VerticalAlign.Top;

        DataTable dt = new DataTable();
        DataRow dr = default(DataRow);
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("COMMUNITYGROUP", typeof(string)));
        dt.Columns.Add(new DataColumn("INFORMATION", typeof(string)));
        PageSettings();
        if ((cgDirectory != null && cgDirectory.DirectoryItems != null && cgDirectory.DirectoryItems.Length > 0))
        {
            AddDeleteIcon = true;
            foreach (CommunityGroupData item in cgDirectory.DirectoryItems)
            {
                TaxonomyItemCount = TaxonomyItemCount + 1;
                dr = dt.NewRow();
                dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.GroupId + "\" onclick=\"checkAll('selected_items',true);\">";

                string groupurl = null;
                groupurl = _Common.ApplicationPath + "Community/groups.aspx?action=viewgroup&id=" + item.GroupId;
                dr["COMMUNITYGROUP"] = "<img src=\"" + (!string.IsNullOrEmpty(item.GroupImage) ? item.GroupImage : this._Common.AppImgPath + "member_default.gif") + "\" align=\"left\" width=\"55\" height=\"55\" />";
                dr["COMMUNITYGROUP"] += "<a href=\"" + groupurl + "\">";
                dr["COMMUNITYGROUP"] += item.GroupName;
                dr["COMMUNITYGROUP"] += "</a>";
                dr["COMMUNITYGROUP"] += " (" + (item.GroupEnroll ? this._MessageHelper.GetMessage("lbl enrollment open") : this._MessageHelper.GetMessage("lbl enrollment restricted")) + ")";
                dr["COMMUNITYGROUP"] += "<br/>";
                dr["COMMUNITYGROUP"] += item.GroupShortDescription;

                dr["ID"] = item.GroupId;

                dr["INFORMATION"] = this._MessageHelper.GetMessage("content dc label") + " " + item.GroupCreatedDate.ToShortDateString();
                dr["INFORMATION"] += "<br/>";
                dr["INFORMATION"] += this._MessageHelper.GetMessage("lbl members") + ": " + item.TotalMember.ToString();
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            TaxonomyItemList.GridLines = GridLines.None;
        }
        DataView dv = new DataView(dt);
        TaxonomyItemList.DataSource = dv;
        TaxonomyItemList.DataBind();
    }
    private void PopulateUserGridData(DirectoryAdvancedUserData uDirectory)
    {
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=\"checkbox\" name=\"checkall\" onclick=\"checkAll('selected_items',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("USERNAME", _MessageHelper.GetMessage("generic username"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("DISPLAYNAME", _MessageHelper.GetMessage("display name label"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));
        DataTable dt = new DataTable();
        DataRow dr = default(DataRow);
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("DISPLAYNAME", typeof(string)));
        PageSettings();
        if ((uDirectory != null && uDirectory.DirectoryItems != null && uDirectory.DirectoryItems.Length > 0))
        {
            AddDeleteIcon = true;
            foreach (DirectoryUserData item in uDirectory.DirectoryItems)
            {
                TaxonomyItemCount = TaxonomyItemCount + 1;
                dr = dt.NewRow();
                dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.Id + "\" onclick=\"checkAll('selected_items',true);\">";
                // TODO: do we need to put in valid groupid and grouptype fields??
                string userurl = _Common.ApplicationPath + "users.aspx?action=View&LangType=" + TaxonomyLanguage + "&groupid=" + 0 + "&grouptype=" + 0 + "&id=" + item.Id + "&FromUsers=&OrderBy=user_name&callbackpage=Localization/LocaleTaxonomy.aspx?" + Request.ServerVariables["query_string"];
                dr["USERNAME"] = "<a href =\"" + userurl + "\">";
                dr["USERNAME"] += item.Username;
                //"<a href=""taxonomy.aspx?action=viewtree&taxonomyid=" & item.TaxonomyItemId & "&LangType=" & item.TaxonomyItemLanguage & """>" & item.TaxonomyItemTitle & "</a>"
                dr["USERNAME"] += "</a>";

                dr["ID"] = item.Id;
                dr["DISPLAYNAME"] = item.DisplayName;
                dt.Rows.Add(dr);
            }
        }
        else
        {
            dr = dt.NewRow();
            dt.Rows.Add(dr);
            TaxonomyItemList.GridLines = GridLines.None;
        }
        DataView dv = new DataView(dt);
        TaxonomyItemList.DataSource = dv;
        TaxonomyItemList.DataBind();
    }

    private void PopulateContentGridData()
    {
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=\"checkbox\" name=\"checkall\" onclick=\"checkAll('selected_items',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("TITLE", _MessageHelper.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(50), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("LANGUAGE", _MessageHelper.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
        TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("URL", _MessageHelper.GetMessage("generic url link"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(30), false, false));
      //  TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ARCHIVED", _MessageHelper.GetMessage("lbl archived"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));

        DataTable dt = new DataTable();
        DataRow dr = default(DataRow);
        LibraryData libraryInfo = default(LibraryData);
        ContentData contData = new ContentData();
        dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
        dt.Columns.Add(new DataColumn("URL", typeof(string)));
       // dt.Columns.Add(new DataColumn("ARCHIVED", typeof(string)));
        if ((_ViewItem != "folder"))
        {
            PageSettings();
            if ((taxonomy_data != null && taxonomy_data.TaxonomyItems != null && taxonomy_data.TaxonomyItems.Length > 0))
            {
                AddDeleteIcon = true;
                foreach (TaxonomyItemData item in taxonomy_data.TaxonomyItems)
                {
                    TaxonomyItemCount = TaxonomyItemCount + 1;
                    dr = dt.NewRow();
                    dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.TaxonomyItemId + "\" onclick=\"checkAll('selected_items',true);\">";
                    string contenturl = "";
                    switch (Convert.ToInt32(item.ContentType))
                    {
                        case 1:
                            if ((item.ContentSubType == EkEnumeration.CMSContentSubtype.WebEvent))
                            {
                                long fid = _Common.EkContentRef.GetFolderIDForContent(item.TaxonomyItemId);
                                contenturl = "content.aspx?action=ViewContentByCategory&LangType=" + item.TaxonomyItemLanguage + "&id=" + fid + "&callerpage=Localization/LocaleTaxonomy.aspx&origurl=" + EkFunctions.UrlEncode("action=view&view=item&taxonomyid=" + TaxonomyId + "&treeViewId=-1&LangType=" + TaxonomyLanguage);
                            }
                            else
                            {
                                contenturl = "content.aspx?action=View&LangType=" + item.TaxonomyItemLanguage + "&id=" + item.TaxonomyItemId + "&callerpage=Localization/LocaleTaxonomy.aspx&origurl=" + EkFunctions.UrlEncode("action=view&view=item&taxonomyid=" + TaxonomyId + "&treeViewId=-1&LangType=" + TaxonomyLanguage);
                            }
                            break;
                        case 7:
                            // Library Item
                            libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId);
                            contenturl = "library.aspx?LangType=" + libraryInfo.LanguageId + "&action=ViewLibraryItem&id=" + libraryInfo.Id + "&parent_id=" + libraryInfo.ParentId;
                            break;
                        default:
                            contenturl = "content.aspx?action=View&LangType=" + item.TaxonomyItemLanguage + "&id=" + item.TaxonomyItemId + "&callerpage=Localization/LocaleTaxonomy.aspx&origurl=" + EkFunctions.UrlEncode("action=view&view=item&taxonomyid=" + TaxonomyId + "&treeViewId=-1&LangType=" + TaxonomyLanguage);
                            break;
                    }

                    long q = item.TaxonomyItemId;
                    long r = Convert.ToInt64(item.TaxonomyItemLanguage);
                    long s = Convert.ToInt64(item.ContentType);
                    long h = Convert.ToInt64((EkEnumeration.CMSContentSubtype)item.ContentSubType);
                    string i = item.TaxonomyItemTitle.ToString();
                    string n = _MessageHelper.GetMessage("generic Title") + " " + item.TaxonomyItemTitle.ToString();
                    string l = _Common.ApplicationPath + contenturl;
                    string po = item.TaxonomyItemAssetInfo.FileName;
                    string qpo = item.TaxonomyItemAssetInfo.ImageUrl;


                    dr["TITLE"] = m_refContentApi.GetDmsContextMenuHTML(item.TaxonomyItemId, Convert.ToInt64(item.TaxonomyItemLanguage), Convert.ToInt64(item.ContentType), h, item.TaxonomyItemTitle.ToString(), _MessageHelper.GetMessage("generic Title") + " " + item.TaxonomyItemTitle.ToString(), _Common.ApplicationPath + contenturl, item.TaxonomyItemAssetInfo.FileName, item.TaxonomyItemAssetInfo.ImageUrl);
                    dr["ID"] = item.TaxonomyItemId;
                    dr["LANGUAGE"] = item.TaxonomyItemLanguage;
                    if (item.ContentType == "102" || item.ContentType == "106")
                    {
                        libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId);
                        dr["URL"] = libraryInfo.FileName.Replace("//", "/");
                    }
                    else
                    {
                        Ektron.Cms.API.Content.Content api = new Ektron.Cms.API.Content.Content();
                        contData = api.GetContent(item.TaxonomyItemId);
                        //contData = m_refContentApi.GetContentById(item.TaxonomyItemId)
                        dr["URL"] = contData.Quicklink;
                    }
                    //if (item.ContentType == EkEnumeration.CMSContentType.Archive_Content.ToString() || item.ContentType == EkEnumeration.CMSContentType.Archive_Forms.ToString() || item.ContentType == EkEnumeration.CMSContentType.Archive_Media.ToString() || (Convert.ToInt32(item.ContentType) >= EkConstants.Archive_ManagedAsset_Min && Convert.ToInt32(item.ContentType) < EkConstants.Archive_ManagedAsset_Max && Convert.ToInt32(item.ContentType) != 3333 && Convert.ToInt32(item.ContentType) != 1111))
                    //{
                    //    dr["ARCHIVED"] = "<span class=\"Archived\"></span>";
                    //}
                    dt.Rows.Add(dr);
                }
               
            }
            else
            {
                dr = dt.NewRow();
                dt.Rows.Add(dr);
                TaxonomyItemList.GridLines = GridLines.None;
            }
        }
        else
        {
            VisiblePageControls(false);
            //TaxonomyFolderSyncData[] taxonomy_sync_folder = null;
            //TaxonomyBaseRequest tax_sync_folder_req = new TaxonomyBaseRequest();
            //tax_sync_folder_req.TaxonomyId = TaxonomyId;
            //tax_sync_folder_req.TaxonomyLanguage = TaxonomyLanguage;
            //taxonomy_sync_folder = _Content.GetAllAssignedCategoryFolder(tax_sync_folder_req);
            Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy api = new Ektron.Cms.BusinessObjects.Localization.LocaleTaxonomy(_Common.RequestInformationRef);
            List<LocalizableItem> folderItemList = api.GetList(TaxonomyId, _Common.ContentLanguage, false);
            int totalFolderCount = 0;
            if ((folderItemList != null && folderItemList.Count > 0))
            {
                
               
                for (int i = 0; i <= folderItemList.Count - 1; i++)
                {
                    if (folderItemList[i].LocalizableType == LocalizableCmsObjectType.FolderContents)
                    {
                        AddDeleteIcon = true;
                        totalFolderCount = totalFolderCount + 1;
                        //get the folder data from folder id.
                        FolderData assignedFolderData = folderApi.GetFolder(folderItemList[i].Id);
                        TaxonomyItemCount = TaxonomyItemCount + 1;
                        dr = dt.NewRow();
                        dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + assignedFolderData.Id + "\" onclick=\"checkAll('selected_items',true);\">";

                        string contenturl = null;
                        contenturl =_Common.ApplicationPath + "content.aspx?action=ViewContentByCategory&id=" + assignedFolderData.Id + "&treeViewId=0";

                        dr["TITLE"] = "<a href=\"" + contenturl + "\">";
                        dr["TITLE"] += "<img src=\"";
                        switch ((EkEnumeration.FolderType)assignedFolderData.FolderType)
                        {
                            case EkEnumeration.FolderType.Catalog:
                                dr["TITLE"] += m_refContentApi.AppPath + "images/ui/icons/folderGreen.png";
                                break;
                            case EkEnumeration.FolderType.Community:
                                dr["TITLE"] += m_refContentApi.AppPath + "images/ui/icons/folderCommunity.png";
                                break;
                            case EkEnumeration.FolderType.Blog:
                                dr["TITLE"] += m_refContentApi.AppPath + "images/ui/icons/folderBlog.png";
                                break;
                            case EkEnumeration.FolderType.DiscussionBoard:
                                dr["TITLE"] += m_refContentApi.AppPath + "images/ui/icons/folderBoard.png";
                                break;
                            case EkEnumeration.FolderType.DiscussionForum:
                                dr["TITLE"] += m_refContentApi.AppPath + "images/ui/icons/folderBoard.png";
                                break;
                            default:
                                dr["TITLE"] += m_refContentApi.AppPath + "images/ui/icons/folder.png";
                                break;
                        }
                        dr["TITLE"] += "\"></img>";
                        dr["TITLE"] += "</a><a href=\"" + contenturl + "\">";
                        dr["TITLE"] += assignedFolderData.Name;
                        //& GetRecursiveTitle(item.FolderRecursive)
                        dr["TITLE"] += "</a>";

                        dr["ID"] = assignedFolderData.Id;
                        dr["LANGUAGE"] = TaxonomyLanguage;
                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                dr = dt.NewRow();
                dt.Rows.Add(dr);
                TaxonomyItemList.GridLines = GridLines.None;
            }
            //ltrItemCount.Text = totalFolderCount.ToString();
        }
        DataView dv = new DataView(dt);
        TaxonomyItemList.DataSource = dv;
        TaxonomyItemList.DataBind();
    }
    private string GetRecursiveTitle(bool value)
    {
        string result = "";
        if ((value))
        {
            result = "<span class=\"important\"> (Recursive)</span>";
        }
        return result;
    }
    private void TaxonomyToolBar()
    {
        string IFrameVariable = "";
        string strDeleteMsg = "";
        if ((Request.QueryString["iframe"] == "true"))
        {
            IFrameVariable = "&iframe=true";
        }
        if ((TaxonomyParentId > 0))
        {
            strDeleteMsg = _MessageHelper.GetMessage("alt delete button text (category)");
            m_strDelConfirm = _MessageHelper.GetMessage("delete locale category confirm");
            m_strDelItemsConfirm = _MessageHelper.GetMessage("delete locale category items confirm");
            m_strSelDelWarning = _MessageHelper.GetMessage("select category item missing warning");
        }
        else
        {
            strDeleteMsg = _MessageHelper.GetMessage("alt delete button text (taxonomy)");
            m_strDelConfirm = _MessageHelper.GetMessage("delete locale taxonomy confirm");
            m_strDelItemsConfirm = _MessageHelper.GetMessage("delete locale taxonomy items confirm");
            m_strSelDelWarning = _MessageHelper.GetMessage("select taxonomy item missing warning");
        }
        divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("view locale taxonomy page title") + " \"" + _TaxonomyName + "\"" + "&nbsp;&nbsp;<img style='vertical-align:middle;' src='" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "' />");
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + "\n");

		if ((Request.QueryString["iframe"] == "true"))
		{
			string parentaction = "javascript:parent.CancelIframe();";
			if ((Request.Params["ccp"] != null))
			{
				parentaction = "javascript:parent.CloseChildPage();";
			}
			result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/cancel.png", "#", _MessageHelper.GetMessage("generic Cancel"), _MessageHelper.GetMessage("generic Cancel"), "onClick=\"" + parentaction + "\"", StyleHelper.CancelButtonCssClass, true));
		}

        if ((AddDeleteIcon))
        {
            removeItemsWrapper.Visible = true;
        }
        //if (((TaxonomyCategoryCount > 1) | (TaxonomyItemCount > 1)))
        //{
        //    result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/arrowUpDown.png", "LocaleTaxonomy.aspx?action=reorder&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&reorder=category" + "&LangType=" + TaxonomyLanguage + IFrameVariable, _MessageHelper.GetMessage("reorder taxonomy page title"), _MessageHelper.GetMessage("reorder taxonomy page title"), ""));
        //}/WorkArea/
        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/translationAddLanguage.png", "LocaleTaxonomy.aspx?view=" + _ViewItem + "&action=additem&type=locales&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage + IFrameVariable, _MessageHelper.GetMessage("assign languages to locale taxonomy page title"), _MessageHelper.GetMessage("assign languages to locale taxonomy page title"), "", StyleHelper.AddTranslationButtonCssClass));
		result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/contentStackAdd.png", "LocaleTaxonomy.aspx?view=" + _ViewItem + "&action=additem&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage + IFrameVariable, _MessageHelper.GetMessage("assign items to locale taxonomy page title"), _MessageHelper.GetMessage("assign items to locale taxonomy page title"), "", StyleHelper.AssignItemsButtonCssClass));
		result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/folderAdd.png", "LocaleTaxonomy.aspx?view=" + _ViewItem + "&action=addfolder&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage + IFrameVariable, _MessageHelper.GetMessage("assign folders to locale taxonomy page title"), _MessageHelper.GetMessage("assign folders to locale taxonomy page title"), "", StyleHelper.AssignFoldersButtonCssClass));

        result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/contentEdit.png", "LocaleTaxonomy.aspx?view="+_ViewItem +"&action=edit&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage + IFrameVariable, _MessageHelper.GetMessage("alt edit button text (locale taxonomy)"), _MessageHelper.GetMessage("btn locale edit"), "", StyleHelper.EditButtonCssClass));
        //if ((TaxonomyParentId == 0))
        //{
        //    result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/translation.png", "#", _MessageHelper.GetMessage("alt export taxonomy"), _MessageHelper.GetMessage("btn export taxonomy"), "onclick=\"window.open('taxonomy_imp_exp.aspx?action=export&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage + "','exptaxonomy','status=0,toolbar=0,location=0,menubar=0,directories=0,resizable=0,scrollbars=1,height=100px,width=200px');void(0);\""));
        //}
        // /WorkArea/images/application/flags/flag0000.gif
		result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/delete.png", "#", _MessageHelper.GetMessage("generic delete title"), _MessageHelper.GetMessage("alt delete button text (locale taxonomy)"), "onclick=\"return DeleteItem();\"", StyleHelper.DeleteButtonCssClass));

        if ((Request.QueryString["iframe"] != "true"))
        {
            result.Append("<td>&nbsp;|&nbsp;</td>");
            result.Append("<td nowrap=\"true\">");
            string addDD = null;
            addDD = GetLanguageForTaxonomy(TaxonomyId, "", false, false, "javascript:TranslateTaxonomy(" + TaxonomyId + ", " + TaxonomyParentId + ", this.value);");
            if (!string.IsNullOrEmpty(addDD))
            {
                addDD = "&nbsp;" + _MessageHelper.GetMessage("add title") + ":&nbsp;" + addDD;
            }
            if (_Common.EnableMultilingual == 1)
            {
                result.Append("View In:&nbsp;" + GetLanguageForTaxonomy(TaxonomyId, "", true, false, "javascript:LoadLanguage(this.value);") + "&nbsp;" + addDD + "<br>");
            }
            result.Append("</td>");
        }

        result.Append("<td>&nbsp;</td>");
        result.Append(ViewTypeDropDown());
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + _StyleHelper.GetHelpButton("ViewLocaleTaxonomy", "") + "</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
        result = null;
    }
    private string ViewTypeDropDown()
    {
        StringBuilder result = new StringBuilder();
        result.Append("<td class=\"label\">");
        result.Append(_MessageHelper.GetMessage("lbl View") + ":");
        result.Append("</td>");
        result.Append("<td>");
        result.Append("<select id=\"selviewtype\" name=\"selviewtype\" onchange=\"LoadViewType(this.value);\">");
        result.Append("<option value=\"locale\"  " + FindSelected("locale") + ">").Append(this._MessageHelper.GetMessage("lbl languages button text")).Append("</option>");
        result.Append("<option value=\"folder\" " + FindSelected("folder") + ">").Append(this._MessageHelper.GetMessage("lbl folders")).Append("</option>");
        result.Append("<option value=\"item\"  " + FindSelected("item") + ">").Append(this._MessageHelper.GetMessage("content button text")).Append("</option>");
        result.Append("</select>");
        result.Append("</td>");
        return result.ToString();
    }

    private string FindSelected(string chk)
    {
        string val = "";
        if ((_ViewItem.ToLower() == chk))
        {
            val = " selected ";
        }
        return val;
    }

    private string GetLanguageForTaxonomy(long TaxonomyId, string BGColor, bool ShowTranslated, bool ShowAllOpt, string onChangeEv)
    {
        string result = "";
        string frmName = "";
        IList<LanguageData> result_language = null;
        TaxonomyLanguageRequest taxonomy_language_request = new TaxonomyLanguageRequest();
        taxonomy_language_request.TaxonomyId = TaxonomyId;

        if ((ShowTranslated))
        {
            taxonomy_language_request.IsTranslated = true;
            result_language = _Content.LoadLanguageForTaxonomy(taxonomy_language_request);
            frmName = "frm_translated";
        }
        else
        {
            taxonomy_language_request.IsTranslated = false;
            result_language = _Content.LoadLanguageForTaxonomy(taxonomy_language_request);
            frmName = "frm_nontranslated";
        }

        result = "<select id=\"" + frmName + "\" name=\"" + frmName + "\" onchange=\"" + onChangeEv + "\">" + "\n";

        if ((Convert.ToBoolean(ShowAllOpt)))
        {
            if (TaxonomyLanguage == -1)
            {
                result = result + "<option value=\"-1\" selected>All</option>";
            }
            else
            {
                result = result + "<option value=\"-1\">All</option>";
            }
        }
        else
        {
            if ((ShowTranslated == false))
            {
                result = result + "<option value=\"0\">-select language-</option>";
            }
        }
        if (((result_language != null) && (result_language.Count > 0) && (_Common.EnableMultilingual == 1)))
        {
            foreach (LanguageData language in result_language)
            {
                if (TaxonomyLanguage == language.Id)
                {
                    result = result + "<option value=" + language.Id + " selected>" + language.Name + "</option>";
                }
                else
                {
                    result = result + "<option value=" + language.Id + ">" + language.Name + "</option>";
                }
            }
        }
        else
        {
            result = "";
        }
        if ((result.Length > 0))
        {
            result = result + "</select>";
        }
        return (result);
    }
    private void PageSettings()
    {
        if ((m_intTotalPages <= 1))
        {
            VisiblePageControls(false);
        }
        else
        {
            VisiblePageControls(true);
            TotalPages.Text = m_intTotalPages.ToString();
            CurrentPage.Text = m_intCurrentPage.ToString();
            PreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            NextPage.Enabled = true;
            LastPage.Enabled = true;
            if (m_intCurrentPage == 1)
            {
                PreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (m_intCurrentPage == m_intTotalPages)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }

    private void VisiblePageControls(bool flag)
    {
        TotalPages.Visible = flag;
        CurrentPage.Visible = flag;
        PreviousPage.Visible = flag;
        NextPage.Visible = flag;
        LastPage.Visible = flag;
        FirstPage.Visible = flag;
        PageLabel.Visible = flag;
        OfLabel.Visible = flag;
    }

    public void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                m_intCurrentPage = 1;
                break;
            case "Last":
                m_intCurrentPage = Int32.Parse(TotalPages.Text);
                break;
            case "Next":
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) + 1;
                break;
            case "Prev":
                m_intCurrentPage = Int32.Parse(CurrentPage.Text) - 1;
                break;
        }
        DisplayPage();
        isPostData.Value = "true";
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDmsMenuJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronDmsMenuIE6Css, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE6);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

    protected void AssignTextStrings()
    {
        removeItemsLink.Text = _MessageHelper.GetMessage("remove taxonomy items");
        removeItemsLink.ToolTip = _MessageHelper.GetMessage("alt remove button text (taxonomyitems)");
        //result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath & "images/ui/Icons/remove.png", "#", _MessageHelper.GetMessage("alt remove button text (taxonomyitems)"), _MessageHelper.GetMessage("btn remove"), "onclick=""return DeleteItem('items');"""))
    }
    private void DisplayTaxonomyMetadata()
    {
        // Set hidden values here
        customPropertyObjectId.Value = TaxonomyId.ToString();
        customPropertyRecordsPerPage.Value = _Common.RequestInformationRef.PagingSize.ToString();
    }

    private string GetValueDropDown(ref Ektron.Cms.Common.CustomPropertyData _propertyDataList, int count)
    {
        StringBuilder result = new StringBuilder();
        int iObj = 0;

        result.Append("<select disabled name=\"selCustPropVal" + count + "\" id=\"selCustPropVal" + count + "\">");
        if (((_propertyDataList != null)))
        {
            for (iObj = 0; iObj <= _propertyDataList.Items.Count - 1; iObj++)
            {
                if ((_propertyDataList.Items[iObj].IsDefault))
                {
                    result.Append("<option selected value=\"" + _propertyDataList.Items[iObj].PropertyValue + "\">");
                    result.Append(_propertyDataList.Items[iObj].PropertyValue);
                    result.Append("</option>");
                }
                else
                {
                    result.Append("<option value=\"" + _propertyDataList.Items[iObj].PropertyValue + "\">");
                    result.Append(_propertyDataList.Items[iObj].PropertyValue);
                    result.Append("</option>");
                }
            }
        }
        result.Append("</select>");

        return result.ToString();
    }
}
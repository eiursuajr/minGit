//-----------------------------------------------------------------------
// <copyright file="addLocaleTaxonomy.ascx.cs" company="Ektron" author="Rama Ila">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
// Adds locale taxonomy or translation package to CMS System.
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Data;
using System.Diagnostics;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Core.CustomProperty;
using Ektron.Cms.Framework.Core.CustomProperty;


public partial class addLocaleTaxonomy : System.Web.UI.UserControl
{
    protected ContentAPI m_refApi = new ContentAPI();
    protected StyleHelper m_refstyle = new StyleHelper();
    protected string AppImgPath = "";
    protected string AppPath = "";
    protected EkMessageHelper m_refMsg;
    protected string m_strPageAction = "";
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected int TaxonomyLanguage = -1;
    protected long TaxonomyId = 0;
    protected long TaxonomyParentId = 0;
    protected LanguageData language_data;
    protected string TitleLabel = "taxonomytitle";
    protected string DescriptionLabel = "taxonomydescription";
    protected string m_strCurrentBreadcrumb = "";
    protected bool m_bSynchronized = true;
    protected List<CustomPropertyData> _customPropertyDataList;
    protected CustomProperty _customProperty = new CustomProperty();
    protected CustomPropertyData _customPropertyData = new CustomPropertyData();
    protected CustomPropertyObjectData _customPropertyObjectData = new CustomPropertyObjectData();
    protected CustomPropertyObjectBL _coreCustomProperty = new CustomPropertyObjectBL();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        { 
            m_refMsg = m_refApi.EkMsgRef;
            AppImgPath = m_refApi.AppImgPath;
            AppPath = m_refApi.AppPath;
            m_strPageAction = Request.QueryString["action"];
            Utilities.SetLanguage(m_refApi);                    
            TaxonomyLanguage = m_refApi.ContentLanguage;
            if ((TaxonomyLanguage == -1))
            {
                TaxonomyLanguage = m_refApi.DefaultContentLanguage;
            }
            if ((Request.QueryString["taxonomyid"] != null))
            {
                TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
            }
            if ((Request.QueryString["parentid"] != null))
            {
                TaxonomyParentId = Convert.ToInt64(Request.QueryString["parentid"]);
                if ((TaxonomyParentId > 0))
                {
                    TitleLabel = "categorytitle";
                    DescriptionLabel = "categorydescription";
                }
            }


            if ((Page.IsPostBack))
            {
                TaxonomyData taxonomy_data = new TaxonomyData();
                taxonomy_data.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Locale;
                taxonomy_data.TaxonomyDescription = Request.Form[taxonomydescription.UniqueID];
                taxonomy_data.TaxonomyName = Request.Form[taxonomytitle.UniqueID];
                taxonomy_data.TaxonomyLanguage = TaxonomyLanguage;
                taxonomy_data.TaxonomyParentId = TaxonomyParentId;
               // taxonomy_data.TaxonomyImage = Request.Form[taxonomy_image.UniqueID];
               // taxonomy_data.CategoryUrl = Request.Form[categoryLink.UniqueID];
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
                //    taxonomy_data.Visible = true;
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
                
                //If (TaxonomyId <> 0) Then
                //  taxonomy_data.TaxonomyId = TaxonomyId
                //End If
                m_refContent = m_refApi.EkContentRef;
                TaxonomyId = m_refContent.CreateTaxonomy(taxonomy_data);
                //add the default language by Default.
                TaxonomyRequest item_request = new TaxonomyRequest();
                item_request.TaxonomyId = TaxonomyId;
                item_request.TaxonomyIdList =Convert.ToString(TaxonomyLanguage);
                item_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Locale;
                item_request.TaxonomyLanguage = TaxonomyLanguage;
                m_refContent.AddTaxonomyItem(item_request);
                if (Request.Form[alllanguages.UniqueID] == "false")
                {
                    m_refContent.UpdateTaxonomyVisible(TaxonomyId, -1, false);
                }
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
                    if ((!(string.IsNullOrEmpty(strConfig))))
                    {
                        m_refContent.UpdateTaxonomyConfig(TaxonomyId, strConfig);
                    }
                }
                //++++++++++++++++++++++++++++++++++++++++++++++++
                //+++++++++ Adding MetaData Information '+++++++++
                //++++++++++++++++++++++++++++++++++++++++++++++++
                //Commeneted as per Doug suggestion
                //AddCustomProperties();

                if ((Request.QueryString["iframe"] == "true"))
                {
                    Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
                }
                else
                {
                    //this should jump back to taxonomy that was added
                    //Response.Redirect("taxonomy.aspx?rf=1", True)
                    Response.Redirect("LocaleTaxonomy.aspx?action=view&view=locale&taxonomyid=" + TaxonomyId + "&rf=1", true);
                }
            }
            else
            {
                m_refContent = m_refApi.EkContentRef;
                TaxonomyRequest req = new TaxonomyRequest();
                req.TaxonomyId = TaxonomyParentId;
                req.TaxonomyLanguage = TaxonomyLanguage;

                if (TaxonomyParentId > 0)
                {
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyParentId, TaxonomyLanguage);
                }
                else if (TaxonomyId > 0)
                {
                    m_bSynchronized = m_refContent.IsSynchronizedTaxonomy(TaxonomyId, TaxonomyLanguage);
                }
                //if (!m_bSynchronized)
                //{
                //    tr_enableDisable.Visible = false;
                //}
                TaxonomyBaseData data = m_refContent.ReadTaxonomy(ref req);
                if ((data == null))
                {
                    EkException.ThrowException(new Exception("Invalid taxonomy ID: " + TaxonomyId + " parent: " + TaxonomyParentId));
                }
                language_data = (new SiteAPI()).GetLanguageById(TaxonomyLanguage);
                if (((language_data != null) && (m_refApi.EnableMultilingual == 1)))
                {
                    lblLanguage.Text = "[" + language_data.Name + "]";
                }
              //  taxonomy_image_thumb.ImageUrl = m_refApi.AppImgPath + "spacer.gif";
                m_strCurrentBreadcrumb = data.TaxonomyPath.Remove(0, 1).Replace("\\", " > ");
                if ((string.IsNullOrEmpty(m_strCurrentBreadcrumb)))
                {
                    m_strCurrentBreadcrumb = "Root";
                }
                //if ((TaxonomyParentId == 0))
                //{
                //    inherittemplate.Visible = false;
                //    lblInherited.Text = "No";
                //}
                //else
                //{
                //    inherittemplate.Checked = true;
                //    taxonomytemplate.Enabled = false;
                //    inherittemplate.Visible = true;
                //    lblInherited.Text = "";
                //}
             //   TemplateData[] templates = null;
               // templates = m_refApi.GetAllTemplates("TemplateFileName");                
                //taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem("-select template-", "0"));
                //if ((templates != null && templates.Length > 0))
                //{
                //    for (int i = 0; i <= templates.Length - 1; i++)
                //    {
                //        taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem(templates[i].FileName, templates[i].Id.ToString()));
                //    }
                //}

               // inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)");
               // inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)");
                //if ((TaxonomyParentId == 0))
                //{
                //    tr_config.Visible = true;
                //}
                //else
                //{
                //    tr_config.Visible = false;
                //}
               // chkConfigContent.Checked = true;
                //LoadCustomPropertyList();
                TaxonomyToolBar();
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
            //Do nothing
        }
        catch (Exception ex)
        {
            Response.Redirect(AppPath + "reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message + ".") + "&LangType=" + TaxonomyLanguage, false);
        }
    }


    private void TaxonomyToolBar()
    {
        if ((TaxonomyParentId == 0))
        {
            txtTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("add translation category page title"));
        }
        else
        {
            txtTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("add category page title"));
        }

        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<table><tr>" + Constants.vbCrLf);

		if ((Request.QueryString["iframe"] == "true"))
		{
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onClick=\"javascript:parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true));
		}
		else
		{
			if (TaxonomyParentId == 0)
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "LocaleTaxonomy.aspx", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "LocaleTaxonomy.aspx?action=view&taxonomyid=" + TaxonomyParentId, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
		}

        if ((TaxonomyParentId == 0))
        {
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (locale taxonomy)"), m_refMsg.GetMessage("btn Save"), "onclick=\"javascript:if(SetPropertyIds()){Validate();}\"", StyleHelper.SaveButtonCssClass, true));
        }
        else
        {
			result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (locale category)"), m_refMsg.GetMessage("btn Save"), "onclick=\"javascript:if(SetPropertyIds()){Validate();}\"", StyleHelper.SaveButtonCssClass, true));
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refstyle.GetHelpButton("AddLocaleTaxonomy", "") + "</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
        result = null;
    }




}

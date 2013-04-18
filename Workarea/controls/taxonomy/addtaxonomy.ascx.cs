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
//using System.DateTime;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Core.CustomProperty;
using Ektron.Cms.Framework.Core.CustomProperty;
using Ektron.Cms.Content;
using Ektron.Cms.Framework.UI;




	public partial class addtaxonomy : System.Web.UI.UserControl
	{
		
		
		protected ContentAPI m_refApi = new ContentAPI();
		protected StyleHelper m_refstyle = new StyleHelper();
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected EkMessageHelper m_refMsg;
		protected string m_strPageAction = "";
		protected EkContent m_refContent;
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
        protected string closeWindow = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				m_refMsg = m_refApi.EkMsgRef;
				AppImgPath = m_refApi.AppImgPath;
				AppPath = m_refApi.AppPath;
                ltr_sitepath.Text = m_refApi.SitePath;
				m_strPageAction = Request.QueryString["action"];
				Utilities.SetLanguage(m_refApi);
				TaxonomyLanguage = m_refApi.ContentLanguage;
				if (TaxonomyLanguage == -1)
				{
					TaxonomyLanguage = m_refApi.DefaultContentLanguage;
				}
				if (Request.QueryString["taxonomyid"] != null)
				{
					TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
				}
				if (Request.QueryString["parentid"] != null)
				{
					TaxonomyParentId = Convert.ToInt64(Request.QueryString["parentid"]);
					if (TaxonomyParentId > 0)
					{
						TitleLabel = "categorytitle";
						DescriptionLabel = "categorydescription";
					}
				}
                if (Request.QueryString["iframe"] == "true")
                {
                    styles.Visible = true;
                }

                chkConfigContent.Text = chkConfigContent.ToolTip = m_refMsg.GetMessage("content text");
                chkConfigUser.Text = chkConfigUser.ToolTip = m_refMsg.GetMessage("generic user");
                chkConfigGroup.Text = chkConfigGroup.ToolTip = m_refMsg.GetMessage("lbl group");

				if (Page.IsPostBack)
				{
					TaxonomyData taxonomy_data = new TaxonomyData();
					taxonomy_data.TaxonomyDescription = Request.Form[taxonomydescription.UniqueID];
					taxonomy_data.TaxonomyName = Request.Form[taxonomytitle.UniqueID];
					taxonomy_data.TaxonomyLanguage = TaxonomyLanguage;
					taxonomy_data.TaxonomyParentId = TaxonomyParentId;
					taxonomy_data.TaxonomyImage = Request.Form[taxonomy_image.UniqueID];
					taxonomy_data.CategoryUrl = Request.Form[categoryLink.UniqueID];
					if (tr_enableDisable.Visible == true)
					{
						if (! string.IsNullOrEmpty(Request.Form[chkEnableDisable.UniqueID]))
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
						taxonomy_data.Visible = true;
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
					
					//If (TaxonomyId <> 0) Then
					//  taxonomy_data.TaxonomyId = TaxonomyId
					//End If
					m_refContent = m_refApi.EkContentRef;
					TaxonomyId = m_refContent.CreateTaxonomy(taxonomy_data);
					if (Request.Form[alllanguages.UniqueID] == "false")
					{
						m_refContent.UpdateTaxonomyVisible(TaxonomyId, -1, false);
					}
                    m_refContent.UpdateTaxonomySynchronization(TaxonomyId, GetCheckBoxValue(chkTaxSynch));
                    // chkCreateLang
					if (TaxonomyParentId == 0)
					{
						string strConfig = string.Empty;
						if (! string.IsNullOrEmpty(Request.Form[chkConfigContent.UniqueID]))
						{
							strConfig = "0";
						}
						if (! string.IsNullOrEmpty(Request.Form[chkConfigUser.UniqueID]))
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
						if (! string.IsNullOrEmpty(Request.Form[chkConfigGroup.UniqueID]))
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
					//++++++++++++++++++++++++++++++++++++++++++++++++
					//+++++++++ Adding MetaData Information '+++++++++
					//++++++++++++++++++++++++++++++++++++++++++++++++
					AddCustomProperties();
                    //++++++++++++++++++++++++++++++++++++++++++++++++
                    //+++++++++ Adding Other Langs (If Applicable) +++
                    //++++++++++++++++++++++++++++++++++++++++++++++++
                    if (!GetCheckBoxValue(chkTaxSynch) && GetCheckBoxValue(chkCreateLang))
                    {   // Figure out which ones were created.
                        IList<LanguageData> result_language = null;
			            TaxonomyLanguageRequest taxonomy_language_request = new TaxonomyLanguageRequest();
			            taxonomy_language_request.TaxonomyId = TaxonomyId;
                        taxonomy_language_request.IsTranslated = false; // langs that we still need
                        result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request);
                        if (result_language != null)
                        {
                            foreach(LanguageData lang in result_language)
                            {
                                UsingLanguage(m_refContent.RequestInformation, lang.Id, delegate
                                {
                                    taxonomy_data.TaxonomyId = TaxonomyId;
                                    taxonomy_data.LanguageId = lang.Id;
                                    m_refContent.CreateTaxonomy(taxonomy_data);
                                });
                            }
                        }
                    }

					if (Request.QueryString["iframe"] == "true")
					{
                        Packages.EktronCoreJS.Register(this);
						closeWindow = "Ektron.ready(function(){parent.CloseChildPage();});";
					}
					else
					{
						//this should jump back to taxonomy that was added
						//Response.Redirect("taxonomy.aspx?rf=1", True)
                        Response.Redirect("taxonomy.aspx?action=view&view=item&taxonomyid=" + TaxonomyId + "&rf=1&reloadtrees=Tax", true);
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
                    chkTaxSynch.Checked = m_bSynchronized;
                    chkTaxSynch.Attributes.Add("onclick", 
                        string.Format("ToggleTaxSynch(this, '{0}');", chkCreateLang.ClientID)
                        );
                    if (m_bSynchronized)
                    {
                        fs_taxsynccreate.Attributes.Add("style", "display:none;");
                    }
					if (! m_bSynchronized)
					{
						tr_enableDisable.Visible = false;
					}
					TaxonomyBaseData data = m_refContent.ReadTaxonomy(ref req);
					if (data == null)
					{
						EkException.ThrowException(new Exception("Invalid taxonomy ID: " + TaxonomyId + " parent: " + TaxonomyParentId));
					}
					language_data = (new SiteAPI()).GetLanguageById(TaxonomyLanguage);
					if ((language_data != null) && (m_refApi.EnableMultilingual == 1))
					{
						lblLanguage.Text = "[" + language_data.Name + "]";
					}
					taxonomy_image_thumb.ImageUrl = m_refApi.AppImgPath + "spacer.gif";
					m_strCurrentBreadcrumb = (string) (data.TaxonomyPath.Remove(0, 1).Replace("\\", " > "));
					if (m_strCurrentBreadcrumb == "")
					{
						m_strCurrentBreadcrumb = "Root";
					}
					if (TaxonomyParentId == 0)
					{
						inherittemplate.Visible = false;
						lblInherited.Text = "No";
						lblInherited.ToolTip = lblInherited.Text;
					}
					else
					{
						inherittemplate.Checked = true;
						taxonomytemplate.Enabled = false;
						inherittemplate.Visible = true;
						lblInherited.Text = "";
					}
					TemplateData[] templates = null;
					templates = m_refApi.GetAllTemplates("TemplateFileName");
                    taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem("- " + m_refMsg.GetMessage("generic select template") + " -", "0"));
					if ((templates != null)&& templates.Length > 0)
					{
						for (int i = 0; i <= templates.Length - 1; i++)
						{
							taxonomytemplate.Items.Add(new System.Web.UI.WebControls.ListItem(templates[i].FileName, templates[i].Id.ToString()));
						}
					}
					
					inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)");
					inherittemplate.Attributes.Add("onclick", "OnInheritTemplateClicked(this)");
					if (TaxonomyParentId == 0)
					{
						tr_config.Visible = true;
					}
					else
					{
						tr_config.Visible = false;
					}
					chkConfigContent.Checked = true;
					LoadCustomPropertyList();
					TaxonomyToolBar();
				}
			}
			catch (System.Threading.ThreadAbortException)
			{
				//Do nothing
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message + ".") + "&LangType=" + TaxonomyLanguage), false);
			}
		}
		
		private void TaxonomyToolBar()
		{
			if (TaxonomyParentId == 0)
			{
				txtTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("add taxonomy page title"));
			}
			else
			{
				txtTitleBar.InnerHtml = m_refstyle.GetTitleBar(m_refMsg.GetMessage("add category page title"));
			}
			
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");

			if (Request.QueryString["iframe"] == "true")
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("generic Cancel"), m_refMsg.GetMessage("generic Cancel"), "onClick=\"javascript:parent.CancelIframe();\"", StyleHelper.CancelButtonCssClass, true));
			}
			else
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", (string)("taxonomy.aspx?action=view&taxonomyid=" + TaxonomyParentId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			if (TaxonomyParentId == 0)
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (taxonomy)"), m_refMsg.GetMessage("btn Save"), "onclick=\"javascript:if(SetPropertyIds()){Validate();}\"", StyleHelper.SaveButtonCssClass, true));
			}
			else
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (category)"), m_refMsg.GetMessage("btn Save"), "onclick=\"javascript:if(SetPropertyIds()){Validate();}\"", StyleHelper.SaveButtonCssClass, true));
			}
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refstyle.GetHelpButton("AddTaxonomyOrCategory", "") + "</td>");
			result.Append("</tr></table>");
			htmToolBar.InnerHtml = result.ToString();
			result = null;
		}

		private void LoadCustomPropertyList()
		{
			
			PagingInfo pageInfo = new PagingInfo();
			pageInfo.CurrentPage = 1;
			pageInfo.RecordsPerPage = 99999;
			
			int i = 0;
			int j = 1;
            //insert default item
            availableCustomProp.Items.Insert(0, new ListItem(m_refMsg.GetMessage("li taxonomy select default text"), "-1"));
			_customPropertyDataList = (List<CustomPropertyData>)_customProperty.GetList(EkEnumeration.CustomPropertyObjectType.TaxonomyNode, m_refApi.ContentLanguage, pageInfo);
			for (i = 0; i <= _customPropertyDataList.Count - 1; i++)
			{
				if (_customPropertyDataList[i].IsEnabled)
				{
					availableCustomProp.Items.Insert(j, _customPropertyDataList[i].PropertyName);
					availableCustomProp.Items[j].Value =Convert.ToString( _customPropertyDataList[i].PropertyId);
					j++;
				}
			}
			
			availableCustomProp.DataBind();
			
		}
		
		private void AddCustomProperties()
		{
			int i = 0;
			CustomProperty cp = new CustomProperty();
			CustomPropertyObject cpo = new CustomPropertyObject();
			string[] selectedIds = null;
			string[] selectedValues = null;
			
			if (Request.Form[hdnSelectedIDS.UniqueID] != "")
			{
				selectedIds = Request.Form[hdnSelectedIDS.UniqueID].Remove(System.Convert.ToInt32(Request.Form[hdnSelectedIDS.UniqueID].Length - 1), 1).Split(";".ToCharArray());
			}
			if (Request.Form[hdnSelectValue.UniqueID] != "")
			{
				selectedValues = Request.Form[hdnSelectValue.UniqueID].Remove(System.Convert.ToInt32(Request.Form[hdnSelectValue.UniqueID].Length - 1), 1).Split(";".ToCharArray());
			}
			
			if ((selectedIds != null)&& (selectedValues != null))
			{
				if (selectedIds.Length == selectedValues.Length)
				{
					for (i = 0; i <= selectedIds.Length - 1; i++)
					{
						CustomPropertyData  customPropertyData = cp.GetItem(Convert.ToInt64 (selectedIds[i]), m_refApi.ContentLanguage);
						CustomPropertyObjectData data = new CustomPropertyObjectData(TaxonomyId, m_refApi.ContentLanguage,Convert.ToInt64 ( selectedIds[i]), EkEnumeration.CustomPropertyObjectType.TaxonomyNode);
						
						if ((customPropertyData != null) && (data != null))
						{
							
							string inputValue = HttpUtility.UrlDecode((string) (selectedValues[i].ToString()));
							
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

        private void UsingLanguage(EkRequestInformation reqInfo, int languageId, Action procedure)
        {
            int savedContentLanguage = reqInfo.ContentLanguage;
            try
            {
                reqInfo.ContentLanguage = languageId;
                procedure();
            }
            finally
            {
                reqInfo.ContentLanguage = savedContentLanguage;
            }
        }
	}
	


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
using System.IO;

public partial class viewattributes : System.Web.UI.UserControl
{
		protected CommonApi m_refCommon = new CommonApi();
		protected StyleHelper m_refstyle = new StyleHelper();
		protected string AppImgPath = "";
		protected EkMessageHelper m_refMsg;
		protected string m_strPageAction = "";
		protected Ektron.Cms.Content.EkContent m_refContent;
		protected long TaxonomyId = 0;
		protected int TaxonomyLanguage = -1;
		protected LanguageData language_data;
		protected TaxonomyRequest taxonomy_request;
		protected TaxonomyData taxonomy_data;
		protected long TaxonomyParentId = 0;
		protected string m_strViewItem = "item";
		protected bool AddDeleteIcon = false;
		protected string m_strTaxonomyName = "";
		protected int m_intCurrentPage = 1;
		protected int m_intTotalPages = 1;
		protected string m_strDelConfirm = "";
		protected string m_strDelItemsConfirm = "";
		protected string m_strSelDelWarning = "";
		protected string m_strCurrentBreadcrumb = "";
		protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_refMsg = m_refCommon.EkMsgRef;
			AppImgPath = m_refCommon.AppImgPath;
			m_strPageAction = Request.QueryString["action"];
            //object refAPI = m_refCommon as object;
        Utilities.SetLanguage(m_refCommon);
			TaxonomyLanguage = m_refCommon.ContentLanguage;
			TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
			if (Request.QueryString["view"] != null)
			{
				m_strViewItem = Request.QueryString["view"];
			}
			taxonomy_request = new TaxonomyRequest();
			taxonomy_request.TaxonomyId = TaxonomyId;
			taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
			m_refContent = m_refCommon.EkContentRef;
			if (Page.IsPostBack)
			{
				if (Request.Form["submittedaction"] == "delete")
				{
					m_refContent.DeleteTaxonomy(taxonomy_request);
					Response.Write("<script type=\"text/javascript\">parent.CloseChildPage();</script>");
				}
				else if (Request.Form["submittedaction"] == "deleteitem")
				{
					if (m_strViewItem != "folder")
					{
						taxonomy_request.TaxonomyIdList = Request.Form["selected_items"];
						if (m_strViewItem.ToLower() == "cgroup")
						{
							taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group;
						}
						else if (m_strViewItem.ToLower() == "user")
						{
							taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User;
						}
						else
						{
							taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Content;
						}
						m_refContent.RemoveTaxonomyItem(taxonomy_request);
					}
					else
					{
						TaxonomySyncRequest tax_folder = new TaxonomySyncRequest();
						tax_folder.TaxonomyId = TaxonomyId;
						tax_folder.TaxonomyLanguage = TaxonomyLanguage;
						tax_folder.SyncIdList = Request.Form["selected_items"];
						m_refContent.RemoveTaxonomyFolder(tax_folder);
					}
					if (Request.Params["ccp"] == null)
					{
						Response.Redirect("taxonomy.aspx?" + Request.ServerVariables["query_string"] + "&ccp=true", true);
					}
					else
					{
                    Response.Redirect((string)("taxonomy.aspx?" + Request.ServerVariables["query_string"]), true);
					}
				}
			}
			else if (IsPostBack == false)
			{
				DisplayPage();
			}
		}
		
		private void DisplayPage()
		{
			taxonomy_request.IncludeItems = true;
			taxonomy_request.PageSize = m_refCommon.RequestInformationRef.PagingSize;
			taxonomy_request.CurrentPage = m_intCurrentPage;
			taxonomy_data = m_refContent.ReadTaxonomy(ref taxonomy_request);
			if (taxonomy_data != null)
			{
				TaxonomyParentId = taxonomy_data.TaxonomyParentId;
				lbltaxonomyid.Text = taxonomy_data.TaxonomyId.ToString();
				lbltaxonomyid.ToolTip = lbltaxonomyid.Text;
				taxonomytitle.Text = taxonomy_data.TaxonomyName;
				taxonomytitle.ToolTip = taxonomytitle.Text;
				m_strTaxonomyName = taxonomy_data.TaxonomyName;
				if (taxonomy_data.TaxonomyDescription == "")
				{
					taxonomydescription.Text = "[None]";
					taxonomydescription.ToolTip = taxonomydescription.Text;
				}
				else
				{
					taxonomydescription.Text = taxonomy_data.TaxonomyDescription;
					taxonomydescription.ToolTip = taxonomydescription.Text;
				}
				if (taxonomy_data.TaxonomyImage == "")
				{
					taxonomy_image.Text = "[None]";
				}
				else
				{
					taxonomy_image.Text = taxonomy_data.TaxonomyImage;
				}
				taxonomy_image_thumb.ImageUrl = taxonomy_data.TaxonomyImage;
				if (taxonomy_data.CategoryUrl == "")
				{
					catLink.Text = "[None]";
				}
				else
				{
					catLink.Text = taxonomy_data.CategoryUrl;
				}
				
				if (taxonomy_data.Visible == true)
				{
					ltrStatus.Text = "Enabled";
				}
				else
				{
					ltrStatus.Text = "Disabled";
				}
				if (taxonomy_data.TaxonomyImage.Trim() != "")
				{
					taxonomy_image_thumb.ImageUrl = (taxonomy_data.TaxonomyImage.IndexOf("/") == 0) ? taxonomy_data.TaxonomyImage : m_refCommon.SitePath + taxonomy_data.TaxonomyImage;
				}
				else
				{
					taxonomy_image_thumb.ImageUrl = m_refCommon.AppImgPath + "spacer.gif";
				}
            m_strCurrentBreadcrumb = (string)(taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\\", " > "));
				if (m_strCurrentBreadcrumb == "")
				{
					m_strCurrentBreadcrumb = "Root";
				}
				if (taxonomy_data.TemplateName == "")
				{
					lblTemplate.Text = "[None]";
					lblTemplate.ToolTip = lblTemplate.Text;
				}
				else
				{
					lblTemplate.Text = taxonomy_data.TemplateName;
					lblTemplate.ToolTip = lblTemplate.Text;
				}
				if (taxonomy_data.TemplateInherited)
				{
					lblTemplateInherit.Text = "Yes";
					lblTemplateInherit.ToolTip = lblTemplateInherit.Text;
				}
				else
				{
					lblTemplateInherit.Text = "No";
					lblTemplateInherit.ToolTip = lblTemplateInherit.Text;
				}
				m_intTotalPages = taxonomy_request.TotalPages;
			}
			TaxonomyToolBar();
			if (TaxonomyParentId == 0)
			{
				tr_config.Visible = true;
				List<int> config_list = m_refContent.GetAllConfigIdListByTaxonomy(TaxonomyId, TaxonomyLanguage);
				configlist.Text = "";
				configlist.ToolTip = configlist.Text;
				for (int i = 0; i <= config_list.Count - 1; i++)
				{
					if (configlist.Text == "")
					{
						configlist.Text = ConfigName(System.Convert.ToInt32(config_list[i]));
						configlist.ToolTip = configlist.Text;
					}
					else
					{
						configlist.Text = configlist.Text + ";" + ConfigName(System.Convert.ToInt32(config_list[i]));
						configlist.ToolTip = configlist.Text;
					}
				}
				if (configlist.Text == "")
				{
					configlist.Text = "None";
					configlist.ToolTip = configlist.Text;
				}
			}
			else
			{
				tr_config.Visible = false;
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
		private string GetRecursiveTitle(bool value)
		{
			string result = "";
			if (value)
			{
				result = "<span class=\"important\"> (Recursive)</span>";
			}
			return result;
		}
		private void TaxonomyToolBar()
		{
			string strDeleteMsg = "";
			if (TaxonomyParentId > 0)
			{
				strDeleteMsg = m_refMsg.GetMessage("alt delete button text (category)");
				m_strDelConfirm = m_refMsg.GetMessage("delete category confirm");
				m_strDelItemsConfirm = m_refMsg.GetMessage("delete category items confirm");
				m_strSelDelWarning = m_refMsg.GetMessage("select category item missing warning");
			}
			else
			{
				strDeleteMsg = m_refMsg.GetMessage("alt delete button text (taxonomy)");
				m_strDelConfirm = m_refMsg.GetMessage("delete taxonomy confirm");
				m_strDelItemsConfirm = m_refMsg.GetMessage("delete taxonomy items confirm");
				m_strSelDelWarning = m_refMsg.GetMessage("select taxonomy item missing warning");
			}
        divTitleBar.InnerHtml = m_refstyle.GetTitleBar((string)(m_refMsg.GetMessage("view taxonomy page title") + " \"" + EkFunctions.HtmlEncode(m_strTaxonomyName) + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "\' />"));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("taxonomy.aspx?action=viewcontent&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/contentEdit.png", (string)("taxonomy.aspx?action=edit&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage), m_refMsg.GetMessage("alt edit button text (taxonomy)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
			// result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath & "btn_delete_folder-nm.gif", "#", strDeleteMsg, m_refMsg.GetMessage("btn delete"), "Onclick=""javascript:return DeleteItem();"""))
			if (AddDeleteIcon)
			{
				result.Append(m_refstyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/remove.png", "#", m_refMsg.GetMessage("alt remove button text (taxonomyitems)"), m_refMsg.GetMessage("btn remove"), "Onclick=\"javascript:return DeleteItem(\'items\');\"", StyleHelper.RemoveButtonCssClass));
			}
			
			result.Append("<td nowrap=\"true\">");
			
			string addDD;
			addDD = GetLanguageForTaxonomy(TaxonomyId, "", false, false, "javascript:TranslateTaxonomy(" + TaxonomyId + ", " + TaxonomyParentId + ", this.value);");
			if (addDD != "")
			{
            addDD = (string)("&nbsp;" + m_refMsg.GetMessage("add title") + ":&nbsp;" + addDD);
			}
			if (m_refCommon.EnableMultilingual == 1)
			{
				result.Append("View In:&nbsp;" + GetLanguageForTaxonomy(TaxonomyId, "", true, false, "javascript:LoadLanguage(this.value);") + "&nbsp;" + addDD + "<br>");
			}
			result.Append("</td>");
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refstyle.GetHelpButton("ViewTaxonomyOrCategory", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		
		private string FindSelected(string chk)
		{
			string val = "";
			if (m_strViewItem == chk)
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
			
			if (ShowTranslated)
			{
				taxonomy_language_request.IsTranslated = true;
				result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request);
				frmName = "frm_translated";
			}
			else
			{
				taxonomy_language_request.IsTranslated = false;
				result_language = m_refContent.LoadLanguageForTaxonomy(taxonomy_language_request);
				frmName = "frm_nontranslated";
			}
			
			result = "<select id=\"" + frmName + "\" name=\"" + frmName + "\" OnChange=\"" + onChangeEv + "\">" + "\r\n";
			
			if (ShowAllOpt)
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
				if (ShowTranslated == false)
				{
					result = result + "<option value=\"0\">-select language-</option>";
				}
			}
			if ((result_language != null) && (result_language.Count > 0) && (m_refCommon.EnableMultilingual == 1))
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
			if (result.Length > 0)
			{
				result = result + "</select>";
			}
			return (result);
		}
}

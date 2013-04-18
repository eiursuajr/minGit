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
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;

	public partial class editcontentattributes : System.Web.UI.UserControl
	{
		
		
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected long m_intId = 0;
		protected FolderData folder_data;
		protected PermissionData security_data;
		protected string AppImgPath = "";
		protected int ContentType = 1;
		protected long CurrentUserId = 0;
		protected Collection pagedata;
		protected string m_strPageAction = "";
		protected string m_strOrderBy = "";
		protected int ContentLanguage = -1;
		protected int EnableMultilingual = 0;
		protected string SitePath = "";
		protected ContentData content_data;
		protected string m_strCallerPage = "";
		protected string m_strFolderId = "";
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			m_refMsg = m_refContentApi.EkMsgRef;
			RegisterResources();
		}
		public bool EditContentProperties()
		{
			if (!(Request.QueryString["id"] == null))
			{
				m_intId = Convert.ToInt64(Request.QueryString["id"]);
			}
			if (!(Request.QueryString["action"] == null))
			{
				m_strPageAction = Convert.ToString(Request.QueryString["action"]).ToLower().Trim();
			}
			if (!(Request.QueryString["orderby"] == null))
			{
				m_strOrderBy = Convert.ToString(Request.QueryString["orderby"]);
			}
			if (!(Request.QueryString["LangType"] == null))
			{
				if (Request.QueryString["LangType"] != "")
				{
					ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
					m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
				}
				else
				{
					if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
			}
			else
			{
				if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
			{
				m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			else
			{
				m_refContentApi.ContentLanguage = ContentLanguage;
			}
			CurrentUserId = m_refContentApi.UserId;
			AppImgPath = m_refContentApi.AppImgPath;
			SitePath = m_refContentApi.SitePath;
			EnableMultilingual = m_refContentApi.EnableMultilingual;
			ContentStateData content;
			content = m_refContentApi.GetContentState(m_intId);
			ContentType = content.Type;
			// the following group is for forms:
			if (!(Request.QueryString["callerpage"] == null))
			{
				m_strCallerPage = Request.QueryString["callerpage"];
			}
			if (!(Request.QueryString["folder_id"] == null))
			{
				m_strFolderId = Request.QueryString["folder_id"];
			}
			
			
			if (!(Page.IsPostBack))
			{
				Display_EditContentProperties();
			}
			else
			{
				Process_UpdateContentProperties();
			}
            return true;
		}
		#region ACTION - UpdateContentProperties
		private void Process_UpdateContentProperties()
		{
			bool bInheritanceIsDif = false;
			bInheritanceIsDif = false;
			string init_xmlconfig = "";
			string init_frm_xmlinheritance = "";
			Ektron.Cms.Content.EkContent m_refContent;
			Ektron.Cms.Content.EkXmlIndexing XmlInd;
			ContentData content_data = null;
			try
			{
				m_refContent = m_refContentApi.EkContentRef;
				
				EkEnumeration.CMSContentSubtype subtype = m_refContent.GetContentSubType(m_intId);
				content_data = m_refContent.GetContentById(m_intId, 0);
				
				init_xmlconfig = Request.Form["init_xmlconfig"];
				init_frm_xmlinheritance = Request.Form["init_frm_xmlinheritance"];
				bInheritanceIsDif = true;
				pagedata = new Collection();
				pagedata.Add(Request.Form[content_id.UniqueID], "ContentID", null, null);
				if (subtype == EkEnumeration.CMSContentSubtype.WebEvent)
				{
					pagedata.Add(content_data.XmlInheritedFrom, "XmlInherited", null, null);
					pagedata.Add(content_data.XmlConfiguration.Id, "CollectionID", null, null);
				}
				else
				{
					pagedata.Add(false, "XmlInherited", null, null);
					pagedata.Add(Request.Form["xmlconfig"], "CollectionID", null, null);
				}
				
				if (Request.Form["IsSearchable"] == "on")
				{
					pagedata.Add(1, "IsSearchable", null, null);
				}
				else
				{
					pagedata.Add(0, "IsSearchable", null, null);
				}
				
				m_refContent.UpdateContentProperties(pagedata);
				//reverting 27535 - do not udpate xml_index table with new xml index search
				if (ContentType != 2 && subtype != EkEnumeration.CMSContentSubtype.WebEvent)
				{
					//form content should not be indexed.
					if (init_xmlconfig != Request.Form["xmlconfig"] || bInheritanceIsDif)
					{
						XmlInd = m_refContentApi.EkXmlIndexingRef;
						XmlInd.RemoveIndexDoc(System.Convert.ToInt64(Request.Form[content_id.UniqueID]));
						XmlInd.IndexDoc(System.Convert.ToInt64(Request.Form[content_id.UniqueID]), false);
					}
				}
				
				if (subtype != EkEnumeration.CMSContentSubtype.WebEvent)
				{
					if (m_refContentApi.EkContentRef.MultiConfigExists(content_data.Id, content_data.LanguageId))
					{
						m_refContentApi.EkContentRef.UpdateMultiConfigToXml(Convert.ToInt64(content_data.Id), content_data.LanguageId, Convert.ToInt64(Request.Form["xmlconfig"]));
					}
					else
					{
						m_refContentApi.EkContentRef.CreateMulticonfigEntry(content_data.Id, content_data.FolderId, content_data.LanguageId, m_refContentApi.GetFolderById(content_data.FolderId).TemplateId, Convert.ToInt64(Request.Form["xmlconfig"]));
					}
				}
				if (m_strCallerPage == "cmsform.aspx")
				{
					Response.Redirect((string) ("cmsform.aspx?LangType=" + ContentLanguage + "&action=ViewForm&form_id=" + m_intId + "&folder_id=" + m_strFolderId), false);
				}
				else
				{
					Response.Redirect((string) ("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + Request.Form[content_id.UniqueID]), false);
				}
				
			}
			catch (Exception ex)
			{
				Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message) + "&LangType=" + ContentLanguage), false);
			}
		}
		
		#endregion
		#region CONTENT - EditContentProperties
		private void Display_EditContentProperties()
		{
			
			XmlConfigData[] xmlconfig_data;
			string xmlseltagvalue = "0";
			bool OptionSelected = false;
			MultiConfigData xmlconfig_content;
			int i = 0;
			string configResource = "xml configuration label";
			
			content_data = m_refContentApi.GetContentById(m_intId, 0);
			if (content_data.Type == EkConstants.CMSContentType_CatalogEntry)
			{
				ProductTypeApi m_refProductTypeAPI = new ProductTypeApi();
				Criteria<ProductTypeProperty> criteria = new Criteria<ProductTypeProperty>();
				
				xmlconfig_data = m_refProductTypeAPI.GetList(criteria).ToArray();
				configResource = "lbl product type xml config";
			}
			else
			{
				xmlconfig_data = m_refContentApi.GetAllXmlConfigurations("title");
			}
			folder_data = m_refContentApi.GetFolderById(content_data.FolderId);
			security_data = m_refContentApi.LoadPermissions(m_intId, "content", 0);
			content_id.Value = m_intId.ToString();
			xmlconfig_content = m_refContentApi.EkContentRef.GetXmlConfig(content_data.Id, content_data.LanguageId);
			if (content_data.Type == EkConstants.CMSContentType_CatalogEntry)
			{
				EditEntryPropertiesToolBar();
			}
			else
			{
				EditContentPropertiesToolBar();
			}
			
			if (xmlconfig_content.XmlID == 0)
			{
				if (content_data.XmlConfiguration == null)
				{
					content_data.XmlConfiguration = new XmlConfigData();
				}
			}
			if (content_data.XmlConfiguration.Id != 0)
			{
				
				td_ecp_xmlconfiglbl.InnerHtml = m_refMsg.GetMessage(configResource);
				
				if (content_data.Type == EkConstants.CMSContentType_CatalogEntry)
				{
					td_ecp_xmlconfig.InnerHtml = "<input type=\"hidden\" name=\"xmlconfig\" id=\"xmlconfig\" value=\"" + content_data.XmlConfiguration.Id + "\"/>";
					td_ecp_xmlconfig.InnerHtml += "<select name=\"xmlconfig_disabled\" ";
				}
				else
				{
					td_ecp_xmlconfig.InnerHtml = "<select name=\"xmlconfig\" ";
				}
				
				if (xmlconfig_content.XmlID == 0 || content_data.Type == EkConstants.CMSContentType_CatalogEntry)
				{
					td_ecp_xmlconfig.InnerHtml += " disabled ";
				}
				td_ecp_xmlconfig.InnerHtml += ">";
				if (!(xmlconfig_data == null))
				{
					for (i = 0; i <= xmlconfig_data.Length - 1; i++)
					{
						if (content_data.IsXmlInherited == false || xmlconfig_content.XmlID != 0)
						{
							if (!(content_data.XmlConfiguration == null))
							{
								if (content_data.XmlConfiguration.Id == xmlconfig_data[i].Id || xmlconfig_content.XmlID == xmlconfig_data[i].Id)
								{
									OptionSelected = true;
									xmlseltagvalue = xmlconfig_data[i].Id.ToString();
								}
								else
								{
									OptionSelected = false;
								}
							}
						}
						
						td_ecp_xmlconfig.InnerHtml += "<option value=\"" + xmlconfig_data[i].Id + "\"";
						if (OptionSelected)
						{
							td_ecp_xmlconfig.InnerHtml += " selected ";
						}
						td_ecp_xmlconfig.InnerHtml += ">" + xmlconfig_data[i].Title;
					}
				}
				
				td_ecp_xmlconfig.InnerHtml += "</select>";
				td_ecp_xmlconfig.InnerHtml += "<input type=\"hidden\" name=\"init_xmlconfig\" value=\"" + xmlseltagvalue + "\">";
				if (!(content_data.Type == EkConstants.CMSContentType_CatalogEntry))
				{
					td_ecp_xmlconfig_lnk.InnerHtml = "<a href=\"#\" Onclick=\"javascript:PreviewXmlConfig();\"><img src=\"" + m_refContentApi.AppPath + "images/UI/Icons/preview.png" + "\" border=\"0\" alt=\"" + m_refMsg.GetMessage("alt preview button text (xml config)") + "\" title=\"" + m_refMsg.GetMessage("alt preview button text (xml config)") + "\"></a>";
				}
			}
            if (content_data.SubType != EkEnumeration.CMSContentSubtype.WebEvent)
			{
				xmlConfigPanel.Visible = true;
			}
			else
			{
				xmlConfigPanel.Visible = false;
			}
            
            searchable.InnerHtml = m_refMsg.GetMessage("lbl content searchable");
			searchable.InnerHtml += " <input type=\"checkbox\" name=\"IsSearchable\"";
            if (content_data.SubType == EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
                searchable.InnerHtml += " disabled=\"disabled\"";
            else
            {
                if (content_data.IsSearchable == true)
                {
                    searchable.InnerHtml += " checked ";
                }
            }
			searchable.InnerHtml += ">";
			
			
			// Display content flagging options:
			
			flagging.InnerHtml = m_refMsg.GetMessage("wa tree flag def");
			
			long contentFlagId = content_data.FlagDefId;
			if (contentFlagId > 0)
			{
				FlagDefData fd = m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(contentFlagId, false);
				if (fd != null)
				{
					if (string.IsNullOrEmpty(fd.Name))
					{
						lblflag.Text = "None";
					}
					else
					{
						lblflag.Text = fd.Name; // & " (Id:" & fd.ID & ")"
					}
				}
				else
				{
					lblflag.Text = "None";
				}
			}
			else
			{
				lblflag.Text = "None";
			}
			
		}
		private void EditContentPropertiesToolBar()
		{
			System.Text.StringBuilder result;
			result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("view properties for content") + " \"" + content_data.Title + "\""));
			result.Append("<table><tr>");

			if (m_strCallerPage == "cmsform.aspx")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("cmsform.aspx?LangType=" + ContentLanguage + "&action=ViewForm&form_id=" + m_intId + "&folder_id=" + m_strFolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (content props)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'editfolder\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton(m_strPageAction, ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void EditEntryPropertiesToolBar()
		{
			System.Text.StringBuilder result;
			result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("view properties for entry") + " \"" + content_data.Title + "\""));
			result.Append("<table><tr>");

			if (m_strCallerPage == "cmsform.aspx")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("cmsform.aspx?LangType=" + ContentLanguage + "&action=ViewForm&form_id=" + m_intId + "&folder_id=" + m_strFolderId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("content.aspx?LangType=" + ContentLanguage + "&action=View&id=" + m_intId), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (entry props)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'editfolder\', \'\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("editcontentproperties_ecom", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		
		#endregion
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}
	
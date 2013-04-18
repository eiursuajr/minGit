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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
	public partial class taxonomy : System.Web.UI.Page
	{
		protected EkMessageHelper m_refMsg;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected string m_strPageAction = "";
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected CommonApi m_refCommon = new CommonApi();
		protected int EnableMultilingual = 0;
		protected EkContent m_refContent;
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
				if (! Utilities.ValidateUserLogin())
				{
					return;
				}
				if ((m_refContentApi.RequestInformationRef.IsMembershipUser == 1) || ! m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TaxonomyAdministrator) && ! m_refContentApi.IsAdmin())
				{
					Response.Redirect((string) ("reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login taxonomy administrator")), false);
					return;
				}
				else
				{
					AppImgPath = m_refCommon.AppImgPath;
					AppPath = m_refCommon.AppPath;
					litAppPath.Text = AppPath;
					EnableMultilingual = m_refCommon.EnableMultilingual;
					displaystylesheet.Text = m_refStyle.GetClientScript();
					if ((Request.QueryString["action"] != null)&& Request.QueryString["action"] != "")
					{
						m_strPageAction = Request.QueryString["action"].ToLower();
					}
                    Utilities.SetLanguage(m_refCommon);
					TaxonomyLanguage = m_refCommon.ContentLanguage;
					if (Page.IsPostBack)
					{
						if ((Request.Form[submittedaction.UniqueID]) == "deleteSelected")
						{
							TaxonomyRequest taxonomy_request_data = new TaxonomyRequest();
							taxonomy_request_data.TaxonomyIdList = Request.Form["selected_taxonomy"];
							taxonomy_request_data.TaxonomyLanguage = TaxonomyLanguage;
							m_refContent.DeleteTaxonomy(taxonomy_request_data);
							Response.Redirect("taxonomy.aspx?rf=1", false);
						}
					}
					switch (m_strPageAction)
					{
						case "add":
							addtaxonomy m_at;
							m_at = (addtaxonomy) (LoadControl("controls/taxonomy/addtaxonomy.ascx"));
							m_at.ID = "taxonomy";
							DataHolder.Controls.Add(m_at);
							break;
						case "viewtree":
							body.Attributes.Add("onload", "javascript:pageLoaded()");
							taxonomytree m_tt;
							m_tt = (taxonomytree) (LoadControl("controls/taxonomy/taxonomytree.ascx"));
							m_tt.ID = "taxonomy";
							DataHolder.Controls.Add(m_tt);
							break;
						case "edit":
						case "reorder":
							edittaxonomy m_et;
							m_et = (edittaxonomy) (LoadControl("controls/taxonomy/edittaxonomy.ascx"));
							m_et.ID = "taxonomy";
							DataHolder.Controls.Add(m_et);
							break;
						case "view":
							viewtaxonomy m_vt;
							m_vt = (viewtaxonomy) (LoadControl("controls/taxonomy/viewtaxonomy.ascx"));
							m_vt.ID = "taxonomy";
							DataHolder.Controls.Add(m_vt);
							break;
						case "viewcontent":
						case "removeitems":
							Control m_vi;
							m_vi = (Control) (LoadControl("controls/taxonomy/viewitems.ascx"));
							m_vi.ID = "taxonomy";
							DataHolder.Controls.Add(m_vi);
							break;
						case "viewattributes":
							Control m_va;
							m_va = (Control) (LoadControl("controls/taxonomy/viewattributes.ascx"));
							m_va.ID = "taxonomy";
							DataHolder.Controls.Add(m_va);
							break;
						case "additem":
						case "addfolder":
							if (m_strPageAction == "addfolder")
							{
								//body.Attributes.Add("onload", "Main.start();displayTreeFolderSelect();showSelectedFolderTree();setupClassNames();")
                                //body.Attributes.Add("onload", "Main.start();displayTreeFolderSelect();showSelectedFolderTree();setupClassNames();");
                                // Skipping the call to showSelectedFolderTree method, as it kept expanding the selected folder tree.
                                body.Attributes.Add("onload", "Main.start();displayTreeFolderSelect();setupClassNames();");
							}
							
							assigntaxonomy m_asnt;
							m_asnt = (assigntaxonomy) (LoadControl("controls/taxonomy/assigntaxonomy.ascx"));
							m_asnt.ID = "taxonomy";
							DataHolder.Controls.Add(m_asnt);
							break;
							
						default:
							div_taxonomylist.Visible = true;
                           
							if (IsPostBack == false || (IsPostBack == true && Request.Form[isSearchPostData.UniqueID] != "" && Request.Form[isPostData.UniqueID] != "") || (!string.IsNullOrEmpty(Request.QueryString["reloadtrees"]) && Request.QueryString["reloadtrees"].ToLower() == "tax"))
							{
								ViewAllTaxonomy();
                                if (!string.IsNullOrEmpty(Request.QueryString["reloadtrees"]) && Request.QueryString["reloadtrees"].ToLower() == "tax")
                                {
                                    ReloadClientScript("");
                                }
							}
                           
							break;
					}
					
					if (Request.QueryString["rf"] == "1")
					{
						litRefreshAccordion.Text = "<script language=\"javascript\">" + ("\r\n" + "top.refreshTaxonomyAccordion(") + TaxonomyLanguage + ");" + ("\r\n" + "</script>") + "\r\n";                        
					}
					
				}
				SetJsServerVariables();
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
        private void ReloadClientScript(string idPath)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            try
            {
                idPath = idPath.Replace("\\", "\\\\");
                result.Append("top.TreeNavigation(\"TaxTree\", \"" + idPath + "\");" + "\r\n");
                Ektron.Cms.API.JS.RegisterJSBlock(this.Page.Header, result.ToString(), "ReloadClientScript");
            }
            catch (Exception)
            {
            }
        }
		private void ViewAllTaxonomy()
		{
			m_strSearchText = Request.Form["txtSearch"];
			if (m_strSearchText == null)
			{
				m_strSearchText = "";
			}
			DisplayAllTaxonomy();
			ViewAllToolBar();
		}
		
		private void DisplayAllTaxonomy()
		{
			TaxonomyList.Columns.Add(m_refStyle.CreateBoundField("CHECK", "<input type=\"Checkbox\" name=\"checkall\" onclick=\"checkAll(\'selected_taxonomy\',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), false, false));
			TaxonomyList.Columns.Add(m_refStyle.CreateBoundField("TITLE", m_refMsg.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
			TaxonomyList.Columns.Add(m_refStyle.CreateBoundField("ID", m_refMsg.GetMessage("generic ID"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
			TaxonomyList.Columns.Add(m_refStyle.CreateBoundField("LANGUAGE", m_refMsg.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.Center, Unit.Percentage(5), Unit.Percentage(5), false, false));
			TaxonomyList.Columns.Add(m_refStyle.CreateBoundField("DESCRIPTION", m_refMsg.GetMessage("lbl discussionforumtitle"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
			TaxonomyList.Columns.Add(m_refStyle.CreateBoundField("PATH", m_refMsg.GetMessage("generic Path"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
			
			DataTable dt = new DataTable();
			DataRow dr;
			dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(string)));
			dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
			dt.Columns.Add(new DataColumn("DESCRIPTION", typeof(string)));
			dt.Columns.Add(new DataColumn("PATH", typeof(string)));
			TaxonomyRequest request = new TaxonomyRequest();
			request.TaxonomyId = 0;
			request.TaxonomyLanguage = TaxonomyLanguage;
			request.SearchText = m_strSearchText;
			request.PageSize = m_refCommon.RequestInformationRef.PagingSize;
			request.CurrentPage = m_intCurrentPage;
            TaxonomyBaseData[] result = m_refContent.ReadAllSubCategories(request);
			m_intTotalPages = request.TotalPages;
			PageSettings();
			if ((result != null)&& result.Length > 0)
			{
				AddDeleteIcon = true;
				for (int i = 0; i <= result.Length - 1; i++)
				{
					dr = dt.NewRow();
					dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_taxonomy\" id=\"selected_taxonomy\" value=\"" + result[i].TaxonomyId + "\" onclick=\"checkAll(\'selected_taxonomy\',true);\">";
					
					string taxonomylink;
					taxonomylink = "taxonomy.aspx?action=view&view=item&taxonomyid=" + result[i].TaxonomyId + "&LangType=" + result[i].TaxonomyLanguage + "&treeViewId=-1";
                    if (Page.Request.QueryString["LangType"] != null)
                    {
                        if (Page.Request.QueryString["LangType"].ToString() == "-1")
                        {
                            // if we're showing all taxonomies, we have to refresh the tree when a user chooses one of these languages
                            taxonomylink = taxonomylink + "&rf=1";
                        }
                    }
					dr["TITLE"] = "<a href=\"" + taxonomylink + "\">" + result[i].TaxonomyName + "</a>";
					dr["ID"] = result[i].TaxonomyId;
					dr["LANGUAGE"] = "<a href=\"" + m_refCommon.AppPath + "javascriptrequired.aspx\" onmouseover=\"ddrivetip(\'" + result[i].TaxonomyLanguageName + "\',\'ADC5EF\', 100);\" onmouseout=\"hideddrivetip()\" style=\"text-decoration:none;\" onclick=\"return false;\">" + "<img src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(result[i].TaxonomyLanguage) + "\' border=\"0\" />" + "</a>";
					dr["DESCRIPTION"] = result[i].TaxonomyDescription;
					dr["PATH"] = result[i].TaxonomyPath;
					dt.Rows.Add(dr);
				}
			}
			else
			{
				dr = dt.NewRow();
				dt.Rows.Add(dr);
				TaxonomyList.GridLines = GridLines.None;
			}
			DataView dv = new DataView(dt);
			TaxonomyList.DataSource = dv;
			TaxonomyList.DataBind();
		}
		
		private void ViewAllToolBar()
		{
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view all taxonomy page title"));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");

			bool primaryCssApplied = false;

			if (this.TaxonomyLanguage > 0)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/add.png", "taxonomy.aspx?action=add", m_refMsg.GetMessage("alt add button text (taxonomy)"), m_refMsg.GetMessage("btn add Taxonomy"), "", StyleHelper.AddButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
			}

			if (AddDeleteIcon && this.TaxonomyLanguage > 0)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/delete.png", "#", m_refMsg.GetMessage("alt delete button text (selectedtaxonomy)"), m_refMsg.GetMessage("btn delete"), "onclick=\"return Delete();\"", StyleHelper.DeleteButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/ui/Icons/taxonomyImport.png", "taxonomy_imp_exp.aspx", m_refMsg.GetMessage("alt import taxonomy"), m_refMsg.GetMessage("btn import taxonomy"), "", StyleHelper.ImportTaxonomyButtonCssClass, !primaryCssApplied));

			primaryCssApplied = true;

			result.Append(m_refStyle.GetExportTranslationButton("content.aspx?type=taxonomy&id=0&LangType=" + this.TaxonomyLanguage + "&action=Localize&callbackpage=taxonomy.aspx&parm1=action&value1=ViewTaxonomyReport", m_refMsg.GetMessage("alt click here to export all taxo for translation"), m_refMsg.GetMessage("lbl Export For Translation")));
			//result.Append("<td>&#160;&#160;</td>")
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td class=\"label\">");
			result.Append(m_refMsg.GetMessage("lbl View") + ":");
			result.Append("</td>");
			result.Append("<td>");
			result.Append(m_refStyle.ShowAllActiveLanguage(true, "", "javascript:defaultLoadLanguage(this.value);", TaxonomyLanguage.ToString()));
			result.Append("</td>");
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append("<label for=\"txtSearch\">" + m_refMsg.GetMessage("generic search") + "</label>");
			result.Append("<input type=text class=\"ektronTextMedium\" id=txtSearch name=txtSearch value=\"" + m_strSearchText + "\" onkeydown=\"CheckForReturn(event)\">");
			result.Append("<input type=button value=" + m_refMsg.GetMessage("btn search") + " id=\"btnSearch\" name=\"btnSearch\"  class=\"ektronWorkareaSearch\" onclick=\"searchtaxonomy();\">");
			result.Append("</td>");
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("ViewAllTaxonomy", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		
		private void PageSettings()
		{
			if (m_intTotalPages <= 1)
			{
				VisiblePageControls(false);
			}
			else
			{
				VisiblePageControls(true);
				TTotalPages.Text = m_intTotalPages.ToString();
				TTotalPages.ToolTip = TTotalPages.Text;
				TCurrentPage.Text = m_intCurrentPage.ToString();
				TCurrentPage.ToolTip = TCurrentPage.Text;
				TPreviousPage.Enabled = true;
				TFirstPage.Enabled = true;
				TNextPage.Enabled = true;
				TLastPage.Enabled = true;
				if (m_intCurrentPage == 1)
				{
					TPreviousPage.Enabled = false;
					TFirstPage.Enabled = false;
				}
				else if (m_intCurrentPage == m_intTotalPages)
				{
					TNextPage.Enabled = false;
					TLastPage.Enabled = false;
				}
			}
		}
		private void VisiblePageControls(bool flag)
		{
			TTotalPages.Visible = flag;
			TCurrentPage.Visible = flag;
			TPreviousPage.Visible = flag;
			TNextPage.Visible = flag;
			TLastPage.Visible = flag;
			TFirstPage.Visible = flag;
			TPageLabel.Visible = flag;
			TOfLabel.Visible = flag;
		}
		protected void TNavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					m_intCurrentPage = 1;
					break;
				case "Last":
					m_intCurrentPage = int.Parse((string) TTotalPages.Text);
					break;
				case "Next":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) TCurrentPage.Text) + 1);
					break;
				case "Prev":
					m_intCurrentPage = System.Convert.ToInt32(int.Parse((string) TCurrentPage.Text) - 1);
					break;
			}
			ViewAllTaxonomy();
			isPostData.Value = "true";
		}
		private void RegisterResources()
		{
			// register JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
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
            ltr_AddPropMsg.Text = m_refMsg.GetMessage("js:taxonomy alert selection required");
			ltr_selTaxMsg.Text = m_refMsg.GetMessage("select taxonomy msg");
			ltr_delCnfrmMsg.Text = m_refMsg.GetMessage("delete taxonomy msg");
			ltr_msgSearch.Text = m_refMsg.GetMessage("alert msg search");
			ltr_lang.Text = Convert.ToString(TaxonomyLanguage);
			ltr_contLangCnfrmMsg.Text = m_refMsg.GetMessage("alert msg add taxonomy lang");
		}
		
	}
	

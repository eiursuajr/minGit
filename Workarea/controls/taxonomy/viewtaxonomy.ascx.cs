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
//using System.DateTime;
using System.IO;
using Ektron.Cms.Framework.Core.CustomProperty;
using Ektron.Cms.Content;
using Microsoft.Security.Application;
	public partial class viewtaxonomy : System.Web.UI.UserControl
	{
		protected CommonApi _Common = new CommonApi();
		protected StyleHelper _StyleHelper = new StyleHelper();
		protected string AppImgPath = "";
		protected string AppPath = "";
		protected EkMessageHelper _MessageHelper;
		protected string _PageAction = "";
		protected EkContent _Content;
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
		protected int m_intCurrentPage = 1;
		protected int m_intTotalPages = 1;
		protected int m_intMetadataCurrentPage = 1;
		protected int m_intMetadataTotalPages = 1;
		protected string m_strDelConfirm = "";
		protected string m_strDelItemsConfirm = "";
		protected string m_strSelDelWarning = "";
		protected string m_strCurrentBreadcrumb = "";
		protected LocalizationAPI objLocalizationApi = new LocalizationAPI();
		protected string parentTaxonomyPath = string.Empty;
		protected ContentAPI m_refContentApi = new ContentAPI();
        protected bool reloadTree = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_MessageHelper = _Common.EkMsgRef;
			AppImgPath = _Common.AppImgPath;
			AppPath = _Common.AppPath;
			_PageAction = Request.QueryString["action"];

            if (!string.IsNullOrEmpty(Request.QueryString["reloadtrees"]))
            {
                reloadTree = true;
            }

            object refCommon = _Common as object;
            Utilities.SetLanguage(_Common);
            //Utilities.SetLanguage(_Common);
			RegisterResources();
			TaxonomyLanguage = _Common.ContentLanguage;
			TaxonomyId = Convert.ToInt64(Request.QueryString["taxonomyid"]);
			if (Request.QueryString["view"] != null)
			{
				_ViewItem = AntiXss.HtmlEncode(Request.QueryString["view"]);
			}
			taxonomy_request = new TaxonomyRequest();
			taxonomy_request.TaxonomyId = TaxonomyId;
			taxonomy_request.TaxonomyLanguage = TaxonomyLanguage;
			_Content = _Common.EkContentRef;
			taxonomy_request.PageSize = 99999999; // pagesize of 0 used to mean "all"
			TaxonomyBaseData[] taxcats;
			taxcats = _Content.ReadAllSubCategories(taxonomy_request);
			if (taxcats != null)
			{
				TaxonomyCategoryCount = taxcats.Length;
			}
			if (Page.IsPostBack && Request.Form[isPostData.UniqueID] != "")
			{
				if (Request.Form["submittedaction"] == "delete")
				{
					_Content.DeleteTaxonomy(taxonomy_request);
					//Response.Write("<script type=""text/javascript"">parent.CloseChildPage();</script>")
                    Response.Redirect("taxonomy.aspx?action=reload&rf=1&reloadtrees=Tax", true);
				}
				else if (Request.Form["submittedaction"] == "deleteitem")
				{
					if (_ViewItem != "folder")
					{
						taxonomy_request.TaxonomyIdList = Request.Form["selected_items"];
						if (_ViewItem.ToLower() == "cgroup")
						{
							taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Group;
						}
						else if (_ViewItem.ToLower() == "user")
						{
							taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.User;
						}
						else
						{
							taxonomy_request.TaxonomyItemType = EkEnumeration.TaxonomyItemType.Content;
						}
						_Content.RemoveTaxonomyItem(taxonomy_request);
					}
					else
					{
						TaxonomySyncRequest tax_folder = new TaxonomySyncRequest();
						tax_folder.TaxonomyId = TaxonomyId;
						tax_folder.TaxonomyLanguage = TaxonomyLanguage;
						tax_folder.SyncIdList = Request.Form["selected_items"];
						_Content.RemoveTaxonomyFolder(tax_folder);
					}
					if (Request.Params["ccp"] == null)
					{
						Response.Redirect("taxonomy.aspx?" + Request.ServerVariables["query_string"] + "&ccp=true", true);
					}
					else
					{
						Response.Redirect((string) ("taxonomy.aspx?" + Request.ServerVariables["query_string"]), true);
					}
				}
			}
			else if (IsPostBack == false)
			{
				DisplayPage();
			}
			AssignTextStrings();
			isPostData.Value = "true";
			hdnSourceId.Value = TaxonomyId.ToString();
		}
		
		private void DisplayPage()
		{
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
					if (uDirectory != null)
					{
						TaxonomyParentId = uDirectory.DirectoryParentId;
						lbltaxonomyid.Text = uDirectory.DirectoryId.ToString();
						lbltaxonomyid.ToolTip = lbltaxonomyid.Text;
						taxonomytitle.Text = uDirectory.DirectoryName;
						taxonomytitle.ToolTip = taxonomytitle.Text;
						_TaxonomyName = uDirectory.DirectoryName;
						taxonomydescription.Text = uDirectory.DirectoryDescription;
						taxonomydescription.ToolTip = taxonomydescription.Text;
						taxonomy_image_thumb.ImageUrl = _Common.AppImgPath + "spacer.gif";
						m_strCurrentBreadcrumb = (string) (uDirectory.DirectoryPath.Remove(0, 1).Replace("\\", " > "));
						if (m_strCurrentBreadcrumb == "")
						{
							m_strCurrentBreadcrumb = "Root";
						}
						else
						{
                            if (uDirectory.DirectoryParentId == 0)
                            {
                                parentTaxonomyPath = uDirectory.DirectoryPath.Replace("\\" + uDirectory.DirectoryName, "\\");
                            }
                            else
                            {
                                parentTaxonomyPath = uDirectory.DirectoryPath.Replace("\\" + uDirectory.DirectoryName, "");
                            }
                            hdn_parentTaxonomyPath.Value = parentTaxonomyPath;
						}
						if (uDirectory.TemplateName == "")
						{
							lblTemplate.Text = "[None]";
							lblTemplate.ToolTip = lblTemplate.Text;
						}
						else
						{
							lblTemplate.Text = uDirectory.TemplateName;
							lblTemplate.ToolTip = lblTemplate.Text;
						}
						if (uDirectory.InheritTemplate)
						{
							lblTemplateInherit.Text = "Yes";
							lblTemplateInherit.ToolTip = lblTemplateInherit.Text;
						}
						else
						{
							lblTemplateInherit.Text = "No";
							lblTemplateInherit.ToolTip = lblTemplateInherit.Text;
						}
						
						m_intTotalPages = uReq.TotalPages;
					}
					PopulateUserGridData(uDirectory);
					TaxonomyToolBar();
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
					if (dagdRet != null)
					{
						TaxonomyParentId = dagdRet.DirectoryParentId;
						lbltaxonomyid.Text = dagdRet.DirectoryId.ToString();
						lbltaxonomyid.ToolTip = lbltaxonomyid.Text;
						taxonomytitle.Text = dagdRet.DirectoryName;
						taxonomytitle.ToolTip = taxonomytitle.Text;
						_TaxonomyName = dagdRet.DirectoryName;
						taxonomydescription.Text = dagdRet.DirectoryDescription;
						taxonomydescription.ToolTip = taxonomydescription.Text;
						taxonomy_image_thumb.ImageUrl = _Common.AppImgPath + "spacer.gif";
						m_strCurrentBreadcrumb = (string) (dagdRet.DirectoryPath.Remove(0, 1).Replace("\\", " > "));
						
						if (m_strCurrentBreadcrumb == "")
						{
							m_strCurrentBreadcrumb = "Root";
						}
						else
						{
                            if (dagdRet.DirectoryParentId == 0)
                            {
                                parentTaxonomyPath = dagdRet.DirectoryPath.Replace("\\" + dagdRet.DirectoryName, "\\");
                            }
                            else
                            {
                                parentTaxonomyPath = dagdRet.DirectoryPath.Replace("\\" + dagdRet.DirectoryName, "");
                            }
							hdn_parentTaxonomyPath.Value = parentTaxonomyPath;
						}
						if (dagdRet.TemplateName == "")
						{
							lblTemplate.Text = "[None]";
							lblTemplate.ToolTip = lblTemplate.Text;
						}
						else
						{
							lblTemplate.Text = dagdRet.TemplateName;
							lblTemplate.ToolTip = lblTemplate.Text;
						}
						if (dagdRet.InheritTemplate)
						{
							lblTemplateInherit.Text = "Yes";
							lblTemplateInherit.ToolTip = lblTemplateInherit.Text;
						}
						else
						{
							lblTemplateInherit.Text = "No";
							lblTemplateInherit.ToolTip = lblTemplateInherit.Text;
						}
						
						m_intTotalPages = cReq.TotalPages;
					}
					PopulateCommunityGroupGridData(dagdRet);
					TaxonomyToolBar();
					break;              

				default: // Content
					taxonomy_request.IncludeItems = true;
					taxonomy_request.PageSize = _Common.RequestInformationRef.PagingSize;
					taxonomy_request.CurrentPage = m_intCurrentPage;
					taxonomy_data = _Content.ReadTaxonomy(ref taxonomy_request);
					if (taxonomy_data != null)
					{
						TaxonomyParentId = taxonomy_data.TaxonomyParentId;
						lbltaxonomyid.Text = taxonomy_data.TaxonomyId.ToString();
						lbltaxonomyid.ToolTip = lbltaxonomyid.Text;
						taxonomytitle.Text = taxonomy_data.TaxonomyName;
						taxonomytitle.ToolTip = taxonomytitle.Text;
						_TaxonomyName = taxonomy_data.TaxonomyName;
						if (taxonomy_data.TaxonomyDescription == "")
						{
							taxonomydescription.Text = "[None]";
							taxonomydescription.ToolTip = taxonomydescription.Text;
						}
						else
						{
							taxonomydescription.Text = Server.HtmlEncode(taxonomy_data.TaxonomyDescription);
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
							catLink.ToolTip = catLink.Text;
						}
						else
						{
							catLink.Text = taxonomy_data.CategoryUrl;
							catLink.ToolTip = catLink.Text;
						}
                        if (_Content.IsSynchronizedTaxonomy(TaxonomyId, TaxonomyLanguage))
                        {
                            ltrTaxSynch.Text = _MessageHelper.GetMessage("enabled");
                        }
                        else
                        {
                            ltrTaxSynch.Text = _MessageHelper.GetMessage("disabled");
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
							taxonomy_image_thumb.ImageUrl = (taxonomy_data.TaxonomyImage.IndexOf("/") == 0) ? taxonomy_data.TaxonomyImage : _Common.SitePath + taxonomy_data.TaxonomyImage;
						}
						else
						{
							taxonomy_image_thumb.ImageUrl = _Common.AppImgPath + "spacer.gif";
						}
						m_strCurrentBreadcrumb = (string) (taxonomy_data.TaxonomyPath.Remove(0, 1).Replace("\\", " > "));
						if (m_strCurrentBreadcrumb == "")
						{
							m_strCurrentBreadcrumb = "Root";
						}
						else
						{
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
                    if (reloadTree)
                    {
                        ReloadClientScript(taxonomy_data.IdPath);
                    }
					PopulateContentGridData();
					TaxonomyToolBar();
					break;
			}
			
			DisplayTaxonomyMetadata();
			
			if (TaxonomyParentId == 0)
			{
				tr_config.Visible = true;
				List<int> config_list = _Content.GetAllConfigIdListByTaxonomy(TaxonomyId, TaxonomyLanguage);
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
			
			// display counts
            ltrCatCount.Text = TaxonomyCategoryCount.ToString();
            ltrItemCount.Text = taxonomy_request.RecordsAffected.ToString();
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
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=\"checkbox\" name=\"checkall\" onclick=\"checkAll(\'selected_items\',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("COMMUNITYGROUP", _MessageHelper.GetMessage("lbl community group"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, true));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("INFORMATION", "&#160;", "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));
			
			TaxonomyItemList.Columns[2].ItemStyle.VerticalAlign = VerticalAlign.Top;
			TaxonomyItemList.Columns[3].ItemStyle.VerticalAlign = VerticalAlign.Top;
			
			DataTable dt = new DataTable();
			DataRow dr;
			dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(string)));
			dt.Columns.Add(new DataColumn("COMMUNITYGROUP", typeof(string)));
			dt.Columns.Add(new DataColumn("INFORMATION", typeof(string)));
			PageSettings();
			if ((cgDirectory != null)&& (cgDirectory.DirectoryItems != null)&& cgDirectory.DirectoryItems.Length > 0)
			{
				AddDeleteIcon = true;
				foreach (CommunityGroupData item in cgDirectory.DirectoryItems)
				{
					TaxonomyItemCount++;
					dr = dt.NewRow();
					dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.GroupId + "\" onclick=\"checkAll(\'selected_items\',true);\">";
					
					string groupurl;
					groupurl = (string) ("Community/groups.aspx?action=viewgroup&id=" + item.GroupId);
					dr["COMMUNITYGROUP"] = "<img src=\"" + (item.GroupImage != "" ? item.GroupImage : this._Common.AppImgPath + "member_default.gif") + "\" align=\"left\" width=\"55\" height=\"55\" />";
					dr["COMMUNITYGROUP"] += "<a href=\"" + groupurl + "\">";
					dr["COMMUNITYGROUP"] += item.GroupName;
					dr["COMMUNITYGROUP"] += "</a>";
					dr["COMMUNITYGROUP"] += " (" + (item.GroupEnroll ? (this._MessageHelper.GetMessage("lbl enrollment open")) : (this._MessageHelper.GetMessage("lbl enrollment restricted"))) + ")";
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
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=\"checkbox\" name=\"checkall\" onclick=\"checkAll(\'selected_items\',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(3), Unit.Percentage(2), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("USERNAME", _MessageHelper.GetMessage("generic username"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("DISPLAYNAME", _MessageHelper.GetMessage("display name label"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(41), false, false));
			DataTable dt = new DataTable();
			DataRow dr;
			dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(string)));
			dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
			dt.Columns.Add(new DataColumn("DISPLAYNAME", typeof(string)));
			PageSettings();
			if ((uDirectory != null)&& (uDirectory.DirectoryItems != null)&& uDirectory.DirectoryItems.Length > 0)
			{
				AddDeleteIcon = true;
				foreach (DirectoryUserData item in uDirectory.DirectoryItems)
				{
					TaxonomyItemCount++;
					dr = dt.NewRow();
					dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.Id + "\" onclick=\"checkAll(\'selected_items\',true);\">";
					// TODO: do we need to put in valid groupid and grouptype fields?
					string userurl = (string) ("users.aspx?action=View&LangType=" + TaxonomyLanguage + "&groupid=" + 0 + "&grouptype=" + 0 + "&id=" + item.Id + "&FromUsers=&OrderBy=user_name&callbackpage=taxonomy.aspx?" + Request.ServerVariables["query_string"]);
					dr["USERNAME"] = "<a href =\"" + userurl + "\">";
					dr["USERNAME"] += item.Username; //"<a href=""taxonomy.aspx?action=viewtree&taxonomyid=" & item.TaxonomyItemId & "&LangType=" & item.TaxonomyItemLanguage & """>" & item.TaxonomyItemTitle & "</a>"
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
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("CHECK", "<input type=\"checkbox\" name=\"checkall\" onclick=\"checkAll(\'selected_items\',false);\">", "title-header", HorizontalAlign.Center, HorizontalAlign.Center, Unit.Percentage(2), Unit.Percentage(2), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("TITLE", _MessageHelper.GetMessage("generic title"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(50), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ID", _MessageHelper.GetMessage("generic id"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("LANGUAGE", _MessageHelper.GetMessage("generic language"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("URL", _MessageHelper.GetMessage("generic url link"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(30), false, false));
			TaxonomyItemList.Columns.Add(_StyleHelper.CreateBoundField("ARCHIVED", _MessageHelper.GetMessage("lbl archived"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
			
			DataTable dt = new DataTable();
			DataRow dr;
			LibraryData libraryInfo;
			ContentData contData = new ContentData();
			dt.Columns.Add(new DataColumn("CHECK", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("ID", typeof(string)));
			dt.Columns.Add(new DataColumn("LANGUAGE", typeof(string)));
			dt.Columns.Add(new DataColumn("URL", typeof(string)));
			dt.Columns.Add(new DataColumn("ARCHIVED", typeof(string)));
			if (_ViewItem != "folder")
			{
				PageSettings();
				if ((taxonomy_data != null)&& (taxonomy_data.TaxonomyItems != null)&& taxonomy_data.TaxonomyItems.Length > 0)
				{
					AddDeleteIcon = true;
					foreach (TaxonomyItemData item in taxonomy_data.TaxonomyItems)
					{
						TaxonomyItemCount++;
						dr = dt.NewRow();
						dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + item.TaxonomyItemId + "\" onclick=\"checkAll(\'selected_items\',true);\">";
						string contenturl = "";
						switch (Convert.ToInt32(item.ContentType))
						{
							case 1:
								if (item.ContentSubType == EkEnumeration.CMSContentSubtype.WebEvent)
								{
									long fid = _Common.EkContentRef.GetFolderIDForContent(item.TaxonomyItemId);
									contenturl = (string) ("content.aspx?action=ViewContentByCategory&LangType=" + item.TaxonomyItemLanguage + "&id=" + fid + "&callerpage=taxonomy.aspx&origurl=" + EkFunctions.UrlEncode((string) ("action=view&view=item&taxonomyid=" + TaxonomyId + "&treeViewId=-1&LangType=" + TaxonomyLanguage)));
								}
								else
								{
									contenturl = (string) ("content.aspx?action=View&LangType=" + item.TaxonomyItemLanguage + "&id=" + item.TaxonomyItemId + "&callerpage=taxonomy.aspx&origurl=" + EkFunctions.UrlEncode((string) ("action=view&view=item&taxonomyid=" + TaxonomyId + "&treeViewId=-1&LangType=" + TaxonomyLanguage)));
								}
								break;
							case 7: // Library Item
								libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId);
                                if(libraryInfo != null)
								  contenturl = (string) ("library.aspx?LangType=" + libraryInfo.LanguageId + "&action=ViewLibraryItem&id=" + libraryInfo.Id + "&parent_id=" + libraryInfo.ParentId);
								break;
                            case 1111:
                                // forum id, board id, taskid
                                DiscussionBoard board_data = _Content.GetTopicbyID(item.TaxonomyItemId.ToString());
                                string taskId = GetTaskIdForTopic(m_refContentApi.EkTaskRef.GetTopicReplies(item.TaxonomyItemId, board_data.Id), item.TaxonomyItemId);
                                contenturl = (string)("threadeddisc/addeditreply.aspx?action=Edit&topicid=" + item.TaxonomyItemId.ToString() + "&forumid=" + board_data.Forums[0].Id.ToString() + "&id=" + taskId.ToString() + "&boardid=" + board_data.Id.ToString());
                                break;
							default:
								contenturl = (string) ("content.aspx?action=View&LangType=" + item.TaxonomyItemLanguage + "&id=" + item.TaxonomyItemId + "&callerpage=taxonomy.aspx&origurl=" + EkFunctions.UrlEncode((string) ("action=view&view=item&taxonomyid=" + TaxonomyId + "&treeViewId=-1&LangType=" + TaxonomyLanguage)));
								break;
						}
                        dr["TITLE"] = m_refContentApi.GetDmsContextMenuHTML(item.TaxonomyItemId, Convert.ToInt64(item.TaxonomyItemLanguage), Convert.ToInt64(item.ContentType),Convert.ToInt32(item.ContentSubType), item.TaxonomyItemTitle.ToString(), _MessageHelper.GetMessage("generic Title") + " " + item.TaxonomyItemTitle.ToString(), contenturl, item.TaxonomyItemAssetInfo.FileName, item.TaxonomyItemAssetInfo.ImageUrl);
						//dr["TITLE"] = m_refContentApi.GetDmsContextMenuHTML(item.TaxonomyItemId, item.TaxonomyItemLanguage, item.ContentType, item.ContentSubType, item.TaxonomyItemTitle, (string) (_MessageHelper.GetMessage("generic Title") + " " + item.TaxonomyItemTitle), contenturl, item.TaxonomyItemAssetInfo.FileName, item.TaxonomyItemAssetInfo.ImageUrl);
						dr["ID"] = item.TaxonomyItemId;
						dr["LANGUAGE"] = item.TaxonomyItemLanguage;
                        switch (Convert.ToInt32(item.ContentType))
                        {
                            case 102: // ManagedAsset (non-office documents)
                                libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId);
                                if(libraryInfo != null)
                                    dr["URL"] = libraryInfo.FileName.Replace("//", "/");
                                break;
                            case 103: // Generic asset content type
                                libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId);
                                if (libraryInfo != null)
                                    dr["URL"] = libraryInfo.FileName.Replace("//", "/");
                                break;
                            case 106: // All images have content_Type 106
                                libraryInfo = m_refContentApi.GetLibraryItemByContentID(item.TaxonomyItemId);
                                if (libraryInfo != null)
                                    dr["URL"] = libraryInfo.FileName.Replace("//", "/");
                                break;
                            default:
                                Ektron.Cms.API.Content.Content api = new Ektron.Cms.API.Content.Content();
                                contData = api.GetContent(item.TaxonomyItemId);
                                //contData = m_refContentApi.GetContentById(item.TaxonomyItemId)
                                dr["URL"] = contData.Quicklink;
                                break;
                        }
                        if (item.ContentType == Convert.ToInt32(EkEnumeration.CMSContentType.Archive_Content).ToString() || item.ContentType == Convert.ToInt32(EkEnumeration.CMSContentType.Archive_Forms).ToString() || item.ContentType == Convert.ToInt32(EkEnumeration.CMSContentType.Archive_Media).ToString() || (Convert.ToInt32(item.ContentType) >= EkConstants.Archive_ManagedAsset_Min && Convert.ToInt32(item.ContentType) < EkConstants.Archive_ManagedAsset_Max && Convert.ToInt32(item.ContentType) != 3333 && Convert.ToInt32(item.ContentType) != 1111))
						{
							dr["ARCHIVED"] = "<span class=\"Archived\"></span>";
						}
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
				TaxonomyFolderSyncData[] taxonomy_sync_folder = null;
				TaxonomyBaseRequest tax_sync_folder_req = new TaxonomyBaseRequest();
				tax_sync_folder_req.TaxonomyId = TaxonomyId;
				tax_sync_folder_req.TaxonomyLanguage = TaxonomyLanguage;
				taxonomy_sync_folder = _Content.GetAllAssignedCategoryFolder(tax_sync_folder_req);
				if ((taxonomy_sync_folder != null)&& taxonomy_sync_folder.Length > 0)
				{
					AddDeleteIcon = true;
					for (int i = 0; i <= taxonomy_sync_folder.Length - 1; i++)
					{
						TaxonomyItemCount++;
						dr = dt.NewRow();
						dr["CHECK"] = "<input type=\"checkbox\" name=\"selected_items\" id=\"selected_items\" value=\"" + taxonomy_sync_folder[i].FolderId + "\" onclick=\"checkAll(\'selected_items\',true);\">";
						
						string contenturl;
						contenturl = "content.aspx?action=ViewContentByCategory&id=" + taxonomy_sync_folder[i].FolderId + "&treeViewId=0";
						
						dr["TITLE"] = "<a href=\"" + contenturl + "\">";
						dr["TITLE"] += "<img src=\"";
                        switch ((EkEnumeration.FolderType)taxonomy_sync_folder[i].FolderType)
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
						dr["TITLE"] += taxonomy_sync_folder[i].FolderTitle; //& GetRecursiveTitle(item.FolderRecursive)
						dr["TITLE"] += "</a>";
						
						dr["ID"] = taxonomy_sync_folder[i].FolderId;
						dr["LANGUAGE"] = taxonomy_sync_folder[i].TaxonomyLanguage;
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
			DataView dv = new DataView(dt);
			TaxonomyItemList.DataSource = dv;
			TaxonomyItemList.DataBind();
		}
        private string GetTaskIdForTopic(EkTasks tasks, long contentID)
        {
            if (tasks != null)
            {
                for (int i = 1; i <= tasks.Count; i++)
                {
                    if (tasks.get_Item(i) != null && tasks.get_Item(i).ContentID == contentID)
                    {
                        return tasks.get_Item(i).TaskID.ToString();
                    }
                }
            }
            return String.Empty;
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
			string IFrameVariable = "";
			string strDeleteMsg = "";
			if (Request.QueryString["iframe"] == "true")
			{
				IFrameVariable = "&iframe=true";
			}
			if (TaxonomyParentId > 0)
			{
				strDeleteMsg = _MessageHelper.GetMessage("alt delete button text (category)");
				m_strDelConfirm = _MessageHelper.GetMessage("delete category confirm");
				m_strDelItemsConfirm = _MessageHelper.GetMessage("delete category items confirm");
				m_strSelDelWarning = _MessageHelper.GetMessage("select category item missing warning");
			}
			else
			{
				strDeleteMsg = _MessageHelper.GetMessage("alt delete button text (taxonomy)");
				m_strDelConfirm = _MessageHelper.GetMessage("delete taxonomy confirm");
				m_strDelItemsConfirm = _MessageHelper.GetMessage("delete taxonomy items confirm");
				m_strSelDelWarning = _MessageHelper.GetMessage("select taxonomy item missing warning");
			}
			divTitleBar.InnerHtml = _StyleHelper.GetTitleBar((string) (_MessageHelper.GetMessage("view taxonomy page title") + " \"" + EkFunctions.HtmlEncode(_TaxonomyName) + "\"" + "&nbsp;&nbsp;<img style=\'vertical-align:middle;\' src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(TaxonomyLanguage) + "\' />"));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");

			if (Request.QueryString["iframe"] == "true")
			{
				string parentaction = "javascript:parent.CancelIframe();";
				if (Request.Params["ccp"] != null)
				{
					parentaction = "javascript:parent.CloseChildPage();";
				}
				result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/cancel.png", "#", _MessageHelper.GetMessage("generic Cancel"), _MessageHelper.GetMessage("generic Cancel"), "onClick=\"" + parentaction + "\"", StyleHelper.CancelButtonCssClass, true));
			}
			
			result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/add.png", (string) ("taxonomy.aspx?action=add&parentid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage + IFrameVariable), _MessageHelper.GetMessage("add category page title"), _MessageHelper.GetMessage("add category page title"), "", StyleHelper.AddButtonCssClass, true));
			
			if (AddDeleteIcon)
			{
				removeItemsWrapper.Visible = true;
			}
			if ((TaxonomyCategoryCount > 1) || (TaxonomyItemCount > 1))
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/arrowUpDown.png", (string) ("taxonomy.aspx?action=reorder&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&reorder=category" + "&LangType=" + TaxonomyLanguage + IFrameVariable), _MessageHelper.GetMessage("reorder taxonomy page title"), _MessageHelper.GetMessage("reorder taxonomy page title"), "", StyleHelper.ReOrderButtonCssClass));
			}
			result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/contentStackAdd.png", (string) ("taxonomy.aspx?action=additem&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage + IFrameVariable), _MessageHelper.GetMessage("assign items to taxonomy page title"), _MessageHelper.GetMessage("assign items to taxonomy page title"), "", StyleHelper.AssignItemsButtonCssClass));
			result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/folderAdd.png", (string) ("taxonomy.aspx?action=addfolder&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage + IFrameVariable), _MessageHelper.GetMessage("assign folders to taxonomy page title"), _MessageHelper.GetMessage("assign folders to taxonomy page title"), "", StyleHelper.AssignFoldersButtonCssClass));
			
			result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/contentEdit.png", (string) ("taxonomy.aspx?action=edit&taxonomyid=" + TaxonomyId + "&parentid=" + TaxonomyParentId + "&LangType=" + TaxonomyLanguage + IFrameVariable), _MessageHelper.GetMessage("alt edit button text (taxonomy)"), _MessageHelper.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass));
			if (TaxonomyParentId == 0)
			{
				result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/taxonomyExport.png", "#", _MessageHelper.GetMessage("alt export taxonomy"), _MessageHelper.GetMessage("btn export taxonomy"), "onclick=\"window.open(\'taxonomy_imp_exp.aspx?action=export&taxonomyid=" + TaxonomyId + "&LangType=" + TaxonomyLanguage + "\',\'exptaxonomy\',\'status=0,toolbar=0,location=0,menubar=0,directories=0,resizable=0,scrollbars=1,height=100px,width=200px\');void(0);\"", StyleHelper.ExportTaxonomyButtonCssClass));
			}
			result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/contentCopy.png", "#", _MessageHelper.GetMessage("generic move copy taxonomy"), _MessageHelper.GetMessage("generic move copy taxonomy"), "onclick=\"$ektron(\'#TaxonomySelect\').modalShow();\"", StyleHelper.CopyButtonCssClass));
			result.Append(_StyleHelper.GetButtonEventsWCaption(AppPath + "images/ui/Icons/delete.png", "#", _MessageHelper.GetMessage("generic delete title"), _MessageHelper.GetMessage("alt delete button text (taxonomy)"), "onclick=\"return DeleteItem();\"", StyleHelper.DeleteButtonCssClass));

			if (Request.QueryString["iframe"] != "true")
			{
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td nowrap=\"true\">");
				string addDD;
				addDD = GetLanguageForTaxonomy(TaxonomyId, "", false, false, "javascript:TranslateTaxonomy(" + TaxonomyId + ", " + TaxonomyParentId + ", this.value);");
				if (addDD != "")
				{
					addDD = (string)("&nbsp;" + _MessageHelper.GetMessage("add title") + ":&nbsp;" + addDD);
				}
				if (_Common.EnableMultilingual == 1)
				{
                    result.Append(_MessageHelper.GetMessage("view in label") + ":&nbsp;" + GetLanguageForTaxonomy(TaxonomyId, "", true, false, "javascript:LoadLanguage(this.value);") + "&nbsp;" + addDD + "<br>");
				}
				result.Append("</td>");
			}

			result.Append(StyleHelper.ActionBarDivider);
			result.Append(ViewTypeDropDown());
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _StyleHelper.GetHelpButton("ViewTaxonomyOrCategory", "") + "</td>");
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
			result.Append("<option value=\"folder\" " + FindSelected("folder") + ">").Append(this._MessageHelper.GetMessage("lbl folders")).Append("</option>");
			result.Append("<option value=\"item\"  " + FindSelected("item") + ">").Append(this._MessageHelper.GetMessage("content button text")).Append("</option>");
			result.Append("<option value=\"user\"  " + FindSelected("user") + ">").Append(this._MessageHelper.GetMessage("generic users")).Append("</option>");
			result.Append("<option value=\"cgroup\"  " + FindSelected("cgroup") + ">").Append(this._MessageHelper.GetMessage("lbl community groups")).Append("</option>");
			result.Append("</select>");
			result.Append("</td>");
			return result.ToString();
		}
		
		private string FindSelected(string chk)
		{
			string val = "";
			if (_ViewItem == chk)
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
				result_language = _Content.LoadLanguageForTaxonomy(taxonomy_language_request);
				frmName = "frm_translated";
			}
			else
			{
				taxonomy_language_request.IsTranslated = false;
				result_language = _Content.LoadLanguageForTaxonomy(taxonomy_language_request);
				frmName = "frm_nontranslated";
			}
			result = "<select id=\"" + frmName + "\" name=\"" + frmName + "\" onchange=\"" + onChangeEv + "\">" + "\r\n";
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
			if ((result_language != null) && (result_language.Count > 0) && (_Common.EnableMultilingual == 1))
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
		private void PageSettings()
		{
			if (m_intTotalPages <= 1)
			{
				VisiblePageControls(false);
			}
			else
			{
				VisiblePageControls(true);
				TotalPages.Text = m_intTotalPages.ToString();
				TotalPages.ToolTip = TotalPages.Text;
				CurrentPage.Text = m_intCurrentPage.ToString();
				CurrentPage.ToolTip = CurrentPage.Text;
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
			customPropertyRecordsPerPage.Value= _Common.RequestInformationRef.PagingSize.ToString();
		}
		
		private string GetValueDropDown(CustomPropertyData _propertyDataList, int count)
		{
			StringBuilder result = new StringBuilder();
			int iObj = 0;
			result.Append("<select disabled name=\"selCustPropVal" + count + "\" id=\"selCustPropVal" + count + "\">");
			if (!(_propertyDataList == null))
			{
				for (iObj = 0; iObj <= _propertyDataList.Items.Count - 1; iObj++)
				{
					if (_propertyDataList.Items[iObj].IsDefault)
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

        private void ReloadClientScript(string idPath)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            try
            {
                idPath = idPath.Replace("\\", "\\\\");
                result.Append("top.TreeNavigation(\"TaxTree\", \"" + idPath + "\");" + "\r\n");
                Ektron.Cms.API.JS.RegisterJSBlock(this.Parent.Parent.Parent.Parent.Page.Header, result.ToString(), "ReloadClientScript");
            }
            catch (Exception)
            {
            }
        }
	}

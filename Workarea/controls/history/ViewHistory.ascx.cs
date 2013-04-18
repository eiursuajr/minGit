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
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.Site;

	public partial class ViewHistory : System.Web.UI.UserControl
	{
		
		
		#region Private members
		
		protected string AppName = "";
		protected EkMessageHelper m_refMsg;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected ContentData content_data;
		protected long ContentId = -1;
		protected long HistoryId = -1;
		protected string AppImgPath = "";
		protected ContentAPI m_refContentApi;
		protected PermissionData security_data;
		protected int ContentLanguage = -1;
		protected bool bXmlContent = false;
		protected bool bApplyXslt = false;
		protected ContentData hist_content_data;
		protected bool bIsBlog = false;
		private BlogPostData blog_post_data;
		private string[] arrBlogPostCategories;
		private int i = 0;
		
		//Commerce declarations begins
		protected long m_contentType = EkConstants.CMSContentType_Content;
		protected CatalogEntry m_refCatalog = null;
		protected EntryData entry_data = null;
		protected EntryData entry_version_data = null;
		protected int m_intId = 0;
		protected List<EntryAttributeData> attrib_data = new List<EntryAttributeData>();
		protected EkSite m_refSite = null;
		protected int m_iFolder = 0;
		protected string m_sEditAction = "";
		protected FolderData catalog_data = new FolderData();
		protected int lValidCounter = 0;
		protected int xid = 0;
		protected string imagePath = "";
		//Commerce declaration ends
		
		#endregion
		
		#region Events
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			m_refContentApi = new ContentAPI();
			
			//register page components
			this.RegisterJS();
			this.RegisterCSS();
			
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			try
			{
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
				
				m_refMsg = m_refContentApi.EkMsgRef;
				
				if (Request.QueryString["id"] != "")
				{
					ContentId = Convert.ToInt64(Request.QueryString["id"]);
				}
				
				if (Request.QueryString["hist_id"] != "")
				{
					HistoryId = Convert.ToInt64(Request.QueryString["hist_id"]);
				}
				
				if (!(Page.IsPostBack))
				{
					
					AppImgPath = m_refContentApi.AppImgPath;
					AppName = m_refContentApi.AppName;
					ContentLanguage = m_refContentApi.ContentLanguage;
					imagePath = m_refContentApi.AppPath + "images/ui/icons/";
					
					content_data = new ContentData();
					
					if (ContentId > 0 && HistoryId > 0)
					{
						
						if (Request.QueryString["xslt"] == "remove")
						{
							bApplyXslt = false;
						}
						else
						{
							bApplyXslt = true;
						}
						
						security_data = m_refContentApi.LoadPermissions(ContentId, "content", ContentAPI.PermissionResultType.Content);
						
						m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId);
						
						switch (m_contentType)
						{
							
							case EkConstants.CMSContentType_CatalogEntry:
								
								Ektron.Cms.Commerce.CatalogEntryApi m_refCatalogAPI = new Ektron.Cms.Commerce.CatalogEntryApi();
								
								entry_data = m_refCatalogAPI.GetItem(ContentId);
								entry_version_data = m_refCatalogAPI.GetItemVersion(ContentId, m_refContentApi.ContentLanguage, HistoryId);
								PopulateCatalogPageData(entry_version_data, entry_data);
								Display_EntryHistoryToolBar(entry_data);
								break;
								
							default:
								
								content_data = m_refContentApi.GetContentById(ContentId, 0);
								hist_content_data = m_refContentApi.GetContentByHistoryId(HistoryId);
								FolderData folder_data;
								blog_post_data = new BlogPostData();
								blog_post_data.Categories = new string[0];
								if (!(content_data == null))
								{
									bIsBlog = System.Convert.ToBoolean(m_refContentApi.EkContentRef.GetFolderType(content_data.FolderId) == Ektron.Cms.Common.EkEnumeration.FolderType.Blog);
									if (bIsBlog)
									{
										folder_data = m_refContentApi.GetFolderById(content_data.FolderId);
										if (hist_content_data.MetaData != null)
										{
											for (int i = 0; i <= (hist_content_data.MetaData.Length - 1); i++)
											{
                                                Ektron.Cms.Common.EkEnumeration.BlogPostDataType MetaType = (Ektron.Cms.Common.EkEnumeration.BlogPostDataType)Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.BlogPostDataType), hist_content_data.MetaData[i].TypeId.ToString());

                                                if (MetaType == Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Categories || hist_content_data.MetaData[i].TypeName.ToLower().IndexOf("blog categories") > -1)
												{
                                                    hist_content_data.MetaData[i].Text = hist_content_data.MetaData[i].Text.Replace("&#39;", "\'");
                                                    hist_content_data.MetaData[i].Text = hist_content_data.MetaData[i].Text.Replace("&quot", "\"");
                                                    hist_content_data.MetaData[i].Text = hist_content_data.MetaData[i].Text.Replace("&gt;", ">");
                                                    hist_content_data.MetaData[i].Text = hist_content_data.MetaData[i].Text.Replace("&lt;", "<");
                                                    blog_post_data.Categories = Strings.Split((string)(hist_content_data.MetaData[i].Text), ";", -1, 0);
												}
                                                else if (MetaType == Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Ping || hist_content_data.MetaData[i].TypeName.ToLower().IndexOf("blog pingback") > -1)
												{
                                                    blog_post_data.Pingback = Ektron.Cms.Common.EkFunctions.GetBoolFromYesNo((string)(hist_content_data.MetaData[i].Text));
												}
                                                else if (MetaType == Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Tags || hist_content_data.MetaData[i].TypeName.ToLower().IndexOf("blog tags") > -1)
												{
                                                    blog_post_data.Tags = (string)(hist_content_data.MetaData[i].Text);
												}
                                                else if (MetaType == Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Trackback || hist_content_data.MetaData[i].TypeName.ToLower().IndexOf("blog trackback") > -1)
												{
                                                    blog_post_data.TrackBackURL = (string)(hist_content_data.MetaData[i].Text);
												}
											}
										}
										if (!(folder_data.XmlConfiguration == null))
										{
											bXmlContent = true;
										}
									}
									hist_content_data.Type = content_data.Type;
									PopulatePageData(hist_content_data, content_data);
								}
								Display_ContentHistoryToolBar();
								break;
								
						}
						
					}
					else if (ContentId > 0)
					{
						
						m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId);
						
						switch (m_contentType)
						{
							
							case EkConstants.CMSContentType_CatalogEntry:
								
								Ektron.Cms.Commerce.CatalogEntryApi m_refCatalogAPI = new Ektron.Cms.Commerce.CatalogEntryApi();
								
								entry_data = m_refCatalogAPI.GetItem(ContentId);
								entry_version_data = m_refCatalogAPI.GetItemVersion(ContentId, m_refContentApi.ContentLanguage, HistoryId);
								PopulateCatalogPageData(entry_version_data, entry_data);
								Display_EntryHistoryToolBar(entry_data);
								break;
								
							default:
								
								content_data = m_refContentApi.GetContentById(ContentId, 0);
								PopulatePageData(hist_content_data, content_data);
								Display_ContentHistoryToolBar();
								break;
								
						}
						
					}
					
				}
				else
				{
					
					m_contentType = m_refContentApi.EkContentRef.GetContentType(ContentId);
					content_data = m_refContentApi.GetContentById(ContentId, 0);
					
					switch (m_contentType)
					{
						
						case EkConstants.CMSContentType_CatalogEntry:
							
							Ektron.Cms.Commerce.CatalogEntryApi m_refCatalogAPI = new Ektron.Cms.Commerce.CatalogEntryApi();
							HistoryId = Convert.ToInt64(Request.QueryString["hist_id"]);
							m_refCatalogAPI.Restore(ContentId, HistoryId);
							break;
							
						default:
							
							HistoryId = Convert.ToInt64(Request.QueryString["hist_id"]);
							m_refContentApi.RestoreHistoryContent(HistoryId);
							break;
							
					}
					
					CloseOnRestore.Text = "<script type=\"text/javascript\">try { location.href = \'content.aspx?LangType=" + ContentLanguage + "&action=ViewStaged&id=" + ContentId + "&fldid=" + content_data.FolderId + "\'; } catch(e) {}</script>";
					
				}
			}
			catch (Exception ex)
			{
				
				ShowError(ex.Message);
				
			}
		}
		
		#endregion
		
		
		#region Catalog entry
		
		private void Display_EntryHistoryToolBar(EntryData entry_data)
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("view catalog entry history title") + " \"" + entry_data.Title + "\""));
			
			if (HistoryId != -1)
			{
				result.Append("<table><tr>");
				result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

				bool primaryCssApplied = false;
				
				if (security_data.CanRestore)
				{
					restore_id.Value = HistoryId.ToString();
					
					if ((entry_data.Status.ToLower() != "o") && (entry_data.Status.ToLower() != "s") && (entry_data.Status.ToLower() != "p"))
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "restore.png", "#", m_refMsg.GetMessage("alt restore button text"), m_refMsg.GetMessage("btn restore"), "  onclick=\"javascript:document.forms[0].submit();return false;\" target=\"history_frame\"", StyleHelper.RestoreButtonCssClass, !primaryCssApplied));

						primaryCssApplied = true;
					}

					result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "contentViewDifferences.png", "#", "View Content Difference", m_refMsg.GetMessage("btn view diff"), "onclick=\"javascript:PopUpWindow(\'compare.aspx?LangType=" + ContentLanguage + "&id=" + ContentId + "&hist_id=" + HistoryId + "\', \'Compare\', 800, 530, 0, 0);return false;\"", StyleHelper.ViewDifferenceButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
				}
				
				result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "history.png", (string) ("history.aspx?action=report&LangType=" + ContentLanguage + "&id=" + Request.QueryString["id"]), m_refMsg.GetMessage("view history report"), m_refMsg.GetMessage("view history report"), "", StyleHelper.HistoryButtonCssClass, !primaryCssApplied));
				
				primaryCssApplied = true;
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton("ViewContentHistory", "") + "</td>");
				result.Append("</tr></table>");
				
				divToolBar.InnerHtml = result.ToString();
			}
			else
			{
				divToolBar.Style.Add("height", "0");
			}
			
			result = null;
		}
		
		private void PopulateCatalogPageData(EntryData entry_data_version, EntryData entry_data)
		{
			bool bPackageDisplayXSLT = false;
			string CurrentXslt = "";
			
			Display_PropertiesTab(entry_data_version);
			Display_PricingTab(entry_data_version);
			Display_MetadataTab(entry_data_version);
			
			phItemsTab.Visible = false;
			phItems.Visible = false;
			//sp_item.Visible = False
			
			if ((!(entry_data_version.ProductType == null)) && (bApplyXslt))
			{
				if (entry_data_version.ProductType.PackageDisplayXslt.Length > 0)
				{
					bPackageDisplayXSLT = true;
				}
				else
				{
					if (entry_data_version.ProductType.DefaultXslt.Length > 0)
					{
						bPackageDisplayXSLT = false;



                        Collection xsltPhysPath = (Collection)content_data.XmlConfiguration.PhysPathComplete;
                        Collection xsltLogicalPath = (Collection)content_data.XmlConfiguration.LogicalPathComplete;
                        if (xsltPhysPath.Contains("Xslt" + entry_data_version.ProductType.DefaultXslt))
                        {
                            CurrentXslt = xsltPhysPath["Xslt" + entry_data_version.ProductType.DefaultXslt].ToString();
                        }
                        else
                        {
                            CurrentXslt = xsltLogicalPath["Xslt" + entry_data_version.ProductType.DefaultXslt].ToString();
                        }
					}
					else
					{
						bPackageDisplayXSLT = true;
					}
				}
				
				if (bPackageDisplayXSLT)
				{
					divContentHtml.InnerHtml = m_refContentApi.XSLTransform(entry_data_version.Html, entry_data_version.ProductType.PackageDisplayXslt, false, false, null, false, true);
				}
				else
				{
					divContentHtml.InnerHtml = m_refContentApi.TransformXSLT(entry_data_version.Html, CurrentXslt);
				}
			}
			else
			{
				divContentHtml.InnerHtml = entry_data_version.Html;
			}
			
			tdsummaryhead.InnerHtml = m_refMsg.GetMessage("content summary label");
			
			tdsummarytext.InnerHtml += entry_data_version.Summary;
			
			tdcommenthead.InnerHtml = m_refMsg.GetMessage("content HC label");
			tdcommenttext.InnerHtml = entry_data_version.Comment;
		}
		
		private void Display_PropertiesTab(EntryData entry_data_version)
		{
			
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			
			colBound.DataField = "NAME";
			colBound.HeaderText = "";
			colBound.ItemStyle.CssClass = "label";
			PropertiesGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = "";
			PropertiesGrid.Columns.Add(colBound);
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("NAME", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			
			int i = 0;
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content title label");
			dr[1] = entry_data_version.Title;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content id label");
			dr[1] = entry_data_version.Id;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content language label");
			dr[1] = entry_data_version.LanguageId;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("lbl calatog entry sku") + "&nbsp;#:";
			dr[1] = entry_data.Sku;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content status label");
			switch (entry_data_version.Status.ToLower())
			{
				case "a":
					dr[1] = m_refMsg.GetMessage("status:Approved (Published)");
					break;
				case "o":
					dr[1] = m_refMsg.GetMessage("status:Checked Out");
					break;
				case "i":
					dr[1] = m_refMsg.GetMessage("status:Checked In");
					break;
				case "p":
					dr[1] = m_refMsg.GetMessage("status:Approved (PGLD)");
					break;
				case "m":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Deletion") + "</font>";
					break;
				case "s":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Approval") + "</font>";
					break;
				case "t":
					dr[1] = m_refMsg.GetMessage("status:Waiting Approval");
					break;
				case "d":
					dr[1] = "Deleted (Pending Start Date)";
					break;
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content LUE label");
			dr[1] = entry_data_version.LastEditorFirstName + " " + entry_data_version.LastEditorLastName; //DisplayUserName
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content LED label");
			dr[1] = entry_data_version.DateModified;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic start date label");
			if (entry_data_version.GoLive == DateTime.MinValue || entry_data_version.GoLive == DateTime.MaxValue)
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			else
			{
				dr[1] = entry_data_version.GoLive;
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic end date label");
			if (entry_data_version.EndDate == DateTime.MinValue || entry_data_version.EndDate == DateTime.MaxValue)
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			else
			{
				dr[1] = entry_data_version.EndDate;
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("End Date Action Title");
			
			if (entry_data_version.EndDate == DateTime.MinValue || entry_data_version.EndDate == DateTime.MaxValue)
			{
				if (entry_data_version.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_archive_display)
				{
					dr[1] = m_refMsg.GetMessage("Archive display descrp");
				}
				else if (entry_data_version.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_refresh)
				{
					dr[1] = m_refMsg.GetMessage("Refresh descrp");
				}
				else
				{
					dr[1] = m_refMsg.GetMessage("Archive expire descrp");
				}
			}
			else
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content DC label");
			dr[1] = entry_data_version.DateModified; //DisplayDateCreated
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content approvals label");
			System.Text.StringBuilder approvallist = new System.Text.StringBuilder();
			ApprovalData[] approvaldata;
			approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(ContentId);
			approvallist.Append(m_refMsg.GetMessage("none specified msg"));
			if (!(approvaldata == null))
			{
				if (approvaldata.Length > 0)
				{
					approvallist.Length = 0;
					for (i = 0; i <= approvaldata.Length - 1; i++)
					{
						if (approvaldata[i].Type == "user")
						{
							approvallist.Append("<img src=\"" + imagePath + "user.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user") + "\" title=\"" + m_refMsg.GetMessage("approver is user") + "\">");
						}
						else
						{
							approvallist.Append("<img src=\"" + imagePath + "user.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user group") + "\" title=\"" + m_refMsg.GetMessage("approver is user group") + "\">");
						}
						if (approvaldata[i].IsCurrentApprover)
						{
							approvallist.Append("<span class=\"important\">");
						}
						else
						{
							approvallist.Append("<span>");
						}
						approvallist.Append(approvaldata[i].DisplayUserName + "</span>");
					}
				}
			}
			dr[1] = approvallist.ToString();
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("xml configuration label");
			dr[1] = "&nbsp;" + entry_data_version.ProductType.Title;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic template label");
			dr[1] = "";
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic Path");
			dr[1] = m_refContentApi.EkContentRef.GetFolderPath(entry_data_version.FolderId);
			
			DataView dv = new DataView(dt);
			PropertiesGrid.DataSource = dv;
			PropertiesGrid.DataBind();
		}
		
		private void Display_MetadataTab(EntryData versionData)
		{
			StringBuilder sbAttrib = new StringBuilder();
			StringBuilder sbResult = new StringBuilder();
			string strResult;
			string strAttrResult;
			string strImage = "";
			ProductTypeApi prod_type_API = new ProductTypeApi();
			ProductTypeData prod_type_data = null;
			
			EnhancedMetadataScript.Text = CustomFields.GetEnhancedMetadataScript().Replace("src=\"java/", "src=\"../java/");
			EnhancedMetadataArea.Text = CustomFields.GetEnhancedMetadataArea();
			
			if (prod_type_data == null)
			{
				prod_type_data = prod_type_API.GetItem(versionData.ProductType.Id);
			}
			
			if (versionData.Metadata.Count > 0 || prod_type_data.Attributes.Count > 0)
			{
				
				m_refSite = new Ektron.Cms.Site.EkSite(this.m_refContentApi.RequestInformationRef);
				Hashtable hPerm = m_refSite.GetPermissions(m_iFolder, 0, "folder");
				sbResult.Append(Ektron.Cms.CustomFields.WriteFilteredMetadataForView(versionData.Metadata.ToArray(), versionData.FolderId, false).Trim());
				if (prod_type_data != null)
				{
					sbAttrib.Append(CustomFields.WriteFilteredAttributesForView(versionData.Attributes, prod_type_data.Id, false,prod_type_data.Attributes));
				}
				
			}
			
			if (m_sEditAction == "update")
			{
				strImage = versionData.Image;
				string strThumbnailPath = versionData.ImageThumbnail;
				if (versionData.ImageThumbnail == "")
				{
					strThumbnailPath = m_refContentApi.AppImgPath + "spacer.gif";
				}
				else if (catalog_data.IsDomainFolder == true)
				{
					
				}
				else
				{
					strThumbnailPath = m_refContentApi.SitePath + strThumbnailPath;
				}
				sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=\"label\" align=\"left\">Image:</td><td><span id=\"sitepath\"" + this.m_refContentApi.SitePath + "</span><input type=\"textbox\" size=\"30\" readonly=\"true\" id=\"content_image\" name=\"content_image\" value=\"" + strImage + "\" /></td></tr><tr><td colomnspan=\"2\"><img id=\"content_image_thumb\" src=\"" + strThumbnailPath + "\" /></td></tr></table></fieldset>");
			}
			else
			{
				sbResult.Append("<fieldset><legend>Image Data:</legend><table><tr><td class=\"label\" align=\"left\">Image:</td><td><span id=\"sitepath\"" + this.m_refContentApi.SitePath + "</span><input type=\"textbox\" size=\"30\" readonly=\"true\" id=\"content_image\" name=\"content_image\" value=\"" + strImage + "\" /></td></tr><tr><td colomnspan=\"2\"><img id=\"content_image_thumb\" src=\"" + m_refContentApi.AppImgPath + "spacer.gif\" /></td></tr></table></fieldset>");
			}
			strAttrResult = (string) (sbAttrib.ToString().Trim());
			strResult = sbResult.ToString().Trim();
			strResult = Util_FixPath(strResult);
			
			MetaDataValue.Text = strResult;
			ltr_attrib.Text = strAttrResult;
		}
		
		private string Util_FixPath(string MetaScript)
		{
			int iTmp = -1;
			iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", 0);
			while (iTmp > -1)
			{
				iTmp = MetaScript.IndexOf(");return (false);", iTmp);
				MetaScript = MetaScript.Insert(iTmp, ", \'" + this.m_refContentApi.ApplicationPath + "\'");
				iTmp = MetaScript.IndexOf("ek_ma_LoadMetaChildPage(", iTmp + 1);
			}
			return MetaScript;
		}
		
		private void Display_PricingTab(EntryData versionData)
		{
			
			Currency m_refCurrency = new Currency(m_refContentApi.RequestInformationRef);
			Ektron.Cms.Workarea.workareabase workarearef = new Ektron.Cms.Workarea.workareabase();
			List<CurrencyData> activeCurrencyList = m_refCurrency.GetActiveCurrencyList();
			List<ExchangeRateData> exchangeRateList = new List<ExchangeRateData>();
			if (activeCurrencyList.Count > 1)
			{
				ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
				Criteria<ExchangeRateProperty> exchangeRateCriteria = new Criteria<ExchangeRateProperty>();
				List<long> currencyIDList = new List<long>();
				for (int i = 0; i <= (activeCurrencyList.Count - 1); i++)
				{
					currencyIDList.Add(activeCurrencyList[i].Id);
				}
				exchangeRateCriteria.AddFilter(ExchangeRateProperty.BaseCurrencyId, CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
				exchangeRateCriteria.AddFilter(ExchangeRateProperty.ExchangeCurrencyId, CriteriaFilterOperator.In, currencyIDList.ToArray());
				exchangeRateList = exchangeRateApi.GetCurrentList(exchangeRateCriteria);
			}
			ltr_pricing.Text = workarearef.CommerceLibrary.GetPricingMarkup(versionData.Pricing, activeCurrencyList, exchangeRateList, entry_data.EntryType, false, workareaCommerce.ModeType.View);
			
		}
		
		#endregion
		
		
		#region Other
		
		private void Display_ContentHistoryToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("view content history title") + " \"" + content_data.Title + "\""));
			
			if (HistoryId != -1)
			{
				result.Append("<table><tr>");

				result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "javascript:history.go(-1);", m_refMsg.GetMessage("btn back"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));

				bool primaryCssApplied = false;

				if (security_data.CanRestore)
				{
					restore_id.Value = HistoryId.ToString();
					if ((content_data.Status.ToLower() != "s") && (content_data.Status.ToLower() != "p"))
					{
						if (content_data.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent)
						{
							string action = "if(confirm(\'This will only restore description changes. Are you sure?\')){javascript:document.forms[0].submit();}return false;";
							result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "restore.png", "#", m_refMsg.GetMessage("alt restore button text"), m_refMsg.GetMessage("btn restore"), " onclick=\"" + action + "\" target=\"history_frame\"", StyleHelper.RestoreButtonCssClass, !primaryCssApplied));
						}
						else
						{
							result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "restore.png", "#", m_refMsg.GetMessage("alt restore button text"), m_refMsg.GetMessage("btn restore"), " onclick=\"javascript:document.forms[0].submit();return false;\" target=\"history_frame\"", StyleHelper.RestoreButtonCssClass, !primaryCssApplied));
						}

						primaryCssApplied = true;
					}
				}

                if (content_data.ContType != 8 &&  content_data.SubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData && content_data.SubType != Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
				{
					result.Append(m_refStyle.GetButtonEventsWCaption(imagePath + "contentViewDifferences.png", "#", "View Content Difference", m_refMsg.GetMessage("btn view diff"), "onclick=\"javascript:PopUpWindow(\'compare.aspx?LangType=" + ContentLanguage + "&id=" + ContentId + "&hist_id=" + HistoryId + "\', \'Compare\', 800, 530, scrollbar=\'yes\', 0, 0);return false;\"", StyleHelper.ViewDifferenceButtonCssClass, !primaryCssApplied));

					primaryCssApplied = true;
				}
				
				if (bXmlContent && content_data.Type == 1 && (content_data.XmlConfiguration != null))
				{
					if (bApplyXslt)
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_noviewxslt-nm.gif", "history.aspx?LangType=" + ContentLanguage + "&xslt=remove&Id=" + ContentId + "&hist_id=" + HistoryId + "", "Remove applied XSLT", m_refMsg.GetMessage("btn view no xslt"), "target=\"history_frame\"", StyleHelper.HistoryButtonCssClass, !primaryCssApplied));
					}
					else
					{
						result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_viewxslt-nm.gif", "history.aspx?LangType=" + ContentLanguage + "&xslt=apply&Id=" + ContentId + "&hist_id=" + HistoryId + "", "Apply default XSLT", m_refMsg.GetMessage("btn view xslt"), "target=\"history_frame\"", StyleHelper.HistoryButtonCssClass, !primaryCssApplied));
					}

					primaryCssApplied = true;
				}
				result.Append(StyleHelper.ActionBarDivider);
				result.Append("<td>" + m_refStyle.GetHelpButton("ViewContentHistory", "") + "</td>");
				result.Append("</tr></table>");
				divToolBar.InnerHtml = result.ToString();
			}
			else
			{
				divToolBar.Style.Add("height", "0");
			}
			
			result = null;
		}
		
		/// <summary>
		/// Fills the history tabs with content body, summary, meta, and properties
		/// </summary>
		/// <param name="content_data_history"></param>
		/// <param name="content_data">This parameter is needed to get the XML XSLT INFO </param>
		/// <remarks></remarks>
		private void PopulatePageData(ContentData content_data_history, ContentData content_data)
		{
			bool bPackageDisplayXSLT = false;
			string CurrentXslt = "";
			
			phPricingTab.Visible = false;
			phPricing.Visible = false;
			phAttributesTab.Visible = false;
			phAttributes.Visible = false;
			phItemsTab.Visible = false;
			phItems.Visible = false;
			
			ViewContentProperties(content_data_history);
			ViewMetaData(content_data_history);
			if ((!(content_data.XmlConfiguration == null)) && (content_data.Type == 1 || content_data.Type == 2))
			{
				if ((!(content_data.XmlConfiguration == null)) && (bApplyXslt))
				{
					if (content_data.XmlConfiguration.PackageDisplayXslt.Length > 0)
					{
						bPackageDisplayXSLT = true;
					}
					else
					{
						if (content_data.XmlConfiguration.DefaultXslt.Length > 0)
						{
							bPackageDisplayXSLT = false;

                            Collection xsltPhysPath = (Collection)content_data.XmlConfiguration.PhysPathComplete;
                            Collection xsltLogicalPath = (Collection)content_data.XmlConfiguration.LogicalPathComplete;
                            if (xsltPhysPath.Contains("Xslt" + content_data.XmlConfiguration.DefaultXslt))
                            {
                                CurrentXslt = xsltPhysPath["Xslt" + content_data.XmlConfiguration.DefaultXslt].ToString();
                            }
                            else
                            {
                                CurrentXslt = xsltLogicalPath["Xslt" + content_data.XmlConfiguration.DefaultXslt].ToString();
                            }
						}
						else
						{
							bPackageDisplayXSLT = true;
						}
					}
					
					if (bPackageDisplayXSLT)
					{
						divContentHtml.InnerHtml = m_refContentApi.XSLTransform(content_data_history.Html, content_data.XmlConfiguration.PackageDisplayXslt, false, false, null, false, true);
					}
					else
					{
						divContentHtml.InnerHtml = m_refContentApi.TransformXSLT(content_data_history.Html, CurrentXslt);
					}
				}
				else
				{
					divContentHtml.InnerHtml = content_data_history.Html;
				}
			}
			else
			{
				if (content_data_history.Type == 104)
				{
					divContentHtml.InnerHtml = FixContentHistory(content_data_history, content_data_history.Html);
				}
				else
				{
					if (content_data_history.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData || content_data_history.SubType == Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderMasterData)
					{
						divContentHtml.InnerHtml = Ektron.Cms.PageBuilder.PageData.RendertoString(content_data_history.Html);
					}
					else
					{
						divContentHtml.InnerHtml = content_data_history.Html;
					}
				}
			}
			
			tdsummaryhead.InnerHtml = m_refMsg.GetMessage("content summary label");
            Ektron.Cms.Common.EkEnumeration.CMSContentType contenttype_history = (Ektron.Cms.Common.EkEnumeration.CMSContentType)Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.CMSContentType), content_data_history.Type.ToString());
            if (Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms == contenttype_history || Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Forms == contenttype_history)
			{
				if (content_data_history.Teaser != null)
				{
					if (content_data_history.Teaser.IndexOf("<ektdesignpackage_design") > -1)
					{
						string strDesign;
						strDesign = m_refContentApi.XSLTransform(null, null, true, false, null, true);
						tdsummarytext.InnerHtml = strDesign;
					}
					else
					{
						tdsummarytext.InnerHtml += content_data_history.Teaser;
					}
				}
				else
				{
					tdsummarytext.InnerHtml = "";
				}
			}
			else
			{
				if (bIsBlog)
				{
					tdsummarytext.InnerHtml += "<table border=\"0\" cellpadding=\"4\" width=\"550\">";
					tdsummarytext.InnerHtml += "	<tr>";
					tdsummarytext.InnerHtml += "		<td width=\"20\">&nbsp;</td>";
					tdsummarytext.InnerHtml += "		<td valign=\"top\" width=\"80%\">";
					tdsummarytext.InnerHtml += "			<b>Description</b>";
					tdsummarytext.InnerHtml += "		</td>";
					tdsummarytext.InnerHtml += "		<td width=\"20\">&nbsp;</td>";
					tdsummarytext.InnerHtml += "		<td valign=\"top\" width=\"20%\">";
					tdsummarytext.InnerHtml += "			<b>Categories</b>";
					tdsummarytext.InnerHtml += "		</td>";
					tdsummarytext.InnerHtml += "	</tr>";
					tdsummarytext.InnerHtml += "	<tr>";
					tdsummarytext.InnerHtml += "		<td width=\"20\">&nbsp;</td>";
					tdsummarytext.InnerHtml += "		<td valign=\"top\">";
				}
				tdsummarytext.InnerHtml += content_data_history.Teaser;
				if (bIsBlog)
				{
					tdsummarytext.InnerHtml += "			<br/><br/>";
					tdsummarytext.InnerHtml += "			<b>Tags</b>";
					tdsummarytext.InnerHtml += "			<br/>";
					if (!(blog_post_data == null))
					{
						tdsummarytext.InnerHtml += blog_post_data.Tags;
					}
					tdsummarytext.InnerHtml += "		</td>";
					tdsummarytext.InnerHtml += "		<td width=\"20\">&nbsp;</td>";
					tdsummarytext.InnerHtml += "		<td valign=\"top\" style=\"border: 1px solid #fffff; \"  width=\"20%\">";
					tdsummarytext.InnerHtml += "	<p>";
					
					if (!(blog_post_data.Categories == null))
					{
						arrBlogPostCategories = blog_post_data.Categories;
						if (arrBlogPostCategories.Length > 0)
						{
							Array.Sort(arrBlogPostCategories);
						}
					}
					else
					{
						arrBlogPostCategories = null;
					}
					if (blog_post_data.Categories.Length > 0)
					{
						for (i = 0; i <= (blog_post_data.Categories.Length - 1); i++)
						{
							if (blog_post_data.Categories[i].ToString() != "")
							{
								tdsummarytext.InnerHtml += "				<input type=\"checkbox\" name=\"blogcategories" + i.ToString() + "\" value=\"" + blog_post_data.Categories[i].ToString() + "\" checked=\"true\" disabled>&nbsp;" + Strings.Replace((string) (blog_post_data.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0) + "<br>";
							}
						}
					}
					else
					{
						tdsummarytext.InnerHtml += "No categories defined.";
					}
					tdsummarytext.InnerHtml += "				<br/>";
					tdsummarytext.InnerHtml += "			</p>";
					tdsummarytext.InnerHtml += "		</td>";
					tdsummarytext.InnerHtml += "	</tr>";
					tdsummarytext.InnerHtml += "	<tr>";
					tdsummarytext.InnerHtml += "		<td width=\"20\">&nbsp;</td>";
					tdsummarytext.InnerHtml += "	    <td colspan=\"3\">";
					if (!(blog_post_data == null))
					{
						tdsummarytext.InnerHtml += "<br/><input type=\"hidden\" name=\"blogposttrackbackid\" id=\"blogposttrackbackid\" value=\"" + blog_post_data.TrackBackURLID.ToString() + "\" /><input type=\"hidden\" id=\"isblogpost\" name=\"isblogpost\" value=\"true\"/><br/><b>TrackBack URL</b><br/>";
						tdsummarytext.InnerHtml += "<input type=\"text\" size=\"75\" id=\"trackback\" name=\"trackback\" value=\"" + EkFunctions.HtmlEncode(blog_post_data.TrackBackURL) + "\" disabled/>";
						tdsummarytext.InnerHtml += "<br/><br/>";
						if (blog_post_data.Pingback == true)
						{
							tdsummarytext.InnerHtml += "<input type=\"checkbox\" name=\"pingback\" id=\"pingback\" checked disabled/>&nbsp;PingBack URLs in this post";
						}
						else
						{
							tdsummarytext.InnerHtml += "<input type=\"checkbox\" name=\"pingback\" id=\"pingback\" disabled/>&nbsp;PingBack URLs in this post";
						}
					}
					else
					{
						tdsummarytext.InnerHtml += "<br/><input type=\"hidden\" name=\"blogposttrackbackid\" id=\"blogposttrackbackid\" value=\"\" /><input type=\"hidden\" id=\"isblogpost\" name=\"isblogpost\" value=\"true\"/><br/><b>TrackBack URL</b><br/>";
						tdsummarytext.InnerHtml += "<input type=\"text\" size=\"75\" id=\"trackback\" name=\"trackback\" value=\"\" disabled/>";
						tdsummarytext.InnerHtml += "<br/><br/>";
						tdsummarytext.InnerHtml += "<input type=\"checkbox\" name=\"pingback\" id=\"pingback\" disabled/>&nbsp;PingBack URLs in this post";
					}
					tdsummarytext.InnerHtml += "		</td>";
					tdsummarytext.InnerHtml += "	</tr>";
					tdsummarytext.InnerHtml += "</table>";
				}
			}
			
			tdcommenthead.InnerHtml = m_refMsg.GetMessage("content HC label");
			tdcommenttext.InnerHtml = content_data_history.Comment;
		}
		
		private void ViewMetaData(ContentData data)
		{
			// Note: History for metadata-to-folder-assignment is not stored, and the
			// metadata array supplied to this function does not include anything useful
			// except for the TypeName property (string). We must compare this name to
			// all the names of the metadata currently assigned to the content-items
			// folder, showing only the matches and filtering out all of the rest:
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			int idx;
			Hashtable htValidMetadata;
			Collection cCustFieldCol;
			Collection cCustFieldItem;
			CustomFieldsApi custFlds;
			string sName;
			
			// folder
			try
			{
				// build a table of valid names:
				custFlds = new CustomFieldsApi();
				cCustFieldCol = custFlds.GetFieldsByFolder(data.FolderId, data.LanguageId);
				htValidMetadata = new Hashtable();
				foreach (Collection tempLoopVar_cCustFieldItem in cCustFieldCol)
				{
					cCustFieldItem = tempLoopVar_cCustFieldItem;
					htValidMetadata.Add(cCustFieldItem["CustomFieldName"], true);
				}
				
				// now display only those items that belong to the containing folder:
				result.Append("<table class=\"ektronForm\"><tbody>");
				
				if (!(data.MetaData == null))
				{
					for (idx = 0; idx <= data.MetaData.Length - 1; idx++)
					{
						sName = (string) (data.MetaData[idx].TypeName);
						if (htValidMetadata.ContainsKey(sName))
						{
							result.Append("<tr><td class=\"label\">" + sName + ":</td><td>&nbsp;&nbsp;");
							result.Append(data.MetaData[idx].Text + "</td></tr>");
						}
					}
				}
				else
				{
					result.Append("<tr><td>There is no metadata defined.</td><tr>");
				}
				result.Append("</tbody></table>");
				MetaDataValue.Text = result.ToString();
				
			}
			catch (Exception ex)
			{
				MetaDataValue.Text = ex.Message;
				
			}
			finally
			{
				result = null;
				custFlds = null;
				htValidMetadata = null;
				cCustFieldCol = null;
				cCustFieldItem = null;
			}
		}
		
		private void ViewContentProperties(ContentData data)
		{
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "NAME";
			colBound.ItemStyle.CssClass = "label";
			PropertiesGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			PropertiesGrid.Columns.Add(colBound);
			
			DataTable dt = new DataTable();
			DataRow dr;
			
			dt.Columns.Add(new DataColumn("NAME", typeof(string)));
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			
			int i = 0;
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content title label");
			dr[1] = data.Title;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content id label");
			dr[1] = data.Id;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content language label");
			dr[1] = data.LanguageId;
			dt.Rows.Add(dr);
			
			if (content_data.Type == 3333)
			{
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("lbl calatog entry sku") + "&nbsp;#:";
				dr[1] = entry_data.Sku;
				dt.Rows.Add(dr);
			}
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content status label");
			switch (data.Status.ToLower())
			{
				case "a":
					dr[1] = m_refMsg.GetMessage("status:Approved (Published)");
					break;
				case "o":
					dr[1] = m_refMsg.GetMessage("status:Checked Out");
					break;
				case "i":
					dr[1] = m_refMsg.GetMessage("status:Checked In");
					break;
				case "p":
					dr[1] = m_refMsg.GetMessage("status:Approved (PGLD)");
					break;
				case "m":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Deletion") + "</font>";
					break;
				case "s":
					dr[1] = "<font color=\"Red\">" + m_refMsg.GetMessage("status:Submitted for Approval") + "</font>";
					break;
				case "t":
					dr[1] = m_refMsg.GetMessage("status:Waiting Approval");
					break;
				case "d":
					dr[1] = "Deleted (Pending Start Date)";
					break;
			}
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content LUE label");
			dr[1] = data.EditorFirstName + " " + data.EditorLastName; //DisplayUserName
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content LED label");
			dr[1] = data.DisplayLastEditDate;
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic start date label");
			if (data.DisplayGoLive.Length == 0)
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			else
			{
				dr[1] = data.DisplayGoLive;
			}
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic end date label");
			if (data.DisplayEndDate == "")
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			else
			{
				dr[1] = data.DisplayEndDate;
			}
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("End Date Action Title");
			if (data.DisplayEndDate.Length > 0)
			{
				if (data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_archive_display)
				{
					dr[1] = m_refMsg.GetMessage("Archive display descrp");
				}
				else if (data.EndDateAction == Ektron.Cms.Common.EkConstants.EndDateActionType_refresh)
				{
					dr[1] = m_refMsg.GetMessage("Refresh descrp");
				}
				else
				{
					dr[1] = m_refMsg.GetMessage("Archive expire descrp");
				}
			}
			else
			{
				dr[1] = m_refMsg.GetMessage("none specified msg");
			}
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content DC label");
			dr[1] = data.DateCreated; //DisplayDateCreated
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = this.m_refMsg.GetMessage("lbl approval method");
			if (data.ApprovalMethod == 1)
			{
				dr[1] = m_refMsg.GetMessage("display for force all approvers");
			}
			else
			{
				dr[1] = m_refMsg.GetMessage("display for do not force all approvers");
			}
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("content approvals label");
			System.Text.StringBuilder approvallist = new System.Text.StringBuilder();
			ApprovalData[] approvaldata;
			approvaldata = m_refContentApi.GetCurrentApprovalInfoByID(ContentId);
			approvallist.Append(m_refMsg.GetMessage("none specified msg"));
			if (!(approvaldata == null))
			{
				if (approvaldata.Length > 0)
				{
					approvallist.Length = 0;
					for (i = 0; i <= approvaldata.Length - 1; i++)
					{
						if (approvaldata[i].Type == "user")
						{
							approvallist.Append("<img src=\"" + imagePath + "user.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user") + "\" title=\"" + m_refMsg.GetMessage("approver is user") + "\">");
						}
						else
						{
							approvallist.Append("<img src=\"" + imagePath + "user.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user group") + "\" title=\"" + m_refMsg.GetMessage("approver is user group") + "\">");
						}
						if (approvaldata[i].IsCurrentApprover)
						{
							approvallist.Append("<span class=\"important\">");
						}
						else
						{
							approvallist.Append("<span>");
						}
						approvallist.Append(approvaldata[i].DisplayUserName + "</span>");
					}
				}
			}
			dr[1] = approvallist.ToString();
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("xml configuration label");
			if (data.IsXmlInherited != false)
			{
				if (!(data.XmlConfiguration == null))
				{
					dr[1] = "&nbsp;" + data.XmlConfiguration.Title;
				}
				else
				{
					dr[1] = m_refMsg.GetMessage("none specified msg") + " " + m_refMsg.GetMessage("html content assumed");
				}
			}
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic template label");
			dr[1] = "";
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic Path");
			dr[1] = m_refContentApi.EkContentRef.GetFolderPath(data.FolderId);
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = "Content Searchable:";
			dr[1] = data.IsSearchable;
			
			DataView dv = new DataView(dt);
			PropertiesGrid.ShowHeader = false;
			PropertiesGrid.DataSource = dv;
			PropertiesGrid.DataBind();
		}
		
		#endregion
		
		#region Helpers
		
		protected void PropertiesGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			switch (e.Item.ItemType)
			{
				case ListItemType.AlternatingItem:
				case ListItemType.Item:
					if (e.Item.Cells[0].Text.Equals(m_refMsg.GetMessage("properties text")) && e.Item.Cells[1].Text.Equals("REMOVEDITEM"))
					{
						e.Item.Cells[0].Attributes.Add("align", "Left");
						e.Item.Cells[0].ColumnSpan = 2;
						e.Item.Cells[0].CssClass = "title-header";
						e.Item.Cells.RemoveAt(1);
					}
					break;
			}
		}
		private void ShowError(string ex)
		{
			
			Response.Redirect((string) ("reterror.aspx?info=" + EkFunctions.UrlEncode(ex)), false);
			
		}
		
		public string FixContentHistory(ContentData myContentData, string curSnippet)
		{
			Regex regExp;
			regExp = new Regex(" ");
			foreach (string strLineVal in regExp.Split(curSnippet))
			{
				regExp = new Regex("=");
				string[] strKeyValues;
				strKeyValues = regExp.Split(strLineVal);
				if ((string) (strKeyValues[0].Trim()) == "src")
				{
					string curStringVal = strKeyValues[1];
					if (curStringVal.ToLower().IndexOf("/assets") >= 0)
					{
						curStringVal = (string) (curStringVal.ToLower().Replace("/assets/" + myContentData.AssetData.Id.ToLower() + myContentData.AssetData.Version.Substring(myContentData.AssetData.Version.IndexOf(".")).ToLower(), "/assetmanagement/DownloadAsset.aspx?history=true&ID=" + myContentData.AssetData.Id + "&version=" + myContentData.AssetData.Version));
						curSnippet = Regex.Replace(curSnippet, strKeyValues[1], curStringVal);
					}
				}
			}
			return curSnippet;
		}
		
		#endregion
		
		#region CSS, JS
		
		private void RegisterJS()
		{
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/searchfuncsupport.js", "EktronSearchFuncSupportJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/dhtml/tableutil.js", "EktronTableUtilJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/metadata_selectlist.js", "EktronMetadataSelectListJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/commerce/com.ektron.commerce.mediatab.js", "EktronCommerceMediaTabJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/commerce/com.Ektron.Commerce.Pricing.js", "EktronCommercePricingJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronDnRJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			
		}
		
		private void RegisterCSS()
		{
			
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/tables/tableutil.css", "EktronTableUtilCss");
			Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/commerce/MediaTab.css", "EktronMediaTabCss");
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
			Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/commerce/Ektron.Commerce.Pricing.css", "EktronCommercePricingCss");
			
			
		}
		
		#endregion
		
	}
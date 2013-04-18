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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using System.IO;



	public partial class LocalCopyAsset : System.Web.UI.Page
	{
		
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Ektron.Cms.ContentAPI content_api = new Ektron.Cms.ContentAPI();
				Ektron.Cms.Content.EkContent m_refContent = content_api.EkContentRef;
				Ektron.Cms.ContentData content_data = null;
				long content_id = 0;
				bool valid_attempt = false;
				int ContentLanguage;
				CommonApi AppUI = new CommonApi();
				
				if (! (Request.QueryString["id"] == null))
				{
					content_id = Convert.ToInt64(Request.QueryString["id"]);
					
					if ((!(Request.QueryString["content_language"] == null)) && (int.TryParse(Request.QueryString["content_language"], out ContentLanguage)) && (ContentLanguage > 0))
					{
						if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
						{
							ContentLanguage = AppUI.DefaultContentLanguage;
						}
						AppUI.ContentLanguage = ContentLanguage;
						content_api.ContentLanguage = ContentLanguage;
					}
					else
					{
						ContentLanguage = AppUI.DefaultContentLanguage;
					}
					
					//if checkout was clicked
					bool checkOutAsset;
					if ((! (Request.QueryString["checkout"] == null)) && (bool.TryParse(Request.QueryString["checkout"], out checkOutAsset)) && (checkOutAsset))
					{
						ContentData contData = content_api.GetContentById(content_id, ContentAPI.ContentResultType.Staged);
						//If checkout and save was clicked on a content in the "S" state, checkout will throw an exception
						//Take ownership of content before checkout
						if ((contData != null) && (contData.UserId != content_api.RequestInformationRef.UserId) && (contData.Status == "S"))
						{
							content_api.EkContentRef.TakeOwnership(content_id);
						}
						content_api.EkContentRef.CheckContentOutv2_0(content_id);
					}
					//
					if (content_id > 0)
					{
						content_data = content_api.GetContentById(content_id, 0);
					}
					if ((content_data != null) && (content_data.AssetData != null) && (content_data.AssetData.Version.Length > 0))
					{
						//GetContentById returns content in default language if no content exists for ContentLanguage
						if (content_data.LanguageId == ContentLanguage)
						{
							AssetManagement.AssetManagementService objAssetMgtService;
							objAssetMgtService = new AssetManagement.AssetManagementService();
							string filepath = objAssetMgtService.GetViewUI(content_data.AssetData.Id, Ektron.ASM.AssetConfig.InstanceType.current, content_data.AssetData.Version,Ektron.ASM.AssetConfig.UIType.url);
							if ((Request.QueryString["originalimage"] != null)&& Request.QueryString["originalimage"] == "true")
							{
								string _path = (string) (content_api.EkContentRef.GetViewUrl(Convert.ToInt32(content_data.Type), content_data.AssetData.Id).Replace(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host, ""));
								if (_path.StartsWith(":")) //If https the assetmanagementservice tends to send the port number too
								{
									_path = _path.Substring(_path.IndexOf("/"));
								}
								filepath = Page.Server.MapPath(_path);
								//filepath = filepath.Replace(content_data.AssetData.Id, (string) ("orig_" + content_data.AssetData.Id));
								if (filepath != null)
								{
									if (File.Exists(filepath))
									{
										valid_attempt = true;
										AssetManagement.AssetManagementService assetmanagementService = new AssetManagement.AssetManagementService();
										Ektron.ASM.AssetConfig.AssetData assetData = assetmanagementService.GetAssetData(content_data.AssetData.Id);
										Response.Clear();
										//Response.ContentType = "application/octet-stream"
										Response.ContentType = content_data.AssetData.MimeType;
										Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlPathEncode(assetData.Handle) + "\"");
										Response.WriteFile(filepath);
										Response.Flush();
										try
										{
											Response.End();
										}
										catch
										{
											
										}
										return;
									}
								}
							}
							
							if (filepath.IndexOf("?") >= 0)
							{
								filepath = filepath + "&mimeType=octet";
							}
							else
							{
								filepath = filepath + "?mimeType=octet";
							}
							Response.Redirect(filepath);
						}
					}
					
					if (! valid_attempt)
					{
						notification_message.Text = "File does not exist or you do not have permission to view this file";
					}
				}
				
			}
			catch (System.Threading.ThreadAbortException)
			{
				notification_message.Text = "";
			}
			catch (Exception)
			{
				notification_message.Text = "File does not exist or you do not have permission to view this file";
			}
			
		}
	}
	


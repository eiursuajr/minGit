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
using System.IO;
using System.Net;
using Ektron.Cms;
using Ektron.ASM.AssetConfig;

	public partial class DownloadAsset : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Ektron.Cms.ContentAPI content_api = new Ektron.Cms.ContentAPI();
			try
			{
				Ektron.Cms.ContentData content_data = null;
				long asset_id = 0;
				bool valid_attempt = false;
				int LangbackUp = 0;
				if (! (Request.QueryString["id"] == null))
				{
					asset_id = Convert.ToInt64(Request.QueryString["id"]);
				}
				LangbackUp = content_api.ContentLanguage;
				if (Request.QueryString["LangType"] != null && content_api.ContentLanguage == -1)
				{
					content_api.ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
				}
				if (content_api.ContentLanguage == -1)
				{
					content_api.ContentLanguage = int.Parse(content_api.GetCookieValue("SiteLanguage"));
				}
				long iTmpCaller = content_api.RequestInformationRef.CallerId;
				if (asset_id > 0)
				{
					content_api.RequestInformationRef.CallerId = Ektron.Cms.Common.EkConstants.InternalAdmin;
					content_api.RequestInformationRef.UserId = Ektron.Cms.Common.EkConstants.InternalAdmin;
					try
					{
						content_data = content_api.GetContentById(asset_id, 0);
					}
					catch
					{
					}
					finally
					{
						content_api.RequestInformationRef.CallerId = iTmpCaller;
						content_api.RequestInformationRef.UserId = iTmpCaller;
					}
					if (content_data != null)
					{
						content_api.ContentLanguage = content_data.LanguageId;
						content_data = null;
					}
					content_data = content_api.ShowContentById(asset_id, content_api.CmsPreview, System.Convert.ToBoolean(! content_api.CmsPreview));
					content_api.ContentLanguage = LangbackUp;
				}
				if ((content_data != null) && (content_data.AssetData != null) && (content_data.AssetData.Version.Length > 0))
				{
					string filepath = Page.Server.MapPath((string) (content_api.EkContentRef.GetViewUrl(Convert.ToInt32(content_data.Type), content_data.AssetData.Id).Replace(Page.Request.Url.Scheme + "://" + Page.Request.Url.Authority, "").Replace(":443", "").Replace(":80", "")));
					if (filepath != null)
					{
						if (File.Exists(filepath))
						{
							valid_attempt = true;
							string filename = Path.GetFileName(filepath);
							string ext = "";
							ext = Path.GetExtension(filepath);
							AssetData _assetData = new AssetData();
							_assetData.AssetDataFromAssetID(content_data.AssetData.Id);
							if (ext.Contains("pdf") || ext.Contains("pps"))
							{
								WebClient client = new WebClient();
								byte[] Buffer = client.DownloadData(Convert.ToString(filepath));
								if (Buffer.Length > 0)
								{
									valid_attempt = true;
									Response.Clear();
									Response.ContentType = (string) ((ext.Contains("pdf")) ? "application/pdf" : "application/vnd.ms-powerpoint");
                                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + (Request.Browser.Browser == "IE" ? (Server.UrlPathEncode(System.IO.Path.GetFileNameWithoutExtension(_assetData.Handle))) : (System.IO.Path.GetFileNameWithoutExtension(_assetData.Handle))) + ext + "\"");
                                    Response.AddHeader("Content-Length", Buffer.Length.ToString());
									Response.BinaryWrite(Buffer);
								}
							}
							else
							{
                                //if (ext.Contains("txt") || ext.Contains("nxb"))
                                //{
                                //    filepath = DocumentManagerData.Instance.StorageLocation + _assetData.Storage + ConfigManager.pathChar + _assetData.Name;
                                //}
								Response.Clear();
								Response.ContentType = content_data.AssetData.MimeType;
								Response.AddHeader("Content-Disposition", "attachment; filename=\"" + (Request.Browser.Browser == "IE" ? (Server.UrlPathEncode(_assetData.Handle)) : _assetData.Handle) + "\"");
                                try
                                {
                                    FileInfo fi = new FileInfo(filepath);
                                    Response.AddHeader("Content-Length", fi.Length.ToString());
                                }
                                catch { }
								Response.WriteFile(filepath);
							}
							Response.Flush();
							try
							{
								Response.End();
							}
							catch
							{
							}
						}
					}
				}
				if (! valid_attempt)
				{
					notification_message.Text = "File does not exist or you do not have permission to view this file";
					notification_message.ToolTip = "Error Message - " + notification_message.Text;
					// Register CSS
					Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
					Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
                    Login.Visible = System.Convert.ToBoolean(!content_api.IsLoggedIn);
					content_api.RequestInformationRef.RedirectFromLoginKeyName = Request.Url.PathAndQuery.ToString();
					Login.RedirectFromLoginPage();
					Login.Fill();
				}
				
			}
			catch (Exception)
			{
				notification_message.Text = "File does not exist or you do not have permission to view this file";
				// Register CSS
				Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
				Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
                Login.Visible = System.Convert.ToBoolean(!content_api.IsLoggedIn);
				content_api.RequestInformationRef.RedirectFromLoginKeyName = Request.Url.PathAndQuery.ToString();
				Login.RedirectFromLoginPage();
				Login.Fill();
			}
			
		}
	}
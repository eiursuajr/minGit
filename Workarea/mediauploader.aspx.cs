using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Web.Caching;
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
using Ektron.Cms.Common;



	public partial class mediauploader : System.Web.UI.Page
	{
		
		
		protected string action = "";
		protected string sEditor = "";
		protected StyleHelper m_refStyle = new StyleHelper();
		protected string AppImgPath = "";
		protected string AppeWebPath = "";
		protected EkMessageHelper m_refMsg;
		protected string scope = "";
		protected string operation = "";
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected long currentUserID = 0;
		protected LibraryConfigData lib_setting_data;
		protected long m_folder = 0;
		protected string m_LibType = "";
		protected PermissionData m_cPerms;
		protected SiteAPI m_refSiteApi;
		protected bool IsMac = false;
		private string content_teaser = "";
		private int m_intContentLanguage;
		private const string MetadataDelimiter = "@@ekt@@";
		protected string sContentEditor = "";
		protected string eWebEditProJS = "";
		protected string SitePath = "";
		protected string sLinkText = "";
		protected bool IsMembershipUser = false;
		protected int DEntryLink;
		protected Ektron.ContentDesignerWithValidator ctlEditor;
		protected CommonApi m_refCommonApi = new CommonApi();
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
            ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("controls/Editor/ContentDesignerWithValidator.ascx");
			ctlEditor.ID = "content_teaser";
			ctlEditor.AllowScripts = true;
			ctlEditor.Height = new Unit(200, UnitType.Pixel);
			ctlEditor.Width = new Unit(100, UnitType.Percentage);
			ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
			ctlEditor.AllowFonts = true;
			ctlEditor.ShowHtmlMode = false;
			EditSummaryHtml.Controls.Add(ctlEditor);
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			
			try
			{
				Response.CacheControl = "no-cache";
				Response.AddHeader("Pragma", "no-cache");
				Response.Expires = -1;
				m_refMsg = m_refContentApi.EkMsgRef;
				RegisterResources();
				IsMembershipUser = System.Convert.ToBoolean(m_refContentApi.RequestInformationRef.IsMembershipUser == 1);
				
				m_refSiteApi = new SiteAPI();
				SitePath = m_refContentApi.SitePath;
				if (Request.QueryString["selected"]!= null && Request.QueryString["selected"] != "")
				{
					sLinkText = Request.QueryString["selected"];
				}
				if (!(Request.QueryString["LangType"] == null))
				{
					if (Request.QueryString["LangType"] != "")
					{
						m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
						if (1 == m_intContentLanguage)
						{
							m_intContentLanguage = m_refContentApi.DefaultContentLanguage;
							m_refContentApi.ContentLanguage = m_intContentLanguage;
						}
						m_refContentApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage.ToString());
					}
					else
					{
						if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
						{
							m_intContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
							if (1 == m_intContentLanguage)
							{
								m_intContentLanguage = m_refContentApi.DefaultContentLanguage;
								m_refContentApi.ContentLanguage = m_intContentLanguage;
							}
						}
					}
				}
				else
				{
					if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						m_intContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
						if (1 == m_intContentLanguage)
						{
							m_intContentLanguage = m_refContentApi.DefaultContentLanguage;
							m_refContentApi.ContentLanguage = m_intContentLanguage;
						}
					}
				}
				if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || m_intContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
				{
					m_intContentLanguage = m_refContentApi.DefaultContentLanguage;
				}
				if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
				{
					m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
				}
				else
				{
					m_refContentApi.ContentLanguage = m_intContentLanguage;
				}
				
				AppeWebPath = m_refContentApi.ApplicationPath + m_refContentApi.AppeWebPath;
                int.TryParse(Request.QueryString["dentrylink"], out DEntryLink);
				action = Request.QueryString["Action"];
				sEditor = Request.QueryString["EditorName"];
				if ((Request.QueryString["folder"] != null)&& Request.QueryString["folder"] != "")
				{
                    m_folder = Convert.ToInt64(Request.QueryString["folder"].TrimEnd(','));
				}
				else
				{
					m_folder = 0;
				}
				
				m_LibType = Request.QueryString["type"];
				scope = Request.QueryString["scope"];
				currentUserID = m_refContentApi.UserId;
				AppImgPath = m_refContentApi.AppImgPath;
				
				jsEditor.Text = sEditor;
				jsLibType.Text = m_LibType;
				jsFolder.Text = m_folder.ToString();
				qsRetfield.Text = Request.QueryString["retfield"];
				jsSitePath.Text = SitePath;
				jsOverwriteItemDenied.Text = m_refMsg.GetMessage("js: alert overwrite item denied");
				jsChooseItemToOverwrite.Text = m_refMsg.GetMessage("js: alert choose item to overwrite ed lib");
				jsChooseImgFileToOverwrite.Text = m_refMsg.GetMessage("js: alert choose image-file to use overwrite") + "\\n\\n" + m_refMsg.GetMessage("js: chosen lib type msg");
				jsCannotOverwrite.Text = m_refMsg.GetMessage("js: alert cannot change overwrite title");
				jsSelectLocalFile.Text = m_refMsg.GetMessage("js: alert select local file");
				jsWarnOverwrite.Text = m_refMsg.GetMessage("js: warning about to overwrite ed lib");
				jsSelectFolder.Text = m_refMsg.GetMessage("js: alert select folder");
				jsEditorClosed2.Text = m_refMsg.GetMessage("js: alert editor closed2");
				jsLibTitleReq.Text = m_refMsg.GetMessage("js: alert title required (library)");
				jsFilenameReq.Text = m_refMsg.GetMessage("js: alert filename is required");
				jsUploadImgWrong.Text = m_refMsg.GetMessage("js: confirm upload image wrong");
				jsUploadFileWrong.Text = m_refMsg.GetMessage("js: confirm upload file wrong");
				jsUploadCorrectFolder.Text = m_refMsg.GetMessage("js: confirm upload correct folder");
				jsUploadImgDenied.Text = m_refMsg.GetMessage("js: alert upload image denied");
				jsUploadFileDenied.Text = m_refMsg.GetMessage("js: alert upload file denied");
				jsUploadImgFileDenied.Text = m_refMsg.GetMessage("js: alert upload image/file denied");
				jsInvalidExt.Text = m_refMsg.GetMessage("js: alert invalid extension");
				jsForImg.Text = m_refMsg.GetMessage("js: alert for images");
				jsForFiles.Text = m_refMsg.GetMessage("js: alert for files");
				jsErrExtOverwrite.Text = m_refMsg.GetMessage("js: same extension for overwrite error") + "\\n\\n" + m_refMsg.GetMessage("js: local file ext prompt");
				jsLibFileExt.Text = m_refMsg.GetMessage("js: library file ext prompt");
				jsNoLocalPreview.Text = m_refMsg.GetMessage("js: alert no local preview");
				jsMakeSelection.Text = m_refMsg.GetMessage("js: alert make selection");
				jsStyleSheet.Text = m_refStyle.GetClientScript();
				jsLinkText.Text = sLinkText != "" ? (" + \'&selected=" + sLinkText.Replace("\'", "\\\'") + "\'") : "";
				
             				
				lib_setting_data = m_refContentApi.GetLibrarySettings(Convert.ToInt64(m_folder)); //Used in the scripting layer
				jsImageExtensions.Text = lib_setting_data.ImageExtensions;
				jsFileExtensions.Text = lib_setting_data.FileExtensions;

                //Adding the MediaUploaderCommon User Control
                MediaUploaderCommon m_Moc;
                m_Moc = (MediaUploaderCommon)(LoadControl("controls/library/MediaUploaderCommon.ascx"));
                m_Moc.ID = "MediaUploaderCommon1";
                DataHolder.Controls.Add(m_Moc);


				if ((m_LibType == null) || (m_LibType == ""))
				{
					m_LibType = "images";
				}
				if ((m_folder.ToString() == ""))
				{
					m_folder = 0;
				}
				m_cPerms = m_refContentApi.LoadPermissions(m_folder, "folder", 0);
				
				if (Request.Browser.Platform.IndexOf("Win") == -1)
				{
					IsMac = true;
				}
				
				if (Page.IsPostBack)
				{
					Process_Form();
				}
				else
				{
					if (sEditor != "JSEditor")
					{
						ctlEditor.FolderId = m_folder;
						ctlEditor.Content = content_teaser;
						Display_CustomSearchMetaData();
					}
					UploaderToolBar();
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
			finally
			{
				m_refContentApi = null;
				m_refSiteApi = null;
			}
		}
		
		private string GetFormTeaserData()
		{
			string returnValue;
			string retVal = "";
			try
			{
				retVal = (string) ctlEditor.Content;
			}
			catch (Exception)
			{
				// no teaser
			}
			finally
			{
				returnValue = retVal;
			}
			return returnValue;
		}
		
		private void Display_CustomSearchMetaData()
		{
			if (m_LibType == "quicklinks" || m_LibType == "forms")
			{
				return;
			}
			ContentMetaData[] meta_data;
			meta_data = m_refContentApi.GetMetaDataTypes("id");
			if (meta_data != null)
			{
				if (meta_data.Length > 0)
				{
                    int count=0;
                    litCustomMeta.Text = (CustomFields.WriteFilteredMetadataForEdit(meta_data, true, "", m_folder,ref count, null)).ToString();
				}
			}
		}
		private void Process_Form()
		{
			string filename = "";
			object BinaryFormData;
			Collection cLibrary = new Collection();
			string lib_filename = "";
			bool UploadOk = false;
			int iLoop = 0;
			string szPhysicalPath = "";
			LoadBalanceData[] extrPaths;
			string libt2 = "";
			string tmpfolder;
			string tmptype;
			Array filenamearray;
			string extensions;
			string MediaPath = "";
			Array ExtensionArray;
			string actErrorString;
			string ErrDescription;
			
			Ektron.Cms.DataIO.EkLibraryRW dataLibObj;
			dataLibObj = new Ektron.Cms.DataIO.EkContentRW(m_refContentApi.RequestInformationRef);
			Collection cItemInfo = new Collection();
			
			if (action == "overwritelibraryitem")
			{
				operation = "overwrite";
			}
			else
			{
				operation = "makeunique";
			}
			
			try
			{
                Information.Err().Clear();
				BinaryFormData = Request.BinaryRead(Request.TotalBytes);
			}
			catch 
			{
                if (Information.Err().Number != 0)
				{
                    if (-2147467259 == Information.Err().Number)
					{
						ErrDescription = "Error: The file being upload is larger than what is allowed in the IIS. ";
						ErrDescription = ErrDescription + "Please change the ASPMaxRequestEntityAllowed to a larger number in the metabase.xml file (usually located in c:\\windows\\system32\\inetsrv). ";
						Response.Write(ErrDescription + '\r' + '\n' + "<br/>");
					}
                    Response.Write(Information.Err().Description);
				}
				return;
			}
			
			cLibrary.Add(m_folder, "ParentID", null, null);
			m_LibType = Request.Form["frm_libtype"];
			cLibrary.Add(m_LibType, "LibraryType", null, null);
			cLibrary.Add(Request.Form["frm_title"], "LibraryTitle", null, null);
			filename = (string) (frm_filename.PostedFile.FileName.Substring((frm_filename.PostedFile.FileName).LastIndexOf("\\") + 1));

            int fileLength = frm_filename.PostedFile.ContentLength;
            byte[] fileData = new byte[fileLength];
            frm_filename.PostedFile.InputStream.Read(fileData, 0, fileLength);
            Stream stream = new MemoryStream(fileData);
            if (EkFunctions.IsImage(Path.GetExtension(frm_filename.PostedFile.FileName)))
            {
                if (!EkFunctions.isImageStreamValid(stream))
                {
                    throw new Exception("The image is corrupted or not in correct format."); 
                }
            }


			cLibrary.Add(Request.Form["frm_content_id"], "ContentID", null, null);
			if (operation == "overwrite")
			{
				cLibrary.Add(Request.Form["frm_library_id"], "LibraryID", null, null);
				if (Request.Form["hidden_filename"].IndexOf("?") != -1)
				{
					lib_filename = (string) (Request.Form["hidden_filename"].Substring(0, System.Convert.ToInt32(Request.Form["hidden_filename"].IndexOf("?"))));
				}
				else
				{
					lib_filename = Request.Form["hidden_filename"];
				}
				
				
			}
			
			if (((operation.ToLower() != "overwrite") && 
                (((m_cPerms.CanAddToImageLib) && (cLibrary["LibraryType"].ToString() == "images"))
                || ((m_cPerms.CanAddToFileLib) && (cLibrary["LibraryType"].ToString() == "files")) 
                || ((m_cPerms.CanAddToHyperlinkLib) && (cLibrary["LibraryType"].ToString() == "hyperlinks")) 
                || ((m_cPerms.CanAddToQuicklinkLib) && (cLibrary["LibraryType"].ToString() == "quicklinks")))) 
                || ((m_cPerms.CanOverwriteLib) && (operation.ToLower() == "overwrite")))
				{
				cLibrary.Add(currentUserID, "UserID", null, null);
				
				tmpfolder = cLibrary["ParentID"].ToString();
				tmptype = cLibrary["LibraryType"].ToString();
				filenamearray = filename.Split('.');
				if (tmptype == "images")
				{
					extensions = lib_setting_data.ImageExtensions;
				}
				else
				{
					extensions = lib_setting_data.FileExtensions;
				}
				
				if ((filenamearray.Length - 1) >= 1)
				{
					ExtensionArray = extensions.Split(',');
                  
					for (iLoop = 0; iLoop <= (ExtensionArray.Length - 1); iLoop++)
					{
						if (filenamearray.GetValue(filenamearray.Length-1).ToString().ToLower().Trim() == ExtensionArray.GetValue(iLoop).ToString().ToLower().Trim())
                        {
                            UploadOk = true;
                            break;
                        }
					}
					if (UploadOk == true)
					{
						if (tmptype == "images")
						{
							MediaPath = lib_setting_data.ImageDirectory;
						}
						else if (tmptype == "files")
						{
							MediaPath = lib_setting_data.FileDirectory;
						}
						szPhysicalPath = getPhysicalPath(MediaPath);
						if (! Directory.Exists(szPhysicalPath))
						{
							Directory.CreateDirectory(szPhysicalPath);
						}
						
						actErrorString = filename;
						if (lib_filename.Trim().Length > 0)
						{
							actErrorString = lib_filename;
						}
						string[] strTmpFilename;
						int iUnqueNameIdentifier = 0;
						FileInfo CheckFile;
						actErrorString = actErrorString.Replace("/", "\\");
						strTmpFilename = actErrorString.Split('\\');
						if ((szPhysicalPath.Substring(szPhysicalPath.Length - 1, 1) != "\\"))
						{
							szPhysicalPath = szPhysicalPath + "\\";
						}
                        string strFileName = "";
                        string strFileExtn = "";
                        actErrorString = (string)(strTmpFilename.GetValue(strTmpFilename.Length - 1));
                        strFileExtn = actErrorString.Substring(actErrorString.LastIndexOf("."));
                        strFileName = actErrorString.Replace(strFileExtn, "");
                        if (operation == "makeunique")
						{
							CheckFile = new FileInfo(szPhysicalPath + actErrorString);
							if (CheckFile.Exists)
							{
								while (CheckFile.Exists)
								{
									iUnqueNameIdentifier++;
                                    actErrorString = (string)(strFileName + "(" + iUnqueNameIdentifier + ")" + strFileExtn);
									CheckFile = new FileInfo(szPhysicalPath + actErrorString);
								}
							}
						}
						
						try
						{
							if (operation.ToLower() != "overwrite" && cLibrary["LibraryType"].ToString()  == "images")
							{
                                cItemInfo = dataLibObj.GetChildLibraryItemByTitlev2_0(Request.Form["frm_title"].ToString(),m_folder,cLibrary["LibraryType"].ToString(),Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSContentType.LibraryItem));
								if ((cItemInfo.Count > 0) && (Convert.ToInt32(cItemInfo["ContentLanguage"]) == m_refContentApi.RequestInformationRef.ContentLanguage))
								{
									Utilities.ShowError(m_refMsg.GetMessage("com: library entry already exists"));
									return;
								}
							}
						}
						catch (Exception)
						{
							
						}
						
						try
						{
							frm_filename.PostedFile.SaveAs(szPhysicalPath + actErrorString);
						}
						catch (Exception ex)
						{
							Response.Write("<html><head><link rel=\"STYLESHEET\" href=\"csslib/ektron.workarea.css\" type=\"text/css\"></head><body>");
							Response.Write("<table><tr><td class=\"titlebar-error\">" + ex.Message + "<td></tr></table></body></html>");
							Response.End();
						}
						if (tmptype == "images")
						{
							// Thumbnails only run if it is an image.
							string thumbnailPath;
							thumbnailPath = getPhysicalPath(MediaPath);
							Utilities.ProcessThumbnail(thumbnailPath, actErrorString);
						}
						//----------------- Load Balance ------------------------------------------------------
						if (tmptype == "images")
						{
							libt2 = "images";
						}
						else
						{
							libt2 = "files";
						}
						
						extrPaths = m_refContentApi.GetAllLoadBalancePathsExtn(m_folder, libt2);
						if (!(extrPaths == null))
						{
							for (iLoop = 0; iLoop <= extrPaths.Length - 1; iLoop++) //Each lbObj In extrPaths
							{
								//actErrorString will contain the name of the file that was loaded on the main directory
								szPhysicalPath = getPhysicalPath(extrPaths[iLoop].Path);
								if ((szPhysicalPath.Substring(szPhysicalPath.Length - 1, 1) != "\\"))
								{
									szPhysicalPath = szPhysicalPath + "\\";
								}
								try
								{
									frm_filename.PostedFile.SaveAs(szPhysicalPath + actErrorString);
									Utilities.ProcessThumbnail(szPhysicalPath, actErrorString);
								}
								catch (Exception)
								{
									
								}
							}
						}
						extrPaths = null;
						
						//----------------------- /Load Balance --------------------------------------------------------
						
						cLibrary.Add(MediaPath + actErrorString, "LibraryFilename", null, null);
						
					}
				}
			}
			else
			{
				EkException.ThrowException(new System.Exception(m_refMsg.GetMessage("com: user does not have permission")));
			}
			
			if (m_LibType != "forms" && m_LibType != "quicklinks")
			{
				cLibrary.Add(GetFormTeaserData(), "ContentTeaser", null, null);
				cLibrary.Add(CollectMetaField(), "ContentMetadata", null, null);
				cLibrary.Add("", "Image", null, null);
			}
			
			if (operation == "overwrite")
			{
				m_refContentApi.UpdateLibraryItemByID(cLibrary);
			}
			else
			{
				Ektron.Cms.Library.EkLibrary m_reflib;
				m_reflib = m_refContentApi.EkLibraryRef;
				bool ret;
				ret = m_reflib.AddLibraryItemv2_0(cLibrary, 0);
			}
			string retField = "";
			retField = Request.QueryString["retfield"];
			
			FolderData f_data = new FolderData();
			f_data = m_refContentApi.GetFolderById(m_folder);
            System.Random rand = new System.Random();
            string file_name = Strings.Replace(cLibrary["LibraryFilename"].ToString(), "\\\\", "/", 1, -1, 0);
            if ("overwrite" == operation && "ContentDesigner" == sEditor)
            {
                file_name += "?n=" + rand.Next(0, 10000);
            }
            //if ((f_data.IsDomainFolder) || (f_data.DomainProduction != "" || f_data.DomainStaging != ""))
            //{
            //    file_name = Strings.Replace(file_name, m_refContentApi.SitePath, "", 1, 1, 0);
            //    if (m_refCommonApi.RequestInformationRef.IsStaging)
            //    {
            //        file_name = (string) ("http://" + f_data.DomainStaging + "/" + file_name);
            //    }
            //    else
            //    {
            //        file_name = (string) ("http://" + f_data.DomainProduction + "/" + file_name);
            //    }
				
            //}
			string InsertFn = "";
            if ((retField != null)&& retField != "")
			{
				InsertFn = "InserValueToField(\"" + file_name.Replace("\\\\", "/") + "\",\"" + Ektron.Cms.Common.EkFunctions.GetThumbnailForContent(file_name) + "\",\"" + SitePath + "\", \"" + retField + "\");";
			}
			else
			{
				if (sEditor == "JSEditor")
				{
					if (cLibrary["LibraryType"].ToString()  == "images")
					{
                        InsertFn = "window.opener.JSEIMGInsert(escape(\'" + file_name + "\'),\'" + Strings.Replace(cLibrary["LibraryTitle"].ToString(), "\'", "\\\'", 1, -1, 0) + "\');self.close();";
					}
                    else if (cLibrary["LibraryType"].ToString() == "files")
					{
                        InsertFn = "window.opener.JSEURLInsert(escape(\'" + file_name + "\'),\'" + Strings.Replace((string)(sLinkText != "" ? sLinkText : (cLibrary["LibraryTitle"])), "\'", "\\\'", 1, -1, 0) + "\');self.close();";
					}
				}
				else
				{
					InsertFn = "InsertFunction(\"" + file_name + "\", \"" + cLibrary["LibraryTitle"] + "\", \"" + cLibrary["LibraryType"] + "\");self.close();";
				}
			}
			JSInsertFn.Text = "<script language=\"javascript\">" + InsertFn + "</script>";
		}
		private void UploaderToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("upload/insert local file msg"));
			result.Append("<table><tr>");

			if (Request.Browser.Browser.ToLower().Contains("ie"))
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/ui/icons/back.png", Request.UrlReferrer.PathAndQuery, m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			}

			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_addlibrary-nm.gif", "#", m_refMsg.GetMessage("upload and insert msg"), m_refMsg.GetMessage("btn add library"), "OnClick=\"javascript:SubmitForm(\'LibraryItem\', \'EditorInsert(\\\'" + scope + "\\\', 0)\', true);return false\"", StyleHelper.AddButtonCssClass, true));

			if (Request.QueryString["retfield"] != "" && Request.QueryString["retfield"] != null)
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/magnifier.png", (string)("isearch.aspx?action=showLibdlg&folderid=" + m_folder + "&retfield=" + Request.QueryString["retfield"] + "&dentrylink=" + DEntryLink + "&source=edit&EditorName=" + Request.QueryString["EditorName"] + "&scope=" + Request.QueryString["scope"]), "Search", m_refMsg.GetMessage("btn search"), "", StyleHelper.SearchButtonCssClass, true));
			}
			else
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/magnifier.png", (string)("isearch.aspx?action=showLibdlg&folderid=" + m_folder + "&dentrylink=" + DEntryLink + "&source=edit&EditorName=" + Request.QueryString["EditorName"] + "&scope=" + Request.QueryString["scope"]), "Search", m_refMsg.GetMessage("btn search"), "", StyleHelper.SearchButtonCssClass, false));
			}
			
			//result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath & "images/UI/Icons/magnifier.png", "isearch.aspx?action=showLibdlg&folderid=0&source=mediainsert", "Search", m_refMsg.GetMessage("btn search"), ""))

			if (sEditor != "JSEditor")
			{
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "btn_overwrite-nm.gif", "#", m_refMsg.GetMessage("alt overwrite button in ed lib"), m_refMsg.GetMessage("btn overwrite"), "Onclick=\"javascript:return SubmitForm(\'LibraryItem\', \'EditorInsert(\\\'" + scope + "\\\', 1)\', true);\"", StyleHelper.OverwriteButtonCssClass));
			}
			//result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_preview-nm.gif", "#", m_refMsg.GetMessage("alt preview button text (library)"), m_refMsg.GetMessage("btn preview"), "onClick=""javascript:previewImage(" & scope & ");return false;"""))
			// Un-comment the next line to provide support for deffered uploading:
			//result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "btn_insert-nm.gif", "#", "alt insert button text", m_refMsg.GetMessage("btn insert"), "onClick=""javascript:InsertDeferredUpload(" & scope & ");return false;"""))

			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		//LEGACY SUPPORT
		protected string gtMess(string str)
		{
			return (m_refMsg.GetMessage(str));
		}
		private string getPhysicalPath(string path)
		{
			return (Server.MapPath(path));
		}
		private Collection CollectMetaField()
		{
			object[] acMetaInfo = new object[4];
			string MetaSelect = "";
			string MetaSeparator = "";
			string MetaTextString = "";
			int ValidCounter = 0;
			if (Request.Form["frm_validcounter"] != "")
			{
				ValidCounter = System.Convert.ToInt32(Request.Form["frm_validcounter"]);
				
			}
			Collection page_meta_data = new Collection();
			int i;
			for (i = 1; i <= ValidCounter; i++)
			{
				acMetaInfo[1] = Request.Form["frm_meta_type_id_" + i];
				acMetaInfo[2] = Request.Form["content_id"];
				MetaSeparator = Request.Form["MetaSeparator_" + i];
				MetaSelect = Request.Form["MetaSelect_" + i];
				if (MetaSelect != null)
				{
					MetaTextString = Strings.Replace(Request.Form["frm_text_" + i], ", ", MetaSeparator, 1, -1, 0);
					if (MetaTextString != null && MetaTextString.Substring(0, 1) == MetaSeparator)
					{
						MetaTextString = MetaTextString.Substring(MetaTextString.Length - (MetaTextString.Length - 1), (MetaTextString.Length - 1));
					}
					acMetaInfo[3] = MetaTextString;
				}
				else
				{
					acMetaInfo[3] = Request.Form["frm_text_" + i];
				}
				page_meta_data.Add(acMetaInfo, i.ToString(), null, null);
				acMetaInfo = new object[4];
			}
			return (page_meta_data);
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/searchfuncsupport.js", "EktronSearchFuncSupportJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/optiontransfer.js", "EktronOptionTransferJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncsJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
		}
	}


using System.Web.UI.WebControls;
using System.Web.UI;
using System;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.ASM.AssetConfig;
using System.IO;
using Ektron.Cms.Controls;
using Ektron.Cms.Common;

	public partial class Workarea_Community_ManageCommunityDocuments : System.Web.UI.Page
	{
        private string DMSCookieName = "DMS_Office_ver";
		public string ImagegalleryImageWidthLbl = "";
		public string ImagegalleryTitleLbl = "";
		public string ImagegalleryAddressLbl = "";
		public string ImagegalleryDescriptionLbl = "";
		public string HeaderLbl = "";
		protected Ektron.ContentDesignerWithValidator ctlEditor;
		protected EkMessageHelper m_refMsg;
		protected ContentAPI m_refContApi = new ContentAPI();
		protected long _contentID;
		protected long _folderID = -1;
		protected int _contentLanguage = -1;
		protected string _fileExtension = "";
		protected long _taxonomyID = -1;
		protected bool isMetadataOrTaxonomyRequired = false;
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
            ctlEditor = (Ektron.ContentDesignerWithValidator)LoadControl("../controls/Editor/ContentDesignerWithValidator.ascx");
			ctlEditor.ID = "ekImagegalleryDescription";
			ctlEditor.AllowScripts = false;
			ctlEditor.Height = new Unit(250, UnitType.Pixel);
			ctlEditor.Width = new Unit(100, UnitType.Percentage);
			ctlEditor.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.Minimal;
			ctlEditor.ShowHtmlMode = false;
			ekImagegalleryDescriptionEditorHolder.Controls.Add(ctlEditor);
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (!Page.IsPostBack)
            {
                Session["EkDavSessionVal"] = null;
            }
			m_refMsg = m_refContApi.EkMsgRef;
			Utilities.ValidateUserLogin();
			RegisterResources();
            aHelp.HRef = "#Help";
            aHelp.Attributes.Add("onclick", "window.open('" + m_refContApi.fetchhelpLink("add_assets") + "', '', 'width=800,height=500,resizable=yes,scrollbars=yes,toolbar=no,location=no,directories=no,status=no,menubar=no,copyhistory=no');return false;");
            setLocalizedStrings();
            if (Request.QueryString["folderiD"] != null)
			{
				_folderID = Convert.ToInt64(Request.QueryString["folderID"]);
				jsFolderID.Text = _folderID.ToString();
                
			}
			
			if (Request.QueryString["TaxonomyId"] != null)
			{
				_taxonomyID = Convert.ToInt64(Request.QueryString["TaxonomyId"]);
				jsTaxonomyId.Text = _taxonomyID.ToString();
				jsTaxonomyIdReloadFrame.Text = _taxonomyID.ToString();
                jsTaxID.Text = _taxonomyID.ToString();
                isMetadataOrTaxonomyRequired = true;
			}
			
			if (Request.QueryString["LangType"] != null)
			{
				_contentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
				jsLanguageID.Text = _contentLanguage.ToString();
			}
			
			if (m_refContApi.EkContentRef.DoesFolderRequireMetadataOrTaxonomy(_folderID, _contentLanguage))
			{
				isMetadataOrTaxonomyRequired = true;
			}
			
			ExplorerDragDrop dragDrop = new ExplorerDragDrop();
			
			bool _useSSL = Ektron.Cms.Common.EkFunctions.GetConfigBoolean("ek_UseOffloadingSSL", false);
			string _scheme = string.Empty;
			if (_useSSL)
			{
				_scheme = "https";
			}
			else
			{
				_scheme = Page.Request.Url.Scheme;
			}
			
			destination.Value = Convert.ToString(_scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath) + "processMultiupload.aspx?close=true";
			PostURL.Value = Convert.ToString(_scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath) + "processMultiupload.aspx?close=true";
			NextUsing.Value = Convert.ToString(_scheme + Uri.SchemeDelimiter + Page.Request.Url.Authority + m_refContApi.ApplicationPath + "content.aspx");
			content_id.Value = "-1";
			content_folder.Value = _folderID.ToString();
			content_language.Value = _contentLanguage.ToString();
			requireMetaTaxonomy.Value = isMetadataOrTaxonomyRequired.ToString();
			taxonomyselectedtree.Value = _taxonomyID.ToString();
			content_teaser.Value = "";
			HtmlGenericControl linebreak = new HtmlGenericControl("div");
			linebreak.InnerHtml += "<div id=\'divFileTypes\' style=\'display:none;float:left;\'> " + m_refMsg.GetMessage("lbl valid file types") + "<p class=\'dmsSupportedFileTypes\' style=\'font-size:11px;\'>" + DocumentManagerData.Instance.FileTypes + "</p></div>";
			linebreak.InnerHtml += "<br />";
			linebreak.InnerHtml += "<div id=idMultipleView style=\'display:inline\'>";
			linebreak.InnerHtml += (string) ("<script type=\"text/javascript\">" + Environment.NewLine);
			linebreak.InnerHtml += (string) (" AC_AX_RunContent(\'id\',\'idUploadCtl\',\'name\',\'idUploadCtl\',\'classid\',\'CLSID:07B06095-5687-4d13-9E32-12B4259C9813\',\'width\',\'100%\',\'height\',\'350px\');" + Environment.NewLine);
			linebreak.InnerHtml += Environment.NewLine + " </script> </div> " + Environment.NewLine;
			linebreak.InnerHtml += "<br /><br />";
			//tabMultipleDMS.Controls.Add(linebreak);
            if (Request.Cookies[DMSCookieName] != null && !string.IsNullOrEmpty(Request.Cookies[DMSCookieName].Value))
            {
                pnl_versionToggle.Visible = pnl_DMSMultiUpload.Visible = true;
                pnl_OfficeVerSelector.Visible = false;

                if (Request.Cookies[DMSCookieName].Value == "2010")
                {
                    ExplorerDragDrop edd = new ExplorerDragDrop();
                    string ekdavurl=edd.GetFolderPath(_folderID).Replace(Page.Request.Url.GetLeftPart(UriPartial.Authority), "");
                    try
                    {
                        string[] urlparts = ekdavurl.Split('_');
                        urlparts[4] = _taxonomyID.ToString();
                        ekdavurl = string.Join("_", urlparts);
                    }
                    catch
                    { }
                    destination.Value = ekdavurl;//edd.GetFolderPath(_folderID).Replace(Page.Request.Url.GetLeftPart(UriPartial.Authority), "");
                    putopts.Value = "false";
                    Upload.OnClientClick = "return MultipleDocumentUpload(0);";
                    lbtn_toggleVersion.Attributes.Add("onclick", string.Format(m_refMsg.GetMessage("js office version toggle confirm format"), m_refMsg.GetMessage("li text office 2010 name")));
                    lbtn_toggleVersion.Text = m_refMsg.GetMessage("lbtn dms switch office version") + "(Office 2010 selected)";
                }
                else
                {
                    destination.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath + "processMultiupload.aspx?close=true";
                    PostURL.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath + "processMultiupload.aspx?close=true";
                    putopts.Value = "true";
                    Upload.OnClientClick = "MultipleDocumentUpload(1);";
                    lbtn_toggleVersion.Attributes.Add("onclick", string.Format(m_refMsg.GetMessage("js office version toggle confirm format"), m_refMsg.GetMessage("li text other office ver name")));
                    lbtn_toggleVersion.Text = m_refMsg.GetMessage("lbtn dms switch office version") + "(Office 2003/2007 selected)";

                }
                tabMultipleDMS.Controls.Add(linebreak);
            }
            else
            {
                pnl_versionToggle.Visible = pnl_DMSMultiUpload.Visible = false;
                pnl_OfficeVerSelector.Visible = true;
            }

            if (Request.QueryString["showtab"] != null && Request.QueryString["showtab"] == "multiple")
            {
                ltrNoUpload.Text = m_refMsg.GetMessage("lbl upload file");
                ClientScript.RegisterStartupScript(this.GetType(), "selectTab", "var ddTabs = $ektron('#DMStabs').tabs({select: function(event, ui){ResizeDocumentContainer(ui);}}); ddTabs.tabs('select', 1);", true);
            }

			if (! Page.IsPostBack)
			{
				
				dragDrop.FolderID = _folderID;
				dragDrop.TaxonomyId = _taxonomyID;
				
				if (_contentLanguage != -1)
				{
					dragDrop.ContentLanguage = _contentLanguage;
				}
				
				HelpMessage.Text = "Fill out the description and then click next to upload image(s)."; //m_refMsg.GetMessage("lbl fill out the description and then click next to drag and drop image(s)")
				btnNext.Text = m_refMsg.GetMessage("next") + ">>";
				ImagegalleryTitleLbl = m_refMsg.GetMessage("generic title label");
				ImagegalleryImageWidthLbl = m_refMsg.GetMessage("lbl maximum width");
				ImagegalleryAddressLbl = m_refMsg.GetMessage("lbl image mapaddress");
				ImagegalleryDescriptionLbl = m_refMsg.GetMessage("description label");
				HeaderLabel.Text = m_refMsg.GetMessage("lbl photo data");
				btnNext.Attributes.Add("onclick", "ResizeContainer();");
				
				if ((Request.QueryString["prop"] != null)&& Request.QueryString["prop"] != "" && (Request.QueryString["type"] == "update") || (Request.QueryString["prop"] == "image"))
				{
					// Content Designer
					ctlEditor.FolderId = _folderID;
					ctlEditor.AllowFonts = true;
					
					if (Request.QueryString["prop"] == "image")
					{
						dragDrop.IsImage = 1;
					}
					
					this.panelImageProperties.Visible = true;
					this.panelDragDrop.Visible = false;
					if ((Request.QueryString["type"] != null)&& Request.QueryString["type"] == "update")
					{
						HelpMessage.Text = m_refMsg.GetMessage("lbl fill out the description and then click save");
						btnNext.Attributes.Add("onclick", "return HideContainer(this);");
						long id = 0;
						if (Request.QueryString["id"] != null)
						{
							id = Convert.ToInt64(Request.QueryString["id"]);
						}
						ekImagegalleryImageWidthLbl.Visible = false;
						ekImagegalleryImageWidth.Visible = false;
						if (id > 0)
						{
							Ektron.Cms.ContentData data;
							Ektron.Cms.ContentAPI api = new Ektron.Cms.ContentAPI();
							data = api.GetContentById(id, 0);
							this.ekImagegalleryTitle.Value = Server.HtmlDecode(data.Title);
							ctlEditor.Content = data.Teaser;
							if (Request.QueryString["prop"] == "image")
							{
								this.HeaderLabel.Text = "Image Properties";
								foreach (Ektron.Cms.ContentMetaData item in data.MetaData)
								{
									if (item.TypeName.ToLower() == "mapaddress")
									{
										this.ekImagegalleryAddress.Value = item.Text;
										break;
									}
								}
							}
							else
							{
								this.HeaderLabel.Text = "Document Properties";
								this.ekImagegalleryAddress.Visible = false;
								this.ekImagegalleryAddressLbl.Visible = false;
							}
							btnNext.Text = "Save";
						}
					}
					else
					{
						ekImagegalleryTitleLbl.Visible = false;
						ekImagegalleryTitle.Visible = false;
					}
				}
				else
				{
					this.panelImageProperties.Visible = false;
					this.panelDragDrop.Visible = true;
				}
			}
			else
			{
				Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
				string desc = "";
				int width = 800;
				string address = "";
				desc = (string) ctlEditor.Content;
				if (Request.Form["ekImagegalleryImageWidth"] != null)
				{
					width = System.Convert.ToInt32(Request.Form["ekImagegalleryImageWidth"].ToString());
				}
				if (Request.Form["ekImagegalleryAddress"] != null)
				{
					address = (string) (Request.Form["ekImagegalleryAddress"].ToString());
				}
				
				string[] imageProp = new string[4];
				imageProp[0] = width.ToString(); //width
				imageProp[1] = "-1"; //height
				imageProp[2] = address; //mapaddress
				imageProp[3] = desc; //Descriptions
				
				if (Request.QueryString["type"] != "update")
				{
					Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Remove(api.UserId.ToString() + "_" + api.UniqueId.ToString() + "_MapMeta");
					Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Add(api.UserId.ToString() + "_" + api.UniqueId.ToString() + "_MapMeta", imageProp);
				}
			}
			
			if (Request.Browser.Type.IndexOf("Firefox") != -1)
			{
				liDragDrop.Visible = true;
				tabDragDrop.Visible = true;
				tabDragDrop.Controls.Add(dragDrop);
			}
			
			literal_wait.Text = m_refMsg.GetMessage("one moment msg");
		}
		
		protected void btnNext_Click(object sender, System.EventArgs e)
		{
			Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
			string desc = "";
			int width = 800;
			string address = "";
			int isimage = 0;
			this.panelImageProperties.Visible = true;
			this.panelDragDrop.Visible = false;
			if (Request.QueryString["prop"] == "image")
			{
				isimage = 1;
			}
			if ((Request.QueryString["type"] != null)&& Request.QueryString["type"] == "update")
			{
				long id = 0;
				if (Request.QueryString["id"] != null)
				{
					id = Convert.ToInt64(Request.QueryString["id"]);
					
					Ektron.Cms.ContentEditData data;
					Ektron.Cms.ContentAPI apiContent = new Ektron.Cms.ContentAPI();
					int metaId = 0;
					data = apiContent.GetContentForEditing(id);
					if (this.ekImagegalleryTitle.Value.IndexOf("<") != -1 || this.ekImagegalleryTitle.Value.IndexOf(">") != -1 || this.ekImagegalleryTitle.Value.IndexOf("\'") != -1 || this.ekImagegalleryTitle.Value.IndexOf("\"") != -1)
					{
						this.ekImagegalleryTitle.Value = this.ekImagegalleryTitle.Value.Replace("<", "").Replace(">", "").Replace("\'", "").Replace("\"", "");
					}
					data.Title = (string) this.ekImagegalleryTitle.Value;
					data.Teaser = (string) ctlEditor.Content;
					content_teaser.Value = ctlEditor.Content;
					data.FileChanged = false;
					apiContent.SaveContent(data);
					if (Request.QueryString["prop"] == "image")
					{
						isimage = 1;
						foreach (Ektron.Cms.ContentMetaData item in data.MetaData)
						{
							if (item.TypeName.ToLower() == "mapaddress")
							{
                                metaId = Convert.ToInt32(item.TypeId);
								break;
							}
						}
						if (metaId > 0)
						{
							apiContent.UpdateContentMetaData(data.Id, metaId, (string) this.ekImagegalleryAddress.Value);
						}
					}
					apiContent.PublishContentById(id, data.FolderId, data.LanguageId, "", api.UserId, "");
					this.ctlEditor.Visible = false; //so that the content designer will not be initialized again.
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "close_box", "Close();", true);
				}
			}
			else
			{
				desc = (string) ctlEditor.Content;
				if (Request.Form["ekImagegalleryImageWidth"] != null)
				{
					width = System.Convert.ToInt32(Request.Form["ekImagegalleryImageWidth"].ToString());
				}
				if (Request.Form["ekImagegalleryAddress"] != null)
				{
					address = (string) (Request.Form["ekImagegalleryAddress"].ToString());
				}
				
				string[] imageProp = new string[4];
				imageProp[0] = width.ToString(); //width
				imageProp[1] = "-1"; //height
				imageProp[2] = address; //mapaddress
				imageProp[3] = desc; //Descriptions
				
				Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Remove(api.UserId.ToString() + "_" + api.UniqueId.ToString() + "_MapMeta");
				Ektron.ASM.EkDavProtocol.Constants.GetCustomCacheManger().Add(api.UserId.ToString() + "_" + api.UniqueId.ToString() + "_MapMeta", imageProp);
				
				this.ctlEditor.Visible = false; //so that the content designer will not be initialized again.
				string _doHide = string.Empty;
				string _galleryString = string.Empty;
				if (! (Request.QueryString["hidecancel"] == null) && Request.QueryString["hidecancel"] == "true")
				{
					_doHide = "&hidecancel=true";
				}
				if (! (Request.QueryString["isimagegallery"] == null) && Request.QueryString["isimagegallery"] == "true")
				{
					_galleryString = "&isimagegallery=true";
				}
				
				Response.Redirect(m_refContApi.AppPath + "DragDropCtl.aspx?mode=0&folder_id=" + Request.QueryString["folderiD"] + "&lang_id=" + m_refContApi.ContentLanguage + "&TaxonomyId=" + Request.QueryString["TaxonomyId"] + "&isimage=" + isimage + _doHide + _galleryString);
			}
		}


        protected void btn_VersionSelect_Click(object sender, EventArgs e)
        {
            //Set hidden values accordingly
            if (rbl_OfficeVersion.SelectedValue == "2010")
            {
                //2010 uploader values
                ExplorerDragDrop edd = new ExplorerDragDrop();
                destination.Value = edd.GetFolderPath(_folderID).Replace(Page.Request.Url.GetLeftPart(UriPartial.Authority), ""); //contentAPI.ApplicationPath + "uploadedfiles";
                PostURL.Value = "";
                putopts.Value = "false";
                Upload.OnClientClick = "return MultipleDocumentUpload(0);";
                lbtn_toggleVersion.Attributes.Add("onclick", string.Format(m_refMsg.GetMessage("js office version toggle confirm format"), m_refMsg.GetMessage("li text office 2010 name")));
                lbtn_toggleVersion.Text = m_refMsg.GetMessage("lbtn dms switch office version") + "(Office 2010 selected)";
            }
            else
            {   //Keep the original values
                bool _useSSL = Ektron.Cms.Common.EkFunctions.GetConfigBoolean("ek_UseOffloadingSSL", false);
                string _scheme = string.Empty;
                if (_useSSL)
                    _scheme = "https";
                else
                    _scheme = Page.Request.Url.Scheme;
                destination.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath + "processMultiupload.aspx?close=true";
                PostURL.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath + "processMultiupload.aspx?close=true";
                putopts.Value = "true";
                Upload.OnClientClick = "MultipleDocumentUpload(1);";
                lbtn_toggleVersion.Attributes.Add("onclick", string.Format(m_refMsg.GetMessage("js office version toggle confirm format"), m_refMsg.GetMessage("li text other office ver name")));
                lbtn_toggleVersion.Text = m_refMsg.GetMessage("lbtn dms switch office version") + "(Office 2003/2007 selected)";

            }

            //Create the control
            string filetypes = "";
            if (this.isImage.Value == "1")
            {
                string[] AllowedFileTypes = null;
                if (DocumentManagerData.Instance.FileTypes.Length > 0)
                {
                    AllowedFileTypes = DocumentManagerData.Instance.FileTypes.Split(',');
                    if (AllowedFileTypes != null && AllowedFileTypes.Length > 0)
                    {
                        foreach (string filetype in AllowedFileTypes)
                        {
                            if (EkFunctions.IsImage(filetype.Trim().Replace("*", "")))
                            {
                                if (filetypes.Length > 0)
                                    filetypes += "," + filetype;
                                else
                                    filetypes = filetype;
                            }
                        }
                    }
                }
            }
            else
                filetypes = DocumentManagerData.Instance.FileTypes;
            HtmlGenericControl linebreak = new HtmlGenericControl("div");
            linebreak.InnerHtml += "<div id='divFileTypes'> " + m_refMsg.GetMessage("lbl valid file types") + "<p class='dmsSupportedFileTypes'>" + filetypes + "</p></div>";
            linebreak.InnerHtml += "<div id=idMultipleView style='display:inline'>";
            linebreak.InnerHtml += "<script type=\"text/javascript\">" + Environment.NewLine;
            linebreak.InnerHtml += " AC_AX_RunContent('id','idUploadCtl','name','idUploadCtl','classid','CLSID:07B06095-5687-4d13-9E32-12B4259C9813','width','100%','height','350px');" + Environment.NewLine;
            linebreak.InnerHtml += Environment.NewLine + " </script> </div> " + Environment.NewLine;
            tabMultipleDMS.Controls.Add(linebreak);

            //Create cookie
            HttpCookie c = new HttpCookie(DMSCookieName, rbl_OfficeVersion.SelectedValue);
            c.Expires = DateTime.Now.AddYears(50);
            Response.Cookies.Remove(c.Name);
            Response.Cookies.Add(c);

            //reset UI
            pnl_versionToggle.Visible = pnl_DMSMultiUpload.Visible = true;
            pnl_OfficeVerSelector.Visible = false;
            ClientScript.RegisterStartupScript(this.GetType(), "selectTab", "var ddTabs = $ektron('#DMStabs').tabs({select: function(event, ui){ResizeDocumentContainer(ui);}}); ddTabs.tabs('select', 1);", true);
        }

        protected void lbtn_toggleVersion_Click(object sender, EventArgs e)
        {
            string cookieVal = "";

            if (Request.Cookies[DMSCookieName].Value == "2010")
            {
                cookieVal = "other";
                bool _useSSL = Ektron.Cms.Common.EkFunctions.GetConfigBoolean("ek_UseOffloadingSSL", false);
                string _scheme = string.Empty;
                if (_useSSL)
                    _scheme = "https";
                else
                    _scheme = Page.Request.Url.Scheme;
                destination.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath + "processMultiupload.aspx?close=true";
                PostURL.Value = _scheme + Uri.SchemeDelimiter + this.Page.Request.Url.Authority + m_refContApi.ApplicationPath + "processMultiupload.aspx?close=true";
                putopts.Value = "true";
                Upload.OnClientClick = "MultipleDocumentUpload(1);";
                lbtn_toggleVersion.Attributes.Add("onclick", string.Format(m_refMsg.GetMessage("js office version toggle confirm format"), m_refMsg.GetMessage("li text other office ver name")));
                lbtn_toggleVersion.Text = m_refMsg.GetMessage("lbtn dms switch office version") + "(Office 2003/2007 selected)";
            }
            else
            {
                cookieVal = "2010";
                ExplorerDragDrop edd = new ExplorerDragDrop();
                destination.Value = edd.GetFolderPath(_folderID).Replace(Page.Request.Url.GetLeftPart(UriPartial.Authority), "");
                putopts.Value = "false";
                Upload.OnClientClick = "return MultipleDocumentUpload(0);";
                lbtn_toggleVersion.Attributes.Add("onclick", string.Format(m_refMsg.GetMessage("js office version toggle confirm format"), m_refMsg.GetMessage("li text office 2010 name")));
                lbtn_toggleVersion.Text = m_refMsg.GetMessage("lbtn dms switch office version") + "(Office 2010 selected)";
            }
            HttpCookie c = new HttpCookie(DMSCookieName, cookieVal);
            c.Expires = DateTime.Now.AddYears(50);
            Response.Cookies.Remove(c.Name);
            Response.Cookies.Add(c);
            ClientScript.RegisterStartupScript(this.GetType(), "selectTab", "var ddTabs = $ektron('#DMStabs').tabs({select: function(event, ui){ResizeDocumentContainer(ui);}}); ddTabs.tabs('select', 1);", true);
        }

        protected void lbtn_processTaxMeta_Click(object sender, EventArgs e)
        {

            string m_idlist = "";
            if (Session["EkDavSessionVal"] != null)
            {
                m_idlist = Session["EkDavSessionVal"].ToString();
            }
            Ektron.Cms.API.Content.Content cContent = new Ektron.Cms.API.Content.Content();
            List<string> tmp_list = new List<string>();
            tmp_list.AddRange(m_idlist.Split(','));
            for(int i=0;i<tmp_list.Count;i++)
            {
                if (string.IsNullOrEmpty(tmp_list[i]))
                    tmp_list.RemoveAt(i);
            }

            string[] ID_arr = tmp_list.ToArray();

            foreach (string id in ID_arr)
            {

                Int64 _contentID = -1;
                if (Int64.TryParse(id, out _contentID))
                {
                    TaxonomyContentRequest t = new TaxonomyContentRequest();
                    t.ContentId = _contentID;
                    t.FolderID = _folderID;
                    t.TaxonomyList = _taxonomyID.ToString();
                    cContent.EkContentRef.AddTaxonomyItem(t);
                }

            }
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "close_box", "Close();", true);
        }


		private void RegisterResources()
		{
			// Css
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
			// JS
			JS.RegisterJS(this, JS.ManagedScript.EktronJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronXmlJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronUITabsJS);
			JS.RegisterJS(this, m_refContApi.ApplicationPath + "java/ActiveXActivate.js", "EktronActiveXActivateJs");
			JS.RegisterJS(this, m_refContApi.ApplicationPath + "java/RunActiveContent.js", "EktronRunActiveContentJs");
			JS.RegisterJS(this, m_refContApi.ApplicationPath + "java/determineoffice.js", "EktronDetermineOfficeJs");
			
			ltrPleaseWait.Text = m_refMsg.GetMessage("msg pls wait file uploads");
		}
        private void setLocalizedStrings()
        {
            lbl_processTacMeta.Text = m_refMsg.GetMessage("lbl cmu mupload taxmeta proc");
            btn_processTaxMeta.Text = m_refMsg.GetMessage("btn cmu mupload taxmeta proc");
            jsHideFTypes.Text = m_refMsg.GetMessage("lbl hide file types");
            jsShowFTypes.Text = m_refMsg.GetMessage("lbl show file types");
            ltrShowFTypes.Text = m_refMsg.GetMessage("lbl show file types");

            lit_VerionSelect.Text = m_refMsg.GetMessage("lbl dms office ver sel header");
            foreach (ListItem li in rbl_OfficeVersion.Items)
            {
                if (li.Value == "2010")
                    li.Text = m_refMsg.GetMessage("li text office 2010 name");
                else
                    li.Text = m_refMsg.GetMessage("li text other office ver name");
            }
        }
		
		protected void uploadFile_Click(object sender, EventArgs e)
		{
			string fileName = string.Empty;
			HttpPostedFile fileUpld = ekFileUpload.PostedFile;
			string hasValidExtension = "";
			List<string> AllowedFileTypes = new List<string>();
			
			AllowedFileTypes.AddRange(DocumentManagerData.Instance.FileTypes.ToString().Split(','.ToString().ToCharArray()));
			if (fileUpld.ContentLength > 0)
			{
				Ektron.Cms.UserAPI uAPI = new UserAPI();
				
				_fileExtension = Path.GetExtension(fileUpld.FileName);
				//hasValidExtension = (string) (AllowedFileTypes.Find(new Predicate<string>(new System.EventHandler(CheckExtension))));
			    hasValidExtension = AllowedFileTypes.Find(new Predicate<string>(CheckExtension));
				
				if ((hasValidExtension != null)&& hasValidExtension != "")
				{
					
					//If Image Gallery, Should check if the file type is an image file type
					if ((Request.QueryString["prop"] != null)&& Request.QueryString["prop"].ToLower() == "image")
					{
						if (! Ektron.Cms.Common.EkFunctions.IsImage(_fileExtension))
						{
							ltrStatus.Text = m_refMsg.GetMessage("msg invalid file upload images only");
							setInvalid();
							return;
						}
					}
					
					fileName = Path.GetFileName(fileUpld.FileName);
					int fileLength = fileUpld.ContentLength;
					byte[] fileData = new byte[fileLength - 1+ 1];
					string file = Convert.ToString(fileUpld.InputStream.Read(fileData, 0, fileLength));
					
					if (fileData.Length > 0)
					{
						System.IO.MemoryStream stream = new System.IO.MemoryStream(fileData);
						m_refContApi.RequestInformationRef.UserId = uAPI.UserId;
						m_refContApi.ContentLanguage = _contentLanguage;
						
						Ektron.ASM.AssetConfig.AssetData asstData = new Ektron.ASM.AssetConfig.AssetData();
						Ektron.Cms.API.Content.Content cContent = new Ektron.Cms.API.Content.Content();
						asstData = m_refContApi.EkContentRef.GetAssetDataBasedOnFileName(fileName, _folderID, _taxonomyID);
						if ((asstData != null)&& asstData.ID != "" && asstData.Name != "")
						{
							Ektron.Cms.AssetUpdateData astData = new AssetUpdateData();
							TaxonomyBaseData[] taxonomyCatArray = null;
							_contentID = Convert.ToInt64(asstData.ID);
							ContentData cData = cContent.GetContent(_contentID, Ektron.Cms.ContentAPI.ContentResultType.Published);
							
							astData.FileName = fileName;
							astData.FolderId = _folderID;
							astData.ContentId = cData.Id;
							astData.Teaser = cData.Teaser;
							astData.Comment = cData.Comment;
							astData.Title = cData.Title;
							astData.GoLive = cData.GoLive;
							astData.TaxonomyTreeIds = this._taxonomyID.ToString();
							
							//Assigning the categories
							taxonomyCatArray = m_refContApi.ReadAllAssignedCategory(_contentID);
							if ((taxonomyCatArray != null)&& taxonomyCatArray.Length > 0)
							{
								foreach (TaxonomyBaseData tBaseData in taxonomyCatArray)
								{
									if (astData.TaxonomyTreeIds == "")
									{
										astData.TaxonomyTreeIds = tBaseData.TaxonomyId.ToString();
									}
									else
									{
										astData.TaxonomyTreeIds += (string) ("," + tBaseData.TaxonomyId.ToString());
									}
								}
							}
							
							//Assigning the metadatas
							if ((cData.MetaData != null)&& cData.MetaData.Length > 0)
							{
								astData.MetaData = new Ektron.Cms.AssetUpdateMetaData[cData.MetaData.Length - 1 + 1];
								for (int i = 0; i <= cData.MetaData.Length - 1; i++)
								{
									astData.MetaData[i] = new AssetUpdateMetaData();
									astData.MetaData[i].TypeId = cData.MetaData[i].TypeId;
									astData.MetaData[i].ContentId = cData.Id;
									astData.MetaData[i].Text = cData.MetaData[i].Text;
								}
							}
							astData.EndDate = cData.EndDate;
							astData.EndDateAction = (Ektron.Cms.Common.EkEnumeration.CMSEndDateAction) (Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.CMSEndDateAction), cData.EndDateAction.ToString(), true));
							
							//Updating the Content
							bool isUpdated = m_refContApi.EditAsset(stream, astData);
						}
						else
						{
							Ektron.Cms.AssetUpdateData astData = new AssetUpdateData();
							astData.FileName = fileName;
							astData.FolderId = _folderID;
							astData.Title = Path.GetFileNameWithoutExtension(fileName);
							astData.LanguageId = _contentLanguage;
							astData.TaxonomyTreeIds = this._taxonomyID.ToString();
							_contentID = m_refContApi.AddAsset(stream, astData);
						}
						
						jsMetaUrl.Text = "";
						if (m_refContApi.EkContentRef.DoesFolderRequireMetadataOrTaxonomy(_folderID, _contentLanguage))
						{
							var _taxString = string.Empty;
							if (this._taxonomyID != -1)
							{
								_taxString = (string) ("&taxonomyId=" + this._taxonomyID);
							}
							jsMetaUrl.Text = m_refContApi.AppPath + "DMSMetadata.aspx?contentId=" + _contentID + "&idString=" + _contentID + "&folderId=" + _folderID + _taxString + "&close=true&EkTB_iframe=true&height=550&width=650&modal=true&refreshCaller=true";
						}
						
						isFileUploadComplete.Value = "true";
						ClientScript.RegisterStartupScript(this.GetType(), "closeThickBox", "uploadClick();", true);
					}
				}
				else
				{
					ltrStatus.Text = m_refMsg.GetMessage("msg invalid file upload");
					setInvalid();
				}
			}
			else
			{
				ltrStatus.Text = m_refMsg.GetMessage("lbl upload file");
				setInvalid();
			}
		}
		
		private void setInvalid()
		{
			DragDropUI.Style.Add("position", "relative");
			DragDropUI.Style.Add("left", "0px");
			ek_DmsFileUploadWait.Style.Add("position", "relative");
			ek_DmsFileUploadWait.Style.Add("left", "-10000px");
			isFileUploadComplete.Value = "invalid";
		}
		private bool CheckExtension(string item)
		{
			return item.ToLower().Replace(" ", "") == "*" + this._fileExtension.ToLower();
		}
		
		[System.Web.Services.WebMethod()][System.Web.Script.Services.ScriptMethod()]public static bool CheckFileExists(string FileName, string FolderID, string ContLanguage,string TaxonomyID)
			{
			ContentAPI cApi = new ContentAPI();
			cApi.ContentLanguage = Convert.ToInt32(ContLanguage);
			Ektron.ASM.AssetConfig.AssetData assetDat = new Ektron.ASM.AssetConfig.AssetData();
            Int64 taxID = -1;
            Int64.TryParse(TaxonomyID, out taxID);
            assetDat = cApi.EkContentRef.GetAssetDataBasedOnFileName(Path.GetFileName(FileName), Convert.ToInt64(FolderID), taxID);
			if ((assetDat != null)&& assetDat.ID != "" && assetDat.Name != "")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
	}


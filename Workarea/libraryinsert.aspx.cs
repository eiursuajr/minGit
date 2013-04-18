using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.UI;
using Microsoft.VisualBasic;

	public partial class libraryinsert : System.Web.UI.Page
	{
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		protected string action;
		protected string RetField;
		protected string Item_scope;
		protected long folder = 0;
		protected string LibType = "";
		protected string AppImgPath = "";
		protected string SitePath = "";
		private Ektron.Cms.Library.EkLibrary m_refLib;
		protected FolderData cFolder;
		protected PermissionData cPerms;
		protected Collection TypeCounts;
		protected bool IsMac = false;
		private int m_intContentLanguage = 0;
		protected int j = 0;
		protected string[] arr_TdName;
		protected bool UseLinkItPage = true; //This is hardcoded for now but could be a switch in the web.config file.
		protected int _currentPageNumber = 1;
		protected int TotalPagesNumber = 1;
		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			m_refMsg = m_refContentApi.EkMsgRef;
            if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            } 
            AppImgPath = m_refContentApi.AppImgPath;
			SitePath = m_refContentApi.SitePath;
			m_refLib = m_refContentApi.EkLibraryRef;
			StyleSheetJS.Text = m_refStyle.GetClientScript();
			UseLinkItPage = m_refContentApi.RequestInformationRef.LinkManagement;
			RegisterResources();
			action = "viewlibrarybycategory";
			if (!(Request.QueryString["LangType"] == null))
			{
				if (Request.QueryString["LangType"] != "")
				{
					m_intContentLanguage = System.Convert.ToInt32(Request.QueryString["LangType"]);
				}
				else
				{
					if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						m_intContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
			}
			else
			{
				if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					m_intContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			
			if (m_intContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
			{
				m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			else
			{
				m_refContentApi.ContentLanguage = m_intContentLanguage;
			}
			
			if (Request.QueryString["scope"] == "images")
			{
				LibType = "images";
				Session["LibCategory"] = LibType;
			}
			else if (Request.QueryString["type"] == null && Request.QueryString["scope"] == "images,hLink,files")
			{
				LibType = "images";
				Session["LibCategory"] = LibType;
			}
			else
			{
				if (!String.IsNullOrEmpty(Request.QueryString["type"]))
				{
					LibType = Request.QueryString["type"];
					Session["LibCategory"] = LibType;
				}
				else
				{
					LibType = Session["LibCategory"].ToString();
				}
				
				if (LibType == "")
				{
					LibType = "images";
					Session["LibCategory"] = LibType;
				}
			}
			if (Request.Browser.Platform.IndexOf("Win") == -1)
			{
				IsMac = true;
			}
			if (!(LibType == "quicklinks" || LibType == "forms" || LibType == "hyperlinks" || LibType == "files" || LibType == "images"))
			{
				m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
			}
			
			if (!String.IsNullOrEmpty(Request.QueryString["action"]))
			{
					action = Request.QueryString["action"].ToLower();
			}
			if (!String.IsNullOrEmpty(Request.QueryString["id"]))
			{
				folder = Convert.ToInt64(Request.QueryString["id"]);
			}
			else if (!String.IsNullOrEmpty(Request.QueryString["folder"]))
			{
				folder = Convert.ToInt64(Request.QueryString["folder"]);
			}
			RetField = Request.QueryString["RetField"];
			if (!String.IsNullOrEmpty(RetField))
			{
                RetField = (string)("&RetField=" + EkFunctions.HtmlEncode(RetField).Replace("&amp;", "&"));
           
			}
			
			Item_scope = Request.QueryString["Scope"];
            if (!String.IsNullOrEmpty(Item_scope))
            {
                Item_scope = (string)("&Scope=" + Item_scope);

                if (Item_scope.Trim() != "")
                {
                    RetField = Item_scope + RetField;
                }
            }
            if (!String.IsNullOrEmpty(Request.QueryString["disableLinkManage"]))
			{
				UseLinkItPage = false;
			}
			if (action.ToLower() == "viewlibrarybycategory" && ! IsPostBack)
			{
				Display_ViewLibraryByCategory();
			}
		}
		
		private void Display_ViewLibraryByCategory()
		{
			LibraryData[] result;
			Ektron.Cms.Content.EkContent m_refcontent;
			Collection cTmp = new Collection();
			m_refcontent = m_refContentApi.EkContentRef;
			
			cTmp.Add(LibType, "LibraryType", null, null);
			cTmp.Add(folder, "ParentID", null, null);
			cTmp.Add(Request.QueryString["orderby"], "OrderBy", null, null);
			
			
			result = m_refLib.GetAllChildLibItemsByTypev5_0(LibType, folder, Request.QueryString["orderby"], _currentPageNumber, m_refContentApi.RequestInformationRef.PagingSize, ref TotalPagesNumber);
			
			if (TotalPagesNumber <= 1)
			{
				TotalPages.Visible = false;
				CurrentPage.Visible = false;
				lnkBtnPreviousPage.Visible = false;
				NextPage.Visible = false;
				LastPage.Visible = false;
				FirstPage.Visible = false;
				PageLabel.Visible = false;
				OfLabel.Visible = false;
			}
			else
			{
				
				TotalPages.Visible = true;
				CurrentPage.Visible = true;
				lnkBtnPreviousPage.Visible = true;
				NextPage.Visible = true;
				LastPage.Visible = true;
				FirstPage.Visible = true;
				PageLabel.Visible = true;
				OfLabel.Visible = true;
				
				TotalPages.Text = TotalPagesNumber.ToString();
				TotalPages.ToolTip = TotalPages.Text;
				
				CurrentPage.Text = _currentPageNumber.ToString();
				CurrentPage.ToolTip = CurrentPage.Text;
				
				if (_currentPageNumber == 1)
				{
					
					lnkBtnPreviousPage.Enabled = false;
					
					if (TotalPagesNumber > 1)
					{
						NextPage.Enabled = true;
					}
					else
					{
						NextPage.Enabled = false;
					}
					
				}
				else
				{
					
					lnkBtnPreviousPage.Enabled = true;
					
					if (_currentPageNumber == TotalPagesNumber)
					{
						NextPage.Enabled = false;
					}
					else
					{
						NextPage.Enabled = true;
					}
					
				}
			}
			
			TypeCounts = m_refcontent.GetLibraryTypeCount(folder);
			cFolder = m_refContentApi.GetFolderById(folder);
			cPerms = m_refContentApi.LoadPermissions(folder, "folder", 0);
			ViewLibraryByCategoryToolBar();
			
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ITEM1";
			
			colBound.HeaderText = "<a href=\"libraryinsert.aspx?action=ViewLibraryByCategory&orderby=librarytitle&id=" + folder + "&type=" + EkFunctions.HtmlEncode(LibType) + EkFunctions.HtmlEncode(RetField) + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic Title") + "</a>";
			colBound.ItemStyle.Wrap = false;
			colBound.HeaderStyle.CssClass = "title-header";
			MediaListGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ITEM2";
            colBound.HeaderText = "<a href=\"libraryinsert.aspx?action=ViewLibraryByCategory&orderby=libraryid&id=" + folder + "&type=" + EkFunctions.HtmlEncode(LibType) + EkFunctions.HtmlEncode(RetField) + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + m_refMsg.GetMessage("generic ID") + "</a>";
			colBound.ItemStyle.Wrap = false;
			colBound.HeaderStyle.CssClass = "title-header";
			MediaListGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "ITEM3";
            colBound.HeaderText = "<a href=\"libraryinsert.aspx?action=ViewLibraryByCategory&orderby=date&id=" + folder + "&type=" + EkFunctions.HtmlEncode(LibType) + EkFunctions.HtmlEncode(RetField) + "\" title=\"" + m_refMsg.GetMessage("click to sort msg") + "\">" + "Date modified" + "</a>";
			colBound.ItemStyle.Wrap = false;
			colBound.HeaderStyle.CssClass = "title-header";
			MediaListGrid.Columns.Add(colBound);
			
			
			DataTable dt = new DataTable();
			DataRow dr;
			int i = 0;
			dt.Columns.Add(new DataColumn("ITEM1", typeof(string)));
			dt.Columns.Add(new DataColumn("ITEM2", typeof(string)));
			dt.Columns.Add(new DataColumn("ITEM3", typeof(string)));
			
			bool showQDContentOnly = System.Convert.ToBoolean(Request.QueryString["qdo"] == "1");
			if (! showQDContentOnly || ! m_refContentApi.RequestInformationRef.EnableReplication || (showQDContentOnly && cFolder.ReplicationMethod == 1))
				{
				// display content selections if this filtering QD content
				// or if quickdeploy is enabled, only display content in a QD folder
				
				string sDesLoc;
				Array myfilePathArray;
				string myImagefile;
				string myNewImagefile;
				string sExt;
				FileInfo  fs;
				string QLinkDisplayName;
				string TdName = "";
				string strLibraryFileName = "";
				foreach (LibraryData myLibrary in result)
				{
					myLibrary.Title = Server.HtmlDecode(myLibrary.Title);
					myLibrary.FileName = Server.HtmlDecode(myLibrary.FileName);
					if (!(IsMac && EkConstants.IsAssetContentType(myLibrary.ContentType,false)))
					{
						Array.Resize(ref arr_TdName, i + 1);
						TdName = (string) ("cell" + myLibrary.Id);
						QLinkDisplayName = myLibrary.Title;
						
						dr = dt.NewRow();
						
						strLibraryFileName = (string) (Convert.ToString(myLibrary.FileName).Replace("\'", "\\\'"));
						if (UseLinkItPage)
						{
							if (LibType == "quicklinks" || LibType == "forms")
							{
								if (LibType == "forms")
								{
									strLibraryFileName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=ekfrm&ItemID=" + myLibrary.ContentId;
								}
								else
								{
									strLibraryFileName = m_refContentApi.AppPath + "linkit.aspx?LinkIdentifier=id&ItemID=" + myLibrary.ContentId;
								}
							}
						}
						arr_TdName[i] = TdName;
						dr["ITEM1"] = "<ILAYER name=\"layer" + TdName + "\">";
						dr["ITEM1"] += "<LAYER width=\"100%\">";
						dr["ITEM1"] += "<a href=\"#\" title=\"" + m_refMsg.GetMessage("double click to insert msg") + "\" onClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + strLibraryFileName + "\', \'" + LibType + "\', " + myLibrary.ContentId + ");Blink(\'" + TdName + "\', \'yellow\');return false;\" OnDblClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + strLibraryFileName + "\', \'" + LibType + "\', \'" + myLibrary.ContentId + "\');showSelAliasdialog();\"" + " >";
						if (myLibrary.TypeId != 3)
						{
							dr["ITEM1"] += QLinkDisplayName + "</a><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  [" + myLibrary.FileName.Substring(myLibrary.FileName.Length - (myLibrary.FileName.Length - SitePath.Length), (myLibrary.FileName.Length - SitePath.Length)) + "]";
						}
						else
						{
							dr["ITEM1"] += QLinkDisplayName + "</a><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  [" + myLibrary.FileName + "]";
						}
						
						dr["ITEM1"] += "</LAYER>";
						dr["ITEM1"] += "</ILAYER>";
						
						dr["ITEM2"] = "<ILAYER name=\"layer" + TdName + "_0" + ">";
						dr["ITEM2"] += "<LAYER width=\"100%\">";
						dr["ITEM2"] += "<a href=\"#\" title=\"double click to insert msg\" onClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + strLibraryFileName + "\', \'" + LibType + "\', " + myLibrary.Id + ");Blink(\'" + TdName + "\', \'#d0e5f5\');return false;\" OnDblClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + strLibraryFileName + "\', \'" + LibType + "\');SubmitInsert();\"" + " >";
						dr["ITEM2"] += Convert.ToString(myLibrary.Id);
						dr["ITEM2"] += "</a>";
						dr["ITEM2"] += "</LAYER>";
						dr["ITEM2"] += "</ILAYER>";
						
						dr["ITEM3"] = "<ILAYER name=\"layer" + TdName + "_1" + ">";
						dr["ITEM3"] += "<LAYER width=\"100%\">";
						dr["ITEM3"] += "<a href=\"#\" title=\"double click to insert msg\" onClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + strLibraryFileName + "\', \'" + LibType + "\', " + myLibrary.Id + ");;Blink(\'" + TdName + "\', \'#d0e5f5\');return false;\" OnDblClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + Convert.ToString(myLibrary.Title).Replace("\'", "\\\'") + "\', \'" + strLibraryFileName + "\', \'" + LibType + "\');SubmitInsert();\"" + " >";
						dr["ITEM3"] += myLibrary.DisplayLastEditDate;
						dr["ITEM3"] += "</a>";
						dr["ITEM3"] += "</LAYER>";
						dr["ITEM3"] += "</ILAYER>";
						dt.Rows.Add(dr);
						
						if (LibType == "images")
						{
							//StagingFileName property would be empty for non-multisite links and as well as when the site is not in Staging Mode
							if (myLibrary.StagingFileName == "")
							{
								myLibrary.StagingFileName = myLibrary.FileName;
							}
							sDesLoc = myLibrary.StagingFileName;
							myfilePathArray = Strings.Split(sDesLoc.ToString(), "/", -1, 0);
							myImagefile = myfilePathArray.GetValue(myfilePathArray.Length - 1).ToString();
							
							sExt = Strings.Right(myImagefile, 3);
							if ("gif" == sExt)
							{
								sExt = "png";
								myNewImagefile = "thumb_" + Strings.Left(myImagefile.ToString(), System.Convert.ToInt32(Strings.Len(myImagefile) - 3)) + "png";
							}
							else
							{
								myNewImagefile = "thumb_" + myImagefile;
							}
							try
							{
								sDesLoc = Strings.Replace(sDesLoc.ToString(), myImagefile.ToString(), myNewImagefile.ToString(), 1, -1, 0);
								fs = new FileInfo(Server.MapPath(sDesLoc.ToString()));
								if (fs.Exists)
								{
									dr = dt.NewRow();
									dr["ITEM1"] = "<a href=\"#\" title=\"" + m_refMsg.GetMessage("double click to insert msg") + "\" onClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + myLibrary.Title.Replace("\'", "\\\'") + "\', \'" + myLibrary.FileName.Replace("\'", "\\\'") + "\', \'" + LibType + "\', " + myLibrary.Id + ");Blink(\'" + TdName + "\', \'#d0e5f5\');return false;\" OnDblClick=\"javascript:Insert(\'" + myLibrary.Id + "\', \'" + folder + "\', \'" + myLibrary.Title.Replace("\'", "\\\'") + "\', \'" + myLibrary.FileName.Replace("\'", "\\\'") + "\', \'" + LibType + "\');SubmitInsert();\"><img src=\"" + sDesLoc + "\" border=0 width=125></a><hr>";
									dr["ITEM2"] = "REMOVE_ITEM";
									dr["ITEM3"] = "REMOVE_ITEM";
									dt.Rows.Add(dr);
								}
							}
							catch (Exception)
							{
								
							}
						}
						i++;
					}
				}
			}
			
			DataView dv = new DataView(dt);
			MediaListGrid.DataSource = dv;
			MediaListGrid.DataBind();
		}
		private void ViewLibraryByCategoryToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("library folder title bar") + " \"" + cFolder.Name + "\\" + LibType + "\""));
			if (m_refContentApi.RequestInformationRef.EnableReplication && (cFolder.ReplicationMethod == 1))
			{
				divTitleBar.InnerHtml += "&nbsp;(QuickDeploy)";
			}
			
			result.Append("<table><tr>");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/bookInsert.png", "#", m_refMsg.GetMessage("alt insert button text"), m_refMsg.GetMessage("btn insert"), "onClick=\"javascript:showSelAliasdialog();return false;\"", StyleHelper.InsertBookButtonCssClass,true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/bookView.png", "#", m_refMsg.GetMessage("alt preview button text (library2)"), m_refMsg.GetMessage("btn preview"), "onClick=\"javascript:previewItem(\'" + LibType + "\');return false;\"", StyleHelper.ViewBookButtonCssClass));
			
			if (Request.QueryString["scope"] != "images")
			{
				result.Append("<td>" + "\r\n");
				result.Append("<script language=\"javascript\">" + "\r\n");
				result.Append("function ChangeLibraryType(SelObj)" + "\r\n");
				result.Append("{	" + "\r\n");
				result.Append("document.location.href = \"libraryinsert.aspx?SelectAll=1&LangType=" + m_intContentLanguage + "&action=ViewLibraryByCategory" + "&fullscreen=off&scope=" + EkFunctions.HtmlEncode(Request.QueryString["scope"]) + "&id=" + folder + "&type=\" + SelObj.value;" + "\r\n");
				result.Append("}" + "\r\n");
				result.Append("</script>" + "\r\n");
				result.Append("<select onchange=\'ChangeLibraryType(this)\' align=right id=\"LibType\" name=\"LibType\">" + "\r\n");
				result.Append("<option " + IsSelected("files") + " value=\"files\">Files</option>" + "\r\n");
				if (Request.QueryString["scope"] != "images,hLink,files")
				{
					result.Append("<option " + IsSelected("forms") + " value=\"forms\">Forms</option>" + "\r\n");
				}
				result.Append("<option " + IsSelected("hyperlinks") + " value=\"hyperlinks\">Hyperlinks</option>" + "\r\n");
				result.Append("<option " + IsSelected("images") + " value=\"images\">Images</option>" + "\r\n");
				if (Request.QueryString["scope"] != "images,hLink,files")
				{
					result.Append("<option " + IsSelected("quicklinks") + " value=\"quicklinks\">Quicklinks</option>" + "\r\n");
				}
				result.Append("</select>" + "\r\n");
				result.Append("</td>" + "\r\n");
			}
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		private string IsSelected(string ret)
		{
			if (LibType == ret)
			{
				return (" selected ");
			}
			else
			{
				return "";
			}
		}
		public string getPhysicalPath(string path)
		{
			string returnValue;
			returnValue = Server.MapPath(path);
			Microsoft.VisualBasic.CompilerServices.ProjectData.ClearProjectError();
			return returnValue;
		}
		
		public bool DoesFileExist(string fileName)
		{
			bool returnValue =false;
			FileInfo fInfo;
            try
            {

                fInfo = new FileInfo(Server.MapPath(fileName));
                returnValue = fInfo.Exists;
                fInfo = null;
                return returnValue;
            }
            catch (Exception)
            {
                returnValue = false;
                fInfo = null;
                return returnValue;
            }
		}
		protected void MediaListGrid_ItemBound(object sender, DataGridItemEventArgs e)
		{
			if (action.ToLower() == "viewlibrarybycategory")
			{
				switch (e.Item.ItemType)
				{
					case ListItemType.AlternatingItem:
					case ListItemType.Item:
						
						if (e.Item.Cells[1].Text.Equals("REMOVE_ITEM") && e.Item.Cells[2].Text.Equals("REMOVE_ITEM"))
						{
							e.Item.Cells[0].ColumnSpan = 3;
							e.Item.Cells.RemoveAt(2);
							e.Item.Cells.RemoveAt(1);
						}
						else
						{
							e.Item.Cells[0].Attributes.Add("id", arr_TdName[j]);
							e.Item.Cells[1].Attributes.Add("id", (string) (arr_TdName[j] + "_0"));
							e.Item.Cells[2].Attributes.Add("id", (string) (arr_TdName[j] + "_1"));
							j++;
						}
						break;
				}
			}
		}
		
		protected void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					_currentPageNumber = 1;
					break;
				case "Last":
                    _currentPageNumber = int.Parse(TotalPages.Text);
					break;
				case "Next":
                    _currentPageNumber = System.Convert.ToInt32(int.Parse(CurrentPage.Text) + 1);
					break;
				case "Prev":
                    _currentPageNumber = System.Convert.ToInt32(int.Parse(CurrentPage.Text) - 1);
					break;
			}
			Display_ViewLibraryByCategory();
		}
		public void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/empjsfunc.js", "EktronEmpJsFuncJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
			Packages.EktronCoreJS.Register(this);
            Packages.Ektron.StringObject.Register(this);
			Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.ApplicationPath + "java/plugins/modal/ektron.modal.js", "modaljs");
		}
	}
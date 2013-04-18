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
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Workarea;


	public partial class product_type : workareabase
	{
		
		
		#region Member Variables
		
		protected ContentAPI _ContentApi = new ContentAPI();
		protected string _PageName = "producttypes.aspx";
		protected string _PageAction = "";
		protected CommonApi _CommonApi = new CommonApi();
		protected string _StyleSheetJS = "";
		protected ProductTypeData _ProductTypeData;
		protected ProductType _ProductType = null;
		protected int _SelectedAttributeIndex = -1;
		protected bool _IsUsed = false;
		protected int _CurrentPage = 1;
		protected int _TotalPages = 1;
		protected EkMessageHelper _MessageHelper;
		private string _ApplicationPath;
		private string _SitePath;
		
		#endregion
		
		#region Properties
		
		private string ApplicationPath
		{
			get
			{
				return _ApplicationPath;
			}
			set
			{
				_ApplicationPath = value;
			}
		}
		
		private string SitePath
		{
			get
			{
				return _SitePath;
			}
			set
			{
				_SitePath = value;
			}
		}
		
		#endregion
		
		#region Events
		
		protected product_type()
		{
			
			char[] slash = new char[] {'/'};
			this.SitePath = _ContentApi.SitePath.TrimEnd(slash.ToString().ToCharArray());
			this.ApplicationPath = _ContentApi.ApplicationPath.TrimEnd(slash.ToString().ToCharArray());
			
		}
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			
			//register page resources
			this.RegisterJS();
			this.RegisterCSS();
			
			_MessageHelper = _ContentApi.EkMsgRef;
			
			if (!string.IsNullOrEmpty(Request.QueryString["id"]))
			{
				_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
				ProductTypeData myProductTypeData = _ProductType.GetItem(Convert.ToInt64(Request.QueryString["id"]), true);
				
				this.ucAttributes.ProductData = myProductTypeData;
				this.ucAttributes.DisplayMode = Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.View;
				this.ucAttributesEdit.ProductData = myProductTypeData;
				this.ucAttributesEdit.DisplayMode = Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.Edit;
				
				this.ucMediaDefaults.ProductData = myProductTypeData;
				this.ucMediaDefaults.DisplayMode = Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.MediaDefaults.DisplayModeValue.View;
				this.ucMediaDefaultsEdit.ProductData = myProductTypeData;
				this.ucMediaDefaultsEdit.DisplayMode = Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.MediaDefaults.DisplayModeValue.Edit;
			}
			
			if (Request.QueryString["action"] == "addproducttype")
			{
				this.ucAttributesEdit.DisplayMode = Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.Attributes.DisplayModeValue.Edit;
				this.ucMediaDefaultsEdit.DisplayMode = Ektron.Cms.Commerce.Workarea.ProductTypes.Tabs.MediaDefaults.DisplayModeValue.Edit;
			}
			
		}
		
		protected override void Page_Load(System.Object sender, System.EventArgs e)
		{
			base.Page_Load(sender, e);
			if (! Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
			{
				Utilities.ShowError(GetMessage("feature locked error"));
			}
			if (! Utilities.ValidateUserLogin())
			{
				return;
			}
			
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			if (!(Request.QueryString["action"] == null))
			{
				_PageAction = Request.QueryString["action"];
				if (_PageAction.Length > 0)
				{
					_PageAction = _PageAction.ToLower();
				}
			}
			if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
			{
				ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
				_CommonApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
			}
			else
			{
				if (_CommonApi.GetCookieValue("LastValidLanguageID") != "")
				{
					ContentLanguage = int.Parse(_CommonApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			_CommonApi.ContentLanguage = ContentLanguage;
			Page.Header.Controls.Add(new LiteralControl(m_refStyle.GetClientScript()));
		}
		
		protected override void Page_PreRender(object sender, System.EventArgs e)
		{
			try
			{
			    base.Page_PreRender(sender, e);
				Util_CheckAccess();
				mvViews.SetActiveView(vwViewAddEdit);
				switch (this._PageAction)
				{
					case "addthumbnail":
						Display_AddThumbnail();
						break;
					case "addattribute":
					case "editattribute":
						Display_ProductAttribute();
						break;
					case "viewproducttype":
						Display_ViewProductType();
						break;
					case "addproducttype":
						if (Page.IsPostBack)
						{
							Process_AddProductType();
						}
						else
						{
							Display_AddProductType();
						}
						break;
					case "editproducttype":
						if (Page.IsPostBack)
						{
							Process_EditProductType();
						}
						else
						{
							Display_EditProductType();
						}
						break;
					case "deleteproducttype":
						_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
						_ProductType.Delete(Convert.ToInt64(Request.QueryString["id"]));
						Response.Redirect(_PageName + "?action=viewallproducttypes", false);
						break;
					default: // "viewallproducttypes"
						mvViews.SetActiveView(vwViewAll);
						if (Page.IsPostBack == false)
						{
							Display_ViewAll();
						}
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		
		protected void NavigationLink_Click(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "First":
					_CurrentPage = 1;
					break;
				case "Last":
					_CurrentPage = int.Parse((string) litTotalPages.Text);
					break;
				case "Next":
					_CurrentPage = System.Convert.ToInt32((int.Parse((string) txtCurrentPage.Text) + 1 <= int.Parse((string) litTotalPages.Text)) ? (int.Parse((string) txtCurrentPage.Text) + 1) : (int.Parse((string) txtCurrentPage.Text)));
					break;
				case "Prev":
					_CurrentPage = System.Convert.ToInt32((int.Parse((string) txtCurrentPage.Text) - 1 >= 1) ? (int.Parse((string) txtCurrentPage.Text) - 1) : (int.Parse((string) txtCurrentPage.Text)));
					break;
			}
			txtCurrentPage.Text = _CurrentPage.ToString();
			hdnCurrentPage.Value = _CurrentPage.ToString();
			
			Display_ViewAll();
		}
		
		protected void AdHocPaging_Click(object sender, CommandEventArgs e)
		{
			_CurrentPage = int.Parse((string) this.txtCurrentPage.Text);
			
			txtCurrentPage.Text = _CurrentPage.ToString();
			hdnCurrentPage.Value = _CurrentPage.ToString();
			
			Display_ViewAll();
		}
		
		protected void DisplayGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			switch (e.Item.ItemType)
			{
				case ListItemType.AlternatingItem:
				case ListItemType.Item:
					if (e.Item.Cells[1].Text.Equals("REMOVE-ITEM"))
					{
						e.Item.Cells[0].ColumnSpan = 2;
						e.Item.Cells[0].CssClass = "label";
						e.Item.Cells.RemoveAt(1);
					}
					else
					{
						if (e.Item.Cells[1].Text.Contains("ektronGrid"))
						{
							e.Item.Cells[1].CssClass = "noPadding";
						}
					}
					break;
			}
		}
		
		#endregion
		
		#region Display
		
		protected void Display_AddThumbnail()
		{
			phAddThumbnail.Visible = true;
			_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
			if (this.m_iID > 0)
			{
				_ProductTypeData = _ProductType.GetItem(m_iID, true);
			}
			
			Util_SetJs();
			Util_SetLabels();
		}
		
		protected void Display_ProductAttribute()
		{
			phAddEditAttributes.Visible = true;
			if (m_iID > 0)
			{
				string AttributeName = (string) ((Request.QueryString["name"] != "") ? (Request.QueryString["name"]) : "");
				string AttributeTagType = (string) ((Request.QueryString["type"] != "") ? (Request.QueryString["type"]) : "");
				string AttributeDefaultText = (string) ((Request.QueryString["def"] != "") ? (Request.QueryString["def"]) : "");
				txt_attrname.Text = AttributeName;
				switch (AttributeTagType)
				{
					case "text":
						txt_textdefault.Text = AttributeDefaultText;
						divNum.Style.Add("position", "absolute");
						divNum.Style.Add("visibility", "hidden");
						divChk.Style.Add("position", "absolute");
						divChk.Style.Add("visibility", "hidden");
						_SelectedAttributeIndex = 0;
						break;
					case "boolean":
						chk_bool.Checked = EkFunctions.GetBoolFromYesNo(AttributeDefaultText);
						divText.Style.Add("position", "absolute");
						divText.Style.Add("visibility", "hidden");
						divNum.Style.Add("position", "absolute");
						divNum.Style.Add("visibility", "hidden");
						_SelectedAttributeIndex = 2;
						break;
					default:
						txt_number.Text = AttributeDefaultText;
						divText.Style.Add("position", "absolute");
						divText.Style.Add("visibility", "hidden");
						divChk.Style.Add("position", "absolute");
						divChk.Style.Add("visibility", "hidden");
						switch (AttributeTagType)
						{
							case "number":
								_SelectedAttributeIndex = 1;
								break;
							case "byte":
								_SelectedAttributeIndex = 1;
								break;
							case "double":
								_SelectedAttributeIndex = 1;
								break;
							case "float":
								_SelectedAttributeIndex = 1;
								break;
							case "integer":
								_SelectedAttributeIndex = 1;
								break;
							case "long":
								_SelectedAttributeIndex = 1;
								break;
							case "short":
								_SelectedAttributeIndex = 1;
								break;
						}
						break;
				}
			}
			else
			{
				divNum.Style.Add("position", "absolute");
				divNum.Style.Add("visibility", "hidden");
				divChk.Style.Add("position", "absolute");
				divChk.Style.Add("visibility", "hidden");
			}
			Util_SetJs();
			Util_SetLabels();
		}
		
		protected void Display_ViewAll()
		{
			_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
			List<ProductTypeData> producttypeList = new List<ProductTypeData>();
			Criteria<ProductTypeProperty> criteria = new Criteria<ProductTypeProperty>();
			
			criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
			criteria.PagingInfo.CurrentPage = _CurrentPage;
			
			producttypeList = _ProductType.GetList(criteria);
			
			_TotalPages = System.Convert.ToInt32(criteria.PagingInfo.TotalPages);
			
			if (_TotalPages <= 1)
			{
				divPaging.Visible = false;
			}
			else
			{
				this.SetPagingUI();
				divPaging.Visible = true;
				litTotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(_TotalPages))).ToString();
				txtCurrentPage.Text = _CurrentPage.ToString();
			}

            if (dgList.Columns != null && dgList.Columns.Count >= 4)
            {
                dgList.Columns[0].HeaderText = m_refMsg.GetMessage("generic title");
                dgList.Columns[1].HeaderText = m_refMsg.GetMessage("generic id");
                dgList.Columns[2].HeaderText = m_refMsg.GetMessage("lbl product type class");
                dgList.Columns[3].HeaderText = m_refMsg.GetMessage("generic date modified");
            }
			dgList.DataSource = producttypeList;
			dgList.DataBind();
			
			Util_SetLabels();
		}
		
		protected void Display_AddProductType()
		{
			_ProductTypeData = new ProductTypeData();
			phAddEdit.Visible = true;
			phTabAttributes.Visible = true;
			phTabMediaDefaults.Visible = true;
			trXslt.Visible = false;
			txtTitle.Attributes.Add("onkeypress", "return " + JSLibrary.CheckKeyValueName+ "(event, \'34,13\');");
			txtDescription.Attributes.Add("onkeypress", "return " + JSLibrary.CheckKeyValueName+ "(event, \'34,13\');");
			
			Util_XSLTLinks();
			Util_AddProductTypeItems(Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product);
			Util_SetJs();
			Util_SetLabels();
			
			drp_SubscriptionProvider.Items.Add(new ListItem("MembershipSubscriptionProvider"));
			this.drp_type.Attributes.Add("onchange", "if (this.selectedIndex == 3) {toggleSubscriptionRow(this, true);} else {toggleSubscriptionRow(this, false);}");
			
		}
		
		protected void Display_EditProductType()
		{
			_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
			_ProductTypeData = _ProductType.GetItem(m_iID, true);
			
			phTabAttributes.Visible = true;
			phTabMediaDefaults.Visible = true;
			
			tr_id.Visible = true;
			txt_id.Text = _ProductTypeData.Id.ToString();
			phAddEdit.Visible = true;
			txtTitle.Attributes.Add("onkeypress", "return " + JSLibrary.CheckKeyValueName+ "(event, \'34,13\');");
			txtDescription.Attributes.Add("onkeypress", "return " + JSLibrary.CheckKeyValueName+ "(event, \'34,13\');");
			
			Util_PopulateData();
			drp_type.Enabled = false;
			
			if (_ProductTypeData.EntryClass == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
			{
				
				drp_SubscriptionProvider.Items.Add(new ListItem("MembershipSubscriptionProvider"));
				drp_SubscriptionProvider.SelectedIndex = 0;
				
			}
			
			Util_XSLTLinks();
			Util_AddProductTypeItems(_ProductTypeData.EntryClass);
			Util_SetJs();
			Util_SetLabels();
		}
		
		protected void Display_ViewProductType()
		{
			_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
			_ProductTypeData = _ProductType.GetItem(m_iID, true);
			
			_IsUsed = _ProductType.IsProductTypeUsed(m_iID);
			
			tr_id.Visible = true;
			txt_id.Text = _ProductTypeData.Id.ToString();
			phAddEdit.Visible = false;
			phView.Visible = true;
			phTabAttributes.Visible = true;
			phTabMediaDefaults.Visible = true;
			txtTitle.Attributes.Add("onkeypress", "return " + JSLibrary.CheckKeyValueName+ "(event, \'34,13\');");
			txtDescription.Attributes.Add("onkeypress", "return " + JSLibrary.CheckKeyValueName+ "(event, \'34,13\');");
			
			XmlConfigData xml_config_data;
			xml_config_data = m_refContentApi.GetXmlConfiguration(m_iID);
			PopulatePropertiesGrid(xml_config_data, Convert.ToInt64(_ProductTypeData.EntryClass), _ProductTypeData.SubscriptionProvider);
			PopulateDisplayGrid(xml_config_data);
			if (xml_config_data.PackageDisplayXslt.Length > 0)
			{
				PopulatePreviewGrid(xml_config_data);
				phPreview.Visible = true;
				phTabPreview.Visible = true;
			}
			else
			{
				phTabPreview.Visible = false;
				phPreview.Visible = false;
			}
			
			Util_PopulateData();
			Util_XSLTLinks();
			Util_AddProductTypeItems(_ProductTypeData.EntryClass);
			Util_SetJs();
			Util_SetLabels();
		}
		
		private void PopulatePropertiesGrid(XmlConfigData xml_config_data, long entryId, string subscriptionProvider)
		{
            Collection xml_data_logical_path = new Collection();
            xml_data_logical_path = (Collection)xml_config_data.LogicalPathComplete;
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.ItemStyle.CssClass = "label";
			PropertiesGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "VALUE";
			PropertiesGrid.Columns.Add(colBound);
			
			DataTable dt = new DataTable();
			DataRow dr;
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("VALUE", typeof(string)));
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("generic title label");
			dr[1] = xml_config_data.Title;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("id label");
			dr[1] = xml_config_data.Id;
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("lbl product type class");
			dr[1] = Util_ShowType(entryId);
			dt.Rows.Add(dr);
			
			if (entryId == Convert.ToInt64(EkEnumeration.CatalogEntryType.SubscriptionProduct))
			{
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("lbl commerce subscription provider");
				dr[1] = subscriptionProvider;
				dt.Rows.Add(dr);
			}
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("description label");
			dr[1] = xml_config_data.Description;
			dt.Rows.Add(dr);
			
			if (xml_config_data.PackageDisplayXslt.Length > 0)
			{
				//dr = dt.NewRow
				//dr(0) = ""
				//dr(1) = "REMOVE-ITEM"
				//dt.Rows.Add(dr)
			}
			else
			{
				
				StringBuilder sb = new StringBuilder();
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("editor info label");
				sb.Append(" <div class=\"innerTable\">");
				sb.Append("     <table>");
				sb.Append("         <tbody>");
				sb.Append("             <tr>");
				sb.Append("                 <th>" + m_refMsg.GetMessage("edit xslt label") + "</th>");
                sb.Append("                 <td>" + ((xml_data_logical_path["EditXslt"].ToString() == string.Empty) ? "-" : (xml_data_logical_path["EditXslt"])) + "</td>");
				sb.Append("             </tr>");
				sb.Append("             <tr>");
				sb.Append("                 <th>" + m_refMsg.GetMessage("save xslt label") + "</th>");
                sb.Append("                 <td>" + ((xml_data_logical_path["SaveXslt"].ToString() == string.Empty) ? "-" : (xml_data_logical_path["SaveXslt"])) + "</td>");
				sb.Append("             </tr>");
				sb.Append("             <tr>");
				sb.Append("                 <th>" + m_refMsg.GetMessage("advanced config label") + "</th>");
                sb.Append("                 <td>" + ((xml_data_logical_path["XmlAdvConfig"].ToString() == string.Empty) ? "-" : (xml_data_logical_path["XmlAdvConfig"])) + "</td>");
				sb.Append("             </tr>");
				sb.Append("         </tbody>");
				sb.Append("     </table>");
				sb.Append(" </div>");
				
				dr[1] = sb.ToString();
				dt.Rows.Add(dr);
				
				dr = dt.NewRow();
				dr[0] = m_refMsg.GetMessage("validation info label");
				
				sb = new StringBuilder();
				sb.Append(" <div class=\"innerTable\">");
				sb.Append("     <table");
				sb.Append("         <tbody>");
				sb.Append("             <tr>");
				sb.Append("                 <th>" + m_refMsg.GetMessage("xml schema label") + "</th>");
                sb.Append("                 <td>" + ((xml_data_logical_path["XmlSchema"].ToString() == string.Empty) ? "-" : (xml_data_logical_path["XmlSchema"])) + "</td>");
				sb.Append("             </tr>");
				sb.Append("             <tr>");
				sb.Append("                 <th>" + m_refMsg.GetMessage("target namespace label") + "</th>");
                sb.Append("                 <td>" + ((xml_data_logical_path["XmlNameSpace"].ToString() == string.Empty) ? "-" : (xml_data_logical_path["XmlNameSpace"])) + "</td>");
				sb.Append("             </tr>");
				sb.Append("         </tbody>");
				sb.Append("     </table>");
				sb.Append(" </div>");
				
				dr[1] = sb.ToString();
				dt.Rows.Add(dr);
				
			}
			
			DataView dv = new DataView(dt);
			PropertiesGrid.DataSource = dv;
			PropertiesGrid.DataBind();
		}
		
		private void PopulatePreviewGrid(XmlConfigData xml_config_data)
		{
            
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.HeaderText = m_refMsg.GetMessage("lbl Preview XSLT on empty XML document");
			colBound.ItemStyle.CssClass = "label left";
			colBound.ItemStyle.Wrap = false;
			colBound.HeaderStyle.Height = Unit.Empty;
			colBound.ItemStyle.Height = Unit.Empty;
			PreviewGrid.Columns.Add(colBound);
			
			DataTable dt = new DataTable();
			DataRow dr;
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			
			dr = dt.NewRow();
			dr[0] = m_refContentApi.XSLTransform("<root></root>", xml_config_data.PackageDisplayXslt, false, false, null, true, true);
			dt.Rows.Add(dr);
			
			DataView dv = new DataView(dt);
			PreviewGrid.DataSource = dv;
			PreviewGrid.DataBind();
		}
		
		private void PopulateDisplayGrid(XmlConfigData xml_config_data)
		{
            Collection xml_data_logical_path = new Collection();
            xml_data_logical_path = (Collection)xml_config_data.LogicalPathComplete;
			System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "TITLE";
			colBound.ItemStyle.Width = 200;
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "right";
            colBound.HeaderText = m_refMsg.GetMessage("lbl xslt");
			DisplayGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "VALUE";
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "left";
			colBound.HeaderText = m_refMsg.GetMessage("generic path");
			DisplayGrid.Columns.Add(colBound);
			
			colBound = new System.Web.UI.WebControls.BoundColumn();
			colBound.DataField = "DEFAULT";
			colBound.ItemStyle.Width = 50;
			colBound.HeaderStyle.CssClass = "center";
			colBound.ItemStyle.CssClass = "center";
			colBound.HeaderText = m_refMsg.GetMessage("lbl default");
			DisplayGrid.Columns.Add(colBound);
			
			DataTable dt = new DataTable();
			DataRow dr;
			bool bValidDefaultXslt = false;
			dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
			dt.Columns.Add(new DataColumn("VALUE", typeof(string)));
			dt.Columns.Add(new DataColumn("DEFAULT", typeof(string)));
			
			string defaultCheck = "<img alt=\"Default\" title=\"Default\" src=\"" + _ContentApi.ApplicationPath + "images/ui/icons/check.png\" />";
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("xslt 1 label");
			dr[2] = "&#160;";
			if (xml_config_data.DefaultXslt == "1")
			{
                if (xml_data_logical_path["Xslt1"].ToString() != "")
				{
					bValidDefaultXslt = true;
					dr[2] += defaultCheck;
				}
			}
            dr[1] = xml_data_logical_path["Xslt1"];
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("xslt 2 label");
			dr[2] = "&#160;";
			if (xml_config_data.DefaultXslt == "2")
			{
                if (xml_data_logical_path["Xslt2"].ToString() != "")
				{
					bValidDefaultXslt = true;
					dr[2] += defaultCheck;
				}
			}
            dr[1] = xml_data_logical_path["Xslt2"];
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("xslt 3 label");
			dr[2] = "&#160;";
			if (xml_config_data.DefaultXslt == "3")
			{
                if (xml_data_logical_path["Xslt3"].ToString() != "")
				{
					bValidDefaultXslt = true;
					dr[2] += defaultCheck;
				}
			}
            dr[1] = xml_data_logical_path["Xslt3"];
			dt.Rows.Add(dr);
			
			dr = dt.NewRow();
			dr[0] = m_refMsg.GetMessage("lbl XSLT packaged") + ":";
			dr[2] = "&#160;";
			if (xml_config_data.DefaultXslt == "0")
			{
				dr[2] += defaultCheck;
			}
			else
			{
				if (!(bValidDefaultXslt))
				{
					dr[2] += defaultCheck;
				}
			}
			dr[1] = "&nbsp;";
			dt.Rows.Add(dr);
			
			if (xml_config_data.PackageXslt.Length > 0)
			{
				//dr = dt.NewRow
				//dr(0) = ""
				//dr(1) = "REMOVE-ITEM"
				//dt.Rows.Add(dr)
				
				h3Xpaths.InnerText = "XPaths";
				
				string xPaths = string.Empty;
				
				int counter = 1;
				foreach (object item in m_refContentApi.GetXPaths(xml_config_data.PackageXslt))
				{
					xPaths += "<li" + (counter % 2 == 0 ? " class=\"stripe\">" : ">") + Convert.ToString(item) + "</li>";
					counter++;
				}
				litXpaths.Text = xPaths;
				
				//dr = dt.NewRow
				//dr(0) = "XPaths:"
				//dr(2) = "&#160;"
				//Dim item As Object
				//dr(1) = ""
				//For Each item In m_refContentApi.GetXPaths(xml_config_data.PackageXslt)
				//    dr(1) += Convert.ToString(item) & "<br/>"
				//Next
				//dt.Rows.Add(dr)
			}
			DataView dv = new DataView(dt);
			DisplayGrid.DataSource = dv;
			DisplayGrid.DataBind();
		}
		
		#endregion
		
		#region Process
		
		public List<ProductTypeAttributeData> Process_GetAttributes()
		{
			
			System.Collections.Generic.List<ProductTypeAttributeData> AttributeList = new System.Collections.Generic.List<ProductTypeAttributeData>();
			
			for (int i = 0; i <= (ucAttributesEdit.AttributeData.Count - 1); i++)
			{
				
				if (! ucAttributesEdit.AttributeData[i].MarkedForDelete)
				{
					
					ProductTypeAttributeData Attribute = new ProductTypeAttributeData();
					
					Attribute.Id = ucAttributesEdit.AttributeData[i].Id;
					Attribute.Name = (string) (ucAttributesEdit.AttributeData[i].Name);
					Attribute.DataType = Util_GetAttributeType((string) (ucAttributesEdit.AttributeData[i].Type));
					Attribute.DefaultValue = ucAttributesEdit.AttributeData[i].Value;
					
					AttributeList.Add(Attribute);
					
				}
				
			}
			
			return AttributeList;
			
		}
		
		public List<ThumbnailDefaultData> Process_GetThumbnails()
		{
			
			System.Collections.Generic.List<ThumbnailDefaultData> thumbnailDefaultList = new System.Collections.Generic.List<ThumbnailDefaultData>();
			
			if (ucMediaDefaultsEdit.ClientData != null)
			{
				
				for (int i = 0; i <= (ucMediaDefaultsEdit.ClientData.Count - 1); i++)
				{
					
					if (! ucMediaDefaultsEdit.ClientData[i].MarkedForDelete)
					{
						
						ThumbnailDefaultData thumbnailData = new ThumbnailDefaultData();
						thumbnailData.Id = ucMediaDefaultsEdit.ClientData[i].Id;
						thumbnailData.Title = (string) (ucMediaDefaultsEdit.ClientData[i].Name);
						thumbnailData.Width = System.Convert.ToInt32(ucMediaDefaultsEdit.ClientData[i].Width);
						thumbnailData.Height = System.Convert.ToInt32(ucMediaDefaultsEdit.ClientData[i].Height);
						
						thumbnailDefaultList.Add(thumbnailData);
						
					}
					
				}
				
			}
			
			return thumbnailDefaultList;
			
		}
		
		public void Process_EditProductType()
		{
			try
			{
				_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
				_ProductTypeData = _ProductType.GetItem(m_iID, true);
				
				_ProductTypeData.Title = (string) txtTitle.Text;
				_ProductTypeData.Description = (string) txtDescription.Text;
				_ProductTypeData.DefaultXslt = (string) (Request.Form["frm_xsltdefault"].Replace("frm_xsltdefault", ""));
				_ProductTypeData.Xslt1 = (string) txt_xslt1.Text;
				_ProductTypeData.Xslt2 = (string) txt_xslt2.Text;
				_ProductTypeData.Xslt3 = (string) txt_xslt3.Text;
				_ProductTypeData.PhysicalPath = Server.MapPath(m_refContentApi.XmlPath);
				_ProductTypeData.Attributes = Process_GetAttributes();
				_ProductTypeData.DefaultThumbnails = Process_GetThumbnails();
				if (_ProductTypeData.EntryClass == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
				{
					_ProductTypeData.SubscriptionProvider = Request.Form["drp_SubscriptionProvider"];
				}
				
				_ProductType.Update(_ProductTypeData);
				Response.Redirect(_PageName + "?action=viewproducttype&id=" + m_iID, false);
				
			}
			catch (Ektron.Cms.Exceptions.SpecialCharactersException)
			{
				
				trError.Visible = true;
				litErrorMessage.Text = string.Format(GetMessage("js alert product type title cant include"), EkFunctions.HtmlEncode("<,>"));
				Display_EditProductType();
				
			}
			catch (Exception ex)
			{
				trError.Visible = true;
				litErrorMessage.Text = ex.Message.ToString();
				Display_EditProductType();
				// Utilities.ShowError()
				
			}
		}
		
		public void Process_AddProductType()
		{
			
			try
			{
				
				_ProductType = new ProductType(m_refContentApi.RequestInformationRef);
				_ProductTypeData = new ProductTypeData();

                _ProductTypeData.EntryClass = (EkEnumeration.CatalogEntryType)Enum.Parse(typeof(EkEnumeration.CatalogEntryType),Request.Form[drp_type.UniqueID]);
				_ProductTypeData.Title = Request.Form[txtTitle.UniqueID];
				_ProductTypeData.Description = Request.Form[txtDescription.UniqueID];
				_ProductTypeData.EditXslt = "";
				_ProductTypeData.SaveXslt = "";
				_ProductTypeData.Xslt1 = "";
				_ProductTypeData.Xslt2 = "";
				_ProductTypeData.Xslt3 = "";
				_ProductTypeData.Xslt4 = "";
				_ProductTypeData.Xslt5 = "";
				_ProductTypeData.XmlSchema = "";
				_ProductTypeData.XmlNameSpace = "";
				_ProductTypeData.XmlAdvConfig = "";
				_ProductTypeData.DefaultXslt = "0";
				_ProductTypeData.PhysicalPath = Server.MapPath(m_refContentApi.XmlPath);
				_ProductTypeData.Attributes = Process_GetAttributes();
				_ProductTypeData.DefaultThumbnails = Process_GetThumbnails();
				if (_ProductTypeData.EntryClass == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
				{
					_ProductTypeData.SubscriptionProvider = Request.Form["drp_SubscriptionProvider"];
				}
				
				_ProductTypeData = _ProductType.Add(_ProductTypeData);
				Response.Redirect((string) ("../editdesign.aspx?action=EditPackage&type=product&id=" + _ProductTypeData.Id.ToString()), false);
				
			}
			catch (Ektron.Cms.Exceptions.SpecialCharactersException)
			{
				
				trError.Visible = true;
				litErrorMessage.Text = string.Format(GetMessage("js alert product type title cant include"), EkFunctions.HtmlEncode("<,>"));
				
				Display_AddProductType();
				
			}
			catch (Exception ex)
			{
				
				trError.Visible = true;
				litErrorMessage.Text = ex.Message.ToString();
				Display_AddProductType();
				
			}
			
		}
		
		#endregion
		
		#region Helpers
		
		private void SetPagingUI()
		{
			
			//paging ui
			divPaging.Visible = true;
			
			litPage.Text = "Page";
			txtCurrentPage.Text = _CurrentPage == 0 ? "1" : (_CurrentPage.ToString());
			litOf.Text = "of";
			litTotalPages.Text = _TotalPages.ToString();
			
			hdnCurrentPage.Value = _CurrentPage == 0 ? "1" : (_CurrentPage.ToString());
			
			ibFirstPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadFirst.png";
			ibFirstPage.AlternateText = "First Page";
			ibFirstPage.ToolTip = "First Page";
			
			ibPreviousPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadLeft.png";
			ibPreviousPage.AlternateText = "Previous Page";
			ibPreviousPage.ToolTip = "Previous Page";
			
			ibNextPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadRight.png";
			ibNextPage.AlternateText = "Next Page";
			ibNextPage.ToolTip = "Next Page";
			
			ibLastPage.ImageUrl = this.ApplicationPath + "/images/ui/icons/arrowheadLast.png";
			ibLastPage.AlternateText = "Last Page";
			ibLastPage.ToolTip = "Last Page";
			
			ibPageGo.ImageUrl = this.ApplicationPath + "/images/ui/icons/forward.png";
			ibPageGo.AlternateText = "Go To Page";
			ibPageGo.ToolTip = "Go To Page";
			ibPageGo.OnClientClick = " return GoToPage(this);";
			
		}
		
		protected void Util_SetLabels()
		{
			char[] endColon = new char[] {':'};
			
			litTitleLabel.Text = GetMessage("generic title") + ":";
			litIdLabel.Text = GetMessage("generic id").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			litDescriptionLabel.Text = GetMessage("generic description").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			litTypeLabel.Text = GetMessage("lbl product type class") + ":";
			litDisplayLabel.Text = GetMessage("display info label").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			litDisplayXsltPathMessage.Text = GetMessage("files prefixed with msg") + " " + m_refContentApi.XmlPath;
			ltr_deflabel.Text = GetMessage("default label").TrimEnd(endColon.ToString().ToCharArray());
			litXslt1Label.Text = GetMessage("xslt 1 label").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			litXslt2Label.Text = GetMessage("xslt 2 label").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			litXslt3Label.Text = GetMessage("xslt 3 label").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			litXsltDefaultLabel.Text = "XSLT 0:";
			ltr_def.Text = GetMessage("default label");
			ltr_attrtype.Text = GetMessage("type label");
			ltr_attrname.Text = GetMessage("generic name");
			ltr_name.Text = GetMessage("lbl name").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			ltr_width.Text = GetMessage("lbl width").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			ltr_height.Text = GetMessage("lbl height").TrimEnd(endColon.ToString().ToCharArray()) + ":";
			ltr_subprovider.Text = GetMessage("lbl commerce subscription provider");
			
			drp_attrtype.Items.Add(new ListItem(Util_AttrText(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.String)), "text"));
			drp_attrtype.Items.Add(new ListItem(Util_AttrText(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric)), "number"));
			drp_attrtype.Items.Add(new ListItem(Util_AttrText(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Boolean)), "boolean"));
			drp_attrtype.Attributes.Add("onchange", "ChangeOption(this);");
			if (_SelectedAttributeIndex > -1)
			{
				drp_attrtype.SelectedIndex = _SelectedAttributeIndex;
			}
			txt_xslt1.Attributes.Add("onkeyup", "MakeNoVerify(this);");
			txt_xslt1.Attributes.Add("onkeypress", "return CheckKeyValue(event,\'34\');");
			txt_xslt2.Attributes.Add("onkeyup", "MakeNoVerify(this);");
			txt_xslt2.Attributes.Add("onkeypress", "return CheckKeyValue(event,\'34\');");
			txt_xslt3.Attributes.Add("onkeyup", "MakeNoVerify(this);");
			txt_xslt3.Attributes.Add("onkeypress", "return CheckKeyValue(event,\'34\');");
			switch (this.m_sPageAction)
			{
				case "addthumbnail":
					System.Text.StringBuilder result = new System.Text.StringBuilder();
					this.SetTitleBarToMessage("lbl add thumbnail default");
					ltr_addthumbnail.Text = GetMessage("lbl add thumbnail default");
					
					result.Append("<table><tr>" + Environment.NewLine);
					string buttonId = Guid.NewGuid().ToString();
					result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"action\">" + m_refMsg.GetMessage("lbl Action") + "</span></td>" + Environment.NewLine);
					result.Append("</tr></table>" + Environment.NewLine);
					result.Append("<script tyle=\"text/javascript\">" + Environment.NewLine);
					result.Append("    var actionmenu = new Menu( \"action\" );" + Environment.NewLine);
					result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + m_refContentApi.AppPath + "images/UI/Icons/save.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("generic add title") + "\", function() { OnClick=validate();  } );" + Environment.NewLine);
					result.Append("    actionmenu.addItem(\"&nbsp;<img height=\'16px\' width=\'16px\' src=\'" + m_refContentApi.AppPath + "images/UI/Icons/delete.png" + " \' />&nbsp;&nbsp;" + m_refMsg.GetMessage("generic cancel") + "\", function() { OnClick=Close();  } );" + Environment.NewLine);
					result.Append("    MenuUtil.add( actionmenu );" + Environment.NewLine);
					result.Append("</script>" + Environment.NewLine);
					break;
				case "addattribute":
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.BackButtonCssClass, true);
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "btn save", "btn save", "Onclick=\"SubmitAttrForm(); return false;\"", StyleHelper.SaveButtonCssClass, true);
					AddHelpButton(m_sPageAction);
					break;
				case "editattribute":
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.BackButtonCssClass, true);
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "alt update product type", "btn update", "Onclick=\"SubmitAttrForm(); return false;\"", StyleHelper.SaveButtonCssClass, true);
					AddHelpButton(m_sPageAction);
					break;
				case "addproducttype":
					
					SetTitleBarToMessage("btn add product type");

					AddBackButton(_PageName);
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "lbl Select to continue", "btn save", "Onclick=\"SubmitForm(); return false;\"", StyleHelper.SaveButtonCssClass, true);
					AddHelpButton(m_sPageAction);
					break;
					
				case "editproducttype":
					
					SetTitleBarToMessage("lbl edit product type");

					AddBackButton(_PageName + "?action=viewproducttype&id=" + m_iID.ToString());
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", "alt update product type", "btn update", "onclick=\"SubmitForm(); return false;\"", StyleHelper.SaveButtonCssClass, true);
					AddHelpButton(m_sPageAction);
					break;
					
				case "viewproducttype":
					
					XmlConfigData product_type_data = null;
					string pkDisplay = string.Empty;
					string PackageXslt = string.Empty;
					
					SetTitleBarToMessage("lbl view product type msg");
					
					phTabDisplayInfo.Visible = true;
					
					product_type_data = m_refContentApi.GetXmlConfiguration(m_iID);
					if (product_type_data != null)
					{
						pkDisplay = product_type_data.PackageDisplayXslt;
						PackageXslt = product_type_data.PackageXslt;
					}
					AddBackButton(_PageName);
					AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentEdit.png", (string) ("producttypes.aspx" + "?action=editproducttype&id=" + m_iID), "alt edit button text (xml config)", "btn edit", "", StyleHelper.EditButtonCssClass, true);
					if (! _IsUsed)
					{
						AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/delete.png", (string) ("producttypes.aspx" + "?action=deleteproducttype&id=" + m_iID), "generic delete title", "generic delete title", " onclick=\"return confirm(\'" + GetMessage("js confirm del product type") + "\');\" ", StyleHelper.DeleteButtonCssClass);
					}
					
					
					if (product_type_data != null)
					{
                        if ((product_type_data.EditXslt.Length == 0) || pkDisplay.Length > 0)
						{
							AddButtonwithMessages(m_refContentApi.AppPath + "images/UI/Icons/contentFormEdit.png", (string) ("../editdesign.aspx?action=EditPackage&type=product&id=" + m_iID), "alt Design mode Package", "btn data design", "", StyleHelper.EditContentFormButtonCssClass);
						}
					}

                    if (pkDisplay.Length > 0)
					{
						AddButtonwithMessages(m_refContentApi.AppPath + "Images/ui/icons/FileTypes/xsl.png", (string) ("../viewXslt.aspx?id=" + m_iID), "alt View the presentation Xslt", "btn view xslt", "", StyleHelper.ViewXslButtonCssClass);
					}
					
					AddHelpButton(m_sPageAction);
					break;
					
				default:
					
					SetTitleBarToMessage("lbl view product types");
					
					workareamenu newMenu = new workareamenu("file", GetMessage("lbl new"), m_refContentApi.AppPath + "images/UI/Icons/star.png");
					newMenu.AddLinkItem(AppImgPath + "commerce/producttype1.gif", GetMessage("lbl product type xml config"), _PageName + "?action=addproducttype");
					AddMenu(newMenu);
					AddHelpButton("viewproducttypes");

					break;
					
			}
		}
		
		protected void Util_PopulateData()
		{
			
			txtTitle.Text = _ProductTypeData.Title;
			txtDescription.Text = _ProductTypeData.Description;
			if (_ProductTypeData.DefaultXslt == "1" && _ProductTypeData.Xslt1 != "")
			{
				frm_xsltdefault1.Checked = true;
			}
			if (_ProductTypeData.DefaultXslt == "2" && _ProductTypeData.Xslt2 != "")
			{
				frm_xsltdefault2.Checked = true;
			}
			if (_ProductTypeData.DefaultXslt == "3" && _ProductTypeData.Xslt3 != "")
			{
				frm_xsltdefault3.Checked = true;
			}
			if (_ProductTypeData.DefaultXslt == "0")
			{
				frm_xsltdefault0.Checked = true;
			}
			txt_xslt1.Text = _ProductTypeData.Xslt1;
			txt_xslt2.Text = _ProductTypeData.Xslt2;
			txt_xslt3.Text = _ProductTypeData.Xslt3;
			
			if (!(_ProductTypeData.EntryClass == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct))
			{
				tr_provider.Visible = false;
			}
			
		}
		
		protected void Util_XSLTLinks()
		{
			ltr_verify1.Text = "<a href=\"#\" onclick=\"VerifyXslt(\'txt_xslt1\'); return false\"><img title=\"" + GetMessage("alt text for xsl or schema verification") + "\" alt=\"" + GetMessage("alt text for xsl or schema verification") + "\" src=\"" + this.ApplicationPath + "/images/ui/icons/contentValidate.png\" border=\"0\" name=\"img_txt_xslt1\"></a>";
			ltr_verify2.Text = "<a href=\"#\" onclick=\"VerifyXslt(\'txt_xslt2\'); return false\"><img title=\"" + GetMessage("alt text for xsl or schema verification") + "\" alt=\"" + GetMessage("alt text for xsl or schema verification") + "\" src=\"" + this.ApplicationPath + "/images/ui/icons/contentValidate.png\" border=\"0\" name=\"img_txt_xslt2\"></a>";
			ltr_verify3.Text = "<a href=\"#\" onclick=\"VerifyXslt(\'txt_xslt3\'); return false\"><img title=\"" + GetMessage("alt text for xsl or schema verification") + "\" alt=\"" + GetMessage("alt text for xsl or schema verification") + "\" src=\"" + this.ApplicationPath + "/images/ui/icons/contentValidate.png\" border=\"0\" name=\"img_txt_xslt3\"></a>";
		}
		
		private void Util_AddProductTypeItems(EkEnumeration.CatalogEntryType EntryClass)
		{
			
			switch (this.m_sPageAction)
			{
				
				case "addproducttype":
					drp_type.Visible = true;
					drp_type.Items.Add(new ListItem(m_refMsg.GetMessage("lbl commerce product"), EkEnumeration.CatalogEntryType.Product.ToString()));
					drp_type.Items.Add(new ListItem(m_refMsg.GetMessage("lbl catalog kit"), EkEnumeration.CatalogEntryType.Kit.ToString()));
					drp_type.Items.Add(new ListItem(m_refMsg.GetMessage("lbl commerce bundle"), EkEnumeration.CatalogEntryType.Bundle.ToString()));
					drp_type.Items.Add(new ListItem(m_refMsg.GetMessage("lbl commerce subscription"), EkEnumeration.CatalogEntryType.SubscriptionProduct.ToString()));
					break;
					
				default: // edit and view
					drp_type.Visible = false;
					
					if (EntryClass == EkEnumeration.CatalogEntryType.Product)
					{
						litType.Text = m_refMsg.GetMessage("lbl commerce product");
					}
					if (EntryClass == EkEnumeration.CatalogEntryType.Kit)
					{
						litType.Text = m_refMsg.GetMessage("lbl catalog kit");
					}
					if (EntryClass == EkEnumeration.CatalogEntryType.Bundle)
					{
						litType.Text = m_refMsg.GetMessage("lbl commerce bundle");
					}
					if (EntryClass == EkEnumeration.CatalogEntryType.SubscriptionProduct)
					{
						litType.Text = m_refMsg.GetMessage("lbl commerce subscription");
					}
					break;
			}
			
		}
		
		protected string Util_ShowType(long TypeId)
		{
			string sret = "";
			switch ((Ektron.Cms.Common.EkEnumeration.CatalogEntryType)TypeId)
			{
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct:
					sret = GetMessage("lbl commerce subscription");
					break;
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle:
					sret = GetMessage("lbl commerce bundle");
					break;
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit:
					sret = GetMessage("lbl catalog kit");
					break;
				default:
					sret = GetMessage("lbl commerce product");
					break;
			}
			return sret;
		}
		
		protected string Util_GetType(long TypeId)
		{
			
			string sret = string.Empty;

            switch ((Ektron.Cms.Common.EkEnumeration.CatalogEntryType)TypeId)
			{
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct:
					sret = "subscription";
					break;
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.ComplexProduct:
					sret = "complexproduct";
					break;
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Bundle:
					sret = "bundle";
					break;
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Kit:
					sret = "kit";
					break;
				case Ektron.Cms.Common.EkEnumeration.CatalogEntryType.Product:
					sret = "product";
					break;
				default:
					sret = "product";
					break;
			}
			return sret;
		}
		
		protected void Util_CheckAccess()
		{
			if (! m_refContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin))
			{
				throw (new Exception(GetMessage("err not role commerce-admin")));
			}
		}
		
		protected string Util_AttrText(int OptionValue)
		{
			string sRet = "";
            switch ((Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType)OptionValue)
			{
				case Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.String:
					sRet = GetMessage("text");
					break;
				case Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Date:
					sRet = GetMessage("lbl attr date");
					break;
				case Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Boolean:
					sRet = GetMessage("lbl attr boolean");
					break;
				case Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric:
					sRet = GetMessage("lbl attr number");
					break;
				default:
					sRet = GetMessage("text");
					break;
			}
			return sRet;
		}
		
		protected EkEnumeration.ProductTypeAttributeDataType Util_GetAttributeType(string attributeTypeValue)
		{
			attributeTypeValue = attributeTypeValue.ToLower();
			switch (attributeTypeValue)
			{
				case "string":
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.String;
				case "date":
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Date;
				case EkConstants.BOOLEAN_PROP:
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Boolean;
				case "numeric":
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric;
				case EkConstants.DOUBLE_PROP:
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric;
				case EkConstants.FLOAT_PROP:
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric;
				case EkConstants.INTEGER_PROP:
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric;
				case EkConstants.LONG_PROP:
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric;
				case EkConstants.SHORT_PROP:
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.Numeric;
				default:
					return Ektron.Cms.Common.EkEnumeration.ProductTypeAttributeDataType.String;
			}
		}
		
		#endregion
		
		#region CSS, JS
		
		private void RegisterJS()
		{
			
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
			Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/dhtml/attribtableutil.js", "EktronTableUtilitiesJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/java/dhtml/mediatableutil.js", "EktronMediaTableUtilitiesJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.string.js", "EktronStringUtilitiesJS");
			Ektron.Cms.API.JS.RegisterJS(this, this.ApplicationPath + "/Tree/js/com.ektron.utils.cookie.js", "EktronCoookieUtilitiesJS");
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
			
		}
		
		protected void Util_SetJs()
		{
			StringBuilder sbJS = new StringBuilder();
			sbJS.Append("<script type=\"text/javascript\">").Append(Environment.NewLine);
			sbJS.Append(this.JSLibrary.CheckKeyValue());
			sbJS.Append(this.JSLibrary.ToggleDiv());
			sbJS.Append(this.JSLibrary.URLEncode());
			sbJS.Append("	var bVerifying = false;").Append(Environment.NewLine);
			sbJS.Append("	var bPassed = true;").Append(Environment.NewLine);
			sbJS.Append("	var numOfVerifyLoops = 0;").Append(Environment.NewLine);
			sbJS.Append("	var strXslErrorMsg = \"\";").Append(Environment.NewLine);
			sbJS.Append("	var unique = 0;	").Append(Environment.NewLine);
			sbJS.Append("   function GetAttrLabel(attrtype) { ").Append(Environment.NewLine);
			sbJS.Append("       switch (attrtype) { ").Append(Environment.NewLine);
			sbJS.Append("             case \"boolean\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr boolean")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"byte\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr byte")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"double\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr double")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"float\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr float")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"integer\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr integer")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"long\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr long")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"short\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr short")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"number\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr number")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"date\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("lbl attr date")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("             case \"text\" : ").Append(Environment.NewLine);
			sbJS.Append("                 attrtype = \'").Append(GetMessage("text")).Append("\'; break; ").Append(Environment.NewLine);
			sbJS.Append("       } ").Append(Environment.NewLine);
			sbJS.Append("       return attrtype; ").Append(Environment.NewLine);
			sbJS.Append("   } ").Append(Environment.NewLine);
			switch (this.m_sPageAction) // m_strPageAction
			{
				case "addthumbnail":
					sbJS.Append("   function validate()").Append(Environment.NewLine);
					sbJS.Append("   {").Append(Environment.NewLine);
					sbJS.Append("           var width = document.getElementById(\'txtWidth\').value;").Append(Environment.NewLine);
					sbJS.Append("           var height = document.getElementById(\'txtHeight\').value;").Append(Environment.NewLine);
					sbJS.Append("           var name = document.getElementById(\'txtName\').value;").Append(Environment.NewLine);
					sbJS.Append("           if(name == \'\')").Append(Environment.NewLine);
					sbJS.Append("           {").Append(Environment.NewLine);
					sbJS.Append("	    		alert(\"").Append(GetMessage("js: alert name required")).Append("\");").Append(Environment.NewLine);
					sbJS.Append("               return false;").Append(Environment.NewLine);
					sbJS.Append("           }").Append(Environment.NewLine);
					sbJS.Append("           else if(isNaN(width) || isNaN(height) || (width == \'\') || (height == \'\') || (width == 0) || (height == 0))").Append(Environment.NewLine);
					sbJS.Append("           {").Append(Environment.NewLine);
					sbJS.Append("               alert(\"").Append(GetMessage("js alert package dimension value")).Append("\");").Append(Environment.NewLine);
					sbJS.Append("               return false;").Append(Environment.NewLine);
					sbJS.Append("           }").Append(Environment.NewLine);
					sbJS.Append("           else").Append(Environment.NewLine);
					sbJS.Append("           {").Append(Environment.NewLine);
					sbJS.Append("               submitThumbnailForm();").Append(Environment.NewLine);
					sbJS.Append("           }").Append(Environment.NewLine);
					sbJS.Append("   }").Append(Environment.NewLine);
					sbJS.Append("   function Close(){").Append(Environment.NewLine);
					sbJS.Append("   self.parent.ektb_remove();").Append(Environment.NewLine);
					sbJS.Append("   }").Append(Environment.NewLine);
					sbJS.Append("   function submitThumbnailForm()").Append(Environment.NewLine);
					sbJS.Append("   {").Append(Environment.NewLine);
					sbJS.Append("           var width = document.getElementById(\'txtWidth\').value;").Append(Environment.NewLine);
					sbJS.Append("           var height = document.getElementById(\'txtHeight\').value;").Append(Environment.NewLine);
					sbJS.Append("           var name = document.getElementById(\'txtName\').value;").Append(Environment.NewLine);
					if (m_iID > 0)
					{
						sbJS.Append("		    parent.editRowInMediaTable(").Append(m_iID.ToString()).Append(", name, width, height); ").Append(Environment.NewLine);
					}
					else
					{
						sbJS.Append("           parent.addRowToMediaTable(null, 0, name, width, height); ").Append(Environment.NewLine);
					}
					sbJS.Append("		        self.parent.ektb_remove(); ").Append(Environment.NewLine);
					sbJS.Append("}   ").Append(Environment.NewLine);
					break;
				case "addattribute":
				case "editattribute":
					sbJS.Append("	function SubmitAttrForm() {").Append(Environment.NewLine);
					sbJS.Append("	    var sAttrName = Trim(document.getElementById(\'").Append(txt_attrname.UniqueID).Append("\').value); ").Append(Environment.NewLine);
					sbJS.Append("       var oDrpAttr = document.getElementById(\'").Append(drp_attrtype.UniqueID).Append("\'); ").Append(Environment.NewLine);
					sbJS.Append("       var sAttrType = oDrpAttr.options[oDrpAttr.selectedIndex].value;").Append(Environment.NewLine);
					sbJS.Append("       var sAttrDef = \'\'; ").Append(Environment.NewLine);
					sbJS.Append("       switch(sAttrType) {").Append(Environment.NewLine);
					sbJS.Append("           case \"text\" :").Append(Environment.NewLine);
					sbJS.Append("               sAttrDef = Trim(document.getElementById(\'").Append(txt_textdefault.UniqueID).Append("\').value); ").Append(Environment.NewLine);
					sbJS.Append("               break;").Append(Environment.NewLine);
					sbJS.Append("           case \"boolean\" :").Append(Environment.NewLine);
					sbJS.Append("               if (document.getElementById(\'").Append(chk_bool.UniqueID).Append("\').checked) {sAttrDef = \'True\'; } else { sAttrDef = \'False\'; } ").Append(Environment.NewLine);
					sbJS.Append("               break;").Append(Environment.NewLine);
					sbJS.Append("           default :").Append(Environment.NewLine);
					sbJS.Append("               sAttrDef = Trim(document.getElementById(\'").Append(txt_number.UniqueID).Append("\').value); ").Append(Environment.NewLine);
					sbJS.Append("               break;").Append(Environment.NewLine);
					sbJS.Append("		} ").Append(Environment.NewLine);
					sbJS.Append("		if (sAttrName == \"\") {").Append(Environment.NewLine);
					sbJS.Append("			alert(\"").Append(GetMessage("js: alert name required")).Append("\");").Append(Environment.NewLine);
					sbJS.Append("			document.getElementById(\'").Append(txt_attrname.UniqueID).Append("\').focus();").Append(Environment.NewLine);
					sbJS.Append("       } else if (!ValidNumeric()) { ").Append(Environment.NewLine);
					sbJS.Append("			alert(\"").Append(GetMessage("res_isrch_iputnum")).Append("\");").Append(Environment.NewLine);
					sbJS.Append("			document.getElementById(\'").Append(txt_number.UniqueID).Append("\').focus();").Append(Environment.NewLine);
					sbJS.Append("		} else {").Append(Environment.NewLine);
					if (m_iID > 0)
					{
						sbJS.Append("		    parent.editRowInTable(").Append(m_iID.ToString()).Append(", sAttrName, sAttrType, sAttrDef); ").Append(Environment.NewLine);
					}
					else
					{
						sbJS.Append("           parent.addRowToTable(null, 0, sAttrName, sAttrType, sAttrDef); ").Append(Environment.NewLine);
					}
					sbJS.Append("		        self.parent.ektb_remove(); ").Append(Environment.NewLine);
					sbJS.Append("		} ").Append(Environment.NewLine);
					sbJS.Append("	}").Append(Environment.NewLine);
					sbJS.Append("     function ChangeOption(selObj) {").Append(Environment.NewLine);
					sbJS.Append("        var selIndex = selObj.selectedIndex;").Append(Environment.NewLine);
					sbJS.Append("        switch(selObj.options[selIndex].value) {").Append(Environment.NewLine);
					sbJS.Append("            case \"text\" :").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divText\', true);").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divNum\', false);").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divChk\', false);").Append(Environment.NewLine);
					sbJS.Append("                break;").Append(Environment.NewLine);
					sbJS.Append("            case \"boolean\" :").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divText\', false);").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divNum\', false);").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divChk\', true);").Append(Environment.NewLine);
					sbJS.Append("                break;").Append(Environment.NewLine);
					sbJS.Append("            default :").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divText\', false);").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divNum\', true);").Append(Environment.NewLine);
					sbJS.Append("                ToggleDiv(\'divChk\', false);").Append(Environment.NewLine);
					sbJS.Append("                break;").Append(Environment.NewLine);
					sbJS.Append("        }").Append(Environment.NewLine);
					sbJS.Append("    }").Append(Environment.NewLine);
					sbJS.Append("    function ValidNumeric() {").Append(Environment.NewLine);
					sbJS.Append("        var bRet = false").Append(Environment.NewLine);
					sbJS.Append("        var selObj = document.getElementById(\'drp_attrtype\');").Append(Environment.NewLine);
					sbJS.Append("        var selIndex = selObj.selectedIndex;").Append(Environment.NewLine);
					sbJS.Append("        switch(selObj.options[selIndex].value) {").Append(Environment.NewLine);
					sbJS.Append("            case \"text\" :").Append(Environment.NewLine);
					sbJS.Append("            case \"boolean\" :").Append(Environment.NewLine);
					sbJS.Append("                bRet = true;").Append(Environment.NewLine);
					sbJS.Append("                break;").Append(Environment.NewLine);
					sbJS.Append("            default :").Append(Environment.NewLine);
					sbJS.Append("                var sNum = Trim(document.getElementById(\'txt_number\').value);").Append(Environment.NewLine);
					sbJS.Append("                if (!(isNaN(parseFloat(sNum)))) { bRet = true; } ").Append(Environment.NewLine);
					sbJS.Append("                break;").Append(Environment.NewLine);
					sbJS.Append("        }").Append(Environment.NewLine);
					sbJS.Append("        return bRet;").Append(Environment.NewLine);
					sbJS.Append("    } ").Append(Environment.NewLine);
					break;
				default:
					sbJS.Append("	function SubmitForm() {").Append(Environment.NewLine);
					sbJS.Append("	    document.getElementById(\'").Append(txtTitle.UniqueID).Append("\').value = Trim(document.getElementById(\'").Append(txtTitle.UniqueID).Append("\').value);").Append(Environment.NewLine);
					sbJS.Append("		if(document.getElementById(\'").Append(txtDescription.UniqueID).Append("\').value.indexOf(\'<\') > -1 || document.getElementById(\'").Append(txtDescription.UniqueID).Append("\').value.indexOf(\'>\') > -1) {").Append(Environment.NewLine);
					sbJS.Append("			alert(\"").Append(string.Format(GetMessage("js alert product type desc cant include"), "<, >")).Append("\");").Append(Environment.NewLine);
					sbJS.Append("			$ektron(\'.tabContainer\').tabs(\'select\',0);document.getElementById(\'").Append(txtDescription.UniqueID).Append("\').focus(); return false;").Append(Environment.NewLine);
					sbJS.Append("		} ").Append(Environment.NewLine);
					sbJS.Append("		if (document.getElementById(\'").Append(txtTitle.UniqueID).Append("\').value == \"\") {").Append(Environment.NewLine);
					sbJS.Append("			alert(\"").Append(GetMessage("js: alert title required")).Append("\");").Append(Environment.NewLine);
					sbJS.Append("			$ektron(\'.tabContainer\').tabs(\'select\',0);document.getElementById(\'").Append(txtTitle.UniqueID).Append("\').focus();").Append(Environment.NewLine);
					sbJS.Append("		} else if (document.getElementById(\'").Append(txtTitle.UniqueID).Append("\').value.indexOf(\'<\') > -1 || document.getElementById(\'").Append(txtTitle.UniqueID).Append("\').value.indexOf(\'>\') > -1) {").Append(Environment.NewLine);
					sbJS.Append("			alert(\"").Append(string.Format(GetMessage("js alert product type title cant include"), "<, >")).Append("\");").Append(Environment.NewLine);
					sbJS.Append("			$ektron(\'.tabContainer\').tabs(\'select\',0);document.getElementById(\'").Append(txtTitle.UniqueID).Append("\').focus();").Append(Environment.NewLine);
					sbJS.Append("		} else {").Append(Environment.NewLine);
					sbJS.Append("		    document.forms[0].submit();").Append(Environment.NewLine);
					sbJS.Append("		} ").Append(Environment.NewLine);
					sbJS.Append("	}").Append(Environment.NewLine);
					sbJS.Append("	function VerifyXsltCallback (formFieldName, displayMsg) {").Append(Environment.NewLine);
					sbJS.Append("		if (bVerifying) {").Append(Environment.NewLine);
					sbJS.Append("			if (numOfVerifyLoops < 350) {").Append(Environment.NewLine);
					sbJS.Append("				setTimeout(\"VerifyXsltCallback(\'\" + formFieldName + \"\', \" + displayMsg + \")\", 100);").Append(Environment.NewLine);
					sbJS.Append("				numOfVerifyLoops++;").Append(Environment.NewLine);
					sbJS.Append("				return false;").Append(Environment.NewLine);
					sbJS.Append("			}").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("		bVerifying = false;").Append(Environment.NewLine);
					sbJS.Append("		if (bPassed) {").Append(Environment.NewLine);
					sbJS.Append("			if (displayMsg) {").Append(Environment.NewLine);
					sbJS.Append("				document.images[\"img_\" + formFieldName].src=\"").Append(this.ApplicationPath).Append("/images/ui/icons/check.png\";").Append(Environment.NewLine);
					sbJS.Append("				alert(\"Verification succeeded.\");").Append(Environment.NewLine);
					sbJS.Append("			}").Append(Environment.NewLine);
					sbJS.Append("			return (true);").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("		else {").Append(Environment.NewLine);
					sbJS.Append("			if (displayMsg) {").Append(Environment.NewLine);
					sbJS.Append("				document.images[\"img_\" + formFieldName].src=\"").Append(this.ApplicationPath).Append("/images/ui/icons/error.png\";").Append(Environment.NewLine);
					sbJS.Append("				alert (strXslErrorMsg);").Append(Environment.NewLine);
					sbJS.Append("			}").Append(Environment.NewLine);
					sbJS.Append("			return (false);").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("	}").Append(Environment.NewLine);
					sbJS.Append("	function VerifyXslt(formFieldName) {").Append(Environment.NewLine);
					sbJS.Append("		var extension;").Append(Environment.NewLine);
					sbJS.Append("		var urlExtension;").Append(Environment.NewLine);
					sbJS.Append("		var thisExtension;").Append(Environment.NewLine);
					sbJS.Append("		var xslPath;").Append(Environment.NewLine);
					sbJS.Append("		").Append(Environment.NewLine);
					sbJS.Append("		if (bVerifying) {").Append(Environment.NewLine);
					sbJS.Append("			return false;").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("		document.forms.xmlconfiguration[formFieldName].value = Trim(document.forms.xmlconfiguration[formFieldName].value);").Append(Environment.NewLine);
					sbJS.Append("		xslPath = document.forms.xmlconfiguration[formFieldName].value;").Append(Environment.NewLine);
					sbJS.Append("		if (xslPath.length == 0) {").Append(Environment.NewLine);
					sbJS.Append("			return false;").Append(Environment.NewLine);
					sbJS.Append("		}		").Append(Environment.NewLine);
					sbJS.Append("		extension = xslPath.split(\"?\");").Append(Environment.NewLine);
					sbJS.Append("		extension = extension[0].split(\".\");").Append(Environment.NewLine);
					sbJS.Append("		thisExtension = extension[extension.length - 1];").Append(Environment.NewLine);
					sbJS.Append("		if (((thisExtension == \"asp\") || (thisExtension == \"aspx\")").Append(Environment.NewLine);
					sbJS.Append("			|| (thisExtension == \"cfm\") || (thisExtension == \"php\"))").Append(Environment.NewLine);
					sbJS.Append("			&& ((xslPath.substring(0,7) != \"http://\") && (xslPath.substring(0,8) != \"https://\"))) {").Append(Environment.NewLine);
					sbJS.Append("			").Append(Environment.NewLine);
					sbJS.Append("			alert(\"Dynamically generated XSLT or schema files must use a fully qualified Web path.\\nExample\\nhttp://localhost/xmlfiles/myxslt.aspx\");").Append(Environment.NewLine);
					sbJS.Append("			return false;").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("		unique++;").Append(Environment.NewLine);
					sbJS.Append("		if (document.all) {").Append(Environment.NewLine);
					sbJS.Append("			document.all[\"iframe1\"].src=\"../xml_verify.aspx?path=\" + escape(xslPath);").Append(Environment.NewLine);
					sbJS.Append("			").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("		else if (document.getElementById) {").Append(Environment.NewLine);
					sbJS.Append("			document.getElementById(\"iframe1\").src=\"../xml_verify.aspx?path=\" + escape(xslPath) + \"&num=\" + unique;").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("		else {").Append(Environment.NewLine);
					sbJS.Append("			document.layers[\"iframe1\"].load(\"../xml_verify.aspx?path=\" + escape(xslPath) + \"&num=\" + unique, \"100%\");").Append(Environment.NewLine);
					sbJS.Append("		}").Append(Environment.NewLine);
					sbJS.Append("		bVerifying = true;").Append(Environment.NewLine);
					sbJS.Append("		bPassed = false;").Append(Environment.NewLine);
					sbJS.Append("		numOfVerifyLoops = 0;").Append(Environment.NewLine);
					sbJS.Append("		strXslErrorMsg = \"Timeout\";").Append(Environment.NewLine);
					sbJS.Append("		setTimeout(\"VerifyXsltCallback(\'\" + formFieldName + \"\', \" + true + \")\", 100);").Append(Environment.NewLine);
					sbJS.Append("	}").Append(Environment.NewLine);
					sbJS.Append("	function MakeNoVerify (formName, item, keys) {").Append(Environment.NewLine);
					//sbJS.Append("		if (document.forms.xmlconfiguration[formName.name + ""_length""].value != formName.value) {").Append(Environment.NewLine)
					sbJS.Append("			document.images[\"img_\" + formName.name].src = \"").Append(this.ApplicationPath).Append("/images/ui/icons/contentValidate.png\";").Append(Environment.NewLine);
					//sbJS.Append("		}").Append(Environment.NewLine)
					//sbJS.Append("		document.forms.xmlconfiguration[formName.name + ""_length""].value = formName.value;").Append(Environment.NewLine)
					sbJS.Append("	}").Append(Environment.NewLine);
					sbJS.Append("   function AddAttribute() { ").Append(Environment.NewLine);
					sbJS.Append("       ektb_show(\'\',\'producttypes.aspx?action=addAttribute&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true\', null); ").Append(Environment.NewLine);
					sbJS.Append("   } ").Append(Environment.NewLine);
					sbJS.Append("   function AddThumbnail() { ").Append(Environment.NewLine);
					sbJS.Append("       ektb_show(\'\',\'producttypes.aspx?action=addThumbnail&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true\', null); ").Append(Environment.NewLine);
					sbJS.Append("   } ").Append(Environment.NewLine);
					sbJS.Append("    function EditAttribute() {").Append(Environment.NewLine);
					sbJS.Append("        var oAttr = getCheckedObj();").Append(Environment.NewLine);
					sbJS.Append("        if (oAttr == null) {").Append(Environment.NewLine);
					sbJS.Append("            alert(\'").Append(GetMessage("js please sel attr")).Append("\');").Append(Environment.NewLine);
					sbJS.Append("        } else {").Append(Environment.NewLine);
					sbJS.Append("            ektb_show(\'\',\'producttypes.aspx?action=editAttribute&id=\' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.one.data) + \'&name=\' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.two.data) + \'&type=\' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.seven.value) + \'&def=\' + ").Append(JSLibrary.URLEncodeFunctionName).Append("(oAttr.four.data) + \'&EkTB_iframe=true&height=300&width=500&modal=true\', null);").Append(Environment.NewLine);
					sbJS.Append("        }").Append(Environment.NewLine);
					sbJS.Append("    }").Append(Environment.NewLine);
					sbJS.Append("    function DeleteAttribute() {").Append(Environment.NewLine);
					sbJS.Append("        var iAttr = getCheckedInt(false);").Append(Environment.NewLine);
					sbJS.Append("        if (iAttr == -1) {").Append(Environment.NewLine);
					sbJS.Append("            alert(\'").Append(GetMessage("js please sel attr")).Append("\');").Append(Environment.NewLine);
					sbJS.Append("        } else {").Append(Environment.NewLine);
					sbJS.Append("            deleteChecked();").Append(Environment.NewLine);
					sbJS.Append("        }").Append(Environment.NewLine);
					sbJS.Append("    }").Append(Environment.NewLine);
					sbJS.Append("    function DeleteMediaThumbnail() {").Append(Environment.NewLine);
					sbJS.Append("        var iAttr = getMediaCheckedInt(false);").Append(Environment.NewLine);
					sbJS.Append("        if (iAttr == -1) {").Append(Environment.NewLine);
					sbJS.Append("            alert(\'").Append(GetMessage("js please sel media default")).Append("\');").Append(Environment.NewLine);
					sbJS.Append("        } else {").Append(Environment.NewLine);
					sbJS.Append("            deleteCheckedMedia();").Append(Environment.NewLine);
					sbJS.Append("        }").Append(Environment.NewLine);
					sbJS.Append("    }").Append(Environment.NewLine);
					break;
			}
			sbJS.Append("</script>").Append(Environment.NewLine);
			ltr_js.Text = sbJS.ToString();
		}
		
		private void RegisterCSS()
		{
			
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/explorer/css/com.ektron.ui.menu.css", "EktronExplorerMenuCss");
			Ektron.Cms.API.Css.RegisterCss(this, this.ApplicationPath + "/csslib/tables/tableutil.css", "EktronTableUtilitiesCss");
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			
		}
		
		#endregion
		
	}
	


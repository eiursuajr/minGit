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

	public partial class AssetManagementConfig : System.Web.UI.Page
	{
		protected StyleHelper m_refStyle = new StyleHelper();
		protected ContentAPI m_refContentApi;
		protected string AppName = "";
		private string AppPath = "";
		protected string AppImgPath = "";
		protected EkMessageHelper m_refMsg;
		private DataView m_dataView;
		private DataTable m_dataTable;
		private AssetConfigInfo[] asset_config;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			m_refContentApi = new ContentAPI();
			m_refMsg = m_refContentApi.EkMsgRef;
            if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(m_refContentApi.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }
            localizeDataGrid();
            AppName = m_refContentApi.AppName;
			AppImgPath = m_refContentApi.AppImgPath;
			AppPath = m_refContentApi.AppPath;
			StyleSheetJS.Text = m_refStyle.GetClientScript();
			AMSToolBar();
			
			if (!(Page.IsPostBack))
			{
				asset_config = m_refContentApi.GetAssetMgtConfigInfo();
				PopulateGridData(asset_config);
				Session["AssetManagementConfigData"] = asset_config;
			}
			else
			{
				if (! (Session["AssetManagementConfigTable"] == null))
				{
                    m_dataTable = (System.Data.DataTable)Session["AssetManagementConfigTable"];
                    asset_config = (AssetConfigInfo[])Session["AssetManagementConfigData"];
					m_dataView = new DataView(m_dataTable);
					m_dataView.Sort = "TAG";
				}
			}
			RegisterResources();
		}
        private void localizeDataGrid()
        {
            foreach (System.Web.UI.WebControls.DataGridColumn c in AMSGrid.Columns)
            {
                switch (c.HeaderText.ToLower())
                {
                    case "":
                        EditCommandColumn col=(EditCommandColumn)c;
                        col.UpdateText = "<img src='./images/UI/Icons/Save.png' alt='" + m_refMsg.GetMessage("alt save changes") + "' >";
                        col.CancelText = "<img src='./images/UI/Icons/cancel.png' alt='" + m_refMsg.GetMessage("alt cancel editing") + "' >";
                        col.EditText = "<img src='./images/UI/Icons/contentEdit.png' alt='" + m_refMsg.GetMessage("alt edit this item") + "' >";
                        break;
                    case "tag":
                        c.HeaderText = m_refMsg.GetMessage("lbl tag");
                        break;
                    case "value":
                        c.HeaderText = m_refMsg.GetMessage("lbl value");
                        break;
                    case "description":
                        c.HeaderText = m_refMsg.GetMessage("lbl description");
                        break;
                }
            }
        }
		private void PopulateGridData(AssetConfigInfo[] assetConfig)
		{
			
			AMSGrid.BorderColor = System.Drawing.Color.White;
			
			m_dataTable = new DataTable();
			DataRow dr;
			m_dataTable.Columns.Add(new DataColumn("EDIT", typeof(string)));
			m_dataTable.Columns.Add(new DataColumn("TAG", typeof(string)));
			m_dataTable.Columns.Add(new DataColumn("VALUE", typeof(string)));
			m_dataTable.Columns.Add(new DataColumn("DESC", typeof(string)));
			int i = 0;
			if (!(assetConfig == null))
			{
				for (i = 0; i <= assetConfig.Length - 1; i++)
				{
					// Temporary Fix untill chandra removes AsetConfigType.CatalogLocation/CatalogName from DMS code
					if ((assetConfig[i] != null) && assetConfig[i].Tag.ToString() != "UserName" && assetConfig[i].Tag.ToString() != "Password" && assetConfig[i].Tag.ToString() != "CatalogLocation" && assetConfig[i].Tag.ToString() != "CatalogName" && assetConfig[i].Tag.ToString() != "pdfGenerator")
					{
						dr = m_dataTable.NewRow();
                        dr[1] = GetResourceText(assetConfig[i].Tag.ToString());
						dr[2] = assetConfig[i].Value;
                        dr[3] = GetResourceText(assetConfig[i].Description);
						m_dataTable.Rows.Add(dr);
					}
				}
			}
			
			m_dataView = new DataView(m_dataTable);
			m_dataView.Sort = "TAG";
			Session["AssetManagementConfigTable"] = m_dataTable;
			
			BindData();
		}
        private string GetResourceText(string st)
        {
            if (st == "DomainName")
                st = this.m_refMsg.GetMessage("lbl DomainName");
            else if (st == "FileTypes")
                st = this.m_refMsg.GetMessage("lbl FileTypes");
            else if (st == "LoadBalanced")
                st = this.m_refMsg.GetMessage("lbl LoadBalanced");
            else if (st == "ServerName")
                st = this.m_refMsg.GetMessage("lbl ServerName");
            else if (st == "StorageLocation")
                st = this.m_refMsg.GetMessage("lbl StorageLocation");
            else if (st == "WebShareDir")
                st = this.m_refMsg.GetMessage("lbl WebShareDir");
            else if(st.Contains("AppDomain under which"))
                st=this.m_refMsg.GetMessage("lbl appdomain desc");
            else if (st.Contains("File types supported for"))
                st = this.m_refMsg.GetMessage("lbl filetypes desc");
            else if (st.Contains("Enables load balancing"))
                st = this.m_refMsg.GetMessage("lbl enablesload desc");
            else if (st.Contains("User domain against"))
                st = this.m_refMsg.GetMessage("lbl user domain desc");
            else if (st.Contains("Secure storage location"))
                st = this.m_refMsg.GetMessage("lbl Secure desc");
            else if (st.Contains("Temporary directory name"))
                st = this.m_refMsg.GetMessage("lbl Temp dir desc");



            return st;
        }
		protected void BindData()
		{
			AMSGrid.DataSource = m_dataView;
			AMSGrid.DataBind();
		}
		public void AMSGrid_Cancel(object sender, DataGridCommandEventArgs e)
		{
			AMSGrid.EditItemIndex = -1;
			BindData();
		}
		public void AMSGrid_Edit(object sender, DataGridCommandEventArgs e)
		{
			AMSGrid.EditItemIndex = e.Item.ItemIndex;
			BindData();
		}
		
		public void AMSGrid_Update(object sender, DataGridCommandEventArgs e)
		{
            DataRow dr;
			TextBox dataText = (System.Web.UI.WebControls.TextBox) e.Item.Cells[2].Controls[1];
			string data = dataText.Text;
			string tag = e.Item.Cells[1].Text;
			string desc = e.Item.Cells[3].Text;
			
			
			//DataViews filter not getting cleared
			
			m_dataView.RowFilter = "TAG=\'" + tag + "\'";
			
			if (m_dataView.Count > 0)
			{
				m_dataView.Delete(0);
			}
			
			m_dataView.RowFilter = "";
			
			dr = m_dataTable.NewRow();
			m_dataTable.Rows.Add(dr);
			dr[1] = tag;
			dr[2] = data;
			dr[3] = desc;
			
			if (tag == AsetConfigType.CatalogLocation.ToString())
			{
				asset_config[ Convert.ToInt32( AsetConfigType.CatalogLocation) ].Value = data;
			}
			else if (tag == AsetConfigType.CatalogName.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.CatalogName)].Value = data;
			}
			else if (tag == AsetConfigType.DomainName.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.DomainName)].Value = data;
			}
			else if (tag == AsetConfigType.FileTypes.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.FileTypes)].Value = data;
			}
			else if (tag == AsetConfigType.LoadBalanced.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.LoadBalanced)].Value = data;
			}
			else if (tag == AsetConfigType.Password.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.Password)].Value = data;
			}
			else if (tag == AsetConfigType.ServerName.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.ServerName)].Value = data;
			}
			else if (tag == AsetConfigType.StorageLocation.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.StorageLocation)].Value = data;
			}
			else if (tag == AsetConfigType.UserName.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.UserName)].Value = data;
			}
			else if (tag == AsetConfigType.WebShareDir.ToString())
			{
				asset_config[Convert.ToInt32(AsetConfigType.WebShareDir)].Value = data;
			}
			
			Session["AssetManagementConfigTable"] = m_dataTable;
			Session["DDSnip"] = null;
			m_refContentApi.SetAssetMgtConfigInfo(asset_config);
			RefreshDropUploader();
			AMSGrid.EditItemIndex = -1;
			BindData();
		}
		
		private void AMSToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl Asset Management Configuration"));
			result.Append("<table><tr>");
			result.Append("<td>");
			result.Append(m_refStyle.GetHelpButton("assetserverconfig", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		
		private void RefreshDropUploader()
		{
			System.Text.StringBuilder sJS = new System.Text.StringBuilder();
			sJS.Append("<script type=\"text/Javascript\">" + "\r\n");
			sJS.Append("var dragDropFrame = top.GetEkDragDropObject();" + "\r\n");
			sJS.Append("      if (dragDropFrame != null) {" + "\r\n");
			sJS.Append("            dragDropFrame.location.reload();" + "\r\n");
			sJS.Append("      }" + "\r\n");
			sJS.Append("</script>" + "\r\n");
			Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "DragRefreshJS", sJS.ToString());
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			Ektron.Cms.API.JS.RegisterJS(this, AppPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
		}
	}


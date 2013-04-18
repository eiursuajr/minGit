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


	public partial class adconfigure : System.Web.UI.Page
	{
		
		
		#region  Web Form Designer Generated Code
		
		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			
		}
		
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			//CODEGEN: This method call is required by the Web Form Designer
			//Do not modify it using the code editor.
			InitializeComponent();
		}

		protected string m_strPageAction = "";
		protected editadconfigure m_editadconfigure;
		protected viewadconfigure m_viewadconfigure;


		#endregion
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
            StyleSheetJS.Text = (new StyleHelper()).GetClientScript().ToString();
		}
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			bool bCompleted;
			RegisterResources();
			try
			{
				if (!(Request.QueryString["action"] == null))
				{
					if (Request.QueryString["action"] != "")
					{
						m_strPageAction = Request.QueryString["action"].ToLower();
					}
				}
				switch (m_strPageAction)
				{
					case "edit":
						m_editadconfigure = (editadconfigure) (LoadControl("controls/configuration/editadconfigure.ascx"));
						DataHolder.Controls.Add(m_editadconfigure);
                        bCompleted = m_editadconfigure.EditAdConfiguration();
						if (bCompleted == true)
						{
							Response.Redirect("adconfigure.aspx", false);
						}
						break;
					default:
						m_viewadconfigure = (viewadconfigure) (LoadControl("controls/configuration/viewadconfigure.ascx"));
						DataHolder.Controls.Add(m_viewadconfigure);
						bCompleted = m_viewadconfigure.Display_ViewConfiguration();
						if (bCompleted)
						{
							Response.Redirect("adconfigure.aspx", false);
						}
						break;
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(ex.Message);
			}
		}
		protected void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
	}


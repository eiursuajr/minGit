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

	public partial class Workarea_widgetsettings : System.Web.UI.Page
	{
		
		protected ContentAPI contentAPI;
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			// Register CSS
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			
			contentAPI = new ContentAPI();
			m_strStyleSheetJS.Text = (new StyleHelper()).GetClientScript();
			
			if (!Utilities.ValidateUserLogin())
			{
				return;
			}
			if ((contentAPI.RequestInformationRef.IsMembershipUser == 1 || ! contentAPI.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminPersonalize)) && ! contentAPI.IsAdmin())
			{
				Response.Redirect((string) ("reterror.aspx?info=" + contentAPI.EkMsgRef.GetMessage("msg login personalize administrator")), true);
				return;
			}
			
			string action = "";
			if (! string.IsNullOrEmpty(Request.QueryString["action"]))
			{
				action = Request.QueryString["action"];
			}
			
			switch (action.ToLower())
			{
				case "widgetsync":
					Workarea_controls_widgetSettings_WidgetSync ctlWidgetSync;
					ctlWidgetSync = (Workarea_controls_widgetSettings_WidgetSync)(Page.LoadControl("controls/widgetSettings/WidgetSync.ascx"));
                    placeHolder.Controls.Add(ctlWidgetSync);
					break;
				case "widgetedit":
					Workarea_controls_widgetSettings_WidgetEdit ctlWidgetEdit;
					ctlWidgetEdit = (Workarea_controls_widgetSettings_WidgetEdit) (Page.LoadControl("controls/widgetSettings/WidgetEdit.ascx"));
                    placeHolder.Controls.Add(ctlWidgetEdit);
					break;
				default:
					Workarea_controls_widgetSettings_WidgetSpace ctlWidgetSpace;
					ctlWidgetSpace = (Workarea_controls_widgetSettings_WidgetSpace) (Page.LoadControl("controls/widgetSettings/WidgetSpace.ascx"));
                    placeHolder.Controls.Add(ctlWidgetSpace);
					break;
			}
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
		}
	}
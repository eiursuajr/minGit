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


	public partial class UserProfile_UserProfile : System.Web.UI.UserControl
	{
		
		
		#region Member Variables
		protected CommonApi commonAPI;
		#endregion
		
		#region Public Properties
		private string m_Css = "";
		public string Css
		{
			get
			{
				return m_Css;
			}
			set
			{
				m_Css = value;
			}
		}
		
		private long m_UserId = 0;
		public long UserId
		{
			get
			{
				return m_UserId;
			}
			set
			{
				m_UserId = value;
			}
		}
		#endregion
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			string action = "";
			
			action = Request.QueryString["action"];
			if (action == null)
			{
				action = "";
			}
			
			commonAPI = new CommonApi();
			
			if (Css == "")
			{
				Css = commonAPI.ApplicationPath + "csslib/userprofile/userprofile.css";
			}
			
			if (Page.Header != null)
			{
				System.Web.UI.HtmlControls.HtmlLink MemberCssLink = new System.Web.UI.HtmlControls.HtmlLink();
				MemberCssLink.Href = ResolveClientUrl(Css);
				MemberCssLink.Attributes["type"] = "text/css";
				MemberCssLink.Attributes["rel"] = "stylesheet";
				Page.Header.Controls.Add(new LiteralControl(Environment.NewLine));
				Page.Header.Controls.Add(MemberCssLink);
				Page.Header.Controls.Add(new LiteralControl(Environment.NewLine));
			}
			else
			{
				Response.Write("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveClientUrl(Css) + "\" />");
			}
			
			if (UserId == 0)
			{
				UserId = commonAPI.UserId;
			}
			
			switch (action.ToLower())
			{
				case "friends":
					this.Friends1.Visible = true;
					this.Friends1.DefaultUserID = UserId;
					this.Friends1.Fill();
					break;
				case "favorites":
					this.Favorites1.Visible = true;
					this.Favorites1.DefaultUserID = UserId;
					this.Favorites1.Fill();
					break;
				case "groups":
					this.CommunityGroupList1.Visible = true;
					this.CommunityGroupList1.DefaultUserID = UserId;
					this.CommunityGroupList1.Fill();
					break;
				case "photos":
					break;
					
				case "documents":
					this.Workspace1.Visible = true;
					this.Workspace1.DefaultObjectID = UserId;
					this.CommunityGroupList1.Fill();
					break;
				case "tags":
					break;
				default:
					this.UserProfile1.Visible = true;
					
					this.UserProfile1.Fill();
					break;
			}
			
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
		}
	}
	


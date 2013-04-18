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
//using System.DateTime;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
//using Ektron.Cms.Common.EkFunctions;
//using Ektron.Cms.CustomFieldsApi;
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkEnumeration;


	public partial class controls_Community_Components_UserSelectControl : System.Web.UI.UserControl
	{
		public controls_Community_Components_UserSelectControl()
		{
			m_user_list = new Ektron.Cms.UserData[0];
			
		}
		
		protected long m_userId = 0;
		protected UserAPI m_refUserApi = new UserAPI();
		protected string m_callbackresult = "";
		protected int m_recipientsPage = 1;
		protected bool m_friendsOnly = false;
		protected bool m_singleSelection = false;
		protected ContentAPI m_refContentAPI = new ContentAPI();
		protected UserData[] m_user_list;
		protected string m_strSearchText = "";
		protected string m_strKeyWords = "";
		protected int m_intTotalPages = 0;
		protected string m_searchMode = "display_name";
		//Protected m_enableButtons As Boolean = False
		
		/// <summary>
		/// When true, narrows user listing to just friends of the current user.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool FriendsOnly
		{
			get
			{
				return (m_friendsOnly);
			}
			set
			{
				m_friendsOnly = value;
			}
		}
		
		/// <summary>
		/// If true, only allows a single user to be selected, otherwise no limit (default).
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool SingleSelection
		{
			get
			{
				return (m_singleSelection);
			}
			set
			{
				m_singleSelection = value;
			}
		}
		
		//Public Property EnableButtons() As Boolean
		//    Get
		//        Return (m_enableButtons)
		//    End Get
		//    Set(ByVal value As Boolean)
		//        m_enableButtons = value
		//    End Set
		//End Property
		
		/// <summary>
		/// Returns the user search control id, to be used when calling
		/// client script methods UserSelectCtl_GetSelectUsers() or
		/// UserSelectCtl_GetUserName().
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string ControlId
		{
			get
			{
				return (usersel_comsearch.ClientID);
			}
		}
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_userId = m_refContentAPI.RequestInformationRef.UserId;
			
			usersel_comsearch_jsinit.Text = "<script type=\"text/javascript\">var usersel_comsearch_ClientID = \'" + usersel_comsearch.ClientID + "\'; var usersel_comsearch_SingleSelection = " + SingleSelection.ToString().ToLower() + ";</script>";
			
			usersel_comsearch.TemplateGroupParamName = "id";
			usersel_comsearch.TemplateUserParamName = "id";
			usersel_comsearch.TemplateTarget = Ektron.Cms.Controls.EkWebControl.ItemLinkTargets._blank;
			usersel_comsearch.MembersOnly = false;
			usersel_comsearch.MaxTagCount = 100;
			usersel_comsearch.EnableMap = false;
			usersel_comsearch.UserTaxonomyID = 440;
			usersel_comsearch.PageSize = 4;
			usersel_comsearch.TemplateUserProfile = m_refContentAPI.AppPath + "UserProfile.aspx";
			usersel_comsearch.DisplayXslt = m_refContentAPI.AppPath + "/xslt/ekUserSelect.xsl";
			usersel_comsearch.FriendsOnly = FriendsOnly;
			
			if (! Page.IsCallback)
			{
				EmitJavascript();
			}
		}
		
		protected string GetMessage(string resource)
		{
			return m_refContentAPI.EkMsgRef.GetMessage(resource);
		}
		
		protected void EmitJavascript()
		{
			if ((Page != null) && ! Page.ClientScript.IsClientScriptBlockRegistered("UserSelectCtl_AjaxJavascript"))
			{
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "UserSelectCtl_AjaxJavascript", GetInitializationJavascript());
			}
		}
		
		protected string GetInitializationJavascript()
		{
			string result = "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			try
			{
				if (!(Page.IsCallback))
				{
					
					sb.Append("<script type=\"text/javascript\">" + Environment.NewLine);
					sb.Append("<!-- \\n " + Environment.NewLine);
					//sb.Append("" + Environment.NewLine)
					sb.Append("//-->" + Environment.NewLine);
					sb.Append("</script>" + Environment.NewLine);
				}
				
			}
			catch (Exception)
			{
				
			}
			finally
			{
				result = sb.ToString();
				sb = null;
			}
			
			return (result);
		}
		
	}

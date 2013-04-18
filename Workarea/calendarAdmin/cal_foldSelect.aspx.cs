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


	public partial class cal_foldSelect : System.Web.UI.Page
	{
		
		protected SiteAPI m_siteRef = new SiteAPI();
        protected EkMessageHelper m_refMsg;
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
            m_refMsg = m_siteRef.EkMsgRef;
			Utilities.ValidateUserLogin();
            if (m_siteRef.RequestInformationRef.IsMembershipUser == 1 || m_siteRef.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(m_siteRef.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }
            JSInc.Text = "<link type=\"text/css\" href=\"" + m_siteRef.AppPath + "csslib/ektron.workarea.css\"/>";
			Collection foldCol = new Collection();
			System.Text.StringBuilder outSB = new System.Text.StringBuilder();
			CalendarAPI cAPI = new CalendarAPI(m_siteRef.RequestInformationRef);
			FolderData fDat = new FolderData();
			fDat = cAPI.GetFolderWithChildren(0);
			TestDate.Text += pShowFolders(fDat).ToString();
		}
		private System.Text.StringBuilder pShowFolders(FolderData inFold)
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			int z;
			ret.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" + "\r\n");
			ret.Append("<tr><td colspan=2 class=\"info\">");
			ret.Append("&#160;<A href=\"JavaScript:folderClick(\'" + inFold.Id + "\',\'" + inFold.NameWithPath.Replace("\'", "\\\'").Replace("\\", "\\\\").Replace("/", "\\/") + "\')\">" + inFold.Name + "</a></td></tr>" + "\r\n");
			if (inFold.ChildFolders != null)
			{
				for (z = 0; z <= (inFold.ChildFolders.Length - 1); z++)
				{
                    ret.Append(pShowFolder(inFold.ChildFolders[z], 1));
				}
			}
			ret.Append("</table>");
			return (ret);
		}
		private System.Text.StringBuilder pShowFolder(FolderData inFold, int level)
		{
			System.Text.StringBuilder ret = new System.Text.StringBuilder();
			int z;
			ret.Append("<tr><td>&#160;&#160;</td><td class=\"info\" style=\"border-left: #000000 1px solid ;\">");
			ret.Append("<span style=\"text-decoration: line-through ;\">");
			for (z = 1; z <= level; z++)
			{
				ret.Append("&#160;&#160;");
			}
			ret.Append("</span>");
			ret.Append("&#160;<A href=\"JavaScript:folderClick(\'" + inFold.Id + "\',\'" + inFold.NameWithPath.Replace("\'", "\\\'").Replace("\\", "\\\\").Replace("/", "\\/") + "\')\">" + inFold.Name + "</a></td></tr>" + "\r\n");
			if (inFold.ChildFolders != null)
			{
				for (z = 0; z <= (inFold.ChildFolders.Length - 1); z++)
				{
					ret.Append(pShowFolder(inFold.ChildFolders[z], level + 1));
				}
			}
			return (ret);
		}
	}

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

	public partial class MetaSelectContainer : System.Web.UI.Page
	{

        protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			Utilities.ValidateUserLogin();
			RegisterResources();
			string FolderID = "";
			if (Request.QueryString["FolderID"] != null)
			{
				FolderID = Request.QueryString["FolderID"];
			}
			string browser = "";
			if (Request.QueryString["browser"] != null)
			{
				browser = Request.QueryString["browser"];
			}
			string WantXmlInfo = "";
			if (Request.QueryString["WantXmlInfo"] != null)
			{
				WantXmlInfo = Request.QueryString["WantXmlInfo"];
			}
			string metadataFormTagId = "";
			if (Request.QueryString["metadataFormTagId"] != null)
			{
				metadataFormTagId = Request.QueryString["metadataFormTagId"];
			}
			string separator = "";
			if (Request.QueryString["separator"] != null)
			{
				separator = Request.QueryString["separator"];
			}
			string selectids = "";
			if (Request.QueryString["selectids"] != null)
			{
				selectids = Request.QueryString["selectids"];
			}
			string selecttitles = "";
			if (Request.QueryString["selecttitles"] != null)
			{
				selecttitles = Request.QueryString["selecttitles"];
			}
			
			string target_page = "";
			if (Request.QueryString["target_page"] != null)
			{
				target_page = Request.QueryString["target_page"];
			}
			
			
			string strTitle = "";
			if (Request.QueryString["title"] != null)
			{
				strTitle = Request.QueryString["title"];
			}
			
			string strType = "";
			if (Request.QueryString["type"] != null)
			{
				strType = Request.QueryString["type"];
			}
			
			string strTagType = "";
			if (Request.QueryString["tagtype"] != null)
			{
				strTagType = Request.QueryString["tagtype"];
			}
			
			string strId = "";
			if (Request.QueryString["id"] != null)
			{
				strId = Request.QueryString["id"];
			}
			
			string menuFlag = "";
			if (Request.QueryString["menuflag"] != null)
			{
				menuFlag = (string) ("&menuflag=" + Request.QueryString["menuflag"]);
			}
			
			string langType = "";
			if (Request.QueryString["LangType"] != null)
			{
				langType = Request.QueryString["LangType"];
			}
			
			sb.Append("<frameset rows=\"60%, 40%\" border=\"1\"  frameSpacing=\"1\">" + "\r\n");
			if (target_page.Length > 0 && (0 <= target_page.IndexOf("MetaSelect.aspx")))
			{
				sb.Append("<frame marginwidth=\"0\" marginheight=\"0\" src=\"" + target_page + "?type=" + strType + "&LangType=" + langType + "&tagtype=" + strTagType + "&id=" + strId + "&title=" + strTitle + "&metadataFormTagId=" + metadataFormTagId + "&separator=" + separator + "&selectids=" + selectids + "&selecttitles=" + selecttitles + "\">" + "\r\n");
			}
			else
			{
				sb.Append("<frame marginwidth=\"0\" marginheight=\"0\" src=\"SelectFolder.aspx?FolderID=" + FolderID + "&browser=" + browser + "&WantXmlInfo=" + WantXmlInfo + "&metadataFormTagId=" + metadataFormTagId + "&separator=" + separator + "&selectids=" + selectids + "&selecttitles=" + selecttitles + menuFlag + "\">" + "\r\n");
			}
			sb.Append("<frame marginwidth=\"0\" marginheight=\"0\" src=\"mediaselect.aspx\">" + "\r\n");
			sb.Append("</frameset>" + "\r\n");
			
			frameset_lit.Text = sb.ToString();
			sb = null;
		}
		
		protected void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
		}	
	}

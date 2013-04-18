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
using Ektron.Cms.Workarea;


	public partial class threadeddisc_emoticon_select : System.Web.UI.Page
	{
        public threadeddisc_emoticon_select()
        {
            getEmoticons = m_refContentApi.EkContentRef;
           // aRep = Array.CreateInstance(typeof(Ektron.Cms.ReplaceWord), 0);

        }
		
		protected ContentAPI m_refContentApi = new Ektron.Cms.ContentAPI();
		protected bool isEWebEditPro = false;
		protected string sEditorName = "";

        protected Ektron.Cms.Content.EkContent getEmoticons;
        protected ReplaceWord[] aRep = (ReplaceWord[])Array.CreateInstance(typeof(Ektron.Cms.ReplaceWord), 0);
		protected bool IsEmoticon = true;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            string base_path = m_refContentApi.RequestInformationRef.ApplicationPath + "threadeddisc/emoticons/";
			base_path = base_path.Replace("//", "/");
			if (Request.QueryString["sEditorName"] == null)
			{
				isEWebEditPro = false;
			}
			else
			{
				isEWebEditPro = true;
				sEditorName = Request.QueryString["sEditorName"];
			}
			// Get all the emoticons
			aRep = getEmoticons.SelectReplaceWord(0, IsEmoticon);
			StringBuilder sb = new StringBuilder();
			int j = 0;
			sb.Append("<td>");
			while (j < aRep.Length)
			{
				sb.Append("<img id=\"emoticon_" + j + "\"");
                sb.Append(" tabindex=\"0\"");
                sb.Append(" ");
				sb.Append("alt=\"" + aRep[j].OldWord + "\"");
				sb.Append(" ");
				sb.Append("src=\"" + base_path + aRep[j].NewWord + "\"");
				sb.Append(" ");
				sb.Append("onclick=\"InsertEmoticon(\'" + j + "\', \'" + base_path + aRep[j].NewWord + "\')\"");
				sb.Append("\"/> ");
				System.Math.Max(System.Threading.Interlocked.Increment(ref j), j - 1);
			}
			sb.Append("</td>");
			ltrImage.Text = sb.ToString();
			
		}
	}
	


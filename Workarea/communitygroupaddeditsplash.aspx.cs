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

public partial class communitygroupaddeditsplash : System.Web.UI.Page
	{	
		
		protected int tlang = 0;
		protected long tid = 0;
		protected long profileTaxonomyId = 0;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			StringBuilder sbMoment = new StringBuilder();
			string sURL = "";
			Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
			if (! Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(api.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.SocialNetworking))
			{
				Utilities.ShowError(api.EkMsgRef.GetMessage("feature locked error"));
			}
			tlang = api.DefaultContentLanguage;
			try
			{
                if ((!string.IsNullOrEmpty(Request.QueryString["tlang"])) && Information.IsNumeric(Request.QueryString["tlang"]) && Convert.ToInt32(Request.QueryString["tlang"].ToString()) > 0)
				{
                    tlang = Convert.ToInt32(Request.QueryString["tlang"].ToString());
				}
				if (tlang == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED || tlang == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
				{
					tlang = api.DefaultContentLanguage;
				}
                if ((!string.IsNullOrEmpty(Request.QueryString["tid"])) && Information.IsNumeric(Request.QueryString["tid"]) && Convert.ToInt64(Request.QueryString["tid"].ToString()) > 0)
				{
                    tid = Convert.ToInt64(Request.QueryString["tid"].ToString());
				}
                if ((!string.IsNullOrEmpty(Request.QueryString["profileTaxonomyId"])) && Information.IsNumeric(Request.QueryString["profileTaxonomyId"]) && Convert.ToInt64(Request.QueryString["profileTaxonomyId"].ToString()) > 0)
				{
                    profileTaxonomyId = Convert.ToInt64(Request.QueryString["profileTaxonomyId"].ToString());
				}
				sURL = (string) ("communitygroupaddedit.aspx?thickbox=true" + (tid > 0 ? ("&tid=" + tid.ToString()) : "") + (tlang > 0 ? ("&LangType=" + tlang.ToString()) : "") + (profileTaxonomyId > 0 ? ("&profileTaxonomyId=" + profileTaxonomyId.ToString()) : ""));
				sbMoment.Append("One Moment Please...").Append(Environment.NewLine);
				sbMoment.Append("<script type=\"text/javascript\" language=\"Javascript\">").Append(Environment.NewLine);
				sbMoment.Append("   setTimeout(\"location.href=\'").Append(sURL).Append("\'\",1000); ").Append(Environment.NewLine);
				sbMoment.Append("</script>").Append(Environment.NewLine);
				
				ltr_go.Text = sbMoment.ToString();
			}
			catch (Exception)
			{
				
			}
			finally
			{
				sbMoment = null;
			}
			
		}
		
	}

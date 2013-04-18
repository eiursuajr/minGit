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

	public partial class ContentDesigner_configurations_InterfaceBlog : ContentDesignerConfigurationBase
	{

        protected string strButtons = "";
		protected Hashtable options = new Hashtable();
		protected bool IsForum = false;

        protected override void Page_Load(System.Object sender, System.EventArgs e)
		{
            string strValue = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["mode"]))
			strValue = Request.QueryString["mode"];
			if (strValue == "forum")
			{
                if (!string.IsNullOrEmpty(Request.QueryString["toolButtons"]))
				strButtons = Request.QueryString["toolButtons"];
				IsForum = true;
				string[] arTools = strButtons.ToLower().Split(",".ToCharArray());
				foreach (string item in arTools)
				{
					if ((options.ContainsKey(item)) == false)
					{
						options.Add(item, item);
					}
				}
			}
			base.Page_Load(sender, e);
		}
	}
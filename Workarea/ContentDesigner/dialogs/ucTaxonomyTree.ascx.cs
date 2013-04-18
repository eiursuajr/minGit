using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using Ektron.Cms;
using Ektron.Cms.API;

namespace Ektron.ContentDesigner.Dialogs
{
	public partial class TaxonomyTree : System.Web.UI.UserControl
	{
		public string Filter = "";
		protected SiteAPI m_refSiteApi = new SiteAPI();
		protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

		protected void Page_Load(object sender, EventArgs e)
		{
			m_refMsg = m_refSiteApi.EkMsgRef;
			ContentAPI capi = new ContentAPI();
			if (_fid > -1)
			{
				FolderData fd = capi.GetFolderById(_fid, true, false);
				if (fd != null)
				{
					if (fd.FolderTaxonomy.Length == 0)
					{
						noTaxonomies.Text = m_refMsg.GetMessage("generic no taxonomy");
                        noTaxonomies.ToolTip = m_refMsg.GetMessage("generic no taxonomy");
					}

					taxRequired.InnerText = fd.CategoryRequired.ToString().ToLower();

					if (!IsPostBack)
					{
						taxonomies.DataSource = fd.FolderTaxonomy;
						taxonomies.DataBind();

						if (fd.FolderTaxonomy.Length > 0 && defaultTaxID > -1)
						{
							//output path to selected taxonomy
							TaxonomyRequest taxrequest = new TaxonomyRequest();
							taxrequest.IncludeItems = false;
							taxrequest.Page = Page;
							taxrequest.TaxonomyLanguage = capi.RequestInformationRef.ContentLanguage;
							taxrequest.TaxonomyId = defaultTaxID;
							taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
							TaxonomyData td = capi.EkContentRef.LoadTaxonomy(ref taxrequest);
							if (td != null)
							{
								txtselectedTaxonomyNodes.Text = td.TaxonomyPath;
							}
						}
					}
				}
			}
			else
			{
				if (!IsPostBack)
				{
					Ektron.Cms.API.Content.Taxonomy tax = new Ektron.Cms.API.Content.Taxonomy();
					TaxonomyRequest tr = new TaxonomyRequest();
					tr.IncludeItems = false;
					tr.Depth = 1;
					tr.Page = Page;
					tr.TaxonomyId = 0;
					tr.TaxonomyLanguage = capi.RequestInformationRef.ContentLanguage;
					tr.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
					tr.TaxonomyItemType = Ektron.Cms.Common.EkEnumeration.TaxonomyItemType.Content;
					TaxonomyBaseData[] td = capi.EkContentRef.ReadAllSubCategories(tr);
					taxonomies.DataSource = td;
					taxonomies.DataBind();
				}
			}

			// Register JS
			JS.RegisterJS(this, JS.ManagedScript.EktronJS);
			JS.RegisterJS(this, JS.ManagedScript.EktronTreeviewJS);

			// Register CSS
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronTreeviewCss);
		}

		private List<long> _tid = new List<long>();
		public List<long> SelectedTaxonomies
		{
			get
			{
				if (txtselectedTaxonomyNodes.Text != "")
				{
					return new List<long>(
						(new List<string>(
							txtselectedTaxonomyNodes.Text.Split(','))
						).ConvertAll<long>(
							delegate(string input)
							{
								long val = 0;
								if (long.TryParse(input, out val))
									return val;
								else
									return 0;
							})
					);
				}
				else
				{
					return new List<long>();
				}
			}
		}

		private long _fid = -1;
		public long FolderID
		{
			get { return _fid; }
			set { _fid = value; }
		}

		private long _defaulttid = -1;
		public long defaultTaxID
		{
			get { return _defaulttid; }
			set { _defaulttid = value; }
		}
	}
}

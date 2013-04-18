using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Ektron.Cms.Widget;
using Ektron.Cms.API;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Workarea_Widgets_CreateReport : WorkareaWidgetBaseControl, IWidget
{
    #region Widget Properties
    [WidgetDataMember("Filter Listing")]
    public string TitleString { get; set; }

    [WidgetDataMember(20)]
    public int ResultsPerPage { get; set; }
	protected ContentAPI contAPI = new ContentAPI();

    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Content Dashboard";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        _host.HelpFile = Page.ResolveClientUrl("~/WorkArea/help/personalization_new.82.1.html");

        ViewSet.SetActiveView(View);
        Ektron.Cms.ContentAPI cApi = new Ektron.Cms.ContentAPI();

        chkLocaleNotIn.Text = this.GetMessage("chk Does not include");
        btnFilter.Text = this.GetMessage("lbl advanced search filter results label");
        btnPrint.Text = this.GetMessage("lbl print");
        btnCreate.Text = this.GetMessage("btn Create Report");
        CancelButton.Text = this.GetMessage("generic cancel");
        SaveButton.Text = this.GetMessage("btn save");

        Utilities.ValidateUserLogin();
        if (!cApi.IsAdmin() && !cApi.IsARoleMember(EkEnumeration.CmsRoleIds.AdminUsers) && !cApi.IsARoleMember(EkEnumeration.CmsRoleIds.AdminXliff) && !cApi.IsARoleMember(EkEnumeration.CmsRoleIds.AdminTranslationState))
        {

            Response.Redirect((string)(cApi.RequestInformationRef.ApplicationPath + "reterror.aspx?info=Please login as an administrator or an Xliff Admin or a Translation state admin"), true);
            return;
        }

        ReportGrid1.ItemsPerPage = cApi.RequestInformationRef.PagingSize;

        if (!Page.IsCallback)
        {
            // Initialize the filters
            ddlLocale.Items.Clear();
            ddlLocale.Items.Add(new ListItem("(All)", "0"));
            ddlLocale.Items.Add(new ListItem("(NULL)", "-1"));
            Dictionary<int, string> locales = ReportGrid1.GetShortLocaleList();
            foreach (int id in locales.Keys)
            {
                string localecode = locales[id];
                ListItem li = ddlLocale.Items.FindByText(localecode);
                if (li != null) // Compensate for variant-sort locale codes
                {
                    li.Text = li.Text + " (" + li.Value + ")";
                    localecode = locales[id] + " (" + id.ToString() + ")";
                }
                ddlLocale.Items.Add(new ListItem(localecode, id.ToString()));
            }

            ddlAuthor.Items.Clear();
            ddlAuthor.Items.Add(new ListItem("(All)", "0"));
            Dictionary<long, string> authors = ReportGrid1.GetAuthorList();
            foreach (long id in authors.Keys)
                ddlAuthor.Items.Add(new ListItem(authors[id], id.ToString()));

            Dictionary<long, string> folders = ReportGrid1.GetFolderList(0, false);
			bool isIE = Request.Browser.Browser.StartsWith("IE") || (!string.IsNullOrEmpty(Request.ServerVariables["User-Agent"]) && (Request.ServerVariables["User-Agent"].Contains("MSIE") || Request.ServerVariables["User-Agent"].Contains("Internet Explorer")));
            foreach (long id in folders.Keys)
			{
				string foldername = folders[id];
				if (isIE && foldername.Length > 32)
				{
					// Try to drop all but the last path
					string[] paths = foldername.Split('/');
					if (paths.Length <= 3) // Only one path, so we will just shorten it
						foldername = foldername.Substring(0, 30) + "...";
					else
					{
						// Generate some ../ instances to represent the depth
						int depth = paths.Length - 3;
						if (depth == 0) // Should never happen
							depth = 1;
						string path = paths[depth + 1];
						if (path.Length > (32 - depth))
							path = path.Substring(0, 30 - depth * 2) + "...";
						foldername = "../../../../../../../../../../../../../../../../../../../../".Substring(0, depth * 3) +
							path + "/";
					}
				}
				ListItem item = new ListItem(foldername, id.ToString());
                ddlFolderID.Items.Add(item);
				if (isIE)
					item.Attributes.Add("title", folders[id]);
			}

            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem(this.GetMessage("lbl Any"), string.Empty));
            ddlStatus.Items.Add(new ListItem(this.GetMessage("lbl not ready for translation"), Ektron.Cms.Localization.LocalizationState.NotReady.ToString()));
            ddlStatus.Items.Add(new ListItem(this.GetMessage("lbl ready for translation"), Ektron.Cms.Localization.LocalizationState.Ready.ToString()));
            //ddlStatus.Items.Add(new ListItem("Needs translation", Ektron.Cms.Localization.LocalizationState.NeedsTranslation.ToString()));
            //ddlStatus.Items.Add(new ListItem("Out for translation", Ektron.Cms.Localization.LocalizationState.OutForTranslation.ToString()));
            //ddlStatus.Items.Add(new ListItem("Translated", Ektron.Cms.Localization.LocalizationState.Translated.ToString()));
            ddlStatus.Items.Add(new ListItem(this.GetMessage("lbl Do not translate"), Ektron.Cms.Localization.LocalizationState.DoNotTranslate.ToString()));
            //ddlStatus.Items.Add(new ListItem("Unknown", Ektron.Cms.Localization.LocalizationState.Undefined.ToString()));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronUITabsCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
		Ektron.Cms.API.JS.RegisterJS(this, contAPI.AppPath + "/java/internCalendarDisplayFuncs.js", "EktronInternalCalendarDisplayJs");
    }

    protected void EditEvent(string settings)
    {
        txtTitle.Text = TitleString;

        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        TitleString = txtTitle.Text;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }
}
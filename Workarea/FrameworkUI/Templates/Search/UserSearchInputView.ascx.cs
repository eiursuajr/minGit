namespace Ektron.Cms.Framework.UI.Controls.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets;
    using Ektron.Cms.Framework.UI.Views;
    using Ektron.Cms.Interfaces.Context;
    using Ektron.Cms.Framework.UI.Controls.EktronUI;

    public partial class UserSearchInputView : BaseTemplate<IUserSearchView, UserSearchController>
    {
        protected List<UserPropertyFilter> AvailableFilters { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AddDirectorySearchButtons();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Visible)
            {
                ICmsContextService cmsContextService = ServiceFactory.CreateCmsContextService();
                aspDirectorySearchLink.InnerText = GetLocalResourceObject("ToggleButtonText") as string;
                aspDirectorySearchLink.HRef = cmsContextService.WorkareaPath + "/JavascriptRequired.aspx";
                aspDirectorySearchIcon.HRef = aspDirectorySearchLink.HRef;

                // create a package that will register the UI JS and CSS we need
                Package searchControlPackage = new Package() {
                    Components = new List<Component>()
                {
                    // Register JS Files
                    Packages.EktronCoreJS,
                    UI.JavaScript.Create(cmsContextService.UIPath + "/js/Ektron/Controls/Ektron.Controls.Search.UserSearch.js"),
                    Packages.jQuery.Plugins.BindReturnKey,
                    // Register CSS Files
                    Packages.Ektron.CssFrameworkBase,
                    UI.Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-search.css"),
                    UI.Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-search-users.css"),
                    UI.Css.Create(cmsContextService.UIPath + "/css/Ektron/Controls/ektron-ui-search-users.ie7.css", BrowserTarget.IE7)
                }
                };
                searchControlPackage.Register(this);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            UserSearchModel model = (UserSearchModel)DataBinder.GetDataItem(this.NamingContainer);
            this.AvailableFilters = model.AvailableFilters;

            if (!this.IsPostBack)
            {
                aspDirectorySearchFilters.DataSource = model.DirectorySearchFilters;
                aspDirectorySearchFilters.DataBind();
                aspDirectorySearchFilters.SelectedValue = "lastname";
            }
            base.OnDataBinding(e);
        }

        protected void uxBasicSearch_Click(object sender, EventArgs e)
        {
            this.Controller.BasicSearch(uxSearchText.Text);
            uxDirectorySearch.Attributes.Add("class", "directorySearch ektron-ui-hidden");
            uxSearchText.Enabled = true;
            uxBasicSearchButton.Enabled = true;
            ChangeCtlCss(aspDirectorySearchLink, "toggleDirectorySearch");
            ChangeCtlCss(aspDirectorySearchIcon, "toggleDirectorySearchIcon toggleDirectorySearch");
        }

        private void AddDirectorySearchButtons()
        {
            Ektron.Cms.Framework.UI.Controls.EktronUI.Button button1 = new Ektron.Cms.Framework.UI.Controls.EktronUI.Button();
            button1.ID = "btnAll";
            button1.Text = "All";
            button1.CommandName = "All";
            button1.Command += new CommandEventHandler(directorySearch_button_click);

            uxDirectoryButtons.Controls.Add(button1);

            for (char i = 'A'; i <= 'Z'; i++)
            {
                button1 = new Ektron.Cms.Framework.UI.Controls.EktronUI.Button();
                button1.ID = "btn" + i.ToString();
                button1.Text = i.ToString();
                button1.ToolTip = i.ToString();
                button1.CommandName = i.ToString();
                button1.Command += new CommandEventHandler(directorySearch_button_click);
                uxDirectoryButtons.Controls.Add(button1);
            }
        }

        void directorySearch_button_click(object sender, CommandEventArgs args)
        {
            DirectorySearchType sortBy = DirectorySearchType.LastName;
            switch (aspDirectorySearchFilters.SelectedValue)
            {
                case "displayname":
                    sortBy = DirectorySearchType.DisplayName;
                    break;
                case "lastname":
                    sortBy = DirectorySearchType.LastName;
                    break;
                default:
                    sortBy = DirectorySearchType.FirstName;
                    break;
            }
            switch (args.CommandName.ToLower())
            {
                case "all":
                    this.Controller.DirectorySearch(sortBy);
                    break;
                default:
                    this.Controller.DirectorySearch(sortBy, Convert.ToChar(args.CommandName));
                    break;
            }

            ((Ektron.Cms.Framework.UI.Controls.EktronUI.Button)sender).Checked = true;
            ((Ektron.Cms.Framework.UI.Controls.EktronUI.Button)sender).Enabled = true;
            uxSearchText.Enabled = false;
            uxBasicSearchButton.Enabled = false;
            uxDirectorySearch.Attributes.Add("class", "directorySearch");
            ChangeCtlCss(aspDirectorySearchLink, "toggleDirectorySearch hideDirectorySearch");
            ChangeCtlCss(aspDirectorySearchIcon, "toggleDirectorySearchIcon toggleDirectorySearch hideDirectorySearch");
        }

        private void ChangeCtlCss(System.Web.UI.HtmlControls.HtmlControl htmlControl, string css)
        {
            htmlControl.Attributes.Remove("class");
            htmlControl.Attributes.Add("class", css);
        }

    }
}
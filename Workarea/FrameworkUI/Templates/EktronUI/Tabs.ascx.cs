namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Web.UI.WebControls;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using System.Web.UI.HtmlControls;
    using System.Web;
    using System.Web.UI;

    public partial class Tabs : TemplateBase<EktronUI.Tabs>
    {
        private string tabOnClick;

        public string TabText { get; set; }
        public string TabClientID { get; set; }
        public string TabOnClick
        {
            get
            {
                string onclick = String.Empty;
                if (!String.IsNullOrEmpty(this.tabOnClick))
                {
                    onclick = "onclick=\"";
                    onclick += this.tabOnClick.Replace("\"", "'");
                    onclick += onclick.EndsWith(";") ? "" : ";";
                    onclick += "return false;\"";
                }
                return onclick;
            }
            set
            {
                this.tabOnClick = value;
            }
        }


        public Tabs()
        {
            this.ID = "Tabs";
        }

        protected override void OnInitialize(object sender, EventArgs e)
        {
            aspTabs.DataSource = this.ControlContainer.Tabs.Controls;
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                aspTabs.DataBind();
            }
        }

        protected void uxTabs_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                EktronUI.Tab tab = e.Item.DataItem as EktronUI.Tab;
                PlaceHolder dataBindHelper = e.Item.FindControl("aspDataBindHelper") as PlaceHolder;
                if (tab != null && tab.Visible == true)
                {
                    this.TabText = tab.Text;
                    this.TabClientID = tab.ClientID;
                    this.TabOnClick = tab.PostBackTrigger;

                    dataBindHelper.Visible = true;
                    dataBindHelper.DataBind();
                }
            }
        }
    }
}
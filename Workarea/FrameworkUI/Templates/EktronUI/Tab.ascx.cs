namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Web.UI.WebControls;
    using Ektron.Cms.Framework.UI.Controls.EktronUI.Core;
    using System.Web.UI.HtmlControls;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// Note: Tab ascx is for Accordion only.  Tab in context of Tabs does not load an ascx
    /// </summary>
    public partial class Tab : TemplateBase<EktronUI.Tab>
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
                    onclick += ";return false;\"";

                }
                this.tabOnClick = String.Empty;
                return onclick;
            }
            set
            {
                this.tabOnClick = value;
            }
        }


        public Tab()
        {
            this.ID = "AccordionTab";
        }

        protected override void OnInitialize(object sender, EventArgs e)
        {
            //content
            this.ControlContainer.ContentTemplate.InstantiateIn(aspContents);
        }

        protected override void OnRegisterResources(object sender, EventArgs e)
        {
            // register any required Javascript or CSS resources here:
        }

        protected override void OnReady(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                //header
                this.TabText = this.ControlContainer.Text;
                this.TabClientID = this.ControlContainer.ClientID;
                this.TabOnClick = this.ControlContainer.PostBackTrigger;
                aspAccordionHeader.DataBind();

                aspAccordionHeader.Attributes.Add("class", this.ControlContainer.CssClass);
            }
        }
    }
}
namespace Ektron.Cms.Framework.UI.Controls.EktronUI.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Specialized;
    using System.Web.UI.HtmlControls;
    using Ektron.Cms.Framework.UI.Controls;
    using Ektron.Cms.Framework.UI;
    using Ektron.Cms.Framework.UI.Controls.EktronUI;

    public partial class Pager : System.Web.UI.UserControl, IBindableTemplate
    {
        protected int numResults = 0;

        public Pager()
        {
            this.ID = "Pager";
        }

        public System.Collections.Specialized.IOrderedDictionary ExtractValues(Control container)
        {
            return new OrderedDictionary();
        }

        public void InstantiateIn(Control container)
        {
            container.Controls.Add(this);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            PageInfo model = (PageInfo)DataBinder.GetDataItem(this.NamingContainer);
            this.Visible = model.ResultCount > 0;

            var pagerBase = NamingContainer.NamingContainer as Ektron.Cms.Framework.UI.Controls.EktronUI.Widgets.PagerBase;
            if (pagerBase != null)
            {
                int trailingPage = Math.Max(model.CurrentPageIndex - pagerBase.TrailingPageCount, 0);
                int leadingPage = Math.Min(model.CurrentPageIndex + pagerBase.LeadingPageCount, model.NumberOfPages);

                EktronUI.Button button;
                aspPages.Controls.Clear();
                for (int i = trailingPage; i < leadingPage; i++)
                {
                    button = new EktronUI.Button();
                    button.DisplayMode = EktronUI.Button.MarkupMode.RadioButton;
                    button.ID = "page" + i.ToString();
                    button.Text = (i + 1).ToString();

                    if (i == model.CurrentPageIndex)
                    {
                        button.Checked = true;
                        button.Enabled = false;
                    }
                    button.CommandName = "Selected Page";
                    button.CommandArgument = button.Text;
                    button.Command += new CommandEventHandler(button_Command);

                    aspPages.Controls.Add(button);
                }
            }

            //set-up previous button
            uxPrevious.Enabled = (model.CurrentPageIndex > 0);

            //set-up next button
            uxNext.Enabled = (model.CurrentPageIndex < model.NumberOfPages - 1);

            base.OnDataBinding(e);
        }

        protected void uxPrevious_Click(object sender, EventArgs e)
        {
            EktronUI.Pager pager = NamingContainer.NamingContainer as EktronUI.Pager;
            if (pager != null)
            {
                int pageNum = int.Parse(aspCurrentPage.Value) - 1;
                if (pageNum < 0) pageNum = 0;
                pager.SetPage(pageNum);
            }
        }
        protected void uxNext_Click(object sender, EventArgs e)
        {
            EktronUI.Pager pager = NamingContainer.NamingContainer as EktronUI.Pager;
            if (pager != null)
            {
                pager.SetPage(int.Parse(aspCurrentPage.Value) + 1);
            }
        }
        protected void button_Command(object sender, CommandEventArgs args)
        {
            int nextPage = -1;
            EktronUI.Button button = sender as EktronUI.Button;
            if (button != null)
            {
                Int32.TryParse((string)args.CommandArgument, out nextPage);
            }
            EktronUI.Pager pager = NamingContainer.NamingContainer as EktronUI.Pager;
            if (pager != null && nextPage >= 0)
            {
                pager.SetPage(nextPage - 1);
            }
        }
    }
}
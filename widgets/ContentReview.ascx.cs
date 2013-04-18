using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;
using System.Collections.Generic;
using System.IO;

public partial class widget_ContentReview : System.Web.UI.UserControl
{

    #region properties
    private bool IsEditEvent = false;
    private string _StarStyleOption;
    private bool _DisplayRatingsHide;
    private int _pageSize;
    [WidgetDataMember("Ajax 5 Stars")]
    public string StarStyleOption { get { return _StarStyleOption; } set { _StarStyleOption = value; } }
    [WidgetDataMember(true)]
    public bool DisplayRatingsHide { get { return _DisplayRatingsHide; } set { _DisplayRatingsHide = value; } }
    [WidgetDataMember(10)]
    public int PageSize { get { return _pageSize; } set { _pageSize = value; } }
    #endregion

    IWidgetHost _host;

    protected void Page_Init(object sender, EventArgs e)
    {
        string sitepath = new CommonApi().SitePath;
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "ContentReview Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        this.PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs PreRenderE) { SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        StarStyleDropDownList.SelectedValue = StarStyleOption;
        HideReviewListCheckBox.Checked = DisplayRatingsHide;
        pagesize.Text = PageSize.ToString();
        IsEditEvent = true;
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        StarStyleOption = StarStyleDropDownList.SelectedValue;
        DisplayRatingsHide = HideReviewListCheckBox.Checked;
        if (!int.TryParse(pagesize.Text, out _pageSize)) _pageSize = 10;
        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        PageBuilder p = Page as PageBuilder;

        if (p != null && string.IsNullOrEmpty(Request.QueryString["id"]))
        {
			//-100 for target content widget.
            if (p.PageId > 0 | (p.PageId == -100 && IsEditEvent))
            {
                ContentReview1.DefaultContentID = p.PageId;
                ContentReview2.DefaultContentID = p.PageId;
            }
            else
            {
                ViewSet.SetActiveView(NotPageBuilderPage);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                int id = new int();
                int.TryParse(Request.QueryString["id"], out id);
                ContentReview1.DefaultContentID = id;
                ContentReview2.DefaultContentID = id;
            }            
        }

        ContentReview2.Visible = !DisplayRatingsHide;
        ContentReview2.MaxReviews = PageSize;
   
        ContentReview1.Fill();
        ContentReview2.Fill();

        string[] scripts = ExtractScript(ContentReview1.Text + ContentReview2.Text);
        //get any js we need to include
        foreach (string thisone in scripts)
        {
            JS.RegisterJSInclude(this, thisone, thisone + "JS");
            string filename = Path.GetFileNameWithoutExtension(thisone);
            JS.RegisterJSBlock(this, String.Format(@"
            if(typeof(Ektron.{0}) == ""undefined""){{               
                Ektron.{0} = {{rhdlr:rhdlr}};
            }} else {{                
                rhdlr = Ektron.{0}.rhdlr;
            }}", filename), filename + "JSBlock");
        }
    }

    protected string[] ExtractScript(string input)
    {
        List<string> output = new List<string>();
        output.AddRange(input.Split(new string[] { "<script" }, StringSplitOptions.RemoveEmptyEntries));
        output = output.ConvertAll<string>(new Converter<string, string>(delegate(string strIn)
        {
            int start = strIn.IndexOf("src");
            string tmp = strIn.Substring(strIn.IndexOf('\"', start) + 1);
            return tmp.Substring(0, tmp.IndexOf('\"'));
        }));
        return output.ToArray();
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

}






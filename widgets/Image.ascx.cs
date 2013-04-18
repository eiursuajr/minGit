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
using System.Text.RegularExpressions;
using System.Text;

public partial class Widgets_Image : System.Web.UI.UserControl, IWidget
{

    #region properties

    private long _ContentBlockId;
    private string _Width;
    private string _Height;
    private string _toolTip = "";
    private int _imageBorder = 0;

    [WidgetDataMember(0)]
    public long ContentBlockId { get { return _ContentBlockId; } set { _ContentBlockId = value; } }
    [WidgetDataMember("100")]
    public string Width { get { return _Width; } set { _Width = value; } }
    [WidgetDataMember("100")]
    public string Height { get { return _Height; } set { _Height = value; } }
    [WidgetDataMember("")]
    public string ToolTip { get { return _toolTip; } set { _toolTip = value; } }
    [WidgetDataMember(0)]
    public int ImageBorder { get { return _imageBorder; } set { _imageBorder = value; } }

    #endregion

    IWidgetHost _host;
    protected ContentAPI _api;
    protected string appPath;
    protected string sitePath;
    protected int langType;

    protected string uniqueId
    {
        get { return (ViewState[this.ID + "uniqueid"] == null) ? "" : (string)ViewState[this.ID + "uniqueid"]; }
        set { ViewState[this.ID + "uniqueid"] = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Image Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { MainView(); });
        _api = new ContentAPI();
        appPath = _api.AppPath;
        sitePath = _api.SitePath.TrimEnd('/');
        langType = _api.RequestInformationRef.ContentLanguage;
        CreateUniqueId();
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
        try
        {
            string webserviceURL = sitePath + "/widgets/image/ImageHandler.ashx";
            // Register JS
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJQueryClueTipJS);
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
            JS.RegisterJSInclude(this, sitePath + "/widgets/image/behavior.js", "ImageWidgetBehaviorJS");
            JS.RegisterJSInclude(this, sitePath + "/widgets/image/Image.js", "ImageJS");
            // Insert CSS Links
            Css.RegisterCss(this, sitePath + "/widgets/image/ImageStyle.css", "ImageWidgetCSS"); 

            JS.RegisterJSBlock(this, "Ektron.PFWidgets.Image.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.Image.setupAll('" + uniqueId + "');", "EktronPFWidgetsImageInit" + this.ID);

            txtWidth.Text = Width;
            txtHeight.Text = Height;
            txtToolTip.Text = ToolTip;
            txtBorder.Text = ImageBorder.ToString();

            ViewSet.SetActiveView(Edit);
            if (ContentBlockId > 0)
            {

                //load & set selected folder path
                Ektron.Cms.API.Library lib = new Library();
                Ektron.Cms.LibraryData ld = lib.GetLibraryItem(ContentBlockId);
                if (!ReferenceEquals(ld, null))
                {
                    long folderid = ld.ParentId;
                    txtSource.InnerText = ld.Title;
                    hdnContentId.Value = ld.Id.ToString();
                    hdnFolderId.Value = folderid.ToString();
                    hdnFolderPath.Value = folderid.ToString();

                    while (folderid != 0)
                    {
                        folderid = _api.GetParentIdByFolderId(folderid);
                        if (folderid > 0) hdnFolderPath.Value += "," + folderid.ToString();
                    }

                    //this will open the properties tab in edit mode
                    JS.RegisterJSBlock(this, "LoadPropertiesTab('" + uniqueId + "');", "LoadPropertiesTab" + this.ID);
                }

            }
        }
        catch (Exception e)
        {
            errorLb.Text = e.Message + e.Data + e.StackTrace + e.Source + e.ToString();
            _host.Title = _host.Title + " error";
            ViewSet.SetActiveView(View);
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        Int64 cid = 0;
        Int64.TryParse(hdnContentId.Value, out cid);
        if (cid > 0)
        {
            ContentBlockId = cid;
            Height = txtHeight.Text;
            Width = txtWidth.Text;
            ToolTip = txtToolTip.Text;
            int imgBorder = 0;
            int.TryParse(txtBorder.Text, out imgBorder);
            ImageBorder = imgBorder;
            _host.SaveWidgetDataMembers();
            MainView();
          
        }
        
        ViewSet.SetActiveView(View);
       
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    #region support
    protected void CreateUniqueId()
    {
        if (uniqueId == null || uniqueId == "")
        {
            uniqueId = System.Guid.NewGuid().ToString();
        }
    }
    #endregion

    protected void MainView()
    {
        if (ContentBlockId > 0)
        {
            Ektron.Cms.LibraryData libData = new LibraryData();
            Ektron.Cms.API.Library lib = new Library();
            libData = lib.GetLibraryItem(ContentBlockId);
            if (!ReferenceEquals(libData, null))
            {
                string title = libData.Title;
                string strTooltip = ToolTip.Trim() == "" ? libData.Title : ToolTip.Trim();
                string strHtml = string.Format(@"<img src=""{0}"" border=""{1}"" width=""{2}"" height=""{3}"" title=""{4}"" alt=""{5}""></img>",
                    libData.FileName.Replace("//", "/"), ImageBorder, Width, Height, strTooltip, strTooltip);
                ltrImage.Text = strHtml;
                ltrImage.Visible = true;
            }

            phContent.Visible = true;
            phHelpText.Visible = false;
        }
        else
        {
            phContent.Visible = false;
            phHelpText.Visible = true;
        }


        if (!(_host == null || _host.IsEditable == false))
        {
            divHelpText.Visible = true;
        }
        else
        {
            divHelpText.Visible = false;
        }

    }

}








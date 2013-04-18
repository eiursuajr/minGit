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



public partial class Widgets_Flash : System.Web.UI.UserControl, IWidget
{

    #region properties
    private string _Thumbnail;
    private string _Width;
    private string _Height;
    private long _ContentBlockId;
    private long _ThumbnailID;
    private bool _AutoStart;

    [WidgetDataMember(0)]
    public long ContentBlockId { get { return _ContentBlockId; } set { _ContentBlockId = value; } }
    [WidgetDataMember(0)]
    public long ThumbnailID { get { return _ThumbnailID; } set { _ThumbnailID = value; } }
    [WidgetDataMember("")]
    public string Thumbnail { get { return _Thumbnail; } set { _Thumbnail = value; } }
    [WidgetDataMember("")]
    public string Width { get { return _Width; } set { _Width = value; } }
    [WidgetDataMember("")]
    public string Height { get { return _Height; } set { _Height = value; } }
    [WidgetDataMember(false)]
    public bool AutoStart { get { return _AutoStart; } set { _AutoStart = value; } }
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
    private bool enableResize = true;


    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Flash Widget";
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
            string webserviceURL = sitePath + "/widgets/Flash/FlashHandler.ashx";
            // Register JS
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJQueryClueTipJS);
            JS.RegisterJSInclude(this, JS.ManagedScript.EktronScrollToJS);
            JS.RegisterJSInclude(this, sitePath + "/widgets/Flash/behavior.js", "FlashWidgetBehaviorJS");

            // Insert CSS Links
            Css.RegisterCss(this, sitePath + "/widgets/Flash/FlashStyle.css", "FlashWidgetCSS"); //cbstyle will include the other req'd stylesheets

            JS.RegisterJSBlock(this, "Ektron.PFWidgets.Flash.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.Flash.setupAll('" + uniqueId + "');", "EktronPFWidgetsFlashInit" + this.ID);

            txtWidth.Text = Width;
            txtHeight.Text = Height;
            chkAutostart.Checked = AutoStart;
            chkAutostart.Enabled = false;

            ViewSet.SetActiveView(Edit);

            thumbChange.Style.Add(HtmlTextWriterStyle.Display, "none");
            thumbRemove.Style.Add(HtmlTextWriterStyle.Display, "none");

            if (ContentBlockId > 0)
            {
                long folderid = _api.GetFolderIdForContentId(ContentBlockId);
                ContentBase cb = _api.EkContentRef.LoadContent(ContentBlockId, false);
                
                txtSource.InnerText = cb.Title;
                if (cb.AssetInfo != null && cb.AssetInfo.FileExtension.ToLower() == "flv") txtSource.InnerText += " (flv)";
                thumbChange.Style.Clear();

                if (ThumbnailID > 0)
                {
                    ContentRequest req = new ContentRequest();
                    req.ContentType = EkEnumeration.CMSContentType.AllTypes;
                    req.GetHtml = false;
                    req.Ids = ThumbnailID.ToString();
                    req.MaxNumber = 1;
                    req.RetrieveSummary = false;
                    Ektron.Cms.Common.ContentResult imageresult = _api.LoadContentByIds(ref req, Page);
                    if (imageresult != null && imageresult.Count > 0)
                    {
                        thumbnailImg.InnerHtml = "<img alt=\"thumbnail\" style=\"width:250px; height:auto;\" src=\"" + imageresult.Item[0].AssetInfo.FileName + "\"/>";
                        hdnThumbFile.Value = imageresult.Item[0].AssetInfo.FileName;
                        thumbRemove.Style.Clear();
                    }
                }
                else if (Thumbnail != string.Empty)
                {
                    thumbnailImg.InnerHtml = "<img alt=\"thumbnail\" style=\"width:250px; height:auto;\" src=\"" + Thumbnail + "\"/>";
                }

                hdnThumbID.Value = ThumbnailID.ToString();
                hdnThumbFolderPath.Value = "";
                if (ThumbnailID > 0)
                {
                    long thumbfolid = _api.GetFolderIdForContentId(ThumbnailID);
                    hdnThumbFolderPath.Value = thumbfolid.ToString();
                    while (thumbfolid != 0)
                    {
                        thumbfolid = _api.GetParentIdByFolderId(thumbfolid);
                        if (thumbfolid > 0) hdnThumbFolderPath.Value += "," + thumbfolid.ToString();
                    }
                }
                hdnContentId.Value = ContentBlockId.ToString();
                if (ContentBlockId == 0)
                {
                    hdnFolderId.Value = "-1";
                }
                else
                {
                    hdnFolderId.Value = folderid.ToString();
                }
                hdnVideoFolderPath.Value = folderid.ToString();
                while (folderid != 0)
                {
                    folderid = _api.GetParentIdByFolderId(folderid);
                    if (folderid > 0) hdnVideoFolderPath.Value += "," + folderid.ToString();
                }
                if (cb.AssetInfo != null && cb.AssetInfo.FileExtension == "flv")
                {
                    chkAutostart.Enabled = true;
                }
            }
            enableResize = false;
        }
        catch (Exception e)
        {
            errorLb.Text = e.Message + e.Data + e.StackTrace + e.Source + e.ToString();
            _host.Title = _host.Title + " error";
            ViewSet.SetActiveView(View);
            enableResize = true;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
       Int64 cid = 0;
        if (Int64.TryParse(hdnContentId.Value, out cid))
        {
            ContentBlockId = cid;
            Height = txtHeight.Text;
            Width = txtWidth.Text;
            AutoStart = chkAutostart.Checked;
            Thumbnail = hdnThumbFile.Value;
            Int64.TryParse(hdnThumbID.Value, out _ThumbnailID);
            _host.SaveWidgetDataMembers();
            MainView();
        }
        else
        {
            hdnContentId.Value = "";
            editError.Text = "Please select an object to play";

        }
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
       
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

    protected string ReplaceEncodeBrackets(string encodetext)
    {
        encodetext = Regex.Replace(encodetext, "&lt;", "<");
        encodetext = Regex.Replace(encodetext, "&gt;", ">");
        return encodetext;
    }

     protected void Page_PreRender(object sender, EventArgs e)
    {
        PageBuilder p = (Page as PageBuilder);
        if (p != null && p.Status == Mode.Editing)
        {
            string script = "";
            if (enableResize)
            {
                script = "Ektron.PageBuilder.WidgetHost.createResizables();";
            }
            else
            {
                script = "Ektron.PageBuilder.WidgetHost.destroyResizables();";
            }
            Ektron.Cms.API.JS.RegisterJSBlock(this, script, System.Guid.NewGuid().ToString());
        }
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

        if (ContentBlockId > -1)
        {
            ContentAPI capi = new ContentAPI();
            PageBuilder page = (Page as PageBuilder);
            if (ContentBlockId > 0)
            {
                contentBlock.DefaultContentID = ContentBlockId;
                if (page != null && page.CacheInterval > 0)
                {
                    contentBlock.CacheInterval = page.CacheInterval;
                }
                contentBlock.Fill();
                if (contentBlock.EkItem != null)
                {
                    if (contentBlock.EkItem.Title != null)
                        _host.Title = contentBlock.EkItem.Title;
                    if (contentBlock.EkItem.AssetInfo != null && contentBlock.EkItem.AssetInfo.FileExtension != null && contentBlock.EkItem.AssetInfo.FileExtension.ToLower() == "flv")
                    {
                        StringBuilder sbflash = new StringBuilder(); //need object tag here
                        sbflash.Append("<embed id=\"ply\" width=\"" + Width + "\" height=\"" + Height + "\"");
                        sbflash.Append("flashvars=\"file=" + contentBlock.EkItem.AssetInfo.FileName);
                        if (ThumbnailID > 0)
                        {
                            ContentRequest req = new ContentRequest();
                            req.ContentType = EkEnumeration.CMSContentType.AllTypes;
                            req.GetHtml = false;
                            req.Ids = ThumbnailID.ToString();
                            req.MaxNumber = 1;
                            req.RetrieveSummary = false;
                            Ektron.Cms.Common.ContentResult imageresult = capi.LoadContentByIds(ref req, Page);
                            if (imageresult != null && imageresult.Count > 0)
                            {
                                sbflash.Append("&image=" + imageresult.Item[0].AssetInfo.FileName);
                            }
                        }
                        else if (Thumbnail != string.Empty)
                        {
                            sbflash.Append("&image=" + Thumbnail);
                        }
                        sbflash.Append("&autostart=" + AutoStart.ToString().ToLower() + "\"");
                        sbflash.Append("allowscriptaccess=\"always\" allowfullscreen=\"true\" quality=\"high\" bgcolor=\"#CCCCCC\" name=\"ply\" style=\"\"");
                        sbflash.Append("src=\"" + sitePath + "/widgets/Flash/player.swf\" wmode=\"transparent\" type=\"application/x-shockwave-flash\"/>");
                        ltrFlash.Text = sbflash.ToString();
                        Ektron.Cms.API.JS.RegisterJSInclude(this, sitePath + "/widgets/flash/swfobject.js", "widgetFlash.js");
                        contentBlock.Visible = false;
                        ltrFlash.Visible = true;
                    }
                    else
                    {
                        //Ektron.Cms.Controls.ContentBlock contentBlock;
                        if (contentBlock.Text != "")
                        {
                            string html = contentBlock.Text;
                            int startwidth = -1, endwidth = -1, startheight = -1, endheight = -1;
                            startwidth = html.IndexOf("width=\"") + 7;
                            if (startwidth > 0)
                            {
                                endwidth = html.IndexOf("px", startwidth);
                            }
                            startheight = html.IndexOf("height=\"") + 8;
                            if (startheight > 0)
                            {
                                endheight = html.IndexOf("px", startheight);
                            }
                            if (startwidth > 0 && endwidth > startwidth && startheight > 0 && endheight > startheight)
                            {
                                string pixelwidth = html.Substring(startwidth, endwidth - startwidth);
                                string pixelheight = html.Substring(startheight, endheight - startheight);
                                int storedheight = 0;
                                int storedwidth = 0;
                                int.TryParse(Width, out storedwidth);
                                int.TryParse(Height, out storedheight);
                                if (storedheight > 0) html = html.Replace(pixelheight, storedheight.ToString());
                                if (storedwidth > 0) html = html.Replace(pixelwidth, storedwidth.ToString());
                            }
                            ltrFlash.Text = html;

                            contentBlock.Visible = false;
                            ltrFlash.Visible = true;
                        }
                    }
                }
            }
        }
        enableResize = true;
       
    }


}






   
    
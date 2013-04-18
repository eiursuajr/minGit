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
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;

public partial class Widgets_Flickr : System.Web.UI.UserControl, IWidget
{

    #region properties
    public string _ApiKey;
    public int _NumCols;
    public int _RecordsPerPage;
    public string _DisplayMode;
    public string _ImageSize;
    public string _WidgetName;

    public List<Photo> _PhotoCollection = null;

    private string _PhotoCollectionData = string.Empty;

    [GlobalWidgetData("8d14f34875c77761bc18730f39fb13c9")]
    public string ApiKey { get { return _ApiKey; } set { _ApiKey = value; } }

    [WidgetDataMember(3)]
    public int NumCols { get { return _NumCols; } set { _NumCols = value; } }

    [WidgetDataMember(5)]
    public int RecordsPerPage { get { return _RecordsPerPage; } set { _RecordsPerPage = value; } }

    [WidgetDataMember("Gallery")]
    public string DisplayMode { get { return _DisplayMode; } set { _DisplayMode = value; } }

    [WidgetDataMember("Flicker Widget")]
    public string WidgetName { get { return _WidgetName; } set { _WidgetName = value; } }


    [WidgetDataMember("_s")]
    public string ImageSize { get { return _ImageSize; } set { _ImageSize = value; } }


    [WidgetDataMember("")]
    public string PhotoCollectionData { get { return _PhotoCollectionData; } set { _PhotoCollectionData = value; } }

    public List<Photo> PhotoCollection
    {
        get
        {
            if (_PhotoCollection == null)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Photo>));
                byte[] data = ASCIIEncoding.Default.GetBytes(_PhotoCollectionData);
                using (MemoryStream memStream = new MemoryStream(data))
                {
                    try
                    {
                        _PhotoCollection = (List<Photo>)xmlSerializer.Deserialize(memStream);
                    }
                    catch
                    {
                        _PhotoCollection = new List<Photo>();
                    }
                }
            }

            return _PhotoCollection;
        }

        set
        {
            _PhotoCollection = value;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Photo>));
            using (MemoryStream memStream = new MemoryStream())
            {
                try
                {
                    xmlSerializer.Serialize(memStream, _PhotoCollection);
                    byte[] data = memStream.ToArray();
                    _PhotoCollectionData = ASCIIEncoding.UTF8.GetString(data);
                }
                catch { }
            }
        }
    }

    #endregion

    #region Page variables
    protected int intHeight = 0;
    protected string appPath = "";
    IWidgetHost _host;
    Ektron.Cms.CommonApi _api = new Ektron.Cms.CommonApi();

    #endregion Page variables

    #region Page related Event(s)

    #region Page Init Event
    /// <summary>
    /// Page Init Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = WidgetName;
        appPath = _api.ApplicationPath;

        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });

        //_host.ExpandOptions = Expandable.ExpandOnEdit;
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { ShowImages(); });

        Ektron.Cms.API.JS.RegisterJSInclude(tbData, Ektron.Cms.API.JS.ManagedScript.EktronJS);

        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/Flickr/js/Flickr.js", "EktronWidgetFlickrJS");
        Ektron.Cms.API.Css.RegisterCss(tbData, _api.SitePath + "widgets/Flickr/css/Flickr.css", "Flickrcss");

        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/Flickr/js/DragDrop/Mover/coordinates.js", "COORDINATESJS");
        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/Flickr/js/DragDrop/Mover/drag.js", "DRAGSJS");
        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/Flickr/js/DragDrop/Mover/dragdrop.js", "DRAGDROPJS");

        Ektron.Cms.API.Css.RegisterCss(tbData, _api.SitePath + "widgets/Flickr/css/DragDrop/DragDrop_Mover.css", "DRAGDROP_MOVERCSS");

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), @"jsloadScript" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function loadwidgetstate(){ if( document.getElementById('" + hdnGettabindex.ClientID + "')!=null)  {  var Hiddenvalue=document.getElementById('" + hdnGettabindex.ClientID + "').value;  if(Hiddenvalue!='-1') {   searchtext=document.getElementById('" + hdnSearchText.ClientID + "').value; searchtype=document.getElementById('" + hdnSeatchType.ClientID + "').value;oSort_by=document.getElementById('" + hdnSortBy.ClientID + "').value;  document.getElementById('" + ClientID + "sort_by').value=oSort_by; document.getElementById('" + ClientID + "SearchText').value=searchtext; document.getElementById('" + ClientID + "searchtype').value=searchtype; Ektron.Widget.Flickr.widgets['" + ClientID + "'].FirstImages(); Ektron.Widget.Flickr.widgets['" + ClientID + "'].SearchFirstImages();  if(Hiddenvalue=='1') { Ektron.Widget.Flickr.SwitchPane(document.getElementById('ImageListTab'), 'ImageListTab');} else if(Hiddenvalue=='2') { Ektron.Widget.Flickr.SwitchPane(document.getElementById('SearchLink'), 'SearchLink');    } else if(Hiddenvalue=='3') { Ektron.Widget.Flickr.SwitchPane(document.getElementById('Collection'), 'Collection'); if(document.getElementById('" + uxbtnRemove.ClientID + "')!=null) document.getElementById('" + uxbtnRemove.ClientID + "').style.display='block'; document.getElementById('helptext').style.display='block';   } else { Ektron.Widget.Flickr.SwitchPane(document.getElementById('Property'), 'Property');  } } } InitDragDrop();}</script>", false);
        //SaveButton.Attributes.Add("onclick", "javascript:getSort('" + hdnIdList.ClientID + "');");

        SaveButton.Attributes.Add("onclick", "javascript:return ValidateFlickrCollection('" + hdnImageCollectionCount.ClientID + "');getSort('" + hdnIdList.ClientID + "');");


        ViewSet.SetActiveView(View);

    }
    #endregion  #region Page Init Event

    #endregion Page related Event(s)

    #region Postback Event(s)

    #region Save Button Click
    /// <summary>
    ///  Save Button Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SaveButton_Click(object sender, EventArgs e)
    {

        try
        {
            NumCols = Convert.ToInt32(NumColsTextBox.Text);
            if (NumCols < 1)
            {
                NumCols = 1;
            }

            RecordsPerPage = Convert.ToInt32(uxRecordPerPage.Text);
            if (RecordsPerPage < 1)
            {
                RecordsPerPage = 1;
            }

            ImageSize = uxSizeDropDownList.SelectedValue;
            WidgetName = uxWdgetName.Text;
            // Set the new Widget title
            _host.Title = WidgetName;

            if (uxRadioGallery.Checked)
                DisplayMode = "Gallery";
            if (uxRadioList.Checked)
                DisplayMode = "List";
            if (PhotoCollection != null)
            {
                if (rptSelected.Items.Count > 0)
                {
                    for (int i = 0; i < rptSelected.Items.Count; i++)
                    {
                        if (PhotoCollection.Count > i)
                        {
                            PhotoCollection[i].ShortDesc = ((TextBox)rptSelected.Items[i].FindControl("uxtxtShortDesc")).Text;
                            PhotoCollection[i].ImageSize = ImageSize;
                        }
                    }
                }

                string strIdList = hdnIdList.Value.Replace(":ulTrash()", "");
                if (strIdList != string.Empty)
                {
                    // extract the list of the IDs
                    string[] strLists = strIdList.Split(new char[] { ':' });
                    strIdList = strLists[strLists.Length - 1];
                    strIdList = strIdList.Replace("ulSelected(", string.Empty);
                    strIdList = strIdList.Replace(")", string.Empty);

                }
                List<Photo> tempItems = PhotoCollection;
                System.Collections.Hashtable tempSortData = new System.Collections.Hashtable();
                if (strIdList.Length > 0)
                {
                    string[] srtList = strIdList.Split(',');
                    if (srtList.Length > 0)
                    {
                        for (int i = 0; i < srtList.Length; i++)
                        {
                            string id = srtList[i];
                            if (!(string.IsNullOrEmpty(id) || tempSortData.ContainsKey(srtList[i])))
                            {
                                tempSortData.Add(id, tempSortData.Count);
                            }

                        }
                    }
                }

                for (int itemC = 0; itemC < tempItems.Count; itemC++)
                {

                    if (tempSortData.ContainsKey(tempItems[itemC].Id))
                    {
                        tempItems[itemC].DisplayOrder = (int)tempSortData[tempItems[itemC].Id];
                    }
                }



                tempItems.Sort(delegate(Photo p1, Photo p2)
                {
                    return p1.DisplayOrder.CompareTo(p2.DisplayOrder);
                });
                PhotoCollection = tempItems;
            }

            //save collection as string
            this.PhotoCollection = PhotoCollection;

            _host.SaveWidgetDataMembers();

        }
        catch (Exception ex)
        {
            lbData.Text = "Error saving widget: " + ex.Message;
        }

        hdnGettabindex.Value = "-1";
        hdnIdList.Value = "";

        ViewSet.SetActiveView(View);
        ShowImages();
 
    }
    #endregion Save Button Click

    #region Add Collection Click
    /// <summary>
    ///  Add Collection Button Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddCollection_Click(object sender, EventArgs e)
    {
        try
        {
            AddToCollection();
            hdnGettabindex.Value = "1";
            Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "loadwidgetstate();", "jsloadwidgetstate" + this.ClientID);
        }
        catch (Exception ex)
        {
            lbData.Text = "Error saving widget: " + ex.Message;
            ViewSet.SetActiveView(View);
        } 
    }
    #endregion Add Collection Click

    #region Add Search Button Click
    /// <summary>
    /// Add Search Button Click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddSearch_Click(object sender, EventArgs e)
    {
        try
        {
            AddToCollection();
            hdnGettabindex.Value = "2";
            Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "loadwidgetstate();", "jsloadwidgetstate" + this.ClientID);
        }
        catch (Exception ex)
        {
            lbData.Text = "Error saving widget: " + ex.Message;
            ViewSet.SetActiveView(View);
        } 
    }
    #endregion Add Search Button Click

    #region Cancel Button Click
    /// <summary>
    ///  Cancel Button Click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        hdnGettabindex.Value = "-1";
        hdnIdList.Value = "";
        ViewSet.SetActiveView(View);
        ShowImages();       
    }
    #endregion Cancel Button Click

    #region Remove Button Click
    /// <summary>
    ///  Remove Button Click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void uxbtnRemove_Click(object sender, EventArgs e)
    {

        if (PhotoCollection != null)
        {


            if (rptSelected.Items.Count > 0 && PhotoCollection.Count > 0)
            {
                int j = 0;
                for (int i = 0; i < rptSelected.Items.Count; i++)
                {

                    if (((CheckBox)rptSelected.Items[i].FindControl("uxchkRemove")).Checked)
                    {
                        PhotoCollection.RemoveAt(j);
                        j = j - 1;
                    }
                    j++;
                }
            }
        }
        BindCollection();

        //save collection as string
        this.PhotoCollection = PhotoCollection;

        _host.SaveWidgetDataMembers();

        hdnGettabindex.Value = "3";
        Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "loadwidgetstate();", "jsloadwidgetstate" + this.ClientID);

    }
    #endregion  Remove Button Click

    #region uxGVPhotoList PageIndexChanging
    /// <summary>
    ///  Chane page Index and bind the grid again
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void uxGVPhotoList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        uxGVPhotoList.PageIndex = e.NewPageIndex;
        BindGridPhoto();
    }
    #endregion uxGVPhotoList PageIndexChanging

    #endregion Postback Event(s)

    #region Method(s)

    #region Edit Event
    /// <summary>
    ///  Edit Event Click
    /// </summary>
    /// <param name="settings">Setting String</param>
    void EditEvent(string settings)
    {
        //clear temp collection
        hdnPhotoCollection.Value = "";

        try
        {
            ApiKeyTextBox.Text = ApiKey;
            NumColsTextBox.Text = NumCols.ToString();
            uxRecordPerPage.Text = RecordsPerPage.ToString();
            uxWdgetName.Text = WidgetName;
            //uxtxtImgHeight.Text = ImageHeight;
            //uxtxtWidth.Text = ImageWidth;
            uxSizeDropDownList.SelectedValue = ImageSize;

            BindCollection();

            if (DisplayMode == "Gallery")
                uxRadioGallery.Checked = true;
            else
                uxRadioList.Checked = true;

            Ektron.Cms.API.JS.RegisterJSBlock(tbData, "Ektron.Widget.Flickr.AddWidget('" + this.ClientID + "', '" + tbData.ClientID + "', '" + SaveButton.ClientID + "', '" + ApiKey + "', '" + hdnPhotoCollection.ClientID + "', '" + hdnPane.ClientID + "', '" + hdnSearchText.ClientID + "', '" + hdnSeatchType.ClientID + "', '" + hdnSortBy.ClientID + "', '" + uxbtnRemove.ClientID + "');", "jsblock" + this.ClientID);

            Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "loadwidgetstate();", "jsloadwidgetstate" + this.ClientID);

            ViewSet.SetActiveView(Edit);
        }
        catch
        {
            lbData.Text = "Error editing widget";
            ViewSet.SetActiveView(View);
        }
    }
    #endregion  Edit Event

    #region Add To Collection
    /// <summary>
    /// Add Photo List To Collection
    /// </summary>
    protected void AddToCollection()
    {

        if (hdnPhotoCollection.Value != string.Empty)
        {
            string strCollectionData = hdnPhotoCollection.Value.Substring(1);

            if (strCollectionData.Length > 0)
            {
                string[] strPhotoList = strCollectionData.Split('~');
                int Recordcount = PhotoCollection.Count;
                if (strPhotoList.Length > 0)
                {
                    for (int i = 0; i < strPhotoList.Length; i++)
                    {
                        string[] strPhotoProperty = strPhotoList[i].Split('|');
                        if (strPhotoProperty.Length > 0)
                        {
                            Photo newPhoto = new Photo();
                            newPhoto.Id = strPhotoProperty[0].ToString();
                            newPhoto.Owner = strPhotoProperty[1].ToString();
                            newPhoto.Secret = strPhotoProperty[2].ToString();
                            newPhoto.Server = strPhotoProperty[3].ToString();
                            newPhoto.Farm = strPhotoProperty[4].ToString();
                            newPhoto.Title = strPhotoProperty[5].ToString();
                            newPhoto.ImageSize = ImageSize;
                            //newPhoto.ImgHeight = ImageHeight;
                            //newPhoto.ImgWidth = ImageWidth;
                            newPhoto.DisplayOrder = Recordcount + i;
                            PhotoCollection.Add(newPhoto);
                        }
                    }
                    BindCollection();

                    //save collection as string
                    this.PhotoCollection = PhotoCollection;

                    _host.SaveWidgetDataMembers();
                }
            }
        }
    }
    #endregion Add To Collection

    #region Show Images
    /// <summary>
    /// Show Images either in Gallery or List mode
    /// </summary>
    protected void ShowImages()
    {
        try
        {
            if (PhotoCollection.Count > 0)
            {
                if (DisplayMode == "Gallery")
                {
                    lbData.Text = "<center>";
                    for (int i = 0; i < PhotoCollection.Count; i++)
                    {
                        lbData.Text += PhotoCollection[i].ImageLinkforView;
                        if (i % NumCols == NumCols - 1)
                        {
                            lbData.Text += "<br />";
                        }
                    }
                    lbData.Text += "</center>";
                    uxGVPhotoList.Visible = false;
                }
                else
                {
                    lbData.Text = "";
                    BindGridPhoto();
                }

                phContent.Visible = true;
                phHelpText.Visible = false;
            }
            else
            {
                phContent.Visible = false;
                phHelpText.Visible = true;
            }

        }
        catch (Exception err)
        {
            lbData.Text += "Error in viewing widget: " + err.Message;
            ViewSet.SetActiveView(View);
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
    #endregion Show Images

    #region Bind Collection with reapeter
    /// <summary>
    /// Bind Collection with reapeter
    /// </summary>
    protected void BindCollection()
    {
        rptSelected.DataSource = PhotoCollection;
        rptSelected.DataBind();
        hdnImageCollectionCount.Value = PhotoCollection.Count.ToString();

        if (rptSelected.Items.Count > 0)
        {
            uxbtnRemove.Visible = true;
            uxNoDataAdded.Visible = false;
        }
        else
        {
            uxbtnRemove.Visible = false;
            uxNoDataAdded.Visible = true;
        }


        //Call calculate height method for setting List height
        CalculateHeight();
        Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "listStyle();", "jslistStyle" + this.ClientID);
    }
    #endregion Bind Collection with reapeter

    #region Bind Grid Photo List
    /// <summary>
    /// Bind photo list with grid 
    /// when display mode List id selected
    /// </summary>
    protected void BindGridPhoto()
    {
        uxGVPhotoList.Visible = true;
        uxGVPhotoList.PageSize = RecordsPerPage;
        uxGVPhotoList.DataSource = PhotoCollection;
        uxGVPhotoList.DataBind();
    }
    #endregion Bind Grid Photo List

    #region Calculate Height
    /// <summary>
    ///  Calculate Height of Collection listing
    /// </summary>
    protected void CalculateHeight()
    {
        intHeight = PhotoCollection.Count;
        intHeight = intHeight * 85;
    }
    #endregion Calculate Height

    #endregion Method(s)

}

#region Photo
[XmlRoot("Photo")]
public class Photo
{
    private string _Id;
    private string _Owner;
    private string _Secret;
    private string _Server;
    private string _Farm;
    private string _Title;
    private int _DisplayOrder;
    private string _ShortDesc;   
    private string _ImageSize;

    [XmlElement("ID")]
    public string Id { get { return _Id; } set { _Id = value; } }

    [XmlElement("O")]
    public string Owner { get { return _Owner; } set { _Owner = value; } }

    [XmlElement("ST")]
    public string Secret { get { return _Secret; } set { _Secret = value; } }

    [XmlElement("SR")]
    public string Server { get { return _Server; } set { _Server = value; } }

    [XmlElement("F")]
    public string Farm { get { return _Farm; } set { _Farm = value; } }

    [XmlElement("T")]
    public string Title { get { return _Title; } set { _Title = value; } }

    [XmlElement("SD")]
    public string ShortDesc { get { return _ShortDesc; } set { _ShortDesc = value; } }

    [XmlElement("IS")]
    public string ImageSize { get { return _ImageSize; } set { _ImageSize = value; } }

    [XmlElement("DO")]
    public int DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder = value; } }

    [XmlElement("IL")]
    public string ImageLink { get { return ImageTag(); } }

    [XmlElement("ILFV")]
    public string ImageLinkforView { get { return @"<a target=""new"" href=""http://www.flickr.com/photos/" + Owner + @"/" + Id + @"/"" alt=""" + ShortDesc + @""" title=""" + ShortDesc + @""">" + ImageTagforView() + @"</a>"; } }

    public string FormURL()
    {
        string retVal = "http://farm" + Farm + ".static.flickr.com/" + Server + "/" + Id + "_" + Secret + ImageSize + ".jpg";
        return retVal;
    }

    public string ImageTag()
    {
        string retVal = @"<img src=""" + "http://farm" + Farm + ".static.flickr.com/" + Server + "/" + Id + "_" + Secret + "_s.jpg" + @""" alt=""" + Title + @""" title=""" + Title + @"""  style=""padding:2px;""></img>";
        return retVal;
    }

    public string ImageTagforView()
    {
        string retVal = @"<img src=""" + FormURL() + @""" alt=""" + ShortDesc + @""" title=""" + ShortDesc + @"""  style=""padding:2px;""></img>";//width=""" + ImgWidth + "px" + @""" height=""" + ImgHeight + "px" + @"""
        return retVal;
    }

    public string ImageAndLinkTag()
    {
        string retVal = @"<a target=""new"" href=""http://www.flickr.com/photos/" + Owner + @"/" + Id + @"/"" alt=""" + Title + @""" title=""" + Title + @""">" + ImageTag() + @"</a>";
        return retVal;
    }
}
#endregion Photo



using System;
using System.Collections;
using System.Configuration;
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
public partial class widgets_ITunePodcast : System.Web.UI.UserControl, IWidget
{
    #region properties
    public int _NumCols;
    public int _RecordsPerPage;
    public string _DisplayMode;
    public string _ImageSize;
    public string _WidgetName;

    public List<Podcast> _PodcastCollection = null;

    private string _PodcastCollectionData = string.Empty;

    [WidgetDataMember(5)]
    public int RecordsPerPage { get { return _RecordsPerPage; } set { _RecordsPerPage = value; } }

    [WidgetDataMember("List")]
    public string DisplayMode { get { return _DisplayMode; } set { _DisplayMode = value; } }

    [WidgetDataMember("ITune Podcast Widget")]
    public string WidgetName { get { return _WidgetName; } set { _WidgetName = value; } }


    [WidgetDataMember("60")]
    public string ImageSize { get { return _ImageSize; } set { _ImageSize = value; } }


    [WidgetDataMember("")]
    public string PodcastCollectionData { get { return _PodcastCollectionData; } set { _PodcastCollectionData = value; } }

    public List<Podcast> PodcastCollection
    {
        get
        {
            if (_PodcastCollection == null)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Podcast>));
                byte[] data = ASCIIEncoding.Default.GetBytes(_PodcastCollectionData);
                using (MemoryStream memStream = new MemoryStream(data))
                {
                    try
                    {
                        _PodcastCollection = (List<Podcast>)xmlSerializer.Deserialize(memStream);
                    }
                    catch
                    {
                        _PodcastCollection = new List<Podcast>();
                    }
                }
            }

            return _PodcastCollection;
        }

        set
        {
            _PodcastCollection = value;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Podcast>));
            using (MemoryStream memStream = new MemoryStream())
            {
                try
                {
                    xmlSerializer.Serialize(memStream, _PodcastCollection);
                    byte[] data = memStream.ToArray();
                    _PodcastCollectionData = ASCIIEncoding.UTF8.GetString(data);
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

        appPath = _api.AppPath;
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });

        //_host.ExpandOptions = Expandable.ExpandOnEdit;
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { ShowPodcast(); });

        Ektron.Cms.API.JS.RegisterJSInclude(tbData, Ektron.Cms.API.JS.ManagedScript.EktronJS);

        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/ITunePodcast/js/ITunePodcast.js", "EktronWidgetITunePodcastJS");
        Ektron.Cms.API.Css.RegisterCss(tbData, _api.SitePath + "widgets/ITunePodcast/css/ITunePodcast.css", "ITunePodcastcss");

        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/ITunePodcast/js/DragDrop/Mover/coordinates.js", "COORDINATESJS");
        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/ITunePodcast/js/DragDrop/Mover/drag.js", "DRAGSJS");
        Ektron.Cms.API.JS.RegisterJSInclude(tbData, _api.SitePath + "widgets/ITunePodcast/js/DragDrop/Mover/dragdrop.js", "DRAGDROPJS");

        Ektron.Cms.API.Css.RegisterCss(tbData, _api.SitePath + "widgets/ITunePodcast/css/DragDrop/DragDrop_Mover.css", "DRAGDROP_MOVERCSS");

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), @"jsloadScript" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function ItunePodcastLoadwidgetstate(){ if( document.getElementById('" + hdnGettabindex.ClientID + "')!=null)  {  var Hiddenvalue=document.getElementById('" + hdnGettabindex.ClientID + "').value;  if(Hiddenvalue!='-1') {   searchtext=document.getElementById('" + hdnSearchText.ClientID + "').value; /*searchtype=document.getElementById('" + hdnSeatchType.ClientID + "').value;oSort_by=document.getElementById('" + hdnSortBy.ClientID + "').value;  document.getElementById('" + ClientID + "sort_by').value=oSort_by;*/ document.getElementById('" + ClientID + "SearchText').value=searchtext; /*document.getElementById('" + ClientID + "searchtype').value=searchtype;*/ Ektron.Widget.ITunePodcast.widgets['" + ClientID + "'].FirstImages(); Ektron.Widget.ITunePodcast.widgets['" + ClientID + "'].SearchFirstImages();  if(Hiddenvalue=='1') { Ektron.Widget.ITunePodcast.SwitchPane(document.getElementById('ImageListTab'), 'ImageListTab');} else if(Hiddenvalue=='2') { Ektron.Widget.ITunePodcast.SwitchPane(document.getElementById('SearchLink'), 'SearchLink');    } else if(Hiddenvalue=='3') { Ektron.Widget.ITunePodcast.SwitchPane(document.getElementById('Collection'), 'Collection'); if(document.getElementById('" + uxbtnRemove.ClientID + "')!=null) document.getElementById('" + uxbtnRemove.ClientID + "').style.display='block'; document.getElementById('helptext').style.display='block';   } else { Ektron.Widget.ITunePodcast.SwitchPane(document.getElementById('Property'), 'Property');  } } } InitDragDrop();}</script>", false);
        SaveButton.Attributes.Add("onclick", "javascript:return ValidateItunePodcastCollection('" + hdnPodcastCollectionCount.ClientID + "','" + hdnIdList.ClientID + "');");


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
            RecordsPerPage = Convert.ToInt32(uxRecordPerPage.Text);
            if (RecordsPerPage < 1)
            {
                RecordsPerPage = 1;
            }

            ImageSize = uxSizeDropDownList.SelectedValue;
            WidgetName = uxWdgetName.Text;
            // Set the new Widget title
            _host.Title = WidgetName;

            DisplayMode = "List";
            if (PodcastCollection != null)
            {
                if (rptSelected.Items.Count > 0)
                {
                    for (int i = 0; i < rptSelected.Items.Count; i++)
                    {
                        if (PodcastCollection.Count > i)
                        {
                            PodcastCollection[i].ImageSize = ImageSize;
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
                List<Podcast> tempItems = PodcastCollection;
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

                    if (tempSortData.ContainsKey(tempItems[itemC].TrackId))
                    {
                        tempItems[itemC].DisplayOrder = (int)tempSortData[tempItems[itemC].TrackId];
                    }
                }



                tempItems.Sort(delegate(Podcast p1, Podcast p2)
                {
                    return p1.DisplayOrder.CompareTo(p2.DisplayOrder);
                });
                PodcastCollection = tempItems;
            }

            //save collection as string
            this.PodcastCollection = PodcastCollection;

            _host.SaveWidgetDataMembers();

        }
        catch (Exception ex)
        {
            lbData.Text = "Error saving widget " + ex.InnerException ;
        }

        hdnGettabindex.Value = "-1";
        hdnIdList.Value = "";

        ViewSet.SetActiveView(View);
        ShowPodcast();
 
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
            Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "ItunePodcastLoadwidgetstate();", "jsloadwidgetstate" + this.ClientID);
        }
        catch (Exception ex)
        {
            lbData.Text = "Error saving widget " + ex.InnerException ;
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
            Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "ItunePodcastLoadwidgetstate();", "jsloadwidgetstate" + this.ClientID);
        }
        catch (Exception ex)
        {
            lbData.Text = "Error saving widget " + ex.InnerException ;
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
        ShowPodcast();       
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

        if (PodcastCollection != null)
        {


            if (rptSelected.Items.Count > 0 && PodcastCollection.Count > 0)
            {
                int j = 0;
                for (int i = 0; i < rptSelected.Items.Count; i++)
                {

                    if (((CheckBox)rptSelected.Items[i].FindControl("uxchkRemove")).Checked)
                    {
                        PodcastCollection.RemoveAt(j);
                        j = j - 1;
                    }
                    j++;
                }
            }
        }
        BindCollection();

        //save collection as string
        this.PodcastCollection = PodcastCollection;

        _host.SaveWidgetDataMembers();

        hdnGettabindex.Value = "3";
        Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "ItunePodcastLoadwidgetstate();", "jsloadwidgetstate" + this.ClientID);

    }
    #endregion  Remove Button Click

    #region uxGVPodcastList PageIndexChanging
    /// <summary>
    ///  Chane page Index and bind the grid again
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void uxGVPodcastList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        uxGVPodcastList.PageIndex = e.NewPageIndex;
        BindGridPodcast();
    }
    #endregion uxGVPodcastList PageIndexChanging

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
        hdnPodcastCollection.Value = "";

        try
        {
            uxRecordPerPage.Text = RecordsPerPage.ToString();
            uxWdgetName.Text = WidgetName;
            uxSizeDropDownList.SelectedValue = ImageSize;

            BindCollection();

            Ektron.Cms.API.JS.RegisterJSBlock(tbData, "Ektron.Widget.ITunePodcast.AddWidget('" + this.ClientID + "', '" + tbData.ClientID + "', '" + SaveButton.ClientID + "', '" + hdnPodcastCollection.ClientID + "', '" + hdnPane.ClientID + "', '" + hdnSearchText.ClientID + "', '" + hdnSeatchType.ClientID + "', '" + hdnSortBy.ClientID + "', '" + uxbtnRemove.ClientID + "');", "jsblock" + this.ClientID);

            Ektron.Cms.API.JS.RegisterJSBlock(tbData1, "ItunePodcastLoadwidgetstate();", "jsloadwidgetstate" + this.ClientID);

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
    /// Add Podcast List To Collection
    /// </summary>
    protected void AddToCollection()
    {

        if (hdnPodcastCollection.Value != string.Empty)
        {
            string strCollectionData = hdnPodcastCollection.Value.Substring(1);

            if (strCollectionData.Length > 0)
            {
                string[] strPodcastList = strCollectionData.Split('~');
                int Recordcount = PodcastCollection.Count;
                if (strPodcastList.Length > 0)
                {
                    for (int i = 0; i < strPodcastList.Length; i++)
                    {
                        string[] strPodcastProperty = strPodcastList[i].Split('|');
                        if (strPodcastProperty.Length > 0)
                        {
                            Podcast newPodcast = new Podcast();
                            newPodcast.TrackId = strPodcastProperty[0].ToString();
                            newPodcast.TrackName = strPodcastProperty[1].ToString();
                            newPodcast.CollectionName = strPodcastProperty[2].ToString();
                            newPodcast.ArtistName = strPodcastProperty[3].ToString();
                            newPodcast.TrackViewUrl = strPodcastProperty[4].ToString();
                            newPodcast.ArtworkUrl60 = strPodcastProperty[5].ToString();
                            newPodcast.ArtworkUrl100 = strPodcastProperty[6].ToString();
                            newPodcast.DisplayOrder = Recordcount + i;
                            PodcastCollection.Add(newPodcast);
                        }
                    }
                    BindCollection();

                    //save collection as string
                    this.PodcastCollection = PodcastCollection;

                    _host.SaveWidgetDataMembers();
                }
            }
        }
    }
    #endregion Add To Collection

    #region Show Podcasts
    /// <summary>
    /// Show Podcasts either in GList mode
    /// </summary>
    protected void ShowPodcast()
    {
        try
        {
            if (PodcastCollection.Count > 0)
            {
                lbData.Text = "";
                BindGridPodcast();

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
            lbData.Text += "Error in viewing widget " + err.InnerException ;
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
        rptSelected.DataSource = PodcastCollection;
        rptSelected.DataBind();
        hdnPodcastCollectionCount.Value = PodcastCollection.Count.ToString();

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

    #region Bind Grid Podcast List
    /// <summary>
    /// Bind photo list with grid 
    /// when display mode List id selected
    /// </summary>
    protected void BindGridPodcast()
    {
        uxGVPodcastList.Visible = true;
        uxGVPodcastList.PageSize = RecordsPerPage;
        uxGVPodcastList.DataSource = PodcastCollection;
        uxGVPodcastList.DataBind();
    }
    #endregion Bind Grid Podcast List

    #region Calculate Height
    /// <summary>
    ///  Calculate Height of Collection listing
    /// </summary>
    protected void CalculateHeight()
    {
        intHeight = PodcastCollection.Count;
        intHeight = intHeight * 85;
    }
    #endregion Calculate Height

    #endregion Method(s)

}

#region Podcast
[XmlRoot("Podcast")]
public class Podcast
{
    private string _TrackId;
    private string _TrackName;
    private string _CollectionName;
    private string _ArtistName;
    private string _TrackViewUrl;
    private string _ArtworkUrl60;
    private string _ArtworkUrl100;
    private int _DisplayOrder;
    private string _ImageSize;

    [XmlElement("ID")]
    public string TrackId { get { return _TrackId; } set { _TrackId = value; } }

    [XmlElement("TN")]
    public string TrackName { get { return _TrackName; } set { _TrackName = value; } }

    [XmlElement("CN")]
    public string CollectionName { get { return _CollectionName; } set { _CollectionName = value; } }

    [XmlElement("AN")]
    public string ArtistName { get { return _ArtistName; } set { _ArtistName = value; } }

    [XmlElement("TVU")]
    public string TrackViewUrl { get { return _TrackViewUrl; } set { _TrackViewUrl = value; } }

    [XmlElement("AU60")]
    public string ArtworkUrl60 { get { return _ArtworkUrl60; } set { _ArtworkUrl60 = value; } }

    [XmlElement("AU100")]
    public string ArtworkUrl100 { get { return _ArtworkUrl100; } set { _ArtworkUrl100 = value; } }

    [XmlElement("IS")]
    public string ImageSize { get { return _ImageSize; } set { _ImageSize = value; } }

    [XmlElement("DO")]
    public int DisplayOrder { get { return _DisplayOrder; } set { _DisplayOrder = value; } }

    [XmlElement("IL")]
    public string ImageLink { get { return ImageTag(); } }

    [XmlElement("ILFV")]
    public string ImageLinkforView { get { return @"<a target=""new"" href=""" + TrackViewUrl + @""">" + ImageTagforView() + @"</a>"; } }

    public string FormURL()
    {
        string retVal = "";
        return retVal;
    }

    public string ImageTag()
    {
        string retVal = @"<img src=""" + ArtworkUrl60 + @""" alt=""" + TrackName + @""" title=""" + TrackName + @"""  style=""padding:2px;""></img>";
        return retVal;
    }

    public string ImageTagforView()
    {
        string retVal = @"<img src=""" + (ImageSize == "60" ? ArtworkUrl60 : ArtworkUrl100) + @""" alt=""" + TrackName + @""" title=""" + TrackName + @"""  style=""padding:2px;""></img>";
        return retVal;
    }

    public string ImageAndLinkTag()
    {
        string retVal = @"<a target=""new"" href=""" + TrackViewUrl + @""" alt=""" + TrackName + @""" title=""" + TrackName + @""">" + ImageTag() + @"</a>";
        return retVal;
    }
}
#endregion Podcast
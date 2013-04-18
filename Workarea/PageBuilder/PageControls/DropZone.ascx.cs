using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms.PageBuilder;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Newtonsoft.Json;
using System.ComponentModel;
using System.Security.Permissions;
using Ektron.Cms.Common;

namespace Ektron.Cms.PageBuilder.Controls
{
    #region JSON action classes
    [JsonObject]
    public class WidgetLocation : IWidgetDropInfo
    {
        private long _widgetTypeID;
        private string _dropZoneID;
        private long _ColumnID;
        private long _OrderID;
        private int _Width;
        private string _Unit;
        private bool _isNested;
        private long _nestedSortOrder;
        private Guid _columnGuid = Guid.Empty;

        [JsonProperty]
        public Guid columnGuid { get { return _columnGuid; } set { _columnGuid = value; } }
        [JsonProperty]
        public long widgetTypeID { get { return _widgetTypeID; } set { _widgetTypeID = value; } }
        [JsonProperty]
        public string dropZoneID { get { return _dropZoneID; } set { _dropZoneID = value; } }
        [JsonProperty]
        public long ColumnID { get { return _ColumnID; } set { _ColumnID = value; } }
        [JsonProperty]
        public long OrderID { get { return _OrderID; } set { _OrderID = value; } }
        [JsonProperty]
        public int Width { get { return _Width; } set { _Width = value; } }
        [JsonProperty]
        public string Unit { get { return _Unit; } set { _Unit = value; } }
        [JsonProperty]
        public bool isNested { get { return _isNested; } set { _isNested = value; } }
        [JsonProperty]
        public long nestedSortOrder { get { return _nestedSortOrder; } set { _nestedSortOrder = value; } }
    }

    [JsonObject]
    public class DropZoneInfo
    {
        private string _dropZoneID;
        private bool _isMaster;
        [JsonProperty]
        public string dropZoneID { get { return _dropZoneID; } set { _dropZoneID = value; } }
        [JsonProperty]
        public bool isMaster { get { return _isMaster; } set { _isMaster = value; } }
    }

    [JsonObject]
    public class JsonRequestDropZone
    {
        [JsonProperty]
        public string Action;
        [JsonProperty]
        public DropZoneInfo DropzoneInfo;
        [JsonProperty]
        public WidgetLocation OldWidgetLocation;
        [JsonProperty]
        public WidgetLocation NewWidgetLocation;
    }
    #endregion

    [ToolboxData("<{0}:UCDropZone runat=\"server\"></{0}:UCDropZone>")]
    [ParseChildren(true)]
    public partial class UCDropZone : DropZone
    {
        private string appPath = "";
        protected EkRequestInformation requestInformation;

        protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
        //protected SiteAPI m_refSiteApi = new SiteAPI();

        public override UpdatePanel UpdatePanel
        {
            get
            {
                return updatepanel;
            }
        }

        private List<Ektron.Cms.PageBuilder.ColumnData> _columndefs = new List<Ektron.Cms.PageBuilder.ColumnData>(); 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<Ektron.Cms.PageBuilder.ColumnData> ColumnDefinitions
        {
            get { return _columndefs; }
            set { _columndefs = value; }
        }

        #region events
        protected void Page_Init(object sender, EventArgs e)
        {
            requestInformation = ObjectFactory.GetRequestInfoProvider().GetRequestInformation();
            m_refMsg = new EkMessageHelper(requestInformation);
            appPath = requestInformation.ApplicationPath;

            columnDisplay.ItemDataBound += new RepeaterItemEventHandler(columnDisplay_ItemDataBound);
            (Page as PageBuilder).PageUpdated += new EventHandler(Page_Updated);
        }

        protected override void ZoneUpdated()
        {
            PBDropZoneError.Visible = false;
            updatepanel.Visible = true;
            updatepanel.Update();

            if ((Page as PageBuilder).Pagedata.pageID == -1 && isZoneEditable)
            {
                PBDropZoneError.Visible = true;
                updatepanel.Visible = false;
                PBDropZoneError.InnerText = "Could not load content";
            }
            else if ((Page as PageBuilder).Pagedata.pageID == 0 && isZoneEditable)
            {
                PBDropZoneError.Visible = true;
                updatepanel.Visible = false;
                PBDropZoneError.InnerText = "Could not restore content";
            }
            else
            {
                if (ZoneData == null)
                {
                    DropZoneData dzone = new DropZoneData();
                    dzone.Columns = new List<Ektron.Cms.PageBuilder.ColumnDataSerialize>();
                    dzone.DropZoneID = this.ID;
                    (Page as PageBuilder).Pagedata.Zones.Add(dzone);
                }
                if (ColumnDefinitions.Count > 0)
                {
                    ZoneData.Columns = ZoneData.Columns.FindAll(delegate(Ektron.Cms.PageBuilder.ColumnDataSerialize cd) { return cd.Guid != Guid.Empty; });
                    ZoneData.Columns.AddRange(ColumnDefinitions);
                }
                if (ZoneData.Columns.Count < 1)
                {
                    Ektron.Cms.PageBuilder.ColumnData col = new Ektron.Cms.PageBuilder.ColumnData();
                    col.columnID = 0;
                    ZoneData.Columns.Add(col);
                }
                List<Ektron.Cms.PageBuilder.ColumnData> displayColumns = Columns.FindAll(delegate(Ektron.Cms.PageBuilder.ColumnData col) { return col.Guid == Guid.Empty; });
                columnDisplay.DataSource = (ColumnDefinitions.Count > 0) ? ColumnDefinitions : columnDisplay.DataSource = displayColumns;
                columnDisplay.DataBind();
            }
        }
        #endregion

        #region actions
        protected void DropZonePanelLoad(object sender, EventArgs e)
        {
            AddColumn.ImageUrl = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/addcolumn_off.png";
            AddColumn.AlternateText = m_refMsg.GetMessage("lbl pagebuilder add column");

            if (IsPostBack)
            {
                string argument = Request.Form["__EVENTARGUMENT"];

                if (argument != null && argument != "" && argument.StartsWith("{\"Action\""))
                {
                    try
                    {
                        JsonRequestDropZone jsonRequest = JsonConvert.DeserializeObject<JsonRequestDropZone>(argument);

                        if (jsonRequest != null && ((jsonRequest.NewWidgetLocation != null && jsonRequest.NewWidgetLocation.dropZoneID == this.ID)
                            || (jsonRequest.DropzoneInfo != null && jsonRequest.DropzoneInfo.dropZoneID == this.ID)))
                        {
                            switch (jsonRequest.Action)
                            {
                                case "add":
                                    {
                                        //add the control to the pagehi again
                                        PageFactory.GetDropZone(this).Add(jsonRequest.NewWidgetLocation as IWidgetDropInfo);
                                        ZoneUpdated();
                                        break;
                                    }
                                case "move":
                                    {
                                        //remove control from old id, add to new, callback with instructions to update both?
                                        PageFactory.GetDropZone(this).Move(jsonRequest.OldWidgetLocation as IWidgetDropInfo, jsonRequest.NewWidgetLocation as IWidgetDropInfo);
                                        PageBuilder p = (Page as PageBuilder);
                                        p.View(p.Pagedata); //update all zones
                                        break;
                                    }
                                case "RemoveColumn":
                                    {
                                        //move controls in >= column to column--, decrease column count by 1
                                        PageFactory.GetDropZone(this).RemoveColumn(jsonRequest.NewWidgetLocation as IWidgetDropInfo);
                                        ZoneUpdated();
                                        break;
                                    }
                                case "ResizeColumn":
                                    {
                                        //set width of column
                                        PageFactory.GetDropZone(this).ResizeColumn(jsonRequest.NewWidgetLocation as IWidgetDropInfo);
                                        ZoneUpdated();
                                        break;
                                    }
                                case "SaveDZType":
                                    {
                                        //save new type of dropzone
                                        PageFactory.GetDropZone(this).SetDropZoneType(jsonRequest.DropzoneInfo.isMaster);
                                        ZoneUpdated();
                                        break;
                                    }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                    }
                }
            }
        }

        protected void AddColumn_click(object sender, EventArgs e)
        {
            Ektron.Cms.PageBuilder.ColumnData col = new Ektron.Cms.PageBuilder.ColumnData();
            List<Ektron.Cms.PageBuilder.ColumnData> tmp = ColumnData.ConvertFromColumnDataSerializeList(ZoneData.Columns);
            tmp.Sort(delegate(Ektron.Cms.PageBuilder.ColumnData l, Ektron.Cms.PageBuilder.ColumnData r) { return l.columnID.CompareTo(r.columnID); });
            col.columnID = tmp[tmp.Count - 1].columnID + 1;
            col.Guid = Guid.Empty;
            ZoneData.Columns.Add(col);
            UpdateViewState();
            ZoneUpdated();
        }
#endregion

        #region rendering

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            dzcontainer.Attributes.Clear();
            dzcontainer.Attributes["class"] = "dropzone PBClear";

            //update column links (add / remove)
            if (isZoneEditable)
            {
                if (Request["thumbnail"] == null)
                {
                    blockuiCall.Visible = true;
                    dzcontainer.Attributes.Add("EditMode", "true");
                }
                AddColumn.Visible = AllowAddColumn;
                masterzoneselect.Visible = (Page as PageBuilder).Pagedata.IsMasterLayout;
                if (AllowAddColumn || (Page as PageBuilder).Pagedata.IsMasterLayout)
                {
                    imgsetmasterzone.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/lock_off.png";
                    imgsetmasterzone.Alt = "Lock - Set as Layout Zone";

                    dzcontainer.Attributes["class"] = "dropzone PBClear PBDZhasHeader";
                    dzheader.Visible = true;
                }
                if (ZoneData.isMasterZone)
                {
                    dzcontainer.Attributes["class"] += " isMasterZone";
                }
            }
            else
            {
                dzheader.Visible = false;
            }

            for (int i = 0; i < columnDisplay.Items.Count; i++)
            {
                LinkButton l = (columnDisplay.Items[i].FindControl("RemColumn") as LinkButton);
                if (l != null) l.Visible = isZoneEditable;
            }

            if (Request["thumbnail"] != null && Request["thumbnail"] == "true")
            {
                if (!(Page as PageBuilder).Pagedata.IsMasterLayout || !ZoneData.isMasterZone)
                {
                    dzcontainer.Attributes["class"] += " dzthumbnail";
                }
            }
        }

        protected void columnDisplay_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //image paths
                //(e.Item.FindControl("imgleftcorner") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/images/column_leftcorner.png";
                //(e.Item.FindControl("imgrightcorner") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/images/column_rightcorner.png";
                (e.Item.FindControl("imgresizecolumn") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/edit_off.png";
                (e.Item.FindControl("imgresizecolumn") as HtmlImage).Alt = (e.Item.FindControl("lbResizeColumn") as HtmlAnchor).Title = m_refMsg.GetMessage("lbl pagebuilder resize");
                (e.Item.FindControl("imgremcolumn") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/icon_close.png";
                (e.Item.FindControl("imgremcolumn") as HtmlImage).Alt = (e.Item.FindControl("lbDeleteColumn") as HtmlAnchor).Title = m_refMsg.GetMessage("generic delete title") + " Column";

                (e.Item.FindControl("lbResizeColumn") as HtmlAnchor).Visible = AllowColumnResize;
                (e.Item.FindControl("lbDeleteColumn") as HtmlAnchor).Visible = AllowColumnResize;
                (e.Item.FindControl("lbResizeColumn") as HtmlAnchor).Title = (e.Item.FindControl("imgresizecolumn") as HtmlImage).Alt.ToString();
                (e.Item.FindControl("lbDeleteColumn") as HtmlAnchor).Title = (e.Item.FindControl("imgremcolumn") as HtmlImage).Alt.ToString();

                (e.Item.FindControl("headerItem") as HtmlControl).Visible = isZoneEditable;

                IColumnData thiscol = e.Item.DataItem as IColumnData;
                List<WidgetData> mywidgets = getColumnWidgets(thiscol.columnID);
                mywidgets.Sort(delegate(WidgetData left, WidgetData right) { return left.Order.CompareTo(right.Order); });

                HtmlControl column = (e.Item.FindControl("column") as HtmlControl);
                column.Attributes.Add("columnid", thiscol.columnID.ToString());
                column.Attributes.Add("columnguid", thiscol.Guid.ToString());

                HtmlControl zonediv = e.Item.FindControl("zone") as HtmlControl;
                if (thiscol.width > 0)
                {
                    string unit = (thiscol.unit == Units.pixels) ? "px" : (thiscol.unit == Units.em) ? "em" : "%";
                    zonediv.Style.Add("width", thiscol.width.ToString() + unit);
                }


                if (!isZoneEditable)
                {
                    zonediv.Attributes["class"] = "PBViewing";
                }
                else
                {
                    zonediv.Attributes.Add("dropzoneid", this.ID);
                    zonediv.Attributes["class"] = "PBColumn";
                    if (AllowColumnResize)
                    {
                        zonediv.Attributes.Add("resizable", "true");
                    }
                    else
                    {
                        zonediv.Attributes["class"] += " PBUnsizable";
                    }
                    if (thiscol.width == 0)
                    {
                        zonediv.Attributes["class"] += " PBColumnUnsized";
                    }
                }

                Repeater controlcolumn = (e.Item.FindControl("controlcolumn") as Repeater);
                controlcolumn.ItemDataBound += new RepeaterItemEventHandler(controlcolumn_ItemDataBound);

                controlcolumn.DataSource = mywidgets;
                controlcolumn.DataBind();
            }
        }

        protected void controlcolumn_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            if (item.ItemType != ListItemType.Header && item.ItemType != ListItemType.Footer && (item.DataItem as WidgetData) != null)
            {
                WidgetData w = item.DataItem as WidgetData;
                WidgetHostCtrl ctrl = (WidgetHostCtrl)item.FindControl("WidgetHost");
                ctrl.ColumnID = w.ColumnID;
                ctrl.SortOrder = w.Order;
                ctrl.WidgetHost_Load();
            }
        }
        #endregion
    }
}

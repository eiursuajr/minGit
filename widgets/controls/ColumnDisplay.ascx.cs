using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Ektron.Cms.PageBuilder;
using Ektron.Cms;
using System.ComponentModel;

[DefaultBindingProperty("Columns")]
public partial class ColumnDisplay : System.Web.UI.UserControl
{
	public class DeleteColumnEventArgs : EventArgs
	{
		private int _index;
		private Guid _guid;

		public int Index { get { return _index; } set { _index = value; } }
		public Guid Guid { get { return _guid; } set { _guid = value; } }

		public DeleteColumnEventArgs(int index, Guid guid)
		{
			this._index = index;
			this._guid = guid;
		}
	}

	private List<ColumnDisplayData> _columns = new List<ColumnDisplayData>();
	public List<ColumnDisplayData> Columns
	{
		get { return _columns; }
		set
		{
			_columns = value;
			repColumns.DataSource = _columns;
			repColumns.DataBind();
		}
	}

	private WidgetHost _host;
	public WidgetHost WidgetHost
	{
		get { return _host; }
		set { _host = value; }
	}

    public bool IsEditable { get; set; }

	public event EventHandler<DeleteColumnEventArgs> DeleteColumn;

    public ColumnDisplay()
    {
        IsEditable = true;
    }

	#region Widget-in-Widget Code
	protected void repColumns_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		ContentAPI _api = new ContentAPI();
		string appPath = _api.AppPath;
		SiteAPI m_refSiteApi = new SiteAPI();
		Ektron.Cms.Common.EkMessageHelper m_refMsg = m_refSiteApi.EkMsgRef;

		HtmlImage imgresizecolumn = (e.Item.FindControl("imgresizecolumn") as HtmlImage);
		HtmlImage imgremcolumn = (e.Item.FindControl("imgremcolumn") as HtmlImage);
		HtmlAnchor lbResizeColumn = (e.Item.FindControl("lbResizeColumn") as HtmlAnchor);
		LinkButton btnDeleteColumn = (e.Item.FindControl("btnDeleteColumn") as LinkButton);
		Repeater controlcolumn = (e.Item.FindControl("controlcolumn") as Repeater);

		HtmlControl zonediv = (e.Item.FindControl("zone") as HtmlControl);
		HtmlControl column = (e.Item.FindControl("column") as HtmlControl);
		HtmlControl headerItem = (e.Item.FindControl("headerItem") as HtmlControl);
		Label headerCaption = (e.Item.FindControl("HeaderCaption") as Label);

		//image paths
		//(e.Item.FindControl("imgleftcorner") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/images/column_leftcorner.png";
		//(e.Item.FindControl("imgrightcorner") as HtmlImage).Src = appPath + "/PageBuilder/PageControls/images/column_rightcorner.png";
		imgresizecolumn.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/edit_off.png";
		imgresizecolumn.Alt = lbResizeColumn.Title = m_refMsg.GetMessage("lbl pagebuilder resize");
		imgremcolumn.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/icon_close.png";
		imgremcolumn.Alt = btnDeleteColumn.Attributes["title"] = m_refMsg.GetMessage("generic delete title");

		lbResizeColumn.Visible = false;
		btnDeleteColumn.Visible = true;
		lbResizeColumn.Title = imgresizecolumn.Alt.ToString();

		headerItem.Visible = ((Page as PageBuilder).Status == Mode.Editing);

		ColumnDisplayData dataItem = (ColumnDisplayData)e.Item.DataItem;
		ColumnData thiscol = dataItem.Column;

		if (headerCaption != null)
		{
			headerCaption.Text = dataItem.Caption;
		}

		List<Ektron.Cms.PageBuilder.WidgetData> mywidgets = (Page as PageBuilder).Pagedata.Widgets.FindAll(delegate(Ektron.Cms.PageBuilder.WidgetData w) { return w.ColumnID == thiscol.columnID && w.ColumnGuid == thiscol.Guid && w.DropID == _host.ZoneID; });
		mywidgets.Sort(delegate(Ektron.Cms.PageBuilder.WidgetData left, Ektron.Cms.PageBuilder.WidgetData right) { return left.Order.CompareTo(right.Order); });


		//cool
		btnDeleteColumn.Click += new EventHandler(delegate(object delSender, EventArgs delArgs)
		{
			if (this.DeleteColumn != null)
				this.DeleteColumn(delSender, new DeleteColumnEventArgs(e.Item.ItemIndex, thiscol.Guid));
			_host.RemoveColumn(thiscol.Guid);
		});

		column.Attributes.Add("columnid", thiscol.columnID.ToString());
		column.Attributes.Add("columnguid", thiscol.Guid.ToString());

		zonediv.Style.Add("width", "100%");

		if ((Page as PageBuilder).Status != Mode.Editing || !IsEditable )
		{
			zonediv.Attributes["class"] = "PBViewing";
		}
		else
		{
			zonediv.Attributes.Add("dropzoneid", _host.ZoneID);
			zonediv.Attributes["class"] = "PBColumn";
			zonediv.Attributes["class"] += " PBUnsizable";
		}

		controlcolumn.ItemDataBound += new RepeaterItemEventHandler(controlcolumn_ItemDataBound);

		controlcolumn.DataSource = mywidgets;
		controlcolumn.DataBind();
	}

	protected void controlcolumn_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		RepeaterItem item = e.Item;
		if (item.ItemType != ListItemType.Header && item.ItemType != ListItemType.Footer && (item.DataItem as Ektron.Cms.PageBuilder.WidgetData) != null)
		{
			Ektron.Cms.PageBuilder.WidgetData w = item.DataItem as Ektron.Cms.PageBuilder.WidgetData;
			WidgetHostCtrl ctrl = (WidgetHostCtrl)item.FindControl("WidgetHost");
			ctrl.ColumnID = w.ColumnID;
			ctrl.ColumnGuid = w.ColumnGuid;
			ctrl.SortOrder = w.Order;
			ctrl.WidgetHost_Load();
		}
	}
	#endregion
}

public class ColumnDisplayData
{
	public ColumnDisplayData() { }
	public ColumnDisplayData(ColumnData data) : this(data, "") { }
	public ColumnDisplayData(ColumnData data, String Caption)
	{
		_data = data;
		_caption = Caption;
	}
	private ColumnData _data = null;
	public ColumnData Column
	{
		get { return _data; }
		set { _data = value; }
	}

	private String _caption = String.Empty;
	public String Caption
	{
		get { return _caption; }
		set { _caption = value; }
	}
}
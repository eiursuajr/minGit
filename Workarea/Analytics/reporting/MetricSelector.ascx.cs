using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Analytics_reporting_MetricSelector : WorkareaBaseControl
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		lblDisplay.Text = GetMessage("lbl display");
	}

	public enum SelectorCount
	{
		Single = 1,
		Dual = 2
	}

	public delegate void SelectionChangedHandler(object sender, EventArgs e);
	public event SelectionChangedHandler SelectionChanged;

	private SelectorCount _metricSelectors = SelectorCount.Dual;
	public SelectorCount MetricSelectors
	{
		get { return _metricSelectors; }
		set { _metricSelectors = value; }
	}

	public ListItem[] Items
	{
		set
		{
			Selector1.Items.Clear();
			Selector1.Items.AddRange(value);
			Selector1.SelectedIndex = 0;
			if ((int)_metricSelectors > 1)
			{
				Selector2.Items.Clear();
				Selector2.Items.Add(new ListItem(GetMessage("lbl unassigned"), ""));
				// need to copy items; can't share the same ListItem object
				foreach (ListItem item in value)
				{
					Selector2.Items.Add(new ListItem(item.Text, item.Value));
				}
			}
		}
	}

	public string SelectedValue
	{
		get
		{
			return Selector1.SelectedValue;
		}
	}

	public string SelectedValue2
	{
		get
		{
			return Selector2.SelectedValue;
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		bool dual = ((int)_metricSelectors > 1);
		Image1.Visible = dual;
		Image2.Visible = dual;
		Selector2.Visible = dual;
		base.OnPreRender(e);
	}
	
	protected virtual void DropDownList_SelectionChanged(object sender, EventArgs e)
	{
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

}

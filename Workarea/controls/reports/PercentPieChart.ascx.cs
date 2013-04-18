using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Ektron.Cms.Analytics.Reporting;

public partial class controls_reports_PercentPieChart : WorkareaBaseControl
{
	private List<float> _percentList = null;
    private List<string> _colorList = null;
    private List<string> _nameList = null;

	private float _minimumPercentToLabel = 0.05F;
	private float _minimumPercentToDraw = 0.01F;

	private const int _maxLabelNameLength = 16;
	private const int _pixelsPerNamedLabel = 18;

	protected override void Render(HtmlTextWriter writer)
	{
		if (_percentList != null)
		{
			if (Width.Type != UnitType.Pixel)
			{
				throw new ArgumentException("Width", "Width must be in pixels");
			}
			if (Height.Type != UnitType.Pixel)
			{
				throw new ArgumentException("Height", "Height must be in pixels");
			}
			if (null == imgChart)
			{
				imgChart = new Image();
				imgChart.ID = "imgChart";
				this.Controls.Add(imgChart);
			}
			imgChart.Attributes.Add("width", Width.Value.ToString("0"));
			imgChart.Attributes.Add("height", Height.Value.ToString("0"));
			imgChart.AlternateText = BriefDescription;
			imgChart.Attributes.Add("title", imgChart.AlternateText);

			StringBuilder sbUrl = new StringBuilder(this.GoogleChartBaseUrl);
			sbUrl.Append("?cht=p");
			if (DisplayType.Display3D == _display)
			{
				sbUrl.Append("3");
			}
			sbUrl.AppendFormat("&chs={0}x{1}", Width.Value, Height.Value);

			StringBuilder values = new StringBuilder();
			StringBuilder labels = new StringBuilder();
            StringBuilder colors = new StringBuilder();
            StringBuilder legend = new StringBuilder();
            StringBuilder legendcolors = new StringBuilder();
			string remainderColor = "";

			int legendHeight = (int)Height.Value;
			if (Legend == LegendPosition.BottomHorizontal || Legend == LegendPosition.BottomVertical || Legend == LegendPosition.TopHorizontal || Legend == LegendPosition.TopVertical)
			{
				legendHeight /= 2;
			}
			float totalValueShown = 0.0F;
			int numNamedLabels = 0;
			int maxNamedLabels = legendHeight / _pixelsPerNamedLabel;

			for (int i = 0; i < _percentList.Count; i++)
			{
				float itemPercent = _percentList[i];

				if (itemPercent >= _minimumPercentToDraw && numNamedLabels < maxNamedLabels) // reserve one named label for "Other"
				{
					if (values.Length > 0)
					{
						values.Append(",");
						labels.Append("|");
						colors.Append(",");
					}
					values.Append(itemPercent.ToString("0.00"));

					if (itemPercent >= _minimumPercentToLabel || Legend != LegendPosition.None)
					{
						if (Legend != LegendPosition.None)
						{
							string name = _nameList[i];
							if (name.Length > _maxLabelNameLength + 3)
							{
								name = name.Substring(0, _maxLabelNameLength / 2) + "..." + name.Substring(name.Length - _maxLabelNameLength / 2);
							}
							labels.Append(name + " ");
							numNamedLabels += 1;
						}
						labels.Append(itemPercent.ToString("0.00%"));
					}

					if (_colorList != null)
					{
						colors.Append(_colorList[i]);
					}
					totalValueShown += itemPercent;
				}
				else if (_colorList != null && String.IsNullOrEmpty(remainderColor))
				{
					remainderColor = _colorList[i];
				}
			}

			float remainder = 1.0F - totalValueShown;

			if (remainder > _minimumPercentToDraw && totalValueShown > 0.0F)
			{
				values.Append(",").Append(remainder.ToString("0.00"));
				labels.Append("|");
				if (remainder >= _minimumPercentToLabel || Legend != LegendPosition.None)
				{
                    labels.Append(GetMessage("generic other"));
				}
				if (!String.IsNullOrEmpty(remainderColor))
				{
					colors.Append(",").Append(remainderColor);
				}
			}

			if (values.Length > 0)
			{
				sbUrl.Append("&chd=t:");
				sbUrl.Append(values.ToString());
			}
			if (labels.Length > 0)
			{
				if (Legend != LegendPosition.None)
				{
					switch (Legend)
					{
						case LegendPosition.BottomHorizontal:
							sbUrl.Append("&chdlp=b");
							break;
						case LegendPosition.BottomVertical:
							sbUrl.Append("&chdlp=bv");
							break;
						case LegendPosition.Left:
							sbUrl.Append("&chdlp=l");
							break;
						case LegendPosition.Right:
							// default
							//sbUrl.Append("&chdlp=r");
							break;
						case LegendPosition.TopHorizontal:
							sbUrl.Append("&chdlp=t");
							break;
						case LegendPosition.TopVertical:
							sbUrl.Append("&chdlp=tv");
							break;
						default:
							throw new ArgumentOutOfRangeException("Legend", "Unknown Legend: " + Legend.ToString());
					}
					sbUrl.Append("&chdl=");
				}
				else
				{
					sbUrl.Append("&chl=");
				}
				sbUrl.Append(Ektron.Cms.Common.EkFunctions.UrlEncode(labels.ToString()));
			}
			if (colors.Length > 0)
			{
				sbUrl.Append("&chco=");
				sbUrl.Append(colors.ToString());
			}

			imgChart.ImageUrl = sbUrl.ToString();
		}
		else
		{
			imgChart.Visible = false;
		}

		base.Render(writer);
	}

	public enum DisplayType
	{
		Display2D,
		Display3D
	}
	private DisplayType _display = DisplayType.Display3D;
	public DisplayType Display
	{
		get { return _display; }
		set { _display = value; }
	}

	private string _briefDescription;
	public string BriefDescription
	{
		get { return _briefDescription; }
		set { _briefDescription = value; }
	}
	private Unit _width;
	public Unit Width
	{
		get { return _width; }
		set { _width = value; }
	}
	private Unit _height;
	public Unit Height
	{
		get { return _height; }
		set { _height = value; }
	}

	public string CssClass
	{
		get { return imgChart.CssClass; }
		set { imgChart.CssClass = value; }
	}

	public enum LegendPosition
	{
		None,
		Right,
		Left,
		TopHorizontal,
		TopVertical,
		BottomHorizontal,
		BottomVertical
	}
	private LegendPosition _legend = LegendPosition.None;
	public LegendPosition Legend
	{
		get { return _legend; }
		set { _legend = value; }
	}

	/// <summary>
	/// Percentage value (0.0-1.0) above which the wedge should contain a label. This value prevents crowding of labels.
	/// </summary>
	public float MinimumPercentToLabel
	{
		get { return _minimumPercentToLabel; }
		set { _minimumPercentToLabel = value; }
	}

	/// <summary>
	/// Percentage value (0.0-1.0) above which the wedge should be drawn. This values prevents very tiny wedges.
	/// </summary>
	public float MinimumPercentToDraw
	{
		get { return _minimumPercentToDraw; }
		set { _minimumPercentToDraw = value; }
	}

	/// <summary>
	/// Values must be between 0.0 and 1.0, inclusively
	/// </summary>
	/// <param name="data"></param>
	public void LoadData(List<float> data)
	{
		_percentList = data;
	}

	/// <summary>
	/// Colors are 6-character hexadecimal representation of RRGGBB, for example, "FF0000" is red.
	/// </summary>
	/// <param name="colors"></param>
	public void LoadColors(List<string> colors)
	{
		_colorList = colors;
	}

    public void LoadNames(List<string> names)
    {
        _nameList = names;
    }


}

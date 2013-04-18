using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.ComponentModel;

public partial class controls_reports_CountryMap : WorkareaBaseControl
{
	public const int MaximumWidth = 440;
	public const int MaximumHeight = 220;

	public enum GeographicalArea
	{
		[Description("africa")]
		Africa = 2,
		[Description("asia")]
		Asia = 142,
		[Description("europe")]
		Europe = 150,
		[Description("middle_east")]
		MiddleEast = 145,
		[Description("south_america")]
		SouthAmerica = 5,
		[Description("world")]
		World = 1
	}

	public controls_reports_CountryMap()
	{
		MapArea = GeographicalArea.World;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		const int DefaultWidth = 440;

		if (Width.Type != UnitType.Pixel || 0.0 == Width.Value)
		{
			Width = new Unit(DefaultWidth, UnitType.Pixel);
		}
		if (Width.Value > MaximumWidth)
		{
			Width = new Unit(MaximumWidth, UnitType.Pixel);
		}
		imgChart.Attributes.Add("width", Width.Value.ToString("0"));
		imgChart.Attributes.Add("height", Height.Value.ToString("0"));
		imgChart.AlternateText = BriefDescription;
		imgChart.Attributes.Add("title", imgChart.AlternateText);

		// INFORMATION ABOUT GOOGLE CHART API:
		
		//Example,
		//http://chart.apis.google.com/chart?cht=t&chs=440x220&chtm=world&chld=AUNZNF&chco=f0f0f0,E08000,E08000&chd=t:1,1,1&chf=bg,s,EAF7FE

		//http://code.google.com/apis/chart/docs/chart_playground.html
		//cht=t
		//chs=440x220
		//chtm=world
		//chld=AUNZNF
		//chco=333366,F69D0B,F69D0B
		//chd=t:1,1,1
		//chf=bg,s,EAF7FE

		if (null == CountryCodes)
		{
			CountryCodes = new string[0];
		}

		string[] data = new string[CountryCodes.Length];
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = "1";
			if (!System.Text.RegularExpressions.Regex.IsMatch(CountryCodes[i], "[a-z]{2}", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
			{
				throw new ArgumentException("Country code must be two letters.", CountryCodes[i]);
			}
		}

		StringBuilder sbUrl = new StringBuilder(this.GoogleChartBaseUrl);
		sbUrl.Append("?cht=t");
		sbUrl.AppendFormat("&chs={0}x{1}", Width.Value, Height.Value);
        sbUrl.AppendFormat("&chtm={0}", EnumExtensions.ToDescriptionString(MapArea));
		if (CountryCodes.Length > 0)
		{
			sbUrl.AppendFormat("&chld={0}", String.Join("", CountryCodes));
			sbUrl.AppendFormat("&chd=t:{0}", String.Join(",", data));
		}
		sbUrl.Append("&chco=333366,F69D0B,F69D0B&chf=bg,s,EAF7FE");

		imgChart.ImageUrl = sbUrl.ToString();

		base.Render(writer);
	}

	[DefaultValue(GeographicalArea.World)]
	public GeographicalArea MapArea { get; set; }

	[TypeConverter(typeof(StringArrayConverter))]
	public string[] CountryCodes { get; set; }

	public string BriefDescription { get; set; }

	private Unit _width;
	public Unit Width
	{
		get { return _width; }
		set { _width = new Unit(value.Value - (value.Value % 2), value.Type); }
	}
	public Unit Height
	{
		get { return new Unit(Width.Value / 2.0, Width.Type); }
	}

	public string CssClass
	{
		get { return imgChart.CssClass; }
		set { imgChart.CssClass = value; }
	}

}

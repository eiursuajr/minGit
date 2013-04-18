using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

public partial class Workarea_diagnostics_cultures : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{
	List<CultureData> _data = null;
	int _nativeNameCellIndex = 0;
	//int _resourceTextCellIndex = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.GetCommonApi().IsAdmin())
        {
            throw new Exception(this.GetMessage("msg login cms administrator"));
        }

		CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
		_data = new List<CultureData>(cultures.Length);

		foreach (CultureInfo c in cultures)
		{
			if (CultureInfo.InvariantCulture.LCID == c.LCID) continue;

			CultureData item = new CultureData();
			_data.Add(item);
			item.IsNeutralCulture = c.IsNeutralCulture;
			item.LCID = c.LCID;
			item.EnglishName = c.EnglishName;
			item.NativeName = c.NativeName;
			item.IsRightToLeft = c.TextInfo.IsRightToLeft;
			// These IsRightToLeft errors have been fixed in .NET 4.0
			// http://msdn.microsoft.com/en-us/netframework/dd890508.aspx
			if ("ps" == c.TwoLetterISOLanguageName || "prs" == c.TwoLetterISOLanguageName || "ug" == c.TwoLetterISOLanguageName)
			{
				item.IsRightToLeft = true;
			}

			if (c.IsNeutralCulture)
			{
				item.LanguageTag = c.IetfLanguageTag;
				string tag = c.IetfLanguageTag;
				if ("zh-Hant" == tag)
				{
					tag = "zh-TW";
				}
				else if ("zh-Hans" == tag)
				{
					tag = "zh-CN";
				}
				CultureInfo ci = CultureInfo.CreateSpecificCulture(tag);
				item.CultureTag = ci.IetfLanguageTag;
			}
			else
			{
				item.CultureTag = c.IetfLanguageTag;
				CultureInfo ci = c.Parent;
				if (CultureInfo.InvariantCulture.LCID == ci.LCID)
				{
					item.LanguageTag = c.IetfLanguageTag;
				}
				else
				{
					// ci.IetfLanguageTag may be different than c.TwoLetterISOLanguageName
					item.LanguageTag = ci.IetfLanguageTag;
				}
			}
			Page.Culture = item.CultureTag; // but not item.LanguageTag
			Page.UICulture = item.LanguageTag; // or item.CultureTag
			//item.ResourceText = GetGlobalResourceObject("DataListSpec", "dlgName") as string;
		}

		// CultureAndRegionInfoBuilder is in C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\sysglobl.dll
		//CultureAndRegionInfoBuilder cib = null;
		//cib = new CultureAndRegionInfoBuilder("zh-US", CultureAndRegionModifiers.None);
		//CultureInfo ci = new CultureInfo("zh-CN");
		//cib.LoadDataFromCultureInfo(ci);
		//RegionInfo ri = new RegionInfo("US");
		//cib.LoadDataFromRegionInfo(ri);

		//This member cannot be used by partially trusted code.
		//cib.Register();
		//Page.Culture = cib.IetfLanguageTag;

		//CultureData d = new CultureData();
		//gvCultures.Add(d);
		//d.IsNeutralCulture = false;
		//d.LCID = cib.LCID;
		//d.IetfLanguageTag = cib.IetfLanguageTag;
		//d.EnglishName = cib.CultureEnglishName + "(" + cib.RegionEnglishName + ")";
		//d.NativeName = cib.CultureNativeName + "(" + cib.RegionNativeName + ")";
		//d.IsRightToLeft = cib.TextInfo.IsRightToLeft;

		if (!Page.IsPostBack)
		{
			_data.Sort(new Comparison<CultureData>(delegate(CultureData data1, CultureData data2)
			{
				return data1.EnglishName.CompareTo(data2.EnglishName);
			}));
		}

		for (int i = 0; i < gvCultures.Columns.Count; i++)
		{
			BoundField field = gvCultures.Columns[i] as BoundField;
			if (field != null)
			{
				if ("NativeName" == field.DataField)
				{
					_nativeNameCellIndex = i;
				}
				//else if ("ResourceText" == field.DataField)
				//{
				//    _resourceTextCellIndex = i;
				//}
			}
		}

		gvCultures.RowDataBound += new GridViewRowEventHandler(gv_RowDataBound);
		gvCultures.Sorting += new GridViewSortEventHandler(gv_Sorting);

		gvCultures.DataSource = _data;
		gvCultures.DataBind();
    }

	protected override void InitializeCulture()
	{
		base.InitializeCulture();
	}

	void gv_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		GridView gv = sender as GridView;
		GridViewRow row = e.Row;
		if (DataControlRowType.DataRow == row.RowType)
		{
			CultureData data = row.DataItem as CultureData;
			if (data.IsNeutralCulture)
			{
				row.Font.Bold = true;
			}
			if (data.IsRightToLeft)
			{
				row.Cells[_nativeNameCellIndex].Attributes.Add("dir", "rtl");
				//row.Cells[_resourceTextCellIndex].Attributes.Add("dir", "rtl");
			}
		}
	}

	void gv_Sorting(object sender, GridViewSortEventArgs e)
	{
		GridView gv = sender as GridView;
		int dir = (e.SortDirection == SortDirection.Ascending ? 1 : -1);
		_data.Sort(new Comparison<CultureData>(delegate (CultureData data1, CultureData data2)
		{
			switch (e.SortExpression)
			{
				case "EnglishName": return data1.EnglishName.CompareTo(data2.EnglishName) * dir;
				case "NativeName": return data1.NativeName.CompareTo(data2.NativeName) * dir;
				case "LanguageTag": return data1.LanguageTag.CompareTo(data2.LanguageTag) * dir;
				case "CultureTag": return data1.CultureTag.CompareTo(data2.CultureTag) * dir;
				case "LCID": return data1.LCID.CompareTo(data2.LCID) * dir;
			}
			return 0;
		}));
		gv.DataBind();
	}
}

[System.ComponentModel.DataObject(true)]
public class CultureData 
{
	[System.ComponentModel.DataObjectField(false)]
	public string EnglishName { get; set; }

	[System.ComponentModel.DataObjectField(false)]
	public string NativeName { get; set; }

	[System.ComponentModel.DataObjectField(false)]
	public bool IsRightToLeft { get; set; }

	[System.ComponentModel.DataObjectField(false)]
	public string LanguageTag { get; set; }

	[System.ComponentModel.DataObjectField(false)]
	public string CultureTag { get; set; }

	[System.ComponentModel.DataObjectField(false)]
	public int LCID { get; set; }

	[System.ComponentModel.DataObjectField(false)]
	public bool IsNeutralCulture { get; set; }

	//[System.ComponentModel.DataObjectField(false)]
	//public string ResourceText { get; set; }
}

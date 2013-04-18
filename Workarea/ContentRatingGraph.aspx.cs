using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using Ektron.Cms;
using System.IO;

public partial class ContentRatingGraph : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WorkareaGraphBase graph;

        string tmp = Request.QueryString["type"];
        if (tmp == "pie")
        {
            graph = new CircleGraph();
        }
        else if (tmp == "time")
        {
            graph = new TimeGraph();
        }
        else
        {
            graph = new BarGraph();
        }

        graph.Init(this);
    }
}

internal class TimeGraph : WorkareaGraphBase
{
    public TimeGraph()
    {
        _barBrush = new SolidBrush(Color.LightBlue);
        _barColor = Color.LightBlue;
        _bgBrush = new SolidBrush(Color.White);
        _bgColor = Color.White;
        _fontBrush = new SolidBrush(Color.Black);
        _fontColor = Color.Black;

    }
    private int _height = 200;
    private int _width = 600;
    private int _bottomArea = 15;
    private float _percentageSpace = (float)(0.3F);
    private int _numBars = 24;
    private int _fontSize = 8;
    private int _clusterSize = 2;
    private SolidBrush _barBrush;
    private Color _barColor;
    private SolidBrush _bgBrush;
    private Color _bgColor;
    private SolidBrush _fontBrush;
    private Color _fontColor;
    private int[] weights;
    private float[] heights;
    private string CurrentView;
    private int Divisions = 8;
    private AnalyticsAPI Analytics = new Ektron.Cms.AnalyticsAPI();
    #region Properties
    private int FontSize
    {
        get
        {
            return _fontSize;
        }
    }

    private int Height
    {
        get
        {
            return _height;
        }
        set
        {
            if (_height > 0)
            {
                _height = value;
            }
        }
    }

    private int Width
    {
        get
        {
            return _width;
        }
        set
        {
            if (_width > 0)
            {
                _width = value;
            }
        }
    }

    private float PercentageSpace
    {
        get
        {
            return _percentageSpace;
        }
        set
        {
            if (value >= 0 || value < 1)
            {
                _percentageSpace = value;
            }
        }
    }

    private int NumBars
    {
        get
        {
            return _numBars;
        }
        set
        {
            if (value > 0)
            {
                _numBars = value;
            }
        }
    }

    private int ClusterSize
    {
        get
        {
            return _clusterSize;
        }
        set
        {
            if (value > 0)
            {
                _clusterSize = value;
            }
        }
    }

    private float BarWidth
    {
        get
        {
            return Width * (1 - PercentageSpace) / (NumBars * ClusterSize);
        }
    }

    private float SpaceWidth
    {
        get
        {
            return Width * PercentageSpace / (NumBars * ClusterSize);
        }
    }

    private int TextHeight
    {
        get
        {
            return _bottomArea;
        }
        set
        {
            _bottomArea = value;
        }
    }

    private Color BarColor
    {
        get
        {
            return _barColor;
        }
        set
        {
            _barBrush = new SolidBrush(value);
        }
    }

    private Color BGColor
    {
        get
        {
            return _bgColor;
        }
        set
        {
            _bgBrush = new SolidBrush(value);
        }
    }

    private Color FontColor
    {
        get
        {
            return _fontColor;
        }
        set
        {
            _fontBrush = new SolidBrush(value);
        }
    }
    #endregion

    protected bool CheckAccess()
    {
        ContentAPI contentApi = new ContentAPI();
		Utilities.ValidateUserLogin();
        if (contentApi.IsLoggedIn)
        {
            if ((!(Page.Request.QueryString["res_type"] == null)) && Page.Request.QueryString["res_type"].ToLower() == "content")
            {

                long contentId = System.Convert.ToInt32(Page.Request.QueryString["res"]);
                ContentAPI.userPermissions permissions = contentApi.GetUserPermissionsForContent(contentId);

                if (Ektron.Cms.Common.EkFunctions.GetBit((long)ContentAPI.userPermissions.View, (long)permissions))
                {
                    return true;
                }
                if (Ektron.Cms.Common.EkFunctions.GetBit((long)ContentAPI.userPermissions.Edit, (long)permissions))
                {
                    return true;
                }
            }
            else
            {
                return true; //this isn't content - return true.
            }
        }

        return false;
    }

    public override void Initialize()
    {
        try
        {
            CurrentView = Page.Request.QueryString["view"];
        }
        catch (Exception)
        {
            CurrentView = "day";
        }
        Height = 200;
        Width = 525;
        PercentageSpace = (float)(0.2F);
        NumBars = 24;
        ClusterSize = 2;
        weights = new int[NumBars * ClusterSize + 1];
        heights = new Single[NumBars * ClusterSize + 1];
        BarColor = Color.Blue;

        if ((string)(CurrentView) == "day")
        {
            NumBars = 24;
            Divisions = 8;
        }
        else if ((string)(CurrentView) == "week")
        {
            NumBars = 7;
            Divisions = 7;
        }
        else if ((string)(CurrentView) == "month")
        {
            NumBars = 30;
            Divisions = 6;
        }
        else if ((string)(CurrentView) == "year")
        {
            NumBars = 12;
            Divisions = 12;
        }

        if (!(Page.Request.QueryString["barColor"] == null))
        {
            try
            {
                BarColor = Color.FromName(Page.Request.QueryString["barColor"]);
            }
            catch
            {

            }
        }

        if (!(Page.Request.QueryString["fontColor"] == null))
        {
            try
            {
                FontColor = Color.FromName(Page.Request.QueryString["fontColor"]);
            }
            catch
            {

            }
        }
    }

    private string AddRestriction(string sqlStr)
    {
        string res_type = "";
        string res = "";

        try
        {
            res_type = Page.Request.QueryString["res_type"];
        }
        catch (Exception)
        {
            res_type = "";
        }

        try
        {
            res = Ektron.Cms.Common.EkFunctions.GetDbString(Page.Request.QueryString["res"], -1, true);
        }
        catch (Exception)
        {
            res = "";
        }

        if (res_type == "content")
        {
            sqlStr += (string)(" AND content_id = " + int.Parse(res));
        }
        else if (res_type == "page")
        {
            sqlStr += " AND url = \'" + res + "\'";
        }
        else if (res_type == "referring")
        {
            sqlStr += " AND referring_url = \'" + res + "\'";
        }

        return sqlStr;
    }

    public override void Drawgraphic()
    {
        int tmpooo = 1000;
        int side = System.Convert.ToInt32(10 * Convert.ToInt32(tmpooo.ToString().Length - 1) + 5);
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width + side, Height + TextHeight);
        MemoryStream ms = new MemoryStream();
        int i;
        Graphics g = Graphics.FromImage(bmp);
        System.Drawing.Font myfont = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, Convert.ToSingle(12), FontStyle.Regular, GraphicsUnit.Pixel);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

        g.FillRectangle(Brushes.White, 0, 0, Width + side, Height + TextHeight);


        if (!CheckAccess())
        {
            ContentAPI contentApi = new ContentAPI();
            Ektron.Cms.Common.EkMessageHelper msgApi = contentApi.EkMsgRef;
            throw (new Exception(msgApi.GetMessage("com: user does not have permission")));
        }

        DateTime StartDate;
        DateTime EndDate;
        try
        {
            EndDate = DateTime.Parse(Page.Request.QueryString["EndDate"]);
        }
        catch (Exception)
        {
            EndDate = DateTime.Today;
        }

        try
        {
            StartDate = DateTime.Parse(Page.Request.QueryString["StartDate"]);
        }
        catch (Exception)
        {
            StartDate = DateTime.MinValue;
        }

        string[] sqlCommands = new string[NumBars + 1];
        DataSet dbData;

        int[] data = new int[NumBars * ClusterSize + 1];

        if (StartDate == DateTime.MinValue)
        {

            if (CurrentView == "day")
            {
                for (i = 0; i <= NumBars - 1; i++)
                {
                    if (i == NumBars - 1)
                    {
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddHours(i)), AnalyticsAPI.ProviderInvariantName) + " AND hit_date <= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddHours(i + 1)), AnalyticsAPI.ProviderInvariantName) + "");
                    }
                    else
                    {
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddHours(i)), AnalyticsAPI.ProviderInvariantName) + " AND hit_date < " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddHours(i + 1)), AnalyticsAPI.ProviderInvariantName) + "");
                    }

                }
                dbData = Analytics.QueryAnalytics(sqlCommands);
                i = 0;
                for (i = 0; i <= dbData.Tables.Count - 1; i++)
                {
                    data[i * 2] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][0]);
                    data[i * 2 + 1] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][1]);
                }
            }
            else if (CurrentView == "week")
            {
                for (i = 0; i <= NumBars - 1; i++)
                {
                    if (i == NumBars - 1)
                    {
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date <= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 2)))), AnalyticsAPI.ProviderInvariantName) + " AND hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 1)))), AnalyticsAPI.ProviderInvariantName) + "");
                    }
                    else
                    {
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date < " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 2)))), AnalyticsAPI.ProviderInvariantName) + " AND hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 1)))), AnalyticsAPI.ProviderInvariantName) + "");
                    }

                }
                dbData = Analytics.QueryAnalytics(sqlCommands);
                i = 0;
                for (i = 0; i <= dbData.Tables.Count - 1; i++)
                {
                    data[i * 2] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][0]);
                    data[i * 2 + 1] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][1]);
                }
            }
            else if (CurrentView == "month")
            {
                for (i = 0; i <= NumBars - 1; i++)
                {
                    if (i == NumBars - 1)
                    {
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date <= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 2)))), AnalyticsAPI.ProviderInvariantName) + " AND hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 1)))), AnalyticsAPI.ProviderInvariantName) + "");
                    }
                    else
                    {
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date < " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 2)))), AnalyticsAPI.ProviderInvariantName) + " AND hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddDays(System.Convert.ToDouble(-1 * (NumBars - i - 1)))), AnalyticsAPI.ProviderInvariantName) + "");
                    }

                }
                dbData = Analytics.QueryAnalytics(sqlCommands);
                i = 0;
                for (i = 0; i <= dbData.Tables.Count - 1; i++)
                {
                    data[i * 2] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][0]);
                    data[i * 2 + 1] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][1]);
                }
            }
            else if (CurrentView == "year")
            {
                for (i = 0; i <= NumBars - 1; i++)
                {
                    if (i == NumBars - 1)
                    {
                        // MM-01-yyyy
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date <= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddMonths(System.Convert.ToInt32(-1 * (NumBars - i - 2)))), AnalyticsAPI.ProviderInvariantName) + " AND hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddMonths(System.Convert.ToInt32(-1 * (NumBars - i - 1)))), AnalyticsAPI.ProviderInvariantName) + "");
                    }
                    else
                    {
                        sqlCommands[i] = AddRestriction("SELECT COUNT(visitor_id), COUNT(DISTINCT visitor_id) FROM content_hits_tbl WHERE " + "hit_date < " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddMonths(System.Convert.ToInt32(-1 * (NumBars - i - 2)))), AnalyticsAPI.ProviderInvariantName) + " AND hit_date >= " + AnalyticsAPI.FormatDate(Convert.ToString(EndDate.AddMonths(System.Convert.ToInt32(-1 * (NumBars - i - 1)))), AnalyticsAPI.ProviderInvariantName) + "");
                    }

                }
                dbData = Analytics.QueryAnalytics(sqlCommands);
                i = 0;
                for (i = 0; i <= dbData.Tables.Count - 1; i++)
                {
                    data[i * 2] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][0]);
                    data[i * 2 + 1] = System.Convert.ToInt32(dbData.Tables[i].Rows[0][1]);
                }
            }
        }
        else
        {

        }

        int tmpS;
        if (CurrentView == "day")
        {
            for (i = 0; i <= 7; i++)
            {
                tmpS = i * (Width / Divisions) + side;
                g.DrawString((string)(EndDate.AddHours(System.Convert.ToDouble(3 * i)).ToString("hh:mm tt")), myfont, Brushes.Black, new System.Drawing.Point(tmpS, Height));
            }
        }
        else if (CurrentView == "week")
        {
            for (i = 0; i <= 6; i++)
            {
                tmpS = System.Convert.ToInt32((Divisions - i - 1) * (Width / Divisions) + side);
                g.DrawString((string)(EndDate.AddDays(System.Convert.ToDouble(-1 * i)).ToString("ddd MM-dd")), myfont, Brushes.Black, new System.Drawing.Point(tmpS, Height));
            }
        }
        else if (CurrentView == "month")
        {
            for (i = 0; i <= 5; i++)
            {
                tmpS = System.Convert.ToInt32((Divisions - i - 1) * (Width / Divisions) + side);
                g.DrawString((string)(EndDate.AddDays(System.Convert.ToDouble(-5 * i)).ToString("MM-dd-yyyy")), myfont, Brushes.Black, new System.Drawing.Point(tmpS, Height));
            }
        }
        else if (CurrentView == "year")
        {
            for (i = 0; i <= 11; i++)
            {
                tmpS = System.Convert.ToInt32((Divisions - i - 1) * (Width / Divisions) + side);
                g.DrawString((string)(EndDate.AddMonths(System.Convert.ToInt32(-1 * i)).ToString("MMM-yy")), myfont, Brushes.Black, new System.Drawing.Point(tmpS, Height));
            }
        }


        for (i = 0; i <= Divisions - 1; i++)
        {
            int start = i * (Width / Divisions) + side;
            g.DrawLine(Pens.Black, start, Height, start, 0);
        }


        Random rand = new Random();

        int max = 1;
        for (i = 0; i <= NumBars * ClusterSize - 1; i++)
        {
            if (data[i] > max)
            {
                max = data[i];
            }
        }

        int oom = this.GetOrderOfMagnitude(max);
        //g.DrawString(max, font, Brushes.Black, 0, 12)
        int trying = 0;
        trying = System.Convert.ToInt32((oom / max) * (Height));


        int j = 1;
        while ((j * oom) < max)
        {
            float tmp1 = Height - trying * j;
            g.DrawLine(Pens.Black, side, System.Convert.ToInt32(tmp1), Width + side, System.Convert.ToInt32(tmp1));
            g.DrawString((string)((oom * j).ToString()), myfont, Brushes.Black, 0, tmp1 - 7);
            j++;
        }

        g.DrawLine(Pens.Black, side, Height, Width + side, Height);

        for (i = 0; i <= NumBars * ClusterSize - 1; i++)
        {
            data[i] = System.Convert.ToInt32(data[i] * Height / max);
            if (i % 2 == 0)
            {
                g.FillRectangle(Brushes.Red, System.Convert.ToInt32((BarWidth + SpaceWidth) * i + side), Height - data[i], System.Convert.ToInt32(BarWidth), data[i]);
            }
            else
            {
                g.FillRectangle(Brushes.Blue, System.Convert.ToInt32((BarWidth + SpaceWidth) * i + side), Height - data[i], System.Convert.ToInt32(BarWidth), data[i]);
            }
        }

        bmp.Save(ms, ImageFormat.Png);
        ms.WriteTo(Page.Response.OutputStream);

        bmp.Dispose();
        ms.Dispose();
    }

    private int GetOrderOfMagnitude(int val)
    {
        int i = 1;

        while (true)
        {
            if (Math.Floor(Convert.ToDecimal(val / (i * 10))) == 0)
            {
                return i;
            }
            else
            {
                i = i * 10;
            }
        }
    }

}

internal class CircleGraph : WorkareaGraphBase
{


    private int _height = 100;
    private int _width = 100;

    protected int Height
    {
        get
        {
            return _height;
        }
        set
        {
            _height = value;
        }
    }

    protected int Width
    {
        get
        {
            return _width;
        }
        set
        {
            _width = value;
        }
    }


    public override void Initialize()
    {
        try
        {
            int tmp = Convert.ToInt32(Page.Request.QueryString["size"]);
            Height = tmp;
            Width = tmp;
        }
        catch (Exception)
        {
            Height = 100;
            Width = 100;
        }

    }

    public override void Drawgraphic()
    {
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height);
        MemoryStream ms = new MemoryStream();
        Graphics objGraphics = Graphics.FromImage(bmp);

        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black);

        int r1 = Convert.ToInt32(Page.Request.QueryString["r1"]);
        int r2 = Convert.ToInt32(Page.Request.QueryString["r2"]);

        int total = r1 + r2;

        int tHeight = System.Convert.ToInt32(Math.Floor(Height * 0.9));
        int tWidth = System.Convert.ToInt32(Math.Floor(Width * 0.9));
        int delta = System.Convert.ToInt32(Math.Floor(Width * 0.05));

        int d1;
        try
        {
            d1 = r1 * 360 / total;
        }
        catch (Exception)
        {
            objGraphics.FillRectangle(whiteBrush, 0, 0, Width, Height);
            objGraphics.FillPie(Brushes.Black, delta, delta, tWidth, tHeight, 0, 360);
            bmp.Save(ms, ImageFormat.Png);
            ms.WriteTo(Page.Response.OutputStream);
            return;
        }

        int d2 = System.Convert.ToInt32(360 - d1);



        objGraphics.FillRectangle(whiteBrush, 0, 0, Width, Height);
        objGraphics.FillPie(Brushes.Red, delta, delta, tWidth, tHeight, 0, d1);
        objGraphics.FillPie(Brushes.Blue, delta, delta, tWidth, tHeight, d1, d2);

        bmp.Save(ms, ImageFormat.Png);
        ms.WriteTo(Page.Response.OutputStream);

        bmp.Dispose();
        ms.Dispose();
    }
}

internal class BarGraph : WorkareaGraphBase
{
    public BarGraph()
    {
        _barBrush = new SolidBrush(Color.LightBlue);
        _barColor = Color.LightBlue;
        _bgBrush = new SolidBrush(Color.White);
        _bgColor = Color.White;
        _fontBrush = new SolidBrush(Color.Black);
        _fontColor = Color.Black;

    }

    private int _height = 100;
    private int _width = 100;
    private int _bottomArea = 15;
    private float _percentageSpace = (float)(0.3F);
    private int _numBars = 9;
    private int _fontSize = 8;
    private SolidBrush _barBrush;
    private Color _barColor;
    private SolidBrush _bgBrush;
    private Color _bgColor;
    private SolidBrush _fontBrush;
    private Color _fontColor;
    private int[] weights;
    private float[] heights;
    private bool m_b0 = true;
    private bool m_bStars = false;

    public override void Initialize()
    {
        Height = 125;
        Width = 150;
        PercentageSpace = (float)(0.3F);
        NumBars = 11;
        weights = new int[NumBars + 1];
        heights = new Single[NumBars + 1];
        int max = 1;
        BarColor = Color.LightBlue;

        int i;
        for (i = 0; i <= 10; i++)
        {
            try
            {
                string val = Page.Request.QueryString.Get((string)("R" + (i)));
                if (i == 0 && val == null)
                {
                    m_b0 = false;
                }
                else if (!(i == 10 && val == null))
                {
                    weights[i] = Convert.ToInt32(val);
                }
            }
            catch
            {
                weights[i] = 0;
            }
            if (weights[i] > max)
            {
                max = weights[i];
            }
        }

        for (i = 0; i <= 10; i++)
        {
            heights[i] = Height * System.Convert.ToSingle(weights[i] / max);
        }

        FontColor = GetColor("fontColor", FontColor);
        BarColor = GetColor("barColor", BarColor);
        BGColor = GetColor("bgColor", BGColor);
        if (Page.Request.QueryString["fontColor"] == "0")
        {
            FontColor = Color.Black;
        }
        if (Page.Request.QueryString["bgColor"] == "0")
        {
            BGColor = Color.White;
        }
        if (Page.Request.QueryString["stars"] != "")
        {
            m_bStars = true;
            Width = 250;
            Height = 150;
            _bottomArea = 90;
        }
    }

    private Color GetColor(string target, Color col)
    {
        if (!(Page.Request.QueryString[target] == null))
        {
            try
            {
                col = Color.FromArgb(Convert.ToInt32(Page.Request.QueryString[target]));
            }
            catch
            {

            }
        }
        return col;
    }


    private int FontSize
    {
        get
        {
            return _fontSize;
        }
    }

    private int Height
    {
        get
        {
            return _height;
        }
        set
        {
            if (_height > 0)
            {
                _height = value;
            }
        }
    }

    private int Width
    {
        get
        {
            return _width;
        }
        set
        {
            if (_width > 0)
            {
                _width = value;
            }
        }
    }

    private float PercentageSpace
    {
        get
        {
            return _percentageSpace;
        }
        set
        {
            if (value >= 0 || value < 1)
            {
                _percentageSpace = value;
            }
        }
    }

    private int NumBars
    {
        get
        {
            return _numBars;
        }
        set
        {
            if (value > 0)
            {
                _numBars = value;
            }
        }
    }

    private float BarWidth
    {
        get
        {
            return Width * (1 - PercentageSpace) / NumBars;
        }
    }

    private float SpaceWidth
    {
        get
        {
            return Width * PercentageSpace / NumBars;
        }
    }

    private int TextHeight
    {
        get
        {
            return _bottomArea;
        }
        set
        {
            _bottomArea = value;
        }
    }

    private Color BarColor
    {
        get
        {
            return _barColor;
        }
        set
        {
            _barBrush = new SolidBrush(value);
        }
    }

    private Color BGColor
    {
        get
        {
            return _bgColor;
        }
        set
        {
            _bgBrush = new SolidBrush(value);
        }
    }

    private Color FontColor
    {
        get
        {
            return _fontColor;
        }
        set
        {
            _fontBrush = new SolidBrush(value);
        }
    }

    public override void Drawgraphic()
    {
        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height + TextHeight);
        ContentAPI cAPI = null;
        System.Drawing.Image iStar = null;
        System.Drawing.Image iStarH = null;
        System.Drawing.Image iStop = null;
        MemoryStream ms = new MemoryStream();

        if (m_bStars == true)
        {
            cAPI = new ContentAPI();
            iStar = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(cAPI.AppPath + "images/UI/icons/star.png"));
            iStarH = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(cAPI.AppPath + "images/UI/icons/starHalf.png"));
            iStop = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(cAPI.AppPath + "images/UI/icons/stop.png"));
        }

        Graphics objGraphics = Graphics.FromImage(bmp);
        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

        Brush whiteBrush = _bgBrush;
        Brush blackBrush = _fontBrush;

        objGraphics.FillRectangle(_bgBrush, 0, 0, Width, Height + TextHeight);
        int i;
        if (m_b0 == false && m_b0 == false)
        {
            NumBars = 9;
            for (i = 0; i <= NumBars - 1; i++)
            {
                objGraphics.FillRectangle(_barBrush, System.Convert.ToInt32((BarWidth + SpaceWidth) * i), Height - heights[i + 1], System.Convert.ToInt32(BarWidth), System.Convert.ToInt32(heights[i + 1]));
                objGraphics.DrawString((string)((i + 1).ToString()), new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, Convert.ToSingle(8)), blackBrush, (BarWidth + SpaceWidth) * (i), Height);
            }
        }
        else
        {
            for (i = 0; i <= NumBars - 1; i++)
            {
                objGraphics.FillRectangle(_barBrush, System.Convert.ToInt32((BarWidth + SpaceWidth) * i), Height - heights[i], System.Convert.ToInt32(BarWidth), System.Convert.ToInt32(heights[i]));
                if (m_bStars == true)
                {
                    if (i % 2 == 0 && i > 0)
                    {
                        for (int j = 2; j <= i; j += 2)
                        {
                            objGraphics.DrawImage(iStar, System.Convert.ToInt32((BarWidth + SpaceWidth) * (i)), Height + ((j - 2) * 9));
                        }
                    }
                    else if (i > 1)
                    {
                        decimal dHeight = 0;
                        for (int j = 3; j <= i; j += 2)
                        {
                            dHeight = System.Convert.ToDecimal(Height + ((j - 2) * 9) - 9);
                            objGraphics.DrawImage(iStar, System.Convert.ToInt32((BarWidth + SpaceWidth) * (i)), System.Convert.ToInt32(dHeight));
                        }
                        if (i == 1)
                        {
                            dHeight = Convert.ToDecimal(Height) - Convert.ToDecimal(heights[i]) - 18;
                        }
                        objGraphics.DrawImage(iStarH, System.Convert.ToInt32((BarWidth + SpaceWidth) * (i)), System.Convert.ToInt32(dHeight + 18));
                    }
                    else if (i == 1)
                    {
                        objGraphics.DrawImage(iStarH, System.Convert.ToSingle((BarWidth + SpaceWidth) * (i)), System.Convert.ToSingle(Height + (-1 * 9) + 9));
                    }
                    else if (i == 0)
                    {
                        objGraphics.DrawImage(iStop, System.Convert.ToSingle((BarWidth + SpaceWidth) * (i)), System.Convert.ToSingle(Height + (-1 * 9) + 9));
                    }
                }
                else
                {
                    objGraphics.DrawString((string)((i).ToString()), new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, Convert.ToSingle(8)), blackBrush, (BarWidth + SpaceWidth) * i, Height);
                }
            }
        }

        bmp.Save(ms, ImageFormat.Png);
        ms.WriteTo(Page.Response.OutputStream);

        bmp.Dispose();
        ms.Dispose();
    }
}

internal abstract class WorkareaGraphBase
{

    protected System.Web.UI.Page Page;

    public void Init(System.Web.UI.Page page)
    {
        this.Page = page;
        this.Page.Response.ContentType = "image/png";
        Initialize();
        Drawgraphic();
    }

    abstract public void Initialize();

    abstract public void Drawgraphic();
}

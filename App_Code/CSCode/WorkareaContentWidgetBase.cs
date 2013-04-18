using System;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Widget;

/// <summary>
/// Summary description for WorkareaWidgetBaseControl
/// </summary>
public class WorkareaWidgetBaseControl : UserControl {
    public EkMessageHelper _messageHelperRef;
    CommonApi _commonAPIRef = new CommonApi();
    EkContent _contentRef;
    ContentAPI _contentApi;
    IWidgetHost _host;
    string _imagePath;
    int _pageSize = -1;

    protected void Page_Init(object sender, EventArgs e) {
        Host.Minimize += new MinimizeDelegate(MinimizeEvent);
        Host.Maximize += new MaximizeDelegate(MaximizeEvent);
    }

    public EkMessageHelper MessageHelper { get { return (_messageHelperRef ?? (_messageHelperRef = _commonAPIRef.EkMsgRef)); } }

    public IWidgetHost Host { get { return (_host ?? (_host = WidgetHost.GetHost(this))); } }

    public EkContent EkContentRef { get { return _contentRef ?? (_contentRef = _commonAPIRef.EkContentRef); } }

    public bool IsLoggedIn { get { return ContentApi.IsLoggedIn; } }

    public bool IsAdmin { get { return IsLoggedIn && ContentApi.IsAdmin(); } }

    public bool IsCommerceAdmin { get { return IsLoggedIn && (IsAdmin || ContentApi.IsARoleMember(EkEnumeration.CmsRoleIds.CommerceAdmin)); } }

    public void SetTitle(string title) {
        Host.Title = title;
    }

    public string GetMessage(string key) {
        return MessageHelper.GetMessage(key);
    }

    public string GetCountText(int dataCount) {
        if (dataCount > PageSize) {
            return "1 to " + PageSize.ToString() + " of " + dataCount.ToString();
        }

        return dataCount.ToString();
    }

    public string GetContentImage(int contentType, string icon) {
        return string.Format("<img alt=\"\" valign=\"absbottom\" src=\"{0}\"></img>", GetContentImagePath(contentType, icon));
    }

    public string GetContentImagePath(int contentType, string icon) {
        if (contentType == 1 || contentType == 3) {
            return ImagePath + "../UI/Icons/contentHTML.png";
        } else if (contentType == 2) {
            return ImagePath + "../UI/Icons/contentForm.png";
        } else if (contentType == 1111) {
            return ImagePath + "../UI/Icons/asteriskOrange.png";
        } else if (contentType == 3333) {
            return ImagePath + "../UI/Icons/brick.png";
        } else {
            return icon;
        }
    }

    void MinimizeEvent() {
        this.Visible = false;
    }

    void MaximizeEvent() {
        Visible = true;
    }

    public CommonApi CommonAPIRef {
        get { return _commonAPIRef; }
    }

    public ContentAPI ContentApi { get { return _contentApi ?? (_contentApi = new ContentAPI()); } }

    /// <summary>
    /// Reference to locale manager object
    /// </summary>
    private Ektron.Cms.Framework.Localization.LocaleManager localeMgr = null;

    /// <summary>
    /// Locale manager object
    /// </summary>
    /// <returns>Reference to locale manager object</returns>
    protected Ektron.Cms.Framework.Localization.LocaleManager LocaleManager
    {
        get
        {
            if (null == this.localeMgr)
            {
                this.localeMgr = new Ektron.Cms.Framework.Localization.LocaleManager();
            }

            return this.localeMgr;
        }
    }

    public string ImagePath { get { return string.IsNullOrEmpty(_imagePath) ? (_imagePath = _commonAPIRef.AppImgPath) : _imagePath; } }

    public int PageSize { get { return (_pageSize < 0) ? (_pageSize = _commonAPIRef.RequestInformationRef.PagingSize) : _pageSize; } }

	protected virtual string GoogleChartBaseUrl
	{
		get
		{
			if (_commonAPIRef.UseSsl)
			{
				return _commonAPIRef.AppPath + "controls/reports/GoogleChart.ashx";
			}
			else
			{
				return "http://chart.apis.google.com/chart";
			}
		}
	}
}

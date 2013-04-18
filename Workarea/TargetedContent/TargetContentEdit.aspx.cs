using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.API;
using Ektron.Cms.PageBuilder;
using System.Text;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Widgets;
using Ektron.Cms.BusinessObjects.Content.Targeting;
using Ektron.Cms.Content.Targeting;

public partial class Workarea_TargetedContent_TargetContentEdit : PageBuilder
{

    #region member variables

    private const string Page_Action = "TargetedContentEdit";

    ContentAPI _contentApi = new ContentAPI();
    StyleHelper _styleHelper = new StyleHelper();
    protected EkMessageHelper _msgHelper;
    long _targetContentId = 0;
    Guid _columnGuid = Guid.Empty;
    string _targetContentWidgetXml = @"<ArrayOfDataStore xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><dataStore><Property>SelectedZone</Property><Value xsi:type=""xsd:int"">0</Value><TypeName>System.Int32</TypeName><AssemblyAndType>System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyAndType></dataStore><dataStore><Property>TargetConfigurationId</Property><Value xsi:type=""xsd:long"">{0}</Value><TypeName>System.Int64</TypeName><AssemblyAndType>System.Int64, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyAndType></dataStore></ArrayOfDataStore>";

    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {

		var m_refMsg = new ContentAPI().EkMsgRef;

		CancelLabel.Text = m_refMsg.GetMessage("generic Cancel");
		SaveLabel.Text = m_refMsg.GetMessage("btn Save");

        //If initial load, reset pagebuilder page to clean slate.
        if (!IsPostBack && Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("targetcontentlist.aspx"))
        {
            Session["EkWidgetDirty"] = null;
            Session["EkWidgetBag"] = null;
            (Page as PageBuilder).Pagedata = null;
            (Page as PageBuilder).ClearView();
        }

        _msgHelper = new EkMessageHelper(_contentApi.RequestInformationRef);

        Utilities.ValidateUserLogin();
        if (_contentApi.RequestInformationRef.IsMembershipUser == 1 || (_contentApi.RequestInformationRef.UserId != 999999999 && !_contentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminPersonalize)))
        {
            Response.Redirect(_contentApi.ApplicationPath + "reterror.aspx?info=" + _contentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
        }

        txtName.Attributes.Add("onkeypress", "return CheckKeyValue(event, '34,13, 60, 62');");
        //Register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

		string cssFilePath =
    		ResolveUrl(String.Format("{0}/{1}", new SiteAPI().SitePath,
    	                         "Workarea/csslib/ektron.workarea.personalization.ie.7.css"));
    	
		Ektron.Cms.API.Css.RegisterCss(this, cssFilePath, "ie-targetcontent", Css.BrowserTarget.IE8);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        ltrlStyleSheetJS.Text = _styleHelper.GetClientScript();

        //ViewAllToolbar
        txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(_msgHelper.GetMessage("lbl targeted content"));
        lblName.Text = _msgHelper.GetMessage("generic title");
        ltrTitleEmpty.Text = _msgHelper.GetMessage("js: alert title required");
        image_link_100.Attributes.Add("onclick", "verifyTitle(event);");

        PageBuilder pb = this as PageBuilder;


        if (Request["targetcontentid"] != null)
        {
            long.TryParse(Request["targetcontentid"], out _targetContentId);
        }

        //Add the Target Content Widget to Page
        if (pb.Pagedata.Widgets == null || pb.Pagedata.Widgets.Count == 0)
        {
            if (_targetContentId == 0)
            {
                _columnGuid = Guid.NewGuid();
                _targetContentWidgetXml = string.Format(@"<ArrayOfDataStore xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><dataStore><Property>RulesetNames</Property><TypeName>System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]</TypeName><AssemblyAndType>System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyAndType></dataStore><dataStore><Property>Rulesets</Property><TypeName>System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]</TypeName><AssemblyAndType>System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyAndType></dataStore><dataStore><Property>SelectedZone</Property><Value xsi:type=""xsd:int"">0</Value><TypeName>System.Int32</TypeName><AssemblyAndType>System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyAndType></dataStore><dataStore><Property>TargetConfigurationId</Property><Value xsi:type=""xsd:long"">{0}</Value><TypeName>System.Int64</TypeName><AssemblyAndType>System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyAndType></dataStore><dataStore><Property>TargetedContent</Property><Value xsi:type=""TargetedContentData""><Id>0</Id><IsGlobal>false</IsGlobal><Segments><SegmentData><Id>0</Id><Name>Default</Name><Rules /><IsGlobal>false</IsGlobal></SegmentData></Segments><PageData><IsMasterLayout xmlns=""PB"">false</IsMasterLayout><masterID xmlns=""PB"">0</masterID><pageID xmlns=""PB"">0</pageID><languageID xmlns=""PB"">0</languageID><Zones xmlns=""PB""><DropZoneData><isMasterZone>false</isMasterZone><Columns><ColumnDataSerialize><Guid>{1}</Guid><Display>false</Display><columnID>-1</columnID><width>100</width><unit>percent</unit></ColumnDataSerialize></Columns></DropZoneData></Zones><Widgets xmlns=""PB"" /></PageData></Value><TypeName>Ektron.Cms.Content.Targeting.TargetedContentData</TypeName><AssemblyAndType>Ektron.Cms.Content.Targeting.TargetedContentData, Ektron.Cms.ObjectFactory, Version=8.5.0.297, Culture=neutral, PublicKeyToken=559a2c4fa21e63be</AssemblyAndType></dataStore></ArrayOfDataStore>", "{0}", _columnGuid);
            }

            pb.ClearView(layoutVersion.Staged, false);
            pb.Status = Mode.Editing;
            pb.viewType = layoutVersion.Staged;

            WidgetData targetContentWidget = new WidgetData()
            {
                ID = 999,
                ControlURL = "TargetedContent.ascx",
                ColumnID = 0,
                DropID = "targetContentZone",
                Settings = string.Format(_targetContentWidgetXml, _targetContentId)
            };

            pb.Pagedata.Widgets = new List<WidgetData>();
            pb.Pagedata.Widgets.Add(targetContentWidget);

            //(Page as PageBuilder).View(pb.Pagedata);

            LoadTargetContentConfiguration(targetContentWidget);

            pb.View(pb.Pagedata);
        }

        if (!IsPostBack)
        {
            Bind();
        }
    }

    protected void Page_Render(object sender, EventArgs e)
    {
        //these calls are required to avoid the error "Invalid postback or callback argument"
        //I think the error was occuring, if the hidden input had a modified postback value, but the widget was removed and then re-added (different control technically)
        //adding this register seems to fix the issue.
        Page.ClientScript.RegisterForEventValidation(txtName.UniqueID);
    }

    protected void ucSaveButton_click(object sender, EventArgs e)
    {
        TargetedContentWidget tcWidget = FindTargetContentWidget(targetContentZone);

        if (tcWidget != null)
        {
            Ektron.Cms.Content.Targeting.TargetedContentData tc = tcWidget.TargetedContent;
            tc.Name = txtName.Text;
            tc.IsGlobal = true;
            tcWidget.SaveConfiguration(tc);
        }


        Session["EkWidgetDirty"] = null;
        Session["EkWidgetBag"] = null;
        (Page as PageBuilder).Pagedata = null;
        (Page as PageBuilder).ClearView();

        RedirectHome();
    }

    protected void ucCancelButton_click(object sender, EventArgs e)
    {
        Session["EkWidgetDirty"] = null;
        Session["EkWidgetBag"] = null;
        (Page as PageBuilder).Pagedata = null;
        (Page as PageBuilder).ClearView();

        RedirectHome();
    }

    private void Bind()
    {
        BindToolbars();

        if (txtName.Text == "" && _targetContentId != 0)
        {
            TargetedContent tcManager = new TargetedContent(_contentApi.RequestInformationRef);
            TargetedContentData targetContentData = tcManager.GetItem(_targetContentId);

            if (targetContentData != null)
            {
                txtName.Text = targetContentData.Name;
            }
        }
    }

    private void BindToolbars()
    {
        WorkareaTitlebar.InnerHtml = _msgHelper.GetMessage("lbl edit target content");
        image_cell_100.Attributes.Add("title", _msgHelper.GetMessage("btn save"));
        image_cell_101.Attributes.Add("title", _msgHelper.GetMessage("btn cancel"));
        uxHelpbutton.Text = _styleHelper.GetHelpButton(Page_Action, "");
        image_link_100.Attributes.Add("onmouseover", "ShowTransString('" + _msgHelper.GetMessage("btn save") + "');RollOver(this);");
        image_link_101.Attributes.Add("onmouseover", "ShowTransString('" + _msgHelper.GetMessage("btn cancel") + "');RollOver(this);");
    }

    private void LoadTargetContentConfiguration(WidgetData targetContentWidget)
    {

        TargetedContent tcManager = new TargetedContent(_contentApi.RequestInformationRef);
        TargetedContentData targetContentData = 
            (_targetContentId > 0 ? tcManager.GetItem(_targetContentId) : new TargetedContentData());

        if (_targetContentId == 0)
        {
            targetContentData.PageData = PageData.Restore(string.Format("<PageData xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"PB\"><IsMasterLayout>false</IsMasterLayout><masterID>0</masterID><pageID>0</pageID><languageID>0</languageID><Zones><DropZoneData><isMasterZone>false</isMasterZone><Columns><ColumnData><Guid>{0}</Guid><Display>false</Display><columnID>-1</columnID><width>100</width><unit>percent</unit></ColumnData></Columns></DropZoneData></Zones><Widgets /></PageData>", _columnGuid));
            targetContentData.Segments.Add(new SegmentData());
        }

        if (targetContentData != null)
        {

            txtName.Text = targetContentData.Name;
            PageBuilder pb = Page as PageBuilder;

            pb.Pagedata.Widgets.AddRange(targetContentData.PageData.Widgets);
            pb.Pagedata.Zones[0].Columns.AddRange(targetContentData.PageData.Zones[0].Columns);

            targetContentWidget.ChildColumns = targetContentData.PageData.Zones[0].Columns;

            pb.ClearView(layoutVersion.Staged, false);
            pb.Status = Mode.Editing;
            pb.viewType = layoutVersion.Staged;
            pb.View(pb.Pagedata);
        }

    }


    private void ReloadPage()
    {
        string newUrl = Page.Request.RawUrl.Replace('?', '&').Replace("&ektronPageBuilderEdit=true", "");
        int first = newUrl.IndexOf('&');
        if (first > -1) newUrl = newUrl.Remove(first, 1).Insert(first, "?");
        Response.Redirect(newUrl);

    }

    private void RedirectHome()
    {
        Response.Redirect(_contentApi.RequestInformationRef.ApplicationPath + "/targetedContent/TargetContentList.aspx");
    }

    private TargetedContentWidget FindTargetContentWidget(Control parent){
        TargetedContentWidget tcWidget = parent as TargetedContentWidget;
        if(tcWidget != null){ return tcWidget;
        }

        foreach (Control c in parent.Controls)
        {
            tcWidget = c as TargetedContentWidget;
            if (tcWidget == null)
            {
                tcWidget = FindTargetContentWidget(c);
            }

            if (tcWidget != null)
            {
                break;
            }
        }

        return tcWidget;
    }


    public override void Error(string message)
    {
        throw new NotImplementedException();
    }

    public override void Notify(string message)
    {
        throw new NotImplementedException();
    }
}

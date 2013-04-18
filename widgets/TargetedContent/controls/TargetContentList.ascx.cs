using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.BusinessObjects.Content.Targeting;
using Ektron.Cms.Content.Targeting;
using System.Text;

public partial class widgets_TargetedContent_controls_TargetContentList : System.Web.UI.UserControl
{

    #region member variables

    ContentAPI _contentApi = new ContentAPI();
    StyleHelper _styleHelper = new StyleHelper();
    protected EkMessageHelper _msgHelper;

    public event EventHandler<TargetContentEventArgs> TargetContentSelected;

    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
 
        //Register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronJQueryUiDefaultCss);

        //JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);

        _msgHelper = new EkMessageHelper(_contentApi.RequestInformationRef);

        //Is the selected page posted back?
        if (Request.Form["SelectedPage"] != null) {
            int page = 0;
            if(int.TryParse(Request.Form["SelectedPage"], out page)){
                ucPaging.SelectedPage = page;
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Bind();

        if (IsPostBack)
        {
            //If the hdnTargetContentSelectId has a value, raise OnTargetedContentSelect event
            //Need to raise it in Page_Load before PageBuilder does all its work.
            if (!string.IsNullOrEmpty(hdnTargetContentSelectId.Value) && !string.IsNullOrEmpty(Request.Form["__EVENTTARGET"]) 
                && ( Request.Form["__EVENTTARGET"].EndsWith("$selectLink") || Request.Form["__EVENTTARGET"].EndsWith("$selectLinkName")) )
            {

                //make sure the "Source" targetcontent control is the parent of this control.
                //there may be many target content controls on page.
                //we track the source of the "Select Global Config" click so we can differentiate between multiple target content widgets
                if (this.Parent.ClientID.Contains(hdnTargetContentControlSource.Value)) {
                    long selectId;
                    long.TryParse(hdnTargetContentSelectId.Value, out selectId);

                    if (selectId > 0) {
                        OnSelect(selectId);
                    }
                }
            }
        }
    }

    protected override void Render(HtmlTextWriter writer)
    {

        //these calls are required to avoid the error "Invalid postback or callback argument"
        //I think the error was occuring, if the hidden input had a modified postback value, but the widget was removed and then re-added (different control technically)
        //adding this register seems to fix the issue.
        Page.ClientScript.RegisterForEventValidation(hdnTargetContentControlSource.UniqueID);
        Page.ClientScript.RegisterForEventValidation(hdnTargetContentSelectId.UniqueID);
        Page.ClientScript.RegisterForEventValidation(hdnTargetContentListUrl.UniqueID);

        base.Render(writer);
    }

    protected void selectLink_Click(object sender, CommandEventArgs e)
    {
        //NOTE: OnTargetedContentSelect is raised from Page_Init because its too late for PageBuilder at this point.
    }

    protected void OnSelect(long id)
    {
        if (TargetContentSelected != null)
        {
            TargetContentSelected(this, new TargetContentEventArgs() { TargetContentId = id });
        }
    }

    private void Bind()
    {

    
        if (!IsPostBack)
        {
            hdnTargetContentSelectId.Value = "0";
        }

        TargetedContent targetContentManager = new TargetedContent(_contentApi.RequestInformationRef);
        Criteria<TargetedContentProperty> criteria = new Criteria<TargetedContentProperty>();
        criteria.PagingInfo.CurrentPage = ucPaging.SelectedPage;
        criteria.PagingInfo.RecordsPerPage = _contentApi.RequestInformationRef.PagingSize;

        criteria.AddFilter(TargetedContentProperty.IsGlobal, CriteriaFilterOperator.EqualTo, true);

        List<TargetedContentData> targetContentList = targetContentManager.GetList(criteria);
        ViewAllRepeater.DataSource = targetContentList;
        ViewAllRepeater.DataBind();

        if (criteria.PagingInfo.TotalPages < 2)
        {
            ucPaging.Visible = false;
        }
        else
        {
            ucPaging.CurrentPageIndex = criteria.PagingInfo.CurrentPage - 1;
            ucPaging.TotalPages = criteria.PagingInfo.TotalPages;
        }

        hdnTargetContentListUrl.Value = targetContentManager.RequestInformation.SitePath + "widgets/targetedcontent/controls/targetcontentlist.aspx";

    }

}

public class TargetContentEventArgs : EventArgs{
    public long TargetContentId {get; set;}
}
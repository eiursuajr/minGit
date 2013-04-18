using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.BusinessObjects.Content.Targeting;
using Ektron.Cms.Content.Targeting;

public partial class Workarea_TargetedContent_TargetContentList : System.Web.UI.Page
{

    #region member variables
    private const string Page_Action = "TargetedContent";
    protected const string Edit_TargetContent_Url = "TargetContentEdit.Aspx?PageId=-100";

    ContentAPI _contentApi = new ContentAPI();
    StyleHelper _styleHelper = new StyleHelper();
    protected EkMessageHelper _msgHelper;

    #endregion


    #region events
    protected void Page_Init(object sender, EventArgs e)
    {
	     _msgHelper = new EkMessageHelper(_contentApi.RequestInformationRef);
         Utilities.ValidateUserLogin();

         if (_contentApi.RequestInformationRef.IsMembershipUser == 1 || !_contentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminPersonalize))
         {
            Response.Redirect(_contentApi.ApplicationPath + "reterror.aspx?info=" + _contentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
         } 

        //Register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        ltrlStyleSheetJS.Text = _styleHelper.GetClientScript();

        ViewAllToolbar();
        
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Bind();
    }
    #endregion

    #region member methods

    private void Bind()
    {
        TargetedContent targetContentManager = new TargetedContent(_contentApi.RequestInformationRef);
        Criteria<TargetedContentProperty> criteria = new Criteria<TargetedContentProperty>();
        criteria.AddFilter(TargetedContentProperty.IsGlobal, CriteriaFilterOperator.EqualTo, true);
        criteria.PagingInfo.RecordsPerPage = _contentApi.RequestInformationRef.PagingSize;
        criteria.PagingInfo.CurrentPage = ucPaging.SelectedPage + 1;
        
		List<TargetedContentData> targetContentList = targetContentManager.GetList(criteria);
		
		if (criteria.PagingInfo.TotalPages < 2)
        {
            ucPaging.Visible = false;
        }
        else 
        { 
           ucPaging.TotalPages = criteria.PagingInfo.TotalPages;
           ucPaging.CurrentPageIndex = ucPaging.SelectedPage;
        }
		
        ViewAllRepeater.DataSource = targetContentList;
        ViewAllRepeater.DataBind();
    }

    private void ViewAllToolbar(){
        StringBuilder result = new StringBuilder();
        txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(_msgHelper.GetMessage("lbl targeted content"));
        result.Append("<table><tr>");
        result.Append(_styleHelper.GetButtonEventsWCaption(_contentApi.AppImgPath + "../UI/Icons/add.png", Edit_TargetContent_Url, _msgHelper.GetMessage("btn add"), _msgHelper.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass,true));
        result.Append(_styleHelper.GetButtonEventsWCaption(_contentApi.AppImgPath + "../UI/Icons/delete.png", "TargetCOntentDelete.aspx", _msgHelper.GetMessage("lbl delete"), _msgHelper.GetMessage("lbl delete"), "", StyleHelper.DeleteButtonCssClass));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(_styleHelper.GetHelpButton(Page_Action, ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }

    #endregion
}

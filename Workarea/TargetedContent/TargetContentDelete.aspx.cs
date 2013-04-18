using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Common;
using Ektron.Cms;
using Ektron.Cms.BusinessObjects.Content.Targeting;
using Ektron.Cms.Content.Targeting;

public partial class Workarea_TargetedContent_TargetContentDelete : System.Web.UI.Page
{
    #region member variables
	private const string Page_Action = "TargetedContentDelete";
    ContentAPI _contentApi = new ContentAPI();
    StyleHelper _styleHelper = new StyleHelper();
    protected EkMessageHelper _msgHelper;

    #endregion


    #region events
    protected void Page_Init(object sender, EventArgs e)
    {
		var m_refMsg = new ContentAPI().EkMsgRef;

		BackLabel.Text = m_refMsg.GetMessage("btn back");
		DeleteLabel.Text = m_refMsg.GetMessage("btn delete");

        //Register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);

        ltrlStyleSheetJS.Text = _styleHelper.GetClientScript();

        _msgHelper = new EkMessageHelper(_contentApi.RequestInformationRef);

        Utilities.ValidateUserLogin();
        if (_contentApi.RequestInformationRef.IsMembershipUser == 1 || !_contentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminPersonalize)) {
            Response.Redirect(_contentApi.ApplicationPath + "reterror.aspx?info=" + _contentApi.EkMsgRef.GetMessage("msg login cms user"), true);
            return;
        }

        BindToolbars();
        this.image_link_100.Attributes.Add("onclick", string.Format("return validateList('{0}');", _msgHelper.GetMessage("select target content")));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Bind();
    }

    protected void ucDeleteButton_click(object sender, EventArgs e)
    {

        foreach (string key in Request.Form.Keys)
        {
            if (key.StartsWith("chkTargetContent_") && Request.Form[key] == "on")
            {
                long tcId = 0;
                long.TryParse(key.Substring(17), out tcId);

                if (tcId != 0)
                {
                    TargetedContent targetContentManager = new TargetedContent(_contentApi.RequestInformationRef);
                    targetContentManager.Delete(tcId);
                }
            }
        }

        Response.Redirect("TargetContentList.aspx");

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

    private void BindToolbars()
    {
        WorkareaTitlebar.InnerHtml = _msgHelper.GetMessage("lbl delete target content");
        image_cell_100.Attributes.Add("title", _msgHelper.GetMessage("lbl delete"));
        image_cell_101.Attributes.Add("title", _msgHelper.GetMessage("alt back button text"));
        uxHelpbutton.Text = _styleHelper.GetHelpButton(Page_Action, "");
        image_link_100.Attributes.Add("onmouseover", "ShowTransString('" + _msgHelper.GetMessage("lbl delete") + "');RollOver(this);");
        image_link_101.Attributes.Add("onmouseover", "ShowTransString('" + _msgHelper.GetMessage("alt back button text") + "');RollOver(this);");
    }
  
    #endregion
}

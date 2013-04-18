using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Diagnostics; 
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Workarea_ToDo_Add : System.Web.UI.Page
{
    #region MemberVariables
    protected string m_uniqueId = "__Page";
    protected StyleHelper _refStyle = new StyleHelper();
    protected   EkMessageHelper msgHelper;
    protected CommonApi _refCommonApi = new CommonApi();
    protected  ContentAPI _refContentApi = new ContentAPI();
    protected Ektron.Cms.Framework.ToDo.TodoItem _refToDoApi = new Ektron.Cms.Framework.ToDo.TodoItem();      
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        msgHelper = _refCommonApi.EkMsgRef;
        if (!(Page.IsCallback))
        {
                cgae_userselect_done_btn.Attributes.Add("onclick", "GetCommunityMsgObject('" + m_uniqueId + "').MsgSaveMessageTargetUI(); return false;");
                cgae_userselect_done_btn.Attributes.Add("class", "EktMsgTargetsDoneBtn");
                cgae_userselect_done_btn.Text = msgHelper.GetMessage("btn done") ;
                cgae_userselect_done_btn.ToolTip = cgae_userselect_done_btn.Text;
                

                cgae_userselect_cancel_btn.Attributes.Add("onclick", "GetCommunityMsgObject('" + m_uniqueId + "').MsgCancelMessageTargetUI(); return false;");
                cgae_userselect_cancel_btn.Attributes.Add("class", "EktMsgTargetsCancelBtn");
                cgae_userselect_cancel_btn.Text = msgHelper.GetMessage("btn cancel");
                cgae_userselect_cancel_btn.ToolTip = cgae_userselect_cancel_btn.Text;
        }
        
        RegisterResources();
        DisplayAddItem();
        EmitInitializationJavascript();
        FillDropDownLists(); 
        
    }

    private void DisplayAddItem()
    {
        Toolbar("add", 2);
        if (Page.IsPostBack)
        {
            //Ektron.Cms.ToDo.TodoItemData data = new Ektron.Cms.ToDo.TodoItemData();
            //data.Title = txtTitle.Text;
            //data.AssignedTo = 1;
            ////TODO.
            //_refToDoApi.Add(); 
        }

    }
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }
    private void FillDropDownLists()
    {
        ddlPriority.DataSource = Enum.GetNames(typeof(EkEnumeration.TaskPriority));
        ddlPriority.DataBind();
        ddlStatus.DataSource = Enum.GetNames(typeof(EkEnumeration.TaskStatus));
        ddlStatus.DataBind();  

    }

    private void Toolbar(string mode, long groupId)
    {
         divTitleBar.InnerHtml = _refStyle.GetTitleBar(msgHelper.GetMessage("lbl add todo item"));
         StringBuilder result = new StringBuilder(); 
         result.Append("<table><tr>\n");
         result.Append(_refStyle.GetButtonEventsWCaption(_refCommonApi.AppPath + "images/UI/Icons/save.png", "#", msgHelper.GetMessage("btn save"), msgHelper.GetMessage("btn save"), "Onclick=\"Javascript:SubmitForm('form1','VerifyAddItem()');\"", StyleHelper.SaveButtonCssClass, true));
         result.Append("</tr></table>");
         divToolBar.InnerHtml = result.ToString();
         result = null;
         StyleSheetJS.Text = (new StyleHelper()).GetClientScript();
    }
    protected void EmitInitializationJavascript()
    {
      StringBuilder sb = new StringBuilder();
        try
        {
            if(!(Page.IsPostBack))
            {
                sb.AppendLine("<script type=\"text/javascript\">");
                sb.AppendLine("<!--//--><![CDATA[//><!-- \n ");
                sb.AppendLine("GetCommunityMsgObject('" + m_uniqueId + "').SetUserSelectId('" + Invite_UsrSel.ControlId + "');");
                sb.AppendLine("//--><!]]>");
                sb.AppendLine("</script>");
                litInitialize.Text = sb.ToString(); 
                BrowseUsers.Text = "<a class=\"button buttonInlineBlock blueHover btnUpload buttonBrowseUSer\" href=\"#\" onclick=\"GetCommunityMsgObject('" + m_uniqueId + "').MsgShowMessageTargetUI('ektouserid" + m_uniqueId + "', true); return false;\" >" + "Browse" + "</a>";
            }
        }
        catch(Exception)
        {
        }
        finally
        {
            sb = null;
        }
    }
       
         
}

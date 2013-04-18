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
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class Workarea_Community_MessageBoard : System.Web.UI.Page
{
    protected Ektron.Cms.ContentAPI _ContentAPI = new Ektron.Cms.ContentAPI();
    protected Ektron.Cms.CommonApi _CommonApi = new Ektron.Cms.CommonApi(); 
    protected Ektron.Cms.Common.EkMessageHelper _msgRef;
    protected Ektron.Cms.Content.EkContent _refContentObject; 
    protected int _currentPageNumber = 1;
    protected string imagePath = String.Empty;
    protected decimal TotalPagesNumber = 1;
    protected long msgId = 0;
    protected Ektron.Cms.Community.MessageBoardAPI _MessageBoard = new Ektron.Cms.Community.MessageBoardAPI();
    protected string objectType = string.Empty;
    protected long objectId = 0;
    protected PagingInfo paging = new PagingInfo();
    protected System.Collections.ObjectModel.Collection<MessageBoardData> _MessageBoardData = new System.Collections.ObjectModel.Collection<MessageBoardData>();
    protected StyleHelper _refStyle = new StyleHelper();
    protected string _PageAction = String.Empty;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _msgRef = new Ektron.Cms.Common.EkMessageHelper(_ContentAPI.RequestInformationRef);
        _refContentObject = _ContentAPI.EkContentRef;

        if (!(_CommonApi.IsAdmin() || (_refContentObject.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.MessageBoardAdmin, _CommonApi.RequestInformationRef.UserId, false))))
        {
            Utilities.ShowError(_msgRef.GetMessage("User not authorized"));    
        }
        try
        {
            
            imagePath = _ContentAPI.AppPath + "images/ui/icons/";
            RegisterResources();
            SetServerSizeJSVariables();
            if (!Utilities.ValidateUserLogin())
                return;
            if (Request.QueryString["action"] != null || Request.QueryString["action"] != String.Empty)
            {
                _PageAction = Request.QueryString["action"];
            }
            if(Request.Form[isPostData.UniqueID] != "")
            {
                switch (_PageAction)
                {
                    case "viewmessage":
                        msgId = Convert.ToInt64(Request.QueryString["messageid"]);
                        objectType = Request.QueryString["objecttype"];
                        objectId = Convert.ToInt64(Request.QueryString["objectId"]);
                        DisplayUnApprovedMessage(msgId, objectId, objectType);
                        break;
                    case "approvemessage":
                        ApproveSelectedMessages();
                        break;
                    case "deletemessage":
                        DeleteSelectedMessages();
                        break;
                    default:
                        Display_ViewAllUnApprovedMessages(selVal.Value);
                        ViewMessageBoardToolBar();
                        break;
                }
            }
            isPostData.Value = "true";
        }
        catch (Exception ex)
        {
            Response.Redirect("../reterror.aspx?info=" + EkFunctions.UrlEncode(ex.Message), false);
        }
    }    
    private void Display_ViewAllUnApprovedMessages(string val)
    {
        paging.CurrentPage = _currentPageNumber;
        paging.RecordsPerPage = _ContentAPI.RequestInformationRef.PagingSize;

        _MessageBoardData = new System.Collections.ObjectModel.Collection<MessageBoardData>();
        
        switch (val)
        {
            case "0":
                _MessageBoardData = _MessageBoard.GetUnApprovedList(paging, _ContentAPI.UserId);
                break;
            case "1":
                _MessageBoardData = _MessageBoard.GetUnApprovedList(Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.Content, paging, _ContentAPI.UserId);
                break;
            case "2":
                _MessageBoardData = _MessageBoard.GetUnApprovedList(Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.CommunityGroup, paging, _ContentAPI.UserId);
                break;
            case "3":
                _MessageBoardData = _MessageBoard.GetUnApprovedList(Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.User, paging, _ContentAPI.UserId);
                break;
            case "4":
                _MessageBoardData = _MessageBoard.GetUnApprovedList(Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.MessageReply, paging, _ContentAPI.UserId);
                break;
            default:
                _MessageBoardData = _MessageBoard.GetUnApprovedList(paging, _ContentAPI.UserId);
                break;
        }

        System.Web.UI.WebControls.BoundColumn colBound = new BoundColumn();

        colBound = new BoundColumn();
        colBound.DataField = "SELECT";
        colBound.HeaderText = "<input type='checkbox' name='selectAll' id='selectAll' onclick=' SelectAll(this);' />"; 
        colBound.HeaderStyle.CssClass = "title-header";
        colBound.HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(5);
        ViewUnApprovedMessages.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = "MESSAGES";
        colBound.HeaderText = _msgRef.GetMessage("lbl subscription message properties");
        colBound.HeaderStyle.CssClass = "title-header";
        ViewUnApprovedMessages.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = "USERNAME";
        colBound.HeaderText = _msgRef.GetMessage("lbl user name");
        colBound.HeaderStyle.CssClass = "title-header";
        ViewUnApprovedMessages.Columns.Add(colBound);

        colBound = new BoundColumn();
        colBound.DataField = "TYPE";
        colBound.HeaderText = _msgRef.GetMessage("generic type");
        colBound.HeaderStyle.CssClass = "title-header";
        ViewUnApprovedMessages.Columns.Add(colBound);

        DataTable dt = new DataTable();
        DataRow dr;
        int i = 0;

        dt.Columns.Add(new DataColumn("SELECT", typeof(string)));
        dt.Columns.Add(new DataColumn("MESSAGES", typeof(string)));
        dt.Columns.Add(new DataColumn("USERNAME", typeof(string)));
        dt.Columns.Add(new DataColumn("TYPE", typeof(string)));

        for (i = 0; i < _MessageBoardData.Count; i++)
        {
            string _Message = String.Empty;
            if (_MessageBoardData[i].MessageText.Length > 50)
            {
                _Message = _MessageBoardData[i].MessageText.Substring(0,50) + "....";
            }
            else 
            {
                _Message = _MessageBoardData[i].MessageText;            
            }
            dr = dt.NewRow();

            dr[0] = "<input type='checkbox' msgId='" + _MessageBoardData[i].MessageId + "' name='selMessage" + i + "' id='selMessage" + i + "' onclick='VerifySelection();' />";
            dr[1] = "<a href='#' id='message" + i + "' onclick='ShowMessage(this); return false;' msgId='" + _MessageBoardData[i].MessageId + "' objType='" + _MessageBoardData[i].ObjectType + "' objId='" + _MessageBoardData[i].ObjectId + "' title='Click to view entire message'>" + _Message + "</a>";
            dr[2] = "<label id='lblUser" + i + "' name='lblUser" + i + "'>" + _MessageBoardData[i].UserName + "</label>";
            if (_MessageBoardData[i].ObjectType != Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.MessageReply)
            {
                dr[3] = "<label id='lblType" + i + "' name='lblType" + i + "'>" + _MessageBoardData[i].ObjectType + " " + _msgRef.GetMessage("lbl msg board") + "</label>";
            }
            else
            {
                dr[3] = "<label id='lblType" + i + "' name='lblType" + i + "'>" + _MessageBoardData[i].ObjectType + "</label>";
            }
            dt.Rows.Add(dr);
        }
        DataView dv = new DataView(dt);
        ViewUnApprovedMessages.DataSource = dv;
        ViewUnApprovedMessages.DataBind();

        TotalPagesNumber = paging.TotalPages;

        SetPaging();
    }
    protected void DisplayUnApprovedMessage(long messageId, long objectId, string objType)
    {
        paging.CurrentPage = _currentPageNumber;
        paging.RecordsPerPage = _ContentAPI.RequestInformationRef.PagingSize;
        Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType Type = new Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType();
        MessageBoardData _MessageById = new MessageBoardData();
        MessageBoardData _ParentMessage = new MessageBoardData();

        switch (objType)
        { 
            case "Content":
                Type = Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.Content;
                break;
            case "CommunityGroup":
                Type = Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.CommunityGroup;
                break;
            case "User":
                Type = Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.User;
                break;
            case "MessageReply":
                Type = Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.MessageReply;
                break;
        }        
        _MessageById = _MessageBoard.GetMessageBoardEntry(messageId, objectId, Type, true, _ContentAPI.UserId);
        ltrMessage.Text += "<div class='commentWrapper'>";
        ltrMessage.Text += "    <blockquote>";
        ltrMessage.Text += "        <span class='bqStart'>“</span>";
        ltrMessage.Text += _MessageById.MessageText;
        ltrMessage.Text += "        <span class='bqEnd'>”</span>";
        ltrMessage.Text += "    </blockquote>";
        if (Type == Ektron.Cms.Common.EkEnumeration.MessageBoardObjectType.MessageReply)
        {
            ltrMessage.Text += "    <label class='replyTo'>" + _msgRef.GetMessage("lbl reply to message") + ":</label>";
            _ParentMessage = _MessageBoard.GetMessageBoardEntry(objectId, true, _ContentAPI.UserId);
            ltrMessage.Text += "    <blockquote>";
            ltrMessage.Text += "        <span class='bqStart'>“</span>";
            ltrMessage.Text += _ParentMessage.MessageText;
            ltrMessage.Text += "        <span class='bqEnd'>”</span>";
            ltrMessage.Text += "    </blockquote>";
        }
        ltrMessage.Text += "</div>";
        ViewUnApprovedMessages.Visible = false;
        ViewUnApprovedMessage.Visible = true;
        hdnMessageInfo.Value = _MessageById.MessageId.ToString();
        ViewMessageBoardToolBar();
        SetPaging();
    }
    protected void ApproveSelectedMessages()
    { 
        System.Collections.Generic.List<long> _MessageIdList = new System.Collections.Generic.List<long>();
        
        if (Request.QueryString["id"] != null && Request.QueryString["id"] != String.Empty)
        {
            _MessageIdList.Add(Convert.ToInt64(Request.QueryString["id"]));
        }
        else if (Request.QueryString["ids"] != null && Request.QueryString["ids"] != String.Empty)
        {
            string[] msgIds = null;
            char[] splitChar = { ',' };
            msgIds = Request.QueryString["ids"].Split(splitChar);
            int i = 0;
            for (i = 0; i < msgIds.Length - 1; i++)
            {
                _MessageIdList.Add(Convert.ToInt64(msgIds[i]));
            }
        }
        _MessageBoard.ApproveMessages(_MessageIdList, _ContentAPI.UserId);
        Response.Redirect("messageboard.aspx", false);
    }
    protected void DeleteSelectedMessages()
    {
        System.Collections.Generic.List<long> _MessageIdList = new System.Collections.Generic.List<long>();

        if (Request.QueryString["id"] != null && Request.QueryString["id"] != String.Empty)
        {
            _MessageIdList.Add(Convert.ToInt64(Request.QueryString["id"]));
        }
        else if (Request.QueryString["ids"] != null && Request.QueryString["ids"] != String.Empty)
        {
            string[] msgIds = null;
            char[] splitChar = { ',' };
            msgIds = Request.QueryString["ids"].Split(splitChar);
            int i = 0;
            for (i = 0; i < msgIds.Length - 1; i++)
            {
                _MessageIdList.Add(Convert.ToInt64(msgIds[i]));
            }
        }
        _MessageBoard.DeleteMessages(_MessageIdList, _ContentAPI.UserId);
        Response.Redirect("messageboard.aspx", false);
    }
    protected void ViewMessageBoardToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        string helpScreenAlias = string.Empty;
        string _All = string.Empty;
        string _Content = string.Empty;
        string _CommunityGroup = string.Empty;
        string _User = string.Empty;
        string _Reply = string.Empty;

        switch (selVal.Value)
        {
            case "0":
                _All = "selected";
                break;
            case "1":
                _Content = "selected";
                break;
            case "2":
                _CommunityGroup = "selected";
                break;
            case "3":
                _User = "selected";
                break;
            case "4":
                _Reply = "selected";
                break;
            default:
                _All = "selected";
                break;
        }

        divTitleBar.InnerHtml = _msgRef.GetMessage("view all unapproved msg");
        result.Append("<table><tr>");

        if (_PageAction != "viewmessage")
        {
            result.Append(_refStyle.GetButtonEventsWCaption(imagePath + "approvalApproveItem.png", "#", _msgRef.GetMessage("generic approve title"), _msgRef.GetMessage("generic approve title"), "onclick='return GetMessagesToApprove();'", StyleHelper.ApproveButtonCssClass, true));
            result.Append(_refStyle.GetButtonEventsWCaption(imagePath + "delete.png", "#", _msgRef.GetMessage("btn delete"), _msgRef.GetMessage("btn delete"), "onclick='return DeleteSelectedMessages();'", StyleHelper.DeleteButtonCssClass));

			result.Append(StyleHelper.ActionBarDivider);

            helpScreenAlias = "unapprovedmessages";
            result.Append("<td><select name='objectType' id='objectType' onchange='SetObjectType(this); return false;'>");
            result.Append(" <option value='0' " + _All + ">" + _msgRef.GetMessage("generic all") + "</option>");
            result.Append(" <option value='1' " + _Content + ">" + _msgRef.GetMessage("top content") + "</option>");
            result.Append(" <option value='2' " + _CommunityGroup + ">" + _msgRef.GetMessage("lbl community group") + "</option>");
            result.Append(" <option value='3' " + _User + ">" + _msgRef.GetMessage("lbl wa mkt user goals") + "</option>");
            result.Append(" <option value='4' " + _Reply + ">" + _msgRef.GetMessage("lbl reply") + "</option>");
            result.Append("</select></td>");
        }
        else
        {
			result.Append(_refStyle.GetButtonEventsWCaption(imagePath + "back.png", "#", _msgRef.GetMessage("btn back"), _msgRef.GetMessage("btn back"), "onclick='history.go(-1);'", StyleHelper.BackButtonCssClass, true));
			result.Append(_refStyle.GetButtonEventsWCaption(imagePath + "approvalApproveItem.png", "#", _msgRef.GetMessage("generic approve title"), _msgRef.GetMessage("generic approve title"), "onclick='return ApproveMessage();'", StyleHelper.ApproveButtonCssClass, true));
			result.Append(_refStyle.GetButtonEventsWCaption(imagePath + "delete.png", "#", _msgRef.GetMessage("btn delete"), _msgRef.GetMessage("btn delete"), "onclick='return DeleteMessage();'", StyleHelper.DeleteButtonCssClass));
            
            helpScreenAlias = "unapprovedmessages";
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + _refStyle.GetHelpButton(helpScreenAlias, "center") + "</td>");
        result.Append("</tr></table>");

        divToolBar.InnerHtml = result.ToString();

        StyleSheetJS.Text = _refStyle.GetClientScript();
    }
    protected void RegisterResources()
    {
        //JavaScript Registration
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);

        //CSS Registration
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
    }
    protected void SetServerSizeJSVariables()
    {
        ltrSelMsgToApprove.Text = _msgRef.GetMessage("js select message to approve");
        ltrSelMsgToDelete.Text = _msgRef.GetMessage("js select message to delete");
        litDeleteMessage.Text = _msgRef.GetMessage("js:are you sure you want to delete this post");
    }
    protected void SetPaging()
    {
        if (TotalPagesNumber <= 1)
        {
            TotalPages.Visible = false;
            CurrentPage.Visible = false;
            lnkBtnPreviousPage.Visible = false;
            NextPage.Visible = false;
            LastPage.Visible = false;
            FirstPage.Visible = false;
            PageLabel.Visible = false;
            OfLabel.Visible = false;
        }
        else
        {
            lnkBtnPreviousPage.Enabled = true;
            FirstPage.Enabled = true;
            LastPage.Enabled = true;
            NextPage.Enabled = true;
            TotalPages.Visible = true;
            CurrentPage.Visible = true;
            lnkBtnPreviousPage.Visible = true;
            NextPage.Visible = true;
            LastPage.Visible = true;
            FirstPage.Visible = true;
            PageLabel.Visible = true;
            OfLabel.Visible = true;
            TotalPages.Text = (System.Math.Ceiling(TotalPagesNumber)).ToString();
            TotalPages.ToolTip = TotalPages.Text;

            CurrentPage.Text = _currentPageNumber.ToString();
            CurrentPage.ToolTip = CurrentPage.Text;

            if (_currentPageNumber == 1)
            {
                lnkBtnPreviousPage.Enabled = false;
                FirstPage.Enabled = false;
            }
            else if (_currentPageNumber == TotalPagesNumber)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }
    protected void NavigationLink_Click(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "First":
                _currentPageNumber = 1;
                break;
            case "Last":
                _currentPageNumber = Int32.Parse(TotalPages.Text);
                break;
            case "Next":
                _currentPageNumber = Int32.Parse(CurrentPage.Text) + 1;
                break;
            case "Prev":
                _currentPageNumber = Int32.Parse(CurrentPage.Text) - 1;
                break;
        }
        Display_ViewAllUnApprovedMessages(selVal.Value);

        isPostData.Value = "true";
    }
}

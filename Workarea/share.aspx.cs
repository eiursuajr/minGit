using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.API;
using Ektron.Cms.Content;
using Ektron.Cms.Framework.SocialNetworking;


public partial class Workarea_share : System.Web.UI.Page
{
    #region Private Member Variables
    Common _Common;
    CommonApi _CommonApi;
    ContentAPI _ContentApi;
    String _PageTitle;
    String _PageUrl;
    String _PageDescription;
    EkMessageHelper _MessageHelper;
    Int32 _MaxDescriptionLength;
    MicroMessage _MicroMessage;
    MicroMessageData _MicroMessageData;
    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _CommonApi = new CommonApi();
        _Common = new Common();
        _ContentApi = new ContentAPI();
        _PageTitle = "";
        _PageUrl = "";
        _PageDescription = "";
        _MessageHelper = _ContentApi.EkMsgRef;
        _MaxDescriptionLength = 200;
        _MicroMessage = new MicroMessage();
        _MicroMessageData = new MicroMessageData();
		
		Utilities.ValidateUserLogin();
        RegisterResources();

        // if the user is already logged in, hide the login panel and reveal the share form
        if (_Common.UserId > 0)
        {
            // set panels to proper visibility
            loginControlPanel.Visible = false;
            shareFormPanel.Visible = true;

            // assign string values as appropriate
            shareHeader.Text = _MessageHelper.GetMessage("post to profile");
            commentsLabel.Text = _MessageHelper.GetMessage("share this comments label");
            sharePost.Text = _MessageHelper.GetMessage("share");
            sharePost.ToolTip = sharePost.Text;
            cancelPost.Text = _MessageHelper.GetMessage("generic cancel");
            cancelPost.ToolTip = cancelPost.Text;
            characterLimitExceededHeader.Text = _MessageHelper.GetMessage("generic max character");
            characterLimitExceededBody.Text = _MessageHelper.GetMessage("micromessagingbookmarklet character limit exceeded");
            maxLength.Text = _MaxDescriptionLength.ToString();

            // page title
            if (!String.IsNullOrEmpty(Request.QueryString["t"]))
            {
                _PageTitle = Request.QueryString["t"];
                if (_PageTitle.Length > 0)
                {
                    pageTitle.Text = _PageTitle;
                    pageTitleField.Text = _PageTitle;
                }
            }
            // page URL
            if (!String.IsNullOrEmpty(Request.QueryString["url"]))
            {
                _PageUrl = Request.QueryString["url"];
                pageUrl.Text = _PageUrl;
                pageUrlField.Text = _PageUrl;
            }
            // page Description
            if (!String.IsNullOrEmpty(Request.QueryString["ds"]))
            {
                _PageDescription = TruncateString(Request.QueryString["ds"].ToString(), _MaxDescriptionLength, TruncateOptions.IncludeEllipsis | TruncateOptions.FinishWord);
                pageDescription.Text = _PageDescription;
                pageDescriptionField.Text = _PageDescription;
            }
        }
    }
    #region Helpers
    protected bool RegisterResources()
    {
        // CSS
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE7);
        // JS
        JS.RegisterJS(this, JS.ManagedScript.EktronInputLabelJS);

        return true;
    }

    public string TruncateString(string valueToTruncate, int maxLength, TruncateOptions options)
    {
        if (valueToTruncate == null)
        {
            return "";
        }

        if (valueToTruncate.Length <= maxLength)
        {
            return valueToTruncate;
        }

        bool includeEllipsis = (options & TruncateOptions.IncludeEllipsis) == TruncateOptions.IncludeEllipsis;
        bool finishWord = (options & TruncateOptions.FinishWord) == TruncateOptions.FinishWord;
        bool allowLastWordOverflow = (options & TruncateOptions.AllowLastWordToGoOverMaxLength) == TruncateOptions.AllowLastWordToGoOverMaxLength;

        string retValue = valueToTruncate;

        if (includeEllipsis)
        {
            maxLength -= 1;
        }

        int lastSpaceIndex = retValue.LastIndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);

        if (!finishWord)
        {
            retValue = retValue.Remove(maxLength);
        }
        else if (allowLastWordOverflow)
        {
            int spaceIndex = retValue.IndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);
            if (spaceIndex != -1)
            {
                retValue = retValue.Remove(spaceIndex);
            }
        }
        else if (lastSpaceIndex > -1)
        {
            retValue = retValue.Remove(lastSpaceIndex);
        }

        if (includeEllipsis && retValue.Length < valueToTruncate.Length)
        {
            retValue += "...";
        }
        return retValue;
    }

    public enum TruncateOptions
    {
        None = 0x0,
        FinishWord = 0x1,
        AllowLastWordToGoOverMaxLength = 0x2,
        IncludeEllipsis = 0x4
    }
    #endregion
    protected void sharePost_Click(object sender, EventArgs e)
    {
        if (_MicroMessage.RequestInformation.UserId > 0)
        {
            String message = String.Empty;
            // build the message from the form fields
            if (!String.IsNullOrEmpty(Request.Form["comments"]))
            {
                message += Request.Form["comments"] + "\n\n";
            }
            if (!String.IsNullOrEmpty(Request.Form["pageTitleField"]))
            {
                message += Request.Form["pageTitleField"] + "\n";
            }
            if (!String.IsNullOrEmpty(Request.Form["pageUrlField"]))
            {
                message += Request.Form["pageUrlField"] + "\n";
            }
            if (!String.IsNullOrEmpty(Request.Form["pageDescriptionField"]))
            {
                message += Request.Form["pageDescriptionField"];
            }

            _MicroMessageData.MessageText = EkFunctions.RemoveHTML(message);
            _MicroMessageData.UserId = _MicroMessage.RequestInformation.UserId;
            try
            {
                _MicroMessage.Add(_MicroMessageData);
                commentsWrapper.Visible = false;
                shareSuccess.Visible = true;
                shareSuccessHeader.Text = _MessageHelper.GetMessage("micromessagingbookmarklet success header");
                shareSuccessWindowClosing.Text = String.Format("{0} <a href='JavaScriptRequired.aspx' class='closeWindowLink' onclick='self.close(); return false;'>{1}</a>", _MessageHelper.GetMessage("window closing automatically"), _MessageHelper.GetMessage("alt close this window"));
                JS.RegisterJSBlock(this, "setTimeout('self.close()', 5000)", "EktronUXAutoWindowClose");
            }
            catch
            {
                // nothing for now
            }            
        }
    }
}

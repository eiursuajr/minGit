using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.Search;
using Ektron.Cms.Search;
using System;

public partial class Workarea_controls_search_viewsuggestedresult : System.Web.UI.UserControl
{
    protected SiteAPI _siteApi = new SiteAPI();
    protected StyleHelper _styleHelper = new StyleHelper();
    protected EkMessageHelper _messageHelper;
    protected string _appImgPath;    

    #region Page Load
    protected void Page_Load(object sender, System.EventArgs e)
    {
        // initialize necessary variables
        _messageHelper = (new CommonApi()).EkMsgRef;
        _messageHelper = _siteApi.EkMsgRef;
        _appImgPath = _siteApi.AppImgPath;        

        string idParam = Request.QueryString["termID"];
        Guid id;
        if (!string.IsNullOrEmpty(idParam) && Guid.TryParse(idParam, out id))
        {
            ISuggestedResults suggestedResults = ObjectFactory.GetSuggestedResults();
            SuggestedResultSet suggestedResultSet = suggestedResults.GetItem(id);
            if (suggestedResultSet != null)
            {
                ViewToolBar(suggestedResultSet);

                litTermName.Text = suggestedResultSet.Name;

                if (suggestedResultSet.Phrases != null)
                {
                    litSynonyms.Text = String.Join(";", suggestedResultSet.Phrases);                    
                }

                StringBuilder jsBuilder = new StringBuilder();
                int suggestedResultIndex = 1;
                foreach (SuggestedResult suggestedResult in suggestedResultSet.SuggestedResults)
                {
                    // Build list item information for individual suggested
                    // result items.

                    HtmlGenericControl suggestedResultListItem = new HtmlGenericControl("li");
                    suggestedResultListItem.Attributes.Add("class", "suggestedResult");

                    HtmlGenericControl anchorDiv = new HtmlGenericControl("div");
                    anchorDiv.Attributes.Add("class", "anchor");

                    HtmlGenericControl anchorSpan = new HtmlGenericControl("span");
                    anchorSpan.Attributes.Add("class", "suggestedResultLink");
                    anchorSpan.InnerText = suggestedResult.Title;

                    HtmlGenericControl summaryDiv = new HtmlGenericControl("div");
                    summaryDiv.Attributes.Add("class", "suggestedResultSummary");
                    summaryDiv.InnerHtml = HttpUtility.HtmlDecode(suggestedResult.Description);

                    anchorDiv.Controls.Add(anchorSpan);
                    anchorDiv.Controls.Add(summaryDiv);
                    suggestedResultListItem.Controls.Add(anchorDiv);

                    selectedSuggestedResults.Controls.Add(suggestedResultListItem);

                    // Append a corresponding javascript variable for the
                    // relevent suggested result.

                    jsBuilder.Append("var existingSuggestedResultObject = new suggestedResultObject(");
                    jsBuilder.Append("\'" + suggestedResult.Title.Replace("\'", "\\\'") + "\',");
                    jsBuilder.Append("\'" + suggestedResult.Title.Replace("\'", "\\\'") + "\',");
                    jsBuilder.Append("\'" + suggestedResult.Url.Replace("\'", "\\\'") + "\',");
                    jsBuilder.Append("\'" + suggestedResult.Description.Replace("\'", "\\\'").Replace("\r", " ").Replace("\n", " ") + "\',");
                    jsBuilder.Append("\'" + suggestedResult.Title + "\',");
                    jsBuilder.Append(suggestedResultIndex.ToString());
                    jsBuilder.Append(");" + "\r\n");
                    jsBuilder.Append("arrSuggestedResults.push(existingSuggestedResultObject);" + "\r\n" + "\r\n");

                    suggestedResultIndex++;
                }

                javaScriptSRObjects.Text = jsBuilder.ToString();
            }
        }
        else
        {
        }
    }
    #endregion

    #region SUB: ViewToolBar
    private void ViewToolBar(SuggestedResultSet suggestedResultSet)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(string.Format(_messageHelper.GetMessage("msg edit-delete suggested results"), "\"" + suggestedResultSet.Name + "\""));

        result.Append("<table><tr>");

		result.Append(
		   _styleHelper.GetButtonEventsWCaption(
			   _appImgPath + "../UI/Icons/back.png",
			   "javascript:history.back(1);",
			   _messageHelper.GetMessage("alt back button text"),
			   _messageHelper.GetMessage("btn back"),
			   string.Empty,
			   StyleHelper.BackButtonCssClass,
			   true));

        result.Append(
            _styleHelper.GetButtonEventsWCaption(
                _appImgPath + "../UI/Icons/contentEdit.png", 
                "suggestedresults.aspx?action=EditSuggestedResult&termID=" + suggestedResultSet.Id.ToString(), 
                _messageHelper.GetMessage("msg edit suggested result"), 
                _messageHelper.GetMessage("generic edit title"), 
                string.Empty,
				StyleHelper.EditButtonCssClass,
				true));

        result.Append(
            _styleHelper.GetButtonEventsWCaption(
                _appImgPath + "../UI/Icons/delete.png", 
                "suggestedresults.aspx?action=DeleteSuggestedResult&termID=" + suggestedResultSet.Id.ToString(), 
                _messageHelper.GetMessage("alt delete suggested results button text"), 
                _messageHelper.GetMessage("generic delete title"), 
                "OnClick=\"javascript:return VerifyDeleteSuggestedResultSet();\"",
				StyleHelper.DeleteButtonCssClass));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_styleHelper.GetHelpButton("editsuggestedresults", string.Empty));
        result.Append("</td>");
        result.Append("</tr></table>");

        // output the result string to the htmToolBar
        htmToolBar.InnerHtml = result.ToString();
    }
    #endregion

}
	

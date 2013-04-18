using System;
using System.Text;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.Search;
using Ektron.Cms.Search;
using System.Web;

public partial class Workarea_controls_search_addsuggestedresult : System.Web.UI.UserControl
{
    protected SiteAPI _siteApi;
    protected StyleHelper _styleHelper;
    protected EkMessageHelper _messageHelper;
    protected string _appImgPath;
    protected string _contentLanguage;
    protected string _pageMode;
    protected string _pageAction;
    protected string _originalPhrase;
    protected int _suggestedResultRecommendedMaxSize;
    protected Guid setId;

    public Workarea_controls_search_addsuggestedresult()
    {
        _siteApi = new SiteAPI();
        _styleHelper = new StyleHelper();
        _appImgPath = string.Empty;
        _contentLanguage = string.Empty;
        _pageMode = string.Empty;
        _pageAction = string.Empty;
        _originalPhrase = string.Empty;
        _suggestedResultRecommendedMaxSize = 10;
    }
    
    private void SaveSuggestedResult()
    {
        SuggestedResultSet suggestedResultSet = new SuggestedResultSet();

        // Set suggested result set title.

        suggestedResultSet.Name = Request.Form["addsuggestedresult$txtPhrase"];

        // Apply suggested result set synonyms.

        if (!string.IsNullOrEmpty(Request.Form["addsuggestedresult$txtSynonyms"]))
        {
            string[] synonyms = Request.Form["addsuggestedresult$txtSynonyms"].Split(
                new string[] { ";" },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string synonym in synonyms)
            {
                suggestedResultSet.Phrases.Add(synonym.Trim().ToLowerInvariant());
            }
        }

        // Apply "best bet" items to the suggested result set.

        int suggestedResultCount;
        if (int.TryParse(Request.Form["numSuggestedResults"], out suggestedResultCount) && suggestedResultCount > 0)
        {
            for (int i = 0; i < suggestedResultCount; i++)
            {
                SuggestedResult suggestedResult = new SuggestedResult();
                suggestedResult.Title = Request.Form["suggestedResult_Title_" + i.ToString()];
                suggestedResult.Description = Request.Form["suggestedResult_Summary_" + i.ToString()];
                suggestedResult.Url = Request.Form["suggestedResult_Link_" + i.ToString()];
                
                suggestedResultSet.SuggestedResults.Add(suggestedResult);
            }
        }

        try
        {
            ISuggestedResults suggestedResults = ObjectFactory.GetSuggestedResults();
            if (_pageAction == "addsuggestedresult")
            {
                suggestedResults.Add(suggestedResultSet);
            }
            else if (_pageAction == "editsuggestedresult")
            {
                suggestedResultSet.Id = setId;
                suggestedResults.Update(suggestedResultSet);
            }

            Response.Redirect("suggestedresults.aspx?action=ViewSuggestedResults", false);
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);  
        }
    }    

    protected void Page_Load(object sender, System.EventArgs e)
    {
        // initialize necessary variables
        _messageHelper = _siteApi.EkMsgRef;
        _appImgPath = _siteApi.AppImgPath;
        _pageAction = Request.QueryString["action"].ToLower();

        // register CSS
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.AllIE);

        // populate any text variables as needed
        lblCMSContent.Text = _messageHelper.GetMessage("generic cms Content");

        // Get the term info if needed
        ISuggestedResults suggestedResults = ObjectFactory.GetSuggestedResults();
        SuggestedResultSet suggestedResultSet = null;


        if (!string.IsNullOrEmpty(Request.QueryString["termID"]) && Guid.TryParse(Request.QueryString["termID"], out setId))
        {
            suggestedResultSet = suggestedResults.GetItem(setId);
            ViewToolBar(suggestedResultSet.Name);
        }
        else
        {
            suggestedResultSet = new SuggestedResultSet();
            ViewToolBar(string.Empty);
        }

        RenderAddEditViewToolBar();

        // figure out what mode we're in
        _pageMode = Request.Form["addsuggestedresult$submitMode"];
        if (Page.IsPostBack && _pageMode == "0" &&
           (_pageAction == "addsuggestedresult" || _pageAction == "editsuggestedresult"))
        {
            // if we've made it this far, add the NEW suggested results
            SaveSuggestedResult();

        }
        else if (!Page.IsPostBack && _pageAction == "editsuggestedresult")
        {
            // if we're editing an existing set of Suggested Results,
            // let's get them and output them to the array

            if (!string.IsNullOrEmpty(suggestedResultSet.Name))
            {
                hdnOriginalPhrase.Value = suggestedResultSet.Id.ToString();
                txtPhrase.Text = suggestedResultSet.Name;

                if (suggestedResultSet.Phrases != null)
                {
                    foreach (string synonym in suggestedResultSet.Phrases)
                    {
                        if (!string.IsNullOrEmpty(txtSynonyms.Text))
                        {
                            txtSynonyms.Text += ";";
                        }

                        txtSynonyms.Text += synonym;
                    }
                }

                StringBuilder jsBuilder = new StringBuilder();
                if (suggestedResultSet.SuggestedResults != null)
                {
                    int suggestedResultIndex = 1;
                    foreach (SuggestedResult suggestedResult in suggestedResultSet.SuggestedResults)
                    {
                        jsBuilder.Append("var existingSuggestedResultObject = new suggestedResultObject(");
                        jsBuilder.Append("\'" + suggestedResult.Title.Replace("\'", "\\\'") + "\',");
                        jsBuilder.Append("\'" + suggestedResult.Title.Replace("\'", "\\\'") + "\',");
                        jsBuilder.Append("\'" + suggestedResult.Url.Replace("\'", "\\\'") + "\',");
                        jsBuilder.Append("\'" + suggestedResult.Description.Replace("\'", "\\\'").Replace("\r", " ").Replace("\n", " ") + "\',");
                        jsBuilder.Append(0 + ",");
                        jsBuilder.Append(suggestedResultIndex.ToString());
                        jsBuilder.Append(");" + "\r\n");
                        jsBuilder.Append("arrSuggestedResults.push(existingSuggestedResultObject);" + "\r\n" + "\r\n");

                        suggestedResultIndex++;
                    }
                }

                javaScriptSRObjects.Text = jsBuilder.ToString();
            }
            else
            {
                // Redirect back to main screen? Should never get here.
            }
        }

        HtmlEditor1.AllowFonts = true;
    }    

    protected void ViewToolBar(string termName)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        // check the Page Action and set the Title Bar Text appropriately
        if (_pageAction == "addsuggestedresult")
        {
            txtTitleBar.InnerHtml = _styleHelper.GetTitleBar(_messageHelper.GetMessage("msg add suggested results"));
        }
        else
        {
            txtTitleBar.InnerHtml = _styleHelper.GetTitleBar((string)(_messageHelper.GetMessage("msg edit suggested results") + " \"" + termName + "\""));
        }

        result.Append("<table><tr>");

		if (_pageAction == "addsuggestedresult")
		{
			result.Append(_styleHelper.GetButtonEventsWCaption(
				_appImgPath + "../UI/Icons/back.png",
				"suggestedresults.aspx?action=ViewSuggestedResults",
				_messageHelper.GetMessage("alt back button text"),
				_messageHelper.GetMessage("btn back"),
				string.Empty,
				StyleHelper.BackButtonCssClass,
				true));
		}
		else
		{
			result.Append(_styleHelper.GetButtonEventsWCaption(
				_appImgPath + "../UI/Icons/back.png",
				"suggestedresults.aspx?action=ViewSuggestedResult" + "&termID=" + setId,
				_messageHelper.GetMessage("alt back button text"),
				_messageHelper.GetMessage("btn back"),
				string.Empty,
				StyleHelper.BackButtonCssClass,
				true));
		}

        result.Append(_styleHelper.GetButtonEventsWCaption(_appImgPath + "../UI/Icons/save.png", "javascript:checkAddSuggestedResults(document.forms[0]);", _messageHelper.GetMessage("alt save button text suggested result"), _messageHelper.GetMessage("btn save suggested result"), string.Empty, StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        if (_pageAction == "addsuggestedresult")
        {
            result.Append(_styleHelper.GetHelpButton("addsuggestedresults", string.Empty));
        }
        else
        {
            result.Append(_styleHelper.GetHelpButton("editsuggestedresults", string.Empty));
        }
        result.Append("</td>");
        result.Append("</tr></table>");

        // output the result string to the htmToolBar
        htmToolBar.InnerHtml = result.ToString();
    }
        
    protected void RenderAddEditViewToolBar()
    {
        StringBuilder result = new StringBuilder();
        // check the Page Action and set the Title Bar Text appropriately
        if (_pageAction == "addsuggestedresult")
        {
            divTitleBar.InnerHtml = _styleHelper.GetTitleBar(_messageHelper.GetMessage("msg add new suggested result"));
        }
        else
        {
            divTitleBar.InnerHtml = _styleHelper.GetTitleBar(_messageHelper.GetMessage("msg edit suggested result"));
        }

        result.Append("<table><tr>");

		result.Append(
			_styleHelper.GetButtonEventsWCaption(
				_appImgPath + "../UI/Icons/back.png",
				"javascript:cancelAddEdit();",
				_messageHelper.GetMessage("alt back button text"),
				_messageHelper.GetMessage("btn back"),
				string.Empty,
				StyleHelper.BackButtonCssClass,
				true));

        result.Append(
            _styleHelper.GetButtonEventsWCaption(
                _appImgPath + "../UI/Icons/save.png",
                "javascript:transferAddEdit();",
                _messageHelper.GetMessage("alt update button text single suggested result"),
                _messageHelper.GetMessage("btn add suggested result"),
                string.Empty,
				StyleHelper.SaveButtonCssClass,
				true));
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(_styleHelper.GetHelpButton("addeditsuggestedresult", string.Empty));
        result.Append("</td>");
        result.Append("</tr></table>");

        // output the result string to the htmToolBar
        divToolBar.InnerHtml = result.ToString();
    }
}
	


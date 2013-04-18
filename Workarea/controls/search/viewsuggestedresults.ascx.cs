using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.Framework.Search;
using Ektron.Cms.Search;


/// <summary>
/// This control is used to output the current Suggested Results in the CMS to the workarea.
/// </summary>
/// <remarks></remarks>

	public partial class Workarea_controls_search_viewsuggestedresults : System.Web.UI.UserControl
	{
        private const string ViewSuggestedResultSetUrlFormat = "~/workarea/search/suggestedresults.aspx?termID={0}&action=ViewSuggestedResult";
        private const string OpenInNewWindowScript = "window.open(this.href); return false;";		

		protected SiteAPI siteApi = new SiteAPI();
		protected StyleHelper styleHelper = new StyleHelper();
		protected EkMessageHelper messageHelper;
		protected string AppImgPath;		
		
		#region Page Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// initialize required page load objects/variables
            messageHelper = siteApi.EkMsgRef;
			AppImgPath = siteApi.AppImgPath;
			
			// build the toolbar
			ViewToolBar(txtTitleBar, htmToolBar);
			
            // Display the Synonym Sets
			OutputSuggestedResults();
			
			// Register JS
			JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
			
			// register CSS
			Css.RegisterCss(this, Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
		#endregion
		
		#region SUB: ViewToolBar
		protected void ViewToolBar(HtmlGenericControl objTitleBar, HtmlGenericControl objHTMToolBar)
		{
			// place the proper title in the Title Bar
			
            objTitleBar.InnerHtml = styleHelper.GetTitleBar(messageHelper.GetMessage("msg view suggested results"));
			
			// build the string for rendering the htmToolBar

            StringBuilder result = new StringBuilder();
            result.Append("<table><tr>");
			
            result.Append(styleHelper.GetButtonEventsWCaption(
                siteApi.AppPath + "images/UI/Icons/add.png", 
                "suggestedresults.aspx?action=AddSuggestedResult",
                messageHelper.GetMessage("alt add button text suggested result"),
                messageHelper.GetMessage("btn add suggested result"), "", StyleHelper.AddButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>");
			result.Append(styleHelper.GetHelpButton("suggestedresults", ""));
			result.Append("</td>");
			result.Append("</tr></table>");
			
			// output the result string to the htmToolBar
			objHTMToolBar.InnerHtml = result.ToString();
		}
		#endregion
		
		#region SUB: OutputSuggestedResults
		protected void OutputSuggestedResults()
		{			
			try
			{
                ISuggestedResults suggestedResults = ObjectFactory.GetSuggestedResults();
                List<SuggestedResultSet> suggestedResultsSets = suggestedResults.GetList();                
				
                foreach (SuggestedResultSet suggestedResultSet in suggestedResultsSets)
                {
                    // Create the table row for this suggested result set.

                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell phraseCell = new HtmlTableCell();
                    HtmlTableCell resultsCell = new HtmlTableCell();
                    row.Cells.Add(phraseCell);
                    row.Cells.Add(resultsCell);

                    // Add hyperlink for viewing the result set details.

                    HyperLink viewSetLink = new HyperLink();
                    viewSetLink.NavigateUrl = string.Format(ViewSuggestedResultSetUrlFormat, Server.HtmlEncode(suggestedResultSet.Id.ToString()));
                    viewSetLink.Text = Server.HtmlEncode(suggestedResultSet.Name);

                    phraseCell.Controls.Add(viewSetLink);

                    foreach (SuggestedResult suggestedResult in suggestedResultSet.SuggestedResults)
                    {
                        if (resultsCell.Controls.Count > 0)
                        {
                            Literal seperator = new Literal();
                            seperator.Text = ", ";

                            resultsCell.Controls.Add(seperator);
                        }

                        HyperLink suggestedResultLink = new HyperLink();
                        suggestedResultLink.NavigateUrl = suggestedResult.Url;
                        suggestedResultLink.Text = Server.HtmlEncode(suggestedResult.Title);
                        suggestedResultLink.Attributes.Add("onclick", OpenInNewWindowScript);

                        resultsCell.Controls.Add(suggestedResultLink);
                    }

                    tblSuggestedResultSets.Rows.Add(row);
                }
			}
			catch
			{
                Utilities.ShowError(messageHelper.GetMessage("msg search suggested results connection error"));
			}
		}
		#endregion
	}
	

using System;
using System.Web.UI;
using Ektron.Cms.Framework.Search;
using Ektron.Cms.Search;
using Ektron.Cms;

public partial class Workarea_controls_search_deletesuggestedresults : UserControl
{

    #region Sub: DeleteSuggestedResultsSets
    /// <summary>
    /// This subroutine accepts an integer indicating the ID for the term to be deleted as specified by the user, and then outputs the current Synonym Sets stored in the CMS to the workarea.  The Keywords associated with each Synonym Set will be truncated in the output so that only one line of terms will be displayed.  A "title" attribute provides the entire list of terms on mouseover.
    /// </summary>
    /// <remarks></remarks>
    private void DeleteSuggestedResultSets(Guid termID)
    {
        bool deleteSuccessful = false;
        try
        {
            ISuggestedResults suggestedResults = ObjectFactory.GetSuggestedResults();
            suggestedResults.Delete(termID);
            deleteSuccessful = true;
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

        if (deleteSuccessful)
        {
            Response.Redirect("suggestedresults.aspx?action=ViewSuggestedResults");
        }
    }
    #endregion

    #region Page Load
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Guid id;
        if (!string.IsNullOrEmpty(Request.QueryString["termID"]) && Guid.TryParse(Request.QueryString["termID"], out id))
        {            
            DeleteSuggestedResultSets(id);
        }
    }
    #endregion

}
	


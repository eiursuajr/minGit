using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms;

/// <summary>
/// Summary description for DistributionHelperMethods
/// </summary>
public class DistributionWizardHelperMethods
{
    private DistributionWizardHelperMethods()
    {
    }

    /// <summary>
    /// Parses the distribution mode provided in the query string and returns the corresponding
    /// enum representation.
    /// </summary>
    /// <param name="request">Request containing the query string</param>
    /// <returns>Mode specified on the querystring</returns>
    public static DistributionWizardEnumerations.Mode GetModeFromQueryString(HttpRequest request)
    {
        DistributionWizardEnumerations.Mode mode = DistributionWizardEnumerations.Mode.None;

        string modeParam = request.QueryString["Mode"];
        if (!String.IsNullOrEmpty(modeParam))
        {
            switch (modeParam.ToLower())
            {
                case DistributionWizardConstants.QUERY_MODE_COMMUNITYCOPY:
                    mode = DistributionWizardEnumerations.Mode.CommunityCopy;
                    break;
                case DistributionWizardConstants.QUERY_MODE_COMMUNITYREPLACE:
                    mode = DistributionWizardEnumerations.Mode.CommunityReplace;
                    break;
                case DistributionWizardConstants.QUERY_MODE_COMMUNITYREDISTRIBUTE:
                    mode = DistributionWizardEnumerations.Mode.CommunityRedistribute;
                    break;
                case DistributionWizardConstants.QUERY_MODE_SHAREPOINTCOPY:
                    mode = DistributionWizardEnumerations.Mode.Sharepoint;
                    break;
                case DistributionWizardConstants.QUERY_MODE_SHAREPOINTREDISTRIBUTE:
                    mode = DistributionWizardEnumerations.Mode.SharepointRedistribute;
                    break;
                default:
                    mode = DistributionWizardEnumerations.Mode.None;
                    break;
            }
        }

        return mode;
    }

    public static string GetQueryStringFromMode(DistributionWizardEnumerations.Mode mode)
    {
        string modeString = string.Empty;
        switch (mode)
        {
            case DistributionWizardEnumerations.Mode.CommunityCopy:
                modeString = DistributionWizardConstants.QUERY_MODE_COMMUNITYCOPY;
                break;
            case DistributionWizardEnumerations.Mode.CommunityRedistribute:
                modeString = DistributionWizardConstants.QUERY_MODE_COMMUNITYREDISTRIBUTE;
                break;
            case DistributionWizardEnumerations.Mode.CommunityReplace:
                modeString = DistributionWizardConstants.QUERY_MODE_COMMUNITYREPLACE;
                break;
            case DistributionWizardEnumerations.Mode.Sharepoint:
                modeString = DistributionWizardConstants.QUERY_MODE_SHAREPOINTCOPY;
                break;
            case DistributionWizardEnumerations.Mode.SharepointRedistribute:
                modeString = DistributionWizardConstants.QUERY_MODE_SHAREPOINTREDISTRIBUTE;
                break;
        }

        return modeString;
    }

    public static bool HasDistributionPrivileges(long folderID)
    {
        ContentAPI contentAPI = new ContentAPI();
        
        return contentAPI.EkContentRef.IsAllowed(folderID, 0, "folder", "Add", contentAPI.UserId) &&
            contentAPI.EkContentRef.IsAllowed(folderID, 0, "folder", "Delete", contentAPI.UserId) &&
            contentAPI.EkContentRef.IsAllowed(folderID, 0, "folder", "Restore", contentAPI.UserId);
    }
}

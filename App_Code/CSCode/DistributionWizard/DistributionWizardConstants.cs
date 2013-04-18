using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for DistributionWizardConstants
/// </summary>
public class DistributionWizardConstants
{
    public const string SESSION_PARAM_ORIG_CONTENT_ID = "DW_ORIGINAL_CONTENT_ID";
    public const string SESSION_PARAM_DEST_CONTENT_ID = "DW_DESTINATION_CONTENT_ID";
    public const string SESSION_PARAM_DEST_FOLDER_ID = "DW_DEST_FOLDER_ID";
    public const string SESSION_PARAM_METADATA = "DW_METADATA";
    public const string SESSION_PARAM_TAXONOMY = "DW_TAXONOMY";
    public const string SESSION_PARAM_SHAREPOINT_CONTENT_ID = "DW_SHAREPOINT_CONTENT_ID";
    public const string SESSION_PARAM_SHAREPOINT_GUID = "DW_SHAREPOINT_GUID";
    public const string SESSION_PARAM_SHAREPOINT_ITEMID = "DW_SHAREPOINT_ITEMID";

    public const string QUERY_MODE_COMMUNITYCOPY = "communitycopy";
    public const string QUERY_MODE_COMMUNITYREPLACE = "communityreplace";
    public const string QUERY_MODE_COMMUNITYREDISTRIBUTE = "communityredistribute";
    public const string QUERY_MODE_SHAREPOINTCOPY = "sharepointcopy";
    public const string QUERY_MODE_SHAREPOINTREDISTRIBUTE = "sharepointredistribute";
}

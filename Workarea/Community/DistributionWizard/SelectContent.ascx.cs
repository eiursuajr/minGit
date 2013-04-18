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

public partial class Community_DistributeCommunityDocuments_SelectContent : System.Web.UI.UserControl
{

    #region Members - Private
    private SiteAPI siteAPI = null;

    private string assetExtension = String.Empty;
    private int contentTypeID = -1;
    private int languageID = -1;
    private bool showRelatedContent = false;
    private bool checkEditPermissions = true;
    private bool isDistributionWizard = true;
    private bool enableMuiltiSelect = false;

    #endregion

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        siteAPI = new SiteAPI();

        // Initialize the folder selection control
        cmsSelectFolder.CallbackFunc = "selectFolder";
        cmsSelectFolder.SitePath = siteAPI.SitePath;
        cmsSelectFolder.FolderID = 0;

        // register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFileTypesCss);
    }

    /// <summary>
    /// This control requires ControlState to maintain the selected
    /// content ID across postbacks.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        // Enable control state to maintain selected content
        Page.RegisterRequiresControlState(this);    
        base.OnInit(e);
    }

    protected override void LoadControlState(object savedState)
    {
        // Load any pre-existing selected content
        if (savedState != null)
        {
            inputSelectedContentID.Value = savedState.ToString();
        }
    }

    protected override object SaveControlState()
    {
        // Maintain any pre-existing selected content across postbacks.
        return inputSelectedContentID.Value;
    }
    #endregion

    #region Properties - Public
    /// <summary>
    /// Returns the site's application path.
    /// </summary>
    public string SitePath
    {
        get
        {
            return siteAPI.SitePath;
        }
    }

    /// <summary>
    /// Returns the name of the hidden field for storing the selected content ID.
    /// This is used to populate a javascript variable, giving the client-side scripts
    /// the ability to populate the field.
    /// </summary>
    public string SelectedContentIDHiddenFieldName
    {
        get
        {
            return inputSelectedContentID.ClientID;
        }
    }

    /// <summary>
    /// Returns ID of the currently selected piece of content.
    /// </summary>
    public long SelectedContentID
    {
        get
        {
            long selectedContentID = -1;
            long.TryParse(inputSelectedContentID.Value, out selectedContentID);

            return selectedContentID;
        }
    }

    /// <summary>
    /// Gets or sets the asset extension on which content list data will be filtered.
    /// </summary>
    public string AssetExtension
    {
        get
        {
            return assetExtension;
        }

        set
        {
            assetExtension = value;
        }
    }

    /// <summary>
    /// Gets or sets the asset extension on which content list data will be filtered.
    /// </summary>
    public int ContentTypeID
    {
        get
        {
            return contentTypeID;
        }

        set
        {
            contentTypeID = value;
        }
    }

    /// <summary>
    /// Filters content if it exists in a distribution relationship.
    /// </summary>
    public bool ShowRelatedContent
    {
        get
        {
            return  showRelatedContent;
        }
        set
        {
            showRelatedContent = value;
        }
    }

    /// <summary>
    /// Content is only displayed if user has edit permissions for content.
    /// </summary>
    public bool CheckEditPermissions
    {
        get
        {
            return checkEditPermissions;
        }
        set
        {
            checkEditPermissions = value;
        }
    }

    /// <summary>
    /// Filters content by a specific language ID.
    /// </summary>
    public int LanguageID
    {
        get
        {
            return languageID;
        }
        set
        {
            languageID = value;
        }
    }

    public bool IsDistributionWizard
    {
        get
        {
            return isDistributionWizard;
        }
        set
        {
            isDistributionWizard = value;
        }
    }

    public bool EnableMultiSelect
    {
        get
        {
            return enableMuiltiSelect;
        }
        set
        {
            enableMuiltiSelect = value;
        }
    }
    #endregion
}
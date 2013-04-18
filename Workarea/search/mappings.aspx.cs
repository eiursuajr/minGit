using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Search;
using Ektron.Cms.Common;

public partial class Workarea_search_mappings : Page
{    
    private StyleHelper _style;
    private IIntegratedSearchMapping _integratedSearchMapping;    
    private List<SourceData> _contentSources;    

    /// <summary>
    /// Constructor
    /// </summary>
    public Workarea_search_mappings()
    {
        _integratedSearchMapping = ObjectFactory.GetIntegratedSearchMapping();
    }

    /// <summary>
    /// Gets a collection of existing integrated search mappings.
    /// </summary>
    private List<IntegratedSearchMappingData> Mappings
    {
        get
        {
            return _integratedSearchMapping.Get();
        }
    }

    /// <summary>
    /// Gets a collection of content sources available for mapping.
    /// </summary>
    private List<SourceData> ContentSources
    {
        get
        {
            if (_contentSources == null)
            {
                ContentSourceManager contentSourceManager = new ContentSourceManager();
                _contentSources = contentSourceManager.Get();
            }

            return _contentSources;
        }
    }

    /// <summary>
    /// Gets a collection of start addresses filtered to include
    /// only those that are available for mapping.
    /// </summary>
    private List<string> AvailableStartAddresses
    {
        get
        {            
            List<string> _availableStartAddresses = new List<string>();
            foreach (SourceData contentSource in ContentSources)
            {
                foreach (string startAddress in contentSource.StartAddresses)
                {                    
                    if (contentSource.Type == SourceDataType.File && !IsAlreadyMapped(startAddress))                        
                    {
                        _availableStartAddresses.Add(startAddress);
                    }
                }
            }            
            
            return _availableStartAddresses;
        }
    }

    /// <summary>
    /// Gets a reference to the message helper.
    /// </summary>
    protected EkMessageHelper MessageHelper
    {
        get
        {
            return SiteAPI.Current.EkMsgRef;
        }
    }


    /// <summary>
    /// Renders static display elements (title bar, etc.) on the page.
    /// </summary>    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HasPermission())
        {
            PrepareResources();
            RenderTitleBar();
        }
        else
        {
            Response.Redirect(ContentAPI.Current.ApplicationPath + "Login.aspx", true);
        }
    }

    /// <summary>
    /// Binds data to UI elements for display.
    /// </summary>    
    protected void Page_PreRender(object sender, EventArgs e)
    {
        BindData();

        divStartAddressWarning.Visible = ddlStartAddresses.Items.Count == 0;
        ddlStartAddresses.Enabled = ddlStartAddresses.Items.Count != 0;
        txtMapping.Enabled = ddlStartAddresses.Items.Count != 0;
        btnAdd.Visible = ddlStartAddresses.Items.Count != 0;
    }

    /// <summary>
    /// Loads and renders data to the page.
    /// </summary>
    private void BindData()
    {
        try
        {
            // Reset the mapping text.
            txtMapping.Text = string.Empty;

            // Load existing mappings from the database.
            rptMappings.DataSource = Mappings;
            rptMappings.DataBind();

            // Load the available start addresses into the drop down.

            ddlStartAddresses.Items.Clear();
            foreach (string startAddress in AvailableStartAddresses)
            {
                ddlStartAddresses.Items.Add(new ListItem(startAddress));
            }
        }
        catch
        {
            Utilities.ShowError(MessageHelper.GetMessage("lbl integrated search mappings error"));
        }
    }

    /// <summary>
    /// Registers scripts and styles with the page.
    /// </summary>
    private void PrepareResources()
    {
        _style = new StyleHelper();
        JS.RegisterJS(this, JS.ManagedScript.EktronStyleHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJFunctJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaHelperJS);
        JS.RegisterJS(this, JS.ManagedScript.EktronWorkareaContextMenusJS);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaCss);
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronWorkareaIeCss, Css.BrowserTarget.LessThanEqualToIE8);
        styleHelper.Text = _style.GetClientScript();
    }

    /// <summary>
    /// Renders the page's title bar.
    /// </summary>
    private void RenderTitleBar()
    {
        txtTitleBar.InnerHtml = _style.GetTitleBar(MessageHelper.GetMessage("lbl integrated search mappings"));
        tdIntegratedSearchHelpButton.InnerHtml = _style.GetHelpButton("AddIntegratedFolder", "");
    }

    /// <summary>
    /// Adds an integrated search mapping for the selected start address.
    /// </summary>
    protected void btnAdd_Click(object sender, ImageClickEventArgs e)
    {
        if (ValidateInput())
        {
            ResetErrorMessage();

            IntegratedSearchMappingData data = new IntegratedSearchMappingData();
            data.StartAddress = ddlStartAddresses.SelectedValue;
            data.VirtualDirectory = txtMapping.Text;

            _integratedSearchMapping.Add(data);
        }
        else
        {
            DisplayErrorMessage(MessageHelper.GetMessage("integrated search mappings invalid input"));
        }
    }

    /// <summary>
    /// Removes the integrated search mapping identified in the command argument.
    /// </summary>
    protected void btnRemove_Command(object sender, CommandEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.CommandName) && 
            e.CommandName == "Remove" && 
            e.CommandArgument != null)
        {
            Guid mappingId;
            if (Guid.TryParse(e.CommandArgument.ToString(), out mappingId))
            {
                _integratedSearchMapping.Delete(mappingId);
            }
        }
    }

    /// <summary>
    /// Validates the form input for processing.
    /// </summary>
    /// <returns>True if the form input is valid, false otherwise</returns>
    private bool ValidateInput()
    {        
        return ddlStartAddresses.SelectedIndex > -1 && !string.IsNullOrEmpty(txtMapping.Text);
    }

    /// <summary>
    /// Displays the specified error message on the page.
    /// </summary>
    /// <param name="message">Error message</param>
    private void DisplayErrorMessage(string message)
    {
        litErrorMessage.Text = HttpUtility.HtmlEncode(message);
        divErrorMessage.Visible = true;
    }

    /// <summary>
    /// Resets any errors messages currently displayed on the page.
    /// </summary>
    private void ResetErrorMessage()
    {
        litErrorMessage.Text = string.Empty;
        divErrorMessage.Visible = false;
    }

    /// <summary>
    /// Returns true if the specified start address has already been mapped.
    /// </summary>
    /// <param name="startAddress">Start address to verify</param>
    /// <returns>True if already mapped, false otherwise</returns>
    private bool IsAlreadyMapped(string startAddress)
    {
        return Mappings.Find(m => m.StartAddress.ToLower() == startAddress.ToLower()) != null;
    }

    /// <summary>
    /// Returns true if the specified start address represents an Ektron start address.
    /// </summary>
    /// <param name="startAddress">Start address to verify</param>
    /// <returns>True if Ektron start address, false otherwise</returns>
    private bool IsEktronStartAddress(string startAddress)
    {
        return !string.IsNullOrWhiteSpace(startAddress) && startAddress.ToLower().StartsWith("ektron://");
    }

    /// <summary>
    /// Returns true if the current user has sufficient permissions to access
    /// the functionality on this page, false otherwise.
    /// </summary>
    /// <returns>True if permissions are sufficient, false otherwise</returns>
    private static bool HasPermission()
    {
        return
            !((Convert.ToBoolean(ContentAPI.Current.RequestInformationRef.IsMembershipUser) ||
            ContentAPI.Current.RequestInformationRef.UserId == 0 ||
            !ContentAPI.Current.EkUserRef.IsAllowed(ContentAPI.Current.UserId, 0, "users", "IsAdmin", 0)) &&
            !(ContentAPI.Current.IsARoleMember((long)EkEnumeration.CmsRoleIds.SearchAdmin, ContentAPI.Current.UserId, false)));
    }
}
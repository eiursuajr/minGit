using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.API.Content;

public partial class Community_DistributionWizard_SelectTaxonomy : System.Web.UI.UserControl
{
    private ContentAPI contentAPI = null;
    private Taxonomy taxonomyAPI = null;
    private SiteAPI siteAPI = null;

    private IEnumerable<long> rootCategories = null;
    private int languageID = -1;

    /// <summary>
    /// List of root 
    /// </summary>
    public IEnumerable<long> RootCategories
    {
        get { return rootCategories; }
        set { rootCategories = value; }
    }
    
    public int LanguageID
    {
        get { return languageID; }
        set { languageID = value; }
    }

    public IEnumerable<long> SelectedTaxonomyCategoryIDs
    {
        get
        {
            string[] splitSelectedCategoryIDs = inputSelectedCategoryIDs.Value.Split(
                new string[] { " " },
                StringSplitOptions.RemoveEmptyEntries);

            List<long> selectedCategoryIDs = new List<long>();
            foreach (string categoryID in splitSelectedCategoryIDs)
            {
                selectedCategoryIDs.Add(long.Parse(categoryID));
            }

            return selectedCategoryIDs;
        }
        set 
        {
            string selectedCategoryIDs = string.Empty;
            foreach (long categoryID in value)
            {
                if (String.IsNullOrEmpty(selectedCategoryIDs))
                {
                    selectedCategoryIDs = categoryID.ToString();
                }
                else
                {
                    selectedCategoryIDs += " " + categoryID.ToString();
                }
            }

            inputSelectedCategoryIDs.Value = selectedCategoryIDs;
        }
    }

    protected override void  OnInit(EventArgs e)
    {
        Page.RegisterRequiresControlState(this);
 	    base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        contentAPI = new Ektron.Cms.ContentAPI();
        taxonomyAPI = new Taxonomy();
        siteAPI = new SiteAPI();

        AddScriptBlocksToHeader();
        FillTaxonomyTree();
    }

    protected override object SaveControlState()
    {        
        return inputSelectedCategoryIDs.Value;
    }

    protected override void LoadControlState(object savedState)
    {
        inputSelectedCategoryIDs.Value = (string)savedState;;
    }

    private void FillTaxonomyTree()
    {
        if (this.RootCategories != null)
        {
            List<long> rootCategoryIDs = new List<long>(this.RootCategories);

            StringBuilder sbTree = new StringBuilder();
            sbTree.Append("<div id=\"ekTaxonomy");
            sbTree.Append(this.ClientID);
            sbTree.Append("_0\">");
            foreach (long id in rootCategoryIDs)
            {
                sbTree.Append("<ul>");
                sbTree.Append(GetTaxonomyTree(this.ClientID, languageID, id));
                sbTree.Append("</ul>");
            }
            sbTree.Append("</div>");

            ltrTreeContainer.Text = sbTree.ToString();
        }
    }

    private void AddScriptBlocksToHeader()
    {
        SiteAPI siteAPI = new SiteAPI();

        HtmlGenericControl initializeAppPathScript = new HtmlGenericControl();
        initializeAppPathScript.TagName = "SCRIPT";
        initializeAppPathScript.Attributes.Add("type", "text/javascript");
        initializeAppPathScript.Attributes.Add("language", "javascript");
        initializeAppPathScript.InnerHtml = String.Format(
                "var taxonomyTreeWrapperID = \"{0}\";var selectedCategoriesFieldName = \"{1}\"",
                this.ClientID,
                this.inputSelectedCategoryIDs.ClientID);

        HtmlHead pageHeader = null;
        if (Page.Master != null)
        {
            pageHeader = Page.Master.Page.Header;
        }
        else
        {
            pageHeader = Page.Header;
        }

        if (pageHeader != null)
        {
            pageHeader.Controls.Add(initializeAppPathScript);
        }
    }

    private string GetTaxonomyTree(string controlId, int languageID, long taxonomyId)
    {        
        TaxonomyRequest taxonomyRequest = new TaxonomyRequest();
        taxonomyRequest.TaxonomyId = taxonomyId;
        taxonomyRequest.TaxonomyLanguage = languageID;

        TaxonomyData taxonomyData = taxonomyAPI.LoadTaxonomy(ref taxonomyRequest);

        StringBuilder sb = new StringBuilder();
        sb.Append("<li id=\"ekTaxonomy");
        sb.Append(controlId);
        sb.Append("_");
        sb.Append(taxonomyData.TaxonomyId.ToString());
        sb.Append("\">");
        if (taxonomyData.Taxonomy.Length > 0)
        {
            sb.Append("<a href=\"#\" onclick=\"toggleTree('");
            sb.Append(controlId);
            sb.Append("', ");
            sb.Append(taxonomyId.ToString());
            sb.Append(");\"><img id=\"ekIMG");
            sb.Append(controlId);
            sb.Append("_");
            sb.Append(taxonomyId.ToString());
            sb.Append("\" src=\"");
            sb.Append(siteAPI.SitePath);
            sb.Append("Workarea/images/ui/icons/tree/taxonomyCollapsed.png\" border=\"0\"></img></a>");
        }
        else
        {
            sb.Append("<img  src=\"");
            sb.Append(siteAPI.SitePath);
            sb.Append("Workarea/images/ui/icons/tree/taxonomy.png\"></img>");
        }
        sb.Append("<input type=\"checkbox\" id=\"");
        sb.Append("ekCheck");
        sb.Append(controlId);
        sb.Append("_");
        sb.Append(taxonomyId.ToString());
        sb.Append("\" onclick=\"selectCategory(this);\">");
        sb.Append(taxonomyData.TaxonomyName);

        if (taxonomyData.Taxonomy.Length > 0)
        {
            sb.Append("<div id=\"ekDiv");
            sb.Append(controlId);
            sb.Append("_");
            sb.Append(taxonomyId.ToString());
            sb.Append("\" style=\"display: none;\">");
            sb.Append("<ul>");
            foreach (TaxonomyData childTaxonomyData in taxonomyData.Taxonomy)
            {
                sb.Append(GetTaxonomyTree(controlId, languageID, childTaxonomyData.TaxonomyId));
            }
            sb.Append("</ul>");
            sb.Append("</div>");
        }

        sb.Append("</li>");

        return sb.ToString();
    }
}
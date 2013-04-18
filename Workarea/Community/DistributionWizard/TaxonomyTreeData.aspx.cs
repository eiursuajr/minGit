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

public partial class Community_DistributionWizard_TaxonomyTreeData : System.Web.UI.Page
{
    /// <summary>
    /// This page generates the data (with HTML markup) to populate the
    /// ajax taxomony tree control (SelectTaxonomy).
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string controlId = Request.QueryString["controlid"];
        long taxonomyId = long.Parse(Request.QueryString["taxonomyid"]);
        int languageId = int.Parse(Request.QueryString["languageid"]);

        string treeHtml = string.Empty;

        string rootCategoriesParam = Request.QueryString["rootcategories"];
        if (!String.IsNullOrEmpty(rootCategoriesParam))
        {
            string[] rootCategoriesSplit = rootCategoriesParam.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            List<long> rootCategories = new List<long>();
            foreach (string category in rootCategoriesSplit)
            {
                long categoryId = -1;
                long.TryParse(category.Trim(), out categoryId);

                if (categoryId > 0)
                {
                    rootCategories.Add(categoryId);
                }
            }

            treeHtml = GenerateRootTreeHtml(controlId, languageId, rootCategories);
        }
        else
        {
            treeHtml = GenerateTreeHtml(controlId, languageId, taxonomyId);
        }

        FolderTree.InnerHtml = treeHtml;
        FolderTree.ID = "ekDiv" + controlId + "_" + taxonomyId;
    }

    protected string GenerateRootTreeHtml(string controlId, int languageID, List<long> rootTaxonomyIds)
    {
        if (rootTaxonomyIds.Count == 0)
            return "";

        Taxonomy taxonomyAPI = new Taxonomy();
        TaxonomyRequest taxonomyRequest = new TaxonomyRequest();
        taxonomyRequest.TaxonomyId = 0;
        taxonomyRequest.TaxonomyLanguage = languageID;

        StringBuilder sb = new StringBuilder();
        sb.Append("<ul>");

        TaxonomyData taxonomyData = taxonomyAPI.LoadTaxonomy(ref taxonomyRequest);
        if (taxonomyData != null)
        {
            foreach (TaxonomyData childTaxonomyData in taxonomyData.Taxonomy)
            {
                if (rootTaxonomyIds.Contains(childTaxonomyData.TaxonomyId))
                {
                    sb.Append(GenerateCategoryHtml(controlId, languageID, childTaxonomyData.TaxonomyId));
                }
            }
        }

        sb.Append("</ul>");

        return sb.ToString();
    }

    protected string GenerateTreeHtml(string controlId, int languageID, long taxonomyId)
    {
        string taxonomyHtml;
        StringBuilder sb = new StringBuilder();
        ContentAPI contentAPI = new Ektron.Cms.ContentAPI();

        TaxonomyRequest taxonomyRequest = new TaxonomyRequest();
        taxonomyRequest.TaxonomyId = taxonomyId;
        taxonomyRequest.TaxonomyLanguage = languageID;

        Taxonomy taxonomyAPI = new Taxonomy();
        TaxonomyData taxonomyData = taxonomyAPI.LoadTaxonomy(ref taxonomyRequest);

        if (taxonomyData.Taxonomy.Length == 0)
            return "";

        sb.Append("<ul>");

        foreach (TaxonomyData childTaxonomyData in taxonomyData.Taxonomy)
        {
            taxonomyHtml = GenerateCategoryHtml(controlId, languageID, childTaxonomyData.TaxonomyId);
            sb.Append(taxonomyHtml);
        }
        sb.Append("</ul>");
        return sb.ToString();
    }

    protected string GenerateCategoryHtml(string controlId, int languageID, long taxonomyId)
    {
        StringBuilder sb = new StringBuilder();

        Taxonomy taxonomyAPI = new Taxonomy();
        TaxonomyRequest taxonomyRequest = new TaxonomyRequest();
        taxonomyRequest.TaxonomyId = taxonomyId;
        taxonomyRequest.TaxonomyLanguage = languageID;

        TaxonomyData taxonomyData = taxonomyAPI.LoadTaxonomy(ref taxonomyRequest);

        SiteAPI siteAPI = new SiteAPI();

        sb.Append("<li id=\"ekTaxonomy");
        sb.Append(controlId);
        sb.Append("_");
        sb.Append(taxonomyId.ToString());
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

        return sb.ToString();
    }
}
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
using Ektron.Cms.API;

public partial class widgets_TaxonomySummary_Preview : System.Web.UI.Page
{
    Ektron.Cms.CommonApi _commonAPI = new Ektron.Cms.CommonApi();
    
    public string SelectedThemes
    {
        get
        {
            string selThemes = "default";
            if (!string.IsNullOrEmpty(Request.QueryString["theme"]))
            {
                selThemes = Request.QueryString["theme"];
            }

            return selThemes;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData();
    }

    private void LoadData()
    {
        //add themes css
        Css.RegisterCss(this, _commonAPI.SitePath + "widgets/taxonomysummary/themes/" + SelectedThemes + "/style.css", "TSWidgetStyleCSS_" + SelectedThemes);       

    }
}

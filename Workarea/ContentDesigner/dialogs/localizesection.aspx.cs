//-----------------------------------------------------------------------
// <copyright file="localizesection.aspx.cs" company="Ektron" author="Rama Ila">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
// this page has ability to assign list of languages to explicitly limit by inclusion or exclusion to a locale.
//-----------------------------------------------------------------------
namespace Ektron.ContentDesigner.Dialogs
{
    using System.Collections.Generic;
    using System.Data;
    using System.Web.UI.WebControls;
    using System.Text;
    using Ektron.Cms;
    using Ektron.Cms.Localization;
    using Ektron.Cms.Workarea.Framework;
    using Ektron.Cms.Common;
    /// <summary>
    /// Adds Locale Ids to the selected text from content designer in workarea.
    /// </summary>  
    public partial class LocalizeSection : WorkareaDialogPage
    {
        /// <summary>
        /// style helper class creates bound fields for the grid view coloumns.
        /// </summary>  
        private StyleHelper styleHelper = new StyleHelper();

        /// <summary>
        /// Loclaization APi to get all the flag files for enabled locales from CMS.
        /// </summary> 
        private LocalizationAPI objLocalizationApi = new LocalizationAPI();

             
        /// <summary>
        /// register Workarea CSS,Dialog CSS  and Get all the Enabled locales from CMS.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, System.EventArgs e)
        {
           
            this.RegisterWorkareaCssLink();
            this.RegisterDialogCssLink();
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
            LocalizeToolBar();
            this.PopulateLocaleGrid();

        }

        /// <summary>
        /// Get all the Enabled locales from CMS.
        /// </summary> 
        private void PopulateLocaleGrid()
        {
            string applicationpath = new Ektron.Cms.CommonApi().ApplicationPath;
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronSmartFormJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, applicationpath + "java/ektron.workarea.js", "ektronWorkareaJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS");
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);
            EnabledLocaleList.Columns.Add(this.styleHelper.CreateBoundField("Include", this.GetMessage("lbl include locales header for content"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            EnabledLocaleList.Columns.Add(this.styleHelper.CreateBoundField("Exclude", this.GetMessage("lbl exclude locales header for content"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(5), Unit.Percentage(5), false, false));
            EnabledLocaleList.Columns.Add(this.styleHelper.CreateBoundField("Combined Name", this.GetMessage("lbl Name"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
            EnabledLocaleList.Columns.Add(this.styleHelper.CreateBoundField("Loc", this.GetMessage("lbl loc header for content"), "title-header", HorizontalAlign.Left, HorizontalAlign.NotSet, Unit.Percentage(30), Unit.Percentage(30), false, false));
            EnabledLocaleList.Columns.Add(this.styleHelper.CreateBoundField("Id", this.GetMessage("lbl id header of locale for content"), "title-header", HorizontalAlign.Right, HorizontalAlign.Right, Unit.Percentage(30), Unit.Percentage(30), false, false));
            DataTable localeDataTable = new DataTable();
            DataRow localeDataRow = default(DataRow);
            localeDataTable.Columns.Add(new DataColumn("Include", typeof(string)));
            localeDataTable.Columns.Add(new DataColumn("Exclude", typeof(string)));
            localeDataTable.Columns.Add(new DataColumn("Combined Name", typeof(string)));
            localeDataTable.Columns.Add(new DataColumn("Loc", typeof(string)));
            localeDataTable.Columns.Add(new DataColumn("Id", typeof(string)));
            Ektron.Cms.Framework.Localization.LocaleManager localeApi = new Ektron.Cms.Framework.Localization.LocaleManager();
            List<LocaleData> locales = localeApi.GetEnabledLocales();
            if (locales.Count > 0)
            {
                for (int b = 0; b < locales.Count; b++)
                {
                    string includeMessage = this.GetMessage("lbl include locales title for include checkbox of localization");
                    string excludeMessage = this.GetMessage("lbl exclude locales title for include checkbox of localization");
                    localeDataRow = localeDataTable.NewRow();
                    localeDataRow["Include"] = "<input type=\"checkbox\" title=\"" + includeMessage + "\" class=" + locales[b].Id + " name=\"include_items\" id=\"include_items" + locales[b].Id + "\" value=\"" + locales[b].Loc + "\" onclick=\"disableIncludeCheck('exclude_items" + locales[b].Id + "','include_items" + locales[b].Id + "');\">";
                    localeDataRow["Exclude"] = "<input type=\"checkbox\" title=\"" + excludeMessage + "\" class=" + locales[b].Id + " name=\"exclude_items\" id=\"exclude_items" + locales[b].Id + "\" value=\"" + locales[b].Loc + "\" onclick=\"disableIncludeCheck('include_items" + locales[b].Id + "','exclude_items" + locales[b].Id + "');\">";
                    localeDataRow["Combined Name"] = "<img title=" + locales[b].EnglishName + " alt=" + locales[b].EnglishName + " src='" + this.objLocalizationApi.GetFlagUrlByLanguageID(locales[b].Id) + "' />&nbsp;&nbsp;&nbsp;" + locales[b].CombinedName;
                    localeDataRow["Loc"] = locales[b].Loc;
                    localeDataRow["Id"] = locales[b].Id;
                    localeDataTable.Rows.Add(localeDataRow);
                }
            }

            DataView dataView = new DataView(localeDataTable);
            EnabledLocaleList.DataSource = dataView;
            EnabledLocaleList.DataBind();
        }


        private void LocalizeToolBar()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table><tbody><tr>");
            sb.Append("<td>").Append(this.GetMessage("lbl locale languages title text for content designer")).Append("<td>"); ;
            sb.Append("<td>");
            sb.Append(styleHelper.GetHelpButton("localizesection", ""));
            sb.Append("</td>");
            sb.Append("</tr></tbody></table>");

            ltrToolBar.Text += sb.ToString();
        }
    }
}

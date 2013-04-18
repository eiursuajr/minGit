using System;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Newtonsoft.Json;
using System.Collections.Generic;
using Ektron.Cms.Workarea.Framework;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for ResourceSelectorPopup.
    /// </summary>
	public partial class ResourceSelectorPopup : WorkareaDialogPage
    {
        private void Page_Load(object sender, EventArgs e)
        {
			this.RegisterWorkareaCssLink();
			this.RegisterDialogCssLink();
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
            //Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);

			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS"); 
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);

            //string webserviceURL = "../../../widgets/contentblock/CBHandler.ashx";
            // Register JS
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJsonJS);
            //Ektron.Cms.API.JS.RegisterJSInclude(this, "jquery.cluetip.js", "EktronJqueryCluetipJS");
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJQueryClueTipJS);
            Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronScrollToJS);
            //Ektron.Cms.API.JS.RegisterJSInclude(this, "../../../widgets/contentblock/behavior.js", "ContentWidgetBehaviorJS");

            //// Insert CSS Links
            Ektron.Cms.API.Css.RegisterCss(this, "CBStyle.css", "CBWidgetCSS"); //cbstyle will include the other req'd stylesheets

            //Ektron.Cms.API.JS.RegisterJSBlock(this, "Ektron.PFWidgets.ContentBlock.webserviceURL = \"" + webserviceURL + "\"; Ektron.PFWidgets.ContentBlock.setupAll('" + ClientID + "');", "EktronPFWidgetsCBInit");
            //Ektron.Cms.API.JS.RegisterJSBlock(this, "setupAll();", "EktronPFWidgetsCBInit");


            string selectorType = Request.QueryString["SelectorType"].ToString().ToLower();
            string idType = Request.QueryString["idType"].ToString().ToLower();
            switch (selectorType)
            {
                case "startingfolder":
                case "folder":
                    this.Title.Text = this.GetMessage("lbl select folder");
                    if ("startingfolder" == selectorType)
                    {
                        this.GroupOptionsDiv.Visible = true;
                        this.lblAllItems.InnerHtml = this.GetMessage("lbl all folders");
                        this.lblChildItem.InnerHtml = this.GetMessage("lbl include child folder");
                        this.lblCurrentItem.InnerHtml = this.GetMessage("lbl this folder only");
                    }
                    else
                    {
                        this.GroupOptionsDiv.Visible = false;
                    }
                    this.ContentSelectionDiv.Visible = false;
                    this.ItemPathDiv.Visible = false;
                    break;
                case "taxonomy":
                    this.Title.Text = this.GetMessage("lbl select taxonomy");
                    this.GroupOptionsDiv.Visible = false;
                    this.ContentSelectionDiv.Visible = false;
                    this.ItemPathDiv.Visible = true;
                    break;
                case "collection":
                    this.Title.Text = this.GetMessage("lbl select collection");
                    this.GroupOptionsDiv.Visible = false;
                    this.ContentSelectionDiv.Visible = true;
                    this.ItemPathDiv.Visible = false;
                    break;
                case "content":
                default:
                    this.Title.Text = this.GetMessage("lbl select content");
                    this.GroupOptionsDiv.Visible = false;
                    this.ContentSelectionDiv.Visible = true;
                    this.ItemPathDiv.Visible = false;
                    break;
            }
            this.RadTabStrip1.Tabs[0].Text = this.GetMessage("lbl folder");
            this.RadTabStrip1.Tabs[0].ToolTip = this.GetMessage("lbl folder");
            this.RadTabStrip1.Tabs[1].Text = this.GetMessage("lbl taxonomy");
            this.RadTabStrip1.Tabs[1].ToolTip = this.GetMessage("lbl taxonomy");
            this.RadTabStrip1.Tabs[2].Text = this.GetMessage("lbl search words");
            this.RadTabStrip1.Tabs[2].ToolTip = this.GetMessage("lbl search words");
            this.sWarnNoSelection.Text = this.GetMessage("warning no selection");
            this.sWarnMultiSelection.Text = this.GetMessage("warning multiple selection");
            this.sWarnNoResult.Text = this.GetMessage("generic no results found");
            this.lblSearchTerms.InnerHtml = this.GetMessage("lbl enter search terms");
            string sSearchLabel = this.GetMessage("generic search");
            this.linkSearch.InnerHtml = sSearchLabel;
            this.linkSearch.Title = sSearchLabel;
            string sViewResults = this.GetMessage("lbl view results");
            this.linkResult.InnerHtml = sViewResults;
            this.linkResult.Title = sViewResults;
            this.sTreeRoot.Text = this.GetMessage("lbl root");
            this.sChildren.Text = this.GetMessage("lbl children");
            this.sItems.Text = this.GetMessage("lbl items");
            string sCollectionItems = this.GetMessage("lbl collection items");
            this.sCollectionItems.Text = sCollectionItems;
            this.collectionItems.Text = sCollectionItems;
            string sTaxonomyItems = this.GetMessage("lbl taxonomy items");
            this.taxonomyItems.Text = sTaxonomyItems;
            this.lblCurrentPath.Text = this.GetMessage("lbl current path");

            //idType Locale strings
            this.sHtmlForm.Text = this.GetMessage("lbl html form");
            this.sHTMLContent.Text = this.GetMessage("lbl html content");
            this.sSmartForm.Text = this.GetMessage("lbl smart form");
            this.sImage.Text = this.GetMessage("generic image");
            this.sTaxonomy.Text = this.GetMessage("lbl taxonomy");
            this.sCollection.Text = this.GetMessage("lbl collection");
            this.sFolder.Text = this.GetMessage("lbl folder");
            this.sContent.Text = this.GetMessage("content text");
            this.sAsset.Text = this.GetMessage("lbl asset");
            this.sVideo.Text = this.GetMessage("lbl video");
            this.sProduct.Text = this.GetMessage("lbl commerce products");
            this.sMSOffice.Text = this.GetMessage("lbl microsoft asset");
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        ///        Required method for Designer support - do not modify
        ///        the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion

    }
}
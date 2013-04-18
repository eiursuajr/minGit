using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ektron.Cms.Workarea.Framework;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for ResourceSelectorField.
    /// </summary>
    public partial class ResourceSelectorField : WorkareaDialogPage
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
			this.RegisterWorkareaCssLink();
			this.RegisterDialogCssLink();
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronSmartFormCss);
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronSmartFormIe7Css, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS");
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);

            string strSelectContent = this.GetMessage("lbl select content");
            string strSelectFolder = this.GetMessage("lbl select folder");
            string strSelectTaxonomy = this.GetMessage("lbl select taxonomy");
            string strSelectCollection = this.GetMessage("lbl select collection");
            string strSelectResource = this.GetMessage("lbl select resource");
            string strDelete = this.GetMessage("generic delete title");
            string srcExplorerIcon = ResolveUrl(this.SkinControlsPath) + "ContentDesigner/btnexplorefolder.gif";
            string srcDeleteIcon = ResolveUrl(this.SkinControlsPath) + "Editor/Dialogs/cancelIcon.gif";
            string strFolder = this.GetMessage("lbl folder");
            this.linkSelectContentPopup.Title = strSelectResource;
            this.imgSelectContentPopup.Src = srcExplorerIcon;
            this.imgSelectContentPopup.Alt = strSelectResource;
            this.linkDeleteDefault.Title = strDelete;
            this.imgDeleteDefault.Src = srcDeleteIcon;
            this.imgDeleteDefault.Alt = strDelete;
            this.Title.Text = this.GetMessage("lbl resource selector field");
            this.RadTabStrip1.Tabs[0].Text = this.GetMessage("tab general");
            this.RadTabStrip1.Tabs[0].ToolTip = this.GetMessage("tab general") + " Tab";
            this.RadTabStrip1.Tabs[1].Text = this.GetMessage("generic type");
            this.RadTabStrip1.Tabs[1].ToolTip = this.GetMessage("generic type") + " Tab";
            this.RadTabStrip1.Tabs[2].Text = this.GetMessage("config page html title");
            this.RadTabStrip1.Tabs[2].ToolTip = this.GetMessage("config page html title") + " Tab";
            this.RadTabStrip1.Tabs[3].Text = this.GetMessage("lbl appearance");
            this.RadTabStrip1.Tabs[3].ToolTip = this.GetMessage("lbl appearance") + " Tab";
            this.lblResType.InnerHtml = this.GetMessage("lbl resource type");
            this.resContent.InnerHtml = this.GetMessage("lbl content resource");
            this.resFolder.InnerHtml = this.GetMessage("lbl folder resource");
            this.resTaxonomy.InnerHtml = this.GetMessage("lbl taxonomy resource");
            this.resCollection.InnerHtml = this.GetMessage("lbl collection resource");
            this.lblDefVal.InnerHtml = this.GetMessage("lbl default value c");
            this.lblAllow.InnerHtml = this.GetMessage("lbl allow");
            this.lblSearchBy.InnerHtml = this.GetMessage("lbl content author select content by");
            this.lblFolder.InnerHtml = this.GetMessage("lbl browsing folders"); 
            this.lblTaxonomy.InnerHtml = this.GetMessage("lbl browsing taxonomy categories"); 
            this.lblWords.InnerHtml = this.GetMessage("lbl search for key words"); 
            this.sFieldHeading.Text = this.GetMessage("lbl field heading");
            this.sInsertFieldHere.Text = this.GetMessage("lbl insert field here");
            this.sSelectContent.Text = strSelectContent;
            this.sCannotBeBlank.Text = this.GetMessage("lbl cannot be blank");
            this.sSelectOneOption.Text = this.GetMessage("alt select one option");
            this.lblFilterBy.InnerHtml = this.GetMessage("lbl filter by");
            string sContent = this.GetMessage("lbl html content");
            this.lblContent.InnerHtml = sContent;
            string sHtmlForm = this.GetMessage("lbl html form");
            this.lblHtmlForm.InnerHtml = sHtmlForm;
            string sSmartForm = this.GetMessage("lbl smart form");
            this.lblSmartForm.InnerHtml = sSmartForm;
            string sMSOffice = this.GetMessage("lbl microsoft asset");
            this.lblMSAsset.InnerHtml = sMSOffice;
            this.lblOtherAsset.InnerHtml = this.GetMessage("lbl other asset");
            this.lblDefFolder.InnerHtml = this.GetMessage("lbl starting folder");
            this.linkSelectFolderPopup.Title = strSelectFolder;
            this.imgSelectFolderPopup.Src = srcExplorerIcon;
            this.imgSelectFolderPopup.Alt = strSelectFolder;
            this.linkDeleteDefaultFolder.Title = strDelete;
            this.imgDeleteDefaultFolder.Src = srcDeleteIcon;
            this.imgDeleteDefaultFolder.Alt = strDelete;
            this.sSelectFolder.Text = strSelectFolder;
            string sVideo = this.GetMessage("lbl video");
            string sProduct = this.GetMessage("lbl commerce products");
            this.lblVideo.InnerHtml = sVideo;
            this.lblProduct.InnerHtml = sProduct;
            string sRoot = this.GetMessage("lbl root");
            this.txtFolderTitle.Value = sRoot;
            this.sTreeRoot.Text = sRoot;
            this.sChildren.Text = this.GetMessage("lbl children");
            this.sItems.Text = this.GetMessage("lbl items");
            this.lblSiteVisitorView.InnerHtml = this.GetMessage("lbl site appearance for resource");
            this.lblContentBlock.InnerHtml = this.GetMessage("lbl content block server control");
            this.lblContentTitle.InnerHtml = this.GetMessage("lbl content title");
            this.lblContentTitleHtml.InnerHtml = this.GetMessage("lbl content title with html");
            this.lblQuickLink.InnerHtml = this.GetMessage("lbl quicklink");
            this.lblQuickLinkSummary.InnerHtml = this.GetMessage("lbl quick link with summary");
            this.lblFolderListSummary.InnerHtml = this.GetMessage("lbl ListSummary of folder contents");
			this.lblFolderQuickLink.InnerHtml = this.GetMessage("lbl folder quicklink");
            string sSiteMap = this.GetMessage("lbl sitemap path");
            this.lblFolderBreadCrumb.InnerHtml = sSiteMap;
            this.lblTaxonomyDirectoryCtl.InnerHtml = this.GetMessage("lbl directory control");
            this.sSelectTaxonomy.Text = strSelectTaxonomy;
            this.sSelectCollection.Text = strSelectCollection;
            this.lblCollectionQuickLink.InnerHtml = this.GetMessage("lbl collection quicklink");
            this.lblCollectionQuickLinkSummary.InnerHtml = this.GetMessage("lbl collection quicklink and teaser");

            //major idType Locale strings 
            this.sHtmlForm.Text = sHtmlForm;
            this.sHTMLContent.Text = sContent;
            this.sSmartForm.Text = sSmartForm;
            this.sImage.Text = this.GetMessage("generic image");
            this.sFolder.Text = strFolder;
            this.sContent.Text = this.GetMessage("content text");
            this.sAsset.Text = this.GetMessage("lbl asset");
            this.sVideo.Text = sVideo;
            this.sProduct.Text = sProduct;
            this.sMSOffice.Text = sMSOffice;
            this.sTaxonomy.Text = this.GetMessage("lbl taxonomy");
            this.sCollection.Text = this.GetMessage("lbl collection");
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

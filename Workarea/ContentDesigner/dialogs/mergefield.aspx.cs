using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Telerik.WebControls;
using Ektron.Cms.Workarea.Framework;
using Ektron.Cms;
using Ektron.Cms.Common;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for MergeField.
    /// </summary>

	public partial class MergeField : WorkareaDialogPage
    {
        protected ContentAPI _ContentApi;
        protected EkMessageHelper _MessageHelper;

        private void Page_Load(object sender, System.EventArgs e)
        {
            _ContentApi = new ContentAPI();
            _MessageHelper = _ContentApi.EkMsgRef;
            if (EkFunctions.UrlDecode(Request.RawUrl).ToLower().Contains("<script") | EkFunctions.UrlDecode(Request.RawUrl).ToLower().Contains("</script"))
            {
                Utilities.ShowError(_MessageHelper.GetMessage("invalid querstring"));
                return;
            }
            this.RegisterWorkareaCssLink();
			this.RegisterDialogCssLink();
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronStringJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, Ektron.Cms.API.JS.ManagedScript.EktronXmlJS);
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekxbrowser.js", "ekxbrowserJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekutil.js", "ekutilJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../RadWindow.js", "RadWindowJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ekformfields.js", "ekformfieldsJS");
			Ektron.Cms.API.JS.RegisterJSInclude(this, "../ektron.dataListMgr.js", "EktronDataListMgrJS");

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow", "InitializeRadWindow();", true);

			this.Title.Text = this.GetMessage("lbl merge field");
			this.RadTabStrip1.Tabs[0].Text = this.GetMessage("tab general");
            this.RadTabStrip1.Tabs[0].ToolTip = this.GetMessage("tab general") + " Tab";
			this.RadTabStrip1.Tabs[1].Text = this.GetMessage("tab list style");
            this.RadTabStrip1.Tabs[1].ToolTip = this.GetMessage("tab list style") + "Tab";
			this.lblSelectField.InnerHtml = this.GetMessage("lbl select field");
			this.lblListStyle.InnerHtml = this.GetMessage("lbl list style");
			this.cboStyleList.Items[0].Text = this.GetMessage("lbl bul list");
            this.cboStyleList.Items[0].ToolTip = this.GetMessage("lbl bul list");
			this.cboStyleList.Items[1].Text = this.GetMessage("lbl num list");
            this.cboStyleList.Items[1].ToolTip = this.GetMessage("lbl num list");
			this.cboStyleList.Items[2].Text = this.GetMessage("lbl horz table");
            this.cboStyleList.Items[2].ToolTip = this.GetMessage("lbl horz table");
			this.cboStyleList.Items[3].Text = this.GetMessage("lbl vert table");
            this.cboStyleList.Items[3].ToolTip = this.GetMessage("lbl vert table");
			this.cboStyleList.Items[4].Text = this.GetMessage("lbl heading formatted");
            this.cboStyleList.Items[4].ToolTip = this.GetMessage("lbl heading formatted");
			this.cboStyleList.Items[5].Text = this.GetMessage("lbl delimited list");
            this.cboStyleList.Items[5].ToolTip = this.GetMessage("lbl delimited list");
			this.lblPreview.InnerHtml = this.GetMessage("lbl preview");
        }

        public void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            string treeNodes = e.Argument;
            if (treeNodes.Length > 0)
            {
                treeNodes = treeNodes.Replace("\r\n", "");
                treeNodes = treeNodes.Replace("\t", "");
                if ("<tree" == treeNodes.Substring(0, 5).ToLower().ToString()) // in case of AjaxManager fail to load xml
                {
                    fieldListTree.LoadXmlString(treeNodes);
                    fieldListTree.ExpandAllNodes();
                    fieldListTree.ToolTip = fieldListTree.DataValueField;
                }
                this.RadAjaxManager1.ResponseScripts.Add("PreFillDialog()");
            }
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
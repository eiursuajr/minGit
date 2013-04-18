using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
    /// Summary description for ucFieldName.
    /// </summary>
	public partial class ucFieldName : System.Web.UI.UserControl
    {
        private bool _tooltipenabled = true;
        private bool _indexedenabled = true;

        public bool ToolTipEnabled
        {
            get { return _tooltipenabled; }
            set { _tooltipenabled = value; }
        }
        public bool IndexedEnabled
        {
            get { return _indexedenabled; }
            set { _indexedenabled = value; }
        }
        protected void Page_Load(Object src, EventArgs e)
        {
			Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
			this.lblDescName.InnerHtml = refMsg.GetMessage("lbl desc name");
			this.lblIndexed.InnerHtml = refMsg.GetMessage("lbl indexed");
			this.lblFieldName.InnerHtml = refMsg.GetMessage("lbl field name");
			this.lblToolTip.InnerHtml = refMsg.GetMessage("lbl tool tip text");
			
			ToolTipArea.Visible = _tooltipenabled;
            IndexedArea.Visible = _indexedenabled;
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

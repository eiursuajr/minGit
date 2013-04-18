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
    /// Summary description for ucFieldMinMax.
    /// </summary>
    public partial class ucFieldMinMax : System.Web.UI.UserControl
    {
        protected void Page_Load(Object src, EventArgs e)
        {
			Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
            this.lblMinNum.InnerHtml = refMsg.GetMessage("lbl min num");
            this.lblMaxNum.InnerHtml = refMsg.GetMessage("lbl max num");
			
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

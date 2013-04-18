using System.Web;
using Ektron.Cms.Workarea.Framework;

public partial class CDcmsdatalist : WorkAreaBasePage
{
    #region  Web Form Designer Generated Code

    //This call is required by the Web Form Designer.
    [System.Diagnostics.DebuggerStepThrough()]
    private void InitializeComponent()
    {

    }

    private void Page_Init(System.Object sender, System.EventArgs e)
    {
        //CODEGEN: This method call is required by the Web Form Designer
        //Do not modify it using the code editor.
        InitializeComponent();
    }

    #endregion

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Ektron.Cms.Xslt.ArgumentList args = new Ektron.Cms.Xslt.ArgumentList();
        AssertInternalReferrer();
        CmsDataList.Visible = false;
        CmsDataListXml.Text = Ektron.Cms.EkXml.XSLTransform(CmsDataList.EkItem.Html, Server.MapPath("cmsdatalist.xslt"), true, false, args, true, null);
    }
}
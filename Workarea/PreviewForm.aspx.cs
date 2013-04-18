using Microsoft.VisualBasic;

public partial class PreviewForm : System.Web.UI.Page
	{
       public Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
            string strFilename = null;
            string strFormDesign = null;
            string strErr = "";

            strFilename = Request.QueryString["design"];
            if ((strFilename != null) & !string.IsNullOrEmpty(strFilename))
            {
                strFilename = Server.MapPath(api.AppPath + strFilename);
                
                strFormDesign = api.GetFileContents(strFilename, ref strErr);
                if (strErr.Length == 0)
                {
                    strFormDesign = Strings.Replace(strFormDesign, "[srcpath]", api.AppPath + api.AppeWebPath,1,-1,CompareMethod.Binary);
                    Response.Write(strFormDesign);
                }
                else
                {
                    Response.Write(strErr);
                }
            }
		}
	}
	
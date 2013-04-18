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
using Ektron.Cms.Controls;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
public partial class Workarea_captchaImage : System.Web.UI.Page
{
    protected Ektron.Cms.ContentAPI cApi;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EkCaptcha"] != null)
        {
            EkCaptcha cImg = new EkCaptcha(Session["EkCaptcha"].ToString(), 250, 45, "GenericSansSerif", Color.Black, Color.White);
            Response.Clear();
            Response.ContentType = "image/jpeg";
            cImg.Image.Save(Response.OutputStream, ImageFormat.Jpeg);
            cImg.Dispose();
            try
            {
                Concatenate(Session["EkCaptcha"].ToString().ToCharArray());
            }
            catch (Exception ex)
            {
                Ektron.Cms.EkException.WriteToEventLog(ex.Message + "[Ektron CMS400>>EkCaptcha]", System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }

    public void Concatenate(char[] inputChars)
    {
        cApi = new Ektron.Cms.ContentAPI();
        int bufSize = 1024 * 64;
        byte[] buf = new byte[bufSize];
        string OutputFileName = this.Server.MapPath(cApi.AppPath + "soundmanager/en-us/audiofile.mp3");
        if (File.Exists(OutputFileName))
            File.Delete(OutputFileName);
        using (FileStream outFile =
            new FileStream(OutputFileName, FileMode.CreateNew,
            FileAccess.Write, FileShare.None, bufSize))
        {
            foreach (char c in inputChars)
            {
                using (FileStream _file = new FileStream(this.Server.MapPath(cApi.AppPath + "soundmanager/en-us/" + c + ".mp3"), FileMode.Open, FileAccess.Read, FileShare.Read, bufSize))
                {
                    int br = 0;
                    while ((br = _file.Read(buf, 0, buf.Length)) > 0)
                    {
                        outFile.Write(buf, 0, br);
                    }
                }
            }
        }
    }
}

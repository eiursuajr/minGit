using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;

public partial class Workarea_Upload : System.Web.UI.Page
{
    protected Ektron.Cms.CommonApi api = new Ektron.Cms.CommonApi();
    protected Ektron.Cms.ContentAPI m_refContApi = new Ektron.Cms.ContentAPI();
    protected string sTarget = "ekavatarpath";
    protected string pageAction = string.Empty;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        //Put user code to initialize the page here
        uploadButton.Text = api.EkMsgRef.GetMessage("upload txt");
        cancelButtonText.Text = api.EkMsgRef.GetMessage("btn cancel");
        string qString = Request.QueryString["addedit"];

        //Register JavaScript and StyleSheet resources
        RegisterResources();
        SetServerJSVariables();
        if (!string.IsNullOrEmpty(Request.QueryString["action"]))
        {
            pageAction = Request.QueryString["action"];
        }
		// defect:63021 For uploading avatar you need not have user validation for membership users.
		//Utilities.ValidateUserLogin();
        //if (qString == null)
        //{
           // if (Convert.ToBoolean(api.RequestInformationRef.IsMembershipUser) || api.RequestInformationRef.UserId == 0)
           // {
             //   Response.Redirect(api.SitePath + "login.aspx", true);
            //    return;
           // }
        //}
        if (!string.IsNullOrEmpty(Request.QueryString["returntarget"])) // just say no to cross site scripting!
        {
            sTarget = (string)(Utilities.StripHTML(Request.QueryString["returntarget"]).Replace("\'", "\\\'"));
        }

        uploadButton.Attributes.Add("onclick", "return CheckUpload();");
    }

    public void UploadImage()
    {
        if (!(fileupload1.PostedFile == null)) //Check to make sure we actually have a file to upload
        {
            bool bSuccess = false;
            string strLongFilePath = (string)fileupload1.PostedFile.FileName;
            string[] aNameArray = Strings.Split((string)fileupload1.PostedFile.FileName, "\\", -1, 0);
            string strFileName = "";
            long userid = m_refContApi.UserId;
            if (aNameArray.Length > 0)
            {
                // fix for defect 32686 two users with the same image and same name for avatar.
                // If userid = 0 Then
                string strGUID = System.Guid.NewGuid().ToString();
                strFileName = (string)(strGUID.Substring(0, 5) + "_u_" + aNameArray[(aNameArray.Length - 1)]);
                //Else
                //   strFileName = userid & "_u_" & aNameArray((aNameArray.Length - 1))
                //End If
            }

            if ((fileupload1.PostedFile != null) && (!(Ektron.Cms.Common.EkFunctions.IsImage(System.IO.Path.GetExtension(fileupload1.PostedFile.FileName)))))
            {
                lbStatus.Text = api.EkMsgRef.GetMessage("lbl err avatar not valid extension");
                bSuccess = false;
                return;
            }

            string thumbnailPath = Server.MapPath(api.SitePath + "uploadedimages");
            if (((((string)fileupload1.PostedFile.ContentType == "image/pjpeg") || ((string)fileupload1.PostedFile.ContentType == "image/jpeg")) || ((string)fileupload1.PostedFile.ContentType == "image/gif")) || ((string)fileupload1.PostedFile.ContentType == "image/x-png") || ((string)fileupload1.PostedFile.ContentType == "image/png") || ((string)fileupload1.PostedFile.ContentType == "image/bmp"))//Make sure we are getting a valid JPG/gif image 
            {
                int numFileSize = System.Convert.ToInt32(System.Math.Ceiling(System.Convert.ToDecimal(fileupload1.PostedFile.ContentLength / 1024)));
                if (numFileSize > 200)
                {
                    lbStatus.Text = string.Format(api.EkMsgRef.GetMessage("lbl avatar filesize exceeded"), strFileName, numFileSize);
                    bSuccess = false;
                }
                else
                {
                    try
                    {
                        //We still need to expose old image upload process since the file has been referenced inside the workarea
                        //and use the new file stream one for MemberShip control upload process to avoid redundant images that may get uploaded by hackers or unknown visitors.
                        if (pageAction != "")
                        {
                            fileupload1.PostedFile.SaveAs(thumbnailPath + "\\" + strFileName);
                            Utilities.ProcessThumbnail(thumbnailPath, strFileName);
                        }
                        else
                        {
                            strFileName = Utilities.GetCorrectThumbnailFileWithExtn(strFileName);
                            //Changed uploadavatar design to avoid redundant images that may get uploaded by hackers. We save the image stream in
                            //Session and retrieve the stream on the server only on final submit.
                            System.Web.HttpContext.Current.Session[thumbnailPath + "\\" + strFileName] = fileupload1.PostedFile.InputStream;
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Redirect((string)("reterror.aspx?info=" + ex.Message.ToString()), false);
                    }

                    lbStatus.Text = string.Format(api.EkMsgRef.GetMessage("lbl success avatar uploaded"), strFileName, api.SitePath + "uploadedimages/" + strFileName);
                    bSuccess = true;
                }
            }
            else
            {
                //Not a valid jpeg/gif image
                lbStatus.Text = api.EkMsgRef.GetMessage("lbl err avatar not valid extension");
            }

            StringBuilder script = new StringBuilder();
            script.Append("<script language=\'javascript\'>").AppendLine();
            if (bSuccess) {
                script.Append("CheckUpHelper_ShowControls(false);").AppendLine();
                script.Append("self.parent.document.getElementById(\'" + sTarget + "\').value=\'" + api.SitePath + "uploadedimages/" + "thumb_" + Utilities.GetCorrectThumbnailFileWithExtn(strFileName) + "\';setTimeout(\'DialogClose()\',2000);").AppendLine();
            } else {
                script.Append("CheckUpHelper_ShowControls(true);").AppendLine();
            }
            script.Append("</script>").AppendLine();
            litScript.Text = script.ToString();
        }
    }

    protected void uploadButton_Click(object sender, System.EventArgs e)
    {
        UploadImage();
    }

    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
    }

    protected void SetServerJSVariables()
    {
        litErrSelAvatarUpload.Text = api.EkMsgRef.GetMessage("js err select avatar upload");
        litErrAvatarNotValidExtn.Text = api.EkMsgRef.GetMessage("lbl err avatar not valid extension");
    }
}



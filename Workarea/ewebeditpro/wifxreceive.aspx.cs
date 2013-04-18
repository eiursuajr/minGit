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
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.API.Search;
using Ektron.Cms;

public partial class wifxreceive : System.Web.UI.Page
{
    protected string g_LogicalRefDestination;
    protected Ektron.Cms.EkFileIO g_objUpload;
    protected object g_binaryFormData;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        // This code is available for modification.
        // Modify to follow the requirements of the server.
        g_objUpload = new Ektron.Cms.EkFileIO(); //CreateObject("eWepAutoSvr4.EkFile")
        BinaryReadPackage();

        // This is where the files will be seen from the web,
        // NOT the physical disk drive location.
        CreateVirtualDestination();

        //Recieve and save the files
        ReceiveSubmittedFiles();
    }
    ///''''''''''''''''''''''''''''''''''''''''''''''
    // This function will Read the Post Information and detect errors.
    private void BinaryReadPackage()
    {
        //On Error Resume Next VBConversions Warning: On Error Resume Next not supported in C#
        Information.Err().Clear();
        g_binaryFormData = Request.BinaryRead(Request.TotalBytes);
        if (Information.Err().Number != 0)
        {
            if (-2147467259 == Information.Err().Number)
            {
                string ErrDescription = "";
                ErrDescription = "Error: The file being upload is larger than what is allowed in the IIS. ";
                ErrDescription = ErrDescription + "Please change the ASPMaxRequestEntityAllowed to a larger number in the metabase.xml file (usually located in c:\\windows\\system32\\inetsrv). ";
                Response.Write(ErrDescription + '\r' + '\n' + "<br/>");
            }
            Response.Write(Information.Err().Description);
        }
    }

    ///''''''''''''''''''''''''''''''''''''''''''''''
    // This function will receive the files and send back
    // the required response data.  There is no processing
    // of the files and there is no affecting the file data.
    private void ReceiveSubmittedFiles()
    {
        //Dim fileObj
        object ErrorCode;
        //Dim iFileIdx As Integer
        string strNewFileName;

        ErrorCode = 0;
        strNewFileName = (string)(g_objUpload.EkFileSave(ref g_binaryFormData, "uploadfilephoto", Server.MapPath(g_LogicalRefDestination), ref ErrorCode, "makeunique", null, null, null)); // was

        Response.Write("<html><body><h1>Upload Received</h1><p>The file resides in:</p></p>" + g_LogicalRefDestination + "</p>" + "g_objUpload.ResponseData()" + "</body></html>");

    }

    ///''''''''''''''''''''''''''''''''''''''''''''''
    // This is where the files will be seen from the web,
    // NOT the physical disk drive location.
    private void CreateVirtualDestination()
    {
        string strCur;
        string[] strDirs;
        int iMax;
        int idx;
        object ErrorCode;

        ErrorCode = 0;
        g_LogicalRefDestination = (string)(g_objUpload.EkFormFieldValue(ref g_binaryFormData, (object)"editor_media_path", ref ErrorCode));
        if (g_LogicalRefDestination.Length == 0)
        {
            // A directory was not sent to us.
            strCur = Request.ServerVariables["URL"];
            strDirs = strCur.Split('/');
            iMax = strDirs.Length - 1;
            if (iMax > 0)
            {
                idx = 1;
                strCur = strDirs[0];
                while (idx < iMax)
                {
                    strCur = strCur + "/" + strDirs[idx];
                    idx++;
                }
                g_LogicalRefDestination = strCur + "/upload";
            }
            else
            {
                //Could not split the directory.
                g_LogicalRefDestination = "/webimagefx/upload";
            }
        }
    }
}
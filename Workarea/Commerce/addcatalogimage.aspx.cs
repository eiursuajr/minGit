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
using Ektron.Cms;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using System.IO;

public partial class Commerce_addcatalogimage : workareabase
{
    #region Variables
    protected int iboardid = 0;
    protected DiscussionBoard _board;
    protected bool bError = false;
    protected ProductType productTypeManager;
    protected ProductTypeData productType = new ProductTypeData();
    protected LibraryConfigData lib_settings_data = new Ektron.Cms.LibraryConfigData();
    #endregion
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
        {
            Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
        }
        lib_settings_data = this.m_refContentApi.GetLibrarySettings(iboardid);
        if (!Page.IsPostBack)
        {
            CheckAccess();
            productType = productTypeManager.GetItem(m_iID);

            lblTitle.InnerText = m_refMsg.GetMessage("generic title label");
            lblaltTitle.InnerText = m_refMsg.GetMessage("alt text");
            lblFullSize.InnerText = m_refMsg.GetMessage("lbl full size");

            if (productType.DefaultThumbnails.Count > 0)
            {
                ltr_pixel.Text = "<br /><table><tr><td>" + m_refMsg.GetMessage("lbl thumbnail spec") + "</td></tr>";
                foreach (ThumbnailDefaultData thumbnail in productType.DefaultThumbnails)
                {
                    ltr_pixel.Text += "<tr><td>&nbsp;&nbsp;" + thumbnail.Width.ToString() + " x " + thumbnail.Height.ToString() + " px</td></tr>";
                }
                ltr_pixel.Text += "</table>";
            }
            btnUpload.Attributes.Add("onclick", " return checkForEmptyTitleAndAlt(); return IsExtensionValid(); return checkntoggle(document.getElementById(\'dvHoldMessage\'),document.getElementById(\'dvErrorMessage\'));");
            btnUpload.Text = m_refMsg.GetMessage("upload txt");
        }
        GenerateJS();
    }
    private void CheckAccess()
    {
        bool loggedIn = m_refContentApi.LoadPermissions(0, "folder", 0).IsLoggedIn;
        if ((!loggedIn) || (loggedIn && (m_refContentApi.RequestInformationRef.IsMembershipUser == 1)))
        {
            dvPage.Visible = false;
            Response.Write("Please login as a cms user.");
            return;
        }
    }
    private void GenerateJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javaScript\" type=\"text/javascript\">").Append(Environment.NewLine);

        sbJS.Append("	function justtoggle(eElem, toshow){").Append(Environment.NewLine);
        sbJS.Append("	    if (toshow == true) {eElem.style.visibility = \"visible\";}").Append(Environment.NewLine);
        sbJS.Append("	    else {eElem.style.visibility=\"hidden\"; }").Append(Environment.NewLine);
        sbJS.Append("	}").Append(Environment.NewLine);

        sbJS.Append("	function checkntoggle(me, you){").Append(Environment.NewLine);
        sbJS.Append("       if (!checkForEmptyTitleAndAlt())").Append(Environment.NewLine);
        sbJS.Append("       {").Append(Environment.NewLine);
        sbJS.Append("           return false;").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
        sbJS.Append("		var bProceed = false; ").Append(Environment.NewLine);
        sbJS.Append("		var ofile = document.getElementById(\'" + ul_image.UniqueID + "\'); ").Append(Environment.NewLine);
        sbJS.Append("		if ( (ofile.type == \'file\') && (ofile.value != \'\') ) { ").Append(Environment.NewLine);
        sbJS.Append("		    bProceed = true; ").Append(Environment.NewLine);
        sbJS.Append("		} ").Append(Environment.NewLine);
        sbJS.Append("		if (bProceed){").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"visible\";").Append(Environment.NewLine);
        sbJS.Append("			you.style.visibility=\"hidden\";").Append(Environment.NewLine);
        sbJS.Append("			}").Append(Environment.NewLine);
        sbJS.Append("		else {").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"hidden\";").Append(Environment.NewLine);
        sbJS.Append("			you.style.visibility=\"visible\";").Append(Environment.NewLine);
        sbJS.Append("			alert(\'File not selected.\');").Append(Environment.NewLine);
        sbJS.Append("	    }").Append(Environment.NewLine);
        sbJS.Append("		return bProceed;").Append(Environment.NewLine);
        sbJS.Append("	}").Append(Environment.NewLine);

        sbJS.Append("   function checkForEmptyTitleAndAlt()").Append(Environment.NewLine);
        sbJS.Append("   {").Append(Environment.NewLine);
        sbJS.Append("       var title = document.getElementById(\'txtTitle\');").Append(Environment.NewLine);
        sbJS.Append("       var alt = document.getElementById(\'altTitle\');").Append(Environment.NewLine);
        sbJS.Append("       if(title.value == \'\' || alt.value == \'\')").Append(Environment.NewLine);
        sbJS.Append("       {").Append(Environment.NewLine);
        sbJS.Append("           alert(\'").Append(GetMessage("js alert title alt not empty")).Append("\');").Append(Environment.NewLine);
        sbJS.Append("           return false;").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
        sbJS.Append("       else").Append(Environment.NewLine);
        sbJS.Append("       {").Append(Environment.NewLine);
        sbJS.Append("           return true;").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
        sbJS.Append("   }").Append(Environment.NewLine);

        sbJS.Append("</script>").Append(Environment.NewLine);
        ltr_topjs.Text = sbJS.ToString();
        //Dim ltr_de_js As New LiteralControl(sbJS.ToString())
        //If Not (Page.Header Is Nothing) Then
        //    Page.Header.Controls.Add(ltr_de_js)
        //End If
    }

    protected void btnUpload_Click(object sender, System.EventArgs e)
    {
        string parentId = "";
        if ((Request.QueryString["catalogid"] != null) && Request.QueryString["catalogid"] != "")
        {
            parentId = Request.QueryString["catalogid"];
        }

        CheckAccess();
        try
        {
            if (!(ul_image.PostedFile == null))
            {
                productType = productTypeManager.GetItem(m_iID);

                int iFolder = iboardid;
                //lib_settings_data = Me.m_refContentApi.GetLibrarySettings(iFolder)

                // file was sent
                HttpPostedFile myFile = ul_image.PostedFile;
                string sFileExt = "";
                // Get and check size of uploaded file
                int nFileLen = myFile.ContentLength;
                //If nFileLen > _board.MaxFileSize Then
                //    Throw New Exception("File is too large. There is a " & _board.MaxFileSize.ToString() & " byte limit.")
                //End If
                //get and check name and extension
                string sFileName = myFile.FileName;
                string sShortName = "";
                if (myFile.FileName.IndexOf("\\") > -1)
                {
                    string[] aFilename = myFile.FileName.Split('\\');
                    // take the very last one
                    if (aFilename.Length > 0)
                    {
                        sFileName = aFilename[aFilename.Length - 1];
                    }
                }
                sFileName = sFileName.Replace(" ", "_").Replace("\'", ""); // make safe
                string[] aFileExt = sFileName.Split('.');
                if (aFileExt.Length > 1)
                {
                    sFileExt = (string)(aFileExt[(aFileExt.Length - 1)].Trim().ToLower()); //use the LAASSTT one.
                    if (sFileExt == "tif" || sFileExt == "bmp")
                    {
                        throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
                    }
                    sShortName = sFileName.Substring(0, System.Convert.ToInt32(sFileName.Length - (sFileExt.Length + 1)));
                }
                else
                {
                    throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
                }
                //aFileExt = Split(_board.AcceptedExtensions, ",")
                if (aFileExt.Length > 0)
                {
                    bool bGo = false;
                    for (int i = 0; i <= (aFileExt.Length - 1); i++)
                    {
                        if (sFileExt == aFileExt[i].Trim().ToLower())
                        {
                            bGo = true;
                            break;
                        }
                    }
                    if (bGo == false)
                    {
                        throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
                    }
                }
                else
                {
                    throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
                }

                // Allocate a buffer for reading of the file
                byte[] myData = new byte[nFileLen + 1];

                // Read uploaded file from the Stream
                myFile.InputStream.Read(myData, 0, nFileLen);

                //check for existence of file.
                FileInfo CheckFile;
                int iUnqueNameIdentifier = 0;
                string uploadImagePath = m_refContentApi.GetPathByFolderID(Convert.ToInt64(parentId));
                string sWebPath = lib_settings_data.ImageDirectory + uploadImagePath.Replace(" ", "_").Replace(".", "");
                string sPhysicalPath = Server.MapPath(sWebPath);
                CheckFile = new FileInfo(sPhysicalPath + "\\" + sFileName);
                if (CheckFile.Exists)
                {
                    while (CheckFile.Exists)
                    {
                        iUnqueNameIdentifier++;
                        sFileName = sShortName + "(" + iUnqueNameIdentifier + ")." + sFileExt;
                        CheckFile = new FileInfo(sPhysicalPath + sFileName);
                    }
                }

                //write
                WriteToFile(sPhysicalPath + "\\" + sFileName, myData);
                //----------------- Load Balance ------------------------------------------------------
                LoadBalanceData[] loadbalance_data;
                loadbalance_data = base.m_refContentApi.GetAllLoadBalancePathsExtn(iFolder, "files");
                if (!(loadbalance_data == null))
                {
                    for (int j = 0; j <= loadbalance_data.Length - 1; j++)
                    {
                        sPhysicalPath = Server.MapPath(loadbalance_data[j].Path);
                        if ((sPhysicalPath.Substring(sPhysicalPath.Length - 1, 1) != "\\"))
                        {
                            sPhysicalPath = sPhysicalPath + "\\";
                        }
                        WriteToFile(sPhysicalPath + "\\" + sFileName, myData);
                    }
                }

                //Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
                if (productType.DefaultThumbnails.Count > 0)
                {
                    foreach (ThumbnailDefaultData thumbnail in productType.DefaultThumbnails)
                    {
                        Utilities.ProcessThumbnail(sPhysicalPath, sFileName, thumbnail.Width, thumbnail.Height, thumbnail.Width);
                        // Utilities.ProcessThumbnail(sPhysicalPath, thumbnail.FileNameFormat.Replace("[filename]", sFileName), thumbnail.Width, thumbnail.Height, thumbnail.Width)
                    }
                }
                //Ends : Generate Thumbnails.

                //upload this file to library.
                LibraryData librarydata = new LibraryData();
                long libraryId = 0;
                librarydata.FileName = (string)((sWebPath.Replace("/\\", "\\") + "\\" + sFileName).Replace(" ", "_"));
                librarydata.Type = "images";
                if (txtTitle.Value == "")
                {
                    librarydata.Title = sShortName;
                }
                else
                {
                    librarydata.Title = (string)txtTitle.Value;
                }
                librarydata.ParentId = Convert.ToInt64(parentId);
                libraryId = m_refContentApi.AddLibraryItem(ref librarydata);

                LibraryData retLibraryData = m_refContentApi.GetLibraryItemByID(libraryId, Convert.ToInt64(parentId));
                //Uploading image to libray ends.
                System.Drawing.Bitmap imageUpload;
                imageUpload = new System.Drawing.Bitmap(sPhysicalPath + "\\" + sFileName);
                // Add media image
                ltrAddMediaJS.Text += "var newImageObj = {";
                ltrAddMediaJS.Text += "        id: \"" + libraryId.ToString() + "\", " + Environment.NewLine;
                ltrAddMediaJS.Text += "        title: \"" + retLibraryData.Title + "\", " + Environment.NewLine;
                ltrAddMediaJS.Text += "        altText: \"" + altTitle.Value + "\", " + Environment.NewLine;
                ltrAddMediaJS.Text += "        path: \"" + retLibraryData.FileName + "\", " + Environment.NewLine;
                ltrAddMediaJS.Text += "        width:" + imageUpload.Width + "," + Environment.NewLine;
                ltrAddMediaJS.Text += "        height:" + imageUpload.Height + "," + Environment.NewLine;
                ltrAddMediaJS.Text += "        Thumbnails: [" + Environment.NewLine;

                int iThumbnail = 0;

                for (iThumbnail = 0; iThumbnail <= productType.DefaultThumbnails.Count - 1; iThumbnail++)
                {
                    int indexThumbnail = retLibraryData.FileName.LastIndexOf("/");
                    string thumbnailpath = retLibraryData.FileName.Substring(0, indexThumbnail + 1);
                    ltrAddMediaJS.Text += "              {path: \"" + thumbnailpath + "thumb" + productType.DefaultThumbnails[iThumbnail].Width + "_" + sFileName + "\"}," + Environment.NewLine;
                }
                ltrAddMediaJS.Text += "        ] " + Environment.NewLine;
                ltrAddMediaJS.Text += "  }; " + Environment.NewLine;

                ltrAddMediaJS.Text += "parent.Ektron.Commerce.MediaTab.Images.addNewImage(newImageObj);";

            }
            else
            {
                throw (new Exception("No File"));
            }
        }
        catch (Exception ex)
        {
            ltr_error.Text = ex.Message;
            ltr_bottomjs.Text += "	justtoggle(document.getElementById(\'dvErrorMessage\'), true);" + Environment.NewLine;
            bError = true;
        }
    }
    private void WriteToFile(string strPath, byte[] Buffer)
    {
        try
        {
            // Create a file
            FileStream newFile = new FileStream(strPath, FileMode.Create);

            // Write data to the file
            newFile.Write(Buffer, 0, Buffer.Length);

            // Close file
            newFile.Close();
        }
        catch (Exception)
        {
            Utilities.ShowError((string)("Unable to write file " + strPath));
        }
    }
    private string MakeJSSafe(string JS)
    {
        JS = JS.Replace("\'", "\\\'");
        JS = JS.Replace(Environment.NewLine, "\\n");
        return JS;
    }
}


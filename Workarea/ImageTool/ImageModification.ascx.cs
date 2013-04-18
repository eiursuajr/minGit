using System;
using System.Data;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Ektron.Cms;
using Ektron.Cms.Workarea.Framework;
using Ektron.Cms.ImageTool;
using Ektron.Cms.Common;

public partial class Widgets_Dialogs_ImageModification : System.Web.UI.UserControl
{
    #region Member Variables  ======================================================

    protected const string mc_nameCommandFieldIn = "_command";
    protected const string mc_nameCallerIdIn = "_callerId";
    protected const string mc_dialogUrlFieldIn = "_dialogUrl";
    protected const string mc_callbackFieldIn = "_cbFunction";

    protected const string mc_imageSelectParamIn = "srcimg";
    //protected const string mc_makeUniqueParamIn = "MakeUnique";
    protected const string mc_resultingImageOut = "image";
    protected const string mc_resultingWidthOut = "width";
    protected const string mc_resultingHeightOut = "height";

    protected const int mc_iResizeModifier = 16;
    protected const int mc_iBorderEditMargin = 30;
    protected const int mc_iMinIFrameWidth = 64;
    protected const int mc_iMinIFrameHeight = 230;
    // This is cancatinated to the site path, so it does not need to be relative.
    protected const string mc_strImageScorePage = "/ImageTool/ImageScore.aspx";

    private string m_strCommand = "";
    private string m_sourceImagePhysical = "";
    private string m_sourceImageURL = "";
    private int m_iResizeWidth = 0;
    private int m_iResizeHeight = 0;
    private Ektron.Cms.CommonApi m_apiCommon = new Ektron.Cms.CommonApi();
    private Ektron.Cms.Common.EkMessageHelper m_refMsg = null;

    // Dialog specific
    private string idCaller = "";

    private string m_strErrorMessage = "";

    // Localization Safety
    System.Globalization.CultureInfo m_culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

    #endregion  // Member Variables  ...............................................


    #region Creation and Initialization  ===========================================

    protected Widgets_Dialogs_ImageModification()
    {
        ;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResources();
        // TODO: Ross - Not sure if this is the best place to put them
        string imagesPath = "images/"; // this.SkinControlsPath + "ImageTool/";
        btnCropImage.ImageUrl = imagesPath + "cropImage.gif";
        btnResizeImage.ImageUrl = imagesPath + "fileResize.gif";
        btnTextImage.ImageUrl = imagesPath + "addText.gif";
        btnRotateImage.ImageUrl = imagesPath + "rotateImage.gif";
        btnBrightImage.ImageUrl = imagesPath + "brightnessChange.gif";
        btnUndoImage.ImageUrl = imagesPath + "undo.gif";
        btnRedoImage.ImageUrl = imagesPath + "redo.gif";

        // no support for undo/redo in 7.x..only 8.0
        btnUndoImage.Visible = false;
        btnRedoImage.Visible = false;

        // set captions
        m_refMsg = m_apiCommon.EkMsgRef;
        btnSave.Alt = m_refMsg.GetMessage("alt imagetool save");
        btnCancel.ToolTip = m_refMsg.GetMessage("alt imagetool cancel");
        btnCropImage.ToolTip = m_refMsg.GetMessage("alt imagetool crop");
        btnResizeImage.ToolTip = m_refMsg.GetMessage("alt imagetool resize");
        btnTextImage.ToolTip = m_refMsg.GetMessage("alt imagetool text");
        btnRotateImage.ToolTip = m_refMsg.GetMessage("alt imagetool rotate");
        btnBrightImage.ToolTip = m_refMsg.GetMessage("alt imagetool brightness");
        btnUndoImage.ToolTip = m_refMsg.GetMessage("alt imagetool undo");
        btnRedoImage.ToolTip = m_refMsg.GetMessage("alt imagetool redo");

        // set resize labels
        lblImageDimensions.Text = m_refMsg.GetMessage("lbl imagetool dimensions");
        lblImageDimensions.ToolTip = lblImageDimensions.Text;
        lblImageFileSize.Text = m_refMsg.GetMessage("lbl imagetool filesize");
        lblImageFileSize.ToolTip = lblImageFileSize.Text;
        lblImageResizeWidth.Text = m_refMsg.GetMessage("lbl imagetool resize width");
        lblImageResizeWidth.ToolTip = lblImageResizeWidth.Text;
        lblImageResizeHeight.Text = m_refMsg.GetMessage("lbl imagetool resize height");
        lblImageResizeHeight.ToolTip = lblImageResizeHeight.Text;
        lblImageResizeAspect.Text = m_refMsg.GetMessage("lbl imagetool resize aspect");
        lblImageResizeAspect.ToolTip = lblImageResizeAspect.Text;
        btnDoImageResize.Text = m_refMsg.GetMessage("lbl btn imagetool resize");
        btnDoImageResize.ToolTip = btnDoImageResize.Text;


        // set crop labels
        litCropHelpTitle.Text = m_refMsg.GetMessage("lit imagetool crop help title");
        litCropHelpDescription.Text = m_refMsg.GetMessage("lit imagetool crop help description");
        btnDoImageCrop.Value = m_refMsg.GetMessage("lbl btn imagetool crop");


        // set text labels
        litTextHelpTitle.Text = m_refMsg.GetMessage("lit imagetool text help title");
        litTextHelpDescription.Text = m_refMsg.GetMessage("lit imagetool text help description");
        btnDoImageText.Value = m_refMsg.GetMessage("lbl btn imagetool text");

        // set rotate labels
        lblRotateHelp.Text = m_refMsg.GetMessage("lbl imagetool rotate help");
        btnFinishImageRotate.Value = m_refMsg.GetMessage("lbl btn imagetool rotate");

        // set brightness labels
        lblBrightnessHelp.Text = m_refMsg.GetMessage("lbl imagetool brightness help");
        btnFinishImageBrightness.Value = m_refMsg.GetMessage("lbl btn imagetool brightness");

    }

    /*
    protected override void OnWidgetInitialize()
    {
        SetDataFromParameters();
    }
     */

    private void SetDataFromParameters()
    {
        string strData;

        // Since we probably will be changing the name of the image file
        // we need to use the value in the field in place of the parameter.
        m_sourceImageURL = hdnSrcFileURL.Value.ToString().Trim();
        if (m_sourceImageURL.Length == 0)
        {
            m_sourceImageURL = GetDialogWidgetParameter(mc_imageSelectParamIn);
            hdnSrcFileURL.Value = m_sourceImageURL;
        }
        if (m_sourceImageURL.Length > 0)
            m_sourceImagePhysical = Page.MapPath(RemoveDomainName(m_sourceImageURL));
        else
            m_sourceImagePhysical = "";

        strData = GetDialogWidgetParameter(mc_nameCallerIdIn);
        if (strData.Length > 0)
        {
            hdnCallerId.Value = strData.Trim();
        }
        idCaller = hdnCallerId.Value;

    }

    #endregion  // Creation and Initialization  ....................................


    #region Information and Configuration Methods  =================================

    /// <summary>
    /// Determines the path of the image scoring aspx file.
    /// It has to be in the workarea somewhere, in the site.
    /// </summary>
    /// <returns></returns>
    private string ImageScoreActiveURL()
    {
        return CatPath(m_apiCommon.AppPath, mc_strImageScorePage, false);
    }

    #endregion  // Information and Configuration Methods  ..........................


    #region Image Edit Area Assembly  ==============================================

    private void FormatCurrentWidgetUI()
    {
        //ImageSizeInfoArea.Visible = (ImageData.ImageCommand.Resize.ToString() == hdnCurrentCommand.Value.ToString());
        ResizeInfoArea.Visible = (ImageData.ImageCommand.Resize.ToString() == hdnCurrentCommand.Value.ToString());
        CropInfoArea.Visible = (ImageData.ImageCommand.Crop.ToString() == hdnCurrentCommand.Value.ToString());
        TextInfoArea.Visible = (ImageData.ImageCommand.Text.ToString() == hdnCurrentCommand.Value.ToString());
        RotateInfoArea.Visible = (ImageData.ImageCommand.Rotate.ToString() == hdnCurrentCommand.Value.ToString());
        BrightnessInfoArea.Visible = (ImageData.ImageCommand.Brightness.ToString() == hdnCurrentCommand.Value.ToString());
    }

    public string DrawImageEditArea()
    {
        try
        {
            string strImgDisp = "";
            m_strCommand = hdnCurrentCommand.Value.ToString();
            strImgDisp = DisplayImageAction();
            return strImgDisp;
        }
        catch
        {
        }
        return "";
    }

    private void FillImageInformation()
    {
        // Should we show the resizer.
        if ((m_sourceImageURL.Length > 0) && (ImageData.ImageCommand.Resize.ToString() == hdnCurrentCommand.Value.ToString()))
        {
            //ImageSizeInfoArea.Visible = true;

            ImageTool objImage = new ImageTool(m_sourceImagePhysical);
            if (objImage.FileSize > 4096)
            {
                lblImageFileSizeInKs.Text = Convert.ToString(objImage.FileSize / 1024);
                lblImageFileSizeInKs.ToolTip = lblImageFileSizeInKs.Text;
                lblImageSizeUnit.Text = "K";
            }
            else
            {
                lblImageFileSizeInKs.Text = Convert.ToString(objImage.FileSize);
                lblImageFileSizeInKs.ToolTip = lblImageFileSizeInKs.Text;
                lblImageSizeUnit.Text = "bytes";
            }

            txtImageResizeWidth.Text = objImage.Width.ToString();
            txtImageResizeWidth.ToolTip = txtImageResizeWidth.Text;
            txtImageResizeHeight.Text = objImage.Height.ToString();
            txtImageResizeHeight.ToolTip = txtImageResizeHeight.Text;
            txtMaxImageHeight.Text = objImage.Height.ToString();

            // We need to save the original settings for aspect calculations.
            hdnImageResizeOrigWidth.Value = objImage.Width.ToString();
            hdnImageResizeOrigHeight.Value = objImage.Height.ToString();
        }
        else
        {
            //ImageSizeInfoArea.Visible = false;
        }
    }

    private string DisplayImageAction()
    {
        int iResize = mc_iResizeModifier;
        string strPageURL = ImageScoreActiveURL();
        int iFrameWidth = 1 + iResize;
        int iFrameHeight = 1 + iResize;

        ImageTool objImage = null;
        if (m_sourceImageURL.Length > 0)
        {
            objImage = new ImageTool(m_sourceImagePhysical);

            //<iframe src="/WorkAreaR2/WorkArea/Foundation/Widgets/Dialogs/ImageScore.aspx?s=/400WorkareaR2/image/ACat1.jpg" 
            //        id="imageaffect" width="80%" height="80%" marginheight="0%" marginwidth="0%"></iframe>
            strPageURL += "?s=" + EkFunctions.UrlEncode(m_sourceImageURL);
            if (m_strCommand.Length > 0)
                strPageURL += "&c=" + EkFunctions.UrlEncode(m_strCommand);

            iFrameWidth = objImage.Width + iResize;
            iFrameHeight = objImage.Height + iResize;

            if (iFrameWidth < mc_iMinIFrameWidth)
                iFrameWidth = mc_iMinIFrameWidth;
            if (iFrameHeight < mc_iMinIFrameHeight)
                iFrameHeight = mc_iMinIFrameHeight;
        }

        int iAreaHeight = iFrameHeight + mc_iBorderEditMargin;
        int iAreaWidth = iFrameWidth + mc_iBorderEditMargin;
        string strIframe = "";
        EkImgToolEditAreaDisplay.Style.Add("width", iAreaWidth + "px");

        if (IsFileAccessibleForEdit(m_sourceImagePhysical))
        {
            strIframe += "<div style=\"text-align: center; background-color: transparent; \" height=\"100%\" width=\"" + iAreaWidth + "px\"><br />";
            //strIframe += "Image location: " + m_sourceImageURL;
            strIframe += "<iframe src=\"" + strPageURL + "\" id=\"imageaffect\" ";
            //strIframe += "width=\"" + iFrameWidth + "\" height=\"" + iFrameHeight + "\" ";
            strIframe += "width=\"" + iAreaWidth + "px\" height=\"100%\" ";
            if (objImage != null)
            {
                int maxdim = (objImage.Height > objImage.Width) ? objImage.Height : objImage.Width;
                if (hdnCurrentCommand.Value != ImageData.ImageCommand.Rotate.ToString())
                {
                    // only need to set max height/width if we're rotating
                    maxdim = objImage.Height;
                }
                strIframe += "onload=\"setIframeHeight('imageaffect', " + (maxdim + 50) + ", " + iAreaWidth + ")\" ";
            }
            //strIframe += "scrolling=\"yes\" ";
            strIframe += "frameborder=\"0\" marginheight=\"0%\" marginwidth=\"0%\"></iframe>";
            strIframe += "</div>";
        }
        else
        {
            strIframe += "<hr /><div style=' font-weight:bold; text-align:center '>";
            if (m_strErrorMessage.Length > 0)
            {
                strIframe += m_strErrorMessage;
            }
            else
            {
                strIframe += "This image can't be edited.";
            }
            strIframe += "</div><hr /><br />";
        }

        return strIframe;
    }

    #endregion  // Image Edit Area Assembly  .......................................


    #region Button Action Methods  =================================================

    private void highlightButton(ImageButton btn, bool selected)
    {
        if (selected)
        {
            btn.BorderStyle = BorderStyle.Solid;
            btn.BorderWidth = 1;
        }
        else
        {
            btn.BorderStyle = BorderStyle.None;
            btn.BorderWidth = 0;
        }
    }
    private void unhighlightButtons()
    {
        highlightButton(btnCropImage, false);
        highlightButton(btnResizeImage, false);
        highlightButton(btnTextImage, false);
        highlightButton(btnRotateImage, false);
        highlightButton(btnBrightImage, false);
        highlightButton(btnUndoImage, false);
        highlightButton(btnRedoImage, false);
    }
    private void highlightButton(ImageButton btn)
    {
        unhighlightButtons();
        highlightButton(btn, true);
    }

    protected void btnUndoImage_Click(object sender, EventArgs e)
    {
        lblCurrentAction.Text = "";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Undo.ToString();  // "undo";
        SetDataFromParameters();
        FormatCurrentWidgetUI();
    }

    protected void btnRedoImage_Click(object sender, EventArgs e)
    {
        lblCurrentAction.Text = "";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Redo.ToString();  // "redo";
        SetDataFromParameters();
        FormatCurrentWidgetUI();
    }

    protected void btnCropImage_Click(object sender, EventArgs e)
    {
        //lblCurrentAction.Text = "Cropping";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Crop.ToString();  // "crop";
        SetDataFromParameters();
        FormatCurrentWidgetUI();
        highlightButton(btnCropImage);
    }

    protected void btnResizeImage_Click(object sender, EventArgs e)
    {
        //lblCurrentAction.Text = "Resizing";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Resize.ToString();  // "resize";
        SetDataFromParameters();
        FormatCurrentWidgetUI();
        FillImageInformation();
        highlightButton(btnResizeImage);
    }

    protected void btnTextImage_Click(object sender, EventArgs e)
    {
        //lblCurrentAction.Text = "Adding Text";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Text.ToString();  // "text";
        SetDataFromParameters();
        FormatCurrentWidgetUI();
        highlightButton(btnTextImage);
    }

    protected void btnRotateImage_Click(object sender, EventArgs e)
    {
        //lblCurrentAction.Text = "Rotating";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Rotate.ToString();  // "rotate";
        SetDataFromParameters();
        FormatCurrentWidgetUI();
        highlightButton(btnRotateImage);
    }

    protected void btnBrightImage_Click(object sender, EventArgs e)
    {
        //lblCurrentAction.Text = "Brightness";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Brightness.ToString();  // "bright";
        SetDataFromParameters();
        FormatCurrentWidgetUI();
        highlightButton(btnBrightImage);
    }

    protected void btnDoImageResize_Click(object sender, EventArgs e)
    {
        //lblCurrentAction.Text = "Resizing";
        hdnCurrentCommand.Value = ImageData.ImageCommand.Resize.ToString();  // "resize";
        SetDataFromParameters();

        if (SetResizeRequestValues())
        {
            ResizeImageToRequest();
            FillImageInformation();
            highlightButton(btnResizeImage);
        }
    }


    #endregion  // Button Action Methods  ..........................................


    #region Private Tools  =========================================================

    private void ResizeImageToRequest()
    {
        if (m_sourceImagePhysical.Length > 0)
        {
            ImageData objImage = new ImageData(m_sourceImagePhysical);
            // We have the aspect parameter always false, because we have taken care of this.
            objImage.Resize(m_iResizeWidth, m_iResizeHeight, false);
            objImage.Commit();
        }
    }

    private bool SetResizeRequestValues()
    {
        // The original values are used in the aspect calculations.
        int iOrigWidth = Convert.ToInt32(hdnImageResizeOrigWidth.Value);
        int iOrigHeight = Convert.ToInt32(hdnImageResizeOrigHeight.Value);

        try
        {
            m_iResizeWidth = Convert.ToInt32(txtImageResizeWidth.Text.ToString());
            m_iResizeHeight = Convert.ToInt32(txtImageResizeHeight.Text.ToString());
        }
        catch (Exception)
        {
            return false;     // invalid values
        }

        // Take into account that they may have put in negative numbers.
        // Use a negative number as if it were 0.
        if ((0 >= m_iResizeWidth) && (0 >= m_iResizeHeight))
        {
            m_iResizeWidth = iOrigWidth;
            m_iResizeHeight = iOrigHeight;
        }
        else if (chkImageResizeAspect.Checked)
        {
            bool bModWidth = false;

            if ((iOrigWidth != m_iResizeWidth) || (iOrigHeight != m_iResizeHeight))
            {
                // If both were changed ... 
                if ((m_iResizeWidth != iOrigWidth) && (m_iResizeHeight != iOrigHeight))
                {
                    // ... maintain the width unless it is set to 0.
                    bModWidth = (0 >= m_iResizeWidth);  // Since both changed, only modify the width if it is 0.
                }
                else
                {
                    // maintain the one that was changed.
                    bModWidth = (m_iResizeWidth == iOrigWidth);  // if the width isn't what they changed, then modify it.
                }

                if (bModWidth)
                    m_iResizeWidth = (Int32)(((Int64)iOrigWidth * (Int64)m_iResizeHeight) / (Int64)iOrigHeight);
                else
                    m_iResizeHeight = (Int32)(((Int64)iOrigHeight * (Int64)m_iResizeWidth) / (Int64)iOrigWidth);
            }
        }
        return true;
    }

    private bool IsFileAccessibleForEdit(string strEditFile)
    {
        bool bRet = false;

        if (strEditFile.Length > 0)
        {
            if (CanAccessFileForWrite(strEditFile))
            {
                bRet = true;  // OK
            }
            else
            {
                if (File.Exists(strEditFile))
                {
                    m_strErrorMessage = "The operating system is not providing access to the '" + System.IO.Path.GetFileName(strEditFile) +
                        "' file.  Please see your administrator to provide write access permissions to the file and the '" +
                        System.IO.Path.GetDirectoryName(strEditFile) + "' directory.";
                }
                else
                {
                    m_strErrorMessage = "'" + strEditFile + "' does not exist!";
                }
            }
        }
        else
        {
            m_strErrorMessage = "No image file is selected for edit.";
        }

        return bRet;
    }

    /// <summary>
    /// This attempts to ensure that we have write access
    /// to the specified file.
    /// 
    /// If we don't have access at all, this will return
    /// a false value.
    /// </summary>
    /// <param name="strFilePath"></param>
    /// <returns>True means that we can access for writing.</returns>
    private bool CanAccessFileForWrite(string strFilePath)
    {
        bool bRet = false;

        try
        {
            // Doing this one line is enough of a test.
            File.SetAttributes(strFilePath, FileAttributes.Normal);
            bRet = true;
        }
        catch { }

        return bRet;
    }

    /// <summary>
    /// Unlike .net's MergePath, this takes into account all the versions
    /// of path parts that may be combined.
    /// </summary>
    /// <param name="strBegin">Left part of the path.</param>
    /// <param name="strEnd">Right part of the path.</param>
    /// <param name="bFinalSlash">Should a final slash be placed on the end.</param>
    /// <returns></returns>
    private string CatPath(string strBegin, string strEnd, bool bFinalSlash)
    {
        string strFinalPath = "";

        // Normalize
        strBegin.Replace('\\', '/');  // Normalize
        strBegin.Trim();
        strEnd.Replace('\\', '/');
        strEnd.Trim();

        // This puts them together, ensuring there is at least one slash between
        strFinalPath = strBegin + "/" + strEnd;
        if (bFinalSlash)
            strFinalPath += "/";

        // This will fix if there are double slashes together.
        strFinalPath = strFinalPath.Replace("//", "/");
        strFinalPath = strFinalPath.Replace("//", "/");

        return (strFinalPath);
    }

    private string RemoveDomainName(string pathURL)
    {
        if (true == pathURL.Contains("://"))
        {
            // Makes:  "localhost/Workarea_R2/WorkArea/poormansimages/which_way_is_up.jpg"
            pathURL = pathURL.Substring(pathURL.IndexOf("://") + 3);
            // Makes:  "/Workarea_R2/WorkArea/poormansimages/which_way_is_up.jpg"
            return pathURL.Substring(pathURL.IndexOf("/"));
        }
        else
        {
            return pathURL;
        }
    }

    private string FieldIDValue(HiddenField HField)
    {
        return HField.UniqueID.ToString();  // Use if in a widget
        //return HField.ID.ToString();  // Is if in a dialog
    }

    private string FormIDValue(HtmlForm FormElem)
    {
        return FormElem.UniqueID.ToString();  // Use if in a widget
        //return FormElem.ID.ToString();
    }

    private string DivIDValue(HtmlGenericControl DivTag)
    {
        return DivTag.UniqueID.ToString().Replace('$', '_');  // Use if in a widget
        //return DivTag.ID.ToString();
    }

    private string GetDialogWidgetParameter(string strParamName)
    {
        string strRet = "";

        try
        {
            //strRet = Arguments[strParamName];
        }
        catch
        {
            strRet = "";
        }

        return strRet;
    }

    private string MapPhysicalToURL(string strPhysicalPath)
    {
        string strURL = "";
        System.Web.UI.Control objControl = new System.Web.UI.Control();
        string rootURL = RemoveDomainName(objControl.ResolveUrl("~/"));
        objControl = null;
        string rootLocation = Server.MapPath(rootURL);

        Int32 indexLoc = strPhysicalPath.IndexOf(rootLocation);
        if (indexLoc >= 0)
        {
            strURL = strPhysicalPath.Substring(indexLoc + rootLocation.Length);
            strURL = CatPath(rootURL, strURL, false);
            strURL = strURL.Replace('\\', '/');
        }
        else
        {
            // We are probably were given a remote path.
            strURL = strPhysicalPath;
        }

        return strURL;
    }

    #endregion  // Private Tools  ..................................................


    #region Response Generation  =================================================

    private void BuildResponseArguments()
    {
        /*
        ArgumentDictionary dictionary = new ArgumentDictionary();

        int countKeys = Arguments.Keys.Count;
        string[] namesList = new string[countKeys];
        string[] valueList = new string[countKeys];
        Arguments.Keys.CopyTo(namesList, 0);
        Arguments.Values.CopyTo(valueList, 0);
        for (int index = 0; index < countKeys; index++)
        {
            dictionary.Add(namesList[index], valueList[index]);
        }

        if (m_sourceImageURL.Length > 0)
        {
            ImageTool objImage = new ImageTool(m_sourceImagePhysical);
            dictionary.Add(mc_resultingImageOut, m_sourceImageURL);
            dictionary.Add(mc_resultingWidthOut, objImage.Width.ToString());
            dictionary.Add(mc_resultingHeightOut, objImage.Height.ToString());
            objImage = null;
        }
        ReturnArguments = dictionary;
         */
    }

    #endregion  // Response Generation  ..........................................


    #region Event Listeners  =======================================================

    public void Edit(string fileurl)
    {
        m_sourceImageURL = fileurl;
        hdnSrcFileURL.Value = m_sourceImageURL;
        if (m_sourceImageURL.Length > 0)
            m_sourceImagePhysical = Page.MapPath(m_sourceImageURL);
        else
            m_sourceImagePhysical = "";

        hdnCurrentCommand.Value = ImageData.ImageCommand.View.ToString();
        m_strCommand = hdnCurrentCommand.Value.ToString();  // Need to correctly set.

        FillImageInformation();
    }


    /*
    public override void OnFileEdit(Control trigger, FileEditEventArgs fileEditArgs)
    {
        m_sourceImageURL = fileEditArgs.UrlPath.ToString().Trim();
        hdnSrcFileURL.Value = m_sourceImageURL;
        //m_strDestURL = m_sourceImageURL;
        //hdnTargetFileURL.Value = m_strDestURL;
        if (m_sourceImageURL.Length > 0)
            m_sourceImagePhysical = Page.MapPath(m_sourceImageURL);
        else
            m_sourceImagePhysical = "";

        hdnCurrentCommand.Value = ImageData.ImageCommand.View.ToString();
        m_strCommand = hdnCurrentCommand.Value.ToString();  // Need to correctly set.

        //DrawImageEditArea();  // ???
        FillImageInformation();
    }

    public override void OnOkClicked() 
    {
        BuildResponseArguments();
    }
     */

    #endregion  // Event Listeners
    
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }

}

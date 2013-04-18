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
using System.Text;
using System.IO;

using Ektron.Cms;
using Ektron.Cms.Workarea.Framework;
using Ektron.Cms.ImageTool;

// PLEASE NOTE:
// The governing client reloading the page can only cause a change
// in the command mode by specifying it as a URL parameter.  This
// forces the reload of the page which resets all the field values.
// When a command is run from the page it generates a postback
// which preserves the field values.  The code assumes that this
// is the architecture under which it is running.

public partial class Widgets_Dialogs_ImageScore : System.Web.UI.Page
{

    #region Member Variables  =======================================================
    // The parameters can be passed as URL parameters, and are also stored in the field.
    // The parameters take precidence, and if a parameter is specified, it replaces 
    // what is in the form fields.
    private const string mc_strCmdParam = "c";     // stored in CommandModeField
    private const string mc_strSrcParam = "s";     // stored in SourceModImageField
    private const string mc_strTargetParam = "d";  // stored in DestModImageField
    private const string mc_strModIdParam = "i";   // stored in ModImageIDField
    //private const string mc_strWidthParam = "w";   // stored in DisplayWidthField
    //private const string mc_strHeightParam = "h";  // stored in DisplayHeightField

    // Action commands used locally, so not in a global enumeration.
    private enum EkImageActionCommand
    {
        undo = 1,
        redo = 2,
        crop = 3,
        text = 4,
        brightness = 5,
        //rotate = 6,
        //resize = 7,
        //flip = 8,
        //view = 9,
    }
    private const string mc_strCropCmd = "crop";
    private const string mc_strTextCmd = "text";
    private const string mc_strBrightCmd = "brightness";
    private const string mc_strUndoCmd = "undo";          // This one is a special case, of action on a mode.
    private const string mc_strRedoCmd = "redo";          // This one is a special case, of action on a mode.

    private const long mc_maxBrightnessSlider = 200;   // Highest value for the brightness slider
    private const long mc_midBrightnessSlider = 100;   // Highest value for the brightness slider
    private const long mc_minBrightnessSlider = 0;     // Lowest value for the brightness slider
    private const long mc_iBrightSliderTickCount = 11; // How many tick marks to select from
    private const long mc_iBrightTickHeight = 7;       // How high is a tick mark

    private string m_strSrcImageURL = "";
    private string m_strTargetImageURL = "";
    private string m_strSrcImagePath = "";
    private string m_strTargetImagePath = "";
    private string m_strCommandMode = "";     // This is the mode, or main command, which determines the UI to show. (Set by URL param.)
    private string m_strCommandAction = "";   // This is the action to take on this load (set only by this script)

    private int m_iLocalImageWidth = 0;
    private int m_iLocalImageHeight = 0;

    // Operational
    private bool m_bUniqueNameOnEdit = false;  // See where this is set for having a unique name for every edit.
    private long m_iRecursiveSafeguard = 0;
    private const long mc_iMaxRecursiveSafeguard = 4000;  // This should be more than enough recursive calls for an instance.
    private bool m_bInitialCmdEntry = false;
    //private bool m_bActionExecuted = false;
    private int m_iModImageID = 0;
    private int m_iLastCommandID = 0;
    //private int m_iDisplayWidth = 0;
    //private int m_iDisplayHeight = 0;
    //private int m_iShowWidth = 0;
    //private int m_iShowHeight = 0;
    ImageData m_objImageData = new ImageData();

    //private static Random rndgenerator = new Random();

    #endregion  // Member Variables

    #region Load and Initialization  ================================================

    protected void Page_Load(object sender, EventArgs e)
    {
        // Page_Load is never called, but it is here for if we move code.
        InitializeLoad();
        this.ClientScript.RegisterClientScriptInclude("SelectImageRectangle", "selectimgrect.js");
    }

    protected void InitializeLoad()
    {
        // DEBUG::
        // We will set this so that every edit will create a unique name so the browser will not cache.
        // Should look at a parameter to see if we are coming from the editor.
        // Remove this line when the browser refresh issue is resolved.
        m_bUniqueNameOnEdit = true;

        m_strCommandMode = GetCommandMode();      // this can be thought of as the mode, or main command
        m_strCommandAction = GetCommandAction();  // this is the action to take on this load (set only by this script)

        // The parameters MUST BE url values.
        m_strSrcImageURL = SrcImageName();
        m_strTargetImageURL = TargetImageName();  // This is the important one, since it is receiving the changes.

        m_strSrcImagePath = Page.Server.MapPath(RemoveDomainName(m_strSrcImageURL));
        m_strTargetImagePath = Page.Server.MapPath(RemoveDomainName(m_strTargetImageURL));

        m_iModImageID = ModImageID();

        m_objImageData.PhysicalSourcePath = m_strSrcImagePath;
        m_objImageData.PhysicalTargetPath = m_strTargetImagePath;

        m_objImageData.Image.UseImage(m_strSrcImagePath);
        m_iLocalImageWidth = m_objImageData.Image.Width;
        m_iLocalImageHeight = m_objImageData.Image.Height;
        hdnSrcFileWidth.Value = m_iLocalImageWidth.ToString();
        hdnSrcFileHeight.Value = m_iLocalImageHeight.ToString();

        if (m_bUniqueNameOnEdit) FixPathsToBeUnique();

        //DebugListValues();

        ExecSubmitedCommand();

        SetImageURLs();
    }

    private void SetImageURLs()
    {
        string imagesPath = "images/";  // this.SkinControlsPath + "ImageScore/";
        ImageButton_1.ImageUrl = imagesPath + "ticktop.gif";
        ImageButton_2.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_3.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_4.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_5.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_6.ImageUrl = imagesPath + "tickmajor.gif";
        ImageButton_7.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_8.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_9.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_10.ImageUrl = imagesPath + "tickmark.gif";
        ImageButton_11.ImageUrl = imagesPath + "tickbottom.gif";
        ImageButton_12.ImageUrl = imagesPath + "tickjumpdown.gif";
        ImageButton_13.ImageUrl = imagesPath + "tickjump.gif";
//        SliderSelection.ImageUrl = imagesPath + "tickselect.gif";
    }

    private void FixPathsToBeUnique()
    {
        ;
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

    private void DebugListValues()
    {
        string strValue;

        // Information from the Fields
        strValue = CommandModeField.Value.ToString();
        strValue = CommandActionField.Value.ToString();
        strValue = SourceModImageField.Value.ToString();
        strValue = DestModImageField.Value.ToString();
        strValue = TmpRelativeFileNameField.Value.ToString();
        strValue = ModImageIDField.Value.ToString();
        strValue = LastCmdIDField.Value.ToString();
        strValue = DataOnSubmit.Value.ToString();
        strValue = TextStartX.Value.ToString();
        strValue = TextStartY.Value.ToString();
        strValue = TextWidth.Value.ToString();
        strValue = TextHeight.Value.ToString();
        strValue = SliderLevelField.Value.ToString();

        // Information from the URL parameters
        if (null != Request.Params.Get(mc_strCmdParam))
            strValue = Page.Server.UrlDecode(Request.Params.Get(mc_strCmdParam)).Trim();
        if (null != Request.Params.Get(mc_strSrcParam))
            strValue = Page.Server.UrlDecode(Request.Params.Get(mc_strSrcParam)).Trim();
        if (null != Request.Params.Get(mc_strTargetParam))
            strValue = Page.Server.UrlDecode(Request.Params.Get(mc_strTargetParam)).Trim();
        if (null != Request.Params.Get(mc_strModIdParam))
            strValue = Page.Server.UrlDecode(Request.Params.Get(mc_strModIdParam)).Trim();

        // Values being used
        strValue = m_strCommandMode;
        strValue = m_strCommandAction;
        strValue = m_strSrcImageURL;
        strValue = m_strTargetImageURL;
        strValue = m_strSrcImagePath;
        strValue = m_strTargetImagePath;
        strValue = m_iModImageID.ToString();
    }

    #endregion  // Load and Initialization


    #region Central Command Processing  =============================================

    protected void ExecSubmitedCommand()
    {
        PerformImageAction();
        DisplayModifiedImage();
        AffectResultingUI();
    }

    private void PerformImageAction()
    {
        if (mc_strCropCmd == m_strCommandAction)
        {
            CropImage();
            SetCommandAction("");
            ResetSelectionRect();
            //m_bActionExecuted = true;
        }
        else if (mc_strTextCmd == m_strCommandAction)
        {
            TextImage();
            SetCommandAction("");
            ResetSelectionRect();
            //m_bActionExecuted = true;
        }
        else if (mc_strBrightCmd == m_strCommandAction)
        {
            // This is handled by the code behind button functionality.
        }
        //else if (mc_strUndoCmd == m_strCommandMode)
        else if (ImageData.ImageCommand.Undo.ToString() == m_strCommandMode)
        {
            // This action only happens on a load of the command mode.
            UndoImage();
            SetCommandAction("");
            //m_bActionExecuted = true;
        }
        //else if (mc_strRedoCmd == m_strCommandMode)
        else if (ImageData.ImageCommand.Redo.ToString() == m_strCommandMode)
        {
            // This action only happens on a load of the command mode.
            RedoImage();
            SetCommandAction("");
            //m_bActionExecuted = true;
        }
        else if ("" == m_strCommandAction)
        {
            // We have just brought up the image with no command.
            // This is the intial display of the image.
            // Create the ImageData object, using the given path.
            // This will go through the DB process.
            // (Since we are not executing anything, this is
            // when we can take the time to perform this.)
            //
            // Since the target is going to change, then that
            // is what should be specified.
            //m_objImageData.InitializeImage(m_strTargetImageURL);
            // This is actually done in the ModImageID method.
            // If we have not passed an ID around, we may not
            // have the image in the system.

            // generate a thumbnail for the image in case the user decides
            // to click the save instead of the cancel button w/o making any changes :-P
            m_objImageData.GenerateThumbnail();
        }
    }

    /// <summary>
    /// Records the ID from the last command committed.
    /// </summary>
    private int LastCommandID
    {
        get
        {
            if (0 == m_iLastCommandID)
            {
                if (LastCmdIDField.Value.ToString().Length > 0)
                {
                    m_iLastCommandID = Convert.ToInt32(LastCmdIDField.Value.ToString());
                }
            }
            return m_iLastCommandID;
        }
        set
        {
            m_iLastCommandID = value;
            LastCmdIDField.Value = m_iLastCommandID.ToString();
        }
    }

    /// <summary>
    /// Transient commands only have their last used command as the official command.
    /// This is for commands that have a UI that allows the user to progress through
    /// settings until they have what they want.  Only the last setting is the
    /// persistent value.
    /// </summary>
    private void CommitTransientCommand()
    {
        // OK, this is tricky:
        // If we have no last command, then this is the first brightness command
        // we have given. If so, commit as a straight command....
        int iLastCmd = LastCommandID;
        if (0 == iLastCmd)
        {
            if (0 != m_iModImageID) LastCommandID = m_objImageData.Commit(m_iModImageID);
        }
        else
        {
            // .... otherwise, we are running a transient command again.
            // Select to update the existing command.
            // The last instance of this command will become the official setting.
            if (0 != m_iModImageID) m_objImageData.Update(m_iModImageID, iLastCmd);
        }
    }

    private void CommitPersistentCommand()
    {
        if (0 != m_iModImageID) LastCommandID = m_objImageData.Commit(m_iModImageID);
    }

    // =================================================================
    //private void AddNewImage(string strImageFile, ImageData objTool)
    //{
    //    objTool.AddNewImage(strImageFile);
    //}
    // =================================================================

    private void DisplayModifiedImage()
    {
        ImageDisplay.ImageUrl = m_strTargetImageURL + "?r="
            + System.Guid.NewGuid().ToString("N");
        // + rndgenerator.Next();
    }

    private void AffectResultingUI()
    {
        UIJavaScript.Text = "\n\n" + GeneralJavaScript() + "\n\n";

        if (ImageData.ImageCommand.Rotate.ToString() == m_strCommandMode)
        {
            CleanUpRelativeFile();
//            RotateMenu.Visible = true;
            SetCommandAction("rotate");
        }
        else if (ImageData.ImageCommand.Brightness.ToString() == m_strCommandMode)
        {
            if (false == IsRelativeFileReady())
            {
                SetUpRelativeFile();
                SetBrightness(100);
            }

            m_strSrcImagePath = TmpRelativeFileNameField.Value.ToString().Trim();

            m_bInitialCmdEntry = ("" == m_strCommandAction);
            if (m_bInitialCmdEntry)
            {
                // The slider goes from 0 to 2.  The value of 1 is the middle.
                SliderLevelField.Value = mc_midBrightnessSlider.ToString();  // We will deal in integers for calcs, then div down.
            }
//            BrightnessMenu.Visible = true;
            SetCommandAction(mc_strBrightCmd);
        }
        else if (ImageData.ImageCommand.Crop.ToString() == m_strCommandMode)
        {
            CleanUpRelativeFile();
            if (IsExec.Value.Length == 0)
            {
                UIJavaScript.Text += SelectRectJavaScript();
            }
            SetCommandAction(mc_strCropCmd);
        }
        else if (ImageData.ImageCommand.Text.ToString() == m_strCommandMode)
        {
            CleanUpRelativeFile();
            TextInsertData.Visible = true;
            //ImageText.Visible = true;
            if (IsExec.Value.Length == 0)
            {
                UIJavaScript.Text += SelectRectJavaScript();
            }
            SetCommandAction(mc_strTextCmd);
        }
        else
        {
            CleanUpRelativeFile();
        }

        UIJavaScript.Text += "\n\n";

        //FitImageWithinViewingArea();
    }

    private string GetReferenceFileName(string strSrc)
    {
        string strTmp;

        strTmp = Path.GetTempFileName() + strSrc.Substring(strSrc.LastIndexOf('.'));

        return strTmp.Trim();
    }

    private string GetCommandMode()
    {
        string strCmd = "";

        if (null != Request.Params.Get(mc_strCmdParam))
        {
            strCmd = Page.Server.UrlDecode(Request.Params.Get(mc_strCmdParam)).Trim();
            CommandModeField.Value = strCmd;
        }
        else
        {
            strCmd = CommandModeField.Value.ToString().Trim();
        }

        return strCmd;
    }

    private string GetCommandAction()
    {
        string strCmd = "";

        // This has no URL parameter.
        strCmd = CommandActionField.Value.ToString().Trim();

        return strCmd;
    }

    private void SetCommandAction(string strCmd)
    {
        CommandActionField.Value = strCmd.Trim();
    }

    private string TargetImageName()
    {
        if (m_strTargetImageURL.Length == 0)
        {
            string strImage = "";

            if (null != Request.Params.Get(mc_strTargetParam))
            {
                strImage = Page.Server.UrlDecode(Request.Params.Get(mc_strTargetParam));
                DestModImageField.Value = strImage;  // Store it for later use.
            }
            else
            {
                // Check the field
                strImage = DestModImageField.Value.ToString().Trim();
            }
            // If there is no destination image given, 
            // then assume that the source and destination
            // will be the same.
            if (0 == strImage.Length)
            {
                strImage = SrcImageName();
                DestModImageField.Value = strImage;  // Store it for later use.
            }

            m_strTargetImageURL = strImage;
            return strImage;
        }

        return m_strTargetImageURL;
    }

    protected string SrcImageName()
    {
        if (0 == m_strSrcImageURL.Length)
        {
            string strImage = "";

            if (null != Request.Params.Get(mc_strSrcParam))
            {
                strImage = Page.Server.UrlDecode(Request.Params.Get(mc_strSrcParam)).Trim();
                SourceModImageField.Value = strImage;  // Store it for later use
            }
            else
            {
                // Check the field
                strImage = SourceModImageField.Value.ToString().Trim();
            }

            m_strSrcImageURL = strImage;
            return strImage;
        }

        return m_strSrcImageURL;
    }

    //protected int GetDisplayWidth()
    //{
    //    int iVal = 0;
    //    string strVal;

    //    if (null != Request.Params.Get(mc_strWidthParam))
    //    {
    //        strVal = Page.Server.UrlDecode(Request.Params.Get(mc_strWidthParam)).Trim();
    //        DisplayWidthField.Value = strVal;
    //        iVal = Convert.ToInt32(strVal);  // Store it for later use
    //    }
    //    else
    //    {
    //        // Check the field
    //        strVal = DisplayWidthField.Value.ToString().Trim();
    //        if (strVal.Length > 0)
    //        {
    //            iVal = Convert.ToInt32(strVal);
    //        }
    //    }

    //    return iVal;
    //}

    //protected int GetDisplayHeight()
    //{
    //    int iVal = 0;
    //    string strVal;

    //    if (null != Request.Params.Get(mc_strHeightParam))
    //    {
    //        strVal = Page.Server.UrlDecode(Request.Params.Get(mc_strHeightParam)).Trim();
    //        DisplayHeightField.Value = strVal;
    //        iVal = Convert.ToInt32(strVal);  // Store it for later use
    //    }
    //    else
    //    {
    //        // Check the field
    //        strVal = DisplayHeightField.Value.ToString().Trim();
    //        if (strVal.Length > 0)
    //        {
    //            iVal = Convert.ToInt32(strVal);
    //        }
    //    }

    //    return iVal;
    //}

    private int ModImageID()
    {
        if (0 == m_iModImageID)
        {
            int iID = 0;

            if (null != Request.Params.Get(mc_strModIdParam))
            {
                iID = Convert.ToInt32(Page.Server.UrlDecode(Request.Params.Get(mc_strModIdParam)));
            }
            if (0 == iID)
            {
                if (ModImageIDField.Value.ToString().Length > 0)
                {
                    // If we did not pass as a parameter, check the field.
                    iID = Convert.ToInt32(ModImageIDField.Value.ToString());
                }
            }

            if (0 == iID)  // if it is _still_ zero...
            {
                iID = m_objImageData.ImageId;
                if (0 == iID)    // if it is even _still_ zero...
                {
                    // Initialize the image, so that we can have an ID.
                    iID = m_objImageData.InitializeImage(m_strTargetImagePath);
                }
            }

            ModImageIDField.Value = iID.ToString();  // Set it in the field for the future use.

            m_iModImageID = iID;

            return iID;
        }

        return m_iModImageID;
    }

    #endregion  // Central Command Processing


    #region UI Control  =============================================================

    private void FitImageWithinViewingArea()
    {
        // TEMPORARILY DISABLED

        // These are the calculations to make an image fit.

        //if ((0 != m_iDisplayWidth) && (0 != m_iDisplayHeight) && (0 != m_objImageData.Image.Width) && (0 != m_objImageData.Image.Height))
        //{
        //    // Resize the image to fit in this viewing area.
        //    int iShowW = 0;
        //    int iShowH = 0;
        //    int iTestW = 0;
        //    int iTestH = 0;

        //    // This is the algorithm:
        //    // Make the image width fit.
        //    // Make the image height fit.
        //    // Use the calculation where the perpendicular side fits.
        //    //
        //    // Using standard proportional calculations.

        //    iTestH = (Int32)(((Int64)m_objImageData.Image.Height * (Int64)m_iDisplayWidth) / (Int64)m_objImageData.Image.Width);
        //    iTestW = (Int32)(((Int64)m_objImageData.Image.Width * (Int64)m_iDisplayHeight) / (Int64)m_objImageData.Image.Height);

        //    if (iTestH > m_iDisplayHeight)
        //    {
        //        // use the width
        //        m_iShowWidth = iTestW;
        //        m_iShowHeight = m_iDisplayHeight;
        //    }
        //    else
        //    {
        //        // use the height
        //        m_iShowHeight = iTestH;
        //        m_iShowWidth = m_iDisplayWidth;
        //    }

        //    // Set it up
        //    ImageDisplay.Width = m_iShowWidth;
        //    ImageDisplay.Height = m_iShowHeight;
        //}
    }

    private void ResetSelectionRect()
    {
        // We will do this here, so that we get the basic image info.
        ImageTool objTool = new ImageTool(m_strSrcImagePath);
        long iLocX = (objTool.Width / 15) + 1;
        long iLocY = (objTool.Height / 15) + 1;
        long iWidth = (objTool.Width / 2) + 1;
        long iHeight = (objTool.Height / 2) + 1;

        TextStartX.Value = iLocX.ToString();
        TextStartY.Value = iLocY.ToString();
        TextWidth.Value = iWidth.ToString();
        TextHeight.Value = iHeight.ToString();
    }

    private string SelectRectJavaScript()
    {
        StringBuilder strJS = new StringBuilder();

        strJS.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n");

        if (ImageData.ImageCommand.Text.ToString() == m_strCommandMode)
        {
            strJS.Append("SetTextRectOnDocument();\n");
        }
        else
        {
            strJS.Append("SetResizeRectOnDocument();\n");
        }
        strJS.Append("ShowAreaSelectRect('ImageDisplay');\n");
        strJS.Append("PositionSizeRectFromEntries();\n");

        strJS.Append("function PositionSizeRectFromEntries()\n");
        strJS.Append("{\n");
        // left, top, width, height
        strJS.Append("    EkImgToolPositionAreaSelectRect(\n");
        strJS.Append("        document.getElementById(\"" + FieldIDValue(TextStartX) + "\").value,\n");
        strJS.Append("        document.getElementById(\"" + FieldIDValue(TextStartY) + "\").value,\n");
        strJS.Append("        document.getElementById(\"" + FieldIDValue(TextWidth) + "\").value,\n");
        strJS.Append("        document.getElementById(\"" + FieldIDValue(TextHeight) + "\").value\n");
        strJS.Append("        );\n");
        strJS.Append("}\n");

        strJS.Append("function onSelectionRectangleMove(iLeft, iTop, iWidth, iHeight)\n");
        strJS.Append("{\n");
        strJS.Append("    ImgEditSetSelectParams(iLeft, iTop, iWidth, iHeight);\n");
        strJS.Append("}\n");

        strJS.Append("function onSelectionDoubleClick(iLeft, iTop, iWidth, iHeight)\n");
        strJS.Append("{\n");
        strJS.Append("    ImgEditSetSelectParams(iLeft, iTop, iWidth, iHeight);\n");

        //strJS.Append("        alert(\"Posting TextStartX = [\" + document.getElementById(\"" + FieldIDValue(TextStartX) + "\").value + \"]\");\n");
        //strJS.Append("        alert(\"Posting TextStartY = [\" + document.getElementById(\"" + FieldIDValue(TextStartY) + "\").value + \"]\");\n");
        //strJS.Append("        alert(\"Posting TextWidth = [\" + document.getElementById(\"" + FieldIDValue(TextWidth) + "\").value + \"]\");\n");
        //strJS.Append("        alert(\"Posting TextHeight = [\" + document.getElementById(\"" + FieldIDValue(TextHeight) + "\").value + \"]\");\n");

        strJS.Append("    SubmitThisForm();\n");
        strJS.Append("}\n");

        strJS.Append("function ImgEditSetSelectParams(iLeft, iTop, iWidth, iHeight)\n");
        strJS.Append("{\n");
        strJS.Append("    var elem;\n");
        strJS.Append("    elem = document.getElementById(\"" + FieldIDValue(TextStartX) + "\");\n");
        strJS.Append("    elem.value = iLeft;\n");
        strJS.Append("    elem = document.getElementById(\"" + FieldIDValue(TextStartY) + "\");\n");
        strJS.Append("    if(elem) elem.value = iTop;\n");
        strJS.Append("    elem = document.getElementById(\"" + FieldIDValue(TextWidth) + "\");\n");
        strJS.Append("    if(elem) elem.value = iWidth;\n");
        strJS.Append("    elem = document.getElementById(\"" + FieldIDValue(TextHeight) + "\");\n");
        strJS.Append("    if(elem) elem.value = iHeight;  \n");
        strJS.Append("}\n");

        if (ImageData.ImageCommand.Text.ToString() == m_strCommandMode)
        {
            string strBold = (ImgTextBold.Value == "true") ? "true" : "false";
            string strItal = (ImgTextItal.Value == "true") ? "true" : "false";

            //strJS.Append("window.document.form1.TextFieldName.value = TextRectFieldName();\n");
            strJS.Append("document.getElementById(\"" + FieldIDValue(TextFieldName) + "\").value = TextRectFieldName();\n");
            strJS.Append("SetSelectionRectText(\"\", \"" + ImgTextFont.Value + "\", \"" + ImgTextSize.Value + "\", " +
                strBold + ", " + strItal + ");\n");
        }

        strJS.Append("</script>\n");

        return strJS.ToString();
    }

    private string GeneralJavaScript()
    {
        StringBuilder strJS = new StringBuilder();
        string strFormName = FormIDValue(form1);  // "form1";

        strJS.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n");

        // Taken from direct aspx code

        strJS.Append("function ModifyBrightness(val)\n");
        strJS.Append("{\n");
        strJS.Append("    var elem = document.getElementById(\"" + FieldIDValue(DataOnSubmit) + "\");\n");
        strJS.Append("    elem.value = val;\n");
        strJS.Append("}\n");

        strJS.Append("function SubmitThisForm()\n");
        strJS.Append("{\n");
        strJS.Append("    document.getElementById('IsExec').value = '1';\n");
        strJS.Append("    document.forms[\"" + strFormName + "\"].submit();\n");
        strJS.Append("}\n");

        if ((m_iLocalImageWidth > 0) && (m_iLocalImageHeight > 0))
            strJS.Append("if (\"function\" == typeof SetImageDimensionsEkImgTool) SetImageDimensionsEkImgTool(" + m_iLocalImageWidth + ", " + m_iLocalImageHeight + ");\n");

        strJS.Append("</script>\n");

        return strJS.ToString();
    }

    public bool NeedsResizeRect()
    {
        return ((ImageData.ImageCommand.Crop.ToString() == m_strCommandMode) ||
            (ImageData.ImageCommand.Text.ToString() == m_strCommandMode));
    }

    #endregion  // UI Control


    #region Text Image Controls  ====================================================

    public void TextImage()
    {
        string strFieldName = TextFieldName.Value.ToString();  // Request.Form["TextFieldName"].ToString();
        if (strFieldName.Length > 0)
        {
            string strText = Request.Form[strFieldName].ToString();  //.Text;
            if (strText.Length > 0)
            {
                //ImageTool objTool = new ImageTool();
                //objTool.Text(Convert.ToInt32(TextStartX.Value), Convert.ToInt32(TextStartY.Value),
                //    strText, ImgTextFont.Value, Convert.ToInt32(ImgTextSize.Value),
                //    ("true" == ImgTextBold.Value), ("true" == ImgTextItal.Value),
                //    m_strSrcImagePath, m_strTargetImagePath);

                if ((null != m_objImageData) && (m_strTargetImagePath.Length > 0))
                {
                    //m_objImageData.PhysicalSourcePath = m_strSrcImagePath;
                    //m_objImageData.PhysicalTargetPath = m_strTargetImagePath;
                    int textx = 0;
                    int texty = 0;
                    try
                    {
                        textx = (int) Convert.ToSingle(TextStartX.Value.ToString());
                        texty = (int) Convert.ToSingle(TextStartY.Value.ToString());
                    }
                    catch (Exception)
                    {
                        return;    // should never have this case?
                    }
                    m_objImageData.Text(textx, texty,
                        strText, ImgTextFont.Value.ToString(), Convert.ToInt32(ImgTextSize.Value.ToString()),
                        ("true" == ImgTextBold.Value.ToString().ToLower()), ("true" == ImgTextItal.Value.ToString().ToLower()));
                    CommitPersistentCommand();
                }
            }
        }
    }

    #endregion  // Text Image Controls


    #region Undo and Redo Controls  =================================================

    public void UndoImage()
    {
        // Simple.
        m_objImageData.Undo();
    }

    public void RedoImage()
    {
        // Simple.
        m_objImageData.Redo();
    }

    #endregion  // Undo and Redo Controls


    #region Crop Image Controls  ====================================================

    public void CropImage()
    {
        if ((null != m_objImageData) && (m_strTargetImagePath.Length > 0))
        {
            int startx = 0;
            int starty = 0;
            int width = 0;
            int height = 0;
            try
            {
                startx = Convert.ToInt32(Convert.ToSingle(TextStartX.Value.ToString()));
                starty = Convert.ToInt32(Convert.ToSingle(TextStartY.Value.ToString()));
                width = Convert.ToInt32(Convert.ToSingle(TextWidth.Value.ToString()));
                height = Convert.ToInt32(Convert.ToSingle(TextHeight.Value.ToString()));
            }
            catch (Exception)
            {
                return;    // should never have this case?
            }
            if (m_objImageData.Crop(startx, starty,
                width, height))
            {
                m_iLocalImageWidth = width;
                m_iLocalImageHeight = height;
                hdnSrcFileWidth.Value = m_iLocalImageWidth.ToString();
                hdnSrcFileHeight.Value = m_iLocalImageHeight.ToString();
            }
            CommitPersistentCommand();
        }
    }

    #endregion  // Crop Image Controls


    #region Rotation Controls  ======================================================

    public void RotateImageLeft_Click(object sender, EventArgs e)
    {
        if ((null != m_objImageData) && (m_strTargetImagePath.Length > 0))
        {
            m_objImageData.Rotate(ImageTool.ImageRotation.Left90);
            int oldwidth = m_iLocalImageWidth;
            m_iLocalImageWidth = m_iLocalImageHeight;
            m_iLocalImageHeight = oldwidth;
            hdnSrcFileWidth.Value = m_iLocalImageWidth.ToString();
            hdnSrcFileHeight.Value = m_iLocalImageHeight.ToString();
            CommitPersistentCommand();
        }
    }

    public void RotateImageRight_Click(object sender, EventArgs e)
    {
        if ((null != m_objImageData) && (m_strTargetImagePath.Length > 0))
        {
            m_objImageData.Rotate(ImageTool.ImageRotation.Right90);
            int oldwidth = m_iLocalImageWidth;
            m_iLocalImageWidth = m_iLocalImageHeight;
            m_iLocalImageHeight = oldwidth;
            hdnSrcFileWidth.Value = m_iLocalImageWidth.ToString();
            hdnSrcFileHeight.Value = m_iLocalImageHeight.ToString();
            CommitPersistentCommand();
        }
    }

    #endregion  // Rotation Controls


    #region Brightness Controls  ====================================================

    public void IncBrFast_Click(object sender, EventArgs e)
    {
        ModifyBrightness(1.10);
    }

    public void IncBrSlow_Click(object sender, EventArgs e)
    {
        ModifyBrightness(1.01);
    }

    public void DecBrSlow_Click(object sender, EventArgs e)
    {
        ModifyBrightness(0.99);
    }

    public void DecBrFast_Click(object sender, EventArgs e)
    {
        ModifyBrightness(0.90);
    }

    // ========================================================

    public void Add10BrightnessStep(object sender, EventArgs e) { AdjustBrightnessBy(10); }

    public void Add20BrightnessStep(object sender, EventArgs e) { SetBrightness(120); }

    public void Add40BrightnessStep(object sender, EventArgs e) { SetBrightness(140); }

    public void Add60BrightnessStep(object sender, EventArgs e) { SetBrightness(160); }

    public void Add80BrightnessStep(object sender, EventArgs e) { SetBrightness(180); }

    public void Add100BrightnessStep(object sender, EventArgs e) { SetBrightness(200); }

    public void AddBrightnessStep(object sender, EventArgs e) { AdjustBrightnessBy(2); }

    public void ZeroBrightnessStep(object sender, EventArgs e) { SetBrightness(100); }

    public void SubBrightnessStep(object sender, EventArgs e) { AdjustBrightnessBy(-2); }

    public void Sub10BrightnessStep(object sender, EventArgs e) { AdjustBrightnessBy(-10); }

    public void Sub20BrightnessStep(object sender, EventArgs e) { SetBrightness(100 - 20); }

    public void Sub40BrightnessStep(object sender, EventArgs e) { SetBrightness(100 - 40); }

    public void Sub60BrightnessStep(object sender, EventArgs e) { SetBrightness(100 - 60); }

    public void Sub80BrightnessStep(object sender, EventArgs e) { SetBrightness(100 - 80); }

    public void Sub100BrightnessStep(object sender, EventArgs e) { SetBrightness(100 - 100); }

    private void AdjustBrightnessBy(long iAdj) { SetBrightness(GetSliderPosition() + iAdj); }

    private void SetBrightness(long iVal)
    {
        if (iVal > mc_maxBrightnessSlider)
        {
            iVal = mc_maxBrightnessSlider;
        }
        else if (iVal < mc_minBrightnessSlider)
        {
            iVal = mc_minBrightnessSlider;
        }
        SetSliderPosition(iVal);
        ModifyBrightness((double)((double)iVal / (double)100.0));
    }

    private long GetSliderPosition()
    {
        return Convert.ToInt32(SliderLevelField.Value);
    }

    private void SetSliderPosition(long iPosition)
    {
        SliderLevelField.Value = iPosition.ToString();

        long iAreaHeight = (Convert.ToInt32(ImageButton_1.Height.ToString().Replace("px", "")) * mc_iBrightSliderTickCount);

        // Also, since the selection is defined after the final add button, we need to
        // subtract this from the height.
        long iSelLoc = iAreaHeight - (long)((((double)((double)(iPosition) * (double)(iAreaHeight - mc_iBrightTickHeight)))) / (double)mc_maxBrightnessSlider);

//        SliderSelection.Style["top"] = iSelLoc + "px";
    }

    /// <summary>
    /// High level modification of the brightness.
    /// </summary>
    /// <param name="fVal">Value of 1 leaves alone, 0.x darkens, 1.x is to lightens.</param>
    private void ModifyBrightness(double fVal)
    {
        if ((null != m_objImageData) && (m_strTargetImagePath.Length > 0))
        {
            // We need to set this because they are different, for this command.
            m_objImageData.PhysicalSourcePath = m_strSrcImagePath;
            m_objImageData.PhysicalTargetPath = m_strTargetImagePath;
            m_objImageData.Brightness(fVal);
            // Transient commands only have their last used command as the official command.
            // This is for commands that have a UI that allows the user to progress through
            // settings until they have what they want.  Only the last setting is the
            // persistent value.
            CommitTransientCommand();
        }
    }

    #endregion  // Brightness Controls


    #region Temporary Relative File  ================================================
    private void CleanUpRelativeFile()
    {
        try
        {
            string strFile = TmpRelativeFileNameField.Value.ToString();
            if (strFile.Length > 0)
            {
                TmpRelativeFileNameField.Value = "";
                if (File.Exists(strFile))
                {
                    File.Delete(strFile);
                }
            }
        }
        catch
        {
        }
    }
    private void SetUpRelativeFile()
    {
        try
        {
            string strTmp = GetReferenceFileName(m_strSrcImagePath);
            File.Copy(m_strSrcImagePath, strTmp);
            TmpRelativeFileNameField.Value = strTmp;
        }
        catch
        {
        }
    }
    private bool IsRelativeFileReady()
    {
        return (0 != TmpRelativeFileNameField.Value.ToString().Length);
    }
    #endregion  // Temporary Relative File


    #region Private Internal Methods  ===============================================

    private string FieldIDValue(HiddenField HField)
    {
        //return HField.UniqueID.ToString();  // Use if in a widget
        return HField.ID.ToString();  // Is if in a dialog
    }

    private string FormIDValue(HtmlForm FormElem)
    {
        //return FormElem.UniqueID.ToString();  // Use if in a widget
        return FormElem.ID.ToString();
    }

    private string DivIDValue(HtmlGenericControl divTag)
    {
        //return divTag.UniqueID.ToString().Replace('$', '_');  // Use if in a widget
        return divTag.ID.ToString();
    }

    // What do we need to add before the final .
    // This also checks that the file does not already exist.
    // if it does, it goes onto the next number.
    private string MakeUniqueExtension(string imagePath)
    {
        string strUExt = "(1)";  // default for if we dont have anything.
        int idx = imagePath.LastIndexOf('.');

        // Since this is a recursive function, use the safeguard.
        if(m_iRecursiveSafeguard++ >= mc_iMaxRecursiveSafeguard) return strUExt;

        if (idx > 0)
        {
            // Need to get the number which is just before the extension.
            if (imagePath.LastIndexOf(").") == (idx - 1))
            {
                // We have an existing number
                int startLoc = imagePath.LastIndexOf('(');
                if (startLoc < (idx - 1))
                {
                    string valueString = imagePath.Substring(startLoc + 1, idx - startLoc - 2);
                    if (valueString.Length > 0)
                    {
                        int valueInt = Convert.ToInt32(valueString) + 1;
                        strUExt = "(" + valueInt.ToString() + ")";
                    }
                }
            }
        }

        // If it exists, try again.
        string testPath = AddUniqueExtension(imagePath, strUExt);
        if(System.IO.File.Exists(testPath))
            return MakeUniqueExtension(testPath);
        else
            return strUExt;
    }

    // Place the given extension before the final .
    // Something like "(12)" must be placed before the final .
    private string AddUniqueExtension(string imagePath, string extNew)
    {
        int idx = imagePath.LastIndexOf(").");

        if (idx > 0)
        {
            // We have something to replace
            int startLoc = imagePath.LastIndexOf('(');
            if (startLoc < (idx - 1))
            {
                // Take this out so that we can put in the new unique one.
                imagePath = imagePath.Remove(startLoc, idx - startLoc + 1);
            }
        }

        // Just add it before the final .
        return imagePath.Insert(imagePath.LastIndexOf('.'), extNew);
    }

    #endregion  // Private Internal Methods

}

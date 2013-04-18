using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

using Ektron.Cms.API.Content;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms;
using System.Globalization;

namespace Ektron.Cms.ImageTool
{
    public class ImageData 
    {
        #region Member Variables  ===============================================

        const string mc_defaultPristineSubFolder = @"EkPristineImg";
        const int mc_badImageId = 0;
        const int mc_unknownUndoStepCount = -1;
        const int mc_unknownLangId = 1033;  // -1?
        const string mc_parameterDelimeter = ", ";

        // These are known commands; stored in m_lastCommand with m_lastParams.
        const string mc_commandCrop = "crop";
        const string mc_commandBrightness = "brightness";
        const string mc_commandFlip = "flip";
        const string mc_commandRotate = "rotate";
        const string mc_commandText = "text";
        const string mc_commandResize = "resize";
        const string mc_commandUndo = "undo";
        const string mc_commandRedo = "redo";
        const string mc_commandView = "view";

        // Values of Properties
        //private int m_pristineImageId;  // ID of the pristine image
        private int m_modifyImageId;       // ID of the image we are currently modifying
        private int m_languageId = mc_unknownLangId;
        //private int m_iDerivedFromImageId;  // This will be used in the future.
        private string m_physicalTargetPath = "";
        private string m_physicalSourcePath = "";
        private string m_Url = "";
        private int m_userId;
        private int m_currentUndoStepCount = mc_unknownUndoStepCount;  // This is the current count of any undo steps, 0 means no undos done.

        // Other Member Values
        private string m_lastCommand = "";
        private string m_lastParams = "";
        private int m_lastCommitCommandId;

        ImageTool m_imageTool;
        //LibraryModifiedImageData m_imageModifyData = new LibraryModifiedImageData();
        Ektron.Cms.API.Library m_Library = new Ektron.Cms.API.Library();

        public enum ImageCommand
        {
            Undo = 0,
            Redo,
            Crop,
            Text,
            Rotate,
            Resize,
            Brightness,
            Flip,
            View  // Nothing, really, just display
        }

        // This appication speaks in US English.
        System.Globalization.CultureInfo m_culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
        NumberFormatInfo m_numberFormatInfo = new NumberFormatInfo();

        #endregion  // Member Variables


        #region Constructors  ===================================================

        /// <summary>
        /// The user of this constructor will create an object
        /// which does not have any image data.
        /// Use this for when adding a new image.
        /// </summary>
        /// <param name="?"></param>
        public ImageData()
        {
            // This ImageTool member object is created when a call
            // is made to AddNewImage().

            InitFormatInfo();

            //throw new Exception("The ImageData(void) constructor is not yet implemented.");
        }

        /// <summary>
        /// This constructor loads the object with the data
        /// of the image that has the given ID.
        /// </summary>
        /// <param name="?"></param>
        public ImageData(int idImage)
        {
            InitFormatInfo();

            if (0 != InitializeImageId(idImage))
            {
                m_imageTool = new ImageTool();
            }
        }

        /// <summary>
        /// This constructor loads the object with the data
        /// of the image that exists on the given physical
        /// path.
        /// </summary>
        /// <param name="strPath"></param>
        public ImageData(string imagePhysicalPath)
        {
            InitFormatInfo();

            if (0 != InitializeImage(imagePhysicalPath))
            {
                m_imageTool = new ImageTool();
            }
        }

        private void InitFormatInfo()
        {
            m_numberFormatInfo.NumberDecimalSeparator = ".";
            m_numberFormatInfo.NumberGroupSeparator = ",";
            m_numberFormatInfo.NumberGroupSizes = new int[] { 3 };
        }

        #endregion  // Constructors


        #region Public Properties  ==============================================

        /// <summary>
        /// READ ONLY
        /// If a command was committed, the last ID is stored.
        /// This method will retrieve it.        
        /// NOTE:  The ID is not static, it does not survive
        /// object instances.
        /// </summary>
        public ImageTool Image
        {
            get
            {
                if (null == m_imageTool) m_imageTool = new ImageTool();
                return m_imageTool;
            }
            //set
            //{
            //    //
            //}
        }

        /// <summary>
        /// READ ONLY
        /// This is the ID of the image.
        /// </summary>
        public int ImageId
        {
            get
            {
                return m_modifyImageId;
            }
            //set
            //{
            //    // Read Only
            //}
        }

        // The Id of the image that this image is derived from.
        // This is -1 if it is derived from the Prisine Original.
        //static int DerivedFromImageId  // This will fall to Public in the future.
        //{
        //    get
        //    {
        //        return 0;  // m_iDerivedFromImageId;  // Use this in the future.
        //    }
        //    //set
        //    //{
        //    //    // Read Only
        //    //}
        //}

        /// <summary>
        /// The physical path of the image on the server
        /// that will be changing.
        /// </summary>
        public string PhysicalTargetPath
        {
            get
            {
                return m_physicalTargetPath;
            }
            set
            {
                if (null != value)
                {
                    m_physicalTargetPath = value.Trim();
                    if (m_physicalSourcePath.Length == 0)
                    {
                        PhysicalSourcePath = m_physicalTargetPath;
                    }
                    if (null == m_imageTool)
                    {
                        if (m_physicalTargetPath.Length > 0)
                        {
                            m_imageTool = new ImageTool();
                        }
                    }
                }
                else
                {
                    m_physicalSourcePath = m_physicalTargetPath;
                }
            }
        }

        /// <summary>
        /// The physical path of the image on the server
        /// that will be the source for all changes.  
        /// If this is different from PhysicalTargetPath
        /// then it becomes the source file that does
        /// NOT change.
        /// This defaults to the PhysicalTargetPath value.
        /// </summary>
        public string PhysicalSourcePath
        {
            get
            {
                return m_physicalSourcePath;
            }
            set
            {
                m_physicalSourcePath = "";  // initialize
                if (null != value)
                {
                    if (value.Length > 0)
                    {
                        if (File.Exists(value))
                        {
                            m_physicalSourcePath = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The URL to the image.
        /// Create using:
        /// <example>
        /// System.Uri localUri = new System.Uri(m_Url);
        /// System.Uri.TryCreate("", UriKind.Absolute, out myUri);
        /// </example>
        /// </summary>
        public System.Uri Url
        {
            get
            {
                if (m_Url.Length > 0)
                {
                    return new System.Uri(m_Url);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (null != value)
                {
                    m_Url = value.AbsoluteUri;
                }
            }
        }

        /// <summary>
        /// This is the ID of the user responsible
        /// for the image.
        /// </summary>
        public int UserId
        {
            get
            {
                return m_userId;
            }
            set
            {
                m_userId = value;
            }
        }

        /// <summary>
        /// READ ONLY
        /// If a command was committed, the last ID is stored.
        /// This method will retrieve it.        
        /// NOTE:  The ID is not static, it does not survive
        /// object instances.
        /// </summary>
        public int LastCommitCommandId
        {
            get
            {
                return m_lastCommitCommandId;
            }
            //set
            //{
            //    //
            //}
        }

        #endregion  // Public Properties


        #region Public Static Methods  ==========================================

        public static ImageCommand CommandEnum(string command)
        {
            ImageCommand enumMoniker = ImageCommand.View;

            switch (command.ToLower())  // Can't use m_culture here, because this is static.
            {
                case mc_commandUndo:
                    enumMoniker = ImageCommand.Undo;
                    break;

                case mc_commandRedo:
                    enumMoniker = ImageCommand.Redo;
                    break;

                case mc_commandBrightness:
                    enumMoniker = ImageCommand.Brightness;
                    break;

                case mc_commandCrop:
                    enumMoniker = ImageCommand.Crop;
                    break;

                case mc_commandFlip:
                    enumMoniker = ImageCommand.Flip;
                    break;

                case mc_commandResize:
                    enumMoniker = ImageCommand.Resize;
                    break;

                case mc_commandRotate:
                    enumMoniker = ImageCommand.Rotate;
                    break;

                case mc_commandText:
                    enumMoniker = ImageCommand.Text;
                    break;

                case mc_commandView:
                    enumMoniker = ImageCommand.View;
                    break;
            }

            return enumMoniker;
        }

        #endregion  // Public Static Methods


        #region Public History Methods  =========================================

        /// <summary>
        /// Commit the last command as an official command.
        /// 
        /// Use the LastCommitCommandId property to retrieve the
        /// new command ID, if it is needed later.
        /// 
        /// </summary>
        /// <param name="idImage">The ID of the image.</param>
        /// <returns>The ID of the saved command, or 0 for error.</returns>
        public int Commit(int imageId)
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // NOTE:  The command ID is not static, it does not survive
            // object instances.  So, we MUST have the ID passed as
            // a parameter.  The caller may track this between instances.
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (0 == imageId)
            {
                imageId = m_modifyImageId;
            }

            /*
            // Commit the last command.
            m_lastCommitCommandId = CommitLastCommand(imageId);
            // Use the LastCommitCommandId property to ret
             */
            GenerateThumbnail();
            return m_lastCommitCommandId;
        }
        public bool Commit()
        {
            bool result = (Commit(0) != 0);
            GenerateThumbnail();
            return result;
        }

        /// <summary>
        /// Update the given command ID with the information
        /// from the last command.
        /// </summary>
        /// <param name="idImage">The ID of the image.</param>
        /// <param name="idCmdId">The ID of the command to update.</param>
        /// <returns>The ID of the updated command, or 0 for error.</returns>
        public int Update(int imageId, int commandId)
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // NOTE:  The IDs are not static, it does not survive
            // object instances.  So, we MUST have the ID passed as
            // a parameter.  The caller may track this between instances.
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (0 == commandId)
                return 0;  // We must have the ID.
            if (0 == imageId)
                imageId = m_modifyImageId;

            // Update the given command.
            m_lastCommitCommandId = commandId;

            // Return the command ID.
            //return UpdateLastCommand(commandId);
            return -1;
        }
        public bool Update()
        {
            return (Update(0, m_lastCommitCommandId) != 0);
        }

        /// <summary>
        /// This is the current count of any undo steps done.
        /// </summary>
        /// <returns>The count of how many undos have been performed.
        /// A value of 0 means that no undos were done.</returns>
        public int UndoStepCount()
        {
            if(mc_unknownUndoStepCount == m_currentUndoStepCount)
            {
                if(m_physicalTargetPath.Length > 0)
                {
                    InitializeImage(m_physicalTargetPath);
                }
            }

            // Init may have failed, so...
            return (mc_unknownUndoStepCount == m_currentUndoStepCount) ? 0 : m_currentUndoStepCount;
        }

        /// <summary>
        /// Performs an undo from the current step.
        /// </summary>
        /// <returns>Returns false if at the beginning of the list and it can't undo.</returns>
        public bool Undo()
        {
            //return PerformUndo();
            return false;
        }

        /// <summary>
        /// Performs a redo of image change steps.
        /// </summary>
        /// <returns>Returns false if at the end and it can't redo.</returns>
        public bool Redo()
        {
            //return PerformRedo();
            return false;
        }

        /// <summary>
        /// Reverts the image to the state of the image it was derived from.
        /// </summary>
        /// <returns></returns>
        //public bool Revert()
        //{
        //    throw new Exception("The Revert() method is not yet implemented.");
        //}

        /// <summary>
        /// Brings the image back to the pristine original.
        /// </summary>
        /// <returns></returns>
        //public bool UndoAll()
        //{
        //    throw new Exception("The UndoAll() method is not yet implemented.");
        //}

        /// <summary>
        /// Undoes any changes, undos or edits, and brings the image back to its
        /// backed up state.
        /// </summary>
        /// <returns></returns>
        //public bool Cancel()
        //{
        //    throw new Exception("The Cancel() method is not yet implemented.");
        //}

        #endregion  // Public History Methods


        #region Public Image Edit Methods =======================================

        public bool Brightness(double valueBrightness)
        {
            bool bRet = false;

            if ((null != m_imageTool) && (m_physicalTargetPath.Length > 0) && (File.Exists(m_physicalSourcePath)))
            {
                //if (false == File.Exists(m_physicalTargetPath))   // These are taken out for performance.
                //    EnsureDestPathExists(m_physicalTargetPath);   // When the image is initialized, this should be done.
                bRet = m_imageTool.Brightness(valueBrightness, m_physicalSourcePath, m_physicalTargetPath);

                m_lastCommand = ImageCommand.Brightness.ToString();  // mc_commandBrightness;
                m_lastParams = valueBrightness.ToString(m_numberFormatInfo);
            }

            return bRet;
        }

        public bool Flip(bool vertical)
        {
            bool bRet = false;

            if ((null != m_imageTool) && (m_physicalTargetPath.Length > 0))
            {
                //if (false == File.Exists(m_physicalTargetPath))   // These are taken out for performance.
                //    EnsureDestPathExists(m_physicalTargetPath);   // When the image is initialized, this should be done.
                bRet = m_imageTool.Flip(vertical, m_physicalSourcePath, m_physicalTargetPath);

                m_lastCommand = ImageCommand.Flip.ToString();  // mc_commandFlip;
                m_lastParams = vertical.ToString(m_numberFormatInfo);
            }

            return bRet;
        }

        public bool Rotate(ImageTool.ImageRotation rotateValue)
        {
            bool bRet = false;

            if ((null != m_imageTool) && (m_physicalTargetPath.Length > 0))
            {
                //if (false == File.Exists(m_physicalTargetPath))   // These are taken out for performance.
                //    EnsureDestPathExists(m_physicalTargetPath);   // When the image is initialized, this should be done.
                bRet = m_imageTool.Rotate(rotateValue, m_physicalSourcePath, m_physicalTargetPath);

                m_lastCommand = ImageCommand.Rotate.ToString();  // mc_commandRotate;
                //m_lastParams = rotateValue.ToString(m_numberFormatInfo);  // Compiler states that this is obsolete, to not use format info.
                m_lastParams = rotateValue.ToString();
            }

            return bRet;
        }

        public bool Text(int textX, int textY, string imageText,
                    string fontName, int fontSizePoint, bool useBold, bool useItalic)
        {
            bool bRet = false;

            if ((null != m_imageTool) && (m_physicalTargetPath.Length > 0))
            {
                //if (false == File.Exists(m_physicalTargetPath))   // These are taken out for performance.
                //    EnsureDestPathExists(m_physicalTargetPath);   // When the image is initialized, this should be done.
                bRet = m_imageTool.Text(textX, textY, imageText,
                    fontName, fontSizePoint, useBold, useItalic, m_physicalSourcePath, m_physicalTargetPath);

                m_lastCommand = ImageCommand.Text.ToString();  // mc_commandText;
                m_lastParams = textX.ToString(m_numberFormatInfo) + mc_parameterDelimeter + textY.ToString(m_numberFormatInfo) + 
                    mc_parameterDelimeter + imageText + mc_parameterDelimeter +
                    fontName + mc_parameterDelimeter + fontSizePoint.ToString(m_numberFormatInfo) + 
                    mc_parameterDelimeter + useBold.ToString(m_numberFormatInfo) + mc_parameterDelimeter + useItalic;
            }

            return bRet;
        }

        public bool Crop(int cropStartX, int cropStartY, int cropWidth, int cropHeight)
        {
            bool bRet = false;

            if ((null != m_imageTool) && (m_physicalTargetPath.Length > 0))
            {
                //if (false == File.Exists(m_physicalTargetPath))   // These are taken out for performance.
                //    EnsureDestPathExists(m_physicalTargetPath);   // When the image is initialized, this should be done.
                bRet = m_imageTool.Crop(cropStartX, cropStartY, cropWidth, cropHeight,
                    m_physicalSourcePath, m_physicalTargetPath);

                m_lastCommand = ImageCommand.Crop.ToString();  // mc_commandCrop;
                m_lastParams = cropStartX.ToString(m_numberFormatInfo) + mc_parameterDelimeter + 
                    cropStartY.ToString(m_numberFormatInfo) + mc_parameterDelimeter +
                    cropWidth.ToString(m_numberFormatInfo) + mc_parameterDelimeter + 
                    cropHeight.ToString(m_numberFormatInfo);
            }

            return bRet;
        }

        public bool Resize(int resizeWidth, int resizeHeight, bool maintainAspect)
        {
            bool bRet = false;

            if ((null != m_imageTool) && (m_physicalTargetPath.Length > 0))
            {
                //if (false == File.Exists(m_physicalTargetPath))   // These are taken out for performance.
                //    EnsureDestPathExists(m_physicalTargetPath);   // When the image is initialized, this should be done.
                bRet = m_imageTool.Resize(resizeWidth, resizeHeight, maintainAspect,
                    m_physicalSourcePath, m_physicalTargetPath);

                m_lastCommand = ImageCommand.Resize.ToString();  // mc_commandResize;
                string strMaintain = (maintainAspect) ? "true" : "false";
                m_lastParams = resizeWidth.ToString(m_numberFormatInfo) + mc_parameterDelimeter +
                    resizeHeight.ToString(m_numberFormatInfo) + mc_parameterDelimeter + strMaintain;
            }

            return bRet;
        }

        public void GenerateThumbnail()
        {
            if ((null != m_imageTool) && (m_physicalTargetPath.Length > 0))
            {
                string imagefilepath = m_physicalTargetPath;
                string thumbfilepath = imagefilepath;
                if (thumbfilepath.ToLower().EndsWith(".gif"))
                {
                    thumbfilepath = thumbfilepath.Substring(0, thumbfilepath.LastIndexOf(".")) + ".png";
                }
                thumbfilepath = thumbfilepath.Substring(0, thumbfilepath.LastIndexOf("\\") + 1)
                    + "thumb_"
                    + thumbfilepath.Substring(thumbfilepath.LastIndexOf("\\") + 1);
                try
                {
                    m_imageTool.Resize(125, -1, true, imagefilepath, thumbfilepath);
                }
                catch (IOException)
                {
                    // the thumbnail service might be already updating the thumbnail
                    // so just ignore any I/O exceptions
                }
            }
        }

        #endregion  // Image Edit Commands


        #region Private Utilities  ==============================================

        /*
        private void TruncateCommands()
        {
            InitGlobalRefs();
            if (null != m_Library)
            {
                if (m_currentUndoStepCount > 0)
                {
                    int idx;
                    int iCount;
                    LibraryModifiedHistoryData[] aryModHistory = m_Library.GetAllImageModCommand(m_modifyImageId);
                    iCount = aryModHistory.Length;
                    // Delete each of the commands at the end.
                    for (idx = 0; idx < m_currentUndoStepCount; idx++)
                    {
                        iCount--;
                        m_Library.DeleteImageModCommand(aryModHistory[iCount].CmdId);
                    }

                    UpdateUndoStepCount(0);
                }
            }
        }

        /// <summary>
        /// Adds the last command.
        /// </summary>
        /// <param name="idImage">The ID of the image that had the command.</param>
        /// <returns>The ID of the new command.</returns>
        private int CommitLastCommand(int imageId)
        {
            if (0 == m_lastCommand.Length)
            {
                // No command to commit.
                return 0;
            }
            if (m_currentUndoStepCount > 0)
            {
                TruncateCommands();
            }
            InitGlobalRefs();
            if (null != m_Library)
                return m_Library.AddImageModCommand(imageId, m_languageId, m_lastCommand, m_lastParams);
            else
                return 0;
        }

        /// <summary>
        /// Updates the given command, with the id, 
        /// with the latest command.
        /// </summary>
        /// <param name="idCmd">the ID of the command</param>
        /// <returns></returns>
        private int UpdateLastCommand(int commandId)
        {
            if (0 == m_lastCommand.Length)
            {
                // No command to commit.
                return 0;
            }
            InitGlobalRefs();
            if (null != m_Library)
            {
                if (false == m_Library.UpdateImageModCommand(commandId, m_lastCommand, m_lastParams))
                    return commandId;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
         */

        /// <summary>
        /// Retrieves the information about the given image.
        /// If the image does not exist, it is added as a Pristine
        /// Image, and the starter Derived Image. 
        /// If imageID has a value, then it is used in the search,
        /// otherwise imagePhysicalPath is used.
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="imagePhysicalPath"></param>
        /// <returns></returns>
        private bool SetUpImageInformation(int imageId, string imagePhysicalPath)
        {
            bool bRet = true;

            bRet = RetrieveImageInformation(imageId, imagePhysicalPath);
            if (false == bRet)
            {
                // We did not find the modified image
                if (imagePhysicalPath.Length > 0)
                {
                    // Add the image as pristine and 
                    bRet = AssignNewImage(imagePhysicalPath);
                }
            }

            if (bRet)
            {
                SetMemberDataFromModData();
            }

            return bRet;
        }

        private bool RetrieveImageInformation(int imageId, string imagePhysicalPath)
        {
            bool bRet = false;

            InitGlobalRefs();

            /*
            if ((null != m_imageModifyData) && (null != m_Library))
            {
                m_imageModifyData.ImageId = imageId;
                m_imageModifyData.PhysicalPath = imagePhysicalPath;
                bRet = (m_Library.RetrieveModifiedImage(ref m_imageModifyData) != mc_badImageId);
            }
             */
            return bRet;
        }

        private void InitGlobalRefs()
        {
            /*
            if (null == m_imageModifyData)
            {
                m_imageModifyData = new LibraryModifiedImageData();
            }
             */
            if(null == m_Library)
            {
                m_Library = new Ektron.Cms.API.Library();
            }
        }

        private bool AssignNewImage(string physicalImagePath)
        {
            bool bRet = false;
            //int iNewId = mc_badImageId;

            if (ValidImageFile(physicalImagePath))
            {
                /*
                m_pristineImageId = AddPristineImage(physicalImagePath);

                // Associate the editable image
                if (m_pristineImageId != mc_badImageId)
                {
                    // Make editable the working image
                    iNewId = AssociateModImage(m_pristineImageId, physicalImagePath);
                    bRet = (iNewId != mc_badImageId);
                }
                 */
                m_physicalSourcePath = physicalImagePath;
                m_physicalTargetPath = physicalImagePath;
                m_modifyImageId = 1;  // has to be non-zero
                //m_pristineImageId = 1;
            }

            return bRet;
        }

        private void SetMemberDataFromModData()
        {
            /*
            if (null != m_imageModifyData)
            {
                m_physicalTargetPath = m_imageModifyData.PhysicalPath;
                m_modifyImageId = m_imageModifyData.ImageId;
                m_userId = m_imageModifyData.LastUserId;
                m_currentUndoStepCount = m_imageModifyData.CurrentUndoStepId;
                m_pristineImageId = m_imageModifyData.PristineId;
                m_languageId = m_imageModifyData.LanguageId;
            }
             */
            m_physicalSourcePath = m_physicalTargetPath;
            if (0 == m_languageId)
            {
                m_languageId = mc_unknownLangId;
            }
        }

        /*
        /// <summary>
        /// Associates the given image with the Pristine image
        /// specified with the ID value.
        /// DANGEROUS FUNCTION
        /// Do NOT use this to derive an image.  This is only for use when 
        /// a newly added image is loaded.
        /// Use DeriveImage() to derive a new image.
        /// </summary>
        /// <param name="strPhysicalImagePath">ID of the Pristine Image</param>
        /// <param name="m_pristineImageId">Optional:  If empty then a new image is created.
        /// if a value</param>
        /// <returns></returns>
        /// <remarks>The scenario is such:
        /// Upload a new image
        /// Add the new image as Pristine
        /// Use the uploaded image (as a shortcut) as the original derived image
        /// Add the image to the DB as a derived image
        /// Edit the image.
        ///</remarks>
        private int AssociateModImage(int pristineImageId, string physicalImagePath)
        {
            Ektron.Cms.API.Content.Content objContent = new Ektron.Cms.API.Content.Content();
            LibraryImageFoundationData imagePristineData = new LibraryImageFoundationData();

            imagePristineData.Id = pristineImageId;
            InitGlobalRefs();
            if (null != m_Library)
            {
                m_Library.RetrieveFoundationImage(ref imagePristineData);

                if (null != m_imageModifyData)
                {
                    //m_imageModifyData.ImageId  // set when image is stored
                    m_imageModifyData.LanguageId = objContent.DefaultContentLanguage;
                    m_imageModifyData.PristineId = imagePristineData.Id;
                    m_imageModifyData.LibraryId = 0;
                    m_imageModifyData.DerivedFromId = 0;
                    m_imageModifyData.PristineCheckSumVal = imagePristineData.CheckSum;
                    m_imageModifyData.WorkbenchURL = "";  // TODO
                    m_imageModifyData.PublishURL = "";  // TODO
                    m_imageModifyData.PhysicalPath = physicalImagePath;
                    m_imageModifyData.MimeType = imagePristineData.MimeType;
                    m_imageModifyData.LastUserId = objContent.UserId;
                    m_imageModifyData.CurrentUndoStepId = 0;
                    m_imageModifyData.DeriveStepId = 0;

                    m_modifyImageId = m_Library.AddModifiedImage(ref m_imageModifyData);
                }
            }

            return m_modifyImageId;
        }

        /// <summary>
        /// This adds the Pristine Original to the database.
        /// All modifications have their origins to this image.
        /// </summary>
        /// <param name="strPhysicalImagePath"></param>
        /// <returns>The ID of the added Pristine Original.
        /// A -1 is an error.</returns>
        private int AddPristineImage(string physicalImagePath)
        {
            string strPristinePath = CreatePristineImagePath(physicalImagePath);
            Ektron.Cms.API.Content.Content objContent = new Ektron.Cms.API.Content.Content();
            // Add the pristine image
            //LibraryNewImageFoundationData imageAddData = new LibraryNewImageFoundationData();

            if (null == m_imageTool)
            {
                m_imageTool = new ImageTool(strPristinePath);
            }

            imageAddData.DepthPx = m_imageTool.Depth;
            imageAddData.Height = m_imageTool.Height;
            imageAddData.LanguageId = objContent.DefaultContentLanguage;
            imageAddData.Width = m_imageTool.Height;
            imageAddData.MimeType = FileMimeType(physicalImagePath);
            imageAddData.FileProperName = System.IO.Path.GetFileName(physicalImagePath);
            imageAddData.PathAndFile = strPristinePath;

            // NOTE:  A return value of -1 means no ID was assigned.
            //m_pristineImageId = m_Library.StoreFoundationImage(ref imageAddData);

            return m_pristineImageId;
        }
         */

        private static string CreatePristineImagePath(string imagePath)
        {
            string strDestPath = imagePath;

            // For now, use the asset path.
            Ektron.Cms.SiteAPI siteAPI = new Ektron.Cms.SiteAPI();

            // There is a problem in the unit tests where the HttpContext is
            // NOT always available.  This is a bad situation, since the module
            // needs to know where the location is.  So, if this is the case
            // use the current folder as the location.
            // So, this has to break from the FXCop standard and eat the
            // error, since it only pertains to this particular instance.
            //strDestPath = System.IO.Path.Combine(System.Web.HttpContext.Current.Request.MapPath(siteAPI.SitePath.ToString()), "assets");
            string strSiteLocation;
            try
            {
                strSiteLocation = System.Web.HttpContext.Current.Request.MapPath(siteAPI.SitePath.ToString());
            }
            catch
            {
                strSiteLocation = Directory.GetCurrentDirectory();  // @"C:\Inetpub\wwwroot\400WorkareaR2\WorkArea\";
            }

            strDestPath = System.IO.Path.Combine(strSiteLocation, "assets"); 
            strDestPath = System.IO.Path.Combine(strDestPath, mc_defaultPristineSubFolder);

            // Make sure it exists
            System.IO.Directory.CreateDirectory(strDestPath);

            strDestPath = System.IO.Path.Combine(strDestPath,
                System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName()) + System.IO.Path.GetExtension(imagePath));

            // Now, copy over.
            System.IO.File.Copy(imagePath, strDestPath);

            return strDestPath;
        }

        //private void EnsureDestPathExists(string targetPath)
        //{
        //    try
        //    {
        //        string strPath = System.IO.Path.GetDirectoryName(targetPath); //targetPath.Substring(1, targetPath.LastIndexOf('\\') - 1);
        //        System.IO.Directory.CreateDirectory(strPath);
        //    }
        //    catch (Exception e)
        //    {
        //        m_lastError = e.Message;
        //    }
        //}

        private bool ValidImageFile(string filePath)
        {
            bool bRet = false;

            if (filePath.Length > 0)
            {
                // We shouldn't use String.Compare with insensitive compare because we need to get
                // the substring anyway, and perform multiple checks on the result.  So, wouldn't
                // it be more efficient to just get a lower case and do simple compares?
                //string strExt = filePath.Substring(filePath.LastIndexOf('.')).ToLower(m_culture);

                //if ((strExt == ".gif") || (strExt == ".jpg") || (strExt == ".png") || (strExt == ".jpe") || (strExt == ".jpeg") || (strExt == ".bmp"))
                if(CompareToStringList(filePath.Substring(filePath.LastIndexOf('.')), new string[] { ".gif", ".jpg", ".png", ".jpe", ".jpeg", ".bmp"} ) >= 0)
                {
                    bRet = System.IO.File.Exists(filePath);
                }
            }

            return bRet;
        }

        /// <summary>
        /// This checks to see if a given string is one of the strings
        /// in the given array.
        /// </summary>
        /// <param name="p">The string to look for.</param>
        /// <param name="p_2">The array of strings.</param>
        /// <returns>The index (0 based) into the array. A -1 means not found.</returns>
        private int CompareToStringList(string p, string[] p_2)
        {
            for (int idx = 0; idx < p_2.Length; idx++)
            {
                if (String.Compare(p, p_2[idx], true, m_culture) == 0)
                {
                    return idx;
                }
            }

            return -1;
        }

        private string FileMimeType(string filePath)
        {
            string strType = "";

            //switch (filePath.Substring(filePath.LastIndexOf(".") + 1).ToLower())
            switch(CompareToStringList(filePath.Substring(filePath.LastIndexOf(".")), new string[] { ".gif", ".jpg", ".png", ".jpe", ".jpeg", ".bmp"} ))
            {
                case 0:  // "gif":
                    strType = "image/gif";
                    break;

                case 1:  // "jpg":
                case 3:  // "jpe":
                case 4:  // "jpeg":
                    strType = "image/jpeg";
                    break;

                case 2:  // "png":
                    strType = "image/png";
                    break;

                case 5:  // "bmp":
                    //return "image/x-ms-bmp";  // this is the official
                    strType = "image/bmp";  // this is what is supported by IIS
                    break;
            }

            return strType;
        }

        /*
        /// <summary>
        /// Performs all the undo functionality for the current image.
        /// </summary>
        /// <returns></returns>
        private bool PerformUndo()
        {
            bool bRet = false;

            // Quick Checks
            if (0 == m_physicalTargetPath.Length)
                return false;
            if (mc_unknownUndoStepCount == UndoStepCount())
                return false;
            if (mc_badImageId == m_modifyImageId)
                return false;

            LibraryModifiedHistoryData[] aryModHistory = m_Library.GetAllImageModCommand(m_modifyImageId);
            if (null != aryModHistory)
            {
                if (aryModHistory.Length > 0)
                {
                    LibraryImageFoundationData dataPristine = new LibraryImageFoundationData();
                    dataPristine.Id = m_pristineImageId;
                    if (m_Library.RetrieveFoundationImage(ref dataPristine) != 0)
                    {
                        int iNewUndoStepCount = m_currentUndoStepCount + 1;
                        int iRunCommandCount = aryModHistory.Length - iNewUndoStepCount;

                        if (iRunCommandCount >= 0)
                        {
                            // Roll through the commands
                            if (ProcessCommandList(ref aryModHistory, dataPristine.PicturePath, iRunCommandCount) == iRunCommandCount)
                            {
                                UpdateUndoStepCount(iNewUndoStepCount);
                                bRet = true;
                            }
                        }
                        else
                        {
                            // We have gone beyond any changes, don't do anything.
                        }
                    }
                }
            }

            return bRet;
        }

        private void UpdateUndoStepCount(int nextUndoStepCount)
        {
            m_Library.UpdateImageUndoCount(m_modifyImageId, nextUndoStepCount);

            m_currentUndoStepCount = nextUndoStepCount;
        }

        /// <summary>
        /// Performs all the undo functionality for the current image.
        /// </summary>
        /// <returns></returns>
        private bool PerformRedo()
        {
            bool bRet = false;

            // Quick Checks
            if (0 == m_physicalTargetPath.Length)
                return false;
            if (mc_unknownUndoStepCount == UndoStepCount())
                return false;
            if (mc_badImageId == m_modifyImageId)
                return false;
            if (0 >= m_currentUndoStepCount)
                return false;

            LibraryModifiedHistoryData[] aryModHistory = m_Library.GetAllImageModCommand(m_modifyImageId);
            if (null != aryModHistory)
            {
                if (aryModHistory.Length > 0)
                {
                    LibraryImageFoundationData dataPristine = new LibraryImageFoundationData();
                    dataPristine.Id = m_pristineImageId;
                    if (m_Library.RetrieveFoundationImage(ref dataPristine) != 0)
                    {
                        int iNewUndoStepCount = m_currentUndoStepCount - 1;
                        int iRunCommandCount = aryModHistory.Length - iNewUndoStepCount;

                        if (iRunCommandCount >= 0)
                        {
                            // Roll through the commands
                            if (ProcessCommandList(ref aryModHistory, dataPristine.PicturePath, iRunCommandCount) == iRunCommandCount)
                            {
                                UpdateUndoStepCount(iNewUndoStepCount);
                                bRet = true;
                            }
                        }
                        else
                        {
                            // We have gone beyond any changes, don't do anything.
                        }
                    }
                }
            }

            return bRet;
        }

        /// <summary>
        /// Processes the commands in the array.
        /// </summary>
        /// <param name="aryModHistory"></param>
        /// <param name="m_currentUndoStepCount"></param>
        /// <returns>The count of how many command executed successfully.</returns>
        private int ProcessCommandList(ref LibraryModifiedHistoryData[] modifyHistory, string pristinePath, int maxCount)
        {
            // Store the current image paths
            string strStoreSrc = m_physicalSourcePath;
            int idx = 0;

            if (0 == maxCount)
            {
                // We have undone all the changes.  Just copy.
                File.Delete(m_physicalTargetPath);
                File.Copy(pristinePath, m_physicalTargetPath);
            }
            else
            {
                // We only use the Pristine image the first time.
                // After that, we use the resulting image.
                m_physicalSourcePath = pristinePath;
                ExecuteCommand(modifyHistory[0].CmdText, modifyHistory[0].CmdParams);
                m_physicalSourcePath = strStoreSrc;

                // Process the rest of the commands
                for (idx = 1; idx < maxCount && idx < modifyHistory.Length; idx++)
                {
                    ExecuteCommand(modifyHistory[idx].CmdText, modifyHistory[idx].CmdParams);
                }
            }

            //m_physicalSourcePath = strStoreSrc;

            return idx;
        }
         */

        private void ExecuteCommand(string command, string parameters)
        {
            string[] aryParms = parameters.Split(',');

            switch (command.ToLower(m_culture))
            {
                case"crop":
                    if (aryParms.Length >= 4)
                    {
                        Crop(Convert.ToInt32(aryParms[0].Trim(), m_numberFormatInfo), Convert.ToInt32(aryParms[1].Trim(), m_numberFormatInfo),
                                Convert.ToInt32(aryParms[2].Trim(), m_numberFormatInfo), Convert.ToInt32(aryParms[3].Trim(), m_numberFormatInfo));
                    }
                    break;

                case "brightness":
                    if (aryParms.Length >= 1)
                    {
                        Brightness(Convert.ToDouble(aryParms[0].Trim(), m_numberFormatInfo));
                    }
                    break;

                case "flip":
                    if (aryParms.Length >= 1)
                    {
                        Flip(Convert.ToBoolean(aryParms[0].Trim(), m_numberFormatInfo));
                    }
                    break;

                case "rotate":
                    if (aryParms.Length >= 1)
                    {
                        ImageTool.ImageRotation enumRot = ImageTool.ImageRotation.None;
                        string strParam = aryParms[0].Trim().ToLower(m_culture);

                        if (strParam == ImageTool.ImageRotation.Half180.ToString().ToLower(m_culture))
                            enumRot = ImageTool.ImageRotation.Half180;
                        else
                            if (strParam == ImageTool.ImageRotation.Left90.ToString().ToLower(m_culture))
                            enumRot = ImageTool.ImageRotation.Left90;
                        else
                            if (strParam == ImageTool.ImageRotation.Right90.ToString().ToLower(m_culture))
                            enumRot = ImageTool.ImageRotation.Right90;

                        if(ImageTool.ImageRotation.None != enumRot)
                            Rotate(enumRot);
                    }
                    break;

                case "text":
                    if (aryParms.Length >= 7)
                    {
                        Text(Convert.ToInt32(aryParms[0].Trim(), m_numberFormatInfo), Convert.ToInt32(aryParms[1].Trim(), m_numberFormatInfo),
                             aryParms[2].Trim(), aryParms[3].Trim(), Convert.ToInt32(aryParms[4].Trim(), m_numberFormatInfo),
                             Convert.ToBoolean(aryParms[5].Trim(), m_numberFormatInfo), Convert.ToBoolean(aryParms[6].Trim(), m_numberFormatInfo));
                    }
                    break;

                case "resize":
                    if (aryParms.Length >= 3)
                    {
                        Resize(Convert.ToInt32(aryParms[0].Trim(), m_numberFormatInfo), Convert.ToInt32(aryParms[1].Trim(), m_numberFormatInfo),
                            Convert.ToBoolean(aryParms[2].Trim(), m_numberFormatInfo));
                    }
                    break;

                // These don't do anything.
                case "undo":
                    break;

                case "redo":
                    break;

                case "view":
                    break;
            
                //default:
                //    throw new Exception("ExecuteCommand requested to execute an invalid command of '" + command + "'.");
            }

        }

        #endregion  // Private Utilties


        #region Initialize Image Action Methods  ================================

        /// <summary>
        /// Initializes the image from the path given.
        /// This assumes that the image MAY NOT exist
        /// in the CMS when this is called.  If it does
        /// not, then it is added to the Image Tool
        /// functionality.  If not, the information
        /// about the image is loaded.
        /// </summary>
        /// <param name="strPhysicalImagePath">Physical path to the image.</param>
        /// <returns>The CMS ID of the image.  A 0 is an error.</returns>
        public int InitializeImage(string physicalImagePath)
        {
            SetUpImageInformation(0, physicalImagePath);
            return m_modifyImageId;
        }

        /// <summary>
        /// Initializes the image from the given ID.
        /// This assumes that since an ID exists the 
        /// image exists in the CMS Image Tool functionality.
        /// If it does not, then the image information is
        /// not loaded.
        /// </summary>
        /// <param name="idImage">The CMS ID of the image.</param>
        /// <returns>The CMS ID of the image.  A 0 is an error.</returns>
        private int InitializeImageId(int imageId)
        {
            SetUpImageInformation(imageId, "");
            return m_modifyImageId;
        }

        #endregion  // Public Action Methods

    }
}

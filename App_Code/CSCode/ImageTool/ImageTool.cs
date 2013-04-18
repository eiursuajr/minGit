using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Ektron.Cms.ImageTool
{
    public class ImageTool
    {
        #region Member Variables  ===============================================

        private int m_width;
        private int m_height;
        private int m_depth;
        private long m_fileSize;
        private string m_localResultFile = "";
        private string m_originalFile = "";

        public enum ImageRotation
        {
            None = 0,
            Right90 = 1,
            Left90 = 2, 
            Half180 = 3
        }

        System.Globalization.CultureInfo m_culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

        #endregion  // Member Variables

        #region Constructors  ===================================================
        
        /// <summary>
        /// Construct an image tool for general use.  The image can be 
        /// assigned later with the UseImage method.
        /// </summary>
        public ImageTool()
        {
            ;
        }

        /// <summary>
        /// Create an image tool using the given file.
        /// </summary>
        /// <param name="originalPathFile">The physical location of the image.</param>
        public ImageTool(string originalPathFile)
        {
            UseImage(originalPathFile);
        }

        #endregion  // Constructors

        #region Public Properties  ==============================================

        public int Width
        {
            get { return m_width; }
            //set { }
        }

        public int Height
        {
            get { return m_height; }
            //set { }
        }

        public int Depth
        {
            get { return m_depth; }
            //set { }
        }

        public string LocalResultFile
        {
            get { return m_localResultFile; }
            //set { }
        }

        public string LocalOriginalFile
        {
            get { return m_originalFile; }
            //set { }
        }

        public long FileSize
        {
            get { return m_fileSize; }
            //set { }
        }

        #endregion  // Public Properties

        #region Public Methods  =================================================

        /// <summary>
        /// Assigns an image to the object.
        /// </summary>
        /// <param name="originalPathFile">The physical location of the image.</param>
        /// <returns>True on success.</returns>
        public bool UseImage(string originalPathFile)
        {
            bool bRet = false;

            if (null == originalPathFile) return false;

            m_originalFile = originalPathFile;

            // Fix slashes - never trust that they have done this.
            m_originalFile = m_originalFile.Replace('/', '\\').Replace("\\\\", "\\");

            // Default to this one
            m_localResultFile = m_originalFile;

            LoadSelectedImageInformation();

            bRet = true;

            return bRet;
        }

        /// <summary>
        /// Modifies the brightness of the given file.
        /// </summary>
        /// <param name="dblAffect">>How to modify.  Value of 1 leaves alone, 0.x darkens, 1.x is to lightens.</param>
        /// <param name="originalPathFile">Source file, could be the same as the change file.</param>
        /// <param name="changeImagePathFile">The file to receive the change.</param>
        /// <returns>True for success.</returns>
        public bool Brightness(double brightValue, string originalPathFile, string changeImagePathFile)
        {
            bool bRet = false;

            if (null == originalPathFile) return false;
            if (null == changeImagePathFile) return false;

            if (originalPathFile.Length > 0)  // may have been set at creation, so not given
                m_originalFile = originalPathFile;

            // Fix slashes - never trust that they have done this.
            m_originalFile = m_originalFile.Replace('/', '\\').Replace("\\\\", "\\");
            changeImagePathFile = changeImagePathFile.Replace('/', '\\').Replace("\\\\", "\\");

            // use cximage library instead of .Net code
            if (m_originalFile != changeImagePathFile)
            {
                // copy file to new if needed
                File.Copy(m_originalFile, changeImagePathFile, true);
                MakeWriteable(changeImagePathFile); // in case the original was readonly
            }
            if (brightValue != 1.0)
            {
                ImageGalleryInterop.CImageResizerClass ig = new ImageGalleryInterop.CImageResizerClass();
                ig.BrightenImageFile(changeImagePathFile, brightValue);
            }

            /*
            System.Drawing.Image img = System.Drawing.Image.FromFile(m_originalFile);
            Bitmap bmpImage = new Bitmap(img);
            img.Dispose();  // Free up the file for changes

            Color pixel;
            int r, g, b;

            for (int y = 0; y < bmpImage.Height; y++)
            {
                for (int x = 0; x < bmpImage.Width; x++)
                {
                    pixel = bmpImage.GetPixel(x, y);
                    r = (int)((double)pixel.R * brightValue); if (r < 0) r = 0; if (r > 255) r = 255;
                    g = (int)((double)pixel.G * brightValue); if (g < 0) g = 0; if (g > 255) g = 255;
                    b = (int)((double)pixel.B * brightValue); if (b < 0) b = 0; if (b > 255) b = 255;
                    bmpImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            bmpImage.Save(changeImagePathFile);

            // Save for convenient use.
            m_width = bmpImage.Width;
            m_height = bmpImage.Height;

            bmpImage.Dispose();
            */

            m_localResultFile = changeImagePathFile;
            bRet = true;

            return bRet;
        }

        /// <summary>
        /// Flips the image.
        /// </summary>
        /// <param name="vertical">True if the flip is vertical, otherwise horizontal.</param>
        /// <param name="originalPathFile">Source file, could be the same as the change file.</param>
        /// <param name="changeImagePathFile">The file to receive the change.</param>
        /// <returns>True for success.</returns>
        public bool Flip(bool vertical, string originalPathFile, string changeImagePathFile)
        {
            bool bRet = false;

            if (null == originalPathFile) return false;
            if (null == changeImagePathFile) return false;

            if (originalPathFile.Length > 0)  // may have been set at creation, so not given
                m_originalFile = originalPathFile;

            // Fix slashes - never trust that they have done this.
            m_originalFile = m_originalFile.Replace('/', '\\').Replace("\\\\", "\\");
            changeImagePathFile = changeImagePathFile.Replace('/', '\\').Replace("\\\\", "\\");

            // use cximage library instead of .Net code
            if (m_originalFile != changeImagePathFile)
            {
                // copy file to new if needed
                File.Copy(m_originalFile, changeImagePathFile, true);
                MakeWriteable(changeImagePathFile); // in case the original was readonly
            }
            ImageGalleryInterop.CImageResizerClass ig = new ImageGalleryInterop.CImageResizerClass();
            ig.FlipImageFile(changeImagePathFile, vertical ? 1 : 0);

            /*
            RotateFlipType rfAction = (vertical) ? RotateFlipType.RotateNoneFlipY : RotateFlipType.RotateNoneFlipX;

            System.Drawing.Image img = System.Drawing.Image.FromFile(m_originalFile);

            img.RotateFlip(rfAction);
            img.Save(changeImagePathFile);

            // Save for convenient use.
            m_width = img.Width;
            m_height = img.Height;
            img.Dispose();
            */
            m_localResultFile = changeImagePathFile;

            bRet = true;

            return bRet;
        }

        /// <summary>
        /// Rotates an image.
        /// </summary>
        /// <param name="rotateValue">The ImageRotation enumeration value.</param>
        /// <param name="originalPathFile">Source file, could be the same as the change file.</param>
        /// <param name="changeImagePathFile">The file to receive the change.</param>
        /// <returns>True for success.</returns>
        public bool Rotate(ImageRotation rotateValue, string originalPathFile, string changeImagePathFile)
        {
            bool bRet = false;

            if (null == originalPathFile) return false;
            if (null == changeImagePathFile) return false;

            if (originalPathFile.Length > 0)  // may have been set at creation, so not given
                m_originalFile = originalPathFile;

            // Fix slashes - never trust that they have done this.
            m_originalFile = m_originalFile.Replace('/', '\\').Replace("\\\\", "\\");
            changeImagePathFile = changeImagePathFile.Replace('/', '\\').Replace("\\\\", "\\");

            // use cximage library instead of .Net code
            if (m_originalFile != changeImagePathFile)
            {
                // copy file to new if needed
                File.Copy(m_originalFile, changeImagePathFile, true);
                MakeWriteable(changeImagePathFile); // in case the original was readonly
            }
            ImageGalleryInterop.CImageResizerClass ig = new ImageGalleryInterop.CImageResizerClass();
            int angle = 0;
            switch (rotateValue)
            {
                case ImageRotation.Half180:
                    angle = 180;
                    break;
                case ImageRotation.Left90:
                    angle = -90;
                    break;
                case ImageRotation.Right90:
                    angle = 90;
                    break;
            }
            ig.RotateImageFile(changeImagePathFile, angle);

            /*
            RotateFlipType rfAction = RotateFlipType.Rotate90FlipNone;

            System.Drawing.Image img = System.Drawing.Image.FromFile(m_originalFile);
            
            switch (rotateValue)
            {
                case ImageRotation.None:
                    break;

                case ImageRotation.Half180:
                    rfAction = RotateFlipType.Rotate180FlipNone;
                    break;

                case ImageRotation.Left90:
                    rfAction = RotateFlipType.Rotate270FlipNone;
                    break;

                case ImageRotation.Right90:
                    rfAction = RotateFlipType.Rotate90FlipNone;
                    break;
            }

            img.RotateFlip(rfAction);
            
            img.Save(changeImagePathFile);

            // Save for convenient use.
            m_width = img.Width;
            m_height = img.Height;
            img.Dispose();
            */

            m_localResultFile = changeImagePathFile;

            bRet = true;

            return bRet;
        }

        /// <summary>
        /// Adds text to an image.
        /// </summary>
        /// <param name="textX">Horizontal pixel location.</param>
        /// <param name="textY">Vertical pixel location.</param>
        /// <param name="imageText">The text to add.</param>
        /// <param name="fontName">The text font name.</param>
        /// <param name="fontSizePoint">The point size.</param>
        /// <param name="useBold">True for bold.</param>
        /// <param name="useItal">True for italic.</param>
        /// <param name="originalPathFile">Source file, could be the same as the change file.</param>
        /// <param name="changeImagePathFile">The file to receive the change.</param>
        /// <returns>True for success.</returns>
        public bool Text(int textX, int textY, string imageText, 
                    string fontName, int fontSizePoint, bool useBold, bool useItal,
                    string originalPathFile, string changeImagePathFile)
        {
            bool bRet = false;

            string strUseFontName = "";
            int iUseFontSize = 0;
            FontStyle fxStyle = FontStyle.Regular;

            if (null == originalPathFile) return false;
            if (null == changeImagePathFile) return false;
            if (null == imageText) return false;
            if (null == fontName) return false;

            if (originalPathFile.Length > 0)  // may have been set at creation, so not given
                m_originalFile = originalPathFile;

            if (useBold)
            {
                fxStyle = FontStyle.Bold;
            }
            else if (useItal)
            {
                fxStyle = FontStyle.Italic;
            }

            if (0 == fontName.Length)
            {
                strUseFontName = "verdana";
            }
            else
            {
                strUseFontName = fontName;
            }
            if (0 == fontSizePoint)
            {
                iUseFontSize = 12;
            }
            else
            {
                iUseFontSize = fontSizePoint;
            }

            // Fix slashes - never trust that they have done this.
            m_originalFile = m_originalFile.Replace('/', '\\').Replace("\\\\", "\\");
            changeImagePathFile = changeImagePathFile.Replace('/', '\\').Replace("\\\\", "\\");

            /*
            // use cximage library instead of .Net code
            if (m_originalFile != changeImagePathFile)
            {
                // copy file to new if needed
                File.Copy(m_originalFile, changeImagePathFile, true);
                MakeWriteable(changeImagePathFile); // in case the original was readonly
            }
            ImageGalleryInterop.CImageResizerClass ig = new ImageGalleryInterop.CImageResizerClass();
            ig.DrawTextOnImageFile(changeImagePathFile, textX, textY, imageText,
                strUseFontName, iUseFontSize, useBold ? 1 : 0, useItal ? 1 : 0, 0);
            */

            System.Drawing.Image img = System.Drawing.Image.FromFile(m_originalFile);

            Bitmap bmpImage = new Bitmap(img);
            img.Dispose();  // Free up the file

            Graphics gphText = Graphics.FromImage(bmpImage);
            gphText.SmoothingMode = SmoothingMode.AntiAlias;
            gphText.DrawString(imageText, new System.Drawing.Font(strUseFontName, iUseFontSize, fxStyle), SystemBrushes.WindowText, textX, textY);

            bmpImage.Save(changeImagePathFile);

            // Save for convenient use.
            m_width = bmpImage.Width;
            m_height = bmpImage.Height;

            bmpImage.Dispose();
            gphText.Dispose();

            m_localResultFile = changeImagePathFile;

            bRet = true;

            return bRet;
        }

        /// <summary>
        /// Crops an image.
        /// </summary>
        /// <param name="cropStartX">The horizontal start point in pixels.</param>
        /// <param name="cropStartY">The vertical start point.</param>
        /// <param name="cropWidth">The width of the crop, in pixels.</param>
        /// <param name="cropHeight">The hight of the crop.</param>
        /// <param name="originalPathFile">Source file, could be the same as the change file.</param>
        /// <param name="changeImagePathFile">The file to receive the change.</param>
        /// <returns>True for success.</returns>
        public bool Crop(int cropStartX, int cropStartY, int cropWidth, int cropHeight, 
                    string originalPathFile, string changeImagePathFile)
        {
            bool bRet = false;

            if (null == originalPathFile)  return false;
            if (null == changeImagePathFile) return false;

            if (originalPathFile.Length > 0)  // may have been set at creation, so not given
                m_originalFile = originalPathFile;

            // Fix slashes - never trust that they have done this.
            m_originalFile = m_originalFile.Replace('/', '\\').Replace("\\\\", "\\");
            changeImagePathFile = changeImagePathFile.Replace('/', '\\').Replace("\\\\", "\\");

            // use cximage library instead of .Net code
            if (m_originalFile != changeImagePathFile)
            {
                // copy file to new if needed
                File.Copy(m_originalFile, changeImagePathFile, true);
                MakeWriteable(changeImagePathFile); // in case the original was readonly
            }
            ImageGalleryInterop.CImageResizerClass ig = new ImageGalleryInterop.CImageResizerClass();
            ig.CropImageFile(changeImagePathFile, cropStartX, cropStartY, cropWidth, cropHeight);

            /*
            System.Drawing.Image img;
            img = System.Drawing.Image.FromFile(m_originalFile);

            Bitmap bmpImage = new Bitmap(img);
            img.Dispose();  // Free up the file

            Rectangle recCrop = new Rectangle(cropStartX, cropStartY,
               cropWidth, cropHeight);
            Bitmap bmpCrop = new Bitmap
               (cropWidth, cropHeight,
               bmpImage.PixelFormat);
            Graphics gphCrop = Graphics.FromImage(bmpCrop);
            Rectangle recDest = new Rectangle(0, 0,
               cropWidth, cropHeight);

            gphCrop.DrawImage(bmpImage, recDest,
               recCrop.X, recCrop.Y,
               recCrop.Width, recCrop.Height,
               GraphicsUnit.Pixel);

            bmpCrop.Save(changeImagePathFile);
            bmpImage.Dispose();
            bmpCrop.Dispose();
            gphCrop.Dispose();
            */

            // Save for convenient use.
            m_width = cropWidth;
            m_height = cropHeight;
            m_localResultFile = changeImagePathFile;

            bRet = true;

            return bRet;
        }

        /// <summary>
        /// Resizes an image.  This is the actual image size, not the display size.
        /// </summary>
        /// <param name="resizeWidth">Width in pixels.</param>
        /// <param name="resizeHeight">Height in pixels.</param>
        /// <param name="maintainAspect">Modify parameters to maintain the aspect ratio.</param>
        /// <param name="originalPathFile">Source file, could be the same as the change file.</param>
        /// <param name="changeImagePathFile">The file to receive the change.</param>
        /// <returns>True for success.</returns>
        public bool Resize(int resizeWidth, int resizeHeight, bool maintainAspect, string originalPathFile,
                    string changeImagePathFile)
        {
            bool bRet = false;

            if (null == originalPathFile) return false;
            if (null == changeImagePathFile) return false;

            UseImage(originalPathFile);     // get current image width/height
            if ((m_width == 0) || (m_height == 0)) return false;
            
            // OK, users will enter in garbage, since they don't know.
            // Let's help them out.
            // (Negative numbers are seen as 0.)
            if ((0 >= resizeWidth) || (0 >= resizeHeight))
            {
                maintainAspect = true;
            }
            if ((0 >= resizeWidth) && (0 >= resizeHeight))
            {
                resizeWidth = 32;  // default
            }

            if (maintainAspect)
            {
                // We need to be sure that our calculations do not overflow.
                double dOx, dOy, dRx, dRy;
                dOx = m_width;
                dOy = m_height;
                dRy = resizeHeight;
                dRx = resizeWidth;

                if (0 >= resizeWidth)
                {
                    //resizeWidth = (resizeHeight * img.Width) / img.Height;
                    //dRx = (dRy * dOx) / dOy;
                    resizeWidth = Convert.ToInt32((dRy * dOx) / dOy);
                }
                else
                {
                    //resizeHeight = (resizeWidth * img.Height) / img.Width;
                    //dRy = (dRx * dOy) / dOx;
                    resizeHeight = Convert.ToInt32((dRx * dOy) / dOx);
                }
            }

            if (originalPathFile.Length > 0)  // may have been set at creation, so not given
                m_originalFile = originalPathFile;

            // Fix slashes - never trust that they have done this.
            m_originalFile = m_originalFile.Replace('/', '\\').Replace("\\\\", "\\");
            changeImagePathFile = changeImagePathFile.Replace('/', '\\').Replace("\\\\", "\\");

            // use cximage library instead of .Net code
            if (m_originalFile != changeImagePathFile)
            {
                // copy file to new if needed
                File.Copy(m_originalFile, changeImagePathFile, true);
                MakeWriteable(changeImagePathFile); // in case the original was readonly
            }
            ImageGalleryInterop.CImageResizerClass ig = new ImageGalleryInterop.CImageResizerClass();
            ig.ResizeImageFile(changeImagePathFile, resizeWidth, resizeHeight);

            /*
            System.Drawing.Image img = System.Drawing.Image.FromFile(m_originalFile);

            System.Drawing.Image.GetThumbnailImageAbort imgCallBack = new System.Drawing.Image.GetThumbnailImageAbort(ImageResizeCallback);

            System.Drawing.Image imgThumbnail = img.GetThumbnailImage(resizeWidth, resizeHeight, imgCallBack, IntPtr.Zero);

            img.Dispose();  // Free up the file
            imgThumbnail.Save(changeImagePathFile);

            imgThumbnail.Dispose();
            */

            // Save for convenient use.
            m_width = resizeWidth;
            m_height = resizeHeight;
            m_localResultFile = changeImagePathFile;

            RetrieveFileSize();

            bRet = true;

            return bRet;
        }

        #endregion //  Public Methods

        #region Protected Methods  ==============================================

        static protected bool ImageResizeCallback()
        {
            // Cool
            return false;
        }

        private void LoadSelectedImageInformation()
        {
            RetrieveFileSize();

            // use cximage library instead of asp.net routines
            ImageGalleryInterop.CImageResizerClass ig = new ImageGalleryInterop.CImageResizerClass();
            ig.GetImageDimensions(m_originalFile, out m_width, out m_height, out m_depth);

            /*
            System.Drawing.Image img = System.Drawing.Image.FromFile(m_originalFile);

            PixelFormat pixFormat;
            pixFormat = img.PixelFormat;
            m_depth = Image.GetPixelFormatSize(pixFormat);

            m_width = img.Width;
            m_height = img.Height;

            img.Dispose();
            */
        }

        private void RetrieveFileSize()
        {
            string strFileName = Path.GetFileName(m_originalFile).ToLower(m_culture);
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(m_originalFile));
            FileInfo[] fiArr = di.GetFiles(strFileName);
            foreach (FileInfo f in fiArr)
            {
                if (String.Compare(f.Name, strFileName, true, m_culture) == 0)
                {
                    m_fileSize = f.Length;
                    return;
                }
            }
            m_fileSize = 0;
        }

        private void MakeWriteable(string filename)
        {
            FileInfo file = new FileInfo(filename);
            if (file.IsReadOnly)
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
            }
        }

        #endregion //  Protected Methods

    }
}

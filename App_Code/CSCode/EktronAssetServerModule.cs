//-----------------------------------------------------------------------
// <copyright file="EktronAssetServerModule.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;

/// <summary>
/// EktronAssetServerModule - dynamically serve the managed asset files according to 
/// the current Content Language.
/// </summary>
public class EktronAssetServerModule : IHttpModule
{
    /// <summary>
    /// number of date to be cached
    /// </summary>
    private int cacheDays = 1;
    
    /// <summary>
    /// Initializes a new instance of the EktronAssetServerModule class.
    /// </summary>
    public EktronAssetServerModule()
    { 
    }

    #region IHttpModule Members
    /// <summary>
    /// Dispose of the HttpModule
    /// </summary>
    public void Dispose()
    { 
    }

    /// <summary>
    /// Initializes the HttpModule
    /// </summary>
    /// <param name="context">current HttpApplication object</param>
    public void Init(HttpApplication context)
    {
        context.BeginRequest += new EventHandler(this.Context_BeginRequest);
    }

    /// <summary>
    /// the begin request event
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event arguments</param>
    private void Context_BeginRequest(object sender, EventArgs e)
    {
        if (Ektron.ASM.EkHttpDavHandler.Utilities.IsAssetFile(HttpContext.Current))
        {
            long contentId = this.GetDynamicFileId(HttpContext.Current);
            if (contentId > 0)
            {
                this.WriteAssetToResponse(contentId, HttpContext.Current);
            }
        }
    }

    #endregion

    #region Private Members
    /// <summary>
    /// Configurate the content id from the current HttpContext Request object
    /// </summary>
    /// <param name="context">current HttpContext object</param>
    /// <returns>content id</returns>
    private long GetDynamicFileId(HttpContext context)
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(context.Request.PhysicalPath);
        long id = 0;
        Int64.TryParse(fileName, out id); // all assets type e.g. files, images
        return id;
    }

    /// <summary>
    /// Configurate the Asset file by its content id to find out if the image needed to 
    /// be created before calling the Response object, bypassing IIS
    /// </summary>
    /// <param name="contentId">content id associated to the asset</param>
    /// <param name="context">current HttpContext object</param>
    private void WriteAssetToResponse(long contentId, HttpContext context)
    {
        string assetPath = string.Empty;
        Ektron.Cms.ContentAPI capi = new Ektron.Cms.ContentAPI();
        int lang = capi.RequestInformationRef.ContentLanguage;
        int langBak = lang;
        if (context.Request.QueryString["langType"] != null)
        {
            Int32.TryParse(context.Request.QueryString["langType"], out lang);
        }

        bool createImg = false;
        int size = 0;
        int width = 0;
        int height = 0;
        bool libraryThumbnail = false;
        string sz = context.Request.QueryString["sz"];
        if (sz != null)
        {
            if (true == Int32.TryParse(sz, out size))
            {
                createImg = true;
            }
            else
            {
                libraryThumbnail = ("thumb" == sz.ToLower());
                size = 120; // default library thumbnail size
            }
        }

        string wd = context.Request.QueryString["wd"];
        if (wd != null)
        {
            Int32.TryParse(wd, out width);
        }

        string ht = context.Request.QueryString["ht"];
        if (ht != null)
        {
            Int32.TryParse(ht, out height);
        }

        if (width > 0 || height > 0)
        {
            createImg = true;
        }

        try
        {
            // GetLibraryItemByContentID() figures out their fallback language in the database L10n_GetContentLanguage
            capi.RequestInformationRef.ContentLanguage = lang;
            Ektron.Cms.LibraryData data = capi.GetLibraryItemByContentID(contentId);
            if (data != null)
            {
                string assetFileName = data.FileName;
                string assetFilePath = context.Server.MapPath(assetFileName);

                // This is too expensive to get the friendly name with the following method. It will be updated when new
                // API is availabe to get asset id, asset GUID name and asset handle with other library properties. Currently,
                // the GUID name is in used as the display name when user right-clicks on image and select "save image as".
                //string assetId = System.IO.Path.GetFileNameWithoutExtension(assetFileName);
                //AssetManagement.AssetManagementService assetmanagementService = new AssetManagement.AssetManagementService();
                //Ektron.ASM.AssetConfig.AssetData assetDetails = assetmanagementService.GetAssetData(assetId);
                //string displayFileName = assetDetails.Handle;
                string displayFileName = System.IO.Path.GetFileName(assetFileName);

                Ektron.Cms.API.ImageHelper g = new Ektron.Cms.API.ImageHelper(assetFileName);
                if ("images" == data.Type && (true == createImg || true == libraryThumbnail))
                {
                    string imageFileName = EkFunctions.GetThumbnailForContent(assetFileName);
                    imageFileName = System.IO.Path.ChangeExtension(imageFileName, "png");
                    assetFilePath = context.Server.MapPath(imageFileName);


                    if (true == libraryThumbnail)
                    {
                        try
                        {
                            // serve the previously created thumbnail image  
                            g.WriteExistingFileToResponse(assetFilePath, displayFileName, data.DateCreated, context.Response);
                        }
                        catch (FileNotFoundException)
                        {
                            // create the thumbnail for its first time
                            this.CreateImage(g, imageFileName, size, width, height, true);
                            g.WriteNewFileToResponse(displayFileName, context.Response, this.cacheDays);
                        }
                    }
                    else
                    {
                        // create the customized image
                        this.CreateImage(g, imageFileName, size, width, height, false);
                        g.WriteNewFileToResponse(displayFileName, context.Response, this.cacheDays);
                    }
                }
                else
                {
                    // serve the original asset
                    g.WriteExistingFileToResponse(assetFilePath, displayFileName, data.DateCreated, context.Response);
                }

                g = null;
            }
        }
        finally
        {
            capi.RequestInformationRef.ContentLanguage = langBak;
        }  
    }

    /// <summary>
    /// calculate the width and height of the destination image, and call the imagehelper 
    /// to crop and/or resize to produce the new image.The new image will remain in 
    /// temporary memory for response object.It can also be saved to the file system.
    /// </summary>
    /// <param name="imgHelper">instance of imagehelper object</param>
    /// <param name="destName">destination filename</param>
    /// <param name="size">max bound of the dimension, for use in resizing image</param>
    /// <param name="width">width of new image</param>
    /// <param name="height">height of new image</param>
    /// <param name="writeToFileSystem">boolean to indicate if the new image file need to be saved to the file system</param>
    private void CreateImage(Ektron.Cms.API.ImageHelper imgHelper, string destName, int size, int width, int height, bool writeToFileSystem)
    {
        if (width > 0 && height > 0)
        {
            imgHelper.CropAndResizeImage(width, height);
        }
        else
        {
            if (width > 0)
            {
                height = imgHelper.SrcPhoto.Height * width / imgHelper.SrcPhoto.Width;
            }
            else if (height > 0)
            {
                width = imgHelper.SrcPhoto.Width * height / imgHelper.SrcPhoto.Height;
            }
            else if (imgHelper.SrcPhoto.Width > imgHelper.SrcPhoto.Height)
            {
                width = size;
                height = imgHelper.SrcPhoto.Height * size / imgHelper.SrcPhoto.Width;
            }
            else
            {
                width = imgHelper.SrcPhoto.Width * size / imgHelper.SrcPhoto.Height;
                height = size;
            }

            imgHelper.ResizeImage(width, height);
        }

        if (writeToFileSystem)
        {
            imgHelper.WriteToFileSystem(HttpContext.Current.Server.MapPath(destName)); 
        }
    }
    #endregion
}

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
using System.Collections.Generic;
using Ektron.Cms.Commerce;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;

namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.MediaData
{
    public class MediaImageData
    {
        private long _Id;
        private string _Title;
        private string _AltText;
        private string _Path;
        private bool _MarkedForDelete;
        private bool _Default;
        private string _width;
        private string _height;
        private bool _Gallery;
        private List<MediaThumbnailImageData> _Thumbnails;

        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string AltText
        {
            get
            {
                return _AltText;
            }
            set
            {
                _AltText = value;
            }
        }
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }
        public bool Default
        {
            get
            {
                return _Default;
            }
            set
            {
                _Default = value;
            }
        }
        public bool MarkedForDelete
        {
            get
            {
                return _MarkedForDelete;
            }
            set
            {
                _MarkedForDelete = value;
            }
        }
        public string Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }
        public string Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        public bool Gallery
        {
            get
            {
                return _Gallery;
            }
            set
            {
                _Gallery = value;
            }
        }
        public List<MediaThumbnailImageData> Thumbnails
        {
            get
            {
                return _Thumbnails;
            }
            set
            {
                _Thumbnails = value;
            }
        }
    }

    public class MediaThumbnailImageData
    {
        private string _ImageName;
        private string _Path;
        private string _Title;

        public string ImageName
        {
            get
            {
                return _ImageName;
            }
            set
            {
                _ImageName = value;
            }
        }
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Medias
{
    public partial class Media : System.Web.UI.UserControl
    {
        #region Member Variables

        private bool _IsEditable;
        private EntryData _EntryEditData;
        private List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.MediaData.MediaImageData> _ImageData;
        private DisplayModeValue _DisplayMode;
        private ContentAPI _contentApi;
        protected SiteAPI _siteApi;
        private String _sitePath;
        private String _applicationPath;
        private long _FolderId;
        private long _ProductId;
        private string _ProductTypeId;
        private string _ThumnbailAltText;
        private string _ThumnbailTitleText;
        Ektron.Cms.Common.EkMessageHelper _msgHelper;

        #endregion

        #region Enumerations

        public enum DisplayModeValue
        {
            View,
            Edit
        }

        #endregion

        #region Properties

        public long FolderId
        {
            get
            {
                return _FolderId;
            }
            set
            {
                _FolderId = value;
            }
        }
        
        public long ProductId
        {
            get
            {
                return _ProductId;
            }
            set
            {
                _ProductId = value;
            }
        }
        
        public DisplayModeValue DisplayMode
        {
            get
            {
                return _DisplayMode;
            }
            set
            {
                _DisplayMode = value;
            }
        }

        public List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.MediaData.MediaImageData> ImageData
        {
            get
            {
                return _ImageData;
            }
        }

        public EntryData EntryEditData
        {
            get
            {
                return _EntryEditData;
            }
            set
            {
                _EntryEditData = value;
            }

        }

        public bool IsEditable
        {
            get
            {
                return _IsEditable;
            }
            set
            {
                _IsEditable = value;
            }
        }
        
        protected String SitePath
        {
            get
            {
                return _sitePath;
            }
            set
            {
                _sitePath = value;
            }
        }

        protected String ApplicationPath
        {
            get
            {
                return _applicationPath;
            }
            set
            {
                _applicationPath = value;
            }
        }

        private string ThumnbailAltText
        {
            get
            {
                return _ThumnbailAltText;
            }
            set
            {
                _ThumnbailAltText = value;
            }
        }

        private string ThumnbailTitleText
        {
            get
            {
                return _ThumnbailTitleText;
            }
            set
            {
                _ThumnbailTitleText = value;
            }
        }

        #endregion

        #region Constructor

        protected Media()
        {
            _contentApi = new ContentAPI();
            _siteApi = new SiteAPI();
            _ImageData = null;

            this.SitePath = _contentApi.SitePath.TrimEnd(new char[] { '/' });
            this.ApplicationPath = _siteApi.ApplicationPath.TrimEnd(new char[] { '/' });
        }

        #endregion

        #region OnInit, Page_Load, rptImage_ItemDataBound

        protected override void OnInit(EventArgs e)
        {
            string mediaTabJsonData = Request.Form["MediaTabData"] ?? String.Empty;
            if (mediaTabJsonData != String.Empty)
            {
                _ImageData = (List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.MediaData.MediaImageData>)JsonConvert.DeserializeObject(mediaTabJsonData, typeof(List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.MediaData.MediaImageData>));
            }

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.IsEditable = (this.DisplayMode == DisplayModeValue.Edit) ? true : false;
            
            //show modal if editable
            phModal.Visible = this.IsEditable == true ? true : false;
            phCloneItem.Visible = this.IsEditable == true ? true : false;
            
            //show add image button only if editable
            phAddImageEdit.Visible = this.IsEditable == true ? true : false;

            //show no image placeholder if no images are present
            if (this.EntryEditData != null)
            {
                phNoImages.Visible = EntryEditData.Media.Images.Count == 0 ? true : false;
            }
            else
            {
                phNoImages.Visible = false;
            }
            litNoImages.Text = "No Images Exist";
            lbl_AddNewImage.Text = GetAddNewImageLabel();
            lbl_AddImage.Text = GetAddLibraryImageLabel();

            if (EntryEditData != null)
            {
                _ProductId = EntryEditData.Id;
                _ProductTypeId = String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["xid"]) ? EntryEditData.ProductType.Id.ToString() : HttpContext.Current.Request.QueryString["xid"];
                int lang = (_contentApi.ContentLanguage > 0 ? _contentApi.ContentLanguage : _contentApi.DefaultContentLanguage);
                inputAddImageIframeSrc.Value = string.Format(
                    "{3}/Commerce/CatalogEntry/Media/AddImage.aspx?LangType={0}&catalogid={1}&productTypeId={2}",
                    lang,
                    this.FolderId,
                    _ProductTypeId,
                    this.ApplicationPath
                    );
                rptImage.DataSource = EntryEditData.Media.Images;
                rptImage.DataBind();
            }

            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void rptImage_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                this.ThumnbailAltText = ((ImageMediaData)e.Item.DataItem).Alt;
                this.ThumnbailTitleText = ((ImageMediaData)e.Item.DataItem).Title;
                bool hasThumbnails = ((ImageMediaData)e.Item.DataItem).Thumbnails.Count > 0 ? true : false;

                //handle attribute placeholders
                
                e.Item.FindControl("phImageDataEdit").Visible = this.IsEditable == true ? true : false;
                e.Item.FindControl("phDefaultButtonEdit").Visible = this.IsEditable == true ? true : false;
                e.Item.FindControl("phDeleteButtonEdit").Visible = this.IsEditable == true ? true : false;
                e.Item.FindControl("phTitleEdit").Visible = this.IsEditable == true ? true : false;
                e.Item.FindControl("phTitleView").Visible = this.IsEditable == true ? false : true;
                e.Item.FindControl("phAltTextEdit").Visible = this.IsEditable == true ? true : false;
                e.Item.FindControl("phAltTextView").Visible = this.IsEditable == true ? false : true;
                e.Item.FindControl("phPathEdit").Visible = this.IsEditable == true ? true : false;
                e.Item.FindControl("phGalleryEdit").Visible = this.IsEditable == true ? true : false;
                e.Item.FindControl("phGalleryView").Visible = this.IsEditable == true ? false : true;

                //handle thumbnails
                PlaceHolder thumbnailPlaceholder = (PlaceHolder)e.Item.FindControl("phThumbnails");
                HtmlAnchor viewThumbnailButton = (HtmlAnchor)e.Item.FindControl("aThumbnails");
                if (hasThumbnails == true)
                {
                    //image has thumbnails
                    Repeater thumbnailRepeater = (Repeater)e.Item.FindControl("rptThumbnail");
                    List<ThumbnailData> thumbnails = ((ImageMediaData)e.Item.DataItem).Thumbnails;
                    thumbnailRepeater.DataSource = thumbnails;
                    thumbnailRepeater.DataBind();
                }
                else
                {
                    //image does not have thumbnails
                    thumbnailPlaceholder.Visible = false;
                    viewThumbnailButton.Visible = false;
                }
            }
        }

        public string FixCacheImage(Object e)
        { 
            Random rand = new Random();
            return Convert.ToString(e) + "?r=" + rand.Next(0, 1000);
        }

        public string GetIsDefaultImageClass(Object e)
        { 
            return Convert.ToString(e).Replace(this.SitePath + "/", String.Empty).ToLower() == this.EntryEditData.Image.Replace(this.SitePath + "/", String.Empty).ToLower() ? " defaultImage" : String.Empty;
        }

        public string GetIsDefaultImageField(Object e)
        {
            return Convert.ToString(e).Replace(this.SitePath + "/", String.Empty).ToLower() == this.EntryEditData.Image.Replace(this.SitePath + "/", String.Empty).ToLower() ? "true" : "false";
        }

        public string GetIsDefaultImageLabel(Object e)
        {
            return Convert.ToString(e).Replace(this.SitePath + "/", String.Empty).ToLower() == this.EntryEditData.Image.Replace(this.SitePath + "/", String.Empty).ToLower() ? "Unset as Product Icon" : "Set As Product Icon";
        }

        public string GetGalleryLabel(Object e)
        {
            return (bool)e == true ? "Yes" : "No";
        }

        public string GetSetDefaultLabel()
        {
            return "Set As Product Icon";
        }

        public string GetSitePath()
        {
            return this.SitePath;
        }

        public string GetSelectedDisplay(bool option, Object e)
        {
            return (option == (bool)e) ? @"selected=""selected""" : String.Empty;
        }

        public string GetThumbnailImageAltText(Object e)
        {
            ThumbnailData thumbnail = (ThumbnailData)e;
            return (thumbnail.Alt == String.Empty) ? thumbnail.FileName : thumbnail.Alt;
        }

        public string GetThumbnailImageTitle(Object e)
        {
            ThumbnailData thumbnail = (ThumbnailData)e;
            return (thumbnail.Title == String.Empty) ? thumbnail.FileName : thumbnail.Title;
        }

        public string GetThumbnailImageTitleData(Object e)
        {
            ThumbnailData thumbnail = (ThumbnailData)e;
            return (thumbnail.Title == String.Empty) ? String.Empty : thumbnail.Title;
        }

        public string GetThumbnailImagePath(Object e)
        {
            string thumbnailPath = e.ToString();
            if (thumbnailPath.IndexOf("/") == 0 && this.SitePath == "" && thumbnailPath.Length > 1)
                thumbnailPath = thumbnailPath.Substring(1);
            return this.SitePath + "/" + System.IO.Path.GetDirectoryName(thumbnailPath);
        }

        public string GetThumbnailImageHeight(Object e)
        {
            return EkFunctions.ReadIntegerValue(e).ToString();
        }

        public string GetThumbnailImageWidth(Object e)
        {
            return EkFunctions.ReadIntegerValue(e).ToString();
        }

        public string GetThumbnailPath(Object e)
        {
            string image = e.ToString();
            // correct faulty URLs:
            image = image.Replace("//", "/");
            image = image.Replace("http:/", "http://");
            image = image.Replace("https:/", "https://");
            // don't prefix base-path if already present (occurs with multisite):
            if (image.Contains("http://") || image.Contains("https://"))
                return image;
            return Page.Request.Url.Scheme + "://" + _contentApi.RequestInformationRef.HostUrl + this.SitePath + "/" + image;
        }

        protected string GetMessage(string key)
        {
            if (null == _msgHelper){
                SiteAPI siteApi = new SiteAPI();
                _msgHelper = siteApi.EkMsgRef;
            }
            return _msgHelper.GetMessage(key);
        }

        public string GetAddNewImageLabel()
        {
            return GetMessage("lbl add new image");
        }

        public string GetAddLibraryImageLabel()
        {
            return GetMessage("lbl add library image");
        }

        #endregion

        #region Helpers

        public string GetProductTypeId()
        {
            return _ProductTypeId.ToString();
        }

        #endregion

        #region JS, CSS, Images

        public string GetMediaImageFilePath(){
            return this.ApplicationPath + "/Commerce/CatalogEntry/Media/css/images";
        }

        private void RegisterJS()
        {
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/Media/js/Media.js", "EktronCommerceMediaJs");
            JS.RegisterJS(this, JS.ManagedScript.EktronUICoreJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronUISortableJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronBlockUiJS);
        }

        private void RegisterCSS()
        {
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/CatalogEntry/Media/css/Media.css", "EktronMediaCss");
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/CatalogEntry/Media/css/Media.ie6.css", "EktronMediaIe6Css", Css.BrowserTarget.LessThanEqualToIE6);
        }

        public string GetMediaImageImagePath()
        {
            return this.ApplicationPath + "/Commerce/CatalogEntry/Media/css/images";
        }

        #endregion
    }
}
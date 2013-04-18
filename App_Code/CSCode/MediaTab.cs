using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Ektron.Cms.Workarea.MediaTab
{
    public class Image {
        private long _Id;
        private string _Title;
        private string _AltText;
        private string _Path;
        private bool _MarkedForDelete;
        private bool _Default;
        private string _width;
        private string _height;
        private List<string> _thumbnailList = new List<string>();

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
        public List<string> Thumbnails
        {
            get
            {
                return _thumbnailList;
            }
            set
            {
                _thumbnailList = value;
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
    }
}

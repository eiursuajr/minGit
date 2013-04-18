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
using Ektron.Cms.API;
using Ektron.Newtonsoft.Json;
using System.Collections.Generic;
using Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items;
using Ektron.Cms.Commerce.Subscriptions;

namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Subscriptions
{
    public class MembershipData
    {
        #region Member Variables

        private string _GroupType;
        private long _GroupId;
        private bool _MarkedForDelete;

        public string GroupType
        {
            get
            {
                return _GroupType;
            }
            set
            {
                _GroupType = value;
            }
        }
        public long GroupId
        {
            get
            {
                return _GroupId;
            }
            set
            {
                _GroupId = value;
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

        #endregion
    }

    public partial class Membership : System.Web.UI.UserControl, IEktronSubscriptionControl
    {
        #region Member Variables

        private bool _IsEditable;
        private string _SitePath;
        private string _ApplicationPath;
        private List<MembershipData> _ClientData;
        private Object _SubscriptionData;
        private ContentAPI _ContentApi;
        private SiteAPI _SiteApi;

        #endregion

        #region Properties

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

        public List<MembershipData> ClientData
        {
            get
            {
                return _ClientData;
            }
            set
            {
                _ClientData = value;
            }
        }

        public Object SubscriptionData
        {
            get
            {
                return _SubscriptionData;
            }
            set
            {
                _SubscriptionData = value;
            }
        }

        protected String SitePath
        {
            get
            {
                return _SitePath;
            }
            set
            {
                _SitePath = value;
            }
        }

        protected String ApplicationPath
        {
            get
            {
                return _ApplicationPath;
            }
            set
            {
                _ApplicationPath = value;
            }
        }

        #endregion

        #region Constructor

        protected Membership()
        {
            _ContentApi = new ContentAPI();
            _SiteApi = new SiteAPI();

            this.SitePath = _ContentApi.SitePath.TrimEnd(new char[] { '/' });
            this.ApplicationPath = _SiteApi.ApplicationPath.TrimEnd(new char[] { '/' });
        }

        #endregion

        #region Init, Load

        protected void Page_Init(object sender, EventArgs e)
        {
            string subscriptionMembershipData = Request.Form["SubscriptionMembershipData"] ?? String.Empty;
            if (subscriptionMembershipData != String.Empty)
            {
                _ClientData = (List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Subscriptions.MembershipData>)JsonConvert.DeserializeObject(subscriptionMembershipData, typeof(List<Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Subscriptions.MembershipData>));
                List<Object> clientData = new List<Object>();
                foreach (MembershipData membershipData in this.ClientData)
                {
                    clientData.Add((Object)membershipData);
                }
                ((Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Items.Item)this.Parent).SetClientData(clientData);
            }

            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //set display mode - view or edit
            this.SetDisplayMode();

            //set image paths
            this.SetImagePaths();

            if (!(this.SubscriptionData is MembershipSubscriptionInfo))
                this.SubscriptionData = new MembershipSubscriptionInfo();

            string cmsGroupName = (((MembershipSubscriptionInfo)this.SubscriptionData).AuthorGroupName) == String.Empty ? "Not Set" : ((MembershipSubscriptionInfo)this.SubscriptionData).AuthorGroupName;
            string memberGroupName = (((MembershipSubscriptionInfo)this.SubscriptionData).MemberGroupName) == String.Empty ? "Not Set" : ((MembershipSubscriptionInfo)this.SubscriptionData).MemberGroupName;
            spanCmsGroupView.InnerText = cmsGroupName;
            spanMemberGroupView.InnerText = memberGroupName;
        }

        #endregion

        #region Helpers

        private void SetDisplayMode()
        {
            //display modal only if IsEditable is true
            phModal.Visible = this.IsEditable;

            //set multiviews to either edit or view
            phCmsAuthorGroupEdit.Visible = this.IsEditable;
            phMemberGroupEdit.Visible = this.IsEditable;

            //site site path for iFrames
            hdnSitePath.Value = this.ApplicationPath;
        }

        public string GetImagePath()
        {
            return this.ApplicationPath + "/Commerce/CatalogEntry/Items/Subscriptions/Membership/css/images";
        }

        private void SetImagePaths()
        {
            imgCmsAuthorGroupEdit.ImageUrl = GetImagePath() + "/edit.gif";
            imgCmsAuthorGroupMarkForDelete.ImageUrl = GetImagePath() + "/markForDelete.gif";
            imgCmsAuthorGroupRestore.ImageUrl = GetImagePath() + "/restore.gif";
            imgMemberGroupEdit.ImageUrl = GetImagePath() + "/edit.gif";
            imgCloseModal.ImageUrl = GetImagePath() + "/closeButton.gif";
        }

        public string GetCmsGroupId()
        {
            return ((MembershipSubscriptionInfo)this.SubscriptionData).AuthorGroupId == 0 ? "0" : ((MembershipSubscriptionInfo)this.SubscriptionData).AuthorGroupId.ToString();
        }

        public string GetMembershipGroupId()
        {
            return ((MembershipSubscriptionInfo)this.SubscriptionData).MemberGroupId == 0 ? "0" : ((MembershipSubscriptionInfo)this.SubscriptionData).MemberGroupId.ToString();
        }

        public string GetMarkForDeleteHide(string groupType)
        {
            string returnValue = String.Empty;
            switch (groupType)
            {
                case "cms":
                    returnValue = ((MembershipSubscriptionInfo)this.SubscriptionData).AuthorGroupId == 0 ? " groupNotSet" : String.Empty;
                    break;
                case "membership":
                    returnValue = ((MembershipSubscriptionInfo)this.SubscriptionData).MemberGroupId == 0 ? " groupNotSet" : String.Empty;
                    break;
            }
            return returnValue;
        }

        #endregion

        #region CSS, JS

        private void RegisterJS()
        {
            JS.RegisterJS(this, JS.ManagedScript.EktronJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronJsonJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronModalJS);
            JS.RegisterJS(this, JS.ManagedScript.EktronDnRJS);
            JS.RegisterJS(this, this.ApplicationPath + "/Commerce/CatalogEntry/Items/Subscriptions/Membership/js/Membership.js", "EktronCommerceSubscriptionsMembershipJs");
        }

        private void RegisterCSS()
        {
            Css.RegisterCss(this, Css.ManagedStyleSheet.EktronModalCss);
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/CatalogEntry/Items/Subscriptions/Membership/css/Membership.css", "EktronCommerceSubscriptionsMembershipCss");
        }

        #endregion
    }
}
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
using Ektron.Newtonsoft.Json;
using Ektron.Cms.API;
using Ektron.Cms.Commerce.Workarea.Coupons;
using Ektron.Cms.Common;

namespace Ektron.Cms.Commerce.Workarea.Coupons
{
    public partial class Finish : CouponUserControlBase
    {
        #region Member Variables
        protected EkMessageHelper messageHelper;
        private long _CouponId;

        #endregion

        #region Properties

        public long CouponId
        {
            get
            {
                return _CouponId;
            }
            set
            {
                _CouponId = value;
            }
        }

        #endregion

        #region Events
        protected EkMessageHelper _messageHelper
        {
            get
            {
                return (messageHelper ?? (messageHelper = _ContentApi.EkMsgRef));
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            //register page components
            this.RegisterCSS();
            this.RegisterJS();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //set localized strings
            this.SetLocalizedControlText();
        }

        #endregion

        #region Helpers

        private void SetLocalizedControlText()
        {
            //header
            litFinishHeader.Text = _messageHelper.GetMessage("btn finish");

            //coupon properties link
            aProperties.ToolTip = _messageHelper.GetMessage("lbl Click view coupon properties");
            aProperties.Text = _messageHelper.GetMessage("lbl coupon");
            aProperties.NavigateUrl = "../Properties/Properties.aspx?couponId=" + this.CouponId;
            litFinishMessage.Text = " "+_messageHelper.GetMessage("lbl success add");
        }

        #endregion

        #region JS, CSS, ImagePath

        protected override void RegisterCSS()
        {
            base.RegisterCSS();
            Css.RegisterCss(this, this.ApplicationPath + "/Commerce/Coupons/Add/css/finish.css", "EktronCommerceCouponAddFinishCss");

        }

        protected override void RegisterJS()
        {
            base.RegisterJS();
        }

        public string GetAmountImagePath()
        {
            return this.ApplicationPath + "/Commerce/Coupons/Add/css/images";
        }

        #endregion
    }
}

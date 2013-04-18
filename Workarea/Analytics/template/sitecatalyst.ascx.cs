//-----------------------------------------------------------------------
// <copyright file="sitecatalyst.ascx.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web;
using System.Collections.Generic;
using Ektron.Cms;
using Ektron.Cms.Common;

/// <summary>
/// partial class as user control to add SiteCatalyst Analytics tracking code to the browser.
/// </summary>
public partial class Analytics_Template_SiteCatalyst : Ektron.Cms.Analytics.BeaconTemplateControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Ektron.Cms.CommonApi capi = new Ektron.Cms.CommonApi();
		Ektron.Cms.API.JS.RegisterJS(this, capi.AppPath + "analytics/template/s_code.js", "Analytics_Template_SiteCatalyst_JS");
        //this.AppPath.Text = capi.AppPath;
    }
}
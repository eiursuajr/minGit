//-----------------------------------------------------------------------
// <copyright file="WebTrends.ascx.cs" company="Ektron">
//     Copyright (c) Ektron, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web;

/// <summary>
/// partial class as user control to add WebTrends Analytics tracking code to the browser.
/// </summary>
public partial class Analytics_Template_WebTrends : Ektron.Cms.Analytics.BeaconTemplateControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Ektron.Cms.CommonApi capi = new Ektron.Cms.CommonApi();
        //this.AppPath.Text = capi.AppPath;
    }
}
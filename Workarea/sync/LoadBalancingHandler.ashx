<%@ WebHandler Language="C#" Class="LoadBalancingHandler" %>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

using Ektron.ASM.PluginManager;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Instrumentation;
using Ektron.Cms.DataIO.LicenseManager;
using Ektron.FileSync.Common;

/// <summary>
/// The LoadBalancingHandler provides access to sync functionality
/// required by the 'Load Balancing' control panel. This functionality
/// primarily encompasses support for manual, load balanced, sync activities.
/// </summary>
public class LoadBalancingHandler : IHttpHandler
{
    private readonly JavaScriptSerializer _serializer;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public LoadBalancingHandler()
    {
        _serializer = new JavaScriptSerializer();
    }

    #region IHttpAsyncHandler Members

    /// <summary>
    /// Processes the specified request, executing the action
    /// indicated in the query string, and writes a JSON
    /// serialized response.
    /// </summary>
    /// <param name="context">HTTP context supporting the request</param>
    public void ProcessRequest(HttpContext context)
    {
        LoadBalanceAction action = new LoadBalanceAction(context);
        action.Execute();
    }

    /// <summary>
    /// Gets a flag indicating if this HTTP handler is reusable. (Returns false)
    /// </summary>
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    #endregion
   
}
<%@ WebHandler Language="C#" Class="LoadBalancingStatusHandler" %>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

public class LoadBalancingStatusHandler : IHttpHandler
{
    private readonly JavaScriptSerializer _serializer;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public LoadBalancingStatusHandler()
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

public class LoadBalanceAction : IAsyncResult
{
    private const string ParamAction = "action";
    private const string ActionLoadBalancedSync = "loadbalance";
    private const string ActionLoadBalancedSyncStatus = "status";
    private const string ResponseContentType = "text/plain";

    private readonly LoadBalanceManager _loadBalanceManager;
    private readonly JavaScriptSerializer _serializer;

    private bool _isComplete;
    private HttpContext _context;

    public LoadBalanceAction(HttpContext context)
    {
        _loadBalanceManager = new LoadBalanceManager(context);
        _serializer = new JavaScriptSerializer();

        _isComplete = false;
        _context = context;
    }

    #region IAsyncResult Members

    public object AsyncState
    {
        get { throw new NotImplementedException(); }
    }

    public WaitHandle AsyncWaitHandle
    {
        get { return null; }
    }

    public bool CompletedSynchronously
    {
        get { return false; }
    }

    public bool IsCompleted
    {
        get { return _isComplete; }
    }

    #endregion

    public void Execute()
    {
        LoadBalanceResponse response = null;

        string action = GetParameter(_context.Request, ParamAction);
        switch (action.ToLower())
        {
            case ActionLoadBalancedSync:
                response = _loadBalanceManager.ForceLoadBalancedSync();
                break;
            case ActionLoadBalancedSyncStatus:
                response = _loadBalanceManager.GetStatus();
                break;
        }

        if (response != null)
        {
            _context.Response.ContentType = ResponseContentType;
            _context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            _context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            _context.Response.Cache.SetNoStore();
            _context.Response.Write(_serializer.Serialize(response));
        }
    }

    /// <summary>
    /// Returns the query string value of the specified parameter.
    /// </summary>
    /// <param name="request">HttpRequest providing the query string</param>
    /// <param name="param">Parameter name</param>
    /// <returns>Value of the specified parameter</returns>
    private string GetParameter(HttpRequest request, string param)
    {
        string paramValue = string.Empty;

        if (request.QueryString[param] != null)
        {
            paramValue = request.QueryString[param];
        }
        else if (request.Form[param] != null)
        {
            paramValue = request.Form[param];
        }

        return paramValue;
    } 
}
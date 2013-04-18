<%@ WebHandler Language="C#" Class="SyncStatusHandler" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Sync;
using Ektron.Cms.Sync.Client;
using Ektron.Cms.Sync.Presenters;
using Ektron.Cms.Sync.Web;
using Ektron.Cms.Sync.Web.Parameters;
using Ektron.Cms.Sync.Web.Responses;

public class SyncStatusHandler : IHttpHandler {

    private const string ResponseContentType = "text/plain";
    private const string StatusCodeSyncPerformingLoadBalance = "syncperformingloadbalance";
    private const string StatusCodeSyncCheckingLoadBalanceEnded = "synccheckingloadbalanceend";
    private const string StatusCodeSyncDatabaseStarted = "syncdatabasestarted";
    private const string StatusCodeSyncDatabaseEnded = "syncdatabaseended";
    private const string StatusCodeSyncEnded = "syncended";
    private const string DatabaseSyncDetailsFormat = "{0} {1}{2}";
    
    private SyncHandlerController _controller;
    private JavaScriptSerializer _serializer;
    private SiteAPI _siteApi;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public SyncStatusHandler()
    {
        _controller = new SyncHandlerController();
        _serializer = new JavaScriptSerializer();
        _siteApi = new SiteAPI();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest(HttpContext context)
    {
        ResponseBase response = null;

        SyncHandlerParameters parameters = new SyncHandlerParameters(context.Request);
        switch (parameters.Action)
        {
            case SyncHandlerAction.GetStatus:
                response = GetSynchronizationStatus(parameters);
                break;
        }

        context.Response.ContentType = ResponseContentType;
        context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();
        context.Response.Write(_serializer.Serialize(response));
    }
 
    /// <summary>
    /// 
    /// </summary>
    public bool IsReusable {
        get {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private GetStatusResponse GetSynchronizationStatus(SyncHandlerParameters parameters)
    {
        GetStatusResponse response = new GetStatusResponse();

        if (parameters.IsValid)
        {
            
            SyncHandlerController.ResultCode result;
            SynchronizationStatus status = _controller.GetSynchronizationStatus(
                parameters.Id, 
                out result);

            switch (result)
            {
                case SyncHandlerController.ResultCode.Success:
                    if (status != null)
                    {
                        response.Success = true;
                        
                        if (status.Entries != null && status.Entries.Count > 0)
                        {
                            response.IsComplete = IsFinalStatusEntry(status.Entries[status.Entries.Count - 1]);
                            
                            response.Entries = new List<GetStatusResponse.GetStatusResponseEntry>();
                            foreach (StatusEntry entry in status.Entries)
                            {
                                if (entry.Code.ToLower().Contains("failed"))
                                {
                                    response.Success = false;
                                }

                                response.Entries.Add(CreateStatusEntry(entry));                                
                            }
                        }
                    }                    
                    break;
                case SyncHandlerController.ResultCode.DatabaseError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                    break;
                case SyncHandlerController.ResultCode.UnknownError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                    break;
                case SyncHandlerController.ResultCode.ProfileNotFound:
                    response.Success = false;
                    response.Messages.Add("Profile not found");
                    break;
            }
        }
        else
        {
            response.Success = false;
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    private GetStatusResponse.GetStatusResponseEntry CreateStatusEntry(StatusEntry entry)
    {
        GetStatusResponse.GetStatusResponseEntry responseEntry =
            new GetStatusResponse.GetStatusResponseEntry();

        responseEntry.Code = entry.Code;
        responseEntry.DateCreated = entry.DateCreated.ToString();
        responseEntry.Message = _siteApi.EkMsgRef.GetMessage(entry.Code);

        if (!string.IsNullOrEmpty(entry.Error))
        {
            EwsExceptionParser exceptionParser = new EwsExceptionParser();
            responseEntry.Details = exceptionParser.Translate(
                entry.Error, 
                SyncHandlerAction.GetStatus);
        }

        switch (entry.Code.ToLower())
        {
            case StatusCodeSyncPerformingLoadBalance:       // Load balancing started for server
                responseEntry.Message = string.Format(
                    _siteApi.EkMsgRef.GetMessage("syncperformingloadbalanceon"),
                    entry.Message);
                break;
            case StatusCodeSyncCheckingLoadBalanceEnded:
                if (!string.IsNullOrEmpty(entry.Message))
                {
                    responseEntry.Message = string.Format(
                        _siteApi.EkMsgRef.GetMessage("synccheckingloadbalanceendon"),
                        entry.Message);
                }
                break;
            case StatusCodeSyncDatabaseStarted:             // Database sync details
                PopulateDatabaseResponse(entry, responseEntry);
                break;
            case StatusCodeSyncEnded:
                responseEntry.Message = _siteApi.EkMsgRef.GetMessage("lbl syncended");
                break;
        }
        
        return responseEntry;
    }

    /// <summary>
    /// Database synchronization status messages include additional
    /// details. Parse out the details and append them to the
    /// status entry. 
    /// </summary>
    /// <param name="entry">Status information</param>
    /// <param name="responseEntry">Response to populate</param>
    private void PopulateDatabaseResponse(StatusEntry entry, GetStatusResponse.GetStatusResponseEntry responseEntry)
    {
        if (!string.IsNullOrEmpty(entry.Message))
        {
            string[] splitMessage = entry.Message.Split(new char[] { '|' });
            if (splitMessage.Length == 3)
            {
                int percentComplete;
                int.TryParse(splitMessage[1], out percentComplete);

                if (percentComplete > 100)
                {
                    percentComplete = 100;
                }

                responseEntry.Details = string.Format(
                    DatabaseSyncDetailsFormat,
                    _siteApi.EkMsgRef.GetMessage(splitMessage[0]),
                    percentComplete.ToString(),
                    _siteApi.EkMsgRef.GetMessage("percentdone"));

                responseEntry.Statistics = splitMessage[2].Replace("\\n", "<br/>").Trim();
            }
        }
    }

    /// <summary>
    /// Returns true if the specified status entry represents the final
    /// entry in the status log.
    /// </summary>
    /// <param name="statusEntry">Entry to examine</param>
    /// <returns>True if the entry represents the final one in the status log</returns>
    private bool IsFinalStatusEntry(StatusEntry statusEntry)
    {
        return statusEntry.Code.ToLower() == StatusCodeSyncEnded;
    }
}
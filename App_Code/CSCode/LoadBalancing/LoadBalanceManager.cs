using System;
using System.Collections.Generic;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Instrumentation;
using Ektron.Cms.Sync.Client;
using Ektron.FileSync.Common;
using Ektron.Cms.Content;

public class LoadBalanceManager
{
    private const string SyncEnded = "syncended";
    private const string SyncFailed = "syncfailed";
    private const string SyncCheckingLoadBalanceEnded = "synccheckingloadbalanceend";
    private const string SyncPerformingLoadBalance = "syncperformingloadbalance";
    private const string SyncErrorFormat = "{0}<br/>{1}";
    private const string StatusTimeFormat = "{0} {1}";
    private const string StatusCodePrefix = "loadbalance";

    private readonly ContentAPI _contentApi;
    private readonly SiteAPI _siteApi;
    private readonly LoadBalanceSynchronizationManager _loadBalanceSyncManager;
    private readonly HttpContext _context;
    private EkContent _m_refContent = null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Current HTTP context</param>
    public LoadBalanceManager(HttpContext context)
    {
        _contentApi = new ContentAPI();
        _siteApi = new SiteAPI();
        _loadBalanceSyncManager = new LoadBalanceSynchronizationManager();
        _context = context;
    }

    /// <summary>
    /// Forces synchronization of load balanced servers. If privileges are
    /// insufficient for this activity, response data will indicate that
    /// the request has failed.
    /// </summary>
    /// <returns>Response structured serialized to JSON format</returns>
    public LoadBalanceResponse ForceLoadBalancedSync()
    {
        LoadBalanceResponse response = new LoadBalanceResponse();

        // Clear any existing sync status information prior
        // to starting synchronization.        
        
        _loadBalanceSyncManager.ResetSyncStatus(
            _context.Request.PhysicalApplicationPath, 
            System.Net.Dns.GetHostName());

        if (CanForceLoadBalancedSync)
        {
            if (TargetsAvailable)
            {
                try
                {
                    // Begin load balance synchronization (synchronous call).

                    _loadBalanceSyncManager.MakeLoadBalanceCall(
                        _siteApi.RequestInformationRef.ConnectionStringSettings.ConnectionString,
                        _context.Request.PhysicalApplicationPath.Trim(new char[] { '\\' }));

                    // Retrieve the final sync status information to verify
                    // whether or not the activity succeeded.

                    SyncStatusInfoList statusInfoList = _loadBalanceSyncManager.GetCurrentSyncStatus(
                        _context.Request.PhysicalApplicationPath,
                        System.Net.Dns.GetHostName());

                    if (statusInfoList.Count > 0)
                    {
                        // Load balance sync completed and status information
                        // is available.

                        if (VerifySyncSuccess(statusInfoList))
                        {
                            // If the operation completed with no "SyncFailed"
                            // message in the status, mark the request a success.

                            response.Success = true;
                            response.Message = _siteApi.EkMsgRef.GetMessage("force load balance success");
                        }
                        else
                        {
                            // Operation completed but "SyncFailed" appeared
                            // in the status.

                            response.Success = false;
                            response.Message = _siteApi.EkMsgRef.GetMessage("force load balance failed");
                        }
                    }
                    else
                    {
                        // If sync completed with no status information
                        // the user's server is most likely not configured
                        // for load balancing.

                        response.Success = false;
                        response.Message = _siteApi.EkMsgRef.GetMessage("force load balance no status");
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteError(ex);

                    response.Success = false;
                    response.Message = string.Format(
                        SyncErrorFormat,
                        _siteApi.EkMsgRef.GetMessage("force load balance failed"),
                        ex.Message);
                }
            }
            else
            {
                // Load balance synchronization is in progress on another server
                // in the configuration.

                response.Success = false;
                response.Message = _siteApi.EkMsgRef.GetMessage("force load balance servers in progress");
            }
        }
        else
        {
            // The current user is not authorized to initiate load balanced
            // synchronization (and/or the site is not configured for LB.

            response.Success = false;
            response.IsAuthorized = false;
            response.Message = _siteApi.EkMsgRef.GetMessage("force load balance auth");
        }

        return response;
    }

    /// <summary>
    /// Gets the last known status of any manual load balancing activities.
    /// </summary>
    /// <returns>JSON response</returns>
    public LoadBalanceStatusResponse GetStatus()
    {
        SyncStatusInfoList statusList = _loadBalanceSyncManager.GetCurrentSyncStatus(
            _context.Request.PhysicalApplicationPath,
            System.Net.Dns.GetHostName());

        LoadBalanceStatusResponse response = new LoadBalanceStatusResponse();
        response.Entries = new List<ForceLoadBalanceStatusEntry>();
        response.IsComplete = false;
        response.Success = true;

        foreach (SyncStatusInfo statusInfo in statusList)
        {
            ForceLoadBalanceStatusEntry entry = new ForceLoadBalanceStatusEntry();
            entry.Code = statusInfo.StatusCode;
            entry.Error = statusInfo.ErrorMessage;
            entry.Date = string.Format(
                StatusTimeFormat,
                statusInfo.DateCreated.ToShortDateString(),
                statusInfo.DateCreated.ToLongTimeString());
           
            switch (statusInfo.StatusCode.ToLower())
            {
                case SyncPerformingLoadBalance:     // Load balancing started for server
                    entry.Message = string.Format(
                        _siteApi.EkMsgRef.GetMessage("performing load balance sync"),
                        statusInfo.StatusMessage);
                    break;
                case SyncCheckingLoadBalanceEnded:  // Load balancing ended for server
                    entry.Message = string.Format(
                        _siteApi.EkMsgRef.GetMessage("completed load balance sync"),
                        statusInfo.StatusMessage);
                    break;
                case SyncFailed:                    // Load balancing failed
                    response.Success = false;
                    response.IsComplete = true;
                    entry.Message =
                        _siteApi.EkMsgRef.GetMessage(StatusCodePrefix + statusInfo.StatusCode.ToLower());
                    break;
                case SyncEnded:                     // Load balancing ended
                    response.IsComplete = true;
                    entry.Message = _siteApi.EkMsgRef.GetMessage("load balance sync complete");
                    break;
                default:                            // Any other status message
                    entry.Message = 
                        _siteApi.EkMsgRef.GetMessage(StatusCodePrefix + statusInfo.StatusCode.ToLower());
                    break;
            }

            if (response.Success)
            {
                response.Message = _siteApi.EkMsgRef.GetMessage("load balance sync complete");
            }
            else
            {
                if (string.IsNullOrEmpty(statusInfo.ErrorMessage))
                {
                    response.Message = _siteApi.EkMsgRef.GetMessage("force load balance failed");
                }
                else
                {
                    response.Message = string.Format(
                       SyncErrorFormat,
                       _siteApi.EkMsgRef.GetMessage("force load balance failed"),
                       statusInfo.ErrorMessage);
                }
            }
            
            response.Entries.Add(entry);
        }

        return response;
    }

    /// <summary>
    /// Searches a sync status collection for a success code.
    /// </summary>
    /// <param name="statusInfoList">Collection of sync status info to search</param>
    /// <returns>True if the status collection indicates success</returns>
    public bool VerifySyncSuccess(SyncStatusInfoList statusInfoList)
    {
        bool success = true;
        for (int i = 0; i < statusInfoList.Count && success; i++)
        {
            success = statusInfoList[i].StatusCode.ToLower() != SyncFailed;
        }

        return success;
    }

    /// <summary>
    /// Returns true if the current user can force load balancing, false otherwise.
    ///    Criteria:
    ///       - User is logged in as an admin or eSync admin.
    ///       - Site has been configured for load balancing.
    /// </summary>
    /// <returns>True if the current user can force load balancing</returns>
    public bool CanForceLoadBalancedSync
    {
        get
        {
            bool canLoadBalance = false;
            _m_refContent = new EkContent(_contentApi.RequestInformationRef);
            // The user must be logged in as an administrator...
            if (_contentApi.EkContentRef.IsAllowed(0, 0, "users", "IsLoggedIn", 0))
            {
                if (_contentApi.IsAdmin() || _m_refContent.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.SyncAdmin, _contentApi.RequestInformationRef.UserId, false))
                {
                    // Load balancing must be enabled...
                    AssetConfigInfo[] assetConfig = GetAssetConfiguration();
                    if (assetConfig[(int)AsetConfigType.LoadBalanced].Value == "1")
                    {
                        canLoadBalance = true;
                    }
                }
            }
            
            return canLoadBalance;
        }
    }

    /// <summary>
    /// Returns true if there are no load balancing activities in progress
    /// on any of the current server's peers.
    /// </summary>
    public bool TargetsAvailable
    {
        get
        {
            bool targetsAvailable = true;

            List<string> loadBalanceMachines = _loadBalanceSyncManager.GetLoadBalanceMachineList();
            for (int i = 0; i < loadBalanceMachines.Count && targetsAvailable; i++)
            {
                List<SyncStatusInfo> statusInfo = null;

                try
                {
                    statusInfo = _loadBalanceSyncManager.GetLoadBalanceSyncStatus(
                        _context.Request.PhysicalApplicationPath,
                        loadBalanceMachines[i]);
                }
                catch(Exception ex)
                {
                    Log.WriteError(ex);
                }

                if (statusInfo != null)
                {
                    targetsAvailable = IsComplete(statusInfo);
                }
            }

            return targetsAvailable;            
        }
    }

    /// <summary>
    /// Returns true if the specified collection of status info contains
    /// a completion (or failure) code.
    /// </summary>
    /// <param name="statusInfo">Collection of status info to search</param>
    /// <returns>True if the status info includes a completion code</returns>
    private bool IsComplete(List<SyncStatusInfo> statusInfo)
    {
        bool isComplete = false;

        if (statusInfo.Count > 0)
        {
            for (int i = 0; i < statusInfo.Count && !isComplete; i++)
            {
                isComplete =
                    statusInfo[i].StatusCode.ToLower() == SyncEnded ||
                    statusInfo[i].StatusCode.ToLower() == SyncFailed;
            }
        }
        else
        {
            isComplete = true;
        }

        return isComplete;
    }

    /// <summary>
    /// Returns the asset management configuration for the site.
    /// </summary>
    /// <returns>Asset management configuration data</returns>
    private AssetConfigInfo[] GetAssetConfiguration()
    {
        return _contentApi.GetAssetMgtConfigInfo();
    }
}

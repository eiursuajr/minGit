<%@ WebHandler Language="C#" Class="SyncHandler" %>

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

public class SyncHandler : IHttpHandler
{

    private const string ResponseContentType = "text/plain";

    private readonly SyncHandlerController _controller;
    private readonly JavaScriptSerializer _serializer;
    private readonly SiteAPI _siteApi;
    private readonly CommonApi _commonApi;

    /// <summary>
    /// Constructor
    /// </summary>
    public SyncHandler()
    {
        _controller = new SyncHandlerController();
        _serializer = new JavaScriptSerializer();
        _siteApi = new SiteAPI();
        _commonApi = new CommonApi();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest(HttpContext context)
    {

        ResponseBase response = null;

        // Prevent the request from timing out during
        // extended synchronization activities.

        context.Server.ScriptTimeout = 6000;

        SyncHandlerParameters parameters = new SyncHandlerParameters(context.Request);
        switch (parameters.Action)
        {
            case SyncHandlerAction.DeleteProfile:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                {
                    response = DeleteProfile(parameters);
                }
                break;
            case SyncHandlerAction.DeleteRelationship:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                {
                    response = DeleteRelationship(parameters);
                }
                break;
            case SyncHandlerAction.Pause:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                {
                    response = PauseProfile(parameters);
                }
                break;
            case SyncHandlerAction.Resume:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                {
                    response = ResumeProfile(parameters);
                }
                break;
            case SyncHandlerAction.Synchronize:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = Synchronize(parameters);
                }
                break;
            case SyncHandlerAction.ContentFolderSync:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = Synchronize(new ContentFolderSyncParameters(parameters));
                }
                break;
            case SyncHandlerAction.Certificates:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = GetCertificates();
                }
                break;
            case SyncHandlerAction.Sites:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = GetSites(new GetSitesParameters(parameters));
                }
                break;
            case SyncHandlerAction.CreateRelationship:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                {
                    response = CreateRelationship(new CreateRelationshipParameters(parameters));
                }
                break;
            case SyncHandlerAction.ResolveConflicts:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                {
                    response = ResolveConflicts();
                }
                break;
            case SyncHandlerAction.GetProfiles:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = GetProfiles(new GetProfilesParameters(parameters));
                }
                break;
            case SyncHandlerAction.IsSyncInProgress:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = IsSyncInProgress();
                }
                break;
            case SyncHandlerAction.GetFiles:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = GetFiles(new GetFilesParameters(parameters));
                }
                break;
            case SyncHandlerAction.SyncFiles:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin) ||
                    _siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncUser))
                {
                    response = Synchronize(new SyncFilesParameters(parameters));
                }
                break;
            case SyncHandlerAction.CreateCloudRelationship:
                if (_siteApi.IsARoleMember(EkEnumeration.CmsRoleIds.SyncAdmin))
                {
                    // This is a demo / POC
                    response = CreateCloudRelationship(new CreateCloudRelationshipParameters(parameters));
                }
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
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private DeleteProfileResponse DeleteProfile(SyncHandlerParameters parameters)
    {
        DeleteProfileResponse response = new DeleteProfileResponse();

        if (parameters.IsValid)
        {
            SyncHandlerController.ResultCode result;
            _controller.DeleteProfile(parameters.Id, out result);

            switch (result)
            {
                case SyncHandlerController.ResultCode.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.ResultCode.DatabaseError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                    break;
                case SyncHandlerController.ResultCode.UnknownError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Messages.Add("Missing parameter: id");
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private DeleteRelationshipResponse DeleteRelationship(SyncHandlerParameters parameters)
    {
        DeleteRelationshipResponse response = new DeleteRelationshipResponse();

        if (parameters.IsValid)
        {
            SyncHandlerController.ResultCode result;
            _controller.DeleteRelationship(parameters.Id, out result);

            switch (result)
            {
                case SyncHandlerController.ResultCode.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.ResultCode.DatabaseError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                    break;
                case SyncHandlerController.ResultCode.UnknownError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Messages.Add("Missing parameter: id");
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private ProfileStatusResponse PauseProfile(SyncHandlerParameters parameters)
    {
        ProfileStatusResponse response = new ProfileStatusResponse();

        if (parameters.IsValid)
        {
            SyncHandlerController.ResultCode result;
            DateTime nextRunTime = _controller.PauseSchedule(parameters.Id, out result);

            response.Id = parameters.Id;
            response.NextRunTime = nextRunTime.ToString();

            switch (result)
            {
                case SyncHandlerController.ResultCode.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.ResultCode.DatabaseError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                    break;
                case SyncHandlerController.ResultCode.UnknownError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Messages.Add("Missing parameter: id");
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private ProfileStatusResponse ResumeProfile(SyncHandlerParameters parameters)
    {
        ProfileStatusResponse response = new ProfileStatusResponse();

        if (parameters.IsValid)
        {
            SyncHandlerController.ResultCode result;
            DateTime nextRunTime = _controller.ResumeSchedule(parameters.Id, out result);

            response.Id = parameters.Id;
            response.NextRunTime = nextRunTime.ToString();

            switch (result)
            {
                case SyncHandlerController.ResultCode.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.ResultCode.DatabaseError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                    break;
                case SyncHandlerController.ResultCode.UnknownError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Messages.Add("Missing parameter: id");
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private SynchronizeResponse Synchronize(SyncHandlerParameters parameters)
    {
        SynchronizeResponse response = new SynchronizeResponse();

        SyncHandlerController.SynchronizeResult result;

        Relationship relationship = Relationship.GetRelationship(parameters.Id);
        if (relationship.Name == "Azure")
        {
            Ektron.FileSync.Common.CloudSyncParams cloudSyncParams = this.getParams(relationship);
            EktronServiceProxy proxy = new EktronServiceProxy();
            proxy.CloudSyncronize(cloudSyncParams);
        }
        else
        {
            Profile profile = _controller.Synchronize(parameters.Id, out result);
            switch (result)
            {
                case SyncHandlerController.SynchronizeResult.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.SynchronizeResult.DatabaseError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                    break;
                case SyncHandlerController.SynchronizeResult.ProfileNotFound:
                    response.Success = false;
                    response.Messages.Add("Profile not found.");
                    break;
                case SyncHandlerController.SynchronizeResult.RelationshipNotInitialized:
                    response.Success = false;
                    response.Messages.Add("Relationship is not initialized.");
                    break;
                case SyncHandlerController.SynchronizeResult.SynchronizationInProgress:
                    response.Success = false;
                    response.ProfileId = profile.Id;
                    response.ProfileName = profile.Name;
                    response.Messages.Add("Synchronization is already in progress: " + profile.Id.ToString());
                    break;
                case SyncHandlerController.SynchronizeResult.UnknownError:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                    break;
            }
        }

        return response;
    }

    private Ektron.FileSync.Common.CloudSyncParams getParams(Relationship relationship)
    {
        Ektron.FileSync.Common.CloudSyncParams cloudSyncParams = new Ektron.FileSync.Common.CloudSyncParams();
        //Cloud Params
        cloudSyncParams.RemoteDBConnectionString = relationship.RemoteSite.ConnectionString;
        string[] data = relationship.RemoteSite.Address.Split('|');
        cloudSyncParams.LocalIPAddress = "";
        cloudSyncParams.RemoteStorage = "";
        cloudSyncParams.StorageContainerName = data[0];
        cloudSyncParams.StorageAccountName = data[1];
        cloudSyncParams.StorageAccountKey = data[2];
        cloudSyncParams.RemoteDomain = "";
        //Base Params
        cloudSyncParams.ForcePreInit = false;
        cloudSyncParams.IsDownloadUpload = false;
        cloudSyncParams.RemoteScheduleId = 0;
        cloudSyncParams.SyncBin = false;
        cloudSyncParams.SyncID = relationship.Id.ToString();
        cloudSyncParams.UserId = _commonApi.UserId;
        cloudSyncParams.PreviewMode = false;
        cloudSyncParams.SyncDatabase = true;
        cloudSyncParams.SyncAssetLibrary = true;
        cloudSyncParams.SyncAssets = true;
        cloudSyncParams.SyncPrivateAssets = true;
        cloudSyncParams.SyncUploadedImages = true;
        cloudSyncParams.SyncUploadedFiles = true;
        cloudSyncParams.SyncTemplates = false;
        cloudSyncParams.SyncWorkArea = false;
        cloudSyncParams.SyncDirection = Ektron.FileSync.Common.SyncDirection.upload;
        cloudSyncParams.ScopeFilter = new Ektron.FileSync.Common.SyncScopeFilter(); // ?
        cloudSyncParams.AssetLibraryPath = relationship.LocalSite.AssetLibraryPath;
        cloudSyncParams.AssetsPath = relationship.LocalSite.AssetsPath;
        cloudSyncParams.LocalSiteName = relationship.LocalSite.ConnectionString;// System.Net.Dns.GetHostName();//?
        cloudSyncParams.PrivateAssetsPath = relationship.LocalSite.PrivateAssetsPath;
        cloudSyncParams.SitePath = relationship.LocalSite.SitePath;
        cloudSyncParams.SiteUrl = relationship.LocalSite.ServiceEndpoint;//.SiteUrl;
        cloudSyncParams.SourceAddress = relationship.LocalSite.SiteAddress;
        cloudSyncParams.UploadedFilesPath = relationship.LocalSite.UploadedFilesPath;
        cloudSyncParams.UploadedImagesPath = relationship.LocalSite.UploadedImagesPath;
        cloudSyncParams.WebSitePath = relationship.LocalSite.SitePath;
        cloudSyncParams.WorkareaPath = relationship.LocalSite.WorkareaPath;
        cloudSyncParams.RemoteAssetLibraryPath = "";
        cloudSyncParams.RemoteAssetsPath = "";
        cloudSyncParams.RemotePrivateAssetsPath = "";
        cloudSyncParams.RemoteScheduleId = 0;
        cloudSyncParams.RemoteSiteName = relationship.RemoteSite.ConnectionString;
        cloudSyncParams.RemoteSitePath = "";
        cloudSyncParams.RemoteUploadedFilesPath = "";
        cloudSyncParams.RemoteUploadedImagesPath = "";
        cloudSyncParams.RemoteUrl = "";
        cloudSyncParams.RemoteWebSitePath = "";
        cloudSyncParams.RemoteWorkareaPath = "";
        cloudSyncParams.RemoteAddress = "";
        cloudSyncParams.SyncStartTime = DateTime.Now;
        cloudSyncParams.ConflictResolutionPolicy = Ektron.FileSync.Common.ConflictResolution.SourceWins;
        return cloudSyncParams;
    }

    private SynchronizeResponse Synchronize(SyncFilesParameters parameters)
    {
        SynchronizeResponse response = new SynchronizeResponse();

        SyncHandlerController.SynchronizeResult result;
        Profile profile = _controller.Synchronize(parameters.Id, parameters.Files, out result);

        switch (result)
        {
            case SyncHandlerController.SynchronizeResult.Success:
                response.Success = true;
                break;
            case SyncHandlerController.SynchronizeResult.DatabaseError:
                response.Success = false;
                response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                break;
            case SyncHandlerController.SynchronizeResult.ProfileNotFound:
                response.Success = false;
                response.Messages.Add("Profile not found.");
                break;
            case SyncHandlerController.SynchronizeResult.RelationshipNotInitialized:
                response.Success = false;
                response.Messages.Add("Relationship is not initialized.");
                break;
            case SyncHandlerController.SynchronizeResult.SynchronizationInProgress:
                response.Success = false;
                response.ProfileId = profile.Id;
                response.ProfileName = profile.Name;
                response.Messages.Add("Synchronization is already in progress: " + profile.Id.ToString());
                break;
            case SyncHandlerController.SynchronizeResult.UnknownError:
                response.Success = false;
                response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                break;
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private SyncResponse Synchronize(ContentFolderSyncParameters parameters)
    {
        SynchronizeResponse response = new SynchronizeResponse();

        SyncHandlerController.SynchronizeResult result;

        Profile profile = null;
        if (parameters.IsContentSync)
        {
            profile = _controller.Synchronize(
                parameters.Id,
                parameters.ContentId,
                parameters.LanguageId,
                parameters.FolderId,
                parameters.AssetId,
                parameters.AssetVersion,
                out result);
        }
        else
        {
            profile = _controller.Synchronize(
                parameters.Id,
                parameters.FolderId,
                out result);
        }

        switch (result)
        {
            case SyncHandlerController.SynchronizeResult.Success:
                response.Success = true;
                break;
            case SyncHandlerController.SynchronizeResult.DatabaseError:
                response.Success = false;
                response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync database error"));
                break;
            case SyncHandlerController.SynchronizeResult.ProfileNotFound:
                response.Success = false;
                response.Messages.Add("Profile not found.");
                break;
            case SyncHandlerController.SynchronizeResult.RelationshipNotInitialized:
                response.Success = false;
                response.Messages.Add("Relationship is not initialized.");
                break;
            case SyncHandlerController.SynchronizeResult.SynchronizationInProgress:
                response.Success = false;
                response.ProfileId = profile.Id;

                if (profile.IsDefault)
                {
                    ConnectionInfo connectionInfo = new ConnectionInfo(profile.Name);
                    response.ProfileName = string.Format(
                        "{0}/{1}",
                        connectionInfo.ServerName,
                        connectionInfo.DatabaseName);
                }
                else
                {
                    response.ProfileName = profile.Name;
                }
                break;
            case SyncHandlerController.SynchronizeResult.UnknownError:
                response.Success = false;
                response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unexpected error"));
                break;
        }

        return response;
    }

    private GetCertificatesResponse GetCertificates()
    {
        GetCertificatesResponse response = new GetCertificatesResponse();

        SyncHandlerController.ResultCode result;
        response.Certificates = _controller.GetCertificates(out result);

        switch (result)
        {
            case SyncHandlerController.ResultCode.Success:
                response.Success = true;
                break;
            case SyncHandlerController.ResultCode.UnknownError:
                response.Success = false;
                response.Messages.Add("Service error");
                break;
        }

        return response;
    }

    private GetSitesResponse GetSites(GetSitesParameters parameters)
    {
        GetSitesResponse response = new GetSitesResponse();
        response.Sites = new List<GetSitesResponse.Site>();

        if (parameters.IsValid)
        {
            SyncHandlerController.ResultCode result;
            List<SiteConfiguration> sites = _controller.GetSites(
                parameters.Server,
                parameters.Certificate,
                parameters.IncludeLocal,
                out result);

            if (sites != null)
            {
                Dictionary<string, GetSitesResponse.Site> sitesTable =
                    new Dictionary<string, GetSitesResponse.Site>();

                foreach (SiteConfiguration siteConfiguration in sites)
                {
                    if (siteConfiguration.Address.ToLower() == parameters.Certificate.ToLower())
                    {
                        string connectionString = siteConfiguration.ConnectionString;
                        if (!sitesTable.ContainsKey(connectionString))
                        {
                            GetSitesResponse.Site site = new GetSitesResponse.Site();
                            site.ServerName = siteConfiguration.Address;
                            site.DatabaseName = siteConfiguration.Connection.DatabaseName;
                            site.DatabaseServerName = siteConfiguration.Connection.ServerName;
                            site.IntegratedSecurity = siteConfiguration.Connection.IntegratedSecurity;
                            site.SitePaths = new List<string>();
                            site.SitePaths.Add(siteConfiguration.SitePath);

                            // Add the site to the reference table for
                            // future lookups.

                            sitesTable.Add(connectionString, site);

                            // Add the site to the response.

                            response.Sites.Add(site);
                        }
                        else
                        {
                            sitesTable[connectionString].SitePaths.Add(siteConfiguration.SitePath);
                        }
                    }
                }
            }

            switch (result)
            {
                case SyncHandlerController.ResultCode.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.ResultCode.UnknownError:
                    response.Success = false;
                    response.Messages.Add("Service error");
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Messages.Add("Invalid parameters");
        }

        return response;
    }

    private CreateRelationshipResponse CreateRelationship(CreateRelationshipParameters parameters)
    {
        CreateRelationshipResponse response = new CreateRelationshipResponse();

        if (parameters.IsValid)
        {
            SyncHandlerController.CreateRelationshipResult result;

            Relationship relationship = _controller.CreateRelationship(
                parameters.LocalDatabaseName,
                parameters.LocalServerName,
                parameters.LocalSitePath,
                parameters.MultiSiteFolderId,
                parameters.RemoteDatabaseName,
                parameters.RemoteServerName,
                parameters.RemoteDatabaseServer,
                parameters.RemoteSitePath,
                parameters.Certificate,
                parameters.Direction,
                out result);

            if (relationship != null)
            {
                response.RelationshipId = relationship.Id;
            }

            switch (result)
            {
                case SyncHandlerController.CreateRelationshipResult.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.CreateRelationshipResult.RelationshipAlreadyExists:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("js sync relationship exists"));
                    break;
                case SyncHandlerController.CreateRelationshipResult.RelationshipResurrected:
                    response.Success = true;
                    response.Resurrected = true;
                    break;
                case SyncHandlerController.CreateRelationshipResult.DatabaseError:
                    response.Success = false;
                    response.Messages.Add("Database error.");
                    break;
                case SyncHandlerController.CreateRelationshipResult.UnknownError:
                    response.Success = false;
                    response.Messages.Add("Unknown error.");
                    break;
                case SyncHandlerController.CreateRelationshipResult.CommunicationFailure:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync service error"));
                    break;
                case SyncHandlerController.CreateRelationshipResult.RelationshipNotInitialized:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unintialized all folders"));
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Messages.Add("Invalid parameters.");
        }

        return response;
    }

    private CreateRelationshipResponse CreateCloudRelationship(CreateCloudRelationshipParameters parameters)
    {
        CreateRelationshipResponse response = new CreateRelationshipResponse();

        if (parameters.IsValid)
        {
            SyncHandlerController.CreateRelationshipResult result;

            Relationship relationship = _controller.CreateCloudRelationship(
                parameters.LocalDatabaseName,
                parameters.LocalServerName,
                parameters.LocalSitePath,
                parameters.MultiSiteFolderId,
                parameters.RemoteDBConnectionString,
                parameters.LocalIPAddress,
                parameters.RemoteStorage,
                parameters.StorageAccountName,
                parameters.StorageContainerName,
                parameters.StorageAccountKey,
                parameters.RemoteDomain,
                parameters.Certificate,
                parameters.Direction,
                out result);

            if (relationship != null)
            {
                response.RelationshipId = relationship.Id;
            }

            switch (result)
            {
                case SyncHandlerController.CreateRelationshipResult.Success:
                    response.Success = true;
                    break;
                case SyncHandlerController.CreateRelationshipResult.RelationshipAlreadyExists:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("js sync relationship exists"));
                    break;
                case SyncHandlerController.CreateRelationshipResult.RelationshipResurrected:
                    response.Success = true;
                    response.Resurrected = true;
                    break;
                case SyncHandlerController.CreateRelationshipResult.DatabaseError:
                    response.Success = false;
                    response.Messages.Add("Database error.");
                    break;
                case SyncHandlerController.CreateRelationshipResult.UnknownError:
                    response.Success = false;
                    response.Messages.Add("Unknown error.");
                    break;
                case SyncHandlerController.CreateRelationshipResult.CommunicationFailure:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync service error"));
                    break;
                case SyncHandlerController.CreateRelationshipResult.RelationshipNotInitialized:
                    response.Success = false;
                    response.Messages.Add(_siteApi.EkMsgRef.GetMessage("sync unintialized all folders"));
                    break;
            }
        }
        else
        {
            response.Success = false;
            response.Messages.Add("Invalid parameters.");
        }

        return response;
    }

    private ResolveConflictsResponse ResolveConflicts()
    {
        ResolveConflictsResponse response = new ResolveConflictsResponse();
        SyncHandlerController.ResultCode result;

        long syncConflictResolutionMode = -1;

        _controller.ResolveSynchronizationConflicts(out result, syncConflictResolutionMode);

        switch (result)
        {
            case SyncHandlerController.ResultCode.Success:
                response.Success = true;
                break;
            case SyncHandlerController.ResultCode.DatabaseError:
                response.Messages.Add(_siteApi.EkMsgRef.GetMessage("resolve database error"));
                response.Success = false;
                break;
            case SyncHandlerController.ResultCode.UnknownError:
                response.Messages.Add(_siteApi.EkMsgRef.GetMessage("resolve unexpected error"));
                response.Success = false;
                break;
        }
        
        return response;
    }

    private GetProfilesResponse GetProfiles(GetProfilesParameters parameters)
    {
        GetProfilesResponse response = new GetProfilesResponse();

        SyncHandlerController.ResultCode result = SyncHandlerController.ResultCode.None;

        // Retrieve profiles meeting the specified input criteria.

        List<Profile> profiles = _controller.GetProfiles(
            parameters.IsContentSync,
            parameters.IncludeMultisiteProfiles,
            parameters.ContentId,
            parameters.Language,
            parameters.FolderId,
            out result);

        if (profiles != null)
        {
            Dictionary<long, GetProfilesResponse.Relationship> relationships =
                new Dictionary<long, GetProfilesResponse.Relationship>();

            foreach (Profile profile in profiles)
            {
                GetProfilesResponse.Relationship relationshipResponse = null;

                // Determine if this profile belongs to a relationship
                // for which a response has already been created.

                if (!relationships.ContainsKey(profile.Parent.Id))
                {
                    // If the relationship response does not yet exist for this
                    // profile's parent, create one.

                    relationshipResponse = new GetProfilesResponse.Relationship();
                    relationshipResponse.RelationshipId = profile.Parent.Id;
                    relationshipResponse.DatabaseName = profile.Parent.RemoteSite.Connection.DatabaseName;
                    relationshipResponse.LocalSitePath = profile.Parent.LocalSite.SitePath;
                    relationshipResponse.RemoteSitePath = profile.Parent.RemoteSite.SitePath;
                    relationshipResponse.ServerName = profile.Parent.RemoteSite.Connection.ServerName;
                    relationshipResponse.Direction = profile.Parent.DefaultProfile.Direction.ToString();

                    if (profile.Parent.MultiSite.IsMultiSite)
                    {
                        relationshipResponse.LocalMultiSite = profile.Parent.MultiSite.SiteName;
                    }

                    // Add the relationship response to a table for more
                    // convenient lookups.

                    relationships.Add(relationshipResponse.RelationshipId, relationshipResponse);

                    // Add the relationship to the response data structure.

                    response.Relationships.Add(relationshipResponse);
                }
                else
                {
                    // This profile's parent relationship already has a response
                    // so retrieve it.

                    relationshipResponse = relationships[profile.Parent.Id];
                }

                // Create response data for the profile.

                GetProfilesResponse.Profile profileResponse = new GetProfilesResponse.Profile();
                profileResponse.ProfileId = profile.Id;
                profileResponse.Direction = profile.Direction.ToString();
                if (profile.SynchronizeDatabase)
                {
                    profileResponse.Items.Add("Database");
                }
                if (profile.SynchronizeTemplates)
                {
                    profileResponse.Items.Add("Templates");
                }
                if (profile.SynchronizeWorkarea)
                {
                    profileResponse.Items.Add("Workarea");
                }
                if (profile.SynchronizeBinaries)
                {
                    profileResponse.Items.Add("Bin");
                }

                if (profile.IsDefault)
                {
                    // Default profile's use the connection string
                    // as a name. Replace this with a safer string.

                    profileResponse.ProfileName = "Default Relationship Profile";
                }
                else
                {
                    profileResponse.ProfileName = profile.Name;
                }

                // Add the profile response to the appropriate parent
                // response.

                relationshipResponse.Profiles.Add(profileResponse);
            }
        }

        switch (result)
        {
            case SyncHandlerController.ResultCode.DatabaseError:
                response.Messages.Add("Database error.");
                response.Success = false;
                break;
            case SyncHandlerController.ResultCode.UnknownError:
                response.Messages.Add("Unknown error.");
                response.Success = false;
                break;
            case SyncHandlerController.ResultCode.Success:
                response.Success = true;
                break;
        }

        return response;
    }

    public IsSyncInProgressResponse IsSyncInProgress()
    {
        IsSyncInProgressResponse response = new IsSyncInProgressResponse();

        SyncHandlerController.ResultCode result;
        Profile profile = _controller.IsSyncInProgress(out result);

        switch (result)
        {
            case SyncHandlerController.ResultCode.Success:
                response.Success = true;
                if (profile != null)
                {
                    response.IsSyncInProgress = true;
                    response.ProfileId = profile.Id;

                    if (profile.IsDefault)
                    {
                        ConnectionInfo connectionInfo = new ConnectionInfo(profile.Name);
                        response.ProfileName = string.Format(
                            "{0}/{1}",
                            connectionInfo.ServerName,
                            connectionInfo.DatabaseName);
                    }
                    else
                    {
                        response.ProfileName = profile.Name;
                    }

                    string message = string.Format(
                        _siteApi.EkMsgRef.GetMessage("lbl sync running confirm"),
                        response.ProfileName);

                    response.Messages.Add(message);
                }
                else
                {
                    response.IsSyncInProgress = false;
                }
                break;
            case SyncHandlerController.ResultCode.DatabaseError:
                response.Success = false;
                response.Messages.Add("Database error.");
                break;
            case SyncHandlerController.ResultCode.UnknownError:
                response.Success = false;
                response.Messages.Add("Unknown error.");
                break;
        }

        return response;
    }

    public GetFilesResponse GetFiles(GetFilesParameters parameters)
    {
        GetFilesResponse response = new GetFilesResponse();

        SyncHandlerController.ResultCode result;
        FileSyncNode node = _controller.GetFiles(
            parameters.Id,
            parameters.Path,
            false,
            out result);

        switch (result)
        {
            case SyncHandlerController.ResultCode.Success:
                response.Success = true;
                response.Node = node;
                break;
            case SyncHandlerController.ResultCode.DatabaseError:
                response.Success = false;
                response.Messages.Add("Database error.");
                break;
            case SyncHandlerController.ResultCode.UnknownError:
                response.Success = false;
                response.Messages.Add("Unknown error.");
                break;
        }

        return response;
    }
}
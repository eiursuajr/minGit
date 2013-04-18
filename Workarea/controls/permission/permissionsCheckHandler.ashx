<%@ WebHandler Language="C#" Class="permissionsCheckHandler" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Web.Script.Serialization;
using Ektron.Cms;
using Ektron.Cms.Content;
using Ektron.Cms.Common;
using Ektron.Cms.Permissions;


namespace Ektron.Cms.Permissions
{
    /// <summary>
    /// The object that is serialized for our response to AJAX requests,
    /// as well as serving as the basis for our JS permissions object.
    /// </summary>
    public class PermissionsResponse
    {
        // Private Variables
        private ContentAPI _ContentApi;
        private long _Id;
        private long _ParentId;
        private bool _IsARoleMemberForFolder_FolderUserAdmin;
        private Dictionary<string, bool> _LicensedFeatures;
        private Dictionary<string, bool> _RoleMemberships;
        private PermissionData _Folder;

        // Properties
        public long Id // the original CMS ID we're checking permissions for.
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public long ParentId // the parent ID of the CMS object we're checking permissions for.
        {
            get { return _ParentId; }
            set { _ParentId = value; }
        }
        public bool IsARoleMemberForFolder_FolderUserAdmin
        {
            get { return _IsARoleMemberForFolder_FolderUserAdmin; }
            set { _IsARoleMemberForFolder_FolderUserAdmin = value; }
        }
        public Dictionary<string , bool> LicensedFeatures
            {
            get { return _LicensedFeatures; }
            set { _LicensedFeatures = value; }
        }
        public Dictionary<string, bool> RoleMemberships
            {
            get { return _RoleMemberships; }
            set { _RoleMemberships = value; }
        }
        public PermissionData Folder
            {
            get { return _Folder; }
            set { _Folder = value; }
        }

        public PermissionsResponse(System.Web.HttpContext context)
        {
            _ContentApi = new ContentAPI();
            Id = -1;
            ParentId = -1;

            // get the list of licensed features and if they are enabled or not
            LicensedFeatures = new Dictionary<string,bool>();
            foreach (Ektron.Cms.DataIO.LicenseManager.Feature feature in Enum.GetValues(typeof(Ektron.Cms.DataIO.LicenseManager.Feature)))
            {
                LicensedFeatures.Add(feature.ToString(), Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, feature));
            }

            // get the list of RoleMemberships the user belongs to
            RoleMemberships = GetRoles(context);

            Folder = new PermissionData();
        }

        public Dictionary<string, bool> GetRoles(HttpContext context)
        {
            _ContentApi = new ContentAPI();
            long currentUserId = _ContentApi.UserId;
            long cachedUserId = 0;
            _ContentApi = new ContentAPI();
            Dictionary<string, bool> RoleMemberships = new Dictionary<string, bool>();
            bool _isCopyOrMoveAdmin = false;
            
            RoleMemberships = (Dictionary<string, bool>)context.Session["RoleMemberships"];
            if (context.Session["CachedPermissionsUserId"] != null)
            {
                cachedUserId = (long)context.Session["CachedPermissionsUserId"];
            }
            if (RoleMemberships == null || cachedUserId != currentUserId)
            {
                RoleMemberships = new Dictionary<string, bool>();
                foreach (Ektron.Cms.Common.EkEnumeration.CmsRoleIds memberships in Enum.GetValues(typeof(Ektron.Cms.Common.EkEnumeration.CmsRoleIds)))
                {
                    RoleMemberships.Add(memberships.ToString(), _ContentApi.IsARoleMember(memberships));
                }
                context.Session["RoleMemberships"] = RoleMemberships;
                context.Session["CachedPermissionsUserId"] = currentUserId;
            }

            if (context.Request.QueryString["id"] != null)
            {
                _Id = Int64.Parse(context.Request.QueryString["id"]);
            }
            if (_Id < 0)
            {
                _Id = 0;
            }
            _isCopyOrMoveAdmin = _ContentApi.IsARoleMemberForFolder(Convert.ToInt16(EkEnumeration.CmsRoleIds.MoveOrCopy), _Id, _ContentApi.UserId, false);
            RoleMemberships.Remove(EkEnumeration.CmsRoleIds.MoveOrCopy.ToString());
            RoleMemberships.Add(EkEnumeration.CmsRoleIds.MoveOrCopy.ToString(), _isCopyOrMoveAdmin);
            return RoleMemberships;
        }
    }

    public enum TreeNodeType
    {
        Folder = 0,
        Blog = 1,
        Domain = 2,
        DiscussionBoard = 3,
        DiscussionForum = 4,
        Root = 5,
        Community = 6,
        Media = 7,
        Calendar = 8,
        Catalog = 9,
        Taxonomy = 10,
        Collection = 11,
        Menu = 12,
        Content = 13
    }
}

public class permissionsCheckHandler : IHttpHandler,IRequiresSessionState
{
    // Member Variables
    private ContentAPI _ContentApi = new ContentAPI();
    private long _CurrentUserID = 1;
    private long _Id = 0;
    private TreeNodeType _CheckType = TreeNodeType.Folder;
    private int _LanguageId = 1033;
    private JavaScriptSerializer _JsonSerializer = new JavaScriptSerializer();
    private string _Action = "getPermissionJsClass";

    public void ProcessRequest (HttpContext context)
    {
        // create a string to store our JSON response.
        string response = "";

        try
        {
            _CurrentUserID = _ContentApi.UserId;
            if (context.Request.QueryString["id"] != null)
            {
                Int64.TryParse(context.Request.QueryString["id"], out _Id);
            }
            if (_Id < 0)
            {
                _Id = 0;
            }
            _LanguageId = int.Parse(_ContentApi.GetCookieValue("LastValidLanguageID"));
            if (context.Request.QueryString["action"] != null)
            {
                // override the default with the action specified
                _Action = context.Request.QueryString["action"];
            }

            switch (_Action)
            {
                case ("getPermissions"):
                    // get the permissions for the request
                    response = getPermissions(context);
                    break;
                default:
                    // return the JavaScript class representing the permissions object for the client side.
                    response = getJsClass(context);
                    break;
            }
            returnResponse(response, context);
        }
        catch(Exception e)
        {
            // convert the exception object to a JSON object
            response = "{}";
            returnResponse(response, context);
        }
    }

    protected bool returnResponse(string response, HttpContext httpContext)
    {
        // prevent this from getting cached
        httpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        httpContext.Response.Cache.SetNoStore();
        //set the Content Type
        httpContext.Response.ContentType = "text/plain";
        // send the response
        httpContext.Response.Write(response);
        return true;
    }

    protected string getJsClass(HttpContext context)
    {
        PermissionsResponse permissionsObj = new PermissionsResponse(context);
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(@"Ektron.Permissions = function Ektron_Permissions()");
        sb.AppendLine(@"{");
        sb.AppendLine(@"$ektron.extend(this," + _JsonSerializer.Serialize(permissionsObj) + @");");
        sb.AppendLine(@"};");

        return sb.ToString();
    }

    protected string getPermissions(HttpContext context)
    {
        string result = "{}";
        // if the user is logged in, let's check their permissions, otherwise do nothing
        if (_ContentApi.IsLoggedIn)
        {
            // determine the permissions type for the check
            if (context.Request.QueryString["checkType"] != null && !String.IsNullOrEmpty(context.Request.QueryString["checkType"]))
            {
                _CheckType = (TreeNodeType)Enum.Parse(typeof(TreeNodeType), context.Request.QueryString["checkType"]);
                // get the appropriate permissions for the check type requested
                switch (_CheckType)
                {
                    case TreeNodeType.Folder:
                    case TreeNodeType.Blog:
                    case TreeNodeType.Domain:
                    case TreeNodeType.DiscussionBoard:
                    case TreeNodeType.DiscussionForum:
                    case TreeNodeType.Root:
                    case TreeNodeType.Community:
                    case TreeNodeType.Media:
                    case TreeNodeType.Calendar:
                    case TreeNodeType.Catalog:
                    case TreeNodeType.Collection:
                    case TreeNodeType.Menu:
                    case TreeNodeType.Content:
                        // Check permissions for all folder types
                        result = checkFolderPermissions(_Id, context);
                        break;
                    case TreeNodeType.Taxonomy:
                        // establish roles based permissions
                        result = checkRolesPermissions(context);
                        break;                        
                    default:
                        //
                        break;
                }
            }
        }
        return result;
    }

    protected string checkFolderPermissions(long id, HttpContext context)
    {
        // declare local variables
        FolderData folderDataWithPermissions;
        Ektron.Cms.API.Folder folderApi = new Ektron.Cms.API.Folder();
        PermissionsResponse permissionsResponse = new PermissionsResponse(context);

        // load permissions
        folderDataWithPermissions = _ContentApi.GetFolderDataWithPermission(id);
        if (folderDataWithPermissions != null)
        {
            // assign values to our response object
            permissionsResponse.Folder = folderDataWithPermissions.Permissions;
            permissionsResponse.Id = id;
            permissionsResponse.ParentId = folderDataWithPermissions.ParentId;
        }
        // serialize the permissions object for this folder and return
        return _JsonSerializer.Serialize(permissionsResponse);
    }

    protected string checkRolesPermissions(HttpContext context)
    {
        PermissionsResponse permissionsResponse = new PermissionsResponse(context);
        Dictionary<string, bool> RoleMemberships = permissionsResponse.RoleMemberships;

        // serialize the permissions object for this folder and return
        return _JsonSerializer.Serialize(RoleMemberships);
    }

    // Impliment interface member System.Web.IHttpHandler.IsReusable just because...
    public bool IsReusable
    {
        get {
            return false;
        }
    }
}
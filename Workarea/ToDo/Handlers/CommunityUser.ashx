<%@ WebHandler Language="C#" Class="CommunityUser" %>

using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using Ektron.Cms;
using Ektron.Cms.Common;
using System.Collections.Generic;
using System.Collections.Specialized;
using Ektron.Cms.User;

[Serializable]
public class UserResponse
{
    private bool _success = false;
    private string _errorMessage = "";
    private int _totalPages = 0;
    private int _currentPage = 0;
    private List<UserBaseData> _users = new List<UserBaseData>();

    public UserResponse() { }

    public UserResponse(bool success, string errorMessage, List<UserBaseData> users)
    {
        this.Success = success;
        this.ErrorMessage = errorMessage;
        this.Users = users;
    }
    
    public bool Success
    {
        get { return _success; }
        set { _success = value; }
    }
    
    public string ErrorMessage
    {
        get { return _errorMessage; }
        set { _errorMessage = value; }
    }

    public int TotalPages
    {
        get { return _totalPages; }
        set { _totalPages = value; }
    }

    public int CurrentPage
    {
        get { return _currentPage; }
        set { _currentPage = value; }
    }

    public List<UserBaseData> Users
    {
        get { return _users; }
        set { _users = value; }
    }
}

public class CommunityUser : IHttpHandler
{
    #region Services
    private Ektron.Cms.User.IUser _userService;
    private Ektron.Cms.User.IUser UserService
    {
        get
        {
            if (_userService == null) _userService = ObjectFactory.GetUser();
            return _userService;
        }
    }

    private Ektron.Cms.Community.CommunityGroupAPI _communityGroupService;
    private Ektron.Cms.Community.CommunityGroupAPI CommunityGroupService
    {
        get
        {
            if (_communityGroupService == null) _communityGroupService = new Ektron.Cms.Community.CommunityGroupAPI();
            return _communityGroupService;
        }
    }
    #endregion
    
    public void ProcessRequest (HttpContext context)
    {
        context.Response.ContentType = "application/json";

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        MemoryStream memStream = new MemoryStream();
        UserResponse userResponse = new UserResponse();
        
        try
        {
            switch (context.Request.Params["action"])
            {
                case "search":
                    userResponse = Search(
                        context.Request.Params["query"], 
                        context.Request.Params["field"], 
                        context.Request.Params["groupId"]
                    );
                    break;
                case "getPage":
                    userResponse = GetPage(
                        context.Request.Params["index"], 
                        context.Request.Params["pageSize"],
                        context.Request.Params["orderBy"],
                        context.Request.Params["groupId"]
                    );
                    break;
                default:
                    userResponse.ErrorMessage = "Invalid action.";
                    break;
            }
        }
        catch (Exception exc)
        {
            userResponse.ErrorMessage = exc.Message;
        }

        context.Response.Write(serializer.Serialize(userResponse));
    }

    #region Actions
    protected UserResponse Search(string query, string field, string groupId)
    {
        if (query == null) throw new MissingMethodException(@"""query"" parameter is missing.");
        if (groupId == null) throw new MissingMethodException(@"""groupId"" parameter is missing.");

        long communityGroupId = long.Parse(groupId);
        int totalPages = 0;
        int totalUsers = 0;
        int totalPending = 0;
        long adminId = 0;

        UserResponse userResponse = new UserResponse();

        DirectoryUserData[] users = CommunityGroupService.GetCommunityGroupUsers(
            communityGroupId, 
            query, 
            field, 
            1, 
            5, 
            ref totalPages, 
            ref totalUsers, 
            ref totalPending, 
            ref adminId
        );

        userResponse.Users = new List<UserBaseData>(
            Array.ConvertAll<DirectoryUserData, UserBaseData>(
                users, 
                delegate(DirectoryUserData userData)
                {
                    return (UserBaseData)userData;
                })
            );
        userResponse.TotalPages = totalPages;
        userResponse.CurrentPage = 1;
        userResponse.Success = true;
        
        return userResponse;
    }

    protected UserResponse GetPage(string index, string pageSize, string orderBy, string groupId)
    {
        if (index == null) throw new MissingMethodException(@"""index"" parameter is missing.");
        if (pageSize == null) throw new MissingMethodException(@"""pageSize"" parameter is missing.");
        if (orderBy == null) throw new MissingMethodException(@"""orderBy"" parameter is missing.");
        if (groupId == null) throw new MissingMethodException(@"""groupId"" parameter is missing.");

        long communityGroupId = long.Parse(groupId);
        int totalPages = 0;
        int totalUsers = 0;
        int totalPending = 0;

        UserResponse userResponse = new UserResponse();
        UserProperty orderByProperty = (UserProperty)Enum.Parse(typeof(UserProperty), orderBy, true);
        int currentPage = int.Parse(index);
        int recordsPerPage = int.Parse(pageSize);

        DirectoryUserData[] users = CommunityGroupService.GetCommunityGroupUsers(
            communityGroupId,
            currentPage,
            recordsPerPage,
            ref totalPages,
            ref totalUsers,
            ref totalPending
        );

        userResponse.Users = new List<UserBaseData>(
            Array.ConvertAll<DirectoryUserData, UserBaseData>(
                users,
                delegate(DirectoryUserData userData)
                {
                    return (UserBaseData)userData;
                })
            );
        userResponse.TotalPages = totalPages;
        userResponse.CurrentPage = currentPage;
        userResponse.Success = true;

        return userResponse;
    }
    #endregion

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
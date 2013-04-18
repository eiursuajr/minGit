<%@ WebHandler Language="C#" Class="User" %>

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
    private List<UserData> _users = new List<UserData>();

    public UserResponse() { }
    
    public UserResponse(bool success, string errorMessage, List<UserData> users)
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
    
    public List<UserData> Users
    {
        get { return _users; }
        set { _users = value; }
    }
}

public class User : IHttpHandler
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
                    userResponse = Search(context.Request.Params["query"], context.Request.Params["field"]);
                    break;
                case "getPage":
                    userResponse = GetPage(
                        context.Request.Params["index"], 
                        context.Request.Params["pageSize"],
                        context.Request.Params["orderBy"]);
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
    protected UserResponse Search(string query, string field)
    {
        if (query == null) throw new MissingMethodException(@"""query"" parameter is missing.");

        UserResponse userResponse = new UserResponse();
        UserProperty userProperty = (UserProperty)Enum.Parse(typeof(UserProperty), field, true);

        Criteria<UserProperty> criteria = new Criteria<UserProperty>();
        criteria.AddFilter(userProperty, CriteriaFilterOperator.Contains, query);
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
        criteria.OrderByField = userProperty;
        criteria.PagingInfo = new PagingInfo(5, 1);
        
        userResponse.Users = UserService.GetList(criteria);
        userResponse.TotalPages = criteria.PagingInfo.TotalPages;
        userResponse.CurrentPage = criteria.PagingInfo.CurrentPage;
        userResponse.Success = true;

        return userResponse;
    }

    protected UserResponse GetPage(string index, string pageSize, string orderBy)
    {
        if (index == null) throw new MissingMethodException(@"""index"" parameter is missing.");
        if (pageSize == null) throw new MissingMethodException(@"""pageSize"" parameter is missing.");
        if (orderBy == null) throw new MissingMethodException(@"""orderBy"" parameter is missing.");

        UserResponse userResponse = new UserResponse();
        UserProperty orderByProperty = (UserProperty)Enum.Parse(typeof(UserProperty), orderBy, true);
        int currentPage = int.Parse(index);
        int recordsPerPage = int.Parse(pageSize);

        Criteria<UserProperty> criteria = new Criteria<UserProperty>();
        criteria.OrderByDirection = EkEnumeration.OrderByDirection.Ascending;
        criteria.OrderByField = orderByProperty;
        criteria.PagingInfo = new PagingInfo(recordsPerPage, currentPage);
        
        userResponse.Users = UserService.GetList(criteria);
        userResponse.TotalPages = criteria.PagingInfo.TotalPages;
        userResponse.CurrentPage = criteria.PagingInfo.CurrentPage;
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
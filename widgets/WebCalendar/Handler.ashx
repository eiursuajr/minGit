<%@ WebHandler Language="C#" Class="LSHandler" %>

using System;
using System.Web;
using System.Text;
using Ektron.Cms;
using System.Collections.Generic;
using Ektron.Cms.API.Search;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.WebSearch;
using Ektron.Cms.API.Content;
using Ektron.Cms.API;
using Ektron.Newtonsoft.Json;
using Ektron.Cms.Search;
using Ektron.Cms.Search.Compatibility;
using Ektron.Cms;
using Ektron.Cms.Framework.Search;
using Ektron.Cms.Search.Expressions;

#region RequestObject
[JsonObject(MemberSerialization.OptIn)]
public class Request
{
    [JsonProperty("action")]
    public string action = "";
    [JsonProperty("page")]
    public int page = 0;
    [JsonProperty("searchText")]
    public string searchText = "";
    [JsonProperty("objectID")]
    public long objectID = 0;
}
#endregion

#region ResponseObjects
[JsonObject(MemberSerialization.OptIn)]
public class DirectoryList
{
    [JsonProperty("subdirectories")]
    public List<DirectoryResult> SubDirectories = new List<DirectoryResult>();
}

[JsonObject(MemberSerialization.OptIn)]
public class DirectoryResult
{
    [JsonProperty("name")]
    public string Name = "";
    [JsonProperty("id")]
    public long id = 0;
    [JsonProperty("haschildren")]
    public bool HasChildren = false;
    [JsonProperty("iscalendar")]
    public bool IsCalendar = false;
}

[JsonObject(MemberSerialization.OptIn)]
public class UserList
{
    [JsonProperty("users")]
    public List<User> Users = new List<User>();
    [JsonProperty("pages")]
    public int Pages = 0;
    [JsonProperty("thispage")]
    public int ThisPage = 0;
}

[JsonObject(MemberSerialization.OptIn)]
public class User
{
    [JsonProperty("avatar")]
    public string Avatar = "";
    [JsonProperty("fname")]
    public string Fname = "";
    [JsonProperty("lname")]
    public string Lname = "";
    [JsonProperty("email")]
    public string Email = "";
    [JsonProperty("userid")]
    public long UserId = 0;
}

[JsonObject(MemberSerialization.OptIn)]
public class GroupList
{
    [JsonProperty("groups")]
    public List<Group> Groups = new List<Group>();
    [JsonProperty("pages")]
    public int Pages = 0;
    [JsonProperty("thispage")]
    public int ThisPage = 0;
}

[JsonObject(MemberSerialization.OptIn)]
public class Group
{
    [JsonProperty("avatar")]
    public string Avatar = "";
    [JsonProperty("gname")]
    public string Gname = "";
    [JsonProperty("gdesc")]
    public string Gdesc = "";
    [JsonProperty("groupid")]
    public long GroupId = 0;
}

[JsonObject(MemberSerialization.OptIn)]
public class Jsonexception
{
    [JsonProperty("message")]
    public string message = "";
    [JsonProperty("innerMessage")]
    public string innerMessage = "";
}
#endregion

public class LSHandler : IHttpHandler {
    private int contentPageSize = 8;
    private Request request;
    private Ektron.Cms.ContentAPI capi;
    HttpContext _context;

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        context.Response.Buffer = false;
        
        capi = new Ektron.Cms.ContentAPI();
        _context = context;
        
        try
        {
            string response = "";
            if (context.Request["request"] != null)
            {
                request = (Request) JsonConvert.DeserializeObject(context.Request["request"], typeof(Request));

                switch (request.action)
                {
                    case "getchildfolders":
                        response = getchildfolders();
                        break;
                    case "getusers":
                        response = getusers();
                        break;
                    case "getgroups":
                        response = getgroups();
                        break;
                }
            }
            context.Response.Write(response);
        }
        catch(Exception e)
        {
            Jsonexception ex = new Jsonexception();
            ex.message = e.Message;
            if (e.InnerException != null) ex.innerMessage = e.InnerException.Message;

            context.Response.Write(JsonConvert.SerializeObject(ex));
        }
        context.Response.End();
    }

   
    public string getchildfolders()
    {
        DirectoryList directoryInfo = new DirectoryList();
        if (request.objectID > -1)
        {
            //Folder fol = new Folder();
            FolderData[] fd = capi.GetChildFolders(request.objectID, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Name);
            
            if (fd != null && fd.Length > 0)
            {
                foreach (FolderData f in fd)
                {
                    DirectoryResult mytd = new DirectoryResult();
                    mytd.Name = f.Name;
                    mytd.id = f.Id;
                    mytd.HasChildren = f.HasChildren;
                    mytd.IsCalendar = f.FolderType == 8;
                    directoryInfo.SubDirectories.Add(mytd);
                }
            }
            return JsonConvert.SerializeObject(directoryInfo);
        }
        return "";
    }

    public string getusers()
    {
        UserList userInfo = new UserList();
        if (request.searchText != "")
        {
            KeywordSearchCriteria criteria = new KeywordSearchCriteria();
            criteria.IncludeSuggestedResults = false;
            criteria.ImplicitAnd = true;
            criteria.ReturnProperties = PropertyMappings.CommunitySearchProperties;
            criteria.PagingInfo = new PagingInfo(8);
            criteria.PagingInfo.CurrentPage = request.page;
            criteria.ExpressionTree = SearchContentProperty.ContentType.EqualTo(201);
            criteria.QueryText = request.searchText;
            
            ISearchManager manager = ObjectFactory.GetSearchManager();
            Ektron.Cms.Search.SearchResponseData responseData = manager.Search(criteria);
            
            userInfo.Pages = criteria.PagingInfo.TotalPages;
            userInfo.ThisPage = criteria.PagingInfo.CurrentPage;

            if (responseData != null)
            {
                foreach (Ektron.Cms.Search.SearchResultData u in responseData.Results)
                {
                    if (u.HasColumn(SearchUserProperty.Id) && u[SearchUserProperty.Id] != null && u[SearchUserProperty.Id] > 0)
                    {
                        User myus = new User();
                        myus.UserId = u[SearchUserProperty.Id];
                        myus.Avatar = "<img width=\"75\" height=\"75\" src=\"";
                        myus.Avatar += (u[SearchUserProperty.Avatar] != null && u[SearchUserProperty.Avatar].ToString().Length > 0) ? u[SearchUserProperty.Avatar].ToString() : capi.RequestInformationRef.ApplicationPath + "images/application/who.jpg";
                        myus.Avatar += "\" alt=\"user icon\" />";
                        myus.Email = (u[SearchUserProperty.EmailAddress] != null && u[SearchUserProperty.EmailAddress].ToString().Length > 0) ? u[SearchUserProperty.EmailAddress] : "";
                        myus.Fname = (u[SearchUserProperty.FirstName] != null) ? u[SearchUserProperty.FirstName] : "";
                        myus.Lname = (u[SearchUserProperty.LastName] != null) ? u[SearchUserProperty.LastName] : "";
                        userInfo.Users.Add(myus);
                    }
                }
            }
            return JsonConvert.SerializeObject(userInfo);
        }
        return "";
    }

    public string getgroups()
    {
        GroupList groupInfo = new GroupList();
        if (request.searchText != "")
        {
            KeywordSearchCriteria criteria = new KeywordSearchCriteria();
            criteria.IncludeSuggestedResults = false;
            criteria.ImplicitAnd = true;
            criteria.ReturnProperties = PropertyMappings.CommunitySearchProperties;
            criteria.PagingInfo = new PagingInfo(8);
            criteria.PagingInfo.CurrentPage = request.page;
            criteria.ExpressionTree = SearchContentProperty.ContentType.EqualTo(202);
            criteria.QueryText = request.searchText;
            
            ISearchManager manager = ObjectFactory.GetSearchManager();
            Ektron.Cms.Search.SearchResponseData responseData = manager.Search(criteria);

            groupInfo.Pages = criteria.PagingInfo.TotalPages;
            groupInfo.ThisPage = criteria.PagingInfo.CurrentPage;

            if (responseData != null)
            {
                foreach (Ektron.Cms.Search.SearchResultData u in responseData.Results)
                {
                    if (u.HasColumn(SearchGroupProperty.Id) && u[SearchGroupProperty.Id] != null && u[SearchGroupProperty.Id] > 0)
                    {
                        Group myus = new Group();
                        myus.GroupId = u[SearchGroupProperty.Id];
                        myus.Avatar = "<img width=\"75\" height=\"75\" src=\"";
                        myus.Avatar += (u[SearchGroupProperty.Avatar] != null && u[SearchGroupProperty.Avatar].ToString().Length > 0) ? u[SearchGroupProperty.Avatar].ToString() : capi.RequestInformationRef.ApplicationPath + "images/application/who.jpg";
                        myus.Avatar += "\" alt=\"group icon\" />";
                        myus.Gname = (u[SearchGroupProperty.Name] != null != null) ? u[SearchGroupProperty.Name].ToString().Replace("\\", "") : "";
                        myus.Gdesc = (u[SearchGroupProperty.ShortDescription] != null) ? u[SearchGroupProperty.ShortDescription] : "";
                        groupInfo.Groups.Add(myus);
                    }
                }
            }
            return JsonConvert.SerializeObject(groupInfo);
        }
        return "";
    }

    public bool IsReusable
    {
        get {
            return false;
        }
    }

}
<%@ WebHandler Language="C#" Class="autocomplete" %>

using System;
using System.Web;
using Ektron.Cms;
using System.Text;
 
public class autocomplete : IHttpHandler
{
    #region MemberVariables
    private Ektron.Cms.UserAPI _userApi = new UserAPI(); 
    private Ektron.Cms.Community.FriendsAPI _friendsApi = new Ektron.Cms.Community.FriendsAPI();
    private Ektron.Cms.Community.CommunityGroupAPI _communityGroupApi = new Ektron.Cms.Community.CommunityGroupAPI();
    protected UserData userInfo;
    protected DirectoryUserData[] friendList;
    protected CommunityGroupData[] groupList;
    protected System.Collections.Generic.List<string> suggestionList = new System.Collections.Generic.List<string>();
    #endregion
    
    public void ProcessRequest (HttpContext context) 
    {
        context.Response.ContentType = "text/plain";
        int totalpages = 0;
        int totalgroups = 0;
        userInfo = _userApi.GetUserById(_friendsApi.RequestInformationRef.UserId, false, false);
        friendList = _friendsApi.GetFriendsForUser(_friendsApi.RequestInformationRef.UserId);
        groupList = _communityGroupApi.GetCommunityGroupsForUser(_communityGroupApi.RequestInformationRef.UserId, 0,1000, ref totalpages, ref totalgroups);
        if (context.Request.QueryString["query"] != null)
        {
            context.Response.Write(GetSuggestionDataJSON(context));

        }
        else
        {
            context.Response.Write("No Suggestions");
        }   
    }
    public string GetSuggestionDataJSON(HttpContext context)
    {
        string query = context.Request.QueryString["query"];
        StringBuilder suggestBuilder = new StringBuilder();
        suggestBuilder.Append("{query:'" + query + "',suggestions:[");

        if (userInfo.DisplayName.ToLower().StartsWith(query.ToLower()))
        {
            suggestBuilder.Append("'" + userInfo.DisplayName + "',");
        }
        for (int i = 0; i <= friendList.Length-1; i++)
        {
            suggestionList.Add(friendList[i].DisplayName);
        }
       for (int i = 0; i <= groupList.Length - 1; i++)
       {
           if (!(suggestionList.Contains(groupList[i].GroupName))) // If group name and user name is the same add only once to the intellisense.
           {
               suggestionList.Add(groupList[i].GroupName);
           }
       }
       
       foreach (string suggestedItem in suggestionList)   // build the suggestion list using the list of friends and groups
       {
           if (suggestedItem.ToLower().StartsWith(query.ToLower()))
           {
               suggestBuilder.Append("'" + suggestedItem + "',");
           }    
       } 
       suggestBuilder = new StringBuilder(suggestBuilder.ToString(0, suggestBuilder.Length - 1));
       suggestBuilder.Append("]}");
       return suggestBuilder.ToString();
    }
    public bool IsReusable
    {
        get {
            return false;
        }
    }

}
using System;
using Ektron.Cms;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;

public partial class threadeddisc_threadeddiscussionRSS : System.Web.UI.Page
	{
		
		
		protected enum ThreadedDiscussionObjectType
		{
			board,
			forum,
			topic
		}
		protected ContentAPI apiContent = new ContentAPI();
		protected ThreadedDiscussionObjectType rssType;
		protected long rssID = 0;
		protected string rssURLpath = "";
		protected string sType = "";
		protected long groupID = -1;
		protected string rssHostUrl = null;
		protected void Page_Load(object sender, System.EventArgs e)
		{
            rssHostUrl = apiContent.RequestInformationRef.HostUrl;
			if (Request.QueryString["id"] != "" && Request.QueryString["type"] != "" && Request.QueryString["page"] != "")
			{
				try
				{
					Response.ContentType = "text/xml";
					Response.CacheControl = "no-cache";
					Response.AddHeader("Pragma", "no-cache");
					Response.Expires = -1;
					
					if ((string) (Strings.Trim(Request.QueryString["type"]).ToLower()) == "board")
					{
						rssType = ThreadedDiscussionObjectType.board;
					}
					else if ((string) (Strings.Trim(Request.QueryString["type"]).ToLower()) == "forum")
					{
						rssType = ThreadedDiscussionObjectType.forum;
					} // "topic"
					else
					{
						rssType = ThreadedDiscussionObjectType.topic;
					}
					if (! (Request.QueryString["groupid"] == null) && Request.QueryString["groupid"] != "")
					{
						groupID = Convert.ToInt64(Request.QueryString["groupid"]);
					}
					
					rssID = Convert.ToInt64(Request.QueryString["id"]);
					rssURLpath = EkFunctions.HtmlEncode(Request.QueryString["page"]);
					if ((apiContent.RequestInformationRef.HttpsProtocol.ToLower() == "on") && (apiContent.RequestInformationRef.HostSSLPort != "80")) 
					{
						if (!(apiContent.RequestInformationRef.HostSSLPort == "443"))
						{
							rssHostUrl = apiContent.RequestInformationRef.HostUrl + ":" + apiContent.RequestInformationRef.HostSSLPort;
						}
					}
					else 
					{
						if (!((apiContent.RequestInformationRef.HostPort == "80") || (apiContent.RequestInformationRef.HostPort == "443")))
						{
							rssHostUrl = apiContent.RequestInformationRef.HostUrl + ":" + apiContent.RequestInformationRef.HostPort;
						}
					}
					sType = System.Enum.GetName(typeof(ThreadedDiscussionObjectType), rssType);
					if (groupID != -1)
					{
                        Ektron.Cms.CustomAttributeList grpIdList = new CustomAttributeList();
                        grpIdList.Add(groupID.ToString());
                        Response.Write(apiContent.RenderThreadedDiscussionRSS(rssID, sType, rssURLpath, rssHostUrl, grpIdList));
					}
					else
					{
                        Response.Write(apiContent.RenderThreadedDiscussionRSS(rssID, sType, rssURLpath, rssHostUrl, null));
					}
					
					
				}
				catch (Exception ex)
				{
					Utilities.ShowError(ex.Message);
				}
			}
			
		}
		
	}

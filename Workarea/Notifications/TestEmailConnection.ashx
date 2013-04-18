<%@ WebHandler Language="C#" Class="TestEmailConnection" %>

using System;
using System.Web;
using System.Collections.Generic;

using OpenPOP.POP3;
using Ektron.Cms.Workarea.Framework;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Notifications;
using Ektron.Cms.Framework;
using Ektron.Cms.Core;

public class TestEmailConnection : WorkareaBaseHttpHandler
{

    public override void ProcessRequest(HttpContext context)
    {
        base.ProcessRequest(context);

        string message = "";
        POPClient popClient = new POPClient();
        try
        {

            bool useSSL = false;
            int port = 0;
            int.TryParse(context.Request["serverPort"], out port);

            if (!string.IsNullOrEmpty(context.Request["useSsl"]))
            {
                bool.TryParse(context.Request["useSsl"], out useSSL);
            }

            string password = "";
            if ((!string.IsNullOrEmpty(context.Request["password"])))
            {
                if (context.Request["password"].ToString().Trim() == "*****")
                {
                    //password has not changed, get old password from db
                    MailServerData mailServer = GetNotificationEmailServer();
                    if (mailServer != null) password = mailServer.POPPassword;
                }
                else
                    password = context.Request["password"].ToString();
            }
            popClient.Connect(context.Request["server"], port, useSSL);
            popClient.Authenticate(context.Request["account"], password, AuthenticationMethod.TRYBOTH);

            message = "Email Connection Test Successful.";

        }
        catch (System.IO.IOException)
        {
            message = "Email Connection Test Failed. Unable to connect to Server.";
        }
        catch (InvalidPasswordException)
        {
            message = "Email Connection Test Failed. Invalid User name or Password.";
        }
        catch (Exception)
        {
            message = "Email Connection Test Failed.";
        }
        finally
        {
            popClient.Disconnect();
        }
        context.Response.Write(message);
    }

    private MailServerData GetNotificationEmailServer()
    {
        IMailServer emailServerApi = ObjectFactory.GetMailServer();
        MailServerData mailServerData = new MailServerData();

        Criteria<MailServerProperty> criteria = new Criteria<MailServerProperty>();
        criteria.AddFilter(MailServerProperty.Type, CriteriaFilterOperator.EqualTo, MailServerType.CommunityEmailNotification);

        List<MailServerData> servers = emailServerApi.GetList(criteria);

        if (servers.Count > 0)
        {
            mailServerData = servers[0];
        }
        return mailServerData;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
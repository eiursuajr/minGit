using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Site;
using Ektron.Cms.User;
using Ektron.Cms.Content;
using Ektron.Cms;
using System.Text;
using Microsoft.VisualBasic;
using Ektron.Cms.Common;

public partial class Workarea_setcookie : System.Web.UI.Page
{

    protected ApplicationAPI AppUI = new ApplicationAPI();
    EkMessageHelper MsgHelper;
    int ContentLanguage;
   
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            MsgHelper = AppUI.EkMsgRef;
            if (Utilities.ValidateUserLogin() != false)
            {
                if (AppUI.RequestInformationRef.IsMembershipUser == 1 || AppUI.RequestInformationRef.UserId == 0)
                {
                    Response.Redirect("reterror.aspx?info=" + Server.UrlEncode(MsgHelper.GetMessage("msg login cms user")), false);
                    return;
                }


                if (!string.IsNullOrEmpty(Request.QueryString["LangType"]))
                {
                    ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                    AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
                else
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(AppUI.GetCookieValue("LastValidLanguageID"))))
                    {
                        ContentLanguage = Convert.ToInt32(AppUI.GetCookieValue("LastValidLanguageID"));
                    }
                }

                AppUI.ContentLanguage = ContentLanguage;
                string path = "";
                string qstring = "";

                path = Request.QueryString["path"];
                qstring = Strings.Replace(Request.ServerVariables["query_string"], "path=" + path, "", 1, -1, CompareMethod.Text);
                if (qstring.IndexOf("&LangType=") > 0)
                {
                    qstring = Strings.Replace(qstring, "&LangType=" + Request.QueryString["LangType"], "", 1, -1, CompareMethod.Text);
                }
                else
                {
                    qstring = Strings.Replace(qstring, "LangType=" + Request.QueryString["LangType"], "", 1, -1, CompareMethod.Text);
                }
                qstring = Strings.Replace(qstring, "&&", "&", 1, -1, CompareMethod.Text);

                path = path + qstring;
                path = Strings.Replace(path, "&", "?", 1, 1, CompareMethod.Text);
                if (Strings.Mid(path, path.Length, 1) == "?")
                {
                    path = Strings.Mid(path, 1, (path.Length - 1));
                }

                Response.Redirect(path);
            }
        }
        catch(Exception) 
        {
        
        }

    }
}

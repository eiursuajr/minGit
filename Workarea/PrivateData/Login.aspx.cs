using System;
using System.Web.UI.WebControls;
using Ektron.Cms;
using Ektron.Cms.Framework.UI;

public partial class Workarea_PrivateData_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Button btnLogin = login.FindControl("LoginButton") as Button;
        btnLogin.OnClientClick = "DoLogin()";
        RegisterResources();
    }

    protected void RegisterResources()
    {
        Packages.EktronCoreJS.Register(this);
    }

    protected void login_Authenticate(object sender, AuthenticateEventArgs e)
    {
        Login login = sender as Login;
        string username = login.UserName;
        string password = login.Password;
        Ektron.Cms.API.User.User user = new Ektron.Cms.API.User.User();
        UserData userData = user.LogInUser(username, password, "", "", "");
        if (userData != null)
        {
            user.SetAuthenticationCookie(userData);
            e.Authenticated = true;
        }
        else
        {
            e.Authenticated = false;
        }
    }
}

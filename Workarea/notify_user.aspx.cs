using Ektron.Cms;

public partial class notify_user : System.Web.UI.Page
	{
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			string splash = string.Empty;
			if (! string.IsNullOrEmpty(Request.QueryString["splash"]))
			{
				splash = Request.QueryString["splash"];
			}
			Ektron.Cms.Common.EkMessageHelper msg;
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			ltr_folderId.Text = Request.QueryString["folder_id"];
			msg = (new CommonApi()).EkMsgRef;
			Utilities.ValidateUserLogin();
			switch (splash.ToLower())
			{
				case "report":
					notifyUser.InnerHtml = "<img src=\"images/UI/splash/reports.jpg\" />";
					break;
				case "admintree":
					notifyUser.InnerHtml = "<img src=\"images/UI/splash/settings.jpg\" />";
					break;
				default:
					notifyUser.InnerHtml = msg.GetMessage("click folder nav msg");
					break;
			}
			
			msg = null;
		}
	}


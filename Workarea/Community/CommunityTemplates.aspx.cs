using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkEnumeration;

	public partial class Workarea_Community_CommunityTemplates : System.Web.UI.Page
	{
		
		protected EkMessageHelper m_refMsg;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected string m_strPageAction = "";
		protected string AppImgPath = "";
		protected ContentAPI m_refContentAPI = new ContentAPI();
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			displaystylesheet.Text = m_refStyle.GetClientScript();
			AppImgPath = m_refContentAPI.AppImgPath;
			m_refMsg = m_refContentAPI.EkMsgRef;
			Utilities.SetLanguage(m_refContentAPI);
			RegisterResources();
			if (! Utilities.ValidateUserLogin())
			{
				return;
			}
			if (! m_refContentAPI.IsAdmin())
			{
				Response.Redirect(m_refContentAPI.ApplicationPath + "reterror.aspx?info=" + m_refContentAPI.EkMsgRef.GetMessage("msg login cms administrator"), true);
				return;
			}
			lblGroupTemplates.Text = m_refMsg.GetMessage("lbl group templates");
			lblGroupCommunityDocuments.Text = m_refMsg.GetMessage("lbl community document") + ":";
			lblGroupPhotoGallery.Text = m_refMsg.GetMessage("lbl photo gallery") + ":";
			lblGroupBlog.Text = m_refMsg.GetMessage("lbl journal") + ":";
			lblGroupCalendar.Text = m_refMsg.GetMessage("calendar lbl") + ":";
			lblGroupProfile.Text = m_refMsg.GetMessage("lbl profile") + ":";
			lblGroupForum.Text = m_refMsg.GetMessage("lbl forum") + ":";
			
			lblUserTemplates.Text = m_refMsg.GetMessage("lbl user templates");
			lblUserCommunityDocuments.Text = m_refMsg.GetMessage("lbl community document") + ":";
			lblUserPhotoGallery.Text = m_refMsg.GetMessage("lbl photo gallery") + ":";
			lblUserBlog.Text = m_refMsg.GetMessage("lbl journal") + ":";
			lblUserCalendar.Text = m_refMsg.GetMessage("calendar lbl") + ":";
			lblUserProfile.Text = m_refMsg.GetMessage("lbl profile") + ":";
			
			TemplateData[] grouptemplate;
			TemplateData[] userTemplate;
			grouptemplate = m_refContentAPI.GetCommunityTemplate(Ektron.Cms.Common.EkEnumeration.TemplateType.Group);
			userTemplate = m_refContentAPI.GetCommunityTemplate(Ektron.Cms.Common.EkEnumeration.TemplateType.User);
			
			if (! Page.IsPostBack)
			{
				ViewAllToolBar();
				if ((grouptemplate != null)&& grouptemplate.Length > 0)
				{
					for (int i = 0; i <= grouptemplate.Length - 1; i++)
					{
						if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Workspace)
						{
							txtGroupCommunityDocuments.Text = grouptemplate[i].FileName;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Photos)
						{
							txtGroupPhotoGallery.Text = grouptemplate[i].FileName;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Profile)
						{
							txtGroupProfile.Text = grouptemplate[i].FileName;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Calendar)
						{
							txtGroupCalendar.Text = grouptemplate[i].FileName;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Forum)
						{
							txtGroupForum.Text = grouptemplate[i].FileName;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Blog)
						{
							txtGroupBlog.Text = grouptemplate[i].FileName;
						}
					}
				}
				
				if ((userTemplate != null)&& userTemplate.Length > 0)
				{
					for (int i = 0; i <= userTemplate.Length - 1; i++)
					{
						if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Workspace)
						{
							txtUserCommunityDocuments.Text = userTemplate[i].FileName;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Photos)
						{
							txtUserPhotoGallery.Text = userTemplate[i].FileName;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Profile)
						{
							txtUserProfile.Text = userTemplate[i].FileName;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Calendar)
						{
							txtUserCalendar.Text = userTemplate[i].FileName;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Blog)
						{
							txtUserBlog.Text = userTemplate[i].FileName;
						}
					}
				}
				
			}
			else
			{
				TemplateData data = null;
				if ((grouptemplate != null)&& grouptemplate.Length > 0)
				{
					for (int i = 0; i <= grouptemplate.Length - 1; i++)
					{
						data = new TemplateData();
						data.Type = Ektron.Cms.Common.EkEnumeration.TemplateType.Group;
						data.Id = grouptemplate[i].Id;
						if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Workspace)
						{
							data.FileName = (string) (txtGroupCommunityDocuments.Text.ToString());
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Workspace;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Photos)
						{
							data.FileName = (string) (txtGroupPhotoGallery.Text.ToString());
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Photos;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Profile)
						{
							data.FileName = (string) txtGroupProfile.Text;
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Profile;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Calendar)
						{
							data.FileName = (string) txtGroupCalendar.Text;
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Calendar;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Forum)
						{
							data.FileName = (string) txtGroupForum.Text;
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Forum;
						}
						else if (grouptemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Blog)
						{
							data.FileName = (string) (txtGroupBlog.Text.ToString());
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Blog;
						}
						if (data.FileName.Length > 0)
						{
							m_refContentAPI.EkContentRef.UpdateTemplatev2_0(data);
						}
					}
				}
				
				if ((userTemplate != null)&& userTemplate.Length > 0)
				{
					for (int i = 0; i <= userTemplate.Length - 1; i++)
					{
						data = new TemplateData();
						data.Type = Ektron.Cms.Common.EkEnumeration.TemplateType.User;
						data.Id = userTemplate[i].Id;
						if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Workspace)
						{
							data.FileName = (string) (txtUserCommunityDocuments.Text.ToString());
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Workspace;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Photos)
						{
							data.FileName = (string) (txtUserPhotoGallery.Text.ToString());
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Photos;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Profile)
						{
							data.FileName = (string) txtUserProfile.Text;
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Profile;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Calendar)
						{
							data.FileName = (string) txtUserCalendar.Text;
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Calendar;
						}
						else if (userTemplate[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Blog)
						{
							data.FileName = (string) (txtUserBlog.Text.ToString());
							data.SubType = Ektron.Cms.Common.EkEnumeration.TemplateSubType.Blog;
						}
						if (data.FileName.Length > 0)
						{
							m_refContentAPI.EkContentRef.UpdateTemplatev2_0(data);
						}
					}
				}
			}
			
		}
		
		private void ViewAllToolBar()
		{
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl templates"));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text"), m_refMsg.GetMessage("alt update button text"), "onclick=\"return Submit();\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("ViewEditCommunityTemplates", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
			
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronThickBoxCss);
		}
	}

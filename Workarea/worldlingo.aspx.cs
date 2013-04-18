using System;
using System.Text;
using System.Web.UI.WebControls;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.UI.CommonUI;
using Microsoft.VisualBasic;

	public partial class worldlingo : System.Web.UI.Page
	{
	
		protected string htmleditor = "";
		protected string htmcontent = "";
		protected StyleHelper m_refStyle = new StyleHelper();
		protected string TargetLanguage;
		protected string SourceLanguage;
        protected const int ALL_CONTENT_LANGUAGES = -1;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
            RegisterResources();
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
            stylesheetjs.Text = m_refStyle.GetClientScript();

			ApplicationAPI AppUI = new ApplicationAPI();
			Ektron.Cms.Common.EkMessageHelper MsgHelper = AppUI.EkMsgRef;
			long currentUserID = AppUI.UserId;
			int EnableMultilingual = AppUI.EnableMultilingual;
			
			string AppPath = AppUI.AppPath;
			string AppImgPath = AppUI.AppImgPath;
			string SelectedEditControl = "";
			string sitePath = AppUI.SitePath;
			string AppName = AppUI.AppName;
			string AppeWebPath = AppUI.AppeWebPath;
			if (AppUI.RequestInformationRef.IsMembershipUser == 1 || AppUI.RequestInformationRef.UserId == 0)
			{
				Response.Redirect("blank.htm", false);
				return;
			}
			
			this.pageTitle.Text = MsgHelper.GetMessage("ektron translation");
			btnTranslate.Text = MsgHelper.GetMessage("lbl translate");
			btnTranslate.ToolTip = btnTranslate.Text;
			TransTitle.Text = m_refStyle.GetTitleBar(MsgHelper.GetMessage("machine translation title"));
			TransTitle.ToolTip = TransTitle.Text;
			lblSrcLang.Text = MsgHelper.GetMessage("lbl source language");
			lblSrcLang.ToolTip = lblSrcLang.Text;
			lblTrgLang.Text = MsgHelper.GetMessage("lbl target language");
			lblTrgLang.ToolTip = lblTrgLang.Text;
			lblGlossary.Text = MsgHelper.GetMessage("lbl glossary");
			lblGlossary.ToolTip = lblGlossary.Text;
			StringBuilder sbButton = new StringBuilder();
			sbButton.AppendLine("<table width=\"100%\">");
			sbButton.AppendLine("<tr>");
			SelectedEditControl = Utilities.GetEditorPreference(Request);
			if (SelectedEditControl == "ContentDesigner")
			{
				sbButton.AppendLine(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic cancel"), MsgHelper.GetMessage("btn cancel"), "Onclick=\"javascript:CloseDlg();\"", StyleHelper.CancelButtonCssClass,true));
			}
			else
			{
                sbButton.AppendLine(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/cancel.png", "#", MsgHelper.GetMessage("generic cancel"), MsgHelper.GetMessage("btn cancel"), "Onclick=\"javascript:window.close();\"", StyleHelper.CancelButtonCssClass, true));
			}
			sbButton.AppendLine("</tr>");
			sbButton.AppendLine("</table>");
			tblButton.InnerHtml = sbButton.ToString();
			
			if (IsPostBack)
			{
				formpage.Visible = false;
				resultPage.Visible = true;
			}
			else
			{
				ListItem  selLang;
				string lang;
				SourceLanguage = "0";
				lang = Request.QueryString["DefaultContentLanguage"];
				if (Information.IsNumeric(lang))
				{
					SourceLanguage = lang;
				}
				if (Convert.ToInt32(SourceLanguage) <= 0)
				{
					SourceLanguage = "1033"; // English
				}
				string langCode;
				langCode = (string) (LangSwitch(SourceLanguage));
				selLang = wl_srclang.Items.FindByValue(langCode);
				if (selLang != null)
				{
					selLang.Selected = true;
				}
				
				htmleditor = Request.QueryString["htmleditor"];
				TargetLanguage = "0";
				lang = Request.QueryString["LangType"];
				if (Information.IsNumeric(lang))
				{
					TargetLanguage = lang;
				}
				if (Convert.ToInt32(TargetLanguage) <= 0)
				{
					TargetLanguage = "1033";
				}
				langCode = (string) (LangSwitch(TargetLanguage));
				selLang = wl_trglang.Items.FindByValue(langCode);
				if (selLang != null)
				{
					selLang.Selected = true;
				}
				
				formpage.Visible = true;
				resultPage.Visible = false;
			}
			
		}

        protected void RegisterResources()
        {
            Packages.Ektron.Workarea.Core.Register(this);
        }

		private string LangSwitch(string lookuplang)
		{
			string langCode;
			switch (lookuplang)
			{
				case "2052":
					langCode = "zh_cn";
					break;
				case "1028":
					langCode = "zh_tw";
					break;
				case "1043":
					langCode = "nl";
					break;
				case "1036":
					langCode = "fr";
					break;
				case "1031":
					langCode = "de";
					break;
				case "1032":
					langCode = "el";
					break;
				case "1040":
					langCode = "it";
					break;
				case "1041":
					langCode = "ja";
					break;
				case "1042":
					langCode = "ko";
					break;
				case "1046":
					langCode = "pt";
					break;
				case "1049":
					langCode = "ru";
					break;
				case "1034":
					langCode = "es";
					break;
				case "1033":
					langCode = "en";
					break;
				default:
					langCode = "en";
					break;
			}
			return langCode;
		}
		
		protected void btnTranslate_Click(System.Object sender, System.EventArgs e)
		{
			string retContent;
			Ektron.Cms.LocalizationAPI LocalizeAPI = new Ektron.Cms.LocalizationAPI();
			retContent = LocalizeAPI.TranslateUsingWorldLingo((string) wl_data.Value, "text/html", (string) wl_srclang.SelectedValue, (string) wl_trglang.SelectedValue, (string) wl_gloss.Value);
			displaycontent.InnerHtml = retContent;
			returnContent.Value = retContent;
		}
	}
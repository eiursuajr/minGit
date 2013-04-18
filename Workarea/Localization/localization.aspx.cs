using System;
using System.Web.UI.WebControls;

public partial class Workarea_localization : Ektron.Cms.Workarea.Framework.WorkAreaBasePage
{

	#region "Member Variables"
	protected Ektron.Cms.LocalizationAPI m_objLocalizationApi = new Ektron.Cms.LocalizationAPI();
	protected StyleHelper m_refStyle = new StyleHelper();
	protected string AppImgPath = "";
	protected Ektron.Cms.SiteAPI m_refSiteApi = new Ektron.Cms.SiteAPI();
	#endregion

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		Response.CacheControl = "no-cache";
		Response.AddHeader("Pragma", "no-cache");
		Response.Expires = -1;
		
		Utilities.ValidateUserLogin();
		RegisterResources();

		SetServerJSVariables();

		AppImgPath = m_objLocalizationApi.AppImgPath;
		StyleSheetJS.Text = m_refStyle.GetClientScript();
		if ((!Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_objLocalizationApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.Xliff, false)))
		{
			Utilities.ShowError(GetMessage("feature locked error"));
		}
		if ((IsPostBack))
		{
			string strPath = m_objLocalizationApi.GetTranslationUploadDirectory();

			ProcessFileUpload(FileUpload0, strPath, FileUploadLabel0);
			ProcessFileUpload(FileUpload1, strPath, FileUploadLabel1);
			ProcessFileUpload(FileUpload2, strPath, FileUploadLabel2);
			ProcessFileUpload(FileUpload3, strPath, FileUploadLabel3);
			ProcessFileUpload(FileUpload4, strPath, FileUploadLabel4);
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		if (m_refSiteApi.RequestInformationRef.IsMembershipUser == 1 || m_refSiteApi.RequestInformationRef.UserId == 0)
		{
			Response.Redirect("../blank.htm", false);
			return;
		}
		LoadingImg.Text = GetMessage("one moment msg");
		GenerateToolbar();
		if ((IsPostBack))
		{
			ImportTranslation();
		}
	}


	private void GenerateToolbar()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();

		string WorkareaTitlebarTitle = null;
		WorkareaTitlebarTitle = GetMessage("lbl import translated XLIFF files");
		txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(WorkareaTitlebarTitle);

		result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refSiteApi.AppPath + "Images/ui/icons/translationImport.png", "#", GetMessage("alt Click here to upload and import translated XLIFF files"), GetMessage("lbl import translated XLIFF files"), "onclick='return SubmitForm(0,\"validate()\")'", StyleHelper.TranslationButtonCssClass, true));
		result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
		result.Append(m_refStyle.GetHelpButton("import_xliff_files", ""));
		result.Append("</td>");
		result.Append("</tr></table>");
		htmToolBar.InnerHtml = result.ToString();
	}

	private void ProcessFileUpload(FileUpload ctlFileUpload, string Path, Label ctlLabel)
	{
		{
			ctlLabel.Text = "";
			if ((ctlFileUpload.HasFile))
			{
				string strFileExt = System.IO.Path.GetExtension(ctlFileUpload.FileName).ToLower();
				// .xml for Trados
				if ((".xlf.xml.zip".Contains(strFileExt)))
				{
					try
					{
						ctlFileUpload.PostedFile.SaveAs(Ektron.Cms.Common.EkFunctions.QualifyURL(Path, ctlFileUpload.FileName));
					}
					catch (Exception ex)
					{
						ctlLabel.Text = "<p>Failed to upload file: " + ctlFileUpload.FileName + "<br />" + ex.Message + "</p>";
					}
				}
				else
				{
					ctlLabel.Text = "<p>Cannot accept files of this type: " + strFileExt + "</p>";
				}
			}
		}
	}

	#region "LOCALIZATION - Import"
	private void ImportTranslation()
	{
		m_objLocalizationApi.StartImportTranslation();
		//If (Not IsNothing(objLocalizationData)) Then
		//	TranslationUploadLabel.Text = ""

		//	' TODO dwd
		//	''LocalizationReport.Text = GenerateLogReport(objLocalizationData.LogFileUrl)
		//Else
		//	TranslationUploadLabel.Text = "No translated files were found."
		//End If
	}
	#endregion

	//Private Function GenerateLogReport(ByVal LogFileUrl As String) As String
	//	Dim sbReport As New StringBuilder
	//	sbReport.Append("<iframe width='95%' height='400' src=""" & LogFileUrl & """>")
	//	sbReport.Append("<a href=""" & LogFileUrl & """ target='_blank'>View report log</a>")
	//	sbReport.Append("</iframe>")
	//	Return sbReport.ToString
	//End Function
	protected void RegisterResources()
	{
		Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);

		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronModalJS);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
		Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
	}

	protected void SetServerJSVariables()
	{
		ltr_fileMissing.Text = GetMessage("alt one or more files could not be found to be uploaded.");
		ltr_Permitted.Text = GetMessage("alt Only XLIFF files (.xlf, .xml, or .zip) are permitted.");
		ltr_selectOne.Text = GetMessage("alt Please select at least one XLIFF (.xlf, .xml, or .zip) file.");
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik, @toddanglin
//Facebook: facebook.com/telerik
//=======================================================

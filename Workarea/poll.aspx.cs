using System;
using System.Collections;
using Ektron.Cms;
using Microsoft.VisualBasic;

public partial class poll : System.Web.UI.Page
	{
        Ektron.Cms.ContentAPI m_refContentApi = new ContentAPI();
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			string strResult = "";
			string sZoom = "";
			string sAppPath = "";
		    
			sAppPath = m_refContentApi.AppPath;
			string sSitePath = "";
			sSitePath = m_refContentApi.SitePath;
			StyleHelper aSytleHelper = new StyleHelper();

			StyleSheetJS.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + sSitePath + "default.css\" />" + "\r\n";
            StyleSheetJS.Text = StyleSheetJS.Text + aSytleHelper.GetClientScript();
			
			if (!(Request.QueryString["zoom"] == null))
			{
				if (Request.QueryString["zoom"] != "1")
				{
					sZoom = (string) (Request.QueryString["zoom"].ToString());
				}
			}
			
			if (Request.QueryString["display_type"] == null)
			{
				//display form block or report in IFRAME
				ReportForm.Visible = false;
				PollForm.Visible = true;
				if (sZoom != "")
				{
					PollForm.Style["zoom"] = sZoom;
				}
				
			}
			else
			{
				//post back from "inline" DisplayType "new window", originally in formresponse.aspx
				PollForm.Visible = false;
				ReportForm.Visible = true;
				if (sZoom != "")
				{
					ReportForm.Style["zoom"] = sZoom;
				}
				
				Collection cForm;
				string XmlData = "";
				string DisplayType = "";
				ArrayList arrResult = new ArrayList();
				Hashtable hshQuestions = null;
				int llResponses;
				
				System.Xml.Xsl.XsltArgumentList objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
				
				Response.CacheControl = "no-cache";
				Response.AddHeader("Pragma", "no-cache");
				Response.Expires = -1;
				
				//Put user code to initialize the page here
				var ContentLanguage = m_refContentApi.ContentLanguage;
				var AppImgPath = m_refContentApi.AppImgPath;
				var FormId = Request.QueryString["id"];
				DisplayType = Request.QueryString["display_type"];
				
				if (Request.QueryString["LangType"] != "")
				{
                    ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
					m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
				}
				else
				{
					if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
					{
						ContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
					}
				}
				
				var EnableMultilingual = m_refContentApi.EnableMultilingual;
				m_refContentApi.ContentLanguage = ContentLanguage;

                var Security_info = m_refContentApi.LoadPermissions(Convert.ToInt64(FormId), "content", 0);
				var objMod = m_refContentApi.EkModuleRef;
                llResponses = Convert.ToInt32(m_refContentApi.EkModuleRef.GetFormSubmissionsByFormId(Convert.ToInt64(FormId)));
                cForm = objMod.GetFormById(Convert.ToInt64(FormId));
				
				if ((Convert.ToString(FormId) != "") && (Convert.ToInt32(FormId) > 0))
				{
					if ((((DisplayType) == Ektron.Cms.Common.EkEnumeration.CMSFormReportType.DataTable.ToString()) || ((DisplayType) == Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Bar.ToString())) || ((DisplayType) == Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Combined.ToString()))
					{
                        arrResult = m_refContentApi.GetFormDataHistogramById(Convert.ToInt64(FormId));
						hshQuestions = m_refContentApi.GetFormFieldQuestionsById(Convert.ToInt64(FormId));
						XmlData = m_refContentApi.EkModuleRef.CreateHistogramFormDataXml(cForm, llResponses, arrResult, hshQuestions, "");
					}
					else if ((DisplayType) == Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Pie.ToString())
					{
						
						bool showAxis = false;
						string ImageUrl;
						//ImageUrl = sAppPath & "chart.aspx?form_page=form_page&amp;grpdisplay=" & DisplayType & "&amp;responses=" & llResponses & "&amp;showAxis=" & showAxis & "&amp;fieldOptionNames=" & strFieldOptionNames & "&amp;FieldNames=" & strFieldNames & "&amp;FormValues=" & strFieldOptionValues
						ImageUrl = sAppPath + "chart.aspx?form_page=form_page&amp;grpdisplay=" + DisplayType + "&amp;responses=" + llResponses + "&amp;showAxis=" + showAxis + "&amp;formid=" + FormId;
						XmlData = m_refContentApi.EkModuleRef.CreateHistogramFormDataXml(cForm, llResponses, arrResult, hshQuestions, ImageUrl);
					}
					else
					{
					}
				}
				
				lblReport.Text = "";
				objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
				objXsltArgs.AddParam("displayType", string.Empty, DisplayType);
				objXsltArgs.AddParam("appPath", string.Empty, sAppPath);
				try
				{
					strResult = m_refContentApi.EkModuleRef.InsertRefreshReportJS("");
					strResult = strResult + Ektron.Cms.Common.EkFunctions.XSLTransform(null, null, true, false, objXsltArgs, true, null);
				}
				catch (Exception ex)
				{
					strResult = ex.Message;
				}
				lblReport.Text = strResult;
				
			}
		}
		
	}
	
using System;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Microsoft.VisualBasic;

	public partial class previewwebalert : System.Web.UI.Page
	{
		protected EkMessageHelper m_refMsg;
		protected ContentAPI m_refContApi = new ContentAPI();
		protected StyleHelper m_refStyle = new StyleHelper();
		protected string AppImgPath = "";
		private EkContent m_refContent = new Ektron.Cms.Content.EkContent();
		private int m_content_type = 0;

		private string m_sContentURL = "";
		private string m_sContentLink = "";

		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			try
			{
				m_refMsg = m_refContApi.EkMsgRef;
				StyleSheetJS.Text = m_refStyle.GetClientScript();
				AppImgPath = m_refContApi.AppImgPath;
				RegisterResources();

				m_content_type = Convert.ToInt32(Request.QueryString["content_type"]);

				divTitleBar.Visible = true;
				divToolBar.Visible = true;

				if ((m_content_type == 3) || (m_content_type == 4)) //form
				{
					m_sContentURL = (string) ("http://" + Request.ServerVariables["http_host"] + m_refContApi.AppPath + "LinkIt.aspx?LinkIdentifier=ekfrm&ItemID=" + Request.QueryString["content"]);
				}
				else
				{
					m_sContentURL = (string) ("http://" + Request.ServerVariables["http_host"] + m_refContApi.AppPath + "LinkIt.aspx?LinkIdentifier=id&ItemID=" + Request.QueryString["content"]);
				}
				m_sContentLink = "<a href=\"" + m_sContentURL + "\">" + m_sContentURL + "</a>";

				Display_Preview();
			}
			catch (Exception ex)
			{
				Utilities.ShowError(EkFunctions.UrlEncode(ex.Message));
			}
		}

		private void Display_Preview()
		{
			Ektron.Cms.Common.ContentBase cbContent = new Ektron.Cms.Common.ContentBase();
			TR_Preview.Visible = true;

			ltrJS.Text += "<script type=\"text/javascript\" language=\"JavaScript\">" + Environment.NewLine;
			ltrJS.Text += "    var contentURL = \'" + m_sContentURL + "\';" + Environment.NewLine;
			ltrJS.Text += "    var contentLink = \'" + m_sContentLink + "\';" + Environment.NewLine;
			ltrJS.Text += "    var rExp = /@appContentLink@/gi;" + Environment.NewLine;
			ltrJS.Text += "</script>" + Environment.NewLine;
			ltrOptOut.Text = PostProcess((string) (m_refContApi.EkContentRef.GetSubscriptionMessageByID(Convert.ToInt64(Request.QueryString["optoutid"]))["Text"])) + "<br/>";
            if (Convert.ToInt32(Request.QueryString["defmsg"]) > 0)
			{
				ltrDefault.Text = "\r\n" + PostProcess((string) (m_refContApi.EkContentRef.GetSubscriptionMessageByID(Convert.ToInt64(Request.QueryString["defmsg"]))["Text"])) + "<br/>";
			}
			else
			{
				ltrDefault.Text = "";
			}
			
            if (Convert.ToBoolean(Request.QueryString["summaryid"]))
			{
				if ( Convert.ToInt32(Request.QueryString["content"]) == -1 && m_content_type == 0)
				{
					ltrSummary.Text = "<p>The summary to your Content Block/Form/Asset will go here.</p>";
					ltrSummary.Text += "<br/>";
				}
				else
				{
					ltrSummary.Text = "\r\n";
					ltrSummary.Text += "<script type=\"text/javascript\" language=\"JavaScript\">" + Environment.NewLine;
					ltrSummary.Text += "    var textsectionSumm = \'content_teaser\';" + Environment.NewLine;
					ltrSummary.Text += "    if (typeof(top.opener.eWebEditPro) != \"undefined\" && typeof(top.opener.eWebEditPro[textsectionSumm]) != \"undefined\") " + Environment.NewLine;
					ltrSummary.Text += "    {" + Environment.NewLine;
					ltrSummary.Text += "    var selectedTextSumm = top.opener.eWebEditPro[textsectionSumm].getBodyHTML();" + Environment.NewLine;
					ltrSummary.Text += "    selectedTextSumm = selectedTextSumm.replace(rExp, \'" + m_sContentLink + "\');" + Environment.NewLine;
					ltrSummary.Text += "    document.writeln(selectedTextSumm);" + Environment.NewLine;
					ltrSummary.Text += "    }" + Environment.NewLine;
					ltrSummary.Text += "</script>" + Environment.NewLine;
					ltrSummary.Text += "<br/>";
				}
			}
            if (Convert.ToInt32(Request.QueryString["content"]) == -1 && m_content_type == 0 && Convert.ToInt32(Request.QueryString["usecontentid"]) == -1)  //using current content for a folder
			{
				ltrContent.Text = "<p>The HTML of your content block or the link to the form or asset will go here.</p>";
			}
			else if (Convert.ToInt32(Request.QueryString["usecontentid"]) == -1)
			{
				if ((m_content_type == 1) || (m_content_type == 3))
				{
					ltrContent.Text = "\r\n";
					ltrContent.Text += "<script type=\"text/javascript\"  language=\"JavaScript\">" + Environment.NewLine;
					ltrContent.Text += "    var textsection = \'content_html\';" + Environment.NewLine;
					ltrContent.Text += "    if (typeof(top.opener.eWebEditPro) != \"undefined\" && typeof(top.opener.eWebEditPro[textsection]) != \"undefined\") " + Environment.NewLine;
					ltrContent.Text += "    {" + Environment.NewLine;
					ltrContent.Text += "    var selectedText = top.opener.eWebEditPro[textsection].getBodyHTML();" + Environment.NewLine;
					ltrContent.Text += "    if (selectedText.indexOf(\'<?xml\') > -1) {" + Environment.NewLine;
					ltrContent.Text += "    	selectedText = \'The XML content using the default packaged XSLT will go here.<br/>\';" + Environment.NewLine;
					ltrContent.Text += "    } else {" + Environment.NewLine;
					ltrContent.Text += "        selectedText = selectedText.replace(rExp, \'" + m_sContentLink + "\');" + Environment.NewLine;
					ltrContent.Text += "    }" + Environment.NewLine;
					ltrContent.Text += "    document.writeln(selectedText);" + Environment.NewLine;
					ltrContent.Text += "    }" + Environment.NewLine;
					ltrContent.Text += "</script>" + Environment.NewLine;
				}
				else if ((m_content_type == 2) || (m_content_type == 4)) //form
				{
					ltrContent.Text += "<a href=\"#\" Onclick=\"javascript: alert(\'This will link to the published form.\'); return false;\">View the form</a><br/>";
				}
				else // assume document
				{
					ltrContent.Text += "<a href=\"#\" Onclick=\"javascript: alert(\'This will link to the published asset.\'); return false;\">View the asset</a><br/>";
				}
			}
            else if (Convert.ToInt32(Request.QueryString["usecontentid"]) > 0)
			{
				try
				{
                    cbContent = m_refContApi.EkContentRef.LoadContent(Convert.ToInt64(Request.QueryString["usecontentid"]), false);
                    m_content_type = Convert.ToInt32(cbContent.ContentType);
					if ((m_content_type == 1) || (m_content_type == 3))
					{
						ltrContent.Text = "\r\n";
						ltrContent.Text += PostProcess(m_refContApi.EkContentRef.TransformXsltPackage(cbContent.Html, cbContent.PackageDisplayXslt,false));
						cbContent = null;
					}
					else if ((m_content_type == 2) || (m_content_type == 4)) //form
					{
						ltrContent.Text += "<a href=\"#\" Onclick=\"javascript: alert(\'This will link to the published form.\'); return false;\">View the form</a><br/>";
					}
					else // assume document
					{
						ltrContent.Text += "<a href=\"#\" Onclick=\"javascript: alert(\'This will link to the published asset.\'); return false;\">View the asset</a><br/>";
					}
				}
				catch (Exception)
				{
					//doesn't exist
					ltrContent.Text = "<p>The HTML of your content block or the link to the form or asset will go here.</p>";
				}
			}
			ltrContent.Text += "<br/>";
            if (Convert.ToInt32(Request.QueryString["uselink"]) == 1)
			{
				ltrContentLink.Text = "Read More: <a href=\"" + m_sContentURL + "\">" + m_sContentURL + "</a><br/>";
			}
            ltrUnsubscribe.Text = "\r\n" + PostProcess((string)(m_refContApi.EkContentRef.GetSubscriptionMessageByID(Convert.ToInt64(Request.QueryString["unsubscribeid"]))["Text"])) + "<br/>";
			PreviewToolBar();
		}

		private void PreviewToolBar()
		{
			divTitleBar.InnerHtml = "Preview Web Alert";
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/cancel.png", "#", "Close Window", "Close Window", " Onclick=\"javascript:self.close();\" ", StyleHelper.CancelButtonCssClass, true));
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}

		private string PostProcess(string sText)
		{
			sText = Strings.Replace(sText, "@appContentLink@", m_sContentLink, 1, -1, CompareMethod.Text);
			return sText;
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, m_refContApi.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
			Ektron.Cms.API.JS.RegisterJS(this, m_refContApi.ApplicationPath + "java/workareahelper.js", "EktronWorkareaHelperJS");
		}
	}

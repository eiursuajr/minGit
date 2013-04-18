using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.UI.CommonUI;

	public partial class BadLinkCheck : System.Web.UI.Page
	{
		
		protected EkRequestInformation m_RequestInfo = null;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected EkMessageHelper m_refMsg;
		
		protected void btnCheck_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (txtURL.Text != "")
			{
				if (txtURL.Text.IndexOf("://") < 0)
				{
					txtURL.Text = "http://" + txtURL.Text;
				}
				EkThreads.URLCheckerClass.CheckURL = (string) txtURL.Text;
				EkThreads.URLCheckerClass.Debug = true;
			}
			ApplicationAPI m_AppRef = new ApplicationAPI();
			EkThreads.URLCheckerClass.getInstance().Start(m_AppRef.RequestInformationRef, Request.Url.Authority);
			int i;
			for (i = 1; i <= 20; i++)
			{
				Thread.Sleep(100); // give thread some time to spin up
				if (EkThreads.URLCheckerClass.ThreadRunning)
				{
					break;
				}
			}
			Response.Redirect("BadLinkCheck.aspx", false);
		}
		
		protected void btnCancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			EkThreads.URLCheckerClass.Cancel = true;
			// give thread some time to stop
			int i;
			for (i = 1; i <= 20; i++)
			{
				Thread.Sleep(100); // give thread some time to shut down
				if (! EkThreads.URLCheckerClass.ThreadRunning)
				{
					break;
				}
			}
			Response.Redirect("BadLinkCheck.aspx", false);
		}
		
		private void Page_Load(object sender, EventArgs e)
		{
			bool IsRunning = EkThreads.URLCheckerClass.ThreadRunning;
			StyleSheetJS.Text = m_refStyle.GetClientScript();
			SiteAPI m_refSiteApi = new SiteAPI();
			m_refMsg = m_refSiteApi.EkMsgRef;
            btnCheck.ToolTip = m_refMsg.GetMessage("btn Check Links");
			if (m_RequestInfo == null)
			{
				ContentAPI refContentApi = new ContentAPI();
				m_RequestInfo = refContentApi.RequestInformationRef;
			}
			RegisterResources();
			Utilities.ValidateUserLogin();
			if ( Convert.ToBoolean( m_RequestInfo.IsMembershipUser ) || m_RequestInfo.UserId == 0)
			{
				Response.Redirect("blank.htm", false);
				return;
			}
			
			if (IsRunning)
			{
				Response.AppendHeader("Refresh", "5");
			}
			
			// set initial values of fields on page
			if (Page.IsPostBack == false)
			{
				var m_refContentApi = new ContentAPI();
				var styleHelper = new StyleHelper();
				
				if (IsRunning)
				{
					checkWrapper.Visible = false;
					btnCheck.Visible = false;
					//printWrapper.Visible = false;
					//btnPrint.Visible = false;
					cancelWrapper.Visible = true;
					btnCancel.Visible = true;
					//CancelButton.Text = styleHelper.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/cancel.png", "javascript:btnCancel_Click();", m_refMsg.GetMessage("generic cancel"), m_refMsg.GetMessage("generic cancel"), "", StyleHelper.CancelButtonCssClass, true);
					lnkTabTestURL.Visible = false;
					lnkTabStatus.Enabled = false;
					// purge page state
					HttpContext.Current.Session["_PAGE_STATE_" + Request.Url.AbsolutePath] = null;
				}
				else
				{
					//CancelButton.Text = String.Empty;
					PrintButton.Text = styleHelper.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/print.png", "#", m_refMsg.GetMessage("Print Report button text"), m_refMsg.GetMessage("btn print"), "onclick=\"PrintReport();\"", StyleHelper.PrintButtonCssClass, true);

					btnCancel.Visible = false;
					cancelWrapper.Visible = false;
				}
				
				if (ConfigurationManager.AppSettings["ek_DebugLinkCheck"] != "True")
				{
					lnkTabTestURL.Visible = false;
				}
				
				if (! IsRunning)
				{
					txtStatus.Text = m_refMsg.GetMessage("txt linkcheck idle");
					txtStatus.ToolTip = txtStatus.Text;
				}
				else
				{
					txtStatus.Text = EkThreads.URLCheckerClass.ThreadStatus;
					txtStatus.ToolTip = txtStatus.Text;
				}
				
				if (EkThreads.URLCheckerClass.ThreadLog.Length == 0)
				{
					ApplicationAPI m_AppRef = new ApplicationAPI();
					string reportfile = m_AppRef.RequestInformationRef.PhysicalAppPath + "ekbadlinkrpt.html";
					if (File.Exists(reportfile))
					{
						System.IO.StreamReader oRead;
						try
						{
							oRead = System.IO.File.OpenText(reportfile);
							EkThreads.URLCheckerClass.ThreadLog = new StringBuilder(oRead.ReadToEnd());
						}
						catch
						{
							// ignore errors reading from report file
						}
					}
					else
					{
						txtStatus.Text = ""; // don't display status if it was never run
						txtStatus.ToolTip = txtStatus.Text;
					}
				}
				
				if (EkThreads.URLCheckerClass.ThreadLog.Length > 0)
				{
					txtReport.Text = txtReport.Text;
					if (IsRunning)
					{
						txtReport.Text = txtReport.Text + "<ul style=\'margin: .5em 2em\'>" + "<li>Objects Checked: " + EkThreads.URLCheckerClass.CountObjsChecked.ToString();
						txtReport.Text = txtReport.Text + "</ li>" + "<li>Links Checked: " + EkThreads.URLCheckerClass.CountLinksChecked.ToString();
						txtReport.Text = txtReport.Text + "</li>" + "<li>Bad Links: " + EkThreads.URLCheckerClass.CountBadLinks.ToString() + "</li></ul>";
						
						if (EkThreads.URLCheckerClass.CountBadLinks > 500)
						{
							txtReport.Text = txtReport.Text + "Too many errors.  Please wait for report to be completed.";
						}
						else
						{
							txtReport.Text = txtReport.Text + EkThreads.URLCheckerClass.ThreadLog.ToString();
						}
					}
					else
					{
						txtReport.Text = txtReport.Text + "<span id=\"ReportDataGrid\">";
						txtReport.Text = txtReport.Text + "<span id=\"viewApprovalList_ViewGrid\">";
						txtReport.Text = txtReport.Text + "<table><tr><td>";
						txtReport.Text = txtReport.Text + EkThreads.URLCheckerClass.ThreadLog.ToString();
						txtReport.Text = txtReport.Text + "</td></tr></table>";
						txtReport.Text = txtReport.Text + "</span>";
						txtReport.Text = txtReport.Text + "</span>";
					}
				}
				
				// handle localization text
				StyleHelper m_refStyle1 = new StyleHelper();
				if (! IsRunning)
				{
					m_refStyle1.MakeToolbarButton(btnCheck, m_refMsg.GetMessage("alt linkcheck button text"), m_refMsg.GetMessage("alt linkcheck button text"));
					//m_refStyle1.MakeToolbarButton(btnPrint, m_refMsg.GetMessage("btn print"), m_refMsg.GetMessage("btn print"));
				}
				else
				{
					//m_refStyle1.MakeToolbarButton(btnCancel, m_refMsg.GetMessage("generic cancel"), m_refMsg.GetMessage("generic cancel"));
				}
				lnkTabStatus.Text = m_refMsg.GetMessage("tab linkcheck status");
				lnkTabStatus.ToolTip = lnkTabStatus.Text;
				lnkTabTestURL.Text = m_refMsg.GetMessage("tab linkcheck testurl");
				lnkTabTestURL.ToolTip = lnkTabTestURL.Text;
				lblStatus.Text = m_refMsg.GetMessage("lbl linkcheck status");
				lblStatus.ToolTip = lblStatus.Text;
				lblURL.Text = m_refMsg.GetMessage("lbl linkcheck testurl");
				lblURL.ToolTip = lblURL.Text;
				btnHelp.Text = m_refStyle.GetHelpButton("badlinkcheck", "");
			}
		}
		
		
		protected void lnkTabStatus_Click(object sender, EventArgs e)
		{
			if (EkThreads.URLCheckerClass.ThreadRunning)
			{
				Response.Redirect("BadLinkCheck.aspx", false);
			}
			MultiView1.SetActiveView(Status);
			lnkTabStatus.BackColor = System.Drawing.Color.White;
			lnkTabTestURL.BackColor = System.Drawing.Color.FromArgb(0xAD, 0xC5, 0xEF);
		}
		
		protected void lnkTabTestURL_Click(object sender, EventArgs e)
		{
			if (EkThreads.URLCheckerClass.ThreadRunning)
			{
				Response.Redirect("BadLinkCheck.aspx", false);
			}
			MultiView1.SetActiveView(TestURL);
			lnkTabStatus.BackColor = System.Drawing.Color.FromArgb(0xAD, 0xC5, 0xEF);
			lnkTabTestURL.BackColor = System.Drawing.Color.White;
		}
		private void RegisterResources()
		{
			//Register CSS
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
			Ektron.Cms.API.Css.RegisterCss(this, m_RequestInfo.ApplicationPath + "csslib/tabui.css", "EktronTabUICSS");
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			
			//Register JS
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
		}
	}
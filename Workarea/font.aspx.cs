using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;


	public partial class ekfont : System.Web.UI.Page
	{
		
		
		
		protected EkMessageHelper m_refMsg;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected string m_strPageAction = "";
		protected string AppPath = "";
		protected long m_intFontId = 0;
		protected FontData font_data;
		protected ContentAPI m_refContApi = new ContentAPI();
		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			try
			{
				m_refMsg = m_refContApi.EkMsgRef;
				StyleSheetJS.Text = m_refStyle.GetClientScript();
				if (!(Request.QueryString["action"] == null))
				{
					m_strPageAction = Request.QueryString["action"];
					if (m_strPageAction.Length > 0)
					{
						m_strPageAction = m_strPageAction.ToLower();
					}
				}
				if ((m_refContApi.EkContentRef).IsAllowed(0, 0, "users", "IsLoggedIn",0) == false)
				{
					Response.Redirect("login.aspx?fromLnkPg=1", false);
					return;
				}
				if ((m_refContApi.RequestInformationRef.IsMembershipUser) > 0 || m_refContApi.RequestInformationRef.UserId == 0)
				{
					Response.Redirect("reterror.aspx?info=Please login as cms user", false);
					return;
				}
				RegisterResources();
				SetJSServerVariables();
				AppPath = m_refContApi.AppPath;
				TR_AddEditFont.Visible = false;
				TR_ViewFont.Visible = false;
				TR_ViewAllFont.Visible = false;
				if (!(Page.IsPostBack))
				{
					switch (m_strPageAction)
					{
						case "viewfontsbygroup":
							Display_ViewAllFont();
							break;
						case "view":
							Display_ViewFont();
							break;
						case "edit":
							Display_EditFont();
							break;
						case "add":
							Display_AddFont();
							break;
						case "delete":
							Process_DeleteFont();
							break;
					}
				}
				else
				{
					switch (m_strPageAction)
					{
						case "edit":
							Process_EditFont();
							break;
						case "add":
							Process_AddFont();
							break;
						case "delete":
							Process_DeleteFont();
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(EkFunctions.UrlEncode(ex.Message));
			}
		}
		private void Process_EditFont()
		{
			Collection pagedata;
			pagedata = new Collection();
			pagedata.Add(Request.Form["FontID"], "FontID", null, null);
			pagedata.Add(Request.Form["FontFace"], "FontFace", null, null);
			m_refContApi.UpdateFont(pagedata);
			Response.Redirect("font.aspx?action=viewfontsbygroup", false);
		}
		private void Process_AddFont()
		{
			Collection pagedata;
			pagedata = new Collection();
			pagedata.Add(Request.Form["FontFace"], "FontFace", null, null);
			m_refContApi.AddFont(pagedata);
			Response.Redirect("font.aspx?action=viewfontsbygroup", false);
		}
		private void Process_DeleteFont()
		{
			Collection pagedata;
			pagedata = new Collection();
			pagedata.Add(Request.QueryString["FontID"], "FontID", null, null);
			m_refContApi.DeleteFont(pagedata);
			Response.Redirect("font.aspx?action=viewfontsbygroup", false);
		}
		private void Display_EditFont()
		{
			TR_AddEditFont.Visible = true;
			if (!(Request.QueryString["id"] == null))
			{
				m_intFontId = Convert.ToInt64( Request.QueryString["id"]);
			}
			font_data = m_refContApi.GetFontById(m_intFontId);
			FontFace.Value = font_data.Face;
			FontID.Value = Convert.ToString( font_data.Id);
			EditFontToolBar();
		}
		private void Display_AddFont()
		{
			TR_AddEditFont.Visible = true;
			AddFontToolBar();
		}
		private void Display_ViewFont()
		{
			TR_ViewFont.Visible = true;
			if (!(Request.QueryString["id"] == null))
			{
				m_intFontId = Convert.ToInt64( Request.QueryString["id"]);
			}
			font_data = m_refContApi.GetFontById(m_intFontId);
			TD_FontFace.InnerHtml = font_data.Face;
			ViewFontToolBar();
		}
		private void Display_ViewAllFont()
		{
			TR_ViewAllFont.Visible = true;
			FontData[] font_data_list;
			font_data_list = m_refContApi.GetAllFonts();
			if (!(font_data_list == null))
			{
				System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "ID";
				colBound.HeaderText = m_refMsg.GetMessage("generic Fontname");
				ViewFontGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "TITLE";
				colBound.HeaderText = m_refMsg.GetMessage("generic Font Face Sample");
				ViewFontGrid.Columns.Add(colBound);
				DataTable dt = new DataTable();
				DataRow dr;
				int i = 0;
				dt.Columns.Add(new DataColumn("ID", typeof(string)));
				dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
				for (i = 0; i <= font_data_list.Length - 1; i++)
				{
					dr = dt.NewRow();
					dr[0] = "<a href=\"font.aspx?action=View&id=" + font_data_list[i].Id + "\" title=\'" + m_refMsg.GetMessage("click to view font msg") + " \"" + Strings.Replace(font_data_list[i].Face, "\'", "`", 1, -1, 0) + "\"\'>" + font_data_list[i].Face + "</a>";
					dr[1] = "<font face=\"" + font_data_list[i].Face + "\">" + m_refMsg.GetMessage("sample font face style") + "</font>";
					
					dt.Rows.Add(dr);
				}
				ViewFontGrid.BorderColor = System.Drawing.Color.White;
				DataView dv = new DataView(dt);
				ViewFontGrid.DataSource = dv;
				ViewFontGrid.DataBind();
			}
			ViewFontsByGroupToolBar();
		}
		private void AddFontToolBar()
		{
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("add font page title"));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "font.aspx?action=ViewFontsByGroup", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt add button text (font)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm( \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("AddFont", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void ViewFontToolBar()
		{
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("view system font msg") + " \"" + font_data.Face + "\""));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "font.aspx?action=ViewFontsByGroup", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/contentEdit.png", "font.aspx?action=Edit&id=" + m_intFontId + "", m_refMsg.GetMessage("alt edit button text (font)"), m_refMsg.GetMessage("btn edit"), "", StyleHelper.EditButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/delete.png", "font.aspx?action=delete&FontID=" + m_intFontId + "", m_refMsg.GetMessage("alt delete button text (font)"), m_refMsg.GetMessage("btn delete"), "OnClick=\"javascript: return ConfirmFontDelete();\"", StyleHelper.DeleteButtonCssClass));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("viewfonts", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void ViewFontsByGroupToolBar()
		{
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("view system fonts msg"));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "font.aspx?action=Add", m_refMsg.GetMessage("alt add button text (fonts)"), m_refMsg.GetMessage("btn add font"), "", StyleHelper.AddButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("ViewFontsByGroup", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void EditFontToolBar()
		{
			divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string) (m_refMsg.GetMessage("edit font page title") + " \"" + font_data.Face + "\""));
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<table><tr>" + "\r\n");
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/back.png", "font.aspx?action=View&id=" + Request.QueryString["id"] + "", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
			result.Append(m_refStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text (font)"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + m_refStyle.GetHelpButton("EditFont", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
		}
		private void RegisterResources()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
		}
		private void SetJSServerVariables()
		{
			jsFontNameRequiredMsg.Text = m_refMsg.GetMessage("font name required msg");
			jsConfirmDeleteFont.Text = m_refMsg.GetMessage("js: confirm delete font");
		}
	}
	


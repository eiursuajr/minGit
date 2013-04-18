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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.Commerce;
using Ektron.Cms.Workarea;
using Ektron.Cms.Common;


	public partial class Commerce_currency : workareabase
	{
		
		
		#region Member Variables
		
		protected Currency m_refCurrency = null;
		protected int _currentPageNumber = 1;
		protected int TotalPagesNumber = 1;
		protected CurrencyProperty sortCriteria = CurrencyProperty.Name;
		protected const string PAGE_NAME = "currency.aspx";
		protected string searchCriteria = "";
		protected System.Collections.Generic.List<ExchangeRateData> exchangeRateList = new System.Collections.Generic.List<ExchangeRateData>();
		protected CurrencyData defaultCurrency = null;
		protected System.Collections.Generic.List<CurrencyData> activeCurrencies = null;
		protected string AppPath = "";
		
		#endregion
		
		#region Events
        protected void Page_Init(System.Object sender, System.EventArgs e)
        {
            ChangeHeaderText(ViewSubscriptionGrid);
            ChangeHeaderText(dg_xc);
        }
		protected override void Page_Load(System.Object sender, System.EventArgs e)
		{
		    base.Page_Load(sender, e); 
            m_refCurrency = new Currency(m_refContentApi.RequestInformationRef);
            defaultCurrency = m_refCurrency.GetDefaultCurrency();
			if (! Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(m_refContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
			{
				Utilities.ShowError(m_refContentApi.EkMsgRef.GetMessage("feature locked error"));
			}
			RegisterResource();
			AppPath = this.m_refContentApi.ApplicationPath;
			if (Page.Request.QueryString["search"] != "")
			{
				searchCriteria = Page.Request.QueryString["search"];
			}
			try
			{
				if (!string.IsNullOrEmpty(Request.QueryString["currentpage"]))
				{
					_currentPageNumber = Convert.ToInt32(Request.QueryString["currentpage"]);
				}
				if (!string.IsNullOrEmpty(Request.QueryString["sortcriteria"]))
				{
					Util_FindSort(Request.QueryString["sortcriteria"]);
				}
				if (! Utilities.ValidateUserLogin())
				{
					return;
				}
				Util_CheckAccess();
				Util_RegisterResources();
				Util_SetServerJSVariables();
				hdnCurrentPage.Value = CurrentPage.Text;
				tr_addedit.Visible = false;
				tr_viewall.Visible = false;
				if (!(Page.IsPostBack))
				{
					switch (m_sPageAction)
					{
						case "exchangerate":
							Display_ExchangeRate();
							break;
						case "goto":
							Display_GoTo();
							break;
						case "edit":
							Display_Edit();
							break;
						case "add":
							Display_Add();
							break;
						case "delete":
							Process_Delete();
							break;
						default:
							if (Page.IsPostBack == false)
							{
								Display_ViewAll();
							}
							break;
					}
				}
				else
				{
					switch (m_sPageAction)
					{
						case "exchangerate":
							Process_ExchangeRate();
							break;
						case "edit":
							Process_Edit();
							break;
						case "add":
							Process_Add();
							break;
						case "delete":
							Process_Delete();
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Utilities.ShowError(EkFunctions.UrlEncode(ex.Message));
			}
		}
		
		#endregion
		
		#region Process
		
		private void Process_ExchangeRate()
		{
			
			ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
			
			for (int i = 0; i <= (dg_xc.Items.Count - 1); i++)
			{
				
				CheckBox chkUpdate = (CheckBox)dg_xc.Items[i].FindControl("chk_email");
				HiddenField hdnCurrency = (HiddenField)dg_xc.Items[i].FindControl("hdn_currencyId");
				long currentCurrencyId = Convert.ToInt64(hdnCurrency.Value);
				
				if (chkUpdate.Checked && Util_IsActiveExchangeCurrency(currentCurrencyId))
				{
					
					// If Request.Form("chk_email_" & currencyList(i).Id) <> "" Then
					
					if (dg_xc.Items[i].FindControl("txt_exchange") != null)
					{
						
						TextBox txtXCRate = (TextBox)dg_xc.Items[i].FindControl("txt_exchange");
						decimal newRate = decimal.Parse(txtXCRate.Text);
						ExchangeRateData exchangeRateData = new ExchangeRateData(exchangeRateApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId, Convert.ToInt32(currentCurrencyId), newRate, DateTime.Now);
						
						exchangeRateApi.Add(exchangeRateData);
						
					}
					
				}
				
			}
			
			ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();";
			
		}
		
		private void Process_Edit()
		{
			CurrencyData currency = null;
			currency = m_refCurrency.GetItem(Convert.ToInt32(m_iID));
			
			ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
			ExchangeRateData exchangeRateData = new ExchangeRateData(exchangeRateApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId, currency.Id, Convert.ToDecimal(txt_exchangerate.Text), DateTime.Now);
			
			exchangeRateApi.Add(exchangeRateData);
			
			currency.Name = (string) txt_name.Text;
			currency.Id = System.Convert.ToInt32(txt_numericisocode.Text);
			currency.AlphaIsoCode = (string) txt_alphaisocode.Text;
			currency.Enabled = System.Convert.ToBoolean(chk_enabled.Checked);
			
			m_refCurrency.Update(currency);
			ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();";
		}
		private void Process_Add()
		{
			CurrencyData currency = new CurrencyData();
			ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
			
			currency.Name = (string) txt_name.Text;
			currency.AlphaIsoCode = (string) txt_alphaisocode.Text;
			currency.Enabled = System.Convert.ToBoolean(chk_enabled.Checked);
			currency.Id = System.Convert.ToInt32(txt_numericisocode.Text);
			currency.CultureCode = (string) txt_alphaisocode.Text;
			
			m_refCurrency.Add(currency);
			
			ExchangeRateData exchangeRateData = new ExchangeRateData(exchangeRateApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId, Convert.ToInt32(txt_numericisocode.Text), Convert.ToDecimal(txt_exchangerate.Text), DateTime.Now);
			exchangeRateApi.Add(exchangeRateData);
			
			ltr_js.Text = "self.parent.location.reload(); self.parent.ektb_remove();";
		}
		private void Process_Delete()
		{
			string[] idList = Strings.Split(Request.QueryString["Ids"], ",", -1, 0);
			if (idList.Length > 0)
			{
				for (int i = 0; i <= (idList.Length - 1); i++)
				{
					if (Information.IsNumeric(idList[i]))
					{
						m_refCurrency.Delete(int.Parse(idList[i]));
					}
				}
				Response.Redirect(System.Convert.ToString(PAGE_NAME + "?action=viewall"), false);
			}
			else
			{
				throw (new Exception(GetMessage("lbl err no currencies selected")));
			}
		}
		
		#endregion
		
		#region Display
		
		private void Display_ExchangeRate()
		{
			
			Ektron.Cms.Common.Criteria<CurrencyProperty> criteria = new Ektron.Cms.Common.Criteria<CurrencyProperty>(sortCriteria, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
			System.Collections.Generic.List<CurrencyData> currencyList;
			
			criteria.PagingInfo = new PagingInfo(1000);
			criteria.PagingInfo.CurrentPage = _currentPageNumber;
			criteria.AddFilter(CurrencyProperty.Enabled, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, true);
			criteria.AddFilter(CurrencyProperty.Id, Ektron.Cms.Common.CriteriaFilterOperator.NotEqualTo, m_refCurrency.RequestInformation.CommerceSettings.DefaultCurrencyId);
			
			currencyList = m_refCurrency.GetList(criteria);
			
			ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
			Ektron.Cms.Common.Criteria<Ektron.Cms.Commerce.ExchangeRateProperty> exchangeRateCriteria = new Ektron.Cms.Common.Criteria<Ektron.Cms.Commerce.ExchangeRateProperty>();
			System.Collections.Generic.List<long> currencyIDList = new System.Collections.Generic.List<long>();
			for (int i = 0; i <= (currencyList.Count - 1); i++)
			{
				currencyIDList.Add(currencyList[i].Id);
			}
			if (currencyIDList.Count > 0)
            {
                exchangeRateCriteria.AddFilter(ExchangeRateProperty.BaseCurrencyId, Ektron.Cms.Common.CriteriaFilterOperator.EqualTo, m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId);
				exchangeRateCriteria.AddFilter(ExchangeRateProperty.ExchangeCurrencyId, Ektron.Cms.Common.CriteriaFilterOperator.In, currencyIDList.ToArray());
				exchangeRateList = exchangeRateApi.GetCurrentList(exchangeRateCriteria);
			
				dg_xc.DataSource = currencyList;
				dg_xc.DataBind();
			}
            else
            {
                ltr_ExchangeRateMsg.Text = GetMessage("ecomm no enabled currencies");
            }
			Util_SetJs();
			paginglinks.Visible = false;
			Util_SetLabels();
			
		}
		
		private void Display_Edit()
		{
			CurrencyData currency = new CurrencyData();
			
			currency = m_refCurrency.GetItem(Convert.ToInt32(m_iID));
			ltr_ISOAlpha.Text = "&nbsp;" + currency.AlphaIsoCode;
			txt_numericisocode.Enabled = false;
			txt_alphaisocode.Enabled = false;
			Util_PopulateData(currency);
			Util_SetLabels();
		}
		
		private void Display_GoTo()
		{
			Util_SetLabels();
		}
		
		private void Display_Add()
		{
			Util_SetLabels();
		}
		
		private void Display_View()
		{
			CurrencyData currency = new CurrencyData();
			
			currency = m_refCurrency.GetItem(Convert.ToInt32(m_iID));
			ltr_ISOAlpha.Text = "&nbsp;<b>" + currency.AlphaIsoCode + "</b>";
			
			Util_PopulateData(currency);
			Util_SetLabels();
		}
		private void Display_ViewAll()
		{
			Ektron.Cms.Common.Criteria<CurrencyProperty> criteria = new Ektron.Cms.Common.Criteria<CurrencyProperty>(sortCriteria, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Ascending);
			if (sortCriteria == CurrencyProperty.Enabled)
			{
				criteria = new Ektron.Cms.Common.Criteria<CurrencyProperty>(sortCriteria, Ektron.Cms.Common.EkEnumeration.OrderByDirection.Descending);
			}
			System.Collections.Generic.List<CurrencyData> currencyList;
			
			criteria.PagingInfo.RecordsPerPage = m_refContentApi.RequestInformationRef.PagingSize;
			criteria.PagingInfo.CurrentPage = _currentPageNumber;
			
			if (!string.IsNullOrEmpty(searchCriteria))
			{
				criteria.AddFilter(CurrencyProperty.Name, Ektron.Cms.Common.CriteriaFilterOperator.Contains, searchCriteria);
			}
            if (criteria != null)
            {
                currencyList = m_refCurrency.GetList(criteria);
                ViewSubscriptionGrid.Columns[ViewSubscriptionGrid.Columns.Count - 1].Visible = false;
                ViewSubscriptionGrid.DataSource = currencyList;
                ViewSubscriptionGrid.DataBind();
                TotalPagesNumber = System.Convert.ToInt32(criteria.PagingInfo.TotalPages);
                Util_SetJs();
                Util_SetPaging();
                Util_SetLabels();
            }
		}
		
		#endregion
		
		#region Util
		
		private void Util_SetLabels()
		{
			ltr_name.Text = GetMessage("generic name");
			ltr_numericisocode.Text = GetMessage("lbl numeric iso code");
			ltr_alphaisocode.Text = GetMessage("lbl alpha iso code");
			ltr_enabled.Text = GetMessage("enabled");
			ltr_exchangerate.Text = GetMessage("lbl exchange rate");
			
			lnk_gotopage.Text = "[" + GetMessage("lbl go to page") + "]";
			
			lnk_first.Text = "[" + GetMessage("lbl first page") + "]";
			lnk_previous.Text = "[" + GetMessage("lbl previous page") + "]";
			lnk_next.Text = "[" + GetMessage("lbl next page") + "]";
			lnk_last.Text = "[" + GetMessage("lbl last page") + "]";
			
			ltr_defaultcurrency.Text = m_refContentApi.RequestInformationRef.CommerceSettings.ISOCurrencySymbol;
			
			switch (m_sPageAction)
			{
				case "exchangerate":
					
					tr_viewall.Visible = false;
					tr_exchangerate.Visible = true;
					
					SetTitleBarToMessage("lbl edit exchange rates");
					
					AddButtonwithMessages(AppPath + "images/UI/Icons/back.png", System.Convert.ToString(PAGE_NAME + "?action=ViewAll"), "alt back button text", "btn back", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.BackButtonCssClass, true);
					workareamenu actionMenu_1 = new workareamenu("action", GetMessage("lbl action"), AppPath + "images/UI/Icons/check.png");
					actionMenu_1.AddItem(AppPath + "images/ui/icons/save.png", GetMessage("btn update"), "SubmitForm(true);");
					AddMenu(actionMenu_1);
					
					AddHelpButton("EditExchangeRates");
					break;
				case "goto":
					break;
					// tr_goto.Visible = True
				case "add":
					tr_addedit.Visible = true;
					
					SetTitleBarToMessage("lbl add currency");
					AddButtonwithMessages(AppPath + "images/UI/Icons/back.png", System.Convert.ToString(PAGE_NAME + "?action=viewall"), "alt back button text", "btn back", " onclick=\"self.parent.ektb_remove();\" ", StyleHelper.BackButtonCssClass, true);
					AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", "#", "lbl Add Email From Address", "btn save", "onclick=\"return SubmitForm( \'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true);
					AddHelpButton(m_sPageAction + "currency");
					break;
				case "edit":
					tr_addedit.Visible = true;
					
					SetTitleBarToMessage("lbl edit currency");

					AddButtonwithMessages(AppPath + "images/UI/Icons/back.png", "#", "alt back button text", "btn back", " onclick=\"javascript:self.parent.ektb_remove();\" ", StyleHelper.BackButtonCssClass, true);
					AddButtonwithMessages(AppPath + "images/UI/Icons/save.png", "#", "lbl update email address", "btn update", "Onclick=\"javascript:return SubmitForm(\'VerifyForm()\');\"", StyleHelper.SaveButtonCssClass, true);
					AddHelpButton(m_sPageAction + "currency");
					break;
				default:
					tr_viewall.Visible = true;
					
					
					SetTitleBarToMessage("lbl currencies");
					
					//Dim newMenu As New workareamenu("file", GetMessage("lbl new"), apppath & "images/UI/Icons/star.png")
					//newMenu.AddItem(AppImgPath & "commerce/currency.gif", GetMessage("lbl currency"), "ektb_show('','" & PAGE_NAME & "?action=Add&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);")
					//AddMenu(newMenu)
					
					workareamenu actionMenu = new workareamenu("action", GetMessage("lbl action"), AppPath + "images/UI/Icons/check.png");
					actionMenu.AddItem(AppPath + "images/ui/icons/pencil.png", GetMessage("lbl edit exchange rates"), "ektb_show(\'\',\'" + PAGE_NAME + "?action=ExchangeRate&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true\', null);");
					actionMenu.AddBreak();
					actionMenu.AddItem(AppPath + "images/ui/icons/delete.png", GetMessage("lbl del sel"), "ConfirmDelete();");
					AddMenu(actionMenu);
					this.AddSearchBox(EkFunctions.HtmlEncode(searchCriteria), new ListItemCollection(), "searchCurrency");
					AddHelpButton(m_sPageAction + "currency");
					break;
			}
		}
        private void ChangeHeaderText(DataGrid dg)
        {
            if (dg == null)
            {
                return;
            }

            foreach (DataGridColumn col in dg.Columns)
            {
                if (col.HeaderText == "<a href='currency.aspx?sortcriteria=Id'>Id</a>")
                {
                    col.HeaderText = "<a href='currency.aspx?sortcriteria=Id'>" + this.GetMessage("generic id") + "</a>";
                }
                if (col.HeaderText == "<a href='currency.aspx?sortcriteria=Name'>Name</a>")
                {
                    col.HeaderText = "<a href='currency.aspx?sortcriteria=Name'>" + this.GetMessage("generic name") + "</a>";
                }
                if (col.HeaderText == "<a href='currency.aspx?sortcriteria=AlphaIsoCode'>AlphaIsoCode</a>")
                {
                    col.HeaderText = "<a href='currency.aspx?sortcriteria=AlphaIsoCode'>" + this.GetMessage("lbl AlphaIsoCode") + "</a>";
                }
                if (col.HeaderText == "<a href='currency.aspx?sortcriteria=Enabled'>Enabled</a>")
                {
                    col.HeaderText = "<a href='currency.aspx?sortcriteria=Enabled'>" + this.GetMessage("enabled") + "</a>";
                }
                if (col.HeaderText == "Id")
                {
                    col.HeaderText = this.GetMessage("generic id");
                }
                if (col.HeaderText == "Name")
                {
                    col.HeaderText = this.GetMessage("generic name");
                }
                if (col.HeaderText == "AlphaIsoCode")
                {
                    col.HeaderText = this.GetMessage("lbl AlphaIsoCode");
                }
                if (col.HeaderText == "Exchange Rate")
                {
                    col.HeaderText = this.GetMessage("lbl exchange rate");
                }
            }
        }
		protected void Util_CheckAccess()
		{
			if (! m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
			{
				throw (new Exception("err not role commerce-admin"));
			}
		}
		protected void Util_NavigationLink_Click(object sender, CommandEventArgs e)
		{
			if (hdnCurrentPage.Value != "")
			{
				_currentPageNumber = int.Parse((string) hdnCurrentPage.Value);
			}
			switch (e.CommandName)
			{
				case "First":
					_currentPageNumber = 1;
					break;
				case "Last":
					_currentPageNumber = int.Parse((string) TotalPages.Text);
					break;
				case "Next":
					_currentPageNumber = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) + 1);
					break;
				case "Prev":
					_currentPageNumber = System.Convert.ToInt32(int.Parse((string) CurrentPage.Text) - 1);
					break;
			}
			Display_ViewAll();
			isPostData.Value = "true";
		}
		protected void Util_FindSort(string sortstring)
		{
			switch (sortstring.ToLower())
			{
				case "alphaisocode":
					sortCriteria = CurrencyProperty.AlphaIsoCode;
					break;
				case "id":
					sortCriteria = CurrencyProperty.Id;
					break;
				case "enabled":
					sortCriteria = CurrencyProperty.Enabled;
					break;
				default:
					sortCriteria = CurrencyProperty.Name;
					break;
			}
		}
		protected void Util_PopulateData(CurrencyData currency)
		{
			ExchangeRateData exchangeRateData = new ExchangeRateData();
			decimal rate = (decimal) 0.0;
			ExchangeRateApi exchangeRateApi = new ExchangeRateApi();
			exchangeRateData = exchangeRateApi.GetCurrentExchangeRate(currency.Id);
			
			if (exchangeRateData != null)
			{
				rate = exchangeRateData.Rate;
			}
			txt_name.Text = currency.Name;
			txt_numericisocode.Text = currency.Id.ToString();
			txt_alphaisocode.Text = currency.AlphaIsoCode;
			chk_enabled.Checked = currency.Enabled;
			
			txt_exchangerate.Text = rate.ToString();
			//txt_exchangerate.Text = txt_exchangerate.Text.Substring(0, txt_exchangerate.Text.LastIndexOf(".") + 3)
		}
		protected void Util_SetPaging()
		{
			if (TotalPagesNumber <= 1)
			{
				paginglinks.Visible = false;
			}
			else
			{
				paginglinks.Visible = true;
				
				TotalPages.Text = (System.Math.Ceiling(Convert.ToDouble(TotalPagesNumber))).ToString();
				TotalPages.ToolTip = TotalPages.Text;
				CurrentPage.Text = _currentPageNumber.ToString();
				CurrentPage.ToolTip = CurrentPage.Text;
				
				lnk_gotopage.NavigateUrl = "javascript:GoToPage(document.getElementById(\'CurrentPage\').value, " + TotalPagesNumber + ");";
				lnk_previous.NavigateUrl = Util_GetPageURL(_currentPageNumber - 1);
				lnk_first.NavigateUrl = Util_GetPageURL(1);
				lnk_next.NavigateUrl = Util_GetPageURL(_currentPageNumber + 1);
				lnk_last.NavigateUrl = Util_GetPageURL(TotalPagesNumber);
				
				if (_currentPageNumber == 1)
				{
					lnk_previous.Enabled = false;
					lnk_first.Enabled = false;
				}
				else if (_currentPageNumber == TotalPagesNumber)
				{
					lnk_next.Enabled = false;
					lnk_last.Enabled = false;
				}
			}
		}
		protected string Util_GetPageURL(int pageid)
		{
			return PAGE_NAME + "?currentpage=" + (pageid.ToString() == "-1" ? "\' + pageid + \'" : pageid.ToString()) + (sortCriteria != CurrencyProperty.Name ? ("&sortcriteria=" + System.Enum.GetName(typeof(CurrencyProperty), sortCriteria)) : "");
		}
		protected void Util_SetJs()
		{
			StringBuilder sbJS = new StringBuilder();
			
			sbJS.Append(" function GoToPage(pageid, pagetotal) { ").Append(Environment.NewLine);
			sbJS.Append("     if (pageid <= pagetotal && pageid >= 1) { ").Append(Environment.NewLine);
			sbJS.Append("         window.location.href = \'").Append(Util_GetPageURL(-1)).Append("\'; ").Append(Environment.NewLine);
			sbJS.Append("     } else { ").Append(Environment.NewLine);
			sbJS.Append("         alert(\'").Append(string.Format(GetMessage("js: err page must be between"), TotalPagesNumber)).Append("\'); ").Append(Environment.NewLine);
			sbJS.Append("     } ").Append(Environment.NewLine);
			sbJS.Append(" } ").Append(Environment.NewLine);
			
			ltr_js.Text = sbJS.ToString();
		}
		
		protected decimal Util_GetExchangeRate(long currencyId)
		{
			
			decimal xcRate = 0;
			
			foreach (ExchangeRateData xChangeRate in exchangeRateList)
			{
				if (xChangeRate.ExchangeCurrencyId == currencyId)
				{
					xcRate = xChangeRate.Rate;
					break;
				}
			}
			
			return xcRate;
			
		}
		
		private void Util_RegisterResources()
		{
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
			Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronThickBoxJS);
			
			Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "csslib/box.css", "EktronBoxCSS");
		}
		private void Util_SetServerJSVariables()
		{
			ltr_nameReq.Text = GetMessage("js: alert name required");
			ltr_nameCantHave.Text = GetMessage("js: alert currency name cant include");
			ltr_rateNotNumeric.Text = GetMessage("js: alert exchange rate not numeric");
			ltr_rateGrtZero.Text = GetMessage("js: alert to enable exchange rate must be greater than zero");
			ltr_notInteger.Text = GetMessage("js: alert numeric isocode not integer");
			ltr_delSelCur.Text = GetMessage("js: confirm delete selected currency");
			ltr_errNoCurSel.Text = GetMessage("lbl err no currencies selected");
		}
		
		protected bool Util_IsActiveExchangeCurrency(long currencyId)
		{
			
			if (activeCurrencies == null)
			{
				activeCurrencies = m_refCurrency.GetActiveCurrencyList();
			}
			
			for (int i = 0; i <= (activeCurrencies.Count - 1); i++)
			{
				
				if (activeCurrencies[i].Id == currencyId && !(currencyId == this.m_refContentApi.RequestInformationRef.CommerceSettings.DefaultCurrencyId))
				{
					return true;
				}
				
			}
			
			return false;
			
		}
		
		#endregion
		
		#region JS/CSS
		
		protected void RegisterResource()
		{
			Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
		
		#endregion
		
	}
	


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
using Ektron.Cms.Community;
using Ektron.Cms.Commerce;

namespace Ektron.Cms.Workarea
{
    public class workareabase : System.Web.UI.Page
    {
        public workareabase()
        {
            m_refMsg = m_refContentApi.EkMsgRef;
            m_wamTabs = new workareatabs(m_refMsg, AppImgPath);
            m_waCommerce = new workareaCommerce(m_refContentApi.RequestInformationRef.CommerceSettings, this);
            m_waaAjax = new workareaajax(m_refContentApi.AppPath);

        }

        protected ContentAPI m_refContentApi = new ContentAPI();
        protected CommunityGroupAPI m_refCommunityGroupApi = new Ektron.Cms.Community.CommunityGroupAPI();
        protected TagsAPI m_refTagsApi = new Ektron.Cms.Community.TagsAPI();
        protected FriendsAPI m_refFriendsApi = new Ektron.Cms.Community.FriendsAPI();
        protected FavoritesAPI m_refFavoritesApi = new Ektron.Cms.Community.FavoritesAPI();
        protected StyleHelper m_refStyle = new StyleHelper();
        protected EkMessageHelper m_refMsg;
        private string TitleBar = string.Empty;
        private string Toolbar = string.Empty;
        protected int ContentLanguage = -1;
        private workareatabs m_wamTabs;
        private workareajavascript m_wajs = new workareajavascript();
        private workareaCommerce m_waCommerce;
        private workareaajax m_waaAjax;
        private workareawizard m_wawWizard = new workareawizard();
        protected string m_sPageAction = "";
        protected long m_iID = 0;
        protected bool m_bAjaxTree = true;
        protected bool m_bIsWindows = true;
        private ArrayList aButtons = new ArrayList();
        private string noticeContainer = "";
        protected TableRow m_cTR = null;
        protected string m_checkVariable = "";

        private bool _Version8TabsImplemented = false;

        #region "Page Events Init,Load,PreRender"
        protected virtual void Page_Load(object sender, System.EventArgs e)
        {
            if (m_refContentApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect(m_refContentApi.ApplicationPath + "blank.htm");
            }
            if (Request.ServerVariables["HTTP_USER_AGENT"].ToLower().IndexOf("windows") == -1)
            {
                m_bIsWindows = false;
            }
            if (m_refContentApi.TreeModel == 1)
            {
                m_bAjaxTree = true;
            }
            if (!(Request.QueryString["LangType"] == null))
            {
                if (Request.QueryString["LangType"] != "")
                {
                    ContentLanguage = GetQueryLanguage();
                    m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
                }
                else
                {
                    if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                    {
                        ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                    }
                }
            }
            else
            {
                if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refContentApi.GetCookieValue("LastValidLanguageID"));
                }
            }

            if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
            {
                m_refContentApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
            }
            else
            {
                m_refContentApi.ContentLanguage = ContentLanguage;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            {
                this.m_sPageAction = (string)(Request.QueryString["action"].ToLower());
            }
            else
            {
                this.m_sPageAction = "";
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                long.TryParse(Request.QueryString["id"], out m_iID);
            }
        }
        protected virtual void Page_PreRender(object sender, System.EventArgs e)
        {
            //cache control
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;

            //register js and css
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "wamenu/includes/com.ektron.ui.menu.js", "EktronComUIMenuJS");
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/workareahelper.js", "EktronWorkareaHelperJS");

            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "base js", (string)((new StyleHelper()).GetClientScript()));

            Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "wamenu/css/com.ektron.ui.menu.css", "EktronComUIMenuCSS");
        }
        #endregion
        public bool Version8TabsImplemented
        {
            get
            {
                return _Version8TabsImplemented;
            }
            set
            {
                _Version8TabsImplemented = value;
            }
        }
        public string GetMessage(string MessageTitle)
        {
            return m_refMsg.GetMessage(MessageTitle);
        }

        public EkRequestInformation RequestInformationRef
        {
            get
            {
                return m_refContentApi.RequestInformationRef;
            }
        }

        public ContentAPI ContentAPIRef
        {
            get
            {
                return m_refContentApi;
            }
        }

        public string AppImgPath
        {
            get
            {
                return m_refContentApi.RequestInformationRef.AppImgPath;
            }
        }

        public string MenuCheckVariable
        {
            get
            {
                return m_checkVariable;
            }
            set
            {
                m_checkVariable = value;
            }
        }

        public void SetTitleBarToMessage(string MessageTitle)
        {
            TitleBar = m_refStyle.GetTitleBar(m_refMsg.GetMessage(MessageTitle));
        }

        public void SetTitleBarToString(string MessageTitle)
        {
            TitleBar = m_refStyle.GetTitleBar(MessageTitle);
        }

        public string FormatCurrency(decimal Price, string CurrencyCharacter)
        {
            if (CurrencyCharacter == "")
            {
                return Strings.FormatNumber(Price, 2, Microsoft.VisualBasic.TriState.UseDefault, Microsoft.VisualBasic.TriState.UseDefault, Microsoft.VisualBasic.TriState.UseDefault);
            }
            else
            {
                return Ektron.Cms.Common.EkFunctions.FormatCurrency(Price, this.m_refContentApi.RequestInformationRef.CommerceSettings.CurrencyCultureCode);
            }
        }

		// AddButton
        public void AddButton(string ImageFile, string hrefPath, string altText, string HeaderText, string SpecialEvents)
        {
			AddButton(ImageFile, hrefPath, altText, HeaderText, SpecialEvents, String.Empty, false);
        }

		public void AddButton(string ImageFile, string hrefPath, string altText, string HeaderText, string SpecialEvents, string aTagClassName)
		{
			AddButton(ImageFile, hrefPath, altText, HeaderText, SpecialEvents, aTagClassName, false);
		}

		public void AddButton(string ImageFile, string hrefPath, string altText, string HeaderText, string SpecialEvents, string aTagClassName, bool isPrimary)
		{
			aButtons.Add(m_refStyle.GetButtonEventsWCaption(ImageFile, hrefPath, altText, HeaderText, SpecialEvents, aTagClassName, isPrimary));
		}

		// AddButtonwithMessages A
        public void AddButtonwithMessages(string ImageFile, string hrefPath, string altTextMessage, string HeaderTextMessage, string SpecialEvents)
        {
			AddButtonwithMessages(ImageFile, hrefPath, altTextMessage, HeaderTextMessage, SpecialEvents, String.Empty, false);
        }

		public void AddButtonwithMessages(string ImageFile, string hrefPath, string altTextMessage, string HeaderTextMessage, string SpecialEvents, string aTagClassName)
		{
			AddButtonwithMessages(ImageFile, hrefPath, altTextMessage, HeaderTextMessage, SpecialEvents, aTagClassName, false);
		}

		public void AddButtonwithMessages(string ImageFile, string hrefPath, string altTextMessage, string HeaderTextMessage, string SpecialEvents, string aTagClassName, bool isPrimary)
		{
			aButtons.Add(m_refStyle.GetButtonEventsWCaption(ImageFile, hrefPath, GetMessage(altTextMessage), GetMessage(HeaderTextMessage), SpecialEvents, aTagClassName, isPrimary));
		}

		// AddButtonwithMessages B
        public void AddButtonwithMessages(string ImageFile, string hrefPath, string altTextMessage, string HeaderTextMessage, string SpecialEvents, int InsertAt)
        {
			AddButtonwithMessages(ImageFile, hrefPath, altTextMessage, HeaderTextMessage, SpecialEvents, InsertAt, String.Empty, false);
        }

		public void AddButtonwithMessages(string ImageFile, string hrefPath, string altTextMessage, string HeaderTextMessage, string SpecialEvents, int InsertAt, string aTagClassName)
		{
			AddButtonwithMessages(ImageFile, hrefPath, altTextMessage, HeaderTextMessage, SpecialEvents, InsertAt, aTagClassName, false);
		}

		public void AddButtonwithMessages(string ImageFile, string hrefPath, string altTextMessage, string HeaderTextMessage, string SpecialEvents, int InsertAt, string aTagClassName, bool isPrimary)
		{
			aButtons.Insert(InsertAt, m_refStyle.GetButtonEventsWCaption(ImageFile, hrefPath, GetMessage(altTextMessage), GetMessage(HeaderTextMessage), SpecialEvents, aTagClassName, isPrimary));
		}


        public void AddBreak()
        {
            aButtons.Add("<td>&nbsp;|&nbsp;</td>");
        }
        public void AddMenuButtonLink(string ImageFile, string TextMessage, string hrefPath)
        {
            AddMenuButton(ImageFile, TextMessage, "window.location.href=\'" + hrefPath + "\'");
        }
        public void AddMenuButton(string ImageFile, string TextMessage, string SpecialEvents)
        {
            aButtons.Add("<td class=\"menuRootItem\" onclick=\"" + SpecialEvents + "\" onmouseover=\"this.className=\'menuRootItemSelected\';\" onmouseout=\"this.className=\'menuRootItem\';\"><span style=\"background-image: url(\'" + ImageFile + "\')\">" + TextMessage + "</span></td>");
        }

        public void AddHelpButton(string MessageTitle)
        {
            AddHelpButton(MessageTitle, true);
        }

		public void AddHelpButton(string MessageTitle, bool useDivider)
		{
			if (useDivider)
			{
				aButtons.Add(StyleHelper.ActionBarDivider + "<td>" + m_refStyle.GetHelpButton(MessageTitle, "") + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");

				return;
			}

			aButtons.Add("<td>" + m_refStyle.GetHelpButton(MessageTitle, "") + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
		}

		public void AddSearchBox(string SearchText, ListItemCollection DropDownItems, string SearchJSFunction)
		{
			AddSearchBox(SearchText, DropDownItems, SearchJSFunction, true);
		}

        public void AddSearchBox(string SearchText, ListItemCollection DropDownItems, string SearchJSFunction, bool useDivider)
        {
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
            Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronInputLabelJS);
            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "java/ektron.workarea.searchBox.inputLabelInit.js", "EktronWorkareaSearchBoxInputLabelInitJS");
            StringBuilder sbSearch = new StringBuilder();

			if (useDivider)
			{
				sbSearch.Append(StyleHelper.ActionBarDivider);
			}
            
            sbSearch.Append("<td class=\"label\">");
            sbSearch.Append("<label for=\"txtSearch\">" + GetMessage("generic search") + "</label>");
            sbSearch.Append("</td>");
            sbSearch.Append("<td>");
            sbSearch.Append("<input type=\"text\" size=\"25\" class=\"ektronTextMedium\" id=\"txtSearch\" name=\"txtSearch\" value=\"" + EkFunctions.HtmlEncode(SearchText) + "\" onkeydown=\"CheckForReturn(event)\" autocomplete=\"off\">");
            if (DropDownItems.Count > 0)
            {
                sbSearch.Append("<select id=\"searchlist\" name=\"searchlist\">");
                for (int i = 0; i <= (DropDownItems.Count - 1); i++)
                {
                    sbSearch.Append("<option value=\"").Append(EkFunctions.HtmlEncode((string)(DropDownItems[i].Value))).Append("\"").Append((DropDownItems[i].Selected) ? " selected " : "").Append(">").Append(Server.HtmlEncode((string)(DropDownItems[i].Text))).Append("</option>");
                }
                sbSearch.Append("</select>");
            }
            sbSearch.Append("</td>");
            sbSearch.Append("<td>");
            sbSearch.Append("<input type=button value=\"" + GetMessage("generic search") + "\" title=\"" + GetMessage("generic search") + "\" id=\"btnSearch\" name=\"btnSearch\" class=\"ektronWorkareaSearch\" onclick=\"").Append(SearchJSFunction).Append("();\" />");

            sbSearch.Append("    <script type=\"text/javascript\">").Append(Environment.NewLine);
            sbSearch.Append(" function CheckForReturn(e) ").Append(Environment.NewLine);
            sbSearch.Append(" {  ").Append(Environment.NewLine);
            sbSearch.Append("   var keynum; ").Append(Environment.NewLine);
            sbSearch.Append("   var keychar; ").Append(Environment.NewLine);
            sbSearch.Append("   if(window.event) // IE ").Append(Environment.NewLine);
            sbSearch.Append("   { ").Append(Environment.NewLine);
            sbSearch.Append("       keynum = e.keyCode ").Append(Environment.NewLine);
            sbSearch.Append("   } ").Append(Environment.NewLine);
            sbSearch.Append("   else if(e.which) // Netscape/Firefox/Opera ").Append(Environment.NewLine);
            sbSearch.Append("   { ").Append(Environment.NewLine);
            sbSearch.Append("       keynum = e.which ").Append(Environment.NewLine);
            sbSearch.Append("   } ").Append(Environment.NewLine);
            sbSearch.Append("   if( keynum == 13 ) { ").Append(Environment.NewLine);
            sbSearch.Append("       document.getElementById(\'btnSearch\').focus(); ").Append(Environment.NewLine);
            sbSearch.Append("   } ").Append(Environment.NewLine);
            sbSearch.Append(" } ").Append(Environment.NewLine);
            sbSearch.Append("</script>");
            sbSearch.Append("</td>");

            aButtons.Add(sbSearch.ToString());
        }

        public string eWebWPEditor(string FieldName, string Width, string Height, string ContentHtml, string onsubmit)
        {
            StringBuilder sbWP = new StringBuilder();
            sbWP.Append("<script type=\"text/javascript\">" + Environment.NewLine);
            sbWP.Append("var eWebWPPath = \"" + m_refContentApi.AppPath + "ewebwp/\";" + Environment.NewLine);
            sbWP.Append("</script>" + Environment.NewLine);
            sbWP.Append("<script type=\"text/javascript\" src=\"" + m_refContentApi.AppPath + "ewebwp/ewebwp.js\"></script>" + Environment.NewLine);
            sbWP.Append("<input type=\"hidden\" name=\"" + FieldName + "\" value=\"" + EkFunctions.HtmlEncode(ContentHtml) + "\">");
            sbWP.Append("<script language=\"javaScript1.2\">" + Environment.NewLine);
            sbWP.Append("<!--" + Environment.NewLine);
            if (Information.TypeName(Width) == "String")
            {
                Width = "\"" + Width + "\"";
            }
            if (Information.TypeName(Height) == "String")
            {
                Height = "\"" + Height + "\"";
            }
            if (onsubmit.Trim().Length > 0)
            {
                sbWP.Append("function " + FieldName + "_onsubmit() { " + Environment.NewLine);
                sbWP.Append(onsubmit).Append(Environment.NewLine);
                sbWP.Append("}" + Environment.NewLine);
            }
            else
            {
                sbWP.Append("function " + FieldName + "_onsubmit(){ return true; }" + Environment.NewLine);
            }
            sbWP.Append("eWebWP.parameters.removeFont = true;" + Environment.NewLine);
            sbWP.Append("eWebWP.create(\"" + FieldName + "\", " + Width + ", " + Height + ");" + Environment.NewLine);
            sbWP.Append("//-->" + Environment.NewLine);
            sbWP.Append("</script>");
            return sbWP.ToString();
        }

        public void AddBackButton(string ReturnPath)
        {
            aButtons.Add(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", ReturnPath, m_refMsg.GetMessage("alt back button"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
        }

        public void AddButtonText(string ButtonText)
        {
            aButtons.Add(ButtonText);
        }

        public void AddMenu(workareamenu waMenu)
        {
            aButtons.Add(waMenu.Render(this.m_checkVariable));
        }

        public void AddNotice(string noticeText)
        {

            noticeContainer = noticeText;

        }

        public workareatabs Tabs
        {
            get
            {
                return this.m_wamTabs;
            }
            set
            {
                this.m_wamTabs = value;
            }
        }

        public workareajavascript JSLibrary
        {
            get
            {
                return this.m_wajs;
            }
            set
            {
                this.m_wajs = value;
            }
        }

        public workareaCommerce CommerceLibrary
        {
            get
            {
                return this.m_waCommerce;
            }
            set
            {
                this.m_waCommerce = value;
            }
        }

        public workareaajax AJAX
        {
            get
            {
                return this.m_waaAjax;
            }
            set
            {
                this.m_waaAjax = value;
            }
        }

        public workareawizard Wizard
        {
            get
            {
                return this.m_wawWizard;
            }
            set
            {
                this.m_wawWizard = value;
            }
        }

        public void AddLanguageDropdown(bool ShowAll)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            if (m_refContentApi.EnableMultilingual == 1)
            {
                SiteAPI m_refsite = new SiteAPI();
                LanguageData[] language_data = m_refsite.GetAllActiveLanguages();

                result.Append(StyleHelper.ActionBarDivider);
                result.Append("<td class=\"label\">View:</td>");
                result.Append("<td>");
                result.Append("<select id=selLang name=selLang OnChange=\"javascript:LoadLanguage(\'frmContent\');\">");
                if (ShowAll)
                {
                    result.Append("<option value=\"" + Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES + "\"");
                    if (ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES)
                    {
                        result.Append(" selected=\"selected\"");
                    }
                    result.Append(">");
                    result.Append("All");
                    result.Append("</option>");
                }
                for (int count = 0; count <= language_data.Length - 1; count++)
                {
                    LanguageData with_1 = language_data[count];
                    result.Append("<option value=\"" + with_1.Id + "\"");
                    if (with_1.Id == ContentLanguage)
                    {
                        result.Append(" selected=\"selected\"");
                    }
                    else
                    {
                    }
                    result.Append(">");
                    result.Append(with_1.LocalName);
                    result.Append("</option>");
                }
                result.Append("</select></td>");
            }
            result.Append("<td>");
            aButtons.Add(result.ToString());
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            Table tHeadertable = new Table();
            TableRow tHeaderrow = new TableRow();
            TableCell tHeadercell = new TableCell();
            Table tOutertable = new Table();
            TableRow tOuterrow = new TableRow();
            TableCell tOutercell = new TableCell();
            //    'Get a reference to the page
            System.Web.UI.HtmlControls.HtmlForm form = Page.Form;
            //    'inner
            tHeadertable.CellPadding = 0;
            tHeadertable.CellSpacing = 0;
            tHeadertable.Width = Unit.Percentage(100);


            tHeadercell.CssClass = "ektronTitlebar";
            tHeadercell.ID = "txtTitleBar";
            tHeadercell.Text = TitleBar;
            tHeaderrow.Controls.Add(tHeadercell);
            tHeadertable.Controls.Add(tHeaderrow);

            tHeaderrow = new TableRow();
            tHeadercell = new TableCell();


            tHeadercell = new TableCell();
            tHeadercell.CssClass = "ektronToolbar";
            tHeadercell.ID = "htmToolBar";
            tHeadercell.Text = GetToolBar();

            tHeaderrow.Controls.Add(tHeadercell);
            tHeadertable.Controls.Add(tHeaderrow);

            //outer
            tOutertable.CellPadding = 0;
            tOutertable.CellSpacing = 0;
            tOutertable.Width = Unit.Percentage(100);

            tOutercell.Controls.Add(tHeadertable);
            tOuterrow.Controls.Add(tOutercell);
            tOutertable.Controls.Add(tOuterrow);
            tOutertable.CssClass = "baseClassToolbar";

            if ((m_cTR != null) && m_cTR.GetType() == typeof(TableRow))
            {
                tOutertable.Controls.Add(m_cTR);
            }

            if (this.m_wamTabs.TabsOn && this.Version8TabsImplemented == false)
            {
                tOutercell = new TableCell();
                tOuterrow = new TableRow();
                tOutercell.Text = this.m_wamTabs.RenderTabs();
                tOuterrow.Controls.Add(tOutercell);
                tOutertable.Controls.Add(tOuterrow);
            }

            Panel divWrapper = new Panel();
            divWrapper.CssClass = "ektronPageHeader";
            if (this.m_sPageAction == "addedit")
            {
                divWrapper.Controls.Add(GetUniqueIdField());
            }
            divWrapper.Controls.Add(tOutertable);
            form.Controls.AddAt(0, divWrapper);

            //    'Render as usual
            base.Render(writer);
        }

        private Control GetUniqueIdField()
        {
            return Utilities.GetUniqueIdField( m_refContentApi.RequestInformationRef.UniqueId);
        }

        public void AddTableRow(TableRow tr)
        {
            m_cTR = tr;
        }

        private string GetToolBar()
        {
            System.Text.StringBuilder sbButtons = new System.Text.StringBuilder();
            sbButtons.Append("<table cellspacing=\"0\"><tr>");
            for (int i = 0; i <= (aButtons.Count - 1); i++)
            {
                sbButtons.Append(aButtons[i]);
            }
            sbButtons.Append("</tr></table>");
            if (noticeContainer != "")
            {
                sbButtons.Append("<div class=\"noticeContainer\">");
                sbButtons.Append(noticeContainer);
                sbButtons.Append("</div>");
            }
            return sbButtons.ToString();
        }

        private string RenderTreeJS()
        {
            StringBuilder sbTreeJS = new StringBuilder();
            sbTreeJS.Append("    <script language=\"javascript\" type=\"text/javascript\">").Append(Environment.NewLine);
            sbTreeJS.Append(" // begin tree JS ").Append(Environment.NewLine);
            if (this.m_bAjaxTree == true)
            {
                sbTreeJS.Append("			if(typeof(top[\"ek_nav_bottom\"])!= \'undefined\'){").Append(Environment.NewLine);
                sbTreeJS.Append("				if(typeof(top[\"ek_nav_bottom\"][\"NavIframeContainer\"])!= \'undefined\'){").Append(Environment.NewLine);
                sbTreeJS.Append("					if(typeof(top[\"ek_nav_bottom\"][\"NavIframeContainer\"][\"nav_folder_area\"])!= \'undefined\'){").Append(Environment.NewLine);
                sbTreeJS.Append("						if(typeof(top[\"ek_nav_bottom\"][\"NavIframeContainer\"][\"nav_folder_area\"][\"ContentTree\"])!= \'undefined\'){").Append(Environment.NewLine);
                sbTreeJS.Append("							var treeobj=top[\"ek_nav_bottom\"][\"NavIframeContainer\"][\"nav_folder_area\"][\"ContentTree\"];").Append(Environment.NewLine);
                sbTreeJS.Append("							if(treeobj.document.getElementById(\"selected_folder_id\")!=null){").Append(Environment.NewLine);
                sbTreeJS.Append("								var SelectedTreeId=treeobj.document.getElementById(\"selected_folder_id\").value;").Append(Environment.NewLine);
                sbTreeJS.Append("								var CurrentFolderId=\"" + Request.QueryString["id"] + "\";").Append(Environment.NewLine);
                sbTreeJS.Append("								if(CurrentFolderId==0 && SelectedTreeId!=0) {").Append(Environment.NewLine);
                sbTreeJS.Append("									var stylenode = treeobj.document.getElementById( SelectedTreeId );").Append(Environment.NewLine);
                sbTreeJS.Append("									if(stylenode!=null){").Append(Environment.NewLine);
                sbTreeJS.Append("										stylenode.style[\"background\"] = \"#ffffff\";").Append(Environment.NewLine);
                sbTreeJS.Append("										stylenode.style[\"color\"] = \"#000000\";").Append(Environment.NewLine);
                sbTreeJS.Append("										var stylenode = treeobj.document.getElementById( 0/*CurrentFolderId*/ );").Append(Environment.NewLine);
                sbTreeJS.Append("										stylenode.style[\"background\"] = \"#3366CC\";").Append(Environment.NewLine);
                sbTreeJS.Append("										stylenode.style[\"color\"] = \"#ffffff\";").Append(Environment.NewLine);
                sbTreeJS.Append("									}").Append(Environment.NewLine);
                sbTreeJS.Append("								}").Append(Environment.NewLine);
                sbTreeJS.Append("							}").Append(Environment.NewLine);
                sbTreeJS.Append("						}").Append(Environment.NewLine);
                sbTreeJS.Append("					}").Append(Environment.NewLine);
                sbTreeJS.Append("				}").Append(Environment.NewLine);
                sbTreeJS.Append("			}").Append(Environment.NewLine);
                sbTreeJS.Append("			function reloadFolder(pid){").Append(Environment.NewLine);
                sbTreeJS.Append("				alert(\'reloadtree\');").Append(Environment.NewLine);
                sbTreeJS.Append("				reloadTreeByName(pid,\"ContentTree\");").Append(Environment.NewLine);
                sbTreeJS.Append("				reloadTreeByName(pid,\"FormsTree\");").Append(Environment.NewLine);
                sbTreeJS.Append("				reloadTreeByName(pid,\"LibraryTree\");").Append(Environment.NewLine);
                sbTreeJS.Append("			}").Append(Environment.NewLine);
                sbTreeJS.Append("			function reloadTreeByName(pid,TreeName){").Append(Environment.NewLine);
                sbTreeJS.Append("				alert(\'reload \' + pid + \' \' + TreeName);").Append(Environment.NewLine);
                sbTreeJS.Append("				var obj=top[\"ek_nav_bottom\"][\"NavIframeContainer\"][\"nav_folder_area\"][TreeName];").Append(Environment.NewLine);
                sbTreeJS.Append("				if(obj!=null){").Append(Environment.NewLine);
                sbTreeJS.Append("				    var node = obj.document.getElementById( \"T\" + pid );").Append(Environment.NewLine);
                sbTreeJS.Append("				    if(node!=null){").Append(Environment.NewLine);
                sbTreeJS.Append("					    for (var i=0;i<node.childNodes.length;i++){").Append(Environment.NewLine);
                sbTreeJS.Append("						    if(node.childNodes[i].nodeName==\'LI\' || node.childNodes[i].nodeName==\'UL\'){").Append(Environment.NewLine);
                sbTreeJS.Append("				alert(\'1\');").Append(Environment.NewLine);
                sbTreeJS.Append("							    var parent = node.childNodes[i].parentElement;").Append(Environment.NewLine);
                sbTreeJS.Append("							    parent.removeChild( node.childNodes[i]);").Append(Environment.NewLine);
                sbTreeJS.Append("						    }").Append(Environment.NewLine);
                sbTreeJS.Append("					    }").Append(Environment.NewLine);
                sbTreeJS.Append("					    obj.TREES[\"T\" + pid].children = [];").Append(Environment.NewLine);
                sbTreeJS.Append("					    obj.onToggleClick(pid,obj.callback_function,pid);").Append(Environment.NewLine);
                sbTreeJS.Append("				    }").Append(Environment.NewLine);
                sbTreeJS.Append("				} ").Append(Environment.NewLine);
                sbTreeJS.Append("			}").Append(Environment.NewLine);
            }
            sbTreeJS.Append(" // end tree JS ").Append(Environment.NewLine);
            sbTreeJS.Append("    </script>").Append(Environment.NewLine);
            return sbTreeJS.ToString();
        }

        private string RenderCSS()
        {
            System.Text.StringBuilder sbCSS = new System.Text.StringBuilder();
            return sbCSS.ToString();
        }

        public string ReloadClientScript()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            long pid = 0;
            try
            {
                if (!(Request.QueryString["id"] == null))
                {
                    pid = Convert.ToInt64(Request.QueryString["id"]);
                }
                else if (!(Request.QueryString["id"] == null))
                {
                    pid = Convert.ToInt64(Request.QueryString["folder_id"]);
                }
                result.Append("<script language=\"javascript\">" + "\r\n");
                if (m_refContentApi.TreeModel == 1 && pid != 0)
                {
                    if ((!(Request.QueryString["TreeUpdated"] == null)) && (Request.QueryString["TreeUpdated"] == "1"))
                    {
                        pid = m_refContentApi.GetParentIdByFolderId(pid);
                        if (pid == -1)
                        {
                            result.Length = 0;
                            //break;
                        }
                    }

                    result.Append("reloadFolder(" + Convert.ToString(pid) + ");" + "\r\n");
                }
                else
                {
                    result.Append("<!--" + "\r\n");
                    result.Append("	// If reloadtrees paramter exists, reload selected navigation trees:" + "\r\n");
                    result.Append("	var m_reloadTrees = \"" + Request.QueryString["reloadtrees"] + "\";" + "\r\n");
                    result.Append("	top.ReloadTrees(m_reloadTrees);" + "\r\n");
                    result.Append("	self.location.href=\"" + Request.ServerVariables["path_info"] + "?" + Strings.Replace(Request.ServerVariables["query_string"], (string)("&reloadtrees=" + Request.QueryString["reloadtrees"]), "", 1, -1, 0) + "\";" + "\r\n");
                    result.Append("	// If TreeNav parameters exist, ensure the desired folders are opened:" + "\r\n");
                    result.Append("	var strTreeNav = \"" + Request.QueryString["TreeNav"] + "\";" + "\r\n");
                    result.Append("	if (strTreeNav != null) {" + "\r\n");
                    result.Append("		strTreeNav = strTreeNav.replace(/\\\\\\\\/g,\"\\\\\");" + "\r\n");
                    result.Append("		top.TreeNavigation(\"ContentTree\", strTreeNav);" + "\r\n");
                    result.Append("	}" + "\r\n");
                    result.Append("//-->" + "\r\n");
                }
                result.Append("</script>" + "\r\n");
            }
            catch (Exception)
            {
            }
            return (result.ToString());
        }

        protected int GetQueryLanguage()
        {
            int result = ContentLanguage;
            int tempVal = 0;
            try
            {
                if (!(Request.QueryString["LangType"] == null) && (Request.QueryString["LangType"].Length > 0))
                {
                    if (int.TryParse((string)(Request.QueryString["LangType"].Replace("#", "")), out tempVal))
                    {
                        result = tempVal;
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool IsLoggedIn
        {
            get
            {
                return m_refContentApi.IsLoggedIn;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return IsLoggedIn && m_refContentApi.IsAdmin();
            }
        }

        public bool IsCommerceAdmin
        {
            get
            {
                return IsLoggedIn && (IsAdmin || m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin));
            }
        }
    }

    public class workareamenu
    {

        protected string m_sName = "";
        protected string m_sLabel = "";
        protected string m_sImagePath = "";
        protected System.Collections.Generic.List<string> m_lItems;

        public workareamenu(string MenuName, string Label, string ImagePath)
        {
            m_lItems = new System.Collections.Generic.List<string>();

            this.m_sName = MenuName;
            this.m_sLabel = Label;
            this.m_sImagePath = ImagePath;
        }
        public void AddItem(string ImagePath, string Label, string JScode)
        {
            m_lItems.Add(m_sName + "menu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + ImagePath + "\' />&nbsp;&nbsp;" + Label + "\", function() { " + JScode + " } ); ");
        }
        public void AddLinkItem(string ImagePath, string Label, string URL)
        {
            this.AddItem(ImagePath, Label, "window.location.href = \'" + URL + "\';");
        }
        public void AddHREFLinkItem(string ImagePath, string Label, string URL)
        {
            m_lItems.Add(m_sName + "menu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + ImagePath + "\' />&nbsp;&nbsp;<a href=\'" + URL.Replace("\"", "\\\"") + "\'>" + Label + "</a>\", function() { return false; } ); ");
        }
        public void AddBreak()
        {
            m_lItems.Add(m_sName + "menu.addBreak();");
        }
        public string Render()
        {
            return Render("");
        }
        public string Render(string checkvariable)
        {

            StringBuilder result = new StringBuilder();

            if (m_lItems.Count > 0)
            {

                result.Append("<script language=\"javascript\"> ");
                result.Append("var " + m_sName + "menu = new Menu( \"" + m_sName + "\" ); ");

                for (int i = 0; i <= (m_lItems.Count - 1); i++)
                {

                    result.Append(m_lItems[i].ToString());

                }

                result.Append("MenuUtil.add( " + m_sName + "menu ); ");
                result.Append("</script>" + Environment.NewLine);

                if (checkvariable != "")
                {

                    result.Append("<td class=\"menuRootItem\" onclick=\"if (" + checkvariable + ") { MenuUtil.use(event, \'" + m_sName + "\'); } else { return false; }\" onmouseover=\"if (" + checkvariable + ") { this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'" + m_sName + "\'); } else { return false; }\" onmouseout=\"if (" + checkvariable + ") { this.className=\'menuRootItem\';} else { return false; }\"><span class=\"dropdownMenuButton\" >" + m_sLabel + "</span></td>");

                }
                else
                {

					result.Append("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'" + m_sName + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'" + m_sName + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span class=\"dropdownMenuButton\" >" + m_sLabel + "</span></td>");

                }

            }

            return result.ToString();

        }

    }

    public class workareaCommerce
    {


        protected CommerceSettings m_CommerceSettings = null;
        protected Workarea.workareabase m_WorkAreaBase = null;

        public workareaCommerce(CommerceSettings eCommerceSettings, Workarea.workareabase workareaRef)
        {

            this.m_CommerceSettings = eCommerceSettings;
            m_WorkAreaBase = workareaRef;

        }

        public enum ModeType
        {
            Add,
            Edit,
            View
        }

        public void CheckCommerceAdminAccess()
        {

            if (!m_WorkAreaBase.ContentAPIRef.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.CommerceAdmin))
            {
                throw (new Exception(m_WorkAreaBase.GetMessage("err not role commerce-admin")));
            }

        }

        public string GetProductImage(EkEnumeration.CatalogEntryType entryType)
        {

            return Utilities.GetProductImage(entryType, (string)m_WorkAreaBase.AppImgPath);

        }

        public string GetPricingMarkup(PricingData pricing, List<CurrencyData> currencyList, List<ExchangeRateData> exchangeRateList, Common.EkEnumeration.CatalogEntryType entryType, bool showPricingTier, ModeType Mode)
        {
            StringBuilder sbPricing = new StringBuilder();
            bool showRemoveForDefault = false;
            string defaultCurrencyName = "";
            int defaultCurrencyId = 0;
            Ektron.Cms.Commerce.CurrencyData defaultCurrency = null;

            for (int i = 0; i <= (currencyList.Count - 1); i++)
            {
                if (currencyList[i].Id == m_CommerceSettings.DefaultCurrencyId)
                {
                    defaultCurrencyName = (string)(currencyList[i].Name);
                    defaultCurrencyId = System.Convert.ToInt32(currencyList[i].Id);
                    defaultCurrency = currencyList[i];
                    break;
                }
            }

            sbPricing.Append("             <table width=\"100%\" border=\"1\" bordercolor=\"#d8e6ff\"> ").Append(Environment.NewLine);
            sbPricing.Append("             <tr> ").Append(Environment.NewLine);
            sbPricing.Append("                 <td width=\"100%\"> ").Append(Environment.NewLine);
            sbPricing.Append(" 						    <div class=\"ektron ektron_PricingWrapper\"> ").Append(Environment.NewLine);
            sbPricing.Append("                             <h3> ").Append(Environment.NewLine);
            sbPricing.Append(" 	                            <span class=\"currencyLabel\">").Append(m_CommerceSettings.ISOCurrencySymbol).Append(m_CommerceSettings.CurrencySymbol).Append(" ").Append(defaultCurrencyName).Append("</span> ").Append(Environment.NewLine);
            sbPricing.Append(" 	                            <select onchange=\"Ektron.Commerce.Pricing.selectCurrency(this.options[this.selectedIndex].value, " + defaultCurrencyId + ");return false;\"> ").Append(Environment.NewLine);
            for (int i = 0; i <= (currencyList.Count - 1); i++)
            {
                sbPricing.Append(" 		                            <option value=\"id:ektron_Pricing_").Append(currencyList[i].Id).Append(";label:").Append(currencyList[i].Name).Append(";symbol:").Append(currencyList[i].ISOCurrencySymbol).Append(currencyList[i].CurrencySymbol).Append("\" " + ((currencyList[i].Id == m_CommerceSettings.DefaultCurrencyId) ? "selected=\"selected\"" : "") + ">").Append(currencyList[i].AlphaIsoCode).Append("</option> ").Append(Environment.NewLine);
            }
            sbPricing.Append(" 	                            </select> ").Append(Environment.NewLine);
            sbPricing.Append("                             </h3> ").Append(Environment.NewLine);
            sbPricing.Append("                             <div class=\"ektron_Pricing_InnerWrapper\"> ").Append(Environment.NewLine);
            for (int i = 0; i <= (currencyList.Count - 1); i++)
            {
                bool IsDefaultCurrency = System.Convert.ToBoolean(m_CommerceSettings.DefaultCurrencyId == currencyList[i].Id);
                //decimal actualCost = (decimal)0.0;
                decimal listPrice = (decimal)0.0;
                decimal currentPrice = (decimal)0.0;
                Ektron.Cms.Commerce.CurrencyPricingData currencyPricing = pricing.GetCurrencyById(currencyList[i].Id);
                List<Ektron.Cms.Commerce.TierPriceData> tierPrices = new List<Ektron.Cms.Commerce.TierPriceData>();
                int tierCount = 0;
                long tierId = 0;
                bool IsFloated = false;
                decimal exchangeRate = 1;
                if (currencyPricing != null)
                {
                    //actualCost = currencyPricing.ActualCost
                    listPrice = currencyPricing.ListPrice;
                    currentPrice = currencyPricing.GetSalePrice(1);
                    tierPrices = currencyPricing.TierPrices;
                    tierCount = System.Convert.ToInt32(tierPrices.Count);
                    IsFloated = System.Convert.ToBoolean(currencyPricing.PricingType == Ektron.Cms.Common.EkEnumeration.PricingType.Floating);
                    if (Mode == ModeType.Add && !IsDefaultCurrency)
                    {
                        IsFloated = true;
                    }
                    if (tierPrices.Count > 0)
                    {
                        tierId = tierPrices[0].Id;
                    }
                    if (tierPrices.Count == 0 || (tierPrices.Count == 1 && tierPrices[0].Quantity == 1))
                    {
                        tierCount = 1;
                    }
                    if (currencyPricing.CurrencyId == m_CommerceSettings.DefaultCurrencyId && tierPrices.Count > 0)
                    {
                        showRemoveForDefault = true;
                    }
                }
                else
                {
                    IsFloated = true;
                    tierCount = 1;
                }
                for (int k = 0; k <= (exchangeRateList.Count - 1); k++)
                {
                    if (exchangeRateList[k].ExchangeCurrencyId == currencyList[i].Id)
                    {
                        exchangeRate = System.Convert.ToDecimal(exchangeRateList[k].Rate);
                        break;
                    }
                }
                sbPricing.Append(" 	                            <div id=\"ektron_Pricing_").Append(currencyList[i].Id).Append("\" class=\"ektron_Pricing_CurrencyWrapper ektron_Pricing_").Append(currencyList[i].AlphaIsoCode).Append("" + ((currencyList[i].Id == m_CommerceSettings.DefaultCurrencyId) ? " ektron_Pricing_CurrencyWrapper_Active" : "") + "\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            <table class=\"ektron_UnitPricing_Table\" summary=\"").Append(m_WorkAreaBase.GetMessage("lbl unit pricing data")).Append("\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <colgroup> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <col class=\"narrowCol\"/> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <col class=\"wideCol\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            </colgroup> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <thead> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <th colspan=\"2\" class=\"alignLeft noBorderRight\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.GetMessage("lbl unit pricing")).Append(" ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            </thead> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <tbody> ").Append(Environment.NewLine);

                if (!(currencyList[i].Id == m_CommerceSettings.DefaultCurrencyId))
                {
                    sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine);
                    sbPricing.Append(" 					                            <th class=\"noBorderRight\"> ").Append(Environment.NewLine);
                    sbPricing.Append(" 						                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl price float exchange")).Append("\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl price float exchange")).Append("\" class=\"moreInfo\" /> ").Append(Environment.NewLine);
                    sbPricing.Append(" 						                            <label for=\"ektron_UnitPricing_Float_").Append(currencyList[i].Id).Append("\">").Append(m_WorkAreaBase.GetMessage("lbl price float")).Append(":</label> ").Append(Environment.NewLine);
                    sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine);
                    sbPricing.Append(" 					                            <td class=\"noBorderLeft\"> ").Append(Environment.NewLine);
                    sbPricing.Append(" 						                            <span class=\"currencySymbol\">").Append(Environment.NewLine);
                    sbPricing.Append("<input onclick=\"Ektron.Commerce.Pricing.floatToggle(this);\" id=\"ektron_UnitPricing_Float_").Append(currencyList[i].Id).Append("\" name=\"ektron_UnitPricing_Float_").Append(currencyList[i].Id).Append("\" class=\"actualPrice\" type=\"checkbox\" ").Append(IsFloated ? "checked=\"checked\" " : "").Append(" ").Append((!(Mode == ModeType.View)) ? "" : "disabled=\"disabled\" ").Append("/> ").Append(Environment.NewLine);
                    sbPricing.Append("</span> ").Append(Environment.NewLine);
                    sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.GetMessage("lbl price current rate")).Append(": ").Append(defaultCurrency.ISOCurrencySymbol).Append(defaultCurrency.CurrencySymbol).Append("1 = ").Append(currencyList[i].ISOCurrencySymbol).Append(currencyList[i].CurrencySymbol).Append(m_WorkAreaBase.FormatCurrency(exchangeRate, "")).Append(Environment.NewLine);

                    sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine);
                    sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine);
                }

                if (!(Mode == ModeType.View))
                {
                    //sbPricing.Append(" 						                            <input maxlength=""8"" id=""ektron_UnitPricing_ActualPrice_").Append(currencyList[i].Id).Append(""" onchange=""UpdateActualPrice(this);"" name=""ektron_UnitPricing_ActualPrice_").Append(currencyList[i].Id).Append(""" class=""actualPrice"" type=""text"" value=""" & m_WorkAreaBase.FormatCurrency(actualCost, "") & """ " + IIf(IsFloated, "disabled=""disabled"" ", "") + " /> ").Append(Environment.NewLine)
                }
                else
                {
                    //sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.FormatCurrency(actualCost, "")).Append(Environment.NewLine)
                }
                //sbPricing.Append(" 						                            &#160;").Append(m_WorkAreaBase.GetMessage("lbl per unit")).Append(" ").Append(Environment.NewLine)
                //sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine)
                //sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine)
                sbPricing.Append(" 				                            <tr class=\"stripe\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <th class=\"noBorderRight\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl list price")).Append("\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl list price desc")).Append("\" class=\"moreInfo\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <label for=\"ektron_UnitPricing_ListPrice_").Append(currencyList[i].Id).Append("\" class=\"listPrice\">").Append(m_WorkAreaBase.GetMessage("lbl list price")).Append(":</label> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <td class=\"noBorderLeft\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <span class=\"currencySymbol\">").Append(currencyList[i].ISOCurrencySymbol).Append(currencyList[i].CurrencySymbol).Append("</span> ").Append(Environment.NewLine);
                if (!(Mode == ModeType.View))
                {
                    sbPricing.Append(" 						                            <input maxlength=\"8\" id=\"ektron_UnitPricing_ListPrice_").Append(currencyList[i].Id).Append("\" onchange=\"UpdateListPrice(this);\" name=\"ektron_UnitPricing_ListPrice_").Append(currencyList[i].Id).Append("\" type=\"text\" value=\"" + m_WorkAreaBase.FormatCurrency(listPrice, "") + ("\" " + (IsFloated ? "disabled=\"disabled\" " : "") + " /> ")).Append(Environment.NewLine);
                }
                else
                {
                    sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.FormatCurrency(listPrice, "")).Append(Environment.NewLine);
                }
                sbPricing.Append(" 						                            &#160;").Append(m_WorkAreaBase.GetMessage("lbl per unit")).Append(" ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <th class=\"noBorderRight\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl our sales price")).Append("\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl our sales price desc")).Append("\" class=\"moreInfo\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <label for=\"ektron_UnitPricing_SalesPrice_").Append(currencyList[i].Id).Append("\">").Append(m_WorkAreaBase.GetMessage("lbl our sales price")).Append(":</label> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <td class=\"noBorderLeft\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <span class=\"currencySymbol\">").Append(currencyList[i].ISOCurrencySymbol).Append(currencyList[i].CurrencySymbol).Append("</span> ").Append(Environment.NewLine);
                if (!(Mode == ModeType.View))
                {
                    sbPricing.Append(" 						                            <input maxlength=\"8\" onchange=\"UpdateSalesPrice(this);\" id=\"ektron_UnitPricing_SalesPrice_").Append(currencyList[i].Id).Append("\" name=\"ektron_UnitPricing_SalesPrice_").Append(currencyList[i].Id).Append("\" type=\"text\" value=\"" + m_WorkAreaBase.FormatCurrency(currentPrice, "") + ("\" " + (IsFloated ? "disabled=\"disabled\" " : "") + " /> ")).Append(Environment.NewLine);
                }
                else
                {
                    sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.FormatCurrency(currentPrice, "")).Append(Environment.NewLine);
                }
                sbPricing.Append(" 							                            <input id=\"hdn_ektron_UnitPricing_DefaultTier_").Append(currencyList[i].Id).Append("\" name=\"hdn_ektron_UnitPricing_DefaultTier_").Append(currencyList[i].Id).Append("\" class=\"noFloat\" type=\"hidden\" ");
                sbPricing.Append("value=\"").Append(tierId).Append("\"");
                sbPricing.Append("/> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            &#160;").Append(m_WorkAreaBase.GetMessage("lbl per unit")).Append(" ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            </tbody> ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            </table> ").Append(Environment.NewLine);
          
                sbPricing.Append(" 		                            <div class=\"ektron_TierPricing_Wrapper\" ").Append(tierCount > 1 ? "style=\"display:block;\"" : "").Append("> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <table class=\"ektron_TierPricing_Table\" summary=\"").Append(m_WorkAreaBase.GetMessage("lbl tier pricing data")).Append("\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <colgroup> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <col class=\"ektron_TierPricing_TierRemove\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <col class=\"ektron_TierPricing_TierQuantity\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <col class=\"ektron_TierPricing_TierPrice\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </colgroup> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <thead> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <th colspan=\"3\" class=\"alignLeft\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 							                            ").Append(m_WorkAreaBase.GetMessage("lbl tier pricing")).Append(" ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            </th> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <tr class=\"ektron_TierPricing_HeaderLabels\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <th><img class=\"ektron_TierPricing_HeaderRemoveImage\" src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/delete.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append("\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append("\" /></th> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <th>").Append(m_WorkAreaBase.GetMessage("lbl if num units greater or equal")).Append("</th> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <th>").Append(m_WorkAreaBase.GetMessage("lbl then tier price is")).Append("</th> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </thead> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <tbody> ").Append(Environment.NewLine);
                int jModifier = 0;
                for (int j = 0; j <= (tierCount - 1); j++)
                {
                    int tierQuantity = 0;
                    long tierQId = 0;
                    decimal tierSalePrice = (decimal)0.0;

                    bool bShow = true;

                    if (j == 0 && tierPrices.Count == 0) // old way
                    {
                        // do nothing
                    }
                    else if (j == 0 && tierPrices.Count == 1 && tierPrices[0].Quantity == 1) //no tier pricing
                    {
                        //do nothing
                    }
                    else if (j == 0 && tierPrices.Count > 0 && tierPrices[0].Quantity == 1) // first is quantity 1 so skip
                    {
                        jModifier = -1;
                        if (tierPrices.Count > 1)
                        {
                            bShow = false;
                        }
                    }
                    else
                    {
                        tierQuantity = System.Convert.ToInt32(tierPrices[j].Quantity);
                        tierSalePrice = System.Convert.ToDecimal(tierPrices[j].SalePrice);
                        tierQId = tierPrices[j].Id;
                    }
                    if (bShow)
                    {
                        sbPricing.Append(" 					                            <tr class=\"tier stripe\" id=\"tier_").Append(j + jModifier).Append("\"> ").Append(Environment.NewLine);
                        sbPricing.Append(" 						                            <td class=\"tierRemove\"> ").Append(Environment.NewLine);
                        if (!(Mode == ModeType.View))
                        {
                            sbPricing.Append(" 							                            <input type=\"checkbox\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl remove tier")).Append("\" class=\"ektron_RemoveTier_Checkbox\" onclick=\"Ektron.Commerce.Pricing.Tier.toggleRemove();\"/> ").Append(Environment.NewLine);
                        }
                        sbPricing.Append(" 						                            </td> ").Append(Environment.NewLine);
                        sbPricing.Append(" 						                            <td class=\"tierQuantity\"> ").Append(Environment.NewLine);
                        if (!((Mode == ModeType.View) == true) && !IsFloated)
                        {
                            sbPricing.Append(" 							                            <input maxlength=\"9\" id=\"ektron_TierPricing_TierQuantity_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" name=\"ektron_TierPricing_TierQuantity_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" type=\"text\" ");
                            sbPricing.Append("value=\"" + tierQuantity + "\"");
                            sbPricing.Append("/> ").Append(Environment.NewLine);
                        }
                        else
                        {
                            sbPricing.Append(" 							                            <input maxlength=\"9\" id=\"ektron_TierPricing_TierQuantity_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" name=\"ektron_TierPricing_TierQuantity_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" type=\"text\" disabled=\"disabled\" ");
                            sbPricing.Append("value=\"" + tierQuantity + "\"");
                            sbPricing.Append("/> ").Append(Environment.NewLine);
                        }
                        sbPricing.Append(" 							                            <input id=\"hdn_ektron_TierPricing_TierId_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" name=\"hdn_ektron_TierPricing_TierId_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" type=\"hidden\" ");
                        sbPricing.Append("value=\"" + tierQId + "\"");
                        sbPricing.Append("/> ").Append(Environment.NewLine);
                        sbPricing.Append(" 						                            </td> ").Append(Environment.NewLine);
                        sbPricing.Append(" 						                            <td class=\"tierPrice\"> ").Append(Environment.NewLine);
                        sbPricing.Append(" 							                            <span class=\"currencySymbol noFloat\">").Append(currencyList[i].ISOCurrencySymbol).Append(currencyList[i].CurrencySymbol).Append("</span> ").Append(Environment.NewLine);
                        if (!((Mode == ModeType.View) == true) && !IsFloated)
                        {
                            sbPricing.Append(" 							                            <input maxlength=\"12\" id=\"ektron_TierPricing_TierPrice_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" name=\"ektron_TierPricing_TierPrice_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" class=\"noFloat\" type=\"text\" ");
                            sbPricing.Append("value=\"" + m_WorkAreaBase.FormatCurrency(tierSalePrice, "") + "\"");
                            sbPricing.Append("/> ").Append(Environment.NewLine);
                        }
                        else
                        {
                            sbPricing.Append(" 							                            <input maxlength=\"12\" id=\"ektron_TierPricing_TierPrice_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" name=\"ektron_TierPricing_TierPrice_").Append(currencyList[i].Id).Append("_").Append(j + jModifier).Append("\" class=\"noFloat\" type=\"text\" disabled=\"disabled\" ");
                            sbPricing.Append("value=\"" + m_WorkAreaBase.FormatCurrency(tierSalePrice, "") + "\"");
                            sbPricing.Append("/> ").Append(Environment.NewLine);
                        }
                        sbPricing.Append(" 						                            </td> ").Append(Environment.NewLine);
                        sbPricing.Append(" 					                            </tr> ").Append(Environment.NewLine);
                    }
                }
                sbPricing.Append(" 				                            </tbody> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            </table> ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            </div> ").Append(Environment.NewLine);
                sbPricing.Append(" 	                            </div> ").Append(Environment.NewLine);
            }
            if (Mode != ModeType.View && showPricingTier == true)
            {
                sbPricing.Append(" 	                            <p class=\"ektron_TierPricing_Commands clearfix\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            <a href=\"#AddPricingTier\" class=\"button buttonRight greenHover marginLeft\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl add pricing tier")).Append("\" onclick=\"Ektron.Commerce.Pricing.Tier.addTier(this);return false;\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/coins_add.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl add pricing tier")).Append("\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            ").Append(m_WorkAreaBase.GetMessage("lbl add pricing tier")).Append(" ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            </a> ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            <a href=\"#RemovePricingTier\" class=\"button buttonRight ektron_RemovePricingTier_Button disabled\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append("\" onclick=\"Ektron.Commerce.Pricing.Tier.removeTier();return false;\" ").Append(showRemoveForDefault ? "style=\"display:block;\"" : "").Append("> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/coins_delete.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append("\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            ").Append(m_WorkAreaBase.GetMessage("lbl remove pricing tier")).Append(" ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            </a> ").Append(Environment.NewLine);
                sbPricing.Append(" 	                            </p> ").Append(Environment.NewLine);
            }
            sbPricing.Append("                             </div> ").Append(Environment.NewLine);
            sbPricing.Append("                             <div id=\"ektron_Pricing_Modal\" class=\"ektronWindow\"> ").Append(Environment.NewLine);
            sbPricing.Append(" 	                            <h4 id=\"ektron_Pricing_Modal_Header\"> ").Append(Environment.NewLine);
            sbPricing.Append(" 		                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/closeButton.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl cancel and close window")).Append("\" class=\"ektronModalClose\" />	 ").Append(Environment.NewLine);
            sbPricing.Append(" 	                            </h4> ").Append(Environment.NewLine);
            sbPricing.Append(" 	                            <div class=\"ektron_Pricing_Modal_InnerWrapper\"> ").Append(Environment.NewLine);
            sbPricing.Append(" 		                            <p>").Append(m_WorkAreaBase.GetMessage("js confirm remove selected pricing tiers")).Append("</p> ").Append(Environment.NewLine);
            sbPricing.Append(" 		                            <p class=\"buttons clearfix\"> ").Append(Environment.NewLine);
            sbPricing.Append(" 			                            <a href=\"#Ok\" class=\"button buttonRight greenHover marginLeft ektronModalClose\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl ok")).Append("\" onclick=\"Ektron.Commerce.Pricing.Tier.removeTier();return false;\"> ").Append(Environment.NewLine);
            sbPricing.Append(" 				                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/accept.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl ok")).Append("\" /> ").Append(Environment.NewLine);
            sbPricing.Append(" 				                            ").Append(m_WorkAreaBase.GetMessage("lbl ok")).Append(" ").Append(Environment.NewLine);
            sbPricing.Append(" 			                            </a> ").Append(Environment.NewLine);
            sbPricing.Append(" 			                            <a href=\"#Cancel\" class=\"button buttonRight redHover ektronModalClose\" title=\"").Append(m_WorkAreaBase.GetMessage("generic cancel")).Append("\" onclick=\"return false;\"> ").Append(Environment.NewLine);
            sbPricing.Append(" 				                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/cancel.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("generic cancel")).Append("\" /> ").Append(Environment.NewLine);
            sbPricing.Append(" 				                            ").Append(m_WorkAreaBase.GetMessage("generic cancel")).Append(" ").Append(Environment.NewLine);
            sbPricing.Append(" 			                            </a> ").Append(Environment.NewLine);
            sbPricing.Append(" 		                            </p> ").Append(Environment.NewLine);
            sbPricing.Append(" 	                            </div> ").Append(Environment.NewLine);
            sbPricing.Append("                             </div> ").Append(Environment.NewLine);
            sbPricing.Append("                         </div> ").Append(Environment.NewLine);
            sbPricing.Append("                          ").Append(Environment.NewLine);
            sbPricing.Append("                 </td> ").Append(Environment.NewLine);
            sbPricing.Append("             </tr> ").Append(Environment.NewLine);
            sbPricing.Append("             </table> ").Append(Environment.NewLine);

            if (entryType == Ektron.Cms.Common.EkEnumeration.CatalogEntryType.SubscriptionProduct)
            {

                Ektron.Cms.Common.RecurrenceType recurrenceType = Ektron.Cms.Common.RecurrenceType.MonthlyByDay;
                int recurrenceInterval = 1;

                if (pricing.IsRecurringPrice)
                {

                    recurrenceType = pricing.Recurrence.RecurrenceType;
                    recurrenceInterval = pricing.Recurrence.Intervals;

                }


                sbPricing.Append(" 		                            <input class=\"EktronRecurringPricingEditStatus\" type=\"hidden\" value=\"").Append(pricing.IsRecurringPrice ? "true" : "false").Append("\" />").Append(Environment.NewLine);
                sbPricing.Append(" 		                            <input class=\"EktronRecurringPricingMode\" type=\"hidden\" value=\"").Append(Mode.ToString()).Append("\" />").Append(Environment.NewLine);

                sbPricing.Append(" 		                            <table class=\"ektron_RecurringPricing_Table\" summary=\"").Append(m_WorkAreaBase.GetMessage("lbl recurring billing data")).Append("\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <colgroup> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <col class=\"narrowCol\"/> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <col class=\"wideCol\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            </colgroup> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <thead> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            <tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <th colspan=\"2\" class=\"alignLeft noBorderRight\"> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            ").Append(m_WorkAreaBase.GetMessage("lbl recurring billing")).Append(" ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            </thead> ").Append(Environment.NewLine);
                sbPricing.Append(" 			                            <tbody> ").Append(Environment.NewLine);

                ///''''''''''''''''''''''''''''''''''''''''
                //Row: Use Recurrent Billing
                sbPricing.Append(" 				                            <tr>").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <th> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl use recurrent billing")).Append("\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl use recurrent billing")).Append("\" class=\"moreInfo\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <label for=\"PricingTabRecurringBillingUseRecurrentBilling").Append("\">").Append(m_WorkAreaBase.GetMessage("lbl use recurrent billing")).Append(":</label> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </th> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <td> ").Append(Environment.NewLine);
                if (Mode == ModeType.View)
                {
                    sbPricing.Append(" 						                            <span id=\"PricingTabRecurringBillingUseRecurrentBilling\">").Append(Environment.NewLine);
                    sbPricing.Append(pricing.IsRecurringPrice ? "Yes" : "No");
                    sbPricing.Append("                                                  </span> ").Append(Environment.NewLine);
                }
                else
                {
                    sbPricing.Append(" 						                            <select class=\"recurringBilling\" onchange=\"Ektron.Commerce.Pricing.floatRecurring(this);\" name=\"PricingTabRecurringBillingUseRecurrentBilling\" id=\"PricingTabRecurringBillingUseRecurrentBilling\">").Append(Environment.NewLine);
                    sbPricing.Append(" 						                                <option value=\"true\"").Append(pricing.IsRecurringPrice ? "selected=\"selected\"" : "").Append(">Yes</option>").Append(Environment.NewLine);
                    sbPricing.Append(" 						                                <option value=\"false\"").Append(pricing.IsRecurringPrice ? "" : "selected=\"selected\"").Append(">No</option>").Append(Environment.NewLine);
                    sbPricing.Append("                                                  </span> ").Append(Environment.NewLine);
                }
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine);

                ///''''''''''''''''''''''''''''''''''''''''
                //Row: Billing Cycle
                sbPricing.Append(" 				                            <tr class=\"billingCycle stripe\">").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <th>").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl billing cycle")).Append("\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl billing cycle desc")).Append("\" class=\"moreInfo\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <label for=\"PricingTabRecurringBillingBillingCycle").Append("\" class=\"billingCycle\">").Append(m_WorkAreaBase.GetMessage("lbl billing cycle")).Append(":</label> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </th>").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <td>").Append(Environment.NewLine);
                if (Mode == ModeType.View)
                {
                    sbPricing.Append(" 						                            <span id=\"PricingTabRecurringBillingBillingCycle\">").Append(Environment.NewLine);
                    sbPricing.Append(recurrenceType == Ektron.Cms.Common.RecurrenceType.MonthlyByDay ? "Monthly" : "");
                    sbPricing.Append(recurrenceType == Ektron.Cms.Common.RecurrenceType.Yearly ? "Yearly" : "");
                }
                else
                {
                    sbPricing.Append(" 						                            <select id=\"PricingTabRecurringBillingBillingCycle\" name=\"PricingTabRecurringBillingBillingCycle\" ").Append(this.GetEnabled(Mode, pricing)).Append(" /> ").Append(Environment.NewLine);
                    sbPricing.Append("                                                      <option value=\"month\"").Append(recurrenceType == Ektron.Cms.Common.RecurrenceType.MonthlyByDay ? " SELECTED " : "").Append(">Monthly</option>");
                    sbPricing.Append("                                                      <option value=\"year\"").Append(recurrenceType == Ektron.Cms.Common.RecurrenceType.Yearly ? " SELECTED " : "").Append(">Yearly</option>");
                    sbPricing.Append("                                                  </select>").Append(Environment.NewLine);
                }
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </tr>").Append(Environment.NewLine);

                ///''''''''''''''''''''''''''''''''''''''''
                //Row: Interval
                sbPricing.Append(" 				                            <tr class=\"interval\">").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <th>").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <img src=\"").Append(m_WorkAreaBase.AppImgPath).Append("commerce/about.gif\" alt=\"").Append(m_WorkAreaBase.GetMessage("lbl billing intervals")).Append("\" title=\"").Append(m_WorkAreaBase.GetMessage("lbl billing intervals desc")).Append("\" class=\"moreInfo\" /> ").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <label for=\"PricingTabRecurringBillingInterval").Append("\" class=\"StartDate\">").Append(m_WorkAreaBase.GetMessage("lbl billing intervals")).Append(":</label> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </th>").Append(Environment.NewLine);
                sbPricing.Append(" 					                            <td>").Append(Environment.NewLine);
                sbPricing.Append(" 						                            <input maxlength=\"8\" type=\"text\" class=\"interval\" ").Append(this.GetEnabled(Mode, pricing)).Append(" name=\"PricingTabRecurringBillingInterval\" title=\"Select Interval\" id=\"PricingTabRecurringBillingInterval\" value=\"" + recurrenceInterval + "\" />").Append(Environment.NewLine);
                sbPricing.Append(" 					                                <span class=\"intervalRequired\">* must be numeric</span> ").Append(Environment.NewLine);
                sbPricing.Append(" 					                            </td> ").Append(Environment.NewLine);
                sbPricing.Append(" 				                            </tr> ").Append(Environment.NewLine);

                sbPricing.Append(" 			                            </tbody> ").Append(Environment.NewLine);
                sbPricing.Append(" 		                            </table> ").Append(Environment.NewLine);

                if (!(Mode == ModeType.View))
                {

                    sbPricing.Append("                                  <div class=\"finish\"> ").Append(Environment.NewLine);
                    sbPricing.Append(" 		                            <h3>").Append(m_WorkAreaBase.GetMessage("lbl important")).Append("</h3> ").Append(Environment.NewLine);
                    sbPricing.Append(" 		                            <div class=\"innerWrapper\"> ").Append(Environment.NewLine);
                    sbPricing.Append(" 		                            <p><span>").Append(m_WorkAreaBase.GetMessage("lbl recurring billing test")).Append("</span></p> ").Append(Environment.NewLine);
                    sbPricing.Append(" 		                            </div> ").Append(Environment.NewLine);
                    sbPricing.Append(" 		                            </div> ").Append(Environment.NewLine);

                }

            }

            return sbPricing.ToString();
        }

        private string GetEnabled(Ektron.Cms.Workarea.workareaCommerce.ModeType Mode, Ektron.Cms.Commerce.PricingData pricing)
        {
            string returnValue;
            returnValue = string.Empty;
            if (Mode == ModeType.View || (Mode != ModeType.View && pricing.IsRecurringPrice == false))
            {
                returnValue = "disabled=\"disabled\"";
            }
            return returnValue;
        }

        private string GetDate(Ektron.Cms.Commerce.PricingData pricing, string StartOrEnd)
        {
            string returnValue;
            returnValue = DateTime.Today.ToString("MM/dd/yyyy");

            if ((pricing != null) && (pricing.Recurrence != null))
            {
                DateTime parsedDate;
                // bool isDate;
                if (StartOrEnd == "start")
                {
                    returnValue = Convert.ToString(DateTime.TryParse(pricing.Recurrence.StartDateUtc.ToString(), out parsedDate));
                }
                else
                {
                    returnValue = Convert.ToString(DateTime.TryParse(pricing.Recurrence.EndDateUtc.ToString(), out parsedDate));
                }
                //if (isDate == true)
                //{
                //    returnValue = Convert.ToString (parsedDate);
                //}
            }

            return returnValue;
        }
    }


    public class workareajavascript
    {
        public enum ErrorType
        {
            Alert,
            ErrorCollection
        }
        protected string m_sPrefix = "";
        public string FunctionPrefix
        {
            get
            {
                return m_sPrefix;
            }
            set
            {
                m_sPrefix = value;
            }
        }
        public string RemoveHTMLFunctionName
        {
            get
            {
                return m_sPrefix + "RemoveHTML";
            }
        }
        public string RemoveHTML()
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append(" function ").Append(m_sPrefix).Append("RemoveHTML(strText) ").Append(Environment.NewLine);
            sbJS.Append(" { ").Append(Environment.NewLine);
            sbJS.Append(" 	return strText.replace(/<[^>]*>/g, \"\"); ").Append(Environment.NewLine);
            sbJS.Append(" } ").Append(Environment.NewLine);
            return sbJS.ToString();
        }
        public string ToggleDivFunctionName
        {
            get
            {
                return m_sPrefix + "ToggleDiv";
            }
        }
        public string ToggleDiv()
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append("function ").Append(m_sPrefix).Append("ToggleDiv(sDiv, overrd) {" + Environment.NewLine);
            sbJS.Append("   var objcustom = document.getElementById(sDiv); " + Environment.NewLine);
            sbJS.Append("   var bOverRide = (overrd != null); " + Environment.NewLine);
            sbJS.Append("   if ((bOverRide && overrd) || (!bOverRide && objcustom.style.visibility == \'hidden\')) { " + Environment.NewLine);
            sbJS.Append("       objcustom.style.position = \'\'; " + Environment.NewLine);
            sbJS.Append("       objcustom.style.visibility = \'visible\';" + Environment.NewLine);
            sbJS.Append("   } else { " + Environment.NewLine);
            sbJS.Append("       objcustom.style.position = \'absolute\'; " + Environment.NewLine);
            sbJS.Append("       objcustom.style.visibility = \'hidden\';" + Environment.NewLine);
            sbJS.Append("   } " + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);
            return sbJS.ToString();
        }

        public string URLEncodeFunctionName
        {
            get
            {
                return m_sPrefix + "JSURLEncode";
            }
        }
        public string URLEncode()
        {

            StringBuilder sbJS = new StringBuilder();

            sbJS.Append(" function ").Append(m_sPrefix).Append("JSURLEncode (clearString) { ").Append(Environment.NewLine);
            sbJS.Append("   var output = \'\'; ").Append(Environment.NewLine);
            sbJS.Append("   var x = 0; ").Append(Environment.NewLine);
            sbJS.Append("   clearString = clearString.toString(); ").Append(Environment.NewLine);
            sbJS.Append("   var regex = /(^[a-zA-Z0-9_.]*)/; ").Append(Environment.NewLine);
            sbJS.Append("   while (x < clearString.length) { ").Append(Environment.NewLine);
            sbJS.Append("     var match = regex.exec(clearString.substr(x)); ").Append(Environment.NewLine);
            sbJS.Append("     if (match != null && match.length > 1 && match[1] != \'\') { ").Append(Environment.NewLine);
            sbJS.Append("     	output += match[1]; ").Append(Environment.NewLine);
            sbJS.Append("       x += match[1].length; ").Append(Environment.NewLine);
            sbJS.Append("     } else { ").Append(Environment.NewLine);
            sbJS.Append("       if (clearString[x] == \' \') ").Append(Environment.NewLine);
            sbJS.Append("         output += \'+\'; ").Append(Environment.NewLine);
            sbJS.Append("       else { ").Append(Environment.NewLine);
            sbJS.Append("         var charCode = clearString.charCodeAt(x); ").Append(Environment.NewLine);
            sbJS.Append("         var hexVal = charCode.toString(16); ").Append(Environment.NewLine);
            sbJS.Append("         output += \'%\' + ( hexVal.length < 2 ? \'0\' : \'\' ) + hexVal.toUpperCase(); ").Append(Environment.NewLine);
            sbJS.Append("       } ").Append(Environment.NewLine);
            sbJS.Append("       x++; ").Append(Environment.NewLine);
            sbJS.Append("     } ").Append(Environment.NewLine);
            sbJS.Append("   } ").Append(Environment.NewLine);
            sbJS.Append("   return output; ").Append(Environment.NewLine);
            sbJS.Append(" } ").Append(Environment.NewLine);

            return sbJS.ToString();

        }
        public string HasIllegalCharacters(ErrorType eErrorType)
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append("function ").Append(m_sPrefix).Append("HasIllegalChar(sElement, sErr) {" + Environment.NewLine);
            sbJS.Append("   var val = document.getElementById(sElement).value;" + Environment.NewLine);
            sbJS.Append("   if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
            sbJS.Append("   { " + Environment.NewLine);
            sbJS.Append("       sErr = sErr + \"(\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')\"; " + Environment.NewLine);
            if (eErrorType == ErrorType.Alert)
            {
                sbJS.Append("       alert(sErr);" + Environment.NewLine);
            }
            else if (eErrorType == ErrorType.ErrorCollection)
            {
                sbJS.Append("       ").Append(m_sPrefix).Append("AddError(sErr);" + Environment.NewLine);
            }
            sbJS.Append("       return true;" + Environment.NewLine);
            sbJS.Append("   }" + Environment.NewLine);
            sbJS.Append("   return false;" + Environment.NewLine);
            sbJS.Append("}" + Environment.NewLine);
            return sbJS.ToString();
        }
        #region CheckKeyValue
        public string CheckKeyValueName
        {
            get
            {
                return m_sPrefix + "CheckKeyValue";
            }
        }
        public string CheckKeyValue()
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append("function ").Append(m_sPrefix).Append("CheckKeyValue(item, keys) {").Append(Environment.NewLine);
            sbJS.Append("  var keyArray = keys.split(\',\');").Append(Environment.NewLine);
            sbJS.Append("  for (var i = 0; i < keyArray.length; i++) {").Append(Environment.NewLine);
            sbJS.Append("    if ((document.layers) || ((!document.all) && (document.getElementById))) {").Append(Environment.NewLine);
            sbJS.Append("      if (item.which == keyArray[i]) { return false; }").Append(Environment.NewLine);
            sbJS.Append("    } else {").Append(Environment.NewLine);
            sbJS.Append("      if (event.keyCode == keyArray[i]) { return false; }").Append(Environment.NewLine);
            sbJS.Append("    }").Append(Environment.NewLine);
            sbJS.Append("  }").Append(Environment.NewLine);
            sbJS.Append("}").Append(Environment.NewLine);
            return sbJS.ToString();
        }
        #endregion
        public string AddErrorFunctionName
        {
            get
            {
                return m_sPrefix + "AddError";
            }
        }
        public string AddError(string ErrorCollectionName)
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append(" function ").Append(m_sPrefix).Append("AddError(sErrText) { ").Append(Environment.NewLine);
            sbJS.Append("   var iNew = ").Append(ErrorCollectionName).Append(".length; ").Append(Environment.NewLine);
            sbJS.Append("   ").Append(ErrorCollectionName).Append("[iNew] = sErrText; ").Append(Environment.NewLine);
            sbJS.Append(" } ").Append(Environment.NewLine);
            return sbJS.ToString();
        }
        public string ShowErrorFunctionName
        {
            get
            {
                return m_sPrefix + "ShowError";
            }
        }
        public string ShowError(string ErrorCollectionName)
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append(" function ").Append(m_sPrefix).Append("ShowError(sValid, sNotValidBefore, sNotValidAfter) { ").Append(Environment.NewLine);
            sbJS.Append("   if (").Append(ErrorCollectionName).Append(".length > 0) { ").Append(Environment.NewLine);
            sbJS.Append("       if (sNotValidBefore != null && sNotValidBefore != \'\') { eval(sNotValidBefore); } ").Append(Environment.NewLine);
            sbJS.Append("       alert(").Append(ErrorCollectionName).Append(".join(\'\\n\')); ").Append(Environment.NewLine);
            sbJS.Append("       if (sNotValidAfter != null && sNotValidAfter != \'\') { eval(sNotValidAfter); } ").Append(Environment.NewLine);
            sbJS.Append("       ").Append(ResetErrorFunctionName).Append("();").Append(Environment.NewLine);
            sbJS.Append("   } else { ").Append(Environment.NewLine);
            sbJS.Append("       if (sValid != \'\') { eval(sValid); } ").Append(Environment.NewLine);
            sbJS.Append("   } ").Append(Environment.NewLine);
            sbJS.Append(" } ").Append(Environment.NewLine);
            return sbJS.ToString();
        }
        public string ResetErrorFunctionName
        {
            get
            {
                return m_sPrefix + "ResetError";
            }
        }
        public string ResetError(string ErrorCollectionName)
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append(" function ").Append(m_sPrefix).Append("ResetError() { ").Append(Environment.NewLine);
            sbJS.Append("   ").Append(ErrorCollectionName).Append(" = new Array(); ").Append(Environment.NewLine);
            sbJS.Append(" } ").Append(Environment.NewLine);
            return sbJS.ToString();
        }
        public string ResizeFrameFunctionName
        {
            get
            {
                return m_sPrefix + "ResizeFrame";
            }
        }
        public string ResizeFrame()
        {
            StringBuilder sbJS = new StringBuilder();
            sbJS.Append(" function ").Append(m_sPrefix).Append("ResizeFrame(val) { ").Append(Environment.NewLine);
            sbJS.Append("   if ((typeof(top.ResizeFrame) == \"function\") && top != self) { ").Append(Environment.NewLine);
            sbJS.Append("       top.ResizeFrame(val); ").Append(Environment.NewLine);
            sbJS.Append("   } ").Append(Environment.NewLine);
            sbJS.Append(" } ").Append(Environment.NewLine);
            return sbJS.ToString();
        }
    }

    public class workareatabs
    {


        private ArrayList m_aTabs = new ArrayList();
        private ArrayList m_aTabCode = new ArrayList();
        private bool m_bUseTabs = false;
        protected EkMessageHelper m_refMsg;
        protected string m_sPreface = "";
        protected bool m_bViewAsWizard = false;
        protected string m_sImgPath = "";
        protected string m_sCSSActive = "tab_actived";
        protected string m_sCSSDisabled = "tab_disabled";

        public workareatabs(EkMessageHelper Messages, string ImagePath)
        {
            m_refMsg = Messages;
            m_sImgPath = ImagePath;
        }
        public string DivPreface
        {
            get
            {
                return this.m_sPreface;
            }
            set
            {
                this.m_sPreface = value;
            }
        }
        public bool TabsOn
        {
            get
            {
                return this.m_bUseTabs;
            }
        }

        public void On()
        {
            this.m_bUseTabs = true;
        }

        public void Off()
        {
            this.m_bUseTabs = false;
        }

        public void ViewAsWizard()
        {
            this.m_bViewAsWizard = System.Convert.ToBoolean(!m_bViewAsWizard);
            m_sCSSActive = "wizardstep_actived";
            m_sCSSDisabled = "wizardstep_disabled";
        }

        public void RemoveAt(int index)
        {
            if (index > -1 && index < this.m_aTabs.Count)
            {
                m_aTabCode.RemoveAt(index);
                m_aTabs.RemoveAt(index);
            }
        }

        public void AddTabByString(string TabName, string TabIdentifier)
        {
            StringBuilder sbTab = new StringBuilder();
            string sClass = m_sCSSDisabled; // "tab_disabled"
            if (this.m_aTabs.Count == 0)
            {
                sClass = m_sCSSActive; // "tab_actived"
            }
            sbTab.Append("          <td class=\"").Append(sClass + "\" id=\"").Append(TabIdentifier).Append("\"" + (m_bViewAsWizard ? "" : " width=\"1%\"") + " nowrap onClick=\"ShowPane(\'").Append(TabIdentifier).Append("\');return false;\">").Append(Environment.NewLine);
            sbTab.Append("              <b>&nbsp;").Append(TabName).Append("&nbsp;</b>").Append(Environment.NewLine);
            sbTab.Append("          </td>").Append(Environment.NewLine);
            m_aTabCode.Add(sbTab.ToString());
            m_aTabs.Add(TabIdentifier);
        }

        public void SetActiveTab(int TabIndex)
        {
            if (m_aTabCode.Count >= (TabIndex + 1))
            {
                for (int i = 0; i <= (m_aTabCode.Count - 1); i++)
                {
                    if (i == TabIndex)
                    {
                        m_aTabCode[i] = Strings.Replace((string)m_aTabCode[i], "class=\"" + m_sCSSDisabled + "", "class=\"" + m_sCSSActive + "", 1, -1, 0);
                    }
                    else
                    {
                        m_aTabCode[i] = Strings.Replace((string)m_aTabCode[i], "class=\"" + m_sCSSActive + "", "class=\"" + m_sCSSDisabled + "", 1, -1, 0);
                    }
                }
            }
        }

        public void AddTabByString(string TabName, string TabIdentifier, string urlpath)
        {
            StringBuilder sbTab = new StringBuilder();
            string sClass = m_sCSSDisabled; // "tab_disabled"
            if (this.m_aTabs.Count == 0)
            {
                sClass = m_sCSSActive; // "tab_actived"
            }
            sbTab.Append("          <td class=\"").Append(sClass + "\" id=\"").Append(TabIdentifier).Append("\" width=\"1%\" nowrap onClick=\"window.location.href=\'" + urlpath.Replace("\'", "\\\'") + "\';\">").Append(Environment.NewLine);
            sbTab.Append("              <b>&nbsp;").Append(TabName).Append("&nbsp;</b>").Append(Environment.NewLine);
            sbTab.Append("          </td>").Append(Environment.NewLine);
            m_aTabCode.Add(sbTab.ToString());
            m_aTabs.Add(TabIdentifier);
        }

        public void AddTabByMessage(string TabMessage, string TabIdentifier)
        {
            AddTabByString(m_refMsg.GetMessage(TabMessage), TabIdentifier);
        }

        public string RenderTabs()
        {
            if (this.m_bUseTabs)
            {
                StringBuilder sbJS = new StringBuilder();
                StringBuilder sbTabs = new StringBuilder();

                sbJS.Append("<script type=\"text/javascript\">" + Environment.NewLine);
                sbJS.Append("		function IsBrowserIE() " + Environment.NewLine);
                sbJS.Append("		{" + Environment.NewLine);
                sbJS.Append("		    // document.all is an IE only property" + Environment.NewLine);
                sbJS.Append("		    return (document.all ? true : false);" + Environment.NewLine);
                sbJS.Append("		}" + Environment.NewLine);
                //sbJS.Append("		function ShowPane(tabID) " & Environment.NewLine)
                //sbJS.Append("		{" & Environment.NewLine)
                //sbJS.Append("			var arTab = new Array(")
                sbJS.Append("       bEnableTabs = true;").Append(Environment.NewLine);
                sbJS.Append("           function ShowPane(tabID, paneshift)  ").Append(Environment.NewLine);
                sbJS.Append("			{").Append(Environment.NewLine);
                sbJS.Append("			    if (typeof inPublishProcess == \'boolean\' && inPublishProcess == true){").Append(Environment.NewLine);
                sbJS.Append("                   return false;").Append(Environment.NewLine);
                sbJS.Append("               }").Append(Environment.NewLine);
                sbJS.Append("				if (false == bEnableTabs){").Append(Environment.NewLine);
                sbJS.Append("					return false;").Append(Environment.NewLine);
                sbJS.Append("				}").Append(Environment.NewLine);
                sbJS.Append("				").Append(Environment.NewLine);
                sbJS.Append("				// For Netscape/FireFox: Objects appear to get destroyed when \"display\" is set to \"none\" and re-created ").Append(Environment.NewLine);
                sbJS.Append("				// when \"display\" is set to \"block.\" Instead will use the appropriate style-sheet ").Append(Environment.NewLine);
                sbJS.Append("				// class to move the unselected items to a position where they are not visible.").Append(Environment.NewLine);
                sbJS.Append("				// For IE: If the ActiveX control is display=\"none\" programmatically rather than by user click,").Append(Environment.NewLine);
                sbJS.Append("				// the ActiveX control seems to uninitialize, for example, the DHTML Edit Control (DEC) is gone.").Append(Environment.NewLine);
                sbJS.Append("				var CurrentPaneIndex = -1; ").Append(Environment.NewLine);
                sbJS.Append("				var aryTabs = [");

                sbTabs.Append("<table height=\"20\" width=\"100%\" " + (m_bViewAsWizard ? "class=\"workareatabbar\" " : "") + ">").Append(Environment.NewLine).Append("     <tr>").Append(Environment.NewLine);
                if (m_bViewAsWizard)
                {
                    sbTabs.Append(" <td valign=\"center\" nowrap=\"nowrap\" width=\"5%\">&nbsp;Step <span id=\"currentStep\">1</span> of <span id=\"totalSteps\">").Append(m_aTabCode.Count.ToString()).Append("</span>&nbsp;&nbsp;&nbsp;&nbsp;</td> ").Append(Environment.NewLine);
                    sbTabs.Append(" <td nowrap=\"nowrap\"><table id=\"stepsTable\" cellspacing=\"0\" cellpadding=\"0\" border=\"1\"> ").Append(Environment.NewLine);
                    sbTabs.Append("     <tbody> ").Append(Environment.NewLine);
                    sbTabs.Append("       <tr> ").Append(Environment.NewLine);
                    for (int i = 0; i <= (m_aTabCode.Count - 1); i++)
                    {
                        sbJS.Append("\"" + this.m_aTabs[i] + "\"");
                        sbTabs.Append(m_aTabCode[i]);
                        if (!(i == (m_aTabCode.Count - 1)))
                        {
                            sbJS.Append(",");
                        }
                    }
                    //sbJS.Append("         <td id=""step1"">1</td> ").Append(Environment.NewLine)
                    sbTabs.Append("       </tr> ").Append(Environment.NewLine);
                    sbTabs.Append("     </tbody> ").Append(Environment.NewLine);
                    sbTabs.Append(" </table></td> ").Append(Environment.NewLine);
                    sbTabs.Append(" <td valign=\"center\" nowrap=\"nowrap\" width=\"10%\">&nbsp;&nbsp;&nbsp;&nbsp;<a id=\"btnBackStep\" title=\"Back\" onClick=\"ShowPane(null, -1); return false;\" href=\"#\"><img height=\"16\" alt=\"Back\" src=\"").Append(m_sImgPath).Append("btn_left_blue.gif\" width=\"16\" align=\"middle\" border=\"0\" />&nbsp;<span id=\"spn_back\">Begin</span></a>&nbsp;&nbsp;<a id=\"btnNextStep\" title=\"Next\" onClick=\"ShowPane(null, 1); return false;\" href=\"#\"><img height=\"16\" alt=\"Next\" src=\"").Append(m_sImgPath).Append("btn_right_blue.gif\" width=\"16\" align=\"middle\" border=\"0\" />&nbsp;<span id=\"spn_next\">Next</span></a>");
                    // sbTabs.Append("<a id=""btnDoneSteps"" title=""Done"" onClick=""javascript:oProgressSteps.done(); return false;"" href=""#""><img height=""16"" alt=""Done"" src=""").Append(m_sImgPath).Append("btn_square_blue.gif"" width=""16"" align=""middle"" border=""0"" />&nbsp;Done</a>&nbsp;&nbsp;<a id=""btnCancelSteps"" title=""Cancel"" onClick=""javascript:oProgressSteps.cancel(); return false;"" href=""#""><img height=""16"" alt=""Cancel"" src=""").Append(m_sImgPath).Append("btn_x_blue.gif"" width=""16"" align=""middle"" border=""0"" />&nbsp;Cancel</a> ")
                    sbTabs.Append("</td> ").Append(Environment.NewLine);
                    // sbTabs.Append(" <td valign=""center""><span id=""helpBtn5"">&nbsp;<a href=""#""><img id=""DeskTopHelp"" title=""Click here to get help "" onClick=""javascript:PopUpWindow('/WEBSRC/workarea/help/index.html?alias=NoCollectiontopiclist.xml', 'SitePreview', 600, 500, 1, 1);return false;"" src=""").Append(m_sImgPath).Append("menu/help.gif"" border=""0"" /></a>&nbsp; </span></td> ").Append(Environment.NewLine)
                    sbTabs.Append(" <td width=\"80%\">&nbsp;&nbsp;</td> ").Append(Environment.NewLine);
                }
                else
                {
                    for (int i = 0; i <= (m_aTabCode.Count - 1); i++)
                    {
                        sbJS.Append("\"" + this.m_aTabs[i] + "\"");
                        sbTabs.Append(m_aTabCode[i]);
                        if (i == (m_aTabCode.Count - 1))
                        {
                            sbTabs.Append("          <td class=\"tab_last\" width=\"91%\" nowrap>&nbsp;</td>").Append(Environment.NewLine);
                        }
                        else
                        {
                            sbJS.Append(",");
                            sbTabs.Append("          <td class=\"tab_spacer\" width=\"1%\" nowrap>&nbsp;</td>").Append(Environment.NewLine);
                        }
                    }
                }

                sbJS.Append("];").Append(Environment.NewLine);
                sbJS.Append(" 				if ( paneshift != null && (paneshift == 1 || paneshift == -1) ) { ").Append(Environment.NewLine);
                sbJS.Append(" 					 ").Append(Environment.NewLine);
                sbJS.Append(" 					for (var i = 0; i < aryTabs.length; i++)  ").Append(Environment.NewLine);
                sbJS.Append(" 					{ ").Append(Environment.NewLine);
                sbJS.Append(" 						objElem = document.getElementById(aryTabs[i]); ").Append(Environment.NewLine);
                sbJS.Append(" 						if ( objElem != null && objElem.className == \"wizardstep_actived\" ) { CurrentPaneIndex = i; tabID = aryTabs[i]; break; } ").Append(Environment.NewLine);
                sbJS.Append(" 					} ").Append(Environment.NewLine);
                sbJS.Append(" 					 ").Append(Environment.NewLine);
                sbJS.Append(" 					if ( aryTabs[CurrentPaneIndex + paneshift] != null ) { ").Append(Environment.NewLine);
                sbJS.Append(" 						tabID = aryTabs[CurrentPaneIndex + paneshift]; ").Append(Environment.NewLine);
                sbJS.Append(" 						if ( (CurrentPaneIndex + paneshift) == 0 ) { document.getElementById(\'spn_back\').innerHTML = \'Begin\'; } else { document.getElementById(\'spn_back\').innerHTML = \'Back\'; } ").Append(Environment.NewLine);
                sbJS.Append(" 						if ( (CurrentPaneIndex + paneshift) == (aryTabs.length - 1) ) { document.getElementById(\'spn_next\').innerHTML = \'Done\'; } ").Append(Environment.NewLine);
                sbJS.Append(" 					} ").Append(Environment.NewLine);
                sbJS.Append(" 					 ").Append(Environment.NewLine);
                sbJS.Append(" 				} ").Append(Environment.NewLine);
                sbJS.Append("				for (var i = 0; i < aryTabs.length; i++) ").Append(Environment.NewLine);
                sbJS.Append("				{").Append(Environment.NewLine);
                sbJS.Append("					SetPaneVisible(aryTabs[i], false);").Append(Environment.NewLine);
                sbJS.Append("					SetPaneVisible(aryTabs[i], (tabID == aryTabs[i]));").Append(Environment.NewLine);
                sbJS.Append("				    if ((tabID == aryTabs[i]) && document.getElementById(\'currentStep\') != null) { document.getElementById(\'currentStep\').innerHTML = (i + 1); }").Append(Environment.NewLine);
                sbJS.Append("				}").Append(Environment.NewLine);
                sbJS.Append("			}").Append(Environment.NewLine);
                sbJS.Append("			").Append(Environment.NewLine);
                sbJS.Append("			function SetPaneVisible(tabID, bVisible)").Append(Environment.NewLine);
                sbJS.Append("			{").Append(Environment.NewLine);
                sbJS.Append("				var objElem = null;").Append(Environment.NewLine);
                sbJS.Append("				objElem = document.getElementById(tabID);").Append(Environment.NewLine);
                sbJS.Append("				if (objElem != null) ").Append(Environment.NewLine);
                sbJS.Append("				{").Append(Environment.NewLine);
                sbJS.Append("					objElem.className = (bVisible ? \"").Append(m_sCSSActive).Append("\" : \"").Append(m_sCSSDisabled).Append("\");").Append(Environment.NewLine);
                sbJS.Append("				}").Append(Environment.NewLine);
                sbJS.Append("				objElem = document.getElementById(\"" + m_sPreface + "_\" + tabID);").Append(Environment.NewLine);
                sbJS.Append("				if (objElem != null) ").Append(Environment.NewLine);
                sbJS.Append("				{").Append(Environment.NewLine);
                //sbJS.Append("					// For Safari on the Mac (to fix Ephox Editor issues), ").Append(Environment.NewLine)
                //sbJS.Append("					// the actual class names are overridden in the code behind").Append(Environment.NewLine)
                //sbJS.Append("					// (uses special classes when Safari on the Mac is detected):").Append(Environment.NewLine)
                sbJS.Append("					objElem.className = (bVisible ? \"selected_editor\" : \"unselected_editor\");").Append(Environment.NewLine);
                sbJS.Append("				}").Append(Environment.NewLine);
                sbJS.Append("			}").Append(Environment.NewLine);
                sbJS.Append("</script>" + Environment.NewLine);

                sbTabs.Append("     </tr>").Append(Environment.NewLine + "</table>").Append(Environment.NewLine);
                return sbJS.ToString() + Environment.NewLine + sbTabs.ToString();
            }
            else
            {
                return "";
            }
        }
    }

    public class workareawizard
    {


        private bool m_bUseWizard = false;

        public bool WizardOn
        {
            get
            {
                return this.m_bUseWizard;
            }
        }

        public void On()
        {
            this.m_bUseWizard = true;
        }

        public void Off()
        {
            this.m_bUseWizard = false;
        }

    }

    public class workareautil
    {


        public workareautil()
        {

        }

    }

    public class workareaajax
    {


        private string m_sAppPath = "";

        public workareaajax(string Apppath)
        {
            m_sAppPath = Apppath;
        }

        public string URLQuery = "";
        public string ResponseJS = "";
        public string FunctionName = "";

        public string Render()
        {
            System.Text.StringBuilder sbAEJS = new System.Text.StringBuilder();
            sbAEJS.Append("var req;").Append(Environment.NewLine);
            sbAEJS.Append("var bexists;").Append(Environment.NewLine);
            sbAEJS.Append("function loadXMLDoc(url) ").Append(Environment.NewLine);
            sbAEJS.Append("{").Append(Environment.NewLine);
            sbAEJS.Append("    // branch for native XMLHttpRequest object").Append(Environment.NewLine);
            sbAEJS.Append("    if (window.XMLHttpRequest) {").Append(Environment.NewLine);
            sbAEJS.Append("        req = new XMLHttpRequest();").Append(Environment.NewLine);
            sbAEJS.Append("        req.onreadystatechange = processReqChange;").Append(Environment.NewLine);
            sbAEJS.Append("        req.open(\"GET\", url, true);").Append(Environment.NewLine);
            sbAEJS.Append("        req.send(null);").Append(Environment.NewLine);
            sbAEJS.Append("    // branch for IE/Windows ActiveX version").Append(Environment.NewLine);
            sbAEJS.Append("    } else if (window.ActiveXObject) {").Append(Environment.NewLine);
            sbAEJS.Append("        req = new ActiveXObject(\"Microsoft.XMLHTTP\");").Append(Environment.NewLine);
            sbAEJS.Append("        if (req) {").Append(Environment.NewLine);
            sbAEJS.Append("            req.onreadystatechange = processReqChange;").Append(Environment.NewLine);
            sbAEJS.Append("            req.open(\"GET\", url, true);").Append(Environment.NewLine);
            sbAEJS.Append("            req.send();").Append(Environment.NewLine);
            sbAEJS.Append("        }").Append(Environment.NewLine);
            sbAEJS.Append("    }").Append(Environment.NewLine);
            sbAEJS.Append("}").Append(Environment.NewLine);
            sbAEJS.Append("function processReqChange() ").Append(Environment.NewLine);
            sbAEJS.Append("{").Append(Environment.NewLine);
            sbAEJS.Append("    // only if req shows \"complete\"").Append(Environment.NewLine);
            sbAEJS.Append("    if (req.readyState == 4) {").Append(Environment.NewLine);
            sbAEJS.Append("        // only if \"OK\"").Append(Environment.NewLine);
            sbAEJS.Append("        if (req.status == 200) {").Append(Environment.NewLine);
            sbAEJS.Append("            // ...processing statements go here...").Append(Environment.NewLine);
            sbAEJS.Append("      response  = req.responseXML.documentElement;").Append(Environment.NewLine);
            sbAEJS.Append("").Append(Environment.NewLine);
            sbAEJS.Append("      method    = response.getElementsByTagName(\'method\')[0].firstChild.data;").Append(Environment.NewLine);
            sbAEJS.Append("").Append(Environment.NewLine);
            sbAEJS.Append("      result    = response.getElementsByTagName(\'result\')[0].firstChild.data;").Append(Environment.NewLine);
            sbAEJS.Append("").Append(Environment.NewLine);
            sbAEJS.Append("      eval(method + \'(\\\'\\\', result);\');").Append(Environment.NewLine);
            sbAEJS.Append("        } else {").Append(Environment.NewLine);
            sbAEJS.Append("            alert(\"There was a problem retrieving the XML data:\\n\" + req.statusText);").Append(Environment.NewLine);
            sbAEJS.Append("        }").Append(Environment.NewLine);
            sbAEJS.Append("    }").Append(Environment.NewLine);
            sbAEJS.Append("}").Append(Environment.NewLine);
            sbAEJS.Append("").Append(Environment.NewLine);
            sbAEJS.Append("function ").Append(FunctionName).Append("(input, response)").Append(Environment.NewLine);
            sbAEJS.Append("{").Append(Environment.NewLine);
            sbAEJS.Append("  if (response != \'\'){ ").Append(Environment.NewLine);
            sbAEJS.Append("    // Response mode").Append(Environment.NewLine);
            sbAEJS.Append(ResponseJS).Append(Environment.NewLine);
            sbAEJS.Append("  }else{").Append(Environment.NewLine);
            sbAEJS.Append("    // Input mode").Append(Environment.NewLine);
            sbAEJS.Append("    url = \'").Append(m_sAppPath).Append("AJAXbase.aspx?").Append(URLQuery).Append("\';").Append(Environment.NewLine);
            sbAEJS.Append("    loadXMLDoc(url);").Append(Environment.NewLine);
            sbAEJS.Append("  }").Append(Environment.NewLine);
            sbAEJS.Append("").Append(Environment.NewLine);
            sbAEJS.Append("}").Append(Environment.NewLine);
            return sbAEJS.ToString();
        }
    }
}
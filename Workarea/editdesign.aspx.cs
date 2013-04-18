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
using Ektron.Cms.Content;
using Ektron;
using Ektron.Cms.Framework.UI;

	public partial class editdesign : System.Web.UI.Page
	{
			
		#region Member Variables
		
		protected bool _IsBrowserIE = false;
		protected string _AppImgPath = "";
		protected string _AppPath = "";
		protected string _AppeWebPath = "";
		protected string _AppName = "";
		protected long _CurrentUserId = 0;
		protected int _ContentLanguage = 0;
		protected StyleHelper _StyleHelper = new StyleHelper();
		protected EkMessageHelper _MessageHelper;
		protected ContentAPI _ContentApi = new ContentAPI();
		protected string _LocaleFileString = "";
		protected string _PageAction = "";
		protected long _Id = 0;
		protected string _LocaleFileNumber = "1033";
		protected string _AppLocaleString = "";
		protected string _EditAction;
		protected LanguageData _LanguageData;
		protected string _UnPackDisplayXslt = "";
		protected string _SitePath = "";
		protected bool _UploadPrivs;
		protected string _Var1;
		protected string _Var2;
		protected XmlConfigData _XmlConfigData;
		protected EkContent _EkContent;
		protected object _UserRights;
		protected object _MetadataNumber;
		protected object _PreviousState;
		protected object _MetaComplete;
		protected int _MaxContLength;
		protected string _Path;
		protected object cLibPerms;
		protected long _XmlId;
		protected Collection _StyleSheets;
		protected int _StyleCount;
		protected int _ii;
		protected string _KeyName;
		protected string _StyleSheet;
		protected object[] _st;
		protected int _Segment = 0;
		protected long _ContLoop;
		protected int _MyStart;
        protected int _MyEnd;
		protected int _MyLength;
		protected string _XmlStartTag;
		protected string _XmlEndTag;
		protected string _IndexCms;
		protected bool _IsProduct = false;
		protected string _XmlPage = "xml_config.aspx";
		protected string _ProductPage = "commerce/producttypes.aspx";
		
		private string _SelectedEditControl;
		private ContentDesignerWithValidator _ContentDesigner;
		
		#endregion
		
		#region Events
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
		    _ContentDesigner = (ContentDesignerWithValidator) LoadControl("controls/Editor/ContentDesignerWithValidator.ascx");
			phEditContent.Controls.Add(_ContentDesigner);
			_ContentDesigner.Visible = false;
			_ContentDesigner.ID = "content_html";
			_ContentDesigner.Toolbars = Ektron.ContentDesignerWithValidator.Configuration.DataDesigner;
			_ContentDesigner.Height = new Unit(635, UnitType.Pixel);
			_ContentDesigner.AllowFonts = true;
			_ContentDesigner.ShowPreviewMode = true;
		}
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
            
            PermissionData security_lib_data;
			string strBrowserCode = "";
			if (Request.Browser.Type.IndexOf("IE") != -1)
			{
				_IsBrowserIE = true;
			}
			if (!String.IsNullOrEmpty(Request.QueryString["type"]))
			{
				_IsProduct = System.Convert.ToBoolean(Request.QueryString["type"].ToLower() == "product");
			}
			_MessageHelper = _ContentApi.EkMsgRef;
			_EkContent = _ContentApi.EkContentRef;
			if (_EkContent.IsAllowed(0, 0, "users", "IsLoggedIn", 0) == false || _EkContent.IsAllowed(0, 0, "users", "IsAdmin", 0) == false)
			{
				if ((_IsProduct && ! _EkContent.IsARoleMember(Convert.ToInt64(EkEnumeration.CmsRoleIds.CommerceAdmin), _EkContent.RequestInformation.UserId, false)) || (! _IsProduct && ! _EkContent.IsARoleMember(Convert.ToInt64(EkEnumeration.CmsRoleIds.AdminXmlConfig), _EkContent.RequestInformation.UserId, false)))
					{
					Utilities.ShowError(_MessageHelper.GetMessage("com: user does not have permission"));
				}
			}

            RegisterResources();
			StyleSheetJS.Text = _StyleHelper.GetClientScript();
				
			_CurrentUserId = _ContentApi.UserId;
			_ContentLanguage = _ContentApi.ContentLanguage;
			_AppImgPath = _ContentApi.AppImgPath;
			_AppImgPath = _ContentApi.AppPath;
			_SitePath = _ContentApi.SitePath;
			_AppeWebPath = _ContentApi.ApplicationPath + _ContentApi.AppeWebPath;
			_AppName = _ContentApi.AppName;
			_Id = Convert.ToInt64(Request.QueryString["id"]);
			_PageAction = Request.QueryString["action"];
			_Var1 = Request.ServerVariables["SERVER_NAME"];
			_LanguageData = (new SiteAPI()).GetLanguageById(_ContentLanguage);
			_Var2 = _EkContent.GetEditorVariablev2_0(_Id, _PageAction);
			if (_LanguageData != null)
			{
				_LocaleFileNumber = Convert.ToString(_LanguageData.Id);
				strBrowserCode = _LanguageData.BrowserCode;
			}
			
			Page.Title = _AppName + " " + _MessageHelper.GetMessage("edit content page title") + " \"" + Ektron.Cms.CommonApi.GetEcmCookie()["username"] + "\"";
			_LocaleFileString = GetLocaleFileString(_LocaleFileNumber);
			
			SiteAPI refSiteApi = new SiteAPI();
			SettingsData settings_data;
			settings_data = refSiteApi.GetSiteVariables(refSiteApi.UserId);
			_AppLocaleString = GetLocaleFileString(settings_data.Language);
			
			_SelectedEditControl = "eWebEditPro";
			bool bIsMac = false;
			try
			{
				if (Request.Browser.Platform.IndexOf("Win") == -1)
				{
					bIsMac = true;
				}
				if (bIsMac)
				{
					_SelectedEditControl = "ContentDesigner";
				}
				else
				{
					if (ConfigurationManager.AppSettings["ek_DataDesignControl"] != null)
					{
						_SelectedEditControl = (string) (ConfigurationManager.AppSettings["ek_DataDesignControl"].ToString());
					}
				}
			}
			catch (Exception)
			{
				_SelectedEditControl = "ContentDesigner";
			}
			
			if (_SelectedEditControl == "eWebEditPro")
			{
				EditorJS.Text = Utilities.EditorScripts(_Var2, _AppeWebPath, strBrowserCode);
			}
			else
			{
				EditorJS.Text = "";
			}
			
			security_lib_data = _ContentApi.LoadPermissions(_Id, "folder", 0);
			_UploadPrivs = security_lib_data.CanAddToFileLib || security_lib_data.CanAddToImageLib;
			
			_EditAction = Request.QueryString["editaction"]; 
			if (!(Page.IsPostBack))
			{
				if (_EditAction == "cancel")
				{
					//do nothing
				}
				else
				{
					DisplayControl();
				}
				
			}
			else
			{
				UpdateEditDesign();
			}
			jsAppeWebPath.Text = _AppeWebPath;
			jsAppLocaleString.Text = _AppLocaleString;
			jsSitePath.Text = _SitePath;
			jsPath.Text = Strings.Replace(_Path, "\\", "\\\\", 1, -1, 0);
			jsTitle.Text = _MessageHelper.GetMessage("generic title label");
			jsXmlStyle.Text = _MessageHelper.GetMessage("lbl select style for xml design");
			jsInputTmpStyleSheet.Text = "<input type=\"text\" maxlength=\"255\" size=\"45\" value=\"" + _StyleSheet + "\" name=\"stylesheet\" id=\"stylesheet\" onchange=\"SetStyleSheetFromInput();\" />";
			jsTmpStyleSheet.Text = _StyleSheet;
			jshdnContentLanguage.Text = "<input type=\"hidden\" name=\"content_language\" value=\"" + _ContentLanguage + "\"/>";
			jshdnMaxContLength.Text = "<input type=\"hidden\" name=\"maxcontentsize\" value=\"" + _MaxContLength + "\"/>";
			jshdnXml_id.Text = "<input type=\"hidden\" name=\"xml_collection_id\" value=\"" + _XmlId + "\"/>";
			jshdnIndex_cms.Text = "<input type=\"hidden\" name=\"index_cms\" value=\"" + _IndexCms + "\"/>";
			jshdniSegment.Text = "<input type=\"hidden\" name=\"numberoffields\" value=\"" + _Segment + "\"/>";
			jsMaxContLength.Text = _MaxContLength.ToString();
			jsEditorMsg.Text = _MessageHelper.GetMessage("lbl wait editor not loaded");
			sContentInvalid.Text = _MessageHelper.GetMessage("msg content invalid");
			sDesignIncompatible.Text = _MessageHelper.GetMessage("msg design incompatible");
			sContinue.Text = _MessageHelper.GetMessage("msg wish to continue");
            sWarnOnUnload.Text = _MessageHelper.GetMessage("js: alert confirm close no save");
		}
		
		#endregion
		
		#region Helpers
		
		private string GetLocaleFileString(string localeFileNumber)
		{
			string LocaleFileString;
			if (localeFileNumber == "" || int.Parse(localeFileNumber) == 1)
			{
				LocaleFileString = "0000";
			}
			else
			{
				LocaleFileString = new string('0', 4 - Conversion.Hex(localeFileNumber).Length);
				LocaleFileString = LocaleFileString + Conversion.Hex(localeFileNumber);
				if (! System.IO.File.Exists(Server.MapPath(_AppeWebPath + "locale" + LocaleFileString + "b.xml")))
				{
					LocaleFileString = "0000";
				}
			}
			return LocaleFileString.ToString();
		}
		
		private void DisplayControl()
		{
			if (_IsProduct)
			{
				EditProductDesignToolBar();
			}
			else
			{
				EditDesignToolBar();
			}
			
			
			_XmlId = _Id;
			
			//stop
			_StyleSheets = (Collection)_EkContent.GetAllStyleSheets();
			_StyleCount = Convert.ToInt32(_StyleSheets["NumberOfStyleSheets"]);
			_st = new object[_StyleCount + 1];
			for (_ii = 1; _ii <= _StyleCount; _ii++)
			{
				_KeyName = "StyleSheet_" + _ii;
				_st[_ii] = _StyleSheets[_KeyName];
			}
			
			_XmlConfigData = _ContentApi.GetXmlConfiguration(_Id);
			
			
			PermissionData cPer;
			object content_html;
			object content_title;
			object content_stylesheet;
			cPer = _ContentApi.LoadPermissions(_Id, "content", 0);
			
			content_html = _XmlConfigData.PackageXslt;
			_StyleSheet = _XmlConfigData.DesignStyleSheet;
			content_title = "Design Mode";
			content_stylesheet = "";
			
			TD_ColTitle.InnerHtml = _XmlConfigData.Title;
			
			if (_StyleCount != 0)
			{
				stylesheetoptions.Items.Add(new ListItem("--" + "-- Select a style sheet --" + "--", "ignore"));
				
				for (_ii = 1; _ii <= _StyleCount; _ii++)
				{
					stylesheetoptions.Items.Add(new ListItem(_st[_ii].ToString(), _st[_ii].ToString()));
				}
				if (_StyleSheet != "")
				{
					stylesheetoptions.SelectedValue = _StyleSheet;
				}
				
				stylesheetoptions.Attributes.Add("onchange", "SetStyleSheet();");
			}
			SettingsData SiteVars;
			
			SiteVars = (new SiteAPI()).GetSiteVariables(_CurrentUserId);
			_MaxContLength = Convert.ToInt32(SiteVars.MaxContentSize);		
			_XmlStartTag = "<index_cms>";
			_XmlEndTag = "</index_cms>";
			_MyStart = content_html.ToString().IndexOf(_XmlStartTag.ToString()) + 1;
			_IndexCms = "";
			if (_MyStart > 0)
			{
				_MyEnd = content_html.ToString().IndexOf(_XmlEndTag.ToString()) + 1;
				if (_MyEnd > 0)
				{
					_MyStart = _MyStart + Strings.Len(_XmlStartTag);
					_MyLength = _MyEnd - _MyStart;
					_IndexCms = Strings.Mid(content_html.ToString(), System.Convert.ToInt32(_MyStart), System.Convert.ToInt32(_MyLength));
					_IndexCms = EkFunctions.HtmlEncode(_IndexCms.ToString());
				}
			}
			
			_ContLoop = 1;
			if (content_html.ToString().Length  > _MaxContLength)
			{
				_Var1 = System.Convert.ToString(Strings.Len(content_html));
			}
			else
			{
				_Var1 = _MaxContLength.ToString();
			}
			while (_ContLoop <= Convert.ToInt64(_Var1))
			{
				hiddenfields.Text += "<input type=\"hidden\" name=\"hiddencontent" + (_Segment + 1) + "\" value=\"\">";
				_ContLoop = _ContLoop + 65000;
				_Segment = System.Convert.ToInt32(_Segment + 1);
			}
			loadSegmentsFn.Text = GetLoadSegmentsScript();
			
			if (_UploadPrivs == false)
			{
				DisabledUpload.Text = "DisableUpload(\'content_html\');";
			}
			if ("ContentDesigner" == _SelectedEditControl)
			{
				_ContentDesigner.Visible = true;
				_ContentDesigner.Width = new Unit(100, UnitType.Percentage);
				_ContentDesigner.Height = new Unit(635, UnitType.Pixel);
				string smartFormDesign = _ContentApi.TransformXsltPackage(content_html.ToString(), Server.MapPath((string) (_ContentDesigner.ScriptLocation + "unpackageDesign.xslt")), true);
				_ContentDesigner.Content = smartFormDesign;
				// Selecting a CSS stylesheet is only for eWebEditPro
				SelectStyleCaption.Visible = false;
				SelectStyleControl.Visible = false;
			}
			else
			{
				Ektron.Cms.Controls.HtmlEditor ctlEditor = new Ektron.Cms.Controls.HtmlEditor();
				phEditContent.Controls.Remove(_ContentDesigner);
				ctlEditor.WorkareaMode(2); // We are designing in the workarea
				ctlEditor.ID = "content_html";
				ctlEditor.Width = new Unit(100, UnitType.Percentage);
				ctlEditor.Height = new Unit(90, UnitType.Percentage);
				ctlEditor.Path = _AppeWebPath;
				ctlEditor.MaxContentSize = System.Convert.ToInt32(_MaxContLength);
				ctlEditor.Text = content_html.ToString();
				phEditContent.Controls.Add(ctlEditor);
			}
		}
		
		private string GetLoadSegmentsScript()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("function loadSegments() {" + "\r\n");
			result.Append("}");
			return (result.ToString());
		}
		
		private void UpdateEditDesign()
		{
			string displayXslt;
			Collection cCont;
			object strcontent;
			int icontloop;
			object index_cms;
			long ContentId;
			Ektron.Cms.Common.EkEnumeration.XmlConfigType configType = Ektron.Cms.Common.EkEnumeration.XmlConfigType.Content;
			
			configType = _EkContent.GetXMLConfigType(_Id);
			
			UnPackDisplay();
			cCont = new Collection();
			strcontent = "";
			if ("ContentDesigner" == _SelectedEditControl)
			{
				string strDesign = (string) _ContentDesigner.Content;
				string strSchema = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath((string) (_ContentDesigner.ScriptLocation + "DesignToSchema.xslt")), true);
				string strFieldList = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath((string) (_ContentDesigner.ScriptLocation + "DesignToFieldList.xslt")), true);
				string strViewEntryXslt = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath((string) (_ContentDesigner.ScriptLocation + "DesignToEntryXSLT.xslt")), true);
				
				System.Xml.Xsl.XsltArgumentList objXsltArgs = new System.Xml.Xsl.XsltArgumentList();
				objXsltArgs.AddParam("srcPath", "", _ContentDesigner.ScriptLocation);
				
				string strViewXslt = _ContentApi.XSLTransform("<root>" + strDesign + "<ektdesignpackage_list>" + strFieldList + "</ektdesignpackage_list></root>", Server.MapPath((string) (_ContentDesigner.ScriptLocation + "DesignToViewXSLT.xslt")), true, false, objXsltArgs, false);
				string strInitDoc = _ContentApi.TransformXsltPackage(strDesign, Server.MapPath((string) (_ContentDesigner.ScriptLocation + "PresentationToData.xslt")), true);
				StringBuilder sbPackage = new StringBuilder();
				sbPackage.Append("<ektdesignpackage_forms><ektdesignpackage_form><ektdesignpackage_designs><ektdesignpackage_design>");
				sbPackage.Append(strDesign);
				sbPackage.Append("</ektdesignpackage_design></ektdesignpackage_designs><ektdesignpackage_schemas><ektdesignpackage_schema>");
				sbPackage.Append(strSchema);
				sbPackage.Append("</ektdesignpackage_schema></ektdesignpackage_schemas><ektdesignpackage_lists><ektdesignpackage_list>");
				sbPackage.Append(strFieldList);
				sbPackage.Append("</ektdesignpackage_list></ektdesignpackage_lists><ektdesignpackage_views><ektdesignpackage_view>");
				sbPackage.Append(strViewEntryXslt);
				sbPackage.Append("</ektdesignpackage_view><ektdesignpackage_view>");
				sbPackage.Append(strViewXslt);
				sbPackage.Append("</ektdesignpackage_view></ektdesignpackage_views><ektdesignpackage_initialDocuments><ektdesignpackage_initialDocument>");
				sbPackage.Append(strInitDoc);
				sbPackage.Append("</ektdesignpackage_initialDocument></ektdesignpackage_initialDocuments></ektdesignpackage_form></ektdesignpackage_forms>");
				strcontent = sbPackage.ToString();
			}
			else
			{
				icontloop = 1;
				while (Strings.Len(Request.Form["hiddencontent" + icontloop]) > 0)
				{
					strcontent = strcontent + Request.Form["hiddencontent" + icontloop];
					icontloop++;
				}
			}
			icontloop = 1;
			cCont.Add(strcontent, "Package", null, null);
			if (Request.Form["stylesheet"] != "")
			{
				cCont.Add(Request.Form["stylesheet"], "DesignStylesheet", null, null);
			}
			else
			{
				cCont.Add("", "DesignStylesheet", null, null);
			}
			
			cCont.Add(Request.Form["xml_collection_id"], "XmlCollectionID", null, null);
			
			displayXslt = _ContentApi.TransformXsltPackage(strcontent.ToString(), _UnPackDisplayXslt, false);
			cCont.Add(displayXslt, "displayXslt", null, null);
			
			
			_EkContent.UpdatexmlCollectionPackage(cCont, _CurrentUserId);
			ContentId = Convert.ToInt64(Request.Form["xml_collection_id"]);
			
			index_cms = Request.Form["index_cms"];
			//now that index_cms is server side .net can play with it
			Server.Transfer((string) ("xml_index_select.aspx?action=servertransfer" + (configType == Ektron.Cms.Common.EkEnumeration.XmlConfigType.Product ? "&type=product" : "")), true);
		}
		
		private void EditProductDesignToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl product type xml config"));
			result.Append("<table><tr>");
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", _ProductPage + "?LangType=" + _ContentLanguage + "&page=" + _ProductPage + "&id=" + Request.QueryString["id"] + "&action=viewproducttype", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "OnClick=\"javascript:return SetAction(\'cancel\');\"", StyleHelper.BackButtonCssClass, true));
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt Save XML Configuration"), _MessageHelper.GetMessage("btn update"), "OnClick=\"javascript:return SetAction(\'update\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _StyleHelper.GetHelpButton("EditProductTypeXML", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		
		private void EditDesignToolBar()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			divTitleBar.InnerHtml = _StyleHelper.GetTitleBar(_MessageHelper.GetMessage("lbl XML Configuration"));
			result.Append("<table><tr>");
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/back.png", _XmlPage + "?LangType=" + _ContentLanguage + "&page=" + _XmlPage + "&id=" + Request.QueryString["id"] + "&action=ViewXmlConfiguration", _MessageHelper.GetMessage("alt back button text"), _MessageHelper.GetMessage("btn back"), "OnClick=\"javascript:return SetAction(\'cancel\');\"", StyleHelper.BackButtonCssClass, true));
			result.Append(_StyleHelper.GetButtonEventsWCaption(_AppPath + "images/UI/Icons/save.png", "#", _MessageHelper.GetMessage("alt Save XML Configuration"), _MessageHelper.GetMessage("btn update"), "OnClick=\"javascript:return SetAction(\'update\');\"", StyleHelper.SaveButtonCssClass, true));
			result.Append(StyleHelper.ActionBarDivider);
			result.Append("<td>" + _StyleHelper.GetHelpButton("EditPackage", "") + "</td>");
			result.Append("</tr></table>");
			divToolBar.InnerHtml = result.ToString();
			result = null;
		}
		
		private void UnPackDisplay()
		{
			_UnPackDisplayXslt = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" + "<xsl:output method=\"xml\" version=\"1.0\" encoding=\"UTF-8\" indent=\"yes\"/>" + "<xsl:template match=\"/\">" + "<xsl:choose>" + "<xsl:when test=\"ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_views/ektdesignpackage_view[2]\">" + "<xsl:copy-of select=\"ektdesignpackage_forms/ektdesignpackage_form		[1]/ektdesignpackage_views/ektdesignpackage_view[2]/node()\"/>" + "</xsl:when>" + "	<xsl:otherwise>" + "<xsl:copy-of select=\"ektdesignpackage_forms/ektdesignpackage_form		[1]/ektdesignpackage_views/ektdesignpackage_view[1]/node()\"/>" + "</xsl:otherwise>" + "</xsl:choose>" + "</xsl:template>" + "</xsl:stylesheet>";
		}
		
		#endregion
		
		#region CSS, JS
		
		private void RegisterResources()
		{
            Packages.EktronCoreJS.Register(this);
            Packages.Ektron.Workarea.Core.Register(this);

			Ektron.Cms.API.JS.RegisterJS(this, _EkContent.RequestInformation.ApplicationPath + "java/empjsfunc.js", "EktronEmpJSFuncJS");
			Ektron.Cms.API.JS.RegisterJS(this, _EkContent.RequestInformation.ApplicationPath + "java/datejsfunc.js", "EktronDateJSFuncJS");
			Ektron.Cms.API.JS.RegisterJS(this, _EkContent.RequestInformation.ApplicationPath + "java/toolbar_roll.js", "EktronToolbarRollJS");
            
            Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
		}
		
		#endregion
	}
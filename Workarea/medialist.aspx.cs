using System;
using System.Web.UI;
using System.Xml.Serialization;
using Ektron.Cms;
using Ektron.Cms.Common;
using Microsoft.VisualBasic;
using Ektron.Cms.Framework.UI;
using System.Collections.Generic;
using System.Text;

public partial class medialist : System.Web.UI.Page
	{	
		#region  Web Form Designer Generated Code
		
		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			
		}
		
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			//CODEGEN: This method call is required by the Web Form Designer
			//Do not modify it using the code editor.
			InitializeComponent();
		}
		protected EkMessageHelper m_refMsg;
		protected string actionType;
		//Protected currentUserID
		protected string scope;
		protected string AppName;
		protected string sitePath;
		protected string iOrderBy;
		protected string sEditor;
		protected Collection cAllFolders;
		protected string DEntryLink;
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected Ektron.Cms.Content.EkContent m_refContent;
		protected string m_action;
		protected bool m_bAjaxTree = false;
		protected string m_AutoNavFolderIDs = "0";
		
		protected string m_strEnhancedMetaSelect = "";
		protected string m_strMetadataFormTagId = "";
		protected string m_strSeparator = "";
		protected string m_retField = "";
		protected string m_showThumb = "";
		protected string m_selectids = "";
		protected string m_selecttitles = "";
		#endregion
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			CommonApi m_refApi = new CommonApi();

            if (!string.IsNullOrEmpty(Request.QueryString["enhancedmetaselect"]))
			{
				m_strEnhancedMetaSelect = (string) (EkFunctions.HtmlEncode(Request.QueryString["enhancedmetaselect"]).Replace("\'", "&#39;"));
			}
            if (!string.IsNullOrEmpty(Request.QueryString["metadataformtagid"]))
			{
				m_strMetadataFormTagId = (string) (EkFunctions.HtmlEncode(Request.QueryString["metadataformtagid"]).Replace("\'", "&#39;"));
			}
            if (!string.IsNullOrEmpty(Request.QueryString["separator"]))
			{
				m_strSeparator = (string) (EkFunctions.HtmlEncode(Request.QueryString["separator"]).Replace("\'", "&#39;"));
			}

            if (!string.IsNullOrEmpty(Request.QueryString["selectids"]))
			{
				m_selectids = (string) (EkFunctions.HtmlEncode(Request.QueryString["selectids"]).Replace("\'", "&#39;"));
			}

            if (!string.IsNullOrEmpty(Request.QueryString["selecttitles"]))
			{
				m_selecttitles = (string) (EkFunctions.HtmlEncode(Request.QueryString["selecttitles"]).Replace("\'", "&#39;"));
			}
			
			if (m_refApi.TreeModel == 1)
			{
				m_bAjaxTree = true;
			}
			
			m_refMsg = m_refContentApi.EkMsgRef;
			if (! Utilities.ValidateUserLogin())
			{
				return;
			}
			Page.Title = AppName + " " + m_refMsg.GetMessage("library page html title") + " " + Ektron.Cms.CommonApi.GetEcmCookie()["username"];
			scope = EkFunctions.HtmlEncode(Request.QueryString["scope"]).ToLower();
			sEditor = EkFunctions.HtmlEncode(Request.QueryString["EditorName"]).Trim();
			DEntryLink = EkFunctions.HtmlEncode(Request.QueryString["dentrylink"]);
			actionType = EkFunctions.HtmlEncode(Request.QueryString["actionType"]);
			if ((Request.QueryString["retfield"] != null)&& Request.QueryString["retfield"] != "")
			{
				m_retField = (string) ("&retfield=" + EkFunctions.HtmlEncode(Request.QueryString["retfield"]));
			}
			if ((Request.QueryString["showthumb"] != null)&& Request.QueryString["showthumb"] != "")
			{
				m_showThumb = (string) ("&showthumb=" + EkFunctions.HtmlEncode(Request.QueryString["showthumb"]));
			}
			m_refContent = m_refContentApi.EkContentRef;
            if (!string.IsNullOrEmpty(Request.QueryString["iOrderBy"]))
			{
				iOrderBy = EkFunctions.HtmlEncode(Request.QueryString["iOrderBy"]);
			}
			else
			{
				iOrderBy = "Title";
			}
            if (!string.IsNullOrEmpty(Request.QueryString["action"]))
			{
				if (Request.QueryString["action"] != "")
				{
					m_action = EkFunctions.HtmlEncode(Request.QueryString["action"].ToLower());
				}
			}
            if (!string.IsNullOrEmpty(Request.QueryString["autonavfolder"]))
			{
				if (Request.QueryString["autonavfolder"] != "" && Convert.ToInt64(Request.QueryString["autonavfolder"].ToString()) > 0)
				{
					m_AutoNavFolderIDs = (string) ((m_refContentApi.EkContentRef).GetFolderParentFolderIdRecursive(Convert.ToInt64(Request.QueryString["autonavfolder"].ToString())));
				}
			}
			if (m_bAjaxTree)
			{
				if (IsInCallbackMode())
				{
					return;
				}
			}
			else
			{
				ltr_ClientScript.Text = GetClientScript();
				Msg.Text = GetString();
			}

            RegisterResources();
		}

        protected void RegisterResources()
        {
            Package mediaListUI = new Package();
            mediaListUI.Components.Add(Packages.EktronCoreJS);
            if (m_bAjaxTree)
            {
                uxBodyTag.Attributes.Add("onclick", "ContextMenuUtil.hide()");
                uxBodyTag.Attributes.Add("onload", "initHdnVals();Main.start();displayTree();");

                mediaListUI.Components.Add(Css.Create("Tree/css/com.ektron.ui.contextmenu.css"));
                mediaListUI.Components.Add(Css.Create("Tree/css/com.ektron.ui.tree.css"));
                mediaListUI.Components.Add(Packages.Ektron.Workarea.Core);
                if ((!string.IsNullOrEmpty(Request.QueryString["scope"])) && Request.QueryString["scope"] == "catalogfolder")
                {
                    StringBuilder initCode = new StringBuilder();
                    initCode.Append(@"$ektron(document).bind(""CMSAPIAjaxComplete"", function(){" + Environment.NewLine);
                    initCode.Append(@"var doNothingFolders = $ektron(""ul.ektree li img[src!='images/ui/icons/tree/folderCollapsed.png'][src!='images/ui/icons/tree/folderExpanded.png'][src!='images/ui/icons/tree/folderGreenCollapsed.png'][src!='images/ui/icons/tree/folderGreenExpanded.png']"");" + Environment.NewLine);
                    initCode.Append(@"doNothingFolders.each(function(i){" + Environment.NewLine);
                    initCode.Append(@"$ektron(this).next().removeAttr(""onclick"");" + Environment.NewLine);
                    initCode.Append(@"$ektron(this).next().children(""a"").bind(""click"", function(){" + Environment.NewLine);
                    initCode.Append(@"alert(""Please select a Catalog Folder"");" + Environment.NewLine);
                    initCode.Append(@"return false;" + Environment.NewLine);
                    initCode.Append(@"});" + Environment.NewLine);
                    initCode.Append(@"$ektron(this).next().children(""a"").removeAttr(""onclick"");" + Environment.NewLine);
                    initCode.Append(@"});" + Environment.NewLine);
                    initCode.Append(@"var plusFolders = $ektron(""ul.ektree li img[src='images/ui/icons/tree/folderCollapsed.png']"");" + Environment.NewLine);
                    initCode.Append(@"plusFolders.each(function(i){" + Environment.NewLine);
                    initCode.Append(@"$ektron(this).next().children(""a"").removeAttr(""onclick"");" + Environment.NewLine);
                    initCode.Append(@"});" + Environment.NewLine);
                    initCode.Append(@"var minusFolders = $ektron(""ul.ektree li img[src='images/ui/icons/tree/folderExpanded.png']"");" + Environment.NewLine);
                    initCode.Append(@"minusFolders.each(function(i){" + Environment.NewLine);
                    initCode.Append(@"$ektron(this).next().children(""a"").removeAttr(""onclick"");" + Environment.NewLine);
                    initCode.Append(@"});" + Environment.NewLine);

                    JavaScript.RegisterJavaScriptBlock(this, initCode.ToString());
                }
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.explorer.init.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.explorer.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.explorer.config.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.explorer.windows.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.cms.types.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.cms.parser.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.cms.toolkit.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.cms.api.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.ui.contextmenu.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.ui.iconlist.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.ui.tree.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.net.http.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.lang.exception.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.utils.form.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.utils.log.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.utils.dom.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.utils.debug.js"));
                mediaListUI.Components.Add(JavaScript.Create("Tree/js/com.ektron.utils.string.js"));
                StringBuilder variables = new StringBuilder();
                variables.Append(@"var ContentUrl = ""mediainsert.aspx?action=ViewLibraryByCategory&dentrylink=" + DEntryLink + @"&EditorName=" + sEditor + @"&scope=" + scope + @"&enhancedmetaselect=" + m_strEnhancedMetaSelect + @"&separator=" + m_strSeparator + "@&metadataformtagid=" + m_strMetadataFormTagId + m_showThumb + m_retField + @"&id="";" + Environment.NewLine);
                variables.Append(@"var FrameName=""Library"";" + Environment.NewLine);
                variables.Append(@"var vFolderName=""" + m_refMsg.GetMessage("generic library title") + @""";" + Environment.NewLine);
                JavaScript.RegisterJavaScriptBlock(this, variables.ToString(), false);
            }
            mediaListUI.Register(this);
        }

		private string GetClientScript()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("function ClearFolderInfo() {" + "\r\n");
			if (actionType == "library")
			{
				result.Append("if (typeof(top.mediainsert.ClearFolderInfo) == \"function\") {" + "\r\n");
				result.Append("top.mediainsert.ClearFolderInfo();" + "\r\n");
				result.Append("} else {" + "\r\n");
				result.Append("//TODO: BCB: Fix: setTimeout(ClearFolderInfo(), 5000);" + "\r\n");
				result.Append("return;" + "\r\n");
				result.Append("}" + "\r\n");
			}
			result.Append("}" + "\r\n");
			
			result.Append("function SetFolderIDvalue(fldID)" + "\r\n");
			result.Append("{" + "\r\n");
			//result.Append("//alert('Old folder -> ' +  top.mediauploader.document.LibraryItem.frm_folder_id.value);" & vbCrLf)
			//If Not (actionType = "library") Then
			//result.Append("top.mediauploader.document.LibraryItem.frm_folder_id.value = fldID;" & vbCrLf)
			//result.Append("top.mediauploader.document.LibraryItem.frm_scope = """ & scope & """;" & vbCrLf)
			//End If
			//result.Append("//alert('New folder -> ' +  top.mediauploader.document.LibraryItem.frm_folder_id.value);" & vbCrLf)
			result.Append("return;" + "\r\n");
			result.Append("}" + "\r\n");
			
			result.Append("ekFolderCreateTextLinks = 1;" + "\r\n");
			result.Append("ekFolderFontSize = 2;" + "\r\n");
			result.Append("ekFolderMaxDescriptionLength=0;" + "\r\n");
			result.Append("ekFolderImagePath = \"images/application/folders/\";" + "\r\n");
			PermissionData cPerms;
			cPerms = m_refContentApi.LoadPermissions(0, "folder", 0);
			
			if (cPerms.IsReadOnlyLib)
			{
				if (scope == "files")
				{
					result.Append("var urlInfoArray = new Array(\"frame\", \"javascript:ClearFolderInfo();\", \"medialist\", \"frame\", \"mediainsert.aspx?action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditor + "&id=" + 0 + "&scope=" + scope + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + "&type=files" + m_showThumb + m_retField + "\", \"mediainsert\");");
				}
				else if (scope == "images")
				{
					result.Append("var urlInfoArray = new Array(\"frame\", \"javascript:ClearFolderInfo();\", \"medialist\", \"frame\", \"mediainsert.aspx?action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditor + "&id=" + 0 + "&scope=" + scope + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + "&type=images" + m_showThumb + m_retField + "\", \"mediainsert\");");
				}
				else
				{
					result.Append("var urlInfoArray = new Array(\"frame\", \"javascript:ClearFolderInfo();\", \"medialist\", \"frame\", \"mediainsert.aspx?action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditor + "&id=" + 0 + "&scope=" + scope + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + m_showThumb + m_retField + "\", \"mediainsert\");");
				}
				result.Append("TopTreeLevel = CreateFolderInstance(\"" + m_refMsg.GetMessage("generic Library title") + "\", urlInfoArray);");
			}
			else
			{
				result.Append("TopTreeLevel = CreateFolderInstance(\"" + m_refMsg.GetMessage("generic Library title") + "\", \"\");");
			}
			
			cAllFolders = m_refContent.GetFolderTreeForUserID(0);
			OutputLibraryFolders(0, 0, result);
			return (result.ToString());
		}
		private string GetString()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			string AutoNavStr = "\\\\"; //javascript escape this character
			if (Request.QueryString["autonav"] != "")
			{
				AutoNavStr = EkFunctions.HtmlEncode(Request.QueryString["autonav"]);
				if (AutoNavStr != "")
				{
					AutoNavStr = AutoNavStr.Replace("\\", "\\\\");
				}
			}
			result.Append("<script language=\"javascript\">" + "\r\n");
			result.Append("<!--" + "\r\n");
			result.Append("function OpenLibFolder() {" + "\r\n");
			result.Append("if (parent.GetLoadStatus()) {" + "\r\n");
			result.Append("OpenFolder(\"" + AutoNavStr + "\", true);" + "\r\n");
			result.Append("}" + "\r\n");
			result.Append("else {" + "\r\n");
			result.Append("setTimeout(\"OpenLibFolder()\", 100);" + "\r\n");
			result.Append("}" + "\r\n");
			result.Append("}" + "\r\n");
			if (m_action == "viewlibrarycategory" || m_action == "viewlibrarybycategory")
			{
				result.Append("OpenLibFolder();" + "\r\n");
			}
			result.Append("//--></script>" + "\r\n");
			return (result.ToString());
		}
		private void OutputLibraryFolders(int level, long Parent, System.Text.StringBuilder result)
		{
			Ektron.Cms.Common.EkEnumeration.FolderDestinationType[] DestType = new Ektron.Cms.Common.EkEnumeration.FolderDestinationType[2];
			string[] Link = new string[2];
			string[] DestName = new string[2];
			string[] ExtParams = new string[2];
			
			DestType[0] = Ektron.Cms.Common.EkEnumeration.FolderDestinationType.Frame;
			Link[0] = "javascript:ClearFolderInfo();";
			DestName[0] = "medialist";
			ExtParams[0] = "";
			DestType[1] = Ektron.Cms.Common.EkEnumeration.FolderDestinationType.Frame;
			DestName[1] = "mediainsert";
			if (scope == "files")
			{
				ExtParams[1] = "&scope=" + scope + "&type=files";
				Link[1] = "mediainsert.aspx?action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditor + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + m_showThumb + m_retField + "&id=";
			}
			else if (scope == "images")
			{
				ExtParams[1] = "&scope=" + scope + "&type=images";
				Link[1] = "mediainsert.aspx?action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditor + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + m_showThumb + m_retField + "&id=";
			}
			else
			{
				ExtParams[1] = (string) ("&scope=" + scope);
				Link[1] = "mediainsert.aspx?action=ViewLibraryByCategory" + "&dentrylink=" + DEntryLink + "&EditorName=" + sEditor + "&enhancedmetaselect=" + m_strEnhancedMetaSelect + "&separator=" + m_strSeparator + "&metadataformtagid=" + m_strMetadataFormTagId + m_showThumb + m_retField + "&id=";
			}
			result.Append(m_refContent.OutputFolders(level, Parent, ref DestType, ref Link, ref DestName, ref ExtParams, ref cAllFolders, Ektron.Cms.Common.EkEnumeration.FolderTreeType.Library));
		}
		#region AJAX Support
		private bool IsInCallbackMode()
		{
            if (!string.IsNullOrEmpty(Request.QueryString["method"]))
			{
				//The following line added to support response to read as responseXml
				//Remove this if you want to read responseText
				Response.ContentType = "text/xml";
				Response.Write(RaiseCallbackEvent());
				Response.Flush();
				Response.End();
				return true;
			}
			return false;
		}
		// ****************************************************************************************************
		// Implement the callback interface
		private string RaiseCallbackEvent()
		{
			string result = "";
			FolderData[] folder_arr_data;
			FolderData folder_data;
			long m_intId;
			if ((!string.IsNullOrEmpty(Request.QueryString["method"])) && Request.QueryString["method"].ToLower() == "get_folder")
			{
				m_intId = Convert.ToInt64(Request.Params["id"]);
				folder_data = m_refContentApi.GetFolderById(m_intId);
				result = SerializeAsXmlData(folder_data, folder_data.GetType());
			}
            else if ((!string.IsNullOrEmpty(Request.QueryString["method"])) && Request.QueryString["method"].ToLower() == "get_child_folders")
			{
				m_intId = Convert.ToInt64(Request.Params["folderid"].ToString());
				folder_arr_data = m_refContentApi.GetChildFolders(m_intId, false, Ektron.Cms.Common.EkEnumeration.FolderOrderBy.Id);
				result = SerializeAsXmlData(folder_arr_data, folder_arr_data.GetType());
			}
			return (result);
		}
		private string SerializeAsXmlData(object data, Type datatype)
		{
			string result = "";
			System.IO.MemoryStream XmlOutStream = new System.IO.MemoryStream();
			XmlSerializer XmlSer;
			byte[] byteArr;
			System.Text.UTF8Encoding Utf8 = new System.Text.UTF8Encoding();
			XmlSer = new XmlSerializer(datatype);
			XmlSer.Serialize(XmlOutStream, data);
			byteArr = XmlOutStream.ToArray();
			result = System.Convert.ToBase64String(byteArr, 0, byteArr.Length);
			result = Utf8.GetString(Convert.FromBase64String(result));
			return (result);
		}
		#endregion
	}
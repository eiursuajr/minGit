using System;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
//using Ektron.Cms.Common.EkConstants;
//using Ektron.Cms.Common.EkEnumeration;

	public partial class CommunityDragDrop : System.Web.UI.Page
	{
		
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected EkContent m_refContent;
		protected EkMessageHelper m_refMsg;
		
		protected AssetInfoData[] asset_data;
		protected bool m_bIsMac;
		
		protected bool mode_set = false;
		protected int mode_id = 0; //mode=0->mode_id=folder_id, mode=1->mode_id=content_id
		protected int mode = 0; // 0=add, 1=update
		protected int isimage = 0;
		protected string overrideextension = "";
		
		protected ContentData content_data;
		protected FolderData folder_data;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			//m_refMsg = m_refContentApi.EkMsgRef
			
			//If (Request.Browser.Platform.IndexOf("Win") = -1) Then
			//m_bIsMac = True
			//Else
			//m_bIsMac = False
			//End If
			
			//m_refContent = m_refContentApi.EkContentRef()
			//Display_DropUploader()
			if (Request.QueryString["mode"] == "0" || (Request.QueryString["mode"] == null))
			{
				if (! (Request.QueryString["folderid"] == null) && Request.QueryString["folderid"] != "")
				{
					dropuploader.FolderID = Convert.ToInt64(Request.QueryString["folderid"]);
				}
				else
				{
					dropuploader.FolderID = Convert.ToInt64(Request.QueryString["mode_id"]);
				}
			}
			else
			{
				if (! (Request.QueryString["id"] == null) && Request.QueryString["id"] != "")
				{
					dropuploader.AssetID = Request.QueryString["id"];
				}
				else
				{
					dropuploader.AssetID = Request.QueryString["mode_id"];
				}
				dropuploader.FolderID = Convert.ToInt64(Request.QueryString["folder_id"]);
			}
			if (! (Request.QueryString["lang_id"] == null) && Request.QueryString["lang_id"] != "")
			{
				dropuploader.ContentLanguage = Convert.ToInt32(Request.QueryString["lang_id"]);
			}
			if (! (Request.QueryString["TaxonomyId"] == null) && Request.QueryString["TaxonomyId"] != "")
			{
				dropuploader.TaxonomyId = Convert.ToInt64(Request.QueryString["TaxonomyId"]);
			}
			
			if (! (Request.QueryString["isimage"] == null) && Request.QueryString["isimage"] != "")
			{
				dropuploader.IsImage = Convert.ToInt32(Request.QueryString["isimage"]);
			}
			
			if (! (Request.QueryString["overrideextension"] == null) && Request.QueryString["overrideextension"] != "")
			{
				dropuploader.OverrideExtension = Request.QueryString["overrideextension"];
			}
			
		}
		
	}


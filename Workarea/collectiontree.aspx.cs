using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Site;
using Ektron.Cms.User;
using Ektron.Cms.Content;
using Ektron.Cms;
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms.Common;
using Ektron.Cms.API;
using Microsoft.VisualBasic;
using System.Collections;
using System.Text;
using Ektron.Cms.Framework.UI;

public partial class Workarea_collectiontree : System.Web.UI.Page
{
    protected long CurrentUserID = -1;
    protected string ErrorString="";
    protected Collection cTmp;
    protected EkContent gtObj;
    protected string CollectionTitle = "";
    protected EkSite SiteObj;
    protected Array cLinkArray ;
    protected Array fLinkArray;
    protected string FolderName;
    protected Collection gtNavs;
    protected Hashtable cPerms;
    protected ApplicationAPI AppUI = new ApplicationAPI();
    protected EkMessageHelper MsgHelper;
    protected string AppPath;
    protected string AppImgPath;
    protected string sitePath;
    protected int ContentLanguage;
    protected bool EnableMultilingual;
    protected const int ALL_CONTENT_LANGUAGES = -1;
    protected const int CONTENT_LANGUAGES_UNDEFINED = 0;
    protected long nID = -1;
    protected long selTaxID = -1;
    protected long folderId = -1;
    protected long mpID = -1;
    protected long maID = -1;

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResources();
               
        Collection cFolders = null;
        long subfolderid = -1;
        long locID = -1;
        long backID = -1;
        Collection cRecursive = null;
        bool rec = false;
        bool CanCreateContent = false;
        string AddType = "";
        string MenuTitle = "";
        string CachedId = "";
        bool canTraverse = true;

        CurrentUserID = AppUI.UserId;
        AppImgPath = AppUI.AppImgPath;
        AppPath = AppUI.AppPath;
        sitePath = AppUI.SitePath;
        MsgHelper = AppUI.EkMsgRef;

        if (!string.IsNullOrEmpty(Request.QueryString["LangType"])) 
        {
	        ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
	        AppUI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        } else {
	        if (!string.IsNullOrEmpty(Convert.ToString(AppUI.GetCookieValue("LastValidLanguageID")))) {
		        ContentLanguage = Convert.ToInt32(AppUI.GetCookieValue("LastValidLanguageID"));
	        }
        }
        AppUI.ContentLanguage = ContentLanguage;

               
        folderId = Convert.ToInt64(Request.QueryString["folderid"]);
        mpID = Convert.ToInt64(Request.QueryString["parentid"]);
        maID = Convert.ToInt64(Request.QueryString["ancestorid"]);
        if (!string.IsNullOrEmpty(Request.QueryString["SelTaxonomyID"]))
        {
	        selTaxID = Convert.ToInt64(Request.QueryString["SelTaxonomyID"]);
        }

        if (CommonApi.GetEcmCookie().HasKeys)
        {
	        CurrentUserID = Convert.ToInt64(CommonApi.GetEcmCookie()["user_id"]);
        }
        else
        {
            CurrentUserID = 0;
        }

             

        AddType = Strings.LCase(Request.QueryString["addto"]);
        if (string.IsNullOrEmpty(AddType)) 
        {
	        AddType = "collection";
        }
        nID = Convert.ToInt64(Request.QueryString["nid"]);
        subfolderid = Convert.ToInt64(Request.QueryString["subfolderid"]);
        if ( !string.IsNullOrEmpty(Request.QueryString["cacheidentifier"]))
        {
	        CachedId = "&cacheidentifier=" + Request.QueryString["cacheidentifier"];
        }
        if (subfolderid > 0)
        {
	        locID = subfolderid;
        } 
        else
        {
	        locID = folderId;
        }

        gtObj = AppUI.EkContentRef;
        SiteObj = AppUI.EkSiteRef;
        cPerms = SiteObj.GetPermissions(locID, 0, "folder");
        canTraverse = Convert.ToBoolean(cPerms["TransverseFolder"]);
        if (ErrorString != "") 
        {
	        CanCreateContent = false;
        } else 
        {
	        CanCreateContent = Convert.ToBoolean(cPerms["Add"]);
        }
             
        if (AddType == "menu" || AddType == "submenu")
        {
	        cRecursive = gtObj.GetMenuByID(nID, 0,false);
	        if (string.IsNullOrEmpty(ErrorString))
            {
		        if (cRecursive.Count > 0)
                {
			        MenuTitle = cRecursive["MenuTitle"].ToString();
			        rec = Convert.ToBoolean(cRecursive["Recursive"]);
		        }
	        }
        }
        else
        {
	        cRecursive = gtObj.GetEcmCollectionByID(nID, false, false, ref ErrorString, true, false, true);
            if (string.IsNullOrEmpty(ErrorString))
            {
                if (cRecursive.Count > 0)
                {
                    CollectionTitle = cRecursive["CollectionTitle"].ToString();
                    rec = Convert.ToBoolean(cRecursive["Recursive"]);
                }
            }
            else
            {
                pnlError.Visible = true;
                pnlPage.Visible = false;
            }
        }
        if (!canTraverse) 
        {
	        rec = false;
        }
        if (rec && ErrorString == "")
        {
	        cTmp = new Collection();
	        cTmp.Add(Convert.ToInt64(locID), "ParentID",null,null);
	        cTmp.Add("name", "OrderBy",null,null);
	        cFolders = gtObj.GetAllViewableChildFoldersv2_0(cTmp);
        }
        if (ErrorString == "")
        {
	        gtNavs = gtObj.GetFolderInfov2_0(locID);
	               
            if(gtNavs.Count > 0)
            {
		        FolderName = gtNavs["FolderName"].ToString();
		        backID = Convert.ToInt64(gtNavs["ParentID"]);
		    }   
	           
        }
        StringBuilder sb = new StringBuilder();
        //ToolBar
        sb.Append("<table width=\"100%\">").Append(Environment.NewLine);
        if(rec)
        {
            if(locID != 0)
            {
                sb.Append("<tr>").Append(Environment.NewLine);
                sb.Append("  <td>").Append(Environment.NewLine);
                sb.Append("     <a href=\"collectiontree.aspx?nId=").Append(nID).Append("&folderid=").Append(folderId).Append("&subfolderid=").Append(backID).Append("&addto=").Append(AddType).Append("&parentid=").Append(mpID).Append("&ancestorid=").Append(maID).Append("&SelTaxonomyID=").Append(selTaxID).Append("\" ").Append(Environment.NewLine);
                sb.Append("         title=\"").Append(MsgHelper.GetMessage("alt: generic previous dir text")).Append("\" > ").Append(Environment.NewLine);
                sb.Append("        <img src=\"").Append(AppPath + "images/UI/Icons/folderUp.png").Append("\" title=\"").Append(MsgHelper.GetMessage("alt: generic previous dir text")).Append("\"  alt=\"").Append(MsgHelper.GetMessage("alt: generic previous dir text")).Append("\" align=\"absbottom\"/>..").Append(Environment.NewLine);
                sb.Append("     </a>").Append(Environment.NewLine);
                sb.Append("  </td>").Append(Environment.NewLine);
                sb.Append("  <td>&nbsp;</td>").Append(Environment.NewLine);
                sb.Append("  <td>&nbsp;</td>").Append(Environment.NewLine);
                sb.Append("</tr>").Append(Environment.NewLine);
           }

            foreach( Collection folder in cFolders)
            {
                if(Convert.ToInt32(folder["FolderType"]) <= 1)
                {
                    sb.Append("<tr>").Append(Environment.NewLine);
                    sb.Append("    <td nowrap=\"true\">").Append(Environment.NewLine);
                    sb.Append("         <a href=\"collectiontree.aspx?nId=").Append(nID).Append("&folderid=").Append(folderId).Append("&subfolderid=").Append(folder["ID"]).Append("&addto=").Append(AddType).Append("&parentid=").Append(mpID).Append("&ancestorid=").Append(maID).Append("&SelTaxonomyID=").Append(selTaxID).Append("\" ").Append(Environment.NewLine);
                    sb.Append("           title=\"").Append(MsgHelper.GetMessage("alt: generic view folder content text")).Append("\">").Append(Environment.NewLine);
                    sb.Append("         <img src=\"").Append(AppImgPath).Append(Convert.ToInt32(folder["FolderType"]) == 1 ? "folders/blogfolderopen.gif": "folderclosed_1.gif").Append("\" title=\"").Append(MsgHelper.GetMessage("alt: generic view folder content text")).Append("\" alt=\"").Append(MsgHelper.GetMessage("alt: generic view folder content text")).Append("\" align=\"absbottom\"/>").Append(folder["Name"]).Append(Environment.NewLine);
                    sb.Append("     </a>").Append(Environment.NewLine);
                    sb.Append("  </td>").Append(Environment.NewLine);
                    sb.Append("  <td>&nbsp;</td>").Append(Environment.NewLine);
                    sb.Append("  <td>&nbsp;</td>").Append(Environment.NewLine);
                    sb.Append("</tr>").Append(Environment.NewLine);
                }
            }
        }

         sb.Append("<tr>").Append(Environment.NewLine);
         sb.Append("    <td align=\"center\" colspan=\"3\">").Append(Environment.NewLine);
         sb.Append("       <hr/>").Append(Environment.NewLine);
         sb.Append("    </td>").Append(Environment.NewLine);
         sb.Append("</tr>").Append(Environment.NewLine);

          sb.Append("<tr>").Append(Environment.NewLine);
         sb.Append("    <td align=\"center\" colspan=\"3\">").Append(Environment.NewLine);
         sb.Append("       First Pick the folder").Append(Environment.NewLine);
         sb.Append("    </td>").Append(Environment.NewLine);
         sb.Append("</tr>").Append(Environment.NewLine);

         sb.Append("<tr>").Append(Environment.NewLine);
         sb.Append("    <td align=\"center\" colspan=\"3\">").Append(Environment.NewLine);
         if(AddType == "submenu")
             sb.Append("    <input name=\"next\" type=\"button\" value=\"Next...\" onclick=\"javascript:location.href='collections.aspx?action=AddSubMenu&LangType=").Append(ContentLanguage).Append("&folderid=").Append(locID).Append("&nId=").Append(nID).Append("&parentid=").Append(mpID).Append("&ancestorid=").Append(maID).Append("&SelTaxonomyID=").Append(selTaxID).Append("'\" />").Append(Environment.NewLine);
         else
             sb.Append("    <input name=\"next\" type=\"button\" value=\"Next...\" onclick=\"javascript:location.href='editarea.aspx?LangType=").Append(ContentLanguage).Append("&type=add&id=").Append(locID).Append("&SelTaxonomyID=").Append(selTaxID).Append("&mycollection=").Append(nID).Append("&addto=").Append(AddType).Append(CachedId).Append("'\" />").Append(Environment.NewLine);
         sb.Append("    </td>").Append(Environment.NewLine);
         sb.Append("</tr>").Append(Environment.NewLine);

         sb.Append("</table>").Append(Environment.NewLine);
         ltrPage.Text  = sb.ToString();
    }

    protected void RegisterResources()
    {
        Packages.EktronCoreJS.Register(this);
        Packages.Ektron.Workarea.Core.Register(this);
    }
}

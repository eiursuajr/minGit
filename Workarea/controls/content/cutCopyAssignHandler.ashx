<%@ WebHandler Language="C#" Class="cutCopyAssignHandler" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using Ektron.Newtonsoft.Json;
using System.Collections.Generic;
using Ektron.Cms;

public class ContentInfo
{
    // i = contentId, p = parentId, l = languageId
    // These one letter variables are used to save query string length
    // As it crashes in IIS7 if more than 2048.
    public long _i;
    public long _p;
    public int _l;
    public int _t;

    public long i
    {
        get{ return _i;}
        set{_i = value;}
    }
    public long p
    {
        get{ return _p; }
        set{ _p = value; }
    }
    public int l
    {
        get{ return _l; }
        set{ _l = value; }
    }
    public int t
    {
        get { return _t; }
        set { _t = value; }
    }
}

public class cutCopyAssignHandler : IHttpHandler
{
    public List<ContentInfo> _contentInfoData;
    int i = 0;
    string pageAction = string.Empty;
    string returnUrl = string.Empty;
    string contentInfoJsonData = string.Empty;
    long targetId = -1;
    long sourceFolderId = -1;
    ContentAPI _ContentAPI = new ContentAPI();
    ContentData _contentData = new ContentData();
    FolderData _folderData = new FolderData();
    Ektron.Cms.API.Folder _folderAPI = new Ektron.Cms.API.Folder();
    Ektron.Cms.Content.EkContent _EkContentRef = new Ektron.Cms.Content.EkContent();
    string folderPath = string.Empty;
    string taxonomyPath = string.Empty;
    long newTaxId = -1;
    int propertyId = 0;
    string multiLang = string.Empty;
    Ektron.Cms.Common.CustomPropertyData _customPropertyData = new Ektron.Cms.Common.CustomPropertyData();
    Ektron.Cms.Framework.Core.CustomProperty.CustomProperty _customProperty = new Ektron.Cms.Framework.Core.CustomProperty.CustomProperty();

    public List<ContentInfo> ContentInfo
    {
        get 
        {
            return _contentInfoData;
        }
    }
    public void ProcessRequest(HttpContext context)
    {
        context.Response.CacheControl = "no-cache";
        context.Response.AddHeader("Pragma", "no-cache");
        context.Response.Expires = -1;
        string contentIDS = string.Empty;

        if (context.Request.Form["action"] != null)
        {
            pageAction = context.Request.Form["action"];
        }
        if (context.Request.Form["targetId"] != null)
        {
            targetId = long.Parse(context.Request.Form["targetid"]);
        }

        if (context.Request.Form["clipBoardContext"] != null)
        {
            contentInfoJsonData = context.Request.Form["clipBoardContext"];
        }
        int langType = -1;
        
        if (context.Request.Form["sourceid"] != null)
        {
            sourceFolderId = long.Parse(context.Request.Form["sourceid"]);
        }

        if (context.Request.Form["LangType"] != null)
        {
            langType = Convert.ToInt32(context.Request.Form["LangType"]);
        }

        if (contentInfoJsonData != string.Empty && contentInfoJsonData!= null)
        {
            _contentInfoData = (List<ContentInfo>)JsonConvert.DeserializeObject(contentInfoJsonData, typeof(List<ContentInfo>));
        }
        if (context.Request.Form["multilang"] != null)
        {
            multiLang = context.Request.Form["multilang"];
        }
        _ContentAPI.RequestInformationRef.ContentLanguage = Convert.ToInt32(_ContentAPI.GetCookieValue("LastValidLanguageID"));
        _EkContentRef = _ContentAPI.EkContentRef;

        if (context.Request.QueryString["propid"] != null && context.Request.QueryString["propid"] != "")
        {
            propertyId = Convert.ToInt16(context.Request.QueryString["propid"]);
        }
        try
        {
            switch (pageAction.ToLower())
            {
                case "cutcontent":
                    for (i = 0; i < _contentInfoData.Count; i++)
                    {
                        UpdateXMLConfig(_contentInfoData[i]);
                        _EkContentRef.MoveContent(_contentInfoData[i].i, targetId);
                    }
                    break;
                    
                case "copycontent":
                    if (multiLang.ToLower() == "true")
                    {
                        for (i = 0; i < _contentInfoData.Count; i++)
                        {
                            UpdateXMLConfig(_contentInfoData[i]);
                            _EkContentRef.CopyAllLanguageContent(_contentInfoData[i].i, targetId, true);
                        }
                    }
                    else
                    {
                        for (i = 0; i < _contentInfoData.Count; i++)
                        {
                            UpdateXMLConfig(_contentInfoData[i]);
                            _EkContentRef.CopyContentByID(_contentInfoData[i].i, _contentInfoData[i].l, targetId, true);
                        }
                    }
                    break;
                    
                case "assignitems":
                    for (i = 0; i < _contentInfoData.Count; i++)
                    {
                        Ektron.Cms.TaxonomyRequest itemRequest = new TaxonomyRequest();
                        itemRequest.TaxonomyId = long.Parse(context.Request.Form["targetid"]);
                        itemRequest.TaxonomyIdList = _contentInfoData[i].i.ToString();
                        itemRequest.TaxonomyLanguage = Convert.ToInt16(_contentInfoData[i].l);
                        _EkContentRef.AddTaxonomyItem(itemRequest);
                    }
                    break;
                    
                case "deletecontent":
                    for (i = 0; i < _contentInfoData.Count; i++)
                    {
                        _EkContentRef.SubmitForDeletev2_0(_contentInfoData[i].i.ToString(), _contentInfoData[i].p);
                    }
                    break;
                    
                case "assignitemstocollection":
                    Microsoft.VisualBasic.Collection cCollection = new Microsoft.VisualBasic.Collection();
                    Microsoft.VisualBasic.Collection collectionContentItems = new Microsoft.VisualBasic.Collection();
                    string str = "";
                    //Only assign unassigned items to a collection
                    
                    cCollection =  _ContentAPI.EkContentRef.pGetEcmCollectionByID(targetId, false, false, ref str, 0, null, true, true);
                    collectionContentItems = (Microsoft.VisualBasic.Collection) cCollection["Contents"];
                    for (i = 0; i < _contentInfoData.Count; i++)
                    {
                        bool valid = IsContentInEcmCollection(_contentInfoData[i].i, collectionContentItems);
                        if(valid != true)
                        {
                            _EkContentRef.AddItemToEcmCollection(targetId, _contentInfoData[i].i, _contentInfoData[i].l);
                        }
                    }
                    break;
                    
                case "assignitemstomenu":
                    for (i = 0; i < _contentInfoData.Count; i++)
                    {
                        Microsoft.VisualBasic.Collection itemObj = new Microsoft.VisualBasic.Collection();
                        itemObj.Add(_contentInfoData[i].i, "ItemID", null, null);
                        itemObj.Add("content", "ItemType", null, null);
                        itemObj.Add("self", "ItemTarget", null, null);
                        itemObj.Add("", "ItemLink", null, null);
                        itemObj.Add("", "ItemDescription", null, null);
                        itemObj.Add("", "ItemTitle", null, null);
                        _EkContentRef.AddItemToEcmMenu(targetId, itemObj);
                    }
                    break;
                    
                case "cutfolder":
                    _ContentAPI.MoveFolder(sourceFolderId, targetId, true);
                    break;
                    
                case "copyfolder":
                    _ContentAPI.CopyFolder(sourceFolderId, targetId, true, true);
                    break;
                    
                case "cuttaxonomy":
                    if (targetId == -1)
                    {
                        targetId = 0;
                    }
                    _ContentAPI.EkContentRef.MoveTaxonomy(sourceFolderId, targetId,  true);
                    newTaxId = sourceFolderId;
                    context.Response.Write(newTaxId);
                    break;

                case "copytaxonomy":
                    if (targetId == -1)
                    {
                        targetId = 0;
                    }
                    newTaxId = _ContentAPI.EkContentRef.CloneTaxonomy(sourceFolderId, targetId, langType, -1, true, false);
                    context.Response.Write(newTaxId);
                    break;

                case "deletetaxonomy":
                    TaxonomyRequest taxonomyRequestData = new TaxonomyRequest();
                    taxonomyRequestData.TaxonomyId = sourceFolderId;
                    taxonomyRequestData.TaxonomyLanguage = langType;
                    _ContentAPI.EkContentRef.DeleteTaxonomy(taxonomyRequestData);
                    break;
                    
                case "getfolderpath":
                    folderPath = _ContentAPI.EkContentRef.GetFolderPath(targetId);
                    if (folderPath.Length > 0)
                    {
                        if (folderPath.PadRight(1) == "\\")
                         {
                             folderPath = folderPath.PadRight(folderPath.Length - 1);
                         }
                         
                         // Strip off the current folder name from the path:
                         int Pos = 0;
                         Pos = folderPath.LastIndexOf("\\");
                         if (Pos > 0)
                         {
                             folderPath = folderPath.PadLeft(Pos - 1);
                         }
                         // Escape backslashes:
                         folderPath = folderPath.Replace("\\", "\\\\");
                         context.Response.Write(folderPath);
                    }
                    break;
                case "getcustompropdata":                    
                    GetCustomPropertyData(context);
                    break;
            }
        }

        catch(Exception ex)
        {
            context.Response.Write("Error: " + ex.Message);
        }
    }
    protected void GetCustomPropertyData(HttpContext httpContext)
    {   string returnResponse;
        _customPropertyData = _customProperty.GetItem(propertyId, _ContentAPI.RequestInformationRef.ContentLanguage);
        returnResponse="{Title:" + _customPropertyData.PropertyName + ", DataType:" + _customPropertyData.PropertyDataType + ", Items" + _customPropertyData.Items.Count + ":" + GetCustomPropertyItems(_customPropertyData) + "}";
        httpContext.Response.Write(returnResponse);
        
    }
    private string GetCustomPropertyItems(Ektron.Cms.Common.CustomPropertyData _customPropertyData)
    {
        int count  = 0;
        System.Text.StringBuilder returnItems = new System.Text.StringBuilder();
        returnItems.Append("{");
        for (count = 0; count< _customPropertyData.Items.Count; count++)
        {
            returnItems.Append("item" + count + ":" + _customPropertyData.Items[count].PropertyValue + ";");
        }
        return returnItems.ToString();
    }
    protected void UpdateXMLConfig(ContentInfo contentInfo) 
    {
        int j = 0;
        Microsoft.VisualBasic.Collection xmlCollection = new Microsoft.VisualBasic.Collection();
        XmlConfigData[] active_list = null;
        
        _contentData = _ContentAPI.GetContentById(contentInfo.i, ContentAPI.ContentResultType.Published);

        active_list = _ContentAPI.GetEnabledXmlConfigsByFolder(targetId);
        
        if (_contentData != null)
        {
            if (active_list.Length > 0)
            {
                for (j = 0; j < active_list.Length; j++)
                {
                    if (_contentData.XmlConfiguration != null)
                    {
                        if (_contentData.XmlConfiguration.Id != active_list[j].Id)
                        {
                            xmlCollection.Add(active_list[j].Id.ToString(), "" + j + "", null, null);
                        }
                    }
                    else
                    {
                        xmlCollection.Add(active_list[j].Id.ToString(), "" + j + "", null, null);
                    }
                }
            }
            if (_contentData.XmlConfiguration != null)
            {
                xmlCollection.Add(_contentData.XmlConfiguration.Id.ToString(), "" + active_list.Length + "", null, null);
            }
        }
        
        // Assigning the XML Configuration to the target folder, if the current content is based on Smart Form.
        _folderAPI.EkContentRef.UpdateFolderXmlList(targetId, 0, xmlCollection);
    }
    
    //Checks if the item is already a part of a collection.
    
    protected bool IsContentInEcmCollection(long ContentID,Microsoft.VisualBasic.Collection collectionContentItems)
    {
        if (collectionContentItems != null)
        {
            foreach (Microsoft.VisualBasic.Collection cTmp in collectionContentItems)
            {
                if (Convert.ToInt32(cTmp["ContentID"]) == ContentID)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
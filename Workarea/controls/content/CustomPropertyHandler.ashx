<%@ WebHandler Language="C#" Class="CustomPropertyHandler" %>

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Content;
using Ektron.Cms.Framework.Core.CustomProperty;

/// <summary>
/// Serializable data structure representing an individual
/// custom property.
/// </summary>
[Serializable]
public class PropertyInfo
{
    public string Title { get; set; }

    public string Id { get; set; }

    public string DataType { get; set; }

    public bool IsEditable { get; set; }

    public List<PropertyItemInfo> Items { get; set; }
}

/// <summary>
/// Serializable data structure representing an individual
/// value associated with a property.
/// </summary>
[Serializable]
public class PropertyItemInfo
{
    public string Id { get; set; }

    public string FormattedValue { get; set; }

    public string Value { get; set; }

    public bool IsSelected { get; set; }
}

/// <summary>
/// Serailize data structure representing a page of property info.
/// </summary>
[Serializable]
public class PropertyInfoPage
{
    public List<PropertyInfo> PropertyInfo { get; set; }

    public int Page { get; set; }

    public int TotalPages { get; set; }
}

/// <summary>
/// HTTP handler providing access to custom property data.
/// </summary>
public class CustomPropertyHandler : IHttpHandler 
{
    private const string ActionGetPropertyData = "getcustompropdata";
    private const string ActionGetPropertyObjectData = "getcustompropobjectdata";
    private const string QueryParamAction = "action";
    private const string QueryParamPropertyId = "propid";
    private const string QueryParamObjectId = "objectid";
    private const string QueryParamCurrentPage = "page";
    private const string QueryParamRecordsPerPage = "count";
    private const int MaxRecords = 99999;
    
    private ContentAPI _contentApi;
    private EkContent _ekContentRef;
    private EkRequestInformation _ekRequestInfo;
    private JavaScriptSerializer _serializer;
    
    /// <summary>
    /// Processes the incoming request and delegates it according to the
    /// specified action.
    /// </summary>
    /// <param name="context">HTTP context supporting the request.</param>
    public void ProcessRequest(HttpContext context)
    {
        Initialize();

        string response = string.Empty;

        string pageAction = context.Request.QueryString[QueryParamAction];
        if (pageAction != null)
        {
            switch (pageAction.ToLower())
            {
                // Processes a request for data associated with a specified
                // custom property ID.
                case ActionGetPropertyData:
                    long propertyId = -1;
                    if (!string.IsNullOrEmpty(context.Request.QueryString[QueryParamPropertyId]))
                    {
                        if (long.TryParse(context.Request.QueryString[QueryParamPropertyId], out propertyId))
                        {
                            response = GetCustomPropertyData(propertyId);
                        }
                    }
                    break;
                    
                // Processes a request for all custom property data associated with
                // a specified CMS object.
                case ActionGetPropertyObjectData:
                    long objectId = -1;
                    int currentPage = -1;
                    int recordsPerPage = -1;
                    
                    if (!string.IsNullOrEmpty(context.Request.QueryString[QueryParamObjectId]))
                    {
                        if (long.TryParse(context.Request.QueryString[QueryParamObjectId], out objectId))
                        {
                            response = GetCustomPropertyObjectData(objectId);
                        }
                    }

                    if (!string.IsNullOrEmpty(context.Request.QueryString[QueryParamCurrentPage]))
                    {
                        int.TryParse(
                            context.Request.QueryString[QueryParamCurrentPage], 
                            out currentPage);
                    }

                    if (!string.IsNullOrEmpty(context.Request.QueryString[QueryParamRecordsPerPage]))
                    {
                        int.TryParse(
                            context.Request.QueryString[QueryParamRecordsPerPage], 
                            out recordsPerPage);
                    }

                    response = GetCustomPropertyObjectData(objectId, currentPage, recordsPerPage);
                    break;
            }
        }

        context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.Cache.SetNoStore();
        
        context.Response.Write(response);
    }

    /// <summary>
    /// Returns a flag indicating whether or not this HTTP handler is
    /// resuable.
    /// </summary>
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// Initializes this HTTP handler for use.
    /// </summary>
    private void Initialize()
    {
        _contentApi = new ContentAPI();
        _ekRequestInfo = _contentApi.RequestInformationRef;
        _ekContentRef = _contentApi.EkContentRef;

        _ekRequestInfo.ContentLanguage =
            Convert.ToInt32(_contentApi.GetCookieValue("LastValidLanguageID"));

        _serializer = new JavaScriptSerializer();
    }

    /// <summary>
    /// Returns a JSON response string representing the custom
    /// property data with the specified ID.
    /// </summary>
    /// <param name="httpContext">HTTP context supporting the request</param>
    protected string GetCustomPropertyData(long propertyId)
    {
        string response = string.Empty;

        if (propertyId > -1)
        {
            CustomProperty customProperty = new CustomProperty();
            CustomPropertyData customPropertyData = customProperty.GetItem(
                propertyId,
                _contentApi.RequestInformationRef.ContentLanguage);

            PropertyInfo propertyInfo = new PropertyInfo();
            propertyInfo.Id = customPropertyData.PropertyId.ToString();
            propertyInfo.Title = customPropertyData.PropertyName;
            propertyInfo.DataType = customPropertyData.PropertyDataType.ToString();
            propertyInfo.IsEditable = customPropertyData.IsEditable;
            propertyInfo.Items = new List<PropertyItemInfo>();
            
            foreach (CustomPropertyItemData item in customPropertyData.Items)
            {
                PropertyItemInfo itemInfo = new PropertyItemInfo();
                itemInfo.Id = item.PropertyId.ToString();
                itemInfo.IsSelected = item.IsDefault;

                if (item.PropertyValue != null)
                {
                    itemInfo.Value = item.PropertyValue.ToString();
                    itemInfo.FormattedValue = GetFormattedValue(item.PropertyValue);
                }
                else
                {
                    itemInfo.Value = string.Empty;
                    itemInfo.FormattedValue = string.Empty;
                }

                propertyInfo.Items.Add(itemInfo);
            }

            response = _serializer.Serialize(propertyInfo);
        }
        
        return response;
    }

    /// <summary>
    /// Returns a collection of properties associated with the specified
    /// object.
    /// </summary>
    /// <param name="objectId">ID of the object to retrieve properties for</param>
    protected string GetCustomPropertyObjectData(long objectId)
    {
        return GetCustomPropertyObjectData(objectId, -1, -1);
    }

    /// <summary>
    /// Gets a page of custom property data. If a value less than 1 is passed for 
    /// page and/or recordsPerPage all records will be returned.
    /// </summary>
    /// <param name="objectId">Custom property object ID</param>
    /// <param name="page">Page of values to return</param>
    /// <param name="recordsPerPage">Records per page to return</param>
    /// <returns>JSON response string</returns>
    protected string GetCustomPropertyObjectData(long objectId, int page, int recordsPerPage)
    {
        string response = string.Empty;

        if (objectId > -1)
        {
            bool includePagingData = false;
            
            PagingInfo pagingInfo = new PagingInfo();
            if (page > 0 && recordsPerPage > 0)
            {
                includePagingData = true;

                pagingInfo.CurrentPage = page;
                pagingInfo.RecordsPerPage = recordsPerPage;
            }
            else
            {
                pagingInfo.CurrentPage = 1;
                pagingInfo.RecordsPerPage = MaxRecords;
            }
            
            CustomPropertyObject customPropertyObject = new CustomPropertyObject();

            // Retrieve a collection of property object data associated with
            // the specified object ID.

            List<CustomPropertyObjectData> customPropertyObjectDataList = customPropertyObject.GetList(
                objectId,
                _ekRequestInfo.ContentLanguage,
                EkEnumeration.CustomPropertyObjectType.TaxonomyNode,
                pagingInfo);

            List<PropertyInfo> propertyInfo = 
                GetCustomPropertyObjectData(customPropertyObjectDataList);

            if (includePagingData)
            {
                PropertyInfoPage propertyInfoPage = new PropertyInfoPage();
                propertyInfoPage.Page = pagingInfo.CurrentPage;
                propertyInfoPage.TotalPages = pagingInfo.TotalPages;
                propertyInfoPage.PropertyInfo = propertyInfo;

                response = _serializer.Serialize(propertyInfoPage);
            }
            else
            {
                response = _serializer.Serialize(propertyInfo);
            }
        }

        return response;
    }

    /// <summary>
    /// Prepares a response for the specified collection of object data.
    /// </summary>
    /// <param name="customPropertyObjectDataList">Collection from which to generate the response</param>
    /// <returns>Collection of PropertyInfo</returns>
    private List<PropertyInfo> GetCustomPropertyObjectData(List<CustomPropertyObjectData> customPropertyObjectDataList)
    {
        List<PropertyInfo> propertyInfo = new List<PropertyInfo>();
        
        if (customPropertyObjectDataList != null)
        {
            foreach (CustomPropertyObjectData objectData in customPropertyObjectDataList)
            {
                CustomProperty customProperty = new CustomProperty();
                CustomPropertyData customPropertyData = customProperty.GetItem(
                    objectData.PropertyId,
                    objectData.LanguageId);

                if (customPropertyData != null)
                {
                    PropertyInfo property = new PropertyInfo();
                    property.Id = customPropertyData.PropertyId.ToString();
                    property.Title = customPropertyData.PropertyName;
                    property.DataType = customPropertyData.PropertyDataType.ToString();
                    property.IsEditable = customPropertyData.IsEditable;
                    property.Items = new List<PropertyItemInfo>();

                    if (customPropertyData.Items.Count > 1)
                    {
                        foreach (CustomPropertyItemData propertyItem in customPropertyData.Items)
                        {
                            PropertyItemInfo itemInfo = new PropertyItemInfo();
                            itemInfo.Id = propertyItem.PropertyId.ToString();
                            itemInfo.Value = propertyItem.PropertyValue.ToString();
                            itemInfo.FormattedValue = GetFormattedValue(propertyItem.PropertyValue);
                            itemInfo.IsSelected = HasPropertyItem(objectData, propertyItem);

                            property.Items.Add(itemInfo);
                        }
                    }
                    else
                    {
                        PropertyItemInfo itemInfo = new PropertyItemInfo();
                        itemInfo.Id = objectData.Items[0].PropertyId.ToString();
                        itemInfo.Value = objectData.Items[0].PropertyValue.ToString();
                        itemInfo.FormattedValue = GetFormattedValue(objectData.Items[0].PropertyValue);
                        itemInfo.IsSelected = true;

                        property.Items.Add(itemInfo);
                    }

                    propertyInfo.Add(property);
                }
            }
        }
        
        return propertyInfo;
    }

    /// <summary>
    /// Returns a specified value formatted for readability.
    /// </summary>
    /// <param name="value">Value to format</param>
    /// <returns>Formatted representation of the specified value</returns>
    private static string GetFormattedValue(object value)
    {
        string formattedValue = null;

        DateTime dateTimeValue;
        if (DateTime.TryParse(value.ToString(), out dateTimeValue))
        {
            formattedValue = dateTimeValue.ToString("F");
        }

        return formattedValue != null ? formattedValue : value.ToString();
    }

    /// <summary>
    /// Returns true if the specified CustomPropertyObjectData instance has had
    /// the specified property associated with it. This is relevent in determining which
    /// property item, of a multi-value property, is associated with a particular property
    /// object.
    /// </summary>
    /// <param name="propertyObjectData">Custom property object to check for value association</param>
    /// <param name="propertyItem">Property item to look for</param>
    /// <returns>True if a property item has been associated with the specified object, false otherwise</returns>
    private static bool HasPropertyItem(CustomPropertyObjectData propertyObjectData, CustomPropertyItemData propertyItem)
    {
        bool hasPropertyItem = false;
        if (propertyObjectData != null && propertyItem != null)
        {
            // Search the CustomPropertyObjectData's list of items for a property
            // matching the ID and value pair.
            
            for (int i = 0; i < propertyObjectData.Items.Count && !hasPropertyItem; i++)
            {
                hasPropertyItem =
                    propertyItem.PropertyId == propertyObjectData.Items[i].PropertyId &&
                    propertyItem.PropertyValue.ToString() == propertyObjectData.Items[i].PropertyValue.ToString();
            }
        }

        return hasPropertyItem;        
    }
}
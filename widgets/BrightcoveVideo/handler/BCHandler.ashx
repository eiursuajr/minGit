<%@ WebHandler Language="C#" Class="Ektron.Widgets.BrightCoveWidget.BCHandler" %>

using System;
using System.Web;
using System.Text;
using Ektron.Cms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ektron.Cms.API.Search;
using Ektron.Cms.WebSearch.SearchData;
using Ektron.Cms.WebSearch;
using Ektron.Cms.API.Content;
using Ektron.Cms.API;
using Ektron.Cms.Widget;
using Ektron.Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Ektron.Widgets.BrightCoveWidget
{
    public class BCHandler : IHttpHandler
    {
        private JsonRequest request;
        private BrightcoveResponse response;
        SettingsManager settingsManager;
        Settings settings;
        AuthenticationManager authManager;

        private static readonly Encoding encoding = Encoding.UTF8;

        HttpContext _context;
        private String JSONCommand;
        private long widgetID = 0;
         
        public void ProcessRequest(HttpContext context)
        {

            // Process initial request and determine action
            // Set some globals
            _context = context;
            _context.Response.ContentType = "text/plain";
            _context.Response.Buffer = false;
            response = new BrightcoveResponse();
            authManager = new AuthenticationManager();
            
            if (authManager.isRequestAuthorized())
            {

                try
                {
                    // Deserialize Request
                    if (_context.Request["request"] != null)
                    {
                        request = (JsonRequest)JsonConvert.DeserializeObject(_context.Request["request"], typeof(JsonRequest));
                    }
                    else
                    {
                        response.message = "The request was not formatted correctly";
                    }

                    if (request != null)
                    {
                        WidgetSettings.widgetID = request.widgetid;
                        //settingsManager = new SettingsManager();
                        //settings = settingsManager.settings;

                        switch (request.action)
                        {
                            case "getallvideos":
                                processGetAllVideos(request.pagenumber);
                                break;
                            case "getallplaylists":
                                processGetAllPlaylists();
                                break;
                            case "searchglobal":
                                processSearchGlobal(request.pagenumber);
                                break;
                            case "videoread":
                                processVideoRead();
                                break;
                            case "videocreate":
                                processVideoCreate();
                                break;
                            case "videodelete":
                                processVideoDelete();
                                break;
                            case "videoupdate":
                                processVideoUpdate();
                                break;
                            case "validate":
                                processValidate();
                                break;
                            case "validatepublisher":
                                processValidatePublisher();
                                break;
                            case "validatereadtoken":
                                processValidateReadToken();
                                break;
                            case "validatewritetoken":
                                processValidateWriteToken();
                                break;
                            case "validateplayerid":
                                processValidatePlayerId();
                                break;
                        }
                    }
                }
                catch (Exception exc)
                {
                    JsonException jExp = new JsonException(exc);
                    _context.Response.Write(jExp.toJSON());
                }
            }

            // TODO - refactor this section 
            if (!response.success)
            {
                if (String.IsNullOrEmpty(response.message))
                {
                    response.message = "There was an error processing the request.";
                }
                JsonException jExp = new JsonException(response.message);
                _context.Response.Write(jExp.toJSON());
            }
            else if (String.IsNullOrEmpty(response.data))
            {
                _context.Response.Write("{\"success\": true}");
            }
            else
            {
                _context.Response.Write(response.data);
            }

            _context.Response.End();
        }

        private void processValidate()
        {
            response = authManager.isReadTokenValid(request.readToken);
            if (response.success)
            {
                response.data = ""; // TODO refactor this
            }
        }

        private void processGetAllVideos(int pagenumber)
        {
            VideoManager videoManager = new VideoManager();
            response = videoManager.GetAll(pagenumber);
            if (!response.success && String.IsNullOrEmpty(response.message))
            {
                response.message = "Videos not availlable at this time.";
            }

        }

        private void processGetAllPlaylists()
        {
            PlaylistManager playlistManger = new PlaylistManager();
            response = playlistManger.GetAll();
        }

        private void processSearchGlobal(int pagenumber)
        {
            SearchManager searchManager = new SearchManager();
            response = searchManager.Search(request.searchterm, request.searchtype, request.searchSort, request.sortOrder, pagenumber);
        }

        private void processVideoRead()
        {
            VideoManager videoManager = new VideoManager();
            response = videoManager.Read(long.Parse(request.id));
        }

        private void processVideoCreate()
        {
            VideoManager videoManager = new VideoManager();
            response = videoManager.Create(request);
        }

        private void processVideoDelete()
        {
            VideoManager videoManager = new VideoManager();
            response = videoManager.Delete(long.Parse(request.id));
        }

        private void processVideoUpdate()
        {
            VideoManager videoManager = new VideoManager();
            response = videoManager.Update(request);
        }

        private void processValidatePublisher()
        {
            response = authManager.isPublisherValid(request.value);
        }

        private void processValidateReadToken()
        {
            response = authManager.isReadTokenValid(request.value);
        }

        private void processValidateWriteToken()
        {
            response = authManager.isWriteTokenValid(request.value);
        }

        private void processValidatePlayerId()
        {
            response = authManager.isPlayerIdValid(request.value);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class SearchManager
    {
        SettingsManager settingsManager;
        Settings settings;

        public SearchManager()
        {
            settingsManager = new SettingsManager();
            settings = settingsManager.settings;
        }

        public BrightcoveResponse Search(string term, string searchType, string searchSort, string sortOrder, int pagenumber)
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.read);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("command", "search_videos");
            paramaters.Add("token", settings.readToken);
            paramaters.Add("page_size", "15");
            paramaters.Add("page_number", pagenumber.ToString());
            paramaters.Add("any", this.getSearchString(term, searchType));
            paramaters.Add("sort_by", String.Format("{0}:{1}", searchSort, sortOrder));
            paramaters.Add("get_item_count", "true");
            paramaters.Add("video_fields", "id,name,shortDescription,lastModifiedDate,tags,thumbnailURL,referenceId,length");
            BrightcoveResponse response = bcRequest.executeGET(paramaters);
            return response;
        }

        public void Search(long playlistID)
        {
            // Waiting on BC API to be released
        }

        private string getSearchString(string term, string searchType)
        {
            string searchTerm = term;

            switch (searchType)
            {
                case "1":
                    searchTerm = term;
                    break;
                case "2":
                    searchTerm = String.Format("custom_fields:{0}", term);
                    break;
                case "3":
                    searchTerm = String.Format("reference_id:{0}", term);
                    break;
                case "4":
                    searchTerm = String.Format("tag:{0}", term);
                    break;
            }

            return searchTerm;
        }
    }

    public class PlaylistManager
    {
        SettingsManager settingsManager;
        Settings settings;

        public PlaylistManager()
        {
            settingsManager = new SettingsManager();
            settings = settingsManager.settings;
        }

        public BrightcoveResponse GetAll()
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.read);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("command", "find_all_playlists");
            paramaters.Add("token", settings.readToken);
            paramaters.Add("sort_by", "MODIFIED_DATE");
            paramaters.Add("sort_order", "DESC");
            paramaters.Add("get_item_count", "true");
            paramaters.Add("video_fields", "id,name,shortDescription,lastModifiedDate,tags,thumbnailURL,referenceId,length");
            BrightcoveResponse response = bcRequest.executeGET(paramaters);
            return response;
        }

        public BrightcoveResponse GetVideos()
        {
            throw new NotImplementedException();
        }
    }

    public class VideoManager
    {
        SettingsManager settingsManager;
        Settings settings;
            
        public VideoManager()
        {
            settingsManager = new SettingsManager();
            settings = settingsManager.settings;
        }

        public BrightcoveResponse Read(long videoID)
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.read);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("command", "find_video_by_id");
            paramaters.Add("token", settings.readToken);
            paramaters.Add("video_id", videoID.ToString());
            paramaters.Add("video_fields", "id,name,shortDescription,lastModifiedDate,tags,thumbnailURL,referenceId,length");
            BrightcoveResponse response = bcRequest.executeGET(paramaters);
            return response;
        }

        public BrightcoveResponse Create(JsonRequest request)
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.write);
            string jsonPayload = this.BuildAddJSON(request);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("paramaters", jsonPayload);
            paramaters.Add("file", bcRequest.getFile(request.fileurl));
            BrightcoveResponse response = bcRequest.executePOST(paramaters);
            return response;
        }

        public BrightcoveResponse Delete(long videoID)
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.write);
            string jsonPayload = this.BuildDeleteJSON(videoID);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("paramaters", jsonPayload);
            BrightcoveResponse response = bcRequest.executePOST(paramaters);
            return response;
        }

        public BrightcoveResponse Update(JsonRequest request)
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.write);
            string jsonPayload = this.BuildUpdateJSON(request);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("paramaters", jsonPayload);
            BrightcoveResponse response = bcRequest.executePOST(paramaters);
            return response;
        }

        public BrightcoveResponse GetAll(int pagenumber)
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.read);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("command", "find_all_videos");
            paramaters.Add("token", settings.readToken);
            paramaters.Add("page_size", "15");
            paramaters.Add("page_number", pagenumber.ToString());
            paramaters.Add("sort_by", "MODIFIED_DATE");
            paramaters.Add("sort_order", "DESC");
            paramaters.Add("get_item_count", "true");
            paramaters.Add("video_fields", "id,name,shortDescription,lastModifiedDate,tags,thumbnailURL,referenceId,length");
            BrightcoveResponse response = bcRequest.executeGET(paramaters);
            return response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String BuildAddJSON(JsonRequest request)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("method");
                jsonWriter.WriteValue("create_video");
                jsonWriter.WritePropertyName("params");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("token");
                jsonWriter.WriteValue(settings.writeToken);
                jsonWriter.WritePropertyName("video");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("name");
                jsonWriter.WriteValue(request.title);
                jsonWriter.WritePropertyName("shortDescription");
                jsonWriter.WriteValue(request.description);
                jsonWriter.WritePropertyName("tags");
                jsonWriter.WriteStartArray();
                string[] tags = request.tags.Split(',');
                foreach (string tag in tags)
                {
                    jsonWriter.WriteValue(tag);
                }
                jsonWriter.WriteEndArray();
                jsonWriter.WritePropertyName("referenceId");
                jsonWriter.WriteValue(request.refid);
                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
                jsonWriter.WritePropertyName("filename");
                jsonWriter.WriteValue(request.fileurl);
                jsonWriter.WriteEndObject();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String BuildUpdateJSON(JsonRequest request)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("method");
                jsonWriter.WriteValue("update_video");
                jsonWriter.WritePropertyName("params");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("video");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("id");
                jsonWriter.WriteValue(request.id);
                jsonWriter.WritePropertyName("name");
                jsonWriter.WriteValue(request.title);
                jsonWriter.WritePropertyName("shortDescription");
                jsonWriter.WriteValue(request.description);
                jsonWriter.WritePropertyName("tags");
                jsonWriter.WriteStartArray();
                string[] tags = request.tags.Split(',');
                foreach (string tag in tags)
                {
                    jsonWriter.WriteValue(tag);
                }
                jsonWriter.WriteEndArray();
                jsonWriter.WritePropertyName("referenceId");
                jsonWriter.WriteValue(request.refid);
                jsonWriter.WriteEndObject();
                jsonWriter.WritePropertyName("token");
                jsonWriter.WriteValue(settings.writeToken);
                jsonWriter.WriteEndObject();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String BuildDeleteJSON(long videoID)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("method");
                jsonWriter.WriteValue("delete_video");
                jsonWriter.WritePropertyName("params");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("video_id");
                jsonWriter.WriteValue(videoID);
                jsonWriter.WritePropertyName("token");
                jsonWriter.WriteValue(settings.writeToken);
                jsonWriter.WriteEndObject();
            }
            return sb.ToString();
        }
    }

    public class PlayersManager
    {
        /// <summary>
        /// Player IDs are stored in s string variable comma seperated
        /// </summary>
        /// <param name="playerIDs"></param>
        /// <returns></returns>
        public static List<string> deserializeList(string playerIDs)
        {
            string[] playersArray = playerIDs.Split(new char[] { ',' });
            List<string> playersList = new List<string>(playersArray);
            return playersList;
        }
    }

    public class SettingsManager
    {
        public SettingsManager() { }
        public Settings settings = new Settings();
    }

    public class WidgetSettings
    {
        public static long widgetID;
    }

    public class Settings
    {
        public long publisherID;
        public string readToken;
        public string writeToken;
        public List<string> players;

        private const string settingsCacheKey = "BrightcoveWidgetSettingsCacheKey";
        private const string playerIDsCacheKey = settingsCacheKey + "PlayerIDs";
        private const string publisherIDCacheKey = settingsCacheKey + "PublisherID";
        private const string readTokenCacheKey = settingsCacheKey + "ReadToken";
        private const string writeTokenCacheKey = settingsCacheKey + "WriteToken";

        public Settings()
        {

            if (HttpRuntime.Cache[playerIDsCacheKey] == null
                || HttpRuntime.Cache[publisherIDCacheKey] == null
                || HttpRuntime.Cache[readTokenCacheKey] == null
                || HttpRuntime.Cache[writeTokenCacheKey] == null)
            {
                List<GlobalWidgetPropertySettings> globalWidgetData = new List<GlobalWidgetPropertySettings>();
                WidgetTypeData widgetData = new WidgetTypeData();
                WidgetTypeModel m_refWidgetModel = new WidgetTypeModel();

                if (m_refWidgetModel.FindGlobalSettingsByID(WidgetSettings.widgetID, out globalWidgetData))
                {
                    foreach (GlobalWidgetPropertySettings setting in globalWidgetData)
                    {
                        switch (setting.PropertyName)
                        {
                            case "PublisherID":
                                this.publisherID = (long)setting.value;
                                break;
                            case "ReadToken":
                                this.readToken = (string)setting.value;
                                break;
                            case "WriteToken":
                                this.writeToken = (string)setting.value;
                                break;
                            case "PlayerIDs":
                                this.players = PlayersManager.deserializeList((string)setting.value);
                                break;
                        }
                    }
                }
            }
            else
            {
                this.players = PlayersManager.deserializeList(HttpRuntime.Cache[playerIDsCacheKey].ToString());
                this.publisherID = (Int64)(HttpRuntime.Cache[publisherIDCacheKey]);
                this.readToken = HttpRuntime.Cache[readTokenCacheKey].ToString();
                this.writeToken = HttpRuntime.Cache[writeTokenCacheKey].ToString();
            }
        }
    }

    public class AuthenticationManager
    {
        public AuthenticationManager() { }

        public bool isRequestAuthorized()
        {
            CommonApi commonapi = new CommonApi();
            return commonapi.IsLoggedIn;
        }

        public BrightcoveResponse isPublisherValid(string publisherId)
        {
            throw new NotImplementedException();
        }

        public BrightcoveResponse isReadTokenValid(string readToken)
        {
            BrightcoveRequest bcRequest = new BrightcoveRequest(BrightcoveRequest.requestType.read);
            Dictionary<string, object> paramaters = new Dictionary<string, object>();
            paramaters.Add("command", "find_video_by_id");
            paramaters.Add("video_id", "1"); // null if doesn't exist - error if invalid
            paramaters.Add("token", readToken);
         
            BrightcoveResponse response = bcRequest.executeGET(paramaters);
            return response;
        }

        public BrightcoveResponse isWriteTokenValid(string writeToken)
        {
            // Waiting on BC API to be released
            throw new NotImplementedException();
        }

        public BrightcoveResponse isPlayerIdValid(string playerId)
        {
            // Waiting on BC API to be released
            throw new NotImplementedException();
        }
    }

    public class BrightcoveRequest
    {
        private string writeServiceURL = "http://api.brightcove.com/services/post";
        private string readServiceURL = "http://api.brightcove.com/services/library";
        private static readonly Encoding encoding = Encoding.UTF8;

        private requestType _requestType;

        public BrightcoveRequest(requestType reqType)
        {
            this._requestType = reqType;
        }

        private BrightcoveResponse executeRequest(Dictionary<string, object> parameters, byte[] postData, string contentType)
        {
            string baseURL = string.Empty;
            string requestMethod = "GET";
            string requestURL = string.Empty;
            switch (this._requestType)
            {
                case requestType.read:
                    baseURL = readServiceURL;
                    requestMethod = "GET";
                    requestURL = string.Format("{0}?{1}", baseURL, serializeParameters(parameters));
                    break;
                case requestType.write:
                    baseURL = writeServiceURL;
                    requestMethod = "POST";
                    requestURL = baseURL;
                    break;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestURL);
            request.Method = requestMethod;

            if (this._requestType == requestType.write && postData.Length > 0)
            {
                request.ContentLength = postData.Length;
                request.ContentType = contentType;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.15) Gecko/20110303 Firefox/3.6.15";// TODO
                request.CookieContainer = new CookieContainer();
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postData, 0, postData.Length);
                    requestStream.Close();
                }
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            TextReader textreader = new StreamReader(response.GetResponseStream());
            string responseData = textreader.ReadToEnd(); // null is an option

            BrightcoveResponse bcResponse = new BrightcoveResponse(responseData);
            return bcResponse;
        }

        public BrightcoveResponse executeGET(Dictionary<string, object> parameters)
        {
            return this.executeRequest(parameters, new byte[0], "");
        }


        public BrightcoveResponse executePOST(Dictionary<string, object> parameters)
        {
            string formDataBoundary = "-----------------------------28947758029299";
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = GetMultipartFormData(parameters, formDataBoundary);
            return this.executeRequest(parameters, formData, contentType);
        }

        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            foreach (var param in postParameters)
            {
                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;
                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, header.Length);
                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, postData.Length);
                }
            }
            // Add the end of the request
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);
            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();
            return formData;
        }

        private string serializeParameters(Dictionary<string, object> parameters)
        {
            string requestString = string.Empty;
            int i = 0;
            foreach (String key in parameters.Keys)
            {
                if (i > 0)
                {
                    requestString += "&";
                }
                requestString += String.Format("{0}={1}", key, HttpUtility.UrlEncode((string)parameters[key]));
                i++;
            }
            return requestString;
        }

        public FileParameter getFile(string filePath)
        {
            FileParameter file = new FileParameter();
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];
            byte[] fileData = new byte[postedFile.ContentLength];
            using (Stream fileStream = postedFile.InputStream)
            {
                fileStream.Read(fileData, 0, postedFile.ContentLength);
            }
            file = new FileParameter(fileData, postedFile.FileName);
            return file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fs"></param>
        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }

        public enum requestType
        {
            none = 0,
            read = 1,
            write = 2
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter() { }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }

    /// <summary>
    /// Server side proxy class for JSON responses
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BrightcoveResponse
    {
        [JsonProperty("success")]
        public bool success;
        [JsonProperty("data")]
        public string data = "";
        [JsonProperty("msg")]
        public string message = "";

        public BrightcoveResponse() { }

        public BrightcoveResponse(string rawResponse)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(rawResponse));
            reader.Read();

            // A valid response is null. If the api returns no data
            if (reader.Value == null && reader.TokenType == JsonToken.Null)
            {
                success = true;
                data = rawResponse;
            }

            while (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (reader.Value.ToString() == "error")
                    {
                        while (reader.Read())
                        {
                            if (reader.Value != null && reader.Value.ToString() == "message")
                            {
                                reader.Read();
                                message = reader.Value.ToString();
                                break;
                            }
                        }
                        success = false;
                        break;
                    }
                    else
                    {
                        success = true;
                        data = rawResponse;
                    }
                }
            }
        }

        public BrightcoveResponse(bool success, string msg)
        {
            this.success = success;
            this.message = msg;
        }

        public string toJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Server side proxy class for JSON requests
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRequest
    {
        [JsonProperty("widgetid")]
        public long widgetid = 0;
        [JsonProperty("action")]
        public string action = string.Empty;
        [JsonProperty("id")]
        public string id = string.Empty;
        [JsonProperty("title")]
        public string title = string.Empty;
        [JsonProperty("description")]
        public string description = string.Empty;
        [JsonProperty("refid")]
        public string refid = string.Empty;
        [JsonProperty("tags")]
        public string tags = string.Empty;
        [JsonProperty("fileurl")]
        public string fileurl = string.Empty;
        [JsonProperty("searchterm")]
        public string searchterm = string.Empty;
        [JsonProperty("searchtype")]
        public string searchtype = string.Empty;
        [JsonProperty("searchSort")]
        public string searchSort = string.Empty;
        [JsonProperty("sortOrder")]
        public string sortOrder = string.Empty;
        [JsonProperty("value")]
        public string value = string.Empty;
        [JsonProperty("readToken")]
        public string readToken = string.Empty;
        [JsonProperty("pagenumber")]
        public int pagenumber = 0;

    }

    /// <summary>
    /// Server side proxy class for JSON exceptions
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonException
    {
        [JsonProperty("error")]
        public bool error = true;
        [JsonProperty("message")]
        public string message = "";
        [JsonProperty("innerMessage")]
        public string innerMessage = "";

        public JsonException(String msg)
        {
            this.message = msg;
        }

        public JsonException(Exception exc)
        {
            this.message = exc.Message;
            if (exc.InnerException != null)
            {
                this.innerMessage = exc.InnerException.Message;
            }
        }

        public string toJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
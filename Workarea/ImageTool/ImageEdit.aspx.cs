using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.IO;

using Ektron.Cms.Workarea.Framework;

public partial class ImageTool_ImageEdit : WorkAreaBasePage
{
    private Ektron.Cms.CommonApi m_refAPI = new Ektron.Cms.CommonApi();
    private Ektron.Cms.Common.EkMessageHelper m_refMsg = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.RegisterWorkareaCssLink();
        this.RegisterDialogCssLink();
        Ektron.Cms.API.JS.RegisterJS(this, "../ContentDesigner/ekxbrowser.js", "ekxbrowserJS");
        Ektron.Cms.API.JS.RegisterJS(this, "../ContentDesigner/ekutil.js", "ekutilJS");
        Ektron.Cms.API.JS.RegisterJS(this, "../ContentDesigner/RadWindow.js", "RadWindowJS");
        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "InitializeRadWindow",
            "InitializeRadWindow();  self.focus();", true);

        m_refMsg = m_refAPI.EkMsgRef;
		Utilities.ValidateUserLogin();
        titlePage.Text = m_refMsg.GetMessage("lbl imagetool edit title");
        if (!Page.IsPostBack) {
            // Information from the URL parameters
            if (null != Request.Params.Get("s"))
            {
                if (null != Session["ImageToolNewFile"])
                {
                    // save image clicked
                    // update the thumbnail for the image
                    Ektron.Cms.ImageTool.ImageTool imager = new Ektron.Cms.ImageTool.ImageTool();
                    string imagefilepath = (string) Session["ImageToolNewFile"];
                    string thumbfilepath = imagefilepath;
                    if (thumbfilepath.ToLower().EndsWith(".gif"))
                    {
                        thumbfilepath = thumbfilepath.Substring(0, thumbfilepath.LastIndexOf(".")) + ".png";
                    }
                    thumbfilepath = thumbfilepath.Substring(0, thumbfilepath.LastIndexOf("\\") + 1)
                        + "thumb_"
                        + thumbfilepath.Substring(thumbfilepath.LastIndexOf("\\") + 1);
                    /**** THIS IS NOW DONE in ImageScore.aspx.cs on CommitPersistentCommand
                    try
                    {
                        imager.Resize(125, -1, true, imagefilepath, thumbfilepath);
                    }
                    catch (IOException)
                    {
                        // the thumbnail service might be already updating the thumbnail
                        // so just ignore any I/O exceptions
                    }
                     */
                    // copy image to any load balance locations
                    Ektron.Cms.Library.EkLibrary refLib = m_refAPI.EkLibraryRef;
                    foreach (Microsoft.VisualBasic.Collection libpath in refLib.GetAllLBPaths("images"))
                    {
                        string relLBpath = (string) libpath["LoadBalancePath"];
                        string realLBpath = Server.MapPath(relLBpath);
                        string imgfilename = imagefilepath.Substring(imagefilepath.LastIndexOf("\\")+1);
                        string thumbnailfname = thumbfilepath.Substring(thumbfilepath.LastIndexOf("\\")+1);
                        try
                        {
                            File.Copy(imagefilepath, realLBpath + imgfilename);
                        }
                        catch (Exception) {}
                        try
                        {
                            File.Copy(thumbfilepath, realLBpath + thumbnailfname);
                        }
                        catch (Exception) { }
                    }
                    // clear out the session variable holding the new filename
                    Session["ImageToolNewFile"] = null;
                    Session["ImageToolNewFileUrl"] = null;
                    Session["ImageToolOldLibId"] = null;
                    Session["ImageToolOldLibContentId"] = null;
                    Session["ImageToolNewLibFileName"] = null;
                    Session["ImageToolLibFolderId"] = null;
                    Session["ImageToolAssetName"] = null;
                }
            }
            else if (null != Request.Params.Get("c"))
            {
                // cancel, so delete the new image file
                // then delete the session variable holding the new filename
                if (null != Session["ImageToolNewFile"])
                {
                    // get rid of library entry for image
                    Ektron.Cms.ContentAPI refContApi = getContentAPI();
                    refContApi.DeleteLibraryItemForImageTool((string)Session["ImageToolNewFileUrl"]);
                    if ((Session["ImageToolAssetName"] != null) && !((string)Session["ImageToolAssetName"]).StartsWith("thumb_"))
                    {
                        refContApi.DeleteAssetForImageTool((string)Session["ImageToolAssetName"]);
                    }
                    // get rid of image file
                    string imagefilepath = (string)Session["ImageToolNewFile"];
                    if (File.Exists(imagefilepath))
                    {
                        try
                        {
                            File.Delete(imagefilepath);
                        }
                        catch (IOException) { }
                    }
                    string thumbfilepath = imagefilepath;
                    if (thumbfilepath.ToLower().EndsWith(".gif"))
                    {
                        thumbfilepath = thumbfilepath.Substring(0, thumbfilepath.LastIndexOf(".")) + ".png";
                    }
                    thumbfilepath = thumbfilepath.Substring(0, thumbfilepath.LastIndexOf("\\") + 1)
                        + "thumb_"
                        + thumbfilepath.Substring(thumbfilepath.LastIndexOf("\\") + 1);
                    // and the thumbnail
                    if (File.Exists(thumbfilepath))
                    {
                        try
                        {
                            File.Delete(thumbfilepath);
                        }
                        catch (IOException) { }
                    }
                }
                Session["ImageToolNewFile"] = null;
                Session["ImageToolNewFileUrl"] = null;
                Session["ImageToolOldLibId"] = null;
                Session["ImageToolOldLibContentId"] = null;
                Session["ImageToolNewLibFileName"] = null;
                Session["ImageToolLibFolderId"] = null;
                Session["ImageToolAssetName"] = null;
            }
            else if (null != Request.Params.Get("i"))
            {
                string strValue = Page.Server.UrlDecode(Request.Params.Get("i")).Trim();
                if (!strValue.Contains("://"))
                {
                    // see if we're editing a thumbnailed image which comes in w/o a full URL
                    strValue = Request.Url.Scheme + "://" + Request.Url.Host +
                        ((((Request.Url.Scheme == "http") && (Request.Url.Port != 80)) ||
                        ((Request.Url.Scheme == "https") && (Request.Url.Port != 443))) ?
                        (":" + Request.Url.Port) : "")
                        + strValue;
                }
                if (strValue.Contains("://"))
                {
                    URL url = new URL(strValue);
                    URL myURL = new URL(Page.Request.Url.ToString());
                    strValue = url.Path;
                    if (strValue.StartsWith(":"))
                    {
                        // work around asp.net bug where url.path includes port# for path
                        strValue = strValue.Substring(strValue.IndexOf("/"));
                    }

                    // get info for old library item
                    string origImagePath = strValue;
                    bool fEditingThumbnail = false;
                    bool fIsAssetImage = false;
                    Ektron.Cms.ContentAPI refContApi = getContentAPI();
                    Ektron.Cms.LibraryData library_data = refContApi.GetLibraryItemByUrl(strValue);
                    if ((library_data == null) && strValue.Contains("thumb_"))
                    {
                        // this is a thumbnail so try to locate the image associated with it
                        // since we don't store the thumbnail path in the DB
                        origImagePath = strValue.Replace("thumb_", "");
                        library_data = refContApi.GetLibraryItemByUrl(origImagePath);
                        if (library_data == null)
                        {
                            // thumbnails are PNG files so see if the original was a JPEG or GIF
                            string[] imageexts = { ".jpg", ".gif" };
                            foreach (string ext in imageexts)
                            {
                                string origImageGuess = origImagePath.Replace(".png", ext);
                                library_data = refContApi.GetLibraryItemByUrl(origImageGuess);
                                if (library_data != null)
                                {
                                    origImagePath = origImageGuess;
                                    break;
                                }
                            }
                        }
                        fEditingThumbnail = true;
                    }
                    if (library_data == null)
                    {
                        panelMessage.Visible = true;
                        lblMessage.Text = m_refMsg.GetMessage("err imagetool unable to find image") + strValue;
                        imagetool.Visible = false;
                        return;
                    }
                    long folderid = library_data.ParentId;
                    if ((library_data.ParentId == 0) && (library_data.ContentId != 0))
                    {
                        // this is a DMS asset, so look up the folder it belongs in
                        folderid = refContApi.GetFolderIdForContentId(library_data.ContentId);
                        fIsAssetImage = true;
                    }
                    Ektron.Cms.PermissionData security_data = refContApi.LoadPermissions(folderid, "folder", Ektron.Cms.ContentAPI.PermissionResultType.Common);
                    Ektron.Cms.PermissionData usersecurity_data = refContApi.LoadPermissions(refContApi.UserId, "users", Ektron.Cms.ContentAPI.PermissionResultType.All);
                    if (!security_data.CanAddToImageLib && !usersecurity_data.IsAdmin)
                    {
                        panelMessage.Visible = true;
                        lblMessage.Text = m_refMsg.GetMessage("err imagetool no library permission");
                        imagetool.Visible = false;
                        return;
                    }

                    // generate new filename
                    if ((url.HostName == myURL.HostName) || (url.HostName.ToLower() == "localhost"))
                    {
                        string strFilePath = null;
                        try
                        {
                            strFilePath = Server.MapPath(origImagePath);
                        }
                        catch (Exception)
                        {
                            panelMessage.Visible = true;
                            lblMessage.Text = m_refMsg.GetMessage("err imagetool non site image");
                            imagetool.Visible = false;
                            return;
                        }
                        string strNewFilePath = strFilePath;
                        if (File.Exists(strFilePath))
                        {
                            FileInfo fileinfo = new FileInfo(strFilePath);
                            // name the file "<oldname>.N.<extension>"
                            string origfname = fileinfo.Name;
                            string fname = origfname;
                            string newfname = fname;
                            do
                            {
                                string[] fnamepieces = fname.Split(new char[] { '.' });
                                newfname = fname;
                                if (fnamepieces.Length > 2)
                                {
                                    // loop until we find one that doesn't exist
                                    string strCurVer = fnamepieces[fnamepieces.Length - 2];
                                    string strExt = fnamepieces[fnamepieces.Length - 1];
                                    int newVer = int.Parse(strCurVer) + 1;
                                    newfname = fname.Replace("." + strCurVer + "." + strExt, "." + newVer + "." + strExt);
                                }
                                else if (fnamepieces.Length == 2)
                                {
                                    newfname = fnamepieces[0] + ".1." + fnamepieces[1];
                                }
                                else
                                {
                                    // not sure what to do w/ filename w/ no extension???
                                }
                                strNewFilePath = strNewFilePath.Replace(fname, newfname);
                                fileinfo = new FileInfo(strNewFilePath);
                                if (fileinfo.Exists)
                                {
                                    fname = fileinfo.Name;
                                }
                            } while (fileinfo.Exists);
                            if (strFilePath != strNewFilePath)
                            {
                                File.Copy(strFilePath, strNewFilePath);
                                strValue = origImagePath.Replace(origfname, newfname);
                                // save new filename so we can clean it up when user clicks on "cancel" button
                                Session["ImageToolNewFile"] = strNewFilePath;
                                // save URL so we can delete it from the library
                                Session["ImageToolNewFileUrl"] = strValue;
                                // save asset filename if we it's an asset
                                Session["ImageToolAssetName"] = null;
                                if (fIsAssetImage)
                                {
                                    Session["ImageToolAssetName"] = newfname;
                                }

                                if (library_data != null)
                                {
                                    // save tags and metadata for original image
                                    Ektron.Cms.TagData[] oldtags = null;
                                    Ektron.Cms.Community.TagsAPI refTagsApi = new Ektron.Cms.Community.TagsAPI();
                                    Ektron.Cms.ContentMetaData[] oldmeta = null;
                                    if (library_data.Id > 0)
                                    {
                                        Session["ImageToolOldLibId"] = library_data.Id;
                                        Session["ImageToolOldLibContentId"] = library_data.ContentId;
                                        oldtags = refTagsApi.GetTagsForObject(library_data.Id,
                                            Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Library,
                                            refContApi.ContentLanguage);
                                        if (library_data.ContentId != 0) {
                                            Ektron.Cms.ContentData content_data = refContApi.GetContentById(library_data.ContentId,
                                                Ektron.Cms.ContentAPI.ContentResultType.Published);
                                            if (content_data != null)
                                            {
                                                oldmeta = content_data.MetaData;
                                            }
                                        } else {
                                            oldmeta = library_data.MetaData;
                                        }
                                    }
                                    // create new image to edit
                                    library_data.Id = 0;
                                    if (fEditingThumbnail)
                                    {
                                        origfname = new FileInfo(origImagePath).Name;
                                        library_data.FileName = library_data.FileName.Replace(origfname, newfname);
                                    }
                                    else
                                    {
                                        library_data.FileName = library_data.FileName.Replace(origfname, newfname);
                                    }
                                    library_data.ParentId = folderid;
                                    Session["ImageToolNewLibFileName"] = library_data.FileName;
                                    Session["ImageToolLibFolderId"] = library_data.ParentId;
                                    long newLibId = refContApi.AddLibraryItemForImageTool(ref library_data);
                                    if (fIsAssetImage && !newfname.StartsWith("thumb_"))
                                    {
                                        // only need to add this for assets, not thumbnail of assets
                                        refContApi.AddAssetForImageTool(origfname, newfname);
                                    }
                                    // add original tags/metadata to new image
                                    if (oldtags != null) {
                                        foreach (Ektron.Cms.TagData tag in oldtags)
                                        {
                                            refTagsApi.AddTagToObject(tag.Id, newLibId,
                                                  Ektron.Cms.Common.EkEnumeration.CMSObjectTypes.Library,
                                                  -1, refContApi.ContentLanguage);
                                        }
                                    }
                                    if (oldmeta != null)
                                    {
                                         refContApi.UpdateLibraryMetadataByID(newLibId, oldmeta);
                                    }
                                }
                                
                                // can't show filenames or it'll be too long and the DMS filenames are ugly
                    		//titlePage.Text = m_refMsg.GetMessage("lbl imagetool edit title") + ": " + origfname;
                            }
                        }
                        imagetool.Edit(strValue);
                    }
                    else
                    {
                        panelMessage.Visible = true;
                        lblMessage.Text = m_refMsg.GetMessage("err imagetool non site image");
                        imagetool.Visible = false;
                    }
                }
            }
        }
    }

    private Ektron.Cms.ContentAPI getContentAPI()
    {
        Ektron.Cms.ContentAPI refContApi = new Ektron.Cms.ContentAPI();
        int m_intContentLanguage = -1;
        if ((Request.QueryString["LangType"] != null) &&
            (Request.QueryString["LangType"] != ""))
        {
            m_intContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            if (1 == m_intContentLanguage)
            {
                m_intContentLanguage = refContApi.DefaultContentLanguage;
            }
            refContApi.SetCookieValue("LastValidLanguageID", m_intContentLanguage.ToString());
        }
        else
        {
            if (refContApi.GetCookieValue("LastValidLanguageID") != "")
            {
                m_intContentLanguage = Convert.ToInt32(refContApi.GetCookieValue("LastValidLanguageID"));
                if (1 == m_intContentLanguage)
                {
                    m_intContentLanguage = refContApi.DefaultContentLanguage;
                }
            }
            else
            {
                m_intContentLanguage = refContApi.DefaultContentLanguage;
            }
        }
        refContApi.ContentLanguage = m_intContentLanguage;
        return refContApi;
    }
}


// from http://damieng.com/blog/2006/07/07/URL_parsing_and_manipulation_in_NET
public class URL : ICloneable, IComparable
{
    private const string schemeDecodeRegex = @"([^:]+):";
    private const string mailtoDecodeRegex = @"(mailto:)(([^@]+)@(.+))";
    private const string urlDecodeRegex = @"([^:]+)://(([^:@]+)(:([^@]+))?@)?([^:/?#]+)(:([d]+))?([^?#]+)?(\?([^#]+))?(#(.*))?";
    private URL baseUrl;
    private string scheme;
    private long port;
    private bool useDefaultPort;
    private string hostName;
    private string user;
    private string password;
    private string path;
    private NameValueCollection query;
    private string fragment;
    private bool relative;

    public URL()
    {
        Reset();
    }

    public URL(string url)
    {
        Reset();
        FullURL = url;
    }

    public URL(URL copyUrl)
    {
        Reset();
        CopyFrom(copyUrl);
    }

    public string Scheme
    {
        get { return scheme; }
        set { scheme = value.Trim(); }
    }

    public long Port
    {
        get { return port; }
        set
        {
            port = value;
            useDefaultPort = false;
        }
    }

    public bool UseDefaultPort
    {
        get { return useDefaultPort; }
        set { useDefaultPort = value; }
    }

    public string User
    {
        get { return user; }
        set { user = value; }
    }

    public string Password
    {
        get { return password; }
        set { password = value; }
    }

    public string HostName
    {
        get { return hostName; }
        set { hostName = value; }
    }

    public string Path
    {
        get { return path; }
        set { path = value; }
    }

    public NameValueCollection Query
    {
        get { return query; }
        set { query = value; }
    }

    public string Fragment
    {
        get { return fragment; }
        set { fragment = value; }
    }

    public string FullURL
    {
        get
        {
            if (Scheme.Equals("mailto"))
                return string.Format("{0}:{1}@{2}", Scheme, User, HostName);
            string newURL = string.Empty;
            if (!Relative)
            {
                newURL += Scheme + "://";
                if (User.Length > 0)
                {
                    newURL += User;
                    if (Password.Length > 0)
                        newURL += ":" + Password;
                    newURL += "@";
                }
                newURL += HostName;
                if (!UseDefaultPort)
                    newURL += ":" + Port;
            }
            newURL += Path;
            if (QueryString.Length > 0)
                newURL += "?" + QueryString;
            if (Fragment.Length > 0)
                newURL += "#" + Fragment;
            return newURL;
        }
        set
        {
            Reset();
            Match m = new Regex(schemeDecodeRegex).Match(value);
            if (m.Success)
                if (m.Groups[1].Captures[0].Value.ToLower().Equals("mailto"))
                    DecodeMailTo(value);
                else
                    DecodeURL(value);
        }
    }

    public bool Relative
    {
        get { return relative; }
        set { relative = value; }
    }

    public string QueryString
    {
        get
        {
            string newQueryString = string.Empty;
            for (int queryIdx = 0; queryIdx < Query.Count; queryIdx++)
            {
                newQueryString += (queryIdx == 0 ? "" : "&") + Query.Keys[queryIdx];
                if (Query[queryIdx].Length > 0)
                    newQueryString += "=" + Query[queryIdx];
            }
            return newQueryString;
        }
        set
        {
            Query.Clear();
            AppendQueryString(value);
        }
    }

    public URL BaseUrl
    {
        get { return baseUrl; }
        set { baseUrl = value; }
    }

    public void AppendQueryString(string newQueryString)
    {
        string[] pairs = newQueryString.Split('&');
        for (int pairIdx = 0; pairIdx < pairs.Length; pairIdx++)
        {
            string pair = pairs[pairIdx];
            int keyPos = pair.IndexOf('=');
            if (keyPos > 0)
            {
                string key = pair.Substring(0, keyPos);
                string value = pair.Substring(keyPos + 1);
                query[key] = value;
            }
            else
                query[pair] = string.Empty;
        }
    }

    public void Reset()
    {
        Scheme = string.Empty;
        Port = 0;
        UseDefaultPort = true;
        HostName = string.Empty;
        User = string.Empty;
        Password = string.Empty;
        Path = string.Empty;
        Query = new NameValueCollection();
        Fragment = string.Empty;
        Relative = false;
    }

    public void CopyFrom(URL copyUrl)
    {
        Scheme = copyUrl.Scheme;
        User = copyUrl.User;
        Password = copyUrl.Password;
        HostName = copyUrl.HostName;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != this.GetType()) return false;
        return (FullURL == ((URL)obj).FullURL);
    }

    public override int GetHashCode()
    {
        return FullURL.GetHashCode();
    }

    public override string ToString()
    {
        return FullURL;
    }

    private void DecodeURL(string value)
    {
        Match m = new Regex(urlDecodeRegex).Match(value);
        if (m.Success)
        {
            if (m.Groups[1].Captures.Count == 1)
                Scheme = m.Groups[1].Captures[0].Value;
            if (m.Groups[4].Captures.Count == 1)
                User = m.Groups[4].Captures[0].Value;
            if (m.Groups[5].Captures.Count == 1)
                Password = m.Groups[5].Captures[0].Value;
            if (m.Groups[6].Captures.Count == 1)
                HostName = m.Groups[6].Captures[0].Value;
            if (m.Groups[8].Captures.Count == 1)
                Port = Int32.Parse(m.Groups[8].Captures[0].Value);
            if (m.Groups[9].Captures.Count == 1)
                Path = m.Groups[9].Captures[0].Value;
            if (m.Groups[11].Captures.Count == 1)
                QueryString = m.Groups[11].Captures[0].Value;
            if (m.Groups[13].Captures.Count == 1)
                Fragment = m.Groups[13].Captures[0].Value;
        }
    }

    private void DecodeMailTo(string value)
    {
        Match m = new Regex(mailtoDecodeRegex).Match(value);
        if (m.Success)
        {
            if (m.Groups[1].Captures.Count == 1)
                Scheme = m.Groups[1].Captures[0].Value;
            if (m.Groups[2].Captures.Count == 1)
                User = m.Groups[2].Captures[0].Value;
            if (m.Groups[3].Captures.Count == 1)
                HostName = m.Groups[3].Captures[0].Value;
        }
    }

    public object Clone()
    {
        URL newClone = (URL)this.MemberwiseClone();
        newClone.Query = new NameValueCollection(Query);
        return newClone;
    }

    public int CompareTo(object obj)
    {
        if (obj == this) return 0;
        if (!(obj is URL)) return -1;
        return ((URL)obj).FullURL.CompareTo(FullURL);
    }
}

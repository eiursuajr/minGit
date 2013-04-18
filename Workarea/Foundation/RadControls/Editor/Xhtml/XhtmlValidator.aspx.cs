using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
// Ektron Editor starts
using System.Threading;
using System.Xml;
// Ektron Editor ends


namespace Ektron.Telerik.WebControls.RadEditorUtils
{
    /// <summary>
    /// Summary description for XhtmlValidator.
    /// </summary>
    public partial class EkXhtmlValidator : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (this.IsPostBack)
            {
                string oValue = EditorContent.Value;
                string doctype = EditorDoctype.Value != string.Empty ?
                    EditorDoctype.Value :
                    "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">";

                RadEditor editor = new RadEditor();

                editor.Html = oValue;

                try
                {
                    oValue = editor.Xhtml;
                }
                catch (Exception)
                {
                }

                // Ektron Editor starts
                string content = "";
                if ("" == EditorDoctype.Value)//
                {
                    Disclaimer.InnerText = "";
                    string editorcontent = EditorContent.Value.ToLower();
                    if (editorcontent.IndexOf("<li>") > -1)
                    {
                        content = EditorContent.Value;
                    }
                    else //editorcontent is an empty html with HTML and BODY tags.
                    {
                        content = "<html><body> The content is valid. </body></html>";
                    }
                }
                else if ("reportresult=" == EditorDoctype.Value.Substring(0,13))
                {
                    string baseHRef = EditorDoctype.Value.Substring(13);
                    if (baseHRef.Substring(0, 4).ToLower() != "http")
                    {
                        baseHRef = "http://" + baseHRef;
                    }
                    content = oValue;
                    content = content.Replace("</head>", "<base href=\"" + baseHRef + "\"/> </head>");
                }
                else
                {
                    content =
                    "<html xmlns=\"http://www.w3.org/1999/xhtml\"> <head> <title>Validation Content</title><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />" +
                    "</head> <body> " +
                    oValue +
                    " </body></html>";

                    HTTPSend oSend;
                    string myStr;
                    string myStrLower;
                    string name = "";
                    string onlineId = "";
                    string src = "";
                    string baseHRef = "";
                    string docTypeSpec = "";

                    string xmlPath = new Uri(Request.Url, "../../../../ContentDesigner/ValidateSpec.xml").AbsoluteUri;
                    XmlTextReader reader = new XmlTextReader(xmlPath);
                    XmlDocument docxml = new XmlDocument();
                    docxml.Load(reader);
                    XmlNodeList xmlnode = docxml.GetElementsByTagName("online");
                    for (int i = 0; i < xmlnode.Count; i++)
                    {
                        // get all the attributes from the ValidateSpec.xml
                        XmlAttributeCollection xmlattrc = xmlnode[i].Attributes;
                        for (int j = 0; j < xmlattrc.Count; j++)
                        {
                            switch (xmlattrc[j].Name)
                            {
                                case "id":
                                    onlineId = xmlattrc[j].Value;
                                    break;
                                case "name":
                                    name = xmlattrc[j].Value;
                                    break;
                                case "src":
                                    src = xmlattrc[j].Value;
                                    break;
                                case "site":
                                    baseHRef = xmlattrc[j].Value;
                                    if (baseHRef.Substring(0, 4).ToLower() != "http")
                                    {
                                        baseHRef = "http://" + baseHRef;
                                    }
                                    break;
                                case "doctype":
                                    docTypeSpec = "<" + xmlattrc[j].Value.Replace("&qout;", "\"") + ">";
                                    docTypeSpec = docTypeSpec.Replace("<>", "");
                                    break;
                                default:
                                    break;
                            }
                        }
                        // if the current validation is the one selected, post the page.
                        if (doctype == onlineId || doctype == docTypeSpec)	   
                        {
                            oSend = new HTTPSend(src);
                            for (int k = 0; k < xmlnode[i].ChildNodes.Count; k++)
                            {
                                XmlAttributeCollection xmlchildattrc = xmlnode[i].ChildNodes[k].Attributes;
                                oSend.SetField(xmlchildattrc[0].Value, xmlnode[i].ChildNodes[k].InnerText);
                            }
                            content = docTypeSpec + content;
                            oSend.SendTextAsFile(content, "RadEditorContent.html");
                            myStr = oSend.ResponseText.ToString();
                            myStrLower = myStr.ToLower();
                            if (myStrLower.IndexOf("</head>") > -1)
                            {
                                content = myStr.Replace("</head>", "<base href=\"" + baseHRef + "\" /> </head>");
                            }
                            sBaseTag.Text = "<base href=\"" + baseHRef + "\" />";
                            Disclaimer.InnerHtml = "<b>Note: This module uses " + baseHRef + " to validate content for Accessibility compliance.</b>";
                        }
                    }
                }
                Disclaimer.Visible = true;
                divReportPane.InnerHtml = content;
                // Ektron Editor ends
            }
        }

        /// <summary>
        /// Allow the transfer of data files using the W3C's 
        /// specification for HTTP multipart form data. 
        /// Microsoft's version has a bug where it does not 
        /// format the ending boundary correctly.
        /// </summary>    

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion
    }

    /// <summary>
    /// Allow the transfer of data files using the W3C's 
    /// specification for HTTP multipart form data. 
    /// Microsoft's version has a bug where it does not 
    /// format the ending boundary correctly.
    /// </summary>
    //public class HTTPSend  // The Class is inherited as part of RadEditor.Net2.DLL So this commented out.
    //{

    //    /// <summary>
    //    /// Transmits a file to the web server stated 
    //    /// in the URL property. 
    //    /// You may call this several times and it will 
    //    /// use the values previously set for fields and URL.
    //    /// </summary>
    //    /// <param name="Filename">The local path of 
    //    /// the file to send.</param>

    //    public void SendTextAsFile(string content, string Filename)
    //    {
    //        // create the request object
    //        coRequest = (HttpWebRequest)WebRequest.Create(URL);

    //        // set the appropriate properties
    //        coRequest.ProtocolVersion = TransferHttpVersion;
    //        coRequest.Method = "POST";
    //        coRequest.ContentType = "multipart/form-data; boundary=" + BeginBoundary;
    //        coRequest.Headers.Add("Cache-Control", "no-cache");
    //        coRequest.KeepAlive = _KeepAlive;

    //        //            ServicePointManager.Expect100Continue = _Expect100;
    //        coRequest.Pipelined = _Pipelined;
    //        coRequest.SendChunked = _Chunked;

    //        // add credential, if present
    //        if (Credentials != null)
    //        {
    //            coRequest.Credentials = Credentials;
    //        }

    //        // add a certificate, if present
    //        if (Certificate != null)
    //        {
    //            coRequest.ClientCertificates.Add(Certificate);
    //        }

    //        // obtain string representation of form 
    //        // fields, file header, file trailer
    //        string strFields = GetFormFields();
    //        string strFileHeader = GetFileHeader(Filename);
    //        string strFileTrailer = GetFileTrailer();

    //        // get file length information
    //        // FileInfo Info = new FileInfo(Filename);

    //        Byte[] contentBytes = Encoding.UTF8.GetBytes(content);//Moved up to calculate proper length

    //        // calculte content length
    //        coRequest.ContentLength = strFields.Length +
    //            strFileHeader.Length +
    //            strFileTrailer.Length +

    //            contentBytes.Length;
    //        //content.Length;//!
    //        //Info.Length;

    //        Stream s = null;

    //        // write the data to the stream
    //        try
    //        {
    //            s = GetStream();
    //            //Ektron Editor Starts
    //            Random RandomClass = new Random();
    //            int randomSleepTime = RandomClass.Next(1, 3000);
    //            if (null == s)
    //            {
    //                Thread.Sleep(randomSleepTime);
    //                s = coRequest.GetRequestStream();
    //            }
    //            //Ektron Eidtor Ends

    //            WriteString(s, strFields);
    //            WriteString(s, strFileHeader);
    //            //WriteFile(s, Filename);
    //            //Byte[] contentBytes = Encoding.UTF8.GetBytes(content);//Moved up to calculate proper length
    //            s.Write(contentBytes, 0, contentBytes.Length);

    //            WriteString(s, strFileTrailer);

    //            GetResponse();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            if (s != null)
    //            {
    //                s.Close();
    //            }
    //        }
    //        coRequest = null;
    //    }

    //    public void SendFile(string Filename)
    //    {
    //        // create the request object
    //        coRequest = (HttpWebRequest)WebRequest.Create(URL);

    //        // set the appropriate properties
    //        coRequest.ProtocolVersion = TransferHttpVersion;
    //        coRequest.Method = "POST";
    //        coRequest.ContentType =
    //            "multipart/form-data; boundary=" +
    //            BeginBoundary;

    //        coRequest.Headers.Add("Cache-Control", "no-cache");
    //        coRequest.KeepAlive = _KeepAlive;

    //        //            ServicePointManager.Expect100Continue = _Expect100;
    //        coRequest.Pipelined = _Pipelined;
    //        coRequest.SendChunked = _Chunked;

    //        // add credential, if present
    //        if (Credentials != null)
    //        {
    //            coRequest.Credentials = Credentials;
    //        }

    //        // add a certificate, if present
    //        if (Certificate != null)
    //        {
    //            coRequest.ClientCertificates.Add(Certificate);
    //        }

    //        // obtain string representation of form 
    //        // fields, file header, file trailer
    //        string strFields = GetFormFields();
    //        string strFileHeader = GetFileHeader(Filename);
    //        string strFileTrailer = GetFileTrailer();

    //        // get file length information
    //        FileInfo Info = new FileInfo(Filename);

    //        // calculte content length
    //        coRequest.ContentLength = strFields.Length +
    //            strFileHeader.Length +
    //            strFileTrailer.Length +
    //            Info.Length;

    //        Stream s = null;

    //        // write the data to the stream
    //        try
    //        {
    //            s = GetStream();

    //            WriteString(s, strFields);
    //            WriteString(s, strFileHeader);
    //            WriteFile(s, Filename);
    //            WriteString(s, strFileTrailer);

    //            GetResponse();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        finally
    //        {
    //            if (s != null)
    //            {
    //                s.Close();
    //            }
    //        }
    //        coRequest = null;
    //    }



    //    // Holds any form fields and values that you wish 
    //    // to transfer with your data.
    //    private Hashtable coFormFields = null;

    //    // Used mainly to avoid passing parameters to 
    //    // other routines.
    //    // Could have been local to sendFile().
    //    private HttpWebRequest coRequest = null;

    //    // Used if we are testing and want to output the 
    //    // raw request, minus http headers, out to a file.
    //    private System.IO.Stream coFileStream = null;

    //    // Defined to build the form field data that is 
    //    // being passed along with the request.
    //    private const string CONTENT_DISP =
    //        "Content-Disposition: form-data; name=";

    //    #region Public Properties and Property Variables

    //    private Version coHttpVersion = null;
    //    /// <summary>
    //    /// Allows you to specify the specific version 
    //    /// of HTTP to use for uploads.
    //    /// The dot NET stuff currently does not allow 
    //    /// you to remove the continue-100 header
    //    /// from 1.1 and 1.0 currently has a bug in it 
    //    /// where it adds the continue-100. 
    //    /// MS has sent a patch to remove the 
    //    /// continue-100 in HTTP 1.0.
    //    /// </summary>
    //    public Version TransferHttpVersion
    //    {
    //        get
    //        {
    //            return coHttpVersion;
    //        }
    //        set
    //        {
    //            coHttpVersion = value;
    //        }
    //    }

    //    private string coFileContentType = null;
    //    /// <summary>
    //    /// Used to change the content type of the file 
    //    /// being sent.
    //    /// Currently defaults to: text/xml. Other options 
    //    /// are text/plain or binary.
    //    /// </summary>
    //    public string FileContentType
    //    {
    //        get
    //        {
    //            return coFileContentType;
    //        }
    //        set
    //        {
    //            coFileContentType = value;
    //        }
    //    }

    //    private string _BeginBoundary = null;
    //    /// <summary>
    //    /// The string that defines the begining boundary 
    //    /// of our multipart transfer as defined in the 
    //    /// w3c specs.
    //    /// This method also sets the Content and Ending 
    //    /// boundaries as defined by the w3c specs.
    //    /// </summary>
    //    public string BeginBoundary
    //    {
    //        get
    //        {
    //            return _BeginBoundary;
    //        }
    //        set
    //        {
    //            _BeginBoundary = value;
    //            ContentBoundary = "--" + BeginBoundary;
    //            EndingBoundary = ContentBoundary + "--";
    //        }
    //    }

    //    private string _ContentBoundary = null;
    //    /// <summary>
    //    /// The string that defines the content boundary 
    //    /// of our multipart transfer as defined in the 
    //    /// w3c specs.
    //    /// </summary>
    //    public string ContentBoundary
    //    {
    //        get
    //        {
    //            return _ContentBoundary;
    //        }
    //        set
    //        {
    //            _ContentBoundary = value;
    //        }
    //    }

    //    private string _EndingBoundary = null;
    //    /// <summary>
    //    /// The string that defines the ending boundary 
    //    /// of our multipart transfer as defined in the 
    //    /// w3c specs.
    //    /// </summary>
    //    public string EndingBoundary
    //    {
    //        get
    //        {
    //            return _EndingBoundary;
    //        }
    //        set
    //        {
    //            _EndingBoundary = value;
    //        }
    //    }

    //    private StringBuilder _ResponseText = null;
    //    /// <summary>
    //    /// The data returned to us after the transfer 
    //    /// is completed.
    //    /// </summary>
    //    public StringBuilder ResponseText
    //    {
    //        get
    //        {
    //            return _ResponseText;
    //        }
    //        set
    //        {
    //            _ResponseText = value;
    //        }
    //    }

    //    private string _URL = null;
    //    /// <summary>
    //    /// The web address of the recipient of the 
    //    /// transfer.
    //    /// </summary>
    //    public string URL
    //    {
    //        get
    //        {
    //            return _URL;
    //        }
    //        set
    //        {
    //            _URL = value;
    //        }
    //    }

    //    private int _BufferSize = 0;
    //    /// <summary>
    //    /// Allows us to determine the size of the buffer 
    //    /// used to send a piece of the file at a time 
    //    /// out the IO stream. 
    //    /// Defaults to 1024 * 10.
    //    /// </summary>
    //    public int BufferSize
    //    {
    //        get
    //        {
    //            return _BufferSize;
    //        }
    //        set
    //        {
    //            _BufferSize = value;
    //        }
    //    }

    //    private ICredentials _Credentials = null;
    //    /// <summary>
    //    /// Allows us to specified the credentials used 
    //    /// for the transfer.
    //    /// </summary>
    //    public ICredentials Credentials
    //    {
    //        get
    //        {
    //            return _Credentials;
    //        }
    //        set
    //        {
    //            _Credentials = value;
    //        }
    //    }

    //    private X509Certificate _Certificate = null;
    //    /// <summary>
    //    /// Allows us to specifiy the certificate to use 
    //    /// for secure communications.
    //    /// </summary>
    //    public X509Certificate Certificate
    //    {
    //        get
    //        {
    //            return _Certificate;
    //        }
    //        set
    //        {
    //            _Certificate = value;
    //        }
    //    }

    //    private bool _KeepAlive = false;
    //    /// <summary>
    //    /// Gets or sets a value indicating whether to 
    //    /// make a persistent connection to the 
    //    /// Internet resource.
    //    /// </summary>
    //    public bool KeepAlive
    //    {
    //        get
    //        {
    //            return _KeepAlive;
    //        }
    //        set
    //        {
    //            _KeepAlive = value;
    //        }
    //    }

    //    private bool _Expect100 = false;
    //    /// <summary>
    //    /// Gets or sets a value indicating whether the 
    //    /// Expect100-Continue header should be sent.
    //    /// </summary>
    //    public bool Expect100
    //    {
    //        get
    //        {
    //            return _Expect100;
    //        }
    //        set
    //        {
    //            _Expect100 = value;
    //        }
    //    }

    //    private bool _Pipelined = false;
    //    /// <summary>
    //    /// Gets or sets a value indicating whether to 
    //    /// pipeline the request to the Internet resource.
    //    /// </summary>
    //    public bool Pipelined
    //    {
    //        get
    //        {
    //            return _Pipelined;
    //        }
    //        set
    //        {
    //            _Pipelined = value;
    //        }
    //    }

    //    private bool _Chunked = false;
    //    /// <summary>
    //    /// Gets or sets a value indicating whether the 
    //    /// file can be sent in smaller packets.
    //    /// </summary>
    //    public bool Chunked
    //    {
    //        get
    //        {
    //            return _Chunked;
    //        }
    //        set
    //        {
    //            _Chunked = value;
    //        }
    //    }

    //    #endregion

    //    #region Public Methods

    //    /// <summary>
    //    /// Initialize our class for use to 
    //    /// send data files.
    //    /// </summary>
    //    /// <param name="URL">The URL of the 
    //    /// destination server.</param>
    //    //public HTTPSend(string URL)
    //    //{
    //    //    this.URL = URL;

    //    //    // set all default values
    //    //    coFormFields = new Hashtable();
    //    //    ResponseText = new StringBuilder();
    //    //    BufferSize = 1024 * 10;
    //    //    BeginBoundary =
    //    //        "ou812--------------8c405ee4e38917c";
    //    //    TransferHttpVersion = HttpVersion.Version11;
    //    //    FileContentType = "text/html";//TO DO: WAS XML. Will allow us to let the user choose! Maybe even choose transitional/strict/whatever???
    //    //}

    //    /// <summary>
    //    /// Used to signal we want the output to go to a 
    //    /// text file verses being transfered to a URL.
    //    /// </summary>
    //    /// <param name="Path">The local path to the 
    //    /// output file.</param>
    //    public void SetFilename(string Path)
    //    {
    //        coFileStream = new FileStream(Path,
    //            FileMode.Create,
    //            FileAccess.Write);
    //    }

    //    /// <summary>
    //    /// Allows you to add some additional field data 
    //    /// to be sent along with the transfer. 
    //    /// This is usually used for things like userid 
    //    /// and password to validate the transfer.
    //    /// </summary>
    //    /// <param name="Name">The name of the 
    //    /// custom field.</param>
    //    /// <param name="Value">The value of the 
    //    /// custom field.</param>
    //    public void SetField(string Name, string Value)
    //    {
    //        coFormFields[Name] = Value;
    //    }

    //    /// <summary>
    //    /// Allows you to add some additional header data 
    //    /// to be sent along with the transfer. 
    //    /// </summary>
    //    /// <param name="Name">The name of the custom header.</param>
    //    /// <param name="Value">The value of the custom header.</param>
    //    public void SetHeader(string Name, string Value)
    //    {
    //        coRequest.Headers.Add(Name, Value);
    //    }

    //    /// <summary>
    //    /// Determines if we have a file stream set, and 
    //    /// returns either the HttpWebRequest stream or 
    //    /// the file.
    //    /// </summary>
    //    /// <returns>Either the HttpWebRequest stream or 
    //    /// the local output file.</returns>
    //    public Stream GetStream()
    //    {
    //        Stream s = null;

    //        //s = new FileStream(HttpContext.Current.Server.MapPath("~/Xhtml.txt"),System.IO.FileMode.OpenOrCreate);            
    //        //return s;


    //        if (coFileStream == null)
    //        {
    //            // get stream from HTTPRequest
    //            s = coRequest.GetRequestStream();

    //        }
    //        else
    //        {
    //            // get stream from local file
    //            s = coFileStream;
    //        }

    //        return s;
    //    }

    //    /// <summary>
    //    /// Make the request to the web server and 
    //    /// retrieve it's response into a text buffer.
    //    /// </summary>
    //    public void GetResponse()
    //    {
    //        // are we using a local output file?
    //        if (coFileStream == null)
    //        {
    //            // using HTTP Request stream

    //            Stream s = null;
    //            WebResponse r = null;

    //            try
    //            {
    //                // get the response from the server
    //                r = coRequest.GetResponse();
    //            }
    //            catch (WebException webex)
    //            {
    //                r = webex.Response;
    //            }

    //            if (r != null)
    //            {
    //                // get the resonse as a stream
    //                s = r.GetResponseStream();

    //                StreamReader sr = new StreamReader(s);

    //                string str = null;

    //                ResponseText.Length = 0;

    //                str = sr.ReadLine();

    //                // read the response data and 
    //                // place in ResponseText 
    //                while (str != null)
    //                {
    //                    ResponseText.Append(str);
    //                    str = sr.ReadLine();
    //                }

    //                r.Close();
    //            }
    //            else
    //            {
    //                throw new Exception("HTTPSend: Error retrieving server response");
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Builds the proper format of the multipart 
    //    /// data that contains the form fields and 
    //    /// their respective values.
    //    /// </summary>
    //    /// <returns>All form fields, properly formatted 
    //    /// in a string.</returns>
    //    public string GetFormFields()
    //    {
    //        string str = "";

    //        IDictionaryEnumerator myEnumerator =
    //            coFormFields.GetEnumerator();

    //        // process each field
    //        /*
    //        while(myEnumerator.MoveNext())
    //        {
    //            str += ContentBoundary + "\n" + 
    //                CONTENT_DISP + "\"" + 
    //                myEnumerator.Key.ToString() + 
    //                "\"\n\n" + 
    //                myEnumerator.Value.ToString() + 
    //                "\n";
    //        }
    //        */
    //        while (myEnumerator.MoveNext())
    //        {
    //            str += ContentBoundary + Environment.NewLine +
    //                CONTENT_DISP + "\"" +
    //                myEnumerator.Key.ToString() +
    //                "\"" + Environment.NewLine + Environment.NewLine +
    //                myEnumerator.Value.ToString() +
    //                Environment.NewLine;
    //        }


    //        return str;
    //    }

    //    /// <summary>
    //    /// Returns the proper content information for 
    //    /// the file we are sending.
    //    /// </summary>
    //    /// <param name="Filename">The local path to 
    //    /// the file that should be sent.</param>
    //    /// <returns>All file headers, properly formatted 
    //    /// in a string.</returns>
    //    public string GetFileHeader(string Filename)
    //    {
    //        /*
    //        return ContentBoundary + 
    //            "\n" + 
    //            CONTENT_DISP + 
    //            "\"uploaded_file\"; filename=\"" + 
    //            Path.GetFileName(Filename) + 
    //            "\"\n" + "Content-type: " + 
    //            FileContentType + 
    //            "\n\n";
    //          */
    //        return ContentBoundary +
    //            Environment.NewLine +
    //            CONTENT_DISP +
    //            "\"uploaded_file\"; filename=\"" +
    //            Path.GetFileName(Filename) +
    //            "\"" + Environment.NewLine + "Content-type: " +
    //            FileContentType +
    //            Environment.NewLine + Environment.NewLine;
    //    }

    //    /// <summary>
    //    /// Creates the proper ending boundary for the 
    //    /// multipart upload.
    //    /// </summary>
    //    /// <returns>The ending boundary.</returns>
    //    public string GetFileTrailer()
    //    {
    //        // return "\n" + EndingBoundary;
    //        return Environment.NewLine + EndingBoundary;
    //    }

    //    /// <summary>
    //    /// Mainly used to turn the string into a byte 
    //    /// buffer and then write it to our IO stream.
    //    /// </summary>
    //    /// <param name="Output">The stream to write to.</param>
    //    /// <param name="Data">The data to place into the stream.</param>
    //    public void WriteString(Stream Output, string Data)
    //    {
    //        byte[] PostData =
    //            System.Text.Encoding.ASCII.GetBytes(Data);

    //        Output.Write(PostData, 0, PostData.Length);
    //    }

    //    /// <summary>
    //    /// Reads in the file a chunck at a time then 
    //    /// sends it to the output stream.
    //    /// </summary>
    //    /// <param name="Output">The stream to write to.</param>
    //    /// <param name="Filename">The local path of the file to send.</param>
    //    public void WriteFile(Stream Output, string Filename)
    //    {
    //        FileStream Input = new FileStream(Filename,
    //            FileMode.Open, FileAccess.Read);

    //        Input.Seek(0, SeekOrigin.Begin);

    //        byte[] FileData = new byte[BufferSize];

    //        int Bytes = Input.Read(FileData, 0, BufferSize);

    //        while (Bytes > 0)
    //        {
    //            Output.Write(FileData, 0, Bytes);
    //            Bytes = Input.Read(FileData, 0, BufferSize);
    //        }

    //        Input.Close();
    //    }


    //    #endregion
    //}
    }

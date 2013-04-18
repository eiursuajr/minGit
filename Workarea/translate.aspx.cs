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
using Ektron.Cms.UI.CommonUI;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class translate : System.Web.UI.Page
{
    protected string htmleditor = "";
    protected string htmcontent = "";
    protected ContentAPI contentAPI = new ContentAPI();
    protected EkMessageHelper _messageHelper;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        _messageHelper = contentAPI.EkMsgRef;
        if (contentAPI.RequestInformationRef.IsMembershipUser == 1 || contentAPI.RequestInformationRef.UserId == 0)
        {
            Response.Redirect(contentAPI.ApplicationPath + "reterror.aspx?info=" + Server.UrlEncode(_messageHelper.GetMessage("msg login cms user")), false);
            return;
        } 
        //Put user code to initialize the page here
        this.pageTitle.Text = (new ApplicationAPI()).EkMsgRef.GetMessage("ektron translation");
        htmleditor = this.Request.Form["htmleditor"];
        htmcontent = this.Request.Form["mycontent"];

        // also run Tidy on the text
        TidyNet.Tidy objTidy = new TidyNet.Tidy();
        objTidy.Options.BreakBeforeBR = true;
        objTidy.Options.CharEncoding = TidyNet.CharEncoding.UTF8;
        objTidy.Options.DocType = TidyNet.DocType.Omit;
        objTidy.Options.DropEmptyParas = false;
        objTidy.Options.MakeClean = true;
        objTidy.Options.NumEntities = true;
        objTidy.Options.QuoteAmpersand = true;
        objTidy.Options.QuoteMarks = false;
        objTidy.Options.QuoteNbsp = true;
        objTidy.Options.RawOut = false;
        objTidy.Options.TidyMark = false;
        objTidy.Options.Word2000 = true;
        objTidy.Options.XmlOut = true;
        TidyNet.TidyMessageCollection messageCollection = new TidyNet.TidyMessageCollection();
        System.IO.MemoryStream streamIn = new System.IO.MemoryStream();
        System.IO.MemoryStream streamOut = new System.IO.MemoryStream();
        byte[] byteArray = Encoding.UTF8.GetBytes(htmcontent);
        streamIn.Write(byteArray, 0, byteArray.Length);
        streamIn.Position = 0;
        objTidy.Parse(streamIn, streamOut, messageCollection);
        streamOut.Position = 0;
        string strTidyResult = Encoding.UTF8.GetString(streamOut.ToArray());
        streamOut.Close();
        streamIn.Close();
        if ((strTidyResult == "") && (messageCollection.Errors > 0))
        {
            foreach (TidyNet.TidyMessage msg in messageCollection)
            {
                if (msg.Level == TidyNet.MessageLevel.Error)
                {
                    strTidyResult = strTidyResult + msg.ToString() + "<br />";
                }
            }

            htmcontent = strTidyResult;
            content.Value = htmcontent;
        }
        else
        {
            strTidyResult = (string)(System.Text.RegularExpressions.Regex.Replace(strTidyResult, "[\\w\\W]*?<body>", "").Replace("</body>" + "\r\n" + "</html>", ""));
            content.Value = strTidyResult;
        }
    }
}



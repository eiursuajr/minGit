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
using System.Net.Mail;
using System.Net;
using System.Text;

public partial class Workarea_diagnostics_sendMail : System.Web.UI.Page
{
    //----- Constants for page.
    private const int ERR_DEFAULT = 0x0001;
    private const int ERR_NOT_LOCAL = 0x0002;

    private const string ERR_DEFAULT_RESPONSE = "Error while loading EkStatus send mail utility";
    private const string ERR_NOT_LOCAL_RESPONSE = "EkStatus diagnostic utility is only accessible from the local machine.";

    private bool m_bMailSentOk = false;
    private string m_sLastError = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder saPage = new StringBuilder();

        if (!this.Context.Request.IsLocal)
        {
            saPage.Append(LoadError(ERR_NOT_LOCAL));
        }
        else
        {
            if (!this.Page.IsPostBack)
            {
                SmtpClient sendClient = null;
                MailMessage message = null;
                Attachment attach = null;

                try
                {
                    sendClient = new SmtpClient(Page.Session["smtpServer"].ToString(), Convert.ToInt32(Page.Session["smtpPort"].ToString()));
                    message = new MailMessage(Page.Session["emailAddressFrom"].ToString(), Page.Session["emailAddressTo"].ToString(), "From EkStatus", "");
                    attach = new Attachment(HttpContext.Current.Server.MapPath("") + "\\EkStatusOutput.htm");

                    message.Body = "Comments from sender:\r\n";

                    if (Page.Session["emailComments"] != null)
                    {
                        message.Body += Page.Session["emailComments"].ToString();
                    }

                    message.Body += "\r\n\r\nReport attached from " + Server.MachineName + "\r\n\r\nPlease save this report in workarea\\diagnostics\\ directory.";

                    if (Page.Session["smtpUser"].ToString() != "Value not set in configuration")
                    {
                        NetworkCredential netCred = new NetworkCredential(Page.Session["smtpUser"].ToString(), Page.Session["smtpPass"].ToString());
                        sendClient.Credentials = netCred;
                        netCred = null;
                    }

                    message.Attachments.Add(attach);
                    sendClient.Send(message);
                    m_bMailSentOk = true;
                }
                catch (Exception exThrown)
                {
                    m_sLastError = exThrown.Message;
                    m_bMailSentOk = false;
                }
                finally
                {

                    //----- Cleanup
                    if (message != null)
                    {
                        message.Dispose();
                        message = null;
                    }
                    if (attach != null)
                    {
                        attach.Dispose();
                        attach = null;
                    }
                    if (sendClient != null)
                    {
                        sendClient = null;
                    }
                }
            }
        }

        saPage.Append(LoadHeader());
        
        if(m_bMailSentOk)
            saPage.AppendLine("       <p>Mail sent successfully.&nbsp;&nbsp;You will be redirected in 5 seconds.</p>");
        else
            saPage.Append(LoadError(ERR_DEFAULT, m_sLastError));

        saPage.Append(LoadFooter());

        if (Page.Session["smtpServer"] != null)
            Page.Session.Remove("smtpServer");
        if (Page.Session["smtpPort"] != null)
            Page.Session.Remove("smtpPort");
        if (Page.Session["emailAddressTo"] != null)
            Page.Session.Remove("emailAddressTo");
        if (Page.Session["emailAddressFrom"] != null)
            Page.Session.Remove("emailAddressFrom");
        if (Page.Session["emailComments"] != null)
            Page.Session.Remove("emailComments");

        this.Response.Write(saPage.ToString());
    }

    private string LoadHeader()
    {
        StringBuilder sbReturn = new StringBuilder();

        sbReturn.AppendLine("<%%>");
        sbReturn.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
        sbReturn.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
        sbReturn.AppendLine("<head runat=\"server\">");
        sbReturn.AppendLine("   <title>CMS400.NET Diagnostic Panel</title>");
        sbReturn.AppendLine("   <link rel='stylesheet' type='text/css' href='com.ektron.diagnostics.ui.css'/>");
        sbReturn.AppendLine("   <link rel='stylesheet' type='text/css' href='com.ektron.ui.tabs.css'/>");
        
        if(m_bMailSentOk)
            sbReturn.AppendLine("   <meta http-equiv=\"Refresh\" content=\"5; url=\"status.aspx\">");

        sbReturn.AppendLine("   <script type='text/javascript' language=\"javascript\" src='com.ektron.ui.tabs.js'></script>");
        sbReturn.AppendLine("</head>");
        sbReturn.AppendLine("<body>");
        sbReturn.AppendLine("   <img src=\"./images/cms400-25.gif\" class=\"logo\" alt=\"cms400.net Diagnostic Panel\"/>");
        sbReturn.AppendLine("   <div style=\"background:#ccc;width:100%;height:1px;overflow:hidden;\"></div>");
         if (this.Context.Request.IsLocal)
         {
			sbReturn.AppendLine("   <div style=\"margin-bottom:25px;font-size:10pt;\"><b>CMS400.NET</b> Diagnostics (<a href=\"http://" + Server.MachineName + "\" target=\"_blank\">" + Server.MachineName + "</a>)</div>");
		 }
        sbReturn.AppendLine("   <div id=\"tabsContainer\"></div>");

        return sbReturn.ToString();
    }

    //----- Creates the dynamic page footer.
    private string LoadFooter()
    {
        StringBuilder sbReturn = new StringBuilder();

        sbReturn.AppendLine("<a href=\"status.aspx\">Return <a> to diagnostic page.");
        sbReturn.AppendLine("</body>");
        sbReturn.AppendLine("</html>");

        return sbReturn.ToString();
    }

    //----- Used to load any error and/or error text into the aspx page.
    private string LoadError(int nErrorCode, string sMessage)
    {
        StringBuilder sbReturn = new StringBuilder();

        sbReturn.AppendLine("   <div id=\"error\">");

        switch (nErrorCode)
        {
            case ERR_DEFAULT:
                {
                    sbReturn.AppendLine("       " + ERR_DEFAULT_RESPONSE);
                    break;
                }
            case ERR_NOT_LOCAL:
                {
                    sbReturn.AppendLine("       " + ERR_NOT_LOCAL_RESPONSE);
                    break;
                }
            default:
                {
                    sbReturn.AppendLine("       " + ERR_DEFAULT_RESPONSE);
                    break;
                }
        }

        if (sMessage.Length > 0)
            sbReturn.AppendLine("       <p>" + sMessage + "</p>");

        sbReturn.AppendLine("   </div>");

        return sbReturn.ToString();
    }

    //----- Overloaded so a string does not have to be specified to load an error.
    private string LoadError(int nErrorCode)
    {
        return LoadError(nErrorCode, "");
    }
}

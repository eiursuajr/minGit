using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using Ektron.Cms.Workarea;

public partial class threadeddisc_notifications : workareabase
{
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        base.Title = "Attachments";
        base.SetTitleBarToString("Choose a file to attach");
        ltr_addfile.Text = "Add the file to the list";
        ltr_currentfiles.Text = "Current File Attachments";

        ltr_uploadmsg.Text = "Uploading File - Please Wait";
        ltr_error.Text = "Please select a file to attach";

        // ul_file.Attributes.Add("onchange", "alert(this.value);")
        cmd_attach.Attributes.Add("onclick", "javascript:return checkntoggle(document.getElementById(\'dvHoldMessage\'),document.getElementById(\'dvErrorMessage\'));");
        cmd_close.Attributes.Add("onclick", "javascript:self.close(); return false;");
		
		Utilities.ValidateUserLogin();
        GenerateJS();
        if (!Page.IsPostBack)
        {
            GetFromParent();
        }
    }


    protected void cmd_attach_Click(object sender, System.EventArgs e)
    {
        if (!(ul_file.PostedFile == null))
        {
            // file was sent
            HttpPostedFile myFile = ul_file.PostedFile;

            // Get size of uploaded file
            int nFileLen = myFile.ContentLength;

            // Allocate a buffer for reading of the file
            byte[] myData = new byte[nFileLen + 1];

            // Read uploaded file from the Stream
            myFile.InputStream.Read(myData, 0, nFileLen);

            WriteToFile((string)("C:\\ektron\\engineering\\CMS\\400_ThreadedDiscussionR2\\WEBSRC\\WorkArea\\forumuploads\\" + myFile.FileName), myData);

            //ltr_filelist.Text += Path.GetFileName(myFile.FileName) & " (" & myFile.ContentLength.ToString() & " Bytes)<br />"
            ltr_filelist.Text += "<div><img alt=\"Image\" src=\"" + this.AppImgPath + "doc.gif\" border=\"0\">&nbsp;<a href=\"\" target=\"_blank\">" + Path.GetFileName(myFile.FileName) + " (" + myFile.ContentLength.ToString() + " Bytes)</a></div>";

            ReUpParent();
        }
        else
        {
            // no file
        }
    }

    // Writes file to current folder
    private void WriteToFile(string strPath, byte[] Buffer)
    {

        // Create a file
        FileStream newFile = new FileStream(strPath, FileMode.Create);

        // Write data to the file
        newFile.Write(Buffer, 0, Buffer.Length);

        // Close file
        newFile.Close();
    }

    private void GenerateJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javaScript\" type=\"text/javascript\">").Append(Environment.NewLine);
        sbJS.Append("	function checkntoggle(me, you){").Append(Environment.NewLine);
        sbJS.Append("		var bProceed = false; ").Append(Environment.NewLine);
        sbJS.Append("		var ofile = document.getElementById(\'" + ul_file.UniqueID + "\'); ").Append(Environment.NewLine);
        sbJS.Append("		if ( (ofile.type == \'file\') && (ofile.value != \'\') ) { ").Append(Environment.NewLine);
        sbJS.Append("		    bProceed = true; ").Append(Environment.NewLine);
        sbJS.Append("		} ").Append(Environment.NewLine);
        sbJS.Append("		if (bProceed == true){").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"visible\";").Append(Environment.NewLine);
        sbJS.Append("			you.style.visibility=\"hidden\";").Append(Environment.NewLine);
        sbJS.Append("			}").Append(Environment.NewLine);
        sbJS.Append("		else {").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"hidden\";").Append(Environment.NewLine);
        sbJS.Append("			you.style.visibility=\"visible\";").Append(Environment.NewLine);
        sbJS.Append("	    }").Append(Environment.NewLine);
        sbJS.Append("		return bProceed;").Append(Environment.NewLine);
        sbJS.Append("	}").Append(Environment.NewLine);
        sbJS.Append("</script>").Append(Environment.NewLine);

        LiteralControl ltr_de_js = new LiteralControl(sbJS.ToString());
        if (!(Page.Header == null))
        {
            Page.Header.Controls.Add(ltr_de_js);
        }
    }

    private void ReUpParent()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("var ofilelist = window.opener.document.getElementById(\"ek_attachfiles\");").Append(Environment.NewLine);
        sbJS.Append("if (ofilelist != null) {").Append(Environment.NewLine);
        sbJS.Append("    ofilelist.innerHTML = \'" + MakeJSSafe((string)ltr_filelist.Text) + "\';").Append(Environment.NewLine);
        sbJS.Append("}").Append(Environment.NewLine);

        ltr_bottomjs.Text = sbJS.ToString();
    }
    private void GetFromParent()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("var ofilelist = window.opener.document.getElementById(\"ek_attachfiles\");").Append(Environment.NewLine);
        sbJS.Append("if (ofilelist != null && ofilelist.innerHTML != \'\') {").Append(Environment.NewLine);
        sbJS.Append("    document.getElementById(\'ek_filelist\').innerHTML = ofilelist.innerHTML; ").Append(Environment.NewLine);
        sbJS.Append("}").Append(Environment.NewLine);

        ltr_bottomjs.Text = sbJS.ToString();
    }

    private string MakeJSSafe(string JS)
    {
        JS = JS.Replace("\'", "\\\'");
        JS = JS.Replace(Environment.NewLine, "\\n");
        return JS;
    }
}

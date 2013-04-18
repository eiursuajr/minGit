using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Workarea;

public partial class threadeddisc_attachments : workareabase
{

    protected long iboardid = 0;
    protected long iforumid = 0;
    protected long itopicid = 0;
    protected DiscussionBoard _board;
    protected PermissionData security_data;
    protected bool bError = false;

    protected void Page_PreLoad(object sender, System.EventArgs e)
    {

        if ((Request.QueryString["b"] != null) && (Request.QueryString["fid"] != null))
        {
            iboardid = Convert.ToInt64(Request.QueryString["b"]);
            iforumid = Convert.ToInt64(Request.QueryString["fid"]);
            if (Request.QueryString["t"] != null)
            {
                itopicid = Convert.ToInt64(Request.QueryString["t"]);
            }
            if (itopicid != 0)
            {
                _board = m_refContentApi.GetTopic(itopicid);
                iforumid = _board.Forums[0].Id;
            }
            else
            {
                _board = m_refContentApi.GetForum(iforumid);
            }
            ltr_allowedext.Text = m_refMsg.GetMessage("lbl allowed extensions") + "&nbsp;" + "&nbsp;" + _board.AcceptedExtensions.Replace(",", " ");
        }
        else
        {
            // exception out
        }
        security_data = this.m_refContentApi.LoadPermissions(iboardid, "folder", 0);
        if (!(security_data.IsAdmin == true || security_data.CanAddToFileLib == true))
        {
            Utilities.ShowError("You do not have permission.");
        }
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        RegisterResources();
        cmd_remove.Text = m_refMsg.GetMessage("btn minus");
        cmd_remove.ToolTip = cmd_remove.Text;
        cmd_attach.Text = m_refMsg.GetMessage("btn attach");
        cmd_attach.ToolTip = cmd_attach.Text;
        cmd_close.Text = m_refMsg.GetMessage("close title");
        cmd_close.ToolTip = cmd_close.Text;
        base.Title = m_refMsg.GetMessage("lbl attachments");
        base.SetTitleBarToString(m_refMsg.GetMessage("lbl forum choose a file to attach"));
        ltr_addfile.Text = m_refMsg.GetMessage("lbl forum add the file to the list");
        ltr_currentfiles.Text = m_refMsg.GetMessage("lbl forum current file attachments");

        ltr_uploadmsg.Text = m_refMsg.GetMessage("lbl forum uploading file - please wait");
        ltr_error.Text = m_refMsg.GetMessage("lbl forum please select a file to attach");

        if (Page.IsPostBack) // And (hdn_ofilelist.Value <> "" Or (Request.ServerVariables("http_user_agent").ToLower().IndexOf("msie") > -1 And Request.Form("hdn_ieval") <> "")) Then
        {
            ltr_filelist.Text = Request.Form["hdn_ieval"];
            hdn_ofilelist.Value = "";
        }


        cmd_attach.Attributes.Add("onclick", "javascript:return checkntoggle(document.getElementById(\'dvHoldMessage\'),document.getElementById(\'dvErrorMessage\'));");
        cmd_remove.Attributes.Add("onclick", "javascript:delattach();");
        cmd_close.Attributes.Add("onclick", "javascript:self.close(); return false;");

        GenerateJS();
        if (!Page.IsPostBack)
        {
            GetFromParent();
        }
    }

    protected void cmd_attach_Click(object sender, System.EventArgs e)
    {
        try
        {
            if (!(ul_file.PostedFile == null))
            {
                long iFolder = iforumid;
                Ektron.Cms.LibraryConfigData lib_settings_data;
                lib_settings_data = this.m_refContentApi.GetLibrarySettings(iFolder);

                // file was sent
                HttpPostedFile myFile = ul_file.PostedFile;
                string sFileExt = "";
                // Get and check size of uploaded file
                int nFileLen = myFile.ContentLength;
                if (nFileLen > _board.MaxFileSize)
                {
                    throw (new Exception("File is too large. There is a " + _board.MaxFileSize.ToString() + " byte limit."));
                }
                //get and check name and extension
                string sFileName = myFile.FileName;
                string sShortName = "";
                if (myFile.FileName.IndexOf("\\") > -1)
                {
                    string[] aFilename = myFile.FileName.Split('\\');
                    // take the very last one
                    if (aFilename.Length > 0)
                    {
                        sFileName = aFilename[aFilename.Length - 1];
                    }
                }
                sFileName = sFileName.Replace("\'", "").Replace("#", "_"); // make safe
                string[] aFileExt = sFileName.Split('.');
                if (aFileExt.Length > 1)
                {
                    sFileExt = (string)(aFileExt[(aFileExt.Length - 1)].Trim().ToLower()); //use the LAASSTT one.
                    sShortName = sFileName.Substring(0, System.Convert.ToInt32(sFileName.Length - (sFileExt.Length + 1)));
                }
                else
                {
                    throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
                }
                aFileExt = _board.AcceptedExtensions.Split(',');
                if (aFileExt.Length > 0)
                {
                    bool bGo = false;
                    for (int i = 0; i <= (aFileExt.Length - 1); i++)
                    {
                        if (sFileExt == aFileExt[i].Trim().ToLower())
                        {
                            bGo = true;
                            break;
                        }
                    }
                    if (bGo == false)
                    {
                        throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
                    }
                }
                else
                {
                    throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
                }

                // Allocate a buffer for reading of the file
                byte[] myData = new byte[nFileLen - 1 + 1];

                // Read uploaded file from the Stream
                myFile.InputStream.Read(myData, 0, nFileLen);

                //check for existence of file.
                FileInfo CheckFile;
                int iUnqueNameIdentifier = 0;
                string sWebPath = lib_settings_data.FileDirectory;
                if (Ektron.Cms.Common.EkFunctions.IsImage(System.IO.Path.GetExtension(sFileName)))
                {
                    sWebPath = lib_settings_data.ImageDirectory;
                }
                string sPhysicalPath = Server.MapPath(sWebPath);
                if (!System.IO.Directory.Exists(sPhysicalPath))
                {
                    Ektron.Cms.API.Folder folderApi = new Ektron.Cms.API.Folder();
                    folderApi.CreateFolder(sPhysicalPath);
                }
                CheckFile = new FileInfo(sPhysicalPath + sFileName);
                if (CheckFile.Exists)
                {
                    while (CheckFile.Exists)
                    {
                        iUnqueNameIdentifier++;
                        sFileName = sShortName + "(" + iUnqueNameIdentifier + ")." + sFileExt;
                        CheckFile = new FileInfo(sPhysicalPath + sFileName);
                    }
                }

                //write
                WriteToFile(sPhysicalPath + sFileName, myData);
                //----------------- Load Balance ------------------------------------------------------
                LoadBalanceData[] loadbalance_data;
                loadbalance_data = base.m_refContentApi.GetAllLoadBalancePathsExtn(iFolder, "files");
                if (!(loadbalance_data == null))
                {
                    for (int j = 0; j <= loadbalance_data.Length - 1; j++)
                    {
                        sPhysicalPath = Server.MapPath(loadbalance_data[j].Path);
                        if ((sPhysicalPath.Substring(sPhysicalPath.Length - 1, 1) != "\\"))
                        {
                            sPhysicalPath = sPhysicalPath + "\\";
                        }
                        WriteToFile(sPhysicalPath + sFileName, myData);
                    }
                }

                //record to db
                Ektron.Cms.FileAttachment faAttach = new Ektron.Cms.FileAttachment();
                faAttach.DoesExist = true;
                faAttach.Filename = sFileName;
                faAttach.Filepath = sWebPath + sFileName;
                faAttach.FileSize = int.Parse(myFile.ContentLength.ToString());
                faAttach.MimeType = myFile.ContentType;
                if (this.m_iID > 0)
                {
                    faAttach.ReplyID = this.m_iID;
                }
                faAttach = m_refContentApi.EkContentRef.AddFileAttachment(faAttach);

                ltr_filelist.Text += "<div id=\"ekfile_" + faAttach.ID.ToString() + "\" onMouseOver=\"doselect(this,\'over\',\'" + lib_settings_data.FileDirectory + faAttach.Filename + "\');\" onMouseOut=\"doselect(this,\'out\',\'" + lib_settings_data.FileDirectory + faAttach.Filename + "\');\" onClick=\"doselect(this,\'toggle\',\'" + lib_settings_data.FileDirectory + faAttach.Filename + ("\');\"><img alt=\"Attachment\" src=\"" + this.AppImgPath + "doc.gif\" border=\"0\">&nbsp;") + Path.GetFileName(sFileName) + " (" + myFile.ContentLength.ToString() + " bytes) <input type=\"hidden\" name=\"ek_attachedfile_id_" + faAttach.ID.ToString() + "\" id=\"ek_attachedfile_id_" + faAttach.ID.ToString() + "\" value=\"" + faAttach.ID.ToString() + "\" /></div>";

                ReUpParent();
            }
            else
            {
                throw (new Exception("No File"));
            }
        }
        catch (Exception ex)
        {
            ltr_error.Text = ex.Message;
            //ltr_bottomjs.Text &= "<script language=""javaScript"" type=""text/javascript"">" & Environment.NewLine
            ltr_bottomjs.Text += "	justtoggle(document.getElementById(\'dvErrorMessage\'), true);" + Environment.NewLine;
            //ltr_bottomjs.Text &= "</script>" & Environment.NewLine
            bError = true;
            //Utilities.ShowError(ex.Message)
        }
    }

    // Writes file to current folder
    private void WriteToFile(string strPath, byte[] Buffer)
    {
        try
        {
            // Create a file
            FileStream newFile = new FileStream(strPath, FileMode.Create);

            // Write data to the file
            newFile.Write(Buffer, 0, Buffer.Length);
            newFile.Flush();
            // Close file
            newFile.Close();
        }
        catch (Exception)
        {
            Utilities.ShowError("File Cannot Be Copied");
        }
    }

    private void GenerateJS()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("<script language=\"javaScript\" type=\"text/javascript\">").Append(Environment.NewLine);

        sbJS.Append("	function justtoggle(eElem, toshow){").Append(Environment.NewLine);
        sbJS.Append("	    if (toshow == true) {eElem.style.visibility = \"visible\";}").Append(Environment.NewLine);
        sbJS.Append("	    else {eElem.style.visibility=\"hidden\"; }").Append(Environment.NewLine);
        sbJS.Append("	}").Append(Environment.NewLine);

        sbJS.Append("	function checkntoggle(me, you){").Append(Environment.NewLine);
        sbJS.Append("		var bProceed = false; ").Append(Environment.NewLine);
        sbJS.Append("		var ofile = document.getElementById(\'" + ul_file.UniqueID + "\'); ").Append(Environment.NewLine);
        sbJS.Append("		if ( (ofile.type == \'file\') && (ofile.value != \'\') ) { ").Append(Environment.NewLine);
        sbJS.Append("		    bProceed = true; ").Append(Environment.NewLine);
        sbJS.Append("		} ").Append(Environment.NewLine);
        sbJS.Append("		if (bProceed == true){").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"visible\";").Append(Environment.NewLine);
        sbJS.Append("			you.style.visibility=\"hidden\";").Append(Environment.NewLine);
        sbJS.Append("			document.getElementById(\'hdn_ieval\').value = document.getElementById(\'ek_filelist\').innerHTML; ").Append(Environment.NewLine);
        sbJS.Append("			}").Append(Environment.NewLine);
        sbJS.Append("		else {").Append(Environment.NewLine);
        sbJS.Append("			me.style.visibility=\"hidden\";").Append(Environment.NewLine);
        sbJS.Append("			you.style.visibility=\"visible\";").Append(Environment.NewLine);
        sbJS.Append("	    }").Append(Environment.NewLine);
        sbJS.Append("		return bProceed;").Append(Environment.NewLine);
        sbJS.Append("	}").Append(Environment.NewLine);

        sbJS.Append("	function doselect(eAttach, smode, urllocation){").Append(Environment.NewLine);
        sbJS.Append("		if (smode == \'toggle\') { ").Append(Environment.NewLine);
        sbJS.Append("		    if (eAttach.style.backgroundColor == \'gainsboro\') { ").Append(Environment.NewLine);
        sbJS.Append("		        eAttach.style.backgroundColor = \'#FFFFFF\'; ").Append(Environment.NewLine);
        sbJS.Append("		    } else { //if (eAttach.style.backgroundColor == \'#FFFFFF\') { ").Append(Environment.NewLine);
        sbJS.Append("		        eAttach.style.backgroundColor = \'gainsboro\'; ").Append(Environment.NewLine);
        sbJS.Append("		    } ").Append(Environment.NewLine);
        sbJS.Append("		} else if (smode == \'over\') { ").Append(Environment.NewLine);
        sbJS.Append("		    //eAttach.style.backgroundColor = \'gainsboro\'; ").Append(Environment.NewLine);
        sbJS.Append("		} else if (smode == \'out\') { ").Append(Environment.NewLine);
        sbJS.Append("		    //eAttach.style.backgroundColor = \'#FFFFFF\'; ").Append(Environment.NewLine);
        sbJS.Append("		} ").Append(Environment.NewLine);
        sbJS.Append("	}").Append(Environment.NewLine);

        sbJS.Append("	function delattach(){").Append(Environment.NewLine);
        sbJS.Append("		var cContain = document.getElementById(\'ek_filelist\'); ").Append(Environment.NewLine);
        sbJS.Append("		if (cContain.childNodes != null && cContain.childNodes.length > 0) {  ").Append(Environment.NewLine);
        sbJS.Append("		    for (i = 0; i < cContain.childNodes.length; i++) { ").Append(Environment.NewLine);
        sbJS.Append("		        if (cContain.childNodes[i].nodeName == \'DIV\') { ").Append(Environment.NewLine);
        sbJS.Append("		            if (cContain.childNodes[i].attributes.length > 3) { ").Append(Environment.NewLine);
        sbJS.Append("		                var aStyle = cContain.childNodes[i].attributes[\'style\']; ").Append(Environment.NewLine);
        sbJS.Append("		                var iloc = -1;").Append(Environment.NewLine);
        sbJS.Append("		                var sekfileIdentity = \'\';").Append(Environment.NewLine);
        sbJS.Append("		                    sekfileIdentity = cContain.childNodes[i].attributes[\'id\'].nodeValue; ").Append(Environment.NewLine);
        sbJS.Append("		                if (aStyle != null && aStyle.nodeValue != null) { // non IE ").Append(Environment.NewLine);
        sbJS.Append("		                    iloc = aStyle.nodeValue.indexOf(\'gainsboro\');").Append(Environment.NewLine);
        sbJS.Append("		                    if (iloc > -1) { ").Append(Environment.NewLine);
        sbJS.Append("		                        document.getElementById(sekfileIdentity).innerHTML = \'\'; ").Append(Environment.NewLine);
        sbJS.Append("		                    } ").Append(Environment.NewLine);
        sbJS.Append("		                } else if (aStyle != null && aStyle.nodeValue == null) { // IE").Append(Environment.NewLine);
        sbJS.Append("		                    eFile = document.getElementById(sekfileIdentity);").Append(Environment.NewLine);
        sbJS.Append("		                    iloc = eFile.style.backgroundColor.indexOf(\'gainsboro\');").Append(Environment.NewLine);
        sbJS.Append("		                    if (iloc > -1) { ").Append(Environment.NewLine);
        sbJS.Append("		                        document.getElementById(sekfileIdentity).outerHTML = \'\'; ").Append(Environment.NewLine);
        sbJS.Append("		                        i = i - 1; ").Append(Environment.NewLine);
        sbJS.Append("		                    } ").Append(Environment.NewLine);
        sbJS.Append("		                } ").Append(Environment.NewLine);
        sbJS.Append("		            } ").Append(Environment.NewLine);
        sbJS.Append("		        } ").Append(Environment.NewLine);
        sbJS.Append("		    } ").Append(Environment.NewLine);
        sbJS.Append("		} ").Append(Environment.NewLine);
        sbJS.Append("       var ofilelist = window.opener.document.getElementById(\"ek_attachfiles\");").Append(Environment.NewLine);
        sbJS.Append("       if (ofilelist != null) {").Append(Environment.NewLine);
        sbJS.Append("           ofilelist.innerHTML = cContain.innerHTML;").Append(Environment.NewLine);
        sbJS.Append("           document.getElementById(\'" + hdn_ofilelist.UniqueID + "\').value = cContain.innerHTML;").Append(Environment.NewLine);
        sbJS.Append("           document.getElementById(\'hdn_ieval\').value = cContain.innerHTML;").Append(Environment.NewLine);
        sbJS.Append("       }").Append(Environment.NewLine);
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
    private void ClearJS()
    {
        ltr_bottomjs.Text = "";
    }
    private void GetFromParent()
    {
        StringBuilder sbJS = new StringBuilder();

        sbJS.Append("var ofilelist = window.opener.document.getElementById(\"ek_attachfiles\");").Append(Environment.NewLine);
        sbJS.Append("if (ofilelist != null && ofilelist.innerHTML != \'\') {").Append(Environment.NewLine);
        sbJS.Append("    document.getElementById(\'ek_filelist\').innerHTML = ofilelist.innerHTML; ").Append(Environment.NewLine);
        sbJS.Append("    document.getElementById(\'" + hdn_ofilelist.UniqueID + "\').value = ofilelist.innerHTML; ").Append(Environment.NewLine);
        sbJS.Append("}").Append(Environment.NewLine);

        ltr_bottomjs.Text = sbJS.ToString();
    }

    private string MakeJSSafe(string JS)
    {
        JS = JS.Replace("\'", "\\\'");
        JS = JS.Replace(Environment.NewLine, "\\n");
        return JS;
    }

    protected void cmd_remove_Click(object sender, System.EventArgs e)
    {
        //ReUpParent()
        ClearJS();
    }
    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
}

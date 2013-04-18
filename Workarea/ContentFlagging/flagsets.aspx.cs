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
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
//using Ektron.Cms.Common.EkFunctions;



public partial class ContentFlagging_flagsets : workareabase
{

    #region Page Variables

    protected PermissionData security_data;
    protected FlagDefData[] aFlagSets;
    protected FlagDefData fdFlagDef = new FlagDefData();
    protected bool bFlagEditor = false;
    protected bool bAdmin = false;
    private LocalizationAPI objLocalizationApi = null;
    private bool AddLink = true;
    string communityflagaction = "";
    #endregion

    #region Page Functions

    protected void Page_Init(object sender, System.EventArgs e)
    {
        Utilities.SetLanguage(m_refContentApi);
        this.security_data = this.m_refContentApi.LoadPermissions(0, "content", 0);
        bAdmin = this.security_data.IsAdmin;
        bFlagEditor = this.m_refContentApi.IsARoleMember(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor, m_refContentApi.UserId, false);
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        RegisterResources();

        try
        {
            if ((Request.QueryString["communityonly"] != null) && Request.QueryString["communityonly"] == "true")
            {
                communityflagaction = "communityonly=true&";
            }
            if (this.security_data.IsAdmin == true)
            {
                if (Page.IsPostBack)
                {
                    switch (this.m_sPageAction)
                    {
                        case "addedit":
                            this.Process_AddEdit();
                            break;
                    }
                }
                else
                {
                    switch (this.m_sPageAction)
                    {
                        case "addedit":
                            this.AddEdit();
                            break;
                        case "remove":
                            this.Process_Remove();
                            break;
                        default: //view
                            this.ViewAll();
                            break;
                    }
                }
                this.SetLabels();
                this.SetJS();
            }
            else
            {
                throw (new Exception(this.GetMessage("err flagset no access")));
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    #endregion

    #region Display

    public void AddEdit()
    {

        if (this.m_iID > 0)
        {
            fdFlagDef = this.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(this.m_iID, true);
        }

        ltr_options.Text = "<input type=\"hidden\" id=\"Flaglength\" name=\"Flaglength\" value=\"" + fdFlagDef.Items.Length.ToString() + "\" /><div id=\"pFlag\" name=\"pFlag\">" + Environment.NewLine;
        string sIndent = "&nbsp;";
        if (this.bAdmin || this.bFlagEditor)
        {
            for (int i = 0; i <= (fdFlagDef.Items.Length - 1); i++)
            {
                ltr_options.Text += "<script type=\"text/javascript\">addFlagInit(" + fdFlagDef.Items[i].ID.ToString() + ",\'" + fdFlagDef.Items[i].Name + "\');</script>";
                ltr_options.Text += "<input type=\"hidden\" name=\"flag_iden" + i.ToString() + "\" id=\"flag_iden" + i.ToString() + "\" value=\"" + fdFlagDef.Items[i].ID.ToString() + "\" /> ";
                ltr_options.Text += sIndent + sIndent + "<input type=\"text\" id=\"flagdefopt" + i.ToString() + "\" name=\"flagdefopt" + i.ToString() + "\" value=\"" + (fdFlagDef.Items[i].Name) + "\" maxlength=\"50\" size=\"35\" onChange=\"javascript:saveFlag(" + i.ToString() + ",this.value,\'fname\');\">";
                if (i == (fdFlagDef.Items.Length - 1))
                {
                    ltr_options.Text += sIndent + sIndent + "<img src=\"" + this.AppImgPath + "movedown_disabled.gif\"/>";
                }
                else
                {
                    ltr_options.Text += sIndent + sIndent + "<a href=\"#\" onclick=\"javascript:moveFlag(" + i.ToString() + ",\'down\'); return false;\"><img src=\"" + this.AppImgPath + "movedown.gif\"/></a>";
                }
                if (i == 0)
                {
                    ltr_options.Text += sIndent + sIndent + "<img src=\"" + this.AppImgPath + "moveup_disabled.gif\"/>";
                }
                else
                {
                    ltr_options.Text += sIndent + sIndent + "<a href=\"#\" onclick=\"javascript:moveFlag(" + i.ToString() + ",\'up\'); return false;\"><img src=\"" + this.AppImgPath + "moveup.gif\"/></a>";
                }
                ltr_options.Text += sIndent + sIndent + ("<a href=\"#\" onclick=\"javascript:removeFlag(\'" + i.ToString() + "\'); return false;\"><img src=\"") + m_refContentApi.RequestInformationRef.ApplicationPath + "images/UI/Icons/delete.png\"/></a>";
                ltr_options.Text += "<br/>";
            }
            ltr_options.Text += sIndent + sIndent + "<a href=\"#\" onclick=\"javascript:addFlag(); return false;\"><img src=\"" + m_refContentApi.RequestInformationRef.ApplicationPath + "images/UI/Icons/add.png\"/></a><br/>";
        }
        else
        {
            for (int i = 0; i <= (fdFlagDef.Items.Length - 1); i++)
            {
                ltr_options.Text += sIndent + sIndent + "<input type=\"text\" id=\"flagdefopt" + i.ToString() + "\" name=\"flagdefopt" + i.ToString() + "\" value=\"" + (fdFlagDef.Items[i].Name) + "\" disabled ><br/>";
            }
        }
        ltr_options.Text += "</div>";
    }

    public void ViewAll()
    {

        this.tbledit.Visible = false;
        StringBuilder sbContent = new StringBuilder();
        sbContent.Append("<div class=\"ektronPageContainer\" style=\"background: none;\"><table class=\"ektronGrid\" width=\"100%\">" + Environment.NewLine);

        sbContent.Append("<tr class=\"title-header\">" + Environment.NewLine);
        sbContent.Append("<th align=\"center\">" + Environment.NewLine);
        sbContent.Append(GetMessage("generic id"));
        sbContent.Append("</th>" + Environment.NewLine);
        sbContent.Append("<th>" + Environment.NewLine);
        sbContent.Append(GetMessage("generic name"));
        sbContent.Append("</th>" + Environment.NewLine);
        sbContent.Append("<th>" + Environment.NewLine);
        sbContent.Append(GetMessage("generic description"));
        sbContent.Append("</th>" + Environment.NewLine);
        sbContent.Append("<th align=\"center\">" + Environment.NewLine);
        sbContent.Append(GetMessage("lbl language"));
        sbContent.Append("</th>" + Environment.NewLine);
        sbContent.Append("<th align=\"center\">" + Environment.NewLine);
        sbContent.Append(GetMessage("generic items"));
        sbContent.Append("</th>" + Environment.NewLine);
        sbContent.Append("</tr>" + Environment.NewLine);
        if (!string.IsNullOrEmpty(communityflagaction))
        {
			// You shouldn't be able to add more than the originally created community flagging definition
			// for community, you can add or remove options within that definition, but not add a new one.
            FlagDefData flagdata = this.m_refContentApi.EkContentRef.GetCommunityFlaggingDefinition(false);
            if (flagdata != null)
            {
                AddLink = false;
                sbContent.Append(ReadFlagSet(flagdata, 0));
            }
        }
        else
        {
            aFlagSets = this.m_refContentApi.EkContentRef.GetAllFlaggingDefinitions(false);
            for (int i = 0; i <= (aFlagSets.Length - 1); i++)
            {
                sbContent.Append(ReadFlagSet(aFlagSets[i], i));
            }
        }

        sbContent.Append("</table>" + Environment.NewLine);
        this.ltr_view.Text = sbContent.ToString();
    }
    public string ReadFlagSet(FlagDefData flag, int i)
    {
        if (objLocalizationApi == null)
        {
            objLocalizationApi = new LocalizationAPI();
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("<tr>" + Environment.NewLine);
        sb.Append("<td align=\"center\">" + Environment.NewLine);
        sb.Append("<a href=\"flagsets.aspx?" + communityflagaction + "action=addedit&id=" + flag.ID.ToString() + "&LangType=" + flag.Language + "\">" + flag.ID.ToString() + "</a>");
        sb.Append("</td>" + Environment.NewLine);
        sb.Append("<td>" + Environment.NewLine);
        sb.Append("<a href=\"flagsets.aspx?" + communityflagaction + "action=addedit&id=" + flag.ID.ToString() + "&LangType=" + flag.Language + "\" class=\"flagEdit\">" + flag.Name + "</a>");
        sb.Append("</td>" + Environment.NewLine);
        sb.Append("<td>" + Environment.NewLine);
        sb.Append(flag.Description);
        sb.Append("</td>" + Environment.NewLine);
        sb.Append("<td align=\"center\">" + Environment.NewLine);
        sb.Append("<img src=\'" + objLocalizationApi.GetFlagUrlByLanguageID(flag.Language) + "\' />");
        sb.Append("</td>" + Environment.NewLine);
        sb.Append("<td align=\"center\">" + Environment.NewLine);
        sb.Append(flag.Items.Length);
        sb.Append("</td>" + Environment.NewLine);
        sb.Append("</tr>" + Environment.NewLine);
        return sb.ToString();
    }
    #endregion

    #region Process

    public void Process_AddEdit()
    {
        if (this.bAdmin || this.bFlagEditor)
        {
            if (this.m_iID > 0)
            {
                fdFlagDef = this.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(this.m_iID, true);
            }
            else
            {
                fdFlagDef.ID = 0; // signal backend that this is a new item.
            }

            fdFlagDef.Name = (string)this.txt_fd_name.Text;
            fdFlagDef.Description = (string)this.txt_fd_desc.Text;
            fdFlagDef.Language = System.Convert.ToInt32(this.ContentLanguage > 0 ? this.ContentLanguage : this.m_refContentApi.RequestInformationRef.DefaultContentLanguage);

            int iLength = 0;
            ArrayList alFlagList = new ArrayList();
            FlagItemData[] aRet = (Ektron.Cms.FlagItemData[])Array.CreateInstance(typeof(FlagItemData), 0);
            iLength = System.Convert.ToInt32(Request.Form["Flaglength"]);
            if (iLength > 0)
            {
                for (int i = 0; i <= (iLength - 1); i++)
                {
                    FlagItemData fiTMP = new FlagItemData();
                    fiTMP.ID = System.Convert.ToInt64(Request.Form["flag_iden" + i.ToString()]);
                    fiTMP.Name = Request.Form["flagdefopt" + i.ToString()];
                    fiTMP.SortOrder = System.Convert.ToInt32(i + 1);
                    fiTMP.FlagDefinitionID = this.m_iID;
                    fiTMP.FlagDefinitionLanguage = System.Convert.ToInt32(this.ContentLanguage > 0 ? this.ContentLanguage : this.m_refContentApi.RequestInformationRef.DefaultContentLanguage);
                    alFlagList.Add(fiTMP);
                }
                aRet = (Ektron.Cms.FlagItemData[])alFlagList.ToArray(typeof(FlagItemData));
            }

            fdFlagDef.Items = aRet;

            if (this.m_iID > 0)
            {
                fdFlagDef = this.m_refContentApi.EkContentRef.UpdateFlaggingDefinition(fdFlagDef);
            }
            else
            {
                if (!(string.IsNullOrEmpty(communityflagaction)))
                {
                    fdFlagDef.Hidden = true;
                }
                fdFlagDef = this.m_refContentApi.EkContentRef.AddFlaggingDefinition(fdFlagDef);
            }
            Response.Redirect("flagsets.aspx?" + communityflagaction + "action=viewall", false); // &id=" & fdFlagDef.ID.ToString(), False)
        }
        else
        {
            throw (new Exception(this.GetMessage("err flagset no access")));
        }
    }

    public void Process_Remove()
    {
        if (this.bAdmin || this.bFlagEditor)
        {
            if (this.m_iID > 0)
            {
                fdFlagDef = this.m_refContentApi.EkContentRef.GetFlaggingDefinitionbyID(this.m_iID, true);
                if (fdFlagDef.ID > 0)
                {
                    this.m_refContentApi.EkContentRef.DeleteFlaggingDefinition(fdFlagDef.ID);
                }
            }
            Response.Redirect("flagsets.aspx?action=viewall", false);
        }
        else
        {
            throw (new Exception(this.GetMessage("err flagset no access")));
        }
    }

    #endregion

    #region Private Helpers

    private void SetLabels()
    {
        switch (this.m_sPageAction)
        {
            case "addedit":
                if (this.m_iID > 0)
                {
                    this.SetTitleBarToMessage("lbl flagset edit");
                }
                else
                {
                    this.SetTitleBarToMessage("lbl flagset add");
                }

                this.ltr_name.Text = this.GetMessage("generic name") + ":";
                this.ltr_desc.Text = this.GetMessage("generic description") + ":";

                this.txt_fd_name.Text = Server.HtmlDecode(fdFlagDef.Name);
                this.hdn_fd_name.Value = Server.HtmlDecode(fdFlagDef.Name);
                this.txt_fd_desc.Text = Server.HtmlDecode(fdFlagDef.Description);

				this.AddBackButton((string)("flagsets.aspx?action=viewall" + ((string.IsNullOrEmpty(communityflagaction)) ? "" : "&communityonly=true")));

                if (this.bAdmin || this.bFlagEditor)
                {
                    base.AddButtonwithMessages(m_refContentApi.RequestInformationRef.ApplicationPath + "images/UI/Icons/save.png", "#", "lbl alt save flagset", "btn save", " onclick=\"javascript:SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
                    if (string.IsNullOrEmpty(communityflagaction))
                    {
                        if (this.m_iID > 0)
                        {
                            base.AddButton(m_refContentApi.RequestInformationRef.ApplicationPath + "images/UI/Icons/delete.png", (string)("flagsets.aspx?action=Remove&id=" + m_iID.ToString()), GetMessage("lbl delete flag"), GetMessage("lbl delete flag def"), "onclick=\"javascript:return VerifyDelete()\" ", StyleHelper.DeleteButtonCssClass);
                        }
                    }
                }
                else
                {
                    this.txt_fd_name.Enabled = false;
                    this.txt_fd_desc.Enabled = false;
                }
                
                if (this.m_iID > 0)
                {
                    base.AddHelpButton("edit_flagdef");
                }
                else
                {
                    base.AddHelpButton("add_flagdef");
                }
                break;
            default:
                if (string.IsNullOrEmpty(communityflagaction))
                {
                    this.SetTitleBarToMessage("wa tree flag def");
                }
                else
                {
                    this.SetTitleBarToMessage("wa tree community flag def");
                }

                if (AddLink)
                {
                    if ((bFlagEditor || bAdmin) && this.m_refContentApi.ContentLanguage > 0)
                    {
                        base.AddButton(m_refContentApi.RequestInformationRef.ApplicationPath + "images/UI/Icons/add.png", "flagsets.aspx?" + communityflagaction + "action=addedit", GetMessage("lbl add flag"), GetMessage("lbl add flag def"), "", StyleHelper.AddButtonCssClass, true);
                    }
                }
                this.AddLanguageDropdown(string.IsNullOrEmpty(communityflagaction));
                base.AddHelpButton("view_flagdef");
                break;
        }
    }

    private void SetJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("  		<script type=\"text/javascript\"> ").Append(Environment.NewLine);

        sbJS.Append("  			function LoadLanguage(FormName){ ").Append(Environment.NewLine);
        sbJS.Append("  				var num=document.forms[FormName].selLang.selectedIndex; ").Append(Environment.NewLine);
        sbJS.Append("  				window.location.href=\"flagsets.aspx?" + communityflagaction + "action=viewall\"+\"&LangType=\"+document.forms[FormName].selLang.options[num].value; ").Append(Environment.NewLine);
        sbJS.Append("  				//document.forms[FormName].submit(); ").Append(Environment.NewLine);
        sbJS.Append("  				return false; ").Append(Environment.NewLine);
        sbJS.Append("  			} ").Append(Environment.NewLine);

        sbJS.Append("           function CheckFlagDefParam() {" + Environment.NewLine);
        sbJS.Append("	            document.forms.frmContent.submit();" + Environment.NewLine);
        sbJS.Append("  			    return false; ").Append(Environment.NewLine);
        sbJS.Append("  			} ").Append(Environment.NewLine);

        sbJS.Append("           function VerifyDelete()" + Environment.NewLine);
        sbJS.Append("           {" + Environment.NewLine);
        sbJS.Append("               var agree=confirm(\'" + GetMessage("js cnfrm del flag def") + "\');" + Environment.NewLine);
        sbJS.Append("               if (agree) {" + Environment.NewLine);
        sbJS.Append("	                return true;" + Environment.NewLine);
        sbJS.Append("               } else {" + Environment.NewLine);
        sbJS.Append("	                return false;" + Environment.NewLine);
        sbJS.Append("               }" + Environment.NewLine);
        sbJS.Append("           }" + Environment.NewLine);

        sbJS.Append("/// Flag Items").Append(Environment.NewLine);
        sbJS.Append("        ").Append(Environment.NewLine);
        sbJS.Append("        var arrFlagID = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrFlag = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrFlagName = new Array(0);").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function addFlag() {").Append(Environment.NewLine);
        sbJS.Append("          arrFlagID.push(\"0\");").Append(Environment.NewLine);
        sbJS.Append("          arrFlag.push(arrFlag.length);").Append(Environment.NewLine);
        sbJS.Append("          arrFlagName.push(\"\");").Append(Environment.NewLine);
        sbJS.Append("          displayFlag();").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("          function addFlagInit(fid,fname) {").Append(Environment.NewLine);
        sbJS.Append("          arrFlagID.push(fid);").Append(Environment.NewLine);
        sbJS.Append("          arrFlag.push(arrFlag.length);").Append(Environment.NewLine);
        sbJS.Append("          arrFlagName.push(fname);").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function displayFlag() {").Append(Environment.NewLine);
        sbJS.Append("          var sItem = \'\';").Append(Environment.NewLine);
        sbJS.Append("          var sList = \'\';").Append(Environment.NewLine);
        sbJS.Append("          document.getElementById(\'pFlag\').innerHTML=\'\';").Append(Environment.NewLine);
        sbJS.Append("          for (intI = 0; intI < arrFlag.length; intI++) {").Append(Environment.NewLine);
        sbJS.Append("            sItem = createFlag(arrFlagID[intI], arrFlag[intI], arrFlagName[intI], intI, arrFlag.length);").Append(Environment.NewLine);
        sbJS.Append("            sList += sItem;").Append(Environment.NewLine);
        sbJS.Append("          }").Append(Environment.NewLine);
        sbJS.Append("            sList += \"&nbsp;&nbsp;<a href=\\\"#\\\" onclick=\\\"javascript:addFlag(); return false;\\\"><img src=\\\"" + m_refContentApi.RequestInformationRef.ApplicationPath + "images/UI/Icons/add.png\\\"/></a><br/>\";").Append(Environment.NewLine);
        sbJS.Append("          document.getElementById(\'pFlag\').innerHTML = sList;").Append(Environment.NewLine);
        sbJS.Append("          document.getElementById(\'Flaglength\').value = arrFlag.length;").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function saveFlag(intId,strValue,type) {").Append(Environment.NewLine);
        // sbJS.Append("            alert(strValue + '-' + type); ").Append(Environment.NewLine)
        sbJS.Append("            if (type == \"fname\") {").Append(Environment.NewLine);
        sbJS.Append("                arrFlagName[intId]=strValue;").Append(Environment.NewLine);
        sbJS.Append("            }").Append(Environment.NewLine);
        sbJS.Append("        }  ").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function createFlag(fid,id,fname, iloc, itot) {").Append(Environment.NewLine);
        sbJS.Append("          var sRet = \"\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"&nbsp;&nbsp;<input type=\\\"text\\\" id=\\\"flagdefopt\" + id + \"\\\" name=\\\"flagdefopt\" + id + \"\\\" value=\\\"\" + fname + \"\\\" maxlength=\\\"50\\\" size=\\\"35\\\" onChange=\\\"javascript:saveFlag(\" + id + \",this.value,\'fname\')\\\">\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"<input type=\\\"hidden\\\" name=\\\"flag_iden\" + id + \"\\\" id=\\\"flag_iden\" + id + \"\\\" value=\\\"\" + fid + \"\\\" />\";").Append(Environment.NewLine);
        sbJS.Append("          if (iloc == (itot - 1)) {").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"&nbsp;&nbsp;<img src=\\\"" + this.AppImgPath + "movedown_disabled.gif\\\"/>\";").Append(Environment.NewLine);
        sbJS.Append("          } else {").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"&nbsp;&nbsp;<a href=\\\"#\\\" onclick=\\\"javascript:moveFlag(\" + id + \",\'down\');return false;\\\"><img src=\\\"" + this.AppImgPath + "movedown.gif\\\"/></a>\";").Append(Environment.NewLine);
        sbJS.Append("          }").Append(Environment.NewLine);
        sbJS.Append("          if (iloc == 0) {").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"&nbsp;&nbsp;<img src=\\\"" + this.AppImgPath + "moveup_disabled.gif\\\"/>\";").Append(Environment.NewLine);
        sbJS.Append("          } else {").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"&nbsp;&nbsp;<a href=\\\"#\\\" onclick=\\\"javascript:moveFlag(\" + id + \",\'up\');return false;\\\"><img src=\\\"" + this.AppImgPath + "moveup.gif\\\"/></a>\";").Append(Environment.NewLine);
        sbJS.Append("          }").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"&nbsp;&nbsp;<a href=\\\"#\\\" onclick=\\\"javascript:removeFlag(\" + id + \"); return false;\\\"><img src=\\\"" + m_refContentApi.RequestInformationRef.ApplicationPath + "images/UI/Icons/delete.png\\\"/></a>\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"<br/>\";").Append(Environment.NewLine);
        sbJS.Append("          return sRet; ").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function deleteFlag() {").Append(Environment.NewLine);
        sbJS.Append("            //remove last").Append(Environment.NewLine);
        sbJS.Append("            var cnfm = confirm(\"").Append(GetMessage("js confirm remove last flag item")).Append("\");").Append(Environment.NewLine);
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine);
        sbJS.Append("            {").Append(Environment.NewLine);
        sbJS.Append("              if (arrFlag.length > 0) { ").Append(Environment.NewLine);
        sbJS.Append("                 arrFlagID.pop(); ").Append(Environment.NewLine);
        sbJS.Append("                 arrFlag.pop(); ").Append(Environment.NewLine);
        sbJS.Append("                 arrFlagName.pop();").Append(Environment.NewLine);
        sbJS.Append("              }").Append(Environment.NewLine);
        sbJS.Append("              displayFlag();").Append(Environment.NewLine);
        sbJS.Append("            }").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function removeFlag(id) {").Append(Environment.NewLine);
        sbJS.Append("            //remove last").Append(Environment.NewLine);
        sbJS.Append("            var cnfm = confirm(\"").Append(GetMessage("js confirm remove flag item")).Append("\");").Append(Environment.NewLine);
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine);
        sbJS.Append("            {").Append(Environment.NewLine);
        sbJS.Append("              if (arrFlag.length > 0) { ").Append(Environment.NewLine);
        sbJS.Append("                 arrFlagID.splice(id,1); ").Append(Environment.NewLine);
        sbJS.Append("                 arrFlag.pop(); ").Append(Environment.NewLine);
        sbJS.Append("                 arrFlagName.splice(id,1);").Append(Environment.NewLine);
        sbJS.Append("              }").Append(Environment.NewLine);
        sbJS.Append("              displayFlag(); ").Append(Environment.NewLine);
        sbJS.Append("            }").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function moveFlag(id, direc) {").Append(Environment.NewLine);
        sbJS.Append("            if (direc == \'up\')").Append(Environment.NewLine);
        sbJS.Append("            {").Append(Environment.NewLine);
        sbJS.Append("                var strID = arrFlagID[id];").Append(Environment.NewLine);
        sbJS.Append("                var strName = arrFlagName[id];").Append(Environment.NewLine);
        sbJS.Append("                arrFlagID[id]=arrFlagID[(id - 1)];").Append(Environment.NewLine);
        sbJS.Append("                arrFlagName[id]=arrFlagName[(id - 1)];").Append(Environment.NewLine);
        sbJS.Append("                arrFlagID[(id - 1)]=strID;").Append(Environment.NewLine);
        sbJS.Append("                arrFlagName[(id - 1)]=strName;").Append(Environment.NewLine);
        sbJS.Append("            } else if (direc == \'down\') {").Append(Environment.NewLine);
        sbJS.Append("                var strID = arrFlagID[id];").Append(Environment.NewLine);
        sbJS.Append("                var strName = arrFlagName[id];").Append(Environment.NewLine);
        sbJS.Append("                arrFlagID[id]=arrFlagID[(id + 1)];").Append(Environment.NewLine);
        sbJS.Append("                arrFlagName[id]=arrFlagName[(id + 1)];").Append(Environment.NewLine);
        sbJS.Append("                arrFlagID[(id + 1)]=strID;").Append(Environment.NewLine);
        sbJS.Append("                arrFlagName[(id + 1)]=strName;").Append(Environment.NewLine);
        sbJS.Append("            } ").Append(Environment.NewLine);
        sbJS.Append("            displayFlag(); ").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function SubmitForm() {" + Environment.NewLine);
        sbJS.Append("           if (!CheckForillegalChar()) { " + Environment.NewLine);
        sbJS.Append("               return false; " + Environment.NewLine);
        sbJS.Append("           } else { " + Environment.NewLine);
        sbJS.Append("               var strRet = CheckFlag();" + Environment.NewLine);
        sbJS.Append("               if (strRet.length > 0)" + Environment.NewLine);
        sbJS.Append("               {" + Environment.NewLine);
        sbJS.Append("                   alert(strRet);" + Environment.NewLine);
        sbJS.Append("               } else { " + Environment.NewLine);
        sbJS.Append("                   document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("               }" + Environment.NewLine);
        sbJS.Append("           }" + Environment.NewLine);
        sbJS.Append("        }" + Environment.NewLine);

        sbJS.Append("       function CheckForillegalChar() {" + Environment.NewLine);
        sbJS.Append("           var val = document.forms[0]." + Strings.Replace((string)this.txt_fd_name.UniqueID, "$", "_", 1, -1, 0) + ".value;" + Environment.NewLine);
        sbJS.Append("           if (Trim(val) == \'\')" + Environment.NewLine);
        sbJS.Append("           {" + Environment.NewLine);
        sbJS.Append("               alert(\'" + GetMessage("lbl please enter flag def name") + "\'); " + Environment.NewLine);
        sbJS.Append("               return false; " + Environment.NewLine);
        sbJS.Append("           } else { " + Environment.NewLine);
        sbJS.Append("               if ((val.indexOf(\";\") > -1) || (val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbJS.Append("               {" + Environment.NewLine);
        sbJS.Append("                   alert(\"" + string.Format(GetMessage("lbl flag def name disallowed chars"), "(\';\', \'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\')") + "\");" + Environment.NewLine);
        sbJS.Append("                   return false;" + Environment.NewLine);
        sbJS.Append("               }" + Environment.NewLine);
        sbJS.Append("           }" + Environment.NewLine);
        sbJS.Append("           return true;" + Environment.NewLine);
        sbJS.Append("       }" + Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function CheckFlag() {" + Environment.NewLine);
        sbJS.Append("           var sErr = \"\"; " + Environment.NewLine);
        sbJS.Append("           var bName = false; " + Environment.NewLine);
        sbJS.Append("           var bDupe = false; " + Environment.NewLine);
        sbJS.Append("           if (arrFlag.length == 0) { " + Environment.NewLine);
        sbJS.Append("               sErr = \"" + GetMessage("lbl flag def at least one flag item") + "\";" + Environment.NewLine);
        sbJS.Append("           } else { " + Environment.NewLine);
        sbJS.Append("               for (var j = 0; j < arrFlag.length; j++)" + Environment.NewLine);
        sbJS.Append("               {" + Environment.NewLine);

        sbJS.Append("                   if ((Trim(arrFlagName[j]).length < 1) && (bName == false)) {" + Environment.NewLine);
        sbJS.Append("                       if (sErr.length > 0) {" + Environment.NewLine);
        sbJS.Append("                           sErr = sErr + \"\\n\";" + Environment.NewLine);
        sbJS.Append("                       }" + Environment.NewLine);
        sbJS.Append("                       sErr = sErr + \"" + GetMessage("lbl flag items no name") + "\";" + Environment.NewLine);
        sbJS.Append("                       bName = true; " + Environment.NewLine);
        sbJS.Append("                   } " + Environment.NewLine);
        sbJS.Append("                   for (var k = 0; k < arrFlag.length; k++) { " + Environment.NewLine);
        sbJS.Append("                       if ((bDupe == false) && (j != k) && (Trim(arrFlagName[j]) == Trim(arrFlagName[k]))) {" + Environment.NewLine);
        sbJS.Append("                           if (sErr.length > 0) {" + Environment.NewLine);
        sbJS.Append("                               sErr = sErr + \"\\n\";" + Environment.NewLine);
        sbJS.Append("                           }" + Environment.NewLine);
        sbJS.Append("                           sErr = sErr + \"" + GetMessage("lbl flag items unique") + "\";" + Environment.NewLine);
        sbJS.Append("                           bDupe = true; " + Environment.NewLine);
        sbJS.Append("                       } " + Environment.NewLine);
        sbJS.Append("                   } " + Environment.NewLine);

        sbJS.Append("               }" + Environment.NewLine);
        sbJS.Append("           }" + Environment.NewLine);
        sbJS.Append("           return sErr;" + Environment.NewLine);
        sbJS.Append("        }" + Environment.NewLine);

        sbJS.Append("  		</script> ").Append(Environment.NewLine);
        ltr_js.Text = sbJS.ToString();
    }
    protected void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
    }
    #endregion

}

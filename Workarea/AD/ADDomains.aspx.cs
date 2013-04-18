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
using Ektron.Cms.Workarea;
using Ektron.Cms.Common;


public partial class AD_ADDomains : workareabase
{


    private ADDomain[] DefinedDomains;
    private Ektron.Cms.User.EkUser eUser;
    protected SettingsData setting_data = null;
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected UserAPI m_refUserApi = new UserAPI();
    protected bool AdValid = false;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        RegisterCss();
    }

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;

        eUser = m_refContentApi.EkUserRef;

        CheckAccess();
        ADCheck();

        if (AdValid == true)
        {
            SetLabels(this.m_sPageAction);
            if ((string)(this.m_sPageAction) == "edit")
            {
                if (Page.IsPostBack)
                {
                    ProcessDomains();
                }
                else
                {
                    //m_refSiteApi = New SiteAPI
                    //setting_data = m_refSiteApi.GetSiteVariables()
                    DefinedDomains = eUser.GetDomainsAdvanced();
                    EditDomains();
                }
            }
            else
            {
                DefinedDomains = eUser.GetDomainsAdvanced();
                ShowDomains();
            }
        }
        else
        {
            SetLabels("error");
        }
    }

    private void CheckAccess()
    {
        if (!Utilities.ValidateUserLogin())
        {
            return;
        }
        if (!m_refContentApi.IsAdmin() && m_refContentApi.RequestInformationRef.UserId != Ektron.Cms.Common.EkConstants.BuiltIn)
        {
            HttpContext.Current.Response.Redirect(m_refContentApi.AppPath + "reterror.aspx?info=" + m_refContentApi.EkMsgRef.GetMessage("msg login cms administrator"), false);
            return;
        }
    }

    private void ADCheck()
    {
        setting_data = m_refSiteApi.GetSiteVariables(m_refUserApi.UserId);
        AdValid = setting_data.AdValid; //CBool(siteVars("AdValid"))
        if (!(AdValid))
        {
            lbl_msg.Visible = true;
            lbl_msg.Text = base.GetMessage("entrprise license with AD required msg");
        }
    }

    private void SetLabels(string type)
    {
        if (type.ToLower() == "error")
        {
            base.Title = base.GetMessage("adconfig page title");
            base.SetTitleBarToMessage("adconfig page title");
        }
        else if (type.ToLower() == "edit")
        {
            base.Title = base.GetMessage("generic edit ad domains");
            base.SetTitleBarToMessage("generic edit ad domains");

			base.AddBackButton("ADDomains.aspx");
			base.AddButtonwithMessages(m_refContentApi.AppImgPath + "../UI/Icons/save.png", "#", "lbl alt save ad domains", "btn save", " onclick=\"javascript:return SubmitForm();\" ", StyleHelper.SaveButtonCssClass, true);
        }
        else
        {
            base.Title = base.GetMessage("generic ad domains");
            base.SetTitleBarToMessage("generic ad domains");
			base.AddButtonwithMessages(m_refContentApi.AppImgPath + "../UI/Icons/contentEdit.png", "ADDomains.aspx?action=edit", "lbl alt edit ad domains", "btn edit", "", StyleHelper.EditButtonCssClass, true);
        }

        if (!(type.ToLower() == "error"))
        {
            base.AddHelpButton("ADDomains");
        }
    }

    private void ProcessDomains()
    {
        int iLength = 0;
        ADDomain adTMP = null;
        ArrayList alDomainList = new ArrayList();
        ADDomain[] aRet = (ADDomain[])Array.CreateInstance(typeof(ADDomain), 0);
        iLength = System.Convert.ToInt32(Request.Form["Domainlength"]);
        if (iLength > 0)
        {
            for (int i = 0; i <= (iLength - 1); i++)
            {
                string[] aDName;
                adTMP = new ADDomain();
                adTMP.ID = EkFunctions.ReadLongValue(Request.Form["domain_iden" + i.ToString()], 0);
                //adTMP.DomainShortName = Request.Form("domain_name" & i.ToString())
                adTMP.DomainDNS = Request.Form["domain_dns" + i.ToString()];
                aDName = adTMP.DomainDNS.Split('.');
                if (aDName.Length > 0)
                {
                    // use first
                    adTMP.DomainShortName = aDName[0];
                }
                else
                {
                    adTMP.DomainShortName = adTMP.DomainDNS;
                }
                adTMP.DomainPath = Ektron.Cms.Common.EkConstants.CreateADsPathFromDomain(adTMP.DomainDNS);
                if (!string.IsNullOrEmpty(Request.Form["use_name" + i.ToString()]))
                {
                    adTMP.NetBIOS = adTMP.DomainShortName;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Form["netbios" + i.ToString()]))
                    {
                        adTMP.NetBIOS = Request.Form["netbios" + i.ToString()];
                    }
                    else
                    {
                        adTMP.NetBIOS = adTMP.DomainShortName;
                    }
                }
                adTMP.Username = Request.Form["user_name" + i.ToString()];
                adTMP.Password = (string)(Request.Form["password" + i.ToString()].Trim());
                adTMP.ServerIP = Request.Form["server_ip" + i.ToString()];
                alDomainList.Add(adTMP);
            }
            aRet = (ADDomain[])alDomainList.ToArray(typeof(ADDomain));
        }

        eUser.UpdateDomainsAdvanced(aRet);

        Response.Redirect("ADDomains.aspx", false);
    }

    private void EditDomains()
    {
        RenderJS();
        StringBuilder sbContent = new StringBuilder();
        sbContent.Append("<a title=\"" + m_refMsg.GetMessage("lbl Add New Domain") + "\" href=\"#\" onclick=\"javascript:addDomain()\">" + m_refMsg.GetMessage("lbl Add New Domain") + "</a>&nbsp;|&nbsp;<a title=\"" + m_refMsg.GetMessage("lbl Remove Last Domain") + "\" href=\"#\" onclick=\"javascript:deleteDomain()\">" + m_refMsg.GetMessage("lbl Remove Last Domain") + "</a><p id=\"parah\"></p>");
        sbContent.Append("<input type=\"hidden\" id=\"Domainlength\" name=\"Domainlength\" value=\"" + DefinedDomains.Length.ToString() + "\" />");
        sbContent.Append("<div id=\"pDomain\" name=\"pDomain\">" + Environment.NewLine);
        sbContent.Append("<table class=\"ektronForm\">" + Environment.NewLine);
        for (int i = 0; i <= (DefinedDomains.Length - 1); i++)
        {

            //remove link
            sbContent.Append("<tr>" + Environment.NewLine);
            sbContent.Append("<td colspan=\"2\">");
            sbContent.Append("<a title=\"" + m_refMsg.GetMessage("lbl Remove Domain") + ("\" href=\"#\" onclick=\"javascript:removeDomain(\'" + i.ToString() + "\');\">") + m_refMsg.GetMessage("lbl Remove Domain") + "</a>");
            sbContent.Append(Environment.NewLine);
            sbContent.Append("<script language=\"javascript\" type=\"text/javascript\">addDomainInit(" + DefinedDomains[i].ID.ToString() + ",\'" + EkFunctions.HtmlEncode(DefinedDomains[i].DomainDNS) + "\',\'" + EkFunctions.HtmlEncode(DefinedDomains[i].NetBIOS) + "\',\'" + EkFunctions.HtmlEncode(DefinedDomains[i].Username) + "\',\'          \',\'" + EkFunctions.HtmlEncode(DefinedDomains[i].ServerIP) + "\');</script>");
            sbContent.Append(Environment.NewLine);
            sbContent.Append("<input type=\"hidden\" name=\"domain_iden" + i.ToString() + "\" id=\"domain_iden" + i.ToString() + "\" value=\"" + DefinedDomains[i].ID.ToString() + "\" /> ");

            //Link Name
            //sbContent.Append("<tr>" & Environment.NewLine)
            //sbContent.Append("<td class=""label"" width=""160"">" & Environment.NewLine)
            //sbContent.Append(GetMessage("lbl ad domain name") & ":")
            //sbContent.Append("</td><td>" & Environment.NewLine)
            //sbContent.Append("<input name=""domain_name" + i.ToString() + """ type=""text"" value=""" + EkFunctions.HtmlEncode(DefinedDomains(i).DomainShortName) + """ id=""domain_name" + i.ToString() + """ onChange=""javascript:saveDomain(" + i.ToString() + ",this.value,'dname')"" />")
            //sbContent.Append("</td></tr>" & Environment.NewLine)

            //Short Description
            sbContent.Append("<tr>" + Environment.NewLine);
            sbContent.Append("<td class=\"label\">" + Environment.NewLine);
            sbContent.Append(GetMessage("lbl ad domain dns") + ":");
            sbContent.Append("</td>");
            sbContent.Append("<td class=\"value\">" + Environment.NewLine);
            sbContent.Append("<input class=\"ektronTextMedium\" name=\"domain_dns" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(DefinedDomains[i].DomainDNS) + "\" id=\"domain_dns" + i.ToString() + "\" onChange=\"javascript:saveDomain(" + i.ToString() + ",this.value,\'dns\')\" />");
            sbContent.Append("</td>");
            sbContent.Append("</tr>" + Environment.NewLine);

            //Relationship
            sbContent.Append("<tr>" + Environment.NewLine);
            sbContent.Append("<td class=\"label\">");
            sbContent.Append(GetMessage("lbl ad netbios") + ":");
            sbContent.Append("</td>");
            sbContent.Append("<td class=\"value\">");
            if (DefinedDomains[i].NetBIOS.Trim().ToLower() == DefinedDomains[i].DomainShortName.Trim().ToLower())
            {
                sbContent.Append("<input class=\"ektronTextMedium\" disabled name=\"netbios" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(DefinedDomains[i].NetBIOS) + "\" id=\"netbios" + i.ToString() + "\" onChange=\"javascript:saveDomain(" + i.ToString() + ",this.value,\'netbios\')\" />");
                sbContent.Append(Environment.NewLine);
                sbContent.Append("<div class=\"ektronCaption\">");
                sbContent.Append("<input type=\"checkbox\" name=\"use_name" + ID.ToString() + "\" id=\"use_name" + ID.ToString() + ("\" checked onclick=\"(document.getElementById(\'netbios" + i.ToString() + "\').disabled)=(!(document.getElementById(\'netbios" + i.ToString() + "\').disabled));\" />"));
            }
            else
            {
                sbContent.Append("<input class=\"ektronTextMedium\" name=\"netbios" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(DefinedDomains[i].NetBIOS) + "\" id=\"netbios" + i.ToString() + "\" onChange=\"javascript:saveDomain(" + i.ToString() + ",this.value,\'netbios\')\" />");
                sbContent.Append(Environment.NewLine);
                sbContent.Append("<div class=\"ektronCaption\">");
                sbContent.Append("<input type=\"checkbox\" name=\"use_name" + ID.ToString() + "\" id=\"use_name" + ID.ToString() + ("\" onclick=\"(document.getElementById(\'netbios" + i.ToString() + "\').disabled)=(!(document.getElementById(\'netbios" + i.ToString() + "\').disabled));\" />"));
            }
            sbContent.Append(GetMessage("lbl ad use domainname"));
            sbContent.Append("</div>");
            sbContent.Append("</td>");
            sbContent.Append("</tr>" + Environment.NewLine);
            //
            sbContent.Append("<tr>" + Environment.NewLine);
            sbContent.Append("<td class=\"label\">");
            sbContent.Append(GetMessage("generic username") + ":");
            sbContent.Append("</td>");
            sbContent.Append("<td class=\"value\">");
            sbContent.Append("<input class=\"ektronTextMedium\" name=\"user_name" + i.ToString() + "\" type=\"text\" value=\'" + EkFunctions.HtmlEncode(DefinedDomains[i].Username) + "\' id=\"user_name" + i.ToString() + "\" onChange=\"javascript:saveDomain(" + i.ToString() + ",this.value,\'uname\')\" /> <span class=\"ektronCaption\">ex: name@domain.com</span>");
            sbContent.Append("</td>");
            sbContent.Append("</tr>" + Environment.NewLine);
            //
            sbContent.Append("<tr>" + Environment.NewLine);
            sbContent.Append("<td class=\"label\">");
            sbContent.Append(GetMessage("password label") + "</b>");
            sbContent.Append("</td>");
            sbContent.Append("<td class=\"value\">");
            sbContent.Append("<input class=\"ektronTextMedium\" name=\"password" + i.ToString() + "\" type=\"password\" value=\"          \" id=\"password" + i.ToString() + "\" onChange=\"javascript:saveDomain(" + i.ToString() + ",this.value,\'pwd\')\" />");
            sbContent.Append("</td>");
            sbContent.Append("</tr>" + Environment.NewLine);
            //
            sbContent.Append("<tr>" + Environment.NewLine);
            sbContent.Append("<td class=\"label\">");
            sbContent.Append(GetMessage("lbl ad serverip") + ":");
            sbContent.Append("</td>");
            sbContent.Append("<td class=\"value\">");
            sbContent.Append("<input class=\"ektronTextXXSmall\" name=\"server_ip" + i.ToString() + "\" type=\"text\" value=\"" + EkFunctions.HtmlEncode(DefinedDomains[i].ServerIP) + "\" id=\"server_ip" + i.ToString() + "\" onChange=\"javascript:saveDomain(" + i.ToString() + ",this.value,\'sip\')\" />");
            sbContent.Append("</td>");
            sbContent.Append("</tr>" + Environment.NewLine);

            // horizontal rule
            sbContent.Append("<tr>" + Environment.NewLine);
            sbContent.Append("<td colspan=\"2\">");
            sbContent.Append("<hr/>");
            sbContent.Append("</td>");
            sbContent.Append("</tr>" + Environment.NewLine);
        }
        sbContent.Append("</table>" + Environment.NewLine);
        sbContent.Append("</div>" + Environment.NewLine);
        lbl_add_domain.Text = sbContent.ToString();
    }

    private void ShowDomains()
    {
        if (DefinedDomains.Length > 0)
        {
            Table tRoll = new Table();
            tRoll.CssClass = "ektronForm";
            TableRow tRollRow = new TableRow();
            TableCell tRollCell = new TableCell();

            tRollCell = new TableCell();
            for (int i = 0; i <= DefinedDomains.Length - 1; i++)
            {
                //Link Name
                //tRollCell = New TableCell
                //tRollRow = New TableRow
                //tRollCell.HorizontalAlign = HorizontalAlign.Right
                //tRollCell.Text = GetMessage("lbl ad domain name") & ":"
                //tRollCell.Width = Unit.Pixel(160)
                //tRollRow.Controls.Add(tRollCell)
                //tRollCell = New TableCell
                //tRollCell.Text = DefinedDomains(i).DomainShortName
                //tRollRow.Controls.Add(tRollCell)
                //tRoll.Controls.Add(tRollRow)
                //Short Description
                tRollCell = new TableCell();
                tRollRow = new TableRow();
                tRollCell.CssClass = "label";
                tRollCell.Text = (string)(GetMessage("lbl ad domain dns") + ":");
                tRollRow.Controls.Add(tRollCell);
                tRollCell = new TableCell();
                tRollCell.Text = EkFunctions.HtmlEncode(DefinedDomains[i].DomainDNS);
                tRollRow.Controls.Add(tRollCell);
                tRoll.Controls.Add(tRollRow);
                //Relationship
                tRollCell = new TableCell();
                tRollRow = new TableRow();
                tRollCell.CssClass = "label";
                tRollCell.VerticalAlign = VerticalAlign.Top;
                tRollCell.Text = (string)(GetMessage("lbl ad netbios") + ":");
                tRollRow.Controls.Add(tRollCell);
                tRollCell = new TableCell();
                tRollCell.Text = EkFunctions.HtmlEncode(DefinedDomains[i].NetBIOS);
                tRollRow.Controls.Add(tRollCell);
                tRoll.Controls.Add(tRollRow);
                //
                tRollCell = new TableCell();
                tRollRow = new TableRow();
                tRollCell.CssClass = "label";
                tRollCell.Text = (string)(GetMessage("generic username") + ":");
                tRollRow.Controls.Add(tRollCell);
                tRollCell = new TableCell();
                tRollCell.Text = EkFunctions.HtmlEncode(DefinedDomains[i].Username);
                tRollRow.Controls.Add(tRollCell);
                tRoll.Controls.Add(tRollRow);
                //
                tRollCell = new TableCell();
                tRollRow = new TableRow();
                tRollCell.CssClass = "label";
                tRollCell.Text = (string)(GetMessage("lbl ad serverip") + ":");
                tRollRow.Controls.Add(tRollCell);
                tRollCell = new TableCell();
                tRollCell.Text = EkFunctions.HtmlEncode(DefinedDomains[i].ServerIP);
                tRollRow.Controls.Add(tRollCell);
                tRoll.Controls.Add(tRollRow);

                // horizontal rule
                tRollCell = new TableCell();
                tRollRow = new TableRow();
                tRollCell.Text = "<hr/>";
                tRollCell.ColumnSpan = 2;
                tRollRow.Controls.Add(tRollCell);
                tRoll.Controls.Add(tRollRow);
            }
            lbl_add_domain.Controls.Add(tRoll);
        }
    }

    private void RenderJS()
    {
        System.Text.StringBuilder sbJS = new System.Text.StringBuilder();

        sbJS.Append("/// AD Domains").Append(Environment.NewLine);
        sbJS.Append("        ").Append(Environment.NewLine);
        sbJS.Append("        var arrDomainID = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrDomain = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        // var arrDomainValue = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrDomainDNS = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrDomainNetBIOS = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrDomainUsername = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrDomainPassword = new Array(0);").Append(Environment.NewLine);
        sbJS.Append("        var arrDomainServerIP = new Array(0);").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function addDomain() {").Append(Environment.NewLine);
        sbJS.Append("          arrDomainID.push(\"0\");").Append(Environment.NewLine);
        sbJS.Append("          arrDomain.push(arrDomain.length);").Append(Environment.NewLine);
        sbJS.Append("          // arrDomainValue.push(\"\");").Append(Environment.NewLine);
        sbJS.Append("          arrDomainDNS.push(\"\");").Append(Environment.NewLine);
        sbJS.Append("          arrDomainNetBIOS.push(\"\");").Append(Environment.NewLine);
        sbJS.Append("          arrDomainUsername.push(\"\");").Append(Environment.NewLine);
        sbJS.Append("          arrDomainPassword.push(\"\");").Append(Environment.NewLine);
        sbJS.Append("          arrDomainServerIP.push(\"\");").Append(Environment.NewLine);
        sbJS.Append("          displayDomain();").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("          function addDomainInit(did,dns,netbios,uname,pwd,sip) {").Append(Environment.NewLine);
        sbJS.Append("          arrDomainID.push(did);").Append(Environment.NewLine);
        sbJS.Append("          arrDomain.push(arrDomain.length);").Append(Environment.NewLine);
        sbJS.Append("          // arrDomainValue.push(dname);").Append(Environment.NewLine);
        sbJS.Append("          arrDomainDNS.push(dns);").Append(Environment.NewLine);
        sbJS.Append("          arrDomainNetBIOS.push(netbios);").Append(Environment.NewLine);
        sbJS.Append("          arrDomainUsername.push(uname);").Append(Environment.NewLine);
        sbJS.Append("          arrDomainPassword.push(pwd);").Append(Environment.NewLine);
        sbJS.Append("          arrDomainServerIP.push(sip);").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function displayDomain() {").Append(Environment.NewLine);
        sbJS.Append("          var sItem = \'\';").Append(Environment.NewLine);
        sbJS.Append("          var sList = \'\';").Append(Environment.NewLine);
        sbJS.Append("          document.getElementById(\'pDomain\').innerHTML=\'\';").Append(Environment.NewLine);
        sbJS.Append("          for (intI = 0; intI < arrDomain.length; intI++) {").Append(Environment.NewLine);
        sbJS.Append("            sItem = createDomain(arrDomainID[intI], arrDomain[intI], arrDomainDNS[intI], arrDomainNetBIOS[intI], arrDomainUsername[intI], arrDomainPassword[intI], arrDomainServerIP[intI]);").Append(Environment.NewLine);
        sbJS.Append("            sList += sItem;").Append(Environment.NewLine);
        sbJS.Append("          }").Append(Environment.NewLine);
        sbJS.Append("          document.getElementById(\'pDomain\').innerHTML = sList;").Append(Environment.NewLine);
        sbJS.Append("          document.getElementById(\'Domainlength\').value = arrDomain.length;").Append(Environment.NewLine);
        sbJS.Append("          FixPass();").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function saveDomain(intId,strValue,type) {").Append(Environment.NewLine);
        // sbJS.Append("            alert(strValue + '-' + type); ").Append(Environment.NewLine)
        sbJS.Append("            if (type == \"dname\") {").Append(Environment.NewLine);
        sbJS.Append("                // arrDomainValue[intId]=strValue;").Append(Environment.NewLine);
        sbJS.Append("            }else if (type == \"dns\") {").Append(Environment.NewLine);
        sbJS.Append("                arrDomainDNS[intId]=strValue;").Append(Environment.NewLine);
        sbJS.Append("            }else if (type == \"netbios\") {").Append(Environment.NewLine);
        sbJS.Append("                arrDomainNetBIOS[intId]=strValue;").Append(Environment.NewLine);
        sbJS.Append("            }else if (type == \"uname\") {").Append(Environment.NewLine);
        sbJS.Append("                arrDomainUsername[intId]=strValue;").Append(Environment.NewLine);
        sbJS.Append("            }else if (type == \"pwd\") {").Append(Environment.NewLine);
        sbJS.Append("                arrDomainPassword[intId]=strValue;").Append(Environment.NewLine);
        sbJS.Append("            }else if (type == \"sip\") {").Append(Environment.NewLine);
        sbJS.Append("                arrDomainServerIP[intId]=strValue;").Append(Environment.NewLine);
        sbJS.Append("            }").Append(Environment.NewLine);
        sbJS.Append("        }  ").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function FixPass() {").Append(Environment.NewLine);
        sbJS.Append("          for (intI=0;intI<arrDomain.length;intI++) {").Append(Environment.NewLine);
        sbJS.Append("            document.getElementById(\'password\' + intI).value = arrDomainPassword[intI];").Append(Environment.NewLine);
        //sbJS.Append("            alert(document.getElementById('password' + intI).value);").Append(Environment.NewLine)
        sbJS.Append("          }").Append(Environment.NewLine);
        sbJS.Append("        }  ").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function createDomain(did,id,dns,netbios,uname,pwd,sip) {").Append(Environment.NewLine);
        sbJS.Append("          var sRet = \"\";").Append(Environment.NewLine);
        sbJS.Append("          var dname = dns.split(\".\");").Append(Environment.NewLine);
        sbJS.Append("          var dname = dname[0].toLowerCase();").Append(Environment.NewLine);
        sbJS.Append("          sRet = \"<table class=\\\"ektronForm\\\" ><tr>\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"<td colspan=\\\"2\\\"><a title=\\\"" + m_refMsg.GetMessage("lbl Remove Domain") + "\\\" href=\\\"#\\\" onClick=\\\"javascript:removeDomain(\" + id + \")\\\">" + m_refMsg.GetMessage("lbl Remove Domain") + "</a></td></tr>\";").Append(Environment.NewLine);
        //sbJS.Append("          sRet = sRet + ""<tr><td class=\""label\"" style=\""width:160px;\""><b>" & MyBase.GetMessage("lbl ad domain name") & ":</b></td><td><input name=\""domain_name"" + id + ""\"" type=\""text\"" value=\"""" + dname + ""\"" size=\""55\"" id=\""domain_name"" + id + ""\"" onChange=\""javascript:saveDomain("" + id + "",this.value,'dname')\"" /></td>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + \"<input type=\\\"hidden\\\" name=\\\"domain_iden\" + id + \"\\\" id=\\\"domain_iden\" + id + \"\\\" value=\\\"\" + did + \"\\\" />\";").Append(Environment.NewLine);
        //sbJS.Append("          sRet = sRet + ""</tr>"";").Append(Environment.NewLine)
        sbJS.Append("          sRet = sRet + \"<tr class=\\\"stripe\\\"><td class=\\\"label\\\"><b>" + base.GetMessage("lbl ad domain dns") + ":</b></td><td class=\\\"value\\\"><input name=\\\"domain_dns\" + id + \"\\\" class=\\\"ektronTextMedium\\\" type=\\\"text\\\" value=\\\"\" + dns + \"\\\" size=\\\"55\\\" id=\\\"domain_dns\" + id + \"\\\" onChange=\\\"javascript:saveDomain(\" + id + \",this.value,\'dns\')\\\" /></td>\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"</tr><tr>\";").Append(Environment.NewLine);
        sbJS.Append("          if (dname == netbios.toLowerCase()) { ").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"<td class=\\\"label\\\" valign=\\\"top\\\" ><b>" + base.GetMessage("lbl ad netbios") + ":</b></td><td class=\\\"value\\\"><input name=\\\"netbios\" + id + \"\\\" class=\\\"ektronTextMedium\\\" type=\\\"text\\\" value=\\\"\" + netbios + \"\\\" size=\\\"55\\\" id=\\\"netbios\" + id + \"\\\" disabled onChange=\\\"javascript:saveDomain(\" + id + \",this.value,\'netbios\')\\\" />\";").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"<div class=\\\"ektronCaption\\\"><input type=\\\"checkbox\\\" name=\\\"use_name\" + id + \"\\\" id=\\\"use_name\" + id + \"\\\" onclick=\\\"(document.getElementById(\'netbios\" + id + \"\').disabled)=(!(document.getElementById(\'netbios\" + id + \"\').disabled));\\\" checked />" + base.GetMessage("lbl ad use domainname") + "</div></td>\";").Append(Environment.NewLine);
        sbJS.Append("          } else { ").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"<td class=\\\"label\\\" valign=\\\"top\\\" ><b>" + base.GetMessage("lbl ad netbios") + ":</b></td><td class=\\\"value\\\"><input name=\\\"netbios\" + id + \"\\\" class=\\\"ektronTextMedium\\\" type=\\\"text\\\" value=\\\"\" + netbios + \"\\\" size=\\\"55\\\" id=\\\"netbios\" + id + \"\\\" onChange=\\\"javascript:saveDomain(\" + id + \",this.value,\'netbios\')\\\" />\";").Append(Environment.NewLine);
        sbJS.Append("               sRet = sRet + \"<div class=\\\"ektronCaption\\\"><input type=\\\"checkbox\\\" name=\\\"use_name\" + id + \"\\\" id=\\\"use_name\" + id + \"\\\" onclick=\\\"(document.getElementById(\'netbios\" + id + \"\').disabled)=(!(document.getElementById(\'netbios\" + id + \"\').disabled));\\\" />" + base.GetMessage("lbl ad use domainname") + "</div></td>\";").Append(Environment.NewLine);
        sbJS.Append("          } ").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"</tr><tr class=\\\"stripe\\\">\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"<td class=\\\"label\\\"><b>" + m_refMsg.GetMessage("username label") + "</b></td><td class=\\\"value\\\"><input class=\\\"ektronTextMedium\\\" name=\\\"user_name\" + id + \"\\\" type=\\\"text\\\" value=\\\"\" + uname + \"\\\" size=\\\"55\\\" id=\\\"user_name\" + id + \"\\\" onChange=\\\"javascript:saveDomain(\" + id + \",this.value,\'uname\')\\\" /> <span class=\\\"ektronCaption\\\">ex: name@domain.com</span></td>\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"</tr><tr>\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"<td class=\\\"label\\\"><b>" + m_refMsg.GetMessage("password label") + "</b></td><td class=\\\"value\\\"><input class=\\\"ektronTextMedium\\\" name=\\\"password\" + id + \"\\\" type=\\\"password\\\" size=\\\"55\\\" id=\\\"password\" + id + \"\\\" value=\\\"\" + pwd + \"\\\" onChange=\\\"javascript:saveDomain(\" + id + \",this.value,\'pwd\')\\\" /></td>\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"</tr><tr class=\\\"stripe\\\">\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"<td class=\\\"label\\\"><b>" + base.GetMessage("lbl ad serverip") + ":</b></td><td class=\\\"value\\\"><input class=\\\"ektronTextMedium\\\" name=\\\"server_ip\" + id + \"\\\" type=\\\"text\\\" value=\\\"\" + sip + \"\\\" size=\\\"55\\\" id=\\\"server_ip\" + id + \"\\\" onChange=\\\"javascript:saveDomain(\" + id + \",this.value,\'sip\')\\\" /></td>\";").Append(Environment.NewLine);
        sbJS.Append("          sRet = sRet + \"</tr><tr><td colspan=\\\"2\\\"><hr/></td></tr></table>\";").Append(Environment.NewLine);
        sbJS.Append("          return sRet; ").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function deleteDomain() {").Append(Environment.NewLine);
        sbJS.Append("            //remove last").Append(Environment.NewLine);
        sbJS.Append("            var cnfm = confirm(\"" + base.GetMessage("delete domain msg") + "\");").Append(Environment.NewLine);
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine);
        sbJS.Append("            {").Append(Environment.NewLine);
        sbJS.Append("              if (arrDomain.length > 0) { ").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainID.pop(); ").Append(Environment.NewLine);
        sbJS.Append("                 arrDomain.pop(); ").Append(Environment.NewLine);
        sbJS.Append("                 // arrDomainValue.pop();").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainDNS.pop();").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainNetBIOS.pop();").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainUsername.pop();").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainPassword.pop();").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainServerIP.pop();").Append(Environment.NewLine);
        sbJS.Append("              }").Append(Environment.NewLine);
        sbJS.Append("              displayDomain();").Append(Environment.NewLine);
        sbJS.Append("            }").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function removeDomain(id) {").Append(Environment.NewLine);
        sbJS.Append("            //remove last").Append(Environment.NewLine);
        sbJS.Append("            var cnfm = confirm(\"" + base.GetMessage("alt are you sure you want to remove this domain?") + "\");").Append(Environment.NewLine);
        sbJS.Append("            if (cnfm == true)").Append(Environment.NewLine);
        sbJS.Append("            {").Append(Environment.NewLine);
        sbJS.Append("              if (arrDomain.length > 0) { ").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainID.splice(id,1); ").Append(Environment.NewLine);
        sbJS.Append("                 arrDomain.pop(); ").Append(Environment.NewLine);
        sbJS.Append("                 // arrDomainValue.splice(id,1);").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainDNS.splice(id,1);").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainNetBIOS.splice(id,1);").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainUsername.splice(id,1);").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainPassword.splice(id,1);").Append(Environment.NewLine);
        sbJS.Append("                 arrDomainServerIP.splice(id,1);").Append(Environment.NewLine);
        sbJS.Append("              }").Append(Environment.NewLine);
        sbJS.Append("              displayDomain(); ").Append(Environment.NewLine);
        sbJS.Append("            }").Append(Environment.NewLine);
        sbJS.Append("        }").Append(Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function SubmitForm() {" + Environment.NewLine);
        sbJS.Append("           var strRet = CheckDomain();" + Environment.NewLine);
        sbJS.Append("           if (strRet.length > 0)" + Environment.NewLine);
        sbJS.Append("           {" + Environment.NewLine);
        sbJS.Append("               alert(strRet);" + Environment.NewLine);
        sbJS.Append("           } else { " + Environment.NewLine);
        sbJS.Append("               document.forms[0].submit();" + Environment.NewLine);
        sbJS.Append("           }" + Environment.NewLine);
        sbJS.Append("        }" + Environment.NewLine);

        sbJS.Append("").Append(Environment.NewLine);

        sbJS.Append("        function CheckDomain() {" + Environment.NewLine);
        sbJS.Append("           var sErr = \"\"; " + Environment.NewLine);
        sbJS.Append("           // var bName = false; " + Environment.NewLine);
        sbJS.Append("           var bDNS = false; " + Environment.NewLine);
        sbJS.Append("           var bUser = false; " + Environment.NewLine);
        sbJS.Append("           var bPass = false; " + Environment.NewLine);
        sbJS.Append("           var bSIP = false; " + Environment.NewLine);

        if (setting_data.ADAuthentication == 1 && setting_data.ADIntegration == true)
        {
            sbJS.Append("           if (arrDomain.length < 1)" + Environment.NewLine);
            sbJS.Append("           {" + Environment.NewLine);
            sbJS.Append("               sErr = \"" + base.GetMessage("js err need domain") + "\";" + Environment.NewLine);
            sbJS.Append("           }" + Environment.NewLine);
        }

        sbJS.Append("           for (var j = 0; j < arrDomain.length; j++)" + Environment.NewLine);
        sbJS.Append("           {" + Environment.NewLine);
        //sbJS.Append("                if ((Trim(arrDomainValue[j]).length < 1) && (bName == false)) {" & Environment.NewLine)
        //sbJS.Append("                    if (sErr.length > 0) {" & Environment.NewLine)
        //sbJS.Append("                       sErr = sErr + ""\n"";" & Environment.NewLine)
        //sbJS.Append("                    }" & Environment.NewLine)
        //sbJS.Append("                    sErr = sErr + ""All domains must have a name."";" & Environment.NewLine)
        //sbJS.Append("                    bName = true; " & Environment.NewLine)
        //sbJS.Append("                }" & Environment.NewLine)

        sbJS.Append("                if ((Trim(arrDomainDNS[j]).length < 1) && (bDNS == false)) {" + Environment.NewLine);
        sbJS.Append("                    if (sErr.length > 0) {" + Environment.NewLine);
        sbJS.Append("                       sErr = sErr + \"\\n\";" + Environment.NewLine);
        sbJS.Append("                    }" + Environment.NewLine);
        sbJS.Append("                    sErr = sErr + \"" + base.GetMessage("js err domains dns") + "\";" + Environment.NewLine);
        sbJS.Append("                    bDNS = true; " + Environment.NewLine);
        sbJS.Append("                }" + Environment.NewLine);

        sbJS.Append("                if ((Trim(arrDomainUsername[j]).length < 1) && (bUser == false)) {" + Environment.NewLine);
        sbJS.Append("                    if (sErr.length > 0) {" + Environment.NewLine);
        sbJS.Append("                       sErr = sErr + \"\\n\";" + Environment.NewLine);
        sbJS.Append("                    }" + Environment.NewLine);
        sbJS.Append("                    sErr = sErr + \"" + base.GetMessage("js err domains uname") + "\";" + Environment.NewLine);
        sbJS.Append("                    bUser = true; " + Environment.NewLine);
        sbJS.Append("                }" + Environment.NewLine);

        sbJS.Append("                if (( (arrDomainPassword[j] != \"          \") && (Trim(arrDomainPassword[j]).length < 1) ) && (bPass == false)) {" + Environment.NewLine);
        sbJS.Append("                    if (sErr.length > 0) {" + Environment.NewLine);
        sbJS.Append("                       sErr = sErr + \"\\n\";" + Environment.NewLine);
        sbJS.Append("                    }" + Environment.NewLine);
        sbJS.Append("                    sErr = sErr + \"" + base.GetMessage("js err domains pwd") + "\";" + Environment.NewLine);
        sbJS.Append("                    bPass = true; " + Environment.NewLine);
        sbJS.Append("                }" + Environment.NewLine);

        sbJS.Append("                if ((Trim(arrDomainServerIP[j]).length < 1) && (bSIP == false)) {" + Environment.NewLine);
        sbJS.Append("                    if (sErr.length > 0) {" + Environment.NewLine);
        sbJS.Append("                       sErr = sErr + \"\\n\";" + Environment.NewLine);
        sbJS.Append("                    }" + Environment.NewLine);
        sbJS.Append("                    sErr = sErr + \"" + base.GetMessage("js err domains dc") + "\";" + Environment.NewLine);
        sbJS.Append("                    bSIP = true; " + Environment.NewLine);
        sbJS.Append("                }" + Environment.NewLine);

        sbJS.Append("           }" + Environment.NewLine);
        sbJS.Append("           return sErr;" + Environment.NewLine);
        sbJS.Append("        }" + Environment.NewLine);

        this.ltr_add_domain_js.Text = sbJS.ToString();
    }

    private void RegisterCss()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }

}



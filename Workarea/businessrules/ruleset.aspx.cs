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
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.RulesEngine;
using Ektron.Cms.Workarea;
using Ektron.Cms.Common;

public partial class businessrules_ruleset : workareabase
{

    protected UI m_rulesUI;
    protected RuleSet[] m_aRuleset;
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected string m_strStyleSheetJS = "";
    protected CommonApi m_refCommon = new Ektron.Cms.CommonApi();
    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);
        try
        {
            if (m_refCommon.RequestInformationRef.IsMembershipUser == 1 || m_refCommon.RequestInformationRef.UserId == 0)
            {
                Response.Redirect("../login.aspx?fromLnkPg=1", false);
                return;
            }
            else
            {
                Page.Title = "CMS Business Rulesets";
                msSelectRule.Text = m_refMsg.GetMessage("lbl select rule");
                msRuleToRemove.Text = m_refMsg.GetMessage("lbl select rule to remove");
                noRulesInSet.Text = m_refMsg.GetMessage("lbl no rules in bussiness ruleset");
                ShowHidden();

                m_rulesUI = new Ektron.Cms.RulesEngine.UI(m_refContentApi.RequestInformationRef);
                m_aRuleset = m_rulesUI.GetAllRulesets();

                if (m_sPageAction == "edit")
                {
                    if (!(Page.IsPostBack))
                    {
                        traddruleset.Visible = true;
                        trgrid.Visible = false;
                        GoGet();
                        AddEditRulesetToolBar();
                        RulesJS();
                        AddEditJS("edit");
                    }
                    else
                    {
                        RulesetSaveHandler();
                        Response.Redirect((string)("ruleset.aspx?action=View&id=" + m_iID.ToString()), false);
                    }
                    traddedit.Visible = true;
                    trgrid.Visible = false;
                }
                else if (m_sPageAction == "view")
                {
                    if (!(Page.IsPostBack))
                    {
                        GoGet();
                        ViewRulesetToolBar();
                        RulesJS();
                        AddEditJS("add");
                    }
                    traddedit.Visible = true;
                    trgrid.Visible = false;
                }
                else if (m_sPageAction == "add")
                {
                    SetAction("");
                    if (!(Page.IsPostBack))
                    {
                        AddRulesetToolBar();
                        traddruleset.Visible = true;
                        trgrid.Visible = false;
                        AddJS();
                        if (!string.IsNullOrEmpty(Request.QueryString["identifier"]))
                        {
                            txtIdentifier.Value = EkFunctions.HtmlEncode(Request.QueryString["identifier"]);
                        }
                        else
                        {
                            tridentifier.Visible = false;
                        }
                    }
                    else
                    {
                        m_iID = RulesetAddHandler();
                        Response.Redirect((string)("ruleset.aspx?action=View&id=" + m_iID.ToString()), false);
                    }
                }
                else if (m_sPageAction == "select")
                {
                    SetAction("");
                    if (!(Page.IsPostBack))
                    {
                        traddedit.Visible = true;
                        SelectRulesToolBar();
                        GetSelectableRules();
                        RulesJS();
                        AddEditJS("select");
                    }
                    else
                    {
                        RulesetSelectHandler();
                        Response.Redirect((string)("ruleset.aspx?action=View&id=" + m_iID.ToString()), false);
                    }
                }
                else if (m_sPageAction == "remove")
                {
                    RulesetDeleteHandler();
                    Response.Redirect("ruleset.aspx", false);
                }
                else
                {
                    traddedit.Visible = false;
                    ShowRulesetToolBar();
                    PopulateViewRuleSetGrid(m_aRuleset);
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
    }
    private void RulesetSelectHandler()
    {
        string[] aruleids;
        RuleSet rsNew = new RuleSet(m_refContentApi.RequestInformationRef, m_iID);
        if (!string.IsNullOrEmpty(Request.Form["txtactiverules"]))
        {
            aruleids = (string[])Strings.Split(Request.Form["txtactiverules"], ",", -1, 0);
        }
        else
        {
            aruleids = (string[])Array.CreateInstance(typeof(string), 0);
        }
        rsNew.AddAssociatedRules(aruleids);
    }
    private long RulesetAddHandler()
    {
        string sName = "";
        RuleSet rsNew = new RuleSet(m_refContentApi.RequestInformationRef);

        if (!string.IsNullOrEmpty(Request.Form["txtRulesetName"]))
            sName = Request.Form["txtRulesetName"];
        rsNew.SetName(sName);
        if (!string.IsNullOrEmpty(Request.Form["txtIdentifier"]))
        {
            rsNew.SetIdentifier(Request.Form["txtIdentifier"]);
        }
        rsNew.Save();
        return rsNew.ID;
    }

    private void RulesetSaveHandler()
    {
        string[] aruleids;
        string[] aenabledruleids;
        RuleSet rsNew = new RuleSet(m_refContentApi.RequestInformationRef, m_iID);
        if (!string.IsNullOrEmpty(Request.Form["txtactiverules"]))
        {
            aruleids = Strings.Split(Request.Form["txtactiverules"], ",", -1, 0);
        }
        else
        {
            aruleids = (string[])Array.CreateInstance(typeof(string), 0);
        }
        if (!string.IsNullOrEmpty(Request.Form["txtenabledrules"]))
        {
            aenabledruleids = Strings.Split(Request.Form["txtenabledrules"], ",", -1, 0);
        }
        else
        {
            aenabledruleids = (string[])Array.CreateInstance(typeof(string), 0);
        }
        rsNew.SetName(Request.Form[txtRulesetName.UniqueID]);
        rsNew.Save();
        rsNew.UpdateRules(aruleids, aenabledruleids);
    }

    private void RulesetDeleteHandler()
    {
        RuleSet rsNew = new RuleSet(m_refContentApi.RequestInformationRef, m_iID);
        rsNew.Delete();
    }

    private void ShowRulesetToolBar()
    {
        bool bAdmin = false;
        bool bRuleEditor = false;
        base.SetTitleBarToMessage("lbl ruleset");
        if (m_refContent == null)
        {
            m_refContent = m_refContentApi.EkContentRef;
        }
        bAdmin = m_refContent.IsAllowed(0, 0, "users", "IsAdmin", 0);
        bRuleEditor = m_refContent.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor), m_refContentApi.UserId, false);
        if (bRuleEditor || bAdmin)
        {
            base.AddButton(m_refContentApi.AppPath + "images/UI/Icons/add.png", "ruleset.aspx?action=Add", m_refMsg.GetMessage("alt addruleset"), m_refMsg.GetMessage("btn addruleset"), "", StyleHelper.AddButtonCssClass, true);
        }
        base.AddHelpButton("view_rulesets");
    }

    private void AddEditRulesetToolBar()
    {
        bool bAdmin = false;
        bool bRuleEditor = false;
        SetAction("edit");
        base.SetTitleBarToMessage("Business Rules");
        if (m_refContent == null)
        {
            m_refContent = m_refContentApi.EkContentRef;
        }
        bAdmin = m_refContent.IsAllowed(0, 0, "users", "IsAdmin", 0);
        bRuleEditor = m_refContent.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor), m_refContentApi.UserId, false);
        if (m_sPageAction == "edit")
        {
            base.SetTitleBarToString((string)(m_refMsg.GetMessage("lbl edit ruleset") + " " + EkFunctions.HtmlEncode(m_aRuleset[0].Name) + ""));
        }
        else
        {
            base.SetTitleBarToString((string)(m_refMsg.GetMessage("alt View Ruleset") + " " + EkFunctions.HtmlEncode(m_aRuleset[0].Name) + ""));
        }

        txtRulesetName.Text = EkFunctions.HtmlEncode(m_aRuleset[0].Name);
		base.AddBackButton((string)("ruleset.aspx?action=View&id=" + m_iID.ToString() + "&LangType=" + m_refContentApi.ContentLanguage.ToString()));

        if (bRuleEditor || bAdmin)
        {
			base.AddButton(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt Click here to save this ruleset"), m_refMsg.GetMessage("lbl save ruleset"), "onclick=\"SaveRule();\" ", StyleHelper.SaveButtonCssClass, true);
			base.AddButton(m_refContentApi.AppPath + "images/UI/Icons/remove.png", "#", m_refMsg.GetMessage("alt Click here to remove rule"), m_refMsg.GetMessage("lbl Remove Rule"), "onclick=\"RuleWizard.removeRuleItem(RuleWizard.getSelectedRule())\" ", StyleHelper.RemoveButtonCssClass);
            if (m_aRuleset[0].Rules.Length > 1)
            {
				base.AddButton(m_refContentApi.AppPath + "images/UI/Icons/arrowUp.png", "#", m_refMsg.GetMessage("alt Click to move up"), m_refMsg.GetMessage("lbl move up"), "onclick=\"RuleWizard.moveRuleItem(\'up\', RuleWizard.getSelectedRule())\"  ", StyleHelper.UpButtonCssClass);
				base.AddButton(m_refContentApi.AppPath + "images/UI/Icons/arrowDown.png", "#", m_refMsg.GetMessage("alt Click to move down"), m_refMsg.GetMessage("lbl Move Down"), "onclick=\"RuleWizard.moveRuleItem(\'down\', RuleWizard.getSelectedRule())\" ", StyleHelper.DownButtonCssClass);
            }
        }
        base.AddHelpButton("edit_ruleset");
    }

    private void ViewRulesetToolBar()
    {
        bool bAdmin = false;
        bool bRuleEditor = false;
        SetAction("view");
        base.SetTitleBarToMessage("lbl ruleset");
        if (m_refContent == null)
        {
            m_refContent = m_refContentApi.EkContentRef;
        }
        bAdmin = m_refContent.IsAllowed(0, 0, "users", "IsAdmin", 0);
        bRuleEditor = m_refContent.IsARoleMember(Convert.ToInt64(Ektron.Cms.Common.EkEnumeration.CmsRoleIds.AdminRuleEditor), m_refContentApi.UserId, false);

        base.SetTitleBarToString((string)(m_refMsg.GetMessage("alt View Ruleset") + " " + EkFunctions.HtmlEncode(m_aRuleset[0].Name) + ""));
        if (bRuleEditor || bAdmin)
        {
			string buttonId = Guid.NewGuid().ToString();
			
            base.AddButtonText("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'file\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"new\">" + m_refMsg.GetMessage("lbl New") + "</span></td>");
            
			buttonId = Guid.NewGuid().ToString();
			
			base.AddButtonText("<td class=\"menuRootItem\" onclick=\"MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseover=\"this.className=\'menuRootItemSelected\';MenuUtil.use(event, \'action\', \'" + buttonId + "\');\" onmouseout=\"this.className=\'menuRootItem\'\"><span id=\"" + buttonId + "\" class=\"action\">" + m_refMsg.GetMessage("lbl Action") + "</span></td>");

            StringBuilder result = new StringBuilder();
            result.Append("<script type=\"text/javascript\">" + Environment.NewLine);
            result.Append("    var actmenu = new Menu( \"action\" );" + Environment.NewLine);
            result.Append("    actmenu.addItem(\"&nbsp;<img src=\'" + m_refContentApi.AppPath + "images/ui/icons/contentEdit.png\' />&nbsp;&nbsp;" + base.GetMessage("lbl edit ruleset") + "\", function() { window.location.href = \'ruleset.aspx?action=Edit&id=" + m_iID.ToString() + "\' } );" + Environment.NewLine);
            result.Append("    actmenu.addItem(\"&nbsp;<img src=\'" + m_refContentApi.AppPath + "images/ui/icons/cogEdit.png\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl Edit Rule") + "\", function() { RuleWizard.showEditRuleWizard(RuleWizard.getSelectedRule()) } );" + Environment.NewLine);
            result.Append("    actmenu.addItem(\"&nbsp;<img src=\'" + m_refContentApi.AppPath + "images/ui/icons/delete.png\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl Delete Ruleset") + "\", function() { var agree = VerifyDelete(); if(agree === true) {window.location.href = \'ruleset.aspx?action=Remove&id=" + m_iID.ToString() + "\';}} );" + Environment.NewLine);
            result.Append("    actmenu.addItem(\"&nbsp;<img src=\'" + m_refContentApi.AppPath + "images/ui/icons/back.png\' />&nbsp;&nbsp;" + m_refMsg.GetMessage("lbl Back") + "\", function() { window.location.href = \'ruleset.aspx\'} );" + Environment.NewLine);

            result.Append("    MenuUtil.add( actmenu );" + Environment.NewLine);
            //end
            result.Append("    </script>" + Environment.NewLine);
            ltrrulejs.Text = result.ToString();
        }
        base.AddHelpButton("view_ruleset");
    }

    private void SelectRulesToolBar()
    {
        base.SetTitleBarToString(m_refMsg.GetMessage("lbl Add Existing Ruleset"));
		base.AddBackButton((string)("ruleset.aspx?action=View&id=" + m_iID.ToString()));
        base.AddButton(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt Click here to add these rules"), m_refMsg.GetMessage("lbl save ruleset"), "onclick=\"SaveRule();\" ", StyleHelper.SaveButtonCssClass, true);
        base.AddHelpButton("SelectExistingRule");
    }

    private void AddRulesetToolBar()
    {
        base.SetTitleBarToString(m_refMsg.GetMessage("btn addruleset"));
		base.AddBackButton("ruleset.aspx");
		base.AddButton(m_refContentApi.AppPath + "images/UI/Icons/save.png", "#", m_refMsg.GetMessage("alt Click here to save this ruleset"), m_refMsg.GetMessage("lbl save ruleset"), "onclick=\"return VerifyForm()\" ", StyleHelper.SaveButtonCssClass, true);
        base.AddHelpButton("AddRuleset");
    }

    private void PopulateViewRuleSetGrid(RuleSet[] aruleset)
    {
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        int iCount;

        ViewRuleSetGrid.ShowHeader = true;

        colBound.DataField = "ID";
        colBound.HeaderText = m_refMsg.GetMessage("rulesetheader id");
        colBound.HeaderStyle.CssClass = "center widthNarrow";
        colBound.ItemStyle.CssClass = "center";
        ViewRuleSetGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "NAME";
        colBound.HeaderText = m_refMsg.GetMessage("rulesetheader name");
        colBound.ItemStyle.Wrap = false;
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        ViewRuleSetGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "IDENTIFIER";
        colBound.HeaderText = m_refMsg.GetMessage("rulesetheader identifier");
        colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
        colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
        // ViewRuleSetGrid.Columns.Add(colBound)

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("NAME", typeof(string)));
        dt.Columns.Add(new DataColumn("IDENTIFIER", typeof(string)));

        int i = 0;
        iCount = System.Convert.ToInt32(aruleset.Length - 1);
        for (i = 0; i <= iCount; i++)
        {
            if (!(aruleset[i] == null))
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"ruleset.aspx?action=View&id=" + aruleset[i].ID.ToString() + "\">" + aruleset[i].ID.ToString() + "</a>";
                dr[1] = "<a href=\"ruleset.aspx?action=View&id=" + aruleset[i].ID.ToString() + "\">" + EkFunctions.HtmlEncode(aruleset[i].Name) + "</a>";
                dr[2] = EkFunctions.HtmlEncode(aruleset[i].Identifier);
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        ViewRuleSetGrid.DataSource = dv;
        ViewRuleSetGrid.DataBind();
    }

    private void AddEditJS(string actiontype)
    {
        StringBuilder sbAEJS = new StringBuilder();

        // register JS files
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "businessrules/includes/com.ektron.ui.rules.js", "EktronRulesJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.net.http.js", "EktronTreeNetHttpJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.cookie.js", "EktronTreeUtilsCookieJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.debug.js", "EktronTreeUtilsDebugJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.dom.js", "EktronTreeUtilsDomJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.form.js", "EktronTreeUtilsFormJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.log.js", "EktronTreeUtilsLogJS");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.querystring.js", "EktronTreeUtilsQueryString");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.string.js", "EktronTreeUtilsString");
        Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "tree/js/com.ektron.utils.xml.js", "EktronTreeUtilsXmlJS");

        // register CSS files
        Ektron.Cms.API.Css.RegisterCss(this, m_refContentApi.AppPath + "businessrules/css/com.ektron.rules.wizard.css", "EktronBusinessRulesWizardCss");

        // build JS
        sbAEJS.Append("<script type=\"text/javascript\">" + Environment.NewLine);
        if (actiontype != "select")
        {
            sbAEJS.Append(AJAXcheck(GetResponseString("VerifyRule"), "action=existingruleset&rid=" + m_iID.ToString() + "&rname=\' + input + \'")).Append(Environment.NewLine);
        }
        sbAEJS.Append("function VerifyDelete()" + Environment.NewLine);
        sbAEJS.Append("{" + Environment.NewLine);
        sbAEJS.Append("    var agree=confirm(\'" + m_refMsg.GetMessage("alt delete this ruleset?") + "\');" + Environment.NewLine);
        sbAEJS.Append("    if (agree) {" + Environment.NewLine);
        sbAEJS.Append("	     return true;" + Environment.NewLine);
        sbAEJS.Append("    } else {" + Environment.NewLine);
        sbAEJS.Append("	     return false;" + Environment.NewLine);
        sbAEJS.Append("    }" + Environment.NewLine);
        sbAEJS.Append("}" + Environment.NewLine);
        sbAEJS.Append("function SaveRule()" + Environment.NewLine);
        sbAEJS.Append("{" + Environment.NewLine);
        if (actiontype != "select")
        {
            sbAEJS.Append("    var stext = Trim(document.getElementById(\'txtRulesetName\').value);" + Environment.NewLine);
            sbAEJS.Append("    checkRuleset(stext,\'\'); " + Environment.NewLine);
            sbAEJS.Append("    return bexists; " + Environment.NewLine);
            sbAEJS.Append("}" + Environment.NewLine);
            sbAEJS.Append("function VerifyRule()" + Environment.NewLine);
            sbAEJS.Append("{" + Environment.NewLine);
            sbAEJS.Append("    var stext = Trim(document.getElementById(\'txtRulesetName\').value);" + Environment.NewLine);
            sbAEJS.Append("    if (stext.length > 0) {" + Environment.NewLine);
            sbAEJS.Append("    } else {" + Environment.NewLine);
            sbAEJS.Append("    alert(\'" + m_refMsg.GetMessage("alt a ruleset name is required!") + "\');" + Environment.NewLine);
            sbAEJS.Append("    return false;    " + Environment.NewLine);
            sbAEJS.Append("    }" + Environment.NewLine);
            sbAEJS.Append("    if (!CheckRuleSetNameForillegalChar()) {" + Environment.NewLine);
            sbAEJS.Append("         return false;    " + Environment.NewLine);
            sbAEJS.Append("    }" + Environment.NewLine);
            sbAEJS.Append("function CheckRuleSetNameForillegalChar() {" + Environment.NewLine);
            sbAEJS.Append("   var val = document.getElementById(\'txtRulesetName\').value;" + Environment.NewLine);
            sbAEJS.Append("   if ((val.indexOf(\"\\\\\") > 0) || (val.indexOf(\"/\") > 0) || (val.indexOf(\":\") > 0)||(val.indexOf(\"*\") > 0) || (val.indexOf(\"?\") > 0)|| (val.indexOf(\"\\\"\") > 0) || (val.indexOf(\"<\") > 0)|| (val.indexOf(\">\") > 0) || (val.indexOf(\"|\") > 0) || (val.indexOf(\"&\") > 0) || (val.indexOf(\"\\\'\") > 0))" + Environment.NewLine);
            sbAEJS.Append("   {" + Environment.NewLine);
            sbAEJS.Append("       alert(\"" + m_refMsg.GetMessage("alert msg ruleset name cant") + " " + "(\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
            sbAEJS.Append("       return false;" + Environment.NewLine);
            sbAEJS.Append("   }" + Environment.NewLine);
            sbAEJS.Append("   return true;" + Environment.NewLine);
            sbAEJS.Append("}" + Environment.NewLine);
        }
        sbAEJS.Append("    var sactivetext = \"\";" + Environment.NewLine);
        sbAEJS.Append("    var senabledtext = \"\";" + Environment.NewLine);
        sbAEJS.Append("    for (i = 0; i < ruleset.length; i++) {" + Environment.NewLine);
        sbAEJS.Append("        if(ruleset[i].active == true) {" + Environment.NewLine);
        sbAEJS.Append("            sactivetext += ruleset[i].id + \",\";" + Environment.NewLine);
        sbAEJS.Append("        }else {" + Environment.NewLine);
        sbAEJS.Append("            senabledtext += ruleset[i].id + \",\";" + Environment.NewLine);
        sbAEJS.Append("        }" + Environment.NewLine);
        sbAEJS.Append("    }" + Environment.NewLine);
        sbAEJS.Append("    if (sactivetext.substr((sactivetext.length-1),1)) {" + Environment.NewLine);
        sbAEJS.Append("        sactivetext = sactivetext.substr(0,(sactivetext.length-1));" + Environment.NewLine);
        sbAEJS.Append("    }" + Environment.NewLine);
        sbAEJS.Append("    if (senabledtext.substr((senabledtext.length-1),1)) {" + Environment.NewLine);
        sbAEJS.Append("        senabledtext = senabledtext.substr(0,(senabledtext.length-1));" + Environment.NewLine);
        sbAEJS.Append("    }" + Environment.NewLine);
        sbAEJS.Append("    document.getElementById(\'txtactiverules\').value = sactivetext;" + Environment.NewLine);
        sbAEJS.Append("    document.getElementById(\'txtenabledrules\').value = senabledtext;" + Environment.NewLine);
        sbAEJS.Append("    document.forms[0].submit();" + Environment.NewLine);
        sbAEJS.Append("}" + Environment.NewLine);
        sbAEJS.Append("function noenter(e) {" + Environment.NewLine);
        sbAEJS.Append("    if (e && e.keyCode == 13) {" + Environment.NewLine);
        sbAEJS.Append("        return SaveRule(); " + Environment.NewLine);
        sbAEJS.Append("    }" + Environment.NewLine);
        sbAEJS.Append("}" + Environment.NewLine);
        sbAEJS.Append("</script>" + Environment.NewLine);
        txtRulesetName.Attributes.Add("onkeypress", "javascript:return noenter(event);");
        ltrjs.Text = sbAEJS.ToString();
        sbAEJS = null;
    }

    private void AddJS()
    {
        StringBuilder sbaddJS = new StringBuilder();
        sbaddJS.Append("<script type=\"text/javascript\">" + Environment.NewLine);

        sbaddJS.Append(AJAXcheck(GetResponseString("VerifyAdd"), "action=existingruleset&rid=" + m_iID.ToString() + "&rname=\' + input + \'")).Append(Environment.NewLine);
        sbaddJS.Append("function VerifyForm()" + Environment.NewLine);
        sbaddJS.Append("{" + Environment.NewLine);
        sbaddJS.Append("    var stext = Trim(document.getElementById(\'txtRulesetName\').value);" + Environment.NewLine);
        sbaddJS.Append("    checkRuleset(stext,\'\'); " + Environment.NewLine);
        sbaddJS.Append("    return bexists; " + Environment.NewLine);
        sbaddJS.Append("}" + Environment.NewLine);

        sbaddJS.Append("function VerifyAdd()" + Environment.NewLine);
        sbaddJS.Append("{" + Environment.NewLine);
        sbaddJS.Append("    var stext = Trim(document.getElementById(\'txtRulesetName\').value);" + Environment.NewLine);
        sbaddJS.Append("    if (stext.length > 0) {" + Environment.NewLine);
        sbaddJS.Append("        if (!CheckRuleSetNameForillegalChar()) {" + Environment.NewLine);
        sbaddJS.Append("           return false;    " + Environment.NewLine);
        sbaddJS.Append("        } else { " + Environment.NewLine);
        sbaddJS.Append("            document.forms[0].submit();" + Environment.NewLine);
        sbaddJS.Append("        } " + Environment.NewLine);
        sbaddJS.Append("    } else {" + Environment.NewLine);
        sbaddJS.Append("    alert(\'" + m_refMsg.GetMessage("alt a ruleset name is required!") + "\');" + Environment.NewLine);
        sbaddJS.Append("    return false;    " + Environment.NewLine);
        sbaddJS.Append("    }" + Environment.NewLine);
        sbaddJS.Append("}" + Environment.NewLine);
        sbaddJS.Append("function CheckRuleSetNameForillegalChar() {" + Environment.NewLine);
        sbaddJS.Append("   var val = document.getElementById(\'txtRulesetName\').value;" + Environment.NewLine);
        sbaddJS.Append("   if ((val.indexOf(\"\\\\\") > -1) || (val.indexOf(\"/\") > -1) || (val.indexOf(\":\") > -1)||(val.indexOf(\"*\") > -1) || (val.indexOf(\"?\") > -1)|| (val.indexOf(\"\\\"\") > -1) || (val.indexOf(\"<\") > -1)|| (val.indexOf(\">\") > -1) || (val.indexOf(\"|\") > -1) || (val.indexOf(\"&\") > -1) || (val.indexOf(\"\\\'\") > -1))" + Environment.NewLine);
        sbaddJS.Append("   {" + Environment.NewLine);
        sbaddJS.Append("       alert(\"" + m_refMsg.GetMessage("alert msg ruleset name cant") + " " + "(\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbaddJS.Append("       return false;" + Environment.NewLine);
        sbaddJS.Append("   }" + Environment.NewLine);
        sbaddJS.Append("   return true;" + Environment.NewLine);
        sbaddJS.Append("}" + Environment.NewLine);
        sbaddJS.Append("function noenter(e) {" + Environment.NewLine);
        sbaddJS.Append("    var iKey = e.keyCode; " + Environment.NewLine);
        sbaddJS.Append("    iKey = iKey + 1; " + Environment.NewLine);
        sbaddJS.Append("    iKey = iKey - 1; " + Environment.NewLine);
        sbaddJS.Append("    if (e && (iKey == 13)) {" + Environment.NewLine);
        sbaddJS.Append("        VerifyForm(); " + Environment.NewLine);
        sbaddJS.Append("        return false; " + Environment.NewLine);
        sbaddJS.Append("    }" + Environment.NewLine);
        sbaddJS.Append("}" + Environment.NewLine);
        sbaddJS.Append("</script>" + Environment.NewLine);
        txtRulesetName.Attributes.Add("onkeypress", "javascript:return noenter(event);");
        ltrjs.Text = sbaddJS.ToString();
        sbaddJS = null;
    }

    private void RulesJS()
    {
        StringBuilder sbruleJS = new StringBuilder();
        sbruleJS.Append("            <script type=\"text/javascript\">" + Environment.NewLine);
        sbruleJS.Append("            var ruleset = " + Environment.NewLine);
        sbruleJS.Append("            [" + Environment.NewLine);
        if (m_aRuleset[0].Rules.Length > 0)
        {
            for (int i = 0; i <= (m_aRuleset[0].Rules.Length - 1); i++)
            {
                sbruleJS.Append("	            {" + Environment.NewLine);
                sbruleJS.Append("		            name: \"" + m_aRuleset[0].Rules[i].RuleName.Replace("\"", "\\\"") + "\"," + Environment.NewLine);
                sbruleJS.Append("		            order: " + i.ToString() + "," + Environment.NewLine);
                sbruleJS.Append("		            active: " + m_aRuleset[0].Rules[i].Active.ToString().ToLower() + "," + Environment.NewLine);
                sbruleJS.Append("		            id: " + m_aRuleset[0].Rules[i].RuleID.ToString() + "" + Environment.NewLine);
                if (i == (m_aRuleset[0].Rules.Length - 1))
                {
                    sbruleJS.Append("	            }" + Environment.NewLine);
                }
                else
                {
                    sbruleJS.Append("	            }," + Environment.NewLine);
                }
            }
        }
        sbruleJS.Append("            ]" + Environment.NewLine);
        sbruleJS.Append("" + Environment.NewLine);
        sbruleJS.Append("            RuleWizard.displayScreen(\"ruleset\", ruleset);" + Environment.NewLine);
        sbruleJS.Append("            </script>" + Environment.NewLine);
        sbruleJS.Append("<script type=\"text/javascript\">" + Environment.NewLine);
        sbruleJS.Append("    var filemenu = new Menu( \"file\" );" + Environment.NewLine);
        sbruleJS.Append("    filemenu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + m_refContentApi.AppPath + "images/ui/icons/cogAdd.png" + "\' />&nbsp;&nbsp;" + base.GetMessage("btn addnewrule") + "\", function() { window.location.href = \'wizard-with-steps.aspx?action=add&rulesetid=" + m_iID.ToString() + "\' } );" + Environment.NewLine);
        //sbruleJS.Append("    filemenu.addBreak();" & Environment.NewLine)
        sbruleJS.Append("    filemenu.addItem(\"&nbsp;<img valign=\'center\' src=\'" + m_refContentApi.AppPath + "images/ui/icons/cog.png" + "\' />&nbsp;&nbsp;" + base.GetMessage("btn addexistrule") + "\", function() { window.location.href = \'ruleset.aspx?action=select&id=" + m_iID.ToString() + "\' } );" + Environment.NewLine);
        sbruleJS.Append("" + Environment.NewLine);
        sbruleJS.Append("    MenuUtil.add( filemenu );" + Environment.NewLine);
        sbruleJS.Append("    </script>" + Environment.NewLine);
        sbruleJS.Append("" + Environment.NewLine);

        ltrrulejs.Text += sbruleJS.ToString();
        sbruleJS = null;
    }

    private void GoGet()
    {
        RuleSet rsNew;

        rsNew = new RuleSet(m_refContentApi.RequestInformationRef, m_iID);
        m_aRuleset = new RuleSet[2];
        m_aRuleset[0] = rsNew;
    }

    private void GetSelectableRules()
    {
        RuleSet rsNew;

        rsNew = new RuleSet(m_refContentApi.RequestInformationRef);
        m_aRuleset = new RuleSet[2];
        m_aRuleset[0] = rsNew;

        m_aRuleset[0].Rules = m_rulesUI.GetSelectableRules(m_iID);
    }

    private void ShowHidden()
    {
        ltrrulesetid.Text = "<input type=\"hidden\" id=\"rulesetid\" name=\"rulesetid\" value=\"" + m_iID.ToString() + "\" />";
    }

    private void SetAction(string stype)
    {
        ltr_action_js.Text = Environment.NewLine + "<script type=\"text/javascript\">var s_action = \"" + stype.ToLower() + "\";</script>";
    }

    private string AJAXcheck(string sResponse, string sURLQuery)
    {
        base.AJAX.ResponseJS = sResponse;
        base.AJAX.URLQuery = sURLQuery;
        base.AJAX.FunctionName = "checkRuleset";
        return base.AJAX.Render();
    }

    private string GetResponseString(string nextfunction)
    {
        System.Text.StringBuilder sbAEJS = new System.Text.StringBuilder();
        sbAEJS.Append("    if (response == \'1\'){").Append(Environment.NewLine);
        sbAEJS.Append("	        alert(\'This ruleset already exists.\');").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine);
        sbAEJS.Append("    }else{").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = ").Append(nextfunction).Append("();").Append(Environment.NewLine);
        sbAEJS.Append("    } ").Append(Environment.NewLine);
        return sbAEJS.ToString();
    }
}

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
using Ektron.Cms.RulesEngine;
using System.Xml;
using Ektron.Cms.Common;

public partial class wizard_with_steps : System.Web.UI.Page
{
    protected StyleHelper m_refStyle = new StyleHelper();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected string AppImgPath = "";
    protected UI m_rulesUI;
    protected Ektron.Cms.RulesEngine.Rule m_rulesengineRule;
    protected Template[] m_acontem;
    protected Template[] m_aacttem;
    protected TemplateParam[] m_aruleparam = null;
    protected TemplateParam[] m_tpacondition;
    protected TemplateParam[] m_tpaaction;
    protected ContentAPI m_refContentApi = new ContentAPI();
    protected string m_strStyleSheetJS = "";
    protected string m_sPageAction = "";
    protected long m_iID = 0;
    protected long m_iRuleID = 0;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        m_refMsg = m_refContentApi.EkMsgRef;
        AppImgPath = m_refContentApi.AppImgPath;
        ltr_Title.Text = m_refMsg.GetMessage("lbl cms business rulesets");

        RegisterResources();
        try
        {
			Utilities.ValidateUserLogin();
            if (m_refContentApi.RequestInformationRef.IsMembershipUser == 1 || m_refContentApi.RequestInformationRef.UserId == 0)
            {
                Response.Redirect((string)("../reterror.aspx?info=" + m_refMsg.GetMessage("msg login cms user")), false);
                return;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["action"]))
            {
                m_sPageAction = (string)(Request.QueryString["action"].ToLower());
            }
            else
            {
                m_sPageAction = "";
            }
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                m_iRuleID = Convert.ToInt64(Request.QueryString["id"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["rulesetid"]))
            {
                m_iID = Convert.ToInt64(Request.QueryString["rulesetid"].ToString());
            }

            m_rulesUI = new Ektron.Cms.RulesEngine.UI(m_refContentApi.RequestInformationRef);
            m_acontem = m_rulesUI.GetAllConditionTemplates();
            m_aacttem = m_rulesUI.GetAllActionTemplates();
            m_tpacondition = m_rulesUI.GetAllConditionTemplateParams();
            m_tpaaction = m_rulesUI.GetAllActionTemplateParams();

            OutputJS();
            ShowHidden();

            if (m_sPageAction == "edit")
            {
                m_aruleparam = m_rulesUI.GetRuleParams(m_iRuleID);
                m_rulesengineRule = new Ektron.Cms.RulesEngine.Rule(m_refContentApi.RequestInformationRef);
                m_rulesengineRule.load(m_iRuleID);
                ruleNameText.Value = m_rulesengineRule.RuleName;
                cmswizard ucWizard;
                ucWizard = (cmswizard)(LoadControl("../controls/wizard/wizard.ascx"));
                ucWizard.AllowSelect = true;
                ucWizard.ID = "ProgressSteps";
                pnlwizard.Controls.Add(ucWizard);
                BuildTemplateJS("edit");
                ShowRuleToolBar();
            }
            else if (m_sPageAction == "view")
            {
                m_aruleparam = m_rulesUI.GetRuleParams(m_iRuleID);
                m_rulesengineRule = new Ektron.Cms.RulesEngine.Rule(m_refContentApi.RequestInformationRef);
                m_rulesengineRule.load(m_iRuleID);
                ruleNameText.Value = m_rulesengineRule.RuleName;
                BuildTemplateJS("view");
                ShowRuleToolBar();
            }
            else if (m_sPageAction == "add")
            {
                BuildTemplateJS("add");
                cmswizard ucWizard;
                ucWizard = (cmswizard)(LoadControl("../controls/wizard/wizard.ascx"));
                ucWizard.AllowSelect = false; // do not allow skip for add
                ucWizard.ID = "ProgressSteps";
                pnlwizard.Controls.Add(ucWizard);
                ShowRuleToolBar();
            }
            else if (m_sPageAction == "process")
            {
                BuildTemplateJS("add");
                ProcessHandler();
            }
        }
        catch (Exception ex)
        {
            //Response.Write(ex.Message & ex.StackTrace)
            Utilities.ShowError(ex.Message);
        }
    }

    private void ProcessHandler()
    {
        int i = 0;
        string sRulename = "New Rule";
        string sLogicalOperator = "and";
        string[] aParam = null;
        string sTemplateType = "";
        long iTemplateID = 0;
        int iOrder = 0;
        string sParam = "";
        string sValue = "";
        XmlDocument sXMLdoc;
        XmlElement xmlelem;
        XmlAttribute xmlatt;
        string stmpText = "";
        bool bWroteCondition = false;
        bool bWroteActionTrue = false;
        bool bNeedWriteActionTrue = false;
        bool bNeedWriteActionFalse = false;
        long icurTemplateID = -1;
        Ektron.Cms.RulesEngine.TemplateParam tpTemParam;

        if (!string.IsNullOrEmpty(Request.QueryString["rule_name"]))
            sRulename = Request.QueryString["rule_name"].ToString();
        if (sRulename.ToString().Length == 0)
        {
            throw (new Exception("You need a rule name."));
        }

        if (m_iRuleID > 0) //edit
        {
            m_rulesUI.ClearRuleParams(m_iRuleID); //clear any params for the rule
            m_rulesengineRule = new Ektron.Cms.RulesEngine.Rule(m_refContentApi.RequestInformationRef);
            m_rulesengineRule.load(m_iRuleID);
            m_rulesengineRule.SetName(sRulename);
        }
        else //add
        {
            if (m_iID == 0)
            {
                throw (new Exception("No associated Rule Set ID"));
            }
            m_rulesengineRule = new Ektron.Cms.RulesEngine.Rule(sRulename, "", true);
            m_rulesengineRule.Initialize(m_refContentApi.RequestInformationRef);
            m_rulesengineRule.AddAndAssociate(m_iID);
            m_iRuleID = m_rulesengineRule.RuleID;
        }

        sXMLdoc = new XmlDocument();
        for (i = 0; i <= (Request.QueryString.Count - 2); i++)
        {
            if (!string.IsNullOrEmpty(Request.QueryString[i]))
            {
                if ((string)(Request.QueryString.Keys[i].ToLower().Trim()) == "rule_name")
                {
                    xmlelem = sXMLdoc.CreateElement("rule");
                    xmlatt = sXMLdoc.CreateAttribute("name");
                    xmlatt.Value = sRulename;
                    xmlelem.Attributes.Append(xmlatt);
                    sXMLdoc.AppendChild(xmlelem);
                }
                else if ((string)(Request.QueryString.Keys[i].ToLower().Trim()) == "logical_operator")
                {
                    sLogicalOperator = Request.QueryString[i];
                    xmlelem = sXMLdoc.CreateElement("condition");
                    sXMLdoc.ChildNodes.Item(0).AppendChild(xmlelem);
                    xmlelem = sXMLdoc.CreateElement("predicate");
                    xmlatt = sXMLdoc.CreateAttribute("type");
                    xmlatt.Value = sLogicalOperator.ToLower();
                    xmlelem.Attributes.Append(xmlatt);
                    sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).AppendChild(xmlelem);

                    xmlelem = sXMLdoc.CreateElement("actions");
                    xmlatt = sXMLdoc.CreateAttribute("case");
                    xmlatt.Value = "true";
                    xmlelem.Attributes.Append(xmlatt);
                    sXMLdoc.ChildNodes.Item(0).AppendChild(xmlelem);

                    xmlelem = sXMLdoc.CreateElement("actions");
                    xmlatt = sXMLdoc.CreateAttribute("case");
                    xmlatt.Value = "false";
                    xmlelem.Attributes.Append(xmlatt);
                    sXMLdoc.ChildNodes.Item(0).AppendChild(xmlelem);
                }
                else
                {
                    aParam = Strings.Split(Request.QueryString.Keys[i], "_", -1, 0);
                    if (aParam.Length == 4)
                    {
                        sTemplateType = aParam[0].ToString();
                        iTemplateID = Convert.ToInt64(aParam[1].ToString());
                        iOrder = Convert.ToInt32(aParam[2].ToString());
                        sParam = aParam[3].ToString();
                        sValue = Strings.Replace(Request.QueryString[i], "&", "&amp;", 1, -1, 0);
                        if (ValidateValue(sTemplateType, iTemplateID, sParam, sValue))
                        {
                            switch (sTemplateType.ToLower())
                            {
                                case "condition":
                                    tpTemParam = new Ektron.Cms.RulesEngine.TemplateParam(iTemplateID, sParam, sValue, Ektron.Cms.Common.EkEnumeration.CustomAttributeValueTypes.String, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.Condition);
                                    m_rulesUI.AddRuleParam(m_iRuleID, tpTemParam);
                                    if (icurTemplateID == -1 || (icurTemplateID == iTemplateID))
                                    {
                                        if (icurTemplateID == -1)
                                        {
                                            FindTemplateIndex(m_acontem, iTemplateID);
                                            stmpText = Strings.Replace(m_acontem[FindTemplateIndex(m_acontem, iTemplateID)].Predicate, "<predicate type=", "<predicate template=\"" + iTemplateID.ToString() + "\" type=", 1, -1, 0);
                                        }
                                        else
                                        {
                                            stmpText = stmpText.Replace("<predicate type=", "<predicate template=\"" + iTemplateID.ToString() + "\" type=");
                                        }
                                        stmpText = stmpText.Replace("[" + sParam.ToLower() + "]", sValue);
                                    }
                                    else if (!(icurTemplateID == iTemplateID))
                                    {
                                        //write out
                                        sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText;
                                        stmpText = ""; //reset
                                        //get values
                                        stmpText = Strings.Replace(m_acontem[FindTemplateIndex(m_acontem, iTemplateID)].Predicate, "<predicate type=", "<predicate template=\"" + iTemplateID.ToString() + "\" type=", 1, -1, 0);
                                        stmpText = stmpText.Replace("[" + sParam.ToLower() + "]", sValue);
                                    }
                                    icurTemplateID = iTemplateID;
                                    break;
                                case "actiontrue":
                                    tpTemParam = new Ektron.Cms.RulesEngine.TemplateParam(iTemplateID, sParam, sValue, Ektron.Cms.Common.EkEnumeration.CustomAttributeValueTypes.String, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionTrue);
                                    m_rulesUI.AddRuleParam(m_iRuleID, tpTemParam);
                                    bNeedWriteActionTrue = true;
                                    if (!(bWroteCondition)) //this is the first time we come to the actions
                                    {
                                        //Write out the conditions
                                        sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText;
                                        stmpText = ""; //reset
                                        icurTemplateID = -1;
                                        bWroteCondition = true;
                                    }
                                    if (icurTemplateID == -1 || (icurTemplateID == iTemplateID))
                                    {
                                        if (icurTemplateID == -1)
                                        {
                                            stmpText = Strings.Replace(m_aacttem[FindTemplateIndex(m_aacttem, iTemplateID)].Predicate, "<action type=", "<action template=\"" + iTemplateID.ToString() + "\" type=", 1, -1, 0);
                                        }
                                        else
                                        {
                                            stmpText = stmpText.Replace("<action type=", "<action template=\"" + iTemplateID.ToString() + "\" type=");
                                        }
                                        stmpText = stmpText.Replace("[" + sParam.ToLower() + "]", sValue);
                                    }
                                    else if (!(icurTemplateID == iTemplateID))
                                    {
                                        //write out
                                        sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(1).InnerXml += stmpText;
                                        //sXMLdoc.ChildNodes.Item(1).InnerXml += stmpText
                                        stmpText = ""; //reset
                                        //get values
                                        stmpText = Strings.Replace(m_aacttem[FindTemplateIndex(m_aacttem, iTemplateID)].Predicate, "<action type=", "<action template=\"" + iTemplateID.ToString() + "\" type=", 1, -1, 0);
                                        stmpText = stmpText.Replace("[" + sParam.ToLower() + "]", sValue);
                                    }
                                    icurTemplateID = iTemplateID;
                                    break;
                                case "actionfalse":
                                    tpTemParam = new Ektron.Cms.RulesEngine.TemplateParam(iTemplateID, sParam, sValue, Ektron.Cms.Common.EkEnumeration.CustomAttributeValueTypes.String, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionFalse);
                                    m_rulesUI.AddRuleParam(m_iRuleID, tpTemParam);
                                    bNeedWriteActionFalse = true;
                                    if (!(bWroteCondition))
                                    {
                                        sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText;
                                        stmpText = ""; //reset
                                        icurTemplateID = -1;
                                        bWroteCondition = true;
                                    }
                                    if ((!(bWroteActionTrue)) && bNeedWriteActionTrue)
                                    {
                                        sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(1).InnerXml += stmpText;
                                        stmpText = ""; //reset
                                        icurTemplateID = -1;
                                        bWroteActionTrue = true;
                                    }
                                    if (icurTemplateID == -1 || (icurTemplateID == iTemplateID))
                                    {
                                        if (icurTemplateID == -1)
                                        {
                                            stmpText = Strings.Replace(m_aacttem[FindTemplateIndex(m_aacttem, iTemplateID)].Predicate, "<action type=", "<action template=\"" + iTemplateID.ToString() + "\" type=", 1, -1, 0);
                                        }
                                        else
                                        {
                                            stmpText = stmpText.Replace("<action type=", "<action template=\"" + iTemplateID.ToString() + "\" type=");
                                        }
                                        stmpText = stmpText.Replace("[" + sParam.ToLower() + "]", sValue);
                                    }
                                    else if (!(icurTemplateID == iTemplateID))
                                    {
                                        //write out
                                        sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(2).InnerXml += stmpText;
                                        //sXMLdoc.ChildNodes.Item(1).InnerXml += stmpText
                                        stmpText = ""; //reset
                                        //get values
                                        stmpText = Strings.Replace(m_aacttem[FindTemplateIndex(m_aacttem, iTemplateID)].Predicate, "<action type=", "<action template=\"" + iTemplateID.ToString() + "\" type=", 1, -1, 0);
                                        stmpText = stmpText.Replace("[" + sParam.ToLower() + "]", sValue);
                                    }
                                    icurTemplateID = iTemplateID;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        if (!(bWroteCondition))
        {
            //Write out the conditions
            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml += stmpText;
        }
        if ((!(bWroteActionTrue)) && bNeedWriteActionTrue)
        {
            //Write out the actiontrue
            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(1).InnerXml += stmpText;
        }
        if (bNeedWriteActionFalse)
        {
            sXMLdoc.ChildNodes.Item(0).ChildNodes.Item(2).InnerXml += stmpText;
        }

        m_rulesengineRule.UpdateXML(sXMLdoc.InnerXml.ToString());

        Response.Redirect((string)("ruleset.aspx?action=View&id=" + m_iID.ToString()), false);
    }
    private bool ValidateValue(string TemplateType, long templateID, string ParamName, string ParamValue)
    {
        bool bret = false;
        if ((string)(TemplateType.ToLower()) == "condition")
        {
            for (int i = 0; i <= (m_tpacondition.Length - 1); i++)
            {
                if ((templateID == m_tpacondition[i].TemplateID) && (m_tpacondition[i].ParamName.ToLower() == ParamName.ToLower()))
                {
                    bret = System.Convert.ToBoolean(m_tpacondition[i].Validate(ParamValue));
                    break;
                }
            }
        }
        else if ((string)(TemplateType.ToLower()) == "actiontrue")
        {
            for (int i = 0; i <= (m_tpaaction.Length - 1); i++)
            {
                if ((templateID == m_tpaaction[i].TemplateID) && (m_tpaaction[i].ParamName.ToLower() == ParamName.ToLower()))
                {
                    bret = System.Convert.ToBoolean(m_tpaaction[i].Validate(ParamValue));
                    break;
                }
            }
        }
        else if ((string)(TemplateType.ToLower()) == "actionfalse")
        {
            for (int i = 0; i <= (m_tpaaction.Length - 1); i++)
            {
                if ((templateID == m_tpaaction[i].TemplateID) && (m_tpaaction[i].ParamName.ToLower() == ParamName.ToLower()))
                {
                    bret = System.Convert.ToBoolean(m_tpaaction[i].Validate(ParamValue));
                    break;
                }
            }
        }
        return bret;
    }
    private void ShowHidden()
    {
        ltrhidden.Text = "<input type=\"hidden\" id=\"rulesetid\" name=\"rulesetid\" value=\"" + m_iID.ToString() + "\"/>" + Environment.NewLine;
        ltrhidden.Text += "<input type=\"hidden\" id=\"action\" name=\"action\" value=\"" + EkFunctions.HtmlEncode(m_sPageAction.ToLower()) + "\"/>" + Environment.NewLine;
        ltrhidden.Text += "<input type=\"hidden\" id=\"ruleid\" name=\"ruleid\" value=\"" + m_iRuleID.ToString() + "\"/>" + Environment.NewLine;
    }
    private void ShowRuleToolBar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        if (this.m_sPageAction == "view")
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("lbl view rule"));
        }
        else
        {
            divTitleBar.InnerHtml = m_refStyle.GetTitleBar("Add/Edit Rules");
        }
        result.Append("<table><tr>");
        result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", "#", m_refMsg.GetMessage("alt Click here to go to the previous step"), m_refMsg.GetMessage("alt Go to the Previous Step"), "onclick=\"WizardUtil.showPreviousStep()\" ", StyleHelper.BackButtonCssClass, true));

		if (!(this.m_sPageAction == "view"))
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/back.png", (string)("ruleset.aspx?action=View&id=" + m_iID.ToString()), "Click here to go back", "Go Back", "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/cancel.png", "#", m_refMsg.GetMessage("alt Click to close this window"), m_refMsg.GetMessage("alt Close this window"), " onclick=\"self.close();\" ", StyleHelper.SaveButtonCssClass, true));
		}

		result.Append(m_refStyle.GetButtonEventsWCaption(m_refContentApi.AppPath + "images/UI/Icons/forward.png", "#", m_refMsg.GetMessage("alt Click here to go to the next step"), m_refMsg.GetMessage("alt Go to the Next Step"), "onclick=\"WizardUtil.showNextStep()\" ", StyleHelper.NextButtonCssClass, true));
		
		result.Append(StyleHelper.ActionBarDivider);
		
		if (!(this.m_sPageAction == "view"))
        {
            result.Append("<td>");
            result.Append(m_refStyle.GetHelpButton("Ruleset", ""));
            result.Append("</td>");
        }
        else
        {
			result.Append("<td>");
            result.Append(m_refStyle.GetHelpButton("Ruleset_view_wizard", ""));
            result.Append("</td>");
        }
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }
    private void OutputJS()
    {
        StringBuilder sbJS = new StringBuilder();

        Page.ClientScript.RegisterClientScriptInclude("statement", "includes/com.ektron.ui.statement.js");
        Page.ClientScript.RegisterClientScriptInclude("manager", "includes/com.ektron.sc.manager.js");
        Page.ClientScript.RegisterClientScriptInclude("rules", "includes/com.ektron.sc.rules.js");
        Page.ClientScript.RegisterClientScriptInclude("wizard", "includes/com.ektron.utils.wizard.js");

        sbJS.Append("<link rel=\'stylesheet\' type=\'text/css\' href=\'css/com.ektron.utils.wizard.css\' />" + Environment.NewLine);
        sbJS.Append("<link rel=\'stylesheet\' type=\'text/css\' href=\'css/com.ektron.rules.wizard.css\' />" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.net.http.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.cookie.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.debug.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.dom.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.form.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.log.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.querystring.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.string.js\"></script>" + Environment.NewLine);
        sbJS.Append("<script type=\"text/javascript\" language=\"javascript\" src=\"../tree/js/com.ektron.utils.xml.js\"></script>" + Environment.NewLine);
        // sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.ui.statement.js""></script>" & Environment.NewLine)
        //sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.sc.manager.js""></script>" & Environment.NewLine)
        //sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.sc.rules.js""></script>" & Environment.NewLine)
        //sbJS.Append("<script type=""text/javascript"" language=""javascript"" src=""includes/com.ektron.utils.wizard.js""></script>" & Environment.NewLine)

        ltrjs.Text = sbJS.ToString();
        sbJS = null;
    }
    private void BuildTemplateJS(string action)
    {
        StringBuilder sbTemplateJS = new StringBuilder();
        sbTemplateJS.Append("<script type=\"text/javascript\" language=\"javascript\">" + Environment.NewLine);
        if (action == "view")
        {
            sbTemplateJS.Append(" s_action = \'view\'; " + Environment.NewLine);
            sbTemplateJS.Append(" document.getElementById(\'ruleNameText\').disabled = true; " + Environment.NewLine);
            sbTemplateJS.Append(" document.getElementById(\'logicalOperator\').disabled = true; " + Environment.NewLine);
            step2.Text = "<span class=\"stepLabel\">2. Actions to take when conditions are TRUE.</span>";
            step3.Text = "<span class=\"stepLabel\">3. Actions to take when conditions are FALSE.</span>";
            step4.Text = "<span class=\"stepLabel\">4. Rule Name.</span>";
        }
        else
        {
            sbTemplateJS.Append(" s_action = \'\'; " + Environment.NewLine);
        }
        GenerateWizardJS(action);
        sbTemplateJS.Append(BuildConditionJS());
        sbTemplateJS.Append(BuildActiontrueJS());
        sbTemplateJS.Append(BuildActionfalseJS());
        sbTemplateJS.Append(BuildConditionParamJS(action));
        sbTemplateJS.Append(BuildActionParamtrueJS(action));
        sbTemplateJS.Append(BuildActionParamfalseJS(action));
        sbTemplateJS.Append("function CheckRuleNameForillegalChar() {" + Environment.NewLine);
        sbTemplateJS.Append("   var val = document.getElementById(\'ruleNameText\').value;" + Environment.NewLine);
        sbTemplateJS.Append("   if ((val.indexOf(\"\\\\\") >= 0) || (val.indexOf(\"/\") >= 0) || (val.indexOf(\":\") >= 0)||(val.indexOf(\"*\") >= 0) || (val.indexOf(\"?\") >= 0)|| (val.indexOf(\"\\\"\") >= 0) || (val.indexOf(\"<\") >= 0)|| (val.indexOf(\">\") >= 0) || (val.indexOf(\"|\") >= 0) || (val.indexOf(\"&\") >= 0) || (val.indexOf(\"\\\'\") >= 0))" + Environment.NewLine);
        sbTemplateJS.Append("   {" + Environment.NewLine);
        sbTemplateJS.Append("       alert(\"" + m_refMsg.GetMessage("msg rule name cannot") + " " + "(\'\\\\\', \'/\', \':\', \'*\', \'?\', \' \\\" \', \'<\', \'>\', \'|\', \'&\', \'\\\'\').\");" + Environment.NewLine);
        sbTemplateJS.Append("       return false;" + Environment.NewLine);
        sbTemplateJS.Append("   }" + Environment.NewLine);
        sbTemplateJS.Append("   return true;" + Environment.NewLine);
        sbTemplateJS.Append("}" + Environment.NewLine);

        sbTemplateJS.Append("</script>" + Environment.NewLine);
        templateJS.Text = sbTemplateJS.ToString();
        sbTemplateJS = null;
    }

    private string BuildConditionJS()
    {
        StringBuilder sbConditionJS = new StringBuilder();
        string sTemplate = "";
        string sPredicate = "";
        int count = 0;
        sbConditionJS.Append("            var condition_templates =" + Environment.NewLine);
        sbConditionJS.Append("            [" + Environment.NewLine);
        if (m_acontem.Length > 0)
        {
            for (int i = 0; i <= (m_acontem.Length - 1); i++)
            {
                sbConditionJS.Append("	            {" + Environment.NewLine);
                sbConditionJS.Append("		            id: " + count.ToString() + "," + Environment.NewLine);
                sbConditionJS.Append("		            templateid: " + m_acontem[i].ID.ToString() + "," + Environment.NewLine);
                sbConditionJS.Append("		            name: \"" + m_acontem[i].Name.Replace("\"", "\\\"") + "\"," + Environment.NewLine);
                sTemplate = (string)(m_acontem[i].TemplateString.Replace("\"", "\\\""));
                sTemplate = sTemplate.Replace("rulewizardmanager", "RuleWizardManager");
                sTemplate = sTemplate.Replace("showinputform", "showInputForm");
                sTemplate = sTemplate.Replace("{:_:}icount{:_:}", "{:_:}" + count.ToString() + "{:_:}");
                sTemplate = sTemplate.Replace("datainputtext", "dataInputText");
                sbConditionJS.Append("		            template: \"" + sTemplate + "\"," + Environment.NewLine);
                sPredicate = (string)(m_acontem[i].Predicate.Replace("\"", "\\\""));
                sPredicate = sPredicate.Replace("{:_:}icount{:_:}", "{:_:}" + count.ToString() + "{:_:}");
                sbConditionJS.Append("		            predicate: \"" + sPredicate + "\"," + Environment.NewLine);
                if (TemplateExists(m_acontem[i].ID, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.Condition))
                {
                    sbConditionJS.Append("		            active: true" + Environment.NewLine);
                }
                else
                {
                    sbConditionJS.Append("		            active: false" + Environment.NewLine);
                }
                sbConditionJS.Append("	            }");
                if (i == (m_acontem.Length - 1))
                {
                    sbConditionJS.Append(Environment.NewLine);
                }
                else
                {
                    sbConditionJS.Append("," + Environment.NewLine);
                }
                count++;
            }
        }
        sbConditionJS.Append("            ]" + Environment.NewLine);
        return sbConditionJS.ToString();
    }
    private string BuildActiontrueJS()
    {
        string sTemplate = "";
        int count = 0;
        StringBuilder sbActionJS = new StringBuilder();
        sbActionJS.Append("            var actiontrue_templates = " + Environment.NewLine);
        sbActionJS.Append("            [" + Environment.NewLine);
        if (m_aacttem.Length > 0)
        {
            for (int i = 0; i <= (m_aacttem.Length - 1); i++)
            {
                sbActionJS.Append("	            {" + Environment.NewLine);
                sbActionJS.Append("		            id: " + count.ToString() + "," + Environment.NewLine);
                sbActionJS.Append("		            templateid: " + m_aacttem[i].ID.ToString() + "," + Environment.NewLine);
                sbActionJS.Append("		            name: \"" + m_aacttem[i].Name.Replace("\"", "\\\"") + "\"," + Environment.NewLine);
                sTemplate = (string)(m_aacttem[i].TemplateString.Replace("\"", "\\\""));
                sTemplate = sTemplate.Replace("action{:_:}", "actiontrue{:_:}");
                sTemplate = sTemplate.Replace("{:_:}icount{:_:}", "{:_:}" + count.ToString() + "{:_:}");
                sbActionJS.Append("		            template: \"" + sTemplate + "\"," + Environment.NewLine);
                if (TemplateExists(m_aacttem[i].ID, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionTrue))
                {
                    sbActionJS.Append("		            active: true" + Environment.NewLine);
                }
                else
                {
                    sbActionJS.Append("		            active: false" + Environment.NewLine);
                }
                sbActionJS.Append("	            }");
                if (i == (m_aacttem.Length - 1))
                {
                    sbActionJS.Append(Environment.NewLine);
                }
                else
                {
                    sbActionJS.Append("," + Environment.NewLine);
                }
                count++;
            }
        }
        sbActionJS.Append("            ]" + Environment.NewLine);
        return sbActionJS.ToString();
    }

    private string BuildActionfalseJS()
    {
        string sTemplate = "";
        int count = 0;
        StringBuilder sbActionJS = new StringBuilder();
        sbActionJS.Append("            var actionfalse_templates = " + Environment.NewLine);
        sbActionJS.Append("            [" + Environment.NewLine);
        if (m_aacttem.Length > 0)
        {
            for (int i = 0; i <= (m_aacttem.Length - 1); i++)
            {
                sbActionJS.Append("	            {" + Environment.NewLine);
                sbActionJS.Append("		            id: " + count.ToString() + "," + Environment.NewLine);
                sbActionJS.Append("		            templateid: " + m_aacttem[i].ID.ToString() + "," + Environment.NewLine);
                sbActionJS.Append("		            name: \"" + m_aacttem[i].Name.Replace("\"", "\\\"") + "\"," + Environment.NewLine);
                sTemplate = (string)(m_aacttem[i].TemplateString.Replace("\"", "\\\""));
                sTemplate = sTemplate.Replace("action{:_:}", "actionfalse{:_:}");
                sTemplate = sTemplate.Replace("{:_:}icount{:_:}", "{:_:}" + count.ToString() + "{:_:}");
                sbActionJS.Append("		            template: \"" + sTemplate + "\"," + Environment.NewLine);
                if (TemplateExists(m_aacttem[i].ID, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionFalse))
                {
                    sbActionJS.Append("		            active: true" + Environment.NewLine);
                }
                else
                {
                    sbActionJS.Append("		            active: false" + Environment.NewLine);
                }
                sbActionJS.Append("	            }");
                if (i == (m_aacttem.Length - 1))
                {
                    sbActionJS.Append(Environment.NewLine);
                }
                else
                {
                    sbActionJS.Append("," + Environment.NewLine);
                }
                count++;
            }
        }
        sbActionJS.Append("            ]" + Environment.NewLine);
        return sbActionJS.ToString();
    }

    public string BuildConditionParamJS(string type)
    {
        StringBuilder sbConditionParamJS = new StringBuilder();
        int icount = 0;
        string sTmpValue = "";

        sbConditionParamJS.Append("	       var condition_template_params =" + Environment.NewLine);
        sbConditionParamJS.Append("            [" + Environment.NewLine);
        if (m_acontem.Length > 0 && m_tpacondition.Length > 0)
        {
            for (int i = 0; i <= (m_acontem.Length - 1); i++)
            {
                sbConditionParamJS.Append("                {" + Environment.NewLine);
                sbConditionParamJS.Append("                    id : " + icount.ToString() + "," + Environment.NewLine);
                sbConditionParamJS.Append("                    params:" + Environment.NewLine);
                sbConditionParamJS.Append("                        [" + Environment.NewLine);
                for (int j = 0; j <= (m_tpacondition.Length - 1); j++)
                {
                    if (m_tpacondition[j].TemplateID == m_acontem[i].ID)
                    {
                        sTmpValue = ObtainValue(m_acontem[i].ID, m_tpacondition[j].ParamName, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.Condition).Replace("&amp;", "&");
                        if (sTmpValue != "")
                        {
                            sbConditionParamJS.Append("                            { \"" + m_tpacondition[j].ParamName + "\": \"" + sTmpValue + "\" }," + Environment.NewLine);
                        }
                        else
                        {
                            sbConditionParamJS.Append("                            { \"" + m_tpacondition[j].ParamName + "\": \"" + m_tpacondition[j].DefaultValue + "\" }," + Environment.NewLine);
                        }
                    }
                }
                sbConditionParamJS.Append("                        ]" + Environment.NewLine);
                sbConditionParamJS.Append("                }");
                if (i == (m_acontem.Length - 1))
                {
                    sbConditionParamJS.Append(Environment.NewLine);
                }
                else
                {
                    sbConditionParamJS.Append("," + Environment.NewLine);
                }
                icount++;
            }
        }
        sbConditionParamJS.Append("            ]" + Environment.NewLine);
        return sbConditionParamJS.ToString();
    }

    public string BuildActionParamtrueJS(string type)
    {
        StringBuilder sbConditionParamJS = new StringBuilder();
        int icount = 0;
        string sTmpValue = "";

        sbConditionParamJS.Append("	       var actiontrue_template_params =" + Environment.NewLine);
        sbConditionParamJS.Append("            [" + Environment.NewLine);
        if (m_acontem.Length > 0 && m_tpaaction.Length > 0)
        {
            for (int i = 0; i <= (m_aacttem.Length - 1); i++)
            {
                sbConditionParamJS.Append("                {" + Environment.NewLine);
                sbConditionParamJS.Append("                    id : " + icount.ToString() + "," + Environment.NewLine);
                sbConditionParamJS.Append("                    params:" + Environment.NewLine);
                sbConditionParamJS.Append("                        [" + Environment.NewLine);
                for (int j = 0; j <= (m_tpaaction.Length - 1); j++)
                {
                    if (m_tpaaction[j].TemplateID == m_aacttem[i].ID)
                    {
                        sTmpValue = ObtainValue(m_aacttem[i].ID, (string)(m_tpaaction[j].ParamName), Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionTrue).Replace("&amp;", "&");
                        if (sTmpValue != "")
                        {
                            sbConditionParamJS.Append("                            { \"" + m_tpaaction[j].ParamName + "\": \"" + sTmpValue + "\" }," + Environment.NewLine);
                        }
                        else
                        {
                            sbConditionParamJS.Append("                            { \"" + m_tpaaction[j].ParamName + "\": \"" + m_tpaaction[j].DefaultValue + "\" }," + Environment.NewLine);
                        }
                    }
                }
                sbConditionParamJS.Append("                        ]" + Environment.NewLine);
                sbConditionParamJS.Append("                }");
                if (i == (m_aacttem.Length - 1))
                {
                    sbConditionParamJS.Append(Environment.NewLine);
                }
                else
                {
                    sbConditionParamJS.Append("," + Environment.NewLine);
                }
                icount++;
            }
        }
        sbConditionParamJS.Append("            ]" + Environment.NewLine);
        return sbConditionParamJS.ToString();
    }

    public string BuildActionParamfalseJS(string type)
    {
        StringBuilder sbConditionParamJS = new StringBuilder();
        int icount = 0;
        string sTmpValue = "";

        sbConditionParamJS.Append("	       var actionfalse_template_params =" + Environment.NewLine);
        sbConditionParamJS.Append("            [" + Environment.NewLine);
        if (m_acontem.Length > 0 && m_tpaaction.Length > 0)
        {
            for (int i = 0; i <= (m_aacttem.Length - 1); i++)
            {
                sbConditionParamJS.Append("                {" + Environment.NewLine);
                sbConditionParamJS.Append("                    id : " + icount.ToString() + "," + Environment.NewLine);
                sbConditionParamJS.Append("                    params:" + Environment.NewLine);
                sbConditionParamJS.Append("                        [" + Environment.NewLine);
                for (int j = 0; j <= (m_tpaaction.Length - 1); j++)
                {
                    if (m_tpaaction[j].TemplateID == m_aacttem[i].ID)
                    {
                        sTmpValue = ObtainValue(m_aacttem[i].ID, m_tpaaction[j].ParamName, Ektron.Cms.Common.EkEnumeration.RuleTemplateType.ActionFalse).Replace("&amp;", "&");
                        if (sTmpValue != "")
                        {
                            sbConditionParamJS.Append("                            { \"" + m_tpaaction[j].ParamName + "\": \"" + sTmpValue + "\" }," + Environment.NewLine);
                        }
                        else
                        {
                            sbConditionParamJS.Append("                            { \"" + m_tpaaction[j].ParamName + "\": \"" + m_tpaaction[j].DefaultValue + "\" }," + Environment.NewLine);
                        }
                    }
                }
                sbConditionParamJS.Append("                        ]" + Environment.NewLine);
                sbConditionParamJS.Append("                }");
                if (i == (m_aacttem.Length - 1))
                {
                    sbConditionParamJS.Append(Environment.NewLine);
                }
                else
                {
                    sbConditionParamJS.Append("," + Environment.NewLine);
                }
                icount++;
            }
        }
        sbConditionParamJS.Append("            ]" + Environment.NewLine);
        return sbConditionParamJS.ToString();
    }

    private int FindTemplateIndex(Template[] TemplateArray, long iTemplateID)
    {
        int iret = 0;
        for (int i = 0; i <= (TemplateArray.Length - 1); i++)
        {
            if (iTemplateID == TemplateArray[i].ID)
            {
                iret = i;
                break;
            }
        }
        return iret;
    }
    private bool TemplateExists(long templateid, Ektron.Cms.Common.EkEnumeration.RuleTemplateType type)
    {
        bool bRet = false;
        if (m_aruleparam == null)
        {
            //return false
        }
        else
        {
            for (int i = 0; i <= (m_aruleparam.Length - 1); i++)
            {
                if ((m_aruleparam[i].TemplateID == templateid) && (m_aruleparam[i].TemplateType == type))
                {
                    bRet = true;
                }
            }
        }
        return bRet;
    }
    private string ObtainValue(long TemplateID, string ParamName, Ektron.Cms.Common.EkEnumeration.RuleTemplateType type)
    {
        string sRet = "";
        ParamName = (string)(ParamName.ToLower().Trim());
        if (m_aruleparam == null)
        {
            //return empty
        }
        else
        {
            for (int i = 0; i <= (m_aruleparam.Length - 1); i++)
            {
                if ((m_aruleparam[i].TemplateID == TemplateID) && (m_aruleparam[i].TemplateType == type) && (m_aruleparam[i].ParamName.ToLower().Trim() == ParamName))
                {
                    sRet = m_aruleparam[i].DefaultValue;
                }
            }
        }
        return sRet;
    }

    private void GenerateWizardJS(string type)
    {
        StringBuilder sbWizard = new StringBuilder();
        sbWizard.Append("       <script type=\"text/javascript\" language=\"javascript\">").Append(Environment.NewLine);
        sbWizard.Append("").Append(Environment.NewLine);
        sbWizard.Append("            StatementWizard.displayScreen();   ").Append(Environment.NewLine);
        sbWizard.Append("").Append(Environment.NewLine);
        if (!(type == "view"))
        {
            sbWizard.Append("var oProgressSteps = new ProgressSteps;").Append(Environment.NewLine);
            sbWizard.Append("").Append(Environment.NewLine);
            sbWizard.Append("oProgressSteps.maxSteps = 4;").Append(Environment.NewLine);
            sbWizard.Append("").Append(Environment.NewLine);
            sbWizard.Append("oProgressSteps.define([").Append(Environment.NewLine);
            sbWizard.Append("      { id:\"condition\",	    title:\"" + m_refMsg.GetMessage("lbl set conditions") + "\",			    description:\"" + m_refMsg.GetMessage("msg conditions match") + "\" }").Append(Environment.NewLine);
            sbWizard.Append("    , { id:\"actiontrue\",	title:\"" + m_refMsg.GetMessage("lbl set actions for true") + "\",		description:\"<font color=\\\"green\\\">" + m_refMsg.GetMessage("msg action true") + "</font>\" }").Append(Environment.NewLine);
            sbWizard.Append("    , { id:\"actionfalse\",	title:\"" + m_refMsg.GetMessage("lbl set actions for false") + "\",		description:\"<font color=\\\"red\\\">" + m_refMsg.GetMessage("msg action false") + "</font>\" }").Append(Environment.NewLine);
            sbWizard.Append("    , { id:\"rulename\",              title:\"" + m_refMsg.GetMessage("lbl assign name") + "\",		        description:\"" + m_refMsg.GetMessage("msg name meaningful") + "\" }").Append(Environment.NewLine);
            sbWizard.Append("    ]);").Append(Environment.NewLine);
            sbWizard.Append("").Append(Environment.NewLine);
            sbWizard.Append("if (\"object\" == typeof oProgressSteps && oProgressSteps != null)").Append(Environment.NewLine);
            sbWizard.Append("{").Append(Environment.NewLine);
            sbWizard.Append("	oProgressSteps.onselect = function(stepNumber)").Append(Environment.NewLine);
            sbWizard.Append("	{").Append(Environment.NewLine);
            sbWizard.Append("		switch (this.getStep(stepNumber).id)").Append(Environment.NewLine);
            sbWizard.Append("		{").Append(Environment.NewLine);
            sbWizard.Append("		case \"condition\":").Append(Environment.NewLine);
            sbWizard.Append("		    WizardUtil.setStep(1);").Append(Environment.NewLine);
            sbWizard.Append("            WizardUtil.showStep(1);").Append(Environment.NewLine);
            sbWizard.Append("			break;").Append(Environment.NewLine);
            sbWizard.Append("		case \"actiontrue\":").Append(Environment.NewLine);
            sbWizard.Append("			WizardUtil.setStep(2);").Append(Environment.NewLine);
            sbWizard.Append("            WizardUtil.showStep(2);").Append(Environment.NewLine);
            sbWizard.Append("			break;").Append(Environment.NewLine);
            sbWizard.Append("		case \"actionfalse\":").Append(Environment.NewLine);
            sbWizard.Append("		    WizardUtil.setStep(3);").Append(Environment.NewLine);
            sbWizard.Append("            WizardUtil.showStep(3);").Append(Environment.NewLine);
            sbWizard.Append("			break;").Append(Environment.NewLine);
            sbWizard.Append("		case \"rulename\":").Append(Environment.NewLine);
            sbWizard.Append("		    WizardUtil.setStep(4);").Append(Environment.NewLine);
            sbWizard.Append("            WizardUtil.showStep(4);").Append(Environment.NewLine);
            sbWizard.Append("			break;").Append(Environment.NewLine);
            sbWizard.Append("		default:").Append(Environment.NewLine);
            sbWizard.Append("			break;").Append(Environment.NewLine);
            sbWizard.Append("		}").Append(Environment.NewLine);
            sbWizard.Append("	}").Append(Environment.NewLine);
            sbWizard.Append("	oProgressSteps.oncancel = function()").Append(Environment.NewLine);
            sbWizard.Append("	{").Append(Environment.NewLine);
            sbWizard.Append("		var objAElem = document.getElementById(\"image_link_101\");").Append(Environment.NewLine);
            sbWizard.Append("		if (objAElem)").Append(Environment.NewLine);
            sbWizard.Append("		{").Append(Environment.NewLine);
            sbWizard.Append("			location.href = objAElem.href;").Append(Environment.NewLine);
            sbWizard.Append("		}").Append(Environment.NewLine);
            sbWizard.Append("	}").Append(Environment.NewLine);
            sbWizard.Append("}").Append(Environment.NewLine);
        }
        sbWizard.Append("</script>").Append(Environment.NewLine);
        ltr_wizardjs.Text = sbWizard.ToString();
    }
    private void RegisterResources()
    {
        // register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJFunctJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronToolBarRollJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronStyleHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);

        // register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
    }
}
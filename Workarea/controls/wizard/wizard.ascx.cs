using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
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
using Ektron.Cms.Workarea;
using Ektron.Cms.Common;

public partial class cmswizard : System.Web.UI.UserControl
{
    #region Private Members
    private bool m_bAllowSelect = false;
    protected EkMessageHelper m_msgRef;
    #endregion

    #region Public Properties
    public bool AllowSelect
    {
        get
        {
            return this.m_bAllowSelect;
        }
        set
        {
            this.m_bAllowSelect = value;
        }
    }
    #endregion
    protected long m_iID = 0;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        m_msgRef = (new Ektron.Cms.CommonApi()).EkMsgRef;
        StyleHelper objStyle = new StyleHelper();
        HelpButton1.Text = objStyle.GetHelpButton("RuleWizardStep1", "");
        HelpButton2.Text = objStyle.GetHelpButton("RuleWizardStep2", "");
        HelpButton3.Text = objStyle.GetHelpButton("RuleWizardStep3", "");
        HelpButton4.Text = objStyle.GetHelpButton("RuleWizardStep4", "");
        HelpButton5.Text = objStyle.GetHelpButton("RuleWizardStep5", "");
        objStyle = null;

        if (Request.QueryString["id"] != "")
        {
            m_iID = Convert.ToInt64(Request.QueryString["id"]);
        }

        RenderJS();
        RegisterResources();
    }

    #region Private Helper Functions
    private void RenderJS()
    {
        StringBuilder sbJS = new StringBuilder();
        sbJS.Append("<script type=\"text/javascript\" id=\"wizardJS\">" + Environment.NewLine);
        sbJS.Append("<!--" + Environment.NewLine);
        sbJS.Append("function ProgressSteps()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	// member vars" + Environment.NewLine);
        sbJS.Append("	this.maxSteps = 9;" + Environment.NewLine);
        sbJS.Append("	this.currentStepNumber = 0; // 1-based" + Environment.NewLine);
        sbJS.Append("	this.startStepNumber = 0; // 1-based" + Environment.NewLine);
        sbJS.Append("	this.nextStepNumber = 0; // 1-based" + Environment.NewLine);
        sbJS.Append("	this.steps = [];" + Environment.NewLine);
        sbJS.Append("	this.disabled = false;" + Environment.NewLine);
        sbJS.Append("   this.allowselect = " + m_bAllowSelect.ToString().ToLower() + ";	" + Environment.NewLine);
        sbJS.Append("		" + Environment.NewLine);
        sbJS.Append("	// methods" + Environment.NewLine);
        sbJS.Append("	this.define = ProgressSteps_define;" + Environment.NewLine);
        sbJS.Append("	this.select = ProgressSteps_select;" + Environment.NewLine);
        sbJS.Append("	this.back = ProgressSteps_back;" + Environment.NewLine);
        sbJS.Append("	this.next = ProgressSteps_next;" + Environment.NewLine);
        sbJS.Append("	this.done = ProgressSteps_done;" + Environment.NewLine);
        sbJS.Append("	this.cancel = ProgressSteps_cancel;" + Environment.NewLine);
        sbJS.Append("	this.close = ProgressSteps_close;" + Environment.NewLine);
        sbJS.Append("	this.disable = ProgressSteps_disable;" + Environment.NewLine);
        sbJS.Append("	this.getStep = ProgressSteps_getStep;" + Environment.NewLine);
        sbJS.Append("	// events" + Environment.NewLine);
        sbJS.Append("	this.onselect = function(stepNumber) { return true; };" + Environment.NewLine);
        sbJS.Append("	this.ondone = function() { return true; };" + Environment.NewLine);
        sbJS.Append("	this.oncancel = function() { return true; };" + Environment.NewLine);
        sbJS.Append("	// private" + Environment.NewLine);
        sbJS.Append("	this.setStepState = ProgressSteps_setStepState;" + Environment.NewLine);
        sbJS.Append("	this.getEnabledStepNumber = ProgressSteps_getEnabledStepNumber;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_define(steps)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("	this.steps = steps;" + Environment.NewLine);
        sbJS.Append("	var n = this.steps.length;" + Environment.NewLine);
        sbJS.Append("	if (n > this.maxSteps)" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		n = this.maxSteps;" + Environment.NewLine);
        sbJS.Append("		this.steps.length = n;" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("	this.currentStepNumber = 0;" + Environment.NewLine);
        sbJS.Append("	this.startStepNumber = 0;" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"totalSteps\").innerHTML = n;" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("	for (var i = 1; i <= this.maxSteps; i++)" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		var objElem = document.getElementById(\"step\" + i);" + Environment.NewLine);
        sbJS.Append("		if (i <= n)" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			var objStep = this.getStep(i);" + Environment.NewLine);
        sbJS.Append("			objElem.title = objStep.title;" + Environment.NewLine);
        sbJS.Append("			objStep.disabled = (objStep.disabled ? true : false);" + Environment.NewLine);
        sbJS.Append("			objElem.className = (objStep.disabled ? \"stepDisabled\" : \"stepFuture\");" + Environment.NewLine);
        sbJS.Append("			if (!objStep.disabled)" + Environment.NewLine);
        sbJS.Append("			{" + Environment.NewLine);
        sbJS.Append("				if (0 == this.startStepNumber) this.startStepNumber = i; " + Environment.NewLine);
        sbJS.Append("			}" + Environment.NewLine);
        sbJS.Append("			objElem.onmouseover = new Function(\"document.getElementById(\'stepTitle\').innerHTML = this.title\");" + Environment.NewLine);
        sbJS.Append("			objElem.onmouseout = new Function(\"document.getElementById(\'stepTitle\').innerHTML = oProgressSteps.getStep(oProgressSteps.currentStepNumber).title\");" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("		else" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			objElem.title = \"\";" + Environment.NewLine);
        sbJS.Append("			objElem.className = \"stepNotUsed\";" + Environment.NewLine);
        sbJS.Append("			objElem.onmouseover = null; " + Environment.NewLine);
        sbJS.Append("			objElem.onmouseout = null;" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("		objElem.onclick = null;" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("	this.nextStepNumber = this.startStepNumber;" + Environment.NewLine);
        sbJS.Append("	this.select(this.startStepNumber);	" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"stepsContainer\").style.display = \"block\";" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_select(stepNumberOrID)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	// stepNumberOrID as number: 1-based index" + Environment.NewLine);
        sbJS.Append("	// stepNumberOrID as ID: string" + Environment.NewLine);
        sbJS.Append("	if (this.disabled) return this.currentStepNumber;" + Environment.NewLine);
        sbJS.Append("	var cur = 1; " + Environment.NewLine);
        sbJS.Append("	if (\"number\" == typeof stepNumberOrID)" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		cur = stepNumberOrID;" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("	else if (\"string\" == typeof stepNumberOrID)" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		for (var i = 0; i < this.steps.length; i++)" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			if (this.steps[i].id == stepNumberOrID)" + Environment.NewLine);
        sbJS.Append("			{" + Environment.NewLine);
        sbJS.Append("				cur = i + 1; // 1-based index" + Environment.NewLine);
        sbJS.Append("				break;" + Environment.NewLine);
        sbJS.Append("			}" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("    //alert(this.startStepNumber <= cur && cur <= this.nextStepNumber && cur <= this.steps.length);" + Environment.NewLine);
        sbJS.Append("	if (this.startStepNumber <= cur && cur <= this.steps.length && ( (this.allowselect) || (cur <= this.nextStepNumber) )) " + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		cur = this.getEnabledStepNumber(cur);" + Environment.NewLine);
        sbJS.Append("		if (false == this.onselect(cur)) return this.currentStepNumber;" + Environment.NewLine);
        sbJS.Append("		var objStep = this.getStep(cur);" + Environment.NewLine);
        sbJS.Append("		document.getElementById(\"currentStep\").innerHTML = cur;" + Environment.NewLine);
        sbJS.Append("		document.getElementById(\"stepTitle\").innerHTML = objStep.title;" + Environment.NewLine);
        sbJS.Append("		document.getElementById(\"stepDescription\").innerHTML = objStep.description;" + Environment.NewLine);
        sbJS.Append("		var old = this.currentStepNumber;" + Environment.NewLine);
        sbJS.Append("		if (1 <= old)" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			if (old > cur && old + 1 == this.nextStepNumber)" + Environment.NewLine);
        sbJS.Append("			{" + Environment.NewLine);
        sbJS.Append("				this.setStepState(old, \"stepNext\");" + Environment.NewLine);
        sbJS.Append("			}" + Environment.NewLine);
        sbJS.Append("			else" + Environment.NewLine);
        sbJS.Append("			{" + Environment.NewLine);
        sbJS.Append("			    if (this.allowselect)" + Environment.NewLine);
        sbJS.Append("			    {" + Environment.NewLine);
        sbJS.Append("			        for (var i = old; i <= cur; i++)" + Environment.NewLine);
        sbJS.Append("		            {" + Environment.NewLine);
        sbJS.Append("		                this.setStepState(i, \"stepVisited\");" + Environment.NewLine);
        sbJS.Append("		            }" + Environment.NewLine);
        sbJS.Append("		        }" + Environment.NewLine);
        sbJS.Append("		        else" + Environment.NewLine);
        sbJS.Append("		        {" + Environment.NewLine);
        sbJS.Append("		            this.setStepState(old, \"stepVisited\");   " + Environment.NewLine);
        sbJS.Append("		        }" + Environment.NewLine);
        sbJS.Append("			}" + Environment.NewLine);
        sbJS.Append("			if (old < cur - 1)" + Environment.NewLine);
        sbJS.Append("			{" + Environment.NewLine);
        sbJS.Append("				this.setStepState(cur - 1, \"stepVisited\");" + Environment.NewLine);
        sbJS.Append("			}" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("		if (cur == this.nextStepNumber)" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			var next = this.getEnabledStepNumber(cur + 1);" + Environment.NewLine);
        sbJS.Append("			if (this.allowselect)" + Environment.NewLine);
        sbJS.Append("			{" + Environment.NewLine);
        sbJS.Append("		        for (var i = next; i <= this.steps.length; i++)" + Environment.NewLine);
        sbJS.Append("		        {" + Environment.NewLine);
        sbJS.Append("		            this.setStepState(i, \"stepNext\");" + Environment.NewLine);
        sbJS.Append("		        }" + Environment.NewLine);
        sbJS.Append("		    }" + Environment.NewLine);
        sbJS.Append("		    else" + Environment.NewLine);
        sbJS.Append("		    {" + Environment.NewLine);
        sbJS.Append("		        this.setStepState(next, \"stepNext\");" + Environment.NewLine);
        sbJS.Append("		    }" + Environment.NewLine);
        sbJS.Append("			this.nextStepNumber = next;" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("		this.setStepState(cur, \"stepCurrent\");" + Environment.NewLine);
        sbJS.Append("		" + Environment.NewLine);
        sbJS.Append("		var objElem;" + Environment.NewLine);
        sbJS.Append("		objElem = document.getElementById(\"btnBackStep\");" + Environment.NewLine);
        sbJS.Append("		var bDisabled = (cur <= this.startStepNumber)" + Environment.NewLine);
        sbJS.Append("		objElem.disabled = (bDisabled);" + Environment.NewLine);
        sbJS.Append("		objElem.title = (bDisabled ? \"" + m_msgRef.GetMessage("lbl cannot go back") + "\" : \"" + m_msgRef.GetMessage("btn back") + ":" + " " + "\" + this.getStep(this.getEnabledStepNumber(cur - 1)).title);" + Environment.NewLine);
        sbJS.Append("		" + Environment.NewLine);
        sbJS.Append("		var bAtEnd = (cur == this.steps.length);" + Environment.NewLine);
        sbJS.Append("		objElem = document.getElementById(\"btnNextStep\");" + Environment.NewLine);
        sbJS.Append("		objElem.style.display = (bAtEnd ? \"none\" : \"\");" + Environment.NewLine);
        sbJS.Append("		objElem.title = (bAtEnd ? \"" + m_msgRef.GetMessage("txt linkcheck idle") + "\" : \"" + m_msgRef.GetMessage("next") + ":" + " " + "\" + this.getStep(this.getEnabledStepNumber(cur + 1)).title);" + Environment.NewLine);
        sbJS.Append("		document.getElementById(\"btnDoneSteps\").style.display = (bAtEnd ? \"\" : \"none\");" + Environment.NewLine);
        sbJS.Append("		this.currentStepNumber = cur;" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("	SelectHelpButton(cur);" + Environment.NewLine);
        sbJS.Append("	return this.currentStepNumber;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("// private" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_setStepState(stepNumber, stepState)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	var n = stepNumber;" + Environment.NewLine);
        sbJS.Append("	if (1 <= n && n <= this.steps.length && !this.getStep(n).disabled)" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		var objElem = document.getElementById(\"step\" + n);" + Environment.NewLine);
        sbJS.Append("		objElem.className = stepState;" + Environment.NewLine);
        sbJS.Append("		if (\"stepVisited\" == stepState || \"stepNext\" == stepState || \"stepFuture\" == stepState)" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			objElem.onclick = new Function(\"oProgressSteps.select(\" + n + \")\");" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("		else" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			objElem.onclick = null;" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_back()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    var wizstep = WizardUtil.getCurrentStep();" + Environment.NewLine);
        sbJS.Append("	if (this.disabled) return;" + Environment.NewLine);
        sbJS.Append("	var cur = this.currentStepNumber - 1;" + Environment.NewLine);
        sbJS.Append("	while (cur >= 1 && true == this.getStep(cur).disabled)" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		cur--;" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("	this.select(cur);" + Environment.NewLine);
        sbJS.Append("	wizstep = wizstep - 1;" + Environment.NewLine);
        sbJS.Append("	WizardUtil.setStep(wizstep);" + Environment.NewLine);
        sbJS.Append("    WizardUtil.showStep(wizstep);" + Environment.NewLine);
        sbJS.Append("	return;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_next()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("    var wizstep = WizardUtil.getCurrentStep();" + Environment.NewLine);
        sbJS.Append("	if (this.disabled) return;" + Environment.NewLine);
        sbJS.Append("	this.select(this.getEnabledStepNumber(this.currentStepNumber + 1));" + Environment.NewLine);
        sbJS.Append("    wizstep = wizstep + 1;" + Environment.NewLine);
        sbJS.Append("	WizardUtil.setStep(wizstep);" + Environment.NewLine);
        sbJS.Append("    WizardUtil.showStep(wizstep);" + Environment.NewLine);
        sbJS.Append("	return;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);

        sbJS.Append(AJAXcheck(GetResponseString(), "action=existingrule&rid=" + m_iID.ToString() + "&rname=\' + input + \'")).Append(Environment.NewLine);

        sbJS.Append("function ProgressSteps_done()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	if (this.disabled) return;" + Environment.NewLine);
        sbJS.Append("	if (false == this.ondone()) return;" + Environment.NewLine);
        sbJS.Append("	var rn = document.getElementById(\'ruleNameText\');" + Environment.NewLine);
        sbJS.Append("   checkRule(rn.value,\'\'); " + Environment.NewLine);
        sbJS.Append("	return;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);

        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_cancel()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	if (this.disabled) return;" + Environment.NewLine);
        sbJS.Append("	//if (false == this.oncancel()) return;" + Environment.NewLine);
        sbJS.Append("	window.location.href = \"ruleset.aspx?action=View&id=\" + document.getElementById(\'rulesetid\').value;" + Environment.NewLine);
        sbJS.Append("	return;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_close()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"stepsContainer\").style.display = \"none\";" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_disable()" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	this.disabled = true;" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"stepsGraphicsTable\").disabled = true;" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"btnBackStep\").disabled = true;" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"btnNextStep\").disabled = true;" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"btnDoneSteps\").disabled = true;" + Environment.NewLine);
        sbJS.Append("	document.getElementById(\"btnCancelSteps\").disabled = true;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_getStep(stepNumber)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	return this.steps[stepNumber - 1];" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("// private" + Environment.NewLine);
        sbJS.Append("function ProgressSteps_getEnabledStepNumber(stepNumber)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	var cur = stepNumber;" + Environment.NewLine);
        sbJS.Append("	while (cur <= this.steps.length && true == this.getStep(cur).disabled)" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		cur++;" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("	return cur;" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("function SelectHelpButton(cur)" + Environment.NewLine);
        sbJS.Append("{" + Environment.NewLine);
        sbJS.Append("	if ((cur >= 1) && (cur <= 5))	" + Environment.NewLine);
        sbJS.Append("	{" + Environment.NewLine);
        sbJS.Append("		var targId;" + Environment.NewLine);
        sbJS.Append("		var targObj;" + Environment.NewLine);
        sbJS.Append("		var idx;" + Environment.NewLine);
        sbJS.Append("		for (idx = 1; idx <= 5; idx ++)" + Environment.NewLine);
        sbJS.Append("		{" + Environment.NewLine);
        sbJS.Append("			targId = \"helpBtn\" + idx;" + Environment.NewLine);
        sbJS.Append("			targObj = document.getElementById(targId);" + Environment.NewLine);
        sbJS.Append("			targObj.style.display = ((cur == idx) ? \"\" : \"none\");" + Environment.NewLine);
        sbJS.Append("		}" + Environment.NewLine);
        sbJS.Append("	}" + Environment.NewLine);
        sbJS.Append("}" + Environment.NewLine);
        sbJS.Append("" + Environment.NewLine);
        sbJS.Append("var oProgressSteps = new ProgressSteps;" + Environment.NewLine);
        sbJS.Append("// -->" + Environment.NewLine);
        sbJS.Append("</script>" + Environment.NewLine);
        ltr_wizard_js.Text = sbJS.ToString();
    }

    private string AJAXcheck(string sResponse, string sURLQuery)
    {
        Ektron.Cms.CommonApi m_refcommonApi = new Ektron.Cms.CommonApi();
        workareaajax waAjax = new workareaajax(m_refcommonApi.AppPath);
        waAjax.ResponseJS = sResponse;
        waAjax.URLQuery = sURLQuery;
        waAjax.FunctionName = "checkRule";
        return waAjax.Render();
    }

    private string GetResponseString()
    {
        System.Text.StringBuilder sbAEJS = new System.Text.StringBuilder();
        sbAEJS.Append("    if (response == \'1\'){").Append(Environment.NewLine);
        sbAEJS.Append("	        alert(\'" + m_msgRef.GetMessage("err exist name already") + "\');").Append(Environment.NewLine);
        sbAEJS.Append("	        bexists = false;").Append(Environment.NewLine);
        sbAEJS.Append("    }else{").Append(Environment.NewLine);
        sbAEJS.Append("	        var rn = document.getElementById(\'ruleNameText\');" + Environment.NewLine);
        sbAEJS.Append("	        var invalid = CheckRuleNameForillegalChar();" + Environment.NewLine);
        sbAEJS.Append("	        if (!invalid)" + Environment.NewLine);
        sbAEJS.Append("	        {" + Environment.NewLine);
        sbAEJS.Append("	            // nothing" + Environment.NewLine);
        sbAEJS.Append("	        }" + Environment.NewLine);
        sbAEJS.Append("	        else if ((Trim(rn.value).length > 0))" + Environment.NewLine);
        sbAEJS.Append("	        {" + Environment.NewLine);
        sbAEJS.Append("	         RuleWizardManager.submitWizardForm();" + Environment.NewLine);
        sbAEJS.Append("	        }" + Environment.NewLine);
        sbAEJS.Append("	        else" + Environment.NewLine);
        sbAEJS.Append("	        {" + Environment.NewLine);
        sbAEJS.Append("	          alert(\'" + m_msgRef.GetMessage("msg enter name") + "\');" + Environment.NewLine);
        sbAEJS.Append("	        }" + Environment.NewLine);
        //sbAEJS.Append("	        bexists = ").Append(nextfunction).Append("();").Append(Environment.NewLine)
        sbAEJS.Append("    } ").Append(Environment.NewLine);
        return sbAEJS.ToString();
    }

    #endregion
    protected void RegisterResources()
    {
        // register JS
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);

        // register CSS
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
    }
}

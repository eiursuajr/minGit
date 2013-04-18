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

public partial class validation : System.Web.UI.Page
{
    #region  Web Form Designer Generated Code

    protected ContentAPI m_refContentApi = new ContentAPI();
    protected EkMessageHelper m_refMsg;
    protected long ValidationId = 0;
    protected string ValidationType = "";
    protected string MinVal = "";
    protected string MaxVal = "";
    protected string ValidationMsg = "";
    protected string editorName = "";
    protected long ContentId = 0;
    protected string Action = "";
    protected string FieldText = "";
    protected Collection objRuleData;
    protected FormValidationData rule_data;
    protected string arMin = "";
    protected string arMax = "";
    protected string strOptions = "";

    #endregion

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);

        //Put user code to initialize the page here
        try
        {
            if (!string.IsNullOrEmpty(Request["validation_id"]))
                ValidationId = Convert.ToInt64(Strings.Trim(Request["validation_id"]));
            ValidationType = Request["validation_type"];
            MinVal = Request["txtMin"];
            MaxVal = Request["txtMax"];
            ValidationMsg = Request["txtErrorMsg"];
            editorName = Request["editorName"];
            if (!string.IsNullOrEmpty(Request["content_id"]))
                ContentId = Convert.ToInt64(Request["content_id"]);
            Action = Request["action"].ToLower();
            FieldText = Request["field_text"];
            if (!(Page.IsPostBack))
            {
                Display_Validation();
            }
            else
            {
                Process_Validation();
            }
        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }
    }

    private void Display_Validation()
    {
        if (ValidationId > 0)
        {
            rule_data = m_refContentApi.GetRuleToEdit(0, ValidationId);
            if (!(rule_data == null))
            {
                ValidationId = rule_data.Id;
                ValidationType = rule_data.Type;
                MinVal = rule_data.MinVal;
                MaxVal = rule_data.MaxVal;
                ValidationMsg = rule_data.Message;
            }
        }

        Microsoft.VisualBasic.Collection cEnums;
        string ElementName = string.Empty;
        string strOptions;
        strOptions = "";
        if (FieldText.IndexOf("<select") + 1 > 0 || FieldText.IndexOf("<SELECT") + 1 > 0)
        {
            cEnums = m_refContentApi.GetAllValidationEnum("SELECT", "", true);
        }
        else if (FieldText.IndexOf("<textarea") + 1 > 0 || FieldText.IndexOf("<TEXTAREA") + 1 > 0)
        {
            cEnums = m_refContentApi.GetAllValidationEnum("TEXTAREA", "text", false);
        }
        else
        {
            ElementName = "INPUT";
            if (FieldText.IndexOf("type=\"password") + 1 > 0 || FieldText.IndexOf("type=password") + 1 > 0)
            {
                cEnums = m_refContentApi.GetAllValidationEnum("INPUT", "PASSWORD", false);
            }
            else
            {
                cEnums = m_refContentApi.GetAllValidationEnum("INPUT", "TEXT", false);
            }
        }

        ////foreach (object cEnum in cEnums)
        ////{
        if (cEnums["EnumRange"] != null && Convert.ToInt32(cEnums["EnumRange"]) != 0)
            {
                if (Convert.ToInt32(cEnums["EnumRange"]) == 3)
                {
                    if (arMin != "")
                    {
                        arMin = arMin + ", " + cEnums["EnumID"];
                    }
                    else
                    {
                        arMin = (string)(cEnums["EnumID"]);
                    }
                    if (arMax != "")
                    {
                        arMax = arMax + ", " + cEnums["EnumID"];
                    }
                    else
                    {
                        arMax = (string)(cEnums["EnumID"]);
                    }
                }
                else
                {
                    if (Convert.ToInt32(cEnums["EnumRange"]) == 1)
                    {
                        if (arMin != "")
                        {
                            arMin = arMin + ", " + cEnums["EnumID"];
                        }
                        else
                        {
                            arMin = (string)(cEnums["EnumID"]);
                        }
                    }
                    else
                    {
                        if (arMax != "")
                        {
                            arMax = arMax + ", " + cEnums["EnumID"];
                        }
                        else
                        {
                            arMax = (string)(cEnums["EnumID"]);
                        }
                    }
                }
            }

            strOptions = strOptions + "<option value=\"" + cEnums["EnumID"] + "\"" + isSelected(ValidationType, (string)(cEnums["EnumID"])) + ">" + cEnums["EnumName"] + "</option>" + "\r\n";
       //// }

        vType.Text = "<select name=\"selType\" onchange=\"show_range2(\'" + arMin + "\',\'" + arMax + "\');\">";
        vType.Text += strOptions + "</select>";
    }

    private void Process_Validation()
    {
        objRuleData = new Collection();
        objRuleData.Add(m_refContentApi.UserId, "USER_ID", null, null);
        objRuleData.Add(ContentId, "CONTENT_ID", null, null);
        objRuleData.Add(ValidationId, "VALIDATION_ID", null, null);
        objRuleData.Add(ValidationType, "VALIDATION_TYPE", null, null);
        objRuleData.Add(MinVal, "MIN_VAL", null, null);
        objRuleData.Add(MaxVal, "MAX_VAL", null, null);
        objRuleData.Add(ValidationMsg, "VALIDATION_MSG", null, null);
        if (Action == "new")
        {
            ValidationId = m_refContentApi.AddRule(objRuleData);
            if (FieldText.IndexOf("<textarea") + 1 > 0)
            {
                if (Convert.ToInt32(ValidationType) != 0)
                {
                    FieldText = FieldText.Replace("<textarea", "<textarea ekv=" + '\u0022' + ValidationId + '\u0022' + " class=\"redvalidation\" ");
                }
                else
                {
                    FieldText = FieldText.Replace("<textarea", "<textarea ekv=" + '\u0022' + ValidationId + '\u0022' + " ");
                }
            }
            else if (FieldText.IndexOf("<TEXTAREA") + 1 > 0)
            {
                if (Convert.ToInt32(ValidationType) != 0)
                {
                    FieldText = FieldText.Replace("<TEXTAREA", "<TEXTAREA ekv=" + '\u0022' + ValidationId + '\u0022' + " class=\"redvalidation\" ");
                }
                else
                {
                    FieldText = FieldText.Replace("<TEXTAREA", "<TEXTAREA ekv=" + '\u0022' + ValidationId + '\u0022' + " ");
                }
            }
            else if (FieldText.IndexOf("<select") + 1 > 0)
            {
                if (Convert.ToInt32(ValidationType) != 0)
                {
                    FieldText = FieldText.Replace("<select", "<select ekv=" + '\u0022' + ValidationId + '\u0022' + " class=\"redvalidation\" ");
                }
                else
                {
                    FieldText = FieldText.Replace("<select", "<select ekv=" + '\u0022' + ValidationId + '\u0022' + " ");
                }
            }
            else if (FieldText.IndexOf("<SELECT") + 1 > 0)
            {
                if (Convert.ToInt32(ValidationType) != 0)
                {
                    FieldText = FieldText.Replace("<SELECT", "<SELECT ekv=" + '\u0022' + ValidationId + '\u0022' + " class=\"redvalidation\" ");
                }
                else
                {
                    FieldText = FieldText.Replace("<SELECT", "<SELECT ekv=" + '\u0022' + ValidationId + '\u0022' + " ");
                }
            }
            else if (FieldText.IndexOf("/>") + 1 > 0)
            {
                if (Convert.ToInt32(ValidationType) != 0)
                {
                    FieldText = FieldText.Replace("/>", " ekv=" + '\u0022' + ValidationId + '\u0022' + " class=\"redvalidation\" />");
                }
                else
                {
                    FieldText = FieldText.Replace("/>", " ekv=" + '\u0022' + ValidationId + '\u0022' + " />");
                }
            }
            else
            {
                if (Convert.ToInt32(ValidationType) != 0)
                {
                    FieldText = FieldText.Replace(">", " ekv=" + '\u0022' + ValidationId + '\u0022' + " class=\"redvalidation\" >");
                }
                else
                {
                    FieldText = FieldText.Replace(">", " ekv=" + '\u0022' + ValidationId + '\u0022' + " >");
                }

            }
        }
        else if (Action == "update")
        {
            m_refContentApi.UpdateRule(objRuleData);
            if (Convert.ToInt32(ValidationType) == 0)
            {
                FieldText = FieldText.Replace("class=redvalidation", " ");
                FieldText = FieldText.Replace("class=\"redvalidation\"", " ");
            }
            else if (FieldText.IndexOf("class=\"redvalidation\"") + 1 == 0)
            {
                FieldText = FieldText.Replace("ekv=\"", "class=\"redvalidation\" ekv=\"");
            }
            else if (FieldText.IndexOf("class=redvalidation") + 1 == 0)
            {
                FieldText = FieldText.Replace("ekv=", "class=\"redvalidation\" ekv=");
            }
        }

        CloseWindowScript();
    }

    private string isSelected(string sValue, string sDefault)
    {
        string returnValue;
        if (sValue == sDefault)
        {
            returnValue = "selected";
        }
        else
        {
            returnValue = "";
        }

        return returnValue;
    }

    private bool CloseWindowScript()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append("<script language=javascript>" + "\r\n");
        result.Append("var objInstance = eWebEditProUtil.getOpenerInstance();" + "\r\n");
        result.Append("objInstance.editor.pasteHTML(\'" + FieldText + "\');" + "\r\n");
        result.Append("self.close();" + "\r\n");
        result.Append("</script>");
        CloseWindow.Text = result.ToString();
        return false;
    }
}



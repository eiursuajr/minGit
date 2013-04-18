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
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.User;


public partial class addcustomproperty : System.Web.UI.UserControl
{
    protected CommonApi m_CommAPI = new CommonApi();
    protected EkUser m_UserRef;
    protected string PageAction;
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = string.Empty;
    protected bool m_bIsEdit = false;
    protected long m_iId = 0;
    protected int ContentLanguage = -1;
    protected int EnableMultiLanguage = -1;
    protected UserCustomPropertyData m_ucpdata = null;
    protected bool DisplaySelect = false;
    protected string m_strSelectedValue = "";
    protected int m_intValidationType = -1;
    protected UserCustomPropertyData[] allUCPData;
    protected int titleCounter = 0;
    protected int titleIncrement = 0;

    #region Page Load
    private void Page_Load(System.Object sender, System.EventArgs e)
    {

        string ctrlName = string.Empty;
        this.txtMaxValue.Visible = true;
        this.txtMinValue.Visible = true;
        TR_Message.Visible = true;
        TR_Validation.Visible = true;
        TR_inputType.Visible = true;
        m_refMsg = m_CommAPI.EkMsgRef;
        
        ctrlName = GetPostBackControlName();
        RegisterResources();
        if (ctrlName.ToString().ToLower() == ddInputType.UniqueID.ToString().ToLower() || ctrlName.ToString().ToLower() == ddTypes.UniqueID.ToString().ToLower())
        {
            if (IsPostBack)
            {
                if (!(Request.Form[ddTypes.UniqueID] == null))
                {
                    // ReDoForm(Convert.ToInt32(Request.Form[ddTypes.SelectedItem.Value]));
                    ReDoForm(GetObjValType(Request.Form[ddTypes.UniqueID].ToString()));
                }
                else if (!(Request.Form["hdnddTypes"] == null))
                {
                    ReDoForm(GetObjValType(Request.Form["hdnddTypes"].ToString()));
                }
            }
        }
        else
        {
            initPage();
            if (PageAction == "editcustomprop")
            {
                m_bIsEdit = true;
            }
            if ((IsPostBack) || (PageAction == "deletecustomprop"))
            {
                DoProcess();
            }
            else
            {
                SetForm();
            }
        }
    }
    #endregion

    #region Private Helper Functions
    private int GetObjValType(string Type)
    {
        int objType = 0;
        switch (Type)
        {
            case "Category":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.Category;
                break;
            case "ThreadedDiscussion":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.ThreadedDiscussion;
                break;
            case "Date":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.Date;
                break;
            case "Boolean":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.Boolean;
                break;
            case "String":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.String;
                break;
            case "Numeric":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.Numeric;
                break;
            case "MultiSelectList":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.MultiSelectList;
                break;
            case "SelectList":
                objType = (int)EkEnumeration.ObjectPropertyValueTypes.SelectList;
                break;
            default:
                break;
        }
        return objType;
    }
    private void ReDoForm(int Type)
    {
        int iVlidationType = -1;
        switch (Type)
        {
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.Category:
                RestoreFieldValue(1);
                BindInputTypes(1);
                iVlidationType = (Int32)EkEnumeration.ObjectPropertyDisplayTypes.CheckBox;
                break;
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.ThreadedDiscussion:
                RestoreFieldValue(1);
                //ddInputType.Enabled = False
                BindInputTypes(1);
                iVlidationType = (Int32)EkEnumeration.ObjectPropertyDisplayTypes.CheckBox;
                break;
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.Date:
                RestoreFieldValue(1);
                BindInputTypes(3);
                iVlidationType = (Int32)EkEnumeration.ObjectPropertyDisplayTypes.TextBox;
                break;
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.Boolean:
                RestoreFieldValue(1);
                BindInputTypes(2);
                iVlidationType = (Int32)EkEnumeration.ObjectPropertyDisplayTypes.CheckBox;
                break;
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.String:
                RestoreFieldValue(0);
                BindInputTypes(4);
                iVlidationType = (Int32)EkEnumeration.ObjectPropertyDisplayTypes.TextBox;
                break;
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.Numeric:
                RestoreFieldValue(1);
                BindInputTypes(3);
                iVlidationType = (Int32)EkEnumeration.ObjectPropertyDisplayTypes.TextBox;
                break;
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.MultiSelectList:
            case (Int32)EkEnumeration.ObjectPropertyValueTypes.SelectList:
                RestoreFieldValue(8);
                BindInputTypes(8);
                break;
            default:
                RestoreFieldValue(0);
                BindInputTypes(0);
                break;
            //GetAllValidation()
        }
        if (m_ucpdata != null)
        {
            iVlidationType = (Int32)m_ucpdata.PropertyDisplayValueType;
        }
        GetAllValidation(iVlidationType);
        if (Type == (Int32)EkEnumeration.ObjectPropertyValueTypes.Date)
        {
            if (m_ucpdata != null)
            {
                DisplayDateFields((string)m_ucpdata.PropertyValidationMinVal, (string)m_ucpdata.PropertyValidationMaxVal);
            }
            else
            {
                DisplayDateFields("", "");
            }
        }
        if (Type == (Int32)EkEnumeration.ObjectPropertyValueTypes.Boolean)
        {
            ddValidationType.Enabled = false;
        }
    }
    private void BindInputTypes(int Type)
    {
        //Type = 0 - Display all
        //Type = 1 - Display Select, checkboxed
        //Type = 2 - Display checkbox - boolean
        //Type = 3 - Display text box
        //Type = 4 - text box, hidden, textarea
        //Type = 5 - * (can be anything, TBD)
        int i;
        ListItem lsItem;
        System.Array strAR;
        strAR = System.Enum.GetValues(typeof(EkEnumeration.ObjectPropertyDisplayTypes));
        ddInputType.Items.Clear();
        for (i = 0; i <= strAR.Length - 1; i++)
        {
            lsItem = new ListItem();
            lsItem.Text = strAR.GetValue(i).ToString();
            lsItem.Value = strAR.GetValue(i).ToString();
            if (Type == 1)
            {
                if ((strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.CheckBox.ToString()) || (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.DropdownList.ToString()) || (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.MultiSelectList.ToString()) || (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.RadioButton.ToString()))
                {
                    ddInputType.Items.Add(lsItem);
                }
            }
            else if (Type == 2)
            {
                if (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.CheckBox.ToString())
                {
                    ddInputType.Items.Add(lsItem);
                }
            }
            else if (Type == 3)
            {
                if (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.TextBox.ToString())
                {
                    ddInputType.Items.Add(lsItem);
                }
            }
            else if (Type == 4)
            {
                if ((strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.TextBox.ToString()) || (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.TextArea.ToString()) || (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyDisplayTypes.Hidden.ToString()))
                {
                    ddInputType.Items.Add(lsItem);
                }
            }
            else
            {
                ddInputType.Items.Add(lsItem);
            }


        }
        ddInputType.SelectedIndex = 0;
        if (!(Request.Form[ddTypes.UniqueID.ToString()] == null))
        {
            string selVal = "";
            selVal = (string)(Request.Form[ddTypes.UniqueID.ToString()].ToString());
            ddTypes.SelectedIndex = ddTypes.Items.IndexOf(ddTypes.Items.FindByValue(selVal));
            //ddTypes.SelectedValue = viewstate("ddtypes")
        }
    }
    private void BindCMSObjectTypes(int Type)
    {

        int i;
        ListItem lsItem;
        System.Array strAR;
        long CategorydefID = 0;
        bool bAdd = true;
        Ektron.Cms.Content.EkContent contentRef = m_CommAPI.EkContentRef;
        CategorydefID = contentRef.GetCategoryDefinitionID();

        strAR = System.Enum.GetValues(typeof(EkEnumeration.ObjectPropertyValueTypes));
        ddTypes.Items.Clear();
        for (i = 0; i <= strAR.Length - 1; i++)
        {
            bAdd = true;
            if ((!this.m_bIsEdit) && (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyValueTypes.Category.ToString()))
            {
                if (CategorydefID != 0)
                {
                    bAdd = false;
                }
            }
            if ((!this.m_bIsEdit) && (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyValueTypes.CategoryProperties.ToString()))
            {
                bAdd = false;
            }
            if ((!this.m_bIsEdit) && (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyValueTypes.Notification.ToString()))
            {
                bAdd = false;
            }
            if ((!this.m_bIsEdit) && (strAR.GetValue(i).ToString() == EkEnumeration.ObjectPropertyValueTypes.ThreadedDiscussion.ToString()))
            {
                bAdd = false;
            }
            if (bAdd)
            {
                lsItem = new ListItem();
                lsItem.Text = strAR.GetValue(i).ToString();
                lsItem.Value = strAR.GetValue(i).ToString();
                ddTypes.Items.Add(lsItem);
            }
        }
    }
    private void RestoreFieldValue(int Type)
    {
        //Type = 0 - Restore all
        //Type = 1 - Only restore label and required

        if (!(Request.Form[txtLabel.UniqueID] == null))
        {
            txtLabel.Text = Request.Form[txtLabel.UniqueID].ToString();
            txtLabel.ToolTip = txtLabel.Text;
        }

        if (Type == 0)
        {
            if (!(Request.Form[txtMessage.UniqueID] == null))
            {
                txtMessage.Text = Request.Form[txtMessage.UniqueID].ToString();
                txtMessage.ToolTip = txtMessage.Text;
            }
            if (!(Request.Form[txtMinValue.UniqueID] == null))
            {
                this.txtMinValue.Text = Request.Form[txtMinValue.UniqueID].ToString();
                this.txtMinValue.ToolTip = this.txtMinValue.Text;
            }
            if (!(Request.Form[txtMaxValue.UniqueID] == null))
            {
                this.txtMaxValue.Text = Request.Form[txtMaxValue.UniqueID].ToString();
                this.txtMaxValue.ToolTip = this.txtMaxValue.Text;
            }
        }
        else if (Type == 8)
        {
            DisplaySelect = true;
            TR_Message.Visible = false;
            TR_Validation.Visible = false;
            TR_inputType.Visible = false;
        }

        if (!(Request.Form["hdnddTypes"] == null))
        {
            //we have to reregister the hidden field otherwise it will be lost
            Page.ClientScript.RegisterHiddenField("hdnddTypes", Request.Form["hdnddTypes"]);
        }
    }
    private void initPage()
    {
        if (!(Request.QueryString["id"] == null))
        {
            m_iId = Convert.ToInt64(Request.QueryString["id"]);
        }
        if (!(Request.QueryString["action"] == null))
        {
            PageAction = (string)(Request.QueryString["action"].ToString().ToLower());
        }
        m_refMsg = m_CommAPI.EkMsgRef;
        AppImgPath = m_CommAPI.AppImgPath;
        EnableMultiLanguage = m_CommAPI.EnableMultilingual;
        if (Request.QueryString["LangType"] != null)
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            m_CommAPI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (m_CommAPI.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = int.Parse(m_CommAPI.GetCookieValue("LastValidLanguageID"));
            }
        }
        m_CommAPI.ContentLanguage = ContentLanguage;
        m_UserRef = m_CommAPI.EkUserRef;
        m_ucpdata = m_UserRef.GetCustomProperty(m_iId);
        allUCPData = m_UserRef.GetAllCustomProperty("");
    }
    private void View_AddCustomProp_Toolbar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        System.Text.StringBuilder sJS = new System.Text.StringBuilder();

        sJS.Append("<script language=\"Javascript\">" + "\r\n");

        sJS.Append("    function VerifyAddCustomProp() {" + "\r\n");
        sJS.Append("      try{ if(document.getElementById(\"selectedvalues\").value==\"\"){ alert(\"please add atleast one item into select list\");return false;}}catch (e){}" + "\r\n");
        sJS.Append("        var lblText = document.getElementById(\'" + (Strings.Replace(txtLabel.UniqueID.ToString(), "$", "_", 1, -1, 0)) + "\');" + "\r\n");
        sJS.Append("        var labelpattern=/^[a-zA-Z\\d\\u0000-\\uFFFF][\\w#@\\s\\u0000-\\uFFFF]{0,127}$/;" + "\r\n");
        sJS.Append("        if (lblText.value == \'\') { " + "\r\n" + "alert(\"" + m_refMsg.GetMessage("js: enter property label").ToString() + "\");" + "\r\n");
        sJS.Append("        return false; " + "\r\n" + "}" + "\r\n");
        sJS.Append("        if (!labelpattern.test(lblText.value)) { " + "\r\n" + "alert(\"" + m_refMsg.GetMessage("js: invalid property label").ToString() + "\");" + "\r\n");
        sJS.Append("        return false; " + "\r\n" + "}" + "\r\n");
        sJS.Append("try{" + "\r\n");
        sJS.Append("        var ddValid = document.getElementById(\'" + (Strings.Replace((string)(this.ddValidationType.UniqueID.ToString()), "$", "_", 1, -1, 0)) + "\');" + "\r\n");
        sJS.Append("        if (ddValid.selectedIndex > 0) { if (document.getElementById(\'" + (Strings.Replace((string)(this.txtMessage.UniqueID.ToString()), "$", "_", 1, -1, 0)) + "\').value == \'\') {alert(\"Please enter validation message.\"); return false;} } " + "\r\n");
        sJS.Append("}catch (e){}" + "\r\n");
        sJS.Append("        return true;" + "\r\n");
        sJS.Append("" + "\r\n");
        sJS.Append("    }" + "\r\n");
        sJS.Append("    function VerifyDeleteCustomProp() {" + "\r\n");
        sJS.Append("        return confirm(\'" + m_refMsg.GetMessage("js: delete user prop msg") + "\');" + "\r\n");
        sJS.Append("    }" + "\r\n");
        sJS.Append("    function VerifyDeleteCustomPropLabel() {" + "\r\n");
        sJS.Append("        return confirm(\'" + m_refMsg.GetMessage("js: delete user prop trans label") + "\');" + "\r\n");
        sJS.Append("    }" + "\r\n");
        sJS.Append("</script>" + "\r\n");
        result.Append(sJS.ToString());
        if (m_bIsEdit)
        {
            if (m_CommAPI.DefaultContentLanguage == ContentLanguage)
            {
                txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props edit"));
            }
            else
            {
                txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props trans"));
            }
        }
        else
        {
            txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props add"));
        }

        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", (string)("users.aspx?action=ViewCustomProp&LangType=" + ContentLanguage + "&OrderBy=" + Request.QueryString["OrderBy"]), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt save button text (user property)"), m_refMsg.GetMessage("btn save"), "Onclick=\"javascript:return SubmitForm(\'userinfo\', \'VerifyAddCustomProp()\');\"", StyleHelper.SaveButtonCssClass, true));
        if (m_bIsEdit)
        {
            if (m_CommAPI.DefaultContentLanguage == ContentLanguage)
            {
                if ((m_ucpdata.PropertyValueType != EkEnumeration.ObjectPropertyValueTypes.Category) && (m_ucpdata.PropertyValueType != EkEnumeration.ObjectPropertyValueTypes.ThreadedDiscussion))
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/delete.png", (string)("users.aspx?action=DeleteCustomProp&id=" + m_iId), m_refMsg.GetMessage("alt delete button text"), m_refMsg.GetMessage("btn delete"), "Onclick=\"javascript:return VerifyDeleteCustomProp();\"", StyleHelper.DeleteButtonCssClass));
                }
            }
            else
            {
				result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/delete.png", (string)("users.aspx?action=DeleteCustomProp&type=label&id=" + m_iId), m_refMsg.GetMessage("alt delete button text"), m_refMsg.GetMessage("btn delete"), "Onclick=\"javascript:return VerifyDeleteCustomPropLabel();\"", StyleHelper.DeleteButtonCssClass));
            }

            if (EnableMultiLanguage == 1)
            {
				result.Append(StyleHelper.ActionBarDivider);
                result.Append("<td align=\"right\">" + m_refStyle.ShowAllActiveLanguage(false, "", "javascript:SelLanguage(this.value);", ContentLanguage.ToString()) + "</td>");
            }
            else
            {
                result.Append("<td>&nbsp;</td>");
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("AddCustomProperty", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void SetForm()
    {
        if (!m_bIsEdit)
        {
            //Input
            BindInputTypes(4);
            GetAllValidation(Convert.ToInt32(EkEnumeration.ObjectPropertyDisplayTypes.TextBox));
        }
        else
        {
            //Input
            BindInputTypes(0);
            GetAllValidation(-1);
        }
        //Type
        BindCMSObjectTypes(0);
        if (m_bIsEdit)
        {
            ReDoForm((Int32)m_ucpdata.PropertyValueType);
            Pupolate_CustomProperty(m_iId);

            lblType.Enabled = false;
            ddTypes.Enabled = false;
            if (ContentLanguage != m_CommAPI.DefaultContentLanguage)
            {
                OnlyLabelEdit(true);
            }
        }
        if (PageAction == "addcustomprop" || PageAction == "editcustomprop")
        {
            View_AddCustomProp_Toolbar();
        }
    }
    private void OnlyLabelEdit(bool bDisable)
    {
        //Hide controls
        this.lblInputType.Enabled = false;
        this.ddInputType.Enabled = false;
        this.TR_Min.Visible = false;
        this.TR_Max.Visible = false;
        this.lblType.Enabled = false;
        this.ddTypes.Enabled = false;
        //Disable controls
        this.lblValidation.Enabled = false;
        this.ddValidationType.Enabled = false;
    }
    private void ShowValidationUI(bool bShow)
    {
        //Me.TR_Min.Visible = bShow
        //Me.TR_Max.Visible = bShow
        //Me.TR_Message.Visible = bShow
        this.lblMaxVal.Visible = bShow;
        this.txtMaxValue.Visible = bShow;
        this.lblMinVal.Visible = bShow;
        this.txtMinValue.Visible = bShow;
        this.lblMessage.Visible = bShow;
        this.txtMessage.Visible = bShow;
    }

    private void GetAllValidation(int DisplayType)
    {
        string arMin = string.Empty;
        string arMax = string.Empty;
        Collection cEnums = new Collection();

        ListItem lsItem;
        string selVal = "";
        int iValType = 0;
        string ctrlName = string.Empty;

        if (IsPostBack)
        {
            ctrlName = GetPostBackControlName();
            if (ctrlName.ToString().ToLower() == ddInputType.UniqueID.ToString().ToLower())
            {
                selVal = (string)(Request.Form[ddInputType.UniqueID.ToString()].ToString());
            }
            else if (ctrlName.ToString().ToLower() == ddTypes.UniqueID.ToString().ToLower())
            {
                selVal = DisplayType.ToString();
            }
        }
        if (selVal == "")
        {
            selVal = DisplayType.ToString();
        }
        ddInputType.SelectedIndex = ddInputType.Items.IndexOf(ddInputType.Items.FindByValue(selVal));
        if ((selVal) == EkEnumeration.ObjectPropertyDisplayTypes.CheckBox.ToString())
        {
            cEnums = null;
        }
        else if ((selVal) == EkEnumeration.ObjectPropertyDisplayTypes.RadioButton.ToString())
        {
            cEnums = null;
        }
        else if ((selVal) == EkEnumeration.ObjectPropertyDisplayTypes.Hidden.ToString())
        {
            cEnums = null;
        }
        else if ((selVal) == EkEnumeration.ObjectPropertyDisplayTypes.DropdownList.ToString())
        {
            cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("SELECT", "", true);
        }
        else if ((selVal) == EkEnumeration.ObjectPropertyDisplayTypes.MultiSelectList.ToString())
        {
            cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("SELECT", "", true);
        }
        else if ((selVal) == EkEnumeration.ObjectPropertyDisplayTypes.TextArea.ToString())
        {
            cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("TEXTAREA", "TEXT", false);
        } //EkEnumeration.ObjectPropertyDisplayTypes.TextBox
        else
        {
            cEnums = (m_CommAPI.EkModuleRef).GetAllValidationEnum("INPUT", "TEXT", false);
        }
        //Clear the validation dropdown list
        this.ddValidationType.Items.Clear();
        //Populate the dropdown list but it should filter by type
        if (m_bIsEdit)
        {
            iValType = Convert.ToInt32(m_ucpdata.PropertyValueType);
        }
        else
        {
            if (ddTypes.SelectedValue != "")
            {
                iValType = GetObjValType(ddTypes.SelectedValue);
            }
        }
        if (cEnums == null)
        {
            ddValidationType.Enabled = false;
            ShowValidationUI(false);
        }
        else
        {
            ddValidationType.Enabled = true;
            ShowValidationUI(true);
            foreach (Collection cEnum in cEnums)
            {
                lsItem = new ListItem();
                lsItem.Text = cEnum["EnumName"].ToString();
                lsItem.Value = cEnum["EnumID"].ToString();
                if (CheckValidValidationType(iValType, Convert.ToInt32(cEnum["EnumID"].ToString())))
                {
                    ddValidationType.Items.Add(lsItem);
                    if (Convert.ToInt32(cEnum["EnumRange"].ToString()) != 0)
                    {
                        if (Convert.ToInt32(cEnum["EnumRange"].ToString()) == 3)
                        {
                            if (arMin != "")
                            {
                                arMin = arMin + ", " + cEnum["EnumID"].ToString();
                            }
                            else
                            {
                                arMin = cEnum["EnumID"].ToString();
                            }
                            if (arMax != "")
                            {
                                arMax = arMax + ", " + cEnum["EnumID"].ToString();
                            }
                            else
                            {
                                arMax = cEnum["EnumID"].ToString();
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(cEnum["EnumRange"].ToString()) == 1)
                            {
                                if (arMin != "")
                                {
                                    arMin = arMin + ", " + cEnum["EnumID"].ToString();
                                }
                                else
                                {
                                    arMin = cEnum["EnumID"].ToString();
                                }
                            }
                            else
                            {
                                if (arMax != "")
                                {
                                    arMax = arMax + ", " + cEnum["EnumID"].ToString();
                                }
                                else
                                {
                                    arMax = cEnum["EnumID"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            ddValidationType.Attributes.Add("onchange", "show_range2(\'" + arMin + "\',\'" + arMax + "\', this);Ektron.Workarea.Grids.init();");
            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "onChangeValidation", "<script language=\"Javascript\">function myBodyLoad() { show_range2(\'" + (arMin) + "\',\'" + (arMax) + "\', document.forms[0].elements[\"" + ddValidationType.UniqueID + "\"]);}</script>");
        }
    }

    private bool CheckValidValidationType(int Type, int ValidationID)
    {
        bool returnvalue = false; ;
        if (Type == 0)
        {
            returnvalue = true;

        }
        switch (Type)
        {
            case (int)EkEnumeration.ObjectPropertyValueTypes.Date:
                if ((ValidationID == 0) || (ValidationID == 1) || (ValidationID == 4))
                {
                    returnvalue = true;
                }
                break;
            case (int)EkEnumeration.ObjectPropertyValueTypes.Numeric:
                if ((ValidationID == 0) || (ValidationID == 1) || (ValidationID == 2) || (ValidationID == 11) || (ValidationID == 16))
                {
                    returnvalue = true;
                }
                break;
            case (int)EkEnumeration.ObjectPropertyValueTypes.Category:
                returnvalue = true;
                break;
            default: //EkEnumeration.ObjectPropertyValueTypes.String                      
                break;

        }
        return returnvalue;
    }

    private string GetPostBackControlName()
    {
        if (!(Page.Request.Params["__EVENTTARGET"] == null))
        {
            return Page.Request.Params["__EVENTTARGET"].ToString();
        }
        return string.Empty;
    }

    private void DisplayDateFields(string StartDate, string EndDate)
    {
        //Hide text box
        this.txtMaxValue.Visible = false;
        this.txtMinValue.Visible = false;

        EkDTSelector dateSchedule;
        dateSchedule = this.m_CommAPI.EkDTSelectorRef;
        dateSchedule.formName = "Form1";
        dateSchedule.extendedMeta = true;
        dateSchedule.formElement = "start_date";
        dateSchedule.spanId = "start_date_span";
        if (StartDate != "")
        {
            dateSchedule.targetDate = DateTime.Parse(StartDate);
        }
        dtStart.Text = dateSchedule.displayCultureDate(true, "", "");
        dateSchedule.formElement = "end_date";
        dateSchedule.spanId = "end_date_span";
        if (EndDate != "")
        {
            dateSchedule.targetDate = DateTime.Parse(EndDate);
        }
        dtEnd.Text = dateSchedule.displayCultureDate(true, "", "");
    }
    #endregion

    #region Populate Edit Form
    private void Pupolate_CustomProperty(long Id)
    {
        if (m_ucpdata != null)
        {
            txtLabel.Text = Server.HtmlDecode(m_ucpdata.Name);
            txtLabel.ToolTip = txtLabel.Text;
            txtMessage.Text = Server.HtmlDecode(m_ucpdata.PropertyValidationMessage);
            txtMessage.ToolTip = txtMessage.Text;
            ddValidationType.SelectedIndex = ddValidationType.Items.IndexOf(ddValidationType.Items.FindByValue(m_ucpdata.PropertyValidationType.ToString()));
            txtMessage.ToolTip = txtMessage.Text;
            if (ContentLanguage != m_CommAPI.DefaultContentLanguage)
            {
                Page.ClientScript.RegisterHiddenField("hdnRequired", m_ucpdata.Required.ToString());
                Page.ClientScript.RegisterHiddenField("hdnddTypes", m_ucpdata.PropertyValueType.ToString());
                Page.ClientScript.RegisterHiddenField("hdninputtype", m_ucpdata.PropertyDisplayValueType.ToString());
                Page.ClientScript.RegisterHiddenField("hdnddValidationType", m_ucpdata.PropertyValidationType.ToString());
                Page.ClientScript.RegisterHiddenField("hdnMinValue", (string)m_ucpdata.PropertyValidationMinVal);
                Page.ClientScript.RegisterHiddenField("hdnMaxValue", (string)m_ucpdata.PropertyValidationMaxVal);
                if (m_ucpdata.Name == "")
                {
                    Page.ClientScript.RegisterHiddenField("hdnInsert", "true"); //Insert label on the pastback
                }
                else
                {
                    Page.ClientScript.RegisterHiddenField("hdnInsert", "false"); //Update label on the pastback
                }
            }
            else
            {
                //chkRequired.Value = m_ucpdata.Required
                ddTypes.SelectedIndex = ddTypes.Items.IndexOf(ddTypes.Items.FindByValue(m_ucpdata.PropertyValueType.ToString()));
                txtMinValue.Text = m_ucpdata.PropertyValidationMinVal.ToString();
                txtMinValue.ToolTip = txtMinValue.Text;
                txtMaxValue.Text = m_ucpdata.PropertyValidationMaxVal.ToString();
                txtMaxValue.ToolTip = txtMaxValue.Text;
                Page.ClientScript.RegisterHiddenField("hdnddTypes", m_ucpdata.PropertyValueType.ToString());
            }
            if (m_ucpdata.PropertyValueType == EkEnumeration.ObjectPropertyValueTypes.SelectList || m_ucpdata.PropertyValueType == EkEnumeration.ObjectPropertyValueTypes.MultiSelectList)
            {
                DisplaySelect = true;
                m_strSelectedValue = m_ucpdata.PropertyValidationSelectList;
                m_intValidationType = m_ucpdata.PropertyValidationType;
            }
        }
    }
    #endregion

    #region Process on submit
    #region Helper Functions
    private void DoProcess()
    {
        m_UserRef = m_CommAPI.EkUserRef;

        if (PageAction == "editcustomprop")
        {
            DoEditCustomProperty();
        }
        else if (PageAction == "deletecustomprop")
        {
            DoDeleteCustomProperty();
        }
        else
        {
            DoAddCustomProperty();
        }
        if (PageAction != "deletecustomprop")
        {
            Response.Redirect(m_CommAPI.AppPath + "users.aspx?action=ViewCustomProp", false);
        }

    }

    private UserCustomPropertyData ReadFormPostData()
    {
        UserCustomPropertyData ucpdata = new UserCustomPropertyData();
        string strLabel;
        bool bReq = false;

        EkEnumeration.ObjectPropertyValueTypes ty = new EkEnumeration.ObjectPropertyValueTypes();
        EkEnumeration.ObjectPropertyDisplayTypes inputtype = new EkEnumeration.ObjectPropertyDisplayTypes();

        int validation = 0;
        string strValidationMsg = "";
        string strMinVal = "";
        string strMaxVal = "";

        strLabel = Request.Form[txtLabel.UniqueID].ToString().Trim();
        if (!(Request.Form[txtMessage.UniqueID] == null))
        {
            strValidationMsg = (string)(Request.Form[txtMessage.UniqueID].ToString().Trim());
        }
        if (ContentLanguage != m_CommAPI.DefaultContentLanguage)
        {
            if ((!(Request.Form["hdnRequired"] == null)) && ((Request.Form["hdnRequired"].ToString() == "on") || (Request.Form["hdnRequired"].ToString() == "1") || (Request.Form["hdnRequired"].ToString().ToLower() == "true")))
            {
                bReq = true;
            }
            if (Request.Form["hdnddTypes"] != null)
                ty = (EkEnumeration.ObjectPropertyValueTypes)Enum.Parse(typeof(EkEnumeration.ObjectPropertyValueTypes), Request.Form["hdnddTypes"].ToString());

            if (Request.Form["hdninputtype"] != null)
                inputtype = (EkEnumeration.ObjectPropertyDisplayTypes)Enum.Parse(typeof(EkEnumeration.ObjectPropertyDisplayTypes), Request.Form["hdninputtype"].ToString());
            int.TryParse(Request.Form["hdnddValidationType"], out validation);
            if (Request.Form["hdnMinValue"] != null)
                strMinVal = Request.Form["hdnMinValue"].ToString().Trim();
            if (Request.Form["hdnMaxValue"] != null)
                strMaxVal = Request.Form["hdnMaxValue"].ToString().Trim();
        }
        else
        {
            //If ((Not Request.Form(chkRequired.UniqueID.ToString) Is Nothing) AndAlso ((Request.Form(chkRequired.UniqueID.ToString).ToString() = "on") Or (Request.Form(chkRequired.UniqueID.ToString).ToString() = "1"))) Then
            //    bReq = True
            //End If
            if (!(Request.Form["hdnddTypes"] == null))
            {
                ty = (EkEnumeration.ObjectPropertyValueTypes)Enum.Parse(typeof(EkEnumeration.ObjectPropertyValueTypes), Request.Form["hdnddTypes"].ToString());
            }
            else
            {
                ty = (EkEnumeration.ObjectPropertyValueTypes)Enum.Parse(typeof(EkEnumeration.ObjectPropertyValueTypes), Request.Form[ddTypes.UniqueID].ToString());
            }
			if (!string.IsNullOrEmpty(Request.Form[ddInputType.UniqueID]))
            {
                inputtype = (EkEnumeration.ObjectPropertyDisplayTypes)Enum.Parse(typeof(EkEnumeration.ObjectPropertyDisplayTypes), Request.Form[ddInputType.UniqueID].ToString());
            }            
            if (!(Request.Form[ddValidationType.UniqueID] == null))
            {
                validation = System.Convert.ToInt32(Request.Form[ddValidationType.UniqueID]);
            }
            if (ty == EkEnumeration.ObjectPropertyValueTypes.Date)
            {
                if (!(Request.Form["start_Date"] == null))
                {
                    strMinVal = (string)(Request.Form["start_Date"].ToString().Trim());
                }
                if (!(Request.Form["end_date"] == null))
                {
                    strMaxVal = (string)(Request.Form["end_date"].ToString().Trim());
                }
            }
            else
            {
                if (!(Request.Form[txtMinValue.UniqueID] == null))
                {
                    strMinVal = (string)(Request.Form[txtMinValue.UniqueID].ToString().Trim());
                }
                if (!(Request.Form[txtMaxValue.UniqueID] == null))
                {
                    strMaxVal = (string)(Request.Form[txtMaxValue.UniqueID].ToString().Trim());
                }
            }
        }
        if (ty == EkEnumeration.ObjectPropertyValueTypes.SelectList || ty == EkEnumeration.ObjectPropertyValueTypes.MultiSelectList)
        {
            if (Request.Form["selectedvalues"] == "")
            {
                throw (new Exception("please add atleast one item into select list"));
            }
            else
            {
                ucpdata.PropertyValidationSelectList = Request.Form["selectedvalues"];
                if (!string.IsNullOrEmpty(Request.Form["chkValidation"]))
                {
                    bReq = true;
                    validation = 8;
                }
                else
                {
                    bReq = false;
                    validation = 0;
                }
            }
        }
        if (strLabel == string.Empty)
        {
            throw (new Exception("Please insert the label."));
        }
        ucpdata.Name = strLabel;
        ucpdata.Required = bReq || (validation > 0);
        ucpdata.PropertyValueType = ty;
        ucpdata.PropertyValidationType = validation;
        ucpdata.PropertyValidationMinVal = strMinVal;
        ucpdata.PropertyValidationMaxVal = strMaxVal;
        ucpdata.PropertyValidationMessage = strValidationMsg;
        ucpdata.PropertyDisplayValueType = inputtype;

        return ucpdata;
    }
    #endregion

    #region Add
    private void DoAddCustomProperty()
    {
        long iRet = 0;
        UserCustomPropertyData ucpdata;
        ucpdata = ReadFormPostData();

        for (titleCounter = 0; titleCounter < allUCPData.Length; titleCounter++)
        {
            if (allUCPData[titleCounter].Name == ucpdata.Name || allUCPData[titleCounter].Name == ucpdata.Name + "(" + titleIncrement + ")")
            {
                titleIncrement = titleIncrement + 1;
            }
        }
        if (titleIncrement > 0)
        {
            ucpdata.Name = ucpdata.Name + "(" + titleIncrement + ")";
        }

        iRet = m_UserRef.AddCustomProperty(ucpdata);
        //ltrJS.Text = "<script language=""JavaScript"">" & Environment.NewLine & "window.location='users.aspx?action=ViewCustomProp&LangType=" & m_CommAPI.DefaultContentLanguage & "';</script>"
    }
    #endregion

    #region Edit
    private void DoEditCustomProperty()
    {
        UserCustomPropertyData ucpdata;
        long iRet = 0;
        bool bInsert = false;

        ucpdata = ReadFormPostData();
        ucpdata.ID = m_iId;
        if (ContentLanguage != m_CommAPI.DefaultContentLanguage)
        {
            if ((!(Request.Form["hdnInsert"] == null)) && (Request.Form["hdnInsert"].ToString().ToLower() == "true"))
            {
                bInsert = true;
            }
        }

        for (titleCounter = 0; titleCounter < allUCPData.Length; titleCounter++)
        {
            if (allUCPData[titleCounter].Name == ucpdata.Name)
            {
                ucpdata.Name = m_ucpdata.Name;
            }
        }

        if (bInsert)
        {
            iRet = m_UserRef.AddCustomProperty(ucpdata);
        }
        else
        {
            iRet = m_UserRef.UpdateCustomProperty(ucpdata);
        }
        //ltrJS.Text = "<script language=""JavaScript"">" & Environment.NewLine & "window.location='users.aspx?action=ViewCustomProp&LangType=" & m_CommAPI.DefaultContentLanguage & "';</script>"
    }
    #endregion

    #region Delete
    private void DoDeleteCustomProperty()
    {
        string strAct = string.Empty;
        bool bRet = false;
        if (!(Request.QueryString["type"] == null))
        {
            if (Request.QueryString["type"].ToString().Trim() != string.Empty)
            {
                strAct = (string)(Request.QueryString["type"].ToString().Trim());
            }
        }
        if (ContentLanguage == m_CommAPI.DefaultContentLanguage)
        {
            strAct = "";
        }
        if (m_iId > 0)
        {
            if (strAct == "label")
            {
                //Remove only translated label not the property.
                bRet = m_UserRef.DeleteCustomProperty(0, true);
                //Response.Redirect("users.aspx?action=EditCustomProp&id=" & m_iId & "&LangType=" & m_CommAPI.DefaultContentLanguage, False)
            }
            else
            {
                bRet = m_UserRef.DeleteCustomProperty(m_iId, false);
            }
            Response.Redirect("users.aspx?action=ViewCustomProp", false);
        }
    }
    #endregion
    #endregion
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, this.m_CommAPI.ApplicationPath + "java/internCalendarDisplayFuncs.js", "EktronInternCalendarDisplayFuncs");
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
    }
}
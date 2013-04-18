using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ektron.Cms.Common;
using Ektron.Cms;
using Microsoft.VisualBasic;

/// <summary>
/// Summary description for SearchHelper
/// </summary>
public class SearchHelper
{
    protected EkMessageHelper _MessageHelper;
    private CommonApi _RefAPI = null;
    protected const int MetaTagType_Searchable = 100;
    protected const int STANDARD_PROP = 0;
    protected const int DMS_PROP = 1;
    protected const int CUSTOM_PROP = 2;
    protected const string TEXT_PROP = "text";
    protected const string NUMBER_PROP = "number";
    protected const string BYTE_PROP = "byte";
    protected const string DOUBLE_PROP = "double";
    protected const string FLOAT_PROP = "float";
    protected const string INTEGER_PROP = "integer";
    protected const string LONG_PROP = "long";
    protected const string SHORT_PROP = "short";
    protected const string DATE_PROP = "date";
    protected const string SELECT_PROP = "select";
    protected const string SELECT1_PROP = "select1";
    protected const string BOOLEAN_PROP = "boolean";
    protected const int MetaTagType_Meta = 1;

    public SearchHelper()
    {
        _RefAPI = new CommonApi();
        _MessageHelper = _RefAPI.EkMsgRef;

    }

    public string CheckedAttr(bool bChecked)
    {
        return (bChecked ? " checked" : "");
    }

    public string DisabledAttr(bool bDisabled)
    {
        return (bDisabled ? " disabled" : "");
    }

    public string SelectedAttr(bool bSelected)
    {
        return (bSelected ? " selected" : "");
    }

    public string SelectedValueAttr(object OptionValue, object SelectedValue)
    {
        return " value=\"" + OptionValue + "\"" + SelectedAttr(OptionValue.ToString() == SelectedValue.ToString());
    }

    public string BoolToYesNo(bool bValue)
    {
        if (bValue)
        {
            return (_MessageHelper.GetMessage("generic Yes"));
        }
        else
        {
            return (_MessageHelper.GetMessage("generic No"));
        }
    }

    public string MetaTagTypeBoxTop(bool bView, int MetaTagType, string Caption)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (!bView)
        {
            sb.Append("<div id=\"idMetaTagType_" + MetaTagType + "\">");
            sb.Append("<fieldset>");
            sb.Append("<legend>" + Caption + "</legend>");
            sb.Append("<table width=\"100%\" class=\"ektronForm\">");
        }
        return (sb.ToString());
    }

    public object MetaTagTypeBoxBottom(bool bView)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (!bView)
        {
            sb.Append("</table>");
            sb.Append("</fieldset>");
            sb.Append("</div>");
        }
        return (sb.ToString());
    }


    // TODO: Convert to use EkDTSelector -- this should become obsolete.
    public object getDateFromString(string strDate)
    {
        string MonthShortNames = null;
        Collection cMonthInfo = null;
        string sfErrString = null;
        object sfDatecurrentUserID = null;
        object sfDateSite = null;
        sfErrString = "";
        Array monthNames = null;
        Array arBoth = null;
        Array arDate = null;
        Array arTime = null;
        object newDateTime = null;
        object strAmPm = null;
        if (HttpContext.Current.Request.Cookies["ecm"].HasKeys)
        {
            sfDatecurrentUserID = HttpContext.Current.Request.Cookies["ecm"]["user_id"];
            sfDateSite = HttpContext.Current.Request.Cookies["ecm"]["site_id"];
        }
        else
        {
            sfDatecurrentUserID = 0;
        }

        // cSfTemp = Scripting.Dictionary
        Collection csfTemp = new Collection();

        csfTemp.Add("", "DateString", null, null);
        csfTemp.Add("", "FormatString", null, null);

        EkFunctions ekF = new EkFunctions();

        cMonthInfo = EkFunctions.GetDateStrings(csfTemp);

        if (string.IsNullOrEmpty(sfErrString) & cMonthInfo.Count > 0)
        {
            MonthShortNames = cMonthInfo["MonthNameString"].ToString();
            monthNames = Strings.Split(MonthShortNames, ",", -1, 0);
        }

        if ((Information.UBound(monthNames, 1) >= 1))
        {
            arBoth = Strings.Split(strDate, "T", -1, 0);
            if ((Information.UBound(arBoth, 1) >= 1))
            {
                arDate = Strings.Split(arBoth.GetValue(0).ToString(), "-", -1, 0);
                arTime = Strings.Split(arBoth.GetValue(1).ToString(), ":", -1, 0);

                if ((Convert.ToInt32(arTime.GetValue(0)) >= 12))
                {
                    strAmPm = "PM";
                    arTime.SetValue(Convert.ToInt32(Convert.ToInt32(arTime.GetValue(0)) - 12), 0);
                }
                else
                {
                    strAmPm = "AM";
                }

                newDateTime = arDate.GetValue(2) + "-" + monthNames.GetValue(Convert.ToInt32(arDate.GetValue(1))) + "-" + arDate.GetValue(0) + " " + arTime.GetValue(0) + ":" + arTime.GetValue(1) + " " + strAmPm;
                return (newDateTime);
            }
            else
            {
                DateTime dt = default(DateTime);
                if ((DateTime.TryParse(strDate.ToString(), out dt)))
                {
                    return (dt.ToString("d"));
                }
                else
                {
                    return ("");
                }
            }
        }
        return null;
    }


    public string WriteSearchProperties(object AppConfStr, object currentUserID, object Site, long folderId, string szBeginFontInfo, string szEndFontInfo)
    {


        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Collection CustomSearchProperties = null;

        CustomSearchProperties = (Collection)_RefAPI.EkContentRef.GetCustomSearchProperties(folderId, null, true);
        if (CustomSearchProperties.Count > 0)
        {
            sb.Append("<table width=\"100%\" id=\"custom_group\">");
            sb.Append("<tbody>");
            sb.Append(WriteSearchColumns());
            sb.Append(WriteSectionSeparator());
            foreach (Collection CustomSearchProp in CustomSearchProperties)
            {

                sb.Append(Ektron.Cms.CustomFields.WriteSearchProp(CUSTOM_PROP.ToString(), CustomSearchProp["DataType"].ToString(), CustomSearchProp["Caption"].ToString(), "ecmCustom_" + CustomSearchProp["ID"].ToString(), (Collection)CustomSearchProp["Items"], "left", szBeginFontInfo, szEndFontInfo));
            }
            sb.Append("</tbody>");
            sb.Append("</table>");

            return (sb.ToString());


        }
        return null;
    }

    public string WriteSearchColumns()
    {
        return ("<col width=\"40%\"><col width=\"60%\">");
    }

    public string WriteSectionSeparator()
    {
        return ("<tr><td style=\"line-height:1ex;\">&nbsp;</td></tr>");
    }

    public string IIf(bool expr, string truepart, string falsepart)
    {
        string functionReturnValue = "";
        if (expr)
        {
            functionReturnValue = truepart;
        }
        else
        {
            functionReturnValue = falsepart;
        }
        return functionReturnValue;
    }

    // I don't know of a way to detect whether the argument is an object or not in VBScript.
    // On Error Goto is not supported, TypeOf is not supported, TypeName is too specific.
    public Collection IIfSet(bool expr, Collection truepart, Collection falsepart)
    {
        Collection functionReturnValue;
        if (expr)
        {
            functionReturnValue = truepart;
        }
        else
        {
            functionReturnValue = falsepart;
        }
        return functionReturnValue;
    }

    public string WriteMetadataForView(Collection cMetadataTypes)
    {

        System.Text.StringBuilder cRet = new System.Text.StringBuilder();

        foreach (Collection cMetadataType in cMetadataTypes)
        {
            // ONLY SHOW CUSTOM SEARCH PROPERTIES
            if (Convert.ToInt32(cMetadataType["MetaTagType"]) >= MetaTagType_Searchable && Convert.ToBoolean(cMetadataType["MetaEditable"]))
            {
                cRet.Append(WriteMetadataTypeForView(cMetadataType["MetaNameTitle"].ToString(), cMetadataType["MetaTypeName"].ToString(), cMetadataType["MetaText"].ToString()));
            }
        }

        return (cRet.ToString());
    }

    public string WriteMetadataTypeForView(string DataType, string Caption, string Value)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("<tr>");
        sb.Append("<td class=\"label\">" + Caption + ":</td>");
        sb.Append("<td class=\"readOnlyValue\">" + WriteMetadataValue(DataType, Value) + "</td>");
        sb.Append("</tr>");
        return (sb.ToString());
    }

    public object WriteMetadataValue(string DataType, string Value)
    {
        if (DataType == "date")
        {
            return (getDateFromString(Value));
        }
        else
        {
            return (Value);
        }
    }

    public object WriteMetadataTypeForEdit(Collection cMetadataType, string lValidCounter, object ty)
    {
        return (WriteMetadataTypeForEditEx(false, cMetadataType, cMetadataType["MetaNameTitle"].ToString(), lValidCounter, ty));
    }

    public object WriteMetadataDefaultForEdit(Collection cMetadataType, string DataType)
    {
        return (WriteMetadataTypeForEditEx(true, (Collection)cMetadataType, DataType, DataType, ""));
    }

    // See mediauploader.aspx and library.aspx for similar definition of fields
    public object WriteMetadataTypeForEditEx(bool bIsDefault, Collection cMetadataType, string DataType, string lValidCounter, object ty)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int nMetaTagType = -1;
        string strName = "";
        string strSelName = "";
        string IdAttr = "";
        bool bIsEditable = false;
        bool bSelectableOnly = false;
        bool bAllowMulti = false;
        bool bMetaRequired = false;
        string strLabelAlign = "";
        string strDefaultNotes = "";
        object strMetaTypeName = "";
        long nMetaTypeID = -1;
        string strMetaDefault = "";
        string strMetaSeparator = "";
        string strMetaSelectableText = "";
        string strMetaValue = "";
        strDefaultNotes = "";
        if (bIsDefault)
        {
            nMetaTagType = MetaTagType_Searchable;
            strName = "frm_metadefault_" + nMetaTagType + "_" + DataType;
            IdAttr = " id=\"idSearchPropStyle_" + DataType + "\"";
            strLabelAlign = "left";
            bIsEditable = true;
            bSelectableOnly = (SELECT1_PROP == DataType | SELECT_PROP == DataType | BOOLEAN_PROP == DataType);
            bAllowMulti = (SELECT_PROP == DataType);
            bMetaRequired = false;
            strMetaTypeName = _MessageHelper.GetMessage("lbl default");
            if ((BOOLEAN_PROP == DataType))
            {
                strDefaultNotes = "&nbsp;&nbsp;" + _MessageHelper.GetMessage("lbl (Checked for Yes)");
            }
        }
        else
        {
            nMetaTagType = Convert.ToInt32(cMetadataType["MetaTagType"]);
            strName = "frm_text_" + lValidCounter;
            // many locations depend on this name
            IdAttr = "";
            strLabelAlign = "right";
            bIsEditable = Convert.ToBoolean(cMetadataType["MetaEditable"]);
            bSelectableOnly = Convert.ToBoolean(cMetadataType["SelectableOnly"]);
            bAllowMulti = Convert.ToBoolean(cMetadataType["AllowMulti"]);
            bMetaRequired = Convert.ToBoolean(cMetadataType["MetaRequired"]);
            strMetaTypeName = cMetadataType["MetaTypeName"];
        }
        if (cMetadataType == null)
        {
            nMetaTypeID = 0;
            strMetaDefault = "";
            strMetaSeparator = ";";
            strMetaSelectableText = "";
            strMetaValue = "";
        }
        else
        {
            nMetaTypeID = Convert.ToInt64(cMetadataType["MetaTypeID"]);
            strMetaDefault = cMetadataType["MetaDefault"].ToString();
            strMetaSeparator = cMetadataType["MetaSeparator"].ToString();
            strMetaSelectableText = cMetadataType["MetaSelectableText"].ToString();
            if (((string)ty == "update"))
            {
                strMetaValue = cMetadataType["MetaText"].ToString();
            }
            else
            {
                strMetaValue = cMetadataType["MetaDefault"].ToString();
            }
        }
        strSelName = "frm_text_sel" + lValidCounter;
        // many locations depend on this name
        sb.Append("<tr " + IdAttr + ">" + Constants.vbCrLf);
        // Label
        if (bSelectableOnly)
        {
            sb.Append("<td class=\"label\" align=\"" + strLabelAlign + "\" nowrap=\"true\">" + Constants.vbCrLf);
            sb.Append("<label for=\"" + strSelName + "\">" + Constants.vbCrLf);
            if (bMetaRequired)
            {
                sb.Append("<span style=\"color:red\">" + Constants.vbCrLf);
            }
            sb.Append(strMetaTypeName + ":" + Constants.vbCrLf);
            if (bMetaRequired)
            {
                sb.Append("*</span>" + Constants.vbCrLf);
            }
            sb.Append("</label>" + Constants.vbCrLf);
            sb.Append("</td>" + Constants.vbCrLf);
        }
        else
        {
            sb.Append("<td class=\"label\" align=\"" + strLabelAlign + "\" nowrap=\"true\">" + Constants.vbCrLf);
            sb.Append("<label for=\"" + strName + "\">" + Constants.vbCrLf);
            if (bMetaRequired)
            {
                sb.Append("<span style=\"color:red\">" + Constants.vbCrLf);
            }
            sb.Append(strMetaTypeName + ":" + Constants.vbCrLf);
            if (bMetaRequired)
            {
                sb.Append("*</span>" + Constants.vbCrLf);
            }
            sb.Append("</label>" + Constants.vbCrLf);
            sb.Append("</td>" + Constants.vbCrLf);
        }

        // Data Value
        if ((bIsEditable))
        {
            sb.Append("<td class=\"readOnlyValue\">" + Constants.vbCrLf);
            sb.Append("<input type=\"hidden\" name=\"frm_meta_type_id_" + (lValidCounter) + "\" id=\"frm_meta_type_id_" + (lValidCounter) + "\" value=\"" + nMetaTypeID + "\">" + Constants.vbCrLf);
            sb.Append("<input type=\"hidden\" name=\"frm_meta_required_" + (lValidCounter) + "\" id=\"frm_meta_required_" + (lValidCounter) + "\" value=\"" + Convert.ToInt64(bMetaRequired) + "\">" + Constants.vbCrLf);
            sb.Append("<input type=\"hidden\" name=\"" + strName + "default\" id=\"" + strName + "default\" value=\"" + EkFunctions.HtmlEncode(strMetaDefault) + "\">" + Constants.vbCrLf);
            sb.Append("<input type=\"hidden\" name=\"MetaSeparator_" + (lValidCounter) + "\" id=\"MetaSeparator_" + (lValidCounter) + "\" value=\"" + EkFunctions.HtmlEncode(strMetaSeparator) + "\">" + Constants.vbCrLf);
            // Note, the following value was hard coded to one, changed to zero to fix defect #14817, BCB.
            sb.Append("<input type=\"hidden\" name=\"MetaSelect_" + (lValidCounter) + "\" id=\"MetaSelect_" + (lValidCounter) + "\" value=\"0\">" + Constants.vbCrLf);

            Array arrMetaOptions = null;
            Array arrMetaText = null;
            int i_option = 0;
            string toption = null;
            int nOptionCount = -1;
            arrMetaOptions = Strings.Split(EkFunctions.HtmlDecode(strMetaSelectableText).ToString(), strMetaSeparator, -1, 0);
            nOptionCount = Information.UBound(arrMetaOptions, 1) - Information.LBound(arrMetaOptions, 1) + 1;
            arrMetaText = Strings.Split(EkFunctions.HtmlDecode(strMetaValue), strMetaSeparator, -1, 0);

            if (BOOLEAN_PROP == DataType)
            {
                object strChecked = null;
                object strValue = null;
                strChecked = "";
                if (nOptionCount < 2)
                {
                    // Avoid an error
                    arrMetaOptions = Strings.Split(BoolToYesNo(false) + strMetaSeparator + BoolToYesNo(true), strMetaSeparator, -1, 0);
                    nOptionCount = 2;
                }
                if (arrMetaOptions.GetValue(0).ToString() == strMetaValue | string.IsNullOrEmpty(strMetaValue))
                {
                    strValue = arrMetaOptions.GetValue(0);
                    strChecked = "";
                }
                else
                {
                    strValue = arrMetaOptions.GetValue(1);
                    strChecked = " checked";
                }

                sb.Append("<input type=\"hidden\" name=\"" + strName + "\" value=\"" + strValue + "\">" + Constants.vbCrLf);
                sb.Append("<input type=\"checkbox\" name=\"" + strSelName + "\" id=\"" + strSelName + "\" " + strChecked + " onclick=\"JavaScript:booleanSelected(document.forms[0]." + strSelName + ",document.forms[0]." + strName + ",'" + arrMetaOptions.GetValue(0) + "','" + arrMetaOptions.GetValue(1) + "');\">" + strDefaultNotes + Constants.vbCrLf);
                sb.Append("<scr" + "ipt  type=\"text/javascript\">" + Constants.vbCrLf);
                sb.Append("<!--" + Constants.vbCrLf);
                sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("var objForm = document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("if(objRemoteForm && objForm)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("if(objRemoteForm." + strSelName + ".value == \"false\")" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("objForm." + strSelName + ".checked = false;\t" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("else" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("objForm." + strSelName + ".checked = true;" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("objForm." + strName + ".value = objRemoteForm." + strName + ".value;" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("// -->" + Constants.vbCrLf);
                sb.Append("</scr" + "ipt>" + Constants.vbCrLf);
            }
            else if (bSelectableOnly)
            {
                if (bAllowMulti)
                {
                    sb.Append("<SCR" + "IPT type='text/javascript'>" + Constants.vbCrLf);
                    sb.Append("var opt" + (lValidCounter) + " = new OptionTransfer(\"list1_" + (lValidCounter) + "\",\"list2_" + (lValidCounter) + "\",\"myinput" + (lValidCounter) + "\");" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".setAutoSort(true);" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".setDelimiter(\"" + strMetaSeparator + "\");" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".saveRemovedLeftOptions(\"removedLeft" + (lValidCounter) + "\");" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".saveRemovedRightOptions(\"removedRight" + (lValidCounter) + "\");" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".saveAddedLeftOptions(\"addedLeft" + (lValidCounter) + "\");" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".saveAddedRightOptions(\"addedRight" + (lValidCounter) + "\");" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".saveNewLeftOptions(\"newLeft" + (lValidCounter) + "\");" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".saveNewRightOptions(\"" + strName + "\");" + Constants.vbCrLf);
                    sb.Append("</SC" + "RIPT>" + Constants.vbCrLf);

                    sb.Append("<input type=\"hidden\" name=\"delimiter\" value=\"" + EkFunctions.HtmlEncode(strMetaSeparator) + "\"/>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=\"autosort\" value=\"Y\"/>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=removedLeft" + (lValidCounter) + " value=\"\"/>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=removedRight" + (lValidCounter) + " value=\"\"/>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=addedLeft" + (lValidCounter) + " value=\"\"/>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=addedRight" + (lValidCounter) + " value=\"\"/>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=newLeft" + (lValidCounter) + " value=\"\"/>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=" + strName + " value=\"\"/>" + Constants.vbCrLf);

                    sb.Append("<SCR" + "IPT type='text/javascript'>" + Constants.vbCrLf);
                    sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                    sb.Append("{" + Constants.vbCrLf);
                    sb.Append(" var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append(" var objForm = document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append(" if(objRemoteForm && objForm)" + Constants.vbCrLf);
                    sb.Append(" {" + Constants.vbCrLf);
                    sb.Append("   objForm.removedLeft" + (lValidCounter) + ".value = objRemoteForm.removedLeft" + (lValidCounter) + ".value;" + Constants.vbCrLf);
                    sb.Append("   objForm.removedRight" + (lValidCounter) + ".value = objRemoteForm.removedRight" + (lValidCounter) + ".value;" + Constants.vbCrLf);
                    sb.Append("   objForm.addedLeft" + (lValidCounter) + ".value = objRemoteForm.addedLeft" + (lValidCounter) + ".value;" + Constants.vbCrLf);
                    sb.Append("   objForm.addedRight" + (lValidCounter) + ".value = objRemoteForm.addedRight" + (lValidCounter) + ".value;" + Constants.vbCrLf);
                    sb.Append("   objForm.newLeft" + (lValidCounter) + ".value = objRemoteForm.newLeft" + (lValidCounter) + ".value;" + Constants.vbCrLf);
                    sb.Append("   objForm." + strName + ".value = objRemoteForm." + strName + ".value;" + Constants.vbCrLf);
                    sb.Append(" }" + Constants.vbCrLf);
                    sb.Append("}" + Constants.vbCrLf);
                    sb.Append("</SCR" + "IPT>" + Constants.vbCrLf);

                    sb.Append("<div id=\"dvMetadata\">");
                    sb.Append("<table>" + Constants.vbCrLf);
                    sb.Append("<tbody>" + Constants.vbCrLf);
                    sb.Append("<tr><td>" + _MessageHelper.GetMessage("lbl Not Included:") + "</td>");
                    sb.Append("<td></td>");
                    sb.Append("<td>" + _MessageHelper.GetMessage("lbl Included:") + "</td>");
                    sb.Append("</tr>" + Constants.vbCrLf);
                    sb.Append("<TR>" + Constants.vbCrLf);
                    sb.Append("<td width=\"47%\">");
                    sb.Append("<select ondblclick=opt" + (lValidCounter) + ".transferRight() multiple size=10 name=list1_" + (lValidCounter) + " id=\"" + strSelName + "\" style=\"width:200px\"> " + Constants.vbCrLf);

                    for (i_option = 0; i_option <= Information.UBound(arrMetaOptions, 1); i_option++)
                    {
                        toption = Strings.Trim(arrMetaOptions.GetValue(i_option).ToString());
                        if (!string.IsNullOrEmpty(toption))
                        {
                            if (!(Testselection(arrMetaText, toption)))
                            {
                                sb.Append("<option value=\"" + toption + "\">" + toption + "</option>" + Constants.vbCrLf);
                            }
                        }
                    }
                    sb.Append("</select>" + Constants.vbCrLf);

                    sb.Append("<scr" + "ipt type='text/javascript'>" + Constants.vbCrLf);
                    sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                    sb.Append("{" + Constants.vbCrLf);
                    sb.Append(" var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append(" var objForm = document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append(" if(objRemoteForm && objForm)" + Constants.vbCrLf);
                    sb.Append(" {" + Constants.vbCrLf);
                    sb.Append("   if(objForm.newLeft" + (lValidCounter) + ".value != \"\" && objForm." + strName + ".value != \"\")" + Constants.vbCrLf);
                    sb.Append("   {" + Constants.vbCrLf);
                    sb.Append("     var wordArray = new Array;" + Constants.vbCrLf);
                    sb.Append("     wordArray = objForm.newLeft" + (lValidCounter) + ".value.split(objForm.delimiter.value);" + Constants.vbCrLf);
                    sb.Append("     var path = objForm.list1_" + (lValidCounter) + ".options;" + Constants.vbCrLf);
                    sb.Append("     for(var i=0; i < wordArray.length; i++)" + Constants.vbCrLf);
                    sb.Append("     {" + Constants.vbCrLf);
                    sb.Append("       path[i].value = wordArray[i];" + Constants.vbCrLf);
                    sb.Append("       path[i].text = wordArray[i];" + Constants.vbCrLf);
                    sb.Append("     }" + Constants.vbCrLf);
                    sb.Append("     for(var j=path.length-1; j >= i; j--)" + Constants.vbCrLf);
                    sb.Append("     {" + Constants.vbCrLf);
                    sb.Append("       path[j]=null;" + Constants.vbCrLf);
                    sb.Append("     }" + Constants.vbCrLf);
                    sb.Append("   }" + Constants.vbCrLf);
                    sb.Append(" }" + Constants.vbCrLf);
                    sb.Append("}" + Constants.vbCrLf);
                    sb.Append("</scr" + "ipt>" + Constants.vbCrLf);

                    if (nMetaTagType < MetaTagType_Searchable)
                    {
                        sb.Append("<br />" + Constants.vbCrLf);
                        sb.Append("<input name=\"myinput" + (lValidCounter) + "\" type=\"text\" size=\"15\" ID=\"Text3\">" + Constants.vbCrLf);
                    }
                    sb.Append("</td>" + Constants.vbCrLf);
                    sb.Append("<td class=\"moveMeta center\">" + Constants.vbCrLf);
                    sb.Append("<a style=\"background-color: #D0E5F5;\" class=\"button greenHover buttonAdd\" onclick=opt" + (lValidCounter) + ".transferRight() type=\"button\" title=\"" + _MessageHelper.GetMessage("generic add title") + "\" name=\"right\"></a>" + Constants.vbCrLf);
                    sb.Append("<a style=\"background-color: #D0E5F5;\" class=\"button greenHover buttonAddAll\" onclick=opt" + (lValidCounter) + ".transferAllRight() type=\"button\" title=\"" + _MessageHelper.GetMessage("generic add all title") + "\" name=\"right\"></a>" + Constants.vbCrLf);
                    sb.Append("<a style=\"background-color: #D0E5F5;\" class=\"button redHover buttonLeftRemove\" onclick=opt" + (lValidCounter) + ".transferLeft() type=\"button\" title=\"" + _MessageHelper.GetMessage("generic remove") + "\" name=\"left\" ></a>" + Constants.vbCrLf);
                    sb.Append("<a style=\"background-color: #D0E5F5;\" class=\"button redHover buttonRemoveAll\" onclick=opt" + (lValidCounter) + ".transferAllLeft() type=\"button\" title=\"" + _MessageHelper.GetMessage("btn remove all") + "\" name=\"left\"></a>" + Constants.vbCrLf);
                    if (nMetaTagType < MetaTagType_Searchable)
                    {
                        sb.Append("<input onClick=opt" + (lValidCounter) + ".newinput() type=\"button\" value=\"&gt;&gt;\" name=\"newinput\"/>" + Constants.vbCrLf);
                    }
                    sb.Append("</td>" + Constants.vbCrLf);
                    sb.Append("<td width=\"40%\">");
                    sb.Append("<select ondblclick=opt" + (lValidCounter) + ".transferLeft() multiple size=10 name=list2_" + (lValidCounter) + " style=\"width:200px\">" + Constants.vbCrLf);
                    //If (ty = "update") Then
                    for (i_option = 0; i_option <= Information.UBound(arrMetaText, 1); i_option++)
                    {
                        toption = Strings.Trim(arrMetaText.GetValue(i_option).ToString());
                        if (!string.IsNullOrEmpty(toption))
                        {
                            sb.Append("<option value=\"" + toption + "\">" + toption + "</option>" + Constants.vbCrLf);
                        }
                    }
                    //End If

                    sb.Append("</select>" + Constants.vbCrLf);
                    sb.Append("<scr" + "ipt type='text/javascript'>" + Constants.vbCrLf);
                    sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                    sb.Append("{" + Constants.vbCrLf);
                    sb.Append(" var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append(" var objForm = document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append(" if(objRemoteForm && objForm)" + Constants.vbCrLf);
                    sb.Append(" {" + Constants.vbCrLf);
                    sb.Append("   if(objForm.newLeft" + (lValidCounter) + ".value != \"\" && objForm." + strName + ".value != \"\")" + Constants.vbCrLf);
                    sb.Append("   {" + Constants.vbCrLf);
                    sb.Append("     var wordArray = new Array;" + Constants.vbCrLf);
                    sb.Append("     wordArray = objForm." + strName + ".value.split(objForm.delimiter.value);" + Constants.vbCrLf);
                    sb.Append("     var path = objForm.list2_" + (lValidCounter) + ".options;" + Constants.vbCrLf);
                    sb.Append("     var j = path.length;" + Constants.vbCrLf);
                    sb.Append("     for(var i=0; i < wordArray.length; i++)" + Constants.vbCrLf);
                    sb.Append("     {" + Constants.vbCrLf);
                    sb.Append("       path[j + i]=new Option(wordArray[i]);" + Constants.vbCrLf);
                    sb.Append("     }" + Constants.vbCrLf);
                    sb.Append("   }" + Constants.vbCrLf);
                    sb.Append(" }" + Constants.vbCrLf);
                    sb.Append("}" + Constants.vbCrLf);
                    sb.Append("</scr" + "ipt>" + Constants.vbCrLf);
                    sb.Append("</td>" + Constants.vbCrLf);
                    sb.Append("</tr>" + Constants.vbCrLf);
                    sb.Append("</tbody>" + Constants.vbCrLf);
                    sb.Append("</table>" + Constants.vbCrLf);
                    sb.Append("</div>");

                    sb.Append("<scr" + "ipt  type=\"text/JavaScript\">" + Constants.vbCrLf);
                    sb.Append("opt" + (lValidCounter) + ".init(document.forms[0]);" + Constants.vbCrLf);
                    sb.Append("</scr" + "ipt>" + Constants.vbCrLf);
                }
                else
                {
                    sb.Append("<select name=\"" + strSelName + "\" id=\"" + strSelName + "\" onChange=\"JavaScript:outputSelected(document.forms[0]." + strSelName + ".options,'" + strName + "','" + strMetaSeparator + "');\">" + Constants.vbCrLf);
                    sb.Append("<option value=\"\">(No Selection)</option>" + Constants.vbCrLf);
                    for (i_option = 0; i_option <= Information.UBound(arrMetaOptions, 1); i_option++)
                    {
                        toption = Strings.Trim(arrMetaOptions.GetValue(i_option).ToString());
                        if (!string.IsNullOrEmpty(toption))
                        {
                            sb.Append("<option value=\"" + toption + "\"");
                            if ((Testselection(arrMetaText, toption)))
                            {
                                sb.Append(" selected");
                            }
                            sb.Append(">" + toption + "</option>" + Constants.vbCrLf);
                        }
                    }
                    sb.Append("</select>" + Constants.vbCrLf);
                    sb.Append("<input type=\"hidden\" name=\"" + strName + "\" value=\"" + EkFunctions.HtmlEncode(strMetaValue) + "\">" + Constants.vbCrLf);
                    sb.Append("<scr" + "ipt  type=\"text/javascript\">" + Constants.vbCrLf);
                    sb.Append("<!--" + Constants.vbCrLf);
                    sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                    sb.Append("{" + Constants.vbCrLf);
                    sb.Append("var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append("var objForm = document.LibraryItem;" + Constants.vbCrLf);
                    sb.Append("if(objRemoteForm && objForm)" + Constants.vbCrLf);
                    sb.Append("{" + Constants.vbCrLf);
                    sb.Append("objForm." + strName + ".value = objRemoteForm." + strName + ".value;" + Constants.vbCrLf);
                    sb.Append("objForm." + strSelName + ".selectedIndex = objRemoteForm." + strSelName + ".value;" + Constants.vbCrLf);
                    sb.Append("}" + Constants.vbCrLf);
                    sb.Append("}" + Constants.vbCrLf);
                    sb.Append("// -->" + Constants.vbCrLf);
                    sb.Append("</scr" + "ipt>" + Constants.vbCrLf);
                }

                if (bIsDefault)
                {
                    sb.Append("<br /><br /><a class=\"button buttonInline greenHover buttonUpdate\" style=\"margin:0;\" href=\"#\" id=\"frm_metadefault_" + nMetaTagType + "_" + DataType + "_update\" disabled onclick=\"UpdateSelectList(this, '" + strSelName + "'); return false;\">Update list</a><br /><br />" + Constants.vbCrLf);
                }

            }
            else if (((NUMBER_PROP == DataType) || (BYTE_PROP == DataType) || (DOUBLE_PROP == DataType) || (FLOAT_PROP == DataType) || (INTEGER_PROP == DataType) || (LONG_PROP == DataType) || (SHORT_PROP == DataType)))
            {
                sb.Append("<input type=\"hidden\" name=\"" + strSelName + "\" id=\"" + strSelName + "\" value=\"\"/>" + Constants.vbCrLf);
                sb.Append("<input type=\"text\" name=\"" + strName + "\" id=\"" + strName + "\" size=\"10\" maxlength=\"500\" style=\"text-align:right;\" value=\"" + EkFunctions.HtmlEncode(strMetaValue) + "\"/>&nbsp; &nbsp;" + Constants.vbCrLf);
                if (!bIsDefault)
                {
                    sb.Append(WriteDefaultButton(strName));
                }
                sb.Append("<scr" + "ipt  type=\"text/javascript\">" + Constants.vbCrLf);
                sb.Append("<!--" + Constants.vbCrLf);
                sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("var objForm = document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("if(objRemoteForm && objForm)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("objForm." + strName + ".value = objRemoteForm." + strName + ".value;" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("// -->" + Constants.vbCrLf);
                sb.Append("</scr" + "ipt>" + Constants.vbCrLf);

            }
            else if (DATE_PROP == DataType)
            {
                EkDTSelector dso = default(EkDTSelector);
                dso = _RefAPI.EkDTSelectorRef;
                dso.formName = "metadefinition";
                if ((!string.IsNullOrEmpty(strMetaValue)))
                {
                    dso.targetDate = Convert.ToDateTime(strMetaValue);
                }
                sb.Append(dso.displayCultureDate(true, strName + "default_span", strName));
                sb.Append("<scr" + "ipt  type=\"text/javascript\">" + Constants.vbCrLf);
                sb.Append("<!--" + Constants.vbCrLf);
                sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("var objForm = document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("if(objRemoteForm && objForm)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("objForm." + strName + ".value = objRemoteForm." + strName + ".value;" + Constants.vbCrLf);
                sb.Append("objForm." + strSelName + ".value = objRemoteForm." + strSelName + ".value;" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("// -->" + Constants.vbCrLf);
                sb.Append("</scr" + "ipt>" + Constants.vbCrLf);
            }
            else
            {
                sb.Append("<input type=\"hidden\" name=\"" + strSelName + "\" id=\"" + strSelName + "\" value=\"\"/>" + Constants.vbCrLf);
                sb.Append("<textarea name=\"" + strName + "\" id=\"" + strName + "\" wrap=\"soft\" style=\"width:100%\" ");
                sb.Append("onclick=\"UpdateTextCounter('" + strName + "', 2000)\" onkeyup=\"UpdateTextCounter('" + strName + "', 2000)\" onchange=\"UpdateTextCounter('" + strName + "', 2000)\" ");
                sb.Append(">" + EkFunctions.HtmlEncode(strMetaValue) + "</textarea><br />" + Constants.vbCrLf);
                if (!bIsDefault)
                {
                    sb.Append(WriteDefaultButton(strName));
                    sb.Append("&nbsp;&nbsp;");
                }
                sb.Append("<div class=\"ektronCaption\">" + _MessageHelper.GetMessage("lbl current character count") + " &nbsp;<span id=\"" + strName + "_len\">0</span>&nbsp; (2000 " + _MessageHelper.GetMessage("abbreviation for maximum") + ")</div>");
                if (MetaTagType_Meta == nMetaTagType)
                {
                    sb.Append("&nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append(_MessageHelper.GetMessage("generic Separator Character") + " \"" + strMetaSeparator + "\"");
                }
                sb.Append("<br /><br />" + Constants.vbCrLf);
                sb.Append("<scr" + "ipt  type=\"text/javascript\">" + Constants.vbCrLf);
                sb.Append("<!--" + Constants.vbCrLf);
                sb.Append("if(top.mediauploader)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("var objRemoteForm = top.mediauploader.document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("var objForm = document.LibraryItem;" + Constants.vbCrLf);
                sb.Append("if(objRemoteForm && objForm)" + Constants.vbCrLf);
                sb.Append("{" + Constants.vbCrLf);
                sb.Append("objForm." + strName + ".value = objRemoteForm." + strName + ".value;" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("}" + Constants.vbCrLf);
                sb.Append("// -->" + Constants.vbCrLf);
                sb.Append("</scr" + "ipt>" + Constants.vbCrLf);
                sb.Append("<scr" + "ipt  type=\"text/javascript\">" + Constants.vbCrLf);
                sb.Append("<!--" + Constants.vbCrLf);
                sb.Append("UpdateTextCounter('" + strName + "', 2000);" + Constants.vbCrLf);
                sb.Append("// -->" + Constants.vbCrLf);
                sb.Append("</scr" + "ipt>" + Constants.vbCrLf);
            }
            // End of SelectableOnly

            if (((bIsDefault) & (nMetaTypeID != 0)))
            {
                sb.Append("<input type=\"hidden\" name=\"needed_validation\" value=\"" + strName + "," + DataType + "\">" + Constants.vbCrLf);
            }

            sb.Append("</td>" + Constants.vbCrLf);
        }
        else
        {
            sb.Append("<td>" + WriteMetadataValue(DataType, strMetaValue) + "</td>" + Constants.vbCrLf);
        }
        sb.Append("</tr>");

        return (sb.ToString());
    }

    public object WriteDefaultButton(object id)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("<a href=\"#\" onclick=\"SetDefault('" + id + "');return false;\"><img ");
        sb.Append("src=\"" + _MessageHelper.GetMessage("ImagePath") + "btn_default.gif\" ");
        sb.Append("alt=\"" + _MessageHelper.GetMessage("alt default button text") + "\" title=\"" + _MessageHelper.GetMessage("alt default button text") + "\"></a>");

        return (sb.ToString());

    }

    public bool Testselection(Array arrofdata, object MetaOptions)
    {
        int i = 0;
        bool match = false;
        for (i = 0; i <= Information.UBound(arrofdata, 1); i++)
        {
            if ((Strings.Trim(arrofdata.GetValue(i).ToString()) == (string)MetaOptions))
            {
                match = true;
            }
        }
        return match;
    }

    public object WriteMetadataForEdit(Collection cMetadataTypes, bool bSearchableOnly, object ty)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string lValidCounter = "";
        int nNumCustomSearchProperties = 0;
        object strReqFields = null;
        string firstField = "";
        string strValidateFields = "";
        string firstValid = "";

        strValidateFields = "";
        strReqFields = "";
        firstField = "true";
        firstValid = "true";
        nNumCustomSearchProperties = 0;
        if ((cMetadataTypes.Count > 0))
        {
            foreach (Collection cMetadataType in cMetadataTypes)
            {

                if (Convert.ToInt32(cMetadataType["MetaTagType"]) >= MetaTagType_Searchable && Convert.ToBoolean(cMetadataType["MetaEditable"]))
                {
                    nNumCustomSearchProperties = nNumCustomSearchProperties + 1;
                }
            }
        }
        sb.Append("<sc" + "ript type=\"text/JavaScript\" ");
        sb.Append("type=\"text/javascript\" src=\"java/optiontransfer.js\"></scr" + "ipt>");

        if (!bSearchableOnly)
        {
            // A fieldset is needed to properly stack the regions for some unknown reason,
            // otherwise the search properties display to the right of the non-search properties.
            sb.Append("<fieldset>");
            sb.Append("<table width=\"100%\">");
            sb.Append("<col width=\"5%\"><col width=\"95%\">");
            foreach (Collection cMetadataType in cMetadataTypes)
            {

                if (Convert.ToInt32(cMetadataType["MetaTagType"]) < MetaTagType_Searchable && Convert.ToBoolean(cMetadataType["MetaEditable"]))
                {
                    lValidCounter = lValidCounter + 1;
                    sb.Append(WriteMetadataTypeForEdit(cMetadataType, lValidCounter, ty));
                    if (Convert.ToBoolean(cMetadataType["MetaRequired"]))
                    {
                        if ((firstField == "true"))
                        {
                            strReqFields = "frm_text_" + lValidCounter;
                            firstField = "false";
                        }
                        else
                        {
                            strReqFields = strReqFields + ",frm_text_" + lValidCounter;
                        }
                    }
                    if ((firstValid == "true"))
                    {
                        strValidateFields = "frm_text_" + lValidCounter + "," + cMetadataType["MetaNameTitle"];
                        firstValid = "false";
                    }
                    else
                    {
                        strValidateFields = strValidateFields + ":frm_text_" + lValidCounter + "," + cMetadataType["MetaNameTitle"];
                    }
                }
            }
            // not MetaTagType_Searchable
            sb.Append("</table></fieldset>");
        }
        if (nNumCustomSearchProperties > 0)
        {
            sb.Append("<fieldset><legend>Search Data</legend>");
            sb.Append("<table width=\"100%\">");
            sb.Append("<col width=\"5%\"><col width=\"95%\">");
            foreach (Collection cMetadataType in cMetadataTypes)
            {
                if (Convert.ToInt32(cMetadataType["MetaTagType"]) >= MetaTagType_Searchable && Convert.ToBoolean(cMetadataType["MetaEditable"]))
                {
                    lValidCounter = lValidCounter + 1;
                    sb.Append(WriteMetadataTypeForEdit(cMetadataType, lValidCounter, ty));
                    if (Convert.ToBoolean(cMetadataType["MetaRequired"]))
                    {
                        if ((firstField == "true"))
                        {
                            strReqFields = "frm_text_" + lValidCounter;
                            firstField = "false";
                        }
                        else
                        {
                            strReqFields = strReqFields + ",frm_text_" + lValidCounter;
                        }
                    }
                    if ((firstValid == "true"))
                    {
                        strValidateFields = "frm_text_" + lValidCounter + "," + cMetadataType["MetaNameTitle"];
                        firstValid = "false";
                    }
                    else
                    {
                        strValidateFields = strValidateFields + ":frm_text_" + lValidCounter + "," + cMetadataType["MetaNameTitle"];
                    }
                }
            }
            sb.Append("</table>");
            sb.Append("</fieldset>");
        }
        if (Strings.Len(strReqFields) > 0)
        {
            sb.Append("<table width=\"100%\">");
            sb.Append("<tr>");
            sb.Append("<td width=\"20%\" nowrap=\"true\" align=\"right\">");
            sb.Append("<font color=\"red\"><b>" + _MessageHelper.GetMessage("explanation of asterisk") + "</b></font>");
            sb.Append("</td>");
            sb.Append("<td></td>");
            sb.Append("</tr>");
            sb.Append("</table>");
        }
        sb.Append("<input type=\"hidden\" name=\"frm_validcounter\" value=\"" + (lValidCounter) + "\">");
        sb.Append("<input type=\"hidden\" name=\"req_fields\" value=\"" + (strReqFields) + "\">");
        sb.Append("<input type=\"hidden\" name=\"needed_validation\" value=\"" + (strValidateFields) + "\">");
        sb.Append("<scr" + "ipt language=\"javascript\">");
        sb.Append("var nLimit, temp;");
        sb.Append("var temp = \"" + (lValidCounter) + "\";");
        sb.Append("if (temp == \"\") {");
        sb.Append("nLimit = 0;");
        sb.Append("}");
        sb.Append("else {");
        sb.Append("nLimit = parseInt(temp, 10);");
        sb.Append("if (isNaN(nLimit)) {");
        sb.Append("nLimit = 0;");
        sb.Append("}");
        sb.Append("}");
        sb.Append("if (nLimit && typeof document.forms[0].frm_text_1 != \"undefined\") {");
        sb.Append("if (document.forms[0].frm_text_1.type  != \"hidden\") {");
        sb.Append("try {");
        sb.Append("SetFullScreenView(false); //  Ensure that controls are not hidden.");
        sb.Append("document.forms[0].frm_text_1.focus();");
        sb.Append("}");
        sb.Append("catch(er) {");
        sb.Append("}");
        sb.Append("}");
        sb.Append("}");
        sb.Append("</scr" + "ipt>");

        return (sb.ToString());
    }

    public object WriteSearchEditProp(string DataType, bool Required, object Name, Collection Items, string Value, string DefaultValue)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        bool addTextInput = false;

        switch (DataType)
        {
            case TEXT_PROP:
                sb.Append("<textarea name=\"" + Name + "\" id=\"" + Name + "\" onkeyup=\"textCounter(this, document.forms[0]." + Name + "_len, 500)\" onkeydown=\"textCounter(this, document.forms[0]." + Name + "_len, 500)\" wrap=\"soft\" style=\"width:100%\">" + EkFunctions.HtmlEncode(Value) + "</textarea><br />");
                sb.Append("<input readonly type=\"text\" name=\"" + Name + "_len\" size=\"3\" maxlength=\"3\" value=\"500\">characters left");
                sb.Append("<scr" + "ipt language=\"javascript\">textCounter(document.forms[0]." + Name + ", document.forms[0]." + Name + "_len, 500)</scr" + "ipt>");
                if (Required)
                {
                    sb.Append("<span style=\"COLOR: red\">* " + _MessageHelper.GetMessage("metadata required msg") + "</span>");
                }
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    sb.Append("<br />");
                    sb.Append("<a href=\"#\" OnClick=\"SetDefault('" + Name + "');return false;\"><img src=\"" + _RefAPI.AppImgPath + "btn_default.gif\" alt=\"" + _MessageHelper.GetMessage("alt default button text") + "\" title=\"" + _MessageHelper.GetMessage("alt default button text") + "\"></a>");
                }
                break;
            case NUMBER_PROP:
                addTextInput = true;
                break;
            case BYTE_PROP:
                addTextInput = true;
                break;
            case DOUBLE_PROP:
                addTextInput = true;
                break;
            case FLOAT_PROP:
                addTextInput = true;
                break;
            case INTEGER_PROP:
                addTextInput = true;
                break;
            case LONG_PROP:
                addTextInput = true;
                break;
            case SHORT_PROP:
                addTextInput = true;
                break;
            case DATE_PROP:
                addTextInput = true;
                break;
            case SELECT_PROP:
            case SELECT1_PROP:
                sb.Append("<select name=\"" + Name + "\" " + (SELECT1_PROP == DataType ? " size=1" : " size=5") + " " + (SELECT1_PROP == DataType ? "" : " multiple") + ">");
                sb.Append("<option value=\"\">(No selection)</option>");
                if ((Items != null))
                {
                    foreach (string objItem in Items)
                    {
                        sb.Append("<option value=\"" + objItem + "\">" + objItem + "</option>");
                    }
                }
                sb.Append("</select>");
                break;
            case BOOLEAN_PROP:
                sb.Append("<input type=\"checkbox\" name=\"" + Name + "\">");
                break;
            default:
                break;
        }
        if ((addTextInput))
        {
            sb.Append("<input type=\"text\" name=\"" + Name + "\" value=\"" + Value + "\">");
        }
        return (sb.ToString());
    }

}

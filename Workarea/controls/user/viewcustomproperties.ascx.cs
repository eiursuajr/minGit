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
using Ektron.Cms.Core.CustomProperty;
using Ektron.Cms.Framework.Core;
using Ektron.Cms.Framework.Core.CustomProperty;
using Ektron.Cms.Common;

public partial class viewcustomproperties : System.Web.UI.UserControl
{
    protected CommonApi m_refComAPI = new CommonApi();
    private UserCustomPropertyData[] m_UCPData;
    private string PageAction = "ViewCustomProp";
    protected StyleHelper m_refStyle = new StyleHelper();
    protected EkMessageHelper m_refMsg;
    protected string AppImgPath = string.Empty;
    protected int ContentLanguage = -1;
    protected int EnableMultiLanguage = -1;
    protected CustomPropertyBL _coreCustomProperty = new CustomPropertyBL();


    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Ektron.Cms.User.EkUser EkUserObj = m_refComAPI.EkUserRef;
        string strOrder;
        bool bRet = false;
        //Test Read ALL
        //Dim AllProperty As UserCustomPropertyData()
        RegisterResources();
        m_refMsg = m_refComAPI.EkMsgRef;
        AppImgPath = m_refComAPI.AppImgPath;
        EnableMultiLanguage = m_refComAPI.EnableMultilingual;
        if (Request.QueryString["LangType"] != "")
        {
            ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
            m_refComAPI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        else
        {
            if (m_refComAPI.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = int.Parse(m_refComAPI.GetCookieValue("LastValidLanguageID"));
            }
        }

        if (ContentLanguage == Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES || ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
        {
            ContentLanguage = m_refComAPI.DefaultContentLanguage;
            m_refComAPI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
        }
        m_refComAPI.ContentLanguage = ContentLanguage;

        if (!(Request.QueryString["action"] == null))
        {
            if (Request.QueryString["action"] != "")
            {
                PageAction = Request.QueryString["action"];
            }
        }

        m_UCPData = EkUserObj.GetAllCustomProperty(""); //make sure to do a get before toolbar.
        if (PageAction.ToString().ToLower() == "reorderproperties")
        {
            if (IsPostBack)
            {
                strOrder = Request.Form["LinkOrder"];
                bRet = EkUserObj.UpdateCustomPropertiesItemOrder(strOrder);
                if (bRet)
                {
                    Response.Redirect("users.aspx?action=ViewCustomProp", false);
                }
            }
            else
            {
                Populate_ReOrder();
                ReOrderProperties_Toolbar();
            }
        }
        else
        {
            ViewCustomProperties_Toolbar();
            Populate_ViewGrid();
        }
    }
    private void Populate_ViewGrid()
    {
        int i;
        System.Web.UI.WebControls.BoundColumn colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "TITLE";
        colBound.HeaderText = m_refMsg.GetMessage("generic Title");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(70);
        colBound.ItemStyle.Width = Unit.Percentage(70);
        ViewAllGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "ID";
        colBound.HeaderText = m_refMsg.GetMessage("generic Id");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(5);
        colBound.ItemStyle.Width = Unit.Percentage(5);
        ViewAllGrid.Columns.Add(colBound);

        colBound = new System.Web.UI.WebControls.BoundColumn();
        colBound.DataField = "VALUE";
        colBound.HeaderText = m_refMsg.GetMessage("generic type");
        colBound.ItemStyle.Wrap = false;
        colBound.HeaderStyle.Width = Unit.Percentage(20);
        colBound.ItemStyle.Width = Unit.Percentage(20);
        ViewAllGrid.Columns.Add(colBound);

        //colBound = New System.Web.UI.WebControls.BoundColumn
        //colBound.DataField = "REQ"
        //colBound.HeaderText = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        //ViewAllGrid.Columns.Add(colBound)

        DataTable dt = new DataTable();
        DataRow dr;

        dt.Columns.Add(new DataColumn("TITLE", typeof(string)));
        dt.Columns.Add(new DataColumn("ID", typeof(string)));
        dt.Columns.Add(new DataColumn("VALUE", typeof(string)));
        dt.Columns.Add(new DataColumn("REQ", typeof(string)));

        if (!(m_UCPData == null))
        {
            for (i = 0; i <= m_UCPData.Length - 1; i++)
            {
                dr = dt.NewRow();
                dr[0] = "<a href=\"users.aspx?action=editcustomprop&id=" + m_UCPData[i].ID + "\">" + m_UCPData[i].Name + "</a>";
                dr[1] = m_UCPData[i].ID.ToString();
                dr[2] = "<a href=\"users.aspx?action=editcustomprop&id=" + m_UCPData[i].ID + "\">" + m_UCPData[i].PropertyValueType.ToString() + "</a>";
                //If (m_UCPData(i).Required) Then
                //    dr(3) = "<input type=""checkbox"" disabled checked=true />"
                //Else
                //    dr(3) = "<input type=""checkbox"" disabled />"
                //End If
                dt.Rows.Add(dr);
            }
        }
        DataView dv = new DataView(dt);
        ViewAllGrid.DataSource = dv;
        ViewAllGrid.DataBind();

    }
    private void ViewCustomProperties_Toolbar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props view"));
        result.Append("<table><tr>");
        if (this.m_refComAPI.ContentLanguage == this.m_refComAPI.DefaultContentLanguage)
        {
            result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/add.png", "users.aspx?action=addcustomprop", m_refMsg.GetMessage("alt add button text (user property)"), m_refMsg.GetMessage("btn add"), "", StyleHelper.AddButtonCssClass, true));
            if ((m_UCPData != null) && (m_UCPData.Length > 1))
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/arrowUpDown.png", "users.aspx?action=reorderproperties", m_refMsg.GetMessage("alt reorder items"), m_refMsg.GetMessage("btn reorder"), "", StyleHelper.ReOrderButtonCssClass));
            }
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
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>" + m_refMsg.GetMessage("lbl object type") + ":&nbsp;" + GetObjectTypeDropDown("SetObjectType(this); return false;"));

        //result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath & "../UI/Icons/back.png", "users.aspx?action=ViewUsersByGroup&LangType=" & ContentLanguage & "&FromUsers=" & Request.QueryString("FromUsers") & "&id=2&OrderBy=" & Request.QueryString("OrderBy"), m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), ""))
		result.Append(StyleHelper.ActionBarDivider); 
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("ViewCustomProperties", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    private void Populate_ReOrder()
    {
        litReOrder.Text = "";
        System.Text.StringBuilder sBuild = new System.Text.StringBuilder();
        string reOrderList = string.Empty;


        sBuild.Append("");
        sBuild.Append("<div class=\"ektronPageInfo\">");
        sBuild.Append("<table>");
        sBuild.Append("	<tr>");
        sBuild.Append("     <td>");
        sBuild.Append("         <select name=\"OrderList\" size=\"" + (m_UCPData.Length < 20 ? (m_UCPData.Length.ToString()) : "20") + "\">");
        reOrderList = "";
        foreach (UserCustomPropertyData data in m_UCPData)
        {
            if (reOrderList.Length > 0)
            {
                reOrderList = reOrderList + "," + data.ID.ToString();
            }
            else
            {
                reOrderList = data.ID.ToString();
            }
            sBuild.Append("         <option value=\"" + data.ID + "\">" + data.Name);
        }
        sBuild.Append("         </select>");
        sBuild.Append("     </td>");
        sBuild.Append("     <td>&nbsp;&nbsp;</td>");
        sBuild.Append("		<td>");
        sBuild.Append("         <a href=\"javascript:Move(\'up\', document.forms[0].OrderList, document.forms[0].LinkOrder)\"><img src=\"" + this.m_refComAPI.AppImgPath + "../UI/Icons/arrowHeadUp.png\" alt=\"" + m_refMsg.GetMessage("move selection up msg") + "\" title=\"" + m_refMsg.GetMessage("move selection up msg") + "\"></a>");
        sBuild.Append("         <br />");
        sBuild.Append("			<a href=\"javascript:Move(\'dn\', document.forms[0].OrderList, document.forms[0].LinkOrder)\"><img src=\"" + this.m_refComAPI.AppImgPath + "../UI/Icons/arrowHeadDown.png\" alt=\"" + m_refMsg.GetMessage("move selection down msg") + "\" title=\"" + m_refMsg.GetMessage("move selection down msg") + "\"></a>");
        sBuild.Append("		</td>");
        sBuild.Append("	</tr>");
        sBuild.Append("</table>");
        sBuild.Append("</div>");

        if (reOrderList.Length > 0)
        {
            sBuild.Append("<script language=\"javascript\">");
            sBuild.Append("document.forms[0].OrderList[0].selected = true;");
            sBuild.Append("</script>");
        }
        sBuild.Append("		<input type=\"hidden\" name=\"LinkOrder\" value=\"" + reOrderList + "\">");
        litReOrder.Text = sBuild.ToString();
    }
    private void ReOrderProperties_Toolbar()
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        txtTitleBar.InnerHtml = m_refStyle.GetTitleBar(m_refMsg.GetMessage("user custom props reorder"));
        result.Append("<table><tr>");
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/back.png", "users.aspx?action=viewcustomprop", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		result.Append(m_refStyle.GetButtonEventsWCaption(AppImgPath + "../UI/Icons/save.png", "#", m_refMsg.GetMessage("alt update button text"), m_refMsg.GetMessage("btn update"), "Onclick=\"javascript:return SubmitForm(\'userinfo\', \'true\');\"", StyleHelper.SaveButtonCssClass, true));
        result.Append(StyleHelper.ActionBarDivider);
		result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton("ReorderCustomProperties", ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        htmToolBar.InnerHtml = result.ToString();
    }
    #region GridItemBound
    protected void Grid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (PageAction.ToString().ToLower() == "viewcustomprop")
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    if (e.Item.Cells[1].Text.Equals("REMOVE-ITEM") || e.Item.Cells[1].Text.Equals("important") || e.Item.Cells[1].Text.Equals("input-box-text"))
                    {
                        e.Item.Cells[0].Attributes.Add("align", "Left");
                        e.Item.Cells[0].ColumnSpan = 2;
                        if (e.Item.Cells[0].Text.Equals("REMOVE-ITEM"))
                        {
                            //e.Item.Cells(0).CssClass = ""
                        }
                        else
                        {
                            e.Item.Cells[0].CssClass = e.Item.Cells[1].Text;
                        }
                        e.Item.Cells.RemoveAt(1);
                    }
                    break;
            }
        }
    }
    #endregion
    private void RegisterResources()
    {
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
    }
    public string GetObjectTypeDropDown(string OnChangeEvt)
    {
        StringBuilder result = new StringBuilder();
        List<Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType> _comObjectType = _coreCustomProperty.GetObjectTypeList();

        try
        {

            List<Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType> _objectType = _coreCustomProperty.GetObjectTypeList();

            result.Append("<select name=\'objectType\' id=\'objectType\' onchange=\"" + OnChangeEvt + "\">");
            result.Append("<option value=\'0\' selected=\'selected\'>");
            result.Append("User");
            result.Append("</option>");

            // Right now there is only one CustomPropertyObjectType that is TaxonomyNode,
            //Need to implement objectType in the future as new ones are added.

            result.Append(" <option value=\'1\' >");
            result.Append(GetObjectTypeString(Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType.TaxonomyNode));
            result.Append("</option>");

            //If _comObjectType IsNot Nothing Then
            //    Dim objectTypeValue As Ektron.Cms.Common.EkEnumeration.CustomPropertyObjectType() = _objectType.ToArray()
            //    For iObj As Integer = 0 To _objectType.Count - 1
            //        result.Append("<option value=""" & iObj + 1 & """")
            //        result.Append(">")
            //        result.Append(_objectType.Item(iObj))
            //        result.Append("</option>")
            //    Next
            //End If
            result.Append("</select>");
        }
        catch (Exception ex)
        {
            EkException.WriteToEventLog((string)(("CMS400: " + ex.Message + ":") + ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            result.Length = 0;
        }

        return (result.ToString());
    }
    private string GetObjectTypeString(EkEnumeration.CustomPropertyObjectType objectType)
    {
        return m_refMsg.GetMessage((string)("CmsObjectType" + objectType.ToString()));
    }
}

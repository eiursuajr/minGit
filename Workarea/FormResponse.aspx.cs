//#define SaveXmlAsFile
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





	public partial class formresponse : System.Web.UI.Page
	{
		
		
		
		protected long FormId;
		protected long CurrentUserId;
		protected string StartDate;
		protected string EndDate;
		protected Collection gtForm;
		protected Collection gtForms;
		protected StyleHelper m_refStyle = new StyleHelper();
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected string DefaultFormTitle = "";
		protected string DisplayType = "";
		protected string sFormDataIds = "";
		protected string DataType = "";
		protected string AppImgPath = "";
		protected int ContentLanguage = -1;
		protected string Flag = "false";
		protected PermissionData Security_info;
		protected string Action = "";
		protected string ResultType = "";
		protected int EnableMultilingual = 0;
        protected Ektron.Cms.Modules.EkModule objForm;
		protected string strFolderID;

		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Dim FormFieldStats As Collection
			Collection FormStats = new Collection();
			//Dim item As Collection
			string Target;
			
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;

            StyleSheetJS.Text = m_refStyle.GetClientScript();
		
			//Put user code to initialize the page here
			ContentLanguage = m_refContentApi.ContentLanguage;
			AppImgPath = m_refContentApi.AppImgPath;
			FormId = Convert.ToInt64(Request.QueryString["id"]);
			DisplayType = Request.QueryString["display_type"];
			Target = Request.QueryString["display_target"];
			
			if (Request.QueryString["LangType"] != "")
			{
				ContentLanguage = Convert.ToInt32( Request.QueryString["LangType"]);
				m_refContentApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
			}
			else
			{
				if (m_refContentApi.GetCookieValue("LastValidLanguageID") != "")
				{
					ContentLanguage = int.Parse(m_refContentApi.GetCookieValue("LastValidLanguageID"));
				}
			}
			
			EnableMultilingual = m_refContentApi.EnableMultilingual;
			m_refContentApi.ContentLanguage = ContentLanguage;
			
			Security_info = m_refContentApi.LoadPermissions(FormId, "content", 0);
			objForm = m_refContentApi.EkModuleRef;
			
			if ((Convert.ToString(FormId) != "") && (FormId > 0))
			{
				DefaultFormTitle = objForm.GetFormTitleById(FormId);
			}
			else
			{
				DefaultFormTitle = Request.QueryString["FormTitle"];
			}
			
			int i;
         
			string strFieldNames = "";
			string strFieldOptionNames = "";
			string strFieldOptionValues = "";
			ArrayList cResult = new ArrayList();
			ArrayList cItem;
			Hashtable hshQuestions;
			long llResponses;
			cResult = m_refContentApi.GetFormDataHistogramById((int) FormId);
			//llResponses = CInt(cResult.Item(0)(0).ToString.Substring(cResult.Item(0)(0).ToString.IndexOf("responses") + 9))
			llResponses = m_refContentApi.EkModuleRef.GetFormSubmissionsByFormId(FormId);
			hshQuestions = m_refContentApi.GetFormFieldQuestionsById((int) FormId);
			
			if (DisplayType == "1")
			{
				FormReportGrid.Visible = true;
				BoundColumn colBound;
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "Names";
				colBound.HeaderText = "";
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.ItemStyle.Wrap = false;
				FormReportGrid.Columns.Add(colBound);
				
				colBound = new System.Web.UI.WebControls.BoundColumn();
				colBound.DataField = "Percent";
				colBound.HeaderText = "";
				colBound.HeaderStyle.CssClass = "title-header";
				colBound.ItemStyle.Wrap = false;
				colBound.ItemStyle.VerticalAlign = VerticalAlign.Top;
				colBound.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
				FormReportGrid.Columns.Add(colBound);
				
				FormReportGrid.BorderColor = System.Drawing.Color.White;
				
				DataTable dt = new DataTable();
				//Dim dr As DataRow
				dt.Columns.Add(new DataColumn("Names", typeof(string)));
				dt.Columns.Add(new DataColumn("Percent", typeof(string)));
				int j;
				
				
				// loop through the names and value pairs to populate the dr and add the dr to dt
				lblTbl.Visible = true;
				lblTbl.Text = "";
				lblTbl.Text = lblTbl.Text + "<Table>";
				lblTbl.Text = lblTbl.Text + "<tr><td align=center><B><U>Responses - " + llResponses.ToString() + "</U></B></td></tr>";
				lblTbl.Text = lblTbl.Text + "</table>";
				
				for (i = 0; i <= cResult.Count - 1; i++)
				{
                    //cItem = cResult[i];
                    cItem =(ArrayList) cResult[i];
					lblTbl.Text = lblTbl.Text + "<Table>";
					lblTbl.Text = lblTbl.Text + "<tr><td align=center><B><U>" + hshQuestions[cItem[0]] + "</U></B></td></tr>";
					lblTbl.Text = lblTbl.Text + "</table>";
					lblTbl.Text = lblTbl.Text + "<table>";
					for (j = 1; j <= cItem.Count - 1; j++)
					{
						lblTbl.Text = lblTbl.Text + "<tr><td>";				

                        lblTbl.Text = lblTbl.Text + "<b>" + (Convert.ToInt64((cItem[j].ToString().Substring(cItem[j].ToString().LastIndexOf(",") + 1))) * 100) / llResponses + "%" + "</b>&nbsp;&nbsp;&nbsp;" + cItem[j].ToString().Substring(0, System.Convert.ToInt32(cItem[j].ToString().LastIndexOf(",") - 5));
						lblTbl.Text = lblTbl.Text + "</td></tr>";
					}
					lblTbl.Text = lblTbl.Text + "</Table>";
				}
				
				//dt.Rows.Add(dr)
				//Dim dv As New DataView(dt)
				//FormReportGrid.DataSource = dv
				//FormReportGrid.DataBind()
			}
			else
			{
				chart.Visible = true;
				
				// Now we have the data get the values
				//For Each item In FormStats
				//strNames = "18-21,22-25,26-30,31-40,41-50,51-60,61-over:10k-20k,21k-30k,31k-40k:High School,Some College,Degree(Associates),Master,Doctoral,Professional"
				//'strNames = "18-21,22-25,26-30,31-40,41-50,51-60,61-over:10k-20k,21k-30k,31k-40k"
				//strStale = "10,30,25,10,5,5,15:10,50,40:10,10,10,10,10,10"
				//strFieldNames = "Age range:Annual Income:Education level"
				strFieldNames = "";
				strFieldOptionNames = "";
				strFieldOptionValues = "";
				//EktComma is used to retain the commas in the fields and field option names
				int idx;
				int j;
				//Dim scaleFactor As Double
				
				for (idx = 0; idx <= cResult.Count - 1; idx++)
				{
					cItem =(ArrayList) cResult[idx];
					if (cItem.Count > 0)
					{
						strFieldNames = strFieldNames + ":" + hshQuestions[cItem[0].ToString().Replace(",", "EktComma")];
					}
				}
				
				strFieldNames = EkFunctions.UrlEncode(strFieldNames.Substring(1, strFieldNames.Length - 1));
				
				cItem = null;
				//For Each cItem In cResult
				
				for (idx = 0; idx <= cResult.Count - 1; idx++)
				{
                    cItem = (ArrayList)cResult[idx];
					for (j = 1; j <= cItem.Count - 1; j++)
					{
						strFieldOptionNames = strFieldOptionNames + cItem[j].ToString().Substring(0, System.Convert.ToInt32(cItem[j].ToString().LastIndexOf(",") - 5)).Replace(",", "EktComma") + ",";

                        strFieldOptionValues = strFieldOptionValues + (Convert.ToInt64((cItem[j].ToString().Substring(cItem[j].ToString().LastIndexOf(",") + 1))) * 100) / llResponses + ",";
					}
					
					strFieldOptionNames = (string) (strFieldOptionNames.Substring(0, strFieldOptionNames.Length - 1) + ":");
					strFieldOptionValues = (string) (strFieldOptionValues.Substring(0, strFieldOptionValues.Length - 1) + ":");
				}
				
				strFieldOptionNames = EkFunctions.UrlEncode(strFieldOptionNames.Substring(0, strFieldOptionNames.Length - 1));
				strFieldOptionValues = strFieldOptionValues.Substring(0, strFieldOptionValues.Length - 1);
				
				if (DisplayType == "2")
				{
					DisplayType = "0"; //Horizontal bar chart in chart.aspx
				}
				
				bool showAxis = false;
				chart.ImageUrl = "chart.aspx?fieldOptionNames=" + strFieldOptionNames + "&FieldNames=" + strFieldNames + "&FormValues=" + strFieldOptionValues + "&form_page=form_page&grpdisplay=" + DisplayType + "&responses=" + llResponses + "&showAxis=" + showAxis; //& Request.QueryString("report_display")
				//Next
			}
			
			
			
			
		}
		
	}
	


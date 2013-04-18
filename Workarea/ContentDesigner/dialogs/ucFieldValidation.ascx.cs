using System;
using Telerik.WebControls;
using System.Text;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
	/// Summary description for ucFieldValidation.
    /// </summary>
	public partial class ucFieldValidation : System.Web.UI.UserControl
    {
		private Ektron.Cms.CommonApi _api = null;

		public Ektron.ContentDesigner.Dialogs.ucFieldTreeView validationTree { get { return validationTreeControl; } }
		protected string ValidateSpecXml = "../../ContentDesigner/ValidateSpec.xml";

		protected void cboV8n_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
		{
			string xslt = new Uri(Request.Url, "../../ContentDesigner/ValidateSpecToValidationCombo.xslt").AbsoluteUri;
			LoadComboBox((RadComboBox)o, e.Text, xslt);
		}
		protected void cboDataType_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
		{
			string xslt = new Uri(Request.Url, "../../ContentDesigner/ValidateSpecToDatatypeCombo.xslt").AbsoluteUri;
			LoadComboBox((RadComboBox)o, e.Text, xslt);
		}
		protected void cboValEx_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
		{
			string xslt = new Uri(Request.Url, "../../ContentDesigner/ValidateSpecToExamplesCombo.xslt").AbsoluteUri;
			LoadComboBox((RadComboBox)o, e.Text, xslt);
		}
		private void LoadComboBox(RadComboBox combo, string validationName, string xsltPath)
		{
			string xmlPath = new Uri(Request.Url, ValidateSpecXml).AbsoluteUri;
			Ektron.Cms.Xslt.ArgumentList objXsltArgs = new Ektron.Cms.Xslt.ArgumentList();
			objXsltArgs.AddParam("validationName", "", validationName);
			objXsltArgs.AddParam("LangType", "", _api.UserLanguage);
            string comboItems = Ektron.Cms.EkXml.XSLTransform(xmlPath, xsltPath, true, true, objXsltArgs, true, null, _api.RequestInformationRef.ApplicationPath, false);

			combo.Items.Clear();
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			doc.LoadXml(comboItems);
			System.Xml.XmlElement root = doc.DocumentElement;
			for (int i = 0; i < root.ChildNodes.Count; i++)
			{
				String text = root.ChildNodes[i].Attributes.GetNamedItem("Text").Value;
				String data = root.ChildNodes[i].Attributes.GetNamedItem("Value").Value;
				if ("" == data) data = "{\"\"}";
				combo.Items.Add(new RadComboBoxItem(text, data));
			}
		}
		protected void Page_Load(Object src, EventArgs e)
		{
			_api = new Ektron.Cms.CommonApi();

			Ektron.Cms.API.JS.RegisterJS(this, "ucFieldValidation.js", "FieldValidationJS");
			Ektron.Cms.Common.EkMessageHelper refMsg = _api.EkMsgRef;
			this.lblCondition.InnerHtml = refMsg.GetMessage("lbl condition");
			this.lblCustomValidation.InnerHtml = refMsg.GetMessage("lbl custom validation");
			this.lblDataType.InnerHtml = refMsg.GetMessage("lbl data type");
			this.lblDataType.Attributes["for"] = cboDataType.ClientID;
			this.lblExamples.InnerHtml = refMsg.GetMessage("lbl examples c");
			this.lblExamples.Attributes["for"] = cboValEx.ClientID;
			this.lblMessage.InnerHtml = refMsg.GetMessage("lbl message");
			this.lblValidation.InnerHtml = refMsg.GetMessage("lbl validation");
			this.lblValidation.Attributes["for"] = cboV8n.ClientID;

			if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldValidationScript"))
			{
				string ScriptText = EkFieldValidationScript.InnerText;
				ScriptText = ScriptText.Replace("<%=this.ClientID%>", this.ClientID);
				ScriptText = ScriptText.Replace("<%=cboV8n.ClientID%>", cboV8n.ClientID);
				ScriptText = ScriptText.Replace("<%=cboDataType.ClientID%>", cboDataType.ClientID);
				ScriptText = ScriptText.Replace("<%=cboValEx.ClientID%>", cboValEx.ClientID);
				ScriptText = ScriptText.Replace("<%=validationTree.fsTree.ClientID%>", validationTreeControl.fsTree.ClientID);
				ScriptText = ScriptText.Replace("<%=validationTree.ClientID%>", validationTreeControl.ClientID);

				StringBuilder sbScript = new StringBuilder();
				sbScript.AppendLine();
				sbScript.AppendLine(@"var EkFieldValidationResourceText = ");
				sbScript.AppendLine(@"{");
				sbScript.Append(@"	sExprContainsVars: """);
				sbScript.Append(refMsg.GetMessage("msg val expr contains vars"));
				sbScript.AppendLine(@"""");
				sbScript.Append(@",	sCondition: """);
				sbScript.Append(refMsg.GetMessage("lbl condition nc"));
				sbScript.AppendLine(@"""");
				sbScript.AppendLine(@"};");
				sbScript.AppendLine(ScriptText);

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldValidationScript", sbScript.ToString(), true);
			}
			EkFieldValidationScript.Visible = false;
		}
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.cboV8n.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(this.cboV8n_ItemsRequested);
			this.cboDataType.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(this.cboDataType_ItemsRequested);
			this.cboValEx.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(this.cboValEx_ItemsRequested);
			this.Load += new System.EventHandler(this.Page_Load);
		}
    }
}

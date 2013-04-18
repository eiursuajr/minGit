using System;
using Telerik.WebControls;
using System.Text;

namespace Ektron.ContentDesigner.Dialogs
{
    /// <summary>
	/// Summary description for ucFieldTreeView.
    /// </summary>
	public partial class ucFieldTreeView : System.Web.UI.UserControl
    {
		public System.Web.UI.HtmlControls.HtmlControl fsTree { get { return fsTreeControl; } }
		public void AjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
		{
			string treeNodes = e.Argument;
			if (treeNodes.Length > 0)
			{
				treeNodes = treeNodes.Replace("\r\n", "");
				treeNodes = treeNodes.Replace("\t", "");
				if ("<tree" == treeNodes.Substring(0, 5).ToLower().ToString()) // in case of AjaxManager fail to load xml
				{
					RadTree1.LoadXmlString(treeNodes);
					RadTree1.ExpandAllNodes();
				}
			}
		}
		protected void Page_Load(Object src, EventArgs e)
		{
			Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
			this.UpdateLink.Text = refMsg.GetMessage("lbl insert field");
			if (this.ClientID.Contains("Calculation"))
			{
				this.lblSelectField.InnerHtml = refMsg.GetMessage("lbl select field for formula");
			}
			else
			{
				this.lblSelectField.InnerHtml = refMsg.GetMessage("lbl select field");
			}

			if (!Page.ClientScript.IsClientScriptBlockRegistered("EkFieldTreeViewScript"))
			{
				StringBuilder sbScript = new StringBuilder();
				sbScript.AppendLine();
				sbScript.AppendLine(@"var EkFieldTreeViewResourceText = ");
				sbScript.AppendLine(@"{");
				sbScript.Append(@"	sInsertField: """);
				sbScript.Append(refMsg.GetMessage("lbl insert field"));
				sbScript.AppendLine(@"""");
				sbScript.Append(@",	sReplace0in1: """);
				sbScript.Append(refMsg.GetMessage("lbl replace 0 in 1"));
				sbScript.AppendLine(@"""");
				sbScript.AppendLine(@"};");
				sbScript.AppendLine(EkFieldTreeViewScript.InnerText);
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldTreeViewScript", sbScript.ToString(), true);
			}
			string ScriptText = EkFieldTreeViewArray.InnerText;
			ScriptText = ScriptText.Replace("<%=this.ClientID%>", this.ClientID);
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EkFieldTreeViewArray" + this.ID, ScriptText, true);
			EkFieldTreeViewScript.Visible = false;
			EkFieldTreeViewArray.Visible = false;
			UpdateLink.NavigateUrl = "javascript:ekFieldTreeViewControl." + this.ClientID + ".updateTextField();";
			RadTree1.BeforeClientClick = "ekFieldTreeViewControl." + this.ClientID + ".treeNodeClick";
			RadTree1.BeforeClientDoubleClick = "ekFieldTreeViewControl." + this.ClientID + ".treeNodeDoubleClick";
		}

		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
    }
}

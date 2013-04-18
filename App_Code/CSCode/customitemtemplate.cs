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


namespace VBWebCode
{
	public class CustomItemTemplate : ITemplate
	{
		
		private string columnName;
		private TextBox editControl;
		public CustomItemTemplate(string colName)
		{
			columnName = colName;
		}
		public void InstantiateIn(Control container)
		{
			editControl = new TextBox();
			editControl.DataBinding += new System.EventHandler(BindDataCtrl);
			container.DataBinding += new System.EventHandler(BindDataCtrl);
			container.Controls.Add(editControl);
		}
		
		private void BindDataCtrl(object sender, EventArgs e)
		{
			DataGridItem container = (DataGridItem) editControl.NamingContainer;
			string str = (string) (((DataRowView) container.DataItem)[columnName].ToString());
			editControl.Text = str;
		}
	}
	
}

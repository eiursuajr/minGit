using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using System.Reflection;
using System.Text;

namespace Ektron.Cms.Common 
{

	public partial class Grid : UserControl
	{
        
        #region member variables

        private List<ColumnData> _columns = new List<ColumnData>();

        #endregion

        #region constructors

        public Grid()
        {

        }

        #endregion

        #region public properties

        public int CurrentPage
        {
            get { return uxGridPaging.SelectedPage; }
        }

        public void Bind(object items, PagingInfo pagingInfo)
        {
            if (pagingInfo.TotalPages > 1)
            {
                uxGridPaging.TotalPages = pagingInfo.TotalPages;
                uxGridPaging.CurrentPageIndex = pagingInfo.CurrentPage - 1;
            }
            else
            {
                uxGridPaging.Visible = false;
            }

            DataGridView.HeaderStyle.CssClass = "title-header";
            DataGridView.CssClass = "ektronGrid";
            DataGridView.DataSource = items;
            if (DataGridView.Columns.Count == 0)
                foreach (ColumnData column in _columns)
                    DataGridView.Columns.Add(CreateColumn(column));
            DataGridView.DataBind();
        }

        public void AddColumn(string headerText, string format)
        {
            _columns.Add(new ColumnData(headerText, format));
        }

        #endregion

        #region protected methods

        #endregion

        #region #formatting 

        private DataControlField CreateColumn(ColumnData columnData)
        {
            DataControlField field = new BoundField();
            field.HeaderText = columnData.Header;
            return field;
        }
        
        protected void GridView_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = 
                        FormatWithData(e.Row.DataItem, _columns[i].Format);
                }
                if (e.Row.RowIndex % 2 == 0)
                    e.Row.CssClass = "stripe";
            }

        }

        protected string GetValue(object item, string property)
        {
            PropertyInfo prop = item.GetType().GetProperty(property);
            return prop.GetValue(item, null).ToString();
        }

        protected string FormatWithData(object item, string format)
        {
            int startBracketLocation = format.IndexOf("[");
            while (startBracketLocation > -1)
            {
                int endBracketLocation = format.IndexOf("]", startBracketLocation);
                string properyName = format.Substring(startBracketLocation + 1, endBracketLocation - startBracketLocation - 1);
                format = format.Replace("[" + properyName + "]",
                    GetValue(item, properyName)
                    );
                startBracketLocation = format.IndexOf("[");
            }
            return format;
        }

        #endregion
    }

    public class ColumnData
    {
        public string Header { get; set; }
        public string Format { get; set; }

        public ColumnData(string header, string format)
        {
            this.Header = header;
            this.Format = format;
        }
    }
}

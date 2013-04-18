using System;
using System.Collections;
using System.Text;
using Ektron.Cms.Widget;
using Ektron.Cms.Common;

public partial class Workarea_Widgets_ForumTopicsAndPosts : WorkareaWidgetBaseControl, IWidget
{
    ArrayList _data;

    protected void Page_Load(object sender, EventArgs e)
    {
        _data = EkContentRef.GetModeratedTotals(0, true);
        if (_data.Count > 0)
        {
            LoadData();
        }
        else
        {
            lblNoRecords.Visible = true;
            string title = GetMessage("lbl forummod smrtdesk") + " (0)";
            SetTitle(title);
        }
        ltrlNoRecords.Text = GetMessage("lbl no topics replies awaiting moderation");
    }

    private void LoadData()
    {
        int itotal = 0;
        long boardID = 0;
        long forumID = 0;
        int alltotal = 0;

        StringBuilder sbForum = new StringBuilder();

        ArrayList alTmp = new ArrayList();
        for (int i = 0; i <= _data.Count - 1; i++)
        {
            alTmp = (ArrayList)_data[i];
            itotal = Convert.ToInt32(alTmp[0]);

            boardID = Convert.ToInt64(alTmp[1]);
            if (boardID != 0)
            {              
                sbForum.Append("<img src=\"images/UI/Icons/folderBoard.png\" style=\"vertical-align:middle\"> ");
                sbForum.Append(alTmp[2]);
                sbForum.Append("<br />");
                sbForum.Append(Environment.NewLine);
            }
            forumID = Convert.ToInt64(alTmp[3]);

            string foldercsvpath = EkContentRef.GetFolderParentFolderIdRecursive(forumID);

            if (forumID != 0)
            {
                //  a new forum
                sbForum.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                sbForum.Append("<img src=\"images/UI/Icons/folderBoard.png\" style=\"vertical-align:middle\"> ");
                sbForum.Append("<a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("content.aspx?action=ViewContentByCategory&id=" + forumID.ToString() + "&from=dashboard") + "', 'Content', '" + foldercsvpath + "')\">");
                sbForum.Append(alTmp[4]);
                sbForum.Append("</a>");
                sbForum.Append("<br />");
                sbForum.Append(Environment.NewLine);
            }
            sbForum.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            sbForum.Append("<img src=\"images/UI/Icons/folderBoard.png\" style=\"vertical-align:middle\">");
            sbForum.Append("&nbsp;");
            sbForum.Append("<a href=\"#\" onclick=\"top.showContentInWorkarea('" + EkFunctions.UrlEncode("content.aspx?id=" + forumID.ToString() + "&action=ViewContentByCategory&LangType=1033&ContType=13&contentid=" + alTmp[5].ToString() + "&from=dashboard") + "', 'Content', '" + foldercsvpath + "')\">");
            sbForum.Append(alTmp[6] + " " + (alTmp[7].ToString().ToUpper() == "I" ? "" : "(" + itotal.ToString() + ")"));
            sbForum.Append("</a>");
            sbForum.Append("<br />");
            sbForum.Append(Environment.NewLine);
            alltotal = (alltotal + itotal);
        }

        ltr_forum_mod.Text = sbForum.ToString();

        string title = GetMessage("lbl forummod smrtdesk") + " (" + alltotal.ToString() + ")";
        SetTitle(title);
    }
}

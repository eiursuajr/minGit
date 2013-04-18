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
using Ektron.Cms.Content;
using Ektron.Cms.Common;

public partial class ViewApprovalContent : System.Web.UI.UserControl
{

    private CommonApi m_refAPI = new CommonApi();
    private EkContent m_refContent;
    private ContentAPI m_refContentApi = new ContentAPI();
    private EkMessageHelper m_refMsg;
    protected StyleHelper m_refStyle = new StyleHelper();
    private Collection m_cCont;
    private Collection m_meObj;
    private Collection cApprovals;
    private bool m_TaskExists;
    private string m_sPage;
    private string toggle;
    protected int ContentLanguage = -1;
    private long CurrentUserId = 0;
    private ektUrlRewrite ekrw;
    protected string SitePath = "";
    // blog - SK
    private bool m_bIsBlog = false;
    private BlogPostData blog_post_data;
    private string[] arrBlogPostCategories;
    int i = 0;
    private long aprId = 0;
    //END: blog - SK

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        string fldid;
        m_refMsg = m_refAPI.EkMsgRef;
        m_sPage = Request.QueryString["page"];
        aprId = System.Convert.ToInt64(Request.QueryString["id"]);
        CurrentUserId = m_refAPI.UserId;
        SitePath = m_refAPI.SitePath;
        RegisterResources();

        if (!(Request.QueryString["LangType"] == null))
        {
            if (Request.QueryString["LangType"] != "")
            {
                ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
                m_refAPI.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
            }
            else
            {
                if (m_refAPI.GetCookieValue("LastValidLanguageID") != "")
                {
                    ContentLanguage = Convert.ToInt32(m_refAPI.GetCookieValue("LastValidLanguageID"));
                }
            }
        }
        else
        {
            if (m_refAPI.GetCookieValue("LastValidLanguageID") != "")
            {
                ContentLanguage = Convert.ToInt32(m_refAPI.GetCookieValue("LastValidLanguageID"));
            }
        }
        m_refAPI.ContentLanguage = ContentLanguage;
        m_refContent = m_refAPI.EkContentRef;

        if (Request.QueryString["content"] == "published")
        {
            m_cCont = m_refContent.GetContentByIDv2_0(aprId);
            toggle = "staged";
        }
        else
        {
            m_cCont = m_refContent.GetStagedContByIDv2_0(aprId);
            toggle = "published";
            if (m_cCont.Count == 0)
            {
                m_cCont = null;
                m_cCont = m_refContent.GetContentByIDv2_0(aprId);
                toggle = "staged";
                if (m_sPage == "workarea")
                {
                    Response.Redirect((string)("approval.aspx?action=viewApprovalList&id=" + Request.QueryString["fldid"]), false);
                }
                else
                {
                    Response.Write("<script language=\"Javascript\">" + "top.opener.location.reload(true);" + "top.close();" + "</script>");
                }
            }
        }
        m_meObj = m_refContent.CanIv2_0(aprId, "content");
        fldid = m_cCont["FolderID"].ToString();

        cApprovals = m_refContent.GetCurrentApprovalInfoByIDv1_1(aprId);

        m_TaskExists = m_refContent.DoesTaskExistForContent(Convert.ToInt64(m_cCont["ContentID"]));
        ViewContent();
    }

    private void ViewContent()
    {
        bool bCanAlias = false;

        ekrw = m_refAPI.EkUrlRewriteRef;
        ViewToolBar();
        System.Text.StringBuilder result = new System.Text.StringBuilder();
       

        blog_post_data = new BlogPostData();
        blog_post_data.Categories = (string[])Array.CreateInstance(typeof(string), 0);
        foreach ( Collection cApproval in (Collection)m_cCont["ContentMetadata"])
        {
            if (System.Convert.ToInt32(cApproval["ObjectType"]) > 0)
            {
                switch (System.Convert.ToInt32(cApproval["ObjectType"]))
                {
                    case (int)Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Categories:
                        string sTmp = cApproval["MetaText"].ToString();
                        sTmp = sTmp.Replace("&#39;", "\'");
                        sTmp = sTmp.Replace("&quot", "\"");
                        sTmp = sTmp.Replace("&gt;", ">");
                        sTmp = sTmp.Replace("&lt;", "<");
                        blog_post_data.Categories = sTmp.Split(';');
                        break;
                    case (int)Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Ping:
                        if (!(cApproval["MetaText"].ToString().Trim().ToLower() == "no"))
                        {
                            m_bIsBlog = true;
                        }
                        blog_post_data.Pingback = Ektron.Cms.Common.EkFunctions.GetBoolFromYesNo(cApproval["MetaText"].ToString());
                        break;
                    case (int)Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Tags:
                        blog_post_data.Tags = cApproval["MetaText"].ToString();
                        break;
                    case (int)Ektron.Cms.Common.EkEnumeration.BlogPostDataType.Trackback:
                        blog_post_data.TrackBackURL = cApproval["MetaText"].ToString();
                        break;
                }
            }
        }

        result.Append("<div class=\"tabContainerWrapper\">");
        result.Append("<div class=\"tabContainer\"><ul>");
        result.Append("<li><a href=\"#dvContent\">" + (m_refMsg.GetMessage("content text")) + "</a></li>");
        result.Append("<li><a href=\"#dvSummary\">" + (m_refMsg.GetMessage("Summary text")) + "</a></li>");

        if (true == bCanAlias)
        {
            result.Append("<li><a href=\"#dvAlias\">" + (m_refMsg.GetMessage("lbl alias")) + "</a></li>");
        }

        result.Append("<li><a href=\"#dvMetadata\">" + (m_refMsg.GetMessage("metadata text")) + "</a></li>");
        result.Append("<li><a href=\"#dvProperties\">" + (m_refMsg.GetMessage("properties text")) + "</a></li>");
        result.Append("<li><a href=\"#dvComment\">" + (m_refMsg.GetMessage("comment text")) + "</a></li>");

        //Taxonomy
        result.Append("<li><a href=\"#dvTaxonomy\">" + (m_refMsg.GetMessage("viewtaxonomytabtitle")) + "</a></li>");
        result.Append("</ul>");

        result.Append("<div id=\"dvProperties\">");
        result.Append("<table class=\"ektronGrid\">");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + (m_refMsg.GetMessage("content title label")) + "</td>");
        result.Append("<td>" + m_cCont["ContentTitle"] + "</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + (m_refMsg.GetMessage("content id label")) + "</td>");
        result.Append("<td>" + (m_cCont["ContentID"]) + "</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + (m_refMsg.GetMessage("content status label")) + "</td>");
        result.Append("<td>");
        if (m_cCont["ContentStatus"].ToString() == "S")
        {
            result.Append(m_refMsg.GetMessage("status:Submitted for Approval"));
        }
        else
        {
            result.Append(m_refMsg.GetMessage("status:Submitted for Deletion"));
        }
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + m_refMsg.GetMessage("submitted by label") + "</td>");
        result.Append("<td>");
        result.Append(m_cCont["EditorLName"] + ", " + m_cCont["EditorFName"]);
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + m_refMsg.GetMessage("content LED label") + "</td>");
        result.Append("<td>" + m_cCont["DisplayLastEditDate"] + "</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + m_refMsg.GetMessage("generic start date label") + "</td>");
        result.Append("<td>");
        if (m_cCont["DisplayGoLive"].ToString() != "")
        {
            result.Append(m_cCont["DisplayGoLive"]);
        }
        else
        {
            result.Append(m_refMsg.GetMessage("none specified msg"));
        }
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + m_refMsg.GetMessage("generic end date label") + "</td>");
        result.Append("<td>");
        if (m_cCont["DisplayEndDate"].ToString() != "")
        {
            result.Append(m_cCont["DisplayEndDate"]);
        }
        else
        {
            result.Append(m_refMsg.GetMessage("none specified msg"));
        }
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + (m_refMsg.GetMessage("content DC label")) + "</td>");
        result.Append("<td>" + (m_cCont["DisplayDateCreated"]) + "</td>");
        result.Append("</tr>");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + (m_refMsg.GetMessage("content approvals label")) + "</td>");
        result.Append("<td>");
        if (cApprovals.Count > 0)
        {
            foreach (Collection cApproval in cApprovals)
            {
                if (cApproval["ApproverType"].ToString().ToLower() == "user")
                {
                    result.Append("<img class=\"imgUsers\" src=\"" + m_refAPI.AppPath + "images/UI/Icons/user.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user") + "\" title=\"" + m_refMsg.GetMessage("approver is user") + "\"/>");
                }
                else
                {
                    result.Append("<img class=\"imgUsers\" src=\"" + m_refAPI.AppPath + "images/UI/Icons/users.png\" align=\"absbottom\" alt=\"" + m_refMsg.GetMessage("approver is user group") + "\" title=\"" + m_refMsg.GetMessage("approver is user group") + "\"/>");
                }
                if (Convert.ToBoolean(cApproval["CurrentApprover"]))
                {
                    result.Append("<font color=\"\"red\"\">");
                }
                else
                {
                    result.Append(" <font> ");
                }
                result.Append(cApproval["Name"]);
            }
        }
        else
        {
            result.Append(m_refMsg.GetMessage("none specified msg"));
        }
        result.Append("</td>");
        result.Append("</tr>");
        result.Append("</table>");
        result.Append("</div>");

        result.Append("<div id=\"dvMetadata\">");
        result.Append("<table class=\"ektronGrid\">");
        foreach (Collection cApproval in (Collection)m_cCont["ContentMetadata"])
        {
           if (!(System.Convert.ToInt32(cApproval["ObjectType"]) > 0))
            {
                result.Append("<tr>");
                result.Append("<td class=\"label\">" + (cApproval["MetaTypeName"]) + ":</td>");
                result.Append("<td>" + (cApproval["MetaText"]) + "</td>");
                result.Append("</tr>");
            }
        }
        result.Append("</table>");
        result.Append("</div>");

        if (true == bCanAlias)
        {
            string m_strAliasPageName = string.Empty;

            if (Request.QueryString["content"] == "published")
            {
                //Do nothing
            }
            else
            {
                m_strAliasPageName = m_cCont["ManualAlias"].ToString();
            }

            if (m_strAliasPageName != "")
            {
                m_strAliasPageName = SitePath + m_strAliasPageName;
            }
            else
            {
                m_strAliasPageName = " [Not Defined]";
            }

            result.Append("<DIV id=\"dvAlias\"");
            result.Append("	<TABLE class=\"ektronGrid\">");
            result.Append("<TR>");
            result.Append("<TD class=\"label\">" + m_refMsg.GetMessage("lbl aliased page") + ":\"</TD>");
            result.Append("<TD>" + m_strAliasPageName + "</TD>");
            result.Append("</TR>");
            result.Append("</TABLE>");
            result.Append("</DIV>");

        }

        result.Append("<div id=\"dvSummary\">");
        result.Append("<table class=\"ektronGrid\">");
        result.Append("<tr>");

        string strTeaser;
        int nContentType;
        strTeaser = m_cCont["ContentTeaser"].ToString();
        if (Information.IsNumeric(m_cCont["ContentType"]))
        {
            nContentType = System.Convert.ToInt32(m_cCont["ContentType"]);
        }
        else
        {
            nContentType = (int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Content; // default
        }
        if ((int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Forms == nContentType || (int)Ektron.Cms.Common.EkEnumeration.CMSContentType.Archive_Forms == nContentType)
        {
            if (strTeaser != null)
            {
                if (strTeaser.IndexOf("<ektdesignpackage_design") > -1)
                {
                    string strDesign;
                    strDesign = m_refAPI.XSLTransform("", "", true, false, null, true);
                    strTeaser = strDesign;
                }
            }
            else
            {
                strTeaser = "";
            }
        }

        result.Append("<td class=\"label\">");
        result.Append(m_refMsg.GetMessage("lbl teaser"));
        result.Append(":</td><td>");
        result.Append(strTeaser);
        result.Append("</td></tr>");

        if (m_bIsBlog)
        {
            result.Append("<tr><td class=\"label\">");
            result.Append(m_refMsg.GetMessage("lbl tags"));
            result.Append(":</td><td>");
            if (!(blog_post_data == null))
            {
                result.AppendLine(blog_post_data.Tags);
            }
            result.AppendLine("</td></tr>");

            result.Append("<tr><td class=\"label\">");
            result.Append(m_refMsg.GetMessage("categories text"));
            result.Append(":</td><td>");
            if (!(blog_post_data.Categories == null))
            {
                arrBlogPostCategories = blog_post_data.Categories;
                if (arrBlogPostCategories.Length > 0)
                {
                    Array.Sort(arrBlogPostCategories);
                }
            }
            else
            {
                arrBlogPostCategories = null;
            }
            if (blog_post_data.Categories.Length > 0)
            {
                for (i = 0; i <= (blog_post_data.Categories.Length - 1); i++)
                {
                    if (blog_post_data.Categories[i].ToString() != "")
                    {
                        result.AppendLine("				<input type=\"checkbox\" name=\"blogcategories" + i.ToString() + "\" value=\"" + blog_post_data.Categories[i].ToString() + "\" checked=\"true\" disabled>&nbsp;" + Strings.Replace((string)(blog_post_data.Categories[i].ToString()), "~@~@~", ";", 1, -1, 0) + "<br>");
                    }
                }
            }
            else
            {
                result.AppendLine("No categories defined.");
            }
            result.Append("</td></tr>");

            result.Append("<tr><td class=\"label\">");
            result.Append(m_refMsg.GetMessage("lbl trackback url"));
            result.Append(":</td><td>");
            result.AppendLine("<input type=\"hidden\" name=\"blogposttrackbackid\" id=\"blogposttrackbackid\" value=\"");
            if (!(blog_post_data == null))
            {
                result.Append(blog_post_data.TrackBackURLID.ToString());
            }
            result.Append("\" /><input type=\"hidden\" id=\"isblogpost\" name=\"isblogpost\" value=\"true\"/>");
            if (!(blog_post_data == null))
            {
                result.AppendLine("<input type=\"text\" size=\"75\" id=\"trackback\" name=\"trackback\" value=\"" + EkFunctions.HtmlEncode(blog_post_data.TrackBackURL) + "\" disabled/>");
            }

            result.Append("<tr><td class=\"label\">");
            result.Append(m_refMsg.GetMessage("lbl blog ae ping"));
            result.Append(":</td><td>");
            result.Append("<input type=\"checkbox\" name=\"pingback\" id=\"pingback\" ");
            if (!(blog_post_data == null))
            {
                if (blog_post_data.Pingback == true)
                {
                    result.Append("checked ");
                }
            }
            result.Append(" disabled/>");

            result.AppendLine("</td>");
            result.AppendLine("</tr>");
            result.AppendLine("</table>");
        }
        result.Append(" </td>");
        result.Append("</tr>");
        result.Append("</table>");
        result.Append("</div>");

        result.Append("<div id=\"dvComment\">");
        result.Append("<table class=\"ektronGrid\">");
        result.Append("<tr>");
        result.Append("<td class=\"label\">" + (m_refMsg.GetMessage("content HC label")) + "</td>");
        result.Append("<td>" + (m_cCont["Comment"]) + "</td>");
        result.Append("</tr>");
        result.Append("</table>");
        result.Append("</div>");

        //Taxonomy
        result.Append("<div id=\"dvTaxonomy\">");
        result.Append("<table class=\"ektronGrid\">");
        result.Append("<tr><td class=\"label\">Assigned Taxonomy/Category:</td><td><table>");
        TaxonomyBaseData[] taxonomy_cat_arr = null;
        taxonomy_cat_arr = m_refContent.ReadAllAssignedCategory(aprId);
        if ((taxonomy_cat_arr != null) && taxonomy_cat_arr.Length > 0)
        {
            foreach (TaxonomyBaseData taxonomy_cat in taxonomy_cat_arr)
            {
                result.Append("<tr>");
                result.Append("<td><li>" + taxonomy_cat.TaxonomyPath.Remove(0, 1).Replace("\\", " > ") + "</li></td>");
                result.Append("</tr>");
            }
        }
        else
        {
            result.Append("<tr><td>&nbsp;</td><td>No categories selected.</td></tr>");
        }
        result.Append("</table></td></tr></table>");
        result.Append("</div>");


        result.Append("<div id=\"dvContent\">");

        bool bPackageDisplayXSLT;
        string CurrentXslt;
        bPackageDisplayXSLT = false;
        CurrentXslt = "";
        if (m_cCont["XmlConfiguration"] != null && ((Collection)m_cCont["XmlConfiguration"]).Count > 0 )
        {
            //check to see if there is alread a defualt display XSLT
            Collection tmpXmlColl = (Collection)m_cCont["XmlConfiguration"];
            if (tmpXmlColl["PackageDisplayXslt"] != null)
            {
                bPackageDisplayXSLT = true;
            }
            else
            {
                if (tmpXmlColl["DefaultXslt"] != null)
                {
                    bPackageDisplayXSLT = false;
                    Collection tmpXsltColl = (Collection)tmpXmlColl["PhysPathComplete"];
                    if (tmpXsltColl["Xslt" + tmpXmlColl["DefaultXslt"]] != null)
                    {
                        CurrentXslt = (string)(tmpXsltColl["Xslt" + tmpXmlColl["DefaultXslt"]]);
                    }
                    else
                    {
                        CurrentXslt = (string)(tmpXsltColl["Xslt" + tmpXmlColl["DefaultXslt"]]);
                    }
                }
                else
                {
                    bPackageDisplayXSLT = true;
                }
            }

            if (bPackageDisplayXSLT)
            {
                result.Append(m_refAPI.XSLTransform(m_cCont["ContentHtml"].ToString(), (string)(tmpXmlColl["PackageDisplayXslt"]), false, false, null, false, true));
            }
            else
            {
                result.Append(m_refAPI.TransformXSLT(m_cCont["ContentHTML"].ToString(), CurrentXslt));
            }
        }
        else
        {
            //----- Defect #28122 - Content tab is blank when viewing dms asset from View Approval Report screen.
            //----- Only contentHtml was being added to the content tab div.  Int he case of an asset, it must be
            //----- downloaded.
            if (Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(m_cCont["ContentType"]), true))
            {
                if ((string)m_cCont["ContentStatus"] != "A" && Convert.ToString(Request.QueryString["action"]).ToLower().Trim() == "view")
                {
                    result.Append("<iframe width=\"100%\" height=\"100%\" src=\"" + m_refContentApi.GetViewUrl(m_cCont["AssetID"].ToString(), Convert.ToInt32(m_cCont["ContentType"])) + "\"></iframe>");
                }
                else
                {
                    string ver = "";
                    ver = (string)("&version=" + m_cCont["AssetVersion"]);
                    result.Append("<div align=\"center\" style=\"padding:15px;\"><a style=\"text-decoration:none;\" href=\"#\" onclick=\"javascript:window.open(\'" + m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?ID=" + m_cCont["AssetID"] + ver + "\',\'DownloadAsset\',\'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=1000,height=800\');return false;\"><img align=\"middle\" src=\"" + m_refContentApi.AppPath + "images/application/download.gif\" />" + m_refMsg.GetMessage("btn download") + " &quot;" + m_cCont["ContentTitle"] + "&quot;</a></div>");
                }
            }
            else
            {
                if (Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData == (EkEnumeration.CMSContentSubtype)m_cCont["ContentSubType"])
                {
                    result.Append(Ektron.Cms.PageBuilder.PageData.RendertoString(m_cCont["ContentHTML"].ToString()));
                }
                else
                {
                    result.Append(m_cCont["ContentHTML"]);
                }
            }
        }
        result.Append("</div>");

        result.Append("</div>"); //tabContainer
        result.Append("</div>"); //tabContainerWrapper
        litViewContent.Text = result.ToString();

    }

    private void ViewToolBar()
    {
        string AltPublishMsg;
        string AltApproveMsg;
        string AltDeclineMsg;
        string PublishIcon;
        string CaptionKey;
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        divTitleBar.InnerHtml = m_refStyle.GetTitleBar((string)(m_refMsg.GetMessage("generic approve title") + " \'" + m_cCont["ContentTitle"] + "\'?"));
        result.Append("<table><tr>");

		if (Request.QueryString["page"] == "workarea")
		{
			// redirect to workarea when user clicks back button if we're in workarea
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/back.png", "javascript:top.switchDesktopTab()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}
		else
		{
			result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/back.png", "javascript:history.back()", m_refMsg.GetMessage("alt back button text"), m_refMsg.GetMessage("btn back"), "", StyleHelper.BackButtonCssClass, true));
		}

        if (m_cCont["ContentStatus"].ToString() == "S" || m_cCont["ContentStatus"].ToString() == "M")
        {
			string primaryCssClass;

            if (m_cCont["ContentStatus"].ToString() == "S")
            {
                AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (change)");
                AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (change)");
                AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (change)");
                PublishIcon = "../UI/Icons/contentPublish.png";
                CaptionKey = "btn publish";
				primaryCssClass = StyleHelper.PublishButtonCssClass;
            }
            else
            {
                AltPublishMsg = m_refMsg.GetMessage("approvals:Alt Publish Msg (delete)");
                AltApproveMsg = m_refMsg.GetMessage("approvals:Alt Approve Msg (delete)");
                AltDeclineMsg = m_refMsg.GetMessage("approvals:Alt Decline Msg (delete)");
                PublishIcon = "../UI/Icons/delete.png";
                CaptionKey = "btn delete";
				primaryCssClass = StyleHelper.DeleteButtonCssClass;
            }

			bool primaryCssApplied = false;

            if (Convert.ToBoolean(m_meObj["CanIPublish"]))
            {
                if (m_TaskExists == true)
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppImgPath + PublishIcon, "#", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), "Onclick=\"javascript:return LoadChildPage(\'action=approveContentAction&id=" + m_cCont["ContentID"] + "&fldid=" + m_cCont["FolderID"] + "&page=" + m_sPage + "&LangType=" + m_refAPI.ContentLanguage + "\');\"", primaryCssClass, !primaryCssApplied));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppImgPath + PublishIcon, "approval.aspx?action=approveContentAction&id=" + m_cCont["ContentID"] + "&fldid=" + m_cCont["FolderID"] + "&page=" + m_sPage + "&LangType=" + m_refAPI.ContentLanguage + "", AltPublishMsg, m_refMsg.GetMessage(CaptionKey), "", primaryCssClass, !primaryCssApplied));
                }

				primaryCssApplied = true;
            }
            else if (Convert.ToBoolean(m_meObj["CanIApprove"]))
            {
                if (m_TaskExists == true)
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/approvalApproveItem.png", "#", AltApproveMsg, m_refMsg.GetMessage("btn approve"), "Onclick=\"javascript:return LoadChildPage(\'action=approveContentAction&id=" + m_cCont["ContentID"] + "&fldid=" + m_cCont["FolderID"] + "&page=" + m_sPage + "&LangType=" + m_refAPI.ContentLanguage + "\');\"", StyleHelper.ApproveButtonCssClass, !primaryCssApplied));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/approvalApproveItem.png", "approval.aspx?action=approveContentAction&id=" + m_cCont["ContentID"] + "&fldid=" + m_cCont["FolderID"] + "&page=" + m_sPage + "&LangType=" + m_refAPI.ContentLanguage + "", AltApproveMsg, m_refMsg.GetMessage("btn approve"), "", StyleHelper.ApproveButtonCssClass, !primaryCssApplied));
                }

				primaryCssApplied = true;
            }

            if (Convert.ToBoolean(m_meObj["CanIPublish"]) || Convert.ToBoolean(m_meObj["CanIApprove"]))
            {
                if (m_TaskExists == true)
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/approvalDenyItem.png", "#", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), "Onclick=\"javascript:return LoadChildPage(\'action=declineContentAction&id=" + m_cCont["ContentID"] + "&fldid=" + m_cCont["FolderID"] + "&page=" + m_sPage + "&LangType=" + m_refAPI.ContentLanguage + "\');\"", StyleHelper.DeclineButtonCssClass, !primaryCssApplied));
                }
                else
                {
					result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/approvalDenyItem.png", "javascript:DeclineContent(\'" + m_cCont["ContentID"] + "\', \'" + m_cCont["FolderID"] + "\', \'" + m_sPage + "\', \'" + m_refAPI.ContentLanguage + "\')", AltDeclineMsg, m_refMsg.GetMessage("btn decline"), "", StyleHelper.DeclineButtonCssClass, !primaryCssApplied));
                }

				primaryCssApplied = true;
            }

            if (Convert.ToBoolean(m_meObj["CanIEditSubmitted"]))
            {
                if (Convert.ToInt32(m_cCont["ContentType"]) != 3333 && Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent != (EkEnumeration.CMSContentSubtype)m_cCont["ContentSubType"])
                {
                    result.Append(this.m_refStyle.GetEditAnchor(Convert.ToInt64(m_cCont["ContentID"]), 1, true, (EkEnumeration.CMSContentSubtype)m_cCont["ContentSubType"], !primaryCssApplied));
                }
                else if (Convert.ToInt32(m_cCont["ContentType"]) == 3333)
                {
					result.Append(this.m_refStyle.GetEditAnchor(Convert.ToInt64(m_cCont["ContentID"]), 3333, true, (EkEnumeration.CMSContentSubtype)m_cCont["ContentSubType"], !primaryCssApplied));
                }

				primaryCssApplied = true;

                if (Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData == (EkEnumeration.CMSContentSubtype)m_cCont["ContentSubType"])
                {
                    result.Append(m_refStyle.GetPageBuilderEditAnchor(Convert.ToInt64(m_cCont["ContentID"]), Convert.ToInt32(m_cCont["ContentLanguage"]), m_cCont["Quicklink"].ToString()));
                }
            }

            if ((toggle == "published") && (m_cCont["ContentStatus"].ToString() == "S"))
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/contentViewPublished.png", "approval.aspx?LangType=" + m_refAPI.ContentLanguage + "&action=viewContent&id=" + m_cCont["ContentID"] + "&content=" + toggle + "&fldid=" + m_cCont["FolderID"] + "&page=" + m_sPage + "", m_refMsg.GetMessage("alt view published button text (approvals)"), m_refMsg.GetMessage("btn view publish"), "", StyleHelper.ViewPublishedButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;

				if (!(Ektron.Cms.Common.EkConstants.IsAssetContentType(Convert.ToInt64(m_cCont["ContentType"]), true) || (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.PageBuilderData == Convert.ToInt32(m_cCont["ContentSubType"]) || (int)Ektron.Cms.Common.EkEnumeration.CMSContentSubtype.WebEvent == Convert.ToInt32(m_cCont["ContentSubType"])))
                {
                    result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/contentViewDifferences.png", "#", "View Difference", m_refMsg.GetMessage("btn view diff"), "onclick=\"PopEditWindow(\'compare.aspx?id=" + m_cCont["ContentID"] + "&LangType=" + m_refAPI.ContentLanguage + "\', \'Compare\', 785, 650, 1, 1);\"", StyleHelper.ViewDifferenceButtonCssClass));
                }
            }
            else if (m_cCont["ContentStatus"].ToString() == "S")
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppPath + "images/UI/Icons/preview.png", "approval.aspx?LangType=" + m_refAPI.ContentLanguage + "&action=viewContent&id=" + m_cCont["ContentID"] + "&content=" + toggle + "&fldid=" + m_cCont["FolderID"] + "&page=" + m_sPage + "", m_refMsg.GetMessage("alt view staged button text (approvals)"), m_refMsg.GetMessage("btn view staged"), "", StyleHelper.PreviewButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
            }

            if (m_TaskExists == true)
            {
                result.Append(m_refStyle.GetButtonEventsWCaption(m_refAPI.AppImgPath + "btn_viewtask-nm.gif", (string)("tasks.aspx?action=viewcontenttask&ty=both&cid=" + m_cCont["ContentID"] + "&callbackpage=content.aspx&parm1=action&value1=" + Request.QueryString["action"] + "&parm2=id&value2=" + m_cCont["ContentID"] + "&parm3=LangType&value3=" + m_refAPI.ContentLanguage + "&LangType=" + m_refAPI.ContentLanguage), "View Task", m_refMsg.GetMessage("btn view task"), "", StyleHelper.ViewTaskButtonCssClass, !primaryCssApplied));

				primaryCssApplied = true;
            }
        }
		result.Append(StyleHelper.ActionBarDivider);
        result.Append("<td>");
        result.Append(m_refStyle.GetHelpButton(Request.QueryString["action"], ""));
        result.Append("</td>");
        result.Append("</tr></table>");
        divToolBar.InnerHtml = result.ToString();
    }
    private void RegisterResources()
    {
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUICoreJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronUITabsJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaHelperJS);
        Ektron.Cms.API.JS.RegisterJSBlock(this, "$ektron( '.tabContainer' ).tabs( 'option', 'selected', 3 );", "DefaultTab");
        
        // Language Translation String
        ltr_decline.Text = m_refMsg.GetMessage("reason to decline");
    }
}

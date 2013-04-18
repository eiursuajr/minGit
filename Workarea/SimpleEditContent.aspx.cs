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
using Ektron.Cms.Common;

	public partial class SimpleEditContent : System.Web.UI.Page
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Utilities.ValidateUserLogin();
			if (this.IsPostBack)
			{
				//This receives fields which are posted to server with these IDs:
				
				//     [ControlName]_Action        = the action to take - save, check in, undo checkout
				//     [ControlName]_ContentTitle  = the content title
				//     [ControlName]_ContentId     = The ID of the content
				//     [ControlName]_Language      = The language of the content
				
				// The content title caption is surrounded with a span that
				// has this ID:
				
				//    [ControlName](_TitleLabel)
				
				// The valid values for the [ControlName]_Action value are:
				//     Publish - Submit the posted content for publication
				//     CheckIn - Check in the content
				//     Save - Save the content and redisplay in the editor
				//     UndoCheckout - Undo the checkout of the content
				
				string strHtml = "<h2 title=\"Received Edit CMS400 HTML Content Post\">Received Edit CMS400 HTML Content Post</h2>";

                string strAction = EkFunctions.HtmlEncode(Request.Form["HtmlEditor1_Action"]);
                string strContentTitle = EkFunctions.HtmlEncode(Request.Form["HtmlEditor1_ContentTitle"]);
                string strContentId = EkFunctions.HtmlEncode(Request.Form["HtmlEditor1_ContentId"]);
                string strLanguage = EkFunctions.HtmlEncode(Request.Form["HtmlEditor1_Language"]);
                string strPostedHtml = EkFunctions.HtmlEncode(Request.Form["HtmlEditor1"]);
				
				EditHtmlUI.Visible = false;
				
				// Display appropriate Title
				switch (strAction)
				{
					case "Publish":
						strHtml += "<h3 title=\"Publishing HTML Content\">Publishing HTML Content</h3>";
						break;
						
					case "CheckIn":
						strHtml += "<h3 title=\"Checking In HTML Content\">Checking In HTML Content</h3>";
						break;
						
					case "Save":
						strHtml += "<h3 title=\"Saving HTML Content\">Saving HTML Content</h3>";
						break;
						
					case "UndoCheckout":
						strHtml += "<h3 title=\"Undoing Checkout of HTML Content\">Undoing Checkout of HTML Content</h3>";
						break;
						
				}
				
				// Show the posted data
				strHtml += "<p>";
				strHtml += "Post Action:  " + strAction + "<br />";
				strHtml += "Content Title:  " + strContentTitle + "<br />";
				strHtml += "Content ID:  " + strContentId + "<br />";
				strHtml += "Langauge ID:  " + strLanguage + "<br />";
				strHtml += "</p>";
				
				strHtml += "<div id=\"TextAreaTitle\">";
				strHtml += "<h3 title=\"Posted HTML Source from the Editor\">Posted HTML Source from the Editor</h3>";
				strHtml += "<textarea title=\"Edit Content here\" name=\"PostedHtmlText\" rows=\"2\" cols=\"20\" id=\"AnyHtmlText\" ";
				strHtml += "style=\"background-color:Azure;height:401px;width:95%;\">";
				strHtml += EkFunctions.HtmlEncode(strPostedHtml.Replace("\r\n" + "\r\n", "\r\n"));
				strHtml += "</textarea><br />";
				
				strHtml += "</div>";
				
				
				// ==========================================================
				// Perform the Action
				strHtml += "<p>Action Status:  ";
				switch (strAction)
				{
					case "Publish":
						strHtml += PublishHtml();
						break;
						
					case "CheckIn":
						strHtml += CheckInHtml();
						break;
						
					case "Save":
						strHtml += SaveHtml();
						break;
						
					case "UndoCheckout":
						strHtml += UndoCheckoutHtml();
						break;
						
				}
				strHtml += "</p>";
				// =========================================================
				
				
				// Display the posted HTML
				strHtml += "<br /><br /><hr />";
				strHtml += strPostedHtml;
				
				
				Response.Write(strHtml);
			}
			else
			{
				
				
			}
			
		}
		
		private string PublishHtml()
		{
			string strRet = "";
			
			return strRet;
		}
		
		private string CheckInHtml()
		{
			string strRet = "";
			
			return strRet;
		}
		
		private string SaveHtml()
		{
			string strRet = "";
			
			return strRet;
		}
		
		private string UndoCheckoutHtml()
		{
			string strRet = "";
			
			return strRet;
		}
		
	}

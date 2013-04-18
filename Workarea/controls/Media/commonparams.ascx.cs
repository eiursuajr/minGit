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

	public partial class Multimedia_commonparams : System.Web.UI.UserControl
	{
		
		
		private Ektron.Cms.CommonApi m_refContentApi = new Ektron.Cms.CommonApi();
		private string m_contentHtml = string.Empty;
		private string m_mimeType = string.Empty;
		private string m_assetID = string.Empty;
		private string m_assetFileName = string.Empty;
		private string m_assetVersion = string.Empty;
		private string objectTagStr = string.Empty;
		
		public string ContentHtml
		{
			set
			{
				value = value.Substring(value.IndexOf("<root>"), System.Convert.ToInt32(value.IndexOf("</root>") + 7));
				m_contentHtml = value;
			}
		}
		
		public string MimeType
		{
			set
			{
				m_mimeType = value;
			}
		}
		
		public string AssetID
		{
			set
			{
				m_assetID = value;
			}
		}
		
		public string AssetVersion
		{
			set
			{
				m_assetVersion = value;
			}
		}
		
		public string AssetFileName
		{
			set
			{
				m_assetFileName = value;
			}
		}
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
			
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string sInclude = string.Empty;
			sInclude = "<script type=\"text/javascript\" language=\"JavaScript\" src=\"" + m_refContentApi.AppPath + "java/colorpicker.js\"></script>" + "\r\n";
			sInclude += "<script type=\"text/javascript\" src=\"" + m_refContentApi.AppPath + "java/com.ektron.utils.tabs.js\" language=\"javascript\"></script> " + "\r\n";
			sInclude += "<link href=\"" + m_refContentApi.AppPath + "csslib/EktTabs.css\" rel=\"stylesheet\" type=\"text/css\" /> " + "\r\n";
			sInclude += "<input type=\"hidden\" id=\"media_title\" name=\"media_title\" value=\"" + m_assetID + "\"/>";
			sInclude += "<input type=\"hidden\" id=\"media_fileName\" name=\"media_fileName\" value=\"" + m_assetFileName + "\" />";

            Ektron.Cms.API.JS.RegisterJS(this, m_refContentApi.AppPath + "controls/media/commonMedia.js", "EktronCommonMediaJS");

			///'''''''''
			//' MAC: I think we should disable preview in mac.
			//' Psudo plan:
			//' Read supported players
			//' Create check boxes of all supoorted players and make default player checkbox read only
			//' Now loop through all object from MediaXML (content_html)
			//' mark checkbox if it found and enable ui.
			//'
			///''''''''''
			Hashtable htSupportedPlayer = m_refContentApi.EkContentRef.GetSupportedPlayer(m_mimeType);
			StringBuilder sHtml = new StringBuilder();
			string strCont = "";
			string strMediaPlayer = string.Empty;
			string strCheckboxes = string.Empty;
			string strDefaultScript = string.Empty;
			string varIDs = string.Empty;
			string fillObjects = string.Empty;
			bool bDefault = false;
			string strDisplayName;
			System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
			System.Xml.XmlNode root;
			System.Xml.XmlNodeList nodelist;
			System.Xml.XmlNode mediaPlayerNode;
			
			//BugFix for 30930
			m_contentHtml = m_contentHtml.Replace("&", "&amp;");
			
			xDoc.LoadXml(m_contentHtml);
			root = xDoc.DocumentElement;
			
			lblMultimedia.Text = m_refContentApi.EkMsgRef.GetMessage("lbl multimedia properties");
			lblMultimedia.ToolTip = lblMultimedia.Text;
			lblwidth.Text = m_refContentApi.EkMsgRef.GetMessage("lbl width");
			lblwidth.ToolTip = lblwidth.Text;
			lblHeight.Text = m_refContentApi.EkMsgRef.GetMessage("lbl height");
			lblMultimedia.ToolTip = lblHeight.Text;
			lblAuto.Text = m_refContentApi.EkMsgRef.GetMessage("lbl autostart");
			lblAuto.ToolTip = lblAuto.Text;
			lblLoop.Text = m_refContentApi.EkMsgRef.GetMessage("lbl loop");
			lblLoop.ToolTip = lblLoop.Text;
			
			foreach (string tempLoopVar_strMediaPlayer in htSupportedPlayer.Keys)
			{
				strMediaPlayer = tempLoopVar_strMediaPlayer;
				bDefault = false;
				strCont = "";
				strDisplayName = "";
				strDisplayName = strMediaPlayer;
				strMediaPlayer = strMediaPlayer.ToLower();
				
				if (strMediaPlayer != "default")
				{
					strCheckboxes += "<input type=\"checkbox\" onclick=\"enableMediaTab(this, \'" + strMediaPlayer + "\')\" id=\"" + strMediaPlayer + "-chk\" ";
					if (strMediaPlayer.ToLower() == htSupportedPlayer["default"].ToString().ToLower())
					{
						bDefault = true;
						strCheckboxes += " checked=\"checked\" disabled=\"disabled\" ";
						strDefaultScript += "myTitle = \'" + m_assetID + "\';" + "\r\n";
						//strDefaultScript &= "alert(document.getElementById( '" & strMediaPlayer & "-chk' ));" & vbCrLf
						strDefaultScript += "enableMediaTab(document.getElementById( \'" + strMediaPlayer + "-chk\' ), \'" + strMediaPlayer + "\');" + "\r\n";
					}
					strCheckboxes += " />" + strDisplayName + "&nbsp;";
					
					if (varIDs != "")
					{
						varIDs += ",\'" + strMediaPlayer + "\'";
					}
					else
					{
						varIDs = "\'" + strMediaPlayer + "\'";
					}
					if (bDefault)
					{
						fillObjects += "js" + strMediaPlayer + ".defaultPlayer=true;" + "\r\n";
					}
					fillObjects += "js" + strMediaPlayer + ".CLSID = \'" + m_refContentApi.EkContentRef.ReadMediaSettings(m_refContentApi.RequestInformationRef.MultimediaSettings, strMediaPlayer + "-CLSID", "") + "\';" + "\r\n";
					
					//Do not fix the path if player is real player since real play needs the physical file.  Real player will not play
					//the dynamically streamed content.
					if (strMediaPlayer.ToLower() != "realplayer")
					{
						fillObjects += "js" + strMediaPlayer + ".URL = \'" + m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?history=true&ID=" + m_assetID + "&version=" + m_assetVersion + "\';";
					}
					else
					{
						fillObjects += "js" + strMediaPlayer + ".URL = \'" + m_assetFileName + "\';";
					}
					
					fillObjects += "js" + strMediaPlayer + ".Codebase = \'" + m_refContentApi.EkContentRef.ReadMediaSettings(m_refContentApi.RequestInformationRef.MultimediaSettings, strMediaPlayer + "-Codebase", "") + "\';" + "\r\n";
					fillObjects += "js" + strMediaPlayer + ".fill(\"" + m_assetID + "_" + strMediaPlayer + "\");" + "\r\n";
					
					//generate preview span tag mediaPlayerNode
					nodelist = root.SelectNodes("MediaPlayer[@player=\'" + strMediaPlayer + "\']");
					sHtml.Append("<span ");
					sHtml.Append("id=\"" + strMediaPlayer + "-preview\" >");
					if (nodelist.Count > 0)
					{
						if (! bDefault)
						{
							fillObjects += " document.getElementById( \'" + strMediaPlayer + "-chk\' ).checked = true;" + "\r\n";
							fillObjects += " enableMediaTab(document.getElementById( \'" + strMediaPlayer + "-chk\' ), \'" + strMediaPlayer + "\');" + "\r\n";
						}
						mediaPlayerNode = nodelist[0];
						strCont = Server.HtmlDecode(mediaPlayerNode.InnerXml.ToString());
						strCont = FixURL(strCont);
						if (strMediaPlayer == "quicktime")
						{
							objectTagStr = strCont;
							strCont = "";
						}
					}
					
					//Do not fix the path if player is real player since real play needs the physical file.  Real player will not play
					//the dynamically streamed content.
					if (strMediaPlayer.ToLower() != "realplayer")
					{
						strCont = FixPath(strCont);
					}
					
					sHtml.Append(strCont);
					sHtml.Append("</span>");
					
					//Render html but ui still not visiable
					if (strMediaPlayer.ToLower() == "windowsmedia")
					{
						this.WindowsMediaPanel.Visible = true;
					}
					else if (strMediaPlayer.ToLower() == "quicktime")
					{
						this.QuicktimePanel.Visible = true;
					}
					else if (strMediaPlayer.ToLower() == "realplayer")
					{
						this.RealPlayerPanel.Visible = true;
					}
					else if (strMediaPlayer.ToLower() == "flash")
					{
						this.FlashPanel.Visible = true;
						this.PlaceHolder1.Visible = false;
					}
					
					
				}
			}
			
			ltInclude.Text = sInclude;
			ltCheckboxes.Text = strCheckboxes;
			//Line below work only in IE
			//Page.ClientScript.RegisterStartupScript(Me.GetType(), "array_of_ids", "arrID = [" & varIDs & "];", True)
			//Page.ClientScript.RegisterStartupScript(Me.GetType(), "enabledefaulttab", fillObjects & strDefaultScript, True)
			ltResults.Text = sHtml.ToString();
            string jsLiteral = string.Empty;
			
			if (strMediaPlayer == "quicktime")
			{
				StringBuilder js = new StringBuilder();
				js.Append("function InsertIntoObjMovieTag(objStr)");
				js.Append("{");
				js.Append(" var target = document.getElementById(\'" + strMediaPlayer + "-preview\');");
				js.Append(" target.innerHTML = objStr;");
				js.Append(" }");
				jsLiteral += js.ToString();
				jsLiteral += "InsertIntoObjMovieTag(\'" + objectTagStr + "\');";
			}
			string ext = "";
			ext = System.IO.Path.GetExtension(m_assetFileName);
         
			jsLiteral += "SetDMSExt(\"" + ext + "\",\"" + this.ID.Replace("\"", "\\\"") + "\");" + "\r\n" + "arrID = [" + varIDs + "];" + "\r\n" + fillObjects + "\r\n" + strDefaultScript ;
            Ektron.Cms.API.JS.RegisterJSBlock(this, jsLiteral, "EktronSetDMSExt");
		}
		public string FixPath(string html)
		{
			//this is used when the content is first checked out
			return html.Replace(m_assetFileName, m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?history=true&ID=" + m_assetID + "&version=" + m_assetVersion);
		}
		private string FixURL(string strCont)
		{
			//This is used when the content is drag and drop in the edit window
			int iPos1 = -1;
			int iPos2 = -1;
			string oldURL = "";
			iPos1 = strCont.IndexOf(m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?history=true&ID=" + m_assetID + "&version=");
			if (iPos1 > -1)
			{
				iPos2 = strCont.IndexOf("\"", iPos1);
				if (iPos2 > -1)
				{
					oldURL = strCont.Substring(iPos1, iPos2 - iPos1);
					strCont = strCont.Replace(oldURL, m_refContentApi.SitePath + "assetmanagement/DownloadAsset.aspx?history=true&ID=" + m_assetID + "&version=" + m_assetVersion);
				}
			}
			return strCont;
		}
		
	}

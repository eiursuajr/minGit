using System;
using System.Text;
using System.Web.UI;
using Ektron.Cms;
//using Ektron.Cms.Common.EkConstants;
using Ektron.Cms.Commerce;
using Microsoft.VisualBasic;

public partial class compares : System.Web.UI.Page
	{
		
		
		#region Private members
		
		private long m_intId = 0;
		private long m_intHistoryId = 0;
		private string newcontent = "";
		private string basecontent = "";
		private ContentAPI m_refContApi = new ContentAPI();
		private bool IsXMLDoc = false;
		private bool IsPublished = false;
		private int ContentLanguage = -1;
		private string CurrentXslt = "";
		private string PageInfo = "";
		private string strContentHtml = "";
		private long m_ContentType = Ektron.Cms.Common.EkConstants.CMSContentType_Content;
		
		#endregion
		
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			try
			{

                int port = Ektron.Cms.Common.EkFunctions.ReadIntegerValue(Request.ServerVariables["SERVER_PORT"], 80);
				if (Page.Request.Url.Scheme == "https")
				{
					PageInfo = "src=\"https://" + Request.ServerVariables["Server_name"] + (port != 443 ? ":" + port.ToString() : "") + "/";
				}
				else
				{
                    PageInfo = "src=\"http://" + Request.ServerVariables["Server_name"] + (port != 80 ? ":" + port.ToString() : "") + "/";
				}
				
				if (Request.QueryString["id"] != "")
				{
					m_intId = Convert.ToInt64(Request.QueryString["id"]);
				}
				if (!(Request.QueryString["LangType"] == null))
				{
					if (Request.QueryString["LangType"] != "")
					{
						ContentLanguage = Convert.ToInt32(Request.QueryString["LangType"]);
						m_refContApi.SetCookieValue("LastValidLanguageID", ContentLanguage.ToString());
					}
					else
					{
						if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
						{
							ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
						}
					}
				}
				else
				{
					if (m_refContApi.GetCookieValue("LastValidLanguageID") != "")
					{
						ContentLanguage = Convert.ToInt32(m_refContApi.GetCookieValue("LastValidLanguageID"));
					}
				}
				if (ContentLanguage == Ektron.Cms.Common.EkConstants.CONTENT_LANGUAGES_UNDEFINED)
				{
					m_refContApi.ContentLanguage = Ektron.Cms.Common.EkConstants.ALL_CONTENT_LANGUAGES;
				}
				else
				{
					m_refContApi.ContentLanguage = ContentLanguage;
				}
				if (Request.QueryString["hist_id"] != "")
				{
					m_intHistoryId = Convert.ToInt64(Request.QueryString["hist_id"]);
				}
				
				m_ContentType = m_refContApi.EkContentRef.GetContentType(m_intId);
				
				Util_GetBaseContent();
				Util_GetNewContent();
				
				//If IsPublished And Request.QueryString("hist_id") Is Nothing Then
				
				//    Dim tempString As String = basecontent
				//    basecontent = newcontent
				//    newcontent = tempString
				
				//End If
				Process_Compare(basecontent, newcontent);
			}
			catch (Exception ex)
			{
                Utilities.ShowError(ex.Message);
			}
		}
		private void Util_GetBaseContent()
		{
			switch (m_ContentType)
			{
				case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry:
					CatalogEntryApi m_refCatalogAPI = new CatalogEntryApi();
					EntryData entry_data = null;
					
					entry_data = m_refCatalogAPI.GetItem(m_intId);
					
					if (entry_data.ProductType.PackageDisplayXslt.Length > 0)
					{
                        basecontent = m_refContApi.XSLTransform(entry_data.Html, entry_data.ProductType.PackageDisplayXslt, false, false, null, false, true);
					}
					else
					{
                        Collection coll = (Collection)entry_data.ProductType.PhysPathComplete;
                        Collection logicalColl = (Collection)entry_data.ProductType.LogicalPathComplete;

						if (Convert.ToString(coll["Xslt" + entry_data.ProductType.DefaultXslt]).Length > 0)
						{
							CurrentXslt = (string) (coll["Xslt" + entry_data.ProductType.DefaultXslt]);
						}
						else
						{
							CurrentXslt = (string) (logicalColl["Xslt" + entry_data.ProductType.DefaultXslt]);
						}
						basecontent = m_refContApi.TransformXSLT(entry_data.Html, CurrentXslt);
					}
					basecontent = Server.HtmlDecode(basecontent.Replace("src=\"/", PageInfo));
					break;
				default:
					ContentData content_data;
					ContentData show_content_data;
					show_content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published);
					if (m_intHistoryId == 0)
					{
						content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published);
					}
					else
					{
						content_data = m_refContApi.GetContentByHistoryId(m_intHistoryId);
					}
					if (show_content_data == null) // only get staged content if there has never been published content
					{
						show_content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged);
					}
					if (content_data == null)
					{
						content_data = show_content_data;
					}
					//Published content
                   

					IsXMLDoc = System.Convert.ToBoolean(!(content_data.XmlConfiguration == null));
					if (IsXMLDoc)
					{
						if (content_data.XmlConfiguration.PackageDisplayXslt.Length > 0)
						{
                            basecontent = m_refContApi.XSLTransform(content_data.Html, content_data.XmlConfiguration.PackageDisplayXslt, false, false, null, false, true);
						}
						else
						{
                            Collection xmlConfig = (Collection)content_data.XmlConfiguration.PhysPathComplete;
                            Collection xmlConfigPhy = (Collection)content_data.XmlConfiguration.LogicalPathComplete;

							if (Convert.ToString(xmlConfig["Xslt" + content_data.XmlConfiguration.DefaultXslt]).Length > 0)
							{
								CurrentXslt = (string) (xmlConfig["Xslt" + content_data.XmlConfiguration.DefaultXslt]);
							}
							else
							{
								CurrentXslt = (string) (xmlConfigPhy["Xslt" + content_data.XmlConfiguration.DefaultXslt]);
							}
							basecontent = m_refContApi.TransformXSLT(content_data.Html, CurrentXslt);
						}
						IsXMLDoc = false;
					}
					else
					{
						basecontent = content_data.Html;
					}
					basecontent = basecontent.Replace("src=\"/", PageInfo);
					if (! IsXMLDoc)
					{
						basecontent = RemoveHTML(basecontent);
					}
					IsPublished = show_content_data.IsPublished;
					break;
			}
		}
		
		private void Util_GetNewContent()
		{
			switch (m_ContentType)
			{
				case Ektron.Cms.Common.EkConstants.CMSContentType_CatalogEntry:
					CatalogEntryApi m_refCatalogAPI = new CatalogEntryApi();
					EntryData entry_version_data = null;
					if (m_intHistoryId == 0)
					{
						entry_version_data = m_refCatalogAPI.GetItemEdit(m_intId, m_refContApi.ContentLanguage, false);
					}
					else
					{
						entry_version_data = m_refCatalogAPI.GetItemVersion(m_intId, m_refContApi.ContentLanguage, m_intHistoryId);
					}
					if (entry_version_data.ProductType.PackageDisplayXslt.Length > 0)
					{
						newcontent = m_refContApi.TransformXsltPackage(entry_version_data.Html, entry_version_data.ProductType.PackageDisplayXslt, false);
					}
					else
					{
                        Collection collEntryVersion = (Collection)entry_version_data.ProductType.PhysPathComplete;
                        Collection collEntryVersionLogical = (Collection)entry_version_data.ProductType.LogicalPathComplete;

                        if (Convert.ToString(collEntryVersion["Xslt" + entry_version_data.ProductType.DefaultXslt]).Length > 0)
						{
                            CurrentXslt = (string)(collEntryVersion["Xslt" + entry_version_data.ProductType.DefaultXslt]);
						}
						else
						{
							CurrentXslt = (string) (collEntryVersionLogical["Xslt" + entry_version_data.ProductType.DefaultXslt]);
						}
						newcontent = m_refContApi.TransformXSLT(entry_version_data.Html, CurrentXslt);
					}
					newcontent = newcontent.Replace("src=\"/", PageInfo);
					break;
				default:
					ContentData content_data;
					//Dim show_content_data As ContentData
					if (m_intHistoryId == 0)
					{
						content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged);
						//show_content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Staged)
						if (content_data == null)
						{
							content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published);
							//content_data = show_content_data
						}
						strContentHtml = content_data.Html;
					}
					else
					{
						content_data = m_refContApi.GetContentById(m_intId, ContentAPI.ContentResultType.Published);
						strContentHtml = content_data.Html;
					}
					CurrentXslt = "";
					IsXMLDoc = System.Convert.ToBoolean(!(content_data.XmlConfiguration == null));
					if (IsXMLDoc)
					{
						if (content_data.XmlConfiguration.PackageDisplayXslt.Length > 0)
						{
							newcontent = m_refContApi.TransformXsltPackage(strContentHtml, content_data.XmlConfiguration.PackageDisplayXslt, false);
						}
						else
						{
                            Collection collCData = (Collection)content_data.XmlConfiguration.PhysPathComplete;
                            Collection collCDataLogical = (Collection)content_data.XmlConfiguration.LogicalPathComplete;

                            if (Convert.ToString(collCData["Xslt" + content_data.XmlConfiguration.DefaultXslt]).Length > 0)
							{
                                CurrentXslt = (string)(collCData["Xslt" + content_data.XmlConfiguration.DefaultXslt]);
							}
							else
							{
								CurrentXslt = (string) (collCDataLogical["Xslt" + content_data.XmlConfiguration.DefaultXslt]);
							}
							newcontent = m_refContApi.TransformXSLT(strContentHtml, CurrentXslt);
						}
						IsXMLDoc = false;
					}
					else
					{
						newcontent = strContentHtml;
					}
					newcontent = newcontent.Replace("src=\"/", PageInfo);
					if (! IsXMLDoc)
					{
						newcontent = RemoveHTML(newcontent);
					}
					break;
			}
		}
		
		private void Process_Compare(string oldXHTML, string newXHTML)
		{
			if (!(oldXHTML == newXHTML))
			{
				// see if we can run the cshtmldiff control server-side (doesn't work on 64-bit windows)
				try
				{
					eWebDiffCtrl.eWebDiffCtrl diffCtrl = null;
					diffCtrl = new eWebDiffCtrl.eWebDiffCtrl();
					// note that any changes to the properties are done in the eWebDiff.eWebDiffCtrl COM object
					if (IsXMLDoc)
					{
						// be aware that we can't do xml diff until we get an All Modes license
						//Throw New Exception("No server-side XML diff support yet")
						// temporary workaround to compare it in HTML mode for now:
						IsXMLDoc = false;
						newXHTML = "<html>" + newXHTML.Replace("<", "&lt;").Replace(">", "&gt;") + "</html>";
						oldXHTML = "<html>" + oldXHTML.Replace("<", "&lt;").Replace(">", "&gt;") + "</html>";
					}
					if (m_intHistoryId == 0)
					{
					DiffResults.Text = diffCtrl.DoDiff(oldXHTML, newXHTML, IsXMLDoc);
					}
					else 
					{
					DiffResults.Text = diffCtrl.DoDiff(newXHTML, oldXHTML, IsXMLDoc);
					}
				}
				catch (Exception ex)
				{
					// log it as a msg in event log so support can ask for it
					EkException.WriteToEventLog((string) ("Unable to use eWebDiffCtrl control; reverting to control download: " + Environment.NewLine+ ex.ToString()), System.Diagnostics.EventLogEntryType.Error);
					// if we get an exception, do it the old way
					ShowDiffPanel.Visible = true;
					SiteAPI m_refSiteAPI = new SiteAPI();
					SettingsData settings_data;
					settings_data = m_refSiteAPI.GetSiteVariables(m_refContApi.UserId);
					if (!(settings_data == null))
					{
						LicKey.Text = settings_data.LicenseKey;
					}
					//If (Not IsXMLDoc) Then
					//    csbasecontent.Value = EkFunctions.HtmlEncode(oldXHTML)
					//    csnewcontent.Value = EkFunctions.HtmlEncode(newXHTML)
					//Else
					// yes, this is weird...dunno why we can't just htmlencode both and decode it on the other side
					csbasecontent.Value = oldXHTML;
					csnewcontent.Value = newXHTML;
					//End If
					return;
				}
				string viewxslt = Server.MapPath("xslt/XmlViewXslt.xsl");
				if (IsXMLDoc)
				{
					// XmlViewXslt garbles the bookstore sample form pretty printing
					//DiffOrigContent.Text = m_refContApi.TransformXSLT(oldXHTML, viewxslt).Replace("&gt;&lt;", "&gt; &lt;")
					//DiffnewXHTML.Text = m_refContApi.TransformXSLT(newXHTML, viewxslt).Replace("&gt;&lt;", "&gt; &lt;")
					DiffOrigContent.Text = oldXHTML.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&gt;&lt;", "&gt; &lt;");
					DiffNewContent.Text = newXHTML.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&gt;&lt;", "&gt; &lt;");
				}
				else if (m_intHistoryId == 0)
				{
					DiffOrigContent.Text = oldXHTML;
					DiffNewContent.Text = newXHTML;
				}
				else 
				{
					DiffOrigContent.Text =  newXHTML;
					DiffNewContent.Text = oldXHTML;
				}
				ShowDiffPanel.Visible = false;
				ShowSvrDiffPanel.Visible = true;
				Util_SetLabels();
			}
			else
			{
				HideDiffPanel.Visible = true;
			}
		}
		
		private void Util_SetLabels()
		{
			SiteAPI refSiteApi = new SiteAPI();
			Ektron.Cms.Common.EkMessageHelper refMsg = refSiteApi.EkMsgRef;
			lblTabOrig.Text = refMsg.GetMessage("lbl webdiff taborig");
			lblTabNew.Text = refMsg.GetMessage("lbl webdiff tabnew");
			lblTabDiff.Text = refMsg.GetMessage("lbl webdiff tabdiff");
			lblLegend.Text = refMsg.GetMessage("lbl webdiff legend");
			lblLegendAdded.Text = refMsg.GetMessage("lbl webdiff legendadded");
			lblLegendDeleted.Text = refMsg.GetMessage("lbl webdiff legenddeleted");
		}
		
		private string RemoveHTML(string strText)
		{
			string returnValue;
			string TAGLIST = ";em;span;u;a;";
			const string BLOCKTAGLIST = ";APPLET;";
			var nPos1 = 0;
            var nPos2 = 0;
            var nPos3 = 0;
			string strResult = "";
			object strTagName;
			object bRemove;
			object bSearchForBlock;
			nPos1 = strText.IndexOf("<") + 1;
			while (Convert.ToInt32(nPos1) > 0)
			{
				nPos2 = (Convert.ToInt32(nPos1) + 1).ToString().IndexOf(strText) + 1;
				if (nPos2 > 0)
				{
					strTagName = strText.Substring(Convert.ToInt32(nPos1) + 1 - 1, Convert.ToInt32(nPos2) - Convert.ToInt32(nPos1) - 1);
					strTagName = Strings.Replace(strTagName.ToString(), Constants.vbCr, " ", 1, -1, 0).Replace(Constants.vbLf, " ");
					
					nPos3 = strTagName.ToString().IndexOf(" ") + 1;
					if (nPos3 > 0)
					{
						strTagName = Strings.Left(strTagName.ToString(), System.Convert.ToInt32(nPos3 - 1));
					}
					
					if (Strings.Left(strTagName.ToString(), 1) == "/")
					{
						strTagName = Strings.Mid(strTagName.ToString(), 2);
						bSearchForBlock = false;
					}
					else
					{
						bSearchForBlock = true;
					}
					
					if (TAGLIST.IndexOf((";" + strTagName + ";").ToString()) + 1 > 0)
					{
						bRemove = true;
						if ( Convert.ToBoolean(bSearchForBlock))
						{
							if (BLOCKTAGLIST.ToString().IndexOf((";" + strTagName + ";").ToString()) + 1 > 0)
							{
								nPos2 = strText.Length;
								nPos3 = strText.IndexOf(("</" + strTagName).ToString(), nPos1 + 1 - 1) + 1;
								if (nPos3 > 0)
								{
									nPos3 = (nPos3 + 1).ToString().IndexOf(strText) + 1;
								}
								
								if (nPos3 > 0)
								{
									nPos2 = nPos3;
								}
							}
						}
					}
					else
					{
						bRemove = false;
					}
					
					if (Convert.ToBoolean(bRemove))
					{
						strResult = strResult + strText.Substring(0, Convert.ToInt32(nPos1) - 1);
						strText = strText.Substring(Convert.ToInt32(nPos2) + 1 - 1);
					}
					else
					{
						strResult = strResult + strText.Substring(0, Convert.ToInt32(nPos1));
						strText = strText.Substring(Convert.ToInt32(nPos1) + 1 - 1);
					}
				}
				else
				{
					strResult = strResult + strText;
					strText = "";
				}
				
				nPos1 = strText.IndexOf("<") + 1;
			}
			strResult = strResult + strText;
			strResult = strResult.Replace("&#160;", " ");
			
			// also run Tidy on the text
			TidyNet.Tidy tidydoc = new TidyNet.Tidy();
			tidydoc.Options.RawOut = false;
			tidydoc.Options.CharEncoding = TidyNet.CharEncoding.UTF8;
			tidydoc.Options.DocType = TidyNet.DocType.Omit;
			tidydoc.Options.TidyMark = false;
			tidydoc.Options.Word2000 = true;
			tidydoc.Options.QuoteNbsp = true;
			tidydoc.Options.QuoteAmpersand = true;
			tidydoc.Options.NumEntities = false;
			tidydoc.Options.QuoteMarks = true;
			tidydoc.Options.Xhtml = false;
			tidydoc.Options.MakeClean = true;
			TidyNet.TidyMessageCollection messageCollection = new TidyNet.TidyMessageCollection();
			System.IO.MemoryStream tidyin = new System.IO.MemoryStream();
			System.IO.MemoryStream tidyout = new System.IO.MemoryStream();
			if (strResult == null)
			{
				strResult = "<p></p>";
			}
			byte[] byteArray = Encoding.UTF8.GetBytes(strResult);
			tidyin.Write(byteArray, 0, byteArray.Length);
			tidyin.Position = 0;
			tidydoc.Parse(tidyin, tidyout, messageCollection);
			tidyout.Position = 0;
			string strTidyResult = Encoding.UTF8.GetString(tidyout.ToArray());
			tidyout.Close();
			if ((strTidyResult == "") && (messageCollection.Errors > 0))
			{
				
				foreach (TidyNet.TidyMessage msg in messageCollection)
				{
					if (msg.Level == TidyNet.MessageLevel.Error)
					{
						strTidyResult = strTidyResult + msg.ToString() + "<BR>";
					}
				}
			}
			else
			{
				strResult = strTidyResult;
			}
			
			returnValue = strResult;
			return returnValue;
		}
	}

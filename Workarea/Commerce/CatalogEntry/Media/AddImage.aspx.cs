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
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;
//using Ektron.Cms.Common.EkEnumeration;
using Ektron.Cms.Workarea;
using System.IO;


namespace Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Media
{
		
								
								
		public class ThumbnailData
		{
			
			
			private string _ImageName;
			private string _Path;
			private string _Alt;
			private string _Title;
			
			public ThumbnailData(string imageName, string path, string alt, string title)
			{
				
				_ImageName = imageName;
				_Path = path;
				_Alt = alt;
				_Title = title;
				
			}
			
			public string ImageName
			{
				get
				{
					return _ImageName;
				}
			}
			public string Path
			{
				get
				{
					return _Path;
				}
			}
			public string Alt
			{
				get
				{
					return _Alt;
				}
			}
			public string Title
			{
				get
				{
					return _Title;
				}
			}
		}
		
		public partial class AddImage : System.Web.UI.Page
		{
			
			
			#region Variables
			
			protected long _Id = 0;
			protected ContentAPI _ContentApi = new ContentAPI();
			protected EkMessageHelper _MessageHelper = null;
			protected ProductType _ProductType = null;
			protected ProductTypeData _ProductTypeData = new ProductTypeData();
			protected LibraryConfigData lib_settings_data = new Ektron.Cms.LibraryConfigData();
			protected List<ThumbnailData> _Thumbnails = new List<ThumbnailData>();
			protected CatalogEntryApi _CatalogEntryApi = new Ektron.Cms.Commerce.CatalogEntryApi();
			protected ProductData _ProductData = new ProductData();
			
			#endregion
			
			protected void Page_Init(object sender, System.EventArgs e)
			{
                _MessageHelper = _ContentApi.EkMsgRef;
                _ProductType = new ProductType(_ContentApi.RequestInformationRef);
				Response.CacheControl = "no-cache";
				Response.AddHeader("Pragma", "no-cache");
				Response.Expires = -1;
				
				if (! Ektron.Cms.DataIO.LicenseManager.LicenseManager.IsFeatureEnable(_ContentApi.RequestInformationRef, Ektron.Cms.DataIO.LicenseManager.Feature.eCommerce))
				{
					Utilities.ShowError(_ContentApi.EkMsgRef.GetMessage("feature locked error"));
				}
				
				if (!string.IsNullOrEmpty(Request.QueryString["productTypeId"]))
				{
					_Id = Convert.ToInt64(Request.QueryString["productTypeId"]);
				}
				
				lib_settings_data = this._ContentApi.GetLibrarySettings(0);
				
				SetLocalizedStrings();
				
				btnUpload.Attributes.Add("onclick", "return checkForEmptyTitleAndAlt(); return checkntoggle(document.getElementById(\'dvHoldMessage\'),document.getElementById(\'dvErrorMessage\'));");
				btnUpload.Text = this.GetMessage("upload txt");
				
				_ProductTypeData = _ProductType.GetItem(_Id);
			}
			
			protected void Page_Load(object sender, System.EventArgs e)
			{
				CheckAccess();
				
				if (! Page.IsPostBack)
				{
					if (_ProductTypeData.DefaultThumbnails.Count > 0)
					{
						rptThumbnails.DataSource = _ProductTypeData.DefaultThumbnails;
						rptThumbnails.DataBind();
					}
					else
					{
						phThumbnails.Visible = false;
					}
				}
			}
			
			private void SetLocalizedStrings()
			{
				lblTitle.Text = this.GetMessage("generic title label");
				lblAlt.Text = this.GetMessage("alt text");
				lblImage.Text = this.GetMessage("lbl full size");
				legendImage.Text = "Image";
				legendThumbnails.Text = "Thumbails";
				litImageHeader.Text = "Enter title, alt text, and file path in the table below.";
			}
			
			public string GetMessage(string MessageTitle)
			{
				return _MessageHelper.GetMessage(MessageTitle);
			}
			
			private void CheckAccess()
			{
				bool loggedIn = _ContentApi.LoadPermissions(0, "folder", 0).IsLoggedIn;
				if ((! loggedIn) || (loggedIn && (_ContentApi.RequestInformationRef.IsMembershipUser == 1)))
				{
					//user is not logged-in or is logged-in as a Membership User
					mvAddImage.SetActiveView(vwError);
				}
				else
				{
					//user is logged-in: show form
					mvAddImage.SetActiveView(vwForm);
				}
			}
			
			protected void btnUpload_Click(object sender, System.EventArgs e)
			{
				string parentId = "";
				if ((Request.QueryString["catalogid"] != null)&& Request.QueryString["catalogid"] != "")
				{
					parentId = Request.QueryString["catalogid"];
				}
				
				if (txtTitle.Text.IndexOfAny(new char[]{'<','>'}) > -1)
				{
					throw (new Ektron.Cms.Exceptions.SpecialCharactersException());
				}
				if (txtAlt.Text.IndexOfAny(new char[]{'<', '>'}) > -1)
				{
					throw (new Ektron.Cms.Exceptions.SpecialCharactersException());
				}
				
				CheckAccess();
				
				try
				{
					if (!(fuImage.PostedFile == null))
					{
						// file was sent
						HttpPostedFile myFile = fuImage.PostedFile;
						string sFileExt = "";
						
						// Get and check size of uploaded file
						int nFileLen = myFile.ContentLength;
						
						//get and check name and extension
						string sFileName = myFile.FileName;
						
						string sShortName = "";
						sFileName = sFileName.Replace("%", "");
						if (sFileName.IndexOf("\\") > -1)
						{
							string[] aFilename = sFileName.Split('\\');
							// take the very last one
							if (aFilename.Length > 0)
							{
								sFileName = aFilename[aFilename.Length - 1];
							}
						}
						
						//make safe
						sFileName = sFileName.Replace(" ", "_").Replace("\'", "");
						
						string[] aFileExt = sFileName.Split('.');
						if (aFileExt.Length > 1)
						{
							sFileExt = (string) (aFileExt[(aFileExt.Length - 1)].Trim().ToLower()); //use the LAASSTT one.
							if (sFileExt == "tif" || sFileExt == "bmp")
							{
								throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
							}
							sShortName = sFileName.Substring(0, System.Convert.ToInt32(sFileName.Length - (sFileExt.Length + 1)));
						}
						else
						{
							throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
						}
						
						if (aFileExt.Length > 0)
						{
							bool bGo = false;
							for (int i = 0; i <= (aFileExt.Length - 1); i++)
							{
								if (sFileExt == aFileExt[i].Trim().ToLower())
								{
									bGo = true;
									break;
								}
							}
							if (bGo == false)
							{
								throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
							}
						}
						else
						{
							throw (new Exception("The extension \"" + sFileExt + "\" is not allowed."));
						}
						
						// Allocate a buffer for reading of the file
						byte[] myData = new byte[nFileLen + 1];
						
						// Read uploaded file from the Stream
						myFile.InputStream.Read(myData, 0, nFileLen);
						
						//check for existence of file.
						FileInfo CheckFile;
						int iUnqueNameIdentifier = 0;
						string uploadImagePath = _ContentApi.GetPathByFolderID(Convert.ToInt64(parentId));
						string sWebPath = (string) (lib_settings_data.ImageDirectory.TrimEnd("/".ToCharArray()) + uploadImagePath.Replace("\\", "/") .Replace(" ", "_") .Replace(".", "") .TrimEnd("/".ToCharArray()));
						string sPhysicalPath = Server.MapPath(sWebPath);
						
						if (! Directory.Exists(sPhysicalPath))
						{
							Directory.CreateDirectory(sPhysicalPath);
						}
						
						CheckFile = new FileInfo(sPhysicalPath + "\\" + sFileName);
						if (CheckFile.Exists)
						{
							while (CheckFile.Exists)
							{
								iUnqueNameIdentifier++;
								sFileName = sShortName + "(" + iUnqueNameIdentifier + ")." + sFileExt;
								CheckFile = new FileInfo(sPhysicalPath + sFileName);
							}
						}
						
						//write
						WriteToFile(sPhysicalPath + "\\" + sFileName, myData);
						Utilities.ProcessThumbnail(sPhysicalPath, sFileName);
						
						//Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
						if (_ProductTypeData.DefaultThumbnails.Count > 0)
						{
							EkFileIO thumbnailCreator = new EkFileIO();
							bool thumbnailResult = false;
							string sourceFile = sPhysicalPath + "\\" + sFileName;
							foreach (ThumbnailDefaultData thumbnail in _ProductTypeData.DefaultThumbnails)
							{
								string fileNameNoExtension = sFileName.Replace(System.IO.Path.GetExtension(sFileName), "");
								string fileNameExtension = System.IO.Path.GetExtension(sFileName);
								string thumbnailFile = sPhysicalPath + "\\" + "thumb_" + fileNameNoExtension + thumbnail.Title.Replace("[filename]", "") + fileNameExtension;
								thumbnailResult = thumbnailCreator.CreateThumbnail(sourceFile, thumbnailFile, thumbnail.Width, thumbnail.Height);
								
								//766 load balancing handled by service - no code needed for load balancing
								
								_Thumbnails.Add(new ThumbnailData((string) ("thumb_" + fileNameNoExtension + thumbnail.Title.Replace("[filename]", "") + fileNameExtension), (string) (sWebPath.TrimEnd("/".ToCharArray()) + "/"), fileNameNoExtension + thumbnail.Title.Replace("[filename]", "") + fileNameExtension, fileNameNoExtension + thumbnail.Title.Replace("[filename]", "") + fileNameExtension));
							}
						}
						//Ends : Generate Thumbnails.
						
						//upload this file to library.
						LibraryData librarydata = new LibraryData();
						long libraryId = 0;
						librarydata.FileName = (string) ((sWebPath.Replace("/\\", "\\") + "/" + sFileName).Replace(" ", "_"));
						librarydata.Type = "images";
						if (txtTitle.Text == "")
						{
							librarydata.Title = sShortName;
						}
						else
						{
							librarydata.Title = (string) txtTitle.Text;
						}
						librarydata.ParentId = Convert.ToInt64(parentId);
						libraryId = _ContentApi.AddLibraryItem(ref librarydata);

                        LibraryData retLibraryData = _ContentApi.GetLibraryItemByID(libraryId, Convert.ToInt64(parentId));
						
						//Uploading image to libray ends.
						System.Drawing.Bitmap imageUpload;
						imageUpload = new System.Drawing.Bitmap(sPhysicalPath + "\\" + sFileName);
						
						// Add media image
						ltrAddMediaJS.Text += "var newImageObj = {";
						ltrAddMediaJS.Text += "        id: \"" + libraryId.ToString() + "\",";
						ltrAddMediaJS.Text += "        title: \"" + retLibraryData.Title + "\",";
						ltrAddMediaJS.Text += "        altText: \"" + txtAlt.Text + "\",";
						ltrAddMediaJS.Text += "        path: \"" + librarydata.FileName + "\",";
						ltrAddMediaJS.Text += "        width:" + imageUpload.Width + ",";
						ltrAddMediaJS.Text += "        height:" + imageUpload.Height;
						
						
						int x = 0;
						if (_Thumbnails.Count > 0)
						{
							string sourceFile = sPhysicalPath + sFileName;
							ltrAddMediaJS.Text += ",";
							ltrAddMediaJS.Text += "Thumbnails: [";
							for (x = 0; x <= _Thumbnails.Count - 1; x++)
							{
								ThumbnailData thumbnail = _Thumbnails[x];
								string fileNameNoExtension = sFileName.Replace(System.IO.Path.GetExtension(sFileName), "");
								string fileNameExtension = System.IO.Path.GetExtension(sFileName);
								string thumbnailPath = retLibraryData.FileName.Replace(System.IO.Path.GetFileName(retLibraryData.FileName), "");
								string thumbnailFile = fileNameNoExtension + thumbnail.Title.Replace("[filename]", "") + fileNameExtension;
								
								ltrAddMediaJS.Text += "{";
								ltrAddMediaJS.Text += "     title: \"" + thumbnail.Title + "\",";
								ltrAddMediaJS.Text += "     imageName: \"" + thumbnail.ImageName + "\",";
								ltrAddMediaJS.Text += "     path: \"" + thumbnail.Path + "\"";
								ltrAddMediaJS.Text += "}" + Environment.NewLine;
								if (x != _Thumbnails.Count - 1)
								{
									ltrAddMediaJS.Text += "," + Environment.NewLine;
								}
								
							}
							
							ltrAddMediaJS.Text += "] " + Environment.NewLine;
							
						}
						
						ltrAddMediaJS.Text += "  }" + Environment.NewLine;
						ltrAddMediaJS.Text += "parent.Ektron.Commerce.MediaTab.Images.addNewImage(newImageObj);";
						
						//766 LOAD BALANCING HANDLED BY SERVICE
						//'----------------- Load Balance ------------------------------------------------------
						//Dim loadbalance_data As LoadBalanceData()
						//loadbalance_data = _ContentApi.GetAllLoadBalancePathsExtn(parentId, "images")
						//If (Not (IsNothing(loadbalance_data))) Then
						//    For j As Integer = 0 To loadbalance_data.Length - 1
						//        sPhysicalPath = Server.MapPath(loadbalance_data(j).Path)
						//        If (Right(sPhysicalPath, 1) <> "\") Then
						//            sPhysicalPath = sPhysicalPath & "\"
						//        End If
						//        WriteToFile(sPhysicalPath & "\" & sFileName, myData)
						//    Next
						//    'Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
						//    If _ProductTypeData.DefaultThumbnails.Count > 0 Then
						//        Dim thumbnailCreator As New EkFileIO
						//        Dim thumbnailResult As Boolean = False
						//        Dim sourceFile As String = sPhysicalPath & "\" & sFileName
						//        For Each thumbnail As ThumbnailDefaultData In _ProductTypeData.DefaultThumbnails
						//            Dim fileNameNoExtension As String = sFileName.Replace(System.IO.Path.GetExtension(sFileName), "")
						//            Dim fileNameExtension As String = System.IO.Path.GetExtension(sFileName)
						//            Dim thumbnailFile As String = sPhysicalPath & "\" & fileNameNoExtension & Replace(thumbnail.Title, "[filename]", "") & fileNameExtension
						//            thumbnailResult = thumbnailCreator.CreateThumbnail(sourceFile, thumbnailFile, thumbnail.Width, thumbnail.Height)
						//        Next
						//    End If
						//    'Ends : Generate Thumbnails.
						//End If
					}
					else
					{
						throw (new Exception("No File"));
					}
				}
				catch (Exception ex)
				{
					litError.Text = ex.Message;
					ltrErrorJS.Text += "justtoggle(document.getElementById(\'dvErrorMessage\'), true);" + Environment.NewLine;
				}
			}
			
			private void WriteToFile(string strPath, byte[] Buffer)
			{
				try
				{
					// Create a file
					FileStream newFile = new FileStream(strPath, FileMode.Create);
					
					// Write data to the file
					newFile.Write(Buffer, 0, Buffer.Length);
					
					// Close file
					newFile.Close();
				}
				catch (Exception)
				{
					Utilities.ShowError((string) ("Unable to write file " + strPath));
				}
			}
		}
		
}
						

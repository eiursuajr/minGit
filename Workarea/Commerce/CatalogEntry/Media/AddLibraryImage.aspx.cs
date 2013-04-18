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
	
	public partial class Workarea_Commerce_CatalogEntry_Media_AddLibraryImage : System.Web.UI.Page
	{
		
		
		#region Member Variables
		
		protected long _ImageId = 0;
		protected long _ProductId = 0;
		protected ContentAPI _ContentApi = new ContentAPI();
		protected EkMessageHelper _MessageHelper = null;
		protected ProductType _ProductType = null;
		protected ProductTypeData _ProductTypeData = new ProductTypeData();
		protected LibraryConfigData _LibraryConfigData = new Ektron.Cms.LibraryConfigData();
		protected CatalogEntryApi _CatalogEntryApi = new Ektron.Cms.Commerce.CatalogEntryApi();
		protected ProductData _ProductData = new ProductData();
		protected List<ThumbnailData> _Thumbnails = new List<ThumbnailData>();
		
		#endregion
		
		#region Events
		
		protected void Page_Init(object sender, System.EventArgs e)
		{
            _MessageHelper = _ContentApi.EkMsgRef;
            _ProductType = new ProductType(_ContentApi.RequestInformationRef);
			_LibraryConfigData = this._ContentApi.GetLibrarySettings(0);
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			try
			{
				_ImageId = long.Parse(Request.QueryString["imageId"]);
				_ProductTypeData = _ProductType.GetItem(long.Parse(Request.QueryString["productTypeId"]));
				
				LibraryData imageData = _ContentApi.GetLibraryItemByID_UnAuth(_ImageId);
				FolderData folderData = _ContentApi.GetFolderById(imageData.ParentId);
				
				if (folderData.PrivateContent == false)
				{
					string uploadImagePath = _ContentApi.GetPathByFolderID(_ProductData.Id);
					
					//Dim sPhysicalPath As String = Server.MapPath(sWebPath)
					
					string sFileName = imageData.FileName.Substring(System.Convert.ToInt32(imageData.FileName.LastIndexOf("/") + 1), System.Convert.ToInt32((imageData.FileName.Length - 1) - imageData.FileName.LastIndexOf("/")));
					string sWebPath = imageData.FileName.Replace(sFileName, string.Empty);
					sWebPath = sWebPath.Replace("//", "/");
					string sPhysicalPath = Server.MapPath(imageData.FileName.Replace(sFileName, ""));
					
					
					//Begins: Generate thumbnails. Generates thumbnails for various pixes sizes.
					if (_ProductTypeData.DefaultThumbnails.Count > 0)
					{
						EkFileIO thumbnailCreator = new EkFileIO();
						bool thumbnailResult = false;
						//Dim sourceFile As String = Server.MapPath(_LibraryConfigData.ImageDirectory) & sFileName
						string sourceFile = Server.MapPath(imageData.FileName);
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
					
					System.Drawing.Bitmap libraryImage;
					string libraryPhysicalPath = Server.MapPath(_LibraryConfigData.ImageDirectory);
					libraryImage = new System.Drawing.Bitmap(Server.MapPath(imageData.FileName));
					
					// Add media image
					this.litAddMediaJS.Text += "<script type=\"text/javascript\">" + Environment.NewLine;
					this.litAddMediaJS.Text += "  var newImageObj = {" + Environment.NewLine;
					this.litAddMediaJS.Text += "      id: \"" + imageData.Id.ToString() + "\"," + Environment.NewLine;
					this.litAddMediaJS.Text += "      title: \"" + imageData.Title + "\"," + Environment.NewLine;
					this.litAddMediaJS.Text += "      altText: \"" + imageData.Title + "\"," + Environment.NewLine;
					this.litAddMediaJS.Text += "      path: \"" + imageData.FileName.Replace("//", "/") + "\"," + Environment.NewLine;
					this.litAddMediaJS.Text += "      width:" + libraryImage.Width + "," + Environment.NewLine;
					this.litAddMediaJS.Text += "      height:" + libraryImage.Height;
					
					
					int i = 0;
					if (_Thumbnails.Count > 0)
					{
						string sourceFile = sPhysicalPath + sFileName;
						this.litAddMediaJS.Text += "," + Environment.NewLine;
						this.litAddMediaJS.Text += "     Thumbnails: [" + Environment.NewLine;
						for (i = 0; i <= _Thumbnails.Count - 1; i++)
						{
							this.litAddMediaJS.Text += "         {";
							this.litAddMediaJS.Text += "             title: \"" + _Thumbnails[i].Title + "\"," + Environment.NewLine;
							this.litAddMediaJS.Text += "             imageName: \"" + _Thumbnails[i].ImageName + "\"," + Environment.NewLine;
							this.litAddMediaJS.Text += "             path: \"" + _Thumbnails[i].Path + "\"" + Environment.NewLine;
							this.litAddMediaJS.Text += "         }";
							if (i != _Thumbnails.Count - 1)
							{
								this.litAddMediaJS.Text += "," + Environment.NewLine;
							}
						}
						this.litAddMediaJS.Text += Environment.NewLine + "] " + Environment.NewLine;
					}
					
					this.litAddMediaJS.Text += "}" + Environment.NewLine;
					this.litAddMediaJS.Text += "parent.CommerceMediaTabAddLibraryImage(newImageObj);";
					this.litAddMediaJS.Text += "</script>";
					
				}
				else
				{
					
					this.litAddMediaJS.Text += "<script type=\"text/javascript\">" + Environment.NewLine;
					this.litAddMediaJS.Text += "alert(\'Images in this folder are private and cannot be added to a catalog\');";
					this.litAddMediaJS.Text += "</script>";
					
				}
				
				
			}
			catch (Exception ex)
			{
				string reason = ex.Message;
			}
		}
		
		
		#endregion
		
		#region Helpers
		
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
		
		#endregion
		
	}
	


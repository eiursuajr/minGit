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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Ektron.Cms;

	public partial class autoupload : System.Web.UI.Page
	{
		
		protected ContentAPI m_refContentApi = new ContentAPI();
		protected LibraryConfigData settingsCmsLibrary;
		protected long currentUserID = 0;
		protected LoadBalanceData[] load_balance_data;
		
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			try
			{
				currentUserID = m_refContentApi.UserId;
				string uploadType;
				uploadType = (string) (Request.Form["actiontyp"].ToString()); // Tells what this upload is.  This is either "uploadfile" or "uploadcontent"
				switch (uploadType)
				{
					case "uploadfile":
						ReceiveSubmittedFiles();
						break;
					case "uploadcontent":
						ReceiveSubmittedContent();
						break;
				}
			}
			catch (Exception ex)
			{
				Response.Write(ex.Message);
			}
		}
		
		public void ReceiveSubmittedContent()
		{
			System.Text.StringBuilder responseToClient = new System.Text.StringBuilder();
			responseToClient.Append("<h2>Automatic Content Upload Not Allowed</h2>");
			responseToClient.Append("<p>The CMS400 is not configured to receive content uploaded through this mechanism.</p>");
			responseToClient.Append("<p>You must use the Publish, Check-In, or Save buttons in the content edit page to save edited content.</p>");
			responseToClient.Append("<p>Click on the \'Undo\' button to restore your edited content.</p>");
			Response.Write(responseToClient.ToString());
		}
		
		public string SaveFileToPath(string filePhysPath, string fileName, bool overwrite)
		{
			string pathSaveFile;
			long counterName;
			string nameNew;
			string pathUpload;
			
			pathUpload = filePhysPath;
			Directory.CreateDirectory(pathUpload); // ensure that it exists
			pathSaveFile = CatPath(pathUpload, fileName, false);
			if (false == overwrite)
			{
				counterName = 0;
				while (File.Exists(pathSaveFile))
				{
					counterName++;
					nameNew = (string) (Path.GetFileNameWithoutExtension(fileName) + "(" + counterName.ToString() + ")" + Path.GetExtension(fileName));
					pathSaveFile = CatPath(pathUpload, nameNew, false);
				}
			}
			
			// If it exists, get around overwrite issues by deleting the file, if it exists.
			if (File.Exists(pathSaveFile))
			{
				File.Delete(pathSaveFile);
			}
			Request.Files["uploadfilephoto"].SaveAs(pathSaveFile);
			return pathSaveFile;
			
		}
		
		public string SimplifyFileName(string nameData)
		{
			int idx;
			int ch;
			string strRet = "";
			const string strReplaceChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_";
			int iMax = strReplaceChars.Length;
			
			for (idx = 0; idx <= nameData.Length - 1; idx++)
			{
				ch = Strings.AscW(nameData[idx]);
				if (ch >= 128)
				{
					// get a good replacement character
					ch = ch - 128;
					while (ch >= iMax)
					{
						ch -= iMax;
					}
					if (ch < 0)
					{
						ch = 0;
					}
					strRet = strRet + strReplaceChars[ch];
				}
				else
				{
					strRet = strRet + nameData[idx];
				}
			}
			
			return strRet;
		}
		
		public string MakeStandardUploadThumbnail(string uploadFilePath)
		{
			int resizeWidth = 125;
			int resizeHeight = 125;
			string originalFile;
			string changeImagePathFile;
			System.Drawing.Image img;
			
			originalFile = uploadFilePath;
			changeImagePathFile = CatPath(Path.GetDirectoryName(uploadFilePath), (string) ("thumb_" + Path.GetFileName(uploadFilePath)), false);
			
			img = System.Drawing.Image.FromFile(originalFile);
			
			// These need to be double for the calculation.
			double dOx;
			double dOy;
			double dRx;
			double dRy;
			dOx = img.Width;
			dOy = img.Height;
			dRx = resizeWidth;
			dRy = resizeHeight;
			
			// The greater side is the governing side.
			if (dOx > dOy)
			{
				resizeHeight = Convert.ToInt32((dRx * dOy) / dOx);
			}
			else
			{
				resizeWidth = Convert.ToInt32((dRy * dOx) / dOy);
			}
			
			System.Drawing.Image.GetThumbnailImageAbort imgCallBack;
			imgCallBack = new System.Drawing.Image.GetThumbnailImageAbort(ImageResizeCallback);
			
			System.Drawing.Image imgThumbnail;
			imgThumbnail = img.GetThumbnailImage(resizeWidth, resizeHeight, imgCallBack, IntPtr.Zero);
			
			img.Dispose(); // Free up the file
			imgThumbnail.Save(changeImagePathFile);
			
			imgThumbnail.Dispose();
			
			return changeImagePathFile;
		}
		
		public bool ImageResizeCallback()
		{
			// Cool
			return false;
		}
		
		public string CatPath(string strBegin, string strEnd, bool bFinalSlash)
		{
			string strFinalPath;
			strFinalPath = "";
			
			// Normalize
			strBegin = strBegin.Replace("\\", "/");
			strBegin = strBegin.Trim();
			strEnd = strEnd.Replace("\\", "/");
			strEnd = strEnd.Trim();
			
			// This puts them together, ensuring there is at least one slash between
			strFinalPath = strBegin + "/" + strEnd;
			if (bFinalSlash)
			{
				strFinalPath = strFinalPath + "/";
			}
			
			// This will fix if there are double slashes together.
			strFinalPath = strFinalPath.Replace("//", "/");
			strFinalPath = strFinalPath.Replace("//", "/");
			
			return strFinalPath;
		}
		
		public void ReceiveSubmittedFiles()
		{
			string errorMessage = "";
			long errorValue = 0;
			long sizeFile;
			string clientFileName = "";
			string clientFileExtension = "";
			string uploadedFilePhysPath = "";
			string altTitleFile = "";
			string typeFile = "";
			long idCmsFolder = 0;
			string[] fileExtnList;
			int i = 0;
			string uploadUrlPath = "";
			string uploadPhysicalPath = "";
			string strTmpServerInfo = "";
			string typeInCmsLib = "";
			Ektron.Cms.Library.EkLibrary libraryCmsObj;
			bool ret = false;
			string metaDataList = "";
			Collection cMetaColl = new Collection();
			string strName = "";
			int lLoop;
			int lInnerLoop;
			string teaserStoreForFile = "";
			string[] teaserListByFile;
			string teaserForFile;
			string teaserFileNormalized;
			string tempTeaser = "";
			bool thisTeaser;
			string strJustFileName = "";
			string clientFilePath;
			bool isUploadOK;
			

			typeFile = Request.Form["file_type"].ToString();
			sizeFile = Convert.ToInt64(Request.Form["file_size"]);
			altTitleFile = Request.Form["file_title"].ToString();
			
			clientFilePath = Request.Files["uploadfilephoto"].FileName.ToString();
			clientFileName = SimplifyFileName(Path.GetFileName(clientFilePath));
			
			// Non-standard (CMS specific) values.
			idCmsFolder = Convert.ToInt64(Request.Form["folder_id"]);
			
			//get the metadata, not critical field so do not error out if it does not exist
			metaDataList = Request.Form["custom_field_meta"];
			
			//get the teaser, not critical field so do not error out if it does not exist
			teaserStoreForFile = Request.Form["custom_field_teaser"];
			if (teaserStoreForFile == null)
			{
				teaserStoreForFile = "";
			}
			else
			{
				teaserStoreForFile = teaserStoreForFile.Replace("<p> </p>" + "\r\n", "<p>&#160;</p>");
			}
			
			thisTeaser = false;
			teaserListByFile = teaserStoreForFile.Split("|-|".ToCharArray()[0]);
			for (lLoop = 0; lLoop <= (teaserListByFile.Length - 1); lLoop++)
			{
				teaserForFile = teaserListByFile[lLoop];
				if (thisTeaser == true)
				{
					tempTeaser = teaserListByFile[lLoop];
					break;
				}
				teaserFileNormalized = teaserForFile.Replace(" ", "_");
				if (teaserFileNormalized == clientFileName)
				{
					thisTeaser = true;
				}
			}
			
			teaserStoreForFile = tempTeaser;
			
			//get the extension from the filename
			clientFileExtension = (string) (Path.GetExtension(clientFileName).ToLower().TrimStart('.'.ToString().ToCharArray()));
			
			//get the library settings- extensions and image paths
			settingsCmsLibrary = m_refContentApi.GetLibrarySettings(idCmsFolder);
			
			//figure out the fileType
			typeFile = "none";
			
			//loop through the image extensions
            fileExtnList = settingsCmsLibrary.ImageExtensions.Split(new char[] { ',' });
			for (i = 0; i <= fileExtnList.Length - 1; i++)
			{
				if (clientFileExtension == fileExtnList[i].ToLower().Trim())
				{
					typeFile = "images";
					//needed to get correct lb paths
					typeInCmsLib = "images";
					//get correct path- cannot have a slash at the end
					uploadUrlPath = MakeUploadPath((string) settingsCmsLibrary.ImageDirectory);
					break;
				}
			}
			
			//if not an image, loop through the file extensions
			if (typeFile == "none")
			{
                fileExtnList = settingsCmsLibrary.FileExtensions.Split(new char[] { ',' });
				for (i = 0; i <= fileExtnList.Length - 1; i++)
				{
					if (clientFileExtension == fileExtnList[i].ToLower().Trim())
					{
						typeFile = "files";
						//needed to get correct lb paths
						typeInCmsLib = "files";
						//get correct path- cannot have a slash at the end
						uploadUrlPath = MakeUploadPath((string) settingsCmsLibrary.FileDirectory);
						break;
					}
				}
			}
			
			uploadPhysicalPath = Server.MapPath(uploadUrlPath);
			
			PermissionData cPerms;
			
			//if the file is one of the allowed fileTypes, check permissions and continue
			if (typeFile != "none")
			{
				//check the permissions first
				cPerms = m_refContentApi.LoadPermissions(idCmsFolder, "folder",0);
				//if the uploader has the correct permission, upload the file
				if (((typeFile == "images") && (cPerms.CanAddToImageLib)) || ((typeFile == "files") && (cPerms.CanAddToFileLib)))
				{
					//upload the file making it unique if there is already one there
					isUploadOK = false;
					uploadedFilePhysPath = SaveFileToPath(uploadPhysicalPath, clientFileName, false);
					if (uploadedFilePhysPath.Length > 0)
					{
						strJustFileName = Path.GetFileName(uploadedFilePhysPath);
						if ("images" == typeFile)
						{
							if (MakeStandardUploadThumbnail(uploadedFilePhysPath).Length == 0)
							{
								errorValue = 4;
								errorMessage = "The server was not able to produce a thumbnail for the file \'" + strJustFileName + "\'\'.\\n\\nThe user may not have write permission to the \'" + uploadUrlPath + "\' folder on the server or there may not be enough resources on the server to produce a thumbnail.\\n\\nPlease see your site administrator to resolve the issue.";
							}
						}
					}
					else
					{
						errorValue = 3;
						errorMessage = "Error performing the upload onto the server system.\\n\\nThe user may not have write permission to the \'" + uploadUrlPath + "\' folder on the server.\\n\\nPlease see your administrator to modify the server security to allow uploads at this location.";
					}
					
					if (0 == errorValue)
					{
						//if the main file uploaded fine, upload to all the load balance folders. Get the extra load balance paths
						load_balance_data = m_refContentApi.GetAllLoadBalancePathsExtn(idCmsFolder, typeInCmsLib);
						
						//if there are load balance paths
						if (!(load_balance_data == null))
						{
							string pathPhysLoadBalanceUpload = "";
							
							//loop through each of the paths, uploading the file to those locations
							for (i = 0; i <= load_balance_data.Length - 1; i++) //Each lbObj In extraPaths
							{
								isUploadOK = false;
								pathPhysLoadBalanceUpload = Server.MapPath(MakeUploadPath((string) (load_balance_data[i].Path)));
								if (SaveFileToPath(pathPhysLoadBalanceUpload, strJustFileName, true).Length > 0)
								{
									if ("images" == typeFile)
									{
										isUploadOK = System.Convert.ToBoolean(MakeStandardUploadThumbnail(CatPath(pathPhysLoadBalanceUpload, strJustFileName, false)).Length > 0);
									}
									else
									{
										isUploadOK = true;
									}
								}
								if (false == isUploadOK)
								{
									//If the load balance upload fails then notify the admin and keep going								
									string eTmp = "An upload failure has occured in the load balancing.  Load balancing path = \'" + pathPhysLoadBalanceUpload + "\'.  Uploaded Filename  = \'" + CatPath(pathPhysLoadBalanceUpload, strJustFileName, false) + "\'.";
                                    ret = System.Convert.ToBoolean(m_refContentApi.EkContentRef.ErrStatusToAdminGroup(eTmp, "Load Balance Error Report"));
								}
							}
						}
						
						//create an object with appropriate info to send to assembly to insert into the database
						Collection cLibrary = new Collection();
						cLibrary.Add(idCmsFolder, "ParentID", null, null);
						cLibrary.Add("", "LibraryID", null, null);
						cLibrary.Add(typeFile, "LibraryType", null, null);
						if (altTitleFile.Length >= 150)
						{
							cLibrary.Add(altTitleFile.Substring(0, 150), "LibraryTitle", null, null);
						}
						else
						{
							cLibrary.Add(altTitleFile, "LibraryTitle", null, null);
						}
						cLibrary.Add("", "ContentID", null, null);
						cLibrary.Add(currentUserID, "UserID", null, null);
						cLibrary.Add(CatPath(uploadUrlPath, strJustFileName, false), "LibraryFilename", null, null);
						cLibrary.Add("makeunique", "TitleConflict", null, null);
						
						cLibrary.Add(teaserStoreForFile, "ContentTeaser", null, null);
						
						if (!(metaDataList == null))
						{
							bool isData = false;
							bool metaThis = false;
							int countMeta = 1;
							string[] metaByFileList;
							string strMetaByFile;
							string[] metaItemList = new string[4];
							string strMeta;
							string[] metaListFromByFile;
							
							metaByFileList = metaDataList.Split("|-|".ToCharArray()[0]);
							for (lLoop = 0; lLoop <= (metaByFileList.Length - 1); lLoop++)
							{
								strMetaByFile = metaByFileList[lLoop];
								if (metaThis == true)
								{
									metaThis = false;
									metaListFromByFile = strMetaByFile.Split("@@ekt@@".ToCharArray()[0]);
									for (lInnerLoop = 0; lInnerLoop <= (metaListFromByFile.Length - 1); lInnerLoop++)
									{
										strMeta = metaListFromByFile[lInnerLoop];
										if (isData == true)
										{
											isData = false;
											metaItemList[1] = strName;
											metaItemList[2] = "0";
											metaItemList[3] = strMeta;
											cMetaColl.Add(metaItemList, countMeta.ToString(), null, null);
											metaItemList = new string[4];
											countMeta++;
										}
										else if (strMeta != "")
										{
											isData = true;
											strName = strMeta;
										}
									}
								}
								teaserFileNormalized = strMetaByFile.Replace(" ", "_");
								if (teaserFileNormalized == clientFileName)
								{
									metaThis = true;
								}
							}
						}
						cLibrary.Add(cMetaColl, "ContentMetadata", null, null);
						
						libraryCmsObj = m_refContentApi.EkLibraryRef;
						ret = System.Convert.ToBoolean(libraryCmsObj.AddLibraryItemv2_0(cLibrary,m_refContentApi.RequestInformationRef.CallerId));
						
						if (true == System.Convert.ToBoolean(Request.ServerVariables["SERVER_PORT_SECURE"]))
						{
							strTmpServerInfo = "https://";
						}
						else
						{
							strTmpServerInfo = "http://";
						}
						strTmpServerInfo = strTmpServerInfo + Request.ServerVariables["SERVER_NAME"];
						if (Request.ServerVariables["SERVER_PORT"] != "80")
						{
							strTmpServerInfo = strTmpServerInfo + ":" + Request.ServerVariables["SERVER_PORT"];
						}
						
					}
					
				}
				else
				{

					errorValue = 1;
					errorMessage = "User does not have CMS permeissions to place files into the selected folder.\\n\\nPlease see your site administrator for modifying the permissions to allow this operation.";
					return;
				}
			}
			else
			{
				errorValue = 2;
				errorMessage = "Invalid extension for the file " + clientFileName + ".\\n\\nPlease select a valid file type or contact your administrator to add this type to the list of valid files allowed in the upload process.";
			} 
			
			System.Text.StringBuilder rData = new System.Text.StringBuilder();
			rData.Append("<XML ID=EktronFileIO>" + "\r\n");
			rData.Append("<UPLOAD>" + "\r\n");
			rData.Append("<FILEINFO ID=\"5\" discard=\"False\">" + "\r\n");
			rData.Append("<FSRC>" + clientFilePath + "</FSRC>" + "\r\n");
			if (0 == errorValue)
			{
				rData.Append("<FURL>" + strTmpServerInfo + CatPath(uploadUrlPath, strJustFileName, false) + "</FURL>" + "\r\n");
			}
			else
			{
				rData.Append("<FURL></FURL>" + "\r\n");
			}
			rData.Append("<FID></FID>" + "\r\n");
			rData.Append("<FSIZE>" + System.Convert.ToInt32("&h" + sizeFile) + "</FSIZE>" + "\r\n");
			rData.Append("<DESC>" + altTitleFile + "</DESC>" + "\r\n");
			rData.Append("<THUMBURL></THUMBURL>" + "\r\n");
			rData.Append("<THUMBHREF></THUMBHREF>" + "\r\n");
			rData.Append("<FTYPE>" + typeFile + "</FTYPE>" + "\r\n");
			rData.Append("<DWIDTH>0</DWIDTH>" + "\r\n");
			rData.Append("<DHEIGHT>0</DHEIGHT>" + "\r\n");
			rData.Append("<DBORDER>0</DBORDER>" + "\r\n");
			rData.Append("<FRAGMENT></FRAGMENT>" + "\r\n");
			if (0 == errorValue)
			{
				rData.Append("<FERROR value=\"0\"></FERROR>" + "\r\n");
			}
			else
			{
				rData.Append("<FERROR value=\"" + errorValue.ToString() + "\">" + errorMessage + "</FERROR>" + "\r\n");
			}
			rData.Append("</FILEINFO>" + "\r\n");
			rData.Append("</UPLOAD>" + "\r\n");
			rData.Append("</XML>" + "\r\n");
			Response.Write(rData.ToString());
			
		}
		private bool EnsureFilePathExists(string path)
		{
			// A return of True is an error, to be consistent
			bool bRet = false;
			try
			{
				string strNewPath;
				System.IO.DirectoryInfo objSyst;
				
				strNewPath = path.Substring(0, path.LastIndexOf("\\"));
				
				if (strNewPath.Length > 0)
				{
					objSyst = new System.IO.DirectoryInfo(strNewPath);
					if (false == objSyst.Exists)
					{
						objSyst.Create();
					}
					objSyst = null;
				}
			}
			catch
			{
				bRet = true;
			}
			return bRet;
		}
		private string MakeUploadPath(string path)
		{
			string returnValue;
			if (path.Substring(path.Length - 1, 1) == "/")
			{
				returnValue = path.Substring(0, path.Length - 1);
			}
			else
			{
				returnValue = path;
			}
			return returnValue;
		}
		private string ReportError(string ErrorCode, string ErrorDescription)
		{
			System.Text.StringBuilder err = new System.Text.StringBuilder();
			err.Append("<XML ID=EktronFileIO>" + "\r\n");
			err.Append("<?xml version=\"1.0\"?>" + "\r\n");
			err.Append("<UPLOAD>" + "\r\n");
			err.Append("<FILEINFO ID=\"5\" discard=\"False\">" + "\r\n");
			err.Append("<FSRC></FSRC>" + "\r\n");
			err.Append("<FURL></FURL>" + "\r\n");
			err.Append("<FID></FID>" + "\r\n");
			err.Append("<FSIZE></FSIZE>" + "\r\n");
			err.Append("<DESC></DESC>" + "\r\n");
			err.Append("<THUMBURL></THUMBURL>" + "\r\n");
			err.Append("<THUMBHREF></THUMBHREF>" + "\r\n");
			err.Append("<FTYPE></FTYPE>" + "\r\n");
			err.Append("<DWIDTH>0</DWIDTH>" + "\r\n");
			err.Append("<DHEIGHT>0</DHEIGHT>" + "\r\n");
			err.Append("<DBORDER>0</DBORDER>" + "\r\n");
			err.Append("<FRAGMENT></FRAGMENT>" + "\r\n");
			err.Append("<FERROR value=\"" + ErrorCode + "\">" + ErrorDescription + "</FERROR>" + "\r\n");
			err.Append("</FILEINFO>" + "\r\n");
			err.Append("</UPLOAD>" + "\r\n");
			err.Append("</XML>" + "\r\n");
			return (err.ToString());
		}
		
		private string GetLocalFilename(object LocalDirectory, ref object strFilename, string strHandleConflict, ref object ErrorCode)
		{
			string returnValue;
			string strLocalFilename;
			string[] strTmpFilename;
			string[] strSplitFilename;
			int iUnqueNameIdentifier;
			string strExtension;
			string strTmpLocalDir = "";
			try
			{
				ErrorCode = 0;
				
				strFilename = Strings.Replace(strFilename.ToString(), "/", "\\", 1, -1, 0);
				strTmpFilename = Strings.Split(strFilename.ToString(), "\\", -1, 0);
				if ((Strings.Right(LocalDirectory.ToString(), 1) != "\\"))
				{
					strTmpLocalDir = (string) (LocalDirectory + "\\");
				}
				
				strLocalFilename = strTmpLocalDir + HttpUtility.UrlEncodeUnicode(strTmpFilename[(strTmpFilename.Length - 1)]);
				
				if (strHandleConflict == "overwrite")
				{
					if (FileExists(strLocalFilename))
					{
						FileSystem.Kill(strLocalFilename);
					}
					returnValue = strLocalFilename;
				}
				else if (strHandleConflict == "makeunique")
				{
					iUnqueNameIdentifier = 1;
					if (FileExists(strLocalFilename))
					{
						strSplitFilename = (strTmpFilename[(strTmpFilename.Length - 1)]).Split('.');
						if ((strSplitFilename.Length - 1) > 0)
						{
							strSplitFilename[0] = Strings.Left(strTmpFilename[(strTmpFilename.Length - 1)], System.Convert.ToInt32(strTmpFilename[(strTmpFilename.Length - 1)].Length - (strSplitFilename[(strSplitFilename.Length - 1)].Length + 1)));
							strExtension = strSplitFilename[(strSplitFilename.Length - 1)];
						}
						else
						{
							strExtension = "";
						}
						strLocalFilename = strTmpLocalDir + strSplitFilename[0] + "(" + iUnqueNameIdentifier + ")" + "." + strExtension;
						while (FileExists(strLocalFilename))
						{
							iUnqueNameIdentifier++;
							strLocalFilename = strTmpLocalDir + strSplitFilename[0] + "(" + iUnqueNameIdentifier + ")" + "." + strExtension;
						}
					}
					returnValue = strLocalFilename;
				}
				else
				{
					returnValue = strLocalFilename;
				}
				return returnValue;
				
			}
			catch (Exception ex)
			{
                returnValue = ex.InnerException.ToString() ;
			}
			return returnValue;
		}
		
		private bool FileExists(string Filename)
		{
			bool returnValue;
			try
			{
				returnValue = FileSystem.FileLen(Filename) >= 0;
			}
			catch (Exception)
			{
				returnValue = false;
			}
			return returnValue;
		}
	}
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
using Ektron.Cms;
using Ektron.ASM.AssetConfig;
using AssetManagement;
using Ektron.Cms.Common;

	public partial class Workarea_ProcessMultiupload : System.Web.UI.Page
	{	
		static int bufSize = 1048576;
		private List<string> fileTypeCol;
        ErrObject err = null;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			long userId = Convert.ToInt64(CommonApi.GetEcmCookie()["user_id"]);
			int loginToken = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie()["unique_id"]);
			string rejectedFiles = "";
			if (Request.Files != null&& Request.Files.Count > 0)
			{
				List<AssetFileData> assetIdFileNameList = new List<AssetFileData>();
				string typesStr = DocumentManagerData.Instance.FileTypes.Replace("*", "");
                string filetypes = "";
                if (Request.QueryString["isimage"] != null && Request.QueryString["isimage"] == "1")
                {
                    string[] AllowedFileTypes = null;
                    if (DocumentManagerData.Instance.FileTypes.Length > 0)
                    {
                        AllowedFileTypes = DocumentManagerData.Instance.FileTypes.Split(',');
                        if (AllowedFileTypes != null && AllowedFileTypes.Length > 0)
                        {
                            foreach (string filetype in AllowedFileTypes)
                            {
                                if (EkFunctions.IsImage(filetype.Trim().Replace("*", "")))
                                {
                                    if (filetypes.Length > 0)
                                        filetypes += "," + filetype;
                                    else
                                        filetypes = filetype;
                                }
                            }
                        }
                    }
                    fileTypeCol = new List<string>(filetypes.ToString().Replace("*","").Split(",".ToCharArray()));
                }
                else
                {
                    fileTypeCol = new List<string>(typesStr.ToString().Split(",".ToCharArray()));
                }
				
				for (int fileIndex = 0; fileIndex <= Request.Files.Count - 1; fileIndex++)
				{
					HttpPostedFile postedFile = Request.Files[fileIndex];
					Context.Cache[userId + loginToken + "RejectedFiles"] = "";
                    if (ValidFileType(Path.GetExtension(postedFile.FileName)) && IsValidFileName(Path.GetFileNameWithoutExtension(postedFile.FileName)))
					{
						try
						{
                            AssetFileData fileData = new AssetFileData();
							string assetId = System.Guid.NewGuid().ToString();
							string docFilePath = DocumentManagerData.Instance.WebSharePath;
							if (! System.IO.Path.IsPathRooted(docFilePath))
							{
								docFilePath = Ektron.ASM.AssetConfig.Utilities.UrlHelper.GetAppPhysicalPath() + docFilePath;
							}
							string destFileName = docFilePath + Path.GetFileName(postedFile.FileName) + assetId;
							
							using (BinaryReader br = new BinaryReader(postedFile.InputStream))
							{
								byte[] buf;
								using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(destFileName, FileMode.Create)))
								{
									buf = br.ReadBytes(bufSize);
                                    if (Ektron.Cms.Common.EkFunctions.IsImage(Path.GetExtension(postedFile.FileName)))
                                    {
                                        Stream streamTemp = new MemoryStream(buf);
                                        if (!Ektron.Cms.Common.EkFunctions.isImageStreamValid(streamTemp))
                                        {
                                            if (rejectedFiles.Length > 0)
                                            {
                                                rejectedFiles += (string)(", " + postedFile.FileName);
                                            }
                                            else
                                            {
                                                rejectedFiles = postedFile.FileName;
                                            }
                                            continue;
                                        }
                                    }

									int index = 0;
									while (buf.Length > 0)
									{
										binaryWriter.Write(buf, 0, buf.Length);
										index += buf.Length;
										buf = br.ReadBytes(bufSize);
									}
									binaryWriter.Flush();
									binaryWriter.Close();
								}
								
								br.Close();
							}
							
							fileData.AssetId = assetId;
							fileData.FileName = postedFile.FileName;
							assetIdFileNameList.Add(fileData);
						}
						catch (Exception)
						{
							if (err.Number != 0)
							{
								if (-2147467259 == err.Number)
								{
								}
								Response.Write(err.Description);
							}
							return;
						}
					}
					else
					{
						if (rejectedFiles.Length > 0)
						{
							rejectedFiles += (string) (", " + postedFile.FileName);
						}
						else
						{
							rejectedFiles = postedFile.FileName;
						}
						
					}
				}

				if (Session[userId + loginToken + "Attachments"] == null)
				{
					Session.Add(Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments", assetIdFileNameList);
				}
				else
				{
                    Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "Attachments"] = assetIdFileNameList;
				}

                if (Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"] == null)
				{
                    Session.Add(Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles", rejectedFiles);
				}
				else
				{
                    Session[Convert.ToString(userId) + Convert.ToString(loginToken) + "RejectedFiles"] = rejectedFiles;
				}		
			}
		}
		
		private bool ValidFileType(string extension)
		{
			bool found = false;
			extension = extension.ToLower();
			foreach (string type in fileTypeCol)
			{
				if (type.Trim().ToString().ToLower() == extension)
				{
					found = true;
					break;
				}
			}
			return found;
		}

        private bool IsValidFileName(string fileName)
        {
            if (fileName.IndexOf("&") > -1 || fileName.IndexOf("+") > -1 || fileName.IndexOf("%") > -1)
            {
                return false;
            }
            return true;
        }
	}

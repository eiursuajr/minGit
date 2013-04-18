using System;
using System.Collections;
using System.IO;
using System.Web;

public class Dictionaries
{
	HttpContext context = null;
	private const string DictionaryDir = "WorkArea/Foundation/RadControls/Spell/TDF";

	public Dictionaries(HttpContext context)
	{
		this.context = context;
	}

	public string AbsoluteDictionaryFileName(string baseName)
	{
		string importedDir = DictionaryPath + "\\";
		return importedDir + baseName;
	}

	public string RelativeUrl(string baseName)
	{
		return context.Request.ApplicationPath + "/" + DictionaryDir + "/" + baseName;
	}
    
	public ArrayList DictionaryNames()
	{
		try
		{
			ArrayList baseNames = new ArrayList();
			foreach (string fullName in Directory.GetFiles(DictionaryPath, "*.tdf"))
			{
				baseNames.Add(BaseName(fullName));
			}
			return baseNames;
		}
		catch (DirectoryNotFoundException)
		{
			Directory.CreateDirectory(DictionaryPath);
			return new ArrayList();
		}
	}

	public string DictionaryPath
	{
		get { return context.Server.MapPath("~/" + DictionaryDir); }
	}

	public string BaseName(string fullName)
	{
		FileInfo file = new FileInfo(fullName);
		return file.Name;
	}
}


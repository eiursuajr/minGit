using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Telerik.WebControls.Dictionaries;

public partial class Controls_EditDictionary : System.Web.UI.UserControl
{
    private DictionaryImporter _importer = null;
    private Dictionaries dictionaries;

    private DictionaryImporter importer
    {
        get
        {
            if (_importer == null)
                _importer = ImporterForDictionary(CurrentDictionaryFile());
            return _importer;
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        dictionaries = new Dictionaries(HttpContext.Current);
        if (!Page.IsPostBack)
        {
            dictionarySelector.DataSource = dictionaries.DictionaryNames();
            dictionarySelector.DataBind();
        }

        if (dictionarySelector.Items.Count == 0)
        {
            addPanel.Visible = false;
            searchPanel.Visible = false;
            importPanel.Visible = false;
            dictionarySelector.Visible = false;
            messageLabel.Text = "No dictionaries imported.";
        }
        else
        {
            addPanel.Visible = true;
            searchPanel.Visible = true;
            importPanel.Visible = true;
        }

        //ClearWordList();
    }

    private void ReportError(string message)
    {
        messageLabel.Text = message;
    }

    protected void addButton_Click(object sender, System.EventArgs e)
    {
        try
        {
            string newWord = addWordBox.Text.Trim();
            if (newWord != string.Empty)
            {
                this.importer.AddWord(newWord);
                this.importer.Save(CurrentDictionaryFile());
                messageLabel.Text = string.Format("Word '{0}' added to the dictionary.", newWord);
            }
        }
        catch (UnauthorizedAccessException)
        {
            ReportError(string.Format("Access denied to the {0} folder.  Please grant the ASP.NET user account write permissions to the folder.", dictionaries.DictionaryPath));
        }
        catch (Exception unknownError)
        {
            ReportError(unknownError.Message);
        }

        ClearWordList();
    }

    private string CurrentDictionaryFile()
    {
        string dictName = dictionarySelector.SelectedItem.Text;
        return dictionaries.AbsoluteDictionaryFileName(dictName);
    }

    private DictionaryImporter ImporterForDictionary(string absoluteDictionaryName)
    {
        DictionaryImporter importer = new DictionaryImporter();
        importer.Load(absoluteDictionaryName);
        return importer;
    }

    protected void findButton_Click(object sender, System.EventArgs e)
    {
        string query = findWordBox.Text.Trim();
        if (query != string.Empty)
        {
            ArrayList similar = importer.Find(query);
            ClearWordList();
            wordsFound.DataSource = similar;
            wordsFound.DataBind();
        }
    }

    private void ClearWordList()
    {
        wordsFound.Items.Clear();
        wordsFound.Items.Add(new ListItem(string.Empty));
    }

    protected void deleteButton_Click(object sender, System.EventArgs e)
    {
        try
        {
            ArrayList victims = new ArrayList();
            foreach (ListItem current in wordsFound.Items)
            {
                if (current.Selected && current.Text != string.Empty)
                {
                    victims.Add(current.Text);
                }
            }

            importer.Delete(victims);
            importer.Save(CurrentDictionaryFile());

            ClearWordList();
            messageLabel.Text = "Word(s) deleted.";
        }
        catch (UnauthorizedAccessException)
        {
            ReportError(string.Format("Access denied to the {0} folder.  Please grant the ASP.NET user account write permissions to the folder.", dictionaries.DictionaryPath));
        }
        catch (Exception unknownError)
        {
            ReportError(unknownError.Message);
        }
    }

    protected void importButton_Click(object sender, System.EventArgs e)
    {
        if (importedFile.PostedFile == null)
            return;

        try
        {
            ImportUploadedFile();
        }
        catch (UnauthorizedAccessException)
        {
            messageLabel.Text = string.Format("Access denied to the {0} folder.  Please grant the ASP.NET user account write permissions to the folder.", dictionaries.DictionaryPath);
        }
        catch (Exception unknownError)
        {
            messageLabel.Text = unknownError.Message;
        }
        ClearWordList();
    }

    private void ImportUploadedFile()
    {
        using (StreamReader imported = new StreamReader(importedFile.PostedFile.InputStream))
        {
            importer.Load(imported);

            importer.Save(CurrentDictionaryFile());
        }
    }
}

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using Ektron.Cms;
using Ektron.Cms.API;

public partial class Workarea_pagebuilder_taxonomytree : System.Web.UI.UserControl
{
    public string Filter = "";
    protected SiteAPI m_refSiteApi = new SiteAPI();
    protected ContentAPI m_contentApi = new ContentAPI();
    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;

    protected void Page_Init(object sender, EventArgs e)
    {
        _forcefill = false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        m_refMsg = m_contentApi.EkMsgRef;

        if (FolderID > -1)
        {
            if (folderData != null)
            {
                if (folderData.FolderTaxonomy.Length == 0)
                {
                    noTaxonomies.Text = m_refMsg.GetMessage("generic no taxonomy");
                    noTaxonomies.ToolTip = noTaxonomies.Text;
                }

                taxRequired.InnerText = folderData.CategoryRequired.ToString().ToLower();

                taxonomies.DataSource = folderData.FolderTaxonomy;
                taxonomies.DataBind();
            }
        }

        // Register JS
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronJS);
        JS.RegisterJSInclude(this, JS.ManagedScript.EktronTreeviewJS);
        JS.RegisterJSInclude(this, m_contentApi.AppPath + "PageBuilder/taxonomytree.js", "TaxTreeJS");

        string inittree = String.Format("Ektron.TaxonomyTree.init(\"{0}\", \"{1}\");", m_contentApi.AppPath, txtselectedTaxonomyNodes.ClientID);
        try
        {
            JS.RegisterJSBlock(this, inittree, txtselectedTaxonomyNodes.ClientID + "TaxTreeInit");
        }
        catch
        {
            //we're apparently in a full postback which doesn't care for registerjsblock
            script.Text = "<script type=\"text/javascript\" defer=\"defer\"> window.setTimeout(function(){" + inittree + "}, 750); </script>";
            script.Visible = true;
        }

        // Register CSS
        Css.RegisterCss(this, Css.ManagedStyleSheet.EktronTreeviewCss);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!IsPostBack || _forcefill)
        {
            if (folderData.FolderTaxonomy.Length > 0 && defaultTaxID > -1)
            {
                //output path to selected taxonomy
                txtselectedTaxonomyNodes.Text = getPathToTaxId(defaultTaxID);
                txtselectedTaxonomyNodes.ToolTip = txtselectedTaxonomyNodes.Text;
            }
            else if (folderData.FolderTaxonomy.Length > 0 && PreselectedTaxonomies.Count > 0)
            {
                foreach (long i in PreselectedTaxonomies)
                {
                    txtselectedTaxonomyNodes.Text += getPathToTaxId(i) + "|";
                    txtselectedTaxonomyNodes.ToolTip = txtselectedTaxonomyNodes.Text;
                }
                txtselectedTaxonomyNodes.Text = txtselectedTaxonomyNodes.Text.Remove(txtselectedTaxonomyNodes.Text.Length - 1);
                txtselectedTaxonomyNodes.ToolTip = txtselectedTaxonomyNodes.Text;
            }
        }
    }

    private string getPathToTaxId(long tid){
        int langid = (m_contentApi.RequestInformationRef.ContentLanguage == -1) ? m_contentApi.RequestInformationRef.DefaultContentLanguage : m_contentApi.RequestInformationRef.ContentLanguage;

        TaxonomyRequest taxrequest = new TaxonomyRequest();
        taxrequest.IncludeItems = false;
        taxrequest.Page = Page;
        taxrequest.TaxonomyLanguage = langid;
        taxrequest.TaxonomyId = tid;
        taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
        TaxonomyData td = m_contentApi.EkContentRef.LoadTaxonomy(ref taxrequest);
        string path = tid.ToString();
        while (td != null && td.TaxonomyLevel != 0 && td.TaxonomyParentId != 0)
        {
            path = td.TaxonomyParentId.ToString() + "," + path;
            taxrequest.IncludeItems = false;
            taxrequest.Page = Page;
            taxrequest.TaxonomyLanguage = langid;
            taxrequest.TaxonomyId = td.TaxonomyParentId;
            taxrequest.TaxonomyType = Ektron.Cms.Common.EkEnumeration.TaxonomyType.Content;
            td = m_contentApi.EkContentRef.LoadTaxonomy(ref taxrequest);
        }
        return path;
    }

    private bool _forcefill;
    public void ForceFill(){
        _forcefill = true;
    }

    public bool HasFolderTaxonomyChoices
    {
        get
        {
            if (FolderID > -1)
            {
                if (folderData != null)
                {
                    return (folderData.FolderTaxonomy.Length > 0);
                }
            }
            return false;
        }
    }

    private List<long> _tid = new List<long>();
    public List<long> SelectedTaxonomies {
        get {
            if (txtselectedTaxonomyNodes.Text != "")
            {
                return new List<long>(
                    (new List<string>(
                        txtselectedTaxonomyNodes.Text.Split(','))
                    ).ConvertAll<long>(
                        delegate(string input)
                        {
                            long val = 0;
                            if (long.TryParse(input, out val))
                                return val;
                            else
                                return 0;
                        })
                );
            }
            else
            {
                return new List<long>();
            }
        }
    }

    private List<long> _PreselectedTaxonomies = new List<long>();
    public List<long> PreselectedTaxonomies
    {
        get
        {
            return _PreselectedTaxonomies;
        }
    }

    private long _fid = -1;
    public long FolderID {
        get { return _fid; }
        set { _fid = value; }
    }

    private long _defaulttid = -1;
    public long defaultTaxID
    {
        get { return _defaulttid; }
        set { _defaulttid = value; }
    }

    public string JSCallBackOnChange
    {
        get { return txtJSCallBack.Text; }
        set { txtJSCallBack.Text = value; }
    }

    private FolderData _fd = null;
    private FolderData folderData
    {
        get
        {
            if (_fd == null)
            {
                _fd = m_contentApi.GetFolderById(FolderID, true, false);
            }
            return _fd;
        }
    }
}

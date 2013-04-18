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

public class FormActionRewriterControlAdapter : System.Web.UI.Adapters.ControlAdapter
{

    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        base.Render(new RewriteFormActionHtmlTextWriter(writer));
    }
}

public class RewriteFormActionHtmlTextWriter : HtmlTextWriter
{

    public RewriteFormActionHtmlTextWriter(HtmlTextWriter writer)
        : base(writer)
    {
        this.InnerWriter = writer.InnerWriter;
    }
    public RewriteFormActionHtmlTextWriter(System.IO.TextWriter writer)
        : base(writer)
    {
        base.InnerWriter = writer;
    }
    public override void WriteAttribute(string name, string value, bool fEncode)
    {
        if (name == "action")
        {
            HttpContext Context;
            Context = HttpContext.Current;
            if (Context.Items["ActionAlreadyWritten"] == null)
            {
                value = Context.Request.RawUrl;
                Context.Items["ActionAlreadyWritten"] = true;
            }
        }
        base.WriteAttribute(name, value, fEncode);
    }
}
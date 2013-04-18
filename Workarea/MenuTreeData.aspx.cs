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
using System.Xml;
using System.Xml.Serialization;
using Ektron.Cms;
using Ektron.Cms.Common;

public partial class MenuTreeData : System.Web.UI.Page
{
    protected ContentAPI m_refContentApi;
    protected EkMessageHelper m_refMsgApi;

    private void Page_Load(System.Object sender, System.EventArgs e)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        if (m_refContentApi == null)
        {
            m_refContentApi = new ContentAPI();
        }
        m_refMsgApi = m_refContentApi.EkMsgRef;
        if (IsInCallbackMode())
        {
            return;
        }
    }

    private bool IsInCallbackMode()
    {
        Response.ContentType = "text/xml";
        Response.Write(RaiseCallbackEvent());
        Response.Flush();
        Response.End();
        return true;
    }
    private string RaiseCallbackEvent()
    {
        string result = "";
        long m_intId;
        AxMenuData menu;
        m_intId = Convert.ToInt64(Request.Params["id"]);
        if (!(Ektron.Cms.CommonApi.GetEcmCookie() == null))
        {
            m_refContentApi.ContentLanguage = System.Convert.ToInt32(Ektron.Cms.CommonApi.GetEcmCookie().Values["LastValidLanguageID"]);
        }
        menu = m_refContentApi.EkContentRef.GetMenuDataByID(m_intId);
        result = SerializeAsXmlData(menu, menu.GetType());
        return (result);
    }
    private string SerializeAsXmlData(object data, Type datatype)
    {
        string result = "";
        System.IO.MemoryStream XmlOutStream = new System.IO.MemoryStream();
        XmlSerializer XmlSer;
        byte[] byteArr;
        System.Text.UTF8Encoding Utf8 = new System.Text.UTF8Encoding();
        XmlSer = new XmlSerializer(datatype);
        XmlSer.Serialize(XmlOutStream, data);
        byteArr = XmlOutStream.ToArray();
        result = System.Convert.ToBase64String(byteArr, 0, byteArr.Length);
        result = Utf8.GetString(Convert.FromBase64String(result));
        return (result);
    }
}

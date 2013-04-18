using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ektron.Cms.Widget;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class ZipCodeTableElement : IComparable
{

    public String City;
    public String State;
    public String Zip;
    public String AreaCode;
    public String TimeZone;

    public ZipCodeTableElement(XmlNode table)
    {
        City = table.FirstChild.InnerText;
        State = table.FirstChild.NextSibling.InnerText;
        Zip = table.FirstChild.NextSibling.NextSibling.InnerText;
        AreaCode = table.FirstChild.NextSibling.NextSibling.NextSibling.InnerText;
        TimeZone = table.FirstChild.NextSibling.NextSibling.NextSibling.NextSibling.InnerText;
    }

    public int CompareTo(object o)
    {
        return City.CompareTo((o as ZipCodeTableElement).City);
    }

}

public partial class widgets_ZipCode : System.Web.UI.UserControl, IWidget
{
    protected void Page_Init(object sender, EventArgs e)
    {
        IWidgetHost _host = WidgetHost.GetHost(this);
        _host.Title = "Zip Code";
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), @"WidgetEnterCheck" + _host.WidgetInfo.ID.ToString(), "<script type='text/javascript'>function checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(e, saveButtonID){var evt = e ? e : window.event;if(evt.keyCode == 13){document.forms[0].onsubmit = function () { return false; }; evt.cancelBubble = true; if (evt.stopPropagation) evt.stopPropagation(); return false; }}</script>", false);
        searchValue.Attributes.Add("onkeydown", @"javascript:checkForEnter" + _host.WidgetInfo.ID.ToString() + @"(event, '" + searchButton.ClientID + "')");
    }
  

    protected void searchButton_Click(object sender, EventArgs e)
    {
        string searchTerm = "";
        string requestURL = "";
        lblResponse.Text = "";
        try
        {
            switch (searchDropDown.SelectedValue)
            {
                case "zip":
                    searchTerm = "Zip Code";
                    int zipCode = System.Convert.ToInt32(searchValue.Text);
                    requestURL = "http://www.webservicex.net/uszip.asmx/GetInfoByZIP?USZip=" + searchValue.Text;
                    break;
                case "area":
                    searchTerm = "Area Code";
                    int areaCode = System.Convert.ToInt32(searchValue.Text);
                    requestURL = "http://www.webservicex.net/uszip.asmx/GetInfoByAreaCode?USAreaCode=" + searchValue.Text;
                    break;
                case "state":
                    searchTerm = "State";
                    requestURL = "http://www.webservicex.net/uszip.asmx/GetInfoByState?USState=" + searchValue.Text;
                    showResults(requestURL, "State");
                    break;
                case "city":
                    requestURL = "http://www.webservicex.net/uszip.asmx/GetInfoByCity?USCity=" + searchValue.Text;
                    searchTerm = "City";
                    break;
            }
            showResults(requestURL, searchTerm);
        }
        catch
        {
            Repeater1.Visible = false;
            lblResponse.Text = "Invalid " + searchTerm;
        }
    }

    

    protected void showResults(String requestURL, String SearchTerm)
    {
        WebRequest request = WebRequest.Create(requestURL);
        //request.Timeout = 5000;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream resStream = response.GetResponseStream();
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(resStream);
        XmlNode root = xmldoc.FirstChild.NextSibling;
        List<ZipCodeTableElement> results = new List<ZipCodeTableElement>();
        if (root.HasChildNodes)
        {
            XmlNode table = root.FirstChild;
            while (table != null)
            {
                results.Add(new ZipCodeTableElement(table));
                table = table.NextSibling;
            }
            results.Sort();
            if (results.ToArray().Length > 0)
            {
                Repeater1.DataSource = results.ToArray();
                Repeater1.DataBind();
                Repeater1.Visible = true;
            }
            else
            {
                Repeater1.Visible = false;
                lblResponse.Text = "Invalid " + SearchTerm;
            }
        }
        else
        {
            Repeater1.Visible = false;
            lblResponse.Text = "Invalid " + SearchTerm;
        }
    }
}

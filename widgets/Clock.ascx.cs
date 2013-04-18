using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using Ektron.Cms.Widget;
using Ektron.Cms;
using Ektron.Cms.API;
using Ektron.Cms.Common;
using Ektron.Cms.PageBuilder;



public class ClockLocationPairs
{
    public string clockTag;
    public string clockLocation;

    public ClockLocationPairs(string tag, string location)
    {
        clockTag = tag;
        clockLocation = location;
    }
}

public partial class Widgets_ClockWidget : System.Web.UI.UserControl, IWidget
{
    #region properties
    private string _ClockLocation;
    private string _CloclTitle;
    [WidgetDataMember("US-NH")]
    public string ClockLocation { get { return _ClockLocation; } set { _ClockLocation = value; } }
    [WidgetDataMember("United States - New Hampshire")]
    public string CloclTitle { get { return _CloclTitle; } set { _CloclTitle = value; } }
    #endregion

    IWidgetHost _host;
    List<ListItem> timeZones;

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = Ektron.Cms.Widget.WidgetHost.GetHost(this);
        _host.Title = "Clock Widget";
        _host.Edit += new EditDelegate(EditEvent);
        _host.Maximize += new MaximizeDelegate(delegate() { Visible = true; });
        _host.Minimize += new MinimizeDelegate(delegate() { Visible = false; });
        _host.Create += new CreateDelegate(delegate() { EditEvent(""); });
        PreRender += new EventHandler(delegate(object PreRenderSender, EventArgs Evt) { SetOutput(); });
        ViewSet.SetActiveView(View);
    }

    void EditEvent(string settings)
    {
       try
        {
            generateClockLocationList();
            DropDownList1.DataSource = timeZones.ToArray();
            DropDownList1.DataBind();


            int i = 0;
            foreach (ListItem item in timeZones)
            {
                if (item.Value == ClockLocation)
                {
                    DropDownList1.SelectedIndex = i;
                    break;
                }
                i++;
            }
            clockTitleTextBox.Text = CloclTitle;
 
        }
        catch
        {
            lblData.Text = "Error editing widget";
            ViewSet.SetActiveView(View);
        }
        
        ViewSet.SetActiveView(Edit);
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
       
        try
        {
          
            if (clockTitleTextBox.Text == "")
            {
                CloclTitle = DropDownList1.SelectedItem.Value;
            }
            else
            {
                CloclTitle = clockTitleTextBox.Text;
            }
            generateClockLocationList();
            ClockLocation = timeZones[DropDownList1.SelectedIndex].Value;
            SetOutput();
        }
        catch
        {
            lblData.Text = "Error saving widget";
            ViewSet.SetActiveView(View);
        }


        _host.SaveWidgetDataMembers();
        ViewSet.SetActiveView(View);
    }

    protected void SetOutput()
    {
        try
        {

            if (ClockLocation.Length > 0)
            {
                lblData.Text = @"<center><object id=""clock" + _host.WidgetInfo.ID.ToString() + @"widget"" classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0"" ><param value=""opaque"" name=""wmode""/><param name=""movie"" width=""240"" height=""240""  value=""http://www.worldtimeserver.com/clocks/wtsclock001.swf?color=FF9900&wtsid=" + ClockLocation + @"""><embed width=""240"" height=""240"" src=""http://www.worldtimeserver.com/clocks/wtsclock001.swf?color=FF9900&wtsid=" + ClockLocation + @""" wmode=""opaque"" type=""application/x-shockwave-flash"" /></object></center>";
            }
        }
        catch
        {
            lblData.Text = "Error loading widget";
            ViewSet.SetActiveView(View);
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewSet.SetActiveView(View);
    }

   

    protected void generateClockLocationList()
    {
        timeZones = new List<ListItem>();

        timeZones.Add(new ListItem("(UTC/GMT)", "UTC"));
        timeZones.Add(new ListItem("Afghanistan", "AF"));
        timeZones.Add(new ListItem("Albania", "AL"));
        timeZones.Add(new ListItem("Algeria", "DZ"));
        timeZones.Add(new ListItem("American Samoa", "AS"));
        timeZones.Add(new ListItem("Andorra", "AD"));
        timeZones.Add(new ListItem("Angola", "AO"));
        timeZones.Add(new ListItem("Anguilla", "AI"));
        timeZones.Add(new ListItem("Antigua and Barbuda", "AG"));
        timeZones.Add(new ListItem("Argentina - Buenos Aires", "AR-BA"));
        timeZones.Add(new ListItem("Argentina - Catamarca", "AR-CT"));
        timeZones.Add(new ListItem("Argentina - Chaco", "AR-CC"));
        timeZones.Add(new ListItem("Argentina - Chubut", "AR-CH"));
        timeZones.Add(new ListItem("Argentina - Ciudad de Buenos Aires", "AR-DF"));
        timeZones.Add(new ListItem("Argentina - C&#243;rdoba", "AR-CB"));
        timeZones.Add(new ListItem("Argentina - Corrientes", "AR-CN"));
        timeZones.Add(new ListItem("Argentina - Entre Rios", "AR-ER"));
        timeZones.Add(new ListItem("Argentina - Formosa", "AR-FM"));
        timeZones.Add(new ListItem("Argentina - Jujuy", "AR-JY"));
        timeZones.Add(new ListItem("Argentina - La Pampa", "AR-LP"));
        timeZones.Add(new ListItem("Argentina - La Rioja", "AR-LR"));
        timeZones.Add(new ListItem("Argentina - Mendoza", "AR-MZ"));
        timeZones.Add(new ListItem("Argentina - Misiones", "AR-MN"));
        timeZones.Add(new ListItem("Argentina - Neuqu&#233;n", "AR-NQ"));
        timeZones.Add(new ListItem("Argentina - Rio Negro", "AR-RN"));
        timeZones.Add(new ListItem("Argentina - Salta", "AR-SA"));
        timeZones.Add(new ListItem("Argentina - San Juan", "AR-SJ"));
        timeZones.Add(new ListItem("Argentina - San Luis", "AR-SL"));
        timeZones.Add(new ListItem("Argentina - Santa Cruz", "AR-SC"));
        timeZones.Add(new ListItem("Argentina - Santa Fe", "AR-SF"));
        timeZones.Add(new ListItem("Argentina - Santiago del Estero", "AR-SE"));
        timeZones.Add(new ListItem("Argentina - Tierra del Fuego", "AR-TF"));
        timeZones.Add(new ListItem("Argentina - Tucum&#225;n", "AR-TM"));
        timeZones.Add(new ListItem("Armenia", "AM"));
        timeZones.Add(new ListItem("Aruba", "AW"));
        timeZones.Add(new ListItem("Australia - Australian Capital Territory", "AU-ACT"));
        timeZones.Add(new ListItem("Australia - Lord Howe Island", "AU1"));
        timeZones.Add(new ListItem("Australia - New South Wales", "AU-NSW"));
        timeZones.Add(new ListItem("Australia - New South Wales (exception)", "AU3"));
        timeZones.Add(new ListItem("Australia - Northern Territory", "AU-NT"));
        timeZones.Add(new ListItem("Australia - Queensland", "AU-QLD"));
        timeZones.Add(new ListItem("Australia - South Australia", "AU-SA"));
        timeZones.Add(new ListItem("Australia - Tasmania", "AU-TAS"));
        timeZones.Add(new ListItem("Australia - Victoria", "AU-VIC"));
        timeZones.Add(new ListItem("Australia - Western Australia", "AU-WA"));
        timeZones.Add(new ListItem("Austria", "AT"));
        timeZones.Add(new ListItem("Azerbaijan", "AZ"));
        timeZones.Add(new ListItem("Bahamas", "BS"));
        timeZones.Add(new ListItem("Bahrain", "BH"));
        timeZones.Add(new ListItem("Bangladesh", "BD"));
        timeZones.Add(new ListItem("Barbados", "BB"));
        timeZones.Add(new ListItem("Belarus", "BY"));
        timeZones.Add(new ListItem("Belgium", "BE"));
        timeZones.Add(new ListItem("Belize", "BZ"));
        timeZones.Add(new ListItem("Benin", "BJ"));
        timeZones.Add(new ListItem("Bermuda", "BM"));
        timeZones.Add(new ListItem("Bhutan", "BT"));
        timeZones.Add(new ListItem("Bolivia", "BO"));
        timeZones.Add(new ListItem("Bosnia and Herzegovina", "BA"));
        timeZones.Add(new ListItem("Botswana", "BW"));
        timeZones.Add(new ListItem("Brazil - Acre", "BR-AC"));
        timeZones.Add(new ListItem("Brazil - Alagoas", "BR-AL"));
        timeZones.Add(new ListItem("Brazil - Amapa", "BR-AP"));
        timeZones.Add(new ListItem("Brazil - Amazonas", "BR-AM"));
        timeZones.Add(new ListItem("Brazil - Bahia", "BR-BA"));
        timeZones.Add(new ListItem("Brazil - Ceara", "BR-CE"));
        timeZones.Add(new ListItem("Brazil - Distrito Federal", "BR-DF"));
        timeZones.Add(new ListItem("Brazil - Espirto Santo", "BR-ES"));
        timeZones.Add(new ListItem("Brazil - Fernando de Noronha", "BR-FN"));
        timeZones.Add(new ListItem("Brazil - Goias", "BR-GO"));
        timeZones.Add(new ListItem("Brazil - Maranhao", "BR-MA"));
        timeZones.Add(new ListItem("Brazil - Mato Grosso", "BR-MT"));
        timeZones.Add(new ListItem("Brazil - Mato Grosso do Sul", "BR-MS"));
        timeZones.Add(new ListItem("Brazil - Minas Gerais", "BR-MG"));
        timeZones.Add(new ListItem("Brazil - Para (eastern)", "BR-PA1"));
        timeZones.Add(new ListItem("Brazil - Para (western)", "BR-PA2"));
        timeZones.Add(new ListItem("Brazil - Paraiba", "BR-PB"));
        timeZones.Add(new ListItem("Brazil - Parana", "BR-PR"));
        timeZones.Add(new ListItem("Brazil - Pernambuco", "BR-PE"));
        timeZones.Add(new ListItem("Brazil - Piaui", "BR-PI"));
        timeZones.Add(new ListItem("Brazil - Rio de Janeiro", "BR-RJ"));
        timeZones.Add(new ListItem("Brazil - Rio Grande do Norte", "BR-RN"));
        timeZones.Add(new ListItem("Brazil - Rio Grande do Sul", "BR-RS"));
        timeZones.Add(new ListItem("Brazil - Rondonia", "BR-RO"));
        timeZones.Add(new ListItem("Brazil - Roraima", "BR-RR"));
        timeZones.Add(new ListItem("Brazil - Santa Catarina", "BR-SC"));
        timeZones.Add(new ListItem("Brazil - Sao Paulo", "BR-SP"));
        timeZones.Add(new ListItem("Brazil - Sergipe", "BR-SE"));
        timeZones.Add(new ListItem("Brazil - Tocantins", "BR-TO"));
        timeZones.Add(new ListItem("Brunei Darussalam", "BN"));
        timeZones.Add(new ListItem("Bulgaria", "BG"));
        timeZones.Add(new ListItem("Burkina Faso", "BF"));
        timeZones.Add(new ListItem("Burundi", "BI"));
        timeZones.Add(new ListItem("Cambodia", "KH"));
        timeZones.Add(new ListItem("Cameroon", "CM"));
        timeZones.Add(new ListItem("Canada - Alberta", "CA-AB"));
        timeZones.Add(new ListItem("Canada - British Columbia", "CA-BC"));
        timeZones.Add(new ListItem("Canada - British Columbia (exception 1)", "CA-BC1"));
        timeZones.Add(new ListItem("Canada - British Columbia (exception 2)", "CA-BC2"));
        timeZones.Add(new ListItem("Canada - Labrador", "CA2"));
        timeZones.Add(new ListItem("Canada - Labrador (exception)", "CA2A"));
        timeZones.Add(new ListItem("Canada - Manitoba", "CA-MB"));
        timeZones.Add(new ListItem("Canada - New Brunswick", "CA-NB"));
        timeZones.Add(new ListItem("Canada - Newfoundland", "CA-NF"));
        timeZones.Add(new ListItem("Canada - Northwest Territories", "CA-NT"));
        timeZones.Add(new ListItem("Canada - Nova Scotia", "CA-NS"));
        timeZones.Add(new ListItem("Canada - Nunavut - Southampton Island", "CA-NT2A"));
        timeZones.Add(new ListItem("Canada - Nunavut (Central)", "CA-NT2B"));
        timeZones.Add(new ListItem("Canada - Nunavut (Eastern)", "CA-NT2"));
        timeZones.Add(new ListItem("Canada - Nunavut (Mountain)", "CA-NT2C"));
        timeZones.Add(new ListItem("Canada - Ontario", "CA-ON"));
        timeZones.Add(new ListItem("Canada - Ontario (western)", "CA-ON1"));
        timeZones.Add(new ListItem("Canada - Prince Edward Island", "CA-PE"));
        timeZones.Add(new ListItem("Canada - Quebec", "CA-QC"));
        timeZones.Add(new ListItem("Canada - Quebec (far east)", "CA-QC1"));
        timeZones.Add(new ListItem("Canada - Saskatchewan", "CA-SK"));
        timeZones.Add(new ListItem("Canada - Saskatchewan (exceptions - east)", "CA-SK2"));
        timeZones.Add(new ListItem("Canada - Saskatchewan (exceptions - west)", "CA-SK1"));
        timeZones.Add(new ListItem("Canada - Yukon", "CA-YT"));
        timeZones.Add(new ListItem("Cape Verde", "CV"));
        timeZones.Add(new ListItem("Cayman Islands", "KY"));
        timeZones.Add(new ListItem("Central African Republic", "CF"));
        timeZones.Add(new ListItem("Chad", "TD"));
        timeZones.Add(new ListItem("Chile", "CL"));
        timeZones.Add(new ListItem("Chile - Easter Island", "CL2"));
        timeZones.Add(new ListItem("China", "CN"));
        timeZones.Add(new ListItem("Christmas Island (Indian Ocean)", "CX"));
        timeZones.Add(new ListItem("Cocos (Keeling) Islands", "CC"));
        timeZones.Add(new ListItem("Colombia", "CO"));
        timeZones.Add(new ListItem("Comoros", "KM"));
        timeZones.Add(new ListItem("Congo", "CG"));
        timeZones.Add(new ListItem("Congo, Democratic Republic of - (Eastern)", "CD2"));
        timeZones.Add(new ListItem("Congo, Democratic Republic of - (Western)", "CD"));
        timeZones.Add(new ListItem("Cook Islands", "CK"));
        timeZones.Add(new ListItem("Costa Rica", "CR"));
        timeZones.Add(new ListItem("Cote D'Ivoire", "CI"));
        timeZones.Add(new ListItem("Croatia", "HR"));
        timeZones.Add(new ListItem("Cuba", "CU"));
        timeZones.Add(new ListItem("Cyprus", "CY"));
        timeZones.Add(new ListItem("Czech Republic", "CZ"));
        timeZones.Add(new ListItem("Denmark", "DK"));
        timeZones.Add(new ListItem("Djibouti", "DJ"));
        timeZones.Add(new ListItem("Dominica", "DM"));
        timeZones.Add(new ListItem("Dominican Republic", "DO"));
        timeZones.Add(new ListItem("Ecuador", "EC"));
        timeZones.Add(new ListItem("Ecuador - Galapagos Islands", "EC2"));
        timeZones.Add(new ListItem("Egypt", "EG"));
        timeZones.Add(new ListItem("El Salvador", "SV"));
        timeZones.Add(new ListItem("Equatorial Guinea", "GQ"));
        timeZones.Add(new ListItem("Eritrea", "ER"));
        timeZones.Add(new ListItem("Estonia", "EE"));
        timeZones.Add(new ListItem("Ethiopia", "ET"));
        timeZones.Add(new ListItem("Falkland Islands (Malvinas)", "FK"));
        timeZones.Add(new ListItem("Faroe Islands", "FO"));
        timeZones.Add(new ListItem("Fiji", "FJ"));
        timeZones.Add(new ListItem("Finland", "FI"));
        timeZones.Add(new ListItem("France", "FR"));
        timeZones.Add(new ListItem("French Guiana", "GF"));
        timeZones.Add(new ListItem("French Polynesia - Austral Islands", "PF2A"));
        timeZones.Add(new ListItem("French Polynesia - Gambier Islands", "PF3"));
        timeZones.Add(new ListItem("French Polynesia - Marquesas Islands", "PF1"));
        timeZones.Add(new ListItem("French Polynesia - Society Islands (including Tahiti)", "PF"));
        timeZones.Add(new ListItem("French Polynesia - Tuamotu Archipelago", "PF2B"));
        timeZones.Add(new ListItem("Gabon", "GA"));
        timeZones.Add(new ListItem("Gambia", "GM"));
        timeZones.Add(new ListItem("Georgia", "GE"));
        timeZones.Add(new ListItem("Germany", "DE"));
        timeZones.Add(new ListItem("Ghana", "GH"));
        timeZones.Add(new ListItem("Gibraltar", "GI"));
        timeZones.Add(new ListItem("Greece", "GR"));
        timeZones.Add(new ListItem("Greenland - Greenland", "GL"));
        timeZones.Add(new ListItem("Greenland - Ittoqqortoormiit", "GL3"));
        timeZones.Add(new ListItem("Greenland - Pituffik", "GL2"));
        timeZones.Add(new ListItem("Grenada", "GD"));
        timeZones.Add(new ListItem("Guadeloupe", "GP"));
        timeZones.Add(new ListItem("Guam", "GU"));
        timeZones.Add(new ListItem("Guatemala", "GT"));
        timeZones.Add(new ListItem("Guinea", "GN"));
        timeZones.Add(new ListItem("Guinea-Bissau", "GW"));
        timeZones.Add(new ListItem("Guyana", "GY"));
        timeZones.Add(new ListItem("Haiti", "HT"));
        timeZones.Add(new ListItem("Honduras", "HN"));
        timeZones.Add(new ListItem("Hong Kong", "HK"));
        timeZones.Add(new ListItem("Hungary", "HU"));
        timeZones.Add(new ListItem("Iceland", "IS"));
        timeZones.Add(new ListItem("India", "IN"));
        timeZones.Add(new ListItem("Indonesia - (Central)", "ID2"));
        timeZones.Add(new ListItem("Indonesia - (Eastern)", "ID3"));
        timeZones.Add(new ListItem("Indonesia - (Western)", "ID"));
        timeZones.Add(new ListItem("Iran, Islamic Republic of", "IR"));
        timeZones.Add(new ListItem("Iraq", "IQ"));
        timeZones.Add(new ListItem("Ireland", "IE"));
        timeZones.Add(new ListItem("Israel", "IL"));
        timeZones.Add(new ListItem("Italy", "IT"));
        timeZones.Add(new ListItem("Jamaica", "JM"));
        timeZones.Add(new ListItem("Japan", "JP"));
        timeZones.Add(new ListItem("Johnston Atoll (U.S.)", "UM1"));
        timeZones.Add(new ListItem("Jordan", "JO"));
        timeZones.Add(new ListItem("Kazakhstan - (Eastern)", "KZ"));
        timeZones.Add(new ListItem("Kazakhstan - (Western)", "KZ1"));
        timeZones.Add(new ListItem("Kenya", "KE"));
        timeZones.Add(new ListItem("Kiribati - Gilbert Islands", "KI"));
        timeZones.Add(new ListItem("Kiribati - Line Islands", "KI2"));
        timeZones.Add(new ListItem("Kiribati - Phoenix Islands", "KI3"));
        timeZones.Add(new ListItem("Korea, Democratic People's Republic of", "KP"));
        timeZones.Add(new ListItem("Korea, Republic of", "KR"));
        timeZones.Add(new ListItem("Kuwait", "KW"));
        timeZones.Add(new ListItem("Kyrgyzstan", "KG"));
        timeZones.Add(new ListItem("Lao People's Democratic Republic", "LA"));
        timeZones.Add(new ListItem("Latvia", "LV"));
        timeZones.Add(new ListItem("Lebanon", "LB"));
        timeZones.Add(new ListItem("Lesotho", "LS"));
        timeZones.Add(new ListItem("Liberia", "LR"));
        timeZones.Add(new ListItem("Libyan Arab Jamahiriya", "LY"));
        timeZones.Add(new ListItem("Liechtenstein", "LI"));
        timeZones.Add(new ListItem("Lithuania", "LT"));
        timeZones.Add(new ListItem("Luxembourg", "LU"));
        timeZones.Add(new ListItem("Macao", "MO"));
        timeZones.Add(new ListItem("Macedonia, The Former Yugoslav Republic Of", "MK"));
        timeZones.Add(new ListItem("Madagascar", "MG"));
        timeZones.Add(new ListItem("Malawi", "MW"));
        timeZones.Add(new ListItem("Malaysia", "MY"));
        timeZones.Add(new ListItem("Maldives", "MV"));
        timeZones.Add(new ListItem("Mali", "ML"));
        timeZones.Add(new ListItem("Malta", "MT"));
        timeZones.Add(new ListItem("Marshall Islands", "MH"));
        timeZones.Add(new ListItem("Martinique", "MQ"));
        timeZones.Add(new ListItem("Mauritania", "MR"));
        timeZones.Add(new ListItem("Mauritius", "MU"));
        timeZones.Add(new ListItem("Mayotte", "YT"));
        timeZones.Add(new ListItem("Mexico - (South, Central, and Eastern)", "MX"));
        timeZones.Add(new ListItem("Mexico - Baja California Norte", "MX3"));
        timeZones.Add(new ListItem("Mexico - Baja California Sur", "MX2"));
        timeZones.Add(new ListItem("Mexico - Chihuahua", "MX2-3"));
        timeZones.Add(new ListItem("Mexico - Nayarit", "MX2-1"));
        timeZones.Add(new ListItem("Mexico - Sinaloa", "MX2-2"));
        timeZones.Add(new ListItem("Mexico - Sonora", "MX2A"));
        timeZones.Add(new ListItem("Micronesia, Federated States Of - Kosrae, Pohnpei", "FM"));
        timeZones.Add(new ListItem("Micronesia, Federated States Of - Yap, Chuuk", "FM1"));
        timeZones.Add(new ListItem("Midway Islands (U.S.)", "UM2"));
        timeZones.Add(new ListItem("Moldova, Republic of", "MD"));
        timeZones.Add(new ListItem("Monaco", "MC"));
        timeZones.Add(new ListItem("Mongolia - (Central and Eastern)", "MN"));
        timeZones.Add(new ListItem("Mongolia - (Western)", "MN1"));
        timeZones.Add(new ListItem("Montenegro", "ME"));
        timeZones.Add(new ListItem("Montserrat", "MS"));
        timeZones.Add(new ListItem("Morocco", "MA"));
        timeZones.Add(new ListItem("Mozambique", "MZ"));
        timeZones.Add(new ListItem("Myanmar", "MM"));
        timeZones.Add(new ListItem("Namibia", "NA"));
        timeZones.Add(new ListItem("Nauru", "NR"));
        timeZones.Add(new ListItem("Nepal", "NP"));
        timeZones.Add(new ListItem("Netherlands", "NL"));
        timeZones.Add(new ListItem("Netherlands Antilles - Bonaire", "AN"));
        timeZones.Add(new ListItem("New Caledonia", "NC"));
        timeZones.Add(new ListItem("New Zealand", "NZ"));
        timeZones.Add(new ListItem("New Zealand - Chatham Islands", "NZ2"));
        timeZones.Add(new ListItem("Nicaragua", "NI"));
        timeZones.Add(new ListItem("Niger", "NE"));
        timeZones.Add(new ListItem("Nigeria", "NG"));
        timeZones.Add(new ListItem("Niue", "NU"));
        timeZones.Add(new ListItem("Norfolk Island", "NF"));
        timeZones.Add(new ListItem("Northern Mariana Islands", "MP"));
        timeZones.Add(new ListItem("Norway", "NO"));
        timeZones.Add(new ListItem("Oman", "OM"));
        timeZones.Add(new ListItem("Pakistan", "PK"));
        timeZones.Add(new ListItem("Palau", "PW"));
        timeZones.Add(new ListItem("Palestinian Territory", "PS"));
        timeZones.Add(new ListItem("Panama", "PA"));
        timeZones.Add(new ListItem("Papua New Guinea", "PG"));
        timeZones.Add(new ListItem("Paraguay", "PY"));
        timeZones.Add(new ListItem("Peru", "PE"));
        timeZones.Add(new ListItem("Philippines", "PH"));
        timeZones.Add(new ListItem("Pitcairn", "PN"));
        timeZones.Add(new ListItem("Poland", "PL"));
        timeZones.Add(new ListItem("Portugal", "PT"));
        timeZones.Add(new ListItem("Portugal - Azores", "PT2"));
        timeZones.Add(new ListItem("Portugal - Madeira Islands", "PT1"));
        timeZones.Add(new ListItem("Puerto Rico", "PR"));
        timeZones.Add(new ListItem("Qatar", "QA"));
        timeZones.Add(new ListItem("Reunion", "RE"));
        timeZones.Add(new ListItem("Romania", "RO"));
        timeZones.Add(new ListItem("Russia - Adygea", "RU-AD"));
        timeZones.Add(new ListItem("Russia - Agin-Buryat", "RU-AGB"));
        timeZones.Add(new ListItem("Russia - Alania", "RU-SE"));
        timeZones.Add(new ListItem("Russia - Altai Republic", "RU-AL"));
        timeZones.Add(new ListItem("Russia - Altaskiy Kray", "RU-ALT"));
        timeZones.Add(new ListItem("Russia - Amur", "RU-AMU"));
        timeZones.Add(new ListItem("Russia - Arkhangel'", "RU-ARK"));
        timeZones.Add(new ListItem("Russia - Astrakhan'", "RU-AST"));
        timeZones.Add(new ListItem("Russia - Bashkortostan", "RU-BA"));
        timeZones.Add(new ListItem("Russia - Belgorod", "RU-BEL"));
        timeZones.Add(new ListItem("Russia - Bryansk", "RU-BRY"));
        timeZones.Add(new ListItem("Russia - Buryatia", "RU-BU"));
        timeZones.Add(new ListItem("Russia - Chechnya", "RU-CE"));
        timeZones.Add(new ListItem("Russia - Chelyabinsk", "RU-CHE"));
        timeZones.Add(new ListItem("Russia - Chita", "RU-CHI"));
        timeZones.Add(new ListItem("Russia - Chukot", "RU-CHU"));
        timeZones.Add(new ListItem("Russia - Chuvashia", "RU-CU"));
        timeZones.Add(new ListItem("Russia - Dagestan", "RU-DA"));
        timeZones.Add(new ListItem("Russia - Evenki", "RU-EVE"));
        timeZones.Add(new ListItem("Russia - Ingushetia", "RU-IN"));
        timeZones.Add(new ListItem("Russia - Irkutsk", "RU-IRK"));
        timeZones.Add(new ListItem("Russia - Ivanovo", "RU-IVA"));
        timeZones.Add(new ListItem("Russia - Jewish Autonomous Oblast'", "RU-YEV"));
        timeZones.Add(new ListItem("Russia - Kabardino-Balkaria", "RU-KB"));
        timeZones.Add(new ListItem("Russia - Kaliningrad", "RU-KGD"));
        timeZones.Add(new ListItem("Russia - Kalmykia", "RU-KL"));
        timeZones.Add(new ListItem("Russia - Kaluga", "RU-KLU"));
        timeZones.Add(new ListItem("Russia - Kamchatka", "RU-KAM"));
        timeZones.Add(new ListItem("Russia - Karachay-Cherkessia", "RU-KC"));
        timeZones.Add(new ListItem("Russia - Karelia", "RU-KR"));
        timeZones.Add(new ListItem("Russia - Kemerovo", "RU-KEM"));
        timeZones.Add(new ListItem("Russia - Khabarovsk", "RU-KHA"));
        timeZones.Add(new ListItem("Russia - Khakassia", "RU-KK"));
        timeZones.Add(new ListItem("Russia - Khanty-Mansi", "RU-KHM"));
        timeZones.Add(new ListItem("Russia - Kirov", "RU-KIR"));
        timeZones.Add(new ListItem("Russia - Komi", "RU-KO"));
        timeZones.Add(new ListItem("Russia - Koryak", "RU-KOR"));
        timeZones.Add(new ListItem("Russia - Kostroma", "RU-KOS"));
        timeZones.Add(new ListItem("Russia - Krasnodar", "RU-KDA"));
        timeZones.Add(new ListItem("Russia - Krasnoyarsk", "RU-KYA"));
        timeZones.Add(new ListItem("Russia - Kurgan", "RU-KGN"));
        timeZones.Add(new ListItem("Russia - Kursk", "RU-KRS"));
        timeZones.Add(new ListItem("Russia - Leningradskaya Oblast'", "RU-LEN"));
        timeZones.Add(new ListItem("Russia - Lipetsk", "RU-LIP"));
        timeZones.Add(new ListItem("Russia - Magadan", "RU-MAG"));
        timeZones.Add(new ListItem("Russia - Mari El", "RU-ME"));
        timeZones.Add(new ListItem("Russia - Mordovia", "RU-MO"));
        timeZones.Add(new ListItem("Russia - Moscow City", "RU-MOW"));
        timeZones.Add(new ListItem("Russia - Moskva", "RU-MOS"));
        timeZones.Add(new ListItem("Russia - Murmansk", "RU-MUR"));
        timeZones.Add(new ListItem("Russia - Nenets", "RU-NEN"));
        timeZones.Add(new ListItem("Russia - Nizhniy Novgorod", "RU-NIZ"));
        timeZones.Add(new ListItem("Russia - Novgorod", "RU-NGR"));
        timeZones.Add(new ListItem("Russia - Novosibirsk", "RU-NVS"));
        timeZones.Add(new ListItem("Russia - Omsk", "RU-OMS"));
        timeZones.Add(new ListItem("Russia - Orel", "RU-ORL"));
        timeZones.Add(new ListItem("Russia - Orenburg", "RU-ORE"));
        timeZones.Add(new ListItem("Russia - Penza", "RU-PNZ"));
        timeZones.Add(new ListItem("Russia - Perm", "RU-PER"));
        timeZones.Add(new ListItem("Russia - Primorskiy", "RU-PRI"));
        timeZones.Add(new ListItem("Russia - Pskov", "RU-PSK"));
        timeZones.Add(new ListItem("Russia - Rostov", "RU-ROS"));
        timeZones.Add(new ListItem("Russia - Ryazan'", "RU-RYA"));
        timeZones.Add(new ListItem("Russia - Sakha (Central)", "RU-SA2"));
        timeZones.Add(new ListItem("Russia - Sakha (Eastern)", "RU-SA3"));
        timeZones.Add(new ListItem("Russia - Sakha (Western)", "RU-SA"));
        timeZones.Add(new ListItem("Russia - Sakhalin", "RU-SAK"));
        timeZones.Add(new ListItem("Russia - Sakhalin (Kuril Islands)", "RU-SAK2"));
        timeZones.Add(new ListItem("Russia - Samara", "RU-SAM"));
        timeZones.Add(new ListItem("Russia - Saratov", "RU-SAR"));
        timeZones.Add(new ListItem("Russia - Smolensk", "RU-SMO"));
        timeZones.Add(new ListItem("Russia - St. Petersburg City", "RU-SPE"));
        timeZones.Add(new ListItem("Russia - Stavropol", "RU-STA"));
        timeZones.Add(new ListItem("Russia - Sverdlovsk", "RU-SVE"));
        timeZones.Add(new ListItem("Russia - Tambov", "RU-TAM"));
        timeZones.Add(new ListItem("Russia - Tatarstan", "RU-TA"));
        timeZones.Add(new ListItem("Russia - Taymyr", "RU-TAY"));
        timeZones.Add(new ListItem("Russia - Tomsk", "RU-TOM"));
        timeZones.Add(new ListItem("Russia - Tula", "RU-TUL"));
        timeZones.Add(new ListItem("Russia - Tuva", "RU-TY"));
        timeZones.Add(new ListItem("Russia - Tver'", "RU-TVE"));
        timeZones.Add(new ListItem("Russia - Tyumen'", "RU-TYU"));
        timeZones.Add(new ListItem("Russia - Udmurtia", "RU-UD"));
        timeZones.Add(new ListItem("Russia - Ul'yanovsk", "RU-ULY"));
        timeZones.Add(new ListItem("Russia - Ust-Ordyn-Buryat", "RU-UOB"));
        timeZones.Add(new ListItem("Russia - Vladimir", "RU-VLA"));
        timeZones.Add(new ListItem("Russia - Volgograd", "RU-VGG"));
        timeZones.Add(new ListItem("Russia - Vologda", "RU-VLG"));
        timeZones.Add(new ListItem("Russia - Voronezh", "RU-VOR"));
        timeZones.Add(new ListItem("Russia - Yamalo-Nenets", "RU-YAN"));
        timeZones.Add(new ListItem("Russia - Yaroslavl'", "RU-YAR"));
        timeZones.Add(new ListItem("Rwanda", "RW"));
        timeZones.Add(new ListItem("Saint Barthelemy", "BL"));
        timeZones.Add(new ListItem("Saint Helena", "SH"));
        timeZones.Add(new ListItem("Saint Kitts and Nevis", "KN"));
        timeZones.Add(new ListItem("Saint Lucia", "LC"));
        timeZones.Add(new ListItem("Saint Martin", "MF"));
        timeZones.Add(new ListItem("Saint Pierre and Miquelon", "PM"));
        timeZones.Add(new ListItem("Saint Vincent and The Grenadines", "VC"));
        timeZones.Add(new ListItem("Samoa", "WS"));
        timeZones.Add(new ListItem("San Marino", "SM"));
        timeZones.Add(new ListItem("Sao Tome and Principe", "ST"));
        timeZones.Add(new ListItem("Saudi Arabia", "SA"));
        timeZones.Add(new ListItem("Senegal", "SN"));
        timeZones.Add(new ListItem("Serbia", "RS"));
        timeZones.Add(new ListItem("Seychelles", "SC"));
        timeZones.Add(new ListItem("Sierra Leone", "SL"));
        timeZones.Add(new ListItem("Singapore", "SG"));
        timeZones.Add(new ListItem("Slovakia", "SK"));
        timeZones.Add(new ListItem("Slovenia", "SI"));
        timeZones.Add(new ListItem("Solomon Islands", "SB"));
        timeZones.Add(new ListItem("Somalia", "SO"));
        timeZones.Add(new ListItem("South Africa", "ZA"));
        timeZones.Add(new ListItem("Spain - Canary Islands", "ES2"));
        timeZones.Add(new ListItem("Spain - Mainland, Baleares, Melilla, Ceuta", "ES"));
        timeZones.Add(new ListItem("Sri Lanka", "LK"));
        timeZones.Add(new ListItem("Sudan", "SD"));
        timeZones.Add(new ListItem("Suriname", "SR"));
        timeZones.Add(new ListItem("Svalbard and Jan Mayen", "SJ"));
        timeZones.Add(new ListItem("Swaziland", "SZ"));
        timeZones.Add(new ListItem("Sweden", "SE"));
        timeZones.Add(new ListItem("Switzerland", "CH"));
        timeZones.Add(new ListItem("Syrian Arab Republic", "SY"));
        timeZones.Add(new ListItem("Taiwan", "TW"));
        timeZones.Add(new ListItem("Tajikistan", "TJ"));
        timeZones.Add(new ListItem("Tanzania, United Republic of", "TZ"));
        timeZones.Add(new ListItem("Thailand", "TH"));
        timeZones.Add(new ListItem("Timor-Leste", "TL"));
        timeZones.Add(new ListItem("Togo", "TG"));
        timeZones.Add(new ListItem("Tokelau", "TK"));
        timeZones.Add(new ListItem("Tonga", "TO"));
        timeZones.Add(new ListItem("Trinidad and Tobago", "TT"));
        timeZones.Add(new ListItem("Tunisia", "TN"));
        timeZones.Add(new ListItem("Turkey", "TR"));
        timeZones.Add(new ListItem("Turkmenistan", "TM"));
        timeZones.Add(new ListItem("Turks and Caicos Islands", "TC"));
        timeZones.Add(new ListItem("Tuvalu", "TV"));
        timeZones.Add(new ListItem("Uganda", "UG"));
        timeZones.Add(new ListItem("Ukraine", "UA"));
        timeZones.Add(new ListItem("United Arab Emirates", "AE"));
        timeZones.Add(new ListItem("United Kingdom", "GB"));
        timeZones.Add(new ListItem("United States - Alabama", "US-AL"));
        timeZones.Add(new ListItem("United States - Alaska", "US-AK"));
        timeZones.Add(new ListItem("United States - Alaska (Aleutian Islands)", "US-AK1"));
        timeZones.Add(new ListItem("United States - Arizona", "US-AZ"));
        timeZones.Add(new ListItem("United States - Arizona (Navajo Reservation)", "US-AZ1"));
        timeZones.Add(new ListItem("United States - Arkansas", "US-AR"));
        timeZones.Add(new ListItem("United States - California", "US-CA"));
        timeZones.Add(new ListItem("United States - Colorado", "US-CO"));
        timeZones.Add(new ListItem("United States - Connecticut", "US-CT"));
        timeZones.Add(new ListItem("United States - Delaware", "US-DE"));
        timeZones.Add(new ListItem("United States - District of Columbia", "US-DC"));
        timeZones.Add(new ListItem("United States - Florida", "US-FL"));
        timeZones.Add(new ListItem("United States - Florida (far west)", "US-FL1"));
        timeZones.Add(new ListItem("United States - Georgia", "US-GA"));
        timeZones.Add(new ListItem("United States - Hawaii", "US-HI"));
        timeZones.Add(new ListItem("United States - Idaho (northern)", "US-ID1"));
        timeZones.Add(new ListItem("United States - Idaho (southern)", "US-ID"));
        timeZones.Add(new ListItem("United States - Illinois", "US-IL"));
        timeZones.Add(new ListItem("United States - Indiana", "US-IN"));
        timeZones.Add(new ListItem("United States - Indiana (far west)", "US-IN1"));
        timeZones.Add(new ListItem("United States - Iowa", "US-IA"));
        timeZones.Add(new ListItem("United States - Kansas", "US-KS"));
        timeZones.Add(new ListItem("United States - Kansas (exception)", "US-KS1"));
        timeZones.Add(new ListItem("United States - Kentucky (eastern)", "US-KY"));
        timeZones.Add(new ListItem("United States - Kentucky (western)", "US-KY1"));
        timeZones.Add(new ListItem("United States - Louisiana", "US-LA"));
        timeZones.Add(new ListItem("United States - Maine", "US-ME"));
        timeZones.Add(new ListItem("United States - Maryland", "US-MD"));
        timeZones.Add(new ListItem("United States - Massachusetts", "US-MA"));
        timeZones.Add(new ListItem("United States - Michigan", "US-MI"));
        timeZones.Add(new ListItem("United States - Michigan (exception)", "US-MI1"));
        timeZones.Add(new ListItem("United States - Minnesota", "US-MN"));
        timeZones.Add(new ListItem("United States - Mississippi", "US-MS"));
        timeZones.Add(new ListItem("United States - Missouri", "US-MO"));
        timeZones.Add(new ListItem("United States - Montana", "US-MT"));
        timeZones.Add(new ListItem("United States - Nebraska", "US-NE"));
        timeZones.Add(new ListItem("United States - Nebraska (western)", "US-NE1"));
        timeZones.Add(new ListItem("United States - Nevada", "US-NV"));
        timeZones.Add(new ListItem("United States - New Hampshire", "US-NH"));
        timeZones.Add(new ListItem("United States - New Jersey", "US-NJ"));
        timeZones.Add(new ListItem("United States - New Mexico", "US-NM"));
        timeZones.Add(new ListItem("United States - New York", "US-NY"));
        timeZones.Add(new ListItem("United States - North Carolina", "US-NC"));
        timeZones.Add(new ListItem("United States - North Dakota", "US-ND"));
        timeZones.Add(new ListItem("United States - North Dakota (western)", "US-ND1"));
        timeZones.Add(new ListItem("United States - Ohio", "US-OH"));
        timeZones.Add(new ListItem("United States - Oklahoma", "US-OK"));
        timeZones.Add(new ListItem("United States - Oregon", "US-OR"));
        timeZones.Add(new ListItem("United States - Oregon (exception)", "US-OR1"));
        timeZones.Add(new ListItem("United States - Pennsylvania", "US-PA"));
        timeZones.Add(new ListItem("United States - Rhode Island", "US-RI"));
        timeZones.Add(new ListItem("United States - South Carolina", "US-SC"));
        timeZones.Add(new ListItem("United States - South Dakota (eastern)", "US-SD"));
        timeZones.Add(new ListItem("United States - South Dakota (western)", "US-SD1"));
        timeZones.Add(new ListItem("United States - Tennessee (eastern)", "US-TN1"));
        timeZones.Add(new ListItem("United States - Tennessee (western)", "US-TN"));
        timeZones.Add(new ListItem("United States - Texas", "US-TX"));
        timeZones.Add(new ListItem("United States - Texas (far west)", "US-TX1"));
        timeZones.Add(new ListItem("United States - Utah", "US-UT"));
        timeZones.Add(new ListItem("United States - Vermont", "US-VT"));
        timeZones.Add(new ListItem("United States - Virginia", "US-VA"));
        timeZones.Add(new ListItem("United States - Washington", "US-WA"));
        timeZones.Add(new ListItem("United States - West Virginia", "US-WV"));
        timeZones.Add(new ListItem("United States - Wisconsin", "US-WI"));
        timeZones.Add(new ListItem("United States - Wyoming", "US-WY"));
        timeZones.Add(new ListItem("Uruguay", "UY"));
        timeZones.Add(new ListItem("Uzbekistan", "UZ"));
        timeZones.Add(new ListItem("Vanuatu", "VU"));
        timeZones.Add(new ListItem("Venezuela", "VE"));
        timeZones.Add(new ListItem("Viet Nam", "VN"));
        timeZones.Add(new ListItem("Virgin Islands (British)", "VG"));
        timeZones.Add(new ListItem("Virgin Islands (U.S.)", "VI"));
        timeZones.Add(new ListItem("Wake Island (U.S.)", "UM3"));
        timeZones.Add(new ListItem("Wallis and Futuna", "WF"));
        timeZones.Add(new ListItem("Yemen", "YE"));
        timeZones.Add(new ListItem("Zambia", "ZM"));
        timeZones.Add(new ListItem("Zimbabwe", "ZW"));
    }

}



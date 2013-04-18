using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ektron.Cms.Widget;
using System.ComponentModel;
using System.Reflection;

public enum SpacerUnit
{
    [Description("em")] em,
    [Description("px")] px,
    [Description("pt")] pt
}

public partial class widgets_Spacer : System.Web.UI.UserControl
{
    private IWidgetHost _host;

    private bool _IsSpacer;
    private uint _Height;
    private SpacerUnit _Unit;

    [WidgetDataMember(false)]
    public bool IsSpacer { get { return _IsSpacer; } set { _IsSpacer = value; } }

    [WidgetDataMember((uint)1)]
    public uint Height { get { return _Height; } set { _Height = value; } }

    [WidgetDataMember(SpacerUnit.px)]
    public SpacerUnit Unit { get { return _Unit; } set { _Unit = value; } }

    // Credits: http://www.codeproject.com/KB/cs/enumwithdescription.aspx?msg=649306#xx649306xx
    private string GetDescription(Enum e)
    {
        FieldInfo fi = e.GetType().GetField(e.ToString());
        DescriptionAttribute[] da = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return (da.Length > 0) ? da[0].Description : e.ToString();
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        _host = WidgetHost.GetHost(this);
        _host.Title = "Spacer";
        _host.Edit += new EditDelegate(_host_Edit);

        ViewSet.SetActiveView(View);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        hrBreak.Visible = IsSpacer;
        divBreak.Visible = !IsSpacer;
        divBreak.Attributes["style"] = String.Format("height: {0}{1};",
                                                      _Height.ToString(),
                                                      GetDescription(_Unit));
    }

    void _host_Edit(string settings)
    {
        tbHeight.Text = _Height.ToString();
        ddlUnit.Text = _Unit.ToString();
        cbIsBreak.Checked = _IsSpacer;

        ddlUnit.DataSource = Enum.GetNames(typeof(SpacerUnit));
        ddlUnit.DataBind();

        ViewSet.SetActiveView(Edit);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        _Height = uint.Parse(tbHeight.Text);
        _Unit = (SpacerUnit)Enum.Parse(typeof(SpacerUnit), ddlUnit.Text);
        _IsSpacer = cbIsBreak.Checked;

        _host.SaveWidgetDataMembers();

        ViewSet.SetActiveView(View);
    }
}

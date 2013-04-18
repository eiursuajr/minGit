<%@ Control Language="C#" %>
<script runat="server">
    private string _onOK = "return insertField();";

    public string OnOK
    {
        get { return _onOK; }
        set { _onOK = value; }
    }

    protected void Page_Load(Object src, EventArgs e)
    {
		Ektron.Cms.Common.EkMessageHelper refMsg = (new Ektron.Cms.ContentAPI()).EkMsgRef;
		this.btnOK.Text = refMsg.GetMessage("btn ok");
        this.btnOK.ToolTip = refMsg.GetMessage("btn ok");
		this.btnCancel.Text = refMsg.GetMessage("btn cancel");
        this.btnCancel.ToolTip = refMsg.GetMessage("btn cancel");
			
		btnOK.OnClientClick = _onOK;
    }
</script>
<div class="Ektron_Dialogs_ButtonContainer Ektron_Dialogs_Buttons" style=" bottom: 0;">
    <asp:button ID="btnOK" CssClass="Ektron_StandardButton" Text="OK" runat="server" />
    &nbsp;
    <asp:button ID="btnCancel" CssClass="Ektron_StandardButton" OnClientClick="CloseDlg(); return false;" Text="Cancel" runat="server" />
</div>

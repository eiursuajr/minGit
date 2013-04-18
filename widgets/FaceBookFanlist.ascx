<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FaceBookFanlist.ascx.cs" Inherits="FaceBookFanlist" %>

   <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
    <asp:View ID="View" runat="server">
        <div id="FB_HiddenContainer"  style="position:absolute; top:-10000px; width:0px; height:0px;" ></div>
        <asp:PlaceHolder ID="phContent" runat="server">
            <asp:Literal ID="uxFacebook" runat="server"></asp:Literal>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">
            <div id="divHelpText" runat="server" style="font: normal 12px/15px arial; width: 100%;
                height: 100%;">
                Click on the 'Edit' icon (<img alt="edit icon" title="edit icon" src="<%=appPath%>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png"
                    width="12" height="12" border="0" />) in the top-right corner of this widget
                to set api key and profile id for facebook fanlist you wish to display.
            </div>
        </asp:PlaceHolder>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>_edit">
            <div style="width: 100%">
                <dl style="width: 100%;">
                    <dt style="padding:4px 5px 0 0; width:14%; float:left;">Api Key:</dt>
                    <dd style="padding:4px 0 0 0;">
                        <asp:TextBox ID="uxApiKey" runat="server" MaxLength="100" Width="300px" ValidationGroup ="FaceBookfanList"> </asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqvdrApiKey" runat ="server" ErrorMessage ="Please select an Api Key" ControlToValidate ="uxApiKey" ValidationGroup ="FaceBookfanList" ></asp:RequiredFieldValidator>
                        </dd>
                </dl>
                <dl style="width: 100%;">
                    <dt style="padding:4px 5px 0 0; width:14%; float:left;">Profile ID:</dt>
                    <dd style="padding:4px 0 0 0;">
                        <asp:TextBox ID="uxProfileID" runat="server" MaxLength="100" Width="300px" ValidationGroup ="FaceBookfanList"> </asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqvdrProfileID" runat ="server" ErrorMessage ="Please select Profile ID" ControlToValidate ="uxProfileID" ValidationGroup ="FaceBookfanList" ></asp:RequiredFieldValidator>
                        </dd>
                </dl>
                <dl style="width: 100%;">
                    <dt style="padding:4px 5px 0 0; width:14%; float:left;">Width:</dt>
                    <dd style="padding:4px 0 0 0;">
                        <asp:TextBox ID="uxWidth" runat="server" Width="300px"  MaxLength="3" onkeypress="return AllowOnlyNumeric(event);"
                            oncopy="return MouseClickEvent();" onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();"> </asp:TextBox></dd>
                </dl>
                <dl style="width: 100%;">
                    <dt style="padding:4px 5px 0 0; width:14%; float:left;">Height:</dt>
                    <dd style="padding:4px 0 0 0;">
                        <asp:TextBox ID="uxHeight" runat="server" Width="300px"  MaxLength="3" onkeypress="return AllowOnlyNumeric(event);"
                            oncopy="return MouseClickEvent();" onpaste="return MouseClickEvent();" oncut="return MouseClickEvent();"> </asp:TextBox></dd>
                </dl>
            </div><br />
            <div>
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup ="FaceBookfanList" />&nbsp;&nbsp;
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation ="false" />
            </div>
        </div>
    </asp:View>
</asp:MultiView>


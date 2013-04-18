<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MessageBoard.ascx.cs"
    Inherits="widget_MessageBoard" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
    <asp:View ID="View" runat="server">
        <CMS:MessageBoard ID="cmsMessageBoard" runat="server" DynamicObjectParameter="PageID" EnablePaging="True"  />
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>_edit">
            Moderate:<asp:CheckBox ID="ModerateCheckBox" runat="server" Checked="false" /><br />
            EnablePaging: <asp:CheckBox ID="EnablePagingCheckBox" runat="server" Checked="true" /><br />
            Page Size: <asp:TextBox ID="pagesizeTextBox" runat="server" Style="max-width: 30px"></asp:TextBox><br />
            Show Max Char: <asp:TextBox ID="MaxCharTextBox" runat="server" Style="max-width: 30px"></asp:TextBox><br /><br />
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
            &nbsp;&nbsp;
            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        </div>
    </asp:View>
</asp:MultiView>

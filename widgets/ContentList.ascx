<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentList.ascx.cs" Inherits="widgets_ContentList" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <CMS:ContentList ID="ContentList1" runat="server" ContentIds="" LinkTarget="_blank" />
    </asp:View>
    <asp:View ID="Edit" runat="server">
          <div id="<%=ClientID%>_edit">
     <table style="width:95%;">
            <tr>
                <td>
                   Content Ids (Separated by commas): </td>
                <td>
                    <asp:TextBox ID="ids" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    Teaser:</td>
                <td>
                    <asp:CheckBox ID="TeaserCheckBox" runat="server" Checked="true" /></td>
            </tr>
               
            <tr>
                <td>
                    IncludeIcons:</td>
                <td>
                    <asp:CheckBox ID="IncludeIconsCheckBox" runat="server" Checked="false" /></td>
            </tr>
            <tr>
                <td>
                    Direction:</td>
                <td>
                    <asp:DropDownList ID="ContentListDirectionSelectDropDownList" runat="server">
                        <asp:ListItem>Ascending</asp:ListItem>
						<asp:ListItem>Descending</asp:ListItem>
                    </asp:DropDownList>
               </td>
           </tr>
            <tr>
                <td>
                    OrderBy:</td>
                <td>
                    <asp:DropDownList ID="ContentListOrderKeyDropDownList" runat="server">
                        <asp:ListItem Value="Title">Title</asp:ListItem>
                        <asp:ListItem Value="OrderOfTheIds">Order of the IDs</asp:ListItem>
						<asp:ListItem Value="DateModified">Date modified</asp:ListItem>
						<asp:ListItem Value="DateCreated">Date created</asp:ListItem>
						<asp:ListItem Value="LastEditorFname">Last editor first name</asp:ListItem>
						<asp:ListItem Value="LastEditorLname">Last editor last name</asp:ListItem>
                    </asp:DropDownList>
               </td>
           </tr>       
            
            
            <tr>
                <td>
                </td>
                <td> <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
                    <asp:Button ID="Button1" runat="server" OnClick="SaveButton_Click" Text="Save" /></td>
            </tr>
        </table>
    </div> 
    </asp:View>
</asp:MultiView>
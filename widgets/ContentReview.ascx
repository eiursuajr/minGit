<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentReview.ascx.cs" Inherits="widget_ContentReview" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        
        <asp:View ID="View" runat="server">
            <!-- You Need To Do ..............................  -->
            <CMS:ContentReview ID="ContentReview1" runat="server" DynamicParameter="id,Pageid" GetReviews="content" DisplayXslt="Ajax 5 Stars" />
                <br />
            <CMS:ContentReview ID="ContentReview2" runat="server" DynamicParameter="id,Pageid" GetReviews="content" DisplayXslt="Review List" MaxReviews="10" />
            <!-- End To Do ..............................  -->
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>_edit">
                 <!-- You Need To Do ..............................  -->
               Star Style: <asp:DropDownList ID="StarStyleDropDownList" runat="server">
                                        <asp:ListItem Value="Ajax 5 Stars">Ajax 5 Stars</asp:ListItem>
                                        <asp:ListItem Value="Ajax 5 Stars Comment">Ajax 5 Stars Comment</asp:ListItem>
                                        <asp:ListItem Value="Ajax 5 Stars with Increments">Ajax 5 Stars with Increments</asp:ListItem>
                                    </asp:DropDownList> <br />
               Hide Review List: <asp:CheckBox ID="HideReviewListCheckBox"  runat="server" Checked="true" />  <br />
               Review Page Size: <asp:TextBox ID="pagesize" runat="server" style="max-width:30px"></asp:TextBox>     
            <br /><br />
               
                 <!-- End To Do ..............................  -->
                 <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
            </div>
        </asp:View>
        <asp:View ID="NotPageBuilderPage" runat="server">
            <p>To view the Content Review widget's user interface, please view it within the context of a published PageBuilder page.</p>
        </asp:View>
    </asp:MultiView>





<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Blogs.ascx.cs" Inherits="widgets_Blogs" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/widgets/blogs/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
    <asp:Label ID="uxError" runat="server" />
    <asp:PlaceHolder ID="phContent" runat="server">
        <asp:Xml runat="server" ID="xmlBlogList"></asp:Xml>
        <asp:Label ID="errorLb" runat="server" />
        
          </asp:PlaceHolder>
        <asp:PlaceHolder ID="phHelpText" runat="server">         
            <div id="divHelpText" runat="server" style="font:normal 12px/15px arial; width:100%; height:100%;">
               Click on the 'Edit' icon (<img alt="edit icon" title="edit icon" src= "<%=appPath%>PageBuilder/PageControls/Themes/TrueBlue/images/edit_on.png" width="12" height="12" border="0" />) in the top-right corner of this widget to select the blog item you wish to display.
            </div>
        </asp:PlaceHolder>
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>" class="LSWidget">
            <div class="LSEdit">
                <div class="LSTabInterface">
                    <ul class="LSTabWrapper">
                        <li class="LSTab <%=FolderSelected%>"><a href="#ByFolder" style="cursor:pointer;"><span>Folder</span></a></li>
                        <li class="LSTab <%=PropertySelected%>""><a href="#ByProperty" style="cursor:pointer;"><span>Properties</span></a></li>
                    </ul>
                    <div class="ByFolder LSTabPanel">
                        <UC:FolderTree ID="foldertree" runat="server" />
                        <asp:Label ID="editError" runat="server" />
                    </div>
                    <div class="ByProperty LSTabPanel">
                    <table class="Edit_Gidwet">
                        <tr>
                            <td class="label">Blog ID:</td>
                            <td><asp:TextBox ID="uxBlogs" CssClass="folderid" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td class="label">Blog Name:</td>
                            <td><asp:Label ID="uxBlogTitle" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="label">Show Summary:</td>
                            <td> <asp:CheckBox ID="uxSummary" runat="server" Checked="true" /></td>
                        </tr>
                        <tr>
                            <td class="label">Show Author:</td>
                            <td><asp:CheckBox ID="uxshowAuthor" runat="server" Checked="true" /></td>
                        </tr>
                        <tr>
                            <td class="label">Show Date Creation:</td>
                            <td><asp:CheckBox ID="uxshowdate" runat="server" Checked="true" /></td>
                        </tr>
                        <tr>
                            <td class="label">Show Comment:</td>
                            <td> <asp:CheckBox ID="uxshowcomment" runat="server" Checked="true" /></td>
                        </tr>
                        <tr>
                            <td class="label">Max Results:</td>
                            <td><asp:TextBox ID="uxMaxResult" runat="server" Text="10"></asp:TextBox></td>
                        </tr>
                    </table>
                    </div>
                </div>
                <div class="CBEditControls">
                    <asp:Button ID="CancelButton" CssClass="CBCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                    <asp:Button ID="SaveButton" CssClass="CBSave" runat="server" Text="Save" OnClick="SaveButton_Click" OnClientClick="return Ektron.PFWidgets.ListSummary.Save(this);" />
                </div>
            </div>
            <input type="hidden" id="hdnAppPath" name="hdnAppPath" value="<%=appPath%>" />
            <input type="hidden" id="hdnLangType" name="hdnLangType" value="<%=langType%>" />
            <asp:HiddenField ID="hdnblogid" runat="server" EnableViewState="true" />
        </div>
    </asp:View>
</asp:MultiView>
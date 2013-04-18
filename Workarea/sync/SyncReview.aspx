<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SyncReview.aspx.cs" Inherits="Workarea_sync2_SyncReview" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>  
    
    <script type="text/javascript">
        Ektron.ready(function() {
            Ektron.Workarea.Sync.Review.Init();
        });
    </script>  
</head>
<body>
    <!-- Ektron Client Script -->
    <asp:Literal id="ektronClientScript" runat="server"></asp:Literal>
    
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
                <table>
                    <tr id="rowToolbarButtons" runat="server"></tr>
                </table>
            </div>
        </div>
        <div class="ektronPageContainer">
            <div id="reviewMessages">
                <div class="messages">
                    <asp:Literal ID="resolveResults" runat="server" /></div>
            </div>
            <div class="ektronPageTabbed">
                <div class="tabContainerWrapper">
                    <div class="ektronSync tabContainer" id="tabWrapper">
                        <ul>
                            <li id="usersTab"><a title="Users tab" href="#users"><span>
                                <asp:Literal ID="lblUsers" runat="server" /></span></a></li>
                            <li id="foldersTab"><a title="Folders tab" href="#folders"><span>
                                <asp:Literal ID="lblFolders" runat="server" /></span></a></li>
                            <li id="metadataTab"><a title="Metadata tab" href="#metadata"><span>
                                <asp:Literal ID="lblMetadata" runat="server" /></span></a></li>
                            <li id="emailTab"><a title="Email tab" href="#email"><span>
                                <asp:Literal ID="lblEmail" runat="server" /></span></a></li>
                        </ul>
                        <div id="users">
                            <div class="conflictsList">
                                <table class="conflicts" width="100%">
                                    <thead>
                                        <tr>
                                            <th class="check">
                                                <input type="checkbox" title="Check/Uncheck All" autocomplete="off" class="checkUncheckAll" value="" />
                                            </th>
                                            <th>
                                                <asp:Label ID="lblOriginalUserName" runat="server"></asp:Label>
                                            </th>
                                            <th>
                                                <asp:Label ID="lblModifiedUserName" runat="server"></asp:Label>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptUserConflicts" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="check">
                                                        <input type="checkbox" title="User Conflicts Ids" name="userConflictIds" class="conflictCheckBox"  value="<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemOriginalName" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemModifiedName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody> 
                                </table>
                            </div>
                            <ul class="ektronModalButtonWrapper ektronSyncButtons clearFix">
                                <li>
                                    <asp:LinkButton ID="btnMarkUsersReviewedAndEmail" runat="server" CssClass="greenHover button markReviewedEmailButton buttonRight"
                                        CommandName="users" /></li>
                                <li>
                                    <asp:LinkButton ID="btnMarkUsersReviewed" runat="server" CssClass="greenHover button markReviewedButton buttonRight"
                                        CommandName="users" /></li>
                            </ul>
                        </div>
                        <div id="folders">
                            <div class="conflictsList">
                                <table class="conflicts">
                                    <thead>
                                        <tr>
                                            <th class="check">
                                                <input type="checkbox" title="Check/Uncheck All" autocomplete="off" class="checkUncheckAll" value="" />
                                            </th>
                                            <th>
                                                <asp:Label ID="lblOriginalFolderName" runat="server"></asp:Label>
                                            </th>
                                            <th>
                                                <asp:Label ID="lblModifiedFolderName" runat="server"></asp:Label>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptFolderConflicts" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="check">
                                                        <input type="checkbox" title="Folder Conflict Ids" name="folderConflictIds" class="conflictCheckBox"  value="<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemOriginalName" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemModifiedName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody> 
                                </table>
                            </div>
                            <ul class="ektronModalButtonWrapper ektronSyncButtons clearFix">
                                <li>
                                    <asp:LinkButton ID="btnMarkFoldersReviewed" runat="server" CssClass="greenHover button markReviewedButton buttonRight"
                                        CommandName="folders" /></li>
                            </ul>
                        </div>
                        <div id="metadata">
                            <div class="conflictsList">
                                <table class="conflicts">
                                    <thead>
                                        <tr>
                                            <th class="check">
                                                <input type="checkbox" title="Check/Uncheck All" autocomplete="off" class="checkUncheckAll" value="" />
                                            </th>
                                            <th>
                                                <asp:Label ID="lblOriginalMetadataName" runat="server"></asp:Label>
                                            </th>
                                            <th>
                                                <asp:Label ID="lblModifiedMetadataName" runat="server"></asp:Label>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptMetadataConflicts" runat="server">                                    
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="check">
                                                        <input type="checkbox" title="Metadata Conflict Ids" name="metadataConflictIds" class="conflictCheckBox" value="<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemOriginalName" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemModifiedName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                            <ul class="ektronModalButtonWrapper ektronSyncButtons clearFix">
                                <li>
                                    <asp:LinkButton ID="btnMarkMetadataReviewed" runat="server" CssClass="greenHover button markReviewedButton buttonRight"
                                        CommandName="metadata" /></li>
                            </ul>
                        </div>
                        <div id="email">
                            <div class="conflictsList">
                                <table class="conflicts">
                                    <thead>
                                        <tr>
                                            <th class="check">
                                                <input type="checkbox" title="Check/Uncheck All" autocomplete="off" class="checkUncheckAll" value="" />
                                            </th>
                                            <th>
                                                <asp:Label ID="lblOriginalEmailName" runat="server"></asp:Label>
                                            </th>
                                            <th>
                                                <asp:Label ID="lblModifiedEmailName" runat="server"></asp:Label>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptEmailConflicts" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="check">
                                                        <input type="checkbox" title="Email Conflict Ids" name="emailConflictIds" class="conflictCheckBox" value="<%#DataBinder.Eval(Container.DataItem, "ObjectId")%>" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemOriginalName" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblItemModifiedName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody> 
                                </table>
                            </div>
                            <ul class="ektronModalButtonWrapper ektronSyncButtons clearFix">
                                <li>
                                    <asp:LinkButton ID="btnMarkEmailReviewed" runat="server" CssClass="greenHover button markReviewedButton buttonRight"
                                        CommandName="email" /></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>

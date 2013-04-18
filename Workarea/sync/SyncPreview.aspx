<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SyncPreview.aspx.cs" Inherits="SyncPreview" %>

<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../Controls/paging/paging.ascx" %>
<%@ Register Src="SyncResources.ascx" TagPrefix="ektron" TagName="SyncResources" %>
<%@ Register Src="SyncDialogs.ascx" TagPrefix="ektron" TagName="SyncDialogs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sync Preview</title>
    <link type="text/css" href="css/ektron.workarea.sync.profile.css" rel="stylesheet" />
    <link type="text/css" href="../csslib/ektron.workarea.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.dialogs.css" rel="stylesheet" />
    <link type="text/css" href="../java/plugins/modal/ektron.modal.css" rel="stylesheet" />
    <script type="text/javascript">
        Ektron.ready(function () {
            Ektron.Workarea.Sync.Relationships.Init();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <!-- Ektron Client Script -->
    <asp:Literal ID="ektronClientScript" runat="server"></asp:Literal>
    <!-- Sync String Resources -->
    <ektron:SyncResources ID="syncResources" runat="server" />
    <div class="ektronPageHeader">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronTitlebar" id="txtTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="htmToolBar" runat="server">
            <table>
                <tr id="rowToolbarButtons" runat="server">
                </tr>
            </table>
        </div>
    </div>
    <div class="ektronPageContainer ektronPageTabbed">
        <div class="tabContainerWrapper ektronPageInfo">
            <asp:Panel ID="pnlError" Visible="false" runat="server">
                <div class="errorMessage" id="divErrorMessage" runat="server">
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlPreview" runat="server">
                <div class="ui-tabs ui-widget ui-widget-content ui-corner-all">
                    <div class="previewOption">
                        <span class="optionGroup">
                            <asp:Label ID="lblPreviewDirection" runat="server" ToolTip="Preview Direction" Text="Preview Direction:"></asp:Label>
                            <asp:DropDownList ID="ddlDirection" runat="server" AutoPostBack="false">
                            </asp:DropDownList>
                        </span><span class="optionGroup">
                            <asp:Label ID="lblPreviewProvider" runat="server" ToolTip="Preview Provider" Text="Preview Provider:"></asp:Label>
                            <asp:DropDownList ID="ddlProvider" runat="server" AutoPostBack="false">
                            </asp:DropDownList>
                        </span><span class="optionGroup">
                            <asp:Button ID="btnPreview" runat="server" Text="Preview" ToolTip="Preview" OnClick="btnPreviewClick"
                                OnClientClick="Ektron.Workarea.Overlay.block();" />
                        </span>
                    </div>
                    <div id="resultWrapper" runat="server" visible="false" class="uitabs">
                        <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
                            <li id="liTabLocal" runat="server"><a href="#LocalChanges" title="Local Changes"
                                id="local change tab"><%=apicontent.EkMsgRef.GetMessage("lbl Local Changes")%></a> </li>
                            <li id="liTabRemote" runat="server"><a href="#RemoteChanges" title="Remote Changes"
                                id="remote change tab"><%=apicontent.EkMsgRef.GetMessage("lbl Remote Changes")%></a> </li>
                        </ul>
                        <div class="ektronPageInfo">
                            <asp:Panel ID="pnlFileChange" runat="server" Visible="false">
                                <div class="previewOption">
                                    <span class="optionGroup">
                                        <asp:Label ID="lblFilter" runat="server" ToolTip="Filter By" Text="Filter By:"></asp:Label>
                                        <asp:DropDownList ID="dllChanges" runat="server">
                                        </asp:DropDownList>
                                    </span><span class="optionGroup">
                                        <asp:Button ID="btnFilter" runat="server" Text="Filter" ToolTip="Filter" OnClick="btnFilterClick"
                                            OnClientClick="Ektron.Workarea.Overlay.block();" />
                                    </span>
                                </div>
                            </asp:Panel>
                            <asp:MultiView ID="mvChanges" runat="server">
                                <asp:View ID="vDB" runat="server">
                                    <div id="localdbChanges" runat="server" >
                                        <div id="LocalChanges">
                                            <div class="info">
                                                <asp:Label ID="lblLocalDBTitle" Visible="false" runat="server"></asp:Label>
                                            </div>
                                            <asp:GridView Width="100%" ID="gvLocalDBChanges" runat="server" AutoGenerateColumns="false"
                                                CssClass="ektronGrid" AllowPaging="true" GridLines="None">
                                                <PagerSettings Visible="false" />
                                                <HeaderStyle CssClass="title-header"></HeaderStyle>
                                                <Columns>
                                                    <asp:BoundField HeaderText="ID" DataField="ID">
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Language" DataField="LanguageID">
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Title" DataField="Title"></asp:BoundField>
                                                    <asp:BoundField HeaderText="Status" DataField="Status">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Folder Id" DataField="FolderID">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Editor Last Name" DataField="EditorLastName">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Editor First Name" DataField="EditorFirstName">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Edit Date" DataField="EditDate">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="18%"></ItemStyle>
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                            <ucEktron:Paging ID="ucPagingLocalDB" runat="server" />
                                            <div>
                                                <asp:Literal ID="ltrLocalDBPreview" Visible="false" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="remotedbChanges" runat="server" >
                                        <div id="RemoteChanges">
                                            <div class="info">
                                                <asp:Label ID="lblRemoteDBTitle" Visible="false" runat="server"></asp:Label>
                                            </div>
                                            <asp:GridView Width="100%" ID="gvRemoteDBChanges" runat="server" AutoGenerateColumns="false"
                                                CssClass="ektronGrid" AllowPaging="true" GridLines="None">
                                                <PagerSettings Visible="false" />
                                                <HeaderStyle CssClass="title-header"></HeaderStyle>
                                                <Columns>
                                                    <asp:BoundField HeaderText="ID" DataField="ID">
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Language" DataField="LanguageID">
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Title" DataField="Title"></asp:BoundField>
                                                    <asp:BoundField HeaderText="Status" DataField="Status">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Folder Id" DataField="FolderID">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Editor Last Name" DataField="EditorLastName">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Editor First Name" DataField="EditorFirstName">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:BoundField HeaderText="Edit Date" DataField="EditDate">
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="18%"></ItemStyle>
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                            <ucEktron:Paging ID="ucPagingRemoteDB" runat="server" />
                                            <div>
                                                <asp:Literal ID="ltrRemoteDBPreview" Visible="false" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View ID="vFile" runat="server">
                                    <div id="localFileChanges" runat="server" >
                                        <div id="LocalChanges">
                                        <div class="info">
                                            <asp:Label ID="lblLocalFileTitle" Visible="false" runat="server"></asp:Label>
                                        </div>
                                        <asp:GridView Width="100%" ID="gvLocalFileChanges" runat="server" AutoGenerateColumns="false"
                                            CssClass="ektronGrid" AllowPaging="true" GridLines="None" OnRowCreated="GridView_RowCreated" >
                                            <PagerSettings Visible="false" />
                                            <HeaderStyle CssClass="title-header"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundField HeaderText="File" DataField="FileName">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField HeaderText="Change Type" DataField="FileChangeType">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                        <ucEktron:Paging ID="ucPagingLocalFile" runat="server" />
                                        <div>
                                            <asp:Literal ID="ltrLocalFilePreview" Visible="false" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                    </div>
                                    <div id="remoteFileChanges" runat="server" >
                                        <div id="RemoteChanges">
                                        <div class="info">
                                            <asp:Label ID="lblRemoteFileTitle" Visible="false" runat="server"></asp:Label>
                                        </div>
                                        <asp:GridView Width="100%" ID="gvRemoteFileChanges" runat="server" AutoGenerateColumns="false"
                                            CssClass="ektronGrid" AllowPaging="true" GridLines="None" OnRowCreated="GridView_RowCreated">
                                            <PagerSettings Visible="false" />
                                            <HeaderStyle CssClass="title-header"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundField HeaderText="File" DataField="FileName">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField HeaderText="Change Type" DataField="FileChangeType">
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%"></ItemStyle>
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                        <ucEktron:Paging ID="ucPagingRemoteFile" runat="server" />
                                        <div>
                                            <asp:Literal ID="ltrRemoteFilePreview" Visible="false" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                    </div>
                                </asp:View>
                            </asp:MultiView>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <ektron:SyncDialogs ID="syncDialogs" runat="server" />
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CloudSync.aspx.cs" Inherits="Workarea_sync_CloudSync" %>
<%@ Register Src="SyncDialogs.ascx" TagPrefix="ektron" TagName="SyncDialogs" %>
<%@ Register Src="SyncResources.ascx" TagPrefix="ektron" TagName="SyncResources" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cloud Synchronization</title>
    <!-- Ektron Client Script -->
    <asp:Literal ID="ektronClientScript" runat="server"></asp:Literal>
    <link type="text/css" href="css/ektron.workarea.sync.profile.css" rel="stylesheet" />
    <link type="text/css" href="../csslib/ektron.workarea.css" rel="stylesheet" />
        <link type="text/css" href="../csslib/ektron.workarea.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.relationships.css" rel="stylesheet" />
    <link type="text/css" href="css/ektron.workarea.sync.dialogs.css" rel="stylesheet" />
    <link type="text/css" href="../java/plugins/modal/ektron.modal.css" rel="stylesheet" />
        <script type="text/javascript">
            Ektron.ready(function () {
                Ektron.Workarea.Sync.Relationships.Init();
            });
    </script>
</head>
<body>
    <!-- Sync String Resources -->
    <ektron:SyncResources ID="syncClientResources" runat="server" />
    <form id="form1" runat="server">
    <div class="ektronPageHeader">
        <div id="dhtmltooltip">
        </div>
        <div class="ektronTitlebar" id="divTitleBar" runat="server">
        </div>
        <div class="ektronToolbar" id="divToolBar" runat="server">
            <table>
                <tr id="rowToolbarButtons" runat="server">
                    <td id="image_cell_101" class="button" title="Sync Now">
                        
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="ektronPageContainer">
        <table id="tblProfile" runat="server">
            <tr>
                <td class="label">
                    <span>Local Connection String</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter SQL Connection String here" CssClass="textInput" ID="tbLocalSQL"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Local Site Path</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter Local Site Path here" CssClass="textInput" ID="tbLocalSitePath"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>SQL Azure Connection String</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter SQL Connection String here" CssClass="textInput" ID="tbSQLServer"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>IP Address</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter Server IP Address Here here" CssClass="textInput" ID="tbIPAddress"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Blob Storage</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter Blob Storage Name here" CssClass="textInput" ID="tbBlobStorage"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Account Name</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter Account Name here" CssClass="textInput" ID="tbAccountName"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Container Name</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter Container Name here" CssClass="textInput" ID="tbContainerName"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Account Key</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter Account Key here" CssClass="textInput" ID="tbAccountKey"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Cloud Domain</span>
                </td>
                <td class="option">
                    <asp:TextBox ToolTip="Enter Cloud Domain here" CssClass="textInput" ID="tbCloudDomain"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Direction</span>
                </td>
                <td class="option">
                    <asp:CheckBox ID="rbDirection" runat="server" Text="Upload" Checked="true" Enabled="false" />
                    <asp:CheckBox ID="RadioButton1" runat="server" Text="Download" Checked="false" Enabled="false" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Items to Synchronize</span>
                </td>
                <td class="option">
                    <asp:CheckBox ID="CheckBox1" runat="server" Text="Database" Checked="true" Enabled="false" />
                    <asp:CheckBox ID="CheckBox2" runat="server" Text="Asset Library" Checked="true" Enabled="false" />
                    <asp:CheckBox ID="CheckBox3" runat="server" Text="Assets" Checked="true" Enabled="false" /><br />
                    <asp:CheckBox ID="CheckBox4" runat="server" Text="Private Assets" Checked="true" Enabled="false" />
                    <asp:CheckBox ID="CheckBox5" runat="server" Text="Uploaded Images" Checked="true" Enabled="false" />
                    <asp:CheckBox ID="CheckBox6" runat="server" Text="Uploaded Files" Checked="true" Enabled="false" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Database Scope</span>
                </td>
                <td class="option">
                    <asp:CheckBox ID="CheckBox9" runat="server" Text="CMS Core" Checked="true" Enabled="false"  />
                    <asp:CheckBox ID="CheckBox10" runat="server" Text="History" Checked="false" Enabled="false" />
                    <asp:CheckBox ID="CheckBox11" runat="server" Text="Workflow (Ecommerce)" Checked="false" Enabled="false" /><br />
                    <asp:CheckBox ID="CheckBox12" runat="server" Text="Search" Checked="false" Enabled="false" />
                    <asp:CheckBox ID="CheckBox13" runat="server" Text="ASP.NET" Checked="false" Enabled="false" />
                    <asp:CheckBox ID="CheckBox14" runat="server" Text="Custom Tables" Checked="false" Enabled="false" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    <span>Conflict Resolution</span>
                </td>
                <td class="option">
                    <asp:CheckBox ID="CheckBox7" runat="server" Text="Source Wins" Checked="true" Enabled="false" />
                    <asp:CheckBox ID="CheckBox8" runat="server" Text="Destination Wins" Checked="false" Enabled="false" />
                </td>
            </tr>
        </table>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="SaveClick" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="CancelClick" />
        <asp:Literal ID="ltrMessages" runat="server"></asp:Literal>
    </div>
    <ektron:SyncDialogs ID="syncDialogs" runat="server" />
    </form>
</body>
</html>

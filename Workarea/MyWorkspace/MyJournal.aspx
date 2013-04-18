<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyJournal.aspx.cs" Inherits="MyWorkspace_MyJournal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>My Journal</title>
</head>
<body>
    <form id="frmContent" runat="server">
        <p>
            <asp:literal ID="ltr_journal" runat="server"></asp:literal>
        </p>
        <p>
            &#160;&#160;<asp:Button ToolTip="Create My Journal" ID="btn_Create" runat="server" Text="  Create My Journal  "  OnClick="btn_Create_Click"/>
        </p>
            <div style="width: 95%; height: 95%" id="_dvProperties">
                <table width="550">
                    <tr>
                        <td align="left" class="input-box-text" nowrap="nowrap" style="width: 100px" valign="top">
                        </td>
                        <td align="left" nowrap="nowrap" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="top">
                            &nbsp;</td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" class="input-box-text" valign="bottom" width="100%">
                        </td>
                    </tr>
                    <tr>
                        <td align="left" nowrap="nowrap" valign="top" style="width: 100px" class="input-box-text">
                            <asp:literal ID="ltr_blogname" runat="server"></asp:literal></td>
                        <td align="left" nowrap="nowrap" valign="top" style="width: 3px">
                        </td>
                        <td align="left" valign="top">
                            <asp:TextBox ToolTip="Enter Blog Name here" ID="txtBlogName" runat="server" Columns="50" MaxLength="70"></asp:TextBox><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" valign="bottom" class="input-box-text" width="100%">
                        </td>
                    </tr>
                    <tr>
                        <td align="left" valign="top" style="width: 100px" class="input-box-text">
                            <asp:literal ID="ltr_blogtitle" runat="server"></asp:literal></td>
                        <td align="left" valign="top" style="width: 3px">
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                            <asp:TextBox ToolTip="Enter Title here" ID="txtTitle" runat="server" Columns="50" MaxLength="75"></asp:TextBox><br />
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="input-box-text" style="width: 100px" valign="top">
                            <asp:Label ToolTip="Visibility" ID="lblVisibility" runat="server" CssClass="input-box-text"><asp:literal ID="ltr_visibility" runat="server"></asp:literal></asp:Label></td>
                        <td align="left" style="width: 3px" valign="top">
                        </td>
                        <td align="left" class="input-box-text" style="width: 3px; height: 21px" valign="top">
                            <asp:DropDownList ID="drpVisibility" ToolTip="Select Public or Private visibility from the Drop Down Menu" runat="server">
                                <asp:ListItem Value="0">Public</asp:ListItem>
                                <asp:ListItem Value="1">Private</asp:ListItem>
                            </asp:DropDownList></td>
                        <td align="left" style="width: 3px; height: 21px" valign="top">
                        </td>
                        <td align="left" style="width: 3px; height: 21px" valign="top">
                        </td>
                    </tr>
                    <tr>
                        <td align="left" valign="top" style="width: 100px" class="input-box-text">
                            <asp:literal ID="ltr_comments" runat="server"></asp:literal></td>
                        <td align="left" valign="top" style="width: 3px">
                        </td>
                        <td align="left" style="width: 3px; height: 21px" valign="top" class="input-box-text">
                            <asp:CheckBox ToolTip="Enable Comments" ID="chkEnable" runat="server" Text="Enable Comments" Width="250px"
                                Checked="True" onclick="javascript:UpdateBlogCheckBoxes();" /><br />
                            <asp:CheckBox ToolTip="Moderate Comments" ID="chkModerate" runat="server" Text="Moderate Comments" Width="250px"
                                /><br />
                            <asp:CheckBox ToolTip="Require Authentication" ID="chkRequire" runat="server" Text="Require Authentication" Width="250px"
                                Checked="True" />
                            </td>
                        <td align="left" style="width: 3px; height: 21px" valign="top">
                        </td>
                        <td align="left" style="width: 3px; height: 21px" valign="top">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" class="input-box-text"><asp:Literal ID="BlogEnableReplication" runat="server" /></td>
                    </tr>
                </table>
            </div>
    <script language="javascript">
		<!--//
	    <asp:Literal runat="server" id="ltr_js"></asp:Literal>
		//-->
    </script>

    </form>
</body>
</html>


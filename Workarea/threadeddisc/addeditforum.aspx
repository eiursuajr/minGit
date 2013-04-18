<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addeditforum.aspx.cs" Inherits="threadeddisc_addeditforum" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <title>Add Edit Forum</title>
    <style type="text/css">
        form {margin-top:-1px;}
    </style>
</head>
<body>
    <asp:literal runat="server" ID="ltr_reload_js" />
    <form id="form1" runat="server">
        <div class="ektronPageInfo">
		    <table class="ektronGrid">
		        <tbody>
                    <tr>
                        <td class="label" title="Name"><asp:Literal ID="ltr_adf_name" runat="server"/>:</td>
                        <td><asp:TextBox ToolTip="Enter Name here" ID="txt_adf_forumname" runat="server" MaxLength="70"/></td>
                    </tr>
                    <tr>
                        <td class="label" title="ID"><asp:Literal ID="ltr_forumid" runat="server"/></td>
                        <td><asp:Literal ID="ltr_forumid_data" runat="server"/></td>
                    </tr>

                    <tr>
                        <td class="label" title="Description"><asp:Literal ID="ltr_adf_title" runat="server"/>:</td>
                        <td><asp:TextBox ToolTip="Enter Description here" ID="txt_adf_forumtitle" runat="server" MaxLength="75"/></td>
                    </tr>
                    <tr>
                        <td class="label" title="Sort Order"><asp:Literal ID="ltr_adf_sortorder" runat="server"/>:</td>
                        <td><asp:TextBox ToolTip="Select the Sort Order from the Drop Down Menu" ID="txt_adf_sortorder" CssClass="ektronTextXXXSmall" runat="server" Text="1" MaxLength="10" /></td>
                    </tr>
                    <tr>
                        <td class="label" title="Subject"><asp:Literal ID="ltr_adf_category" runat="server"/>:</td>
                        <td><asp:DropDownList ToolTip="Select a Subject from the Drop Down Menu" ID="drp_adf_category" runat="server" /></td>
                    </tr>
                    <tr id="TR_moderate" runat="server" >
                        <td class="label" title="Moderate Comments"><asp:Literal ID="ltr_adf_moderate" runat="server"/>:</td>
                        <td><asp:CheckBox ToolTip="Moderate Comments" ID="chk_adf_moderate" runat="server" Checked="True" /></td>
                    </tr>
                    <tr>
                        <td class="label" title="Lock Forum"><asp:Literal ID="ltr_lock" runat="server" />:</td>
                        <td>
                            <asp:CheckBox ToolTip="Lock Discussion Forum" ID="chk_adf_lock" runat="server" Checked="False" />
                            <asp:Panel id="pnlLocked" Visible="false" runat="server">
                                <asp:Label ToolTip="Board is currently locked" ID="chk_board_locked" runat="server" Text="NOTE: Board is currently locked" />
                            </asp:Panel>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <asp:Literal ID="ltr_adf_dynreplication" runat="server"/>
        <asp:HiddenField ID="hdn_adf_forumname" runat="server" />
        <script language="javascript" type="text/javascript">
            <asp:Literal ID="ltr_af_js" runat="server"/>
        </script>
        <asp:HiddenField ID="hdn_adf_folderid" runat="server" />
    </form>
</body>
</html>


<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeFile="addeditcontentflag.aspx.cs" Inherits="addeditcontentflag" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>AddEditContentFlag</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="ltr_js" runat="server"/>
    <div class="selected_editor" id="_dvContent"">
        <table id="Table8" class="ektronGrid">
            <tr>
                <td colspan="2">
                </td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Literal ID="ltr_uname" runat="server"/>
                </td>
                <td>
                    <asp:Literal ID="ltr_uname_data" runat="server"/>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Literal ID="ltr_date" runat="server"/>
                </td>
                <td>
                    <asp:Literal ID="ltr_date_data" runat="server"/>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Literal ID="ltr_flag" runat="server"/>
                </td>
                <td>
                    <asp:DropDownList id="drp_flag_data" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Literal ID="ltr_comment" runat="server"/>                
                </td>
                <td>
                    <asp:TextBox TextMode="MultiLine" ID="txt_comment" runat="server" Columns="50" Rows="15" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hdn_action" runat="server" />
    <asp:HiddenField ID="hdn_folderid" runat="server" />
    </form>
</body>
</html>


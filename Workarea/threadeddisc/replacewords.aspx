<%@ Page Language="C#" AutoEventWireup="true" CodeFile="replacewords.aspx.cs" Inherits="threadeddisc_replacewords" ValidateRequest="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Replace Words</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageContainer">
            <div id="divAE" runat="server">
                <div class="ektronPageInfo">
                    <table class="ektronForm">
                        <tr>
                            <td class="label" title="Old Word"><asp:Literal ID="ltr_old" runat="server" />:</td>
                            <td class="value"><asp:TextBox ToolTip="Enter Old Word here" ID="txt_old" runat="server" MaxLength="50" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="New Word"><asp:Literal ID="ltr_new" runat="server" />:</td>
                            <td class="value"><asp:TextBox ToolTip="Enter New Word here" ID="txt_new" runat="server" MaxLength="50" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Language"><asp:Literal ID="ltr_lang" runat="server" />:</td>
                            <td class="value"><asp:DropDownList ToolTip="Select Language from Drop Down Menu" ID="drp_lang" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="Regex"><asp:Literal ID="ltr_regex" runat="server" />:</td>
                            <td class="value"><asp:CheckBox ToolTip="Regex Option" ID="chk_regex" runat="server" /></td>
                        </tr>
                        <tr id="tr_applies" runat="server">
                            <td class="label" title="Applies To"><asp:Literal ID="ltr_appliesto" runat="server" />:</td>
                            <td class="value"><asp:DropDownList ToolTip="Select what word replacement applies to from the Drop Down Menu" ID="drp_boards" runat="server" /></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="divList" runat="server">
                <div class="ektronPageGrid">
                    <asp:datagrid ID="dgReplace"
                        runat="server"
                        AutoGenerateColumns="false"
                        Width="100%"
                        CssClass="ektronGrid"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                    </asp:datagrid>
                </div>
            </div>
        </div>
    </form>
</body>
</html>


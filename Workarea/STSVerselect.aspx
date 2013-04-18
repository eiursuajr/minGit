<%@ Page Language="C#" AutoEventWireup="true" CodeFile="STSVerselect.aspx.cs" Inherits="Workarea_STSVerselect" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <div id="pnl_OfficeVerSelector" runat="server" visible="true" class="ui-helper-clearfix" style="margin-bottom: .5em">
                                <asp:Literal runat="server" ID="lit_VerionSelect"></asp:Literal><br />
                                <asp:RadioButtonList runat="server" ID="rbl_OfficeVersion">
                                    <asp:ListItem Text="" Value="2010" />
                                    <asp:ListItem Text="" Value="other" />
                                </asp:RadioButtonList><br />
                                <asp:Button runat="server" ID="btn_VersionSelect" Text="OK" OnClick="btn_VersionSelect_Click" />

                            </div>
    </div>
    </form>
</body>
</html>

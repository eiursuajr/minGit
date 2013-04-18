<%@ Page Language="C#" AutoEventWireup="true" CodeFile="devicepreview.aspx.cs" Inherits="Workarea_devicepreview" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Preview</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table class="ektronGrid">
              <tr>
                    <td class="label"><%=_MessageHelper.GetMessage("lbl Device")%>:</td>
                    <td class="value">
                        <asp:DropDownList ID="ddlDevices" AutoPostBack="true"  runat="server"></asp:DropDownList>
                    </td>
              </tr>
              <tr>
                    <td class="label"><%=_MessageHelper.GetMessage("lbl Orientation")%>:</td>
                    <td class="value">
                        <asp:RadioButtonList ID="rbList" AutoPostBack="true" runat="server" >
                            <asp:ListItem Text="Portrait" Selected="True" ></asp:ListItem>
                            <asp:ListItem Text="Landscape"></asp:ListItem>
                        </asp:RadioButtonList>

                    </td>
              </tr>
              <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="btnPreview" CssClass="button buttonPreview" Text="Preview" runat="server" OnClick="btnPreview_Click" />
                        <asp:Button ID="btnCancel" CssClass="button buttonCancel" Text="Cancel" runat="server" OnClientClick="window.close();"/>
                    </td>
              </tr>
	     </table>
    </div>
    </form>
</body>
</html>

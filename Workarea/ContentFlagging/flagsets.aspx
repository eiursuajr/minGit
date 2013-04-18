<%@ Page Language="C#" AutoEventWireup="true" CodeFile="flagsets.aspx.cs" Inherits="ContentFlagging_flagsets" ValidateRequest="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Flagging Definitions</title>
    <meta http-equiv="Pragma" content="no-cache"/>
    <script type="text/javascript">
        Ektron.ready( function()
            {
                var tabsContainers = $ektron(".tabContainer");            
                tabsContainers.tabs();
            }
        );
    </script>    
    <style type="text/css">
        a.flagEdit {
            background-image:url(../images/UI/Icons/contentEdit.png);
            background-position: left center;
            background-repeat:no-repeat;
            display:inline block;
            margin:0;
            padding:0 0 0  1.75em;
        }
    </style>
</head>
<body>
    <form id="frmContent" runat="server">
        <asp:literal runat="server" ID="ltr_js" />			
        <asp:Literal ID="ltr_view" runat="Server" />
        <div id="tbledit" class="ektronPageContainer ektronPageInfo" runat="server">
	        <table class="ektronGrid">							
                <tr>
                    <td class="label" title="<%= this.GetMessage("generic name")%>"><asp:Literal ID="ltr_name" runat="server" Text="Name" /></td>
                    <td><asp:TextBox ToolTip="Name" ID="txt_fd_name" runat="server" Columns="50" MaxLength="50" /></td>
                </tr>
                <tr>
                    <td class="label" title="<%=this.GetMessage("generic description")%>"><asp:Literal ID="ltr_desc" runat="server" Text="Description" /></td>
                    <td><asp:TextBox ToolTip="Description" ID="txt_fd_desc" runat="server" Columns="50" MaxLength="255" /></td>
                </tr>
            </table> 
            <div class="ektronTopSpace"></div>
            <table class="ektronGrid">
                <tr>
                    <td style="width:8em" class="label" title="<%= this.GetMessage("generic options")%>">
                        <%= this.GetMessage("generic options")%>
                    </td>
                    <td>
                        <asp:Literal ID="ltr_options" runat="server" />
                        <asp:HiddenField ID="hdn_fd_name" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </form>    
</body>
</html>



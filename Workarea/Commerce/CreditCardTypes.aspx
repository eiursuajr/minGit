<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreditCardTypes.aspx.cs" Inherits="Commerce_cctypes" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <title>Credit Card Types</title>
    <asp:Placeholder id="phLiteralJs" runat="server">
        <asp:literal id="ltr_js" runat="server" />
    </asp:Placeholder>
    <style type="text/css">
        
    </style>
</head>
<body onclick="MenuUtil.hide()">
    <script type="text/javascript" language="javascript">
        function RemoveCCImage(path) {
	        var elem = null;
	        var elemThumb = null;
	        elem = document.getElementById( '<%=cc_image.ClientID%>' );
	        if (elem != null)
	        {
	            elem.value = '';
	        }
	        elemThumb = document.getElementById( '<%=cc_image_thumb.ClientID%>' );
	        if ( elemThumb != null )
	        {
	            elemThumb.src = path;
	        }
	    }
	    function resetPostback()
        {
            document.forms[0].isPostData.value = "";
        }
    </script>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
            <asp:Panel ID="pnl_viewall" CssClass="ektronPageContainer ektronPageGrid" runat="Server">
            <asp:DataGrid ID="dg_cctypes"
                runat="server"
                CssClass="ektronGrid"
                AutoGenerateColumns="false"
                Width="100%"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
                <Columns>
                    <asp:BoundColumn DataField="id" HeaderText="Id"></asp:BoundColumn>
                    <asp:HyperLinkColumn DataTextField="Name" HeaderText="Name" DataNavigateUrlField="id" DataNavigateUrlFormatString="creditcardtypes.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                    <asp:TemplateColumn HeaderText="Accepted" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate><asp:CheckBox ToolTip="Status (Accept or Unaccept)" ID="chk_accepted" runat="server" Enabled="false" Checked='<%#(DataBinder.Eval(Container.DataItem, "isAccepted"))%>' /></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Image" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate><%#Util_ShowImage(DataBinder.Eval(Container.DataItem, "image").ToString())%></ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <p class="pageLinks">
                <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
            </p>
            <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
            <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
            <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
        </asp:Panel>
            <asp:Panel ID="pnl_view" Cssclass="ektronPageContainer ektronPageInfo" runat="Server" Visible="false">
                <table id="tblmain" class="ektronGrid" runat="server">
                <tr>
                    <td class="label"><asp:Literal ID="ltr_name" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_name" runat="server" MaxLength="25" /></td>
                </tr>
                <tr id="tr_id" runat="server">
                    <td class="label"><asp:Literal ID="ltr_id" runat="server" />:</td>
                    <td><asp:Label ID="lbl_id" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Literal ID="ltr_image" runat="server" />:</td>
                    <td>
                        <span id="sitepath"><asp:Literal ID="ltr_sitepath" runat="Server" /></span>
                        <asp:TextBox id="cc_image" runat="server" ReadOnly="true" />
                        <asp:PlaceHolder ID="pnl_edit" runat="server">
                            <a href="#" class="button buttonInline greenHover buttonChange" onclick="PopUpWindow('../mediamanager.aspx?scope=images&upload=true&retfield=<%=cc_image.ClientID%>&showthumb=false&autonav=0', 'Meadiamanager', 790, 580, 1,1);return false;"><%=this.GetMessage("lbl change")%></a>
                            <a href="#" class="button buttonInline greenHover buttonRemove" onclick="RemoveCCImage('../images/application/spacer.gif');return false"><%=this.GetMessage("custom prop remove item")%></a>
                        </asp:PlaceHolder>
                        <div>                   
                            <asp:Image ID="cc_image_thumb" runat="server" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="label"><asp:Literal ID="ltr_regex" runat="server" />:</td>
                    <td><asp:TextBox ID="txt_regex" runat="server" MaxLength="50" /></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Literal ID="ltr_accepted" runat="server" />
                    </td>
                    <td>
                        <asp:checkbox ToolTip="Status (Accept or Unaccept)" ID="chk_accepted" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        </div>
    </form>
</body>
</html>


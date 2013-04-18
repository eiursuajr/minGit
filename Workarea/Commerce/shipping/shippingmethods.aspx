<%@ Page Language="C#" AutoEventWireup="true" CodeFile="shippingmethods.aspx.cs" Inherits="Commerce_shipping_methods" %>
<%@ Register src="../../controls/Reorder/Reorder.ascx" tagname="Reorder" tagprefix="workarea" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <title>Shipping Methods</title>
    <asp:literal id="ltr_js" runat="server" />
    <script type="text/javascript">
        function resetPostback()
        {
            document.forms[0].isPostData.value = "";
        }

        function redirectIframe(iframeSelector, newUrl)
        {
            var iframe = $ektron(iframeSelector);
            $ektron(iframeSelector).attr("src", newUrl);
        }

        Ektron.ready(function()
        {
            var modal = new String();
            modal = '<div class="ektronWindow ektronModalStandard ektronShippingReorderModal" id="ektronShippingReorderModal">\n';
            modal += '    <iframe src=" " noresize="noresize" frameborder="0" border="0"  marginwidth="0" marginheight="0" id="ektronShippingReorderIframe" class="ektronShippingReorderIframe" scrolling="auto"></iframe>\n';

            modal += '</div>\n';

            var pageBody = $ektron("body");
            pageBody.append(modal);

            var addPage = $ektron(".ektronShippingReorderModal");
                    addPage.modal({
                        modal: true,
                        toTop: true,
                        overlay: 0,
                        onShow: function(hash) {
                            addPage.css("margin-top", -1 * Math.round(addPage.outerHeight()/2));
			                hash.o.fadeTo("fast", 0.5, function() {
				                hash.w.fadeIn("fast");
			                });
                        },
                        onHide: function(hash) {
                            hash.w.fadeOut("fast");
			                hash.o.fadeOut("fast", function(){
				                if (hash.o)
				                {
					                hash.o.remove();
			                    }
			                });
                        }
                    });
        });
        function OpenReorder()
        {
            redirectIframe('.ektronShippingReorderIframe', '<asp:literal id="ltr_appPath" runat="server" />commerce/shipping/shippingmethods.aspx?action=reorder');
            $ektron(".ektronShippingReorderModal").modalShow();
        }
    </script>
    <style type="text/css">
        form {position:relative;margin-top:-1px;}
        div.ektronShippingReorderModal {width: 50em; margin-left: -25em;}
        .ektronShippingReorderModal iframe {width: 100%; height: 20em;}
        div.ektronModalStandard div.ektronModalBody {padding: 0 !important;}
    </style>
</head>
<body onclick="MenuUtil.hide()">
    <form id="form1" runat="server">
	<div class="ektronPageContainer">
        <asp:Panel cssclass="ektronPageGrid" ID="pnl_viewall" runat="Server">
            <asp:DataGrid ID="dg_viewall"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="ektronGrid"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
                <Columns>
                    <asp:HyperLinkColumn DataTextField="id" HeaderText="Id" DataNavigateUrlField="id" DataNavigateUrlFormatString="shippingmethods.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                    <asp:HyperLinkColumn DataTextField="Name" HeaderText="Name" DataNavigateUrlField="id" DataNavigateUrlFormatString="shippingmethods.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                    <asp:BoundColumn DataField="DisplayOrder" HeaderText="Order"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ProviderService" HeaderText="Service"></asp:BoundColumn>
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
        <asp:Panel ID="pnl_view" runat="Server" Visible="false">
            <div class="ektronPageInfo">
                <table id="tblmain" class="ektronGrid" runat="server">
                    <tr>
                        <td class="label"><asp:Literal ID="ltr_name" runat="server"/>:</td>
                        <td class="value"><asp:TextBox ID="txt_name" runat="server" MaxLength="25" /></td>
                    </tr>
                    <tr id="tr_id" runat="server">
                        <td class="label"><asp:Literal ID="ltr_id" runat="server"/>:</td>
                        <td class="readOnlyValue"><asp:Label ID="lbl_id" runat="server" /></td>
                    </tr>
                    <tr id="tr1" runat="server">
                        <td class="label">
                            <asp:Literal ID="ltr_active" runat="server"/>
                        </td>
                        <td>
                            <asp:CheckBox ToolTip="Activate/Deactivate Shipping Method" ID="chk_active" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><asp:Literal ID="ltr_provservice" runat="server"/>:</td>
                        <td class="value">
                            <asp:TextBox ID="txt_provservice" runat="server" MaxLength="25"/>
                            <asp:Literal id="ltr_viewopt" runat="server"/>
                            <br />
                            <br />
                            <div id="dvOptions"></div>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnl_reorder" runat="Server" Visible="false">
            <workarea:Reorder ID="Reorder1" runat="server" />
        </asp:Panel>
	</div>
    </form>
</body>
</html>


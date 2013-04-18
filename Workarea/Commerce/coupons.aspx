<%@ Page Language="C#" AutoEventWireup="true" CodeFile="coupons.aspx.cs" Inherits="Commerce_coupons" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Coupons</title>
    <asp:literal id="ltr_js" runat="server" />
    <script type="text/javascript" language="javascript">
    function resetPostback()
    {
        document.forms[0].isPostData.value = "";
    }
    function GetEntryImage(entrytype) {
        var sImage = '../images/ui/icons/brick.png';
        switch(entrytype) {
            case 1 :
                sImage = '../images/ui/icons/bricks.png';
                break;
            case 2 :
                sImage = '../images/ui/icons/box.png';
                break;
            case 3 :
                sImage = '../images/ui/icons/package.png';
                break;
        }
        return sImage;
    }

    function getCurrency()
    {
        var currency = document.getElementById('sel_currency');
        var splitString = currency.value.split(";");
		var idSplit = splitString[0].split(":");
		var id = idSplit[1];
		var labelSplit = splitString[1].split(":");
		var label = unescape(labelSplit[1]);
		var symbolSplit = splitString[2].split(":");
		var symbol = symbolSplit[1];
        var hdn_currency = document.getElementById("hdn_currency");
        id = id.replace("ektron_Pricing_","");
        hdn_currency.value = id;
    }
    </script>
</head>
<body onclick="MenuUtil.hide()" style="min-height:30em;height:auto;padding-bottom:15em;background-color:White;">
    <form id="form1" runat="server">
    <div>
         <asp:Panel ID="pnl_view" runat="Server" Visible="false">
         <div class="selected_editor" id="_dvProp">
            <table id="tblmain" class="ektronForm" runat="server">
                <tr>
                    <td class="label" title="Code"><asp:Literal ID="ltr_code" runat="server" />:</td>
                    <td><asp:textbox ToolTip="Enter Coupon Code here" ID="txt_code" runat="server" /></td>
                </tr>
                <tr id="tr_id" runat="server">
                    <td class="label" title="ID"><asp:Literal ID="lbl_id" runat="server" />:</td>
                    <td><asp:Literal ID="ltr_id" runat="server" /></td>
                </tr>
                <tr id="tr_used" runat="server">
                    <td class="label" title="Used"><asp:Literal ID="lbl_used" runat="server" />:</td>
                    <td><asp:CheckBox ToolTip="Used Option" Enabled="false" ID="chk_used" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label" title="Description"><asp:Literal ID="ltr_desc" runat="server" />:</td>
                    <td><asp:textbox ToolTip="Enter Coupon Description here" ID="txt_desc" runat="server" Columns="50" /></td>
                </tr>
                <tr>
                    <td class="label" title="Active"><asp:Literal ID="ltr_active" runat="server" />:</td>
                    <td><asp:CheckBox ToolTip="Click here to make Coupon Active" ID="chk_active" runat="server" Checked="true" /></td>
                </tr>
                <tr>
                    <td class="label" title="Discount"><asp:Literal ID="ltr_discountval" runat="server" />:</td>
                    <td>
                        <asp:textbox ToolTip="Enter Discount here" ID="txt_discountval" MaxLength="9" runat="server" Columns="5" />
                        <asp:DropDownList ToolTip="Select Discount Type from the Drop Down Menu" ID="drp_discounttype" runat="server"></asp:DropDownList>
                        <asp:Literal ID="ltr_drpCurrency" runat="server" />
                    </td>
                </tr>
            </table>
            <input runat="server" id="hdn_currency" name="hdn_currency" type="hidden" value="" />
        </div>
        <div class="unselected_editor" id="_dvType">
            <table class="ektronForm">
                <tr>
                    <td class="label" title="Coupon Type"><asp:Literal ID="ltr_type" runat="server" />:</td>
                    <td><asp:RadioButtonList ToolTip="Coupon Type" ID="rad_type" runat="server" /></td>
                </tr>
            </table>
        </div>
        <div class="unselected_editor" id="_dvOptions">
            <table class="ektronForm">
                <tr>
                    <td class="label" title="One Per Customer"><asp:Literal ID="ltr_oneper" runat="server" />:</td>
                    <td><asp:CheckBox ToolTip="Click here to restrict Coupon to One Per Customer" ID="chk_oneper" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label" title="Expires after N uses"><asp:Literal ID="ltr_expireafter" runat="server" />:</td>
                    <td><asp:textbox ToolTip="Enter the Number of Uses until the Coupon Expires here" ID="txt_expireafter" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label" title="Minimum Amount"><asp:Literal ID="ltr_minamount" runat="server" />:</td>
                    <td><asp:textbox ToolTip="Enter Minimum Amount here" ID="txt_minamount" MaxLength="9" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label" title="Maximum Amount"><asp:Literal ID="ltr_maxamount" runat="server" />:</td>
                    <td><asp:textbox ToolTip="Enter Maximum Amount here" ID="txt_maxamount" MaxLength="9" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label" title="Start Date"><asp:Literal ID="ltr_startdate" runat="server" />:</td>
                    <td title="Enter a Start Date here or select a Date From the Calendar"><asp:Literal ID="ltr_startdatesel" runat="Server" /></td>
                </tr>
                <tr>
                    <td class="label" title="End Date"><asp:Literal ID="ltr_enddate" runat="server" />:</td>
                    <td title="Enter an End Date here or select a Date From the Calendar"><asp:Literal ID="ltr_enddatesel" runat="Server" /></td>
                </tr>
            </table>
        </div>

        <div title="Select entries that the coupon applies to. Not selecting any will make the coupon apply to all. " class="unselected_editor" id="_dvApplies">
        <asp:Literal ID="ltr_instructions" runat="server" />
            <table width="100%" border="1" style="border-color:#d8e6ff">
                <tr>
                    <td>
                        <table cellspacing="10" width="100%">
                            <tr>
                                <td>
                                    <table style="text-align:center" cellspacing="0" cellpadding="0" border="0" width="100%">
                                        <tr>
                                            <td style="width:50%" title="Coupon Applies To...">
                                                <asp:Literal ID="ltr_appliesto" runat="server" />
                                                <p runat="server" id="para_options">
                                                    <img src="../images/ui/icons/add.png" alt=" Add " title=" Add " onclick="AddItem();" /> <a href="#" onclick="AddItem();">Add</a>
                                                    &nbsp;&nbsp;|&nbsp;&nbsp;
                                                    <img src="../images/ui/icons/delete.png" alt="Remove" title="Remove" onclick="DeleteItem();" /> <a href="#" onclick="DeleteItem();">Remove</a>
                                                </p>
        		                            </td>
	                                    </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>

        </asp:Panel>
        <asp:Panel CssClass="ektronPageContainer ektronPageGrid" ID="pnl_viewall" runat="Server">
            <asp:DataGrid ID="dg_coupon"
                runat="server"
                AutoGenerateColumns="false"
                Width="100%"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
                <Columns>
                    <asp:HyperLinkColumn DataTextField="Id" HeaderText="Id" DataNavigateUrlField="Id" DataNavigateUrlFormatString="coupons.aspx?action=view&id={0}" HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="center"></asp:HyperLinkColumn>
                    <asp:HyperLinkColumn DataTextField="Code" HeaderText="Code" DataNavigateUrlField="Id" DataNavigateUrlFormatString="coupons.aspx?action=view&id={0}"></asp:HyperLinkColumn>
                    <asp:TemplateColumn HeaderText="Active">
                        <ItemTemplate><asp:CheckBox ToolTip="Coupon Active" ID="chk_coupon_active" runat="Server" Enabled="false" Checked='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' /></ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="Description" HeaderText="Description"></asp:BoundColumn>
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
        </asp:Panel>
    </div>
    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
    <asp:Literal ID="ltr_endJS" runat="Server" />
    </form>
</body>
</html>


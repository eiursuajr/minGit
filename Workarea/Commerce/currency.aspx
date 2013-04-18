<%@ Page Language="C#" AutoEventWireup="true" Inherits="Commerce_currency" CodeFile="currency.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
		<title>Currencies</title>
		<script language="javascript" type="text/javascript">
	        <asp:literal id="ltr_js" runat="server" />

            function searchCurrency(){
                var searchTerms = "";
                if ($ektron("#txtSearch").getInputLabelValue())
                {
                    searchTerms = $ektron("#txtSearch").getInputLabelValue();
                    window.location = "currency.aspx?search=" + searchTerms;
                }
                return false;
            }

	        function VerifyForm () {
			    document.forms[0].txt_name.value = Trim(document.forms[0].txt_name.value);
			    document.forms[0].txt_exchangerate.value = Trim(document.forms[0].txt_exchangerate.value);
			    document.forms[0].txt_numericisocode.value = Trim(document.forms[0].txt_numericisocode.value);
			    var currencyTitle=document.forms[0].txt_name.value;
			    if (document.forms[0].txt_name.value == "")
			    {
			        alert('<asp:Literal id="ltr_nameReq" runat="server" />');
				    document.forms[0].txt_name.focus();
				    return false;
			    }
			    else if((currencyTitle.indexOf('<') > -1) ||(currencyTitle.indexOf('>') > -1))
			    {
			       //alert('<asp:Literal id="ltr_nameCantHave" runat="server" /> &gt; or &lt;');
			       document.forms[0].txt_name.focus();
			       return false;
			    }
			    if (document.forms[0].txt_exchangerate.value == "" || isNaN(document.forms[0].txt_exchangerate.value))
			    {
				    alert('<asp:Literal id="ltr_rateNotNumeric" runat="server" />');
				    document.forms[0].txt_exchangerate.focus();
				    return false;
			    }

			    if (document.forms[0].txt_exchangerate.value != ""
			        && document.forms[0].chk_enabled.checked
			        && !isNaN(document.forms[0].txt_exchangerate.value)
			        && 0 >= parseFloat(document.forms[0].txt_exchangerate.value) )
			    {
				    alert ('<asp:Literal id="ltr_rateGrtZero" runat="server" />');
				    document.forms[0].txt_exchangerate.focus();
				    return false;
			    }

			    if (document.forms[0].txt_numericisocode.value == "" || isNaN(document.forms[0].txt_numericisocode.value) || !isInteger(document.forms[0].txt_numericisocode.value) || document.forms[0].txt_numericisocode.value >= 2147483648)
			    {
				    alert ('<asp:Literal id="ltr_notInteger" runat="server" />');
				    document.forms[0].txt_numericisocode.focus();
				    return false;
			    }
			    return true;
		    }
		    function ConfirmDelete() {
		        var sIdList = "";
		        var frmObj = document.forms[0];
		        for (var i=0;i<frmObj.elements.length;i++) {
                    var e = frmObj.elements[i];
                    if ( (e.type=='checkbox') && (e.checked) ) {
                        if (sIdList != "") { sIdList = sIdList + ',' + e.value; }
                        else { sIdList = e.value; }
                    }
                }
		        if (sIdList != "") {
			        if (confirm('<asp:Literal id="ltr_delSelCur" runat="server" />')) { window.location = "currency.aspx?action=delete&IDs=" + sIdList; }
			    } else {
			        alert('<asp:Literal id="ltr_errNoCurSel" runat="server" />');
			    }
		    }
		   function isInteger(s)
           {
               return s.length > 0 && !(/[^0-9]/).test(s);
           }

		    function SubmitForm(Validate) {
			    if (Validate.length > 0) {
				    if (eval(Validate)) {
					    document.forms[0].submit();
					    return false;
				    }
				    else {
					    return false;
				    }
			    }
			    else {
				    document.forms[0].submit();
				    return false;
			    }
		    }
            function resetPostback()
            {
                document.forms[0].isPostData.value = "";
            }
		</script>
    <style type="text/css">
        .Menu{
            margin-top: 9px !important;
        }
    </style>

		<!--[if lt IE 8]>
        <style type="text/css">
            input#btnSearch {float: none; display: block;}
        </style>
        <![endif]-->
	</head>
	<body onclick="MenuUtil.hide()">
		<form id="subscription" method="post" runat="server" onsubmit="return false;" class="modal-content settings-currencies">
		<div class="ektronPageContainer">
		    <div id="tr_addedit" runat="server">
		        <div class="ektronPageInfo">
			        <table class="ektronGrid">
				        <tr>
					        <td class="label"><asp:Literal id="ltr_name" runat="server" />:</td>
					        <td class="value"><asp:textbox id="txt_name" runat="server" MaxLength="50" /></td>
				        </tr>
				        <tr>
					        <td class="label"><asp:Literal id="ltr_numericisocode" runat="server" />:</td>
					        <td class="value"><asp:textbox id="txt_numericisocode" runat="server" MaxLength="9" /></td>
				        </tr>
				        <tr>
					        <td class="label"><asp:Literal id="ltr_alphaisocode" runat="server" />:</td>
					        <td class="value"><asp:textbox id="txt_alphaisocode" runat="server" MaxLength="3" Columns="3" /></td>
				        </tr>
				        <tr>
					        <td class="label"><asp:Literal id="ltr_enabled" runat="server" />:</td>
					        <td class="value"><asp:CheckBox ToolTip="Enable/Disable Currency" ID="chk_enabled" runat="server" /></td>
				        </tr>
				        <tr>
					        <td class="label"><asp:Literal id="ltr_exchangerate" runat="server" />:</td>
					        <td class="value"><label id="lbl_1USD">1 <asp:Literal ID="ltr_defaultcurrency" runat="Server" /> = </label><asp:textbox Width="275px" id="txt_exchangerate" MaxLength="12" runat="server" /><asp:Literal runat="server" ID="ltr_ISOAlpha" /></td>
				        </tr>
			        </table>
			    </div>
		    </div>
		    <div id="tr_viewall" runat="server">
			    <div class="ektronPageGrid">
			        <asp:datagrid id="ViewSubscriptionGrid"
			            Runat="server"
			            Width="100%"
			            AutoGenerateColumns="False"
                        CssClass="ektronGrid"
			            GridLines="None">
			            <HeaderStyle CssClass="title-header" />
			            <Columns>
			                <asp:TemplateColumn ItemStyle-Width="20px" HeaderStyle-CssClass="center" ItemStyle-CssClass="center"  HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate><input type="checkbox" name='chk_email_<%#DataBinder.Eval(Container.DataItem, "id")%>' id='chk_email_<%#DataBinder.Eval(Container.DataItem, "id")%>' value='<%#DataBinder.Eval(Container.DataItem, "id")%>' /></ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:HyperLinkColumn DataTextField="id" ItemStyle-Width="60px" HeaderStyle-CssClass="center" ItemStyle-CssClass="center" HeaderText="<a href='currency.aspx?sortcriteria=Id'>Id</a>" DataNavigateUrlField="id" DataNavigateUrlFormatString="javascript:ektb_show('','currency.aspx?action=edit&id={0}&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);" SortExpression="Id"></asp:HyperLinkColumn>
                            <asp:HyperLinkColumn DataTextField="Name" HeaderText="<a href='currency.aspx?sortcriteria=Name'>Name</a>" DataNavigateUrlField="id" DataNavigateUrlFormatString="javascript:ektb_show('','currency.aspx?action=edit&id={0}&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);"></asp:HyperLinkColumn>
                            <asp:HyperLinkColumn DataTextField="AlphaIsoCode" HeaderText="<a href='currency.aspx?sortcriteria=AlphaIsoCode'>AlphaIsoCode</a>" DataNavigateUrlField="id" DataNavigateUrlFormatString="javascript:ektb_show('','currency.aspx?action=edit&id={0}&thickbox=true&EkTB_iframe=true&height=300&width=500&modal=true', null);"></asp:HyperLinkColumn>
                            <asp:TemplateColumn ItemStyle-Width="60px" HeaderStyle-CssClass="center" ItemStyle-CssClass="center" HeaderText="<a href='currency.aspx?sortcriteria=Enabled'>Enabled</a>">
                                <ItemTemplate>
                                    <asp:CheckBox ToolTip="Enable/Disable" ID="chk_enabled" runat="server" Enabled="false" Checked='<%#DataBinder.Eval(Container.DataItem, "enabled")%>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderStyle-CssClass="center" ItemStyle-CssClass="center" HeaderText="<a href='currency.aspx?sortcriteria=Enabled'>Enabled</a>">
                                <ItemTemplate><asp:CheckBox ToolTip="Enable/Disable" ID="chk_enabled" runat="server" Enabled="false" Checked='<%#DataBinder.Eval(Container.DataItem, "enabled")%>' /></ItemTemplate>
                            </asp:TemplateColumn>
			            </Columns>
			        </asp:datagrid>
		            <p class="pageLinks" runat="server" id="paginglinks">
                        <asp:Label ToolTip="Page" runat="server" ID="PageLabel">Page</asp:Label>
                        <asp:textbox ID="CurrentPage" CssClass="pageLinks"  MaxLength="5" Columns="2" runat="server" />
                        <asp:Label ToolTip="of" runat="server" ID="OfLabel">of</asp:Label>
                        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                        <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
                        <asp:HyperLink runat="server" CssClass="pageLinks" ID="lnk_gotopage" >[Go To Page]</asp:HyperLink>
                        <br />
                        <br />
                        <asp:HyperLink ToolTip="First Page" runat="server" CssClass="pageLinks" ID="lnk_first" Text="[First Page]" />
                        <asp:HyperLink ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="lnk_previous" Text="[Previous Page]" />
                        <asp:HyperLink ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="lnk_next" Text="[Next Page]" />
                        <asp:HyperLink ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="lnk_last" Text="[Last Page]" />
                    </p>
                    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
			    </div>
		    </div>
		    <div id="tr_exchangerate" runat="server" visible="false">
	            <div class="ektronPageGrid">
                    <asp:Literal ID="ltr_ExchangeRateMsg" runat="server" />
	                <asp:datagrid id="dg_xc"
	                    Runat="server"
	                    Width="100%"
	                    CssClass="ektronGrid"
	                    AutoGenerateColumns="False"
	                    AllowSorting="true">
	                    <HeaderStyle CssClass="title-header" />
		                <Columns>
		                    <asp:TemplateColumn HeaderText="&#160;" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate><asp:CheckBox ToolTip="email" id="chk_email" runat="server" checked="true" /><asp:HiddenField ID="hdn_currencyId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "id")%>' /></ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:boundcolumn DataField="Name" HeaderText="Name"/>
                            <asp:boundcolumn DataField="AlphaIsoCode" HeaderText="AlphaIsoCode"/>
                            <asp:boundcolumn DataField="id" HeaderText="Id"/>
                            <asp:TemplateColumn HeaderText="Exchange Rate">
                                <ItemTemplate>
                                    1 <%=defaultCurrency.ISOCurrencySymbol%> =
                                    <input type="hidden" id='txt_currencyId_<%#DataBinder.Eval(Container.DataItem, "id")%>' name='txt_currencyId_<%#DataBinder.Eval(Container.DataItem, "id")%>' value='<%#DataBinder.Eval(Container.DataItem, "id")%>' />
                                    <asp:textbox Width="160" ID="txt_exchange" runat="server" Text='<%#Util_GetExchangeRate(Convert.ToInt64(DataBinder.Eval(Container.DataItem, "id")))%>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
		                </Columns>
		            </asp:datagrid>
		        </div>
		    </div>
		 </div>
		</form>
	</body>
</html>


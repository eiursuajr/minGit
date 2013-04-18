<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bycustomer.aspx.cs" Inherits="Commerce_bycustomer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reporting By Customer</title>
    
    <script type="text/javascript">
    function resetPostback()
	{
	    document.forms[0].isPostData.value = "";
	}

    function CheckForReturn(e)
	{
	    var keynum;
        var keychar;

        if(window.event) // IE
        {
            keynum = e.keyCode
        }
        else if(e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which
        }

        if( keynum == 13 ) {
            document.getElementById('btnSearch').focus();
        }
	}
	function searchuser()
	{
	    if(document.forms[0].txtSearch.value == "")
	    {
	        alert('<asp:Literal runat="server" id="ltr_searchuserdefault" />');
	        return false;
	    }
	    if(document.forms[0].txtSearch.value.indexOf('\"')!=-1)
	    {
	        alert('<asp:Literal runat="server" id="ltr_searchusertext" />');
	        return false;
	    }
	    var user = document.getElementById('txtSearch');
	    var field = document.getElementById('searchlist');

	    document.forms[0].isSearchPostData.value = "1";
	    document.forms[0].isPostData.value="true";
	    document.forms[0].submit();
	}
	function getcheckedid()
	{
	    var checkedLength = $ektron("table#dg_customers").find("input").length;
	    var i = 0;
	    var j = 0;
	    var sIds = '';
	    for( i = 0; i < checkedLength; i++)
	    {
	        if( $ektron("table#dg_customers").find("input")[i].value == "on" )
	        {
	               sIds += $ektron("table#dg_customers").find("input")[i].nextSibling.parentNode.nextSibling.innerHTML + ',';
	               j = j + 1;
	        }
	    }
	    if( j == 0 )
	    {
	        alert('<asp:Literal runat="server" id="ltr_noCustSelected" />');
	        return false;
	    }
	    parent.location.href = "fulfillment.aspx?action=bycustomer&user="+sIds;
	    parent.ektb_remove();
	}
	function UpdateCheckBox(obj)
	{
	    var checkBox = document.getElementById(obj.id);
	    if( checkBox.value == "on" )
	    {
	        checkBox.value = "off"
	    }
	    else
	    {
	        checkBox.value = "on"
	    }
	}
    </script>
    <style type="text/css">
        .minWidth { width: auto !important;}
        .btnFilter { padding-top: .2em !important; padding-bottom: .2em !important;line-height: 16pt !important;}
        .btnOk { padding-top: .2em !important; padding-bottom: .2em !important; line-height: 16pt !important;}
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div id="divToolBar" runat="server"></div>
        <div class="ektronPageContainer">
            <asp:Panel CssClass="ektronPageGrid" ID="pnl_viewall" runat="Server">
                <asp:DataGrid ID="dg_customers"
                    runat="server"
                    AutoGenerateColumns="false"
                    Width="100%"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:templatecolumn headerstyle-width="1%" headerstyle-cssclass="title-header">
                            <ItemTemplate>
                                <input type="checkbox" onclick="UpdateCheckBox(this);" id="<%#DataBinder.Eval(Container.DataItem, "Id") %>" checked="checked" />
                            </ItemTemplate>
                        </asp:templatecolumn>
                        <asp:BoundColumn DataField="id" HeaderText="Id"/>
                        <asp:BoundColumn  DataField="userName" HeaderText="Name"/>
                    </Columns>
                </asp:DataGrid>
            </asp:Panel>
            <div>
                <table>
                    <tr>
                        <td style="width: 20000px;">
                            <div class="ektronPageGrid">
                                <asp:DataGrid ID="MapCMSUserToADGrid"
                                    runat="server"
                                    AutoGenerateColumns="False"
                                    Width="100%"
                                    GridLines="None"
                                    AllowCustomPaging="True"
                                    PageSize="10"
                                    PagerStyle-Visible="False"
                                    EnableViewState="False">
                                    <HeaderStyle CssClass="title-header" />
                                    <Columns>
                                        <asp:HyperLinkColumn DataTextField="Id" HeaderStyle-CssClass="title-header"
                                            HeaderText="Id" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}">
                                        </asp:HyperLinkColumn>
                                        <asp:HyperLinkColumn DataTextField="DateCreated" HeaderStyle-CssClass="title-header"
                                            HeaderText="Date" DataNavigateUrlField="Id" DataNavigateUrlFormatString="fulfillment.aspx?action=vieworder&id={0}">
                                        </asp:HyperLinkColumn>
                                        <asp:BoundColumn DataField="Status" HeaderStyle-CssClass="title-header" HeaderText="Status"
                                            HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                                        <asp:TemplateColumn HeaderStyle-CssClass="title-header" HeaderText="Order Value"
                                            HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                               <%-- <%#FormatCurrency(DataBinder.Eval(Container.DataItem, "OrderTotal"))%>--%>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                                </asp:DataGrid>
                            </div>
                            <asp:Literal ID="ltr_message" runat="server" />
                        </td>
                    </tr>
                </table>
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
                <input type="hidden" runat="server" id="isDeleted" value="" name="isDeleted" />
                <input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
                <asp:Literal ID="literal1" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>


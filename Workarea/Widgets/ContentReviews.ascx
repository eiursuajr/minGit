<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentReviews.ascx.cs" Inherits="Workarea_Widgets_ContentReviews" %>

<script type="text/javascript">
	function CheckApproveSelect()
	{
	    var _countChecked = 0;
        var err = 1;
        var bret = true;

        var ins = document.getElementsByTagName('input');
        for (i = 0; i < ins.length; i++)
        {
            if (ins[i].getAttribute('type') == 'radio' && ins[i].name.indexOf("cr_") > -1 && ins[i].checked == true) 
            { 
                err = 0; 
            }
        }

        if (err == 1) 
        { 
            bret = false; 
            alert('Please select as least one review to update.'); 
        }
        
        return bret;
	}
</script>

<asp:Label ToolTip="No Records" ID="lblNoRecords" Visible="false" runat="server"><asp:literal ID="ltrlNoRecords" runat="server" /></asp:Label>
<asp:Panel ID="pnlData" runat="server">
    <asp:LinkButton ToolTip="View All" id="lnkViewAll" runat="server"><asp:Literal id="ltrlViewAll" runat="server" /></asp:LinkButton>

    <div class="ektronTopSpace"></div>
    <div class="ektronPageGrid">
        <asp:DataGrid ID="dataGrid" 
            runat="server" 
            Width="100%" 
            AutoGenerateColumns="false"
            EnableViewState="False"
            GridLines="None"
            CssClass="ektronGrid ektronBorder">
            <HeaderStyle CssClass="title-header" />
        </asp:DataGrid>
    </div>

    <div class="ektronTopSpace"></div>
    <asp:Button ToolTip="Approve" ID="btnApprove" runat="Server" Text="Submit" OnClick="btnApprove_Click" Visible="false" />
</asp:Panel>
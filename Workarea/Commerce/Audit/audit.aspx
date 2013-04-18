<%@ Page Language="C#" AutoEventWireup="true" CodeFile="audit.aspx.cs" Inherits="Commerce_Audit_Audit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Audit</title>
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
                function resetPostback() { document.forms[0].isPostData.value = ""; }
            //--><!]]>
        </script>
        <asp:literal id="ltr_js" runat="server"></asp:literal>
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="ektronPageContainer ektronPageInfo">
                <asp:Panel ID="pnl_viewall" runat="Server" CssClass="ektronPageGrid">
                    <asp:literal id="ltr_noEntries" runat="server" Visible="false"></asp:literal>
                    <asp:DataGrid 
                        ID="dg_audit"
                        runat="server"
                        AutoGenerateColumns="false"
                        AllowSorting="true" 
                        CssClass="ektronGrid"
                        HeaderStyle-CssClass="title-header"
                        OnSortCommand="Util_DG_Sort" 
                        SortExpression="datecreated">
                        <Columns>
                            <asp:TemplateColumn HeaderStyle-CssClass="title-header" SortExpression="datecreated"><ItemTemplate><%#Util_ShowLocal(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "DateCreated")))%></ItemTemplate></asp:TemplateColumn>
                            <%--<asp:BoundColumn DataField="Severity" HeaderStyle-CssClass="title-header"></asp:BoundColumn>--%>
                            <asp:BoundColumn DataField="ipaddress" HeaderStyle-CssClass="title-header" SortExpression="ip"></asp:BoundColumn>
                            <asp:BoundColumn DataField="formattedMessage" HeaderStyle-CssClass="title-header" SortExpression="fmessage"></asp:BoundColumn>
                            <asp:BoundColumn DataField="orderid" HeaderStyle-CssClass="title-header" SortExpression="orderid"></asp:BoundColumn>
                            <asp:BoundColumn DataField="userid" HeaderStyle-CssClass="title-header" SortExpression="userid"></asp:BoundColumn>
                        </Columns>
                    </asp:DataGrid>
                    <br />
                    <br />
                    <p class="pageLinks">
                        <asp:Label ToolTip="<%=GetMessage('lbl pagecontrol page')%>" runat="server" ID="PageLabel">Page</asp:Label>
                        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                        <asp:Label ToolTip="<%=GetMessage('lbl pagecontrol of')%>" runat="server" ID="OfLabel">of</asp:Label>
                        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                    </p>
                    <asp:LinkButton ToolTip="<%=GetMessage('lbl first page')%>" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                        OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
                    <asp:LinkButton ToolTip="<%=GetMessage('lbl previous page')%>" runat="server" CssClass="pageLinks" ID="lnkBtnPreviousPage" Text="[Previous Page]"
                        OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
                    <asp:LinkButton ToolTip="<%=GetMessage('lbl next page')%>" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                        OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
                    <asp:LinkButton ToolTip="<%=GetMessage('lbl last page')%>" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                        OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
                    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />   
                </asp:Panel>
            </div>
        </form>
    </body>
</html>


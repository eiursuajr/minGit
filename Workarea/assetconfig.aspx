<%@ Page Language="C#" AutoEventWireup="true" Inherits="AssetManagementConfig" CodeFile="assetconfig.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=m_refMsg.GetMessage("lbl asset management configuration")%>
    </title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
    <meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <asp:literal id="StyleSheetJS" runat="server"></asp:literal>

    <script type="text/javascript">
		<!--//--><![CDATA[//><!--
			var linkvisible=true;
			var mode="<asp:literal id="Mode" runat="Server"/>";

			function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
					var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
					var popupwin = window.open(url, hWind, cToolBar);
					return popupwin;
				}
		//--><!]]>
    </script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
		<div id="dhtmltooltip"></div>
		<div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server" title="Asset Management Configuration"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageGrid">
            <asp:DataGrid ID="AMSGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                CssClass="ektronGrid"
                GridLines="None" OnCancelCommand="AMSGrid_Cancel" OnEditCommand="AMSGrid_Edit" OnUpdateCommand="AMSGrid_Update">
                <EditItemStyle Wrap="False" VerticalAlign="Top"/>
                <HeaderStyle CssClass="title-header"/>
                <Columns>
                    <asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img src='./images/UI/Icons/Save.png' alt='Save changes'&gt;"
                        CancelText="&lt;img src='./images/UI/Icons/cancel.png' alt='Cancel editing'&gt;"
                        EditText="&lt;img src='./images/UI/Icons/contentEdit.png' alt='Edit this item'&gt;">
                        <ItemStyle Wrap="False" VerticalAlign="Top"/>
                    </asp:EditCommandColumn>
                    <asp:BoundColumn DataField="TAG" ReadOnly="True" HeaderText="Tag">
                        <ItemStyle Wrap="False" VerticalAlign="Top"/>
                    </asp:BoundColumn>
                    <asp:TemplateColumn HeaderText="Value">
                        <ItemTemplate>
                            <asp:Label ID="Label1" ToolTip="Value" runat="server" Text='<%# Microsoft.Security.Application.AntiXss.HtmlEncode(DataBinder.Eval(Container, "DataItem.VALUE").ToString()) %>'/>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" ToolTip="Enter Text here" runat="server" Text='<%# HttpContext.Current.Server.HtmlDecode(DataBinder.Eval(Container, "DataItem.VALUE").ToString()) %>'
                                Width="95%"/>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="DESC" ReadOnly="True" HeaderText="Description">
                        <ItemStyle VerticalAlign="Top"/>
                    </asp:BoundColumn>
                </Columns>
                <PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"/>
            </asp:DataGrid>
        </div>
    </form>
</body>
</html>


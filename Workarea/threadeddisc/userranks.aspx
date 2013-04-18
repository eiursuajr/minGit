<%@ Page Language="C#" AutoEventWireup="true" CodeFile="userranks.aspx.cs" Inherits="threadeddisc_userranks" ValidateRequest="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>User Ranks</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <style type="text/css">
       <!--/*--><![CDATA[/*><!--*/
            a.btnUpload { padding-top: .2em !important; padding-bottom: .2em !important;line-height: 16pt !important; display:inline-block; text-decoration: none !important; }
       /*]]>*/-->
    </style>
    <!--[if lte IE 7]>
        <style type="text/css">
            form {position:relative;top:-1px;}
        </style>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <div id="dhtmltooltip"></div>
		<div class="ektronPageContainer">
			<div id="divAE" runat="server">
				<div class="ektronPageInfo">
					<table class="ektronGrid">
						<tr>
							<td class="label" title="Name"><asp:Literal ID="ltr_name" runat="server" />:</td>
							<td><asp:TextBox ToolTip="Enter Name here" ID="txt_name" runat="server" MaxLength="50" /></td>
						</tr>
						<tr>
							<td class="label" title="Rank Image"><asp:Literal ID="ltr_icon_image" runat="server" />:</td>
							<td>
								<asp:TextBox ToolTip="Enter Rank Image Link here" ID="txt_icon_image" runat="server" Text="http://" MaxLength="255" />
								<asp:Literal ID="ltr_upload" runat="server"/>
								<asp:Literal ID="ltr_preview" runat="server" />
							</td>
						</tr>
						<tr>
							<td class="label" title="Number of Posts"><asp:Literal ID="ltr_posts" runat="server" />:</td>
							<td><asp:TextBox ToolTip="Enter Number of Posts here" ID="txt_posts" runat="server" CssClass="ektronTextXXXSmall" MaxLength="50" /></td>
						</tr>
						<tr>
							<td class="label" title="Ladder System"><asp:Literal ID="ltr_isladder" runat="server" />:</td>
							<td><asp:CheckBox ToolTip="Ladder System Option" ID="chk_isladder" runat="server" /></td>
						</tr>
						<tr>
							<td class="label" title="Starting Rank"><asp:Literal ID="ltr_isstart" runat="server" />:</td>
							<td><asp:CheckBox ToolTip="Starting Rank Option" ID="chk_isstart" runat="server" /></td>
						</tr>
						<tr id="tr_applies" runat="server">
							<td class="label" title="Applies To" valign="top"><asp:Literal ID="ltr_appliesto" runat="server" />:</td>
							<td><asp:DropDownList ToolTip="Select what the Ranking applies to from the Drop Down Menu" ID="drp_boards" runat="server" /></td>
						</tr>
					</table>
				</div>
			</div>
			<div id="divViewUser" runat="server" visible="false">
				<div class="ektronPageInfo">
					<table class="ektronForm">
						<tr>
							<td class="label" title="Username"><asp:Literal ID="ltr_vu_username" runat="server" />:</td>
							<td><asp:Literal ID="ltr_vud_username" runat="server" /></td>
						</tr>
						<tr>
							<td class="label" title="Display Name"><asp:Literal ID="ltr_vu_displayname" runat="server" />:</td>
							<td><asp:Literal ID="ltr_vud_displayname" runat="server" /></td>
						</tr>
						<tr>
							<td class="label" title="First Name"><asp:Literal ID="ltr_vu_firstname" runat="server" />:</td>
							<td><asp:Literal ID="ltr_vud_firstname" runat="server" /></td>
						</tr>
						<tr>
							<td class="label" title="Last Name"><asp:Literal ID="ltr_vu_lastname" runat="server" />:</td>
							<td><asp:Literal ID="ltr_vud_lastname" runat="server" /></td>
						</tr>
					</table>
				</div>
			</div>
			<asp:Panel ID="pnlList" runat="server">
				<div class="ektronHeader"><asp:Literal ID="ltr_vu_select" runat="server" /></div>
				<asp:Panel ID="pnlGrid" CssClass="ektronPageGrid" runat="server">
					<asp:datagrid ID="dgUserRank"
						runat="server"
						AutoGenerateColumns="false"
						Width="100%"
						CssClass="ektronGrid"
						GridLines="None">
						<HeaderStyle CssClass="title-header" />
					</asp:datagrid>
				</asp:Panel>
			</asp:Panel>
			<asp:literal ID="ltr_init_js" runat="server" />
		</div>
    </form>
</body>
</html>


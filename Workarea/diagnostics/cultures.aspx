<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cultures.aspx.cs" Inherits="Workarea_diagnostics_cultures" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cultures</title>
    <style type="text/css">
    p, div, td, th
    {
		font-family: Arial, Sans-Serif;
		padding-left: 1em;
		padding-right: 1em;
    }
    
    caption
    {
		padding: 1em;
		font-size: large;
		font-weight: bold;
    }
    
    .LocaleHeader
    {
		background-color: Navy;
		text-align: left;
		color: White;
    }
    
    .LocaleHeader a
	{
		color: White;
		text-decoration: none;
	}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:GridView ID="gvCultures" AutoGenerateColumns="false" AllowSorting="true" runat="server"
		 BorderStyle="None" GridLines="None" Caption="Cultures" HeaderStyle-CssClass="LocaleHeader" AlternatingRowStyle-BackColor="Lavender" >
			<Columns>
				<asp:BoundField DataField="EnglishName" SortExpression="EnglishName" HeaderText="English Name" />
				<asp:BoundField DataField="NativeName" HeaderText="Native Name" />
				<asp:BoundField DataField="LanguageTag" SortExpression="LanguageTag" HeaderText="Lang Tag" />
				<asp:BoundField DataField="CultureTag" SortExpression="CultureTag" HeaderText="Culture" />
				<asp:BoundField DataField="LCID" SortExpression="LCID" HeaderText="LCID" />
				<asp:BoundField DataField="LCID" SortExpression="LCID" DataFormatString="{0:x04}" HeaderText="Hex" />
				<%--<asp:BoundField DataField="ResourceText" HeaderText="'Name'" />--%>
			</Columns>
		</asp:GridView>
    </div>
    </form>
</body>
</html>

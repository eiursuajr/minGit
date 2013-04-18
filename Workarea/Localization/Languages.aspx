<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Languages.aspx.cs" Inherits="Workarea_Languages" %>
<%@ Register TagPrefix="loc" TagName="LocaleDetail" Src="LocaleDetail.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <title>Languages and Regions</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    
    <style type="text/css">
    .ektronGrid td 
    {
		padding-left: 3px;
		padding-right: 3px;
    }
    .ektronGrid td input
    {
		width: auto;
    }
    .ektronGrid td.right input
    {
		text-align: right;
    }
    .ektronGrid td img
    {
		vertical-align: bottom;
    }
    .ektronGrid td.commandField input.commandButton
    {
		margin-right: 1em;
    }
    img.regionMap
    {
		display: block; 
		border: solid 1px #1D5987 !important; 
		padding: 2px; 
		background-color: white;
    }
    img.locatorMap
    {
		float: left;
		margin: 0 4px 4px 0;
		border: solid 1px #1D5987 !important; 
		padding: 2px; 
		background-color: white;
    }
    </style>

	<script language="javascript" type="text/javascript" src="../java/jfunct.js"></script>
    <script type="text/javascript">
    <!--//--><![CDATA[//><!--
	function SubmitForm(formName, action)
	{
		if ("save_detail" == action)
		{
			document.forms[formName].action="languages.aspx?action=edit_grid";
			$ektron("#LocaleDetail_Save").click();
			return;
		}
		else if ("update_detail" == action)
		{
			document.forms[formName].action="languages.aspx?action=edit_grid";
			$ektron("#LocaleDetail_Update").click();
			return;
		}
		
		if ("undefined" == typeof formName) formName = 0;
		for (var i = 0; i < document.forms[formName].elements.length; i++)
		{
			var oElem = document.forms[formName].elements[i];
			if (oElem && oElem.name.indexOf("site") != -1)
			{
				if (oElem.checked)
				{
					document.forms[formName].action="languages.aspx";
					__doPostBack("action", "update");
					return;
				}
			}
		}
		alert("At least one language must be enabled for the web site.");
		return false;
	}
	
	function onSiteClick(objThis, idActive)
    {
		var objActive = document.getElementById(idActive);
		if (objActive)
		{
			if (objThis.checked)
			{
				objActive.disabled = true;
				objActive.checked = true;
			}
			else if (!$ektron(objActive).parent().hasClass("fallbackLocale"))
			{
			    $ektron(objActive).parent().removeAttr("disabled");
			    objActive.disabled = false;
			}
		}
    }
	//--><!]]>
    </script>
    <script language="javascript" type="text/javascript" src="../java/toolbar_roll.js"></script>
    <asp:Literal ID="ltrStyleSheet" runat="server" />
</head>
<body>
    <form id="frmLanguage" name="frmLanguage" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageGrid">
		    <asp:GridView ID="LanguageGrid" runat="server" EnableViewState="true" AutoGenerateColumns="false" 
		        ShowHeader="true" AllowSorting="true" AllowPaging="false"
		        Width="100%" CssClass="ektronGrid" GridLines="None">
                <HeaderStyle CssClass="title-header" />
				<Columns>
					<asp:CheckBoxField DataField="SiteEnabled"
						HeaderImageUrl="../UI/Icons/check.png" HeaderText="reskey:alt available on web site">
						<HeaderStyle Width="16px" HorizontalAlign="Center" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="center" />
					</asp:CheckBoxField>
					<asp:CheckBoxField DataField="Enabled"
						HeaderImageUrl="../UI/Icons/caution.png" HeaderText="reskey:alt available in workarea only">
						<HeaderStyle Width="16px" HorizontalAlign="Center" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="center" />
					</asp:CheckBoxField>
					<asp:ImageField DataImageUrlField="LanguageState" SortExpression="LanguageState" HeaderText="reskey:enabled">
						<HeaderStyle Width="16px" HorizontalAlign="Center" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="center" />
					</asp:ImageField>
					<asp:ImageField DataImageUrlField="FlagUrl" DataAlternateTextField="EnglishName">
						<HeaderStyle Width="16px" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="center" />
					</asp:ImageField>
					<asp:BoundField DataField="CombinedName" SortExpression="EnglishName" HtmlEncode="false" HeaderText="reskey:lbl name">
						<HeaderStyle />
						<ItemStyle Wrap="false" />
					</asp:BoundField>
					<asp:BoundField DataField="Loc" SortExpression="Loc" HeaderText="reskey:lbl loc">
						<HeaderStyle Width="6em" />
						<ItemStyle Width="6em" Wrap="false" />
					</asp:BoundField>
					<asp:TemplateField HeaderText="reskey:lbl fallbackloc" SortExpression="FallbackId">
						<ItemTemplate>
							<%# GetFallbackLoc(Container.DataItem)%>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="XmlLang" SortExpression="XmlLang" HeaderText="reskey:lbl lang">
						<HeaderStyle Width="6em" />
						<ItemStyle Width="6em" Wrap="false" />
					</asp:BoundField>
					<asp:BoundField DataField="Id" SortExpression="Id" HeaderText="reskey:generic id">
						<HeaderStyle Width="8em" CssClass="right" />
						<ItemStyle Width="8em" Wrap="false" CssClass="right" />
					</asp:BoundField>
				</Columns>
		    </asp:GridView>
		    <asp:GridView ID="EditableGrid" runat="server" EnableViewState="true" AutoGenerateColumns="false" 
		        ShowHeader="true" AllowSorting="true" AllowPaging="false"
		        Width="100%" CssClass="ektronGrid" GridLines="None">
                <HeaderStyle CssClass="title-header" />
				<Columns>
					<asp:ButtonField CommandName="EditDetail" ButtonType="Image" Text="Edit" />
					<asp:CommandField ShowEditButton="True" ButtonType="Image" 
						EditText="Rename or change fallback locale" UpdateText="Save" CancelText="Cancel"
						ItemStyle-CssClass="commandField" ControlStyle-CssClass="commandButton" />
					<asp:ImageField DataImageUrlField="LanguageState" SortExpression="LanguageState" HeaderText="reskey:enabled" ReadOnly="true">
						<HeaderStyle Width="16px" HorizontalAlign="Center" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="center" />
					</asp:ImageField>
					<asp:ImageField DataImageUrlField="FlagUrl" DataAlternateTextField="EnglishName" ReadOnly="true">
						<HeaderStyle Width="16px" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" CssClass="center" />
					</asp:ImageField>
					<asp:BoundField DataField="EnglishName" SortExpression="EnglishName" HeaderText="reskey:lbl english name">
						<HeaderStyle />
						<ItemStyle Wrap="false" />
					</asp:BoundField>
					<asp:BoundField DataField="NativeName" HeaderText="reskey:lbl native name">
						<HeaderStyle />
						<ItemStyle Wrap="false" />
					</asp:BoundField>
					<asp:BoundField DataField="Loc" SortExpression="Loc" HeaderText="reskey:lbl loc" ReadOnly="true">
						<HeaderStyle Width="6em" />
						<ItemStyle Width="6em" Wrap="false" />
					</asp:BoundField>
					<asp:TemplateField HeaderText="reskey:lbl fallbackloc" SortExpression="FallbackId">
						<EditItemTemplate>
							<asp:DropDownList ID="lstFallbackLoc" runat="server" />
						</EditItemTemplate>
						<ItemTemplate>
							<%# GetFallbackLoc(Container.DataItem) %>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField DataField="XmlLang" SortExpression="XmlLang" HeaderText="reskey:lbl lang" ReadOnly="true">
						<HeaderStyle Width="6em" />
						<ItemStyle Width="6em" Wrap="false" />
					</asp:BoundField>
					<asp:BoundField DataField="Id" SortExpression="Id" HeaderText="reskey:generic id" ReadOnly="true">
						<HeaderStyle Width="8em" CssClass="right" />
						<ItemStyle Width="8em" Wrap="false" CssClass="right" />
					</asp:BoundField>
				</Columns>
		    </asp:GridView>
			<loc:LocaleDetail ID="LocaleDetail" runat="server" Visible="false" />
        </div>
   </form>
</body>
</html>

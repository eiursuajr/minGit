<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mappings.aspx.cs" Inherits="Workarea_search_mappings" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= MessageHelper.GetMessage("lbl integrated search mappings") %></title>
    <asp:Literal ID="styleHelper" runat="server"></asp:Literal>
    <style type="text/css">
        td.startAddress { width:400px; }
        td.virtualDirectory { width:210px; }
        td.addButton { vertical-align: middle; }
        select.startAddress { width:350px; }
        input.virtualDirectory { width:300px; }                
        div.error { color: #D8000C; width: auto; background-color: #FFBABA; background-image: url('../images/UI/Icons/exclamation.png'); background-position: 4px 4px; border: 1px solid; margin: 10px 0px; padding:10px 10px 15px 25px; background-repeat: no-repeat; }
        div.description { margin-bottom: 10px; padding-left: 2px; }
        div.warningMessage { color: Maroon !important; }
        div.helpText { font-size: 10px; color: #555555; margin: 0px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
	<div class="ektronPageContainer">
        <div>
            <div id="dhtmltooltip"></div>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
                <div class="ektronToolbar" id="htmToolBar" runat="server">
                    <table>
                        <tr>
						    <td id="tdIntegratedSearchHelpButton" runat="server"></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div>
                <div class="ektronPageInfo">
                    <div id="divErrorMessage" class="error" runat="server" visible="false">
                        <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
                    </div>
                    <div class="description">
                        <%= MessageHelper.GetMessage("integrated search mappings description") %>
                    </div>                    
                    <table class="ektronGrid">
                        <tr class="title-header">
                            <th><%= MessageHelper.GetMessage("integrated search mappings start address header") %></th>
                            <th><%= MessageHelper.GetMessage("integrated search mappings mapping header") %></th>
                            <th></th>
                        </tr>
                        <tr>                            
                            <td class="startAddress">
                                <asp:DropDownList ID="ddlStartAddresses" runat="server" CssClass="startAddress"></asp:DropDownList>
                                <div id="divStartAddressWarning" runat="server" class="helpText warningMessage"><%= MessageHelper.GetMessage("integrated search mappings no start addresses") %></div>    
                            </td>
                            <td class="virtualDirectory">
                                <asp:TextBox ID="txtMapping" runat="server" CssClass="virtualDirectory"></asp:TextBox>
                                <div class="helpText"><%= MessageHelper.GetMessage("integrated search mappings example") %></div>
                            </td>
                            <td><asp:ImageButton ID="btnAdd" runat="server" ImageUrl="../images/UI/Icons/add.png" onclick="btnAdd_Click" /></td>
                        </tr>
                        <asp:Repeater ID="rptMappings" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="startAddress"><%# HttpUtility.HtmlEncode(Eval("StartAddress")) %></td>
                                    <td class="virtualDirectory"><%# HttpUtility.HtmlEncode(Eval("VirtualDirectory")) %></td>
                                    <td class="addButton"><asp:ImageButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("Id") %>' ImageUrl="../images/UI/Icons/delete.png" OnCommand="btnRemove_Command" /></td>                                    
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
            </div>
        </div>
		</div>
    </form>
</body>
</html>

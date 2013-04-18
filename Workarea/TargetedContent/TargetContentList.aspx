<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TargetContentList.aspx.cs" Inherits="Workarea_TargetedContent_TargetContentList" %>
<%@ Register Src="../controls/paging/paging.ascx" TagPrefix="ek" TagName="paging" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <asp:Literal id="ltrlStyleSheetJS" runat="server" />
    <style>
        table.ektronGrid {display:block;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer">
            <table class="ektronGrid">
                <tr class="title-header">
                    <th style="width:1%; white-space:nowrap;">&#160;</th>
                    <th style="width:1%; white-space:nowrap"><%=_msgHelper.GetMessage("generic id")%></th>
                    <th><%=_msgHelper.GetMessage("generic title") %></th>
                </tr>
                <asp:Repeater ID="ViewAllRepeater" runat="server">
                    <ItemTemplate>
                        <tr class="row">
                            <td>
                                <asp:HyperLink ID="editLink" runat="server" NavigateUrl='<%# Edit_TargetContent_Url + "&targetcontentid=" + DataBinder.Eval(Container.DataItem, "Id")%>'> 
                                    <img src="../images/UI/Icons/contentEdit.png" alt="Edit" />
                                </asp:HyperLink>
                            </td>
                            <td><%# Eval("Id")%></td>
                            <td><%# Eval("Name")%></td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="evenrow stripe">
                            <td>
                                <asp:HyperLink ID="editLink" runat="server" NavigateUrl='<%# Edit_TargetContent_Url + "&targetcontentid=" + DataBinder.Eval(Container.DataItem, "Id")%>'> 
                                    <img src="../images/UI/Icons/contentEdit.png" alt="Edit" />
                                </asp:HyperLink>
                            </td>
                            <td><%# Eval("Id")%></td>
                            <td><%#  Eval("Name")%></td>
                        </tr>
                    </AlternatingItemTemplate>
                </asp:Repeater>
            </table>
            <ek:paging ID="ucPaging" CurrentPageIndex="0" runat="server" />
        </div>
    </form>
</body>
</html>

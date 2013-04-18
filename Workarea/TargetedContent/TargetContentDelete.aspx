<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TargetContentDelete.aspx.cs" Inherits="Workarea_TargetedContent_TargetContentDelete" %>
<%@ Register Src="../controls/paging/paging.ascx" TagPrefix="ek" TagName="paging" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<script type="text/javascript">
    function validateList(message) {
        var list = $ektron("input:checkbox");
        for (var i = 0; i < list.length; i++) {
            if ($ektron(list[i]).attr('checked')) return true;
        }
        alert(message);
        return false;
    }
    function ShowTransString(Text) {
        var ObjId = "ektronTitlebar";
        var ObjShow = document.getElementById('_' + ObjId);
        var ObjHide = document.getElementById(ObjId);
        if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
            ObjShow.innerHTML = Text;
            ObjShow.style.display = "inline";
            if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
                ObjHide.style.display = "none";
            }
        }
    }

    function HideTransString() {
        var ObjId = "ektronTitlebar";
        var ObjShow = document.getElementById(ObjId);
        var ObjHide = document.getElementById('_' + ObjId);
        if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
            ObjShow.style.display = "inline";
            if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
                ObjHide.style.display = "none";
            }
        }
    }
</script>     
    <asp:Literal id="ltrlStyleSheetJS" runat="server" />
    <style>
        table.ektronGrid {display:block;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
       <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server">
                <span id="WorkareaTitlebar" style="display: inline;" runat="server"></span>
                <span id="_WorkareaTitlebar" style="display: none;"></span>
            </div>
            <div class="ektronToolbar" id="htmToolBar" runat="server">
                <table>
                <tbody>
                    <tr>
						<td class="button" id="image_cell_101" runat="server">
                            <asp:HyperLink CssClass="primary backButton" NavigateUrl="TargetContentList.aspx" ID="image_link_101" style="cursor:default;"  onmouseout="HideTransString();RollOut(this);" runat="server">
                                <asp:Literal ID="BackLabel" runat="server" />
                            </asp:HyperLink>
                        </td>
                        <td class="button" title="Click here to delete target content" id="image_cell_100" runat="server">
                            <asp:LinkButton CssClass="primary deleteButton" ID="image_link_100" OnClick="ucDeleteButton_click"  style="cursor:default;"  onmouseout="HideTransString();RollOut(this);"  runat="server">
                                <asp:Literal ID="DeleteLabel" runat="server" />
                            </asp:LinkButton>
                        </td>
                        <td>
                          <asp:Literal ID="uxHelpbutton" runat="server" />
                        </td>
                    </tr>
                </tbody>
            </table>
            </div>
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
                            <td><input type="checkbox" name ='chkTargetContent_<%# Eval("Id")%>' id='chkTargetContent_<%# Eval("Id")%>' /></td>
                            <td><%# Eval("Id")%></td>
                            <td><%# Eval("Name")%></td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="evenrow stripe">
                           <td><input type="checkbox" name ='chkTargetContent_<%# Eval("Id")%>' id='chkTargetContent_<%# Eval("Id")%>' /></td>
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

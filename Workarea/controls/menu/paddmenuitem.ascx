<%@ Control Language="C#" AutoEventWireup="true" CodeFile="paddmenuitem.ascx.cs"
    Inherits="Workarea_controls_menu_pAddMenuItem" %>
<div id="divLibrary" runat="server" visible="false">
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" title="Add New Item">
            <asp:Literal ID="litTitle" runat="server"></asp:Literal>
        </div>
        <div class="ektronToolbar">
            <table>
                <tr>
                    <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                    <asp:Literal ID="litHelp" runat="server"></asp:Literal>
                </tr>
            </table>
        </div>
    </div>
    <form name="AddMenuItem" action="collections.aspx?action=doAddMenuItem&type=library&LangType=<%=ContentLanguage%>&iframe=<%=Request.QueryString["iframe"]%><%=noWorkAreaString %>"
    method="post">
    <div class="ektronPageContainer ektronPageInfo">
        <div class="heightFix">
            <table class="ektronGrid">
                <tr>
                    <td class="label" title="Title">
                        <%=MsgHelper.GetMessage("generic title label")%>
                    </td>
                    <td class="value">
                        <input type="text" title="Enter Title here" name="title" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <input type="button" name="Browse Library" value="<%=(MsgHelper.GetMessage("Browse Library Button"))%>"
                            onclick="PopBrowseWin('images,hLink,files', '<%=FolderPath%>', null, '<%=enableQDOparam%>');return false;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <input type="hidden" name="FolderID" value="<%=(FolderId)%>" />
    <input type="hidden" name="CollectionID" value="<%=(MenuId)%>" />
    <input type="hidden" name="DefaultTitle" value="" />
    <input type="hidden" name="id" value="" />
    <input type="hidden" name="frm_back" value="<%=(Request.QueryString["back"])%>" />
    </form>
</div>
<div id="divOther" runat="server" visible="false">
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="Add New Item">
        <asp:Literal ID="litTitle1" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons1" runat="server"></asp:Literal>
                <td>
                    <asp:Literal ID="litHelp1" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
    </div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <form name="AddMenuItem" action="collections.aspx?action=doAddMenuItem&type=link&langType=<%=ContentLanguage%>&iframe=<%=Request.QueryString["iframe"]%><%=noWorkAreaString %>"
    method="post">
    <input type="hidden" name="FolderID" value="<%=(FolderId)%>" />
    <input type="hidden" name="CollectionID" value="<%=(MenuId)%>" />
    <input type="hidden" name="frm_back" value="<%=(Request.QueryString["back"])%>" />
    <table width="100%">
        <tr>
            <td class="info" nowrap title="Title">
                <%=MsgHelper.GetMessage("generic title label")%>
            </td>
            <td>
                <input type="text" title="Enter Title here" name="Title" value="" />
            </td>
        </tr>
        <tr>
            <td class="info" nowrap valign="top" title="Link">
                <%=MsgHelper.GetMessage("lbl link")%>:
            </td>
            <td>
                <input type="text" title="Enter Link here" name="Link" value="" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td class="info">
                <br />
                <%=MsgHelper.GetMessage("lbl Examples")%>:
                <br />
                <%=MsgHelper.GetMessage("lbl external link")%>: http://www.ektron.com
                <br />
                <%=MsgHelper.GetMessage("lbl Root of the web site")%>: /news/pr.aspx
                <br />
                <%=MsgHelper.GetMessage("generic relative")%>: pr.aspx
            </td>
        </tr>
    </table>
    </form>
</div>
</div>
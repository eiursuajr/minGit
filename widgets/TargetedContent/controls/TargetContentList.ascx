<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TargetContentList.ascx.cs" Inherits="widgets_TargetedContent_controls_TargetContentList" %>
<%@ Register Src="../../../workarea/controls/paging/clientpaging/clientpaging.ascx" TagName="paging" TagPrefix="ek" %>

<div class="ektronTargetContentList">
    <script>
       
    </script>
    <input type="hidden" ID="hdnTargetContentSelectId" name="hdnTargetContentSelectId" class="hdnTargetContentSelectId" runat="server"  />
    <input type="hidden" ID="hdnTargetContentControlSource" name="hdnTargetContentControlSource" class="hdnTargetContentControlSource" runat="server"  />
    <input type="hidden" ID="hdnTargetContentListUrl" name="hdnTargetContentListUrl" class="hdnTargetContentListUrl" runat="server"  />
    
    <div class="targetContentListModal" title="Select Target Configuration">
        <table class="ektronGrid">
            <tr class="title-header">
                <th style="width:1%; white-space:nowrap"><%=_msgHelper.GetMessage("generic id")%></th>
                <th><%=_msgHelper.GetMessage("generic title") %></th>
            </tr>
            <asp:Repeater ID="ViewAllRepeater" runat="server">
                <ItemTemplate>
                    <tr class="row">
                        <td>
                            <asp:LinkButton ID="selectLink" runat="server" OnClientClick='<%# "Ektron.Widget.TargetedContentList.select(" + Eval("Id") + ");" %>' OnCommand="selectLink_Click" CommandName="selectLink" CommandArgument='<%# Eval("Id")%>'>
                                <%# Eval("Id")%>
                            </asp:LinkButton>
                        </td>
                        <td>
                            <asp:LinkButton ID="selectLinkName" runat="server" OnClientClick='<%# "Ektron.Widget.TargetedContentList.select(" + Eval("Id") + ");" %>' OnCommand="selectLink_Click" CommandName="selectLink" CommandArgument='<%# Eval("Id")%>'>
                                <%# Eval("Name")%>
                            </asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="evenrow stripe">
                         <td>
                            <asp:LinkButton ID="selectLink" runat="server" OnClientClick='<%# "Ektron.Widget.TargetedContentList.select(" + Eval("Id") + ");" %>' OnCommand="selectLink_Click" CommandName="selectLink" CommandArgument='<%# Eval("Id")%>'>
                                <%# Eval("Id")%>
                            </asp:LinkButton>
                        </td>
                        <td>
                            <asp:LinkButton ID="selectLinkName" runat="server" OnClientClick='<%# "Ektron.Widget.TargetedContentList.select(" + Eval("Id") + ");" %>'  OnCommand="selectLink_Click" CommandName="selectLink" CommandArgument='<%# Eval("Id")%>'>
                                <%# Eval("Name")%>
                            </asp:LinkButton>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </table>
        <ek:paging ID="ucPaging" SelectedPage="1" CurrentPageIndex="0"  runat="server" />
    </div>
</div>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Finish.ascx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.Finish" %>
<div class="finish ektronPageHeader">
    <table class="ektronGrid">
        <thead>
            <tr class="title-header">
                <th style="border-right:none;"><asp:Literal ID="litFinishHeader" runat="server" /></th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td style="border-bottom:none;border-right:none;">
                    <div class="innerWrapper">
                        <p><span><asp:Hyperlink ID="aProperties" runat="server" /><asp:Literal ID="litFinishMessage" runat="server" /></span></p>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
</div>
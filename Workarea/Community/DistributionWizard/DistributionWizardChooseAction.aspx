<%@ Page Language="C#" MasterPageFile="DistributionWizard.master" AutoEventWireup="true" CodeFile="DistributionWizardChooseAction.aspx.cs" Inherits="Community_DistributionWizard_DistributionWizardChooseAction" Title="Distribution Wizard" %>
<%@ MasterType VirtualPath="DistributionWizard.master" %>
<asp:Content ID="contentChooseAction" ContentPlaceHolderID="cphDistributionWizardContent" Runat="Server">
    <div id="divDistributionOptions" runat="server">
        <asp:LinkButton ToolTip="Copy" ID="lbCopy" runat="server" CssClass="DistributionWizardChooseButton" OnClick="lbCopy_Click">
            I want to distribute a copy of this item.<br /><br />
            I will choose metadata and taxonomy for this item.
        </asp:LinkButton>
        <div class="DistributionWizardCenteredText"><strong>Or</strong></div>
        <asp:LinkButton ToolTip="Replace" ID="lbReplace" runat="server" CssClass="DistributionWizardChooseButton" OnClick="lbReplace_Click">
            I want to replace an existing item with a copy of this one.<br /><br />
            My replacement will inherit all of the metadata and taxonomy of the item I replace.
        </asp:LinkButton>
    </div>
    <div id="divRedistributionOptions" runat="server">
        <div class="RedistributionDetails" id="divRedistritbutionDetails" runat="server">
            <asp:Literal ID="ltrRedistributionDetails" runat="server"></asp:Literal>
        </div>
        <asp:LinkButton ToolTip="Redistribute" ID="lbRedistribute" runat="server" CssClass="DistributionWizardChooseButton" OnClick="lbRedistribute_Click">
            I want to redistribute the latest version of this item to the site.<br /><br />
            It will replace the current version of the document mentioned above.
        </asp:LinkButton>
    </div>
    <div id="DistributionWizardFooter"><asp:Button ToolTip="Close" ID="btnClose" Text="Cancel" runat="server" OnClick="btnClose_Click" /></div>
</asp:Content>


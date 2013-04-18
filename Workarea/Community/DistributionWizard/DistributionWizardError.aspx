<%@ Page Language="C#" MasterPageFile="DistributionWizard.master" AutoEventWireup="true" CodeFile="DistributionWizardError.aspx.cs" Inherits="Community_DistributionWizard_DistributionWizardError" Title="Distribution Wizard" %>
<%@ MasterType VirtualPath="DistributionWizard.master" %>

<asp:Content ID="contentDistributionError" ContentPlaceHolderID="cphDistributionWizardContent" Runat="Server">
    <asp:Label ToolTip="The parameters necessary to distribute this content are missing or invalid. Check permissions. " ID="lblErrorMessage" runat="server"></asp:Label>
    <div id="DistributionWizardFooter"><asp:Button ToolTip="Close" ID="btnClose" Text="Close" runat="server" OnClick="btnClose_Click" /></div>
</asp:Content>
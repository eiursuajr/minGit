<%@ Control Language="C#" AutoEventWireup="true" CodeFile="taxonomytree.ascx.cs" Inherits="Flash_pagebuilder_taxtree" %>
<asp:Literal ID="css" runat="server"></asp:Literal>
<asp:Label ID="noTaxonomies" runat="server" />
<asp:Repeater ID="taxonomies" runat="server">
    <ItemTemplate>
        <div class="treecontainer">
        <span class="folder" data-ektron-taxid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>"><%#DataBinder.Eval(Container.DataItem, "TaxonomyName")%></span>
        <ul class="EktronTaxonomyTree EktronTreeview-gray" data-ektron-taxid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>"></ul>
        </div>
    </ItemTemplate>
</asp:Repeater>
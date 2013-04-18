<%@ Control Language="C#" AutoEventWireup="true" CodeFile="taxonomytree.ascx.cs" Inherits="Workarea_pagebuilder_CBTaxTree" %>
<asp:Literal ID="css" runat="server"></asp:Literal>
<asp:Label ID="noTaxonomies" runat="server" />
<div class="treecontainer">
    <ul class="EktronTaxonomyTree EktronTreeview-gray">
    <asp:Repeater ID="taxonomies" runat="server">
        <ItemTemplate>
            <li class="closed">
                <span class="folder" data-ektron-taxid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>">
                    <%#DataBinder.Eval(Container.DataItem, "TaxonomyName")%>
                </span>
                <ul data-ektron-taxid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>"></ul>
            </li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</div>

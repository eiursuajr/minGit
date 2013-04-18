<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Taxonomy.Tree.A.aspx.cs" Inherits="Taxonomy_Tree_A_aspx" %>
   
    var taxonomytreearr="<asp:Literal ID="litTaxonomyTreeIdList" runat="server" />".split(",");   
    var taxonomytreedisablearr="<asp:Literal ID="litTaxonomyTreeParentIdList" runat="server" />".split(",");     
    var __TaxonomyOverrideId="<asp:Literal ID="litTaxonomyOverrideId" runat="server" />".split(",");       
    var m_fullScreenView=false;
    var __EkFolderId = <asp:Literal ID="litFolderId" runat="server" />;
    
   
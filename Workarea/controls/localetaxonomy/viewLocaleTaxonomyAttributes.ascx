<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewLocaleTaxonomyAttributes.ascx.cs" 
  Inherits="viewLocaleTaxonomyAttributes" %>
  
  <div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div id="searchpanel" class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
        <tr>
            <td class="label" title="Breadcrumb"><%=m_refMsg.GetMessage("lbl sitemap path")%>:</td>
            <td class="readOnlyValue"><%=m_strCurrentBreadcrumb%></td>
        </tr>
        <tr>
            <td class="label" title="ID"><%=m_refMsg.GetMessage("generic id")%>:</td>
            <td class="readOnlyValue"><asp:Label ID="lbltaxonomyid" runat="server" /></td>
        </tr>
        <tr>
            <td class="label" title="Title"><%=m_refMsg.GetMessage("lbl title")%>:</td>
            <td class="readOnlyValue"><asp:Label ID="taxonomytitle" runat="server" /></td>
        </tr>
        <tr>
            <td class="label" title="Description"><%=m_refMsg.GetMessage("lbl description")%>:</td>
            <td class="readOnlyValue"><asp:Label ID="taxonomydescription" runat="server" /></td>
        </tr>
        <tr>
            <td class="label" title="Image"><%=m_refMsg.GetMessage("generic image")%>:</td>
            <td class="readOnlyValue">
                <span id="sitepath"><asp:Literal ID="ltr_sitepath" runat="Server" /></span><asp:literal id="taxonomy_image" runat="server" />
                <div class="ektronTopSpace"></div>
                <asp:Image ID="taxonomy_image_thumb" runat="server" />
            </td>
        </tr>
        <tr id="tr_tmpl" runat="server">
            <td class="label" title="Template"><%=m_refMsg.GetMessage("template label")%>:</td>
            <td class="readOnlyValue"><asp:Label ID="lblTemplate" runat="server" /></td>
        </tr>
        <tr id="tr_tmplinhrt" runat="server">
            <td class="label" title="Inherit Template"><%=m_refMsg.GetMessage("lbl inherit template")%>:</td>
            <td class="readOnlyValue"><asp:Label ID="lblTemplateInherit" runat="server" /></td>
        </tr>
        <tr id="tr_config" runat="server">
            <td class="label" title="Configuration"><%=m_refMsg.GetMessage("config page html title")%>:</td>
            <td class="readOnlyValue"><asp:Label ID="configlist" runat="server" /></td>
        </tr>
        <tr>
            <td id="tr_catLink" class="label" title="Category Link"><%=m_refMsg.GetMessage("lbl category link")%>:</td>
            <td class="readOnlyValue"><asp:Label ID="catLink" runat="server" /></td>
        </tr>
        <tr>
           <td id="tr_enDis" class="label" title="Display to Users"><%=m_refMsg.GetMessage("lbl enable/disable")%>:</td>
           <td class="readOnlyValue"><asp:Literal ID="ltrStatus" runat="server" /></td>            
        </tr>
    </table>
</div>

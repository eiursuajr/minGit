<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Default.ascx.cs" Inherits="Controls_Default" %>
<div class="sideNav">
    <a title="Import Dictionary" href="<%=Page.ResolveUrl(ConfigRoot)%>?Page=Import">Import a new dictionary.</a>
</div>
<div class="sideNav">
    <a title="Edit Dictionary" href="<%= Page.ResolveUrl(ConfigRoot)%>?Page=EditDictionary">Edit dictionary.</a>
</div>

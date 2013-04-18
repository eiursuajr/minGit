<?xml version="1.0" encoding="utf-8" ?>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>

<%@ Page ContentType="text/xml" Language="C#" AutoEventWireup="true" Inherits="cmsdatalist"
    CodeFile="cmsdatalist.aspx.cs" %>

<asp:literal id="CmsDataListXml" runat="server"></asp:literal>
<cms:ContentBlock ID="CmsDataList" runat="server" DynamicParameter="id"></cms:ContentBlock>

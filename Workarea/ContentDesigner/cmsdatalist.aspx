<?xml version="1.0" encoding="utf-8"?>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<%@ Page ContentType="text/xml" Language="C#" AutoEventWireup="true" Inherits="CDcmsdatalist" CodeFile="cmsdatalist.aspx.cs" %>
<asp:Literal id="CmsDataListXml" runat="server"></asp:Literal>
<cms:ContentBlock id="CmsDataList" runat="server" DynamicParameter="id"></cms:ContentBlock>
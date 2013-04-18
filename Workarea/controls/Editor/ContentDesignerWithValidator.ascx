<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentDesignerWithValidator.ascx.cs" Inherits="Ektron.ContentDesignerWithValidator" %>
<%@ Register TagPrefix="ektron" Assembly="Ektron.ContentDesigner" Namespace="Ektron.ContentDesigner" %>
<ektron:ContentDesigner ID="ContentDesigner" runat="server" />
<asp:RegularExpressionValidator ID="ContentValidator" ControlToValidate="ContentDesigner" EnableClientScript="true" runat="server" />


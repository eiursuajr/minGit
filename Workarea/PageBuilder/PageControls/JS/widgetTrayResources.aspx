<%@ Page Language="C#" AutoEventWireup="true" CodeFile="widgetTrayResources.aspx.cs" Inherits="Workarea_PageBuilder_PageControls_JS_widgetTrayResources" %>
// the following JavaScript defines the resource text values
// used by the page Builder Wizard scripts on this page

// establish Resource Text object for JS string references
if ("undefined" === typeof Ektron.ResourceText)
{
    Ektron.ResourceText = {};
}

// ensure that the PageBuilder object exists
if ("undefined" === typeof Ektron.ResourceText.PageBuilder)
{
    Ektron.ResourceText.PageBuilder = {};
}
// define resource text strings as properties
Ektron.ResourceText.PageBuilder.WidgetTray =
{
    cancel: "<asp:literal id="jsCancel" runat="server" />",
    dropControlHere: "<asp:literal id="jsDropControlHere" runat="server" />",
    em: "<asp:literal id="jsEm" runat="server" />",
    newWidth: "<asp:literal id="jsNewWidth" runat="server" />",
    pixels: "<asp:literal id="jsPixels" runat="server" />",
    percent: "<asp:literal id="jsPercent" runat="server" />",
    save: "<asp:literal id="jsSave" runat="server" />",
    widget: "<asp:literal id="jsWidget" runat="server" />"
}

if(typeof(Sys) !== "undefined") Sys.Application.notifyScriptLoaded();
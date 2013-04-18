<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wizardResources.aspx.cs" Inherits="Workarea_PageBuilder_Wizards_js_wizardResources" %>
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
Ektron.ResourceText.PageBuilder.Wizards =
{
   addPage: "<asp:literal id="jsAddPage" runat="server"/>", 
   appPath: "<asp:literal id="jsAppPath" runat="server"/>",
   back: "<asp:literal id="jsBack" runat="server"/>",
   cancel: "<asp:literal id="jsCancel" runat="server"/>",
   errorPageTitle: "<asp:literal id="jsErrorPageTitle" runat="server"/>",
   errorSelectLayout: "<asp:literal id="jsErrorSelectLayout" runat="server"/>",
   errorUrlAlias: "<asp:literal id="jsErrorUrlAlias" runat="server"/>",
   errorUrlAliasExists: "<asp:literal id="jsErrorUrlAliasExists" runat="server" />",
   finish: "<asp:literal id="jsFinish" runat="server"/>",
   next: "<asp:literal id="jsNext" runat="server"/>",
   ok: "<asp:literal id="jsOk" runat="server"/>",
   path: "<asp:literal id="jsWizardsPath" runat="server"/>",
   savePageAs: "<asp:literal id="jsSavePageAs" runat="server"/>",
   dropdownMustMatch: "<asp:literal id="jsdropdownMustMatch" runat="server"/>",
   invalidExtension: "<asp:literal id="jsinvalidExtension" runat="server" />",
   selectExtension: "<asp:literal id="jsselectExtension" runat="server" />",
   errorMetadata: "<asp:literal id="jserrorMetadata" runat="server" />",
   errorTaxonomy: "<asp:literal id="jserrorTaxonomy" runat="server" />",
   loading: "<asp:literal id="jsloading" runat="server" />",
   addMasterLayout: "<asp:literal id="jsAddMaster" runat="server" />"
}

if(typeof(Sys) !== "undefined") Sys.Application.notifyScriptLoaded();
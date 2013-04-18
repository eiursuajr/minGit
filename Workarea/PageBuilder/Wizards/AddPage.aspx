<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddPage.aspx.cs" Inherits="Workarea_PageBuilder_Wizards_AddPage" %>

<%@ Register Src="../../controls/Editor/ContentDesignerWithValidator.ascx" TagName="ContentDesigner" TagPrefix="ektronUC" %>
<%@ Register Src="../../Community/DistributionWizard/Metadata.ascx" TagName="Metadata" TagPrefix="ektronUC" %>
<%@ Register Src="../taxonomytree.ascx" TagName="SelectTaxonomy" TagPrefix="ektronUC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head runat="server">
    <title>Add Page</title>
    <!--[if lt IE 8]>
    <style type="text/css">
        #ektronPageBuilderSummary {overflow: visible}
	table.ektronMetadataForm {width:97%;}
        ul.buttonWrapper {float:left;clear:both;padding-top:1em;}
        ul.buttonWrapper  li {float:left;}
    </style>
    <![endif]-->
    <script type="text/javascript">
        var currentMode = Ektron.QueryString["mode"];
        if ("saveAs" == currentMode)
        {
            Ektron.ready(function()
            {
                $ektron("#step1").hide();
                $ektron("#step2").show();
            });
        }
    </script>
</head>

<body class="ektronWizardFrame">
    <form id="form1" runat="server" class="ektronWizardForm">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div ID="ektronWizardStepWrapper" class="ektronWizardStepWrapper" runat="server">
            <div id="step1">
                <div class="ektronPageBuilderPageLayouts">
                    <h4>
                        <asp:Label ID="ektronPageBuilderPageLayoutsLabel" runat="server" />
                    </h4>
                    <div class="ektronFolderSelect">
                        <asp:Label ID="ektronPageBuilderPageLayoutsFolderLabel" runat="server"><%=m_refMsg.GetMessage("generic folder")%>: </asp:Label>
                        <asp:Label ID="ektronPageBuilderPageLayoutsSelectedFolderPath" runat="server"></asp:Label>
                        <a href="#" onclick="Ektron.PageBuilder.Wizards.AddPage.Templates.showChangeFolder(); return false;" id="ektronPageBuilderChangePath"><%=m_refMsg.GetMessage("btn change")%></a>
                        <div class="ektronPageBuilderPageLayoutsFolderSelector" style="display:none;">
                            <ul id="finder" runat="server">
                            </ul>
                        </div>
                    </div>
                    <asp:Label ID="ektronPageBuilderPleaseSelectLayout" CssClass="pageBuilderPleaseSelectLayout" runat="server" />
                    <div class="ektronTemplateListWrapper">
                        <asp:Literal ID="templates" runat="server" />
                        <asp:HiddenField ID="ektronSelectedTemplate" runat="server" />
                    </div>
                </div>
            </div>
            <div id="step2">
                <h4>
                    <asp:Label ID="ektronPageBuilderPageAliasing" ToolTip="Enter a title for the new layout and Select the taxonomy nodes you wish to associate it with" runat="server"><%=m_refMsg.GetMessage("lbl enter title for new layout")%></asp:Label>
                </h4>
                <div class="ektronAliasTaxonomy clearfix">
                    <div class="ektronAliasTaxLeft">
                        <fieldset>
                            <legend><%=m_refMsg.GetMessage("lbl pagebuilder page title")%></legend>
                            <div class="ektronPageTitle">
                                <label for="pageBuilderWizardPageTitle"><asp:Label ID="pageBuilderWizardPageTitleLabel" runat="server"><%=m_refMsg.GetMessage("generic title label")%> </asp:Label></label>
                                <asp:TextBox ID="pageBuilderWizardPageTitle" ToolTip="Enter Page Title here" runat="server" CssClass="inputText pageBuilderWizardPageTitle" autocomplete="off"></asp:TextBox>
                            </div>
                        </fieldset>
                        <fieldset class="ektronTaxonomy">
                            <legend><%=m_refMsg.GetMessage("generic taxonomy lbl")%></legend>
                            <div>
                                <ektronUC:SelectTaxonomy ID="selectTaxonomy" runat="server" 
                                JSCallBackOnChange="Ektron.PageBuilder.Wizards.AddPage.Aliasing.TaxonomyChangedCallBack" />
                            </div>
                        </fieldset>
                    </div>
                    <div class="ektronAliasing">
                        <fieldset>
                            <legend><%=m_refMsg.GetMessage("lbl forcemanualaliasing")%></legend>
                            <div class="ektronFieldInside">
                                <ul>
                                    <li><h5><%=m_refMsg.GetMessage("lbl tree url manual aliasing")%></h5>
                                        <asp:MultiView ID="ManualAliasing" runat="server">
                                            <asp:View ID="ManualAliasingEnabled" runat="server">
                                                <asp:CheckBox ToolTip="Create Manual Alias" ID="pageBuilderCreateManualAlias" runat="server" Checked="true" CssClass="createManualAlias" />
                                                <label for="pageBuilderCreateManualAlias">
                                                    <asp:Label ID="pageBuilderCreateManualAliasLabel" runat="server" ToolTip="Use Manual Aliasing"><%=m_refMsg.GetMessage("lbl Use Manual Aliasing")%></asp:Label>
                                                </label>
                                                <div class="manualContainer">
                                                    <span class="Folid" style="display:none">
                                                        <asp:HiddenField ID="pageBuilderFolderID" runat="server" />
                                                    </span>
                                                    <label for="pageBuilderWizardAlias">
                                                        <asp:Label ID="pageBuilderWizardAliasLabel" runat="server" />
                                                    </label>
                                                    <asp:TextBox ToolTip="Enter Page Builder Wizard Alias here" ID="pageBuilderWizardAlias" runat="server" CssClass="inputText pageBuilderWizardAlias" autocomplete="off"></asp:TextBox>
                                                        <asp:DropDownList ToolTip="Select Extension from Drop Down Menu" ID="ExtensionDropdown" runat="server"></asp:DropDownList>
                                                    <asp:Label CssClass="aliasPreviewLabel" ID="pageBuilderWizardAliasPreviewLabel" runat="server" />:
                                                    <span class="AliasPreview">
                                                        <asp:Literal ID="lblSitePath" runat="server"></asp:Literal>
                                                        <span id="aliasValue"></span>
                                                        <span id="extValue"></span>
                                                    </span>
                                                    <span class="InvalidAlias" style="display:none;" id="lbInvalidAlias" runat="server"></span>
                                                </div>
                                            </asp:View>
                                            <asp:View ID="ManualAliasingDisabled" runat="server">
                                                <span><%=m_refMsg.GetMessage("lbl Manual Aliasing Disabled")%></span>
                                            </asp:View>
                                            <asp:View ID="ManualAliasingUnallowed" runat="server">
                                                <span><%=m_refMsg.GetMessage("lbl permission Manual Aliases")%></span>
                                            </asp:View>
                                        </asp:MultiView>
                                    </li>
                                    <li><h5><%=m_refMsg.GetMessage("generic folder")%></h5>
                                        <asp:MultiView ID="FolderAliasing" runat="server">
                                            <asp:View ID="FolderAliasingEnabled" runat="server">
                                                <ul class="folderAlias">
                                                    <asp:Repeater ID="folderAliasRepeater" runat="server">
                                                        <ItemTemplate>
                                                            <li class="folderAlias" data-ektron-rawalias="<%# Container.DataItem %>"><%# Container.DataItem %></li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>
                                            </asp:View>
                                            <asp:View ID="FolderAliasingDisabled" runat="server">
                                                <span><%=m_refMsg.GetMessage("lbl Folder Aliasing Disabled")%></span>
                                            </asp:View>
                                            <asp:View ID="FolderAliasingNoAliases" runat="server">
                                                <span><%=m_refMsg.GetMessage("lbl no Folder Aliasing")%></span>
                                            </asp:View>
                                        </asp:MultiView>
                                    </li>
                                    <li><h5><%=m_refMsg.GetMessage("generic taxonomy lbl")%></h5>
                                        <asp:MultiView ID="TaxonomyAliasing" runat="server">
                                            <asp:View ID="TaxonomyAliasingEnabled" runat="server">
                                                <asp:Repeater ID="taxonomyAliasRepeater" runat="server">
                                                     <ItemTemplate>
                                                        <ul class="taxAlias"
                                                            data-ektron-taxroot="<%# ((Ektron.Cms.Common.UrlAliasAutoData)Container.DataItem).SourceId %>"
                                                            data-ektron-format="<%# ((Ektron.Cms.Common.UrlAliasAutoData)Container.DataItem).Example %>"
                                                            data-ektron-excludepath="<%#  ((Ektron.Cms.Common.UrlAliasAutoData)Container.DataItem).ExcludedPath %>"
                                                            data-ektron-extension="<%#  ((Ektron.Cms.Common.UrlAliasAutoData)Container.DataItem).FileExtension %>"
                                                            data-ektron-aliastype="<%#  Enum.GetName(typeof(Ektron.Cms.Common.EkEnumeration.AutoAliasNameType), ((Ektron.Cms.Common.UrlAliasAutoData)Container.DataItem).PageNameType) %>"
                                                            data-ektron-replacechar="<%#  ((Ektron.Cms.Common.UrlAliasAutoData)Container.DataItem).ReplacementCharacter %>">
                                                        <%# ((Ektron.Cms.Common.UrlAliasAutoData)Container.DataItem).Example %>
                                                        </ul>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </asp:View>
                                            <asp:View ID="TaxonomyAliasingDisabled" runat="server">
                                                <span><%=m_refMsg.GetMessage("lbl Taxonomy Aliasing Disabled")%></span>
                                            </asp:View>
                                            <asp:View ID="TaxonomyAliasingNoAliases" runat="server">
                                                <span><%=m_refMsg.GetMessage("lbl no Taxonomy Aliasing")%></span>
                                            </asp:View>
                                        </asp:MultiView>
                                    </li>
                                </ul>
                            </div>
                        </fieldset>
                    </div>
                </div>
                <asp:Button ID="btnFinish" runat="server" OnClick="btnFinish_Click" />
            </div>
            <div id="step3">
                <p class="ektronPageBuilderMetadata_TaxonomyIntro">
                    <asp:Literal ID="MetadataTaxonomyIntro" runat="server"></asp:Literal></p>
                <ul class="ektronPageBuilderTabs">
                    <li class="ektronPageBuilderTab selected"><a href="#ektronPageBuilderMetadata"><asp:Label ID="lblMetaDataTab" runat="server"></asp:Label></a></li>
                    <li class="ektronPageBuilderTab unselect"><a href="#ektronPageBuilderSummary"><asp:Label ID="lblSummaryTab" runat="server"></asp:Label></a></li>
                </ul>
                <div id="ektronPageBuilderMetadata" class="ektronPageBuilderTabPanel">
                    <ektronUC:Metadata ID="metadata" runat="server" ForceNewWindow="true" MetaUpdateString="Add" />
                </div>
                <div id="ektronPageBuilderSummary" class="ektronPageBuilderTabPanel">
                    <ektronUC:ContentDesigner ID="Summary" Height="165px" AllowScripts="true" runat="server" Visible="false" />
                </div>
            </div>
        </div>
        <div ID="redirectMessage" class="redirectMessage" runat="server">
            <h4><asp:Literal ID="pageCreationSuccess" runat="server" /></h4>
            <p>
                <asp:Label ID="redirectPrompt" runat="server"></asp:Label>
                <asp:HiddenField ID="fullAlias" runat="server" />
            </p>
        </div>
    </form>

</body>
</html>

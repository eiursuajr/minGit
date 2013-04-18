<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddMasterPage.aspx.cs" Inherits="Workarea_PageBuilder_Wizards_AddMasterPage" %>

<%@ Register Src="../../controls/Editor/ContentDesignerWithValidator.ascx" TagName="ContentDesigner" TagPrefix="ektronUC" %>
<%@ Register Src="../../Community/DistributionWizard/Metadata.ascx" TagName="Metadata" TagPrefix="ektronUC" %>
<%@ Register Src="../taxonomytree.ascx" TagName="SelectTaxonomy" TagPrefix="ektronUC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head runat="server">
    <title>Add Master Layout</title>
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

        Ektron.ready(function() { 
            $ektron("#pageBuilderWizardPageTitle").bind("keypress", function(e) {
                var charCheck;
                var k = e.keyCode ? e.keyCode : e.charCode ? e.charCode : e.which;
                if (String.fromCharCode(k) == "*"
                                        || String.fromCharCode(k) == "/"
                                        || String.fromCharCode(k) == "|"
                                        || String.fromCharCode(k) == "\""
                                        || String.fromCharCode(k) == ">"
                                        || String.fromCharCode(k) == "<"
                                        || String.fromCharCode(k) == "\'"
                                        || String.fromCharCode(k) == "&"
                                        || String.fromCharCode(k) == "\\") {
                    return false;
                }
                $var = $ektron($ektron(this).parent());
            });
        });
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
                        <a href="#" title="Change" onclick="Ektron.PageBuilder.Wizards.AddPage.Templates.showChangeFolder(); return false;" id="ektronPageBuilderChangePath"><%=m_refMsg.GetMessage("btn change")%></a>
                        <div class="ektronPageBuilderPageLayoutsFolderSelector" style="display:none;">
                            <ul id="finder" runat="server">
                            </ul>
                        </div>
                    </div>
                    <div class="ektronMasterPageTitle">
                        <label for="pageBuilderWizardPageTitle"><asp:Label ID="pageBuilderWizardPageTitleLabel" runat="server"><%=m_refMsg.GetMessage("generic title label")%> </asp:Label></label>
                        <asp:TextBox ToolTip="Enter Page Title here" ID="pageBuilderWizardPageTitle" runat="server" CssClass="inputText pageBuilderWizardPageTitle" autocomplete="off"></asp:TextBox>
                    </div>
                    <asp:Label ID="ektronPageBuilderPleaseSelectLayout" CssClass="pageBuilderPleaseSelectLayout" runat="server" />
                    <div class="ektronTemplateListWrapper">
                        <asp:Literal ID="templates" runat="server" />
                        <asp:HiddenField ID="ektronSelectedTemplate" runat="server" />
                    </div>
                </div>
                <asp:Button ID="btnFinish" runat="server" OnClick="btnFinish_Click" />
            </div>
            <div id="step3">
                <h4>
                    <span class="withMaster"><asp:Literal ID="MetadataTaxonomyIntro" runat="server"></asp:Literal></span></h4>
                <ul class="ektronPageBuilderTabs">
                    <li class="ektronPageBuilderTab selected"><a href="#ektronPageBuilderMetadata"><asp:Label ID="lblMetaDataTab" runat="server"></asp:Label></a></li>
                    <li class="ektronPageBuilderTab unselect"><a href="#ektronPageBuilderSummary"><asp:Label ID="lblSummaryTab" runat="server"></asp:Label></a></li>
                    <li class="ektronPageBuilderTab unselect"><a href="#ektronPageBuilderTaxonomy"><asp:Label ID="lblTaxonomyTab" runat="server" ToolTip="Taxonomy"><%=m_refMsg.GetMessage("generic taxonomy lbl")%></asp:Label></a></li>
                </ul>
                <div id="ektronPageBuilderMetadata" class="ektronPageBuilderTabPanel">
                    <ektronUC:Metadata ID="metadata" runat="server" ForceNewWindow="true" />
                </div>
                <div id="ektronPageBuilderSummary" class="ektronPageBuilderTabPanel">
                    <ektronUC:ContentDesigner ID="Summary" Height="165px" AllowScripts="true" runat="server" />
                </div>
                <div id="ektronPageBuilderTaxonomy" class="ektronPageBuilderTabPanel">
                    <ektronUC:SelectTaxonomy ID="selectTaxonomy" runat="server" />
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

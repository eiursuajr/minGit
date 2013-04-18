<%@ Page Language="C#" AutoEventWireup="false" CodeFile="exportjobs.aspx.cs" Inherits="Workarea_exportjobs" %>
<%@ Register TagPrefix="Ektron" TagName="LocaleTaxonomyTree" Src="localeTaxonomyTree.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Export for Translation</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />    
    <asp:Literal id="StyleSheetJS" runat="server" />
	<script type="text/javascript">
    <!--//--><![CDATA[//><!--	
	function SubmitForm(FormName, Validate) 
	{   
	    $ektron('#pleaseWait').modalShow();
		try
		{
			if (Validate.length > 0) {
				if (eval(Validate)) {
					document.forms[0].submit();
					return false;
				}
				else {
				    $ektron('#pleaseWait').modalHide();
					return false;
				}
			}
			else {
				document.forms[0].submit();
				return false;
			}
		}
		catch (e)
		{
			$ektron('#pleaseWait').modalHide();
			alert(e.message);
		}
	}

	function SetQueryNotReady() 
	{
	    document.getElementById("querynotreadyfortranslation").value = "1";
	}
	
	function validate()
	{
		var valid = true; 
		var objForm = document.forms[0];
		var objElem = null;

		if (valid)
		{
			objElem = objForm.elements["treeTargetJob$taxonomyselectedtree"];//todo: update when the tree is ready
			if (objElem && "" == objElem.value)
			{
				alert("<asp:Localize ID="JsSelectPackage" runat="server" Text="Please select a translation package." meta:resourcekey="JsSelectPackageResource" />");
				valid = false;
			}
		}
		//objForm.elements["action"].value = "LocalizeExport";
		return valid;
	}
	
	function setSourceLanguage(languageId)
	{
	    document.getElementById("languageSelector").value = languageId;
	}
	
	Ektron.ready(function() 
	{
	    document.getElementById("querynotreadyfortranslation").value = "0";
	});
	
	Ektron.ready( function() {
        // PLEASE WAIT MODAL
        $ektron("#pleaseWait").modal({
            trigger: '',
            modal: true,
            toTop: true,
            onShow: function(hash) {
                hash.o.fadeIn();
                hash.w.fadeIn();
            },
            onHide: function(hash) {
                hash.w.fadeOut("fast");
                hash.o.fadeOut("fast", function()
                {
                    if (hash.o) {
                        hash.o.remove();
                    }
                });
            }
        });
    });
    //--><!]]>	
	</script>	

<style type="text/css">
	div#pleaseWait { width: 128px; height: 128px; margin: -64px 0 0 -64px; background-color: #fff; background-image: url("../images/ui/loading_big.gif"); background-repeat: no-repeat; text-indent: -10000px; border: none; padding: 0; top: 50%; }
    
	input.JobTitle
	{
		width: 20em;
	}
</style>
</head>
<body>
    <form id="myform" name="myform" method="post" runat="server">
        <input type="hidden" name="querynotreadyfortranslation" id="querynotreadyfortranslation" value="0" />
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <div class="ektronWindow" id="pleaseWait">
                <h3><asp:Literal ID="LoadingImg" runat="server" /></h3>
            </div>
		    <div class="ektronPageGrid" id="divExport" runat="server">
		        <table class="ektronGrid">
		            <caption class="ektronHeader"><asp:Localize ID="JobCaption" runat="server" 
							Text="Translation Job" meta:resourcekey="JobCaptionResource1" /></caption>
		        <tbody>
		            <tr>
		                <td class="label"><asp:Label ID="lblSourceLanguage" Text="Source Language" 
								xAssociatedControlID="txtSourceLanguage" runat="server" 
								meta:resourcekey="lblSourceLanguageResource1" /></td>
		                <td class="value" id="SourceLangSelector" runat="server"></td>
		                <td class="description">&#160;<asp:HiddenField id="languageSelector" runat="server" /></td>
		            </tr>
		            <tr>
		                <td class="label"><asp:Label ID="lblJobTitle" Text="Job Title" 
								AssociatedControlID="txtJobTitle" runat="server" 
								meta:resourcekey="lblJobTitleResource1" /></td>
		                <td class="value"><asp:TextBox ID="txtJobTitle" MaxLength="40" CssClass="JobTitle" 
								runat="server" EnableViewState="False" ToolTip="Job Title" /></td>
		                <td class="description">&#160;</td>
		            </tr>
		            <tr>
		                <td class="label"><asp:Label ID="lblTargetJob" Text="Select Packages" 
								AssociatedControlID="treeTargetJob" runat="server" 
								meta:resourcekey="lblTargetJobResource1" /></td>
		                <td class="value">
		                <div runat="server">
							<Ektron:LocaleTaxonomyTree ID="treeTargetJob" runat="server" AllowSelectMultiple="true" ImpliedInheritance="Descendants" />
		                </div>
		                </td>
		                <td class="description">&#160;</td>
		            </tr>
		            <tr>
		                <td class="label"><asp:Label ID="lblXliffVer" Text="XLIFF Version" 
								AssociatedControlID="txtXliffVer" runat="server" 
								meta:resourcekey="lblXliffVerResource1" /></td>
		                <td class="value">
		                    <asp:RadioButtonList ID="txtXliffVer" runat="server" meta:resourcekey="txtXliffVerResource1">
								<asp:ListItem Value="1.0" Text="XLIFF 1.0 (for older Trados)" 
									meta:resourcekey="ListItemResource1" />
								<asp:ListItem Value="1.1" Text="XLIFF 1.1" 
									meta:resourcekey="ListItemResource2" />
								<asp:ListItem Value="1.2" Text="XLIFF 1.2 (recommended)" 
									meta:resourcekey="ListItemResource3" />
								<asp:ListItem Value="1.2.1" Text="XLIFF 1.2.1" 
								    meta:resourcekey="ListItemResource3a" />
		                    </asp:RadioButtonList>
		                </td>
		                <td class="description">
							<asp:Localize id="XliffVersionDesc" runat="server" 
								Text="The version to choose depends on the version(s) supported by the translation tool. Your translation provider should be able to guide you. Trados is a commonly used translation tool. Older versions of Trados (e.g., 7) only support XLIFF 1.0. When the Ektron CMS exports XLIFF 1.0, it includes source tags and target tags with a duplicate of the source content. Additionally, the file extension is .xml. This is needed for older versions of Trados. On the other hand, SDL Trados Studio 2009 and other XLIFF tools do not need the target tags, so when XLIFF 1.1 and later are produced, the target tags are not included and the file extension is .xlf. The XLIFF tool will add target tags during the translation process." 
								meta:resourcekey="XliffVersionDescResource1"/>
		                </td>
		            </tr>
		            <tr>
		                <td class="label">
                            <asp:Label ID="lblMaxZipSize" Text="ZIP File Size" 
								AssociatedControlID="lstMaxZipSize" runat="server" meta:resourcekey="lblMaxZipSizeResource1"/></td>
		                <td class="value">
		                    <asp:DropDownList ID="lstMaxZipSize" runat="server" CssClass="right" 
                                meta:resourcekey="lstMaxZipSizeResource1">
		                        <asp:ListItem Value="20000000" Text="20 MB" 
                                    meta:resourcekey="ListItemResource4" />
		                        <asp:ListItem Value="200000000" Text="200 MB" 
                                    meta:resourcekey="ListItemResource5" />
		                        <asp:ListItem Value="2000000000" Text="2 GB" 
                                    meta:resourcekey="ListItemResource6" />
		                        <asp:ListItem Value="20000000000" Text="20 GB" 
                                    meta:resourcekey="ListItemResource7" />
		                        <asp:ListItem Value="200000000000" Text="200 GB" 
                                    meta:resourcekey="ListItemResource8" />
		                        <asp:ListItem Value="" Text="Unlimited" meta:resourcekey="ListItemResource9" />
		                    </asp:DropDownList>
		                </td>
		                <td class="description">
							<asp:Localize id="Localize1" runat="server" 
                                Text="Exported XLIFF files are grouped in ZIP files. The size may be limited for download and email attachments." 
                                meta:resourcekey="Localize1Resource1"/>
		                </td>
		            </tr>
		            <%--<tr>
		                <td class="label">&#160;</td>
		                <td class="value">
		                    <asp:CheckBox ID="chkIncludeHistory" runat="server" 
		                        Text="Include Previous Translation" 
		                        ToolTip="Include Previous Translation"
								meta:resourcekey="lblIncludeHistoryJobResource1" />
		                </td>
		                <td class="description"><asp:Localize ID="IncludeHistoryDesc" runat="server"
		                    Text="This is usually needed when there is a switch in translation providers. If checked, the package file will include the previous version of source file and the previous version of translated file. The package size will be increased."
		                    meta:resourcekey="IncludeHistoryDescResource1" />
		                </td>
		            </tr>--%>
				</tbody>
		        </table>
		    </div>
            <div class="ektronTopSpace"></div>
            <asp:PlaceHolder runat="server" ID="plcXLIFFData">
			<div id="xliffDataArea">
				<div class="ektronHeader"><%= GetMessage("lbl generic history")%></div>
				<div class="ektronBorder">
					<iframe src="localizationjobs.aspx" height="360" width="100%" title="History"></iframe>
				</div>
			</div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcNotReadyContent" Visible="false">
            <div class="ektronHeader"><asp:Localize runat="server" id="ContentNotReadyForTranslationTitle"
                Text="Content Not Ready for Translation"
                meta:resourcekey="ContentNotReadyForTranslationResource1" /></div>
            <div class="ektronBorder">
                <asp:Repeater runat="server" ID="rptNotReadyContent">
                    <HeaderTemplate>
                        <table class="ektronGrid" cellspacing="0" rules="all" border="1" style="border-collapse: collapse; display: table;">
	                    <thead>
	                    <tr class="title-header">
		                    <th class="title-header"><%= GetMessage("generic title")%></td>
		                    <%--<th class="title-header">Status</td>--%>
		                    <th class="title-header"><%= GetMessage("content type")%></td>
		                    <%--<th class="title-header center"><%= GetMessage("generic language")%></td>--%>
		                    <th class="title-header center"><%= GetMessage("generic ID")%></td>
		                    <th class="title-header"><%= GetMessage("generic date modified")%></td>
	                    </tr>
	                    </thead>
	                    <tbody>
					</HeaderTemplate>
                    <ItemTemplate>
                        <tr>
		                    <td class="left"><a href="<%# BuildContentLink((Ektron.Cms.BusinessObjects.Localization.ILocalizable)Container.DataItem) %>" title="<%# Eval("Title") %>"><%# Eval("Title") %></a></td>
		                    <%--<td class="left"><%# GetLocalizationState((Ektron.Cms.BusinessObjects.Localization.ILocalizable)Container.DataItem)%></td>--%>
		                    <td class="left"><%# GetContentTypeName((Ektron.Cms.BusinessObjects.Localization.ILocalizable)Container.DataItem) %></td>
		                    <%--<td class="center"><%# GetContentLanguageName((int)Eval("LanguageId")) %></td>--%>
		                    <td class="center"><%# Eval("Id") %></td>
		                    <td class="left"><%# Eval("DateModified") %></td>
	                    </tr>
                    </ItemTemplate>
                    <FooterTemplate>
						</tbody>
						</table>
					</FooterTemplate>
                </asp:Repeater>
                <asp:Localize runat="server" ID="lblNoResultsForNotReady" Visible="false" 
                    meta:resourcekey="lblNoResultsForNotReadyResource2">All content is ready for translation.</asp:Localize>
            </div>
            </asp:PlaceHolder>
        </div>
    </form>
</body>
</html>

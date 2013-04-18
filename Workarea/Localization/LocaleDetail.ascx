<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LocaleDetail.ascx.cs" Inherits="Workarea_LocaleDetail" %>
<%@Register TagPrefix="ektron" TagName="CountryMap" Src="../controls/reports/CountryMap.ascx" %>

<span style="display:none">
	<asp:CheckBox ID="Save" AutoPostBack="True" CausesValidation="True" 
	runat="server" EnableViewState="False" meta:resourcekey="SaveResource1" />
	<asp:CheckBox ID="Update" AutoPostBack="True" runat="server" 
	EnableViewState="False" meta:resourcekey="UpdateResource1" />
	<asp:HiddenField ID="hdnLocaleId" Value="-1" runat="server" 
	EnableViewState="False" />
	<asp:HiddenField ID="hdnRecommendedFlag" runat="server" 
	EnableViewState="False" />
	<asp:CheckBox ID="chkIsRightToLeft" runat="server" EnableViewState="False" meta:resourcekey="chkIsRightToLeftResource1" />
	<asp:HiddenField ID="hdnState" Value="Active" runat="server" 
	EnableViewState="False" />
</span>
<script type="text/javascript">
<!--
Ektron.ready(function()
{
	function updateLocaleDetail()
	{
		$ektron("#<%= Update.ClientID %>").click();
	}
	$ektron("#<%= LanguageSelector.ClientID %>").change(updateLocaleDetail);
	$ektron("#<%= RegionSelector.ClientID %>").change(updateLocaleDetail);
	$ektron("#<%= ScriptSubtagSelector.ClientID %>").change(updateLocaleDetail);
});
// -->
</script>
<table class="ektronGrid">
    <tbody>
		<tr>
			<td class="label">
				<asp:Label ID="lblLanguage" Text="Language" 
					AssociatedControlID="LanguageSelector" runat="server" meta:resourcekey="lblLanguageResource1" />
			</td>
			<td class="value">
				<asp:Literal ID="LanguageSelector" runat="server" EnableViewState="False"/> <%-- languageList.xsl --%>
				<br />
				<asp:RadioButtonList ID="LanguageSet" runat="server" RepeatLayout="Flow" 
					RepeatDirection="Horizontal" AutoPostBack="True" meta:resourcekey="LanguageSetResource1">
					<asp:ListItem Text="Recommended" Value="recommended" Selected="True" meta:resourcekey="ListItemResource1" />
					<asp:ListItem Text="Common" Value="common" meta:resourcekey="ListItemResource2" />
					<asp:ListItem Text="Show All" Value="all" meta:resourcekey="ListItemResource3" />
				</asp:RadioButtonList>
			</td>
			<td class="description">
				<asp:Localize ID="LanguageDesc" runat="server" 
					Text="Select a language. When a country is selected, percentages shown for recommended languages refer to the population within that country who know that language, based on information in {0:yyyy}." 
					meta:resourcekey="LanguageDescResource1" />
			</td>
		</tr>
		<tr>
			<td class="label">
				<asp:Label ID="lblRegion" runat="server" Text="Region" 
					AssociatedControlID="RegionSelector" meta:resourcekey="lblRegionResource1" />
			</td>
			<td class="value">
				<asp:Literal ID="RegionSelector" runat="server" EnableViewState="False"/> <%-- territoryList.xsl --%>
				<br />
				<asp:RadioButtonList ID="RegionSet" runat="server" RepeatLayout="Flow" 
					RepeatDirection="Horizontal" AutoPostBack="True" meta:resourcekey="RegionSetResource1">
					<asp:ListItem Text="Recommended" Value="recommended" Selected="True" meta:resourcekey="ListItemResource4" />
					<asp:ListItem Text="Common" Value="common" meta:resourcekey="ListItemResource5" />
					<asp:ListItem Text="Show All" Value="all" meta:resourcekey="ListItemResource6" />
				</asp:RadioButtonList>
			</td>
			<td class="description">
				<asp:Image ID="LocatorMap" CssClass="locatorMap" runat="server" 
					EnableViewState="False" Visible="False" meta:resourcekey="LocatorMapResource1" 
					 />
				<asp:Localize ID="RegionSubtagDesc" 
					Text="Select the geographical area most closely associated with the locale. The territory may be a single country or multinational region. When a language is selected, recommended countries are those where the language is spoken." 
					runat="server" meta:resourcekey="RegionSubtagDescResource1" />
				<ektron:CountryMap ID="CountryMap" CssClass="regionMap" runat="server" Width="240" />
			</td>
		</tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblLoc" Text="Locale" AssociatedControlID="txtLoc" 
					runat="server" meta:resourcekey="lblLocResource1" /></td>
            <td class="value">
				<asp:TextBox ID="txtLoc" MaxLength="20" runat="server" EnableViewState="False" meta:resourcekey="txtLocResource1" />
				<asp:RequiredFieldValidator ID="rfvLoc" ControlToValidate="txtLoc" 
					ErrorMessage="* required" Display="Dynamic" runat="server" meta:resourcekey="rfvLocResource1" />
				<asp:RegularExpressionValidator ID="revLoc" ControlToValidate="txtLoc" 
					ErrorMessage="* invalid" Display="Dynamic" runat="server" 
					EnableViewState="False" meta:resourcekey="revLocResource1" />
				<asp:CustomValidator ID="valLoc" ControlToValidate="txtLoc" 
					ErrorMessage="* must be unique" Display="Dynamic" runat="server" 
					EnableViewState="False" meta:resourcekey="valLocResource1" />
				<p><asp:Localize ID="litLocRec" runat="server" Text="Recommended:" 
						meta:resourcekey="litLocRecResource1" /> <asp:Label ID="RecommendedLoc" 
						Text="?" runat="server" meta:resourcekey="RecommendedLocResource1" /></p>
				<p class="required"><asp:Localize ID="litLocReq" runat="server" 
						Text="Must be unique. Up to 20 letters, number, underscores (_) and dashes (-). No spaces." 
						meta:resourcekey="litLocReqResource1" /></p>
			</td>
            <td class="description">
				<asp:Localize ID="LocDesc" 
					Text="The locale is an alphanumeric key uniquely identifing this language-region combination. This value may be used for the 'loc' Url parameter. This value may be customized to your needs. It does not need to conform to international standards." 
					runat="server" meta:resourcekey="LocDescResource1" />
			</td>
        </tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblFallbackLoc" Text="Fallback Locale" 
					AssociatedControlID="lstFallbackLoc" runat="server" meta:resourcekey="lblFallbackLocResource1" /></td>
            <td class="value">
				<asp:DropDownList ID="lstFallbackLoc" runat="server" meta:resourcekey="lstFallbackLocResource1" />
				<p><asp:Localize ID="FallbackLocRec" runat="server" Text="Recommended:" 
						meta:resourcekey="FallbackLocRecResource1" /> 
					<asp:Label ID="RecommendedFallbackLoc" Text="?" runat="server" meta:resourcekey="RecommendedFallbackLocResource1" /></p>
			</td>
            <td class="description">
				<asp:Localize ID="FallbackLocDesc" 
					Text="Content will display in the fallback locale if it is not available in the specified locale. Only enabled locales may be selected." 
					runat="server" meta:resourcekey="FallbackLocDescResource1" />
			</td>
        </tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblXmlLang" Text="Language Tag" 
					AssociatedControlID="txtXmlLang" runat="server" meta:resourcekey="lblXmlLangResource1" /></td>
            <td class="value">
				<asp:TextBox ID="txtXmlLang" runat="server" ReadOnly="True" 
					EnableViewState="False" meta:resourcekey="txtXmlLangResource1" />
				<table class="ektronGrid">
				<tbody>
					<tr runat="server" id="rowScriptSubtag" visible="true">
						<td class="label">
							<asp:Label ID="lblScriptSubtag" Text="Alphabet" 
								AssociatedControlID="ScriptSubtagSelector" runat="server" meta:resourcekey="lblScriptSubtagResource1" />
						</td>
						<td class="value">
							<asp:Literal ID="ScriptSubtagSelector" runat="server" EnableViewState="False" 
								meta:resourcekey="ScriptSubtagSelectorResource1"/> <%-- scriptList.xsl --%>
							<br />
							<asp:RadioButtonList ID="ScriptSubtagSet" runat="server" RepeatLayout="Flow" 
								AutoPostBack="True" meta:resourcekey="ScriptSubtagSetResource1">
								<asp:ListItem Text="Recommended" Value="recommended" Selected="True" meta:resourcekey="ListItemResource7" />
								<asp:ListItem Text="Common" Value="common" meta:resourcekey="ListItemResource8" />
								<asp:ListItem Text="Show All" Value="all" meta:resourcekey="ListItemResource9" />
							</asp:RadioButtonList>
							<div class="description">
								<asp:Localize ID="ScriptSubtagDesc" 
									Text="The alphabet specifies the set of letters and characters used to write the language." 
									runat="server" meta:resourcekey="ScriptSubtagDescResource1" />
							</div>
						</td>
						<%--<td class="description">
						</td>--%>
					</tr>
					<tr runat="server" id="rowPrivateUseSubtag" visible="true">
						<td class="label">
							<asp:Label ID="lblPrivateUseSubtag" Text="Custom Ext." 
								AssociatedControlID="txtPrivateUseSubtag" runat="server" meta:resourcekey="lblPrivateUseSubtagResource1" />
						</td>
						<td class="value">
							x- 
							<asp:TextBox ID="txtPrivateUseSubtag" runat="server" EnableViewState="False" meta:resourcekey="txtPrivateUseSubtagResource1" />
							<asp:RegularExpressionValidator ID="revPrivateUseSubtag" 
								ControlToValidate="txtPrivateUseSubtag" 
								ErrorMessage="One to eight letters or numbers" Display="Dynamic" runat="server" 
								EnableViewState="False" meta:resourcekey="revPrivateUseSubtagResource1" />
							<div class="description">
								<asp:Localize ID="PrivateUseSubtagDesc" 
									Text="The custom extension (i.e., private use subtag) is meaningful only within this system. The following are associated with objectFactory.config strategies." 
									runat="server" meta:resourcekey="PrivateUseSubtagDescResource1" />
				                <ul>
					                <li>
								        <strong><asp:Localize ID="MachineTranslationExt" 
									        Text="mt" 
									        runat="server" meta:resourcekey="MachineTranslationExtResource1" /></strong>
									    <asp:Localize ID="MachineTranslationExtName" 
									        Text="(machine translation)" 
									        runat="server" meta:resourcekey="MachineTranslationNameResource1" />
									    <br />
								        <asp:Localize ID="MachineTranslationDesc" 
									        Text="When content is exported to this locale, it is translated using machine translation and immediately imported." 
									        runat="server" meta:resourcekey="MachineTranslationDescResource1" />
									    <asp:Panel ID="MachineTranslationUndefinedPanel" CssClass="important" runat="server">
								            <asp:Localize ID="MachineTranslationUndefined" 
									            Text="No strategy for 'MachineTranslation' has been defined. The x-mt subtag will simply copy the content when exported."
									            runat="server" meta:resourcekey="MachineTranslationUndefinedResource1" />
									    </asp:Panel>
					                </li>
					                <li>
								        <strong><asp:Localize ID="PseudoLocalizationExt" 
									        Text="pseudo" 
									        runat="server" meta:resourcekey="PseudoLocalizationExtResource1" /></strong>
									    <asp:Localize ID="PseudoLocalizationExtName" 
									        Text="(pseudo localization)" 
									        runat="server" meta:resourcekey="PseudoLocalizationNameResource1" />
									    <br />
								        <asp:Localize ID="PseudoLocalizationDesc" 
									        Text="When content is exported to this locale, it is converted to pseudo-language text and immediately imported." 
									        runat="server" meta:resourcekey="PseudoLocalizationDescResource1" />
									    <asp:Panel ID="PseudoLocalizationUndefinedPanel" CssClass="important" runat="server">
									        <asp:Localize ID="PseudoLocalizationUndefined" 
									            Text="No strategy for 'PseudoLocalization' has been defined. The x-pseudo subtag will simply copy the content when exported." 
									            runat="server" meta:resourcekey="PseudoLocalizationUndefinedResource1" />
									    </asp:Panel>
					                </li>
				                </ul>
							</div>
						</td>
						<%--<td class="description">
						</td>--%>
					</tr>
				</tbody>
				</table>
            </td>
			<td class="description">
				<p><asp:Localize ID="XmlLangDescP1" runat="server" 
						Text="The language tag informs translators and browsers of the language and region of this content. 
				This value must conform to the {0} (Best Current Practice), which at this date is defined by {1}." 
						meta:resourcekey="XmlLangDescP1Resource1" />
				<asp:HyperLink ID="BCP47" Text="IETF BCP 47" 
						NavigateUrl="http://tools.ietf.org/html/bcp47" Target="_blank" runat="server" 
						meta:resourcekey="BCP47Resource1"/>
				<asp:HyperLink ID="RFC5646" Text="RFC 5646" 
						NavigateUrl="http://www.rfc-editor.org/rfc/rfc5646.txt" Target="_blank" 
						runat="server" meta:resourcekey="RFC5646Resource1"/></p>
				<p><asp:Localize ID="XmlLangDescP2" runat="server" Text="The alphabet is the script subtag of the language tag, however, the script subtag is omitted from the language tag when it is the presumed script. 
				For example, English is presumed written in Latin letters." 
						meta:resourcekey="XmlLangDescP2Resource1" /></p>
				<p><asp:Localize ID="XmlLangDescP3" runat="server" 
						Text="The custom extension is the private use subtag of the language tag, however, it should be avoided unless absolutely necessary." 
						meta:resourcekey="XmlLangDescP3Resource1" /></p>
				<p><asp:Localize ID="XmlLangDescP4" runat="server" 
						Text="The following sites provide more information about language tags." 
						meta:resourcekey="XmlLangDescP4Resource1" /></p>
				<ul>
					<li>
						<asp:HyperLink Text="W3C Choosing a Language Tag" 
							NavigateUrl="http://www.w3.org/International/questions/qa-choosing-language-tags" 
							Target="_blank" runat="server" meta:resourcekey="HyperLinkResource1"/>
					</li>
					<li>
						<asp:HyperLink Text="W3C Language tags in HTML and XML" 
							NavigateUrl="http://www.w3.org/International/articles/language-tags/Overview.en.php" 
							Target="_blank" runat="server" meta:resourcekey="HyperLinkResource2"/>
					</li>
					<li>
						<asp:HyperLink Text="Language Subtag Lookup" 
							NavigateUrl="http://rishida.net/utils/subtags/" Target="_blank" runat="server" meta:resourcekey="HyperLinkResource3" />
					</li>
				</ul>
			</td>
        </tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblCulture" runat="server" Text="ASP.NET Culture" 
					AssociatedControlID="lstCulture" meta:resourcekey="lblCultureResource1" />
			</td>
            <td class="value">
				<asp:DropDownList ID="lstCulture" runat="server" meta:resourcekey="lstCultureResource1" />
				<p><asp:Localize ID="CultureRec" runat="server" Text="Recommended:" 
						meta:resourcekey="CultureRecResource1" /> 
					<asp:Label ID="RecommendedCulture" Text="?" runat="server" meta:resourcekey="RecommendedCultureResource1" /></p>
            </td>
            <td class="description">
            	<p><asp:Localize ID="CultureDesc" runat="server" 
						Text="The ASP.NET Culture specifies the region for formatting dates, times, currencies, etc. This value is assigned to the ASP.NET 'Culture' property." 
						meta:resourcekey="CultureDescResource1" /></p>
			</td>
        </tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblUICulture" runat="server" 
					Text="ASP.NET UICulture" AssociatedControlID="lstUICulture" meta:resourcekey="lblUICultureResource1" />
			</td>
            <td class="value">
				<asp:DropDownList ID="lstUICulture" runat="server" meta:resourcekey="lstUICultureResource1" />
				<p><asp:Localize ID="litUICultureRec" runat="server" Text="Recommended:" 
						meta:resourcekey="litUICultureRecResource1" /> 
					<asp:Label ID="RecommendedUICulture" Text="?" runat="server" meta:resourcekey="RecommendedUICultureResource1" /></p>
			</td>
            <td class="description">
				<p><asp:Localize ID="UICultureDesc" runat="server" 
						Text="ASP.NET UICulture specifies the natural language of resource strings. This value is assigned to the ASP.NET 'UICulture' property." 
						meta:resourcekey="UICultureDescResource1" /></p>
            </td>
        </tr>
        <tr>
            <td class="label"><asp:Localize ID="litCustomCultures" runat="server" 
					Text="Custom Cultures" meta:resourcekey="litCustomCulturesResource1" />
			</td>
            <td class="description" colspan="2">
				<p><asp:Localize ID="CustomCulturesDesc" runat="server" 
						Text="On Windows Vista and later, the Microsoft Locale Builder may be used to create custom, supplemental ASP.NET cultures. 
				Custom cultures are most helpful if no ASP.NET Culture matches either the language or the region, for example, eo-150 (Esperanto in Europe). 
				Custom cultures may also be created for new combinations, for example, fr-US (French in U.S.). For information about custom cultures and the Locale Builder, see the following Ektron article:" 
						meta:resourcekey="CustomCulturesDescResource1" />
				<asp:HyperLink Text="Using Locale Builder to create custom locales" 
						NavigateUrl="http://dev.ektron.com/kb_article.aspx?id=31465" Target="_blank" 
						runat="server" meta:resourcekey="HyperLinkResource4" /></p>
            </td>
        </tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblFlag" runat="server" Text="Flag Icon" 
					AssociatedControlID="rblFlag" meta:resourcekey="lblFlagResource1" />
			</td>
            <td class="value">
				<asp:RadioButtonList ID="rblFlag" runat="server" meta:resourcekey="rblFlagResource1" />
            </td>
            <td class="description">
				<asp:Localize ID="FlagDesc" 
					Text="The flag provides a graphical icon to represent the locale. It may be language-centric or region-centric to minimize duplicates. Disabled flags are files that do not exist, but you may create one for the locale. Flag files must be 16x16 pixel GIF images in the flags folder." 
					runat="server" meta:resourcekey="FlagDescResource1" />
				<asp:Label ID="lblFlagFolder" runat="server" EnableViewState="False" meta:resourcekey="lblFlagFolderResource1" />
			</td>
        </tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblNativeName" runat="server" Text="Native Name" 
					AssociatedControlID="txtNativeName" meta:resourcekey="lblNativeNameResource1" /></td>
            <td class="value">
				<asp:TextBox ID="txtNativeName" runat="server" Width="20em" 
					EnableViewState="False" meta:resourcekey="txtNativeNameResource1" />
				<asp:RequiredFieldValidator ID="rfvNativeName" 
					ControlToValidate="txtNativeName" ErrorMessage="* required" Display="Dynamic" 
					runat="server" EnableViewState="False" meta:resourcekey="rfvNativeNameResource1" />
				<asp:RegularExpressionValidator ID="revNativeName" 
					ControlToValidate="txtNativeName" ErrorMessage="Maximum of 50 characters" 
					Display="Dynamic" runat="server" EnableViewState="False" meta:resourcekey="revNativeNameResource1" />
				<p><asp:Localize ID="NativeNameRec" runat="server" Text="Recommended:" 
						meta:resourcekey="NativeNameRecResource1" /> 
					<asp:Label ID="RecommendedNativeName" Text="?" runat="server" meta:resourcekey="RecommendedNativeNameResource1" /><br />
				<span id="AlternateNativeNameContainer" runat="server" visible="false" enableviewstate="false">
					<asp:Localize ID="NativeNameAlt" runat="server" Text="Alternate:" 
						meta:resourcekey="NativeNameAltResource1" /> 
					<asp:Label ID="AlternateNativeName" runat="server" meta:resourcekey="AlternateNativeNameResource1" /></span></p>
			</td>
            <td class="description">
				<asp:Localize ID="NativeNameDesc" runat="server" 
					Text="Name of the locale in its native natural language, that is, what the people in that locale call their own language and region." 
					meta:resourcekey="NativeNameDescResource1" />
			</td>
        </tr>
        <tr>
            <td class="label">
				<asp:Label ID="lblEnglishName" runat="server" Text="English Name" 
					AssociatedControlID="txtEnglishName" meta:resourcekey="lblEnglishNameResource1" />
			</td>
            <td class="value">
				<asp:TextBox ID="txtEnglishName" runat="server" Width="20em" 
					EnableViewState="False" meta:resourcekey="txtEnglishNameResource1" />
				<asp:RequiredFieldValidator ID="rfvEnglishName" 
					ControlToValidate="txtEnglishName" ErrorMessage="* required" Display="Dynamic" 
					runat="server" EnableViewState="False" meta:resourcekey="rfvEnglishNameResource1" />
				<asp:RegularExpressionValidator ID="revEnglishName" 
					ControlToValidate="txtEnglishName" ErrorMessage="Maximum of 50 characters" 
					Display="Dynamic" runat="server" EnableViewState="False" meta:resourcekey="revEnglishNameResource1" />
				<p><asp:Localize ID="EnglishNameRec" runat="server" Text="Recommended:" 
						meta:resourcekey="EnglishNameRecResource1" /> 
					<asp:Label ID="RecommendedEnglishName" Text="?" runat="server" meta:resourcekey="RecommendedEnglishNameResource1" /><br />
				<span id="AlternateEnglishNameContainer" runat="server" visible="false" enableviewstate="false">
					<asp:Localize ID="EnglishNameAlt" runat="server" Text="Alternate:" 
						meta:resourcekey="EnglishNameAltResource1" /> 
					<asp:Label ID="AlternateEnglishName" runat="server" meta:resourcekey="AlternateEnglishNameResource1" /></span></p>
			</td>
            <td class="description">
				<asp:Localize ID="EnglishNameDesc" Text="Name of the locale in English." 
					runat="server" meta:resourcekey="EnglishNameDescResource1" />
			</td>
        </tr>
    </tbody>
</table>


<?xml version='1.0'?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- Transform on CLDR/common/main/{$lang}.xml -->

<xsl:output method="xml" version="1.0" indent="yes" encoding="utf-8"/>

<xsl:include href="includeForList.xsl"/>

<xsl:key name="script" match="/ldml/localeDisplayNames/scripts/script" use="@type"/>

<xsl:template match="/">
	<select id="{$controlId}" name="ScriptSubtag" size="1" lang="{$lang}">
		<xsl:if test="string-length($prompt) &gt; 0">
			<option value=""><xsl:value-of select="$prompt"/></option>
		</xsl:if>
		<xsl:variable name="list">
			<xsl:variable name="scripts" select="/ldml/localeDisplayNames/scripts/script[not(@alt)]"/>
			<xsl:choose>
				<xsl:when test="$subset='recommended' and string-length($selectedLanguageOnly) &gt; 0 and $selectedLanguageOnly != 'und'">
					<xsl:variable name="info" select="$data/languageData/language[@type=$selectedLanguageOnly]/@scripts"/>
					<xsl:apply-templates select="$scripts[contains($info,@type) or @type=$likelyScript]"/>
				</xsl:when>
				<xsl:when test="$subset='common' or $subset='recommended'">
					<xsl:variable name="coverage" select="$meta/metadata/coverageAdditions/scriptCoverage"/>
					<!--
            		<scriptCoverage type="basic" values="Latn Hans Hant Cyrl Arab"/>
					-->
					<xsl:variable name="common" select="$coverage[@type='basic']/@values"/>
					<xsl:apply-templates select="$scripts[contains($common,@type) or @type=$likelyScript]"/>
				</xsl:when>
				<xsl:when test="$subset='all'">
					<xsl:apply-templates select="$scripts"/>
				</xsl:when>
			</xsl:choose>
		</xsl:variable>
		<xsl:for-each select="msxsl:node-set($list)/option">
			<xsl:sort select="text()" lang="{$lang}" />
			<xsl:copy-of select="."/>
		</xsl:for-each>
	</select>
	<input type="hidden" id="Likely{$controlId}" name="LikelyLanguageTag" value="{translate($likelySubtag,'_','-')}" />
</xsl:template>

<xsl:template match="script">
	<xsl:variable name="name" select="normalize-space(text())"/>
	<xsl:if test="string-length($name) &gt; 0">
		<xsl:if test="$registry/script[subtag=current()/@type]">
			<option value="{@type}">
				<xsl:if test="$likelyScript=@type">
					<xsl:attribute name="selected">selected</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="$name"/>
			</option>
		</xsl:if>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
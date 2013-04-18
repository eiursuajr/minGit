<?xml version='1.0'?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- Transform on CLDR/common/main/{$lang}.xml -->

<xsl:output method="xml" version="1.0" indent="yes" encoding="utf-8"/>

<xsl:include href="includeForList.xsl"/>

<xsl:key name="language" match="/ldml/localeDisplayNames/languages/language" use="@type"/>

<xsl:template match="/">
	<select id="{$controlId}" name="Language" size="1" lang="{$lang}">
		<xsl:if test="string-length($prompt) &gt; 0">
			<option value=""><xsl:value-of select="$prompt"/></option>
		</xsl:if>
		<xsl:variable name="list">
			<xsl:variable name="languages" select="/ldml/localeDisplayNames/languages/language"/>
			<xsl:variable name="info" select="$data/territoryInfo/territory[@type=$selectedTerritory]/languagePopulation"/>
			<xsl:choose>
				<xsl:when test="$subset='recommended' and string-length($selectedTerritory) &gt; 0 and count($info) &gt; 0">
					<xsl:apply-templates select="$languages[@type=$info/@type or (substring-before(@type,'_')=$info/@type and substring-after(@type,'_')=$selectedTerritory) or @type=$likelyLanguage]">
						<xsl:with-param name="languagePopulation" select="$info"/>
					</xsl:apply-templates>
				</xsl:when>
				<xsl:when test="$subset='common' or $subset='recommended'">
					<xsl:variable name="coverage" select="$meta/metadata/coverageAdditions/languageCoverage"/>
					<!--
		            <languageCoverage type="basic" values="de en es fr it ja pt ru zh zh_Hans zh_Hant"/>
		            <languageCoverage type="moderate" values="ar hi ko in nl bn tr th pl"/>            
		            <languageCoverage type="modern" values="de_CH de_AT en_AU en_CA en_GB en_US es_419 es_ES fr_CA fr_CH nl_BE pt_BR pt_PT"/> 
					-->
					<xsl:variable name="common" select="concat(' ',$coverage[@type='basic']/@values,' ',$coverage[@type='moderate']/@values,' ',$coverage[@type='modern']/@values,' ')"/>
					<xsl:apply-templates select="$languages[contains($common,concat(' ',@type,' ')) or @type=$likelyLanguage]"/>
				</xsl:when>
				<xsl:when test="$subset='all'">
					<xsl:apply-templates select="$languages"/>
				</xsl:when>
			</xsl:choose>
		</xsl:variable>
		<xsl:for-each select="msxsl:node-set($list)/*">
			<xsl:sort select="@percent" data-type="number" order="descending"/>
			<xsl:sort select="@sort" lang="{$lang}"/>
			<xsl:copy>
				<xsl:copy-of select="@*[name()!='percent' and name()!='sort']"/>
				<xsl:copy-of select="node()"/>
			</xsl:copy>
		</xsl:for-each>
	</select>
</xsl:template>

<xsl:template match="language">
	<xsl:param name="languagePopulation" select="null"/>

	<xsl:variable name="name" select="normalize-space(text())"/>
	<xsl:if test="string-length($name) &gt; 0">
		<xsl:variable name="percent" select="$languagePopulation[@type=current()/@type or @type=concat(current()/@type,'_',$likelyScript)]/@populationPercent"/>
		<xsl:choose>
			<xsl:when test="string-length($percent) &gt; 0">
				<xsl:variable name="patternNode" select="/ldml/numbers/percentFormats/percentFormatLength/percentFormat/pattern"/>
				<xsl:variable name="pattern">
					<xsl:choose>
						<xsl:when test="string-length($patternNode) &gt; 0">
							<xsl:value-of select="$patternNode"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="'##0%'"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<option value="{translate(@type,'_','-')}" percent="{$percent}" sort="{$name}">
					<xsl:if test="$likelyLanguage=@type">
						<xsl:attribute name="selected">selected</xsl:attribute>
					</xsl:if>
					<xsl:value-of select="concat($name,' (',format-number((number($percent)+0.05) div 100,$pattern),')')"/>
				</option>
			</xsl:when>
			<xsl:when test="contains(@type,'_')">
				<xsl:variable name="fallback" select="substring-before(@type,'_')"/>
				<xsl:variable name="rootLang" select="key('language',$fallback)"/>
				<xsl:variable name="rootPercent" select="$languagePopulation[@type=$fallback]/@populationPercent"/>
				<xsl:if test="$registry/language[subtag=$fallback]">
					<option value="{translate(@type,'_','-')}" percent="{$rootPercent}" sort="{concat(normalize-space($rootLang/text()),' &#8594; ',$name)}">
						<xsl:if test="$likelyLanguage=@type">
							<xsl:attribute name="selected">selected</xsl:attribute>
						</xsl:if>
						<xsl:value-of select="concat('&#160;&#160;&#160;&#160;',$name)"/>
					</option>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="$registry/language[subtag=current()/@type]">
					<option value="{@type}" percent="" sort="{$name}">
						<xsl:if test="$likelyLanguage=@type">
							<xsl:attribute name="selected">selected</xsl:attribute>
						</xsl:if>
						<xsl:value-of select="$name"/>
					</option>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
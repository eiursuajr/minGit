<?xml version='1.0'?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- Transform on CLDR/common/main/{$lang}.xml -->

<xsl:import href="territoryTree.xsl"/>

<xsl:output method="html" version="1.0" indent="yes" encoding="utf-8"/>

<!--<xsl:include href="includeForList.xsl"/>-->

<xsl:param name="labelMultinational" select="'Multinational Regions'"/> <!-- localize -->
<xsl:param name="labelCountries" select="'Countries'"/> <!-- localize -->

<xsl:key name="territory" match="/ldml/localeDisplayNames/territories/territory" use="@type"/>

<xsl:template match="/">
	<select id="{$controlId}" name="Territory" size="10" lang="{$lang}">
		<xsl:if test="string-length($prompt) &gt; 0">
			<option value=""><xsl:value-of select="$prompt"/></option>
		</xsl:if>
		<optgroup label="{$labelMultinational}">
			<xsl:variable name="regionTree_rtf">
				<xsl:apply-imports/>
			</xsl:variable>
			<xsl:variable name="regionTree" select="msxsl:node-set($regionTree_rtf)/*"/>
			<xsl:apply-templates select="$regionTree//li"/>
		</optgroup>
		<optgroup label="{$labelCountries}">
			<xsl:variable name="list">
				<xsl:variable name="territories" select="/ldml/localeDisplayNames/territories/territory[not(@type=$data/territoryContainment/group/@type)]"/>
				<xsl:choose>
					<xsl:when test="$subset='recommended' and string-length($selectedLanguageOnly) &gt; 0 and $selectedLanguageOnly != 'und'">
						<xsl:variable name="info" select="$data/languageData/language[@type=$selectedLanguageOnly]/@territories"/>
						<xsl:choose>
							<xsl:when test="count($info) &gt; 0">
								<xsl:apply-templates select="$territories[contains($info,@type) or @type=$likelyTerritory]"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:apply-templates select="$territories"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="$subset='common' or $subset='recommended'">
						<xsl:variable name="coverage" select="$meta/metadata/coverageAdditions/territoryCoverage"/>
						<!--
			            <territoryCoverage type="basic" values="BR CN DE GB FR IN IT JP RU US"/>           
			            <territoryCoverage type="moderate" values="ES BE SE TR AT ID SA NO DK PL ZA GR FI IE PT TH HK TW 419"/>
						-->
						<xsl:variable name="common" select="concat($coverage[@type='basic']/@values,' ',$coverage[@type='moderate']/@values)"/>
						<xsl:apply-templates select="$territories[contains($common,@type) or @type=$likelyTerritory]"/>
					</xsl:when>
					<xsl:when test="$subset='all'">
						<xsl:apply-templates select="$territories"/>
					</xsl:when>
				</xsl:choose>
			</xsl:variable>
			<xsl:for-each select="msxsl:node-set($list)/option">
				<xsl:sort select="text()" lang="{$lang}" />
				<xsl:copy-of select="."/>
			</xsl:for-each>
		</optgroup>
	</select> 
</xsl:template>

<xsl:template match="territory[not(@alt) or @alt='short']">
	<xsl:if test="not(following-sibling::territory[@type=current()/@type and @alt='short'])">
		<xsl:variable name="name" select="normalize-space(text())"/>
		<xsl:if test="string-length($name) &gt; 0">
			<xsl:if test="$registry/region[subtag=current()/@type]">
				<option value="{@type}">
					<xsl:if test="$selectedTerritory=@type">
						<xsl:attribute name="selected">selected</xsl:attribute>
					</xsl:if>
					<xsl:value-of select="$name"/>
				</option>
			</xsl:if>
		</xsl:if>
	</xsl:if>
</xsl:template>

<xsl:template match="li">
	<xsl:variable name="indent">
		<xsl:variable name="remaining" select="count(following-sibling::li)"/>
		<xsl:for-each select="ancestor-or-self::li[position()!=last()]">
			<xsl:variable name="remainingThisLevel" select="count(following-sibling::li)"/>
			<xsl:choose>
				<xsl:when test="position() != last() and 0 = $remainingThisLevel">
					<xsl:value-of select="'&#160;&#160;&#160;&#160;&#160;'"/> <!-- spaces -->
				</xsl:when>
				<xsl:when test="position() != last()">
					<xsl:value-of select="'&#x2502;&#160;&#160;&#160;'"/> <!-- vert bar -->
				</xsl:when>
				<xsl:when test="0 = $remaining">
					<xsl:value-of select="'&#x2514;&#x2500;&#160;'"/> <!-- elbow -->
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="'&#x251C;&#x2500;&#160;'"/> <!-- T bar -->
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="name" select="normalize-space(text())"/>
	<xsl:if test="string-length($name) &gt; 0">
		<option value="{@id}">
			<xsl:if test="$selectedTerritory=@id">
				<xsl:attribute name="selected">selected</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="concat($indent,$name)"/>
		</option>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:param name="controlId"/> <!-- id of the SELECT element -->
<xsl:param name="subset" select="'recommended'"/> <!-- 'recommended', 'common' or 'all' -->
<xsl:param name="selectedLanguage" select="'und'"/> <!-- may be gg-CC or gg-Ssss format, e.g., en-CA, zh-Hans -->
<xsl:param name="selectedScript" select="''"/>
<xsl:param name="selectedTerritory" select="''"/>
<xsl:param name="lang" select="'en'"/>
<xsl:param name="prompt" select="''"/> <!-- localize "(Select)" -->

<xsl:variable name="selectedLanguageOnly">
	<xsl:choose>
		<xsl:when test="contains($selectedLanguage,'-')">
			<xsl:value-of select="substring-before($selectedLanguage,'-')"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$selectedLanguage"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:variable>
<xsl:variable name="selectedLanguageCldr" select="translate($selectedLanguage,'-','_')"/> <!-- may be gg_CC or gg_Ssss format, e.g., en_CA, zh_Hans -->

<!-- http://www.langtag.net/registries/language-subtag-registry.xml -->
<xsl:param name="registryFile" select="'language-subtag-registry.xml'"/>
<xsl:variable name="registry" select="document($registryFile)/registry"/>

<xsl:param name="cldrDataFile" select="'CLDR/common/supplemental/supplementalData.xml'"/>
<xsl:variable name="data" select="document($cldrDataFile)/supplementalData"/>

<xsl:param name="cldrMetaFile" select="'CLDR/common/supplemental/supplementalMetaData.xml'"/>
<xsl:variable name="meta" select="document($cldrMetaFile)/supplementalData"/>

<xsl:param name="cldrLikelyFile" select="'CLDR/common/supplemental/likelySubtags.xml'"/>
<xsl:variable name="likely" select="document($cldrLikelyFile)/supplementalData/likelySubtags/likelySubtag"/>
<xsl:variable name="likelySubtag">
	<xsl:variable name="language_script" select="concat($selectedLanguageOnly,'_',$selectedScript)"/>
	<xsl:variable name="language_territory" select="concat($selectedLanguageOnly,'_',$selectedTerritory)"/>
	<xsl:variable name="script_territory" select="concat($selectedLanguageOnly,'_',$selectedScript,'_',$selectedTerritory)"/>
	<xsl:choose>
		<xsl:when test="contains($selectedLanguageCldr,'_') and $likely[@from=$selectedLanguageCldr]">
			<!-- $selectedLanguageCldr format: gg_CC or gg_Ssss, ie, language_territory -->
			<xsl:value-of select="$likely[@from=$selectedLanguageCldr]/@to"/>
		</xsl:when>
		<xsl:when test="$likely[@from=$script_territory]">
			<xsl:value-of select="$likely[@from=$script_territory]/@to"/>
		</xsl:when>
		<xsl:when test="$likely[@from=$language_script]">
			<xsl:value-of select="$likely[@from=$language_script]/@to"/>
		</xsl:when>
		<xsl:when test="$likely[@from=$language_territory]">
			<xsl:value-of select="$likely[@from=$language_territory]/@to"/>
		</xsl:when>
		<xsl:when test="$likely[@from=$selectedLanguageOnly]">
			<xsl:value-of select="$likely[@from=$selectedLanguageOnly]/@to"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$likely[@from='und']/@to"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:variable> 

<xsl:variable name="likelyLanguage">
	<xsl:choose>
		<xsl:when test="string-length($selectedLanguage) = 0">
			<!-- use empty language if language is explicity empty -->
			<xsl:value-of select="''"/>
		</xsl:when>
		<xsl:when test="$selectedLanguageCldr != 'und'">
			<xsl:value-of select="$selectedLanguageCldr"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="substring-before($likelySubtag,'_')"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:variable>

<xsl:variable name="likelyScript">
	<xsl:choose>
		<xsl:when test="string-length($selectedLanguage) = 0">
			<!-- use given script if language is explicity empty -->
			<xsl:value-of select="$selectedScript"/>
		</xsl:when>
		<xsl:when test="string-length($selectedScript) &gt; 0">
			<xsl:value-of select="$selectedScript"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="substring-before(substring-after($likelySubtag,'_'),'_')"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:variable>

<xsl:variable name="likelyTerritory">
	<xsl:choose>
		<xsl:when test="string-length($selectedLanguage) = 0">
			<!-- use given territory if language is explicity empty -->
			<xsl:value-of select="$selectedTerritory"/>
		</xsl:when>
		<xsl:when test="string-length($selectedTerritory) &gt; 0">
			<xsl:value-of select="$selectedTerritory"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="substring-after(substring-after($likelySubtag,'_'),'_')"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:variable>

</xsl:stylesheet>
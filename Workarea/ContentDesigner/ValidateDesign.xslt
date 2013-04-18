<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" extension-element-prefixes="msxsl js" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript">

<xsl:output method="text" encoding="utf-8"/>

	<xsl:param name="LangType" select="''"/>
	<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=ValidateSpec&amp;LangType=',$LangType)"/>

<xsl:variable name="localeXml" select="document($localeUrl)/*" />

<!--
	Run on content
-->

<xsl:include href="template_getDesignName.xslt"/>

<xsl:template match="/">
	<xsl:apply-templates />
</xsl:template>

<xsl:template match="@ektdesignns_calculate|@ektdesignns_normalize|@ektdesignns_validate">
	<xsl:if test="starts-with(.,'xpathr:') and contains(.,'{') and contains(.,'}')">
		<xsl:value-of select="$localeXml/data[@name='sFNVar']/value/text()"/> <!-- 'An expression still contains a field name variable' -->
		<xsl:value-of select="': '"/>
		<xsl:value-of select="substring-before(substring-after(.,'{'),'}')"/> <!-- field name variable -->
		<xsl:value-of select="'&#13;&#10;'"/>
		<xsl:value-of select="translate($localeXml/data[@name='dlgName']/value/text(),'&amp;:','')"/> <!-- 'Name' -->
		<xsl:value-of select="': '"/>
		<xsl:for-each select=".."> <!-- set context node -->
			<xsl:call-template name="getDesignName"/> <!-- field name -->
		</xsl:for-each>
		<xsl:value-of select="'&#13;&#10;'"/>

		<xsl:variable name="caption">
			<xsl:choose>
				<xsl:when test="name()='ektdesignns_calculate' or name()='ektdesignns_normalize'">
					<xsl:value-of select="$localeXml/data[@name='valFormula']/value/text()"/> <!-- 'Formula' -->
				</xsl:when>
				<xsl:when test="name()='ektdesignns_validate'">
					<xsl:value-of select="$localeXml/data[@name='valConditn']/value/text()"/> <!-- 'Condition' -->
				</xsl:when>
			</xsl:choose>
		</xsl:variable>

		<xsl:value-of select="translate($caption,'&amp;:','')"/> <!-- 'Formula' or 'Condition' -->
		<xsl:value-of select="': '"/>
		<xsl:value-of select="substring-after(.,'xpathr:')" disable-output-escaping="yes"/> <!-- xpath expression -->
		<xsl:value-of select="'&#13;&#10;'"/> <!-- 3 new lines indicates separation b/n error messages -->
		<xsl:value-of select="'&#13;&#10;'"/>
		<xsl:value-of select="'&#13;&#10;'"/>
	</xsl:if>
</xsl:template>

<xsl:template match="*">
	<xsl:apply-templates select="@*|*"/>
</xsl:template>

<xsl:template match="@*"/>
<xsl:template match="text()"/>

</xsl:stylesheet>
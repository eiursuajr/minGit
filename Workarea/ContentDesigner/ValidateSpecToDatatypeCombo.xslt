<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" />

	<xsl:param name="validationName" select="'plaintext'"/>
	<xsl:param name="LangType" select="''"/>
	<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=ValidateSpec&amp;LangType=',$LangType)"/>

<xsl:variable name="localeXml" select="document($localeUrl)/*" />
<!--

Run on ValidateSpec.xml to feed RAD Combobox.

Outputs 

<Items name="plaintext">
  <Item data="none" text="No Validation"/>
  ...
</Items>

-->

<xsl:template match="/">
<Items name="{$validationName}">
	<xsl:apply-templates select="datadesign/validation[@name=$validationName]" />
</Items>
</xsl:template>

<xsl:template match="selections[@name='datatype']/listchoice">
	<xsl:variable name="caption">
		<xsl:call-template name="getCaption"/>
	</xsl:variable>
	<Item Value="{./@value}" Text="{$caption}" />
</xsl:template>

<xsl:template name="getCaption">
	<xsl:param name="context" select="."/>
	<xsl:variable name="localeRef" select="$context/@localeRef" />
	<xsl:variable name="localizedText" select="$localeXml/data[@name=$localeRef]/value/text()"/>
	<xsl:choose>
		<xsl:when test="string-length($localizedText) &gt; 0">
			<xsl:value-of select="$localizedText"/>
		</xsl:when>
		<xsl:when test="$context/text()">
			<xsl:value-of select="$context/text()"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$localeRef"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="text()"/>
</xsl:stylesheet>
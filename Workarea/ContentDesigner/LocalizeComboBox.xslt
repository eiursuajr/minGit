<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" />

	<xsl:param name="bExamples" select="'true'"/>
	<xsl:param name="LangType" select="''"/>
	<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=CalculatedFieldExamples&amp;LangType=',$LangType)"/>

<xsl:variable name="localeXml" select="document($localeUrl)/*" />
<!--

Run on CalculatedFieldExamples.xml or RelevantExamples.xml to the output for feeding RAD Combobox.

Outputs 

<Items name="calculatedField">
  <Item Value="none" Text="No Validation"/>
  ...
</Items>

-->
<xsl:template match="/">
	<xsl:apply-templates />
</xsl:template>

<xsl:template match="Items">
	<Items name="{@name}">
		<xsl:apply-templates />
	</Items>
</xsl:template>

<xsl:template match="Item">
	<xsl:variable name="localeRef" select="@localeRef" />
	<xsl:variable name="caption" select="$localeXml/data[@name=$localeRef]/value/text()"/>
	<xsl:choose>
		<xsl:when test="$bExamples='true'">
			<Item Value="{@Value}" Text="{concat($caption,' (',@Value,')')}" />
		</xsl:when>
		<xsl:otherwise>
			<Item Value="{@Value}" Text="{$caption}" />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="text()"/>
</xsl:stylesheet>
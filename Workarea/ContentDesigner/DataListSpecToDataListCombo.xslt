<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" />

	<xsl:param name="LangType" select="''"/>
	<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=DataListSpec&amp;LangType=',$LangType)"/>

<xsl:variable name="localeXml" select="document($localeUrl)/*" />
<!--

Run on DataListSpec.xml to the output for feeding RAD Combobox.

Outputs 

<Items name="choicesfield">
  <Item Value="none" Text="No Validation"/>
  ...
</Items>

-->
<xsl:template match="/">
<Items name="choicesfield">
	<xsl:variable name="custom">
		<xsl:variable name="localizedText" select="$localeXml/data[@name='dlgCustom']/value/text()"/>
		<xsl:choose>
			<xsl:when test="string-length($localizedText) &gt; 0">
				<xsl:value-of select="$localizedText"/>
			</xsl:when>
			<xsl:otherwise>(Custom)</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<Item Value="" Text="{$custom}" />
	<xsl:apply-templates />
</Items>
</xsl:template>

<xsl:template match="datalist">
	<xsl:variable name="localeRef" select="./@localeRef" />
	<xsl:variable name="caption">
		<xsl:variable name="localizedText" select="$localeXml/data[@name=$localeRef]/value/text()"/>
		<xsl:choose>
			<xsl:when test="string-length($localizedText) &gt; 0">
				<xsl:value-of select="$localizedText"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@name"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<Item Value="{./@name}" Text="{$caption}" />
</xsl:template>

<xsl:template match="text()"/>
</xsl:stylesheet>
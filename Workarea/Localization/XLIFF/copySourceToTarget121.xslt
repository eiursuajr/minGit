<?xml version='1.0'?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl xlf" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
 xmlns="urn:oasis:names:tc:xliff:document:1.2.1" xmlns:xlf="urn:oasis:names:tc:xliff:document:1.2.1"
 xmlns:ekt="urn:ektron:xliff">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

<xsl:template match="/">
	<xsl:apply-templates/>
</xsl:template>

<xsl:template match="xlf:trans-unit/xlf:source">
	<xsl:copy-of select="."/>
	<target>
		<xsl:copy-of select="@*"/>
		<xsl:copy-of select="node()"/>
	</target>
</xsl:template>

<xsl:template match="xlf:bin-unit/xlf:bin-source">
	<xsl:copy-of select="."/>
	<bin-target>
		<xsl:copy-of select="@*"/>
		<xsl:copy-of select="node()"/>
	</bin-target>
</xsl:template>

<xsl:template match="*">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:apply-templates select="node()"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="*[not(node())]|area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="@*|text()">
	<xsl:copy/>
</xsl:template>

</xsl:stylesheet>
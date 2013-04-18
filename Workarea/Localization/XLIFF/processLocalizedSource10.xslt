<?xml version='1.0'?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:ekt="urn:ektron:xliff">

<!-- XPathDocument instance of the XLIFF document -->
<xsl:param name="localizedXliffSource"/>

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"
 doctype-public="-//XLIFF//DTD XLIFF//EN" 
 doctype-system="http://www.oasis-open.org/committees/xliff/documents/xliff.dtd"/>

<xsl:variable name="locSrc" select="msxsl:node-set($localizedXliffSource)"/>

<xsl:variable name="newline">
<xsl:text>
</xsl:text>
</xsl:variable>

<xsl:template match="/">
	<xsl:apply-templates/>
</xsl:template>

<xsl:template match="trans-unit[@id]/source">
	<xsl:variable name="id" select="../@id"/>
	<xsl:copy-of select="."/>
	<xsl:variable name="locSrcNode" select="$locSrc//trans-unit[@id=$id]/source/node()"/>
	<xsl:if test="$locSrcNode">
		<xsl:value-of select="$newline"/>
		<target>
			<xsl:copy-of select="@*"/>
			<xsl:copy-of select="$locSrcNode"/>
		</target>
	</xsl:if>
</xsl:template>

<xsl:template match="bin-unit/bin-source">
	<xsl:copy-of select="."/>
	<xsl:value-of select="$newline"/>
	<bin-target>
		<xsl:copy-of select="@*"/>
		<xsl:copy-of select="node()"/>
	</bin-target>
</xsl:template>
<!-- remove any existing TARGET elements -->
<xsl:template match="trans-unit[@id]/target"/>
<xsl:template match="bin-unit/bin-target"/>

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
<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="xml" omit-xml-declaration="yes"/>

<xsl:param name="resourceKey" />

<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="$resourceKey">
			<xsl:copy-of select="root/data[@name=$resourceKey]"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="root/data"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>
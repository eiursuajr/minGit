<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="xml" omit-xml-declaration="no" indent="yes" encoding="utf-8"/>
	
<xsl:template match="/">
	<ol>
		<xsl:apply-templates select="*/ol[1]/li"/>
	</ol>
</xsl:template>

<xsl:template match="li">
	<li>
		<xsl:attribute name="title">
			<xsl:choose>
				<xsl:when test="string-length(@title) &gt; 0">
					<xsl:value-of select="@title"/>				
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="."/>				
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
		<xsl:value-of select="."/>
	</li>
</xsl:template>

</xsl:stylesheet>
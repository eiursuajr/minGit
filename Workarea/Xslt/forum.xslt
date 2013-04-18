<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" version="1.0" encoding="UTF-8" omit-xml-declaration="yes"/>

	<xsl:param name="baseURL" select="''"/>

	<xsl:param name="LangType" select="''"/>



	<xsl:template match="@background|@dynsrc|@href|@src">
		<xsl:variable name="concatbaseurl">
			<xsl:value-of select="concat($baseURL, '/')"/>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="starts-with(.,'//') or contains(.,':')">
				<xsl:copy-of select="."/>
			</xsl:when>
			<xsl:when test="starts-with(.,'#')">
				<xsl:copy-of select="."/>
			</xsl:when>
			<xsl:when test="starts-with(.,'/')">
				<xsl:attribute name="{name()}">
					<!-- assert $baseURL ends in '/' -->
					<xsl:value-of select="concat($baseURL, .)"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="{name()}">
					<xsl:value-of select="concat($concatbaseurl, .)"/>
				</xsl:attribute>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>





	<!-- identity template -->
	<!-- identity with closing tags -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>

	<!-- See similar templates for identity without closing tags -->
	<!-- identity without closing tags -->
	<xsl:template match="area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" >
		<xsl:copy>
			<xsl:apply-templates select="@*" />
		</xsl:copy>
	</xsl:template>

	


</xsl:stylesheet>


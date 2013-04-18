<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

	<!-- Run on a document with 'datalist' elements -->

	<xsl:param name="name"/> <!-- required, name of datalist -->
	<xsl:param name="LangType" select="''"/>
	<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=DataListSpec&amp;LangType=',$LangType)"/>

	<xsl:variable name="localeXml" select="document($localeUrl)/*"/>

	<xsl:template match="datalist">
		<xsl:if test="@name=$name">
			<select>
				<xsl:for-each select="item">
					<xsl:variable name="caption">
						<xsl:choose>
							<xsl:when test="@localeRef">
								<xsl:value-of select="$localeXml/data[@name=current()/@localeRef]/value/text()"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="text()"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
					<option>
						<xsl:choose>
							<xsl:when test="@value">
								<xsl:attribute name="value"><xsl:value-of select="@value"/></xsl:attribute>
							</xsl:when>
							<xsl:otherwise>
								<xsl:attribute name="value"><xsl:value-of select="$caption"/></xsl:attribute>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:value-of select="$caption"/>
					</option>
				</xsl:for-each>
			</select>
		</xsl:if>
	</xsl:template>

	<xsl:template match="text()"/>
</xsl:stylesheet>
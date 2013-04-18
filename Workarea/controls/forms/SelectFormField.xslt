<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- Run on a fieldlist. Outputs list of HTML option elements. -->

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="value"/> <!-- current value -->
<xsl:param name="datatypes"/> <!-- space-delimited list, not specified means all -->
<xsl:param name="basetypes"/> <!-- space-delimited list, not specified means all -->

<xsl:template match="/">
	<xsl:apply-templates select="fieldlist/field"/>
</xsl:template>

<xsl:template match="field">
	<xsl:variable name="datatypelist" select="concat(' ',normalize-space($datatypes),' ')"/>
	<xsl:variable name="basetypelist" select="concat(' ',normalize-space($basetypes),' ')"/>
	<xsl:variable name="valid">
		<xsl:choose>
			<xsl:when test="$datatypes and $basetypes">
				<xsl:value-of select="contains($datatypelist,concat(' ',@datatype,' ')) and contains($basetypelist,concat(' ',@basetype,' '))"/>
			</xsl:when>
			<xsl:when test="$datatypes">
				<xsl:value-of select="contains($datatypelist,concat(' ',@datatype,' '))"/>
			</xsl:when>
			<xsl:when test="$basetypes">
				<xsl:value-of select="contains($basetypelist,concat(' ',@basetype,' '))"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'true'"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:if test="$valid='true'">
		<option value="{@name}">
			<xsl:if test="@name=$value">
				<xsl:attribute name="selected">selected</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="node()"/>
		</option>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
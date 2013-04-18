<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- uses global variable "fieldlist" -->

<xsl:template match="Data">
	<xsl:choose>
		<xsl:when test="$fieldlist">
			<xsl:variable name="dataItems" select="*"/>
			<xsl:for-each select="$fieldlist/field">
				<xsl:variable name="field" select="."/>
				<!-- find the corresponding data -->
				<xsl:variable name="data" select="$dataItems[local-name()=current()/@name]"/>
				<tr>
					<td title="{$field/@name}"><xsl:copy-of select="$field/node()"/></td> <!-- DisplayName -->
					<td>
					<xsl:call-template name="buildDataValue">
						<xsl:with-param name="field" select="$field"/>
						<xsl:with-param name="data" select="$data"/>
					</xsl:call-template>
					</td>
				</tr>
			</xsl:for-each>
		</xsl:when>
		<xsl:otherwise> <!-- no field definitions -->
			<xsl:for-each select="*">
			<tr>
				<td><xsl:copy-of select="local-name()"/></td> <!-- Name -->
				<td><xsl:copy-of select="./node()"/></td> <!-- Value -->
			</tr>
			</xsl:for-each>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:include href="template_buildDataValue.xslt"/>

</xsl:stylesheet>
<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- uses $tableHeaderClass -->

<xsl:variable name="userColumn" select="boolean(count(/Form/SubmittedData/User) &gt; 0)"/>

<xsl:template name="buildUserColumn">
	<xsl:param name="valign" select="'top'"/>
	<xsl:if test="$userColumn">
		<col valign="{$valign}" />
	</xsl:if>
</xsl:template>

<xsl:template name="buildUserHeader">
	<xsl:param name="caption" select="'Submitted By'"/>
	<xsl:param name="valign" select="'top'"/>
	<xsl:param name="rowspan"/>
	<xsl:if test="$userColumn">
		<th class="{$tableHeaderClass}" valign="{$valign}">
		<xsl:if test="$rowspan">
			<xsl:attribute name="rowspan"><xsl:value-of select="$rowspan"/></xsl:attribute>
		</xsl:if>
		<xsl:copy-of select="$caption"/>
		</th>
	</xsl:if>
</xsl:template>

<xsl:template name="buildUserCell">
	<xsl:param name="rowspan"/>
	<xsl:if test="$userColumn">
		<td>
		<xsl:if test="$rowspan">
			<xsl:attribute name="rowspan"><xsl:value-of select="$rowspan"/></xsl:attribute>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="User">
				<xsl:value-of select="concat(User/Name/LastName,', ',User/Name/FirstName)"/>
			</xsl:when>
			<xsl:otherwise>anonymous</xsl:otherwise>
		</xsl:choose>
		</td>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
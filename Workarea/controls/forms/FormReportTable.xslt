<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="template_buildHeader.xslt"/>
<xsl:import href="template_standardColumns.xslt"/>

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="canDelete"/>
<xsl:param name="includeHeading" select="'true'"/>
<xsl:variable name="tableHeaderClass" select="'headcell'"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:template match="Form">
	<xsl:if test="$includeHeading='true'">
	<xsl:call-template name="buildHeader"/>
	</xsl:if>
	<table border="1" cellpadding="4" cellspacing="0" class="ektronGrid ektronReport">
		<xsl:if test="$fieldlist">
			<xsl:call-template name="buildStandardColumnGroup"/>
			<xsl:for-each select="$fieldlist/field">
				<col valign="top">
				<xsl:if test="@basetype='number'">
					<xsl:attribute name="align">right</xsl:attribute>
				</xsl:if>
				</col>
			</xsl:for-each>
		</xsl:if>
		<xsl:if test="$includeHeading='true'">
		<thead>
			<tr>
				<xsl:call-template name="buildStandardHeaders"/>
				<xsl:choose>
					<xsl:when test="$fieldlist">
						<xsl:for-each select="$fieldlist/field">
							<th class="{$tableHeaderClass}" title="{@name}"><xsl:copy-of select="node()"/></th>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise> <!-- no field definitions -->
						<xsl:for-each select="SubmittedData[1]/Data/*">
							<th class="{$tableHeaderClass}"><xsl:copy-of select="local-name()"/></th>
						</xsl:for-each>
					</xsl:otherwise>
				</xsl:choose>
			</tr>
		</thead>
		</xsl:if>
		<tbody>
			<xsl:apply-templates select="SubmittedData"/>
		</tbody>
	</table>
</xsl:template>

<xsl:template match="SubmittedData">
	<tr>
		<xsl:call-template name="buildStandardCells"/>
		<xsl:choose>
			<xsl:when test="$fieldlist">
				<xsl:variable name="dataItems" select="Data/*"/>
				<xsl:for-each select="$fieldlist/field">
					<xsl:variable name="field" select="."/>
					<!-- find the corresponding data -->
					<xsl:variable name="data" select="$dataItems[local-name()=current()/@name]"/>
					<td>
					<xsl:call-template name="buildDataValue">
						<xsl:with-param name="field" select="$field"/>
						<xsl:with-param name="data" select="$data"/>
					</xsl:call-template>
					</td>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise> <!-- no field definitions -->
				<xsl:for-each select="Data/*">
					<td><xsl:copy-of select="./node()"/></td>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>
	</tr>
</xsl:template>

<xsl:include href="template_buildDataValue.xslt"/>

</xsl:stylesheet>
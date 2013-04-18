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
		<xsl:call-template name="buildStandardColumnGroup"/>
		<col valign="top"/>
		<col valign="top" />
		<xsl:if test="$includeHeading='true'">
		<thead>
			<tr>
				<xsl:call-template name="buildStandardHeaders"/>
				<th class="{$tableHeaderClass}">Field</th>
				<th class="{$tableHeaderClass}">Value</th>
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
		<xsl:call-template name="buildStandardCells">
			<xsl:with-param name="rowspan">
				<xsl:choose>
					<xsl:when test="$fieldlist">
						<xsl:value-of select="count($fieldlist/field) + 1"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="count(Data/*) + 1"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>
		</xsl:call-template>
		<xsl:apply-templates select="Data"/>
	</tr>
</xsl:template>

<xsl:include href="template_FormFieldValue.xslt"/>

</xsl:stylesheet>
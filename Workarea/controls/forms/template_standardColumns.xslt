<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- uses $canDelete -->
<!-- uses $tableHeaderClass -->

<!-- 

Standard columns:

1. Checkbox to delete (if $canDelete)
2. ID
3. Submitted By (if /Form/SubmittedData/User)
4. Date Submitted

-->

<xsl:import href="template_buildCellCheck.xslt"/>
<xsl:import href="template_buildUser.xslt"/>

<xsl:template name="buildStandardColumnGroup">
	<xsl:param name="valign" select="'top'"/>
	<xsl:if test="$canDelete='true'">
		<col valign="{$valign}" />
	</xsl:if>
	<col valign="{$valign}" align="right" />
	<xsl:call-template name="buildUserColumn">
		<xsl:with-param name="valign" select="$valign"/>
	</xsl:call-template>
	<col valign="{$valign}" />
</xsl:template>

<xsl:template name="buildStandardHeaders">
	<xsl:param name="valign"/>
	<xsl:param name="rowspan"/>
	<xsl:if test="$canDelete='true'">
		<xsl:call-template name="buildCellCheckAll">
			<xsl:with-param name="caption" select="'(Delete)'"/>
			<xsl:with-param name="valign" select="$valign"/>
			<xsl:with-param name="rowspan" select="$rowspan"/>
		</xsl:call-template>
	</xsl:if>
	<th class="{$tableHeaderClass}">
		<xsl:if test="$valign">
			<xsl:attribute name="valign"><xsl:value-of select="$valign"/></xsl:attribute>
		</xsl:if>
		<xsl:if test="$rowspan">
			<xsl:attribute name="rowspan"><xsl:value-of select="$rowspan"/></xsl:attribute>
		</xsl:if>
		<xsl:text>ID</xsl:text>
	</th>
	<xsl:call-template name="buildUserHeader">
		<xsl:with-param name="valign" select="$valign"/>
		<xsl:with-param name="rowspan" select="$rowspan"/>
	</xsl:call-template>
	<th class="{$tableHeaderClass}">
		<xsl:if test="$valign">
			<xsl:attribute name="valign"><xsl:value-of select="$valign"/></xsl:attribute>
		</xsl:if>
		<xsl:if test="$rowspan">
			<xsl:attribute name="rowspan"><xsl:value-of select="$rowspan"/></xsl:attribute>
		</xsl:if>
		<xsl:text>Date Submitted</xsl:text>
	</th>
</xsl:template>

<xsl:template name="buildStandardCells">
	<xsl:param name="rowspan"/>
	<xsl:if test="$canDelete='true'">
		<xsl:call-template name="buildCellCheckRow">
			<xsl:with-param name="name" select="concat('ektChk',Data/@form_data_id)"/>
			<xsl:with-param name="attributes">
				<!-- The attributes need an element as a container -->
				<!-- Can't use the name of the container element in an XPath of the node-set 
						because it does not work for some unknown reason. Use '*[1]' instead. -->
				<attribute-set rowspan="{$rowspan}"/> 
			</xsl:with-param>
		</xsl:call-template>
	</xsl:if>
	<td>
		<xsl:if test="$rowspan">
			<xsl:attribute name="rowspan"><xsl:value-of select="$rowspan"/></xsl:attribute>
		</xsl:if>
		<xsl:value-of select="Data/@form_data_id"/>
	</td>
	<xsl:call-template name="buildUserCell">
		<xsl:with-param name="rowspan" select="$rowspan"/>
	</xsl:call-template>
	<td>
		<xsl:if test="$rowspan">
			<xsl:attribute name="rowspan"><xsl:value-of select="$rowspan"/></xsl:attribute>
		</xsl:if>
		<xsl:value-of select="Date"/>
	</td>
</xsl:template>

</xsl:stylesheet>
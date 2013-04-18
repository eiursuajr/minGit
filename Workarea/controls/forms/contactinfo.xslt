<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="template_buildHeader.xslt"/>
<xsl:import href="template_standardColumns.xslt"/>

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="canDelete"/>
<xsl:param name="includeHeading" select="'true'"/>
<xsl:variable name="tableHeaderClass" select="'headcell'"/>

<xsl:template match="Form">
	<xsl:if test="$includeHeading='true'">
	<xsl:call-template name="buildHeader"/>
	</xsl:if>
	<table border="1" cellpadding="4" cellspacing="0">
		<xsl:call-template name="buildStandardColumnGroup"/>
		<col valign="top" />
		<col valign="top" />
		<col valign="top" />
		<col valign="top" />
		<xsl:if test="$includeHeading='true'">
		<thead>
			<tr>
				<xsl:call-template name="buildStandardHeaders"/>
				<th class="{$tableHeaderClass}">Address</th>
				<th class="{$tableHeaderClass}">Email</th>
				<th class="{$tableHeaderClass}">Business Phone</th>
				<th class="{$tableHeaderClass}">Home Phone</th>
			</tr>
		</thead>
		</xsl:if>
		<tbody>
			<xsl:apply-templates select="SubmittedData">
				<xsl:sort select="Data/Zip"/>
				<xsl:sort select="Data/Name"/>
			</xsl:apply-templates>
		</tbody>
	</table>
</xsl:template>

<xsl:template match="SubmittedData">
	<tr>
		<xsl:call-template name="buildStandardCells"/>
		<td>
		<xsl:value-of select="Data/Name"/><br />
		<xsl:value-of select="Data/Address"/><br />
		<xsl:value-of select="Data/City"/>
		<xsl:if test="string-length(Data/City) &gt; 0">
			<xsl:text>, </xsl:text>
		</xsl:if>
		<xsl:value-of select="Data/State"/>
		<xsl:if test="string-length(Data/State) &gt; 0">
			<xsl:text> </xsl:text>
		</xsl:if>
		<xsl:value-of select="Data/Country"/>
		<xsl:if test="string-length(Data/Country) &gt; 0">
			<xsl:text> </xsl:text>
		</xsl:if>
		<xsl:value-of select="Data/Zip"/>
		</td>
		<td>&#160;<xsl:value-of select="Data/Email"/></td>
		<td>&#160;<xsl:value-of select="Data/BusinessPhone"/></td>
		<td>&#160;<xsl:value-of select="Data/HomePhone"/></td>
	</tr>
</xsl:template>

</xsl:stylesheet>
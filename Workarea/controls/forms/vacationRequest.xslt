<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="template_buildHeader.xslt"/>

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="canDelete"/>
<xsl:param name="includeHeading" select="'true'"/>
<xsl:variable name="tableHeaderClass" select="'headcell'"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:template match="Form">
	<xsl:if test="$includeHeading='true'">
	<xsl:call-template name="buildHeader"/>
	</xsl:if>
	<table border="1" cellpadding="4" cellspacing="0">
		<col valign="top" />
		<col valign="top" />
		<col valign="top" align="right" />
		<xsl:if test="$includeHeading='true'">
		<thead>
			<tr>
				<th class="{$tableHeaderClass}">Name</th>
				<th class="{$tableHeaderClass}">Reason</th>
				<th class="{$tableHeaderClass}">Days (with pay)</th>
			</tr>
		</thead>
		</xsl:if>
		<tbody>
		<xsl:for-each select="SubmittedData[not(Data/Name=preceding-sibling::SubmittedData/Data/Name)]">
			<xsl:sort select="Data/Name"/>
			<xsl:variable name="data" select="Data"/>
			<xsl:for-each select="$fieldlist/datalist[@name=$fieldlist/field[@name='Reason']/@datalist]/item">
			<tr>
				<xsl:if test="position()=1">
					<td rowspan="{last()}"><xsl:value-of select="$data/Name"/></td>
				</xsl:if>
				<td><xsl:copy-of select="./node()"/></td>
				<td><xsl:value-of select="sum($data/../../SubmittedData/Data[Name=$data/Name and Reason=current()/@value]/day_pay[number(.) &gt; 0])"/></td>
			</tr>
			</xsl:for-each>
		</xsl:for-each>
		</tbody>
	</table>
</xsl:template>

</xsl:stylesheet>
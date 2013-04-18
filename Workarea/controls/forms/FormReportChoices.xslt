<?xml version='1.0'?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:import href="template_buildHeader.xslt"/>

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:variable name="largeListSize" select="5"/>

<xsl:variable name="tableHeaderClass" select="'headcell'"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:template match="Form">
	<xsl:call-template name="buildHeader"/>
	<table border="1" cellpadding="4" cellspacing="0" class="ektronGrid ektronReport">
		<col valign="top"/>
		<col valign="top"/>
		<col valign="top" align="right" />
		<thead>
			<tr>
				<th class="{$tableHeaderClass}">Field</th>
				<th class="{$tableHeaderClass}">Value</th>
				<th class="{$tableHeaderClass}">Count</th>
			</tr>
		</thead>
		<tbody>
		<xsl:variable name="data" select="SubmittedData/Data"/>
		<xsl:for-each select="$fieldlist/field[@datalist]|$fieldlist/field[@datatype='boolean']">
			<xsl:sort select="."/>
			<xsl:variable name="field" select="."/>
			<xsl:choose>
				<xsl:when test="@datalist">
					<!-- omit values whose count is zero if number of possible values is $largeListSize or more -->
					<xsl:variable name="valuelist-rtf">
						<xsl:for-each select="$fieldlist/datalist[@name=$field/@datalist]/item">
							<xsl:variable name="count" select="count($data/*[name()=$field/@name and string(.)=current()/@value])"/>
							<xsl:if test="number($count) &gt; 0 or last() &lt; $largeListSize">
								<xsl:copy>
									<xsl:attribute name="ektdesignns_count"><xsl:value-of select="$count"/></xsl:attribute>
									<xsl:copy-of select="@*"/>
									<xsl:copy-of select="node()"/>
								</xsl:copy>
							</xsl:if>
						</xsl:for-each>
					</xsl:variable>

					<xsl:for-each select="msxsl:node-set($valuelist-rtf)/*">
						<xsl:sort select="."/>
						<tr>
							<xsl:if test="position()=1">
								<td rowspan="{last()}" title="{$field/@name}"><xsl:copy-of select="$field/node()"/></td>
							</xsl:if>
							<td><xsl:copy-of select="./node()"/></td>
							<td><xsl:value-of select="@ektdesignns_count"/></td>
						</tr>
					</xsl:for-each>
				</xsl:when>
				<xsl:otherwise>
					<tr>
						<td rowspan="2" title="{$field/@name}"><xsl:copy-of select="$field/node()"/></td>
						<td>Checked</td>
						<td><xsl:value-of select="count($data/*[name()=$field/@name and string(.)!='false'])"/></td>
					</tr>
					<tr>
						<td>Unchecked</td>
						<td><xsl:value-of select="count($data/*[name()=$field/@name and string(.)='false'])"/></td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
		</tbody>
	</table>
</xsl:template>

</xsl:stylesheet>
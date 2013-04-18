<?xml version='1.0'?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:import href="template_buildHeader.xslt"/>

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:variable name="largeListSize" select="11"/>

<xsl:variable name="barWidth" select="250"/> <!-- pixels -->
<xsl:variable name="barColor" select="'#3163BD'"/>

<xsl:variable name="tableHeaderClass" select="'headcell'"/>

<xsl:variable name="percentFormat" select="'0.00%'"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:template match="Form">
	<xsl:call-template name="buildHeader"/>
	<xsl:variable name="count" select="count(SubmittedData)"/>
	<xsl:variable name="data" select="SubmittedData/Data"/>
	<xsl:for-each select="$fieldlist/field[(@datalist and @datatype!='selection') or @datatype='boolean']">
		<xsl:sort select="."/>
		<xsl:variable name="field" select="."/>
		<table border="0" cellpadding="1" cellspacing="0" class="ektronGrid">
			<col align="left" />
			<col align="left" />
			<thead>
				<tr>
					<th class="{$tableHeaderClass}" style="padding-top:5px;" align="left" colspan="2" title="{$field/@name}">
						<xsl:copy-of select="$field/node()"/>
						<xsl:value-of select="concat(' (',$count,' responses)')"/>
						<hr />
					</th>
				</tr>
			</thead>
			<tbody>
			<xsl:choose>
				<xsl:when test="@datalist">
					<!-- omit values whose count is zero if number of possible values is $largeListSize or more -->
					<xsl:variable name="valuelist-rtf">
						<xsl:for-each select="$fieldlist/datalist[@name=$field/@datalist]/item">
							<xsl:variable name="totalCount" select="count($data/*[name()=$field/@name and string(.)=current()/@value])"/>
							<xsl:if test="number($totalCount) &gt; 0 or last() &lt; $largeListSize">
								<xsl:copy>
									<xsl:attribute name="totalCount"><xsl:value-of select="$totalCount"/></xsl:attribute>
									<xsl:copy-of select="@*"/>
									<xsl:copy-of select="node()"/>
								</xsl:copy>
							</xsl:if>
						</xsl:for-each>
					</xsl:variable>

					<xsl:for-each select="msxsl:node-set($valuelist-rtf)/*">
						<xsl:call-template name="buildResult">
							<xsl:with-param name="value" select="@value"/>
							<xsl:with-param name="displayValue" select="./node()"/>
							<xsl:with-param name="total" select="@totalCount"/>
							<xsl:with-param name="count" select="$count"/>
						</xsl:call-template>
					</xsl:for-each>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="buildResult">
						<xsl:with-param name="displayValue" select="'Checked'"/>
						<xsl:with-param name="total" select="count($data/*[name()=$field/@name and string(.)!='false'])"/>
						<xsl:with-param name="count" select="$count"/>
					</xsl:call-template>
					<xsl:call-template name="buildResult">
						<xsl:with-param name="displayValue" select="'Unchecked'"/>
						<xsl:with-param name="total" select="count($data/*[name()=$field/@name and string(.)='false'])"/>
						<xsl:with-param name="count" select="$count"/>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
			</tbody>
		</table>
		<br />
	</xsl:for-each>
</xsl:template>

<xsl:template name="buildResult">
	<xsl:param name="value" select="''"/>
	<xsl:param name="displayValue" select="''"/>
	<xsl:param name="total" select="0"/>
	<xsl:param name="count" select="1"/>

	<xsl:variable name="percent" select="$total div $count"/>
	<tr>
		<td colspan="2" title="{$value}">
			<b><xsl:copy-of select="$displayValue"/></b>
		</td>
	</tr>
	<tr style="padding-bottom:5px;">
		<td title="{$total}/{$count}" style="padding-right:5px;">
			<xsl:value-of select="format-number($percent, $percentFormat)"/>
		</td>
		<td style="width:{$barWidth}px; height:20px">
			<div style="width:{round($percent * $barWidth)}px; height:100%; background-color:{$barColor}; border-left:solid 1px {$barColor};" 
				title="{format-number($percent, $percentFormat)}"></div>
		</td>
	</tr>
</xsl:template>

</xsl:stylesheet>
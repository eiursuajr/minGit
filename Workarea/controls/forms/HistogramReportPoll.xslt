<?xml version='1.0'?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

	<xsl:import href="template_buildHeader.xslt"/>
	<xsl:import href="template_buildFooter.xslt"/>

	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="displayType" select="2"/>
	<!-- displayType = 1 for Data Table
	 displayType = 2 for Bar Chart (default)
	 displayType = 3 for Pie Chart (display a chart img)
	 displayType = 4 for Combined Bar Chart with Percent -->

	<xsl:param name="appPath" select="string('/workarea/')"/>

	<xsl:variable name="barWidth" select="210"/>
	<!-- pixels -->

	<xsl:variable name="percentFormat" select="'0.00%'"/>

	<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

	<xsl:template match="Form">
		<link type="text/css" rel="stylesheet" href="{$appPath}csslib/reportchart.css" />
		<!--<xsl:call-template name="buildHeader"/>-->

		<xsl:choose>
			<xsl:when test="$displayType=3">
				<img src="{/*/SubmittedData/Data/ChartUrl/@src}"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="$fieldlist/field">
					<!--<xsl:sort select="."/>-->
					<xsl:variable name="count" select="/*/SubmittedData/Data/@totalCount"/>
					<xsl:variable name="data" select="/*/SubmittedData/Data"/>
					<xsl:variable name="field" select="."/>
					<table class="tblreport">
						<col class="tblcol1"/>
						<col class="tblcol2"/>
						<thead>
							<tr>
								<th colspan="2" class="headreport" title="{$field/@title}">
									<xsl:value-of select="$field/@title"/>

								</th>
							</tr>
						</thead>
						<tbody>
							<xsl:choose>
								<xsl:when test="@name">
									<xsl:for-each select="/*/SubmittedData/Data/*[name()=$field/@name]/item">
										<xsl:call-template name="buildResult">
											<xsl:with-param name="value" select="@value"/>
											<xsl:with-param name="displayValue" select="./node()"/>
											<xsl:with-param name="total" select="./@totalCount"/>
											<xsl:with-param name="count" select="/*/SubmittedData/Data/@totalCount"/>
											<xsl:with-param name="displayType" select="$displayType"/>
										</xsl:call-template>
									</xsl:for-each>
								</xsl:when>
								<xsl:otherwise>
									<xsl:call-template name="buildResult">
										<xsl:with-param name="displayValue" select="'Checked'"/>
										<xsl:with-param name="total" select="count($data/*[name()=$field/@name and string(.)!='false'])"/>
										<xsl:with-param name="count" select="$count"/>
										<xsl:with-param name="displayType" select="$displayType"/>
									</xsl:call-template>
									<xsl:call-template name="buildResult">
										<xsl:with-param name="displayValue" select="'Unchecked'"/>
										<xsl:with-param name="total" select="count($data/*[name()=$field/@name and string(.)='false'])"/>
										<xsl:with-param name="count" select="$count"/>
										<xsl:with-param name="displayType" select="$displayType"/>
									</xsl:call-template>
								</xsl:otherwise>
							</xsl:choose>
						</tbody>
					</table>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:variable name="count" select="/*/SubmittedData/Data/@totalCount"/>
		<span class="refreshlink">
			<xsl:value-of select="concat(' ',$count,' responses')"/>
		</span>
		<br />
		<xsl:call-template name="buildFooter"/>
	</xsl:template>

	<xsl:template name="buildResult">
		<xsl:param name="value" select="''"/>
		<xsl:param name="displayValue" select="''"/>
		<xsl:param name="total" select="0"/>
		<xsl:param name="count" select="1"/>
		<xsl:param name="displayType" select="2"/>

		<xsl:variable name="percent" select="$total div $count"/>
		<xsl:choose>
			<xsl:when test="$displayType=1">
				<tr>
					<!-- data table -->
					<td class="percentcell" title="{$total}/{$count}">
						<xsl:value-of select="format-number($percent, $percentFormat)"/>
					</td>
					<td class="optiontextcell" title="{$value}" >
						<xsl:copy-of select="$displayValue"/>
					</td>
				</tr>
			</xsl:when>
			<xsl:when test="$displayType=4">
				<!-- combine bar chart with percent -->
				<tr>
					<td colspan="2" class="optiontextcell" title="{$value}">
						<xsl:copy-of select="$displayValue"/> - <xsl:value-of select="format-number($percent, $percentFormat)"/>
					</td>
				</tr>
				<tr>
					<td colspan="2" class="barcell" title="{$total}/{$count}">
						<xsl:choose>
							<xsl:when test="$percent > 0">
								<img src="{$appPath}images/spacer.gif" class="resultbar" title="{format-number($percent, $percentFormat)}" style="width: {format-number($percent* .85, $percentFormat)}"/><span class="percentcell"></span>
							</xsl:when>
							<xsl:otherwise>
								<span class="percentcell"></span>
							</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>
			</xsl:when>
			<xsl:otherwise>
				<!-- default bar chart -->
				<tr>
					<td colspan="2" class="optiontextcell" title="{$value}">
						<xsl:copy-of select="$displayValue"/>
					</td>
				</tr>
				<tr>
					<td colspan="2" class="barcell" title="{$total}/{$count}">
						<xsl:choose>
							<xsl:when test="$percent > 0">
								<img src="{$appPath}images/spacer.gif" class="resultbar" title="{format-number($percent, $percentFormat)}" style="width: {format-number($percent, $percentFormat)}" />
							</xsl:when>
							<xsl:otherwise>
								<span class="spacercell">&#160;</span>
							</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
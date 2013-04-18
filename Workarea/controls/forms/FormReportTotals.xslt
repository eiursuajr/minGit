<?xml version='1.0'?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:import href="template_buildHeader.xslt"/>
<xsl:import href="template_standardColumns.xslt"/>

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="canDelete"/>
<xsl:param name="checkmarkUrl" select="'http://localhost/workarea/images/application/icon_greencheck.gif'"/>
<xsl:param name="includeHeading" select="'true'"/>

<xsl:variable name="largeListSize" select="5"/>

<xsl:variable name="tableHeaderClass" select="'headcell'"/>

<xsl:variable name="integerFormat" select="'0'"/>
<xsl:variable name="decimalFormat" select="'0.0#'"/>
<xsl:variable name="decimal2Format" select="'#,###,###,##0.00'"/>
<xsl:variable name="largeFormat" select="'#,###,###,##0.####'"/>
<xsl:variable name="preciseFormat" select="'0.0#####'"/>
<xsl:variable name="percentFormat" select="'0.##%'"/>

<xsl:decimal-format NaN="0"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:variable name="sparseDatalists-rtf">
	<!-- omit values whose count is zero if number of possible values is $largeListSize or more -->
	<xsl:variable name="dataItems" select="/Form/SubmittedData/Data/*"/>
	<xsl:for-each select="$fieldlist/field">
		<xsl:variable name="field" select="."/>
		<xsl:if test="@datalist">
			<!-- find the corresponding data -->
			<xsl:variable name="data" select="$dataItems[local-name()=current()/@name]"/>
			<xsl:variable name="datalist" select="$fieldlist/datalist[@name=$field/@datalist]"/>
			<datalist id="{position()}">
				<xsl:copy-of select="@*"/>
				<xsl:for-each select="$datalist/item">
					<xsl:variable name="count" select="count($data[normalize-space(.)=current()/@value])"/>
					<xsl:if test="$count &gt; 0 or last() &lt; $largeListSize">
						<xsl:if test="not(following-sibling::item[@value=current()/@value])">
							<item totalCount="{$count}">
								<xsl:copy-of select="@*"/>
								<xsl:copy-of select="node()"/>
							</item>
						</xsl:if>
					</xsl:if>
				</xsl:for-each>
			</datalist>
		</xsl:if>
	</xsl:for-each>
</xsl:variable>
<xsl:variable name="sparseDatalists" select="msxsl:node-set($sparseDatalists-rtf)/*"/>

<xsl:template match="Form">
	<xsl:if test="$includeHeading='true'">
		<xsl:call-template name="buildHeader"/>
	</xsl:if>
	<table border="1" cellpadding="4" cellspacing="0" class="ektronReport ektronGrid">
		<xsl:call-template name="buildStandardColumnGroup"/>
		<xsl:for-each select="$fieldlist/field">
			<xsl:variable name="field" select="."/>
			<xsl:choose>
				<xsl:when test="@datalist">
					<!-- definition of datalist is duplicated multiple times in this file. -->
					<!-- XSLT 1.0 does not support user-defined XPath functions. 
							Using a named template would require calling msxsl:node-set() 
							and generally be ugly. -->
					<xsl:variable name="id" select="position()"/>
					<xsl:variable name="datalist" select="$sparseDatalists[@id=$id]"/>
					<!-- xsl:variable name="datalist" select="$fieldlist/datalist[@name=$field/@datalist]"/ -->
					<xsl:for-each select="$datalist/item">
						<col valign="middle" align="center" />
					</xsl:for-each>
				</xsl:when>
				<xsl:when test="@datatype='boolean'">
					<col valign="middle" align="center" />
				</xsl:when>
				<xsl:when test="@basetype='number'">
					<col valign="top" align="right" />
				</xsl:when>
				<xsl:otherwise>
					<col valign="top" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
		<xsl:if test="$includeHeading='true'">
		<thead>
			<tr>
				<xsl:call-template name="buildStandardHeaders">
					<xsl:with-param name="valign" select="'middle'"/>
					<xsl:with-param name="rowspan" select="2"/>
				</xsl:call-template>
				<xsl:for-each select="$fieldlist/field">
					<xsl:variable name="field" select="."/>
					<xsl:choose>
						<xsl:when test="@datalist">
							<!-- definition of datalist is duplicated multiple times in this file. -->
							<xsl:variable name="id" select="position()"/>
							<xsl:variable name="datalist" select="$sparseDatalists[@id=$id]"/>
							<!-- xsl:variable name="datalist" select="$fieldlist/datalist[@name=$field/@datalist]"/ -->
							<xsl:if test="count($datalist/item) &gt; 0">
								<th class="{$tableHeaderClass}" valign="middle" title="{@name}" colspan="{count($datalist/item)}">
									<xsl:copy-of select="node()"/>
								</th>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<th class="{$tableHeaderClass}" valign="middle" title="{@name}" rowspan="2">
								<xsl:copy-of select="node()"/>
							</th>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
			</tr>
			<tr>
				<xsl:for-each select="$fieldlist/field">
					<xsl:variable name="field" select="."/>
					<xsl:if test="@datalist">
						<!-- definition of datalist is duplicated multiple times in this file. -->
						<xsl:variable name="id" select="position()"/>
						<xsl:variable name="datalist" select="$sparseDatalists[@id=$id]"/>
						<!-- xsl:variable name="datalist" select="$fieldlist/datalist[@name=$field/@datalist]"/ -->
						<xsl:for-each select="$datalist/item">
							<th class="{$tableHeaderClass}" valign="middle" title="{@value}">
								<xsl:value-of select="."/>
							</th>
						</xsl:for-each>
					</xsl:if>
				</xsl:for-each>
			</tr>
		</thead>
		</xsl:if>
		<tfoot>
			<xsl:variable name="count" select="count(SubmittedData)"/>
			<xsl:variable name="dataItems" select="SubmittedData/Data/*"/>
			<xsl:variable name="numStdCols">
				<xsl:variable name="stdHdrs">
					<xsl:call-template name="buildStandardHeaders"/>
				</xsl:variable>
				<xsl:value-of select="count(msxsl:node-set($stdHdrs)/*)"/>
			</xsl:variable>
			<tr>
				<th class="{$tableHeaderClass}" title="Rows: {$count}" align="right" colspan="{$numStdCols}">
					Total:
				</th>
				<xsl:for-each select="$fieldlist/field">
					<xsl:variable name="field" select="."/>
					<!-- find the corresponding data -->
					<xsl:variable name="data" select="$dataItems[local-name()=current()/@name]"/>
					<xsl:choose>
						<xsl:when test="@datalist">
							<!-- definition of datalist is duplicated multiple times in this file. -->
							<xsl:variable name="id" select="position()"/>
							<xsl:variable name="datalist" select="$sparseDatalists[@id=$id]"/>
							<!-- xsl:variable name="datalist" select="$fieldlist/datalist[@name=$field/@datalist]"/ -->
							<xsl:for-each select="$datalist/item">
								<xsl:variable name="total" select="@totalCount"/>
								<!-- xsl:variable name="total" select="count($data[normalize-space(.)=current()/@value])"/ -->
								<th class="{$tableHeaderClass}" title="{$total}/{$count}">
									<xsl:value-of select="$total"/>
								</th>
							</xsl:for-each>
						</xsl:when>
						<xsl:when test="@datatype='boolean'">
							<xsl:variable name="total" select="count($data[.='true'])"/>
							<th class="{$tableHeaderClass}" title="{$total}/{$count}">
								<xsl:value-of select="$total"/>
							</th>
						</xsl:when>
						<xsl:when test="@basetype='number'"> 
							<xsl:variable name="total" select="sum($data)"/>
							<th class="{$tableHeaderClass}" title="Rows: {$count}">
								<xsl:choose>
									<xsl:when test="starts-with(@datatype,'decnum2')">
										<xsl:value-of select="format-number($total, $decimal2Format)"/>
									</xsl:when>
									<xsl:when test="starts-with(@datatype,'percent')">
										<xsl:value-of select="format-number($total div 100, $percentFormat)"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="format-number($total, $largeFormat)"/>
									</xsl:otherwise>
								</xsl:choose>
							</th>
						</xsl:when>
						<xsl:otherwise>
							<th class="{$tableHeaderClass}">&#160;</th>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
			</tr>
			<tr>
				<th class="{$tableHeaderClass}" align="right" colspan="{$numStdCols}">
					Average (<xsl:value-of select="$count"/> rows):
				</th>
				<xsl:for-each select="$fieldlist/field">
					<xsl:variable name="field" select="."/>
					<!-- find the corresponding data -->
					<xsl:variable name="data" select="$dataItems[local-name()=current()/@name]"/>
					<xsl:choose>
						<xsl:when test="@datalist">
							<!-- definition of datalist is duplicated multiple times in this file. -->
							<xsl:variable name="id" select="position()"/>
							<xsl:variable name="datalist" select="$sparseDatalists[@id=$id]"/>
							<!-- xsl:variable name="datalist" select="$fieldlist/datalist[@name=$field/@datalist]"/ -->
							<xsl:for-each select="$datalist/item">
								<xsl:variable name="total" select="@totalCount"/>
								<!-- xsl:variable name="total" select="count($data[normalize-space(.)=current()/@value])"/ -->
								<xsl:variable name="percent" select="$total div $count"/>
								<th class="{$tableHeaderClass}" title="{$total}/{$count}">
									<xsl:value-of select="format-number($percent, $percentFormat)"/>
								</th>
							</xsl:for-each>
						</xsl:when>
						<xsl:when test="@datatype='boolean'">
							<xsl:variable name="total" select="count($data[.='true'])"/>
							<xsl:variable name="percent" select="$total div $count"/>
							<th class="{$tableHeaderClass}" title="{$total}/{$count}">
								<xsl:value-of select="format-number($percent, $percentFormat)"/>
							</th>
						</xsl:when>
						<xsl:when test="@basetype='number'"> 
							<xsl:variable name="total" select="sum($data)"/>
							<xsl:variable name="ave" select="$total div $count"/>
							<th class="{$tableHeaderClass}" title="Average: {format-number($ave, $preciseFormat)}">
								<xsl:choose>
									<xsl:when test="starts-with(@datatype,'decnum2')">
										<xsl:value-of select="format-number($ave, $decimal2Format)"/>
									</xsl:when>
									<xsl:when test="starts-with(@datatype,'percent')">
										<xsl:value-of select="format-number($ave div 100, $percentFormat)"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="format-number($ave, $decimalFormat)"/>
									</xsl:otherwise>
								</xsl:choose>
							</th>
						</xsl:when>
						<xsl:otherwise>
							<th class="{$tableHeaderClass}">&#160;</th>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
			</tr>
		</tfoot>
		<tbody>
			<xsl:apply-templates select="SubmittedData"/>
		</tbody>
	</table>
</xsl:template>

<xsl:template match="SubmittedData">
	<tr>
		<xsl:call-template name="buildStandardCells"/>
		<xsl:variable name="dataItems" select="Data/*"/>
		<xsl:for-each select="$fieldlist/field">
			<xsl:variable name="field" select="."/>
			<!-- find the corresponding data -->
			<xsl:variable name="data" select="$dataItems[local-name()=current()/@name]"/>
			<xsl:choose>
				<xsl:when test="$field/@datalist">
					<!-- definition of datalist is duplicated multiple times in this file. -->
					<xsl:variable name="id" select="position()"/>
					<xsl:variable name="datalist" select="$sparseDatalists[@id=$id]"/>
					<!-- xsl:variable name="datalist" select="$fieldlist/datalist[@name=$field/@datalist]"/ -->
					<xsl:for-each select="$datalist/item">
						<td title="{@value}">&#160;<xsl:if test="$data[normalize-space(.)=current()/@value]">
							<xsl:call-template name="buildCheckmark"/>
						</xsl:if>&#160;</td>
					</xsl:for-each>
				</xsl:when>
				<xsl:when test="@datatype='boolean'">
					<td>&#160;<xsl:if test="$data='true'">
						<xsl:call-template name="buildCheckmark"/>
					</xsl:if>&#160;</td>
				</xsl:when>
				<xsl:when test="starts-with(@datatype,'decnum2')">
					<td><xsl:value-of select="format-number($data, $decimal2Format)"/></td>
				</xsl:when>
				<xsl:when test="starts-with(@datatype,'percent')">
					<td><xsl:value-of select="format-number($data div 100, $percentFormat)"/></td>
				</xsl:when>
				<xsl:otherwise>
					<td>
					<xsl:call-template name="buildDataValue">
						<xsl:with-param name="field" select="$field"/>
						<xsl:with-param name="data" select="$data"/>
					</xsl:call-template>
					</td>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
	</tr>
</xsl:template>

<xsl:template name="buildCheckmark">
	<img src="{$checkmarkUrl}" alt="Checked" width="9" height="12" />
</xsl:template>

<xsl:include href="template_buildDataValue.xslt"/>

</xsl:stylesheet>
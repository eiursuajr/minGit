<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template name="buildDataValue">
	<xsl:param name="field"/>
	<xsl:param name="data"/>

	<xsl:if test="$field/@datalist or $field/@basetype='calendar' or $field/@datatype='date'">
		<xsl:variable name="valueCSV">
			<xsl:for-each select="$data">
				<xsl:if test="position() &gt; 1">, </xsl:if>
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:for-each>
		</xsl:variable>
		<xsl:attribute name="title"><xsl:value-of select="$valueCSV"/></xsl:attribute>
	</xsl:if>

	<xsl:choose>
		<xsl:when test="not($data)">
			<xsl:text>&#160;</xsl:text>
		</xsl:when>
		<xsl:when test="count($data)=0">
			<xsl:text>&#160;</xsl:text>
		</xsl:when>
		<xsl:otherwise>
			<xsl:for-each select="$data">
				<xsl:choose>
					<xsl:when test="$field/@datalist">
						<xsl:variable name="displayValue" select="$fieldlist/datalist[@name=$field/@datalist]/item[@value=normalize-space(current())]"/>
						<xsl:choose>
							<xsl:when test="$displayValue">
								<xsl:copy-of select="$displayValue/node()"/>
							</xsl:when>
							<xsl:when test="string-length(normalize-space(.))=0">
								<xsl:text>&#160;</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:copy-of select="./node()"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="$field/@basetype='calendar' or $field/@datatype='date'">
						<xsl:call-template name="buildDate"/>
					</xsl:when>
					<xsl:when test="string-length(normalize-space(.))=0">
						<xsl:text>&#160;</xsl:text>
					</xsl:when>
					<xsl:when test="$field/@basetype='textbox' or $field/@datatype='textarea'">
						<pre style="white-space:normal;word-wrap:break-word;"><xsl:copy-of select="./node()"/></pre>
					</xsl:when>
					<xsl:otherwise>
						<xsl:copy-of select="./node()"/>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="position() != last()">
					<br />
				</xsl:if>
			</xsl:for-each>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:include href="template_buildDate.xslt"/>

</xsl:stylesheet>
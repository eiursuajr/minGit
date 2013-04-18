<?xml version="1.0"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<!-- copies a result tree fragment a given number of times giving special care to HTML tag with no closing tags -->
<!-- this deprecates the used of xsl:copy-of -->

<xsl:template name="copyData">
	<xsl:param name="data"/>
	<xsl:param name="numberOfTimes" select="1"/>
	<xsl:param name="i" select="1"/>
	<!-- use system-property msxsl version to differentiate if it needs to use "msxsl:node-set". -->
	<xsl:variable name="msxslVersion" select="system-property('msxsl:version')"/>
	
	<xsl:if test="($i &lt;= $numberOfTimes) or not($numberOfTimes)">
		<!-- copies an RTF but adds a closing tag to empty elements with a few exceptions -->
		<xsl:choose>
			<xsl:when test="$msxslVersion='6' or $msxslVersion='5' or $msxslVersion='4' or $msxslVersion='3'">
				<xsl:apply-templates select="msxsl:node-set($data)" mode="resultTreeFragment"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="$data" mode="resultTreeFragment"/>			
			</xsl:otherwise>
		</xsl:choose>
	</xsl:if>
	<xsl:if test="$i &lt; $numberOfTimes">
		<xsl:call-template name="copyData">
			<xsl:with-param name="data" select="$data"/>
			<xsl:with-param name="numberOfTimes" select="$numberOfTimes"/>
			<xsl:with-param name="i" select="$i + 1"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<xsl:template match="@*|node()" mode="resultTreeFragment">
	<!-- identity with closing tags -->
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" mode="resultTreeFragment"/>
	</xsl:copy>
</xsl:template>

<!-- See similar templates for identity without closing tags -->
<xsl:template match="xsl:*[not(node())]|area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" mode="resultTreeFragment">
	<!-- identity without closing tags -->
	<xsl:copy>
		<xsl:apply-templates select="@*" mode="resultTreeFragment"/>
	</xsl:copy>
</xsl:template>

</xsl:stylesheet>
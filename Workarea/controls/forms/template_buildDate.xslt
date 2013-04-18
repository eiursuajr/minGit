<?xml version="1.0"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js ekext" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:ekext="urn:ektron:extension-object:common">

<xsl:template name="buildDate">
	<xsl:param name="date" select="string(.)"/> <!-- format: CCYY-MM-DD (ISO8601) -->
	<xsl:choose>
		<xsl:when test="string-length($date) = 10">
			<xsl:choose>
				<xsl:when test="function-available('ekext:formatISO8601Date')">
					<xsl:value-of select="ekext:formatISO8601Date($date)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$date"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$date"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!--
<msxsl:script language="JavaScript" implements-prefix="js"><![CDATA[
function formatISO8601Date(date)
{
	// asset("string" == typeof date && 10 == date.length)
	var oTempDate = new Date(parseInt(date.substr(0,4),10), parseInt(date.substr(5,2),10)-1, parseInt(date.substr(8,2),10));
	return (oTempDate.toLocaleDateString ? oTempDate.toLocaleDateString() : oTempDate.toLocaleString());
}
]]></msxsl:script>
-->

</xsl:stylesheet>
<?xml version="1.0"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js ekext" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:ekext="urn:ektron:extension-object:common">

<xsl:template name="getDesignName">
	<xsl:choose>
		<xsl:when test="@ektdesignns_name">
			<xsl:value-of select="translate(@ektdesignns_name,'/','')"/>
		</xsl:when>
		<xsl:when test="@ektdesignns_bind">
			<xsl:value-of select="ekext:extractName(string(@ektdesignns_bind))"/>
		</xsl:when>
	</xsl:choose>
</xsl:template>

<!--
<msxsl:script language="JavaScript" implements-prefix="js"><![CDATA[
function extractName(path)
{
	var sName = path;
	var p = sName.lastIndexOf('/');
	if (p >= 0)
	{
		sName = sName.substring(p + 1);
	}
	if ("@" == sName.substring(0,1))
	{
		sName = sName.substring(1);
	}
	return sName;
}
]]></msxsl:script>
-->

</xsl:stylesheet>
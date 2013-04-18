<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:template match="/">
	<xsl:variable name="views" select="ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_views"/>
	<xsl:choose>
		<xsl:when test="$views/ektdesignpackage_view[2]">
			<xsl:copy-of select="$views/ektdesignpackage_view[2]/node()"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="$views/ektdesignpackage_view[1]/node()"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>

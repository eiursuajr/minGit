<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template name="buildHeader">
	<h3 class="lblsFormTitle"><xsl:copy-of select="Title/node()"/></h3>
</xsl:template>

</xsl:stylesheet>
<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="template_buildHeader.xslt"/>

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:template match="Form">
	<xsl:call-template name="buildHeader"/>
	<textarea rows="20" cols="70">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:copy-of select="node()[not(starts-with(local-name(),'ektdesign'))]"/>
		</xsl:copy>
	</textarea>
</xsl:template>

</xsl:stylesheet>
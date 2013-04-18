<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="template_FormDataDisplay.xslt"/>

<xsl:output method="html" indent="yes"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:template match="/">
	<div id="FormDataSubmitted">
	<p>Data from form "<xsl:value-of select="*/FormTitle"/>" was received on <xsl:value-of select="*/Date"/>.</p>
	<p><xsl:copy-of select="*/FormDescription/node()"/></p>
	<xsl:apply-templates select="*/Data"/>
	<xsl:if test="*/Data/@form_data_id">
		<p>Form data id: <xsl:value-of select="*/Data/@form_data_id"/></p>
	</xsl:if>
	</div>
</xsl:template>

</xsl:stylesheet>